using System;
using System.Collections.Generic;
using System.Linq;

namespace Continuum93.ServiceModule.Parsers
{
    public static class StepHistory
    {
        private static readonly List<DissLine> _history = new();
        private const int MAX_HISTORY = 100;

        public static void PushToHistory(DissLine line)
        {
            if (line == null)
                return;

            _history.Add(line);
            if (_history.Count > MAX_HISTORY)
            {
                _history.RemoveAt(0);
            }
        }

        public static List<DissLine> GetAtMost(int count)
        {
            if (_history.Count == 0)
                return new List<DissLine>();

            int start = Math.Max(0, _history.Count - count);
            return _history.Skip(start).ToList();
        }
    }
}

