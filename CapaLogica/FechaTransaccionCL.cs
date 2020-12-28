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
        public async Task<IEnumerable<PostingPeriodDTO>> GetAllAsync(string companyid)
        {
            var response = await RESTClient.TinyRestClient.GetRequest($"PostingPeriods/GetAllByBaseEntityId/{companyid}").ExecuteAsHttpResponseMessageAsync();

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<IEnumerable<PostingPeriodDTO>>();
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        public async Task<PostingPeriodDTO> InsertAsync(PostingPeriodDTO PostingPeriodDTO, int companyid)
        {
            var response = await RESTClient.TinyRestClient
                .PostRequest($"companies/{companyid}/AccountingPeriod", PostingPeriodDTO)
                .ExecuteAsHttpResponseMessageAsync();

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<PostingPeriodDTO>();
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        public async Task<DataTable> GetDataTableAsync(string companyid)
        {
            var response = await RESTClient.TinyRestClient
                .GetRequest($"companies/{companyid}/AccountingPeriod/GetDataTable")
                .ExecuteAsHttpResponseMessageAsync();

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<DataTable>();
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        public async Task<IEnumerable<PostingPeriodDTO>> GetAvailableMonthsAsync(string companyid)
        {
            var response = await RESTClient.TinyRestClient
                .GetRequest($"AccountingPeriod/GetAllOpen/{companyid}")
                .ExecuteAsHttpResponseMessageAsync();
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<IEnumerable<PostingPeriodDTO>>();
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        public async Task CloseMonthAsync(int companyid, PostingPeriodDTO PostingPeriodDTO)
        {
            var response = await RESTClient.TinyRestClient
                .PutRequest($"AccountingPeriod/GetAllClose/{companyid}", PostingPeriodDTO)
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

    }
}
