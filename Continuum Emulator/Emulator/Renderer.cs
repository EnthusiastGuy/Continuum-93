using Continuum93.Emulator;
using Continuum93.Emulator.Window;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Continuum93.Emulator
{
    public static class Renderer
    {
        private static GraphicsDeviceManager _graphicsDeviceManager;
        private static SpriteBatch _spriteBatch;
        
        public static Effect InterlaceEffect;
        public static Effect CrtEffect;

        // Phosphor effect
        static RenderTarget2D _phosphorA;
        static RenderTarget2D _phosphorB;
        static bool _phosphorInit;
        public static Effect PhosphorEffect;
        public static bool UsePhosphor = true;

        public static float PhosphorDecay = 0.75f;  // Tune: 0.85..0.97

        private static Texture2D _pixelTexture;
        private static Game _game;

        public static void EnsurePhosphorTargets(GraphicsDevice device, int w, int h)
        {
            if (_phosphorA != null && _phosphorA.Width == w && _phosphorA.Height == h)
                return;

            _phosphorA?.Dispose();
            _phosphorB?.Dispose();

            _phosphorA = new RenderTarget2D(device, w, h, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
            _phosphorB = new RenderTarget2D(device, w, h, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);

            _phosphorInit = false;
        }

        public static void RegisterGraphicsDeviceManager(GraphicsDeviceManager gdm, Game game)
        {
            _graphicsDeviceManager = gdm;
            _graphicsDeviceManager.HardwareModeSwitch = false;   // correct display of fullscreen
            _game = game;
        }

        public static void InitializeSpriteBatch()
        {
            _spriteBatch = new SpriteBatch(_graphicsDeviceManager.GraphicsDevice);
        }

        public static SpriteBatch GetSpriteBatch()
        {
            return _spriteBatch;
        }

        public static Rectangle GetScreenBounds()
        {
            // However you access the Game instance:
            // e.g. Service.Game, MainGame.Instance, etc.
            var windowBounds = _game.Window.ClientBounds;
            return windowBounds;
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

        public static Texture2D GetPixelTexture()
        {
            if (_pixelTexture == null)
            {
                var device = _graphicsDeviceManager.GraphicsDevice;
                _pixelTexture = new Texture2D(device, 1, 1, false, SurfaceFormat.Color);
                _pixelTexture.SetData(new[] { Color.White });
            }

            return _pixelTexture;
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
            var device = _graphicsDeviceManager.GraphicsDevice;

            // Clear everything first so we get the black bars.
            device.Clear(Color.Black);

            // Use the shared helper so everyone agrees on this rect
            Rectangle destRect = GetDestinationRectangle(width, height);
            Rectangle srcRect = new(0, 0, width, height);

            // Actual scale being used horizontally (same vertically because of aspect)
            float scale = (float)destRect.Width / width;

            float roundedScale = (float)Math.Round(scale);
            bool isIntegerScale =
                roundedScale >= 1f &&
                Math.Abs(scale - roundedScale) < 0.01f;

            SamplerState sampler = isIntegerScale
                ? SamplerState.PointClamp
                : SamplerState.LinearClamp;


            // Effect testing

            if (UsePhosphor)
                projection = ApplyPhosphor(projection, width, height);

            // Set params:
            int vpW = device.Viewport.Width;
            int vpH = device.Viewport.Height;

            var fx = CrtEffect;
            fx.Parameters["SourceSize"]?.SetValue(new Vector2(width, height));
            fx.Parameters["OutputSize"]?.SetValue(new Vector2(vpW, vpH));

            float edgePx = 2.0f; // to test 1.5..3.5
            float edgeFeather = edgePx / MathF.Min(vpW, vpH);

            // Tuning
            fx.Parameters["EdgeFeather"]?.SetValue(edgeFeather);

            fx.Parameters["Curvature"]?.SetValue(0.005f);
            fx.Parameters["Bleed"]?.SetValue(0.3f);                 // pixels
            fx.Parameters["ScanlineIntensity"]?.SetValue(0.25f);
            fx.Parameters["Vignette"]?.SetValue(0.20f);
            fx.Parameters["CornerRadius"]?.SetValue(0.06f);
            fx.Parameters["CornerFeather"]?.SetValue(0.022f);

            fx.Parameters["BorderGlow"]?.SetValue(1.0f);
            fx.Parameters["BorderColor"]?.SetValue(new Vector3(0.02f, 0.02f, 0.025f));

            // Optional
            fx.Parameters["NoiseIntensity"]?.SetValue(0.01f);        // try 0.03f if you want a tiny bit



            Begin(
                SpriteSortMode.Deferred,
                BlendState.Opaque,
                sampler,
                DepthStencilState.None,
                RasterizerState.CullNone,
                fx
            );

            _spriteBatch.Draw(
                projection,
                destRect,
                srcRect,
                Color.White
            );

            End();
        }

        static Texture2D ApplyPhosphor(Texture2D currentFrame, int w, int h)
        {
            var device = _graphicsDeviceManager.GraphicsDevice;

            EnsurePhosphorTargets(device, w, h);

            // First frame: start history as the current frame (avoids a ramp-up)
            if (!_phosphorInit)
            {
                device.SetRenderTarget(_phosphorA);
                device.Clear(Color.Black);

                _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
                _spriteBatch.Draw(currentFrame, new Rectangle(0, 0, w, h), Color.White);
                _spriteBatch.End();

                device.SetRenderTarget(null);
                _phosphorInit = true;
                return _phosphorA;
            }

            // Ping-pong: write into B using A as history
            device.SetRenderTarget(_phosphorB);
            device.Clear(Color.Black);

            var fx = PhosphorEffect;
            fx.Parameters["HistoryTexture"]?.SetValue(_phosphorA);
            fx.Parameters["Decay"]?.SetValue(PhosphorDecay);
            fx.Parameters["CurrentGain"]?.SetValue(1.0f);

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, fx);
            _spriteBatch.Draw(currentFrame, new Rectangle(0, 0, w, h), Color.White);
            _spriteBatch.End();

            device.SetRenderTarget(null);

            // Swap
            (_phosphorA, _phosphorB) = (_phosphorB, _phosphorA);

            return _phosphorA;
        }

        public static Rectangle GetDestinationRectangle(int sourceWidth, int sourceHeight)
        {
            int backWidth = WindowManager.GetClientWidth();
            int backHeight = WindowManager.GetClientHeight();

            // Scale needed in each direction to fit inside the backbuffer
            float scaleX = (float)backWidth / sourceWidth;
            float scaleY = (float)backHeight / sourceHeight;
            float scale = Math.Min(scaleX, scaleY);

            int destWidth = (int)(sourceWidth * scale);
            int destHeight = (int)(sourceHeight * scale);

            int destX = (backWidth - destWidth) / 2;
            int destY = (backHeight - destHeight) / 2;

            return new Rectangle(destX, destY, destWidth, destHeight);
        }

        public static void DrawBlank()
        {
            _graphicsDeviceManager.GraphicsDevice.Clear(Color.DarkGray);
        }
    }
}
