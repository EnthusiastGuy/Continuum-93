using Continuum93.CodeAnalysis;
using Continuum93.Tools;
using Continuum93.Emulator;
using Continuum93.CodeAnalysis.Network.DataModel;
using Continuum93.Emulator.RAM;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Continuum93.CodeAnalysis.Network
{
    public static class ClientQueue
    {
        public static Queue<Operation> clientCommand = new();

        public static void ParseResponse(byte[] buffer)
        {
            List<byte[]> response = ArrayUtils.SplitArrayByTerminator(buffer);

            foreach (byte[] item in response)
            {
                string json = Encoding.ASCII.GetString(item, 0, item.Length);
                Operation message = JsonSerializer.Deserialize<Operation>(json, NOptions.jsonSerializerOptions);

                clientCommand.Enqueue(message);
            }

            ResolveQueue();
        }

        private static void ResolveQueue()
        {
            while (clientCommand.Count > 0)
            {
                InterpretCommand(clientCommand.Dequeue());
            }
        }

        private static void InterpretCommand(Operation response)
        {
            string command = response.Oper;
            string op = DUtils.GetUntilOrWhole(command);
            Log.WriteLine($"Command '{op}' received from Tools");

            switch (op)
            {
                case "getAllRegisters":
                    {
                        Operation serverResponse = new()
                        {
                            Oper = response.Oper,
                            Data = Machine.COMPUTER.CPU.REGS.GetCurrentRegisterPageData()
                        };
                        ClientHandler.SendOperation(serverResponse);
                        break;
                    }
                case "getAllFlags":
                    {
                        Operation serverResponse = new()
                        {
                            Oper = response.Oper,
                            Data = new byte[] { Machine.COMPUTER.CPU.FLAGS.GetFlagsByte() }
                        };
                        ClientHandler.SendOperation(serverResponse);
                        break;
                    }
                case "getAllFloatRegisters":
                    {
                        Operation serverResponse = new()
                        {
                            Oper = response.Oper,
                            Data = Machine.COMPUTER.CPU.FREGS.GetCurrentRegisterPageData()
                        };
                        ClientHandler.SendOperation(serverResponse);
                        break;
                    }
                case "getAllMemoryPointedByRegisters":
                    {
                        Operation serverResponse = new()
                        {
                            Oper = response.Oper,
                            Data = Machine.COMPUTER.MEMC.GetMemoryPointedByAllAddressingRegisters()
                        };
                        ClientHandler.SendOperation(serverResponse);
                        break;
                    }
                case "getRegisterBankIndex":
                    {
                        Operation serverResponse = new()
                        {
                            Oper = response.Oper,
                            Data = new byte[] { Machine.COMPUTER.CPU.REGS.GetRegisterBank() }
                        };
                        ClientHandler.SendOperation(serverResponse);
                        break;
                    }
                case "getIPAddress":
                    {
                        Operation serverResponse = new()
                        {
                            Oper = response.Oper,
                            Data = DataConverter.GetBytesFrom24bit(Machine.COMPUTER.CPU.REGS.IPO)
                        };
                        ClientHandler.SendOperation(serverResponse);
                        break;
                    }
                case "getDissassembledAt":
                    {
                        int[] parameters = DUtils.GetIntParameters(command);
                        ContinuumDebugger.DebugInstructionsCount = parameters[1];
                        ContinuumDebugger.RunAt((uint)parameters[0], Machine.COMPUTER);

                        Operation serverResponse = new()
                        {
                            Oper = response.Oper,
                            TextData = ContinuumDebugger.GetDissassembledFull(),
                        };
                        ClientHandler.SendOperation(serverResponse);
                        break;
                    }
                case "getMemoryDataAt":
                    {
                        int[] parameters = DUtils.GetIntParameters(command);

                        Operation serverResponse = new()
                        {
                            Oper = response.Oper,
                            Data = MemoryAnalyzer.GetMemoryAt((uint)parameters[0], parameters[1] * 16),
                        };
                        ClientHandler.SendOperation(serverResponse);
                        break;
                    }
                case "getStackData":
                    {
                        StackData stackData = new()
                        {
                            SPR = Machine.COMPUTER.CPU.REGS.SPR,
                            SPC = Machine.COMPUTER.CPU.REGS.SPC,
                            RegStackData = MemoryDebugController.GetRegisterStack(Machine.COMPUTER, 50),
                            CallStackData = MemoryDebugController.GetCallStack(Machine.COMPUTER, 50),
                        };

                        Operation serverResponse = new()
                        {
                            Oper = response.Oper,
                            TextData = JsonSerializer.Serialize(stackData, NOptions.jsonSerializerOptions),
                        };

                        ClientHandler.SendOperation(serverResponse);

                        break;
                    }
                case "toggleStepByStepMode":
                    {
                        DebugState.StepByStep = !DebugState.StepByStep;
                        DebugState.MoveNext = false;
                        Operation serverResponse = new()
                        {
                            Oper = response.Oper,
                            TextData = "" + DebugState.StepByStep,
                        };
                        ClientHandler.SendOperation(serverResponse);
                        break;
                    }
                case "continueExecution":
                    {
                        DebugState.StepByStep = false;
                        DebugState.MoveNext = true;
                        Operation serverResponse = new()
                        {
                            Oper = response.Oper,
                            TextData = "false",
                        };
                        ClientHandler.SendOperation(serverResponse);
                        break;
                    }
                case "advanceStepByStep":
                    {
                        DebugState.MoveNext = true;
                        Operation serverResponse = new()
                        {
                            Oper = response.Oper,
                        };
                        ClientHandler.SendOperation(serverResponse);
                        break;
                    }
                // Video
                case "getPalettes":
                    {
                        Operation serverResponse = new()
                        {
                            Oper = response.Oper,
                            Data = MemoryAnalyzer.GetPalettes(),
                        };
                        ClientHandler.SendOperation(serverResponse);
                        break;
                    }
                case "getVideoLayers":
                    {
                        Operation serverResponse = new()
                        {
                            Oper = response.Oper,
                            Data = MemoryAnalyzer.GetVideoLayersData(),
                        };
                        ClientHandler.SendOperation(serverResponse);
                        break;
                    }
                case "getVideo":
                    {
                        Operation serverResponse = new()
                        {
                            Oper = response.Oper,
                            Data = MemoryAnalyzer.GetVideoData(),
                        };
                        ClientHandler.SendOperation(serverResponse);
                        break;
                    }
            }


        }
    }
}
