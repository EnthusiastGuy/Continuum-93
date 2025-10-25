using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace CPUTests
{
    public class TestFLAGS_DIV
    {
        [Fact]
        public void TestDiv_r_n_noCarry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            computer.CPU.FLAGS.SetCarry(true);
            Assert.True(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.A = 0xFF;

            cp.Build(@"
                DIV A, 2
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.False(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestDiv_r_0_carry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.A = 0xFF;

            cp.Build(@"
                DIV A, 0
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.True(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestDiv_rr_n_noCarry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            computer.CPU.FLAGS.SetCarry(true);
            Assert.True(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.AB = 0xFFFF;

            cp.Build(@"
                DIV AB, 2
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.False(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestDiv_rr_0_carry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.AB = 0xFFFF;

            cp.Build(@"
                DIV AB, 0
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.True(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestDiv_rrr_n_noCarry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            computer.CPU.FLAGS.SetCarry(true);
            Assert.True(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.ABC = 0xFFFFFF;

            cp.Build(@"
                DIV ABC, 2
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.False(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestDiv_rrr_0_carry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.ABC = 0xFFFFFF;

            cp.Build(@"
                DIV ABC, 0
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.True(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestDiv_rrrr_n_noCarry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            computer.CPU.FLAGS.SetCarry(true);
            Assert.True(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.ABCD = 0xFFFFFFFF;

            cp.Build(@"
                DIV ABCD, 2
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.False(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestDiv_rrrr_0_carry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.ABCD = 0xFFFFFFFF;

            cp.Build(@"
                DIV ABCD, 0
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.True(computer.CPU.FLAGS.IsCarry());
        }

        // DIV r, r
        [Fact]
        public void TestDiv_r_r_noCarry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            computer.CPU.FLAGS.SetCarry(true);
            Assert.True(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.A = 0xFF;
            computer.CPU.REGS.E = 2;

            cp.Build(@"
                DIV A, E
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.False(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestDiv_r_r_carry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.A = 0xFF;
            computer.CPU.REGS.E = 0;

            cp.Build(@"
                DIV A, E
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.True(computer.CPU.FLAGS.IsCarry());
        }

        // DIV rr, r
        [Fact]
        public void TestDiv_rr_r_noCarry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            computer.CPU.FLAGS.SetCarry(true);
            Assert.True(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.AB = 0xFFFF;
            computer.CPU.REGS.E = 2;

            cp.Build(@"
                DIV AB, E
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.False(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestDiv_rr_r_carry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.AB = 0xFFFF;
            computer.CPU.REGS.E = 0;

            cp.Build(@"
                DIV AB, E
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.True(computer.CPU.FLAGS.IsCarry());
        }

        // DIV rr, rr
        [Fact]
        public void TestDiv_rr_rr_noCarry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            computer.CPU.FLAGS.SetCarry(true);
            Assert.True(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.AB = 0xFFFF;
            computer.CPU.REGS.EF = 2;

            cp.Build(@"
                DIV AB, EF
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.False(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestDiv_rr_rr_carry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.AB = 0xFFFF;
            computer.CPU.REGS.EF = 0;

            cp.Build(@"
                DIV AB, EF
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.True(computer.CPU.FLAGS.IsCarry());
        }

        // DIV rrr, r
        [Fact]
        public void TestDiv_rrr_r_noCarry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            computer.CPU.FLAGS.SetCarry(true);
            Assert.True(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.ABC = 0xFFFFFF;
            computer.CPU.REGS.E = 2;

            cp.Build(@"
                DIV ABC, E
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.False(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestDiv_rrr_r_carry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.ABC = 0xFFFFFF;
            computer.CPU.REGS.E = 0;

            cp.Build(@"
                DIV ABC, E
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.True(computer.CPU.FLAGS.IsCarry());
        }

        // DIV rrr, rr
        [Fact]
        public void TestDiv_rrr_rr_noCarry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            computer.CPU.FLAGS.SetCarry(true);
            Assert.True(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.ABC = 0xFFFFFF;
            computer.CPU.REGS.EF = 2;

            cp.Build(@"
                DIV ABC, EF
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.False(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestDiv_rrr_rr_carry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.ABC = 0xFFFFFF;
            computer.CPU.REGS.EF = 0;

            cp.Build(@"
                DIV ABC, EF
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.True(computer.CPU.FLAGS.IsCarry());
        }

        // DIV rrr, rrr
        [Fact]
        public void TestDiv_rrr_rrr_noCarry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            computer.CPU.FLAGS.SetCarry(true);
            Assert.True(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.ABC = 0xFFFFFF;
            computer.CPU.REGS.EFG = 2;

            cp.Build(@"
                DIV ABC, EFG
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.False(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestDiv_rrr_rrr_carry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.ABC = 0xFFFFFF;
            computer.CPU.REGS.EFG = 0;

            cp.Build(@"
                DIV ABC, EFG
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.True(computer.CPU.FLAGS.IsCarry());
        }

        // DIV rrrr, r
        [Fact]
        public void TestDiv_rrrr_r_noCarry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            computer.CPU.FLAGS.SetCarry(true);
            Assert.True(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.ABCD = 0xFFFFFFFF;
            computer.CPU.REGS.E = 2;

            cp.Build(@"
                DIV ABCD, E
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.False(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestDiv_rrrr_r_carry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.ABCD = 0xFFFFFFFF;
            computer.CPU.REGS.E = 0;

            cp.Build(@"
                DIV ABCD, E
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.True(computer.CPU.FLAGS.IsCarry());
        }

        // DIV rrrr, rr
        [Fact]
        public void TestDiv_rrrr_rr_noCarry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            computer.CPU.FLAGS.SetCarry(true);
            Assert.True(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.ABCD = 0xFFFFFFFF;
            computer.CPU.REGS.EF = 2;

            cp.Build(@"
                DIV ABCD, EF
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.False(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestDiv_rrrr_rr_carry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.ABCD = 0xFFFFFFFF;
            computer.CPU.REGS.EF = 0;

            cp.Build(@"
                DIV ABCD, EF
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.True(computer.CPU.FLAGS.IsCarry());
        }

        // DIV rrrr, rrr
        [Fact]
        public void TestDiv_rrrr_rrr_noCarry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            computer.CPU.FLAGS.SetCarry(true);
            Assert.True(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.ABCD = 0xFFFFFFFF;
            computer.CPU.REGS.EFG = 2;

            cp.Build(@"
                DIV ABCD, EFG
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.False(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestDiv_rrrr_rrr_carry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.ABCD = 0xFFFFFFFF;
            computer.CPU.REGS.EFG = 0;

            cp.Build(@"
                DIV ABCD, EFG
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.True(computer.CPU.FLAGS.IsCarry());
        }

        // DIV rrrr, rrrr
        [Fact]
        public void TestDiv_rrrr_rrrr_noCarry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            computer.CPU.FLAGS.SetCarry(true);
            Assert.True(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.ABCD = 0xFFFFFFFF;
            computer.CPU.REGS.EFGH = 2;

            cp.Build(@"
                DIV ABCD, EFGH
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.False(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestDiv_rrrr_rrrr_carry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.ABCD = 0xFFFFFFFF;
            computer.CPU.REGS.EFGH = 0;

            cp.Build(@"
                DIV ABCD, EFGH
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.True(computer.CPU.FLAGS.IsCarry());
        }


        // With remainder
        [Fact]
        public void TestDiv_r_n_r_noCarry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            computer.CPU.FLAGS.SetCarry(true);
            Assert.True(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.A = 0xFF;

            cp.Build(@"
                DIV A, 2, I
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.False(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestDiv_r_0_r_carry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.A = 0xFF;

            cp.Build(@"
                DIV A, 0, I
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.True(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestDiv_r_r_r_noCarry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            computer.CPU.FLAGS.SetCarry(true);
            Assert.True(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.A = 0xFF;
            computer.CPU.REGS.E = 2;

            cp.Build(@"
                DIV A, E, I
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.False(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestDiv_r_r_r_carry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.A = 0xFF;
            computer.CPU.REGS.E = 0;

            cp.Build(@"
                DIV A, E, I
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.True(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestDiv_rr_n_rr_noCarry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            computer.CPU.FLAGS.SetCarry(true);
            Assert.True(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.AB = 0xFF;

            cp.Build(@"
                DIV AB, 2, IJ
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.False(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestDiv_rr_0_rr_carry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.AB = 0xFF;

            cp.Build(@"
                DIV AB, 0, IJ
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.True(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestDiv_rr_r_r_noCarry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            computer.CPU.FLAGS.SetCarry(true);
            Assert.True(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.AB = 0xFF;
            computer.CPU.REGS.E = 2;

            cp.Build(@"
                DIV AB, E, I
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.False(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestDiv_rr_r_r_carry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.AB = 0xFF;
            computer.CPU.REGS.E = 0;

            cp.Build(@"
                DIV AB, E, I
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.True(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestDiv_rr_rr_rr_noCarry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            computer.CPU.FLAGS.SetCarry(true);
            Assert.True(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.AB = 0xFF;
            computer.CPU.REGS.EF = 2;

            cp.Build(@"
                DIV AB, EF, IJ
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.False(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestDiv_rr_rr_rr_carry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.AB = 0xFF;
            computer.CPU.REGS.EF = 0;

            cp.Build(@"
                DIV AB, EF, IJ
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.True(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestDiv_rrr_n_rr_noCarry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            computer.CPU.FLAGS.SetCarry(true);
            Assert.True(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.ABC = 0xFF;

            cp.Build(@"
                DIV ABC, 2, IJ
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.False(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestDiv_rrr_0_rr_carry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.ABC = 0xFF;

            cp.Build(@"
                DIV ABC, 0, IJ
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.True(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestDiv_rrr_r_r_noCarry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            computer.CPU.FLAGS.SetCarry(true);
            Assert.True(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.ABC = 0xFF;
            computer.CPU.REGS.E = 2;

            cp.Build(@"
                DIV ABC, E, I
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.False(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestDiv_rrr_r_r_carry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.ABC = 0xFF;
            computer.CPU.REGS.E = 0;

            cp.Build(@"
                DIV ABC, E, I
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.True(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestDiv_rrr_rr_rr_noCarry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            computer.CPU.FLAGS.SetCarry(true);
            Assert.True(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.ABC = 0xFF;
            computer.CPU.REGS.EF = 2;

            cp.Build(@"
                DIV ABC, EF, IJ
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.False(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestDiv_rrr_rr_rr_carry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.ABC = 0xFF;
            computer.CPU.REGS.EF = 0;

            cp.Build(@"
                DIV ABC, EF, IJ
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.True(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestDiv_rrr_rrr_rrr_noCarry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            computer.CPU.FLAGS.SetCarry(true);
            Assert.True(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.ABC = 0xFF;
            computer.CPU.REGS.EFG = 2;

            cp.Build(@"
                DIV ABC, EFG, IJK
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.False(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestDiv_rrr_rrr_rrr_carry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.ABC = 0xFF;
            computer.CPU.REGS.EFG = 0;

            cp.Build(@"
                DIV ABC, EFG, IJK
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.True(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestDiv_rrrr_n_rr_noCarry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            computer.CPU.FLAGS.SetCarry(true);
            Assert.True(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.ABCD = 0xFF;

            cp.Build(@"
                DIV ABCD, 2, IJ
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.False(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestDiv_rrrr_0_rr_carry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.ABCD = 0xFF;

            cp.Build(@"
                DIV ABCD, 0, IJ
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.True(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestDiv_rrrr_r_r_noCarry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            computer.CPU.FLAGS.SetCarry(true);
            Assert.True(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.ABCD = 0xFF;
            computer.CPU.REGS.E = 2;

            cp.Build(@"
                DIV ABCD, E, I
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.False(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestDiv_rrrr_r_r_carry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.ABCD = 0xFF;
            computer.CPU.REGS.E = 0;

            cp.Build(@"
                DIV ABCD, E, I
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.True(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestDiv_rrrr_rr_rr_noCarry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            computer.CPU.FLAGS.SetCarry(true);
            Assert.True(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.ABCD = 0xFF;
            computer.CPU.REGS.EF = 2;

            cp.Build(@"
                DIV ABCD, EF, IJ
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.False(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestDiv_rrrr_rr_rr_carry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.ABCD = 0xFF;
            computer.CPU.REGS.EF = 0;

            cp.Build(@"
                DIV ABCD, EF, IJ
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.True(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestDiv_rrrr_rrr_rrr_noCarry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            computer.CPU.FLAGS.SetCarry(true);
            Assert.True(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.ABCD = 0xFF;
            computer.CPU.REGS.EFG = 2;

            cp.Build(@"
                DIV ABCD, EFG, IJK
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.False(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestDiv_rrrr_rrr_rrr_carry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.ABCD = 0xFF;
            computer.CPU.REGS.EFG = 0;

            cp.Build(@"
                DIV ABCD, EFG, IJK
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.True(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestDiv_rrrr_rrrr_rrrr_noCarry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            computer.CPU.FLAGS.SetCarry(true);
            Assert.True(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.ABCD = 0xFF;
            computer.CPU.REGS.EFGH = 2;

            cp.Build(@"
                DIV ABCD, EFGH, IJKL
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.False(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestDiv_rrrr_rrrr_rrrr_carry()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.IsCarry());

            computer.CPU.REGS.ABCD = 0xFF;
            computer.CPU.REGS.EFG = 0;

            cp.Build(@"
                DIV ABCD, EFGH, IJKL
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.True(computer.CPU.FLAGS.IsCarry());
        }
    }
}
