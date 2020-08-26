using System;
using System.Collections.Generic;
using System.Linq;
using CapaEntidad.Entidades.Compañias;
using CapaEntidad.Interfaces;

namespace CapaLogica.Validaciones.Validaciones_de_compania
{
    class PersonaJuridicaRegisterValidator : IValidator<PersonaJuridica>
    {
        public bool IsValid(PersonaJuridica entity, ref IEnumerable<string> brokenRules)
        {
            brokenRules = BrokenRules(entity);
            return brokenRules.Count() > 0;
        }

        private IEnumerable<string> BrokenRules(PersonaJuridica entity)
        {
            return new List<string> { "perosn juridica" };
        }
    }
}
