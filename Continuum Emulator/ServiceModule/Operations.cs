using Continuum93.CodeAnalysis;
using Continuum93.Emulator;
using Continuum93.ServiceModule.Parsers;

namespace Continuum93.ServiceModule
{
    public static class Operations
    {
        public static string DisassembledCode;
        public static string DisassembledText;

        public static void Disassemble()
        {
            if (!Service.STATE.ServiceMode || Machine.COMPUTER == null)
                return;

            uint startAddress = Machine.COMPUTER.CPU.REGS.IPO;
            ContinuumDebugger.RunAt(startAddress, Machine.COMPUTER);
            DisassembledCode = ContinuumDebugger.GetDissassembledFull();
            Disassembled.SetResponse(DisassembledCode);
        }

        public static void ToggleStepByStepMode()
        {
            if (!Service.STATE.ServiceMode)
                return;

            DebugState.StepByStep = !DebugState.StepByStep;
            DebugState.MoveNext = false;

            if (DebugState.StepByStep)
            {
                Disassemble();
            }
        }

        public static void ContinueExecution()
        {
            if (!Service.STATE.ServiceMode)
                return;

            DebugState.StepByStep = false;
            DebugState.MoveNext = true;
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
        }
    }
}
