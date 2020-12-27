using AriesContador.Entities;
using AriesContador.Entities.Administration.Users;
using CapaLogica;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapaPresentacion.Seguridad
{
    public partial class FrameMaestroUsuario : Form
    {
        #region Class properties
        private UserCL _userCL { get; set; } = new UserCL();
        private List<UserDTO> _userList { get; set; } = new List<UserDTO>();
        private UserDTO _userInDashboard { get; set; } = new UserDTO();
        #endregion
        public FrameMaestroUsuario()
        {
            InitializeComponent();
            LoadEvents();
            CargarDatos();
        }
        private async void CargarDatos()
        {
            lstIUsers.Items.Clear();
            _userList = await _userCL.GetAllAsync();
            lstIUsers.Items.AddRange(_userList.ToArray());
        }

        private UserDTO BuildUser()
        {
            var user = new UserDTO(); 
            user.Id = _userInDashboard.Id;
            user.IdNumber = txtBoxID.Text;
            user.Name = txtBoxNombre.Text;
            user.UserName = txtBoxUserName.Text;
            user.UserType = (rdbIUserAdmin.Checked) ?
                             UserType.Administrador : UserType.Usuario;
            user.LastName = txtBoxOp1.Text;
            user.MiddleName = txtBoxOp2.Text;
            user.PhoneNumber = txtBoxTelefono.Text;
            user.Mail = txtBoxMail.Text;
            user.Memo = txtBoxObservaciones.Text;
            user.Password = txtBoxCLave.Text;
            user.Active = ActiveUser.Checked; 
            return user;
        }

        private async void GuardarIUser(object sender, EventArgs e)
        {
            if (ValidateChildren())
            {

                var user = BuildUser(); 

                try
                {
                    var newUser = await _userCL.InsertAsync(user);
                    var mensaje = $"Usuario {newUser.Name} código {newUser.UserName} - id {newUser.Id} creado exitosamente";
                    MessageBox.Show(mensaje, StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarFormulario(null, null);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else
            {
                //do nothing
            }
        }

        private async void ActualizarIUser(object sender, EventArgs e)
        {
            if (ValidateChildren())
            {
                try
                {
                    var newUser = BuildUser(); 
                    await _userCL.UpdateAsync(newUser);
                    var mensaje = "Usuario actualizado correctamente";
                    MessageBox.Show(mensaje, StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, StaticInfoString.MensajeBannerError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                //do nothing
            }
        }
        #region Eventos
        private void Salir(object sender, EventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// Limpia el formulario y agrega el autoincrementador del codigo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LimpiarFormulario(object sender, EventArgs e)
        {
            this.txtBoxID.Clear();
            this.txtBoxNombre.Clear();
            this.txtBoxOp1.Clear();
            this.txtBoxOp2.Clear();
            this.txtBoxTelefono.Clear();
            this.txtBoxMail.Clear();
            this.txtBoxObservaciones.Clear();
            this.txtBoxCLave.Clear();
            this.btnActualizar.Visible = false;
            this.btnActualizar.Enabled = false;
            this.btnGuardar.Visible = true;
            this.txtBoxUserName.Clear();
            this.txtBoxUserName.ReadOnly = false;
            this.txtBoxID.ReadOnly = false;
            CargarDatos();
        }
        private void UserPressAnyKey(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)(Keys.Enter))
            {
                e.Handled = true;
                SendKeys.Send("{TAB}");
            }
        }
        private void ViewPassword(object sender, EventArgs e)
        {
            if (txtBoxCLave.UseSystemPasswordChar)
            {
                txtBoxCLave.UseSystemPasswordChar = false;
                this.btnVerContraseña.Image = global::CapaPresentacion.Properties.Resources.icons8_invisible_20;
            }
            else
            {
                txtBoxCLave.UseSystemPasswordChar = true;
                this.btnVerContraseña.Image = global::CapaPresentacion.Properties.Resources.icons8_visible_20;
            }
        }
        private void UserLeaveSearchBox(object sender, EventArgs e)
        {
            if (this.Visible && txtBoxBuscar.Text.Length > 0)
            {
                var tyxt = txtBoxBuscar.Text;

                var r = _userList.Find(x => x.UserName.Equals(txtBoxBuscar.Text, StringComparison.OrdinalIgnoreCase));

                if (r != null)
                {
                    lstIUsers.SelectedItem = r;
                    //CargarIUserEvent();
                }
                else
                {
                    MessageBox.Show($"No se encontro ningún usuario con el código {tyxt}", StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }
        private void UserPressSearchKey(object sender, KeyPressEventArgs e)
        {
            if ((Keys)e.KeyChar == Keys.Enter)
            {
                UserLeaveSearchBox(null, null);
            }
        }
        private void UsersDrownListSelectedIndexChanged(object sender, EventArgs e)
        {
            _userInDashboard = (UserDTO)lstIUsers.SelectedItem;
            LoadUsersToDashboard();
        }
        private void LoadEvents()
        {
            this.txtBoxID.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.UserPressAnyKey);
            this.txtBoxNombre.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.UserPressAnyKey);
            this.txtBoxOp1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.UserPressAnyKey);
            this.txtBoxOp2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.UserPressAnyKey);
            this.txtBoxTelefono.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.UserPressAnyKey);
            this.txtBoxMail.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.UserPressAnyKey);
            this.txtBoxObservaciones.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.UserPressAnyKey);
            this.txtBoxCLave.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.UserPressAnyKey);
            this.ActiveUser.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.UserPressAnyKey);
            this.txtBoxUserName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.UserPressAnyKey);
            rdbIUserNormal.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.UserPressAnyKey);
            rdbIUserAdmin.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.UserPressAnyKey);
        }
        private void LoadUsersToDashboard()
        {
            this.txtBoxID.Text = _userInDashboard.IdNumber;
            this.txtBoxNombre.Text = _userInDashboard.Name;
            this.txtBoxUserName.Text = _userInDashboard.UserName;
            this.txtBoxOp1.Text = _userInDashboard.LastName;
            this.txtBoxOp2.Text = _userInDashboard.MiddleName;
            this.txtBoxTelefono.Text = _userInDashboard.PhoneNumber;
            this.txtBoxMail.Text = _userInDashboard.Mail;
            this.txtBoxObservaciones.Text = _userInDashboard.Memo;
            this.ActiveUser.Checked = _userInDashboard.Active;
            this.rdbIUserAdmin.Checked = (_userInDashboard.UserType == UserType.Administrador) ? true : false;
            this.rdbIUserNormal.Checked = (_userInDashboard.UserType == UserType.Usuario) ? true : false;
            this.txtBoxCLave.Text = _userInDashboard.Password;
            this.btnActualizar.Enabled = true;
            this.btnActualizar.Visible = true;
            this.btnGuardar.Visible = false;
            this.txtBoxUserName.ReadOnly = true;
            this.txtBoxID.ReadOnly = true;
        }
        #endregion



        private void txtBoxUserName_Validating(object sender, CancelEventArgs e)
        {
            var txtName = txtBoxUserName.Text;
            if (txtBoxUserName.ReadOnly)
            {
                SetErrorMessage(txtBoxUserName, string.Empty, ref e, false);
                return;
            }
            else if (string.IsNullOrWhiteSpace(txtName))
            {
                SetErrorMessage(txtBoxUserName, "Código de usuario no puede ir en blanco!", ref e, true);
            }
            else if (!IsUserNameValid(txtName))
            {
                SetErrorMessage(txtBoxUserName, "Código de Usuario en uso!", ref e, true);
            }
            else
            {
                SetErrorMessage(txtBoxUserName, string.Empty, ref e, false);
            }
        }

        private bool IsUserNameValid(string txtName)
        {
            return _userList.All(x => x.UserName != txtName);
        }

        private bool IsIdValid(string txtId)
        {
            return _userList.All(x => x.IdNumber != txtId);
        }

        private void SetErrorMessage(Control control, string message, ref CancelEventArgs e, bool cancel)
        {
            e.Cancel = cancel;
            errorProviderApp.SetError(control, message);
        }

        private void txtBoxID_Validating(object sender, CancelEventArgs e)
        {
            var txtId = txtBoxID.Text;
            if (txtBoxID.ReadOnly)
            {
                SetErrorMessage(txtBoxID, string.Empty, ref e, false);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtId))
            {
                SetErrorMessage(txtBoxID, "Identificación no puede ir en blanco!", ref e, true);
            }
            else if (!IsIdValid(txtId))
            {
                SetErrorMessage(txtBoxID, "Número de identificación en uso!", ref e, true);
            }
            else
            {
                SetErrorMessage(txtBoxID, string.Empty, ref e, false);
            }
        }

        private void txtBoxNombre_Validating(object sender, CancelEventArgs e)
        {
            var txtNombre = txtBoxNombre.Text;

            if (string.IsNullOrEmpty(txtNombre))
            {
                SetErrorMessage(txtBoxNombre, "Nombre de usuario no puede ir en blanco!", ref e, true);
            }
            else
            {
                SetErrorMessage(txtBoxNombre, string.Empty, ref e, false);
            }
        }

        private void txtBoxMail_Validating(object sender, CancelEventArgs e)
        {
            var txtMail = txtBoxMail.Text;
            var expression = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");

            if (string.IsNullOrEmpty(txtMail))
            {
                SetErrorMessage(txtBoxMail, "Correo no puede ir en blanco!", ref e, true);
            }
            else if (!expression.IsMatch(txtMail))
            {
                SetErrorMessage(txtBoxMail, "Formato incorrecto!", ref e, true);
            }
            else
            {
                SetErrorMessage(txtBoxMail, string.Empty, ref e, false);
            }

        }
    }
}
