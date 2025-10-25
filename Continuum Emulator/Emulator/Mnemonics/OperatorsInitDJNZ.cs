namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInitDJNZ
    {
        public static void Initialize()
        {
            Mnem.AddSubOp("DJNZ r,nnn", Mnem.DJNZ, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.DJNZ_r_nnn);
            Mnem.AddSubOp("DJNZ r,rrr", Mnem.DJNZ, "oooooooo uuuAAAAA uuuBBBBB", Mnem.DJNZ_r_rrr);

            Mnem.AddSubOp("DJNZ rr,nnn", Mnem.DJNZ, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.DJNZ_rr_nnn);
            Mnem.AddSubOp("DJNZ rr,rrr", Mnem.DJNZ, "oooooooo uuuAAAAA uuuBBBBB", Mnem.DJNZ_rr_rrr);

            Mnem.AddSubOp("DJNZ rrr,nnn", Mnem.DJNZ, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.DJNZ_rrr_nnn);
            Mnem.AddSubOp("DJNZ rrr,rrr", Mnem.DJNZ, "oooooooo uuuAAAAA uuuBBBBB", Mnem.DJNZ_rrr_rrr);

            Mnem.AddSubOp("DJNZ rrrr,nnn", Mnem.DJNZ, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.DJNZ_rrrr_nnn);
            Mnem.AddSubOp("DJNZ rrrr,rrr", Mnem.DJNZ, "oooooooo uuuAAAAA uuuBBBBB", Mnem.DJNZ_rrrr_rrr);

            Mnem.AddSubOp("DJNZ (nnn),nnn", Mnem.DJNZ, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.DJNZ_InnnI_nnn);
            Mnem.AddSubOp("DJNZ (nnn),rrr", Mnem.DJNZ, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.DJNZ_InnnI_rrr);

            Mnem.AddSubOp("DJNZ16 (nnn),nnn", Mnem.DJNZ, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.DJNZ16_InnnI_nnn);
            Mnem.AddSubOp("DJNZ16 (nnn),rrr", Mnem.DJNZ, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.DJNZ16_InnnI_rrr);

            Mnem.AddSubOp("DJNZ24 (nnn),nnn", Mnem.DJNZ, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.DJNZ24_InnnI_nnn);
            Mnem.AddSubOp("DJNZ24 (nnn),rrr", Mnem.DJNZ, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.DJNZ24_InnnI_rrr);

            Mnem.AddSubOp("DJNZ32 (nnn),nnn", Mnem.DJNZ, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.DJNZ32_InnnI_nnn);
            Mnem.AddSubOp("DJNZ32 (nnn),rrr", Mnem.DJNZ, "oooooooo AAAAAAAA AAAAAAAA AAAAAAAA uuuBBBBB", Mnem.DJNZ32_InnnI_rrr);

            Mnem.AddSubOp("DJNZ (rrr),nnn", Mnem.DJNZ, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.DJNZ_IrrrI_nnn);
            Mnem.AddSubOp("DJNZ (rrr),rrr", Mnem.DJNZ, "oooooooo uuuAAAAA uuuBBBBB", Mnem.DJNZ_IrrrI_rrr);

            Mnem.AddSubOp("DJNZ16 (rrr),nnn", Mnem.DJNZ, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.DJNZ16_IrrrI_nnn);
            Mnem.AddSubOp("DJNZ16 (rrr),rrr", Mnem.DJNZ, "oooooooo uuuAAAAA uuuBBBBB", Mnem.DJNZ16_IrrrI_rrr);

            Mnem.AddSubOp("DJNZ24 (rrr),nnn", Mnem.DJNZ, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.DJNZ24_IrrrI_nnn);
            Mnem.AddSubOp("DJNZ24 (rrr),rrr", Mnem.DJNZ, "oooooooo uuuAAAAA uuuBBBBB", Mnem.DJNZ24_IrrrI_rrr);

            Mnem.AddSubOp("DJNZ32 (rrr),nnn", Mnem.DJNZ, "oooooooo uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.DJNZ32_IrrrI_nnn);
            Mnem.AddSubOp("DJNZ32 (rrr),rrr", Mnem.DJNZ, "oooooooo uuuAAAAA uuuBBBBB", Mnem.DJNZ32_IrrrI_rrr);
        }
    }
}
