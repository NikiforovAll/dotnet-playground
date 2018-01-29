using System;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CustomServer
{
    public class CustomServer : IServer
    {
        public CustomServer(IServiceProvider serviceProvider, IOptions<CustomServerOptions> options)
        {
            var serverAddressesFeature = new ServerAddressesFeature();
            serverAddressesFeature.Addresses.Add(options.Value.FolderPath);

            Features.Set<IHttpRequestFeature>(new HttpRequestFeature());
            Features.Set<IHttpResponseFeature>(new HttpResponseFeature());
            Features.Set<IServerAddressesFeature>(serverAddressesFeature);
        }

        public IFeatureCollection Features { get; } = new FeatureCollection();

        public void Dispose() { }
        public void Start<TContext>(IHttpApplication<TContext> application)
        {
            var watcher = new CustomServerFolderWatcher<TContext>(application, Features);

            watcher.Watch();
        }

        public Task StartAsync<TContext>(IHttpApplication<TContext> application, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                Start<TContext>(application);
            });
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // throw new NotImplementedException();
            //TBD: probably need to free resources used by watcher
            return Task.Run(() => System.Console.WriteLine("Bye!"));
        }
    }
}

namespace Microsoft.AspNetCore.Hosting
{
    public static class CustomServerExtensions
    {
        public static IWebHostBuilder UseCustomWebServer(this IWebHostBuilder hostBuilder,
            Action<CustomServer.CustomServerOptions> options)
        {
            return hostBuilder.ConfigureServices(services =>
            {
                services.Configure(options);
                services.AddSingleton<IServer, CustomServer.CustomServer>();
            });
        }

    }
}