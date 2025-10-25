namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInitSMUL
    {
        public static void Initialize()
        {
            Mnem.AddSubOp("SMUL r,nnn", Mnem.SMUL, "ooooooAA AAABBBBB BBBBBBBB", Mnem.DIVMUL_r_n);
            Mnem.AddSubOp("SMUL r,r", Mnem.SMUL, "ooooooAA AAABBBBB", Mnem.DIVMUL_r_r);

            Mnem.AddSubOp("SMUL rr,nnn", Mnem.SMUL, "ooooooAA AAABBBBB BBBBBBBB", Mnem.DIVMUL_rr_n);
            Mnem.AddSubOp("SMUL rr,r", Mnem.SMUL, "ooooooAA AAABBBBB", Mnem.DIVMUL_rr_r);
            Mnem.AddSubOp("SMUL rr,rr", Mnem.SMUL, "ooooooAA AAABBBBB", Mnem.DIVMUL_rr_rr);

            Mnem.AddSubOp("SMUL rrr,nnn", Mnem.SMUL, "ooooooAA AAABBBBB BBBBBBBB", Mnem.DIVMUL_rrr_n);
            Mnem.AddSubOp("SMUL rrr,r", Mnem.SMUL, "ooooooAA AAABBBBB", Mnem.DIVMUL_rrr_r);
            Mnem.AddSubOp("SMUL rrr,rr", Mnem.SMUL, "ooooooAA AAABBBBB", Mnem.DIVMUL_rrr_rr);
            Mnem.AddSubOp("SMUL rrr,rrr", Mnem.SMUL, "ooooooAA AAABBBBB", Mnem.DIVMUL_rrr_rrr);

            Mnem.AddSubOp("SMUL rrrr,nnn", Mnem.SMUL, "ooooooAA AAABBBBB BBBBBBBB", Mnem.DIVMUL_rrrr_n);
            Mnem.AddSubOp("SMUL rrrr,r", Mnem.SMUL, "ooooooAA AAABBBBB", Mnem.DIVMUL_rrrr_r);
            Mnem.AddSubOp("SMUL rrrr,rr", Mnem.SMUL, "ooooooAA AAABBBBB", Mnem.DIVMUL_rrrr_rr);
            Mnem.AddSubOp("SMUL rrrr,rrr", Mnem.SMUL, "ooooooAA AAABBBBB", Mnem.DIVMUL_rrrr_rrr);
            Mnem.AddSubOp("SMUL rrrr,rrrr", Mnem.SMUL, "ooooooAA AAABBBBB", Mnem.DIVMUL_rrrr_rrrr);
        }
    }
}
