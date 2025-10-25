using Last_Known_Reality.Reality;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace ContinuumTools.States
{
    public static class Video
    {
        public static volatile byte PaletteCount;
        public static volatile List<Color[]> Palettes = new();
        public static volatile List<Texture2D> Layers = new();
        private const int VIDEO_LAYER_SIZE = 480 * 270;
        private const int VIDEO_PAGE_TOTAL_SIZE = VIDEO_LAYER_SIZE + 768;
        private static Color[] colData = new Color[VIDEO_LAYER_SIZE];

        public static void InitializeLayers()
        {
            for (int i = 0; i < 8; i++)
            {
                Texture2D texture = new(Renderer.GetGraphicsDevice(), 480, 270);
                Layers.Add(texture);
                Color[] palette = new Color[256];
                Palettes.Add(palette);
            }
        }

        public static void ProcessPaletteData(byte[] data)
        {
            int colorsPerPalette = 256; // Each palette is 256 colors (3 bytes per color)

            if (data.Length % (colorsPerPalette * 3) != 0)
            {
                throw new ArgumentException("Invalid palette data length. It must be a multiple of " + (colorsPerPalette * 3) + " bytes.");
            }

            PaletteCount = (byte)(data.Length / (colorsPerPalette * 3));

            for (int p = 0; p < PaletteCount; p++)
            {
                Color[] palette = new Color[colorsPerPalette];

                for (int i = 0; i < colorsPerPalette; i++)
                {
                    int startIndex = (p * colorsPerPalette * 3) + (i * 3);
                    byte r = data[startIndex];
                    byte g = data[startIndex + 1];
                    byte b = data[startIndex + 2];

                    // Create a Color from the RGB values
                    palette[i] = new Color(r, g, b);
                }

                Palettes[p] = palette;
            }
        }

        public static void ProcessLayerData(byte[] data)
        {
            for (int p = 0; p < PaletteCount; p++)
            {
                int dIndex = p * VIDEO_LAYER_SIZE;

                for (int i = 0; i < VIDEO_LAYER_SIZE; i++)
                {
                    colData[i] = (data[dIndex + i] == 0 && p > 0) ?
                        Color.Transparent :
                        Palettes[p][data[dIndex + i]];
                }

                Layers[p].SetData(colData);
            }
        }

        public static void ProcessVideoData(byte[] data)
        {
            byte[] decompressed = Decompress(data);
            PaletteCount = (byte)(decompressed.Length / VIDEO_PAGE_TOTAL_SIZE);
            byte[] palettes = new byte[PaletteCount * 768];
            byte[] videoLayers = new byte[PaletteCount * VIDEO_LAYER_SIZE];

            Array.Copy(decompressed, 0, palettes, 0, PaletteCount * 768);
            Array.Copy(decompressed, PaletteCount * 768, videoLayers, 0, PaletteCount * VIDEO_LAYER_SIZE);

            ProcessPaletteData(palettes);
            ProcessLayerData(videoLayers);
        }

        public static byte[] Decompress(byte[] compressedData)
        {
            using MemoryStream compressedStream = new MemoryStream(compressedData);
            using MemoryStream decompressedStream = new MemoryStream();
            using GZipStream decompressionStream = new GZipStream(compressedStream, CompressionMode.Decompress);
            decompressionStream.CopyTo(decompressedStream);
            return decompressedStream.ToArray();
        }
    }
}
