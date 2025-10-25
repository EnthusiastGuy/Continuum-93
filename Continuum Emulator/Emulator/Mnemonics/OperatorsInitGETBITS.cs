namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInitGETBITS
    {
        public static void Initialize()
        {
            Mnem.AddSubOp("GETBITS r,rrrr,nnn", Mnem.GETBITS, "ooooooAA AAABBBBB CCCCCCCC", Mnem.GETBITS_r_rrrr_n);
            Mnem.AddSubOp("GETBITS rr,rrrr,nnn", Mnem.GETBITS, "ooooooAA AAABBBBB CCCCCCCC", Mnem.GETBITS_rr_rrrr_n);
            Mnem.AddSubOp("GETBITS rrr,rrrr,nnn", Mnem.GETBITS, "ooooooAA AAABBBBB CCCCCCCC", Mnem.GETBITS_rrr_rrrr_n);
            Mnem.AddSubOp("GETBITS rrrr,rrrr,nnn", Mnem.GETBITS, "ooooooAA AAABBBBB CCCCCCCC", Mnem.GETBITS_rrrr_rrrr_n);

            Mnem.AddSubOp("GETBITS r,rrrr,r", Mnem.GETBITS, "ooooooAA AAABBBBB uuuCCCCC", Mnem.GETBITS_r_rrrr_r);
            Mnem.AddSubOp("GETBITS rr,rrrr,r", Mnem.GETBITS, "ooooooAA AAABBBBB uuuCCCCC", Mnem.GETBITS_rr_rrrr_r);
            Mnem.AddSubOp("GETBITS rrr,rrrr,r", Mnem.GETBITS, "ooooooAA AAABBBBB uuuCCCCC", Mnem.GETBITS_rrr_rrrr_r);
            Mnem.AddSubOp("GETBITS rrrr,rrrr,r", Mnem.GETBITS, "ooooooAA AAABBBBB uuuCCCCC", Mnem.GETBITS_rrrr_rrrr_r);
        }
    }
}
