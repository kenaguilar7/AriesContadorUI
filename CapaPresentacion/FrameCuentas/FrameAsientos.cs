using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapaEntidad.Entidades.Asientos;
using CapaEntidad.Entidades.Cuentas;
using CapaEntidad.Entidades.FechaTransacciones;
using CapaEntidad.Enumeradores;
using CapaEntidad.Interfaces;
using CapaEntidad.Textos;
using CapaLogica;
using CapaPresentacion.Reportes;


namespace CapaPresentacion.FrameCuentas
{
    public partial class FrameAsientos : Form, ICallingForm
    {
        #region Properties
        private Asiento _asiento;
        private Transaccion TransaccionOnEdit = new Transaccion();
        private AsientoCL _asientoCL = new AsientoCL();
        private FechaTransaccionCL _fechaTransaccion = new FechaTransaccionCL();
        private TransaccionCL _transaccionCL = new TransaccionCL();
        #endregion

        public FrameAsientos()
        {
            InitializeComponent();
            AgregarEventos();
        }

        #region SetUpData
        private async void FrameAsientos_Load(object sender, EventArgs e)
        {
            ConfigExchangeController(GlobalConfig.Compañia.TipoMoneda);
            await LoadAccountingPeriodList();
        }
        private void AgregarEventos()
        {
            //this.lstMesesAbiertos.SelectedIndexChanged += new System.EventHandler(this.LstMesesAbiertos_SelectedIndexChanged);
            this.TxtMontoTotalTransaccion.KeyPress += IUserKeyPress;
            this.txtTipoCambio.KeyPress += IUserKeyPress;

        }
        private void CargarDatosPanelTransaction(Transaccion dummy)
        {

            TransaccionOnEdit = dummy;
            TransferirCuenta(dummy.CuentaDeAsiento);
            //establece cual radio button estara checado.
            if (dummy.ComportamientoCuenta == Comportamiento.Debito)
            { rDebitos.PerformClick(); }
            else { rCreditos.PerformClick(); }

            txtBoxReferencia.Text = dummy.Referencia;
            txtBoxDetalle.Text = dummy.Detalle;
            txtBoxFechaFactura.Text = dummy.FechaFactura.ToShortDateString();

            if (dummy.TipoCambio == TipoCambio.Dolares)
            {
                TxtMontoTotalTransaccion.Text = (dummy.Monto / dummy.MontoTipoCambio).ToString();
                lstTipoCambio.SelectedIndex = 1;
                txtTipoCambio.Text = dummy.MontoTipoCambio.ToString();
            }
            else
            {
                TxtMontoTotalTransaccion.Text = dummy.Monto.ToString();
                lstTipoCambio.SelectedIndex = 0;
            }
        }
        private void SetColorBalance()
        {

            this.txtTotalDebitos.Text = string.Format("{0:₡###,###,###,##0.00}", _asiento.DebitosColones);
            this.txtTotalCreditos.Text = string.Format("{0:₡###,###,###,##0.00}", _asiento.CreditosColones);

            if (_asiento.Cuadrado && (_asiento.Transaccions.Count != 0))
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

            foreach (var tr in _asiento.Transaccions)
            {

                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(GridDatos);
                row.Tag = tr;
                row.Cells[0].Value = tr.CuentaDeAsiento;
                row.Cells[1].Value = tr.Referencia;
                row.Cells[2].Value = tr.Detalle;
                row.Cells[3].Value = tr.FechaFactura;

                if (tr.ComportamientoCuenta == Comportamiento.Debito)
                {
                    row.Cells[4].Value = tr.Monto;
                    debitos += Convert.ToDecimal(tr.Monto);
                }
                else if (tr.ComportamientoCuenta == Comportamiento.Credito)
                {
                    row.Cells[5].Value = tr.Monto;
                    creditos += Convert.ToDecimal(tr.Monto);
                }
                row.Cells[6].Value = tr.TipoCambio.ToString();
                row.Cells[7].Value = tr.MontoTipoCambio;
                if (tr.TipoCambio == TipoCambio.Dolares)
                {
                    row.Cells[8].Value = tr.Monto / tr.MontoTipoCambio;
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
            var lst = await _fechaTransaccion.GetAllAsync(GlobalConfig.Compañia.Codigo);
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
            lstTipoCambio.SelectedIndex = 0;
            lstTipoCambio.Enabled = false;
        }
        private void CaseOnlyDolarsConfig()
        {
            ColumnTipoCambio.Visible = true;
            ColumnMontoDolares.Visible = true;
            lstTipoCambio.SelectedIndex = 1;
            lstTipoCambio.Enabled = false;
        }
        private void CaseColonsAndDolarsConfig()
        {
            ColumnTipoCambio.Visible = true;
            ColumnMontoDolares.Visible = true;
            lstTipoCambio.SelectedIndex = 0;
            lstTipoCambio.Enabled = true;
        }
        private void ConfigExchangeController(TipoMonedaCompañia tipoMoneda)
        {

            switch (tipoMoneda)
            {
                case TipoMonedaCompañia.Dolares_y_Colones:
                    CaseColonsAndDolarsConfig();
                    break;
                case TipoMonedaCompañia.Solo_Colones:
                    CaseOnlyDolarsConfig();
                    break;
                case TipoMonedaCompañia.Solo_Dolares:
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
        private async Task<IEnumerable<Asiento>> ConfigAsientoBorrador(IEnumerable<Asiento> asientos)
        {
            var newEntryNum = await _asientoCL.GetPreEntryAsync(GlobalConfig.Compañia.Codigo, GetFechTransaccionEnUso().Id);
            List<Asiento> newList = asientos.ToList();
            newList.Insert(0, newEntryNum);

            return newList;
        }
        private FechaTransaccion GetFechTransaccionEnUso()
        {
            return (FechaTransaccion)lstMesesAbiertos.SelectedItem;
        }
        private async void LstMesesAbiertosSelectedIndexChanged(object sender, EventArgs e)
        {
            if (VerificarAsiento())
            {
                try
                {
                    IEnumerable<Asiento> lst = await _asientoCL.GetAllAsync(GlobalConfig.Compañia.Codigo, GetFechTransaccionEnUso().Id);
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
        public bool TransferirCuenta(Cuenta cuenta)
        {
            if (cuenta.Indicador == IndicadorCuenta.Cuenta_Auxiliar)
            {
                this.txtBoxNombreCuenta.Tag = cuenta;
                this.txtBoxNombreCuenta.Text = cuenta.Nombre;
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
        public Transaccion GetNewTransaction()
        {
            var tr = new Transaccion();
            tr.CuentaDeAsiento = (txtBoxNombreCuenta.Tag != null) ? (Cuenta)txtBoxNombreCuenta.Tag : null;
            tr.Referencia = txtBoxReferencia.Text;
            tr.Detalle = txtBoxDetalle.Text;
            tr.FechaFactura = DateTime.Parse(txtBoxFechaFactura.Text);

            if (lstTipoCambio.SelectedIndex == 0)
            {
                tr.TipoCambio = TipoCambio.Colones;
                tr.MontoTipoCambio = 1.00m;
                tr.Monto = Convert.ToDecimal(TxtMontoTotalTransaccion.Text);
            }
            else
            {
                tr.TipoCambio = TipoCambio.Dolares;
                tr.MontoTipoCambio = Convert.ToDecimal(txtTipoCambio.Text);
                tr.Monto = Convert.ToDecimal(TxtMontoTotalTransaccion.Text) * tr.MontoTipoCambio; ///pasamos los dolares a colones
            }
            if (rDebitos.Checked)
            {
                tr.ComportamientoCuenta = Comportamiento.Debito;
            }
            else if (rCreditos.Checked)
            {
                tr.ComportamientoCuenta = Comportamiento.Credito;
            }

            var monto = Convert.ToDecimal(TxtMontoTotalTransaccion.Text) * Convert.ToDecimal(txtTipoCambio.Text);

            return tr;
        }
        private IEnumerable<Transaccion> GetSelectedItemsFromGrid()
        {
            var retorno = new List<Transaccion>();
            var lstTrns = this.GridDatos.SelectedRows;

            for (int i = 0; i < lstTrns.Count; i++)
            {
                retorno.Add((Transaccion)lstTrns[i].Tag);
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
                    var newTransaction = GetNewTransaction();
                    var asiento = await ValidateBookEntryForInsert(_asiento);
                    var newTrans = await ExecuteInsertTransactionAsync(newTransaction, asiento);
                    _asiento.Transaccions.Add(newTrans);
                    _asiento.Id = asiento.Id; 
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
        private void ReplaceTransactionInList(Transaccion newTransaction)
        {
            var tran = _asiento.Transaccions.First(x => x.Id == newTransaction.Id);
            var index = _asiento.Transaccions.IndexOf(tran);

            if (index != -1)
            {
                _asiento.Transaccions[index] = newTransaction;
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
            if (_asiento.Cuadrado)
            {
                _asiento.Estado = EstadoAsiento.Aprobado;//se cambia a proceso dentro del if. 
                await _asientoCL.UpdateAsync(GlobalConfig.Compañia.Codigo, GetFechTransaccionEnUso().Id, _asiento);
            }
            else
            {
                _asiento.Estado = EstadoAsiento.Proceso;//se cambia a proceso dentro del if. 
                await _asientoCL.UpdateAsync(GlobalConfig.Compañia.Codigo, GetFechTransaccionEnUso().Id, _asiento);
            }
        }
        private async Task<Transaccion> ExecuteInsertTransactionAsync(Transaccion newTransaction, Asiento asiento)
        => await _transaccionCL.InsertAsync(newTransaction, asiento.Id);
        private async Task ExecuteDeleteTransactionAsync(IEnumerable<Transaccion> transacciones)
        {
            foreach (var trns in transacciones)
            {
                var _trans = (Transaccion)trns;
                await _transaccionCL.DeleteAsync(_trans.Id, _asiento.Id);
                _asiento.Transaccions.Remove(_trans);

            }
        }
        private async Task ExecuteUpdateTransactionAsync(Transaccion newTransaction)
                    => await _transaccionCL.UpdateAsync(newTransaction, _asiento.Id);
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
                TransaccionOnEdit = (Transaccion)this.GridDatos.SelectedRows[0].Tag;
                CargarDatosPanelTransaction(TransaccionOnEdit);
                BuildUpdateButtons(
                    availableInsertButton: false,
                    availableUpdateButton: true
                    );


            }


        }
        #endregion

        #region CRUD Asientos
        private async Task<Asiento> ValidateBookEntryForInsert(Asiento asiento)
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
        private async Task<Asiento> ExecuteInserBookEntryAsync(Asiento asiento)
            => await _asientoCL.InsertAsync(GlobalConfig.Compañia.Codigo, GetFechTransaccionEnUso().Id, asiento);
        private async void BtnEliminarAsientoClick(object sender, EventArgs e)
        {
            if (_asiento.Id == 0)
            {
                MessageBox.Show("No se puede eliminar este asiento porque aun no ha sido registrado", StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (MessageBox.Show("Se eliminara este asiento desea continuar", StaticInfoString.NombreApp, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    await _asientoCL.DeleteAsync(GlobalConfig.Compañia.Codigo, GetFechTransaccionEnUso().Id, _asiento.Id);
                    MessageBox.Show("Asiento eliminado correctamente", StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _asiento.Id = 0;
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

            if (_asiento.Id == 0)
            {
                try
                {
                    _asiento = await ExecuteInserBookEntryAsync(_asiento);
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
                _asiento = (Asiento)lstNumeroAsientos.SelectedItem;
                var lst = await _transaccionCL.GetALlAsync(_asiento.Id);
                _asiento.Transaccions = lst.ToList();
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
                ConfigExchangeController(GlobalConfig.Compañia.TipoMoneda);
                this.TxtMontoTotalTransaccion.Clear();
                lstNumeroAsientos.SelectedIndex = 0;
                txtBoxNombreCuenta.Text = "";
                txtBoxNombreCuenta.Tag = null;
                btnAgregarTransa.Text = "Agregar";
                if (_asiento.Id != 0)
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
            if (_asiento != null && _asiento.Id != 0 && !_asiento.Cuadrado)
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
                    n.fechaTransaccions.Add((FechaTransaccion)item);
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

                txtPathCuenta.Text = $"Ruta: {((Transaccion)this.GridDatos.SelectedRows[0].Tag).CuentaDeAsiento.PathDirection}";
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
            if (decimal.TryParse(txtTipoCambio.Text, out decimal dummy))
            {
                ////Buscar para que con el tipo de cambio se defina la cantidad de numeros 
                if (dummy <= 0)
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
            return (txtBoxNombreCuenta.Tag is Cuenta) ? true : false;
        }
        private void SetErrorMessage(Control control, string message, ref CancelEventArgs e, bool cancel)
        {
            e.Cancel = cancel;
            AppErrorProvider.SetError(control, message);
        }
        #endregion
    }
}
