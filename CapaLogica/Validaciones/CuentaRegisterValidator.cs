using CapaEntidad.Entidades.Cuentas;
using CapaEntidad.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaLogica.Validaciones
{
    public class CuentaRegisterValidator : IValidator<Cuenta>
    {
        public bool IsValid(Cuenta entity, ref IEnumerable<string> brokenRules)
        {
            brokenRules = BrokenRules(entity);



            return brokenRules.Count() > 0;
        }

        private IEnumerable<string> BrokenRules(Cuenta entity)
        {
            //
            if (string.IsNullOrEmpty(entity.Nombre))
                yield return "Must include a username";
            if (entity.Padre == 0)
                yield return "Must include a email";

            yield break;
        }



    }
}
