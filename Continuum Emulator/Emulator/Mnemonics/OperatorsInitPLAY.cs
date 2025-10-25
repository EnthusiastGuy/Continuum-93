namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInitPLAY
    {
        public static void Initialize()
        {
            Mnem.AddSubOp("PLAY nnn", Mnem.PLAY, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA", Mnem.PLAY_nnn);
            Mnem.AddSubOp("PLAY rrr", Mnem.PLAY, "oooooooo uuuAAAAA", Mnem.PLAY_rrr);
        }
    }
}
