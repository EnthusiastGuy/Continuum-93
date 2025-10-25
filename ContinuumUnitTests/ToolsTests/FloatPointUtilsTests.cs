using Continuum93.Tools;

namespace ToolsTests
{

    public class FloatPointUtilsTests
    {
        [Fact]
        public void TestFloatStringToBytes()
        {
            byte[] actual = FloatPointUtils.FloatStringToBytes("3.14159265359");
            Assert.Equal(new byte[] { 0x40, 0x49, 0x0F, 0xDB }, actual);
        }

        [Fact]
        public void TestFloatToUint()
        {
            uint actual = FloatPointUtils.FloatToUint(3.14159265359f);
            Assert.Equal((uint)0x40490FDB, actual);
        }

        [Fact]
        public void TestUintToFloat()
        {
            float actual = FloatPointUtils.UintToFloat(0x40490FDB);
            Assert.Equal(3.14159265359f, actual);
        }

        [Fact]
        public void TestFloatToBinary()
        {
            string actual = FloatPointUtils.FloatToBinary(3.1415926535897931f);
            Assert.Equal("01000000010010010000111111011011", actual);
        }

        [Fact]
        public void TestBytesToBinary()
        {
            byte[] bytes = new byte[] { 0xF0, 0xAA, 0xCC, 0xFF };
            string actual = FloatPointUtils.BytesToBinary(bytes);

            Assert.Equal("11110000101010101100110011111111", actual);
        }

        [Fact]
        public void TestBytesToFloat()
        {
            byte[] bytes = new byte[] { 64, 73, 15, 219 };
            float actual = FloatPointUtils.BytesToFloat(bytes);

            Assert.Equal((float)Math.PI, actual);
        }
    }
}
