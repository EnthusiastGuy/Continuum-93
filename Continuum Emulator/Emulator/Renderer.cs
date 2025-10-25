using Continuum93.Emulator.Window;
using Continuum93.Emulator;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Continuum93.Emulator
{
    public static class Renderer
    {
        private static GraphicsDeviceManager _graphicsDeviceManager;
        private static SpriteBatch _spriteBatch;
        public static Effect InterlaceEffect;

        public static void RegisterGraphicsDeviceManager(GraphicsDeviceManager gdm)
        {
            _graphicsDeviceManager = gdm;
            _graphicsDeviceManager.HardwareModeSwitch = false;   // correct display of fullscreen
        }

        public static void InitializeSpriteBatch()
        {
            _spriteBatch = new SpriteBatch(_graphicsDeviceManager.GraphicsDevice);
        }

        public static void SetPreferredBackBufferSize(int width, int height)
        {
            _graphicsDeviceManager.PreferredBackBufferWidth = width;
            _graphicsDeviceManager.PreferredBackBufferHeight = height;
            _graphicsDeviceManager.ApplyChanges();
        }

        public static void Clear(Color color)
        {
            _graphicsDeviceManager.GraphicsDevice.Clear(color);
        }

        public static GraphicsDeviceManager GetGraphicsDeviceManager()
        {
            return _graphicsDeviceManager;
        }

        public static GraphicsDevice GetGraphicsDevice()
        {
            return _graphicsDeviceManager.GraphicsDevice;
        }

        public static void SetRenderTarget(RenderTarget2D target)
        {
            _graphicsDeviceManager.GraphicsDevice.SetRenderTarget(target);
        }

        public static void Begin(SpriteSortMode sortMode = SpriteSortMode.Deferred, BlendState blendState = null, SamplerState samplerState = null, DepthStencilState depthStencilState = null, RasterizerState rasterizerState = null, Effect effect = null, Matrix? transformMatrix = null)
        {
            _spriteBatch.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, transformMatrix);
        }

        public static void End()
        {
            _spriteBatch.End();
        }

        public static void ToggleFullscreen()
        {
            _graphicsDeviceManager.ToggleFullScreen();
        }

        public static bool IsFullScreen()
        {
            return _graphicsDeviceManager.IsFullScreen;
        }

        public static void SetWindowMode()
        {
            _graphicsDeviceManager.IsFullScreen = false;
            _graphicsDeviceManager.ApplyChanges();
        }

        public static void SetFullScreen(bool value = true)
        {
            _graphicsDeviceManager.IsFullScreen = value;
            _graphicsDeviceManager.ApplyChanges();
        }

        // Render the target projection to the actual window
        public static void RenderToWindow(Texture2D projection, int width, int height)
        {
            Begin(
                SpriteSortMode.Deferred,
                BlendState.Opaque,
                SamplerState.PointClamp,
                DepthStencilState.None,
                RasterizerState.CullNone
                //, effect: InterlaceEffect
            );

            _spriteBatch.Draw(
                projection,
                new Rectangle(
                    0, 0,
                    WindowManager.GetClientWidth(), WindowManager.GetClientHeight()
                ),     // Destination
                new Rectangle(0, 0, width, height),     // Source
                Color.White
            );

            End();
        }

        public static void DrawBlank()
        {
            _graphicsDeviceManager.GraphicsDevice.Clear(Color.DarkGray);
        }
    }
}
