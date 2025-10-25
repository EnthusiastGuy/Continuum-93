using Continuum93.Emulator;
using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace Interrupts
{

    public class TestInterrupts_00_Machine
    {
        [Fact]
        public void TestBuildSimpleOrg()
        {
            using Computer computer = new();

            PrepBuild(@"
                #ORG 10001
                LD ABC, 0x123456
                BREAK
            ", computer);

            Assert.Equal(10001, (int)computer.CPU.REGS.BCD);
            TUtils.IncrementCountedTests("interrupts");
        }

        [Fact]
        public void TestBuildSimpleRun()
        {
            using Computer computer = new();

            PrepBuild(@"
                #RUN 5000
                #ORG 10001
                LD ABC, 0x123456
                BREAK
            ", computer);

            Assert.Equal(5000, (int)computer.CPU.REGS.BCD);
            TUtils.IncrementCountedTests("interrupts");
        }

        [Fact]
        public void TestBuildSimpleWithExecute()
        {
            using Computer computer = new();

            PrepBuild(@"
                #RUN 5000
                #ORG 10001
                LD ABC, 0x123456
                BREAK
            ", computer, true);

            Assert.Equal(0x123456, (int)computer.CPU.REGS.ABC);
            TUtils.IncrementCountedTests("interrupts");
        }

        [Fact]
        public void TestBuildSimpleWithRunAndExecute()
        {
            using Computer computer = new();

            PrepBuild(@"
                #RUN 20000
                #ORG 10001
                LD ABC, 0x123456
                BREAK

                #ORG 20000
                LD ABC, 0xABCDEF
                BREAK
            ", computer, true);

            Assert.Equal(0xABCDEF, (int)computer.CPU.REGS.ABC);
            TUtils.IncrementCountedTests("interrupts");
        }

        private void PrepBuild(string source, Computer computer, bool run = false)
        {
            Assembler cp = new();

            Directory.CreateDirectory(Constants.FS_ROOT);
            File.WriteAllText(Path.Combine(Constants.FS_ROOT, "TestInterrupts_00_Machine.asm"), source);

            cp.Build(string.Format(@"
                LD A, 0xC0      ; 0xC0 - Build
                LD BCD, .Path
                INT 0, A
                {0}
                BREAK
            .Path
                ", run ? "JP BCD" : ""
             ) + "#DB \"TestInterrupts_00_Machine.asm\", 0");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            File.Delete(Path.Combine(Constants.FS_ROOT, "TestInterrupts_00_Machine.asm"));
        }
    }
}
