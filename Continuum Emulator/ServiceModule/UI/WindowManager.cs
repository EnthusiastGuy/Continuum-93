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

            // FIRST: Check if any pop-up is visible and contains the mouse
            // Pop-ups should block all input from reaching windows underneath
            Window hoverPopup = null;
            foreach (var w in Windows)
            {
                if (!w.Visible) continue;
                
                if (w is DisassemblerWindow dw && dw.HoverPopup != null && dw.HoverPopup.Visible)
                {
                    if (dw.HoverPopup.Bounds.Contains(mousePos))
                    {
                        hoverPopup = dw.HoverPopup;
                        break;
                    }
                }
                else if (w is MemoryWindow mw && mw.GetHoverPopup() != null && mw.GetHoverPopup().Visible)
                {
                    if (mw.GetHoverPopup().Bounds.Contains(mousePos))
                    {
                        hoverPopup = mw.GetHoverPopup();
                        break;
                    }
                }
                else if (w is MemoryMapWindow mmw && mmw.GetHoverPopup() != null && mmw.GetHoverPopup().Visible)
                {
                    if (mmw.GetHoverPopup().Bounds.Contains(mousePos))
                    {
                        hoverPopup = mmw.GetHoverPopup();
                        break;
                    }
                }
                else if (w is StatusBarWindow sbw && sbw.HoverPopup != null && sbw.HoverPopup.Visible)
                {
                    if (sbw.HoverPopup.Bounds.Contains(mousePos))
                    {
                        hoverPopup = sbw.HoverPopup;
                        break;
                    }
                }
                else if (w is RegisterWindow rw && rw.GetHoverPopup() != null && rw.GetHoverPopup().Visible)
                {
                    if (rw.GetHoverPopup().Bounds.Contains(mousePos))
                    {
                        hoverPopup = rw.GetHoverPopup();
                        break;
                    }
                }
            }

            // If a pop-up is under the mouse, handle its input first and block everything else
            if (hoverPopup != null)
            {
                hoverPopup.HandleInput(mouse, prevMouse);
                hoverPopup.UpdateCursor(mouse);
                // Don't process regular windows - pop-up blocks all input
                return;
            }

            // Let the topmost window under the mouse update the cursor (only if no pop-up is blocking)
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

            // Normal input handling (drag, resize, clicks) - only if no pop-up is blocking
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

            // Update focus flags (persist focus until another window captures input)
            foreach (var w in Windows)
            {
                w.IsFocused = (w == _focusedWindow);
            }
        }


        // PER-FRAME LOGIC – this will be called from ServiceGraphics.Update
        public void Update(GameTime gameTime)
        {
            var mouse = Mouse.GetState();
            Point mousePos = new(mouse.X, mouse.Y);

            // Determine if a popup is under the mouse
            Window popupOwner = null;
            foreach (var w in Windows)
            {
                if (!w.Visible) continue;

                if (w is DisassemblerWindow dw && dw.HoverPopup != null && dw.HoverPopup.Visible && dw.HoverPopup.Bounds.Contains(mousePos))
                {
                    popupOwner = dw;
                    break;
                }
                if (w is MemoryWindow mw && mw.GetHoverPopup() != null && mw.GetHoverPopup().Visible && mw.GetHoverPopup().Bounds.Contains(mousePos))
                {
                    popupOwner = mw;
                    break;
                }
                if (w is MemoryMapWindow mmw && mmw.GetHoverPopup() != null && mmw.GetHoverPopup().Visible && mmw.GetHoverPopup().Bounds.Contains(mousePos))
                {
                    popupOwner = mmw;
                    break;
                }
                if (w is StatusBarWindow sbw && sbw.HoverPopup != null && sbw.HoverPopup.Visible && sbw.HoverPopup.Bounds.Contains(mousePos))
                {
                    popupOwner = sbw;
                    break;
                }
                if (w is RegisterWindow rw && rw.GetHoverPopup() != null && rw.GetHoverPopup().Visible && rw.GetHoverPopup().Bounds.Contains(mousePos))
                {
                    popupOwner = rw;
                    break;
                }
            }

            Window topUnderMouse = null;
            if (popupOwner == null)
            {
                for (int i = Windows.Count - 1; i >= 0; i--)
                {
                    var w = Windows[i];
                    if (!w.Visible) continue;
                    if (w.Bounds.Contains(mousePos))
                    {
                        topUnderMouse = w;
                        break;
                    }
                }
            }

            foreach (var w in Windows)
            {
                if (!w.Visible)
                    continue;

                bool mouseOverPopup = popupOwner != null;
                if (mouseOverPopup)
                {
                    // Only the popup owner processes updates when mouse is over a popup
                    if (w == popupOwner)
                        w.Update(gameTime);
                    else if (!w.Bounds.Contains(mousePos))
                        w.Update(gameTime);
                }
                else
                {
                    // Without popup: only the top-most window under the mouse processes hover-sensitive updates
                    if (topUnderMouse == null || w == topUnderMouse || !w.Bounds.Contains(mousePos))
                        w.Update(gameTime);
                }
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
                else if (w.Visible && w is MemoryMapWindow mmw && mmw.GetHoverPopup() != null && mmw.GetHoverPopup().Visible)
                {
                    mmw.GetHoverPopup().Update(gameTime);
                }
                else if (w.Visible && w is StatusBarWindow sbw && sbw.HoverPopup != null && sbw.HoverPopup.Visible)
                {
                    sbw.HoverPopup.Update(gameTime);
                }
                else if (w.Visible && w is RegisterWindow rw && rw.GetHoverPopup() != null && rw.GetHoverPopup().Visible)
                {
                    rw.GetHoverPopup().Update(gameTime);
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
                else if (w.Visible && w is MemoryMapWindow mmw && mmw.GetHoverPopup() != null && mmw.GetHoverPopup().Visible)
                {
                    mmw.GetHoverPopup().Draw();
                }
                else if (w.Visible && w is StatusBarWindow sbw && sbw.HoverPopup != null && sbw.HoverPopup.Visible)
                {
                    sbw.HoverPopup.Draw();
                }
                else if (w.Visible && w is RegisterWindow rw && rw.GetHoverPopup() != null && rw.GetHoverPopup().Visible)
                {
                    rw.GetHoverPopup().Draw();
                }
            }
        }
    }
}
