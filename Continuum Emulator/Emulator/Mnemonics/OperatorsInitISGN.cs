namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInitISGN
    {
        public static void Initialize()
        {
            // Invert sign
            Mnem.AddSubOp("ISGN fr", Mnem.ISGN, "ooooAAAA", Mnem.SINCTC_fr);
            Mnem.AddSubOp("ISGN fr,fr", Mnem.ISGN, "ooooAAAA uuuuBBBB", Mnem.SINCTC_fr_fr);
        }
    }
}
