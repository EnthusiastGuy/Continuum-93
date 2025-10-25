using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{

    public class TestEXEC_SUB
    {
        // SUB r, [n, r, (nnn), (rrr)]
        [Fact]
        public void TestEXEC_SUB_r_n()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 0; i < 26; i++)
            {
                computer.Clear();
                computer.CPU.REGS.Set8BitRegister(i, 100);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("SUB {0},{1}", TUtils.Get8bitRegisterChar(i), i * 9),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                byte expectedValue = (byte)((100 - i * 9) & 0b11111111);

                Assert.Equal(expectedValue, computer.CPU.REGS.Get8BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_SUB_r_r()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 0; i < 26; i++)
            {
                computer.Clear();
                computer.CPU.REGS.Set8BitRegister(i, 100);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("SUB {0},{1}", TUtils.Get8bitRegisterChar(i), TUtils.Get8bitRegisterChar(i)),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                byte expectedValue = 0;

                Assert.Equal(expectedValue, computer.CPU.REGS.Get8BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_SUB_r_InnnI()
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
                        string.Format("SUB {0},({1})", TUtils.Get8bitRegisterChar(i), address),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                byte expectedValue = 156;

                Assert.Equal(expectedValue, computer.CPU.REGS.Get8BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_SUB_r_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            uint address = 10000;
            computer.CPU.REGS.A = 255;
            computer.CPU.REGS.BCD = address;
            computer.MEMC.Set8bitToRAM(address, 200);

            cp.Build(
                TUtils.GetFormattedAsm(
                    string.Format("SUB {0},({1})", TUtils.Get8bitRegisterChar(0), TUtils.Get24bitRegisterString(1)),
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            byte expectedValue = 55;

            Assert.Equal(expectedValue, computer.CPU.REGS.A);
            TUtils.IncrementCountedTests("exec");
        }

        // SUB rr, [n.., r, (nnn), (rrr)]
        [Fact]
        public void TestEXEC_SUB_rr_n()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 0; i < 26; i++)
            {
                computer.Clear();
                computer.CPU.REGS.Set16BitRegister(i, 100);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("SUB {0},{1}", TUtils.Get16bitRegisterString(i), i * 9),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                ushort expectedValue = (ushort)((100 - i * 9) & 0xFFFFFFFF);

                Assert.Equal(expectedValue, computer.CPU.REGS.Get16BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_SUB16_rr_nn()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 0; i < 26; i++)
            {
                computer.Clear();
                computer.CPU.REGS.Set16BitRegister(i, 100);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("SUB16 {0},{1}", TUtils.Get16bitRegisterString(i), i * 2520),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                ushort expectedValue = (ushort)((100 - i * 2520) & 0xFFFFFFFF);

                Assert.Equal(expectedValue, computer.CPU.REGS.Get16BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_SUB_rr_r()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 0; i < 26; i++)
            {
                computer.Clear();
                computer.CPU.REGS.Set16BitRegister(i, 0x1010);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("SUB {0},{1}", TUtils.Get16bitRegisterString(i), TUtils.Get8bitRegisterChar(i)),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                ushort expectedValue = 0x1000;

                Assert.Equal(expectedValue, computer.CPU.REGS.Get16BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_SUB_rr_rr()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 0; i < 26; i++)
            {
                computer.Clear();
                computer.CPU.REGS.Set16BitRegister(i, 6000);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("SUB {0},{1}", TUtils.Get16bitRegisterString(i), TUtils.Get16bitRegisterString(i)),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                ushort expectedValue = 0;

                Assert.Equal(expectedValue, computer.CPU.REGS.Get16BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_SUB_rr_InnnI()
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
                        string.Format("SUB {0},({1})", TUtils.Get16bitRegisterString(i), address),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                ushort expectedValue = 7800;

                Assert.Equal(expectedValue, computer.CPU.REGS.Get16BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_SUB16_rr_InnnI()
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
                        string.Format("SUB16 {0},({1})", TUtils.Get16bitRegisterString(i), address),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                ushort expectedValue = 6000;

                Assert.Equal(expectedValue, computer.CPU.REGS.Get16BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_SUB_rr_IrrrI()
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
                    string.Format("SUB AB,(CDE)", TUtils.Get16bitRegisterString(0)),
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            ushort expectedValue = 0x1FE0;

            Assert.Equal(expectedValue, computer.CPU.REGS.AB);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SUB16_rr_IrrrI()
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
                    "SUB16 AB,(CDE)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            ushort expectedValue = 0xFFD0;

            Assert.Equal(expectedValue, computer.CPU.REGS.AB);
            TUtils.IncrementCountedTests("exec");
        }

        // SUB rrr, (n..., r, rr, rrr, (nnn)..., (rrr)...)
        [Fact]
        public void TestEXEC_SUB_rrr_n()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 0; i < 26; i++)
            {
                computer.Clear();
                computer.CPU.REGS.Set24BitRegister(i, 0x102030);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("SUB {0},{1}", TUtils.Get24bitRegisterString(i), i * 9),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint expectedValue = (uint)((0x102030 - i * 9) & 0xFFFFFFFF);

                Assert.Equal(expectedValue, computer.CPU.REGS.Get24BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_SUB16_rrr_nn()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 0; i < 26; i++)
            {
                computer.Clear();
                computer.CPU.REGS.Set24BitRegister(i, 0x102030);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("SUB16 {0},{1}", TUtils.Get24bitRegisterString(i), i * 0xCC),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint expectedValue = (uint)((0x102030 - i * 0xCC) & 0xFFFFFFFF);

                Assert.Equal(expectedValue, computer.CPU.REGS.Get24BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_SUB24_rrr_nnn()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 0; i < 26; i++)
            {
                computer.Clear();
                computer.CPU.REGS.Set24BitRegister(i, 0x102030);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("SUB24 {0},{1}", TUtils.Get24bitRegisterString(i), (uint)(i * 0xAACC)),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint expectedValue = (uint)((0x102030 - i * 0xAACC) & 0xFFFFFF);

                Assert.Equal(expectedValue, computer.CPU.REGS.Get24BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_SUB_rrr_r()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            computer.CPU.REGS.ABC = 3000;
            computer.CPU.REGS.D = 250;

            cp.Build(
                TUtils.GetFormattedAsm(
                    string.Format("SUB ABC,D"),
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            ushort expectedValue = 2750;

            Assert.Equal(expectedValue, computer.CPU.REGS.ABC);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SUB_rrr_rr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            computer.CPU.REGS.ABC = 3000;
            computer.CPU.REGS.DE = 4000;

            cp.Build(
                TUtils.GetFormattedAsm(
                    string.Format("SUB ABC,DE"),
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint expectedValue = 0xFFFC18;

            Assert.Equal(expectedValue, computer.CPU.REGS.ABC);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SUB_rrr_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            computer.CPU.REGS.ABC = 0x1000;
            computer.CPU.REGS.DEF = 0x100000;

            cp.Build(
                TUtils.GetFormattedAsm(
                    string.Format("SUB ABC,DEF"),
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint expectedValue = 0xF01000;

            Assert.Equal(expectedValue, computer.CPU.REGS.ABC);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SUB_rrr_InnnI()
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
                        string.Format("SUB {0},({1})", TUtils.Get24bitRegisterString(i), address),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint expectedValue = 0x102000;

                Assert.Equal(expectedValue, computer.CPU.REGS.Get24BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_SUB16_rrr_InnnI()
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
                        string.Format("SUB16 {0},({1})", TUtils.Get24bitRegisterString(i), address),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint expectedValue = 0x100000;

                Assert.Equal(expectedValue, computer.CPU.REGS.Get24BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_SUB24_rrr_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 0; i < 26; i++)
            {
                computer.Clear();
                uint address = 10000;
                computer.CPU.REGS.Set24BitRegister(i, 0x102030);
                computer.MEMC.Set24bitToRAM(address, 0x102031);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("SUB24 {0},({1})", TUtils.Get24bitRegisterString(i), address),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint expectedValue = 0xFFFFFF;

                Assert.Equal(expectedValue, computer.CPU.REGS.Get24BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_SUB_rrr_IrrrI()
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
                    "SUB ABC,(DEF)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            ushort expectedValue = 0x3FE0;

            Assert.Equal(expectedValue, computer.CPU.REGS.ABC);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SUB16_rrr_IrrrI()
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
                    "SUB16 ABC,(DEF)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            ushort expectedValue = 0x1FD0;

            Assert.Equal(expectedValue, computer.CPU.REGS.ABC);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SUB24_rrr_IrrrI()
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
                    "SUB24 ABC,(DEF)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint expectedValue = 0xE00FC0;

            Assert.Equal(expectedValue, computer.CPU.REGS.ABC);
            TUtils.IncrementCountedTests("exec");
        }

        // SUB rrrr, (n..., r, rr, rrr, (nnn)..., (rrr)...)
        [Fact]
        public void TestEXEC_SUB_rrrr_n()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 0; i < 26; i++)
            {
                computer.Clear();
                computer.CPU.REGS.Set32BitRegister(i, 0x10203040);

                byte value = (byte)(i * 9 + 1);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("SUB {0},{1}", TUtils.Get32bitRegisterString(i), value),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint expectedValue = (uint)((0x10203040 - value) & 0xFFFFFFFF);

                Assert.Equal(expectedValue, computer.CPU.REGS.Get32BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_SUB16_rrrr_nn()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 0; i < 26; i++)
            {
                computer.Clear();
                computer.CPU.REGS.Set32BitRegister(i, 0x10203040);

                uint value = (uint)(i * 0x900 + 1);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("SUB16 {0},{1}", TUtils.Get32bitRegisterString(i), value),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint expectedValue = (0x10203040 - value) & 0xFFFFFFFF;

                Assert.Equal(expectedValue, computer.CPU.REGS.Get32BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_SUB24_rrrr_nnn()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 0; i < 26; i++)
            {
                computer.Clear();
                computer.CPU.REGS.Set32BitRegister(i, 0x10203040);

                uint value = (uint)(i * 0x60000 + 1);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("SUB24 {0},{1}", TUtils.Get32bitRegisterString(i), value),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint expectedValue = (0x10203040 - value) & 0xFFFFFFFF;

                Assert.Equal(expectedValue, computer.CPU.REGS.Get32BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_SUB32_rrrr_nnnn()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 0; i < 26; i++)
            {
                computer.Clear();
                computer.CPU.REGS.Set32BitRegister(i, 0x10203040);

                uint value = (uint)(i * 0x4000000 + 1);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("SUB32 {0},{1}", TUtils.Get32bitRegisterString(i), value),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint expectedValue = (0x10203040 - value) & 0xFFFFFFFF;

                Assert.Equal(expectedValue, computer.CPU.REGS.Get32BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_SUB_rrrr_r()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            computer.CPU.REGS.ABCD = 1;
            computer.CPU.REGS.E = 2;

            cp.Build(
                TUtils.GetFormattedAsm(
                    string.Format("SUB ABCD,E"),
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint expectedValue = 0xFFFFFFFF;

            Assert.Equal(expectedValue, computer.CPU.REGS.ABCD);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SUB_rrrr_rr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            computer.CPU.REGS.ABCD = 0;
            computer.CPU.REGS.EF = 0xFFFF;

            cp.Build(
                TUtils.GetFormattedAsm(
                    string.Format("SUB ABCD,EF"),
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint expectedValue = 0xFFFF0001;

            Assert.Equal(expectedValue, computer.CPU.REGS.ABCD);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SUB_rrrr_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            computer.CPU.REGS.ABCD = 0;
            computer.CPU.REGS.EFG = 0xFFFFFF;

            cp.Build(
                TUtils.GetFormattedAsm(
                    string.Format("SUB ABCD,EFG"),
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint expectedValue = 0xFF000001;

            Assert.Equal(expectedValue, computer.CPU.REGS.ABCD);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SUB_rrrr_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            computer.CPU.REGS.ABCD = 0xFFFFFFFF;
            computer.CPU.REGS.EFGH = 0xFFFFFFFE;

            cp.Build(
                TUtils.GetFormattedAsm(
                    string.Format("SUB ABCD,EFGH"),
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint expectedValue = 1;

            Assert.Equal(expectedValue, computer.CPU.REGS.ABCD);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SUB_rrrr_InnnI()
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
                        string.Format("SUB {0},({1})", TUtils.Get32bitRegisterString(i), address),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint expectedValue = 0x10203010;

                Assert.Equal(expectedValue, computer.CPU.REGS.Get32BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_SUB16_rrrr_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 0; i < 26; i++)
            {
                computer.Clear();
                uint address = 10000;
                computer.CPU.REGS.Set32BitRegister(i, 0x20304050);
                computer.MEMC.Set32bitToRAM(address, 0x10203040);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("SUB16 {0},({1})", TUtils.Get32bitRegisterString(i), address),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint expectedValue = 0x20303030;

                Assert.Equal(expectedValue, computer.CPU.REGS.Get32BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_SUB24_rrrr_InnnI()
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
                        string.Format("SUB24 {0},({1})", TUtils.Get32bitRegisterString(i), address),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint expectedValue = 0x10000000;

                Assert.Equal(expectedValue, computer.CPU.REGS.Get32BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_SUB32_rrrr_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 0; i < 26; i++)
            {
                computer.Clear();
                uint address = 10000;
                computer.CPU.REGS.Set32BitRegister(i, 0x20304050);
                computer.MEMC.Set32bitToRAM(address, 0x10203040);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("SUB32 {0},({1})", TUtils.Get32bitRegisterString(i), address),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint expectedValue = 0x10101010;

                Assert.Equal(expectedValue, computer.CPU.REGS.Get32BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_SUB_rrrr_IrrrI()
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
                    "SUB ABCD,(EFG)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            ushort expectedValue = 0x3FE0;

            Assert.Equal(expectedValue, computer.CPU.REGS.ABCD);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SUB16_rrrr_IrrrI()
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
                    "SUB16 ABCD,(EFG)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            ushort expectedValue = 0x1FD0;

            Assert.Equal(expectedValue, computer.CPU.REGS.ABCD);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SUB24_rrrr_IrrrI()
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
                    "SUB24 ABCD,(EFG)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint expectedValue = 0xFFE0_0FC0;

            Assert.Equal(expectedValue, computer.CPU.REGS.ABCD);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SUB32_rrrr_IrrrI()
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
                    "SUB32 ABCD,(EFG)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint expectedValue = 0xDFCFFFB0;

            Assert.Equal(expectedValue, computer.CPU.REGS.ABCD);
            TUtils.IncrementCountedTests("exec");
        }

        // SUB (nnn), (n..., r..., rr..., rrr..., (nnn)..., (rrr)...)
        [Fact]
        public void TestEXEC_SUB_InnnI_n()
        {
            Assert.True(SUB_InnnI_n(10000, 250, 10, 240));
            Assert.True(SUB_InnnI_n(10000, 200 * 256 + 100, 101, 255));
            Assert.True(SUB_InnnI_n(10000, 65536 * 20 + 256 * 10 + 7, 255, 8));
            Assert.True(SUB_InnnI_n(10000, 16777216 * 50 + 65536 * 20 + 256 * 10 + 100, 25, 75));
        }

        [Fact]
        public void TestEXEC_SUB16_InnnI_n()
        {
            Assert.True(SUB16_InnnI_nn(10000, 250, 10, 240));
            Assert.True(SUB16_InnnI_nn(10000, 200 * 256 + 100, 100, 200 * 256));
            Assert.True(SUB16_InnnI_nn(10000, 65536 * 20 + 256 * 10 + 7, 255, 256 * 9 + 8));
            Assert.True(SUB16_InnnI_nn(10000, 16777216 * 50 + 65536 * 20 + 256 * 10 + 110, 100, 256 * 10 + 10));
        }

        [Fact]
        public void TestEXEC_SUB24_InnnI_n()
        {
            Assert.True(SUB24_InnnI_nnn(10000, 250, 10, 240));
            Assert.True(SUB24_InnnI_nnn(10000, 200 * 256 + 100, 100, 200 * 256));
            Assert.True(SUB24_InnnI_nnn(10000, 65536 * 20 + 256 * 10 + 7, 255, 65536 * 20 + 256 * 9 + 8));
            Assert.True(SUB24_InnnI_nnn(10000, 16777216 * 50 + 65536 * 20 + 256 * 10 + 110, 100, 65536 * 20 + 256 * 10 + 10));
        }

        [Fact]
        public void TestEXEC_SUB32_InnnI_n()
        {
            Assert.True(SUB32_InnnI_nnnn(10000, 250, 10, 240));
            Assert.True(SUB32_InnnI_nnnn(10000, 200 * 256 + 100, 100, 200 * 256));
            Assert.True(SUB32_InnnI_nnnn(10000, 65536 * 20 + 256 * 10 + 7, 255, 65536 * 20 + 256 * 9 + 8));
            Assert.True(SUB32_InnnI_nnnn(10000, 16777216 * 50 + 65536 * 20 + 256 * 10 + 110, 100, 16777216 * 50 + 65536 * 20 + 256 * 10 + 10));
        }

        [Fact]
        public void TestEXEC_SUB_InnnI_r()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 10000;
            byte initialValue = 200;

            for (byte i = 0; i < 26; i++)
            {
                byte subValue = (byte)(i * 9);
                byte expectedValue = (byte)(initialValue - subValue);
                computer.Clear();

                computer.MEMC.Set8bitToRAM(address, initialValue);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("LD {0},{1}", TUtils.Get8bitRegisterChar(i), subValue),
                        string.Format("SUB ({0}),{1}", address, TUtils.Get8bitRegisterChar(i)),
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
        public void TestEXEC_SUB16_InnnI_r()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 30;
            ushort initialValue = 200 * 0x100 + 100;

            for (byte i = 0; i < 26; i++)
            {
                byte subValue = (byte)(i * 9 + 1);
                ushort expectedValue = (ushort)(initialValue - subValue);
                computer.Clear();

                computer.MEMC.Set16bitToRAM(address, initialValue);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("LD {0},{1}", TUtils.Get8bitRegisterChar(i), subValue),
                        string.Format("SUB16 ({0}),{1}", address, TUtils.Get8bitRegisterChar(i)),
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
        public void TestEXEC_SUB24_InnnI_r()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 10000;
            uint initialValue = 50 * 0x10000 + 200 * 0x100 + 100;

            for (byte i = 0; i < 26; i++)
            {
                byte subValue = (byte)(i * 9 + 1);
                uint expectedValue = initialValue - subValue;
                computer.Clear();

                computer.MEMC.Set24bitToRAM(address, initialValue);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("LD {0},{1}", TUtils.Get8bitRegisterChar(i), subValue),
                        string.Format("SUB24 ({0}),{1}", address, TUtils.Get8bitRegisterChar(i)),
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
        public void TestEXEC_SUB32_InnnI_r()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 30;
            uint initialValue = 10 * 0x1000000 + 50 * 0x10000 + 200 * 0x100 + 100;

            for (byte i = 0; i < 26; i++)
            {
                byte subValue = (byte)(i * 9 + 1);
                uint expectedValue = initialValue - subValue;
                computer.Clear();

                computer.MEMC.Set32bitToRAM(address, initialValue);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("LD {0},{1}", TUtils.Get8bitRegisterChar(i), subValue),
                        string.Format("SUB32 ({0}),{1}", address, TUtils.Get8bitRegisterChar(i)),
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
        public void TestEXEC_SUB16_InnnI_rr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 10000;

            ushort initialValue = 200 * 256 + 100;

            for (byte i = 0; i < 26; i++)
            {
                ushort subValue = (ushort)(i * 1000);
                ushort expectedValue = (ushort)(initialValue - subValue);
                computer.Clear();

                computer.MEMC.Set16bitToRAM(address, initialValue);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("LD {0},{1}", TUtils.Get16bitRegisterString(i), subValue),
                        string.Format("SUB16 ({0}),{1}", address, TUtils.Get16bitRegisterString(i)),
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
        public void TestEXEC_SUB24_InnnI_rr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 10000;

            uint initialValue = 100 * 0x10000 + 200 * 256 + 100;

            for (byte i = 0; i < 26; i++)
            {
                ushort subValue = (ushort)(i * 1000);
                uint expectedValue = initialValue - subValue;
                computer.Clear();

                computer.MEMC.Set24bitToRAM(address, initialValue);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("LD {0},{1}", TUtils.Get16bitRegisterString(i), subValue),
                        string.Format("SUB24 ({0}),{1}", address, TUtils.Get16bitRegisterString(i)),
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
        public void TestEXEC_SUB32_InnnI_rr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 10000;

            uint initialValue = 50 * 0x1000000 + 100 * 0x10000 + 200 * 256 + 100;

            for (byte i = 0; i < 26; i++)
            {
                ushort subValue = (ushort)(i * 1000);
                uint expectedValue = initialValue - subValue;
                computer.Clear();

                computer.MEMC.Set32bitToRAM(address, initialValue);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("LD {0},{1}", TUtils.Get16bitRegisterString(i), subValue),
                        string.Format("SUB32 ({0}),{1}", address, TUtils.Get16bitRegisterString(i)),
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
        public void TestEXEC_SUB24_InnnI_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 10000;

            uint initialValue = 100 * 0x10000 + 200 * 0x100 + 100;

            for (byte i = 0; i < 26; i++)
            {
                uint subValue = (uint)(i * 0x1FFFF);
                uint expectedValue = initialValue - subValue;
                computer.Clear();

                computer.MEMC.Set24bitToRAM(address, initialValue);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("LD {0},{1}", TUtils.Get24bitRegisterString(i), subValue),
                        string.Format("SUB24 ({0}),{1}", address, TUtils.Get24bitRegisterString(i)),
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
        public void TestEXEC_SUB32_InnnI_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 10000;

            uint initialValue = 50 * 0x1000000 + 100 * 0x10000 + 200 * 0x100 + 100;

            for (byte i = 0; i < 26; i++)
            {
                uint subValue = (uint)(i * 0x1FFFF);
                uint expectedValue = initialValue - subValue;
                computer.Clear();

                computer.MEMC.Set32bitToRAM(address, initialValue);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("LD {0},{1}", TUtils.Get24bitRegisterString(i), subValue),
                        string.Format("SUB32 ({0}),{1}", address, TUtils.Get24bitRegisterString(i)),
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
        public void TestEXEC_SUB_InnnI_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 30;

            uint initialValue = 50 * 0x1000000 + 100 * 0x10000 + 200 * 0x100 + 100;

            for (byte i = 0; i < 26; i++)
            {
                uint subValue = (uint)(i * 0x1FFFFFF + 1);
                uint expectedValue = (uint)(initialValue - subValue);
                computer.Clear();

                computer.MEMC.Set32bitToRAM(address, initialValue);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("LD {0},{1}", TUtils.Get32bitRegisterString(i), subValue),
                        string.Format("SUB ({0}),{1}", address, TUtils.Get32bitRegisterString(i)),
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

        // SUB (rrr), (n..., r..., rr..., rrr..., (nnn)..., (rrr)...)
        [Fact]
        public void TestEXEC_SUB_IrrrI_n()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 10000;

            byte initialValue = 100;

            for (byte i = 0; i < 26; i++)
            {
                byte subValue = (byte)(i * 9 + 1);
                uint expectedValue = (byte)(initialValue - subValue);
                computer.Clear();

                computer.MEMC.Set8bitToRAM(address, initialValue);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("LD {0},{1}", TUtils.Get24bitRegisterString(i), address),
                        string.Format("SUB ({0}),{1}", TUtils.Get24bitRegisterString(i), subValue),
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
        public void TestEXEC_SUB16_IrrrI_n()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 10000;

            ushort initialValue = 200 * 0x100 + 100;

            for (byte i = 0; i < 26; i++)
            {
                ushort subValue = (ushort)(i * 0x2FF + 1);
                uint expectedValue = (ushort)(initialValue - subValue);
                computer.Clear();

                computer.MEMC.Set16bitToRAM(address, initialValue);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("LD {0},{1}", TUtils.Get24bitRegisterString(i), address),
                        string.Format("SUB16 ({0}),{1}", TUtils.Get24bitRegisterString(i), subValue),
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
        public void TestEXEC_SUB24_IrrrI_n()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 10000;

            uint initialValue = 100 * 0x10000 + 200 * 0x100 + 100;

            for (byte i = 0; i < 26; i++)
            {
                uint subValue = (uint)(i * 0x12FF + 1);
                uint expectedValue = initialValue - subValue;
                computer.Clear();

                computer.MEMC.Set24bitToRAM(address, initialValue);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("LD {0},{1}", TUtils.Get24bitRegisterString(i), address),
                        string.Format("SUB24 ({0}),{1}", TUtils.Get24bitRegisterString(i), subValue),
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
        public void TestEXEC_SUB32_IrrrI_n()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 10000;

            uint initialValue = 50 * 0x1000000 + 100 * 0x10000 + 200 * 0x100 + 100;

            for (byte i = 0; i < 26; i++)
            {
                uint subValue = (uint)(i * 0x212FF + 1);
                uint expectedValue = initialValue - subValue;
                computer.Clear();

                computer.MEMC.Set32bitToRAM(address, initialValue);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("LD {0},{1}", TUtils.Get24bitRegisterString(i), address),
                        string.Format("SUB32 ({0}),{1}", TUtils.Get24bitRegisterString(i), subValue),
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
        public void TestEXEC_SUB_IrrrI_r()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 10000;

            byte initialValue = 200;
            byte subValue = 100;
            byte expectedValue = (byte)(initialValue - subValue);
            computer.Clear();

            computer.MEMC.Set8bitToRAM(address, initialValue);

            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.Z = subValue;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SUB (ABC),Z",
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
        public void TestEXEC_SUB16_IrrrI_r()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 10000;

            ushort initialValue = 200 * 0x100 + 100;
            byte subValue = 100;
            ushort expectedValue = (ushort)(initialValue - subValue);
            computer.Clear();

            computer.MEMC.Set16bitToRAM(address, initialValue);

            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.Z = subValue;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SUB16 (ABC),Z",
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
        public void TestEXEC_SUB24_IrrrI_r()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 10000;

            uint initialValue = 100 * 0x10000 + 200 * 0x100 + 100;
            byte subValue = 100;
            uint expectedValue = initialValue - subValue;
            computer.Clear();

            computer.MEMC.Set24bitToRAM(address, initialValue);

            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.Z = subValue;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SUB24 (ABC),Z",
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
        public void TestEXEC_SUB32_IrrrI_r()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 10000;

            uint initialValue = 50 * 0x1000000 + 100 * 0x10000 + 200 * 0x100 + 100;
            byte subValue = 100;
            uint expectedValue = initialValue - subValue;
            computer.Clear();

            computer.MEMC.Set32bitToRAM(address, initialValue);

            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.Z = subValue;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SUB32 (ABC),Z",
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
        public void TestEXEC_SUB16_IrrrI_rr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 10000;

            ushort initialValue = 230 * 0x100 + 100;
            ushort subValue = 45 * 0x100 + 100;
            ushort expectedValue = (ushort)(initialValue - subValue);
            computer.Clear();

            computer.MEMC.Set16bitToRAM(address, initialValue);

            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.YZ = subValue;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SUB16 (ABC),YZ",
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
        public void TestEXEC_SUB24_IrrrI_rr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 10000;

            uint initialValue = 100 * 0x10000 + 230 * 0x100 + 100;
            ushort subValue = 45 * 0x100 + 100;
            uint expectedValue = initialValue - subValue;
            computer.Clear();

            computer.MEMC.Set24bitToRAM(address, initialValue);

            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.YZ = subValue;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SUB24 (ABC),YZ",
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
        public void TestEXEC_SUB32_IrrrI_rr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 10000;

            uint initialValue = 50 * 0x1000000 + 100 * 0x10000 + 230 * 0x100 + 100;
            ushort subValue = 45 * 0x100 + 100;
            uint expectedValue = initialValue - subValue;
            computer.Clear();

            computer.MEMC.Set32bitToRAM(address, initialValue);

            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.YZ = subValue;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SUB32 (ABC),YZ",
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
        public void TestEXEC_SUB24_IrrrI_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 10000;

            uint initialValue = 100 * 0x10000 + 230 * 0x100 + 100;
            uint subValue = 80 * 0x10000 + 45 * 0x100 + 100;
            uint expectedValue = initialValue - subValue;
            computer.Clear();

            computer.MEMC.Set24bitToRAM(address, initialValue);

            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.XYZ = subValue;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SUB24 (ABC), XYZ",
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
        public void TestEXEC_SUB32_IrrrI_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 10000;

            uint initialValue = 50 * 0x1000000 + 100 * 0x10000 + 230 * 0x100 + 100;
            uint subValue = 80 * 0x10000 + 45 * 0x100 + 100;
            uint expectedValue = initialValue - subValue;
            computer.Clear();

            computer.MEMC.Set32bitToRAM(address, initialValue);

            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.XYZ = subValue;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SUB32 (ABC), XYZ",
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
        public void TestEXEC_SUB_IrrrI_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 10000;

            uint initialValue = 50 * 0x1000000 + 100 * 0x10000 + 230 * 0x100 + 100;
            uint subValue = 100 * 0x1000000 + 80 * 0x10000 + 45 * 0x100 + 100;
            uint expectedValue = initialValue - subValue;
            computer.Clear();

            computer.MEMC.Set32bitToRAM(address, initialValue);

            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.WXYZ = subValue;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SUB (ABC), WXYZ",
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

        // Memory subtractions
        [Fact]
        public void TestEXEC_SUB_InnnI_InnnI()
        {
            Assert.True(SUB_InnnI_InnnI(10000, 250, 20000, 10, 240));
            Assert.True(SUB_InnnI_InnnI(10000, 200 * 256 + 100, 20000, 101, 255));
            Assert.True(SUB_InnnI_InnnI(10000, 65536 * 20 + 256 * 10 + 7, 20000, 255, 8));
            Assert.True(SUB_InnnI_InnnI(10000, 16777216 * 50 + 65536 * 20 + 256 * 10 + 100, 20000, 25, 75));
        }

        [Fact]
        public void TestEXEC_SUB16_InnnI_InnnI()
        {
            Assert.True(SUB16_InnnI_InnnI(10000, 250, 20000, 10, 240));
            Assert.True(SUB16_InnnI_InnnI(10000, 200 * 256 + 100, 20000, 100, 200 * 256));
            Assert.True(SUB16_InnnI_InnnI(10000, 65536 * 20 + 256 * 10 + 7, 20000, 255, 256 * 9 + 8));
            Assert.True(SUB16_InnnI_InnnI(10000, 16777216 * 50 + 65536 * 20 + 256 * 10 + 110, 20000, 100, 256 * 10 + 10));
        }

        [Fact]
        public void TestEXEC_SUB24_InnnI_InnnI()
        {
            Assert.True(SUB24_InnnI_InnnI(10000, 250, 20000, 10, 240));
            Assert.True(SUB24_InnnI_InnnI(10000, 200 * 256 + 100, 20000, 100, 200 * 256));
            Assert.True(SUB24_InnnI_InnnI(10000, 65536 * 20 + 256 * 10 + 7, 20000, 255, 65536 * 20 + 256 * 9 + 8));
            Assert.True(SUB24_InnnI_InnnI(10000, 16777216 * 50 + 65536 * 20 + 256 * 10 + 110, 20000, 100, 65536 * 20 + 256 * 10 + 10));
        }

        [Fact]
        public void TestEXEC_SUB32_InnnI_InnnI()
        {
            Assert.True(SUB32_InnnI_InnnI(10000, 250, 20000, 10, 240));
            Assert.True(SUB32_InnnI_InnnI(10000, 200 * 256 + 100, 20000, 100, 200 * 256));
            Assert.True(SUB32_InnnI_InnnI(10000, 65536 * 20 + 256 * 10 + 7, 20000, 255, 65536 * 20 + 256 * 9 + 8));
            Assert.True(SUB32_InnnI_InnnI(10000, 16777216 * 50 + 65536 * 20 + 256 * 10 + 110, 20000, 100, 16777216 * 50 + 65536 * 20 + 256 * 10 + 10));
        }


        // Test support
        private bool SUB_InnnI_n(uint address, uint initialValue, uint subValue, uint expectedValue)
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();

            computer.MEMC.Set8bitToRAM(address, (byte)initialValue);

            cp.Build(
                TUtils.GetFormattedAsm(
                    string.Format("SUB ({0}),{1}", address, subValue),
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

        private bool SUB16_InnnI_nn(uint address, uint initialValue, uint subValue, uint expectedValue)
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();

            computer.MEMC.Set16bitToRAM(address, (ushort)initialValue);

            cp.Build(
                TUtils.GetFormattedAsm(
                    string.Format("SUB16 ({0}),{1}", address, subValue),
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

        private bool SUB24_InnnI_nnn(uint address, uint initialValue, uint subValue, uint expectedValue)
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();

            computer.MEMC.Set24bitToRAM(address, initialValue);

            cp.Build(
                TUtils.GetFormattedAsm(
                    string.Format("SUB24 ({0}),{1}", address, subValue),
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

        private bool SUB32_InnnI_nnnn(uint address, uint initialValue, uint value, uint expectedValue)
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();

            computer.MEMC.Set32bitToRAM(address, initialValue);

            cp.Build(
                TUtils.GetFormattedAsm(
                    string.Format("SUB32 ({0}),{1}", address, value),
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
        private bool SUB_InnnI_InnnI(uint address1, uint value1, uint address2, uint value2, uint expectedValue)
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();

            computer.MEMC.Set8bitToRAM(address1, (byte)value1);
            computer.MEMC.Set8bitToRAM(address2, (byte)value2);

            cp.Build(
                TUtils.GetFormattedAsm(
                    string.Format("SUB ({0}),({1})", address1, address2),
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

        private bool SUB16_InnnI_InnnI(uint address1, uint value1, uint address2, uint value2, uint expectedValue)
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();

            computer.MEMC.Set16bitToRAM(address1, (ushort)value1);
            computer.MEMC.Set16bitToRAM(address2, (ushort)value2);

            cp.Build(
                TUtils.GetFormattedAsm(
                    string.Format("SUB16 ({0}),({1})", address1, address2),
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

        private bool SUB24_InnnI_InnnI(uint address1, uint value1, uint address2, uint value2, uint expectedValue)
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();

            computer.MEMC.Set24bitToRAM(address1, value1);
            computer.MEMC.Set24bitToRAM(address2, value2);

            cp.Build(
                TUtils.GetFormattedAsm(
                    string.Format("SUB24 ({0}),({1})", address1, address2),
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

        private bool SUB32_InnnI_InnnI(uint address1, uint value1, uint address2, uint value2, uint expectedValue)
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();

            computer.MEMC.Set32bitToRAM(address1, value1);
            computer.MEMC.Set32bitToRAM(address2, value2);

            cp.Build(
                TUtils.GetFormattedAsm(
                    string.Format("SUB32 ({0}),({1})", address1, address2),
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
