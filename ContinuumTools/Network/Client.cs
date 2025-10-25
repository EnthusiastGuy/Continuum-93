using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace ContinuumTools.Network
{
    public static class Client
    {
        public static bool Abort = false;
        static TcpClient client = null;
        private static readonly ManualResetEvent connectEvent = new(false);
        private static readonly int BUFFER_SIZE_512k = 512 * 1024;

        public static bool IsConnected()
        {
            return client != null;
        }

        public static bool IsDisconnected()
        {
            return client == null;
        }

        public static void Start()
        {
            while (!Abort)
            {
                connectEvent.Reset();
                ConnectToServer();

                if (!connectEvent.WaitOne(200))
                {
                    // Connection attempt timed out, try again
                    continue;
                }

                if (client == null || !client.Connected)
                {
                    // Failed to connect, wait a bit and try again
                    Thread.Sleep(200);
                    continue;
                }

                // Start reading messages from the server in a separate thread
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[BUFFER_SIZE_512k];
                Thread t = new(() =>
                {
                    while (!Abort && client?.Connected == true)
                    {
                        try
                        {
                            int bytesRead = stream.Read(buffer, 0, buffer.Length);

                            if (bytesRead > 0)
                            {
                                ResponseQueue.ParseBlock(buffer);
                                Array.Clear(buffer, 0, buffer.Length);
                            }
                            else
                            {
                                client = null;
                            }
                        }
                        catch (IOException)
                        {
                            // The server has disconnected
                            Reporter.PushError("Disconnected from server!");
                            client?.Close();
                            client = null;
                            break;
                        }
                    }
                });
                t.Start();

                // Send messages to the server. TODO handle this
                while (client != null && !Abort)
                {
                    while (Requests.Exist())
                    {
                        Operation operation = Requests.Dequeue();

                        string json = JsonSerializer.Serialize(operation, NOptions.jsonSerializerOptions) + "\0";   // null terminated individual operation

                        byte[] buffer2 = Encoding.ASCII.GetBytes(json);
                        stream.Write(buffer2, 0, buffer2.Length);
                    }
                }
            }

            client?.Close();
        }

        private static void ConnectToServer()
        {
            try
            {
                if (client == null)
                {
                    // Attempt to connect to the server
                    client = new TcpClient("localhost", NetworkSettings.Port);
                    connectEvent.Set();
                    Reporter.PushInfo("Connected to server!");
                }
            }
            catch (Exception e)
            {
                Reporter.PushError(e.Message);
                client?.Close();
                client = null;
            }
        }
    }
}
