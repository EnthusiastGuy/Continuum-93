namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInitSUB
    {
        public static void Initialize()
        {
            // SUB
            Mnem.AddSubOp("SUB r,nnn", Mnem.SUB, "oooooooo uuuAAAAA BBBBBBBB", Mnem.ADDSUB_r_n);
            Mnem.AddSubOp("SUB r,r", Mnem.SUB, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB_r_r);
            Mnem.AddSubOp("SUB r,(nnn)", Mnem.SUB, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.ADDSUB_r_InnnI);
            Mnem.AddSubOp("SUB r,(rrr)", Mnem.SUB, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB_r_IrrrI);

            Mnem.AddSubOp("SUB rr,nnn", Mnem.SUB, "oooooooo uuuAAAAA BBBBBBBB", Mnem.ADDSUB_rr_n);
            Mnem.AddSubOp("SUB16 rr,nnn", Mnem.SUB16, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB", Mnem.ADDSUB16_rr_nn);
            Mnem.AddSubOp("SUB rr,r", Mnem.SUB, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB_rr_r);
            Mnem.AddSubOp("SUB rr,rr", Mnem.SUB, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB_rr_rr);

            Mnem.AddSubOp("SUB rr,(nnn)", Mnem.SUB, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.ADDSUB_rr_InnnI);
            Mnem.AddSubOp("SUB16 rr,(nnn)", Mnem.SUB16, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.ADDSUB16_rr_InnnI);

            Mnem.AddSubOp("SUB rr,(rrr)", Mnem.SUB, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB_rr_IrrrI);
            Mnem.AddSubOp("SUB16 rr,(rrr)", Mnem.SUB16, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB16_rr_IrrrI);

            Mnem.AddSubOp("SUB rrr,nnn", Mnem.SUB, "oooooooo uuuAAAAA BBBBBBBB", Mnem.ADDSUB_rrr_n);
            Mnem.AddSubOp("SUB16 rrr,nnn", Mnem.SUB16, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB", Mnem.ADDSUB16_rrr_n);
            Mnem.AddSubOp("SUB24 rrr,nnn", Mnem.SUB24, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.ADDSUB24_rrr_n);

            Mnem.AddSubOp("SUB rrr,r", Mnem.SUB, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB_rrr_r);
            Mnem.AddSubOp("SUB rrr,rr", Mnem.SUB, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB_rrr_rr);
            Mnem.AddSubOp("SUB rrr,rrr", Mnem.SUB, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB_rrr_rrr);

            Mnem.AddSubOp("SUB rrr,(nnn)", Mnem.SUB, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.ADDSUB_rrr_InnnI);
            Mnem.AddSubOp("SUB16 rrr,(nnn)", Mnem.SUB16, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.ADDSUB16_rrr_InnnI);
            Mnem.AddSubOp("SUB24 rrr,(nnn)", Mnem.SUB24, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.ADDSUB24_rrr_InnnI);

            Mnem.AddSubOp("SUB rrr,(rrr)", Mnem.SUB, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB_rrr_IrrrI);
            Mnem.AddSubOp("SUB16 rrr,(rrr)", Mnem.SUB16, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB16_rrr_IrrrI);
            Mnem.AddSubOp("SUB24 rrr,(rrr)", Mnem.SUB24, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB24_rrr_IrrrI);

            Mnem.AddSubOp("SUB rrrr,nnn", Mnem.SUB, "oooooooo uuuAAAAA BBBBBBBB", Mnem.ADDSUB_rrrr_n);
            Mnem.AddSubOp("SUB16 rrrr,nnn", Mnem.SUB16, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB", Mnem.ADDSUB16_rrrr_n);
            Mnem.AddSubOp("SUB24 rrrr,nnn", Mnem.SUB24, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.ADDSUB24_rrrr_n);
            Mnem.AddSubOp("SUB32 rrrr,nnn", Mnem.SUB32, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.ADDSUB32_rrrr_n);

            Mnem.AddSubOp("SUB rrrr,r", Mnem.SUB, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB_rrrr_r);
            Mnem.AddSubOp("SUB rrrr,rr", Mnem.SUB, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB_rrrr_rr);
            Mnem.AddSubOp("SUB rrrr,rrr", Mnem.SUB, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB_rrrr_rrr);
            Mnem.AddSubOp("SUB rrrr,rrrr", Mnem.SUB, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB_rrrr_rrrr);

            Mnem.AddSubOp("SUB rrrr,(nnn)", Mnem.SUB, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.ADDSUB_rrrr_InnnI);
            Mnem.AddSubOp("SUB16 rrrr,(nnn)", Mnem.SUB16, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.ADDSUB16_rrrr_InnnI);
            Mnem.AddSubOp("SUB24 rrrr,(nnn)", Mnem.SUB24, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.ADDSUB24_rrrr_InnnI);
            Mnem.AddSubOp("SUB32 rrrr,(nnn)", Mnem.SUB32, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.ADDSUB32_rrrr_InnnI);

            Mnem.AddSubOp("SUB rrrr,(rrr)", Mnem.SUB, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB_rrrr_IrrrI);
            Mnem.AddSubOp("SUB16 rrrr,(rrr)", Mnem.SUB16, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB16_rrrr_IrrrI);
            Mnem.AddSubOp("SUB24 rrrr,(rrr)", Mnem.SUB24, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB24_rrrr_IrrrI);
            Mnem.AddSubOp("SUB32 rrrr,(rrr)", Mnem.SUB32, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB32_rrrr_IrrrI);

            Mnem.AddSubOp("SUB (nnn),nnn", Mnem.SUB, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB", Mnem.ADDSUB_InnnI_nnn);
            Mnem.AddSubOp("SUB16 (nnn),nnn", Mnem.SUB16, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB BBBBBBBB", Mnem.ADDSUB16_InnnI_nnn);
            Mnem.AddSubOp("SUB24 (nnn),nnn", Mnem.SUB24, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.ADDSUB24_InnnI_nnn);
            Mnem.AddSubOp("SUB32 (nnn),nnn", Mnem.SUB32, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.ADDSUB32_InnnI_nnn);

            Mnem.AddSubOp("SUB (nnn),r", Mnem.SUB, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.ADDSUB_InnnI_r);
            Mnem.AddSubOp("SUB16 (nnn),r", Mnem.SUB16, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.ADDSUB16_InnnI_r);
            Mnem.AddSubOp("SUB24 (nnn),r", Mnem.SUB24, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.ADDSUB24_InnnI_r);
            Mnem.AddSubOp("SUB32 (nnn),r", Mnem.SUB32, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.ADDSUB32_InnnI_r);

            Mnem.AddSubOp("SUB16 (nnn),rr", Mnem.SUB16, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.ADDSUB16_InnnI_rr);
            Mnem.AddSubOp("SUB24 (nnn),rr", Mnem.SUB24, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.ADDSUB24_InnnI_rr);
            Mnem.AddSubOp("SUB32 (nnn),rr", Mnem.SUB32, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.ADDSUB32_InnnI_rr);

            Mnem.AddSubOp("SUB24 (nnn),rrr", Mnem.SUB24, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.ADDSUB24_InnnI_rrr);
            Mnem.AddSubOp("SUB32 (nnn),rrr", Mnem.SUB32, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.ADDSUB32_InnnI_rrr);

            Mnem.AddSubOp("SUB (nnn),rrrr", Mnem.SUB, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.ADDSUB32_InnnI_rrrr);

            Mnem.AddSubOp("SUB (rrr),nnn", Mnem.SUB, "oooooooo uuuAAAAA BBBBBBBB", Mnem.ADDSUB_IrrrI_nnn);
            Mnem.AddSubOp("SUB16 (rrr),nnn", Mnem.SUB16, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB", Mnem.ADDSUB16_IrrrI_nnn);
            Mnem.AddSubOp("SUB24 (rrr),nnn", Mnem.SUB24, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.ADDSUB24_IrrrI_nnn);
            Mnem.AddSubOp("SUB32 (rrr),nnn", Mnem.SUB32, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.ADDSUB32_IrrrI_nnn);

            Mnem.AddSubOp("SUB (rrr),r", Mnem.SUB, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB_IrrrI_r);
            Mnem.AddSubOp("SUB16 (rrr),r", Mnem.SUB16, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB16_IrrrI_r);
            Mnem.AddSubOp("SUB24 (rrr),r", Mnem.SUB24, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB24_IrrrI_r);
            Mnem.AddSubOp("SUB32 (rrr),r", Mnem.SUB32, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB32_IrrrI_r);

            Mnem.AddSubOp("SUB16 (rrr),rr", Mnem.SUB16, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB16_IrrrI_rr);
            Mnem.AddSubOp("SUB24 (rrr),rr", Mnem.SUB24, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB24_IrrrI_rr);
            Mnem.AddSubOp("SUB32 (rrr),rr", Mnem.SUB32, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB32_IrrrI_rr);

            Mnem.AddSubOp("SUB24 (rrr),rrr", Mnem.SUB24, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB24_IrrrI_rrr);
            Mnem.AddSubOp("SUB32 (rrr),rrr", Mnem.SUB32, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB32_IrrrI_rrr);

            Mnem.AddSubOp("SUB (rrr),rrrr", Mnem.SUB, "oooooooo uuuAAAAA uuuBBBBB", Mnem.ADDSUB_IrrrI_rrrr);

            // Floating point
            Mnem.AddSubOp("SUB fr,fr", Mnem.SUB, "oooooooo uuuuAAAA uuuuBBBB", Mnem.ADDSUB_fr_fr);
            Mnem.AddSubOp("SUB fr,nnn", Mnem.SUB, "oooooooo uuuuAAAA BBBBBBBB BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.ADDSUB_fr_nnn);


            Mnem.AddSubOp("SUB fr,r", Mnem.SUB, "oooooooo uuuuAAAA uuuBBBBB", Mnem.ADDSUB_fr_r);
            Mnem.AddSubOp("SUB fr,rr", Mnem.SUB, "oooooooo uuuuAAAA uuuBBBBB", Mnem.ADDSUB_fr_rr);
            Mnem.AddSubOp("SUB fr,rrr", Mnem.SUB, "oooooooo uuuuAAAA uuuBBBBB", Mnem.ADDSUB_fr_rrr);
            Mnem.AddSubOp("SUB fr,rrrr", Mnem.SUB, "oooooooo uuuuAAAA uuuBBBBB", Mnem.ADDSUB_fr_rrrr);

            Mnem.AddSubOp("SUB r,fr", Mnem.SUB, "oooooooo uuuAAAAA uuuuBBBB", Mnem.ADDSUB_r_fr);
            Mnem.AddSubOp("SUB rr,fr", Mnem.SUB, "oooooooo uuuAAAAA uuuuBBBB", Mnem.ADDSUB_rr_fr);
            Mnem.AddSubOp("SUB rrr,fr", Mnem.SUB, "oooooooo uuuAAAAA uuuuBBBB", Mnem.ADDSUB_rrr_fr);
            Mnem.AddSubOp("SUB rrrr,fr", Mnem.SUB, "oooooooo uuuAAAAA uuuuBBBB", Mnem.ADDSUB_rrrr_fr);

            Mnem.AddSubOp("SUB fr,(nnn)", Mnem.SUB, "oooooooo uuuuAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.ADDSUB_fr_InnnI);
            Mnem.AddSubOp("SUB fr,(rrr)", Mnem.SUB, "oooooooo uuuuAAAA uuuBBBBB", Mnem.ADDSUB_fr_IrrrI);
            Mnem.AddSubOp("SUB (nnn),fr", Mnem.SUB, "oooooooo uuuuBBBB AAAAAAAA AAAAAAAA AAAAAAAA", Mnem.ADDSUB_InnnI_fr);
            Mnem.AddSubOp("SUB (rrr),fr", Mnem.SUB, "oooooooo uuuuBBBB uuuAAAAA", Mnem.ADDSUB_IrrrI_fr);

            Mnem.AddSubOp("SUB (nnn),(nnn)", Mnem.SUB, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.ADDSUB_InnnI_InnnI);
            Mnem.AddSubOp("SUB16 (nnn),(nnn)", Mnem.SUB16, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.ADDSUB16_InnnI_InnnI);
            Mnem.AddSubOp("SUB24 (nnn),(nnn)", Mnem.SUB24, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.ADDSUB24_InnnI_InnnI);
            Mnem.AddSubOp("SUB32 (nnn),(nnn)", Mnem.SUB32, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.ADDSUB32_InnnI_InnnI);
        }
    }
}
