namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInitBIT
    {
        public static void Initialize()
        {
            // BIT
            Mnem.AddSubOp("BIT r,nnn", Mnem.BIT, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_r_n);
            Mnem.AddSubOp("BIT r,r", Mnem.BIT, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_r_r);

            Mnem.AddSubOp("BIT rr,nnn", Mnem.BIT, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_rr_n);
            Mnem.AddSubOp("BIT rr,r", Mnem.BIT, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_rr_r);

            Mnem.AddSubOp("BIT rrr,nnn", Mnem.BIT, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_rrr_n);
            Mnem.AddSubOp("BIT rrr,r", Mnem.BIT, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_rrr_r);

            Mnem.AddSubOp("BIT rrrr,nnn", Mnem.BIT, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_rrrr_n);
            Mnem.AddSubOp("BIT rrrr,r", Mnem.BIT, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_rrrr_r);
        }
    }
}
