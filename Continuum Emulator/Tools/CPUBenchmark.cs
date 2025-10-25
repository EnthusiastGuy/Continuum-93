using System.Diagnostics;

namespace Continuum93.Tools
{
    public static class CPUBenchmark
    {
        private static long _instructionsRan = 0;
        private static Stopwatch _timer = new();

        public static void Start()
        {
            _timer.Start();
        }

        public static void IncrementInstructions()
        {
            _instructionsRan++;
        }

        public static double GetInstructionsPerSecond()
        {
            _timer.Stop();
            return _instructionsRan / _timer.Elapsed.TotalSeconds;
        }
    }
}
