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

            // Title
            ServiceGraphics.DrawText(
                Fonts.ModernDOS_12x18,
                "Flags",
                contentRect.X + Padding,
                contentRect.Y + Padding,
                contentRect.Width - Padding * 2,
                theme.TextTitle,
                theme.TextOutline,
                (byte)ServiceFontFlags.DrawOutline,
                0xFF
            );

            int originX = contentRect.X + Padding;
            int originY = contentRect.Y + Padding + 24;

            // Draw flags
            for (byte i = 0; i < 8; i++)
            {
                int x = originX + i * (int)(charWidth * 4.5f);
                
                // Calculate Y positions based on actual font height
                int y1 = originY + lineHeight;           // First row of flag names (positive flags)
                int y1Value = originY + lineHeight * 2;  // Flag value below first row
                int y2 = originY + lineHeight * 3;       // Second row of flag names (negative flags)
                int y2Value = originY + lineHeight * 4;  // Flag value below second row

                bool flagValue = CPUState.GetBitValue(flags, i);
                bool oldFlagValue = CPUState.GetBitValue(oldFlags, i);
                bool valueChanged = flagValue != oldFlagValue;

                Color regColor = theme.FlagNameColor;
                Color markColor = valueChanged ? theme.FlagValueChangedColor : theme.FlagValueUnchangedColor;

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

                // Flag values
                ServiceGraphics.DrawText(
                    theme.PrimaryFont,
                    (flagValue ? 1 : 0).ToString(),
                    x,
                    y1Value,
                    contentRect.Width - Padding * 2,
                    markColor,
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
                    markColor,
                    theme.TextOutline,
                    (byte)(ServiceFontFlags.Monospace | ServiceFontFlags.DrawOutline),
                    0xFF
                );
            }
        }
    }
}

