namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInitSQR
    {
        public static void Initialize()
        {
            // SQR
            Mnem.AddSubOp("SQR fr", Mnem.SQR, "oooooouu uuuuAAAA", Mnem.SQRCR_fr);
            Mnem.AddSubOp("SQR r", Mnem.SQR, "oooooouu uuuAAAAA", Mnem.SQRCR_r);
            Mnem.AddSubOp("SQR rr", Mnem.SQR, "oooooouu uuuAAAAA", Mnem.SQRCR_rr);
            Mnem.AddSubOp("SQR rrr", Mnem.SQR, "oooooouu uuuAAAAA", Mnem.SQRCR_rrr);
            Mnem.AddSubOp("SQR rrrr", Mnem.SQR, "oooooouu uuuAAAAA", Mnem.SQRCR_rrrr);
            Mnem.AddSubOp("SQR (rrr)", Mnem.SQR, "oooooouu uuuAAAAA", Mnem.SQRCR_IrrrI);
            Mnem.AddSubOp("SQR (nnn)", Mnem.SQR, "oooooouu AAAAAAAA AAAAAAAA AAAAAAAA", Mnem.SQRCR_InnnI);

            Mnem.AddSubOp("SQR fr,fr", Mnem.SQR, "oooooouu AAAABBBB", Mnem.SQRCR_fr_fr);
            Mnem.AddSubOp("SQR fr,r", Mnem.SQR, "oooooouB BBBBAAAA", Mnem.SQRCR_fr_r);
            Mnem.AddSubOp("SQR fr,rr", Mnem.SQR, "oooooouB BBBBAAAA", Mnem.SQRCR_fr_rr);
            Mnem.AddSubOp("SQR fr,rrr", Mnem.SQR, "oooooouB BBBBAAAA", Mnem.SQRCR_fr_rrr);
            Mnem.AddSubOp("SQR fr,rrrr", Mnem.SQR, "oooooouB BBBBAAAA", Mnem.SQRCR_fr_rrrr);

            Mnem.AddSubOp("SQR fr,(rrr)", Mnem.SQR, "oooooouB BBBBAAAA", Mnem.SQRCR_fr_IrrrI);
            Mnem.AddSubOp("SQR fr,(nnn)", Mnem.SQR, "oooooouu uuuuAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.SQRCR_fr_InnnI);
            Mnem.AddSubOp("SQR fr,nnn", Mnem.SQR, "oooooouu uuuuAAAA BBBBBBBB BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.SQRCR_fr_n);

            Mnem.AddSubOp("SQR r,fr", Mnem.SQR, "oooooouA AAAABBBB", Mnem.SQRCR_r_fr);
            Mnem.AddSubOp("SQR rr,fr", Mnem.SQR, "oooooouA AAAABBBB", Mnem.SQRCR_rr_fr);
            Mnem.AddSubOp("SQR rrr,fr", Mnem.SQR, "oooooouA AAAABBBB", Mnem.SQRCR_rrr_fr);
            Mnem.AddSubOp("SQR rrrr,fr", Mnem.SQR, "oooooouA AAAABBBB", Mnem.SQRCR_rrrr_fr);

            Mnem.AddSubOp("SQR (rrr),fr", Mnem.SQR, "oooooouA AAAABBBB", Mnem.SQRCR_IrrrI_fr);
            Mnem.AddSubOp("SQR (nnn),fr", Mnem.SQR, "oooooouu uuuuBBBB AAAAAAAA AAAAAAAA AAAAAAAA", Mnem.SQRCR_InnnI_fr);
        }
    }
}
