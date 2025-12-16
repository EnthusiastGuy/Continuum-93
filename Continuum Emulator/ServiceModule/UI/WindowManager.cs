using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Continuum93.ServiceModule.UI
{
    public class WindowManager
    {
        public readonly List<Window> Windows = [];
        public Taskbar Taskbar { get; }

        public WindowManager()
        {
            Taskbar = new Taskbar(this);
        }

        public void Add(Window window)
        {
            window.Manager = this;
            Windows.Add(window);
        }
        public void Remove(Window window) => Windows.Remove(window);

        public void BringToFront(Window window)
        {
            if (Windows.Remove(window))
                Windows.Add(window);

            UpdateTopMostFlags();
        }

        private void UpdateTopMostFlags()
        {
            // Clear all
            foreach (var w in Windows)
                w.IsOnTop = false;

            // The last window in the list is on top
            if (Windows.Count > 0)
                Windows[^1].IsOnTop = true;
        }


        // INPUT ONLY – this will be called from ServiceInput
        public void HandleInput(MouseState mouse, MouseState prevMouse)
        {
            // Default cursor every frame
            Mouse.SetCursor(MouseCursor.Arrow);

            // Clear focus
            foreach (var w in Windows)
                w.IsFocused = false;

            var mousePos = new Point(mouse.X, mouse.Y);

            // Let the topmost window under the mouse update the cursor
            Window hover = null;
            for (int i = Windows.Count - 1; i >= 0; i--)
            {
                var w = Windows[i];
                if (!w.Visible) continue;

                if (w.Bounds.Contains(mousePos))
                {
                    hover = w;
                    break;
                }
            }

            if (hover != null)
            {
                hover.UpdateCursor(mouse);
            }

            // Normal input handling (drag, resize, clicks)
            for (int i = Windows.Count - 1; i >= 0; i--)
            {
                var w = Windows[i];
                if (!w.Visible) continue;

                if (w.HandleInput(mouse, prevMouse))
                {
                    w.IsFocused = true;
                    BringToFront(w);
                    break;
                }
            }
        }


        // PER-FRAME LOGIC – this will be called from ServiceGraphics.Update
        public void Update(GameTime gameTime)
        {
            foreach (var w in Windows)
            {
                if (w.Visible)
                    w.Update(gameTime);
            }
        }

        public void Draw()
        {
            foreach (var w in Windows)
            {
                if (w.Visible)
                    w.Draw();
            }

            Taskbar?.Draw();
        }
    }
}
