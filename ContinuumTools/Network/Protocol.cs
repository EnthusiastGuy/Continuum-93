using ContinuumTools.States;
using Last_Known_Reality.Reality;

namespace ContinuumTools.Network
{
    public static class Protocol
    {
        static readonly double UPDATE_INTERVAL_MS = 200;
        static double _lastUpdate = 0;
        
        public static void Update()
        {
            double nowTime = GameTimePlus.GetTotalRelativeTimeMs(0);
            if (nowTime - _lastUpdate > UPDATE_INTERVAL_MS)
            {
                _lastUpdate = nowTime;
                //AutoUpdate();
            }
        }

        public static void Ping()
        {
            Requests.Ping();
        }

        public static void GetRegisters()
        {
            Requests.GetAllRegisters();
            Requests.GetAllFloatRegisters();
            Requests.GetRegisterBankIndex();
            Requests.GetAllMemoryPointedByRegisters();
        }

        public static void GetFlags()
        {
            Requests.GetAllFlags();
        }

        public static void GetStacks()
        {
            Requests.GetStacks();
        }

        public static void GetMemory()
        {
            Requests.GetMemory();
        }

        public static void GetInstructionPointerAddress()
        {
            Requests.GetIPAddress();
        }

        public static void GetDissassembled()
        {
            Requests.GetDisassembled();
        }

        public static void ToggleStepByStepMode()
        {
            Requests.ToggleStepByStepMode();
        }

        public static void ContinueExecution()
        {
            Requests.ContinueExecution();
        }

        public static void AdvanceStepByStep()
        {
            StepHistory.PushToHistory(Disassembled.GetCurentInstruction());
            Requests.AdvanceStepByStep();
        }

        // Video
        public static void GetPalettes()
        {
            Requests.GetPalettes();
        }

        public static void GetVideoLayers()
        {
            Requests.GetVideoLayers();
        }

        public static void GetVideo()
        {
            Requests.GetVideo();
        }

        private static void AutoUpdate()
        {
            GetRegisters();
        }

    }
}
