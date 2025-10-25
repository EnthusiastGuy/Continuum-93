using Microsoft.Xna.Framework;

namespace ContinuumTools.Display.Views
{
    public static class Colors
    {
        public static Color TitleIdle = Color.LightSeaGreen;
        public static Color TitleHovered = Color.SandyBrown;
        public static Color MenuBackground = new(0, 40, 60);
        public static Color ButtonColorIdle = new(100, 149, 180);
        public static Color ButtonColorHovered = new(120, 179, 216);
        public static Color ButtonColorActive = Color.DarkOrange;

        public static Color BackgroundConnected = new(0, 0.05f, 0.1f);
        public static Color BackgroundDisconnected = new(0.1f, 0, 0.0f);


        public static Color CurrentInstructionBackground = new(126, 83, 53);
        public static Color HistoryInstructionBackground = new(84, 162, 135);


        public static Color DiscreteNonASCII = new(.1f, .1f, .1f, .4f);
        public static Color TextASCII = new(184, 134, 11);
        public static Color TextASCIIHovered = new(204, 154, 31);
        public static Color MemoryByte = new(.268f, .56f, .678f, 1);
        public static Color MemoryByteHovered = new(.368f, .66f, .778f, 1);
    }
}
