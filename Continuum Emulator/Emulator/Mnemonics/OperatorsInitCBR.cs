namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInitCBR
    {
        public static void Initialize()
        {
            // CBR
            Mnem.AddSubOp("CBR fr", Mnem.CBR, "oooooouu uuuuAAAA", Mnem.SQRCR_fr);
            Mnem.AddSubOp("CBR r", Mnem.CBR, "oooooouu uuuAAAAA", Mnem.SQRCR_r);
            Mnem.AddSubOp("CBR rr", Mnem.CBR, "oooooouu uuuAAAAA", Mnem.SQRCR_rr);
            Mnem.AddSubOp("CBR rrr", Mnem.CBR, "oooooouu uuuAAAAA", Mnem.SQRCR_rrr);
            Mnem.AddSubOp("CBR rrrr", Mnem.CBR, "oooooouu uuuAAAAA", Mnem.SQRCR_rrrr);
            Mnem.AddSubOp("CBR (rrr)", Mnem.CBR, "oooooouu uuuAAAAA", Mnem.SQRCR_IrrrI);
            Mnem.AddSubOp("CBR (nnn)", Mnem.CBR, "oooooouu AAAAAAAA AAAAAAAA AAAAAAAA", Mnem.SQRCR_InnnI);

            Mnem.AddSubOp("CBR fr,fr", Mnem.CBR, "oooooouu AAAABBBB", Mnem.SQRCR_fr_fr);
            Mnem.AddSubOp("CBR fr,r", Mnem.CBR, "oooooouB BBBBAAAA", Mnem.SQRCR_fr_r);
            Mnem.AddSubOp("CBR fr,rr", Mnem.CBR, "oooooouB BBBBAAAA", Mnem.SQRCR_fr_rr);
            Mnem.AddSubOp("CBR fr,rrr", Mnem.CBR, "oooooouB BBBBAAAA", Mnem.SQRCR_fr_rrr);
            Mnem.AddSubOp("CBR fr,rrrr", Mnem.CBR, "oooooouB BBBBAAAA", Mnem.SQRCR_fr_rrrr);

            Mnem.AddSubOp("CBR fr,(rrr)", Mnem.CBR, "oooooouB BBBBAAAA", Mnem.SQRCR_fr_IrrrI);
            Mnem.AddSubOp("CBR fr,(nnn)", Mnem.CBR, "oooooouu uuuuAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.SQRCR_fr_InnnI);
            Mnem.AddSubOp("CBR fr,nnn", Mnem.CBR, "oooooouu uuuuAAAA BBBBBBBB BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.SQRCR_fr_n);

            Mnem.AddSubOp("CBR r,fr", Mnem.CBR, "oooooouA AAAABBBB", Mnem.SQRCR_r_fr);
            Mnem.AddSubOp("CBR rr,fr", Mnem.CBR, "oooooouA AAAABBBB", Mnem.SQRCR_rr_fr);
            Mnem.AddSubOp("CBR rrr,fr", Mnem.CBR, "oooooouA AAAABBBB", Mnem.SQRCR_rrr_fr);
            Mnem.AddSubOp("CBR rrrr,fr", Mnem.CBR, "oooooouA AAAABBBB", Mnem.SQRCR_rrrr_fr);

            Mnem.AddSubOp("CBR (rrr),fr", Mnem.CBR, "oooooouA AAAABBBB", Mnem.SQRCR_IrrrI_fr);
            Mnem.AddSubOp("CBR (nnn),fr", Mnem.CBR, "oooooouu uuuuBBBB AAAAAAAA AAAAAAAA AAAAAAAA", Mnem.SQRCR_InnnI_fr);
        }
    }
}
