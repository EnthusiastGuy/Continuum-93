using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Last_Known_Reality.Reality
{
    public static class Config
    {
        // MacOS compatibility. Mac doesn't assume the relative path is in the game directory, rather in the user folder
        public static string APP_PATH;

        public static OSPlatform Unknown = OSPlatform.Create("Unknown");
        public static OSPlatform Platform;

        // Save game
        public static readonly int SAVES_AUTO = 10;
        public static readonly int SAVES_QUICK = 10;
        public static readonly int SAVES_NORMAL = 100;

        // This is the base resolution that will be changed depending on the screen ratio
        public static int BaseWidth = 1280;
        public static int BaseHeight = 540;

        public static int UIBufferWidth = BaseWidth * 8;
        public static int UIBufferHeight = BaseHeight * 8;

        public static int ViewportWidth;
        public static int ViewportHeight;

        public static float ViewportHorizontalRatio = 1.0f;
        public static float ViewportVerticalRatio = 1.0f;

        public static bool VSync = true;

        public static string FontsFolder = "fonts/";

        public static bool SetFullScreenAtStartup = true;
        public static bool FullScreenSet = false;

        // Used in json outputs
        public static JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        };

        public static readonly string TilesPortraits = "Test_Characters_{0}_Portraits";
        public static readonly string TilesPlayerCharacters = "Test_Characters_";

        public static void RegisterViewportBounds(int width, int height)
        {
            //Debug.WriteLine("width: " + width + ", Renderer.CanvasWidth: " + Renderer.CanvasWidth);
            ViewportWidth = width;
            ViewportHeight = height;
            // Register horizontal and vertical ratios between the canvas width and the viewport width
            // so it's used to scale the result when rendering camera film onto the resizing window
            // Also used for the mouse coordinates
            ViewportHorizontalRatio = width / Renderer.CanvasWidth;
            ViewportVerticalRatio = height / Renderer.CanvasHeight;
        }

        public static void Initialize()
        {
            APP_PATH = AppContext.BaseDirectory;

            // Get current platform
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Platform = OSPlatform.Windows;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Platform = OSPlatform.OSX;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Platform = OSPlatform.Linux;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
            {
                Platform = OSPlatform.FreeBSD;
            }
            else
            {
                Platform = Unknown;
            }

        }
    }
}
