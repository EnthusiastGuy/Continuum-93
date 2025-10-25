using System;
using System.IO;

namespace Continuum93.Emulator
{
    public static class Constants
    {
        public static readonly string FS_ROOT = Path.Combine(AppContext.BaseDirectory, "Data", "filesystem");

        public static readonly uint V_WIDTH = 480;
        public static readonly uint V_HEIGHT = 270;
        public static readonly uint V_SIZE = V_WIDTH * V_HEIGHT;    // Video size

        public static readonly byte B_TRUE = 0xFF;
        public static readonly byte B_FALSE = 0x00;
        public static readonly char TERMINATOR = '\0';

        public static readonly uint MAX24BIT = 0xFFFFFF;

        public static readonly char LBL_ABSOLUTE = '.';
        public static readonly char LBL_RELATIVE = '~';

        public static readonly char[] LABEL_MARKERS = new char[] { LBL_ABSOLUTE, LBL_RELATIVE };

        public static string CR = Environment.NewLine;
        public static string END_PAGE = "#EndPage#";

        public static string ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZABC";
    }
}
