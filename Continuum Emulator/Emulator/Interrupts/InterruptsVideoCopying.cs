using Continuum93.Emulator;
using System;
using System.Text.RegularExpressions;

namespace Continuum93.Emulator.Interrupts
{
    public static class InterruptsVideoCopying
    {
        public static void CopyRectangle(byte regId, Computer computer)
        {
            // Read parameters from registers
            byte sourceVideoPage = computer.CPU.REGS.Get8BitRegister(computer.CPU.REGS.GetNextRegister(regId, 1));
            ushort sourceX = computer.CPU.REGS.Get16BitRegister(computer.CPU.REGS.GetNextRegister(regId, 2));
            ushort sourceY = computer.CPU.REGS.Get16BitRegister(computer.CPU.REGS.GetNextRegister(regId, 4));
            ushort sourceWidth = computer.CPU.REGS.Get16BitRegister(computer.CPU.REGS.GetNextRegister(regId, 6));
            ushort sourceHeight = computer.CPU.REGS.Get16BitRegister(computer.CPU.REGS.GetNextRegister(regId, 8));
            byte destVideoPage = computer.CPU.REGS.Get8BitRegister(computer.CPU.REGS.GetNextRegister(regId, 10));
            ushort destX = computer.CPU.REGS.Get16BitRegister(computer.CPU.REGS.GetNextRegister(regId, 11));
            ushort destY = computer.CPU.REGS.Get16BitRegister(computer.CPU.REGS.GetNextRegister(regId, 13));
            ushort destWidth = computer.CPU.REGS.Get16BitRegister(computer.CPU.REGS.GetNextRegister(regId, 15));
            ushort destHeight = computer.CPU.REGS.Get16BitRegister(computer.CPU.REGS.GetNextRegister(regId, 17));

            // Get video memory addresses
            uint sourceVideoAddr = computer.GRAPHICS.GetVideoPageAddress(sourceVideoPage);
            uint destVideoAddr = computer.GRAPHICS.GetVideoPageAddress(destVideoPage);

            // If source or destination rectangles have zero area, return
            if (sourceWidth == 0 || sourceHeight == 0 || destWidth == 0 || destHeight == 0)
                return;

            // Read the source rectangle into a temporary buffer
            byte[] sourceBuffer = new byte[sourceWidth * sourceHeight];

            // Adjust source rectangle if it exceeds source video bounds
            ushort adjustedSourceWidth = sourceWidth;
            ushort adjustedSourceHeight = sourceHeight;

            if (sourceX >= Constants.V_WIDTH || sourceY >= Constants.V_HEIGHT)
                return; // Source rectangle is completely outside the screen

            if (sourceX + sourceWidth > Constants.V_WIDTH)
                adjustedSourceWidth = (ushort)(Constants.V_WIDTH - sourceX);

            if (sourceY + sourceHeight > Constants.V_HEIGHT)
                adjustedSourceHeight = (ushort)(Constants.V_HEIGHT - sourceY);

            // Copy the valid portion of the source rectangle
            for (int i = 0; i < adjustedSourceHeight; i++)
            {
                int srcY = sourceY + i;
                int srcIndex = (int)(sourceVideoAddr + srcY * Constants.V_WIDTH + sourceX);
                Array.Copy(computer.MEMC.RAM.Data, srcIndex, sourceBuffer, i * sourceWidth, adjustedSourceWidth);
            }

            // Create the destination buffer
            byte[] destBuffer = new byte[destWidth * destHeight];

            // Calculate scaling factors
            float scaleX = (float)sourceWidth / destWidth;
            float scaleY = (float)sourceHeight / destHeight;

            // Determine the clipping boundaries for the destination
            int startX = 0;
            int startY = 0;
            int endX = destWidth;
            int endY = destHeight;

            if (destX >= Constants.V_WIDTH || destY >= Constants.V_HEIGHT)
                return; // Destination rectangle is completely outside the screen

            // Adjust start and end positions based on screen bounds
            if (destX < 0)
                startX = -destX;
            if (destY < 0)
                startY = -destY;
            if (destX + destWidth > Constants.V_WIDTH)
                endX = (int)Constants.V_WIDTH - destX;
            if (destY + destHeight > Constants.V_HEIGHT)
                endY = (int)Constants.V_HEIGHT - destY;

            // Perform nearest-neighbor scaling and copy to destination buffer
            for (int y = startY; y < endY; y++)
            {
                int destRowIndex = y * destWidth;
                int sourceYIndex = (int)(y * scaleY);
                if (sourceYIndex >= sourceHeight)
                    sourceYIndex = sourceHeight - 1;

                for (int x = startX; x < endX; x++)
                {
                    int sourceXIndex = (int)(x * scaleX);
                    if (sourceXIndex >= sourceWidth)
                        sourceXIndex = sourceWidth - 1;

                    int sourceIndex = sourceYIndex * sourceWidth + sourceXIndex;
                    int destIndex = destRowIndex + x;

                    // Ensure we don't read beyond the source buffer if source is clipped
                    if (sourceXIndex >= adjustedSourceWidth || sourceYIndex >= adjustedSourceHeight)
                        continue;

                    destBuffer[destIndex] = sourceBuffer[sourceYIndex * sourceWidth + sourceXIndex];
                }
            }

            // Write the destination buffer back to the video memory
            for (int y = startY; y < endY; y++)
            {
                int destYPos = destY + y;
                int destMemIndex = (int)(destVideoAddr + destYPos * Constants.V_WIDTH + destX + startX);
                int bufferIndex = y * destWidth + startX;
                int copyWidth = endX - startX;

                // Ensure we don't write beyond the video memory bounds
                if (destMemIndex + copyWidth > computer.MEMC.RAM.Data.Length)
                    copyWidth = computer.MEMC.RAM.Data.Length - destMemIndex;

                Array.Copy(destBuffer, bufferIndex, computer.MEMC.RAM.Data, destMemIndex, copyWidth);
            }
        }

    }
}
