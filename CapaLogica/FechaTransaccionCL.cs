using CapaDatos.Daos;
using CapaEntidad.Entidades.Compañias;
using CapaEntidad.Entidades.Cuentas;
using CapaEntidad.Entidades.FechaTransacciones;
using CapaEntidad.Entidades.IUsers;
using CapaEntidad.Enumeradores;
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
        public async Task<IEnumerable<FechaTransaccion>> GetAllAsync(string companyid)
        {
            var response = await RESTClient.TinyRestClient.GetRequest($"companies/{companyid}/AccountingPeriod").ExecuteAsHttpResponseMessageAsync();

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<IEnumerable<FechaTransaccion>>();
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        public async Task<FechaTransaccion> InsertAsync(FechaTransaccion fechaTransaccion, string companyid)
        {
            var response = await RESTClient.TinyRestClient.PostRequest($"companies/{companyid}/AccountingPeriod", fechaTransaccion).ExecuteAsHttpResponseMessageAsync();

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<FechaTransaccion>();
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

        public async Task<IEnumerable<FechaTransaccion>> GetAvailableMonthsAsync(string companyid)
        {
            var response = await RESTClient.TinyRestClient.GetRequest($"companies/{companyid}/AccountingPeriod/GetAvailableMonths").ExecuteAsHttpResponseMessageAsync();
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<IEnumerable<FechaTransaccion>>();
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }


        public async Task CloseMonthAsync(string companyid, FechaTransaccion fechaTransaccion)
        {
            var response = await RESTClient.TinyRestClient.PutRequest($"companies/{companyid}/AccountingPeriod", fechaTransaccion).ExecuteAsHttpResponseMessageAsync();
            if (response.IsSuccessStatusCode)
            {
                ///nothing to do
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }

        }



        private FechaTransaccionDao _fechaDao = new FechaTransaccionDao();
        private AsientoCL _asientoCL = new AsientoCL();
        public List<FechaTransaccion> GetAll(Compañia t, IUser user)
        {
            return _fechaDao.GetAll(t, user);
        }
        public List<FechaTransaccion> GetAllActive(Compañia t, IUser user, Boolean traerAsientos = false)
        {
            List<FechaTransaccion> retorno = GetAll(t, user);
            var Lstretorno = (from c in retorno where c.Cerrada == false select c).ToList<FechaTransaccion>();
            if (traerAsientos)
            {
                foreach (var item in Lstretorno)
                {
                    var lstAsiento = _asientoCL.GetPorFecha(item, t, traerInfoCompleta: true, traerNuevo: false);

                    item.Asientos = lstAsiento;
                }

            }

            return Lstretorno;
        }
        
    }
}
