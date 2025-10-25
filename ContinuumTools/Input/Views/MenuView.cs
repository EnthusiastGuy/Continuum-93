using ContinuumTools.Display.Sections.Manager;
using ContinuumTools.States;
using Last_Known_Reality.Reality;

namespace ContinuumTools.Input.Views
{
    public static class MenuView
    {
        public static void Update()
        {
            UserInput.HoveringButtonIndex = -1;
            if (
                InputMouse.GetCurrentMouseX() > Renderer.CanvasWidth - 62 &&
                InputMouse.GetCurrentMouseX() < Renderer.CanvasWidth &&
                InputMouse.GetCurrentMouseY() > 6 &&
                InputMouse.GetCurrentMouseY() < 6 + 25* SectionManager.Sections.Length)
            {
                UserInput.HoveringButtonIndex = (InputMouse.GetCurrentMouseY() - 6) / 25;
            }

            if (InputMouse.LeftMouseDown() && UserInput.HoveringButtonIndex != -1)
            {
                SectionManager.SetActiveSection();
            }
        }
    }
}
