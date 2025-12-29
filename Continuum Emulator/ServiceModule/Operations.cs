using Continuum93.CodeAnalysis;
using Continuum93.Emulator;
using Continuum93.ServiceModule.Parsers;
using System;

namespace Continuum93.ServiceModule
{
    public static class Operations
    {
        public static string DisassembledCode;
        public static string DisassembledText;

        private static uint? _lastDisassembledIP;
        private static DateTime _lastDisassembleTime = DateTime.MinValue;
        private const int MinDisassembleIntervalMs = 30;

        public static void Disassemble(bool force = false)
        {
            if (!Service.STATE.ServiceMode || Machine.COMPUTER == null)
                return;

            uint startAddress = Machine.COMPUTER.CPU.REGS.IPO;
            if (!force
                && _lastDisassembledIP.HasValue
                && _lastDisassembledIP.Value == startAddress
                && (DateTime.UtcNow - _lastDisassembleTime).TotalMilliseconds < MinDisassembleIntervalMs)
            {
                return;
            }

            ContinuumDebugger.RunAt(startAddress, Machine.COMPUTER);
            DisassembledCode = ContinuumDebugger.GetDissassembledFull();
            Disassembled.SetResponse(DisassembledCode);
            _lastDisassembledIP = startAddress;
            _lastDisassembleTime = DateTime.UtcNow;
            StepHistory.PushCurrent(Disassembled.GetCurentInstruction());
        }

        public static void ToggleStepByStepMode()
        {
            if (!Service.STATE.ServiceMode)
                return;

            DebugState.StepByStep = !DebugState.StepByStep;
            DebugState.MoveNext = false;
            _lastDisassembledIP = null;

            if (DebugState.StepByStep)
            {
                Disassemble(true);
            }
        }

        public static void ContinueExecution()
        {
            if (!Service.STATE.ServiceMode)
                return;

            DebugState.StepByStep = false;
            DebugState.MoveNext = true;
            _lastDisassembledIP = null;
        }

        public static void AdvanceStepByStep()
        {
            if (!Service.STATE.ServiceMode || !DebugState.StepByStep)
                return;

            DebugState.MoveNext = true;
        }

        public static void UpdateAll()
        {
            CPUState.Update();
            Memory.Update();
            Stacks.Update();
            Video.Update();

            // Keep disassembly in sync during service-mode debugging
            if (Service.STATE.ServiceMode && DebugState.StepByStep)
            {
                Disassemble();
            }
        }
    }
}
