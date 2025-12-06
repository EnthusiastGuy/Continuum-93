using Continuum93.Emulator.Controls;
using Continuum93.Emulator.Interrupts;
using Continuum93.CodeAnalysis;
using Continuum93.Emulator;
using Microsoft.Xna.Framework;
using Continuum93.Emulator.States;
using Continuum93.ServiceModule;

namespace Continuum93.Emulator.Window
{
    public static class WindowManager
    {
        private static GameWindow _gameWindow;
        private static WindowState _newState = new();
        private static WindowState _oldState = new();
        public static Point WindowPosition;

        public static void Initialize(GameWindow gameWindow)
        {
            _gameWindow = gameWindow;

            // TODO think of something about this crazy reference to Machine.COMPUTER that kills any possible testing
            _gameWindow.TextInput += (object sender, TextInputEventArgs e) => InterruptsInput.TriggerKeyboardHandler(e.Character, e.Key, Machine.COMPUTER);

            _gameWindow.KeyUp += (object sender, InputKeyEventArgs e) => KeyboardStateExt.RegisterKeyUp(e.Key);
            _gameWindow.KeyDown += (object sender, InputKeyEventArgs e) => KeyboardStateExt.RegisterKeyDown(e.Key);

            _gameWindow.AllowUserResizing = true;
        }

        public static int GetClientWidth() => _newState.ClientWidth;
        public static int GetClientHeight() => _newState.ClientHeight;
        public static float GetClientXRatio() => _newState.ClientWidth / (float)Constants.V_WIDTH;

        public static float GetClientYRatio() => _newState.ClientHeight / (float)Constants.V_HEIGHT;

        public static void Update()
        {
            UpdateStates();

            if (_oldState.ClientX != _newState.ClientX || _oldState.ClientY != _newState.ClientY)
            {
                PositionChanged();
            }

            if (_oldState.ClientWidth != _newState.ClientWidth || _oldState.ClientHeight != _newState.ClientHeight)
            {

                int newWidth = (int)(_newState.ClientHeight * 1.7777777777777777777777777777777777f);
                Renderer.SetPreferredBackBufferSize(newWidth, _newState.ClientHeight);
            }

            _gameWindow.Title = 
                $"Continuum 93    {Version.GetVersion()} {(Service.STATE.ServiceMode ? "- Service mode, \"" + Service.STATE.ServiceKey + "\" exits" : "")} {(DebugState.ClientConnected ? "- tools connected" : "")}{(DebugState.StepByStep ? ", debugging" : "")}";
        }

        public static void SetWindowSize(float ratio)
        {
            if (ratio > 4)
                return;

            Renderer.SetPreferredBackBufferSize((int)(Constants.V_WIDTH * ratio), (int)(Constants.V_HEIGHT * ratio));
        }

        private static void PositionChanged()
        {
            WindowPosition.X = _newState.ClientX;
            WindowPosition.Y = _newState.ClientY;
        }

        private static void UpdateStates()
        {
            _oldState.ClientWidth = _newState.ClientWidth;
            _oldState.ClientHeight = _newState.ClientHeight;
            _oldState.ClientX = _newState.ClientX;
            _oldState.ClientY = _newState.ClientY;

            _newState.ClientWidth = _gameWindow.ClientBounds.Width;
            _newState.ClientHeight = _gameWindow.ClientBounds.Height;
            _newState.ClientX = _gameWindow.Position.X;
            _newState.ClientY = _gameWindow.Position.Y;
        }
    }
}
