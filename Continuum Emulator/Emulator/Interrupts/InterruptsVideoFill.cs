using Continuum93.Emulator;
using System.Collections.Generic;

namespace Continuum93.Emulator.Interrupts
{
    public static class InterruptsVideoFill
    {
        public static void FillAreaOrPolygon(byte regId, Computer computer)
        {
            byte videoPage = computer.CPU.REGS.Get8BitRegister(computer.CPU.REGS.GetNextRegister(regId, 1));
            ushort x = computer.CPU.REGS.Get16BitRegister(computer.CPU.REGS.GetNextRegister(regId, 2));
            ushort y = computer.CPU.REGS.Get16BitRegister(computer.CPU.REGS.GetNextRegister(regId, 4));
            byte fillColor = computer.CPU.REGS.Get8BitRegister(computer.CPU.REGS.GetNextRegister(regId, 6));
            byte borderColor = computer.CPU.REGS.Get8BitRegister(computer.CPU.REGS.GetNextRegister(regId, 7));
            byte flags = computer.CPU.REGS.Get8BitRegister(computer.CPU.REGS.GetNextRegister(regId, 8));

            bool isBorderFill = (flags & 0b00000010) != 0; // Check if bit 1 is set (bordered fill)

            // Get the base video address for the specified video page
            uint videoAddress = computer.GRAPHICS.GetVideoPageAddress(videoPage);

            // Calculate the address of the pixel at (x, y) for direct memory access
            uint startAddress = videoAddress + y * Constants.V_WIDTH + x;

            // Get the color at the starting point (x, y)
            byte targetColor = computer.MEMC.Get8bitFromRAM(startAddress);

            // If the fill color is the same as the target color, nothing to do
            if (targetColor == fillColor)
            {
                return;
            }

            // Depending on the mode (bordered or not), choose the right fill algorithm
            if (isBorderFill)
            {
                // Use border fill: fill inside the polygon bounded by borderColor
                IterativeBoundaryFill(x, y, fillColor, borderColor, videoAddress, computer);
            }
            else
            {
                // Use flood fill: fill connected area of the same color
                IterativeFloodFill(x, y, targetColor, fillColor, videoAddress, computer);
            }
        }

        // Iterative Flood Fill using a stack to avoid recursion
        private static void IterativeFloodFill(ushort x, ushort y, byte targetColor, byte fillColor, uint videoAddress, Computer computer)
        {
            Stack<(ushort x, ushort y)> pixelStack = new Stack<(ushort x, ushort y)>();
            pixelStack.Push((x, y));

            while (pixelStack.Count > 0)
            {
                var (px, py) = pixelStack.Pop();

                if (px < 0 || py < 0 || px >= Constants.V_WIDTH || py >= Constants.V_HEIGHT)
                    continue;

                uint address = videoAddress + py * Constants.V_WIDTH + px;

                // Check if the pixel has the target color
                if (computer.MEMC.Get8bitFromRAM(address) != targetColor)
                    continue;

                // Replace the color
                computer.MEMC.Set8bitToRAM(address, fillColor);

                // Push neighboring pixels to the stack
                pixelStack.Push(((ushort)(px + 1), py));
                pixelStack.Push(((ushort)(px - 1), py));
                pixelStack.Push((px, (ushort)(py + 1)));
                pixelStack.Push((px, (ushort)(py - 1)));
            }
        }

        // Iterative Boundary Fill using a stack to avoid recursion
        private static void IterativeBoundaryFill(ushort x, ushort y, byte fillColor, byte borderColor, uint videoAddress, Computer computer)
        {
            Stack<(ushort x, ushort y)> pixelStack = new Stack<(ushort x, ushort y)>();
            pixelStack.Push((x, y));

            while (pixelStack.Count > 0)
            {
                var (px, py) = pixelStack.Pop();

                if (px < 0 || py < 0 || px >= Constants.V_WIDTH || py >= Constants.V_HEIGHT)
                    continue;

                uint address = videoAddress + py * Constants.V_WIDTH + px;

                // Stop if we hit the border or already filled pixel
                byte currentColor = computer.MEMC.Get8bitFromRAM(address);
                if (currentColor == borderColor || currentColor == fillColor)
                    continue;

                // Replace the color
                computer.MEMC.Set8bitToRAM(address, fillColor);

                // Push neighboring pixels to the stack
                pixelStack.Push(((ushort)(px + 1), py));
                pixelStack.Push(((ushort)(px - 1), py));
                pixelStack.Push((px, (ushort)(py + 1)));
                pixelStack.Push((px, (ushort)(py - 1)));
            }
        }
    }
}
