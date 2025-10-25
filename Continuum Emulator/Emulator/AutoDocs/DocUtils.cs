using System.Text.RegularExpressions;

namespace Continuum93.Emulator.AutoDocs
{
    public static class DocUtils
    {
        public static string GetReadableBytesSize(int size)
        {
            return string.Format("{0} {1}", size, size == 1 ? "byte" : "bytes");
        }

        public static int CountFormatArguments(string input)
        {
            // Regular expression to match placeholders like {0}, {1}, etc.
            string pattern = @"\{\d+\}";

            // Count the number of matches
            int count = Regex.Matches(input, pattern).Count;
            return count;
        }
    }
}
