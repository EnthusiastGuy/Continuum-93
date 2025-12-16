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

            int linesPerPage = Math.Max(1, contentRect.Height / lineHeight);
            int visibleCount = Math.Min(linesPerPage, lines.Count);

            // max opcode length among visible lines
            int maxOpcodeLen = 0;
            for (int i = 0; i < visibleCount; i++)
            {
                var op = lines[i].OpCodes ?? string.Empty;
                if (op.Length > maxOpcodeLen)
                    maxOpcodeLen = op.Length;
            }

            // assume addresses are fixed-width already, but compute max just in case
            int maxAddrLen = 0;
            for (int i = 0; i < visibleCount; i++)
            {
                var addr = lines[i].TextAddress ?? string.Empty;
                if (addr.Length > maxAddrLen)
                    maxAddrLen = addr.Length;
            }

            // column X positions
            int addrColumnX = contentRect.X + Padding;
            int addrColumnWidth = maxAddrLen * charWidth;
            int opcodeColumnX = addrColumnX + addrColumnWidth + charWidth;                // +1 space
            int opcodeColumnWidth = maxOpcodeLen * charWidth;
            int instrColumnX = opcodeColumnX + opcodeColumnWidth + charWidth;           // +1 space

            // Some colors
            // TODO move to themes
            Color addrFull = new(80, 160, 255);          // bright blue
            Color addrZero = new(80, 160, 255, 22);     // transparent blue
            Color addrZeroOutline = new(0, 0, 0, 22);     // transparent blue
            Color opcodeColor = new(40, 80, 160);        // darker blue
            Color instrColor = new(255, 180, 50);        // orange
            Color hoverBorder = new(80, 220, 80);        // green

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
                var line = lines[i];
                int y = contentRect.Y + Padding + i * lineHeight;

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
                        Fonts.ModernDOS_12x18_thin,
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
                        Fonts.ModernDOS_12x18_thin,
                        addrRest,
                        addrX,
                        y,
                        contentRect.Width - Padding * 2,
                        addrFull,
                        Color.Black,
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
                    Fonts.ModernDOS_12x18_thin,
                    opcodes,
                    opcodeColumnX,
                    y,
                    contentRect.Width - Padding * 2,
                    opcodeColor,
                    Color.Black,
                    fontFlags,
                    0xFF
                );

                // --- instruction (orange) ---
                string instr = line.Instruction ?? string.Empty;

                ServiceGraphics.DrawText(
                    Fonts.ModernDOS_12x18_thin,
                    instr,
                    instrColumnX,
                    y,
                    contentRect.Width - Padding * 2,
                    instrColor,
                    Color.Black,
                    fontFlags,
                    0xFF
                );
            }
        }
    }
}
