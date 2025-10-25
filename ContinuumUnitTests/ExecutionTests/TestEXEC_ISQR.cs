using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;
using Continuum93.Tools;

namespace ExecutionTests
{

    public class TestEXEC_ISQR
    {
        [Fact]
        public void TestEXEC_ISQR_fr()
        {
            for (byte i = 0; i < 16; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                float value = i + 17.0f;
                computer.CPU.FREGS.SetRegister(i, value);

                cp.Build(
                    @$"ISQR F{i}
                    BREAK"
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                Assert.Equal(CMath.InverseSqrt(value), computer.CPU.FREGS.GetRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_ISQR_r()
        {
            for (byte i = 0; i < 16; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte value = 18;
                computer.CPU.REGS.Set8BitRegister(i, value);

                cp.Build(
                    @$"ISQR {TUtils.Get8bitRegisterChar(i)}
                    BREAK"
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                Assert.Equal(1 / 4, computer.CPU.REGS.Get8BitRegister(i), 0.0000001f);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_ISQR_rr()
        {
            for (byte i = 0; i < 16; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                ushort value = 30000;
                computer.CPU.REGS.Set16BitRegister(i, value);

                cp.Build(
                    @$"ISQR {TUtils.Get16bitRegisterString(i)}
                    BREAK"
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                Assert.Equal(1 / 173, computer.CPU.REGS.Get16BitRegister(i), 0.0000001f);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_ISQR_rrr()
        {
            for (byte i = 0; i < 16; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                uint value = 150_500;
                computer.CPU.REGS.Set24BitRegister(i, value);

                cp.Build(
                    @$"ISQR {TUtils.Get24bitRegisterString(i)}
                    BREAK"
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                Assert.Equal((uint)(1.0 / 387), computer.CPU.REGS.Get24BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_ISQR_rrrr()
        {
            for (byte i = 0; i < 16; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                uint value = 180_700_600;
                computer.CPU.REGS.Set32BitRegister(i, value);

                cp.Build(
                    @$"ISQR {TUtils.Get32bitRegisterString(i)}
                    BREAK"
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                Assert.Equal((uint)(1.0 / 13_442), computer.CPU.REGS.Get32BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_ISQR_IrrrI()
        {
            for (byte i = 0; i < 16; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                computer.CPU.REGS.Set24BitRegister(i, 0x2000);
                computer.MEMC.SetFloatToRam(0x2000, 12500);

                cp.Build(
                    @$"ISQR ({TUtils.Get24bitRegisterString(i)})
                    BREAK"
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                Assert.Equal(1.0 / 111.803398f, computer.MEMC.GetFloatFromRAM(0x2000), 0.0000001f);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_ISQR_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.MEMC.SetFloatToRam(0x2000, 12500);

            cp.Build(
                @$"ISQR (0x2000)
                BREAK"
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(1.0 / 111.803398f, computer.MEMC.GetFloatFromRAM(0x2000), 0.0000000001f);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_ISQR_fr_fr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 2.0f);
            computer.CPU.FREGS.SetRegister(1, 3.0f);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "ISQR F0, F1",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(1.0 / 1.7320508f, computer.CPU.FREGS.GetRegister(0), 0.0000001f);
            Assert.Equal(3.0f, computer.CPU.FREGS.GetRegister(1));

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_ISQR_fr_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 2.0f);
            computer.CPU.REGS.A = 3;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "ISQR F0, A",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(1.0 / 1.7320508f, computer.CPU.FREGS.GetRegister(0), 0.0000001f);
            Assert.Equal(3, computer.CPU.REGS.A);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_ISQR_fr_rr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 2.0f);
            computer.CPU.REGS.AB = 3000;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "ISQR F0, AB",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(1.0 / 54.772255f, computer.CPU.FREGS.GetRegister(0), 0.0000001f);
            Assert.Equal(3000, computer.CPU.REGS.AB);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_ISQR_fr_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 2.0f);
            computer.CPU.REGS.ABC = 150_000;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "ISQR F0, ABC",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(1.0 / 387.29833f, computer.CPU.FREGS.GetRegister(0), 0.0000000001f);
            Assert.Equal((uint)150_000, computer.CPU.REGS.ABC);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_ISQR_fr_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 2.0f);
            computer.CPU.REGS.ABCD = 150_000_200;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "ISQR F0, ABCD",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(1.0 / 12247.457f, computer.CPU.FREGS.GetRegister(0), 0.0000000001f);
            Assert.Equal((uint)150_000_200, computer.CPU.REGS.ABCD);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_ISQR_fr_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 2.0f);
            computer.CPU.REGS.ABC = 0x2000;
            computer.MEMC.SetFloatToRam(0x2000, 123.456f);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "ISQR F0, (ABC)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(1.0 / 11.111075f, computer.CPU.FREGS.GetRegister(0), 0.0000001f);
            Assert.Equal(123.456f, computer.MEMC.GetFloatFromRAM(0x2000));

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_ISQR_fr_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 2.0f);
            computer.MEMC.SetFloatToRam(0x2000, 123.456f);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "ISQR F0, (0x2000)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(1.0 / 11.111075f, computer.CPU.FREGS.GetRegister(0), 0.0000001f);
            Assert.Equal(123.456f, computer.MEMC.GetFloatFromRAM(0x2000));

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_ISQR_fr_n()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 2.0f);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "ISQR F0, 123.456",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(1.0 / 11.111075f, computer.CPU.FREGS.GetRegister(0), 0.0000001f);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_ISQR_r_fr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 55.1f);
            computer.CPU.REGS.A = 3;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "ISQR A, F0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(55.1f, computer.CPU.FREGS.GetRegister(0));
            Assert.Equal(1 / 7, computer.CPU.REGS.A);
            Assert.False(computer.CPU.FLAGS.IsOverflow());

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_ISQR_rr_fr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 500_000_000f);
            computer.CPU.REGS.AB = 3;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "ISQR AB, F0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(500_000_000f, computer.CPU.FREGS.GetRegister(0), 0.0000001f);
            Assert.Equal(1 / 22_360, computer.CPU.REGS.AB);
            Assert.False(computer.CPU.FLAGS.IsOverflow());

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_ISQR_rrr_fr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 4_489_000_000.0f);
            computer.CPU.REGS.ABC = 3;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "ISQR ABC, F0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(4_489_000_000f, computer.CPU.FREGS.GetRegister(0));
            Assert.Equal((uint)(1.0 / 67_000), computer.CPU.REGS.ABC);
            Assert.False(computer.CPU.FLAGS.IsOverflow());

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_ISQR_rrrr_fr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 1_225_000_000_000_000f);
            computer.CPU.REGS.ABCD = 3;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "ISQR ABCD, F0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(1_225_000_000_000_000f, computer.CPU.FREGS.GetRegister(0));
            Assert.Equal((uint)(1.0 / 35_000_000), computer.CPU.REGS.ABCD);
            Assert.False(computer.CPU.FLAGS.IsOverflow());

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_ISQR_IrrrI_fr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 4_489_000_000.0f);
            computer.CPU.REGS.ABC = 0x2000;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "ISQR (ABC), F0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(4_489_000_000f, computer.CPU.FREGS.GetRegister(0));
            Assert.Equal(1.0 / 67_000, computer.MEMC.GetFloatFromRAM(0x2000), 0.0000001f);
            Assert.False(computer.CPU.FLAGS.IsOverflow());

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_ISQR_InnnI_fr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 4_489_000_000.0f);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "ISQR (0x2000), F0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(4_489_000_000f, computer.CPU.FREGS.GetRegister(0));
            Assert.Equal(1.0 / 67_000, computer.MEMC.GetFloatFromRAM(0x2000), 0.0000000001f);
            Assert.False(computer.CPU.FLAGS.IsOverflow());

            TUtils.IncrementCountedTests("exec");
        }
    }
}
