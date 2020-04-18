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

namespace CapaLogica
{
    public static class RESTClient
    {

        public static HttpClient ApiClient { get; set; }
        public static void InitializeClient() {

            ApiClient = new HttpClient();
            ApiClient.BaseAddress = new Uri("http://localhost:5000");
            ApiClient.DefaultRequestHeaders.Accept.Clear();
            ApiClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }
        
    }
}
