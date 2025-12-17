using Continuum93.Emulator;
using Microsoft.Xna.Framework;

namespace Continuum93.ServiceModule.Parsers
{
    public static class Video
    {
        public static int PaletteCount = 16;
        public static Color[][] Palettes = new Color[16][];

        public static void Update()
        {
            if (Machine.COMPUTER?.GRAPHICS == null)
            {
                // Initialize with empty palettes
                for (int p = 0; p < 16; p++)
                {
                    Palettes[p] = new Color[256];
                    for (int i = 0; i < 256; i++)
                    {
                        Palettes[p][i] = Color.Black;
                    }
                }
                return;
            }

            // TODO: Access actual palette data from GRAPHICS when available
            // For now, initialize with empty palettes
            for (int p = 0; p < 16; p++)
            {
                Palettes[p] = new Color[256];
                for (int i = 0; i < 256; i++)
                {
                    Palettes[p][i] = Color.Black;
                }
            }
        }
    }
}

