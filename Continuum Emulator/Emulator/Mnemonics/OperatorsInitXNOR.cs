namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInitXNOR
    {
        public static void Initialize()
        {
            // XNOR
            Mnem.AddSubOp("XNOR r,nnn", Mnem.XNOR, "oooooouu uuuAAAAA BBBBBBBB", Mnem.AOX_r_n);
            Mnem.AddSubOp("XNOR r,r", Mnem.XNOR, "ooooooAA AAABBBBB", Mnem.AOX_r_r);

            Mnem.AddSubOp("XNOR rr,nnn", Mnem.XNOR, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB", Mnem.AOX_rr_nn);
            Mnem.AddSubOp("XNOR rr,rr", Mnem.XNOR, "ooooooAA AAABBBBB", Mnem.AOX_rr_rr);

            Mnem.AddSubOp("XNOR rrr,nnn", Mnem.XNOR, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX_rrr_nnn);
            Mnem.AddSubOp("XNOR rrr,rrr", Mnem.XNOR, "ooooooAA AAABBBBB", Mnem.AOX_rrr_rrr);

            Mnem.AddSubOp("XNOR rrrr,nnn", Mnem.XNOR, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX_rrrr_nnnn);
            Mnem.AddSubOp("XNOR rrrr,rrrr", Mnem.XNOR, "ooooooAA AAABBBBB", Mnem.AOX_rrrr_rrrr);

            Mnem.AddSubOp("XNOR (rrr),nnn", Mnem.XNOR, "oooooouu uuuAAAAA BBBBBBBB", Mnem.AOX_IrrrI_n);
            Mnem.AddSubOp("XNOR16 (rrr),nnn", Mnem.XNOR, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB", Mnem.AOX16_IrrrI_nn);
            Mnem.AddSubOp("XNOR24 (rrr),nnn", Mnem.XNOR, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX24_IrrrI_nnn);
            Mnem.AddSubOp("XNOR32 (rrr),nnn", Mnem.XNOR, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX32_IrrrI_nnnn);

            Mnem.AddSubOp("XNOR (rrr),r", Mnem.XNOR, "ooooooAA AAABBBBB", Mnem.AOX_IrrrI_r);
            Mnem.AddSubOp("XNOR (rrr),rr", Mnem.XNOR, "ooooooAA AAABBBBB", Mnem.AOX_IrrrI_rr);
            Mnem.AddSubOp("XNOR (rrr),rrr", Mnem.XNOR, "ooooooAA AAABBBBB", Mnem.AOX_IrrrI_rrr);
            Mnem.AddSubOp("XNOR (rrr),rrrr", Mnem.XNOR, "ooooooAA AAABBBBB", Mnem.AOX_IrrrI_rrrr);

            // new
            Mnem.AddSubOp("XNOR r,(rrr)", Mnem.XNOR, "ooooooAA AAABBBBB", Mnem.AOX_r_IrrrI);
            Mnem.AddSubOp("XNOR rr,(rrr)", Mnem.XNOR, "ooooooAA AAABBBBB", Mnem.AOX_rr_IrrrI);
            Mnem.AddSubOp("XNOR rrr,(rrr)", Mnem.XNOR, "ooooooAA AAABBBBB", Mnem.AOX_rrr_IrrrI);
            Mnem.AddSubOp("XNOR rrrr,(rrr)", Mnem.XNOR, "ooooooAA AAABBBBB", Mnem.AOX_rrrr_IrrrI);

            Mnem.AddSubOp("XNOR (nnn),nnn", Mnem.XNOR, "oooooouu AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB", Mnem.AOX_InnnI_n);
            Mnem.AddSubOp("XNOR16 (nnn),nnn", Mnem.XNOR, "oooooouu AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB BBBBBBBB", Mnem.AOX16_InnnI_nn);
            Mnem.AddSubOp("XNOR24 (nnn),nnn", Mnem.XNOR, "oooooouu AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX24_InnnI_nnn);
            Mnem.AddSubOp("XNOR32 (nnn),nnn", Mnem.XNOR, "oooooouu AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX32_InnnI_nnnn);

            Mnem.AddSubOp("XNOR (nnn),r", Mnem.XNOR, "oooooouu AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.AOX_InnnI_r);
            Mnem.AddSubOp("XNOR (nnn),rr", Mnem.XNOR, "oooooouu AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.AOX_InnnI_rr);
            Mnem.AddSubOp("XNOR (nnn),rrr", Mnem.XNOR, "oooooouu AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.AOX_InnnI_rrr);
            Mnem.AddSubOp("XNOR (nnn),rrrr", Mnem.XNOR, "oooooouu AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.AOX_InnnI_rrrr);

            Mnem.AddSubOp("XNOR r,(nnn)", Mnem.XNOR, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX_r_InnnI);
            Mnem.AddSubOp("XNOR rr,(nnn)", Mnem.XNOR, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX_rr_InnnI);
            Mnem.AddSubOp("XNOR rrr,(nnn)", Mnem.XNOR, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX_rrr_InnnI);
            Mnem.AddSubOp("XNOR rrrr,(nnn)", Mnem.XNOR, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX_rrrr_InnnI);
        }
    }
}
