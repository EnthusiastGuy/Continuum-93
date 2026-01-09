namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInitAND
    {
        public static void Initialize()
        {
            // AND
            //Mnem.AddSubOp("AND r,nnn", Mnem.AND, "oooooooo uuuAAAAA BBBBBBBB", Mnem.AOX_r_n);
            //Mnem.AddSubOp("AND r,r", Mnem.AND, "oooooooo uuuAAAAA uuuBBBBB", Mnem.AOX_r_r);

            //Mnem.AddSubOp("AND rr,nnn", Mnem.AND, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB", Mnem.AOX_rr_nn);
            //Mnem.AddSubOp("AND rr,rr", Mnem.AND, "oooooooo uuuAAAAA uuuBBBBB", Mnem.AOX_rr_rr);

            //Mnem.AddSubOp("AND rrr,nnn", Mnem.AND, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX_rrr_nnn);
            //Mnem.AddSubOp("AND rrr,rrr", Mnem.AND, "oooooooo uuuAAAAA uuuBBBBB", Mnem.AOX_rrr_rrr);

            //Mnem.AddSubOp("AND rrrr,nnn", Mnem.AND, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX_rrrr_nnnn);
            //Mnem.AddSubOp("AND rrrr,rrrr", Mnem.AND, "oooooooo uuuAAAAA uuuBBBBB", Mnem.AOX_rrrr_rrrr);

            //Mnem.AddSubOp("AND (rrr),nnn", Mnem.AND, "oooooooo uuuAAAAA BBBBBBBB", Mnem.AOX_IrrrI_n);
            //Mnem.AddSubOp("AND16 (rrr),nnn", Mnem.AND16, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB", Mnem.AOX16_IrrrI_nn);
            //Mnem.AddSubOp("AND24 (rrr),nnn", Mnem.AND24, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX24_IrrrI_nnn);
            //Mnem.AddSubOp("AND32 (rrr),nnn", Mnem.AND32, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX32_IrrrI_nnnn);

            //Mnem.AddSubOp("AND (rrr),r", Mnem.AND, "oooooooo uuuAAAAA uuuBBBBB", Mnem.AOX_IrrrI_r);
            //Mnem.AddSubOp("AND (rrr),rr", Mnem.AND, "oooooooo uuuAAAAA uuuBBBBB", Mnem.AOX_IrrrI_rr);
            //Mnem.AddSubOp("AND (rrr),rrr", Mnem.AND, "oooooooo uuuAAAAA uuuBBBBB", Mnem.AOX_IrrrI_rrr);
            //Mnem.AddSubOp("AND (rrr),rrrr", Mnem.AND, "oooooooo uuuAAAAA uuuBBBBB", Mnem.AOX_IrrrI_rrrr);

            //// new
            //Mnem.AddSubOp("AND r,(rrr)", Mnem.AND, "oooooooo uuuAAAAA uuuBBBBB", Mnem.AOX_r_IrrrI);
            //Mnem.AddSubOp("AND rr,(rrr)", Mnem.AND, "oooooooo uuuAAAAA uuuBBBBB", Mnem.AOX_rr_IrrrI);
            //Mnem.AddSubOp("AND rrr,(rrr)", Mnem.AND, "oooooooo uuuAAAAA uuuBBBBB", Mnem.AOX_rrr_IrrrI);
            //Mnem.AddSubOp("AND rrrr,(rrr)", Mnem.AND, "oooooooo uuuAAAAA uuuBBBBB", Mnem.AOX_rrrr_IrrrI);

            //Mnem.AddSubOp("AND (nnn),nnn", Mnem.AND, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB", Mnem.AOX_InnnI_n);
            //Mnem.AddSubOp("AND16 (nnn),nnn", Mnem.AND16, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB BBBBBBBB", Mnem.AOX16_InnnI_nn);
            //Mnem.AddSubOp("AND24 (nnn),nnn", Mnem.AND24, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX24_InnnI_nnn);
            //Mnem.AddSubOp("AND32 (nnn),nnn", Mnem.AND32, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX32_InnnI_nnnn);

            //Mnem.AddSubOp("AND (nnn),r", Mnem.AND, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.AOX_InnnI_r);
            //Mnem.AddSubOp("AND (nnn),rr", Mnem.AND, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.AOX_InnnI_rr);
            //Mnem.AddSubOp("AND (nnn),rrr", Mnem.AND, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.AOX_InnnI_rrr);
            //Mnem.AddSubOp("AND (nnn),rrrr", Mnem.AND, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.AOX_InnnI_rrrr);

            //Mnem.AddSubOp("AND r,(nnn)", Mnem.AND, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX_r_InnnI);
            //Mnem.AddSubOp("AND rr,(nnn)", Mnem.AND, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX_rr_InnnI);
            //Mnem.AddSubOp("AND rrr,(nnn)", Mnem.AND, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX_rrr_InnnI);
            //Mnem.AddSubOp("AND rrrr,(nnn)", Mnem.AND, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.AOX_rrrr_InnnI);
        }
    }
}
