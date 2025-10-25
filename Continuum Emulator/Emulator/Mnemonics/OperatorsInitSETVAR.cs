namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInitSETVAR
    {
        public static void Initialize()
        {
            Mnem.AddSubOp("SETVAR nnn,nnn", Mnem.SETVAR, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.SGVAR_n_n);
            Mnem.AddSubOp("SETVAR nnn,rrrr", Mnem.SETVAR, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.SGVAR_n_rrrr);
            Mnem.AddSubOp("SETVAR rrrr,nnn", Mnem.SETVAR, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.SGVAR_rrrr_n);
            Mnem.AddSubOp("SETVAR rrrr,rrrr", Mnem.SETVAR, "oooooooo uuuAAAAA uuuBBBBB", Mnem.SGVAR_rrrr_rrrr);
        }
    }
}
