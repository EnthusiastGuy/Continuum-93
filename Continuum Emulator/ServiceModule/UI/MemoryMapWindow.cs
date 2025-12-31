using Continuum93.CodeAnalysis;
using Continuum93.Emulator;
using Continuum93.Emulator.RAM;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Continuum93.ServiceModule.UI
{
    public class MemoryMapWindow : Window
    {
        private const int BrickSize = 8;
        private const int BrickSpacing = 2;
        private const int ControlBarHeight = 36;
        private const int ScrollbarThickness = 12;
        private const float HoverDelay = 0.3f;
        private const float PollIntervalSeconds = 1f / 30f;
        private const float DefaultActivityFadeSeconds = 0.5f;
        private const int MinZoomSteps = -6;

        private readonly List<BrickVisual> _visibleBricks = new();
        private readonly byte[] _activityScratch = new byte[MemoryActivityTracker.PageCount];
        private readonly float[] _pageReadIntensity = new float[MemoryActivityTracker.PageCount];
        private readonly float[] _pageWriteIntensity = new float[MemoryActivityTracker.PageCount];

        private MemoryMapHoverPopup _hoverPopup;
        private float _hoverTimer;
        private int _hoveredIndex = -1;
        private int _previousHoveredIndex = -1;

        private int _previousWheel;
        private MouseState _previousMouse;

        private int _zoomSteps;
        private float _scrollX;
        private float _scrollY;
        private float _pollAccumulator;
        private float _activityFadeSeconds = DefaultActivityFadeSeconds;

        private bool _draggingH;
        private bool _draggingV;
        private int _dragOffsetH;
        private int _dragOffsetV;

        private LayoutState _layout;

        private readonly Color _shadowColor = new(0, 60, 0);

        private struct BrickVisual
        {
            public Rectangle Rect;
            public uint StartAddress;
            public int ByteCount;
            public byte Average;
            public float ReadIntensity;
            public float WriteIntensity;
        }

        private struct LayoutState
        {
            public int BrickStride;
            public int BrickCapacity;
            public int MapColumns;
            public int MapRows;
            public int VisibleColumns;
            public int VisibleRows;
            public int ScrollColumn;
            public int ScrollRow;
            public Rectangle AreaRect;
            public Rectangle ControlRect;
            public bool ShowHScroll;
            public bool ShowVScroll;
            public Rectangle HScrollRect;
            public Rectangle HThumbRect;
            public Rectangle VScrollRect;
            public Rectangle VThumbRect;
            public Rectangle ClearButtonRect;
        }

        public MemoryMapWindow(
            string title,
            int x, int y,
            int width, int height,
            float spawnDelaySeconds = 0f,
            bool canResize = true,
            bool canClose = false)
            : base(title, x, y, width, height, spawnDelaySeconds, canResize, canClose)
        {
        }

        public MemoryMapHoverPopup GetHoverPopup() => _hoverPopup;

        // Draw the pop-up after windows
        public void DrawHoverPopup()
        {
            if (_hoverPopup != null && _hoverPopup.Visible)
            {
                _hoverPopup.Draw();
            }
        }

        public float ActivityFadeSeconds
        {
            get => _activityFadeSeconds;
            set => _activityFadeSeconds = Math.Max(0.01f, value);
        }

        protected override void UpdateContent(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var mouse = Mouse.GetState();
            Point mousePos = new(mouse.X, mouse.Y);

            CalculateLayout(ContentRect);
            HandleZoom(mouse, mousePos);
            HandleScrollbars(mouse, mousePos);
            ClampScroll();

            _pollAccumulator += dt;
            if (_pollAccumulator >= PollIntervalSeconds)
            {
                RefreshActivity(_pollAccumulator);
                RefreshVisibleBricks();
                _pollAccumulator = 0f;
            }

            HandleClearButton(mouse, mousePos);
            UpdateHover(mousePos, dt);

            _previousMouse = mouse;
            _previousWheel = mouse.ScrollWheelValue;
        }

        public override bool HandleInput(MouseState mouse, MouseState prevMouse)
        {
            if (!Visible)
                return false;

            // Let base handle drag/resize/clicks first
            if (base.HandleInput(mouse, prevMouse))
                return true;

            // Consume input when over this window so it doesn't bubble to others
            Point pos = new(mouse.X, mouse.Y);
            if (Bounds.Contains(pos))
                return true;

            return false;
        }

        protected override void DrawContent(SpriteBatch spriteBatch, Rectangle contentRect)
        {
            var theme = ServiceGraphics.Theme;
            var pixel = Renderer.GetPixelTexture();

            for (int i = 0; i < _visibleBricks.Count; i++)
            {
                var brick = _visibleBricks[i];
                bool isHovered = i == _hoveredIndex;

                Color baseColor = new(brick.Average, brick.Average, brick.Average);
                Color finalColor = baseColor;

                if (brick.WriteIntensity > 0f)
                {
                    finalColor = Color.Lerp(baseColor, Color.Red, MathHelper.Clamp(brick.WriteIntensity, 0f, 1f));
                }
                else if (brick.ReadIntensity > 0f)
                {
                    finalColor = Color.Lerp(baseColor, Color.LimeGreen, MathHelper.Clamp(brick.ReadIntensity, 0f, 1f));
                }

                spriteBatch.Draw(pixel, brick.Rect, finalColor);

                // Shadow on right and bottom edges
                spriteBatch.Draw(pixel, new Rectangle(brick.Rect.Right - 1, brick.Rect.Y, 1, brick.Rect.Height), _shadowColor);
                spriteBatch.Draw(pixel, new Rectangle(brick.Rect.X, brick.Rect.Bottom - 1, brick.Rect.Width, 1), _shadowColor);

                if (isHovered)
                {
                    var border = new Rectangle(brick.Rect.X - 1, brick.Rect.Y - 1, brick.Rect.Width + 2, brick.Rect.Height + 2);
                    spriteBatch.Draw(pixel, border, new Color(0, 120, 0, 80));
                }
            }

            DrawScrollbars(spriteBatch, pixel, theme);
            DrawControlBar(spriteBatch, pixel, theme);
        }

        private void CalculateLayout(Rectangle contentRect)
        {
            int pad = Padding;
            int availableWidth = Math.Max(1, contentRect.Width - pad * 2);
            int availableHeight = Math.Max(1, contentRect.Height - pad * 2 - ControlBarHeight);

            // Initial area assumes no scrollbars
            bool needH = false;
            bool needV = false;

            LayoutState layout = new()
            {
                BrickStride = BrickSize + BrickSpacing
            };

            ComputeLayoutForArea(ref layout, availableWidth, availableHeight);

            needH = layout.MapColumns > layout.VisibleColumns;
            needV = layout.MapRows > layout.VisibleRows;

            int adjustedWidth = availableWidth - (needV ? ScrollbarThickness : 0);
            int adjustedHeight = availableHeight - (needH ? ScrollbarThickness : 0);

            ComputeLayoutForArea(ref layout, adjustedWidth, adjustedHeight);

            int maxScrollCols = Math.Max(0, layout.MapColumns - layout.VisibleColumns);
            int maxScrollRows = Math.Max(0, layout.MapRows - layout.VisibleRows);

            layout.ScrollColumn = maxScrollCols == 0 ? 0 : (int)Math.Round(MathHelper.Clamp(_scrollX, 0f, 1f) * maxScrollCols);
            layout.ScrollRow = maxScrollRows == 0 ? 0 : (int)Math.Round(MathHelper.Clamp(_scrollY, 0f, 1f) * maxScrollRows);

            layout.ShowHScroll = layout.MapColumns > layout.VisibleColumns;
            layout.ShowVScroll = layout.MapRows > layout.VisibleRows;

            layout.AreaRect = new Rectangle(
                contentRect.X + pad,
                contentRect.Y + pad,
                adjustedWidth,
                adjustedHeight);

            layout.ControlRect = new Rectangle(
                contentRect.X + pad,
                contentRect.Bottom - ControlBarHeight,
                contentRect.Width - pad * 2,
                ControlBarHeight);

            if (layout.ShowHScroll)
            {
                layout.HScrollRect = new Rectangle(
                    layout.AreaRect.X,
                    layout.AreaRect.Bottom,
                    layout.AreaRect.Width,
                    ScrollbarThickness);

                float thumbWidthRatio = layout.VisibleColumns / (float)layout.MapColumns;
                int thumbWidth = Math.Max(8, (int)(layout.HScrollRect.Width * thumbWidthRatio));
                int maxThumbTravel = Math.Max(1, layout.HScrollRect.Width - thumbWidth);
                int thumbX = layout.HScrollRect.X + (int)(maxThumbTravel * (_scrollX = MathHelper.Clamp(_scrollX, 0f, 1f)));

                layout.HThumbRect = new Rectangle(
                    thumbX,
                    layout.HScrollRect.Y,
                    thumbWidth,
                    ScrollbarThickness);
            }

            if (layout.ShowVScroll)
            {
                layout.VScrollRect = new Rectangle(
                    layout.AreaRect.Right,
                    layout.AreaRect.Y,
                    ScrollbarThickness,
                    layout.AreaRect.Height);

                float thumbHeightRatio = layout.VisibleRows / (float)layout.MapRows;
                int thumbHeight = Math.Max(8, (int)(layout.VScrollRect.Height * thumbHeightRatio));
                int maxThumbTravel = Math.Max(1, layout.VScrollRect.Height - thumbHeight);
                int thumbY = layout.VScrollRect.Y + (int)(maxThumbTravel * (_scrollY = MathHelper.Clamp(_scrollY, 0f, 1f)));

                layout.VThumbRect = new Rectangle(
                    layout.VScrollRect.X,
                    thumbY,
                    ScrollbarThickness,
                    thumbHeight);
            }

            // Clear button placed on the right side of the control bar
            int buttonWidth = 160;
            layout.ClearButtonRect = new Rectangle(
                layout.ControlRect.Right - buttonWidth - pad,
                layout.ControlRect.Y + 6,
                buttonWidth,
                layout.ControlRect.Height - 12);

            _layout = layout;
        }

        private void ComputeLayoutForArea(ref LayoutState layout, int areaWidth, int areaHeight)
        {
            int bricksX = Math.Max(1, areaWidth / layout.BrickStride);
            int bricksY = Math.Max(1, areaHeight / layout.BrickStride);

            layout.BrickCapacity = CalculateBrickCapacity(bricksX, bricksY);
            int totalBricks = (int)Math.Ceiling(0x1000000d / (double)layout.BrickCapacity);

            layout.MapColumns = Math.Max(bricksX, (int)Math.Ceiling(Math.Sqrt(totalBricks)));
            layout.MapRows = (totalBricks + layout.MapColumns - 1) / layout.MapColumns;

            layout.VisibleColumns = Math.Min(bricksX, layout.MapColumns);
            layout.VisibleRows = Math.Min(bricksY, layout.MapRows);
        }

        private int CalculateBrickCapacity(int bricksX, int bricksY)
        {
            long bricks = Math.Max(1, bricksX * bricksY);
            double baseBytes = Math.Ceiling(0x1000000d / (double)bricks);
            int capacity = NextPowerOfTwo((int)Math.Max(1, baseBytes));

            if (_zoomSteps > 0)
        {
            capacity = Math.Max(1, capacity >> _zoomSteps);
        }
        else if (_zoomSteps < 0)
        {
            capacity = Math.Min(0x1000000, capacity << -_zoomSteps);
        }

            capacity = Math.Max(1, capacity);
            return capacity;
        }

        private static int NextPowerOfTwo(int value)
        {
            value--;
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            value++;
            return value;
        }

        private void HandleZoom(MouseState mouse, Point mousePos)
        {
            if (!ContentRect.Contains(mousePos))
                return;

            int delta = mouse.ScrollWheelValue - _previousWheel;
            if (delta == 0)
                return;

            int next = _zoomSteps + (delta > 0 ? 1 : -1);
            _zoomSteps = Math.Max(MinZoomSteps, next);
        }

        private void HandleScrollbars(MouseState mouse, Point mousePos)
        {
            bool leftPressed = mouse.LeftButton == ButtonState.Pressed;
            bool leftJustPressed = leftPressed && _previousMouse.LeftButton == ButtonState.Released;
            bool leftJustReleased = !leftPressed && _previousMouse.LeftButton == ButtonState.Pressed;

            if (leftJustPressed && _layout.ShowHScroll && _layout.HThumbRect.Contains(mousePos))
            {
                _draggingH = true;
                _dragOffsetH = mousePos.X - _layout.HThumbRect.X;
            }

            if (leftJustPressed && _layout.ShowVScroll && _layout.VThumbRect.Contains(mousePos))
            {
                _draggingV = true;
                _dragOffsetV = mousePos.Y - _layout.VThumbRect.Y;
            }

            if (_draggingH)
            {
                if (leftPressed)
                {
                    int travel = _layout.HScrollRect.Width - _layout.HThumbRect.Width;
                    float ratio = (mousePos.X - _layout.HScrollRect.X - _dragOffsetH) / (float)Math.Max(1, travel);
                    _scrollX = MathHelper.Clamp(ratio, 0f, 1f);
                }
                else if (leftJustReleased)
                {
                    _draggingH = false;
                }
            }

            if (_draggingV)
            {
                if (leftPressed)
                {
                    int travel = _layout.VScrollRect.Height - _layout.VThumbRect.Height;
                    float ratio = (mousePos.Y - _layout.VScrollRect.Y - _dragOffsetV) / (float)Math.Max(1, travel);
                    _scrollY = MathHelper.Clamp(ratio, 0f, 1f);
                }
                else if (leftJustReleased)
                {
                    _draggingV = false;
                }
            }
        }

        private void ClampScroll()
        {
            int maxScrollCols = Math.Max(0, _layout.MapColumns - _layout.VisibleColumns);
            int maxScrollRows = Math.Max(0, _layout.MapRows - _layout.VisibleRows);

            _scrollX = maxScrollCols == 0 ? 0f : MathHelper.Clamp(_scrollX, 0f, 1f);
            _scrollY = maxScrollRows == 0 ? 0f : MathHelper.Clamp(_scrollY, 0f, 1f);
        }

        private void RefreshActivity(float elapsedSeconds)
        {
            var tracker = Machine.COMPUTER?.MEMC.ActivityTracker;
            if (tracker != null)
            {
                tracker.ConsumeActivity(_activityScratch);

                for (int i = 0; i < _activityScratch.Length; i++)
                {
                    byte flags = _activityScratch[i];
                    if ((flags & MemoryActivityTracker.ReadFlag) != 0)
                        _pageReadIntensity[i] = 1f;
                    if ((flags & MemoryActivityTracker.WriteFlag) != 0)
                        _pageWriteIntensity[i] = 1f;
                }
            }

            if (!DebugState.StepByStep)
            {
                float decay = elapsedSeconds / _activityFadeSeconds;
                for (int i = 0; i < _pageReadIntensity.Length; i++)
                {
                    _pageReadIntensity[i] = Math.Max(0f, _pageReadIntensity[i] - decay);
                    _pageWriteIntensity[i] = Math.Max(0f, _pageWriteIntensity[i] - decay);
                }
            }
        }

        private void RefreshVisibleBricks()
        {
            _visibleBricks.Clear();

            var computer = Machine.COMPUTER;
            if (computer == null)
                return;

            var ram = computer.MEMC.RAM.Data;
            uint totalBytes = computer.MEMC.RAM.Size;

            for (int row = 0; row < _layout.VisibleRows; row++)
            {
                for (int col = 0; col < _layout.VisibleColumns; col++)
                {
                    int globalColumn = _layout.ScrollColumn + col;
                    int globalRow = _layout.ScrollRow + row;
                    int globalIndex = globalRow * _layout.MapColumns + globalColumn;

                    uint startAddress = (uint)(globalIndex * _layout.BrickCapacity);
                    if (startAddress >= totalBytes)
                        continue;

                    int count = (int)Math.Min(_layout.BrickCapacity, totalBytes - startAddress);
                    byte average = ComputeAverage(ram, startAddress, count);
                    float readIntensity = GetMaxIntensity(_pageReadIntensity, startAddress, count);
                    float writeIntensity = GetMaxIntensity(_pageWriteIntensity, startAddress, count);

                    Rectangle rect = new(
                        _layout.AreaRect.X + col * _layout.BrickStride,
                        _layout.AreaRect.Y + row * _layout.BrickStride,
                        BrickSize,
                        BrickSize);

                    _visibleBricks.Add(new BrickVisual
                    {
                        Rect = rect,
                        StartAddress = startAddress,
                        ByteCount = count,
                        Average = average,
                        ReadIntensity = readIntensity,
                        WriteIntensity = writeIntensity
                    });
                }
            }
        }

        private static byte ComputeAverage(byte[] data, uint start, int count)
        {
            if (count <= 0 || start >= data.Length)
                return 0;

            int length = Math.Min(count, data.Length - (int)start);
            long sum = 0;
            int end = (int)start + length;
            for (int i = (int)start; i < end; i++)
            {
                sum += data[i];
            }

            return (byte)(sum / length);
        }

        private static float GetMaxIntensity(float[] intensities, uint start, int count)
        {
            if (count <= 0)
                return 0f;

            int startPage = (int)(start >> MemoryActivityTracker.PageShift);
            int endPage = (int)((start + (uint)Math.Max(0, count - 1)) >> MemoryActivityTracker.PageShift);

            float max = 0f;
            for (int page = startPage; page <= endPage && page < intensities.Length; page++)
            {
                if (page < 0)
                    continue;
                max = Math.Max(max, intensities[page]);
            }
            return max;
        }

        private void UpdateHover(Point mousePos, float dt)
        {
            bool mouseOverPopup = _hoverPopup != null && _hoverPopup.Visible && _hoverPopup.Bounds.Contains(mousePos);

            if (!_layout.AreaRect.Contains(mousePos) && !mouseOverPopup)
            {
                HideHoverPopup();
                _hoveredIndex = -1;
                _hoverTimer = 0f;
                return;
            }

            int hovered = -1;
            for (int i = 0; i < _visibleBricks.Count; i++)
            {
                if (_visibleBricks[i].Rect.Contains(mousePos))
                {
                    hovered = i;
                    break;
                }
            }

            if (hovered != _previousHoveredIndex)
            {
                _hoverTimer = 0f;
                _previousHoveredIndex = hovered;
            }

            _hoveredIndex = hovered;

            if (_hoveredIndex >= 0 && !mouseOverPopup)
            {
                _hoverTimer += dt;
                if (_hoverTimer >= HoverDelay)
                {
                    ShowHoverPopup(_visibleBricks[_hoveredIndex]);
                }
            }
            else if (!mouseOverPopup)
            {
                HideHoverPopup();
                _hoverTimer = 0f;
            }

            if (_hoverPopup != null && _hoverPopup.Visible)
            {
                _hoverPopup.Update(new GameTime());
            }
        }

        private void ShowHoverPopup(BrickVisual brick)
        {
            var computer = Machine.COMPUTER;
            if (computer == null)
                return;

            byte[] data = computer.MEMC.DumpMemAt(brick.StartAddress, brick.ByteCount);

            const int offset = 16;
            int popupX = brick.Rect.Right + offset;
            int popupY = brick.Rect.Y;

            var device = Renderer.GetGraphicsDevice();
            int popupWidth = 280;
            int popupHeight = 260;

            if (popupX + popupWidth > device.Viewport.Width)
                popupX = brick.Rect.Left - popupWidth - offset;
            if (popupY + popupHeight > device.Viewport.Height)
                popupY = device.Viewport.Height - popupHeight - offset;

            if (_hoverPopup == null)
            {
                _hoverPopup = new MemoryMapHoverPopup(popupX, popupY, brick.StartAddress, brick.ByteCount, data);
            }
            else
            {
                _hoverPopup.X = popupX;
                _hoverPopup.Y = popupY;
                _hoverPopup.UpdateData(brick.StartAddress, brick.ByteCount, data);
                _hoverPopup.Visible = true;
            }
        }

        private void HideHoverPopup()
        {
            if (_hoverPopup != null)
            {
                _hoverPopup.Visible = false;
            }
        }

        private void HandleClearButton(MouseState mouse, Point mousePos)
        {
            bool leftJustPressed = mouse.LeftButton == ButtonState.Pressed &&
                                   _previousMouse.LeftButton == ButtonState.Released;

            if (leftJustPressed && _layout.ClearButtonRect.Contains(mousePos))
            {
                ClearActivity();
            }
        }

        private void ClearActivity()
        {
            Array.Clear(_pageReadIntensity, 0, _pageReadIntensity.Length);
            Array.Clear(_pageWriteIntensity, 0, _pageWriteIntensity.Length);
            Machine.COMPUTER?.MEMC.ActivityTracker.Clear();
        }

        private void DrawScrollbars(SpriteBatch spriteBatch, Texture2D pixel, Themes.Theme theme)
        {
            if (_layout.ShowHScroll)
            {
                spriteBatch.Draw(pixel, _layout.HScrollRect, theme.TaskbarItemNormalBackground);
                spriteBatch.Draw(pixel, _layout.HThumbRect, theme.TaskbarItemActiveBackground);
            }

            if (_layout.ShowVScroll)
            {
                spriteBatch.Draw(pixel, _layout.VScrollRect, theme.TaskbarItemNormalBackground);
                spriteBatch.Draw(pixel, _layout.VThumbRect, theme.TaskbarItemActiveBackground);
            }
        }

        private void DrawControlBar(SpriteBatch spriteBatch, Texture2D pixel, Themes.Theme theme)
        {
            spriteBatch.Draw(pixel, _layout.ControlRect, theme.WindowBackgroundFocused);

            byte fontFlags = (byte)(ServiceFontFlags.Monospace | ServiceFontFlags.DrawOutline);
            int textY = _layout.ControlRect.Y + 8;
            int textX = _layout.ControlRect.X + Padding;

            var computer = Machine.COMPUTER;
            uint totalBytes = computer?.MEMC.RAM.Size ?? 0;
            uint viewStart = (uint)((_layout.ScrollRow * _layout.MapColumns + _layout.ScrollColumn) * _layout.BrickCapacity);
            uint viewEnd = viewStart + (uint)(_layout.VisibleColumns * _layout.VisibleRows * _layout.BrickCapacity);
            if (viewEnd > totalBytes)
                viewEnd = totalBytes == 0 ? 0 : totalBytes - 1;

            string human = FormatSize(_layout.BrickCapacity);

            ServiceGraphics.DrawText(
                theme.PrimaryFont,
                $"Bytes/brick: {_layout.BrickCapacity} (0x{_layout.BrickCapacity:X}) [{human}]  Zoom: {_zoomSteps}",
                textX,
                textY,
                _layout.ControlRect.Width - Padding * 2,
                theme.TextPrimary,
                theme.TextOutline,
                fontFlags,
                0xFF);

            ServiceGraphics.DrawText(
                theme.PrimaryFont,
                $"View: 0x{viewStart:X6} - 0x{viewEnd:X6}",
                textX,
                textY + 16,
                _layout.ControlRect.Width - Padding * 2,
                theme.TextInfo,
                theme.TextOutline,
                fontFlags,
                0xFF);

            Color buttonColor = _layout.ClearButtonRect.Contains(new Point(Mouse.GetState().X, Mouse.GetState().Y))
                ? theme.TaskbarItemActiveBackground
                : theme.TaskbarItemNormalBackground;

            spriteBatch.Draw(pixel, _layout.ClearButtonRect, buttonColor);

            ServiceGraphics.DrawText(
                theme.PrimaryFont,
                "Clear Activity Map",
                _layout.ClearButtonRect.X + 10,
                _layout.ClearButtonRect.Y + 6,
                _layout.ClearButtonRect.Width - 20,
                theme.TextPrimary,
                theme.TextOutline,
                fontFlags,
                0xFF);
        }

        private static string FormatSize(int bytes)
        {
            const double kb = 1024d;
            const double mb = 1024d * 1024d;
            if (bytes >= mb)
                return $"{bytes / mb:0.##} MB";
            if (bytes >= kb)
                return $"{bytes / kb:0.##} KB";
            return $"{bytes} B";
        }
    }
}

