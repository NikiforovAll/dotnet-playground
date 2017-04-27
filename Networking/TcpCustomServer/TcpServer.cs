using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static TcpCustomServer.CustomProtocol;
namespace TcpCustomServer
{
    class TcpServer
    {
        public int PortNumber { get; } = 8800;
        public TcpListener TcpListener { get; private set; }
        public SessionManager SessionManager { get; private set; }

        public async Task StartServerAsync()
        {
            try
            {
                TcpListener = new TcpListener(IPAddress.Any, PortNumber);
                SessionManager = new SessionManager();
                TcpListener.Start();
                while (true)
                {
                    await ProcessClientRequest();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception of type {ex.GetType().Name}, Message: {ex.Message}");
            }
        }

        private Task ProcessClientRequest()
        {
            return Task.Run(async () =>
            {
                try
                {
                    using (TcpClient client = await TcpListener.AcceptTcpClientAsync())
                    {
                        Console.WriteLine("client connected");
                        using (NetworkStream stream = client.GetStream())
                        {
                            bool completed = false;

                            do
                            {
                                byte[] readBuffer = new byte[1024];
                                int read = await stream.ReadAsync(readBuffer, 0, readBuffer.Length);
                                string request = Encoding.ASCII.GetString(readBuffer, 0, read);
                                Console.WriteLine($"received {request}");

                                string sessionId;
                                string result;
                                byte[] writeBuffer = null;
                                string response = string.Empty;

                                ParseResponse resp = request.ParseRequest(SessionManager, out sessionId, out result);
                                switch (resp)
                                {
                                    case ParseResponse.OK:
                                        string content = $"{STATUSOK}::{SESSIONID}::{sessionId}";
                                        if (!string.IsNullOrEmpty(result)) content += $"{SEPARATOR}{result}";
                                        response = $"{STATUSOK}{SEPARATOR}{SESSIONID}{SEPARATOR}{sessionId}{SEPARATOR}{content}";
                                        break;
                                    case ParseResponse.CLOSE:
                                        response = $"{STATUSCLOSED}";
                                        completed = true;
                                        break;
                                    case ParseResponse.TIMEOUT:
                                        response = $"{STATUSTIMEOUT}";
                                        break;
                                    case ParseResponse.ERROR:
                                        response = $"{STATUSINVALID}";
                                        break;
                                    default:
                                        break;
                                }
                                writeBuffer = Encoding.ASCII.GetBytes(response);
                                await stream.WriteAsync(writeBuffer, 0, writeBuffer.Length);
                                await stream.FlushAsync();
                                Console.WriteLine($"returned {Encoding.ASCII.GetString(writeBuffer, 0, writeBuffer.Length)}");


                            } while (!completed);

                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception in client request handling of type {ex.GetType().Name}, Message: {ex.Message}");
                }
            });
        }
    }
}
