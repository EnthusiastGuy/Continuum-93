using System;
using System.Collections.Generic;
using System.Linq;

namespace Continuum93.ServiceModule.Parsers
{
    public static class StepHistory
    {
        private static readonly List<DissLine> _history = new();
        private const int MAX_HISTORY = 100;
        private static int? _lastAddress;

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

        public static void PushCurrent(DissLine current)
        {
            if (current == null)
                return;

            if (_lastAddress.HasValue && _lastAddress.Value == current.Address)
                return;

            _lastAddress = current.Address;
            PushToHistory(current);
        }

        public static List<DissLine> GetFittingWidth(int maxWidth, int charWidth, int paddingPerItem)
        {
            List<DissLine> result = new();
            int widthUsed = 0;

            for (int i = _history.Count - 1; i >= 0; i--)
            {
                string instr = _history[i].Instruction ?? string.Empty;
                int width = instr.Length * charWidth + paddingPerItem;

                if (widthUsed + width > maxWidth)
                    break;

                widthUsed += width;
                result.Insert(0, _history[i]); // keep chronological order (oldest -> newest)
            }

            return result;
        }
    }
}

