namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInitMIN_MAX
    {
        public static void Initialize()
        {
            Mnem.AddSubOp("MIN r,r", Mnem.MIN, "ooooooAA AAABBBBB", Mnem.MIAX_r_r);
            Mnem.AddSubOp("MIN rr,rr", Mnem.MIN, "ooooooAA AAABBBBB", Mnem.MIAX_rr_rr);
            Mnem.AddSubOp("MIN rrr,rrr", Mnem.MIN, "ooooooAA AAABBBBB", Mnem.MIAX_rrr_rrr);
            Mnem.AddSubOp("MIN rrrr,rrrr", Mnem.MIN, "ooooooAA AAABBBBB", Mnem.MIAX_rrrr_rrrr);

            Mnem.AddSubOp("MIN r,nnn", Mnem.MIN, "oooooouu uuuAAAAA BBBBBBBB", Mnem.MIAX_r_n);
            Mnem.AddSubOp("MIN rr,nnn", Mnem.MIN, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB", Mnem.MIAX_rr_nn);
            Mnem.AddSubOp("MIN rrr,nnn", Mnem.MIN, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.MIAX_rrr_nnn);
            Mnem.AddSubOp("MIN rrrr,nnn", Mnem.MIN, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.MIAX_rrrr_nnnn);

            // MIN with float
            Mnem.AddSubOp("MIN fr,nnn", Mnem.MIN, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.MIAX_fr_n);
            Mnem.AddSubOp("MIN fr,fr", Mnem.MIN, "oooooouu AAAABBBB", Mnem.MIAX_fr_fr);
            Mnem.AddSubOp("MIN r,fr", Mnem.MIN, "oooooouA AAAABBBB", Mnem.MIAX_r_fr);
            Mnem.AddSubOp("MIN rr,fr", Mnem.MIN, "oooooouA AAAABBBB", Mnem.MIAX_rr_fr);
            Mnem.AddSubOp("MIN rrr,fr", Mnem.MIN, "oooooouA AAAABBBB", Mnem.MIAX_rrr_fr);
            Mnem.AddSubOp("MIN rrrr,fr", Mnem.MIN, "oooooouA AAAABBBB", Mnem.MIAX_rrrr_fr);

            Mnem.AddSubOp("MIN fr,r", Mnem.MIN, "oooooouB BBBBAAAA", Mnem.MIAX_fr_r);
            Mnem.AddSubOp("MIN fr,rr", Mnem.MIN, "oooooouB BBBBAAAA", Mnem.MIAX_fr_rr);
            Mnem.AddSubOp("MIN fr,rrr", Mnem.MIN, "oooooouB BBBBAAAA", Mnem.MIAX_fr_rrr);
            Mnem.AddSubOp("MIN fr,rrrr", Mnem.MIN, "oooooouB BBBBAAAA", Mnem.MIAX_fr_rrrr);


            Mnem.AddSubOp("MAX r,r", Mnem.MAX, "ooooooAA AAABBBBB", Mnem.MIAX_r_r);
            Mnem.AddSubOp("MAX rr,rr", Mnem.MAX, "ooooooAA AAABBBBB", Mnem.MIAX_rr_rr);
            Mnem.AddSubOp("MAX rrr,rrr", Mnem.MAX, "ooooooAA AAABBBBB", Mnem.MIAX_rrr_rrr);
            Mnem.AddSubOp("MAX rrrr,rrrr", Mnem.MAX, "ooooooAA AAABBBBB", Mnem.MIAX_rrrr_rrrr);

            Mnem.AddSubOp("MAX r,nnn", Mnem.MAX, "oooooouu uuuAAAAA BBBBBBBB", Mnem.MIAX_r_n);
            Mnem.AddSubOp("MAX rr,nnn", Mnem.MAX, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB", Mnem.MIAX_rr_nn);
            Mnem.AddSubOp("MAX rrr,nnn", Mnem.MAX, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.MIAX_rrr_nnn);
            Mnem.AddSubOp("MAX rrrr,nnn", Mnem.MAX, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.MIAX_rrrr_nnnn);

            // MAX with float
            Mnem.AddSubOp("MAX fr,nnn", Mnem.MAX, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.MIAX_fr_n);
            Mnem.AddSubOp("MAX fr,fr", Mnem.MAX, "oooooouu AAAABBBB", Mnem.MIAX_fr_fr);
            Mnem.AddSubOp("MAX r,fr", Mnem.MAX, "oooooouA AAAABBBB", Mnem.MIAX_r_fr);
            Mnem.AddSubOp("MAX rr,fr", Mnem.MAX, "oooooouA AAAABBBB", Mnem.MIAX_rr_fr);
            Mnem.AddSubOp("MAX rrr,fr", Mnem.MAX, "oooooouA AAAABBBB", Mnem.MIAX_rrr_fr);
            Mnem.AddSubOp("MAX rrrr,fr", Mnem.MAX, "oooooouA AAAABBBB", Mnem.MIAX_rrrr_fr);

            Mnem.AddSubOp("MAX fr,r", Mnem.MAX, "oooooouB BBBBAAAA", Mnem.MIAX_fr_r);
            Mnem.AddSubOp("MAX fr,rr", Mnem.MAX, "oooooouB BBBBAAAA", Mnem.MIAX_fr_rr);
            Mnem.AddSubOp("MAX fr,rrr", Mnem.MAX, "oooooouB BBBBAAAA", Mnem.MIAX_fr_rrr);
            Mnem.AddSubOp("MAX fr,rrrr", Mnem.MAX, "oooooouB BBBBAAAA", Mnem.MIAX_fr_rrrr);
        }
    }
}
