namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInit_C_HSL2RGB
    {
        public static void Initialize()
        {
            Mnem.AddSubOp("HSL2RGB nnn,rrr", Mnem.HSL2RGB, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.HSLB2RGB_nnnn_rrr);
            Mnem.AddSubOp("HSL2RGB rrrr,rrr", Mnem.HSL2RGB, "oooooooo uuuAAAAA uuuBBBBB", Mnem.HSLB2RGB_rrrr_rrr);
            Mnem.AddSubOp("HSL2RGB nnn,(nnn)", Mnem.HSL2RGB, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.HSLB2RGB_nnnn_InnnI);
            Mnem.AddSubOp("HSL2RGB rrrr,(nnn)", Mnem.HSL2RGB, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.HSLB2RGB_rrrr_InnnI);

            Mnem.AddSubOp("HSL2RGB nnn,(rrr)", Mnem.HSL2RGB, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.HSLB2RGB_nnnn_IrrrI);
            Mnem.AddSubOp("HSL2RGB rrrr,(rrr)", Mnem.HSL2RGB, "oooooooo uuuAAAAA uuuBBBBB", Mnem.HSLB2RGB_rrrr_IrrrI);


            Mnem.AddSubOp("HSL2RGB (nnn),rrr", Mnem.HSL2RGB, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.HSLB2RGB_InnnI_rrr);
            Mnem.AddSubOp("HSL2RGB (rrr),rrr", Mnem.HSL2RGB, "oooooooo uuuAAAAA uuuBBBBB", Mnem.HSLB2RGB_IrrrI_rrr);
            Mnem.AddSubOp("HSL2RGB (nnn),(nnn)", Mnem.HSL2RGB, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.HSLB2RGB_InnnI_InnnI);
            Mnem.AddSubOp("HSL2RGB (rrr),(nnn)", Mnem.HSL2RGB, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.HSLB2RGB_IrrrI_InnnI);

            Mnem.AddSubOp("HSL2RGB (nnn),(rrr)", Mnem.HSL2RGB, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.HSLB2RGB_InnnI_IrrrI);
            Mnem.AddSubOp("HSL2RGB (rrr),(rrr)", Mnem.HSL2RGB, "oooooooo uuuAAAAA uuuBBBBB", Mnem.HSLB2RGB_IrrrI_IrrrI);
        }
    }
}
