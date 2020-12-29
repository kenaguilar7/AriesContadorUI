using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AriesContador.Entities.Financial.Accounts;
using AriesContador.Entities.Financial.JournalEntries;
using AriesContador.Entities.Financial.PostingPeriods;
using AriesContador.Entities.Utils;
using CapaLogica;
using CapaPresentacion.Reportes;
using CapaPresentacion.utils;

namespace CapaPresentacion.FrameCuentas
{
    public partial class FrameAsientos : Form, ICallingForm
    {
        #region Properties
        private JournalEntryDTO journalEntry;
        private JournalEntryLineDTO TransaccionOnEdit { get; set; }
        private CuentaCL _cuentaCL = new CuentaCL();
        private AsientoCL _asientoCL = new AsientoCL();
        private FechaTransaccionCL _fechaTransaccion = new FechaTransaccionCL();
        private TransaccionCL _transaccionCL = new TransaccionCL();
        private AccountDTO AccountInTxtBoxNombreCuenta
        {
            get
            {
                if (txtBoxNombreCuenta.Tag != null)
                {
                    return txtBoxNombreCuenta.Tag as AccountDTO;
                }
                else
                {
                    return null;
                }
            }
        }
        private PostingPeriodDTO PostingPeriodOnSelect
        {
            get { return (PostingPeriodDTO)lstMesesAbiertos.SelectedItem; }
        }

        #endregion

        public FrameAsientos()
        {
            InitializeComponent();
            AgregarEventos();
        }

        #region SetUpData
        private async void FrameAsientos_Load(object sender, EventArgs e)
        {
            ConfigExchangeController(GlobalConfig.company.CurrencyType);
            await LoadAccountingPeriodList();
        }
        private void AgregarEventos()
        {
            //this.lstMesesAbiertos.SelectedIndexChanged += new System.EventHandler(this.LstMesesAbiertos_SelectedIndexChanged);
            this.TxtMontoTotalTransaccion.KeyPress += IUserKeyPress;
            this.txtTipoCambio.KeyPress += IUserKeyPress;

        }
        private void CargarDatosPanelTransaction(JournalEntryLineDTO jLine)
        {

            TransaccionOnEdit = jLine;

            var account = new AccountDTO() { Name = jLine.AccountName, Id = jLine.AccountId, AccountType = AccountType.Cuenta_Auxiliar };


            TransferirCuenta(account);
            //establece cual radio button estara checado.
            if (jLine.DebOCred == DebOCred.Debito)
            { rDebitos.PerformClick(); }
            else { rCreditos.PerformClick(); }

            txtBoxReferencia.Text = jLine.Reference;
            txtBoxDetalle.Text = jLine.Memo;
            txtBoxFechaFactura.Text = jLine.Date.ToShortDateString();

            CargaMontosEnPanelDeTransaccion(jLine);
        }

        private void CargaMontosEnPanelDeTransaccion(JournalEntryLineDTO jLine)
        {
            if (jLine.Currency == Currency.dolares)
            {
                TxtMontoTotalTransaccion.Text = jLine.ForenignAmount.ToString();
                lstTipoCambio.SelectedItem = Currency.dolares;
                txtTipoCambio.Text = jLine.Rate.ToString();
            }
            else
            {
                TxtMontoTotalTransaccion.Text = jLine.Amount.ToString();
                lstTipoCambio.SelectedItem = Currency.colones;
            }
        }

        private void SetColorBalance()
        {

            this.txtTotalDebitos.Text = string.Format("{0:₡###,###,###,##0.00}", journalEntry.DebitBalance);
            this.txtTotalCreditos.Text = string.Format("{0:₡###,###,###,##0.00}", journalEntry.CreditBalance);
            var cuadrado = (journalEntry.DebitBalance == journalEntry.CreditBalance) ? true : false;

            if (cuadrado && (journalEntry.JournalEntryLines.Count != 0))
            {
                txtTotalCreditos.ForeColor = Color.Green;
                txtTotalDebitos.ForeColor = Color.Green;
            }
            else
            {
                txtTotalCreditos.ForeColor = Color.Black;
                txtTotalDebitos.ForeColor = Color.Black;
            }
        }
        private void LimpiarPanelDatosAsiento()
        {
            this.SetColorBalance();
            this.txtBoxReferencia.Clear();
            this.txtBoxFechaFactura.Clear();
            this.txtBoxDetalle.Clear();
            this.TxtMontoTotalTransaccion.Clear();
        }
        private void UpdateView()
        {
            GridDatos.Rows.Clear();

            decimal debitos = 0;
            decimal creditos = 0;

            foreach (var tr in journalEntry.JournalEntryLines)
            {

                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(GridDatos);
                row.Tag = tr;
                row.Cells[0].Value = tr.AccountName;
                row.Cells[1].Value = tr.Reference;
                row.Cells[2].Value = tr.Memo;
                row.Cells[3].Value = tr.Date;

                if (tr.DebOCred == DebOCred.Debito)
                {
                    row.Cells[4].Value = tr.Amount;
                    debitos += tr.Amount;
                }
                else if (tr.DebOCred == DebOCred.Credito)
                {
                    row.Cells[5].Value = tr.Amount;
                    creditos += tr.Amount;
                }

                row.Cells[6].Value = tr.Currency.ToString();
                row.Cells[7].Value = tr.Rate;

                if (tr.Currency == Currency.dolares)
                {

                    row.Cells[8].Value = tr.ForenignAmount;
                }
                else
                {
                    row.Cells[8].Value = 00.00;
                }
                GridDatos.Rows.Add(row);

            }
            SetColorBalance();
        }
        private async Task LoadAccountingPeriodList()
        {
            var lst = await _fechaTransaccion.GetAllAsync(GlobalConfig.company.Code);
            lstMesesAbiertos.DataSource = lst;


            if (lstMesesAbiertos.Items.Count == 0)
            {
                btnAgregarTransa.Enabled = false;
            }
            else { }

        }
        private void CaseOnlyColonsConfig()
        {
            ColumnTipoCambio.Visible = false;
            ColumnMontoDolares.Visible = false;
            lstTipoCambio.SelectedItem = Currency.colones;
            lstTipoCambio.Enabled = false;
        }
        private void CaseOnlyDolarsConfig()
        {
            ColumnTipoCambio.Visible = true;
            ColumnMontoDolares.Visible = true;
            lstTipoCambio.SelectedItem = Currency.dolares;
            lstTipoCambio.Enabled = false;
        }
        private void CaseColonsAndDolarsConfig()
        {
            ColumnTipoCambio.Visible = true;
            ColumnMontoDolares.Visible = true;
            lstTipoCambio.SelectedItem = Currency.colones;
            lstTipoCambio.Enabled = true;
        }
        private void ConfigExchangeController(CurrencyTypeCompany tipoMoneda)
        {
            lstTipoCambio.DataSource = Enum.GetValues(typeof(Currency));

            switch (tipoMoneda)
            {
                case CurrencyTypeCompany.Dolares_y_Colones:
                    CaseColonsAndDolarsConfig();
                    break;
                case CurrencyTypeCompany.Solo_Colones:
                    CaseOnlyDolarsConfig();
                    break;
                case CurrencyTypeCompany.Solo_Dolares:
                    CaseOnlyColonsConfig();
                    break;
                default:
                    break;
            }

            //lstTipoCambio.Enabled = (GlobalConfig.Compañia.TipoMoneda == TipoMonedaCompañia.Dolares_y_Colones) ? true : DummyTipoCambio();
            //Boolean DummyTipoCambio()
            //{
            //    lstTipoCambio.SelectedIndex = (GlobalConfig.Compañia.TipoMoneda == TipoMonedaCompañia.Solo_Colones) ? 0 : 1;
            //    return false;
            //}

        }
        private async Task<IEnumerable<JournalEntryDTO>> ConfigAsientoBorrador(IEnumerable<JournalEntryDTO> asientos)
        {
            var newEntryNum = await _asientoCL.GetPreEntryAsync(GetFechTransaccionEnUso().Id);

            var newJEnt = new JournalEntryDTO()
            {
                Number = newEntryNum,
                PostingPeriodId = PostingPeriodOnSelect.Id,
                JournalEntryStatus = JournalEntryStatus.Proceso,
                UpdatedBy = GlobalConfig.UserDTO.Id,
                CreatedBy = GlobalConfig.UserDTO.Id
            };

            List<JournalEntryDTO> jEntries = asientos.ToList();
            jEntries.Add(newJEnt);

            return jEntries.OrderByDescending(x => x.Number).ToArray();
        }
        private PostingPeriodDTO GetFechTransaccionEnUso()
        {
            return (PostingPeriodDTO)lstMesesAbiertos.SelectedItem;
        }
        private async void LstMesesAbiertosSelectedIndexChanged(object sender, EventArgs e)
        {
            if (VerificarAsiento())
            {
                try
                {
                    IEnumerable<JournalEntryDTO> lst = await _asientoCL.GetAllAsync(GetFechTransaccionEnUso().Id);
                    lstNumeroAsientos.DataSource = await ConfigAsientoBorrador(lst);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion

        #region Button Config Events
        private void FrameAsientos_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar == '\u0013'))
            {
                ///CTR + S
                btnSeleccionarCuenta_Click(sender, e);

            }
            else if (e.KeyChar == '\u0004')
            {
                ///CTR + D
                rDebitos.Checked = true;
            }
            else if (e.KeyChar == '\u0003')
            {
                ///CTR + C
                rCreditos.Checked = true;
            }
            else if (e.KeyChar == '\u0001' && btnAgregarTransa.Visible)
            {
                ///CTRL + A
                this.BtnAgregarTransaccion(sender, e);
            }
            else if (e.KeyChar == '\u0012' && btnActualizarTrans.Visible)
            {
                ///CTR + R
                this.BtnActualizarTrans_Click(sender, e);
            }


        }
        public bool TransferirCuenta(AccountDTO cuenta)
        {
            if (cuenta.AccountType == AccountType.Cuenta_Auxiliar)
            {
                this.txtBoxNombreCuenta.Tag = cuenta;
                this.txtBoxNombreCuenta.Text = cuenta.Name;
                this.rDebitos.Focus();
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region CRUD Transaccion



        public JournalEntryLineDTO GetNewTransaction()
        {
            var tr = new JournalEntryLineDTO
            {
                AccountId = AccountInTxtBoxNombreCuenta.Id,
                Reference = txtBoxReferencia.Text,
                Memo = txtBoxDetalle.Text,
                Date = DateTime.Parse(txtBoxFechaFactura.Text),
                JournalEntryId = journalEntry.Id,
                CreatedBy = GlobalConfig.UserDTO.Id,
                UpdatedBy = GlobalConfig.UserDTO.Id,
            };

            if ((Currency)lstTipoCambio.SelectedItem == Currency.colones)
            {
                tr.Currency = Currency.colones;
                tr.Rate = 1.00m;
                tr.Amount = Convert.ToDecimal(TxtMontoTotalTransaccion.Text);
            }
            else if ((Currency)lstTipoCambio.SelectedItem == Currency.dolares)
            {
                tr.Currency = Currency.dolares;
                tr.Rate = Convert.ToDecimal(txtTipoCambio.Text);
                tr.ForenignAmount = Convert.ToDecimal(TxtMontoTotalTransaccion.Text);
                tr.Amount = tr.ForenignAmount * tr.Rate;
            }
            else
            {
                //
            }

            tr.DebOCred = (rDebitos.Checked) ? DebOCred.Debito : DebOCred.Credito;

            return tr;

        }
        private IEnumerable<JournalEntryLineDTO> GetSelectedItemsFromGrid()
        {
            var retorno = new List<JournalEntryLineDTO>();
            var lstTrns = this.GridDatos.SelectedRows;

            for (int i = 0; i < lstTrns.Count; i++)
            {
                retorno.Add((JournalEntryLineDTO)lstTrns[i].Tag);
            }
            return retorno;
        }
        private async void BtnAgregarTransaccion(object sender, EventArgs e)
        {
            if (!ValidateAccount())
            {
                MessageBox.Show("Seleccione una cuenta valida", StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (ValidateChildren())
            {
                try
                {
                    var asiento = await ValidateBookEntryForInsert(journalEntry);
                    journalEntry.Id = asiento.Id;
                    var newTransaction = GetNewTransaction();
                    var newTrans = await ExecuteInsertTransactionAsync(newTransaction);
                    journalEntry.JournalEntryLines.Add(newTrans);
                    UpdateView();
                    LimpiarPanelDatosAsiento();
                    await VerificarAsientoCuadrado();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            else
            {
                //do nothing 
            }
        }
        private async void BtnActualizarTrans_Click(object sender, EventArgs e)
        {

            if (ValidateChildren())
            {
                try
                {
                    var newTransaction = GetNewTransaction();
                    newTransaction.Id = TransaccionOnEdit.Id;

                    await ExecuteUpdateTransactionAsync(newTransaction);
                    ReplaceTransactionInList(newTransaction);

                    BuildUpdateButtons(
                         availableUpdateButton: false,
                         availableInsertButton: true
                         );

                    UpdateView();
                    LimpiarPanelDatosAsiento();

                    await VerificarAsientoCuadrado();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            else
            {
                //do nothing 
            }
        }
        private void ReplaceTransactionInList(JournalEntryLineDTO newTransaction)
        {
            var tran = journalEntry.JournalEntryLines.First(x => x.Id == newTransaction.Id);
            var index = journalEntry.JournalEntryLines.IndexOf(tran);

            if (index != -1)
            {
                journalEntry.JournalEntryLines[index] = newTransaction;
            }
        }
        private async void BtbEliminar_Click(object sender, EventArgs e)
        {
            var lstTrns = GetSelectedItemsFromGrid();

            if ((lstTrns.Count() > 0) && MessageBox.Show($"Se van a eliminar {lstTrns.Count()} elementos ¿Desea continuar?", StaticInfoString.NombreApp, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    await ExecuteDeleteTransactionAsync(lstTrns);
                    await VerificarAsientoCuadrado();
                    this.UpdateView();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (lstTrns.Count() == 0)
            {
                MessageBox.Show("Seleccione una transacción", StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }
        private async Task VerificarAsientoCuadrado()
        {
            if (journalEntry.EqualDebitAndCredit)
            {
                journalEntry.JournalEntryStatus = JournalEntryStatus.Aprobado;//se cambia a proceso dentro del if. 
                await _asientoCL.UpdateAsync(journalEntry);
            }
            else
            {
                journalEntry.JournalEntryStatus = JournalEntryStatus.Proceso;//se cambia a proceso dentro del if. 
                await _asientoCL.UpdateAsync(journalEntry);
            }
        }
        private async Task<JournalEntryLineDTO> ExecuteInsertTransactionAsync(JournalEntryLineDTO newTransaction)
        => await _transaccionCL.InsertAsync(newTransaction);
        private async Task ExecuteDeleteTransactionAsync(IEnumerable<JournalEntryLineDTO> transacciones)
        {
            foreach (var trns in transacciones)
            {
                var _trans = (JournalEntryLineDTO)trns;
                await _transaccionCL.DeleteAsync(_trans);
                journalEntry.JournalEntryLines.Remove(_trans);
            }
        }
        private async Task ExecuteUpdateTransactionAsync(JournalEntryLineDTO newTransaction)
                    => await _transaccionCL.UpdateAsync(newTransaction);
        private void BuildUpdateButtons(bool availableInsertButton, bool availableUpdateButton)
        {
            btnAgregarTransa.Visible = availableInsertButton;
            btnActualizarTrans.Visible = availableUpdateButton;
        }
        private void BtnCargarTransaccionAlPanelDeEdicion(object sender, EventArgs e)
        {
            var adummy = this.GridDatos.SelectedRows;

            if (adummy.Count != 1)
            {
                MessageBox.Show("Seleccione una transacción", StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                TransaccionOnEdit = (JournalEntryLineDTO)this.GridDatos.SelectedRows[0].Tag;
                CargarDatosPanelTransaction(TransaccionOnEdit);
                BuildUpdateButtons(
                    availableInsertButton: false,
                    availableUpdateButton: true
                    );


            }


        }
        #endregion

        #region CRUD Asientos
        private async Task<JournalEntryDTO> ValidateBookEntryForInsert(JournalEntryDTO asiento)
        {
            if (asiento.Id == 0)
            {
                return await ExecuteInserBookEntryAsync(asiento);
            }
            else
            {
                return asiento;
            }
        }
        private async Task<JournalEntryDTO> ExecuteInserBookEntryAsync(JournalEntryDTO asiento)
            => await _asientoCL.InsertAsync(asiento);
        private async void BtnEliminarAsientoClick(object sender, EventArgs e)
        {
            if (journalEntry.Id == 0)
            {
                MessageBox.Show("No se puede eliminar este asiento porque aun no ha sido registrado", StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (MessageBox.Show("Se eliminara este asiento desea continuar", StaticInfoString.NombreApp, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    await _asientoCL.DeleteAsync(journalEntry);
                    MessageBox.Show("Asiento eliminado correctamente", StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    journalEntry.Id = 0;
                    FrameAsientos_Load(sender, e);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, StaticInfoString.MensajeBannerError, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }
        private async void BtnNuevoAsiento_Click(object sender, EventArgs e)
        {
            ///Nos aseguramos que el asiento que este seleccionado sea el ultimo
            lstNumeroAsientos.SelectedIndex = 0;

            if (journalEntry.Id == 0)
            {
                try
                {
                    journalEntry = await ExecuteInserBookEntryAsync(journalEntry);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            LstMesesAbiertosSelectedIndexChanged(null, null);
        }
        private async void LstNumeroAsientos_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (VerificarAsiento())
            {
                journalEntry = (JournalEntryDTO)lstNumeroAsientos.SelectedItem;
                var lst = await _transaccionCL.GetALlAsync(journalEntry.Id);
                journalEntry.JournalEntryLines = lst.ToList();
                UpdateView();
            }

        }

        #endregion

        #region Events
        private void BtnLimpiar_Click(object sender, EventArgs e)
        {
            if (VerificarAsiento())
            {
                this.GridDatos.Rows.Clear();
                this.SetColorBalance();
                this.txtBoxReferencia.Clear();
                this.txtBoxFechaFactura.Clear();
                this.txtBoxDetalle.Clear();
                ConfigExchangeController(GlobalConfig.company.CurrencyType);
                this.TxtMontoTotalTransaccion.Clear();
                lstNumeroAsientos.SelectedIndex = 0;
                txtBoxNombreCuenta.Text = "";
                txtBoxNombreCuenta.Tag = null;
                btnAgregarTransa.Text = "Agregar";
                if (journalEntry.Id != 0)
                {
                    BtnNuevoAsiento_Click(null, null);
                }
                BuildUpdateButtons(
                     availableInsertButton: true,
                     availableUpdateButton: false);
            }
        }
        private Boolean VerificarAsiento()
        {

            if (journalEntry != null && journalEntry.Id != 0 && !journalEntry.EqualDebitAndCredit)
            {
                this.WindowState = FormWindowState.Normal;
                MessageBox.Show("Este asiento se encuentra descuadrado, cuadre el asiento antes de salir", StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            else
            {
                return true;
            }
        }
        private void BtnReporte_Click(object sender, EventArgs e)
        {
            try
            {
                ReporteAsientos n = new ReporteAsientos();
                n.MdiParent = this.MdiParent;
                foreach (var item in lstMesesAbiertos.Items)
                {
                    n.fechaTransaccions.Add((PostingPeriodDTO)item);
                }
                n.Commit();
                n.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void LstTipoCambio_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstTipoCambio.SelectedIndex == 0)
            {
                txtTipoCambio.Enabled = false;
                txtTipoCambio.Text = "1.00";
            }
            else
            {
                txtTipoCambio.Enabled = true;
                txtTipoCambio.Clear();
            }

        }
        private void IUserKeyPress(object sender, KeyPressEventArgs e)
        {
            if ((Keys)e.KeyChar == Keys.Enter)
            {
                e.Handled = true;
                SendKeys.Send("{TAB}");
            }
        }
        private void Cerrar(object sender, EventArgs e)
        {
            this.Close();
        }
        private void FrameAsientos_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !VerificarAsiento();
        }
        private void GridDatos_SelectionChanged(object sender, EventArgs e)
        {
            if (GridDatos.SelectedRows.Count > 0)
            {

                txtPathCuenta.Text = $"Ruta: {((JournalEntryLineDTO)this.GridDatos.SelectedRows[0].Tag).AccountPath}";
            }
            else
            {
                txtPathCuenta.Text = "";
            }
        }
        private void btnSeleccionarCuenta_Click(object sender, EventArgs e)
        {
            FrameSeleccionCuenta n = new FrameSeleccionCuenta(this);

            n.ShowDialog();
        }

        #endregion

        #region Verifications
        private void maskedTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
                (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }
        private void tb_TextChanged(object sender, EventArgs e)
        {
            ///si se llega a este punto es porque si era un numero o un . (punto)

            string value = TxtMontoTotalTransaccion.Text.Replace(",", "");
            value = (value == ".") ? "0." : value;
            if (decimal.TryParse(value, out decimal ul))
            {
                TxtMontoTotalTransaccion.TextChanged -= tb_TextChanged;

                ///Primero todo lo que hace si comienza con cero

                if (ul == 0)
                {
                    if (value.StartsWith("."))
                    {
                        TxtMontoTotalTransaccion.Text = string.Format("{0:0.}", ul);
                        TxtMontoTotalTransaccion.Text += ".";
                        TxtMontoTotalTransaccion.SelectionStart = TxtMontoTotalTransaccion.Text.Length;
                    }
                    else if (value.EndsWith("."))
                    {
                        TxtMontoTotalTransaccion.Text = string.Format("{0:0.}", ul);
                        TxtMontoTotalTransaccion.Text += ".";
                        TxtMontoTotalTransaccion.SelectionStart = TxtMontoTotalTransaccion.Text.Length;
                    }
                    else if (value.IndexOf('.') != 1 && ul == 0)
                    {
                        TxtMontoTotalTransaccion.Text = string.Format("{0:0}", ul);
                        TxtMontoTotalTransaccion.SelectionStart = TxtMontoTotalTransaccion.Text.Length;
                    }

                }
                else if (!value.Contains("."))
                {
                    TxtMontoTotalTransaccion.Text = string.Format("{0:#,#}", ul);
                    TxtMontoTotalTransaccion.SelectionStart = TxtMontoTotalTransaccion.Text.Length;
                }
                else
                {

                    if (value.IndexOf(".") + 2 < value.Length - 1)
                    {
                        var ss = value.IndexOf('.');
                        TxtMontoTotalTransaccion.Text = string.Format("{0:#,#.##}", Convert.ToDecimal(value.Substring(0, value.Length - 1)));
                        //if (value.EndsWith("0"))
                        //{
                        //    textBox1.Text += ".00";
                        //}

                        TxtMontoTotalTransaccion.SelectionStart = TxtMontoTotalTransaccion.Text.Length;
                    }
                }
                TxtMontoTotalTransaccion.TextChanged += tb_TextChanged;
            }
        }
        private void txtTipoCambio_TextChanged(object sender, EventArgs e)
        {

            if (decimal.TryParse(txtTipoCambio.Text, out decimal ul))
            {
                if (txtTipoCambio.Text.Contains("."))
                {
                    var slp = txtTipoCambio.Text.Split(new char[] { '.' });
                    txtTipoCambio.MaxLength = slp[0].Length + 3;

                }
                else
                {
                    txtTipoCambio.MaxLength = 4;
                }

            }
        }
        private void txtBoxReferencia_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtBoxReferencia.Text))
            {
                SetErrorMessage(txtBoxReferencia, "Referencia no puede ir en blanco!", ref e, true);
            }
            else
            {
                SetErrorMessage(txtBoxReferencia, string.Empty, ref e, false);
            }
        }
        private void txtBoxDetalle_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBoxDetalle.Text))
            {
                SetErrorMessage(txtBoxDetalle, "Detalle no puede ir en blanco!", ref e, true);
            }
            else
            {
                SetErrorMessage(txtBoxDetalle, string.Empty, ref e, false);
            }
        }
        private void fechaFactura_Validating(object sender, CancelEventArgs e)
        {
            if (DateTime.TryParse(txtBoxFechaFactura.Text, out DateTime sr))
            {
                if (sr.Year < 1000 || sr.Year > 9999)
                {
                    SetErrorMessage(txtBoxFechaFactura, "Formato de fecha incorrecto", ref e, true);
                }
                else
                {
                    SetErrorMessage(txtBoxFechaFactura, string.Empty, ref e, false);
                }
            }
            else
            {
                SetErrorMessage(txtBoxFechaFactura, "Formato de fecha incorrecto", ref e, true);
            }
        }
        private void txtTipoCambio_Validating(object sender, CancelEventArgs e)
        {
            var tipoCambioString = txtTipoCambio.Text;
            var canParse = decimal.TryParse(tipoCambioString, out decimal tipoCambio);

            if (canParse)
            {
                ////Buscar para que con el tipo de cambio se defina la cantidad de numeros 
                if (tipoCambio <= 0)
                {
                    SetErrorMessage(txtTipoCambio, "El tipo de cambio no puede ser cero o menor a cero", ref e, true);
                }
                else
                {
                    SetErrorMessage(txtTipoCambio, string.Empty, ref e, false);
                }
            }
            else
            {
                SetErrorMessage(txtTipoCambio, "El tipo de cambio no puede ser cero o menor a cero", ref e, true);
            }
        }
        private void textBox1_Validating(object sender, CancelEventArgs e)
        {
            if (TxtMontoTotalTransaccion.Text.Length == 0)
            {
                TxtMontoTotalTransaccion.TextChanged -= tb_TextChanged;
                TxtMontoTotalTransaccion.Text = "0.00";
                TxtMontoTotalTransaccion.TextChanged += tb_TextChanged;
            }
            if (decimal.TryParse(TxtMontoTotalTransaccion.Text, out decimal dummys))
            {
                SetErrorMessage(TxtMontoTotalTransaccion, string.Empty, ref e, false);
            }
            else
            {
                SetErrorMessage(TxtMontoTotalTransaccion, "Formato de monto incorrecto", ref e, true);
            }
        }
        private bool ValidateAccount()
        {
            return (txtBoxNombreCuenta.Tag is AccountDTO) ? true : false;
        }
        private void SetErrorMessage(Control control, string message, ref CancelEventArgs e, bool cancel)
        {
            e.Cancel = cancel;
            AppErrorProvider.SetError(control, message);
        }
        #endregion
    }
}
