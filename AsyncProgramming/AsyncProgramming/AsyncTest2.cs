using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncProgramming
{
    public class AsyncTest2
    {
        public static object _lock = new Object();

        public void TaskMethod(object o)
        {
            lock (_lock)
            {
                Console.WriteLine(o?.ToString());
                Console.WriteLine($"Task is: {Task.CurrentId?.ToString() ?? "no task"}");
                Console.WriteLine($"Thread is: {Thread.CurrentThread.ManagedThreadId}");
            }
        }

        public async Task TasksUsingThreadPool()
        {
            var taskFactory = new TaskFactory();
            Task t1 = taskFactory.StartNew(TaskMethod, "obj task factory ");
            Task t2 = Task.Factory.StartNew(TaskMethod, "task factory ");
            Task t3 = new Task(TaskMethod, "task ctor");
            t3.Start();
            Task t4 = Task.Run(() => TaskMethod("from run method"));
            await Task.WhenAll(new Task[]{ t1, t2, t3, t4 });
        }

        public async Task TaskContinuation()
        {
            Task<int> t1 = Task.Factory.StartNew<int>(() => { return 1; });
            await t1.ContinueWith((prevTask) =>
             {
                 Console.WriteLine($"task finished {prevTask.Id} " +
                     $"with result {prevTask.Result}, started task {Task.CurrentId?.ToString() ?? "no task"}");
             });
            //already started 
        }

        public void TaskCancelation()
        {
            var tokenSource = new CancellationTokenSource();
            tokenSource.Token.Register(() => Console.WriteLine("Finished by cancelation token"));
            tokenSource.CancelAfter(500);
            Task t1 = Task.Run(async () =>
            {
                
                Console.WriteLine("in task");
                await Task.Delay(1000);
                if (tokenSource.IsCancellationRequested)
                {
                    Console.WriteLine("Cancellation was requested");
                    tokenSource.Token.ThrowIfCancellationRequested();
                }
                //based on closure
            }, tokenSource.Token);
            try
            {
                t1.Wait();
            }
            catch (AggregateException ex)
            {
                Console.WriteLine($"{ex.GetType().Name} - {ex.Message} - {ex.InnerException.Message}");
            }

        }
    }
}
