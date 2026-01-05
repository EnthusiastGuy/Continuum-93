namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInitSHRL
    {
        public static void Initialize()
        {
            // SHIFT, ROLL
            // SL now uses the shared Instructions addressing matrix via GenericInitializer.
            // Legacy SL-specific sub-ops have been retired in favor of the 244-variant system.
            //Mnem.AddSubOp("SL r,nnn", Mnem.SL, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_r_n);
            //Mnem.AddSubOp("SL r,r", Mnem.SL, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_r_r);

            //Mnem.AddSubOp("SL rr,nnn", Mnem.SL, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_rr_n);
            //Mnem.AddSubOp("SL rr,r", Mnem.SL, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_rr_r);

            //Mnem.AddSubOp("SL rrr,nnn", Mnem.SL, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_rrr_n);
            //Mnem.AddSubOp("SL rrr,r", Mnem.SL, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_rrr_r);

            //Mnem.AddSubOp("SL rrrr,nnn", Mnem.SL, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_rrrr_n);
            //Mnem.AddSubOp("SL rrrr,r", Mnem.SL, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_rrrr_r);

            Mnem.AddSubOp("SR r,nnn", Mnem.SR, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_r_n);
            Mnem.AddSubOp("SR r,r", Mnem.SR, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_r_r);

            Mnem.AddSubOp("SR rr,nnn", Mnem.SR, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_rr_n);
            Mnem.AddSubOp("SR rr,r", Mnem.SR, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_rr_r);

            Mnem.AddSubOp("SR rrr,nnn", Mnem.SR, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_rrr_n);
            Mnem.AddSubOp("SR rrr,r", Mnem.SR, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_rrr_r);

            Mnem.AddSubOp("SR rrrr,nnn", Mnem.SR, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_rrrr_n);
            Mnem.AddSubOp("SR rrrr,r", Mnem.SR, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_rrrr_r);

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
