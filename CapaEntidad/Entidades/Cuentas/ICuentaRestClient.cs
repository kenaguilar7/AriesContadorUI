using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.Entidades.Cuentas
{

    public class ICuentaRestClient
    {
        [JsonProperty("id")]
        int Id { get; set; }

        [JsonProperty("nombre")]
        string Nombre { get; set; }
    }
}
