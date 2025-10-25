namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInit_C_HSB2RGB
    {
        public static void Initialize()
        {
            Mnem.AddSubOp("HSB2RGB nnn,rrr", Mnem.HSB2RGB, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.HSLB2RGB_nnnn_rrr);
            Mnem.AddSubOp("HSB2RGB rrrr,rrr", Mnem.HSB2RGB, "oooooooo uuuAAAAA uuuBBBBB", Mnem.HSLB2RGB_rrrr_rrr);
            Mnem.AddSubOp("HSB2RGB nnn,(nnn)", Mnem.HSB2RGB, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.HSLB2RGB_nnnn_InnnI);
            Mnem.AddSubOp("HSB2RGB rrrr,(nnn)", Mnem.HSB2RGB, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.HSLB2RGB_rrrr_InnnI);

            Mnem.AddSubOp("HSB2RGB nnn,(rrr)", Mnem.HSB2RGB, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.HSLB2RGB_nnnn_IrrrI);
            Mnem.AddSubOp("HSB2RGB rrrr,(rrr)", Mnem.HSB2RGB, "oooooooo uuuAAAAA uuuBBBBB", Mnem.HSLB2RGB_rrrr_IrrrI);


            Mnem.AddSubOp("HSB2RGB (nnn),rrr", Mnem.HSB2RGB, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.HSLB2RGB_InnnI_rrr);
            Mnem.AddSubOp("HSB2RGB (rrr),rrr", Mnem.HSB2RGB, "oooooooo uuuAAAAA uuuBBBBB", Mnem.HSLB2RGB_IrrrI_rrr);
            Mnem.AddSubOp("HSB2RGB (nnn),(nnn)", Mnem.HSB2RGB, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.HSLB2RGB_InnnI_InnnI);
            Mnem.AddSubOp("HSB2RGB (rrr),(nnn)", Mnem.HSB2RGB, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.HSLB2RGB_IrrrI_InnnI);

            Mnem.AddSubOp("HSB2RGB (nnn),(rrr)", Mnem.HSB2RGB, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.HSLB2RGB_InnnI_IrrrI);
            Mnem.AddSubOp("HSB2RGB (rrr),(rrr)", Mnem.HSB2RGB, "oooooooo uuuAAAAA uuuBBBBB", Mnem.HSLB2RGB_IrrrI_IrrrI);
        }
    }
}
