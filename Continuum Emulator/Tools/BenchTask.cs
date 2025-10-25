using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Continuum93.Tools
{
    public class BenchTask
    {
        private Stopwatch stopwatch = new Stopwatch();
        private List<long> statistics = new List<long>();
        public string Key;

        public BenchTask(string key)
        {
            Key = key;
        }

        public void StartStopWatch()
        {
            stopwatch.Start();
        }

        public void EndStopWatch()
        {
            stopwatch.Stop();
            statistics.Add(stopwatch.ElapsedTicks);

            stopwatch.Reset();
        }

        public long GetAverageTicks()
        {
            long response = 0;
            statistics.RemoveRange(0, Math.Min(30, statistics.Count));

            for (int i = 0; i < statistics.Count; i++)
            {
                response += statistics[i];
            }

            return statistics.Count > 0 ? response / statistics.Count : 0;
        }

        public string GetStatistics()
        {
            return Key + ": " + GetAverageTicks();
        }

        public string GetStatisticsDetailed()
        {
            string result = Key + ": ";
            for (int i = 0; i < statistics.Count; i++)
            {
                result += statistics[i] + ", ";
            }

            return result;
        }
    }
}
