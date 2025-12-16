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

            // Enter Service mode here

        }
    }
}
