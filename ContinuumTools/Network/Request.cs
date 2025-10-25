using ContinuumTools.States;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace ContinuumTools.Network
{
    public static class Requests
    {
        private static readonly Queue<Operation> _operations = new();

        public static void Ping()
        {
            Enqueue(new Operation() { Oper = "ping" });
        }

        public static void GetAllRegisters()
        {
            Enqueue(new Operation() { Oper = "getAllRegisters"});
        }

        public static void GetAllFlags()
        {
            Enqueue(new Operation() { Oper = "getAllFlags" });
        }

        public static void GetAllFloatRegisters()
        {
            Enqueue(new Operation() { Oper = "getAllFloatRegisters" });
        }

        public static void GetAllMemoryPointedByRegisters()
        {
            Enqueue(new Operation() { Oper = "getAllMemoryPointedByRegisters" });
        }

        public static void GetStacks()
        {
            Enqueue(new Operation() { Oper = "getStackData" });
        }

        public static void GetMemory()
        {
            Enqueue(new Operation() { Oper = $"getMemoryDataAt:{Memory.Address},{Memory.FetchLines}" });
        }

        public static void GetRegisterBankIndex()
        {
            Enqueue(new Operation() { Oper = "getRegisterBankIndex" });
        }

        public static void GetIPAddress()
        {
            Enqueue(new Operation() { Oper = "getIPAddress" });
        }

        public static void GetDisassembled()
        {
            Enqueue(new Operation() { Oper = $"getDissassembledAt:{Disassembled.Address},{Disassembled.FetchLines}" });
        }

        // Video
        public static void GetPalettes()
        {
            Enqueue(new Operation() { Oper = "getPalettes" });
        }

        public static void GetVideoLayers()
        {
            Enqueue(new Operation() { Oper = "getVideoLayers" });
        }

        public static void GetVideo()
        {
            Enqueue(new Operation() { Oper = "getVideo" });
        }

        public static void ToggleStepByStepMode()
        {
            Enqueue(new Operation() { Oper = "toggleStepByStepMode" });
        }

        public static void ContinueExecution()
        {
            Enqueue(new Operation() { Oper = "continueExecution" });
        }

        public static void AdvanceStepByStep()
        {
            Enqueue(new Operation() { Oper = "advanceStepByStep" });
        }

        public static bool Exist()
        {
            return _operations.Count > 0;
        }

        public static Operation Dequeue()
        {
            return _operations.Dequeue();
        }

        private static void Enqueue(Operation operation)
        {
            if (Client.IsDisconnected())
                return;

            _operations.Enqueue(operation);
            Thread.Sleep(10);   // This probably fixes the bug introduced by commenting the line below.
            //Debug.WriteLine($"Added {operation.Oper}"); // BUG - commenting this seems to mess up some timings
        }
    }

    [Serializable]
    public class Operation
    {
        public string Oper { get; set; } = string.Empty;

        public string Time { get; set; } = string.Empty;

        public byte[] Data { get; set; } = Array.Empty<byte>();

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
