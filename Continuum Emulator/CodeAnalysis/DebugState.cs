namespace Continuum93.CodeAnalysis
{
    public static class DebugState
    {
        public static volatile bool StepByStep = false;
        public static volatile bool MoveNext = false;

        public static volatile bool ClientConnected = false;
    }
}
