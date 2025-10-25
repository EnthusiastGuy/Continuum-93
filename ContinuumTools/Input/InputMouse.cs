using Microsoft.Xna.Framework.Input;
using System;

namespace ContinuumTools.Input
{
    public class InputMouse
    {
        private static int _historyDepth = 4;
        private static MouseState[] _stateHistory;

        public static void Initialize()
        {
            _stateHistory = new MouseState[_historyDepth];

            for (int i = 0; i < _historyDepth; i++)
                _stateHistory[i] = Mouse.GetState();
        }

        public static void Update()
        {
            ShiftStateHistory();
            _stateHistory[_historyDepth - 1] = Mouse.GetState();
        }

        // MouseUp / MouseDown
        public static bool LeftMouseUp()
        {
            return _stateHistory[_historyDepth - 1].LeftButton == ButtonState.Released &&
                _stateHistory[_historyDepth - 2].LeftButton == ButtonState.Pressed;
        }

        public static bool LeftMouseDown()
        {
            return _stateHistory[_historyDepth - 1].LeftButton == ButtonState.Pressed &&
                _stateHistory[_historyDepth - 2].LeftButton == ButtonState.Released;
        }

        public static bool RightMouseUp()
        {
            return _stateHistory[_historyDepth - 1].RightButton == ButtonState.Released &&
                _stateHistory[_historyDepth - 2].RightButton == ButtonState.Pressed;
        }

        public static bool RightMouseDown()
        {
            return _stateHistory[_historyDepth - 1].RightButton == ButtonState.Pressed &&
                _stateHistory[_historyDepth - 2].RightButton == ButtonState.Released;
        }

        // MousePressed
        public static bool LeftMousePressed()
        {
            return _stateHistory[_historyDepth - 1].LeftButton == ButtonState.Pressed
                && _stateHistory[_historyDepth - 2].LeftButton == ButtonState.Pressed;
        }

        public static bool RightMousePressed()
        {
            return _stateHistory[_historyDepth - 1].RightButton == ButtonState.Pressed
                && _stateHistory[_historyDepth - 2].RightButton == ButtonState.Pressed;
        }

        // Mouse wheel
        public static bool WheelScrolledDown()
        {
            return _stateHistory[_historyDepth - 1].ScrollWheelValue < _stateHistory[_historyDepth - 2].ScrollWheelValue;
        }

        public static bool WheelScrolledUp()
        {
            return _stateHistory[_historyDepth - 1].ScrollWheelValue > _stateHistory[_historyDepth - 2].ScrollWheelValue;
        }

        public static int GetWheelScrolledValue()
        {
            return Math.Abs(_stateHistory[_historyDepth - 1].ScrollWheelValue - _stateHistory[_historyDepth - 2].ScrollWheelValue);
        }

        // MouseXY
        public static int GetCurrentMouseX()
        {
            return _stateHistory[_historyDepth - 1].X;
        }

        public static int GetCurrentMouseY()
        {
            return _stateHistory[_historyDepth - 1].Y;
        }

        private static void ShiftStateHistory()
        {
            for (int i = 1; i < _historyDepth; i++)
                _stateHistory[i - 1] = _stateHistory[i];
        }
    }
}
