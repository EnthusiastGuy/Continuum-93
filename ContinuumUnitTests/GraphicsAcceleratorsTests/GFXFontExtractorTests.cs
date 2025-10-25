using Continuum93.Emulator.GraphicsAccelerators;
using ContinuumUnitTests.GraphicsAcceleratorsTests;
using System.Reflection;

namespace GraphicsAcceleratorsTests
{
    public class GFXFontExtractorTests
    {
        // Helper class to hold test case data
        public class MaxGlyphWidthTestCase
        {
            public int CellWidth { get; set; }
            public int CellHeight { get; set; }
            public byte[] Widths { get; set; }
            public byte ExpectedMaxWidth { get; set; }
            public string Description { get; set; }
        }

        // Generate test cases as a static property
        public static IEnumerable<object[]> MaxGlyphWidthTestCases
        {
            get
            {
                var testCases = new List<MaxGlyphWidthTestCase>
            {
                // Test Case 1
                new()
                {
                    CellWidth = 8,
                    CellHeight = 12,
                    Widths = new byte[96],  // All zeros
                    ExpectedMaxWidth = 0,
                    Description = "All glyphs have zero width; expected max width is 0",
                },
                // Test Case 2
                new()
                {
                    CellWidth = 8,
                    CellHeight = 12,
                    Widths = new byte[]
                    {
                        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,     //   ! " # $ % & ' ( ) * + , - . /
                        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,     // 0 1 2 3 4 5 6 7 8 9 : ; < = > ?
                        0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,     // @ A B C D E F G H I J K L M N O
                        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,     // P Q R S T U V W X Y Z [ \ ] ^ _
                        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,     // ` a b c d e f g h i j k l m n o
                        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,     // p q r s t u v w x y z { | } ~ ©
                    },
                    ExpectedMaxWidth = 3,
                    Description = "All glyphs have zero width except glyph 'D'; expected max width is 3",
                }
            };

                // Add test cases for each glyph index
                for (int i = 0; i < 96; i++)
                {
                    byte[] widths = new byte[96];
                    widths[i] = 5;

                    testCases.Add(new MaxGlyphWidthTestCase
                    {
                        CellWidth = 8,
                        CellHeight = 12,
                        Widths = widths,
                        ExpectedMaxWidth = (byte)(IsNotAlphaCharacterIndex(i) ? 0 : 5),
                        Description = $"All glyphs have zero width except glyph index '{i}'; expected max width is {(byte)(IsNotAlphaCharacterIndex(i) ? 0 : 5)}"
                    });
                }

                // Convert each test case to an object array
                foreach (var testCase in testCases)
                {
                    yield return new object[] { testCase };
                }
            }
        }

        [Theory]
        [MemberData(nameof(MaxGlyphWidthTestCases))]
        public void TestMaxGlyphWidthForAlphabetic(MaxGlyphWidthTestCase testCase)
        {
            int pngWidth = testCase.CellWidth * 16;
            int pngHeight = testCase.CellHeight * 6;

            // Generate the test font sheet with specified glyph widths
            byte[] fontData = GFXFontExtractorDummyGenerator.GenerateTestFontSheetWithWidths(testCase.CellWidth, testCase.CellHeight, testCase.Widths);

            // Create an instance of GFXFontExtractor with the generated font data
            GFXFontExtractor extractor = new(fontData, pngWidth, pngHeight);

            // Test that the maximum width is calculated correctly
            byte maxWidth = extractor.GetMaxGlyphWidthForAlphabetic();

            // Assert that the calculated max width matches the expected value
            // Include the test case description in the assertion message
            Assert.True(testCase.ExpectedMaxWidth == maxWidth, $"Test Failed: {testCase.Description}");

        }

        private static bool IsNotAlphaCharacterIndex(int i)
        {
            return (i < 33 || (i > 58 && i < 65) || (i > 90));
        }


        [Fact]
        public void TestFlattenGlyphData_SkipFirstGlyph()
        {
            // Prepare a mock glyphDataList with sample data
            List<byte[]> glyphDataList = new()
        {
            new byte[] { 0x00, 0x00 }, // Space (glyph 0, will be skipped)
            new byte[] { 0x01, 0x02 }, // Glyph 1
            new byte[] { 0x03, 0x04 }, // Glyph 2
        };

            GFXFontExtractor extractor = new(new byte[128 * 72], 128, 72); // Dummy values for constructor
            byte[] flattenedData = extractor.FlattenGlyphData(glyphDataList);

            // We expect the flattened array to contain the data from glyph 1 and glyph 2 only
            byte[] expectedFlattenedData = new byte[] { 0x01, 0x02, 0x03, 0x04 };

            Assert.Equal(expectedFlattenedData, flattenedData);
        }

        [Fact]
        public void TestProcess_ValidGlyphWidths()
        {
            // Test a simple font with two non-transparent pixels per glyph

            int glyphWidth = 8;
            int glyphHeight = 12;
            int pngWidth = glyphWidth * 16;
            int pngHeight = glyphHeight * 6;

            byte[] pixelData = new byte[pngWidth * pngHeight]; // Assume 128x72 PNG, 8x12 per glyph

            // Fill in some pixels for glyphs, assuming glyphs 33 to 58 are uppercase characters
            for (int i = 33; i <= 58; i++)
            {
                // Set non-transparent pixels for each glyph
                int y = i / 16;
                int x = i % 16;
                int index = y * glyphHeight * pngWidth + x * glyphWidth;
                pixelData[index] = 0x01; // Non-transparent pixel for testing
            }

            // Create the extractor
            GFXFontExtractor extractor = new(pixelData, pngWidth, pngHeight);

            // Ensure glyph width for the first uppercase glyph (index 33) is calculated correctly
            byte maxWidth = extractor.GetMaxGlyphWidthForAlphabetic();

            // The width should be non-zero, based on the test data
            Assert.True(maxWidth > 0);
        }

        [Fact]
        public void TestClearData_ResetsPixelAndGlyphData()
        {
            byte[] pixelData = new byte[128 * 72];
            GFXFontExtractor extractor = new(pixelData, 128, 72);

            // Now clear the data
            extractor.ClearData();

            // Verify that pixelData and glyphWidth have been reset (set to null)
            Assert.Null(extractor.GetType().GetField("pngPixelData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(extractor));
            Assert.Null(extractor.GetType().GetField("glyphWidth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(extractor));
        }

        [Fact]
        public void TestFlattenGlyphData_ReturnsExpectedArrayLength()
        {
            // Define pixel data for a simple 128x72 PNG (which corresponds to 8x12 cells for 96 glyphs)
            byte[] pixelData = new byte[128 * 72];

            // Simulate non-transparent pixels for some glyphs (for simplicity, we assume random non-transparent data)
            // Modify the pixelData as necessary, here we'll set some random pixels to non-zero
            for (int i = 0; i < pixelData.Length; i += 10) // Set every 10th pixel to non-transparent
            {
                pixelData[i] = 0x01;
            }

            // Create an instance of GFXFontExtractor
            GFXFontExtractor extractor = new(pixelData, 128, 72); // Assume 128x72 PNG with 16x6 grid of glyphs

            // Get the flattened byte array from the processed data
            byte[] flattenedData = extractor.FlattenGlyphData(extractor.ConvertFontToBitData());

            // Calculate the expected length of the flattened array
            // bytesPerGlyphWidth = Math.Ceiling(cellWidth / 8) -> cellWidth is 8 in this case -> 1 byte per row
            // Each glyph has 12 rows, so for each glyph we expect 12 bytes.
            // We have 95 glyphs (skipping the space), so expected length is 95 * 12 = 1140 bytes
            int expectedLength = 95 * 12;

            // Assert that the flattened data has the expected length
            Assert.Equal(expectedLength, flattenedData.Length);
        }


        /// <summary>
        /// Tests that the GFXFontExtractor correctly calculates cellWidth, cellHeight, bytesPerGlyphWidth,
        /// and the size of fontData for various input image dimensions.
        /// </summary>
        [Theory]
        [InlineData(160, 60)]   // Expected cellWidth = 10, cellHeight = 10, bytesPerGlyphWidth = 2
        [InlineData(256, 96)]   // Expected cellWidth = 16, cellHeight = 16, bytesPerGlyphWidth = 2
        [InlineData(192, 72)]   // Expected cellWidth = 12, cellHeight = 12, bytesPerGlyphWidth = 2
        [InlineData(128, 72)]   // Expected cellWidth = 8,  cellHeight = 12, bytesPerGlyphWidth = 1
        [InlineData(320, 180)]  // Expected cellWidth = 20, cellHeight = 30, bytesPerGlyphWidth = 3
        public void TestCellDimensionsAndFontDataSize(int pngWidth, int pngHeight)
        {
            // Calculate expected cellWidth and cellHeight based on the image dimensions
            int expectedCellWidth = pngWidth / 16;
            int expectedCellHeight = pngHeight / 6;

            // Calculate expected bytesPerGlyphWidth
            byte expectedBytesPerGlyphWidth = (byte)Math.Ceiling(expectedCellWidth / 8.0);

            // Create a dummy pixel data array with the appropriate size
            int pixelDataLength = pngWidth * pngHeight;
            byte[] pixelData = new byte[pixelDataLength];

            // Instantiate the GFXFontExtractor with the test inputs
            GFXFontExtractor extractor = new(pixelData, pngWidth, pngHeight);

            // Use reflection to access private fields within the GFXFontExtractor instance
            Type extractorType = typeof(GFXFontExtractor);

            // Access the 'cellWidth' field
            FieldInfo cellWidthField = extractorType.GetField("cellWidth", BindingFlags.NonPublic | BindingFlags.Instance);
            int actualCellWidth = (int)cellWidthField.GetValue(extractor);

            // Access the 'cellHeight' field
            FieldInfo cellHeightField = extractorType.GetField("cellHeight", BindingFlags.NonPublic | BindingFlags.Instance);
            int actualCellHeight = (int)cellHeightField.GetValue(extractor);

            // Access the 'bytesPerGlyphWidth' field
            FieldInfo bytesPerGlyphWidthField = extractorType.GetField("bytesPerGlyphWidth", BindingFlags.NonPublic | BindingFlags.Instance);
            byte actualBytesPerGlyphWidth = (byte)bytesPerGlyphWidthField.GetValue(extractor);

            // Assert that the actual values match the expected values
            Assert.Equal(expectedCellWidth, actualCellWidth);
            Assert.Equal(expectedCellHeight, actualCellHeight);
            Assert.Equal(expectedBytesPerGlyphWidth, actualBytesPerGlyphWidth);

            // Now, calculate expected fontData size
            int expectedBytesPerRow = (int)Math.Ceiling(expectedCellWidth / 8.0);
            int glyphDataSize = expectedBytesPerRow * expectedCellHeight;
            int totalGlyphs = 96 - 1; // Excluding the first glyph (space character)
            int expectedFontDataSize = totalGlyphs * glyphDataSize;

            // Use reflection to access the private field 'fontData'
            FieldInfo fontDataField = extractorType.GetField("fontData", BindingFlags.NonPublic | BindingFlags.Instance);
            byte[] fontData = (byte[])fontDataField.GetValue(extractor);

            // Assert that the actual fontData size matches the expected size
            Assert.Equal(expectedFontDataSize, fontData.Length);
        }
    }
}
