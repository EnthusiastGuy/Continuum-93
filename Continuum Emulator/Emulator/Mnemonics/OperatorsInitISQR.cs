namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInitISQR
    {
        public static void Initialize()
        {
            // Inverse Square Root
            Mnem.AddSubOp("ISQR fr", Mnem.ISQR, "oooooouu uuuuAAAA", Mnem.SQRCR_fr);
            Mnem.AddSubOp("ISQR r", Mnem.ISQR, "oooooouu uuuAAAAA", Mnem.SQRCR_r);
            Mnem.AddSubOp("ISQR rr", Mnem.ISQR, "oooooouu uuuAAAAA", Mnem.SQRCR_rr);
            Mnem.AddSubOp("ISQR rrr", Mnem.ISQR, "oooooouu uuuAAAAA", Mnem.SQRCR_rrr);
            Mnem.AddSubOp("ISQR rrrr", Mnem.ISQR, "oooooouu uuuAAAAA", Mnem.SQRCR_rrrr);
            Mnem.AddSubOp("ISQR (rrr)", Mnem.ISQR, "oooooouu uuuAAAAA", Mnem.SQRCR_IrrrI);
            Mnem.AddSubOp("ISQR (nnn)", Mnem.ISQR, "oooooouu AAAAAAAA AAAAAAAA AAAAAAAA", Mnem.SQRCR_InnnI);

            Mnem.AddSubOp("ISQR fr,fr", Mnem.ISQR, "oooooouu AAAABBBB", Mnem.SQRCR_fr_fr);
            Mnem.AddSubOp("ISQR fr,r", Mnem.ISQR, "oooooouB BBBBAAAA", Mnem.SQRCR_fr_r);
            Mnem.AddSubOp("ISQR fr,rr", Mnem.ISQR, "oooooouB BBBBAAAA", Mnem.SQRCR_fr_rr);
            Mnem.AddSubOp("ISQR fr,rrr", Mnem.ISQR, "oooooouB BBBBAAAA", Mnem.SQRCR_fr_rrr);
            Mnem.AddSubOp("ISQR fr,rrrr", Mnem.ISQR, "oooooouB BBBBAAAA", Mnem.SQRCR_fr_rrrr);

            Mnem.AddSubOp("ISQR fr,(rrr)", Mnem.ISQR, "oooooouB BBBBAAAA", Mnem.SQRCR_fr_IrrrI);
            Mnem.AddSubOp("ISQR fr,(nnn)", Mnem.ISQR, "oooooouu uuuuAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.SQRCR_fr_InnnI);
            Mnem.AddSubOp("ISQR fr,nnn", Mnem.ISQR, "oooooouu uuuuAAAA BBBBBBBB BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.SQRCR_fr_n);

            Mnem.AddSubOp("ISQR r,fr", Mnem.ISQR, "oooooouA AAAABBBB", Mnem.SQRCR_r_fr);
            Mnem.AddSubOp("ISQR rr,fr", Mnem.ISQR, "oooooouA AAAABBBB", Mnem.SQRCR_rr_fr);
            Mnem.AddSubOp("ISQR rrr,fr", Mnem.ISQR, "oooooouA AAAABBBB", Mnem.SQRCR_rrr_fr);
            Mnem.AddSubOp("ISQR rrrr,fr", Mnem.ISQR, "oooooouA AAAABBBB", Mnem.SQRCR_rrrr_fr);

            Mnem.AddSubOp("ISQR (rrr),fr", Mnem.ISQR, "oooooouA AAAABBBB", Mnem.SQRCR_IrrrI_fr);
            Mnem.AddSubOp("ISQR (nnn),fr", Mnem.ISQR, "oooooouu uuuuBBBB AAAAAAAA AAAAAAAA AAAAAAAA", Mnem.SQRCR_InnnI_fr);
        }
    }
}
