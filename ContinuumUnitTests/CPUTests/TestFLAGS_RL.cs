using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace CPUTests
{

    public class TestFLAGS_RL
    {
        [Fact]
        public void TestFLAG_RL_r_n_CARRY()
        {
            Assembler cp = new();
            using Computer computer = new();


            cp.Build(@"
                LD A, 0b00011111
                RL A, 3
                JP NC, .OkNoCarry
                LD X, 1
                BREAK
            .OkNoCarry
                RL A, 1
                JP C, .OkCarry
                LD X, 2
                BREAK
            .OkCarry
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0, computer.CPU.REGS.X);
        }

        [Fact]
        public void TestFLAG_RL_r_r_CARRY()
        {
            Assembler cp = new();
            using Computer computer = new();


            cp.Build(@"
                LD A, 0b00011111
                LD B, 3
                LD C, 1
                RL A, B
                JP NC, .OkNoCarry
                LD X, 1
                BREAK
            .OkNoCarry
                RL A, C
                JP C, .OkCarry
                LD X, 2
                BREAK
            .OkCarry
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0, computer.CPU.REGS.X);
        }

        [Fact]
        public void TestFLAG_RL_rr_n_CARRY()
        {
            Assembler cp = new();
            using Computer computer = new();


            cp.Build(@"
                LD AB, 0b0000011100011111
                RL AB, 5
                JP NC, .OkNoCarry
                LD X, 1
                BREAK
            .OkNoCarry
                RL AB, 6
                JP C, .OkCarry
                LD X, 2
                BREAK
            .OkCarry
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0, computer.CPU.REGS.X);
        }

        [Fact]
        public void TestFLAG_RL_rr_r_CARRY()
        {
            Assembler cp = new();
            using Computer computer = new();


            cp.Build(@"
                LD AB, 0b0000011100011111
                LD C, 5
                LD D, 6
                RL AB, C
                JP NC, .OkNoCarry
                LD X, 1
                BREAK
            .OkNoCarry
                RL AB, D
                JP C, .OkCarry
                LD X, 2
                BREAK
            .OkCarry
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0, computer.CPU.REGS.X);
        }

        [Fact]
        public void TestFLAG_RL_rrr_n_CARRY()
        {
            Assembler cp = new();
            using Computer computer = new();


            cp.Build(@"
                LD ABC, 0b000000000001111100011111
                RL ABC, 10
                JP NC, .OkNoCarry
                LD X, 1
                BREAK
            .OkNoCarry
                RL ABC, 5
                JP C, .OkCarry
                LD X, 2
                BREAK
            .OkCarry
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0, computer.CPU.REGS.X);
        }

        [Fact]
        public void TestFLAG_RL_rrr_r_CARRY()
        {
            Assembler cp = new();
            using Computer computer = new();


            cp.Build(@"
                LD ABC, 0b000000000001111100011111
                LD D, 10
                LD E, 5
                RL ABC, D
                JP NC, .OkNoCarry
                LD X, 1
                BREAK
            .OkNoCarry
                RL ABC, E
                JP C, .OkCarry
                LD X, 2
                BREAK
            .OkCarry
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0, computer.CPU.REGS.X);
        }

        [Fact]
        public void TestFLAG_RL_rrrr_n_CARRY()
        {
            Assembler cp = new();
            using Computer computer = new();


            cp.Build(@"
                LD ABCD, 0b00000000000000011111000111111011
                RL ABCD, 14
                JP NC, .OkNoCarry
                LD X, 1
                BREAK
            .OkNoCarry
                RL ABCD, 24
                JP C, .OkCarry
                LD X, 2
                BREAK
            .OkCarry
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0, computer.CPU.REGS.X);
        }

        [Fact]
        public void TestFLAG_RL_rrrr_r_CARRY()
        {
            Assembler cp = new();
            using Computer computer = new();


            cp.Build(@"
                LD ABCD, 0b00000000000000011111000111111011
                LD E, 14
                LD F, 24
                RL ABCD, E
                JP NC, .OkNoCarry
                LD X, 1
                BREAK
            .OkNoCarry
                RL ABCD, F
                JP C, .OkCarry
                LD X, 2
                BREAK
            .OkCarry
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0, computer.CPU.REGS.X);
        }
    }
}
