using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncProgramming
{
    class AsyncTest
    {
        public static int DELAY_IN_MSEC = 1000;
        public static string DelayedTask()
        {
            Task.Delay(DELAY_IN_MSEC).Wait();
            return "processed";
        }
        // task based 
        public static Task<string> ProcessDelayedTaskAsync(string echo = "default")
        {
            return Task.Run<string>(() =>
            {
                Console.WriteLine(echo);
                return DelayedTask();
            });
        }
        public async static void ProcessDelayedTaskAsync2()
        {
            Console.WriteLine(await ProcessDelayedTaskAsync());
        }
        
        public static void CallerWithContinuationTask()
        {
            ProcessDelayedTaskAsync()
                .ContinueWith(t => Console.WriteLine($"#1 - {t.Result}"))
                .ContinueWith(t => ProcessDelayedTaskAsync2())
                .ContinueWith(t => Console.WriteLine("Finished"));
        }
        
        public async static void MultipleAsyncMethhod()
        {
            Task<string> task1 = ProcessDelayedTaskAsync("test1");
            Task<string> task2 = ProcessDelayedTaskAsync("test2");
            await Task.WhenAll(task1, task2);
            Console.WriteLine($"result1: {task1.Result} - result2: {task2.Result}");
        }
        // begin/end approach: converting pattern. 
        public static Func<string, string> asyncIvoker = (s) => s.Trim();

        public static IAsyncResult BeginInvoker(string param, AsyncCallback callback, object state)
        {
            return asyncIvoker.BeginInvoke(param, callback, state);
        }

        public static string EndInvoker(IAsyncResult result)
        {
            return asyncIvoker.EndInvoke(result);
        }

        public static async void ConvertingAsyncPattern()
        {
            string result = await Task<string>.Factory.FromAsync<string>(
                BeginInvoker, EndInvoker, "echo", null);
        }
    }
}
