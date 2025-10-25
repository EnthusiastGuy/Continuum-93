using ContinuumTools.Display.Sections.Manager;
using ContinuumTools.Display.Views;
using ContinuumTools.Display.Views.Main;

namespace ContinuumTools.Display.Sections
{
    public static class SectionMain
    {
        public static void Update()
        {
            DisassemblerView.Update();
            RegisterView.Update();
            FloatRegView.Update();
            FlagView.Update();
            StacksView.Update();
            MemoryView.Update();
            MenuView.Update();
            StatusBarView.Update();
        }

        public static void Draw()
        {
            if (SectionManager.ActiveSection != "main")
                return;

            DisassemblerView.Draw();
            RegisterView.Draw();
            FloatRegView.Draw();
            FlagView.Draw();
            StacksView.Draw();
            MemoryView.Draw();
            FlagView.Draw();
        }
    }
}
