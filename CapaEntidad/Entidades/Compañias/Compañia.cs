﻿using CapaEntidad.Entidades.FechaTransacciones;
using CapaEntidad.Enumeradores;
using CapaEntidad.Interfaces;
using JsonSubTypes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CapaEntidad.Entidades.Compañias
{
    //[JsonConverter(typeof(CompanyTypeConverter))]
    [JsonConverter(typeof(JsonSubtypes), "_tipoCompany")]
    [JsonSubtypes.KnownSubType(typeof(PersonaFisica), "fisica")]
    [JsonSubtypes.KnownSubType(typeof(PersonaJuridica), "juridica")]
    public  class Compañia
    {
        //use this property for serialice process
        public virtual string _tipoCompany { get; set; }
        public String Codigo { get; set; }
        public TipoID TipoId { get; set; }
        public String NumeroCedula { get; set; }
        public String Nombre { get; set; }
        public String Direccion { get; set; }
        public String Web { get; set; }
        public String Correo { get; set; }
        public String[] Telefono { get; set; }
        public String Observaciones { get; set; }
        public Boolean Activo { get; set; }
        public TipoMonedaCompañia TipoMoneda { get; set; }
        public List<FechaTransaccion> FechasTransaccion { get; set; }
        public Compañia() { }

        protected Compañia(TipoID tipoID, string numeroId, string nombre, TipoMonedaCompañia TipoMoneda, string direccion,
                         string[] telefono, string web, string correo, string observaciones, string codigo = "", Boolean activo = true)
        {
            this.Codigo = codigo;
            this.TipoId = tipoID;
            this.NumeroCedula = numeroId;
            this.Nombre = nombre;
            this.TipoMoneda = TipoMoneda;
            this.Direccion = direccion;
            this.Telefono = telefono;
            this.Web = web;
            this.Correo = correo;
            this.Observaciones = observaciones;
            this.Activo = activo;
        }


        public override string ToString()
        {
            return $"{ Nombre.ToUpper()}-{Codigo}";
        }


    }
}
