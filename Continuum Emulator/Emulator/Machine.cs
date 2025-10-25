namespace Continuum93.Emulator
{
    public static class Machine
    {
        public static Computer COMPUTER;

        public static void InitializeComputer()
        {
            COMPUTER?.Dispose();
            COMPUTER = new Computer(initWithAudio: true);
        }
    }
}
