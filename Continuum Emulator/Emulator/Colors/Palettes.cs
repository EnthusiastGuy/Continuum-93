using Microsoft.Xna.Framework;

namespace Continuum93.Emulator.Colors
{
    public static class Palettes
    {
        public static Color[] P8BPP_Standard = new Color[256];

        public static void Initialize()
        {
            // 8 bits
            P8BPP_Standard = Pal8Bit.Get();
        }

        public static Color Get8BPPColor(int value)
        {
            return P8BPP_Standard[value];
        }

        public static byte[] GetPaletteForPage(byte page)
        {
            byte[] colorData = new byte[768];

            for (int i = 0; i < 256; i++)
            {
                colorData[i * 3] = P8BPP_Standard[i].R;
                colorData[i * 3 + 1] = P8BPP_Standard[i].G;
                colorData[i * 3 + 2] = P8BPP_Standard[i].B;
            }

            return colorData;
        }
    }
}
