using Continuum93.Emulator;
using Continuum93.ServiceModule.Parsers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Continuum93.ServiceModule.UI
{
    public class MemoryWindow : Window
    {
        private int _previousScrollValue = 0;

        public MemoryWindow(
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
            Memory.Update();

            // Handle mouse wheel scrolling
            var mouse = Mouse.GetState();
            if (ContentRect.Contains(new Point(mouse.X, mouse.Y)))
            {
                int scrollDelta = mouse.ScrollWheelValue - _previousScrollValue;
                if (scrollDelta != 0)
                {
                    int multiplier = Keyboard.GetState().IsKeyDown(Keys.LeftShift) ? 10 : 1;
                    int jumpDistance = multiplier * 16;

                    if (scrollDelta > 0)
                    {
                        // Scroll up
                        if (Memory.Address >= jumpDistance)
                        {
                            Memory.Address -= jumpDistance;
                        }
                        else
                        {
                            Memory.Address = 0;
                        }
                    }
                    else
                    {
                        // Scroll down
                        if (Memory.Address <= (0xFFFFFF - (jumpDistance + Memory.FetchLines * 16)))
                        {
                            Memory.Address += jumpDistance;
                        }
                        else
                        {
                            Memory.Address = 0xFFFFFF - (Memory.FetchLines * 16);
                        }
                    }
                    Memory.Update();
                }
                _previousScrollValue = mouse.ScrollWheelValue;
            }
        }

        protected override void DrawContent(SpriteBatch spriteBatch, Rectangle contentRect)
        {
            const int lineHeight = 18;
            //const int charWidth = 13;
            var theme = ServiceGraphics.Theme;

            // Title
            ServiceGraphics.DrawText(
                Fonts.ModernDOS_12x18,
                "Memory view",
                contentRect.X + Padding,
                contentRect.Y + Padding,
                contentRect.Width - Padding * 2,
                theme.TextTitle,
                theme.TextOutline,
                (byte)ServiceFontFlags.DrawOutline,
                0xFF
            );

            int startY = contentRect.Y + Padding + 24;

            // Draw memory lines
            var lines = Memory.Lines;
            int visibleLines = Math.Min(lines.Count, (contentRect.Height - (startY - contentRect.Y)) / lineHeight);

            byte fontFlags = (byte)(ServiceFontFlags.Monospace | ServiceFontFlags.DrawOutline);
            const int ColumnSpacing = 20; // Spacing between columns

            for (int i = 0; i < visibleLines; i++)
            {
                int y = startY + i * lineHeight;
                if (y + lineHeight > contentRect.Bottom)
                    break;

                var line = lines[i];

                // Measure address text width
                int addressWidth = theme.PrimaryFont.MeasureText(
                    line.TextAddress,
                    contentRect.Width - Padding * 2,
                    fontFlags
                ).width;

                // Address
                ServiceGraphics.DrawText(
                    theme.PrimaryFont,
                    line.TextAddress,
                    contentRect.X + Padding,
                    y,
                    contentRect.Width - Padding * 2,
                    theme.TextPrimary,
                    theme.TextOutline,
                    fontFlags,
                    0xFF
                );

                // Measure hex bytes text width
                int hexBytesWidth = theme.PrimaryFont.MeasureText(
                    line.HexBytes,
                    contentRect.Width - Padding * 2,
                    fontFlags
                ).width;

                // Hex bytes
                ServiceGraphics.DrawText(
                    theme.PrimaryFont,
                    line.HexBytes,
                    contentRect.X + Padding + addressWidth + ColumnSpacing,
                    y,
                    contentRect.Width - Padding * 2,
                    theme.MemoryByteColor,
                    theme.TextOutline,
                    fontFlags,
                    0xFF
                );

                // ASCII bytes
                ServiceGraphics.DrawText(
                    theme.PrimaryFont,
                    line.ASCIIBytes,
                    contentRect.X + Padding + addressWidth + ColumnSpacing + hexBytesWidth + ColumnSpacing,
                    y,
                    contentRect.Width - Padding * 2,
                    theme.MemoryAsciiColor,
                    theme.TextOutline,
                    fontFlags,
                    0xFF
                );
            }
        }
    }
}

