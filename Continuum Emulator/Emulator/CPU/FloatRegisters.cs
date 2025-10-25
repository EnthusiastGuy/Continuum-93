using Continuum93.Tools;
using System;
using System.Text;

namespace Continuum93.Emulator.CPU
{
    public class FloatRegisters
    {
        private byte registerBank = 0;
        public readonly float[][] _fregs;
        private Computer _computer;

        public FloatRegisters(Computer computer)
        {
            _computer = computer;
            _fregs = new float[256][];
            for (int bank = 0; bank < 256; bank++)
                _fregs[bank] = new float[16];
        }

        public void SetRegisterBank(byte value)
        {
            registerBank = value;
        }

        public byte GetRegisterBank()
        {
            return registerBank;
        }

        public float GetRegister(byte index)
        {
            return _fregs[registerBank][index];
        }

        public void SetRegister(byte index, float value)
        {
            _fregs[registerBank][index] = value;
        }

        public string GetDebugInfo()
        {
            string response = string.Empty;

            for (byte i = 0; i < 16; i++)
                response += $"F{i}:" + GetRegister(i) + Environment.NewLine;

            return response;
        }

        public string GetDebugTemplate()
        {
            string response = "";

            for (byte i = 0; i < 16; i++)
                response += $"F{i}:" + "{" + i + "}" + Environment.NewLine;

            return response;
        }

        public void ClearAll()
        {
            for (int bank = 0; bank < _fregs.Length; bank++)
            {
                Array.Clear(_fregs[bank], 0, _fregs[bank].Length);
            }
        }

        public float AddFloatValues(float v1, float v2)
        {
            _computer.CPU.FLAGS.UpdateFloatAddFlags(v1, v2);
            float sum = v1 + v2;
            if (sum <= float.MaxValue && sum >= float.MinValue)
                return sum;

            return v1;
        }

        public float SubFloatValues(float v1, float v2)
        {
            _computer.CPU.FLAGS.UpdateFloatSubFlags(v1, v2);
            float diff = v1 - v2;
            if (diff <= float.MaxValue && diff >= float.MinValue)
                return diff;

            return v1;
        }

        public void ExchangeRegisters(byte fIndex1, byte fIndex2)
        {
            float[] regs = _fregs[registerBank];
            float temp = regs[fIndex1];
            regs[fIndex1] = regs[fIndex2];
            regs[fIndex2] = temp;
        }

        public byte[] GetCurrentRegisterPageData()
        {
            byte[] response = new byte[64];

            for (int i = 0; i < 16; i++)
            {
                byte[] rawFloat = FloatPointUtils.FloatToBytes(_fregs[registerBank][i]);
                response[i * 4] = rawFloat[0];
                response[i * 4 + 1] = rawFloat[1];
                response[i * 4 + 2] = rawFloat[2];
                response[i * 4 + 3] = rawFloat[3];
            }

            return response;
        }

        public string GetRegistersState()
        {
            StringBuilder response = new();

            for (byte i = 0; i < _fregs.Length; i++)
            {
                response.Append($"F{i}: {_fregs[i]}{Environment.NewLine}");
            }

            return response.ToString();
        }
    }
}
