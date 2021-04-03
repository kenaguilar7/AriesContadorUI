using CapaEntidad.Entidades.Cuentas;
using CapaEntidad.Enumeradores;
using CapaEntidad.Interfaces;
using CapaEntidad.Textos;
using CapaLogica;
using CapaLogica.Validaciones;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CapaPresentacion.FrameCuentas
{
    public partial class FrameNuevaCuenta : Form
    {
        private CuentaCL cuentaCL = new CuentaCL();
        private Cuenta CuentaPadre { get; set; } = new Cuenta();
        private ICallingForm FormParaEnviarCuenta = null;
        public FrameNuevaCuenta(ICallingForm callingFrom, Cuenta cuenta)
        {
            FormParaEnviarCuenta = callingFrom as ICallingForm;
            CuentaPadre = cuenta;
            InitializeComponent();
            txtCuentaPadre.Text = CuentaPadre.Nombre;

        }
        private void UsuarioKeyPress(object sender, KeyPressEventArgs e)
        {
            if ((Keys)e.KeyChar == Keys.Enter)
            {
                e.Handled = true;
                SendKeys.Send("{TAB}");
            }
        }

        private Cuenta CrearEntidad()
        {

            return new Cuenta
            {
                Nombre = txtBoxNombre.Text,
                Indicador = IndicadorCuenta.Cuenta_Auxiliar,
                MyCompania = CuentaPadre.MyCompania,
                TipoCuenta = CuentaPadre.TipoCuenta,
                Detalle = txtBoxDetalle.Text,
                Padre = CuentaPadre.Id,
                Editable = true
            };

        }
        public void SetUpTransfferpipe(Cuenta cuenta)
        {

            if (FormParaEnviarCuenta != null)
            {
                FormParaEnviarCuenta.TransferirCuenta(cuenta);
            }

        }
        public void PintarErrores(IEnumerable<string> brokenRules) {

            foreach (var bkndRule in brokenRules)
            {
                //todo
            }

        }
        private void CrearCuenta(object sender, EventArgs e)
        {
            IEnumerable<string> brokenRules = null;
            var nuevaCuenta = CrearEntidad();
            var mensaje = "";
            var urlresource = "";
            

            if (!nuevaCuenta.Validate(new CuentaRegisterValidator(), ref brokenRules))
            {
                PintarErrores(brokenRules); 
            }
            else
            {
                try
                {
                    urlresource = cuentaCL.Insert(nuevaCuenta, CuentaPadre, GlobalConfig.Usuario).GetAwaiter().GetResult();
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }

            if (urlresource != null)
            {
                MessageBox.Show(mensaje, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetUpTransfferpipe(nuevaCuenta);
                this.Close();
            }
            else
            {
                MessageBox.Show(mensaje, TextoGeneral.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }


        }
        private void CerrarClick(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
