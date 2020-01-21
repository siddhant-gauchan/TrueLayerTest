using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace TrulayerApiTest.Helpers
{
    public interface IHttpHelpers
    {
        HttpClient GetClient(string baseUrl);
    }
    public class HttpHelpers : IHttpHelpers
    {     
     
        public HttpClient GetClient(string baseUrl)
        {
            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;

        }
    }
}

