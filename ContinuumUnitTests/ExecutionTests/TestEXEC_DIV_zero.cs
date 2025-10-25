
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


        // With remainder
        //
        [Fact]
        public void TestEXEC_DIV_r_n_r_zero()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.A = 9;
            computer.CPU.REGS.I = 0x80;

            cp.Build(@"
                DIV A, 0, I
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0xFF, computer.CPU.REGS.A);
            Assert.Equal(0, computer.CPU.REGS.I);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DIV_r_r_r_zero()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.A = 9;
            computer.CPU.REGS.E = 0;
            computer.CPU.REGS.I = 0x80;

            cp.Build(@"
                DIV A, E, I
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0xFF, computer.CPU.REGS.A);
            Assert.Equal(0, computer.CPU.REGS.E);
            Assert.Equal(0, computer.CPU.REGS.I);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DIV_rr_nn_rr_zero()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.AB = 9;
            computer.CPU.REGS.IJ = 0x8000;

            cp.Build(@"
                DIV AB, 0, IJ
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0xFFFF, computer.CPU.REGS.AB);
            Assert.Equal(0, computer.CPU.REGS.IJ);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DIV_rr_r_r_zero()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.AB = 9;
            computer.CPU.REGS.E = 0;
            computer.CPU.REGS.I = 0x80;

            cp.Build(@"
                DIV AB, E, I
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0xFFFF, computer.CPU.REGS.AB);
            Assert.Equal(0, computer.CPU.REGS.E);
            Assert.Equal(0, computer.CPU.REGS.I);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DIV_rr_rr_rr_zero()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.AB = 9;
            computer.CPU.REGS.EF = 0;
            computer.CPU.REGS.IJ = 0x8000;

            cp.Build(@"
                DIV AB, EF, IJ
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0xFFFF, computer.CPU.REGS.AB);
            Assert.Equal(0, computer.CPU.REGS.EF);
            Assert.Equal(0, computer.CPU.REGS.IJ);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DIV_rrr_nn_rr_zero()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.ABC = 9;
            computer.CPU.REGS.IJ = 0x8000;

            cp.Build(@"
                DIV ABC, 0, IJ
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0xFFFFFF, computer.CPU.REGS.ABC);
            Assert.Equal(0, computer.CPU.REGS.IJ);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DIV_rrr_r_r_zero()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.ABC = 9;
            computer.CPU.REGS.E = 0;
            computer.CPU.REGS.I = 0x80;

            cp.Build(@"
                DIV ABC, E, I
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0xFFFFFF, computer.CPU.REGS.ABC);
            Assert.Equal(0, computer.CPU.REGS.E);
            Assert.Equal(0, computer.CPU.REGS.I);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DIV_rrr_rr_rr_zero()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.ABC = 9;
            computer.CPU.REGS.EF = 0;
            computer.CPU.REGS.IJ = 0x8000;

            cp.Build(@"
                DIV ABC, EF, IJ
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0xFFFFFF, computer.CPU.REGS.ABC);
            Assert.Equal(0, computer.CPU.REGS.EF);
            Assert.Equal(0, computer.CPU.REGS.IJ);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DIV_rrr_rrr_rrr_zero()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.ABC = 9;
            computer.CPU.REGS.EFG = 0;
            computer.CPU.REGS.IJK = 0x800000;

            cp.Build(@"
                DIV ABC, EFG, IJK
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0xFFFFFF, computer.CPU.REGS.ABC);
            Assert.Equal((long)0, computer.CPU.REGS.EFG);
            Assert.Equal((long)0, computer.CPU.REGS.IJK);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DIV_rrrr_nn_rr_zero()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.ABCD = 9;
            computer.CPU.REGS.IJ = 0x8000;

            cp.Build(@"
                DIV ABCD, 0, IJ
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0xFFFFFFFF, computer.CPU.REGS.ABCD);
            Assert.Equal(0, computer.CPU.REGS.IJ);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DIV_rrrr_r_r_zero()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.ABCD = 9;
            computer.CPU.REGS.E = 0;
            computer.CPU.REGS.I = 0x80;

            cp.Build(@"
                DIV ABCD, E, I
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0xFFFFFFFF, computer.CPU.REGS.ABCD);
            Assert.Equal(0, computer.CPU.REGS.E);
            Assert.Equal(0, computer.CPU.REGS.I);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DIV_rrrr_rr_rr_zero()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.ABCD = 9;
            computer.CPU.REGS.EF = 0;
            computer.CPU.REGS.IJ = 0x8000;

            cp.Build(@"
                DIV ABCD, EF, IJ
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0xFFFFFFFF, computer.CPU.REGS.ABCD);
            Assert.Equal(0, computer.CPU.REGS.EF);
            Assert.Equal(0, computer.CPU.REGS.IJ);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DIV_rrrr_rrr_rrr_zero()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.ABCD = 9;
            computer.CPU.REGS.EFG = 0;
            computer.CPU.REGS.IJK = 0x800000;

            cp.Build(@"
                DIV ABCD, EFG, IJK
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0xFFFFFFFF, computer.CPU.REGS.ABCD);
            Assert.Equal((long)0, computer.CPU.REGS.EFG);
            Assert.Equal((long)0, computer.CPU.REGS.IJK);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DIV_rrrr_rrrr_rrrr_zero()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.ABCD = 9;
            computer.CPU.REGS.EFGH = 0;
            computer.CPU.REGS.IJKL = 0x80000000;

            cp.Build(@"
                DIV ABCD, EFGH, IJKL
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0xFFFFFFFF, computer.CPU.REGS.ABCD);
            Assert.Equal((long)0, computer.CPU.REGS.EFGH);
            Assert.Equal((long)0, computer.CPU.REGS.IJKL);
            TUtils.IncrementCountedTests("exec");
        }
    }
}
