namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInit_TRIGONOMETRY
    {
        public static void Initialize()
        {
            // SIN
            Mnem.AddSubOp("SIN fr", Mnem.SIN, "ooooAAAA", Mnem.SINCTC_fr);
            Mnem.AddSubOp("SIN fr,fr", Mnem.SIN, "ooooAAAA uuuuBBBB", Mnem.SINCTC_fr_fr);

            // COS
            Mnem.AddSubOp("COS fr", Mnem.COS, "ooooAAAA", Mnem.SINCTC_fr);
            Mnem.AddSubOp("COS fr,fr", Mnem.COS, "ooooAAAA uuuuBBBB", Mnem.SINCTC_fr_fr);

            // TAN
            Mnem.AddSubOp("TAN fr", Mnem.TAN, "ooooAAAA", Mnem.SINCTC_fr);
            Mnem.AddSubOp("TAN fr,fr", Mnem.TAN, "ooooAAAA uuuuBBBB", Mnem.SINCTC_fr_fr);
        }
    }
}
