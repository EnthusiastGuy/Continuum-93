using Continuum93.Emulator;

namespace Registers
{

    public class TestRegisters
    {
        [Fact]
        public void Test8BitRegisters()
        {
            using Computer computer = new();
            computer.CPU.REGS.Set8BitRegister(0, 0x80);
            Assert.Equal(0x80, computer.CPU.REGS.A);
        }

        [Fact]
        public void Test16BitComposition()
        {
            using Computer computer = new();
            computer.CPU.REGS.Set8BitRegister(0, 0xAB);
            computer.CPU.REGS.Set8BitRegister(1, 0xCD);
            Assert.Equal(0xABCD, computer.CPU.REGS.AB);

            computer.CPU.REGS.Set16BitRegister(0, 0x1234);
            Assert.Equal(0x12, computer.CPU.REGS.A);
            Assert.Equal(0x34, computer.CPU.REGS.B);
        }

        [Fact]
        public void Test16BitRegisterAddressing()
        {
            using Computer computer = new();
            computer.CPU.REGS.A = 0xAB;
            computer.CPU.REGS.B = 0xCD;
            Assert.Equal(0xABCD, computer.CPU.REGS.Get16BitRegister(0));
        }

        [Fact]
        public void Test24BitRegisterAddressing()
        {
            using Computer computer = new();
            computer.CPU.REGS.A = 0xAB;
            computer.CPU.REGS.B = 0xCD;
            computer.CPU.REGS.C = 0xEF;
            uint expected = 0xABCDEF;
            uint actual = computer.CPU.REGS.Get24BitRegister(0);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test24BitRegisterAddressingOverflow()
        {
            using Computer computer = new();
            computer.CPU.REGS.Z = 0xAB;
            computer.CPU.REGS.A = 0xCD;
            computer.CPU.REGS.B = 0xEF;
            uint expected = 0xABCDEF;
            uint actual = computer.CPU.REGS.Get24BitRegister(25);
            Assert.Equal(expected, actual);
            expected = 0x00ABCD;
            actual = computer.CPU.REGS.Get24BitRegister(24);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test24BitComposition()
        {
            using Computer computer = new();
            computer.CPU.REGS.ABC = 0x123456;
            Assert.Equal(0x12, computer.CPU.REGS.A);
            Assert.Equal(0x34, computer.CPU.REGS.B);
            Assert.Equal(0x56, computer.CPU.REGS.C);

            computer.CPU.REGS.A = 0xAB;
            computer.CPU.REGS.B = 0xCD;
            computer.CPU.REGS.C = 0xEF;
            uint expected = 0xABCDEF;
            uint actual = computer.CPU.REGS.ABC;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test8BitUnsignedToSignedAndBack()
        {
            using Computer computer = new();

            computer.CPU.REGS.Set8BitRegister(0, 0xFF);
            Assert.Equal(-1, computer.CPU.REGS.Get8BitRegisterSigned(0));

            computer.CPU.REGS.Set8BitRegister(0, 0x80);
            Assert.Equal(-128, computer.CPU.REGS.Get8BitRegisterSigned(0));

            computer.CPU.REGS.Set8BitRegister(0, 0x7F);
            Assert.Equal(127, computer.CPU.REGS.Get8BitRegisterSigned(0));

            computer.CPU.REGS.Set8BitRegisterSigned(0, -5);
            Assert.Equal(0xFB, computer.CPU.REGS.Get8BitRegister(0));
        }

        [Fact]
        public void Test16BitUnsignedToSignedAndBack()
        {
            using Computer computer = new();

            computer.CPU.REGS.Set16BitRegister(0, 0xFFFF);
            Assert.Equal(-1, computer.CPU.REGS.Get16BitRegisterSigned(0));

            computer.CPU.REGS.Set16BitRegisterSigned(0, -1);
            Assert.Equal(0xFFFF, computer.CPU.REGS.Get16BitRegister(0));

            computer.CPU.REGS.Set16BitRegister(0, 0x8000);
            Assert.Equal(-32768, computer.CPU.REGS.Get16BitRegisterSigned(0));

            computer.CPU.REGS.Set16BitRegister(0, 0x7FFF);
            Assert.Equal(32767, computer.CPU.REGS.Get16BitRegisterSigned(0));

            computer.CPU.REGS.Set16BitRegisterSigned(0, -32767);
            Assert.Equal(0x8001, computer.CPU.REGS.Get16BitRegister(0));
        }

        [Fact]
        public void Test24BitUnsignedToSignedAndBack()
        {
            using Computer computer = new();

            computer.CPU.REGS.Set24BitRegister(0, 0xFFFFFF);
            Assert.Equal(-1, computer.CPU.REGS.Get24BitRegisterSigned(0));

            computer.CPU.REGS.Set24BitRegisterSigned(0, -1);
            Assert.Equal((uint)0xFFFFFF, computer.CPU.REGS.Get24BitRegister(0));

            computer.CPU.REGS.Set24BitRegister(0, 0x800000);
            Assert.Equal(-8388608, computer.CPU.REGS.Get24BitRegisterSigned(0));

            computer.CPU.REGS.Set24BitRegister(0, 0x7FFFFF);
            Assert.Equal(8388607, computer.CPU.REGS.Get24BitRegisterSigned(0));

            computer.CPU.REGS.Set24BitRegisterSigned(0, -8388607);
            Assert.Equal((uint)0x800001, computer.CPU.REGS.Get24BitRegister(0));
        }

        [Fact]
        public void Test32BitUnsignedToSignedAndBack()
        {
            using Computer computer = new();

            computer.CPU.REGS.Set32BitRegister(0, 0xFFFFFFFF);
            Assert.Equal(-1, computer.CPU.REGS.Get32BitRegisterSigned(0));

            computer.CPU.REGS.Set32BitRegisterSigned(0, -1);
            Assert.Equal(0xFFFFFFFF, computer.CPU.REGS.Get32BitRegister(0));

            computer.CPU.REGS.Set32BitRegister(0, 0x80000000);
            Assert.Equal(-2147483648, computer.CPU.REGS.Get32BitRegisterSigned(0));

            computer.CPU.REGS.Set32BitRegister(0, 0x7FFFFFFF);
            Assert.Equal(2147483647, computer.CPU.REGS.Get32BitRegisterSigned(0));

            computer.CPU.REGS.Set32BitRegisterSigned(0, -2147483647);
            Assert.Equal(0x80000001, computer.CPU.REGS.Get32BitRegister(0));
        }
    }
}
