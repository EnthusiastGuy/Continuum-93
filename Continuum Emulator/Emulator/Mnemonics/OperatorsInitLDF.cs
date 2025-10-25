namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInitLDF
    {
        public static void Initialize()
        {
            // LDF
            Mnem.AddSubOp("LDF r", Mnem.LDF, "oooAAAAA", Mnem.LDF_r);
            Mnem.AddSubOp("LDF (rrr)", Mnem.LDF, "oooAAAAA", Mnem.LDF_IrrrI);
            Mnem.AddSubOp("LDF (nnn)", Mnem.LDF, "ooouuuuu AAAAAAAA AAAAAAAA AAAAAAAA", Mnem.LDF_InnnI);

            Mnem.AddSubOp("LDF r,nnn", Mnem.LDF, "oooAAAAA BBBBBBBB", Mnem.LDF_r_n);
            Mnem.AddSubOp("LDF (rrr),nnn", Mnem.LDF, "oooAAAAA BBBBBBBB", Mnem.LDF_IrrrI_n);
            Mnem.AddSubOp("LDF (nnn),nnn", Mnem.LDF, "ooouuuuu AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB", Mnem.LDF_InnnI_n);
        }
    }
}
