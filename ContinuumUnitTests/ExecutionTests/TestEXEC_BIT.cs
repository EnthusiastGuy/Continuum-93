using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{

    public class TestEXEC_BIT
    {
        [Fact]
        public void TestEXEC_RES_r_n()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.A = 0b00000001;
            computer.CPU.REGS.B = 0b00000001;


            cp.Build(@"
                    BIT A, 0
                    LD A, 1
                    JP Z, .Aset
                    LD A, 0
                .Aset
                    BIT B, 7
                    LD B, 1
                    JP Z, .Bset
                    LD B, 0
                .Bset
                    BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(1, computer.CPU.REGS.A);
            Assert.Equal(0, computer.CPU.REGS.B);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_RES_r_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.A = 0b00001111;
            computer.CPU.REGS.X = 3;
            computer.CPU.REGS.B = 0b00001111;
            computer.CPU.REGS.Y = 5;

            cp.Build(@"
                    BIT A, X
                    LD A, 1
                    JP Z, .Aset
                    LD A, 0
                .Aset
                    BIT B, Y
                    LD B, 1
                    JP Z, .Bset
                    LD B, 0
                .Bset
                    BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(1, computer.CPU.REGS.A);
            Assert.Equal(0, computer.CPU.REGS.B);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_RES_rr_n()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.AB = 0b0000001000000000;
            computer.CPU.REGS.CD = 0b1101111111111111;

            cp.Build(@"
					BIT AB, 9
					LD A, 1
					JP Z, .Aset
					LD A, 0
				.Aset
					BIT BC, 13
					LD B, 1
					JP Z, .Bset
					LD B, 0
				.Bset
					BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(1, computer.CPU.REGS.A);
            Assert.Equal(0, computer.CPU.REGS.B);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_RES_rr_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.AB = 0b0000001000000000;
            computer.CPU.REGS.X = 9;
            computer.CPU.REGS.CD = 0b1101111111111111;
            computer.CPU.REGS.Y = 13;

            cp.Build(@"
                    BIT AB, X
                    LD A, 1
                    JP Z, .Aset
                    LD A, 0
                .Aset
                    BIT BC, Y
                    LD B, 1
                    JP Z, .Bset
                    LD B, 0
                .Bset
                    BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(1, computer.CPU.REGS.A);
            Assert.Equal(0, computer.CPU.REGS.B);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_RES_rrr_n()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABC = 0b000000010000000000000000;
            computer.CPU.REGS.DEF = 0b110111111111111111111111;

            cp.Build(@"
                    BIT ABC, 16
                    LD A, 1
                    JP Z, .Aset
                    LD A, 0
                .Aset
                    BIT DEF, 21
                    LD B, 1
                    JP Z, .Bset
                    LD B, 0
                .Bset
                    BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(1, computer.CPU.REGS.A);
            Assert.Equal(0, computer.CPU.REGS.B);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_RES_rrr_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABC = 0b000000010000000000000000;
            computer.CPU.REGS.X = 16;
            computer.CPU.REGS.DEF = 0b110111111111111111111111;
            computer.CPU.REGS.Y = 21;

            cp.Build(@"
                    BIT ABC, X
                    LD A, 1
                    JP Z, .Aset
                    LD A, 0
                .Aset
                    BIT DEF, Y
                    LD B, 1
                    JP Z, .Bset
                    LD B, 0
                .Bset
                    BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(1, computer.CPU.REGS.A);
            Assert.Equal(0, computer.CPU.REGS.B);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_RES_rrrr_n()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABCD = 0b00000001000000000000000000000000;
            computer.CPU.REGS.EFGH = 0b11011111111111111111111111111111;

            cp.Build(@"
            		BIT ABCD, 24
					LD A, 1
					JP Z, .Aset
					LD A, 0
				.Aset
					BIT EFGH, 29
					LD B, 1
					JP Z, .Bset
					LD B, 0
				.Bset
					BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(1, computer.CPU.REGS.A);
            Assert.Equal(0, computer.CPU.REGS.B);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_RES_rrrr_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABCD = 0b00000001000000000000000000000000;
            computer.CPU.REGS.X = 24;
            computer.CPU.REGS.EFGH = 0b11011111111111111111111111111111;
            computer.CPU.REGS.Y = 29;

            cp.Build(@"
                    BIT ABCD, X
                    LD A, 1
                    JP Z, .Aset
                    LD A, 0
                .Aset
                    BIT EFGH, Y
                    LD B, 1
                    JP Z, .Bset
                    LD B, 0
                .Bset
                    BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(1, computer.CPU.REGS.A);
            Assert.Equal(0, computer.CPU.REGS.B);
            TUtils.IncrementCountedTests("exec");
        }
    }
}
