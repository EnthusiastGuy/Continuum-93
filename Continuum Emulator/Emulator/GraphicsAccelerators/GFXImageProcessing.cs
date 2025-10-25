using Continuum93.Emulator;
using Continuum93.Emulator.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Continuum93.Emulator.GraphicsAccelerators
{
    public static class GFXImageProcessing
    {
        private static Texture2D texture;


        // Process strinctly the palette of an image
        public static void ProcessPalette(string path)
        {
            ProcessPalette(path, Renderer.GetGraphicsDevice());
        }

        public static void ProcessPalette(string path, GraphicsDevice gd)
        {
            texture = Texture2D.FromFile(gd, path);

            Color[] pixelData = new Color[texture.Width * texture.Height];
            texture.GetData(pixelData);

            HashSet<Color> uniqueColors = new();

            for (int i = 0; i < pixelData.Length; i++)
            {
                uniqueColors.Add(pixelData[i]);
            }

            ImageLoadState.ColorPalette = uniqueColors.ToArray();
            ImageLoadState.ByteRGBPalette = ProcessPaletteArray(ImageLoadState.ColorPalette);

            texture.Dispose();
            texture = null;
        }

        // Process strinctly the pixel data of an image
        public static void ProcessPixelData(string path)
        {
            ProcessPixelData(path, Renderer.GetGraphicsDevice());
        }

        public static void ProcessPixelData(string path, GraphicsDevice gd)
        {
            texture = Texture2D.FromFile(gd, path);

            Color[] pixelData = new Color[texture.Width * texture.Height];
            ImageLoadState.ImageData = new byte[texture.Width * texture.Height];

            texture.GetData(pixelData);

            Dictionary<Color, byte> colorIndexMap = new();
            List<Color> uniqueColors = new();

            for (int i = 0; i < pixelData.Length; i++)
            {
                if (!colorIndexMap.ContainsKey(pixelData[i]))
                {
                    colorIndexMap[pixelData[i]] = (byte)uniqueColors.Count;
                    uniqueColors.Add(pixelData[i]);
                }

                ImageLoadState.ImageData[i] = colorIndexMap[pixelData[i]];
            }

            texture.Dispose();
            texture = null;
        }

        // Process the pixel data and palette of an image
        public static void ProcessImageAndPalette(string path)
        {
            ProcessImageAndPalette(path, Renderer.GetGraphicsDevice());
        }

        public static void ProcessImageAndPalette(string path, GraphicsDevice gd)
        {
            texture = Texture2D.FromFile(gd, path);

            Color[] pixelData = new Color[texture.Width * texture.Height];
            ImageLoadState.ImageData = new byte[texture.Width * texture.Height];

            texture.GetData(pixelData);

            Dictionary<Color, byte> colorIndexMap = new();
            List<Color> uniqueColors = new();

            for (int i = 0; i < pixelData.Length; i++)
            {
                if (!colorIndexMap.ContainsKey(pixelData[i]))
                {
                    colorIndexMap[pixelData[i]] = (byte)uniqueColors.Count;
                    uniqueColors.Add(pixelData[i]);
                }

                ImageLoadState.ImageData[i] = colorIndexMap[pixelData[i]];
            }

            ImageLoadState.ColorPalette = uniqueColors.ToArray();
            ImageLoadState.ByteRGBPalette = ProcessPaletteArray(ImageLoadState.ColorPalette);

            texture.Dispose();
            texture = null;
        }

        public static void ClearAll()
        {
            ImageLoadState.ColorPalette = null;
            ImageLoadState.ByteRGBPalette = null;
            ImageLoadState.ImageData = null;
        }

        public static unsafe byte[] ProcessPaletteArray(Color[] colorPalette)
        {
            byte[] byteColors = new byte[colorPalette.Length * 3];

            fixed (byte* byteColorsPtr = byteColors)
            fixed (Color* colorPalettePtr = colorPalette)
            {
                byte* src = (byte*)colorPalettePtr;
                byte* dest = byteColorsPtr;
                int length = colorPalette.Length;

                for (int i = 0; i < length; i++)
                {
                    *dest++ = src[i * 4];     // R
                    *dest++ = src[i * 4 + 1]; // G
                    *dest++ = src[i * 4 + 2]; // B
                }
            }

            return byteColors;
        }
    }
}
