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

            // Get mouse position for hover detection
            var mouse = Mouse.GetState();
            Point mousePos = new Point(mouse.X, mouse.Y);

            Color highlightColor = theme.TextHighlight;

            // Measure width of a single character
            int charWidth = theme.PrimaryFont.MeasureText("M", 0, fontFlags).width;

            // Determine which byte (if any) is being hovered
            int hoveredLineIndex = -1;
            int hoveredByteIndex = -1;
            if (contentRect.Contains(mousePos))
            {
                int localY = mousePos.Y - startY;
                if (localY >= 0)
                {
                    hoveredLineIndex = localY / lineHeight;
                    if (hoveredLineIndex >= 0 && hoveredLineIndex < visibleLines)
                    {
                        var hoveredLine = lines[hoveredLineIndex];
                        
                        // Measure address text width
                        int addressWidth = theme.PrimaryFont.MeasureText(
                            hoveredLine.TextAddress,
                            contentRect.Width - Padding * 2,
                            fontFlags
                        ).width;

                        int hexColumnX = contentRect.X + Padding + addressWidth + ColumnSpacing;
                        int localX = mousePos.X - hexColumnX;

                        // Check if mouse is in hex bytes column
                        if (localX >= 0)
                        {
                            // Parse hex bytes to get byte count
                            string[] hexParts = hoveredLine.HexBytes.TrimEnd().Split(' ');
                            
                            // Calculate actual positions of each byte to determine which one is hovered
                            int currentX = 0;
                            for (int j = 0; j < hexParts.Length && j < 16; j++)
                            {
                                int byteWidth = theme.PrimaryFont.MeasureText(hexParts[j], 0, fontFlags).width;
                                
                                // Check if mouse is over this byte
                                if (localX >= currentX && localX < currentX + byteWidth)
                                {
                                    hoveredByteIndex = j;
                                    break;
                                }
                                
                                // Move to next byte position (byte + space)
                                currentX += byteWidth;
                                if (j < hexParts.Length - 1)
                                {
                                    int spaceWidth = theme.PrimaryFont.MeasureText(" ", 0, fontFlags).width;
                                    currentX += spaceWidth;
                                }
                            }
                        }
                    }
                }
            }

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

                // Parse hex bytes to get individual bytes
                string[] hexParts = line.HexBytes.TrimEnd().Split(' ');
                int hexColumnX = contentRect.X + Padding + addressWidth + ColumnSpacing;

                // Draw hex bytes individually for hover detection
                int currentHexX = hexColumnX;
                for (int j = 0; j < hexParts.Length && j < 16; j++)
                {
                    bool isHovered = (i == hoveredLineIndex && j == hoveredByteIndex);
                    Color hexColor = isHovered ? highlightColor : theme.MemoryByteColor;

                    ServiceGraphics.DrawText(
                        theme.PrimaryFont,
                        hexParts[j],
                        currentHexX,
                        y,
                        contentRect.Width - Padding * 2,
                        hexColor,
                        theme.TextOutline,
                        fontFlags,
                        0xFF
                    );

                    // Add space after hex byte (except last)
                    if (j < hexParts.Length - 1)
                    {
                        int hexByteTextWidth = theme.PrimaryFont.MeasureText(hexParts[j], 0, fontFlags).width;
                        currentHexX += hexByteTextWidth;
                        // Measure and add space
                        int spaceWidth = theme.PrimaryFont.MeasureText(" ", 0, fontFlags).width;
                        currentHexX += spaceWidth;
                    }
                }

                // Measure hex bytes text width for ASCII column positioning
                int hexBytesWidth = theme.PrimaryFont.MeasureText(
                    line.HexBytes,
                    contentRect.Width - Padding * 2,
                    fontFlags
                ).width;

                // Parse bytes from hex strings to determine ASCII vs non-ASCII
                byte[] bytes = new byte[hexParts.Length];
                for (int j = 0; j < hexParts.Length && j < 16; j++)
                {
                    if (byte.TryParse(hexParts[j], System.Globalization.NumberStyles.HexNumber, null, out byte b))
                    {
                        bytes[j] = b;
                    }
                }

                // Draw ASCII bytes individually
                int asciiColumnX = contentRect.X + Padding + addressWidth + ColumnSpacing + hexBytesWidth + ColumnSpacing;
                int currentAsciiX = asciiColumnX;
                for (int j = 0; j < bytes.Length && j < 16; j++)
                {
                    bool isHovered = (i == hoveredLineIndex && j == hoveredByteIndex);
                    bool isAscii = bytes[j] >= 32 && bytes[j] <= 127;
                    
                    string asciiChar = isAscii ? ((char)bytes[j]).ToString() : "?";
                    Color asciiColor = isHovered ? highlightColor : (isAscii ? theme.MemoryAsciiColor : theme.MemoryAsciiNonAsciiColor);

                    ServiceGraphics.DrawText(
                        theme.PrimaryFont,
                        asciiChar,
                        currentAsciiX,
                        y,
                        contentRect.Width - Padding * 2,
                        asciiColor,
                        theme.TextOutline,
                        fontFlags,
                        0xFF
                    );

                    currentAsciiX += charWidth;
                }
            }
        }
    }
}

