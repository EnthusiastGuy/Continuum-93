namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInitDIV
    {
        public static void Initialize()
        {
            // DIV now uses the shared Instructions addressing matrix via GenericInitializer.
            // Legacy DIV-specific sub-ops (including 3-operand remainder variants) have been
            // retired; a dedicated remainder instruction will handle that separately.
        }
    }
}
