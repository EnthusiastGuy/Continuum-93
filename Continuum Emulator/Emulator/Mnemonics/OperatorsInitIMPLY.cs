namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInitIMPLY
    {
        public static void Initialize()
        {
            // IMPLY
            Mnem.AddSubOp("IMPLY r,nnn", Mnem.IMPLY, "oooooouu uuuAAAAA BBBBBBBB", Mnem.AOX_r_n);
            Mnem.AddSubOp("IMPLY r,r", Mnem.IMPLY, "ooooooAA AAABBBBB", Mnem.AOX_r_r);

            Mnem.AddSubOp("IMPLY rr,nnn", Mnem.IMPLY, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB", Mnem.AOX_rr_nn);
            Mnem.AddSubOp("IMPLY rr,rr", Mnem.IMPLY, "ooooooAA AAABBBBB", Mnem.AOX_rr_rr);

            Mnem.AddSubOp("IMPLY rrr,nnn", Mnem.IMPLY, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX_rrr_nnn);
            Mnem.AddSubOp("IMPLY rrr,rrr", Mnem.IMPLY, "ooooooAA AAABBBBB", Mnem.AOX_rrr_rrr);

            Mnem.AddSubOp("IMPLY rrrr,nnn", Mnem.IMPLY, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX_rrrr_nnnn);
            Mnem.AddSubOp("IMPLY rrrr,rrrr", Mnem.IMPLY, "ooooooAA AAABBBBB", Mnem.AOX_rrrr_rrrr);

            Mnem.AddSubOp("IMPLY (rrr),nnn", Mnem.IMPLY, "oooooouu uuuAAAAA BBBBBBBB", Mnem.AOX_IrrrI_n);
            Mnem.AddSubOp("IMPLY16 (rrr),nnn", Mnem.IMPLY, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB", Mnem.AOX16_IrrrI_nn);
            Mnem.AddSubOp("IMPLY24 (rrr),nnn", Mnem.IMPLY, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX24_IrrrI_nnn);
            Mnem.AddSubOp("IMPLY32 (rrr),nnn", Mnem.IMPLY, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX32_IrrrI_nnnn);

            Mnem.AddSubOp("IMPLY (rrr),r", Mnem.IMPLY, "ooooooAA AAABBBBB", Mnem.AOX_IrrrI_r);
            Mnem.AddSubOp("IMPLY (rrr),rr", Mnem.IMPLY, "ooooooAA AAABBBBB", Mnem.AOX_IrrrI_rr);
            Mnem.AddSubOp("IMPLY (rrr),rrr", Mnem.IMPLY, "ooooooAA AAABBBBB", Mnem.AOX_IrrrI_rrr);
            Mnem.AddSubOp("IMPLY (rrr),rrrr", Mnem.IMPLY, "ooooooAA AAABBBBB", Mnem.AOX_IrrrI_rrrr);

            // new
            Mnem.AddSubOp("IMPLY r,(rrr)", Mnem.IMPLY, "ooooooAA AAABBBBB", Mnem.AOX_r_IrrrI);
            Mnem.AddSubOp("IMPLY rr,(rrr)", Mnem.IMPLY, "ooooooAA AAABBBBB", Mnem.AOX_rr_IrrrI);
            Mnem.AddSubOp("IMPLY rrr,(rrr)", Mnem.IMPLY, "ooooooAA AAABBBBB", Mnem.AOX_rrr_IrrrI);
            Mnem.AddSubOp("IMPLY rrrr,(rrr)", Mnem.IMPLY, "ooooooAA AAABBBBB", Mnem.AOX_rrrr_IrrrI);

            Mnem.AddSubOp("IMPLY (nnn),nnn", Mnem.IMPLY, "oooooouu AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB", Mnem.AOX_InnnI_n);
            Mnem.AddSubOp("IMPLY16 (nnn),nnn", Mnem.IMPLY, "oooooouu AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB BBBBBBBB", Mnem.AOX16_InnnI_nn);
            Mnem.AddSubOp("IMPLY24 (nnn),nnn", Mnem.IMPLY, "oooooouu AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX24_InnnI_nnn);
            Mnem.AddSubOp("IMPLY32 (nnn),nnn", Mnem.IMPLY, "oooooouu AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX32_InnnI_nnnn);

            Mnem.AddSubOp("IMPLY (nnn),r", Mnem.IMPLY, "oooooouu AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.AOX_InnnI_r);
            Mnem.AddSubOp("IMPLY (nnn),rr", Mnem.IMPLY, "oooooouu AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.AOX_InnnI_rr);
            Mnem.AddSubOp("IMPLY (nnn),rrr", Mnem.IMPLY, "oooooouu AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.AOX_InnnI_rrr);
            Mnem.AddSubOp("IMPLY (nnn),rrrr", Mnem.IMPLY, "oooooouu AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.AOX_InnnI_rrrr);

            Mnem.AddSubOp("IMPLY r,(nnn)", Mnem.IMPLY, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX_r_InnnI);
            Mnem.AddSubOp("IMPLY rr,(nnn)", Mnem.IMPLY, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX_rr_InnnI);
            Mnem.AddSubOp("IMPLY rrr,(nnn)", Mnem.IMPLY, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX_rrr_InnnI);
            Mnem.AddSubOp("IMPLY rrrr,(nnn)", Mnem.IMPLY, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX_rrrr_InnnI);

        }
    }
}
