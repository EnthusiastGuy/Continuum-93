namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInit_C_RGB2HSB
    {
        public static void Initialize()
        {
            Mnem.AddSubOp("RGB2HSB nnn,rrrr", Mnem.RGB2HSB, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.RGB2HSLB_nnn_rrrr);
            Mnem.AddSubOp("RGB2HSB rrr,rrrr", Mnem.RGB2HSB, "oooooooo uuuAAAAA uuuBBBBB", Mnem.RGB2HSLB_rrr_rrrr);
            Mnem.AddSubOp("RGB2HSB nnn,(nnn)", Mnem.RGB2HSB, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.RGB2HSLB_nnn_InnnI);
            Mnem.AddSubOp("RGB2HSB rrr,(nnn)", Mnem.RGB2HSB, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.RGB2HSLB_rrr_InnnI);

            Mnem.AddSubOp("RGB2HSB nnn,(rrr)", Mnem.RGB2HSB, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.RGB2HSLB_nnn_IrrrI);
            Mnem.AddSubOp("RGB2HSB rrr,(rrr)", Mnem.RGB2HSB, "oooooooo uuuAAAAA uuuBBBBB", Mnem.RGB2HSLB_rrr_IrrrI);


            Mnem.AddSubOp("RGB2HSB (nnn),rrrr", Mnem.RGB2HSB, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.RGB2HSLB_InnnI_rrrr);
            Mnem.AddSubOp("RGB2HSB (rrr),rrrr", Mnem.RGB2HSB, "oooooooo uuuAAAAA uuuBBBBB", Mnem.RGB2HSLB_IrrrI_rrrr);
            Mnem.AddSubOp("RGB2HSB (nnn),(nnn)", Mnem.RGB2HSB, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.RGB2HSLB_InnnI_InnnI);
            Mnem.AddSubOp("RGB2HSB (rrr),(nnn)", Mnem.RGB2HSB, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.RGB2HSLB_IrrrI_InnnI);

            Mnem.AddSubOp("RGB2HSB (nnn),(rrr)", Mnem.RGB2HSB, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.RGB2HSLB_InnnI_IrrrI);
            Mnem.AddSubOp("RGB2HSB (rrr),(rrr)", Mnem.RGB2HSB, "oooooooo uuuAAAAA uuuBBBBB", Mnem.RGB2HSLB_IrrrI_IrrrI);
        }
    }
}
