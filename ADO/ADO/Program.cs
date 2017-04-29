using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                var sample = new ADOSample();
                ////Console.WriteLine(await sample.ExecuteScalar());
                (await sample.ExecuteReader(771)).ForEach(el => Console.WriteLine(el));
            }
           ).GetAwaiter().GetResult();
        }
    }
}
