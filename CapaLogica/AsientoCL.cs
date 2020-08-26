using CapaDatos.Daos;
using CapaEntidad.Entidades.Asientos;
using CapaEntidad.Entidades.Compañias;
using CapaEntidad.Entidades.FechaTransacciones;
using CapaEntidad.Entidades.IUsers;
using CapaEntidad.Enumeradores;
using CapaEntidad.Interfaces;
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

        public async Task<Asiento> GetPreEntryAsync(string companyId, decimal fechaTransaccionId)
        {

            var respose = await RESTClient.TinyRestClient.GetRequest($"company/{companyId}/accountingperiod/{fechaTransaccionId}/bookentry/GetPreEntry").ExecuteAsHttpResponseMessageAsync();
            if (respose.IsSuccessStatusCode)
            {
                return await respose.Content.ReadAsAsync<Asiento>();
            }
            else
            {
                throw new Exception(respose.ReasonPhrase);
            }

        }
        public async Task<IEnumerable<Asiento>> GetAllAsync(string companyId, decimal fechaTransaccionId)
        {
            var respose = await RESTClient.TinyRestClient.GetRequest($"company/{companyId}/accountingperiod/{fechaTransaccionId}/bookentry").ExecuteAsHttpResponseMessageAsync();
            if (respose.IsSuccessStatusCode)
            {
                return await respose.Content.ReadAsAsync<IEnumerable<Asiento>>();
            }
            else
            {
                throw new Exception(respose.ReasonPhrase);
            }
        }
        public async Task<Asiento> InsertAsync(string companyId, decimal fechaTransaccionId, Asiento asiento)
        {

            var response = await RESTClient.TinyRestClient.PostRequest($"company/{companyId}/accountingperiod/{fechaTransaccionId}/bookentry", asiento).ExecuteAsHttpResponseMessageAsync();
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<Asiento>();
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
        public async Task UpdateAsync(string companyId, decimal fechaTransaccionId, Asiento asiento)
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




        AsientosDao asientoDao = new AsientosDao();
        /// <summary>
        /// Busca todos los asientos de una fecha y compañia especifica
        /// </summary>
        /// <param name="fecha"></param>
        /// <param name="compania"></param>
        /// <param name="traerInfoCompleta">True si quiere los asientos con todas las transacciones</param>
        /// <param name="traerNuevo">False si quiere que no venga con un nuevo(Esto se usa cuando se quiere regitrar nuevos y se necesita una nuevo(FrameAsiento))/param>
        /// <returns>
        /// <param name="List">Lista con todos los asientos encontrados en esa fecha
        /// y con esa compañia
        /// </param>
        /// </returns>
        public List<Asiento> GetPorFecha(FechaTransaccion fecha, Compañia compania, Boolean traerInfoCompleta = false, Boolean traerNuevo = true)
        {
            List<Asiento> lst = asientoDao.GetPorFecha(fecha, compania, traerInfoCompleta);

            //esto lo unico que hace es generar un nuevo asiento para que se posicione de primer lugar la lista
            if (traerNuevo)
            {
                var dummy = new Asiento(
                             numeroAsiento: asientoDao.GetConsecutivo(compania, fecha.Fecha),
                                 compania: compania
                              )
                {
                    FechaAsiento = fecha
                };

                lst.Insert(0, dummy);
            }

            return lst;

        }
        /// <summary>
        /// Esta funcion nos permite conocer si todos los asientos estas cuadrados
        /// o si hay alguno que este pendiente de cuadrar
        /// nos devovera los asientos pendientes de cuadrar
        /// </summary>
        /// <param name="fecha"></param>
        /// <param name="compania"></param>
        /// <returns></returns>
        public DataTable ReporteAsientos(Compañia compañia, FechaTransaccion fecha, Boolean traerTodos)
        {
            return asientoDao.ReporteAsientos(compañia, fecha, traerTodos);
        }
        /// <summary>
        /// Esta funcion devuelve una tabla con los asientos 
        /// que tengan desigual cantidad de debitos y creditos
        /// </summary>
        /// <returns></returns>
    }
}
