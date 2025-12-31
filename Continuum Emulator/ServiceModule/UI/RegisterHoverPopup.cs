using Continuum93.ServiceModule;
using Continuum93.ServiceModule.Parsers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Continuum93.ServiceModule.UI
{
    public class RegisterHoverPopup : Window
    {
        private int _regIndex;
        private int _bitWidth; // 1 = 8-bit, 2 = 16-bit, 3 = 24-bit, 4 = 32-bit
        private uint _currentValue;
        private uint _previousValue;

        public RegisterHoverPopup(int x, int y, int regIndex, int bitWidth, uint currentValue, uint previousValue)
            : base("", x, y, 500, 250, 0f, false, false) // Title will be set, no resize, no close
        {
            _regIndex = regIndex;
            _bitWidth = bitWidth;
            _currentValue = currentValue;
            _previousValue = previousValue;
            IsOnTop = true; // Always on top
            // Set height immediately to full size (no spawn animation)
            Height = 250;
            UpdateTitle();
        }

        private void UpdateTitle()
        {
            string regName = RegistryUtils.GetNBitRegisterName(_regIndex, _bitWidth);
            Title = $"Register: {regName} ({_bitWidth * 8}-bit)";
        }

        // Override Update to skip spawn animation - pop-up should appear immediately
        public override void Update(GameTime gameTime)
        {
            if (!Visible) return;
            // Skip spawn animation - just update content
            UpdateContent(gameTime);
        }

        public void UpdateData(int regIndex, int bitWidth, uint currentValue, uint previousValue)
        {
            _regIndex = regIndex;
            _bitWidth = bitWidth;
            _currentValue = currentValue;
            _previousValue = previousValue;
            UpdateTitle();
        }

        protected override void DrawChrome(SpriteBatch spriteBatch, Texture2D pixel)
        {
            // Draw pop-up with title bar - use base implementation but with custom background transparency
            var bounds = Bounds;
            var theme = ServiceGraphics.Theme;

            // Border (1px)
            Rectangle top = new(bounds.X, bounds.Y, bounds.Width, BorderWidth);
            Rectangle bottom = new(bounds.X, bounds.Bottom - BorderWidth, bounds.Width, BorderWidth);
            Rectangle left = new(bounds.X, bounds.Y, BorderWidth, bounds.Height);
            Rectangle right = new(bounds.Right - BorderWidth, bounds.Y, BorderWidth, bounds.Height);

            Color borderColor = theme.WindowBorder;
            // Use WindowBackgroundFocused to match MemoryHoverPopup (same as MemoryHoverPopup uses)
            Color backgroundColor = theme.WindowBackgroundFocused;

            // Background
            if (ShouldDrawBackground)
                spriteBatch.Draw(pixel, bounds, backgroundColor);

            // Title bar area
            Color titleBarColor = IsOnTop ? theme.WindowTitleBarOnTop : theme.WindowTitleBarFocused;
            spriteBatch.Draw(pixel, TitleBarRect, titleBarColor);

            // Border
            spriteBatch.Draw(pixel, top, borderColor);
            spriteBatch.Draw(pixel, bottom, borderColor);
            spriteBatch.Draw(pixel, left, borderColor);
            spriteBatch.Draw(pixel, right, borderColor);

            // Title text
            ServiceGraphics.DrawText(
                Fonts.ModernDOS_12x18,
                Title,
                bounds.X + Padding,
                bounds.Y + 4,
                bounds.Width - Padding * 2,
                theme.WindowTitleText,
                theme.TextOutline,
                (byte)(ServiceFontFlags.DrawOutline),
                0xFF
            );
        }

        private string FormatBinaryWithSeparators(uint value, int bitWidth)
        {
            string binary = Convert.ToString(value, 2).PadLeft(bitWidth * 8, '0');
            
            // Add separator every 8 bits for 16, 24, or 32 bit values
            if (bitWidth >= 2)
            {
                string result = "";
                for (int i = 0; i < binary.Length; i += 8)
                {
                    if (i > 0)
                        result += " ";
                    int remaining = Math.Min(8, binary.Length - i);
                    result += binary.Substring(i, remaining);
                }
                return result;
            }
            
            return binary;
        }

        protected override void DrawContent(SpriteBatch spriteBatch, Rectangle contentRect)
        {
            var theme = ServiceGraphics.Theme;
            var font = Fonts.ModernDOS_9x15; // Use small font like DisassemblerHoverPopup
            byte fontFlags = (byte)(ServiceFontFlags.Monospace | ServiceFontFlags.DrawOutline);
            int lineHeight = 18;
            int y = contentRect.Y + Padding;

            bool valuesChanged = _currentValue != _previousValue;

            // Current value section
            ServiceGraphics.DrawText(
                font,
                "Current:",
                contentRect.X + Padding,
                y,
                contentRect.Width - Padding * 2,
                theme.TextPrimary,
                theme.TextOutline,
                fontFlags,
                0xFF
            );
            y += lineHeight;

            // Current - Hex
            int hexDigits = _bitWidth * 2;
            string currentHexText = $"  Hex:     0x{_currentValue.ToString($"X{hexDigits}")}";
            ServiceGraphics.DrawText(
                font,
                currentHexText,
                contentRect.X + Padding,
                y,
                contentRect.Width - Padding * 2,
                theme.RegisterValueChangedColor,
                theme.TextOutline,
                fontFlags,
                0xFF
            );
            y += lineHeight;

            // Current - Decimal
            string currentDecimalText = $"  Decimal: {_currentValue}";
            ServiceGraphics.DrawText(
                font,
                currentDecimalText,
                contentRect.X + Padding,
                y,
                contentRect.Width - Padding * 2,
                theme.RegisterValueChangedColor,
                theme.TextOutline,
                fontFlags,
                0xFF
            );
            y += lineHeight;

            // Current - Binary
            int binaryDigits = _bitWidth * 8;
            string currentBinaryText = $"  Binary:  {FormatBinaryWithSeparators(_currentValue, _bitWidth)}";
            ServiceGraphics.DrawText(
                font,
                currentBinaryText,
                contentRect.X + Padding,
                y,
                contentRect.Width - Padding * 2,
                theme.RegisterValueChangedColor,
                theme.TextOutline,
                fontFlags,
                0xFF
            );
            y += lineHeight;

            // Current - Octal
            int octalDigits = (binaryDigits + 2) / 3; // Calculate required octal digits (round up)
            string currentOctalText = $"  Octal:   {Convert.ToString((long)_currentValue, 8).PadLeft(octalDigits, '0')}";
            ServiceGraphics.DrawText(
                font,
                currentOctalText,
                contentRect.X + Padding,
                y,
                contentRect.Width - Padding * 2,
                theme.RegisterValueChangedColor,
                theme.TextOutline,
                fontFlags,
                0xFF
            );
            y += lineHeight + 2; // Extra spacing

            // Previous value section
            ServiceGraphics.DrawText(
                font,
                "Previous:",
                contentRect.X + Padding,
                y,
                contentRect.Width - Padding * 2,
                theme.TextPrimary,
                theme.TextOutline,
                fontFlags,
                0xFF
            );
            y += lineHeight;

            if (!valuesChanged)
            {
                // Show "no change" message if values are the same
                string noChangeText = "  no change";
                ServiceGraphics.DrawText(
                    font,
                    noChangeText,
                    contentRect.X + Padding,
                    y,
                    contentRect.Width - Padding * 2,
                    theme.TextSecondary,
                    theme.TextOutline,
                    fontFlags,
                    0xFF
                );
            }
            else
            {
                // Previous - Hex
                string previousHexText = $"  Hex:     0x{_previousValue.ToString($"X{hexDigits}")}";
                Color previousColor = theme.TextPrimary;
                ServiceGraphics.DrawText(
                    font,
                    previousHexText,
                    contentRect.X + Padding,
                    y,
                    contentRect.Width - Padding * 2,
                    previousColor,
                    theme.TextOutline,
                    fontFlags,
                    0xFF
                );
                y += lineHeight;

                // Previous - Decimal
                string previousDecimalText = $"  Decimal: {_previousValue}";
                ServiceGraphics.DrawText(
                    font,
                    previousDecimalText,
                    contentRect.X + Padding,
                    y,
                    contentRect.Width - Padding * 2,
                    previousColor,
                    theme.TextOutline,
                    fontFlags,
                    0xFF
                );
                y += lineHeight;

                // Previous - Binary
                string previousBinaryText = $"  Binary:  {FormatBinaryWithSeparators(_previousValue, _bitWidth)}";
                ServiceGraphics.DrawText(
                    font,
                    previousBinaryText,
                    contentRect.X + Padding,
                    y,
                    contentRect.Width - Padding * 2,
                    previousColor,
                    theme.TextOutline,
                    fontFlags,
                    0xFF
                );
                y += lineHeight;

                // Previous - Octal
                string previousOctalText = $"  Octal:   {Convert.ToString((long)_previousValue, 8).PadLeft(octalDigits, '0')}";
                ServiceGraphics.DrawText(
                    font,
                    previousOctalText,
                    contentRect.X + Padding,
                    y,
                    contentRect.Width - Padding * 2,
                    previousColor,
                    theme.TextOutline,
                    fontFlags,
                    0xFF
                );
            }
        }

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
