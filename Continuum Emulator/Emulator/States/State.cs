using Microsoft.Xna.Framework.Input;

namespace Continuum93.Emulator.States
{
    public static class State
    {
        public static volatile bool FullScreenRequest = false;
        public static volatile bool IsFullScreen = false;
        public static volatile bool ShutDownRequested = false;

        // Requested visibility of the host (operating system) mouse cursor: -1 no request, 0 hidden, 1 visible.
        // Guest software that draws its own cursor (e.g. the Halcyon desktop) hides the host cursor.
        public static volatile int HostCursorRequest = -1;

    }
}
