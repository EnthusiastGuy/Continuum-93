using Last_Known_Reality.Reality;
using Microsoft.Xna.Framework;

namespace ContinuumTools.Display.Components
{
    public class HexLabel: Label
    {
        private static Color _leadingZeros = new(1f, 1f, 1f, 0.1f);
        private static Color _registerColor = new(.5f, .5f, 1f);
        private static Color _changedRegisterColor = new(.8f, .5f, 1f);

        public string Register { get; set; }

        public string Value { get; set; }

        public string HexValue { get; set; }

        public string OldHexValue { get; set; }

        public void Draw()
        {
            bool valueChanged = HexValue != OldHexValue;

            int monoWidth = Font.GetMaxWidth() - 4;
            int currentX = Area.X;
            Renderer.DrawMonospaced(Font, $"{Register}", currentX, Area.Y, valueChanged ? _changedRegisterColor : _registerColor, -4);
            currentX += monoWidth * (Register.Length + 1);

            bool nonZero = false;

            for (int i = 0; i < HexValue.Length; i++)
            {
                Color markColor = Color.White;

                if (HexValue[i] != '0')
                    nonZero = true;

                if (HexValue[i] != OldHexValue[i])
                {
                    markColor = Color.DarkOrange;
                }

                if (!nonZero && HexValue[i] == '0')
                {
                    markColor = _leadingZeros;
                }

                Renderer.DrawMonospaced(Font, $"{HexValue[i]}", currentX, Area.Y, markColor, -4);
                currentX += monoWidth;
                if ((i + 1) % 2 == 0)
                    currentX += 2;
            }

        }
    }
}
