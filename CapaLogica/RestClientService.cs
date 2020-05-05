using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CapaLogica
{
    public static class RestClientService <T>
    {
        public async static Task<string> Post(string url, T entity) {

            HttpResponseMessage response = await RESTClient.ApiClient.PostAsJsonAsync(url, entity);

            if (response.IsSuccessStatusCode)
            {
                response.EnsureSuccessStatusCode();
                return response.Headers.Location.ToString();
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        public async static Task<T> Get(string url) {

            
            HttpResponseMessage response = await RESTClient.ApiClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<T>();
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }

        }

    }
}
