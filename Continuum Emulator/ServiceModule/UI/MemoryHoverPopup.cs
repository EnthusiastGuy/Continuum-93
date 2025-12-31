using Continuum93.ServiceModule;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Continuum93.ServiceModule.UI
{
    public class MemoryHoverPopup : Window
    {
        private int _address;
        private byte _value;

        public MemoryHoverPopup(int x, int y, int address, byte value)
            : base("", x, y, 300, 150, 0f, false, false) // No title, no resize, no close
        {
            _address = address;
            _value = value;
            IsOnTop = true; // Always on top
            // Set height immediately to full size (no spawn animation)
            Height = 150;
        }

        // Override Update to skip spawn animation - pop-up should appear immediately
        public override void Update(GameTime gameTime)
        {
            if (!Visible) return;
            // Skip spawn animation - just update content
            UpdateContent(gameTime);
        }

        public void UpdateData(int address, byte value)
        {
            _address = address;
            _value = value;
        }

        protected override void DrawChrome(SpriteBatch spriteBatch, Texture2D pixel)
        {
            // Draw pop-up without title bar - just border and background
            var bounds = Bounds;
            var theme = ServiceGraphics.Theme;

            // Border (1px)
            Rectangle top = new(bounds.X, bounds.Y, bounds.Width, BorderWidth);
            Rectangle bottom = new(bounds.X, bounds.Bottom - BorderWidth, bounds.Width, BorderWidth);
            Rectangle left = new(bounds.X, bounds.Y, BorderWidth, bounds.Height);
            Rectangle right = new(bounds.Right - BorderWidth, bounds.Y, BorderWidth, bounds.Height);

            Color borderColor = theme.WindowBorder;
            Color backgroundColor = theme.WindowBackgroundFocused; // Use focused background for visibility

            // Background
            spriteBatch.Draw(pixel, bounds, backgroundColor);

            // Border
            spriteBatch.Draw(pixel, top, borderColor);
            spriteBatch.Draw(pixel, bottom, borderColor);
            spriteBatch.Draw(pixel, left, borderColor);
            spriteBatch.Draw(pixel, right, borderColor);
        }

        protected override void DrawContent(SpriteBatch spriteBatch, Rectangle contentRect)
        {
            var theme = ServiceGraphics.Theme;
            byte fontFlags = (byte)(ServiceFontFlags.Monospace | ServiceFontFlags.DrawOutline);
            int lineHeight = 18;
            int y = contentRect.Y + Padding;

            // Address
            string addressText = $"Address: 0x{_address:X6}";
            ServiceGraphics.DrawText(
                theme.PrimaryFont,
                addressText,
                contentRect.X + Padding,
                y,
                contentRect.Width - Padding * 2,
                theme.TextPrimary,
                theme.TextOutline,
                fontFlags,
                0xFF
            );
            y += lineHeight;

            // Hex
            string hexText = $"Hex:     0x{_value:X2}";
            ServiceGraphics.DrawText(
                theme.PrimaryFont,
                hexText,
                contentRect.X + Padding,
                y,
                contentRect.Width - Padding * 2,
                theme.TextPrimary,
                theme.TextOutline,
                fontFlags,
                0xFF
            );
            y += lineHeight;

            // Binary
            string binaryText = $"Binary:  {Convert.ToString(_value, 2).PadLeft(8, '0')}b";
            ServiceGraphics.DrawText(
                theme.PrimaryFont,
                binaryText,
                contentRect.X + Padding,
                y,
                contentRect.Width - Padding * 2,
                theme.TextPrimary,
                theme.TextOutline,
                fontFlags,
                0xFF
            );
            y += lineHeight;

            // Decimal
            string decimalText = $"Decimal: {_value}";
            ServiceGraphics.DrawText(
                theme.PrimaryFont,
                decimalText,
                contentRect.X + Padding,
                y,
                contentRect.Width - Padding * 2,
                theme.TextPrimary,
                theme.TextOutline,
                fontFlags,
                0xFF
            );
            y += lineHeight;

            // ASCII
            bool isAscii = _value >= 32 && _value <= 127;
            string asciiChar = isAscii ? ((char)_value).ToString() : "?";
            string asciiText = $"ASCII:   '{asciiChar}'";
            Color asciiColor = isAscii ? theme.MemoryAsciiColor : theme.MemoryAsciiNonAsciiColor;
            ServiceGraphics.DrawText(
                theme.PrimaryFont,
                asciiText,
                contentRect.X + Padding,
                y,
                contentRect.Width - Padding * 2,
                asciiColor,
                theme.TextOutline,
                fontFlags,
                0xFF
            );
        }

        // Hide base ContentRect since it's not virtual - we need no title bar
        public new Rectangle ContentRect =>
            new(
                X + BorderWidth,
                Y + BorderWidth, // No title bar, start from top border
                Width - BorderWidth * 2,
                Height - BorderWidth * 2
            );

        // Override to block input from reaching windows underneath
        public override bool HandleInput(MouseState mouse, MouseState prevMouse)
        {
            if (!Visible)
                return false;
            
            // Block all input when mouse is over the pop-up to prevent it from reaching windows underneath
            Point mousePos = new Point(mouse.X, mouse.Y);
            if (Bounds.Contains(mousePos))
            {
                return true; // Consume input to prevent it from bubbling down
            }
            
            return false;
        }
    }
}

