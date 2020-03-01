﻿using CapaDatos.Daos;
using CapaEntidad.Entidades.Usuarios;
using CapaEntidad.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaLogica
{
    public class UsuarioCL
    {
        UsuarioDao usuarioDao = new UsuarioDao();
        public Boolean Insert(Usuario u, Usuario user, out String mensaje)
        {

            try
            {
                if (String.IsNullOrWhiteSpace(u.MyNombre) || String.IsNullOrWhiteSpace(u.UserName))
                {
                    mensaje = "No se puede guardar usuarios con nombes en blanco";
                    return false;
                }
                else
                {
                    if (usuarioDao.VerificarNombre(u.UserName))//si es true el usuario existe
                    {
                        mensaje = "El nombre de usuario ya se encuentra registrado, intente con otro";
                        return false;
                    }

                    return (usuarioDao.Insert(u, user, out mensaje)) ? true : false; 

                    //if (usuarioDao.Insert(u, user, out mensaje))
                    //{
                    //    return true;
                    //}
                    //else
                    //{
                    //    return false;
                    //}
                }
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return false;
            }

        }
        public List<Usuario> GetAll()
        {
            return usuarioDao.GetAll();
        }
        public Boolean Update(Usuario t, Usuario user, out String mensaje)
        {
            try
            {

                return (usuarioDao.Update(t, user, out mensaje)) ? true : false; 
                //if (usuarioDao.Update(t, user))
                //{
                //    mensaje = "Usuario actulizado correctamente";
                //    return true;
                //}
                //else
                //{
                //    mensaje = "No se pudo actualizar el usuario";
                //    return false;
                //}
            }
            catch (Exception ex)
            {
                ///?¡?¡?¡?¡?¡?¡?¿?¡¿?¿?¿?¿?¿?¿?¿?
                if (usuarioDao.VerificarNombre(t.UserName))
                {
                    mensaje = "El nombre de usuario ya se encuentra registrado";
                }
                else
                {
                    mensaje = ex.Message;
                }

                return false;

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
                    myID: Convert.ToString(vs[0]),
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
                    myUpdated: Convert.ToString(Convert.ToString(vs[12])),
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
    }
}
