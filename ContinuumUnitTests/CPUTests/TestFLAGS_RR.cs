using System;
using Continuum93.Emulator;
using Continuum93.Emulator.Interpreter;
using Xunit;

namespace CPUTests
{
    public sealed class TestFLAGS_RR
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

        // Convention:
        // - On failure, set X to a non-zero code.
        // - Success leaves X == 0.
        private static void AssertXIsZero(Computer c) => Assert.Equal(0, c.CPU.REGS.X);

        // -------------------------
        // r, n
        // -------------------------

        [Fact]
        public void FLAG_RR_r_n_OkNoCarry()
        {
            RunTest(@"
                LD A, 0b11111000
                RR A, 3
                JP NC, .OkNoCarry
                LD X, 1
                BREAK
            .OkNoCarry
                BREAK
            ", null, AssertXIsZero);
        }

        [Fact]
        public void FLAG_RR_r_n_OkCarry()
        {
            RunTest(@"
                ; Force carry in one step: LSB is 1, rotate right by 1 -> Carry must be set
                LD A, 0b00000001
                RR A, 1
                JP C, .OkCarry
                LD X, 1
                BREAK
            .OkCarry
                BREAK
            ", null, AssertXIsZero);
        }

        // -------------------------
        // r, r
        // -------------------------

        [Fact]
        public void FLAG_RR_r_r_OkNoCarry()
        {
            RunTest(@"
                LD A, 0b11111000
                LD B, 3

                RR A, B
                JP NC, .OkNoCarry
                LD X, 1
                BREAK

            .OkNoCarry
                BREAK
            ", null, AssertXIsZero);
        }

        [Fact]
        public void FLAG_RR_r_r_OkCarry()
        {
            RunTest(@"
                LD A, 0b00000001
                LD B, 1

                RR A, B
                JP C, .OkCarry
                LD X, 1
                BREAK

            .OkCarry
                BREAK
            ", null, AssertXIsZero);
        }

        // -------------------------
        // rr, n
        // -------------------------

        [Fact]
        public void FLAG_RR_rr_n_OkNoCarry()
        {
            RunTest(@"
                LD AB, 0b1111100011100000

                RR AB, 5
                JP NC, .OkNoCarry
                LD X, 1
                BREAK

            .OkNoCarry
                BREAK
            ", null, AssertXIsZero);
        }

        [Fact]
        public void FLAG_RR_rr_n_OkCarry()
        {
            RunTest(@"
                ; Force carry in one step: bit0 is 1
                LD AB, 0b0000000000000001
                RR AB, 1
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
        public void FLAG_RR_rr_r_OkNoCarry()
        {
            RunTest(@"
                LD AB, 0b1111100011100000
                LD C, 5

                RR AB, C
                JP NC, .OkNoCarry
                LD X, 1
                BREAK

            .OkNoCarry
                BREAK
            ", null, AssertXIsZero);
        }

        [Fact]
        public void FLAG_RR_rr_r_OkCarry()
        {
            RunTest(@"
                LD AB, 0b0000000000000001
                LD C, 1

                RR AB, C
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
        public void FLAG_RR_rrr_n_OkNoCarry()
        {
            RunTest(@"
                LD ABC, 0b111110001111100000000000

                RR ABC, 10
                JP NC, .OkNoCarry
                LD X, 1
                BREAK

            .OkNoCarry
                BREAK
            ", null, AssertXIsZero);
        }

        [Fact]
        public void FLAG_RR_rrr_n_OkCarry()
        {
            RunTest(@"
                ; Force carry in one step: bit0 is 1 (24-bit)
                LD ABC, 0b000000000000000000000001
                RR ABC, 1
                JP C, .OkCarry
                LD X, 1
                BREAK
            .OkCarry
                BREAK
            ", null, AssertXIsZero);
        }

        // -------------------------
        // rrr, r
        // -------------------------

        [Fact]
        public void FLAG_RR_rrr_r_OkNoCarry()
        {
            RunTest(@"
                LD ABC, 0b111110001111100000000000
                LD D, 10

                RR ABC, D
                JP NC, .OkNoCarry
                LD X, 1
                BREAK

            .OkNoCarry
                BREAK
            ", null, AssertXIsZero);
        }

        [Fact]
        public void FLAG_RR_rrr_r_OkCarry()
        {
            RunTest(@"
                LD ABC, 0b000000000000000000000001
                LD D, 1

                RR ABC, D
                JP C, .OkCarry
                LD X, 1
                BREAK

            .OkCarry
                BREAK
            ", null, AssertXIsZero);
        }

        // -------------------------
        // rrrr, n
        // -------------------------

        [Fact]
        public void FLAG_RR_rrrr_n_OkNoCarry()
        {
            RunTest(@"
                LD ABCD, 0b11011111100011111000000000000000

                RR ABCD, 14
                JP NC, .OkNoCarry
                LD X, 1
                BREAK

            .OkNoCarry
                BREAK
            ", null, AssertXIsZero);
        }

        [Fact]
        public void FLAG_RR_rrrr_n_OkCarry()
        {
            RunTest(@"
                ; Force carry in one step: bit0 is 1 (32-bit)
                LD ABCD, 0b00000000000000000000000000000001
                RR ABCD, 1
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
        public void FLAG_RR_rrrr_r_OkNoCarry()
        {
            RunTest(@"
                LD ABCD, 0b11011111100011111000000000000000
                LD E, 14

                RR ABCD, E
                JP NC, .OkNoCarry
                LD X, 1
                BREAK

            .OkNoCarry
                BREAK
            ", null, AssertXIsZero);
        }

        [Fact]
        public void FLAG_RR_rrrr_r_OkCarry()
        {
            RunTest(@"
                LD ABCD, 0b00000000000000000000000000000001
                LD E, 1

                RR ABCD, E
                JP C, .OkCarry
                LD X, 1
                BREAK

            .OkCarry
                BREAK
            ", null, AssertXIsZero);
        }
    }
}
