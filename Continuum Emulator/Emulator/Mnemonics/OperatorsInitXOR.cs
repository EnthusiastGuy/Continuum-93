namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInitXOR
    {
        public static void Initialize()
        {
            Mnem.AddSubOp("XOR r,nnn", Mnem.XOR, "oooooouu uuuAAAAA BBBBBBBB", Mnem.AOX_r_n);
            Mnem.AddSubOp("XOR r,r", Mnem.XOR, "ooooooAA AAABBBBB", Mnem.AOX_r_r);

            Mnem.AddSubOp("XOR rr,nnn", Mnem.XOR, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB", Mnem.AOX_rr_nn);
            Mnem.AddSubOp("XOR rr,rr", Mnem.XOR, "ooooooAA AAABBBBB", Mnem.AOX_rr_rr);

            Mnem.AddSubOp("XOR rrr,nnn", Mnem.XOR, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX_rrr_nnn);
            Mnem.AddSubOp("XOR rrr,rrr", Mnem.XOR, "ooooooAA AAABBBBB", Mnem.AOX_rrr_rrr);

            Mnem.AddSubOp("XOR rrrr,nnn", Mnem.XOR, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX_rrrr_nnnn);
            Mnem.AddSubOp("XOR rrrr,rrrr", Mnem.XOR, "ooooooAA AAABBBBB", Mnem.AOX_rrrr_rrrr);

            Mnem.AddSubOp("XOR (rrr),nnn", Mnem.XOR, "oooooouu uuuAAAAA BBBBBBBB", Mnem.AOX_IrrrI_n);
            Mnem.AddSubOp("XOR16 (rrr),nnn", Mnem.XOR16, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB", Mnem.AOX16_IrrrI_nn);
            Mnem.AddSubOp("XOR24 (rrr),nnn", Mnem.XOR24, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX24_IrrrI_nnn);
            Mnem.AddSubOp("XOR32 (rrr),nnn", Mnem.XOR32, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX32_IrrrI_nnnn);

            Mnem.AddSubOp("XOR (rrr),r", Mnem.XOR, "ooooooAA AAABBBBB", Mnem.AOX_IrrrI_r);
            Mnem.AddSubOp("XOR (rrr),rr", Mnem.XOR, "ooooooAA AAABBBBB", Mnem.AOX_IrrrI_rr);
            Mnem.AddSubOp("XOR (rrr),rrr", Mnem.XOR, "ooooooAA AAABBBBB", Mnem.AOX_IrrrI_rrr);
            Mnem.AddSubOp("XOR (rrr),rrrr", Mnem.XOR, "ooooooAA AAABBBBB", Mnem.AOX_IrrrI_rrrr);

            // new
            Mnem.AddSubOp("XOR r,(rrr)", Mnem.XOR, "ooooooAA AAABBBBB", Mnem.AOX_r_IrrrI);
            Mnem.AddSubOp("XOR rr,(rrr)", Mnem.XOR, "ooooooAA AAABBBBB", Mnem.AOX_rr_IrrrI);
            Mnem.AddSubOp("XOR rrr,(rrr)", Mnem.XOR, "ooooooAA AAABBBBB", Mnem.AOX_rrr_IrrrI);
            Mnem.AddSubOp("XOR rrrr,(rrr)", Mnem.XOR, "ooooooAA AAABBBBB", Mnem.AOX_rrrr_IrrrI);

            Mnem.AddSubOp("XOR (nnn),nnn", Mnem.XOR, "oooooouu AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB", Mnem.AOX_InnnI_n);
            Mnem.AddSubOp("XOR16 (nnn),nnn", Mnem.XOR16, "oooooouu AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB BBBBBBBB", Mnem.AOX16_InnnI_nn);
            Mnem.AddSubOp("XOR24 (nnn),nnn", Mnem.XOR24, "oooooouu AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX24_InnnI_nnn);
            Mnem.AddSubOp("XOR32 (nnn),nnn", Mnem.XOR32, "oooooouu AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX32_InnnI_nnnn);

            Mnem.AddSubOp("XOR (nnn),r", Mnem.XOR, "oooooouu AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.AOX_InnnI_r);
            Mnem.AddSubOp("XOR (nnn),rr", Mnem.XOR, "oooooouu AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.AOX_InnnI_rr);
            Mnem.AddSubOp("XOR (nnn),rrr", Mnem.XOR, "oooooouu AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.AOX_InnnI_rrr);
            Mnem.AddSubOp("XOR (nnn),rrrr", Mnem.XOR, "oooooouu AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.AOX_InnnI_rrrr);

            Mnem.AddSubOp("XOR r,(nnn)", Mnem.XOR, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX_r_InnnI);
            Mnem.AddSubOp("XOR rr,(nnn)", Mnem.XOR, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX_rr_InnnI);
            Mnem.AddSubOp("XOR rrr,(nnn)", Mnem.XOR, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX_rrr_InnnI);
            Mnem.AddSubOp("XOR rrrr,(nnn)", Mnem.XOR, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX_rrrr_InnnI);
        }
    }
}
