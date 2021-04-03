using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CapaEntidad.Entidades.Usuarios;
using CapaEntidad.Enumeradores;
using CapaDatos.Daos;

namespace CapaLogica
{
    public class UsuarioCL
    {
        UsuarioDao usuarioDao = new UsuarioDao();
        public async Task<string> Insert(Usuario userInsert, Usuario user)
        {

           return await RestClientService<Usuario>.Post("users", userInsert); 

            //HttpResponseMessage response = await RESTClient.ApiClient.PostAsJsonAsync("users",userInsert);
            ////response.EnsureSuccessStatusCode();

            //if (response.IsSuccessStatusCode)
            //{
            //    response.EnsureSuccessStatusCode();
            //    var newUserUrl = response.Headers.Location;
            //    //Ther user can be consume using newUserUrl string
            //    return true; 
            //}
            //else
            //{
            //    throw new Exception(response.ReasonPhrase);
            //}
        }

        public async Task<List<Usuario>> GetAllAsync()
        {

            return await RestClientService<List<Usuario>>.Get("users"); 
            //List<Usuario> usuarios = null;
            //HttpResponseMessage response = await RESTClient.ApiClient.GetAsync("users");
            //if (response.IsSuccessStatusCode)
            //{
            //    usuarios = await response.Content.ReadAsAsync<List<Usuario>>();
            //    return usuarios;
            //}
            //else
            //{
            //    throw new Exception(response.ReasonPhrase);
            //}
        }

        public async Task<Boolean> Update(Usuario user, Usuario userTrigger)
        {
            HttpResponseMessage response = await RESTClient.ApiClient.PutAsJsonAsync($"users/{user.UsuarioId}", user);
            response.EnsureSuccessStatusCode();

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }

        }
        public Usuario Login(String user, String pass)
        {
            var dt = usuarioDao.Login(user, pass);
            if (dt.Rows.Count != 0)
            {
                object[] vs = dt.Rows[0].ItemArray;

                //GetAllModules

                    var retorno = new Usuario(
                    myID: Convert.ToDouble(vs[0]),
                    username: Convert.ToString(vs[1]),
                    tipoUsuario: (TipoUsuario)Convert.ToInt16(vs[2]),
                    myCedula: Convert.ToString(vs[3]),
                    myNombre: Convert.ToString(vs[4]),
                    myApellidoPaterno: Convert.ToString(vs[5]),
                    myApellidoMaterno: Convert.ToString(vs[6]),
                    myTelefono: Convert.ToString(vs[7]),
                    myMail: Convert.ToString(vs[8]),
                    myNotas: Convert.ToString(vs[9]),
                    //myAdmin: Convert.ToBoolean(vs[10]),
                    myFechaCreacion: Convert.ToDateTime(vs[10]),
                    myFechaActualizacion: Convert.ToDateTime(Convert.ToString(vs[11])),
                    updatedBy: Convert.ToString(Convert.ToString(vs[12])),
                    myActivo: Convert.ToBoolean(vs[13]),
                    myClave: Convert.ToString(vs[14]));

               retorno.Modulos = new PermisoCL().GetAllModules(retorno); 

                return retorno;                  

            }
            else
            {
                throw new Exception("CLAVE INCORRECTA, INTENTE DE NUEVO");
            }

        }
        public Boolean VerificarUserName(String username)
        {
            try
            {

                if (usuarioDao.VerificarNombre(username))
                {
                    return true; 
                }
                else
                {
                    return false; 
                }
            }
            catch (Exception)
            {
                return true; 
            }


        }

        public void Test() {

        }
    }
}
