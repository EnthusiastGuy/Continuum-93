using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace Continuum93.Emulator.GraphicsAccelerators
{

    public class PNGContainer
    {
        private static readonly byte[] PNG_SIGNATURE =
            { 137, (byte)'P', (byte)'N', (byte)'G', 13, 10, 26, 10 };

        private const string IhdrChunkType = "IHDR";
        private const string PlteChunkType = "PLTE";
        private const string IdatChunkType = "IDAT";
        private const string TrnsChunkType = "tRNS";

        /* The IHDR chunk must appear FIRST. It contains:
               Width:              4 bytes
               Height:             4 bytes
               Bit depth:          1 byte
               Color type:         1 byte
                    Color    Allowed    Interpretation
                    Type    Bit Depths
                    0       1,2,4,8,16  Each pixel is a grayscale sample.
                    2       8,16        Each pixel is an R,G,B triple.
                    3       1,2,4,8     Each pixel is a palette index;
                                        a PLTE chunk must appear.
                    4       8,16        Each pixel is a grayscale sample,
                                        followed by an alpha sample.
                    6       8,16        Each pixel is an R,G,B triple,
                                        followed by an alpha sample.
               Compression method: 1 byte
               Filter method:      1 byte
               Interlace method:   1 byte   */
        private byte[] ihdrData;
        private byte[] plteData;
        private byte[] trnsData;
        private byte[] idatData;
        private readonly List<byte[]> idatDataList = new();

        private readonly string name;
        private int width;
        private int height;
        private byte bitDepth;
        private byte colorType;
        private byte compressionMethod;
        private byte filterMethod;
        private byte interlaceMethod;
        private byte errorCode;
        private byte colorCount;

        private byte[] pixelData;

        public PNGContainer(string path)
        {
            name = Path.GetFileName(path);
            Extract(path);
        }

        public PNGContainer()
        {

        }

        public string Name => name;
        public int Width => width;
        public int Height => height;
        public int BitDepth => bitDepth;
        public byte ColorType => colorType;
        public byte CompressionMethod => compressionMethod;
        public byte FilterMethod => filterMethod;
        public byte InterlaceMethod => interlaceMethod;
        public byte[] PixelData => pixelData;
        public byte[] TransparencyData => trnsData;
        public byte[] PaletteData => plteData;
        public byte ErrorCode => errorCode;
        public byte ColorCount => colorCount;

        public byte GetColorCount()
        {
            return (byte)(plteData.Length / 3);
        }

        private void Extract(string path)
        {
            if (!File.Exists(path))
            {
                errorCode = 1;  // File not found
                return;
            }
            using FileStream fs = new(path, FileMode.Open, FileAccess.Read);

            // Read and verify PNG signature
            byte[] signature = new byte[8];
            fs.Read(signature, 0, 8);

            bool isPNG = IsPngSignature(signature);

            if (!isPNG)
            {
                //Console.WriteLine("Not a PNG file.");
                errorCode = 2;  // Not a PNG file
                return;
            }

            // Continue reading chunks until PLTE or IDAT is found
            while (fs.Position < fs.Length)
            {
                // Read chunk length
                byte[] lengthBytes = new byte[4];
                fs.Read(lengthBytes, 0, 4);
                Array.Reverse(lengthBytes); // PNG uses big-endian
                int length = BitConverter.ToInt32(lengthBytes, 0);

                // Read chunk type
                byte[] typeBytes = new byte[4];
                fs.Read(typeBytes, 0, 4);
                string type = Encoding.ASCII.GetString(typeBytes);

                // Prepare a buffer for the chunk data
                byte[] chunkData = new byte[length];
                fs.Read(chunkData, 0, length); // Read the chunk data

                if (type == IhdrChunkType)
                {
                    ihdrData = chunkData;
                    ReadHeaderData();
                }
                else if (type == PlteChunkType)
                {
                    plteData = chunkData;
                    colorCount = (byte)(plteData.Length / 3);
                }
                else if (type == TrnsChunkType)
                {
                    trnsData = chunkData;
                }
                else if (type == IdatChunkType)
                {
                    idatDataList.Add(chunkData);
                }
                else
                {
                    //Debug.WriteLine($"Ignoring '{type}' chunk.");
                }

                // Skip the chunk data and CRC
                fs.Seek(4, SeekOrigin.Current);
            }

            ValidatePNG();
            MatchTransparencyValues();
            if (errorCode != 0)
            {
                return;
            }

            GenerateData();
        }

        private void ValidatePNG()
        {
            if (width > 65535 || height > 65535)
            {
                errorCode = 3;  // Width or height over 65535
                return;
            }

            if (bitDepth != 8 && bitDepth != 4 && bitDepth != 2 && bitDepth != 1)
            {
                errorCode = 4;  // Bit depth is not 8
                return;
            }

            if (colorType != 3)
            {
                errorCode = 5;  // Not a palette based PNG
                return;
            }

            if (compressionMethod != 0)
            {
                errorCode = 6;  // Unknown compression method
                return;
            }

            if (filterMethod != 0)
            {
                errorCode = 7;  // Unknown filtering method
                return;
            }

            if (interlaceMethod != 0)
            {
                errorCode = 8;  // Interlacing not supported
                return;
            }

            if (colorCount == 0)
            {
                errorCode = 9;
                return;
            }
        }

        private void MatchTransparencyValues()
        {
            byte[] transparency = new byte[colorCount];
            for (short i = 0; i < transparency.Length; i++)
                transparency[i] = 255;

            if (trnsData != null)
            {
                if (colorCount == trnsData.Length)
                    return;

                Array.Copy(trnsData, transparency, trnsData.Length);
            }

            trnsData = transparency;
        }

        public void SetPalette(byte[] palette)
        {
            plteData = palette;
            colorCount = (byte)(plteData.Length / 3);
            MatchTransparencyValues();
        }

        public void SetPixelData(byte[] data)
        {
            pixelData = data;
        }

        public void GenerateData()
        {
            byte[] concatenatedIdatData = ConcatenateIdatDataAndClearList();
            idatData = DecompressConcatenatedIdatData(concatenatedIdatData);
            concatenatedIdatData = null;

            SetPixelData();
            idatData = null;
        }

        public byte[] ConcatenateIdatDataAndClearList()
        {
            // Determine the total length of all IDAT data
            int totalLength = 0;
            foreach (var data in idatDataList)
            {
                totalLength += data.Length;
            }

            // Create a new array to hold all concatenated IDAT data
            byte[] concatenatedData = new byte[totalLength];

            // Copy each array from the list into the concatenated array
            int currentPosition = 0;
            foreach (var data in idatDataList)
            {
                Array.Copy(data, 0, concatenatedData, currentPosition, data.Length);
                currentPosition += data.Length;
            }

            // Clear the list to free up memory
            idatDataList.Clear();

            // Return the concatenated IDAT data
            return concatenatedData;
        }

        public static byte[] DecompressConcatenatedIdatData(byte[] concatenatedIdatData)
        {
            using var compressedStream = new MemoryStream(concatenatedIdatData);
            compressedStream.Seek(2, SeekOrigin.Begin); // Skip the 2-byte zlib header at the start of the IDAT data.

            // Create a deflate stream for decompression.
            // Note: Do not use CompressionMode.Decompress directly because it expects a zlib header, which we've manually skipped.
            using var deflateStream = new DeflateStream(compressedStream, CompressionMode.Decompress);
            // Create a memory stream to hold the decompressed data.
            using var decompressedStream = new MemoryStream();
            // Copy the decompressed data from the deflate stream to the decompressed stream.
            deflateStream.CopyTo(decompressedStream);

            // Return the decompressed data as a byte array.
            return decompressedStream.ToArray();
        }

        public void ApplyTransparencyCorrection(byte r, byte g, byte b, byte a)
        {
            for (int i = 0; i < TransparencyData.Length; i++)
            {
                // Check if the current palette entry matches the provided RGBA values
                if (PaletteData[i * 3] == r && PaletteData[i * 3 + 1] == g && PaletteData[i * 3 + 2] == b && TransparencyData[i] == a)
                {
                    // Swap the first palette entry with the matching entry
                    byte tempR = PaletteData[0];
                    byte tempG = PaletteData[1];
                    byte tempB = PaletteData[2];
                    PaletteData[0] = r;
                    PaletteData[1] = g;
                    PaletteData[2] = b;
                    // This below inverts some palette colors. Why?
                    // Because we assume that the color there is a valid color used somewhere else
                    // 6 months later, I'd love to understand what you two are talking about there...
                    PaletteData[i * 3] = tempR;
                    PaletteData[i * 3 + 1] = tempG;
                    PaletteData[i * 3 + 2] = tempB;

                    // Adjust the pixel data
                    for (int j = 0; j < PixelData.Length; j++)
                    {
                        if (PixelData[j] == 0)
                        {
                            PixelData[j] = (byte)i; // Replace value 0 with the index i
                        }
                        else if (PixelData[j] == i)
                        {
                            PixelData[j] = 0; // Replace value i with 0, as we swapped the palette entries
                        }
                    }

                    break; // Exit the loop as we've found and processed the matching entry
                }
            }
        }

        public void MergePalette(byte[] externalPalette)
        {
            // Initialize a list to hold the merged palette and start with the external palette
            List<byte> mergedPalette = externalPalette.ToList();

            // Dictionary to map old indices to new indices
            Dictionary<byte, byte> indexMap = new();

            // Iterate through each color in the existing palette
            for (int i = 0; i < plteData.Length; i += 3)
            {
                byte r = plteData[i];
                byte g = plteData[i + 1];
                byte b = plteData[i + 2];

                // Check if this color exists in the external palette
                bool found = false;
                for (int j = 3; j < externalPalette.Length; j += 3)
                {
                    if (externalPalette[j] == r && externalPalette[j + 1] == g && externalPalette[j + 2] == b)
                    {
                        // Color found, map old index to new index
                        indexMap[(byte)(i / 3)] = (byte)(j / 3);
                        found = true;
                        break;
                    }
                }

                // If the color was not found in the external palette, add it
                if (!found)
                {
                    indexMap[(byte)(i / 3)] = (byte)(mergedPalette.Count / 3);
                    mergedPalette.AddRange(new byte[] { r, g, b });
                }
            }

            // Now, update the PixelData based on the indexMap
            for (int i = 0; i < pixelData.Length; i++)
            {
                if (indexMap.ContainsKey(pixelData[i]))
                {
                    pixelData[i] = indexMap[pixelData[i]];
                }
            }

            // Replace the merged palette
            SetPalette(mergedPalette.ToArray());
        }

        public void MergePaletteWithTransparent(byte[] externalPalette, byte tr, byte tg, byte tb, byte ta)
        {
            // Initialize a list to hold the merged palette and start with the external palette
            List<byte> mergedPalette = externalPalette.ToList();

            if (mergedPalette.Count == 0)
            {
                mergedPalette.AddRange(new List<byte>() { 0, 0, 0 });
            }

            // Dictionary to map old indices to new indices
            Dictionary<byte, byte> indexMap = new();

            // Index for transparent color, if found
            byte transparentIndex = 0;
            bool transparentColorFound = false;

            // Iterate through each color in the existing palette
            for (int i = 0; i < plteData.Length; i += 3)
            {
                byte r = plteData[i];
                byte g = plteData[i + 1];
                byte b = plteData[i + 2];
                byte a = (i / 3) < trnsData.Length ? trnsData[i / 3] : (byte)255;

                // Check if this color matches the specified transparent color
                if (r == tr && g == tg && b == tb && a == ta)
                {
                    transparentIndex = (byte)(i / 3);
                    transparentColorFound = true;
                    continue; // Skip adding this color to the merged palette
                }

                // Check if this color exists in the external palette (excluding the first color)
                bool found = false;
                for (int j = 3; j < externalPalette.Length; j += 3)
                {
                    if (externalPalette[j] == r && externalPalette[j + 1] == g && externalPalette[j + 2] == b)
                    {
                        // Color found, map old index to new index
                        indexMap[(byte)(i / 3)] = (byte)(j / 3);
                        found = true;
                        break;
                    }
                }

                // If the color was not found in the external palette, add it
                if (!found)
                {
                    indexMap[(byte)(i / 3)] = (byte)(mergedPalette.Count / 3);
                    mergedPalette.AddRange(new byte[] { r, g, b });
                }
            }

            // Update the PixelData based on the indexMap, set references to the transparent color to zero
            for (int i = 0; i < pixelData.Length; i++)
            {
                if (transparentColorFound && pixelData[i] == transparentIndex)
                {
                    pixelData[i] = 0; // Set to zero for the transparent color
                }
                else if (indexMap.ContainsKey(pixelData[i]))
                {
                    pixelData[i] = indexMap[pixelData[i]];
                }
            }

            // Replace the merged palette, excluding the specified transparent color
            SetPalette(mergedPalette.ToArray());
        }

        public void ClearData()
        {
            ihdrData = null;
            plteData = null;
            trnsData = null;
            idatData = null;
            pixelData = null;
            idatDataList.Clear();
        }

        private void ReadHeaderData()
        {
            width = (ihdrData[0] << 24) | (ihdrData[1] << 16) | (ihdrData[2] << 8) | ihdrData[3];
            height = (ihdrData[4] << 24) | (ihdrData[5] << 16) | (ihdrData[6] << 8) | ihdrData[7];
            bitDepth = ihdrData[8];
            colorType = ihdrData[9];
            compressionMethod = ihdrData[10];
            filterMethod = ihdrData[11];
            interlaceMethod = ihdrData[12];
        }

        private void SetPixelData()
        {
            int scanlineLength = width + 1;
            int tHeight = idatData.Length / scanlineLength;
            pixelData = new byte[width * tHeight];

            for (int line = 0; line < tHeight; line++)
            {
                int srcIndex = line * scanlineLength + 1; // Skip the filter byte
                int destIndex = line * width;

                Array.Copy(idatData, srcIndex, pixelData, destIndex, width);
            }
        }

        private static bool IsPngSignature(byte[] signature)
        {
            return signature.SequenceEqual(PNG_SIGNATURE);
        }
    }
}
