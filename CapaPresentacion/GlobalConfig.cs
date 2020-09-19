using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using Squirrel;
using AriesContador.Entities.Financial.Accounts;
using AriesContador.Entities.Administration.Companies;
using AriesContador.Entities.Administration.Users;
using DocumentFormat.OpenXml.ExtendedProperties;

namespace CapaPresentacion
{
    /// <summary>
    /// usar como clase estatica para cargar datos generales 
    /// a futuro para 
    /// </summary>
    public class GlobalConfig
    {

        public GlobalConfig()
        {
            CheckForUpdates();
        }

        private async Task CheckForUpdates()
        {
            
            using (var manager = new UpdateManager(ConfigurationManager.ConnectionStrings["UpdateServerString"].ConnectionString))
            {
                await manager.UpdateApp();
            }

        }










        //public static List<Modulo> Permisos = new List<Modulo>();



        //public static bool GetPermiso(Ventana ventana, CRUDItem cRUDItem)
        //{
            
            
        //    return false;
        //}


        //public static void SetModule(UserDTO UserDTO, Modulo modulo)
        //{
        //    var permisos = UserDTO.Modulos;

        //}

        public static List<AccountDTO> Cuentas { get; set; } = new List<AccountDTO>();
        public static List<CompanyDTO> Compañias { get; set; } = new List<CompanyDTO>();
        public static UserDTO UserDTO { get; set; }
        public static CompanyDTO company { get; set; }


    }
}
