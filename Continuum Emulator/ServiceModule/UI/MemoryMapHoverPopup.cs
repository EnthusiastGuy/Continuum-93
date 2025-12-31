using Continuum93.Emulator;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Continuum93.ServiceModule.UI
{
    public class MemoryMapHoverPopup : Window
    {
        private const int ItemsPerPage = 10;

        private uint _startAddress;
        private int _byteCount;
        private byte[] _data = Array.Empty<byte>();
        private int _scrollOffset;
        private int _previousScroll;
        private string _titleText = string.Empty;

        public MemoryMapHoverPopup(int x, int y, uint startAddress, int byteCount, byte[] data)
            : base(string.Empty, x, y, 360, 280, 0f, false, false)
        {
            IsOnTop = true;
            Height = 280;
            Width = 360;
            UpdateData(startAddress, byteCount, data);
        }

        public override void Update(GameTime gameTime)
        {
            if (!Visible)
                return;

            var mouse = Mouse.GetState();
            int delta = mouse.ScrollWheelValue - _previousScroll;
            if (delta != 0)
            {
                int direction = delta > 0 ? -1 : 1;
                _scrollOffset = Math.Clamp(_scrollOffset + direction, 0, Math.Max(0, _byteCount - 1));
            }

            _previousScroll = mouse.ScrollWheelValue;
            UpdateContent(gameTime);
        }

        public void UpdateData(uint startAddress, int byteCount, byte[] data)
        {
            _startAddress = startAddress;
            _byteCount = Math.Max(0, byteCount);
            _scrollOffset = Math.Clamp(_scrollOffset, 0, Math.Max(0, _byteCount - 1));
            _data = data ?? Array.Empty<byte>();

            uint end = _byteCount > 0
                ? Math.Min(0xFFFFFF, _startAddress + (uint)Math.Max(0, _byteCount - 1))
                : _startAddress;
            _titleText = $"0x{_startAddress:X6} -> 0x{end:X6}";
        }

        protected override void DrawChrome(SpriteBatch spriteBatch, Texture2D pixel)
        {
            var bounds = Bounds;
            var theme = ServiceGraphics.Theme;

            Rectangle top = new(bounds.X, bounds.Y, bounds.Width, BorderWidth);
            Rectangle bottom = new(bounds.X, bounds.Bottom - BorderWidth, bounds.Width, BorderWidth);
            Rectangle left = new(bounds.X, bounds.Y, BorderWidth, bounds.Height);
            Rectangle right = new(bounds.Right - BorderWidth, bounds.Y, BorderWidth, bounds.Height);

            Color borderColor = theme.WindowBorder;
            Color backgroundColor = theme.WindowBackgroundFocused;
            Color titleColor = theme.WindowTitleBarFocused;

            // Background
            spriteBatch.Draw(pixel, bounds, backgroundColor);

            // Title bar
            Rectangle titleRect = new(bounds.X + BorderWidth, bounds.Y + BorderWidth, bounds.Width - BorderWidth * 2, TitleBarHeight);
            spriteBatch.Draw(pixel, titleRect, titleColor);

            ServiceGraphics.DrawText(
                theme.PrimaryFont,
                _titleText,
                titleRect.X + Padding,
                titleRect.Y + 2,
                titleRect.Width - Padding * 2,
                theme.WindowTitleText,
                theme.TextOutline,
                (byte)ServiceFontFlags.DrawOutline,
                0xFF);

            // Border
            spriteBatch.Draw(pixel, top, borderColor);
            spriteBatch.Draw(pixel, bottom, borderColor);
            spriteBatch.Draw(pixel, left, borderColor);
            spriteBatch.Draw(pixel, right, borderColor);
        }

        protected override void DrawContent(SpriteBatch spriteBatch, Rectangle contentRect)
        {
            var theme = ServiceGraphics.Theme;
            var font = Fonts.ModernDOS_9x15;
            byte fontFlags = (byte)(ServiceFontFlags.Monospace | ServiceFontFlags.DrawOutline);
            int lineHeight = 16;
            int y = contentRect.Y + Padding;

            ServiceGraphics.DrawText(
                font,
                $"Start: 0x{_startAddress:X6} ({_startAddress})",
                contentRect.X + Padding,
                y,
                contentRect.Width - Padding * 2,
                theme.TextPrimary,
                theme.TextOutline,
                fontFlags,
                0xFF
            );
            y += lineHeight;

            ServiceGraphics.DrawText(
                font,
                $"Bytes: {_byteCount}",
                contentRect.X + Padding,
                y,
                contentRect.Width - Padding * 2,
                theme.TextPrimary,
                theme.TextOutline,
                fontFlags,
                0xFF
            );
            y += lineHeight;

            ServiceGraphics.DrawText(
                font,
                "Values:",
                contentRect.X + Padding,
                y,
                contentRect.Width - Padding * 2,
                theme.TextSecondary,
                theme.TextOutline,
                fontFlags,
                0xFF
            );
            y += lineHeight;

            int linesDrawn = 0;
            for (int i = 0; i < ItemsPerPage; i++)
            {
                int index = _scrollOffset + i;
                if (index >= _byteCount || index >= _data.Length)
                    break;

                uint addr = _startAddress + (uint)index;
                byte value = _data[index];
                string line = $"{addr:X6} : {value:X2} ({value,3})";

                // Draw address and value
                ServiceGraphics.DrawText(
                    font,
                    line,
                    contentRect.X + Padding,
                    y,
                    contentRect.Width - Padding * 2,
                    theme.TextPrimary,
                    theme.TextOutline,
                    fontFlags,
                    0xFF
                );

                // Measure the width of the address/value line to position ASCII column
                var lineSize = font.MeasureText(
                    line,
                    0,
                    fontFlags
                );
                int asciiX = contentRect.X + Padding + lineSize.width + 20; // 20px spacing

                // Draw ASCII representation
                bool isAscii = value >= 32 && value <= 127;
                string asciiChar = isAscii ? ((char)value).ToString() : "?";
                Color asciiColor = isAscii ? theme.MemoryAsciiColor : theme.MemoryAsciiNonAsciiColor;
                
                string asciiText = $"'{asciiChar}'";
                ServiceGraphics.DrawText(
                    font,
                    asciiText,
                    asciiX,
                    y,
                    contentRect.Width - asciiX - Padding,
                    asciiColor,
                    theme.TextOutline,
                    fontFlags,
                    0xFF
                );

                // Measure ASCII text to position bit pattern
                var asciiSize = font.MeasureText(asciiText, 0, fontFlags);
                int bitPatternX = asciiX + asciiSize.width + 12; // 12px spacing after ASCII

                // Draw 8-bit pattern visualization
                const int bitSquareSize = 6;
                const int bitSpacing = 1;
                var pixel = Renderer.GetPixelTexture();
                Color bitZeroColor = new Color(64, 64, 64); // Dark gray for 0
                Color bitOneColor = Color.White; // White for 1

                int bitY = y + (lineHeight - bitSquareSize) / 2; // Center vertically with text
                for (int bit = 7; bit >= 0; bit--) // MSB to LSB
                {
                    bool bitSet = (value & (1 << bit)) != 0;
                    Color bitColor = bitSet ? bitOneColor : bitZeroColor;
                    int bitX = bitPatternX + (7 - bit) * (bitSquareSize + bitSpacing);
                    
                    Rectangle bitRect = new Rectangle(bitX, bitY, bitSquareSize, bitSquareSize);
                    spriteBatch.Draw(pixel, bitRect, bitColor);
                }

                y += lineHeight;
                linesDrawn++;
            }

            if (linesDrawn == 0)
            {
                ServiceGraphics.DrawText(
                    font,
                    "(no data)",
                    contentRect.X + Padding,
                    y,
                    contentRect.Width - Padding * 2,
                    theme.TextInfo,
                    theme.TextOutline,
                    fontFlags,
                    0xFF
                );
            }
        }

        public override bool HandleInput(MouseState mouse, MouseState prevMouse)
        {
            if (!Visible)
                return false;

            Point mousePos = new(mouse.X, mouse.Y);
            if (Bounds.Contains(mousePos))
            {
                return true;
            }

            return false;
        }
    }
}

