namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInitSETRES
    {
        public static void Initialize()
        {
            // SET, Mnem.RESET
            Mnem.AddSubOp("SET r,nnn", Mnem.SET, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_r_n);
            Mnem.AddSubOp("SET r,r", Mnem.SET, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_r_r);

            Mnem.AddSubOp("SET rr,nnn", Mnem.SET, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_rr_n);
            Mnem.AddSubOp("SET rr,r", Mnem.SET, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_rr_r);

            Mnem.AddSubOp("SET rrr,nnn", Mnem.SET, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_rrr_n);
            Mnem.AddSubOp("SET rrr,r", Mnem.SET, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_rrr_r);

            Mnem.AddSubOp("SET rrrr,nnn", Mnem.SET, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_rrrr_n);
            Mnem.AddSubOp("SET rrrr,r", Mnem.SET, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_rrrr_r);

            Mnem.AddSubOp("RES r,nnn", Mnem.RES, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_r_n);
            Mnem.AddSubOp("RES r,r", Mnem.RES, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_r_r);

            Mnem.AddSubOp("RES rr,nnn", Mnem.RES, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_rr_n);
            Mnem.AddSubOp("RES rr,r", Mnem.RES, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_rr_r);

            Mnem.AddSubOp("RES rrr,nnn", Mnem.RES, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_rrr_n);
            Mnem.AddSubOp("RES rrr,r", Mnem.RES, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_rrr_r);

            Mnem.AddSubOp("RES rrrr,nnn", Mnem.RES, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_rrrr_n);
            Mnem.AddSubOp("RES rrrr,r", Mnem.RES, "ooooooAA AAABBBBB", Mnem.SHRLSTRE_rrrr_r);
        }
    }
}
