using System;

namespace Continuum93.Emulator.Interpreter
{
    public static class CompileLog
    {
        private static string _log;
        public static bool StopBuild;

        public static void Log(string message, CompileIssue issue)
        {
            _log += string.Format("{0}: {1}{2}", issue.Text, message, Environment.NewLine);
            StopBuild = (issue == CompileIssue.Error) || StopBuild;
        }

        public static string GetLog()
        {
            return _log;
        }

        public static void Reset()
        {
            _log = "";
            StopBuild = false;
        }
    }

    public class CompileIssue
    {
        public static readonly CompileIssue Info = new CompileIssue("Info");
        public static readonly CompileIssue Warning = new CompileIssue("Warning");
        public static readonly CompileIssue Error = new CompileIssue("Error");

        public string Text;

        private CompileIssue(string value)
        {
            Text = value;
        }
    }
}
