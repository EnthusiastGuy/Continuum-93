namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInitDIV
    {
        public static void Initialize()
        {
            // DIV, MUL
            Mnem.AddSubOp("DIV r,nnn", Mnem.DIV, "ooooooAA AAABBBBB BBBBBBBB", Mnem.DIVMUL_r_n);
            Mnem.AddSubOp("DIV r,r", Mnem.DIV, "ooooooAA AAABBBBB", Mnem.DIVMUL_r_r);

            Mnem.AddSubOp("DIV rr,nnn", Mnem.DIV, "ooooooAA AAABBBBB BBBBBBBB", Mnem.DIVMUL_rr_n);
            Mnem.AddSubOp("DIV rr,r", Mnem.DIV, "ooooooAA AAABBBBB", Mnem.DIVMUL_rr_r);
            Mnem.AddSubOp("DIV rr,rr", Mnem.DIV, "ooooooAA AAABBBBB", Mnem.DIVMUL_rr_rr);

            Mnem.AddSubOp("DIV rrr,nnn", Mnem.DIV, "ooooooAA AAABBBBB BBBBBBBB", Mnem.DIVMUL_rrr_n);
            Mnem.AddSubOp("DIV rrr,r", Mnem.DIV, "ooooooAA AAABBBBB", Mnem.DIVMUL_rrr_r);
            Mnem.AddSubOp("DIV rrr,rr", Mnem.DIV, "ooooooAA AAABBBBB", Mnem.DIVMUL_rrr_rr);
            Mnem.AddSubOp("DIV rrr,rrr", Mnem.DIV, "ooooooAA AAABBBBB", Mnem.DIVMUL_rrr_rrr);

            Mnem.AddSubOp("DIV rrrr,nnn", Mnem.DIV, "ooooooAA AAABBBBB BBBBBBBB", Mnem.DIVMUL_rrrr_n);
            Mnem.AddSubOp("DIV rrrr,r", Mnem.DIV, "ooooooAA AAABBBBB", Mnem.DIVMUL_rrrr_r);
            Mnem.AddSubOp("DIV rrrr,rr", Mnem.DIV, "ooooooAA AAABBBBB", Mnem.DIVMUL_rrrr_rr);
            Mnem.AddSubOp("DIV rrrr,rrr", Mnem.DIV, "ooooooAA AAABBBBB", Mnem.DIVMUL_rrrr_rrr);
            Mnem.AddSubOp("DIV rrrr,rrrr", Mnem.DIV, "ooooooAA AAABBBBB", Mnem.DIVMUL_rrrr_rrrr);


            Mnem.AddSubOp("DIV r,nnn,r", Mnem.DIV, "ooooooAA AAACCCCC BBBBBBBB", Mnem.DIV_r_n_r);
            Mnem.AddSubOp("DIV r,r,r", Mnem.DIV, "ooooooAA AAABBBBB 000CCCCC", Mnem.DIV_r_r_r);

            Mnem.AddSubOp("DIV rr,nnn,rr", Mnem.DIV, "ooooooAA AAACCCCC BBBBBBBB BBBBBBBB", Mnem.DIV_rr_n_rr);
            Mnem.AddSubOp("DIV rr,r,r", Mnem.DIV, "ooooooAA AAABBBBB 000CCCCC", Mnem.DIV_rr_r_r);
            Mnem.AddSubOp("DIV rr,rr,rr", Mnem.DIV, "ooooooAA AAABBBBB 000CCCCC", Mnem.DIV_rr_rr_rr);

            Mnem.AddSubOp("DIV rrr,nnn,rr", Mnem.DIV, "ooooooAA AAACCCCC BBBBBBBB BBBBBBBB", Mnem.DIV_rrr_n_rr);
            Mnem.AddSubOp("DIV rrr,r,r", Mnem.DIV, "ooooooAA AAABBBBB 000CCCCC", Mnem.DIV_rrr_r_r);
            Mnem.AddSubOp("DIV rrr,rr,rr", Mnem.DIV, "ooooooAA AAABBBBB 000CCCCC", Mnem.DIV_rrr_rr_rr);
            Mnem.AddSubOp("DIV rrr,rrr,rrr", Mnem.DIV, "ooooooAA AAABBBBB 000CCCCC", Mnem.DIV_rrr_rrr_rrr);

            Mnem.AddSubOp("DIV rrrr,nnn,rr", Mnem.DIV, "ooooooAA AAACCCCC BBBBBBBB BBBBBBBB", Mnem.DIV_rrrr_n_rr);
            Mnem.AddSubOp("DIV rrrr,r,r", Mnem.DIV, "ooooooAA AAABBBBB 000CCCCC", Mnem.DIV_rrrr_r_r);
            Mnem.AddSubOp("DIV rrrr,rr,rr", Mnem.DIV, "ooooooAA AAABBBBB 000CCCCC", Mnem.DIV_rrrr_rr_rr);
            Mnem.AddSubOp("DIV rrrr,rrr,rrr", Mnem.DIV, "ooooooAA AAABBBBB 000CCCCC", Mnem.DIV_rrrr_rrr_rrr);
            Mnem.AddSubOp("DIV rrrr,rrrr,rrrr", Mnem.DIV, "ooooooAA AAABBBBB 000CCCCC", Mnem.DIV_rrrr_rrrr_rrrr);

            // Floating point
            Mnem.AddSubOp("DIV fr,fr", Mnem.DIV, "oooooouu AAAABBBB", Mnem.DIVMUL_fr_fr);
            Mnem.AddSubOp("DIV fr,nnn", Mnem.DIV, "oooooouu uuuuAAAA BBBBBBBB BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.DIVMUL_fr_nnn);

            Mnem.AddSubOp("DIV fr,r", Mnem.DIV, "oooooouu uuuuAAAA uuuBBBBB", Mnem.DIVMUL_fr_r);
            Mnem.AddSubOp("DIV fr,rr", Mnem.DIV, "oooooouu uuuuAAAA uuuBBBBB", Mnem.DIVMUL_fr_rr);
            Mnem.AddSubOp("DIV fr,rrr", Mnem.DIV, "oooooouu uuuuAAAA uuuBBBBB", Mnem.DIVMUL_fr_rrr);
            Mnem.AddSubOp("DIV fr,rrrr", Mnem.DIV, "oooooouu uuuuAAAA uuuBBBBB", Mnem.DIVMUL_fr_rrrr);

            Mnem.AddSubOp("DIV r,fr", Mnem.DIV, "oooooouu uuuAAAAA uuuuBBBB", Mnem.DIVMUL_r_fr);
            Mnem.AddSubOp("DIV rr,fr", Mnem.DIV, "oooooouu uuuAAAAA uuuuBBBB", Mnem.DIVMUL_rr_fr);
            Mnem.AddSubOp("DIV rrr,fr", Mnem.DIV, "oooooouu uuuAAAAA uuuuBBBB", Mnem.DIVMUL_rrr_fr);
            Mnem.AddSubOp("DIV rrrr,fr", Mnem.DIV, "oooooouu uuuAAAAA uuuuBBBB", Mnem.DIVMUL_rrrr_fr);

            Mnem.AddSubOp("DIV fr,(nnn)", Mnem.DIV, "oooooouu uuuuAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.DIVMUL_fr_InnnI);
            Mnem.AddSubOp("DIV fr,(rrr)", Mnem.DIV, "ooooooAA AAuBBBBB", Mnem.DIVMUL_fr_IrrrI);
            Mnem.AddSubOp("DIV (nnn),fr", Mnem.DIV, "oooooouu uuuuBBBB AAAAAAAA AAAAAAAA AAAAAAAA", Mnem.DIVMUL_InnnI_fr);
            Mnem.AddSubOp("DIV (rrr),fr", Mnem.DIV, "ooooooBB BBuAAAAA", Mnem.DIVMUL_IrrrI_fr);
        }
    }
}
