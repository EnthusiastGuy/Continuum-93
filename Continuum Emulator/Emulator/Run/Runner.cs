using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator.IO;
using Continuum93.Emulator;
using System.Collections.Generic;
using System.IO;

namespace Continuum93.Emulator.Run
{
    public static class Runner
    {
        public static void LoadAndRun(string asmFile)
        {
            if (!File.Exists(asmFile))
                return;

            string dirPath = Path.GetDirectoryName(asmFile);
            string fileName = Path.GetFileName(asmFile);
            Directory.CreateDirectory(Path.Combine(dirPath, "debug"));

            CompileLog.Reset();
            Machine.COMPUTER.Clear();
            Machine.COMPUTER.Pause();

            Assembler assembler = new();
            string asmSource = FileManager.ReadFile(asmFile);
            assembler.Build(asmSource, asmFile);

            string log = CompileLog.GetLog();
            File.WriteAllText(Path.Combine(dirPath, "debug", fileName + ".log"), log);

            File.WriteAllText(Path.Combine(dirPath, "debug", fileName + ".full.asm"), assembler.FullSource);
            assembler.FullSource = "";

            List<CodeBlock> cBlocks = assembler.BlockManager.GetBlocks();

            if (cBlocks.Count == 0)
                return;

            foreach (CodeBlock cBlock in cBlocks)
            {
                Machine.COMPUTER.LoadMemAt(cBlock.Start, cBlock.Data);
                File.WriteAllBytes(Path.Combine(dirPath, "debug", fileName + "." + cBlock.Start), cBlock.Data);
            }

            Machine.COMPUTER.CPU.REGS.IPO = cBlocks[0].Start;
            Machine.COMPUTER.Unpause();
        }
    }
}
