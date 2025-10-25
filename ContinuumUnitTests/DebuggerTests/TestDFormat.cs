using Continuum93.CodeAnalysis;


namespace DebuggerTests
{

    public class TestDFormat
    {
        [Fact]
        public void TestGetFormattedByte()
        {
            // Decimal
            Assert.Equal("00000", DFormat.GetFormattedByte(0, 5));
            Assert.Equal("0100", DFormat.GetFormattedByte(100, 4));
            Assert.Equal("166", DFormat.GetFormattedByte(166, 1));

            // Hex
            Assert.Equal("0x00", DFormat.GetFormattedByte(0, 2, FormatType.Hex));
            Assert.Equal("0x0020", DFormat.GetFormattedByte(32, 4, FormatType.Hex));
            Assert.Equal("0x000000FF", DFormat.GetFormattedByte(255, 8, FormatType.Hex));

            // Binary
            Assert.Equal("0b00000000", DFormat.GetFormattedByte(0, 8, FormatType.Binary));
            Assert.Equal("0b01110010", DFormat.GetFormattedByte(114, 8, FormatType.Binary));
            Assert.Equal("0b11111111", DFormat.GetFormattedByte(255, 8, FormatType.Binary));

            // Octal
            Assert.Equal("0o000", DFormat.GetFormattedByte(0, 3, FormatType.Octal));
            Assert.Equal("0o0000177", DFormat.GetFormattedByte(127, 7, FormatType.Octal));
            Assert.Equal("0o377", DFormat.GetFormattedByte(255, 3, FormatType.Octal));
        }

        [Fact]
        public void TestGetFormattedValue()
        {
            // Decimal
            Assert.Equal("004294967295", DFormat.GetFormattedValue(0xFFFFFFFF, 12));

            // Hex
            Assert.Equal("0x00FFFFFFFF", DFormat.GetFormattedValue(0xFFFFFFFF, 10, FormatType.Hex));

            // Binary
            Assert.Equal("0b00000011111111111111111111111111111111", DFormat.GetFormattedValue(0xFFFFFFFF, 38, FormatType.Binary));

            // Octal
            Assert.Equal("0o00000037777777777", DFormat.GetFormattedValue(0xFFFFFFFF, 17, FormatType.Octal));
        }
    }
}
