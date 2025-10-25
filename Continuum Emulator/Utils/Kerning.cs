using System.Collections.Generic;
using System.IO;

namespace Continuum93.Utils
{
    public class Kerning
    {
        // Data structure to store kerning pairs and their values
        private readonly List<(byte Char1, byte Char2, sbyte Value)> kerningPairs;

        public Kerning(string filePath)
        {
            kerningPairs = new List<(byte, byte, sbyte)>();

            // Check if the file exists
            if (!File.Exists(filePath))
            {
                // Handle the case where the file doesn't exist (optional: throw an exception or log a warning)
                return;
            }

            // Process the file line by line
            using var reader = new StreamReader(filePath);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                // Ignore empty lines
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                // Remove any leading and trailing whitespaces
                line = line.Trim();

                // Ignore lines starting with '#' (comments)
                if (line.StartsWith("#"))
                    continue;

                // Find the index of the colon separator
                int colonIndex = line.IndexOf(':');
                if (colonIndex == -1)
                {
                    // Invalid line format (no colon found), ignore this line
                    continue;
                }

                // Extract the character pair and the value string
                string pair = line[..colonIndex].Trim();
                string valueString = line[(colonIndex + 1)..].Trim();

                // Ensure the pair consists of exactly two characters
                if (pair.Length != 2)
                    continue;

                char char1 = pair[0];
                char char2 = pair[1];

                // Check if both characters are printable ASCII characters (32 to 126)
                if (char1 < 32 || char1 > 127 || char2 < 32 || char2 > 127)
                    continue;

                // Parse the value as a signed byte
                if (!sbyte.TryParse(valueString, out sbyte value))
                    continue;

                // Ignore kerning pairs with a value of zero
                if (value == 0)
                    continue;

                // Store the valid kerning pair and its value
                kerningPairs.Add(((byte)char1, (byte)char2, value));
            }
        }

        /// <summary>
        /// Returns all kerning pairs and their values encoded as bytes.
        /// Each entry consists of three bytes: [char1, char2, value].
        /// </summary>
        /// <returns>A byte array containing the kerning data.</returns>
        public byte[] GetKerningBytes()
        {
            byte[] result = new byte[kerningPairs.Count * 3];

            for (int i = 0; i < kerningPairs.Count; i++)
            {
                var (char1, char2, value) = kerningPairs[i];
                result[i * 3] = char1;
                result[i * 3 + 1] = char2;
                result[i * 3 + 2] = unchecked((byte)value); // Store the signed byte as an unsigned byte
            }

            return result;
        }

        /// <summary>
        /// Gets the total number of kerning pairs.
        /// </summary>
        public int KerningPairCount => kerningPairs.Count;
    }
}
