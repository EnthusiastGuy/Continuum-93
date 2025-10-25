namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInitSDIV
    {
        public static void Initialize()
        {
            Mnem.AddSubOp("SDIV r,nnn", Mnem.SDIV, "ooooooAA AAABBBBB BBBBBBBB", Mnem.DIVMUL_r_n);
            Mnem.AddSubOp("SDIV r,r", Mnem.SDIV, "ooooooAA AAABBBBB", Mnem.DIVMUL_r_r);

            Mnem.AddSubOp("SDIV rr,nnn", Mnem.SDIV, "ooooooAA AAABBBBB BBBBBBBB", Mnem.DIVMUL_rr_n);
            Mnem.AddSubOp("SDIV rr,r", Mnem.SDIV, "ooooooAA AAABBBBB", Mnem.DIVMUL_rr_r);
            Mnem.AddSubOp("SDIV rr,rr", Mnem.SDIV, "ooooooAA AAABBBBB", Mnem.DIVMUL_rr_rr);

            Mnem.AddSubOp("SDIV rrr,nnn", Mnem.SDIV, "ooooooAA AAABBBBB BBBBBBBB", Mnem.DIVMUL_rrr_n);
            Mnem.AddSubOp("SDIV rrr,r", Mnem.SDIV, "ooooooAA AAABBBBB", Mnem.DIVMUL_rrr_r);
            Mnem.AddSubOp("SDIV rrr,rr", Mnem.SDIV, "ooooooAA AAABBBBB", Mnem.DIVMUL_rrr_rr);
            Mnem.AddSubOp("SDIV rrr,rrr", Mnem.SDIV, "ooooooAA AAABBBBB", Mnem.DIVMUL_rrr_rrr);

            Mnem.AddSubOp("SDIV rrrr,nnn", Mnem.SDIV, "ooooooAA AAABBBBB BBBBBBBB", Mnem.DIVMUL_rrrr_n);
            Mnem.AddSubOp("SDIV rrrr,r", Mnem.SDIV, "ooooooAA AAABBBBB", Mnem.DIVMUL_rrrr_r);
            Mnem.AddSubOp("SDIV rrrr,rr", Mnem.SDIV, "ooooooAA AAABBBBB", Mnem.DIVMUL_rrrr_rr);
            Mnem.AddSubOp("SDIV rrrr,rrr", Mnem.SDIV, "ooooooAA AAABBBBB", Mnem.DIVMUL_rrrr_rrr);
            Mnem.AddSubOp("SDIV rrrr,rrrr", Mnem.SDIV, "ooooooAA AAABBBBB", Mnem.DIVMUL_rrrr_rrrr);

            Mnem.AddSubOp("SDIV r,nnn,r", Mnem.SDIV, "ooooooAA AAACCCCC BBBBBBBB", Mnem.DIV_r_n_r);
            Mnem.AddSubOp("SDIV r,r,r", Mnem.SDIV, "ooooooAA AAABBBBB 000CCCCC", Mnem.DIV_r_r_r);

            Mnem.AddSubOp("SDIV rr,nnn,rr", Mnem.SDIV, "ooooooAA AAACCCCC BBBBBBBB BBBBBBBB", Mnem.DIV_rr_n_rr);
            Mnem.AddSubOp("SDIV rr,r,r", Mnem.SDIV, "ooooooAA AAABBBBB 000CCCCC", Mnem.DIV_rr_r_r);
            Mnem.AddSubOp("SDIV rr,rr,rr", Mnem.SDIV, "ooooooAA AAABBBBB 000CCCCC", Mnem.DIV_rr_rr_rr);

            Mnem.AddSubOp("SDIV rrr,nnn,rr", Mnem.SDIV, "ooooooAA AAACCCCC BBBBBBBB BBBBBBBB", Mnem.DIV_rrr_n_rr);
            Mnem.AddSubOp("SDIV rrr,r,r", Mnem.SDIV, "ooooooAA AAABBBBB 000CCCCC", Mnem.DIV_rrr_r_r);
            Mnem.AddSubOp("SDIV rrr,rr,rr", Mnem.SDIV, "ooooooAA AAABBBBB 000CCCCC", Mnem.DIV_rrr_rr_rr);
            Mnem.AddSubOp("SDIV rrr,rrr,rrr", Mnem.SDIV, "ooooooAA AAABBBBB 000CCCCC", Mnem.DIV_rrr_rrr_rrr);

            Mnem.AddSubOp("SDIV rrrr,nnn,rr", Mnem.SDIV, "ooooooAA AAACCCCC BBBBBBBB BBBBBBBB", Mnem.DIV_rrrr_n_rr);
            Mnem.AddSubOp("SDIV rrrr,r,r", Mnem.SDIV, "ooooooAA AAABBBBB 000CCCCC", Mnem.DIV_rrrr_r_r);
            Mnem.AddSubOp("SDIV rrrr,rr,rr", Mnem.SDIV, "ooooooAA AAABBBBB 000CCCCC", Mnem.DIV_rrrr_rr_rr);
            Mnem.AddSubOp("SDIV rrrr,rrr,rrr", Mnem.SDIV, "ooooooAA AAABBBBB 000CCCCC", Mnem.DIV_rrrr_rrr_rrr);
            Mnem.AddSubOp("SDIV rrrr,rrrr,rrrr", Mnem.SDIV, "ooooooAA AAABBBBB 000CCCCC", Mnem.DIV_rrrr_rrrr_rrrr);
        }
    }
}
