namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInitSUB
    {
        public static void Initialize()
        {
            // SUB now uses the shared Instructions addressing matrix via GenericInitializer.
            // Legacy ADDSUB-specific sub-ops have been retired to keep encoding aligned
            // with ExSUB/ExADD. If additional SUB-specific aliases are needed, add them here.
        }
    }
}
