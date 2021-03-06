﻿using CapaEntidad.Interfaces;
using CapaEntidad.Entidades.Compañias;
using System;
using CapaEntidad.Enumeradores;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CapaEntidad.Entidades.Cuentas
{

    public class Cuenta
    {
        [JsonProperty("nombre")]
        public string Nombre { get; set; }

        [JsonIgnore]
        //[JsonProperty("myCompania")]
        public Compañia MyCompania { get; set; } ///Quitar

        [JsonProperty("id")]
        public int Id { get; set; }

        //[JsonProperty("padre")]
        public int Padre { get; set; }

        //[JsonProperty("saldoAnteriorColones")]
        public decimal SaldoAnteriorColones { get; set; }

        //[JsonProperty("saldoAnteriorDolares")]
        public decimal SaldoAnteriorDolares { get; set; }

        //[JsonProperty("debitosColones")]
        public decimal DebitosColones { get; set; }

        //[JsonProperty("debitosDolares")]
        public decimal DebitosDolares { get; set; }

        //[JsonProperty("creditosColones")]
        public decimal CreditosColones { get; set; }

        //[JsonProperty("creditosDolares")]
        public decimal CreditosDolares { get; set; }

        //[JsonProperty("detalle")]
        public String Detalle { get; set; }

        //[JsonProperty("active")]
        public Boolean Active { get; set; }

        //[JsonProperty("indicador")]
        public IndicadorCuenta Indicador { get; set; }


        //[JsonProperty("tipoCuenta")]
        public ITipoCuenta TipoCuenta { get; set; }

        //[JsonProperty("editable")]
        public Boolean Editable { get; set; }

        //[JsonProperty("pathDirection")]
        public String PathDirection { get; set; }

        //[JsonProperty("cuadrada")]
        public Boolean Cuadrada { get; set; }

        public Cuenta() { }

        public Cuenta(string nombre, Compañia myCompania, ITipoCuenta tipoCuenta, IndicadorCuenta indicador, String detalle,
                      int padre, int id = 0, decimal saldoAnteriorColones = 0.0M, decimal saldoAnteriorDolares = 0.00M,
                      decimal debitos = 0.0M, decimal creditos = 0.0M, bool active = true, bool editable = false, bool cuadrada = true)
        {
            Nombre = nombre;
            MyCompania = myCompania;
            Id = id;
            Padre = padre;
            SaldoAnteriorColones = saldoAnteriorColones;
            SaldoAnteriorDolares = SaldoAnteriorDolares;
            DebitosColones = debitos;
            CreditosColones = creditos;
            Detalle = detalle;
            Active = active;
            TipoCuenta = tipoCuenta;
            Indicador = indicador;
            Editable = editable;
            Cuadrada = cuadrada;
        }
        public Cuenta(string nombre, int id, int padre, IndicadorCuenta indicadorCuenta = IndicadorCuenta.Cuenta_Auxiliar)
        {
            Nombre = nombre;
            Id = id;
            Padre = padre;
            Indicador = indicadorCuenta;
        }

        [JsonIgnore]
        public decimal SaldoActualColones
        {
            get { return TipoCuenta.SaldoActual(this.SaldoAnteriorColones, this.DebitosColones, this.CreditosColones); }
        }

        [JsonIgnore]
        public decimal SaldoMensualColones
        {
            get { return TipoCuenta.SaldoMensual(this.DebitosColones, this.CreditosColones); }
        }

        [JsonIgnore]
        public decimal SaldoActualDolares
        {
            get { return TipoCuenta.SaldoActual(this.SaldoAnteriorDolares, DebitosDolares, CreditosDolares); }
        }

        [JsonIgnore]
        public decimal SaldoMensualDolares
        {
            get { return TipoCuenta.SaldoMensual(this.DebitosDolares, this.CreditosDolares); }
        }

        public override string ToString()
        {
            return Nombre;
        }
        public static ITipoCuenta GenerarTipoCuenta(int type)
        {
            switch (type)
            {
                case 1: return new Activo();
                case 2: return new Pasivo();
                case 3: return new Patrimonio();
                case 4: return new Ingreso();
                case 5: return new CostoVenta();
                case 6: return new Egreso();
                default: return null;

            }
        }
        /// <summary>
        /// Usar solo para los reportes de excel 
        /// mandar por parametro un 1 si se desea mostar el campo de debitos 
        /// y un dos si desea momstar el campo de debitos
        /// el sistema se encaraga de procesar la informarcion
        /// </summary>
        /// <param name="tipo"></param>
        /// <returns></returns>
        public decimal SaldoAnteriorParaExcel(Int16 tipo)
        {

            if (this.TipoCuenta.Comportamiento == (Comportamiento)tipo)
            {
                return SaldoAnteriorColones;
            }
            else
            {
                return 0.00m;
            }


        }
        public String[] GetNombreParaExcel(List<Cuenta> list)
        {

            ///BUscar cuantos padres tiene 
            var dummy = this;
            var listaRetorno = new List<Cuenta>();

            while ((dummy = BuscarCuentaPadre(dummy)) != null)
            {

                listaRetorno.Add(dummy);
            }

            if (listaRetorno.Count == 0)
            {
                var retorno = new String[1];
                retorno[0] = this.Nombre;
                for (int i = 0; i < retorno.Length; i++)
                {
                    if (retorno[i] == null)
                    {
                        retorno[i] = "";
                    }
                }
                return retorno;
            }
            else
            {
                var retorno = new String[listaRetorno.Count + 1];

                retorno[retorno.Length - 1] = this.Nombre;
                for (int i = 0; i < retorno.Length; i++)
                {
                    if (retorno[i] == null)
                    {
                        retorno[i] = "";
                    }
                }
                return retorno;
            }

            Cuenta BuscarCuentaPadre(Cuenta cuentaHija)
            {
                if (list.Count != 0)
                {
                    foreach (Cuenta item in list)
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
        }
        public String[] GetArrayNameAndKey(List<Cuenta> list)
        {

            ///BUscar cuantos padres tiene 
            var dummy = this;
            var listaRetorno = new List<Cuenta>();

            while ((dummy = BuscarCuentaPadre(dummy)) != null)
            {

                listaRetorno.Add(dummy);
            }

            if (listaRetorno.Count == 0)
            {
                var retorno = new String[1];
                retorno[0] = this.Nombre;
                for (int i = 0; i < retorno.Length; i++)
                {
                    if (retorno[i] == null)
                    {
                        retorno[i] = "";
                    }
                }
                return retorno;
            }
            else
            {
                var retorno = new String[listaRetorno.Count + 1];

                retorno[retorno.Length - 1] = this.Nombre;
                for (int i = 0; i < retorno.Length; i++)
                {
                    if (retorno[i] == null)
                    {
                        retorno[i] = "";
                    }
                }
                return retorno;
            }

            Cuenta BuscarCuentaPadre(Cuenta cuentaHija)
            {
                if (list.Count != 0)
                {
                    foreach (Cuenta item in list)
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
        }
        public Cuenta DeepCopy()
        {
            Cuenta other = (Cuenta)this.MemberwiseClone();

            other.Nombre = String.Copy(Nombre);
            other.MyCompania = MyCompania;
            other.Id = Id;
            other.Padre = Padre;
            other.SaldoAnteriorColones = SaldoAnteriorColones;
            other.SaldoAnteriorDolares = SaldoAnteriorDolares;
            other.DebitosColones = DebitosColones;
            other.DebitosDolares = DebitosDolares;
            other.CreditosColones = CreditosColones;
            other.CreditosDolares = CreditosDolares;
            other.Active = Active;
            other.Indicador = Indicador;
            other.TipoCuenta = TipoCuenta;

            return other;
        }
        /// <summary>
        /// Si la cuenta tiene algun tipo de movientos
        /// ya sea en Debitos, Creditos o saldo actual devolvera true
        /// </summary>
        /// <returns></returns>
        public Boolean CuentaConMovientos()
        {
            if (SaldoAnteriorColones == 0.00m && DebitosColones == 0.00m && CreditosColones == 0.00m)
            {
                return false;
            }
            else
            {
                return true;
            }

        }
        public bool Validate(IValidator<Cuenta> validator, ref IEnumerable<string> brokenRules)
        {
            return validator.IsValid(this, ref brokenRules);
        }

    }

}