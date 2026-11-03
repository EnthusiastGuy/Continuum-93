using Continuum93.Emulator;
using Continuum93.Emulator.Compilers.C93C;
using Continuum93.Emulator.Interpreter;

namespace ContinuumUnitTests.CCompilerTests
{
    /// <summary>Result of compiling and running a C program on an emulated machine.</summary>
    public sealed class CRunResult
    {
        public int ReturnValue;
        public long Steps;
        public string Assembly = "";
        public Dictionary<string, uint> Labels = [];
        private byte[] _ram = [];
        private uint _ramBase;

        internal void SetMemory(uint baseAddress, byte[] ram)
        {
            _ramBase = baseAddress;
            _ram = ram;
        }

        public uint AddressOf(string cName)
        {
            string label = ".c_" + cName;
            Assert.True(Labels.ContainsKey(label), $"No label for C symbol '{cName}'");
            return Labels[label];
        }

        public byte[] Read(uint address, int length)
        {
            byte[] r = new byte[length];
            Array.Copy(_ram, address - _ramBase, r, 0, length);
            return r;
        }

        public int GlobalInt(string name, int index = 0)
        {
            byte[] b = Read(AddressOf(name) + (uint)(index * 4), 4);
            return (b[0] << 24) | (b[1] << 16) | (b[2] << 8) | b[3];
        }

        public uint GlobalUInt(string name, int index = 0) => (uint)GlobalInt(name, index);

        public byte GlobalByte(string name, int index = 0) => Read(AddressOf(name) + (uint)index, 1)[0];

        public string GlobalString(string name)
        {
            uint a = AddressOf(name);
            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < 4096; i++)
            {
                byte c = Read(a + (uint)i, 1)[0];
                if (c == 0) break;
                sb.Append((char)c);
            }
            return sb.ToString();
        }
    }

    public static class CTestHarness
    {
        private const uint StubAddress = 0x001000;

        /// <summary>Compiles, assembles and runs a C program; fails the test on any compile error or runaway program.</summary>
        public static CRunResult Run(string source, Dictionary<string, string>? includes = null, long maxSteps = 20_000_000)
        {
            var compiler = new CCompiler();
            if (includes != null)
                compiler.IncludeResolver = (name, _) => includes.TryGetValue(name, out string? s) ? s : null;

            string asm = compiler.Compile(source);
            Assert.True(compiler.Errors == 0, "C compile errors:\n" + compiler.Log);

            CompileLog.Reset();
            var assembler = new Assembler();
            assembler.Build(asm);
            if (assembler.Errors > 0)
            {
                string errors = string.Join("\n", CompileLog.GetLog().Split('\n').Where(l => l.Contains("rror")).Take(20));
                Assert.Fail($"Assembler reported {assembler.Errors} error(s):\n{errors}\n--- generated assembly ---\n{asm}");
            }

            var stub = new Assembler();
            stub.Build($"#ORG 0x{StubAddress:X6}\n    CALL 0x{CCodeGen.CodeOrigin:X6}\n    BREAK\n");

            using var computer = new Computer();
            computer.Clear();
            foreach (var block in assembler.BlockManager.GetBlocks())
                computer.LoadMemAt(block.Start, block.Data);
            foreach (var block in stub.BlockManager.GetBlocks())
                computer.LoadMemAt(block.Start, block.Data);

            uint sprBefore = computer.CPU.REGS.SPR;
            uint spcBefore = computer.CPU.REGS.SPC;
            computer.CPU.REGS.IPO = StubAddress;

            long steps = 0;
            while (computer.IsRunning && steps < maxSteps)
            {
                computer.ExecuteNextInstruction();
                steps++;
            }
            Assert.True(!computer.IsRunning, $"Program did not finish within {maxSteps} instructions");

            // Invariants: balanced hardware stacks and the C stack pointer restored
            Assert.True(computer.CPU.REGS.SPR == sprBefore, $"Register stack unbalanced: SPR {computer.CPU.REGS.SPR} vs {sprBefore}");
            Assert.True(computer.CPU.REGS.SPC == spcBefore, $"Call stack unbalanced: SPC {computer.CPU.REGS.SPC} vs {spcBefore}");
            uint uvw = computer.CPU.REGS.Get24BitRegister((byte)('U' - 'A'));
            Assert.True(uvw == CCodeGen.StackTop, $"C stack pointer not restored: 0x{uvw:X6}");

            var result = new CRunResult
            {
                ReturnValue = (int)computer.CPU.REGS.Get32BitRegister(0),
                Steps = steps,
                Assembly = asm,
                Labels = new Dictionary<string, uint>(assembler.Stats.Labels)
            };

            uint heap = assembler.Stats.Labels[CCodeGen.HeapStartLabel];
            uint end = Math.Min(heap + 0x20000, 0xFFFFFF);
            result.SetMemory(CCodeGen.CodeOrigin, computer.GetMemFrom(CCodeGen.CodeOrigin, end - CCodeGen.CodeOrigin));
            return result;
        }

        /// <summary>Compiles and runs, returning main's return value.</summary>
        public static int Eval(string source, Dictionary<string, string>? includes = null) => Run(source, includes).ReturnValue;

        /// <summary>Compiles a program that is expected to fail and returns the errors.</summary>
        public static List<CCompileError> CompileErrors(string source, Dictionary<string, string>? includes = null)
        {
            var compiler = new CCompiler();
            if (includes != null)
                compiler.IncludeResolver = (name, _) => includes.TryGetValue(name, out string? s) ? s : null;
            compiler.Compile(source);
            Assert.True(compiler.Errors > 0, "Expected compile errors but compilation succeeded");
            return compiler.ErrorList;
        }

        /// <summary>Asserts that compilation fails with an error whose message contains the given text.</summary>
        public static void AssertError(string source, string expectedFragment)
        {
            var errors = CompileErrors(source);
            Assert.True(errors.Any(e => e.Message.Contains(expectedFragment, StringComparison.OrdinalIgnoreCase)),
                $"Expected an error containing '{expectedFragment}', got:\n{string.Join("\n", errors)}");
        }
    }
}
