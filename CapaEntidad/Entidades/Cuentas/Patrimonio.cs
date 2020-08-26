using CapaEntidad.Enumeradores;
using CapaEntidad.Interfaces;
using System;

namespace CapaEntidad.Entidades.Cuentas
{
    public class Patrimonio :  ITipoCuenta
    {
        public NombreTipoCuenta TipoCuenta { get { return NombreTipoCuenta.Patrimonio; } }
        public Comportamiento Comportamiento { get { return Comportamiento.Credito; } }
        public decimal SaldoActual(decimal saldo, decimal debito, decimal credito)
        {
            return (saldo - debito + credito);
        }
        public decimal SaldoMensual(decimal debito, decimal credito)
        {
            //Cresdito - debito
            return (credito - debito);
        }
    }
}
