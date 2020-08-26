using CapaDatos.Daos;
using CapaEntidad.Entidades.Compañias;
using CapaEntidad.Entidades.Seguridad;
using CapaEntidad.Entidades.IUsers;
using CapaEntidad.Entidades.Ventanas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaLogica
{
    public class PermisoCL
    {
        readonly private PermisoDAO permisoDAO = new PermisoDAO();

        public Boolean InsertCompany(List<Compañia> compañias, IUser asignacion, IUser IUser)
        {
            return permisoDAO.InsertCompany(compañias, asignacion, IUser); 
        }
        public Boolean RemoveCompany(List<Compañia> compañias, IUser asignacion, IUser IUser) {
            return permisoDAO.RemoveCompany(compañias, asignacion, IUser); 
        }
        public List<Modulo> GetAllModules(IUser IUser) {
            return permisoDAO.GetAllModules(IUser); 
        }
        public Boolean UpdatePermisos(List<Modulo> lst, IUser actualizante, IUser actualizador) {
            return permisoDAO.UpdatePermisos(lst, actualizante, actualizador); 
        }
    }
}
