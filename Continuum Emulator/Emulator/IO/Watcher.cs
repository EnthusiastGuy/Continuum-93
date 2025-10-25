using System.IO;

namespace Continuum93.Emulator.IO
{
    public static class Watcher
    {
        static readonly FileSystemWatcher fileWatcher = new FileSystemWatcher();

        public static void WatchDirectoryOfFile(string path)
        {
            fileWatcher.Path = Path.GetDirectoryName(path);
            fileWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName;
            fileWatcher.Changed += new FileSystemEventHandler(Changed);
        }

        private static void Changed(object sender, FileSystemEventArgs e)
        {
            //Debug.WriteLine(sender);
            // Runner.LoadAndRun(SettingsManager.GetSettingValue("bootProgram"));
        }
    }
}
