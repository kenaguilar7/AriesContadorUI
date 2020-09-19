using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Tiny.RestClient;

namespace DataAccessLayer
{
    public static class RESClient
    {
        public static TinyRestClient TinyRestClient { get; private set; }

        public static void InitializeClient()
        {
            TinyRestClient = new TinyRestClient(new HttpClient(), "https://localhost:44316/api");
        }
    }
}
