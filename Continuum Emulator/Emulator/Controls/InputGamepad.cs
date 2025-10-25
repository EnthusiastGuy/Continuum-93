using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Continuum93.Emulator.Controls
{
    public class InputGamepad
    {
        private static int _historyDepth = 4;
        private static GamePadState[] _stateHistory;

        public static void Initialize()
        {
            _stateHistory = new GamePadState[_historyDepth];

            for (int i = 0; i < _historyDepth; i++)
                _stateHistory[i] = GamePad.GetState(PlayerIndex.One); // You can change PlayerIndex as needed
        }

        public static void Update()
        {
            ShiftStateHistory();
            _stateHistory[_historyDepth - 1] = GamePad.GetState(PlayerIndex.One); // You can change PlayerIndex as needed
            /*if (_stateHistory[0] != _stateHistory[1])
            {
                int x = 0;
            }*/
        }

        public static GamePadState GetCurrentGamepadState()
        {
            return _stateHistory[_historyDepth - 1];
        }

        public static GamePadState GetPreviousGamepadState()
        {
            return _stateHistory[_historyDepth - 2];
        }

        public static bool ButtonPressed(params Buttons[] buttons)
        {
            foreach (Buttons button in buttons)
                if (GetCurrentGamepadState().IsButtonUp(button) || GetPreviousGamepadState().IsButtonDown(button))
                    return false;
            return true;
        }

        public static bool ButtonReleased(params Buttons[] buttons)
        {
            foreach (Buttons button in buttons)
                if (GetCurrentGamepadState().IsButtonDown(button) || GetPreviousGamepadState().IsButtonUp(button))
                    return false;
            return true;
        }

        public static bool ButtonDown(params Buttons[] buttons)
        {
            foreach (Buttons button in buttons)
                if (GetCurrentGamepadState().IsButtonUp(button))
                    return false;

            return true;
        }

        public static bool ButtonChangedState(Buttons button)
        {
            return GetCurrentGamepadState().IsButtonUp(button) != GetPreviousGamepadState().IsButtonUp(button);
        }

        private static void ShiftStateHistory()
        {
            for (int i = 1; i < _historyDepth; i++)
                _stateHistory[i - 1] = _stateHistory[i];
        }
    }
}
