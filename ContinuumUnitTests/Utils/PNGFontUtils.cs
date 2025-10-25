namespace ContinuumUnitTests.Utils
{
    public static class PNGFontUtils
    {
        public static int CalculatePNGFontSize(int cellWidth, int cellHeight)
        {
            // Calculate the expected output size

            // 1 byte: font type
            // 1 byte: glyph cell width
            // 1 byte: glyph cell height
            // 1 byte: max glyph width
            // 1 byte: max glyph height
            // 1 byte: number of kerning pairs (nk value)
            int headerSize = 6;

            // 96 bytes: width of each character starting with space
            int glyphWidthsSize = 96;

            // nk * 3 bytes: kerning pair values (nk = 0)
            int kerningDataSize = 0;

            // Calculate bytes per row for glyph data
            int bytesPerRow = (int)Math.Ceiling(cellWidth / 8.0);

            // Calculate glyph data size per glyph
            int glyphDataSizePerGlyph = bytesPerRow * cellHeight;

            // Number of glyphs excluding the space character (which is skipped in font data)
            int glyphCount = 96 - 1;

            // Total font data size
            int fontDataSize = glyphCount * glyphDataSizePerGlyph;

            // Expected total size
            return headerSize + glyphWidthsSize + kerningDataSize + fontDataSize;
        }
    }
}
