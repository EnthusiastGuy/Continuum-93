namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInitMUL
    {
        public static void Initialize()
        {
            Mnem.AddSubOp("MUL r,nnn", Mnem.MUL, "ooooooAA AAABBBBB BBBBBBBB", Mnem.DIVMUL_r_n);
            Mnem.AddSubOp("MUL r,r", Mnem.MUL, "ooooooAA AAABBBBB", Mnem.DIVMUL_r_r);

            Mnem.AddSubOp("MUL rr,nnn", Mnem.MUL, "ooooooAA AAABBBBB BBBBBBBB", Mnem.DIVMUL_rr_n);
            Mnem.AddSubOp("MUL rr,r", Mnem.MUL, "ooooooAA AAABBBBB", Mnem.DIVMUL_rr_r);
            Mnem.AddSubOp("MUL rr,rr", Mnem.MUL, "ooooooAA AAABBBBB", Mnem.DIVMUL_rr_rr);

            Mnem.AddSubOp("MUL rrr,nnn", Mnem.MUL, "ooooooAA AAABBBBB BBBBBBBB", Mnem.DIVMUL_rrr_n);
            Mnem.AddSubOp("MUL rrr,r", Mnem.MUL, "ooooooAA AAABBBBB", Mnem.DIVMUL_rrr_r);
            Mnem.AddSubOp("MUL rrr,rr", Mnem.MUL, "ooooooAA AAABBBBB", Mnem.DIVMUL_rrr_rr);
            Mnem.AddSubOp("MUL rrr,rrr", Mnem.MUL, "ooooooAA AAABBBBB", Mnem.DIVMUL_rrr_rrr);

            Mnem.AddSubOp("MUL rrrr,nnn", Mnem.MUL, "ooooooAA AAABBBBB BBBBBBBB", Mnem.DIVMUL_rrrr_n);
            Mnem.AddSubOp("MUL rrrr,r", Mnem.MUL, "ooooooAA AAABBBBB", Mnem.DIVMUL_rrrr_r);
            Mnem.AddSubOp("MUL rrrr,rr", Mnem.MUL, "ooooooAA AAABBBBB", Mnem.DIVMUL_rrrr_rr);
            Mnem.AddSubOp("MUL rrrr,rrr", Mnem.MUL, "ooooooAA AAABBBBB", Mnem.DIVMUL_rrrr_rrr);
            Mnem.AddSubOp("MUL rrrr,rrrr", Mnem.MUL, "ooooooAA AAABBBBB", Mnem.DIVMUL_rrrr_rrrr);

            // Floating point
            Mnem.AddSubOp("MUL fr,fr", Mnem.MUL, "oooooouu AAAABBBB", Mnem.DIVMUL_fr_fr);
            Mnem.AddSubOp("MUL fr,nnn", Mnem.MUL, "oooooouu uuuuAAAA BBBBBBBB BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.DIVMUL_fr_nnn);

            Mnem.AddSubOp("MUL fr,r", Mnem.MUL, "oooooouu uuuuAAAA uuuBBBBB", Mnem.DIVMUL_fr_r);
            Mnem.AddSubOp("MUL fr,rr", Mnem.MUL, "oooooouu uuuuAAAA uuuBBBBB", Mnem.DIVMUL_fr_rr);
            Mnem.AddSubOp("MUL fr,rrr", Mnem.MUL, "oooooouu uuuuAAAA uuuBBBBB", Mnem.DIVMUL_fr_rrr);
            Mnem.AddSubOp("MUL fr,rrrr", Mnem.MUL, "oooooouu uuuuAAAA uuuBBBBB", Mnem.DIVMUL_fr_rrrr);

            Mnem.AddSubOp("MUL r,fr", Mnem.MUL, "oooooouu uuuAAAAA uuuuBBBB", Mnem.DIVMUL_r_fr);
            Mnem.AddSubOp("MUL rr,fr", Mnem.MUL, "oooooouu uuuAAAAA uuuuBBBB", Mnem.DIVMUL_rr_fr);
            Mnem.AddSubOp("MUL rrr,fr", Mnem.MUL, "oooooouu uuuAAAAA uuuuBBBB", Mnem.DIVMUL_rrr_fr);
            Mnem.AddSubOp("MUL rrrr,fr", Mnem.MUL, "oooooouu uuuAAAAA uuuuBBBB", Mnem.DIVMUL_rrrr_fr);

            Mnem.AddSubOp("MUL fr,(nnn)", Mnem.MUL, "oooooouu uuuuAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.DIVMUL_fr_InnnI);
            Mnem.AddSubOp("MUL fr,(rrr)", Mnem.MUL, "ooooooAA AAuBBBBB", Mnem.DIVMUL_fr_IrrrI);
            Mnem.AddSubOp("MUL (nnn),fr", Mnem.MUL, "oooooouu uuuuBBBB AAAAAAAA AAAAAAAA AAAAAAAA", Mnem.DIVMUL_InnnI_fr);
            Mnem.AddSubOp("MUL (rrr),fr", Mnem.MUL, "ooooooBB BBuAAAAA", Mnem.DIVMUL_IrrrI_fr);
        }
    }
}
