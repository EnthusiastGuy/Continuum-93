namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInitEX
    {
        public static void Initialize()
        {
            // EX
            Mnem.AddSubOp("EX r,r", Mnem.EX, "ooooooAA AAABBBBB", Mnem.EX_r_r);
            Mnem.AddSubOp("EX rr,rr", Mnem.EX, "ooooooAA AAABBBBB", Mnem.EX_rr_rr);
            Mnem.AddSubOp("EX rrr,rrr", Mnem.EX, "ooooooAA AAABBBBB", Mnem.EX_rrr_rrr);
            Mnem.AddSubOp("EX rrrr,rrrr", Mnem.EX, "ooooooAA AAABBBBB", Mnem.EX_rrrr_rrrr);

            // Float registers
            Mnem.AddSubOp("EX fr,fr", Mnem.EX, "oooooouu AAAABBBB", Mnem.EX_fr_fr);
        }
    }
}
