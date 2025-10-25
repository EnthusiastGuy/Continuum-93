namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInitMEMF_MEMC
    {
        public static void Initialize()
        {
            // MEMF
            Mnem.AddSubOp("MEMF rrr,rrr,r", Mnem.MEMF, "oooAAAAA uuuBBBBB uuuCCCCC", Mnem.MEMF_rrr_rrr_r);
            Mnem.AddSubOp("MEMF nnn,rrr,r", Mnem.MEMF, "oooBBBBB uuuCCCCC AAAAAAAA AAAAAAAA AAAAAAAA", Mnem.MEMF_nnn_rrr_r);
            Mnem.AddSubOp("MEMF rrr,nnn,r", Mnem.MEMF, "oooAAAAA uuuCCCCC BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.MEMF_rrr_nnn_r);
            Mnem.AddSubOp("MEMF nnn,nnn,r", Mnem.MEMF, "oooCCCCC AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.MEMF_nnn_nnn_r);

            Mnem.AddSubOp("MEMF rrr,rrr,nnn", Mnem.MEMF, "oooAAAAA uuuBBBBB CCCCCCCC", Mnem.MEMF_rrr_rrr_n);
            Mnem.AddSubOp("MEMF nnn,rrr,nnn", Mnem.MEMF, "oooBBBBB AAAAAAAA AAAAAAAA AAAAAAAA CCCCCCCC", Mnem.MEMF_nnn_rrr_n);
            Mnem.AddSubOp("MEMF rrr,nnn,nnn", Mnem.MEMF, "oooAAAAA BBBBBBBB BBBBBBBB BBBBBBBB CCCCCCCC", Mnem.MEMF_rrr_nnn_n);
            Mnem.AddSubOp("MEMF nnn,nnn,nnn", Mnem.MEMF, "ooouuuuu AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB BBBBBBBB BBBBBBBB CCCCCCCC", Mnem.MEMF_nnn_nnn_n);

            // MEMC
            Mnem.AddSubOp("MEMC rrr,rrr,rrr", Mnem.MEMC, "oooAAAAA uuuBBBBB uuuCCCCC", Mnem.MEMC_rrr_rrr_rrr);
            Mnem.AddSubOp("MEMC nnn,rrr,rrr", Mnem.MEMC, "oooBBBBB uuuCCCCC AAAAAAAA AAAAAAAA AAAAAAAA", Mnem.MEMC_nnn_rrr_rrr);
            Mnem.AddSubOp("MEMC rrr,nnn,rrr", Mnem.MEMC, "oooAAAAA uuuCCCCC BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.MEMC_rrr_nnn_rrr);
            Mnem.AddSubOp("MEMC nnn,nnn,rrr", Mnem.MEMC, "oooCCCCC AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.MEMC_nnn_nnn_rrr);

            Mnem.AddSubOp("MEMC rrr,rrr,nnn", Mnem.MEMC, "oooAAAAA uuuBBBBB CCCCCCCC CCCCCCCC CCCCCCCC", Mnem.MEMC_rrr_rrr_nnn);
            Mnem.AddSubOp("MEMC nnn,rrr,nnn", Mnem.MEMC, "oooBBBBB AAAAAAAA AAAAAAAA AAAAAAAA CCCCCCCC CCCCCCCC CCCCCCCC", Mnem.MEMC_nnn_rrr_nnn);
            Mnem.AddSubOp("MEMC rrr,nnn,nnn", Mnem.MEMC, "oooAAAAA BBBBBBBB BBBBBBBB BBBBBBBB CCCCCCCC CCCCCCCC CCCCCCCC", Mnem.MEMC_rrr_nnn_nnn);
            Mnem.AddSubOp("MEMC nnn,nnn,nnn", Mnem.MEMC, "ooouuuuu AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB BBBBBBBB BBBBBBBB CCCCCCCC CCCCCCCC CCCCCCCC", Mnem.MEMC_nnn_nnn_nnn);
        }
    }
}
