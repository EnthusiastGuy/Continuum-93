using Continuum93.Emulator;
using System.Collections.Generic;
using System.Text;
using System;

namespace Continuum93.Emulator.Interrupts
{
    public static class InterruptsVideoDrawText
    {
        public static void DrawText(byte regId, Computer computer)
        {
            // Read parameters from registers
            uint fontAdrAddress = computer.CPU.REGS.Get24BitRegister(computer.CPU.REGS.GetNextRegister(regId, 1));
            uint stringAdrAddress = computer.CPU.REGS.Get24BitRegister(computer.CPU.REGS.GetNextRegister(regId, 4));
            short xValue = computer.CPU.REGS.Get16BitRegisterSigned(computer.CPU.REGS.GetNextRegister(regId, 7));
            short yValue = computer.CPU.REGS.Get16BitRegisterSigned(computer.CPU.REGS.GetNextRegister(regId, 9));
            byte colorValue = computer.CPU.REGS.Get8BitRegister(computer.CPU.REGS.GetNextRegister(regId, 11));
            byte videoPage = computer.CPU.REGS.Get8BitRegister(computer.CPU.REGS.GetNextRegister(regId, 12));
            ushort maxWidth = computer.CPU.REGS.Get16BitRegister(computer.CPU.REGS.GetNextRegister(regId, 13));
            byte flags = computer.CPU.REGS.Get8BitRegister(computer.CPU.REGS.GetNextRegister(regId, 15));
            byte outlineColor = computer.CPU.REGS.Get8BitRegister(computer.CPU.REGS.GetNextRegister(regId, 16));
            byte outlinePattern = computer.CPU.REGS.Get8BitRegister(computer.CPU.REGS.GetNextRegister(regId, 17));

            // Read font metadata
            //byte fontType = computer.MEMC.Get8bitFromRAM(fontAdrAddress);
            byte glyphCellWidth = computer.MEMC.Get8bitFromRAM(fontAdrAddress + 1);
            byte glyphCellHeight = computer.MEMC.Get8bitFromRAM(fontAdrAddress + 2);
            //byte maxGlyphWidth = computer.MEMC.Get8bitFromRAM(fontAdrAddress + 3);
            //byte maxGlyphHeight = computer.MEMC.Get8bitFromRAM(fontAdrAddress + 4);
            byte kerningCount = computer.MEMC.Get8bitFromRAM(fontAdrAddress + 5);

            byte characterSpacing = 1; // Default character spacing

            // Parse flags
            bool monospace = (flags & 0b00000001) != 0;
            bool monospaceCentering = (flags & 0b00000010) != 0;
            bool disableKerning = (flags & 0b00000100) != 0;
            bool center = (flags & 0b00001000) != 0;
            bool wrap = (flags & 0b00010000) != 0;
            bool drawOutline = (flags & 0b00100000) != 0;

            // Read glyph widths
            uint glyphWidthsAddress = fontAdrAddress + 6;
            byte[] glyphWidths = new byte[96];
            for (int i = 0; i < 96; i++)
            {
                glyphWidths[i] = computer.MEMC.Get8bitFromRAM(glyphWidthsAddress + (uint)i);
            }

            // Read kerning pairs
            uint kerningPairsAddress = glyphWidthsAddress + 96;
            Dictionary<(byte, byte), sbyte> kerningPairsDict = new();
            for (int i = 0; i < kerningCount; i++)
            {
                uint kpAddress = kerningPairsAddress + (uint)(i * 3);
                byte firstGlyph = computer.MEMC.Get8bitFromRAM(kpAddress);
                byte secondGlyph = computer.MEMC.Get8bitFromRAM(kpAddress + 1);
                sbyte offset = (sbyte)(computer.MEMC.Get8bitFromRAM(kpAddress + 2));
                kerningPairsDict[(firstGlyph, secondGlyph)] = offset;
            }

            // Calculate bytes per row and total bytes per glyph
            int bytesPerRow = (int)Math.Ceiling(glyphCellWidth / 8.0);
            int totalBytesPerGlyph = bytesPerRow * glyphCellHeight;

            // Calculate glyph data start address
            uint glyphDataAddress = kerningPairsAddress + (uint)(kerningCount * 3);

            // Get the string to render
            string str = computer.MEMC.GetStringAt(stringAdrAddress);

            // Initialize video memory boundaries
            uint vStart = computer.GRAPHICS.GetVideoPageAddress(videoPage);
            uint vEnd = vStart + Constants.V_WIDTH * Constants.V_HEIGHT;

            // Prepare variables for rendering
            int posX = xValue;
            int posY = yValue;
            int currentX;
            int currentY = posY;
            int lineHeight = glyphCellHeight + characterSpacing;
            List<string> lines = new();

            // Handle wrapping and centering
            if (wrap || center)
            {
                lines = WrapText(str, glyphWidths, glyphCellWidth, monospace, monospaceCentering, characterSpacing, maxWidth, kerningPairsDict, disableKerning);
            }
            else
            {
                lines.Add(str);
            }

            // --- Track actual drawn bounds ---
            int maxRenderedLineWidth = 0; // widest actually drawn line (post-wrap, clamped to maxWidth if provided)
            int renderedLines = 0;

            // Render each line
            foreach (string line in lines)
            {
                int lineWidth = CalculateLineWidth(line, glyphWidths, glyphCellWidth, monospace, monospaceCentering, characterSpacing, kerningPairsDict, disableKerning);

                // Clamp to maxWidth if we’re constraining drawing to that box
                int effectiveLineWidth = (maxWidth > 0) ? Math.Min(lineWidth, maxWidth) : lineWidth;
                if (effectiveLineWidth > maxRenderedLineWidth) maxRenderedLineWidth = effectiveLineWidth;

                // Adjust starting X position for centering
                if (center && maxWidth > 0)
                {
                    currentX = posX + (maxWidth - lineWidth) / 2;
                }
                else
                {
                    currentX = posX;
                }

                // Render the line
                RenderLine(line, currentX, currentY, glyphWidths, glyphDataAddress, bytesPerRow, totalBytesPerGlyph, glyphCellWidth, glyphCellHeight, monospace, monospaceCentering, drawOutline, outlineColor, outlinePattern, characterSpacing, colorValue, vStart, vEnd, computer, kerningPairsDict, disableKerning, maxWidth);

                // Move to the next line
                currentY += lineHeight;
                renderedLines++;
            }

            // Compute final bounding box of what we actually drew
            // Height = N * glyphCellHeight + (N-1) * characterSpacing
            int baseHeight = (renderedLines == 0) ? 0 : (renderedLines * glyphCellHeight) + ((renderedLines - 1) * characterSpacing);

            // If outline is drawn outside glyphs, pad 1px on each side
            int outlinePad = drawOutline ? 2 : 0; // +1 left/right and +1 top/bottom in total
            int finalWidth = maxRenderedLineWidth + outlinePad;
            int finalHeight = baseHeight + outlinePad;

            // Clamp to ushort for register write
            static ushort ClampUShort(int v) => (v < 0) ? (ushort)0 : (v > ushort.MaxValue ? ushort.MaxValue : (ushort)v);

            ushort totalTextWidth = ClampUShort(finalWidth);
            ushort totalTextHeight = ClampUShort(finalHeight);

            byte regHeight = computer.CPU.REGS.GetNextRegister(regId, 2);
            computer.CPU.REGS.Set16BitRegister(regId, totalTextWidth);
            computer.CPU.REGS.Set16BitRegister(regHeight, totalTextHeight);
        }

        public static void GetTextRectangle(byte regId, Computer computer)
        {
            // --- Read parameters from registers (same as DrawText) ---
            uint fontAdrAddress = computer.CPU.REGS.Get24BitRegister(computer.CPU.REGS.GetNextRegister(regId, 1));
            uint stringAdrAddress = computer.CPU.REGS.Get24BitRegister(computer.CPU.REGS.GetNextRegister(regId, 4));
            short xValue = computer.CPU.REGS.Get16BitRegisterSigned(computer.CPU.REGS.GetNextRegister(regId, 7));
            short yValue = computer.CPU.REGS.Get16BitRegisterSigned(computer.CPU.REGS.GetNextRegister(regId, 9));
            byte colorValue = computer.CPU.REGS.Get8BitRegister(computer.CPU.REGS.GetNextRegister(regId, 11));
            byte videoPage = computer.CPU.REGS.Get8BitRegister(computer.CPU.REGS.GetNextRegister(regId, 12));
            ushort maxWidth = computer.CPU.REGS.Get16BitRegister(computer.CPU.REGS.GetNextRegister(regId, 13));
            byte flags = computer.CPU.REGS.Get8BitRegister(computer.CPU.REGS.GetNextRegister(regId, 15));
            byte outlineColor = computer.CPU.REGS.Get8BitRegister(computer.CPU.REGS.GetNextRegister(regId, 16));
            byte outlinePattern = computer.CPU.REGS.Get8BitRegister(computer.CPU.REGS.GetNextRegister(regId, 17));

            // --- Font metadata ---
            byte glyphCellWidth = computer.MEMC.Get8bitFromRAM(fontAdrAddress + 1);
            byte glyphCellHeight = computer.MEMC.Get8bitFromRAM(fontAdrAddress + 2);
            byte kerningCount = computer.MEMC.Get8bitFromRAM(fontAdrAddress + 5);

            // --- Flags (same bits as DrawText) ---
            bool monospace = (flags & 0b00000001) != 0;
            bool monospaceCentering = (flags & 0b00000010) != 0;
            bool disableKerning = (flags & 0b00000100) != 0;
            bool center = (flags & 0b00001000) != 0;
            bool wrap = (flags & 0b00010000) != 0;
            bool drawOutline = (flags & 0b00100000) != 0;

            // --- Inputs for measuring ---
            byte characterSpacing = 1; // keep in sync with DrawText
            string str = computer.MEMC.GetStringAt(stringAdrAddress);

            // --- Glyph widths table (96 entries) ---
            uint glyphWidthsAddress = fontAdrAddress + 6;
            byte[] glyphWidths = new byte[96];
            for (int i = 0; i < 96; i++)
                glyphWidths[i] = computer.MEMC.Get8bitFromRAM(glyphWidthsAddress + (uint)i);

            // --- Kerning pairs ---
            uint kerningPairsAddress = glyphWidthsAddress + 96;
            Dictionary<(byte, byte), sbyte> kerningPairsDict = new();
            for (int i = 0; i < kerningCount; i++)
            {
                uint kpAddress = kerningPairsAddress + (uint)(i * 3);
                byte firstGlyph = computer.MEMC.Get8bitFromRAM(kpAddress);
                byte secondGlyph = computer.MEMC.Get8bitFromRAM(kpAddress + 1);
                sbyte offset = (sbyte)computer.MEMC.Get8bitFromRAM(kpAddress + 2);
                kerningPairsDict[(firstGlyph, secondGlyph)] = offset;
            }

            // --- Build the lines exactly as DrawText would (wrap if wrap || center) ---
            List<string> lines = new();
            if (wrap || center)
                lines = WrapText(str, glyphWidths, glyphCellWidth, monospace, monospaceCentering, characterSpacing, maxWidth, kerningPairsDict, disableKerning);
            else
                lines.Add(str);

            // --- Measure widths across lines, respecting maxWidth clipping like RenderLine does ---
            int maxRenderedLineWidth = 0;
            foreach (string line in lines)
            {
                int lineWidth = CalculateLineWidth(
                    line, glyphWidths, glyphCellWidth,
                    monospace, monospaceCentering, characterSpacing,
                    kerningPairsDict, disableKerning);

                int effective = (maxWidth > 0) ? Math.Min(lineWidth, maxWidth) : lineWidth;
                if (effective > maxRenderedLineWidth) maxRenderedLineWidth = effective;
            }

            // --- Measure height: N * glyphCellHeight + (N-1) * characterSpacing ---
            int renderedLines = lines.Count;
            int baseHeight = (renderedLines == 0)
                ? 0
                : (renderedLines * glyphCellHeight) + ((renderedLines - 1) * characterSpacing);

            // --- Account for outline growing 1px outward on each side (same as DrawText) ---
            int outlinePad = drawOutline ? 2 : 0; // total extra pixels width/height
            int finalWidth = maxRenderedLineWidth + outlinePad;
            int finalHeight = baseHeight + outlinePad;

            // --- Clamp to ushort and write back to the same result registers ---
            static ushort ClampUShort(int v) => (v < 0) ? (ushort)0 : (v > ushort.MaxValue ? ushort.MaxValue : (ushort)v);

            ushort totalTextWidth = ClampUShort(finalWidth);
            ushort totalTextHeight = ClampUShort(finalHeight);

            byte regHeight = computer.CPU.REGS.GetNextRegister(regId, 2);
            computer.CPU.REGS.Set16BitRegister(regId, totalTextWidth);
            computer.CPU.REGS.Set16BitRegister(regHeight, totalTextHeight);

            // NOTE: No drawing, no framebuffer access, no RenderLine calls.
        }

        // Helper method to wrap text into lines
        private static List<string> WrapText(string text, byte[] glyphWidths, byte glyphCellWidth, bool monospace, bool monospaceCentering, byte characterSpacing, ushort maxWidth, Dictionary<(byte, byte), sbyte> kerningPairsDict, bool disableKerning)
        {
            List<string> lines = new();
            string[] words = text.Split(' ');
            StringBuilder currentLine = new();
            int currentLineWidth = 0;
            int spaceWidth = monospace ? glyphCellWidth : glyphWidths[0] + characterSpacing; // Width of space character

            foreach (string word in words)
            {
                int wordWidth = CalculateLineWidth(word, glyphWidths, glyphCellWidth, monospace, monospaceCentering, characterSpacing, kerningPairsDict, disableKerning);

                if (currentLineWidth + wordWidth > maxWidth && currentLine.Length > 0)
                {
                    // Start a new line
                    lines.Add(currentLine.ToString().TrimEnd());
                    currentLine.Clear();
                    currentLineWidth = 0;
                }

                currentLine.Append(word + " ");
                currentLineWidth += wordWidth + spaceWidth;
            }

            if (currentLine.Length > 0)
            {
                lines.Add(currentLine.ToString().TrimEnd());
            }

            return lines;
        }

        // Helper method to calculate the width of a line
        private static int CalculateLineWidth(string line, byte[] glyphWidths, byte glyphCellWidth, bool monospace, bool monospaceCentering, byte characterSpacing, Dictionary<(byte, byte), sbyte> kerningPairsDict, bool disableKerning)
        {
            int lineWidth = 0;
            byte lastAscii = 0;

            foreach (char c in line)
            {
                byte asciiCode = (byte)c;

                if (asciiCode < 32 || asciiCode > 127)
                    continue;

                int glyphWidth = monospace ? glyphCellWidth : glyphWidths[asciiCode - 32];
                int kerningOffset = 0;

                if (!disableKerning && !monospace && lastAscii != 0)
                {
                    if (kerningPairsDict.TryGetValue((lastAscii, asciiCode), out sbyte offset))
                    {
                        kerningOffset = offset;
                    }
                }

                lineWidth += glyphWidth + characterSpacing + kerningOffset;
                lastAscii = asciiCode;
            }

            return lineWidth;
        }

        // Helper method to render a line of text
        private static void RenderLine(
            string line,
            int startX, int startY,
            byte[] glyphWidths,
            uint glyphDataAddress,
            int bytesPerRow,
            int totalBytesPerGlyph,
            byte glyphCellWidth, byte glyphCellHeight,
            bool monospace, bool monospaceCentering,
            bool drawOutline, byte outlineColor, byte outlinePattern,
            byte characterSpacing, byte colorValue,
            uint vStart, uint vEnd,
            Computer computer,
            Dictionary<(byte, byte), sbyte> kerningPairsDict, bool disableKerning,
            ushort maxWidth
        ) {
            int posX = startX;
            byte lastAscii = 0;

            foreach (char c in line)
            {
                byte asciiCode = (byte)c;

                if (asciiCode == 32) // Space character
                {
                    posX += monospace ? glyphCellWidth + characterSpacing : glyphWidths[0] + characterSpacing;
                    lastAscii = asciiCode;
                    continue;
                }

                if (asciiCode < 32 || asciiCode > 127)
                    continue;

                int glyphIndex = asciiCode - 32 - 1; // Adjust for space and 0-based index
                if (glyphIndex < 0)
                {
                    lastAscii = asciiCode;
                    continue; // Skip space glyph as it's not stored
                }

                // Apply kerning
                int kerningOffset = 0;
                if (!disableKerning && !monospace && lastAscii != 0)
                {
                    if (kerningPairsDict.TryGetValue((lastAscii, asciiCode), out sbyte offset))
                    {
                        kerningOffset = offset;
                    }
                }

                int glyphWidth = monospace ? glyphCellWidth : glyphWidths[asciiCode - 32];
                int actualGlyphWidth = glyphWidths[asciiCode - 32];
                int glyphHeight = glyphCellHeight;
                uint glyphDataOffset = glyphDataAddress + (uint)(glyphIndex * totalBytesPerGlyph);

                // Center glyph if monospace and monospaceCentering enabled
                int glyphPosX = posX + kerningOffset;
                if (monospace && monospaceCentering)
                {
                    glyphPosX += (glyphCellWidth - actualGlyphWidth) / 2;
                }

                // Check for maxWidth clipping
                if (!monospace && maxWidth > 0 && (glyphPosX - startX) + glyphWidth > maxWidth)
                {
                    break; // Stop rendering further characters
                }

                // If enabled, draw an outline to the characters
                if (drawOutline)
                {
                    // Render the glyph outline by drawing a 3x3 box around each set pixel
                    for (int y = 0; y < glyphHeight; y++)
                    {
                        int byteIndex = y * bytesPerRow;
                        for (int x = 0; x < glyphWidth; x++)
                        {
                            int bitIndex = 7 - (x % 8);
                            int dataIndex = byteIndex + (x / 8);

                            byte glyphByte = computer.MEMC.Get8bitFromRAM(glyphDataOffset + (uint)dataIndex);

                            bool isPixelSet = (glyphByte & (1 << bitIndex)) != 0;

                            if (isPixelSet)
                            {
                                int pixelX = glyphPosX + x;
                                int pixelY = startY + y;

                                // Draw a 3x3 outline box centered at (pixelX, pixelY)
                                for (int dy = -1; dy <= 1; dy++)
                                {
                                    for (int dx = -1; dx <= 1; dx++)
                                    {
                                        byte currentBit = GetBitPosition(dy, dx);

                                        if (currentBit == 255)
                                            continue; // Skip the center position

                                        byte mask = (byte)(1 << currentBit);

                                        if ((outlinePattern & mask) == 0)
                                            continue;

                                        int outlineX = pixelX + dx;
                                        int outlineY = pixelY + dy;

                                        // Check if outline pixel is within screen bounds
                                        if (outlineX >= 0 && outlineX < Constants.V_WIDTH &&
                                            outlineY >= 0 && outlineY < Constants.V_HEIGHT)
                                        {
                                            uint outlineAddress = vStart + (uint)(outlineY * Constants.V_WIDTH + outlineX);
                                            computer.MEMC.Set8bitToRAM(outlineAddress, outlineColor);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // Render the glyph
                for (int y = 0; y < glyphHeight; y++)
                {
                    int byteIndex = y * bytesPerRow;
                    for (int x = 0; x < glyphWidth; x++)
                    {
                        int bitIndex = 7 - (x % 8);
                        int dataIndex = byteIndex + (x / 8);

                        byte glyphByte = computer.MEMC.Get8bitFromRAM(glyphDataOffset + (uint)dataIndex);

                        bool isPixelSet = (glyphByte & (1 << bitIndex)) != 0;

                        if (isPixelSet)
                        {
                            int pixelX = glyphPosX + x;
                            int pixelY = startY + y;

                            // Check if pixel is within screen bounds
                            if (pixelX >= 0 && pixelX < Constants.V_WIDTH && pixelY >= 0 && pixelY < Constants.V_HEIGHT)
                            {
                                uint pixelAddress = vStart + (uint)(pixelY * Constants.V_WIDTH + pixelX);
                                computer.MEMC.Set8bitToRAM(pixelAddress, colorValue);
                            }
                        }
                    }
                }

                // Update position for next character
                posX += glyphWidth + characterSpacing + kerningOffset;
                lastAscii = asciiCode;
            }
        }

        /// <summary>
        /// Maps a 2D offset (dx, dy) from the center position in a 3x3 grid to a bit position in a byte,
        /// excluding the center position itself.
        /// </summary>
        /// <param name="dy">
        /// The vertical offset from the center position, ranging from -1 (top) to 1 (bottom).
        /// </param>
        /// <param name="dx">
        /// The horizontal offset from the center position, ranging from -1 (left) to 1 (right).
        /// </param>
        /// <returns>
        /// A byte value representing the bit position (0 to 7) corresponding to the given offset.
        /// Returns 255 if the offset corresponds to the center position (0, 0), indicating exclusion.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This function is used to determine which surrounding pixels in a 3x3 grid around a central pixel
        /// correspond to specific bits in an outline pattern byte. Each bit position in the outline pattern
        /// represents one of the eight surrounding pixels (excluding the center pixel).
        /// </para>
        /// <para>
        /// The mapping is as follows (with bit positions in parentheses):
        /// <code>
        /// (-1, -1) => Bit 7    (-1,  0) => Bit 6    (-1,  1) => Bit 5
        ///  (0, -1) => Bit 4     (0,  0) => Excluded  (0,  1) => Bit 3
        ///  (1, -1) => Bit 2     (1,  0) => Bit 1     (1,  1) => Bit 0
        /// </code>
        /// </para>
        /// <para>
        /// This mapping allows for flexible control over which neighboring pixels are affected when rendering
        /// effects like outlines or shadows around text glyphs. By setting specific bits in the outline pattern,
        /// you can include or exclude pixels in certain directions relative to the center pixel.
        /// </para>
        /// </remarks>
        static byte GetBitPosition(int dy, int dx)
        {
            byte index = (byte)((dy + 1) * 3 + (dx + 1));

            if (index == 4)
                return 255; // Exclude the center position (0,0)

            byte bitPosition = (byte)(index < 4 ? 7 - index : 8 - index);

            return bitPosition;
        }




        public static void GetTextMetrics(byte regId, Computer computer)
        {
            // Read parameters from registers
            uint fontAdrAddress = computer.CPU.REGS.Get24BitRegister(computer.CPU.REGS.GetNextRegister(regId, 1));
            uint stringAdrAddress = computer.CPU.REGS.Get24BitRegister(computer.CPU.REGS.GetNextRegister(regId, 4));
            short xValue = computer.CPU.REGS.Get16BitRegisterSigned(computer.CPU.REGS.GetNextRegister(regId, 7));
            short yValue = computer.CPU.REGS.Get16BitRegisterSigned(computer.CPU.REGS.GetNextRegister(regId, 9));
            byte colorValue = computer.CPU.REGS.Get8BitRegister(computer.CPU.REGS.GetNextRegister(regId, 11));
            byte videoPage = computer.CPU.REGS.Get8BitRegister(computer.CPU.REGS.GetNextRegister(regId, 12));
            ushort maxWidth = computer.CPU.REGS.Get16BitRegister(computer.CPU.REGS.GetNextRegister(regId, 13));
            byte flags = computer.CPU.REGS.Get8BitRegister(computer.CPU.REGS.GetNextRegister(regId, 15));
            byte outlineColor = computer.CPU.REGS.Get8BitRegister(computer.CPU.REGS.GetNextRegister(regId, 16));

            // Read font metadata
            byte glyphCellWidth = computer.MEMC.Get8bitFromRAM(fontAdrAddress + 1);
            byte glyphCellHeight = computer.MEMC.Get8bitFromRAM(fontAdrAddress + 2);
            byte kerningCount = computer.MEMC.Get8bitFromRAM(fontAdrAddress + 5);

            byte characterSpacing = 1; // Default character spacing

            // Parse flags
            bool monospace = (flags & 0b00000001) != 0;
            bool monospaceCentering = (flags & 0b00000010) != 0;
            bool disableKerning = (flags & 0b00000100) != 0;
            bool center = (flags & 0b00001000) != 0;
            bool wrap = (flags & 0b00010000) != 0;
            bool drawOutline = (flags & 0b00100000) != 0;

            // Read glyph widths
            uint glyphWidthsAddress = fontAdrAddress + 6;
            byte[] glyphWidths = new byte[96];
            for (int i = 0; i < 96; i++)
            {
                glyphWidths[i] = computer.MEMC.Get8bitFromRAM(glyphWidthsAddress + (uint)i);
            }

            // Read kerning pairs
            uint kerningPairsAddress = glyphWidthsAddress + 96;
            Dictionary<(byte, byte), sbyte> kerningPairsDict = new();
            for (int i = 0; i < kerningCount; i++)
            {
                uint kpAddress = kerningPairsAddress + (uint)(i * 3);
                byte firstGlyph = computer.MEMC.Get8bitFromRAM(kpAddress);
                byte secondGlyph = computer.MEMC.Get8bitFromRAM(kpAddress + 1);
                sbyte offset = (sbyte)(computer.MEMC.Get8bitFromRAM(kpAddress + 2));
                kerningPairsDict[(firstGlyph, secondGlyph)] = offset;
            }

            // Get the string to process
            string str = computer.MEMC.GetStringAt(stringAdrAddress);

            // Prepare variables for calculation
            int lineHeight = glyphCellHeight + characterSpacing;
            List<string> lines = new();

            // Handle wrapping and centering
            if (wrap || center)
            {
                lines = WrapText(str, glyphWidths, glyphCellWidth, monospace, monospaceCentering, characterSpacing, maxWidth, kerningPairsDict, disableKerning);
            }
            else
            {
                lines.Add(str);
            }

            int numberOfLines = lines.Count;
            int totalWidth = 0;
            int totalHeight = lineHeight * numberOfLines;

            // Adjust height for outline if needed
            if (drawOutline)
            {
                // Outline adds 2 pixels (1 pixel on top and 1 pixel at the bottom)
                totalHeight += 2 * numberOfLines;
            }

            // Calculate the maximum width among all lines
            foreach (string line in lines)
            {
                int lineWidth = CalculateLineWidth(line, glyphWidths, glyphCellWidth, monospace, monospaceCentering, characterSpacing, kerningPairsDict, disableKerning);

                // Adjust width for outline if needed
                if (drawOutline)
                {
                    // Outline adds 2 pixels (1 pixel on left and 1 pixel on right)
                    lineWidth += 2;
                }

                if (lineWidth > totalWidth)
                {
                    totalWidth = lineWidth;
                }
            }

            computer.CPU.REGS.Set16BitRegister(computer.CPU.REGS.GetNextRegister(regId, 1), (ushort)totalWidth);
            computer.CPU.REGS.Set16BitRegister(computer.CPU.REGS.GetNextRegister(regId, 3), (ushort)totalHeight);
            computer.CPU.REGS.Set16BitRegister(computer.CPU.REGS.GetNextRegister(regId, 5), (ushort)numberOfLines);
        }
    }
}
