using Continuum93.Emulator.GraphicsAccelerators;

namespace Interrupts
{
    public class TestPNGContainerMergingTransparent
    {
        [Fact]
        public void TestMerging_EmptyExisting()
        {
            byte[] existingPalette = Array.Empty<byte>();

            byte[] foundPalette =
            {
                0, 0, 0,
                255, 0, 255,
                64, 0, 128
            };

            byte[] expected =
            {
                255, 0, 255,
                0, 0, 0,
                64, 0, 128
            };

            PNGContainer png = new();

            png.SetPalette(foundPalette);
            png.SetPixelData(Array.Empty<byte>());
            png.MergePalette(existingPalette);
            png.ApplyTransparencyCorrection(255, 0, 255, 255);
            byte[] actual = png.PaletteData;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestMerging_PartialExisting()
        {
            byte[] existingPalette =
            {
                100, 0, 100,
                0, 0, 0,
                64, 0, 128
            };

            byte[] foundPalette =
            {
                0, 0, 0,
                255, 0, 255,
                128, 255, 128
            };

            byte[] expected =
            {
                100, 0, 100,
                0, 0, 0,
                64, 0, 128,
                128, 255, 128
            };

            PNGContainer png = new();

            png.SetPalette(foundPalette);
            png.SetPixelData(Array.Empty<byte>());
            png.MergePaletteWithTransparent(existingPalette, 255, 0, 255, 255);
            byte[] actual = png.PaletteData;

            Assert.Equal(expected, actual);
        }
    }
}
