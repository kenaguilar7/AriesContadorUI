using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using AriesContador.Entities.Administration.Users;

namespace CapaLogica
{
    public class IUserCL
    {
        public async Task<IUser> InsertAsync(IUser userInsert, IUser user)
        {
            var response = await RESTClient.TinyRestClient.PostRequest("users", userInsert).ExecuteAsHttpResponseMessageAsync();
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<IUser>();
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }
        public async Task<List<IUser>> GetAllAsync()
        {
            var response = await RESTClient.TinyRestClient.GetRequest("users").ExecuteAsHttpResponseMessageAsync();
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<List<IUser>>();
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }
        public async Task UpdateAsync(IUser user, IUser userTrigger)
        {
            var response = await RESTClient.TinyRestClient.PutRequest($"users/{user.Id}", user).ExecuteAsHttpResponseMessageAsync();
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
