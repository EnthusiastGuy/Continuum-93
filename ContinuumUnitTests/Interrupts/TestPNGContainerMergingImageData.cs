using Continuum93.Emulator.GraphicsAccelerators;

namespace Interrupts
{
    public class TestPNGContainerMergingImageData
    {
        [Fact]
        public void TestMerging_Image_EmptyExisting()
        {
            byte[] existingPalette = Array.Empty<byte>();

            byte[] foundPalette =
            {
                0, 0, 0,
                128, 128, 0,
                64, 0, 128
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
                64, 0, 128
            };

            byte[] expectedImageData = foundImageData;

            PNGContainer png = new();

            png.SetPalette(foundPalette);
            png.SetPixelData(foundImageData);
            png.MergePalette(existingPalette);
            byte[] actualPalette = png.PaletteData;
            byte[] actualImageData = png.PixelData;

            Assert.Equal(expectedPalette, actualPalette);
            Assert.Equal(expectedImageData, actualImageData);
        }

        [Fact]
        public void TestMerging_Image_AllNew()
        {
            byte[] existingPalette =
            {
                0, 0, 0,
                116, 122, 241,
                41, 55, 0,
            };

            byte[] foundPalette =
            {
                128, 64, 0,
                255, 128, 0,
                128, 0, 128
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
                116, 122, 241,
                41, 55, 0,
                128, 64, 0,
                255, 128, 0,
                128, 0, 128
            };

            byte[] expectedImageData = {
                3, 3, 3, 3, 3, 3, 3, 3,
                3, 4, 4, 4, 4, 4, 4, 3,
                3, 4, 5, 5, 5, 5, 4, 3,
                3, 4, 5, 5, 5, 5, 4, 3,
                3, 4, 4, 4, 4, 4, 4, 3,
                3, 3, 3, 3, 3, 3, 3, 3
            };

            PNGContainer png = new();

            png.SetPalette(foundPalette);
            png.SetPixelData(foundImageData);
            png.MergePalette(existingPalette);
            byte[] actualPalette = png.PaletteData;
            byte[] actualImageData = png.PixelData;

            Assert.Equal(expectedPalette, actualPalette);
            Assert.Equal(expectedImageData, actualImageData);
        }

        [Fact]
        public void TestMerging_Image_SomeDuplicates()
        {
            byte[] existingPalette =
            {
                0, 0, 0,
                116, 122, 241,
                41, 55, 0,
            };

            byte[] foundPalette =
            {
                128, 64, 0,
                255, 128, 0,
                116, 122, 241,
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
                116, 122, 241,
                41, 55, 0,
                128, 64, 0,
                255, 128, 0,
            };

            byte[] expectedImageData = {
                3, 3, 3, 3, 3, 3, 3, 3,
                3, 4, 4, 4, 4, 4, 4, 3,
                3, 4, 1, 1, 1, 1, 4, 3,
                3, 4, 1, 1, 1, 1, 4, 3,
                3, 4, 4, 4, 4, 4, 4, 3,
                3, 3, 3, 3, 3, 3, 3, 3
            };

            PNGContainer png = new();

            png.SetPalette(foundPalette);
            png.SetPixelData(foundImageData);
            png.MergePalette(existingPalette);
            byte[] actualPalette = png.PaletteData;
            byte[] actualImageData = png.PixelData;

            Assert.Equal(expectedPalette, actualPalette);
            Assert.Equal(expectedImageData, actualImageData);
        }

        [Fact]
        public void TestMerging_Image_AllDuplicates()
        {
            byte[] existingPalette =
            {
                0, 0, 0,
                116, 122, 241,
                41, 55, 0,
                255, 128, 0,
            };

            byte[] foundPalette =
            {
                255, 128, 0,
                41, 55, 0,
                116, 122, 241,
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
                116, 122, 241,
                41, 55, 0,
                255, 128, 0,
            };

            byte[] expectedImageData = {
                3, 3, 3, 3, 3, 3, 3, 3,
                3, 2, 2, 2, 2, 2, 2, 3,
                3, 2, 1, 1, 1, 1, 2, 3,
                3, 2, 1, 1, 1, 1, 2, 3,
                3, 2, 2, 2, 2, 2, 2, 3,
                3, 3, 3, 3, 3, 3, 3, 3
            };

            PNGContainer png = new();

            png.SetPalette(foundPalette);
            png.SetPixelData(foundImageData);
            png.MergePalette(existingPalette);
            byte[] actualPalette = png.PaletteData;
            byte[] actualImageData = png.PixelData;

            Assert.Equal(expectedPalette, actualPalette);
            Assert.Equal(expectedImageData, actualImageData);
        }
    }
}
