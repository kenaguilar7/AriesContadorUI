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
using AriesContador.Entities.Financial.Accounts;

namespace CapaPresentacion.FrameCompañias
{
    public partial class FrameMaestroCompañia : Form
    {
        private CompañiaCL _companyCL { get; } = new CompañiaCL();
        private List<CompanyDTO> _lstCompanies { get; set; } = new List<CompanyDTO>();
        private CompanyDTO CompanyDTOOnUpdated { get; set; }
        public FrameMaestroCompañia()
        {
            InitializeComponent();
        }

        private CompanyDTO BuilCompany()
        {

            CompanyDTO company = new CompanyDTO
            {
                IdType = (IdType)lstTipoId.SelectedItem,
                Code = txtCodigoCia.Text,
                IdNumber = txtBoxID.Text,
                Name = txtBoxNombre.Text,
                Address = txtBoxDireccion.Text,
                Mail = txtBoxMail.Text,
                Memo = txtBoxObservaciones.Text,
                PhoneNumber1 = txtBoxTelefono1.Text,
                PhoneNumber2 = txtBoxTelefono2.Text,
                CurrencyType = (CurrencyTypeCompany)lstMovimientosRegistro.SelectedItem,
                Op1 = txtBoxOp1.Text,
                Op2 = txtBoxOp2.Text,
                UpdatedBy = 1
                //Account = BuildAccounts() 
            };


            return company;
        }

        #region Create
        private async Task ExecutePostAsync()
        {
            var newCompany = BuilCompany();

            try
            {
                var companyResoulse = await _companyCL.InsertAsync(newCompany);
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
                var company = BuilCompany();
                company.Code = CompanyDTOOnUpdated.Code;
                await _companyCL.UpdateAsync(company, GlobalConfig.UserDTO);
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

            this.lstTipoId.DataSource = Enum.GetValues(typeof(IdType));
            this.lstMovimientosRegistro.DataSource = Enum.GetValues(typeof(CurrencyTypeCompany)); 

            this.lstCompanias.SelectedIndexChanged -= new System.EventHandler(this.LstCompaniasSelectedIndexChanged);
            var companyListFromApi = await _companyCL.GetAllAsync();
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

            txtCodigoCia.Text = await _companyCL.CreateNewCodeAsync();
            lstTipoId.SelectedIndex = 0;
            this.lstMovimientosRegistro.SelectedIndex = 0;
        }
        private void CargarCompaniaFormulario(CompanyDTO compania)
        {

            lstTipoId.SelectedItem = compania.IdType; 
            lstCopiarMaestroCuentas.SelectedIndex = -1;
            lstCopiarMaestroCuentas.Enabled = false;
            CompanyDTOOnUpdated = compania;
            this.txtBoxID.Text = compania.IdNumber;
            this.txtBoxID.ReadOnly = true;
            this.txtBoxNombre.Text = compania.Name;
            this.txtBoxDireccion.Text = compania.Address;
            this.txtBoxTelefono1.Text = compania.PhoneNumber1;
            this.txtBoxTelefono2.Text = compania.PhoneNumber2;
            this.ttCodigo.Text = compania.Code;
            this.groupCodigo.Visible = true;
            //this.txtBoxWeb.Text = compania.Web;
            this.txtBoxMail.Text = compania.Mail;
            this.txtBoxObservaciones.Text = compania.Memo;
            this.btnDelete.Enabled = true;
            this.btnDelete.Visible = true;

            if (compania.IdType == IdType.CEDULA_JURIDICA/* is PersonaFisica*/)
            {
                txtBoxOp1.Text = compania.Op1; /*((PersonaJuridica)compania).MyRepresentanteLegal;*/
                txtBoxOp2.Text = compania.Op2; /*((PersonaJuridica)compania).MyIDRepresentanteLegal;*/
            }
            else
            {
                txtBoxOp1.Text = compania.Op1;  /*((PersonaFisica)compania).MyApellidoPaterno;*/
                txtBoxOp2.Text = compania.Op2; /*((PersonaFisica)compania).MyApellidoMaterno;*/
            }

            this.lstMovimientosRegistro.SelectedItem = compania.CurrencyType; 

            if (compania.CurrencyType == CurrencyTypeCompany.Solo_Colones)
            {
                lstMovimientosRegistro.Enabled = false;
            }
            this.btnActualizar.Visible = true;
            this.btnActualizar.Enabled = true;
            this.btnGuardar.Enabled = false;
            this.btnGuardar.Visible = false;
            this.lstTipoId.Enabled = false;


        }
        #endregion

        #region Events

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
            this.btnDelete.Enabled = false;
            this.btnDelete.Visible = false;
            this.txtBoxBuscar.Clear();
            this.lstMovimientosRegistro.Enabled = true;
            this.lstCopiarMaestroCuentas.Enabled = true;
            this.lstCopiarMaestroCuentas.SelectedIndex = -1;
            ClearLstCompaniesBox();
            this.ttC.Text = string.Empty;
        }

        private void ClearLstCompaniesBox()
        {
            this.lstCompanias.SelectedIndexChanged -= this.LstCompaniasSelectedIndexChanged;
            this.lstCompanias.SelectedIndex = -1;
            this.lstCompanias.SelectedIndexChanged += this.LstCompaniasSelectedIndexChanged;
        }

        private void TipoIdSelectedIndexChanged(object sender, EventArgs e)
        {
            this.LimpiarFormulario();
            SetOpsTextMask();
            txtBoxID.Enabled = true;

            IdType _idType = (IdType)lstTipoId.SelectedItem;
            txtBoxID.Mask = BuildIDMask(_idType);
        }

        private void SetOpsTextMask()
        {
            if ((IdType)lstTipoId.SelectedItem == IdType.CEDULA_NACIONAL)
            {
                txtOp1.Text = "Primer Apellido:";
                txtOp2.Text = "Segundo Apellido:";
            }
            else
            {
                txtOp1.Text = "Representante Legal:";
                txtOp2.Text = "ID Representante Legal:";
            }
        }

        private string BuildIDMask(IdType idType)
        {
            switch (idType)
            {
                case IdType.CEDULA_JURIDICA: return "3-000-000000";
                case IdType.CEDULA_NACIONAL: return "0-0000-0000";
                case IdType.DIMEX: return "000000000000";
                case IdType.NITE: return "0000000000";
                default: return "null";

            }
        }

        private void LstCompaniasSelectedIndexChanged(object sender, EventArgs e)
        {
            var company = (CompanyDTO)this.lstCompanias.SelectedItem;
            CargarCompaniaFormulario(company);
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
            string _code = CreateCodeFromTypingString();
            CompanyDTO searchedCompany = FindEntity(_code);

            if (searchedCompany != null)
            {
                CargarCompaniaFormulario(searchedCompany);
            }
            else if(string.IsNullOrEmpty(_code) == false)
            {
                MessageBox.Show($"No se encontró compañias con el codigo {_code}",
                                StaticInfoString.NombreApp,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
            }

        }

        private CompanyDTO FindEntity(string _code)
        {
            var dataSource = (List<CompanyDTO>)lstCompanias.DataSource;
            if (dataSource != null)
            {
                var searchedCompany = (from c in dataSource where c.Code == _code select c).FirstOrDefault();
                return searchedCompany;
            }
            else
            {
                return null; 
            }

        }

        private string CreateCodeFromTypingString()
        {
            var xx = int.TryParse(txtBoxBuscar.Text, out int num);
            var _code = (xx) ? "C" + num.ToString("000") : txtBoxBuscar.Text;
            return _code;
        }
       
        private async void BtnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                await _companyCL.DeleteAsync(CompanyDTOOnUpdated);
                MessageBox.Show("Compañia eliminada correctamente", StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
