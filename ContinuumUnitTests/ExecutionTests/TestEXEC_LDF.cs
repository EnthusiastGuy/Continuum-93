using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{

    public class TestEXEC_LDF
    {
        [Fact]
        public void TestEXEC_LDF_r_all()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 0; i < 26; i++)
            {
                computer.Clear();

                Assert.Equal(0, computer.CPU.REGS.Get8BitRegister(i));

                computer.CPU.FLAGS.SetAll();
                cp.Build(
                    @$"
                        LDF {TUtils.Get8bitRegisterChar(i)}
                        BREAK
                    "
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                Assert.Equal(0xFF, computer.CPU.REGS.Get8BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LDF_r_zero()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 0; i < 26; i++)
            {
                computer.Clear();

                Assert.Equal(0, computer.CPU.REGS.Get8BitRegister(i));

                computer.CPU.FLAGS.SetZero(true);
                cp.Build(
                    @$"LDF {TUtils.Get8bitRegisterChar(i)}
                       BREAK");

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                Assert.Equal(0x01, computer.CPU.REGS.Get8BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LDF_IrrrI_all()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 0; i < 26; i++)
            {
                computer.Clear();
                computer.CPU.REGS.Set24BitRegister(i, (uint)(0x2000 + i));

                computer.CPU.FLAGS.SetAll();
                cp.Build(
                    @$"LDF ({TUtils.Get24bitRegisterString(i)})
                       BREAK");

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                Assert.Equal(0xFF, computer.MEMC.Get8bitFromRAM((uint)(0x2000 + i)));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LDF_InnnI_all()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();

            computer.CPU.FLAGS.SetAll();
            cp.Build(
                @"LDF (0x2000)
                  BREAK");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0xFF, computer.MEMC.Get8bitFromRAM(0x2000));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_LDF_r_n_all()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 0; i < 26; i++)
            {
                computer.Clear();

                Assert.Equal(0, computer.CPU.REGS.Get8BitRegister(i));

                computer.CPU.FLAGS.SetAll();
                cp.Build(
                    @$"
                        LDF {TUtils.Get8bitRegisterChar(i)}, {i}
                        BREAK
                    "
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                Assert.Equal(i, computer.CPU.REGS.Get8BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LDF_r_n_zero()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 0; i < 26; i++)
            {
                computer.Clear();

                Assert.Equal(0, computer.CPU.REGS.Get8BitRegister(i));

                computer.CPU.FLAGS.SetZero(true);
                cp.Build(
                    @$"LDF {TUtils.Get8bitRegisterChar(i)}, 0b00000010
                       BREAK");

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                Assert.Equal(0x00, computer.CPU.REGS.Get8BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LDF_IrrrI_n_all()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 0; i < 26; i++)
            {
                computer.Clear();
                computer.CPU.REGS.Set24BitRegister(i, (uint)(0x2000 + i));

                computer.CPU.FLAGS.SetAll();
                cp.Build(
                    @$"LDF ({TUtils.Get24bitRegisterString(i)}), 0b00001111
                       BREAK");

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                Assert.Equal(0x0F, computer.MEMC.Get8bitFromRAM((uint)(0x2000 + i)));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_LDF_InnnI_n_all()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();

            computer.CPU.FLAGS.SetAll();
            cp.Build(
                @"LDF (0x2000), 0x0F
                  BREAK");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0x0F, computer.MEMC.Get8bitFromRAM(0x2000));
            TUtils.IncrementCountedTests("exec");
        }
    }
}
