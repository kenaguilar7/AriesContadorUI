using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using AriesContador.Entities.Administration.Users;
using AriesContador.Entities.Administration.Companies;

namespace CapaLogica
{
    public class UserCL
    {
        public async Task<UserDTO> InsertAsync(UserDTO user)
        {
            var response = await RESTClient.TinyRestClient.PostRequest("users", user).ExecuteAsHttpResponseMessageAsync();
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<UserDTO>();
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }
        public async Task<List<UserDTO>> GetAllAsync()
        {
            var response = await RESTClient.TinyRestClient.GetRequest("users").ExecuteAsHttpResponseMessageAsync();
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<List<UserDTO>>();

            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        public async Task UpdateAsync(UserDTO user)
        {
            var response = await RESTClient.TinyRestClient.PutRequest($"users", user).ExecuteAsHttpResponseMessageAsync();
            if (response.IsSuccessStatusCode)
            {
                //nothing to do
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }


        }

    }
}
