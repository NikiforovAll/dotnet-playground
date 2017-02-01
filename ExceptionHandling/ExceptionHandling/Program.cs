using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceptionHandling
{
    class Program
    {
        static void Main(string[] args)
        {
            //client code

            Console.WriteLine("Enter file name:");
            var fileName = "test.txt"; //Console.ReadLine();
            using (DocumentReader reader =
                new DocumentReader()
                    .SetProcessor((arg) => $"[{arg}:{arg.Length}]")
                    .SetValidator((arg) => arg != null && arg.Length > 3))
            {
                reader.Open(fileName);
                foreach (var chunk in reader)
                {
                    Console.WriteLine(chunk);
                }
            }

        }
    }
}
