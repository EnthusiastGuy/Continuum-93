using Continuum93.Emulator;
using Continuum93.ServiceModule.Parsers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Globalization;

namespace Continuum93.ServiceModule.UI
{
    public class RegisterWindow : Window
    {
        private const int ColumnSpacing = 20; // Spacing between columns

        // Hover pop-up tracking
        private RegisterHoverPopup _hoverPopup;
        private int _hoveredRegIndex = -1;
        private int _hoveredColumn = -1; // 0 = 32-bit, 1 = 8-bit, 2 = 16-bit, 3 = 24-bit
        private int _previousHoveredRegIndex = -1;
        private int _previousHoveredColumn = -1;
        private float _hoverTimer = 0f;
        private const float HoverDelay = 0.3f; // 300ms

        // Store column positions for hover detection
        private int[] _columnXPositions = new int[4];
        private int[] _columnWidths = new int[4];

        public RegisterWindow(
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
        public RegisterHoverPopup GetHoverPopup() => _hoverPopup;

        protected override void UpdateContent(GameTime gameTime)
        {
            CPUState.Update();

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var mouse = Mouse.GetState();
            Point mousePos = new Point(mouse.X, mouse.Y);

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
            // Hide pop-up if RegisterWindow is not visible
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

            // If mouse moved to a different register/column or outside, reset timer
            if (_hoveredRegIndex != _previousHoveredRegIndex || _hoveredColumn != _previousHoveredColumn)
            {
                _hoverTimer = 0f;
                _previousHoveredRegIndex = _hoveredRegIndex;
                _previousHoveredColumn = _hoveredColumn;
            }

            // If hovering over a valid register and not over pop-up, increment timer
            if (_hoveredRegIndex >= 0 && _hoveredColumn >= 0 && !mouseOverPopup)
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
                        // Update pop-up data if register changed
                        UpdateHoverPopupData();
                    }
                }
            }
            else
            {
                // Not hovering over a register, or mouse is outside
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
            if (_hoveredRegIndex < 0 || _hoveredColumn < 0)
                return;

            byte[] regPage0 = CPUState.RegPage0;
            byte[] regPageOld = CPUState.RegPageOld;

            // Calculate bit width: 0 = 32-bit (4), 1 = 8-bit (1), 2 = 16-bit (2), 3 = 24-bit (3)
            int colPosition = (_hoveredColumn + 3) % 4; // Convert to internal column order
            int bitWidth = colPosition + 1;

            // Get current and previous values
            uint currentValue = GetRegisterValue(_hoveredRegIndex, bitWidth, regPage0);
            uint previousValue = GetRegisterValue(_hoveredRegIndex, bitWidth, regPageOld);

            // Calculate position for popup
            const int lineHeight = 18;
            int startY = ContentRect.Y + Padding + 24;
            int regY = startY + _hoveredRegIndex * lineHeight;

            // Position pop-up to the right of the hovered register value
            int popupX = _columnXPositions[_hoveredColumn] + _columnWidths[_hoveredColumn] + 10;
            int popupY = regY;

            // Ensure pop-up stays on screen
            var device = Renderer.GetGraphicsDevice();
            int popupWidth = 350;
            int popupHeight = 200;
            if (popupX + popupWidth > device.Viewport.Width)
                popupX = _columnXPositions[_hoveredColumn] - popupWidth - 10; // Show to the left instead
            if (popupY + popupHeight > device.Viewport.Height)
                popupY = device.Viewport.Height - popupHeight - 10;

            if (_hoverPopup == null)
            {
                _hoverPopup = new RegisterHoverPopup(popupX, popupY, _hoveredRegIndex, bitWidth, currentValue, previousValue);
            }
            else
            {
                _hoverPopup.X = popupX;
                _hoverPopup.Y = popupY;
                _hoverPopup.UpdateData(_hoveredRegIndex, bitWidth, currentValue, previousValue);
                _hoverPopup.Visible = true;
            }
        }

        private void UpdateHoverPopupData()
        {
            if (_hoverPopup == null || _hoveredRegIndex < 0 || _hoveredColumn < 0)
                return;

            byte[] regPage0 = CPUState.RegPage0;
            byte[] regPageOld = CPUState.RegPageOld;

            int colPosition = (_hoveredColumn + 3) % 4;
            int bitWidth = colPosition + 1;

            uint currentValue = GetRegisterValue(_hoveredRegIndex, bitWidth, regPage0);
            uint previousValue = GetRegisterValue(_hoveredRegIndex, bitWidth, regPageOld);

            _hoverPopup.UpdateData(_hoveredRegIndex, bitWidth, currentValue, previousValue);
        }

        private void HideHoverPopup()
        {
            if (_hoverPopup != null)
            {
                _hoverPopup.Visible = false;
            }
        }

        private uint GetRegisterValue(int regIndex, int bitWidth, byte[] regs)
        {
            // Build hex string from register bytes
            string hexValue = RegistryUtils.GetHexValueForNBitRegister(regIndex, bitWidth, regs);
            return uint.Parse(hexValue, NumberStyles.HexNumber);
        }

        protected override void DrawContent(SpriteBatch spriteBatch, Rectangle contentRect)
        {
            const int lineHeight = 18;
            const int charWidth = 13;
            var theme = ServiceGraphics.Theme;

            byte[] regPage0 = CPUState.RegPage0;
            byte[] regPageOld = CPUState.RegPageOld;
            byte regPageIndex = CPUState.RegPageIndex;
            byte[] regMemoryData = CPUState.RegMemoryData;

            // Get mouse position for hover detection
            var mouse = Mouse.GetState();
            Point mousePos = new Point(mouse.X, mouse.Y);

            // Reset hover state
            _hoveredRegIndex = -1;
            _hoveredColumn = -1;

            // Title
            ServiceGraphics.DrawText(
                Fonts.ModernDOS_12x18,
                $"Register view (bank: {regPageIndex})",
                contentRect.X + Padding,
                contentRect.Y + Padding,
                contentRect.Width - Padding * 2,
                theme.TextTitle,
                theme.TextOutline,
                (byte)ServiceFontFlags.DrawOutline,
                0xFF
            );

            int startY = contentRect.Y + Padding + 24;
            byte fontFlags = (byte)(ServiceFontFlags.Monospace | ServiceFontFlags.DrawOutline);

            // width of "FF" with the active font rules (monospace + spacing + outline pad)
            int byteCellWidth = theme.PrimaryFont.MeasureText("FF", 0, fontFlags).width;

            // extra gap between bytes so it doesn�t look cramped
            const int byteGap = 4;
            int byteStride = byteCellWidth + byteGap;

            // Draw registers
            for (int i = 0; i < 26; i++)
            {
                int y = startY + i * lineHeight;
                if (y + lineHeight > contentRect.Bottom)
                    break;

                // Draw register name and values (1-4 bit)
                int currentX = contentRect.X + Padding;
                for (int j = 0; j < 4; j++)
                {
                    int colPosition = (j + 3) % 4;  // Starting with the 32 bit register column, then 8, 16 and finally
                    // 24 bit, so we can represent them as address:value pairs more naturally

                    string regName = RegistryUtils.GetNBitRegisterName(i, 1 + colPosition);
                    string hexValue = RegistryUtils.GetHexValueForNBitRegister(i, 1 + colPosition, regPage0);
                    string oldHexValue = RegistryUtils.GetHexValueForNBitRegister(i, 1 + colPosition, regPageOld);
                    Color regColor = RegistryUtils.GetRegisterStateColor(i);

                    // Measure the full column text (register name + ":" + hex value")
                    string fullColumnText = $"{regName}:{hexValue}";
                    int columnWidth = theme.PrimaryFont.MeasureText(
                        fullColumnText,
                        contentRect.Width - Padding * 2,
                        fontFlags
                    ).width;

                    // Store column position and width for hover detection (only for first row)
                    if (i == 0)
                    {
                        _columnXPositions[j] = currentX;
                        _columnWidths[j] = columnWidth;
                    }

                    // Check if mouse is hovering over this register value
                    if (ContentRect.Contains(mousePos) && 
                        mousePos.Y >= y && mousePos.Y < y + lineHeight &&
                        mousePos.X >= currentX && mousePos.X < currentX + columnWidth)
                    {
                        _hoveredRegIndex = i;
                        _hoveredColumn = j;
                    }

                    // Register name
                    ServiceGraphics.DrawText(
                        theme.PrimaryFont,
                        $"{regName}:",
                        currentX,
                        y,
                        contentRect.Width + Padding * 9,
                        regColor,
                        theme.TextOutline,
                        fontFlags,
                        0xFF
                    );

                    // Hex value with change detection
                    string regNameWithColon = $"{regName}:";
                    int regNameWidth = theme.PrimaryFont.MeasureText(
                        regNameWithColon,
                        contentRect.Width - Padding * 2,
                        fontFlags
                    ).width;
                    int hexX = currentX + regNameWidth;
                    bool valueChanged = hexValue != oldHexValue;
                    Color hexColor = valueChanged ? theme.RegisterValueChangedColor : theme.RegisterValueUnchangedColor;

                    // Draw hex value with leading zero transparency
                    bool nonZero = false;
                    for (int k = 0; k < hexValue.Length; k++)
                    {
                        if (hexValue[k] != '0')
                            nonZero = true;

                        Color charColor = hexColor;
                        if (!nonZero && hexValue[k] == '0')
                            charColor = theme.NumberLeadingZeroes;

                        if (k < oldHexValue.Length && hexValue[k] != oldHexValue[k])
                            charColor = theme.RegisterValueChangedColor;

                        ServiceGraphics.DrawText(
                            theme.PrimaryFont,
                            hexValue[k].ToString(),
                            hexX + k * charWidth,
                            y,
                            contentRect.Width - Padding * 2,
                            charColor,
                            theme.TextOutline,
                            fontFlags,
                            0xFF
                        );
                    }

                    // Update X position for next column: current position + column width + spacing
                    currentX += columnWidth + ColumnSpacing;
                }

                // Draw separator
                ServiceGraphics.DrawText(
                    theme.PrimaryFont,
                    ">",
                    contentRect.X + Padding + 530,
                    startY + i * lineHeight,
                    contentRect.Width - Padding * 2,
                    theme.TextPrimary,
                    theme.TextOutline,
                    (byte)(ServiceFontFlags.Monospace | ServiceFontFlags.DrawOutline),
                    0xFF
                );




                // Draw memory data pointed by register
                int memBaseX = contentRect.X + Padding + 550;

                for (int j = 0; j < 6; j++)
                {
                    int idx = i * 16 + j;
                    if (idx >= regMemoryData.Length)
                        break;

                    int memX = memBaseX + j * byteStride;
                    string hexMem = regMemoryData[idx].ToString("X2");

                    ServiceGraphics.DrawText(
                        theme.PrimaryFont,
                        hexMem,
                        memX,
                        startY + i * lineHeight,
                        contentRect.Width - Padding * 2,
                        theme.RegisterMemoryDataColor,
                        theme.TextOutline,
                        fontFlags,
                        0xFF
                    );
                }
            }
        }
    }
}

