using Continuum93.Emulator;

namespace Registers
{

    public class TestFloatRegisters
    {
        [Fact]
        public void TestRegisterLimitAt16()
        {
            using Computer computer = new();
            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                float f0 = computer.CPU.FREGS.GetRegister(16);
            });

            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                computer.CPU.FREGS.SetRegister(16, 0);
            });
        }

        [Fact]
        public void TestRegisterAssignment()
        {
            using Computer computer = new();

            float[] fregs = new float[16];

            for (byte i = 0; i < 16; i++)
            {
                fregs[i] = computer.CPU.FREGS.GetRegister(i);
            }

            for (byte i = 0; i < 16; i++)
            {
                Assert.Equal(0, fregs[i]);
            }

            float[] floatPool = new float[] {
                1.1f, 2.2f, 3.3f, 4.4f, 5.5f, 6.6f, 7.7f, 8.8f,
                9.9f, 11, 12.1f, 13.2f, 14.3f, 15.4f, 16.5f, 17.6f
            };

            for (byte i = 0; i < 16; i++)
            {
                computer.CPU.FREGS.SetRegister(i, floatPool[i]);
            }

            for (byte i = 0; i < 16; i++)
            {
                fregs[i] = computer.CPU.FREGS.GetRegister(i);
            }

            for (byte i = 0; i < 16; i++)
            {
                Assert.Equal(floatPool[i], fregs[i]);
            }
        }
    }
}
