using ContinuumTools.Display.Views.Main;
using ContinuumTools.Network;
using ContinuumTools.Utils;
using System;

namespace ContinuumTools.States
{
    public static class CPUState
    {
        public static byte[] RegPageOld = new byte[26];
        public static byte[] RegPage0 = new byte[26];
        public static float[] FRegsOld = new float[16];
        public static float[] FRegs = new float[16];
        public static byte RegPageIndex = 0;

        public static byte OldFlags;
        public static byte Flags;

        public static byte[] RegMemoryData = new byte[26 * 16];

        public static int IPAddress;

        public static bool StepByStepMode = false;

        private static bool[] regModified = new bool[26];

        public static void Update()
        {
            if (Client.IsDisconnected())
            {
                StepByStepMode = false;
            }
        }

        public static void PushNewMemoryPointedByRegs(byte[] data)
        {
            RegMemoryData = data;
        }

        public static void PushNewRegs(byte[] newState)
        {
            //Console.WriteLine($"PushNewRegs received new state, newState: {newState.Length} items");

            for (int i = 0; i < 26; i++)
            {
                regModified[i] = newState[i] != RegPage0[i];
            }

            RegPageOld = RegPage0;
            RegPage0 = newState;

            RegisterView.UpdateRegs();
        }

        public static void PushNewFlags(byte[] newFlags)
        {
            OldFlags = Flags;
            Flags = newFlags[0];

            FlagView.UpdateFlags();
        }

        public static void PushNewFloatRegs(byte[] raw)
        {
            Array.Copy(FRegs, FRegsOld, FRegs.Length);

            for (int i = 0; i < 16; i++)
            {
                int idx = i * 4;
                FRegs[i] = FloatPointUtils.BytesToFloat( new byte[] { raw[idx], raw[idx + 1], raw[idx + 2], raw[idx + 3]});
            }
            
            FloatRegView.UpdateRegs();
        }

        public static bool GetBitValue(byte inputByte, int bitPosition)
        {
            if (bitPosition < 0 || bitPosition > 7)
            {
                throw new ArgumentOutOfRangeException(nameof(bitPosition), "Bit position must be between 0 and 7.");
            }

            return (inputByte & (1 << bitPosition)) != 0;
        }

        public static bool IsRegisterChanged(int registerIndex)
        {
            return regModified[registerIndex];
        }

        public static uint Get24BitRegisterValue(byte index)
        {
            return Compose24Bit(
                index,
                (index < 25) ? (byte)(index + 1) : (byte)(index - 25),
                (index < 24) ? (byte)(index + 2) : (byte)(index - 24));
        }

        private static uint Compose24Bit(byte reg1Pointer, byte reg2Pointer, byte reg3Pointer)
        {
            return (uint)((RegPage0[reg1Pointer] << 16) + (RegPage0[reg2Pointer] << 8) + RegPage0[reg3Pointer]);
        }
    }
}
