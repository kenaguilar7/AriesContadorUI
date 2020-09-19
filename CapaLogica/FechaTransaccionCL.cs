using AriesContador.Entities.Financial.PostingPeriods;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace CapaLogica
{
    public class FechaTransaccionCL
    {
        public async Task<IEnumerable<IPostingPeriod>> GetAllAsync(string companyid)
        {
            var response = await RESTClient.TinyRestClient.GetRequest($"companies/{companyid}/AccountingPeriod").ExecuteAsHttpResponseMessageAsync();

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<IEnumerable<IPostingPeriod>>();
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        public async Task<IPostingPeriod> InsertAsync(IPostingPeriod IPostingPeriod, string companyid)
        {
            var response = await RESTClient.TinyRestClient.PostRequest($"companies/{companyid}/AccountingPeriod", IPostingPeriod).ExecuteAsHttpResponseMessageAsync();

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<IPostingPeriod>();
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        public async Task<DataTable> GetDataTableAsync(string companyid)
        {
            var response = await RESTClient.TinyRestClient.GetRequest($"companies/{companyid}/AccountingPeriod/GetDataTable").ExecuteAsHttpResponseMessageAsync();

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<DataTable>();
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        public async Task<IEnumerable<IPostingPeriod>> GetAvailableMonthsAsync(string companyid)
        {
            var response = await RESTClient.TinyRestClient.GetRequest($"companies/{companyid}/AccountingPeriod/GetAvailableMonths").ExecuteAsHttpResponseMessageAsync();
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<IEnumerable<IPostingPeriod>>();
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }


        public async Task CloseMonthAsync(string companyid, IPostingPeriod IPostingPeriod)
        {
            var response = await RESTClient.TinyRestClient.PutRequest($"companies/{companyid}/AccountingPeriod", IPostingPeriod).ExecuteAsHttpResponseMessageAsync();
            if (response.IsSuccessStatusCode)
            {
                ///nothing to do
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }

        }

    }
}
