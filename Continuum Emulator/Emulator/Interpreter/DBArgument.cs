using Continuum93.Tools;
using System.Collections.Generic;
using System.Text;

namespace Continuum93.Emulator.Interpreter
{
    public class DBArgument
    {
        public string StringArgument;

        public string StringArg;
        public string LabelValue;
        private AssemblerStats _stats;

        private readonly List<byte> _data = new();

        public DBArgument(string argument, AssemblerStats stats)
        {
            _stats = stats;
            StringArgument = argument;
            ProcessArgument();
        }

        private void ProcessArgument()
        {
            // Check if this is a repetition macro
            if (DataConverter.HasValidRepetitionArgument(StringArgument))
            {
                byte[] repData = DataConverter.GetRepetitionData(StringArgument);
                foreach (byte b in repData)
                    _data.Add(b);

                return;
            }
            // Check if it is a string
            else if (DataConverter.HasStringContent(StringArgument))
            {
                StringArg = DataConverter.GetStringContents(StringArgument);
                if (StringArg != null)
                {
                    byte[] bStr = Encoding.ASCII.GetBytes(StringArg);
                    foreach (byte b in bStr)
                        _data.Add(b);

                    return;
                }
            }
            // Check if it is a numeric value - implement negative numbers
            else if (DataConverter.TryToGetValueOf(StringArgument.Replace("_", "")).HasValue)
            {
                byte[] values = DataConverter.TryToGetBytesValueOf(StringArgument.Replace("_", ""));

                if (values.Length == 0)
                {
                    CompileLog.Log(
                    string.Format("Could not convert #db value {0} into bytes.", StringArgument), CompileIssue.Error);
                    return;
                }

                foreach (byte b in values)
                    _data.Add(b);

                return;
            }
            // Check if it a floating number
            else if (DataConverter.TryToGetFloatValueOf(StringArgument.Replace("_", "")) != null)
            {
                float floatNumber = DataConverter.TryToGetFloatValueOf(StringArgument.Replace("_", "")).GetValueOrDefault();
                byte[] values = FloatPointUtils.FloatToBytes(floatNumber);

                foreach (byte b in values)
                    _data.Add(b);

            }
            // Check if it is an absolute or relative label
            else if (StringArgument.Length >= 2 && StringArgument[0] == '.')    // || items[i][0] == '~' // We ignore the relative for now
            {
                LabelValue = StringArgument;
                RefreshLabelValue();
            }
            else if (StringArgument == "\"\"")
            {
                // Tolerated, will simply not compile any bytes
                _data.Clear();
            }
            else
            {
                CompileLog.Log(
                    string.Format("Could not interpret #db value {0}.", StringArgument), CompileIssue.Error);

                _data.Clear();  // Maybe not necessarily clear it. Seems pointless unless we debug something
                return;
            }
        }

        public void RefreshLabelValue()
        {
            _data.Clear();
            uint value = _stats.Labels[LabelValue];
            byte[] bValues = DataConverter.GetBytesFrom24bit(value);

            foreach (byte b in bValues)
                _data.Add(b);
            return;
        }

        public bool HasLabelData()
        {
            return !string.IsNullOrEmpty(LabelValue);
        }

        public List<byte> GetData()
        {
            return _data;
        }
    }
}
