using System;
using Continuum93.Emulator;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Continuum93.ServiceModule.UI
{
    public class Taskbar
    {
        private readonly WindowManager _manager;

        private const int BarWidth = 200;
        private const int HeaderHeight = 40;
        private const int ItemHeight = 28;
        private const int ItemSpacing = 2;

        private const int SnapDetectDistance = 8; // not for windows, just feel threshold

        private float _openAmount;            // 0 = closed, 1 = open
        private const float OpenSpeed = 10f;  // slide speed

        private int _scrollOffset;
        private int _contentHeight;

        private MouseState _prevMouse;

        public Taskbar(WindowManager manager)
        {
            _manager = manager;
        }

        /// <summary>
        /// Updates animation, hover-scroll and handles clicks.
        /// Returns true if the taskbar consumed the mouse input this frame.
        /// Call this BEFORE WindowManager.HandleInput.
        /// </summary>
        public bool Update(GameTime gameTime, MouseState mouse, MouseState prevMouse)
        {
            bool captured = false;

            var device = Renderer.GetGraphicsDevice();
            Rectangle screen = Renderer.GetScreenBounds();

            int screenW = screen.Width;
            int screenH = screen.Height;

            // Current bar rect for drawing / hit-testing
            Rectangle barRect = GetBarRect(screenW, screenH);

            // A fixed hot zone on the far right edge (always active)
            const int HotZoneWidth = 4;
            Rectangle hotZoneRect = new Rectangle(
                screenW - HotZoneWidth,
                0,
                HotZoneWidth,
                screenH
            );

            bool mouseInHotZone = hotZoneRect.Contains(mouse.Position);
            bool mouseOnBarSurface = barRect.Width > 0 && barRect.Contains(mouse.Position);

            // OPEN/CLOSE RULES:
            // - If mouse is on the bar surface → stay open
            // - Else if mouse is in the hot zone → open
            // - Else → close
            float targetOpen;
            if (mouseOnBarSurface)
                targetOpen = 1f;
            else if (mouseInHotZone)
                targetOpen = 1f;
            else
                targetOpen = 0f;

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _openAmount = MathHelper.Lerp(_openAmount, targetOpen, dt * OpenSpeed);
            _openAmount = MathHelper.Clamp(_openAmount, 0f, 1f);

            // Recompute bar rect after _openAmount changed
            barRect = GetBarRect(screenW, screenH);

            // Mac-like scroll: move mouse up/down while inside the (visible) bar area
            if (_openAmount > 0.2f && barRect.Contains(mouse.Position))
            {
                int dy = mouse.Y - prevMouse.Y;
                _scrollOffset -= dy;
            }

            // Handle clicks only if mostly open, and inside the visible bar
            if (_openAmount > 0.2f)
            {
                bool leftPressed = mouse.LeftButton == ButtonState.Pressed;
                bool leftJustPressed = leftPressed && prevMouse.LeftButton == ButtonState.Released;

                if (leftJustPressed && barRect.Contains(mouse.Position))
                {
                    // Header/settings button
                    Rectangle headerRect = new Rectangle(
                        barRect.X + 4,
                        barRect.Y + 4,
                        barRect.Width - 8,
                        HeaderHeight - 8
                    );

                    if (headerRect.Contains(mouse.Position))
                    {
                        SpawnDummyWindow();
                        captured = true;
                    }
                    else
                    {
                        // Hit-test window bricks
                        int y = barRect.Y + HeaderHeight - _scrollOffset;

                        foreach (var w in _manager.Windows)
                        {
                            Rectangle itemRect = new Rectangle(
                                barRect.X + 4,
                                y,
                                barRect.Width - 8,
                                ItemHeight
                            );

                            if (itemRect.Contains(mouse.Position))
                            {
                                if (!w.Visible)
                                {
                                    w.Visible = true;
                                    _manager.BringToFront(w);
                                }
                                else
                                {
                                    // minimize
                                    w.Visible = false;
                                }

                                captured = true;
                                break;
                            }

                            y += ItemHeight + ItemSpacing;
                        }
                    }
                }
            }

            _prevMouse = mouse;
            return captured;
        }



        private Rectangle GetBarRect(int screenW, int screenH)
        {
            int visibleWidth = (int)(BarWidth * _openAmount);
            return new Rectangle(screenW - visibleWidth, 0, visibleWidth, screenH);
        }

        public void Draw()
        {
            if (_openAmount <= 0.01f)
                return;

            var device = Renderer.GetGraphicsDevice();
            var spriteBatch = Renderer.GetSpriteBatch();
            var pixel = Renderer.GetPixelTexture();

            Rectangle screen = Renderer.GetScreenBounds();
            int screenW = screen.Width;
            int screenH = screen.Height;

            Rectangle barRect = GetBarRect(screenW, screenH);

            // Background + border
            Color bg = new Color(10, 10, 20, 220);
            Color border = new Color(80, 80, 120);

            spriteBatch.Draw(pixel, barRect, bg);

            // left border line
            spriteBatch.Draw(
                pixel,
                new Rectangle(barRect.X, barRect.Y, 1, barRect.Height),
                border
            );

            // HEADER: neutral "settings" button
            Rectangle headerRect = new Rectangle(
                barRect.X + 4,
                barRect.Y + 4,
                barRect.Width - 8,
                HeaderHeight - 8
            );

            spriteBatch.Draw(pixel, headerRect, new Color(30, 30, 60));

            ServiceGraphics.DrawText(
                Fonts.ModernDOS_12x18,
                "[ Settings ]",
                headerRect.X + 6,
                headerRect.Y + 4,
                headerRect.Width - 12,
                Color.Cyan,
                Color.Black,
                (byte)ServiceFontFlags.DrawOutline,
                0xFF
            );

            // WINDOW BRICKS
            int y = barRect.Y + HeaderHeight - _scrollOffset;
            _contentHeight = 0;

            foreach (var w in _manager.Windows)
            {
                Rectangle itemRect = new Rectangle(
                    barRect.X + 4,
                    y,
                    barRect.Width - 8,
                    ItemHeight
                );

                _contentHeight += ItemHeight + ItemSpacing;

                // skip if completely outside visible bar area
                if (itemRect.Bottom < barRect.Top || itemRect.Top > barRect.Bottom)
                {
                    y += ItemHeight + ItemSpacing;
                    continue;
                }

                bool isActive = w.IsOnTop && w.Visible;
                bool isMinimized = !w.Visible;

                Color itemBg;
                if (isActive) itemBg = new Color(80, 80, 140);
                else if (isMinimized) itemBg = new Color(20, 20, 20);
                else itemBg = new Color(40, 40, 60);

                spriteBatch.Draw(pixel, itemRect, itemBg);

                ServiceGraphics.DrawText(
                    Fonts.ModernDOS_12x18_thin,
                    w.Title,
                    itemRect.X + 6,
                    itemRect.Y + 4,
                    itemRect.Width - 12,
                    Color.White,
                    Color.Black,
                    (byte)ServiceFontFlags.DrawOutline,
                    0xFF
                );

                y += ItemHeight + ItemSpacing;
            }

            // Clamp scroll after computing content height
            int visibleArea = barRect.Height - HeaderHeight;
            if (_contentHeight <= visibleArea)
            {
                _scrollOffset = 0;
            }
            else
            {
                int maxScroll = _contentHeight - visibleArea;
                if (_scrollOffset < 0) _scrollOffset = 0;
                if (_scrollOffset > maxScroll) _scrollOffset = maxScroll;
            }
        }

        /// <summary>
        /// Placeholder action for the top "settings" button:
        /// spawns a small TextWindow. Replace this later.
        /// </summary>
        private void SpawnDummyWindow()
        {
            if (_manager == null) return;

            var rnd = new Random();

            var device = Renderer.GetGraphicsDevice();
            int screenW = device.Viewport.Width;
            int screenH = device.Viewport.Height;

            int x = 40 + rnd.Next(120);
            int y = 40 + rnd.Next(80);

            int w = Math.Min(360, screenW - x - 40);
            int h = Math.Min(220, screenH - y - 40);

            var win = new TextWindow(
                "Taskbar test",
                () => "This window was spawned from the taskbar settings button.\n\n" +
                "You can replace SpawnDummyWindow() with a real settings UI later.",
                x, y, w, h,
                spawnDelaySeconds: 0.05f,
                canResize: true,
                canClose: true
            );

            _manager.Add(win);
            _manager.BringToFront(win);
        }
    }
}
