using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.Interfaces
{
    public interface IValidator <T>
    {
        bool IsValid(T entity, ref IEnumerable<string> brokenRules);
    }
}
