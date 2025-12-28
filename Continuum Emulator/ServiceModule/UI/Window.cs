using Continuum93.Emulator;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Continuum93.ServiceModule.UI
{
    public abstract class Window
    {
        private const int SnapGap = 4;          // final distance between things
        private const int SnapDetectDistance = 8; // how close before we snap

        public static int TitleBarHeight = 24;
        public static int BorderWidth = 1;     // 1px minimalistic border
        public static int MinWidth = 100;
        public static int MaxWidth = 1920;     // or int.MaxValue
        public static int MinHeight = 80;
        public static int MaxHeight = 1200;
        public static int Padding = 8;
        public static int ResizeGripSize = 12;

        public WindowManager Manager { get; internal set; }

        public string Title;
        public int X;
        public int Y;
        public int Width;
        public int Height;
        public bool Visible = true;
        public bool RefreshRequired = true;
        public bool IsFocused;
        public bool IsOnTop = false;

        // Spawn animation
        private float _spawnTime = 0f;             // elapsed time
        private float _spawnDuration = 1f;      // growth animation
        private int _initialTargetHeight;          // full intended height
        private float _spawnDelay = 0f;            // in seconds
        private bool _isSpawning = true;           // window currently animating in

        private float _spawnFade = 0f; // 0 = invisible, 1 = fully visible

        private bool _canResize = true;
        private bool _canClose = false;
        private Rectangle CloseButtonRect => new Rectangle(X + Width - 20, Y + 4, 16, 16);

        // dragging
        private bool _isDragging;
        private Point _dragOffset;

        // resizing
        private bool _isResizing;
        private Point _resizeStartMouse;
        private Point _resizeStartSize;
        private Point _resizeStartPos;
        private int _resizeStartRight;
        private int _resizeStartBottom;

        private bool _resizeFromLeft;
        private bool _resizeFromRight;
        private bool _resizeFromTop;
        private bool _resizeFromBottom;

        // easing
        private float _focusBlend = 0f; // 0 = unfocused, 1 = focused
        private const float FocusLerpSpeed = 5f; // approx. 200 ms (5 → because 1/5 ≈ 0.2s)

        public Window(string title, int x, int y, int width, int height,
                 float spawnDelaySeconds = 0f, bool canResize = true, bool canClose = false)
        {
            Title = title;
            X = x;
            Y = y;
            Width = width;

            _initialTargetHeight = height;
            Height = TitleBarHeight;
            _spawnDelay = spawnDelaySeconds;

            _canResize = canResize;
            _canClose = canClose;
        }


        #region Geometry helpers

        public Rectangle Bounds =>
            new(X, Y, Width, Height);

        public Rectangle TitleBarRect =>
            new(X, Y, Width, TitleBarHeight);

        public Rectangle ContentRect =>
            new(
                X + BorderWidth,
                Y + TitleBarHeight,
                Width - BorderWidth * 2,
                Height - TitleBarHeight - BorderWidth
            );

        public Rectangle ResizeGripRect =>
            new(
                X + Width - ResizeGripSize,
                Y + Height - ResizeGripSize,
                ResizeGripSize,
                ResizeGripSize
            );

        #endregion

        /// <summary>
        /// Called each frame to process input that might affect this window.
        /// Returns true if the window captured the mouse this frame.
        /// </summary>
        public virtual bool HandleInput(MouseState mouse, MouseState prevMouse)
        {
            if (!Visible)
                return false;

            Point mousePos = new Point(mouse.X, mouse.Y);
            bool leftPressed = mouse.LeftButton == ButtonState.Pressed;
            bool leftJustPressed = leftPressed && prevMouse.LeftButton == ButtonState.Released;
            bool leftJustReleased = !leftPressed && prevMouse.LeftButton == ButtonState.Pressed;

            // Close button
            if (_canClose && leftJustPressed && CloseButtonRect.Contains(mousePos))
            {
                Manager?.Remove(this);
                return true;
            }

            // --- Determine which edge/corner we're on for resizing ---
            ResizeHit hit = ResizeHit.None;
            if (_canResize && !_isResizing && !_isDragging)
            {
                hit = GetResizeHit(mousePos);
            }

            // --- Start resize from any edge / bottom-right corner ---
            if (_canResize && leftJustPressed && hit != ResizeHit.None && !_isDragging)
            {
                _isResizing = true;
                IsFocused = true;

                _resizeFromLeft = _resizeFromRight = _resizeFromTop = _resizeFromBottom = false;

                switch (hit)
                {
                    case ResizeHit.Left:
                        _resizeFromLeft = true;
                        break;
                    case ResizeHit.Right:
                        _resizeFromRight = true;
                        break;
                    case ResizeHit.Top:
                        _resizeFromTop = true;
                        break;
                    case ResizeHit.Bottom:
                        _resizeFromBottom = true;
                        break;
                    case ResizeHit.BottomRight:
                        _resizeFromRight = true;
                        _resizeFromBottom = true;
                        break;
                }

                _resizeStartMouse = mousePos;
                _resizeStartSize = new Point(Width, Height);
                _resizeStartPos = new Point(X, Y);
                _resizeStartRight = X + Width;
                _resizeStartBottom = Y + Height;

                return true;
            }

            // --- Start drag from title bar (only if NOT on a resize zone) ---
            if (leftJustPressed && hit == ResizeHit.None && TitleBarRect.Contains(mousePos) && !_isResizing)
            {
                _isDragging = true;
                IsFocused = true;
                _dragOffset = new Point(mousePos.X - X, mousePos.Y - Y);
                return true;
            }

            // --- Dragging logic ---
            if (_isDragging)
            {
                if (leftPressed)
                {
                    X = mousePos.X - _dragOffset.X;
                    Y = mousePos.Y - _dragOffset.Y;
                }

                ClampToScreen();
                SnapToOtherWindows();

                if (leftJustReleased)
                {
                    _isDragging = false;
                }
                return true;
            }

            // --- Resizing logic ---
            if (_canResize && _isResizing)
            {
                if (leftPressed)
                {
                    int deltaX = mousePos.X - _resizeStartMouse.X;
                    int deltaY = mousePos.Y - _resizeStartMouse.Y;

                    // Start from original bounds
                    int newX = _resizeStartPos.X;
                    int newY = _resizeStartPos.Y;
                    int newWidth = _resizeStartSize.X;
                    int newHeight = _resizeStartSize.Y;

                    // Horizontal resize
                    if (_resizeFromLeft)
                    {
                        newX = _resizeStartPos.X + deltaX;
                        newWidth = _resizeStartRight - newX;   // keep right edge anchored
                    }
                    else if (_resizeFromRight)
                    {
                        newWidth = _resizeStartSize.X + deltaX; // keep left edge anchored
                    }

                    // Vertical resize
                    if (_resizeFromTop)
                    {
                        newY = _resizeStartPos.Y + deltaY;
                        newHeight = _resizeStartBottom - newY;  // keep bottom edge anchored
                    }
                    else if (_resizeFromBottom)
                    {
                        newHeight = _resizeStartSize.Y + deltaY; // keep top edge anchored
                    }

                    // Clamp sizes, adjust anchor if too small
                    if (newWidth < MinWidth)
                    {
                        int diff = MinWidth - newWidth;
                        newWidth = MinWidth;
                        if (_resizeFromLeft)
                            newX -= diff;
                    }
                    if (newWidth > MaxWidth)
                    {
                        newWidth = MaxWidth;
                    }

                    if (newHeight < MinHeight)
                    {
                        int diff = MinHeight - newHeight;
                        newHeight = MinHeight;
                        if (_resizeFromTop)
                            newY -= diff;
                    }
                    if (newHeight > MaxHeight)
                    {
                        newHeight = MaxHeight;
                    }

                    // Let snapping adjust the proposed rect (with magnetic easing)
                    SnapResizeToOtherWindows(
                        ref newX, ref newY,
                        ref newWidth, ref newHeight,
                        _resizeFromLeft, _resizeFromRight,
                        _resizeFromTop, _resizeFromBottom
                    );

                    X = newX;
                    Y = newY;
                    Width = newWidth;
                    Height = newHeight;

                    OnResized();
                }

                if (leftJustReleased)
                {
                    _isResizing = false;
                    _resizeFromLeft = _resizeFromRight = _resizeFromTop = _resizeFromBottom = false;
                }
                return true;
            }

            // Click focusing (inside window)
            if (leftJustPressed && Bounds.Contains(mousePos))
            {
                IsFocused = true;
                return true;
            }

            return false;
        }


        /// <summary>
        /// Per-frame logic (not input) for this window.
        /// </summary>
        public virtual void Update(GameTime gameTime)
        {
            if (!Visible) return;

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            float target = IsFocused ? 1f : 0f;

            // SPAWN DELAY HANDLING
            if (_isSpawning)
            {
                // wait if delayed
                if (_spawnDelay > 0f)
                {
                    _spawnDelay -= dt;
                    return;
                }

                _spawnTime += dt;
                float t = MathHelper.Clamp(_spawnTime / _spawnDuration, 0f, 1f);

                // Smooth easing
                float eased = t * t * (3f - 2f * t);

                // Height grows from title bar → target
                Height = (int)MathHelper.Lerp(TitleBarHeight, _initialTargetHeight, eased);

                // Window fades in (used in DrawChrome)
                _spawnFade = eased;

                if (t >= 1f)
                    _isSpawning = false;
            }

            // Smoothly move _focusBlend toward target
            _focusBlend = MathHelper.Lerp(_focusBlend, target, dt * FocusLerpSpeed);

            UpdateContent(gameTime);
        }

        /// <summary>
        /// Main draw entry point.
        /// </summary>
        public void Draw()
        {
            if (!Visible) return;

            var spriteBatch = Renderer.GetSpriteBatch();
            // assume you have some 1x1 white texture; adapt name as needed
            var pixel = Renderer.GetPixelTexture();

            DrawChrome(spriteBatch, pixel);
            //DrawContent(spriteBatch, ContentRect);

            DrawContentClipped(spriteBatch, ContentRect);
        }

        /// <summary>
        /// Draws the border + title bar.
        /// </summary>
        protected virtual void DrawChrome(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Microsoft.Xna.Framework.Graphics.Texture2D pixel)
        {
            var bounds = Bounds;

            // Border (1px)
            Rectangle top = new(bounds.X, bounds.Y, bounds.Width, BorderWidth);
            Rectangle bottom = new(bounds.X, bounds.Bottom - BorderWidth, bounds.Width, BorderWidth);
            Rectangle left = new(bounds.X, bounds.Y, BorderWidth, bounds.Height);
            Rectangle right = new(bounds.Right - BorderWidth, bounds.Y, BorderWidth, bounds.Height);

            var theme = ServiceGraphics.Theme;
            Color borderColor = theme.WindowBorder;

            // Base colors
            Color unfocused = theme.WindowTitleBarUnfocused;
            Color focused = theme.WindowTitleBarFocused;   // normal focused
            Color onTop = theme.WindowTitleBarOnTop;  // lighter for top-most

            Color titleBarColor;

            // If this window is on top, lerp toward the brighter color
            if (IsOnTop)
            {
                titleBarColor = Color.Lerp(focused, onTop, _focusBlend);
            }
            else
            {
                titleBarColor = Color.Lerp(unfocused, focused, _focusBlend);
            }

            Color backgroundColor = Color.Lerp(
                theme.WindowBackgroundUnfocused,
                theme.WindowBackgroundFocused,
                _focusBlend
            );

            // Background
            spriteBatch.Draw(pixel, bounds, backgroundColor);

            // Title bar area
            spriteBatch.Draw(pixel, TitleBarRect, titleBarColor);

            // Border
            spriteBatch.Draw(pixel, top, borderColor);
            spriteBatch.Draw(pixel, bottom, borderColor);
            spriteBatch.Draw(pixel, left, borderColor);
            spriteBatch.Draw(pixel, right, borderColor);

            // Resize grip (simple small triangle / square)
            if (_canResize)
                spriteBatch.Draw(pixel, ResizeGripRect, theme.WindowResizeGrip);

            // Title text (using your ServiceGraphics)
            ServiceGraphics.DrawText(
                Fonts.ModernDOS_12x18,
                Title,
                bounds.X + Padding,
                bounds.Y + 4,       // vertically center-ish in title bar
                bounds.Width - Padding * 2,
                theme.WindowTitleText,
                theme.TextOutline,
                (byte)(ServiceFontFlags.DrawOutline),
                0xFF
            );

            // Draw close button
            if (_canClose)
            {
                spriteBatch.Draw(pixel, CloseButtonRect, theme.WindowCloseButton);
                ServiceGraphics.DrawText(
                    Fonts.ModernDOS_12x18,
                    "X",
                    CloseButtonRect.X + 3,
                    CloseButtonRect.Y - 2,
                    20,
                    theme.WindowCloseButtonText,
                    theme.TextOutline,
                    (byte)ServiceFontFlags.DrawOutline,
                    0xFF
                );
            }
        }

        /// <summary>
        /// Called when the window is resized.
        /// </summary>
        protected virtual void OnResized()
        {
            RefreshRequired = true;
        }

        /// <summary>
        /// Derived windows override this to update their own internal state.
        /// </summary>
        protected virtual void UpdateContent(GameTime gameTime) { }

        /// <summary>
        /// Derived windows override this to draw their content inside ContentRect.
        /// </summary>
        protected abstract void DrawContent(SpriteBatch spriteBatch, Rectangle contentRect);

        protected void DrawContentClipped(SpriteBatch spriteBatch, Rectangle contentRect)
        {
            var device = Renderer.GetGraphicsDevice();

            // Save old state
            var oldScissor = device.ScissorRectangle;
            var oldRaster = device.RasterizerState;

            // End the outer batch so we can start a clipped one
            spriteBatch.End();

            // Set up clipping rectangle (add padding)
            Rectangle scissor = new Rectangle(
                contentRect.X + 4,
                contentRect.Y + 4,
                contentRect.Width - 8,
                contentRect.Height - 8
            );

            // Clamp to backbuffer just in case
            scissor = Rectangle.Intersect(scissor,
                new Rectangle(0, 0,
                    device.Viewport.Width,
                    device.Viewport.Height));

            device.ScissorRectangle = scissor;

            var raster = new RasterizerState() { ScissorTestEnable = true };

            spriteBatch.Begin(
                rasterizerState: raster,
                blendState: BlendState.AlphaBlend,
                samplerState: SamplerState.PointClamp
            );

            // Now draw the actual content
            DrawContent(spriteBatch, contentRect);

            spriteBatch.End();

            // Restore previous scissor + restart normal batch
            device.ScissorRectangle = oldScissor;
            spriteBatch.Begin(
                blendState: BlendState.AlphaBlend,
                samplerState: SamplerState.PointClamp
            );
        }

        private void ClampToScreen()
        {
            var device = Renderer.GetGraphicsDevice();
            int screenW = device.Viewport.Width;
            int screenH = device.Viewport.Height;

            // Prevent dragging window entirely off left/right
            if (X < 0)
                X = 0;
            if (X + Width > screenW)
                X = screenW - Width;

            // Prevent dragging window above top edge (title bar must stay visible)
            if (Y < 0)
                Y = 0;

            // Prevent dragging window past bottom edge
            if (Y + Height > screenH)
                Y = screenH - Height;
        }

        private void SnapToOtherWindows()
        {
            if (Manager == null) return;

            int left = X;
            int right = X + Width;
            int top = Y;
            int bottom = Y + Height;

            foreach (var other in Manager.Windows)
            {
                if (other == this || !other.Visible)
                    continue;

                int oLeft = other.X;
                int oRight = other.X + other.Width;
                int oTop = other.Y;
                int oBottom = other.Y + other.Height;

                bool verticalOverlap = bottom > oTop && top < oBottom;
                bool horizontalOverlap = right > oLeft && left < oRight;

                // stacked (one above/below) with SnapGap
                bool verticallyAttached =
                    Math.Abs(top - (oBottom + SnapGap)) <= SnapDetectDistance ||
                    Math.Abs(bottom - (oTop - SnapGap)) <= SnapDetectDistance;

                // side-by-side (one left/right) with SnapGap
                bool horizontallyAttached =
                    Math.Abs(left - (oRight + SnapGap)) <= SnapDetectDistance ||
                    Math.Abs(right - (oLeft - SnapGap)) <= SnapDetectDistance;

                // --- Horizontal snapping (left/right, side-by-side) ---
                if (verticalOverlap)
                {
                    // Snap this.left to other.right + gap
                    int targetLeft = oRight + SnapGap;
                    int snappedLeft = MagneticSnap(left, targetLeft);
                    if (snappedLeft != left)
                    {
                        X = snappedLeft;
                        left = X;
                        right = X + Width;
                    }

                    // Snap this.right to other.left - gap
                    int targetRight = oLeft - SnapGap;
                    int snappedRight = MagneticSnap(right, targetRight);
                    if (snappedRight != right)
                    {
                        X = snappedRight - Width;
                        left = X;
                        right = X + Width;
                    }
                }

                // --- Vertical snapping (top/bottom, stacked horizontally) ---
                if (horizontalOverlap)
                {
                    // Snap this.top to other.bottom + gap
                    int targetTop = oBottom + SnapGap;
                    int snappedTop = MagneticSnap(top, targetTop);
                    if (snappedTop != top)
                    {
                        Y = snappedTop;
                        top = Y;
                        bottom = Y + Height;
                    }

                    // Snap this.bottom to other.top - gap
                    int targetBottom = oTop - SnapGap;
                    int snappedBottom = MagneticSnap(bottom, targetBottom);
                    if (snappedBottom != bottom)
                    {
                        Y = snappedBottom - Height;
                        top = Y;
                        bottom = Y + Height;
                    }
                }

                // --- HORIZONTAL ALIGNMENT when stacked (one above/below the other) ---
                if (verticallyAttached)
                {
                    // Align LEFT edges
                    int snappedLeftAlign = MagneticSnap(left, oLeft);
                    if (snappedLeftAlign != left)
                    {
                        X = snappedLeftAlign;
                        left = X;
                        right = X + Width;
                    }

                    // Align RIGHT edges
                    int snappedRightAlign = MagneticSnap(right, oRight);
                    if (snappedRightAlign != right)
                    {
                        X = snappedRightAlign - Width;
                        left = X;
                        right = X + Width;
                    }
                }

                // --- VERTICAL ALIGNMENT when side-by-side (one left/right of the other) ---
                if (horizontallyAttached)
                {
                    // Align TOP edges
                    int snappedTopAlign = MagneticSnap(top, oTop);
                    if (snappedTopAlign != top)
                    {
                        Y = snappedTopAlign;
                        top = Y;
                        bottom = Y + Height;
                    }

                    // Align BOTTOM edges
                    int snappedBottomAlign = MagneticSnap(bottom, oBottom);
                    if (snappedBottomAlign != bottom)
                    {
                        Y = snappedBottomAlign - Height;
                        top = Y;
                        bottom = Y + Height;
                    }
                }
            }

            // --- Snap to screen edges with a SnapGap margin ---
            var device = Renderer.GetGraphicsDevice();
            int screenW = device.Viewport.Width;
            int screenH = device.Viewport.Height;

            int snappedScreenLeft = MagneticSnap(X, SnapGap);
            if (snappedScreenLeft != X)
                X = snappedScreenLeft;

            int snappedScreenRight = MagneticSnap(X + Width, screenW - SnapGap);
            if (snappedScreenRight != X + Width)
                X = snappedScreenRight - Width;

            int snappedScreenTop = MagneticSnap(Y, SnapGap);
            if (snappedScreenTop != Y)
                Y = snappedScreenTop;

            int snappedScreenBottom = MagneticSnap(Y + Height, screenH - SnapGap);
            if (snappedScreenBottom != Y + Height)
                Y = snappedScreenBottom - Height;
        }

        private void SnapResizeToOtherWindows(
            ref int newX, ref int newY,
            ref int newWidth, ref int newHeight,
            bool fromLeft, bool fromRight,
            bool fromTop, bool fromBottom)
        {
            if (Manager == null) return;

            int newLeft = newX;
            int newTop = newY;
            int newRight = newX + newWidth;
            int newBottom = newY + newHeight;

            foreach (var other in Manager.Windows)
            {
                if (other == this || !other.Visible)
                    continue;

                int oLeft = other.X;
                int oRight = other.X + other.Width;
                int oTop = other.Y;
                int oBottom = other.Y + other.Height;

                bool horizontalOverlap = newRight > oLeft && newLeft < oRight;
                bool verticalOverlap = newBottom > oTop && newTop < oBottom;

                // stacked with SnapGap
                bool verticallyAttached =
                    Math.Abs(newTop - (oBottom + SnapGap)) <= SnapDetectDistance ||
                    Math.Abs(newBottom - (oTop - SnapGap)) <= SnapDetectDistance;

                // side-by-side with SnapGap
                bool horizontallyAttached =
                    Math.Abs(newLeft - (oRight + SnapGap)) <= SnapDetectDistance ||
                    Math.Abs(newRight - (oLeft - SnapGap)) <= SnapDetectDistance;

                // --- Vertical resizing (bottom / top edges) ---
                // Allow when overlapping horizontally OR side-by-side with SnapGap
                if (horizontalOverlap || horizontallyAttached)
                {
                    if (fromBottom)
                    {
                        // Snap bottom to just above other's top (gap)
                        int targetBottomAbove = oTop - SnapGap;
                        int snappedBottom = MagneticSnap(newBottom, targetBottomAbove);
                        if (snappedBottom != newBottom)
                        {
                            newBottom = snappedBottom;
                            newHeight = newBottom - newY;
                        }

                        // Snap bottom to just below other's bottom (gap)
                        int targetBottomBelow = oBottom + SnapGap;
                        snappedBottom = MagneticSnap(newBottom, targetBottomBelow);
                        if (snappedBottom != newBottom)
                        {
                            newBottom = snappedBottom;
                            newHeight = newBottom - newY;
                        }

                        // Alignment when side-by-side or overlapping: match bottoms exactly
                        int targetBottomAlign = oBottom;
                        snappedBottom = MagneticSnap(newBottom, targetBottomAlign);
                        if (snappedBottom != newBottom)
                        {
                            newBottom = snappedBottom;
                            newHeight = newBottom - newY;
                        }
                    }

                    if (fromTop)
                    {
                        // Snap top to just below other's bottom (gap)
                        int targetTopBelow = oBottom + SnapGap;
                        int snappedTop = MagneticSnap(newTop, targetTopBelow);
                        if (snappedTop != newTop)
                        {
                            newTop = snappedTop;
                            newY = newTop;
                            newHeight = _resizeStartBottom - newY; // keep bottom anchored
                            newBottom = newY + newHeight;
                        }

                        // Alignment when side-by-side or overlapping: match tops exactly
                        int targetTopAlign = oTop;
                        snappedTop = MagneticSnap(newTop, targetTopAlign);
                        if (snappedTop != newTop)
                        {
                            newTop = snappedTop;
                            newY = newTop;
                            newHeight = _resizeStartBottom - newY; // keep bottom anchored
                            newBottom = newY + newHeight;
                        }
                    }
                }

                // --- Horizontal resizing (right / left edges) ---
                // Works for overlapping OR stacked vertically
                if (verticalOverlap || verticallyAttached)
                {
                    if (fromRight)
                    {
                        // Side-by-side gap snaps
                        int targetRightLeft = oLeft - SnapGap;
                        int snappedRight = MagneticSnap(newRight, targetRightLeft);
                        if (snappedRight != newRight)
                        {
                            newRight = snappedRight;
                            newWidth = newRight - newX;
                        }

                        int targetRightRight = oRight + SnapGap;
                        snappedRight = MagneticSnap(newRight, targetRightRight);
                        if (snappedRight != newRight)
                        {
                            newRight = snappedRight;
                            newWidth = newRight - newX;
                        }

                        // Alignment when stacked: match right edges exactly
                        int targetRightAlign = oRight;
                        snappedRight = MagneticSnap(newRight, targetRightAlign);
                        if (snappedRight != newRight)
                        {
                            newRight = snappedRight;
                            newWidth = newRight - newX;
                        }
                    }

                    if (fromLeft)
                    {
                        // Side-by-side gap snap: left to right+gap
                        int targetLeftRightOfOther = oRight + SnapGap;
                        int snappedLeft = MagneticSnap(newLeft, targetLeftRightOfOther);
                        if (snappedLeft != newLeft)
                        {
                            newLeft = snappedLeft;
                            newX = newLeft;
                            newWidth = _resizeStartRight - newX; // keep right anchored
                            newRight = newX + newWidth;
                        }

                        // Alignment when stacked: match left edges exactly
                        int targetLeftAlign = oLeft;
                        snappedLeft = MagneticSnap(newLeft, targetLeftAlign);
                        if (snappedLeft != newLeft)
                        {
                            newLeft = snappedLeft;
                            newX = newLeft;
                            newWidth = _resizeStartRight - newX; // keep right anchored
                            newRight = newX + newWidth;
                        }
                    }
                }
            }

            // Push results back
            newX = newLeft;
            newY = newTop;
            newWidth = newRight - newLeft;
            newHeight = newBottom - newTop;
        }





        private int MagneticSnap(int current, int target)
        {
            int dist = Math.Abs(current - target);
            if (dist <= SnapDetectDistance)
                return target;    // within radius → snap hard
            return current;        // too far → no snap
        }



        // Thin resize zones on left and top edges
        public Rectangle LeftResizeRect =>
            new(
                X,
                Y + TitleBarHeight,                   // avoid overlapping close/title bar
                BorderWidth * 4,                      // ~4 px thick
                Height - TitleBarHeight - BorderWidth
            );

        public Rectangle TopResizeRect =>
            new(
                X + BorderWidth,
                Y,
                Width - BorderWidth * 2,
                BorderWidth * 4                       // ~4 px thick
            );

        public Rectangle RightResizeRect =>
            new(
                X + Width - BorderWidth * 4,
                Y + TitleBarHeight,
                BorderWidth * 4,
                Height - TitleBarHeight - BorderWidth
            );

        public Rectangle BottomResizeRect =>
            new(
                X + BorderWidth,
                Y + Height - BorderWidth * 4,
                Width - BorderWidth * 2,
                BorderWidth * 4
            );

        private enum ResizeHit
        {
            None,
            Left,
            Right,
            Top,
            Bottom,
            BottomRight
        }

        private ResizeHit GetResizeHit(Point p)
        {
            if (!_canResize)
                return ResizeHit.None;

            // Corner first: bottom-right grip = diagonal resize
            if (ResizeGripRect.Contains(p))
                return ResizeHit.BottomRight;

            if (LeftResizeRect.Contains(p))
                return ResizeHit.Left;

            if (RightResizeRect.Contains(p))
                return ResizeHit.Right;

            if (TopResizeRect.Contains(p))
                return ResizeHit.Top;

            if (BottomResizeRect.Contains(p))
                return ResizeHit.Bottom;

            return ResizeHit.None;
        }

        public virtual void UpdateCursor(MouseState mouse)
        {
            if (!_canResize || !Visible)
                return;

            Point p = new Point(mouse.X, mouse.Y);

            // While actively resizing, keep the resize cursor even if mouse slightly
            // slips off the exact edge rect.
            if (_isResizing)
            {
                Mouse.SetCursor(GetCursorForResizeFlags());
                return;
            }

            var hit = GetResizeHit(p);

            switch (hit)
            {
                case ResizeHit.Left:
                case ResizeHit.Right:
                    Mouse.SetCursor(MouseCursor.SizeWE);
                    break;

                case ResizeHit.Top:
                case ResizeHit.Bottom:
                    Mouse.SetCursor(MouseCursor.SizeNS);
                    break;

                case ResizeHit.BottomRight:
                    Mouse.SetCursor(MouseCursor.SizeNWSE); // diag ↘↖
                    break;

                case ResizeHit.None:
                default:
                    // Let WindowManager / others keep the default Arrow
                    break;
            }
        }

        private MouseCursor GetCursorForResizeFlags()
        {
            bool horz = _resizeFromLeft || _resizeFromRight;
            bool vert = _resizeFromTop || _resizeFromBottom;

            if (horz && vert)
                return MouseCursor.SizeNWSE; // we only use bottom-right diag for now

            if (horz)
                return MouseCursor.SizeWE;

            if (vert)
                return MouseCursor.SizeNS;

            return MouseCursor.Arrow;
        }


    }
}
