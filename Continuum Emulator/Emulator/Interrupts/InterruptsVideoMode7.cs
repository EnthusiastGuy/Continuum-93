using Continuum93.Emulator;
using System;

namespace Continuum93.Emulator.Interrupts
{
    public static class InterruptsVideoMode7
    {

        // Experimental Mode 7 implementation
        public static void DrawMode7(byte regId, Computer computer)
        {
            // Extract parameters from registers
            byte pcValueIndex = computer.CPU.REGS.GetNextRegister(regId, 1);
            byte pageValue = computer.CPU.REGS.Get8BitRegister(pcValueIndex);

            byte texAddrIndex = computer.CPU.REGS.GetNextRegister(regId, 2);
            uint textureSource = computer.CPU.REGS.Get24BitRegister(texAddrIndex);

            byte texWidthIndex = computer.CPU.REGS.GetNextRegister(regId, 5);
            ushort textureWidth = computer.CPU.REGS.Get16BitRegister(texWidthIndex);

            byte texHeightIndex = computer.CPU.REGS.GetNextRegister(regId, 7);
            ushort textureHeight = computer.CPU.REGS.Get16BitRegister(texHeightIndex);

            byte destXIndex = computer.CPU.REGS.GetNextRegister(regId, 9);
            short destX = computer.CPU.REGS.Get16BitRegisterSigned(destXIndex);

            byte destYIndex = computer.CPU.REGS.GetNextRegister(regId, 11);
            short destY = computer.CPU.REGS.Get16BitRegisterSigned(destYIndex);

            byte angleIndex = computer.CPU.REGS.GetNextRegister(regId, 13);
            byte rotationAngle = computer.CPU.REGS.Get8BitRegister(angleIndex);

            byte scaleIndex = computer.CPU.REGS.GetNextRegister(regId, 14);
            byte scaleFactor = computer.CPU.REGS.Get8BitRegister(scaleIndex);

            byte cameraHeightIndex = computer.CPU.REGS.GetNextRegister(regId, 15);
            ushort cameraHeight = computer.CPU.REGS.Get16BitRegister(cameraHeightIndex);

            byte horizonIndex = computer.CPU.REGS.GetNextRegister(regId, 17);
            short horizonOffset = computer.CPU.REGS.Get16BitRegisterSigned(horizonIndex);

            byte pitchIndex = computer.CPU.REGS.GetNextRegister(regId, 19);
            byte pitchAngle = computer.CPU.REGS.Get8BitRegister(pitchIndex);

            uint videoAddr = computer.GRAPHICS.GetVideoPageAddress(pageValue);

            // Derived parameters
            uint screenWidth = Constants.V_WIDTH;
            uint screenHeight = Constants.V_HEIGHT;
            // We'll use destX as the horizontal center and destY as the horizon line.
            int screenCenterX = destX;
            int screenCenterY = destY;

            // Convert rotation and pitch from 0-255 to radians.
            double rotation = rotationAngle * (2 * Math.PI / 256.0);
            double pitchRadians = pitchAngle * (2 * Math.PI / 256.0);

            // Scale factor: assume 128 means no zoom.
            double scale = scaleFactor / 128.0;

            // Adjust camera height with pitch (a rough approximation).
            double adjustedCameraHeight = cameraHeight * Math.Cos(pitchRadians);

            // Define a default sky color (for pixels above the horizon)
            byte skyColor = 0; // e.g., black

            // Iterate over the entire screen.
            for (int y = 0; y < screenHeight; y++)
            {
                for (int x = 0; x < screenWidth; x++)
                {
                    uint addr = (uint)(videoAddr + y * screenWidth + x);

                    // If the pixel is above the horizon, use the sky color.
                    if (y < screenCenterY)
                    {
                        computer.MEMC.Set8bitToRAM(addr, skyColor);
                    }
                    else
                    {
                        // Compute the vertical distance from the horizon.
                        int dy = y - screenCenterY;
                        if (dy == 0)
                            dy = 1; // prevent division by zero

                        // Calculate perspective factor (closer to the horizon yields larger values).
                        double perspective = adjustedCameraHeight / dy;

                        // Horizontal offset from the center.
                        double dx = x - screenCenterX;

                        // Apply rotation and scaling.
                        double worldX = dx * Math.Cos(rotation) * scale - perspective * Math.Sin(rotation);
                        double worldY = dx * Math.Sin(rotation) * scale + perspective * Math.Cos(rotation);

                        // Map world coordinates to texture space.
                        int texCenterX = textureWidth / 2;
                        int texCenterY = textureHeight / 2;
                        int sampleX = (int)(worldX + texCenterX) % textureWidth;
                        int sampleY = (int)(worldY + texCenterY) % textureHeight;
                        if (sampleX < 0)
                            sampleX += textureWidth;
                        if (sampleY < 0)
                            sampleY += textureHeight;

                        uint texPixelAddr = textureSource + (uint)(sampleY * textureWidth + sampleX);
                        byte color = computer.MEMC.Get8bitFromRAM(texPixelAddr);

                        computer.MEMC.Set8bitToRAM(addr, color);
                    }
                }
            }
        }
    }
}
