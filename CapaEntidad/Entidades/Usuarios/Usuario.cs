﻿using CapaEntidad.Entidades.Seguridad;
using CapaEntidad.Entidades.Ventanas;
using CapaEntidad.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.Entidades.Usuarios
{
    public class Usuario
    {
        public double UsuarioId { get; set; }
        public string UserName { get; set; }
        public TipoUsuario TipoUsuario { get; set; }
        public string MyCedula { get; set; }
        public string MyNombre { get; set; }
        public string MyApellidoPaterno { get; set; }
        public string MyApellidoMaterno { get; set; }
        public string MyTelefono { get; set; }
        public string MyMail { get; set; }
        public string MyNotas { get; set; }
        public string MyClave { get; set; }
        public string UpdatedBy { get; set; }
        public bool MyActivo { get; set; }
        //public Boolean MyAdmin { get; set; }
        public DateTime MyFechaCreacion { get; set; }
        public DateTime MyFechaActualizacion { get; set; }
        public List<Modulo> Modulos { get; set; } = new List<Modulo>(); 

        public Usuario(string myCedula, string myNombre, string username, TipoUsuario tipoUsuario, string myApellidoPaterno, string myApellidoMaterno,
                       string myTelefono, string myMail, string myNotas, string myClave, bool myActivo,
                       DateTime myFechaCreacion, DateTime myFechaActualizacion, double myID = 0, string updatedBy = "")
        {
            UsuarioId = myID;
            MyCedula = myCedula ?? throw new ArgumentNullException("CEDULA ");
            MyNombre = myNombre ?? throw new ArgumentNullException("NOMBRE");
            UserName = username;
            TipoUsuario = tipoUsuario;
            MyApellidoPaterno = myApellidoPaterno;
            MyApellidoMaterno = myApellidoMaterno;
            MyTelefono = myTelefono;
            MyMail = myMail;
            MyNotas = myNotas;
            MyClave = myClave ?? throw new ArgumentNullException("CLAVE");
            MyActivo = myActivo;
            MyFechaCreacion = myFechaCreacion;
            MyFechaActualizacion = myFechaActualizacion;
            //MyAdmin = myAdmin;
            UpdatedBy = updatedBy;
        }
        public Usuario(string myCedula, string myNombre, string username, TipoUsuario tipoUsuario, string myApellidoPaterno, string myApellidoMaterno,
                   string myTelefono, string myMail, string myNotas, string myClave, bool myActivo, double myID = 0,  string myUpdated = "")
        {
            UsuarioId = myID;
            MyCedula = myCedula ?? throw new ArgumentNullException("CEDULA ");
            MyNombre = myNombre ?? throw new ArgumentNullException("NOMBRE");
            UserName = username;
            TipoUsuario = tipoUsuario;
            MyApellidoPaterno = myApellidoPaterno;
            MyApellidoMaterno = myApellidoMaterno;
            MyTelefono = myTelefono;
            MyMail = myMail;
            MyNotas = myNotas;
            MyClave = myClave ?? throw new ArgumentNullException("CLAVE");
            MyActivo = myActivo;
            //MyAdmin = myAdmin;
            UpdatedBy = myUpdated;
        }
        public Usuario() { }
        public override string ToString()
        {
            return $"{MyNombre} {MyApellidoPaterno} {MyApellidoMaterno}"; 
        }
    }
}
