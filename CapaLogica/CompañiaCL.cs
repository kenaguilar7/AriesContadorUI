using CapaDatos.Daos;
using CapaEntidad.Entidades.Compañias;
using CapaEntidad.Entidades.Usuarios;
using CapaEntidad.Enumeradores;
using CapaEntidad.Verificaciones;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using RestSharp;
using RestSharp.Authenticators;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CapaLogica
{
    public class CompañiaCL
    {
        CompañiaDao compañiaDao = new CompañiaDao();

        //static HttpClient client = new HttpClient();

        public Boolean Insert(Compañia t, Usuario user, Compañia copiarDe, out String mensaje)
        {

            try
            {
                ///Mandemos estas verificaciones a la capa entida
                if (!VerificaString.VerificarID(t.NumeroCedula, t.TipoId, out mensaje))
                {
                    return false;
                }
                if (!VerificaString.IsNullOrWhiteSpace(t.Nombre, "Nombre", out mensaje))
                {
                    return false;
                }
                if (!VerificaString.ValidarEmail(t.Correo))
                {
                    mensaje = "Formato de correo invalido";
                    return false;
                }

                if (compañiaDao.Insert(t, user, copiarDe, out mensaje))
                {
                    return true;
                }
                else
                {
                    return false;
                }



            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return false;
            }

        }
        public async Task<bool> UpdateAsync(Compañia t, Usuario user)
        {
            try
            {
                HttpResponseMessage response = await RESTClient.ApiClient.PutAsJsonAsync(
                 $"api/products/{t.Codigo}", t);
                response.EnsureSuccessStatusCode();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return true;
                }
                else {
                    throw new Exception("No se pudo actualizar"); 
                }
                


            }
            catch (Exception ex)
            {

                throw ex;
            }

            //try
            //{
            //    if (!VerificaString.VerificarID(t.NumeroCedula, t.TipoId, out mensaje))
            //    {
            //        return false;
            //    }
            //    if (!VerificaString.IsNullOrWhiteSpace(t.Nombre, "Nombre", out mensaje))
            //    {
            //        return false;
            //    }
            //    if (!VerificaString.ValidarEmail(t.Correo))
            //    {
            //        mensaje = "Formato de correo invalido";
            //        return false;
            //    }

            //    if (compañiaDao.Update(t, user, out mensaje))
            //    {
            //        return true;
            //    }
            //    else
            //    {
            //        return false;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    mensaje = ex.Message;
            //    return false;
            //}
        }
        /**/
        public string NuevoCodigo()
        {
            return compañiaDao.NuevoCodigo();
        }

        public async Task<List<Compañia>> GetAllAsync(Usuario usuario)
        {
            /*Pendiente enviar usuario */
            string url = "http://localhost:5000/api/companies";
            List<Compañia> companias = null;
            HttpResponseMessage response = await RESTClient.ApiClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                companias = await response.Content.ReadAsAsync<List<Compañia>>();
                return companias;

            }else {
                throw new Exception(response.ReasonPhrase); 
            }
        }
    }
}
