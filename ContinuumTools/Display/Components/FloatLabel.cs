using Last_Known_Reality.Reality;
using Microsoft.Xna.Framework;

namespace ContinuumTools.Display.Components
{
    public class FloatLabel: Label
    {
        private static Color _leadingZeros = new(1f, 1f, 1f, 0.1f);
        private static Color _registerColor = new(.5f, .5f, 1f);
        private static Color _changedRegisterColor = new(.8f, .5f, 1f);

        public string Register { get; set; }

        public float FloatValue { get; set; }

        public float OldFloatValue { get; set; }

        public string Value { get; set; }

        public void Draw()
        {
            int monoWidth = Font.GetMaxWidth() - 4;
            int currentX = Area.X;
            Renderer.DrawMonospaced(Font, $"{Register}", currentX, Area.Y, _registerColor, -4);
            currentX += monoWidth * 4;

            Color markColor = Color.White;

            if (!FloatValue.Equals(OldFloatValue))
            {
                markColor = Color.DarkOrange;
            }

            Renderer.DrawMonospaced(Font, $"{FloatValue}", currentX, Area.Y, markColor, -4);
        }
    }
}
