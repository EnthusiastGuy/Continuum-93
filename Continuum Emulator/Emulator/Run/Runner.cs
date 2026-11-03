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

            // C boot programs (e.g. the dual-boot loader) are compiled to assembly first
            if (Path.GetExtension(asmFile).Equals(".c", System.StringComparison.OrdinalIgnoreCase))
            {
                var cCompiler = new Compilers.C93C.CCompiler
                {
                    IncludeResolver = (name, includingFile) =>
                    {
                        string candidate = Path.Combine(dirPath, Path.GetDirectoryName(includingFile) ?? "", name);
                        if (!File.Exists(candidate))
                            candidate = Path.Combine(dirPath, name);
                        return File.Exists(candidate) ? File.ReadAllText(candidate) : null;
                    }
                };
                asmSource = cCompiler.Compile(asmSource, fileName);
                File.WriteAllText(Path.Combine(dirPath, "debug", fileName + ".c.log"), cCompiler.Log);
                if (cCompiler.Errors > 0)
                {
                    Log.WriteLine($"Boot program {fileName} failed to compile: {cCompiler.ErrorList[0]}");
                    return;
                }
                asmFile = Path.Combine(dirPath, "debug", fileName + ".generated.asm");
                File.WriteAllText(asmFile, asmSource);
            }

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
