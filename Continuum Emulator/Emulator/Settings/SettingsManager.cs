using System;
using System.Collections.Generic;
using System.IO;

namespace Continuum93.Emulator.Settings
{
    public static class SettingsManager
    {
        private static readonly string CONFIG_PATH = Path.Combine(AppContext.BaseDirectory, DataConverter.GetCrossPlatformPath("Data"), "init.cfg");
        private static Dictionary<string, string> _settings = [];

        public static void LoadConfig()
        {
            if (File.Exists(CONFIG_PATH))
            {
                ParseConfig(File.ReadAllText(CONFIG_PATH));
            }
            else
            {
                File.WriteAllText(CONFIG_PATH,
                    "# This provides the configuration to be loaded when Continuum starts." + Environment.NewLine + Environment.NewLine +

                    "# Controls whether the emulator enables network communication with any Continuum Tools listening." + Environment.NewLine +
                    "enableDebugging=true" + Environment.NewLine + Environment.NewLine +

                    "# Attempts to start in fullscreen. Usually this would be succesful." + Environment.NewLine +
                    "fullscreen=false" + Environment.NewLine + Environment.NewLine +

                    "# Disables the mouse API (some linux based OSs need this for Continuum to run)." + Environment.NewLine +
                    "disableMouse=false" + Environment.NewLine + Environment.NewLine +

                    "# Points to the assembly source file that will be automatically loaded and ran on boot." + Environment.NewLine +
                    "bootProgram=Data\\os\\q.asm" + Environment.NewLine + Environment.NewLine +

                    "# The port for the Continuum Tools to connect through." + Environment.NewLine +
                    "serverPort=21098" + Environment.NewLine + Environment.NewLine +

                    "# Enable logging events" + Environment.NewLine +
                    "logging=true" + Environment.NewLine + Environment.NewLine

                //"# Points to the directory that mounts as drive A for Continuum." + Environment.NewLine +
                //@"driveAPath=Data\filesystem" + Environment.NewLine +
                //"" + Environment.NewLine +

                //+ "# This enables active monitoring of the bootProgram .asm file. Once any changes are" + Environment.NewLine +
                //"# detected, the .asm file is automatically reloaded and ran." + Environment.NewLine +
                //"monitorBootProgramChanges=true" + Environment.NewLine
                );

                ParseConfig(File.ReadAllText(CONFIG_PATH));
            }
        }

        public static int GetIntSettingValue(string settingKey)
        {
            if (_settings.ContainsKey(settingKey))
            {
                bool conversionSuccess = int.TryParse(_settings[settingKey], out int response);
                if (conversionSuccess)
                {
                    return response;
                }
            }

            return -1;
        }

        public static string GetSettingValue(string settingKey)
        {
            if (_settings.ContainsKey(settingKey))
                return _settings[settingKey];

            return null;
        }

        public static string GetPathSettingValue(string settingKey)
        {
            string path = GetSettingValue(settingKey);
            if (path == null)
                return null;

            return Path.Combine(AppContext.BaseDirectory, DataConverter.GetCrossPlatformPath(path));
        }

        public static bool GetBoleanSettingsValue(string settingKey)
        {
            return GetSettingValue(settingKey) == "true";
        }

        private static void ParseConfig(string data)
        {
            string[] lines = data.Split(Environment.NewLine);
            _settings.Clear();

            foreach (string line in lines)
            {
                string cline = line.Trim();

                if (cline.Length == 0)
                    continue;                   // Ignore empty lines

                if (cline[0].Equals('#'))
                    continue;                   // Ignore comments

                if (!cline.Contains("="))
                    continue;                   // malformed setting

                string key = cline.Split('=')[0];
                string value = cline.Split('=')[1];

                _settings.Add(key, value);
            }
        }
    }
}
