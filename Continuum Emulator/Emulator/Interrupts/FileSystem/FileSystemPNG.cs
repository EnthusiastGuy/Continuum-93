using Continuum93.Emulator;
using Continuum93.Emulator.GraphicsAccelerators;
using System.IO;

namespace Continuum93.Emulator.Interrupts.FileSystem
{

    public static class FileSystemPNG
    {
        /**
         * Emulates an interrupt for loading an 8-bit PNG image into the emulator's RAM. 
         * Enables the dynamic loading of graphical content. It uses a sequence of 24-bit registers
         * to point to:
         * - the address of the string pointing to the PNG file;
         * - the address where the palette should be deposited;
         * - the address where the image data should be deposited;
         *
         * @param regId The base register identifier from which subsequent register addresses 
         *              for path, palette, and image data are calculated.
         * @param computer The instance of the Computer class representing the current state 
         *                 of the emulator, including CPU, memory, and other subsystems.
         */
        public static void Load8BitPNG(byte regId, Computer computer)
        {
            byte pathAdrReg = computer.CPU.REGS.GetNextRegister(regId, 1);
            byte destPalAdrReg = computer.CPU.REGS.GetNextRegister(regId, 4);
            byte destImgAdrReg = computer.CPU.REGS.GetNextRegister(regId, 7);

            uint pathAddress = computer.CPU.REGS.Get24BitRegister(pathAdrReg);      // Get the pointer to the path string
            uint destPalAdr = computer.CPU.REGS.Get24BitRegister(destPalAdrReg);    // Get the pointer to the start memory address to load the PNG palette to
            uint destImgAdr = computer.CPU.REGS.Get24BitRegister(destImgAdrReg);    // Get the pointer to the start memory address to load the PNG image data to

            string filePath = computer.MEMC.GetStringAt(pathAddress);
            string imagePath = Path.Combine(DataConverter.GetCrossPlatformPath(Constants.FS_ROOT), DataConverter.GetCrossPlatformPath(filePath));

            PNGContainer png = new(imagePath);  // Load and process the PNG

            computer.CPU.REGS.Set8BitRegister(regId, png.ErrorCode);

            if (png.ErrorCode != 0)             // An error has been found, no point in continuing
                return;

            computer.LoadMemAt(destPalAdr, png.PaletteData);
            computer.LoadMemAt(destImgAdr, png.PixelData);

            byte widthReg = computer.CPU.REGS.GetNextRegister(regId, 1);
            byte heightReg = computer.CPU.REGS.GetNextRegister(regId, 3);
            byte colorsReg = computer.CPU.REGS.GetNextRegister(regId, 5);

            computer.CPU.REGS.Set16BitRegister(widthReg, (ushort)png.Width);
            computer.CPU.REGS.Set16BitRegister(heightReg, (ushort)png.Height);
            computer.CPU.REGS.Set8BitRegister(colorsReg, png.ColorCount);

            png.ClearData();
        }

        /**
         * Emulates an interrupt for loading an 8-bit PNG image into the emulator's RAM, with the added capability
         * to handle custom transparency. This method extends the basic PNG loading functionality by allowing
         * the specification of an RGBA color that should be considered transparent in the loaded image. The specified
         * transparent color is then moved to the first position in the palette, leveraging the emulator's behavior
         * where only the first palette color is treated as transparent. It utilizes a sequence of 24-bit registers to indicate:
         * - the address of the string pointing to the PNG file;
         * - the address where the palette should be loaded;
         * - the address where the image data should be loaded;
         * - Additionally, it uses a set of four 8-bit registers to specify the RGBA values of the transparency color.
         *
         * @param regId The base register identifier from which subsequent register addresses for path, palette, image data,
         *              and transparency color are calculated.
         * @param computer The instance of the Computer class representing the current state of the emulator, including CPU, 
         *                 memory, and other subsystems.
         */
        public static void Load8BitPNGCustomTransparency(byte regId, Computer computer)
        {
            byte pngPathAdrReg = computer.CPU.REGS.GetNextRegister(regId, 1);
            byte destPalAdrReg = computer.CPU.REGS.GetNextRegister(regId, 4);
            byte destImgAdrReg = computer.CPU.REGS.GetNextRegister(regId, 7);
            byte transColorRegR = computer.CPU.REGS.GetNextRegister(regId, 10);
            byte transColorRegG = computer.CPU.REGS.GetNextRegister(regId, 11);
            byte transColorRegB = computer.CPU.REGS.GetNextRegister(regId, 12);
            byte transColorRegA = computer.CPU.REGS.GetNextRegister(regId, 13);

            uint pathAddress = computer.CPU.REGS.Get24BitRegister(pngPathAdrReg);   // Get the pointer to the path string
            uint destPalAdr = computer.CPU.REGS.Get24BitRegister(destPalAdrReg);    // Get the pointer to the start memory address to load the PNG palette to
            uint destImgAdr = computer.CPU.REGS.Get24BitRegister(destImgAdrReg);    // Get the pointer to the start memory address to load the PNG image data to

            byte colR = computer.CPU.REGS.Get8BitRegister(transColorRegR);
            byte colG = computer.CPU.REGS.Get8BitRegister(transColorRegG);
            byte colB = computer.CPU.REGS.Get8BitRegister(transColorRegB);
            byte colA = computer.CPU.REGS.Get8BitRegister(transColorRegA);

            string filePath = computer.MEMC.GetStringAt(pathAddress);
            string imagePath = Path.Combine(DataConverter.GetCrossPlatformPath(Constants.FS_ROOT), DataConverter.GetCrossPlatformPath(filePath));

            PNGContainer png = new(imagePath);  // Load and process the PNG
            
            computer.CPU.REGS.Set8BitRegister(regId, png.ErrorCode);

            if (png.ErrorCode != 0)             // An error has been found, no point in continuing
                return;

            png.ApplyTransparencyCorrection(colR, colG, colB, colA);

            computer.LoadMemAt(destPalAdr, png.PaletteData);
            computer.LoadMemAt(destImgAdr, png.PixelData);

            byte widthReg = computer.CPU.REGS.GetNextRegister(regId, 1);
            byte heightReg = computer.CPU.REGS.GetNextRegister(regId, 3);
            byte colorsReg = computer.CPU.REGS.GetNextRegister(regId, 5);

            computer.CPU.REGS.Set16BitRegister(widthReg, (ushort)png.Width);
            computer.CPU.REGS.Set16BitRegister(heightReg, (ushort)png.Height);
            computer.CPU.REGS.Set8BitRegister(colorsReg, png.ColorCount);

            png.ClearData();
        }

        /**
         * Emulates an interrupt for integrating an 8-bit PNG image into the emulator's video memory, 
         * specifically focusing on merging the PNG's color palette with an existing palette in memory 
         * and updating the image pixel data accordingly. This method facilitates dynamic image and palette 
         * management within the emulator, allowing for efficient use of palette entries and ensuring 
         * consistent color representation across multiple images.
         * 
         * Parameters (via CPU registers):
         * - Address of the string pointing to the PNG file (24-bit): The memory address where the path 
         *   to the PNG file is stored. This path is used to locate and load the PNG file for processing.
         * - Address where the palette should be deposited (24-bit): The starting memory address where 
         *   the merged palette will be stored. This allows the emulator to directly utilize the new palette 
         *   for rendering.
         * - Number of existing colors in the target palette (8-bit): Indicates how many colors are 
         *   currently in the palette that the new PNG palette will be merged into. This is crucial for 
         *   correctly mapping existing image data to the new palette.
         * - Address where the image data should be deposited (24-bit): The starting memory address where 
         *   the processed PNG image data (with updated palette indices) will be stored, ready for rendering 
         *   by the emulator.
         * 
         * @param regId The base register identifier. This is used as the starting point for calculating 
         *              subsequent register addresses for the path, palette, and image data, enabling 
         *              flexible parameter passing through CPU registers.
         * @param computer The instance of the Computer class. This represents the current state of the 
         *                 emulator, including its CPU, memory, and other subsystems. It provides the 
         *                 necessary context and methods for memory management, file access, and error 
         *                 handling within the emulator environment.
         * 
         * This method performs several key operations:
         * 1. It retrieves the file path from the emulator's memory and loads the specified PNG file.
         * 2. It merges the PNG's palette with the existing palette in memory, ensuring unique colors are 
         *    preserved and correctly referenced.
         * 3. It updates the PNG image data to reflect the indices of the merged palette.
         * 4. It writes the merged palette and updated image data back to the emulator's memory.
         * 5. It updates CPU registers with the dimensions of the loaded image and the count of unique colors 
         *    in the merged palette, facilitating further processing or rendering by the emulator.
         * 
         * The method also sets an error code in a register if an issue occurs during processing, allowing 
         * the calling routine to handle errors appropriately.
         */
        public static void Merge8BitPNG(byte regId, Computer computer)
        {
            byte pathAdrReg = computer.CPU.REGS.GetNextRegister(regId, 1);
            byte destPalAdrReg = computer.CPU.REGS.GetNextRegister(regId, 4);
            byte destPalSizeReg = computer.CPU.REGS.GetNextRegister(regId, 7);
            byte destImgAdrReg = computer.CPU.REGS.GetNextRegister(regId, 8);


            uint pathAddress = computer.CPU.REGS.Get24BitRegister(pathAdrReg);      // Get the pointer to the path string
            uint destPalAdr = computer.CPU.REGS.Get24BitRegister(destPalAdrReg);    // Get the pointer to the start memory address to load the PNG palette to
            byte destPalSize = computer.CPU.REGS.Get8BitRegister(destPalSizeReg);
            uint destImgAdr = computer.CPU.REGS.Get24BitRegister(destImgAdrReg);    // Get the pointer to the start memory address to load the PNG image data to

            string filePath = computer.MEMC.GetStringAt(pathAddress);
            string imagePath = Path.Combine(DataConverter.GetCrossPlatformPath(Constants.FS_ROOT), DataConverter.GetCrossPlatformPath(filePath));

            byte[] existingPalette = computer.GetMemFrom(destPalAdr, destPalSize);
            PNGContainer png = new(imagePath);  // Load and process the PNG
            png.MergePalette(existingPalette);


            computer.CPU.REGS.Set8BitRegister(regId, png.ErrorCode);

            if (png.ErrorCode != 0)             // An error has been found, no point in continuing
                return;

            computer.LoadMemAt(destPalAdr, png.PaletteData);
            computer.LoadMemAt(destImgAdr, png.PixelData);

            byte widthReg = computer.CPU.REGS.GetNextRegister(regId, 1);
            byte heightReg = computer.CPU.REGS.GetNextRegister(regId, 3);
            byte colorsReg = computer.CPU.REGS.GetNextRegister(regId, 5);

            computer.CPU.REGS.Set16BitRegister(widthReg, (ushort)png.Width);
            computer.CPU.REGS.Set16BitRegister(heightReg, (ushort)png.Height);
            computer.CPU.REGS.Set8BitRegister(colorsReg, png.ColorCount);

            png.ClearData();
        }

        /**
         * Emulates an advanced interrupt for integrating an 8-bit PNG image with custom transparency into the emulator's video memory.
         * This method extends the basic palette merging functionality by also allowing the specification of a custom transparency color.
         * The specified color is moved to the first position in the palette, treating it as transparent in the retro computing environment,
         * and the PNG's color palette is merged with an existing palette in memory, updating the image pixel data accordingly.
         * 
         * Parameters (via CPU registers):
         * - Address of the string pointing to the PNG file (24-bit): Specifies the memory address of the PNG file path.
         * - Address for the palette storage (24-bit): Designates the starting memory address for storing the merged palette.
         * - Number of existing colors in the target palette (8-bit): Indicates the initial number of colors in the existing palette.
         * - Address for the image data storage (24-bit): Points to the starting memory address for storing the updated PNG image data.
         * - RGBA values for the custom transparency color (4x8-bit): Specifies the Red, Green, Blue, and Alpha values of the color
         *   that should be treated as transparent within the image.
         * 
         * @param regId The base register identifier used for calculating the addresses of parameters passed via CPU registers.
         * @param computer The Computer class instance, representing the emulator's current state, including CPU, memory, and subsystems.
         * 
         * Operations:
         * 1. Retrieves the PNG file path from emulator memory and loads the PNG file for processing.
         * 2. Merges the PNG's color palette with the existing palette in memory, preserving unique colors and updating references.
         * 3. Applies a transparency correction by moving the specified RGBA color to the first position in the palette, adjusting
         *    the image data to treat this color as transparent in the rendering process.
         * 4. Writes the updated palette and image data back to the emulator's memory, ready for use in rendering or further processing.
         * 5. Updates CPU registers with the image's dimensions and the count of colors in the updated palette to facilitate additional
         *    emulator operations.
         * 
         * Error handling is integrated, with an error code set in a register if processing fails at any stage, allowing for appropriate
         * response handling by the emulator or calling routine.
         */
        public static void Merge8BitPNGCustomTransparency(byte regId, Computer computer)
        {
            byte pathAdrReg = computer.CPU.REGS.GetNextRegister(regId, 1);
            byte destPalAdrReg = computer.CPU.REGS.GetNextRegister(regId, 4);
            byte destPalSizeReg = computer.CPU.REGS.GetNextRegister(regId, 7);
            byte destImgAdrReg = computer.CPU.REGS.GetNextRegister(regId, 8);
            byte transColorRegR = computer.CPU.REGS.GetNextRegister(regId, 11);
            byte transColorRegG = computer.CPU.REGS.GetNextRegister(regId, 12);
            byte transColorRegB = computer.CPU.REGS.GetNextRegister(regId, 13);
            byte transColorRegA = computer.CPU.REGS.GetNextRegister(regId, 14);

            uint pathAddress = computer.CPU.REGS.Get24BitRegister(pathAdrReg);      // Get the pointer to the path string
            uint destPalAdr = computer.CPU.REGS.Get24BitRegister(destPalAdrReg);    // Get the pointer to the start memory address to load the PNG palette to
            byte destPalSize = computer.CPU.REGS.Get8BitRegister(destPalSizeReg);
            uint destImgAdr = computer.CPU.REGS.Get24BitRegister(destImgAdrReg);    // Get the pointer to the start memory address to load the PNG image data to

            byte colR = computer.CPU.REGS.Get8BitRegister(transColorRegR);
            byte colG = computer.CPU.REGS.Get8BitRegister(transColorRegG);
            byte colB = computer.CPU.REGS.Get8BitRegister(transColorRegB);
            byte colA = computer.CPU.REGS.Get8BitRegister(transColorRegA);

            string filePath = computer.MEMC.GetStringAt(pathAddress);
            string imagePath = Path.Combine(DataConverter.GetCrossPlatformPath(Constants.FS_ROOT), DataConverter.GetCrossPlatformPath(filePath));

            byte[] existingPalette = computer.GetMemFrom(destPalAdr, (uint)(destPalSize * 3));
            PNGContainer png = new(imagePath);  // Load and process the PNG

            computer.CPU.REGS.Set8BitRegister(regId, png.ErrorCode);

            if (png.ErrorCode != 0)             // An error has been found, no point in continuing
                return;

            png.MergePalette(existingPalette);
            png.ApplyTransparencyCorrection(colR, colG, colB, colA);

            computer.LoadMemAt(destPalAdr, png.PaletteData);
            computer.LoadMemAt(destImgAdr, png.PixelData);

            byte widthReg = computer.CPU.REGS.GetNextRegister(regId, 1);
            byte heightReg = computer.CPU.REGS.GetNextRegister(regId, 3);
            byte colorsReg = computer.CPU.REGS.GetNextRegister(regId, 5);

            computer.CPU.REGS.Set16BitRegister(widthReg, (ushort)png.Width);
            computer.CPU.REGS.Set16BitRegister(heightReg, (ushort)png.Height);
            computer.CPU.REGS.Set8BitRegister(colorsReg, png.ColorCount);

            png.ClearData();
        }
    }
}
