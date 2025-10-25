namespace Last_Known_Reality.Reality
{
    using ContinuumTools.Network;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.IO;

    public class PngFont
    {
        private Texture2D _fontSheet;
        private JsonFont _fontData;
        private int _maxWidth = 0;

        //https://www.rapidtables.com/code/text/unicode-characters.html
        public PngFont(string name)
        {
            // load the texture and the font data
            string path = Path.Combine(Config.APP_PATH, "toolsData", Config.FontsFolder, name + ".json");
            string fontData = File.ReadAllText(path);
            _fontData = JsonConvert.DeserializeObject<JsonFont>(fontData, NOptions.newtonsoftJsonSerializerSettings);
            _fontSheet = GameContent.LoadTexture(Path.Combine(Config.APP_PATH, "toolsData", Config.FontsFolder, name + ".png"));

            CalculateMaxWidth();
        }

        public int GetSpacing()
        {
            return _fontData.Spacing;
        }

        public int GetMaxWidth()
        {
            return _maxWidth;
        }

        public void SetSpacing(int spacing)
        {
            _fontData.Spacing = spacing;
        }

        public Texture2D GetFontSheet()
        {
            return _fontSheet;
        }

        public Rectangle GetCharSource(char character)
        {
            string unicode = GetUnicodeFromChar(character);
            if (_fontData.Glyphs.ContainsKey(unicode))
            {
                Glyph glyph = _fontData.Glyphs[unicode];
                return new Rectangle(glyph.x, glyph.y, glyph.w, glyph.h);
            }
            else
            {
                return new Rectangle();
            }
        }

        public int GetCharactersKerning(char previous, char current)
        {
            string prevUnicode = GetUnicodeFromChar(previous);
            string currUnicode = GetUnicodeFromChar(current);
            int kerning = 0;
            if (_fontData.Kernings.ContainsKey(prevUnicode))
                if (_fontData.Kernings[prevUnicode].Secondary.ContainsKey(currUnicode))
                    kerning = _fontData.Kernings[prevUnicode].Secondary[currUnicode];

            return kerning;
        }

        public int MeasureStringWidth(string text)
        {
            int width = 1;
            char previous = ' ';
            foreach (char c in text)
            {
                width += GetCharSource(c).Width + GetSpacing() + GetCharactersKerning(previous, c);
                previous = c;
            }

            return width;
        }

        public int MeasureStringHeight(string text)
        {
            int height = 0;

            foreach (char c in text)
                if (height < GetCharSource(c).Height)
                    height = GetCharSource(c).Height;

            return height;
        }

        /// <summary>
        /// Get how wide the space character is, in pixels
        /// </summary>
        /// <returns>The number of pixels the space character has</returns>
        public int GetSpaceWidth()
        {
            return _fontData.Glyphs.ContainsKey("0020") ? _fontData.Glyphs["0020"].w : 0;
        }

        /// <summary>
        /// Sets the number of pixels the space character advances the next letter for
        /// </summary>
        /// <param name="width">The width in pixels</param>
        public void SetSpaceWidth(int width)
        {
            Glyph spaceGlyph;

            if (_fontData.Glyphs.ContainsKey("0020"))
            {
                spaceGlyph = _fontData.Glyphs["0020"];
            }
            else
            {
                spaceGlyph = new Glyph();
                foreach (var pair in _fontData.Glyphs)
                {
                    spaceGlyph.h = pair.Value.h;
                    break;
                }
            }

            spaceGlyph.w = width;

            _fontData.Glyphs["0020"] = spaceGlyph;
        }

        private string GetUnicodeFromChar(char c)
        {
            return ((int)c).ToString("X4");
        }

        private void CalculateMaxWidth()
        {
            for (byte i = 32; i < 128; i++)
            {
                int width = GetCharSource((char)i).Width;
                if (_maxWidth < width)
                {
                    _maxWidth = width;
                }
            }
        }

    }

    public class JsonFont
    {
        public string FontName;
        public int Spacing;
        public Dictionary<string, Glyph> Glyphs;
        public Dictionary<string, Kerning> Kernings;
    }

    public class Glyph
    {
        public int x;
        public int y;
        public int w;
        public int h;
    }

    public class Kerning
    {
        public Dictionary<string, int> Secondary;
    }
}
