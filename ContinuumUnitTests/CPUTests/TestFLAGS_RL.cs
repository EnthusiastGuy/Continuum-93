using System;
using Continuum93.Emulator;
using Continuum93.Emulator.Interpreter;
using Xunit;

namespace CPUTests
{
    public sealed class TestFLAGS_RL
    {
        private static void RunTest(string asm, Action<Computer>? arrange, Action<Computer> assert)
        {
            Assembler cp = new();
            using Computer computer = new();

            arrange?.Invoke(computer);

            cp.Build(asm.TrimEnd() + "\nBREAK\n");
            if (cp.Errors > 0)
                throw new InvalidOperationException($"Assembly failed: {cp.Log}");

            computer.LoadMem(cp.GetCompiledCode());
            computer.Run();

            assert(computer);
        }

        private static void AssertXIsZero(Computer c) => Assert.Equal(0, c.CPU.REGS.X);

        // -------------------------
        // r, n
        // -------------------------

        [Fact]
        public void FLAG_RL_r_n_OkNoCarry()
        {
            RunTest(@"
                LD A, 0b00011111
                RL A, 3
                JP NC, .OkNoCarry
                LD X, 1
                BREAK
            .OkNoCarry
                BREAK
            ", null, AssertXIsZero);
        }

        [Fact]
        public void FLAG_RL_r_n_OkCarry()
        {
            RunTest(@"
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
            ", null, AssertXIsZero);
        }

        // -------------------------
        // r, r
        // -------------------------

        [Fact]
        public void FLAG_RL_r_r_OkNoCarry()
        {
            RunTest(@"
                LD A, 0b00011111
                LD B, 3

                RL A, B
                JP NC, .OkNoCarry
                LD X, 1
                BREAK

            .OkNoCarry
                BREAK
            ", null, AssertXIsZero);
        }

        [Fact]
        public void FLAG_RL_r_r_OkCarry()
        {
            RunTest(@"
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
            ", null, AssertXIsZero);
        }

        // -------------------------
        // rr, n
        // -------------------------

        [Fact]
        public void FLAG_RL_rr_n_OkNoCarry()
        {
            RunTest(@"
                LD AB, 0b0000011100011111

                RL AB, 5
                JP NC, .OkNoCarry
                LD X, 1
                BREAK

            .OkNoCarry
                BREAK
            ", null, AssertXIsZero);
        }

        [Fact]
        public void FLAG_RL_rr_n_OkCarry()
        {
            RunTest(@"
                ; Force carry: MSB is 1, rotate left by 1 -> MSB rotates out -> Carry must be set
                LD AB, 0b1000000000000000
                RL AB, 1
                JP C, .OkCarry
                LD X, 1
                BREAK
            .OkCarry
                BREAK
            ", null, AssertXIsZero);
        }


        // -------------------------
        // rr, r
        // -------------------------

        [Fact]
        public void FLAG_RL_rr_r_OkNoCarry()
        {
            RunTest(@"
                LD AB, 0b0000011100011111
                LD C, 5

                RL AB, C
                JP NC, .OkNoCarry
                LD X, 1
                BREAK

            .OkNoCarry
                BREAK
            ", null, AssertXIsZero);
        }

        [Fact]
        public void FLAG_RL_rr_r_OkCarry()
        {
            RunTest(@"
                LD AB, 0b1000000000000000
                LD C, 1
                RL AB, C
                JP C, .OkCarry
                LD X, 1
                BREAK
            .OkCarry
                BREAK
            ", null, AssertXIsZero);
        }



        // -------------------------
        // rrr, n
        // -------------------------

        [Fact]
        public void FLAG_RL_rrr_n_OkNoCarry()
        {
            RunTest(@"
                LD ABC, 0b000000000001111100011111

                RL ABC, 10
                JP NC, .OkNoCarry
                LD X, 1
                BREAK

            .OkNoCarry
                BREAK
            ", null, AssertXIsZero);
        }

        [Fact]
        public void FLAG_RL_rrr_n_OkCarry()
        {
            RunTest(@"
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
            ", null, AssertXIsZero);
        }

        // -------------------------
        // rrr, r
        // -------------------------

        [Fact]
        public void FLAG_RL_rrr_r_OkNoCarry()
        {
            RunTest(@"
                LD ABC, 0b000000000001111100011111
                LD D, 10

                RL ABC, D
                JP NC, .OkNoCarry
                LD X, 1
                BREAK

            .OkNoCarry
                BREAK
            ", null, AssertXIsZero);
        }

        [Fact]
        public void FLAG_RL_rrr_r_OkCarry()
        {
            RunTest(@"
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
            ", null, AssertXIsZero);
        }

        // -------------------------
        // rrrr, n
        // -------------------------

        [Fact]
        public void FLAG_RL_rrrr_n_OkNoCarry()
        {
            RunTest(@"
                LD ABCD, 0b00000000000000011111000111111011

                RL ABCD, 14
                JP NC, .OkNoCarry
                LD X, 1
                BREAK

            .OkNoCarry
                BREAK
            ", null, AssertXIsZero);
        }

        [Fact]
        public void FLAG_RL_rrrr_n_OkCarry()
        {
            RunTest(@"
                ; Force carry on 32-bit: MSB is 1
                LD ABCD, 0b10000000000000000000000000000000
                RL ABCD, 1
                JP C, .OkCarry
                LD X, 1
                BREAK
            .OkCarry
                BREAK
            ", null, AssertXIsZero);
        }


        // -------------------------
        // rrrr, r
        // -------------------------

        [Fact]
        public void FLAG_RL_rrrr_r_OkNoCarry()
        {
            RunTest(@"
                LD ABCD, 0b00000000000000011111000111111011
                LD E, 14

                RL ABCD, E
                JP NC, .OkNoCarry
                LD X, 1
                BREAK

            .OkNoCarry
                BREAK
            ", null, AssertXIsZero);
        }

        [Fact]
        public void FLAG_RL_rrrr_r_OkCarry()
        {
            RunTest(@"
                LD ABCD, 0b10000000000000000000000000000000
                LD E, 1
                RL ABCD, E
                JP C, .OkCarry
                LD X, 1
                BREAK
            .OkCarry
                BREAK
            ", null, AssertXIsZero);
        }

    }
}
