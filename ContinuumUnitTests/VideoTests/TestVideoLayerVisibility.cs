using Continuum93.Emulator;

namespace VideoTests
{
    public class TestVideoLayerVisibility
    {
        [Theory]
        [InlineData(8, 0b00000000, new bool[] { false, false, false, false, false, false, false, false }, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0b00000000)]
        [InlineData(8, 0b10000001, new bool[] { true, false, false, false, false, false, false, true }, new byte[] { 0, 7, 0, 0, 0, 0, 0, 0 }, 0b10000001)]
        [InlineData(8, 0b00111100, new bool[] { false, false, true, true, true, true, false, false }, new byte[] { 2, 3, 4, 5, 0, 0, 0, 0 }, 0b00111100)]
        [InlineData(8, 0b10111100, new bool[] { false, false, true, true, true, true, false, true }, new byte[] { 2, 3, 4, 5, 7, 0, 0, 0 }, 0b10111100)]
        [InlineData(8, 0b11100000, new bool[] { false, false, false, false, false, true, true, true }, new byte[] { 5, 6, 7, 0, 0, 0, 0, 0 }, 0b11100000)]
        [InlineData(8, 0b11111111, new bool[] { true, true, true, true, true, true, true, true }, new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 }, 0b11111111)]
        public void TestSetLayerVisibility_8Pages(byte vramPages, byte values, bool[] expectedLayerVisible, byte[] expectedVisibleLayers, byte expectedLayerVisibleBits)
        {
            using Computer computer = new();
            computer.GRAPHICS.VRAM_PAGES = vramPages;
            computer.GRAPHICS.SetLayerVisibility(values);

            Assert.Equal(expectedLayerVisible, computer.GRAPHICS.LAYER_VISIBLE);
            Assert.Equal(expectedVisibleLayers, computer.GRAPHICS.GetVisibleLayers());
            Assert.Equal(expectedLayerVisibleBits, computer.GRAPHICS.LAYER_VISIBLE_BITS);
        }

        [Theory]
        [InlineData(7, 0b00000000, new bool[] { false, false, false, false, false, false, false, false }, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0b00000000)]
        [InlineData(7, 0b10000001, new bool[] { true, false, false, false, false, false, false, true }, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0b10000001)]
        [InlineData(7, 0b00111100, new bool[] { false, false, true, true, true, true, false, false }, new byte[] { 2, 3, 4, 5, 0, 0, 0, 0 }, 0b00111100)]
        [InlineData(7, 0b10111100, new bool[] { false, false, true, true, true, true, false, true }, new byte[] { 2, 3, 4, 5, 0, 0, 0, 0 }, 0b10111100)]
        [InlineData(7, 0b11100000, new bool[] { false, false, false, false, false, true, true, true }, new byte[] { 5, 6, 0, 0, 0, 0, 0, 0 }, 0b11100000)]
        [InlineData(7, 0b11111111, new bool[] { true, true, true, true, true, true, true, true }, new byte[] { 0, 1, 2, 3, 4, 5, 6, 0 }, 0b11111111)]
        public void TestSetLayerVisibility_7Pages(byte vramPages, byte values, bool[] expectedLayerVisible, byte[] expectedVisibleLayers, byte expectedLayerVisibleBits)
        {
            using Computer computer = new();
            computer.GRAPHICS.VRAM_PAGES = vramPages;
            computer.GRAPHICS.SetLayerVisibility(values);

            Assert.Equal(expectedLayerVisible, computer.GRAPHICS.LAYER_VISIBLE);
            Assert.Equal(expectedVisibleLayers, computer.GRAPHICS.GetVisibleLayers());
            Assert.Equal(expectedLayerVisibleBits, computer.GRAPHICS.LAYER_VISIBLE_BITS);
        }

        [Theory]
        [InlineData(6, 0b00000000, new bool[] { false, false, false, false, false, false, false, false }, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0b00000000)]
        [InlineData(6, 0b10000001, new bool[] { true, false, false, false, false, false, false, true }, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0b10000001)]
        [InlineData(6, 0b00111100, new bool[] { false, false, true, true, true, true, false, false }, new byte[] { 2, 3, 4, 5, 0, 0, 0, 0 }, 0b00111100)]
        [InlineData(6, 0b10111100, new bool[] { false, false, true, true, true, true, false, true }, new byte[] { 2, 3, 4, 5, 0, 0, 0, 0 }, 0b10111100)]
        [InlineData(6, 0b11100000, new bool[] { false, false, false, false, false, true, true, true }, new byte[] { 5, 0, 0, 0, 0, 0, 0, 0 }, 0b11100000)]
        [InlineData(6, 0b11111111, new bool[] { true, true, true, true, true, true, true, true }, new byte[] { 0, 1, 2, 3, 4, 5, 0, 0 }, 0b11111111)]
        public void TestSetLayerVisibility_6Pages(byte vramPages, byte values, bool[] expectedLayerVisible, byte[] expectedVisibleLayers, byte expectedLayerVisibleBits)
        {
            using Computer computer = new();
            computer.GRAPHICS.VRAM_PAGES = vramPages;
            computer.GRAPHICS.SetLayerVisibility(values);

            Assert.Equal(expectedLayerVisible, computer.GRAPHICS.LAYER_VISIBLE);
            Assert.Equal(expectedVisibleLayers, computer.GRAPHICS.GetVisibleLayers());
            Assert.Equal(expectedLayerVisibleBits, computer.GRAPHICS.LAYER_VISIBLE_BITS);
        }

        [Theory]
        [InlineData(5, 0b00000000, new bool[] { false, false, false, false, false, false, false, false }, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0b00000000)]
        [InlineData(5, 0b10000001, new bool[] { true, false, false, false, false, false, false, true }, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0b10000001)]
        [InlineData(5, 0b00111100, new bool[] { false, false, true, true, true, true, false, false }, new byte[] { 2, 3, 4, 0, 0, 0, 0, 0 }, 0b00111100)]
        [InlineData(5, 0b10111100, new bool[] { false, false, true, true, true, true, false, true }, new byte[] { 2, 3, 4, 0, 0, 0, 0, 0 }, 0b10111100)]
        [InlineData(5, 0b11100000, new bool[] { false, false, false, false, false, true, true, true }, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0b11100000)]
        [InlineData(5, 0b11111111, new bool[] { true, true, true, true, true, true, true, true }, new byte[] { 0, 1, 2, 3, 4, 0, 0, 0 }, 0b11111111)]
        public void TestSetLayerVisibility_5Pages(byte vramPages, byte values, bool[] expectedLayerVisible, byte[] expectedVisibleLayers, byte expectedLayerVisibleBits)
        {
            using Computer computer = new();
            computer.GRAPHICS.VRAM_PAGES = vramPages;
            computer.GRAPHICS.SetLayerVisibility(values);

            Assert.Equal(expectedLayerVisible, computer.GRAPHICS.LAYER_VISIBLE);
            Assert.Equal(expectedVisibleLayers, computer.GRAPHICS.GetVisibleLayers());
            Assert.Equal(expectedLayerVisibleBits, computer.GRAPHICS.LAYER_VISIBLE_BITS);
        }

        [Theory]
        [InlineData(4, 0b00000000, new bool[] { false, false, false, false, false, false, false, false }, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0b00000000)]
        [InlineData(4, 0b10000001, new bool[] { true, false, false, false, false, false, false, true }, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0b10000001)]
        [InlineData(4, 0b00111100, new bool[] { false, false, true, true, true, true, false, false }, new byte[] { 2, 3, 0, 0, 0, 0, 0, 0 }, 0b00111100)]
        [InlineData(4, 0b10111100, new bool[] { false, false, true, true, true, true, false, true }, new byte[] { 2, 3, 0, 0, 0, 0, 0, 0 }, 0b10111100)]
        [InlineData(4, 0b11100000, new bool[] { false, false, false, false, false, true, true, true }, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0b11100000)]
        [InlineData(4, 0b11111111, new bool[] { true, true, true, true, true, true, true, true }, new byte[] { 0, 1, 2, 3, 0, 0, 0, 0 }, 0b11111111)]
        public void TestSetLayerVisibility_4Pages(byte vramPages, byte values, bool[] expectedLayerVisible, byte[] expectedVisibleLayers, byte expectedLayerVisibleBits)
        {
            using Computer computer = new();
            computer.GRAPHICS.VRAM_PAGES = vramPages;
            computer.GRAPHICS.SetLayerVisibility(values);

            Assert.Equal(expectedLayerVisible, computer.GRAPHICS.LAYER_VISIBLE);
            Assert.Equal(expectedVisibleLayers, computer.GRAPHICS.GetVisibleLayers());
            Assert.Equal(expectedLayerVisibleBits, computer.GRAPHICS.LAYER_VISIBLE_BITS);
        }

        [Theory]
        [InlineData(3, 0b00000000, new bool[] { false, false, false, false, false, false, false, false }, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0b00000000)]
        [InlineData(3, 0b10000001, new bool[] { true, false, false, false, false, false, false, true }, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0b10000001)]
        [InlineData(3, 0b00111100, new bool[] { false, false, true, true, true, true, false, false }, new byte[] { 2, 0, 0, 0, 0, 0, 0, 0 }, 0b00111100)]
        [InlineData(3, 0b10111100, new bool[] { false, false, true, true, true, true, false, true }, new byte[] { 2, 0, 0, 0, 0, 0, 0, 0 }, 0b10111100)]
        [InlineData(3, 0b11100000, new bool[] { false, false, false, false, false, true, true, true }, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0b11100000)]
        [InlineData(3, 0b11111111, new bool[] { true, true, true, true, true, true, true, true }, new byte[] { 0, 1, 2, 0, 0, 0, 0, 0 }, 0b11111111)]
        public void TestSetLayerVisibility_3Pages(byte vramPages, byte values, bool[] expectedLayerVisible, byte[] expectedVisibleLayers, byte expectedLayerVisibleBits)
        {
            using Computer computer = new();
            computer.GRAPHICS.VRAM_PAGES = vramPages;
            computer.GRAPHICS.SetLayerVisibility(values);

            Assert.Equal(expectedLayerVisible, computer.GRAPHICS.LAYER_VISIBLE);
            Assert.Equal(expectedVisibleLayers, computer.GRAPHICS.GetVisibleLayers());
            Assert.Equal(expectedLayerVisibleBits, computer.GRAPHICS.LAYER_VISIBLE_BITS);
        }

        [Theory]
        [InlineData(2, 0b00000000, new bool[] { false, false, false, false, false, false, false, false }, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0b00000000)]
        [InlineData(2, 0b10000001, new bool[] { true, false, false, false, false, false, false, true }, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0b10000001)]
        [InlineData(2, 0b00111100, new bool[] { false, false, true, true, true, true, false, false }, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0b00111100)]
        [InlineData(2, 0b10111100, new bool[] { false, false, true, true, true, true, false, true }, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0b10111100)]
        [InlineData(2, 0b11100000, new bool[] { false, false, false, false, false, true, true, true }, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0b11100000)]
        [InlineData(2, 0b11111111, new bool[] { true, true, true, true, true, true, true, true }, new byte[] { 0, 1, 0, 0, 0, 0, 0, 0 }, 0b11111111)]
        public void TestSetLayerVisibility_2Pages(byte vramPages, byte values, bool[] expectedLayerVisible, byte[] expectedVisibleLayers, byte expectedLayerVisibleBits)
        {
            using Computer computer = new();
            computer.GRAPHICS.VRAM_PAGES = vramPages;
            computer.GRAPHICS.SetLayerVisibility(values);

            Assert.Equal(expectedLayerVisible, computer.GRAPHICS.LAYER_VISIBLE);
            Assert.Equal(expectedVisibleLayers, computer.GRAPHICS.GetVisibleLayers());
            Assert.Equal(expectedLayerVisibleBits, computer.GRAPHICS.LAYER_VISIBLE_BITS);
        }

        [Theory]
        [InlineData(1, 0b00000000, new bool[] { false, false, false, false, false, false, false, false }, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0b00000000)]
        [InlineData(1, 0b10000001, new bool[] { true, false, false, false, false, false, false, true }, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0b10000001)]
        [InlineData(1, 0b00111100, new bool[] { false, false, true, true, true, true, false, false }, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0b00111100)]
        [InlineData(1, 0b10111100, new bool[] { false, false, true, true, true, true, false, true }, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0b10111100)]
        [InlineData(1, 0b11100000, new bool[] { false, false, false, false, false, true, true, true }, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0b11100000)]
        [InlineData(1, 0b11111111, new bool[] { true, true, true, true, true, true, true, true }, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0b11111111)]
        public void TestSetLayerVisibility_1Page(byte vramPages, byte values, bool[] expectedLayerVisible, byte[] expectedVisibleLayers, byte expectedLayerVisibleBits)
        {
            using Computer computer = new();
            computer.GRAPHICS.VRAM_PAGES = vramPages;
            computer.GRAPHICS.SetLayerVisibility(values);

            Assert.Equal(expectedLayerVisible, computer.GRAPHICS.LAYER_VISIBLE);
            Assert.Equal(expectedVisibleLayers, computer.GRAPHICS.GetVisibleLayers());
            Assert.Equal(expectedLayerVisibleBits, computer.GRAPHICS.LAYER_VISIBLE_BITS);
        }

        [Theory]
        [InlineData(0, 0b00000000, new bool[] { false, false, false, false, false, false, false, false }, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0b00000000)]
        [InlineData(0, 0b10000001, new bool[] { true, false, false, false, false, false, false, true }, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0b10000001)]
        [InlineData(0, 0b00111100, new bool[] { false, false, true, true, true, true, false, false }, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0b00111100)]
        [InlineData(0, 0b10111100, new bool[] { false, false, true, true, true, true, false, true }, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0b10111100)]
        [InlineData(0, 0b11100000, new bool[] { false, false, false, false, false, true, true, true }, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0b11100000)]
        [InlineData(0, 0b11111111, new bool[] { true, true, true, true, true, true, true, true }, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0b11111111)]
        public void TestSetLayerVisibility_NoPages(byte vramPages, byte values, bool[] expectedLayerVisible, byte[] expectedVisibleLayers, byte expectedLayerVisibleBits)
        {
            using Computer computer = new();
            computer.GRAPHICS.VRAM_PAGES = vramPages;
            computer.GRAPHICS.SetLayerVisibility(values);

            Assert.Equal(expectedLayerVisible, computer.GRAPHICS.LAYER_VISIBLE);
            Assert.Equal(expectedVisibleLayers, computer.GRAPHICS.GetVisibleLayers());
            Assert.Equal(expectedLayerVisibleBits, computer.GRAPHICS.LAYER_VISIBLE_BITS);
        }

    }
}
