using System;
using System.Drawing;
using System.Runtime.InteropServices;
using static SDL2.SDL;

namespace Stas.ImGuiNet {
    /// <summary>
    /// A very basic SDL wrapper to handle creating a window and processing SDL events.
    /// </summary>
    public class SimpleSDLWindow :IDisposable {
        #region imports
        [DllImport("user32.dll")]
        static extern uint SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
        [DllImport("user32.dll")]
        static extern uint GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        static extern bool SetLayeredWindowAttributes(IntPtr hWnd, uint crKey, byte bAlpha, uint dwFlags);
        #endregion

        /// <summary>
        /// Creates a color key for use as a mask for <see cref="MakeTransparent(uint)"/>
        /// </summary>
        /// <param name="r">The red component of the mask color (0-1)</param>
        /// <param name="g">The green component of the mask color (0-1)</param>
        /// <param name="b">The blue component of the mask color (0-1)</param>
        /// <returns></returns>
        public static uint CreateColorKey(float r, float g, float b) {
            return ((uint)(r * 255.0f)) | ((uint)(g * 255.0f) << 8) | ((uint)(b * 255.0f) << 16);
        }

        public delegate void ProcessEventDelegate(ref SDL_Event sdlEvent);

        /// <summary>
        /// The SDL_Window pointer for this window.
        /// </summary>
        public IntPtr sdl_window_ptr { get; private set; }

        /// <summary>
        /// Whether an event has closed this window.
        /// </summary>
        public bool WantsClose { get; set; } = false;

        /// <summary>
        /// Delegate for providing user event handler methods that want to respond to SDL_Events.
        /// </summary>
        public ProcessEventDelegate OnSDLEvent { get; set; }
        /// <summary>
        /// Gets the HWND of this window for interop with Windows methods.
        /// </summary>
        /// <returns>This window's HWND</returns>
        public IntPtr GetHWnd() {
            if (sdl_window_ptr == IntPtr.Zero) {
                return IntPtr.Zero;
            }

            var sysWmInfo = new SDL_SysWMinfo();
            SDL_GetVersion(out sysWmInfo.version);
            SDL_GetWindowWMInfo(sdl_window_ptr, ref sysWmInfo);
            return sysWmInfo.info.win.window;
        }

        IntPtr transp_ptr;
        /// <summary>
        /// Creates a new SDL_Window with the given renderer attached.
        /// </summary>
        /// <param name="renderer">The renderer to attach to this window.</param>
        /// <param name="wcf">The creation parameters to use when building this window.</param>
        internal SimpleSDLWindow(SimpleD3D renderer, WindowCreateInfo wcf) {
            if(SDL_Init(SDL_INIT_VIDEO) != 0) {
                throw new Exception("SDL_Init error: " + SDL_GetError());
            }
            //https://wiki.libsdl.org/SDL_WindowFlags
            var windowFlags =  SDL_WindowFlags.SDL_WINDOW_BORDERLESS 
                             | SDL_WindowFlags.SDL_WINDOW_ALWAYS_ON_TOP //<--comment here for debug sdl_window_ptr
                             | SDL_WindowFlags.SDL_WINDOW_ALLOW_HIGHDPI 
                             | SDL_WindowFlags.SDL_WINDOW_SKIP_TASKBAR;
            sdl_window_ptr = SDL_CreateWindow(wcf.Title,0, 0, wcf.Width, wcf.Height, windowFlags);
            if(sdl_window_ptr == IntPtr.Zero) {
                SDL_Quit();
                throw new Exception("Failed to create window: " + SDL_GetError());
            }
            SDL_SysWMinfo info = new SDL_SysWMinfo();
            SDL_GetWindowWMInfo(sdl_window_ptr, ref info);
            transp_ptr = info.info.win.window;//mega importand get this right ptr
            InitTransparency(transp_ptr);
            SetOverlayClickable(false);
            renderer.AttachToWindow(this);
        }
        //https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getwindowlongptra
        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        private static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        static IntPtr GWL_clicable = IntPtr.Zero;
        static IntPtr GWL_not_clickable = IntPtr.Zero;
        const int GWL_EXSTYLE = -20;
        const int GWL_STYLE = -16;
        const int WS_EX_LAYERED = 0x80000;
        const int WS_EX_TRANSPARENT = 0x20;
        //https://stackoverflow.com/questions/23048993/sdl-fullscreen-translucent-background
        /// <summary>
        /// SDL2Window init.. 
        /// </summary>
        /// <param name="handle"></param>
        ///  <param name="b_on_top">Will be TOP all the time</param>
        void InitTransparency(IntPtr handle, bool b_on_top = true) {
            var curr_style = GetWindowLongPtr(handle, GWL_STYLE);
            GWL_clicable = GetWindowLongPtr(handle, GWL_EXSTYLE);
            GWL_not_clickable = new IntPtr( GWL_clicable.ToInt64() | WS_EX_LAYERED | WS_EX_TRANSPARENT);
            //Console.WriteLine("curr_style="+ Convert.ToString(GWL_clicable.ToInt64(), 2));

            Margins margins = Margins.FromRectangle(new Rectangle(-1, -1, -1, -1));
            DwmExtendFrameIntoClientArea(handle, ref margins);
        }
        /// <summary>
        /// Try set ImGui window be TOP all the time
        /// </summary>
        public void SetAlwaisOnTOP() {
            //GWL_clicable must be: 11000 => it's block
            GWL_clicable = new IntPtr(Convert.ToInt32("11000", 2));
            GWL_not_clickable = new IntPtr(GWL_clicable.ToInt64() | WS_EX_LAYERED | WS_EX_TRANSPARENT);
        }
        /// <summary>
        /// will DONT block the Visual studio window when a break point is triggered
        /// </summary>
        public void DontBlockVisualStudio() {
            //GWL_clicable must be:  10000
            GWL_clicable = new IntPtr(Convert.ToInt32("10000", 2));
            GWL_not_clickable = new IntPtr(GWL_clicable.ToInt64() | WS_EX_LAYERED | WS_EX_TRANSPARENT);
        }
        static bool isClickable = true;
        /// <summary>
        /// Enables (clickable) / Disables (not clickable) the SDL2Window keyboard/mouse inputs.
        /// NOTE: This function depends on InitTransparency being called when the SDL2Winhdow was created.
        /// </summary>
        /// <param name="handle">Veldrid window handle in IntPtr format.</param>
        /// <param name="WantClickable">Set to true if you want to make the window clickable otherwise false.</param>
        public void SetOverlayClickable(bool WantClickable) {
            if (!isClickable && WantClickable) {
                SetWindowLongPtr(transp_ptr, GWL_EXSTYLE, GWL_clicable);
                SetFocus(transp_ptr);
                isClickable = true;
            } else if (isClickable && !WantClickable) {
                SetWindowLongPtr(transp_ptr, GWL_EXSTYLE, GWL_not_clickable);
                isClickable = false;
            }
        }
       
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

        //https://docs.microsoft.com/en-us/windows/win32/winmsg/window-styles
        //https://docs.microsoft.com/en-us/windows/win32/winmsg/extended-window-styles

        [DllImport("dwmapi.dll")]
        private static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref Margins pMarInset);
        [StructLayout(LayoutKind.Sequential)]
        private struct Margins {
            private int left;
            private int right;
            private int top;
            private int bottom;

            public static Margins FromRectangle(Rectangle rectangle) {
                var margins = new Margins {
                    left = rectangle.Left,
                    right = rectangle.Right,
                    top = rectangle.Top,
                    bottom = rectangle.Bottom,
                };
                return margins;
            }
        }

        /// <summary>
        /// Basic SDL event loop to consume all events and handle window closure.
        /// User handlers from <see cref="OnSDLEvent"/> are invoked for every event.
        /// </summary>
        public void ProcessEvents() {
            while(SDL_PollEvent(out SDL_Event sdlEvent) != 0) {
                OnSDLEvent?.Invoke(ref sdlEvent);

                if(sdlEvent.type == SDL_EventType.SDL_QUIT) {
                    WantsClose = true;
                }
                else if(sdlEvent.type == SDL_EventType.SDL_WINDOWEVENT &&
                         sdlEvent.window.windowEvent == SDL_WindowEventID.SDL_WINDOWEVENT_CLOSE &&
                         sdlEvent.window.windowID == SDL_GetWindowID(sdl_window_ptr)) {
                    WantsClose = true;
                }
            }
        }

        internal void Show() {
            SDL_ShowWindow(sdl_window_ptr);
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
                if(sdl_window_ptr != IntPtr.Zero) {
                    SDL_DestroyWindow(sdl_window_ptr);
                }

                SDL_Quit();

                disposedValue = true;
            }
        }

        ~SimpleSDLWindow() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
