namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInitINC
    {
        public static void Initialize()
        {
            // INC
            Mnem.AddSubOp("INC r", Mnem.INC, "oooooooo uuuAAAAA", Mnem.INCDEC_r);
            Mnem.AddSubOp("INC rr", Mnem.INC, "oooooooo uuuAAAAA", Mnem.INCDEC_rr);
            Mnem.AddSubOp("INC rrr", Mnem.INC, "oooooooo uuuAAAAA", Mnem.INCDEC_rrr);
            Mnem.AddSubOp("INC rrrr", Mnem.INC, "oooooooo uuuAAAAA", Mnem.INCDEC_rrrr);

            Mnem.AddSubOp("INC (rrr)", Mnem.INC, "oooooooo uuuAAAAA", Mnem.INCDEC_IrrrI);
            Mnem.AddSubOp("INC16 (rrr)", Mnem.INC, "oooooooo uuuAAAAA", Mnem.INCDEC16_IrrrI);
            Mnem.AddSubOp("INC24 (rrr)", Mnem.INC, "oooooooo uuuAAAAA", Mnem.INCDEC24_IrrrI);
            Mnem.AddSubOp("INC32 (rrr)", Mnem.INC, "oooooooo uuuAAAAA", Mnem.INCDEC32_IrrrI);

            Mnem.AddSubOp("INC (nnn)", Mnem.INC, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA", Mnem.INCDEC_InnnI);
            Mnem.AddSubOp("INC16 (nnn)", Mnem.INC, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA", Mnem.INCDEC16_InnnI);
            Mnem.AddSubOp("INC24 (nnn)", Mnem.INC, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA", Mnem.INCDEC24_InnnI);
            Mnem.AddSubOp("INC32 (nnn)", Mnem.INC, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA", Mnem.INCDEC32_InnnI);
        }
    }
}
