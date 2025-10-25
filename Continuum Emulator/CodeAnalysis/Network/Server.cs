using Continuum93.Emulator;
using Continuum93.Emulator.Settings;
using Continuum93.CodeAnalysis.Network.DataModel;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace Continuum93.CodeAnalysis.Network
{
    public static class Server
    {
        private static readonly ManualResetEvent serverStoppingEvent = new(false);
        public static bool Abort = false;
        public static bool serverStopping = false;
        static TcpListener server = null;
        static TcpClient client = null;
        private static ClientHandler handler;

        private static readonly Thread ServerThread = new(() =>
        {

            try
            {
                RunServer();
            }
            catch (Exception e)
            {
                Log.WriteLine($"Server threw exception: {e.Message}");
            }
            finally
            {
                server?.Stop();
                client?.Close();
            }
        })
        { Priority = ThreadPriority.Normal };

        public static void Start()
        {
            ServerThread.Start();
        }

        public static void Stop()
        {
            Abort = true;
            serverStopping = true;
            serverStoppingEvent.Set();

            handler?.Stop();

            server?.Stop();
            client?.Close();

            server = null;
            client = null;

            if (ServerThread.IsAlive)
            {
                ServerThread.Join(200); // Wait for the ServerThread to finish
            }
        }

        private static void RunServer()
        {
            int port = SettingsManager.GetIntSettingValue("serverPort");

            if (port < 0)
            {
                Log.WriteLine($"Invalid port '{port}'");
                throw new Exception("Server port invalid");
            }

            server ??= new TcpListener(IPAddress.Any, port);
            server.Start();

            while (!Abort)
            {
                try
                {
                    // Wait for a client connection with a timeout
                    if (serverStoppingEvent.WaitOne(100)) // Wait for 100ms
                    {
                        // If the server is stopping, exit the loop
                        Log.WriteLine("Server stopping event");
                        break;
                    }

                    if (server.Pending())
                    {
                        //Debug.WriteLine("Waiting for client connection...");
                        client = server.AcceptTcpClient();

                        // Create a separate thread to handle the client
                        handler = new ClientHandler(client);
                        handler.Start();

                        Log.WriteLine("Client connected");
                        DebugState.ClientConnected = true;
                    }

                }
                catch (IOException)
                {
                    client?.Close();
                    client = null;
                }
            }
            Log.WriteLine("Client connection aborted");
            server?.Stop();
        }
    }

    public class ClientHandler
    {
        private static readonly int BUFFER_SIZE_512k = 512 * 1024;
        private TcpClient client;
        private NetworkStream stream;
        private byte[] buffer;
        private Thread handlerThread;
        private ManualResetEvent handlerStoppingEvent = new(false);

        public ClientHandler(TcpClient client)
        {
            this.client = client;
            stream = client.GetStream();
            buffer = new byte[BUFFER_SIZE_512k];
        }

        public void Start()
        {
            handlerThread = new Thread(HandleClient);
            handlerThread.Start();
        }

        public void Stop()
        {
            DebugState.StepByStep = false;
            handlerStoppingEvent.Set();
            client?.Close();
            client = null;
            if (handlerThread != null && handlerThread.IsAlive)
            {
                handlerThread.Join(1000); // Wait for the handlerThread to finish
            }
        }

        private bool IsClientDisconnected()
        {
            try
            {
                if (client.Client.Poll(0, SelectMode.SelectRead))
                {
                    byte[] checkBuffer = new byte[1];
                    if (client.Client.Receive(checkBuffer, SocketFlags.Peek) == 0)
                    {
                        return true;
                    }
                }
            }
            catch (SocketException)
            {
                return true;
            }
            return false;
        }

        private void HandleClient()
        {
            Log.WriteLine("Attempting to connect network client");

            try
            {
                // Start reading messages from the client
                while (client != null && !IsClientDisconnected() && !Server.Abort)
                {
                    if (handlerStoppingEvent.WaitOne(100)) // Wait for 100ms
                    {
                        // If the handler is stopping, exit the loop
                        break;
                    }

                    while (ServerQueue.HasCommands())
                    {
                        Operation operation = ServerQueue.Dequeue();
                        SendToClient(operation);
                    }
                    try
                    {
                        GetFromClient();
                    }
                    catch (Exception e)
                    {
                        Log.WriteLine($"Client handling exception occured while getting data from client: {e.Message}");

                        client?.Close();
                        client = null;
                    }
                }
            }
            catch (Exception e)
            {
                Log.WriteLine($"Client handling exception occured: {e.Message}");
            }
            finally
            {
                Log.WriteLine($"Client handling stopped");

                client?.Close();
                client = null;
                DebugState.ClientConnected = false;
                Log.WriteLine($"Client disconnected");
            }
        }

        public static void SendOperation(Operation oper)
        {
            //Console.WriteLine($"Operation {oper.Oper}, data length: {oper.Data.Length}");
            ServerQueue.Enqueue(oper);
        }

        private void GetFromClient()
        {
            if (Server.serverStopping)
            {
                return;
            }

            try
            {
                if (stream.DataAvailable)
                {
                    // Read incoming messages from the client
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    //string json = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                    if (bytesRead > 0)
                    {
                        ClientQueue.ParseResponse(buffer);
                        Array.Clear(buffer, 0, buffer.Length);
                    }
                    else
                    {
                        client = null;
                    }
                }

            }
            catch (IOException e)
            {
                Log.WriteLine($"Exception raised when trying to read data from client: {e.Message}");
                client?.Close();
                client = null;
            }
        }

        // Sends mostly the response back to the client.
        private void SendToClient(Operation message)
        {
            if (Server.serverStopping) return;

            try
            {
                Operation messageCopy = new Operation
                {
                    Oper = message.Oper,
                    Time = message.Time,
                    Data = message.Data != null ? (byte[])message.Data.Clone() : null,
                    TextData = message.TextData
                };
                //Console.WriteLine($"non-serialized operation\n{message.GetString()}");
                // Serialize the object to JSON and send it to the client
                string json = JsonSerializer.Serialize(messageCopy, NOptions.jsonSerializerOptions) + "\0";
                //Console.WriteLine("Sending response to client: " + json);
                byte[] buffer = Encoding.ASCII.GetBytes(json);
                stream.Write(buffer, 0, buffer.Length);
            }
            catch (IOException e)
            {
                // The server has disconnected
                Log.WriteLine($"The server has disconnected. Exception: {e.Message}");

                client?.Close();
                client = null;
            }
        }
    }
}
