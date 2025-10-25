using ContinuumTools.Display.Sections.Manager;
using ContinuumTools.Display.Views.Video;
using ContinuumTools.Input._3D_View;

namespace ContinuumTools.Display.Sections
{
    internal class SectionVideo
    {
        public static void Update()
        {
            if (SectionManager.ActiveSection != "video")
                return;

            VirtualScreen.Update();
        }

        public static void Draw()
        {
            if (SectionManager.ActiveSection != "video")
                return;

            PalettesView.Draw();
            
        }
    }
}
