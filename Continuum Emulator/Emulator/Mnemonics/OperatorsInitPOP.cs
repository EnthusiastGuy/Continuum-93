namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInitPOP
    {
        public static void Initialize()
        {
            Mnem.AddSubOp("POP r", Mnem.POP, "oooooooo uuuAAAAA", Mnem.POPU_r);
            Mnem.AddSubOp("POP rr", Mnem.POP, "oooooooo uuuAAAAA", Mnem.POPU_rr);
            Mnem.AddSubOp("POP rrr", Mnem.POP, "oooooooo uuuAAAAA", Mnem.POPU_rrr);
            Mnem.AddSubOp("POP rrrr", Mnem.POP, "oooooooo uuuAAAAA", Mnem.POPU_rrrr);
            Mnem.AddSubOp("POP r,r", Mnem.POP, "oooooooo uuuAAAAA uuuBBBBB", Mnem.POPU_r_r);

            Mnem.AddSubOp("POP fr", Mnem.POP, "oooooooo uuuuAAAA", Mnem.POPU_fr);
            Mnem.AddSubOp("POP fr,fr", Mnem.POP, "oooooooo AAAABBBB", Mnem.POPU_fr_fr);

            Mnem.AddSubOp("POP (nnn)", Mnem.POP, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA", Mnem.POPU_InnnI);
            Mnem.AddSubOp("POP (rrr)", Mnem.POP, "oooooooo uuuAAAAA", Mnem.POPU_IrrrI);

            Mnem.AddSubOp("POP16 (nnn)", Mnem.POP16, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA", Mnem.POPU16_InnnI);
            Mnem.AddSubOp("POP16 (rrr)", Mnem.POP16, "oooooooo uuuAAAAA", Mnem.POPU16_IrrrI);

            Mnem.AddSubOp("POP24 (nnn)", Mnem.POP24, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA", Mnem.POPU24_InnnI);
            Mnem.AddSubOp("POP24 (rrr)", Mnem.POP24, "oooooooo uuuAAAAA", Mnem.POPU24_IrrrI);

            Mnem.AddSubOp("POP32 (nnn)", Mnem.POP32, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA", Mnem.POPU32_InnnI);
            Mnem.AddSubOp("POP32 (rrr)", Mnem.POP32, "oooooooo uuuAAAAA", Mnem.POPU32_IrrrI);
        }
    }
}
