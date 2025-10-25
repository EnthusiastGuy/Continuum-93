namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInitSCP
    {
        public static void Initialize()
        {
            // SCP
            Mnem.AddSubOp("SCP r,nnn", Mnem.SCP, "oooooouu uuuAAAAA BBBBBBBB", Mnem.CP_r_n);
            Mnem.AddSubOp("SCP r,r", Mnem.SCP, "ooooooAA AAABBBBB", Mnem.CP_r_r);
            Mnem.AddSubOp("SCP rr,nnn", Mnem.SCP, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB", Mnem.CP_rr_nn);
            Mnem.AddSubOp("SCP rr,rr", Mnem.SCP, "ooooooAA AAABBBBB", Mnem.CP_rr_rr);
            Mnem.AddSubOp("SCP rrr,nnn", Mnem.SCP, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.CP_rrr_nnn);
            Mnem.AddSubOp("SCP rrr,rrr", Mnem.SCP, "ooooooAA AAABBBBB", Mnem.CP_rrr_rrr);
            Mnem.AddSubOp("SCP rrrr,nnn", Mnem.SCP, "oooooouu uuuAAAAA BBBBBBBB BBBBBBBB BBBBBBBB BBBBBBBB", Mnem.CP_rrrr_nnnn);
            Mnem.AddSubOp("SCP rrrr,rrrr", Mnem.SCP, "ooooooAA AAABBBBB", Mnem.CP_rrrr_rrrr);

            Mnem.AddSubOp("SCP r,(rrr)", Mnem.SCP, "ooooooAA AAABBBBB", Mnem.CP_r_IrrrI);
            Mnem.AddSubOp("SCP rr,(rrr)", Mnem.SCP, "ooooooAA AAABBBBB", Mnem.CP_rr_IrrrI);
            Mnem.AddSubOp("SCP rrr,(rrr)", Mnem.SCP, "ooooooAA AAABBBBB", Mnem.CP_rrr_IrrrI);
            Mnem.AddSubOp("SCP rrrr,(rrr)", Mnem.SCP, "ooooooAA AAABBBBB", Mnem.CP_rrrr_IrrrI);
            Mnem.AddSubOp("SCP (rrr),(rrr)", Mnem.SCP, "ooooooAA AAABBBBB", Mnem.CP_IrrrI_IrrrI);
            Mnem.AddSubOp("SCP (rrr),nnn", Mnem.SCP, "oooooouu uuuAAAAA BBBBBBBB", Mnem.CP_IrrrI_nnn);
        }
    }
}
