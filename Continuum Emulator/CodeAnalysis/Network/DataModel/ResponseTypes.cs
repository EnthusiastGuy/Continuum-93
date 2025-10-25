namespace Continuum93.CodeAnalysis.Network.DataModel
{
    using System;

    [Serializable]
    public class Operation
    {
        public string Oper { get; set; } = string.Empty;

        public string Time { get; set; } = string.Empty;

        public byte[] Data { get; set; } = [];

        public string TextData { get; set; } = string.Empty;

        public string GetString()
        {
            string dataString = Data != null && Data.Length > 0
                ? BitConverter.ToString(Data)
                : "No Data";

            return $"Operation Details:\n" +
                   $"- Oper: {Oper}\n" +
                   $"- Time: {Time}\n" +
                   $"- Data: {dataString}\n" +
                   $"- TextData: {TextData}";
        }
    }
}
