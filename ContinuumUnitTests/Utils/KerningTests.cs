using Continuum93.Utils;

namespace MiscUtils
{
    public class KerningTests
    {
        [Fact]
        public void Kerning_GetKerningBytes_ReturnsCorrectData()
        {
            // Assume the test file exists at this path
            string testFilePath = "test_kerning.txt";

            // For the purpose of the test, create the file locally.
            CreateTestKerningFile(testFilePath);

            // Initialize the Kerning class with the test file path
            Kerning kerning = new(testFilePath);

            // Get the kerning bytes
            byte[] kerningData = kerning.GetKerningBytes();



            // Expected data:
            // "F", "a", -2
            // "F", "e", -1
            // "F", "i", 3
            // "A", "$", -5
            // "A", "B", -2
            // Encoded as bytes:
            // [70, 97, 254] // -2 as unsigned byte is 254
            // [70, 101, 255] // -1 as unsigned byte is 255
            // [70, 105, 3]
            // [65, 36, 251]
            // [65, 66, 254]

            byte[] expectedData = new byte[]
            {
                (byte)'F', (byte)'a', unchecked((byte)(sbyte)-2),
                (byte)'F', (byte)'e', unchecked((byte)(sbyte)-1),
                (byte)'F', (byte)'i', (byte)3,
                (byte)'A', (byte)'$', unchecked((byte)(sbyte)-5),
                (byte)'A', (byte)'B', unchecked((byte)(sbyte)-2)
            };

            // Assert that the kerning data matches the expected data
            Assert.Equal(expectedData, kerningData);

            // Clean up the test file (optional)
            File.Delete(testFilePath);
        }

        // Helper method to create the test kerning file
        private static void CreateTestKerningFile(string filePath)
        {
            string[] lines = new string[]
            {
                "# This is a comment",
                "Fa: -2",
                "Fe: -1",
                "Fi: 3",
                "Fx: 0",
                "InvalidLine",
                "A$: -5",
                "AB: 128",
                "AB: -2",
                "Non-ASCII: -1"
            };

            File.WriteAllLines(filePath, lines);
        }
    }
}
