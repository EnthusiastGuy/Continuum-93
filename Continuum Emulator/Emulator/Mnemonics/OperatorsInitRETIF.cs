namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInitRETIF
    {
        public static void Initialize()
        {
            // RETIF
            Mnem.AddSubOp("RETIF ff", Mnem.RETIF, "ouuuAAAA", Mnem.RETIF_ff);
        }
    }
}
