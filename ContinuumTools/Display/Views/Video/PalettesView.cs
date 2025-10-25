using ContinuumTools.States;
using ContinuumTools.Utils;
using Last_Known_Reality.Reality;
using Microsoft.Xna.Framework;

namespace ContinuumTools.Display.Views.Video
{
    public static class PalettesView
    {
        static Vector2 DIS_TITLE_POINT_A = new(Constants.TITLE_LEFT_PADDING, Constants.TITLE_TOP_PADDING + 24);
        static Vector2 DIS_TITLE_POINT_B = new(543 + 119, Constants.TITLE_TOP_PADDING + 24);

        public static void Update()
        {

        }

        public static void Draw()
        {
            // Draw 3D background
            Renderer.DrawStringPlusHiRes(
                GameContent.Fonts.SparkleMedium,
                $"Video layers",
                Constants.TITLE_LEFT_PADDING, Constants.TITLE_TOP_PADDING + 8,
                UserInput.HoveringDisassembler ? Colors.TitleHovered : Colors.TitleIdle);

            Renderer.DrawLine(DIS_TITLE_POINT_A, DIS_TITLE_POINT_B, Colors.TitleIdle);

            Renderer.DrawRectangle(7, 28, 652, 367, Colors.TitleIdle);
            Renderer.DrawRectangle(8, 29, 650, 365, Colors.MenuBackground);

            for (int p = 0; p < States.Video.PaletteCount; p++)
            {
                for (int i = 0; i < States.Video.Palettes[p].Length; i++)
                {
                    Renderer.DrawStringPlusHiRes(GameContent.Fonts.SparkleMedium, $"{p}", 16, (int)Renderer.CanvasHeight - 31 - p * 11, Color.AliceBlue);
                    Renderer.DrawRectangle(30 + i * 4, (int)Renderer.CanvasHeight - 30 - p * 11, 4, 10, States.Video.Palettes[p][i]);
                }
            }
        }
    }
}
