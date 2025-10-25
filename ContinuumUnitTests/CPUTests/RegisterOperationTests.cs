using Continuum93.Emulator;
using Continuum93.Emulator.Mnemonics;

namespace CPUTests
{

    public class RegisterOperationTests
    {
        [Fact]
        public void CPU_REGS_Addition8bit()
        {
            using Computer computer = new();
            computer.CPU.REGS.Set8BitRegister(0, 200);
            computer.CPU.REGS.AddTo8BitRegister(0, 50);

            Assert.Equal(250, computer.CPU.REGS.A);

            computer.CPU.REGS.Set8BitRegister(0, 200);
            computer.CPU.REGS.AddTo8BitRegister(0, 100);

            Assert.Equal(44, computer.CPU.REGS.A);

            computer.CPU.REGS.Set8BitRegister(0, 200);
            computer.CPU.REGS.AddTo8BitRegister(0, 200);

            Assert.Equal(144, computer.CPU.REGS.A);
        }

        [Fact]
        public void CPU_REGS_Addition16bit()
        {
            using Computer computer = new();
            computer.CPU.REGS.Set16BitRegister(0, 1800);
            computer.CPU.REGS.AddTo16BitRegister(0, 50);

            Assert.Equal(1850, computer.CPU.REGS.AB);

            computer.CPU.REGS.Set16BitRegister(0, 1800);
            computer.CPU.REGS.AddTo16BitRegister(0, 1900);

            Assert.Equal(3700, computer.CPU.REGS.AB);

            computer.CPU.REGS.Set16BitRegister(0, 32768);
            computer.CPU.REGS.AddTo16BitRegister(0, 32769);

            Assert.Equal(1, computer.CPU.REGS.AB);
        }

        [Fact]
        public void CPU_REGS_Addition24bit()
        {
            using Computer computer = new();
            computer.CPU.REGS.Set24BitRegister(0, 65536);
            computer.CPU.REGS.AddTo24BitRegister(0, 4);

            Assert.Equal((uint)65540, computer.CPU.REGS.ABC);

            computer.CPU.REGS.Set24BitRegister(0, 16777215);
            computer.CPU.REGS.AddTo24BitRegister(0, 4);

            Assert.Equal((uint)3, computer.CPU.REGS.ABC);
        }

        [Fact]
        public void CPU_REGS_Addition32bit()
        {
            using Computer computer = new();
            computer.CPU.REGS.Set32BitRegister(0, 65536);
            computer.CPU.REGS.AddTo32BitRegister(0, 4);

            Assert.Equal((uint)65540, computer.CPU.REGS.ABCD);

            computer.CPU.REGS.Set32BitRegister(0, 16777215);
            computer.CPU.REGS.AddTo32BitRegister(0, 4);

            Assert.Equal((uint)16777219, computer.CPU.REGS.ABCD);

            computer.CPU.REGS.Set32BitRegister(0, 0xFFFFFFFF);
            computer.CPU.REGS.AddTo32BitRegister(0, 4);

            Assert.Equal((uint)3, computer.CPU.REGS.ABCD);
        }

        // Subtraction
        [Fact]
        public void CPU_REGS_Subtraction8bit()
        {
            using Computer computer = new();
            computer.CPU.REGS.Set8BitRegister(0, 200);
            computer.CPU.REGS.SubtractFrom8BitRegister(0, 50);

            Assert.Equal(150, computer.CPU.REGS.A);

            computer.CPU.REGS.Set8BitRegister(0, 100);
            computer.CPU.REGS.SubtractFrom8BitRegister(0, 200);

            Assert.Equal(156, computer.CPU.REGS.A);

            computer.CPU.REGS.Set8BitRegister(0, 200);
            computer.CPU.REGS.SubtractFrom8BitRegister(0, 201);

            Assert.Equal(255, computer.CPU.REGS.A);
        }

        [Fact]
        public void CPU_REGS_Subtractions16bit()
        {
            using Computer computer = new();
            computer.CPU.REGS.Set16BitRegister(0, 1800);
            computer.CPU.REGS.SubtractFrom16BitRegister(0, 50);

            Assert.Equal(1750, computer.CPU.REGS.AB);

            computer.CPU.REGS.Set16BitRegister(0, 1800);
            computer.CPU.REGS.SubtractFrom16BitRegister(0, 1900);

            Assert.Equal(65436, computer.CPU.REGS.AB);

            computer.CPU.REGS.Set16BitRegister(0, 32768);
            computer.CPU.REGS.SubtractFrom16BitRegister(0, 32769);

            Assert.Equal(65535, computer.CPU.REGS.AB);
        }

        [Fact]
        public void CPU_REGS_Subtractions24bit()
        {
            using Computer computer = new();
            computer.CPU.REGS.Set24BitRegister(0, 3);
            computer.CPU.REGS.SubtractFrom24BitRegister(0, 4);

            Assert.Equal((uint)16777215, computer.CPU.REGS.ABC);

            computer.CPU.REGS.Set24BitRegister(0, 16777215);
            computer.CPU.REGS.SubtractFrom24BitRegister(0, 4);

            Assert.Equal((uint)16777211, computer.CPU.REGS.ABC);
        }

        [Fact]
        public void CPU_REGS_Subtractions32bit()
        {
            using Computer computer = new();
            computer.CPU.REGS.Set32BitRegister(0, 65536);
            computer.CPU.REGS.SubtractFrom32BitRegister(0, 4);

            Assert.Equal((uint)65532, computer.CPU.REGS.ABCD);

            computer.CPU.REGS.Set32BitRegister(0, 16777215);
            computer.CPU.REGS.SubtractFrom32BitRegister(0, 4);

            Assert.Equal((uint)16777211, computer.CPU.REGS.ABCD);

            computer.CPU.REGS.Set32BitRegister(0, 3);
            computer.CPU.REGS.SubtractFrom32BitRegister(0, 4);

            Assert.Equal(0xFFFFFFFF, computer.CPU.REGS.ABCD);
        }

        [Fact]
        public void CPU_REGS_GetNextRegister()
        {
            using Computer computer = new();

            Assert.Equal(
                Mnem.B,
                computer.CPU.REGS.GetNextRegister(Mnem.A, 1)
            );
            Assert.Equal(
                Mnem.C,
                computer.CPU.REGS.GetNextRegister(Mnem.A, 2)
            );
            Assert.Equal(
                Mnem.D,
                computer.CPU.REGS.GetNextRegister(Mnem.A, 3)
            );
            Assert.Equal(
                Mnem.E,
                computer.CPU.REGS.GetNextRegister(Mnem.A, 4)
            );
            Assert.Equal(
                Mnem.A,
                computer.CPU.REGS.GetNextRegister(Mnem.Z, 1)
            );
            Assert.Equal(
                Mnem.B,
                computer.CPU.REGS.GetNextRegister(Mnem.Z, 2)
            );
            Assert.Equal(
                Mnem.C,
                computer.CPU.REGS.GetNextRegister(Mnem.Z, 3)
            );
            Assert.Equal(
                Mnem.D,
                computer.CPU.REGS.GetNextRegister(Mnem.Z, 4)
            );
        }
    }
}
