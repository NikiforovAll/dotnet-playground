using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketServer
{
    class SocketClient
    {
        public static async Task SendAndReceive(string hostName, int port)
        {
            try
            {
                IPHostEntry ipHost = await Dns.GetHostEntryAsync(hostName);
                IPAddress ipAddress = 
                    ipHost.AddressList
                        .Where(address => address.AddressFamily == AddressFamily.InterNetwork)
                        .First();
                if (ipAddress == null)
                {
                    return;
                }

                using (var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    client.Connect(ipAddress, port);
                    var stream = new NetworkStream(client);
                    var cts = new CancellationTokenSource();

                    Task tSender = Sender(stream, cts);
                    Task tReceiver = Receiver(stream, cts.Token);
                    await Task.WhenAll(tSender, tReceiver);

                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static async Task Sender(NetworkStream stream, CancellationTokenSource cts)
        {
            while (true)
            {
                string line = Console.ReadLine();
                byte[] buffer = Encoding.UTF8.GetBytes($"{line}\r\n");
                await stream.WriteAsync(buffer, 0, buffer.Length);
                await stream.FlushAsync();
                if (string.Compare(line, "shutdown", ignoreCase: true) == 0)
                {
                    cts.Cancel();
                    break;
                }
            }
        }

        private const int ReadBufferSize = 1024;

        public static async Task Receiver(NetworkStream stream, CancellationToken token)
        {
            try
            {
                stream.ReadTimeout = 5000;
                byte[] readBuffer = new byte[ReadBufferSize];
                while (true)
                {
                    Array.Clear(readBuffer, 0, ReadBufferSize);

                    int read = await stream.ReadAsync(readBuffer, 0, ReadBufferSize, token);
                    string receivedLine = Encoding.UTF8.GetString(readBuffer, 0, read);
                }
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
