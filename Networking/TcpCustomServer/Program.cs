using System;
using System.Threading.Tasks;

namespace TcpCustomServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                //https://github.com/ProfessionalCSharp/ProfessionalCSharp6/tree/master/Networking/NetworkingSamples
                await new TcpServer().StartServerAsync();

            }).GetAwaiter().GetResult();
        }
    }
}