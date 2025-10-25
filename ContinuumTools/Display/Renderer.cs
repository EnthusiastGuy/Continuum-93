using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

namespace Last_Known_Reality.Reality
{
    public static class Renderer
    {
        // These should be dynamic, updated on fullscreen or ratio change
        public static float CanvasWidth = 1280;
        public static float CanvasHeight = 540;
        public static float UIWidth;
        public static float UIHeight;
        private static GraphicsDeviceManager graphicsDeviceManager;
        private static SpriteBatchPlus spriteBatch;

        public static void Draw()
        {
            RenderToGameWindow();   // Draw the back-buffer to screen
        }

        // Takes a given resolution and sets the correct canvas width and height to be scaled at it.
        public static void RefreshCanvas()
        {
            CanvasWidth = WindowManager.GetClientWidth();
            CanvasHeight = WindowManager.GetClientHeight();
        }

        public static void RegisterGraphicsDeviceManager(GraphicsDeviceManager gdm)
        {
            graphicsDeviceManager = gdm;
        }

        public static void InitializeSpriteBatch()
        {
            spriteBatch = new SpriteBatchPlus();
        }

        public static GraphicsDeviceManager GetGraphics()
        {
            return graphicsDeviceManager;
        }

        public static GraphicsDevice GetGraphicsDevice()
        {
            return graphicsDeviceManager.GraphicsDevice;
        }
        public static void DrawMonospaced(PngFont font, string text, float x, float y, Color color, int offset)
        {
            spriteBatch.DrawMonospaced(font, text, x, y, color, offset);
        }

        public static void DrawStringPlusHiRes(PngFont font, string text, float x, float y, Color color)
        {
            spriteBatch.DrawStringPlus(font, text, x, y, color, true);
        }

        public static void SetRenderTarget(RenderTarget2D target)
        {
            graphicsDeviceManager.GraphicsDevice.SetRenderTarget(target);
        }

        public static void Begin(SpriteSortMode sortMode = SpriteSortMode.Deferred, BlendState blendState = null, SamplerState samplerState = null, DepthStencilState depthStencilState = null, RasterizerState rasterizerState = null, Effect effect = null, Matrix? transformMatrix = null)
        {
            spriteBatch.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, transformMatrix);
        }

        public static void End()
        {
            spriteBatch.End();
        }

        // The draw methods
        public static void Draw(Texture2D texture, Rectangle destinationRectangle, Color color)
        {
            spriteBatch.Draw(texture, destinationRectangle, color);
        }

        public static void Draw(Texture2D texture, Vector2 position, Color color)
        {
            spriteBatch.Draw(texture, position, color);
        }

        public static void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color)
        {
            spriteBatch.Draw(texture, position, sourceRectangle, color);
        }

        public static void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth)
        {
            spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, color, rotation, origin, effects, layerDepth);
        }

        public static void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
        {
            spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, color);
        }

        public static void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
            spriteBatch.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
        }

        public static void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
        {
            spriteBatch.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
        }

        // END of the draw methods

        public static void DrawLine(Vector2 point1, Vector2 point2, Color color, float thickness = 1f, float depth = 1f)
        {
            var distance = Vector2.Distance(point1, point2);
            var angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            DrawLine(point1, distance, angle, color, thickness, depth);
        }

        public static void DrawLine(Vector2 point, float length, float angle, Color color, float thickness = 1f, float depth = 1f)
        {
            Vector2 yCorrector = point;
            //yCorrector.Y = 0;
            var origin = new Vector2(0f, 0.5f);
            var scale = new Vector2(length, thickness);
            spriteBatch.Draw(TextureColors.GetTextureColor(color), yCorrector, null, color, angle, origin, scale, SpriteEffects.None, depth);
        }

        public static void DrawRectangle(int x, int y, int width, int height, Color color)
        {
            Rectangle rectangle = new(x, y, width, height);
            spriteBatch.Draw(TextureColors.GetTextureColor(color), rectangle, color);
        }

        public static void RegisterGraphics()
        {
            // TODO check these
            graphicsDeviceManager.PreferredBackBufferWidth = (int)CanvasWidth;
            graphicsDeviceManager.PreferredBackBufferHeight = (int)CanvasHeight;

            graphicsDeviceManager.HardwareModeSwitch = false;   // correct display of fullscreen

            graphicsDeviceManager.SynchronizeWithVerticalRetrace = true;     // VSync
            graphicsDeviceManager.PreferMultiSampling = false;

            graphicsDeviceManager.IsFullScreen = false;
            graphicsDeviceManager.ApplyChanges();
        }


        public static void ShowClientBounds()
        {
            Debug.WriteLine(
                "GetClientBounds().Width: " + WindowManager.GetClientWidth() +
                ", GetClientBounds().Height: " + WindowManager.GetClientHeight()
                );
        }

        // Exposes a method that allows fullscreen toggling
        public static void ToggleFullscreen()
        {
            graphicsDeviceManager.ToggleFullScreen();
        }

        public static void SetWindowMode()
        {
            graphicsDeviceManager.IsFullScreen = false;
            graphicsDeviceManager.ApplyChanges();
        }

        public static void SetFullScreen()
        {
            graphicsDeviceManager.IsFullScreen = true;
            graphicsDeviceManager.ApplyChanges();
            Config.FullScreenSet = true;
        }

        public static void SetWindowSizeRatio(int ratio)
        {
            int nWidth = Config.BaseWidth * ratio;
            int nHeight = Config.BaseHeight * ratio;
            Config.RegisterViewportBounds(nWidth, nHeight);
            SetPreferredBackBufferSize(nWidth, nHeight);
        }

        public static void SetPreferredBackBufferSize(int width, int height)
        {
            graphicsDeviceManager.PreferredBackBufferWidth = width;
            graphicsDeviceManager.PreferredBackBufferHeight = height;
            graphicsDeviceManager.ApplyChanges();
        }

        // Some TV's don't support this. In such event the resolution gets skewed due to:
        // "The specified device interface or feature level is not supported on this system."
        private static void RenderToGameWindow()
        {
            Begin(
                SpriteSortMode.Deferred,
                BlendState.NonPremultiplied,
                SamplerState.PointClamp
            );

            // Render the actual game. For each active camera
            /*
            for (int i = 0; i < CameraManager.GetCamerasCount(); i++)
            {
                WorldCamera camera = CameraManager.GetCamera(i);
                if (!(camera is null) && camera.Active)
                {
                    // Draw the camera
                    spriteBatch.Draw(
                        camera.Film,
                        camera.GetTargetRectangle(),    // DESTINATION: Scales on the target viewport (the final window size)
                        new Rectangle(0, 0, camera.GetViewWidth(), camera.GetViewHeight()), // SOURCE
                        Color.White

                    );

                    //Debug.WriteLine("camera.GetTargetRectangle(): " + camera.GetTargetRectangle().ToString());
                }
            }

            // Draw the hi-res UI here
            spriteBatch.Draw(
                UIBuffer.TheUIBuffer,
                new Rectangle(0, 0, Config.ViewportWidth, Config.ViewportHeight),   // Destination
                new Rectangle(0, 0, (int)UIWidth, (int)UIHeight),  // Source
                Color.White

            );
            */

            End();
        }
    }
}
