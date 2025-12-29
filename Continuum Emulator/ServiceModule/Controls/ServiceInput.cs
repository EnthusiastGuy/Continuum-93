using Continuum93.CodeAnalysis;
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
            bool wasServiceMode = Service.STATE.ServiceMode;
            if (InputKeyboard.KeyPressed(Service.STATE.ServiceKey))
            {
                Service.STATE.ToggleServiceMode();

                if (!wasServiceMode && Service.STATE.ServiceMode)
                {
                    ServiceLayoutManager.CapturePreServiceSize();
                    ServiceLayoutManager.OnServiceModeEntered();
                    DebugState.StepByStep = true;
                    DebugState.MoveNext = false;
                    Operations.Disassemble(true);
                }
                else if (wasServiceMode && !Service.STATE.ServiceMode)
                {
                    ServiceLayoutManager.RestorePreServiceSize();
                }
            }

            if (Service.STATE.ServiceMode)
            {
                // Debugging keyboard shortcuts (service mode only)
                if (InputKeyboard.KeyPressed(Keys.F10))
                {
                    Operations.Disassemble();
                }

                if (InputKeyboard.KeyPressed(Keys.Enter))
                {
                    Operations.Disassemble();
                    Operations.UpdateAll();
                }

                if (InputKeyboard.KeyPressed(Keys.F5))
                {
                    Operations.ToggleStepByStepMode();
                }

                if (InputKeyboard.KeyPressed(Keys.F8))
                {
                    Operations.ContinueExecution();
                }

                if (InputKeyboard.KeyPressed(Keys.F9))
                {
                    Operations.AdvanceStepByStep();
                }
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
