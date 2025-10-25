namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInit_C_RGB2HSL
    {
        public static void Initialize()
        {
            Mnem.AddSubOp("RGB2HSL nnn,rrrr", Mnem.RGB2HSL, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.RGB2HSLB_nnn_rrrr);
            Mnem.AddSubOp("RGB2HSL rrr,rrrr", Mnem.RGB2HSL, "oooooooo uuuAAAAA uuuBBBBB", Mnem.RGB2HSLB_rrr_rrrr);
            Mnem.AddSubOp("RGB2HSL nnn,(nnn)", Mnem.RGB2HSL, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.RGB2HSLB_nnn_InnnI);
            Mnem.AddSubOp("RGB2HSL rrr,(nnn)", Mnem.RGB2HSL, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.RGB2HSLB_rrr_InnnI);

            Mnem.AddSubOp("RGB2HSL nnn,(rrr)", Mnem.RGB2HSL, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.RGB2HSLB_nnn_IrrrI);
            Mnem.AddSubOp("RGB2HSL rrr,(rrr)", Mnem.RGB2HSL, "oooooooo uuuAAAAA uuuBBBBB", Mnem.RGB2HSLB_rrr_IrrrI);


            Mnem.AddSubOp("RGB2HSL (nnn),rrrr", Mnem.RGB2HSL, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.RGB2HSLB_InnnI_rrrr);
            Mnem.AddSubOp("RGB2HSL (rrr),rrrr", Mnem.RGB2HSL, "oooooooo uuuAAAAA uuuBBBBB", Mnem.RGB2HSLB_IrrrI_rrrr);
            Mnem.AddSubOp("RGB2HSL (nnn),(nnn)", Mnem.RGB2HSL, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.RGB2HSLB_InnnI_InnnI);
            Mnem.AddSubOp("RGB2HSL (rrr),(nnn)", Mnem.RGB2HSL, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.RGB2HSLB_IrrrI_InnnI);

            Mnem.AddSubOp("RGB2HSL (nnn),(rrr)", Mnem.RGB2HSL, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.RGB2HSLB_InnnI_IrrrI);
            Mnem.AddSubOp("RGB2HSL (rrr),(rrr)", Mnem.RGB2HSL, "oooooooo uuuAAAAA uuuBBBBB", Mnem.RGB2HSLB_IrrrI_IrrrI);
        }
    }
}
