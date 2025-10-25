namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInitINV
    {
        public static void Initialize()
        {
            // INV
            Mnem.AddSubOp("INV r", Mnem.INV, "oooAAAAA", Mnem.INV_r);
            Mnem.AddSubOp("INV rr", Mnem.INV, "oooAAAAA", Mnem.INV_rr);
            Mnem.AddSubOp("INV rrr", Mnem.INV, "oooAAAAA", Mnem.INV_rrr);
            Mnem.AddSubOp("INV rrrr", Mnem.INV, "oooAAAAA", Mnem.INV_rrrr);

        }
    }
}
