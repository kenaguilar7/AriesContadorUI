using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CapaDatos.Daos;
using CapaEntidad.Entidades.Compañias;
using CapaEntidad.Entidades.Cuentas;
using CapaEntidad.Entidades.FechaTransacciones;
using CapaEntidad.Entidades.IUsers;
using CapaEntidad.Enumeradores;
using CapaEntidad.Interfaces;
using Newtonsoft.Json;

namespace CapaLogica
{
    public class CuentaCL
    {
        public async Task<List<Cuenta>> GetAllAsync(string companyId)
        {
            var response = await RESTClient.TinyRestClient.GetRequest($"company/{companyId}/accounts").ExecuteAsHttpResponseMessageAsync();
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<List<Cuenta>>();
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        public async Task<Cuenta> InsertAsync(string companyId, Cuenta account)
        {
            var response = await RESTClient.TinyRestClient.PostRequest($"company/{companyId}/accounts/", account).ExecuteAsHttpResponseMessageAsync();
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<Cuenta>();
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

        public async Task UpdateAsyc(string companyId, Cuenta account)
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

        public async Task<Cuenta> GetFullBalanceAsync(string companyId, Cuenta account)
        {
            var months = await new FechaTransaccionCL().GetAllAsync(companyId);
            return await GetMonthlyBalanceAsync(companyId, account, months.GetOlderAccountPeriod(), months.GetNewerAccountPeriod()); 
        }
        
        public async Task<Cuenta> GetMonthlyBalanceAsync(string companyId, Cuenta account, FechaTransaccion fromAccountPeriod, FechaTransaccion toAccountPeriod)
        {
            var urlString = $"company/{companyId}/accounts/GetFullBalanceWithDateRange/{account.Id}?fromAccountPeriodId={fromAccountPeriod.Id}&toAccountPeriodId={toAccountPeriod.Id}";

            var response = await RESTClient.TinyRestClient.GetRequest(urlString).ExecuteAsHttpResponseMessageAsync();
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<Cuenta>();
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        public async Task<IEnumerable<Cuenta>> GetAllAccountBalanceWithJournalEntriesRangeAsync(string companyId, decimal fromJournalDateId, decimal toJournalDateId)
        {
            var urlString = $"company/{companyId}/accounts/GetAllAccountBalanceWithJournalEntriesRange?fromAccountPeriodId={fromJournalDateId}&toAccountPeriodId={toJournalDateId}";

            var response = await RESTClient.TinyRestClient.GetRequest(urlString).ExecuteAsHttpResponseMessageAsync();
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<IEnumerable<Cuenta>>();
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }






        CuentaDao cuentaDao = new CuentaDao();
        FechaTransaccionCL _fechaTransaccionCL = new FechaTransaccionCL();
        
        public DataTable GetInfoCompleta(Cuenta cuenta)
        {
            var retorno = new DataTable();
            if (cuenta.Indicador == IndicadorCuenta.Cuenta_Auxiliar)
            {
                retorno = cuentaDao.GetInfoCompletaCuentaAux(cuenta);
            }
            else
            {
                retorno = cuentaDao.GetInforCompletaCuentaMayor(cuenta);
            }


            //decimal acumulado = 0;

            decimal lastSaldoActual = 0m;
            foreach (DataRow item in retorno.Rows)
            {
                var rw = item["Saldo Actual"];
                decimal debito = String.IsNullOrWhiteSpace(item["Debito"].ToString()) ? 0m : Convert.ToDecimal(item["Debito"]);
                ITipoCuenta tpcnta = Cuenta.GenerarTipoCuenta(Convert.ToInt32(rw));
                lastSaldoActual = tpcnta.SaldoActual(saldo: lastSaldoActual, debito: debito, credito: string.IsNullOrWhiteSpace(item["Credito"].ToString()) ? 0m : Convert.ToDecimal(item["Credito"]));
                //acumulado += lastSaldoActual;

                item["Saldo Actual"] = string.Format("{0:n}", lastSaldoActual);


            }




            return retorno;
        }

        public void LLenarConSaldos(DateTime fechaInicio, DateTime fechaFinal, List<Cuenta> lst, Compañia compañia)
        {
            lst.ForEach(delegate (Cuenta c)
            {
                //c.SaldoAnteriorColones = 0.00;
                //c.SaldoAnteriorDolares = 0.00;
                c.DebitosColones = 0.00m;
                c.CreditosColones = 0.00m;
                c.DebitosDolares = 0.00m;
                c.CreditosDolares = 0.00m;
            });

            cuentaDao.CuentaConSaldos(lst, compañia, fechaInicio, fechaFinal);

            ///Pasar este codigo a un metodo independiente

            ///LLenamos tambien la cuenta padre
            ///Como solo trae el saldo de las cuentas axiliares 
            ///entonces llenamos el arbol de cuentas hacia arriba
            foreach (var item in lst)
            {
                if (item.Indicador == IndicadorCuenta.Cuenta_Auxiliar)
                {

                    var dummy = item;

                    while ((dummy = BuscarCuentaPadre(lst, dummy)) != null)
                    {
                        //dummy.SaldoAnteriorColones += item.SaldoAnteriorColones;
                        //dummy.SaldoAnteriorDolares += item.SaldoAnteriorDolares; 
                        dummy.DebitosColones += item.DebitosColones;
                        dummy.CreditosColones += item.CreditosColones;
                        dummy.DebitosDolares += item.DebitosDolares;
                        dummy.CreditosDolares += item.CreditosDolares;

                    }
                }
            }


        }
        public Cuenta BuscarCuentaPadre(List<Cuenta> lst, Cuenta cuentaHija)
        {
            if (lst.Count != 0)
            {
                foreach (Cuenta item in lst)
                {
                    if (item.Id == cuentaHija.Padre)
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
        public List<Cuenta> QuitarCuentasSinSaldos(List<Cuenta> lis)
        {

            var retorno = new List<Cuenta>();

            foreach (var item in lis)
            {
                if (item.CuentaConMovientos() || !item.CuentaConMovientos() && item.Indicador == IndicadorCuenta.Cuenta_Titulo)
                {
                    retorno.Add(item.DeepCopy());
                }
            }

            return retorno;
        }


    }

}