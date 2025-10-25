using Continuum93.Emulator;
using Continuum93.Emulator;

namespace ContinuumMachine
{
    public class MachineTests
    {
        readonly byte[] filledArray1 = new byte[] {
            0x07, 0xc3, 0x7d, 0x09, 0x28, 0xed, 0xb1, 0xff, 0x9d, 0xa0,
            0xf0, 0x74, 0xa2, 0xb8, 0x42, 0x5e, 0xf2, 0x70, 0x5a, 0xf8,
            0xb2, 0xe3, 0x6f, 0x1b, 0x2c, 0x62, 0x60, 0x4a, 0xb1, 0x7a,
            0x75, 0xd5, 0x88, 0x40, 0xdd, 0x7d, 0x1a, 0x84, 0x39, 0x66,
            0x16, 0x11, 0x10, 0x76, 0xe0, 0x7d, 0x66, 0xed, 0x17, 0x20
        };

        readonly byte[] filledArray2 = new byte[] {
            0x3c, 0x5c, 0x26, 0x37, 0x59, 0xf9, 0x48, 0x71, 0x35, 0x41,
        };

        readonly byte[] filledArray3 = new byte[] {
            0x1c, 0x6a, 0x0c, 0x83, 0x74, 0x91, 0xa5, 0x61, 0x18, 0x13,
            0xb5, 0x51, 0xf0, 0x0a, 0xd7, 0x19, 0x89, 0x20, 0x2f, 0x69,
        };

        [Fact]
        public void Test_LoadMem()
        {
            using Computer computer = new();

            int length = 50;
            byte[] expectedEmpty = new byte[length];
            byte[] actual = new byte[length];

            Array.Copy(computer.MEMC.RAM.Data, 0, actual, 0, length);
            Assert.Equal(expectedEmpty, actual);

            computer.LoadMem(filledArray1);

            Array.Copy(computer.MEMC.RAM.Data, 0, actual, 0, length);
            Assert.Equal(filledArray1, actual);
        }

        [Fact]
        public void Test_LoadMemAt_Single()
        {
            using Computer computer = new();

            int length = 50;
            byte[] expectedEmpty = new byte[length];
            byte[] actual = new byte[length];

            Array.Copy(computer.MEMC.RAM.Data, 1000, actual, 0, length);
            Assert.Equal(expectedEmpty, actual);

            computer.LoadMemAt(1000, filledArray1);

            Array.Copy(computer.MEMC.RAM.Data, 1000, actual, 0, length);
            Assert.Equal(filledArray1, actual);
        }

        [Fact]
        public void Test_LoadMemAt_Multiple_two()
        {
            using Computer computer = new();

            int length = 60;
            byte[] expectedEmpty = new byte[length];
            byte[] actual = new byte[length];

            Array.Copy(computer.MEMC.RAM.Data, 1000, actual, 0, length);
            Assert.Equal(expectedEmpty, actual);

            computer.LoadMemAt(1000, filledArray1, filledArray2);

            Array.Copy(computer.MEMC.RAM.Data, 1000, actual, 0, length);
            Assert.Equal(DataConverter.MergeByteArrays(filledArray1, filledArray2), actual);
        }

        [Fact]
        public void Test_LoadMemAt_Multiple_three()
        {
            using Computer computer = new();

            int length = 80;
            byte[] expectedEmpty = new byte[length];
            byte[] actual = new byte[length];

            Array.Copy(computer.MEMC.RAM.Data, 1000, actual, 0, length);
            Assert.Equal(expectedEmpty, actual);

            computer.LoadMemAt(1000, filledArray1, filledArray2, filledArray3);

            Array.Copy(computer.MEMC.RAM.Data, 1000, actual, 0, length);
            Assert.Equal(DataConverter.MergeByteArrays(filledArray1, filledArray2, filledArray3), actual);
        }

        [Fact]
        public void Test_FillMemoryAt()
        {
            using Computer computer = new();

            int length = 10;
            byte[] expectedEmpty = new byte[length + 2];
            byte[] actual = new byte[length + 2];

            Array.Copy(computer.MEMC.RAM.Data, 999, actual, 0, length + 2);
            Assert.Equal(expectedEmpty, actual);

            computer.FillMemoryAt(1000, length, 255);

            Array.Copy(computer.MEMC.RAM.Data, 999, actual, 0, length + 2);
            Assert.Equal(new byte[] { 0, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 0 }, actual);
        }

        [Fact]
        public void Test_GetMemFrom()
        {
            using Computer computer = new();

            uint length = 10;

            computer.FillMemoryAt(1000, (int)length, 255);
            byte[] actual = computer.GetMemFrom(1000, length);

            Assert.Equal(new byte[] { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 }, actual);
        }

        [Theory]
        [InlineData(0, 8, 0b11010010)]      // Expect to read the entire first byte
        [InlineData(3, 6, 0b100101)]        // Expect bits [3..8) => 0b100101
        [InlineData(11, 8, 0b10111010)]     // Expect bits [11..18) => 0b10111010
        [InlineData(27, 3, 0b011)]          // Expect bits [27..29) => 0b011
        public void Test_Get8BitValueFromBitMemoryAt(uint bitAddress, byte bits, byte expected)
        {
            using Computer computer = new();
            computer.LoadMemAt(0, new byte[] { 0b11010010, 0b11010111, 0b01010101, 0b00001111 });

            byte actual = computer.MEMC.RAM.Get8BitValueFromBitMemoryAt(bitAddress, bits);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0, 16, 0b1101001011010111)]      // Expect to read the entire first two bytes
        [InlineData(7, 12, 0b011010111010)]
        [InlineData(18, 11, 0b01010100001)]
        public void Test_Get16BitValueFromBitMemoryAt(uint bitAddress, byte bits, ushort expected)
        {
            using Computer computer = new();
            computer.LoadMemAt(0, new byte[] { 0b11010010, 0b11010111, 0b01010101, 0b00001111 });

            ushort actual = computer.MEMC.RAM.Get16BitValueFromBitMemoryAt(bitAddress, bits);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0, 24, 0b110100101101011101010101)]      // Expect to read the entire first three bytes
        [InlineData(2, 20, 0b01001011010111010101)]
        [InlineData(4, 23, 0b00101101011101010101010)]
        public void Test_Get24BitValueFromBitMemoryAt(uint bitAddress, byte bits, uint expected)
        {
            using Computer computer = new();
            computer.LoadMemAt(0, new byte[] { 0b11010010, 0b11010111, 0b01010101, 0b01001111 });

            uint actual = computer.MEMC.RAM.Get24BitValueFromBitMemoryAt(bitAddress, bits);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0, 32, 0b11010010110101110101010101001111)]      // Expect to read the entire first four bytes
        [InlineData(4, 30, 0b001011010111010101010100111101)]
        public void Test_Get32BitValueFromBitMemoryAt(uint bitAddress, byte bits, uint expected)
        {
            using Computer computer = new();
            computer.LoadMemAt(0, new byte[] { 0b11010010, 0b11010111, 0b01010101, 0b01001111, 0b01100010 });

            uint actual = computer.MEMC.RAM.Get32BitValueFromBitMemoryAt(bitAddress, bits);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0b01101010, 0, 8, new byte[]{ 0b01101010, 0 })]
        [InlineData(0b01101010, 4, 8, new byte[] { 0b00000110, 0b10100000 })]
        [InlineData(0b01101010, 4, 3, new byte[] { 0b00000100, 0})]
        public void Test_Set8BitValueFromBitMemoryAt(byte value, uint bitAddress, byte bits, byte[] expected)
        {
            using Computer computer = new();
            computer.MEMC.RAM.Clear();
            computer.MEMC.RAM.Set8BitValueToBitMemoryAt(value, bitAddress, bits);
            byte[] actualMem = computer.MEMC.RAM.GetMemoryAt(0, 2);

            Assert.Equal(expected, actualMem);
        }

        [Theory]
        [InlineData(0b0110101011110110, 0, 16, new byte[] { 0b01101010, 0b11110110, 0, 0 })]
        [InlineData(0b0110101011110110, 4, 10, new byte[] { 0b00001011, 0b11011000, 0, 0 })]
        [InlineData(0b0110101011110110, 6, 12, new byte[] { 0b00000010, 0b10111101, 0b10000000, 0 })]
        public void Test_Set16BitValueFromBitMemoryAt(ushort value, uint bitAddress, byte bits, byte[] expected)
        {
            using Computer computer = new();
            computer.MEMC.RAM.Clear();
            computer.MEMC.RAM.Set16BitValueToBitMemoryAt(value, bitAddress, bits);
            byte[] actualMem = computer.MEMC.RAM.GetMemoryAt(0, 4);

            Assert.Equal(expected, actualMem);
        }

        [Theory]
        [InlineData(0b011010101111011001010101, 0, 24, new byte[] { 0b01101010, 0b11110110, 0b01010101, 0, 0 })]
        [InlineData(0b011010101111011001010101, 4, 20, new byte[] { 0b00001010, 0b11110110, 0b01010101, 0, 0 })]
        [InlineData(0b011010101111011001010101, 6, 22, new byte[] { 0b00000010, 0b10101111, 0b01100101, 0b01010000, 0 })]
        public void Test_Set24BitValueFromBitMemoryAt(uint value, uint bitAddress, byte bits, byte[] expected)
        {
            using Computer computer = new();
            computer.MEMC.RAM.Clear();
            computer.MEMC.RAM.Set24BitValueToBitMemoryAt(value, bitAddress, bits);
            byte[] actualMem = computer.MEMC.RAM.GetMemoryAt(0, 5);

            Assert.Equal(expected, actualMem);
        }

        [Theory]
        [InlineData(0b01101010111101100101010111001011, 0, 32, new byte[] { 0b01101010, 0b11110110, 0b01010101, 0b11001011, 0 })]
        [InlineData(0b01101010111101100101010111001011, 6, 30, new byte[] { 0b00000010, 0b10101111, 0b01100101, 0b01011100, 0b10110000 })]
        public void Test_Set32BitValueFromBitMemoryAt(uint value, uint bitAddress, byte bits, byte[] expected)
        {
            using Computer computer = new();
            computer.MEMC.RAM.Clear();
            computer.MEMC.RAM.Set32BitValueToBitMemoryAt(value, bitAddress, bits);
            byte[] actualMem = computer.MEMC.RAM.GetMemoryAt(0, 5);

            Assert.Equal(expected, actualMem);
        }
    }
}
