using Continuum93.Emulator;
using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;
using Newtonsoft.Json.Linq;
using System.Net;

namespace ExecutionTests
{

    public class TestEXEC_LD
    {
        // LD r, [n, r, (nnn), (rrr)]
        [Fact]
        public void TestEXEC_LD_r_n()
        {
            for (byte i = 0; i < 26; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                string code = $@"
                    LD {TUtils.Get8bitRegisterChar(i)},{i * 9}
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[i] = (byte)(i * 9);

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_r_r()
        {
            for (byte i = 0; i < 26; i++)
            {
                for (byte j = 0; j < 26; j++)
                {
                    Assembler cp = new();
                    using Computer computer = new();

                    byte[] reg = new byte[26];

                    byte targetIndex = (byte)(j + 1);
                    if (targetIndex == 26)
                    {
                        targetIndex = 0;
                    }
                    uint spr = 0;
                    uint spc = 0;

                    string code = $@"
                        LD {TUtils.Get8bitRegisterChar(i)},{i * 9 + 1}
                        LD {TUtils.Get8bitRegisterChar(targetIndex)},{TUtils.Get8bitRegisterChar(i)}
                        BREAK
                    ";

                    cp.Build(code);

                    byte[] compiled = cp.GetCompiledCode();
                    int ipo = compiled.Length;

                    computer.LoadMem(compiled);
                    computer.Run();

                    reg[i] = (byte)(i * 9 + 1);
                    reg[targetIndex] = reg[i];

                    string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                        ipo, spr, spc,
                        reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                        reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                        reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                    string actual = computer.CPU.REGS.GetDebugInfo();

                    Assert.Equal(expected, actual);
                    TUtils.IncrementCountedTests("exec");
                }

            }
        }

        [Fact]
        public void TestEXEC_LD_r_InnnI()
        {
            for (byte i = 0; i < 26; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("LD {0},({1})", TUtils.Get8bitRegisterChar(i), 10000),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[i] = 40;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_r_Innn_nnnI_positive()
        {
            for (byte i = 0; i < 26; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });

                string code = $@"
                    LD {TUtils.Get8bitRegisterChar(i)},(10000+4)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[i] = 80;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_r_Innn_nnnI_negative()
        {
            for (byte i = 0; i < 26; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });

                string code = $@"
                    LD {TUtils.Get8bitRegisterChar(i)},(10000-3)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[i] = 10;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_r_Innn_rI_positive()
        {
            for (byte i = 0; i < 25; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });
                computer.CPU.REGS.Z = 4;

                string code = $@"
                    LD {TUtils.Get8bitRegisterChar(i)},(10000+Z)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[i] = 80;
                reg[25] = 4;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_r_Innn_rI_negative()
        {
            for (byte i = 0; i < 25; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });
                computer.CPU.REGS.Z = 253;  // -3

                string code = $@"
                    LD {TUtils.Get8bitRegisterChar(i)},(10000+Z)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[i] = 10;
                reg[25] = 253;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_r_Innn_rrI_positive()
        {
            for (byte i = 0; i < 24; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });
                computer.CPU.REGS.YZ = 4;

                string code = $@"
                    LD {TUtils.Get8bitRegisterChar(i)},(10000+ YZ)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[i] = 80;
                reg[24] = 0;
                reg[25] = 4;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_r_Innn_rrI_negative()
        {
            for (byte i = 0; i < 24; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });
                computer.CPU.REGS.YZ = 0xFFFD;  // -3

                string code = $@"
                    LD {TUtils.Get8bitRegisterChar(i)},(10000+ YZ)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[i] = 10;
                reg[24] = 0xFF;
                reg[25] = 0xFD;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_r_Innn_rrrI_positive()
        {
            for (byte i = 0; i < 23; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });
                computer.CPU.REGS.XYZ = 4;

                string code = $@"
                    LD {TUtils.Get8bitRegisterChar(i)},(10000 + XYZ)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[i] = 80;
                reg[23] = 0;
                reg[24] = 0;
                reg[25] = 4;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_r_Innn_rrrI_negative()
        {
            for (byte i = 0; i < 23; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });
                computer.CPU.REGS.XYZ = 0xFFFFFD;  // -3

                string code = $@"
                    LD {TUtils.Get8bitRegisterChar(i)},(10000 + XYZ)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[i] = 10;
                reg[23] = 0xFF;
                reg[24] = 0xFF;
                reg[25] = 0xFD;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_r_IrrrI()
        {
            for (byte i = 3; i < 26; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("LD ABC,10001"),
                        string.Format("LD {0},(ABC)", TUtils.Get8bitRegisterChar(i)),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[1] = 39; // The ABC register
                reg[2] = 17;

                reg[i] = 50;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_r_Irrr_nnnI_positive()
        {
            for (byte i = 3; i < 26; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });

                string code = $@"
                    LD ABC,10000
                    LD {TUtils.Get8bitRegisterChar(i)},(ABC + 4)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[1] = 39; // The ABC register
                reg[2] = 16;

                reg[i] = 80;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_r_Irrr_nnnI_negative()
        {
            for (byte i = 3; i < 26; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });

                string code = $@"
                    LD ABC,10000
                    LD {TUtils.Get8bitRegisterChar(i)},(ABC - 3)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[1] = 39; // The ABC register
                reg[2] = 16;

                reg[i] = 10;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_r_Irrr_rI_positive()
        {
            for (byte i = 4; i < 26; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });

                string code = $@"
                    LD ABC,10000
                    LD D, 4
                    LD {TUtils.Get8bitRegisterChar(i)},(ABC + D)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[1] = 39;    // ABC register
                reg[2] = 16;
                reg[3] = 4;     // D register

                reg[i] = 80;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_r_Irrr_rI_negative()
        {
            for (byte i = 4; i < 26; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });

                string code = $@"
                    LD ABC,10000
                    LD D, -3
                    LD {TUtils.Get8bitRegisterChar(i)},(ABC - D)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[1] = 39; // The ABC register
                reg[2] = 16;
                reg[3] = 253; // D register: -3

                reg[i] = 10;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_r_Irrr_rrI_positive()
        {
            for (byte i = 5; i < 26; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });

                string code = $@"
                    LD ABC,10000
                    LD DE, 4
                    LD {TUtils.Get8bitRegisterChar(i)},(ABC + DE)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[1] = 39;    // ABC register
                reg[2] = 16;
                reg[3] = 0;     // DE register
                reg[4] = 4;

                reg[i] = 80;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_r_Irrr_rrI_negative()
        {
            for (byte i = 4; i < 26; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });

                string code = $@"
                    LD ABC,10000
                    LD DE, -3
                    LD {TUtils.Get8bitRegisterChar(i)},(ABC - DE)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[1] = 39;    // ABC register
                reg[2] = 16;
                reg[3] = 0xFF;   // DE register: -3
                reg[4] = 0xFD;

                reg[i] = 10;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }


        [Fact]
        public void TestEXEC_LD_r_Irrr_rrrI_positive()
        {
            for (byte i = 6; i < 26; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });

                string code = $@"
                    LD ABC,10000
                    LD DEF, 4
                    LD {TUtils.Get8bitRegisterChar(i)},(ABC + DEF)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[1] = 39;    // ABC register
                reg[2] = 16;
                reg[3] = 0;     // DEF register
                reg[4] = 0;
                reg[5] = 4;

                reg[i] = 80;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_r_Irrr_rrrI_negative()
        {
            for (byte i = 5; i < 26; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });

                string code = $@"
                    LD ABC,10000
                    LD DEF, -3
                    LD {TUtils.Get8bitRegisterChar(i)},(ABC - DEF)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[1] = 39;    // ABC register
                reg[2] = 16;
                reg[3] = 0xFF;   // DEF register: -3
                reg[4] = 0xFF;
                reg[5] = 0xFD;

                reg[i] = 10;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }



        // LD rr, [n, r, (nnn), (rrr)]
        [Fact]
        public void TestEXEC_LD_rr_nn()
        {
            for (byte i = 0; i < 26; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                ushort expectedValue = (ushort)(i * 2520 + 1);

                string code = $@"
                    LD {TUtils.Get16bitRegisterString(i)},{expectedValue}
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                byte[] ExpectedValueBytes = DataConverter.GetBytesFrom16bit(expectedValue);
                reg[i] = ExpectedValueBytes[0];

                // Last double register is ZA, so the least significant byte is in A
                reg[(i == 25) ? 0 : (i + 1)] = ExpectedValueBytes[1];


                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rr_r()
        {
            for (byte i = 0; i < 26; i++)
            {
                for (byte j = 0; j < 26; j++)
                {
                    Assembler cp = new();
                    using Computer computer = new();

                    byte[] reg = new byte[26];

                    byte targetIndex = (byte)(j + 2);
                    if (targetIndex > 25)
                        targetIndex = (byte)(targetIndex - 26);

                    uint spr = 0;
                    uint spc = 0;

                    byte expectedValue = (byte)(i + 1);

                    string code = $@"
                        LD {TUtils.Get8bitRegisterChar(i)},{expectedValue}
                        LD {TUtils.Get16bitRegisterString(targetIndex)},{TUtils.Get8bitRegisterChar(i)}
                        BREAK
                    ";

                    cp.Build(code);

                    byte[] compiled = cp.GetCompiledCode();
                    int ipo = compiled.Length;

                    computer.LoadMem(compiled);
                    computer.Run();

                    reg[i] = expectedValue;

                    reg[targetIndex] = 0;
                    reg[(targetIndex + 1) % 26] = expectedValue;

                    string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                        ipo, spr, spc,
                        reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                        reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                        reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                    string actual = computer.CPU.REGS.GetDebugInfo();

                    Assert.Equal(expected, actual);
                    TUtils.IncrementCountedTests("exec");
                }

            }
        }

        [Fact]
        public void TestEXEC_LD_rr_rr()
        {
            for (byte i = 0; i < 26; i++)
            {
                for (byte j = 0; j < 26; j++)
                {
                    Assembler cp = new();
                    using Computer computer = new();

                    byte[] reg = new byte[26];

                    byte targetIndex = (byte)(j + 2);
                    if (targetIndex > 25)
                        targetIndex = (byte)(targetIndex - 26);

                    uint spr = 0;
                    uint spc = 0;

                    ushort expectedValue = (ushort)(i * 2520 + 1);

                    string code = $@"
                        LD {TUtils.Get16bitRegisterString(i)},{expectedValue}
                        LD {TUtils.Get16bitRegisterString(targetIndex)},{TUtils.Get16bitRegisterString(i)}
                        BREAK";

                    cp.Build(code);

                    byte[] compiled = cp.GetCompiledCode();
                    int ipo = compiled.Length;

                    computer.LoadMem(compiled);
                    computer.Run();

                    byte[] ExpectedValueBytes = DataConverter.GetBytesFrom16bit(expectedValue);

                    reg[i] = ExpectedValueBytes[0];

                    reg[(i + 1) % 26] = ExpectedValueBytes[1];
                    reg[targetIndex] = ExpectedValueBytes[0];
                    reg[(targetIndex + 1) % 26] = ExpectedValueBytes[1];

                    string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                        ipo, spr, spc,
                        reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                        reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                        reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                    string actual = computer.CPU.REGS.GetDebugInfo();

                    Assert.Equal(expected, actual);
                    TUtils.IncrementCountedTests("exec");
                }

            }
        }

        [Fact]
        public void TestEXEC_LD_rr_InnnI()
        {
            for (byte i = 0; i < 26; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });

                string code = $@"
                    LD {TUtils.Get16bitRegisterString(i)},(10000)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[i] = 40;
                reg[(i + 1) % 26] = 50;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rr_Innn_nnnI_positive()
        {
            for (byte i = 0; i < 26; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });

                string code = $@"
                    LD {TUtils.Get16bitRegisterString(i)},(10000 + 4)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[i] = 80;
                reg[(i + 1) % 26] = 90;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rr_Innn_nnnI_negative()
        {
            for (byte i = 0; i < 26; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });

                string code = $@"
                    LD {TUtils.Get16bitRegisterString(i)},(10000 - 3)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[i] = 10;
                reg[(i + 1) % 26] = 20;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rr_Innn_rI_positive()
        {
            for (byte i = 0; i < 24; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });
                computer.CPU.REGS.Z = 4;

                string code = $@"
                    LD {TUtils.Get16bitRegisterString(i)},(10000 + Z)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[i] = 80;
                reg[(i + 1) % 26] = 90;
                reg[25] = 4;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rr_Innn_rI_negative()
        {
            for (byte i = 0; i < 24; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });
                computer.CPU.REGS.Z = 253;  // -3

                string code = $@"
                    LD {TUtils.Get16bitRegisterString(i)},(10000+Z)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[i] = 10;
                reg[(i + 1) % 26] = 20;
                reg[25] = 253;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rr_Innn_rrI_positive()
        {
            for (byte i = 0; i < 23; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });
                computer.CPU.REGS.YZ = 4;

                string code = $@"
                    LD {TUtils.Get16bitRegisterString(i)},(10000 + YZ)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[i] = 80;
                reg[(i + 1) % 26] = 90;
                reg[24] = 0;
                reg[25] = 4;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rr_Innn_rrI_negative()
        {
            for (byte i = 0; i < 23; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });
                computer.CPU.REGS.YZ = 0xFFFD;  // -3

                string code = $@"
                    LD {TUtils.Get16bitRegisterString(i)},(10000 + YZ)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[i] = 10;
                reg[(i + 1) % 26] = 20;
                reg[24] = 0xFF;
                reg[25] = 0xFD;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rr_Innn_rrrI_positive()
        {
            for (byte i = 0; i < 22; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });
                computer.CPU.REGS.XYZ = 4;

                string code = $@"
                    LD {TUtils.Get16bitRegisterString(i)},(10000 + XYZ)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[i] = 80;
                reg[(i + 1) % 26] = 90;
                reg[23] = 0;
                reg[24] = 0;
                reg[25] = 4;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rr_Innn_rrrI_negative()
        {
            for (byte i = 0; i < 22; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });
                computer.CPU.REGS.XYZ = 0xFFFFFD;  // -3

                string code = $@"
                    LD {TUtils.Get16bitRegisterString(i)},(10000 + XYZ)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[i] = 10;
                reg[(i + 1) % 26] = 20;
                reg[23] = 0xFF;
                reg[24] = 0xFF;
                reg[25] = 0xFD;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }


        [Fact]
        public void TestEXEC_LD_rr_IrrrI()
        {
            for (byte i = 3; i < 26; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("LD ABC,10001"),
                        string.Format("LD {0},(ABC)", TUtils.Get16bitRegisterString(i)),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[0] = 0;  // The ABC register
                reg[1] = 39;
                reg[2] = 17;

                reg[i] = 50;
                reg[i < 25 ? i + 1 : 0] = 60;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rr_Irrr_nnnI_positive()
        {
            for (byte i = 3; i < 25; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });

                string code = $@"
                    LD ABC,10000
                    LD {TUtils.Get16bitRegisterString(i)},(ABC + 4)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[1] = 39; // The ABC register
                reg[2] = 16;

                reg[i] = 80;
                reg[(i + 1) % 26] = 90;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rr_Irrr_nnnI_negative()
        {
            for (byte i = 3; i < 25; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });

                string code = $@"
                    LD ABC,10000
                    LD {TUtils.Get16bitRegisterString(i)},(ABC - 3)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[1] = 39; // The ABC register
                reg[2] = 16;

                reg[i] = 10;
                reg[(i + 1) % 26] = 20;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rr_Irrr_rI_positive()
        {
            for (byte i = 4; i < 25; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });

                string code = $@"
                    LD ABC,10000
                    LD D, 4
                    LD {TUtils.Get16bitRegisterString(i)},(ABC + D)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[1] = 39;    // ABC register
                reg[2] = 16;
                reg[3] = 4;     // D register

                reg[i] = 80;
                reg[(i + 1) % 26] = 90;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rr_Irrr_rI_negative()
        {
            for (byte i = 4; i < 25; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });

                string code = $@"
                    LD ABC,10000
                    LD D, -3
                    LD {TUtils.Get16bitRegisterString(i)},(ABC - D)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[1] = 39; // The ABC register
                reg[2] = 16;
                reg[3] = 253; // D register: -3

                reg[i] = 10;
                reg[(i + 1) % 26] = 20;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rr_Irrr_rrI_positive()
        {
            for (byte i = 5; i < 25; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });

                string code = $@"
                    LD ABC,10000
                    LD DE, 4
                    LD {TUtils.Get16bitRegisterString(i)},(ABC + DE)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[1] = 39;    // ABC register
                reg[2] = 16;
                reg[3] = 0;     // DE register
                reg[4] = 4;

                reg[i] = 80;
                reg[(i + 1) % 26] = 90;
                
                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rr_Irrr_rrI_negative()
        {
            for (byte i = 4; i < 25; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });

                string code = $@"
                    LD ABC,10000
                    LD DE, -3
                    LD {TUtils.Get16bitRegisterString(i)},(ABC - DE)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[1] = 39;    // ABC register
                reg[2] = 16;
                reg[3] = 0xFF;   // DE register: -3
                reg[4] = 0xFD;

                reg[i] = 10;
                reg[(i + 1) % 26] = 20;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rr_Irrr_rrrI_positive()
        {
            for (byte i = 6; i < 25; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });

                string code = $@"
                    LD ABC,10000
                    LD DEF, 4
                    LD {TUtils.Get16bitRegisterString(i)},(ABC + DEF)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[1] = 39;    // ABC register
                reg[2] = 16;
                reg[3] = 0;     // DEF register
                reg[4] = 0;
                reg[5] = 4;

                reg[i] = 80;
                reg[(i + 1) % 26] = 90;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rr_Irrr_rrrI_negative()
        {
            for (byte i = 5; i < 26; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });

                string code = $@"
                    LD ABC,10000
                    LD DEF, -3
                    LD {TUtils.Get16bitRegisterString(i)},(ABC - DEF)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[1] = 39;    // ABC register
                reg[2] = 16;
                reg[3] = 0xFF;   // DEF register: -3
                reg[4] = 0xFF;
                reg[5] = 0xFD;

                reg[i] = 10;
                reg[(i + 1) % 26] = 20;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        // LD rrr, [n, r, (nnn), (rrr)]
        [Fact]
        public void TestEXEC_LD_rrr_nnn()
        {
            for (byte i = 0; i < 26; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                uint expectedValue = (uint)(i * 645_277 + 1);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("LD {0},{1}", TUtils.Get24bitRegisterString(i), expectedValue),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                byte[] ExpectedValueBytes = DataConverter.GetBytesFrom24bit(expectedValue);

                reg[i] = ExpectedValueBytes[0];
                // Last registers to cross over are YZA and ZAB
                reg[(i + 1) % 26] = ExpectedValueBytes[1];
                reg[(i + 2) % 26] = ExpectedValueBytes[2];


                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }
        
        [Fact]
        public void TestEXEC_LD_rrr_r()
        {
            for (byte i = 0; i < 26; i++)
            {
                for (byte j = 0; j < 26; j++)
                {
                    Assembler cp = new();
                    using Computer computer = new();

                    byte[] reg = new byte[26];

                    byte targetIndex = (byte)(j + 2);
                    if (targetIndex > 25)
                        targetIndex = (byte)(targetIndex - 26);

                    uint spr = 0;
                    uint spc = 0;

                    byte expectedValue = (byte)(i + 1);

                    string code = $@"
                        LD {TUtils.Get8bitRegisterChar(i)},{expectedValue}
                        LD {TUtils.Get24bitRegisterString(targetIndex)},{TUtils.Get8bitRegisterChar(i)}
                        BREAK
                    ";

                    cp.Build(code);

                    byte[] compiled = cp.GetCompiledCode();
                    int ipo = compiled.Length;

                    computer.LoadMem(compiled);
                    computer.Run();

                    reg[i] = expectedValue;

                    reg[targetIndex] = 0;
                    reg[(targetIndex + 1) % 26] = 0;
                    reg[(targetIndex + 2) % 26] = expectedValue;

                    string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                        ipo, spr, spc,
                        reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                        reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                        reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                    string actual = computer.CPU.REGS.GetDebugInfo();

                    Assert.Equal(expected, actual);
                    TUtils.IncrementCountedTests("exec");
                }

            }
        }

        [Fact]
        public void TestEXEC_LD_rrr_rr()
        {
            for (byte i = 0; i < 26; i++)
            {
                for (byte j = 0; j < 26; j++)
                {
                    Assembler cp = new();
                    using Computer computer = new();

                    byte[] reg = new byte[26];

                    byte targetIndex = (byte)(j + 2);
                    if (targetIndex > 25)
                        targetIndex = (byte)(targetIndex - 26);

                    uint spr = 0;
                    uint spc = 0;

                    ushort expectedValue = (ushort)(i * 2520 + 1);

                    string code = $@"
                        LD {TUtils.Get16bitRegisterString(i)},{expectedValue}
                        LD {TUtils.Get24bitRegisterString(targetIndex)},{TUtils.Get16bitRegisterString(i)}
                        BREAK";

                    cp.Build(code);

                    byte[] compiled = cp.GetCompiledCode();
                    int ipo = compiled.Length;

                    computer.LoadMem(compiled);
                    computer.Run();

                    byte[] ExpectedValueBytes = DataConverter.GetBytesFrom16bit(expectedValue);

                    reg[i] = ExpectedValueBytes[0];
                    reg[(i + 1) % 26] = ExpectedValueBytes[1];

                    reg[targetIndex] = 0;
                    reg[(targetIndex + 1) % 26] = ExpectedValueBytes[0];
                    reg[(targetIndex + 2) % 26] = ExpectedValueBytes[1];

                    string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                        ipo, spr, spc,
                        reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                        reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                        reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                    string actual = computer.CPU.REGS.GetDebugInfo();

                    Assert.Equal(expected, actual);
                    TUtils.IncrementCountedTests("exec");
                }

            }
        }

        [Fact]
        public void TestEXEC_LD_rrr_rrr()
        {
            for (byte i = 3; i < 26; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                byte targetIndex = 0;

                uint spr = 0;
                uint spc = 0;

                uint expectedValue = (uint)(i * 645_277 + 1);

                string code = $@"
                    LD {TUtils.Get24bitRegisterString(i)},{expectedValue}
                    LD {TUtils.Get24bitRegisterString(targetIndex)},{TUtils.Get24bitRegisterString(i)}
                    BREAK";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                byte[] ExpectedValueBytes = DataConverter.GetBytesFrom24bit(expectedValue);

                // The order is important. The code can override some registers and it is important
                // to do it in the same order that the code does.
                reg[i] = ExpectedValueBytes[0];
                reg[(i + 1) % 26] = ExpectedValueBytes[1];
                reg[(i + 2) % 26] = ExpectedValueBytes[2];

                reg[targetIndex] = ExpectedValueBytes[0];
                reg[(targetIndex + 1) % 26] = ExpectedValueBytes[1];
                reg[(targetIndex + 2) % 26] = ExpectedValueBytes[2];

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rrr_InnnI()
        {
            for (byte i = 0; i < 26; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });

                string code = $@"
                    LD {TUtils.Get24bitRegisterString(i)},(10000)
                    BREAK";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[i] = 40;
                reg[(i + 1) % 26] = 50;
                reg[(i + 2) % 26] = 60;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rrr_Innn_nnnI_positive()
        {
            for (byte i = 0; i < 25; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 });

                string code = $@"
                    LD {TUtils.Get24bitRegisterString(i)},(10000 + 4)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[i] = 80;
                reg[(i + 1) % 26] = 90;
                reg[(i + 2) % 26] = 100;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rrr_Innn_nnnI_negative()
        {
            for (byte i = 0; i < 25; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });

                string code = $@"
                    LD {TUtils.Get24bitRegisterString(i)},(10000 - 3)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[i] = 10;
                reg[(i + 1) % 26] = 20;
                reg[(i + 2) % 26] = 30;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }


        [Fact]
        public void TestEXEC_LD_rrr_Innn_rI_positive()
        {
            for (byte i = 0; i < 23; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 });
                computer.CPU.REGS.Z = 4;

                string code = $@"
                    LD {TUtils.Get24bitRegisterString(i)},(10000 + Z)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[i] = 80;
                reg[(i + 1) % 26] = 90;
                reg[(i + 2) % 26] = 100;
                reg[25] = 4;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rrr_Innn_rI_negative()
        {
            for (byte i = 0; i < 23; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });
                computer.CPU.REGS.Z = 253;  // -3

                string code = $@"
                    LD {TUtils.Get24bitRegisterString(i)},(10000 + Z)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[i] = 10;
                reg[(i + 1) % 26] = 20;
                reg[(i + 2) % 26] = 30;
                reg[25] = 253;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rrr_Innn_rrI_positive()
        {
            for (byte i = 0; i < 22; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 });
                computer.CPU.REGS.YZ = 4;

                string code = $@"
                    LD {TUtils.Get24bitRegisterString(i)},(10000 + YZ)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[i] = 80;
                reg[(i + 1) % 26] = 90;
                reg[(i + 2) % 26] = 100;
                reg[24] = 0;
                reg[25] = 4;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rrr_Innn_rrI_negative()
        {
            for (byte i = 0; i < 22; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });
                computer.CPU.REGS.YZ = 0xFFFD;  // -3

                string code = $@"
                    LD {TUtils.Get24bitRegisterString(i)},(10000 + YZ)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[i] = 10;
                reg[(i + 1) % 26] = 20;
                reg[(i + 2) % 26] = 30;

                reg[24] = 0xFF;
                reg[25] = 0xFD;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rrr_Innn_rrrI_positive()
        {
            for (byte i = 0; i < 21; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 });
                computer.CPU.REGS.XYZ = 4;

                string code = $@"
                    LD {TUtils.Get24bitRegisterString(i)},(10000 + XYZ)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[i] = 80;
                reg[(i + 1) % 26] = 90;
                reg[(i + 2) % 26] = 100;

                reg[23] = 0;
                reg[24] = 0;
                reg[25] = 4;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rrr_Innn_rrrI_negative()
        {
            for (byte i = 0; i < 21; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });
                computer.CPU.REGS.XYZ = 0xFFFFFD;  // -3

                string code = $@"
                    LD {TUtils.Get24bitRegisterString(i)},(10000 + XYZ)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[i] = 10;
                reg[(i + 1) % 26] = 20;
                reg[(i + 2) % 26] = 30;

                reg[23] = 0xFF;
                reg[24] = 0xFF;
                reg[25] = 0xFD;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rrr_IrrrI()
        {
            for (byte i = 3; i < 26; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("LD ABC,10001"),
                        string.Format("LD {0},(ABC)", TUtils.Get24bitRegisterString(i)),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[0] = 0;  // The ABC register
                reg[1] = 39;
                reg[2] = 17;

                reg[i] = 50;
                reg[i < 25 ? i + 1 : i - 25] = 60;
                reg[i < 24 ? i + 2 : i - 24] = 70;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        
        [Fact]
        public void TestEXEC_LD_rrr_Irrr_nnnI_positive()
        {
            for (byte i = 3; i < 24; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 });

                string code = $@"
                    LD ABC,10000
                    LD {TUtils.Get24bitRegisterString(i)},(ABC + 4)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[1] = 39; // The ABC register
                reg[2] = 16;

                reg[i] = 80;
                reg[(i + 1) % 26] = 90;
                reg[(i + 2) % 26] = 100;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rrr_Irrr_nnnI_negative()
        {
            for (byte i = 3; i < 24; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });

                string code = $@"
                    LD ABC,10000
                    LD {TUtils.Get24bitRegisterString(i)},(ABC - 3)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[1] = 39; // The ABC register
                reg[2] = 16;

                reg[i] = 10;
                reg[(i + 1) % 26] = 20;
                reg[(i + 2) % 26] = 30;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rrr_Irrr_rI_positive()
        {
            for (byte i = 4; i < 24; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 });

                string code = $@"
                    LD ABC,10000
                    LD D, 4
                    LD {TUtils.Get24bitRegisterString(i)},(ABC + D)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[1] = 39;    // ABC register
                reg[2] = 16;
                reg[3] = 4;     // D register

                reg[i] = 80;
                reg[(i + 1) % 26] = 90;
                reg[(i + 2) % 26] = 100;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rrr_Irrr_rI_negative()
        {
            for (byte i = 4; i < 24; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });

                string code = $@"
                    LD ABC,10000
                    LD D, -3
                    LD {TUtils.Get24bitRegisterString(i)},(ABC - D)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[1] = 39; // The ABC register
                reg[2] = 16;
                reg[3] = 253; // D register: -3

                reg[i] = 10;
                reg[(i + 1) % 26] = 20;
                reg[(i + 2) % 26] = 30;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rrr_Irrr_rrI_positive()
        {
            for (byte i = 5; i < 24; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 });

                string code = $@"
                    LD ABC,10000
                    LD DE, 4
                    LD {TUtils.Get24bitRegisterString(i)},(ABC + DE)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[1] = 39;    // ABC register
                reg[2] = 16;
                reg[3] = 0;     // DE register
                reg[4] = 4;

                reg[i] = 80;
                reg[(i + 1) % 26] = 90;
                reg[(i + 2) % 26] = 100;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rrr_Irrr_rrI_negative()
        {
            for (byte i = 4; i < 24; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });

                string code = $@"
                    LD ABC,10000
                    LD DE, -3
                    LD {TUtils.Get24bitRegisterString(i)},(ABC - DE)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[1] = 39;    // ABC register
                reg[2] = 16;
                reg[3] = 0xFF;   // DE register: -3
                reg[4] = 0xFD;

                reg[i] = 10;
                reg[(i + 1) % 26] = 20;
                reg[(i + 2) % 26] = 30;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rrr_Irrr_rrrI_positive()
        {
            for (byte i = 6; i < 24; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 });

                string code = $@"
                    LD ABC,10000
                    LD DEF, 4
                    LD {TUtils.Get24bitRegisterString(i)},(ABC + DEF)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[1] = 39;    // ABC register
                reg[2] = 16;
                reg[3] = 0;     // DEF register
                reg[4] = 0;
                reg[5] = 4;

                reg[i] = 80;
                reg[(i + 1) % 26] = 90;
                reg[(i + 2) % 26] = 100;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rrr_Irrr_rrrI_negative()
        {
            for (byte i = 5; i < 24; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });

                string code = $@"
                    LD ABC,10000
                    LD DEF, -3
                    LD {TUtils.Get24bitRegisterString(i)},(ABC - DEF)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[1] = 39;    // ABC register
                reg[2] = 16;
                reg[3] = 0xFF;   // DEF register: -3
                reg[4] = 0xFF;
                reg[5] = 0xFD;

                reg[i] = 10;
                reg[(i + 1) % 26] = 20;
                reg[(i + 2) % 26] = 30;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        // LD rrrr, [n, r, (nnn), (rrr)]
        [Fact]
        public void TestEXEC_LD_rrrr_nnnn()
        {
            for (byte i = 0; i < 26; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                uint expectedValue = (uint)(i * 165_191_049 + 1);

                string code = $@"
                    LD {TUtils.Get32bitRegisterString(i)}, {expectedValue}
                    BREAK";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                byte[] ExpectedValueBytes = DataConverter.GetBytesFrom32bit(expectedValue);

                reg[i] = ExpectedValueBytes[0];
                // Last registers to cross over are YZA and ZAB
                reg[i < 25 ? i + 1 : i - 25] = ExpectedValueBytes[1];
                reg[i < 24 ? i + 2 : i - 24] = ExpectedValueBytes[2];
                reg[i < 23 ? i + 3 : i - 23] = ExpectedValueBytes[3];


                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rrrr_r()
        {
            for (byte i = 0; i < 26; i++)
            {
                for (byte j = 0; j < 26; j++)
                {
                    Assembler cp = new();
                    using Computer computer = new();

                    byte[] reg = new byte[26];

                    byte targetIndex = (byte)(j + 2);
                    if (targetIndex > 25)
                        targetIndex = (byte)(targetIndex - 26);

                    uint spr = 0;
                    uint spc = 0;

                    byte expectedValue = (byte)(i + 1);

                    string code = $@"
                        LD {TUtils.Get8bitRegisterChar(i)},{expectedValue}
                        LD {TUtils.Get32bitRegisterString(targetIndex)},{TUtils.Get8bitRegisterChar(i)}
                        BREAK
                    ";

                    cp.Build(code);

                    byte[] compiled = cp.GetCompiledCode();
                    int ipo = compiled.Length;

                    computer.LoadMem(compiled);
                    computer.Run();

                    reg[i] = expectedValue;

                    reg[targetIndex] = 0;
                    reg[(targetIndex + 1) % 26] = 0;
                    reg[(targetIndex + 2) % 26] = 0;
                    reg[(targetIndex + 3) % 26] = expectedValue;

                    string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                        ipo, spr, spc,
                        reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                        reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                        reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                    string actual = computer.CPU.REGS.GetDebugInfo();

                    Assert.Equal(expected, actual);
                    TUtils.IncrementCountedTests("exec");
                }

            }
        }

        [Fact]
        public void TestEXEC_LD_rrrr_rr()
        {
            for (byte i = 0; i < 26; i++)
            {
                for (byte j = 0; j < 26; j++)
                {
                    Assembler cp = new();
                    using Computer computer = new();

                    byte[] reg = new byte[26];

                    byte targetIndex = (byte)(j + 2);
                    if (targetIndex > 25)
                        targetIndex = (byte)(targetIndex - 26);

                    uint spr = 0;
                    uint spc = 0;

                    ushort expectedValue = (ushort)(i * 2520 + 1);

                    string code = $@"
                        LD {TUtils.Get16bitRegisterString(i)},{expectedValue}
                        LD {TUtils.Get32bitRegisterString(targetIndex)},{TUtils.Get16bitRegisterString(i)}
                        BREAK";

                    cp.Build(code);

                    byte[] compiled = cp.GetCompiledCode();
                    int ipo = compiled.Length;

                    computer.LoadMem(compiled);
                    computer.Run();

                    byte[] ExpectedValueBytes = DataConverter.GetBytesFrom16bit(expectedValue);

                    reg[i] = ExpectedValueBytes[0];
                    reg[(i + 1) % 26] = ExpectedValueBytes[1];

                    reg[targetIndex] = 0;
                    reg[(targetIndex + 1) % 26] = 0;
                    reg[(targetIndex + 2) % 26] = ExpectedValueBytes[0];
                    reg[(targetIndex + 3) % 26] = ExpectedValueBytes[1];

                    string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                        ipo, spr, spc,
                        reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                        reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                        reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                    string actual = computer.CPU.REGS.GetDebugInfo();

                    Assert.Equal(expected, actual);
                    TUtils.IncrementCountedTests("exec");
                }

            }
        }

        [Fact]
        public void TestEXEC_LD_rrrr_rrr()
        {
            for (byte i = 3; i < 26; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                byte targetIndex = 0;

                uint spr = 0;
                uint spc = 0;

                uint expectedValue = (uint)(i * 645_277 + 1);

                string code = $@"
                    LD {TUtils.Get24bitRegisterString(i)},{expectedValue}
                    LD {TUtils.Get32bitRegisterString(targetIndex)},{TUtils.Get24bitRegisterString(i)}
                    BREAK";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                byte[] ExpectedValueBytes = DataConverter.GetBytesFrom24bit(expectedValue);

                // The order is important. The code can override some registers and it is important
                // to do it in the same order that the code does.
                reg[i] = ExpectedValueBytes[0];
                reg[(i + 1) % 26] = ExpectedValueBytes[1];
                reg[(i + 2) % 26] = ExpectedValueBytes[2];

                reg[targetIndex] = 0;
                reg[(targetIndex + 1) % 26] = ExpectedValueBytes[0];
                reg[(targetIndex + 2) % 26] = ExpectedValueBytes[1];
                reg[(targetIndex + 3) % 26] = ExpectedValueBytes[2];

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rrrr_rrrr()
        {
            for (byte i = 4; i < 26; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                byte targetIndex = 0;

                uint spr = 0;
                uint spc = 0;

                uint expectedValue = (uint)(i * 165_191_049 + 1);

                string code = $@"
                    LD {TUtils.Get32bitRegisterString(i)},{expectedValue}
                    LD {TUtils.Get32bitRegisterString(targetIndex)},{TUtils.Get32bitRegisterString(i)}
                    BREAK";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                byte[] ExpectedValueBytes = DataConverter.GetBytesFrom32bit(expectedValue);

                // The order is important. The code can override some registers and it is important
                // to do it in the same order that the code does.
                reg[i] = ExpectedValueBytes[0];
                reg[(i + 1) % 26] = ExpectedValueBytes[1];
                reg[(i + 2) % 26] = ExpectedValueBytes[2];
                reg[(i + 3) % 26] = ExpectedValueBytes[3];

                reg[targetIndex] = ExpectedValueBytes[0];
                reg[(targetIndex + 1) % 26] = ExpectedValueBytes[1];
                reg[(targetIndex + 2) % 26] = ExpectedValueBytes[2];
                reg[(targetIndex + 3) % 26] = ExpectedValueBytes[3];

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rrrr_InnnI()
        {
            for (byte i = 0; i < 26; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });

                string code = $@"
                    LD {TUtils.Get32bitRegisterString(i)}, (10000)
                    BREAK";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[i] = 40;
                reg[(i + 1) % 26] = 50;
                reg[(i + 2) % 26] = 60;
                reg[(i + 3) % 26] = 70;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rrrr_Innn_nnnI_positive()
        {
            for (byte i = 0; i < 24; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110 });

                string code = $@"
                    LD {TUtils.Get32bitRegisterString(i)},(10000 + 4)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[i] = 80;
                reg[(i + 1) % 26] = 90;
                reg[(i + 2) % 26] = 100;
                reg[(i + 3) % 26] = 110;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rrrr_Innn_nnnI_negative()
        {
            for (byte i = 0; i < 25; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });

                string code = $@"
                    LD {TUtils.Get32bitRegisterString(i)},(10000 - 3)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[i] = 10;
                reg[(i + 1) % 26] = 20;
                reg[(i + 2) % 26] = 30;
                reg[(i + 3) % 26] = 40;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rrrr_Innn_rI_positive()
        {
            for (byte i = 0; i < 22; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110 });
                computer.CPU.REGS.Z = 4;

                string code = $@"
                    LD {TUtils.Get32bitRegisterString(i)},(10000 + Z)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[i] = 80;
                reg[(i + 1) % 26] = 90;
                reg[(i + 2) % 26] = 100;
                reg[(i + 3) % 26] = 110;
                reg[25] = 4;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rrrr_Innn_rI_negative()
        {
            for (byte i = 0; i < 22; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });
                computer.CPU.REGS.Z = 253;  // -3

                string code = $@"
                    LD {TUtils.Get32bitRegisterString(i)},(10000 + Z)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[i] = 10;
                reg[(i + 1) % 26] = 20;
                reg[(i + 2) % 26] = 30;
                reg[(i + 3) % 26] = 40;
                reg[25] = 253;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rrrr_Innn_rrI_positive()
        {
            for (byte i = 0; i < 21; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110 });
                computer.CPU.REGS.YZ = 4;

                string code = $@"
                    LD {TUtils.Get32bitRegisterString(i)},(10000 + YZ)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[i] = 80;
                reg[(i + 1) % 26] = 90;
                reg[(i + 2) % 26] = 100;
                reg[(i + 3) % 26] = 110;

                reg[24] = 0;
                reg[25] = 4;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rrrr_Innn_rrI_negative()
        {
            for (byte i = 0; i < 21; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });
                computer.CPU.REGS.YZ = 0xFFFD;  // -3

                string code = $@"
                    LD {TUtils.Get32bitRegisterString(i)},(10000 + YZ)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[i] = 10;
                reg[(i + 1) % 26] = 20;
                reg[(i + 2) % 26] = 30;
                reg[(i + 3) % 26] = 40;

                reg[24] = 0xFF;
                reg[25] = 0xFD;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rrrr_Innn_rrrI_positive()
        {
            for (byte i = 0; i < 20; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110 });
                computer.CPU.REGS.XYZ = 4;

                string code = $@"
                    LD {TUtils.Get32bitRegisterString(i)},(10000 + XYZ)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[i] = 80;
                reg[(i + 1) % 26] = 90;
                reg[(i + 2) % 26] = 100;
                reg[(i + 3) % 26] = 110;

                reg[23] = 0;
                reg[24] = 0;
                reg[25] = 4;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rrrr_Innn_rrrI_negative()
        {
            for (byte i = 0; i < 20; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });
                computer.CPU.REGS.XYZ = 0xFFFFFD;  // -3

                string code = $@"
                    LD {TUtils.Get32bitRegisterString(i)},(10000 + XYZ)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[i] = 10;
                reg[(i + 1) % 26] = 20;
                reg[(i + 2) % 26] = 30;
                reg[(i + 3) % 26] = 40;

                reg[23] = 0xFF;
                reg[24] = 0xFF;
                reg[25] = 0xFD;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rrrr_IrrrI()
        {
            for (byte i = 3; i < 26; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });

                cp.Build(
                    TUtils.GetFormattedAsm(
                        string.Format("LD ABC,10001"),
                        string.Format("LD {0},(ABC)", TUtils.Get32bitRegisterString(i)),
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[0] = 0;  // The ABC register
                reg[1] = 39;
                reg[2] = 17;

                reg[i] = 50;
                reg[i < 25 ? i + 1 : i - 25] = 60;
                reg[i < 24 ? i + 2 : i - 24] = 70;
                reg[i < 23 ? i + 3 : i - 23] = 80;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rrrr_Irrr_nnnI_positive()
        {
            for (byte i = 3; i < 23; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110 });

                string code = $@"
                    LD ABC,10000
                    LD {TUtils.Get32bitRegisterString(i)},(ABC + 4)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[1] = 39; // The ABC register
                reg[2] = 16;

                reg[i] = 80;
                reg[(i + 1) % 26] = 90;
                reg[(i + 2) % 26] = 100;
                reg[(i + 3) % 26] = 110;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rrrr_Irrr_nnnI_negative()
        {
            for (byte i = 3; i < 23; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });

                string code = $@"
                    LD ABC,10000
                    LD {TUtils.Get32bitRegisterString(i)},(ABC - 3)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[1] = 39; // The ABC register
                reg[2] = 16;

                reg[i] = 10;
                reg[(i + 1) % 26] = 20;
                reg[(i + 2) % 26] = 30;
                reg[(i + 3) % 26] = 40;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rrrr_Irrr_rI_positive()
        {
            for (byte i = 4; i < 23; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110 });

                string code = $@"
                    LD ABC,10000
                    LD D, 4
                    LD {TUtils.Get32bitRegisterString(i)},(ABC + D)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[1] = 39;    // ABC register
                reg[2] = 16;
                reg[3] = 4;     // D register

                reg[i] = 80;
                reg[(i + 1) % 26] = 90;
                reg[(i + 2) % 26] = 100;
                reg[(i + 3) % 26] = 110;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rrrr_Irrr_rI_negative()
        {
            for (byte i = 4; i < 23; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });

                string code = $@"
                    LD ABC,10000
                    LD D, -3
                    LD {TUtils.Get32bitRegisterString(i)},(ABC - D)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[1] = 39; // The ABC register
                reg[2] = 16;
                reg[3] = 253; // D register: -3

                reg[i] = 10;
                reg[(i + 1) % 26] = 20;
                reg[(i + 2) % 26] = 30;
                reg[(i + 3) % 26] = 40;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rrrr_Irrr_rrI_positive()
        {
            for (byte i = 5; i < 23; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110 });

                string code = $@"
                    LD ABC,10000
                    LD DE, 4
                    LD {TUtils.Get32bitRegisterString(i)},(ABC + DE)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[1] = 39;    // ABC register
                reg[2] = 16;
                reg[3] = 0;     // DE register
                reg[4] = 4;

                reg[i] = 80;
                reg[(i + 1) % 26] = 90;
                reg[(i + 2) % 26] = 100;
                reg[(i + 3) % 26] = 110;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rrrr_Irrr_rrI_negative()
        {
            for (byte i = 4; i < 23; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });

                string code = $@"
                    LD ABC,10000
                    LD DE, -3
                    LD {TUtils.Get32bitRegisterString(i)},(ABC - DE)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[1] = 39;    // ABC register
                reg[2] = 16;
                reg[3] = 0xFF;   // DE register: -3
                reg[4] = 0xFD;

                reg[i] = 10;
                reg[(i + 1) % 26] = 20;
                reg[(i + 2) % 26] = 30;
                reg[(i + 3) % 26] = 40;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rrrr_Irrr_rrrI_positive()
        {
            for (byte i = 6; i < 24; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110 });

                string code = $@"
                    LD ABC,10000
                    LD DEF, 4
                    LD {TUtils.Get32bitRegisterString(i)},(ABC + DEF)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[1] = 39;    // ABC register
                reg[2] = 16;
                reg[3] = 0;     // DEF register
                reg[4] = 0;
                reg[5] = 4;

                reg[i] = 80;
                reg[(i + 1) % 26] = 90;
                reg[(i + 2) % 26] = 100;
                reg[(i + 3) % 26] = 110;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LD_rrrr_Irrr_rrrI_negative()
        {
            for (byte i = 5; i < 23; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte[] reg = new byte[26];

                uint spr = 0;
                uint spc = 0;

                computer.LoadMemAt(9997, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });

                string code = $@"
                    LD ABC,10000
                    LD DEF, -3
                    LD {TUtils.Get32bitRegisterString(i)},(ABC - DEF)
                    BREAK
                ";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();
                int ipo = compiled.Length;

                computer.LoadMem(compiled);
                computer.Run();

                reg[1] = 39;    // ABC register
                reg[2] = 16;
                reg[3] = 0xFF;   // DEF register: -3
                reg[4] = 0xFF;
                reg[5] = 0xFD;

                reg[i] = 10;
                reg[(i + 1) % 26] = 20;
                reg[(i + 2) % 26] = 30;
                reg[(i + 3) % 26] = 40;

                string expected = string.Format(computer.CPU.REGS.GetDebugTemplate(),
                    ipo, spr, spc,
                    reg[0], reg[1], reg[2], reg[3], reg[4], reg[5], reg[6], reg[7], reg[8], reg[9],
                    reg[10], reg[11], reg[12], reg[13], reg[14], reg[15], reg[16], reg[17], reg[18],
                    reg[19], reg[20], reg[21], reg[22], reg[23], reg[24], reg[25]);

                string actual = computer.CPU.REGS.GetDebugInfo();

                Assert.Equal(expected, actual);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Theory]
        [InlineData((byte)1)]
        [InlineData((byte)2)]
        [InlineData((byte)3)]
        [InlineData((byte)4)]
        [InlineData((byte)8)]
        [InlineData((byte)255)]
        public void TestEXEC_LD_InnnI_nnnn_n_VariousCounts(byte count)
        {
            // — Arrange —
            Assembler cp = new();
            using var computer = new Computer();

            uint address = 0x1000;
            uint immediate = 0x11223344;  // bytes: 11 22 33 44

            // Build a tiny program: LD (address), immediate32, count ; BREAK
            string source = $@"
                LD ({address}), {immediate}, {count}
                BREAK";
            cp.Build(source);

            byte[] compiled = cp.GetCompiledCode();
            computer.LoadMem(compiled);

            // — Act —
            computer.Run();

            // — Assert —

            // 1) Pull out exactly 'count' bytes at 'address'
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address, count);

            // 2) Build the expected array:
            //    zero-pad on the left for count>4, then copy the low-order bytes of 'immediate'
            var expected = new byte[count];
            int toCopy = Math.Min(count, (byte)4);
            for (int i = 0; i < toCopy; i++)
            {
                // Big-endian slice of 0x11223344:
                //   for toCopy=4 → [0]=0x11, [1]=0x22, [2]=0x33, [3]=0x44
                //   for toCopy=2 → [0]=0x33, [1]=0x44
                expected[count - toCopy + i] =
                    (byte)(immediate >> ((toCopy - 1 - i) * 8));
            }

            Assert.Equal(expected, actual);

            // 3) Check the byte immediately before/after is still zero:
            if (address > 0)
            {
                Assert.Equal(0, computer.MEMC.RAM[address - 1]);
            }
                
            Assert.Equal(0, computer.MEMC.RAM[address + count]);
        }


        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_InnnI_nnnn_n_nnn(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address = 0x2000;
            uint immediate = 0x11223344;  // pattern bytes: 11 22 33 44

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ({address}), {immediate}, {count}, {repeat}
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(immediate >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address > 0)
                Assert.Equal(0, computer.MEMC.RAM[address - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address + (uint)total]);
        }

        [Fact]
        public void TestEXEC_LD_InnnI_r()
        {
            uint address = 10000;

            for (byte i = 0; i < 26; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte expectedValue = (byte)(i * 9);

                string code = $@"
                    LD {TUtils.Get8bitRegisterChar(i)}, {expectedValue}
                    LD ({address}), {TUtils.Get8bitRegisterChar(i)}
                    BREAK";

                cp.Build(code);

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
        public void TestEXEC_LD_InnnI_rr()
        {
            uint address = 10000;

            for (byte i = 0; i < 26; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                ushort expectedValue = (ushort)(i * 2520);

                string code = $@"
                    LD {TUtils.Get16bitRegisterString(i)}, {expectedValue}
                    LD ({address}), {TUtils.Get16bitRegisterString(i)}
                    BREAK";

                cp.Build(code);

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
        public void TestEXEC_LD_InnnI_rrr()
        {
            uint address = 10000;

            for (byte i = 0; i < 26; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                uint expectedValue = (uint)(i * 645_277);

                string code = $@"
                    LD {TUtils.Get24bitRegisterString(i)}, {expectedValue}
                    LD ({address}), {TUtils.Get24bitRegisterString(i)}
                    BREAK";

                cp.Build(code);

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
        public void TestEXEC_LD_InnnI_rrrr()
        {
            uint address = 10000;

            for (byte i = 0; i < 26; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                uint expectedValue = (uint)(i * 165_191_049);

                string code = $@"
                    LD {TUtils.Get32bitRegisterString(i)}, {expectedValue}
                    LD ({address}), {TUtils.Get32bitRegisterString(i)}
                    BREAK";

                cp.Build(code);

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

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_InnnI_InnnI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4000, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD ({address1}), ({address2}), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_InnnI_Innn_nnnI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD ({address1}), ({address2} + 4), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_InnnI_Innn_rI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD D, 4
                LD ({address1}), ({address2} + D), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_InnnI_Innn_rrI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DE, 4
                LD ({address1}), ({address2} + DE), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_InnnI_Innn_rrrI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, 4
                LD ({address1}), ({address2} + DEF), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_InnnI_IrrrI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4000, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address2}
                LD ({address1}), (DEF), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_InnnI_Irrr_nnnI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address2}
                LD ({address1}), (DEF + 4), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_InnnI_Irrr_rI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address2}
                LD G, 4
                LD ({address1}), (DEF + G), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_InnnI_Irrr_rrI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address2}
                LD GH, 4
                LD ({address1}), (DEF + GH), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_InnnI_Irrr_rrrI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address2}
                LD GHI, 4
                LD ({address1}), (DEF + GHI), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + (uint)total]);
        }

        [Theory]
        [InlineData((byte)1)]
        [InlineData((byte)2)]
        [InlineData((byte)3)]
        [InlineData((byte)4)]
        [InlineData((byte)8)]
        [InlineData((byte)255)]
        public void TestEXEC_LD_Innn_nnnI_nnnn_n_VariousCounts(byte count)
        {
            // — Arrange —
            Assembler cp = new();
            using var computer = new Computer();

            uint address = 0x1000;
            uint immediate = 0x11223344;  // bytes: 11 22 33 44

            // Build a tiny program: LD (address), immediate32, count ; BREAK
            string source = $@"
                LD ({address} + 4), {immediate}, {count}
                BREAK";
            cp.Build(source);

            byte[] compiled = cp.GetCompiledCode();
            computer.LoadMem(compiled);

            // — Act —
            computer.Run();

            // — Assert —

            // 1) Pull out exactly 'count' bytes at 'address'
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address + 4, count);

            // 2) Build the expected array:
            //    zero-pad on the left for count>4, then copy the low-order bytes of 'immediate'
            var expected = new byte[count];
            int toCopy = Math.Min(count, (byte)4);
            for (int i = 0; i < toCopy; i++)
            {
                // Big-endian slice of 0x11223344:
                //   for toCopy=4 → [0]=0x11, [1]=0x22, [2]=0x33, [3]=0x44
                //   for toCopy=2 → [0]=0x33, [1]=0x44
                expected[count - toCopy + i] =
                    (byte)(immediate >> ((toCopy - 1 - i) * 8));
            }

            Assert.Equal(expected, actual);

            // 3) Check the byte immediately before/after is still zero:
            if ((address + 4) > 0)
            {
                Assert.Equal(0, computer.MEMC.RAM[address - 1]);
            }

            Assert.Equal(0, computer.MEMC.RAM[(address + 4) + count]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Innn_nnnI_nnnn_n_nnn(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address = 0x2000;
            uint immediate = 0x11223344;  // pattern bytes: 11 22 33 44

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ({address} + 4), {immediate}, {count}, {repeat}
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address + 4, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(immediate >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address + 4 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address + 4 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address + 4 + (uint)total]);
        }

        [Fact]
        public void TestEXEC_LD_Innn_nnnI_r()
        {
            uint address = 10000;

            for (byte i = 0; i < 26; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte expectedValue = (byte)(i * 9);

                string code = $@"
                    LD {TUtils.Get8bitRegisterChar(i)}, {expectedValue}
                    LD ({address} + 4), {TUtils.Get8bitRegisterChar(i)}
                    BREAK";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint actualValueBelow = computer.MEMC.Get32bitFromRAM((address + 4) - 3);
                uint actualValueAbove = computer.MEMC.Get32bitFromRAM((address + 4) + 1);

                TUtils.IncrementCountedTests("exec");
                // We are making sure nothing "bleeds-out" onto other memory locations
                Assert.True((expectedValue == actualValueBelow) && (actualValueAbove == 0));
            }
        }

        [Fact]
        public void TestEXEC_LD_Innn_nnnI_rr()
        {
            uint address = 10000;

            for (byte i = 0; i < 26; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                ushort expectedValue = (ushort)(i * 2520);

                string code = $@"
                    LD {TUtils.Get16bitRegisterString(i)}, {expectedValue}
                    LD ({address} + 4), {TUtils.Get16bitRegisterString(i)}
                    BREAK";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint actualValueBelow = computer.MEMC.Get32bitFromRAM((address + 4) - 4);
                uint actualValue = computer.MEMC.Get16bitFromRAM(address + 4);
                uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 4 + 2);

                TUtils.IncrementCountedTests("exec");
                // We are making sure nothing "bleeds-out" onto other memory locations
                Assert.True((actualValueBelow == 0) && (actualValue == expectedValue) && (actualValueAbove == 0));
            }
        }

        [Fact]
        public void TestEXEC_LD_Innn_nnnI_rrr()
        {
            uint address = 10000;

            for (byte i = 0; i < 26; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                uint expectedValue = (uint)(i * 645_277);

                string code = $@"
                    LD {TUtils.Get24bitRegisterString(i)}, {expectedValue}
                    LD ({address} + 4), {TUtils.Get24bitRegisterString(i)}
                    BREAK";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint actualValueBelow = computer.MEMC.Get32bitFromRAM((address + 4) - 4);
                uint actualValue = computer.MEMC.Get24bitFromRAM(address + 4);
                uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 4 + 3);

                TUtils.IncrementCountedTests("exec");
                // We are making sure nothing "bleeds-out" onto other memory locations
                Assert.True((actualValueBelow == 0) && (actualValue == expectedValue) && (actualValueAbove == 0));
            }
        }

        [Fact]
        public void TestEXEC_LD_Innn_nnnI_rrrr()
        {
            uint address = 10000;

            for (byte i = 0; i < 26; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                uint expectedValue = (uint)(i * 165_191_049);

                string code = $@"
                    LD {TUtils.Get32bitRegisterString(i)}, {expectedValue}
                    LD ({address} + 4), {TUtils.Get32bitRegisterString(i)}
                    BREAK";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint actualValueBelow = computer.MEMC.Get32bitFromRAM((address + 4) - 4);
                uint actualValue = computer.MEMC.Get32bitFromRAM(address + 4);
                uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 4 + 4);

                TUtils.IncrementCountedTests("exec");
                // We are making sure nothing "bleeds-out" onto other memory locations
                Assert.True((actualValueBelow == 0) && (actualValue == expectedValue) && (actualValueAbove == 0));
            }
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Innn_nnnI_InnnI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4000, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD ({address1} + 4), ({address2}), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 4, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 4 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 4 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 4 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Innn_nnnI_Innn_nnnI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD ({address1} + 8), ({address2} + 4), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 8, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 8 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 8 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 8 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Innn_nnnI_Innn_rI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD D, 4
                LD ({address1} + 8), ({address2} + D), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 8, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 8 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 8 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 8 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Innn_nnnI_Innn_rrI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DE, 4
                LD ({address1} + 8), ({address2} + DE), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 8, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 8 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 8 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 8 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Innn_nnnI_Innn_rrrI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, 4
                LD ({address1} + 8), ({address2} + DEF), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 8, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 8 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 8 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 8 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Innn_nnnI_IrrrI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4000, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address2}
                LD ({address1} + 8), (DEF), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 8, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 8 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 8 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 8 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Innn_nnnI_Irrr_nnnI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address2}
                LD ({address1} + 8), (DEF + 4), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 8, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 8 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 8 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 8 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Innn_nnnI_Irrr_rI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address2}
                LD G, 4
                LD ({address1} + 8), (DEF + G), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 8, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 8 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 8 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 8 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Innn_nnnI_Irrr_rrI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address2}
                LD GH, 4
                LD ({address1} + 8), (DEF + GH), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 8, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 8 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 8 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 8 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Innn_nnnI_Irrr_rrrI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address2}
                LD GHI, 4
                LD ({address1} + 8), (DEF + GHI), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 8, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 8 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 8 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 8 + (uint)total]);
        }

        [Theory]
        [InlineData((byte)1)]
        [InlineData((byte)2)]
        [InlineData((byte)3)]
        [InlineData((byte)4)]
        [InlineData((byte)8)]
        [InlineData((byte)255)]
        public void TestEXEC_LD_Innn_rI_nnnn_n_VariousCounts(byte count)
        {
            // — Arrange —
            Assembler cp = new();
            using var computer = new Computer();

            uint address = 0x1000;
            uint immediate = 0x11223344;  // bytes: 11 22 33 44

            // Build a tiny program: LD (address), immediate32, count ; BREAK
            string source = $@"
                LD A, 4
                LD ({address} + A), {immediate}, {count}
                BREAK";
            cp.Build(source);

            byte[] compiled = cp.GetCompiledCode();
            computer.LoadMem(compiled);

            // — Act —
            computer.Run();

            // — Assert —

            // 1) Pull out exactly 'count' bytes at 'address'
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address + 4, count);

            // 2) Build the expected array:
            //    zero-pad on the left for count>4, then copy the low-order bytes of 'immediate'
            var expected = new byte[count];
            int toCopy = Math.Min(count, (byte)4);
            for (int i = 0; i < toCopy; i++)
            {
                // Big-endian slice of 0x11223344:
                //   for toCopy=4 → [0]=0x11, [1]=0x22, [2]=0x33, [3]=0x44
                //   for toCopy=2 → [0]=0x33, [1]=0x44
                expected[count - toCopy + i] =
                    (byte)(immediate >> ((toCopy - 1 - i) * 8));
            }

            Assert.Equal(expected, actual);

            // 3) Check the byte immediately before/after is still zero:
            if ((address + 4) > 0)
            {
                Assert.Equal(0, computer.MEMC.RAM[address - 1]);
            }

            Assert.Equal(0, computer.MEMC.RAM[(address + 4) + count]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Innn_rI_nnnn_n_nnn(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address = 0x2000;
            uint immediate = 0x11223344;  // pattern bytes: 11 22 33 44

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD A, 4
                LD ({address} + A), {immediate}, {count}, {repeat}
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address + 4, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(immediate >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address + 4 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address + 4 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address + 4 + (uint)total]);
        }

        [Fact]
        public void TestEXEC_LD_Innn_rI_r()
        {
            uint address = 10000;

            for (byte i = 0; i < 25; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte expectedValue = (byte)(i * 9);

                string code = $@"
                    LD {TUtils.Get8bitRegisterChar(i)}, {expectedValue}
                    LD Z, 4
                    LD ({address} + Z), {TUtils.Get8bitRegisterChar(i)}
                    BREAK";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint actualValueBelow = computer.MEMC.Get32bitFromRAM((address + 4) - 3);
                uint actualValueAbove = computer.MEMC.Get32bitFromRAM((address + 4) + 1);

                TUtils.IncrementCountedTests("exec");
                // We are making sure nothing "bleeds-out" onto other memory locations
                Assert.True((expectedValue == actualValueBelow) && (actualValueAbove == 0));
            }
        }

        [Fact]
        public void TestEXEC_LD_Innn_rI_rr()
        {
            uint address = 10000;

            for (byte i = 0; i < 24; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                ushort expectedValue = (ushort)(i * 2520);

                string code = $@"
                    LD {TUtils.Get16bitRegisterString(i)}, {expectedValue}
                    LD Z, 4
                    LD ({address} + Z), {TUtils.Get16bitRegisterString(i)}
                    BREAK";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint actualValueBelow = computer.MEMC.Get32bitFromRAM((address + 4) - 4);
                uint actualValue = computer.MEMC.Get16bitFromRAM(address + 4);
                uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 4 + 2);

                TUtils.IncrementCountedTests("exec");
                // We are making sure nothing "bleeds-out" onto other memory locations
                Assert.True((actualValueBelow == 0) && (actualValue == expectedValue) && (actualValueAbove == 0));
            }
        }

        [Fact]
        public void TestEXEC_LD_Innn_rI_rrr()
        {
            uint address = 10000;

            for (byte i = 0; i < 23; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                uint expectedValue = (uint)(i * 645_277);

                string code = $@"
                    LD {TUtils.Get24bitRegisterString(i)}, {expectedValue}
                    LD Z, 4
                    LD ({address} + Z), {TUtils.Get24bitRegisterString(i)}
                    BREAK";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint actualValueBelow = computer.MEMC.Get32bitFromRAM((address + 4) - 4);
                uint actualValue = computer.MEMC.Get24bitFromRAM(address + 4);
                uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 4 + 3);

                TUtils.IncrementCountedTests("exec");
                // We are making sure nothing "bleeds-out" onto other memory locations
                Assert.True((actualValueBelow == 0) && (actualValue == expectedValue) && (actualValueAbove == 0));
            }
        }

        [Fact]
        public void TestEXEC_LD_Innn_rI_rrrr()
        {
            uint address = 10000;

            for (byte i = 0; i < 22; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                uint expectedValue = (uint)(i * 165_191_049);

                string code = $@"
                    LD {TUtils.Get32bitRegisterString(i)}, {expectedValue}
                    LD Z, 4
                    LD ({address} + Z), {TUtils.Get32bitRegisterString(i)}
                    BREAK";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint actualValueBelow = computer.MEMC.Get32bitFromRAM((address + 4) - 4);
                uint actualValue = computer.MEMC.Get32bitFromRAM(address + 4);
                uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 4 + 4);

                TUtils.IncrementCountedTests("exec");
                // We are making sure nothing "bleeds-out" onto other memory locations
                Assert.True((actualValueBelow == 0) && (actualValue == expectedValue) && (actualValueAbove == 0));
            }
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Innn_rI_InnnI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4000, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD D, 4
                LD ({address1} + D), ({address2}), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 4, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 4 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 4 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 4 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Innn_rI_Innn_nnnI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD D, 8
                LD ({address1} + D), ({address2} + 4), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 8, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 8 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 8 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 8 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Innn_rI_Innn_rI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD D, 4
                LD G, 8
                LD ({address1} + G), ({address2} + D), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 8, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 8 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 8 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 8 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Innn_rI_Innn_rrI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DE, 4
                LD G, 8
                LD ({address1} + G), ({address2} + DE), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 8, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 8 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 8 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 8 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Innn_rI_Innn_rrrI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, 4
                LD G, 8
                LD ({address1} + G), ({address2} + DEF), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 8, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 8 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 8 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 8 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Innn_rI_IrrrI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4000, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address2}
                LD G, 8
                LD ({address1} + G), (DEF), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 8, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 8 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 8 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 8 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Innn_rI_Irrr_nnnI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address2}
                LD G, 8
                LD ({address1} + G), (DEF + 4), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 8, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 8 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 8 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 8 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Innn_rI_Irrr_rI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address2}
                LD G, 4
                LD J, 8
                LD ({address1} + J), (DEF + G), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 8, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 8 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 8 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 8 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Innn_rI_Irrr_rrI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address2}
                LD GH, 4
                LD J, 8
                LD ({address1} + J), (DEF + GH), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 8, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 8 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 8 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 8 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Innn_rI_Irrr_rrrI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address2}
                LD GHI, 4
                LD J, 8
                LD ({address1} + J), (DEF + GHI), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 8, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 8 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 8 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 8 + (uint)total]);
        }





        [Theory]
        [InlineData((byte)1)]
        [InlineData((byte)2)]
        [InlineData((byte)3)]
        [InlineData((byte)4)]
        [InlineData((byte)8)]
        [InlineData((byte)255)]
        public void TestEXEC_LD_Innn_rrI_nnnn_n_VariousCounts(byte count)
        {
            // — Arrange —
            Assembler cp = new();
            using var computer = new Computer();

            uint address = 0x1000;
            uint immediate = 0x11223344;  // bytes: 11 22 33 44

            // Build a tiny program: LD (address), immediate32, count ; BREAK
            string source = $@"
                LD AB, 4
                LD ({address} + AB), {immediate}, {count}
                BREAK";
            cp.Build(source);

            byte[] compiled = cp.GetCompiledCode();
            computer.LoadMem(compiled);

            // — Act —
            computer.Run();

            // — Assert —

            // 1) Pull out exactly 'count' bytes at 'address'
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address + 4, count);

            // 2) Build the expected array:
            //    zero-pad on the left for count>4, then copy the low-order bytes of 'immediate'
            var expected = new byte[count];
            int toCopy = Math.Min(count, (byte)4);
            for (int i = 0; i < toCopy; i++)
            {
                // Big-endian slice of 0x11223344:
                //   for toCopy=4 → [0]=0x11, [1]=0x22, [2]=0x33, [3]=0x44
                //   for toCopy=2 → [0]=0x33, [1]=0x44
                expected[count - toCopy + i] =
                    (byte)(immediate >> ((toCopy - 1 - i) * 8));
            }

            Assert.Equal(expected, actual);

            // 3) Check the byte immediately before/after is still zero:
            if ((address + 4) > 0)
            {
                Assert.Equal(0, computer.MEMC.RAM[address - 1]);
            }

            Assert.Equal(0, computer.MEMC.RAM[(address + 4) + count]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Innn_rrI_nnnn_n_nnn(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address = 0x2000;
            uint immediate = 0x11223344;  // pattern bytes: 11 22 33 44

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD AB, 4
                LD ({address} + AB), {immediate}, {count}, {repeat}
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address + 4, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(immediate >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address + 4 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address + 4 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address + 4 + (uint)total]);
        }

        [Fact]
        public void TestEXEC_LD_Innn_rrI_r()
        {
            uint address = 10000;

            for (byte i = 0; i < 24; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte expectedValue = (byte)(i * 9);

                string code = $@"
                    LD {TUtils.Get8bitRegisterChar(i)}, {expectedValue}
                    LD YZ, 4
                    LD ({address} + YZ), {TUtils.Get8bitRegisterChar(i)}
                    BREAK";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint actualValueBelow = computer.MEMC.Get32bitFromRAM((address + 4) - 3);
                uint actualValueAbove = computer.MEMC.Get32bitFromRAM((address + 4) + 1);

                TUtils.IncrementCountedTests("exec");
                // We are making sure nothing "bleeds-out" onto other memory locations
                Assert.True((expectedValue == actualValueBelow) && (actualValueAbove == 0));
            }
        }

        [Fact]
        public void TestEXEC_LD_Innn_rrI_rr()
        {
            uint address = 10000;

            for (byte i = 0; i < 23; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                ushort expectedValue = (ushort)(i * 2520);

                string code = $@"
                    LD {TUtils.Get16bitRegisterString(i)}, {expectedValue}
                    LD YZ, 4
                    LD ({address} + YZ), {TUtils.Get16bitRegisterString(i)}
                    BREAK";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint actualValueBelow = computer.MEMC.Get32bitFromRAM((address + 4) - 4);
                uint actualValue = computer.MEMC.Get16bitFromRAM(address + 4);
                uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 4 + 2);

                TUtils.IncrementCountedTests("exec");
                // We are making sure nothing "bleeds-out" onto other memory locations
                Assert.True((actualValueBelow == 0) && (actualValue == expectedValue) && (actualValueAbove == 0));
            }
        }

        [Fact]
        public void TestEXEC_LD_Innn_rrI_rrr()
        {
            uint address = 10000;

            for (byte i = 0; i < 22; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                uint expectedValue = (uint)(i * 645_277);

                string code = $@"
                    LD {TUtils.Get24bitRegisterString(i)}, {expectedValue}
                    LD YZ, 4
                    LD ({address} + YZ), {TUtils.Get24bitRegisterString(i)}
                    BREAK";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint actualValueBelow = computer.MEMC.Get32bitFromRAM((address + 4) - 4);
                uint actualValue = computer.MEMC.Get24bitFromRAM(address + 4);
                uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 4 + 3);

                TUtils.IncrementCountedTests("exec");
                // We are making sure nothing "bleeds-out" onto other memory locations
                Assert.True((actualValueBelow == 0) && (actualValue == expectedValue) && (actualValueAbove == 0));
            }
        }

        [Fact]
        public void TestEXEC_LD_Innn_rrI_rrrr()
        {
            uint address = 10000;

            for (byte i = 0; i < 21; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                uint expectedValue = (uint)(i * 165_191_049);

                string code = $@"
                    LD {TUtils.Get32bitRegisterString(i)}, {expectedValue}
                    LD YZ, 4
                    LD ({address} + YZ), {TUtils.Get32bitRegisterString(i)}
                    BREAK";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint actualValueBelow = computer.MEMC.Get32bitFromRAM((address + 4) - 4);
                uint actualValue = computer.MEMC.Get32bitFromRAM(address + 4);
                uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 4 + 4);

                TUtils.IncrementCountedTests("exec");
                // We are making sure nothing "bleeds-out" onto other memory locations
                Assert.True((actualValueBelow == 0) && (actualValue == expectedValue) && (actualValueAbove == 0));
            }
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Innn_rrI_InnnI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4000, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DE, 4
                LD ({address1} + DE), ({address2}), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 4, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 4 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 4 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 4 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Innn_rrI_Innn_nnnI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DE, 8
                LD ({address1} + DE), ({address2} + 4), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 8, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 8 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 8 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 8 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Innn_rrI_Innn_rI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD D, 4
                LD GH, 8
                LD ({address1} + GH), ({address2} + D), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 8, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 8 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 8 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 8 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Innn_rrI_Innn_rrI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DE, 4
                LD GH, 8
                LD ({address1} + GH), ({address2} + DE), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 8, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 8 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 8 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 8 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Innn_rrI_Innn_rrrI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, 4
                LD GH, 8
                LD ({address1} + GH), ({address2} + DEF), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 8, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 8 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 8 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 8 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Innn_rrI_IrrrI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4000, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address2}
                LD GH, 8
                LD ({address1} + GH), (DEF), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 8, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 8 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 8 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 8 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Innn_rrI_Irrr_nnnI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address2}
                LD GH, 8
                LD ({address1} + GH), (DEF + 4), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 8, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 8 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 8 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 8 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Innn_rrI_Irrr_rI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address2}
                LD G, 4
                LD JK, 8
                LD ({address1} + JK), (DEF + G), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 8, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 8 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 8 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 8 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Innn_rrI_Irrr_rrI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address2}
                LD GH, 4
                LD JK, 8
                LD ({address1} + JK), (DEF + GH), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 8, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 8 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 8 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 8 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Innn_rrI_Irrr_rrrI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address2}
                LD GHI, 4
                LD JK, 8
                LD ({address1} + JK), (DEF + GHI), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 8, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 8 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 8 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 8 + (uint)total]);
        }






        [Theory]
        [InlineData((byte)1)]
        [InlineData((byte)2)]
        [InlineData((byte)3)]
        [InlineData((byte)4)]
        [InlineData((byte)8)]
        [InlineData((byte)255)]
        public void TestEXEC_LD_Innn_rrrI_nnnn_n_VariousCounts(byte count)
        {
            // — Arrange —
            Assembler cp = new();
            using var computer = new Computer();

            uint address = 0x1000;
            uint immediate = 0x11223344;  // bytes: 11 22 33 44

            // Build a tiny program: LD (address), immediate32, count ; BREAK
            string source = $@"
                LD ABC, 4
                LD ({address} + ABC), {immediate}, {count}
                BREAK";
            cp.Build(source);

            byte[] compiled = cp.GetCompiledCode();
            computer.LoadMem(compiled);

            // — Act —
            computer.Run();

            // — Assert —

            // 1) Pull out exactly 'count' bytes at 'address'
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address + 4, count);

            // 2) Build the expected array:
            //    zero-pad on the left for count>4, then copy the low-order bytes of 'immediate'
            var expected = new byte[count];
            int toCopy = Math.Min(count, (byte)4);
            for (int i = 0; i < toCopy; i++)
            {
                // Big-endian slice of 0x11223344:
                //   for toCopy=4 → [0]=0x11, [1]=0x22, [2]=0x33, [3]=0x44
                //   for toCopy=2 → [0]=0x33, [1]=0x44
                expected[count - toCopy + i] =
                    (byte)(immediate >> ((toCopy - 1 - i) * 8));
            }

            Assert.Equal(expected, actual);

            // 3) Check the byte immediately before/after is still zero:
            if ((address + 4) > 0)
            {
                Assert.Equal(0, computer.MEMC.RAM[address + 4 - 1]);
            }

            Assert.Equal(0, computer.MEMC.RAM[(address + 4) + count]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Innn_rrrI_nnnn_n_nnn(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address = 0x2000;
            uint immediate = 0x11223344;  // pattern bytes: 11 22 33 44

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, 4
                LD ({address} + ABC), {immediate}, {count}, {repeat}
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address + 4, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(immediate >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address + 4 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address + 4 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address + 4 + (uint)total]);
        }

        [Fact]
        public void TestEXEC_LD_Innn_rrrI_r()
        {
            uint address = 10000;

            for (byte i = 0; i < 23; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte expectedValue = (byte)(i * 9);

                string code = $@"
                    LD {TUtils.Get8bitRegisterChar(i)}, {expectedValue}
                    LD XYZ, 4
                    LD ({address} + XYZ), {TUtils.Get8bitRegisterChar(i)}
                    BREAK";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint actualValueBelow = computer.MEMC.Get32bitFromRAM((address + 4) - 3);
                uint actualValueAbove = computer.MEMC.Get32bitFromRAM((address + 4) + 1);

                TUtils.IncrementCountedTests("exec");
                // We are making sure nothing "bleeds-out" onto other memory locations
                Assert.True((expectedValue == actualValueBelow) && (actualValueAbove == 0));
            }
        }

        [Fact]
        public void TestEXEC_LD_Innn_rrrI_rr()
        {
            uint address = 10000;

            for (byte i = 0; i < 22; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                ushort expectedValue = (ushort)(i * 2520);

                string code = $@"
                    LD {TUtils.Get16bitRegisterString(i)}, {expectedValue}
                    LD XYZ, 4
                    LD ({address} + XYZ), {TUtils.Get16bitRegisterString(i)}
                    BREAK";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint actualValueBelow = computer.MEMC.Get32bitFromRAM((address + 4) - 4);
                uint actualValue = computer.MEMC.Get16bitFromRAM(address + 4);
                uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 4 + 2);

                TUtils.IncrementCountedTests("exec");
                // We are making sure nothing "bleeds-out" onto other memory locations
                Assert.True((actualValueBelow == 0) && (actualValue == expectedValue) && (actualValueAbove == 0));
            }
        }

        [Fact]
        public void TestEXEC_LD_Innn_rrrI_rrr()
        {
            uint address = 10000;

            for (byte i = 0; i < 21; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                uint expectedValue = (uint)(i * 645_277);

                string code = $@"
                    LD {TUtils.Get24bitRegisterString(i)}, {expectedValue}
                    LD XYZ, 4
                    LD ({address} + XYZ), {TUtils.Get24bitRegisterString(i)}
                    BREAK";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint actualValueBelow = computer.MEMC.Get32bitFromRAM((address + 4) - 4);
                uint actualValue = computer.MEMC.Get24bitFromRAM(address + 4);
                uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 4 + 3);

                TUtils.IncrementCountedTests("exec");
                // We are making sure nothing "bleeds-out" onto other memory locations
                Assert.True((actualValueBelow == 0) && (actualValue == expectedValue) && (actualValueAbove == 0));
            }
        }

        [Fact]
        public void TestEXEC_LD_Innn_rrrI_rrrr()
        {
            uint address = 10000;

            for (byte i = 0; i < 20; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                uint expectedValue = (uint)(i * 165_191_049);

                string code = $@"
                    LD {TUtils.Get32bitRegisterString(i)}, {expectedValue}
                    LD XYZ, 4
                    LD ({address} + XYZ), {TUtils.Get32bitRegisterString(i)}
                    BREAK";

                cp.Build(code);

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint actualValueBelow = computer.MEMC.Get32bitFromRAM((address + 4) - 4);
                uint actualValue = computer.MEMC.Get32bitFromRAM(address + 4);
                uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 4 + 4);

                TUtils.IncrementCountedTests("exec");
                // We are making sure nothing "bleeds-out" onto other memory locations
                Assert.True((actualValueBelow == 0) && (actualValue == expectedValue) && (actualValueAbove == 0));
            }
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Innn_rrrI_InnnI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4000, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, 4
                LD ({address1} + DEF), ({address2}), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 4, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 4 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 4 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 4 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Innn_rrrI_Innn_nnnI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, 8
                LD ({address1} + DEF), ({address2} + 4), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 8, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 8 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 8 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 8 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Innn_rrrI_Innn_rI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD D, 4
                LD GHI, 8
                LD ({address1} + GHI), ({address2} + D), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 8, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 8 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 8 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 8 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Innn_rrrI_Innn_rrI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DE, 4
                LD GHI, 8
                LD ({address1} + GHI), ({address2} + DE), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 8, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 8 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 8 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 8 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Innn_rrrI_Innn_rrrI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, 4
                LD GHI, 8
                LD ({address1} + GHI), ({address2} + DEF), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 8, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 8 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 8 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 8 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Innn_rrrI_IrrrI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4000, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address2}
                LD GHI, 8
                LD ({address1} + GHI), (DEF), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 8, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 8 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 8 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 8 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Innn_rrrI_Irrr_nnnI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address2}
                LD GHI, 8
                LD ({address1} + GHI), (DEF + 4), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 8, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 8 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 8 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 8 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Innn_rrrI_Irrr_rI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address2}
                LD G, 4
                LD JKL, 8
                LD ({address1} + JKL), (DEF + G), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 8, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 8 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 8 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 8 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Innn_rrrI_Irrr_rrI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address2}
                LD GH, 4
                LD JKL, 8
                LD ({address1} + JKL), (DEF + GH), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 8, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 8 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 8 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 8 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Innn_rrrI_Irrr_rrrI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address2}
                LD GHI, 4
                LD JKL, 8
                LD ({address1} + JKL), (DEF + GHI), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 8, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 8 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 8 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 8 + (uint)total]);
        }



        [Theory]
        [InlineData((byte)1)]
        [InlineData((byte)2)]
        [InlineData((byte)3)]
        [InlineData((byte)4)]
        [InlineData((byte)8)]
        [InlineData((byte)255)]
        public void TestEXEC_LD_IrrrI_nnnn_n_VariousCounts(byte count)
        {
            // — Arrange —
            Assembler cp = new();
            using var computer = new Computer();

            uint address = 0x1000;
            uint immediate = 0x11223344;  // bytes: 11 22 33 44

            // Build a tiny program: LD (address), immediate32, count ; BREAK
            string source = $@"
                LD ABC, {address}
                LD (ABC), {immediate}, {count}
                BREAK";
            cp.Build(source);

            byte[] compiled = cp.GetCompiledCode();
            computer.LoadMem(compiled);

            // — Act —
            computer.Run();

            // — Assert —

            // 1) Pull out exactly 'count' bytes at 'address'
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address, count);

            // 2) Build the expected array:
            //    zero-pad on the left for count>4, then copy the low-order bytes of 'immediate'
            var expected = new byte[count];
            int toCopy = Math.Min(count, (byte)4);
            for (int i = 0; i < toCopy; i++)
            {
                // Big-endian slice of 0x11223344:
                //   for toCopy=4 → [0]=0x11, [1]=0x22, [2]=0x33, [3]=0x44
                //   for toCopy=2 → [0]=0x33, [1]=0x44
                expected[count - toCopy + i] =
                    (byte)(immediate >> ((toCopy - 1 - i) * 8));
            }

            Assert.Equal(expected, actual);

            // 3) Check the byte immediately before/after is still zero:
            if ((address) > 0)
            {
                Assert.Equal(0, computer.MEMC.RAM[address - 1]);
            }

            Assert.Equal(0, computer.MEMC.RAM[address + count]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_IrrrI_nnnn_n_nnn(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address = 0x2000;
            uint immediate = 0x11223344;  // pattern bytes: 11 22 33 44

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {address}
                LD (ABC), {immediate}, {count}, {repeat}
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(immediate >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address > 0)
                Assert.Equal(0, computer.MEMC.RAM[address - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address + (uint)total]);
        }

        [Fact]
        public void TestEXEC_LD_IrrrI_r()
        {
            uint address = 10000;

            for (byte i = 1; i < 23; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                uint expectedValue = (byte)(i * 9 + 1);

                string source = $@"
                    LD {TUtils.Get24bitRegisterString(i)}, {address}
                    LD A, {expectedValue}
                    LD ({TUtils.Get24bitRegisterString(i)}), A
                    BREAK";

                cp.Build(source);

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
        public void TestEXEC_LD_IrrrI_rr()
        {
            uint address = 10000;

            for (byte i = 2; i < 23; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                uint expectedValue = (ushort)(i * 2520 + 1);

                string source = $@"
                    LD {TUtils.Get24bitRegisterString(i)}, {address}
                    LD AB, {expectedValue}
                    LD ({TUtils.Get24bitRegisterString(i)}), AB
                    BREAK";

                cp.Build(source);

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
        public void TestEXEC_LD_IrrrI_rrr()
        {
            uint address = 10000;

            for (byte i = 3; i < 23; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                uint expectedValue = (uint)(i * 645_277 + 1);

                string source = $@"
                    LD {TUtils.Get24bitRegisterString(i)}, {address}
                    LD ABC, {expectedValue}
                    LD ({TUtils.Get24bitRegisterString(i)}), ABC
                    BREAK";

                cp.Build(source);

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
        public void TestEXEC_LD_IrrrI_rrrr()
        {
            uint address = 10000;

            for (byte i = 4; i < 23; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                uint expectedValue = (uint)(i * 165_191_049 + 1);

                string source = $@"
                    LD {TUtils.Get24bitRegisterString(i)}, {address}
                    LD ABCD, {expectedValue}
                    LD ({TUtils.Get24bitRegisterString(i)}), ABCD
                    BREAK";

                cp.Build(source);

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

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_IrrrI_InnnI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4000, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address1}
                LD (DEF), ({address2}), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_IrrrI_Innn_nnnI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address1}
                LD (DEF), ({address2} + 4), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_IrrrI_Innn_rI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD D, 4
                LD GHI, {address1}
                LD (GHI), ({address2} + D), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_IrrrI_Innn_rrI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DE, 4
                LD GHI, {address1}
                LD (GHI), ({address2} + DE), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_IrrrI_Innn_rrrI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, 4
                LD GHI, {address1}
                LD (GHI), ({address2} + DEF), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_IrrrI_IrrrI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4000, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address2}
                LD GHI, {address1}
                LD (GHI), (DEF), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_IrrrI_Irrr_nnnI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address2}
                LD GHI, {address1}
                LD (GHI), (DEF + 4), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_IrrrI_Irrr_rI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address2}
                LD G, 4
                LD JKL, {address1}
                LD (JKL), (DEF + G), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_IrrrI_Irrr_rrI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address2}
                LD GH, 4
                LD JKL, {address1}
                LD (JKL), (DEF + GH), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_IrrrI_Irrr_rrrI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address2}
                LD GHI, 4
                LD JKL, {address1}
                LD (JKL), (DEF + GHI), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + (uint)total]);
        }

        [Theory]
        [InlineData((byte)1)]
        [InlineData((byte)2)]
        [InlineData((byte)3)]
        [InlineData((byte)4)]
        [InlineData((byte)8)]
        [InlineData((byte)255)]
        public void TestEXEC_LD_Irrr_nnnI_nnnn_n_VariousCounts(byte count)
        {
            // — Arrange —
            Assembler cp = new();
            using var computer = new Computer();

            uint address = 0x1000;
            uint immediate = 0x11223344;  // bytes: 11 22 33 44

            // Build a tiny program: LD (address), immediate32, count ; BREAK
            string source = $@"
                LD ABC, {address}
                LD (ABC + 4), {immediate}, {count}
                BREAK";
            cp.Build(source);

            byte[] compiled = cp.GetCompiledCode();
            computer.LoadMem(compiled);

            // — Act —
            computer.Run();

            // — Assert —

            // 1) Pull out exactly 'count' bytes at 'address'
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address + 4, count);

            // 2) Build the expected array:
            //    zero-pad on the left for count>4, then copy the low-order bytes of 'immediate'
            var expected = new byte[count];
            int toCopy = Math.Min(count, (byte)4);
            for (int i = 0; i < toCopy; i++)
            {
                // Big-endian slice of 0x11223344:
                //   for toCopy=4 → [0]=0x11, [1]=0x22, [2]=0x33, [3]=0x44
                //   for toCopy=2 → [0]=0x33, [1]=0x44
                expected[count - toCopy + i] =
                    (byte)(immediate >> ((toCopy - 1 - i) * 8));
            }

            Assert.Equal(expected, actual);

            // 3) Check the byte immediately before/after is still zero:
            if ((address + 4) > 0)
            {
                Assert.Equal(0, computer.MEMC.RAM[address + 4 - 1]);
            }

            Assert.Equal(0, computer.MEMC.RAM[address + 4 + count]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Irrr_nnnI_nnnn_n_nnn(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address = 0x2000;
            uint immediate = 0x11223344;  // pattern bytes: 11 22 33 44

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {address}
                LD (ABC + 4), {immediate}, {count}, {repeat}
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address + 4, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(immediate >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address + 4 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address + 4 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address + 4 + (uint)total]);
        }

        [Fact]
        public void TestEXEC_LD_Irrr_nnnI_r()
        {
            uint address = 10000;

            for (byte i = 1; i < 23; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                uint expectedValue = (byte)(i * 9 + 1);

                string source = $@"
                    LD {TUtils.Get24bitRegisterString(i)}, {address}
                    LD A, {expectedValue}
                    LD ({TUtils.Get24bitRegisterString(i)} + 4), A
                    BREAK";

                cp.Build(source);

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint actualValueBelow = computer.MEMC.Get32bitFromRAM(address + 4 - 3);
                uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 4 + 1);

                TUtils.IncrementCountedTests("exec");
                // We are making sure nothing "bleeds-out" onto other memory locations
                Assert.True((expectedValue == actualValueBelow) && (actualValueAbove == 0));
            }
        }

        [Fact]
        public void TestEXEC_LD_Irrr_nnnI_rr()
        {
            uint address = 10000;

            for (byte i = 2; i < 23; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                uint expectedValue = (ushort)(i * 2520 + 1);

                string source = $@"
                    LD {TUtils.Get24bitRegisterString(i)}, {address}
                    LD AB, {expectedValue}
                    LD ({TUtils.Get24bitRegisterString(i)} + 4), AB
                    BREAK";

                cp.Build(source);

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint actualValueBelow = computer.MEMC.Get32bitFromRAM(address + 4 - 4);
                uint actualValue = computer.MEMC.Get16bitFromRAM(address + 4);
                uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 4 + 2);

                TUtils.IncrementCountedTests("exec");
                // We are making sure nothing "bleeds-out" onto other memory locations
                Assert.True((actualValueBelow == 0) && (actualValue == expectedValue) && (actualValueAbove == 0));
            }
        }

        [Fact]
        public void TestEXEC_LD_Irrr_nnnI_rrr()
        {
            uint address = 10000;

            for (byte i = 3; i < 23; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                uint expectedValue = (uint)(i * 645_277 + 1);

                string source = $@"
                    LD {TUtils.Get24bitRegisterString(i)}, {address}
                    LD ABC, {expectedValue}
                    LD ({TUtils.Get24bitRegisterString(i)} + 4), ABC
                    BREAK";

                cp.Build(source);

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint actualValueBelow = computer.MEMC.Get32bitFromRAM(address + 4 - 4);
                uint actualValue = computer.MEMC.Get24bitFromRAM(address + 4);
                uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 4 + 3);

                TUtils.IncrementCountedTests("exec");
                // We are making sure nothing "bleeds-out" onto other memory locations
                Assert.True((actualValueBelow == 0) && (actualValue == expectedValue) && (actualValueAbove == 0));
            }
        }

        [Fact]
        public void TestEXEC_LD_Irrr_nnnI_rrrr()
        {
            uint address = 10000;

            for (byte i = 4; i < 23; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                uint expectedValue = (uint)(i * 165_191_049 + 1);

                string source = $@"
                    LD {TUtils.Get24bitRegisterString(i)}, {address}
                    LD ABCD, {expectedValue}
                    LD ({TUtils.Get24bitRegisterString(i)} + 4), ABCD
                    BREAK";

                cp.Build(source);

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint actualValueBelow = computer.MEMC.Get32bitFromRAM(address + 4 - 4);
                uint actualValue = computer.MEMC.Get32bitFromRAM(address + 4);
                uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 4 + 4);

                TUtils.IncrementCountedTests("exec");
                // We are making sure nothing "bleeds-out" onto other memory locations
                Assert.True((actualValueBelow == 0) && (actualValue == expectedValue) && (actualValueAbove == 0));
            }
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Irrr_nnnI_InnnI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4000, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address1}
                LD (DEF + 4), ({address2}), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 4, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 4 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 4 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 4 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Irrr_nnnI_Innn_nnnI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address1}
                LD (DEF + 4), ({address2} + 4), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 4, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 4 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 4 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 4 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Irrr_nnnI_Innn_rI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD D, 4
                LD GHI, {address1}
                LD (GHI + 4), ({address2} + D), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 4, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 4 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 4 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 4 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Irrr_nnnI_Innn_rrI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DE, 4
                LD GHI, {address1}
                LD (GHI + 4), ({address2} + DE), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 4, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 4 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 4 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 4 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Irrr_nnnI_Innn_rrrI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, 4
                LD GHI, {address1}
                LD (GHI + 4), ({address2} + DEF), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 4, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 4 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 4 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 4 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Irrr_nnnI_IrrrI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4000, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address2}
                LD GHI, {address1}
                LD (GHI + 4), (DEF), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 4, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 4 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 4 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 4 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Irrr_nnnI_Irrr_nnnI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address2}
                LD GHI, {address1}
                LD (GHI + 5), (DEF + 4), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 5, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 5 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 5 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 5 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Irrr_nnnI_Irrr_rI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address2}
                LD G, 4
                LD JKL, {address1}
                LD (JKL + 5), (DEF + G), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 5, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 5 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 5 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 5 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Irrr_nnnI_Irrr_rrI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address2}
                LD GH, 4
                LD JKL, {address1}
                LD (JKL + 6), (DEF + GH), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 6, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 6 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 6 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 6 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Irrr_nnnI_Irrr_rrrI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address2}
                LD GHI, 4
                LD JKL, {address1}
                LD (JKL + 4), (DEF + GHI), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 4, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 4 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 4 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 4 + (uint)total]);
        }



        [Theory]
        [InlineData((byte)1)]
        [InlineData((byte)2)]
        [InlineData((byte)3)]
        [InlineData((byte)4)]
        [InlineData((byte)8)]
        [InlineData((byte)255)]
        public void TestEXEC_LD_Irrr_rI_nnnn_n_VariousCounts(byte count)
        {
            // — Arrange —
            Assembler cp = new();
            using var computer = new Computer();

            uint address = 0x1000;
            uint immediate = 0x11223344;  // bytes: 11 22 33 44

            // Build a tiny program: LD (address), immediate32, count ; BREAK
            string source = $@"
                LD ABC, {address}
                LD G, 4
                LD (ABC + G), {immediate}, {count}
                BREAK";
            cp.Build(source);

            byte[] compiled = cp.GetCompiledCode();
            computer.LoadMem(compiled);

            // — Act —
            computer.Run();

            // — Assert —

            // 1) Pull out exactly 'count' bytes at 'address'
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address + 4, count);

            // 2) Build the expected array:
            //    zero-pad on the left for count>4, then copy the low-order bytes of 'immediate'
            var expected = new byte[count];
            int toCopy = Math.Min(count, (byte)4);
            for (int i = 0; i < toCopy; i++)
            {
                // Big-endian slice of 0x11223344:
                //   for toCopy=4 → [0]=0x11, [1]=0x22, [2]=0x33, [3]=0x44
                //   for toCopy=2 → [0]=0x33, [1]=0x44
                expected[count - toCopy + i] =
                    (byte)(immediate >> ((toCopy - 1 - i) * 8));
            }

            Assert.Equal(expected, actual);

            // 3) Check the byte immediately before/after is still zero:
            if ((address + 4) > 0)
            {
                Assert.Equal(0, computer.MEMC.RAM[address + 4 - 1]);
            }

            Assert.Equal(0, computer.MEMC.RAM[address + 4 + count]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Irrr_rI_nnnn_n_nnn(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address = 0x2000;
            uint immediate = 0x11223344;  // pattern bytes: 11 22 33 44

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {address}
                LD G, 4
                LD (ABC + G), {immediate}, {count}, {repeat}
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address + 4, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(immediate >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address + 4 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address + 4 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address + 4 + (uint)total]);
        }

        [Fact]
        public void TestEXEC_LD_Irrr_rI_r()
        {
            uint address = 10000;

            for (byte i = 2; i < 23; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                uint expectedValue = (byte)(i * 9 + 1);

                string source = $@"
                    LD {TUtils.Get24bitRegisterString(i)}, {address}
                    LD A, {expectedValue}
                    LD B, 4
                    LD ({TUtils.Get24bitRegisterString(i)} + B), A
                    BREAK";

                cp.Build(source);

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint actualValueBelow = computer.MEMC.Get32bitFromRAM(address + 4 - 3);
                uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 4 + 1);

                TUtils.IncrementCountedTests("exec");
                // We are making sure nothing "bleeds-out" onto other memory locations
                Assert.True((expectedValue == actualValueBelow) && (actualValueAbove == 0));
            }
        }

        [Fact]
        public void TestEXEC_LD_Irrr_rI_rr()
        {
            uint address = 10000;

            for (byte i = 3; i < 23; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                uint expectedValue = (ushort)(i * 2520 + 1);

                string source = $@"
                    LD {TUtils.Get24bitRegisterString(i)}, {address}
                    LD AB, {expectedValue}
                    LD C, 4
                    LD ({TUtils.Get24bitRegisterString(i)} + C), AB
                    BREAK";

                cp.Build(source);

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint actualValueBelow = computer.MEMC.Get32bitFromRAM(address + 4 - 4);
                uint actualValue = computer.MEMC.Get16bitFromRAM(address + 4);
                uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 4 + 2);

                TUtils.IncrementCountedTests("exec");
                // We are making sure nothing "bleeds-out" onto other memory locations
                Assert.True((actualValueBelow == 0) && (actualValue == expectedValue) && (actualValueAbove == 0));
            }
        }

        [Fact]
        public void TestEXEC_LD_Irrr_rI_rrr()
        {
            uint address = 10000;

            for (byte i = 4; i < 23; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                uint expectedValue = (uint)(i * 645_277 + 1);

                string source = $@"
                    LD {TUtils.Get24bitRegisterString(i)}, {address}
                    LD ABC, {expectedValue}
                    LD D, 4
                    LD ({TUtils.Get24bitRegisterString(i)} + D), ABC
                    BREAK";

                cp.Build(source);

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint actualValueBelow = computer.MEMC.Get32bitFromRAM(address + 4 - 4);
                uint actualValue = computer.MEMC.Get24bitFromRAM(address + 4);
                uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 4 + 3);

                TUtils.IncrementCountedTests("exec");
                // We are making sure nothing "bleeds-out" onto other memory locations
                Assert.True((actualValueBelow == 0) && (actualValue == expectedValue) && (actualValueAbove == 0));
            }
        }

        [Fact]
        public void TestEXEC_LD_Irrr_rI_rrrr()
        {
            uint address = 10000;

            for (byte i = 5; i < 23; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                uint expectedValue = (uint)(i * 165_191_049 + 1);

                string source = $@"
                    LD {TUtils.Get24bitRegisterString(i)}, {address}
                    LD ABCD, {expectedValue}
                    LD E, 4
                    LD ({TUtils.Get24bitRegisterString(i)} + E), ABCD
                    BREAK";

                cp.Build(source);

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint actualValueBelow = computer.MEMC.Get32bitFromRAM(address + 4 - 4);
                uint actualValue = computer.MEMC.Get32bitFromRAM(address + 4);
                uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 4 + 4);

                TUtils.IncrementCountedTests("exec");
                // We are making sure nothing "bleeds-out" onto other memory locations
                Assert.True((actualValueBelow == 0) && (actualValue == expectedValue) && (actualValueAbove == 0));
            }
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Irrr_rI_InnnI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4000, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address1}
                LD G, 4
                LD (DEF + G), ({address2}), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 4, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 4 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 4 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 4 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Irrr_rI_Innn_nnnI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address1}
                LD G, 4
                LD (DEF + G), ({address2} + 4), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 4, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 4 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 4 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 4 + (uint)total]);
        }


        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Irrr_rI_Innn_rI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD D, 4
                LD GHI, {address1}
                LD J, 4
                LD (GHI + J), ({address2} + D), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 4, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 4 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 4 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 4 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Irrr_rI_Innn_rrI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DE, 4
                LD GHI, {address1}
                LD J, 4
                LD (GHI + J), ({address2} + DE), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 4, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 4 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 4 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 4 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Irrr_rI_Innn_rrrI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, 4
                LD GHI, {address1}
                LD J, 4
                LD (GHI + J), ({address2} + DEF), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 4, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 4 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 4 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 4 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Irrr_rI_IrrrI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4000, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address2}
                LD GHI, {address1}
                LD J, 4
                LD (GHI + J), (DEF), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 4, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 4 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 4 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 4 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Irrr_rI_Irrr_nnnI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address2}
                LD GHI, {address1}
                LD J, 5
                LD (GHI + J), (DEF + 4), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 5, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 5 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 5 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 5 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Irrr_rI_Irrr_rI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address2}
                LD G, 4
                LD JKL, {address1}
                LD M, 5
                LD (JKL + M), (DEF + G), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 5, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 5 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 5 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 5 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Irrr_rI_Irrr_rrI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address2}
                LD GH, 4
                LD JKL, {address1}
                LD M, 6
                LD (JKL + M), (DEF + GH), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 6, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 6 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 6 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 6 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Irrr_rI_Irrr_rrrI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address2}
                LD GHI, 4
                LD JKL, {address1}
                LD M, 4
                LD (JKL + M), (DEF + GHI), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 4, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 4 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 4 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 4 + (uint)total]);
        }

        [Theory]
        [InlineData((byte)1)]
        [InlineData((byte)2)]
        [InlineData((byte)3)]
        [InlineData((byte)4)]
        [InlineData((byte)8)]
        [InlineData((byte)255)]
        public void TestEXEC_LD_Irrr_rrI_nnnn_n_VariousCounts(byte count)
        {
            // — Arrange —
            Assembler cp = new();
            using var computer = new Computer();

            uint address = 0x1000;
            uint immediate = 0x11223344;  // bytes: 11 22 33 44

            // Build a tiny program: LD (address), immediate32, count ; BREAK
            string source = $@"
                LD ABC, {address}
                LD GH, 4
                LD (ABC + GH), {immediate}, {count}
                BREAK";
            cp.Build(source);

            byte[] compiled = cp.GetCompiledCode();
            computer.LoadMem(compiled);

            // — Act —
            computer.Run();

            // — Assert —

            // 1) Pull out exactly 'count' bytes at 'address'
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address + 4, count);

            // 2) Build the expected array:
            //    zero-pad on the left for count>4, then copy the low-order bytes of 'immediate'
            var expected = new byte[count];
            int toCopy = Math.Min(count, (byte)4);
            for (int i = 0; i < toCopy; i++)
            {
                // Big-endian slice of 0x11223344:
                //   for toCopy=4 → [0]=0x11, [1]=0x22, [2]=0x33, [3]=0x44
                //   for toCopy=2 → [0]=0x33, [1]=0x44
                expected[count - toCopy + i] =
                    (byte)(immediate >> ((toCopy - 1 - i) * 8));
            }

            Assert.Equal(expected, actual);

            // 3) Check the byte immediately before/after is still zero:
            if ((address + 4) > 0)
            {
                Assert.Equal(0, computer.MEMC.RAM[address + 4 - 1]);
            }

            Assert.Equal(0, computer.MEMC.RAM[address + 4 + count]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Irrr_rrI_nnnn_n_nnn(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address = 0x2000;
            uint immediate = 0x11223344;  // pattern bytes: 11 22 33 44

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {address}
                LD GH, 4
                LD (ABC + GH), {immediate}, {count}, {repeat}
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address + 4, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(immediate >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address + 4 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address + 4 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address + 4 + (uint)total]);
        }

        [Fact]
        public void TestEXEC_LD_Irrr_rrI_r()
        {
            uint address = 10000;

            for (byte i = 3; i < 23; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                uint expectedValue = (byte)(i * 9 + 1);

                string source = $@"
                    LD {TUtils.Get24bitRegisterString(i)}, {address}
                    LD A, {expectedValue}
                    LD BC, 4
                    LD ({TUtils.Get24bitRegisterString(i)} + BC), A
                    BREAK";

                cp.Build(source);

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint actualValueBelow = computer.MEMC.Get32bitFromRAM(address + 4 - 3);
                uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 4 + 1);

                TUtils.IncrementCountedTests("exec");
                // We are making sure nothing "bleeds-out" onto other memory locations
                Assert.True((expectedValue == actualValueBelow) && (actualValueAbove == 0));
            }
        }

        [Fact]
        public void TestEXEC_LD_Irrr_rrI_rr()
        {
            uint address = 10000;

            for (byte i = 4; i < 23; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                uint expectedValue = (ushort)(i * 2520 + 1);

                string source = $@"
                    LD {TUtils.Get24bitRegisterString(i)}, {address}
                    LD AB, {expectedValue}
                    LD CD, 4
                    LD ({TUtils.Get24bitRegisterString(i)} + CD), AB
                    BREAK";

                cp.Build(source);

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint actualValueBelow = computer.MEMC.Get32bitFromRAM(address + 4 - 4);
                uint actualValue = computer.MEMC.Get16bitFromRAM(address + 4);
                uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 4 + 2);

                TUtils.IncrementCountedTests("exec");
                // We are making sure nothing "bleeds-out" onto other memory locations
                Assert.True((actualValueBelow == 0) && (actualValue == expectedValue) && (actualValueAbove == 0));
            }
        }

        [Fact]
        public void TestEXEC_LD_Irrr_rrI_rrr()
        {
            uint address = 10000;

            for (byte i = 5; i < 23; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                uint expectedValue = (uint)(i * 645_277 + 1);

                string source = $@"
                    LD {TUtils.Get24bitRegisterString(i)}, {address}
                    LD ABC, {expectedValue}
                    LD DE, 4
                    LD ({TUtils.Get24bitRegisterString(i)} + DE), ABC
                    BREAK";

                cp.Build(source);

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint actualValueBelow = computer.MEMC.Get32bitFromRAM(address + 4 - 4);
                uint actualValue = computer.MEMC.Get24bitFromRAM(address + 4);
                uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 4 + 3);

                TUtils.IncrementCountedTests("exec");
                // We are making sure nothing "bleeds-out" onto other memory locations
                Assert.True((actualValueBelow == 0) && (actualValue == expectedValue) && (actualValueAbove == 0));
            }
        }

        [Fact]
        public void TestEXEC_LD_Irrr_rrI_rrrr()
        {
            uint address = 10000;

            for (byte i = 6; i < 23; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                uint expectedValue = (uint)(i * 165_191_049 + 1);

                string source = $@"
                    LD {TUtils.Get24bitRegisterString(i)}, {address}
                    LD ABCD, {expectedValue}
                    LD EF, 4
                    LD ({TUtils.Get24bitRegisterString(i)} + EF), ABCD
                    BREAK";

                cp.Build(source);

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint actualValueBelow = computer.MEMC.Get32bitFromRAM(address + 4 - 4);
                uint actualValue = computer.MEMC.Get32bitFromRAM(address + 4);
                uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 4 + 4);

                TUtils.IncrementCountedTests("exec");
                // We are making sure nothing "bleeds-out" onto other memory locations
                Assert.True((actualValueBelow == 0) && (actualValue == expectedValue) && (actualValueAbove == 0));
            }
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Irrr_rrI_InnnI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4000, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address1}
                LD GH, 4
                LD (DEF + GH), ({address2}), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 4, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 4 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 4 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 4 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Irrr_rrI_Innn_nnnI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address1}
                LD GH, 4
                LD (DEF + GH), ({address2} + 4), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 4, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 4 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 4 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 4 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Irrr_rrI_Innn_rI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD D, 4
                LD GHI, {address1}
                LD JK, 4
                LD (GHI + JK), ({address2} + D), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 4, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 4 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 4 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 4 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Irrr_rrI_Innn_rrI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DE, 4
                LD GHI, {address1}
                LD JK, 4
                LD (GHI + JK), ({address2} + DE), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 4, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 4 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 4 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 4 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Irrr_rrI_Innn_rrrI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, 4
                LD GHI, {address1}
                LD JK, 4
                LD (GHI + JK), ({address2} + DEF), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 4, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 4 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 4 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 4 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Irrr_rrI_IrrrI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4000, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address2}
                LD GHI, {address1}
                LD JK, 4
                LD (GHI + JK), (DEF), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 4, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 4 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 4 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 4 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Irrr_rrI_Irrr_nnnI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address2}
                LD GHI, {address1}
                LD JK, 5
                LD (GHI + JK), (DEF + 4), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 5, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 5 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 5 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 5 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Irrr_rrI_Irrr_rI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address2}
                LD G, 4
                LD JKL, {address1}
                LD MN, 5
                LD (JKL + MN), (DEF + G), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 5, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 5 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 5 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 5 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Irrr_rrI_Irrr_rrI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address2}
                LD GH, 4
                LD JKL, {address1}
                LD MN, 6
                LD (JKL + MN), (DEF + GH), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 6, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 6 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 6 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 6 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Irrr_rrI_Irrr_rrrI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address2}
                LD GHI, 4
                LD JKL, {address1}
                LD MN, 4
                LD (JKL + MN), (DEF + GHI), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 4, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 4 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 4 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 4 + (uint)total]);
        }












        [Theory]
        [InlineData((byte)1)]
        [InlineData((byte)2)]
        [InlineData((byte)3)]
        [InlineData((byte)4)]
        [InlineData((byte)8)]
        [InlineData((byte)255)]
        public void TestEXEC_LD_Irrr_rrrI_nnnn_n_VariousCounts(byte count)
        {
            // — Arrange —
            Assembler cp = new();
            using var computer = new Computer();

            uint address = 0x1000;
            uint immediate = 0x11223344;  // bytes: 11 22 33 44

            // Build a tiny program: LD (address), immediate32, count ; BREAK
            string source = $@"
                LD ABC, {address}
                LD GHI, 4
                LD (ABC + GHI), {immediate}, {count}
                BREAK";
            cp.Build(source);

            byte[] compiled = cp.GetCompiledCode();
            computer.LoadMem(compiled);

            // — Act —
            computer.Run();

            // — Assert —

            // 1) Pull out exactly 'count' bytes at 'address'
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address + 4, count);

            // 2) Build the expected array:
            //    zero-pad on the left for count>4, then copy the low-order bytes of 'immediate'
            var expected = new byte[count];
            int toCopy = Math.Min(count, (byte)4);
            for (int i = 0; i < toCopy; i++)
            {
                // Big-endian slice of 0x11223344:
                //   for toCopy=4 → [0]=0x11, [1]=0x22, [2]=0x33, [3]=0x44
                //   for toCopy=2 → [0]=0x33, [1]=0x44
                expected[count - toCopy + i] =
                    (byte)(immediate >> ((toCopy - 1 - i) * 8));
            }

            Assert.Equal(expected, actual);

            // 3) Check the byte immediately before/after is still zero:
            if ((address + 4) > 0)
            {
                Assert.Equal(0, computer.MEMC.RAM[address + 4 - 1]);
            }

            Assert.Equal(0, computer.MEMC.RAM[address + 4 + count]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Irrr_rrrI_nnnn_n_nnn(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address = 0x2000;
            uint immediate = 0x11223344;  // pattern bytes: 11 22 33 44

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {address}
                LD GHI, 4
                LD (ABC + GHI), {immediate}, {count}, {repeat}
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address + 4, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(immediate >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address + 4 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address + 4 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address + 4 + (uint)total]);
        }

        [Fact]
        public void TestEXEC_LD_Irrr_rrrI_r()
        {
            uint address = 10000;

            for (byte i = 4; i < 23; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                uint expectedValue = (byte)(i * 9 + 1);

                string source = $@"
                    LD {TUtils.Get24bitRegisterString(i)}, {address}
                    LD A, {expectedValue}
                    LD BCD, 4
                    LD ({TUtils.Get24bitRegisterString(i)} + BCD), A
                    BREAK";

                cp.Build(source);

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint actualValueBelow = computer.MEMC.Get32bitFromRAM(address + 4 - 3);
                uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 4 + 1);

                TUtils.IncrementCountedTests("exec");
                // We are making sure nothing "bleeds-out" onto other memory locations
                Assert.True((expectedValue == actualValueBelow) && (actualValueAbove == 0));
            }
        }

        [Fact]
        public void TestEXEC_LD_Irrr_rrrI_rr()
        {
            uint address = 10000;

            for (byte i = 5; i < 23; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                uint expectedValue = (ushort)(i * 2520 + 1);

                string source = $@"
                    LD {TUtils.Get24bitRegisterString(i)}, {address}
                    LD AB, {expectedValue}
                    LD CDE, 4
                    LD ({TUtils.Get24bitRegisterString(i)} + CDE), AB
                    BREAK";

                cp.Build(source);

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint actualValueBelow = computer.MEMC.Get32bitFromRAM(address + 4 - 4);
                uint actualValue = computer.MEMC.Get16bitFromRAM(address + 4);
                uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 4 + 2);

                TUtils.IncrementCountedTests("exec");
                // We are making sure nothing "bleeds-out" onto other memory locations
                Assert.True((actualValueBelow == 0) && (actualValue == expectedValue) && (actualValueAbove == 0));
            }
        }

        [Fact]
        public void TestEXEC_LD_Irrr_rrrI_rrr()
        {
            uint address = 10000;

            for (byte i = 6; i < 23; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                uint expectedValue = (uint)(i * 645_277 + 1);

                string source = $@"
                    LD {TUtils.Get24bitRegisterString(i)}, {address}
                    LD ABC, {expectedValue}
                    LD DEF, 4
                    LD ({TUtils.Get24bitRegisterString(i)} + DEF), ABC
                    BREAK";

                cp.Build(source);

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint actualValueBelow = computer.MEMC.Get32bitFromRAM(address + 4 - 4);
                uint actualValue = computer.MEMC.Get24bitFromRAM(address + 4);
                uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 4 + 3);

                TUtils.IncrementCountedTests("exec");
                // We are making sure nothing "bleeds-out" onto other memory locations
                Assert.True((actualValueBelow == 0) && (actualValue == expectedValue) && (actualValueAbove == 0));
            }
        }

        [Fact]
        public void TestEXEC_LD_Irrr_rrrI_rrrr()
        {
            uint address = 10000;

            for (byte i = 7; i < 23; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                uint expectedValue = (uint)(i * 165_191_049 + 1);

                string source = $@"
                    LD {TUtils.Get24bitRegisterString(i)}, {address}
                    LD ABCD, {expectedValue}
                    LD EFG, 4
                    LD ({TUtils.Get24bitRegisterString(i)} + EFG), ABCD
                    BREAK";

                cp.Build(source);

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                uint actualValueBelow = computer.MEMC.Get32bitFromRAM(address + 4 - 4);
                uint actualValue = computer.MEMC.Get32bitFromRAM(address + 4);
                uint actualValueAbove = computer.MEMC.Get32bitFromRAM(address + 4 + 4);

                TUtils.IncrementCountedTests("exec");
                // We are making sure nothing "bleeds-out" onto other memory locations
                Assert.True((actualValueBelow == 0) && (actualValue == expectedValue) && (actualValueAbove == 0));
            }
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Irrr_rrrI_InnnI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4000, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address1}
                LD GHI, 4
                LD (DEF + GHI), ({address2}), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 4, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 4 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 4 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 4 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Irrr_rrrI_Innn_nnnI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address1}
                LD GHI, 4
                LD (DEF + GHI), ({address2} + 4), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 4, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 4 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 4 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 4 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Irrr_rrrI_Innn_rI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD D, 4
                LD GHI, {address1}
                LD JKL, 4
                LD (GHI + JKL), ({address2} + D), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 4, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 4 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 4 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 4 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Irrr_rrrI_Innn_rrI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DE, 4
                LD GHI, {address1}
                LD JKL, 4
                LD (GHI + JKL), ({address2} + DE), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 4, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 4 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 4 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 4 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Irrr_rrrI_Innn_rrrI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, 4
                LD GHI, {address1}
                LD JKL, 4
                LD (GHI + JKL), ({address2} + DEF), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 4, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 4 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 4 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 4 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Irrr_rrrI_IrrrI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4000, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address2}
                LD GHI, {address1}
                LD JKL, 4
                LD (GHI + JKL), (DEF), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 4, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 4 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 4 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 4 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Irrr_rrrI_Irrr_nnnI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address2}
                LD GHI, {address1}
                LD JKL, 5
                LD (GHI + JKL), (DEF + 4), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 5, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 5 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 5 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 5 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Irrr_rrrI_Irrr_rI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address2}
                LD G, 4
                LD JKL, {address1}
                LD MNO, 5
                LD (JKL + MNO), (DEF + G), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 5, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 5 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 5 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 5 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Irrr_rrrI_Irrr_rrI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address2}
                LD GH, 4
                LD JKL, {address1}
                LD MNO, 6
                LD (JKL + MNO), (DEF + GH), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 6, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 6 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 6 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 6 + (uint)total]);
        }

        [Theory]
        // count, repeat
        [InlineData((byte)1, (uint)1)]
        [InlineData((byte)1, (uint)4)]
        [InlineData((byte)2, (uint)3)]
        [InlineData((byte)3, (uint)2)]
        [InlineData((byte)4, (uint)5)]
        [InlineData((byte)8, (uint)3)]  // test >4 to verify zero-padding
        public void TestEXEC_LD_Irrr_rrrI_Irrr_rrrI_n_rrr(byte count, uint repeat)
        {
            // Arrange
            var cp = new Assembler();
            using var computer = new Computer();

            uint address1 = 0x2000;
            uint address2 = 0x4000;  // 4 bytes later, stores pattern bytes: 11 22 33 44

            computer.LoadMemAt(0x4004, new byte[] { 0x11, 0x22, 0x33, 0x44 });

            // build: LD (address), immediate, count, repeat ; BREAK
            string src = $@"
                LD ABC, {repeat}
                LD DEF, {address2}
                LD GHI, 4
                LD JKL, {address1}
                LD MNO, 4
                LD (JKL + MNO), (DEF + GHI), {count}, ABC
                BREAK";
            cp.Build(src);

            byte[] code = cp.GetCompiledCode();
            computer.LoadMem(code);

            // Act
            computer.Run();

            // Assert
            int total = checked((int)(count * repeat));
            byte[] actual = computer.MEMC.RAM.GetMemoryAt(address1 + 4, total);

            // construct expected single “block”
            byte toCopy = Math.Min(count, (byte)4);
            byte[] block = new byte[count];
            // left zero-pad
            for (int i = 0; i < count - toCopy; i++)
                block[i] = 0;
            // fill the toCopy bytes in big‐endian
            for (int i = 0; i < toCopy; i++)
            {
                // i=0 => highest of the low-order toCopy bytes
                block[(count - toCopy) + i] =
                    (byte)(0x11223344 >> ((toCopy - 1 - i) * 8));
            }

            // repeat it
            var expected = new byte[total];
            for (int r = 0; r < repeat; r++)
                Array.Copy(block, 0, expected, r * count, count);

            Assert.Equal(expected, actual);

            // check no bleed
            if (address1 + 4 > 0)
                Assert.Equal(0, computer.MEMC.RAM[address1 + 4 - 1]);

            Assert.Equal(0, computer.MEMC.RAM[address1 + 4 + (uint)total]);
        }













    }
}
