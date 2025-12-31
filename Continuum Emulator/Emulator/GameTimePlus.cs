using Microsoft.Xna.Framework;

namespace Continuum93.Emulator
{
    public static class GameTimePlus
    {
        //private static readonly double TICKS_PER_SECOND = 10_000_000;
        //private static readonly double TICKS_PER_MS = 10_000;
        private static GameTime _gameTime;

        public static void Update(GameTime gameTime)
        {
            _gameTime = gameTime;
        }

        public static double GetTotalMs()
        {
            return _gameTime != null ? _gameTime.TotalGameTime.TotalMilliseconds : 0;
        }

        /*
         * TICKS_PER_SECOND = 10_000_000;
         * TICKS_PER_MS = 10_000;
         */
        public static long GetTotalTicks()
        {
            return _gameTime.TotalGameTime.Ticks;
        }

        public static double GetTotalSeconds()
        {
            return _gameTime != null ? _gameTime.TotalGameTime.TotalSeconds : 0;
        }
    }
}
