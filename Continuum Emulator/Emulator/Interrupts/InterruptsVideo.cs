using Continuum93.Emulator;
using Microsoft.Xna.Framework;
using System;

namespace Continuum93.Emulator.Interrupts
{
    public static class InterruptsVideo
    {
        public static void ReadVideoResolution(byte regId, Computer computer)
        {
            byte rX = computer.CPU.REGS.GetNextRegister(regId, 1);
            byte rY = computer.CPU.REGS.GetNextRegister(regId, 3);

            computer.CPU.REGS.Set16BitRegister(rX, (ushort)Constants.V_WIDTH);
            computer.CPU.REGS.Set16BitRegister(rY, (ushort)Constants.V_HEIGHT);
        }

        public static void ReadVideoPagesCount(byte regId, Computer computer)
        {
            computer.CPU.REGS.Set8BitRegister(regId, computer.GRAPHICS.VRAM_PAGES);
        }

        public static void SetVideoPagesCount(byte regId, Computer computer)
        {
            byte pcValueIndex = computer.CPU.REGS.GetNextRegister(regId, 1);
            byte value = computer.CPU.REGS.Get8BitRegister(pcValueIndex);
            computer.GRAPHICS.SetVramPages(value);
        }

        public static void ReadVideoAddress(byte regId, Computer computer)
        {
            byte pcValueIndex = computer.CPU.REGS.GetNextRegister(regId, 1);
            byte value = computer.CPU.REGS.Get8BitRegister(pcValueIndex);
            byte adrIndex = computer.CPU.REGS.GetNextRegister(regId, 1);

            uint address = computer.GRAPHICS.GetVideoPageAddress(value);
            computer.CPU.REGS.Set24BitRegister(adrIndex, address);
        }

        public static void ReadVideoPaletteAddress(byte regId, Computer computer)
        {
            byte pcValueIndex = computer.CPU.REGS.GetNextRegister(regId, 1);
            byte value = computer.CPU.REGS.Get8BitRegister(pcValueIndex);
            byte adrIndex = computer.CPU.REGS.GetNextRegister(regId, 1);

            uint address = computer.GRAPHICS.GetVideoPagePaletteAddress(value);
            computer.CPU.REGS.Set24BitRegister(adrIndex, address);
        }

        public static void ClearVideoPage(byte regId, Computer computer)
        {
            byte pcValueIndex = computer.CPU.REGS.GetNextRegister(regId, 1);
            byte pageValue = computer.CPU.REGS.Get8BitRegister(pcValueIndex);
            byte colorValueIndex = computer.CPU.REGS.GetNextRegister(regId, 2);
            byte colorValue = computer.CPU.REGS.Get8BitRegister(colorValueIndex);

            uint addr = computer.GRAPHICS.GetVideoPageAddress(pageValue);
            computer.MEMC.RAM.Fill(colorValue, addr, Constants.V_SIZE);
        }

        public static void DrawFilledRectangle(byte regId, Computer computer)
        {
            byte pcValueIndex = computer.CPU.REGS.GetNextRegister(regId, 1);
            byte pageValue = computer.CPU.REGS.Get8BitRegister(pcValueIndex);

            byte xIndex = computer.CPU.REGS.GetNextRegister(regId, 2);
            short xValue = computer.CPU.REGS.Get16BitRegisterSigned(xIndex);

            byte yIndex = computer.CPU.REGS.GetNextRegister(regId, 4);
            short yValue = computer.CPU.REGS.Get16BitRegisterSigned(yIndex);

            byte wIndex = computer.CPU.REGS.GetNextRegister(regId, 6);
            ushort widthValue = computer.CPU.REGS.Get16BitRegister(wIndex);

            byte hIndex = computer.CPU.REGS.GetNextRegister(regId, 8);
            ushort heightValue = computer.CPU.REGS.Get16BitRegister(hIndex);

            byte colorIndex = computer.CPU.REGS.GetNextRegister(regId, 10);
            byte colorValue = computer.CPU.REGS.Get8BitRegister(colorIndex);

            uint videoAddr = computer.GRAPHICS.GetVideoPageAddress(pageValue);

            for (uint y = 0; y < heightValue; y++)
            {
                if ((yValue + y) < 0)
                    continue;

                uint xOffset = (xValue < 0) ? (uint)Math.Abs(xValue) : 0;
                ushort currentY = (ushort)(yValue + y);
                int spriteRightEdge = xValue + widthValue;

                // Check if the rectangle is partially or fully offscreen on the right
                int visibleWidth = (int)Math.Min(widthValue - xOffset, Constants.V_WIDTH - xValue);

                if (visibleWidth <= 0 || spriteRightEdge <= 0)
                    continue;

                Array.Fill(
                    computer.MEMC.RAM.Data,
                    colorValue,
                    (int)(videoAddr + currentY * Constants.V_WIDTH + xValue + xOffset),
                    visibleWidth
                );
            }
        }

        public static void DrawRectangle(byte regId, Computer computer)
        {
            byte pcValueIndex = computer.CPU.REGS.GetNextRegister(regId, 1);
            byte pageValue = computer.CPU.REGS.Get8BitRegister(pcValueIndex);

            byte xIndex = computer.CPU.REGS.GetNextRegister(regId, 2);
            short xValue = computer.CPU.REGS.Get16BitRegisterSigned(xIndex);

            byte yIndex = computer.CPU.REGS.GetNextRegister(regId, 4);
            short yValue = computer.CPU.REGS.Get16BitRegisterSigned(yIndex);

            byte wIndex = computer.CPU.REGS.GetNextRegister(regId, 6);
            ushort widthValue = computer.CPU.REGS.Get16BitRegister(wIndex);

            byte hIndex = computer.CPU.REGS.GetNextRegister(regId, 8);
            ushort heightValue = computer.CPU.REGS.Get16BitRegister(hIndex);

            byte colorIndex = computer.CPU.REGS.GetNextRegister(regId, 10);
            byte colorValue = computer.CPU.REGS.Get8BitRegister(colorIndex);

            uint videoAddr = computer.GRAPHICS.GetVideoPageAddress(pageValue);

            // Draw top and bottom sides
            for (int i = 0; i < widthValue; i++)
            {
                int x = xValue + i;

                if (x >= 0 && x < Constants.V_WIDTH)
                {
                    // Draw top side
                    if (yValue >= 0 && yValue < Constants.V_HEIGHT)
                    {
                        uint addrTop = (uint)(videoAddr + yValue * Constants.V_WIDTH + x);
                        computer.MEMC.Set8bitToRAM(addrTop, colorValue);
                    }

                    // Draw bottom side
                    if ((yValue + heightValue - 1) >= 0 && (yValue + heightValue - 1) < Constants.V_HEIGHT)
                    {
                        uint addrBottom = (uint)(videoAddr + (yValue + heightValue - 1) * Constants.V_WIDTH + x);
                        computer.MEMC.Set8bitToRAM(addrBottom, colorValue);
                    }
                }
            }

            // Draw left and right sides
            for (int i = 0; i < heightValue; i++)
            {
                int y = yValue + i;

                if (y >= 0 && y < Constants.V_HEIGHT)
                {
                    // Draw left side
                    if (xValue >= 0 && xValue < Constants.V_WIDTH)
                    {
                        uint addrLeft = (uint)(videoAddr + y * Constants.V_WIDTH + xValue);
                        computer.MEMC.Set8bitToRAM(addrLeft, colorValue);
                    }

                    // Draw right side
                    if ((xValue + widthValue - 1) >= 0 && (xValue + widthValue - 1) < Constants.V_WIDTH)
                    {
                        uint addrRight = (uint)(videoAddr + y * Constants.V_WIDTH + (xValue + widthValue - 1));
                        computer.MEMC.Set8bitToRAM(addrRight, colorValue);
                    }
                }
            }
        }

        public static void DrawFilledRoundedRectangle(byte regId, Computer computer)
        {
            // Extract parameters from registers (same as the outline method)
            byte pcValueIndex = computer.CPU.REGS.GetNextRegister(regId, 1);
            byte pageValue = computer.CPU.REGS.Get8BitRegister(pcValueIndex);

            byte xIndex = computer.CPU.REGS.GetNextRegister(regId, 2);
            short xValue = computer.CPU.REGS.Get16BitRegisterSigned(xIndex);

            byte yIndex = computer.CPU.REGS.GetNextRegister(regId, 4);
            short yValue = computer.CPU.REGS.Get16BitRegisterSigned(yIndex);

            byte wIndex = computer.CPU.REGS.GetNextRegister(regId, 6);
            ushort widthValue = computer.CPU.REGS.Get16BitRegister(wIndex);

            byte hIndex = computer.CPU.REGS.GetNextRegister(regId, 8);
            ushort heightValue = computer.CPU.REGS.Get16BitRegister(hIndex);

            byte colorIndex = computer.CPU.REGS.GetNextRegister(regId, 10);
            byte colorValue = computer.CPU.REGS.Get8BitRegister(colorIndex);

            // Radius for the rounded corners
            byte roundnessIndex = computer.CPU.REGS.GetNextRegister(regId, 11);
            byte roundnessValue = computer.CPU.REGS.Get8BitRegister(roundnessIndex);

            // Clamp the roundness so it doesn’t exceed half the width/height
            if (roundnessValue > widthValue / 2)
                roundnessValue = (byte)(widthValue / 2);
            if (roundnessValue > heightValue / 2)
                roundnessValue = (byte)(heightValue / 2);

            uint videoAddr = computer.GRAPHICS.GetVideoPageAddress(pageValue);
            int r = roundnessValue; // radius for convenience

            // Pre-calculate the centers for the four rounded corners (same as outline)
            int centerTLX = xValue + r;
            int centerTLY = yValue + r;
            int centerTRX = xValue + widthValue - r - 1;
            int centerTRY = yValue + r;
            int centerBLX = xValue + r;
            int centerBLY = yValue + heightValue - r - 1;
            int centerBRX = xValue + widthValue - r - 1;
            int centerBRY = yValue + heightValue - r - 1;

            // Loop through each pixel in the bounding rectangle
            for (int y = yValue; y < yValue + heightValue; y++)
            {
                for (int x = xValue; x < xValue + widthValue; x++)
                {
                    bool fillPixel = false;

                    // If the pixel is in the "central" region (not in the left/right rounded areas)
                    // or in the middle vertical band (not in the top/bottom rounded areas), fill it.
                    if ((x >= xValue + r && x < xValue + widthValue - r) ||
                        (y >= yValue + r && y < yValue + heightValue - r))
                    {
                        fillPixel = true;
                    }
                    else if (r > 0) // otherwise we’re in one of the corner regions
                    {
                        // Top-left corner
                        if (x < xValue + r && y < yValue + r)
                        {
                            int dx = centerTLX - x;
                            int dy = centerTLY - y;
                            if (dx * dx + dy * dy <= r * r)
                                fillPixel = true;
                        }
                        // Top-right corner
                        else if (x >= xValue + widthValue - r && y < yValue + r)
                        {
                            int dx = x - centerTRX;
                            int dy = centerTRY - y;
                            if (dx * dx + dy * dy <= r * r)
                                fillPixel = true;
                        }
                        // Bottom-left corner
                        else if (x < xValue + r && y >= yValue + heightValue - r)
                        {
                            int dx = centerTLX - x;  // centerTLX equals (xValue + r) for left corners
                            int dy = y - centerBLY;
                            if (dx * dx + dy * dy <= r * r)
                                fillPixel = true;
                        }
                        // Bottom-right corner
                        else if (x >= xValue + widthValue - r && y >= yValue + heightValue - r)
                        {
                            int dx = x - centerBRX;
                            int dy = y - centerBRY;
                            if (dx * dx + dy * dy <= r * r)
                                fillPixel = true;
                        }
                    }
                    else
                    {
                        // When r == 0, there's no rounding – fill the whole rectangle.
                        fillPixel = true;
                    }

                    // Set the pixel if within valid video boundaries.
                    if (fillPixel && x >= 0 && x < Constants.V_WIDTH && y >= 0 && y < Constants.V_HEIGHT)
                    {
                        uint addr = (uint)(videoAddr + y * Constants.V_WIDTH + x);
                        computer.MEMC.Set8bitToRAM(addr, colorValue);
                    }
                }
            }
        }

        public static void DrawRoundedRectangle(byte regId, Computer computer)
        {
            // Extract parameters from registers
            byte pcValueIndex = computer.CPU.REGS.GetNextRegister(regId, 1);
            byte pageValue = computer.CPU.REGS.Get8BitRegister(pcValueIndex);

            byte xIndex = computer.CPU.REGS.GetNextRegister(regId, 2);
            short xValue = computer.CPU.REGS.Get16BitRegisterSigned(xIndex);

            byte yIndex = computer.CPU.REGS.GetNextRegister(regId, 4);
            short yValue = computer.CPU.REGS.Get16BitRegisterSigned(yIndex);

            byte wIndex = computer.CPU.REGS.GetNextRegister(regId, 6);
            ushort widthValue = computer.CPU.REGS.Get16BitRegister(wIndex);

            byte hIndex = computer.CPU.REGS.GetNextRegister(regId, 8);
            ushort heightValue = computer.CPU.REGS.Get16BitRegister(hIndex);

            byte colorIndex = computer.CPU.REGS.GetNextRegister(regId, 10);
            byte colorValue = computer.CPU.REGS.Get8BitRegister(colorIndex);

            // radius for the rounded corners
            byte roundnessIndex = computer.CPU.REGS.GetNextRegister(regId, 11);
            byte roundnessValue = computer.CPU.REGS.Get8BitRegister(roundnessIndex);

            // Clamp the roundness so it doesn’t exceed half the width/height
            if (roundnessValue > widthValue / 2)
                roundnessValue = (byte)(widthValue / 2);
            if (roundnessValue > heightValue / 2)
                roundnessValue = (byte)(heightValue / 2);

            uint videoAddr = computer.GRAPHICS.GetVideoPageAddress(pageValue);

            // Draw horizontal segments (top and bottom) excluding the rounded corner areas
            for (int i = roundnessValue; i < widthValue - roundnessValue; i++)
            {
                int x = xValue + i;
                // Top side
                if (x >= 0 && x < Constants.V_WIDTH && yValue >= 0 && yValue < Constants.V_HEIGHT)
                {
                    uint addrTop = (uint)(videoAddr + yValue * Constants.V_WIDTH + x);
                    computer.MEMC.Set8bitToRAM(addrTop, colorValue);
                }
                // Bottom side
                int bottomY = yValue + heightValue - 1;
                if (x >= 0 && x < Constants.V_WIDTH && bottomY >= 0 && bottomY < Constants.V_HEIGHT)
                {
                    uint addrBottom = (uint)(videoAddr + bottomY * Constants.V_WIDTH + x);
                    computer.MEMC.Set8bitToRAM(addrBottom, colorValue);
                }
            }

            // Draw vertical segments (left and right) excluding the rounded corner areas
            for (int i = roundnessValue; i < heightValue - roundnessValue; i++)
            {
                int y = yValue + i;
                // Left side
                if (y >= 0 && y < Constants.V_HEIGHT && xValue >= 0 && xValue < Constants.V_WIDTH)
                {
                    uint addrLeft = (uint)(videoAddr + y * Constants.V_WIDTH + xValue);
                    computer.MEMC.Set8bitToRAM(addrLeft, colorValue);
                }
                // Right side
                int rightX = xValue + widthValue - 1;
                if (y >= 0 && y < Constants.V_HEIGHT && rightX >= 0 && rightX < Constants.V_WIDTH)
                {
                    uint addrRight = (uint)(videoAddr + y * Constants.V_WIDTH + rightX);
                    computer.MEMC.Set8bitToRAM(addrRight, colorValue);
                }
            }

            // Draw quarter-circle arcs for the rounded corners using a Bresenham circle algorithm.
            // (xOffset and yOffset iterate over one-eighth of a circle; we mirror the points for each corner.)
            int r = roundnessValue;
            int xOffset = 0;
            int yOffset = r;
            int d = 3 - 2 * r;

            while (xOffset <= yOffset)
            {
                // Top-left corner (center at (xValue + r, yValue + r))
                int centerTLX = xValue + r;
                int centerTLY = yValue + r;
                PlotPixel(computer, videoAddr, centerTLX - xOffset, centerTLY - yOffset, colorValue);
                PlotPixel(computer, videoAddr, centerTLX - yOffset, centerTLY - xOffset, colorValue);

                // Top-right corner (center at (xValue + widthValue - r - 1, yValue + r))
                int centerTRX = xValue + widthValue - r - 1;
                int centerTRY = yValue + r;
                PlotPixel(computer, videoAddr, centerTRX + xOffset, centerTRY - yOffset, colorValue);
                PlotPixel(computer, videoAddr, centerTRX + yOffset, centerTRY - xOffset, colorValue);

                // Bottom-left corner (center at (xValue + r, yValue + heightValue - r - 1))
                int centerBLX = xValue + r;
                int centerBLY = yValue + heightValue - r - 1;
                PlotPixel(computer, videoAddr, centerBLX - yOffset, centerBLY + xOffset, colorValue);
                PlotPixel(computer, videoAddr, centerBLX - xOffset, centerBLY + yOffset, colorValue);

                // Bottom-right corner (center at (xValue + widthValue - r - 1, yValue + heightValue - r - 1))
                int centerBRX = xValue + widthValue - r - 1;
                int centerBRY = yValue + heightValue - r - 1;
                PlotPixel(computer, videoAddr, centerBRX + xOffset, centerBRY + yOffset, colorValue);
                PlotPixel(computer, videoAddr, centerBRX + yOffset, centerBRY + xOffset, colorValue);

                if (d < 0)
                {
                    d += 4 * xOffset + 6;
                }
                else
                {
                    d += 4 * (xOffset - yOffset) + 10;
                    yOffset--;
                }
                xOffset++;
            }
        }

        // Helper method to safely plot a pixel (with bounds checking)
        private static void PlotPixel(Computer computer, uint videoAddr, int x, int y, byte colorValue)
        {
            if (x >= 0 && x < Constants.V_WIDTH && y >= 0 && y < Constants.V_HEIGHT)
            {
                uint addr = (uint)(videoAddr + y * Constants.V_WIDTH + x);
                computer.MEMC.Set8bitToRAM(addr, colorValue);
            }
        }


        public static void DrawSpriteToVideoPage(byte regId, Computer computer)
        {
            byte sourceIndex = computer.CPU.REGS.GetNextRegister(regId, 1);
            uint sourceAddress = computer.CPU.REGS.Get24BitRegister(sourceIndex);

            byte pcValueIndex = computer.CPU.REGS.GetNextRegister(regId, 4);
            byte pageValue = computer.CPU.REGS.Get8BitRegister(pcValueIndex);

            byte xIndex = computer.CPU.REGS.GetNextRegister(regId, 5);
            short xValue = computer.CPU.REGS.Get16BitRegisterSigned(xIndex);

            byte yIndex = computer.CPU.REGS.GetNextRegister(regId, 7);
            short yValue = computer.CPU.REGS.Get16BitRegisterSigned(yIndex);

            byte wIndex = computer.CPU.REGS.GetNextRegister(regId, 9);
            ushort widthVal = computer.CPU.REGS.Get16BitRegister(wIndex);

            byte hIndex = computer.CPU.REGS.GetNextRegister(regId, 11);
            ushort heightVal = computer.CPU.REGS.Get16BitRegister(hIndex);

            uint videoAddr = computer.GRAPHICS.GetVideoPageAddress(pageValue);

            for (uint y = 0; y < heightVal && (yValue + y) < Constants.V_HEIGHT; y++)
            {
                if ((yValue + y) < 0 || (yValue + y) >= Constants.V_HEIGHT)
                    continue;

                uint xOffset = (xValue < 0) ? (uint)Math.Abs(xValue) : 0;
                ushort currentY = (ushort)Math.Abs(yValue + y);

                int spriteRightEdge = xValue + widthVal;

                // Check if the sprite is partially or fully offscreen on the right
                int visibleWidth = (int)Math.Min(widthVal - xOffset, Constants.V_WIDTH - xValue);

                if (visibleWidth <= 0 || spriteRightEdge <= 0)
                    continue;
                if (pageValue != 0)
                {
                    for (uint x = 0; x < visibleWidth; x++)
                    {
                        uint sourceIndexPosition = sourceAddress + widthVal * y + xOffset + x;
                        uint destIndexPosition = (uint)(videoAddr + currentY * Constants.V_WIDTH + xValue + xOffset + x);

                        byte valueToCopy = computer.MEMC.RAM[sourceIndexPosition];
                        if (valueToCopy != 0)
                        {
                            computer.MEMC.RAM[destIndexPosition] = valueToCopy;
                        }
                    }
                }
                else
                {
                    computer.MEMC.RAM.Copy(
                        sourceAddress + widthVal * y + xOffset,          // Source
                        (uint)(videoAddr + currentY * Constants.V_WIDTH + xValue + xOffset),  // Destination
                        (uint)visibleWidth                                 // Length
                    );
                }
            }
        }

        public static void DrawTileMapSprite(byte regId, Computer computer)
        {
            uint tileMapSourceAddress = computer.CPU.REGS.Get24BitRegister(computer.CPU.REGS.GetNextRegister(regId, 1));
            ushort tileMapWidth = computer.CPU.REGS.Get16BitRegister(computer.CPU.REGS.GetNextRegister(regId, 4));
            short spriteX = computer.CPU.REGS.Get16BitRegisterSigned(computer.CPU.REGS.GetNextRegister(regId, 6));
            short spriteY = computer.CPU.REGS.Get16BitRegisterSigned(computer.CPU.REGS.GetNextRegister(regId, 8));

            ushort spriteW = computer.CPU.REGS.Get16BitRegister(computer.CPU.REGS.GetNextRegister(regId, 10));
            ushort spriteH = computer.CPU.REGS.Get16BitRegister(computer.CPU.REGS.GetNextRegister(regId, 12));
            byte videoPage = computer.CPU.REGS.Get8BitRegister(computer.CPU.REGS.GetNextRegister(regId, 14));
            short videoX = computer.CPU.REGS.Get16BitRegisterSigned(computer.CPU.REGS.GetNextRegister(regId, 15));
            short videoY = computer.CPU.REGS.Get16BitRegisterSigned(computer.CPU.REGS.GetNextRegister(regId, 17));
            byte effects = computer.CPU.REGS.Get8BitRegister(computer.CPU.REGS.GetNextRegister(regId, 19));

            // Extract flip bits + new 3-bit rotation index => 0..7
            bool flipH = (effects & 0x01) != 0;
            bool flipV = (effects & 0x02) != 0;

            bool tileX = (effects & 0x04) != 0;
            bool tileY = (effects & 0x08) != 0;

            byte tileH = 1;
            byte tileV = 1;

            if (tileX)
            {
                tileH = computer.CPU.REGS.Get8BitRegister(computer.CPU.REGS.GetNextRegister(regId, 20));
            }
            if (tileY)
            {
                tileV = computer.CPU.REGS.Get8BitRegister(computer.CPU.REGS.GetNextRegister(regId, 21));
            }

            // bits [4..7] for 22.5° increments
            byte rotationIndex = (byte)(effects >> 4);
            double angleDeg = rotationIndex * 22.5;
            double angleRad = angleDeg * (Math.PI / 180.0);

            // Compute bounding box of the rotated sprite
            int rotWidth = (int)Math.Ceiling(
                Math.Abs(spriteW * Math.Cos(angleRad)) + Math.Abs(spriteH * Math.Sin(angleRad))
            );
            int rotHeight = (int)Math.Ceiling(
                Math.Abs(spriteW * Math.Sin(angleRad)) + Math.Abs(spriteH * Math.Cos(angleRad))
            );

            // "Center" of the source sprite
            double cx = (spriteW - 1) / 2.0;
            double cy = (spriteH - 1) / 2.0;

            // The center of the rotated bounding box
            double cxDst = (rotWidth - 1) / 2.0;
            double cyDst = (rotHeight - 1) / 2.0;

            // Where in video memory to write
            uint videoAddr = computer.GRAPHICS.GetVideoPageAddress(videoPage);

            // Loop over each pixel in the bounding box
            // Precompute trig once
            double cosA = Math.Cos(angleRad);
            double sinA = Math.Sin(angleRad);

            // Loop over each tile copy (horizontal x vertical), then each pixel in that tile's rotated bbox
            for (int ty = 0; ty < tileV; ty++)
            {
                int baseY = videoY + ty * rotHeight;

                for (int tx = 0; tx < tileH; tx++)
                {
                    int baseX = videoX + tx * rotWidth;

                    for (int dy = 0; dy < rotHeight; dy++)
                    {
                        int destY = baseY + dy;
                        // clip check
                        if (destY < 0 || destY >= Constants.V_HEIGHT)
                            continue;

                        for (int dx = 0; dx < rotWidth; dx++)
                        {
                            int destX = baseX + dx;
                            // clip check
                            if (destX < 0 || destX >= Constants.V_WIDTH)
                                continue;

                            // Convert (dx, dy) => source sprite coords using inverse rotation
                            double rx = dx - cxDst;  // relative X from tile's bbox center
                            double ry = dy - cyDst;  // relative Y from tile's bbox center

                            // Inverse rotation back into the unrotated sprite space
                            double sxF = cx + (rx * cosA) + (ry * sinA);
                            double syF = cy - (rx * sinA) + (ry * cosA);

                            // Nearest-neighbor
                            int sx = (int)Math.Floor(sxF + 0.5);
                            int sy = (int)Math.Floor(syF + 0.5);

                            // Apply flips in source domain
                            if (flipH) sx = (spriteW - 1) - sx;
                            if (flipV) sy = (spriteH - 1) - sy;

                            // Bounds check in source
                            if (sx < 0 || sx >= spriteW || sy < 0 || sy >= spriteH)
                                continue;

                            // Compute the sourceIndex
                            uint sourceIndex =
                                tileMapSourceAddress
                                + (uint)(tileMapWidth * (spriteY + sy))
                                + (uint)(spriteX + sx);

                            // Compute the destination index
                            uint destIndex =
                                videoAddr
                                + (uint)(destY * Constants.V_WIDTH)
                                + (uint)destX;

                            // Read from source, write if not transparent
                            byte valueToCopy = computer.MEMC.Get8bitFromRAM(sourceIndex);
                            if (valueToCopy != 0)
                            {
                                computer.MEMC.RAM[destIndex] = valueToCopy;
                            }
                        }
                    }
                }
            }

        }

        [Obsolete("Use Draw Text instead.", false)]
        public static void DrawString(byte regId, Computer computer)
        {
            uint fontAdrAddress = computer.CPU.REGS.Get24BitRegister(computer.CPU.REGS.GetNextRegister(regId, 1));
            uint stringAdrAddress = computer.CPU.REGS.Get24BitRegister(computer.CPU.REGS.GetNextRegister(regId, 4));
            short xValue = computer.CPU.REGS.Get16BitRegisterSigned(computer.CPU.REGS.GetNextRegister(regId, 7));
            short yValue = computer.CPU.REGS.Get16BitRegisterSigned(computer.CPU.REGS.GetNextRegister(regId, 9));
            byte colorValue = computer.CPU.REGS.Get8BitRegister(computer.CPU.REGS.GetNextRegister(regId, 11));
            byte videoPage = computer.CPU.REGS.Get8BitRegister(computer.CPU.REGS.GetNextRegister(regId, 12));
            ushort maxWidth = computer.CPU.REGS.Get16BitRegister(computer.CPU.REGS.GetNextRegister(regId, 13));

            uint vStart = computer.GRAPHICS.GetVideoPageAddress(videoPage); // Video page start
            uint vEnd = vStart + Constants.V_WIDTH * Constants.V_HEIGHT;    // Video page end

            string str = computer.MEMC.GetStringAt(stringAdrAddress);

            byte h = computer.MEMC.Get8bitFromRAM(fontAdrAddress + 1);
            short posX = 0;
            int lastDrawnX = 0;
            byte overflowX = 0;

            for (int i = 0; i < str.Length; i++)    // For each character
            {
                byte c = (byte)str[i];

                if (c < 32 || c > 127) // Does not allow to exit the font range
                    continue;

                byte w = computer.MEMC.Get8bitFromRAM(fontAdrAddress + 2 + c - 32);
                uint dAdr = (uint)(fontAdrAddress + 97 + (c - 32) * h);

                for (short y = 0; y < h; y++)
                {
                    byte cBits = computer.MEMC.Get8bitFromRAM((uint)(dAdr + y));

                    for (short x = 0; x < w; x++)
                    {
                        if (((cBits << x) & 0b10000000) > 0)
                        {
                            /*if (yValue + y < 0 || yValue + y >= Constants.V_HEIGHT ||
                                xValue + posX + x < 0 || xValue + posX + x >= Constants.V_WIDTH)
                            {
                                continue;
                            }*/

                            short cX = (short)(xValue + posX + x);
                            short cY = (short)(yValue + y);

                            if (maxWidth != 0 && posX > maxWidth)
                            {
                                overflowX = 1;
                                continue;
                            }

                            if (
                                (cY < 0) || (cY >= Constants.V_HEIGHT) ||
                                (cX < 0) || (cX >= Constants.V_WIDTH)
                            )
                            {
                                continue;
                            }

                            uint tAddress = (uint)(vStart + cY * Constants.V_WIDTH + cX);
                            if (tAddress >= vStart && tAddress <= vEnd)
                            {
                                computer.MEMC.Set8bitToRAM(tAddress, colorValue);
                                lastDrawnX = posX + w + 1 + x;  // TODO use this instead after fixing the space problem
                            }
                        }
                    }
                }

                posX += (short)(w + 1);
            }
            byte regOverX = computer.CPU.REGS.GetNextRegister(regId, 2);
            computer.CPU.REGS.Set16BitRegister(regId, (ushort)posX);
            computer.CPU.REGS.Set8BitRegister(regOverX, overflowX);

        }

        public static void Plot(byte regId, Computer computer)
        {
            byte pcValueIndex = computer.CPU.REGS.GetNextRegister(regId, 1);
            byte pageValue = computer.CPU.REGS.Get8BitRegister(pcValueIndex);

            byte xIndex = computer.CPU.REGS.GetNextRegister(regId, 2);
            ushort xValue = computer.CPU.REGS.Get16BitRegister(xIndex);

            byte yIndex = computer.CPU.REGS.GetNextRegister(regId, 4);
            ushort yValue = computer.CPU.REGS.Get16BitRegister(yIndex);

            byte colorIndex = computer.CPU.REGS.GetNextRegister(regId, 6);
            byte colorValue = computer.CPU.REGS.Get8BitRegister(colorIndex);

            if (xValue >= Constants.V_WIDTH || yValue >= Constants.V_HEIGHT)
                return; // No plotting outside the video page limits

            uint addr = computer.GRAPHICS.GetVideoPageAddress(pageValue) + yValue * Constants.V_WIDTH + xValue;
            computer.MEMC.Set8bitToRAM(addr, colorValue);
        }

        public static void Line(byte regId, Computer computer)
        {
            byte pcValueIndex = computer.CPU.REGS.GetNextRegister(regId, 1);
            byte pageValue = computer.CPU.REGS.Get8BitRegister(pcValueIndex);

            byte x1Index = computer.CPU.REGS.GetNextRegister(regId, 2);
            short x1Value = computer.CPU.REGS.Get16BitRegisterSigned(x1Index);

            byte y1Index = computer.CPU.REGS.GetNextRegister(regId, 4);
            short y1Value = computer.CPU.REGS.Get16BitRegisterSigned(y1Index);

            byte x2Index = computer.CPU.REGS.GetNextRegister(regId, 6);
            short x2Value = computer.CPU.REGS.Get16BitRegisterSigned(x2Index);

            byte y2Index = computer.CPU.REGS.GetNextRegister(regId, 8);
            short y2Value = computer.CPU.REGS.Get16BitRegisterSigned(y2Index);

            byte colorIndex = computer.CPU.REGS.GetNextRegister(regId, 10);
            byte colorValue = computer.CPU.REGS.Get8BitRegister(colorIndex);

            int dx = Math.Abs(x2Value - x1Value);
            int dy = Math.Abs(y2Value - y1Value);
            short sx = (short)((x1Value < x2Value) ? 1 : -1);
            short sy = (short)((y1Value < y2Value) ? 1 : -1);
            int err = dx - dy;

            while (true)
            {
                // Check if the current point is within the screen boundaries
                if (x1Value >= 0 && x1Value < Constants.V_WIDTH && y1Value >= 0 && y1Value < Constants.V_HEIGHT)
                {
                    uint addr = (uint)(computer.GRAPHICS.GetVideoPageAddress(pageValue) + y1Value * Constants.V_WIDTH + x1Value);
                    computer.MEMC.Set8bitToRAM(addr, colorValue);
                }

                if (x1Value == x2Value && y1Value == y2Value)
                    break;

                int err2 = 2 * err;
                if (err2 > -dy)
                {
                    err -= dy;
                    x1Value += sx;
                }

                if (err2 < dx)
                {
                    err += dx;
                    y1Value += sy;
                }
            }
        }

        public static void Ellipse(byte regId, Computer computer)
        {
            byte pcValueIndex = computer.CPU.REGS.GetNextRegister(regId, 1);
            byte pageValue = computer.CPU.REGS.Get8BitRegister(pcValueIndex);

            byte xIndex = computer.CPU.REGS.GetNextRegister(regId, 2);
            short xValue = computer.CPU.REGS.Get16BitRegisterSigned(xIndex);

            byte yIndex = computer.CPU.REGS.GetNextRegister(regId, 4);
            short yValue = computer.CPU.REGS.Get16BitRegisterSigned(yIndex);

            byte radiusXIndex = computer.CPU.REGS.GetNextRegister(regId, 6);
            short radiusXValue = computer.CPU.REGS.Get16BitRegisterSigned(radiusXIndex);

            byte radiusYIndex = computer.CPU.REGS.GetNextRegister(regId, 8);
            short radiusYValue = computer.CPU.REGS.Get16BitRegisterSigned(radiusYIndex);

            byte colorIndex = computer.CPU.REGS.GetNextRegister(regId, 10);
            byte colorValue = computer.CPU.REGS.Get8BitRegister(colorIndex);

            short rx = Math.Abs(radiusXValue);
            short ry = Math.Abs(radiusYValue);

            short x = 0;
            short y = ry;

            // Region 1
            long rxSq = rx * rx;
            long rySq = ry * ry;
            long twoRxSq = 2 * rxSq;
            long twoRySq = 2 * rySq;
            long p;

            PlotEllipsePoints(pageValue, xValue, yValue, x, y, colorValue, computer);

            p = (long)Math.Round(rySq - rxSq * ry + 0.25 * rxSq);
            while (twoRySq * x < twoRxSq * y)
            {
                x++;
                if (p < 0)
                {
                    p += twoRySq * x + rySq;
                }
                else
                {
                    y--;
                    p += twoRySq * x - twoRxSq * y + rySq;
                }
                PlotEllipsePoints(pageValue, xValue, yValue, x, y, colorValue, computer);
            }

            // Region 2
            p = (long)Math.Round(rySq * (x + 0.5) * (x + 0.5) + rxSq * (y - 1) * (y - 1) - rxSq * rySq);
            while (y > 0)
            {
                y--;
                if (p > 0)
                {
                    p += rxSq - twoRxSq * y;
                }
                else
                {
                    x++;
                    p += twoRySq * x - twoRxSq * y + rxSq;
                }
                PlotEllipsePoints(pageValue, xValue, yValue, x, y, colorValue, computer);
            }
        }

        private static void PlotEllipsePoints(byte pageValue, short xCenter, short yCenter, short x, short y, byte colorValue, Computer computer)
        {
            // Plot points for the ellipse by symmetry
            UpdatePixel(pageValue, xCenter, yCenter, x, y, colorValue, computer);
            UpdatePixel(pageValue, xCenter, yCenter, (short)-x, y, colorValue, computer);
            UpdatePixel(pageValue, xCenter, yCenter, x, (short)-y, colorValue, computer);
            UpdatePixel(pageValue, xCenter, yCenter, (short)-x, (short)-y, colorValue, computer);
        }

        private static void UpdatePixel(byte pageValue, short xCenter, short yCenter, short x, short y, byte colorValue, Computer computer)
        {
            // Update pixel values directly without using Plot
            if (xCenter + x >= 0 && xCenter + x < Constants.V_WIDTH && yCenter + y >= 0 && yCenter + y < Constants.V_HEIGHT)
            {
                uint addr = (uint)(computer.GRAPHICS.GetVideoPageAddress(pageValue) + (yCenter + y) * Constants.V_WIDTH + (xCenter + x));
                computer.MEMC.Set8bitToRAM(addr, colorValue);
            }

            if (xCenter - x >= 0 && xCenter - x < Constants.V_WIDTH && yCenter + y >= 0 && yCenter + y < Constants.V_HEIGHT)
            {
                uint addr = (uint)(computer.GRAPHICS.GetVideoPageAddress(pageValue) + (yCenter + y) * Constants.V_WIDTH + (xCenter - x));
                computer.MEMC.Set8bitToRAM(addr, colorValue);
            }

            if (xCenter + x >= 0 && xCenter + x < Constants.V_WIDTH && yCenter - y >= 0 && yCenter - y < Constants.V_HEIGHT)
            {
                uint addr = (uint)(computer.GRAPHICS.GetVideoPageAddress(pageValue) + (yCenter - y) * Constants.V_WIDTH + (xCenter + x));
                computer.MEMC.Set8bitToRAM(addr, colorValue);
            }

            if (xCenter - x >= 0 && xCenter - x < Constants.V_WIDTH && yCenter - y >= 0 && yCenter - y < Constants.V_HEIGHT)
            {
                uint addr = (uint)(computer.GRAPHICS.GetVideoPageAddress(pageValue) + (yCenter - y) * Constants.V_WIDTH + (xCenter - x));
                computer.MEMC.Set8bitToRAM(addr, colorValue);
            }
        }

        public static void DrawFilledEllipse(byte regId, Computer computer)
        {
            byte pcValueIndex = computer.CPU.REGS.GetNextRegister(regId, 1);
            byte pageValue = computer.CPU.REGS.Get8BitRegister(pcValueIndex);

            byte xIndex = computer.CPU.REGS.GetNextRegister(regId, 2);
            short xValue = computer.CPU.REGS.Get16BitRegisterSigned(xIndex);

            byte yIndex = computer.CPU.REGS.GetNextRegister(regId, 4);
            short yValue = computer.CPU.REGS.Get16BitRegisterSigned(yIndex);

            byte radiusXIndex = computer.CPU.REGS.GetNextRegister(regId, 6);
            short radiusXValue = computer.CPU.REGS.Get16BitRegisterSigned(radiusXIndex);

            byte radiusYIndex = computer.CPU.REGS.GetNextRegister(regId, 8);
            short radiusYValue = computer.CPU.REGS.Get16BitRegisterSigned(radiusYIndex);

            byte colorIndex = computer.CPU.REGS.GetNextRegister(regId, 10);
            byte colorValue = computer.CPU.REGS.Get8BitRegister(colorIndex);

            short rx = Math.Abs(radiusXValue);
            short ry = Math.Abs(radiusYValue);

            uint videoAddr = computer.GRAPHICS.GetVideoPageAddress(pageValue);

            for (short y = (short)-ry; y <= ry; y++)
            {
                for (short x = (short)-rx; x <= rx; x++)
                {
                    if (x * x * ry * ry + y * y * rx * rx <= rx * rx * ry * ry)
                    {
                        int drawX = xValue + x;
                        int drawY = yValue + y;

                        if (drawX >= 0 && drawX < Constants.V_WIDTH && drawY >= 0 && drawY < Constants.V_HEIGHT)
                        {
                            uint addr = (uint)(videoAddr + drawY * Constants.V_WIDTH + drawX);
                            computer.MEMC.Set8bitToRAM(addr, colorValue);
                        }
                    }
                }
            }
        }

        public static void LinePath(byte regId, Computer computer)
        {
            byte valuesPointerIndex = computer.CPU.REGS.GetNextRegister(regId, 1);
            uint valuesPointer = computer.CPU.REGS.Get24BitRegister(valuesPointerIndex);

            byte pcValueIndex = computer.CPU.REGS.GetNextRegister(regId, 4);
            byte pageValue = computer.CPU.REGS.Get8BitRegister(pcValueIndex);

            byte colorIndex = computer.CPU.REGS.GetNextRegister(regId, 5);
            byte colorValue = computer.CPU.REGS.Get8BitRegister(colorIndex);

            Point NULL_POINT = new(-1, -1);

            Point firstPoint = NULL_POINT;
            Point lastPoint = NULL_POINT;
            uint index = 0;

            while (true)
            {
                int x = computer.MEMC.Get16bitFromRAM(valuesPointer + index);
                int y = computer.MEMC.Get16bitFromRAM(valuesPointer + index + 2);
                Point currentPoint = new(x, y);

                // If first point hasn't been set, set it and skip to next point
                if (firstPoint == NULL_POINT)
                {
                    firstPoint = currentPoint;
                    lastPoint = currentPoint;
                    index += 4;
                    continue;
                }

                // If current point is the same as the last point, end the loop
                if (currentPoint == lastPoint)
                {
                    break;
                }

                // Draw line from last point to current point
                DrawLine(lastPoint, currentPoint, colorValue, pageValue, computer);

                lastPoint = currentPoint;
                index += 4;
            }
        }

        private static void DrawLine(Point start, Point end, byte color, byte pageValue, Computer computer)
        {
            int x1 = start.X;
            int y1 = start.Y;
            int x2 = end.X;
            int y2 = end.Y;

            int dx = Math.Abs(x2 - x1);
            int dy = Math.Abs(y2 - y1);
            short sx = (short)((x1 < x2) ? 1 : -1);
            short sy = (short)((y1 < y2) ? 1 : -1);
            int err = dx - dy;

            while (true)
            {
                // Check if the current point is within the screen boundaries
                if (x1 >= 0 && x1 < Constants.V_WIDTH && y1 >= 0 && y1 < Constants.V_HEIGHT)
                {
                    uint addr = (uint)(computer.GRAPHICS.GetVideoPageAddress(pageValue) + y1 * Constants.V_WIDTH + x1);
                    computer.MEMC.Set8bitToRAM(addr, color);
                }

                if (x1 == x2 && y1 == y2)
                    break;

                int err2 = 2 * err;
                if (err2 > -dy)
                {
                    err -= dy;
                    x1 += sx;
                }

                if (err2 < dx)
                {
                    err += dx;
                    y1 += sy;
                }
            }
        }

        public static void ReadLayerVisibility(byte regId, Computer computer)
        {
            computer.CPU.REGS.Set8BitRegister(regId, computer.GRAPHICS.LAYER_VISIBLE_BITS);
        }

        public static void SetLayersVisibility(byte regId, Computer computer)
        {
            byte pcValueIndex = computer.CPU.REGS.GetNextRegister(regId, 1);
            byte visibilityValue = computer.CPU.REGS.Get8BitRegister(pcValueIndex);

            computer.GRAPHICS.SetLayerVisibility(visibilityValue);
        }

        public static void ReadBufferControlMode(byte regId, Computer computer)
        {
            computer.CPU.REGS.Set8BitRegister(regId, computer.GRAPHICS.LAYER_BUFFER_MODE_BITS);
        }

        public static void SetBufferControlMode(byte regId, Computer computer)
        {
            byte pcValueIndex = computer.CPU.REGS.GetNextRegister(regId, 1);
            byte controlMode = computer.CPU.REGS.Get8BitRegister(pcValueIndex);

            computer.GRAPHICS.SetLayerBufferControlMode(controlMode);
        }
    }
}
