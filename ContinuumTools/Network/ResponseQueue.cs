using Continuum93.CodeAnalysis;
using ContinuumTools.Display.Views.Main;
using ContinuumTools.Network.DataModel;
using ContinuumTools.States;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace ContinuumTools.Network
{
    // This processes the responses from the server
    public static class ResponseQueue
    {
        public static Queue<Operation> serverResponse = new();

        public static void ParseBlock(byte[] buffer)
        {
            //Console.WriteLine($"ParseBlock received buffer with length {buffer.Length}");
            List<byte[]> response = GeneralUtils.SplitArrayByTerminator(buffer);

            foreach (byte[] item in response)
            {
                string json = Encoding.ASCII.GetString(item, 0, item.Length);
                //Console.WriteLine($"Deserializing item {json}");
                Operation message = JsonSerializer.Deserialize<Operation>(json, NOptions.jsonSerializerOptions);
                //Console.WriteLine($"Deserialized data[]: {message.Data.Length}");

                serverResponse.Enqueue(message);
            }

            ResolveQueue();
        }

        private static void ResolveQueue()
        {
            while (serverResponse.Count > 0)
            {
                InterpretCommand(serverResponse.Dequeue());
            }
        }

        private static void InterpretCommand(Operation response)
        {
            //Console.WriteLine($"InterpretCommand received this response operation:\n{response.GetString()}");

            string command = response.Oper;
            string op = GeneralUtils.GetUntilOrWhole(command);

            switch (op)
            {
                case "getAllRegisters":
                    {
                        //Console.WriteLine($"Got all registers: {GeneralUtils.ByteArrayToString(response.Data)}");
                        CPUState.PushNewRegs(response.Data);
                        //ClientHandler.SendOperation(optionalResponse);

                        break;
                    }
                case "getAllFloatRegisters":
                    {
                        CPUState.PushNewFloatRegs(response.Data);
                        break;
                    }
                case "getAllFlags":
                    {
                        CPUState.PushNewFlags(response.Data);
                        break;
                    }
                case "getAllMemoryPointedByRegisters":
                    {
                        CPUState.PushNewMemoryPointedByRegs(response.Data);
                        break;
                    }
                case "getRegisterBankIndex":
                    {
                        //Debug.WriteLine($"Got register page index: {response.Data[0]}");
                        CPUState.RegPageIndex = response.Data[0];

                        break;
                    }
                case "getIPAddress":
                    {
                        //Debug.WriteLine($"Got IP address: {GeneralUtils.ByteArrayToString(response.Data)}");
                        CPUState.IPAddress = response.Data[0] * 256 * 256 + response.Data[1] * 256 + response.Data[2];
                        Disassembled.Address = CPUState.IPAddress;
                        //Memory.Address = CPUState.IPAddress;
                        Protocol.GetDissassembled();
                        //Protocol.GetMemory();

                        break;
                    }
                case "getDissassembledAt":
                    {
                        //Debug.WriteLine($"Got disassembled data: {response.TextData}");
                        Disassembled.SetResponse(response.TextData);
                        DisassemblerView.UpdateDiss();

                        break;
                    }
                case "getMemoryDataAt":
                    {
                        Memory.SetResponse(response.Data);
                        MemoryView.UpdateMem();

                        break;
                    }
                case "getStackData":
                    {
                        Stacks.SetStackData(JsonSerializer.Deserialize<ClientStackData>(response.TextData, NOptions.jsonSerializerOptions));

                        break;
                    }
                case "toggleStepByStepMode":
                case "advanceStepByStep":   // The advance succeeded, this is a signal we can pull our data
                    {
                        CPUState.StepByStepMode = true;

                        Protocol.GetRegisters();
                        Protocol.GetFlags();
                        Protocol.GetStacks();
                        Protocol.GetInstructionPointerAddress();
                        Protocol.GetMemory(); // Handled by GetInstructionPointerAddress response
                        Protocol.GetVideo();

                        UserInput.LockStepByStep = false;
                        break;
                    }
                case "continueExecution":
                    {
                        CPUState.StepByStepMode = false;
                        break;
                    }
                // Video
                case "getPalettes":
                    {
                        Video.ProcessPaletteData(response.Data);
                        break;
                    }
                case "getVideoLayers":
                    {
                        Video.ProcessLayerData(response.Data);
                        break;
                    }
                case "getVideo":
                    {
                        Video.ProcessVideoData(response.Data);
                        break;
                    }
            }


        }
    }
}
