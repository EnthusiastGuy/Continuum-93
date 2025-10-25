namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInit_MATH
    {
        public static void Initialize()
        {
            // ABS
            Mnem.AddSubOp("ABS fr", Mnem.ABS, "oooooooo uuuuAAAA", Mnem.SINCTC_fr);
            Mnem.AddSubOp("ABS fr,fr", Mnem.ABS, "oooooooo uuuuAAAA uuuuBBBB", Mnem.SINCTC_fr_fr);

            // ROUND
            Mnem.AddSubOp("ROUND fr", Mnem.ROUND, "oooooooo uuuuAAAA", Mnem.SINCTC_fr);
            Mnem.AddSubOp("ROUND fr,fr", Mnem.ROUND, "oooooooo uuuuAAAA uuuuBBBB", Mnem.SINCTC_fr_fr);

            // FLOOR
            Mnem.AddSubOp("FLOOR fr", Mnem.FLOOR, "oooooooo uuuuAAAA", Mnem.SINCTC_fr);
            Mnem.AddSubOp("FLOOR fr,fr", Mnem.FLOOR, "oooooooo uuuuAAAA uuuuBBBB", Mnem.SINCTC_fr_fr);

            // CEIL
            Mnem.AddSubOp("CEIL fr", Mnem.CEIL, "oooooooo uuuuAAAA", Mnem.SINCTC_fr);
            Mnem.AddSubOp("CEIL fr,fr", Mnem.CEIL, "oooooooo uuuuAAAA uuuuBBBB", Mnem.SINCTC_fr_fr);
        }
    }
}
