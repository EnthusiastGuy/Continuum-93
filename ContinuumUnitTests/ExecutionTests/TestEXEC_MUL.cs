using Continuum93.Emulator;
using Continuum93.Emulator.Interpreter;

namespace ExecutionTests
{
    public class TestEXEC_MUL
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

            byte[] toRun = cp.GetCompiledCode();
            computer.LoadMem(toRun);
            computer.Run();

            assert(computer);
            TUtils.IncrementCountedTests("exec");
        }

        #endregion

        #region MUL r,* tests

        [Fact]
        public void MUL_r_n() // Ok
        {
            RunTest(
                "MUL A,5",
                c => c.CPU.REGS.A = 10,
                c => Assert.Equal((byte)50, c.CPU.REGS.A));
        }

        [Fact]
        public void MUL_r_r() // Ok
        {
            RunTest(
                "MUL A,B",
                c =>
                {
                    c.CPU.REGS.A = 10;
                    c.CPU.REGS.B = 5;
                },
                c => Assert.Equal((byte)50, c.CPU.REGS.A));
        }

        [Fact]
        public void MUL_r_InnnI() // Ok
        {
            RunTest(
                "MUL A,(0x4000)",
                c =>
                {
                    c.CPU.REGS.A = 10;
                    c.MEMC.Set8bitToRAM(0x4000, 5);
                },
                c => Assert.Equal((byte)50, c.CPU.REGS.A));
        }

        [Fact]
        public void MUL_r_Innn_nnnI() // Ok
        {
            RunTest(
                "MUL A,(0x4000+4)",
                c =>
                {
                    c.CPU.REGS.A = 10;
                    c.MEMC.Set8bitToRAM(0x4004, 5);
                },
                c => Assert.Equal((byte)50, c.CPU.REGS.A));
        }

        [Fact]
        public void MUL_r_Innn_rI() // Ok
        {
            RunTest(
                "MUL A,(0x4000+X)",
                c =>
                {
                    c.CPU.REGS.A = 10;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set8bitToRAM(0x4003, 5);
                },
                c => Assert.Equal((byte)50, c.CPU.REGS.A));
        }

        [Fact]
        public void MUL_r_Innn_rrI() // Ok
        {
            RunTest(
                "MUL A,(0x4000+WX)",
                c =>
                {
                    c.CPU.REGS.A = 10;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x4005, 5);
                },
                c => Assert.Equal((byte)50, c.CPU.REGS.A));
        }

        [Fact]
        public void MUL_r_Innn_rrrI() // Ok
        {
            RunTest(
                "MUL A,(0x4000+VWX)",
                c =>
                {
                    c.CPU.REGS.A = 10;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x4007, 5);
                },
                c => Assert.Equal((byte)50, c.CPU.REGS.A));
        }

        [Fact]
        public void MUL_r_IrrrI() // Ok
        {
            RunTest(
                "MUL A,(QRS)",
                c =>
                {
                    c.CPU.REGS.A = 10;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x5000, 5);
                },
                c => Assert.Equal((byte)50, c.CPU.REGS.A));
        }

        [Fact]
        public void MUL_r_Irrr_nnnI() // Ok
        {
            RunTest(
                "MUL A,(QRS+4)",
                c =>
                {
                    c.CPU.REGS.A = 10;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x5004, 5);
                },
                c => Assert.Equal((byte)50, c.CPU.REGS.A));
        }

        [Fact]
        public void MUL_r_Irrr_rI() // Ok
        {
            RunTest(
                "MUL A,(QRS+X)",
                c =>
                {
                    c.CPU.REGS.A = 10;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set8bitToRAM(0x5003, 5);
                },
                c => Assert.Equal((byte)50, c.CPU.REGS.A));
        }

        [Fact]
        public void MUL_r_Irrr_rrI() // Ok
        {
            RunTest(
                "MUL A,(QRS+WX)",
                c =>
                {
                    c.CPU.REGS.A = 10;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x5005, 5);
                },
                c => Assert.Equal((byte)50, c.CPU.REGS.A));
        }

        [Fact]
        public void MUL_r_Irrr_rrrI() // Ok
        {
            RunTest(
                "MUL A,(QRS+VWX)",
                c =>
                {
                    c.CPU.REGS.A = 10;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x5007, 5);
                },
                c => Assert.Equal((byte)50, c.CPU.REGS.A));
        }

        [Fact]
        public void MUL_r_fr() // Ok
        {
            RunTest(
                "MUL A,F0",
                c =>
                {
                    c.CPU.REGS.A = 10;
                    c.CPU.FREGS.SetRegister(0, 2.5f);
                },
                c => Assert.Equal((byte)25, c.CPU.REGS.A));
        }

        #endregion

        #region MUL rr,* tests

        [Fact]
        public void MUL_rr_nn() // Ok
        {
            RunTest(
                "MUL AB,10",
                c => c.CPU.REGS.AB = 100,
                c => Assert.Equal((ushort)1000, c.CPU.REGS.AB));
        }

        [Fact]
        public void MUL_rr_r() // Ok
        {
            RunTest(
                "MUL AB,E",
                c =>
                {
                    c.CPU.REGS.AB = 100;
                    c.CPU.REGS.E = 10;
                },
                c => Assert.Equal((ushort)1000, c.CPU.REGS.AB));
        }

        [Fact]
        public void MUL_rr_rr() // Ok
        {
            RunTest(
                "MUL AB,CD",
                c =>
                {
                    c.CPU.REGS.AB = 100;
                    c.CPU.REGS.CD = 10;
                },
                c => Assert.Equal((ushort)1000, c.CPU.REGS.AB));
        }

        [Fact]
        public void MUL_rr_InnnI() // Ok
        {
            RunTest(
                "MUL AB,(0x4000)",
                c =>
                {
                    c.CPU.REGS.AB = 100;
                    c.MEMC.Set16bitToRAM(0x4000, 10);
                },
                c => Assert.Equal((ushort)1000, c.CPU.REGS.AB));
        }

        [Fact]
        public void MUL_rr_Innn_nnnI() // Ok
        {
            RunTest(
                "MUL AB,(0x4000+4)",
                c =>
                {
                    c.CPU.REGS.AB = 100;
                    c.MEMC.Set16bitToRAM(0x4004, 10);
                },
                c => Assert.Equal((ushort)1000, c.CPU.REGS.AB));
        }

        [Fact]
        public void MUL_rr_Innn_rI() // Ok
        {
            RunTest(
                "MUL AB,(0x4000+X)",
                c =>
                {
                    c.CPU.REGS.AB = 100;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set16bitToRAM(0x4003, 10);
                },
                c => Assert.Equal((ushort)1000, c.CPU.REGS.AB));
        }

        [Fact]
        public void MUL_rr_Innn_rrI() // Ok
        {
            RunTest(
                "MUL AB,(0x4000+WX)",
                c =>
                {
                    c.CPU.REGS.AB = 100;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set16bitToRAM(0x4005, 10);
                },
                c => Assert.Equal((ushort)1000, c.CPU.REGS.AB));
        }

        [Fact]
        public void MUL_rr_Innn_rrrI() // Ok
        {
            RunTest(
                "MUL AB,(0x4000+VWX)",
                c =>
                {
                    c.CPU.REGS.AB = 100;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set16bitToRAM(0x4007, 10);
                },
                c => Assert.Equal((ushort)1000, c.CPU.REGS.AB));
        }

        [Fact]
        public void MUL_rr_IrrrI() // Ok
        {
            RunTest(
                "MUL AB,(QRS)",
                c =>
                {
                    c.CPU.REGS.AB = 100;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set16bitToRAM(0x5000, 10);
                },
                c => Assert.Equal((ushort)1000, c.CPU.REGS.AB));
        }

        [Fact]
        public void MUL_rr_Irrr_nnnI() // Ok
        {
            RunTest(
                "MUL AB,(QRS+4)",
                c =>
                {
                    c.CPU.REGS.AB = 100;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set16bitToRAM(0x5004, 10);
                },
                c => Assert.Equal((ushort)1000, c.CPU.REGS.AB));
        }

        [Fact]
        public void MUL_rr_Irrr_rI() // Ok
        {
            RunTest(
                "MUL AB,(QRS+X)",
                c =>
                {
                    c.CPU.REGS.AB = 100;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set16bitToRAM(0x5003, 10);
                },
                c => Assert.Equal((ushort)1000, c.CPU.REGS.AB));
        }

        [Fact]
        public void MUL_rr_Irrr_rrI() // Ok
        {
            RunTest(
                "MUL AB,(QRS+WX)",
                c =>
                {
                    c.CPU.REGS.AB = 100;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set16bitToRAM(0x5005, 10);
                },
                c => Assert.Equal((ushort)1000, c.CPU.REGS.AB));
        }

        [Fact]
        public void MUL_rr_Irrr_rrrI() // Ok
        {
            RunTest(
                "MUL AB,(QRS+VWX)",
                c =>
                {
                    c.CPU.REGS.AB = 100;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set16bitToRAM(0x5007, 10);
                },
                c => Assert.Equal((ushort)1000, c.CPU.REGS.AB));
        }

        [Fact]
        public void MUL_rr_fr() // Ok
        {
            RunTest(
                "MUL AB,F1",
                c =>
                {
                    c.CPU.REGS.AB = 100;
                    c.CPU.FREGS.SetRegister(1, 2.5f);
                },
                c => Assert.Equal((ushort)250, c.CPU.REGS.AB));
        }

        #endregion

        #region MUL rrr,* tests

        [Fact]
        public void MUL_rrr_nnn() // Ok
        {
            const uint multiplicand = 10000;
            const uint expected = 100000;

            RunTest(
                "MUL ABC,10",
                c => c.CPU.REGS.ABC = multiplicand,
                c => Assert.Equal(expected, c.CPU.REGS.ABC));
        }

        [Fact]
        public void MUL_rrr_r() // Ok
        {
            const uint multiplicand = 10000;
            const uint expected = 100000;

            RunTest(
                "MUL ABC,D",
                c =>
                {
                    c.CPU.REGS.ABC = multiplicand;
                    c.CPU.REGS.D = 10;
                },
                c => Assert.Equal(expected, c.CPU.REGS.ABC));
        }

        [Fact]
        public void MUL_rrr_rr() // Ok
        {
            const uint multiplicand = 10000;
            const uint expected = 100000;

            RunTest(
                "MUL ABC,DE",
                c =>
                {
                    c.CPU.REGS.ABC = multiplicand;
                    c.CPU.REGS.DE = 10;
                },
                c => Assert.Equal(expected, c.CPU.REGS.ABC));
        }

        [Fact]
        public void MUL_rrr_rrr() // Ok
        {
            const uint multiplicand = 10000;
            const uint expected = 100000;

            RunTest(
                "MUL ABC,DEF",
                c =>
                {
                    c.CPU.REGS.ABC = multiplicand;
                    c.CPU.REGS.DEF = 10;
                },
                c => Assert.Equal(expected, c.CPU.REGS.ABC));
        }

        [Fact]
        public void MUL_rrr_InnnI() // Ok
        {
            const uint multiplicand = 10000;
            const uint expected = 100000;

            RunTest(
                "MUL ABC,(0x4000)",
                c =>
                {
                    c.CPU.REGS.ABC = multiplicand;
                    c.MEMC.Set24bitToRAM(0x4000, 10);
                },
                c => Assert.Equal(expected, c.CPU.REGS.ABC));
        }

        [Fact]
        public void MUL_rrr_Innn_nnnI() // Ok
        {
            const uint multiplicand = 10000;
            const uint expected = 100000;

            RunTest(
                "MUL ABC,(0x4000+4)",
                c =>
                {
                    c.CPU.REGS.ABC = multiplicand;
                    c.MEMC.Set24bitToRAM(0x4004, 10);
                },
                c => Assert.Equal(expected, c.CPU.REGS.ABC));
        }

        [Fact]
        public void MUL_rrr_Innn_rI() // Ok
        {
            const uint multiplicand = 10000;
            const uint expected = 100000;

            RunTest(
                "MUL ABC,(0x4000+X)",
                c =>
                {
                    c.CPU.REGS.ABC = multiplicand;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set24bitToRAM(0x4003, 10);
                },
                c => Assert.Equal(expected, c.CPU.REGS.ABC));
        }

        [Fact]
        public void MUL_rrr_Innn_rrI() // Ok
        {
            const uint multiplicand = 10000;
            const uint expected = 100000;

            RunTest(
                "MUL ABC,(0x4000+WX)",
                c =>
                {
                    c.CPU.REGS.ABC = multiplicand;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set24bitToRAM(0x4005, 10);
                },
                c => Assert.Equal(expected, c.CPU.REGS.ABC));
        }

        [Fact]
        public void MUL_rrr_Innn_rrrI() // Ok
        {
            const uint multiplicand = 10000;
            const uint expected = 100000;

            RunTest(
                "MUL ABC,(0x4000+VWX)",
                c =>
                {
                    c.CPU.REGS.ABC = multiplicand;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set24bitToRAM(0x4007, 10);
                },
                c => Assert.Equal(expected, c.CPU.REGS.ABC));
        }

        [Fact]
        public void MUL_rrr_IrrrI() // Ok
        {
            const uint multiplicand = 10000;
            const uint expected = 100000;

            RunTest(
                "MUL ABC,(QRS)",
                c =>
                {
                    c.CPU.REGS.ABC = multiplicand;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set24bitToRAM(0x5000, 10);
                },
                c => Assert.Equal(expected, c.CPU.REGS.ABC));
        }

        [Fact]
        public void MUL_rrr_Irrr_nnnI() // Ok
        {
            const uint multiplicand = 10000;
            const uint expected = 100000;

            RunTest(
                "MUL ABC,(QRS+4)",
                c =>
                {
                    c.CPU.REGS.ABC = multiplicand;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set24bitToRAM(0x5004, 10);
                },
                c => Assert.Equal(expected, c.CPU.REGS.ABC));
        }

        [Fact]
        public void MUL_rrr_Irrr_rI() // Ok
        {
            const uint multiplicand = 10000;
            const uint expected = 100000;

            RunTest(
                "MUL ABC,(QRS+X)",
                c =>
                {
                    c.CPU.REGS.ABC = multiplicand;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set24bitToRAM(0x5003, 10);
                },
                c => Assert.Equal(expected, c.CPU.REGS.ABC));
        }

        [Fact]
        public void MUL_rrr_Irrr_rrI() // Ok
        {
            const uint multiplicand = 10000;
            const uint expected = 100000;

            RunTest(
                "MUL ABC,(QRS+WX)",
                c =>
                {
                    c.CPU.REGS.ABC = multiplicand;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set24bitToRAM(0x5005, 10);
                },
                c => Assert.Equal(expected, c.CPU.REGS.ABC));
        }

        [Fact]
        public void MUL_rrr_Irrr_rrrI() // Ok
        {
            const uint multiplicand = 10000;
            const uint expected = 100000;

            RunTest(
                "MUL ABC,(QRS+VWX)",
                c =>
                {
                    c.CPU.REGS.ABC = multiplicand;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set24bitToRAM(0x5007, 10);
                },
                c => Assert.Equal(expected, c.CPU.REGS.ABC));
        }

        [Fact]
        public void MUL_rrr_fr() // Ok
        {
            const uint multiplicand = 10000;
            const uint expected = 25000;

            RunTest(
                "MUL ABC,F2",
                c =>
                {
                    c.CPU.REGS.ABC = multiplicand;
                    c.CPU.FREGS.SetRegister(2, 2.5f);
                },
                c => Assert.Equal(expected, c.CPU.REGS.ABC));
        }

        #endregion

        #region MUL rrrr,* tests

        [Fact]
        public void MUL_rrrr_nnnn() // Ok
        {
            const uint multiplicand = 100_000;
            const uint expected = 1_000_000;

            RunTest(
                "MUL ABCD,10",
                c => c.CPU.REGS.ABCD = multiplicand,
                c => Assert.Equal(expected, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void MUL_rrrr_r() // Ok
        {
            const uint multiplicand = 100_000;
            const uint expected = 1_000_000;

            RunTest(
                "MUL ABCD,E",
                c =>
                {
                    c.CPU.REGS.ABCD = multiplicand;
                    c.CPU.REGS.E = 10;
                },
                c => Assert.Equal(expected, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void MUL_rrrr_rr() // Ok
        {
            const uint multiplicand = 100_000;
            const uint expected = 1_000_000;

            RunTest(
                "MUL ABCD,EF",
                c =>
                {
                    c.CPU.REGS.ABCD = multiplicand;
                    c.CPU.REGS.EF = 10;
                },
                c => Assert.Equal(expected, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void MUL_rrrr_rrr() // Ok
        {
            const uint multiplicand = 100_000;
            const uint expected = 1_000_000;

            RunTest(
                "MUL ABCD,EFG",
                c =>
                {
                    c.CPU.REGS.ABCD = multiplicand;
                    c.CPU.REGS.EFG = 10;
                },
                c => Assert.Equal(expected, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void MUL_rrrr_rrrr() // Ok
        {
            const uint multiplicand = 100_000;
            const uint expected = 1_000_000;

            RunTest(
                "MUL ABCD,EFGH",
                c =>
                {
                    c.CPU.REGS.ABCD = multiplicand;
                    c.CPU.REGS.EFGH = 10;
                },
                c => Assert.Equal(expected, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void MUL_rrrr_InnnI() // Ok
        {
            const uint multiplicand = 100_000;
            const uint expected = 1_000_000;

            RunTest(
                "MUL ABCD,(0x4000)",
                c =>
                {
                    c.CPU.REGS.ABCD = multiplicand;
                    c.MEMC.Set32bitToRAM(0x4000, 10);
                },
                c => Assert.Equal(expected, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void MUL_rrrr_Innn_nnnI() // Ok
        {
            const uint multiplicand = 100_000;
            const uint expected = 1_000_000;

            RunTest(
                "MUL ABCD,(0x4000+4)",
                c =>
                {
                    c.CPU.REGS.ABCD = multiplicand;
                    c.MEMC.Set32bitToRAM(0x4004, 10);
                },
                c => Assert.Equal(expected, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void MUL_rrrr_Innn_rI() // Ok
        {
            const uint multiplicand = 100_000;
            const uint expected = 1_000_000;

            RunTest(
                "MUL ABCD,(0x4000+X)",
                c =>
                {
                    c.CPU.REGS.ABCD = multiplicand;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set32bitToRAM(0x4003, 10);
                },
                c => Assert.Equal(expected, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void MUL_rrrr_Innn_rrI() // Ok
        {
            const uint multiplicand = 100_000;
            const uint expected = 1_000_000;

            RunTest(
                "MUL ABCD,(0x4000+WX)",
                c =>
                {
                    c.CPU.REGS.ABCD = multiplicand;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set32bitToRAM(0x4005, 10);
                },
                c => Assert.Equal(expected, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void MUL_rrrr_Innn_rrrI() // Ok
        {
            const uint multiplicand = 100_000;
            const uint expected = 1_000_000;

            RunTest(
                "MUL ABCD,(0x4000+VWX)",
                c =>
                {
                    c.CPU.REGS.ABCD = multiplicand;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set32bitToRAM(0x4007, 10);
                },
                c => Assert.Equal(expected, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void MUL_rrrr_IrrrI() // Ok
        {
            const uint multiplicand = 100_000;
            const uint expected = 1_000_000;

            RunTest(
                "MUL ABCD,(QRS)",
                c =>
                {
                    c.CPU.REGS.ABCD = multiplicand;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set32bitToRAM(0x5000, 10);
                },
                c => Assert.Equal(expected, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void MUL_rrrr_Irrr_nnnI() // Ok
        {
            const uint multiplicand = 100_000;
            const uint expected = 1_000_000;

            RunTest(
                "MUL ABCD,(QRS+4)",
                c =>
                {
                    c.CPU.REGS.ABCD = multiplicand;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set32bitToRAM(0x5004, 10);
                },
                c => Assert.Equal(expected, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void MUL_rrrr_Irrr_rI() // Ok
        {
            const uint multiplicand = 100_000;
            const uint expected = 1_000_000;

            RunTest(
                "MUL ABCD,(QRS+X)",
                c =>
                {
                    c.CPU.REGS.ABCD = multiplicand;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set32bitToRAM(0x5003, 10);
                },
                c => Assert.Equal(expected, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void MUL_rrrr_Irrr_rrI() // Ok
        {
            const uint multiplicand = 100_000;
            const uint expected = 1_000_000;

            RunTest(
                "MUL ABCD,(QRS+WX)",
                c =>
                {
                    c.CPU.REGS.ABCD = multiplicand;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set32bitToRAM(0x5005, 10);
                },
                c => Assert.Equal(expected, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void MUL_rrrr_Irrr_rrrI() // Ok
        {
            const uint multiplicand = 100_000;
            const uint expected = 1_000_000;

            RunTest(
                "MUL ABCD,(QRS+VWX)",
                c =>
                {
                    c.CPU.REGS.ABCD = multiplicand;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set32bitToRAM(0x5007, 10);
                },
                c => Assert.Equal(expected, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void MUL_rrrr_fr() // Ok
        {
            const uint multiplicand = 100_000;
            const uint expected = 250_000;

            RunTest(
                "MUL ABCD,F3",
                c =>
                {
                    c.CPU.REGS.ABCD = multiplicand;
                    c.CPU.FREGS.SetRegister(3, 2.5f);
                },
                c => Assert.Equal(expected, c.CPU.REGS.ABCD));
        }

        #endregion

        #region MUL memory destination variants (unwound: 10 targets Ã— 17 cases)

        // Important: index registers overlap (X, WX, VWX), so for combined addressing-mode tests
        // compute effective addresses after ALL setup is complete, mirroring the old loop-based tests.

        // --------------------
        // Target: (0x4000)
        // --------------------

        [Fact]
        public void MUL_mem_InnnI_block_immediate_2bytes_once() // Ok
        {
            const uint addr = 0x4000;

            RunTest(
                "MUL (0x4000),0x00000002,2",
                c => c.LoadMemAt(addr, [0x00, 0x0A]),
                c => Assert.Equal([0x00, 0x20], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_InnnI_block_immediate_2bytes_twice() // Ok
        {
            const uint addr = 0x4000;

            RunTest(
                "MUL (0x4000),0x00000002,2,2",
                c => c.LoadMemAt(addr, [0x00, 0x0A]),
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_InnnI_r() // Ok
        {
            const uint addr = 0x4000;

            RunTest(
                "MUL (0x4000),B",
                c =>
                {
                    c.MEMC.Set8bitToRAM(addr, 10);
                    c.CPU.REGS.B = 5;
                },
                c => Assert.Equal((byte)50, c.MEMC.Get8bitFromRAM(addr)));
        }

        [Fact]
        public void MUL_mem_InnnI_rr() // Ok
        {
            const uint addr = 0x4000;

            RunTest(
                "MUL (0x4000),CD",
                c =>
                {
                    c.MEMC.Set16bitToRAM(addr, 100);
                    c.CPU.REGS.CD = 10;
                },
                c => Assert.Equal((ushort)1000, c.MEMC.Get16bitFromRAM(addr)));
        }

        [Fact]
        public void MUL_mem_InnnI_rrr() // Ok
        {
            const uint addr = 0x4000;

            RunTest(
                "MUL (0x4000),DEF",
                c =>
                {
                    c.MEMC.Set24bitToRAM(addr, 10000);
                    c.CPU.REGS.DEF = 10;
                },
                c => Assert.Equal((uint)100000, c.MEMC.Get24bitFromRAM(addr)));
        }

        [Fact]
        public void MUL_mem_InnnI_rrrr() // Ok
        {
            const uint addr = 0x4000;

            RunTest(
                "MUL (0x4000),EFGH",
                c =>
                {
                    c.MEMC.Set32bitToRAM(addr, 100_000);
                    c.CPU.REGS.EFGH = 10;
                },
                c => Assert.Equal((uint)1_000_000, c.MEMC.Get32bitFromRAM(addr)));
        }

        [Fact]
        public void MUL_mem_InnnI_value_InnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (0x4000),(0x7000),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    // target setup: none
                    // value setup: none
                    addr = 0x4000;
                    valueAddr = 0x7000;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_InnnI_value_Innn_nnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (0x4000),(0x7000+4),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    addr = 0x4000;
                    valueAddr = 0x7004;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_InnnI_value_Innn_rI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (0x4000),(0x7000+X),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    // target setup: none
                    c.CPU.REGS.X = 3; // value setup
                    addr = 0x4000;
                    valueAddr = (uint)(0x7000 + c.CPU.REGS.X);
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_InnnI_value_Innn_rrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (0x4000),(0x7000+WX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.WX = 5; // value setup
                    addr = 0x4000;
                    valueAddr = (uint)(0x7000 + c.CPU.REGS.WX);
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_InnnI_value_Innn_rrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (0x4000),(0x7000+VWX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.VWX = 7; // value setup
                    addr = 0x4000;
                    valueAddr = (uint)(0x7000 + c.CPU.REGS.VWX);
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_InnnI_value_IrrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (0x4000),(TUV),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.TUV = 0x8000; // value setup
                    addr = 0x4000;
                    valueAddr = c.CPU.REGS.TUV;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_InnnI_value_Irrr_nnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (0x4000),(TUV+4),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.TUV = 0x8000; // value setup
                    addr = 0x4000;
                    valueAddr = c.CPU.REGS.TUV + 4;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_InnnI_value_Irrr_rI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (0x4000),(TUV+X),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.TUV = 0x8000; // value setup (part 1)
                    c.CPU.REGS.X = 3;        // value setup (part 2)
                    addr = 0x4000;
                    valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.X;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_InnnI_value_Irrr_rrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (0x4000),(TUV+WX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.TUV = 0x8000; // value setup (part 1)
                    c.CPU.REGS.WX = 5;       // value setup (part 2)
                    addr = 0x4000;
                    valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.WX;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_InnnI_value_Irrr_rrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (0x4000),(TUV+VWX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.TUV = 0x8000; // value setup (part 1)
                    c.CPU.REGS.VWX = 7;      // value setup (part 2)
                    addr = 0x4000;
                    valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.VWX;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_InnnI_fr() // Ok
        {
            const uint addr = 0x4000;

            RunTest(
                "MUL (0x4000),F0",
                c =>
                {
                    c.MEMC.SetFloatToRam(addr, 10.0f);
                    c.CPU.FREGS.SetRegister(0, 2.5f);
                },
                c => Assert.Equal(25.0f, c.MEMC.GetFloatFromRAM(addr)));
        }

        // --------------------
        // Target: (0x4000+4)
        // --------------------

        [Fact]
        public void MUL_mem_Innn_nnnI_block_immediate_2bytes_once() // Ok
        {
            const uint addr = 0x4004;

            RunTest(
                "MUL (0x4000+4),0x00000002,2",
                c => c.LoadMemAt(addr, [0x00, 0x0A]),
                c => Assert.Equal([0x00, 0x14], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Innn_nnnI_block_immediate_2bytes_twice() // Ok
        {
            const uint addr = 0x4004;

            RunTest(
                "MUL (0x4000+4),0x00000002,2,2",
                c => c.LoadMemAt(addr, [0x00, 0x0A]),
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Innn_nnnI_r() // Ok
        {
            const uint addr = 0x4004;

            RunTest(
                "MUL (0x4000+4),B",
                c =>
                {
                    c.MEMC.Set8bitToRAM(addr, 10);
                    c.CPU.REGS.B = 5;
                },
                c => Assert.Equal((byte)50, c.MEMC.Get8bitFromRAM(addr)));
        }

        [Fact]
        public void MUL_mem_Innn_nnnI_rr() // Ok
        {
            const uint addr = 0x4004;

            RunTest(
                "MUL (0x4000+4),CD",
                c =>
                {
                    c.MEMC.Set16bitToRAM(addr, 100);
                    c.CPU.REGS.CD = 10;
                },
                c => Assert.Equal((ushort)1000, c.MEMC.Get16bitFromRAM(addr)));
        }

        [Fact]
        public void MUL_mem_Innn_nnnI_rrr() // Ok
        {
            const uint addr = 0x4004;

            RunTest(
                "MUL (0x4000+4),DEF",
                c =>
                {
                    c.MEMC.Set24bitToRAM(addr, 10000);
                    c.CPU.REGS.DEF = 10;
                },
                c => Assert.Equal((uint)100000, c.MEMC.Get24bitFromRAM(addr)));
        }

        [Fact]
        public void MUL_mem_Innn_nnnI_rrrr() // Ok
        {
            const uint addr = 0x4004;

            RunTest(
                "MUL (0x4000+4),EFGH",
                c =>
                {
                    c.MEMC.Set32bitToRAM(addr, 100_000);
                    c.CPU.REGS.EFGH = 10;
                },
                c => Assert.Equal((uint)1_000_000, c.MEMC.Get32bitFromRAM(addr)));
        }

        [Fact]
        public void MUL_mem_Innn_nnnI_value_InnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (0x4000+4),(0x7000),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    addr = 0x4004;
                    valueAddr = 0x7000;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Innn_nnnI_value_Innn_nnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (0x4000+4),(0x7000+4),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    addr = 0x4004;
                    valueAddr = 0x7004;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Innn_nnnI_value_Innn_rI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (0x4000+4),(0x7000+X),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    // target setup: none
                    c.CPU.REGS.X = 3; // value setup
                    addr = 0x4004;
                    valueAddr = (uint)(0x7000 + c.CPU.REGS.X);
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Innn_nnnI_value_Innn_rrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (0x4000+4),(0x7000+WX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.WX = 5; // value setup
                    addr = 0x4004;
                    valueAddr = (uint)(0x7000 + c.CPU.REGS.WX);
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Innn_nnnI_value_Innn_rrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (0x4000+4),(0x7000+VWX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.VWX = 7; // value setup
                    addr = 0x4004;
                    valueAddr = (uint)(0x7000 + c.CPU.REGS.VWX);
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Innn_nnnI_value_IrrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (0x4000+4),(TUV),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.TUV = 0x8000; // value setup
                    addr = 0x4004;
                    valueAddr = c.CPU.REGS.TUV;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Innn_nnnI_value_Irrr_nnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (0x4000+4),(TUV+4),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.TUV = 0x8000; // value setup
                    addr = 0x4004;
                    valueAddr = c.CPU.REGS.TUV + 4;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Innn_nnnI_value_Irrr_rI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (0x4000+4),(TUV+X),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.TUV = 0x8000;
                    c.CPU.REGS.X = 3;
                    addr = 0x4004;
                    valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.X;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Innn_nnnI_value_Irrr_rrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (0x4000+4),(TUV+WX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.TUV = 0x8000;
                    c.CPU.REGS.WX = 5;
                    addr = 0x4004;
                    valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.WX;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Innn_nnnI_value_Irrr_rrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (0x4000+4),(TUV+VWX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.TUV = 0x8000;
                    c.CPU.REGS.VWX = 7;
                    addr = 0x4004;
                    valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.VWX;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([ 0x00, 0x10], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Innn_nnnI_fr() // Ok
        {
            const uint addr = 0x4004;

            RunTest(
                "MUL (0x4000+4),F0",
                c =>
                {
                    c.MEMC.SetFloatToRam(addr, 10.0f);
                    c.CPU.FREGS.SetRegister(0, 2.5f);
                },
                c => Assert.Equal(25.0f, c.MEMC.GetFloatFromRAM(addr)));
        }

        // --------------------
        // Target: (0x4000+X)
        // --------------------

        [Fact]
        public void MUL_mem_Innn_rI_block_immediate_2bytes_once() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (0x4000+X),0x00000002,2",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    addr = (uint)(0x4000 + c.CPU.REGS.X);
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                },
                c => Assert.Equal([ 0x00, 0x20], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Innn_rI_block_immediate_2bytes_twice() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (0x4000+X),0x00000002,2,2",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    addr = (uint)(0x4000 + c.CPU.REGS.X);
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                },
                c => Assert.Equal([ 0x00, 0x10], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Innn_rI_r() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (0x4000+X),B",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    addr = (uint)(0x4000 + c.CPU.REGS.X);
                    c.MEMC.Set8bitToRAM(addr, 10);
                    c.CPU.REGS.B = 5;
                },
                c => Assert.Equal((byte)50, c.MEMC.Get8bitFromRAM(addr)));
        }

        [Fact]
        public void MUL_mem_Innn_rI_rr() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (0x4000+X),CD",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    addr = (uint)(0x4000 + c.CPU.REGS.X);
                    c.MEMC.Set16bitToRAM(addr, 100);
                    c.CPU.REGS.CD = 10;
                },
                c => Assert.Equal((ushort)1000, c.MEMC.Get16bitFromRAM(addr)));
        }

        [Fact]
        public void MUL_mem_Innn_rI_rrr() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (0x4000+X),DEF",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    addr = (uint)(0x4000 + c.CPU.REGS.X);
                    c.MEMC.Set24bitToRAM(addr, 10000);
                    c.CPU.REGS.DEF = 10;
                },
                c => Assert.Equal((uint)100000, c.MEMC.Get24bitFromRAM(addr)));
        }

        [Fact]
        public void MUL_mem_Innn_rI_rrrr() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (0x4000+X),EFGH",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    addr = (uint)(0x4000 + c.CPU.REGS.X);
                    c.MEMC.Set32bitToRAM(addr, 100_000);
                    c.CPU.REGS.EFGH = 10;
                },
                c => Assert.Equal((uint)1_000_000, c.MEMC.Get32bitFromRAM(addr)));
        }

        [Fact]
        public void MUL_mem_Innn_rI_value_InnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (0x4000+X),(0x7000),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.X = 3; // target setup
                    // value setup: none
                    addr = (uint)(0x4000 + c.CPU.REGS.X);
                    valueAddr = 0x7000;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([ 0x00, 0x10], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Innn_rI_value_Innn_nnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (0x4000+X),(0x7000+4),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.X = 3; // target setup
                    addr = (uint)(0x4000 + c.CPU.REGS.X);
                    valueAddr = 0x7004;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([ 0x00, 0x10], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Innn_rI_value_Innn_rI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (0x4000+X),(0x7000+X),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.X = 3; // target setup
                    c.CPU.REGS.X = 3; // value setup (same register)
                    addr = (uint)(0x4000 + c.CPU.REGS.X);
                    valueAddr = (uint)(0x7000 + c.CPU.REGS.X);
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([ 0x00, 0x10], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Innn_rI_value_Innn_rrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (0x4000+X),(0x7000+WX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.X = 3;  // target setup
                    c.CPU.REGS.WX = 5; // value setup
                    addr = (uint)(0x4000 + c.CPU.REGS.X);
                    valueAddr = (uint)(0x7000 + c.CPU.REGS.WX);
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([ 0x00, 0x10], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Innn_rI_value_Innn_rrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (0x4000+X),(0x7000+VWX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.X = 3;   // target setup
                    c.CPU.REGS.VWX = 7; // value setup
                    addr = (uint)(0x4000 + c.CPU.REGS.X);
                    valueAddr = (uint)(0x7000 + c.CPU.REGS.VWX);
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([ 0x00, 0x10], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Innn_rI_value_IrrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (0x4000+X),(TUV),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.X = 3;       // target setup
                    c.CPU.REGS.TUV = 0x8000; // value setup
                    addr = (uint)(0x4000 + c.CPU.REGS.X);
                    valueAddr = c.CPU.REGS.TUV;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Innn_rI_value_Irrr_nnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (0x4000+X),(TUV+4),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.X = 3;        // target setup
                    c.CPU.REGS.TUV = 0x8000; // value setup
                    addr = (uint)(0x4000 + c.CPU.REGS.X);
                    valueAddr = c.CPU.REGS.TUV + 4;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Innn_rI_value_Irrr_rI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (0x4000+X),(TUV+X),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.X = 3;        // target setup
                    c.CPU.REGS.TUV = 0x8000; // value setup (part 1)
                    c.CPU.REGS.X = 3;        // value setup (part 2)
                    addr = (uint)(0x4000 + c.CPU.REGS.X);
                    valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.X;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Innn_rI_value_Irrr_rrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (0x4000+X),(TUV+WX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.X = 3;        // target setup
                    c.CPU.REGS.TUV = 0x8000; // value setup (part 1)
                    c.CPU.REGS.WX = 5;       // value setup (part 2)
                    addr = (uint)(0x4000 + c.CPU.REGS.X);
                    valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.WX;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Innn_rI_value_Irrr_rrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (0x4000+X),(TUV+VWX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.X = 3;        // target setup
                    c.CPU.REGS.TUV = 0x8000; // value setup (part 1)
                    c.CPU.REGS.VWX = 7;      // value setup (part 2)
                    addr = (uint)(0x4000 + c.CPU.REGS.X);
                    valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.VWX;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Innn_rI_fr() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (0x4000+X),F0",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    addr = (uint)(0x4000 + c.CPU.REGS.X);
                    c.MEMC.SetFloatToRam(addr, 10.0f);
                    c.CPU.FREGS.SetRegister(0, 2.5f);
                },
                c => Assert.Equal(25.0f, c.MEMC.GetFloatFromRAM(addr)));
        }

        // --------------------
        // Target: (0x4000+WX)
        // --------------------

        [Fact]
        public void MUL_mem_Innn_rrI_block_immediate_2bytes_once() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (0x4000+WX),0x00000002,2",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    addr = (uint)(0x4000 + c.CPU.REGS.WX);
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                },
                c => Assert.Equal([ 0x00, 0x20 ], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Innn_rrI_block_immediate_2bytes_twice() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (0x4000+WX),0x00000002,2,2",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    addr = (uint)(0x4000 + c.CPU.REGS.WX);
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Innn_rrI_r() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (0x4000+WX),B",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    addr = (uint)(0x4000 + c.CPU.REGS.WX);
                    c.MEMC.Set8bitToRAM(addr, 10);
                    c.CPU.REGS.B = 5;
                },
                c => Assert.Equal((byte)50, c.MEMC.Get8bitFromRAM(addr)));
        }

        [Fact]
        public void MUL_mem_Innn_rrI_rr() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (0x4000+WX),CD",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    addr = (uint)(0x4000 + c.CPU.REGS.WX);
                    c.MEMC.Set16bitToRAM(addr, 100);
                    c.CPU.REGS.CD = 10;
                },
                c => Assert.Equal((ushort)1000, c.MEMC.Get16bitFromRAM(addr)));
        }

        [Fact]
        public void MUL_mem_Innn_rrI_rrr() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (0x4000+WX),DEF",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    addr = (uint)(0x4000 + c.CPU.REGS.WX);
                    c.MEMC.Set24bitToRAM(addr, 10000);
                    c.CPU.REGS.DEF = 10;
                },
                c => Assert.Equal((uint)100000, c.MEMC.Get24bitFromRAM(addr)));
        }

        [Fact]
        public void MUL_mem_Innn_rrI_rrrr() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (0x4000+WX),EFGH",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    addr = (uint)(0x4000 + c.CPU.REGS.WX);
                    c.MEMC.Set32bitToRAM(addr, 100_000);
                    c.CPU.REGS.EFGH = 10;
                },
                c => Assert.Equal((uint)1_000_000, c.MEMC.Get32bitFromRAM(addr)));
        }

        [Fact]
        public void MUL_mem_Innn_rrI_value_InnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (0x4000+WX),(0x7000),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.WX = 5; // target setup
                    addr = (uint)(0x4000 + c.CPU.REGS.WX);
                    valueAddr = 0x7000;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Innn_rrI_value_Innn_nnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (0x4000+WX),(0x7000+4),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.WX = 5; // target setup
                    addr = (uint)(0x4000 + c.CPU.REGS.WX);
                    valueAddr = 0x7004;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Innn_rrI_value_Innn_rI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (0x4000+WX),(0x7000+X),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.WX = 5; // target setup
                    c.CPU.REGS.X = 3;  // value setup
                    addr = (uint)(0x4000 + c.CPU.REGS.WX);
                    valueAddr = (uint)(0x7000 + c.CPU.REGS.X);
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Innn_rrI_value_Innn_rrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (0x4000+WX),(0x7000+WX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.WX = 5; // target setup
                    c.CPU.REGS.WX = 5; // value setup (same register)
                    addr = (uint)(0x4000 + c.CPU.REGS.WX);
                    valueAddr = (uint)(0x7000 + c.CPU.REGS.WX);
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Innn_rrI_value_Innn_rrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (0x4000+WX),(0x7000+VWX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.WX = 5;   // target setup
                    c.CPU.REGS.VWX = 7;  // value setup
                    addr = (uint)(0x4000 + c.CPU.REGS.WX);
                    valueAddr = (uint)(0x7000 + c.CPU.REGS.VWX);
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Innn_rrI_value_IrrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (0x4000+WX),(TUV),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.WX = 5;        // target setup
                    c.CPU.REGS.TUV = 0x8000;  // value setup
                    addr = (uint)(0x4000 + c.CPU.REGS.WX);
                    valueAddr = c.CPU.REGS.TUV;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Innn_rrI_value_Irrr_nnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (0x4000+WX),(TUV+4),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.WX = 5;        // target setup
                    c.CPU.REGS.TUV = 0x8000;  // value setup
                    addr = (uint)(0x4000 + c.CPU.REGS.WX);
                    valueAddr = c.CPU.REGS.TUV + 4;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Innn_rrI_value_Irrr_rI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (0x4000+WX),(TUV+X),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.WX = 5;        // target setup
                    c.CPU.REGS.TUV = 0x8000;  // value setup (part 1)
                    c.CPU.REGS.X = 3;         // value setup (part 2)
                    addr = (uint)(0x4000 + c.CPU.REGS.WX);
                    valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.X;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Innn_rrI_value_Irrr_rrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (0x4000+WX),(TUV+WX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.WX = 5;        // target setup
                    c.CPU.REGS.TUV = 0x8000;  // value setup (part 1)
                    c.CPU.REGS.WX = 5;        // value setup (part 2; same register)
                    addr = (uint)(0x4000 + c.CPU.REGS.WX);
                    valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.WX;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Innn_rrI_value_Irrr_rrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (0x4000+WX),(TUV+VWX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.WX = 5;        // target setup
                    c.CPU.REGS.TUV = 0x8000;  // value setup (part 1)
                    c.CPU.REGS.VWX = 7;       // value setup (part 2)
                    addr = (uint)(0x4000 + c.CPU.REGS.WX);
                    valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.VWX;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Innn_rrI_fr() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (0x4000+WX),F0",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    addr = (uint)(0x4000 + c.CPU.REGS.WX);
                    c.MEMC.SetFloatToRam(addr, 10.0f);
                    c.CPU.FREGS.SetRegister(0, 2.5f);
                },
                c => Assert.Equal(25.0f, c.MEMC.GetFloatFromRAM(addr)));
        }

        // --------------------
        // Target: (0x4000+VWX)
        // --------------------

        [Fact]
        public void MUL_mem_Innn_rrrI_block_immediate_2bytes_once() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (0x4000+VWX),0x00000002,2",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    addr = (uint)(0x4000 + c.CPU.REGS.VWX);
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                },
                c => Assert.Equal([ 0x00, 0x20 ], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Innn_rrrI_block_immediate_2bytes_twice() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (0x4000+VWX),0x00000002,2,2",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    addr = (uint)(0x4000 + c.CPU.REGS.VWX);
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Innn_rrrI_r() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (0x4000+VWX),B",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    addr = (uint)(0x4000 + c.CPU.REGS.VWX);
                    c.MEMC.Set8bitToRAM(addr, 10);
                    c.CPU.REGS.B = 5;
                },
                c => Assert.Equal((byte)50, c.MEMC.Get8bitFromRAM(addr)));
        }

        [Fact]
        public void MUL_mem_Innn_rrrI_rr() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (0x4000+VWX),CD",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    addr = (uint)(0x4000 + c.CPU.REGS.VWX);
                    c.MEMC.Set16bitToRAM(addr, 100);
                    c.CPU.REGS.CD = 10;
                },
                c => Assert.Equal((ushort)1000, c.MEMC.Get16bitFromRAM(addr)));
        }

        [Fact]
        public void MUL_mem_Innn_rrrI_rrr() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (0x4000+VWX),DEF",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    addr = (uint)(0x4000 + c.CPU.REGS.VWX);
                    c.MEMC.Set24bitToRAM(addr, 10000);
                    c.CPU.REGS.DEF = 10;
                },
                c => Assert.Equal((uint)100000, c.MEMC.Get24bitFromRAM(addr)));
        }

        [Fact]
        public void MUL_mem_Innn_rrrI_rrrr() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (0x4000+VWX),EFGH",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    addr = (uint)(0x4000 + c.CPU.REGS.VWX);
                    c.MEMC.Set32bitToRAM(addr, 100_000);
                    c.CPU.REGS.EFGH = 10;
                },
                c => Assert.Equal((uint)1_000_000, c.MEMC.Get32bitFromRAM(addr)));
        }

        [Fact]
        public void MUL_mem_Innn_rrrI_value_InnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (0x4000+VWX),(0x7000),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.VWX = 7; // target setup
                    addr = (uint)(0x4000 + c.CPU.REGS.VWX);
                    valueAddr = 0x7000;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Innn_rrrI_value_Innn_nnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (0x4000+VWX),(0x7000+4),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.VWX = 7; // target setup
                    addr = (uint)(0x4000 + c.CPU.REGS.VWX);
                    valueAddr = 0x7004;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Innn_rrrI_value_Innn_rI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (0x4000+VWX),(0x7000+X),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.VWX = 7; // target setup
                    c.CPU.REGS.X = 3;   // value setup
                    addr = (uint)(0x4000 + c.CPU.REGS.VWX);
                    valueAddr = (uint)(0x7000 + c.CPU.REGS.X);
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Innn_rrrI_value_Innn_rrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (0x4000+VWX),(0x7000+WX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.VWX = 7; // target setup
                    c.CPU.REGS.WX = 5;  // value setup
                    addr = (uint)(0x4000 + c.CPU.REGS.VWX);
                    valueAddr = (uint)(0x7000 + c.CPU.REGS.WX);
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Innn_rrrI_value_Innn_rrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (0x4000+VWX),(0x7000+VWX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.VWX = 7; // target setup
                    c.CPU.REGS.VWX = 7; // value setup (same register)
                    addr = (uint)(0x4000 + c.CPU.REGS.VWX);
                    valueAddr = (uint)(0x7000 + c.CPU.REGS.VWX);
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Innn_rrrI_value_IrrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (0x4000+VWX),(TUV),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.VWX = 7;      // target setup
                    c.CPU.REGS.TUV = 0x8000; // value setup
                    addr = (uint)(0x4000 + c.CPU.REGS.VWX);
                    valueAddr = c.CPU.REGS.TUV;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Innn_rrrI_value_Irrr_nnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (0x4000+VWX),(TUV+4),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.VWX = 7;      // target setup
                    c.CPU.REGS.TUV = 0x8000; // value setup
                    addr = (uint)(0x4000 + c.CPU.REGS.VWX);
                    valueAddr = c.CPU.REGS.TUV + 4;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Innn_rrrI_value_Irrr_rI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (0x4000+VWX),(TUV+X),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.VWX = 7;      // target setup
                    c.CPU.REGS.TUV = 0x8000; // value setup (part 1)
                    c.CPU.REGS.X = 3;        // value setup (part 2)
                    addr = (uint)(0x4000 + c.CPU.REGS.VWX);
                    valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.X;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Innn_rrrI_value_Irrr_rrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (0x4000+VWX),(TUV+WX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.VWX = 7;      // target setup
                    c.CPU.REGS.TUV = 0x8000; // value setup (part 1)
                    c.CPU.REGS.WX = 5;       // value setup (part 2)
                    addr = (uint)(0x4000 + c.CPU.REGS.VWX);
                    valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.WX;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Innn_rrrI_value_Irrr_rrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (0x4000+VWX),(TUV+VWX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.VWX = 7;      // target setup
                    c.CPU.REGS.TUV = 0x8000; // value setup (part 1)
                    c.CPU.REGS.VWX = 7;      // value setup (part 2; same register)
                    addr = (uint)(0x4000 + c.CPU.REGS.VWX);
                    valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.VWX;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Innn_rrrI_fr() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (0x4000+VWX),F0",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    addr = (uint)(0x4000 + c.CPU.REGS.VWX);
                    c.MEMC.SetFloatToRam(addr, 10.0f);
                    c.CPU.FREGS.SetRegister(0, 2.5f);
                },
                c => Assert.Equal(25.0f, c.MEMC.GetFloatFromRAM(addr)));
        }

        // --------------------
        // Target: (QRS)
        // --------------------

        [Fact]
        public void MUL_mem_IrrrI_block_immediate_2bytes_once() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (QRS),0x00000002,2",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    addr = c.CPU.REGS.QRS;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                },
                c => Assert.Equal([ 0x00, 0x14], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_IrrrI_block_immediate_2bytes_twice() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (QRS),0x00000002,2,2",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    addr = c.CPU.REGS.QRS;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_IrrrI_r() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (QRS),B",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    addr = c.CPU.REGS.QRS;
                    c.MEMC.Set8bitToRAM(addr, 10);
                    c.CPU.REGS.B = 5;
                },
                c => Assert.Equal((byte)50, c.MEMC.Get8bitFromRAM(addr)));
        }

        [Fact]
        public void MUL_mem_IrrrI_rr() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (QRS),CD",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    addr = c.CPU.REGS.QRS;
                    c.MEMC.Set16bitToRAM(addr, 100);
                    c.CPU.REGS.CD = 10;
                },
                c => Assert.Equal((ushort)1000, c.MEMC.Get16bitFromRAM(addr)));
        }

        [Fact]
        public void MUL_mem_IrrrI_rrr() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (QRS),DEF",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    addr = c.CPU.REGS.QRS;
                    c.MEMC.Set24bitToRAM(addr, 10000);
                    c.CPU.REGS.DEF = 10;
                },
                c => Assert.Equal((uint)100000, c.MEMC.Get24bitFromRAM(addr)));
        }

        [Fact]
        public void MUL_mem_IrrrI_rrrr() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (QRS),EFGH",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    addr = c.CPU.REGS.QRS;
                    c.MEMC.Set32bitToRAM(addr, 100_000);
                    c.CPU.REGS.EFGH = 10;
                },
                c => Assert.Equal((uint)1_000_000, c.MEMC.Get32bitFromRAM(addr)));
        }

        [Fact]
        public void MUL_mem_IrrrI_value_InnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (QRS),(0x7000),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; // target setup
                    // value setup: none
                    addr = c.CPU.REGS.QRS;
                    valueAddr = 0x7000;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_IrrrI_value_Innn_nnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (QRS),(0x7000+4),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; // target setup
                    addr = c.CPU.REGS.QRS;
                    valueAddr = 0x7004;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_IrrrI_value_Innn_rI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (QRS),(0x7000+X),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; // target setup
                    c.CPU.REGS.X = 3;        // value setup
                    addr = c.CPU.REGS.QRS;
                    valueAddr = (uint)(0x7000 + c.CPU.REGS.X);
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_IrrrI_value_Innn_rrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (QRS),(0x7000+WX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; // target setup
                    c.CPU.REGS.WX = 5;       // value setup
                    addr = c.CPU.REGS.QRS;
                    valueAddr = (uint)(0x7000 + c.CPU.REGS.WX);
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_IrrrI_value_Innn_rrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (QRS),(0x7000+VWX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; // target setup
                    c.CPU.REGS.VWX = 7;      // value setup
                    addr = c.CPU.REGS.QRS;
                    valueAddr = (uint)(0x7000 + c.CPU.REGS.VWX);
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_IrrrI_value_IrrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (QRS),(TUV),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; // target setup
                    c.CPU.REGS.TUV = 0x8000; // value setup
                    addr = c.CPU.REGS.QRS;
                    valueAddr = c.CPU.REGS.TUV;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_IrrrI_value_Irrr_nnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (QRS),(TUV+4),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; // target setup
                    c.CPU.REGS.TUV = 0x8000; // value setup
                    addr = c.CPU.REGS.QRS;
                    valueAddr = c.CPU.REGS.TUV + 4;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_IrrrI_value_Irrr_rI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (QRS),(TUV+X),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; // target setup
                    c.CPU.REGS.TUV = 0x8000; // value setup (part 1)
                    c.CPU.REGS.X = 3;        // value setup (part 2)
                    addr = c.CPU.REGS.QRS;
                    valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.X;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_IrrrI_value_Irrr_rrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (QRS),(TUV+WX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; // target setup
                    c.CPU.REGS.TUV = 0x8000; // value setup (part 1)
                    c.CPU.REGS.WX = 5;       // value setup (part 2)
                    addr = c.CPU.REGS.QRS;
                    valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.WX;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_IrrrI_value_Irrr_rrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (QRS),(TUV+VWX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; // target setup
                    c.CPU.REGS.TUV = 0x8000; // value setup (part 1)
                    c.CPU.REGS.VWX = 7;      // value setup (part 2)
                    addr = c.CPU.REGS.QRS;
                    valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.VWX;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_IrrrI_fr() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (QRS),F0",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    addr = c.CPU.REGS.QRS;
                    c.MEMC.SetFloatToRam(addr, 10.0f);
                    c.CPU.FREGS.SetRegister(0, 2.5f);
                },
                c => Assert.Equal(25.0f, c.MEMC.GetFloatFromRAM(addr)));
        }

        // NOTE: Remaining memory-destination targets are similarly unwound below.
        // (QRS+4), (QRS+X), (QRS+WX), (QRS+VWX)

        // --------------------
        // Target: (QRS+4)
        // --------------------

        [Fact]
        public void MUL_mem_Irrr_nnnI_block_immediate_2bytes_once() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (QRS+4),0x00000002,2",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    addr = c.CPU.REGS.QRS + 4;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                },
                c => Assert.Equal([ 0x00, 0x20 ], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Irrr_nnnI_block_immediate_2bytes_twice() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (QRS+4),0x00000002,2,2",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    addr = c.CPU.REGS.QRS + 4;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Irrr_nnnI_r() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (QRS+4),B",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    addr = c.CPU.REGS.QRS + 4;
                    c.MEMC.Set8bitToRAM(addr, 10);
                    c.CPU.REGS.B = 5;
                },
                c => Assert.Equal((byte)50, c.MEMC.Get8bitFromRAM(addr)));
        }

        [Fact]
        public void MUL_mem_Irrr_nnnI_rr() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (QRS+4),CD",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    addr = c.CPU.REGS.QRS + 4;
                    c.MEMC.Set16bitToRAM(addr, 100);
                    c.CPU.REGS.CD = 10;
                },
                c => Assert.Equal((ushort)1000, c.MEMC.Get16bitFromRAM(addr)));
        }

        [Fact]
        public void MUL_mem_Irrr_nnnI_rrr() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (QRS+4),DEF",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    addr = c.CPU.REGS.QRS + 4;
                    c.MEMC.Set24bitToRAM(addr, 10000);
                    c.CPU.REGS.DEF = 10;
                },
                c => Assert.Equal((uint)100000, c.MEMC.Get24bitFromRAM(addr)));
        }

        [Fact]
        public void MUL_mem_Irrr_nnnI_rrrr() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (QRS+4),EFGH",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    addr = c.CPU.REGS.QRS + 4;
                    c.MEMC.Set32bitToRAM(addr, 100_000);
                    c.CPU.REGS.EFGH = 10;
                },
                c => Assert.Equal((uint)1_000_000, c.MEMC.Get32bitFromRAM(addr)));
        }

        [Fact]
        public void MUL_mem_Irrr_nnnI_value_InnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (QRS+4),(0x7000),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000;
                    addr = c.CPU.REGS.QRS + 4;
                    valueAddr = 0x7000;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Irrr_nnnI_value_Innn_nnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (QRS+4),(0x7000+4),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000;
                    addr = c.CPU.REGS.QRS + 4;
                    valueAddr = 0x7004;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Irrr_nnnI_value_Innn_rI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (QRS+4),(0x7000+X),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    addr = c.CPU.REGS.QRS + 4;
                    valueAddr = (uint)(0x7000 + c.CPU.REGS.X);
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Irrr_nnnI_value_Innn_rrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (QRS+4),(0x7000+WX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    addr = c.CPU.REGS.QRS + 4;
                    valueAddr = (uint)(0x7000 + c.CPU.REGS.WX);
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Irrr_nnnI_value_Innn_rrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (QRS+4),(0x7000+VWX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    addr = c.CPU.REGS.QRS + 4;
                    valueAddr = (uint)(0x7000 + c.CPU.REGS.VWX);
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Irrr_nnnI_value_IrrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (QRS+4),(TUV),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.TUV = 0x8000;
                    addr = c.CPU.REGS.QRS + 4;
                    valueAddr = c.CPU.REGS.TUV;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Irrr_nnnI_value_Irrr_nnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (QRS+4),(TUV+4),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.TUV = 0x8000;
                    addr = c.CPU.REGS.QRS + 4;
                    valueAddr = c.CPU.REGS.TUV + 4;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Irrr_nnnI_value_Irrr_rI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (QRS+4),(TUV+X),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.TUV = 0x8000;
                    c.CPU.REGS.X = 3;
                    addr = c.CPU.REGS.QRS + 4;
                    valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.X;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Irrr_nnnI_value_Irrr_rrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (QRS+4),(TUV+WX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.TUV = 0x8000;
                    c.CPU.REGS.WX = 5;
                    addr = c.CPU.REGS.QRS + 4;
                    valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.WX;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Irrr_nnnI_value_Irrr_rrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (QRS+4),(TUV+VWX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.TUV = 0x8000;
                    c.CPU.REGS.VWX = 7;
                    addr = c.CPU.REGS.QRS + 4;
                    valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.VWX;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Irrr_nnnI_fr() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (QRS+4),F0",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    addr = c.CPU.REGS.QRS + 4;
                    c.MEMC.SetFloatToRam(addr, 10.0f);
                    c.CPU.FREGS.SetRegister(0, 2.5f);
                },
                c => Assert.Equal(25.0f, c.MEMC.GetFloatFromRAM(addr)));
        }

        // --------------------
        // Target: (QRS+X)
        // --------------------

        [Fact]
        public void MUL_mem_Irrr_rI_block_immediate_2bytes_once() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (QRS+X),0x00000002,2",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    addr = c.CPU.REGS.QRS + c.CPU.REGS.X;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                },
                c => Assert.Equal([ 0x00, 0x14 ], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Irrr_rI_block_immediate_2bytes_twice() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (QRS+X),0x00000002,2,2",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    addr = c.CPU.REGS.QRS + c.CPU.REGS.X;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Irrr_rI_r() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (QRS+X),B",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    addr = c.CPU.REGS.QRS + c.CPU.REGS.X;
                    c.MEMC.Set8bitToRAM(addr, 10);
                    c.CPU.REGS.B = 5;
                },
                c => Assert.Equal((byte)50, c.MEMC.Get8bitFromRAM(addr)));
        }

        [Fact]
        public void MUL_mem_Irrr_rI_rr() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (QRS+X),CD",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    addr = c.CPU.REGS.QRS + c.CPU.REGS.X;
                    c.MEMC.Set16bitToRAM(addr, 100);
                    c.CPU.REGS.CD = 10;
                },
                c => Assert.Equal((ushort)1000, c.MEMC.Get16bitFromRAM(addr)));
        }

        [Fact]
        public void MUL_mem_Irrr_rI_rrr() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (QRS+X),DEF",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    addr = c.CPU.REGS.QRS + c.CPU.REGS.X;
                    c.MEMC.Set24bitToRAM(addr, 10000);
                    c.CPU.REGS.DEF = 10;
                },
                c => Assert.Equal((uint)100000, c.MEMC.Get24bitFromRAM(addr)));
        }

        [Fact]
        public void MUL_mem_Irrr_rI_rrrr() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (QRS+X),EFGH",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    addr = c.CPU.REGS.QRS + c.CPU.REGS.X;
                    c.MEMC.Set32bitToRAM(addr, 100_000);
                    c.CPU.REGS.EFGH = 10;
                },
                c => Assert.Equal((uint)1_000_000, c.MEMC.Get32bitFromRAM(addr)));
        }

        [Fact]
        public void MUL_mem_Irrr_rI_value_InnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (QRS+X),(0x7000),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000; // target setup
                    c.CPU.REGS.X = 3;        // target setup cont.
                    addr = c.CPU.REGS.QRS + c.CPU.REGS.X;
                    valueAddr = 0x7000;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Irrr_rI_value_Innn_nnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (QRS+X),(0x7000+4),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    addr = c.CPU.REGS.QRS + c.CPU.REGS.X;
                    valueAddr = 0x7004;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Irrr_rI_value_Innn_rI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (QRS+X),(0x7000+X),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3; // target setup (also affects value)
                    c.CPU.REGS.X = 3; // value setup
                    addr = c.CPU.REGS.QRS + c.CPU.REGS.X;
                    valueAddr = (uint)(0x7000 + c.CPU.REGS.X);
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Irrr_rI_value_Innn_rrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (QRS+X),(0x7000+WX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;  // target setup
                    c.CPU.REGS.WX = 5; // value setup
                    addr = c.CPU.REGS.QRS + c.CPU.REGS.X;
                    valueAddr = (uint)(0x7000 + c.CPU.REGS.WX);
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Irrr_rI_value_Innn_rrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (QRS+X),(0x7000+VWX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;   // target setup
                    c.CPU.REGS.VWX = 7; // value setup
                    addr = c.CPU.REGS.QRS + c.CPU.REGS.X;
                    valueAddr = (uint)(0x7000 + c.CPU.REGS.VWX);
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Irrr_rI_value_IrrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (QRS+X),(TUV),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;        // target setup
                    c.CPU.REGS.TUV = 0x8000; // value setup
                    addr = c.CPU.REGS.QRS + c.CPU.REGS.X;
                    valueAddr = c.CPU.REGS.TUV;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Irrr_rI_value_Irrr_nnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (QRS+X),(TUV+4),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.CPU.REGS.TUV = 0x8000;
                    addr = c.CPU.REGS.QRS + c.CPU.REGS.X;
                    valueAddr = c.CPU.REGS.TUV + 4;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Irrr_rI_value_Irrr_rI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (QRS+X),(TUV+X),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;        // target setup (also affects value)
                    c.CPU.REGS.TUV = 0x8000; // value setup
                    c.CPU.REGS.X = 3;        // value setup cont.
                    addr = c.CPU.REGS.QRS + c.CPU.REGS.X;
                    valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.X;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Irrr_rI_value_Irrr_rrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (QRS+X),(TUV+WX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;        // target setup
                    c.CPU.REGS.TUV = 0x8000; // value setup (part 1)
                    c.CPU.REGS.WX = 5;       // value setup (part 2)
                    addr = c.CPU.REGS.QRS + c.CPU.REGS.X;
                    valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.WX;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Irrr_rI_value_Irrr_rrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (QRS+X),(TUV+VWX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;        // target setup
                    c.CPU.REGS.TUV = 0x8000; // value setup (part 1)
                    c.CPU.REGS.VWX = 7;      // value setup (part 2)
                    addr = c.CPU.REGS.QRS + c.CPU.REGS.X;
                    valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.VWX;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Irrr_rI_fr() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (QRS+X),F0",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    addr = c.CPU.REGS.QRS + c.CPU.REGS.X;
                    c.MEMC.SetFloatToRam(addr, 10.0f);
                    c.CPU.FREGS.SetRegister(0, 2.5f);
                },
                c => Assert.Equal(25.0f, c.MEMC.GetFloatFromRAM(addr)));
        }

        // --------------------
        // Target: (QRS+WX)
        // --------------------

        [Fact]
        public void MUL_mem_Irrr_rrI_block_immediate_2bytes_once() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (QRS+WX),0x00000002,2",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    addr = c.CPU.REGS.QRS + c.CPU.REGS.WX;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                },
                c => Assert.Equal([ 0x00, 0x14 ], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Irrr_rrI_block_immediate_2bytes_twice() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (QRS+WX),0x00000002,2,2",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    addr = c.CPU.REGS.QRS + c.CPU.REGS.WX;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Irrr_rrI_r() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (QRS+WX),B",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    addr = c.CPU.REGS.QRS + c.CPU.REGS.WX;
                    c.MEMC.Set8bitToRAM(addr, 10);
                    c.CPU.REGS.B = 5;
                },
                c => Assert.Equal((byte)50, c.MEMC.Get8bitFromRAM(addr)));
        }

        [Fact]
        public void MUL_mem_Irrr_rrI_rr() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (QRS+WX),CD",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    addr = c.CPU.REGS.QRS + c.CPU.REGS.WX;
                    c.MEMC.Set16bitToRAM(addr, 100);
                    c.CPU.REGS.CD = 10;
                },
                c => Assert.Equal((ushort)1000, c.MEMC.Get16bitFromRAM(addr)));
        }

        [Fact]
        public void MUL_mem_Irrr_rrI_rrr() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (QRS+WX),DEF",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    addr = c.CPU.REGS.QRS + c.CPU.REGS.WX;
                    c.MEMC.Set24bitToRAM(addr, 10000);
                    c.CPU.REGS.DEF = 10;
                },
                c => Assert.Equal((uint)100000, c.MEMC.Get24bitFromRAM(addr)));
        }

        [Fact]
        public void MUL_mem_Irrr_rrI_rrrr() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (QRS+WX),EFGH",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    addr = c.CPU.REGS.QRS + c.CPU.REGS.WX;
                    c.MEMC.Set32bitToRAM(addr, 100_000);
                    c.CPU.REGS.EFGH = 10;
                },
                c => Assert.Equal((uint)1_000_000, c.MEMC.Get32bitFromRAM(addr)));
        }

        [Fact]
        public void MUL_mem_Irrr_rrI_value_InnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (QRS+WX),(0x7000),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    addr = c.CPU.REGS.QRS + c.CPU.REGS.WX;
                    valueAddr = 0x7000;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Irrr_rrI_value_Innn_nnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (QRS+WX),(0x7000+4),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    addr = c.CPU.REGS.QRS + c.CPU.REGS.WX;
                    valueAddr = 0x7004;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Irrr_rrI_value_Innn_rI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (QRS+WX),(0x7000+X),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5; // target setup
                    c.CPU.REGS.X = 3;  // value setup
                    addr = c.CPU.REGS.QRS + c.CPU.REGS.WX;
                    valueAddr = (uint)(0x7000 + c.CPU.REGS.X);
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Irrr_rrI_value_Innn_rrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (QRS+WX),(0x7000+WX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5; // target setup (also affects value)
                    c.CPU.REGS.WX = 5; // value setup
                    addr = c.CPU.REGS.QRS + c.CPU.REGS.WX;
                    valueAddr = (uint)(0x7000 + c.CPU.REGS.WX);
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Irrr_rrI_value_Innn_rrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (QRS+WX),(0x7000+VWX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;  // target setup
                    c.CPU.REGS.VWX = 7; // value setup
                    addr = c.CPU.REGS.QRS + c.CPU.REGS.WX;
                    valueAddr = (uint)(0x7000 + c.CPU.REGS.VWX);
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Irrr_rrI_value_IrrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (QRS+WX),(TUV),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.CPU.REGS.TUV = 0x8000;
                    addr = c.CPU.REGS.QRS + c.CPU.REGS.WX;
                    valueAddr = c.CPU.REGS.TUV;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Irrr_rrI_value_Irrr_nnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (QRS+WX),(TUV+4),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.CPU.REGS.TUV = 0x8000;
                    addr = c.CPU.REGS.QRS + c.CPU.REGS.WX;
                    valueAddr = c.CPU.REGS.TUV + 4;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Irrr_rrI_value_Irrr_rI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (QRS+WX),(TUV+X),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.CPU.REGS.TUV = 0x8000;
                    c.CPU.REGS.X = 3;
                    addr = c.CPU.REGS.QRS + c.CPU.REGS.WX;
                    valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.X;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Irrr_rrI_value_Irrr_rrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (QRS+WX),(TUV+WX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;        // target setup (and also value)
                    c.CPU.REGS.TUV = 0x8000;  // value setup part 1
                    c.CPU.REGS.WX = 5;        // value setup part 2
                    addr = c.CPU.REGS.QRS + c.CPU.REGS.WX;
                    valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.WX;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Irrr_rrI_value_Irrr_rrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (QRS+WX),(TUV+VWX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;        // target setup
                    c.CPU.REGS.TUV = 0x8000;  // value setup part 1
                    c.CPU.REGS.VWX = 7;       // value setup part 2
                    addr = c.CPU.REGS.QRS + c.CPU.REGS.WX;
                    valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.VWX;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Irrr_rrI_fr() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (QRS+WX),F0",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    addr = c.CPU.REGS.QRS + c.CPU.REGS.WX;
                    c.MEMC.SetFloatToRam(addr, 10.0f);
                    c.CPU.FREGS.SetRegister(0, 2.5f);
                },
                c => Assert.Equal(25.0f, c.MEMC.GetFloatFromRAM(addr)));
        }

        // --------------------
        // Target: (QRS+VWX)
        // --------------------

        [Fact]
        public void MUL_mem_Irrr_rrrI_block_immediate_2bytes_once() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (QRS+VWX),0x00000002,2",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    addr = c.CPU.REGS.QRS + c.CPU.REGS.VWX;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                },
                c => Assert.Equal([ 0x00, 0x14 ], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Irrr_rrrI_block_immediate_2bytes_twice() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (QRS+VWX),0x00000002,2,2",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    addr = c.CPU.REGS.QRS + c.CPU.REGS.VWX;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Irrr_rrrI_r() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (QRS+VWX),B",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    addr = c.CPU.REGS.QRS + c.CPU.REGS.VWX;
                    c.MEMC.Set8bitToRAM(addr, 10);
                    c.CPU.REGS.B = 5;
                },
                c => Assert.Equal((byte)50, c.MEMC.Get8bitFromRAM(addr)));
        }

        [Fact]
        public void MUL_mem_Irrr_rrrI_rr() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (QRS+VWX),CD",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    addr = c.CPU.REGS.QRS + c.CPU.REGS.VWX;
                    c.MEMC.Set16bitToRAM(addr, 100);
                    c.CPU.REGS.CD = 10;
                },
                c => Assert.Equal((ushort)1000, c.MEMC.Get16bitFromRAM(addr)));
        }

        [Fact]
        public void MUL_mem_Irrr_rrrI_rrr() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (QRS+VWX),DEF",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    addr = c.CPU.REGS.QRS + c.CPU.REGS.VWX;
                    c.MEMC.Set24bitToRAM(addr, 10000);
                    c.CPU.REGS.DEF = 10;
                },
                c => Assert.Equal((uint)100000, c.MEMC.Get24bitFromRAM(addr)));
        }

        [Fact]
        public void MUL_mem_Irrr_rrrI_rrrr() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (QRS+VWX),EFGH",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    addr = c.CPU.REGS.QRS + c.CPU.REGS.VWX;
                    c.MEMC.Set32bitToRAM(addr, 100_000);
                    c.CPU.REGS.EFGH = 10;
                },
                c => Assert.Equal((uint)1_000_000, c.MEMC.Get32bitFromRAM(addr)));
        }

        [Fact]
        public void MUL_mem_Irrr_rrrI_value_InnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (QRS+VWX),(0x7000),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    addr = c.CPU.REGS.QRS + c.CPU.REGS.VWX;
                    valueAddr = 0x7000;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Irrr_rrrI_value_Innn_nnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (QRS+VWX),(0x7000+4),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    addr = c.CPU.REGS.QRS + c.CPU.REGS.VWX;
                    valueAddr = 0x7004;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Irrr_rrrI_value_Innn_rI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (QRS+VWX),(0x7000+X),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.CPU.REGS.X = 3;
                    addr = c.CPU.REGS.QRS + c.CPU.REGS.VWX;
                    valueAddr = (uint)(0x7000 + c.CPU.REGS.X);
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Irrr_rrrI_value_Innn_rrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (QRS+VWX),(0x7000+WX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.CPU.REGS.WX = 5;
                    addr = c.CPU.REGS.QRS + c.CPU.REGS.VWX;
                    valueAddr = (uint)(0x7000 + c.CPU.REGS.WX);
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Irrr_rrrI_value_Innn_rrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (QRS+VWX),(0x7000+VWX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7; // target setup (also affects value)
                    c.CPU.REGS.VWX = 7; // value setup
                    addr = c.CPU.REGS.QRS + c.CPU.REGS.VWX;
                    valueAddr = (uint)(0x7000 + c.CPU.REGS.VWX);
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Irrr_rrrI_value_IrrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (QRS+VWX),(TUV),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.CPU.REGS.TUV = 0x8000;
                    addr = c.CPU.REGS.QRS + c.CPU.REGS.VWX;
                    valueAddr = c.CPU.REGS.TUV;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Irrr_rrrI_value_Irrr_nnnI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (QRS+VWX),(TUV+4),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.CPU.REGS.TUV = 0x8000;
                    addr = c.CPU.REGS.QRS + c.CPU.REGS.VWX;
                    valueAddr = c.CPU.REGS.TUV + 4;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Irrr_rrrI_value_Irrr_rI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (QRS+VWX),(TUV+X),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.CPU.REGS.TUV = 0x8000;
                    c.CPU.REGS.X = 3;
                    addr = c.CPU.REGS.QRS + c.CPU.REGS.VWX;
                    valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.X;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Irrr_rrrI_value_Irrr_rrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (QRS+VWX),(TUV+WX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.CPU.REGS.TUV = 0x8000;
                    c.CPU.REGS.WX = 5;
                    addr = c.CPU.REGS.QRS + c.CPU.REGS.VWX;
                    valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.WX;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Irrr_rrrI_value_Irrr_rrrI_block() // Ok
        {
            uint addr = 0;
            uint valueAddr = 0;

            RunTest(
                "MUL (QRS+VWX),(TUV+VWX),2,GHI",
                c =>
                {
                    c.CPU.REGS.GHI = 2;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;       // target setup (also affects value)
                    c.CPU.REGS.TUV = 0x8000;  // value setup part 1
                    c.CPU.REGS.VWX = 7;       // value setup part 2
                    addr = c.CPU.REGS.QRS + c.CPU.REGS.VWX;
                    valueAddr = c.CPU.REGS.TUV + c.CPU.REGS.VWX;
                    c.LoadMemAt(addr, [0x00, 0x0A]);
                    c.LoadMemAt(valueAddr, [0x00, 0x02]);
                },
                c => Assert.Equal([0x00, 0x28], c.MEMC.RAM.GetMemoryAt(addr, 2)));
        }

        [Fact]
        public void MUL_mem_Irrr_rrrI_fr() // Ok
        {
            uint addr = 0;

            RunTest(
                "MUL (QRS+VWX),F0",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    addr = c.CPU.REGS.QRS + c.CPU.REGS.VWX;
                    c.MEMC.SetFloatToRam(addr, 10.0f);
                    c.CPU.FREGS.SetRegister(0, 2.5f);
                },
                c => Assert.Equal(25.0f, c.MEMC.GetFloatFromRAM(addr)));
        }

        #endregion

        #region MUL fr,* tests (16 sub-ops)

        [Fact]
        public void MUL_fr_nnnn() // Ok
        {
            RunTest(
                "MUL F0,4.0",
                c => c.CPU.FREGS.SetRegister(0, 10.0f),
                c => Assert.Equal(40.0f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void MUL_fr_r() // Ok
        {
            RunTest(
                "MUL F0,A",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 10.0f);
                    c.CPU.REGS.A = 4;
                },
                c => Assert.Equal(40.0f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void MUL_fr_rr() // Ok
        {
            RunTest(
                "MUL F0,AB",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 10.0f);
                    c.CPU.REGS.AB = 4;
                },
                c => Assert.Equal(40.0f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void MUL_fr_rrr() // Ok
        {
            RunTest(
                "MUL F0,ABC",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 10.0f);
                    c.CPU.REGS.ABC = 4;
                },
                c => Assert.Equal(40.0f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void MUL_fr_rrrr() // Ok
        {
            RunTest(
                "MUL F0,ABCD",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 10.0f);
                    c.CPU.REGS.ABCD = 4;
                },
                c => Assert.Equal(40.0f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void MUL_fr_InnnI() // Ok
        {
            RunTest(
                "MUL F0,(0x9000)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 10.0f);
                    c.MEMC.SetFloatToRam(0x9000, 2.5f);
                },
                c => Assert.Equal(25.0f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void MUL_fr_Innn_nnnI() // Ok
        {
            RunTest(
                "MUL F0,(0x9000+4)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 10.0f);
                    c.MEMC.SetFloatToRam(0x9004, 2.5f);
                },
                c => Assert.Equal(25.0f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void MUL_fr_Innn_rI() // Ok
        {
            RunTest(
                "MUL F0,(0x9000+X)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 10.0f);
                    c.CPU.REGS.X = 3;
                    c.MEMC.SetFloatToRam(0x9003, 2.5f);
                },
                c => Assert.Equal(25.0f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void MUL_fr_Innn_rrI() // Ok
        {
            RunTest(
                "MUL F0,(0x9000+WX)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 10.0f);
                    c.CPU.REGS.WX = 5;
                    c.MEMC.SetFloatToRam(0x9005, 2.5f);
                },
                c => Assert.Equal(25.0f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void MUL_fr_Innn_rrrI() // Ok
        {
            RunTest(
                "MUL F0,(0x9000+VWX)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 10.0f);
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.SetFloatToRam(0x9007, 2.5f);
                },
                c => Assert.Equal(25.0f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void MUL_fr_IrrrI() // Ok
        {
            RunTest(
                "MUL F0,(QRS)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 10.0f);
                    c.CPU.REGS.QRS = 0xA000;
                    c.MEMC.SetFloatToRam(0xA000, 2.5f);
                },
                c => Assert.Equal(25.0f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void MUL_fr_Irrr_nnnI() // Ok
        {
            RunTest(
                "MUL F0,(QRS+4)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 10.0f);
                    c.CPU.REGS.QRS = 0xA000;
                    c.MEMC.SetFloatToRam(0xA004, 2.5f);
                },
                c => Assert.Equal(25.0f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void MUL_fr_Irrr_rI() // Ok
        {
            RunTest(
                "MUL F0,(QRS+X)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 10.0f);
                    c.CPU.REGS.QRS = 0xA000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.SetFloatToRam(0xA003, 2.5f);
                },
                c => Assert.Equal(25.0f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void MUL_fr_Irrr_rrI() // Ok
        {
            RunTest(
                "MUL F0,(QRS+WX)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 10.0f);
                    c.CPU.REGS.QRS = 0xA000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.SetFloatToRam(0xA005, 2.5f);
                },
                c => Assert.Equal(25.0f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void MUL_fr_Irrr_rrrI() // Ok
        {
            RunTest(
                "MUL F0,(QRS+VWX)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 10.0f);
                    c.CPU.REGS.QRS = 0xA000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.SetFloatToRam(0xA007, 2.5f);
                },
                c => Assert.Equal(25.0f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void MUL_fr_fr() // Ok
        {
            RunTest(
                "MUL F0,F1",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 10.0f);
                    c.CPU.FREGS.SetRegister(1, 2.5f);
                },
                c => Assert.Equal(25.0f, c.CPU.FREGS.GetRegister(0)));
        }

        #endregion
    }
}








