using Continuum93.Emulator;
using Continuum93.Emulator.IO;
using Continuum93.Emulator.States;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Continuum93.Emulator.Interrupts.FileSystem
{
    public static class FileSystemGeneral
    {
        public static void CheckIfFileExists(byte regId, Computer computer)
        {
            byte adrReg = computer.CPU.REGS.GetNextRegister(regId, 1);
            uint pathAddress = computer.CPU.REGS.Get24BitRegister(adrReg);   // Get the pointer to the path string

            string path = computer.MEMC.GetStringAt(pathAddress);            // Get the string path
            string fPath = Path.Combine(DataConverter.GetCrossPlatformPath(Constants.FS_ROOT), DataConverter.GetCrossPlatformPath(path));
            byte response = File.Exists(fPath) ? Constants.B_TRUE : Constants.B_FALSE;      // Deposit result in the register as a 0xFF value if true or 0 if false

            computer.CPU.REGS.Set8BitRegister(regId, response);
        }

        public static void CheckIfDirectoryExists(byte regId, Computer computer)
        {
            byte adrReg = computer.CPU.REGS.GetNextRegister(regId, 1);
            uint pathAddress = computer.CPU.REGS.Get24BitRegister(adrReg);   // Get the pointer to the path string

            string path = computer.MEMC.GetStringAt(pathAddress);            // Get the string path
            string fPath = Path.Combine(DataConverter.GetCrossPlatformPath(Constants.FS_ROOT), DataConverter.GetCrossPlatformPath(path));
            byte response = Directory.Exists(fPath) ? Constants.B_TRUE : Constants.B_FALSE; // Deposit result in the register as a 0xFF value if true or 0 if false

            computer.CPU.REGS.Set8BitRegister(regId, response);
        }

        private static void ListInDirectory(byte regId, byte mode, Computer computer)
        {
            byte adrReg = computer.CPU.REGS.GetNextRegister(regId, 1);
            uint pathAddress = computer.CPU.REGS.Get24BitRegister(adrReg);   // Get the pointer to the path string
            byte targetReg = computer.CPU.REGS.GetNextRegister(regId, 4);
            uint targetAddress = computer.CPU.REGS.Get24BitRegister(targetReg);

            string path = computer.MEMC.GetStringAt(pathAddress);            // Get the string path
            string fullpath = Path.Combine(DataConverter.GetCrossPlatformPath(Constants.FS_ROOT), DataConverter.GetCrossPlatformPath(path));

            if (!FileManager.DirectoryExists(fullpath))
            {
                computer.CPU.REGS.Set24BitRegister(adrReg, Constants.MAX24BIT);
                return;
            }

            List<string[]> allItems = new();
            string[] files = Directory.GetFiles(fullpath);
            string[] directories = Directory.GetDirectories(fullpath);

            switch (mode)
            {
                case 0:
                    allItems.Add(files);
                    break;
                case 1:
                    allItems.Add(directories);
                    break;
                case 2:
                    allItems.Add(directories);
                    allItems.Add(files);

                    int folderBufferStart = 1;

                    foreach (string dir in directories)
                        folderBufferStart += Path.GetFileName(dir).Length + 1;

                    byte dReg = computer.CPU.REGS.GetNextRegister(adrReg, 3);
                    byte fReg = computer.CPU.REGS.GetNextRegister(dReg, 3);
                    byte fAdrReg = computer.CPU.REGS.GetNextRegister(fReg, 3);

                    computer.CPU.REGS.Set24BitRegister(dReg, (uint)directories.Length);
                    computer.CPU.REGS.Set24BitRegister(fReg, (uint)files.Length);
                    computer.CPU.REGS.Set24BitRegister(fAdrReg, (uint)folderBufferStart);

                    break;
            }

            StringBuilder sb = new();
            uint totalItems = 0;

            foreach (string[] items in allItems)
            {
                foreach (string item in items)
                {
                    sb.Append(Path.GetFileName(item));
                    sb.Append(Constants.TERMINATOR);
                }
                totalItems += (uint)items.Length;

                sb.Append(Constants.TERMINATOR);
            }

            byte[] byteItems = Encoding.ASCII.GetBytes(sb.ToString());
            computer.LoadMemAt(targetAddress, byteItems);
            computer.CPU.REGS.Set24BitRegister(adrReg, totalItems);
        }

        public static void ListFilesInDirectory(byte regId, Computer computer)
        {
            ListInDirectory(regId, 0, computer);
        }

        public static void ListDirectoriesInDirectory(byte regId, Computer computer)
        {
            ListInDirectory(regId, 1, computer);
        }

        public static void ListDirectoriesAndFilesInDirectory(byte regId, Computer computer)
        {
            ListInDirectory(regId, 2, computer);
        }

        public static void SaveFile(byte regId, Computer computer)
        {
            byte pathAdrReg = computer.CPU.REGS.GetNextRegister(regId, 1);
            byte sourceAdrReg = computer.CPU.REGS.GetNextRegister(regId, 4);
            byte sourceLenReg = computer.CPU.REGS.GetNextRegister(regId, 7);
            uint pathAddress = computer.CPU.REGS.Get24BitRegister(pathAdrReg);   // Get the pointer to the path string
            uint sourceAdr = computer.CPU.REGS.Get24BitRegister(sourceAdrReg);   // Get the pointer to the start memory address
            uint sourceLen = computer.CPU.REGS.Get24BitRegister(sourceLenReg);   // Get the length
            string path = computer.MEMC.GetStringAt(pathAddress);                // Get the string path

            using FileStream
            fileStream = new(Path.Combine(Constants.FS_ROOT, path), FileMode.Create);

            for (int i = 0; i < sourceLen; i++)
                fileStream.WriteByte(computer.MEMC.RAM.Data[sourceAdr + i]);
        }

        public static void LoadFile(byte regId, Computer computer)
        {
            byte pathAdrReg = computer.CPU.REGS.GetNextRegister(regId, 1);
            byte destAdrReg = computer.CPU.REGS.GetNextRegister(regId, 4);

            uint pathAddress = computer.CPU.REGS.Get24BitRegister(pathAdrReg);   // Get the pointer to the path string
            uint destAdr = computer.CPU.REGS.Get24BitRegister(destAdrReg);       // Get the pointer to the start memory address to load the file to

            string path = computer.MEMC.GetStringAt(pathAddress);                // Get the string path

            byte[] fileBytes = File.ReadAllBytes(Path.Combine(DataConverter.GetCrossPlatformPath(Constants.FS_ROOT), DataConverter.GetCrossPlatformPath(path)));

            computer.LoadMemAt(destAdr, fileBytes);
        }

        public static void GetFileSize(byte regId, Computer computer)
        {
            byte pathAdrReg = computer.CPU.REGS.GetNextRegister(regId, 1);
            uint pathAddress = computer.CPU.REGS.Get24BitRegister(pathAdrReg);   // Get the pointer to the path string

            string path = computer.MEMC.GetStringAt(pathAddress);                // Get the string path

            FileInfo fInfo = new(Path.Combine(DataConverter.GetCrossPlatformPath(Constants.FS_ROOT), DataConverter.GetCrossPlatformPath(path)));
            uint fileSize = (uint)fInfo.Length;

            computer.CPU.REGS.Set32BitRegister(pathAdrReg, fileSize);
        }

        // See ExecuteProcessImageAndPalette for the continuation of this protocol
        public static void LoadImageAndPalette(byte regId, Computer computer)
        {
            byte pathAdrReg = computer.CPU.REGS.GetNextRegister(regId, 1);
            byte destAdrReg = computer.CPU.REGS.GetNextRegister(regId, 4);

            uint pathAddress = computer.CPU.REGS.Get24BitRegister(pathAdrReg);   // Get the pointer to the path string
            ImageLoadState.DestinationAddress = computer.CPU.REGS.Get24BitRegister(destAdrReg);       // Get the pointer to the start memory address to load the file to
            ImageLoadState.ImagePath = computer.MEMC.GetStringAt(pathAddress);

            ImageLoadState.ImageIntRegId = regId;
            ImageLoadState.RequestLoadImageAndPalette = true;

            computer.Pause();
        }

        public static void LoadImage(byte regId, Computer computer)
        {
            byte pathAdrReg = computer.CPU.REGS.GetNextRegister(regId, 1);
            byte destAdrReg = computer.CPU.REGS.GetNextRegister(regId, 4);

            uint pathAddress = computer.CPU.REGS.Get24BitRegister(pathAdrReg);   // Get the pointer to the path string

            ImageLoadState.DestinationAddress = computer.CPU.REGS.Get24BitRegister(destAdrReg);       // Get the pointer to the start memory address to load the file to
            ImageLoadState.ImagePath = computer.MEMC.GetStringAt(pathAddress);
            ImageLoadState.RequestLoadImage = true;

            computer.Pause();
        }

        public static void LoadPalette(byte regId, Computer computer)
        {
            byte pathAdrReg = computer.CPU.REGS.GetNextRegister(regId, 1);
            byte destAdrReg = computer.CPU.REGS.GetNextRegister(regId, 4);

            uint pathAddress = computer.CPU.REGS.Get24BitRegister(pathAdrReg);   // Get the pointer to the path string
            ImageLoadState.DestinationAddress = computer.CPU.REGS.Get24BitRegister(destAdrReg);       // Get the pointer to the start memory address to load the file to
            ImageLoadState.ImagePath = computer.MEMC.GetStringAt(pathAddress);

            ImageLoadState.ImageIntRegId = regId;
            ImageLoadState.RequestLoadPalette = true;

            computer.Pause();
        }
    }
}
