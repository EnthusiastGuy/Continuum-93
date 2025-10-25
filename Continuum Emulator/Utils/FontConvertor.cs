using Continuum93.Tools;
using Continuum93.Tools.FontTools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace Continuum93.Utils
{
    public static class FontConvertor
    {
        public static byte[] ConvertFont(string fontPath)
        {
            const short HORIZ_CELLS = 16;
            if (!File.Exists(fontPath))
            {
                //Log.WriteLine("Font file not found: " + fontPath);
                throw new FileNotFoundException("Font file not found");
            }

            //Log.WriteLine("Creating texture.");
            Texture2D fontSheet = ImageConvertor.LoadTexture(fontPath);   //@"data\fontName.png"
            //Log.WriteLine("Texture created.");
            Color[] colors2D = new Color[fontSheet.Width * fontSheet.Height];
            fontSheet.GetData(colors2D);

            short cellWidth = (short)(fontSheet.Width / HORIZ_CELLS);
            short cellHeight = (short)(fontSheet.Height / 6);

            string characters = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";
            short cX = 0;
            short cY = 0;

            BitFont font = new();

            foreach (char c in characters)
            {
                //Debug.WriteLine("X: " + cX + ", Y: " + cY);
                // Check for first filled pixel
                short foundX = -1;

                for (int colX = cX + cellWidth - 1; colX >= cX; colX--)
                {
                    for (int colY = cY; colY < cY + cellHeight; colY++)
                        if (foundX == -1 && colors2D[colX + colY * fontSheet.Width] != Color.Transparent)
                            foundX = (short)(colX - cX + 1);

                    if (foundX != -1)
                        break;
                }

                if (foundX == -1)
                {
                    foundX = 3; // Space
                }

                Glyph glyph = new()
                {
                    Code = (byte)c,
                    x = cX,
                    y = cY,
                    w = (byte)foundX,
                    h = (byte)cellHeight,
                    c = GetSafeChar(c)
                };

                for (int colY = cY; colY < cY + cellHeight; colY++)
                {
                    uint bits = 0;

                    for (int colX = cX + cellWidth - 1; colX >= cX; colX--)
                    {
                        bits <<= 1;

                        if (colors2D[colX + colY * fontSheet.Width] != Color.Transparent)
                            bits += 1;
                    }

                    byte data8Bit = ReverseByteBits((byte)bits);
                    glyph.Data.Add(data8Bit);
                }

                font.UpdateGlyph(glyph);

                cX += cellWidth;
                if (cX >= HORIZ_CELLS * cellWidth)
                {
                    cX = 0;
                    cY += cellHeight;
                }
            }

            return font.ExportFontType(2);

        }

        private static string GetUnicodeFromChar(char c)
        {
            return ((int)c).ToString("X4");
        }

        private static string GetSafeChar(char c)
        {
            if (c.ToString() == "\"")
            {
                return "\\\"";
            }
            else if (c.ToString() == "\\")
            {
                return "\\\\";
            }

            return c.ToString();
        }

        public static byte ReverseByteBits(byte bt)
        {
            byte ret = 0;

            for (int i = 0; i < 8; i++)
            {
                ret <<= 1;
                if ((bt & 0b00000001) > 0)
                    ret += 1;

                bt >>= 1;
            }

            return ret;
        }
    }
}
