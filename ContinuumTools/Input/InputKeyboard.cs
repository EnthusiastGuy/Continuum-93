using ContinuumTools.Input;
using ContinuumTools.Input.Views;
using Microsoft.Xna.Framework.Input;

namespace Continuum_Emulator.Emulator.Controls
{
    public class InputKeyboard
    {
        private static int _historyDepth = 4;
        private static KeyboardState[] _stateHistory;
        private static byte[] keys = new byte[256];

        public static void Initialize()
        {
            _stateHistory = new KeyboardState[_historyDepth];

            for (int i = 0; i < _historyDepth; i++)
                _stateHistory[i] = Keyboard.GetState();
        }

        public static void Update()
        {
            ShiftStateHistory();
            _stateHistory[_historyDepth - 1] = Keyboard.GetState();
        }

        public static KeyboardState GetCurrentKeyboarsState()
        {
            return _stateHistory[_historyDepth - 1];
        }

        public static KeyboardState GetPreviousKeyboardState()
        {
            return _stateHistory[_historyDepth - 2];
        }

        public static bool KeyPressed(params Keys[] keys)
        {
            foreach (Keys key in keys)
                if (GetCurrentKeyboarsState().IsKeyUp(key) || GetPreviousKeyboardState().IsKeyDown(key))
                    return false;
            return true;
        }

        public static bool KeyDown(params Keys[] keys)
        {
            foreach (Keys key in keys)
                if (GetCurrentKeyboarsState().IsKeyUp(key))
                    return false;

            return true;
        }

        // Soft method
        public static byte[] GetPressedKeys()
        {
            Keys[] pressedKeys = GetCurrentKeyboarsState().GetPressedKeys();
            byte[] keys = new byte[pressedKeys.Length];

            for (int i = 0; i < pressedKeys.Length; i++)
            {
                keys[i] = (byte)pressedKeys[i];
            }

            return keys;
        }

        public static bool KeyChangedState(Keys key)
        {
            return GetCurrentKeyboarsState().IsKeyUp(key) != GetPreviousKeyboardState().IsKeyUp(key);
        }

        private static void ShiftStateHistory()
        {
            for (int i = 1; i < _historyDepth; i++)
                _stateHistory[i - 1] = _stateHistory[i];
        }
    }
}
