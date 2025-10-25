namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInitPOW
    {
        public static void Initialize()
        {
            // POW
            Mnem.AddSubOp("POW fr,fr", Mnem.POW, "oooooouu AAAABBBB", Mnem.POW_fr_fr);
            Mnem.AddSubOp("POW fr,r", Mnem.POW, "oooooouB BBBBAAAA", Mnem.POW_fr_r);
            Mnem.AddSubOp("POW fr,rr", Mnem.POW, "oooooouB BBBBAAAA", Mnem.POW_fr_rr);
            Mnem.AddSubOp("POW fr,rrr", Mnem.POW, "oooooouB BBBBAAAA", Mnem.POW_fr_rrr);
            Mnem.AddSubOp("POW fr,rrrr", Mnem.POW, "oooooouB BBBBAAAA", Mnem.POW_fr_rrrr);

            Mnem.AddSubOp("POW r,fr", Mnem.POW, "oooooouA AAAABBBB", Mnem.POW_r_fr);
            Mnem.AddSubOp("POW rr,fr", Mnem.POW, "oooooouA AAAABBBB", Mnem.POW_rr_fr);
            Mnem.AddSubOp("POW rrr,fr", Mnem.POW, "oooooouA AAAABBBB", Mnem.POW_rrr_fr);
            Mnem.AddSubOp("POW rrrr,fr", Mnem.POW, "oooooouA AAAABBBB", Mnem.POW_rrrr_fr);

            Mnem.AddSubOp("POW fr,(rrr)", Mnem.POW, "oooooouB BBBBAAAA", Mnem.POW_fr_IrrrI);
            Mnem.AddSubOp("POW (rrr),fr", Mnem.POW, "oooooouA AAAABBBB", Mnem.POW_IrrrI_fr);

            Mnem.AddSubOp("POW fr,nnn", Mnem.POW, "oooooouu AAAAuuuu BBBBBBBB BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.POW_fr_nnn);

            Mnem.AddSubOp("POW fr,(nnn)", Mnem.POW, "oooooouu AAAAuuuu BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.POW_fr_InnnI);
            Mnem.AddSubOp("POW (nnn),fr", Mnem.POW, "oooooouu BBBBuuuu AAAAAAAA AAAAAAAA AAAAAAAA", Mnem.POW_InnnI_fr);
        }
    }
}
