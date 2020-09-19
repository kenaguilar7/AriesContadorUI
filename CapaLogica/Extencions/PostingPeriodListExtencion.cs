using AriesContador.Entities.Financial.PostingPeriods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaLogica.Extencions
{
    public static class PostingPeriodListExtencion
    {
        public static IPostingPeriod GetOlderAccountPeriod(this IEnumerable<IPostingPeriod> fechaTransaccions)
          => fechaTransaccions.OrderByDescending(fecha => fecha.Date).FirstOrDefault();

        public static IPostingPeriod GetNewerAccountPeriod(this IEnumerable<IPostingPeriod> fechaTransaccions)
            => fechaTransaccions.OrderBy(fecha => fecha.Date).FirstOrDefault();
    }
}
