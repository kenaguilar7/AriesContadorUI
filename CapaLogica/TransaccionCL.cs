using CapaDatos.Daos;
using CapaEntidad.Entidades.Asientos;
using CapaEntidad.Entidades.IUsers;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace CapaLogica
{
    public class TransaccionCL
    {
        public async Task<Transaccion> InsertAsync(Transaccion transaccion, double asientoId)
        {
            var response = await RESTClient.TinyRestClient.PostRequest($"bookentry/{asientoId}/BookTransaction", transaccion).ExecuteAsHttpResponseMessageAsync();
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<Transaccion>();
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }
        public async Task<IEnumerable<Transaccion>> GetALlAsync(double asientoId)
        {
            var response = await RESTClient.TinyRestClient.GetRequest($"bookentry/{asientoId}/BookTransaction").ExecuteAsHttpResponseMessageAsync();
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<IEnumerable<Transaccion>>();
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }
        public async Task UpdateAsync(Transaccion transaccion, double asientoId)
        {
            var response = await RESTClient.TinyRestClient.PutRequest($"bookentry/{asientoId}/BookTransaction/{transaccion.Id}", transaccion).ExecuteAsHttpResponseMessageAsync();
            if (response.IsSuccessStatusCode)
            {
                //noting to do, the resource has been delete succesfull
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }
        public async Task DeleteAsync(int transaccionId, int asientoId)
        {
            var response = await RESTClient.TinyRestClient.DeleteRequest($"bookentry/{asientoId}/BookTransaction/{transaccionId}").ExecuteAsHttpResponseMessageAsync();
            if (response.IsSuccessStatusCode)
            {
                //nothing to do, resource has been delete succesfull'
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }
    }
}
