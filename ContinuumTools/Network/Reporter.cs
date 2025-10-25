using System.Collections.Generic;
using System.Diagnostics;

namespace ContinuumTools.Network
{
    public static class Reporter
    {
        private static readonly List<Report> reports = new();

        public static void Update()
        {
            if (reports.Count == 0)
                return;

            for (int i = 0; i < reports.Count; i++)
            {
                reports[i].ShowInDebug();
            }

            reports.Clear();
        }

        public static void PushInfo(string message)
        {
            PushMessage(Report.State.Info, message);
        }

        public static void PushError(string message)
        {
            PushMessage(Report.State.Error, message);
        }

        public static void PushMessage(Report.State state, string message)
        {
            reports.Add(new Report(state, message));
        }
    }

    public class Report
    {
        public State _state;
        public string _message;

        public Report(State state, string message)
        {
            _state = state;
            _message = message;
        }

        public void ShowInDebug()
        {
            Debug.WriteLine($"{_state.GetName()}: {_message}");
        }

        public class State
        {
            public static State Error = new("Error");
            public static State Info = new("Info");
            public static State Warning = new("Warning");

            private string _state;

            public State(string state)
            {
                _state = state;
            }

            public string GetName()
            {
                return _state;
            }
        }
    }
}
