using Last_Known_Reality.Reality;
using Microsoft.Xna.Framework;
using System.Reflection;

namespace Last_Known_Reality
{
    public static class WindowManager
    {
        private static GameWindow _gameWindow;
        private static WindowState _newState = new WindowState();
        private static WindowState _oldState = new WindowState();
        private static bool _isActive;

        public static void Initialize(GameWindow gameWindow)
        {
            _gameWindow = gameWindow;

            //_gameWindow.IsBorderless = true;
            _gameWindow.Title = "Continuum toolset " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            _gameWindow.AllowUserResizing = false;
            //_gameWindow.ClientSizeChanged += (object sender, EventArgs e) => ClientSizeChanged();
            RegisterViewportBounds();
        }

        private static void RegisterViewportBounds()
        {
            Config.RegisterViewportBounds(_gameWindow.ClientBounds.Width, _gameWindow.ClientBounds.Height);
            Renderer.SetPreferredBackBufferSize(_gameWindow.ClientBounds.Width, _gameWindow.ClientBounds.Height);
        }

        public static void Update(bool isActive)
        {
            _isActive = isActive;

            UpdateStates();

            if (_oldState.ClientWidth != _newState.ClientWidth || _oldState.ClientHeight != _newState.ClientHeight)
            {
                StateChanged();
            }
        }

        private static void StateChanged()
        {
            Renderer.RefreshCanvas();
            RegisterViewportBounds();
        }

        private static void UpdateStates()
        {
            _oldState.ClientWidth = _newState.ClientWidth;
            _oldState.ClientHeight = _newState.ClientHeight;

            _newState.ClientWidth = _gameWindow.ClientBounds.Width;
            _newState.ClientHeight = _gameWindow.ClientBounds.Height;
        }

        public static int GetClientWidth()
        {
            return _newState.ClientWidth;
        }

        public static int GetClientHeight()
        {
            return _newState.ClientHeight;
        }

        public static float GetRatio()
        {
            return (float)_gameWindow.ClientBounds.Width / _gameWindow.ClientBounds.Height;
        }

        public static bool IsActive()
        {
            return _isActive;
        }
    }
}
