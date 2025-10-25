using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{

    public class TestEXEC_ADD
    {
        // ADD r, [n, r, (nnn), (rrr)]
        [Fact]
        public void TestEXEC_ADD_r_n()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 1; i < 26; i++)
            {
                computer.Clear();
                computer.CPU.REGS.Set8BitRegister(i, 100);

                string code = $@"
                    ADD {TUtils.Get8bitRegisterChar(i)},{i * 9}
                    BREAK";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                byte expectedValue = (byte)((100 + i * 9) & 0b11111111);
                byte actual = computer.CPU.REGS.Get8BitRegister(i);

                Assert.Equal(expectedValue, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_ADD_r_r()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 0; i < 26; i++)
            {
                computer.Clear();
                computer.CPU.REGS.Set8BitRegister(i, 100);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("ADD {0},{1}", TUtils.Get8bitRegisterChar(i), TUtils.Get8bitRegisterChar(i)),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                byte expectedValue = 200;

                Assert.Equal(expectedValue, computer.CPU.REGS.Get8BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_ADD_r_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 0; i < 26; i++)
            {
                computer.Clear();
                uint address = 10000;
                computer.CPU.REGS.Set8BitRegister(i, 100);
                computer.MEMC.Set8bitToRAM(address, 200);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("ADD {0},({1})", TUtils.Get8bitRegisterChar(i), address),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                byte expectedValue = 44;

                Assert.Equal(expectedValue, computer.CPU.REGS.Get8BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_ADD_r_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            uint address = 10000;
            computer.CPU.REGS.A = 200;
            computer.CPU.REGS.BCD = address;
            computer.MEMC.Set8bitToRAM(address, 200);

            cp.Build(
                TUtils.GetFormattedAsm(
                    string.Format("ADD {0},({1})", TUtils.Get8bitRegisterChar(0), TUtils.Get24bitRegisterString(1)),
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            byte expectedValue = 144;

            Assert.Equal(expectedValue, computer.CPU.REGS.A);
            TUtils.IncrementCountedTests("exec");
        }


        // ADD rr, [n.., r, (nnn), (rrr)]
        [Fact]
        public void TestEXEC_ADD_rr_n()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 0; i < 26; i++)
            {
                computer.Clear();
                computer.CPU.REGS.Set16BitRegister(i, 100);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("ADD {0},{1}", TUtils.Get16bitRegisterString(i), i * 9),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                ushort expectedValue = (ushort)((100 + i * 9) & 0xFFFFFFFF);

                Assert.Equal(expectedValue, computer.CPU.REGS.Get16BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_ADD16_rr_nn()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 0; i < 26; i++)
            {
                computer.Clear();
                computer.CPU.REGS.Set16BitRegister(i, 100);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("ADD16 {0},{1}", TUtils.Get16bitRegisterString(i), i * 2520),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                ushort expectedValue = (ushort)((100 + i * 2520) & 0xFFFFFFFF);

                Assert.Equal(expectedValue, computer.CPU.REGS.Get16BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_ADD_rr_r()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 0; i < 26; i++)
            {
                computer.Clear();
                computer.CPU.REGS.Set16BitRegister(i, 0x2001);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("ADD {0},{1}", TUtils.Get16bitRegisterString(i), TUtils.Get8bitRegisterChar(i)),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                ushort expectedValue = 0x2021;

                Assert.Equal(expectedValue, computer.CPU.REGS.Get16BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_ADD_rr_rr()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 0; i < 26; i++)
            {
                computer.Clear();
                computer.CPU.REGS.Set16BitRegister(i, 6000);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("ADD {0},{1}", TUtils.Get16bitRegisterString(i), TUtils.Get16bitRegisterString(i)),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                ushort expectedValue = 12000;

                Assert.Equal(expectedValue, computer.CPU.REGS.Get16BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_ADD_rr_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 0; i < 26; i++)
            {
                computer.Clear();
                uint address = 10000;
                computer.CPU.REGS.Set16BitRegister(i, 8000);
                computer.MEMC.Set8bitToRAM(address, 200);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("ADD {0},({1})", TUtils.Get16bitRegisterString(i), address),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                ushort expectedValue = 8200;

                Assert.Equal(expectedValue, computer.CPU.REGS.Get16BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_ADD16_rr_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 0; i < 26; i++)
            {
                computer.Clear();
                uint address = 10000;
                computer.CPU.REGS.Set16BitRegister(i, 8000);
                computer.MEMC.Set16bitToRAM(address, 2000);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("ADD16 {0},({1})", TUtils.Get16bitRegisterString(i), address),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                ushort expectedValue = 10000;

                Assert.Equal(expectedValue, computer.CPU.REGS.Get16BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_ADD_rr_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            uint address = 10000;
            computer.CPU.REGS.AB = 0x2000;
            computer.CPU.REGS.CDE = address;
            computer.MEMC.Set16bitToRAM(address, 0x2030);

            cp.Build(
                TUtils.GetFormattedAsm(
                    string.Format("ADD AB,(CDE)", TUtils.Get16bitRegisterString(0)),
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            ushort expectedValue = 0x2020;

            Assert.Equal(expectedValue, computer.CPU.REGS.AB);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_ADD16_rr_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            uint address = 10000;
            computer.CPU.REGS.AB = 0x2000;
            computer.CPU.REGS.CDE = address;
            computer.MEMC.Set16bitToRAM(address, 0x2030);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "ADD16 AB,(CDE)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            ushort expectedValue = 0x4030;

            Assert.Equal(expectedValue, computer.CPU.REGS.AB);
            TUtils.IncrementCountedTests("exec");
        }

        // ADD rrr, (n..., r, rr, rrr, (nnn)..., (rrr)...)
        [Fact]
        public void TestEXEC_ADD_rrr_n()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 0; i < 26; i++)
            {
                computer.Clear();
                computer.CPU.REGS.Set24BitRegister(i, 0x102030);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("ADD {0},{1}", TUtils.Get24bitRegisterString(i), i * 9),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint expectedValue = (uint)((0x102030 + i * 9) & 0xFFFFFFFF);

                Assert.Equal(expectedValue, computer.CPU.REGS.Get24BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_ADD16_rrr_nn()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 0; i < 26; i++)
            {
                computer.Clear();
                computer.CPU.REGS.Set24BitRegister(i, 0x102030);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("ADD16 {0},{1}", TUtils.Get24bitRegisterString(i), i * 0xCC),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint expectedValue = (uint)((0x102030 + i * 0xCC) & 0xFFFFFFFF);

                Assert.Equal(expectedValue, computer.CPU.REGS.Get24BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_ADD24_rrr_nnn()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 0; i < 26; i++)
            {
                computer.Clear();
                computer.CPU.REGS.Set24BitRegister(i, 0x102030);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("ADD24 {0},{1}", TUtils.Get24bitRegisterString(i), (uint)(i * 0xAACC)),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint expectedValue = (uint)((0x102030 + i * 0xAACC) & 0xFFFFFF);

                Assert.Equal(expectedValue, computer.CPU.REGS.Get24BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_ADD_rrr_r()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            computer.CPU.REGS.ABC = 3000;
            computer.CPU.REGS.D = 250;

            cp.Build(
                TUtils.GetFormattedAsm(
                    string.Format("ADD ABC,D"),
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            ushort expectedValue = 3250;

            Assert.Equal(expectedValue, computer.CPU.REGS.ABC);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_ADD_rrr_rr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            computer.CPU.REGS.ABC = 3000;
            computer.CPU.REGS.DE = 4000;

            cp.Build(
                TUtils.GetFormattedAsm(
                    string.Format("ADD ABC,DE"),
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            ushort expectedValue = 7000;

            Assert.Equal(expectedValue, computer.CPU.REGS.ABC);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_ADD_rrr_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            computer.CPU.REGS.ABC = 0x1000;
            computer.CPU.REGS.DEF = 0x100000;

            cp.Build(
                TUtils.GetFormattedAsm(
                    string.Format("ADD ABC,DEF"),
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint expectedValue = 0x101000;

            Assert.Equal(expectedValue, computer.CPU.REGS.ABC);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_ADD_rrr_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 0; i < 26; i++)
            {
                computer.Clear();
                uint address = 10000;
                computer.CPU.REGS.Set24BitRegister(i, 0x102030);
                computer.MEMC.Set8bitToRAM(address, 0x30);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("ADD {0},({1})", TUtils.Get24bitRegisterString(i), address),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint expectedValue = 0x102060;

                Assert.Equal(expectedValue, computer.CPU.REGS.Get24BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_ADD16_rrr_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 0; i < 26; i++)
            {
                computer.Clear();
                uint address = 10000;
                computer.CPU.REGS.Set24BitRegister(i, 0x102030);
                computer.MEMC.Set16bitToRAM(address, 0x2030);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("ADD16 {0},({1})", TUtils.Get24bitRegisterString(i), address),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint expectedValue = 0x104060;

                Assert.Equal(expectedValue, computer.CPU.REGS.Get24BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_ADD24_rrr_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 0; i < 26; i++)
            {
                computer.Clear();
                uint address = 10000;
                computer.CPU.REGS.Set24BitRegister(i, 0x102030);
                computer.MEMC.Set24bitToRAM(address, 0x102030);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("ADD24 {0},({1})", TUtils.Get24bitRegisterString(i), address),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint expectedValue = 0x204060;

                Assert.Equal(expectedValue, computer.CPU.REGS.Get24BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_ADD_rrr_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            uint address = 10000;
            computer.CPU.REGS.ABC = 0x4000;
            computer.CPU.REGS.DEF = address;
            computer.MEMC.Set24bitToRAM(address, 0x203040);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "ADD ABC,(DEF)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            ushort expectedValue = 0x4020;

            Assert.Equal(expectedValue, computer.CPU.REGS.ABC);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_ADD16_rrr_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            uint address = 10000;
            computer.CPU.REGS.ABC = 0x4000;
            computer.CPU.REGS.DEF = address;
            computer.MEMC.Set24bitToRAM(address, 0x203040);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "ADD16 ABC,(DEF)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            ushort expectedValue = 0x6030;

            Assert.Equal(expectedValue, computer.CPU.REGS.ABC);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_ADD24_rrr_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            uint address = 10000;
            computer.CPU.REGS.ABC = 0x4000;
            computer.CPU.REGS.DEF = address;
            computer.MEMC.Set24bitToRAM(address, 0x203040);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "ADD24 ABC,(DEF)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint expectedValue = 0x207040;

            Assert.Equal(expectedValue, computer.CPU.REGS.ABC);
            TUtils.IncrementCountedTests("exec");
        }

        // ADD rrrr, (n..., r, rr, rrr, (nnn)..., (rrr)...)
        [Fact]
        public void TestEXEC_ADD_rrrr_n()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 0; i < 26; i++)
            {
                computer.Clear();
                computer.CPU.REGS.Set32BitRegister(i, 0x10203040);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("ADD {0},{1}", TUtils.Get32bitRegisterString(i), i * 9 + 1),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint expectedValue = (uint)((0x10203040 + i * 9 + 1) & 0xFFFFFFFF);

                Assert.Equal(expectedValue, computer.CPU.REGS.Get32BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_ADD16_rrrr_nn()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 0; i < 26; i++)
            {
                computer.Clear();
                computer.CPU.REGS.Set32BitRegister(i, 0x10203040);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("ADD16 {0},{1}", TUtils.Get32bitRegisterString(i), (uint)(i * 0x900 + 1)),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint expectedValue = (uint)((0x10203040 + i * 0x900 + 1) & 0xFFFFFFFF);

                Assert.Equal(expectedValue, computer.CPU.REGS.Get32BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_ADD24_rrrr_nnn()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 0; i < 26; i++)
            {
                computer.Clear();
                computer.CPU.REGS.Set32BitRegister(i, 0x10203040);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("ADD24 {0},{1}", TUtils.Get32bitRegisterString(i), (uint)(i * 0x60000 + 1)),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint expectedValue = (uint)((0x10203040 + i * 0x60000 + 1) & 0xFFFFFFFF);

                Assert.Equal(expectedValue, computer.CPU.REGS.Get32BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_ADD32_rrrr_nnnn()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 0; i < 26; i++)
            {
                computer.Clear();
                computer.CPU.REGS.Set32BitRegister(i, 0x10203040);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("ADD32 {0},{1}", TUtils.Get32bitRegisterString(i), (uint)(i * 0x4000000 + 1)),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint expectedValue = (uint)((0x10203040 + i * 0x4000000 + 1) & 0xFFFFFFFF);

                Assert.Equal(expectedValue, computer.CPU.REGS.Get32BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_ADD_rrrr_r()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            computer.CPU.REGS.ABCD = 0xFFFFFFFF - 10;
            computer.CPU.REGS.E = 9;

            cp.Build(
                TUtils.GetFormattedAsm(
                    string.Format("ADD ABCD,E"),
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint expectedValue = 0xFFFFFFFE;

            Assert.Equal(expectedValue, computer.CPU.REGS.ABCD);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_ADD_rrrr_rr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            computer.CPU.REGS.ABCD = 0xFFFFFFFF - 1000;
            computer.CPU.REGS.EF = 999;

            cp.Build(
                TUtils.GetFormattedAsm(
                    string.Format("ADD ABCD,EF"),
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint expectedValue = 0xFFFFFFFE;

            Assert.Equal(expectedValue, computer.CPU.REGS.ABCD);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_ADD_rrrr_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            computer.CPU.REGS.ABCD = 0xFFFFFFFF - 100000;
            computer.CPU.REGS.EFG = 99999;

            cp.Build(
                TUtils.GetFormattedAsm(
                    string.Format("ADD ABCD,EFG"),
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint expectedValue = 0xFFFFFFFE;

            Assert.Equal(expectedValue, computer.CPU.REGS.ABCD);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_ADD_rrrr_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            computer.CPU.REGS.ABCD = 0xFFFFFFFF - 0xFFFFFF;
            computer.CPU.REGS.EFGH = 0xFFFFFE;

            cp.Build(
                TUtils.GetFormattedAsm(
                    string.Format("ADD ABCD,EFGH"),
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint expectedValue = 0xFFFFFFFE;

            Assert.Equal(expectedValue, computer.CPU.REGS.ABCD);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_ADD_rrrr_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 0; i < 26; i++)
            {
                computer.Clear();
                uint address = 10000;
                computer.CPU.REGS.Set32BitRegister(i, 0x10203040);
                computer.MEMC.Set8bitToRAM(address, 0x30);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("ADD {0},({1})", TUtils.Get32bitRegisterString(i), address),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint expectedValue = 0x10203070;

                Assert.Equal(expectedValue, computer.CPU.REGS.Get32BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_ADD16_rrrr_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 0; i < 26; i++)
            {
                computer.Clear();
                uint address = 10000;
                computer.CPU.REGS.Set32BitRegister(i, 0x10203040);
                computer.MEMC.Set32bitToRAM(address, 0x20304050);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("ADD16 {0},({1})", TUtils.Get32bitRegisterString(i), address),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint expectedValue = 0x10205070;

                Assert.Equal(expectedValue, computer.CPU.REGS.Get32BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_ADD24_rrrr_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 0; i < 26; i++)
            {
                computer.Clear();
                uint address = 10000;
                computer.CPU.REGS.Set32BitRegister(i, 0x10203040);
                computer.MEMC.Set32bitToRAM(address, 0x20304050);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("ADD24 {0},({1})", TUtils.Get32bitRegisterString(i), address),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint expectedValue = 0x10406080;

                Assert.Equal(expectedValue, computer.CPU.REGS.Get32BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_ADD32_rrrr_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 0; i < 26; i++)
            {
                computer.Clear();
                uint address = 10000;
                computer.CPU.REGS.Set32BitRegister(i, 0x10203040);
                computer.MEMC.Set32bitToRAM(address, 0x20304050);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("ADD32 {0},({1})", TUtils.Get32bitRegisterString(i), address),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint expectedValue = 0x30507090;

                Assert.Equal(expectedValue, computer.CPU.REGS.Get32BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_ADD_rrrr_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            uint address = 10000;
            computer.CPU.REGS.ABCD = 0x4000;
            computer.CPU.REGS.EFG = address;
            computer.MEMC.Set24bitToRAM(address, 0x203040);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "ADD ABCD,(EFG)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            ushort expectedValue = 0x4020;

            Assert.Equal(expectedValue, computer.CPU.REGS.ABCD);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_ADD16_rrrr_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            uint address = 10000;
            computer.CPU.REGS.ABCD = 0x4000;
            computer.CPU.REGS.EFG = address;
            computer.MEMC.Set24bitToRAM(address, 0x203040);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "ADD16 ABCD,(EFG)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            ushort expectedValue = 0x6030;

            Assert.Equal(expectedValue, computer.CPU.REGS.ABCD);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_ADD24_rrrr_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            uint address = 10000;
            computer.CPU.REGS.ABCD = 0x4000;
            computer.CPU.REGS.EFG = address;
            computer.MEMC.Set24bitToRAM(address, 0x203040);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "ADD24 ABCD,(EFG)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint expectedValue = 0x207040;

            Assert.Equal(expectedValue, computer.CPU.REGS.ABCD);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_ADD32_rrrr_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            uint address = 10000;
            computer.CPU.REGS.ABCD = 0x4000;
            computer.CPU.REGS.EFG = address;
            computer.MEMC.Set32bitToRAM(address, 0x20304050);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "ADD32 ABCD,(EFG)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint expectedValue = 0x20308050;

            Assert.Equal(expectedValue, computer.CPU.REGS.ABCD);
            TUtils.IncrementCountedTests("exec");
        }

        // ADD (nnn), (n..., r..., rr..., rrr..., (nnn)..., (rrr)...)
        [Fact]
        public void TestEXEC_ADD_InnnI_n()
        {
            Assert.True(ADD_InnnI_n(10000, 250, 10, 4));
            Assert.True(ADD_InnnI_n(10000, 200 * 256 + 100, 100, 200));
            Assert.True(ADD_InnnI_n(10000, 65536 * 20 + 256 * 10 + 7, 255, 6));
            Assert.True(ADD_InnnI_n(10000, 16777216 * 50 + 65536 * 20 + 256 * 10 + 23, 100, 123));
        }

        [Fact]
        public void TestEXEC_ADD16_InnnI_n()
        {
            Assert.True(ADD16_InnnI_nn(10000, 250, 10, 260));
            Assert.True(ADD16_InnnI_nn(10000, 200 * 256 + 100, 100, 200 * 256 + 200));
            Assert.True(ADD16_InnnI_nn(10000, 65536 * 20 + 256 * 10 + 7, 255, 256 * 11 + 6));
            Assert.True(ADD16_InnnI_nn(10000, 16777216 * 50 + 65536 * 20 + 256 * 10 + 23, 100, 256 * 10 + 123));
        }

        [Fact]
        public void TestEXEC_ADD24_InnnI_n()
        {
            Assert.True(ADD24_InnnI_nnn(10000, 250, 10, 260));
            Assert.True(ADD24_InnnI_nnn(10000, 200 * 256 + 100, 100, 200 * 256 + 200));
            Assert.True(ADD24_InnnI_nnn(10000, 65536 * 20 + 256 * 10 + 7, 255, 65536 * 20 + 256 * 11 + 6));
            Assert.True(ADD24_InnnI_nnn(10000, 16777216 * 50 + 65536 * 20 + 256 * 10 + 23, 100, 65536 * 20 + 256 * 10 + 123));
        }

        [Fact]
        public void TestEXEC_ADD32_InnnI_n()
        {
            Assert.True(ADD32_InnnI_nnnn(10000, 250, 10, 260));
            Assert.True(ADD32_InnnI_nnnn(10000, 200 * 256 + 100, 100, 200 * 256 + 200));
            Assert.True(ADD32_InnnI_nnnn(10000, 65536 * 20 + 256 * 10 + 7, 255, 65536 * 20 + 256 * 11 + 6));
            Assert.True(ADD32_InnnI_nnnn(10000, 16777216 * 50 + 65536 * 20 + 256 * 10 + 23, 100, 16777216 * 50 + 65536 * 20 + 256 * 10 + 123));
        }


        [Fact]
        public void TestEXEC_ADD_InnnI_r()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 10000;
            byte initialValue = 200;

            for (byte i = 0; i < 26; i++)
            {
                byte addValue = (byte)(i * 9);
                byte expectedValue = (byte)(addValue + initialValue);
                computer.Clear();

                computer.MEMC.Set8bitToRAM(address, initialValue);

                string code = TUtils.GetFormattedAsm(
                        $"LD {TUtils.Get8bitRegisterChar(i)},{addValue}",
                        $"ADD ({address}),{TUtils.Get8bitRegisterChar(i)}",
                        "BREAK"
                    );

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint actualValueBelow = computer.MEMC.Get32bitFromRAM(address - 3);
                uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 1);

                TUtils.IncrementCountedTests("exec");
                // We are making sure nothing "bleeds-out" onto other memory locations
                Assert.Equal(expectedValue, actualValueBelow);
                Assert.True((expectedValue == actualValueBelow) && (actualValueAbove == 0));
            }
        }

        [Fact]
        public void TestEXEC_ADD16_InnnI_r()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 30;
            ushort initialValue = 200 * 0x100 + 100;

            for (byte i = 0; i < 26; i++)
            {
                byte addValue = (byte)(i * 9 + 1);
                ushort expectedValue = (ushort)(addValue + initialValue);
                computer.Clear();

                computer.MEMC.Set16bitToRAM(address, initialValue);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("LD {0},{1}", TUtils.Get8bitRegisterChar(i), addValue),
                        string.Format("ADD16 ({0}),{1}", address, TUtils.Get8bitRegisterChar(i)),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint actualValueBelow = computer.MEMC.Get32bitFromRAM(address - 2);
                uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 2);

                TUtils.IncrementCountedTests("exec");
                // We are making sure nothing "bleeds-out" onto other memory locations
                Assert.True((expectedValue == actualValueBelow) && (actualValueAbove == 0));
            }
        }

        [Fact]
        public void TestEXEC_ADD24_InnnI_r()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 10000;
            uint initialValue = 50 * 0x10000 + 200 * 0x100 + 100;

            for (byte i = 0; i < 26; i++)
            {
                byte addValue = (byte)(i * 9 + 1);
                uint expectedValue = addValue + initialValue;
                computer.Clear();

                computer.MEMC.Set24bitToRAM(address, initialValue);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("LD {0},{1}", TUtils.Get8bitRegisterChar(i), addValue),
                        string.Format("ADD24 ({0}),{1}", address, TUtils.Get8bitRegisterChar(i)),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint actualValueBelow = computer.MEMC.Get32bitFromRAM(address - 1);
                uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 3);

                TUtils.IncrementCountedTests("exec");
                // We are making sure nothing "bleeds-out" onto other memory locations
                Assert.True((expectedValue == actualValueBelow) && (actualValueAbove == 0));
            }
        }

        [Fact]
        public void TestEXEC_ADD32_InnnI_r()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 30;
            uint initialValue = 10 * 0x1000000 + 50 * 0x10000 + 200 * 0x100 + 100;

            for (byte i = 0; i < 26; i++)
            {
                byte addValue = (byte)(i * 9 + 1);
                uint expectedValue = addValue + initialValue;
                computer.Clear();

                computer.MEMC.Set32bitToRAM(address, initialValue);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("LD {0},{1}", TUtils.Get8bitRegisterChar(i), addValue),
                        string.Format("ADD32 ({0}),{1}", address, TUtils.Get8bitRegisterChar(i)),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint actualValueBelow = computer.MEMC.Get32bitFromRAM(address);
                uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 4);

                TUtils.IncrementCountedTests("exec");
                // We are making sure nothing "bleeds-out" onto other memory locations
                Assert.True((expectedValue == actualValueBelow) && (actualValueAbove == 0));
            }
        }

        [Fact]
        public void TestEXEC_ADD16_InnnI_rr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 10000;

            ushort initialValue = 200 * 256 + 100;

            for (byte i = 0; i < 26; i++)
            {
                ushort addValue = (ushort)(i * 1000);
                ushort expectedValue = (ushort)(addValue + initialValue);
                computer.Clear();

                computer.MEMC.Set16bitToRAM(address, initialValue);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("LD {0},{1}", TUtils.Get16bitRegisterString(i), addValue),
                        string.Format("ADD16 ({0}),{1}", address, TUtils.Get16bitRegisterString(i)),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint actualValueBelow = computer.MEMC.Get32bitFromRAM(address - 4);
                uint actualValue = computer.MEMC.Get16bitFromRAM(address);
                uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 2);

                TUtils.IncrementCountedTests("exec");
                // We are making sure nothing "bleeds-out" onto other memory locations
                Assert.True((actualValueBelow == 0) && (actualValue == expectedValue) && (actualValueAbove == 0));

            }
        }

        [Fact]
        public void TestEXEC_ADD24_InnnI_rr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 10000;

            uint initialValue = 100 * 0x10000 + 200 * 256 + 100;

            for (byte i = 0; i < 26; i++)
            {
                ushort addValue = (ushort)(i * 1000);
                uint expectedValue = (uint)(addValue + initialValue);
                computer.Clear();

                computer.MEMC.Set24bitToRAM(address, initialValue);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("LD {0},{1}", TUtils.Get16bitRegisterString(i), addValue),
                        string.Format("ADD24 ({0}),{1}", address, TUtils.Get16bitRegisterString(i)),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint actualValueBelow = computer.MEMC.Get32bitFromRAM(address - 4);
                uint actualValue = computer.MEMC.Get24bitFromRAM(address);
                uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 3);

                TUtils.IncrementCountedTests("exec");
                // We are making sure nothing "bleeds-out" onto other memory locations
                Assert.True((actualValueBelow == 0) && (actualValue == expectedValue) && (actualValueAbove == 0));

            }
        }

        [Fact]
        public void TestEXEC_ADD32_InnnI_rr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 10000;

            uint initialValue = 50 * 0x1000000 + 100 * 0x10000 + 200 * 256 + 100;

            for (byte i = 0; i < 26; i++)
            {
                ushort addValue = (ushort)(i * 1000);
                uint expectedValue = (uint)(addValue + initialValue);
                computer.Clear();

                computer.MEMC.Set32bitToRAM(address, initialValue);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("LD {0},{1}", TUtils.Get16bitRegisterString(i), addValue),
                        string.Format("ADD32 ({0}),{1}", address, TUtils.Get16bitRegisterString(i)),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint actualValueBelow = computer.MEMC.Get32bitFromRAM(address - 4);
                uint actualValue = computer.MEMC.Get32bitFromRAM(address);
                uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 4);

                TUtils.IncrementCountedTests("exec");
                // We are making sure nothing "bleeds-out" onto other memory locations
                Assert.True((actualValueBelow == 0) && (actualValue == expectedValue) && (actualValueAbove == 0));

            }
        }

        [Fact]
        public void TestEXEC_ADD24_InnnI_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 10000;

            uint initialValue = 100 * 0x10000 + 200 * 0x100 + 100;

            for (byte i = 0; i < 26; i++)
            {
                uint addValue = (uint)(i * 0x1FFFF);
                uint expectedValue = addValue + initialValue;
                computer.Clear();

                computer.MEMC.Set24bitToRAM(address, initialValue);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("LD {0},{1}", TUtils.Get24bitRegisterString(i), addValue),
                        string.Format("ADD24 ({0}),{1}", address, TUtils.Get24bitRegisterString(i)),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint actualValueBelow = computer.MEMC.Get32bitFromRAM(address - 4);
                uint actualValue = computer.MEMC.Get24bitFromRAM(address);
                uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 3);

                TUtils.IncrementCountedTests("exec");
                // We are making sure nothing "bleeds-out" onto other memory locations
                Assert.True((actualValueBelow == 0) && (actualValue == expectedValue) && (actualValueAbove == 0));

            }
        }

        [Fact]
        public void TestEXEC_ADD32_InnnI_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 10000;

            uint initialValue = 50 * 0x1000000 + 100 * 0x10000 + 200 * 0x100 + 100;

            for (byte i = 0; i < 26; i++)
            {
                uint addValue = (uint)(i * 0x1FFFF);
                uint expectedValue = addValue + initialValue;
                computer.Clear();

                computer.MEMC.Set32bitToRAM(address, initialValue);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("LD {0},{1}", TUtils.Get24bitRegisterString(i), addValue),
                        string.Format("ADD32 ({0}),{1}", address, TUtils.Get24bitRegisterString(i)),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint actualValueBelow = computer.MEMC.Get32bitFromRAM(address - 4);
                uint actualValue = computer.MEMC.Get32bitFromRAM(address);
                uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 4);

                TUtils.IncrementCountedTests("exec");
                // We are making sure nothing "bleeds-out" onto other memory locations
                Assert.True((actualValueBelow == 0) && (actualValue == expectedValue) && (actualValueAbove == 0));

            }
        }

        [Fact]
        public void TestEXEC_ADD_InnnI_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 10000;

            uint initialValue = 50 * 0x1000000 + 100 * 0x10000 + 200 * 0x100 + 100;

            for (byte i = 0; i < 26; i++)
            {
                uint addValue = (uint)(i * 0x1FFFFFF);
                uint expectedValue = addValue + initialValue;
                computer.Clear();

                computer.MEMC.Set32bitToRAM(address, initialValue);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("LD {0},{1}", TUtils.Get32bitRegisterString(i), addValue),
                        string.Format("ADD ({0}),{1}", address, TUtils.Get32bitRegisterString(i)),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint actualValueBelow = computer.MEMC.Get32bitFromRAM(address - 4);
                uint actualValue = computer.MEMC.Get32bitFromRAM(address);
                uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 4);

                TUtils.IncrementCountedTests("exec");
                // We are making sure nothing "bleeds-out" onto other memory locations
                Assert.True((actualValueBelow == 0) && (actualValue == expectedValue) && (actualValueAbove == 0));
            }
        }


        // ADD (rrr), (n..., r..., rr..., rrr..., (nnn)..., (rrr)...)
        [Fact]
        public void TestEXEC_ADD_IrrrI_n()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 10000;

            byte initialValue = 100;

            for (byte i = 0; i < 26; i++)
            {
                byte addValue = (byte)(i * 9 + 1);
                uint expectedValue = (byte)(addValue + initialValue);
                computer.Clear();

                computer.MEMC.Set8bitToRAM(address, initialValue);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("LD {0},{1}", TUtils.Get24bitRegisterString(i), address),
                        string.Format("ADD ({0}),{1}", TUtils.Get24bitRegisterString(i), addValue),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint actualValueBelow = computer.MEMC.Get32bitFromRAM(address - 3);
                uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 1);

                TUtils.IncrementCountedTests("exec");
                // We are making sure nothing "bleeds-out" onto other memory locations
                Assert.True((expectedValue == actualValueBelow) && (actualValueAbove == 0));
            }
        }

        [Fact]
        public void TestEXEC_ADD16_IrrrI_n()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 10000;

            ushort initialValue = (ushort)(200 * 0x100 + 100);

            for (byte i = 0; i < 26; i++)
            {
                ushort addValue = (ushort)(i * 0x2FF + 1);
                uint expectedValue = (ushort)(addValue + initialValue);
                computer.Clear();

                computer.MEMC.Set16bitToRAM(address, initialValue);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("LD {0},{1}", TUtils.Get24bitRegisterString(i), address),
                        string.Format("ADD16 ({0}),{1}", TUtils.Get24bitRegisterString(i), addValue),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint actualValueBelow = computer.MEMC.Get32bitFromRAM(address - 2);
                uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 2);

                TUtils.IncrementCountedTests("exec");
                // We are making sure nothing "bleeds-out" onto other memory locations
                Assert.True((expectedValue == actualValueBelow) && (actualValueAbove == 0));
            }
        }

        [Fact]
        public void TestEXEC_ADD24_IrrrI_n()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 10000;

            uint initialValue = 100 * 0x10000 + 200 * 0x100 + 100;

            for (byte i = 0; i < 26; i++)
            {
                uint addValue = (uint)(i * 0x12FF + 1);
                uint expectedValue = addValue + initialValue;
                computer.Clear();

                computer.MEMC.Set24bitToRAM(address, initialValue);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("LD {0},{1}", TUtils.Get24bitRegisterString(i), address),
                        string.Format("ADD24 ({0}),{1}", TUtils.Get24bitRegisterString(i), addValue),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint actualValueBelow = computer.MEMC.Get32bitFromRAM(address - 1);
                uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 3);

                TUtils.IncrementCountedTests("exec");
                // We are making sure nothing "bleeds-out" onto other memory locations
                Assert.True((expectedValue == actualValueBelow) && (actualValueAbove == 0));
            }
        }

        [Fact]
        public void TestEXEC_ADD32_IrrrI_n()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 10000;

            uint initialValue = 50 * 0x1000000 + 100 * 0x10000 + 200 * 0x100 + 100;

            for (byte i = 0; i < 26; i++)
            {
                uint addValue = (uint)(i * 0x212FF + 1);
                uint expectedValue = addValue + initialValue;
                computer.Clear();

                computer.MEMC.Set32bitToRAM(address, initialValue);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("LD {0},{1}", TUtils.Get24bitRegisterString(i), address),
                        string.Format("ADD32 ({0}),{1}", TUtils.Get24bitRegisterString(i), addValue),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint actualValueBelow = computer.MEMC.Get32bitFromRAM(address);
                uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 4);

                TUtils.IncrementCountedTests("exec");
                // We are making sure nothing "bleeds-out" onto other memory locations
                Assert.True((expectedValue == actualValueBelow) && (actualValueAbove == 0));
            }
        }

        [Fact]
        public void TestEXEC_ADD_IrrrI_r()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 10000;

            byte initialValue = 200;
            byte addValue = 100;
            byte expectedValue = (byte)(initialValue + addValue);
            computer.Clear();

            computer.MEMC.Set8bitToRAM(address, initialValue);

            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.Z = addValue;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "ADD (ABC),Z",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint actualValueBelow = computer.MEMC.Get32bitFromRAM(address - 3);
            uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 1);

            TUtils.IncrementCountedTests("exec");
            // We are making sure nothing "bleeds-out" onto other memory locations
            Assert.True((expectedValue == actualValueBelow) && (actualValueAbove == 0));
        }

        [Fact]
        public void TestEXEC_ADD16_IrrrI_r()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 10000;

            ushort initialValue = 200 * 0x100 + 100;
            byte addValue = 100;
            ushort expectedValue = (ushort)(initialValue + addValue);
            computer.Clear();

            computer.MEMC.Set16bitToRAM(address, initialValue);

            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.Z = addValue;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "ADD16 (ABC),Z",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint actualValueBelow = computer.MEMC.Get32bitFromRAM(address - 2);
            uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 2);

            TUtils.IncrementCountedTests("exec");
            // We are making sure nothing "bleeds-out" onto other memory locations
            Assert.True((expectedValue == actualValueBelow) && (actualValueAbove == 0));
        }

        [Fact]
        public void TestEXEC_ADD24_IrrrI_r()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 10000;

            uint initialValue = 100 * 0x10000 + 200 * 0x100 + 100;
            byte addValue = 100;
            uint expectedValue = initialValue + addValue;
            computer.Clear();

            computer.MEMC.Set24bitToRAM(address, initialValue);

            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.Z = addValue;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "ADD24 (ABC),Z",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint actualValueBelow = computer.MEMC.Get32bitFromRAM(address - 1);
            uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 3);

            TUtils.IncrementCountedTests("exec");
            // We are making sure nothing "bleeds-out" onto other memory locations
            Assert.True((expectedValue == actualValueBelow) && (actualValueAbove == 0));
        }

        [Fact]
        public void TestEXEC_ADD32_IrrrI_r()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 10000;

            uint initialValue = 50 * 0x1000000 + 100 * 0x10000 + 200 * 0x100 + 100;
            byte addValue = 100;
            uint expectedValue = initialValue + addValue;
            computer.Clear();

            computer.MEMC.Set32bitToRAM(address, initialValue);

            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.Z = addValue;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "ADD32 (ABC),Z",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint actualValueBelow = computer.MEMC.Get32bitFromRAM(address);
            uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 4);

            TUtils.IncrementCountedTests("exec");
            // We are making sure nothing "bleeds-out" onto other memory locations
            Assert.True((expectedValue == actualValueBelow) && (actualValueAbove == 0));
        }

        [Fact]
        public void TestEXEC_ADD16_IrrrI_rr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 10000;

            ushort initialValue = 230 * 0x100 + 100;
            ushort addValue = 45 * 0x100 + 100;
            ushort expectedValue = (ushort)(initialValue + addValue);
            computer.Clear();

            computer.MEMC.Set16bitToRAM(address, initialValue);

            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.YZ = addValue;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "ADD16 (ABC),YZ",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint actualValueBelow = computer.MEMC.Get32bitFromRAM(address - 2);
            uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 2);

            TUtils.IncrementCountedTests("exec");
            // We are making sure nothing "bleeds-out" onto other memory locations
            Assert.True((expectedValue == actualValueBelow) && (actualValueAbove == 0));
        }

        [Fact]
        public void TestEXEC_ADD24_IrrrI_rr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 10000;

            uint initialValue = 100 * 0x10000 + 230 * 0x100 + 100;
            ushort addValue = 45 * 0x100 + 100;
            uint expectedValue = initialValue + addValue;
            computer.Clear();

            computer.MEMC.Set24bitToRAM(address, initialValue);

            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.YZ = addValue;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "ADD24 (ABC),YZ",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint actualValueBelow = computer.MEMC.Get32bitFromRAM(address - 1);
            uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 3);

            TUtils.IncrementCountedTests("exec");
            // We are making sure nothing "bleeds-out" onto other memory locations
            Assert.True((expectedValue == actualValueBelow) && (actualValueAbove == 0));
        }

        [Fact]
        public void TestEXEC_ADD32_IrrrI_rr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 10000;

            uint initialValue = 50 * 0x1000000 + 100 * 0x10000 + 230 * 0x100 + 100;
            ushort addValue = 45 * 0x100 + 100;
            uint expectedValue = initialValue + addValue;
            computer.Clear();

            computer.MEMC.Set32bitToRAM(address, initialValue);

            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.YZ = addValue;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "ADD32 (ABC),YZ",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint actualValueBelow = computer.MEMC.Get32bitFromRAM(address);
            uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 4);

            TUtils.IncrementCountedTests("exec");
            // We are making sure nothing "bleeds-out" onto other memory locations
            Assert.True((expectedValue == actualValueBelow) && (actualValueAbove == 0));
        }

        [Fact]
        public void TestEXEC_ADD24_IrrrI_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 10000;

            uint initialValue = 100 * 0x10000 + 230 * 0x100 + 100;
            uint addValue = 80 * 0x10000 + 45 * 0x100 + 100;
            uint expectedValue = initialValue + addValue;
            computer.Clear();

            computer.MEMC.Set24bitToRAM(address, initialValue);

            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.XYZ = addValue;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "ADD24 (ABC), XYZ",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint actualValueBelow = computer.MEMC.Get32bitFromRAM(address - 1);
            uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 3);

            TUtils.IncrementCountedTests("exec");
            // We are making sure nothing "bleeds-out" onto other memory locations
            Assert.True((expectedValue == actualValueBelow) && (actualValueAbove == 0));
        }

        [Fact]
        public void TestEXEC_ADD32_IrrrI_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 10000;

            uint initialValue = 50 * 0x1000000 + 100 * 0x10000 + 230 * 0x100 + 100;
            uint addValue = 80 * 0x10000 + 45 * 0x100 + 100;
            uint expectedValue = initialValue + addValue;
            computer.Clear();

            computer.MEMC.Set32bitToRAM(address, initialValue);

            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.XYZ = addValue;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "ADD32 (ABC), XYZ",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint actualValueBelow = computer.MEMC.Get32bitFromRAM(address);
            uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 4);

            TUtils.IncrementCountedTests("exec");
            // We are making sure nothing "bleeds-out" onto other memory locations
            Assert.True((expectedValue == actualValueBelow) && (actualValueAbove == 0));
        }

        [Fact]
        public void TestEXEC_ADD_IrrrI_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 10000;

            uint initialValue = 50 * 0x1000000 + 100 * 0x10000 + 230 * 0x100 + 100;
            uint addValue = 100 * 0x1000000 + 80 * 0x10000 + 45 * 0x100 + 100;
            uint expectedValue = initialValue + addValue;
            computer.Clear();

            computer.MEMC.Set32bitToRAM(address, initialValue);

            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.WXYZ = addValue;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "ADD (ABC), WXYZ",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint actualValueBelow = computer.MEMC.Get32bitFromRAM(address);
            uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 4);

            TUtils.IncrementCountedTests("exec");
            // We are making sure nothing "bleeds-out" onto other memory locations
            Assert.True((expectedValue == actualValueBelow) && (actualValueAbove == 0));
        }

        // Memory additions
        [Fact]
        public void TestEXEC_ADD_InnnI_InnnI()
        {
            Assert.True(ADD_InnnI_InnnI(10000, 250, 20000, 10, 4));
            Assert.True(ADD_InnnI_InnnI(10000, 200 * 256 + 100, 20000, 100, 200));
            Assert.True(ADD_InnnI_InnnI(10000, 65536 * 20 + 256 * 10 + 7, 20000, 255, 6));
            Assert.True(ADD_InnnI_InnnI(10000, 16777216 * 50 + 65536 * 20 + 256 * 10 + 23, 20000, 100, 123));
        }

        [Fact]
        public void TestEXEC_ADD16_InnnI_InnnI()
        {
            Assert.True(ADD16_InnnI_InnnI(10000, 250, 20000, 10, 260));
            Assert.True(ADD16_InnnI_InnnI(10000, 200 * 256 + 100, 20000, 100, 200 * 256 + 200));
            Assert.True(ADD16_InnnI_InnnI(10000, 65536 * 20 + 256 * 10 + 7, 20000, 255, 256 * 11 + 6));
            Assert.True(ADD16_InnnI_InnnI(10000, 16777216 * 50 + 65536 * 20 + 256 * 10 + 23, 20000, 100, 256 * 10 + 123));
        }

        [Fact]
        public void TestEXEC_ADD24_InnnI_InnnI()
        {
            Assert.True(ADD24_InnnI_InnnI(10000, 250, 20000, 10, 260));
            Assert.True(ADD24_InnnI_InnnI(10000, 200 * 256 + 100, 20000, 100, 200 * 256 + 200));
            Assert.True(ADD24_InnnI_InnnI(10000, 65536 * 20 + 256 * 10 + 7, 20000, 255, 65536 * 20 + 256 * 11 + 6));
            Assert.True(ADD24_InnnI_InnnI(10000, 16777216 * 50 + 65536 * 20 + 256 * 10 + 23, 20000, 100, 65536 * 20 + 256 * 10 + 123));
        }

        [Fact]
        public void TestEXEC_ADD32_InnnI_InnnI()
        {
            Assert.True(ADD32_InnnI_InnnI(10000, 250, 20000, 10, 260));
            Assert.True(ADD32_InnnI_InnnI(10000, 200 * 256 + 100, 20000, 100, 200 * 256 + 200));
            Assert.True(ADD32_InnnI_InnnI(10000, 65536 * 20 + 256 * 10 + 7, 20000, 255, 65536 * 20 + 256 * 11 + 6));
            Assert.True(ADD32_InnnI_InnnI(10000, 16777216 * 50 + 65536 * 20 + 256 * 10 + 23, 20000, 100, 16777216 * 50 + 65536 * 20 + 256 * 10 + 123));
        }


        // Test support
        private bool ADD_InnnI_n(uint address, uint initialValue, uint addValue, uint expectedValue)
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();

            computer.MEMC.Set8bitToRAM(address, (byte)initialValue);

            cp.Build(
                TUtils.GetFormattedAsm(
                    string.Format("ADD ({0}),{1}", address, addValue),
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint actualValueBelow = computer.MEMC.Get32bitFromRAM(address - 3);
            uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 1);

            TUtils.IncrementCountedTests("exec");
            // We are making sure nothing "bleeds-out" onto other memory locations
            return (expectedValue == actualValueBelow) && (actualValueAbove == 0);
        }

        private bool ADD16_InnnI_nn(uint address, uint initialValue, uint addValue, uint expectedValue)
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();

            computer.MEMC.Set16bitToRAM(address, (ushort)initialValue);

            cp.Build(
                TUtils.GetFormattedAsm(
                    string.Format("ADD16 ({0}),{1}", address, addValue),
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint actualValueBelow = computer.MEMC.Get32bitFromRAM(address - 4);
            uint actualValue = computer.MEMC.Get16bitFromRAM(address);
            uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 2);

            TUtils.IncrementCountedTests("exec");
            // We are making sure nothing "bleeds-out" onto other memory locations
            return (actualValueBelow == 0) && (actualValue == expectedValue) && (actualValueAbove == 0);
        }

        private bool ADD24_InnnI_nnn(uint address, uint initialValue, uint addValue, uint expectedValue)
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();

            computer.MEMC.Set24bitToRAM(address, initialValue);

            cp.Build(
                TUtils.GetFormattedAsm(
                    string.Format("ADD24 ({0}),{1}", address, addValue),
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint actualValueBelow = computer.MEMC.Get32bitFromRAM(address - 4);
            uint actualValue = computer.MEMC.Get24bitFromRAM(address);
            uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 3);

            TUtils.IncrementCountedTests("exec");
            // We are making sure nothing "bleeds-out" onto other memory locations
            return (actualValueBelow == 0) && (actualValue == expectedValue) && (actualValueAbove == 0);
        }

        private bool ADD32_InnnI_nnnn(uint address, uint initialValue, uint value, uint expectedValue)
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();

            computer.MEMC.Set32bitToRAM(address, initialValue);

            cp.Build(
                TUtils.GetFormattedAsm(
                    string.Format("ADD32 ({0}),{1}", address, value),
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint actualValueBelow = computer.MEMC.Get32bitFromRAM(address - 4);
            uint actualValue = computer.MEMC.Get32bitFromRAM(address);
            uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 4);

            TUtils.IncrementCountedTests("exec");
            // We are making sure nothing "bleeds-out" onto other memory locations
            return (actualValueBelow == 0) && (actualValue == expectedValue) && (actualValueAbove == 0);
        }

        // Memory to memory
        private bool ADD_InnnI_InnnI(uint address1, uint value1, uint address2, uint value2, uint expectedValue)
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();

            computer.MEMC.Set8bitToRAM(address1, (byte)value1);
            computer.MEMC.Set8bitToRAM(address2, (byte)value2);

            cp.Build(
                TUtils.GetFormattedAsm(
                    string.Format("ADD ({0}),({1})", address1, address2),
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint actualValue1Below = computer.MEMC.Get8bitFromRAM(address1 - 1);
            uint actualValue1 = computer.MEMC.Get8bitFromRAM(address1);
            uint actualValue1Above = computer.MEMC.Get32bitFromRAM(address1 + 1);

            TUtils.IncrementCountedTests("exec");
            // We are making sure nothing "bleeds-out" onto other memory locations
            return (expectedValue == actualValue1) && (actualValue1Below == 0 && actualValue1Above == 0);
        }

        private bool ADD16_InnnI_InnnI(uint address1, uint value1, uint address2, uint value2, uint expectedValue)
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();

            computer.MEMC.Set16bitToRAM(address1, (ushort)value1);
            computer.MEMC.Set16bitToRAM(address2, (ushort)value2);

            cp.Build(
                TUtils.GetFormattedAsm(
                    string.Format("ADD16 ({0}),({1})", address1, address2),
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint actualValue1Below = computer.MEMC.Get8bitFromRAM(address1 - 1);
            uint actualValue1 = computer.MEMC.Get16bitFromRAM(address1);
            uint actualValue1Above = computer.MEMC.Get32bitFromRAM(address1 + 2);

            TUtils.IncrementCountedTests("exec");
            // We are making sure nothing "bleeds-out" onto other memory locations
            return (expectedValue == actualValue1) && (actualValue1Below == 0 && actualValue1Above == 0);
        }

        private bool ADD24_InnnI_InnnI(uint address1, uint value1, uint address2, uint value2, uint expectedValue)
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();

            computer.MEMC.Set24bitToRAM(address1, value1);
            computer.MEMC.Set24bitToRAM(address2, value2);

            cp.Build(
                TUtils.GetFormattedAsm(
                    string.Format("ADD24 ({0}),({1})", address1, address2),
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint actualValue1Below = computer.MEMC.Get8bitFromRAM(address1 - 1);
            uint actualValue1 = computer.MEMC.Get24bitFromRAM(address1);
            uint actualValue1Above = computer.MEMC.Get32bitFromRAM(address1 + 3);

            TUtils.IncrementCountedTests("exec");
            // We are making sure nothing "bleeds-out" onto other memory locations
            return (expectedValue == actualValue1) && (actualValue1Below == 0 && actualValue1Above == 0);
        }

        private bool ADD32_InnnI_InnnI(uint address1, uint value1, uint address2, uint value2, uint expectedValue)
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();

            computer.MEMC.Set32bitToRAM(address1, value1);
            computer.MEMC.Set32bitToRAM(address2, value2);

            cp.Build(
                TUtils.GetFormattedAsm(
                    string.Format("ADD32 ({0}),({1})", address1, address2),
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint actualValue1Below = computer.MEMC.Get8bitFromRAM(address1 - 1);
            uint actualValue1 = computer.MEMC.Get32bitFromRAM(address1);
            uint actualValue1Above = computer.MEMC.Get32bitFromRAM(address1 + 4);

            TUtils.IncrementCountedTests("exec");
            // We are making sure nothing "bleeds-out" onto other memory locations
            return (expectedValue == actualValue1) && (actualValue1Below == 0 && actualValue1Above == 0);
        }
    }
}
