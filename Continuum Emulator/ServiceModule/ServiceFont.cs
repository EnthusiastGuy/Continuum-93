using Continuum93.Emulator;
using Continuum93.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Continuum93.ServiceModule
{
    [Flags]
    public enum ServiceFontFlags : byte
    {
        Monospace = 1 << 0,             // 0b00000001
        MonospaceCentering = 1 << 1,    // 0b00000010
        DisableKerning = 1 << 2,        // 0b00000100
        Center = 1 << 3,                // 0b00001000
        Wrap = 1 << 4,                  // 0b00010000
        DrawOutline = 1 << 5,           // 0b00100000
    }

    public sealed class ServiceFont : IDisposable
    {
        public Texture2D Texture { get; }
        public int GlyphCellWidth { get; }
        public int GlyphCellHeight { get; }

        // 96 glyphs: ASCII 32..127 inclusive
        private readonly byte[] _glyphWidths = new byte[96];
        private readonly byte[] _glyphInkWidth = new byte[96];   // width of actual ink box
        private readonly byte[] _glyphInkOffset = new byte[96];   // left offset of ink inside cell

        private readonly int _spaceGlyphWidth;
        private readonly Dictionary<(byte, byte), sbyte> _kerningPairs = new();

        private readonly Color[] _pixelData;
        private readonly int _texWidth;
        private readonly int _texHeight;

        private const byte FirstAscii = 32;
        private const byte LastAscii = 127;

        public ServiceFont(string fontPath)
        {
            GraphicsDevice graphicsDevice = Renderer.GetGraphicsDevice();
            string imagePath = Path.Combine(
                DataConverter.GetCrossPlatformPath(fontPath)
            );

            string kerningPath = Path.Combine(
                DataConverter.GetCrossPlatformPath(Path.ChangeExtension(imagePath, ".txt"))
            );


            if (graphicsDevice == null) throw new ArgumentNullException(nameof(graphicsDevice));
            if (imagePath == null) throw new ArgumentNullException(nameof(imagePath));

            using (var fs = File.OpenRead(imagePath))
            {
                Texture = Texture2D.FromStream(graphicsDevice, fs);
            }

            _texWidth = Texture.Width;
            _texHeight = Texture.Height;

            // 16 x 6 grid sanity
            GlyphCellWidth = _texWidth / 16;
            GlyphCellHeight = _texHeight / 6;

            // Grab all pixels once, then scan for widths
            _pixelData = new Color[_texWidth * _texHeight];
            Texture.GetData(_pixelData);

            // NEW: normalize the font sheet for tinting
            NormalizeFontTextureUsingSpaceCell();

            // Write modified pixels back into the texture
            Texture.SetData(_pixelData);

            ComputeGlyphWidths();

            int maxGlyph = 0;
            for (int i = 0; i < _glyphWidths.Length; i++)
            {
                if (_glyphWidths[i] > maxGlyph)
                    maxGlyph = _glyphWidths[i];
            }

            _spaceGlyphWidth = (int)Math.Round(maxGlyph * 0.6f);
            if (_spaceGlyphWidth <= 0)
                _spaceGlyphWidth = GlyphCellWidth; // fallback

            LoadKerning(kerningPath);
        }

        private void NormalizeFontTextureUsingSpaceCell()
        {
            // Collect all colors that appear in the first cell (space glyph).
            // That cell is at (0,0) with size GlyphCellWidth x GlyphCellHeight.
            var bgColors = new List<Color>();

            for (int y = 0; y < GlyphCellHeight; y++)
            {
                for (int x = 0; x < GlyphCellWidth; x++)
                {
                    var c = _pixelData[y * _texWidth + x];
                    bgColors.Add(c);
                }
            }

            bool IsBackground(Color c)
            {
                // Optional small tolerance in case of slight variations.
                foreach (var bg in bgColors)
                {
                    if (Math.Abs(c.R - bg.R) <= 2 &&
                        Math.Abs(c.G - bg.G) <= 2 &&
                        Math.Abs(c.B - bg.B) <= 2)
                    {
                        return true;
                    }
                }
                return false;
            }

            // Now remap the whole texture:
            for (int i = 0; i < _pixelData.Length; i++)
            {
                var c = _pixelData[i];

                if (IsBackground(c))
                {
                    // Fully transparent background
                    _pixelData[i] = new Color(0, 0, 0, 0);
                }
                else
                {
                    // Solid white glyph pixel, so SpriteBatch tinting works
                    _pixelData[i] = new Color(255, 255, 255, 255);
                }
            }
        }



        public void Dispose()
        {
            Texture?.Dispose();
        }

        // ---------------------------------------------------------------------
        // Public API – Draw & Measure
        // ---------------------------------------------------------------------

        /// <summary>
        /// Draw text into the current SpriteBatch using emulator-like semantics.
        /// </summary>
        public void DrawString(
            
            string text,
            float x,
            float y,
            Color color,
            int maxWidthPixels,
            byte flagsByte,
            Color outlineColor,
            byte outlinePattern)
        {
            SpriteBatch spriteBatch = Renderer.GetSpriteBatch();
            if (spriteBatch == null) throw new ArgumentNullException(nameof(spriteBatch));
            if (string.IsNullOrEmpty(text)) return;

            var flags = (ServiceFontFlags)flagsByte;

            bool monospace = flags.HasFlag(ServiceFontFlags.Monospace);
            bool monospaceCentering = flags.HasFlag(ServiceFontFlags.MonospaceCentering);
            bool disableKerning = flags.HasFlag(ServiceFontFlags.DisableKerning);
            bool center = flags.HasFlag(ServiceFontFlags.Center);
            bool wrap = flags.HasFlag(ServiceFontFlags.Wrap);
            bool drawOutline = flags.HasFlag(ServiceFontFlags.DrawOutline);

            byte characterSpacing = 1;
            int maxWidth = Math.Max(0, maxWidthPixels);

            // Split on explicit newlines first
            var paragraphs = text.Replace("\r", string.Empty).Split('\n');

            var allLines = new List<string>();
            foreach (var p in paragraphs)
            {
                if (wrap || center)
                {
                    var wrapped = WrapText(
                        p,
                        _glyphWidths,
                        (byte)GlyphCellWidth,
                        monospace,
                        monospaceCentering,
                        characterSpacing,
                        (ushort)maxWidth,
                        _kerningPairs,
                        disableKerning,
                        _spaceGlyphWidth);

                    allLines.AddRange(wrapped);
                }
                else
                {
                    allLines.Add(p);
                }
            }

            int lineHeight = GlyphCellHeight + characterSpacing;
            int posY = (int)y;

            foreach (var line in allLines)
            {
                int lineWidth = CalculateLineWidth(
                    line,
                    _glyphWidths,
                    (byte)GlyphCellWidth,
                    monospace,
                    monospaceCentering,
                    characterSpacing,
                    _kerningPairs,
                    disableKerning,
                    _spaceGlyphWidth);

                int startX;
                if (center && maxWidth > 0)
                    startX = (int)x + (maxWidth - lineWidth) / 2;
                else
                    startX = (int)x;

                RenderLine(
                    spriteBatch,
                    line,
                    startX,
                    posY,
                    _glyphWidths,
                    (byte)GlyphCellWidth,
                    (byte)GlyphCellHeight,
                    monospace,
                    monospaceCentering,
                    drawOutline,
                    outlineColor,
                    outlinePattern,
                    characterSpacing,
                    color,
                    _kerningPairs,
                    disableKerning,
                    (ushort)maxWidth);

                posY += lineHeight;
            }
        }

        /// <summary>
        /// Basic metrics similar to GetTextRectangle: width, height, line count.
        /// </summary>
        public (int width, int height, int lines) MeasureText(
            string text,
            int maxWidthPixels,
            byte flagsByte)
        {
            if (string.IsNullOrEmpty(text))
                return (0, 0, 0);

            var flags = (ServiceFontFlags)flagsByte;

            bool monospace = flags.HasFlag(ServiceFontFlags.Monospace);
            bool monospaceCentering = flags.HasFlag(ServiceFontFlags.MonospaceCentering);
            bool disableKerning = flags.HasFlag(ServiceFontFlags.DisableKerning);
            bool center = flags.HasFlag(ServiceFontFlags.Center);
            bool wrap = flags.HasFlag(ServiceFontFlags.Wrap);
            bool drawOutline = flags.HasFlag(ServiceFontFlags.DrawOutline);

            byte characterSpacing = 1;
            int maxWidth = Math.Max(0, maxWidthPixels);

            var paragraphs = text.Replace("\r", string.Empty).Split('\n');
            var allLines = new List<string>();

            foreach (var p in paragraphs)
            {
                if (wrap || center)
                {
                    var wrapped = WrapText(
                        p,
                        _glyphWidths,
                        (byte)GlyphCellWidth,
                        monospace,
                        monospaceCentering,
                        characterSpacing,
                        (ushort)maxWidth,
                        _kerningPairs,
                        disableKerning,
                        _spaceGlyphWidth);

                    allLines.AddRange(wrapped);
                }
                else
                {
                    allLines.Add(p);
                }
            }

            int maxRenderedLineWidth = 0;
            foreach (var line in allLines)
            {
                int lineWidth = CalculateLineWidth(
                    line,
                    _glyphWidths,
                    (byte)GlyphCellWidth,
                    monospace,
                    monospaceCentering,
                    characterSpacing,
                    _kerningPairs,
                    disableKerning,
                    _spaceGlyphWidth);

                int effective = (maxWidth > 0) ? Math.Min(lineWidth, maxWidth) : lineWidth;
                if (effective > maxRenderedLineWidth)
                    maxRenderedLineWidth = effective;
            }

            int lineHeight = GlyphCellHeight + characterSpacing;
            int linesCount = allLines.Count;
            int baseHeight = (linesCount == 0)
                ? 0
                : (linesCount * GlyphCellHeight) + ((linesCount - 1) * characterSpacing);

            int outlinePad = drawOutline ? 2 : 0;

            int finalWidth = maxRenderedLineWidth + outlinePad;
            int finalHeight = baseHeight + outlinePad;

            return (finalWidth, finalHeight, linesCount);
        }

        // ---------------------------------------------------------------------
        // Internal helpers: glyph width from PNG, kerning, wrap, render...
        // ---------------------------------------------------------------------

        private void ComputeGlyphWidths()
        {
            for (int index = 0; index < 96; index++)
            {
                int col = index % 16;
                int row = index / 16;

                int cellX = col * GlyphCellWidth;
                int cellY = row * GlyphCellHeight;

                int maxX = -1;
                int minX = GlyphCellWidth;

                for (int x = 0; x < GlyphCellWidth; x++)
                {
                    for (int y = 0; y < GlyphCellHeight; y++)
                    {
                        var p = GetPixel(cellX + x, cellY + y);
                        if (p.A != 0)
                        {
                            if (x > maxX) maxX = x;
                            if (x < minX) minX = x;
                        }
                    }
                }

                if (maxX < 0)
                {
                    // Empty cell
                    _glyphWidths[index] = 0;
                    _glyphInkWidth[index] = 0;
                    _glyphInkOffset[index] = 0;
                }
                else
                {
                    // Ink box [minX .. maxX]
                    byte inkWidth = (byte)(maxX - minX + 1);

                    // Use the trimmed ink width as the glyph width (matches how the extractor would behave)
                    _glyphWidths[index] = inkWidth;
                    _glyphInkOffset[index] = (byte)minX;
                    _glyphInkWidth[index] = inkWidth;
                }
            }
        }


        private void LoadKerning(string kerningPath)
        {
            if (string.IsNullOrEmpty(kerningPath))
                return;

            var kerning = new Kerning(kerningPath);
            var bytes = kerning.GetKerningBytes();

            for (int i = 0; i + 2 < bytes.Length; i += 3)
            {
                byte c1 = bytes[i];
                byte c2 = bytes[i + 1];
                sbyte v = unchecked((sbyte)bytes[i + 2]);

                _kerningPairs[(c1, c2)] = v;
            }
        }

        private Color GetPixel(int x, int y)
        {
            return _pixelData[y * _texWidth + x];
        }

        // ------------------------ wrapping & widths ---------------------------

        private static List<string> WrapText(
            string text,
            byte[] glyphWidths,
            byte glyphCellWidth,
            bool monospace,
            bool monospaceCentering,
            byte characterSpacing,
            ushort maxWidth,
            Dictionary<(byte, byte), sbyte> kerningPairsDict,
            bool disableKerning,
            int spaceGlyphWidth)
        {
            var lines = new List<string>();
            if (string.IsNullOrEmpty(text))
            {
                lines.Add(string.Empty);
                return lines;
            }

            var words = text.Split(' ');
            var currentLine = new StringBuilder();
            int currentLineWidth = 0;
            int spaceWidth = monospace
                ? glyphCellWidth + characterSpacing
                : spaceGlyphWidth + characterSpacing;

            foreach (var word in words)
            {
                int wordWidth = CalculateLineWidth(
                    word,
                    glyphWidths,
                    glyphCellWidth,
                    monospace,
                    monospaceCentering,
                    characterSpacing,
                    kerningPairsDict,
                    disableKerning,
                    spaceGlyphWidth);

                if (maxWidth > 0 &&
                    currentLineWidth + wordWidth > maxWidth &&
                    currentLine.Length > 0)
                {
                    lines.Add(currentLine.ToString().TrimEnd());
                    currentLine.Clear();
                    currentLineWidth = 0;
                }

                currentLine.Append(word);
                currentLine.Append(' ');
                currentLineWidth += wordWidth + spaceWidth;
            }

            if (currentLine.Length > 0)
                lines.Add(currentLine.ToString().TrimEnd());

            return lines;
        }

        private static int CalculateLineWidth(
            string line,
            byte[] glyphWidths,
            byte glyphCellWidth,
            bool monospace,
            bool monospaceCentering,
            byte characterSpacing,
            Dictionary<(byte, byte), sbyte> kerningPairsDict,
            bool disableKerning,
            int spaceGlyphWidth)
        {
            int lineWidth = 0;
            byte lastAscii = 0;

            foreach (char c in line)
            {
                byte ascii = (byte)c;
                if (ascii < FirstAscii || ascii > LastAscii)
                    continue;

                if (ascii == 32)
                {
                    int gw = monospace ? glyphCellWidth : spaceGlyphWidth;
                    lineWidth += gw + characterSpacing;
                    lastAscii = ascii;
                    continue;
                }

                int glyphIndex = ascii - FirstAscii;
                int glyphWidth = monospace ? glyphCellWidth : glyphWidths[glyphIndex];

                int kerningOffset = 0;
                if (!disableKerning && !monospace && lastAscii != 0)
                {
                    if (kerningPairsDict.TryGetValue((lastAscii, ascii), out sbyte offset))
                        kerningOffset = offset;
                }

                lineWidth += glyphWidth + characterSpacing + kerningOffset;
                lastAscii = ascii;
            }

            return lineWidth;
        }

        // -------------------------- rendering --------------------------------

        private static readonly (int dx, int dy, byte bit)[] OutlineOffsets =
        {
        (-1, -1, 7),
        (-1,  0, 6),
        (-1,  1, 5),
        ( 0, -1, 4),
        ( 0,  1, 3),
        ( 1, -1, 2),
        ( 1,  0, 1),
        ( 1,  1, 0)
    };

        private void RenderLine(
            SpriteBatch spriteBatch,
            string line,
            int startX,
            int startY,
            byte[] glyphWidths,
            byte glyphCellWidth,
            byte glyphCellHeight,
            bool monospace,
            bool monospaceCentering,
            bool drawOutline,
            Color outlineColor,
            byte outlinePattern,
            byte characterSpacing,
            Color color,
            Dictionary<(byte, byte), sbyte> kerningPairsDict,
            bool disableKerning,
            ushort maxWidth)
        {
            int posX = startX;
            byte lastAscii = 0;

            foreach (char c in line)
            {
                byte ascii = (byte)c;

                // Space
                if (ascii == 32) // Space
                {
                    int spaceWidth = monospace ? glyphCellWidth : _spaceGlyphWidth;
                    posX += spaceWidth + characterSpacing;
                    lastAscii = ascii;
                    continue;
                }

                if (ascii < FirstAscii || ascii > LastAscii)
                    continue;

                int glyphIndex = ascii - FirstAscii;

                int cellCol = glyphIndex % 16;
                int cellRow = glyphIndex / 16;

                int cellX = cellCol * glyphCellWidth;
                int cellY = cellRow * glyphCellHeight;

                int actualWidth = glyphWidths[glyphIndex];
                int glyphWidth = monospace ? glyphCellWidth : actualWidth;
                int glyphHeight = glyphCellHeight;

                // Kerning
                int kerningOffset = 0;
                if (!disableKerning && !monospace && lastAscii != 0)
                {
                    if (kerningPairsDict.TryGetValue((lastAscii, ascii), out sbyte offset))
                        kerningOffset = offset;
                }

                // Default: use full cell width from the left
                int srcX = cellX;
                int srcWidth = actualWidth;
                int glyphPosX = posX + kerningOffset;

                // When monospace centering is enabled, center the *ink box* in the cell.
                if (monospace && monospaceCentering && _glyphInkWidth[glyphIndex] > 0)
                {
                    int inkWidth = _glyphInkWidth[glyphIndex];
                    int inkOffset = _glyphInkOffset[glyphIndex];

                    srcX = cellX + inkOffset;
                    srcWidth = inkWidth;

                    glyphPosX = posX + kerningOffset + (glyphCellWidth - inkWidth) / 2;
                }

                // Respect maxWidth: stop when this glyph overflows horizontally (non-mono)
                if (!monospace && maxWidth > 0 &&
                    (glyphPosX - startX) + glyphWidth > maxWidth)
                {
                    break;
                }

                var srcRect = new Rectangle(srcX, cellY, srcWidth, glyphHeight);


                // Draw outline as 8 copies offset, controlled by outlinePattern
                if (drawOutline && outlinePattern != 0)
                {
                    foreach (var (dx, dy, bit) in OutlineOffsets)
                    {
                        if ((outlinePattern & (1 << bit)) == 0)
                            continue;

                        spriteBatch.Draw(
                            Texture,
                            new Vector2(glyphPosX + dx, startY + dy),
                            srcRect,
                            outlineColor);
                    }
                }

                // Main glyph
                spriteBatch.Draw(
                    Texture,
                    new Vector2(glyphPosX, startY),
                    srcRect,
                    color);

                posX += glyphWidth + characterSpacing + kerningOffset;
                lastAscii = ascii;
            }
        }
    }
}
