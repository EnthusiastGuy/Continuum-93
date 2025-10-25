namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInitVDL_VCL
    {
        public static void Initialize()
        {
            // VDL
            Mnem.AddSubOp("VDL nnn", Mnem.VDL, "oouuuuuu AAAAAAAA", Mnem.VDCL_n);
            Mnem.AddSubOp("VDL r", Mnem.VDL, "oouAAAAA", Mnem.VDCL_r);
            Mnem.AddSubOp("VDL (nnn)", Mnem.VDL, "oouuuuuu AAAAAAAA AAAAAAAA AAAAAAAA", Mnem.VDCL_InnnI);
            Mnem.AddSubOp("VDL (rrr)", Mnem.VDL, "oouAAAAA", Mnem.VDCL_IrrrI);

            // VCL
            Mnem.AddSubOp("VCL nnn", Mnem.VCL, "oouuuuuu AAAAAAAA", Mnem.VDCL_n);
            Mnem.AddSubOp("VCL r", Mnem.VCL, "oouAAAAA", Mnem.VDCL_r);
            Mnem.AddSubOp("VCL (nnn)", Mnem.VCL, "oouuuuuu AAAAAAAA AAAAAAAA AAAAAAAA", Mnem.VDCL_InnnI);
            Mnem.AddSubOp("VCL (rrr)", Mnem.VCL, "oouAAAAA", Mnem.VDCL_IrrrI);
        }
    }
}
