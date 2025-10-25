using Continuum93.CodeAnalysis.Network.DataModel;
using System.Collections.Generic;

namespace Continuum93.CodeAnalysis.Network
{
    public class ServerQueue
    {
        public static Queue<Operation> serverCommand = new();

        public static void Enqueue(Operation operation)
        {
            serverCommand.Enqueue(operation);
        }

        public static bool HasCommands()
        {
            return serverCommand.Count > 0;
        }

        public static Operation Dequeue()
        {
            return (serverCommand.Dequeue());
        }
    }
}
