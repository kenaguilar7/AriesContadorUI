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

        public async Task<int> GetPreEntryAsync(int fechaTransaccionId)
        {
            var url = $"journalEntry/GetConsecutive/{fechaTransaccionId}"; 
            var respose = await RESTClient.TinyRestClient
                        .GetRequest(url)
                        .ExecuteAsHttpResponseMessageAsync();
            
            if (respose.IsSuccessStatusCode)
            {
                return await respose.Content.ReadAsAsync<int>();
            }
            else
            {
                throw new Exception(respose.ReasonPhrase);
            }

        }

        public async Task<IEnumerable<JournalEntryDTO>> GetAllAsync(int postingPeriodId)
        {
            var url = $"JournalEntry/GetAllByBaseEntityId/{postingPeriodId}"; 

            var respose = await RESTClient.TinyRestClient
                            .GetRequest(url)
                            .ExecuteAsHttpResponseMessageAsync();
           
            if (respose.IsSuccessStatusCode)
            {
                return await respose.Content.ReadAsAsync<IEnumerable<JournalEntryDTO>>();
            }
            else
            {
                throw new Exception(respose.ReasonPhrase);
            }
        }

        public async Task<JournalEntryDTO> InsertAsync(JournalEntryDTO asiento)
        {
            var url = $"journalentry"; 
            var response = await RESTClient.TinyRestClient.PostRequest(url, asiento)
                .ExecuteAsHttpResponseMessageAsync();
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<JournalEntryDTO>();
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }
        public async Task DeleteAsync(JournalEntryDTO journalEntry)
        {
            var url = "journalentry/delete"; 
            var response = await RESTClient.TinyRestClient.PutRequest(url)
                            .AddContent(journalEntry)
                            .ExecuteAsHttpResponseMessageAsync();

            if (response.IsSuccessStatusCode)
            {
                //nothing to do
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }
        public async Task UpdateAsync(JournalEntryDTO asiento)
        {
            var url = "journalentry"; 
            var response = await RESTClient.TinyRestClient.PutRequest(url, asiento).ExecuteAsHttpResponseMessageAsync();
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
