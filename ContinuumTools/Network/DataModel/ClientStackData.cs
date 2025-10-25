namespace ContinuumTools.Network.DataModel
{
    public class ClientStackData
    {
        public uint SPR { get; set; }
        public uint SPC { get; set; }
        public int RegStackCount { get; set; }
        public byte[] RegStackData { get; set; }
        public int CallStackCount { get; set; }
        public uint[] CallStackData { get; set; }
    }
}
