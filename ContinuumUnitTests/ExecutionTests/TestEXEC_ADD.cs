using Continuum93.Emulator;
using Continuum93.Emulator.Interpreter;

namespace ExecutionTests
{
    public class TestEXEC_ADD
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

        private static byte[] ComputeExpectedAdd(byte[] initial, byte[] add, int count, int repeat)
        {
            byte[] working = new byte[count];
            Array.Copy(initial, working, count);
            
            for (int r = 0; r < repeat; r++)
            {
                byte carry = 0;
                for (int i = count - 1; i >= 0; i--)
                {
                    ushort sum = (ushort)(working[i] + add[i] + carry);
                    working[i] = (byte)sum;
                    carry = (byte)(sum >> 8);
                }
            }
            
            return working;
        }

        #endregion

        #region ADD r,* tests

        [Fact]
        public void ADD_r_n()   // Ok
        {
            RunTest(
                "ADD A,5",
                c => c.CPU.REGS.A = 10,
                c => Assert.Equal((byte)15, c.CPU.REGS.A));
        }

        [Fact]
        public void ADD_r_r()   // Ok
        {
            RunTest(
                "ADD A,B",
                c =>
                {
                    c.CPU.REGS.A = 10;
                    c.CPU.REGS.B = 3;
                },
                c => Assert.Equal((byte)13, c.CPU.REGS.A));
        }

        [Fact]
        public void ADD_r_InnnI()   // Ok
        {
            RunTest(
                "ADD A,(0x4000)",
                c =>
                {
                    c.CPU.REGS.A = 10;
                    c.MEMC.Set8bitToRAM(0x4000, 51);
                },
                c => Assert.Equal((byte)61, c.CPU.REGS.A));
        }

        [Fact]
        public void ADD_r_Innn_nnnI()   // Ok
        {
            RunTest(
                "ADD A,(0x4000+4)",
                c =>
                {
                    c.CPU.REGS.A = 10;
                    c.MEMC.Set8bitToRAM(0x4004, 51);
                },
                c => Assert.Equal((byte)61, c.CPU.REGS.A));
        }

        [Fact]
        public void ADD_r_Innn_rI() // Ok
        {
            RunTest(
                "ADD A,(0x4000+X)",
                c =>
                {
                    c.CPU.REGS.A = 10;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set8bitToRAM(0x4003, 51);
                },
                c => Assert.Equal((byte)61, c.CPU.REGS.A));
        }

        [Fact]
        public void ADD_r_Innn_rrI()    // Ok
        {
            RunTest(
                "ADD A,(0x4000+WX)",
                c =>
                {
                    c.CPU.REGS.A = 10;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x4005, 51);
                },
                c => Assert.Equal((byte)61, c.CPU.REGS.A));
        }

        [Fact]
        public void ADD_r_Innn_rrrI()   // Ok
        {
            RunTest(
                "ADD A,(0x4000+VWX)",
                c =>
                {
                    c.CPU.REGS.A = 10;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x4007, 51);
                },
                c => Assert.Equal((byte)61, c.CPU.REGS.A));
        }

        [Fact]
        public void ADD_r_IrrrI()   // Ok
        {
            RunTest(
                "ADD A,(QRS)",
                c =>
                {
                    c.CPU.REGS.A = 10;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x5000, 51);
                },
                c => Assert.Equal((byte)61, c.CPU.REGS.A));
        }

        [Fact]
        public void ADD_r_Irrr_nnnI()   // Ok
        {
            RunTest(
                "ADD A,(QRS+4)",
                c =>
                {
                    c.CPU.REGS.A = 10;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x5004, 51);
                },
                c => Assert.Equal((byte)61, c.CPU.REGS.A));
        }

        [Fact]
        public void ADD_r_Irrr_rI() // Ok
        {
            RunTest(
                "ADD A,(QRS+X)",
                c =>
                {
                    c.CPU.REGS.A = 10;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set8bitToRAM(0x5003, 51);
                },
                c => Assert.Equal((byte)61, c.CPU.REGS.A));
        }

        [Fact]
        public void ADD_r_Irrr_rrI()    // Ok
        {
            RunTest(
                "ADD A,(QRS+WX)",
                c =>
                {
                    c.CPU.REGS.A = 10;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x5005, 51);
                },
                c => Assert.Equal((byte)61, c.CPU.REGS.A));
        }

        [Fact]
        public void ADD_r_Irrr_rrrI()   // Ok
        {
            RunTest(
                "ADD A,(QRS+VWX)",
                c =>
                {
                    c.CPU.REGS.A = 10;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x5007, 51);
                },
                c => Assert.Equal((byte)61, c.CPU.REGS.A));
        }

        [Fact]
        public void ADD_r_fr()  // Ok
        {
            RunTest(
                "ADD A,F0",
                c =>
                {
                    c.CPU.REGS.A = 10;
                    c.CPU.FREGS.SetRegister(0, 1.5f);
                },
                c => Assert.Equal((byte)12, c.CPU.REGS.A));
        }

        #endregion

        #region ADD rr,* tests

        [Fact]
        public void ADD_rr_nn() // Ok
        {
            RunTest(
                "ADD AB,0x1234",
                c => c.CPU.REGS.AB = 0x1000,
                c => Assert.Equal((ushort)0x2234, c.CPU.REGS.AB));
        }

        [Fact]
        public void ADD_rr_r()  // Ok
        {
            RunTest(
                "ADD AB,E",
                c =>
                {
                    c.CPU.REGS.AB = 0x1000;
                    c.CPU.REGS.E = 4;
                },
                c => Assert.Equal((ushort)0x1004, c.CPU.REGS.AB));
        }

        [Fact]
        public void ADD_rr_rr() // Ok
        {
            RunTest(
                "ADD AB,CD",
                c =>
                {
                    c.CPU.REGS.AB = 0x1000;
                    c.CPU.REGS.CD = 0x00FF;
                },
                c => Assert.Equal((ushort)0x10FF, c.CPU.REGS.AB));
        }

        [Fact]
        public void ADD_rr_InnnI()  // Ok
        {
            RunTest(
                "ADD AB,(0x4000)",
                c =>
                {
                    c.CPU.REGS.AB = 0x1000;
                    c.MEMC.Set16bitToRAM(0x4000, 0x0F0F);
                },
                c => Assert.Equal((ushort)0x1F0F, c.CPU.REGS.AB));
        }

        [Fact]
        public void ADD_rr_Innn_nnnI()  // Ok
        {
            RunTest(
                "ADD AB,(0x4000+4)",
                c =>
                {
                    c.CPU.REGS.AB = 0x1000;
                    c.MEMC.Set16bitToRAM(0x4004, 0x0F0F);
                },
                c => Assert.Equal((ushort)0x1F0F, c.CPU.REGS.AB));
        }

        [Fact]
        public void ADD_rr_Innn_rI()    // Ok
        {
            RunTest(
                "ADD AB,(0x4000+X)",
                c =>
                {
                    c.CPU.REGS.AB = 0x1000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set16bitToRAM(0x4003, 0x0F0F);
                },
                c => Assert.Equal((ushort)0x1F0F, c.CPU.REGS.AB));
        }

        [Fact]
        public void ADD_rr_Innn_rrI()   // Ok
        {
            RunTest(
                "ADD AB,(0x4000+WX)",
                c =>
                {
                    c.CPU.REGS.AB = 0x1000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set16bitToRAM(0x4005, 0x0F0F);
                },
                c => Assert.Equal((ushort)0x1F0F, c.CPU.REGS.AB));
        }

        [Fact]
        public void ADD_rr_Innn_rrrI()  // Ok
        {
            RunTest(
                "ADD AB,(0x4000+VWX)",
                c =>
                {
                    c.CPU.REGS.AB = 0x1000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set16bitToRAM(0x4007, 0x0F0F);
                },
                c => Assert.Equal((ushort)0x1F0F, c.CPU.REGS.AB));
        }

        [Fact]
        public void ADD_rr_IrrrI()  // Ok
        {
            RunTest(
                "ADD AB,(QRS)",
                c =>
                {
                    c.CPU.REGS.AB = 0x1000;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set16bitToRAM(0x5000, 0x0F0F);
                },
                c => Assert.Equal((ushort)0x1F0F, c.CPU.REGS.AB));
        }

        [Fact]
        public void ADD_rr_Irrr_nnnI()  // Ok
        {
            RunTest(
                "ADD AB,(QRS+4)",
                c =>
                {
                    c.CPU.REGS.AB = 0x1000;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set16bitToRAM(0x5004, 0x0F0F);
                },
                c => Assert.Equal((ushort)0x1F0F, c.CPU.REGS.AB));
        }

        [Fact]
        public void ADD_rr_Irrr_rI()    // Ok
        {
            RunTest(
                "ADD AB,(QRS+X)",
                c =>
                {
                    c.CPU.REGS.AB = 0x1000;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set16bitToRAM(0x5003, 0x0F0F);
                },
                c => Assert.Equal((ushort)0x1F0F, c.CPU.REGS.AB));
        }

        [Fact]
        public void ADD_rr_Irrr_rrI()   // Ok
        {
            RunTest(
                "ADD AB,(QRS+WX)",
                c =>
                {
                    c.CPU.REGS.AB = 0x1000;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set16bitToRAM(0x5005, 0x0F0F);
                },
                c => Assert.Equal((ushort)0x1F0F, c.CPU.REGS.AB));
        }

        [Fact]
        public void ADD_rr_Irrr_rrrI()  // Ok
        {
            RunTest(
                "ADD AB,(QRS+VWX)",
                c =>
                {
                    c.CPU.REGS.AB = 0x1000;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set16bitToRAM(0x5007, 0x0F0F);
                },
                c => Assert.Equal((ushort)0x1F0F, c.CPU.REGS.AB));
        }

        [Fact]
        public void ADD_rr_fr() // Ok
        {
            RunTest(
                "ADD AB,F1",
                c =>
                {
                    c.CPU.REGS.AB = 0x1000;
                    c.CPU.FREGS.SetRegister(1, 5.6f);
                },
                c => Assert.Equal((ushort)0x1006, c.CPU.REGS.AB));
        }

        #endregion

        #region ADD rrr,* tests

        [Fact]
        public void ADD_rrr_nnn()   // Ok
        {
            RunTest(
                "ADD ABC,0x000102",
                c => c.CPU.REGS.ABC = 0x010203,
                c => Assert.Equal((uint)0x010305, c.CPU.REGS.ABC));
        }

        [Fact]
        public void ADD_rrr_r() // Ok
        {
            RunTest(
                "ADD ABC,E",
                c =>
                {
                    c.CPU.REGS.ABC = 0x010203;
                    c.CPU.REGS.E = 7;
                },
                c => Assert.Equal((uint)0x01020A, c.CPU.REGS.ABC));
        }

        [Fact]
        public void ADD_rrr_rr()    // Ok
        {
            RunTest(
                "ADD ABC,EF",
                c =>
                {
                    c.CPU.REGS.ABC = 0x010203;
                    c.CPU.REGS.EF = 0x0102;
                },
                c => Assert.Equal((uint)0x010305, c.CPU.REGS.ABC));
        }

        [Fact]
        public void ADD_rrr_rrr()   // Ok
        {
            RunTest(
                "ADD ABC,DEF",
                c =>
                {
                    c.CPU.REGS.ABC = 0x010203;
                    c.CPU.REGS.DEF = 0x00000F;
                },
                c => Assert.Equal((uint)0x010212, c.CPU.REGS.ABC));
        }

        [Fact]
        public void ADD_rrr_InnnI() // Ok
        {
            RunTest(
                "ADD ABC,(0x4000)",
                c =>
                {
                    c.CPU.REGS.ABC = 0x010203;
                    c.MEMC.Set24bitToRAM(0x4000, 0x000F0F);
                },
                c => Assert.Equal((uint)0x011112, c.CPU.REGS.ABC));
        }

        [Fact]
        public void ADD_rrr_Innn_nnnI() // Ok
        {
            RunTest(
                "ADD ABC,(0x4000+4)",
                c =>
                {
                    c.CPU.REGS.ABC = 0x010203;
                    c.MEMC.Set24bitToRAM(0x4004, 0x000F0F);
                },
                c => Assert.Equal((uint)0x011112, c.CPU.REGS.ABC));
        }

        [Fact]
        public void ADD_rrr_Innn_rI()   // Ok
        {
            RunTest(
                "ADD ABC,(0x4000+X)",
                c =>
                {
                    c.CPU.REGS.ABC = 0x010203;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set24bitToRAM(0x4003, 0x000F0F);
                },
                c => Assert.Equal((uint)0x011112, c.CPU.REGS.ABC));
        }

        [Fact]
        public void ADD_rrr_Innn_rrI()  // Ok
        {
            RunTest(
                "ADD ABC,(0x4000+WX)",
                c =>
                {
                    c.CPU.REGS.ABC = 0x010203;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set24bitToRAM(0x4005, 0x000F0F);
                },
                c => Assert.Equal((uint)0x011112, c.CPU.REGS.ABC));
        }

        [Fact]
        public void ADD_rrr_Innn_rrrI() // Ok
        {
            RunTest(
                "ADD ABC,(0x4000+VWX)",
                c =>
                {
                    c.CPU.REGS.ABC = 0x010203;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set24bitToRAM(0x4007, 0x000F0F);
                },
                c => Assert.Equal((uint)0x011112, c.CPU.REGS.ABC));
        }

        [Fact]
        public void ADD_rrr_IrrrI() // Ok
        {
            RunTest(
                "ADD ABC,(QRS)",
                c =>
                {
                    c.CPU.REGS.ABC = 0x010203;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set24bitToRAM(0x5000, 0x000F0F);
                },
                c => Assert.Equal((uint)0x011112, c.CPU.REGS.ABC));
        }

        [Fact]
        public void ADD_rrr_Irrr_nnnI() // Ok
        {
            RunTest(
                "ADD ABC,(QRS+4)",
                c =>
                {
                    c.CPU.REGS.ABC = 0x010203;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set24bitToRAM(0x5004, 0x000F0F);
                },
                c => Assert.Equal((uint)0x011112, c.CPU.REGS.ABC));
        }

        [Fact]
        public void ADD_rrr_Irrr_rI()   // Ok
        {
            RunTest(
                "ADD ABC,(QRS+X)",
                c =>
                {
                    c.CPU.REGS.ABC = 0x010203;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set24bitToRAM(0x5003, 0x000F0F);
                },
                c => Assert.Equal((uint)0x011112, c.CPU.REGS.ABC));
        }

        [Fact]
        public void ADD_rrr_Irrr_rrI()  // Ok
        {
            RunTest(
                "ADD ABC,(QRS+WX)",
                c =>
                {
                    c.CPU.REGS.ABC = 0x010203;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set24bitToRAM(0x5005, 0x000F0F);
                },
                c => Assert.Equal((uint)0x011112, c.CPU.REGS.ABC));
        }

        [Fact]
        public void ADD_rrr_Irrr_rrrI() // Ok
        {
            RunTest(
                "ADD ABC,(QRS+VWX)",
                c =>
                {
                    c.CPU.REGS.ABC = 0x010203;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set24bitToRAM(0x5007, 0x000F0F);
                },
                c => Assert.Equal((uint)0x011112, c.CPU.REGS.ABC));
        }

        [Fact]
        public void ADD_rrr_fr()    // Ok
        {
            RunTest(
                "ADD ABC,F2",
                c =>
                {
                    c.CPU.REGS.ABC = 0x010203;
                    c.CPU.FREGS.SetRegister(2, 12.5f);
                },
                c => Assert.Equal((uint)0x01020F, c.CPU.REGS.ABC));
        }

        #endregion

        #region ADD rrrr,* tests

        [Fact]
        public void ADD_rrrr_nnnn() // Ok
        {
            RunTest(
                "ADD ABCD,0x00010002",
                c => c.CPU.REGS.ABCD = 0x10002000,
                c => Assert.Equal((uint)0x10012002, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void ADD_rrrr_r()    // Ok
        {
            RunTest(
                "ADD ABCD,E",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x10002000;
                    c.CPU.REGS.E = 9;
                },
                c => Assert.Equal((uint)0x10002009, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void ADD_rrrr_rr()   // Ok
        {
            RunTest(
                "ADD ABCD,EF",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x10002000;
                    c.CPU.REGS.EF = 0x00FF;
                },
                c => Assert.Equal((uint)0x100020FF, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void ADD_rrrr_rrr()  // Ok
        {
            RunTest(
                "ADD ABCD,DEF",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x10002000;
                    c.CPU.REGS.DEF = 0x0000AA;
                },
                c => Assert.Equal((uint)0x100020AA, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void ADD_rrrr_rrrr() // Ok
        {
            RunTest(
                "ADD ABCD,EFGH",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x10002000;
                    c.CPU.REGS.EFGH = 0x00000010;
                },
                c => Assert.Equal((uint)0x10002010, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void ADD_rrrr_InnnI()    // Ok
        {
            RunTest(
                "ADD ABCD,(0x4000)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x10002000;
                    c.MEMC.Set32bitToRAM(0x4000, 0x000000FF);
                },
                c => Assert.Equal((uint)0x100020FF, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void ADD_rrrr_Innn_nnnI()    // Ok
        {
            RunTest(
                "ADD ABCD,(0x4000+4)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x10002000;
                    c.MEMC.Set32bitToRAM(0x4004, 0x000000FF);
                },
                c => Assert.Equal((uint)0x100020FF, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void ADD_rrrr_Innn_rI()  // Ok
        {
            RunTest(
                "ADD ABCD,(0x4000+X)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x10002000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set32bitToRAM(0x4003, 0x000000FF);
                },
                c => Assert.Equal((uint)0x100020FF, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void ADD_rrrr_Innn_rrI() // Ok
        {
            RunTest(
                "ADD ABCD,(0x4000+WX)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x10002000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set32bitToRAM(0x4005, 0x000000FF);
                },
                c => Assert.Equal((uint)0x100020FF, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void ADD_rrrr_Innn_rrrI()    // Ok
        {
            RunTest(
                "ADD ABCD,(0x4000+VWX)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x10002000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set32bitToRAM(0x4007, 0x000000FF);
                },
                c => Assert.Equal((uint)0x100020FF, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void ADD_rrrr_IrrrI()    // Ok
        {
            RunTest(
                "ADD ABCD,(QRS)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x10002000;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set32bitToRAM(0x5000, 0x000000FF);
                },
                c => Assert.Equal((uint)0x100020FF, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void ADD_rrrr_Irrr_nnnI()    // Ok
        {
            RunTest(
                "ADD ABCD,(QRS+4)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x10002000;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set32bitToRAM(0x5004, 0x000000FF);
                },
                c => Assert.Equal((uint)0x100020FF, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void ADD_rrrr_Irrr_rI()  // Ok
        {
            RunTest(
                "ADD ABCD,(QRS+X)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x10002000;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set32bitToRAM(0x5003, 0x000000FF);
                },
                c => Assert.Equal((uint)0x100020FF, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void ADD_rrrr_Irrr_rrI() // Ok
        {
            RunTest(
                "ADD ABCD,(QRS+WX)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x10002000;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set32bitToRAM(0x5005, 0x000000FF);
                },
                c => Assert.Equal((uint)0x100020FF, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void ADD_rrrr_Irrr_rrrI()    // Ok
        {
            RunTest(
                "ADD ABCD,(QRS+VWX)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x10002000;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set32bitToRAM(0x5007, 0x000000FF);
                },
                c => Assert.Equal((uint)0x100020FF, c.CPU.REGS.ABCD));
        }

        [Fact]
        public void ADD_rrrr_fr()   // Ok
        {
            RunTest(
                "ADD ABCD,F3",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x10002000;
                    c.CPU.FREGS.SetRegister(3, 42.8f);
                },
                c => Assert.Equal((uint)0x1000202B, c.CPU.REGS.ABCD));
        }

        #endregion

        #region ADD (nnn),* tests - immediate and register

        [Fact]
        public void ADD_InnnI_nnnn_n()  // Ok
        {
            RunTest(
                "ADD (0x4000),0x00010203,2",
                c => c.LoadMemAt(0x4000, [0x01, 0x02, 0x03]),
                c => Assert.Equal([0x03, 0x05], c.MEMC.RAM.GetMemoryAt(0x4000, 2)));
        }

        [Fact]
        public void ADD_InnnI_nnnn_n_repeat()   // Ok
        {
            RunTest(
                "ADD (0x4000),0x00010203,3,2",
                c => c.LoadMemAt(0x4000, [0x01, 0x02, 0x03]),
                c => Assert.Equal([0x03, 0x06, 0x09], c.MEMC.RAM.GetMemoryAt(0x4000, 3)));
        }

        [Fact]
        public void ADD_InnnI_r()   // Ok
        {
            RunTest(
                "ADD (0x4000),B",
                c =>
                {
                    c.LoadMemAt(0x4000, [0x01, 0x02]);
                    c.CPU.REGS.B = 0x05;
                },
                c => Assert.Equal(0x06, c.MEMC.Get8bitFromRAM(0x4000)));
        }

        [Fact]
        public void ADD_InnnI_rr()  // Ok
        {
            RunTest(
                "ADD (0x4000),CD",
                c =>
                {
                    c.LoadMemAt(0x4000, [0x01, 0x01]);
                    c.CPU.REGS.CD = 0x0102;
                },
                c => Assert.Equal(0x0203, c.MEMC.Get16bitFromRAM(0x4000)));
        }

        [Fact]
        public void ADD_InnnI_rrr() // Ok
        {
            RunTest(
                "ADD (0x4000),DEF",
                c =>
                {
                    c.LoadMemAt(0x4000, [0x0A, 0x14, 0x1E]);
                    c.CPU.REGS.DEF = 0x000203;
                },
                c => Assert.Equal((uint)0x0A1621, c.MEMC.Get24bitFromRAM(0x4000)));
        }

        [Fact]
        public void ADD_InnnI_rrrr()    // Ok
        {
            RunTest(
                "ADD (0x4000),EFGH",
                c =>
                {
                    c.LoadMemAt(0x4000, [0x01, 0x02, 0x03, 0x04]);
                    c.CPU.REGS.EFGH = 0x00030405;
                },
                c => Assert.Equal((uint)0x01050709, c.MEMC.Get32bitFromRAM(0x4000)));
        }

        [Fact]
        public void ADD_InnnI_InnnI_n_rrr() // Ok
        {
            byte[] initial = [0x0A, 0x14, 0x1E, 0x28];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "ADD (0x4000),(0x7000),3,GHI",
                c =>
                {
                    c.LoadMemAt(0x4000, initial);
                    c.LoadMemAt(0x7000, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [0x0C, 0x18, 0x24, 0x28];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4000, 4));
                });
        }

        // TODO: ADD _InnnI_Innn_nnnI_n_rrr
        // TODO: ADD _InnnI_Innn_rI_n_rrr
        // TODO: ADD _InnnI_Innn_rrI_n_rrr
        // TODO: ADD _InnnI_Innn_rrrI_n_rrr
        // TODO: ADD _InnnI_IrrrI_n_rrr
        // TODO: ADD _InnnI_Irrr_nnnI_n_rrr
        // TODO: ADD _InnnI_Irrr_rI_n_rrr
        // TODO: ADD _InnnI_Irrr_rrI_n_rrr
        // TODO: ADD _InnnI_Irrr_rrrI_n_rrr

        [Fact]
        public void ADD_InnnI_fr()  // Ok
        {
            RunTest(
                "ADD (0x4000),F4",
                c =>
                {
                    c.MEMC.SetFloatToRam(0x4000, 1.25f);
                    c.CPU.FREGS.SetRegister(4, 2.5f);
                },
                c => Assert.Equal(3.75f, c.MEMC.GetFloatFromRAM(0x4000)));
        }

        [Fact]
        public void ADD_Innn_nnnI_nnnn_n()  // Ok
        {
            RunTest(
                "ADD (0x4000+4),0x00010203,2",
                c => c.LoadMemAt(0x4004, [0x01, 0x02, 0x03]),
                c => Assert.Equal([0x03, 0x05, 0x03], c.MEMC.RAM.GetMemoryAt(0x4004, 3)));
        }

        // TODO: ADD _Innn_nnnI_nnnn_n_nnn (last nnn is repeat)

        [Fact]
        public void ADD_Innn_nnnI_r()   // Ok
        {
            RunTest(
                "ADD (0x4000+4),B",
                c =>
                {
                    c.LoadMemAt(0x4004, [1, 2]);
                    c.CPU.REGS.B = 5;
                },
                c => Assert.Equal(0x06, c.MEMC.Get8bitFromRAM(0x4004)));
        }

        [Fact]
        public void ADD_Innn_nnnI_rr()  // Ok
        {
            RunTest(
                "ADD (0x4000+4),CD",
                c =>
                {
                    c.LoadMemAt(0x4004, [1, 1]);
                    c.CPU.REGS.CD = 0x0102;
                },
                c => Assert.Equal((ushort)0x0203, c.MEMC.Get16bitFromRAM(0x4004)));
        }

        [Fact]
        public void ADD_Innn_nnnI_rrr() // Ok
        {
            RunTest(
                "ADD (0x4000+4),DEF",
                c =>
                {
                    c.LoadMemAt(0x4004, [0x0A, 0x14, 0x1E]);
                    c.CPU.REGS.DEF = 0x000203;
                },
                c => Assert.Equal((uint)0x0A1621, c.MEMC.Get24bitFromRAM(0x4004)));
        }

        [Fact]
        public void ADD_Innn_nnnI_rrrr()    // Ok
        {
            RunTest(
                "ADD (0x4000+4),EFGH",
                c =>
                {
                    c.LoadMemAt(0x4004, [0x01, 0x01, 0x01, 0x01]);
                    c.CPU.REGS.EFGH = 0x00030405;
                },
                c => Assert.Equal((uint)0x01040506, c.MEMC.Get32bitFromRAM(0x4004)));
        }

        [Fact]
        public void ADD_Innn_nnnI_InnnI_n_rrr() // Ok
        {
            byte[] initial = [10, 20, 30, 40];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "ADD (0x4000+4),(0x7000),3,GHI",
                c =>
                {
                    c.LoadMemAt(0x4004, initial);
                    c.LoadMemAt(0x7000, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [12, 24, 36, 40];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4004, 4));
                });
        }

        // TODO: ADD _Innn_nnnI_Innn_nnnI_n_rrr
        // TODO: ADD _Innn_nnnI_Innn_rI_n_rrr
        // TODO: ADD _Innn_nnnI_Innn_rrI_n_rrr
        // TODO: ADD _Innn_nnnI_Innn_rrrI_n_rrr
        // TODO: ADD _Innn_nnnI_IrrrI_n_rrr
        // TODO: ADD _Innn_nnnI_Irrr_nnnI_n_rrr
        // TODO: ADD _Innn_nnnI_Irrr_rI_n_rrr
        // TODO: ADD _Innn_nnnI_Irrr_rrI_n_rrr
        // TODO: ADD _Innn_nnnI_Irrr_rrrI_n_rrr

        [Fact]
        public void ADD_Innn_nnnI_fr()  // Ok
        {
            RunTest(
                "ADD (0x4000+4),F4",
                c =>
                {
                    c.MEMC.SetFloatToRam(0x4004, 1.25f);
                    c.CPU.FREGS.SetRegister(4, 2.5f);
                },
                c => Assert.Equal(3.75f, c.MEMC.GetFloatFromRAM(0x4004)));
        }

        // TODO: ADD _Innn_rI_nnnn_n
        // TODO: ADD _Innn_rI_nnnn_n_nnn (last nnn is repeat)

        [Fact]
        public void ADD_Innn_rI_r() // Ok
        {
            RunTest(
                "ADD (0x4000+X),B",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.LoadMemAt(0x4003, [0x01, 0x02]);
                    c.CPU.REGS.B = 5;
                },
                c => Assert.Equal([0x06, 0x02], c.MEMC.RAM.GetMemoryAt(0x4003, 2)));
        }

        [Fact]
        public void ADD_Innn_rI_rr()    // Ok
        {
            RunTest(
                "ADD (0x4000+X),CD",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.LoadMemAt(0x4003, new byte[] { 1, 1, 1 });
                    c.CPU.REGS.CD = 0x0102;
                },
                c => Assert.Equal([0x02, 0x03, 0x01], c.MEMC.RAM.GetMemoryAt(0x4003, 3)));
        }

        [Fact]
        public void ADD_Innn_rI_rrr()   // Ok
        {
            RunTest(
                "ADD (0x4000+X),DEF",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.LoadMemAt(0x4003, [0x04, 0x03, 0x02, 0x01]);
                    c.CPU.REGS.DEF = 0x010203;
                },
                c => Assert.Equal([0x05, 0x05, 0x05, 0x01], c.MEMC.RAM.GetMemoryAt(0x4003, 4)));
        }

        [Fact]
        public void ADD_Innn_rI_rrrr()  // Ok
        {
            RunTest(
                "ADD (0x4000+X),EFGH",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.LoadMemAt(0x4003, [1, 1, 1, 1, 1]);
                    c.CPU.REGS.EFGH = 0x05040302;
                },
                c => Assert.Equal([0x06, 0x05, 0x04, 0x03, 0x01], c.MEMC.RAM.GetMemoryAt(0x4003, 5)));
        }

        [Fact]
        public void ADD_Innn_rI_InnnI_n_rrr()   // Ok
        {
            byte[] initial = [10, 20, 30, 40];
            byte[] value = [1, 2, 3, 4];
            RunTest(
                "ADD (0x4000+X),(0x7000),3,GHI",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.LoadMemAt(0x4003, initial);
                    c.LoadMemAt(0x7000, value);
                    c.CPU.REGS.GHI = 2;
                },
                c =>
                {
                    byte[] expected = [12, 24, 36, 40];
                    Assert.Equal(expected, c.MEMC.RAM.GetMemoryAt(0x4003, 4));
                });
        }

        // TODO: ADD _Innn_rI_Innn_nnnI_n_rrr
        // TODO: ADD _Innn_rI_Innn_rI_n_rrr
        // TODO: ADD _Innn_rI_Innn_rrI_n_rrr
        // TODO: ADD _Innn_rI_Innn_rrrI_n_rrr
        // TODO: ADD _Innn_rI_IrrrI_n_rrr
        // TODO: ADD _Innn_rI_Irrr_nnnI_n_rrr
        // TODO: ADD _Innn_rI_Irrr_rI_n_rrr
        // TODO: ADD _Innn_rI_Irrr_rrI_n_rrr
        // TODO: ADD _Innn_rI_Irrr_rrrI_n_rrr


        [Fact]
        public void ADD_Innn_rI_fr()    // Ok
        {
            RunTest(
                "ADD (0x4000+X),F4",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.MEMC.SetFloatToRam(0x4003, 1.25f);
                    c.CPU.FREGS.SetRegister(4, 2.5f);
                },
                c => Assert.Equal(3.75f, c.MEMC.GetFloatFromRAM(0x4003)));
        }

        // TODO: ADD _Innn_rrI_nnnn_n
        // TODO: ADD _Innn_rrI_nnnn_n_nnn   (last nnn is repeat)
        // TODO: ADD _Innn_rrI_r
        // TODO: ADD _Innn_rrI_rr
        // TODO: ADD _Innn_rrI_rrr
        // TODO: ADD _Innn_rrI_rrrr
        // TODO: ADD _Innn_rrI_InnnI_n_rrr
        // TODO: ADD _Innn_rrI_Innn_nnnI_n_rrr
        // TODO: ADD _Innn_rrI_Innn_rI_n_rrr
        // TODO: ADD _Innn_rrI_Innn_rrI_n_rrr
        // TODO: ADD _Innn_rrI_Innn_rrrI_n_rrr
        // TODO: ADD _Innn_rrI_IrrrI_n_rrr
        // TODO: ADD _Innn_rrI_Irrr_nnnI_n_rrr
        // TODO: ADD _Innn_rrI_Irrr_rI_n_rrr
        // TODO: ADD _Innn_rrI_Irrr_rrI_n_rrr
        // TODO: ADD _Innn_rrI_Irrr_rrrI_n_rrr
        // TODO: ADD _Innn_rrI_fr

        // TODO: ADD _Innn_rrrI_nnnn_n
        // TODO: ADD _Innn_rrrI_nnnn_n_nnn
        // TODO: ADD _Innn_rrrI_r
        // TODO: ADD _Innn_rrrI_rr
        // TODO: ADD _Innn_rrrI_rrr
        // TODO: ADD _Innn_rrrI_rrrr
        // TODO: ADD _Innn_rrrI_InnnI_n_rrr
        // TODO: ADD _Innn_rrrI_Innn_nnnI_n_rrr
        // TODO: ADD _Innn_rrrI_Innn_rI_n_rrr
        // TODO: ADD _Innn_rrrI_Innn_rrI_n_rrr
        // TODO: ADD _Innn_rrrI_Innn_rrrI_n_rrr
        // TODO: ADD _Innn_rrrI_IrrrI_n_rrr
        // TODO: ADD _Innn_rrrI_Irrr_nnnI_n_rrr
        // TODO: ADD _Innn_rrrI_Irrr_rI_n_rrr
        // TODO: ADD _Innn_rrrI_Irrr_rrI_n_rrr
        // TODO: ADD _Innn_rrrI_Irrr_rrrI_n_rrr
        // TODO: ADD _Innn_rrrI_fr

        // TODO: ADD _IrrrI_nnnn_n
        // TODO: ADD _IrrrI_nnnn_n_nnn (last nnn is repeat)

        [Fact]
        public void ADD_IrrrI_r()   // Ok
        {
            RunTest(
                "ADD (QRS),B",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5000, [1, 2]);
                    c.CPU.REGS.B = 5;
                },
                c => Assert.Equal([0x06, 0x02], c.MEMC.RAM.GetMemoryAt(0x5000, 2)));
        }

        [Fact]
        public void ADD_IrrrI_rr()  // Ok
        {
            RunTest(
                "ADD (QRS),CD",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5000, [0x01, 0x02]);
                    c.CPU.REGS.CD = 0x0102;
                },
                c => Assert.Equal([0x02, 0x04], c.MEMC.RAM.GetMemoryAt(0x5000, 2)));
        }

        [Fact]
        public void ADD_IrrrI_rrr() // Ok
        {
            RunTest(
                "ADD (QRS),DEF",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5000, [10, 20, 30]);
                    c.CPU.REGS.DEF = 0x010203;
                },
                c => Assert.Equal([11, 22, 33], c.MEMC.RAM.GetMemoryAt(0x5000, 3)));
        }

        [Fact]
        public void ADD_IrrrI_rrrr()    // Ok
        {
            RunTest(
                "ADD (QRS),EFGH",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.LoadMemAt(0x5000, [4, 3, 2, 1]);
                    c.CPU.REGS.EFGH = 0x02030405;
                },
                c => Assert.Equal([0x06, 0x06, 0x06, 0x06], c.MEMC.RAM.GetMemoryAt(0x5000, 4)));
        }

        // TODO: ADD _IrrrI_InnnI_n_rrr
        // TODO: ADD _IrrrI_Innn_nnnI_n_rrr
        // TODO: ADD _IrrrI_Innn_rI_n_rrr
        // TODO: ADD _IrrrI_Innn_rrI_n_rrr
        // TODO: ADD _IrrrI_Innn_rrrI_n_rrr
        // TODO: ADD _IrrrI_IrrrI_n_rrr
        // TODO: ADD _IrrrI_Irrr_nnnI_n_rrr
        // TODO: ADD _IrrrI_Irrr_rI_n_rrr
        // TODO: ADD _IrrrI_Irrr_rrI_n_rrr
        // TODO: ADD _IrrrI_Irrr_rrrI_n_rrr
        // TODO: ADD _IrrrI_fr

        // TODO: ADD _Irrr_nnnI_nnnn_n
        // TODO: ADD _Irrr_nnnI_nnnn_n_nnn
        // TODO: ADD _Irrr_nnnI_r
        // TODO: ADD _Irrr_nnnI_rr
        // TODO: ADD _Irrr_nnnI_rrr
        // TODO: ADD _Irrr_nnnI_rrrr
        // TODO: ADD _Irrr_nnnI_InnnI_n_rrr
        // TODO: ADD _Irrr_nnnI_Innn_nnnI_n_rrr
        // TODO: ADD _Irrr_nnnI_Innn_rI_n_rrr
        // TODO: ADD _Irrr_nnnI_Innn_rrI_n_rrr
        // TODO: ADD _Irrr_nnnI_Innn_rrrI_n_rrr
        // TODO: ADD _Irrr_nnnI_IrrrI_n_rrr
        // TODO: ADD _Irrr_nnnI_Irrr_nnnI_n_rrr
        // TODO: ADD _Irrr_nnnI_Irrr_rI_n_rrr
        // TODO: ADD _Irrr_nnnI_Irrr_rrI_n_rrr
        // TODO: ADD _Irrr_nnnI_Irrr_rrrI_n_rrr
        // TODO: ADD _Irrr_nnnI_fr

        // TODO: ADD _Irrr_rI_nnnn_n
        // TODO: ADD _Irrr_rI_nnnn_n_nnn
        // TODO: ADD _Irrr_rI_r
        // TODO: ADD _Irrr_rI_rr
        // TODO: ADD _Irrr_rI_rrr
        // TODO: ADD _Irrr_rI_rrrr
        // TODO: ADD _Irrr_rI_InnnI_n_rrr
        // TODO: ADD _Irrr_rI_Innn_nnnI_n_rrr
        // TODO: ADD _Irrr_rI_Innn_rI_n_rrr
        // TODO: ADD _Irrr_rI_Innn_rrI_n_rrr
        // TODO: ADD _Irrr_rI_Innn_rrrI_n_rrr
        // TODO: ADD _Irrr_rI_IrrrI_n_rrr
        // TODO: ADD _Irrr_rI_Irrr_nnnI_n_rrr
        // TODO: ADD _Irrr_rI_Irrr_rI_n_rrr
        // TODO: ADD _Irrr_rI_Irrr_rrI_n_rrr
        // TODO: ADD _Irrr_rI_Irrr_rrrI_n_rrr
        // TODO: ADD _Irrr_rI_fr

        // TODO: ADD _Irrr_rrI_nnnn_n
        // TODO: ADD _Irrr_rrI_nnnn_n_nnn
        // TODO: ADD _Irrr_rrI_r
        // TODO: ADD _Irrr_rrI_rr
        // TODO: ADD _Irrr_rrI_rrr
        // TODO: ADD _Irrr_rrI_rrrr
        // TODO: ADD _Irrr_rrI_InnnI_n_rrr
        // TODO: ADD _Irrr_rrI_Innn_nnnI_n_rrr
        // TODO: ADD _Irrr_rrI_Innn_rI_n_rrr
        // TODO: ADD _Irrr_rrI_Innn_rrI_n_rrr
        // TODO: ADD _Irrr_rrI_Innn_rrrI_n_rrr
        // TODO: ADD _Irrr_rrI_IrrrI_n_rrr
        // TODO: ADD _Irrr_rrI_Irrr_nnnI_n_rrr
        // TODO: ADD _Irrr_rrI_Irrr_rI_n_rrr
        // TODO: ADD _Irrr_rrI_Irrr_rrI_n_rrr
        // TODO: ADD _Irrr_rrI_Irrr_rrrI_n_rrr
        // TODO: ADD _Irrr_rrI_fr

        // TODO: ADD _Irrr_rrrI_nnnn_n
        // TODO: ADD _Irrr_rrrI_nnnn_n_nnn
        // TODO: ADD _Irrr_rrrI_r
        // TODO: ADD _Irrr_rrrI_rr
        // TODO: ADD _Irrr_rrrI_rrr
        // TODO: ADD _Irrr_rrrI_rrrr
        // TODO: ADD _Irrr_rrrI_InnnI_n_rrr
        // TODO: ADD _Irrr_rrrI_Innn_nnnI_n_rrr
        // TODO: ADD _Irrr_rrrI_Innn_rI_n_rrr
        // TODO: ADD _Irrr_rrrI_Innn_rrI_n_rrr
        // TODO: ADD _Irrr_rrrI_Innn_rrrI_n_rrr
        // TODO: ADD _Irrr_rrrI_IrrrI_n_rrr
        // TODO: ADD _Irrr_rrrI_Irrr_nnnI_n_rrr
        // TODO: ADD _Irrr_rrrI_Irrr_rI_n_rrr
        // TODO: ADD _Irrr_rrrI_Irrr_rrI_n_rrr
        // TODO: ADD _Irrr_rrrI_Irrr_rrrI_n_rrr
        // TODO: ADD _Irrr_rrrI_fr

        #endregion

        #region ADD fr,* tests

        [Fact]
        public void ADD_fr_nnnn()   // Ok
        {
            RunTest(
                "ADD F0,6.1",
                c => c.CPU.FREGS.SetRegister(0, 10.5f),
                c => Assert.Equal(16.6f, c.CPU.FREGS.GetRegister(0), precision: 5));
        }

        [Fact]
        public void ADD_fr_r()  // Ok
        {
            RunTest(
                "ADD F0,J",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 10.5f);
                    c.CPU.REGS.J = 3;
                },
                c => Assert.Equal(13.5f, c.CPU.FREGS.GetRegister(0), precision: 5));
        }

        [Fact]
        public void ADD_fr_rr()
        {
            RunTest(
                "ADD F0,JK",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 10.5f);
                    c.CPU.REGS.JK = 1000;
                },
                c => Assert.Equal(1010.5f, c.CPU.FREGS.GetRegister(0), precision: 5));
        }

        [Fact]
        public void ADD_fr_rrr()
        {
            RunTest(
                "ADD F0,JKL",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 10.5f);
                    c.CPU.REGS.JKL = 50000;
                },
                c => Assert.Equal(50010.5f, c.CPU.FREGS.GetRegister(0), precision: 5));
        }

        [Fact]
        public void ADD_fr_rrrr()
        {
            RunTest(
                "ADD F0,JKLM",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 10.5f);
                    c.CPU.REGS.JKLM = 1000000;
                },
                c => Assert.Equal(1000010.5f, c.CPU.FREGS.GetRegister(0), precision: 5));
        }

        [Fact]
        public void ADD_fr_InnnI()
        {
            RunTest(
                "ADD F0,(0x4000)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                    c.MEMC.SetFloatToRam(0x4000, 0.75f);
                },
                c => Assert.Equal(1.75f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void ADD_fr_Innn_nnnI()
        {
            RunTest(
                "ADD F0,(0x4000+4)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                    c.MEMC.SetFloatToRam(0x4004, 0.75f);
                },
                c => Assert.Equal(1.75f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void ADD_fr_Innn_rI()
        {
            RunTest(
                "ADD F0,(0x4000+X)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                    c.CPU.REGS.X = 3;
                    c.MEMC.SetFloatToRam(0x4003, 0.75f);
                },
                c => Assert.Equal(1.75f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void ADD_fr_Innn_rrI()
        {
            RunTest(
                "ADD F0,(0x4000+WX)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                    c.CPU.REGS.WX = 5;
                    c.MEMC.SetFloatToRam(0x4005, 0.75f);
                },
                c => Assert.Equal(1.75f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void ADD_fr_Innn_rrrI()
        {
            RunTest(
                "ADD F0,(0x4000+VWX)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.SetFloatToRam(0x4007, 0.75f);
                },
                c => Assert.Equal(1.75f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void ADD_fr_IrrrI()
        {
            RunTest(
                "ADD F0,(QRS)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.SetFloatToRam(0x5000, 0.75f);
                },
                c => Assert.Equal(1.75f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void ADD_fr_Irrr_nnnI()
        {
            RunTest(
                "ADD F0,(QRS+4)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.SetFloatToRam(0x5004, 0.75f);
                },
                c => Assert.Equal(1.75f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void ADD_fr_Irrr_rI()
        {
            RunTest(
                "ADD F0,(QRS+X)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.SetFloatToRam(0x5003, 0.75f);
                },
                c => Assert.Equal(1.75f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void ADD_fr_Irrr_rrI()
        {
            RunTest(
                "ADD F0,(QRS+WX)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.SetFloatToRam(0x5005, 0.75f);
                },
                c => Assert.Equal(1.75f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void ADD_fr_Irrr_rrrI()
        {
            RunTest(
                "ADD F0,(QRS+VWX)",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 1.0f);
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.SetFloatToRam(0x5007, 0.75f);
                },
                c => Assert.Equal(1.75f, c.CPU.FREGS.GetRegister(0)));
        }

        [Fact]
        public void ADD_fr_fr()
        {
            RunTest(
                "ADD F0,F1",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 10.5f);
                    c.CPU.FREGS.SetRegister(1, 2.75f);
                },
                c => Assert.Equal(13.25f, c.CPU.FREGS.GetRegister(0), precision: 5));
        }

        #endregion
    }
}
