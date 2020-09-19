using AriesContador.Entities.Financial.JournalEntries;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Threading.Tasks;

namespace CapaLogica
{
    public class AsientoCL
    {
        //endpoints.MapControllerRoute("entries", "company/{companyid}/accountingperiod/{accountingperiodid}/{controller=Home}/{action=Index}");

        public async Task<JournalEntryDTO> GetPreEntryAsync(string companyId, decimal fechaTransaccionId)
        {

            var respose = await RESTClient.TinyRestClient.GetRequest($"company/{companyId}/accountingperiod/{fechaTransaccionId}/bookentry/GetPreEntry").ExecuteAsHttpResponseMessageAsync();
            if (respose.IsSuccessStatusCode)
            {
                return await respose.Content.ReadAsAsync<JournalEntryDTO>();
            }
            else
            {
                throw new Exception(respose.ReasonPhrase);
            }

        }
        public async Task<IEnumerable<JournalEntryDTO>> GetAllAsync(string companyId, decimal fechaTransaccionId)
        {
            var respose = await RESTClient.TinyRestClient.GetRequest($"company/{companyId}/accountingperiod/{fechaTransaccionId}/bookentry").ExecuteAsHttpResponseMessageAsync();
            if (respose.IsSuccessStatusCode)
            {
                return await respose.Content.ReadAsAsync<IEnumerable<JournalEntryDTO>>();
            }
            else
            {
                throw new Exception(respose.ReasonPhrase);
            }
        }
        public async Task<JournalEntryDTO> InsertAsync(string companyId, decimal fechaTransaccionId, JournalEntryDTO asiento)
        {

            var response = await RESTClient.TinyRestClient.PostRequest($"company/{companyId}/accountingperiod/{fechaTransaccionId}/bookentry", asiento).ExecuteAsHttpResponseMessageAsync();
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<JournalEntryDTO>();
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }
        public async Task DeleteAsync(string companyId,decimal fechaTransaccionId, double asientoid)
        {
            var response = await RESTClient.TinyRestClient.DeleteRequest($"company/{companyId}/accountingperiod/{fechaTransaccionId}/bookentry/{asientoid}").ExecuteAsHttpResponseMessageAsync();
            if (response.IsSuccessStatusCode)
            {
                //nothing to do
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }
        public async Task UpdateAsync(string companyId, decimal fechaTransaccionId, JournalEntryDTO asiento)
        {
            var response = await RESTClient.TinyRestClient.PutRequest($"company/{companyId}/accountingperiod/{fechaTransaccionId}/bookentry", asiento).ExecuteAsHttpResponseMessageAsync();
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
