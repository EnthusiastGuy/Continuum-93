namespace Continuum93.Emulator.Mnemonics
{
    using Continuum93.Emulator.CPU;

    public static class OperatorsInitADD
    {
        public static void Initialize()
        {
            // Supplementary aliases to match legacy general forms that still use
            // the “nnn” placeholder naming. They map to the new shared
            // instruction set sub-ops defined in Instructions.
            AddAliasIfMissing("ADD r,nnn", Mnem.ADD, "oooooooo uuuAAAAA BBBBBBBB", Instructions._r_n);
            AddAliasIfMissing("ADD rr,nnn", Mnem.ADD, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB", Instructions._rr_nn);
            AddAliasIfMissing("ADD fr,nnn", Mnem.ADD, "oooooooo uuuuAAAA BBBBBBBB BBBBBBBB BBBBBBBB BBBBBBBB", Instructions._fr_nnnn);
            AddAliasIfMissing("ADD r,fr", Mnem.ADD, "oooooooo uuuAAAAA uuuuBBBB", Instructions._r_fr);
            AddAliasIfMissing("ADD fr,r", Mnem.ADD, "oooooooo uuuuAAAA uuuBBBBB", Instructions._fr_r);

            // Block memory variants with explicit count/repeat parameters
            AddAliasIfMissing("ADD (nnn),nnn,nnn", Mnem.ADD,
                "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB BBBBBBBB BBBBBBBB BBBBBBBB CCCCCCCC",
                Instructions._InnnI_nnnn_n);
            AddAliasIfMissing("ADD (nnn),nnn,nnn,nnn", Mnem.ADD,
                "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB BBBBBBBB BBBBBBBB BBBBBBBB CCCCCCCC DDDDDDDD DDDDDDDD DDDDDDDD",
                Instructions._InnnI_nnnn_n_nnn);
        }

        private static void AddAliasIfMissing(string key, byte parent, string format, byte subOp)
        {
            if (!Mnem.OPS.ContainsKey(key))
            {
                Mnem.AddSubOp(key, parent, format, subOp);
            }
        }
    }
}
