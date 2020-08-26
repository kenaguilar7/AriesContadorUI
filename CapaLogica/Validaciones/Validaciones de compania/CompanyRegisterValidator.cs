using System;
using System.Collections.Generic;
using System.Linq;
using CapaEntidad.Entidades.Compañias;
using CapaEntidad.Interfaces;

namespace CapaLogica.Validaciones.Validaciones_de_compania
{
    public class CompanyRegisterValidator : IValidator<Compañia>
    {
        public bool IsValid(Compañia entity, ref IEnumerable<string> brokenRules)
        {
            //brokenRules = BrokenRules(entity);
            //return brokenRules.Count() > 0;
            return false;
        }

        private IEnumerable<string> BrokenRules(Compañia entity)
        {
            return new List<string> {"person fisica"};
        }
    }
}
