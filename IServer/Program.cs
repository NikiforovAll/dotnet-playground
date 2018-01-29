using Microsoft.AspNetCore.Hosting;

namespace CustomServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseCustomWebServer(options =>
                {
                    var isDebug = true;
                    var workspaceName = isDebug?
                        System.AppDomain.CurrentDomain.BaseDirectory:
                        System.IO.Directory.GetCurrentDirectory();
                    System.Console.WriteLine($"Server is initiated at {workspaceName}");
                    options.FolderPath = workspaceName;

                })
                .UseStartup<Startup>()
                .Build();
            host.Run();
        }
    }
}
