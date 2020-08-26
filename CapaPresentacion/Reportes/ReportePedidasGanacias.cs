using CapaEntidad.Entidades.Cuentas;
using CapaEntidad.Entidades.FechaTransacciones;
using CapaEntidad.Enumeradores;
using CapaEntidad.Reportes;
using CapaEntidad.Textos;
using CapaLogica;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapaPresentacion.Reportes
{
    public partial class ReportePedidasGanacias : Form
    {
        private CuentaCL CuentaCL { get; } = new CuentaCL();
        private FechaTransaccionCL FechaTransaccionCL { get; } = new FechaTransaccionCL();
        private IEnumerable<Cuenta> _lstExcel { get; set; }

        private FechaTransaccion _StartPostingPeriod { get => StartPostingPeriod.SelectedItem as FechaTransaccion; }
        private FechaTransaccion _EndPostingPeriod { get => EndPostingPeriod.SelectedItem as FechaTransaccion; }

        public ReportePedidasGanacias()
        {
            InitializeComponent();
        }

        private async void ReportePedidasGanacias_Load(object sender, EventArgs e)
        {
            await LoadAccounts();
            await LoadPostingPeriods();
            CrearColumnasParaNombre();
        }

        private async Task LoadAccounts()
        {
            var lst = await CuentaCL.GetAllAsync(GlobalConfig.Compañia.Codigo);
            _lstExcel = from c in lst where c.TipoCuenta.TipoCuenta == NombreTipoCuenta.Ingreso || c.TipoCuenta.TipoCuenta == NombreTipoCuenta.Egreso || c.TipoCuenta.TipoCuenta == NombreTipoCuenta.Costo_Venta select c;
        }

        private async Task LoadPostingPeriods()
        {
            var lstFechas = await FechaTransaccionCL.GetAllAsync(GlobalConfig.Compañia.Codigo);
            FillDropDownLists(lstFechas);
        }

        private void FillDropDownLists(IEnumerable<FechaTransaccion> lstFechas)
        {
            EndPostingPeriod.DataSource = lstFechas;
            StartPostingPeriod.DataSource = CreateDefaulListForStartPostingPeriod(lstFechas);
        }

        private static List<FechaTransaccion> CreateDefaulListForStartPostingPeriod(IEnumerable<FechaTransaccion> lstFechas)
        {
            return new List<FechaTransaccion> { (from c1 in lstFechas select c1).OrderByDescending(x => x.Fecha).LastOrDefault() };
        }

        private async void BtnCalcular(object sender, EventArgs e)
        {
            try
            {
                _lstExcel = await CuentaCL.GetAllAccountBalanceWithJournalEntriesRangeAsync(GlobalConfig.Compañia.Codigo, _StartPostingPeriod.Id, _EndPostingPeriod.Id);
                CargarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarFormulario()
        {
            GridDatos.Rows.Clear();

            foreach (var cuenta in _lstExcel)
            {
                if (cuenta.Indicador == IndicadorCuenta.Cuenta_Titulo)
                {

                    foreach (var acntAux in _lstExcel)
                    {
                        if (acntAux.Indicador != IndicadorCuenta.Cuenta_Titulo
                            && acntAux.TipoCuenta.TipoCuenta == cuenta.TipoCuenta.TipoCuenta)
                        {
                            DataGridViewRow row = new DataGridViewRow();
                            row.CreateCells(GridDatos);

                            var acntname = acntAux.GetNombreParaExcel(_lstExcel.ToList());

                            row.Cells[acntname.Length - 1].Value = acntname.Last();

                            row.Cells[GridDatos.Columns.Count - (acntname.Length)].Value = acntAux.SaldoActualColones;
                            GridDatos.Rows.Add(row);


                        }
                    }

                    DataGridViewRow rowTotal = new DataGridViewRow();
                    rowTotal.CreateCells(GridDatos);

                    var nameTotal = cuenta.GetNombreParaExcel(_lstExcel.ToList());
                    rowTotal.Cells[nameTotal.Length - 1].Value = $"TOTAL {nameTotal.Last()}";
                    rowTotal.DefaultCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    rowTotal.Cells[GridDatos.Columns.Count - (nameTotal.Length)].Value = cuenta.SaldoActualColones;
                    GridDatos.Rows.Add(rowTotal);
                }
            }

            DataGridViewRow rowTotalBalance = new DataGridViewRow();
            rowTotalBalance.CreateCells(GridDatos);


            rowTotalBalance.Cells[0].Value = $"UTILIDAD/PERDIDA PERIODO";
            rowTotalBalance.DefaultCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            rowTotalBalance.Cells[GridDatos.Columns.Count - 1].Value = TotalPerdida;
            GridDatos.Rows.Add(rowTotalBalance);

        }
        private void tbnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void CrearColumnasParaNombre()
        {

            var maxValue = (from c in _lstExcel orderby c.GetNombreParaExcel(_lstExcel.ToList()).Length select new { top = c.GetNombreParaExcel(_lstExcel.ToList()).Length }).LastOrDefault();

            for (int i = 0; i < maxValue.top * 2; i++)
            {
                DataGridViewTextBoxColumn column1P = new DataGridViewTextBoxColumn
                {
                    HeaderText = "",
                    Name = $"columnN{i}",
                    ReadOnly = true
                };
                if (((maxValue.top * 2) / 2) > i)
                {
                    column1P.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                }
                else
                {
                    column1P.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    column1P.DefaultCellStyle.Format = "###,##0.00";
                }
                GridDatos.Columns.Insert(i, column1P);
            }
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel|*.xlsx", FileName = $"REPORTE DE PERDIDAS Y GANANCIAS {GlobalConfig.Compañia.ToString()}" })
                {
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {


                        ReporteExcel.ReporteUtilidadPerdida(ConverRowsToExcel(), Encabezado(), sfd.FileName);
                        //ReportePerdidasGanancias.GenerarReporte(sfd.FileName, _lstExcel.ToList(), GlobalConfig.Compañia, GlobalConfig.IUser);
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public List<Object[]> ConverRowsToExcel()
        {

            var retorno = new List<object[]>();
            var dd = GridDatos.Rows;

            foreach (DataGridViewRow item in dd)
            {

                Object[] line = new object[item.Cells.Count];
                var cont = 0;

                foreach (DataGridViewCell cell in item.Cells)
                {
                    line[cont] = cell.Value ?? "";
                    cont++;
                }
                retorno.Add(line);

            }
            return retorno;
        }

        public String[] Encabezado()
        {

            return new string[]{

                $"{GlobalConfig.Compañia}",
                $"REPORTE DE PERDIDAS Y GANANCIAS AL MES {(((FechaTransaccion)EndPostingPeriod.SelectedItem).ToString().ToUpper())}",
                $"EMITIDO POR {GlobalConfig.IUser} ",
            };

        }

        private decimal TotalPerdida
        {
            get
            {
                var ingreso = _lstExcel.ToList().Find(x => x.Indicador == IndicadorCuenta.Cuenta_Titulo && x.TipoCuenta.TipoCuenta == NombreTipoCuenta.Ingreso);
                var Egreso = _lstExcel.ToList().Find(x => x.Indicador == IndicadorCuenta.Cuenta_Titulo && x.TipoCuenta.TipoCuenta == NombreTipoCuenta.Egreso);
                var costoVenta = _lstExcel.ToList().Find(x => x.Indicador == IndicadorCuenta.Cuenta_Titulo && x.TipoCuenta.TipoCuenta == NombreTipoCuenta.Costo_Venta);

                return ingreso.SaldoActualColones - costoVenta.SaldoActualColones - Egreso.SaldoActualColones;

            }
        }


    }
}
