using System.Reflection;

namespace Continuum93.Emulator.Window
{
    public static class Version
    {
        public static string GetVersion()
        {
            System.Version info = Assembly.GetEntryAssembly().GetName().Version;
            return $"V {info.Major}.{info.Minor}.{info.Build}";
        }
    }
}
