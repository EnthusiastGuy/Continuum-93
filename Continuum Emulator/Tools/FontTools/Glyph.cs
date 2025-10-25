using System.Collections.Generic;

namespace Continuum93.Tools.FontTools
{
    public class Glyph
    {
        public byte Code;
        public short x;
        public short y;
        public byte w;
        public byte h;
        public string c;

        public List<byte> Data = new();

    }
}
