namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInitFREGS
    {
        public static void Initialize()
        {
            // FREGS
            Mnem.AddSubOp("FREGS nnn", Mnem.FREGS, "ooouuuuu AAAAAAAA", Mnem.FREGS_n);
            Mnem.AddSubOp("FREGS (nnn)", Mnem.FREGS, "ooouuuuu AAAAAAAA AAAAAAAA AAAAAAAA", Mnem.FREGS_InnnI);
            Mnem.AddSubOp("FREGS r", Mnem.FREGS, "oooAAAAA", Mnem.FREGS_r);
            Mnem.AddSubOp("FREGS (rrr)", Mnem.FREGS, "oooAAAAA", Mnem.FREGS_IrrrI);
        }
    }
}
