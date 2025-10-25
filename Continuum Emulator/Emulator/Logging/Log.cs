using Continuum93.Emulator.Settings;
using System;
using System.IO;

namespace Continuum93.Emulator
{
    public static class Log
    {
        private static readonly string logDir = Path.Combine(AppContext.BaseDirectory, "logs");
        private static readonly string logFile = $"{DateTime.Now:yyyy.MM.dd HH.mm.ss} emulator.log";
        private static readonly string fastLogFile = $"{DateTime.Now:yyyy.MM.dd HH.mm.ss} fastLog.log";
        private static readonly string logPath = Path.Combine(logDir, logFile);
        private static readonly string fastLogPath = Path.Combine(logDir, fastLogFile);
        private static string logBuffer = "";

        public static void WriteLine(string message)
        {
            bool logEnabled = SettingsManager.GetBoleanSettingsValue("logging");
            if (!logEnabled)
                return;

            logBuffer += $"{DateTime.Now:yyyy.MM.dd HH:mm:ss.fff}: {message}{Environment.NewLine}";
            if (!Directory.Exists(logDir))
                Directory.CreateDirectory(logDir);

            FlushData();
        }

        public static void WriteFast(string message)
        {
            File.AppendAllText(fastLogPath, $"{message}{Environment.NewLine}");
        }

        public static void FlushData()
        {
            if (logBuffer.Length == 0)
                return;

            try
            {
                File.AppendAllText(logPath, logBuffer);
                logBuffer = "";
            }
            catch (Exception)
            {
                logBuffer += $"{DateTime.Now:yyyy.MM.dd HH:mm:ss.fff}: Log writing met a concurrent request. Delaying write.{Environment.NewLine}"; ;
            }
        }
    }
}