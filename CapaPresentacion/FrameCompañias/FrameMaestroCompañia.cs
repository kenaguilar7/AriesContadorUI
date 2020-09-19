using CapaLogica;
using System;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using CapaPresentacion.Reportes;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Text.RegularExpressions;
using AriesContador.Entities.Administration.Companies;
using AriesContador.Entities;
using AriesContador.Entities.Utils;
using DocumentFormat.OpenXml.Drawing;

namespace CapaPresentacion.FrameCompañias
{
    public partial class FrameMaestroCompañia : Form
    {
        private CompañiaCL _companyCL { get; } = new CompañiaCL();
        private List<CompanyDTO> _lstCompanies { get; set; } = new List<CompanyDTO>();
        public FrameMaestroCompañia()
        {
            InitializeComponent();

        }

        private CompanyDTO BuilCompany()
        {

            throw new NotImplementedException();  
            //CompanyDTO company = Factory.CreateCompany();

            ////mas uno porque porque no tenemos cero como valor en el tipo id
            //company. = (TipoID)lstTipoId.SelectedIndex + 1;
            //company.Code = ttCodigo.Text; 
            //company.IdNumber = txtBoxID.Text;
            //company.Name = txtBoxNombre.Text;

            //company.Address = txtBoxDireccion.Text;
            //company.Mail = txtBoxMail.Text;
            //company.Memo = txtBoxObservaciones.Text;
            //company.PhoneNumber1 = txtBoxTelefono1.Text;
            //company.PhoneNumber2 = txtBoxTelefono2.Text;
            //company.Delete = chekActive.Checked;
            //company.CurrencyType = (CurrencyTypeCompany)lstMovimientosRegistro.SelectedIndex + 1; 
            


            //if (lstTipoId.SelectedIndex == 0)
            //{
            //    company = new PersonaJuridica(
            //        codigo: ttCodigo.Text,
            //        numeroId: txtBoxID.Text,
            //        tipoID: _tipoId,
            //        nombre: txtBoxNombre.Text,
            //        TipoMoneda: (TipoMonedaCompañia)lstMovimientosRegistro.SelectedIndex + 1,
            //        representanteLegal: txtBoxOp1.Text,
            //        IDRepresentante: txtBoxOp2.Text,
            //        //direccion: txtBoxDireccion.Text,
            //        //web: txtBoxWeb.Text,
            //        //correo: txtBoxMail.Text,
            //        //observaciones: txtBoxObservaciones.Text,
            //        //telefono: new string[] { this.txtBoxTelefono1.Text, this.txtBoxTelefono2.Text },
            //        //activo: chekActive.Checked);
            //}
            //else
            //{
            //    company = new PersonaFisica(
            //        codigo: ttCodigo.Text,
            //        numeroId: txtBoxID.Text,
            //        tipoID: _tipoId,
            //        nombre: txtBoxNombre.Text,
            //        TipoMoneda: (TipoMonedaCompañia)lstMovimientosRegistro.SelectedIndex + 1,
            //        apellidoPaterno: txtBoxOp1.Text,
            //        apellidoMaterno: txtBoxOp2.Text,
            //        direccion: txtBoxDireccion.Text,
            //        web: txtBoxWeb.Text,
            //        correo: txtBoxMail.Text,
            //        observaciones: txtBoxObservaciones.Text,
            //        telefono: new string[] { this.txtBoxTelefono1.Text, this.txtBoxTelefono2.Text },
            //        activo: chekActive.Checked);
            //}

            //return company;
        }
        private string GetCopyFrom()
        {
            string copiarde = (string)lstCopiarMaestroCuentas.SelectedItem;
            if (copiarde == null)
            {
                copiarde = (string)lstCopiarMaestroCuentas.Items[0];
            }
            return copiarde;
        }

        #region Create
        private async Task ExecutePostAsync()
        {
            var newCompany = BuilCompany();
            var copyFromId = GetCopyFrom();

            try
            {
                var companyResoulse = await _companyCL.InsertAsync(newCompany, copyFromId);
                var mensaje = $"Compañia creada exitosamente con el código {companyResoulse.Code}";
                MessageBox.Show(mensaje, StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarFormulario();
                FrameMaestroCompaniaLoad(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Error);
                //do something
            }
        }
        private async void GuardarNuevaCompaña(object sender, EventArgs e)
        {
            if (ValidateChildren())
            {//sucessful
                await ExecutePostAsync();
            }
            else
            {
                //do something 
            }
        }
        #endregion

        #region Update
        private async Task ExecuteUpdateAsync()
        {
            try
            {
                await _companyCL.UpdateAsync(BuilCompany(), GlobalConfig.UserDTO);
                var mensaje = "Compañia actualizada correctamente";
                MessageBox.Show(mensaje, StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async void ActualizarCompañia(object sender, EventArgs e)
        {
            if (ValidateChildren())
            {
                await ExecuteUpdateAsync();
            }
        }

        #endregion

        #region FormConfig
        private async void FrameMaestroCompaniaLoad(object sender, EventArgs e)
        {
            ///load companies
            ///Remove index change event momentarily
            this.lstCompanias.SelectedIndexChanged -= new System.EventHandler(this.LstCompaniasSelectedIndexChanged);
            var companyListFromApi = await _companyCL.GetAllAsync(GlobalConfig.UserDTO);
            _lstCompanies = (from cmp in companyListFromApi orderby cmp.Code descending select cmp).ToList<CompanyDTO>();
            lstCompanias.DataSource = _lstCompanies;
            lstCompanias.SelectedIndex = -1;
            this.lstCompanias.SelectedIndexChanged += new System.EventHandler(this.LstCompaniasSelectedIndexChanged);

            ///build copy from
            var lstMCuentas = new string[_lstCompanies.Count + 1];
            lstMCuentas[0] = "POR DEFECTO";
            var companyNameList = (from cm in _lstCompanies select cm.ToString()).ToList<string>();
            companyNameList.CopyTo(lstMCuentas, 1);
            lstCopiarMaestroCuentas.DataSource = lstMCuentas;
            lstCopiarMaestroCuentas.SelectedIndex = 0;
            ///set some index
            txtCodigoCia.Text = await _companyCL.CreateNewCodeAsync();
            lstTipoId.SelectedIndex = 0;
            this.lstMovimientosRegistro.SelectedIndex = 0;
        }
        private void CargarCompaniaFormulario(CompanyDTO compania)
        {
            /**
             * la lista lstTipoId tiene como primer indice 0; mientras que 
             * los unum de tipo id tiene como primer indice 1
             * en este caso le restamos 1 
             */
            //lstTipoId.SelectedIndex = Convert.ToInt16(compania.TipoId) - 1;
            //lstCopiarMaestroCuentas.SelectedIndex = -1;
            //lstCopiarMaestroCuentas.Enabled = false;
            //btnActualizar.Tag = compania;
            //this.txtBoxID.Text = compania.NumeroCedula;
            //this.txtBoxID.ReadOnly = true;
            //this.txtBoxNombre.Text = compania.Nombre;
            //this.txtBoxDireccion.Text = compania.Direccion;
            //this.txtBoxTelefono1.Text = compania.Telefono[0];
            //this.txtBoxTelefono2.Text = compania.Telefono[1];
            //this.ttCodigo.Text = compania.Codigo;
            //this.groupCodigo.Visible = true;
            //this.txtBoxWeb.Text = compania.Web;
            //this.txtBoxMail.Text = compania.Correo;
            //this.txtBoxObservaciones.Text = compania.Observaciones;
            //this.chekActive.Enabled = true;
            //this.chekActive.Checked = compania.Activo;
            //if (compania is PersonaFisica)
            //{
            //    txtBoxOp1.Text = ((PersonaFisica)compania).MyApellidoPaterno;
            //    txtBoxOp2.Text = ((PersonaFisica)compania).MyApellidoMaterno;
            //}
            //if (compania is PersonaJuridica)
            //{

            //    txtBoxOp1.Text = ((PersonaJuridica)compania).MyRepresentanteLegal;
            //    txtBoxOp2.Text = ((PersonaJuridica)compania).MyIDRepresentanteLegal;

            //}
            //this.lstMovimientosRegistro.SelectedIndex = Convert.ToInt32(compania.TipoMoneda) - 1;

            //if (compania.TipoMoneda == TipoMonedaCompañia.Solo_Colones)
            //{
            //    lstMovimientosRegistro.Enabled = false;
            //}
            //this.btnActualizar.Visible = true;
            //this.btnActualizar.Enabled = true;
            //this.btnGuardar.Enabled = false;
            //this.btnGuardar.Visible = false;
            //this.lstTipoId.Enabled = false;


        }
        #endregion

        #region Events
        public IEnumerable<Control> GetAllControl(Control control, Type type)
        {
            var controls = control.Controls.Cast<Control>();

            return controls.SelectMany(ctrl => GetAllControl(ctrl, type))
                                      .Concat(controls)
                                      .Where(c => c.GetType() == type);
        }
        private void LimpiarFormulario()
        {
            this.lstTipoId.Enabled = true;
            this.txtBoxID.Clear();
            this.txtBoxNombre.Clear();
            this.txtBoxOp1.Clear();
            this.txtBoxOp2.Clear();
            this.txtBoxWeb.Clear();
            this.txtBoxMail.Clear();
            this.txtBoxTelefono1.Clear();
            this.txtBoxTelefono2.Clear();
            this.txtBoxDireccion.Clear();
            this.txtBoxObservaciones.Clear();
            this.btnGuardar.Enabled = true;
            this.btnGuardar.Visible = true;
            this.btnActualizar.Enabled = false;
            this.btnActualizar.Visible = false;
            this.btnActualizar.Tag = null;
            this.txtBoxID.ReadOnly = false;
            this.groupCodigo.Visible = false;
            this.chekActive.Enabled = false;
            this.txtBoxBuscar.Clear();
            this.lstMovimientosRegistro.Enabled = true;
            this.lstMovimientosRegistro.SelectedIndex = 0;
            this.lstCopiarMaestroCuentas.Enabled = true;
            this.lstCopiarMaestroCuentas.SelectedIndex = 0;
            this.lstCompanias.SelectedIndex = -1;
            this.ttC.Text = string.Empty;
        }
        private void TipoIdSelectedIndexChanged(object sender, EventArgs e)
        {
            //if (Convert.ToString((((DataRowView)lstTipoId.SelectedItem).Row.ItemArray)[0]) == "CEDULA JURIDICA")
            this.LimpiarFormulario();
            if (lstTipoId.SelectedIndex != 0)
            {
                txtOp1.Text = "Primer Apellido:";
                txtOp2.Text = "Segundo Apellido:";
            }
            else
            {
                txtOp1.Text = "Representante Legal:";
                txtOp2.Text = "ID Representante Legal:";
            }


            txtBoxID.Enabled = true;
            //txtBoxID.Mask = VerificaString.MascaraIdentificacion((IdType)lstTipoId.SelectedIndex + 1);

        }
        private void LstCompaniasSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                CargarCompaniaFormulario((CompanyDTO)this.lstCompanias.SelectedItem);
                // btnActualizar.Tag = (Compañia)this.lstCompanias.SelectedItem;
            }
            catch (Exception)
            {

            }
        }
        private void BtnLimpiar(object sender, EventArgs e)
        {
            this.LimpiarFormulario();
        }
        private void Reporte(object sender, EventArgs e)
        {
            ReporteCompañia c = new ReporteCompañia();
            c.MdiParent = this.MdiParent;
            c.Show();

        }
        private void SiguienteEnter(object sender, KeyPressEventArgs e)
        {

            if (e.KeyChar == (char)(Keys.Enter))
            {
                e.Handled = true;
                SendKeys.Send("{TAB}");
            }


        }
        private void Salir(object sender, EventArgs e)
        {
            this.Close();
        }
        private void TxtBoxBuscarLeave(object sender, EventArgs e)
        {
            try
            {

                if (int.TryParse(txtBoxBuscar.Text, out int num))
                {
                    //Le decimos que me devuelva un String con el formto del parametro
                    var cod = "C" + num.ToString("000");

                    List<CompanyDTO> salida = (from c in (List<CompanyDTO>)lstCompanias.DataSource where c.Code == cod select c).Take(1).ToList<CompanyDTO>();

                    if (salida.Count != 0)
                    {
                        CargarCompaniaFormulario(salida[0]);
                    }
                }
                else
                {
                    List<CompanyDTO> salida = (from c in (List<CompanyDTO>)lstCompanias.DataSource where c.Code == txtBoxBuscar.Text select c).Take(1).ToList<CompanyDTO>();
                    if (salida.Count != 0)
                    {
                        CargarCompaniaFormulario(salida[0]);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Validations
        private void TxtBoxMailValidating(object sender, CancelEventArgs e)
        {
            var expression = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
            if (expression.IsMatch(txtBoxMail.Text))
            {
                SetErrorMessage(txtBoxMail, string.Empty, ref e, false);
            }
            else
            {
                SetErrorMessage(txtBoxMail, "Formato incorrecto!", ref e, true);
            }
        }
        private void TxtBoxNombreValidating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBoxNombre.Text))
            {
                SetErrorMessage(txtBoxNombre, "Nombre no puede ir en blanco!", ref e, true);
            }
            else
            {
                SetErrorMessage(txtBoxNombre, string.Empty, ref e, false);
            }

        }
        private void TxtBoxIdValidating(object sender, CancelEventArgs e)
        {
            
            if (txtBoxID.ReadOnly)
            {
                SetErrorMessage(txtBoxID, string.Empty, ref e, false);
                return;
            }
            if (!txtBoxID.MaskCompleted)
            {
                SetErrorMessage(txtBoxID, "Formato incorrecto", ref e, true);
            }
            else if (!IsUserIdValid(txtBoxID.Text))
            {
                SetErrorMessage(txtBoxID, "Número de cédula en uso!", ref e, true);
            }
            else
            {
                SetErrorMessage(txtBoxID, string.Empty, ref e, false);
            }
        }

        private bool IsUserIdValid(string txtId)
        {
            return _lstCompanies.All(x => x.IdNumber != txtId);
        }

        private void SetErrorMessage(Control control, string message, ref CancelEventArgs e, bool cancel)
        {
            e.Cancel = cancel;
            errorProviderApp.SetError(control, message);
        }
        #endregion



    }
}
