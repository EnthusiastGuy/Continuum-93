using ContinuumTools.States;

namespace ContinuumTools.Input.Views
{
    public static class StacksViewInput
    {
        public static void Update()
        {
            UserInput.HoveringStacksView =
                InputMouse.GetCurrentMouseX() < 950 && InputMouse.GetCurrentMouseX() > 550 && InputMouse.GetCurrentMouseY() > 370 && InputMouse.GetCurrentMouseY() <= 520;
        }
    }
}
