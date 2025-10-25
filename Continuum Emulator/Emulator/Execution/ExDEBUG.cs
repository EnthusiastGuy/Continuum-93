using Continuum93.Emulator;
using Continuum93.CodeAnalysis;
using Continuum93.CodeAnalysis.Network;

namespace Continuum93.Emulator.Execution
{
    public class ExDEBUG
    {
        //static string tempLog;

        public static void Process(Computer computer)
        {
            if (DebugState.ClientConnected)
            {
                Log.WriteLine("CPU found a DEBUG instruction and sent the step-by-step request to the connected Tools client");
                ClientActions.StartStepByStepMode();
            }

            //tempLog += $"State:{Machine.COMPUTER.CPU.FREGS.GetRegistersState()}{Environment.NewLine}";



            // Dump regs
            //Debug.WriteLine(Machine.COMPUTER.CPU.REGS.EFG);
            //string path = Machine.COMPUTER.MEMC.GetStringAt(Machine.COMPUTER.CPU.REGS.ABCD);
            //byte[] bytes1 = Machine.COMPUTER.MEMC.DumpMemAt(Machine.COMPUTER.CPU.REGS.CDE, 8);
            //byte[] bytes2 = Machine.COMPUTER.MEMC.DumpMemAt(Machine.COMPUTER.CPU.REGS.FGH, 8);
            //Debug.WriteLine(Machine.COMPUTER.CPU.REGS.RST);
        }
    }
}
