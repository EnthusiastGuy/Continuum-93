namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInitDEC
    {
        public static void Initialize()
        {
            // DEC
            Mnem.AddSubOp("DEC r", Mnem.DEC, "oooooooo uuuAAAAA", Mnem.INCDEC_r);
            Mnem.AddSubOp("DEC rr", Mnem.DEC, "oooooooo uuuAAAAA", Mnem.INCDEC_rr);
            Mnem.AddSubOp("DEC rrr", Mnem.DEC, "oooooooo uuuAAAAA", Mnem.INCDEC_rrr);
            Mnem.AddSubOp("DEC rrrr", Mnem.DEC, "oooooooo uuuAAAAA", Mnem.INCDEC_rrrr);

            Mnem.AddSubOp("DEC (rrr)", Mnem.DEC, "oooooooo uuuAAAAA", Mnem.INCDEC_IrrrI);
            Mnem.AddSubOp("DEC16 (rrr)", Mnem.DEC, "oooooooo uuuAAAAA", Mnem.INCDEC16_IrrrI);
            Mnem.AddSubOp("DEC24 (rrr)", Mnem.DEC, "oooooooo uuuAAAAA", Mnem.INCDEC24_IrrrI);
            Mnem.AddSubOp("DEC32 (rrr)", Mnem.DEC, "oooooooo uuuAAAAA", Mnem.INCDEC32_IrrrI);

            Mnem.AddSubOp("DEC (nnn)", Mnem.DEC, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA", Mnem.INCDEC_InnnI);
            Mnem.AddSubOp("DEC16 (nnn)", Mnem.DEC, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA", Mnem.INCDEC16_InnnI);
            Mnem.AddSubOp("DEC24 (nnn)", Mnem.DEC, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA", Mnem.INCDEC24_InnnI);
            Mnem.AddSubOp("DEC32 (nnn)", Mnem.DEC, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA", Mnem.INCDEC32_InnnI);
        }
    }
}
