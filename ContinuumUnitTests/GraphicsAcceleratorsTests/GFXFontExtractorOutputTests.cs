using Continuum93.Emulator.GraphicsAccelerators;
using ContinuumUnitTests.Utils;

namespace GraphicsAcceleratorsTests
{
    public class GFXFontExtractorOutputTests
    {
        /// <summary>
        /// Tests that the GetRawFontData method produces output of the correct size
        /// for various glyph cell dimensions.
        /// </summary>
        [Theory]
        [InlineData(7, 8)]    // Scenario 1: cellWidth = 7,  cellHeight = 8
        [InlineData(12, 10)]  // Scenario 2: cellWidth = 12, cellHeight = 10
        [InlineData(16, 16)]  // Scenario 3: cellWidth = 16, cellHeight = 16
        [InlineData(20, 24)]  // Scenario 4: cellWidth = 20, cellHeight = 24
        [InlineData(32, 32)]  // Scenario 5: cellWidth = 32, cellHeight = 32
        public void TestGetRawFontDataSize(int cellWidth, int cellHeight)
        {
            // Calculate the image dimensions based on the cell size
            int pngWidth = cellWidth * 16;  // 16 glyphs per row
            int pngHeight = cellHeight * 6; // 6 rows of glyphs

            // Create dummy pixel data for the font extractor
            int pixelDataLength = pngWidth * pngHeight;
            byte[] pixelData = new byte[pixelDataLength];

            // Fill the pixel data with some dummy values (e.g., all pixels set to 0xFF)
            // For this test, the actual pixel values are not important
            for (int i = cellHeight * pngWidth; i < pixelDataLength; i++)
            {
                pixelData[i] = 0xFF;
            }

            // Instantiate the GFXFontExtractor with the dummy pixel data
            GFXFontExtractor extractor = new(pixelData, pngWidth, pngHeight);

            // Get the raw font data
            byte[] rawFontData = extractor.GetRawFontData();

            int expectedTotalSize = PNGFontUtils.CalculatePNGFontSize(cellWidth, cellHeight);

            // Assert that the length of the raw font data matches the expected total size
            Assert.Equal(expectedTotalSize, rawFontData.Length);

            Assert.Equal(1, rawFontData[0]);            // Font type
            Assert.Equal(cellWidth, rawFontData[1]);    // Glyph width
            Assert.Equal(cellHeight, rawFontData[2]);   // Glyph height
        }

        
    }
}
