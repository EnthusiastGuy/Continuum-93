using Continuum93.Emulator;
using Continuum93.ServiceModule.Parsers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Continuum93.ServiceModule.UI
{
    public class PalettesWindow : Window
    {
        public PalettesWindow(
            string title,
            int x, int y,
            int width, int height,
            float spawnDelaySeconds = 0,
            bool canResize = true,
            bool canClose = false)
            : base(title, x, y, width, height, spawnDelaySeconds, canResize, canClose)
        {
        }

        protected override void UpdateContent(GameTime gameTime)
        {
            Video.Update();
        }

        protected override void DrawContent(SpriteBatch spriteBatch, Rectangle contentRect)
        {
            // Title
            ServiceGraphics.DrawText(
                Fonts.ModernDOS_12x18,
                "Video layers",
                contentRect.X + Padding,
                contentRect.Y + Padding,
                contentRect.Width - Padding * 2,
                Color.Yellow,
                Color.Black,
                (byte)ServiceFontFlags.DrawOutline,
                0xFF
            );

            var pixel = Renderer.GetPixelTexture();
            int startY = contentRect.Y + Padding + 24;

            // Draw palettes
            for (int p = 0; p < Video.PaletteCount; p++)
            {
                if (Video.Palettes[p] == null)
                    continue;

                // Palette number label
                ServiceGraphics.DrawText(
                    Fonts.ModernDOS_12x18,
                    p.ToString(),
                    contentRect.X + Padding,
                    startY + p * 11,
                    contentRect.Width - Padding * 2,
                    Color.AliceBlue,
                    Color.Black,
                    (byte)ServiceFontFlags.DrawOutline,
                    0xFF
                );

                // Draw palette colors
                for (int i = 0; i < Video.Palettes[p].Length && i < 256; i++)
                {
                    Rectangle colorRect = new(
                        contentRect.X + Padding + 30 + i * 4,
                        startY + p * 11,
                        4,
                        10
                    );
                    spriteBatch.Draw(pixel, colorRect, Video.Palettes[p][i]);
                }
            }
        }
    }
}

