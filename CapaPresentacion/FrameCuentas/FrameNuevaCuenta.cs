using CapaEntidad.Entidades.Cuentas;
using CapaEntidad.Enumeradores;
using CapaEntidad.Interfaces;
using CapaEntidad.Textos;
using CapaLogica;
using CapaLogica.Validaciones;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace CapaPresentacion.FrameCuentas
{
    public partial class FrameNuevaCuenta : Form
    {
        public IEnumerable<Cuenta> Cuentas { get; set; }
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
        private void IUserKeyPress(object sender, KeyPressEventArgs e)
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

        private async void CrearCuenta(object sender, EventArgs e)
        {

            if (ValidateChildren())
            {

                try
                {
                    this.btnGuardar.Click -= new System.EventHandler(this.CrearCuenta);
                    var newEntidad = await cuentaCL.InsertAsync(GlobalConfig.Compañia.Codigo, CrearEntidad());
                    SetUpTransfferpipe(newEntidad);
                    this.Close();

                }
                catch (Exception ex)
                {
                    this.btnGuardar.Click += new System.EventHandler(this.CrearCuenta);
                    MessageBox.Show(ex.Message, StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }


        }
        private void CerrarClick(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtBoxNombre_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var txtNombre = txtBoxNombre.Text;
            bool namerepeat = IsNameRepeat(txtNombre, CuentaPadre.TipoCuenta);

            if (string.IsNullOrEmpty(txtNombre))
            {
                SetErrorMessage(txtBoxNombre, "Nombre no puede ir en blanco!", ref e, true);

            }
            else if (namerepeat)
            {
                SetErrorMessage(txtBoxNombre, $"Nombre {txtNombre} en uso!", ref e, true);
            }
            else
            {
                SetErrorMessage(txtBoxNombre, string.Empty, ref e, true);

            }

        }

        private bool IsNameRepeat(string txtNombre, ITipoCuenta tipoCuenta)
        {
            var retorno = false;
            foreach (var item in Cuentas)
            {
                if ((item.TipoCuenta.TipoCuenta == tipoCuenta.TipoCuenta) && (item.Nombre == txtNombre))
                {
                    return true;
                }
            }
            return retorno;
            //return Cuentas.All(x => x.TipoCuenta.TipoCuenta == tipoCuenta.TipoCuenta && x.Nombre == txtNombre);
        }

        private bool IsNameValid(string txtNombre, ITipoCuenta tipoCuenta)
        {
            return Cuentas.All(x => x.TipoCuenta == tipoCuenta && x.Nombre != txtNombre);
        }
        private void SetErrorMessage(Control control, string message, ref CancelEventArgs e, bool cancel)
        {
            e.Cancel = cancel;
            AppErrorProvider.SetError(control, message);
        }
    }
}
