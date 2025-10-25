using Continuum93.Emulator.GraphicsAccelerators;

namespace Interrupts
{
    public class TestPNGContainerMergingTransparentImageData
    {
        [Fact]
        public void TestMerging_Image_EmptyExisting()
        {
            byte[] existingPalette = Array.Empty<byte>();

            byte[] foundPalette =
            {
                0, 0, 0,
                128, 128, 0,
                255, 0, 255
            };

            byte[] foundImageData =
            {
                0, 0, 0, 0, 0, 0, 0, 0,
                0, 1, 1, 1, 1, 1, 1, 0,
                0, 1, 2, 2, 2, 2, 1, 0,
                0, 1, 2, 2, 2, 2, 1, 0,
                0, 1, 1, 1, 1, 1, 1, 0,
                0, 0, 0, 0, 0, 0, 0, 0
            };

            byte[] expectedPalette =
            {
                0, 0, 0,
                0, 0, 0,
                128, 128, 0,
            };

            byte[] expectedImageData = {
                1, 1, 1, 1, 1, 1, 1, 1,
                1, 2, 2, 2, 2, 2, 2, 1,
                1, 2, 0, 0, 0, 0, 2, 1,
                1, 2, 0, 0, 0, 0, 2, 1,
                1, 2, 2, 2, 2, 2, 2, 1,
                1, 1, 1, 1, 1, 1, 1, 1
            }; ;

            PNGContainer png = new();

            png.SetPalette(foundPalette);
            png.SetPixelData(foundImageData);
            png.MergePaletteWithTransparent(existingPalette, 255, 0, 255, 255);
            byte[] actualPalette = png.PaletteData;
            byte[] actualImageData = png.PixelData;

            Assert.Equal(expectedPalette, actualPalette);
            Assert.Equal(expectedImageData, actualImageData);
        }

        [Fact]
        public void TestMerging_Image_PartialExisting()
        {
            byte[] existingPalette = {
                0, 0, 0,
                128, 128, 0,
                255, 255, 255,
                255, 0, 255
            };

            byte[] foundPalette =
            {
                0, 0, 0,
                128, 128, 0,
                255, 0, 255
            };

            byte[] foundImageData =
            {
                0, 0, 0, 0, 0, 0, 0, 0,
                0, 1, 1, 1, 1, 1, 1, 0,
                0, 1, 2, 2, 2, 2, 1, 0,
                0, 1, 2, 2, 2, 2, 1, 0,
                0, 1, 1, 1, 1, 1, 1, 0,
                0, 0, 0, 0, 0, 0, 0, 0
            };

            byte[] expectedPalette =
            {
                0, 0, 0,
                128, 128, 0,
                255, 255, 255,
                255, 0, 255,
                0, 0, 0,
            };

            byte[] expectedImageData = {
                4, 4, 4, 4, 4, 4, 4, 4,
                4, 1, 1, 1, 1, 1, 1, 4,
                4, 1, 0, 0, 0, 0, 1, 4,
                4, 1, 0, 0, 0, 0, 1, 4,
                4, 1, 1, 1, 1, 1, 1, 4,
                4, 4, 4, 4, 4, 4, 4, 4
            }; ;

            PNGContainer png = new();

            png.SetPalette(foundPalette);
            png.SetPixelData(foundImageData);
            png.MergePaletteWithTransparent(existingPalette, 255, 0, 255, 255);
            byte[] actualPalette = png.PaletteData;
            byte[] actualImageData = png.PixelData;

            Assert.Equal(expectedPalette, actualPalette);
            Assert.Equal(expectedImageData, actualImageData);
        }

        [Fact]
        public void TestMerging_Image_AllExisting()
        {
            byte[] existingPalette = {
                0, 0, 0,
                128, 128, 0,
                255, 255, 255,
                0, 0, 0,
                255, 0, 255,
                100, 200, 0
            };

            byte[] foundPalette =
            {
                0, 0, 0,
                128, 128, 0,
                255, 0, 255
            };

            byte[] foundImageData =
            {
                0, 0, 0, 0, 0, 0, 0, 0,
                0, 1, 1, 1, 1, 1, 1, 0,
                0, 1, 2, 2, 2, 2, 1, 0,
                0, 1, 2, 2, 2, 2, 1, 0,
                0, 1, 1, 1, 1, 1, 1, 0,
                0, 0, 0, 0, 0, 0, 0, 0
            };

            byte[] expectedPalette =
            {
                0, 0, 0,
                128, 128, 0,
                255, 255, 255,
                0, 0, 0,
                255, 0, 255,
                100, 200, 0
            };

            byte[] expectedImageData = {
                3, 3, 3, 3, 3, 3, 3, 3,
                3, 1, 1, 1, 1, 1, 1, 3,
                3, 1, 0, 0, 0, 0, 1, 3,
                3, 1, 0, 0, 0, 0, 1, 3,
                3, 1, 1, 1, 1, 1, 1, 3,
                3, 3, 3, 3, 3, 3, 3, 3
            }; ;

            PNGContainer png = new();

            png.SetPalette(foundPalette);
            png.SetPixelData(foundImageData);
            png.MergePaletteWithTransparent(existingPalette, 255, 0, 255, 255);
            byte[] actualPalette = png.PaletteData;
            byte[] actualImageData = png.PixelData;

            Assert.Equal(expectedPalette, actualPalette);
            Assert.Equal(expectedImageData, actualImageData);
        }
    }
}
