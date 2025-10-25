using Continuum93.Utils;


namespace ToolsTests
{

    public class FontConvertorTests
    {
        [Fact]
        public void TestReverseByteBits()
        {
            Assert.Equal(0b00000000, FontConvertor.ReverseByteBits(0b00000000));
            Assert.Equal(0b11111111, FontConvertor.ReverseByteBits(0b11111111));
            Assert.Equal(0b10111111, FontConvertor.ReverseByteBits(0b11111101));
            Assert.Equal(0b10000000, FontConvertor.ReverseByteBits(0b00000001));
            Assert.Equal(0b11110000, FontConvertor.ReverseByteBits(0b00001111));
            Assert.Equal(0b10101010, FontConvertor.ReverseByteBits(0b01010101));
            Assert.Equal(0b11010000, FontConvertor.ReverseByteBits(0b00001011));
            Assert.Equal(0b00000110, FontConvertor.ReverseByteBits(0b01100000));
            Assert.Equal(0b00000100, FontConvertor.ReverseByteBits(0b00100000));

        }
    }
}
