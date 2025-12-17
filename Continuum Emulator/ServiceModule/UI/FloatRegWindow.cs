using Continuum93.Emulator;
using Continuum93.ServiceModule.Parsers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Continuum93.ServiceModule.UI
{
    public class FloatRegWindow : Window
    {
        public FloatRegWindow(
            string title,
            int x, int y,
            int width, int height,
            float spawnDelaySeconds = 0,
            bool canResize = true,
            bool canClose = false)
            : base(title, x, y, width, height, spawnDelaySeconds, canResize, canClose)
        {
        }

        protected override void UpdateContent(GameTime gameTime)
        {
            CPUState.Update();
        }

        protected override void DrawContent(SpriteBatch spriteBatch, Rectangle contentRect)
        {
            const int lineHeight = 18;

            float[] fRegs = CPUState.FRegs;
            float[] fRegsOld = CPUState.FRegsOld;

            // Title
            ServiceGraphics.DrawText(
                Fonts.ModernDOS_12x18,
                "Float register view",
                contentRect.X + Padding,
                contentRect.Y + Padding,
                contentRect.Width - Padding * 2,
                Color.Yellow,
                Color.Black,
                (byte)ServiceFontFlags.DrawOutline,
                0xFF
            );

            int startY = contentRect.Y + Padding + 24;

            // Draw float registers
            for (byte i = 0; i < 16; i++)
            {
                int y = startY + i * lineHeight;
                if (y + lineHeight > contentRect.Bottom)
                    break;

                string regName = RegistryUtils.GetFloatRegisterRepresentation(i);
                float value = fRegs[i];
                float oldValue = fRegsOld[i];
                bool valueChanged = !value.Equals(oldValue);

                Color regColor = new Color(0.5f, 0.5f, 1f); // Register color
                Color valueColor = valueChanged ? Color.DarkOrange : Color.White;

                // Register name
                ServiceGraphics.DrawText(
                    Fonts.ModernDOS_12x18_thin,
                    $"{regName}:",
                    contentRect.X + Padding,
                    y,
                    contentRect.Width - Padding * 2,
                    regColor,
                    Color.Black,
                    (byte)(ServiceFontFlags.Monospace | ServiceFontFlags.DrawOutline),
                    0xFF
                );

                // Float value
                ServiceGraphics.DrawText(
                    Fonts.ModernDOS_12x18_thin,
                    value.ToString(),
                    contentRect.X + Padding + 60,
                    y,
                    contentRect.Width - Padding * 2,
                    valueColor,
                    Color.Black,
                    (byte)(ServiceFontFlags.Monospace | ServiceFontFlags.DrawOutline),
                    0xFF
                );
            }
        }
    }
}

