namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInitCALL_JUMP
    {
        public static void Initialize()
        {
            // CALL, JUMP
            Mnem.AddSubOp("CALL nnn", Mnem.CALL, "ooouuuuu AAAAAAAA AAAAAAAA AAAAAAAA", Mnem.CLRJ_nnn);
            Mnem.AddSubOp("CALL ff,nnn", Mnem.CALL, "oooAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.CLRJ_ff_nnn);
            Mnem.AddSubOp("CALL rrr", Mnem.CALL, "oooAAAAA", Mnem.CLRJ_rrr);
            Mnem.AddSubOp("CALL ff,rrr", Mnem.CALL, "oooAAAAA uuuBBBBB", Mnem.CLRJ_ff_rrr);

            Mnem.AddSubOp("CALLR nnn", Mnem.CALLR, "ooouuuuu AAAAAAAA AAAAAAAA AAAAAAAA", Mnem.CLRJ_nnn);
            Mnem.AddSubOp("CALLR ff,nnn", Mnem.CALLR, "oooAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.CLRJ_ff_nnn);

            Mnem.AddSubOp("JP nnn", Mnem.JP, "ooouuuuu AAAAAAAA AAAAAAAA AAAAAAAA", Mnem.CLRJ_nnn);
            Mnem.AddSubOp("JP ff,nnn", Mnem.JP, "oooAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.CLRJ_ff_nnn);
            Mnem.AddSubOp("JP rrr", Mnem.JP, "oooAAAAA", Mnem.CLRJ_rrr);
            Mnem.AddSubOp("JP ff,rrr", Mnem.JP, "oooAAAAA uuuBBBBB", Mnem.CLRJ_ff_rrr);

            Mnem.AddSubOp("JR nnn", Mnem.JR, "ooouuuuu AAAAAAAA AAAAAAAA AAAAAAAA", Mnem.CLRJ_nnn);
            Mnem.AddSubOp("JR ff,nnn", Mnem.JR, "oooAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.CLRJ_ff_nnn);
        }
    }
}
