using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;
using ContinuumUnitTests.Utils;
using ContinuumUnitTests.Utils.DataModels;

namespace Interrupts
{
    public class TestInterrupt_LoadPNGFont
    {
        /// <summary>
        /// Unit test for verifying the correct loading and parsing of a specially designed PNG font.
        ///
        /// **Purpose:**
        /// This test ensures that a PNG font file is correctly loaded into memory, parsed, and that the glyphs
        /// have the expected pixel data after being processed by the emulator. The font used in this test is
        /// specifically designed with known glyphs and pixel patterns to facilitate precise verification.
        ///
        /// **Test Steps:**
        /// 1. **Assembly and Execution:**
        ///    - Assembles a small program that loads a PNG font file ("fonts\\Test font 1.png") into memory
        ///      at a specified address (e.g., address `20000`).
        ///    - The program uses an interrupt (`INT 0x04`) to perform the font loading operation.
        ///    - Executes the assembled program on a simulated computer/emulator.
        ///
        /// 2. **Font Data Retrieval:**
        ///    - After execution, retrieves the font data from the specified memory location.
        ///    - Calculates the expected size of the font data using utility methods.
        ///
        /// 3. **Font Parsing:**
        ///    - Constructs a `PNGFont` object by parsing the retrieved font data.
        ///    - The `PNGFont` class interprets the raw font data and provides access to font properties and glyphs.
        ///
        /// 4. **Assertions:**
        ///    - Verifies that the font type and total bytes per glyph match the expected values (e.g., font type `1`, total bytes per glyph `20`).
        ///    - Defines expected numbers of filled pixels for specific ranges of glyph indices based on the known design of the test font.
        ///    - Iterates over the glyphs and asserts that each glyph has the expected number of filled pixels.
        ///
        /// 5. **Specific Pixel Verification:**
        ///    - For glyphs in the first row, checks that specific pixels are set at expected positions.
        ///    - For example, verifies that in glyph `i`, the pixel at position `(9 - i, 0)` is set, ensuring that the pixels are in the right places.
        ///
        /// **Key Points:**
        /// - The test font is deliberately designed so that certain glyphs have a known number of filled pixels and specific pixel patterns.
        /// - By verifying both the overall filled pixel counts and the state of individual pixels, the test provides comprehensive validation of the font loading and parsing logic.
        /// - This test helps ensure that the emulator correctly interprets the PNG font data and that the glyphs are accurately represented in memory.
        /// </summary>
        [Fact]
        public void TestLoadingPNGFont()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD A, 0x40
                LD BCD, .FontPath
                LD EFG, 20000
                INT 0x04, A

                BREAK

            .FontPath
                #DB " + "\"fonts\\Test font 1.png\"" + @", 0
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            int expectedFontSize = PNGFontUtils.CalculatePNGFontSize(10, 10);

            byte[] expectedFontHeader = computer.GetMemFrom(20000, 6);

            byte kerningPairs = expectedFontHeader[5];
            expectedFontSize += kerningPairs * 3;

            // Verify whether the interrupt correctly returns the font size in bytes
            Assert.Equal((uint)expectedFontSize, computer.CPU.REGS.ABC);

            byte[] expectedFontData = computer.GetMemFrom(20000, (uint)expectedFontSize);

            PNGFont expectedFont = new(expectedFontData);

            Assert.Equal(1, expectedFont.GetFontType());
            Assert.Equal(20, expectedFont.GetTotalBytesPerGlyph());

            // Verify kernings
            Assert.Equal(21, expectedFont.GetKerningCount());

            var expectedKerningPairs = new List<(char firstGlyph, char secondGlyph, sbyte offset)>
            {
                ('F', 'a', -2),
                ('F', 'e', -1),
                ('F', 'i', -1),
                ('F', 'o', -2),
                ('F', 'u', -1),
                ('F', 'r', -1),
                ('F', 'w', -1),
                ('J', 'a', -1),
                ('J', 'o', -1),
                ('L', 'T', -2),
                ('T', 'a', -1),
                ('T', 'e', -1),
                ('T', 'i', -1),
                ('T', 'o', -1),
                ('T', 'u', -1),
                ('T', 'r', -1),
                ('T', 'w', -1),
                ('e', 'l', -1),
                ('i', 'f', -1),
                ('i', 't', -1),
                ('l', 'o', -1),
            };

            for (byte index = 0; index < expectedKerningPairs.Count; index++)
            {
                KerningPair kerning = expectedFont.GetKerning(index);
                var (firstGlyph, secondGlyph, offset) = expectedKerningPairs[index];

                Assert.Equal((byte)firstGlyph, kerning.FirstGlyph);
                Assert.Equal((byte)secondGlyph, kerning.SecondGlyph);
                Assert.Equal(offset, kerning.Offset);
            }

            // Define expected filled pixels for glyph ranges
            var expectedGlyphRanges = new List<(int StartIndex, int EndIndex, byte ExpectedFilledPixels)>
            {
                (0, 9, 1),      // First row
                (10, 14, 0),
                (15, 23, 2),    // Second row
                (24, 30, 0),
                (31, 38, 3),    // Third row
                (39, 46, 0),
                (47, 53, 4),    // Fourth row
                (54, 62, 0),
                (63, 68, 5),    // Fifth row
                (69, 78, 0),
                (79, 83, 6),    // Sixth row
                (84, 93, 0),
                (94, 94, 19)    // Special glyph at index 94
            };

            // Initialize expected filled pixels array
            byte[] expectedFilledPixels = new byte[95]; // Glyph indices from 0 to 94

            foreach (var (startIndex, endIndex, expectedPixels) in expectedGlyphRanges)
            {
                for (int i = startIndex; i <= endIndex; i++)
                {
                    expectedFilledPixels[i] = expectedPixels;
                }
            }

            // Assert the filled pixels for each glyph
            for (byte i = 0; i < expectedFilledPixels.Length; i++)
            {
                Assert.Equal(expectedFilledPixels[i], expectedFont.GetGlyph(i).GetFilledPixels());
            }

            // Check specific pixels on the first row glyphs
            for (byte i = 0; i < 10; i++)
            {
                Assert.True(expectedFont.GetGlyph(i).IsPixelSetAt(9 - i, 0));
            }

            TUtils.IncrementCountedTests("interrupts");
        }
    }
}
