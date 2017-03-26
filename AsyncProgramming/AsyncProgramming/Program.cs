using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncProgramming
{
    class Program
    {
        static void Main(string[] args)
        {
            // async context 
            Task.Run( async () =>
                {
                    var test = new AsyncTest2();
                    //await test.TasksUsingThreadPool();
                    //await test.TaskContinuation();
                    test.TaskCancelation();
                }
            ).GetAwaiter().GetResult();
        }
    }
}
