using Continuum93.Emulator.GraphicsAccelerators;

namespace Interrupts
{
    public class TestPNGContainerMerging
    {
        [Fact]
        public void TestMerging_EmptyExisting()
        {
            byte[] existingPalette = Array.Empty<byte>();

            byte[] foundPalette =
            {
                0, 0, 0,
                128, 128, 0,
                64, 0, 128
            };

            byte[] expected =
            {
                0, 0, 0,
                128, 128, 0,
                64, 0, 128
            };

            PNGContainer png = new();

            png.SetPalette(foundPalette);
            png.SetPixelData(Array.Empty<byte>());

            Assert.Equal(3, png.ColorCount);

            png.MergePalette(existingPalette);
            byte[] actual = png.PaletteData;

            Assert.Equal(expected, actual);
            Assert.Equal(3, png.ColorCount);    // Palette size does not increase
        }

        [Fact]
        public void TestMerging_NoDuplicates()
        {
            byte[] existingPalette =
            {
                0, 0, 0,
                79, 104, 255,
                159, 0, 0,
                255, 168, 0,
                207, 88, 0,
                184, 255, 255,
                200, 0, 207
            };

            byte[] foundPalette =
            {
                0, 0, 0,
                128, 128, 0,
                64, 0, 128
            };

            byte[] expected =
            {
                0, 0, 0,        // This is generally the "transparent" color, so it can be duplicated by another
                79, 104, 255,
                159, 0, 0,
                255, 168, 0,
                207, 88, 0,
                184, 255, 255,
                200, 0, 207,
                0, 0, 0,
                128, 128, 0,
                64, 0, 128
            };

            PNGContainer png = new();

            png.SetPalette(foundPalette);
            png.SetPixelData(Array.Empty<byte>());

            Assert.Equal(3, png.ColorCount);        // Initial color palette size

            png.MergePalette(existingPalette);
            byte[] actual = png.PaletteData;

            Assert.Equal(expected, actual);
            Assert.Equal(10, png.ColorCount);        // Resulting color palette size
        }

        [Fact]
        public void TestMerging_All_Duplicates()
        {
            byte[] existingPalette =
            {
                0, 0, 0,
                79, 104, 255,
                159, 0, 0,
                255, 168, 0,
                207, 88, 0,
                184, 255, 255,
                200, 0, 207
            };

            byte[] foundPalette =
            {
                159, 0, 0,
                200, 0, 207,
                79, 104, 255,
            };

            byte[] expected =
            {
                0, 0, 0,
                79, 104, 255,
                159, 0, 0,
                255, 168, 0,
                207, 88, 0,
                184, 255, 255,
                200, 0, 207
            };

            PNGContainer png = new();

            png.SetPalette(foundPalette);
            png.SetPixelData(Array.Empty<byte>());
            png.MergePalette(existingPalette);
            byte[] actual = png.PaletteData;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestMerging_Same_Palette()
        {
            byte[] existingPalette =
            {
                255, 255, 255,
                252, 252, 252,
                204, 204, 204,
                163, 69, 0,
                207, 88, 0,
                60, 76, 188,
                76, 108, 252,
                0, 0, 0,
            };

            byte[] foundPalette =
            {
                60, 76, 188,
                76, 108, 252,
                207, 88, 0,
                163, 69, 0,
                252, 252, 252,
                204, 204, 204,
                0, 0, 0,
                0, 0, 0,
            };

            byte[] expected =
            {
                255, 255, 255,
                252, 252, 252,
                204, 204, 204,
                163, 69, 0,
                207, 88, 0,
                60, 76, 188,
                76, 108, 252,
                0, 0, 0,
            };

            PNGContainer png = new();

            png.SetPalette(foundPalette);
            png.SetPixelData(Array.Empty<byte>());
            png.MergePalette(existingPalette);
            png.ApplyTransparencyCorrection(0, 0, 0, 0);
            byte[] actual = png.PaletteData;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestMerging_Duplicates1()
        {
            byte[] existingPalette =
            {
                0, 0, 0,
                79, 104, 255,
                159, 0, 0,
                255, 168, 0,
                207, 88, 0,
                184, 255, 255,
                200, 0, 207
            };

            byte[] foundPalette =
            {
                0, 0, 0,
                255, 168, 0,
                64, 0, 128
            };

            byte[] expected =
            {
                0, 0, 0,
                79, 104, 255,
                159, 0, 0,
                255, 168, 0,
                207, 88, 0,
                184, 255, 255,
                200, 0, 207,
                0, 0, 0,
                64, 0, 128
            };

            PNGContainer png = new();

            png.SetPalette(foundPalette);
            png.SetPixelData(Array.Empty<byte>());
            png.MergePalette(existingPalette);
            byte[] actual = png.PaletteData;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestMerging_Duplicates2()
        {
            byte[] existingPalette =
            {
                0, 0, 0,
                79, 104, 255,
                159, 0, 0,
                255, 168, 0,
                207, 88, 0,
                184, 255, 255,
                200, 0, 207
            };

            byte[] foundPalette =
            {
                184, 255, 255,
                200, 0, 207,
                0, 0, 0,
                255, 168, 0,
                64, 0, 128
            };

            byte[] expected =
            {
                0, 0, 0,
                79, 104, 255,
                159, 0, 0,
                255, 168, 0,
                207, 88, 0,
                184, 255, 255,
                200, 0, 207,
                0, 0, 0,
                64, 0, 128
            };

            PNGContainer png = new();

            png.SetPalette(foundPalette);
            png.SetPixelData(Array.Empty<byte>());
            png.MergePalette(existingPalette);
            byte[] actual = png.PaletteData;

            Assert.Equal(expected, actual);
        }
    }
}
