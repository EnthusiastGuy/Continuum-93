using Last_Known_Reality.Reality;
using Microsoft.Xna.Framework;

namespace ContinuumTools.Display.Components
{
    public class FlagLabel: Label
    {
        private readonly static string[] FLAGNAMES = new string[] {
            "NZ", "NC", "SP", "NO", "PE", "NE", "LTE", "GTE",
            "Z", "C", "SN", "OV", "PO", "EQ", "GT", "LT" };
        private static Color _registerColor = new(.5f, .5f, 1f);

        public static int OriginX;
        public static int OriginY;

        public byte FlagIndex { get; set; }

        public bool FlagValue { get; set; }

        public bool OldFlagValue { get; set; }

        public string Value { get; set; }

        public void Draw()
        {
            int monoWidth = Font.GetMaxWidth() - 4;
            int x = OriginX + FlagIndex * (int)(monoWidth * 4.5f);
            int y1 = OriginY + 12;
            int y2 = OriginY + 40;

            Color markColor = Color.White;

            Renderer.DrawMonospaced(Font, $"{FLAGNAMES[8 + FlagIndex]}", x, y1, _registerColor, -4);
            Renderer.DrawMonospaced(Font, $"{FLAGNAMES[FlagIndex]}", x, y2, _registerColor, -4);

            if (!FlagValue.Equals(OldFlagValue))
            {
                markColor = Color.DarkOrange;
            }

            Renderer.DrawMonospaced(Font, $"{(FlagValue ? 1 : 0)}", x, y1 + 12, markColor, -4);
            Renderer.DrawMonospaced(Font, $"{(FlagValue ? 0 : 1)}", x, y2 + 12, markColor, -4);
        }
    }
}
