namespace ContinuumUnitTests.GraphicsAcceleratorsTests
{
    public static class GFXFontExtractorDummyGenerator
    {

        public static byte[] GenerateTestFontSheetWithWidths(int cellWidth, int cellHeight, byte[] widths)
        {
            int sheetWidth = cellWidth * 16;    // 16 glyphs per row
            int sheetHeight = cellHeight * 6;   // 6 rows of glyphs

            byte[] glyphData = new byte[sheetWidth * sheetHeight];

            // Iterate through all glyphs except the first one (space character)
            for (int i = 1; i < 96; i++)
            {
                int glyphCartesianY = i / 16;
                int glyphCartesianX = i % 16;

                // Calculate the starting index in the glyphData array for this glyph
                int glyphTopLeft = glyphCartesianY * cellHeight * sheetWidth + glyphCartesianX * cellWidth;

                // Get the width for the current glyph from the widths array
                int glyphWidth = widths[i];

                // Generate the glyph data as a filled rectangle of the specified width
                byte[] glyphCharData = GenerateGlyph(cellWidth, cellHeight, glyphWidth);

                // Copy the glyph data into the glyphData array at the correct position
                for (int j = 0; j < cellHeight; j++)
                {
                    for (int k = 0; k < cellWidth; k++)
                    {
                        // Calculate the index in the glyphData array
                        int glyphSheetIndex = glyphTopLeft + j * sheetWidth + k;
                        // Copy the glyph pixel data to the corresponding index in glyphData
                        glyphData[glyphSheetIndex] = glyphCharData[j * cellWidth + k];
                    }
                }
            }

            return glyphData;
        }

        private static byte[] GenerateGlyph(int cellWidth, int cellHeight, int glyphWidth)
        {
            byte[] glyphCharData = new byte[cellWidth * cellHeight];

            // Loop through each row of the glyph
            for (int row = 0; row < cellHeight; row++)
            {
                // Loop through each column of the glyph
                for (int col = 0; col < cellWidth; col++)
                {
                    if (col < glyphWidth)
                    {
                        // Set filled pixels (e.g., 0xFF represents a filled pixel)
                        glyphCharData[row * cellWidth + col] = 0xFF;
                    }
                    else
                    {
                        // Set transparent pixels (e.g., 0x00 represents transparency)
                        glyphCharData[row * cellWidth + col] = 0x00;
                    }
                }
            }

            return glyphCharData;
        }
    }
}
