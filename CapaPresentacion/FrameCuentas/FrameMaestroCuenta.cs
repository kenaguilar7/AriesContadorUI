using AriesContador.Entities.Financial.Accounts;
using AriesContador.Entities.Financial.PostingPeriods;
using CapaLogica;
using CapaPresentacion.cods;
using CapaPresentacion.Reportes;
using CapaPresentacion.utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapaPresentacion.FrameCuentas
{
    public partial class FrameMaestroCuenta : Form, ICallingForm
    {
        #region WindowsProperties
        private CuentaCL _cuentaCL { get; } = new CuentaCL();
        private FechaTransaccionCL _fechaTransaccionCL { get; } = new FechaTransaccionCL();
        private List<AccountDTO> _lstCuentas { get; set; } = new List<AccountDTO>();
        private IEnumerable<IPostingPeriod> _lstFechas { get; set; } = new List<IPostingPeriod>();
        private AccountDTO CuentaActual
        {
            get => (treeCuentas.SelectedNode is null) ? null : treeCuentas.SelectedNode.Tag as AccountDTO;
            set => CuentaActual = value;
        }
        #endregion

        public FrameMaestroCuenta()
        {
            InitializeComponent();
        }

        #region CRUD
        #region Create
        private async void CrearNuevaCuenta(object sender, EventArgs e)
        {
            try
            {
                AccountDTO cuenta = CuentaActual;
                var desicionHeredarSaldo = true;


                if (cuenta.AccountType == AccountType.Cuenta_Titulo)
                {
                    MessageBox.Show("No se pueden crear cuentas a este nivel", StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    desicionHeredarSaldo = false;
                }
                else if (cuenta.AccountType == AccountType.Cuenta_Auxiliar)
                {
                    var cuentaConMovimiento = await ObtenerSaldoDeCuentaAsync(cuenta);
                    desicionHeredarSaldo = IUserAceptaHeredarSaldo(cuentaConMovimiento);
                }

                if (desicionHeredarSaldo)
                {
                    AbrirVentanaNuevaCuenta(cuenta);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool IUserAceptaHeredarSaldo(AccountDTO cuenta)
        {
            if (cuenta.JournalEntryLines.Count > 0)
            {
                var message = $"Esta cuenta posee movimientos, si continua estos seran heredados a la nueva cuenta\n" +
                              $"Saldo Anterior      {string.Format("{0:₡###,###,###,##0.00##}", cuenta.PriorBalance)}\n" +
                              $"Debitos             {string.Format("{0:₡###,###,###,##0.00##}", cuenta.GetDebitBalance())}\n" +
                              $"Creditos            {string.Format("{0:₡###,###,###,##0.00##}", cuenta.GetCreditBalance())}\n" +
                              $"\n" +
                              $"¿Desea continuar y crear una cuenta nueva?";

                var dgResult = MessageBox.Show(message, StaticInfoString.NombreApp, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                return (dgResult == DialogResult.Yes) ? true : false;
            }
            else
            {
                return true;
            }
        }

        private void AbrirVentanaNuevaCuenta(AccountDTO cuenta)
        {
            FrameNuevaCuenta nv = new FrameNuevaCuenta(this, cuenta);
            nv.Cuentas = _lstCuentas;
            nv.ShowDialog();
        }

        private async Task<AccountDTO> ObtenerSaldoDeCuentaAsync(AccountDTO cuenta)
        {
            return await _cuentaCL.GetFullBalanceAsync(GlobalConfig.company.Code, cuenta);
        }

        public bool TransferirCuenta(AccountDTO cuenta)
        {
            if (cuenta != null)
            {
                TreeViewCuentas.CargarCuentaAlTreeView(cuenta, ref treeCuentas, _lstCuentas);
                return true;
            }
            else { return false; }
        }
        #endregion

        #region Delete
        private async void DeleteAccount(object sender, EventArgs e)
        {
            string brokenRule = string.Empty;

            if (VerifyAccountForDeleteAction(ref brokenRule))
            {
                try
                {
                    await ExecuteDeleteAsync();
                    var mensaje = "Cuenta elimada correctamente";
                    MessageBox.Show(mensaje, StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DeleteAccountFromList(CuentaActual);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

            }
            else
            {
                MessageBox.Show(brokenRule, StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private bool VerifyAccountForDeleteAction(ref string brokenRule)
        {
            if (CuentaActual is null)
            {
                brokenRule = "Seleccione una cuenta";
                return false;
            }
            else if (!CuentaActual.Editable)
            {
                brokenRule = "No se puede eliminar una cuenta principal del maestro de cuentas";
                return false;
            }
            else
            {
                brokenRule = string.Empty;
                return true;
            }
        }

        private void DeleteAccountFromList(AccountDTO cuenta)
        {
            _lstCuentas.Remove(cuenta);
            var padre = treeCuentas.SelectedNode.Parent;
            treeCuentas.Nodes.Remove(treeCuentas.SelectedNode);
            treeCuentas.SelectedNode = padre;
        }

        private async Task ExecuteDeleteAsync() => await _cuentaCL.DeleteAsync(GlobalConfig.company.Code, CuentaActual.Id);

        #endregion

        #region Edit
        private async void UpdateAccount(object sender, EventArgs e)
        {
            var nombreActualCuenta = CuentaActual.Name;
            var detalleActualCuenta = CuentaActual.Memo;

            try
            {
                if (ValidateChildren())
                {
                    CuentaActual.Memo = txtBoxDetalle.Text;
                    CuentaActual.Name = txtNombreInfo.Text;
                    await ExecuteUpdateAsync(CuentaActual);
                    DisableEditButtons();
                    treeCuentas.SelectedNode.Text = CuentaActual.Name;
                }
            }
            catch (Exception ex)
            {
                CuentaActual.Name = nombreActualCuenta;
                CuentaActual.Memo = detalleActualCuenta;
                MessageBox.Show(ex.Message, StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task ExecuteUpdateAsync(AccountDTO cuenta)
            => await _cuentaCL.UpdateAsyc(GlobalConfig.company.Code, cuenta);


        private void EditarCuenta(object sender, EventArgs e)
        {
            try
            {
                if (CuentaActual != null && CuentaActual.Editable)
                {
                    this.txtNombreInfo.ReadOnly = false;
                    this.txtNombreInfo.Focus();
                    this.txtBoxDetalle.ReadOnly = false;
                    EnableEditOptions();
                }
                else
                {
                    MessageBox.Show("Esta cuenta no puede ser editada", StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisableEditButtons()
        {
            this.txtNombreInfo.ReadOnly = true;
            this.txtBoxDetalle.ReadOnly = true;
            this.btnActualizarCuenta.Enabled = false;
            this.btnActualizarCuenta.Visible = false;
        }

        private void EnableEditOptions()
        {
            btnActualizarCuenta.Enabled = true;
            btnActualizarCuenta.Visible = true;
        }
        #endregion

        #endregion

        #region Load Account Info To Grid
        private void BuildAccountGeneralInformationDashboard()
        {
            txtNombreInfo.Text = CuentaActual.Name;
            txtTipoInfo.Text = CuentaActual.AccountTag.ToString().Replace('_', ' ');
            txtIndicadorInfo.Text = CuentaActual.AccountType.ToString().Replace('_', ' ');
            txtBoxDetalle.Text = CuentaActual.Memo;
            infoPanel.Tag = CuentaActual;
        }

        private async Task BuildAccountBalanceInformationDashboard()
        {
            IPostingPeriod startMonth = GetSelectedMonthInFechaInicio();
            IPostingPeriod endMonth = GetSelectedMonthInFechaFinal();

            try
            {
                if (startMonth != null && endMonth != null && CuentaActual != null)
                {
                    var _account = await _cuentaCL.GetMonthlyBalanceAsync(GlobalConfig.company.Code, CuentaActual, startMonth, endMonth);
                    PrintBalanceInCurPanel(_account);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void PrintBalanceInCurPanel(AccountDTO account)
        {
            DataGridViewRow row = BuildDataGridViewRow();

            if (gridDatosA.Visible)
            {
                CargarGridA(row, account);
            }
            else
            {
                CargarGridB(row, account);
            }
        }

        private void CargarGridA(DataGridViewRow row, AccountDTO cuenta)
        {
            row.CreateCells(gridDatosA);
            row.Cells[0].Value = cuenta.PriorBalance;
            row.Cells[1].Value = cuenta.GetDebitBalance();
            row.Cells[2].Value = cuenta.GetCreditBalance();
            row.Cells[3].Value = cuenta.CurrentBalance;
            gridDatosA.Rows.Add(row);
        }

        private void CargarGridB(DataGridViewRow row, AccountDTO cuenta)
        {
            row.CreateCells(gridDatosB);
            row.Cells[0].Value = cuenta.GetDebitBalance();
            row.Cells[1].Value = cuenta.GetCreditBalance();
            row.Cells[2].Value = cuenta.MontlyBalance;
            gridDatosB.Rows.Add(row);
        }

        private DataGridViewRow BuildDataGridViewRow()
        {
            gridDatosA.Rows.Clear();
            gridDatosB.Rows.Clear();
            DataGridViewRow row = new DataGridViewRow();
            return row;
        }

        private IPostingPeriod GetSelectedMonthInFechaFinal()
        {
            if (gridDatosA.Visible)
            {
                return AFechaFinal.SelectedItem as IPostingPeriod;
            }
            else if (gridDatosB.Visible)
            {
                return BFechaFinal.SelectedItem as IPostingPeriod;
            }
            else
            {
                throw new Exception();
            }
        }

        private IPostingPeriod GetSelectedMonthInFechaInicio()
        {
            if (gridDatosA.Visible)
            {
                return AFechaInicio.SelectedItem as IPostingPeriod ?? null;
            }
            else if (gridDatosB.Visible)
            {
                return BFechaInicio.SelectedItem as IPostingPeriod ?? null;
            }
            else
            {
                throw new Exception();
            }
        }

        #endregion

        #region LoadData
        private async void FrameMaestroCuentaLoad(object sender, EventArgs e)
        {
            await LoadAccountsAsync();
            ExpandirArbol(null, null);
            DisableComboBoxDatesEvents();
            LoadDatesAsync();
            EnableComboBoxDatesEvents();
        }

        private async Task LoadDatesAsync()
        {
            _lstFechas = await _fechaTransaccionCL.GetAllAsync(GlobalConfig.company.Code);
            ///Se añaden interfaces / copias (las interfaces no son copias) 
            ///la idea con eso es que cada item y de cada combo box tenga diferente hash
            var lstBfechasFnl = new List<IPostingPeriod> { (from c1 in _lstFechas select c1).OrderByDescending(x => x.Date).LastOrDefault() };

            AFechaInicio.DataSource = lstBfechasFnl;
            AFechaFinal.DataSource = (from c1 in _lstFechas select c1).ToList();
            AFechaFinal.SelectedIndex = -1;

            BFechaInicio.DataSource = (from c1 in _lstFechas select c1).ToList();
            BFechaFinal.DataSource = (from c1 in _lstFechas where c1.Date >= ((IPostingPeriod)BFechaInicio.SelectedItem).Date select c1).ToList();
            BFechaFinal.SelectedIndex = -1;
        }

        private async Task LoadAccountsAsync()
        {
            _lstCuentas.Clear();

            try
            {
                _lstCuentas = await _cuentaCL.GetAllAsync(GlobalConfig.company.Code);
                //cargar las cuentas al arbol
                treeCuentas.Nodes.AddRange(TreeViewCuentas.CrearTreeView(_lstCuentas));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void DisableComboBoxDatesEvents()
        {
            AFechaFinal.SelectedIndexChanged -= this.AFechaFinalSelectedIndexChanged;
            BFechaFinal.SelectedIndexChanged -= this.BFechaFinalSelectedIndexChanged;
        }

        private void EnableComboBoxDatesEvents()
        {
            AFechaFinal.SelectedIndexChanged += this.AFechaFinalSelectedIndexChanged;
            BFechaFinal.SelectedIndexChanged += this.BFechaFinalSelectedIndexChanged;
        }
        #endregion

        #region Open Windows
        private void OpenAccountReports(object sender, EventArgs e)
        {
            ReporteCuenta reporte = new ReporteCuenta(GlobalConfig.company, GlobalConfig.UserDTO)
            {
                MdiParent = this.MdiParent
            };
            reporte.Show();
        }

        private void OpenAccountActivityReport(object sender, EventArgs e)
        {
            //ReporteMovimientosCuenta frame = new ReporteMovimientosCuenta();
            //if (frame.TransferirCuenta(CuentaActual))
            //{
            //    frame.MdiParent = this.MdiParent;
            //    frame.Show();
            //}
            //else
            //{
            //    MessageBox.Show("Seleccione una cuenta valida", StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Hand);
            //}
        }

        #endregion

        #region Events
        private async void TabControlGeneralSelectedIndexChanged(object sender, EventArgs e)
        {
            await BuildAccountBalanceInformationDashboard();
        }

        private async void AFechaFinalSelectedIndexChanged(object sender, EventArgs e)
        {
            await BuildAccountBalanceInformationDashboard();
        }

        private async void BFechaFinalSelectedIndexChanged(object sender, EventArgs e)
        {

            await BuildAccountBalanceInformationDashboard();

        }

        private async void TreeCuentasAfterSelect(object sender, TreeViewEventArgs e)
        {
            DisableEditButtons();
            BuildAccountGeneralInformationDashboard();
            await BuildAccountBalanceInformationDashboard();
        }

        private void ConvertEnterUserKeyPressToTab(object sender, KeyPressEventArgs e)
        {
            if ((Keys)e.KeyChar == Keys.Enter)
            {
                e.Handled = true;
                SendKeys.Send("{TAB}");
            }
        }

        private void CerrarVentana(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ExpandirArbol(object sender, EventArgs e)
        {
            treeCuentas.ExpandAll();
        }

        private void ColapsarArbol(object sender, EventArgs e)
        {
            treeCuentas.CollapseAll();
        }

        private void LstMesInicioSelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControlGeneral.SelectedIndex == 1)
            {

                BFechaFinal.DataSource = (from n in _lstFechas where n.Date >= ((IPostingPeriod)BFechaInicio.SelectedItem).Date select n).ToList<IPostingPeriod>();
            }
        }
        #endregion

        #region Validations
        private void TxtNombreInfoValidating(object sender, CancelEventArgs e)
        {
            var txtNombre = txtNombreInfo.Text;

            if (string.IsNullOrEmpty(txtNombre))
            {
                var mensaje = "Nombre no puede ir en blanco!";
                SetErrorMessage(txtNombreInfo, mensaje, ref e, true);

            }
            else if (txtNombre != CuentaActual.Name && NameExist(txtNombre, CuentaActual.AccountTag))
            {
                var mensaje = "Nombre en uso!";
                SetErrorMessage(txtNombreInfo, mensaje, ref e, true);
            }
            else
            {
                SetErrorMessage(txtNombreInfo, string.Empty, ref e, false);
            }
        }

        private bool NameExist(string txtNombre, AccountTag tipoCuenta)
        {
            return _lstCuentas.All(x => x.AccountTag == tipoCuenta && x.Name == txtNombre);
        }

        private void SetErrorMessage(Control control, string message, ref CancelEventArgs e, bool cancel)
        {
            e.Cancel = cancel;
            AppErrorProvider.SetError(control, message);
        }
        #endregion

    }
}
