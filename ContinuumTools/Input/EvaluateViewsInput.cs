using ContinuumTools.Display.Sections.Manager;
using ContinuumTools.Input.Views;
using ContinuumTools.Network;
using Last_Known_Reality;

namespace ContinuumTools.Input
{
    public static class EvaluateViewsInput
    {
        public static void Update()
        {
            MenuView.Update();
            if (WindowManager.IsActive() && Client.IsConnected())       // Work only when connected and in focus
            {
                if (SectionManager.ActiveSection == "main")
                {
                    RegistryViewInput.Update();
                    DisassemblerInput.Update();
                    MemoryViewInput.Update();
                    StacksViewInput.Update();
                }
            }
        }
    }
}
