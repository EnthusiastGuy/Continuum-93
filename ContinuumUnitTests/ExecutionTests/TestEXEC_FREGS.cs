using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{
    public class TestEXEC_FREGS
    {
        [Fact]
        public void TestEXEC_FREGS_n()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD F0, 0.5
                FREGS 1
                LD F0, 12.5
                FREGS 0
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0.5, computer.CPU.FREGS.GetRegister(0));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_FREGS_n_variation()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD F0, 0.5
                FREGS 1
                LD F0, 12.5
                FREGS 2
                LD F0, 33.3
                FREGS 1
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(12.5, computer.CPU.FREGS.GetRegister(0));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_FREGS_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD F0, 0.5
                FREGS 1
                LD F0, 12.5
                FREGS (20000)    ; should equival with FREGS 0
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0.5, computer.CPU.FREGS.GetRegister(0));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_FREGS_r()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD Z, 1
                LD F0, 0.5
                FREGS Z
                LD F0, 12.5
                INC Z
                FREGS Z
                LD F0, 33.3
                LD Z, 0
                FREGS Z
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0.5, computer.CPU.FREGS.GetRegister(0));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_FREGS_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD F0, 12.5
                FREGS 1
                LD XYZ, 20000
                LD F0, 0.5
                FREGS (XYZ)    ; should equival with FREGS 0
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(12.5, computer.CPU.FREGS.GetRegister(0));
            TUtils.IncrementCountedTests("exec");
        }
    }
}
