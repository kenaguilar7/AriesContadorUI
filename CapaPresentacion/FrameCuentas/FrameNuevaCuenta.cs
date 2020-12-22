using AriesContador.Entities;
using AriesContador.Entities.Financial.Accounts;
using CapaLogica;
using CapaPresentacion.utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace CapaPresentacion.FrameCuentas
{
    public partial class FrameNuevaCuenta : Form
    {
        public IEnumerable<AccountDTO> Cuentas { get; set; }
        private CuentaCL cuentaCL = new CuentaCL();
        private AccountDTO CuentaPadre { get; set; }
        private ICallingForm FormParaEnviarCuenta = null;
        public FrameNuevaCuenta(ICallingForm callingFrom, AccountDTO cuenta)
        {
            FormParaEnviarCuenta = callingFrom as ICallingForm;
            CuentaPadre = cuenta;
            InitializeComponent();
            txtCuentaPadre.Text = CuentaPadre.Name;

        }
        private void IUserKeyPress(object sender, KeyPressEventArgs e)
        {
            if ((Keys)e.KeyChar == Keys.Enter)
            {
                e.Handled = true;
                SendKeys.Send("{TAB}");
            }
        }

        private AccountDTO CrearEntidad()
        {
            var account = new AccountDTO();
            account.Name = txtBoxNombre.Text;
            account.AccountType = AccountType.Cuenta_Auxiliar;
            account.CompanyId = CuentaPadre.CompanyId;
            account.AccountTag = CuentaPadre.AccountTag;
            account.Memo = txtBoxDetalle.Text;
            account.FatherAccount = CuentaPadre.Id;
            account.Editable = true;
            

            return account;
        }

        public void SetUpTransfferpipe(AccountDTO cuenta)
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
                    var newAccount = CrearEntidad(); 
                    var newEntidad = await cuentaCL.InsertAsync(CrearEntidad());
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
            bool namerepeat = IsNameRepeat(txtNombre, CuentaPadre.AccountTag);

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
                SetErrorMessage(txtBoxNombre, string.Empty, ref e, false);

            }

        }

        private bool IsNameRepeat(string txtNombre, AccountTag tipoCuenta)
        {
            var retorno = false;
            foreach (var item in Cuentas)
            {
                if ((item.AccountTag == tipoCuenta) && (item.Name == txtNombre))
                {
                    return true;
                }
            }
            return retorno;
            //return Cuentas.All(x => x.TipoCuenta.TipoCuenta == tipoCuenta.TipoCuenta && x.Nombre == txtNombre);
        }

        private bool IsNameValid(string txtNombre, AccountTag tipoCuenta)
        {
            return Cuentas.All(x => x.AccountTag == tipoCuenta && x.Name != txtNombre);
        }

        private void SetErrorMessage(Control control, string message, ref CancelEventArgs e, bool cancel)
        {
            e.Cancel = cancel;
            AppErrorProvider.SetError(control, message);
        }
    }
}
