using Continuum93.Emulator;
using Continuum93.Emulator.States;
using Continuum93.ServiceModule.Themes;
using Continuum93.ServiceModule.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Continuum93.ServiceModule
{
    public class ServiceGraphics
    {
        private float _serviceAnim;          // 0 = normal, 1 = debug
        private const float ServiceAnimSpeed = 4.0f; // how fast it moves between states
        private static ColorTheme Theme;

        private static WindowManager _windowManager;
        private static TextWindow _testWindow;

        // Expose to ServiceInput so it can send mouse events
        public static WindowManager WindowManager => _windowManager;

        public static void Initialize()
        {
            Fonts.LoadFonts();
            Theme = ThemeLoader.Load("Data/themes/default.json");

            _windowManager = new WindowManager();

            _testWindow = new TextWindow(
                "DEBUGGER",
                () => Operations.DisassembledCode,
                x: 988,
                y: 16,
                width: 400,
                height: 600,
                1.1f,
                true,
                true
            );

            _windowManager.Add(_testWindow);

            _testWindow = new TextWindow(
                "REGISTERS",
                () => "This is a draggable, resizable test window.\n" +
                "Mouse input is routed via ServiceInput.",
                x: 160,
                y: 540,
                width: 600,
                height: 200,
                1.1f,
                true,
                true
            );

            _windowManager.Add(_testWindow);
        }

        public void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float target = Service.STATE.ServiceMode ? 1f : 0f;

            if (_serviceAnim < target)
            {
                _serviceAnim = Math.Min(target, _serviceAnim + ServiceAnimSpeed * dt);
            }
            else if (_serviceAnim > target)
            {
                _serviceAnim = Math.Max(target, _serviceAnim - ServiceAnimSpeed * dt);
            }

            // We want to use the service-layout renderer both:
            // - when service mode is ON
            // - while we are animating back (_serviceAnim > 0)
            Service.STATE.UseServiceView =
                (Service.STATE.ServiceMode || _serviceAnim > 0.001f) && Machine.COMPUTER?.GRAPHICS != null;

            _windowManager?.Update(gameTime);
        }
        public void Draw()
        {
            // Easing (feels nicer than linear)
            float t = _serviceAnim;
            float eased = t * t * (3f - 2f * t); // smoothstep 0..1

            DrawThumbnail(eased);
            DrawViews(eased);

        }

        private void DrawThumbnail(float eased)
        {
            // Making sure the 480x270 texture is up-to-date
            Machine.COMPUTER?.GRAPHICS.UpdateProjectionOnly();

            var projection = Machine.COMPUTER?.GRAPHICS.VideoProjection;

            if (projection == null)
            {
                // Nothing to show yet
                Renderer.DrawBlank();
                return;
            }

            // --- FULLSCREEN RECT (normal mode with pillars/bars) ---
            var rectFull = Renderer.GetDestinationRectangle(projection.Width, projection.Height);

            // --- SERVICE RECT (final position in service mode) ---
            const int padding = 16;
            var serviceScreenThumbnail = new Rectangle(
                padding,
                padding,
                projection.Width * 2,   // 480
                projection.Height * 2   // 270
            );

            // Clear the fullscreen backbuffer for the service layout,
            // LERPing from black (normal mode) to dark gray (service mode)
            // TODO, move the colors to some form of theme/settings
            Color bgColor = Color.Lerp(Color.Black, Theme.Background, eased);
            Renderer.Clear(bgColor);

            // Interpolate between fullscreen and service rect
            Rectangle destRect = LerpRect(rectFull, serviceScreenThumbnail, eased);

            Renderer.GetSpriteBatch().Begin(
                SpriteSortMode.Deferred,
                BlendState.Opaque,
                SamplerState.PointClamp,
                DepthStencilState.None,
                RasterizerState.CullNone
            );

            // Live view of the emulator with animated position/size
            Renderer.GetSpriteBatch().Draw(
                projection,
                destRect,
                Color.White
            );

            Renderer.GetSpriteBatch().End();
        }

        private static void DrawViews(float eased)
        {
            Renderer.GetSpriteBatch().Begin(
                samplerState: SamplerState.PointClamp,
                blendState: BlendState.AlphaBlend);

            Color titleColor = Color.Lerp(Color.Transparent, Color.YellowGreen, eased);
            Color color = Color.Lerp(Color.Transparent, Color.White, eased);
            Color textInfocolor = Color.Lerp(Color.Transparent, Color.Khaki, eased);
            Color outlineColor = Color.Lerp(Color.Transparent, Color.Black, eased);

            //DrawText(
            //    Fonts.ModernDOS_12x18,
            //    "DISASSEMBLER",
            //    976 + 8, 16, 600,
            //    titleColor,
            //    outlineColor,
            //    (byte)(ServiceFontFlags.Wrap | ServiceFontFlags.DrawOutline | ServiceFontFlags.Monospace | ServiceFontFlags.MonospaceCentering),
            //    0b1111_1111
            //);

            //DrawText(
            //    Fonts.ModernDOS_12x18_thin,
            //    Operations.DisassembledCode,
            //    976 + 8, 16 + 20, 600,
            //    color,
            //    outlineColor,
            //    (byte)(ServiceFontFlags.Wrap | ServiceFontFlags.DrawOutline | ServiceFontFlags.Monospace | ServiceFontFlags.MonospaceCentering),
            //    0b1111_1111
            //);

            //DrawText(
            //    Fonts.ModernDOS_12x18_thin,
            //    "Sample text to show non-monospaced text drawing for textual information.\nAnd also to test font characters...\nthe quick brown fox jumps over the lazy dog\nTHE QUICK BROWN FOX JUMPS OVER THE LAZY DOG",
            //    16, 540 + 24, 600,
            //    textInfocolor,
            //    outlineColor,
            //    (byte)(ServiceFontFlags.Wrap | ServiceFontFlags.DrawOutline | ServiceFontFlags.MonospaceCentering),
            //    0b1111_1111
            //);

            _windowManager?.Draw();

            Renderer.GetSpriteBatch().End();
        }

        public static void DrawText(
            ServiceFont font,
            string text,
            int x,
            int y,
            int maxWidthPixels,
            Color? color,
            Color? outlineColor,
            byte flags,
            byte outlinePattern // full 3x3 outline
        )
        {
            // Build flags based on parameters

            // Because Color is a struct, allow null to mean "use default"
            var finalColor = color ?? Color.White;
            var finalOutlineColor = outlineColor ?? Color.Black;

            font.DrawString(
                text,
                x: x,
                y: y,
                color: finalColor,
                maxWidthPixels: maxWidthPixels,
                flagsByte: flags,
                outlineColor: finalOutlineColor,
                outlinePattern: outlinePattern
            );
        }

        private static Rectangle LerpRect(Rectangle from, Rectangle to, float t)
        {
            t = MathHelper.Clamp(t, 0f, 1f);
            int x = (int)MathHelper.Lerp(from.X, to.X, t);
            int y = (int)MathHelper.Lerp(from.Y, to.Y, t);
            int w = (int)MathHelper.Lerp(from.Width, to.Width, t);
            int h = (int)MathHelper.Lerp(from.Height, to.Height, t);
            return new Rectangle(x, y, w, h);
        }
    }
}
