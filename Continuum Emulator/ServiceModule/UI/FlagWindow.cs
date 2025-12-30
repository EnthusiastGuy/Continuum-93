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
            byte oldFlags = CPUState.OldFlags;

            // Get font metrics for proper line spacing
            int fontHeight = theme.PrimaryFont.GlyphCellHeight;
            const byte characterSpacing = 1; // Matches ServiceFont's internal characterSpacing
            int lineHeight = fontHeight + characterSpacing;
            const int spacingBetweenPairs = 1; // Extra line height spacing between the two pairs of rows

            int originX = contentRect.X + Padding;
            int originY = contentRect.Y + Padding;

            // Draw flags
            for (byte i = 0; i < 8; i++)
            {
                int x = originX + i * (int)(charWidth * 4.5f);
                
                // Calculate Y positions based on actual font height
                int y1 = originY + lineHeight;           // First row of flag names (positive flags)
                int y1Value = originY + lineHeight * 2;  // Flag value below first row
                // Add spacing between the two pairs
                int y2 = originY + lineHeight * 3 + lineHeight * spacingBetweenPairs;       // Second row of flag names (negative flags)
                int y2Value = originY + lineHeight * 4 + lineHeight * spacingBetweenPairs;  // Flag value below second row

                bool flagValue = CPUState.GetBitValue(flags, i);
                bool oldFlagValue = CPUState.GetBitValue(oldFlags, i);
                bool valueChanged = flagValue != oldFlagValue;

                Color regColor = theme.FlagNameColor;

                // Flag names (positive and negative)
                ServiceGraphics.DrawText(
                    theme.PrimaryFont,
                    FLAGNAMES[8 + i],
                    x,
                    y1,
                    contentRect.Width - Padding * 2,
                    regColor,
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
                    regColor,
                    theme.TextOutline,
                    (byte)(ServiceFontFlags.Monospace | ServiceFontFlags.DrawOutline),
                    0xFF
                );

                // Flag values - color based on value: 1 = green, 0 = red
                Color value1Color = flagValue ? theme.FlagValueOneColor : theme.FlagValueZeroColor;
                Color value0Color = flagValue ? theme.FlagValueZeroColor : theme.FlagValueOneColor;

                ServiceGraphics.DrawText(
                    theme.PrimaryFont,
                    (flagValue ? 1 : 0).ToString(),
                    x,
                    y1Value,
                    contentRect.Width - Padding * 2,
                    value1Color,
                    theme.TextOutline,
                    (byte)(ServiceFontFlags.Monospace | ServiceFontFlags.DrawOutline),
                    0xFF
                );

                ServiceGraphics.DrawText(
                    theme.PrimaryFont,
                    (flagValue ? 0 : 1).ToString(),
                    x,
                    y2Value,
                    contentRect.Width - Padding * 2,
                    value0Color,
                    theme.TextOutline,
                    (byte)(ServiceFontFlags.Monospace | ServiceFontFlags.DrawOutline),
                    0xFF
                );
            }
        }
    }
}

