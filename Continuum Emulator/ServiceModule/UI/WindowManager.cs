using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Continuum93.ServiceModule.UI
{
    public class WindowManager
    {
        public readonly List<Window> Windows = [];
        public Taskbar Taskbar { get; }
        private Window _focusedWindow;

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

            // Drop focus if the focused window is no longer visible
            if (_focusedWindow != null && !_focusedWindow.Visible)
            {
                _focusedWindow = null;
            }

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
                    _focusedWindow = w;
                    BringToFront(w);
                    break;
                }
            }
            
            // Handle input for hover popups (for scrolling)
            foreach (var w in Windows)
            {
                if (w.Visible && w is DisassemblerWindow dw && dw.HoverPopup != null && dw.HoverPopup.Visible)
                {
                    if (dw.HoverPopup.HandleInput(mouse, prevMouse))
                    {
                        // Popup consumed the input (scrolling)
                        break;
                    }
                }
                else if (w.Visible && w is MemoryWindow mw && mw.GetHoverPopup() != null && mw.GetHoverPopup().Visible)
                {
                    if (mw.GetHoverPopup().HandleInput(mouse, prevMouse))
                    {
                        // Popup consumed the input (scrolling)
                        break;
                    }
                }
                else if (w.Visible && w is StatusBarWindow sbw && sbw.HoverPopup != null && sbw.HoverPopup.Visible)
                {
                    if (sbw.HoverPopup.HandleInput(mouse, prevMouse))
                    {
                        // Popup consumed the input (scrolling)
                        break;
                    }
                }
            }

            // Update focus flags (persist focus until another window captures input)
            foreach (var w in Windows)
            {
                w.IsFocused = (w == _focusedWindow);
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
            
            // Update hover popups
            foreach (var w in Windows)
            {
                if (w.Visible && w is DisassemblerWindow dw && dw.HoverPopup != null && dw.HoverPopup.Visible)
                {
                    dw.HoverPopup.Update(gameTime);
                }
                else if (w.Visible && w is MemoryWindow mw && mw.GetHoverPopup() != null && mw.GetHoverPopup().Visible)
                {
                    mw.GetHoverPopup().Update(gameTime);
                }
                else if (w.Visible && w is StatusBarWindow sbw && sbw.HoverPopup != null && sbw.HoverPopup.Visible)
                {
                    sbw.HoverPopup.Update(gameTime);
                }
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
            
            // Draw hover popups on top of everything
            foreach (var w in Windows)
            {
                if (w.Visible && w is DisassemblerWindow dw && dw.HoverPopup != null && dw.HoverPopup.Visible)
                {
                    dw.HoverPopup.Draw();
                }
                else if (w.Visible && w is MemoryWindow mw && mw.GetHoverPopup() != null && mw.GetHoverPopup().Visible)
                {
                    mw.GetHoverPopup().Draw();
                }
                else if (w.Visible && w is StatusBarWindow sbw && sbw.HoverPopup != null && sbw.HoverPopup.Visible)
                {
                    sbw.HoverPopup.Draw();
                }
            }
        }
    }
}
