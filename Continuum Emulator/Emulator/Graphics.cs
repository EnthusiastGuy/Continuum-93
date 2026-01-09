using Continuum93.Emulator;
using Continuum93.Emulator.Colors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Continuum93.Emulator
{
    public class Graphics: IDisposable
    {
        private bool _disposed;

        const int PALETTE_SIZE = 768;
        public byte VRAM_PAGES_OLD;
        public byte VRAM_PAGES;
        public uint VRAM_OFFSET;
        public uint[] PALETTE_ADDRESSES = new uint[8];
        public volatile bool[] LAYER_VISIBLE
            = [true, true, true, true, true, true, true, true];
        public volatile byte LAYER_VISIBLE_BITS = 0xFF;

        public bool[] LAYER_BUFFER_AUTO_MODE = [true, true, true, true, true, true, true, true];
        public volatile byte LAYER_BUFFER_MODE_BITS = 0xFF;

        private Texture2D _videoProjection;
        private volatile Color[] _colorData;

        private uint _palettesStartAddress;
        private uint _videoStartAddress;
        private volatile byte[] _videoBuffer;
        private volatile byte[] _palettesBuffer;

        private byte _visibleLayersCount = 0;
        private byte[] _visibleLayers = [0, 0, 0, 0, 0, 0, 0, 0];
        private Computer _computer;

        // Expose the video projection to be used with the Service mode
        public Texture2D VideoProjection => _videoProjection;


        public Graphics(Computer computer)
        {
            _computer = computer;
            InitializeVideoPages();
        }

        public void InitializeVideoPages()
        {
            SetVramPages(2);
            SetLayerVisibility(0xFF);
        }

        public void InitializeTexture()
        {
            _colorData = new Color[Constants.V_WIDTH * Constants.V_HEIGHT];
            _videoProjection = new Texture2D(
                Renderer.GetGraphicsDevice(),
                (int)Constants.V_WIDTH,
                (int)Constants.V_HEIGHT,
                false,
                Renderer.GetGraphicsDevice().PresentationParameters.BackBufferFormat
            );

            Renderer.EnsurePhosphorTargets(Renderer.GetGraphicsDevice(),
                (int)Constants.V_WIDTH,
                (int)Constants.V_HEIGHT);
        }

        private void UpdateVideoInfrastructure()
        {
            // TODO fix this assumption that the whole RAM is 16 MB
            VRAM_OFFSET = 0x1000000 - Constants.V_SIZE * VRAM_PAGES;
            // Update palettes
            for (byte p = 0; p < VRAM_PAGES; p++)
            {
                uint adr = GetVideoPagePaletteAddress(p);
                PALETTE_ADDRESSES[p] = adr;
                _computer.LoadMemAt(adr, Palettes.GetPaletteForPage(p));
            }

            _palettesStartAddress = GetVideoPagePaletteAddress((byte)(VRAM_PAGES - 1));
            _videoStartAddress = GetVideoPageAddress((byte)(VRAM_PAGES - 1));

            _videoBuffer = new byte[VRAM_PAGES * Constants.V_SIZE];
            _palettesBuffer = new byte[VRAM_PAGES * PALETTE_SIZE];

            SetLayerVisibility(LAYER_VISIBLE_BITS);
        }

        public void SetVramPages(byte pages)
        {
            VRAM_PAGES_OLD = VRAM_PAGES;
            VRAM_PAGES = pages;
            UpdateVideoInfrastructure();
        }

        public void SetLayerVisibility(byte values)
        {
            LAYER_VISIBLE_BITS = values;
            _visibleLayersCount = 0;
            for (int i = 0; i < 8; i++)
            {
                LAYER_VISIBLE[i] = (LAYER_VISIBLE_BITS & (1 << i)) != 0;
                if (LAYER_VISIBLE[i] && i < VRAM_PAGES)
                    _visibleLayersCount++;
            }

            lock (_visibleLayers)  // Lock to ensure thread safety
            {
                _visibleLayers = [0, 0, 0, 0, 0, 0, 0, 0];

                byte visibleIndex = 0;
                for (byte i = 0; i < VRAM_PAGES; i++)
                {
                    if (LAYER_VISIBLE[i])
                    {
                        _visibleLayers[visibleIndex] = i;
                        visibleIndex++;
                    }
                }
            }
        }

        public void SetLayerBufferControlMode(byte values)
        {
            LAYER_BUFFER_MODE_BITS = values;
            for (int i = 0; i < 8; i++)
                LAYER_BUFFER_AUTO_MODE[i] = (LAYER_BUFFER_MODE_BITS & (1 << i)) != 0;
        }

        public byte[] GetVisibleLayers()
        {
            return _visibleLayers;
        }

        public uint GetVideoPageAddress(byte page)
        {
            return (uint)(0x1000000 - Constants.V_SIZE * (page + 1));
        }

        public uint GetVideoPagePaletteAddress(byte page)
        {
            return (uint)(VRAM_OFFSET - PALETTE_SIZE * (page + 1));
        }

        public void Draw()
        {
            if (_videoProjection == null)
            {
                Renderer.DrawBlank();
                return;
            }
            
            DrawToProjection();

            Renderer.RenderToWindow(_videoProjection, (int)Constants.V_WIDTH, (int)Constants.V_HEIGHT);
        }

        private void DrawToProjection()
        {
            try
            {
                SetColorData();
            }
            catch (Exception ex)
            {
                Log.WriteLine($"SetColorData() failed: {ex.Message}");
            }

            _videoProjection.SetData(_colorData);
        }

        public void UpdateProjectionOnly()
        {
            if (_videoProjection == null)
                return;

            DrawToProjection();
        }


        private void SetColorData()
        {
            if (_visibleLayersCount == 0)
                return;

            UpdateBackBuffers();

            // Local refs
            uint video_width = Constants.V_WIDTH;
            uint video_height = Constants.V_HEIGHT;
            uint video_size = Constants.V_SIZE;
            Color[] colorData = _colorData;
            byte[] videoBuffer = _videoBuffer;
            byte[] palettesBuffer = _palettesBuffer;

            for (uint y = 0; y < video_height; y++)
            {
                for (uint x = 0; x < video_width; x++)
                {
                    int colVal = 0;
                    byte l;
                    uint rowOffset = y * video_width;

                    for (l = 0; l < _visibleLayersCount; l++)
                    {
                        colVal = videoBuffer[(video_size * _visibleLayers[l]) + rowOffset + x];
                        if (colVal != 0)    // color zero means transparent. Anything else (1-255) is opaque
                            break;
                    }

                    if (l >= _visibleLayersCount)
                    {
                        l = (byte)(_visibleLayersCount - 1);
                    }

                    long pAdr = (_visibleLayers[l]) * PALETTE_SIZE + (colVal * 3);

                    if (pAdr < 0)   // TODO fix bug, pAdr gets negative values: VRAM_PAGES = 1, l = 2
                        return;

                    // Creates directly the packed color value
                    colorData[y * video_width + x] = new Color(
                        (uint)(                 // (255 << 24) |
                        (palettesBuffer[pAdr + 2] << 16) |
                        (palettesBuffer[pAdr + 1] << 8) |
                        palettesBuffer[pAdr]));
                }
            }
        }

        private void UpdateBackBuffers()
        {
            for (byte i = 0; i < VRAM_PAGES; i++)
            {
                if (LAYER_BUFFER_AUTO_MODE[i])
                    UpdateBackBuffer((byte)(VRAM_PAGES - 1 - i));
            }
        }

        public void ManualUpdateBackBuffer(byte values)
        {
            for (byte i = 0; i < 8; i++)
            {
                if (i >= VRAM_PAGES)
                    return;

                if ((values & (1 << i)) != 0)
                    UpdateBackBuffer((byte)(VRAM_PAGES - 1 - i));
            }
        }

        public void ManualClearBackBuffer(byte values)
        {
            for (byte i = 0; i < 8; i++)
            {
                if (i >= VRAM_PAGES)
                    return;

                if ((values & (1 << i)) != 0)
                    ClearBackBuffer((byte)(VRAM_PAGES - 1 - i));
            }
        }

        public void UpdateBackBuffer(byte index)
        {
            Array.Copy(
                _computer.MEMC.RAM.Data, _videoStartAddress + Constants.V_SIZE * index,
                _videoBuffer, Constants.V_SIZE * index,
                Constants.V_SIZE);

            Array.Copy(
                _computer.MEMC.RAM.Data, _palettesStartAddress + PALETTE_SIZE * index,
                _palettesBuffer, PALETTE_SIZE * index,
                PALETTE_SIZE);
        }

        public void ClearBackBuffer(byte index)
        {
            Array.Clear(_videoBuffer, (int)(Constants.V_SIZE * index), (int)Constants.V_SIZE);
        }

        public byte[] GetVideoBuffer()
        {
            return _videoBuffer;
        }

        public byte[] GetPalettesBuffer()
        {
            return _palettesBuffer;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _videoProjection?.Dispose();
            }

            _disposed = true;
        }

        ~Graphics() => Dispose(false);  // Last resort
    }
}
