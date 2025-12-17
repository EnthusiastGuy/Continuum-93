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
            const int charWidth = 13;

            // Title
            ServiceGraphics.DrawText(
                Fonts.ModernDOS_12x18,
                "Memory view",
                contentRect.X + Padding,
                contentRect.Y + Padding,
                contentRect.Width - Padding * 2,
                Color.Yellow,
                Color.Black,
                (byte)ServiceFontFlags.DrawOutline,
                0xFF
            );

            int startY = contentRect.Y + Padding + 24;

            // Draw memory lines
            var lines = Memory.Lines;
            int visibleLines = Math.Min(lines.Count, (contentRect.Height - (startY - contentRect.Y)) / lineHeight);

            for (int i = 0; i < visibleLines; i++)
            {
                int y = startY + i * lineHeight;
                if (y + lineHeight > contentRect.Bottom)
                    break;

                var line = lines[i];

                // Address
                ServiceGraphics.DrawText(
                    Fonts.ModernDOS_12x18_thin,
                    line.TextAddress,
                    contentRect.X + Padding,
                    y,
                    contentRect.Width - Padding * 2,
                    Color.White,
                    Color.Black,
                    (byte)(ServiceFontFlags.Monospace | ServiceFontFlags.DrawOutline),
                    0xFF
                );

                // Hex bytes
                ServiceGraphics.DrawText(
                    Fonts.ModernDOS_12x18_thin,
                    line.HexBytes,
                    contentRect.X + Padding + 60,
                    y,
                    contentRect.Width - Padding * 2,
                    new Color(40, 80, 160), // Memory byte color
                    Color.Black,
                    (byte)(ServiceFontFlags.Monospace | ServiceFontFlags.DrawOutline),
                    0xFF
                );

                // ASCII bytes
                ServiceGraphics.DrawText(
                    Fonts.ModernDOS_12x18_thin,
                    line.ASCIIBytes,
                    contentRect.X + Padding + 406,
                    y,
                    contentRect.Width - Padding * 2,
                    new Color(200, 200, 200), // ASCII text color
                    Color.Black,
                    (byte)(ServiceFontFlags.Monospace | ServiceFontFlags.DrawOutline),
                    0xFF
                );
            }
        }
    }
}

