using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using RestSharp;
using RestSharp.Authenticators;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AriesContador.Entities.Administration.Companies;
using AriesContador.Entities.Administration.Users;

namespace CapaLogica
{
    public class CompañiaCL
    {
        public async Task<string> CreateNewCodeAsync()
        {
            var response = await RESTClient.TinyRestClient.GetRequest("companies/GetConsecutive").ExecuteAsHttpResponseMessageAsync();
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<string>();
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }
        public async Task<CompanyDTO> InsertAsync(CompanyDTO company)
        {
            var response = await RESTClient.TinyRestClient.PostRequest("companies", company)
                                                .ExecuteAsHttpResponseMessageAsync();
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<CompanyDTO>();
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        public async Task UpdateAsync(CompanyDTO company, UserDTO UserDTO)
        {
            var response = await RESTClient.
                            TinyRestClient.PutRequest($"companies", company)
                            .ExecuteAsHttpResponseMessageAsync();
            if (response.IsSuccessStatusCode)
            {
                ///nothing to do
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }
        public async Task DeleteAsync(CompanyDTO company)
        {
            var response = await RESTClient.
                            TinyRestClient.DeleteRequest($"companies")
                            .AddContent(company)
                            .ExecuteAsHttpResponseMessageAsync();
            if (response.IsSuccessStatusCode)
            {
                ///nothing to do
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        public async Task<List<CompanyDTO>> GetAllAsync()
        {
            var response = await RESTClient.TinyRestClient.GetRequest("companies").ExecuteAsHttpResponseMessageAsync();
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<List<CompanyDTO>>();
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }

        }
    }
}
