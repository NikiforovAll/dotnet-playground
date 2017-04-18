using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Networking
{
    class ProxyClientHandler: HttpClientHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Add("Proxy", System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
            return base.SendAsync(request, cancellationToken);
            
        }
    }
}
