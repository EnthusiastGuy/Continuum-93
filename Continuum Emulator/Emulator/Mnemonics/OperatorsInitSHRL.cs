namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInitSHRL
    {
        public static void Initialize()
        {
            // SHIFT, ROLL

            // SL and SR now uses the shared Instructions addressing matrix via GenericInitializer.
            

            Mnem.AddSubOp("RL r,nnn", Mnem.RL, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_r_n);
            Mnem.AddSubOp("RL r,r", Mnem.RL, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_r_r);

            Mnem.AddSubOp("RL rr,nnn", Mnem.RL, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_rr_n);
            Mnem.AddSubOp("RL rr,r", Mnem.RL, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_rr_r);

            Mnem.AddSubOp("RL rrr,nnn", Mnem.RL, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_rrr_n);
            Mnem.AddSubOp("RL rrr,r", Mnem.RL, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_rrr_r);

            Mnem.AddSubOp("RL rrrr,nnn", Mnem.RL, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_rrrr_n);
            Mnem.AddSubOp("RL rrrr,r", Mnem.RL, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_rrrr_r);

            Mnem.AddSubOp("RR r,nnn", Mnem.RR, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_r_n);
            Mnem.AddSubOp("RR r,r", Mnem.RR, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_r_r);

            Mnem.AddSubOp("RR rr,nnn", Mnem.RR, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_rr_n);
            Mnem.AddSubOp("RR rr,r", Mnem.RR, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_rr_r);

            Mnem.AddSubOp("RR rrr,nnn", Mnem.RR, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_rrr_n);
            Mnem.AddSubOp("RR rrr,r", Mnem.RR, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_rrr_r);

            Mnem.AddSubOp("RR rrrr,nnn", Mnem.RR, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_rrrr_n);
            Mnem.AddSubOp("RR rrrr,r", Mnem.RR, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_rrrr_r);
        }
    }
}
