using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Networking
{
    class HttpClientSample
    {
        public static async Task<string> GetSampleData(string baseUrl)
        {
            string result = "";
            using (var client = new HttpClient(new ProxyClientHandler()))
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, baseUrl+ "/services");
                request.Headers.Add("Accept", "application/json");
                HttpResponseMessage response = await client.SendAsync(request);
                foreach(var header in response.Headers)
                {
                    Console.WriteLine($"Header: {header.Key} - Value: {header.Value.Aggregate(String.Join)}");
                }
                result = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Payload: {result}");
            }
            return result;
        }
    }
}
