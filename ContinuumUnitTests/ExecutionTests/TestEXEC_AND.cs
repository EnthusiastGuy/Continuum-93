using Continuum93.Emulator;
using Continuum93.Emulator.Interpreter;

namespace ExecutionTests
{
    public class TestEXEC_AND
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

        #region AND r,* tests

        [Fact]
        public void AND_r_n()   // Ok
        {
            RunTest(
                "AND A,0",
                c => c.CPU.REGS.A = 0b11111111,
                c => Assert.Equal((byte)0b00000000, c.CPU.REGS.A));
        }

        [Fact]
        public void AND_r_r()   // Ok
        {
            RunTest(
                "AND A,B",
                c =>
                {
                    c.CPU.REGS.A = 0b11111111;
                    c.CPU.REGS.B = 0b00100100;
                },
                c => Assert.Equal((byte)0b00100100, c.CPU.REGS.A));
        }

        [Fact]
        public void AND_r_InnnI()   // Ok
        {
            RunTest(
                "AND A,(0x4000)",
                c =>
                {
                    c.CPU.REGS.A = 0b11111111;
                    c.MEMC.Set8bitToRAM(0x4000, 0b00111100);
                },
                c => Assert.Equal((byte)0b00111100, c.CPU.REGS.A));
        }

        [Fact]
        public void AND_r_Innn_nnnI()   // Ok
        {
            RunTest(
                "AND A,(0x4000+4)",
                c =>
                {
                    c.CPU.REGS.A = 0b11111111;
                    c.MEMC.Set8bitToRAM(0x4004, 0b00111100);
                },
                c => Assert.Equal((byte)0b00111100, c.CPU.REGS.A));
        }

        [Fact]
        public void AND_r_Innn_rI() // Ok
        {
            RunTest(
                "AND A,(0x4000+X)",
                c =>
                {
                    c.CPU.REGS.A = 0b11111111;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set8bitToRAM(0x4003, 0b00111100);
                },
                c => Assert.Equal((byte)0b00111100, c.CPU.REGS.A));
        }

        [Fact]
        public void AND_r_Innn_rrI()    // Ok
        {
            RunTest(
                "AND A,(0x4000+WX)",
                c =>
                {
                    c.CPU.REGS.A = 0b11111111;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x4005, 0b00111100);
                },
                c => Assert.Equal((byte)0b00111100, c.CPU.REGS.A));
        }

        [Fact]
        public void AND_r_Innn_rrrI()   // Ok
        {
            RunTest(
                "AND A,(0x4000+VWX)",
                c =>
                {
                    c.CPU.REGS.A = 0b11111111;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x4007, 0b00111100);
                },
                c => Assert.Equal((byte)0b00111100, c.CPU.REGS.A));
        }

        [Fact]
        public void AND_r_IrrrI()   // Ok
        {
            RunTest(
                "AND A,(QRS)",
                c =>
                {
                    c.CPU.REGS.A = 0b11111111;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x5000, 0b00111100);
                },
                c => Assert.Equal((byte)0b00111100, c.CPU.REGS.A));
        }

        [Fact]
        public void AND_r_Irrr_nnnI()   // Ok
        {
            RunTest(
                "AND A,(QRS+4)",
                c =>
                {
                    c.CPU.REGS.A = 0b11111111;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x5004, 0b00111100);
                },
                c => Assert.Equal((byte)0b00111100, c.CPU.REGS.A));
        }

        [Fact]
        public void AND_r_Irrr_rI() // Ok
        {
            RunTest(
                "AND A,(QRS+X)",
                c =>
                {
                    c.CPU.REGS.A = 0b11111111;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set8bitToRAM(0x5003, 0b00111100);
                },
                c => Assert.Equal((byte)0b00111100, c.CPU.REGS.A));
        }

        [Fact]
        public void AND_r_Irrr_rrI()    // Ok
        {
            RunTest(
                "AND A,(QRS+WX)",
                c =>
                {
                    c.CPU.REGS.A = 0b11111111;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x5005, 0b00111100);
                },
                c => Assert.Equal((byte)0b00111100, c.CPU.REGS.A));
        }

        [Fact]
        public void AND_r_Irrr_rrrI()   // Ok
        {
            RunTest(
                "AND A,(QRS+VWX)",
                c =>
                {
                    c.CPU.REGS.A = 0b11111111;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x5007, 0b00111100);
                },
                c => Assert.Equal((byte)0b00111100, c.CPU.REGS.A));
        }

        [Fact]
        public void AND_r_fr()  // Ok
        {
            RunTest(
                "AND A,F0",
                c =>
                {
                    c.CPU.REGS.A = 0b11111111;
                    c.CPU.FREGS.SetRegister(0, 1.5f);
                },
                c => Assert.Equal((byte)0b00000001, c.CPU.REGS.A));
        }

        #endregion

        #region AND rr,* tests

        [Fact]
        public void AND_rr_nn() // Ok
        {
            RunTest(
                "AND AB,0x1234",
                c => c.CPU.REGS.AB = 0xFFFF,
                c => Assert.Equal((ushort)0x1234, c.CPU.REGS.AB));
        }

        [Fact]
        public void AND_rr_r()  // Ok
        {
            RunTest(
                "AND AB,E",
                c =>
                {
                    c.CPU.REGS.AB = 0xFFFF;
                    c.CPU.REGS.E = 0x34;
                },
                c => Assert.Equal((ushort)0x0034, c.CPU.REGS.AB));
        }

        [Fact]
        public void AND_rr_rr() // Ok
        {
            RunTest(
                "AND AB,CD",
                c =>
                {
                    c.CPU.REGS.AB = 0xFFFF;
                    c.CPU.REGS.CD = 0x00FF;
                },
                c => Assert.Equal((ushort)0x00FF, c.CPU.REGS.AB));
        }

        [Fact]
        public void AND_rr_InnnI()  // Ok
        {
            RunTest(
                "AND AB,(0x4000)",
                c =>
                {
                    c.CPU.REGS.AB = 0xFFFF;
                    c.MEMC.Set16bitToRAM(0x4000, 0x0F0F);
                },
                c => Assert.Equal((ushort)0x0F0F, c.CPU.REGS.AB));
        }

        [Fact]
        public void AND_rr_Innn_nnnI()  // Ok
        {
            RunTest(
                "AND AB,(0x4000+4)",
                c =>
                {
                    c.CPU.REGS.AB = 0xFFFF;
                    c.MEMC.Set16bitToRAM(0x4004, 0x0F0F);
                },
                c => Assert.Equal((ushort)0x0F0F, c.CPU.REGS.AB));
        }

        [Fact]
        public void AND_rr_Innn_rI()    // Ok
        {
            RunTest(
                "AND AB,(0x4000+X)",
                c =>
                {
                    c.CPU.REGS.AB = 0xFFFF;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set16bitToRAM(0x4003, 0x0F0F);
                },
                c => Assert.Equal((ushort)0x0F0F, c.CPU.REGS.AB));
        }

        [Fact]
        public void AND_rr_Innn_rrI()   // Ok
        {
            RunTest(
                "AND AB,(0x4000+WX)",
                c =>
                {
                    c.CPU.REGS.AB = 0xFFFF;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set16bitToRAM(0x4005, 0x0F0F);
                },
                c => Assert.Equal((ushort)0x0F0F, c.CPU.REGS.AB));
        }

        [Fact]
        public void AND_rr_Innn_rrrI()  // Ok
        {
            RunTest(
                "AND AB,(0x4000+VWX)",
                c =>
                {
                    c.CPU.REGS.AB = 0xFFFF;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set16bitToRAM(0x4007, 0x0F0F);
                },
                c => Assert.Equal((ushort)0x0F0F, c.CPU.REGS.AB));
        }

        [Fact]
        public void AND_rr_IrrrI()  // Ok
        {
            RunTest(
                "AND AB,(QRS)",
                c =>
                {
                    c.CPU.REGS.AB = 0xFFFF;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set16bitToRAM(0x5000, 0x0F0F);
                },
                c => Assert.Equal((ushort)0x0F0F, c.CPU.REGS.AB));
        }

        [Fact]
        public void AND_rr_Irrr_nnnI()  // Ok
        {
            RunTest(
                "AND AB,(QRS+4)",
                c =>
                {
                    c.CPU.REGS.AB = 0xFFFF;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set16bitToRAM(0x5004, 0x0F0F);
                },
                c => Assert.Equal((ushort)0x0F0F, c.CPU.REGS.AB));
        }

        [Fact]
        public void AND_rr_Irrr_rI()    // Ok
        {
            RunTest(
                "AND AB,(QRS+X)",
                c =>
                {
                    c.CPU.REGS.AB = 0xFFFF;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set16bitToRAM(0x5003, 0x0F0F);
                },
                c => Assert.Equal((ushort)0x0F0F, c.CPU.REGS.AB));
        }

        [Fact]
        public void AND_rr_Irrr_rrI()   // Ok
        {
            RunTest(
                "AND AB,(QRS+WX)",
                c =>
                {
                    c.CPU.REGS.AB = 0xFFFF;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set16bitToRAM(0x5005, 0x0F0F);
                },
                c => Assert.Equal((ushort)0x0F0F, c.CPU.REGS.AB));
        }

        [Fact]
        public void AND_rr_Irrr_rrrI()  // Ok
        {
            RunTest(
                "AND AB,(QRS+VWX)",
                c =>
                {
                    c.CPU.REGS.AB = 0xFFFF;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set16bitToRAM(0x5007, 0x0F0F);
                },
                c => Assert.Equal((ushort)0x0F0F, c.CPU.REGS.AB));
        }

        [Fact]
        public void AND_rr_fr() // Ok
        {
            RunTest(
                "AND AB,F1",
                c =>
                {
                    c.CPU.REGS.AB = 0xFFFF;
                    c.CPU.FREGS.SetRegister(1, 5.6f);
                },
                c => Assert.Equal((ushort)0x0006, c.CPU.REGS.AB));
        }

        #endregion

        #region AND rrr,* tests

        [Fact]
        public void AND_rrr_nnn()   // Ok
        {
            RunTest(
                "AND ABC,0x000102",
                c => c.CPU.REGS.ABC = 0xFFFFFF,
                c => Assert.Equal((uint)0x000102, c.CPU.REGS.ABC));
        }

        [Fact]
        public void AND_rrr_r() // Ok
        {
            RunTest(
                "AND ABC,E",
                c =>
                {
                    c.CPU.REGS.ABC = 0xFFFFFF;
                    c.CPU.REGS.E = 7;
                },
                c => Assert.Equal((uint)0x000007, c.CPU.REGS.ABC));
        }

        [Fact]
        public void AND_rrr_rr()    // Ok
        {
            RunTest(
                "AND ABC,EF",
                c =>
                {
                    c.CPU.REGS.ABC = 0xFFFFFF;
                    c.CPU.REGS.EF = 0x0102;
                },
                c => Assert.Equal((uint)0x000102, c.CPU.REGS.ABC));
        }

        [Fact]
        public void AND_rrr_rrr()   // Ok
        {
            RunTest(
                "AND ABC,DEF",
                c =>
                {
                    c.CPU.REGS.ABC = 0xFFFFFF;
                    c.CPU.REGS.DEF = 0x00000F;
                },
                c => Assert.Equal((uint)0x00000F, c.CPU.REGS.ABC));
        }

        [Fact]
        public void AND_rrr_InnnI() // Ok
        {
            RunTest(
                "AND ABC,(0x4000)",
                c =>
                {
                    c.CPU.REGS.ABC = 0xFFFFFF;
                    c.MEMC.Set24bitToRAM(0x4000, 0x000F0F);
                },
                c => Assert.Equal((uint)0x000F0F, c.CPU.REGS.ABC));
        }

        [Fact]
        public void AND_rrr_Innn_nnnI() // Ok
        {
            RunTest(
                "AND ABC,(0x4000+4)",
                c =>
                {
                    c.CPU.REGS.ABC = 0xFFFFFF;
                    c.MEMC.Set24bitToRAM(0x4004, 0x000F0F);
                },
                c => Assert.Equal((uint)0x000F0F, c.CPU.REGS.ABC));
        }

        [Fact]
        public void AND_rrr_Innn_rI()   // Ok
        {
            RunTest(
                "AND ABC,(0x4000+X)",
                c =>
                {
                    c.CPU.REGS.ABC = 0xFFFFFF;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set24bitToRAM(0x4003, 0x000F0F);
                },
                c => Assert.Equal((uint)0x000F0F, c.CPU.REGS.ABC));
        }

        [Fact]
        public void AND_rrr_Innn_rrI()  // Ok
        {
            RunTest(
                "AND ABC,(0x4000+WX)",
                c =>
                {
                    c.CPU.REGS.ABC = 0xFFFFFF;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set24bitToRAM(0x4005, 0x000F0F);
                },
                c => Assert.Equal((uint)0x000F0F, c.CPU.REGS.ABC));
        }

        [Fact]
        public void AND_rrr_Innn_rrrI() // Ok
        {
            RunTest(
                "AND ABC,(0x4000+VWX)",
                c =>
                {
                    c.CPU.REGS.ABC = 0xFFFFFF;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set24bitToRAM(0x4007, 0x000F0F);
                },
                c => Assert.Equal((uint)0x000F0F, c.CPU.REGS.ABC));
        }

        [Fact]
        public void AND_rrr_IrrrI() // Ok
        {
            RunTest(
                "AND ABC,(QRS)",
                c =>
                {
                    c.CPU.REGS.ABC = 0xFFFFFF;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set24bitToRAM(0x5000, 0x000F0F);
                },
                c => Assert.Equal((uint)0x000F0F, c.CPU.REGS.ABC));
        }

        [Fact]
        public void AND_rrr_Irrr_nnnI() // Ok
        {
            RunTest(
                "AND ABC,(QRS+4)",
                c =>
                {
                    c.CPU.REGS.ABC = 0xFFFFFF;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set24bitToRAM(0x5004, 0x000F0F);
                },
                c => Assert.Equal((uint)0x000F0F, c.CPU.REGS.ABC));
        }

        [Fact]
        public void AND_rrr_Irrr_rI()   // Ok
        {
            RunTest(
                "AND ABC,(QRS+X)",
                c =>
                {
                    c.CPU.REGS.ABC = 0xFFFFFF;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set24bitToRAM(0x5003, 0x000F0F);
                },
                c => Assert.Equal((uint)0x000F0F, c.CPU.REGS.ABC));
        }

        [Fact]
        public void AND_rrr_Irrr_rrI()  // Ok
        {
            RunTest(
                "AND ABC,(QRS+WX)",
                c =>
                {
                    c.CPU.REGS.ABC = 0xFFFFFF;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set24bitToRAM(0x5005, 0x000F0F);
                },
                c => Assert.Equal((uint)0x000F0F, c.CPU.REGS.ABC));
        }

        [Fact]
        public void AND_rrr_Irrr_rrrI() // Ok
        {
            RunTest(
                "AND ABC,(QRS+VWX)",
                c =>
                {
                    c.CPU.REGS.ABC = 0xFFFFFF;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set24bitToRAM(0x5007, 0x000F0F);
                },
                c => Assert.Equal((uint)0x000F0F, c.CPU.REGS.ABC));
        }

        [Fact]
        public void AND_rrr_fr()    // Ok
        {
            RunTest(
                "AND ABC,F2",
                c =>
                {
                    c.CPU.REGS.ABC = 0xFFFFFF;
                    c.CPU.FREGS.SetRegister(2, 12.5f);
                },
                c => Assert.Equal((uint)0x00000C, c.CPU.REGS.ABC));
        }

        #endregion

        #region AND rrrr,* tests

        [Fact]
        public void AND_rrrr_nnnn() // Ok
        {
            RunTest(
                "AND ABCD,0x00010002",
                c => c.CPU.REGS.ABCD = 0xFFFFFFFF,
                c => Assert.Equal((uint)0x00010002, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void AND_rrrr_r()    // Ok
        {
            RunTest(
                "AND ABCD,E",
                c =>
                {
                    c.CPU.REGS.ABCD = 0xFFFFFFFF;
                    c.CPU.REGS.E = 9;
                },
                c => Assert.Equal((uint)0x00000009, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void AND_rrrr_rr()   // Ok
        {
            RunTest(
                "AND ABCD,EF",
                c =>
                {
                    c.CPU.REGS.ABCD = 0xFFFFFFFF;
                    c.CPU.REGS.EF = 0x00FF;
                },
                c => Assert.Equal((uint)0x000000FF, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void AND_rrrr_rrr()  // Ok
        {
            RunTest(
                "AND ABCD,DEF",
                c =>
                {
                    c.CPU.REGS.ABCD = 0xFFFFFFFF;
                    c.CPU.REGS.DEF = 0x0000AA;
                },
                c => Assert.Equal((uint)0x000000AA, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void AND_rrrr_rrrr() // Ok
        {
            RunTest(
                "AND ABCD,EFGH",
                c =>
                {
                    c.CPU.REGS.ABCD = 0xFFFFFFFF;
                    c.CPU.REGS.EFGH = 0x00000010;
                },
                c => Assert.Equal((uint)0x00000010, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void AND_rrrr_InnnI()    // Ok
        {
            RunTest(
                "AND ABCD,(0x4000)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0xFFFFFFFF;
                    c.MEMC.Set32bitToRAM(0x4000, 0x000000FF);
                },
                c => Assert.Equal((uint)0x000000FF, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void AND_rrrr_Innn_nnnI()    // Ok
        {
            RunTest(
                "AND ABCD,(0x4000+4)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0xFFFFFFFF;
                    c.MEMC.Set32bitToRAM(0x4004, 0x000000FF);
                },
                c => Assert.Equal((uint)0x000000FF, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void AND_rrrr_Innn_rI()  // Ok
        {
            RunTest(
                "AND ABCD,(0x4000+X)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0xFFFFFFFF;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set32bitToRAM(0x4003, 0x000000FF);
                },
                c => Assert.Equal((uint)0x000000FF, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void AND_rrrr_Innn_rrI() // Ok
        {
            RunTest(
                "AND ABCD,(0x4000+WX)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0xFFFFFFFF;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set32bitToRAM(0x4005, 0x000000FF);
                },
                c => Assert.Equal((uint)0x000000FF, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void AND_rrrr_Innn_rrrI()    // Ok
        {
            RunTest(
                "AND ABCD,(0x4000+VWX)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0xFFFFFFFF;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set32bitToRAM(0x4007, 0x000000FF);
                },
                c => Assert.Equal((uint)0x000000FF, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void AND_rrrr_IrrrI()    // Ok
        {
            RunTest(
                "AND ABCD,(QRS)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0xFFFFFFFF;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set32bitToRAM(0x5000, 0x000000FF);
                },
                c => Assert.Equal((uint)0x000000FF, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void AND_rrrr_Irrr_nnnI()    // Ok
        {
            RunTest(
                "AND ABCD,(QRS+4)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0xFFFFFFFF;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set32bitToRAM(0x5004, 0x000000FF);
                },
                c => Assert.Equal((uint)0x000000FF, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void AND_rrrr_Irrr_rI()  // Ok
        {
            RunTest(
                "AND ABCD,(QRS+X)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0xFFFFFFFF;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set32bitToRAM(0x5003, 0x000000FF);
                },
                c => Assert.Equal((uint)0x000000FF, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void AND_rrrr_Irrr_rrI() // Ok
        {
            RunTest(
                "AND ABCD,(QRS+WX)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0xFFFFFFFF;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set32bitToRAM(0x5005, 0x000000FF);
                },
                c => Assert.Equal((uint)0x000000FF, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void AND_rrrr_Irrr_rrrI()    // Ok
        {
            RunTest(
                "AND ABCD,(QRS+VWX)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0xFFFFFFFF;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set32bitToRAM(0x5007, 0x000000FF);
                },
                c => Assert.Equal((uint)0x000000FF, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void AND_rrrr_fr()   // Ok
        {
            RunTest(
                "AND ABCD,F3",
                c =>
                {
                    c.CPU.REGS.ABCD = 0xFFFFFFFF;
                    c.CPU.FREGS.SetRegister(3, 42.8f);
                },
                c => Assert.Equal((uint)0x0000002B, c.CPU.REGS.ABCD));
        }

        #endregion

        #region AND (nnn),* tests - immediate and register

        [Fact]
        public void AND_InnnI_nnnn_n()  // Ok
        {
            RunTest(
                "AND (0x4000),0x00010203,2",
                c => c.LoadMemAt(0x4000, [0xFF, 0xFF, 0xFF]),
                c => Assert.Equal([0x03, 0x02], c.MEMC.RAM.GetMemoryAt(0x4000, 2)));
        }

        [Fact]
        public void AND_InnnI_nnnn_n_repeat()   // Ok
        {
            RunTest(
                "AND (0x4000),0x00010203,3,2",
                c => c.LoadMemAt(0x4000, [0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF]),
                c => Assert.Equal([0x03, 0x02, 0x01, 0x03, 0x02, 0x01], c.MEMC.RAM.GetMemoryAt(0x4000, 6)));
        }

        [Fact]
        public void AND_InnnI_r()   // Ok
        {
            RunTest(
                "AND (0x4000),B",
                c =>
                {
                    c.LoadMemAt(0x4000, [0xFF, 0xFF]);
                    c.CPU.REGS.B = 0x05;
                },
                c => Assert.Equal(0x05, c.MEMC.Get8bitFromRAM(0x4000)));
        }

        [Fact]
        public void AND_InnnI_rr()  // Ok
        {
            RunTest(
                "AND (0x4000),CD",
                c =>
                {
                    c.LoadMemAt(0x4000, [0xFF, 0xFF]);
                    c.CPU.REGS.CD = 0x0102;
                },
                c => Assert.Equal(0x0102, c.MEMC.Get16bitFromRAM(0x4000)));
        }

        [Fact]
        public void AND_InnnI_rrr() // Ok
        {
            RunTest(
                "AND (0x4000),DEF",
                c =>
                {
                    c.LoadMemAt(0x4000, [0xFF, 0xFF, 0xFF]);
                    c.CPU.REGS.DEF = 0x000203;
                },
                c => Assert.Equal((uint)0x000203, c.MEMC.Get24bitFromRAM(0x4000)));
        }

        [Fact]
        public void AND_InnnI_rrrr()    // Ok
        {
            RunTest(
                "AND (0x4000),EFGH",
                c =>
                {
                    c.LoadMemAt(0x4000, [0xFF, 0xFF, 0xFF, 0xFF]);
                    c.CPU.REGS.EFGH = 0x00030405;
                },
                c => Assert.Equal((uint)0x00030405, c.MEMC.Get32bitFromRAM(0x4000)));
        }

        [Fact]
        public void AND_InnnI_InnnI_n_rrr() // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (0x4000),(0x7000),3,GHI",
                c =>
                {
                    c.LoadMemAt(0x4000, initial);
                    c.LoadMemAt(0x7000, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4000, 4));
                });
        }

        [Fact]
        public void AND_InnnI_Innn_nnnI_n_rrr() // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (0x4000),(0x7000+4),3,GHI",
                c =>
                {
                    c.LoadMemAt(0x4000, initial);
                    c.LoadMemAt(0x7004, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4000, 4));
                });
        }

        [Fact]
        public void AND_InnnI_Innn_rI_n_rrr()   // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (0x4000),(0x7000+K),3,GHI",
                c =>
                {
                    c.LoadMemAt(0x4000, initial);
                    c.CPU.REGS.K = 2;
                    c.LoadMemAt(0x7002, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4000, 4));
                });
        }

        [Fact]
        public void AND_InnnI_Innn_rrI_n_rrr()  // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (0x4000),(0x7000+KL),3,GHI",
                c =>
                {
                    c.LoadMemAt(0x4000, initial);
                    c.CPU.REGS.KL = 6;
                    c.LoadMemAt(0x7006, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4000, 4));
                });
        }

        [Fact]
        public void AND_InnnI_Innn_rrrI_n_rrr() // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (0x4000),(0x7000+KLM),3,GHI",
                c =>
                {
                    c.LoadMemAt(0x4000, initial);
                    c.CPU.REGS.KLM = 9;
                    c.LoadMemAt(0x7009, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4000, 4));
                });
        }

        [Fact]
        public void AND_InnnI_IrrrI_n_rrr() // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (0x4000),(DEF),3,GHI",
                c =>
                {
                    c.LoadMemAt(0x4000, initial);
                    c.CPU.REGS.DEF = 0x7100;
                    c.LoadMemAt(0x7100, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4000, 4));
                });
        }

        [Fact]
        public void AND_InnnI_Irrr_nnnI_n_rrr() // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (0x4000),(DEF+4),3,GHI",
                c =>
                {
                    c.LoadMemAt(0x4000, initial);
                    c.CPU.REGS.DEF = 0x7100;
                    c.LoadMemAt(0x7104, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4000, 4));
                });
        }

        [Fact]
        public void AND_InnnI_Irrr_rI_n_rrr()   // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (0x4000),(DEF+K),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4000, 4));
                });
        }

        [Fact]
        public void AND_InnnI_Irrr_rrI_n_rrr()  // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (0x4000),(DEF+KL),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4000, 4));
                });
        }

        [Fact]
        public void AND_InnnI_Irrr_rrrI_n_rrr() // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (0x4000),(DEF+KLM),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4000, 4));
                });
        }

        [Fact]
        public void AND_InnnI_fr()  // Ok
        {
            RunTest(
                "AND (0x4000),F4",
                c =>
                {
                    c.MEMC.SetFloatToRam(0x4000, 1.25f);
                    c.CPU.FREGS.SetRegister(4, 2.5f);
                },
                c => Assert.Equal(0.0f, c.MEMC.GetFloatFromRAM(0x4000)));
        }

        [Fact]
        public void AND_Innn_nnnI_nnnn_n()  // Ok
        {
            RunTest(
                "AND (0x4000+4),0x00010203,2",
                c => c.LoadMemAt(0x4004, [0xFF, 0xFF, 0xFF]),
                c => Assert.Equal([0x03, 0x02, 0xFF], c.MEMC.RAM.GetMemoryAt(0x4004, 3)));
        }

        [Fact]
        public void AND_Innn_nnnI_nnnn_n_nnn()  // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            RunTest(
                "AND (0x4000+4),0x00010203,3,2",
                c =>
                {
                    c.LoadMemAt(0x4004, initial);
                },
                c =>
                {
                    byte[] expected = [0x03, 0x02, 0x01, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4004, 4));
                });
        }

        [Fact]
        public void AND_Innn_nnnI_r()   // Ok
        {
            RunTest(
                "AND (0x4000+4),B",
                c =>
                {
                    c.LoadMemAt(0x4004, [0xFF, 0xFF]);
                    c.CPU.REGS.B = 5;
                },
                c => Assert.Equal(0x05, c.MEMC.Get8bitFromRAM(0x4004)));
        }

        [Fact]
        public void AND_Innn_nnnI_rr()  // Ok
        {
            RunTest(
                "AND (0x4000+4),CD",
                c =>
                {
                    c.LoadMemAt(0x4004, [0xFF, 0xFF]);
                    c.CPU.REGS.CD = 0x0102;
                },
                c => Assert.Equal((ushort)0x0102, c.MEMC.Get16bitFromRAM(0x4004)));
        }

        [Fact]
        public void AND_Innn_nnnI_rrr() // Ok
        {
            RunTest(
                "AND (0x4000+4),DEF",
                c =>
                {
                    c.LoadMemAt(0x4004, [0xFF, 0xFF, 0xFF]);
                    c.CPU.REGS.DEF = 0x000203;
                },
                c => Assert.Equal((uint)0x000203, c.MEMC.Get24bitFromRAM(0x4004)));
        }

        [Fact]
        public void AND_Innn_nnnI_rrrr()    // Ok
        {
            RunTest(
                "AND (0x4000+4),EFGH",
                c =>
                {
                    c.LoadMemAt(0x4004, [0xFF, 0xFF, 0xFF, 0xFF]);
                    c.CPU.REGS.EFGH = 0x00030405;
                },
                c => Assert.Equal((uint)0x00030405, c.MEMC.Get32bitFromRAM(0x4004)));
        }

        [Fact]
        public void AND_Innn_nnnI_InnnI_n_rrr() // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (0x4000+4),(0x7000),3,GHI",
                c =>
                {
                    c.LoadMemAt(0x4004, initial);
                    c.LoadMemAt(0x7000, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4004, 4));
                });
        }

        [Fact]
        public void AND_Innn_nnnI_Innn_nnnI_n_rrr() // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (0x4000+4),(0x7000+4),3,GHI",
                c =>
                {
                    c.LoadMemAt(0x4004, initial);
                    c.LoadMemAt(0x7004, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4004, 4));
                });
        }

        [Fact]
        public void AND_Innn_nnnI_Innn_rI_n_rrr()   // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (0x4000+4),(0x7000+K),3,GHI",
                c =>
                {
                    c.LoadMemAt(0x4004, initial);
                    c.CPU.REGS.K = 2;
                    c.LoadMemAt(0x7002, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4004, 4));
                });
        }

        [Fact]
        public void AND_Innn_nnnI_Innn_rrI_n_rrr()  // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (0x4000+4),(0x7000+KL),3,GHI",
                c =>
                {
                    c.LoadMemAt(0x4004, initial);
                    c.CPU.REGS.KL = 6;
                    c.LoadMemAt(0x7006, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4004, 4));
                });
        }

        [Fact]
        public void AND_Innn_nnnI_Innn_rrrI_n_rrr() // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (0x4000+4),(0x7000+KLM),3,GHI",
                c =>
                {
                    c.LoadMemAt(0x4004, initial);
                    c.CPU.REGS.KLM = 9;
                    c.LoadMemAt(0x7009, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4004, 4));
                });
        }

        [Fact]
        public void AND_Innn_nnnI_IrrrI_n_rrr() // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (0x4000+4),(DEF),3,GHI",
                c =>
                {
                    c.LoadMemAt(0x4004, initial);
                    c.CPU.REGS.DEF = 0x7100;
                    c.LoadMemAt(0x7100, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4004, 4));
                });
        }

        [Fact]
        public void AND_Innn_nnnI_Irrr_nnnI_n_rrr() // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (0x4000+4),(DEF+4),3,GHI",
                c =>
                {
                    c.LoadMemAt(0x4004, initial);
                    c.CPU.REGS.DEF = 0x7100;
                    c.LoadMemAt(0x7104, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4004, 4));
                });
        }

        [Fact]
        public void AND_Innn_nnnI_Irrr_rI_n_rrr()   // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (0x4000+4),(DEF+K),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4004, 4));
                });
        }

        [Fact]
        public void AND_Innn_nnnI_Irrr_rrI_n_rrr()  // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (0x4000+4),(DEF+KL),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4004, 4));
                });
        }

        [Fact]
        public void AND_Innn_nnnI_Irrr_rrrI_n_rrr() // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (0x4000+4),(DEF+KLM),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4004, 4));
                });
        }

        [Fact]
        public void AND_Innn_nnnI_fr()  // Ok
        {
            RunTest(
                "AND (0x4000+4),F4",
                c =>
                {
                    c.MEMC.SetFloatToRam(0x4004, 1.25f);
                    c.CPU.FREGS.SetRegister(4, 2.5f);
                },
                c => Assert.Equal(0.0f, c.MEMC.GetFloatFromRAM(0x4004)));
        }

        [Fact]
        public void AND_Innn_rI_nnnn_n()    // Ok
        {
            RunTest(
                "AND (0x4000+X),0x00010203,3",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.LoadMemAt(0x4003, [0xFF, 0xFF, 0xFF, 0xFF]);
                },
                c =>
                {
                    byte[] expected = [0x03, 0x02, 0x01, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4003, 4));
                });
        }

        [Fact]
        public void AND_Innn_rI_nnnn_n_nnn()    // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            RunTest(
                "AND (0x4000+X),0x00010203,3,2",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.LoadMemAt(0x4003, initial);
                },
                c =>
                {
                    byte[] expected = [0x03, 0x02, 0x01, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4003, 4));
                });
        }

        [Fact]
        public void AND_Innn_rI_r() // Ok
        {
            RunTest(
                "AND (0x4000+X),B",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.LoadMemAt(0x4003, [0xFF, 0xFF]);
                    c.CPU.REGS.B = 5;
                },
                c => Assert.Equal([0x05, 0xFF], c.MEMC.RAM.GetMemoryAt(0x4003, 2)));
        }

        [Fact]
        public void AND_Innn_rI_rr()    // Ok
        {
            RunTest(
                "AND (0x4000+X),CD",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.LoadMemAt(0x4003, [0xFF, 0xFF, 0xFF]);
                    c.CPU.REGS.CD = 0x0102;
                },
                c => Assert.Equal([0x02, 0x01, 0xFF], c.MEMC.RAM.GetMemoryAt(0x4003, 3)));
        }

        [Fact]
        public void AND_Innn_rI_rrr()   // Ok
        {
            RunTest(
                "AND (0x4000+X),DEF",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.LoadMemAt(0x4003, [0xFF, 0xFF, 0xFF, 0xFF]);
                    c.CPU.REGS.DEF = 0x010203;
                },
                c => Assert.Equal([0x03, 0x02, 0x01, 0xFF], c.MEMC.RAM.GetMemoryAt(0x4003, 4)));
        }

        [Fact]
        public void AND_Innn_rI_rrrr()  // Ok
        {
            RunTest(
                "AND (0x4000+X),EFGH",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.LoadMemAt(0x4003, [0xFF, 0xFF, 0xFF, 0xFF, 0xFF]);
                    c.CPU.REGS.EFGH = 0x05040302;
                },
                c => Assert.Equal([0x02, 0x03, 0x04, 0x05, 0xFF], c.MEMC.RAM.GetMemoryAt(0x4003, 5)));
        }

        [Fact]
        public void AND_Innn_rI_InnnI_n_rrr()   // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (0x4000+X),(0x7000),3,GHI",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.LoadMemAt(0x4003, initial);
                    c.LoadMemAt(0x7000, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4003, 4));
                });
        }

        [Fact]
        public void AND_Innn_rI_Innn_nnnI_n_rrr()   // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (0x4000+X),(0x7000+4),3,GHI",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.LoadMemAt(0x4003, initial);
                    c.LoadMemAt(0x7004, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4003, 4));
                });
        }

        [Fact]
        public void AND_Innn_rI_Innn_rI_n_rrr() // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (0x4000+X),(0x7000+K),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4003, 4));
                });
        }

        [Fact]
        public void AND_Innn_rI_Innn_rrI_n_rrr()    // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (0x4000+X),(0x7000+KL),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4003, 4));
                });
        }

        [Fact]
        public void AND_Innn_rI_Innn_rrrI_n_rrr()   // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (0x4000+X),(0x7000+KLM),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4003, 4));
                });
        }

        [Fact]
        public void AND_Innn_rI_IrrrI_n_rrr()   // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (0x4000+X),(DEF),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4003, 4));
                });
        }

        [Fact]
        public void AND_Innn_rI_Irrr_nnnI_n_rrr()   // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (0x4000+X),(DEF+4),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4003, 4));
                });
        }

        [Fact]
        public void AND_Innn_rI_Irrr_rI_n_rrr() // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (0x4000+X),(DEF+K),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4003, 4));
                });
        }

        [Fact]
        public void AND_Innn_rI_Irrr_rrI_n_rrr()    // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (0x4000+X),(DEF+KL),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4003, 4));
                });
        }

        [Fact]
        public void AND_Innn_rI_Irrr_rrrI_n_rrr()   // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (0x4000+X),(DEF+KLM),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4003, 4));
                });
        }

        [Fact]
        public void AND_Innn_rI_fr()    // Ok
        {
            RunTest(
                "AND (0x4000+X),F4",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.MEMC.SetFloatToRam(0x4003, 1.25f);
                    c.CPU.FREGS.SetRegister(4, 2.5f);
                },
                c => Assert.Equal(0.0f, c.MEMC.GetFloatFromRAM(0x4003)));
        }

        [Fact]
        public void AND_Innn_rrI_nnnn_n()   // Ok
        {
            RunTest(
                "AND (0x4000+WX),0x00010203,3",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    c.LoadMemAt(0x4005, [0xFF, 0xFF, 0xFF, 0xFF]);
                },
                c =>
                {
                    byte[] expected = [0x03, 0x02, 0x01, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4005, 4));
                });
        }

        [Fact]
        public void AND_Innn_rrI_nnnn_n_nnn()   // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            RunTest(
                "AND (0x4000+WX),0x00010203,3,2",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    c.LoadMemAt(0x4005, initial);
                },
                c =>
                {
                    byte[] expected = [0x03, 0x02, 0x01, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4005, 4));
                });
        }

        [Fact]
        public void AND_Innn_rrI_r()    // Ok
        {
            RunTest(
                "AND (0x4000+WX),B",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    c.LoadMemAt(0x4005, [0xFF, 0xFF]);
                    c.CPU.REGS.B = 5;
                },
                c => Assert.Equal((byte)0x05, c.MEMC.Get8bitFromRAM(0x4005)));
        }

        [Fact]
        public void AND_Innn_rrI_rr()   // Ok
        {
            RunTest(
                "AND (0x4000+WX),CD",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    c.LoadMemAt(0x4005, [0xFF, 0xFF]);
                    c.CPU.REGS.CD = 0x0102;
                },
                c => Assert.Equal((ushort)0x0102, c.MEMC.Get16bitFromRAM(0x4005)));
        }

        [Fact]
        public void AND_Innn_rrI_rrr()  // Ok
        {
            RunTest(
                "AND (0x4000+WX),DEF",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    c.LoadMemAt(0x4005, [0xFF, 0xFF, 0xFF]);
                    c.CPU.REGS.DEF = 0x000203;
                },
                c => Assert.Equal((uint)0x000203, c.MEMC.Get24bitFromRAM(0x4005)));
        }

        [Fact]
        public void AND_Innn_rrI_rrrr() // Ok
        {
            RunTest(
                "AND (0x4000+WX),EFGH",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    c.LoadMemAt(0x4005, [0xFF, 0xFF, 0xFF, 0xFF]);
                    c.CPU.REGS.EFGH = 0x00030405;
                },
                c => Assert.Equal((uint)0x00030405, c.MEMC.Get32bitFromRAM(0x4005)));
        }

        [Fact]
        public void AND_Innn_rrI_InnnI_n_rrr()  // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (0x4000+WX),(0x7000),3,GHI",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    c.LoadMemAt(0x4005, initial);
                    c.LoadMemAt(0x7000, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4005, 4));
                });
        }

        [Fact]
        public void AND_Innn_rrI_Innn_nnnI_n_rrr()  // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (0x4000+WX),(0x7000+4),3,GHI",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    c.LoadMemAt(0x4005, initial);
                    c.LoadMemAt(0x7004, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4005, 4));
                });
        }

        [Fact]
        public void AND_Innn_rrI_Innn_rI_n_rrr()    // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (0x4000+WX),(0x7000+K),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4005, 4));
                });
        }

        [Fact]
        public void AND_Innn_rrI_Innn_rrI_n_rrr()   // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (0x4000+WX),(0x7000+KL),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4005, 4));
                });
        }

        [Fact]
        public void AND_Innn_rrI_Innn_rrrI_n_rrr()  // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (0x4000+WX),(0x7000+KLM),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4005, 4));
                });
        }

        [Fact]
        public void AND_Innn_rrI_IrrrI_n_rrr()  // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (0x4000+WX),(DEF),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4005, 4));
                });
        }

        [Fact]
        public void AND_Innn_rrI_Irrr_nnnI_n_rrr()  // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (0x4000+WX),(DEF+4),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4005, 4));
                });
        }

        [Fact]
        public void AND_Innn_rrI_Irrr_rI_n_rrr()    // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (0x4000+WX),(DEF+K),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4005, 4));
                });
        }

        [Fact]
        public void AND_Innn_rrI_Irrr_rrI_n_rrr()   // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (0x4000+WX),(DEF+KL),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4005, 4));
                });
        }

        [Fact]
        public void AND_Innn_rrI_Irrr_rrrI_n_rrr()  // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (0x4000+WX),(DEF+KLM),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4005, 4));
                });
        }

        [Fact]
        public void AND_Innn_rrI_fr()   // Ok
        {
            RunTest(
                "AND (0x4000+WX),F4",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    c.MEMC.SetFloatToRam(0x4005, 1.25f);
                    c.CPU.FREGS.SetRegister(4, 2.5f);
                },
                c => Assert.Equal(0.0f, c.MEMC.GetFloatFromRAM(0x4005)));
        }

        [Fact]
        public void AND_Innn_rrrI_nnnn_n()  // Ok
        {
            RunTest(
                "AND (0x4000+VWX),0x00010203,3",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    c.LoadMemAt(0x4007, [0xFF, 0xFF, 0xFF, 0xFF]);
                },
                c =>
                {
                    byte[] expected = [0x03, 0x02, 0x01, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4007, 4));
                });
        }

        [Fact]
        public void AND_Innn_rrrI_nnnn_n_nnn()  // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            RunTest(
                "AND (0x4000+VWX),0x00010203,3,2",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    c.LoadMemAt(0x4007, initial);
                },
                c =>
                {
                    byte[] expected = [0x03, 0x02, 0x01, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4007, 4));
                });
        }

        [Fact]
        public void AND_Innn_rrrI_r()   // Ok
        {
            RunTest(
                "AND (0x4000+VWX),B",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    c.LoadMemAt(0x4007, [0xFF, 0xFF]);
                    c.CPU.REGS.B = 5;
                },
                c => Assert.Equal((byte)0x05, c.MEMC.Get8bitFromRAM(0x4007)));
        }

        [Fact]
        public void AND_Innn_rrrI_rr()  // Ok
        {
            RunTest(
                "AND (0x4000+VWX),CD",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    c.LoadMemAt(0x4007, [0xFF, 0xFF]);
                    c.CPU.REGS.CD = 0x0102;
                },
                c => Assert.Equal((ushort)0x0102, c.MEMC.Get16bitFromRAM(0x4007)));
        }

        [Fact]
        public void AND_Innn_rrrI_rrr() // Ok
        {
            RunTest(
                "AND (0x4000+VWX),DEF",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    c.LoadMemAt(0x4007, [0xFF, 0xFF, 0xFF]);
                    c.CPU.REGS.DEF = 0x000203;
                },
                c => Assert.Equal((uint)0x000203, c.MEMC.Get24bitFromRAM(0x4007)));
        }

        [Fact]
        public void AND_Innn_rrrI_rrrr()    // Ok
        {
            RunTest(
                "AND (0x4000+VWX),EFGH",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    c.LoadMemAt(0x4007, [0xFF, 0xFF, 0xFF, 0xFF]);
                    c.CPU.REGS.EFGH = 0x00030405;
                },
                c => Assert.Equal((uint)0x00030405, c.MEMC.Get32bitFromRAM(0x4007)));
        }

        [Fact]
        public void AND_Innn_rrrI_InnnI_n_rrr() // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (0x4000+VWX),(0x7000),3,GHI",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    c.LoadMemAt(0x4007, initial);
                    c.LoadMemAt(0x7000, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4007, 4));
                });
        }

        [Fact]
        public void AND_Innn_rrrI_Innn_nnnI_n_rrr() // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (0x4000+VWX),(0x7000+4),3,GHI",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    c.LoadMemAt(0x4007, initial);
                    c.LoadMemAt(0x7004, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4007, 4));
                });
        }

        [Fact]
        public void AND_Innn_rrrI_Innn_rI_n_rrr()   // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (0x4000+VWX),(0x7000+K),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4007, 4));
                });
        }

        [Fact]
        public void AND_Innn_rrrI_Innn_rrI_n_rrr()  // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (0x4000+VWX),(0x7000+KL),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4007, 4));
                });
        }

        [Fact]
        public void AND_Innn_rrrI_Innn_rrrI_n_rrr() // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (0x4000+VWX),(0x7000+KLM),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4007, 4));
                });
        }

        [Fact]
        public void AND_Innn_rrrI_IrrrI_n_rrr() // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (0x4000+VWX),(DEF),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4007, 4));
                });
        }

        [Fact]
        public void AND_Innn_rrrI_Irrr_nnnI_n_rrr() // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (0x4000+VWX),(DEF+4),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4007, 4));
                });
        }

        [Fact]
        public void AND_Innn_rrrI_Irrr_rI_n_rrr()   // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (0x4000+VWX),(DEF+K),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4007, 4));
                });
        }

        [Fact]
        public void AND_Innn_rrrI_Irrr_rrI_n_rrr()  // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (0x4000+VWX),(DEF+KL),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4007, 4));
                });
        }

        [Fact]
        public void AND_Innn_rrrI_Irrr_rrrI_n_rrr() // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (0x4000+VWX),(DEF+KLM),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4007, 4));
                });
        }

        [Fact]
        public void AND_Innn_rrrI_fr()  // Ok
        {
            RunTest(
                "AND (0x4000+VWX),F4",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.SetFloatToRam(0x4007, 1.25f);
                    c.CPU.FREGS.SetRegister(4, 2.5f);
                },
                c => Assert.Equal(0.0f, c.MEMC.GetFloatFromRAM(0x4007)));
        }

        [Fact]
        public void AND_IrrrI_nnnn_n()  // Ok
        {
            RunTest(
                "AND (QRS),0x00010203,3",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5000, [0xFF, 0xFF, 0xFF, 0xFF]);
                },
                c =>
                {
                    byte[] expected = [0x03, 0x02, 0x01, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5000, 4));
                });
        }

        [Fact]
        public void AND_IrrrI_nnnn_n_nnn()  // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            RunTest(
                "AND (QRS),0x00010203,3,2",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5000, initial);
                },
                c =>
                {
                    byte[] expected = [0x03, 0x02, 0x01, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5000, 4));
                });
        }

        [Fact]
        public void AND_IrrrI_r()   // Ok
        {
            RunTest(
                "AND (QRS),B",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5000, [0xFF, 0xFF]);
                    c.CPU.REGS.B = 5;
                },
                c => Assert.Equal([0x05, 0xFF], c.MEMC.RAM.GetMemoryAt(0x5000, 2)));
        }

        [Fact]
        public void AND_IrrrI_rr()  // Ok
        {
            RunTest(
                "AND (QRS),CD",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5000, [0xFF, 0xFF]);
                    c.CPU.REGS.CD = 0x0102;
                },
                c => Assert.Equal([0x02, 0x01], c.MEMC.RAM.GetMemoryAt(0x5000, 2)));
        }

        [Fact]
        public void AND_IrrrI_rrr() // Ok
        {
            RunTest(
                "AND (QRS),DEF",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5000, [0xFF, 0xFF, 0xFF]);
                    c.CPU.REGS.DEF = 0x010203;
                },
                c => Assert.Equal([0x03, 0x02, 0x01], c.MEMC.RAM.GetMemoryAt(0x5000, 3)));
        }

        [Fact]
        public void AND_IrrrI_rrrr()    // Ok
        {
            RunTest(
                "AND (QRS),EFGH",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5000, [0xFF, 0xFF, 0xFF, 0xFF]);
                    c.CPU.REGS.EFGH = 0x02030405;
                },
                c => Assert.Equal([0x05, 0x04, 0x03, 0x02], c.MEMC.RAM.GetMemoryAt(0x5000, 4)));
        }

        [Fact]
        public void AND_IrrrI_InnnI_n_rrr() // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (QRS),(0x7000),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5000, initial);
                    c.LoadMemAt(0x7000, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5000, 4));
                });
        }

        [Fact]
        public void AND_IrrrI_Innn_nnnI_n_rrr() // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (QRS),(0x7000+4),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5000, initial);
                    c.LoadMemAt(0x7004, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5000, 4));
                });
        }

        [Fact]
        public void AND_IrrrI_Innn_rI_n_rrr()   // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (QRS),(0x7000+K),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5000, 4));
                });
        }

        [Fact]
        public void AND_IrrrI_Innn_rrI_n_rrr()  // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (QRS),(0x7000+KL),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5000, 4));
                });
        }

        [Fact]
        public void AND_IrrrI_Innn_rrrI_n_rrr() // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (QRS),(0x7000+KLM),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5000, 4));
                });
        }

        [Fact]
        public void AND_IrrrI_IrrrI_n_rrr() // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (QRS),(DEF),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5000, 4));
                });
        }

        [Fact]
        public void AND_IrrrI_Irrr_nnnI_n_rrr() // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (QRS),(DEF+4),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5000, 4));
                });
        }

        [Fact]
        public void AND_IrrrI_Irrr_rI_n_rrr()   // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (QRS),(DEF+K),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5000, 4));
                });
        }

        [Fact]
        public void AND_IrrrI_Irrr_rrI_n_rrr()  // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (QRS),(DEF+KL),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5000, 4));
                });
        }

        [Fact]
        public void AND_IrrrI_Irrr_rrrI_n_rrr() // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (QRS),(DEF+KLM),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5000, 4));
                });
        }

        [Fact]
        public void AND_IrrrI_fr()  // Ok
        {
            RunTest(
                "AND (QRS),F4",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.SetFloatToRam(0x5000, 1.25f);
                    c.CPU.FREGS.SetRegister(4, 2.5f);
                },
                c => Assert.Equal(0.0f, c.MEMC.GetFloatFromRAM(0x5000)));
        }

        [Fact]
        public void AND_Irrr_nnnI_nnnn_n()  // Ok
        {
            RunTest(
                "AND (QRS+4),0x00010203,3",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5004, [0xFF, 0xFF, 0xFF, 0xFF]);
                },
                c =>
                {
                    byte[] expected = [0x03, 0x02, 0x01, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5004, 4));
                });
        }

        [Fact]
        public void AND_Irrr_nnnI_r()   // Ok
        {
            RunTest(
                "AND (QRS+4),B",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5004, [0xFF, 0xFF]);
                    c.CPU.REGS.B = 5;
                },
                c => Assert.Equal([0x05, 0xFF], c.MEMC.RAM.GetMemoryAt(0x5004, 2)));
        }

        [Fact]
        public void AND_Irrr_nnnI_rr()  // Ok
        {
            RunTest(
                "AND (QRS+4),CD",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5004, [0xFF, 0xFF]);
                    c.CPU.REGS.CD = 0x0102;
                },
                c => Assert.Equal([0x02, 0x01], c.MEMC.RAM.GetMemoryAt(0x5004, 2)));
        }

        [Fact]
        public void AND_Irrr_nnnI_rrr() // Ok
        {
            RunTest(
                "AND (QRS+4),DEF",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5004, [0xFF, 0xFF, 0xFF]);
                    c.CPU.REGS.DEF = 0x010203;
                },
                c => Assert.Equal([0x03, 0x02, 0x01], c.MEMC.RAM.GetMemoryAt(0x5004, 3)));
        }

        [Fact]
        public void AND_Irrr_nnnI_rrrr()    // Ok
        {
            RunTest(
                "AND (QRS+4),EFGH",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5004, [0xFF, 0xFF, 0xFF, 0xFF]);
                    c.CPU.REGS.EFGH = 0x01020304;
                },
                c => Assert.Equal([0x04, 0x03, 0x02, 0x01], c.MEMC.RAM.GetMemoryAt(0x5004, 4)));
        }

        [Fact]
        public void AND_Irrr_nnnI_InnnI_n_rrr() // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (QRS+4),(0x7000),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5004, initial);
                    c.LoadMemAt(0x7000, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5004, 4));
                });
        }

        [Fact]
        public void AND_Irrr_nnnI_Innn_nnnI_n_rrr() // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (QRS+4),(0x7000+4),3,GHI",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5004, initial);
                    c.LoadMemAt(0x7004, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5004, 4));
                });
        }

        [Fact]
        public void AND_Irrr_nnnI_Innn_rI_n_rrr()   // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (QRS+4),(0x7000+K),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5004, 4));
                });
        }

        [Fact]
        public void AND_Irrr_nnnI_Innn_rrI_n_rrr()  // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (QRS+4),(0x7000+KL),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5004, 4));
                });
        }

        [Fact]
        public void AND_Irrr_nnnI_Innn_rrrI_n_rrr() // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (QRS+4),(0x7000+KLM),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5004, 4));
                });
        }

        [Fact]
        public void AND_Irrr_nnnI_IrrrI_n_rrr() // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (QRS+4),(MNO),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5004, 4));
                });
        }

        [Fact]
        public void AND_Irrr_nnnI_Irrr_nnnI_n_rrr() // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (QRS+4),(MNO+4),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5004, 4));
                });
        }

        [Fact]
        public void AND_Irrr_nnnI_Irrr_rI_n_rrr()   // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (QRS+4),(MNO+K),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5004, 4));
                });
        }

        [Fact]
        public void AND_Irrr_nnnI_Irrr_rrI_n_rrr()  // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (QRS+4),(MNO+KL),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5004, 4));
                });
        }

        [Fact]
        public void AND_Irrr_nnnI_Irrr_rrrI_n_rrr() // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (QRS+4),(MNO+ABC),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5004, 4));
                });
        }

        [Fact]
        public void AND_Irrr_nnnI_fr()  // Ok
        {
            RunTest(
                "AND (QRS+4),F4",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.SetFloatToRam(0x5004, 1.25f);
                    c.CPU.FREGS.SetRegister(4, 2.5f);
                },
                c => Assert.Equal(0.0f, c.MEMC.GetFloatFromRAM(0x5004)));
        }

        [Fact]
        public void AND_Irrr_rI_nnnn_n()    // Ok
        {
            RunTest(
                "AND (QRS+X),0x00010203,3",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 5;
                    c.LoadMemAt(0x5005, [0xFF, 0xFF, 0xFF, 0xFF]);
                },
                c =>
                {
                    byte[] expected = [0x03, 0x02, 0x01, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5005, 4));
                });
        }

        [Fact]
        public void AND_Irrr_rI_r() // Ok
        {
            RunTest(
                "AND (QRS+X),B",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 5;
                    c.LoadMemAt(0x5005, [0xFF, 0xFF]);
                    c.CPU.REGS.B = 5;
                },
                c => Assert.Equal([0x05, 0xFF], c.MEMC.RAM.GetMemoryAt(0x5005, 2)));
        }

        [Fact]
        public void AND_Irrr_rI_rr()    // Ok
        {
            RunTest(
                "AND (QRS+X),CD",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 5;
                    c.LoadMemAt(0x5005, [0xFF, 0xFF]);
                    c.CPU.REGS.CD = 0x0102;
                },
                c => Assert.Equal([0x02, 0x01], c.MEMC.RAM.GetMemoryAt(0x5005, 2)));
        }

        [Fact]
        public void AND_Irrr_rI_rrr()   // Ok
        {
            RunTest(
                "AND (QRS+X),DEF",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 5;
                    c.LoadMemAt(0x5005, [0xFF, 0xFF, 0xFF]);
                    c.CPU.REGS.DEF = 0x010203;
                },
                c => Assert.Equal([0x03, 0x02, 0x01], c.MEMC.RAM.GetMemoryAt(0x5005, 3)));
        }

        [Fact]
        public void AND_Irrr_rI_rrrr()  // Ok
        {
            RunTest(
                "AND (QRS+X),EFGH",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 5;
                    c.LoadMemAt(0x5005, [0xFF, 0xFF, 0xFF, 0xFF]);
                    c.CPU.REGS.EFGH = 0x01020304;
                },
                c => Assert.Equal([0x04, 0x03, 0x02, 0x01], c.MEMC.RAM.GetMemoryAt(0x5005, 4)));
        }

        [Fact]
        public void AND_Irrr_rI_InnnI_n_rrr()   // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (QRS+X),(0x7000),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5005, 4));
                });
        }

        [Fact]
        public void AND_Irrr_rI_Innn_nnnI_n_rrr()   // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (QRS+X),(0x7000+4),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5005, 4));
                });
        }

        [Fact]
        public void AND_Irrr_rI_Innn_rI_n_rrr() // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (QRS+X),(0x7000+K),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5005, 4));
                });
        }

        [Fact]
        public void AND_Irrr_rI_Innn_rrI_n_rrr()    // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (QRS+X),(0x7000+KL),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5005, 4));
                });
        }

        [Fact]
        public void AND_Irrr_rI_Innn_rrrI_n_rrr()   // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (QRS+X),(0x7000+KLM),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5005, 4));
                });
        }

        [Fact]
        public void AND_Irrr_rI_IrrrI_n_rrr()   // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (QRS+X),(MNO),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5005, 4));
                });
        }

        [Fact]
        public void AND_Irrr_rI_Irrr_nnnI_n_rrr()   // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (QRS+X),(MNO+4),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5005, 4));
                });
        }

        [Fact]
        public void AND_Irrr_rI_Irrr_rI_n_rrr() // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (QRS+X),(MNO+K),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5005, 4));
                });
        }

        [Fact]
        public void AND_Irrr_rI_Irrr_rrI_n_rrr()    // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (QRS+X),(MNO+KL),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5005, 4));
                });
        }

        [Fact]
        public void AND_Irrr_rI_Irrr_rrrI_n_rrr()   // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (QRS+X),(MNO+ABC),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5005, 4));
                });
        }

        [Fact]
        public void AND_Irrr_rI_fr()    // Ok
        {
            RunTest(
                "AND (QRS+X),F4",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 5;
                    c.MEMC.SetFloatToRam(0x5005, 1.25f);
                    c.CPU.FREGS.SetRegister(4, 2.5f);
                },
                c => Assert.Equal(0.0f, c.MEMC.GetFloatFromRAM(0x5005)));
        }

        [Fact]
        public void AND_Irrr_rrI_nnnn_n()   // Ok
        {
            RunTest(
                "AND (QRS+WX),0x00010203,3",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 7;
                    c.LoadMemAt(0x5007, [0xFF, 0xFF, 0xFF, 0xFF]);
                },
                c =>
                {
                    byte[] expected = [0x03, 0x02, 0x01, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5007, 4));
                });
        }

        [Fact]
        public void AND_Irrr_rrI_r()    // Ok
        {
            RunTest(
                "AND (QRS+WX),B",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 7;
                    c.LoadMemAt(0x5007, [0xFF, 0xFF]);
                    c.CPU.REGS.B = 5;
                },
                c => Assert.Equal([0x05, 0xFF], c.MEMC.RAM.GetMemoryAt(0x5007, 2)));
        }

        [Fact]
        public void AND_Irrr_rrI_rr()   // Ok
        {
            RunTest(
                "AND (QRS+WX),CD",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 7;
                    c.LoadMemAt(0x5007, [0xFF, 0xFF]);
                    c.CPU.REGS.CD = 0x0102;
                },
                c => Assert.Equal([0x02, 0x01], c.MEMC.RAM.GetMemoryAt(0x5007, 2)));
        }

        [Fact]
        public void AND_Irrr_rrI_rrr()  // Ok
        {
            RunTest(
                "AND (QRS+WX),DEF",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 7;
                    c.LoadMemAt(0x5007, [0xFF, 0xFF, 0xFF]);
                    c.CPU.REGS.DEF = 0x010203;
                },
                c => Assert.Equal([0x03, 0x02, 0x01], c.MEMC.RAM.GetMemoryAt(0x5007, 3)));
        }

        [Fact]
        public void AND_Irrr_rrI_rrrr() // Ok
        {
            RunTest(
                "AND (QRS+WX),EFGH",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 7;
                    c.LoadMemAt(0x5007, [0xFF, 0xFF, 0xFF, 0xFF]);
                    c.CPU.REGS.EFGH = 0x01020304;
                },
                c => Assert.Equal([0x04, 0x03, 0x02, 0x01], c.MEMC.RAM.GetMemoryAt(0x5007, 4)));
        }

        [Fact]
        public void AND_Irrr_rrI_InnnI_n_rrr()  // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (QRS+WX),(0x7000),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5007, 4));
                });
        }

        [Fact]
        public void AND_Irrr_rrI_Innn_nnnI_n_rrr()  // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (QRS+WX),(0x7000+4),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5007, 4));
                });
        }

        [Fact]
        public void AND_Irrr_rrI_Innn_rI_n_rrr()    // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (QRS+WX),(0x7000+K),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5007, 4));
                });
        }

        [Fact]
        public void AND_Irrr_rrI_Innn_rrI_n_rrr()   // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (QRS+WX),(0x7000+KL),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5007, 4));
                });
        }

        [Fact]
        public void AND_Irrr_rrI_Innn_rrrI_n_rrr()  // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (QRS+WX),(0x7000+KLM),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5007, 4));
                });
        }

        [Fact]
        public void AND_Irrr_rrI_IrrrI_n_rrr()  // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (QRS+WX),(MNO),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5007, 4));
                });
        }

        [Fact]
        public void AND_Irrr_rrI_Irrr_nnnI_n_rrr()  // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (QRS+WX),(MNO+4),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5007, 4));
                });
        }

        [Fact]
        public void AND_Irrr_rrI_Irrr_rI_n_rrr()    // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (QRS+WX),(MNO+K),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5007, 4));
                });
        }

        [Fact]
        public void AND_Irrr_rrI_Irrr_rrI_n_rrr()   // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (QRS+WX),(MNO+KL),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5007, 4));
                });
        }

        [Fact]
        public void AND_Irrr_rrI_Irrr_rrrI_n_rrr()  // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (QRS+WX),(MNO+ABC),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5007, 4));
                });
        }

        [Fact]
        public void AND_Irrr_rrI_fr()   // Ok
        {
            RunTest(
                "AND (QRS+WX),F4",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 7;
                    c.MEMC.SetFloatToRam(0x5007, 1.25f);
                    c.CPU.FREGS.SetRegister(4, 2.5f);
                },
                c => Assert.Equal(0.0f, c.MEMC.GetFloatFromRAM(0x5007)));
        }

        [Fact]
        public void AND_Irrr_rrrI_nnnn_n()  // Ok
        {
            RunTest(
                "AND (QRS+TUV),0x00010203,3",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.TUV = 9;
                    c.LoadMemAt(0x5009, [0xFF, 0xFF, 0xFF, 0xFF]);
                },
                c =>
                {
                    byte[] expected = [0x03, 0x02, 0x01, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5009, 4));
                });
        }

        [Fact]
        public void AND_Irrr_rrrI_r()   // Ok
        {
            RunTest(
                "AND (QRS+TUV),B",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.TUV = 9;
                    c.LoadMemAt(0x5009, [0xFF, 0xFF]);
                    c.CPU.REGS.B = 5;
                },
                c => Assert.Equal([0x05, 0xFF], c.MEMC.RAM.GetMemoryAt(0x5009, 2)));
        }

        [Fact]
        public void AND_Irrr_rrrI_rr()  // Ok
        {
            RunTest(
                "AND (QRS+TUV),CD",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.TUV = 9;
                    c.LoadMemAt(0x5009, [0xFF, 0xFF]);
                    c.CPU.REGS.CD = 0x0102;
                },
                c => Assert.Equal([0x02, 0x01], c.MEMC.RAM.GetMemoryAt(0x5009, 2)));
        }

        [Fact]
        public void AND_Irrr_rrrI_rrr() // Ok
        {
            RunTest(
                "AND (QRS+TUV),DEF",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.TUV = 9;
                    c.LoadMemAt(0x5009, [0xFF, 0xFF, 0xFF]);
                    c.CPU.REGS.DEF = 0x010203;
                },
                c => Assert.Equal([0x03, 0x02, 0x01], c.MEMC.RAM.GetMemoryAt(0x5009, 3)));
        }

        [Fact]
        public void AND_Irrr_rrrI_rrrr()    // Ok
        {
            RunTest(
                "AND (QRS+TUV),EFGH",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.TUV = 9;
                    c.LoadMemAt(0x5009, [0xFF, 0xFF, 0xFF, 0xFF]);
                    c.CPU.REGS.EFGH = 0x01020304;
                },
                c => Assert.Equal([0x04, 0x03, 0x02, 0x01], c.MEMC.RAM.GetMemoryAt(0x5009, 4)));
        }

        [Fact]
        public void AND_Irrr_rrrI_InnnI_n_rrr() // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (QRS+TUV),(0x7000),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5009, 4));
                });
        }

        [Fact]
        public void AND_Irrr_rrrI_Innn_nnnI_n_rrr() // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (QRS+TUV),(0x7000+4),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5009, 4));
                });
        }

        [Fact]
        public void AND_Irrr_rrrI_Innn_rI_n_rrr()   // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (QRS+TUV),(0x7000+K),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5009, 4));
                });
        }

        [Fact]
        public void AND_Irrr_rrrI_Innn_rrI_n_rrr()  // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (QRS+TUV),(0x7000+KL),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5009, 4));
                });
        }

        [Fact]
        public void AND_Irrr_rrrI_Innn_rrrI_n_rrr() // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (QRS+TUV),(0x7000+KLM),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5009, 4));
                });
        }

        [Fact]
        public void AND_Irrr_rrrI_IrrrI_n_rrr() // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (QRS+TUV),(MNO),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5009, 4));
                });
        }

        [Fact]
        public void AND_Irrr_rrrI_Irrr_nnnI_n_rrr() // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (QRS+TUV),(MNO+4),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5009, 4));
                });
        }

        [Fact]
        public void AND_Irrr_rrrI_Irrr_rI_n_rrr()   // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (QRS+TUV),(MNO+K),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5009, 4));
                });
        }

        [Fact]
        public void AND_Irrr_rrrI_Irrr_rrI_n_rrr()  // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (QRS+TUV),(MNO+KL),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5009, 4));
                });
        }

        [Fact]
        public void AND_Irrr_rrrI_Irrr_rrrI_n_rrr() // Ok
        {
            byte[] initial = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] value = [0x0F, 0x0F, 0x0F, 0x0F];
            RunTest(
                "AND (QRS+TUV),(MNO+ABC),3,GHI",
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
                    byte[] expected = [0x0F, 0x0F, 0x0F, 0xFF];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x5009, 4));
                });
        }

        [Fact]
        public void AND_Irrr_rrrI_fr()  // Ok
        {
            RunTest(
                "AND (QRS+TUV),F4",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.TUV = 9;
                    c.MEMC.SetFloatToRam(0x5009, 1.25f);
                    c.CPU.FREGS.SetRegister(4, 2.5f);
                },
                c => Assert.Equal(0.0f, c.MEMC.GetFloatFromRAM(0x5009)));
        }

        #endregion

        #region AND fr,* tests

        [Fact]
        public void AND_fr_nnnn()   // Ok
        {
            RunTest(
                "AND F0,0x3F800000",
                c => c.CPU.FREGS.SetRegister(0, 10.5f),
                c => Assert.Equal(1.0f, c.CPU.FREGS.GetRegister(0), precision: 5));
        }

        [Fact]
        public void AND_fr_r()  // Ok
        {
            RunTest(
                "AND F0,J",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 10.5f);
                    c.CPU.REGS.J = 3;
                },
                c => Assert.Equal(2.0f, c.CPU.FREGS.GetRegister(0), precision: 5));
        }

        [Fact]
        public void AND_fr_rr() // Ok
        {
            RunTest(
                "AND F0,JK",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 10.5f);
                    c.CPU.REGS.JK = 1000;
                },
                c => Assert.Equal(0.0f, c.CPU.FREGS.GetRegister(0), precision: 5));
        }

        [Fact]
        public void AND_fr_rrr()    // Ok
        {
            RunTest(
                "AND F0,JKL",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 10.5f);
                    c.CPU.REGS.JKL = 50000;
                },
                c => Assert.Equal(0.0f, c.CPU.FREGS.GetRegister(0), precision: 5));
        }

        [Fact]
        public void AND_fr_rrrr()   // Ok
        {
            RunTest(
                "AND F0,JKLM",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 10.5f);
                    c.CPU.REGS.JKLM = 1000000;
                },
                c => Assert.Equal(0.0f, c.CPU.FREGS.GetRegister(0), precision: 5));
        }

        [Fact]
        public void AND_fr_InnnI()  // Ok
        {
            RunTest(
                "AND F0,(0x4000)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                    c.MEMC.SetFloatToRam(0x4000, 0.75f);
                },
                c => Assert.Equal(0.0f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void AND_fr_Innn_nnnI()  // Ok
        {
            RunTest(
                "AND F0,(0x4000+4)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                    c.MEMC.SetFloatToRam(0x4004, 0.75f);
                },
                c => Assert.Equal(0.0f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void AND_fr_Innn_rI()    // Ok
        {
            RunTest(
                "AND F0,(0x4000+X)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                    c.CPU.REGS.X = 3;
                    c.MEMC.SetFloatToRam(0x4003, 0.75f);
                },
                c => Assert.Equal(0.0f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void AND_fr_Innn_rrI()   // Ok
        {
            RunTest(
                "AND F0,(0x4000+WX)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                    c.CPU.REGS.WX = 5;
                    c.MEMC.SetFloatToRam(0x4005, 0.75f);
                },
                c => Assert.Equal(0.0f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void AND_fr_Innn_rrrI()  // Ok
        {
            RunTest(
                "AND F0,(0x4000+VWX)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.SetFloatToRam(0x4007, 0.75f);
                },
                c => Assert.Equal(0.0f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void AND_fr_IrrrI()  // Ok
        {
            RunTest(
                "AND F0,(QRS)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.SetFloatToRam(0x5000, 0.75f);
                },
                c => Assert.Equal(0.0f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void AND_fr_Irrr_nnnI()  // Ok
        {
            RunTest(
                "AND F0,(QRS+4)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.SetFloatToRam(0x5004, 0.75f);
                },
                c => Assert.Equal(0.0f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void AND_fr_Irrr_rI()    // Ok
        {
            RunTest(
                "AND F0,(QRS+X)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.SetFloatToRam(0x5003, 0.75f);
                },
                c => Assert.Equal(0.0f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void AND_fr_Irrr_rrI()   // Ok
        {
            RunTest(
                "AND F0,(QRS+WX)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.SetFloatToRam(0x5005, 0.75f);
                },
                c => Assert.Equal(0.0f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void AND_fr_Irrr_rrrI()  // Ok
        {
            RunTest(
                "AND F0,(QRS+VWX)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.SetFloatToRam(0x5007, 0.75f);
                },
                c => Assert.Equal(0.0f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void AND_fr_fr() // Ok
        {
            RunTest(
                "AND F0,F1",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 10.5f);
                    c.CPU.FREGS.SetRegister(1, 2.75f);
                },
                c => Assert.Equal(2.0f, c.CPU.FREGS.GetRegister(0), precision: 5));
        }

        #endregion
    }
}
