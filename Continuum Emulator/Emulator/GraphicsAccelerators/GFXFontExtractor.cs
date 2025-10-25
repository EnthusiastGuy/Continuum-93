using System;
using System.Collections.Generic;

namespace Continuum93.Emulator.GraphicsAccelerators
{
    /// <summary>
    /// Extracts font glyph data from a PNG image containing a grid of font glyphs.
    /// The PNG image is expected to be arranged in a grid of 16 columns and 6 rows,
    /// representing 96 glyphs starting from the space character (ASCII code 32).
    /// Each glyph cell has uniform width and height, calculated based on the image dimensions.
    /// 
    /// **Transparency Handling:**
    /// 
    /// To accommodate designs where multiple colors are used as transparent backgrounds (e.g., checkered patterns or cell outlines),
    /// this class determines all transparent color indices by scanning the first glyph cell (space character).
    /// 
    /// - **Transparent Colors Determination:**
    ///   - The first glyph cell (located at the top-left corner) is scanned entirely.
    ///   - All unique color indices found in the pixels of this cell are collected into a set of transparent colors.
    ///   - These colors are then used throughout the glyph extraction process to identify transparent pixels.
    /// 
    /// - **Glyph Processing:**
    ///   - When scanning glyphs, any pixel whose color index is in the set of transparent colors is considered transparent (background).
    ///   - Pixels with color indices not in the transparent set are considered part of the glyph (foreground).
    /// 
    /// **Important Notes for Designers:**
    /// 
    /// - **Space Character Cell Usage:**
    ///   - Designers should ensure that the space character cell contains all colors that should be treated as transparent.
    ///   - This allows the use of multiple background colors without affecting glyph extraction.
    /// 
    /// - **Design Flexibility:**
    ///   - You can use patterns or colors (e.g., alternating colors, grid lines) in the background to assist in glyph design.
    ///   - As long as these colors are included in the space character cell, they will be recognized as transparent.
    /// 
    /// **Overall Process:**
    /// 
    /// - The class processes the image to determine individual glyph widths by scanning for non-transparent pixels.
    /// - It then converts the glyph pixel data into a bit-packed format suitable for font rendering.
    /// - The final font data includes metadata and the actual glyph data, ready for use in rendering engines.
    /// </summary>
    public class GFXFontExtractor
    {
        private readonly int pngWidth;                  // Width of the PNG image in pixels.
        private readonly int pngHeight;                 // Height of the PNG image in pixels.
        private byte[] pngPixelData;                    // Flattened array of pixel data from the PNG image.

        private readonly int cellWidth;                 // Width of each glyph cell in the image grid.
        private readonly int cellHeight;                // Height of each glyph cell in the image grid.
        private readonly byte bytesPerGlyphWidth;       // Number of bytes required to represent the width of a glyph in bits.
        private byte[] glyphWidth;                      // Array storing the calculated width of each glyph.
        private byte maxAlphaGlyphWidth;                // Maximum glyph width among alphabetic characters.

        private byte[] fontData;                        // Flattened array containing the font glyph data.

        private HashSet<byte> transparentColors;        // Set of transparent color indices.

        private byte[] kerningData;                     // Byte array containing kerning pairs and values.

        /// <summary>
        /// Initializes a new instance of the <see cref="GFXFontExtractor"/> class.
        /// </summary>
        /// <param name="pixelData">An array of bytes representing the pixel data of the PNG image.</param>
        /// <param name="width">The width of the PNG image in pixels.</param>
        /// <param name="height">The height of the PNG image in pixels.</param>
        /// <param name="kerningData">An array of bytes representing kerning pairs and values.</param>
        public GFXFontExtractor(byte[] pixelData, int width, int height, byte[] kerning = null)
        {
            glyphWidth = new byte[96];
            pngWidth = width;
            pngHeight = height;

            // Calculate the dimensions of each glyph cell in the image grid.
            // Assume the font image contains 16 columns and 6 rows of glyphs.
            cellWidth = pngWidth / 16;
            cellHeight = pngHeight / 6;

            // Calculate the number of bytes required to represent the width of a glyph in bits.
            bytesPerGlyphWidth = (byte)Math.Ceiling(cellWidth / 8.0);

            // Store the pixel data and start processing the font glyphs.
            pngPixelData = pixelData;

            // Store the kerning data.
            kerningData = kerning ?? new byte[0];

            Process();
        }

        /// <summary>
        /// Processes the PNG image to extract glyph widths and generate font data.
        /// </summary>
        private void Process()
        {
            // Get the set of transparent colors from the first glyph cell (space character).
            GetTransparencyColors();

            // Calculate the width of each glyph by scanning from right to left.
            // Skip the first glyph (space character) as it will be handled separately.
            for (int i = 1; i < 96; i++)
            {
                // Determine the grid position (x, y) of the glyph in the image.
                int y = i / 16;
                int x = i % 16;

                // Scan each column of the glyph cell from right to left.
                for (int j = cellWidth - 1; j >= 0; j--)
                {
                    // Scan each row (pixel) in the column.
                    for (int k = 0; k < cellHeight; k++)
                    {
                        int index = (y * cellHeight + k) * pngWidth + (x * cellWidth + j);

                        byte pixelData = pngPixelData[index];
                        if (!transparentColors.Contains(pixelData))
                        {
                            // Found a non-transparent pixel; set the glyph width.
                            glyphWidth[i] = (byte)(j + 1);
                            break; // Stop scanning this column.
                        }
                    }
                    if (glyphWidth[i] > 0)
                    {
                        break; // Width determined; move to the next glyph.
                    }
                }
            }

            // Set the width of the space character to half of the maximum width of alphanumeric glyphs.
            glyphWidth[0] = (byte)(GetMaxGlyphWidthForAlphabetic() / 2);

            // Convert the glyphs into bit data and flatten into a single byte array.
            fontData = FlattenGlyphData(ConvertFontToBitData());
        }

        /// <summary>
        /// Gets the set of transparent colors by scanning the first glyph cell (space character).
        /// </summary>
        private void GetTransparencyColors()
        {
            transparentColors = new HashSet<byte>();

            // The first glyph (space character) is at position (0, 0)
            int glyphX = 0;
            int glyphY = 0;

            // Loop through each pixel in the first glyph cell
            for (int row = 0; row < cellHeight; row++)
            {
                for (int col = 0; col < cellWidth; col++)
                {
                    int pixelX = glyphX + col;
                    int pixelY = glyphY + row;
                    int pixelIndex = pixelY * pngWidth + pixelX;
                    byte pixelData = pngPixelData[pixelIndex];

                    // Add the color index to the set of transparent colors
                    transparentColors.Add(pixelData);
                }
            }
        }

        /// <summary>
        /// Calculates the maximum glyph width among all glyphs.
        /// </summary>
        /// <returns>The maximum glyph width.</returns>
        private byte GetMaxGlyphWidth()
        {
            byte maxWidth = 0;
            for (int i = 0; i < glyphWidth.Length; i++)
            {
                if (glyphWidth[i] > maxWidth)
                {
                    maxWidth = glyphWidth[i];
                }
            }
            return maxWidth;
        }

        /// <summary>
        /// Calculates the maximum glyph width among alphabetic characters (uppercase and lowercase letters).
        /// </summary>
        /// <returns>The maximum width of the alphanumeric glyphs.</returns>
        public byte GetMaxGlyphWidthForAlphabetic()
        {
            byte maxWidth = 0;

            // Iterate through uppercase letters 'A' to 'Z' (indices 33 to 58).
            // Indices correspond to ASCII codes offset by 32 (space character).
            for (int i = 33; i <= 58; i++)
            {
                if (glyphWidth[i] > maxWidth)
                {
                    maxWidth = glyphWidth[i];
                }
            }

            // Iterate through lowercase letters 'a' to 'z' (indices 65 to 90).
            for (int i = 65; i <= 90; i++)
            {
                if (glyphWidth[i] > maxWidth)
                {
                    maxWidth = glyphWidth[i];
                }
            }

            return maxWidth;    // Returns the maximum width found.
        }

        /// <summary>
        /// Converts each glyph's pixel data into a list of byte arrays representing the glyphs in bit format.
        /// </summary>
        /// <returns>A list of byte arrays, each representing a glyph's bitmap data.</returns>
        public List<byte[]> ConvertFontToBitData()
        {
            // Create a list to store the bit data for each glyph.
            List<byte[]> glyphDataList = new();

            // Loop through each glyph in the font image grid (total of 96 glyphs).
            for (int glyphIndex = 0; glyphIndex < 96; glyphIndex++)
            {
                // Calculate the top-left pixel coordinates (glyphX, glyphY) of the glyph in the image.
                int glyphX = (glyphIndex % 16) * cellWidth;     // Column position.
                int glyphY = (glyphIndex / 16) * cellHeight;    // Row position.

                // Calculate the number of bytes needed per row to represent the glyph's width in bits.
                int bytesPerRow = (int)Math.Ceiling(cellWidth / 8.0);

                // Initialize a byte array to store the glyph's bitmap data.
                byte[] glyphData = new byte[bytesPerRow * cellHeight];

                // Loop through each row of the glyph.
                for (int row = 0; row < cellHeight; row++)
                {
                    int byteIndex = row * bytesPerRow;  // Index in glyphData where this row's data starts.
                    int bitPosition = 7;                // Start with the most significant bit in the byte.

                    // Loop through each column (pixel) in the row.
                    for (int col = 0; col < cellWidth; col++)
                    {
                        int pixelX = glyphX + col; // X position in the PNG
                        int pixelY = glyphY + row; // Y position in the PNG
                        int pixelIndex = pixelY * pngWidth + pixelX; // Calculate the index in the linear pixel array

                        // Check if the pixel is filled or transparent.
                        byte pixelData = pngPixelData[pixelIndex];
                        bool isFilledPixel = !transparentColors.Contains(pixelData);

                        // Set the corresponding bit in glyphData.
                        if (isFilledPixel)
                        {
                            glyphData[byteIndex] |= (byte)(1 << bitPosition);
                        }

                        // Move to the next bit.
                        bitPosition--;

                        // If we have processed 8 bits, move to the next byte.
                        if (bitPosition < 0)
                        {
                            bitPosition = 7;    // Reset bit position for the next byte.
                            byteIndex++;        // Move to the next byte in glyphData.
                        }
                    }
                }

                // Add the glyph's bitmap data to the list.
                glyphDataList.Add(glyphData);
            }

            return glyphDataList;
        }

        /// <summary>
        /// Flattens the list of glyph bitmap data into a single byte array, excluding the space character.
        /// </summary>
        /// <param name="glyphDataList">The list of byte arrays containing glyph bitmap data.</param>
        /// <returns>A single byte array containing all glyph data concatenated.</returns>
        public byte[] FlattenGlyphData(List<byte[]> glyphDataList)
        {
            // Calculate the total size of the flattened array by summing the sizes of glyph data.
            int totalSize = 0;

            // Exclude the first glyph (space character) from the flattened data
            // as it contains no visible pixels.
            for (int i = 1; i < glyphDataList.Count; i++)
            {
                totalSize += glyphDataList[i].Length;
            }

            // Create the flattened array.
            byte[] flattenedArray = new byte[totalSize];

            // Copy each glyph's data into the flattened array, starting from the current position.
            int currentPosition = 0;
            for (int i = 1; i < glyphDataList.Count; i++)   // Start at 1 to skip the first entry (space).
            {
                Array.Copy(glyphDataList[i], 0, flattenedArray, currentPosition, glyphDataList[i].Length);
                currentPosition += glyphDataList[i].Length;
            }

            return flattenedArray;
        }

        /// <summary>
        /// Gets the raw font data in the specified format.
        /// </summary>
        /// <returns>A byte array containing the font data.</returns>
        public byte[] GetRawFontData()
        {
            // Step 1: Font type (default 1)
            byte fontType = 1;

            // Step 2: Glyph cell width (2-255)
            byte glyphCellWidth = (byte)cellWidth;
            if (glyphCellWidth < 2)
            {
                throw new Exception("Invalid glyph cell width. Must be between 2 and 255.");
            }

            // Step 3: Glyph cell height (2-255)
            byte glyphCellHeight = (byte)cellHeight;
            if (glyphCellHeight < 2)
            {
                throw new Exception("Invalid glyph cell height. Must be between 2 and 255.");
            }

            // Step 4: Max glyph width (non-zero)
            byte maxGlyphWidth = GetMaxGlyphWidth();
            if (maxGlyphWidth == 0)
            {
                throw new Exception("Invalid max glyph width. Must be greater than 0.");
            }

            // Step 5: Max glyph height (non-zero)
            byte maxGlyphHeight = glyphCellHeight; // Since all glyphs have the same height

            // Step 6: Number of kerning pairs (nk value)
            // nk = length of kerningData divided by 3
            byte nk = (byte)(kerningData.Length / 3);

            // Step 7: Actual font data (already stored in fontData)

            // Calculate total size of the output array
            int totalSize = 6 + 96 + kerningData.Length + fontData.Length;
            byte[] output = new byte[totalSize];

            int offset = 0;

            // Write data to the output array

            // 1 byte: font type
            output[offset++] = fontType;

            // 1 byte: glyph cell width
            output[offset++] = glyphCellWidth;

            // 1 byte: glyph cell height
            output[offset++] = glyphCellHeight;

            // 1 byte: max glyph width
            output[offset++] = maxGlyphWidth;

            // 1 byte: max glyph height
            output[offset++] = maxGlyphHeight;

            // 1 byte: number of kerning pairs
            output[offset++] = nk;

            // 96 bytes: width of each character starting with space
            Array.Copy(glyphWidth, 0, output, offset, glyphWidth.Length);
            offset += glyphWidth.Length;

            // nk * 3 bytes: kerning data
            if (kerningData.Length > 0)
            {
                Array.Copy(kerningData, 0, output, offset, kerningData.Length);
                offset += kerningData.Length;
            }

            // Remaining bytes: actual font data
            Array.Copy(fontData, 0, output, offset, fontData.Length);

            // Return the constructed byte array
            return output;
        }

        /// <summary>
        /// Clears the pixel data and glyph width information to free up memory.
        /// </summary>
        public void ClearData()
        {
            pngPixelData = null;
            glyphWidth = null;
            fontData = null;
            kerningData = null;
            transparentColors = null;
        }
    }
}
