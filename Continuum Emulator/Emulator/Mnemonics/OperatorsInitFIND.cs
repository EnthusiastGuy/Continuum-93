namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInitFIND
    {
        public static void Initialize()
        {
            // FIND
            Mnem.AddSubOp("FIND (rrr),nnn", Mnem.FIND, "oooAAAAA BBBBBBBB", Mnem.FIND_IrrrI_n);
            Mnem.AddSubOp("FIND (rrr),(nnn)", Mnem.FIND, "oooAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.FIND_IrrrI_InnnI);
            Mnem.AddSubOp("FIND (rrr),(rrr)", Mnem.FIND, "oooAAAAA uuuBBBBB", Mnem.FIND_IrrrI_IrrrI);
        }
    }
}
