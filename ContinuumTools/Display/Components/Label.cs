using Last_Known_Reality.Reality;
using Microsoft.Xna.Framework;

namespace ContinuumTools.Display.Components
{
    /*
        A label represents a part of text. It has a bounding rectangle.
        Text can be represented letter-by-letter with different colors
     */
    public class Label
    {
        public Rectangle Area { get; set; }

        public string Text { get; set; }

        public Color[] CharacterColor { get; set; }

        public PngFont Font { get; set; }

        public Label()
        {

        }
    }
}
