namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInitCP
    {
        public static void Initialize()
        {
            // CP
            Mnem.AddSubOp("CP r,nnn", Mnem.CP, "oooooouu uuuAAAAA BBBBBBBB", Mnem.CP_r_n);
            Mnem.AddSubOp("CP r,r", Mnem.CP, "ooooooAA AAABBBBB", Mnem.CP_r_r);
            Mnem.AddSubOp("CP rr,nnn", Mnem.CP, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB", Mnem.CP_rr_nn);
            Mnem.AddSubOp("CP rr,rr", Mnem.CP, "ooooooAA AAABBBBB", Mnem.CP_rr_rr);
            Mnem.AddSubOp("CP rrr,nnn", Mnem.CP, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.CP_rrr_nnn);
            Mnem.AddSubOp("CP rrr,rrr", Mnem.CP, "ooooooAA AAABBBBB", Mnem.CP_rrr_rrr);
            Mnem.AddSubOp("CP rrrr,nnn", Mnem.CP, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.CP_rrrr_nnnn);
            Mnem.AddSubOp("CP rrrr,rrrr", Mnem.CP, "ooooooAA AAABBBBB", Mnem.CP_rrrr_rrrr);

            Mnem.AddSubOp("CP r,(rrr)", Mnem.CP, "ooooooAA AAABBBBB", Mnem.CP_r_IrrrI);
            Mnem.AddSubOp("CP rr,(rrr)", Mnem.CP, "ooooooAA AAABBBBB", Mnem.CP_rr_IrrrI);
            Mnem.AddSubOp("CP rrr,(rrr)", Mnem.CP, "ooooooAA AAABBBBB", Mnem.CP_rrr_IrrrI);
            Mnem.AddSubOp("CP rrrr,(rrr)", Mnem.CP, "ooooooAA AAABBBBB", Mnem.CP_rrrr_IrrrI);
            Mnem.AddSubOp("CP (rrr),(rrr)", Mnem.CP, "ooooooAA AAABBBBB", Mnem.CP_IrrrI_IrrrI);
            Mnem.AddSubOp("CP (rrr),nnn", Mnem.CP, "oooooouu uuuAAAAA BBBBBBBB", Mnem.CP_IrrrI_nnn);

            // Floating point
            Mnem.AddSubOp("CP fr,fr", Mnem.CP, "oooooouu AAAABBBB", Mnem.CP_fr_fr);
            Mnem.AddSubOp("CP fr,nnn", Mnem.CP, "oooooouu uuuuAAAA BBBBBBBB BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.CP_fr_nnn);

            Mnem.AddSubOp("CP fr,r", Mnem.CP, "oooooouu uuuuAAAA uuuBBBBB", Mnem.CP_fr_r);
            Mnem.AddSubOp("CP fr,rr", Mnem.CP, "oooooouu uuuuAAAA uuuBBBBB", Mnem.CP_fr_rr);
            Mnem.AddSubOp("CP fr,rrr", Mnem.CP, "oooooouu uuuuAAAA uuuBBBBB", Mnem.CP_fr_rrr);
            Mnem.AddSubOp("CP fr,rrrr", Mnem.CP, "oooooouu uuuuAAAA uuuBBBBB", Mnem.CP_fr_rrrr);

            Mnem.AddSubOp("CP r,fr", Mnem.CP, "oooooouu uuuAAAAA uuuuBBBB", Mnem.CP_r_fr);
            Mnem.AddSubOp("CP rr,fr", Mnem.CP, "oooooouu uuuAAAAA uuuuBBBB", Mnem.CP_rr_fr);
            Mnem.AddSubOp("CP rrr,fr", Mnem.CP, "oooooouu uuuAAAAA uuuuBBBB", Mnem.CP_rrr_fr);
            Mnem.AddSubOp("CP rrrr,fr", Mnem.CP, "oooooouu uuuAAAAA uuuuBBBB", Mnem.CP_rrrr_fr);

            Mnem.AddSubOp("CP fr,(nnn)", Mnem.CP, "oooooouu uuuuAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.CP_fr_InnnI);
            Mnem.AddSubOp("CP fr,(rrr)", Mnem.CP, "ooooooAA AAuBBBBB", Mnem.CP_fr_IrrrI);
            Mnem.AddSubOp("CP (nnn),fr", Mnem.CP, "oooooouu uuuuBBBB AAAAAAAA AAAAAAAA AAAAAAAA", Mnem.CP_InnnI_fr);
            Mnem.AddSubOp("CP (rrr),fr", Mnem.CP, "ooooooBB BBuAAAAA", Mnem.CP_IrrrI_fr);
        }
    }
}
