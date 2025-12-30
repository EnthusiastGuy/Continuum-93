using Continuum93.Emulator;
using Continuum93.ServiceModule.Parsers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Continuum93.ServiceModule.UI
{
    public class FlagWindow : Window
    {
        private static readonly string[] FLAGNAMES = new string[] {
            "NZ", "NC", "SP", "NO", "PE", "NE", "LTE", "GTE",
            "Z", "C", "SN", "OV", "PO", "EQ", "GT", "LT" };

        public FlagWindow(
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
            const int charWidth = 13;
            var theme = ServiceGraphics.Theme;

            byte flags = CPUState.Flags;

            // Get font metrics for proper line spacing
            int fontHeight = theme.PrimaryFont.GlyphCellHeight;
            const byte characterSpacing = 1; // Matches ServiceFont's internal characterSpacing
            int lineHeight = fontHeight + characterSpacing;

            int originX = contentRect.X + Padding;
            int originY = contentRect.Y + Padding;

            // Draw flags
            for (byte i = 0; i < 8; i++)
            {
                int x = originX + i * (int)(charWidth * 4.5f);
                
                // Calculate Y positions based on actual font height
                int y1 = originY;                       // First row of flag names (positive flags)
                int y2 = originY + lineHeight;          // Second row of flag names (negative flags)

                bool flagValue = CPUState.GetBitValue(flags, i);

                // Color flag names based on value: 1 = green, 0 = red
                // Positive flags: green if 1, red if 0
                Color positiveFlagColor = flagValue ? theme.FlagValueOneColor : theme.FlagValueZeroColor;
                // Negative flags: red if 1, green if 0 (inverse)
                Color negativeFlagColor = flagValue ? theme.FlagValueZeroColor : theme.FlagValueOneColor;

                // Flag names (positive and negative) - colored by their value
                ServiceGraphics.DrawText(
                    theme.PrimaryFont,
                    FLAGNAMES[8 + i],
                    x,
                    y1,
                    contentRect.Width - Padding * 2,
                    positiveFlagColor,
                    theme.TextOutline,
                    (byte)(ServiceFontFlags.Monospace | ServiceFontFlags.DrawOutline),
                    0xFF
                );

                ServiceGraphics.DrawText(
                    theme.PrimaryFont,
                    FLAGNAMES[i],
                    x,
                    y2,
                    contentRect.Width - Padding * 2,
                    negativeFlagColor,
                    theme.TextOutline,
                    (byte)(ServiceFontFlags.Monospace | ServiceFontFlags.DrawOutline),
                    0xFF
                );
            }
        }
    }
}

