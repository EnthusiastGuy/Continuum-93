using Continuum93.Emulator;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;

namespace Continuum93.Tools
{
    public static class ImageConvertor
    {

        static Dictionary<Color, byte> palette = new Dictionary<Color, byte>();

        public static void ConvertPNGToByteArray(string path)
        {
            byte[] saveData = TextureTo2DArray(LoadTexture(path));
            File.WriteAllBytes(path + ".data", saveData);
        }

        public static Texture2D LoadTexture(string path)
        {
            FileStream fileStream = new FileStream(path, FileMode.Open);
            return Texture2D.FromStream(Renderer.GetGraphicsDevice(), fileStream);
        }

        public static byte[] TextureTo2DArray(Texture2D texture)
        {
            int tWidth = texture.Width;
            int tHeight = texture.Height;

            byte currentColor = 1;
            Color col;
            byte[] texArr = new byte[Constants.V_SIZE];

            palette.Clear();

            Color[] colors1D = new Color[tWidth * tHeight];
            texture.GetData(colors1D);

            for (int x = 0; x < tWidth; x++)
                for (int y = 0; y < tHeight; y++)
                {
                    col = colors1D[x + y * tWidth];

                    if (!palette.ContainsKey(col))
                    {
                        palette.Add(col, currentColor);
                        currentColor++;
                    }

                    texArr[x + y * tWidth] = palette[col];

                }

            return texArr;
        }
    }
}
