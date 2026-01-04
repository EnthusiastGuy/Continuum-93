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

        private static byte[] ComputeExpectedSub(byte[] initial, byte[] sub, int count, int repeat)
        {
            byte[] working = new byte[count];
            Array.Copy(initial, working, count);
            
            for (int r = 0; r < repeat; r++)
            {
                byte borrow = 0;
                for (int i = count - 1; i >= 0; i--)
                {
                    int diff = working[i] - sub[i] - borrow;
                    if (diff < 0)
                    {
                        diff += 256;
                        borrow = 1;
                    }
                    else
                    {
                        borrow = 0;
                    }
                    working[i] = (byte)diff;
                }
            }
            
            return working;
        }

        #endregion

        #region SUB r,* tests (14 tests)

        [Fact]
        public void SUB_r_n()
        {
            RunTest(
                "SUB A,5",
                c => c.CPU.REGS.A = 50,
                c => Assert.Equal((byte)45, c.CPU.REGS.A));
        }

        [Fact]
        public void SUB_r_nnn()
        {
            RunTest(
                "SUB A,30",
                c => c.CPU.REGS.A = 50,
                c => Assert.Equal((byte)20, c.CPU.REGS.A));
        }

        [Fact]
        public void SUB_r_r()
        {
            RunTest(
                "SUB A,B",
                c =>
                {
                    c.CPU.REGS.A = 50;
                    c.CPU.REGS.B = 3;
                },
                c => Assert.Equal((byte)47, c.CPU.REGS.A));
        }

        [Fact]
        public void SUB_r_InnnI()
        {
            RunTest(
                "SUB A,(0x4000)",
                c =>
                {
                    c.CPU.REGS.A = 50;
                    c.MEMC.Set8bitToRAM(0x4000, 0x0A);
                },
                c => Assert.Equal((byte)40, c.CPU.REGS.A));
        }

        [Fact]
        public void SUB_r_Innn_nnnI()
        {
            RunTest(
                "SUB A,(0x4000+4)",
                c =>
                {
                    c.CPU.REGS.A = 50;
                    c.MEMC.Set8bitToRAM(0x4004, 0x0A);
                },
                c => Assert.Equal((byte)40, c.CPU.REGS.A));
        }

        [Fact]
        public void SUB_r_Innn_rI()
        {
            RunTest(
                "SUB A,(0x4000+X)",
                c =>
                {
                    c.CPU.REGS.A = 50;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set8bitToRAM(0x4003, 0x0A);
                },
                c => Assert.Equal((byte)40, c.CPU.REGS.A));
        }

        [Fact]
        public void SUB_r_Innn_rrI()
        {
            RunTest(
                "SUB A,(0x4000+WX)",
                c =>
                {
                    c.CPU.REGS.A = 50;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x4005, 0x0A);
                },
                c => Assert.Equal((byte)40, c.CPU.REGS.A));
        }

        [Fact]
        public void SUB_r_Innn_rrrI()
        {
            RunTest(
                "SUB A,(0x4000+VWX)",
                c =>
                {
                    c.CPU.REGS.A = 50;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x4007, 0x0A);
                },
                c => Assert.Equal((byte)40, c.CPU.REGS.A));
        }

        [Fact]
        public void SUB_r_IrrrI()
        {
            RunTest(
                "SUB A,(QRS)",
                c =>
                {
                    c.CPU.REGS.A = 50;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x5000, 0x0A);
                },
                c => Assert.Equal((byte)40, c.CPU.REGS.A));
        }

        [Fact]
        public void SUB_r_Irrr_nnnI()
        {
            RunTest(
                "SUB A,(QRS+4)",
                c =>
                {
                    c.CPU.REGS.A = 50;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x5004, 0x0A);
                },
                c => Assert.Equal((byte)40, c.CPU.REGS.A));
        }

        [Fact]
        public void SUB_r_Irrr_rI()
        {
            RunTest(
                "SUB A,(QRS+X)",
                c =>
                {
                    c.CPU.REGS.A = 50;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set8bitToRAM(0x5003, 0x0A);
                },
                c => Assert.Equal((byte)40, c.CPU.REGS.A));
        }

        [Fact]
        public void SUB_r_Irrr_rrI()
        {
            RunTest(
                "SUB A,(QRS+WX)",
                c =>
                {
                    c.CPU.REGS.A = 50;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x5005, 0x0A);
                },
                c => Assert.Equal((byte)40, c.CPU.REGS.A));
        }

        [Fact]
        public void SUB_r_Irrr_rrrI()
        {
            RunTest(
                "SUB A,(QRS+VWX)",
                c =>
                {
                    c.CPU.REGS.A = 50;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x5007, 0x0A);
                },
                c => Assert.Equal((byte)40, c.CPU.REGS.A));
        }

        [Fact]
        public void SUB_r_fr()
        {
            RunTest(
                "SUB A,F0",
                c =>
                {
                    c.CPU.REGS.A = 50;
                    c.CPU.FREGS.SetRegister(0, 1.5f);
                },
                c => Assert.Equal((byte)48, c.CPU.REGS.A));
        }

        #endregion

        #region SUB rr,* tests (15 tests)

        [Fact]
        public void SUB_rr_nn()
        {
            RunTest(
                "SUB AB,0x1234",
                c => c.CPU.REGS.AB = 0x5000,
                c => Assert.Equal((ushort)0x3DCC, c.CPU.REGS.AB));
        }

        [Fact]
        public void SUB_rr_nnn()
        {
            RunTest(
                "SUB AB,0x0020",
                c => c.CPU.REGS.AB = 0x5000,
                c => Assert.Equal((ushort)0x4FE0, c.CPU.REGS.AB));
        }

        [Fact]
        public void SUB_rr_r()
        {
            RunTest(
                "SUB AB,E",
                c =>
                {
                    c.CPU.REGS.AB = 0x5000;
                    c.CPU.REGS.E = 4;
                },
                c => Assert.Equal((ushort)0x4FFC, c.CPU.REGS.AB));
        }

        [Fact]
        public void SUB_rr_rr()
        {
            RunTest(
                "SUB AB,CD",
                c =>
                {
                    c.CPU.REGS.AB = 0x5000;
                    c.CPU.REGS.CD = 0x00FF;
                },
                c => Assert.Equal((ushort)0x4F01, c.CPU.REGS.AB));
        }

        [Fact]
        public void SUB_rr_InnnI()
        {
            RunTest(
                "SUB AB,(0x4000)",
                c =>
                {
                    c.CPU.REGS.AB = 0x5000;
                    c.MEMC.Set16bitToRAM(0x4000, 0x0100);
                },
                c => Assert.Equal((ushort)0x4F00, c.CPU.REGS.AB));
        }

        [Fact]
        public void SUB_rr_Innn_nnnI()
        {
            RunTest(
                "SUB AB,(0x4000+4)",
                c =>
                {
                    c.CPU.REGS.AB = 0x5000;
                    c.MEMC.Set16bitToRAM(0x4004, 0x0100);
                },
                c => Assert.Equal((ushort)0x4F00, c.CPU.REGS.AB));
        }

        [Fact]
        public void SUB_rr_Innn_rI()
        {
            RunTest(
                "SUB AB,(0x4000+X)",
                c =>
                {
                    c.CPU.REGS.AB = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set16bitToRAM(0x4003, 0x0100);
                },
                c => Assert.Equal((ushort)0x4F00, c.CPU.REGS.AB));
        }

        [Fact]
        public void SUB_rr_Innn_rrI()
        {
            RunTest(
                "SUB AB,(0x4000+WX)",
                c =>
                {
                    c.CPU.REGS.AB = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set16bitToRAM(0x4005, 0x0100);
                },
                c => Assert.Equal((ushort)0x4F00, c.CPU.REGS.AB));
        }

        [Fact]
        public void SUB_rr_Innn_rrrI()
        {
            RunTest(
                "SUB AB,(0x4000+VWX)",
                c =>
                {
                    c.CPU.REGS.AB = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set16bitToRAM(0x4007, 0x0100);
                },
                c => Assert.Equal((ushort)0x4F00, c.CPU.REGS.AB));
        }

        [Fact]
        public void SUB_rr_IrrrI()
        {
            RunTest(
                "SUB AB,(QRS)",
                c =>
                {
                    c.CPU.REGS.AB = 0x5000;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set16bitToRAM(0x5000, 0x0100);
                },
                c => Assert.Equal((ushort)0x4F00, c.CPU.REGS.AB));
        }

        [Fact]
        public void SUB_rr_Irrr_nnnI()
        {
            RunTest(
                "SUB AB,(QRS+4)",
                c =>
                {
                    c.CPU.REGS.AB = 0x5000;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set16bitToRAM(0x5004, 0x0100);
                },
                c => Assert.Equal((ushort)0x4F00, c.CPU.REGS.AB));
        }

        [Fact]
        public void SUB_rr_Irrr_rI()
        {
            RunTest(
                "SUB AB,(QRS+X)",
                c =>
                {
                    c.CPU.REGS.AB = 0x5000;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set16bitToRAM(0x5003, 0x0100);
                },
                c => Assert.Equal((ushort)0x4F00, c.CPU.REGS.AB));
        }

        [Fact]
        public void SUB_rr_Irrr_rrI()
        {
            RunTest(
                "SUB AB,(QRS+WX)",
                c =>
                {
                    c.CPU.REGS.AB = 0x5000;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set16bitToRAM(0x5005, 0x0100);
                },
                c => Assert.Equal((ushort)0x4F00, c.CPU.REGS.AB));
        }

        [Fact]
        public void SUB_rr_Irrr_rrrI()
        {
            RunTest(
                "SUB AB,(QRS+VWX)",
                c =>
                {
                    c.CPU.REGS.AB = 0x5000;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set16bitToRAM(0x5007, 0x0100);
                },
                c => Assert.Equal((ushort)0x4F00, c.CPU.REGS.AB));
        }

        [Fact]
        public void SUB_rr_fr()
        {
            RunTest(
                "SUB AB,F1",
                c =>
                {
                    c.CPU.REGS.AB = 0x5000;
                    c.CPU.FREGS.SetRegister(1, 5.6f);
                },
                c => Assert.Equal((ushort)0x4FFA, c.CPU.REGS.AB));
        }

        #endregion

        #region SUB rrr,* tests (14 tests)

        [Fact]
        public void SUB_rrr_nnn()
        {
            RunTest(
                "SUB ABC,0x000102",
                c => c.CPU.REGS.ABC = 0x050000,
                c => Assert.Equal((uint)0x04FEFE, c.CPU.REGS.ABC));
        }

        [Fact]
        public void SUB_rrr_r()
        {
            RunTest(
                "SUB ABC,E",
                c =>
                {
                    c.CPU.REGS.ABC = 0x050000;
                    c.CPU.REGS.E = 7;
                },
                c => Assert.Equal((uint)0x04FFF9, c.CPU.REGS.ABC));
        }

        [Fact]
        public void SUB_rrr_rr()
        {
            RunTest(
                "SUB ABC,EF",
                c =>
                {
                    c.CPU.REGS.ABC = 0x050000;
                    c.CPU.REGS.EF = 0x0102;
                },
                c => Assert.Equal((uint)0x04FEFE, c.CPU.REGS.ABC));
        }

        [Fact]
        public void SUB_rrr_rrr()
        {
            RunTest(
                "SUB ABC,DEF",
                c =>
                {
                    c.CPU.REGS.ABC = 0x050000;
                    c.CPU.REGS.DEF = 0x00000F;
                },
                c => Assert.Equal((uint)0x04FFF1, c.CPU.REGS.ABC));
        }

        [Fact]
        public void SUB_rrr_InnnI()
        {
            RunTest(
                "SUB ABC,(0x4000)",
                c =>
                {
                    c.CPU.REGS.ABC = 0x050000;
                    c.MEMC.Set24bitToRAM(0x4000, 0x001000);
                },
                c => Assert.Equal((uint)0x040000, c.CPU.REGS.ABC));
        }

        [Fact]
        public void SUB_rrr_Innn_nnnI()
        {
            RunTest(
                "SUB ABC,(0x4000+4)",
                c =>
                {
                    c.CPU.REGS.ABC = 0x050000;
                    c.MEMC.Set24bitToRAM(0x4004, 0x001000);
                },
                c => Assert.Equal((uint)0x040000, c.CPU.REGS.ABC));
        }

        [Fact]
        public void SUB_rrr_Innn_rI()
        {
            RunTest(
                "SUB ABC,(0x4000+X)",
                c =>
                {
                    c.CPU.REGS.ABC = 0x050000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set24bitToRAM(0x4003, 0x001000);
                },
                c => Assert.Equal((uint)0x040000, c.CPU.REGS.ABC));
        }

        [Fact]
        public void SUB_rrr_Innn_rrI()
        {
            RunTest(
                "SUB ABC,(0x4000+WX)",
                c =>
                {
                    c.CPU.REGS.ABC = 0x050000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set24bitToRAM(0x4005, 0x001000);
                },
                c => Assert.Equal((uint)0x040000, c.CPU.REGS.ABC));
        }

        [Fact]
        public void SUB_rrr_Innn_rrrI()
        {
            RunTest(
                "SUB ABC,(0x4000+VWX)",
                c =>
                {
                    c.CPU.REGS.ABC = 0x050000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set24bitToRAM(0x4007, 0x001000);
                },
                c => Assert.Equal((uint)0x040000, c.CPU.REGS.ABC));
        }

        [Fact]
        public void SUB_rrr_IrrrI()
        {
            RunTest(
                "SUB ABC,(QRS)",
                c =>
                {
                    c.CPU.REGS.ABC = 0x050000;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set24bitToRAM(0x5000, 0x001000);
                },
                c => Assert.Equal((uint)0x040000, c.CPU.REGS.ABC));
        }

        [Fact]
        public void SUB_rrr_Irrr_nnnI()
        {
            RunTest(
                "SUB ABC,(QRS+4)",
                c =>
                {
                    c.CPU.REGS.ABC = 0x050000;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set24bitToRAM(0x5004, 0x001000);
                },
                c => Assert.Equal((uint)0x040000, c.CPU.REGS.ABC));
        }

        [Fact]
        public void SUB_rrr_Irrr_rI()
        {
            RunTest(
                "SUB ABC,(QRS+X)",
                c =>
                {
                    c.CPU.REGS.ABC = 0x050000;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set24bitToRAM(0x5003, 0x001000);
                },
                c => Assert.Equal((uint)0x040000, c.CPU.REGS.ABC));
        }

        [Fact]
        public void SUB_rrr_Irrr_rrI()
        {
            RunTest(
                "SUB ABC,(QRS+WX)",
                c =>
                {
                    c.CPU.REGS.ABC = 0x050000;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set24bitToRAM(0x5005, 0x001000);
                },
                c => Assert.Equal((uint)0x040000, c.CPU.REGS.ABC));
        }

        [Fact]
        public void SUB_rrr_Irrr_rrrI()
        {
            RunTest(
                "SUB ABC,(QRS+VWX)",
                c =>
                {
                    c.CPU.REGS.ABC = 0x050000;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set24bitToRAM(0x5007, 0x001000);
                },
                c => Assert.Equal((uint)0x040000, c.CPU.REGS.ABC));
        }

        [Fact]
        public void SUB_rrr_fr()
        {
            RunTest(
                "SUB ABC,F2",
                c =>
                {
                    c.CPU.REGS.ABC = 0x050000;
                    c.CPU.FREGS.SetRegister(2, 12.5f);
                },
                c => Assert.Equal((uint)0x04FFF3, c.CPU.REGS.ABC));
        }

        #endregion

        #region SUB rrrr,* tests (16 tests)

        [Fact]
        public void SUB_rrrr_nnnn()
        {
            RunTest(
                "SUB ABCD,0x00010002",
                c => c.CPU.REGS.ABCD = 0x50000000,
                c => Assert.Equal((uint)0x4FFFFFFE, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void SUB_rrrr_r()
        {
            RunTest(
                "SUB ABCD,E",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x50000000;
                    c.CPU.REGS.E = 9;
                },
                c => Assert.Equal((uint)0x4FFFFFF7, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void SUB_rrrr_rr()
        {
            RunTest(
                "SUB ABCD,EF",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x50000000;
                    c.CPU.REGS.EF = 0x00FF;
                },
                c => Assert.Equal((uint)0x4FFFFF01, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void SUB_rrrr_rrr()
        {
            RunTest(
                "SUB ABCD,DEF",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x50000000;
                    c.CPU.REGS.DEF = 0x0000AA;
                },
                c => Assert.Equal((uint)0x4FFFFF56, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void SUB_rrrr_rrrr()
        {
            RunTest(
                "SUB ABCD,EFGH",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x50000000;
                    c.CPU.REGS.EFGH = 0x00000010;
                },
                c => Assert.Equal((uint)0x4FFFFFF0, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void SUB_rrrr_InnnI()
        {
            RunTest(
                "SUB ABCD,(0x4000)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x50000000;
                    c.MEMC.Set32bitToRAM(0x4000, 0x00000100);
                },
                c => Assert.Equal((uint)0x4FFFFF00, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void SUB_rrrr_Innn_nnnI()
        {
            RunTest(
                "SUB ABCD,(0x4000+4)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x50000000;
                    c.MEMC.Set32bitToRAM(0x4004, 0x00000100);
                },
                c => Assert.Equal((uint)0x4FFFFF00, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void SUB_rrrr_Innn_rI()
        {
            RunTest(
                "SUB ABCD,(0x4000+X)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x50000000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set32bitToRAM(0x4003, 0x00000100);
                },
                c => Assert.Equal((uint)0x4FFFFF00, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void SUB_rrrr_Innn_rrI()
        {
            RunTest(
                "SUB ABCD,(0x4000+WX)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x50000000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set32bitToRAM(0x4005, 0x00000100);
                },
                c => Assert.Equal((uint)0x4FFFFF00, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void SUB_rrrr_Innn_rrrI()
        {
            RunTest(
                "SUB ABCD,(0x4000+VWX)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x50000000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set32bitToRAM(0x4007, 0x00000100);
                },
                c => Assert.Equal((uint)0x4FFFFF00, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void SUB_rrrr_IrrrI()
        {
            RunTest(
                "SUB ABCD,(QRS)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x50000000;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set32bitToRAM(0x5000, 0x00000100);
                },
                c => Assert.Equal((uint)0x4FFFFF00, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void SUB_rrrr_Irrr_nnnI()
        {
            RunTest(
                "SUB ABCD,(QRS+4)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x50000000;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set32bitToRAM(0x5004, 0x00000100);
                },
                c => Assert.Equal((uint)0x4FFFFF00, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void SUB_rrrr_Irrr_rI()
        {
            RunTest(
                "SUB ABCD,(QRS+X)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x50000000;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set32bitToRAM(0x5003, 0x00000100);
                },
                c => Assert.Equal((uint)0x4FFFFF00, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void SUB_rrrr_Irrr_rrI()
        {
            RunTest(
                "SUB ABCD,(QRS+WX)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x50000000;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set32bitToRAM(0x5005, 0x00000100);
                },
                c => Assert.Equal((uint)0x4FFFFF00, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void SUB_rrrr_Irrr_rrrI()
        {
            RunTest(
                "SUB ABCD,(QRS+VWX)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x50000000;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set32bitToRAM(0x5007, 0x00000100);
                },
                c => Assert.Equal((uint)0x4FFFFF00, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void SUB_rrrr_fr()
        {
            RunTest(
                "SUB ABCD,F3",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x50000000;
                    c.CPU.FREGS.SetRegister(3, 42.8f);
                },
                c => Assert.Equal((uint)0x4FFFFFD5, c.CPU.REGS.ABCD));
        }

        #endregion

        #region SUB (nnn),* tests - immediate and register (24 tests)

        [Fact]
        public void SUB_InnnI_nnnn_n()
        {
            RunTest(
                "SUB (0x4000),0x00010203,2",
                c => c.LoadMemAt(0x4000, new byte[] { 10, 20, 30 }),
                c => Assert.Equal(new byte[] { 10, 18 }, c.MEMC.RAM.GetMemoryAt(0x4000, 2)));
        }

        [Fact]
        public void SUB_InnnI_nnnn_n_repeat()
        {
            RunTest(
                "SUB (0x4000),0x00010203,3,2",
                c => c.LoadMemAt(0x4000, new byte[] { 10, 20, 30 }),
                c => Assert.Equal(new byte[] { 10, 16, 24 }, c.MEMC.RAM.GetMemoryAt(0x4000, 3)));
        }

        [Fact]
        public void SUB_InnnI_r()
        {
            RunTest(
                "SUB (0x4000),B",
                c =>
                {
                    c.LoadMemAt(0x4000, new byte[] { 10, 20 });
                    c.CPU.REGS.B = 5;
                },
                c => Assert.Equal((byte)5, c.MEMC.Get8bitFromRAM(0x4000)));
        }

        [Fact]
        public void SUB_InnnI_rr()
        {
            RunTest(
                "SUB (0x4000),CD",
                c =>
                {
                    c.LoadMemAt(0x4000, new byte[] { 10, 10 });
                    c.CPU.REGS.CD = 0x0102;
                },
                c => Assert.Equal((ushort)0x0908, c.MEMC.Get16bitFromRAM(0x4000)));
        }

        [Fact]
        public void SUB_InnnI_rrr()
        {
            RunTest(
                "SUB (0x4000),DEF",
                c =>
                {
                    c.LoadMemAt(0x4000, new byte[] { 20, 30, 40 });
                    c.CPU.REGS.DEF = 0x000203;
                },
                c => Assert.Equal((uint)0x141C25, c.MEMC.Get24bitFromRAM(0x4000)));
        }

        [Fact]
        public void SUB_InnnI_rrrr()
        {
            RunTest(
                "SUB (0x4000),EFGH",
                c =>
                {
                    c.LoadMemAt(0x4000, new byte[] { 10, 10, 10, 10 });
                    c.CPU.REGS.EFGH = 0x00030405;
                },
                c => Assert.Equal((uint)0x0A070605, c.MEMC.Get32bitFromRAM(0x4000)));
        }

        [Fact]
        public void SUB_InnnI_InnnI_n_rrr()
        {
            byte[] initial = new byte[] { 20, 30, 40, 50 };
            byte[] value = new byte[] { 1, 2, 3, 4 };
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
                    var expected = ComputeExpectedSub(initial, value, 3, 2);
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4000, 3));
                });
        }

        [Fact]
        public void SUB_InnnI_fr()
        {
            RunTest(
                "SUB (0x4000),F4",
                c =>
                {
                    c.MEMC.SetFloatToRam(0x4000, 5.25f);
                    c.CPU.FREGS.SetRegister(4, 2.5f);
                },
                c => Assert.Equal(2.75f, c.MEMC.GetFloatFromRAM(0x4000)));
        }

        [Fact]
        public void SUB_Innn_nnnI_nnnn_n()
        {
            RunTest(
                "SUB (0x4000+4),0x00010203,2",
                c => c.LoadMemAt(0x4004, new byte[] { 10, 20, 30 }),
                c => Assert.Equal(new byte[] { 10, 18 }, c.MEMC.RAM.GetMemoryAt(0x4004, 2)));
        }

        [Fact]
        public void SUB_Innn_nnnI_r()
        {
            RunTest(
                "SUB (0x4000+4),B",
                c =>
                {
                    c.LoadMemAt(0x4004, new byte[] { 10, 20 });
                    c.CPU.REGS.B = 5;
                },
                c => Assert.Equal((byte)5, c.MEMC.Get8bitFromRAM(0x4004)));
        }

        [Fact]
        public void SUB_Innn_nnnI_rr()
        {
            RunTest(
                "SUB (0x4000+4),CD",
                c =>
                {
                    c.LoadMemAt(0x4004, new byte[] { 10, 10 });
                    c.CPU.REGS.CD = 0x0102;
                },
                c => Assert.Equal((ushort)0x0908, c.MEMC.Get16bitFromRAM(0x4004)));
        }

        [Fact]
        public void SUB_Innn_nnnI_rrr()
        {
            RunTest(
                "SUB (0x4000+4),DEF",
                c =>
                {
                    c.LoadMemAt(0x4004, new byte[] { 20, 30, 40 });
                    c.CPU.REGS.DEF = 0x000203;
                },
                c => Assert.Equal((uint)0x001C25, c.MEMC.Get24bitFromRAM(0x4004)));
        }

        [Fact]
        public void SUB_Innn_nnnI_rrrr()
        {
            RunTest(
                "SUB (0x4000+4),EFGH",
                c =>
                {
                    c.LoadMemAt(0x4004, new byte[] { 10, 10, 10, 10 });
                    c.CPU.REGS.EFGH = 0x00030405;
                },
                c => Assert.Equal((uint)0x00070605, c.MEMC.Get32bitFromRAM(0x4004)));
        }

        [Fact]
        public void SUB_Innn_nnnI_InnnI_n_rrr()
        {
            byte[] initial = new byte[] { 20, 30, 40, 50 };
            byte[] value = new byte[] { 1, 2, 3, 4 };
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
                    var expected = ComputeExpectedSub(initial, value, 3, 2);
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4004, 3));
                });
        }

        [Fact]
        public void SUB_Innn_nnnI_fr()
        {
            RunTest(
                "SUB (0x4000+4),F4",
                c =>
                {
                    c.MEMC.SetFloatToRam(0x4004, 5.25f);
                    c.CPU.FREGS.SetRegister(4, 2.5f);
                },
                c => Assert.Equal(2.75f, c.MEMC.GetFloatFromRAM(0x4004)));
        }

        [Fact]
        public void SUB_Innn_rI_r()
        {
            RunTest(
                "SUB (0x4000+X),B",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.LoadMemAt(0x4003, new byte[] { 10, 20 });
                    c.CPU.REGS.B = 5;
                },
                c => Assert.Equal((byte)5, c.MEMC.Get8bitFromRAM(0x4003)));
        }

        [Fact]
        public void SUB_Innn_rI_rr()
        {
            RunTest(
                "SUB (0x4000+X),CD",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.LoadMemAt(0x4003, new byte[] { 10, 10 });
                    c.CPU.REGS.CD = 0x0102;
                },
                c => Assert.Equal((ushort)0x0908, c.MEMC.Get16bitFromRAM(0x4003)));
        }

        [Fact]
        public void SUB_Innn_rI_rrr()
        {
            RunTest(
                "SUB (0x4000+X),DEF",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.LoadMemAt(0x4003, new byte[] { 20, 30, 40 });
                    c.CPU.REGS.DEF = 0x000203;
                },
                c => Assert.Equal((uint)0x001C25, c.MEMC.Get24bitFromRAM(0x4003)));
        }

        [Fact]
        public void SUB_Innn_rI_rrrr()
        {
            RunTest(
                "SUB (0x4000+X),EFGH",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.LoadMemAt(0x4003, new byte[] { 10, 10, 10, 10 });
                    c.CPU.REGS.EFGH = 0x00030405;
                },
                c => Assert.Equal((uint)0x00070605, c.MEMC.Get32bitFromRAM(0x4003)));
        }

        [Fact]
        public void SUB_Innn_rI_InnnI_n_rrr()
        {
            byte[] initial = new byte[] { 20, 30, 40, 50 };
            byte[] value = new byte[] { 1, 2, 3, 4 };
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
                    var expected = ComputeExpectedSub(initial, value, 3, 2);
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4003, 3));
                });
        }

        [Fact]
        public void SUB_Innn_rI_fr()
        {
            RunTest(
                "SUB (0x4000+X),F4",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.MEMC.SetFloatToRam(0x4003, 5.25f);
                    c.CPU.FREGS.SetRegister(4, 2.5f);
                },
                c => Assert.Equal(2.75f, c.MEMC.GetFloatFromRAM(0x4003)));
        }

        [Fact]
        public void SUB_IrrrI_r()
        {
            RunTest(
                "SUB (QRS),B",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5000, new byte[] { 10, 20 });
                    c.CPU.REGS.B = 5;
                },
                c => Assert.Equal((byte)5, c.MEMC.Get8bitFromRAM(0x5000)));
        }

        [Fact]
        public void SUB_IrrrI_rr()
        {
            RunTest(
                "SUB (QRS),CD",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5000, new byte[] { 10, 10 });
                    c.CPU.REGS.CD = 0x0102;
                },
                c => Assert.Equal((ushort)0x0908, c.MEMC.Get16bitFromRAM(0x5000)));
        }

        [Fact]
        public void SUB_IrrrI_rrr()
        {
            RunTest(
                "SUB (QRS),DEF",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5000, new byte[] { 20, 30, 40 });
                    c.CPU.REGS.DEF = 0x000203;
                },
                c => Assert.Equal((uint)0x001C25, c.MEMC.Get24bitFromRAM(0x5000)));
        }

        [Fact]
        public void SUB_IrrrI_rrrr()
        {
            RunTest(
                "SUB (QRS),EFGH",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5000, new byte[] { 10, 10, 10, 10 });
                    c.CPU.REGS.EFGH = 0x00030405;
                },
                c => Assert.Equal((uint)0x00070605, c.MEMC.Get32bitFromRAM(0x5000)));
        }

        #endregion

        #region SUB fr,* tests (17 tests)

        [Fact]
        public void SUB_fr_nnn()
        {
            RunTest(
                "SUB F0,0x40A00000",
                c => c.CPU.FREGS.SetRegister(0, 100.5f),
                c => Assert.Equal(95.5f, c.CPU.FREGS.GetRegister(0), precision: 5));
        }

        [Fact]
        public void SUB_fr_nnnn()
        {
            RunTest(
                "SUB F0,0x40C00000",
                c => c.CPU.FREGS.SetRegister(0, 100.5f),
                c => Assert.Equal(94.5f, c.CPU.FREGS.GetRegister(0), precision: 5));
        }

        [Fact]
        public void SUB_fr_r()
        {
            RunTest(
                "SUB F0,J",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 100.5f);
                    c.CPU.REGS.J = 3;
                },
                c => Assert.Equal(97.5f, c.CPU.FREGS.GetRegister(0), precision: 5));
        }

        [Fact]
        public void SUB_fr_rr()
        {
            RunTest(
                "SUB F0,JK",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 100.5f);
                    c.CPU.REGS.JK = 10;
                },
                c => Assert.Equal(90.5f, c.CPU.FREGS.GetRegister(0), precision: 5));
        }

        [Fact]
        public void SUB_fr_rrr()
        {
            RunTest(
                "SUB F0,JKL",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 100000.5f);
                    c.CPU.REGS.JKL = 50000;
                },
                c => Assert.Equal(50000.5f, c.CPU.FREGS.GetRegister(0), precision: 5));
        }

        [Fact]
        public void SUB_fr_rrrr()
        {
            RunTest(
                "SUB F0,JKLM",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 2000000.5f);
                    c.CPU.REGS.JKLM = 1000;
                },
                c => Assert.Equal(1999000.5f, c.CPU.FREGS.GetRegister(0), precision: 5));
        }

        [Fact]
        public void SUB_fr_fr()
        {
            RunTest(
                "SUB F0,F1",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 100.5f);
                    c.CPU.FREGS.SetRegister(1, 2.75f);
                },
                c => Assert.Equal(97.75f, c.CPU.FREGS.GetRegister(0), precision: 5));
        }

        [Fact]
        public void SUB_fr_InnnI()
        {
            RunTest(
                "SUB F0,(0x4000)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 5.0f);
                    c.MEMC.SetFloatToRam(0x4000, 0.75f);
                },
                c => Assert.Equal(4.25f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void SUB_fr_Innn_nnnI()
        {
            RunTest(
                "SUB F0,(0x4000+4)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 5.0f);
                    c.MEMC.SetFloatToRam(0x4004, 0.75f);
                },
                c => Assert.Equal(4.25f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void SUB_fr_Innn_rI()
        {
            RunTest(
                "SUB F0,(0x4000+X)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 5.0f);
                    c.CPU.REGS.X = 3;
                    c.MEMC.SetFloatToRam(0x4003, 0.75f);
                },
                c => Assert.Equal(4.25f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void SUB_fr_Innn_rrI()
        {
            RunTest(
                "SUB F0,(0x4000+WX)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 5.0f);
                    c.CPU.REGS.WX = 5;
                    c.MEMC.SetFloatToRam(0x4005, 0.75f);
                },
                c => Assert.Equal(4.25f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void SUB_fr_Innn_rrrI()
        {
            RunTest(
                "SUB F0,(0x4000+VWX)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 5.0f);
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.SetFloatToRam(0x4007, 0.75f);
                },
                c => Assert.Equal(4.25f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void SUB_fr_IrrrI()
        {
            RunTest(
                "SUB F0,(QRS)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 5.0f);
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.SetFloatToRam(0x5000, 0.75f);
                },
                c => Assert.Equal(4.25f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void SUB_fr_Irrr_nnnI()
        {
            RunTest(
                "SUB F0,(QRS+4)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 5.0f);
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.SetFloatToRam(0x5004, 0.75f);
                },
                c => Assert.Equal(4.25f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void SUB_fr_Irrr_rI()
        {
            RunTest(
                "SUB F0,(QRS+X)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 5.0f);
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.SetFloatToRam(0x5003, 0.75f);
                },
                c => Assert.Equal(4.25f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void SUB_fr_Irrr_rrI()
        {
            RunTest(
                "SUB F0,(QRS+WX)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 5.0f);
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.SetFloatToRam(0x5005, 0.75f);
                },
                c => Assert.Equal(4.25f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void SUB_fr_Irrr_rrrI()
        {
            RunTest(
                "SUB F0,(QRS+VWX)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 5.0f);
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.SetFloatToRam(0x5007, 0.75f);
                },
                c => Assert.Equal(4.25f, c.CPU.FREGS.GetRegister(0)));
        }

        #endregion
    }
}
