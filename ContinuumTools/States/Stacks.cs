using ContinuumTools.Network.DataModel;
using ContinuumTools.Utils;
using System.Collections.Generic;

namespace ContinuumTools.States
{
    public static class Stacks
    {
        private static ClientStackData _stackData;
        public static List<string> RegisterStack = new();
        public static List<string> CallStack = new();

        public static void SetStackData(ClientStackData stackData)
        {
            _stackData = stackData;
            SetViewStackData();
        }

        private static void SetViewStackData()
        {
            RegisterStack.Clear();
            CallStack.Clear();
            for (int i = 0; i < _stackData.RegStackCount; i++)
                RegisterStack.Add(StringUtils.ByteToHex(_stackData.RegStackData[_stackData.RegStackCount - i - 1]));

            for (int i = 0; i < _stackData.CallStackCount; i++)
                CallStack.Add(StringUtils.UintToHex(_stackData.CallStackData[_stackData.CallStackCount - i - 1], 6));
        }
    }
}
