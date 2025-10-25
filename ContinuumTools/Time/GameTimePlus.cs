namespace Last_Known_Reality.Reality
{
    using Microsoft.Xna.Framework;

    // This class adds some time lanes that increment at the same time.
    // The added value is that the increment multiplier can be changed from the default 1.0
    // thus allowing the developer to implement story situations where time can be slowed or advanced
    // without extensive programming.
    public static class GameTimePlus
    {
        private static readonly int TIME_LINES = 2;
        private static readonly double TICKS_PER_SECOND = 10_000_000;
        private static readonly double TICKS_PER_MS = 10_000;
        private static GameTime _gameTime;
        private static long[] _relativeTicksBefore;
        private static long[] _relativeTicksNow;
        private static float[] _timeMultiplier;

        // TODO move the time lines to the state so they get saved
        public static void Initialize()
        {
            _relativeTicksNow = new long[TIME_LINES];
            _relativeTicksBefore = new long[TIME_LINES];
            _timeMultiplier = new float[TIME_LINES];
            for (int i = 0; i < TIME_LINES; i++)
            {
                _timeMultiplier[i] = 1.0f;
            }
        }

        public static void Update(GameTime gameTime)
        {
            // gameTime.ElapsedGameTime - Time since the last call to Update(GameTime). Typically 0.01(6) seconds for 60fps
            // gameTime.IsRunningSlowly - Indicates whether the Game is running slowly. This flag is set to true when IsFixedTimeStep
            //                            is set to true and a tick of the game loop takes longer than TargetElapsedTime for a few frames in a row.
            // gameTime.TotalGameTime - Time since the start of the Game

            _gameTime = gameTime;
            UpdateTimeLines();
        }

        public static double GetElapsedRelativeTimeMs(int timeLineIndex)
        {
            return (_relativeTicksNow[timeLineIndex] - _relativeTicksBefore[timeLineIndex]) / TICKS_PER_MS;
        }

        public static double GetElapsedRelativeTimeSeconds(int timeLineIndex)
        {
            return (_relativeTicksNow[timeLineIndex] - _relativeTicksBefore[timeLineIndex]) / TICKS_PER_SECOND;
        }

        public static double GetElapsedRelativeTimeTicks(int timeLineIndex)
        {
            return (_relativeTicksNow[timeLineIndex] - _relativeTicksBefore[timeLineIndex]);
        }

        public static double GetTotalRelativeTimeMs(int timeLineIndex)
        {
            return _relativeTicksNow[timeLineIndex] / TICKS_PER_MS;
        }

        public static double GetTotalRelativeTimeSeconds(int timeLineIndex)
        {
            return _relativeTicksNow[timeLineIndex] / TICKS_PER_SECOND;
        }

        public static double GetTotalRelativeTimeTicks(int timeLineIndex)
        {
            return _relativeTicksNow[timeLineIndex];
        }

        public static float GetRelativeTimeMultiplier(int timeLineIndex)
        {
            return _timeMultiplier[timeLineIndex];
        }

        public static void SetRelativeTimeMultiplier(int timeLineIndex, float modifier)
        {
            _timeMultiplier[timeLineIndex] = modifier;
        }

        private static void UpdateTimeLines()
        {
            double timeDiff = _gameTime.ElapsedGameTime.Ticks;
            for (int i = 0; i < TIME_LINES; i++)
            {
                _relativeTicksBefore[i] = _relativeTicksNow[i];
                _relativeTicksNow[i] += (long)(timeDiff * _timeMultiplier[i]);
            }
        }
    }
}
