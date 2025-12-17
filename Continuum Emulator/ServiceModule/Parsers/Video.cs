using Continuum93.Emulator;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Continuum93.ServiceModule.Parsers
{
    public static class Video
    {
        public static int PaletteCount = 16;
        public static Color[][] Palettes = new Color[16][];
        public static List<Texture2D> Layers = new List<Texture2D>();
        private static Color[] _layerColorData = new Color[(int)Constants.V_SIZE];

        public static void InitializeLayers()
        {
            var device = Renderer.GetGraphicsDevice();
            Layers.Clear();
            for (int i = 0; i < 8; i++)
            {
                Texture2D texture = new Texture2D(device, (int)Constants.V_WIDTH, (int)Constants.V_HEIGHT);
                Layers.Add(texture);
            }
        }

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

            var graphics = Machine.COMPUTER.GRAPHICS;
            PaletteCount = graphics.VRAM_PAGES;

            // Update palettes from graphics
            var palettesBuffer = graphics.GetPalettesBuffer();
            const int PALETTE_SIZE = 768; // 256 colors * 3 bytes
            for (int p = 0; p < PaletteCount && p < 8; p++)
            {
                if (Palettes[p] == null)
                    Palettes[p] = new Color[256];

                for (int i = 0; i < 256; i++)
                {
                    int offset = p * PALETTE_SIZE + i * 3;
                    if (offset + 2 < palettesBuffer.Length)
                    {
                        byte r = palettesBuffer[offset];
                        byte g = palettesBuffer[offset + 1];
                        byte b = palettesBuffer[offset + 2];
                        Palettes[p][i] = new Color(r, g, b);
                    }
                }
            }

            // Update layer textures
            if (Layers.Count == 0)
            {
                InitializeLayers();
            }

            var videoBuffer = graphics.GetVideoBuffer();
            for (int p = 0; p < PaletteCount && p < 8; p++)
            {
                int videoOffset = p * (int)Constants.V_SIZE;
                for (int i = 0; i < Constants.V_SIZE; i++)
                {
                    if (videoOffset + i < videoBuffer.Length)
                    {
                        byte colorIndex = videoBuffer[videoOffset + i];
                        if (colorIndex == 0 && p > 0)
                        {
                            _layerColorData[i] = Color.Transparent;
                        }
                        else if (p < Palettes.Length && Palettes[p] != null && colorIndex < Palettes[p].Length)
                        {
                            _layerColorData[i] = Palettes[p][colorIndex];
                        }
                        else
                        {
                            _layerColorData[i] = Color.Black;
                        }
                    }
                }
                Layers[p].SetData(_layerColorData);
            }
        }
    }
}

