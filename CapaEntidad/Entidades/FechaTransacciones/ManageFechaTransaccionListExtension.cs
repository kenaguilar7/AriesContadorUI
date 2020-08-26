using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.Entidades.FechaTransacciones
{
    public static class ManageFechaTransaccionListExtension
    {
        public static FechaTransaccion GetOlderAccountPeriod(this IEnumerable<FechaTransaccion> fechaTransaccions)
            => fechaTransaccions.OrderByDescending(fecha => fecha.Fecha).FirstOrDefault();

        /// <summary>
        /// Get the first 
        /// </summary>
        /// <param name="fechaTransaccions"></param>
        /// <returns></returns>
        public static FechaTransaccion GetNewerAccountPeriod(this IEnumerable<FechaTransaccion> fechaTransaccions)
            => fechaTransaccions.OrderBy(fecha => fecha.Fecha).FirstOrDefault();
    }
}
