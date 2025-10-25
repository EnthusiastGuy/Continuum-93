namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInitLDREGS_STREGS
    {
        public static void Initialize()
        {
            // LDREGS
            Mnem.AddSubOp("LDREGS r,r,(rrr)", Mnem.LDREGS, "oAAAAABB BBBCCCCC", Mnem.LDSTREGS_r_r_IrrrI);
            Mnem.AddSubOp("LDREGS r,r,(nnn)", Mnem.LDREGS, "ouuAAAAA uuuBBBBB CCCCCCCC CCCCCCCC CCCCCCCC", Mnem.LDSTREGS_r_r_InnnI);

            // STREGS
            Mnem.AddSubOp("STREGS (rrr),r,r", Mnem.STREGS, "oAAAAABB BBBCCCCC", Mnem.LDSTREGS_r_r_IrrrI);
            Mnem.AddSubOp("STREGS (nnn),r,r", Mnem.STREGS, "ouuBBBBB uuuCCCCC AAAAAAAA AAAAAAAA AAAAAAAA", Mnem.LDSTREGS_r_r_InnnI);
        }
    }
}
