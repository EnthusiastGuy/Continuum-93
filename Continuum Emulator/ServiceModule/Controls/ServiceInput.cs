using Continuum93.Emulator.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Continuum93.ServiceModule.Controls
{
    public class ServiceInput
    {
        private MouseState _prevMouse;

        public void Update(GameTime gameTime)
        {
            // Toggle Service Mode
            if (InputKeyboard.KeyPressed(Service.STATE.ServiceKey))
            {
                Service.STATE.ToggleServiceMode();
            }

            if (InputKeyboard.KeyPressed(Keys.F10))
            {
                Operations.Disassemble();
            }

            // NEW: mouse -> window system
            var mouse = Mouse.GetState();

            if (ServiceGraphics.WindowManager != null && Service.STATE.UseServiceView)
            {
                bool taskbarCaptured = false;

                // Let the taskbar see the input first
                if (ServiceGraphics.WindowManager.Taskbar != null)
                {
                    taskbarCaptured = ServiceGraphics.WindowManager
                        .Taskbar
                        .Update(gameTime, mouse, _prevMouse);
                }

                // Only send input to windows if taskbar didn’t use it
                if (!taskbarCaptured)
                {
                    ServiceGraphics.WindowManager.HandleInput(mouse, _prevMouse);
                }
            }

            _prevMouse = mouse;
        }
    }
}
