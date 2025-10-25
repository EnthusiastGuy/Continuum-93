using Continuum93.Emulator;

namespace Registers
{
    public class TestRegistersBlockOperations
    {
        [Fact]
        public void TestSetRegistersBetween()
        {
            using Computer computer = new();

            computer.CPU.REGS.SetRegistersBetween(0, 0, new byte[] { 0xFF });
            Assert.Equal(0xFF, computer.CPU.REGS.A);

            computer.CPU.REGS.SetRegistersBetween(1, 1, new byte[] { 0xAD });
            Assert.Equal(0xAD, computer.CPU.REGS.B);

            computer.CPU.REGS.SetRegistersBetween(0, 1, new byte[] { 0x0E, 0x0D });
            Assert.Equal(0x0E0D, computer.CPU.REGS.AB);

            computer.CPU.REGS.SetRegistersBetween(1, 0, new byte[] { 0x0E, 0x0D });
            Assert.Equal(0x0D0E, computer.CPU.REGS.AB);

            computer.CPU.REGS.SetRegistersBetween(6, 12, new byte[] { 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F, 0x20 });
            Assert.Equal(0x1A1B1C1D, (long)computer.CPU.REGS.GHIJ);
            Assert.Equal(0x1E1F20, (long)computer.CPU.REGS.KLM);

            computer.CPU.REGS.SetRegistersBetween(12, 6, new byte[] { 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F, 0x20 });
            Assert.Equal(0x201F1E1D, (long)computer.CPU.REGS.GHIJ);
            Assert.Equal(0x1C1B1A, (long)computer.CPU.REGS.KLM);
        }

        [Fact]
        public void TestGetRegistersBetween()
        {
            using Computer computer = new();

            computer.CPU.REGS.ABCD = 0x01020304;
            computer.CPU.REGS.EFGH = 0x05060708;
            computer.CPU.REGS.IJKL = 0x090A0B0C;
            computer.CPU.REGS.MNOP = 0x0D0E0F10;
            computer.CPU.REGS.QRST = 0x11121314;
            computer.CPU.REGS.UVWX = 0x15161718;
            computer.CPU.REGS.YZ = 0x191A;

            byte[] actual = computer.CPU.REGS.GetRegistersBetween(0, 0);
            Assert.Equal(new byte[] { 0x01 }, actual);

            actual = computer.CPU.REGS.GetRegistersBetween(0, 1);
            Assert.Equal(new byte[] { 0x01, 0x02 }, actual);

            actual = computer.CPU.REGS.GetRegistersBetween(1, 1);
            Assert.Equal(new byte[] { 0x02 }, actual);

            actual = computer.CPU.REGS.GetRegistersBetween(1, 0);
            Assert.Equal(new byte[] { 0x02, 0x01 }, actual);

            actual = computer.CPU.REGS.GetRegistersBetween(6, 12);
            Assert.Equal(new byte[] { 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D }, actual);

            actual = computer.CPU.REGS.GetRegistersBetween(12, 6);
            Assert.Equal(new byte[] { 0x0D, 0x0C, 0x0B, 0x0A, 0x09, 0x08, 0x07 }, actual);

            actual = computer.CPU.REGS.GetRegistersBetween(0, 25);
            Assert.Equal(
                new byte[] {
                    0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07,
                    0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E,
                    0x0F, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15,
                    0x16, 0x17, 0x18, 0x19, 0x1A
                }, actual);
        }
    }
}
