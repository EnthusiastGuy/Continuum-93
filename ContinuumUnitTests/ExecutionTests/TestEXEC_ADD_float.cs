using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{

    public class TestEXEC_ADD_float
    {
        [Fact]
        public void TestEXEC_ADD_fr_fr()
        {
            for (byte i = 0; i < 16; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                computer.CPU.FREGS.SetRegister(i, (i + 1) * 1.1f);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        $"ADD {TUtils.GetFloatRegisterString(i)},{TUtils.GetFloatRegisterString(i)}",
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                float expectedValue = (i + 1) * 2.2f;

                Assert.Equal(expectedValue, computer.CPU.FREGS.GetRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_ADD_fr_nnn()
        {
            for (byte i = 0; i < 16; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                computer.CPU.FREGS.SetRegister(i, (i + 1) * 1.1f);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("ADD {0},0.25", TUtils.GetFloatRegisterString(i)),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                float expectedValue = (i + 1) * 1.1f + 0.25f;

                Assert.Equal(expectedValue, computer.CPU.FREGS.GetRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_ADD__fr_r_8bit()
        {
            for (byte i = 0; i < 16; i++)
            {
                for (byte j = 0; j < 26; j++)
                {
                    Assembler cp = new();
                    using Computer computer = new();

                    byte regValue = (byte)(j + 1);
                    float fRegValue = (i + 1) * 1.1f;

                    computer.CPU.FREGS.SetRegister(i, fRegValue);
                    computer.CPU.REGS.Set8BitRegister(j, regValue);

                    cp.Build(
                        TUtils.GetFormattedAsm(
                            string.Format("ADD {0},{1}",
                                TUtils.GetFloatRegisterString(i),
                                TUtils.Get8bitRegisterChar(j)
                            ),
                            "BREAK"
                        )
                    );

                    byte[] compiled = cp.GetCompiledCode();

                    computer.LoadMem(compiled);
                    computer.Run();

                    float expectedValue = fRegValue + regValue;

                    Assert.Equal(expectedValue, computer.CPU.FREGS.GetRegister(i));
                    TUtils.IncrementCountedTests("exec");
                }
            }
        }

        [Fact]
        public void TestEXEC_ADD__fr_r_16bit()
        {
            for (byte i = 0; i < 16; i++)
            {
                for (byte j = 0; j < 26; j++)
                {
                    Assembler cp = new();
                    using Computer computer = new();

                    ushort regValue = (ushort)((j + 1) * 1024);
                    float fRegValue = (i + 1) * 1.1f;

                    computer.CPU.FREGS.SetRegister(i, fRegValue);
                    computer.CPU.REGS.Set16BitRegister(j, regValue);

                    cp.Build(
                        TUtils.GetFormattedAsm(
                            string.Format("ADD {0},{1}",
                                TUtils.GetFloatRegisterString(i),
                                TUtils.Get16bitRegisterString(j)
                            ),
                            "BREAK"
                        )
                    );

                    byte[] compiled = cp.GetCompiledCode();

                    computer.LoadMem(compiled);
                    computer.Run();

                    float expectedValue = fRegValue + regValue;

                    Assert.Equal(expectedValue, computer.CPU.FREGS.GetRegister(i));
                    TUtils.IncrementCountedTests("exec");
                }
            }
        }

        [Fact]
        public void TestEXEC_ADD__fr_r_24bit()
        {
            for (byte i = 0; i < 16; i++)
            {
                for (byte j = 0; j < 26; j++)
                {
                    Assembler cp = new();
                    using Computer computer = new();

                    uint regValue = (uint)(j + 1) * 240000;
                    float fRegValue = (i + 1) * 1.1f;

                    computer.CPU.FREGS.SetRegister(i, fRegValue);
                    computer.CPU.REGS.Set24BitRegister(j, regValue);

                    cp.Build(
                        TUtils.GetFormattedAsm(
                            string.Format("ADD {0},{1}",
                                TUtils.GetFloatRegisterString(i),
                                TUtils.Get24bitRegisterString(j)
                            ),
                            "BREAK"
                        )
                    );

                    byte[] compiled = cp.GetCompiledCode();

                    computer.LoadMem(compiled);
                    computer.Run();

                    float expectedValue = fRegValue + regValue;

                    Assert.Equal(expectedValue, computer.CPU.FREGS.GetRegister(i));
                    TUtils.IncrementCountedTests("exec");
                }
            }
        }

        [Fact]
        public void TestEXEC_ADD__fr_r_32bit()
        {
            for (byte i = 0; i < 16; i++)
            {
                for (byte j = 0; j < 26; j++)
                {
                    Assembler cp = new();
                    using Computer computer = new();

                    uint regValue = (uint)(j + 1) * 160_000_000;
                    float fRegValue = (i + 1) * 12.122f;

                    computer.CPU.FREGS.SetRegister(i, fRegValue);
                    computer.CPU.REGS.Set32BitRegister(j, regValue);

                    cp.Build(
                        TUtils.GetFormattedAsm(
                            string.Format("ADD {0},{1}",
                                TUtils.GetFloatRegisterString(i),
                                TUtils.Get32bitRegisterString(j)
                            ),
                            "BREAK"
                        )
                    );

                    byte[] compiled = cp.GetCompiledCode();

                    computer.LoadMem(compiled);
                    computer.Run();

                    float expectedValue = fRegValue + regValue;

                    Assert.Equal(expectedValue, computer.CPU.FREGS.GetRegister(i));
                    TUtils.IncrementCountedTests("exec");
                }
            }
        }

        [Fact]
        public void TestEXEC_ADD_r_fr_8bit_positive()
        {
            for (byte i = 0; i < 16; i++)
            {
                for (byte j = 0; j < 26; j++)
                {
                    Assembler cp = new();
                    using Computer computer = new();

                    byte regValue = (byte)((j + 1) * 10);
                    float fRegValue = (i + 1) * 12.125f;

                    computer.CPU.FREGS.SetRegister(i, fRegValue);
                    computer.CPU.REGS.Set8BitRegister(j, regValue);

                    cp.Build(
                        TUtils.GetFormattedAsm(
                            string.Format("ADD {0},{1}",
                                TUtils.Get8bitRegisterChar(j),
                                TUtils.GetFloatRegisterString(i)
                            ),
                            "BREAK"
                        )
                    );

                    byte[] compiled = cp.GetCompiledCode();

                    computer.LoadMem(compiled);
                    computer.Run();

                    byte expectedValue = (byte)Math.Round(regValue + fRegValue);

                    Assert.Equal(expectedValue, computer.CPU.REGS.Get8BitRegister(j));
                    TUtils.IncrementCountedTests("exec");
                }
            }
        }

        [Fact]
        public void TestEXEC_ADD_r_fr_8bit_negative()
        {
            string shortLog = "";

            for (byte i = 0; i < 16; i++)
            {
                for (byte j = 0; j < 26; j++)
                {
                    Assembler cp = new();
                    using Computer computer = new();

                    shortLog += $"i:{i}, j:{j}{Environment.NewLine}";
                    byte regValue = (byte)((j + 1) * 10);
                    float fRegValue = -(i + 1) * 8.122f;

                    computer.CPU.FREGS.SetRegister(i, fRegValue);
                    computer.CPU.REGS.Set8BitRegister(j, regValue);

                    cp.Build(
                        TUtils.GetFormattedAsm(
                            string.Format("ADD {0},{1}",
                                TUtils.Get8bitRegisterChar(j),
                                TUtils.GetFloatRegisterString(i)
                            ),
                            "BREAK"
                        )
                    );

                    byte[] compiled = cp.GetCompiledCode();

                    computer.LoadMem(compiled);
                    computer.Run();

                    byte expectedValue = (byte)Math.Round(regValue + fRegValue);

                    byte actual = computer.CPU.REGS.Get8BitRegister(j);

                    Assert.Equal(expectedValue, actual);
                    TUtils.IncrementCountedTests("exec");
                }
            }
        }

        [Fact]
        public void TestEXEC_ADD_r_fr_16bit_positive()
        {
            for (byte i = 0; i < 16; i++)
            {
                for (byte j = 0; j < 26; j++)
                {
                    Assembler cp = new();
                    using Computer computer = new();

                    ushort regValue = (ushort)((j + 1) * 600);
                    float fRegValue = (i + 1) * 12.122f;

                    computer.CPU.FREGS.SetRegister(i, fRegValue);
                    computer.CPU.REGS.Set16BitRegister(j, regValue);

                    cp.Build(
                        TUtils.GetFormattedAsm(
                            string.Format("ADD {0},{1}",
                                TUtils.Get16bitRegisterString(j),
                                TUtils.GetFloatRegisterString(i)
                            ),
                            "BREAK"
                        )
                    );

                    byte[] compiled = cp.GetCompiledCode();

                    computer.LoadMem(compiled);
                    computer.Run();

                    ushort expectedValue = (ushort)Math.Round(regValue + fRegValue);

                    Assert.Equal(expectedValue, computer.CPU.REGS.Get16BitRegister(j));
                    TUtils.IncrementCountedTests("exec");
                }
            }
        }

        [Fact]
        public void TestEXEC_ADD_r_fr_16bit_negative()
        {
            for (byte i = 0; i < 16; i++)
            {
                for (byte j = 0; j < 26; j++)
                {
                    Assembler cp = new();
                    using Computer computer = new();

                    ushort regValue = (ushort)((j + 1) * 600);
                    float fRegValue = -(i + 1) * 8.122f;

                    computer.CPU.FREGS.SetRegister(i, fRegValue);
                    computer.CPU.REGS.Set16BitRegister(j, regValue);

                    cp.Build(
                        TUtils.GetFormattedAsm(
                            string.Format("ADD {0},{1}",
                                TUtils.Get16bitRegisterString(j),
                                TUtils.GetFloatRegisterString(i)
                            ),
                            "BREAK"
                        )
                    );

                    byte[] compiled = cp.GetCompiledCode();

                    computer.LoadMem(compiled);
                    computer.Run();

                    ushort expectedValue = (ushort)Math.Round(regValue + fRegValue);
                    ushort actual = computer.CPU.REGS.Get16BitRegister(j);

                    Assert.Equal(expectedValue, actual);
                    TUtils.IncrementCountedTests("exec");
                }
            }
        }

        [Fact]
        public void TestEXEC_ADD_r_fr_24bit_positive()
        {
            for (byte i = 0; i < 16; i++)
            {
                for (byte j = 0; j < 26; j++)
                {
                    Assembler cp = new();
                    using Computer computer = new();

                    uint regValue = (uint)((j + 1) * 240_000);
                    float fRegValue = (i + 1) * 12.5f;

                    computer.CPU.FREGS.SetRegister(i, fRegValue);
                    computer.CPU.REGS.Set24BitRegister(j, regValue);

                    cp.Build(
                        TUtils.GetFormattedAsm(
                            string.Format("ADD {0},{1}",
                                TUtils.Get24bitRegisterString(j),
                                TUtils.GetFloatRegisterString(i)
                            ),
                            "BREAK"
                        )
                    );

                    byte[] compiled = cp.GetCompiledCode();

                    computer.LoadMem(compiled);
                    computer.Run();

                    uint expectedValue = (uint)Math.Round(regValue + fRegValue);
                    uint actualValue = computer.CPU.REGS.Get24BitRegister(j);

                    Assert.Equal(expectedValue, actualValue);
                    TUtils.IncrementCountedTests("exec");
                }
            }
        }

        [Fact]
        public void TestEXEC_ADD_r_fr_24bit_negative()
        {
            for (byte i = 0; i < 16; i++)
            {
                for (byte j = 0; j < 26; j++)
                {
                    Assembler cp = new();
                    using Computer computer = new();

                    uint regValue = (uint)((j + 1) * 240_000);
                    float fRegValue = -(i + 1) * 8.5f;

                    computer.CPU.FREGS.SetRegister(i, fRegValue);
                    computer.CPU.REGS.Set24BitRegister(j, regValue);

                    cp.Build(
                        TUtils.GetFormattedAsm(
                            string.Format("ADD {0},{1}",
                                TUtils.Get24bitRegisterString(j),
                                TUtils.GetFloatRegisterString(i)
                            ),
                            "BREAK"
                        )
                    );

                    byte[] compiled = cp.GetCompiledCode();

                    computer.LoadMem(compiled);
                    computer.Run();

                    uint expectedValue = (uint)Math.Round(regValue + fRegValue);
                    uint actual = computer.CPU.REGS.Get24BitRegister(j);

                    Assert.Equal(expectedValue, actual);
                    TUtils.IncrementCountedTests("exec");
                }
            }
        }

        [Fact]
        public void TestEXEC_ADD_r_fr_32bit_positive()
        {
            for (byte i = 0; i < 16; i++)
            {
                for (byte j = 0; j < 26; j++)
                {
                    Assembler cp = new();
                    using Computer computer = new();

                    uint regValue = (uint)((j + 1) * 160_000);
                    float fRegValue = (i + 1) * 12.5f;

                    computer.CPU.FREGS.SetRegister(i, fRegValue);
                    computer.CPU.REGS.Set32BitRegister(j, regValue);

                    cp.Build(
                        TUtils.GetFormattedAsm(
                            string.Format("ADD {0},{1}",
                                TUtils.Get32bitRegisterString(j),
                                TUtils.GetFloatRegisterString(i)
                            ),
                            "BREAK"
                        )
                    );

                    byte[] compiled = cp.GetCompiledCode();

                    computer.LoadMem(compiled);
                    computer.Run();

                    uint expectedValue = (uint)Math.Round(regValue + fRegValue);
                    uint actualValue = computer.CPU.REGS.Get32BitRegister(j);

                    Assert.Equal(expectedValue, actualValue);
                    TUtils.IncrementCountedTests("exec");
                }
            }
        }

        [Fact]
        public void TestEXEC_ADD_r_fr_32bit_negative()
        {
            for (byte i = 0; i < 16; i++)
            {
                for (byte j = 0; j < 26; j++)
                {
                    Assembler cp = new();
                    using Computer computer = new();

                    uint regValue = (uint)((j + 1) * 240_000);
                    float fRegValue = -(i + 1) * 8.5f;

                    computer.CPU.FREGS.SetRegister(i, fRegValue);
                    computer.CPU.REGS.Set32BitRegister(j, regValue);

                    cp.Build(
                        TUtils.GetFormattedAsm(
                            string.Format("ADD {0},{1}",
                                TUtils.Get32bitRegisterString(j),
                                TUtils.GetFloatRegisterString(i)
                            ),
                            "BREAK"
                        )
                    );

                    byte[] compiled = cp.GetCompiledCode();

                    computer.LoadMem(compiled);
                    computer.Run();

                    uint expectedValue = (uint)Math.Round(regValue + fRegValue);
                    uint actual = computer.CPU.REGS.Get32BitRegister(j);

                    Assert.Equal(expectedValue, actual);
                    TUtils.IncrementCountedTests("exec");
                }
            }
        }

        [Fact]
        public void TestEXEC_ADD_fr_InnnI()
        {
            for (byte i = 0; i < 16; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                float floatRAMValue = (i + 1) * 1.1f;
                float floatRegValue = (i + 1) * 1.3f;
                computer.MEMC.SetFloatToRam(0x2000, floatRAMValue);
                computer.CPU.FREGS.SetRegister(i, floatRegValue);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("ADD {0},(0x2000)", TUtils.GetFloatRegisterString(i)),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                float expectedValue = floatRAMValue + floatRegValue;

                Assert.Equal(expectedValue, computer.CPU.FREGS.GetRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_ADD_fr_IrrrI()
        {
            for (byte i = 0; i < 16; i++)
            {
                for (byte j = 0; j < 26; j++)
                {
                    Assembler cp = new();
                    using Computer computer = new();

                    float floatRAMValue = (i + 1) * 1.1f;
                    float floatRegValue = (i + 1) * 1.3f;
                    computer.MEMC.SetFloatToRam(0x2000, floatRAMValue);
                    computer.CPU.REGS.Set24BitRegister(j, 0x2000);
                    computer.CPU.FREGS.SetRegister(i, floatRegValue);

                    cp.Build(
                        TUtils.GetFormattedAsm(
                            string.Format("ADD {0},({1})",
                            TUtils.GetFloatRegisterString(i),
                            TUtils.Get24bitRegisterString(j)
                        ),
                            "BREAK"
                        )
                    );

                    byte[] compiled = cp.GetCompiledCode();

                    computer.LoadMem(compiled);
                    computer.Run();

                    float expectedValue = floatRAMValue + floatRegValue;

                    Assert.Equal(expectedValue, computer.CPU.FREGS.GetRegister(i));
                    TUtils.IncrementCountedTests("exec");
                }
            }
        }

        [Fact]
        public void TestEXEC_ADD_InnnI_fr()
        {
            for (byte i = 0; i < 16; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                float floatRAMValue = (i + 1) * 1.1f;
                float floatRegValue = (i + 1) * 1.3f;

                computer.MEMC.SetFloatToRam(0x2000, floatRAMValue);
                computer.CPU.FREGS.SetRegister(i, floatRegValue);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("ADD (0x2000),{0}", TUtils.GetFloatRegisterString(i)),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                float expectedValue = floatRAMValue + floatRegValue;

                Assert.Equal(expectedValue, computer.MEMC.GetFloatFromRAM(0x2000));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_ADD_IrrrI_fr()
        {
            for (byte i = 0; i < 16; i++)
            {
                for (byte j = 0; j < 26; j++)
                {
                    Assembler cp = new();
                    using Computer computer = new();

                    float floatRAMValue = (i + 1) * 1.1f;
                    float floatRegValue = (i + 1) * 1.3f;
                    computer.MEMC.SetFloatToRam(0x2000, floatRAMValue);
                    computer.CPU.REGS.Set24BitRegister(j, 0x2000);
                    computer.CPU.FREGS.SetRegister(i, floatRegValue);

                    cp.Build(
                        TUtils.GetFormattedAsm(
                            string.Format("ADD ({1}),{0}",
                            TUtils.GetFloatRegisterString(i),
                            TUtils.Get24bitRegisterString(j)
                        ),
                            "BREAK"
                        )
                    );

                    byte[] compiled = cp.GetCompiledCode();

                    computer.LoadMem(compiled);
                    computer.Run();

                    float expectedValue = floatRAMValue + floatRegValue;

                    Assert.Equal(expectedValue, computer.MEMC.GetFloatFromRAM(0x2000));
                    TUtils.IncrementCountedTests("exec");
                }
            }
        }
    }
}
