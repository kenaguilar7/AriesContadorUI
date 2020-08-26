﻿using CapaDatos.Daos;
using CapaEntidad.Entidades.Compañias;
using CapaEntidad.Entidades.IUsers;
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
        public async Task<string> CreateNewCodeAsync()
        {
            var response = await RESTClient.TinyRestClient.GetRequest("companies/getnewcode").ExecuteAsHttpResponseMessageAsync();
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<string>();
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }
        public async Task<Compañia> InsertAsync(Compañia company, string copyFrom)
        {
            var response = await RESTClient.TinyRestClient.PostRequest("companies", company)
                                                .AddHeader("copyfromid", copyFrom)
                                                .ExecuteAsHttpResponseMessageAsync();
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<Compañia>();
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }
        public async Task UpdateAsync(Compañia company, IUser IUser)
        {
            var response = await RESTClient.
                            TinyRestClient.PutRequest($"companies", company)
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
        public async Task<List<Compañia>> GetAllAsync(IUser IUser)
        {
            var response = await RESTClient.TinyRestClient.GetRequest("companies").ExecuteAsHttpResponseMessageAsync();
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<List<Compañia>>();
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }

        }
    }
}
