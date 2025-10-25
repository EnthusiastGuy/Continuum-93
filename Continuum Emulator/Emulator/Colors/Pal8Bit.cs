using Microsoft.Xna.Framework;

namespace Continuum93.Emulator.Colors
{
    public static class Pal8Bit
    {
        public static Color[] Get()
        {
            Color[] response = new Color[256];

            for (int i = 0; i < 256; i++)
            {
                // bits 0 and 1 are blue
                // bits 2, 3 and 4 are green
                // bits 5, 6 and 7 are red

                response[i] = new Color(GetRedFromByte(255 - i), GetGreenFromByte(255 - i), GetBlueFromByte(255 - i));
            }

            return response;
        }

        public static string Export()
        {
            string response = "Index, Color Code, RGB Values\n";

            for (int i = 0; i < 256; i++)
            {
                Color tColor = new(GetRedFromByte(255 - i), GetGreenFromByte(255 - i), GetBlueFromByte(255 - i));

                string colorCode = $"#{tColor.R:X2}{tColor.G:X2}{tColor.B:X2}";
                string rgbValues = $"{tColor.R}X {tColor.G}X {tColor.B}";

                response += $"{i}, {colorCode}, {rgbValues}\n";
            }

            return response;
        }

        private static int GetBlueFromByte(int c)
        {
            int response = 64 * (c & 3) - 1;

            if (response == -1)
                response = 0;

            return response;
        }

        private static int GetGreenFromByte(int c)
        {
            int response = 32 * ((c & 28) >> 2) - 1;

            if (response == -1)
                response = 0;

            return response;
        }

        private static int GetRedFromByte(int c)
        {
            int response = 32 * ((c & 224) >> 5) - 1;

            if (response == -1)
                response = 0;

            return response;
        }
    }
}
