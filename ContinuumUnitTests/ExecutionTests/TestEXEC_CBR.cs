using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{

    public class TestEXEC_CBR
    {
        [Fact]
        public void TestEXEC_CBR_fr()
        {
            for (byte i = 0; i < 16; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                float value = i + 17.0f;
                computer.CPU.FREGS.SetRegister(i, value);

                cp.Build(
                    @$"CBR F{i}
                    BREAK"
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                Assert.Equal(MathF.Cbrt(value), computer.CPU.FREGS.GetRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_CBR_r()
        {
            for (byte i = 0; i < 16; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                byte value = 18;
                computer.CPU.REGS.Set8BitRegister(i, value);

                cp.Build(
                    @$"CBR {TUtils.Get8bitRegisterChar(i)}
                    BREAK"
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                Assert.Equal(2, computer.CPU.REGS.Get8BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_CBR_rr()
        {
            for (byte i = 0; i < 16; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                ushort value = 30000;
                computer.CPU.REGS.Set16BitRegister(i, value);

                cp.Build(
                    @$"CBR {TUtils.Get16bitRegisterString(i)}
                    BREAK"
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                Assert.Equal(31, computer.CPU.REGS.Get16BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_CBR_rrr()
        {
            for (byte i = 0; i < 16; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                uint value = 150_500;
                computer.CPU.REGS.Set24BitRegister(i, value);

                cp.Build(
                    @$"CBR {TUtils.Get24bitRegisterString(i)}
                    BREAK"
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                Assert.Equal((uint)53, computer.CPU.REGS.Get24BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_CBR_rrrr()
        {
            for (byte i = 0; i < 16; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                uint value = 180_700_600;
                computer.CPU.REGS.Set32BitRegister(i, value);

                cp.Build(
                    @$"CBR {TUtils.Get32bitRegisterString(i)}
                    BREAK"
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                Assert.Equal((uint)565, computer.CPU.REGS.Get32BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_CBR_IrrrI()
        {
            for (byte i = 0; i < 16; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                computer.CPU.REGS.Set24BitRegister(i, 0x2000);
                computer.MEMC.SetFloatToRam(0x2000, 12500);

                cp.Build(
                    @$"CBR ({TUtils.Get24bitRegisterString(i)})
                    BREAK"
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                Assert.Equal(23.2079441f, computer.MEMC.GetFloatFromRAM(0x2000));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_CBR_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.MEMC.SetFloatToRam(0x2000, 12500);

            cp.Build(
                @$"CBR (0x2000)
                BREAK"
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(23.2079441f, computer.MEMC.GetFloatFromRAM(0x2000));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_CBR_fr_fr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FREGS.SetRegister(0, 2.0f);
            computer.CPU.FREGS.SetRegister(1, 3.0f);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "CBR F0, F1",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(1.4422495f, computer.CPU.FREGS.GetRegister(0));
            Assert.Equal(3.0f, computer.CPU.FREGS.GetRegister(1));

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_CBR_fr_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 2.0f);
            computer.CPU.REGS.A = 3;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "CBR F0, A",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(1.4422495f, computer.CPU.FREGS.GetRegister(0));
            Assert.Equal(3, computer.CPU.REGS.A);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_CBR_fr_rr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 2.0f);
            computer.CPU.REGS.AB = 3000;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "CBR F0, AB",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(14.422495f, computer.CPU.FREGS.GetRegister(0));
            Assert.Equal(3000, computer.CPU.REGS.AB);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_CBR_fr_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 2.0f);
            computer.CPU.REGS.ABC = 150_000;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "CBR F0, ABC",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(53.132928f, computer.CPU.FREGS.GetRegister(0));
            Assert.Equal((uint)150_000, computer.CPU.REGS.ABC);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_CBR_fr_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 2.0f);
            computer.CPU.REGS.ABCD = 150_000_200;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "CBR F0, ABCD",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(531.32952f, computer.CPU.FREGS.GetRegister(0));
            Assert.Equal((uint)150_000_200, computer.CPU.REGS.ABCD);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_CBR_fr_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 2.0f);
            computer.CPU.REGS.ABC = 0x2000;
            computer.MEMC.SetFloatToRam(0x2000, 123.456f);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "CBR F0, (ABC)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(4.97932816f, computer.CPU.FREGS.GetRegister(0));
            Assert.Equal(123.456f, computer.MEMC.GetFloatFromRAM(0x2000));

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_CBR_fr_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 2.0f);
            computer.MEMC.SetFloatToRam(0x2000, 123.456f);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "CBR F0, (0x2000)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(4.97932816f, computer.CPU.FREGS.GetRegister(0));
            Assert.Equal(123.456f, computer.MEMC.GetFloatFromRAM(0x2000));

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_CBR_fr_n()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 2.0f);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "CBR F0, 123.456",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(4.97932816f, computer.CPU.FREGS.GetRegister(0));

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_CBR_r_fr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 55.1f);
            computer.CPU.REGS.A = 100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "CBR A, F0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(55.1f, computer.CPU.FREGS.GetRegister(0));
            Assert.Equal(3, computer.CPU.REGS.A);
            Assert.False(computer.CPU.FLAGS.IsOverflow());

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_CBR_r_fr_overflow()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 550_000_000.1f);
            computer.CPU.REGS.A = 3;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "CBR A, F0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(550_000_000.1f, computer.CPU.FREGS.GetRegister(0));
            Assert.Equal(3, computer.CPU.REGS.A);
            Assert.True(computer.CPU.FLAGS.IsOverflow());

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_CBR_rr_fr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 500_000_000f);
            computer.CPU.REGS.AB = 3;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "CBR AB, F0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(500_000_000f, computer.CPU.FREGS.GetRegister(0));
            Assert.Equal(793, computer.CPU.REGS.AB);
            Assert.False(computer.CPU.FLAGS.IsOverflow());

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_CBR_rr_fr_overflow()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 4_489_000_000_000_000f);
            computer.CPU.REGS.AB = 3;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "CBR AB, F0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(4_489_000_000_000_000f, computer.CPU.FREGS.GetRegister(0));
            Assert.Equal(3, computer.CPU.REGS.AB);
            Assert.True(computer.CPU.FLAGS.IsOverflow());

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_CBR_rrr_fr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 4_489_000_000.0f);
            computer.CPU.REGS.ABC = 3;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "CBR ABC, F0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(4_489_000_000f, computer.CPU.FREGS.GetRegister(0));
            Assert.Equal((uint)1649, computer.CPU.REGS.ABC);
            Assert.False(computer.CPU.FLAGS.IsOverflow());

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_CBR_rrr_fr_overflow()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 324_000_000_000_000_000_000_000f);
            computer.CPU.REGS.ABC = 3;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "CBR ABC, F0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(324_000_000_000_000_000_000_000f, computer.CPU.FREGS.GetRegister(0));
            Assert.Equal((uint)3, computer.CPU.REGS.ABC);
            Assert.True(computer.CPU.FLAGS.IsOverflow());

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_CBR_rrrr_fr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 1_225_000_000_000_000f);
            computer.CPU.REGS.ABCD = 3;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "CBR ABCD, F0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(1_225_000_000_000_000f, computer.CPU.FREGS.GetRegister(0));
            Assert.Equal((uint)106_998, computer.CPU.REGS.ABCD);
            Assert.False(computer.CPU.FLAGS.IsOverflow());

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_CBR_rrrr_fr_overflow()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 25_000_000_000_000_000_000_000_000_000_000.0f);
            computer.CPU.REGS.ABCD = 3;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "CBR ABCD, F0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(25_000_000_000_000_000_000_000_000_000_000.0f, computer.CPU.FREGS.GetRegister(0));
            Assert.Equal((uint)3, computer.CPU.REGS.ABCD);
            Assert.True(computer.CPU.FLAGS.IsOverflow());

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_CBR_IrrrI_fr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 4_489_000_000.0f);
            computer.CPU.REGS.ABC = 0x2000;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "CBR (ABC), F0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(4_489_000_000f, computer.CPU.FREGS.GetRegister(0));
            Assert.Equal(1649.617297f, computer.MEMC.GetFloatFromRAM(0x2000));
            Assert.False(computer.CPU.FLAGS.IsOverflow());

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_CBR_InnnI_fr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 4_489_000_000.0f);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "CBR (0x2000), F0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(4_489_000_000f, computer.CPU.FREGS.GetRegister(0));
            Assert.Equal(1649.617297f, computer.MEMC.GetFloatFromRAM(0x2000));
            Assert.False(computer.CPU.FLAGS.IsOverflow());

            TUtils.IncrementCountedTests("exec");
        }
    }
}
