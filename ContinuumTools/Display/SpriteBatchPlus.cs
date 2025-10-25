namespace Last_Known_Reality.Reality
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    // Summary:
    //      Helper class for drawing advanced text strings.
    //      Strings can be drawn normally or rotated, however, the rotation
    //      currently is applied per character only.
    public class SpriteBatchPlus : SpriteBatch
    {
        public SpriteBatchPlus()
            : base(Renderer.GetGraphicsDevice())
        {
        }

        public void DrawMonospaced(PngFont font, string text, float x, float y, Color color, int offset)
        {
            int currentX = 0;

            foreach (char c in text)
            {
                Rectangle source = font.GetCharSource(c);
                Rectangle destination = source;
                destination.X = (int)x + currentX;
                destination.Y = (int)y;

                DrawSimpleString(font, source, destination, color);

                currentX += font.GetMaxWidth() + offset;
            }
        }

        public void DrawStringPlus(PngFont font, string text, float x, float y, Color color, bool? hiRes)
        {
            int currentX = 0;
            char previous = ' ';

            foreach (char c in text)
            {
                int kerning = font.GetCharactersKerning(previous, c);   // set to 0 to disable kerning
                Rectangle source = font.GetCharSource(c);
                Rectangle destination = source;
                destination.X = (int)x + currentX + kerning;
                destination.Y = (int)y;

                DrawSimpleString(font, source, destination, color);

                currentX += destination.Width + font.GetSpacing() + kerning;
                previous = c;
            }
        }

        private void DrawSimpleString(PngFont font, Rectangle source, Rectangle destination, Color color)
        {
            Renderer.Draw(
                    font.GetFontSheet(),
                    destination,
                    source,
                    color, 0, Vector2.Zero, SpriteEffects.None, 0);
        }

        private void DrawSimpleStringHiRes(PngFont font, Rectangle source, Rectangle destination, Color color)
        {
            Renderer.Draw(
                    font.GetFontSheet(),
                    destination,
                    source,
                    color, 0, Vector2.Zero, SpriteEffects.None, 0);
        }

        private void DrawAdvancedString(PngFont font, Rectangle source, Rectangle destination, Color color, float rotation)
        {
            Draw(font.GetFontSheet(), destination, source, color, rotation, Vector2.Zero, SpriteEffects.None, 0);
        }
    }
}
