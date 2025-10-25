using Continuum93.Emulator;
using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{

    public class TestEXEC_LD_float
    {
        // LD r, fr
        [Fact]
        public void TestEXEC_LD_r_fr()
        {
            for (byte i = 0; i < 16; i++)
            {
                for (byte j = 0; j < 26; j++)
                {
                    Assembler cp = new();
                    using Computer computer = new();

                    byte[] reg = new byte[26];
                    float targetValue = 3.14f;

                    byte targetIndex = (byte)(j + 1);
                    if (targetIndex == 26)
                    {
                        targetIndex = 0;
                    }

                    char fReg1 = TUtils.Get8bitRegisterChar(targetIndex);
                    string fReg2 = TUtils.GetFloatRegisterString(i);

                    string code = $@"
                        LD {fReg1},{fReg2}
                        BREAK
                    ";

                    cp.Build(code);

                    byte[] compiled = cp.GetCompiledCode();
                    int ipo = compiled.Length;
                    Assert.Equal(5, compiled.Length);

                    computer.CPU.FREGS.SetRegister(i, targetValue);

                    computer.LoadMem(compiled);
                    computer.Run();

                    reg[targetIndex] = (byte)targetValue;

                    string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                        ipo, 0, 0,
                        reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                        reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                        reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]
                    );

                    string actual = computer.CPU.REGS.GetDebugInfo();

                    //Assert.True(expected == actual, $"Failed on i:{i}, j:{j}");
                    Assert.Equal(expected, actual);
                    TUtils.IncrementCountedTests("exec");
                }
            }
        }

        // LD rr, fr
        [Fact]
        public void TestEXEC_LD_rr_fr()
        {
            for (byte i = 0; i < 16; i++)
            {
                for (byte j = 0; j < 26; j++)
                {
                    Assembler cp = new();
                    using Computer computer = new();

                    byte[] reg = new byte[26];
                    float targetValue = 35012.14f;

                    byte targetIndex = (byte)(j + 1);
                    if (targetIndex == 26)
                    {
                        targetIndex = 0;
                    }

                    string fReg1 = TUtils.Get16bitRegisterString(targetIndex);
                    string fReg2 = TUtils.GetFloatRegisterString(i);

                    string code = $@"
                        LD {fReg1},{fReg2}
                        BREAK
                    ";

                    cp.Build(code);

                    byte[] compiled = cp.GetCompiledCode();
                    int ipo = compiled.Length;
                    Assert.Equal(5, compiled.Length);

                    computer.CPU.FREGS.SetRegister(i, targetValue);

                    computer.LoadMem(compiled);
                    computer.Run();

                    reg[targetIndex] = (byte)(targetValue / 256);
                    reg[(targetIndex + 1) % 26] = (byte)(targetValue % 256);

                    string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                        ipo, 0, 0,
                        reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                        reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                        reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]
                    );

                    string actual = computer.CPU.REGS.GetDebugInfo();

                    //Assert.True(expected == actual, $"Failed on i:{i}, j:{j}");
                    Assert.Equal(expected, actual);
                    TUtils.IncrementCountedTests("exec");
                }
            }
        }

        // LD rrr, fr
        [Fact]
        public void TestEXEC_LD_rrr_fr()
        {
            for (byte i = 0; i < 16; i++)
            {
                for (byte j = 0; j < 26; j++)
                {
                    Assembler cp = new();
                    using Computer computer = new();

                    byte[] reg = new byte[26];
                    float targetValue = 12936002.14f;

                    byte targetIndex = (byte)(j + 1);
                    if (targetIndex == 26)
                    {
                        targetIndex = 0;
                    }



                    string fReg1 = TUtils.Get24bitRegisterString(targetIndex);
                    string fReg2 = TUtils.GetFloatRegisterString(i);

                    string data = TUtils.GetFormattedAsm(
                        string.Format("LD {0},{1}", fReg1, fReg2),
                        "BREAK"
                    );

                    cp.Build(data);

                    byte[] compiled = cp.GetCompiledCode();
                    int ipo = compiled.Length;
                    Assert.Equal(5, compiled.Length);

                    computer.CPU.FREGS.SetRegister(i, targetValue);

                    computer.LoadMem(compiled);
                    computer.Run();

                    string bin = DataConverter.GetBinaryOfUint((uint)targetValue, 24);
                    byte[] bytes = DataConverter.GetBytesFromBinaryString(bin);

                    reg[targetIndex] = bytes[0];
                    reg[(targetIndex + 1) % 26] = bytes[1];
                    reg[(targetIndex + 2) % 26] = bytes[2];

                    string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                        ipo, 0, 0,
                        reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                        reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                        reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]
                    );

                    string actual = computer.CPU.REGS.GetDebugInfo();

                    //Assert.True(expected == actual, $"Failed on i:{i}, j:{j}");
                    Assert.Equal(expected, actual);
                    TUtils.IncrementCountedTests("exec");
                }
            }
        }

        // LD rrrr, fr
        [Fact]
        public void TestEXEC_LD_rrrr_fr()
        {
            for (byte i = 0; i < 16; i++)
            {
                for (byte j = 0; j < 26; j++)
                {
                    Assembler cp = new();
                    using Computer computer = new();

                    byte[] reg = new byte[26];
                    float targetValue = 3000000000.14f;

                    byte targetIndex = (byte)(j + 1);
                    if (targetIndex == 26)
                    {
                        targetIndex = 0;
                    }



                    string fReg1 = TUtils.Get32bitRegisterString(targetIndex);
                    string fReg2 = TUtils.GetFloatRegisterString(i);

                    string data = TUtils.GetFormattedAsm(
                        string.Format("LD {0},{1}", fReg1, fReg2),
                        "BREAK"
                    );

                    cp.Build(data);

                    byte[] compiled = cp.GetCompiledCode();
                    int ipo = compiled.Length;
                    Assert.Equal(5, compiled.Length);

                    computer.CPU.FREGS.SetRegister(i, targetValue);

                    computer.LoadMem(compiled);
                    computer.Run();

                    string bin = DataConverter.GetBinaryOfUint((uint)targetValue, 32);
                    byte[] bytes = DataConverter.GetBytesFromBinaryString(bin);

                    reg[targetIndex] = bytes[0];
                    reg[(targetIndex + 1) % 26] = bytes[1];
                    reg[(targetIndex + 2) % 26] = bytes[2];
                    reg[(targetIndex + 3) % 26] = bytes[3];

                    string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                        ipo, 0, 0,
                        reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                        reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                        reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]
                    );

                    string actual = computer.CPU.REGS.GetDebugInfo();

                    //Assert.True(expected == actual, $"Failed on i:{i}, j:{j}");
                    Assert.Equal(expected, actual);
                    TUtils.IncrementCountedTests("exec");
                }
            }
        }

        // LD (nnn), fr
        [Fact]
        public void TestEXEC_LD_InnnI_fr()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte j = 0; j < 16; j++)
            {
                string fReg = TUtils.GetFloatRegisterString(j);

                string code = $@"
                    LD (0x2000),{fReg}
                    BREAK";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                Assert.Equal(7, compiled.Length);

                computer.CPU.FREGS.SetRegister(j, (float)Math.PI);

                computer.LoadMem(compiled);
                computer.Run();

                float actual = computer.MEMC.GetFloatFromRAM(0x2000);

                //Assert.True(expected == actual, $"Failed on j:{j}");
                Assert.Equal((float)Math.PI, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        // LD (nnn + nnn), fr
        [Fact]
        public void TestEXEC_LD_Innn_nnnI_fr()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte j = 0; j < 16; j++)
            {
                string fReg = TUtils.GetFloatRegisterString(j);

                string code = $@"
                    LD (0x2000 + 4),{fReg}
                    BREAK";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                Assert.Equal(10, compiled.Length);

                computer.CPU.FREGS.SetRegister(j, (float)Math.PI);

                computer.LoadMem(compiled);
                computer.Run();

                float actual = computer.MEMC.GetFloatFromRAM(0x2000 + 4);

                //Assert.True(expected == actual, $"Failed on j:{j}");
                Assert.Equal((float)Math.PI, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        // LD (nnn + r), fr
        [Fact]
        public void TestEXEC_LD_Innn_rI_fr()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte j = 0; j < 16; j++)
            {
                string fReg = TUtils.GetFloatRegisterString(j);

                string code = $@"
                    LD A, 4
                    LD (0x2000 + A),{fReg}
                    BREAK";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                Assert.Equal(12, compiled.Length);

                computer.CPU.FREGS.SetRegister(j, (float)Math.PI);

                computer.LoadMem(compiled);
                computer.Run();

                float actual = computer.MEMC.GetFloatFromRAM(0x2000 + 4);

                //Assert.True(expected == actual, $"Failed on j:{j}");
                Assert.Equal((float)Math.PI, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        // LD (nnn + rr), fr
        [Fact]
        public void TestEXEC_LD_Innn_rrI_fr()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte j = 0; j < 16; j++)
            {
                string fReg = TUtils.GetFloatRegisterString(j);

                string code = $@"
                    LD AB, 4
                    LD (0x2000 + AB),{fReg}
                    BREAK";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                Assert.Equal(13, compiled.Length);

                computer.CPU.FREGS.SetRegister(j, (float)Math.PI);

                computer.LoadMem(compiled);
                computer.Run();

                float actual = computer.MEMC.GetFloatFromRAM(0x2000 + 4);

                //Assert.True(expected == actual, $"Failed on j:{j}");
                Assert.Equal((float)Math.PI, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        // LD (nnn + rrr), fr
        [Fact]
        public void TestEXEC_LD_Innn_rrrI_fr()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte j = 0; j < 16; j++)
            {
                string fReg = TUtils.GetFloatRegisterString(j);

                string code = $@"
                    LD ABC, 4
                    LD (0x2000 + ABC),{fReg}
                    BREAK";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                Assert.Equal(14, compiled.Length);

                computer.CPU.FREGS.SetRegister(j, (float)Math.PI);

                computer.LoadMem(compiled);
                computer.Run();

                float actual = computer.MEMC.GetFloatFromRAM(0x2000 + 4);

                //Assert.True(expected == actual, $"Failed on j:{j}");
                Assert.Equal((float)Math.PI, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        // LD (rrr), fr
        [Fact]
        public void TestEXEC_LD_IrrrI_fr()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte j = 0; j < 16; j++)
            {
                string fReg = TUtils.GetFloatRegisterString(j);

                string code = $@"
                    LD (ABC),{fReg}
                    BREAK";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                Assert.Equal(5, compiled.Length);

                computer.CPU.FREGS.SetRegister(j, (float)Math.PI);
                computer.CPU.REGS.Set24BitRegister(0, 0x2000);

                computer.LoadMem(compiled);
                computer.Run();

                float actual = computer.MEMC.GetFloatFromRAM(0x2000);

                //Assert.True(expected == actual, $"Failed on j:{j}");
                Assert.Equal((float)Math.PI, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        // LD (rrr +/- nnn), fr
        [Fact]
        public void TestEXEC_LD_Irrr_nnnI_fr()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte j = 0; j < 16; j++)
            {
                string fReg = TUtils.GetFloatRegisterString(j);

                string code = $@"
                    LD (ABC + 5),{fReg}
                    BREAK";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                Assert.Equal(8, compiled.Length);

                computer.CPU.FREGS.SetRegister(j, (float)Math.PI);
                computer.CPU.REGS.Set24BitRegister(0, 0x2000);      // ABC

                computer.LoadMem(compiled);
                computer.Run();

                float actual = computer.MEMC.GetFloatFromRAM(0x2000 + 5);

                //Assert.True(expected == actual, $"Failed on j:{j}");
                Assert.Equal((float)Math.PI, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        // LD (rrr + r), fr
        [Fact]
        public void TestEXEC_LD_Irrr_rI_fr()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte j = 0; j < 16; j++)
            {
                string fReg = TUtils.GetFloatRegisterString(j);

                string code = $@"
                    LD Z, 5
                    LD (ABC + Z),{fReg}
                    BREAK";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                Assert.Equal(10, compiled.Length);

                computer.CPU.FREGS.SetRegister(j, (float)Math.PI);
                computer.CPU.REGS.Set24BitRegister(0, 0x2000);      // ABC

                computer.LoadMem(compiled);
                computer.Run();

                float actual = computer.MEMC.GetFloatFromRAM(0x2000 + 5);

                //Assert.True(expected == actual, $"Failed on j:{j}");
                Assert.Equal((float)Math.PI, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        // LD (rrr + rr), fr
        [Fact]
        public void TestEXEC_LD_Irrr_rrI_fr()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte j = 0; j < 16; j++)
            {
                string fReg = TUtils.GetFloatRegisterString(j);

                string code = $@"
                    LD YZ, 5
                    LD (ABC + YZ),{fReg}
                    BREAK";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                Assert.Equal(11, compiled.Length);

                computer.CPU.FREGS.SetRegister(j, (float)Math.PI);
                computer.CPU.REGS.Set24BitRegister(0, 0x2000);      // ABC

                computer.LoadMem(compiled);
                computer.Run();

                float actual = computer.MEMC.GetFloatFromRAM(0x2000 + 5);

                //Assert.True(expected == actual, $"Failed on j:{j}");
                Assert.Equal((float)Math.PI, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        // LD (rrr + rr), fr
        [Fact]
        public void TestEXEC_LD_Irrr_rrrI_fr()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte j = 0; j < 16; j++)
            {
                string fReg = TUtils.GetFloatRegisterString(j);

                string code = $@"
                    LD XYZ, 5
                    LD (ABC + XYZ),{fReg}
                    BREAK";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                Assert.Equal(12, compiled.Length);

                computer.CPU.FREGS.SetRegister(j, (float)Math.PI);
                computer.CPU.REGS.Set24BitRegister(0, 0x2000);      // ABC

                computer.LoadMem(compiled);
                computer.Run();

                float actual = computer.MEMC.GetFloatFromRAM(0x2000 + 5);

                //Assert.True(expected == actual, $"Failed on j:{j}");
                Assert.Equal((float)Math.PI, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        // LD fr, nnnn
        [Fact]
        public void TestEXEC_LD_fr_nnnn()
        {
            for (byte i = 0; i < 16; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                string fReg = TUtils.GetFloatRegisterString(i);
                string data = TUtils.GetFormattedAsm(
                    string.Format("LD {0}, 3.14159265359", fReg),
                    "BREAK"
                );

                cp.Build(data);

                byte[] compiled = cp.GetCompiledCode();
                Assert.Equal(8, compiled.Length);

                computer.LoadMem(compiled);
                computer.Run();

                float actual = computer.CPU.FREGS.GetRegister(i);

                //Assert.True(expected == actual, $"Failed on i:{i}");
                Assert.Equal((float)Math.PI, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        // LD fr, nnn (binary)
        [Fact]
        public void TestEXEC_LD_fr_nnn_binary()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD F0, 0b01000000010010010000111111011011
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();
            Assert.Equal(8, compiled.Length);

            computer.LoadMem(compiled);
            computer.Run();

            float actual = computer.CPU.FREGS.GetRegister(0);

            Assert.Equal((float)Math.PI, actual);
            TUtils.IncrementCountedTests("exec");
        }

        // LD fr, r
        [Fact]
        public void TestEXEC_LD_fr_r()
        {
            for (byte i = 0; i < 26; i++)
            {
                for (byte j = 0; j < 16; j++)
                {
                    Assembler cp = new();
                    using Computer computer = new();

                    float[] fReg = new float[16];
                    byte targetValue = 0xF0;

                    byte targetIndex = (byte)(j + 1);
                    if (targetIndex == 16)
                    {
                        targetIndex = 0;
                    }



                    string fReg1 = TUtils.GetFloatRegisterString(targetIndex);
                    char fReg2 = TUtils.Get8bitRegisterChar(i);

                    string data = TUtils.GetFormattedAsm(
                        string.Format("LD {0},{1}", fReg1, fReg2),
                        "BREAK"
                    );

                    cp.Build(data);

                    byte[] compiled = cp.GetCompiledCode();
                    Assert.Equal(5, compiled.Length);

                    computer.CPU.REGS.Set8BitRegister(i, targetValue);

                    computer.LoadMem(compiled);
                    computer.Run();

                    fReg[targetIndex] = targetValue;

                    string expected = string.Format(computer.CPU.FREGS.GetDebugTemplate(),
                        fReg[0], fReg[1], fReg[2], fReg[3], fReg[4], fReg[5], fReg[6], fReg[7],
                        fReg[8], fReg[9], fReg[10], fReg[11], fReg[12], fReg[13], fReg[14], fReg[15]
                    );

                    string actual = computer.CPU.FREGS.GetDebugInfo();

                    Assert.True(expected == actual, $"Failed on i:{i}, j:{j}");
                    TUtils.IncrementCountedTests("exec");
                }
            }
        }

        // LD fr, rr
        [Fact]
        public void TestEXEC_LD_fr_rr()
        {
            for (byte i = 0; i < 26; i++)
            {
                for (byte j = 0; j < 16; j++)
                {
                    Assembler cp = new();
                    using Computer computer = new();

                    float[] fReg = new float[16];
                    ushort targetValue = 0xF0F0;

                    byte targetIndex = (byte)(j + 1);
                    if (targetIndex == 16)
                    {
                        targetIndex = 0;
                    }



                    string fReg1 = TUtils.GetFloatRegisterString(targetIndex);
                    string fReg2 = TUtils.Get16bitRegisterString(i);

                    string data = TUtils.GetFormattedAsm(
                        string.Format("LD {0},{1}", fReg1, fReg2),
                        "BREAK"
                    );

                    cp.Build(data);

                    byte[] compiled = cp.GetCompiledCode();
                    Assert.Equal(5, compiled.Length);

                    computer.CPU.REGS.Set16BitRegister(i, targetValue);

                    computer.LoadMem(compiled);
                    computer.Run();

                    fReg[targetIndex] = targetValue;

                    string expected = string.Format(computer.CPU.FREGS.GetDebugTemplate(),
                        fReg[0], fReg[1], fReg[2], fReg[3], fReg[4], fReg[5], fReg[6], fReg[7],
                        fReg[8], fReg[9], fReg[10], fReg[11], fReg[12], fReg[13], fReg[14], fReg[15]
                    );

                    string actual = computer.CPU.FREGS.GetDebugInfo();

                    Assert.True(expected == actual, $"Failed on i:{i}, j:{j}");
                    TUtils.IncrementCountedTests("exec");
                }
            }
        }

        // LD fr, rrr
        [Fact]
        public void TestEXEC_LD_fr_rrr()
        {
            for (byte i = 0; i < 26; i++)
            {
                for (byte j = 0; j < 16; j++)
                {
                    Assembler cp = new();
                    using Computer computer = new();

                    float[] fReg = new float[16];
                    uint targetValue = 0xF0F0F0;

                    byte targetIndex = (byte)(j + 1);
                    if (targetIndex == 16)
                    {
                        targetIndex = 0;
                    }



                    string fReg1 = TUtils.GetFloatRegisterString(targetIndex);
                    string fReg2 = TUtils.Get24bitRegisterString(i);

                    string data = TUtils.GetFormattedAsm(
                        string.Format("LD {0},{1}", fReg1, fReg2),
                        "BREAK"
                    );

                    cp.Build(data);

                    byte[] compiled = cp.GetCompiledCode();
                    Assert.Equal(5, compiled.Length);

                    computer.CPU.REGS.Set24BitRegister(i, targetValue);

                    computer.LoadMem(compiled);
                    computer.Run();

                    fReg[targetIndex] = targetValue;

                    string expected = string.Format(computer.CPU.FREGS.GetDebugTemplate(),
                        fReg[0], fReg[1], fReg[2], fReg[3], fReg[4], fReg[5], fReg[6], fReg[7],
                        fReg[8], fReg[9], fReg[10], fReg[11], fReg[12], fReg[13], fReg[14], fReg[15]
                    );

                    string actual = computer.CPU.FREGS.GetDebugInfo();

                    Assert.True(expected == actual, $"Failed on i:{i}, j:{j}");
                    TUtils.IncrementCountedTests("exec");
                }
            }
        }

        // LD fr, rrrr
        [Fact]
        public void TestEXEC_LD_fr_rrrr()
        {
            for (byte i = 0; i < 26; i++)
            {
                for (byte j = 0; j < 16; j++)
                {
                    Assembler cp = new();
                    using Computer computer = new();

                    float[] fReg = new float[16];
                    uint targetValue = 0xF1F0F0FA;

                    byte targetIndex = (byte)(j + 1);
                    if (targetIndex == 16)
                    {
                        targetIndex = 0;
                    }



                    string fReg1 = TUtils.GetFloatRegisterString(targetIndex);
                    string fReg2 = TUtils.Get32bitRegisterString(i);

                    string data = TUtils.GetFormattedAsm(
                        string.Format("LD {0},{1}", fReg1, fReg2),
                        "BREAK"
                    );

                    cp.Build(data);

                    byte[] compiled = cp.GetCompiledCode();
                    Assert.Equal(5, compiled.Length);

                    computer.CPU.REGS.Set32BitRegister(i, targetValue);

                    computer.LoadMem(compiled);
                    computer.Run();

                    fReg[targetIndex] = targetValue;

                    string expected = string.Format(computer.CPU.FREGS.GetDebugTemplate(),
                        fReg[0], fReg[1], fReg[2], fReg[3], fReg[4], fReg[5], fReg[6], fReg[7],
                        fReg[8], fReg[9], fReg[10], fReg[11], fReg[12], fReg[13], fReg[14], fReg[15]
                    );

                    string actual = computer.CPU.FREGS.GetDebugInfo();

                    Assert.True(expected == actual, $"Failed on i:{i}, j:{j}");
                    TUtils.IncrementCountedTests("exec");
                }
            }
        }

        // LD fr, (nnn)
        [Fact]
        public void TestEXEC_LD_fr_InnnI()
        {
            for (byte j = 0; j < 16; j++)
            {
                Assembler cp = new();
                using Computer computer = new();

                float[] fReg = new float[16];

                string fReg1 = TUtils.GetFloatRegisterString(j);

                string data = TUtils.GetFormattedAsm(
                    string.Format("LD {0},(0x2000)", fReg1),
                    "BREAK"
                );

                cp.Build(data);

                byte[] compiled = cp.GetCompiledCode();
                Assert.Equal(7, compiled.Length);

                computer.CPU.REGS.Set24BitRegister(0, 0x2000);
                computer.MEMC.SetFloatToRam(0x2000, (float)Math.PI);

                computer.LoadMem(compiled);
                computer.Run();

                fReg[j] = 3.1415927f;

                string expected = string.Format(computer.CPU.FREGS.GetDebugTemplate(),
                    fReg[0], fReg[1], fReg[2], fReg[3], fReg[4], fReg[5], fReg[6], fReg[7],
                    fReg[8], fReg[9], fReg[10], fReg[11], fReg[12], fReg[13], fReg[14], fReg[15]
                );

                string actual = computer.CPU.FREGS.GetDebugInfo();

                //Assert.True(expected == actual, $"Failed on j:{j}");
                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        // LD fr, (nnn + nnn)
        [Fact]
        public void TestEXEC_LD_fr_Innn_nnnI()
        {
            for (byte j = 0; j < 16; j++)
            {
                Assembler cp = new();
                using Computer computer = new();

                float[] fReg = new float[16];

                string fReg1 = TUtils.GetFloatRegisterString(j);

                string data = TUtils.GetFormattedAsm(
                    string.Format("LD {0},(0x2000 + 100)", fReg1),
                    "BREAK"
                );

                cp.Build(data);

                byte[] compiled = cp.GetCompiledCode();
                Assert.Equal(10, compiled.Length);

                computer.MEMC.SetFloatToRam(0x2000 + 100, (float)Math.PI);

                computer.LoadMem(compiled);
                computer.Run();

                fReg[j] = 3.1415927f;

                string expected = string.Format(computer.CPU.FREGS.GetDebugTemplate(),
                    fReg[0], fReg[1], fReg[2], fReg[3], fReg[4], fReg[5], fReg[6], fReg[7],
                    fReg[8], fReg[9], fReg[10], fReg[11], fReg[12], fReg[13], fReg[14], fReg[15]
                );

                string actual = computer.CPU.FREGS.GetDebugInfo();

                //Assert.True(expected == actual, $"Failed on j:{j}");
                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        // LD fr, (nnn + r)
        [Fact]
        public void TestEXEC_LD_fr_Innn_rI()
        {
            for (byte j = 0; j < 16; j++)
            {
                Assembler cp = new();
                using Computer computer = new();
                computer.CPU.REGS.A = 100;

                float[] fReg = new float[16];

                string fReg1 = TUtils.GetFloatRegisterString(j);

                string data = TUtils.GetFormattedAsm(
                    string.Format("LD {0},(0x2000 + A)", fReg1),
                    "BREAK"
                );

                cp.Build(data);

                byte[] compiled = cp.GetCompiledCode();
                Assert.Equal(8, compiled.Length);

                computer.MEMC.SetFloatToRam(0x2000 + 100, (float)Math.PI);

                computer.LoadMem(compiled);
                computer.Run();

                fReg[j] = 3.1415927f;

                string expected = string.Format(computer.CPU.FREGS.GetDebugTemplate(),
                    fReg[0], fReg[1], fReg[2], fReg[3], fReg[4], fReg[5], fReg[6], fReg[7],
                    fReg[8], fReg[9], fReg[10], fReg[11], fReg[12], fReg[13], fReg[14], fReg[15]
                );

                string actual = computer.CPU.FREGS.GetDebugInfo();

                //Assert.True(expected == actual, $"Failed on j:{j}");
                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        // LD fr, (nnn + rr)
        [Fact]
        public void TestEXEC_LD_fr_Innn_rrI()
        {
            for (byte j = 0; j < 16; j++)
            {
                Assembler cp = new();
                using Computer computer = new();
                computer.CPU.REGS.AB = 100;

                float[] fReg = new float[16];

                string fReg1 = TUtils.GetFloatRegisterString(j);

                string data = TUtils.GetFormattedAsm(
                    string.Format("LD {0},(0x2000 + AB)", fReg1),
                    "BREAK"
                );

                cp.Build(data);

                byte[] compiled = cp.GetCompiledCode();
                Assert.Equal(8, compiled.Length);

                computer.MEMC.SetFloatToRam(0x2000 + 100, (float)Math.PI);

                computer.LoadMem(compiled);
                computer.Run();

                fReg[j] = 3.1415927f;

                string expected = string.Format(computer.CPU.FREGS.GetDebugTemplate(),
                    fReg[0], fReg[1], fReg[2], fReg[3], fReg[4], fReg[5], fReg[6], fReg[7],
                    fReg[8], fReg[9], fReg[10], fReg[11], fReg[12], fReg[13], fReg[14], fReg[15]
                );

                string actual = computer.CPU.FREGS.GetDebugInfo();

                //Assert.True(expected == actual, $"Failed on j:{j}");
                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        // LD fr, (nnn + rrr)
        [Fact]
        public void TestEXEC_LD_fr_Innn_rrrI()
        {
            for (byte j = 0; j < 16; j++)
            {
                Assembler cp = new();
                using Computer computer = new();
                computer.CPU.REGS.ABC = 100;

                float[] fReg = new float[16];

                string fReg1 = TUtils.GetFloatRegisterString(j);

                string data = TUtils.GetFormattedAsm(
                    string.Format("LD {0},(0x2000 + ABC)", fReg1),
                    "BREAK"
                );

                cp.Build(data);

                byte[] compiled = cp.GetCompiledCode();
                Assert.Equal(8, compiled.Length);

                computer.MEMC.SetFloatToRam(0x2000 + 100, (float)Math.PI);

                computer.LoadMem(compiled);
                computer.Run();

                fReg[j] = 3.1415927f;

                string expected = string.Format(computer.CPU.FREGS.GetDebugTemplate(),
                    fReg[0], fReg[1], fReg[2], fReg[3], fReg[4], fReg[5], fReg[6], fReg[7],
                    fReg[8], fReg[9], fReg[10], fReg[11], fReg[12], fReg[13], fReg[14], fReg[15]
                );

                string actual = computer.CPU.FREGS.GetDebugInfo();

                //Assert.True(expected == actual, $"Failed on j:{j}");
                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        // LD fr, (rrr)
        [Fact]
        public void TestEXEC_LD_fr_IrrrI()
        {
            for (byte j = 0; j < 16; j++)
            {
                Assembler cp = new();
                using Computer computer = new();

                float[] fReg = new float[16];

                byte targetIndex = (byte)(j + 1);
                if (targetIndex == 16)
                    targetIndex = 0;

                string fReg1 = TUtils.GetFloatRegisterString(targetIndex);
                string fReg2 = TUtils.Get24bitRegisterString(0);

                string data = TUtils.GetFormattedAsm(
                    string.Format("LD {0},({1})", fReg1, fReg2),
                    "BREAK"
                );

                cp.Build(data);

                byte[] compiled = cp.GetCompiledCode();
                Assert.Equal(5, compiled.Length);

                computer.CPU.REGS.Set24BitRegister(0, 0x2000);
                computer.MEMC.SetFloatToRam(0x2000, (float)Math.PI);

                computer.LoadMem(compiled);
                computer.Run();

                fReg[targetIndex] = 3.1415927f;

                string expected = string.Format(computer.CPU.FREGS.GetDebugTemplate(),
                    fReg[0], fReg[1], fReg[2], fReg[3], fReg[4], fReg[5], fReg[6], fReg[7],
                    fReg[8], fReg[9], fReg[10], fReg[11], fReg[12], fReg[13], fReg[14], fReg[15]
                );

                string actual = computer.CPU.FREGS.GetDebugInfo();

                //Assert.True(expected == actual, $"Failed on j:{j}");
                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        // LD fr, (rrr + nnn)
        [Fact]
        public void TestEXEC_LD_fr_Irrr_nnnI()
        {
            for (byte j = 0; j < 16; j++)
            {
                Assembler cp = new();
                using Computer computer = new();

                float[] fReg = new float[16];

                byte targetIndex = (byte)(j + 1);
                if (targetIndex == 16)
                    targetIndex = 0;

                string fReg1 = TUtils.GetFloatRegisterString(targetIndex);
                string fReg2 = TUtils.Get24bitRegisterString(0);

                string data = TUtils.GetFormattedAsm(
                    string.Format("LD {0},({1} + 100)", fReg1, fReg2),
                    "BREAK"
                );

                cp.Build(data);

                byte[] compiled = cp.GetCompiledCode();
                Assert.Equal(8, compiled.Length);

                computer.CPU.REGS.Set24BitRegister(0, 0x2000);
                computer.MEMC.SetFloatToRam(0x2000 + 100, (float)Math.PI);

                computer.LoadMem(compiled);
                computer.Run();

                fReg[targetIndex] = 3.1415927f;

                string expected = string.Format(computer.CPU.FREGS.GetDebugTemplate(),
                    fReg[0], fReg[1], fReg[2], fReg[3], fReg[4], fReg[5], fReg[6], fReg[7],
                    fReg[8], fReg[9], fReg[10], fReg[11], fReg[12], fReg[13], fReg[14], fReg[15]
                );

                string actual = computer.CPU.FREGS.GetDebugInfo();

                //Assert.True(expected == actual, $"Failed on j:{j}");
                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        // LD fr, (rrr + r)
        [Fact]
        public void TestEXEC_LD_fr_Irrr_rI()
        {
            for (byte j = 0; j < 16; j++)
            {
                Assembler cp = new();
                using Computer computer = new();
                computer.CPU.REGS.Z = 100;

                float[] fReg = new float[16];

                byte targetIndex = (byte)(j + 1);
                if (targetIndex == 16)
                    targetIndex = 0;

                string fReg1 = TUtils.GetFloatRegisterString(targetIndex);
                string fReg2 = TUtils.Get24bitRegisterString(0);

                string data = TUtils.GetFormattedAsm(
                    string.Format("LD {0},({1} + Z)", fReg1, fReg2),
                    "BREAK"
                );

                cp.Build(data);

                byte[] compiled = cp.GetCompiledCode();
                Assert.Equal(6, compiled.Length);

                computer.CPU.REGS.Set24BitRegister(0, 0x2000);
                computer.MEMC.SetFloatToRam(0x2000 + 100, (float)Math.PI);

                computer.LoadMem(compiled);
                computer.Run();

                fReg[targetIndex] = 3.1415927f;

                string expected = string.Format(computer.CPU.FREGS.GetDebugTemplate(),
                    fReg[0], fReg[1], fReg[2], fReg[3], fReg[4], fReg[5], fReg[6], fReg[7],
                    fReg[8], fReg[9], fReg[10], fReg[11], fReg[12], fReg[13], fReg[14], fReg[15]
                );

                string actual = computer.CPU.FREGS.GetDebugInfo();

                //Assert.True(expected == actual, $"Failed on j:{j}");
                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        // LD fr, (rrr + rr)
        [Fact]
        public void TestEXEC_LD_fr_Irrr_rrI()
        {
            for (byte j = 0; j < 16; j++)
            {
                Assembler cp = new();
                using Computer computer = new();
                computer.CPU.REGS.YZ = 100;

                float[] fReg = new float[16];

                byte targetIndex = (byte)(j + 1);
                if (targetIndex == 16)
                    targetIndex = 0;

                string fReg1 = TUtils.GetFloatRegisterString(targetIndex);
                string fReg2 = TUtils.Get24bitRegisterString(0);

                string data = TUtils.GetFormattedAsm(
                    string.Format("LD {0},({1} + YZ)", fReg1, fReg2),
                    "BREAK"
                );

                cp.Build(data);

                byte[] compiled = cp.GetCompiledCode();
                Assert.Equal(6, compiled.Length);

                computer.CPU.REGS.Set24BitRegister(0, 0x2000);
                computer.MEMC.SetFloatToRam(0x2000 + 100, (float)Math.PI);

                computer.LoadMem(compiled);
                computer.Run();

                fReg[targetIndex] = 3.1415927f;

                string expected = string.Format(computer.CPU.FREGS.GetDebugTemplate(),
                    fReg[0], fReg[1], fReg[2], fReg[3], fReg[4], fReg[5], fReg[6], fReg[7],
                    fReg[8], fReg[9], fReg[10], fReg[11], fReg[12], fReg[13], fReg[14], fReg[15]
                );

                string actual = computer.CPU.FREGS.GetDebugInfo();

                //Assert.True(expected == actual, $"Failed on j:{j}");
                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        // LD fr, (rrr + rrr)
        [Fact]
        public void TestEXEC_LD_fr_Irrr_rrrI()
        {
            for (byte j = 0; j < 16; j++)
            {
                Assembler cp = new();
                using Computer computer = new();
                computer.CPU.REGS.XYZ = 100;

                float[] fReg = new float[16];

                byte targetIndex = (byte)(j + 1);
                if (targetIndex == 16)
                    targetIndex = 0;

                string fReg1 = TUtils.GetFloatRegisterString(targetIndex);
                string fReg2 = TUtils.Get24bitRegisterString(0);

                string data = TUtils.GetFormattedAsm(
                    string.Format("LD {0},({1} + XYZ)", fReg1, fReg2),
                    "BREAK"
                );

                cp.Build(data);

                byte[] compiled = cp.GetCompiledCode();
                Assert.Equal(6, compiled.Length);

                computer.CPU.REGS.Set24BitRegister(0, 0x2000);
                computer.MEMC.SetFloatToRam(0x2000 + 100, (float)Math.PI);

                computer.LoadMem(compiled);
                computer.Run();

                fReg[targetIndex] = 3.1415927f;

                string expected = string.Format(computer.CPU.FREGS.GetDebugTemplate(),
                    fReg[0], fReg[1], fReg[2], fReg[3], fReg[4], fReg[5], fReg[6], fReg[7],
                    fReg[8], fReg[9], fReg[10], fReg[11], fReg[12], fReg[13], fReg[14], fReg[15]
                );

                string actual = computer.CPU.FREGS.GetDebugInfo();

                //Assert.True(expected == actual, $"Failed on j:{j}");
                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        // LD fr, fr
        [Fact]
        public void TestEXEC_LD_fr_fr()
        {
            for (byte i = 0; i < 16; i++)
            {
                for (byte j = 0; j < 16; j++)
                {
                    Assembler cp = new();
                    using Computer computer = new();

                    float[] fReg = new float[16];
                    float targetValue = (i + 1) * (j + 1) * 2.34f;

                    byte targetIndex = (byte)(j + 1);
                    if (targetIndex == 16)
                    {
                        targetIndex = 0;
                    }

                    string fReg1 = TUtils.GetFloatRegisterString(targetIndex);
                    string fReg2 = TUtils.GetFloatRegisterString(i);

                    string data = TUtils.GetFormattedAsm(
                        string.Format("LD {0},{1}", fReg1, fReg2),
                        "BREAK"
                    );

                    cp.Build(data);

                    byte[] compiled = cp.GetCompiledCode();
                    Assert.Equal(5, compiled.Length);

                    computer.CPU.FREGS.SetRegister(i, targetValue);

                    computer.LoadMem(compiled);
                    computer.Run();

                    fReg[i] = targetValue;
                    fReg[targetIndex] = fReg[i];

                    string expected = string.Format(computer.CPU.FREGS.GetDebugTemplate(),
                        fReg[0], fReg[1], fReg[2], fReg[3], fReg[4], fReg[5], fReg[6], fReg[7],
                        fReg[8], fReg[9], fReg[10], fReg[11], fReg[12], fReg[13], fReg[14], fReg[15]
                    );

                    string actual = computer.CPU.FREGS.GetDebugInfo();

                    Assert.True(expected == actual, $"Failed on i:{i}, j:{j}");
                    TUtils.IncrementCountedTests("exec");
                }
            }
        }



        
    }
}
