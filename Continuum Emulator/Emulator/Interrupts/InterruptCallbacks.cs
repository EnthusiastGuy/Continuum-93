using System.Collections.Generic;

namespace Continuum93.Emulator.Interrupts
{
    public static class InterruptCallbacks
    {
        public static List<char> CharBuffer = new List<char>();
        public static uint KeyboardCallback = 0xFFFFFF;
        public static uint MouseCallback = 0xFFFFFF;
        public static uint ControllerCallback = 0xFFFFFF;
    }
}
