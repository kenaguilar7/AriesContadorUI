using AriesContador.Entities.Financial.Accounts;
using AriesContador.Entities.Financial.PostingPeriods;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AriesContador.Entities.Utils;
using System.Text;

namespace CapaLogica
{
    public class CuentaCL
    {
        public async Task<AccountDTO> GetAccountById(int accountId)
        {

            var response = await RESTClient.TinyRestClient.GetRequest($"company/accounts/getbyid").ExecuteAsHttpResponseMessageAsync();
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<AccountDTO>();
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        public async Task<List<AccountDTO>> GetAllAsync(int companyId)
        {
            var response = await RESTClient.TinyRestClient.GetRequest($"accounts/{companyId}").ExecuteAsHttpResponseMessageAsync();
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<List<AccountDTO>>();
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        public async Task<AccountDTO> InsertAsync(AccountDTO account)
        {

            var response = await RESTClient.TinyRestClient.PostRequest($"accounts/", account).ExecuteAsHttpResponseMessageAsync();
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<AccountDTO>();
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        public async Task DeleteAsync(string companyId, double accountId)
        {
            var response = await RESTClient.TinyRestClient.DeleteRequest($"company/{companyId}/accounts/{accountId}").ExecuteAsHttpResponseMessageAsync();

            if (response.IsSuccessStatusCode)
            {
                //
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        public async Task UpdateAsyc(string companyId, AccountDTO account)
        {
            var urlString = $"company/{companyId}/accounts/{account.Id}";

            var response = await RESTClient.TinyRestClient.PutRequest(urlString, account).ExecuteAsHttpResponseMessageAsync();
            if (response.IsSuccessStatusCode)
            {
                //
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        public async Task<AccountDTO> GetFullBalanceAsync(int companyId, AccountDTO account)
        {
            var months = await new FechaTransaccionCL().GetAllAsync(companyId);

            return await GetMonthlyBalanceAsync(companyId, account, months.GetOlderAccountPeriod(), months.GetNewerAccountPeriod());
        }

        public async Task<AccountDTO> GetMonthlyBalanceAsync(int companyId, AccountDTO account, PostingPeriodDTO fromAccountPeriod, PostingPeriodDTO toAccountPeriod)
        {
            var urlString = $"company/{companyId}/accounts/GetFullBalanceWithDateRange/{account.Id}?fromAccountPeriodId={fromAccountPeriod.Id}&toAccountPeriodId={toAccountPeriod.Id}";

            var response = await RESTClient.TinyRestClient.GetRequest(urlString).ExecuteAsHttpResponseMessageAsync();
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<AccountDTO>();
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        public async Task<IEnumerable<AccountDTO>> GetAllAccountBalanceWithJournalEntriesRangeAsync(string companyId, decimal fromJournalDateId, decimal toJournalDateId)
        {
            var urlString = $"company/{companyId}/accounts/GetAllAccountBalanceWithJournalEntriesRange?fromAccountPeriodId={fromJournalDateId}&toAccountPeriodId={toJournalDateId}";

            var response = await RESTClient.TinyRestClient.GetRequest(urlString).ExecuteAsHttpResponseMessageAsync();
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<IEnumerable<AccountDTO>>();
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        public AccountDTO BuscarCuentaPadre(List<AccountDTO> lst, AccountDTO cuentaHija)
        {
            if (lst.Count != 0)
            {
                foreach (AccountDTO item in lst)
                {
                    if (item.Id == cuentaHija.FatherAccount)
                    {
                        return item;
                    }

                }
                return null;
            }
            else
            {
                return null;
            }
        }
        
        public List<AccountDTO> QuitarCuentasSinSaldos(List<AccountDTO> lis)
        {

            var retorno = new List<AccountDTO>();

            //foreach (var item in lis)
            //{
            //    if (item.JournalEntryLines.Count() > 0 || !item.CuentaConMovientos() && item.Indicador == IndicadorCuenta.Cuenta_Titulo)
            //    {
            //        retorno.Add(item.DeepCopy());
            //    }
            //}

            return retorno;
        }


    }

}