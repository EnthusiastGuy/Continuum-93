using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator.IO;
using Continuum93.Emulator;
using Continuum93.Emulator.States;
using Continuum93.Emulator.Compilers.C93Basic;
using System;
using System.Collections.Generic;
using System.IO;

namespace Continuum93.Emulator.Interrupts
{
    public class InterruptsMachine
    {
        public static void Stop(Computer computer)
        {
            computer.Stop();
        }

        public static void Clear(Computer computer)
        {
            computer.Clear();
        }

        public static void ReadClock(byte regId, Computer computer)
        {
            byte rModeIndex = computer.CPU.REGS.GetNextRegister(regId, 1);
            byte mode = computer.CPU.REGS.Get8BitRegister(rModeIndex);
            byte destinationIndex = computer.CPU.REGS.GetNextRegister(regId, 2);
            uint destAdr = computer.CPU.REGS.Get24BitRegister(destinationIndex);

            switch (mode)
            {
                case 0x00:  // return time since machine started in milliseconds
                    {
                        uint time = (uint)GameTimePlus.GetTotalMs();
                        computer.MEMC.Set32bitToRAM(destAdr, time);
                        return;
                    }
                case 0x01:  // return time since machine started in ticks
                    {
                        byte[] clockTicks = BitConverter.GetBytes(GameTimePlus.GetTotalTicks());
                        Array.Reverse(clockTicks);
                        computer.LoadMemAt(destAdr, clockTicks);
                        return;
                    }
            }
        }

        public static void Build(byte regId, Computer computer)
        {
            // Reg ID 
            byte nRegister = computer.CPU.REGS.GetNextRegister(regId, 1);
            uint pathAddress = computer.CPU.REGS.Get24BitRegister(nRegister);

            // Get filepath
            string filePath = Path.Combine(DataConverter.GetCrossPlatformPath(Constants.FS_ROOT), DataConverter.GetCrossPlatformPath(computer.MEMC.GetStringAt(pathAddress)));
            CompileLog.Reset();

            Assembler assembler = new();
            if (!FileManager.FileExists(filePath))
            {
                computer.CPU.REGS.Set24BitRegister(nRegister, 0xFFFFFF);
                return;
            }

            string dirPath = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileName(filePath);
            Directory.CreateDirectory(Path.Combine(dirPath, "debug"));

            string asmSource = FileManager.ReadFile(filePath);
            assembler.Build(asmSource, filePath);

            string log = CompileLog.GetLog();
            File.WriteAllText(Path.Combine(dirPath, "debug", fileName + ".log"), log);

            File.WriteAllText(Path.Combine(dirPath, "debug", fileName + ".full.asm"), assembler.FullSource);
            assembler.FullSource = "";

            List<CodeBlock> cBlocks = assembler.BlockManager.GetBlocks();

            if (cBlocks.Count == 0)
                return;

            foreach (CodeBlock cBlock in cBlocks)
            {
                computer.LoadMemAt(cBlock.Start, cBlock.Data);
                File.WriteAllBytes(Path.Combine(dirPath, "debug", fileName + "." + cBlock.Start), cBlock.Data);
            }

            cBlocks.Clear();

            uint address = assembler.GetRunAddress();
            computer.CPU.REGS.Set24BitRegister(nRegister, address);

            byte eRegister = computer.CPU.REGS.GetNextRegister(regId, 4);
            byte errors = assembler.Errors > 255 ? (byte)255 : (byte)assembler.Errors;
            computer.CPU.REGS.Set8BitRegister(eRegister, errors);
        }

        public static void ShutDown()
        {
            State.ShutDownRequested = true;
        }

        public static void ToggleFullscreen(byte regId, Computer computer)
        {
            State.FullScreenRequest = true;
            computer.CPU.REGS.Set8BitRegister(regId, (byte)(Renderer.IsFullScreen() ? 1 : 0));
        }

        public static void BuildBasic(byte regId, Computer computer)
        {
            // Reg ID 
            byte nRegister = computer.CPU.REGS.GetNextRegister(regId, 1);
            uint pathAddress = computer.CPU.REGS.Get24BitRegister(nRegister);

            // Get filepath
            string filePath = Path.Combine(DataConverter.GetCrossPlatformPath(Constants.FS_ROOT), DataConverter.GetCrossPlatformPath(computer.MEMC.GetStringAt(pathAddress)));
            CompileLog.Reset();

            if (!FileManager.FileExists(filePath))
            {
                computer.CPU.REGS.Set24BitRegister(nRegister, 0xFFFFFF);
                return;
            }

            string dirPath = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileName(filePath);
            Directory.CreateDirectory(Path.Combine(dirPath, "debug"));

            // Read BASIC source
            string basicSource = FileManager.ReadFile(filePath);

            // Compile BASIC to assembly
            BasicCompiler basicCompiler = new BasicCompiler();
            uint codeStartAddress = 0x080000; // Default start address
            string assemblyCode = basicCompiler.Compile(basicSource, codeStartAddress);

            if (basicCompiler.Errors > 0)
            {
                // Write compiler log
                File.WriteAllText(Path.Combine(dirPath, "debug", fileName + ".basic.log"), basicCompiler.Log);
                
                byte basicErrorReg = computer.CPU.REGS.GetNextRegister(regId, 4);
                byte basicErrorCount = basicCompiler.Errors > 255 ? (byte)255 : (byte)basicCompiler.Errors;
                computer.CPU.REGS.Set8BitRegister(basicErrorReg, basicErrorCount);
                computer.CPU.REGS.Set24BitRegister(nRegister, 0xFFFFFF);
                return;
            }

            // Write generated assembly for debugging
            File.WriteAllText(Path.Combine(dirPath, "debug", fileName + ".generated.asm"), assemblyCode);

            // Compile assembly
            Assembler assembler = new();
            assembler.Build(assemblyCode, Path.Combine(dirPath, "debug", fileName + ".generated.asm"));

            string log = CompileLog.GetLog();
            File.WriteAllText(Path.Combine(dirPath, "debug", fileName + ".log"), log);

            File.WriteAllText(Path.Combine(dirPath, "debug", fileName + ".full.asm"), assembler.FullSource);
            assembler.FullSource = "";

            List<CodeBlock> cBlocks = assembler.BlockManager.GetBlocks();

            if (cBlocks.Count == 0)
            {
                computer.CPU.REGS.Set24BitRegister(nRegister, 0xFFFFFF);
                return;
            }

            foreach (CodeBlock cBlock in cBlocks)
            {
                computer.LoadMemAt(cBlock.Start, cBlock.Data);
                File.WriteAllBytes(Path.Combine(dirPath, "debug", fileName + "." + cBlock.Start), cBlock.Data);
            }

            cBlocks.Clear();

            uint address = assembler.GetRunAddress();
            computer.CPU.REGS.Set24BitRegister(nRegister, address);

            byte errorReg = computer.CPU.REGS.GetNextRegister(regId, 4);
            byte totalErrors = (byte)(basicCompiler.Errors + assembler.Errors);
            if (totalErrors > 255) totalErrors = 255;
            computer.CPU.REGS.Set8BitRegister(errorReg, totalErrors);
        }

        public static void GetCPUDesignationByFrequency(byte regId, Computer computer)
        {
            // Get the register containing the frequency
            byte regIndex = computer.CPU.REGS.GetNextRegister(regId, 1);
            ushort frequency = computer.CPU.REGS.Get16BitRegister(regIndex);

            // Get the destination address
            regIndex = computer.CPU.REGS.GetNextRegister(regId, 3);
            uint destAdr = computer.CPU.REGS.Get24BitRegister(regIndex);

            // Determine the CPU designation based on frequency
            string designation;
            if (frequency < 2)
                designation = "Proto";
            else if (frequency <= 3)
                designation = "Nova I";
            else if (frequency <= 5)
                designation = "Nova II";
            else if (frequency <= 7)
                designation = "Nova III";
            else if (frequency <= 10)
                designation = "Comet";
            else if (frequency <= 12)
                designation = "Comet II";
            else if (frequency <= 15)
                designation = "Comet III";
            else if (frequency <= 18)
                designation = "Comet Prime";
            else if (frequency <= 22)
                designation = "Meteor";
            else if (frequency <= 30)
                designation = "Meteor II";
            else if (frequency <= 40)
                designation = "Apollo";
            else if (frequency <= 50)
                designation = "Apollo II";
            else if (frequency <= 60)
                designation = "Apollo III";
            else if (frequency <= 75)
                designation = "Phoenix I";
            else if (frequency <= 90)
                designation = "Phoenix II";
            else if (frequency <= 100)
                designation = "Phoenix Keanu";
            else if (frequency <= 120)
                designation = "Titan";
            else if (frequency <= 150)
                designation = "Titan II";
            else if (frequency <= 175)
                designation = "Titan Keanu";
            else if (frequency <= 200)
                designation = "Vulcan I";
            else if (frequency <= 250)
                designation = "Vulcan II";
            else if (frequency <= 300)
                designation = "Vulcan III";
            else if (frequency <= 350)
                designation = "Atlas";
            else if (frequency <= 400)
                designation = "Atlas Keanu";
            else if (frequency <= 500)
                designation = "Chronos I";
            else if (frequency <= 600)
                designation = "Chronos II";
            else if (frequency <= 700)
                designation = "Chronos Keanu";
            else if (frequency <= 850)
                designation = "Odin I";
            else if (frequency <= 1000)
                designation = "Odin II";
            else
                designation = $"Beyond Odin +{(frequency - 1000)/100 + 1}";

            // Convert designation to a null-terminated byte array
            byte[] stringArr = new byte[designation.Length + 1];
            for (int i = 0; i < designation.Length; i++)
            {
                stringArr[i] = (byte)designation[i];
            }
            stringArr[designation.Length] = 0; // Null-terminate

            // Write the designation to memory at destAdr
            computer.LoadMemAt(destAdr, stringArr);
        }

    }
}
