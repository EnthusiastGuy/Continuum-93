using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace CPUTests
{

    public class TestFLAGS_RR
    {
        [Fact]
        public void TestFLAG_RR_r_n_CARRY()
        {
            Assembler cp = new();
            using Computer computer = new();


            cp.Build(@"
                LD A, 0b11111000
                RR A, 3
                JP NC, .OkNoCarry
                LD X, 1
                BREAK
            .OkNoCarry
                RR A, 1
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
        public void TestFLAG_RR_r_r_CARRY()
        {
            Assembler cp = new();
            using Computer computer = new();


            cp.Build(@"
                LD A, 0b11111000
                LD B, 3
                LD C, 1
                RR A, B
                JP NC, .OkNoCarry
                LD X, 1
                BREAK
            .OkNoCarry
                RR A, C
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
        public void TestFLAG_RR_rr_n_CARRY()
        {
            Assembler cp = new();
            using Computer computer = new();


            cp.Build(@"
                LD AB, 0b1111100011100000
                RR AB, 5
                JP NC, .OkNoCarry
                LD X, 1
                BREAK
            .OkNoCarry
                RR AB, 6
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
        public void TestFLAG_RR_rr_r_CARRY()
        {
            Assembler cp = new();
            using Computer computer = new();


            cp.Build(@"
                LD AB, 0b1111100011100000
                LD C, 5
                LD D, 6
                RR AB, C
                JP NC, .OkNoCarry
                LD X, 1
                BREAK
            .OkNoCarry
                RR AB, D
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
        public void TestFLAG_RR_rrr_n_CARRY()
        {
            Assembler cp = new();
            using Computer computer = new();


            cp.Build(@"
                LD ABC, 0b111110001111100000000000
                RR ABC, 10
                JP NC, .OkNoCarry
                LD X, 1
                BREAK
            .OkNoCarry
                RR ABC, 5
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
        public void TestFLAG_RR_rrr_r_CARRY()
        {
            Assembler cp = new();
            using Computer computer = new();


            cp.Build(@"
                LD ABC, 0b111110001111100000000000
                LD D, 10
                LD E, 5
                RR ABC, D
                JP NC, .OkNoCarry
                LD X, 1
                BREAK
            .OkNoCarry
                RR ABC, E
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
        public void TestFLAG_RR_rrrr_n_CARRY()
        {
            Assembler cp = new();
            using Computer computer = new();


            cp.Build(@"
                LD ABCD, 0b11011111100011111000000000000000
                RR ABCD, 14
                JP NC, .OkNoCarry
                LD X, 1
                BREAK
            .OkNoCarry
                RR ABCD, 24
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
        public void TestFLAG_RR_rrrr_r_CARRY()
        {
            Assembler cp = new();
            using Computer computer = new();


            cp.Build(@"
                LD ABCD, 0b11011111100011111000000000000000
                LD E, 14
                LD F, 24
                RR ABCD, E
                JP NC, .OkNoCarry
                LD X, 1
                BREAK
            .OkNoCarry
                RR ABCD, F
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
