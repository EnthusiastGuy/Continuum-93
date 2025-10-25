namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInitPUSH
    {
        public static void Initialize()
        {
            Mnem.AddSubOp("PUSH r", Mnem.PUSH, "oooooooo uuuAAAAA", Mnem.POPU_r);
            Mnem.AddSubOp("PUSH rr", Mnem.PUSH, "oooooooo uuuAAAAA", Mnem.POPU_rr);
            Mnem.AddSubOp("PUSH rrr", Mnem.PUSH, "oooooooo uuuAAAAA", Mnem.POPU_rrr);
            Mnem.AddSubOp("PUSH rrrr", Mnem.PUSH, "oooooooo uuuAAAAA", Mnem.POPU_rrrr);
            Mnem.AddSubOp("PUSH r,r", Mnem.PUSH, "oooooooo uuuAAAAA uuuBBBBB", Mnem.POPU_r_r);

            Mnem.AddSubOp("PUSH fr", Mnem.PUSH, "oooooooo uuuuAAAA", Mnem.POPU_fr);
            Mnem.AddSubOp("PUSH fr,fr", Mnem.PUSH, "oooooooo AAAABBBB", Mnem.POPU_fr_fr);

            Mnem.AddSubOp("PUSH (nnn)", Mnem.PUSH, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA", Mnem.POPU_InnnI);
            Mnem.AddSubOp("PUSH (rrr)", Mnem.PUSH, "oooooooo uuuAAAAA", Mnem.POPU_IrrrI);

            Mnem.AddSubOp("PUSH16 (nnn)", Mnem.PUSH16, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA", Mnem.POPU16_InnnI);
            Mnem.AddSubOp("PUSH16 (rrr)", Mnem.PUSH16, "oooooooo uuuAAAAA", Mnem.POPU16_IrrrI);

            Mnem.AddSubOp("PUSH24 (nnn)", Mnem.PUSH24, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA", Mnem.POPU24_InnnI);
            Mnem.AddSubOp("PUSH24 (rrr)", Mnem.PUSH24, "oooooooo uuuAAAAA", Mnem.POPU24_IrrrI);

            Mnem.AddSubOp("PUSH32 (nnn)", Mnem.PUSH32, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA", Mnem.POPU32_InnnI);
            Mnem.AddSubOp("PUSH32 (rrr)", Mnem.PUSH32, "oooooooo uuuAAAAA", Mnem.POPU32_IrrrI);

        }
    }
}
