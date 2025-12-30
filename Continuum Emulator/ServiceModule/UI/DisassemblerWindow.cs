using Continuum93.Emulator;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Continuum93.ServiceModule.UI
{
    public class DisassemblerWindow : Window
    {
        private DisassemblerHoverPopup _hoverPopup;
        private int _hoveredLineIndex = -1;          // Absolute index in the lines list
        private int _hoveredVisibleIndex = -1;       // Index in the visible range (for positioning)
        private int _previousHoveredLineIndex = -1;
        private float _hoverTimer = 0f;
        private const float HoverDelay = 0.5f; // 500ms delay before showing popup
        
        public DisassemblerWindow(
            string title,
            int x, int y,
            int width, int height,
            float spawnDelaySeconds = 0,
            bool canResize = true,
            bool canClose = false)
            : base(title, x, y, width, height, spawnDelaySeconds, canResize, canClose)
        {
        }
        
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            if (!Visible) return;
            
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var mouse = Mouse.GetState();
            Point mousePos = new(mouse.X, mouse.Y);
            
            UpdateHoverPopup(dt, mousePos);
        }
        
        private void UpdateHoverPopup(float dt, Point mousePos)
        {
            // Hide pop-up if DisassemblerWindow is not visible
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
            
            // If mouse moved to a different line, reset timer
            if (_hoveredLineIndex != _previousHoveredLineIndex)
            {
                _hoverTimer = 0f;
                _previousHoveredLineIndex = _hoveredLineIndex;
            }

            // If hovering over a valid line and not over pop-up, increment timer
            if (_hoveredLineIndex >= 0 && !mouseOverPopup)
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
                        // Update pop-up data if line changed
                        UpdateHoverPopupData();
                    }
                }
            }
            else
            {
                // Not hovering over a line, or mouse is outside
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
            if (_hoveredLineIndex < 0 || _hoveredVisibleIndex < 0)
                return;

            var lines = Parsers.Disassembled.Lines;
            if (_hoveredLineIndex >= lines.Count)
                return;

            var line = lines[_hoveredLineIndex];
            
            // Extract instruction name (first word of instruction)
            string instruction = line.Instruction ?? "";
            string instructionName = instruction.Split(' ')[0].Trim();
            
            if (string.IsNullOrEmpty(instructionName))
                return;

            // Calculate position for popup using visible index
            const int lineHeight = 18;
            int lineY = ContentRect.Y + Padding + _hoveredVisibleIndex * lineHeight;
            
            // Position pop-up to the right of the window
            int popupX = Bounds.Right + 10;
            int popupY = lineY;
            
            // Ensure pop-up stays on screen
            var device = Renderer.GetGraphicsDevice();
            int popupWidth = 500;
            int popupHeight = 400;
            if (popupX + popupWidth > device.Viewport.Width)
                popupX = Bounds.Left - popupWidth - 10; // Show to the left instead
            if (popupY + popupHeight > device.Viewport.Height)
                popupY = device.Viewport.Height - popupHeight - 10;

            if (_hoverPopup == null)
            {
                _hoverPopup = new DisassemblerHoverPopup(
                    popupX, popupY, 
                    instructionName, 
                    line.Instruction, 
                    line.OpCodes ?? ""
                );
            }
            else
            {
                _hoverPopup.X = popupX;
                _hoverPopup.Y = popupY;
                _hoverPopup.UpdateData(instructionName, line.Instruction, line.OpCodes ?? "");
                _hoverPopup.Visible = true;
            }
        }

        private void UpdateHoverPopupData()
        {
            if (_hoverPopup == null || _hoveredLineIndex < 0)
                return;

            var lines = Parsers.Disassembled.Lines;
            if (_hoveredLineIndex >= lines.Count)
                return;

            var line = lines[_hoveredLineIndex];
            string instruction = line.Instruction ?? "";
            string instructionName = instruction.Split(' ')[0].Trim();
            
            if (string.IsNullOrEmpty(instructionName))
                return;

            _hoverPopup.UpdateData(instructionName, line.Instruction, line.OpCodes ?? "");
        }

        private void HideHoverPopup()
        {
            if (_hoverPopup != null)
            {
                _hoverPopup.Visible = false;
            }
        }
        
        // Public accessor so the window manager can draw the popup on top
        public DisassemblerHoverPopup HoverPopup => _hoverPopup;

        protected override void DrawContent(SpriteBatch spriteBatch, Rectangle contentRect)
        {
            var lines = Parsers.Disassembled.Lines;
            if (lines == null || lines.Count == 0)
                return;

            // layout / metrics
            const int lineHeight = 18;    // matches font size
            const int charWidth = 13;     // monospaced font width

            // Account for the clipped content area (Window.DrawContentClipped applies a 4px inset scissor)
            // plus the top padding we use for text.
            int visibleHeight = Math.Max(1, contentRect.Height - 4 - Padding);
            int linesPerPage = Math.Max(1, visibleHeight / lineHeight);
            int visibleCount = Math.Min(linesPerPage, lines.Count);

            // Get current IP address
            uint currentIP = Machine.COMPUTER?.CPU.REGS.IPO ?? 0;

            // Determine which slice of lines to show so current IP stays in view
            int focusIndex = lines.FindIndex(l => l.Address == currentIP);
            if (focusIndex < 0) focusIndex = 0;

            int startIndex = Math.Max(0, focusIndex - visibleCount / 2);
            if (startIndex + visibleCount > lines.Count)
                startIndex = Math.Max(0, lines.Count - visibleCount);

            // Ensure the focused line is actually within the visible slice (in case of rounding)
            if (focusIndex < startIndex)
                startIndex = focusIndex;
            if (focusIndex >= startIndex + visibleCount)
                startIndex = Math.Max(0, focusIndex - visibleCount + 1);

            // assume addresses are fixed-width already, but compute max just in case
            int maxAddrLen = 0;
            for (int i = 0; i < visibleCount; i++)
            {
                var addr = lines[startIndex + i].TextAddress ?? string.Empty;
                if (addr.Length > maxAddrLen)
                    maxAddrLen = addr.Length;
            }

            // column X positions (no opcodes column anymore)
            int addrColumnX = contentRect.X + Padding;
            int addrColumnWidth = maxAddrLen * charWidth;
            int instrColumnX = addrColumnX + addrColumnWidth + (charWidth * 2);  // +2 spaces for separation

            var theme = ServiceGraphics.Theme;
            Color addrFull = theme.DisassemblerAddressFull;
            Color addrZero = theme.DisassemblerAddressZero;
            Color addrZeroOutline = theme.DisassemblerAddressZeroOutline;
            Color opcodeColor = theme.DisassemblerOpcodeColor;
            Color instrColor = theme.DisassemblerInstructionColor;
            Color hoverBorder = theme.DisassemblerHoverBorder;

            byte fontFlags = (byte)(ServiceFontFlags.Monospace |
                                    ServiceFontFlags.DrawOutline);

            var pixel = Renderer.GetPixelTexture();
            var mouse = Mouse.GetState();
            Point mousePos = new(mouse.X, mouse.Y);

            // figure out which line (if any) is hovered
            int hoverIndex = -1;
            _hoveredLineIndex = -1;
            _hoveredVisibleIndex = -1;
            if (contentRect.Contains(mousePos))
            {
                int localY = mousePos.Y - (contentRect.Y + Padding);
                if (localY >= 0)
                {
                    int idx = localY / lineHeight;
                    if (idx >= 0 && idx < visibleCount)
                    {
                        hoverIndex = idx;
                        _hoveredLineIndex = startIndex + idx;  // Store absolute index
                        _hoveredVisibleIndex = idx;            // Store visible index
                    }
                }
            }

            for (int i = 0; i < visibleCount; i++)
            {
                var line = lines[startIndex + i];
                int y = contentRect.Y + Padding + i * lineHeight;

                // Highlight current instruction
                if (line.Address == currentIP)
                {
                    Rectangle currentRect = new(
                        contentRect.X + 1,
                        y - 1,
                        contentRect.Width - 2,
                        lineHeight + 2
                    );
                    spriteBatch.Draw(pixel, currentRect, theme.DisassemblerCurrentInstructionBackground);
                }

                // --- hover highlight border ---
                if (i == hoverIndex)
                {
                    Rectangle hoverRect = new(
                        contentRect.X + 1,
                        y,
                        contentRect.Width - 2,
                        lineHeight
                    );

                    // top
                    spriteBatch.Draw(pixel, new Rectangle(hoverRect.X, hoverRect.Y-1, hoverRect.Width, 1), hoverBorder);
                    // bottom
                    spriteBatch.Draw(pixel, new Rectangle(hoverRect.X, hoverRect.Bottom - 1, hoverRect.Width, 1), hoverBorder);
                    // left
                    spriteBatch.Draw(pixel, new Rectangle(hoverRect.X, hoverRect.Y, 1, hoverRect.Height), hoverBorder);
                    // right
                    spriteBatch.Draw(pixel, new Rectangle(hoverRect.Right - 1, hoverRect.Y, 1, hoverRect.Height), hoverBorder);
                }

                // --- address with faded leading zeros ---
                string addr = line.TextAddress ?? string.Empty;

                // pad address to max width for alignment (left-padded with zeros)
                if (addr.Length < maxAddrLen)
                    addr = addr.PadLeft(maxAddrLen, '0');

                int firstNonZero = 0;
                while (firstNonZero < addr.Length && addr[firstNonZero] == '0')
                    firstNonZero++;

                string addrZeros = addr.Substring(0, firstNonZero);
                string addrRest = addr.Substring(firstNonZero);

                int addrX = addrColumnX;

                if (addrZeros.Length > 0)
                {
                    ServiceGraphics.DrawText(
                        theme.PrimaryFont,
                        addrZeros,
                        addrX,
                        y,
                        contentRect.Width - Padding * 2,
                        addrZero,
                        addrZeroOutline,
                        fontFlags,
                        0xFF
                    );
                    addrX += addrZeros.Length * charWidth;
                }

                if (addrRest.Length > 0)
                {
                    ServiceGraphics.DrawText(
                        theme.PrimaryFont,
                        addrRest,
                        addrX,
                        y,
                        contentRect.Width - Padding * 2,
                        addrFull,
                        theme.TextOutline,
                        fontFlags,
                        0xFF
                    );
                }

                // --- instruction (orange) ---
                string instr = line.Instruction ?? string.Empty;

                ServiceGraphics.DrawText(
                    theme.PrimaryFont,
                    instr,
                    instrColumnX,
                    y,
                    contentRect.Width - Padding * 2,
                    instrColor,
                    theme.TextOutline,
                    fontFlags,
                    0xFF
                );
            }
        }
    }
}
