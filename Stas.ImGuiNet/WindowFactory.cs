using System;


namespace Stas.ImGuiNet {
    /// <summary>
    /// Simple wrapper for information necessary to create an application window
    /// </summary>
    public class WindowCreateInfo {
        /// <summary>
        /// The window title.  This will not be visible for fullscreen windows except in things like task manager.
        /// </summary>
        public string Title;
        /// <summary>
        /// The x location of the top left corner of the window.  Ignored for fullscreen windows.
        /// </summary>
        public int XPos;
        /// <summary>
        /// The y location of the top left corner of the window.  Ignored for fullscreen windows.
        /// </summary>
        public int YPos;
        /// <summary>
        /// The width of the window.  Ignored for fullscreen windows.
        /// </summary>
        public int Width = 2560;
        /// <summary>
        /// The height of the window.  Ignored for fullscreen windows.
        /// </summary>
        public int Height = 1440;
        /// <summary>
        /// Whether the window should be created fullscreen.  This is a borderless windowed mode and will not affect desktop resolution.
        /// Fullscreen windows are "always on top".
        /// </summary>
        public bool Fullscreen;
        /// <summary>
        /// An optional float[4] color key used to make any matching portion of the window's client area transparent.  For example, setting this to magenta will
        /// then make any area where you render magenta instead be fully transparent to the window(s) behind this one.
        /// Values are red, green, blue from 0 to 1.
        /// </summary>
        public float[] TransparentColor;
    }

    /// <summary>
    /// Factory used to create a SimpleSDLWindow set up to work with the selected renderer.
    /// </summary>
    public class WindowFactory {
        /// <summary>
        /// Creates a window configured for use with the specified renderer.
        /// </summary>
        /// <param name="renderer">The renderer to use with this window.</param>
        /// <param name="createInfo">The <see cref="WindowCreateInfo"/> specifying the details of window creation.</param>
        /// <returns></returns>
        public static SimpleSDLWindow CreateForRenderer(SimpleD3D renderer, WindowCreateInfo createInfo) {
            return new SimpleSDLWindow(renderer, createInfo);
        }
    }
}
