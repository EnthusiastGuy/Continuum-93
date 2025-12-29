using Continuum93.Emulator;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Continuum93.ServiceModule.UI
{
    public class DisassemblerWindow : Window
    {
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

            // max opcode length among visible lines
            int maxOpcodeLen = 0;
            for (int i = 0; i < visibleCount; i++)
            {
                var op = lines[startIndex + i].OpCodes ?? string.Empty;
                if (op.Length > maxOpcodeLen)
                    maxOpcodeLen = op.Length;
            }

            // assume addresses are fixed-width already, but compute max just in case
            int maxAddrLen = 0;
            for (int i = 0; i < visibleCount; i++)
            {
                var addr = lines[startIndex + i].TextAddress ?? string.Empty;
                if (addr.Length > maxAddrLen)
                    maxAddrLen = addr.Length;
            }

            // column X positions
            int addrColumnX = contentRect.X + Padding;
            int addrColumnWidth = maxAddrLen * charWidth;
            int opcodeColumnX = addrColumnX + addrColumnWidth + charWidth;                // +1 space
            int opcodeColumnWidth = maxOpcodeLen * charWidth;
            int instrColumnX = opcodeColumnX + opcodeColumnWidth + charWidth;           // +1 space

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
            if (contentRect.Contains(mousePos))
            {
                int localY = mousePos.Y - (contentRect.Y + Padding);
                if (localY >= 0)
                {
                    int idx = localY / lineHeight;
                    if (idx >= 0 && idx < visibleCount)
                        hoverIndex = idx;
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

                // --- opcodes column (dark blue, padded to maxOpcodeLen) ---
                string opcodes = line.OpCodes ?? string.Empty;
                if (opcodes.Length > maxOpcodeLen)
                    opcodes = opcodes.Substring(0, maxOpcodeLen);
                else
                    opcodes = opcodes.PadRight(maxOpcodeLen, ' ');

                ServiceGraphics.DrawText(
                    theme.PrimaryFont,
                    opcodes,
                    opcodeColumnX,
                    y,
                    contentRect.Width - Padding * 2,
                    opcodeColor,
                    theme.TextOutline,
                    fontFlags,
                    0xFF
                );

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
