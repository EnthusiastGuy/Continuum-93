using System.IO;

namespace Continuum93.Emulator.IO
{
    public static class FileManager
    {
        public static string ReadFile(string path)
        {
            return File.ReadAllText(path);
        }

        public static bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public static bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }
    }
}
