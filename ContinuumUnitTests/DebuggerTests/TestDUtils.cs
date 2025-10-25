using Continuum93.CodeAnalysis;


namespace DebuggerTests
{

    public class TestDUtils
    {
        [Fact]
        public void TestByteToBinary()
        {
            Assert.Equal("00000000", DUtils.ByteToBinary(0b00000000));
            Assert.Equal("00000001", DUtils.ByteToBinary(0b00000001));
            Assert.Equal("00000010", DUtils.ByteToBinary(0b00000010));
            Assert.Equal("00000011", DUtils.ByteToBinary(0b00000011));
            Assert.Equal("00000100", DUtils.ByteToBinary(0b00000100));
            Assert.Equal("00000101", DUtils.ByteToBinary(0b00000101));
            Assert.Equal("00000110", DUtils.ByteToBinary(0b00000110));
            Assert.Equal("00000111", DUtils.ByteToBinary(0b00000111));
            Assert.Equal("00001000", DUtils.ByteToBinary(0b00001000));
            Assert.Equal("00001001", DUtils.ByteToBinary(0b00001001));
            Assert.Equal("00001010", DUtils.ByteToBinary(0b00001010));
            Assert.Equal("00001011", DUtils.ByteToBinary(0b00001011));
            Assert.Equal("00001100", DUtils.ByteToBinary(0b00001100));
            Assert.Equal("00001101", DUtils.ByteToBinary(0b00001101));
            Assert.Equal("00001110", DUtils.ByteToBinary(0b00001110));
            Assert.Equal("00001111", DUtils.ByteToBinary(0b00001111));
            Assert.Equal("00010000", DUtils.ByteToBinary(0b00010000));
            Assert.Equal("00010001", DUtils.ByteToBinary(0b00010001));
            Assert.Equal("00010010", DUtils.ByteToBinary(0b00010010));
            Assert.Equal("00010011", DUtils.ByteToBinary(0b00010011));
            Assert.Equal("00010100", DUtils.ByteToBinary(0b00010100));
            Assert.Equal("00010101", DUtils.ByteToBinary(0b00010101));
            Assert.Equal("00010110", DUtils.ByteToBinary(0b00010110));
            Assert.Equal("00010111", DUtils.ByteToBinary(0b00010111));
            Assert.Equal("00011000", DUtils.ByteToBinary(0b00011000));
            Assert.Equal("00011001", DUtils.ByteToBinary(0b00011001));
            Assert.Equal("00011010", DUtils.ByteToBinary(0b00011010));
            Assert.Equal("00011011", DUtils.ByteToBinary(0b00011011));
            Assert.Equal("00011100", DUtils.ByteToBinary(0b00011100));
            Assert.Equal("00011101", DUtils.ByteToBinary(0b00011101));
            Assert.Equal("00011110", DUtils.ByteToBinary(0b00011110));
            Assert.Equal("00011111", DUtils.ByteToBinary(0b00011111));
            Assert.Equal("00100000", DUtils.ByteToBinary(0b00100000));
            Assert.Equal("00100001", DUtils.ByteToBinary(0b00100001));
            Assert.Equal("00100010", DUtils.ByteToBinary(0b00100010));
            Assert.Equal("00100011", DUtils.ByteToBinary(0b00100011));
            Assert.Equal("00100100", DUtils.ByteToBinary(0b00100100));
            Assert.Equal("00100101", DUtils.ByteToBinary(0b00100101));
            Assert.Equal("00100110", DUtils.ByteToBinary(0b00100110));
            Assert.Equal("00100111", DUtils.ByteToBinary(0b00100111));
            Assert.Equal("00101000", DUtils.ByteToBinary(0b00101000));
            Assert.Equal("00101001", DUtils.ByteToBinary(0b00101001));
            Assert.Equal("00101010", DUtils.ByteToBinary(0b00101010));
            Assert.Equal("00101011", DUtils.ByteToBinary(0b00101011));
            Assert.Equal("00101100", DUtils.ByteToBinary(0b00101100));
            Assert.Equal("00101101", DUtils.ByteToBinary(0b00101101));
            Assert.Equal("00101110", DUtils.ByteToBinary(0b00101110));
            Assert.Equal("00101111", DUtils.ByteToBinary(0b00101111));
            Assert.Equal("00110000", DUtils.ByteToBinary(0b00110000));
            Assert.Equal("00110001", DUtils.ByteToBinary(0b00110001));
            Assert.Equal("00110010", DUtils.ByteToBinary(0b00110010));
            Assert.Equal("00110011", DUtils.ByteToBinary(0b00110011));
            Assert.Equal("00110100", DUtils.ByteToBinary(0b00110100));
            Assert.Equal("00110101", DUtils.ByteToBinary(0b00110101));
            Assert.Equal("00110110", DUtils.ByteToBinary(0b00110110));
            Assert.Equal("00110111", DUtils.ByteToBinary(0b00110111));
            Assert.Equal("00111000", DUtils.ByteToBinary(0b00111000));
            Assert.Equal("00111001", DUtils.ByteToBinary(0b00111001));
            Assert.Equal("00111010", DUtils.ByteToBinary(0b00111010));
            Assert.Equal("00111011", DUtils.ByteToBinary(0b00111011));
            Assert.Equal("00111100", DUtils.ByteToBinary(0b00111100));
            Assert.Equal("00111101", DUtils.ByteToBinary(0b00111101));
            Assert.Equal("00111110", DUtils.ByteToBinary(0b00111110));
            Assert.Equal("00111111", DUtils.ByteToBinary(0b00111111));
            Assert.Equal("01000000", DUtils.ByteToBinary(0b01000000));
            Assert.Equal("01000001", DUtils.ByteToBinary(0b01000001));
            Assert.Equal("01000010", DUtils.ByteToBinary(0b01000010));
            Assert.Equal("01000011", DUtils.ByteToBinary(0b01000011));
            Assert.Equal("01000100", DUtils.ByteToBinary(0b01000100));
            Assert.Equal("01000101", DUtils.ByteToBinary(0b01000101));
            Assert.Equal("01000110", DUtils.ByteToBinary(0b01000110));
            Assert.Equal("01000111", DUtils.ByteToBinary(0b01000111));
            Assert.Equal("01001000", DUtils.ByteToBinary(0b01001000));
            Assert.Equal("01001001", DUtils.ByteToBinary(0b01001001));
            Assert.Equal("01001010", DUtils.ByteToBinary(0b01001010));
            Assert.Equal("01001011", DUtils.ByteToBinary(0b01001011));
            Assert.Equal("01001100", DUtils.ByteToBinary(0b01001100));
            Assert.Equal("01001101", DUtils.ByteToBinary(0b01001101));
            Assert.Equal("01001110", DUtils.ByteToBinary(0b01001110));
            Assert.Equal("01001111", DUtils.ByteToBinary(0b01001111));
            Assert.Equal("01010000", DUtils.ByteToBinary(0b01010000));
            Assert.Equal("01010001", DUtils.ByteToBinary(0b01010001));
            Assert.Equal("01010010", DUtils.ByteToBinary(0b01010010));
            Assert.Equal("01010011", DUtils.ByteToBinary(0b01010011));
            Assert.Equal("01010100", DUtils.ByteToBinary(0b01010100));
            Assert.Equal("01010101", DUtils.ByteToBinary(0b01010101));
            Assert.Equal("01010110", DUtils.ByteToBinary(0b01010110));
            Assert.Equal("01010111", DUtils.ByteToBinary(0b01010111));
            Assert.Equal("01011000", DUtils.ByteToBinary(0b01011000));
            Assert.Equal("01011001", DUtils.ByteToBinary(0b01011001));
            Assert.Equal("01011010", DUtils.ByteToBinary(0b01011010));
            Assert.Equal("01011011", DUtils.ByteToBinary(0b01011011));
            Assert.Equal("01011100", DUtils.ByteToBinary(0b01011100));
            Assert.Equal("01011101", DUtils.ByteToBinary(0b01011101));
            Assert.Equal("01011110", DUtils.ByteToBinary(0b01011110));
            Assert.Equal("01011111", DUtils.ByteToBinary(0b01011111));
            Assert.Equal("01100000", DUtils.ByteToBinary(0b01100000));
            Assert.Equal("01100001", DUtils.ByteToBinary(0b01100001));
            Assert.Equal("01100010", DUtils.ByteToBinary(0b01100010));
            Assert.Equal("01100011", DUtils.ByteToBinary(0b01100011));
            Assert.Equal("01100100", DUtils.ByteToBinary(0b01100100));
            Assert.Equal("01100101", DUtils.ByteToBinary(0b01100101));
            Assert.Equal("01100110", DUtils.ByteToBinary(0b01100110));
            Assert.Equal("01100111", DUtils.ByteToBinary(0b01100111));
            Assert.Equal("01101000", DUtils.ByteToBinary(0b01101000));
            Assert.Equal("01101001", DUtils.ByteToBinary(0b01101001));
            Assert.Equal("01101010", DUtils.ByteToBinary(0b01101010));
            Assert.Equal("01101011", DUtils.ByteToBinary(0b01101011));
            Assert.Equal("01101100", DUtils.ByteToBinary(0b01101100));
            Assert.Equal("01101101", DUtils.ByteToBinary(0b01101101));
            Assert.Equal("01101110", DUtils.ByteToBinary(0b01101110));
            Assert.Equal("01101111", DUtils.ByteToBinary(0b01101111));
            Assert.Equal("01110000", DUtils.ByteToBinary(0b01110000));
            Assert.Equal("01110001", DUtils.ByteToBinary(0b01110001));
            Assert.Equal("01110010", DUtils.ByteToBinary(0b01110010));
            Assert.Equal("01110011", DUtils.ByteToBinary(0b01110011));
            Assert.Equal("01110100", DUtils.ByteToBinary(0b01110100));
            Assert.Equal("01110101", DUtils.ByteToBinary(0b01110101));
            Assert.Equal("01110110", DUtils.ByteToBinary(0b01110110));
            Assert.Equal("01110111", DUtils.ByteToBinary(0b01110111));
            Assert.Equal("01111000", DUtils.ByteToBinary(0b01111000));
            Assert.Equal("01111001", DUtils.ByteToBinary(0b01111001));
            Assert.Equal("01111010", DUtils.ByteToBinary(0b01111010));
            Assert.Equal("01111011", DUtils.ByteToBinary(0b01111011));
            Assert.Equal("01111100", DUtils.ByteToBinary(0b01111100));
            Assert.Equal("01111101", DUtils.ByteToBinary(0b01111101));
            Assert.Equal("01111110", DUtils.ByteToBinary(0b01111110));
            Assert.Equal("01111111", DUtils.ByteToBinary(0b01111111));
            Assert.Equal("10000000", DUtils.ByteToBinary(0b10000000));
            Assert.Equal("10000001", DUtils.ByteToBinary(0b10000001));
            Assert.Equal("10000010", DUtils.ByteToBinary(0b10000010));
            Assert.Equal("10000011", DUtils.ByteToBinary(0b10000011));
            Assert.Equal("10000100", DUtils.ByteToBinary(0b10000100));
            Assert.Equal("10000101", DUtils.ByteToBinary(0b10000101));
            Assert.Equal("10000110", DUtils.ByteToBinary(0b10000110));
            Assert.Equal("10000111", DUtils.ByteToBinary(0b10000111));
            Assert.Equal("10001000", DUtils.ByteToBinary(0b10001000));
            Assert.Equal("10001001", DUtils.ByteToBinary(0b10001001));
            Assert.Equal("10001010", DUtils.ByteToBinary(0b10001010));
            Assert.Equal("10001011", DUtils.ByteToBinary(0b10001011));
            Assert.Equal("10001100", DUtils.ByteToBinary(0b10001100));
            Assert.Equal("10001101", DUtils.ByteToBinary(0b10001101));
            Assert.Equal("10001110", DUtils.ByteToBinary(0b10001110));
            Assert.Equal("10001111", DUtils.ByteToBinary(0b10001111));
            Assert.Equal("10010000", DUtils.ByteToBinary(0b10010000));
            Assert.Equal("10010001", DUtils.ByteToBinary(0b10010001));
            Assert.Equal("10010010", DUtils.ByteToBinary(0b10010010));
            Assert.Equal("10010011", DUtils.ByteToBinary(0b10010011));
            Assert.Equal("10010100", DUtils.ByteToBinary(0b10010100));
            Assert.Equal("10010101", DUtils.ByteToBinary(0b10010101));
            Assert.Equal("10010110", DUtils.ByteToBinary(0b10010110));
            Assert.Equal("10010111", DUtils.ByteToBinary(0b10010111));
            Assert.Equal("10011000", DUtils.ByteToBinary(0b10011000));
            Assert.Equal("10011001", DUtils.ByteToBinary(0b10011001));
            Assert.Equal("10011010", DUtils.ByteToBinary(0b10011010));
            Assert.Equal("10011011", DUtils.ByteToBinary(0b10011011));
            Assert.Equal("10011100", DUtils.ByteToBinary(0b10011100));
            Assert.Equal("10011101", DUtils.ByteToBinary(0b10011101));
            Assert.Equal("10011110", DUtils.ByteToBinary(0b10011110));
            Assert.Equal("10011111", DUtils.ByteToBinary(0b10011111));
            Assert.Equal("10100000", DUtils.ByteToBinary(0b10100000));
            Assert.Equal("10100001", DUtils.ByteToBinary(0b10100001));
            Assert.Equal("10100010", DUtils.ByteToBinary(0b10100010));
            Assert.Equal("10100011", DUtils.ByteToBinary(0b10100011));
            Assert.Equal("10100100", DUtils.ByteToBinary(0b10100100));
            Assert.Equal("10100101", DUtils.ByteToBinary(0b10100101));
            Assert.Equal("10100110", DUtils.ByteToBinary(0b10100110));
            Assert.Equal("10100111", DUtils.ByteToBinary(0b10100111));
            Assert.Equal("10101000", DUtils.ByteToBinary(0b10101000));
            Assert.Equal("10101001", DUtils.ByteToBinary(0b10101001));
            Assert.Equal("10101010", DUtils.ByteToBinary(0b10101010));
            Assert.Equal("10101011", DUtils.ByteToBinary(0b10101011));
            Assert.Equal("10101100", DUtils.ByteToBinary(0b10101100));
            Assert.Equal("10101101", DUtils.ByteToBinary(0b10101101));
            Assert.Equal("10101110", DUtils.ByteToBinary(0b10101110));
            Assert.Equal("10101111", DUtils.ByteToBinary(0b10101111));
            Assert.Equal("10110000", DUtils.ByteToBinary(0b10110000));
            Assert.Equal("10110001", DUtils.ByteToBinary(0b10110001));
            Assert.Equal("10110010", DUtils.ByteToBinary(0b10110010));
            Assert.Equal("10110011", DUtils.ByteToBinary(0b10110011));
            Assert.Equal("10110100", DUtils.ByteToBinary(0b10110100));
            Assert.Equal("10110101", DUtils.ByteToBinary(0b10110101));
            Assert.Equal("10110110", DUtils.ByteToBinary(0b10110110));
            Assert.Equal("10110111", DUtils.ByteToBinary(0b10110111));
            Assert.Equal("10111000", DUtils.ByteToBinary(0b10111000));
            Assert.Equal("10111001", DUtils.ByteToBinary(0b10111001));
            Assert.Equal("10111010", DUtils.ByteToBinary(0b10111010));
            Assert.Equal("10111011", DUtils.ByteToBinary(0b10111011));
            Assert.Equal("10111100", DUtils.ByteToBinary(0b10111100));
            Assert.Equal("10111101", DUtils.ByteToBinary(0b10111101));
            Assert.Equal("10111110", DUtils.ByteToBinary(0b10111110));
            Assert.Equal("10111111", DUtils.ByteToBinary(0b10111111));
            Assert.Equal("11000000", DUtils.ByteToBinary(0b11000000));
            Assert.Equal("11000001", DUtils.ByteToBinary(0b11000001));
            Assert.Equal("11000010", DUtils.ByteToBinary(0b11000010));
            Assert.Equal("11000011", DUtils.ByteToBinary(0b11000011));
            Assert.Equal("11000100", DUtils.ByteToBinary(0b11000100));
            Assert.Equal("11000101", DUtils.ByteToBinary(0b11000101));
            Assert.Equal("11000110", DUtils.ByteToBinary(0b11000110));
            Assert.Equal("11000111", DUtils.ByteToBinary(0b11000111));
            Assert.Equal("11001000", DUtils.ByteToBinary(0b11001000));
            Assert.Equal("11001001", DUtils.ByteToBinary(0b11001001));
            Assert.Equal("11001010", DUtils.ByteToBinary(0b11001010));
            Assert.Equal("11001011", DUtils.ByteToBinary(0b11001011));
            Assert.Equal("11001100", DUtils.ByteToBinary(0b11001100));
            Assert.Equal("11001101", DUtils.ByteToBinary(0b11001101));
            Assert.Equal("11001110", DUtils.ByteToBinary(0b11001110));
            Assert.Equal("11001111", DUtils.ByteToBinary(0b11001111));
            Assert.Equal("11010000", DUtils.ByteToBinary(0b11010000));
            Assert.Equal("11010001", DUtils.ByteToBinary(0b11010001));
            Assert.Equal("11010010", DUtils.ByteToBinary(0b11010010));
            Assert.Equal("11010011", DUtils.ByteToBinary(0b11010011));
            Assert.Equal("11010100", DUtils.ByteToBinary(0b11010100));
            Assert.Equal("11010101", DUtils.ByteToBinary(0b11010101));
            Assert.Equal("11010110", DUtils.ByteToBinary(0b11010110));
            Assert.Equal("11010111", DUtils.ByteToBinary(0b11010111));
            Assert.Equal("11011000", DUtils.ByteToBinary(0b11011000));
            Assert.Equal("11011001", DUtils.ByteToBinary(0b11011001));
            Assert.Equal("11011010", DUtils.ByteToBinary(0b11011010));
            Assert.Equal("11011011", DUtils.ByteToBinary(0b11011011));
            Assert.Equal("11011100", DUtils.ByteToBinary(0b11011100));
            Assert.Equal("11011101", DUtils.ByteToBinary(0b11011101));
            Assert.Equal("11011110", DUtils.ByteToBinary(0b11011110));
            Assert.Equal("11011111", DUtils.ByteToBinary(0b11011111));
            Assert.Equal("11100000", DUtils.ByteToBinary(0b11100000));
            Assert.Equal("11100001", DUtils.ByteToBinary(0b11100001));
            Assert.Equal("11100010", DUtils.ByteToBinary(0b11100010));
            Assert.Equal("11100011", DUtils.ByteToBinary(0b11100011));
            Assert.Equal("11100100", DUtils.ByteToBinary(0b11100100));
            Assert.Equal("11100101", DUtils.ByteToBinary(0b11100101));
            Assert.Equal("11100110", DUtils.ByteToBinary(0b11100110));
            Assert.Equal("11100111", DUtils.ByteToBinary(0b11100111));
            Assert.Equal("11101000", DUtils.ByteToBinary(0b11101000));
            Assert.Equal("11101001", DUtils.ByteToBinary(0b11101001));
            Assert.Equal("11101010", DUtils.ByteToBinary(0b11101010));
            Assert.Equal("11101011", DUtils.ByteToBinary(0b11101011));
            Assert.Equal("11101100", DUtils.ByteToBinary(0b11101100));
            Assert.Equal("11101101", DUtils.ByteToBinary(0b11101101));
            Assert.Equal("11101110", DUtils.ByteToBinary(0b11101110));
            Assert.Equal("11101111", DUtils.ByteToBinary(0b11101111));
            Assert.Equal("11110000", DUtils.ByteToBinary(0b11110000));
            Assert.Equal("11110001", DUtils.ByteToBinary(0b11110001));
            Assert.Equal("11110010", DUtils.ByteToBinary(0b11110010));
            Assert.Equal("11110011", DUtils.ByteToBinary(0b11110011));
            Assert.Equal("11110100", DUtils.ByteToBinary(0b11110100));
            Assert.Equal("11110101", DUtils.ByteToBinary(0b11110101));
            Assert.Equal("11110110", DUtils.ByteToBinary(0b11110110));
            Assert.Equal("11110111", DUtils.ByteToBinary(0b11110111));
            Assert.Equal("11111000", DUtils.ByteToBinary(0b11111000));
            Assert.Equal("11111001", DUtils.ByteToBinary(0b11111001));
            Assert.Equal("11111010", DUtils.ByteToBinary(0b11111010));
            Assert.Equal("11111011", DUtils.ByteToBinary(0b11111011));
            Assert.Equal("11111100", DUtils.ByteToBinary(0b11111100));
            Assert.Equal("11111101", DUtils.ByteToBinary(0b11111101));
            Assert.Equal("11111110", DUtils.ByteToBinary(0b11111110));
            Assert.Equal("11111111", DUtils.ByteToBinary(0b11111111));
        }

        [Fact]
        public void TestGetArgumentsCount()
        {
            Assert.Equal(1, DUtils.GetArgumentsCount("ooooouuuuuAAAAAA"));
            Assert.Equal(2, DUtils.GetArgumentsCount("ooooouuuuuAAAAAABBBBBBBB"));
            Assert.Equal(3, DUtils.GetArgumentsCount("ooooouuuuuCCCCCC"));
        }

        [Fact]
        public void TestGetRegisterRepresentation()
        {
            for (char cr = 'A'; cr <= 'Z'; cr++)
                Assert.Equal("" + cr, DUtils.GetRegisterRepresentation("r", (byte)(cr - 'A')));

            Assert.Equal("AB", DUtils.GetRegisterRepresentation("rr", 0));
            Assert.Equal("BC", DUtils.GetRegisterRepresentation("rr", 1));
            Assert.Equal("ZA", DUtils.GetRegisterRepresentation("rr", 25));

            Assert.Equal("ABC", DUtils.GetRegisterRepresentation("rrr", 0));
            Assert.Equal("BCD", DUtils.GetRegisterRepresentation("rrr", 1));
            Assert.Equal("ZAB", DUtils.GetRegisterRepresentation("rrr", 25));

            Assert.Equal("ABCD", DUtils.GetRegisterRepresentation("rrrr", 0));
            Assert.Equal("BCDE", DUtils.GetRegisterRepresentation("rrrr", 1));
            Assert.Equal("ZABC", DUtils.GetRegisterRepresentation("rrrr", 25));

            Assert.Equal("(ABC)", DUtils.GetRegisterRepresentation("(rrr)", 0));
            Assert.Equal("(BCD)", DUtils.GetRegisterRepresentation("(rrr)", 1));
            Assert.Equal("(ZAB)", DUtils.GetRegisterRepresentation("(rrr)", 25));
        }

        [Fact]
        public void TestGetFailedRegisterRepresentation()
        {
            for (byte i = 26; i < 32; i++)
            {
                Assert.Equal("??", DUtils.GetRegisterRepresentation("rr", i));
                Assert.Equal("???", DUtils.GetRegisterRepresentation("rrr", i));
                Assert.Equal("????", DUtils.GetRegisterRepresentation("rrrr", i));
                Assert.Equal($"({i}?)", DUtils.GetRegisterRepresentation("(rrr)", i));
            }
        }

        [Fact]
        public void TestGetFloatRegisterRepresentation()
        {
            Assert.Equal("F0", DUtils.GetFloatRegisterRepresentation(0));
            Assert.Equal("F1", DUtils.GetFloatRegisterRepresentation(1));
            Assert.Equal("F2", DUtils.GetFloatRegisterRepresentation(2));
            Assert.Equal("F12", DUtils.GetFloatRegisterRepresentation(12));
        }

        [Fact]
        public void TestGetByteLengthOfBits()
        {
            Assert.Equal(1, DUtils.GetByteLengthOfBits(8));
            Assert.Equal(2, DUtils.GetByteLengthOfBits(10));
            Assert.Equal(2, DUtils.GetByteLengthOfBits(12));
            Assert.Equal(2, DUtils.GetByteLengthOfBits(16));
            Assert.Equal(3, DUtils.GetByteLengthOfBits(24));
            Assert.Equal(4, DUtils.GetByteLengthOfBits(32));
        }

        [Fact]
        public void TestGetUntilOrEmpty()
        {
            Assert.Equal("dCom2", DUtils.GetUntilOrWhole("dCom2: 123"));
            Assert.Equal("dCom3", DUtils.GetUntilOrWhole("dCom3: 123, 456"));
            Assert.Equal("dCom", DUtils.GetUntilOrWhole("dCom"));
            Assert.Equal("dCom", DUtils.GetUntilOrWhole("dCom "));
            Assert.Equal("dCom", DUtils.GetUntilOrWhole("dCom : "));
        }

        [Fact]
        public void TestGetTextParameters()
        {
            Assert.Equal(new string[] { "123" }, DUtils.GetTextParameters("dCom2: 123"));
            Assert.Equal(new string[] { "123", "456", "789", "" }, DUtils.GetTextParameters("commands: 123, 456  ,  789 ,"));
        }

        [Fact]
        public void TestGetIntParameters()
        {
            Assert.Equal(new int[] { 123 }, DUtils.GetIntParameters("dCom2: 123"));
            Assert.Equal(new int[] { 123, 100, 0 }, DUtils.GetIntParameters("dCom2: 123, 100, 0"));
        }

        [Theory]
        [InlineData("00000000", "+0x00")]
        [InlineData("00000001", "+0x01")]
        [InlineData("00000010", "+0x02")]
        [InlineData("01111110", "+0x7E")]
        [InlineData("01111111", "+0x7F")]
        [InlineData("11111111", "-0x01")]
        [InlineData("11111110", "-0x02")]
        [InlineData("10000000", "-0x80")]
        [InlineData("10000001", "-0x7F")]
        [InlineData("10000010", "-0x7E")]
        [InlineData("10000011", "-0x7D")]
        public void TestGetNumberRepresentationFor8Bit(string binaryString, string expectedHexRepresentation)
        {
            Assert.Equal(expectedHexRepresentation, DUtils.GetNumberRepresentation("nnn", new DArg(binaryString), false, 0, true));
        }

        [Theory]
        [InlineData("0000000000000000", "+0x0000")]
        [InlineData("0000000000000001", "+0x0001")]
        [InlineData("0000000011111111", "+0x00FF")]
        [InlineData("0111111111111100", "+0x7FFC")]
        [InlineData("0111111111111101", "+0x7FFD")]
        [InlineData("0111111111111110", "+0x7FFE")]
        [InlineData("0111111111111111", "+0x7FFF")]
        [InlineData("1000000000000000", "-0x8000")]
        [InlineData("1000000000000001", "-0x7FFF")]
        [InlineData("1000000000000010", "-0x7FFE")]
        [InlineData("1111111111111101", "-0x0003")]
        [InlineData("1111111111111110", "-0x0002")]
        [InlineData("1111111111111111", "-0x0001")]
        public void TestGetNumberRepresentationFor16Bit(string binaryString, string expectedHexRepresentation)
        {
            Assert.Equal(expectedHexRepresentation, DUtils.GetNumberRepresentation("nnn", new DArg(binaryString), false, 0, true));
        }

        [Theory]
        [InlineData("000000000000000000000000", "+0x000000")]
        [InlineData("000000000000000000000001", "+0x000001")]
        [InlineData("000000000000000011111111", "+0x0000FF")]
        [InlineData("011111111111111111111100", "+0x7FFFFC")]
        [InlineData("011111111111111111111101", "+0x7FFFFD")]
        [InlineData("011111111111111111111110", "+0x7FFFFE")]
        [InlineData("011111111111111111111111", "+0x7FFFFF")]
        [InlineData("100000000000000000000000", "-0x800000")]
        [InlineData("100000000000000000000001", "-0x7FFFFF")]
        [InlineData("100000000000000000000010", "-0x7FFFFE")]
        [InlineData("111111111111111111111101", "-0x000003")]
        [InlineData("111111111111111111111110", "-0x000002")]
        [InlineData("111111111111111111111111", "-0x000001")]
        public void TestGetNumberRepresentationFor24Bit(string binaryString, string expectedHexRepresentation)
        {
            Assert.Equal(expectedHexRepresentation, DUtils.GetNumberRepresentation("nnn", new DArg(binaryString), false, 0, true));
        }

        [Theory]
        [InlineData("00000000000000000000000000000000", "+0x00000000")]
        [InlineData("00000000000000000000000000000001", "+0x00000001")]
        [InlineData("00000000000000000000000011111111", "+0x000000FF")]
        [InlineData("01111111111111111111111111111100", "+0x7FFFFFFC")]
        [InlineData("01111111111111111111111111111101", "+0x7FFFFFFD")]
        [InlineData("01111111111111111111111111111110", "+0x7FFFFFFE")]
        [InlineData("01111111111111111111111111111111", "+0x7FFFFFFF")]
        [InlineData("10000000000000000000000000000000", "-0x80000000")]
        [InlineData("10000000000000000000000000000001", "-0x7FFFFFFF")]
        [InlineData("10000000000000000000000000000010", "-0x7FFFFFFE")]
        [InlineData("11111111111111111111111111111101", "-0x00000003")]
        [InlineData("11111111111111111111111111111110", "-0x00000002")]
        [InlineData("11111111111111111111111111111111", "-0x00000001")]
        public void TestGetNumberRepresentationFor32Bit(string binaryString, string expectedHexRepresentation)
        {
            Assert.Equal(expectedHexRepresentation, DUtils.GetNumberRepresentation("nnn", new DArg(binaryString), false, 0, true));
        }
    }
}
