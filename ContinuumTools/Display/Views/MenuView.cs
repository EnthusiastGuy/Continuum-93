using ContinuumTools.Display.Sections.Manager;
using ContinuumTools.States;
using Last_Known_Reality.Reality;
using Microsoft.Xna.Framework;

namespace ContinuumTools.Display.Views
{
    public static class MenuView
    {
        public static void Update()
        {
        }

        public static void Draw()
        {
            Renderer.DrawLine(
                new Vector2(Renderer.CanvasWidth - 32, 0),
                new Vector2(Renderer.CanvasWidth - 32, Renderer.CanvasHeight - 18),
                Colors.MenuBackground,
                64
            );

            // Draw buttons
            for (int i = 0; i < SectionManager.Sections.Length; i++)
            {
                Color buttonCol = UserInput.HoveringButtonIndex == i ? Colors.ButtonColorHovered : Colors.ButtonColorIdle;
                if (SectionManager.GetActiveIndex() == i)
                    buttonCol = Colors.ButtonColorActive;

                Renderer.DrawRectangle((int)Renderer.CanvasWidth - 62, 6 + 25 * i, 64, 24, buttonCol);
                Renderer.DrawStringPlusHiRes(
                    GameContent.Fonts.SparkleMedium,
                    SectionManager.Sections[i].ToUpper(), (int)Renderer.CanvasWidth - 54, 12 + 25 * i, Color.White);
            }
        }
    }
}
