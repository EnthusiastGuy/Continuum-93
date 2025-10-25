using System.Runtime.CompilerServices;

namespace Continuum93.Emulator
{
    public static class DelayHelper
    {
        private static volatile int _dummy; // volatile ensures that accesses aren’t optimized away.

        /// <summary>
        /// Busy-waits by performing a volatile operation on _dummy.
        /// Increasing the iteration count increases the delay.
        /// </summary>
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public static void AtomicDelay(int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                _dummy++;
            }
        }
    }
}
