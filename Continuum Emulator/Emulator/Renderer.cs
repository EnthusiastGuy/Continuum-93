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
        static RenderTarget2D _scale2xRT;
        static RenderTarget2D _vhsRT;

        static uint _vhsRng = 0x12345678u;
        static uint _vhsFrame = 0;

        static bool _phosphorInit;
        public static Effect PhosphorEffect;
        public static Effect Scale2xEffect;
        public static Effect VhsEffect;

        public static bool UsePhosphor = true;
        public static bool UseGreenMonitor = false;
        public static bool UseScale2x = false;
        public static bool UseVhs = false;

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

        static void EnsureVhsTarget(GraphicsDevice device, int w, int h)
        {
            if (_vhsRT != null && _vhsRT.Width == w && _vhsRT.Height == h)
                return;

            _vhsRT?.Dispose();
            _vhsRT = new RenderTarget2D(
                device, w, h, false,
                SurfaceFormat.Color, DepthFormat.None,
                0, RenderTargetUsage.DiscardContents);
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

            


            // Effect testing

            if (UsePhosphor)
                projection = ApplyPhosphor(projection, width, height);

            int srcW = width;
            int srcH = height;

            if (UseVhs)
                projection = ApplyVhs(projection, width, height, (float)GameTimePlus.GetTotalSeconds());

            if (UseScale2x)
            {
                projection = ApplyScale2x(projection, width, height);
                srcW = width * 2;
                srcH = height * 2;
            }

            // Use the shared helper so everyone agrees on this rect
            Rectangle destRect = GetDestinationRectangle(width, height);
            Rectangle srcRect = new(0, 0, srcW, srcH);

            // Actual scale being used horizontally (same vertically because of aspect)
            float scale = (float)destRect.Width / width;

            float roundedScale = (float)Math.Round(scale);
            bool isIntegerScale =
                roundedScale >= 1f &&
                Math.Abs(scale - roundedScale) < 0.01f;

            SamplerState sampler = isIntegerScale
                ? SamplerState.PointClamp
                : SamplerState.LinearClamp;

            // Set params:
            int vpW = device.Viewport.Width;
            int vpH = device.Viewport.Height;

            var fx = CrtEffect;
            fx.Parameters["SourceSize"]?.SetValue(new Vector2(srcW, srcH));
            fx.Parameters["OutputSize"]?.SetValue(new Vector2(vpW, vpH));

            float edgePx = 2.0f; // to test 1.5..3.5
            float edgeFeather = edgePx / MathF.Min(vpW, vpH);

            // Tuning
            fx.Parameters["EdgeFeather"]?.SetValue(edgeFeather);

            fx.Parameters["ScanlineScale"]?.SetValue(UseScale2x ? 0.5f : 1.0f);

            fx.Parameters["Curvature"]?.SetValue(0.005f);
            fx.Parameters["Bleed"]?.SetValue(0.3f);                 // pixels
            fx.Parameters["ScanlineIntensity"]?.SetValue(0.25f);
            fx.Parameters["Vignette"]?.SetValue(0.20f);
            fx.Parameters["CornerRadius"]?.SetValue(0.06f);
            fx.Parameters["CornerFeather"]?.SetValue(0.022f);

            fx.Parameters["BorderGlow"]?.SetValue(1.0f);
            fx.Parameters["BorderColor"]?.SetValue(
                UseGreenMonitor
                    ? new Vector3(0.005f, 0.02f, 0.006f)   // dim green tube edge
                    : new Vector3(0.02f, 0.02f, 0.025f)
            );

            fx.Parameters["Monochrome"]?.SetValue(UseGreenMonitor ? 1.0f : 0.0f);
            fx.Parameters["MonoGamma"]?.SetValue(0.95f);
            fx.Parameters["MonoGain"]?.SetValue(1.45f);

            // A classic “green phosphor” vibe; tweak to taste
            fx.Parameters["MonoTint"]?.SetValue(new Vector3(0.25f, 1.05f, 0.35f));
            fx.Parameters["MonoGamma"]?.SetValue(1.65f);

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

        static void EnsureHq2xTarget(GraphicsDevice device, int w, int h)
        {
            int tw = w * 2;
            int th = h * 2;

            if (_scale2xRT != null && _scale2xRT.Width == tw && _scale2xRT.Height == th)
                return;

            _scale2xRT?.Dispose();
            _scale2xRT = new RenderTarget2D(
                device, tw, th, false,
                SurfaceFormat.Color, DepthFormat.None,
                0, RenderTargetUsage.DiscardContents);
        }

        static Texture2D ApplyScale2x(Texture2D src, int w, int h)
        {
            var device = _graphicsDeviceManager.GraphicsDevice;

            EnsureHq2xTarget(device, w, h);

            device.SetRenderTarget(_scale2xRT);
            device.Clear(Color.Black);

            var fx = Scale2xEffect;
            fx.Parameters["SourceSize"]?.SetValue(new Vector2(w, h));
            fx.Parameters["OutputSize"]?.SetValue(new Vector2(w * 2, h * 2));

            _spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.Opaque,
                SamplerState.PointClamp,        // important
                DepthStencilState.None,
                RasterizerState.CullNone,
                fx);

            _spriteBatch.Draw(src, new Rectangle(0, 0, w * 2, h * 2), Color.White);
            _spriteBatch.End();

            device.SetRenderTarget(null);

            return _scale2xRT;
        }

        static Texture2D ApplyVhs(Texture2D src, int w, int h, float timeSeconds)
        {
            var device = _graphicsDeviceManager.GraphicsDevice;

            EnsureVhsTarget(device, w, h);

            device.SetRenderTarget(_vhsRT);
            device.Clear(Color.Black);

            var fx = VhsEffect;
            fx.Parameters["SourceSize"]?.SetValue(new Vector2(w, h));

            // Good defaults (tweak live)
            fx.Parameters["Strength"]?.SetValue(0.25f);
            fx.Parameters["ChromaShift"]?.SetValue(0.25f);
            fx.Parameters["LumaSmear"]?.SetValue(0.3f);
            fx.Parameters["Jitter"]?.SetValue(0.05f);
            fx.Parameters["Wobble"]?.SetValue(0.12f);
            fx.Parameters["Noise"]?.SetValue(0.22f);
            fx.Parameters["Dropouts"]?.SetValue(0.10f);
            fx.Parameters["HeadSwitch"]?.SetValue(0.35f);

            fx.Parameters["Frame"]?.SetValue(_vhsFrame++);

            var seed = new Vector2(Next01(), Next01());
            fx.Parameters["NoiseSeed"]?.SetValue(seed);

            fx.Parameters["Time"]?.SetValue((float)timeSeconds);

            _spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.Opaque,
                SamplerState.LinearClamp, // VHS is inherently soft
                DepthStencilState.None,
                RasterizerState.CullNone,
                fx);

            _spriteBatch.Draw(src, new Rectangle(0, 0, w, h), Color.White);
            _spriteBatch.End();

            device.SetRenderTarget(null);
            return _vhsRT;
        }

        static float Next01()
        {
            // xorshift32
            _vhsRng ^= _vhsRng << 13;
            _vhsRng ^= _vhsRng >> 17;
            _vhsRng ^= _vhsRng << 5;
            return (_vhsRng & 0x00FFFFFFu) / 16777216f; // [0..1)
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
