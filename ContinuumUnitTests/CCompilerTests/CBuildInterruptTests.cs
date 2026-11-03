using Continuum93.Emulator;
using Continuum93.Emulator.Interpreter;

namespace ContinuumUnitTests.CCompilerTests
{
    /// <summary>End-to-end tests of INT 0x00 / 0xC2 (Build C): source file on the virtual filesystem → running program.</summary>
    [Collection("CCompiler")]
    public class CBuildInterruptTests
    {
        private static Computer BuildAndMaybeRun(string relativePath, bool run)
        {
            var stub = new Assembler();
            stub.Build($@"
                LD A, 0xC2      ; Build C
                LD BCD, .Path
                INT 0, A
                {(run ? "CALL BCD" : "")}
                BREAK
            .Path
                #DB ""{relativePath.Replace('\\', '/')}"", 0");

            var computer = new Computer();
            computer.LoadMem(stub.GetCompiledCode());
            long steps = 0;
            while (computer.IsRunning && steps++ < 5_000_000)
                computer.ExecuteNextInstruction();
            return computer;
        }

        private static string WriteFile(string relativePath, string content)
        {
            string full = Path.Combine(Constants.FS_ROOT, relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(full)!);
            File.WriteAllText(full, content);
            return full;
        }

        [Fact]
        public void BuildsAndRunsCFileWithLocalInclude()
        {
            string dir = Path.Combine("ctests", "build_ok");
            WriteFile(Path.Combine(dir, "include", "math_utils.h"), "int square(int x) { return x * x; }\n");
            WriteFile(Path.Combine(dir, "main.c"), "#include \"include/math_utils.h\"\nint main(void) { return square(12) + 1; }\n");

            using Computer computer = BuildAndMaybeRun("ctests/build_ok/main.c", true);

            Assert.Equal(145u, computer.CPU.REGS.Get32BitRegister(0));            // ABCD = main's return value
            Assert.True(File.Exists(Path.Combine(Constants.FS_ROOT, dir, "debug", "main.c.generated.asm")));
        }

        [Fact]
        public void ReportsCompileErrorsAndWritesLog()
        {
            WriteFile(Path.Combine("ctests", "build_err", "bad.c"), "int main(void) { return undefined_thing; }\n");

            using Computer computer = BuildAndMaybeRun("ctests/build_err/bad.c", false);

            Assert.Equal(0xFFFFFFu, computer.CPU.REGS.Get24BitRegister((byte)('B' - 'A')));  // BCD
            Assert.True(computer.CPU.REGS.Get8BitRegister((byte)('E' - 'A')) > 0);           // E = error count
            string log = File.ReadAllText(Path.Combine(Constants.FS_ROOT, "ctests", "build_err", "debug", "bad.c.c.log"));
            Assert.Contains("Undeclared identifier 'undefined_thing'", log);
        }

        [Fact]
        public void MissingFileReturnsFailureAddress()
        {
            using Computer computer = BuildAndMaybeRun("ctests/does_not_exist.c", false);

            Assert.Equal(0xFFFFFFu, computer.CPU.REGS.Get24BitRegister((byte)('B' - 'A')));
            Assert.True(computer.CPU.REGS.Get8BitRegister((byte)('E' - 'A')) > 0);
        }

        [Fact]
        public void SuccessfulBuildReportsZeroErrorsAndRunAddress()
        {
            WriteFile(Path.Combine("ctests", "build_addr", "p.c"), "int main(void) { return 0; }\n");

            using Computer computer = BuildAndMaybeRun("ctests/build_addr/p.c", false);

            Assert.Equal(0x080000u, computer.CPU.REGS.Get24BitRegister((byte)('B' - 'A')));
            Assert.Equal(0, computer.CPU.REGS.Get8BitRegister((byte)('E' - 'A')));
        }
    }
}
