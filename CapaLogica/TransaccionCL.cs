using AriesContador.Entities.Financial.JournalEntries;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace CapaLogica
{
    public class TransaccionCL
    {
        public async Task<JournalEntryLineDTO> InsertAsync(JournalEntryLineDTO transaccion)
        {
            var url = $"journalentrieslines"; 
            var response = await RESTClient.TinyRestClient.PostRequest(url, transaccion).ExecuteAsHttpResponseMessageAsync();
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<JournalEntryLineDTO>();
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }
        public async Task<IEnumerable<JournalEntryLineDTO>> GetALlAsync(int asientoId)
        {
            var url = $"journalentrieslines/GetAllByBaseEntityId/{asientoId}";
            var response = await RESTClient.TinyRestClient
                .GetRequest(url)
                .ExecuteAsHttpResponseMessageAsync();

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<IEnumerable<JournalEntryLineDTO>>();
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }
        public async Task UpdateAsync(JournalEntryLineDTO transaccion)
        {
            var url = $"journalentrieslines";
            var response = await RESTClient.TinyRestClient.PutRequest(url, transaccion).ExecuteAsHttpResponseMessageAsync();
            if (response.IsSuccessStatusCode)
            {
                //noting to do, the resource has been delete succesfull
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }
        public async Task DeleteAsync(JournalEntryLineDTO journalEntryLine)
        {
            var url = $"journalentrieslines/delete";
            var response = await RESTClient.TinyRestClient.PutRequest(url)
                .AddContent(journalEntryLine)
                .ExecuteAsHttpResponseMessageAsync();
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
