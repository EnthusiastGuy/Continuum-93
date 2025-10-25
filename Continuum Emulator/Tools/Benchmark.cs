using Continuum93.Emulator;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Continuum93.Tools
{
    public static class Benchmark
    {
        private static Stopwatch stopwatch = new Stopwatch();
        private static List<BenchTask> tasks = new List<BenchTask>();

        private static List<long> statistics = new List<long>();

        public static void StartOn(string key)
        {
            var task = tasks.Find(item => item.Key == key);
            if (task == null)
            {
                task = new BenchTask(key);
                tasks.Add(task);
            }

            task.StartStopWatch();
        }

        public static void StopOn(string key)
        {
            var task = tasks.Find(item => item.Key == key);

            task.EndStopWatch();
        }

        public static void StartStopWatch()
        {
            stopwatch.Start();
        }

        public static void EndStopWatch()
        {
            stopwatch.Stop();
            if (GameTimePlus.GetTotalMs() > 2000)
                statistics.Add(stopwatch.ElapsedTicks);

            stopwatch.Reset();
        }

        public static string GetAllStatistics()
        {
            string result = "";

            for (int i = 0; i < tasks.Count; i++)
            {
                result += tasks[i].GetStatistics() + Environment.NewLine;
            }

            return result;
        }

        public static string GetAllStatisticsDetailed()
        {
            string result = "";

            for (int i = 0; i < tasks.Count; i++)
            {
                result += tasks[i].GetStatisticsDetailed() + Environment.NewLine;
            }

            return result;
        }

        // One millisecond = 10_000 ticks
        public static long GetAverageTicks()
        {
            long response = 0;

            for (int i = 0; i < statistics.Count; i++)
            {
                response += statistics[i];
            }


            return statistics.Count > 0 ? response / statistics.Count : 0;
        }
    }
}
