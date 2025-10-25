namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInitADD
    {
        public static void Initialize()
        {
            // ADD
            Mnem.AddSubOp("ADD r,nnn", Mnem.ADD, "oooooooo uuuAAAAA BBBBBBBB", Mnem.ADDSUB_r_n);
            Mnem.AddSubOp("ADD r,r", Mnem.ADD, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB_r_r);
            Mnem.AddSubOp("ADD r,(nnn)", Mnem.ADD, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.ADDSUB_r_InnnI);
            Mnem.AddSubOp("ADD r,(rrr)", Mnem.ADD, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB_r_IrrrI);

            Mnem.AddSubOp("ADD rr,nnn", Mnem.ADD, "oooooooo uuuAAAAA BBBBBBBB", Mnem.ADDSUB_rr_n);
            Mnem.AddSubOp("ADD16 rr,nnn", Mnem.ADD16, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB", Mnem.ADDSUB16_rr_nn);
            Mnem.AddSubOp("ADD rr,r", Mnem.ADD, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB_rr_r);
            Mnem.AddSubOp("ADD rr,rr", Mnem.ADD, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB_rr_rr);

            // TODO check these two below, something might be off with the operations
            Mnem.AddSubOp("ADD rr,(nnn)", Mnem.ADD, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.ADDSUB_rr_InnnI);
            Mnem.AddSubOp("ADD16 rr,(nnn)", Mnem.ADD16, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.ADDSUB16_rr_InnnI);

            Mnem.AddSubOp("ADD rr,(rrr)", Mnem.ADD, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB_rr_IrrrI);
            Mnem.AddSubOp("ADD16 rr,(rrr)", Mnem.ADD16, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB16_rr_IrrrI);

            Mnem.AddSubOp("ADD rrr,nnn", Mnem.ADD, "oooooooo uuuAAAAA BBBBBBBB", Mnem.ADDSUB_rrr_n);
            Mnem.AddSubOp("ADD16 rrr,nnn", Mnem.ADD16, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB", Mnem.ADDSUB16_rrr_n);
            Mnem.AddSubOp("ADD24 rrr,nnn", Mnem.ADD24, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.ADDSUB24_rrr_n);

            Mnem.AddSubOp("ADD rrr,r", Mnem.ADD, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB_rrr_r);
            Mnem.AddSubOp("ADD rrr,rr", Mnem.ADD, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB_rrr_rr);
            Mnem.AddSubOp("ADD rrr,rrr", Mnem.ADD, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB_rrr_rrr);

            Mnem.AddSubOp("ADD rrr,(nnn)", Mnem.ADD, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.ADDSUB_rrr_InnnI);
            Mnem.AddSubOp("ADD16 rrr,(nnn)", Mnem.ADD16, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.ADDSUB16_rrr_InnnI);
            Mnem.AddSubOp("ADD24 rrr,(nnn)", Mnem.ADD24, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.ADDSUB24_rrr_InnnI);

            Mnem.AddSubOp("ADD rrr,(rrr)", Mnem.ADD, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB_rrr_IrrrI);
            Mnem.AddSubOp("ADD16 rrr,(rrr)", Mnem.ADD16, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB16_rrr_IrrrI);
            Mnem.AddSubOp("ADD24 rrr,(rrr)", Mnem.ADD24, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB24_rrr_IrrrI);

            Mnem.AddSubOp("ADD rrrr,nnn", Mnem.ADD, "oooooooo uuuAAAAA BBBBBBBB", Mnem.ADDSUB_rrrr_n);
            Mnem.AddSubOp("ADD16 rrrr,nnn", Mnem.ADD16, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB", Mnem.ADDSUB16_rrrr_n);
            Mnem.AddSubOp("ADD24 rrrr,nnn", Mnem.ADD24, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.ADDSUB24_rrrr_n);
            Mnem.AddSubOp("ADD32 rrrr,nnn", Mnem.ADD32, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.ADDSUB32_rrrr_n);

            Mnem.AddSubOp("ADD rrrr,r", Mnem.ADD, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB_rrrr_r);
            Mnem.AddSubOp("ADD rrrr,rr", Mnem.ADD, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB_rrrr_rr);
            Mnem.AddSubOp("ADD rrrr,rrr", Mnem.ADD, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB_rrrr_rrr);
            Mnem.AddSubOp("ADD rrrr,rrrr", Mnem.ADD, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB_rrrr_rrrr);

            Mnem.AddSubOp("ADD rrrr,(nnn)", Mnem.ADD, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.ADDSUB_rrrr_InnnI);
            Mnem.AddSubOp("ADD16 rrrr,(nnn)", Mnem.ADD16, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.ADDSUB16_rrrr_InnnI);
            Mnem.AddSubOp("ADD24 rrrr,(nnn)", Mnem.ADD24, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.ADDSUB24_rrrr_InnnI);
            Mnem.AddSubOp("ADD32 rrrr,(nnn)", Mnem.ADD32, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.ADDSUB32_rrrr_InnnI);

            Mnem.AddSubOp("ADD rrrr,(rrr)", Mnem.ADD, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB_rrrr_IrrrI);
            Mnem.AddSubOp("ADD16 rrrr,(rrr)", Mnem.ADD16, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB16_rrrr_IrrrI);
            Mnem.AddSubOp("ADD24 rrrr,(rrr)", Mnem.ADD24, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB24_rrrr_IrrrI);
            Mnem.AddSubOp("ADD32 rrrr,(rrr)", Mnem.ADD32, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB32_rrrr_IrrrI);

            Mnem.AddSubOp("ADD (nnn),nnn", Mnem.ADD, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB", Mnem.ADDSUB_InnnI_nnn);
            Mnem.AddSubOp("ADD16 (nnn),nnn", Mnem.ADD16, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB BBBBBBBB", Mnem.ADDSUB16_InnnI_nnn);
            Mnem.AddSubOp("ADD24 (nnn),nnn", Mnem.ADD24, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.ADDSUB24_InnnI_nnn);
            Mnem.AddSubOp("ADD32 (nnn),nnn", Mnem.ADD32, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.ADDSUB32_InnnI_nnn);

            Mnem.AddSubOp("ADD (nnn),r", Mnem.ADD, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.ADDSUB_InnnI_r);
            Mnem.AddSubOp("ADD16 (nnn),r", Mnem.ADD16, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.ADDSUB16_InnnI_r);
            Mnem.AddSubOp("ADD24 (nnn),r", Mnem.ADD24, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.ADDSUB24_InnnI_r);
            Mnem.AddSubOp("ADD32 (nnn),r", Mnem.ADD32, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.ADDSUB32_InnnI_r);

            Mnem.AddSubOp("ADD16 (nnn),rr", Mnem.ADD16, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.ADDSUB16_InnnI_rr);
            Mnem.AddSubOp("ADD24 (nnn),rr", Mnem.ADD24, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.ADDSUB24_InnnI_rr);
            Mnem.AddSubOp("ADD32 (nnn),rr", Mnem.ADD32, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.ADDSUB32_InnnI_rr);

            Mnem.AddSubOp("ADD24 (nnn),rrr", Mnem.ADD24, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.ADDSUB24_InnnI_rrr);
            Mnem.AddSubOp("ADD32 (nnn),rrr", Mnem.ADD32, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.ADDSUB32_InnnI_rrr);

            Mnem.AddSubOp("ADD (nnn),rrrr", Mnem.ADD, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.ADDSUB32_InnnI_rrrr);

            Mnem.AddSubOp("ADD (rrr),nnn", Mnem.ADD, "oooooooo uuuAAAAA BBBBBBBB", Mnem.ADDSUB_IrrrI_nnn);
            Mnem.AddSubOp("ADD16 (rrr),nnn", Mnem.ADD16, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB", Mnem.ADDSUB16_IrrrI_nnn);
            Mnem.AddSubOp("ADD24 (rrr),nnn", Mnem.ADD24, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.ADDSUB24_IrrrI_nnn);
            Mnem.AddSubOp("ADD32 (rrr),nnn", Mnem.ADD32, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.ADDSUB32_IrrrI_nnn);

            Mnem.AddSubOp("ADD (rrr),r", Mnem.ADD, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB_IrrrI_r);
            Mnem.AddSubOp("ADD16 (rrr),r", Mnem.ADD16, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB16_IrrrI_r);
            Mnem.AddSubOp("ADD24 (rrr),r", Mnem.ADD24, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB24_IrrrI_r);
            Mnem.AddSubOp("ADD32 (rrr),r", Mnem.ADD32, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB32_IrrrI_r);

            Mnem.AddSubOp("ADD16 (rrr),rr", Mnem.ADD16, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB16_IrrrI_rr);
            Mnem.AddSubOp("ADD24 (rrr),rr", Mnem.ADD24, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB24_IrrrI_rr);
            Mnem.AddSubOp("ADD32 (rrr),rr", Mnem.ADD32, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB32_IrrrI_rr);

            Mnem.AddSubOp("ADD24 (rrr),rrr", Mnem.ADD24, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB24_IrrrI_rrr);
            Mnem.AddSubOp("ADD32 (rrr),rrr", Mnem.ADD32, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB32_IrrrI_rrr);

            Mnem.AddSubOp("ADD (rrr),rrrr", Mnem.ADD, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB_IrrrI_rrrr);

            // Floating point
            Mnem.AddSubOp("ADD fr,fr", Mnem.ADD, "oooooooo uuuuAAAA uuuuBBBB", Mnem.ADDSUB_fr_fr);
            Mnem.AddSubOp("ADD fr,nnn", Mnem.ADD, "oooooooo uuuuAAAA BBBBBBBB BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.ADDSUB_fr_nnn);


            Mnem.AddSubOp("ADD fr,r", Mnem.ADD, "oooooooo uuuuAAAA uuuBBBBB", Mnem.ADDSUB_fr_r);
            Mnem.AddSubOp("ADD fr,rr", Mnem.ADD, "oooooooo uuuuAAAA uuuBBBBB", Mnem.ADDSUB_fr_rr);
            Mnem.AddSubOp("ADD fr,rrr", Mnem.ADD, "oooooooo uuuuAAAA uuuBBBBB", Mnem.ADDSUB_fr_rrr);
            Mnem.AddSubOp("ADD fr,rrrr", Mnem.ADD, "oooooooo uuuuAAAA uuuBBBBB", Mnem.ADDSUB_fr_rrrr);

            Mnem.AddSubOp("ADD r,fr", Mnem.ADD, "oooooooo uuuAAAAA uuuuBBBB", Mnem.ADDSUB_r_fr);
            Mnem.AddSubOp("ADD rr,fr", Mnem.ADD, "oooooooo uuuAAAAA uuuuBBBB", Mnem.ADDSUB_rr_fr);
            Mnem.AddSubOp("ADD rrr,fr", Mnem.ADD, "oooooooo uuuAAAAA uuuuBBBB", Mnem.ADDSUB_rrr_fr);
            Mnem.AddSubOp("ADD rrrr,fr", Mnem.ADD, "oooooooo uuuAAAAA uuuuBBBB", Mnem.ADDSUB_rrrr_fr);

            Mnem.AddSubOp("ADD fr,(nnn)", Mnem.ADD, "oooooooo uuuuAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.ADDSUB_fr_InnnI);
            Mnem.AddSubOp("ADD fr,(rrr)", Mnem.ADD, "oooooooo uuuuAAAA uuuBBBBB", Mnem.ADDSUB_fr_IrrrI);
            Mnem.AddSubOp("ADD (nnn),fr", Mnem.ADD, "oooooooo uuuuBBBB AAAAAAAA AAAAAAAA AAAAAAAA", Mnem.ADDSUB_InnnI_fr);
            Mnem.AddSubOp("ADD (rrr),fr", Mnem.ADD, "oooooooo uuuuBBBB uuuAAAAA", Mnem.ADDSUB_IrrrI_fr);

            Mnem.AddSubOp("ADD (nnn),(nnn)", Mnem.ADD, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.ADDSUB_InnnI_InnnI);
            Mnem.AddSubOp("ADD16 (nnn),(nnn)", Mnem.ADD16, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.ADDSUB16_InnnI_InnnI);
            Mnem.AddSubOp("ADD24 (nnn),(nnn)", Mnem.ADD24, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.ADDSUB24_InnnI_InnnI);
            Mnem.AddSubOp("ADD32 (nnn),(nnn)", Mnem.ADD32, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.ADDSUB32_InnnI_InnnI);
        }
    }
}
