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
        private const float CloseDelay = 0.2f; // Delay before closing popup when mouse leaves
        private const float PollIntervalSeconds = 1f / 30f;
        private const float DefaultActivityFadeSeconds = 0.5f;
        private const int MinZoomSteps = -6;

        private readonly List<BrickVisual> _visibleBricks = new();
        private readonly byte[] _activityScratch = new byte[MemoryActivityTracker.PageCount];
        private readonly ulong[] _byteReadMask = new ulong[MemoryActivityTracker.ByteMaskLength];
        private readonly ulong[] _byteWriteMask = new ulong[MemoryActivityTracker.ByteMaskLength];
        private readonly ulong[] _byteReadScratch = new ulong[MemoryActivityTracker.ByteMaskLength];
        private readonly ulong[] _byteWriteScratch = new ulong[MemoryActivityTracker.ByteMaskLength];
        private readonly float[] _pageReadIntensity = new float[MemoryActivityTracker.PageCount];
        private readonly float[] _pageWriteIntensity = new float[MemoryActivityTracker.PageCount];

        private MemoryMapHoverPopup _hoverPopup;
        private float _hoverTimer;
        private float _closeTimer;
        private int _hoveredIndex = -1;
        private int _previousHoveredIndex = -1;

        private int _previousWheel;
        private MouseState _previousMouse;

        private int _zoomSteps;
        private float _scrollY;
        private float _pollAccumulator;
        private float _activityFadeSeconds = DefaultActivityFadeSeconds;
        private uint _lastStepTag;

        private bool _draggingV;
        private int _dragOffsetV;

        private LayoutState _layout;

        private readonly Color _shadowColor = new(0, 60, 0);
        // Palette colors: darker yellow (even layers), lighter yellow (odd layers)
        private readonly Color _paletteColorDark = new Color(200, 200, 0); // Darker yellow
        private readonly Color _paletteColorLight = new Color(255, 255, 100); // Lighter yellow
        // Video colors: blue (even layers), cyan (odd layers)
        private readonly Color _videoColorBlue = Color.Blue;
        private readonly Color _videoColorCyan = Color.Cyan;

        // Video region cache
        private byte _cachedVramPages = 0;
        private readonly List<VideoRegion> _videoRegions = new();

        private struct VideoRegion
        {
            public uint StartAddress;
            public uint EndAddress; // Inclusive
            public byte LayerIndex;
            public bool IsPalette; // true for palette, false for video data
        }

        private struct BrickVisual
        {
            public Rectangle Rect;
            public uint StartAddress;
            public int ByteCount;
            public byte Average;
            public float ReadIntensity;
            public float WriteIntensity;
            public bool HasPaletteData;
            public bool HasVideoData;
            public List<byte> PaletteLayers; // Layer indices that have palette data in this brick
            public List<byte> VideoLayers; // Layer indices that have video data in this brick
        }

        private struct LayoutState
        {
            public int BrickStride;
            public int BrickCapacity;
            public int MapColumns;
            public int MapRows;
            public int VisibleColumns;
            public int VisibleRows;
            public int ScrollRow;
            public Rectangle AreaRect;
            public Rectangle ControlRect;
            public bool ShowVScroll;
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

            // Update video regions cache if VRAM_PAGES changed
            UpdateVideoRegionsCache();

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

            Point pos = new(mouse.X, mouse.Y);
            
            // Only handle input if we're the topmost window at this position
            // This prevents covered windows from being brought to front on hover
            if (!IsTopmostAtPosition(pos))
                return false;

            // Handle scroll wheel input (zoom)
            int scrollDelta = mouse.ScrollWheelValue - prevMouse.ScrollWheelValue;
            if (scrollDelta != 0 && ContentRect.Contains(pos))
            {
                // Scroll handling will be done in UpdateContent, but we return true
                // to indicate we're consuming this input
                return true;
            }

            // Handle clicks on clear button or scrollbar (only if layout is initialized)
            bool leftJustPressed = mouse.LeftButton == ButtonState.Pressed && 
                                   prevMouse.LeftButton == ButtonState.Released;
            if (leftJustPressed)
            {
                // Check if layout is initialized (AreaRect will be non-zero if initialized)
                if (_layout.AreaRect.Width > 0)
                {
                    if (_layout.ClearButtonRect.Contains(pos) || 
                        (_layout.ShowVScroll && _layout.VScrollRect.Contains(pos)))
                    {
                        return true;
                    }
                }
            }

            // Don't return true just for hovering - that would bring window to front
            return false;
        }

        private bool IsTopmostAtPosition(Point pos)
        {
            if (Manager == null || !Bounds.Contains(pos))
                return false;

            // Check if any window after us in the list (which are visually above us) contains the mouse
            int ourIndex = Manager.Windows.IndexOf(this);
            if (ourIndex < 0)
                return false;

            // Windows are ordered from bottom to top, so windows after us are above us
            for (int i = ourIndex + 1; i < Manager.Windows.Count; i++)
            {
                var other = Manager.Windows[i];
                if (other.Visible && other.Bounds.Contains(pos))
                    return false; // Another window is above us at this position
            }

            return true; // We're the topmost window at this position
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

                // Determine border color based on video regions with alternating colors
                Color borderColor = _shadowColor; // Default green shadow
                if (brick.HasVideoData && brick.HasPaletteData)
                {
                    // If both, prioritize video - use lowest layer index to determine color
                    byte lowestLayer = GetLowestLayer(brick.VideoLayers, brick.PaletteLayers);
                    borderColor = (lowestLayer % 2 == 0) ? _videoColorBlue : _videoColorCyan;
                }
                else if (brick.HasVideoData)
                {
                    // Use lowest video layer index to determine color
                    byte lowestLayer = GetLowestLayer(brick.VideoLayers, new List<byte>());
                    borderColor = (lowestLayer % 2 == 0) ? _videoColorBlue : _videoColorCyan;
                }
                else if (brick.HasPaletteData)
                {
                    // Use lowest palette layer index to determine color
                    byte lowestLayer = GetLowestLayer(new List<byte>(), brick.PaletteLayers);
                    borderColor = (lowestLayer % 2 == 0) ? _paletteColorDark : _paletteColorLight;
                }

                // Draw border on right and bottom edges
                spriteBatch.Draw(pixel, new Rectangle(brick.Rect.Right - 1, brick.Rect.Y, 1, brick.Rect.Height), borderColor);
                spriteBatch.Draw(pixel, new Rectangle(brick.Rect.X, brick.Rect.Bottom - 1, brick.Rect.Width, 1), borderColor);

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
            uint totalBytes = Machine.COMPUTER?.MEMC.RAM.Size ?? 0;

            LayoutState layout = new()
            {
                BrickStride = BrickSize + BrickSpacing
            };

            // Only vertical scrollbar supported: adjust width when needed
            ComputeLayoutForArea(ref layout, availableWidth, availableHeight);
            bool needV = layout.MapRows > layout.VisibleRows;

            int adjustedWidth = availableWidth - (needV ? ScrollbarThickness : 0);
            int adjustedHeight = availableHeight;

            ComputeLayoutForArea(ref layout, adjustedWidth, adjustedHeight);

            int maxScrollCols = Math.Max(0, layout.MapColumns - layout.VisibleColumns);
            int totalBricksData = layout.BrickCapacity > 0
                ? (int)Math.Ceiling(totalBytes / (double)layout.BrickCapacity)
                : 0;
            int totalRowsData = layout.MapColumns > 0
                ? (int)Math.Ceiling(totalBricksData / (double)layout.MapColumns)
                : 0;

            int maxScrollRows = Math.Max(0, totalRowsData - layout.VisibleRows);

            layout.ScrollRow = maxScrollRows == 0 ? 0 : (int)Math.Round(MathHelper.Clamp(_scrollY, 0f, 1f) * maxScrollRows);

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
            int buttonWidth = 195;
            layout.ClearButtonRect = new Rectangle(
                layout.ControlRect.Right - buttonWidth - pad * 4,
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

            layout.MapColumns = bricksX; // wrap horizontally; no horizontal scroll
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

            bool ctrl = Keyboard.GetState().IsKeyDown(Keys.LeftControl) || Keyboard.GetState().IsKeyDown(Keys.RightControl);

            // If hovering over popup, don't forward wheel to the map.
            if (_hoverPopup != null && _hoverPopup.Visible && _hoverPopup.Bounds.Contains(mousePos))
                return;

            // Ctrl + wheel: scroll vertically (no zoom)
            if (ctrl)
            {
                int direction = delta > 0 ? -1 : 1;
                float step = _layout.MapRows > 0
                    ? (_layout.VisibleRows / (float)Math.Max(1, _layout.MapRows))
                    : 0.1f;
                _scrollY = MathHelper.Clamp(_scrollY + direction * step, 0f, 1f);
                return;
            }

            // Stop zooming in once brick capacity is 1 byte
            if (delta > 0 && _layout.BrickCapacity <= 1)
                return;

            int next = _zoomSteps + (delta > 0 ? 1 : -1);
            _zoomSteps = Math.Max(MinZoomSteps, next);
        }

        private void HandleScrollbars(MouseState mouse, Point mousePos)
        {
            bool leftPressed = mouse.LeftButton == ButtonState.Pressed;
            bool leftJustPressed = leftPressed && _previousMouse.LeftButton == ButtonState.Released;
            bool leftJustReleased = !leftPressed && _previousMouse.LeftButton == ButtonState.Pressed;

            if (leftJustPressed && _layout.ShowVScroll && _layout.VThumbRect.Contains(mousePos))
            {
                _draggingV = true;
                _dragOffsetV = mousePos.Y - _layout.VThumbRect.Y;
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
            int maxScrollRows = Math.Max(0, _layout.MapRows - _layout.VisibleRows);

            _scrollY = maxScrollRows == 0 ? 0f : MathHelper.Clamp(_scrollY, 0f, 1f);
        }

        private void RefreshActivity(float elapsedSeconds)
        {
            var tracker = Machine.COMPUTER?.MEMC.ActivityTracker;
            if (tracker != null)
            {
                // Pull the latest activity into scratch then accumulate so highlights persist
                tracker.ConsumeActivity(_activityScratch, _byteReadScratch, _byteWriteScratch);

                for (int i = 0; i < _byteReadMask.Length; i++)
                {
                    _byteReadMask[i] |= _byteReadScratch[i];
                    _byteWriteMask[i] |= _byteWriteScratch[i];
                }

                if (tracker.StepTag != _lastStepTag)
                {
                    Array.Clear(_pageReadIntensity, 0, _pageReadIntensity.Length);
                    Array.Clear(_pageWriteIntensity, 0, _pageWriteIntensity.Length);
                    Array.Clear(_byteReadMask, 0, _byteReadMask.Length);
                    Array.Clear(_byteWriteMask, 0, _byteWriteMask.Length);
                    _lastStepTag = tracker.StepTag;
                }

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

        private void UpdateVideoRegionsCache()
        {
            var computer = Machine.COMPUTER;
            if (computer?.GRAPHICS == null)
            {
                _videoRegions.Clear();
                _cachedVramPages = 0;
                return;
            }

            byte vramPages = computer.GRAPHICS.VRAM_PAGES;
            if (vramPages == _cachedVramPages && _videoRegions.Count > 0)
                return; // Cache is up to date

            _cachedVramPages = vramPages;
            _videoRegions.Clear();

            if (vramPages == 0)
                return;

            const uint PALETTE_SIZE = 768;
            uint vSize = Constants.V_SIZE; // 480 * 270 = 129,600
            uint ramSize = 0x1000000; // 16 MB

            // Calculate VRAM_OFFSET (same as Graphics.cs)
            uint vramOffset = ramSize - vSize * vramPages;

            // For each layer (numbered backwards from 0 to vramPages-1)
            for (byte layerIndex = 0; layerIndex < vramPages; layerIndex++)
            {
                // Calculate addresses (same as Graphics.cs methods)
                uint videoAddress = (uint)(ramSize - vSize * (layerIndex + 1));
                uint paletteAddress = (uint)(vramOffset - PALETTE_SIZE * (layerIndex + 1));

                // Add palette region
                _videoRegions.Add(new VideoRegion
                {
                    StartAddress = paletteAddress,
                    EndAddress = paletteAddress + PALETTE_SIZE - 1,
                    LayerIndex = layerIndex,
                    IsPalette = true
                });

                // Add video data region
                _videoRegions.Add(new VideoRegion
                {
                    StartAddress = videoAddress,
                    EndAddress = videoAddress + vSize - 1,
                    LayerIndex = layerIndex,
                    IsPalette = false
                });
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
            if (totalBytes == 0 || _layout.BrickCapacity <= 0)
                return;

            for (int row = 0; row < _layout.VisibleRows; row++)
            {
                for (int col = 0; col < _layout.VisibleColumns; col++)
                {
                    int globalColumn = col;
                    int globalRow = _layout.ScrollRow + row;
                    int globalIndex = globalRow * _layout.MapColumns + globalColumn;

                    uint startAddress = (uint)(globalIndex * _layout.BrickCapacity);
                    if (startAddress >= totalBytes)
                        continue;

                    int count = (int)Math.Min(_layout.BrickCapacity, totalBytes - startAddress);
                    uint endAddress = startAddress + (uint)count - 1;
                    byte average = ComputeAverage(ram, startAddress, count);
                    float readIntensity = GetIntensityForRange(_pageReadIntensity, _byteReadMask, startAddress, count);
                    float writeIntensity = GetIntensityForRange(_pageWriteIntensity, _byteWriteMask, startAddress, count);

                    // Check for video regions
                    bool hasPaletteData = false;
                    bool hasVideoData = false;
                    var paletteLayers = new List<byte>();
                    var videoLayers = new List<byte>();

                    foreach (var region in _videoRegions)
                    {
                        // Check if brick overlaps with region
                        if (startAddress <= region.EndAddress && endAddress >= region.StartAddress)
                        {
                            if (region.IsPalette)
                            {
                                hasPaletteData = true;
                                if (!paletteLayers.Contains(region.LayerIndex))
                                    paletteLayers.Add(region.LayerIndex);
                            }
                            else
                            {
                                hasVideoData = true;
                                if (!videoLayers.Contains(region.LayerIndex))
                                    videoLayers.Add(region.LayerIndex);
                            }
                        }
                    }

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
                        WriteIntensity = writeIntensity,
                        HasPaletteData = hasPaletteData,
                        HasVideoData = hasVideoData,
                        PaletteLayers = paletteLayers,
                        VideoLayers = videoLayers
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

        private static float GetIntensityForRange(float[] pageIntensities, ulong[] byteMask, uint start, int count)
        {
            if (count <= 0)
                return 0f;

            int startPage = (int)(start >> MemoryActivityTracker.PageShift);
            int endPage = (int)((start + (uint)Math.Max(0, count - 1)) >> MemoryActivityTracker.PageShift);

            float max = 0f;
            for (int page = startPage; page <= endPage && page < pageIntensities.Length; page++)
            {
                if (page < 0)
                    continue;

                if (pageIntensities[page] <= 0f)
                    continue;

                uint pageStart = (uint)(page << MemoryActivityTracker.PageShift);
                uint pageEnd = pageStart + MemoryActivityTracker.PageSize - 1;

                uint rangeStart = Math.Max(pageStart, start);
                uint rangeEnd = Math.Min(pageEnd, start + (uint)Math.Max(0, count - 1));
                int localOffset = (int)(rangeStart - pageStart);
                int localCount = (int)(rangeEnd - rangeStart + 1);

                if (HasByteActivity(byteMask, page, localOffset, localCount))
                {
                    max = Math.Max(max, pageIntensities[page]);
                }
            }

            return max;
        }

        private static bool HasByteActivity(ulong[] mask, int pageIndex, int offset, int length)
        {
            if (length <= 0 || pageIndex < 0)
                return false;

            int end = offset + length;
            int wordBase = pageIndex * MemoryActivityTracker.PageBitMaskWordCount;
            int cursor = offset;

            while (cursor < end)
            {
                int wordIndex = cursor >> 6;
                int bitIndex = cursor & 63;
                int span = Math.Min(64 - bitIndex, end - cursor);

                ulong bits = span == 64
                    ? ulong.MaxValue
                    : (((1UL << span) - 1UL) << bitIndex);

                if ((mask[wordBase + wordIndex] & bits) != 0)
                    return true;

                cursor += span;
            }

            return false;
        }

        private void UpdateHover(Point mousePos, float dt)
        {
            bool mouseOverPopup = _hoverPopup != null && _hoverPopup.Visible && _hoverPopup.Bounds.Contains(mousePos);
            bool mouseOverArea = _layout.AreaRect.Contains(mousePos);

            // If mouse is over area or popup, reset close timer and handle normal hover logic
            if (mouseOverArea || mouseOverPopup)
            {
                _closeTimer = 0f; // Reset close timer when mouse re-enters

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
                else if (!mouseOverPopup && _hoveredIndex < 0)
                {
                    // Mouse is over area but not a brick - keep popup if it's visible
                    // Don't hide it here, let the close timer handle it if mouse leaves
                }

                if (_hoverPopup != null && _hoverPopup.Visible)
                {
                    _hoverPopup.Update(new GameTime());
                }
            }
            else
            {
                // Mouse is not over area or popup - start close timer
                if (_hoverPopup != null && _hoverPopup.Visible)
                {
                    _closeTimer += dt;
                    if (_closeTimer >= CloseDelay)
                    {
                        HideHoverPopup();
                        _hoveredIndex = -1;
                        _hoverTimer = 0f;
                        _closeTimer = 0f;
                    }
                    else
                    {
                        // Keep popup visible during delay, but don't update it
                        _hoverPopup.Update(new GameTime());
                    }
                }
                else
                {
                    _hoveredIndex = -1;
                    _hoverTimer = 0f;
                    _closeTimer = 0f;
                }
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
            int popupWidth = 360; // Extended width to fit longer titles with layer info
            int popupHeight = 260;

            if (popupX + popupWidth > device.Viewport.Width)
                popupX = brick.Rect.Left - popupWidth - offset;
            if (popupY + popupHeight > device.Viewport.Height)
                popupY = device.Viewport.Height - popupHeight - offset;

            // Build title with layer information
            string layerInfo = BuildLayerInfo(brick);
            string titleSuffix = string.IsNullOrEmpty(layerInfo) ? string.Empty : $" {layerInfo}";

            if (_hoverPopup == null)
            {
                _hoverPopup = new MemoryMapHoverPopup(popupX, popupY, brick.StartAddress, brick.ByteCount, data, titleSuffix);
            }
            else
            {
                _hoverPopup.X = popupX;
                _hoverPopup.Y = popupY;
                _hoverPopup.UpdateData(brick.StartAddress, brick.ByteCount, data, titleSuffix);
                _hoverPopup.Visible = true;
            }
        }

        private byte GetLowestLayer(List<byte> videoLayers, List<byte> paletteLayers)
        {
            byte lowest = byte.MaxValue;
            foreach (var layer in videoLayers)
            {
                if (layer < lowest)
                    lowest = layer;
            }
            foreach (var layer in paletteLayers)
            {
                if (layer < lowest)
                    lowest = layer;
            }
            return lowest == byte.MaxValue ? (byte)0 : lowest;
        }

        private string BuildLayerInfo(BrickVisual brick)
        {
            if (!brick.HasPaletteData && !brick.HasVideoData)
                return string.Empty;

            // If multiple layers intersect (either multiple palettes, multiple videos, or both), show generic message
            bool hasMultipleLayers = (brick.PaletteLayers.Count + brick.VideoLayers.Count) > 1;

            if (hasMultipleLayers)
            {
                return "(video data)";
            }

            // Single layer case - check if we have both palette and video from the same layer
            if (brick.HasPaletteData && brick.HasVideoData && 
                brick.PaletteLayers.Count == 1 && brick.VideoLayers.Count == 1 &&
                brick.PaletteLayers[0] == brick.VideoLayers[0])
            {
                // Same layer has both palette and video - show as video data
                return "(video data)";
            }

            // Single region case
            if (brick.HasPaletteData && brick.PaletteLayers.Count > 0)
            {
                return $"(palette [{brick.PaletteLayers[0]}])";
            }
            else if (brick.HasVideoData && brick.VideoLayers.Count > 0)
            {
                return $"(video [{brick.VideoLayers[0]}])";
            }

            return string.Empty;
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
            Array.Clear(_byteReadMask, 0, _byteReadMask.Length);
            Array.Clear(_byteWriteMask, 0, _byteWriteMask.Length);
            Array.Clear(_byteReadScratch, 0, _byteReadScratch.Length);
            Array.Clear(_byteWriteScratch, 0, _byteWriteScratch.Length);
            Machine.COMPUTER?.MEMC.ActivityTracker.Clear();
        }

        private void DrawScrollbars(SpriteBatch spriteBatch, Texture2D pixel, Themes.Theme theme)
        {
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
            uint viewStart = (uint)((_layout.ScrollRow * _layout.MapColumns) * _layout.BrickCapacity);
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

            Color buttonColor = _layout.ClearButtonRect.Contains(new Point(Mouse.GetState().X, Mouse.GetState().Y))
                ? theme.TaskbarItemActiveBackground
                : theme.TaskbarItemNormalBackground;

            spriteBatch.Draw(pixel, _layout.ClearButtonRect, buttonColor);

            ServiceGraphics.DrawText(
                Fonts.ModernDOS_9x15,
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

