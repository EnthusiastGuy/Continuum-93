using Microsoft.Xna.Framework.Input;

namespace Continuum93.Emulator.States
{
    public static class State
    {
        public static volatile bool FullScreenRequest = false;
        public static volatile bool IsFullScreen = false;
        public static volatile bool ShutDownRequested = false;
        public static bool ServiceMode = false;
        public static Keys ServiceKey = Keys.F1;

        public static void ToggleServiceMode()
        {
            ServiceMode = !ServiceMode;
        }
    }
}
