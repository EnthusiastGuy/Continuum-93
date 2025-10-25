namespace ContinuumUnitTests.Utils.DataModels
{
    /// <summary>
    /// Represents a font extracted from a PNG image containing glyph data, including glyph sizes,
    /// kerning information, and the bitmap representation of each glyph.
    ///
    /// **Class Purpose:**
    /// - The `PNGFont` class is designed to handle the storage and interpretation of font data
    ///   that has been extracted from a PNG image file, typically using the `GFXFontExtractor`.
    /// - This font includes all metadata needed for rendering, such as glyph sizes, kerning pairs,
    ///   and the pixel data of each glyph in a boolean array format.
    ///
    /// **Fields and Properties:**
    /// - **Font Metadata:**
    ///   - `fontType`, `glyphCellWidth`, `glyphCellHeight`, `maxGlyphWidth`, `maxGlyphHeight`, `kerningCount`:
    ///     Represents the various attributes that define the general structure of the font.
    /// - **Glyph Data:**
    ///   - `glyphWidths`: An array representing the individual width of each glyph.
    ///   - `glyphs`: A list containing the detailed pixel information for each glyph in the font, stored in `FontGlyph` objects.
    /// - **Kerning Data:**
    ///   - `kernings`: Stores kerning pairs that help adjust the spacing between particular character combinations.
    /// 
    /// **Constructor:**
    /// - The `PNGFont(byte[] data)` constructor initializes the `PNGFont` object by interpreting a given byte array.
    ///   - This byte array contains all the font data, including metadata, kerning information, and glyph data.
    ///   - It extracts glyph and kerning information, processes the glyph bitmap data into individual `FontGlyph` objects,
    ///     and validates that the data structure conforms to the expected format.
    /// 
    /// **Methods:**
    /// - Getter methods (`GetFontType`, `GetGlyphCellWidth`, `GetTotalBytesPerGlyph`, etc.) provide access to key metadata.
    /// - `GetGlyph(byte index)`: Retrieves a specific glyph based on its index.
    /// - The `FontGlyph` class nested within holds the pixel data for each individual glyph, represented as a 2D boolean array.
    /// 
    /// **Key Features:**
    /// - **Kerning Pairs:**
    ///   - Kerning pairs are represented by the `KerningPair` class and provide adjustments to spacing between glyphs,
    ///     which improves the visual quality of text rendering.
    /// - **Bit-Packed Glyph Data:**
    ///   - Each glyph's bitmap data is packed in bits to reduce the memory footprint.
    ///   - The `FontGlyph` constructor decodes this bit-packed data into a boolean array (`pixels`) representing each pixel of the glyph.
    /// - **Validation:**
    ///   - The class validates the font data, ensuring the correct number of bytes is provided for kerning pairs, glyph widths, and glyph data.
    /// 
    /// **Usage:**
    /// - This class is primarily used to manage the font data extracted from a PNG image file, providing the necessary methods
    ///   to access individual glyphs, their dimensions, and rendering information for use in an emulator or rendering engine.
    /// 
    /// **Exception Handling:**
    /// - The constructor includes error handling to verify the data length, kerning count, and glyph data length, throwing
    ///   `ArgumentException` when the provided data is not in the expected format.
    /// </summary>
    public class PNGFont
    {
        byte fontType;
        byte glyphCellWidth;
        byte glyphCellHeight;
        byte maxGlyphWidth;
        byte maxGlyphHeight;
        byte kerningCount;
        byte bytesPerGlyphWidth;
        ushort totalBytesPerGlyph;

        byte[] glyphWidths = new byte[96];
        List<KerningPair> kernings = new();
        List<FontGlyph> glyphs = new();
        
        public byte GetFontType() { return fontType; }
        public byte GetGlyphCellWidth() { return glyphCellWidth; }
        public byte GetGlyphCellHeight() { return glyphCellHeight; }
        public byte GetKerningCount() { return kerningCount; }
        public byte GetBytesPerGlyphWidth() { return bytesPerGlyphWidth; }
        public ushort GetTotalBytesPerGlyph() { return totalBytesPerGlyph; }

        public FontGlyph GetGlyph(byte index) { return glyphs[index]; }

        public KerningPair GetKerning(byte index) { return kernings[index]; }


        public PNGFont(byte[] data)
        {
            if (data.Length < 6 + 96 + 96)
            {
                throw new ArgumentException("Invalid font size");
            }

            fontType = data[0];
            glyphCellWidth = data[1];
            glyphCellHeight = data[2];
            maxGlyphWidth = data[3];
            maxGlyphHeight = data[4];
            kerningCount = data[5];

            if (kerningCount > 254)
            {
                throw new ArgumentException("Invalid kerning table size. Maximum number of entries is 254");
            }

            bytesPerGlyphWidth = (byte)Math.Ceiling(glyphCellWidth / 8.0);
            totalBytesPerGlyph = (ushort)(bytesPerGlyphWidth * glyphCellHeight);

            int currentIndex = 6;

            Array.Copy(data, currentIndex, glyphWidths, 0, 96);  // Take 96 bytes from where the kernings ended
            currentIndex += 96;

            for (byte i = 0; i < kerningCount; i++)
            {
                byte[] kerning = new byte[3];
                Array.Copy(data, currentIndex, kerning, 0, 3);
                kernings.Add(new KerningPair(kerning));

                currentIndex += 3;
            }

            for (byte i = 0; i < 95; i++)   // We ignore space
            {
                byte[] glyphData = new byte[totalBytesPerGlyph];

                Array.Copy(data, currentIndex, glyphData, 0, totalBytesPerGlyph);
                currentIndex += totalBytesPerGlyph;

                glyphs.Add(new FontGlyph(glyphCellWidth, glyphCellHeight, bytesPerGlyphWidth, totalBytesPerGlyph, glyphData));
            }
        }
    }

    public class KerningPair
    {
        public byte FirstGlyph;
        public byte SecondGlyph;
        public sbyte Offset;

        public KerningPair(byte[] data)
        {
            if (data.Length != 3) { throw new ArgumentException("Invalid kerning format"); }

            FirstGlyph = data[0];
            SecondGlyph = data[1];
            Offset = (sbyte)data[2];
        }
    }

    public class FontGlyph
    {
        byte glyphWidth;
        byte glyphHeight;
        bool[,] pixels;
        byte bytesPerGlyphWidth;
        ushort totalBytesPerGlyph;
        int filledPixels;
        int emptyPixels;

        public int GetFilledPixels() { return filledPixels; }
        public int GetEmptyPixels() { return emptyPixels; }
        public bool IsPixelSetAt(int x, int y)
        {
            return pixels[y, x];
        }

        public FontGlyph(byte width, byte height, byte bpgWidth, ushort tbpGlyph, byte[] data)
        {
            glyphWidth = width;
            glyphHeight = height;
            pixels = new bool[glyphHeight, glyphWidth];
            bytesPerGlyphWidth = bpgWidth;
            totalBytesPerGlyph = tbpGlyph;
            filledPixels = 0;
            emptyPixels = 0;

            // Validate the data length
            if (data.Length != totalBytesPerGlyph)
            {
                throw new ArgumentException("Invalid glyph data length");
            }

            // Iterate over each row (y-axis)
            for (int y = 0; y < glyphHeight; y++)
            {
                // Calculate the starting index for this row in the data array
                int rowDataStartIndex = y * bytesPerGlyphWidth;

                // Iterate over each column (x-axis)
                for (int x = 0; x < glyphWidth; x++)
                {
                    // Calculate the byte index and bit index within that byte
                    int byteIndex = rowDataStartIndex + (x / 8);
                    int bitIndex = 7 - (x % 8); // Bits are stored from MSB to LSB

                    // Get the byte from the data array
                    byte currentByte = data[byteIndex];

                    // Extract the bit at the bitIndex position
                    bool isFilled = (currentByte & (1 << bitIndex)) != 0;

                    // Set the pixel in the pixels array
                    pixels[y, x] = isFilled;

                    // Update the filled and empty pixels count
                    if (isFilled)
                    {
                        filledPixels++;
                    }
                    else
                    {
                        emptyPixels++;
                    }
                }
            }
        }
    }
}
