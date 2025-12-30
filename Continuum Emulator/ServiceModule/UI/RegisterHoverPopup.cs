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
            : base("", x, y, 350, 200, 0f, false, false) // No title, no resize, no close
        {
            _regIndex = regIndex;
            _bitWidth = bitWidth;
            _currentValue = currentValue;
            _previousValue = previousValue;
            IsOnTop = true; // Always on top
            // Set height immediately to full size (no spawn animation)
            Height = 200;
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

            // Register name
            string regName = RegistryUtils.GetNBitRegisterName(_regIndex, _bitWidth);
            string regNameText = $"Register: {regName} ({_bitWidth * 8}-bit)";
            ServiceGraphics.DrawText(
                theme.PrimaryFont,
                regNameText,
                contentRect.X + Padding,
                y,
                contentRect.Width - Padding * 2,
                theme.TextPrimary,
                theme.TextOutline,
                fontFlags,
                0xFF
            );
            y += lineHeight + 2; // Extra spacing

            // Current value section
            ServiceGraphics.DrawText(
                theme.PrimaryFont,
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
                theme.PrimaryFont,
                currentHexText,
                contentRect.X + Padding,
                y,
                contentRect.Width - Padding * 2,
                theme.TextPrimary,
                theme.TextOutline,
                fontFlags,
                0xFF
            );
            y += lineHeight;

            // Current - Decimal
            string currentDecimalText = $"  Decimal: {_currentValue}";
            ServiceGraphics.DrawText(
                theme.PrimaryFont,
                currentDecimalText,
                contentRect.X + Padding,
                y,
                contentRect.Width - Padding * 2,
                theme.TextPrimary,
                theme.TextOutline,
                fontFlags,
                0xFF
            );
            y += lineHeight;

            // Current - Binary
            int binaryDigits = _bitWidth * 8;
            string currentBinaryText = $"  Binary:  {Convert.ToString(_currentValue, 2).PadLeft(binaryDigits, '0')}b";
            ServiceGraphics.DrawText(
                theme.PrimaryFont,
                currentBinaryText,
                contentRect.X + Padding,
                y,
                contentRect.Width - Padding * 2,
                theme.TextPrimary,
                theme.TextOutline,
                fontFlags,
                0xFF
            );
            y += lineHeight;

            // Current - Octal
            int octalDigits = (binaryDigits + 2) / 3; // Calculate required octal digits (round up)
            string currentOctalText = $"  Octal:   {Convert.ToString((long)_currentValue, 8).PadLeft(octalDigits, '0')}o";
            ServiceGraphics.DrawText(
                theme.PrimaryFont,
                currentOctalText,
                contentRect.X + Padding,
                y,
                contentRect.Width - Padding * 2,
                theme.TextPrimary,
                theme.TextOutline,
                fontFlags,
                0xFF
            );
            y += lineHeight + 2; // Extra spacing

            // Previous value section
            ServiceGraphics.DrawText(
                theme.PrimaryFont,
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

            // Previous - Hex
            string previousHexText = $"  Hex:     0x{_previousValue.ToString($"X{hexDigits}")}";
            Color previousColor = _currentValue != _previousValue ? theme.RegisterValueChangedColor : theme.TextPrimary;
            ServiceGraphics.DrawText(
                theme.PrimaryFont,
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
                theme.PrimaryFont,
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
            string previousBinaryText = $"  Binary:  {Convert.ToString(_previousValue, 2).PadLeft(binaryDigits, '0')}b";
            ServiceGraphics.DrawText(
                theme.PrimaryFont,
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
            string previousOctalText = $"  Octal:   {Convert.ToString((long)_previousValue, 8).PadLeft(octalDigits, '0')}o";
            ServiceGraphics.DrawText(
                theme.PrimaryFont,
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

        // Hide base ContentRect since it's not virtual - we need no title bar
        public new Rectangle ContentRect =>
            new(
                X + BorderWidth,
                Y + BorderWidth, // No title bar, start from top border
                Width - BorderWidth * 2,
                Height - BorderWidth * 2
            );

        // Override to prevent input capture - pop-up should not be interactive
        public override bool HandleInput(MouseState mouse, MouseState prevMouse)
        {
            // Don't capture any input - let it pass through
            return false;
        }
    }
}

