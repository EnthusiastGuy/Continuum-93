using Continuum93.CodeAnalysis.Network.DataModel;

namespace Continuum93.CodeAnalysis.Network
{
    public static class ClientActions
    {
        public static void StartStepByStepMode()
        {
            DebugState.StepByStep = true;
            DebugState.MoveNext = false;
            Operation serverResponse = new()
            {
                Oper = "toggleStepByStepMode",
            };
            ClientHandler.SendOperation(serverResponse);
        }
    }
}
