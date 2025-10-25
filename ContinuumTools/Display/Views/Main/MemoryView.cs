using ContinuumTools.States;
using Last_Known_Reality.Reality;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ContinuumTools.Display.Views.Main
{
    public static class MemoryView
    {
        private static bool _memUpdated = false;
        static List<MemLine> lines = new();

        static Vector2 MEM_TITLE_POINT_A = new(Constants.TITLE_LEFT_PADDING, Constants.TITLE_TOP_PADDING + 296);
        static Vector2 MEM_TITLE_POINT_B = new(543, Constants.TITLE_TOP_PADDING + 296);

        public static void Update()
        {
            if (!_memUpdated)
            {
                // TODO This bugs out sometimes when scrolling RAM
                lines = new(Memory.Lines);
                _memUpdated = true;
            }
        }
        public static void UpdateMem()
        {
            _memUpdated = false;
        }

        public static void Draw()
        {
            Renderer.DrawStringPlusHiRes(
                GameContent.Fonts.SparkleMedium, "Memory view", Constants.TITLE_LEFT_PADDING, Constants.TITLE_TOP_PADDING + 280,
                UserInput.HoveringMemoryViewer ? Colors.TitleHovered : Colors.TitleIdle);
            Renderer.DrawLine(MEM_TITLE_POINT_A, MEM_TITLE_POINT_B, Colors.TitleIdle);

            for (int i = 0; i < lines.Count; i++)
            {
                int yPos = i * 12 + 272;

                Renderer.DrawMonospaced(
                    GameContent.Fonts.SparkleMedium,
                    $"{lines[i].TextAddress}",
                    Constants.REGS_LEFT_PADDING, Constants.REGS_TOP_PADDING + yPos,
                    Color.White,
                    -4
                );

                Renderer.DrawMonospaced(
                    GameContent.Fonts.SparkleMedium,
                    $"{lines[i].HexBytes}",
                    Constants.REGS_LEFT_PADDING + 60, Constants.REGS_TOP_PADDING + yPos,
                    Colors.MemoryByte,
                    -4
                );

                Renderer.DrawMonospaced(
                    GameContent.Fonts.SparkleMedium,
                    $"{lines[i].COVERBytes}",
                    Constants.REGS_LEFT_PADDING + 406, Constants.REGS_TOP_PADDING + yPos,
                    Colors.DiscreteNonASCII,
                    -3
                );

                Renderer.DrawMonospaced(
                    GameContent.Fonts.SparkleMedium,
                    $"{lines[i].ASCIIBytes}",
                    Constants.REGS_LEFT_PADDING + 406, Constants.REGS_TOP_PADDING + yPos,
                    Colors.TextASCII,
                    -3
                );
            }

        }
    }
}
