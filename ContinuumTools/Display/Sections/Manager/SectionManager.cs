using ContinuumTools.States;

namespace ContinuumTools.Display.Sections.Manager
{
    public static class SectionManager
    {
        public static string ActiveSection = "main";
        public static string[] Sections = new[] { "main", "video", "utils" };

        public static void SetActiveSection()
        {
            ActiveSection = Sections[UserInput.HoveringButtonIndex];
        }

        public static int GetActiveIndex()
        {
            for (int i = 0; i < Sections.Length; i++)
            {
                if (Sections[i] == ActiveSection)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
