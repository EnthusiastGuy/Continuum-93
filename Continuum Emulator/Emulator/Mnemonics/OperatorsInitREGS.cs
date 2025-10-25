namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInitREGS
    {
        public static void Initialize()
        {
            // REGS
            Mnem.AddSubOp("REGS nnn", Mnem.REGS, "ooouuuuu AAAAAAAA", Mnem.REGS_n);
            Mnem.AddSubOp("REGS (nnn)", Mnem.REGS, "ooouuuuu AAAAAAAA AAAAAAAA AAAAAAAA", Mnem.REGS_InnnI);
            Mnem.AddSubOp("REGS r", Mnem.REGS, "oooAAAAA", Mnem.REGS_r);
            Mnem.AddSubOp("REGS (rrr)", Mnem.REGS, "oooAAAAA", Mnem.REGS_IrrrI);
        }
    }
}
