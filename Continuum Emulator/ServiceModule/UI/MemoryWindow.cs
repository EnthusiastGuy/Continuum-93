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
        
        // Hover pop-up tracking
        private MemoryHoverPopup _hoverPopup;
        private int _hoveredLineIndex = -1;
        private int _hoveredByteIndex = -1;
        private int _previousHoveredLineIndex = -1;
        private int _previousHoveredByteIndex = -1;
        private float _hoverTimer = 0f;
        private const float HoverDelay = 0.3f; // 300ms

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

        // Draw the pop-up (called from ServiceGraphics after all windows are drawn)
        public void DrawHoverPopup()
        {
            if (_hoverPopup != null && _hoverPopup.Visible)
            {
                _hoverPopup.Draw();
            }
        }
        
        // Public accessor so the window manager can draw the popup on top
        public MemoryHoverPopup GetHoverPopup() => _hoverPopup;

        protected override void UpdateContent(GameTime gameTime)
        {
            Memory.Update();

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var mouse = Mouse.GetState();
            Point mousePos = new Point(mouse.X, mouse.Y);

            // Handle mouse wheel scrolling
            if (ContentRect.Contains(mousePos))
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

            // Update hover pop-up
            UpdateHoverPopup(dt, mousePos);
            
            // Update pop-up directly (not managed by WindowManager)
            if (_hoverPopup != null && _hoverPopup.Visible)
            {
                _hoverPopup.Update(gameTime);
            }
        }

        private void UpdateHoverPopup(float dt, Point mousePos)
        {
            // Hide pop-up if MemoryWindow is not visible
            if (!Visible)
            {
                if (_hoverPopup != null && _hoverPopup.Visible)
                {
                    HideHoverPopup();
                }
                _hoverTimer = 0f;
                return;
            }

            // Check if mouse is over the pop-up (keep it open)
            bool mouseOverPopup = _hoverPopup != null && _hoverPopup.Visible && _hoverPopup.Bounds.Contains(mousePos);
            
            // If mouse moved to a different byte or outside, reset timer
            if (_hoveredLineIndex != _previousHoveredLineIndex || _hoveredByteIndex != _previousHoveredByteIndex)
            {
                _hoverTimer = 0f;
                _previousHoveredLineIndex = _hoveredLineIndex;
                _previousHoveredByteIndex = _hoveredByteIndex;
            }

            // If hovering over a valid byte and not over pop-up, increment timer
            if (_hoveredLineIndex >= 0 && _hoveredByteIndex >= 0 && !mouseOverPopup)
            {
                _hoverTimer += dt;
                
                // Show pop-up after delay
                if (_hoverTimer >= HoverDelay)
                {
                    if (_hoverPopup == null || !_hoverPopup.Visible)
                    {
                        ShowHoverPopup();
                    }
                    else
                    {
                        // Update pop-up data if byte changed
                        UpdateHoverPopupData();
                    }
                }
            }
            else
            {
                // Not hovering over a byte, or mouse is outside
                if (!mouseOverPopup)
                {
                    // Hide pop-up if mouse moved away
                    if (_hoverPopup != null && _hoverPopup.Visible)
                    {
                        HideHoverPopup();
                    }
                    _hoverTimer = 0f;
                }
            }
        }

        private void ShowHoverPopup()
        {
            if (_hoveredLineIndex < 0 || _hoveredByteIndex < 0)
                return;

            var lines = Memory.Lines;
            if (_hoveredLineIndex >= lines.Count)
                return;

            var line = lines[_hoveredLineIndex];
            string[] hexParts = line.HexBytes.TrimEnd().Split(' ');
            
            if (_hoveredByteIndex >= hexParts.Length)
                return;

            // Parse the byte value
            if (!byte.TryParse(hexParts[_hoveredByteIndex], System.Globalization.NumberStyles.HexNumber, null, out byte byteValue))
                return;

            // Calculate address
            int address = line.Address + _hoveredByteIndex;

            // Position pop-up near the hovered byte
            const int lineHeight = 18;
            int startY = ContentRect.Y + Padding + 24;
            int byteY = startY + _hoveredLineIndex * lineHeight;
            
            var theme = ServiceGraphics.Theme;
            byte fontFlags = (byte)(ServiceFontFlags.Monospace | ServiceFontFlags.DrawOutline);
            int addressWidth = theme.PrimaryFont.MeasureText(
                line.TextAddress,
                ContentRect.Width - Padding * 2,
                fontFlags
            ).width;
            const int ColumnSpacing = 20;
            int hexColumnX = ContentRect.X + Padding + addressWidth + ColumnSpacing;
            
            // Calculate X position of the hovered byte
            int byteX = hexColumnX;
            for (int j = 0; j < _hoveredByteIndex; j++)
            {
                int byteWidth = theme.PrimaryFont.MeasureText(hexParts[j], 0, fontFlags).width;
                byteX += byteWidth;
                if (j < hexParts.Length - 1)
                {
                    int spaceWidth = theme.PrimaryFont.MeasureText(" ", 0, fontFlags).width;
                    byteX += spaceWidth;
                }
            }

            // Position pop-up to the right and slightly below the byte
            int popupX = byteX + 20;
            int popupY = byteY;
            
            // Ensure pop-up stays on screen
            var device = Renderer.GetGraphicsDevice();
            int popupWidth = 200;
            int popupHeight = 100;
            if (popupX + popupWidth > device.Viewport.Width)
                popupX = byteX - popupWidth - 10; // Show to the left instead
            if (popupY + popupHeight > device.Viewport.Height)
                popupY = device.Viewport.Height - popupHeight - 10;

            if (_hoverPopup == null)
            {
                _hoverPopup = new MemoryHoverPopup(popupX, popupY, address, byteValue);
                // Don't add to WindowManager - manage it directly to avoid collection modification issues
            }
            else
            {
                _hoverPopup.X = popupX;
                _hoverPopup.Y = popupY;
                _hoverPopup.UpdateData(address, byteValue);
                _hoverPopup.Visible = true;
            }
        }

        private void UpdateHoverPopupData()
        {
            if (_hoverPopup == null || _hoveredLineIndex < 0 || _hoveredByteIndex < 0)
                return;

            var lines = Memory.Lines;
            if (_hoveredLineIndex >= lines.Count)
                return;

            var line = lines[_hoveredLineIndex];
            string[] hexParts = line.HexBytes.TrimEnd().Split(' ');
            
            if (_hoveredByteIndex >= hexParts.Length)
                return;

            if (!byte.TryParse(hexParts[_hoveredByteIndex], System.Globalization.NumberStyles.HexNumber, null, out byte byteValue))
                return;

            int address = line.Address + _hoveredByteIndex;
            _hoverPopup.UpdateData(address, byteValue);
        }

        private void HideHoverPopup()
        {
            if (_hoverPopup != null)
            {
                _hoverPopup.Visible = false;
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
            _hoveredLineIndex = -1;
            _hoveredByteIndex = -1;
            if (contentRect.Contains(mousePos))
            {
                int localY = mousePos.Y - startY;
                if (localY >= 0)
                {
                    _hoveredLineIndex = localY / lineHeight;
                    if (_hoveredLineIndex >= 0 && _hoveredLineIndex < visibleLines)
                    {
                        var hoveredLine = lines[_hoveredLineIndex];
                        
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
                                    _hoveredByteIndex = j;
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

                // Address with leading zero transparency
                string address = line.TextAddress;
                int addressX = contentRect.X + Padding;
                
                bool nonZero = false;
                for (int k = 0; k < address.Length; k++)
                {
                    if (address[k] != '0')
                        nonZero = true;

                    Color charColor = theme.TextPrimary;
                    if (!nonZero && address[k] == '0')
                        charColor = theme.NumberLeadingZeroes;

                    ServiceGraphics.DrawText(
                        theme.PrimaryFont,
                        address[k].ToString(),
                        addressX + k * charWidth,
                        y,
                        contentRect.Width - Padding * 2,
                        charColor,
                        theme.TextOutline,
                        fontFlags,
                        0xFF
                    );
                }

                // Measure address text width for column positioning
                int addressWidth = theme.PrimaryFont.MeasureText(
                    line.TextAddress,
                    contentRect.Width - Padding * 2,
                    fontFlags
                ).width;

                // Parse hex bytes to get individual bytes
                string[] hexParts = line.HexBytes.TrimEnd().Split(' ');
                int hexColumnX = contentRect.X + Padding + addressWidth + ColumnSpacing;

                // Calculate actual hex bytes column width by summing individual measurements
                int hexBytesWidth = 0;
                int spaceWidth = theme.PrimaryFont.MeasureText(" ", 0, fontFlags).width;
                for (int j = 0; j < hexParts.Length && j < 16; j++)
                {
                    int hexByteTextWidth = theme.PrimaryFont.MeasureText(hexParts[j], 0, fontFlags).width;
                    hexBytesWidth += hexByteTextWidth;
                    // Add space after hex byte (except last)
                    if (j < hexParts.Length - 1)
                    {
                        hexBytesWidth += spaceWidth;
                    }
                }

                // Draw hex bytes individually for hover detection
                int currentHexX = hexColumnX;
                for (int j = 0; j < hexParts.Length && j < 16; j++)
                {
                    bool isHovered = (i == _hoveredLineIndex && j == _hoveredByteIndex);
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
                        currentHexX += spaceWidth;
                    }
                }

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
                    bool isHovered = (i == _hoveredLineIndex && j == _hoveredByteIndex);
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

