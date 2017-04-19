using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;

namespace SocketServer
{
    class SocketServer: ISocketServer
    {
        public void Listener(int port)
        {
            var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.ReceiveTimeout = 5000; // receive timout 5 seconds
            listener.SendTimeout = 5000; // send timeout 5 seconds 

            listener.Bind(new IPEndPoint(IPAddress.Any, port));
            listener.Listen(backlog: 15);

            var cts = new CancellationTokenSource();
            var tf = new TaskFactory(TaskCreationOptions.LongRunning, TaskContinuationOptions.None);
            tf.StartNew(() =>  // listener task
            {
                while (true)
                {
                    if (cts.Token.IsCancellationRequested)
                    {
                        cts.Token.ThrowIfCancellationRequested();
                        break;
                    }
                    Socket client = listener.Accept();
                    if (!client.Connected)
                    {
                        WriteLine("not connected");
                        continue;
                    }
                    Task t = CommunicateWithClientUsingSocketAsync(client);

                }
                listener.Dispose();

            }, cts.Token);
            ReadLine();
            cts.Cancel();
        }

        private static Task CommunicateWithClientUsingSocketAsync(Socket socket)
        {
            return Task.Run(() =>
            {
                try
                {
                    using (socket)
                    {
                        bool completed = false;
                        do
                        {
                            byte[] readBuffer = new byte[1024];
                            int read = socket.Receive(readBuffer, 0, 1024, SocketFlags.None);
                            string fromClient = Encoding.UTF8.GetString(readBuffer, 0, read);
                            WriteLine($"read {read} bytes: {fromClient}");
                            if (string.Compare(fromClient, "shutdown", ignoreCase: true) == 0)
                            {
                                completed = true;
                            }

                            byte[] writeBuffer = Encoding.UTF8.GetBytes($"echo {fromClient}");

                            int send = socket.Send(writeBuffer);
                            WriteLine($"sent {send} bytes");

                        } while (!completed);
                    }
                    WriteLine("closed stream and client socket");
                }
                catch (SocketException ex)
                {
                    WriteLine(ex.Message);
                }
                catch (Exception ex)
                {
                    WriteLine(ex.Message);
                }
            });
        }

        private static async Task CommunicateWithClientUsingNetworkStreamAsync(Socket socket)
        {
            try
            {
                using (var stream = new NetworkStream(socket, ownsSocket: true))
                {

                    bool completed = false;
                    do
                    {
                        byte[] readBuffer = new byte[1024];
                        int read = await stream.ReadAsync(readBuffer, 0, 1024);
                        string fromClient = Encoding.UTF8.GetString(readBuffer, 0, read);
                        WriteLine($"read {read} bytes: {fromClient}");
                        if (string.Compare(fromClient, "shutdown", ignoreCase: true) == 0)
                        {
                            completed = true;
                        }

                        byte[] writeBuffer = Encoding.UTF8.GetBytes($"echo {fromClient}");

                        await stream.WriteAsync(writeBuffer, 0, writeBuffer.Length);

                    } while (!completed);
                }
                WriteLine("closed stream and client socket");
            }
            catch (Exception ex)
            {
                WriteLine(ex.Message);
            }
        }

        private static async Task CommunicateWithClientUsingReadersAndWritersAsync(Socket socket)
        {
            try
            {
                using (var stream = new NetworkStream(socket, ownsSocket: true))
                using (var reader = new StreamReader(stream, Encoding.UTF8, false, 8192, leaveOpen: true))
                using (var writer = new StreamWriter(stream, Encoding.UTF8, 8192, leaveOpen: true))
                {
                    writer.AutoFlush = true;

                    bool completed = false;
                    do
                    {
                        string fromClient = await reader.ReadLineAsync();
                        WriteLine($"read {fromClient}");
                        if (string.Compare(fromClient, "shutdown", ignoreCase: true) == 0)
                        {
                            completed = true;
                        }

                        await writer.WriteLineAsync($"echo {fromClient}");

                    } while (!completed);
                }
                WriteLine("closed stream and client socket");
            }
            catch (Exception ex)
            {
                WriteLine(ex.Message);
            }
        }
    }
}
