namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInitGETVAR
    {
        public static void Initialize()
        {
            Mnem.AddSubOp("GETVAR nnn,rrrr", Mnem.GETVAR, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.SGVAR_n_rrrr);
            Mnem.AddSubOp("GETVAR rrrr,rrrr", Mnem.GETVAR, "oooooooo uuuAAAAA uuuBBBBB", Mnem.SGVAR_rrrr_rrrr);
        }
    }
}
