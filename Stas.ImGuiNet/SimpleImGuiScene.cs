using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using static SDL2.SDL;

namespace Stas.ImGuiNet {
     /// <summary>
    /// Simple wrapper to handle texture resources from different APIs, while accounting for resource freeing and ImGui interaction.
    /// </summary>
    public interface TextureWrap : IDisposable
    {
        /// <summary>
        /// A texture handle suitable for direct use with ImGui::Image() etc.
        /// </summary>
        IntPtr ImGuiHandle { get; }
        int Width { get; }
        int Height { get; }
    }
    /// <summary>
    /// Simple class to wrap everything necessary to use ImGui inside a window.
    /// Currently this always creates a new window rather than take ownership of an existing one.
    /// 
    /// Internally this uses SDL and DirectX 11 or OpenGL 3.2.  Rendering is tied to vsync.
    /// </summary>
    public class SimpleImGuiScene :IDisposable {
        /// <summary>
        /// The main application container window where we do all our rendering and input processing.
        /// </summary>
        public SimpleSDLWindow sdl_window { get; }

        /// <summary>
        /// The renderer backend being used to render into this window.
        /// </summary>
        public SimpleD3D Renderer { get; }
        
        /// <summary>
        /// Whether the user application has requested the system to terminate.
        /// </summary>
        public bool ShouldQuit { get; set; } = false;

        private FramerateLimit _framerateLimit;
        /// <summary>
        /// The method of framerate control used by the scene and renderer.
        /// The default behavior is <see cref="FramerateLimit.LimitType.Vsync"/>, which is greatly recommended unless you have a specific need to change it.
        /// </summary>
        public FramerateLimit FramerateLimit {
            get => _framerateLimit;
            set {
                _framerateLimit = value;
                Renderer.Vsync = _framerateLimit.Type == FramerateLimit.LimitType.Vsync ? true : false;

                if(_framerateLimit.Type == FramerateLimit.LimitType.FixedFPS) {
                    _isFramerateLimited = true;
                    // convert to ms here since that is what Sleep() uses
                    _targetFrameTime = 1000.0 / _framerateLimit.FPS;
                }
                else {
                    _isFramerateLimited = false;
                }
            }
        }

        private string _imguiIniPath = null;
        public string ImGuiIniPath {
            get { return _imguiIniPath; }
            set {
                _imguiIniPath = value;
                _imguiInput.SetIniPath(_imguiIniPath);
            }
        }

        public delegate void BuildUIDelegate();

        /// <summary>
        /// User methods invoked every ImGui frame to construct custom UIs.
        /// </summary>
        public BuildUIDelegate OnBuildUI;

        private bool _pauseWhenUnfocused;
        /// <summary>
        /// Whether rendering should be paused when the window is not active.  Window events will still be processed.
        /// This should help reduce processing when the overlay is not the focus, but obviously cannot be used
        /// if you are rendering dynamic data.
        /// </summary>
        public bool PauseWhenUnfocused {
            get => _pauseWhenUnfocused;
            set {
                _pauseWhenUnfocused = value;
                if(_pauseWhenUnfocused) {
                    OnSDLEvent += FocusHandler;
                }
                else {
                    OnSDLEvent -= FocusHandler;
                }
            }
        }

        // framerate limiting
        // many of these could be inferred or computed, but are cached to avoid unnecessary
        // processing in the render loop
        private bool _isFramerateLimited;
        private double _targetFrameTime;
        private ulong _lastFrameCounter;
        private readonly double _msPerTick;
        private FramerateLimit _savedFrameLimit;

        /// <summary>
        /// Delegate for providing user event handler methods that want to respond to SDL_Events.
        /// This is just a convenience wrapper around <see cref="SimpleSDLWindow.OnSDLEvent"/>.
        /// </summary>
        public SimpleSDLWindow.ProcessEventDelegate OnSDLEvent {
            get => sdl_window.OnSDLEvent;
            set { sdl_window.OnSDLEvent = value; }
        }

        // TODO: weak refs?
        private List<IDisposable> _allocatedResources = new List<IDisposable>();
        private bool _pauseRendering;

        // Not using interface for now, needs work
        private ImGui_Impl_SDL _imguiInput;

        /// <summary>
        /// Helper method to create a fullscreen transparent overlay that exits when pressing the specified key.
        /// </summary>
        /// <param name="rendererBackend">Which rendering backend to use.</param>
        /// <param name="closeOverlayKey">Which <see cref="SDL_Scancode"/> to listen for in order to exit the scene.  Defaults to <see cref="SDL_Scancode.SDL_SCANCODE_ESCAPE"/>.</param>
        /// <param name="transparentColor">A float[4] representing the background window color that will be masked as transparent.  Defaults to solid black.</param>
        /// <param name="enableRenderDebugging">Whether to enable debugging of the renderer internals.  This will likely greatly impact performance and is not usually recommended.</param>
        /// <returns></returns>
        public static SimpleImGuiScene CreateOverlay(string title_name,  float[] transparentColor = null, bool enableRenderDebugging = false) {
            var scene = new SimpleImGuiScene(
                new WindowCreateInfo { Title = title_name, Fullscreen = false,TransparentColor = transparentColor ?? new float[] { 0, 0, 0, 0 }  }, enableRenderDebugging);

            //// Add a simple handler for the close key, so user classes don't have to bother with events in most cases
            //scene.OnSDLEvent += (ref SDL_Event sdlEvent) => {
            //    if(sdlEvent.type == SDL_EventType.SDL_KEYDOWN && sdlEvent.key.keysym.scancode == closeOverlayKey) {
            //        scene.ShouldQuit = true;
            //    }
            //};

            return scene;
        }

        /// <summary>
        /// Creates a new window and a new renderer of the specified type, and initializes ImGUI.
        /// </summary>
        /// <param name="backend">Which rendering backend to use.</param>
        /// <param name="createInfo">Creation details for the window.</param>
        /// <param name="enableRenderDebugging">Whether to enable debugging of the renderer internals.  This will likely greatly impact performance and is not usually recommended.</param>
        public SimpleImGuiScene( WindowCreateInfo createInfo, bool enableRenderDebugging = false) {
            // cache this off since it should hopefully never change
            // inverted and *1000 to reduce math in the render loop to compute frame times in ms at the loss of a tiny bit of precision
            _msPerTick = 1000.0 / SDL_GetPerformanceFrequency();

            Renderer = new SimpleD3D(enableRenderDebugging);
            sdl_window = WindowFactory.CreateForRenderer(Renderer, createInfo);

            // This is the default beahvior anyway, but manually creating the object simplifies some checks
            FramerateLimit = new FramerateLimit(FramerateLimit.LimitType.Vsync);

            ImGui.CreateContext();

            _imguiInput = new ImGui_Impl_SDL(sdl_window.sdl_window_ptr);
            Renderer.ImGui_Init();
            sdl_window.OnSDLEvent += _imguiInput.ProcessEvent;
        }

        public unsafe Tuple<IntPtr, int, int> LoadImageToPtr(string path) {
            IntPtr res = IntPtr.Zero;
            int w = 0; int h = 0;
            using (var bmp = new Bitmap(path)) {
                w = bmp.Width; h = bmp.Height;
                var bmp_data = bmp.LockBits(new System.Drawing.Rectangle(0, 0, w, h),
                    System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);
                res = Renderer.LoadTextureFromPtr((byte*)bmp_data.Scan0, bmp.Width, bmp.Height, 4).NativePointer;
                bmp.UnlockBits(bmp_data);
            }
            return new Tuple<IntPtr, int, int>(res, w, h);
        }
        public unsafe Tuple<IntPtr, int, int> LoadImageToPtr(Bitmap bmp) {
            IntPtr res = IntPtr.Zero;
            int w = 0; int h = 0;
            using (bmp ) {
                w = bmp.Width; h = bmp.Height;
                var bmp_data = bmp.LockBits(new System.Drawing.Rectangle(0, 0, w, h),
                    System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);
                res = Renderer.LoadTextureFromPtr((byte*)bmp_data.Scan0, bmp.Width, bmp.Height, 4).NativePointer;
                bmp.UnlockBits(bmp_data);
            }
            return new Tuple<IntPtr, int, int>(res, w, h);
        }
        /// <summary>
        /// Performs a single-frame update of ImGui and renders it to the window.
        /// This method does not check any quit conditions.
        /// </summary>
        public void Update() {
            var frameStart = SDL_GetPerformanceCounter();
            // var fps = 1000.0 / ((frameStart - _lastFrameCounter) * _msPerTick);
            _lastFrameCounter = frameStart;
            sdl_window.ProcessEvents();

            if(!_pauseRendering) {
                Renderer.ImGui_NewFrame();

                SDL_GetWindowSize(sdl_window.sdl_window_ptr, out int width, out int height);
                _imguiInput.NewFrame(width, height);

                ImGui.NewFrame();
                OnBuildUI?.Invoke();
                ImGui.Render();

                Renderer.Clear();

                Renderer.ImGui_RenderDrawData(ImGui.GetDrawData());

                Renderer.Present();
            }

            if(_isFramerateLimited) {
                var frameTime = (double)(SDL_GetPerformanceCounter() - frameStart) * _msPerTick;

                // This is somewhat less precise than looping over a Sleep(0) and updating frameTime until we pass _targetFrameTime
                // but that method generally results in 'apparent' higher cpu, which somewhat defeats the purpose
                // In reality, it would yield if necessary, but I don't want to handle complaints that cpu usage appears to
                // go up when we render less often.  Our rendering is trivial enough that all the imprecisions in timing won't matter.
                var sleepTime = _targetFrameTime - frameTime;
                if(sleepTime > 0) {
                    Thread.Sleep((int)sleepTime);
                }
            }
        }

        /// <summary>
        /// Simple method to run the scene in a loop until the window is closed or the application
        /// requests an exit (via <see cref="ShouldQuit"/>)
        /// </summary>
        public void Run() {
            // For now we consider the window closing to be a quit request
            // while ShouldQuit is used for external/application close requests
            while(!sdl_window.WantsClose && !ShouldQuit) {
                Update();
            }
        }

        private void FocusHandler(ref SDL_Event sdlEvent) {
            if(!PauseWhenUnfocused)
                return;

            if(sdlEvent.type == SDL_EventType.SDL_WINDOWEVENT) {
                if(sdlEvent.window.windowEvent == SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_LOST) {
                    _pauseRendering = true;
                    // Manually limit updating if we are paused
                    // otherwise cpu usage can spike crazily as the update loop just spins super fast
                    if(FramerateLimit.Type != FramerateLimit.LimitType.FixedFPS) {
                        _savedFrameLimit = FramerateLimit;
                        // for now cap to 60.  This is somewhat arbitrary but should allow responsive window re-focus
                        // and seems to drop cpu to effectively 0
                        FramerateLimit = new FramerateLimit(FramerateLimit.LimitType.FixedFPS, 60);
                    }
                }
                else if(sdlEvent.window.windowEvent == SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_GAINED) {
                    _pauseRendering = false;
                    if(_savedFrameLimit != null) {
                        FramerateLimit = _savedFrameLimit;
                        _savedFrameLimit = null;
                    }
                }
            }
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing) {
            if(!disposedValue) {
                if(disposing) {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                Renderer.ImGui_Shutdown();
                _imguiInput.Dispose();

                ImGui.DestroyContext();

                _allocatedResources.ForEach(res => res.Dispose());
                _allocatedResources.Clear();

                // Probably not necessary, but may as well be nice and try to clean up in case we used anything
                // This is safe even if nothing from the library was used
                // We also never call IMG_Load() for now since it is done automatically where needed
                //IMG_Quit(); //same time this throw error

                Renderer?.Dispose();
                sdl_window?.Dispose();

                disposedValue = true;
            }
        }

        ~SimpleImGuiScene() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
