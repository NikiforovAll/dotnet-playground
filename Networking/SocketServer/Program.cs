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
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1 || !int.TryParse(args[0], out int port))
            {
                return;
            }
            var servers = new[] { new SocketServer(), };
            foreach(var server in servers)
            {
                server.Listener(port++);
            }
            ReadLine();
        }
    }
}