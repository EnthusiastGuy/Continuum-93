namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInitOR
    {
        public static void Initialize()
        {
            Mnem.AddSubOp("OR r,nnn", Mnem.OR, "oooooouu uuuAAAAA BBBBBBBB", Mnem.AOX_r_n);
            Mnem.AddSubOp("OR r,r", Mnem.OR, "ooooooAA AAABBBBB", Mnem.AOX_r_r);

            Mnem.AddSubOp("OR rr,nnn", Mnem.OR, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB", Mnem.AOX_rr_nn);
            Mnem.AddSubOp("OR rr,rr", Mnem.OR, "ooooooAA AAABBBBB", Mnem.AOX_rr_rr);

            Mnem.AddSubOp("OR rrr,nnn", Mnem.OR, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX_rrr_nnn);
            Mnem.AddSubOp("OR rrr,rrr", Mnem.OR, "ooooooAA AAABBBBB", Mnem.AOX_rrr_rrr);

            Mnem.AddSubOp("OR rrrr,nnn", Mnem.OR, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX_rrrr_nnnn);
            Mnem.AddSubOp("OR rrrr,rrrr", Mnem.OR, "ooooooAA AAABBBBB", Mnem.AOX_rrrr_rrrr);

            Mnem.AddSubOp("OR (rrr),nnn", Mnem.OR, "oooooouu uuuAAAAA BBBBBBBB", Mnem.AOX_IrrrI_n);
            Mnem.AddSubOp("OR16 (rrr),nnn", Mnem.OR16, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB", Mnem.AOX16_IrrrI_nn);
            Mnem.AddSubOp("OR24 (rrr),nnn", Mnem.OR24, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX24_IrrrI_nnn);
            Mnem.AddSubOp("OR32 (rrr),nnn", Mnem.OR32, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX32_IrrrI_nnnn);

            Mnem.AddSubOp("OR (rrr),r", Mnem.OR, "ooooooAA AAABBBBB", Mnem.AOX_IrrrI_r);
            Mnem.AddSubOp("OR (rrr),rr", Mnem.OR, "ooooooAA AAABBBBB", Mnem.AOX_IrrrI_rr);
            Mnem.AddSubOp("OR (rrr),rrr", Mnem.OR, "ooooooAA AAABBBBB", Mnem.AOX_IrrrI_rrr);
            Mnem.AddSubOp("OR (rrr),rrrr", Mnem.OR, "ooooooAA AAABBBBB", Mnem.AOX_IrrrI_rrrr);

            // new
            Mnem.AddSubOp("OR r,(rrr)", Mnem.OR, "ooooooAA AAABBBBB", Mnem.AOX_r_IrrrI);
            Mnem.AddSubOp("OR rr,(rrr)", Mnem.OR, "ooooooAA AAABBBBB", Mnem.AOX_rr_IrrrI);
            Mnem.AddSubOp("OR rrr,(rrr)", Mnem.OR, "ooooooAA AAABBBBB", Mnem.AOX_rrr_IrrrI);
            Mnem.AddSubOp("OR rrrr,(rrr)", Mnem.OR, "ooooooAA AAABBBBB", Mnem.AOX_rrrr_IrrrI);

            Mnem.AddSubOp("OR (nnn),nnn", Mnem.OR, "oooooouu AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB", Mnem.AOX_InnnI_n);
            Mnem.AddSubOp("OR16 (nnn),nnn", Mnem.OR16, "oooooouu AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB BBBBBBBB", Mnem.AOX16_InnnI_nn);
            Mnem.AddSubOp("OR24 (nnn),nnn", Mnem.OR24, "oooooouu AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX24_InnnI_nnn);
            Mnem.AddSubOp("OR32 (nnn),nnn", Mnem.OR32, "oooooouu AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX32_InnnI_nnnn);

            Mnem.AddSubOp("OR (nnn),r", Mnem.OR, "oooooouu AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.AOX_InnnI_r);
            Mnem.AddSubOp("OR (nnn),rr", Mnem.OR, "oooooouu AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.AOX_InnnI_rr);
            Mnem.AddSubOp("OR (nnn),rrr", Mnem.OR, "oooooouu AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.AOX_InnnI_rrr);
            Mnem.AddSubOp("OR (nnn),rrrr", Mnem.OR, "oooooouu AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.AOX_InnnI_rrrr);

            Mnem.AddSubOp("OR r,(nnn)", Mnem.OR, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX_r_InnnI);
            Mnem.AddSubOp("OR rr,(nnn)", Mnem.OR, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX_rr_InnnI);
            Mnem.AddSubOp("OR rrr,(nnn)", Mnem.OR, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX_rrr_InnnI);
            Mnem.AddSubOp("OR rrrr,(nnn)", Mnem.OR, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX_rrrr_InnnI);
        }
    }
}
