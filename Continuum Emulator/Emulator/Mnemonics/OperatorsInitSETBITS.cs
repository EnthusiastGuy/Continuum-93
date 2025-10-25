namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInitSETBITS
    {
        public static void Initialize()
        {
            Mnem.AddSubOp("SETBITS rrrr,r,nnn", Mnem.SETBITS, "ooooooAA AAABBBBB CCCCCCCC", Mnem.SETBITS_rrrr_r_n);
            Mnem.AddSubOp("SETBITS rrrr,rr,nnn", Mnem.SETBITS, "ooooooAA AAABBBBB CCCCCCCC", Mnem.SETBITS_rrrr_rr_n);
            Mnem.AddSubOp("SETBITS rrrr,rrr,nnn", Mnem.SETBITS, "ooooooAA AAABBBBB CCCCCCCC", Mnem.SETBITS_rrrr_rrr_n);
            Mnem.AddSubOp("SETBITS rrrr,rrrr,nnn", Mnem.SETBITS, "ooooooAA AAABBBBB CCCCCCCC", Mnem.SETBITS_rrrr_rrrr_n);

            Mnem.AddSubOp("SETBITS rrrr,r,r", Mnem.SETBITS, "ooooooAA AAABBBBB uuuCCCCC", Mnem.SETBITS_rrrr_r_r);
            Mnem.AddSubOp("SETBITS rrrr,rr,r", Mnem.SETBITS, "ooooooAA AAABBBBB uuuCCCCC", Mnem.SETBITS_rrrr_rr_r);
            Mnem.AddSubOp("SETBITS rrrr,rrr,r", Mnem.SETBITS, "ooooooAA AAABBBBB uuuCCCCC", Mnem.SETBITS_rrrr_rrr_r);
            Mnem.AddSubOp("SETBITS rrrr,rrrr,r", Mnem.SETBITS, "ooooooAA AAABBBBB uuuCCCCC", Mnem.SETBITS_rrrr_rrrr_r);
        }
    }
}
