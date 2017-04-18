using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Net.Http.Server;
using System.Net;

namespace Server
{
    class Server
    {
        public async Task StartServerAsync(string prefix)
        {
            try
            {
                var settings = new WebListenerSettings();
                settings.UrlPrefixes.Add(prefix);
                var listener = new WebListener(settings);
                listener.Start();
                do
                {
                    using (RequestContext context = await listener.AcceptAsync())
                    {
                        Console.WriteLine("test");
                        var hostAddress = await Dns.GetHostAddressesAsync("www.google.com");
                        byte[] payload = Encoding.UTF8.GetBytes(
                            $"[{DateTime.Now.ToString("M/d/yyyy")}] {hostAddress}");
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        context.Response.Headers.Add("content-type", new[] { "text" });
                        await context.Response.Body.WriteAsync(payload, 0, payload.Length);
                    }
                } while (true);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
