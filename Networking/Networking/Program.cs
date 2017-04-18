using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Networking
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                var url = "http://localhost:8080";
                var result = await HttpClientSample.GetSampleData(url);

            }).GetAwaiter().GetResult();
        }
    }
}
