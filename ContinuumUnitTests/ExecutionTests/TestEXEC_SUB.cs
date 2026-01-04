using Continuum93.Emulator;
using Continuum93.Emulator.Interpreter;

namespace ExecutionTests
{
    public class TestEXEC_SUB
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

        #endregion

        #region SUB r,* tests

        [Fact]
        public void SUB_r_n()   // Ok
        {
            RunTest(
                "SUB A,5",
                c => c.CPU.REGS.A = 10,
                c => Assert.Equal((byte)5, c.CPU.REGS.A));
        }

        [Fact]
        public void SUB_r_r()   // Ok
        {
            RunTest(
                "SUB A,B",
                c =>
                {
                    c.CPU.REGS.A = 10;
                    c.CPU.REGS.B = 3;
                },
                c => Assert.Equal((byte)7, c.CPU.REGS.A));
        }

        [Fact]
        public void SUB_r_InnnI()   // Ok
        {
            RunTest(
                "SUB A,(0x4000)",
                c =>
                {
                    c.CPU.REGS.A = 10;
                    c.MEMC.Set8bitToRAM(0x4000, 51);
                },
                c => Assert.Equal((byte)215, c.CPU.REGS.A));
        }

        [Fact]
        public void SUB_r_Innn_nnnI()   // Ok
        {
            RunTest(
                "SUB A,(0x4000+4)",
                c =>
                {
                    c.CPU.REGS.A = 10;
                    c.MEMC.Set8bitToRAM(0x4004, 51);
                },
                c => Assert.Equal((byte)215, c.CPU.REGS.A));
        }

        [Fact]
        public void SUB_r_Innn_rI() // Ok
        {
            RunTest(
                "SUB A,(0x4000+X)",
                c =>
                {
                    c.CPU.REGS.A = 10;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set8bitToRAM(0x4003, 51);
                },
                c => Assert.Equal((byte)215, c.CPU.REGS.A));
        }

        [Fact]
        public void SUB_r_Innn_rrI()    // Ok
        {
            RunTest(
                "SUB A,(0x4000+WX)",
                c =>
                {
                    c.CPU.REGS.A = 10;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x4005, 51);
                },
                c => Assert.Equal((byte)215, c.CPU.REGS.A));
        }

        [Fact]
        public void SUB_r_Innn_rrrI()   // Ok
        {
            RunTest(
                "SUB A,(0x4000+VWX)",
                c =>
                {
                    c.CPU.REGS.A = 10;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x4007, 51);
                },
                c => Assert.Equal((byte)215, c.CPU.REGS.A));
        }

        [Fact]
        public void SUB_r_IrrrI()   // Ok
        {
            RunTest(
                "SUB A,(QRS)",
                c =>
                {
                    c.CPU.REGS.A = 10;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x5000, 51);
                },
                c => Assert.Equal((byte)215, c.CPU.REGS.A));
        }

        [Fact]
        public void SUB_r_Irrr_nnnI()   // Ok
        {
            RunTest(
                "SUB A,(QRS+4)",
                c =>
                {
                    c.CPU.REGS.A = 10;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x5004, 51);
                },
                c => Assert.Equal((byte)215, c.CPU.REGS.A));
        }

        [Fact]
        public void SUB_r_Irrr_rI() // Ok
        {
            RunTest(
                "SUB A,(QRS+X)",
                c =>
                {
                    c.CPU.REGS.A = 10;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set8bitToRAM(0x5003, 51);
                },
                c => Assert.Equal((byte)215, c.CPU.REGS.A));
        }

        [Fact]
        public void SUB_r_Irrr_rrI()    // Ok
        {
            RunTest(
                "SUB A,(QRS+WX)",
                c =>
                {
                    c.CPU.REGS.A = 10;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x5005, 51);
                },
                c => Assert.Equal((byte)215, c.CPU.REGS.A));
        }

        [Fact]
        public void SUB_r_Irrr_rrrI()   // Ok
        {
            RunTest(
                "SUB A,(QRS+VWX)",
                c =>
                {
                    c.CPU.REGS.A = 10;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x5007, 51);
                },
                c => Assert.Equal((byte)215, c.CPU.REGS.A));
        }

        [Fact]
        public void SUB_r_fr()  // Ok
        {
            RunTest(
                "SUB A,F0",
                c =>
                {
                    c.CPU.REGS.A = 10;
                    c.CPU.FREGS.SetRegister(0, 1.5f);
                },
                c => Assert.Equal((byte)8, c.CPU.REGS.A));
        }

        #endregion

        #region SUB rr,* tests

        [Fact]
        public void SUB_rr_nn() // Ok
        {
            RunTest(
                "SUB AB,0x1234",
                c => c.CPU.REGS.AB = 0x1000,
                c => Assert.Equal((ushort)0xFDCC, c.CPU.REGS.AB));
        }

        [Fact]
        public void SUB_rr_r()  // Ok
        {
            RunTest(
                "SUB AB,E",
                c =>
                {
                    c.CPU.REGS.AB = 0x1000;
                    c.CPU.REGS.E = 4;
                },
                c => Assert.Equal((ushort)0x0FFC, c.CPU.REGS.AB));
        }

        [Fact]
        public void SUB_rr_rr() // Ok
        {
            RunTest(
                "SUB AB,CD",
                c =>
                {
                    c.CPU.REGS.AB = 0x1000;
                    c.CPU.REGS.CD = 0x00FF;
                },
                c => Assert.Equal((ushort)0x0F01, c.CPU.REGS.AB));
        }

        [Fact]
        public void SUB_rr_InnnI()  // Ok
        {
            RunTest(
                "SUB AB,(0x4000)",
                c =>
                {
                    c.CPU.REGS.AB = 0x1000;
                    c.MEMC.Set16bitToRAM(0x4000, 0x0F0F);
                },
                c => Assert.Equal((ushort)0x00F1, c.CPU.REGS.AB));
        }

        [Fact]
        public void SUB_rr_Innn_nnnI()  // Ok
        {
            RunTest(
                "SUB AB,(0x4000+4)",
                c =>
                {
                    c.CPU.REGS.AB = 0x1000;
                    c.MEMC.Set16bitToRAM(0x4004, 0x0F0F);
                },
                c => Assert.Equal((ushort)0x00F1, c.CPU.REGS.AB));
        }

        [Fact]
        public void SUB_rr_Innn_rI()    // Ok
        {
            RunTest(
                "SUB AB,(0x4000+X)",
                c =>
                {
                    c.CPU.REGS.AB = 0x1000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set16bitToRAM(0x4003, 0x0F0F);
                },
                c => Assert.Equal((ushort)0x00F1, c.CPU.REGS.AB));
        }

        [Fact]
        public void SUB_rr_Innn_rrI()   // Ok
        {
            RunTest(
                "SUB AB,(0x4000+WX)",
                c =>
                {
                    c.CPU.REGS.AB = 0x1000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set16bitToRAM(0x4005, 0x0F0F);
                },
                c => Assert.Equal((ushort)0x00F1, c.CPU.REGS.AB));
        }

        [Fact]
        public void SUB_rr_Innn_rrrI()  // Ok
        {
            RunTest(
                "SUB AB,(0x4000+VWX)",
                c =>
                {
                    c.CPU.REGS.AB = 0x1000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set16bitToRAM(0x4007, 0x0F0F);
                },
                c => Assert.Equal((ushort)0x00F1, c.CPU.REGS.AB));
        }

        [Fact]
        public void SUB_rr_IrrrI()  // Ok
        {
            RunTest(
                "SUB AB,(QRS)",
                c =>
                {
                    c.CPU.REGS.AB = 0x1000;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set16bitToRAM(0x5000, 0x0F0F);
                },
                c => Assert.Equal((ushort)0x00F1, c.CPU.REGS.AB));
        }

        [Fact]
        public void SUB_rr_Irrr_nnnI()  // Ok
        {
            RunTest(
                "SUB AB,(QRS+4)",
                c =>
                {
                    c.CPU.REGS.AB = 0x1000;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set16bitToRAM(0x5004, 0x0F0F);
                },
                c => Assert.Equal((ushort)0x00F1, c.CPU.REGS.AB));
        }

        [Fact]
        public void SUB_rr_Irrr_rI()    // Ok
        {
            RunTest(
                "SUB AB,(QRS+X)",
                c =>
                {
                    c.CPU.REGS.AB = 0x1000;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set16bitToRAM(0x5003, 0x0F0F);
                },
                c => Assert.Equal((ushort)0x00F1, c.CPU.REGS.AB));
        }

        [Fact]
        public void SUB_rr_Irrr_rrI()   // Ok
        {
            RunTest(
                "SUB AB,(QRS+WX)",
                c =>
                {
                    c.CPU.REGS.AB = 0x1000;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set16bitToRAM(0x5005, 0x0F0F);
                },
                c => Assert.Equal((ushort)0x00F1, c.CPU.REGS.AB));
        }

        [Fact]
        public void SUB_rr_Irrr_rrrI()  // Ok
        {
            RunTest(
                "SUB AB,(QRS+VWX)",
                c =>
                {
                    c.CPU.REGS.AB = 0x1000;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set16bitToRAM(0x5007, 0x0F0F);
                },
                c => Assert.Equal((ushort)0x00F1, c.CPU.REGS.AB));
        }

        [Fact]
        public void SUB_rr_fr() // Ok
        {
            RunTest(
                "SUB AB,F1",
                c =>
                {
                    c.CPU.REGS.AB = 0x1000;
                    c.CPU.FREGS.SetRegister(1, 5.6f);
                },
                c => Assert.Equal((ushort)0x0FFA, c.CPU.REGS.AB));
        }

        #endregion

        #region SUB rrr,* tests

        [Fact]
        public void SUB_rrr_nnn()   // Ok
        {
            RunTest(
                "SUB ABC,0x000102",
                c => c.CPU.REGS.ABC = 0x010203,
                c => Assert.Equal((uint)0x010101, c.CPU.REGS.ABC));
        }

        [Fact]
        public void SUB_rrr_r() // Ok
        {
            RunTest(
                "SUB ABC,E",
                c =>
                {
                    c.CPU.REGS.ABC = 0x010203;
                    c.CPU.REGS.E = 7;
                },
                c => Assert.Equal((uint)0x0101FC, c.CPU.REGS.ABC));
        }

        [Fact]
        public void SUB_rrr_rr()    // Ok
        {
            RunTest(
                "SUB ABC,EF",
                c =>
                {
                    c.CPU.REGS.ABC = 0x010203;
                    c.CPU.REGS.EF = 0x0102;
                },
                c => Assert.Equal((uint)0x010101, c.CPU.REGS.ABC));
        }

        [Fact]
        public void SUB_rrr_rrr()   // Ok
        {
            RunTest(
                "SUB ABC,DEF",
                c =>
                {
                    c.CPU.REGS.ABC = 0x010203;
                    c.CPU.REGS.DEF = 0x00000F;
                },
                c => Assert.Equal((uint)0x0101F4, c.CPU.REGS.ABC));
        }

        [Fact]
        public void SUB_rrr_InnnI() // Ok
        {
            RunTest(
                "SUB ABC,(0x4000)",
                c =>
                {
                    c.CPU.REGS.ABC = 0x010203;
                    c.MEMC.Set24bitToRAM(0x4000, 0x000F0F);
                },
                c => Assert.Equal((uint)0x00F2F4, c.CPU.REGS.ABC));
        }

        [Fact]
        public void SUB_rrr_Innn_nnnI() // Ok
        {
            RunTest(
                "SUB ABC,(0x4000+4)",
                c =>
                {
                    c.CPU.REGS.ABC = 0x010203;
                    c.MEMC.Set24bitToRAM(0x4004, 0x000F0F);
                },
                c => Assert.Equal((uint)0x00F2F4, c.CPU.REGS.ABC));
        }

        [Fact]
        public void SUB_rrr_Innn_rI()   // Ok
        {
            RunTest(
                "SUB ABC,(0x4000+X)",
                c =>
                {
                    c.CPU.REGS.ABC = 0x010203;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set24bitToRAM(0x4003, 0x000F0F);
                },
                c => Assert.Equal((uint)0x00F2F4, c.CPU.REGS.ABC));
        }

        [Fact]
        public void SUB_rrr_Innn_rrI()  // Ok
        {
            RunTest(
                "SUB ABC,(0x4000+WX)",
                c =>
                {
                    c.CPU.REGS.ABC = 0x010203;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set24bitToRAM(0x4005, 0x000F0F);
                },
                c => Assert.Equal((uint)0x00F2F4, c.CPU.REGS.ABC));
        }

        [Fact]
        public void SUB_rrr_Innn_rrrI() // Ok
        {
            RunTest(
                "SUB ABC,(0x4000+VWX)",
                c =>
                {
                    c.CPU.REGS.ABC = 0x010203;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set24bitToRAM(0x4007, 0x000F0F);
                },
                c => Assert.Equal((uint)0x00F2F4, c.CPU.REGS.ABC));
        }

        [Fact]
        public void SUB_rrr_IrrrI() // Ok
        {
            RunTest(
                "SUB ABC,(QRS)",
                c =>
                {
                    c.CPU.REGS.ABC = 0x010203;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set24bitToRAM(0x5000, 0x000F0F);
                },
                c => Assert.Equal((uint)0x00F2F4, c.CPU.REGS.ABC));
        }

        [Fact]
        public void SUB_rrr_Irrr_nnnI() // Ok
        {
            RunTest(
                "SUB ABC,(QRS+4)",
                c =>
                {
                    c.CPU.REGS.ABC = 0x010203;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set24bitToRAM(0x5004, 0x000F0F);
                },
                c => Assert.Equal((uint)0x00F2F4, c.CPU.REGS.ABC));
        }

        [Fact]
        public void SUB_rrr_Irrr_rI()   // Ok
        {
            RunTest(
                "SUB ABC,(QRS+X)",
                c =>
                {
                    c.CPU.REGS.ABC = 0x010203;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set24bitToRAM(0x5003, 0x000F0F);
                },
                c => Assert.Equal((uint)0x00F2F4, c.CPU.REGS.ABC));
        }

        [Fact]
        public void SUB_rrr_Irrr_rrI()  // Ok
        {
            RunTest(
                "SUB ABC,(QRS+WX)",
                c =>
                {
                    c.CPU.REGS.ABC = 0x010203;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set24bitToRAM(0x5005, 0x000F0F);
                },
                c => Assert.Equal((uint)0x00F2F4, c.CPU.REGS.ABC));
        }

        [Fact]
        public void SUB_rrr_Irrr_rrrI() // Ok
        {
            RunTest(
                "SUB ABC,(QRS+VWX)",
                c =>
                {
                    c.CPU.REGS.ABC = 0x010203;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set24bitToRAM(0x5007, 0x000F0F);
                },
                c => Assert.Equal((uint)0x00F2F4, c.CPU.REGS.ABC));
        }

        [Fact]
        public void SUB_rrr_fr()    // Ok
        {
            RunTest(
                "SUB ABC,F2",
                c =>
                {
                    c.CPU.REGS.ABC = 0x010203;
                    c.CPU.FREGS.SetRegister(2, 12.5f);
                },
                c => Assert.Equal((uint)0x0101F7, c.CPU.REGS.ABC));
        }

        #endregion

        #region SUB rrrr,* tests

        [Fact]
        public void SUB_rrrr_nnnn() // Ok
        {
            RunTest(
                "SUB ABCD,0x00010002",
                c => c.CPU.REGS.ABCD = 0x10002000,
                c => Assert.Equal((uint)0x0FFF1FFE, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void SUB_rrrr_r()    // Ok
        {
            RunTest(
                "SUB ABCD,E",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x10002000;
                    c.CPU.REGS.E = 9;
                },
                c => Assert.Equal((uint)0x10001FF7, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void SUB_rrrr_rr()   // Ok
        {
            RunTest(
                "SUB ABCD,EF",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x10002000;
                    c.CPU.REGS.EF = 0x00FF;
                },
                c => Assert.Equal((uint)0x10001F01, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void SUB_rrrr_rrr()  // Ok
        {
            RunTest(
                "SUB ABCD,DEF",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x10002000;
                    c.CPU.REGS.DEF = 0x0000AA;
                },
                c => Assert.Equal((uint)0x10001F56, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void SUB_rrrr_rrrr() // Ok
        {
            RunTest(
                "SUB ABCD,EFGH",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x10002000;
                    c.CPU.REGS.EFGH = 0x00000010;
                },
                c => Assert.Equal((uint)0x10001FF0, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void SUB_rrrr_InnnI()    // Ok
        {
            RunTest(
                "SUB ABCD,(0x4000)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x10002000;
                    c.MEMC.Set32bitToRAM(0x4000, 0x000000FF);
                },
                c => Assert.Equal((uint)0x10001F01, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void SUB_rrrr_Innn_nnnI()    // Ok
        {
            RunTest(
                "SUB ABCD,(0x4000+4)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x10002000;
                    c.MEMC.Set32bitToRAM(0x4004, 0x000000FF);
                },
                c => Assert.Equal((uint)0x10001F01, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void SUB_rrrr_Innn_rI()  // Ok
        {
            RunTest(
                "SUB ABCD,(0x4000+X)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x10002000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set32bitToRAM(0x4003, 0x000000FF);
                },
                c => Assert.Equal((uint)0x10001F01, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void SUB_rrrr_Innn_rrI() // Ok
        {
            RunTest(
                "SUB ABCD,(0x4000+WX)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x10002000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set32bitToRAM(0x4005, 0x000000FF);
                },
                c => Assert.Equal((uint)0x10001F01, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void SUB_rrrr_Innn_rrrI()    // Ok
        {
            RunTest(
                "SUB ABCD,(0x4000+VWX)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x10002000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set32bitToRAM(0x4007, 0x000000FF);
                },
                c => Assert.Equal((uint)0x10001F01, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void SUB_rrrr_IrrrI()    // Ok
        {
            RunTest(
                "SUB ABCD,(QRS)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x10002000;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set32bitToRAM(0x5000, 0x000000FF);
                },
                c => Assert.Equal((uint)0x10001F01, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void SUB_rrrr_Irrr_nnnI()    // Ok
        {
            RunTest(
                "SUB ABCD,(QRS+4)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x10002000;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set32bitToRAM(0x5004, 0x000000FF);
                },
                c => Assert.Equal((uint)0x10001F01, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void SUB_rrrr_Irrr_rI()  // Ok
        {
            RunTest(
                "SUB ABCD,(QRS+X)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x10002000;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set32bitToRAM(0x5003, 0x000000FF);
                },
                c => Assert.Equal((uint)0x10001F01, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void SUB_rrrr_Irrr_rrI() // Ok
        {
            RunTest(
                "SUB ABCD,(QRS+WX)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x10002000;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set32bitToRAM(0x5005, 0x000000FF);
                },
                c => Assert.Equal((uint)0x10001F01, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void SUB_rrrr_Irrr_rrrI()    // Ok
        {
            RunTest(
                "SUB ABCD,(QRS+VWX)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x10002000;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set32bitToRAM(0x5007, 0x000000FF);
                },
                c => Assert.Equal((uint)0x10001F01, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void SUB_rrrr_fr()   // Ok
        {
            RunTest(
                "SUB ABCD,F3",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x10002000;
                    c.CPU.FREGS.SetRegister(3, 42.8f);
                },
                c => Assert.Equal((uint)0x10001FD5, c.CPU.REGS.ABCD));
        }

        #endregion

        #region SUB (nnn),* tests - immediate and register

        [Fact]
        public void SUB_InnnI_nnnn_n()  // Ok
        {
            RunTest(
                "SUB (0x4000),0x00010203,2",
                c => c.LoadMemAt(0x4000, [0x01, 0x02, 0x03]),
                c => Assert.Equal([0xFE, 0xFF], c.MEMC.RAM.GetMemoryAt(0x4000, 2)));
        }

        [Fact]
        public void SUB_InnnI_nnnn_n_repeat()   // Ok
        {
            RunTest(
                "SUB (0x4000),0x00010203,3,2",
                c => c.LoadMemAt(0x4000, [0x01, 0x02, 0x03]),
                c => Assert.Equal([0xFE, 0xFD, 0xFD], c.MEMC.RAM.GetMemoryAt(0x4000, 3)));
        }

        [Fact]
        public void SUB_InnnI_r()   // Ok
        {
            RunTest(
                "SUB (0x4000),B",
                c =>
                {
                    c.LoadMemAt(0x4000, [0x01, 0x02]);
                    c.CPU.REGS.B = 0x05;
                },
                c => Assert.Equal(0xFC, c.MEMC.Get8bitFromRAM(0x4000)));
        }

        [Fact]
        public void SUB_InnnI_rr()  // Ok
        {
            RunTest(
                "SUB (0x4000),CD",
                c =>
                {
                    c.LoadMemAt(0x4000, [0x01, 0x01]);
                    c.CPU.REGS.CD = 0x0102;
                },
                c => Assert.Equal(0xFFFF, c.MEMC.Get16bitFromRAM(0x4000)));
        }

        [Fact]
        public void SUB_InnnI_rrr() // Ok
        {
            RunTest(
                "SUB (0x4000),DEF",
                c =>
                {
                    c.LoadMemAt(0x4000, [0x0A, 0x14, 0x1E]);
                    c.CPU.REGS.DEF = 0x000203;
                },
                c => Assert.Equal((uint)0x0A121B, c.MEMC.Get24bitFromRAM(0x4000)));
        }

        [Fact]
        public void SUB_InnnI_rrrr()    // Ok
        {
            RunTest(
                "SUB (0x4000),EFGH",
                c =>
                {
                    c.LoadMemAt(0x4000, [0x01, 0x02, 0x03, 0x04]);
                    c.CPU.REGS.EFGH = 0x00030405;
                },
                c => Assert.Equal((uint)0x00FEFEFF, c.MEMC.Get32bitFromRAM(0x4000)));
        }

        [Fact]
        public void SUB_InnnI_InnnI_n_rrr() // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (0x4000),(0x7000),3,GHI",
                c =>
                {
                    c.LoadMemAt(0x4000, initial);
                    c.LoadMemAt(0x7000, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4000, 4));
                });
        }

        [Fact]
        public void SUB_InnnI_Innn_nnnI_n_rrr() // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (0x4000),(0x7000+4),3,GHI",
                c =>
                {
                    c.LoadMemAt(0x4000, initial);
                    c.LoadMemAt(0x7004, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4000, 4));
                });
        }

        [Fact]
        public void SUB_InnnI_Innn_rI_n_rrr()   // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (0x4000),(0x7000+K),3,GHI",
                c =>
                {
                    c.LoadMemAt(0x4000, initial);
                    c.CPU.REGS.K = 2;
                    c.LoadMemAt(0x7002, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4000, 4));
                });
        }

        [Fact]
        public void SUB_InnnI_Innn_rrI_n_rrr()  // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (0x4000),(0x7000+KL),3,GHI",
                c =>
                {
                    c.LoadMemAt(0x4000, initial);
                    c.CPU.REGS.KL = 6;
                    c.LoadMemAt(0x7006, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4000, 4));
                });
        }

        [Fact]
        public void SUB_InnnI_Innn_rrrI_n_rrr() // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (0x4000),(0x7000+KLM),3,GHI",
                c =>
                {
                    c.LoadMemAt(0x4000, initial);
                    c.CPU.REGS.KLM = 9;
                    c.LoadMemAt(0x7009, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4000, 4));
                });
        }

        [Fact]
        public void SUB_InnnI_IrrrI_n_rrr() // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (0x4000),(DEF),3,GHI",
                c =>
                {
                    c.LoadMemAt(0x4000, initial);
                    c.CPU.REGS.DEF = 0x7100;
                    c.LoadMemAt(0x7100, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4000, 4));
                });
        }

        [Fact]
        public void SUB_InnnI_Irrr_nnnI_n_rrr() // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (0x4000),(DEF+4),3,GHI",
                c =>
                {
                    c.LoadMemAt(0x4000, initial);
                    c.CPU.REGS.DEF = 0x7100;
                    c.LoadMemAt(0x7104, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4000, 4));
                });
        }

        [Fact]
        public void SUB_InnnI_Irrr_rI_n_rrr()   // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (0x4000),(DEF+K),3,GHI",
                c =>
                {
                    c.LoadMemAt(0x4000, initial);
                    c.CPU.REGS.DEF = 0x7100;
                    c.CPU.REGS.K = 2;
                    c.LoadMemAt(0x7102, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4000, 4));
                });
        }

        [Fact]
        public void SUB_InnnI_Irrr_rrI_n_rrr()  // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (0x4000),(DEF+KL),3,GHI",
                c =>
                {
                    c.LoadMemAt(0x4000, initial);
                    c.CPU.REGS.DEF = 0x7100;
                    c.CPU.REGS.KL = 6;
                    c.LoadMemAt(0x7106, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4000, 4));
                });
        }

        [Fact]
        public void SUB_InnnI_Irrr_rrrI_n_rrr() // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (0x4000),(DEF+KLM),3,GHI",
                c =>
                {
                    c.LoadMemAt(0x4000, initial);
                    c.CPU.REGS.DEF = 0x7100;
                    c.CPU.REGS.KLM = 9;
                    c.LoadMemAt(0x7109, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4000, 4));
                });
        }

        [Fact]
        public void SUB_InnnI_fr()  // Ok
        {
            RunTest(
                "SUB (0x4000),F4",
                c =>
                {
                    c.MEMC.SetFloatToRam(0x4000, 1.25f);
                    c.CPU.FREGS.SetRegister(4, 2.5f);
                },
                c => Assert.Equal(-1.25f, c.MEMC.GetFloatFromRAM(0x4000)));
        }

        [Fact]
        public void SUB_Innn_nnnI_nnnn_n()  // Ok
        {
            RunTest(
                "SUB (0x4000+4),0x00010203,2",
                c => c.LoadMemAt(0x4004, [0x01, 0x02, 0x03]),
                c => Assert.Equal([0xFE, 0xFF, 0x03], c.MEMC.RAM.GetMemoryAt(0x4004, 3)));
        }

        [Fact]
        public void SUB_Innn_nnnI_nnnn_n_nnn()  // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            RunTest(
                "SUB (0x4000+4),0x00010203,3,2",
                c =>
                {
                    c.LoadMemAt(0x4004, initial);
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4004, 4));
                });
        }

        [Fact]
        public void SUB_Innn_nnnI_r()   // Ok
        {
            RunTest(
                "SUB (0x4000+4),B",
                c =>
                {
                    c.LoadMemAt(0x4004, [1, 2]);
                    c.CPU.REGS.B = 5;
                },
                c => Assert.Equal(0xFC, c.MEMC.Get8bitFromRAM(0x4004)));
        }

        [Fact]
        public void SUB_Innn_nnnI_rr()  // Ok
        {
            RunTest(
                "SUB (0x4000+4),CD",
                c =>
                {
                    c.LoadMemAt(0x4004, [1, 1]);
                    c.CPU.REGS.CD = 0x0102;
                },
                c => Assert.Equal((ushort)0xFFFF, c.MEMC.Get16bitFromRAM(0x4004)));
        }

        [Fact]
        public void SUB_Innn_nnnI_rrr() // Ok
        {
            RunTest(
                "SUB (0x4000+4),DEF",
                c =>
                {
                    c.LoadMemAt(0x4004, [0x0A, 0x14, 0x1E]);
                    c.CPU.REGS.DEF = 0x000203;
                },
                c => Assert.Equal((uint)0x0A121B, c.MEMC.Get24bitFromRAM(0x4004)));
        }

        [Fact]
        public void SUB_Innn_nnnI_rrrr()    // Ok
        {
            RunTest(
                "SUB (0x4000+4),EFGH",
                c =>
                {
                    c.LoadMemAt(0x4004, [0x01, 0x01, 0x01, 0x01]);
                    c.CPU.REGS.EFGH = 0x00030405;
                },
                c => Assert.Equal((uint)0x00FDFCFC, c.MEMC.Get32bitFromRAM(0x4004)));
        }

        [Fact]
        public void SUB_Innn_nnnI_InnnI_n_rrr() // Ok
        {
            byte[] initial = [10, 20, 30, 40];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (0x4000+4),(0x7000),3,GHI",
                c =>
                {
                    c.LoadMemAt(0x4004, initial);
                    c.LoadMemAt(0x7000, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [8, 16, 24, 40];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4004, 4));
                });
        }

        [Fact]
        public void SUB_Innn_nnnI_Innn_nnnI_n_rrr() // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (0x4000+4),(0x7000+4),3,GHI",
                c =>
                {
                    c.LoadMemAt(0x4004, initial);
                    c.LoadMemAt(0x7004, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4004, 4));
                });
        }

        [Fact]
        public void SUB_Innn_nnnI_Innn_rI_n_rrr()   // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (0x4000+4),(0x7000+K),3,GHI",
                c =>
                {
                    c.LoadMemAt(0x4004, initial);
                    c.CPU.REGS.K = 2;
                    c.LoadMemAt(0x7002, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4004, 4));
                });
        }

        [Fact]
        public void SUB_Innn_nnnI_Innn_rrI_n_rrr()  // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (0x4000+4),(0x7000+KL),3,GHI",
                c =>
                {
                    c.LoadMemAt(0x4004, initial);
                    c.CPU.REGS.KL = 6;
                    c.LoadMemAt(0x7006, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4004, 4));
                });
        }

        [Fact]
        public void SUB_Innn_nnnI_Innn_rrrI_n_rrr() // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (0x4000+4),(0x7000+KLM),3,GHI",
                c =>
                {
                    c.LoadMemAt(0x4004, initial);
                    c.CPU.REGS.KLM = 9;
                    c.LoadMemAt(0x7009, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4004, 4));
                });
        }

        [Fact]
        public void SUB_Innn_nnnI_IrrrI_n_rrr() // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (0x4000+4),(DEF),3,GHI",
                c =>
                {
                    c.LoadMemAt(0x4004, initial);
                    c.CPU.REGS.DEF = 0x7100;
                    c.LoadMemAt(0x7100, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4004, 4));
                });
        }

        [Fact]
        public void SUB_Innn_nnnI_Irrr_nnnI_n_rrr() // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (0x4000+4),(DEF+4),3,GHI",
                c =>
                {
                    c.LoadMemAt(0x4004, initial);
                    c.CPU.REGS.DEF = 0x7100;
                    c.LoadMemAt(0x7104, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4004, 4));
                });
        }

        [Fact]
        public void SUB_Innn_nnnI_Irrr_rI_n_rrr()   // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (0x4000+4),(DEF+K),3,GHI",
                c =>
                {
                    c.LoadMemAt(0x4004, initial);
                    c.CPU.REGS.DEF = 0x7100;
                    c.CPU.REGS.K = 2;
                    c.LoadMemAt(0x7102, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4004, 4));
                });
        }

        [Fact]
        public void SUB_Innn_nnnI_Irrr_rrI_n_rrr()  // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (0x4000+4),(DEF+KL),3,GHI",
                c =>
                {
                    c.LoadMemAt(0x4004, initial);
                    c.CPU.REGS.DEF = 0x7100;
                    c.CPU.REGS.KL = 6;
                    c.LoadMemAt(0x7106, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4004, 4));
                });
        }

        [Fact]
        public void SUB_Innn_nnnI_Irrr_rrrI_n_rrr() // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (0x4000+4),(DEF+KLM),3,GHI",
                c =>
                {
                    c.LoadMemAt(0x4004, initial);
                    c.CPU.REGS.DEF = 0x7100;
                    c.CPU.REGS.KLM = 9;
                    c.LoadMemAt(0x7109, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4004, 4));
                });
        }

        [Fact]
        public void SUB_Innn_nnnI_fr()  // Ok
        {
            RunTest(
                "SUB (0x4000+4),F4",
                c =>
                {
                    c.MEMC.SetFloatToRam(0x4004, 1.25f);
                    c.CPU.FREGS.SetRegister(4, 2.5f);
                },
                c => Assert.Equal(-1.25f, c.MEMC.GetFloatFromRAM(0x4004)));
        }

        [Fact]
        public void SUB_Innn_rI_nnnn_n()    // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            RunTest(
                "SUB (0x4000+X),0x00010203,3",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.LoadMemAt(0x4003, initial);
                },
                c =>
                {
                    byte[] expected = [0x09, 0x12, 0x1B, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4003, 4));
                });
        }

        [Fact]
        public void SUB_Innn_rI_nnnn_n_nnn()    // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            RunTest(
                "SUB (0x4000+X),0x00010203,3,2",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.LoadMemAt(0x4003, initial);
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4003, 4));
                });
        }

        [Fact]
        public void SUB_Innn_rI_r() // Ok
        {
            RunTest(
                "SUB (0x4000+X),B",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.LoadMemAt(0x4003, [0x01, 0x02]);
                    c.CPU.REGS.B = 5;
                },
                c => Assert.Equal([0xFC, 0x02], c.MEMC.RAM.GetMemoryAt(0x4003, 2)));
        }

        [Fact]
        public void SUB_Innn_rI_rr()    // Ok
        {
            RunTest(
                "SUB (0x4000+X),CD",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.LoadMemAt(0x4003, new byte[] { 1, 1, 1 });
                    c.CPU.REGS.CD = 0x0102;
                },
                c => Assert.Equal([0xFF, 0xFF, 0x01], c.MEMC.RAM.GetMemoryAt(0x4003, 3)));
        }

        [Fact]
        public void SUB_Innn_rI_rrr()   // Ok
        {
            RunTest(
                "SUB (0x4000+X),DEF",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.LoadMemAt(0x4003, [0x04, 0x03, 0x02, 0x01]);
                    c.CPU.REGS.DEF = 0x010203;
                },
                c => Assert.Equal([0x03, 0x00, 0xFF, 0x01], c.MEMC.RAM.GetMemoryAt(0x4003, 4)));
        }

        [Fact]
        public void SUB_Innn_rI_rrrr()  // Ok
        {
            RunTest(
                "SUB (0x4000+X),EFGH",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.LoadMemAt(0x4003, [1, 1, 1, 1, 1]);
                    c.CPU.REGS.EFGH = 0x05040302;
                },
                c => Assert.Equal([0xFB, 0xFC, 0xFD, 0xFF, 0x01], c.MEMC.RAM.GetMemoryAt(0x4003, 5)));
        }

        [Fact]
        public void SUB_Innn_rI_InnnI_n_rrr()   // Ok
        {
            byte[] initial = [10, 20, 30, 40];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (0x4000+X),(0x7000),3,GHI",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.LoadMemAt(0x4003, initial);
                    c.LoadMemAt(0x7000, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [8, 16, 24, 40];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4003, 4));
                });
        }

        [Fact]
        public void SUB_Innn_rI_Innn_nnnI_n_rrr()   // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (0x4000+X),(0x7000+4),3,GHI",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.LoadMemAt(0x4003, initial);
                    c.LoadMemAt(0x7004, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4003, 4));
                });
        }

        [Fact]
        public void SUB_Innn_rI_Innn_rI_n_rrr() // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (0x4000+X),(0x7000+K),3,GHI",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.LoadMemAt(0x4003, initial);
                    c.CPU.REGS.K = 2;
                    c.LoadMemAt(0x7002, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4003, 4));
                });
        }

        [Fact]
        public void SUB_Innn_rI_Innn_rrI_n_rrr()    // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (0x4000+X),(0x7000+KL),3,GHI",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.LoadMemAt(0x4003, initial);
                    c.CPU.REGS.KL = 6;
                    c.LoadMemAt(0x7006, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4003, 4));
                });
        }

        [Fact]
        public void SUB_Innn_rI_Innn_rrrI_n_rrr()   // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (0x4000+X),(0x7000+KLM),3,GHI",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.LoadMemAt(0x4003, initial);
                    c.CPU.REGS.KLM = 9;
                    c.LoadMemAt(0x7009, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4003, 4));
                });
        }

        [Fact]
        public void SUB_Innn_rI_IrrrI_n_rrr()   // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (0x4000+X),(DEF),3,GHI",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.LoadMemAt(0x4003, initial);
                    c.CPU.REGS.DEF = 0x7100;
                    c.LoadMemAt(0x7100, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4003, 4));
                });
        }

        [Fact]
        public void SUB_Innn_rI_Irrr_nnnI_n_rrr()   // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (0x4000+X),(DEF+4),3,GHI",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.LoadMemAt(0x4003, initial);
                    c.CPU.REGS.DEF = 0x7100;
                    c.LoadMemAt(0x7104, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4003, 4));
                });
        }

        [Fact]
        public void SUB_Innn_rI_Irrr_rI_n_rrr() // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (0x4000+X),(DEF+K),3,GHI",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.LoadMemAt(0x4003, initial);
                    c.CPU.REGS.DEF = 0x7100;
                    c.CPU.REGS.K = 2;
                    c.LoadMemAt(0x7102, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4003, 4));
                });
        }

        [Fact]
        public void SUB_Innn_rI_Irrr_rrI_n_rrr()    // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (0x4000+X),(DEF+KL),3,GHI",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.LoadMemAt(0x4003, initial);
                    c.CPU.REGS.DEF = 0x7100;
                    c.CPU.REGS.KL = 6;
                    c.LoadMemAt(0x7106, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4003, 4));
                });
        }

        [Fact]
        public void SUB_Innn_rI_Irrr_rrrI_n_rrr()   // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (0x4000+X),(DEF+KLM),3,GHI",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.LoadMemAt(0x4003, initial);
                    c.CPU.REGS.DEF = 0x7100;
                    c.CPU.REGS.KLM = 9;
                    c.LoadMemAt(0x7109, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4003, 4));
                });
        }

        [Fact]
        public void SUB_Innn_rI_fr()    // Ok
        {
            RunTest(
                "SUB (0x4000+X),F4",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.MEMC.SetFloatToRam(0x4003, 1.25f);
                    c.CPU.FREGS.SetRegister(4, 2.5f);
                },
                c => Assert.Equal(-1.25f, c.MEMC.GetFloatFromRAM(0x4003)));
        }


        [Fact]
        public void SUB_Innn_rrI_nnnn_n()   // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            RunTest(
                "SUB (0x4000+WX),0x00010203,3",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    c.LoadMemAt(0x4005, initial);
                },
                c =>
                {
                    byte[] expected = [0x09, 0x12, 0x1B, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4005, 4));
                });
        }

        [Fact]
        public void SUB_Innn_rrI_nnnn_n_nnn()   // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            RunTest(
                "SUB (0x4000+WX),0x00010203,3,2",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    c.LoadMemAt(0x4005, initial);
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4005, 4));
                });
        }

        [Fact]
        public void SUB_Innn_rrI_r()    // Ok
        {
            RunTest(
                "SUB (0x4000+WX),B",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    c.LoadMemAt(0x4005, [10, 20]);
                    c.CPU.REGS.B = 5;
                },
                c => Assert.Equal((byte)5, c.MEMC.Get8bitFromRAM(0x4005)));
        }

        [Fact]
        public void SUB_Innn_rrI_rr()   // Ok
        {
            RunTest(
                "SUB (0x4000+WX),CD",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    c.LoadMemAt(0x4005, [1, 1]);
                    c.CPU.REGS.CD = 0x0102;
                },
                c => Assert.Equal((ushort)0xFFFF, c.MEMC.Get16bitFromRAM(0x4005)));
        }

        [Fact]
        public void SUB_Innn_rrI_rrr()  // Ok
        {
            RunTest(
                "SUB (0x4000+WX),DEF",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    c.LoadMemAt(0x4005, [10, 20, 30]);
                    c.CPU.REGS.DEF = 0x000203;
                },
                c => Assert.Equal((uint)0x0A121B, c.MEMC.Get24bitFromRAM(0x4005)));
        }

        [Fact]
        public void SUB_Innn_rrI_rrrr() // Ok
        {
            RunTest(
                "SUB (0x4000+WX),EFGH",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    c.LoadMemAt(0x4005, [1, 1, 1, 1]);
                    c.CPU.REGS.EFGH = 0x00030405;
                },
                c => Assert.Equal((uint)0x00FDFCFC, c.MEMC.Get32bitFromRAM(0x4005)));
        }

        [Fact]
        public void SUB_Innn_rrI_InnnI_n_rrr()  // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (0x4000+WX),(0x7000),3,GHI",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    c.LoadMemAt(0x4005, initial);
                    c.LoadMemAt(0x7000, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4005, 4));
                });
        }

        [Fact]
        public void SUB_Innn_rrI_Innn_nnnI_n_rrr()  // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (0x4000+WX),(0x7000+4),3,GHI",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    c.LoadMemAt(0x4005, initial);
                    c.LoadMemAt(0x7004, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4005, 4));
                });
        }

        [Fact]
        public void SUB_Innn_rrI_Innn_rI_n_rrr()    // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (0x4000+WX),(0x7000+K),3,GHI",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    c.LoadMemAt(0x4005, initial);
                    c.CPU.REGS.K = 2;
                    c.LoadMemAt(0x7002, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4005, 4));
                });
        }

        [Fact]
        public void SUB_Innn_rrI_Innn_rrI_n_rrr()   // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (0x4000+WX),(0x7000+KL),3,GHI",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    c.LoadMemAt(0x4005, initial);
                    c.CPU.REGS.KL = 6;
                    c.LoadMemAt(0x7006, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4005, 4));
                });
        }

        [Fact]
        public void SUB_Innn_rrI_Innn_rrrI_n_rrr()  // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (0x4000+WX),(0x7000+KLM),3,GHI",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    c.LoadMemAt(0x4005, initial);
                    c.CPU.REGS.KLM = 9;
                    c.LoadMemAt(0x7009, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4005, 4));
                });
        }

        [Fact]
        public void SUB_Innn_rrI_IrrrI_n_rrr()  // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (0x4000+WX),(DEF),3,GHI",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    c.LoadMemAt(0x4005, initial);
                    c.CPU.REGS.DEF = 0x7100;
                    c.LoadMemAt(0x7100, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4005, 4));
                });
        }

        [Fact]
        public void SUB_Innn_rrI_Irrr_nnnI_n_rrr()  // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (0x4000+WX),(DEF+4),3,GHI",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    c.LoadMemAt(0x4005, initial);
                    c.CPU.REGS.DEF = 0x7100;
                    c.LoadMemAt(0x7104, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4005, 4));
                });
        }

        [Fact]
        public void SUB_Innn_rrI_Irrr_rI_n_rrr()    // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (0x4000+WX),(DEF+K),3,GHI",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    c.LoadMemAt(0x4005, initial);
                    c.CPU.REGS.DEF = 0x7100;
                    c.CPU.REGS.K = 2;
                    c.LoadMemAt(0x7102, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4005, 4));
                });
        }

        [Fact]
        public void SUB_Innn_rrI_Irrr_rrI_n_rrr()   // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (0x4000+WX),(DEF+KL),3,GHI",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    c.LoadMemAt(0x4005, initial);
                    c.CPU.REGS.DEF = 0x7100;
                    c.CPU.REGS.KL = 6;
                    c.LoadMemAt(0x7106, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4005, 4));
                });
        }

        [Fact]
        public void SUB_Innn_rrI_Irrr_rrrI_n_rrr()  // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (0x4000+WX),(DEF+KLM),3,GHI",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    c.LoadMemAt(0x4005, initial);
                    c.CPU.REGS.DEF = 0x7100;
                    c.CPU.REGS.KLM = 9;
                    c.LoadMemAt(0x7109, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4005, 4));
                });
        }

        [Fact]
        public void SUB_Innn_rrI_fr()   // Ok
        {
            RunTest(
                "SUB (0x4000+WX),F4",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    c.MEMC.SetFloatToRam(0x4005, 1.25f);
                    c.CPU.FREGS.SetRegister(4, 2.5f);
                },
                c => Assert.Equal(-1.25f, c.MEMC.GetFloatFromRAM(0x4005)));
        }


        [Fact]
        public void SUB_Innn_rrrI_nnnn_n()  // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            RunTest(
                "SUB (0x4000+VWX),0x00010203,3",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    c.LoadMemAt(0x4007, initial);
                },
                c =>
                {
                    byte[] expected = [0x09, 0x12, 0x1B, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4007, 4));
                });
        }

        [Fact]
        public void SUB_Innn_rrrI_nnnn_n_nnn()  // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            RunTest(
                "SUB (0x4000+VWX),0x00010203,3,2",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    c.LoadMemAt(0x4007, initial);
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4007, 4));
                });
        }

        [Fact]
        public void SUB_Innn_rrrI_r()   // Ok
        {
            RunTest(
                "SUB (0x4000+VWX),B",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    c.LoadMemAt(0x4007, [10, 20]);
                    c.CPU.REGS.B = 5;
                },
                c => Assert.Equal((byte)5, c.MEMC.Get8bitFromRAM(0x4007)));
        }

        [Fact]
        public void SUB_Innn_rrrI_rr()  // Ok
        {
            RunTest(
                "SUB (0x4000+VWX),CD",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    c.LoadMemAt(0x4007, [1, 1]);
                    c.CPU.REGS.CD = 0x0102;
                },
                c => Assert.Equal((ushort)0xFFFF, c.MEMC.Get16bitFromRAM(0x4007)));
        }

        [Fact]
        public void SUB_Innn_rrrI_rrr() // Ok
        {
            RunTest(
                "SUB (0x4000+VWX),DEF",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    c.LoadMemAt(0x4007, [10, 20, 30]);
                    c.CPU.REGS.DEF = 0x000203;
                },
                c => Assert.Equal((uint)0x0A121B, c.MEMC.Get24bitFromRAM(0x4007)));
        }

        [Fact]
        public void SUB_Innn_rrrI_rrrr()    // Ok
        {
            RunTest(
                "SUB (0x4000+VWX),EFGH",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    c.LoadMemAt(0x4007, [1, 1, 1, 1]);
                    c.CPU.REGS.EFGH = 0x00030405;
                },
                c => Assert.Equal((uint)0x00FDFCFC, c.MEMC.Get32bitFromRAM(0x4007)));
        }

        [Fact]
        public void SUB_Innn_rrrI_InnnI_n_rrr() // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (0x4000+VWX),(0x7000),3,GHI",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    c.LoadMemAt(0x4007, initial);
                    c.LoadMemAt(0x7000, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4007, 4));
                });
        }

        [Fact]
        public void SUB_Innn_rrrI_Innn_nnnI_n_rrr() // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (0x4000+VWX),(0x7000+4),3,GHI",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    c.LoadMemAt(0x4007, initial);
                    c.LoadMemAt(0x7004, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4007, 4));
                });
        }

        [Fact]
        public void SUB_Innn_rrrI_Innn_rI_n_rrr()   // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (0x4000+VWX),(0x7000+K),3,GHI",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    c.LoadMemAt(0x4007, initial);
                    c.CPU.REGS.K = 2;
                    c.LoadMemAt(0x7002, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4007, 4));
                });
        }

        [Fact]
        public void SUB_Innn_rrrI_Innn_rrI_n_rrr()  // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (0x4000+VWX),(0x7000+KL),3,GHI",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    c.LoadMemAt(0x4007, initial);
                    c.CPU.REGS.KL = 6;
                    c.LoadMemAt(0x7006, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4007, 4));
                });
        }

        [Fact]
        public void SUB_Innn_rrrI_Innn_rrrI_n_rrr() // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (0x4000+VWX),(0x7000+KLM),3,GHI",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    c.LoadMemAt(0x4007, initial);
                    c.CPU.REGS.KLM = 9;
                    c.LoadMemAt(0x7009, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4007, 4));
                });
        }

        [Fact]
        public void SUB_Innn_rrrI_IrrrI_n_rrr() // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (0x4000+VWX),(DEF),3,GHI",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    c.LoadMemAt(0x4007, initial);
                    c.CPU.REGS.DEF = 0x7100;
                    c.LoadMemAt(0x7100, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4007, 4));
                });
        }

        [Fact]
        public void SUB_Innn_rrrI_Irrr_nnnI_n_rrr() // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (0x4000+VWX),(DEF+4),3,GHI",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    c.LoadMemAt(0x4007, initial);
                    c.CPU.REGS.DEF = 0x7100;
                    c.LoadMemAt(0x7104, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4007, 4));
                });
        }

        [Fact]
        public void SUB_Innn_rrrI_Irrr_rI_n_rrr()   // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (0x4000+VWX),(DEF+K),3,GHI",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    c.LoadMemAt(0x4007, initial);
                    c.CPU.REGS.DEF = 0x7100;
                    c.CPU.REGS.K = 2;
                    c.LoadMemAt(0x7102, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4007, 4));
                });
        }

        [Fact]
        public void SUB_Innn_rrrI_Irrr_rrI_n_rrr()  // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (0x4000+VWX),(DEF+KL),3,GHI",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    c.LoadMemAt(0x4007, initial);
                    c.CPU.REGS.DEF = 0x7100;
                    c.CPU.REGS.KL = 6;
                    c.LoadMemAt(0x7106, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4007, 4));
                });
        }

        [Fact]
        public void SUB_Innn_rrrI_Irrr_rrrI_n_rrr() // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (0x4000+VWX),(DEF+KLM),3,GHI",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    c.LoadMemAt(0x4007, initial);
                    c.CPU.REGS.DEF = 0x7100;
                    c.CPU.REGS.KLM = 9;
                    c.LoadMemAt(0x7109, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4007, 4));
                });
        }

        [Fact]
        public void SUB_Innn_rrrI_fr()  // Ok
        {
            RunTest(
                "SUB (0x4000+VWX),F4",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.SetFloatToRam(0x4007, 1.25f);
                    c.CPU.FREGS.SetRegister(4, 2.5f);
                },
                c => Assert.Equal(-1.25f, c.MEMC.GetFloatFromRAM(0x4007)));
        }


        [Fact]
        public void SUB_IrrrI_nnnn_n()  // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            RunTest(
                "SUB (QRS),0x00010203,3",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5000, initial);
                },
                c =>
                {
                    byte[] expected = [0x09, 0x12, 0x1B, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5000, 4));
                });
        }

        [Fact]
        public void SUB_IrrrI_nnnn_n_nnn()  // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            RunTest(
                "SUB (QRS),0x00010203,3,2",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5000, initial);
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5000, 4));
                });
        }

        [Fact]
        public void SUB_IrrrI_r()   // Ok
        {
            RunTest(
                "SUB (QRS),B",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5000, [1, 2]);
                    c.CPU.REGS.B = 5;
                },
                c => Assert.Equal([0xFC, 0x02], c.MEMC.RAM.GetMemoryAt(0x5000, 2)));
        }

        [Fact]
        public void SUB_IrrrI_rr()  // Ok
        {
            RunTest(
                "SUB (QRS),CD",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5000, [0x01, 0x02]);
                    c.CPU.REGS.CD = 0x0102;
                },
                c => Assert.Equal([0x00, 0x00], c.MEMC.RAM.GetMemoryAt(0x5000, 2)));
        }

        [Fact]
        public void SUB_IrrrI_rrr() // Ok
        {
            RunTest(
                "SUB (QRS),DEF",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5000, [10, 20, 30]);
                    c.CPU.REGS.DEF = 0x010203;
                },
                c => Assert.Equal([9, 18, 27], c.MEMC.RAM.GetMemoryAt(0x5000, 3)));
        }

        [Fact]
        public void SUB_IrrrI_rrrr()    // Ok
        {
            RunTest(
                "SUB (QRS),EFGH",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5000, [4, 3, 2, 1]);
                    c.CPU.REGS.EFGH = 0x02030405;
                },
                c => Assert.Equal([0x01, 0xFF, 0xFD, 0xFC], c.MEMC.RAM.GetMemoryAt(0x5000, 4)));
        }

        [Fact]
        public void SUB_IrrrI_InnnI_n_rrr() // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (QRS),(0x7000),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5000, initial);
                    c.LoadMemAt(0x7000, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5000, 4));
                });
        }

        [Fact]
        public void SUB_IrrrI_Innn_nnnI_n_rrr() // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (QRS),(0x7000+4),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5000, initial);
                    c.LoadMemAt(0x7004, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5000, 4));
                });
        }

        [Fact]
        public void SUB_IrrrI_Innn_rI_n_rrr()   // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (QRS),(0x7000+K),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5000, initial);
                    c.CPU.REGS.K = 2;
                    c.LoadMemAt(0x7002, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5000, 4));
                });
        }

        [Fact]
        public void SUB_IrrrI_Innn_rrI_n_rrr()  // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (QRS),(0x7000+KL),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5000, initial);
                    c.CPU.REGS.KL = 6;
                    c.LoadMemAt(0x7006, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5000, 4));
                });
        }

        [Fact]
        public void SUB_IrrrI_Innn_rrrI_n_rrr() // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (QRS),(0x7000+KLM),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5000, initial);
                    c.CPU.REGS.KLM = 9;
                    c.LoadMemAt(0x7009, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5000, 4));
                });
        }

        [Fact]
        public void SUB_IrrrI_IrrrI_n_rrr() // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (QRS),(DEF),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5000, initial);
                    c.CPU.REGS.DEF = 0x7100;
                    c.LoadMemAt(0x7100, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5000, 4));
                });
        }

        [Fact]
        public void SUB_IrrrI_Irrr_nnnI_n_rrr() // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (QRS),(DEF+4),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5000, initial);
                    c.CPU.REGS.DEF = 0x7100;
                    c.LoadMemAt(0x7104, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5000, 4));
                });
        }

        [Fact]
        public void SUB_IrrrI_Irrr_rI_n_rrr()   // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (QRS),(DEF+K),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5000, initial);
                    c.CPU.REGS.DEF = 0x7100;
                    c.CPU.REGS.K = 2;
                    c.LoadMemAt(0x7102, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5000, 4));
                });
        }

        [Fact]
        public void SUB_IrrrI_Irrr_rrI_n_rrr()  // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (QRS),(DEF+KL),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5000, initial);
                    c.CPU.REGS.DEF = 0x7100;
                    c.CPU.REGS.KL = 6;
                    c.LoadMemAt(0x7106, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5000, 4));
                });
        }

        [Fact]
        public void SUB_IrrrI_Irrr_rrrI_n_rrr() // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (QRS),(DEF+KLM),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5000, initial);
                    c.CPU.REGS.DEF = 0x7100;
                    c.CPU.REGS.KLM = 9;
                    c.LoadMemAt(0x7109, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5000, 4));
                });
        }

        [Fact]
        public void SUB_IrrrI_fr()  // Ok
        {
            RunTest(
                "SUB (QRS),F4",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.SetFloatToRam(0x5000, 1.25f);
                    c.CPU.FREGS.SetRegister(4, 2.5f);
                },
                c => Assert.Equal(-1.25f, c.MEMC.GetFloatFromRAM(0x5000)));
        }


        [Fact]
        public void SUB_Irrr_nnnI_nnnn_n()  // Ok
        {
            byte[] initial = [0x01, 0x02, 0x03, 0x04];
            RunTest(
                "SUB (QRS+4),0x00010203,3",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5004, initial);
                },
                c =>
                {
                    byte[] expected = [0x00, 0x00, 0x00, 0x04];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5004, 4));
                });
        }

        [Fact]
        public void SUB_Irrr_nnnI_nnnn_n_nnn()  // Ok
        {
            byte[] initial = [0x01, 0x02, 0x03, 0x04];
            RunTest(
                "SUB (QRS+4),0x00010203,3,2",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5004, initial);
                },
                c =>
                {
                    byte[] expected = [0xFE, 0xFD, 0xFD, 0x04];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5004, 4));
                });
        }

        [Fact]
        public void SUB_Irrr_nnnI_r()   // Ok
        {
            RunTest(
                "SUB (QRS+4),B",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5004, [0x01, 0x02]);
                    c.CPU.REGS.B = 5;
                },
                c => Assert.Equal([0xFC, 0x02], c.MEMC.RAM.GetMemoryAt(0x5004, 2)));
        }

        [Fact]
        public void SUB_Irrr_nnnI_rr()  // Ok
        {
            RunTest(
                "SUB (QRS+4),CD",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5004, [0x01, 0x01]);
                    c.CPU.REGS.CD = 0x0102;
                },
                c => Assert.Equal([0xFF, 0xFF], c.MEMC.RAM.GetMemoryAt(0x5004, 2)));
        }

        [Fact]
        public void SUB_Irrr_nnnI_rrr() // Ok
        {
            RunTest(
                "SUB (QRS+4),DEF",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5004, [0x01, 0x02, 0x03]);
                    c.CPU.REGS.DEF = 0x010203;
                },
                c => Assert.Equal([0x00, 0x00, 0x00], c.MEMC.RAM.GetMemoryAt(0x5004, 3)));
        }

        [Fact]
        public void SUB_Irrr_nnnI_rrrr()    // Ok
        {
            RunTest(
                "SUB (QRS+4),EFGH",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5004, [0x01, 0x01, 0x01, 0x01]);
                    c.CPU.REGS.EFGH = 0x01020304;
                },
                c => Assert.Equal([0xFF, 0xFE, 0xFD, 0xFD], c.MEMC.RAM.GetMemoryAt(0x5004, 4)));
        }

        [Fact]
        public void SUB_Irrr_nnnI_InnnI_n_rrr() // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (QRS+4),(0x7000),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5004, initial);
                    c.LoadMemAt(0x7000, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5004, 4));
                });
        }

        [Fact]
        public void SUB_Irrr_nnnI_Innn_nnnI_n_rrr() // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (QRS+4),(0x7000+4),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5004, initial);
                    c.LoadMemAt(0x7004, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5004, 4));
                });
        }

        [Fact]
        public void SUB_Irrr_nnnI_Innn_rI_n_rrr()   // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (QRS+4),(0x7000+K),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5004, initial);
                    c.CPU.REGS.K = 2;
                    c.LoadMemAt(0x7002, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5004, 4));
                });
        }

        [Fact]
        public void SUB_Irrr_nnnI_Innn_rrI_n_rrr()  // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (QRS+4),(0x7000+KL),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5004, initial);
                    c.CPU.REGS.KL = 6;
                    c.LoadMemAt(0x7006, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5004, 4));
                });
        }

        [Fact]
        public void SUB_Irrr_nnnI_Innn_rrrI_n_rrr() // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (QRS+4),(0x7000+KLM),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5004, initial);
                    c.CPU.REGS.KLM = 9;
                    c.LoadMemAt(0x7009, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5004, 4));
                });
        }

        [Fact]
        public void SUB_Irrr_nnnI_IrrrI_n_rrr() // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (QRS+4),(MNO),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5004, initial);
                    c.CPU.REGS.MNO = 0x7100;
                    c.LoadMemAt(0x7100, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5004, 4));
                });
        }

        [Fact]
        public void SUB_Irrr_nnnI_Irrr_nnnI_n_rrr() // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (QRS+4),(MNO+4),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5004, initial);
                    c.CPU.REGS.MNO = 0x7100;
                    c.LoadMemAt(0x7104, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5004, 4));
                });
        }

        [Fact]
        public void SUB_Irrr_nnnI_Irrr_rI_n_rrr()   // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (QRS+4),(MNO+K),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5004, initial);
                    c.CPU.REGS.MNO = 0x7100;
                    c.CPU.REGS.K = 2;
                    c.LoadMemAt(0x7102, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5004, 4));
                });
        }

        [Fact]
        public void SUB_Irrr_nnnI_Irrr_rrI_n_rrr()  // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (QRS+4),(MNO+KL),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5004, initial);
                    c.CPU.REGS.MNO = 0x7100;
                    c.CPU.REGS.KL = 6;
                    c.LoadMemAt(0x7106, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5004, 4));
                });
        }

        [Fact]
        public void SUB_Irrr_nnnI_Irrr_rrrI_n_rrr() // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (QRS+4),(MNO+ABC),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5004, initial);
                    c.CPU.REGS.MNO = 0x7100;
                    c.CPU.REGS.ABC = 9;
                    c.LoadMemAt(0x7109, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5004, 4));
                });
        }

        [Fact]
        public void SUB_Irrr_nnnI_fr()  // Ok
        {
            RunTest(
                "SUB (QRS+4),F4",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.SetFloatToRam(0x5004, 1.25f);
                    c.CPU.FREGS.SetRegister(4, 2.5f);
                },
                c => Assert.Equal(-1.25f, c.MEMC.GetFloatFromRAM(0x5004)));
        }


        [Fact]
        public void SUB_Irrr_rI_nnnn_n()    // Ok
        {
            byte[] initial = [0x01, 0x02, 0x03, 0x04];
            RunTest(
                "SUB (QRS+X),0x00010203,3",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 5;
                    c.LoadMemAt(0x5005, initial);
                },
                c =>
                {
                    byte[] expected = [0x00, 0x00, 0x00, 0x04];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5005, 4));
                });
        }

        [Fact]
        public void SUB_Irrr_rI_nnnn_n_nnn()    // Ok
        {
            byte[] initial = [0x01, 0x02, 0x03, 0x04];
            RunTest(
                "SUB (QRS+X),0x00010203,3,2",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 5;
                    c.LoadMemAt(0x5005, initial);
                },
                c =>
                {
                    byte[] expected = [0xFE, 0xFD, 0xFD, 0x04];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5005, 4));
                });
        }

        [Fact]
        public void SUB_Irrr_rI_r() // Ok
        {
            RunTest(
                "SUB (QRS+X),B",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 5;
                    c.LoadMemAt(0x5005, [0x01, 0x02]);
                    c.CPU.REGS.B = 5;
                },
                c => Assert.Equal([0xFC, 0x02], c.MEMC.RAM.GetMemoryAt(0x5005, 2)));
        }

        [Fact]
        public void SUB_Irrr_rI_rr()    // Ok
        {
            RunTest(
                "SUB (QRS+X),CD",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 5;
                    c.LoadMemAt(0x5005, [0x01, 0x01]);
                    c.CPU.REGS.CD = 0x0102;
                },
                c => Assert.Equal([0xFF, 0xFF], c.MEMC.RAM.GetMemoryAt(0x5005, 2)));
        }

        [Fact]
        public void SUB_Irrr_rI_rrr()   // Ok
        {
            RunTest(
                "SUB (QRS+X),DEF",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 5;
                    c.LoadMemAt(0x5005, [0x01, 0x02, 0x03]);
                    c.CPU.REGS.DEF = 0x010203;
                },
                c => Assert.Equal([0x00, 0x00, 0x00], c.MEMC.RAM.GetMemoryAt(0x5005, 3)));
        }

        [Fact]
        public void SUB_Irrr_rI_rrrr()  // Ok
        {
            RunTest(
                "SUB (QRS+X),EFGH",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 5;
                    c.LoadMemAt(0x5005, [0x01, 0x01, 0x01, 0x01]);
                    c.CPU.REGS.EFGH = 0x01020304;
                },
                c => Assert.Equal([0xFF, 0xFE, 0xFD, 0xFD], c.MEMC.RAM.GetMemoryAt(0x5005, 4)));
        }

        [Fact]
        public void SUB_Irrr_rI_InnnI_n_rrr()   // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (QRS+X),(0x7000),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 5;
                    c.LoadMemAt(0x5005, initial);
                    c.LoadMemAt(0x7000, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5005, 4));
                });
        }

        [Fact]
        public void SUB_Irrr_rI_Innn_nnnI_n_rrr()   // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (QRS+X),(0x7000+4),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 5;
                    c.LoadMemAt(0x5005, initial);
                    c.LoadMemAt(0x7004, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5005, 4));
                });
        }

        [Fact]
        public void SUB_Irrr_rI_Innn_rI_n_rrr() // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (QRS+X),(0x7000+K),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 5;
                    c.LoadMemAt(0x5005, initial);
                    c.CPU.REGS.K = 2;
                    c.LoadMemAt(0x7002, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5005, 4));
                });
        }

        [Fact]
        public void SUB_Irrr_rI_Innn_rrI_n_rrr()    // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (QRS+X),(0x7000+KL),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 5;
                    c.LoadMemAt(0x5005, initial);
                    c.CPU.REGS.KL = 6;
                    c.LoadMemAt(0x7006, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5005, 4));
                });
        }

        [Fact]
        public void SUB_Irrr_rI_Innn_rrrI_n_rrr()   // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (QRS+X),(0x7000+KLM),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 5;
                    c.LoadMemAt(0x5005, initial);
                    c.CPU.REGS.KLM = 9;
                    c.LoadMemAt(0x7009, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5005, 4));
                });
        }

        [Fact]
        public void SUB_Irrr_rI_IrrrI_n_rrr()   // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (QRS+X),(MNO),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 5;
                    c.LoadMemAt(0x5005, initial);
                    c.CPU.REGS.MNO = 0x7100;
                    c.LoadMemAt(0x7100, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5005, 4));
                });
        }

        [Fact]
        public void SUB_Irrr_rI_Irrr_nnnI_n_rrr()   // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (QRS+X),(MNO+4),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 5;
                    c.LoadMemAt(0x5005, initial);
                    c.CPU.REGS.MNO = 0x7100;
                    c.LoadMemAt(0x7104, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5005, 4));
                });
        }

        [Fact]
        public void SUB_Irrr_rI_Irrr_rI_n_rrr() // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (QRS+X),(MNO+K),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 5;
                    c.LoadMemAt(0x5005, initial);
                    c.CPU.REGS.MNO = 0x7100;
                    c.CPU.REGS.K = 2;
                    c.LoadMemAt(0x7102, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5005, 4));
                });
        }

        [Fact]
        public void SUB_Irrr_rI_Irrr_rrI_n_rrr()    // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (QRS+X),(MNO+KL),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 5;
                    c.LoadMemAt(0x5005, initial);
                    c.CPU.REGS.MNO = 0x7100;
                    c.CPU.REGS.KL = 6;
                    c.LoadMemAt(0x7106, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5005, 4));
                });
        }

        [Fact]
        public void SUB_Irrr_rI_Irrr_rrrI_n_rrr()   // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (QRS+X),(MNO+ABC),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 5;
                    c.LoadMemAt(0x5005, initial);
                    c.CPU.REGS.MNO = 0x7100;
                    c.CPU.REGS.ABC = 9;
                    c.LoadMemAt(0x7109, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5005, 4));
                });
        }

        [Fact]
        public void SUB_Irrr_rI_fr()    // Ok
        {
            RunTest(
                "SUB (QRS+X),F4",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 5;
                    c.MEMC.SetFloatToRam(0x5005, 1.25f);
                    c.CPU.FREGS.SetRegister(4, 2.5f);
                },
                c => Assert.Equal(-1.25f, c.MEMC.GetFloatFromRAM(0x5005)));
        }


        [Fact]
        public void SUB_Irrr_rrI_nnnn_n()   // Ok
        {
            byte[] initial = [0x01, 0x02, 0x03, 0x04];
            RunTest(
                "SUB (QRS+WX),0x00010203,3",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 7;
                    c.LoadMemAt(0x5007, initial);
                },
                c =>
                {
                    byte[] expected = [0x00, 0x00, 0x00, 0x04];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5007, 4));
                });
        }

        [Fact]
        public void SUB_Irrr_rrI_nnnn_n_nnn()   // Ok
        {
            byte[] initial = [0x01, 0x02, 0x03, 0x04];
            RunTest(
                "SUB (QRS+WX),0x00010203,3,2",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 7;
                    c.LoadMemAt(0x5007, initial);
                },
                c =>
                {
                    byte[] expected = [0xFE, 0xFD, 0xFD, 0x04];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5007, 4));
                });
        }

        [Fact]
        public void SUB_Irrr_rrI_r()    // Ok
        {
            RunTest(
                "SUB (QRS+WX),B",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 7;
                    c.LoadMemAt(0x5007, [0x01, 0x02]);
                    c.CPU.REGS.B = 5;
                },
                c => Assert.Equal([0xFC, 0x02], c.MEMC.RAM.GetMemoryAt(0x5007, 2)));
        }

        [Fact]
        public void SUB_Irrr_rrI_rr()   // Ok
        {
            RunTest(
                "SUB (QRS+WX),CD",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 7;
                    c.LoadMemAt(0x5007, [0x01, 0x01]);
                    c.CPU.REGS.CD = 0x0102;
                },
                c => Assert.Equal([0xFF, 0xFF], c.MEMC.RAM.GetMemoryAt(0x5007, 2)));
        }

        [Fact]
        public void SUB_Irrr_rrI_rrr()  // Ok
        {
            RunTest(
                "SUB (QRS+WX),DEF",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 7;
                    c.LoadMemAt(0x5007, [0x01, 0x02, 0x03]);
                    c.CPU.REGS.DEF = 0x010203;
                },
                c => Assert.Equal([0x00, 0x00, 0x00], c.MEMC.RAM.GetMemoryAt(0x5007, 3)));
        }

        [Fact]
        public void SUB_Irrr_rrI_rrrr() // Ok
        {
            RunTest(
                "SUB (QRS+WX),EFGH",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 7;
                    c.LoadMemAt(0x5007, [0x01, 0x01, 0x01, 0x01]);
                    c.CPU.REGS.EFGH = 0x01020304;
                },
                c => Assert.Equal([0xFF, 0xFE, 0xFD, 0xFD], c.MEMC.RAM.GetMemoryAt(0x5007, 4)));
        }

        [Fact]
        public void SUB_Irrr_rrI_InnnI_n_rrr()  // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (QRS+WX),(0x7000),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 7;
                    c.LoadMemAt(0x5007, initial);
                    c.LoadMemAt(0x7000, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5007, 4));
                });
        }

        [Fact]
        public void SUB_Irrr_rrI_Innn_nnnI_n_rrr()  // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (QRS+WX),(0x7000+4),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 7;
                    c.LoadMemAt(0x5007, initial);
                    c.LoadMemAt(0x7004, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5007, 4));
                });
        }

        [Fact]
        public void SUB_Irrr_rrI_Innn_rI_n_rrr()    // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (QRS+WX),(0x7000+K),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 7;
                    c.LoadMemAt(0x5007, initial);
                    c.CPU.REGS.K = 2;
                    c.LoadMemAt(0x7002, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5007, 4));
                });
        }

        [Fact]
        public void SUB_Irrr_rrI_Innn_rrI_n_rrr()   // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (QRS+WX),(0x7000+KL),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 7;
                    c.LoadMemAt(0x5007, initial);
                    c.CPU.REGS.KL = 6;
                    c.LoadMemAt(0x7006, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5007, 4));
                });
        }

        [Fact]
        public void SUB_Irrr_rrI_Innn_rrrI_n_rrr()  // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (QRS+WX),(0x7000+KLM),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 7;
                    c.LoadMemAt(0x5007, initial);
                    c.CPU.REGS.KLM = 9;
                    c.LoadMemAt(0x7009, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5007, 4));
                });
        }

        [Fact]
        public void SUB_Irrr_rrI_IrrrI_n_rrr()  // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (QRS+WX),(MNO),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 7;
                    c.LoadMemAt(0x5007, initial);
                    c.CPU.REGS.MNO = 0x7100;
                    c.LoadMemAt(0x7100, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5007, 4));
                });
        }

        [Fact]
        public void SUB_Irrr_rrI_Irrr_nnnI_n_rrr()  // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (QRS+WX),(MNO+4),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 7;
                    c.LoadMemAt(0x5007, initial);
                    c.CPU.REGS.MNO = 0x7100;
                    c.LoadMemAt(0x7104, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5007, 4));
                });
        }

        [Fact]
        public void SUB_Irrr_rrI_Irrr_rI_n_rrr()    // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (QRS+WX),(MNO+K),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 7;
                    c.LoadMemAt(0x5007, initial);
                    c.CPU.REGS.MNO = 0x7100;
                    c.CPU.REGS.K = 2;
                    c.LoadMemAt(0x7102, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5007, 4));
                });
        }

        [Fact]
        public void SUB_Irrr_rrI_Irrr_rrI_n_rrr()   // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (QRS+WX),(MNO+KL),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 7;
                    c.LoadMemAt(0x5007, initial);
                    c.CPU.REGS.MNO = 0x7100;
                    c.CPU.REGS.KL = 6;
                    c.LoadMemAt(0x7106, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5007, 4));
                });
        }

        [Fact]
        public void SUB_Irrr_rrI_Irrr_rrrI_n_rrr()  // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (QRS+WX),(MNO+ABC),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 7;
                    c.LoadMemAt(0x5007, initial);
                    c.CPU.REGS.MNO = 0x7100;
                    c.CPU.REGS.ABC = 9;
                    c.LoadMemAt(0x7109, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5007, 4));
                });
        }

        [Fact]
        public void SUB_Irrr_rrI_fr()   // Ok
        {
            RunTest(
                "SUB (QRS+WX),F4",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 7;
                    c.MEMC.SetFloatToRam(0x5007, 1.25f);
                    c.CPU.FREGS.SetRegister(4, 2.5f);
                },
                c => Assert.Equal(-1.25f, c.MEMC.GetFloatFromRAM(0x5007)));
        }

        [Fact]
        public void SUB_Irrr_rrrI_nnnn_n()  // Ok
        {
            byte[] initial = [0x01, 0x02, 0x03, 0x04];
            RunTest(
                "SUB (QRS+TUV),0x00010203,3",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.TUV = 9;
                    c.LoadMemAt(0x5009, initial);
                },
                c =>
                {
                    byte[] expected = [0x00, 0x00, 0x00, 0x04];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5009, 4));
                });
        }

        [Fact]
        public void SUB_Irrr_rrrI_nnnn_n_nnn()  // Ok
        {
            byte[] initial = [0x01, 0x02, 0x03, 0x04];
            RunTest(
                "SUB (QRS+TUV),0x00010203,3,2",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.TUV = 9;
                    c.LoadMemAt(0x5009, initial);
                },
                c =>
                {
                    byte[] expected = [0xFE, 0xFD, 0xFD, 0x04];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5009, 4));
                });
        }

        [Fact]
        public void SUB_Irrr_rrrI_r()   // Ok
        {
            RunTest(
                "SUB (QRS+TUV),B",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.TUV = 9;
                    c.LoadMemAt(0x5009, [0x01, 0x02]);
                    c.CPU.REGS.B = 5;
                },
                c => Assert.Equal([0xFC, 0x02], c.MEMC.RAM.GetMemoryAt(0x5009, 2)));
        }

        [Fact]
        public void SUB_Irrr_rrrI_rr()  // Ok
        {
            RunTest(
                "SUB (QRS+TUV),CD",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.TUV = 9;
                    c.LoadMemAt(0x5009, [0x01, 0x01]);
                    c.CPU.REGS.CD = 0x0102;
                },
                c => Assert.Equal([0xFF, 0xFF], c.MEMC.RAM.GetMemoryAt(0x5009, 2)));
        }

        [Fact]
        public void SUB_Irrr_rrrI_rrr() // Ok
        {
            RunTest(
                "SUB (QRS+TUV),DEF",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.TUV = 9;
                    c.LoadMemAt(0x5009, [0x01, 0x02, 0x03]);
                    c.CPU.REGS.DEF = 0x010203;
                },
                c => Assert.Equal([0x00, 0x00, 0x00], c.MEMC.RAM.GetMemoryAt(0x5009, 3)));
        }

        [Fact]
        public void SUB_Irrr_rrrI_rrrr()    // Ok
        {
            RunTest(
                "SUB (QRS+TUV),EFGH",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.TUV = 9;
                    c.LoadMemAt(0x5009, [0x01, 0x01, 0x01, 0x01]);
                    c.CPU.REGS.EFGH = 0x01020304;
                },
                c => Assert.Equal([0xFF, 0xFE, 0xFD, 0xFD], c.MEMC.RAM.GetMemoryAt(0x5009, 4)));
        }

        [Fact]
        public void SUB_Irrr_rrrI_InnnI_n_rrr() // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (QRS+TUV),(0x7000),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.TUV = 9;
                    c.LoadMemAt(0x5009, initial);
                    c.LoadMemAt(0x7000, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5009, 4));
                });
        }

        [Fact]
        public void SUB_Irrr_rrrI_Innn_nnnI_n_rrr() // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (QRS+TUV),(0x7000+4),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.TUV = 9;
                    c.LoadMemAt(0x5009, initial);
                    c.LoadMemAt(0x7004, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5009, 4));
                });
        }

        [Fact]
        public void SUB_Irrr_rrrI_Innn_rI_n_rrr()   // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (QRS+TUV),(0x7000+K),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.TUV = 9;
                    c.LoadMemAt(0x5009, initial);
                    c.CPU.REGS.K = 2;
                    c.LoadMemAt(0x7002, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5009, 4));
                });
        }

        [Fact]
        public void SUB_Irrr_rrrI_Innn_rrI_n_rrr()  // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (QRS+TUV),(0x7000+KL),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.TUV = 9;
                    c.LoadMemAt(0x5009, initial);
                    c.CPU.REGS.KL = 6;
                    c.LoadMemAt(0x7006, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5009, 4));
                });
        }

        [Fact]
        public void SUB_Irrr_rrrI_Innn_rrrI_n_rrr() // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (QRS+TUV),(0x7000+KLM),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.TUV = 9;
                    c.LoadMemAt(0x5009, initial);
                    c.CPU.REGS.KLM = 9;
                    c.LoadMemAt(0x7009, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5009, 4));
                });
        }

        [Fact]
        public void SUB_Irrr_rrrI_IrrrI_n_rrr() // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (QRS+TUV),(MNO),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.TUV = 9;
                    c.LoadMemAt(0x5009, initial);
                    c.CPU.REGS.MNO = 0x7100;
                    c.LoadMemAt(0x7100, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5009, 4));
                });
        }

        [Fact]
        public void SUB_Irrr_rrrI_Irrr_nnnI_n_rrr() // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (QRS+TUV),(MNO+4),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.TUV = 9;
                    c.LoadMemAt(0x5009, initial);
                    c.CPU.REGS.MNO = 0x7100;
                    c.LoadMemAt(0x7104, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5009, 4));
                });
        }

        [Fact]
        public void SUB_Irrr_rrrI_Irrr_rI_n_rrr()   // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (QRS+TUV),(MNO+K),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.TUV = 9;
                    c.LoadMemAt(0x5009, initial);
                    c.CPU.REGS.MNO = 0x7100;
                    c.CPU.REGS.K = 2;
                    c.LoadMemAt(0x7102, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5009, 4));
                });
        }

        [Fact]
        public void SUB_Irrr_rrrI_Irrr_rrI_n_rrr()  // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (QRS+TUV),(MNO+KL),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.TUV = 9;
                    c.LoadMemAt(0x5009, initial);
                    c.CPU.REGS.MNO = 0x7100;
                    c.CPU.REGS.KL = 6;
                    c.LoadMemAt(0x7106, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5009, 4));
                });
        }

        [Fact]
        public void SUB_Irrr_rrrI_Irrr_rrrI_n_rrr() // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "SUB (QRS+TUV),(MNO+ABC),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.TUV = 9;
                    c.LoadMemAt(0x5009, initial);
                    c.CPU.REGS.MNO = 0x7100;
                    c.CPU.REGS.ABC = 9;
                    c.LoadMemAt(0x7109, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x08, 0x10, 0x18, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5009, 4));
                });
        }

        [Fact]
        public void SUB_Irrr_rrrI_fr()  // Ok
        {
            RunTest(
                "SUB (QRS+TUV),F4",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.TUV = 9;
                    c.MEMC.SetFloatToRam(0x5009, 1.25f);
                    c.CPU.FREGS.SetRegister(4, 2.5f);
                },
                c => Assert.Equal(-1.25f, c.MEMC.GetFloatFromRAM(0x5009)));
        }

        #endregion

        #region SUB fr,* tests

        [Fact]
        public void SUB_fr_nnnn()   // Ok
        {
            RunTest(
                "SUB F0,6.1",
                c => c.CPU.FREGS.SetRegister(0, 10.5f),
                c => Assert.Equal(4.4f, c.CPU.FREGS.GetRegister(0), precision: 5));
        }

        [Fact]
        public void SUB_fr_r()  // Ok
        {
            RunTest(
                "SUB F0,J",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 10.5f);
                    c.CPU.REGS.J = 3;
                },
                c => Assert.Equal(7.5f, c.CPU.FREGS.GetRegister(0), precision: 5));
        }

        [Fact]
        public void SUB_fr_rr() // Ok
        {
            RunTest(
                "SUB F0,JK",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 10.5f);
                    c.CPU.REGS.JK = 1000;
                },
                c => Assert.Equal(-989.5f, c.CPU.FREGS.GetRegister(0), precision: 5));
        }

        [Fact]
        public void SUB_fr_rrr()    // Ok
        {
            RunTest(
                "SUB F0,JKL",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 10.5f);
                    c.CPU.REGS.JKL = 50000;
                },
                c => Assert.Equal(-49989.5f, c.CPU.FREGS.GetRegister(0), precision: 5));
        }

        [Fact]
        public void SUB_fr_rrrr()   // Ok
        {
            RunTest(
                "SUB F0,JKLM",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 10.5f);
                    c.CPU.REGS.JKLM = 1000000;
                },
                c => Assert.Equal(-999989.5f, c.CPU.FREGS.GetRegister(0), precision: 5));
        }

        [Fact]
        public void SUB_fr_InnnI()  // Ok
        {
            RunTest(
                "SUB F0,(0x4000)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                    c.MEMC.SetFloatToRam(0x4000, 0.75f);
                },
                c => Assert.Equal(0.25f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void SUB_fr_Innn_nnnI()  // Ok
        {
            RunTest(
                "SUB F0,(0x4000+4)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                    c.MEMC.SetFloatToRam(0x4004, 0.75f);
                },
                c => Assert.Equal(0.25f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void SUB_fr_Innn_rI()    // Ok
        {
            RunTest(
                "SUB F0,(0x4000+X)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                    c.CPU.REGS.X = 3;
                    c.MEMC.SetFloatToRam(0x4003, 0.75f);
                },
                c => Assert.Equal(0.25f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void SUB_fr_Innn_rrI()   // Ok
        {
            RunTest(
                "SUB F0,(0x4000+WX)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                    c.CPU.REGS.WX = 5;
                    c.MEMC.SetFloatToRam(0x4005, 0.75f);
                },
                c => Assert.Equal(0.25f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void SUB_fr_Innn_rrrI()  // Ok
        {
            RunTest(
                "SUB F0,(0x4000+VWX)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.SetFloatToRam(0x4007, 0.75f);
                },
                c => Assert.Equal(0.25f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void SUB_fr_IrrrI()  // Ok
        {
            RunTest(
                "SUB F0,(QRS)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.SetFloatToRam(0x5000, 0.75f);
                },
                c => Assert.Equal(0.25f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void SUB_fr_Irrr_nnnI()  // Ok
        {
            RunTest(
                "SUB F0,(QRS+4)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.SetFloatToRam(0x5004, 0.75f);
                },
                c => Assert.Equal(0.25f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void SUB_fr_Irrr_rI()    // Ok
        {
            RunTest(
                "SUB F0,(QRS+X)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.SetFloatToRam(0x5003, 0.75f);
                },
                c => Assert.Equal(0.25f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void SUB_fr_Irrr_rrI()   // Ok
        {
            RunTest(
                "SUB F0,(QRS+WX)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.SetFloatToRam(0x5005, 0.75f);
                },
                c => Assert.Equal(0.25f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void SUB_fr_Irrr_rrrI()  // Ok
        {
            RunTest(
                "SUB F0,(QRS+VWX)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.SetFloatToRam(0x5007, 0.75f);
                },
                c => Assert.Equal(0.25f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void SUB_fr_fr() // Ok
        {
            RunTest(
                "SUB F0,F1",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 10.5f);
                    c.CPU.FREGS.SetRegister(1, 2.75f);
                },
                c => Assert.Equal(7.75f, c.CPU.FREGS.GetRegister(0), precision: 5));
        }

        #endregion
    }
}
