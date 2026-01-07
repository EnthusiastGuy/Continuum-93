using Continuum93.Emulator;
using Continuum93.Emulator.Interpreter;
using Continuum93.Tools;

namespace ExecutionTests
{

    public class TestEXEC_RL
    {
        #region Helper Methods

        private static void RunTest(string asm, Action<Computer> arrange, Action<Computer> assert)
        {
            Assembler cp = new();
            using Computer computer = new();

            arrange(computer);

            cp.Build(TUtils.GetFormattedAsm(asm, "BREAK"));
            if (cp.Errors > 0)
                throw new InvalidOperationException($"Assembly failed: {cp.Log}");

            computer.LoadMem(cp.GetCompiledCode());
            computer.Run();

            assert(computer);
            TUtils.IncrementCountedTests("exec");
        }

        private static byte SL8(byte v, int n) => (byte)(v << (n & 7));
        private static ushort SL16(ushort v, int n) => (ushort)(v << (n & 15));
        private static uint SL24(uint v, int n) => ((v << (n % 24)) & 0xFFFFFFu);
        private static uint SL32(uint v, int n) => (v << (n & 31));

        private static float ShiftFloatBits(float f, int n)
        {
            uint bits = FloatPointUtils.FloatToUint(f);
            bits <<= (n & 31);
            return FloatPointUtils.UintToFloat(bits);
        }

        #endregion
        #region SL r,* tests

        [Fact]
        public void RL_r_n() // Ok
        {
            RunTest(
                "RL A,4",
                c => c.CPU.REGS.A = 0x0F,
                c => Assert.Equal((byte)0xF0, c.CPU.REGS.A));
        }

        [Fact]
        public void RL_r_r() // Ok
        {
            RunTest(
                "RL A,B",
                c =>
                {
                    c.CPU.REGS.A = 0x0F;
                    c.CPU.REGS.B = 4;
                },
                c => Assert.Equal((byte)0xF0, c.CPU.REGS.A));
        }

        [Fact]
        public void RL_r_InnnI() // Ok
        {
            RunTest(
                "RL A,(0x4000)",
                c =>
                {
                    c.CPU.REGS.A = 0x0F;
                    c.MEMC.Set8bitToRAM(0x4000, 4);
                },
                c => Assert.Equal((byte)0xF0, c.CPU.REGS.A));
        }

        [Fact]
        public void RL_r_Innn_nnnI() // Ok
        {
            RunTest(
                "RL A,(0x4000+4)",
                c =>
                {
                    c.CPU.REGS.A = 0x0F;
                    c.MEMC.Set8bitToRAM(0x4004, 4);
                },
                c => Assert.Equal((byte)0xF0, c.CPU.REGS.A));
        }

        [Fact]
        public void RL_r_Innn_rI() // Ok
        {
            RunTest(
                "RL A,(0x4000+X)",
                c =>
                {
                    c.CPU.REGS.A = 0x0F;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set8bitToRAM(0x4003, 4);
                },
                c => Assert.Equal((byte)0xF0, c.CPU.REGS.A));
        }

        [Fact]
        public void RL_r_Innn_rrI() // Ok
        {
            RunTest(
                "RL A,(0x4000+WX)",
                c =>
                {
                    c.CPU.REGS.A = 0x0F;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x4005, 4);
                },
                c => Assert.Equal((byte)0xF0, c.CPU.REGS.A));
        }

        [Fact]
        public void RL_r_Innn_rrrI() // Ok
        {
            RunTest(
                "RL A,(0x4000+VWX)",
                c =>
                {
                    c.CPU.REGS.A = 0x0F;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x4007, 4);
                },
                c => Assert.Equal((byte)0xF0, c.CPU.REGS.A));
        }

        [Fact]
        public void RL_r_IrrrI() // Ok
        {
            RunTest(
                "RL A,(QRS)",
                c =>
                {
                    c.CPU.REGS.A = 0x0F;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x5000, 4);
                },
                c => Assert.Equal((byte)0xF0, c.CPU.REGS.A));
        }

        [Fact]
        public void RL_r_Irrr_nnnI() // Ok
        {
            RunTest(
                "RL A,(QRS+4)",
                c =>
                {
                    c.CPU.REGS.A = 0x0F;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x5004, 4);
                },
                c => Assert.Equal((byte)0xF0, c.CPU.REGS.A));
        }

        [Fact]
        public void RL_r_Irrr_rI() // Ok
        {
            RunTest(
                "RL A,(QRS+X)",
                c =>
                {
                    c.CPU.REGS.A = 0x0F;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set8bitToRAM(0x5003, 4);
                },
                c => Assert.Equal((byte)0xF0, c.CPU.REGS.A));
        }

        [Fact]
        public void RL_r_Irrr_rrI() // Ok
        {
            RunTest(
                "RL A,(QRS+WX)",
                c =>
                {
                    c.CPU.REGS.A = 0x0F;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x5005, 4);
                },
                c => Assert.Equal((byte)0xF0, c.CPU.REGS.A));
        }

        [Fact]
        public void RL_r_Irrr_rrrI() // Ok
        {
            RunTest(
                "RL A,(QRS+VWX)",
                c =>
                {
                    c.CPU.REGS.A = 0x0F;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x5007, 4);
                },
                c => Assert.Equal((byte)0xF0, c.CPU.REGS.A));
        }

        [Fact]
        public void RL_r_fr() // Ok
        {
            RunTest(
                "RL A,F0",
                c =>
                {
                    c.CPU.REGS.A = 0x0F;
                    c.CPU.FREGS.SetRegister(0, 4.0f);
                },
                c => Assert.Equal((byte)0xF0, c.CPU.REGS.A));
        }

        #endregion

        #region SL rr,* tests

        [Fact]
        public void RL_rr_nn() // Ok
        {
            RunTest(
                "RL AB,0x0004",
                c => c.CPU.REGS.AB = 0x00FF,
                c => Assert.Equal((ushort)0x0FF0, c.CPU.REGS.AB));
        }

        [Fact]
        public void RL_rr_r() // Ok
        {
            RunTest(
                "RL AB,E",
                c =>
                {
                    c.CPU.REGS.AB = 0x00FF;
                    c.CPU.REGS.E = 4;
                },
                c => Assert.Equal((ushort)0x0FF0, c.CPU.REGS.AB));
        }

        [Fact]
        public void RL_rr_rr() // Ok
        {
            RunTest(
                "RL AB,CD",
                c =>
                {
                    c.CPU.REGS.AB = 0x00FF;
                    c.CPU.REGS.CD = 4;
                },
                c => Assert.Equal((ushort)0x0FF0, c.CPU.REGS.AB));
        }

        [Fact]
        public void RL_rr_InnnI() // Ok
        {
            RunTest(
                "RL AB,(0x4000)",
                c =>
                {
                    c.CPU.REGS.AB = 0x00FF;
                    c.MEMC.Set8bitToRAM(0x4000, 4);
                },
                c => Assert.Equal((ushort)0x0FF0, c.CPU.REGS.AB));
        }

        [Fact]
        public void RL_rr_Innn_nnnI() // Ok
        {
            RunTest(
                "RL AB,(0x4000+4)",
                c =>
                {
                    c.CPU.REGS.AB = 0x00FF;
                    c.MEMC.Set8bitToRAM(0x4004, 4);
                },
                c => Assert.Equal((ushort)0x0FF0, c.CPU.REGS.AB));
        }

        [Fact]
        public void RL_rr_Innn_rI() // Ok
        {
            RunTest(
                "RL AB,(0x4000+X)",
                c =>
                {
                    c.CPU.REGS.AB = 0x00FF;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set8bitToRAM(0x4003, 4);
                },
                c => Assert.Equal((ushort)0x0FF0, c.CPU.REGS.AB));
        }

        [Fact]
        public void RL_rr_Innn_rrI() // Ok
        {
            RunTest(
                "RL AB,(0x4000+WX)",
                c =>
                {
                    c.CPU.REGS.AB = 0x00FF;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x4005, 4);
                },
                c => Assert.Equal((ushort)0x0FF0, c.CPU.REGS.AB));
        }

        [Fact]
        public void RL_rr_Innn_rrrI() // Ok
        {
            RunTest(
                "RL AB,(0x4000+VWX)",
                c =>
                {
                    c.CPU.REGS.AB = 0x00FF;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x4007, 4);
                },
                c => Assert.Equal((ushort)0x0FF0, c.CPU.REGS.AB));
        }

        [Fact]
        public void RL_rr_IrrrI() // Ok
        {
            RunTest(
                "RL AB,(QRS)",
                c =>
                {
                    c.CPU.REGS.AB = 0x00FF;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x5000, 4);
                },
                c => Assert.Equal((ushort)0x0FF0, c.CPU.REGS.AB));
        }

        [Fact]
        public void RL_rr_Irrr_nnnI() // Ok
        {
            RunTest(
                "RL AB,(QRS+4)",
                c =>
                {
                    c.CPU.REGS.AB = 0x00FF;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x5004, 4);
                },
                c => Assert.Equal((ushort)0x0FF0, c.CPU.REGS.AB));
        }

        [Fact]
        public void RL_rr_Irrr_rI() // Ok
        {
            RunTest(
                "RL AB,(QRS+X)",
                c =>
                {
                    c.CPU.REGS.AB = 0x00FF;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set8bitToRAM(0x5003, 4);
                },
                c => Assert.Equal((ushort)0x0FF0, c.CPU.REGS.AB));
        }

        [Fact]
        public void RL_rr_Irrr_rrI() // Ok
        {
            RunTest(
                "RL AB,(QRS+WX)",
                c =>
                {
                    c.CPU.REGS.AB = 0x00FF;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x5005, 4);
                },
                c => Assert.Equal((ushort)0x0FF0, c.CPU.REGS.AB));
        }

        [Fact]
        public void RL_rr_Irrr_rrrI() // Ok
        {
            RunTest(
                "RL AB,(QRS+VWX)",
                c =>
                {
                    c.CPU.REGS.AB = 0x00FF;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x5007, 4);
                },
                c => Assert.Equal((ushort)0x0FF0, c.CPU.REGS.AB));
        }

        [Fact]
        public void RL_rr_fr() // Ok
        {
            RunTest(
                "RL AB,F1",
                c =>
                {
                    c.CPU.REGS.AB = 0x00FF;
                    c.CPU.FREGS.SetRegister(1, 4.0f);
                },
                c => Assert.Equal((ushort)0x0FF0, c.CPU.REGS.AB));
        }

        #endregion

        #region SL rrr,* tests

        [Fact]
        public void RL_rrr_nnn() // Ok
        {
            RunTest(
                "RL ABC,0x000004",
                c => c.CPU.REGS.ABC = 0x0000FF,
                c => Assert.Equal((uint)0x000FF0, c.CPU.REGS.ABC));
        }

        [Fact]
        public void RL_rrr_r() // Ok
        {
            RunTest(
                "RL ABC,D",
                c =>
                {
                    c.CPU.REGS.ABC = 0x0000FF;
                    c.CPU.REGS.D = 4;
                },
                c => Assert.Equal((uint)0x000FF0, c.CPU.REGS.ABC));
        }

        [Fact]
        public void RL_rrr_rr() // Ok
        {
            RunTest(
                "RL ABC,DE",
                c =>
                {
                    c.CPU.REGS.ABC = 0x0000FF;
                    c.CPU.REGS.DE = 4;
                },
                c => Assert.Equal((uint)0x000FF0, c.CPU.REGS.ABC));
        }

        [Fact]
        public void RL_rrr_rrr() // Ok
        {
            RunTest(
                "RL ABC,DEF",
                c =>
                {
                    c.CPU.REGS.ABC = 0x0000FF;
                    c.CPU.REGS.DEF = 4;
                },
                c => Assert.Equal((uint)0x000FF0, c.CPU.REGS.ABC));
        }

        [Fact]
        public void RL_rrr_InnnI() // Ok
        {
            RunTest(
                "RL ABC,(0x4000)",
                c =>
                {
                    c.CPU.REGS.ABC = 0x0000FF;
                    c.MEMC.Set8bitToRAM(0x4000, 4);
                },
                c => Assert.Equal((uint)0x000FF0, c.CPU.REGS.ABC));
        }

        [Fact]
        public void RL_rrr_Innn_nnnI() // Ok
        {
            RunTest(
                "RL ABC,(0x4000+4)",
                c =>
                {
                    c.CPU.REGS.ABC = 0x0000FF;
                    c.MEMC.Set8bitToRAM(0x4004, 4);
                },
                c => Assert.Equal((uint)0x000FF0, c.CPU.REGS.ABC));
        }

        [Fact]
        public void RL_rrr_Innn_rI() // Ok
        {
            RunTest(
                "RL ABC,(0x4000+X)",
                c =>
                {
                    c.CPU.REGS.ABC = 0x0000FF;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set8bitToRAM(0x4003, 4);
                },
                c => Assert.Equal((uint)0x000FF0, c.CPU.REGS.ABC));
        }

        [Fact]
        public void RL_rrr_Innn_rrI() // Ok
        {
            RunTest(
                "RL ABC,(0x4000+WX)",
                c =>
                {
                    c.CPU.REGS.ABC = 0x0000FF;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x4005, 4);
                },
                c => Assert.Equal((uint)0x000FF0, c.CPU.REGS.ABC));
        }

        [Fact]
        public void RL_rrr_Innn_rrrI() // Ok
        {
            RunTest(
                "RL ABC,(0x4000+VWX)",
                c =>
                {
                    c.CPU.REGS.ABC = 0x0000FF;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x4007, 4);
                },
                c => Assert.Equal((uint)0x000FF0, c.CPU.REGS.ABC));
        }

        [Fact]
        public void RL_rrr_IrrrI() // Ok
        {
            RunTest(
                "RL ABC,(QRS)",
                c =>
                {
                    c.CPU.REGS.ABC = 0x0000FF;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x5000, 4);
                },
                c => Assert.Equal((uint)0x000FF0, c.CPU.REGS.ABC));
        }

        [Fact]
        public void RL_rrr_Irrr_nnnI() // Ok
        {
            RunTest(
                "RL ABC,(QRS+4)",
                c =>
                {
                    c.CPU.REGS.ABC = 0x0000FF;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x5004, 4);
                },
                c => Assert.Equal((uint)0x000FF0, c.CPU.REGS.ABC));
        }

        [Fact]
        public void RL_rrr_Irrr_rI() // Ok
        {
            RunTest(
                "RL ABC,(QRS+X)",
                c =>
                {
                    c.CPU.REGS.ABC = 0x0000FF;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set8bitToRAM(0x5003, 4);
                },
                c => Assert.Equal((uint)0x000FF0, c.CPU.REGS.ABC));
        }

        [Fact]
        public void RL_rrr_Irrr_rrI() // Ok
        {
            RunTest(
                "RL ABC,(QRS+WX)",
                c =>
                {
                    c.CPU.REGS.ABC = 0x0000FF;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x5005, 4);
                },
                c => Assert.Equal((uint)0x000FF0, c.CPU.REGS.ABC));
        }

        [Fact]
        public void RL_rrr_Irrr_rrrI() // Ok
        {
            RunTest(
                "RL ABC,(QRS+VWX)",
                c =>
                {
                    c.CPU.REGS.ABC = 0x0000FF;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x5007, 4);
                },
                c => Assert.Equal((uint)0x000FF0, c.CPU.REGS.ABC));
        }

        [Fact]
        public void RL_rrr_fr() // Ok
        {
            RunTest(
                "RL ABC,F2",
                c =>
                {
                    c.CPU.REGS.ABC = 0x0000FF;
                    c.CPU.FREGS.SetRegister(2, 4.0f);
                },
                c => Assert.Equal((uint)0x000FF0, c.CPU.REGS.ABC));
        }

        #endregion

        #region SL rrrr,* tests

        [Fact]
        public void RL_rrrr_nnnn() // Ok
        {
            RunTest(
                "RL ABCD,0x00000004",
                c => c.CPU.REGS.ABCD = 0x000000FF,
                c => Assert.Equal((uint)0x00000FF0, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void RL_rrrr_r() // Ok
        {
            RunTest(
                "RL ABCD,E",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x000000FF;
                    c.CPU.REGS.E = 4;
                },
                c => Assert.Equal((uint)0x00000FF0, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void RL_rrrr_rr() // Ok
        {
            RunTest(
                "RL ABCD,EF",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x000000FF;
                    c.CPU.REGS.EF = 4;
                },
                c => Assert.Equal((uint)0x00000FF0, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void RL_rrrr_rrr() // Ok
        {
            RunTest(
                "RL ABCD,EFG",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x000000FF;
                    c.CPU.REGS.EFG = 4;
                },
                c => Assert.Equal((uint)0x00000FF0, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void RL_rrrr_rrrr() // Ok
        {
            RunTest(
                "RL ABCD,EFGH",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x000000FF;
                    c.CPU.REGS.EFGH = 4;
                },
                c => Assert.Equal((uint)0x00000FF0, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void RL_rrrr_InnnI() // Ok
        {
            RunTest(
                "RL ABCD,(0x4000)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x000000FF;
                    c.MEMC.Set8bitToRAM(0x4000, 4);
                },
                c => Assert.Equal((uint)0x00000FF0, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void RL_rrrr_Innn_nnnI() // Ok
        {
            RunTest(
                "RL ABCD,(0x4000+4)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x000000FF;
                    c.MEMC.Set8bitToRAM(0x4004, 4);
                },
                c => Assert.Equal((uint)0x00000FF0, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void RL_rrrr_Innn_rI() // Ok
        {
            RunTest(
                "RL ABCD,(0x4000+X)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x000000FF;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set8bitToRAM(0x4003, 4);
                },
                c => Assert.Equal((uint)0x00000FF0, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void RL_rrrr_Innn_rrI() // Ok
        {
            RunTest(
                "RL ABCD,(0x4000+WX)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x000000FF;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x4005, 4);
                },
                c => Assert.Equal((uint)0x00000FF0, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void RL_rrrr_Innn_rrrI() // Ok
        {
            RunTest(
                "RL ABCD,(0x4000+VWX)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x000000FF;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x4007, 4);
                },
                c => Assert.Equal((uint)0x00000FF0, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void RL_rrrr_IrrrI() // Ok
        {
            RunTest(
                "RL ABCD,(QRS)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x000000FF;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x5000, 4);
                },
                c => Assert.Equal((uint)0x00000FF0, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void RL_rrrr_Irrr_nnnI() // Ok
        {
            RunTest(
                "RL ABCD,(QRS+4)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x000000FF;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x5004, 4);
                },
                c => Assert.Equal((uint)0x00000FF0, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void RL_rrrr_Irrr_rI() // Ok
        {
            RunTest(
                "RL ABCD,(QRS+X)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x000000FF;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set8bitToRAM(0x5003, 4);
                },
                c => Assert.Equal((uint)0x00000FF0, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void RL_rrrr_Irrr_rrI() // Ok
        {
            RunTest(
                "RL ABCD,(QRS+WX)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x000000FF;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x5005, 4);
                },
                c => Assert.Equal((uint)0x00000FF0, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void RL_rrrr_Irrr_rrrI() // Ok
        {
            RunTest(
                "RL ABCD,(QRS+VWX)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x000000FF;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x5007, 4);
                },
                c => Assert.Equal((uint)0x00000FF0, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void RL_rrrr_fr() // Ok
        {
            RunTest(
                "RL ABCD,F3",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x000000FF;
                    c.CPU.FREGS.SetRegister(3, 4.0f);
                },
                c => Assert.Equal((uint)0x00000FF0, c.CPU.REGS.ABCD));
        }

        #endregion

        #region SL memory destination variants (unwound: 10 targets × 17 cases)

        // Important: index registers overlap (X, WX, VWX), so for combined addressing-mode tests
        // compute effective addresses after ALL setup is complete, mirroring the DIV tests.

        // --------------------
        // Target: (0x4000)
        // --------------------

        [Fact]
        public void RL_mem_InnnI_block_immediate_2bytes_once() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (0x4000),0x00000002,2",
                c =>
                {
                    addr = 0x4000;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                },
                c => Assert.Equal([0x01, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_InnnI_block_immediate_2bytes_twice() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (0x4000),0x00000002,2,2",
                c =>
                {
                    addr = 0x4000;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_InnnI_r() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (0x4000),B",
                c =>
                {
                    addr = 0x4000;
                    c.MEMC.Set8bitToRAM(addr, 0x0F);
                    c.CPU.REGS.B = 4;
                },
                c => Assert.Equal((byte)0xF0, c.MEMC.Get8bitFromRAM(addr)));
        }

        [Fact]
        public void RL_mem_InnnI_rr() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (0x4000),CD",
                c =>
                {
                    addr = 0x4000;
                    c.MEMC.Set16bitToRAM(addr, 0x00FF);
                    c.CPU.REGS.CD = 4;
                },
                c => Assert.Equal((ushort)0x0FF0, c.MEMC.Get16bitFromRAM(addr)));
        }

        [Fact]
        public void RL_mem_InnnI_rrr() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (0x4000),DEF",
                c =>
                {
                    addr = 0x4000;
                    c.MEMC.Set24bitToRAM(addr, 0x0000FF);
                    c.CPU.REGS.DEF = 4;
                },
                c => Assert.Equal((uint)0x000FF0, c.MEMC.Get24bitFromRAM(addr)));
        }

        [Fact]
        public void RL_mem_InnnI_rrrr() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (0x4000),EFGH",
                c =>
                {
                    addr = 0x4000;
                    c.MEMC.Set32bitToRAM(addr, 0x000000FF);
                    c.CPU.REGS.EFGH = 4;
                },
                c => Assert.Equal((uint)0x00000FF0, c.MEMC.Get32bitFromRAM(addr)));
        }

        [Fact]
        public void RL_mem_InnnI_value_InnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (0x4000),(0x7000),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    addr = 0x4000;
                    valueAddr = 0x7000;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_InnnI_value_Innn_nnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (0x4000),(0x7000+4),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    addr = 0x4000;
                    valueAddr = 0x7004;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_InnnI_value_Innn_rI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (0x4000),(0x7000+X),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    addr = 0x4000;
                    c.CPU.REGS.X = 3; valueAddr = (uint)(0x7000 + c.CPU.REGS.X);
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_InnnI_value_Innn_rrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (0x4000),(0x7000+WX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    addr = 0x4000;
                    c.CPU.REGS.WX = 5; valueAddr = (uint)(0x7000 + c.CPU.REGS.WX);
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_InnnI_value_Innn_rrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (0x4000),(0x7000+VWX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    addr = 0x4000;
                    c.CPU.REGS.VWX = 7; valueAddr = (uint)(0x7000 + c.CPU.REGS.VWX);
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_InnnI_value_IrrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (0x4000),(TUV),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    addr = 0x4000;
                    c.CPU.REGS.TUV = 0x8000; valueAddr = c.CPU.REGS.TUV;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_InnnI_value_Irrr_nnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (0x4000),(TUV+4),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    addr = 0x4000;
                    c.CPU.REGS.TUV = 0x8000; valueAddr = c.CPU.REGS.TUV + 4;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_InnnI_value_Irrr_rI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (0x4000),(TUV+X),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    addr = 0x4000;
                    c.CPU.REGS.TUV = 0x8000; c.CPU.REGS.X = 3; valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.X;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_InnnI_value_Irrr_rrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (0x4000),(TUV+WX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    addr = 0x4000;
                    c.CPU.REGS.TUV = 0x8000; c.CPU.REGS.WX = 5; valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.WX;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_InnnI_value_Irrr_rrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (0x4000),(TUV+VWX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    addr = 0x4000;
                    c.CPU.REGS.TUV = 0x8000; c.CPU.REGS.VWX = 7; valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.VWX;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_InnnI_fr() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (0x4000),F0",
                c =>
                {
                    addr = 0x4000;
                    c.MEMC.SetFloatToRam(addr, 1.0f);
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                },
                c =>
                {
                    float expected = ShiftFloatBits(1.0f, 1);
                    Assert.Equal(expected, c.MEMC.GetFloatFromRAM(addr));
                });
        }

        // --------------------
        // Target: (0x4000+4)
        // --------------------

        [Fact]
        public void RL_mem_Innn_nnnI_block_immediate_2bytes_once() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (0x4000+4),0x00000002,2",
                c =>
                {
                    addr = 0x4004;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                },
                c => Assert.Equal([0x01, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Innn_nnnI_block_immediate_2bytes_twice() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (0x4000+4),0x00000002,2,2",
                c =>
                {
                    addr = 0x4004;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Innn_nnnI_r() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (0x4000+4),B",
                c =>
                {
                    addr = 0x4004;
                    c.MEMC.Set8bitToRAM(addr, 0x0F);
                    c.CPU.REGS.B = 4;
                },
                c => Assert.Equal((byte)0xF0, c.MEMC.Get8bitFromRAM(addr)));
        }

        [Fact]
        public void RL_mem_Innn_nnnI_rr() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (0x4000+4),CD",
                c =>
                {
                    addr = 0x4004;
                    c.MEMC.Set16bitToRAM(addr, 0x00FF);
                    c.CPU.REGS.CD = 4;
                },
                c => Assert.Equal((ushort)0x0FF0, c.MEMC.Get16bitFromRAM(addr)));
        }

        [Fact]
        public void RL_mem_Innn_nnnI_rrr() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (0x4000+4),DEF",
                c =>
                {
                    addr = 0x4004;
                    c.MEMC.Set24bitToRAM(addr, 0x0000FF);
                    c.CPU.REGS.DEF = 4;
                },
                c => Assert.Equal((uint)0x000FF0, c.MEMC.Get24bitFromRAM(addr)));
        }

        [Fact]
        public void RL_mem_Innn_nnnI_rrrr() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (0x4000+4),EFGH",
                c =>
                {
                    addr = 0x4004;
                    c.MEMC.Set32bitToRAM(addr, 0x000000FF);
                    c.CPU.REGS.EFGH = 4;
                },
                c => Assert.Equal((uint)0x00000FF0, c.MEMC.Get32bitFromRAM(addr)));
        }

        [Fact]
        public void RL_mem_Innn_nnnI_value_InnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (0x4000+4),(0x7000),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    addr = 0x4004;
                    valueAddr = 0x7000;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Innn_nnnI_value_Innn_nnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (0x4000+4),(0x7000+4),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    addr = 0x4004;
                    valueAddr = 0x7004;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Innn_nnnI_value_Innn_nnnI_block_larger() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (0x4000+4),(0x7000+4),16,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 1;
                    addr = 0x4004;
                    valueAddr = 0x7004;
                    c.LoadMemAt(addr, [0x01, 0x40, 0x02, 0x39, 0x03, 0x38, 0x04, 0x37, 0x05, 0x36, 0x06, 0x35, 0x07, 0x34, 0x08, 0x33, 0x09, 0x32]);
                    c.LoadMemAt(valueAddr, [0x04]);
                },
                c => Assert.Equal([0x14, 0x00, 0x23, 0x90, 0x33, 0x80, 0x43, 0x70, 0x53, 0x60, 0x63, 0x50, 0x73, 0x40, 0x83, 0x30, 0x09, 0x32], c.MEMC.RAM.GetMemoryAt(addr, 18)));
        }

        [Fact]
        public void RL_mem_Innn_nnnI_value_Innn_rI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (0x4000+4),(0x7000+X),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    addr = 0x4004;
                    c.CPU.REGS.X = 3; valueAddr = (uint)(0x7000 + c.CPU.REGS.X);
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Innn_nnnI_value_Innn_rrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (0x4000+4),(0x7000+WX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    addr = 0x4004;
                    c.CPU.REGS.WX = 5; valueAddr = (uint)(0x7000 + c.CPU.REGS.WX);
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Innn_nnnI_value_Innn_rrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (0x4000+4),(0x7000+VWX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    addr = 0x4004;
                    c.CPU.REGS.VWX = 7; valueAddr = (uint)(0x7000 + c.CPU.REGS.VWX);
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Innn_nnnI_value_IrrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (0x4000+4),(TUV),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    addr = 0x4004;
                    c.CPU.REGS.TUV = 0x8000; valueAddr = c.CPU.REGS.TUV;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Innn_nnnI_value_Irrr_nnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (0x4000+4),(TUV+4),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    addr = 0x4004;
                    c.CPU.REGS.TUV = 0x8000; valueAddr = c.CPU.REGS.TUV + 4;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Innn_nnnI_value_Irrr_rI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (0x4000+4),(TUV+X),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    addr = 0x4004;
                    c.CPU.REGS.TUV = 0x8000; c.CPU.REGS.X = 3; valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.X;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Innn_nnnI_value_Irrr_rrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (0x4000+4),(TUV+WX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    addr = 0x4004;
                    c.CPU.REGS.TUV = 0x8000; c.CPU.REGS.WX = 5; valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.WX;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Innn_nnnI_value_Irrr_rrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (0x4000+4),(TUV+VWX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    addr = 0x4004;
                    c.CPU.REGS.TUV = 0x8000; c.CPU.REGS.VWX = 7; valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.VWX;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Innn_nnnI_fr() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (0x4000+4),F0",
                c =>
                {
                    addr = 0x4004;
                    c.MEMC.SetFloatToRam(addr, 1.0f);
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                },
                c =>
                {
                    float expected = ShiftFloatBits(1.0f, 1);
                    Assert.Equal(expected, c.MEMC.GetFloatFromRAM(addr));
                });
        }

        // --------------------
        // Target: (0x4000+X)
        // --------------------

        [Fact]
        public void RL_mem_Innn_rI_block_immediate_2bytes_once() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (0x4000+X),0x00000002,2",
                c =>
                {
                    c.CPU.REGS.X = 3; addr = (uint)(0x4000 + c.CPU.REGS.X);
                    c.LoadMemAt(addr, [0x00, 0x40]);
                },
                c => Assert.Equal([0x01, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Innn_rI_block_immediate_2bytes_twice() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (0x4000+X),0x00000002,2,2",
                c =>
                {
                    c.CPU.REGS.X = 3; addr = (uint)(0x4000 + c.CPU.REGS.X);
                    c.LoadMemAt(addr, [0x00, 0x40]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Innn_rI_r() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (0x4000+X),B",
                c =>
                {
                    c.CPU.REGS.X = 3; addr = (uint)(0x4000 + c.CPU.REGS.X);
                    c.MEMC.Set8bitToRAM(addr, 0x0F);
                    c.CPU.REGS.B = 4;
                },
                c => Assert.Equal((byte)0xF0, c.MEMC.Get8bitFromRAM(addr)));
        }

        [Fact]
        public void RL_mem_Innn_rI_rr() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (0x4000+X),CD",
                c =>
                {
                    c.CPU.REGS.X = 3; addr = (uint)(0x4000 + c.CPU.REGS.X);
                    c.MEMC.Set16bitToRAM(addr, 0x00FF);
                    c.CPU.REGS.CD = 4;
                },
                c => Assert.Equal((ushort)0x0FF0, c.MEMC.Get16bitFromRAM(addr)));
        }

        [Fact]
        public void RL_mem_Innn_rI_rrr() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (0x4000+X),DEF",
                c =>
                {
                    c.CPU.REGS.X = 3; addr = (uint)(0x4000 + c.CPU.REGS.X);
                    c.MEMC.Set24bitToRAM(addr, 0x0000FF);
                    c.CPU.REGS.DEF = 4;
                },
                c => Assert.Equal((uint)0x000FF0, c.MEMC.Get24bitFromRAM(addr)));
        }

        [Fact]
        public void RL_mem_Innn_rI_rrrr() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (0x4000+X),EFGH",
                c =>
                {
                    c.CPU.REGS.X = 3; addr = (uint)(0x4000 + c.CPU.REGS.X);
                    c.MEMC.Set32bitToRAM(addr, 0x000000FF);
                    c.CPU.REGS.EFGH = 4;
                },
                c => Assert.Equal((uint)0x00000FF0, c.MEMC.Get32bitFromRAM(addr)));
        }

        [Fact]
        public void RL_mem_Innn_rI_value_InnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (0x4000+X),(0x7000),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.X = 3; addr = (uint)(0x4000 + c.CPU.REGS.X);
                    valueAddr = 0x7000;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Innn_rI_value_Innn_nnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (0x4000+X),(0x7000+4),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.X = 3; addr = (uint)(0x4000 + c.CPU.REGS.X);
                    valueAddr = 0x7004;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Innn_rI_value_Innn_rI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (0x4000+X),(0x7000+X),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.X = 3; addr = (uint)(0x4000 + c.CPU.REGS.X);
                    c.CPU.REGS.X = 3; valueAddr = (uint)(0x7000 + c.CPU.REGS.X);
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Innn_rI_value_Innn_rrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (0x4000+X),(0x7000+YZ),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.X = 3; addr = (uint)(0x4000 + c.CPU.REGS.X);
                    c.CPU.REGS.YZ = 5; valueAddr = (uint)(0x7000 + c.CPU.REGS.YZ);
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Innn_rI_value_Innn_rrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (0x4000+X),(0x7000+ABC),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.X = 3; addr = (uint)(0x4000 + c.CPU.REGS.X);
                    c.CPU.REGS.ABC = 7; valueAddr = (uint)(0x7000 + c.CPU.REGS.ABC);
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Innn_rI_value_IrrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (0x4000+X),(TUV),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.X = 3; addr = (uint)(0x4000 + c.CPU.REGS.X);
                    c.CPU.REGS.TUV = 0x8000; valueAddr = c.CPU.REGS.TUV;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Innn_rI_value_Irrr_nnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (0x4000+X),(TUV+4),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.X = 3; addr = (uint)(0x4000 + c.CPU.REGS.X);
                    c.CPU.REGS.TUV = 0x8000; valueAddr = c.CPU.REGS.TUV + 4;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Innn_rI_value_Irrr_rI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (0x4000+X),(TUV+X),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.X = 3; addr = (uint)(0x4000 + c.CPU.REGS.X);
                    c.CPU.REGS.TUV = 0x8000; c.CPU.REGS.X = 3; valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.X;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Innn_rI_value_Irrr_rrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (0x4000+X),(TUV+YZ),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.X = 3; addr = (uint)(0x4000 + c.CPU.REGS.X);
                    c.CPU.REGS.TUV = 0x8000; c.CPU.REGS.YZ = 5; valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.YZ;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Innn_rI_value_Irrr_rrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (0x4000+X),(TUV+ABC),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.X = 3; addr = (uint)(0x4000 + c.CPU.REGS.X);
                    c.CPU.REGS.TUV = 0x8000; c.CPU.REGS.ABC = 7; valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.ABC;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Innn_rI_fr() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (0x4000+X),F0",
                c =>
                {
                    c.CPU.REGS.X = 3; addr = (uint)(0x4000 + c.CPU.REGS.X);
                    c.MEMC.SetFloatToRam(addr, 1.0f);
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                },
                c =>
                {
                    float expected = ShiftFloatBits(1.0f, 1);
                    Assert.Equal(expected, c.MEMC.GetFloatFromRAM(addr));
                });
        }

        // --------------------
        // Target: (0x4000+WX)
        // --------------------

        [Fact]
        public void RL_mem_Innn_rrI_block_immediate_2bytes_once() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (0x4000+WX),0x00000002,2",
                c =>
                {
                    c.CPU.REGS.WX = 5; addr = (uint)(0x4000 + c.CPU.REGS.WX);
                    c.LoadMemAt(addr, [0x00, 0x40]);
                },
                c => Assert.Equal([0x01, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Innn_rrI_block_immediate_2bytes_twice() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (0x4000+WX),0x00000002,2,2",
                c =>
                {
                    c.CPU.REGS.WX = 5; addr = (uint)(0x4000 + c.CPU.REGS.WX);
                    c.LoadMemAt(addr, [0x00, 0x40]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Innn_rrI_r() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (0x4000+WX),B",
                c =>
                {
                    c.CPU.REGS.WX = 5; addr = (uint)(0x4000 + c.CPU.REGS.WX);
                    c.MEMC.Set8bitToRAM(addr, 0x0F);
                    c.CPU.REGS.B = 4;
                },
                c => Assert.Equal((byte)0xF0, c.MEMC.Get8bitFromRAM(addr)));
        }

        [Fact]
        public void RL_mem_Innn_rrI_rr() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (0x4000+WX),CD",
                c =>
                {
                    c.CPU.REGS.WX = 5; addr = (uint)(0x4000 + c.CPU.REGS.WX);
                    c.MEMC.Set16bitToRAM(addr, 0x00FF);
                    c.CPU.REGS.CD = 4;
                },
                c => Assert.Equal((ushort)0x0FF0, c.MEMC.Get16bitFromRAM(addr)));
        }

        [Fact]
        public void RL_mem_Innn_rrI_rrr() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (0x4000+WX),DEF",
                c =>
                {
                    c.CPU.REGS.WX = 5; addr = (uint)(0x4000 + c.CPU.REGS.WX);
                    c.MEMC.Set24bitToRAM(addr, 0x0000FF);
                    c.CPU.REGS.DEF = 4;
                },
                c => Assert.Equal((uint)0x000FF0, c.MEMC.Get24bitFromRAM(addr)));
        }

        [Fact]
        public void RL_mem_Innn_rrI_rrrr() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (0x4000+WX),EFGH",
                c =>
                {
                    c.CPU.REGS.WX = 5; addr = (uint)(0x4000 + c.CPU.REGS.WX);
                    c.MEMC.Set32bitToRAM(addr, 0x000000FF);
                    c.CPU.REGS.EFGH = 4;
                },
                c => Assert.Equal((uint)0x00000FF0, c.MEMC.Get32bitFromRAM(addr)));
        }

        [Fact]
        public void RL_mem_Innn_rrI_value_InnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (0x4000+WX),(0x7000),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.WX = 5; addr = (uint)(0x4000 + c.CPU.REGS.WX);
                    valueAddr = 0x7000;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Innn_rrI_value_Innn_nnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (0x4000+WX),(0x7000+4),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.WX = 5; addr = (uint)(0x4000 + c.CPU.REGS.WX);
                    valueAddr = 0x7004;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Innn_rrI_value_Innn_rI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (0x4000+YZ),(0x7000+X),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.YZ = 5; addr = (uint)(0x4000 + c.CPU.REGS.YZ);
                    c.CPU.REGS.X = 3; valueAddr = (uint)(0x7000 + c.CPU.REGS.X);
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Innn_rrI_value_Innn_rrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (0x4000+WX),(0x7000+WX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.WX = 5; addr = (uint)(0x4000 + c.CPU.REGS.WX);
                    c.CPU.REGS.WX = 5; valueAddr = (uint)(0x7000 + c.CPU.REGS.WX);
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Innn_rrI_value_Innn_rrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (0x4000+WX),(0x7000+ABC),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.WX = 5; addr = (uint)(0x4000 + c.CPU.REGS.WX);
                    c.CPU.REGS.ABC = 7; valueAddr = (uint)(0x7000 + c.CPU.REGS.ABC);
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Innn_rrI_value_IrrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (0x4000+WX),(TUV),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.WX = 5; addr = (uint)(0x4000 + c.CPU.REGS.WX);
                    c.CPU.REGS.TUV = 0x8000; valueAddr = c.CPU.REGS.TUV;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Innn_rrI_value_Irrr_nnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (0x4000+WX),(TUV+4),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.WX = 5; addr = (uint)(0x4000 + c.CPU.REGS.WX);
                    c.CPU.REGS.TUV = 0x8000; valueAddr = c.CPU.REGS.TUV + 4;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Innn_rrI_value_Irrr_rI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (0x4000+YZ),(TUV+X),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.YZ = 5; addr = (uint)(0x4000 + c.CPU.REGS.YZ);
                    c.CPU.REGS.TUV = 0x8000; c.CPU.REGS.X = 3; valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.X;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Innn_rrI_value_Irrr_rrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (0x4000+WX),(TUV+WX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.WX = 5; addr = (uint)(0x4000 + c.CPU.REGS.WX);
                    c.CPU.REGS.TUV = 0x8000; c.CPU.REGS.WX = 5; valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.WX;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Innn_rrI_value_Irrr_rrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (0x4000+AB),(TUV+VWX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.AB = 5; addr = (uint)(0x4000 + c.CPU.REGS.AB);
                    c.CPU.REGS.TUV = 0x8000; c.CPU.REGS.VWX = 7; valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.VWX;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Innn_rrI_fr() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (0x4000+WX),F0",
                c =>
                {
                    c.CPU.REGS.WX = 5; addr = (uint)(0x4000 + c.CPU.REGS.WX);
                    c.MEMC.SetFloatToRam(addr, 1.0f);
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                },
                c =>
                {
                    float expected = ShiftFloatBits(1.0f, 1);
                    Assert.Equal(expected, c.MEMC.GetFloatFromRAM(addr));
                });
        }

        // --------------------
        // Target: (0x4000+VWX)
        // --------------------

        [Fact]
        public void RL_mem_Innn_rrrI_block_immediate_2bytes_once() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (0x4000+VWX),0x00000002,2",
                c =>
                {
                    c.CPU.REGS.VWX = 7; addr = (uint)(0x4000 + c.CPU.REGS.VWX);
                    c.LoadMemAt(addr, [0x00, 0x40]);
                },
                c => Assert.Equal([0x01, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Innn_rrrI_block_immediate_2bytes_twice() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (0x4000+VWX),0x00000002,2,2",
                c =>
                {
                    c.CPU.REGS.VWX = 7; addr = (uint)(0x4000 + c.CPU.REGS.VWX);
                    c.LoadMemAt(addr, [0x00, 0x40]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Innn_rrrI_r() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (0x4000+VWX),B",
                c =>
                {
                    c.CPU.REGS.VWX = 7; addr = (uint)(0x4000 + c.CPU.REGS.VWX);
                    c.MEMC.Set8bitToRAM(addr, 0x0F);
                    c.CPU.REGS.B = 4;
                },
                c => Assert.Equal((byte)0xF0, c.MEMC.Get8bitFromRAM(addr)));
        }

        [Fact]
        public void RL_mem_Innn_rrrI_rr() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (0x4000+VWX),CD",
                c =>
                {
                    c.CPU.REGS.VWX = 7; addr = (uint)(0x4000 + c.CPU.REGS.VWX);
                    c.MEMC.Set16bitToRAM(addr, 0x00FF);
                    c.CPU.REGS.CD = 4;
                },
                c => Assert.Equal((ushort)0x0FF0, c.MEMC.Get16bitFromRAM(addr)));
        }

        [Fact]
        public void RL_mem_Innn_rrrI_rrr() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (0x4000+VWX),DEF",
                c =>
                {
                    c.CPU.REGS.VWX = 7; addr = (uint)(0x4000 + c.CPU.REGS.VWX);
                    c.MEMC.Set24bitToRAM(addr, 0x0000FF);
                    c.CPU.REGS.DEF = 4;
                },
                c => Assert.Equal((uint)0x000FF0, c.MEMC.Get24bitFromRAM(addr)));
        }

        [Fact]
        public void RL_mem_Innn_rrrI_rrrr() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (0x4000+VWX),EFGH",
                c =>
                {
                    c.CPU.REGS.VWX = 7; addr = (uint)(0x4000 + c.CPU.REGS.VWX);
                    c.MEMC.Set32bitToRAM(addr, 0x000000FF);
                    c.CPU.REGS.EFGH = 4;
                },
                c => Assert.Equal((uint)0x00000FF0, c.MEMC.Get32bitFromRAM(addr)));
        }

        [Fact]
        public void RL_mem_Innn_rrrI_value_InnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (0x4000+VWX),(0x7000),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.VWX = 7; addr = (uint)(0x4000 + c.CPU.REGS.VWX);
                    valueAddr = 0x7000;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Innn_rrrI_value_Innn_nnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (0x4000+VWX),(0x7000+4),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.VWX = 7; addr = (uint)(0x4000 + c.CPU.REGS.VWX);
                    valueAddr = 0x7004;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Innn_rrrI_value_Innn_rI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (0x4000+VWX),(0x7000+A),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.VWX = 7; addr = (uint)(0x4000 + c.CPU.REGS.VWX);
                    c.CPU.REGS.A = 3; valueAddr = (uint)(0x7000 + c.CPU.REGS.A);
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Innn_rrrI_value_Innn_rrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (0x4000+VWX),(0x7000+AB),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.VWX = 7; addr = (uint)(0x4000 + c.CPU.REGS.VWX);
                    c.CPU.REGS.AB = 5; valueAddr = (uint)(0x7000 + c.CPU.REGS.AB);
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Innn_rrrI_value_Innn_rrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (0x4000+VWX),(0x7000+VWX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.VWX = 7; addr = (uint)(0x4000 + c.CPU.REGS.VWX);
                    c.CPU.REGS.VWX = 7; valueAddr = (uint)(0x7000 + c.CPU.REGS.VWX);
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Innn_rrrI_value_IrrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (0x4000+VWX),(TUV),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.VWX = 7; addr = (uint)(0x4000 + c.CPU.REGS.VWX);
                    c.CPU.REGS.TUV = 0x8000; valueAddr = c.CPU.REGS.TUV;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Innn_rrrI_value_Irrr_nnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (0x4000+VWX),(TUV+4),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.VWX = 7; addr = (uint)(0x4000 + c.CPU.REGS.VWX);
                    c.CPU.REGS.TUV = 0x8000; valueAddr = c.CPU.REGS.TUV + 4;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Innn_rrrI_value_Irrr_rI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (0x4000+VWX),(TUV+A),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.VWX = 7; addr = (uint)(0x4000 + c.CPU.REGS.VWX);
                    c.CPU.REGS.TUV = 0x8000; c.CPU.REGS.A = 3; valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.A;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Innn_rrrI_value_Irrr_rrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (0x4000+VWX),(TUV+AB),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.VWX = 7; addr = (uint)(0x4000 + c.CPU.REGS.VWX);
                    c.CPU.REGS.TUV = 0x8000; c.CPU.REGS.AB = 5; valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.AB;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Innn_rrrI_value_Irrr_rrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (0x4000+VWX),(TUV+VWX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.VWX = 7; addr = (uint)(0x4000 + c.CPU.REGS.VWX);
                    c.CPU.REGS.TUV = 0x8000; c.CPU.REGS.VWX = 7; valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.VWX;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Innn_rrrI_fr() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (0x4000+VWX),F0",
                c =>
                {
                    c.CPU.REGS.VWX = 7; addr = (uint)(0x4000 + c.CPU.REGS.VWX);
                    c.MEMC.SetFloatToRam(addr, 1.0f);
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                },
                c =>
                {
                    float expected = ShiftFloatBits(1.0f, 1);
                    Assert.Equal(expected, c.MEMC.GetFloatFromRAM(addr));
                });
        }

        // --------------------
        // Target: (QRS)
        // --------------------

        [Fact]
        public void RL_mem_IrrrI_block_immediate_2bytes_once() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (QRS),0x00000002,2",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000; addr = c.CPU.REGS.QRS;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                },
                c => Assert.Equal([0x01, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_IrrrI_block_immediate_2bytes_twice() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (QRS),0x00000002,2,2",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000; addr = c.CPU.REGS.QRS;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_IrrrI_r() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (QRS),B",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000; addr = c.CPU.REGS.QRS;
                    c.MEMC.Set8bitToRAM(addr, 0x0F);
                    c.CPU.REGS.B = 4;
                },
                c => Assert.Equal((byte)0xF0, c.MEMC.Get8bitFromRAM(addr)));
        }

        [Fact]
        public void RL_mem_IrrrI_rr() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (QRS),CD",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000; addr = c.CPU.REGS.QRS;
                    c.MEMC.Set16bitToRAM(addr, 0x00FF);
                    c.CPU.REGS.CD = 4;
                },
                c => Assert.Equal((ushort)0x0FF0, c.MEMC.Get16bitFromRAM(addr)));
        }

        [Fact]
        public void RL_mem_IrrrI_rrr() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (QRS),DEF",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000; addr = c.CPU.REGS.QRS;
                    c.MEMC.Set24bitToRAM(addr, 0x0000FF);
                    c.CPU.REGS.DEF = 4;
                },
                c => Assert.Equal((uint)0x000FF0, c.MEMC.Get24bitFromRAM(addr)));
        }

        [Fact]
        public void RL_mem_IrrrI_rrrr() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (QRS),EFGH",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000; addr = c.CPU.REGS.QRS;
                    c.MEMC.Set32bitToRAM(addr, 0x000000FF);
                    c.CPU.REGS.EFGH = 4;
                },
                c => Assert.Equal((uint)0x00000FF0, c.MEMC.Get32bitFromRAM(addr)));
        }

        [Fact]
        public void RL_mem_IrrrI_value_InnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (QRS),(0x7000),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; addr = c.CPU.REGS.QRS;
                    valueAddr = 0x7000;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_IrrrI_value_Innn_nnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (QRS),(0x7000+4),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; addr = c.CPU.REGS.QRS;
                    valueAddr = 0x7004;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_IrrrI_value_Innn_rI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (QRS),(0x7000+X),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; addr = c.CPU.REGS.QRS;
                    c.CPU.REGS.X = 3; valueAddr = (uint)(0x7000 + c.CPU.REGS.X);
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_IrrrI_value_Innn_rrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (QRS),(0x7000+WX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; addr = c.CPU.REGS.QRS;
                    c.CPU.REGS.WX = 5; valueAddr = (uint)(0x7000 + c.CPU.REGS.WX);
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_IrrrI_value_Innn_rrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (QRS),(0x7000+VWX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; addr = c.CPU.REGS.QRS;
                    c.CPU.REGS.VWX = 7; valueAddr = (uint)(0x7000 + c.CPU.REGS.VWX);
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_IrrrI_value_IrrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (QRS),(TUV),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; addr = c.CPU.REGS.QRS;
                    c.CPU.REGS.TUV = 0x8000; valueAddr = c.CPU.REGS.TUV;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_IrrrI_value_Irrr_nnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (QRS),(TUV+4),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; addr = c.CPU.REGS.QRS;
                    c.CPU.REGS.TUV = 0x8000; valueAddr = c.CPU.REGS.TUV + 4;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_IrrrI_value_Irrr_rI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (QRS),(TUV+X),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; addr = c.CPU.REGS.QRS;
                    c.CPU.REGS.TUV = 0x8000; c.CPU.REGS.X = 3; valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.X;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_IrrrI_value_Irrr_rrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (QRS),(TUV+WX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; addr = c.CPU.REGS.QRS;
                    c.CPU.REGS.TUV = 0x8000; c.CPU.REGS.WX = 5; valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.WX;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_IrrrI_value_Irrr_rrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (QRS),(TUV+VWX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; addr = c.CPU.REGS.QRS;
                    c.CPU.REGS.TUV = 0x8000; c.CPU.REGS.VWX = 7; valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.VWX;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_IrrrI_fr() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (QRS),F0",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000; addr = c.CPU.REGS.QRS;
                    c.MEMC.SetFloatToRam(addr, 1.0f);
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                },
                c =>
                {
                    float expected = ShiftFloatBits(1.0f, 1);
                    Assert.Equal(expected, c.MEMC.GetFloatFromRAM(addr));
                });
        }

        // --------------------
        // Target: (QRS+4)
        // --------------------

        [Fact]
        public void RL_mem_Irrr_nnnI_block_immediate_2bytes_once() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (QRS+4),0x00000002,2",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000; addr = c.CPU.REGS.QRS + 4;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                },
                c => Assert.Equal([0x01, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Irrr_nnnI_block_immediate_2bytes_twice() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (QRS+4),0x00000002,2,2",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000; addr = c.CPU.REGS.QRS + 4;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Irrr_nnnI_r() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (QRS+4),B",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000; addr = c.CPU.REGS.QRS + 4;
                    c.MEMC.Set8bitToRAM(addr, 0x0F);
                    c.CPU.REGS.B = 4;
                },
                c => Assert.Equal((byte)0xF0, c.MEMC.Get8bitFromRAM(addr)));
        }

        [Fact]
        public void RL_mem_Irrr_nnnI_rr() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (QRS+4),CD",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000; addr = c.CPU.REGS.QRS + 4;
                    c.MEMC.Set16bitToRAM(addr, 0x00FF);
                    c.CPU.REGS.CD = 4;
                },
                c => Assert.Equal((ushort)0x0FF0, c.MEMC.Get16bitFromRAM(addr)));
        }

        [Fact]
        public void RL_mem_Irrr_nnnI_rrr() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (QRS+4),DEF",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000; addr = c.CPU.REGS.QRS + 4;
                    c.MEMC.Set24bitToRAM(addr, 0x0000FF);
                    c.CPU.REGS.DEF = 4;
                },
                c => Assert.Equal((uint)0x000FF0, c.MEMC.Get24bitFromRAM(addr)));
        }

        [Fact]
        public void RL_mem_Irrr_nnnI_rrrr() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (QRS+4),EFGH",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000; addr = c.CPU.REGS.QRS + 4;
                    c.MEMC.Set32bitToRAM(addr, 0x000000FF);
                    c.CPU.REGS.EFGH = 4;
                },
                c => Assert.Equal((uint)0x00000FF0, c.MEMC.Get32bitFromRAM(addr)));
        }

        [Fact]
        public void RL_mem_Irrr_nnnI_value_InnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (QRS+4),(0x7000),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; addr = c.CPU.REGS.QRS + 4;
                    valueAddr = 0x7000;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Irrr_nnnI_value_Innn_nnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (QRS+4),(0x7000+4),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; addr = c.CPU.REGS.QRS + 4;
                    valueAddr = 0x7004;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Irrr_nnnI_value_Innn_rI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (QRS+4),(0x7000+X),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; addr = c.CPU.REGS.QRS + 4;
                    c.CPU.REGS.X = 3; valueAddr = (uint)(0x7000 + c.CPU.REGS.X);
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Irrr_nnnI_value_Innn_rrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (QRS+4),(0x7000+WX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; addr = c.CPU.REGS.QRS + 4;
                    c.CPU.REGS.WX = 5; valueAddr = (uint)(0x7000 + c.CPU.REGS.WX);
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Irrr_nnnI_value_Innn_rrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (QRS+4),(0x7000+VWX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; addr = c.CPU.REGS.QRS + 4;
                    c.CPU.REGS.VWX = 7; valueAddr = (uint)(0x7000 + c.CPU.REGS.VWX);
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Irrr_nnnI_value_IrrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (QRS+4),(TUV),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; addr = c.CPU.REGS.QRS + 4;
                    c.CPU.REGS.TUV = 0x8000; valueAddr = c.CPU.REGS.TUV;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Irrr_nnnI_value_Irrr_nnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (QRS+4),(TUV+4),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; addr = c.CPU.REGS.QRS + 4;
                    c.CPU.REGS.TUV = 0x8000; valueAddr = c.CPU.REGS.TUV + 4;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Irrr_nnnI_value_Irrr_rI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (QRS+4),(TUV+X),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; addr = c.CPU.REGS.QRS + 4;
                    c.CPU.REGS.TUV = 0x8000; c.CPU.REGS.X = 3; valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.X;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Irrr_nnnI_value_Irrr_rrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (QRS+4),(TUV+WX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; addr = c.CPU.REGS.QRS + 4;
                    c.CPU.REGS.TUV = 0x8000; c.CPU.REGS.WX = 5; valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.WX;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Irrr_nnnI_value_Irrr_rrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (QRS+4),(TUV+VWX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; addr = c.CPU.REGS.QRS + 4;
                    c.CPU.REGS.TUV = 0x8000; c.CPU.REGS.VWX = 7; valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.VWX;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Irrr_nnnI_fr() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (QRS+4),F0",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000; addr = c.CPU.REGS.QRS + 4;
                    c.MEMC.SetFloatToRam(addr, 1.0f);
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                },
                c =>
                {
                    float expected = ShiftFloatBits(1.0f, 1);
                    Assert.Equal(expected, c.MEMC.GetFloatFromRAM(addr));
                });
        }

        // --------------------
        // Target: (QRS+X)
        // --------------------

        [Fact]
        public void RL_mem_Irrr_rI_block_immediate_2bytes_once() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (QRS+X),0x00000002,2",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.X = 3; addr = c.CPU.REGS.QRS + c.CPU.REGS.X;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                },
                c => Assert.Equal([0x01, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Irrr_rI_block_immediate_2bytes_twice() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (QRS+X),0x00000002,2,2",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.X = 3; addr = c.CPU.REGS.QRS + c.CPU.REGS.X;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Irrr_rI_r() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (QRS+X),B",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.X = 3; addr = c.CPU.REGS.QRS + c.CPU.REGS.X;
                    c.MEMC.Set8bitToRAM(addr, 0x0F);
                    c.CPU.REGS.B = 4;
                },
                c => Assert.Equal((byte)0xF0, c.MEMC.Get8bitFromRAM(addr)));
        }

        [Fact]
        public void RL_mem_Irrr_rI_rr() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (QRS+X),CD",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.X = 3; addr = c.CPU.REGS.QRS + c.CPU.REGS.X;
                    c.MEMC.Set16bitToRAM(addr, 0x00FF);
                    c.CPU.REGS.CD = 4;
                },
                c => Assert.Equal((ushort)0x0FF0, c.MEMC.Get16bitFromRAM(addr)));
        }

        [Fact]
        public void RL_mem_Irrr_rI_rrr() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (QRS+X),DEF",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.X = 3; addr = c.CPU.REGS.QRS + c.CPU.REGS.X;
                    c.MEMC.Set24bitToRAM(addr, 0x0000FF);
                    c.CPU.REGS.DEF = 4;
                },
                c => Assert.Equal((uint)0x000FF0, c.MEMC.Get24bitFromRAM(addr)));
        }

        [Fact]
        public void RL_mem_Irrr_rI_rrrr() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (QRS+X),EFGH",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.X = 3; addr = c.CPU.REGS.QRS + c.CPU.REGS.X;
                    c.MEMC.Set32bitToRAM(addr, 0x000000FF);
                    c.CPU.REGS.EFGH = 4;
                },
                c => Assert.Equal((uint)0x00000FF0, c.MEMC.Get32bitFromRAM(addr)));
        }

        [Fact]
        public void RL_mem_Irrr_rI_value_InnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (QRS+X),(0x7000),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.X = 3; addr = c.CPU.REGS.QRS + c.CPU.REGS.X;
                    valueAddr = 0x7000;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Irrr_rI_value_Innn_nnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (QRS+X),(0x7000+4),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.X = 3; addr = c.CPU.REGS.QRS + c.CPU.REGS.X;
                    valueAddr = 0x7004;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Irrr_rI_value_Innn_rI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (QRS+X),(0x7000+X),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.X = 3; addr = c.CPU.REGS.QRS + c.CPU.REGS.X;
                    c.CPU.REGS.X = 3; valueAddr = (uint)(0x7000 + c.CPU.REGS.X);
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Irrr_rI_value_Innn_rrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (QRS+A),(0x7000+WX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.A = 3; addr = c.CPU.REGS.QRS + c.CPU.REGS.A;
                    c.CPU.REGS.WX = 5; valueAddr = (uint)(0x7000 + c.CPU.REGS.WX);
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Irrr_rI_value_Innn_rrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (QRS+A),(0x7000+VWX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.A = 3; addr = c.CPU.REGS.QRS + c.CPU.REGS.A;
                    c.CPU.REGS.VWX = 7; valueAddr = (uint)(0x7000 + c.CPU.REGS.VWX);
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Irrr_rI_value_IrrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (QRS+X),(TUV),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.X = 3; addr = c.CPU.REGS.QRS + c.CPU.REGS.X;
                    c.CPU.REGS.TUV = 0x8000; valueAddr = c.CPU.REGS.TUV;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Irrr_rI_value_Irrr_nnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (QRS+X),(TUV+4),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.X = 3; addr = c.CPU.REGS.QRS + c.CPU.REGS.X;
                    c.CPU.REGS.TUV = 0x8000; valueAddr = c.CPU.REGS.TUV + 4;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Irrr_rI_value_Irrr_rI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (QRS+X),(TUV+X),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.X = 3; addr = c.CPU.REGS.QRS + c.CPU.REGS.X;
                    c.CPU.REGS.TUV = 0x8000; c.CPU.REGS.X = 3; valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.X;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Irrr_rI_value_Irrr_rrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (QRS+A),(TUV+WX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.A = 3; addr = c.CPU.REGS.QRS + c.CPU.REGS.A;
                    c.CPU.REGS.TUV = 0x8000; c.CPU.REGS.WX = 5; valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.WX;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Irrr_rI_value_Irrr_rrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (QRS+A),(TUV+VWX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.A = 3; addr = c.CPU.REGS.QRS + c.CPU.REGS.A;
                    c.CPU.REGS.TUV = 0x8000; c.CPU.REGS.VWX = 7; valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.VWX;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Irrr_rI_fr() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (QRS+X),F0",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.X = 3; addr = c.CPU.REGS.QRS + c.CPU.REGS.X;
                    c.MEMC.SetFloatToRam(addr, 1.0f);
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                },
                c =>
                {
                    float expected = ShiftFloatBits(1.0f, 1);
                    Assert.Equal(expected, c.MEMC.GetFloatFromRAM(addr));
                });
        }

        // --------------------
        // Target: (QRS+WX)
        // --------------------

        [Fact]
        public void RL_mem_Irrr_rrI_block_immediate_2bytes_once() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (QRS+WX),0x00000002,2",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.WX = 5; addr = c.CPU.REGS.QRS + c.CPU.REGS.WX;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                },
                c => Assert.Equal([0x01, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Irrr_rrI_block_immediate_2bytes_twice() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (QRS+WX),0x00000002,2,2",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.WX = 5; addr = c.CPU.REGS.QRS + c.CPU.REGS.WX;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Irrr_rrI_r() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (QRS+WX),B",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.WX = 5; addr = c.CPU.REGS.QRS + c.CPU.REGS.WX;
                    c.MEMC.Set8bitToRAM(addr, 0x0F);
                    c.CPU.REGS.B = 4;
                },
                c => Assert.Equal((byte)0xF0, c.MEMC.Get8bitFromRAM(addr)));
        }

        [Fact]
        public void RL_mem_Irrr_rrI_rr() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (QRS+WX),CD",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.WX = 5; addr = c.CPU.REGS.QRS + c.CPU.REGS.WX;
                    c.MEMC.Set16bitToRAM(addr, 0x00FF);
                    c.CPU.REGS.CD = 4;
                },
                c => Assert.Equal((ushort)0x0FF0, c.MEMC.Get16bitFromRAM(addr)));
        }

        [Fact]
        public void RL_mem_Irrr_rrI_rrr() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (QRS+WX),DEF",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.WX = 5; addr = c.CPU.REGS.QRS + c.CPU.REGS.WX;
                    c.MEMC.Set24bitToRAM(addr, 0x0000FF);
                    c.CPU.REGS.DEF = 4;
                },
                c => Assert.Equal((uint)0x000FF0, c.MEMC.Get24bitFromRAM(addr)));
        }

        [Fact]
        public void RL_mem_Irrr_rrI_rrrr() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (QRS+WX),EFGH",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.WX = 5; addr = c.CPU.REGS.QRS + c.CPU.REGS.WX;
                    c.MEMC.Set32bitToRAM(addr, 0x000000FF);
                    c.CPU.REGS.EFGH = 4;
                },
                c => Assert.Equal((uint)0x00000FF0, c.MEMC.Get32bitFromRAM(addr)));
        }

        [Fact]
        public void RL_mem_Irrr_rrI_value_InnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (QRS+WX),(0x7000),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.WX = 5; addr = c.CPU.REGS.QRS + c.CPU.REGS.WX;
                    valueAddr = 0x7000;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Irrr_rrI_value_Innn_nnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (QRS+WX),(0x7000+4),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.WX = 5; addr = c.CPU.REGS.QRS + c.CPU.REGS.WX;
                    valueAddr = 0x7004;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Irrr_rrI_value_Innn_rI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (QRS+WX),(0x7000+Y),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.WX = 5; addr = c.CPU.REGS.QRS + c.CPU.REGS.WX;
                    c.CPU.REGS.Y = 3; valueAddr = (uint)(0x7000 + c.CPU.REGS.Y);
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Irrr_rrI_value_Innn_rrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (QRS+WX),(0x7000+WX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.WX = 5; addr = c.CPU.REGS.QRS + c.CPU.REGS.WX;
                    c.CPU.REGS.WX = 5; valueAddr = (uint)(0x7000 + c.CPU.REGS.WX);
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Irrr_rrI_value_Innn_rrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (QRS+AB),(0x7000+VWX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.AB = 5; addr = c.CPU.REGS.QRS + c.CPU.REGS.AB;
                    c.CPU.REGS.VWX = 7; valueAddr = (uint)(0x7000 + c.CPU.REGS.VWX);
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Irrr_rrI_value_IrrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (QRS+WX),(TUV),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.WX = 5; addr = c.CPU.REGS.QRS + c.CPU.REGS.WX;
                    c.CPU.REGS.TUV = 0x8000; valueAddr = c.CPU.REGS.TUV;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Irrr_rrI_value_Irrr_nnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (QRS+WX),(TUV+4),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.WX = 5; addr = c.CPU.REGS.QRS + c.CPU.REGS.WX;
                    c.CPU.REGS.TUV = 0x8000; valueAddr = c.CPU.REGS.TUV + 4;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Irrr_rrI_value_Irrr_rI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (QRS+WX),(TUV+Y),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.WX = 5; addr = c.CPU.REGS.QRS + c.CPU.REGS.WX;
                    c.CPU.REGS.TUV = 0x8000; c.CPU.REGS.Y = 3; valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.Y;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Irrr_rrI_value_Irrr_rrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (QRS+WX),(TUV+WX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.WX = 5; addr = c.CPU.REGS.QRS + c.CPU.REGS.WX;
                    c.CPU.REGS.TUV = 0x8000; c.CPU.REGS.WX = 5; valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.WX;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Irrr_rrI_value_Irrr_rrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (QRS+AB),(TUV+VWX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.AB = 5; addr = c.CPU.REGS.QRS + c.CPU.REGS.AB;
                    c.CPU.REGS.TUV = 0x8000; c.CPU.REGS.VWX = 7; valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.VWX;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Irrr_rrI_fr() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (QRS+WX),F0",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.WX = 5; addr = c.CPU.REGS.QRS + c.CPU.REGS.WX;
                    c.MEMC.SetFloatToRam(addr, 1.0f);
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                },
                c =>
                {
                    float expected = ShiftFloatBits(1.0f, 1);
                    Assert.Equal(expected, c.MEMC.GetFloatFromRAM(addr));
                });
        }

        // --------------------
        // Target: (QRS+VWX)
        // --------------------

        [Fact]
        public void RL_mem_Irrr_rrrI_block_immediate_2bytes_once() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (QRS+VWX),0x00000002,2",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.VWX = 7; addr = c.CPU.REGS.QRS + c.CPU.REGS.VWX;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                },
                c => Assert.Equal([0x01, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Irrr_rrrI_block_immediate_2bytes_twice() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (QRS+VWX),0x00000002,2,2",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.VWX = 7; addr = c.CPU.REGS.QRS + c.CPU.REGS.VWX;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Irrr_rrrI_r() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (QRS+VWX),B",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.VWX = 7; addr = c.CPU.REGS.QRS + c.CPU.REGS.VWX;
                    c.MEMC.Set8bitToRAM(addr, 0x0F);
                    c.CPU.REGS.B = 4;
                },
                c => Assert.Equal((byte)0xF0, c.MEMC.Get8bitFromRAM(addr)));
        }

        [Fact]
        public void RL_mem_Irrr_rrrI_rr() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (QRS+VWX),CD",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.VWX = 7; addr = c.CPU.REGS.QRS + c.CPU.REGS.VWX;
                    c.MEMC.Set16bitToRAM(addr, 0x00FF);
                    c.CPU.REGS.CD = 4;
                },
                c => Assert.Equal((ushort)0x0FF0, c.MEMC.Get16bitFromRAM(addr)));
        }

        [Fact]
        public void RL_mem_Irrr_rrrI_rrr() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (QRS+VWX),DEF",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.VWX = 7; addr = c.CPU.REGS.QRS + c.CPU.REGS.VWX;
                    c.MEMC.Set24bitToRAM(addr, 0x0000FF);
                    c.CPU.REGS.DEF = 4;
                },
                c => Assert.Equal((uint)0x000FF0, c.MEMC.Get24bitFromRAM(addr)));
        }

        [Fact]
        public void RL_mem_Irrr_rrrI_rrrr() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (QRS+VWX),EFGH",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.VWX = 7; addr = c.CPU.REGS.QRS + c.CPU.REGS.VWX;
                    c.MEMC.Set32bitToRAM(addr, 0x000000FF);
                    c.CPU.REGS.EFGH = 4;
                },
                c => Assert.Equal((uint)0x00000FF0, c.MEMC.Get32bitFromRAM(addr)));
        }

        [Fact]
        public void RL_mem_Irrr_rrrI_value_InnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (QRS+VWX),(0x7000),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.VWX = 7; addr = c.CPU.REGS.QRS + c.CPU.REGS.VWX;
                    valueAddr = 0x7000;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Irrr_rrrI_value_Innn_nnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (QRS+VWX),(0x7000+4),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.VWX = 7; addr = c.CPU.REGS.QRS + c.CPU.REGS.VWX;
                    valueAddr = 0x7004;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Irrr_rrrI_value_Innn_rI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (QRS+VWX),(0x7000+Y),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.VWX = 7; addr = c.CPU.REGS.QRS + c.CPU.REGS.VWX;
                    c.CPU.REGS.Y = 3; valueAddr = (uint)(0x7000 + c.CPU.REGS.Y);
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Irrr_rrrI_value_Innn_rrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (QRS+VWX),(0x7000+YZ),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.VWX = 7; addr = c.CPU.REGS.QRS + c.CPU.REGS.VWX;
                    c.CPU.REGS.YZ = 5; valueAddr = (uint)(0x7000 + c.CPU.REGS.YZ);
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Irrr_rrrI_value_Innn_rrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (QRS+VWX),(0x7000+VWX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.VWX = 7; addr = c.CPU.REGS.QRS + c.CPU.REGS.VWX;
                    c.CPU.REGS.VWX = 7; valueAddr = (uint)(0x7000 + c.CPU.REGS.VWX);
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Irrr_rrrI_value_IrrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (QRS+VWX),(TUV),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.VWX = 7; addr = c.CPU.REGS.QRS + c.CPU.REGS.VWX;
                    c.CPU.REGS.TUV = 0x8000; valueAddr = c.CPU.REGS.TUV;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Irrr_rrrI_value_Irrr_nnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (QRS+VWX),(TUV+4),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.VWX = 7; addr = c.CPU.REGS.QRS + c.CPU.REGS.VWX;
                    c.CPU.REGS.TUV = 0x8000; valueAddr = c.CPU.REGS.TUV + 4;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Irrr_rrrI_value_Irrr_rI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (QRS+VWX),(TUV+Y),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.VWX = 7; addr = c.CPU.REGS.QRS + c.CPU.REGS.VWX;
                    c.CPU.REGS.TUV = 0x8000; c.CPU.REGS.Y = 3; valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.Y;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Irrr_rrrI_value_Irrr_rrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (QRS+VWX),(TUV+YZ),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.VWX = 7; addr = c.CPU.REGS.QRS + c.CPU.REGS.VWX;
                    c.CPU.REGS.TUV = 0x8000; c.CPU.REGS.YZ = 5; valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.YZ;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Irrr_rrrI_value_Irrr_rrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "RL (QRS+VWX),(TUV+VWX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.VWX = 7; addr = c.CPU.REGS.QRS + c.CPU.REGS.VWX;
                    c.CPU.REGS.TUV = 0x8000; c.CPU.REGS.VWX = 7; valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.VWX;
                    c.LoadMemAt(addr, [0x00, 0x40]);
                    c.LoadMemAt(valueAddr, [0x02]);
                },
                c => Assert.Equal([0x04, 0x00], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void RL_mem_Irrr_rrrI_fr() // Ok
        {
            uint addr = 0;

            RunTest(
                "RL (QRS+VWX),F0",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.VWX = 7; addr = c.CPU.REGS.QRS + c.CPU.REGS.VWX;
                    c.MEMC.SetFloatToRam(addr, 1.0f);
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                },
                c =>
                {
                    float expected = ShiftFloatBits(1.0f, 1);
                    Assert.Equal(expected, c.MEMC.GetFloatFromRAM(addr));
                });
        }

        #endregion

        #region SL float register destination variants

        [Fact]
        public void RL_fr_fr() // Ok
        {
            RunTest(
                "RL F0,F1",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                    c.CPU.FREGS.SetRegister(1, 1.0f);
                },
                c =>
                {
                    float expected = ShiftFloatBits(1.0f, 1);
                    Assert.Equal(expected, c.CPU.FREGS.GetRegister(0));
                });
        }

        [Fact]
        public void RL_fr_nnnn() // Ok
        {
            RunTest(
                "RL F0,0x00000001",
                c => c.CPU.FREGS.SetRegister(0, 1.0f),
                c =>
                {
                    float expected = ShiftFloatBits(1.0f, 1);
                    Assert.Equal(expected, c.CPU.FREGS.GetRegister(0));
                });
        }

        [Fact]
        public void RL_fr_r() // Ok
        {
            RunTest(
                "RL F0,A",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                    c.CPU.REGS.A = 1;
                },
                c =>
                {
                    float expected = ShiftFloatBits(1.0f, 1);
                    Assert.Equal(expected, c.CPU.FREGS.GetRegister(0));
                });
        }

        [Fact]
        public void RL_fr_rr() // Ok
        {
            RunTest(
                "RL F0,AB",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                    c.CPU.REGS.AB = 1;
                },
                c =>
                {
                    float expected = ShiftFloatBits(1.0f, 1);
                    Assert.Equal(expected, c.CPU.FREGS.GetRegister(0));
                });
        }

        [Fact]
        public void RL_fr_rrr() // Ok
        {
            RunTest(
                "RL F0,ABC",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                    c.CPU.REGS.ABC = 1;
                },
                c =>
                {
                    float expected = ShiftFloatBits(1.0f, 1);
                    Assert.Equal(expected, c.CPU.FREGS.GetRegister(0));
                });
        }

        [Fact]
        public void RL_fr_rrrr() // Ok
        {
            RunTest(
                "RL F0,ABCD",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                    c.CPU.REGS.ABCD = 1;
                },
                c =>
                {
                    float expected = ShiftFloatBits(1.0f, 1);
                    Assert.Equal(expected, c.CPU.FREGS.GetRegister(0));
                });
        }

        [Fact]
        public void RL_fr_InnnI() // Ok
        {
            RunTest(
                "RL F0,(0x4000)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                    c.MEMC.Set8bitToRAM(0x4000, 1);
                },
                c =>
                {
                    float expected = ShiftFloatBits(1.0f, 1);
                    Assert.Equal(expected, c.CPU.FREGS.GetRegister(0));
                });
        }

        [Fact]
        public void RL_fr_Innn_nnnI() // Ok
        {
            RunTest(
                "RL F0,(0x4000+4)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                    c.MEMC.Set8bitToRAM(0x4004, 1);
                },
                c =>
                {
                    float expected = ShiftFloatBits(1.0f, 1);
                    Assert.Equal(expected, c.CPU.FREGS.GetRegister(0));
                });
        }

        [Fact]
        public void RL_fr_Innn_rI() // Ok
        {
            RunTest(
                "RL F0,(0x4000+X)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set8bitToRAM(0x4003, 1);
                },
                c =>
                {
                    float expected = ShiftFloatBits(1.0f, 1);
                    Assert.Equal(expected, c.CPU.FREGS.GetRegister(0));
                });
        }

        [Fact]
        public void RL_fr_Innn_rrI() // Ok
        {
            RunTest(
                "RL F0,(0x4000+WX)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x4005, 1);
                },
                c =>
                {
                    float expected = ShiftFloatBits(1.0f, 1);
                    Assert.Equal(expected, c.CPU.FREGS.GetRegister(0));
                });
        }

        [Fact]
        public void RL_fr_Innn_rrrI() // Ok
        {
            RunTest(
                "RL F0,(0x4000+VWX)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x4007, 1);
                },
                c =>
                {
                    float expected = ShiftFloatBits(1.0f, 1);
                    Assert.Equal(expected, c.CPU.FREGS.GetRegister(0));
                });
        }

        [Fact]
        public void RL_fr_IrrrI() // Ok
        {
            RunTest(
                "RL F0,(QRS)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x5000, 1);
                },
                c =>
                {
                    float expected = ShiftFloatBits(1.0f, 1);
                    Assert.Equal(expected, c.CPU.FREGS.GetRegister(0));
                });
        }

        [Fact]
        public void RL_fr_Irrr_nnnI() // Ok
        {
            RunTest(
                "RL F0,(QRS+4)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x5004, 1);
                },
                c =>
                {
                    float expected = ShiftFloatBits(1.0f, 1);
                    Assert.Equal(expected, c.CPU.FREGS.GetRegister(0));
                });
        }

        [Fact]
        public void RL_fr_Irrr_rI() // Ok
        {
            RunTest(
                "RL F0,(QRS+X)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set8bitToRAM(0x5003, 1);
                },
                c =>
                {
                    float expected = ShiftFloatBits(1.0f, 1);
                    Assert.Equal(expected, c.CPU.FREGS.GetRegister(0));
                });
        }

        [Fact]
        public void RL_fr_Irrr_rrI() // Ok
        {
            RunTest(
                "RL F0,(QRS+WX)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x5005, 1);
                },
                c =>
                {
                    float expected = ShiftFloatBits(1.0f, 1);
                    Assert.Equal(expected, c.CPU.FREGS.GetRegister(0));
                });
        }

        [Fact]
        public void RL_fr_Irrr_rrrI() // Ok
        {
            RunTest(
                "RL F0,(QRS+VWX)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x5007, 1);
                },
                c =>
                {
                    float expected = ShiftFloatBits(1.0f, 1);
                    Assert.Equal(expected, c.CPU.FREGS.GetRegister(0));
                });
        }

        #endregion
    }
}
