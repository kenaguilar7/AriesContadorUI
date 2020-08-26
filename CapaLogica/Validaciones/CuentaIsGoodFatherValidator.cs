using CapaEntidad.Entidades.Cuentas;
using CapaEntidad.Enumeradores;
using CapaEntidad.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaLogica.Validaciones
{
    public class CuentaIsGoodFatherValidator : IValidator<Cuenta>
    {
        public bool IsValid(Cuenta entity, ref IEnumerable<string> brokenRules)
        {
            brokenRules = BrokenRules(entity);

            return brokenRules.Count() == 0;
        }
        private IEnumerable<string> BrokenRules(Cuenta entity)
        {

            if (entity.Indicador == IndicadorCuenta.Cuenta_Titulo)
                yield return "No es posible crear cuentas a este nivel";
            

            yield break;
        }

    }
}
