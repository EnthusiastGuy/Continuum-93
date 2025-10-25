using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace CPUTests
{
    public class TestFLAGS_MUL
    {
        // MUL r
        [Fact]
        public void TestFLAG_MUL_r_n_noCarry()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.SetCarry(true);

            Assert.True(computer.CPU.FLAGS.IsCarry());

            cp.Build(@"
                LD A, 0x08
                MUL A, 0x0A ; Should not set carry
                BREAK

            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.False(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestFLAG_MUL_r_n_carry()
        {
            Assembler cp = new();
            using Computer computer = new();

            Assert.False(computer.CPU.FLAGS.IsCarry());

            cp.Build(@"
                LD A, 0x80
                MUL A, 0x0A ; Should set carry
                BREAK

            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.True(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestFLAG_MUL_r_r_noCarry()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.SetCarry(true);

            Assert.True(computer.CPU.FLAGS.IsCarry());

            cp.Build(@"
                LD A, 0x08
                LD B, 0x0A
                MUL A, B ; Should not set carry
                BREAK

            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.False(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestFLAG_MUL_r_r_carry()
        {
            Assembler cp = new();
            using Computer computer = new();

            Assert.False(computer.CPU.FLAGS.IsCarry());

            cp.Build(@"
                LD A, 0x80
                LD B, 0x0A
                MUL A, B ; Should set carry
                BREAK

            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.True(computer.CPU.FLAGS.IsCarry());
        }

        // MUL rr
        [Fact]
        public void TestFLAG_MUL_rr_n_noCarry()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.SetCarry(true);

            Assert.True(computer.CPU.FLAGS.IsCarry());

            cp.Build(@"
                LD AB, 0x00FF
                MUL AB, 0x0A ; Should not set carry
                BREAK

            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.False(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestFLAG_MUL_rr_r_noCarry()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.SetCarry(true);

            Assert.True(computer.CPU.FLAGS.IsCarry());

            cp.Build(@"
                LD AB, 0x00FF
                LD C, 0x0A
                MUL AB, C ; Should not set carry
                BREAK

            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.False(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestFLAG_MUL_rr_rr_noCarry()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.SetCarry(true);

            Assert.True(computer.CPU.FLAGS.IsCarry());

            cp.Build(@"
                LD AB, 0x00FF
                LD CD, 0x0A
                MUL AB, CD ; Should not set carry
                BREAK

            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.False(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestFLAG_MUL_rr_n_carry()
        {
            Assembler cp = new();
            using Computer computer = new();

            Assert.False(computer.CPU.FLAGS.IsCarry());

            cp.Build(@"
                LD AB, 0x8000
                MUL AB, 0x0A ; Should set carry
                BREAK

            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.True(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestFLAG_MUL_rr_r_carry()
        {
            Assembler cp = new();
            using Computer computer = new();

            Assert.False(computer.CPU.FLAGS.IsCarry());

            cp.Build(@"
                LD AB, 0x8000
                LD C, 0x0A
                MUL AB, C ; Should set carry
                BREAK

            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.True(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestFLAG_MUL_rr_rr_carry()
        {
            Assembler cp = new();
            using Computer computer = new();

            Assert.False(computer.CPU.FLAGS.IsCarry());

            cp.Build(@"
                LD AB, 0x8000
                LD CD, 0x0A
                MUL AB, CD ; Should set carry
                BREAK

            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.True(computer.CPU.FLAGS.IsCarry());
        }

        // MUL rrr
        [Fact]
        public void TestFLAG_MUL_rrr_n_noCarry()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.SetCarry(true);

            Assert.True(computer.CPU.FLAGS.IsCarry());

            cp.Build(@"
                LD ABC, 0x00FFFF
                MUL ABC, 0x0A ; Should not set carry
                BREAK

            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.False(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestFLAG_MUL_rrr_r_noCarry()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.SetCarry(true);

            Assert.True(computer.CPU.FLAGS.IsCarry());

            cp.Build(@"
                LD ABC, 0x00FFFF
                LD D, 0x0A
                MUL ABC, D ; Should not set carry
                BREAK

            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.False(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestFLAG_MUL_rrr_rr_noCarry()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.SetCarry(true);

            Assert.True(computer.CPU.FLAGS.IsCarry());

            cp.Build(@"
                LD ABC, 0x00FFFF
                LD DE, 0x0A
                MUL ABC, DE ; Should not set carry
                BREAK

            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.False(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestFLAG_MUL_rrr_rrr_noCarry()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.SetCarry(true);

            Assert.True(computer.CPU.FLAGS.IsCarry());

            cp.Build(@"
                LD ABC, 0x00FFFF
                LD DEF, 0x0A
                MUL ABC, DEF ; Should not set carry
                BREAK

            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.False(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestFLAG_MUL_rrr_n_carry()
        {
            Assembler cp = new();
            using Computer computer = new();

            Assert.False(computer.CPU.FLAGS.IsCarry());

            cp.Build(@"
                LD ABC, 0x800000
                MUL ABC, 0x0A ; Should set carry
                BREAK

            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.True(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestFLAG_MUL_rrr_r_carry()
        {
            Assembler cp = new();
            using Computer computer = new();

            Assert.False(computer.CPU.FLAGS.IsCarry());

            cp.Build(@"
                LD ABC, 0x800000
                LD D, 0x0A
                MUL ABC, D ; Should set carry
                BREAK

            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.True(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestFLAG_MUL_rrr_rr_carry()
        {
            Assembler cp = new();
            using Computer computer = new();

            Assert.False(computer.CPU.FLAGS.IsCarry());

            cp.Build(@"
                LD ABC, 0x800000
                LD DE, 0x0A
                MUL ABC, DE ; Should set carry
                BREAK

            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.True(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestFLAG_MUL_rrr_rrr_carry()
        {
            Assembler cp = new();
            using Computer computer = new();

            Assert.False(computer.CPU.FLAGS.IsCarry());

            cp.Build(@"
                LD ABC, 0x800000
                LD DEF, 0x0A
                MUL ABC, DEF ; Should set carry
                BREAK

            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.True(computer.CPU.FLAGS.IsCarry());
        }

        // MUL rrrr
        [Fact]
        public void TestFLAG_MUL_rrrr_n_noCarry()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.SetCarry(true);

            Assert.True(computer.CPU.FLAGS.IsCarry());

            cp.Build(@"
                LD ABCD, 0x00FFFFFF
                MUL ABCD, 0x0A ; Should not set carry
                BREAK

            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.False(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestFLAG_MUL_rrrr_r_noCarry()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.SetCarry(true);

            Assert.True(computer.CPU.FLAGS.IsCarry());

            cp.Build(@"
                LD ABCD, 0x00FFFFFF
                LD E, 0x0A
                MUL ABCD, E ; Should not set carry
                BREAK

            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.False(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestFLAG_MUL_rrrr_rr_noCarry()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.SetCarry(true);

            Assert.True(computer.CPU.FLAGS.IsCarry());

            cp.Build(@"
                LD ABCD, 0x00FFFFFF
                LD EF, 0x0A
                MUL ABCD, EF ; Should not set carry
                BREAK

            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.False(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestFLAG_MUL_rrrr_rrr_noCarry()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.SetCarry(true);

            Assert.True(computer.CPU.FLAGS.IsCarry());

            cp.Build(@"
                LD ABCD, 0x00FFFFFF
                LD EFG, 0x0A
                MUL ABCD, EFG ; Should not set carry
                BREAK

            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.False(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestFLAG_MUL_rrrr_rrrr_noCarry()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.SetCarry(true);

            Assert.True(computer.CPU.FLAGS.IsCarry());

            cp.Build(@"
                LD ABCD, 0x00FFFFFF
                LD EFGH, 0x0A
                MUL ABCD, EFGH ; Should not set carry
                BREAK

            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.False(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestFLAG_MUL_rrrr_n_carry()
        {
            Assembler cp = new();
            using Computer computer = new();

            Assert.False(computer.CPU.FLAGS.IsCarry());

            cp.Build(@"
                LD ABCD, 0x80000000
                MUL ABCD, 0x0A ; Should set carry
                BREAK

            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.True(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestFLAG_MUL_rrrr_r_carry()
        {
            Assembler cp = new();
            using Computer computer = new();

            Assert.False(computer.CPU.FLAGS.IsCarry());

            cp.Build(@"
                LD ABCD, 0x80000000
                LD E, 0x0A
                MUL ABCD, E ; Should set carry
                BREAK

            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.True(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestFLAG_MUL_rrrr_rr_carry()
        {
            Assembler cp = new();
            using Computer computer = new();

            Assert.False(computer.CPU.FLAGS.IsCarry());

            cp.Build(@"
                LD ABCD, 0x80000000
                LD EF, 0x0A
                MUL ABCD, EF ; Should set carry
                BREAK

            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.True(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestFLAG_MUL_rrrr_rrr_carry()
        {
            Assembler cp = new();
            using Computer computer = new();

            Assert.False(computer.CPU.FLAGS.IsCarry());

            cp.Build(@"
                LD ABCD, 0x80000000
                LD EFG, 0x0A
                MUL ABCD, EFG ; Should set carry
                BREAK

            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.True(computer.CPU.FLAGS.IsCarry());
        }

        [Fact]
        public void TestFLAG_MUL_rrrr_rrrr_carry()
        {
            Assembler cp = new();
            using Computer computer = new();

            Assert.False(computer.CPU.FLAGS.IsCarry());

            cp.Build(@"
                LD ABCD, 0x80000000
                LD EFGH, 0x0A
                MUL ABCD, EFGH ; Should set carry
                BREAK

            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.True(computer.CPU.FLAGS.IsCarry());
        }

    }
}
