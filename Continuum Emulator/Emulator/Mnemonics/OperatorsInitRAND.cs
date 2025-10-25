namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInitRAND
    {
        public static void Initialize()
        {
            Mnem.AddSubOp("RAND r", Mnem.RAND, "oooooooo uuuAAAAA", Mnem.RAND_r);
            Mnem.AddSubOp("RAND rr", Mnem.RAND, "oooooooo uuuAAAAA", Mnem.RAND_rr);
            Mnem.AddSubOp("RAND rrr", Mnem.RAND, "oooooooo uuuAAAAA", Mnem.RAND_rrr);
            Mnem.AddSubOp("RAND rrrr", Mnem.RAND, "oooooooo uuuAAAAA", Mnem.RAND_rrrr);

            Mnem.AddSubOp("RAND r,nnn", Mnem.RAND, "oooooooo uuuAAAAA BBBBBBBB", Mnem.RAND_r_n);
            Mnem.AddSubOp("RAND rr,nnn", Mnem.RAND, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB", Mnem.RAND_rr_nn);
            Mnem.AddSubOp("RAND rrr,nnn", Mnem.RAND, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.RAND_rrr_nnn);
            Mnem.AddSubOp("RAND rrrr,nnn", Mnem.RAND, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.RAND_rrrr_nnnn);

            Mnem.AddSubOp("RAND fr", Mnem.RAND, "oooooooo uuuuAAAA", Mnem.RAND_fr);
            Mnem.AddSubOp("RAND fr,nnn", Mnem.RAND, "oooooooo uuuuAAAA BBBBBBBB BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.RAND_fr_nnnn);
            Mnem.AddSubOp("RAND fr,rrrr", Mnem.RAND, "oooooooo uuuuAAAA uuuBBBBB", Mnem.RAND_fr_rrrr);
        }
    }
}
