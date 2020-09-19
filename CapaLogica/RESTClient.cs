using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Tiny.RestClient;

namespace CapaLogica
{
    public static class RESTClient
    {

        public static HttpClient ApiClient { get; set; }
        public static TinyRestClient TinyRestClient { get; private set; }

        public static void InitializeClient() {

            TinyRestClient = new TinyRestClient(new HttpClient(), "https://localhost:44367/api");

            ApiClient = new HttpClient();
            ApiClient.BaseAddress = new Uri("http://localhost:5000/api/");
            ApiClient.DefaultRequestHeaders.Accept.Clear();
            ApiClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }
        
    }
}
