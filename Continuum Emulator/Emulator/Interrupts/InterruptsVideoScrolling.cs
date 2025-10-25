using Continuum93.Emulator;
using System;

namespace Continuum93.Emulator.Interrupts
{
    public static class InterruptsVideoScrolling
    {
        public static void Scroll(byte regId, Computer computer)
        {
            // Read parameters from registers
            byte videoPage = computer.CPU.REGS.Get8BitRegister(computer.CPU.REGS.GetNextRegister(regId, 1));
            ushort x = computer.CPU.REGS.Get16BitRegister(computer.CPU.REGS.GetNextRegister(regId, 2));
            ushort y = computer.CPU.REGS.Get16BitRegister(computer.CPU.REGS.GetNextRegister(regId, 4));
            ushort width = computer.CPU.REGS.Get16BitRegister(computer.CPU.REGS.GetNextRegister(regId, 6));
            ushort height = computer.CPU.REGS.Get16BitRegister(computer.CPU.REGS.GetNextRegister(regId, 8));
            short scrollX = computer.CPU.REGS.Get16BitRegisterSigned(computer.CPU.REGS.GetNextRegister(regId, 10));
            short scrollY = computer.CPU.REGS.Get16BitRegisterSigned(computer.CPU.REGS.GetNextRegister(regId, 12));
            byte flags = computer.CPU.REGS.Get8BitRegister(computer.CPU.REGS.GetNextRegister(regId, 14));

            /*
             * Flags:
             * - bit 0: Horizontal rolling (X-axis)
             * - bit 1: Vertical rolling (Y-axis)
             * - bits 2-7: Not used
             */

            uint videoAddr = computer.GRAPHICS.GetVideoPageAddress(videoPage);

            // Ensure rectangle is within screen bounds
            if (x >= Constants.V_WIDTH || y >= Constants.V_HEIGHT)
                return; // Rectangle is outside the screen

            if (x + width > Constants.V_WIDTH)
                width = (ushort)(Constants.V_WIDTH - x);

            if (y + height > Constants.V_HEIGHT)
                height = (ushort)(Constants.V_HEIGHT - y);

            if (width == 0 || height == 0)
                return; // Rectangle has zero area

            // Read the rectangle into a temporary buffer
            byte[] tempBuffer = new byte[width * height];

            for (int i = 0; i < height; i++)
            {
                int srcY = y + i;
                int srcIndex = (int)(videoAddr + srcY * Constants.V_WIDTH + x);
                Array.Copy(computer.MEMC.RAM.Data, srcIndex, tempBuffer, i * width, width);
            }

            // Prepare the scrolled buffer
            byte[] scrolledBuffer = new byte[width * height];

            // Compute effective scroll amounts
            int effectiveScrollX = scrollX;
            int effectiveScrollY = scrollY;

            bool rollingX = (flags & 0x01) != 0;
            bool rollingY = (flags & 0x02) != 0;

            if (rollingX)
            {
                effectiveScrollX = scrollX % width;
                if (effectiveScrollX < 0)
                    effectiveScrollX += width;
            }

            if (rollingY)
            {
                effectiveScrollY = scrollY % height;
                if (effectiveScrollY < 0)
                    effectiveScrollY += height;
            }

            // Apply scrolling to the buffer
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int destIndex = i * width + j;

                    int srcX;
                    int srcY;

                    // Handle horizontal scrolling
                    if (rollingX)
                    {
                        srcX = (j - effectiveScrollX + width) % width;
                    }
                    else
                    {
                        srcX = j - scrollX;
                        if (srcX < 0 || srcX >= width)
                        {
                            scrolledBuffer[destIndex] = 0; // Fill vacated areas with zero
                            continue;
                        }
                    }

                    // Handle vertical scrolling
                    if (rollingY)
                    {
                        srcY = (i - effectiveScrollY + height) % height;
                    }
                    else
                    {
                        srcY = i - scrollY;
                        if (srcY < 0 || srcY >= height)
                        {
                            scrolledBuffer[destIndex] = 0; // Fill vacated areas with zero
                            continue;
                        }
                    }

                    int srcIndex = srcY * width + srcX;
                    scrolledBuffer[destIndex] = tempBuffer[srcIndex];
                }
            }

            // Write the scrolled buffer back to the video memory
            for (int i = 0; i < height; i++)
            {
                int destY = y + i;
                int destIndex = (int)(videoAddr + destY * Constants.V_WIDTH + x);
                Array.Copy(scrolledBuffer, i * width, computer.MEMC.RAM.Data, destIndex, width);
            }
        }
    }
}
