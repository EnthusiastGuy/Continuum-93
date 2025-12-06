using Continuum93.Emulator;
using Continuum93.Utils;
using Continuum93.Emulator.GraphicsAccelerators;
using System.IO;

namespace Continuum93.Emulator.Interrupts.FileSystem
{
    public static class FileSystemFont
    {
        public static void LoadPNGFont(byte regId, Computer computer)
        {
            byte pathAdrReg = computer.CPU.REGS.GetNextRegister(regId, 1);
            byte destAdrReg = computer.CPU.REGS.GetNextRegister(regId, 4);

            uint pathAddress = computer.CPU.REGS.Get24BitRegister(pathAdrReg);      // Get the pointer to the path string
            uint destinationAddress = computer.CPU.REGS.Get24BitRegister(destAdrReg);       // Get the pointer to the start memory address to load the font to

            string filePath = computer.MEMC.GetStringAt(pathAddress);
            string imagePath = Path.Combine(
                DataConverter.GetCrossPlatformPath(Constants.FS_ROOT),
                DataConverter.GetCrossPlatformPath(filePath)
            );

            string kerningPath = Path.Combine(
                DataConverter.GetCrossPlatformPath(Constants.FS_ROOT),
                DataConverter.GetCrossPlatformPath(Path.ChangeExtension(filePath, ".txt"))
            );

            PNGContainer png = new(imagePath);  // Load and process the PNG
            Kerning kerning = new(kerningPath); // Load any possible kerning pairs

            // TODO fix this, it's off
            computer.CPU.REGS.Set8BitRegister(regId, png.ErrorCode);

            if (png.ErrorCode != 0)             // An error has been found, no point in continuing
                return;

            // Do not allow png files with dimmensions not abing evenly by the 16 by 6 grid
            if (png.Width % 16 + png.Height % 6 > 0)
                return;

            GFXFontExtractor fe = new(png.PixelData, png.Width, png.Height, kerning.GetKerningBytes());
            png.ClearData();

            byte[] fontData = fe.GetRawFontData();

            // get the font data and place it in memory
            computer.LoadMemAt(destinationAddress, fontData);

            // return the size of the font
            computer.CPU.REGS.Set24BitRegister(regId, (uint)fontData.Length);

            fe.ClearData();
        }
    }
}
