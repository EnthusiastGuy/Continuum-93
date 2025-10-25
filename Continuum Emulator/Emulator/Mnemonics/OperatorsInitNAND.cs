namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInitNAND
    {
        public static void Initialize()
        {
            // NAND
            Mnem.AddSubOp("NAND r,nnn", Mnem.NAND, "oooooouu uuuAAAAA BBBBBBBB", Mnem.AOX_r_n);
            Mnem.AddSubOp("NAND r,r", Mnem.NAND, "ooooooAA AAABBBBB", Mnem.AOX_r_r);

            Mnem.AddSubOp("NAND rr,nnn", Mnem.NAND, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB", Mnem.AOX_rr_nn);
            Mnem.AddSubOp("NAND rr,rr", Mnem.NAND, "ooooooAA AAABBBBB", Mnem.AOX_rr_rr);

            Mnem.AddSubOp("NAND rrr,nnn", Mnem.NAND, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX_rrr_nnn);
            Mnem.AddSubOp("NAND rrr,rrr", Mnem.NAND, "ooooooAA AAABBBBB", Mnem.AOX_rrr_rrr);

            Mnem.AddSubOp("NAND rrrr,nnn", Mnem.NAND, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX_rrrr_nnnn);
            Mnem.AddSubOp("NAND rrrr,rrrr", Mnem.NAND, "ooooooAA AAABBBBB", Mnem.AOX_rrrr_rrrr);

            Mnem.AddSubOp("NAND (rrr),nnn", Mnem.NAND, "oooooouu uuuAAAAA BBBBBBBB", Mnem.AOX_IrrrI_n);
            Mnem.AddSubOp("NAND16 (rrr),nnn", Mnem.NAND, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB", Mnem.AOX16_IrrrI_nn);
            Mnem.AddSubOp("NAND24 (rrr),nnn", Mnem.NAND, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX24_IrrrI_nnn);
            Mnem.AddSubOp("NAND32 (rrr),nnn", Mnem.NAND, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX32_IrrrI_nnnn);

            Mnem.AddSubOp("NAND (rrr),r", Mnem.NAND, "ooooooAA AAABBBBB", Mnem.AOX_IrrrI_r);
            Mnem.AddSubOp("NAND (rrr),rr", Mnem.NAND, "ooooooAA AAABBBBB", Mnem.AOX_IrrrI_rr);
            Mnem.AddSubOp("NAND (rrr),rrr", Mnem.NAND, "ooooooAA AAABBBBB", Mnem.AOX_IrrrI_rrr);
            Mnem.AddSubOp("NAND (rrr),rrrr", Mnem.NAND, "ooooooAA AAABBBBB", Mnem.AOX_IrrrI_rrrr);

            // new
            Mnem.AddSubOp("NAND r,(rrr)", Mnem.NAND, "ooooooAA AAABBBBB", Mnem.AOX_r_IrrrI);
            Mnem.AddSubOp("NAND rr,(rrr)", Mnem.NAND, "ooooooAA AAABBBBB", Mnem.AOX_rr_IrrrI);
            Mnem.AddSubOp("NAND rrr,(rrr)", Mnem.NAND, "ooooooAA AAABBBBB", Mnem.AOX_rrr_IrrrI);
            Mnem.AddSubOp("NAND rrrr,(rrr)", Mnem.NAND, "ooooooAA AAABBBBB", Mnem.AOX_rrrr_IrrrI);

            Mnem.AddSubOp("NAND (nnn),nnn", Mnem.NAND, "oooooouu AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB", Mnem.AOX_InnnI_n);
            Mnem.AddSubOp("NAND16 (nnn),nnn", Mnem.NAND, "oooooouu AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB BBBBBBBB", Mnem.AOX16_InnnI_nn);
            Mnem.AddSubOp("NAND24 (nnn),nnn", Mnem.NAND, "oooooouu AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX24_InnnI_nnn);
            Mnem.AddSubOp("NAND32 (nnn),nnn", Mnem.NAND, "oooooouu AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX32_InnnI_nnnn);

            Mnem.AddSubOp("NAND (nnn),r", Mnem.NAND, "oooooouu AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.AOX_InnnI_r);
            Mnem.AddSubOp("NAND (nnn),rr", Mnem.NAND, "oooooouu AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.AOX_InnnI_rr);
            Mnem.AddSubOp("NAND (nnn),rrr", Mnem.NAND, "oooooouu AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.AOX_InnnI_rrr);
            Mnem.AddSubOp("NAND (nnn),rrrr", Mnem.NAND, "oooooouu AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.AOX_InnnI_rrrr);

            Mnem.AddSubOp("NAND r,(nnn)", Mnem.NAND, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX_r_InnnI);
            Mnem.AddSubOp("NAND rr,(nnn)", Mnem.NAND, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX_rr_InnnI);
            Mnem.AddSubOp("NAND rrr,(nnn)", Mnem.NAND, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX_rrr_InnnI);
            Mnem.AddSubOp("NAND rrrr,(nnn)", Mnem.NAND, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX_rrrr_InnnI);
        }
    }
}
