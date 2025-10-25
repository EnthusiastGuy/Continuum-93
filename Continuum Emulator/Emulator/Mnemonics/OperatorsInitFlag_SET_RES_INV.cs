namespace Continuum93.Emulator.Mnemonics
{
    public static class OperatorsInitFlag_SET_RES_INV
    {
        public static void Initialize()
        {
            // SETF
            Mnem.AddSubOp("SETF ff", Mnem.SETF, "ouuAAAAA", Mnem.STRSIVF_ff);
            // RESF
            Mnem.AddSubOp("RESF ff", Mnem.RESF, "ouuAAAAA", Mnem.STRSIVF_ff);
            // INVF
            Mnem.AddSubOp("INVF ff", Mnem.INVF, "ouuAAAAA", Mnem.STRSIVF_ff);
        }
    }
}
