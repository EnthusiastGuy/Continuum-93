public class StackData
{
    public uint SPR { get; set; }
    public uint SPC { get; set; }

    private byte[] _regStackData;
    public byte[] RegStackData
    {
        get { return _regStackData; }
        set
        {
            _regStackData = value;
            RegStackCount = value != null ? value.Length : 0;
        }
    }

    public int RegStackCount { get; private set; }

    private uint[] _callStackData;
    public uint[] CallStackData
    {
        get { return _callStackData; }
        set
        {
            _callStackData = value;
            CallStackCount = value != null ? value.Length : 0;
        }
    }

    public int CallStackCount { get; private set; }
}
