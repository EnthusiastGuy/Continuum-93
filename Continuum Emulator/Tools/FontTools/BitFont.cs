using System;
using System.Collections.Generic;
using System.Linq;

namespace Continuum93.Tools.FontTools
{
    public class BitFont
    {
        private Glyph[] glyphs = new Glyph[95];

        public void UpdateGlyph(Glyph glyph)
        {
            if (glyph.Code < 32 || glyph.Code > 126)
                throw new Exception("Wrong glyph code");

            glyphs[glyph.Code - 32] = glyph;
        }

        public byte[] ExportFontType(byte type)
        {
            switch (type)
            {
                case 1:
                    return ExportSimpleFont();
                case 2:
                    return ExportVariableWidthFont();
                case 3:
                    break;
            }

            throw new Exception("Font type not found");
        }

        private byte[] ExportSimpleFont()
        {
            List<byte> bytes = new()
            {
                1,              // Font type one
                glyphs[0].w,    // Simple fonts all have same width
                glyphs.Max(x => x.h)    // All characters have same height, the largest
            };

            for (int i = 0; i < 95; i++)
                bytes.AddRange(glyphs[i].Data);

            return bytes.ToArray();
        }

        private byte[] ExportVariableWidthFont()
        {
            List<byte> bytes = new()
            {
                2,                      // Font type two
                glyphs[0].h     // All characters have same height 
            };

            for (int i = 0; i < 95; i++)
                bytes.Add(glyphs[i].w);

            for (int i = 0; i < 95; i++)
                bytes.AddRange(glyphs[i].Data);

            return bytes.ToArray();
        }

    }
}
