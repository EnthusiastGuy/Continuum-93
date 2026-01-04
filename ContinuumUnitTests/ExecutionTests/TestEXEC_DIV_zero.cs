
using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{
    // Tests the behavior when a division by zero exists
    public class TestEXEC_DIV_zero
    {
        // DIV r/rr/rrr/rrrr, n
        [Fact]
        public void TestEXEC_DIV_r_n_zero()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.A = 9;

            cp.Build(@"
                DIV A, 0
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0xFF, computer.CPU.REGS.A);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DIV_rr_n_zero()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.AB = 9;

            cp.Build(@"
                DIV AB, 0
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0xFFFF, computer.CPU.REGS.AB);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DIV_rrr_n_zero()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.ABC = 9;

            cp.Build(@"
                DIV ABC, 0
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0xFFFFFF, computer.CPU.REGS.ABC);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DIV_rrrr_n_zero()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.ABCD = 9;

            cp.Build(@"
                DIV ABCD, 0
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0xFFFFFFFF, computer.CPU.REGS.ABCD);
            TUtils.IncrementCountedTests("exec");
        }

        // DIV r/rr/rrr/rrrr, r
        [Fact]
        public void TestEXEC_DIV_r_r_zero()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.A = 9;
            computer.CPU.REGS.E = 0;

            cp.Build(@"
                DIV A, E
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0xFF, computer.CPU.REGS.A);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DIV_rr_r_zero()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.AB = 9;
            computer.CPU.REGS.E = 0;

            cp.Build(@"
                DIV AB, E
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0xFFFF, computer.CPU.REGS.AB);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DIV_rrr_r_zero()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.ABC = 9;
            computer.CPU.REGS.E = 0;

            cp.Build(@"
                DIV ABC, E
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0xFFFFFF, computer.CPU.REGS.ABC);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DIV_rrrr_r_zero()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.ABCD = 9;
            computer.CPU.REGS.E = 0;

            cp.Build(@"
                DIV ABCD, E
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0xFFFFFFFF, computer.CPU.REGS.ABCD);
            TUtils.IncrementCountedTests("exec");
        }

        // DIV rr/rrr/rrrr, rr
        [Fact]
        public void TestEXEC_DIV_rr_rr_zero()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.AB = 9;
            computer.CPU.REGS.EF = 0;

            cp.Build(@"
                DIV AB, EF
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0xFFFF, computer.CPU.REGS.AB);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DIV_rrr_rr_zero()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.ABC = 9;
            computer.CPU.REGS.EF = 0;

            cp.Build(@"
                DIV ABC, EF
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0xFFFFFF, computer.CPU.REGS.ABC);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DIV_rrrr_rr_zero()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.ABCD = 9;
            computer.CPU.REGS.EF = 0;

            cp.Build(@"
                DIV ABCD, EF
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0xFFFFFFFF, computer.CPU.REGS.ABCD);
            TUtils.IncrementCountedTests("exec");
        }

        // DIV rrr/rrrr, rrr
        [Fact]
        public void TestEXEC_DIV_rrr_rrr_zero()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.ABC = 9;
            computer.CPU.REGS.EFG = 0;

            cp.Build(@"
                DIV ABC, EFG
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0xFFFFFF, computer.CPU.REGS.ABC);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DIV_rrrr_rrr_zero()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.ABCD = 9;
            computer.CPU.REGS.EFG = 0;

            cp.Build(@"
                DIV ABCD, EFG
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0xFFFFFFFF, computer.CPU.REGS.ABCD);
            TUtils.IncrementCountedTests("exec");
        }

        // DIV rrrr, rrrr
        [Fact]
        public void TestEXEC_DIV_rrrr_rrrr_zero()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.ABCD = 9;
            computer.CPU.REGS.EFGH = 0;

            cp.Build(@"
                DIV ABCD, EFGH
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0xFFFFFFFF, computer.CPU.REGS.ABCD);
            TUtils.IncrementCountedTests("exec");
        }
    }
}
