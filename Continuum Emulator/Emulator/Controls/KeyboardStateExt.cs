/*
 * Events to be implemented:
 * 
 * KeyUp
 * KeyDown
 * KeyPressed
 * KeyReleased
 */

using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Continuum93.Emulator.Controls
{
    public static class KeyboardStateExt
    {
        private static bool[] state = new bool[256];
        private static byte pressedKeys = 0;

        public static void ClearState()
        {
            state = new bool[256];
        }

        public static void RegisterKeyUp(Keys key)
        {
            if (state[(byte)key])
                pressedKeys--;

            state[(byte)key] = false;
            ManageModifierKeys(key);
        }

        public static void RegisterKeyDown(Keys key)
        {
            if (!state[(byte)key])
                pressedKeys++;

            state[(byte)key] = true;
            ManageModifierKeys(key);
        }

        public static byte[] GetStateAsBytes()
        {
            byte[] bytes = new byte[32];
            int statePos;

            for (byte i = 0; i < 32; i++)
            {
                statePos = i * 8;
                bytes[i] = (byte)(
                    (state[statePos] ? 128 : 0) |
                    (state[statePos + 1] ? 64 : 0) |
                    (state[statePos + 2] ? 32 : 0) |
                    (state[statePos + 3] ? 16 : 0) |
                    (state[statePos + 4] ? 8 : 0) |
                    (state[statePos + 5] ? 4 : 0) |
                    (state[statePos + 6] ? 2 : 0) |
                    (state[statePos + 7] ? 1 : 0)
                );
            }

            return bytes;
        }

        public static byte[] GetStateAsCodes()
        {
            List<byte> response = new List<byte>();

            for (short i = 0; i < 256; i++)
            {
                if (state[i])
                {
                    response.Add((byte)i);
                }
            }

            return response.ToArray();
        }

        public static byte GetTotalOfPressedKeys()
        {
            return pressedKeys;
        }

        public static void SetStateForTests(bool[] testState)
        {
            state = testState;
        }

        private static void ManageModifierKeys(Keys key)
        {
            switch (key)
            {
                // Any shift
                case Keys.LeftShift:
                case Keys.RightShift:
                    state[16] = state[160] || state[161];
                    break;
                // Any Ctrl
                case Keys.LeftControl:
                case Keys.RightControl:
                    state[17] = state[162] || state[163];
                    break;
                // Any Alt
                case Keys.LeftAlt:
                case Keys.RightAlt:
                    state[18] = state[164] || state[165];
                    break;
            }
        }
    }
}
