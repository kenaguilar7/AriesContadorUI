using System.Collections.Generic;
using System.Windows.Forms;
using System.Data;
using System.Linq;
using CapaLogica;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using FastMember;
using AriesContador.Entities.Financial.Accounts;
using AriesContador.Entities.Financial.PostingPeriods;

namespace CapaPresentacion.Reportes
{
    public partial class ReporteBalanceSituacion : Form
    {
        private CuentaCL CuentaCL { get; } = new CuentaCL();
        private FechaTransaccionCL FechaTransaccionCL { get; } = new FechaTransaccionCL();
        private IEnumerable<AccountDTO> ListaCuentas { get; set; }
        private IEnumerable<AccountDTO> ListaCuentasBalancePerdida { get; set; }
        private IEnumerable<AccountDTO> ListaCuentasBalanceSitucion { get; set; }
        private PostingPeriodDTO _StartPostingPeriod { get => StartPostingPeriod.SelectedItem as PostingPeriodDTO; }
        private PostingPeriodDTO _EndPostingPeriod { get => EndPostingPeriod.SelectedItem as PostingPeriodDTO; }

        public ReporteBalanceSituacion()
        {
            InitializeComponent();
        }
        private async void ReporteBalanceSituacion_Load(object sender, EventArgs e)
        {
            await LoadAccounts();
            await LoadPostingPeriods();
            SetCuentasEnLista();
            CrearColumnasParaNombre();
        }
        private async Task LoadAccounts()
        {
            var lst = await CuentaCL.GetAllAsync(GlobalConfig.company.Code);
            ListaCuentas = from c in lst where c.AccountTag == AccountTag.Ingreso || c.AccountTag == AccountTag.Egreso || c.AccountTag == AccountTag.Costo_Venta select c;
        }

        private async Task LoadPostingPeriods()
        {
            var lstFechas = await FechaTransaccionCL.GetAllAsync(GlobalConfig.company.Code);
            FillDropDownLists(lstFechas);
        }

        private void FillDropDownLists(IEnumerable<PostingPeriodDTO> lstFechas)
        {
            EndPostingPeriod.DataSource = lstFechas;
            StartPostingPeriod.DataSource = CreateDefaulListForStartPostingPeriod(lstFechas);
        }

        private static List<PostingPeriodDTO> CreateDefaulListForStartPostingPeriod(IEnumerable<PostingPeriodDTO> lstFechas)
        {
            return new List<PostingPeriodDTO> { (from c1 in lstFechas select c1).OrderByDescending(x => x.Date).LastOrDefault() };
        }
        //private async void CargarDatos()
        //{
        //    ListaCuentas = await CuentaCL.GetAllAsync(GlobalConfig.Compañia.Codigo);
        //    var lstDts = await FechaTransaccionCL.GetAllAsync(GlobalConfig.Compañia.Codigo);

        //    EndPostingPeriod.DataSource = lstDts;


        //    var lstBfchFnl = new List<PostingPeriodDTO> { (from c1 in lstDts select c1).OrderByDescending(x => x.Fecha).LastOrDefault() };
        //    StartPostingPeriod.DataSource = lstBfchFnl;

        //    SetCuentasEnLista();

        //    CrearColumnasParaNombre();
        //}

        private void SetCuentasEnLista()
        {
            ListaCuentasBalancePerdida = from c in ListaCuentas where c.AccountTag == AccountTag.Ingreso || c.AccountTag == AccountTag.Egreso || c.AccountTag == AccountTag.Costo_Venta select c;
            ListaCuentasBalanceSitucion = from c in ListaCuentas where c.AccountTag != AccountTag.Ingreso && c.AccountTag != AccountTag.Egreso && c.AccountTag != AccountTag.Costo_Venta select c;

        }
        private async void BtnCalcular(object sender, EventArgs e)
        {
            var fch1 = (PostingPeriodDTO)StartPostingPeriod.SelectedItem;
            var fch2 = (PostingPeriodDTO)EndPostingPeriod.SelectedItem;


            var kst = await CuentaCL.GetAllAccountBalanceWithJournalEntriesRangeAsync(GlobalConfig.company.Code, fch1.Id, fch2.Id);

            ListaCuentas = kst.ToList();
            SetCuentasEnLista();
            CargarFormulario();

        }

        private void CargarFormulario()
        {
            GridDatos.Rows.Clear();

            foreach (var cuenta in ListaCuentasBalanceSitucion)
            {
                if (cuenta.AccountType == AccountType.Cuenta_Titulo)
                {

                    foreach (var cntAux in ListaCuentas)
                    {
                        if (cntAux.AccountType != AccountType.Cuenta_Titulo && cntAux.AccountTag == cuenta.AccountTag)
                        {
                            //DataGridViewRow row = new DataGridViewRow();
                            //row.CreateCells(GridDatos);
                            //var name = cntAux.GetNombreParaExcel(ListaCuentas.ToList());
                            //row.Cells[name.Length - 1].Value = name.Last();
                            //row.Cells[GridDatos.Columns.Count - (name.Length)].Value = cntAux.SaldoActualColones;
                            //GridDatos.Rows.Add(row);
                        }
                    }

                    if (cuenta.AccountTag == AccountTag.Patrimonio)
                    {

                        DataGridViewRow rowTotalPerdida = new DataGridViewRow();
                        rowTotalPerdida.CreateCells(GridDatos);

                        rowTotalPerdida.Cells[0].Value = $"UTILIDAD/PERDIDA PERIODO";
                        rowTotalPerdida.DefaultCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                        rowTotalPerdida.Cells[GridDatos.Columns.Count - 1].Value = TotalPerdida;

                        //GridDatos.Rows.Add(Perdida);
                        GridDatos.Rows.Add(rowTotalPerdida);

                        //patrimonio


                        //DataGridViewRow rowTotalpatrimonio = new DataGridViewRow();
                        //rowTotalpatrimonio.CreateCells(GridDatos);
                        //var nameTotal = cuenta.GetNombreParaExcel(ListaCuentas.ToList());
                        //rowTotalpatrimonio.Cells[nameTotal.Length - 1].Value = $"TOTAL {nameTotal.Last()}";
                        //rowTotalpatrimonio.DefaultCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                        //rowTotalpatrimonio.Cells[GridDatos.Columns.Count - (nameTotal.Length)].Value = TotalPerdida + cuenta.SaldoActualColones;

                        
                        //GridDatos.Rows.Add(rowTotalpatrimonio);


                    }
                    else
                    {

                        //DataGridViewRow rowTotal = new DataGridViewRow();
                        //rowTotal.CreateCells(GridDatos);
                        //var nameTotal = cuenta.GetNombreParaExcel(ListaCuentas.ToList());
                        //rowTotal.Cells[nameTotal.Length - 1].Value = $"TOTAL {nameTotal.Last()}";
                        //rowTotal.DefaultCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                        //rowTotal.Cells[GridDatos.Columns.Count - (nameTotal.Length)].Value = cuenta.SaldoActualColones;

                        
                        //GridDatos.Rows.Add(rowTotal);
                    }
                }
            }
            DataGridViewRow rowTotalPasivoPatrimonio = new DataGridViewRow();
            rowTotalPasivoPatrimonio.CreateCells(GridDatos);

            rowTotalPasivoPatrimonio.Cells[0].Value = $"TOTAL PASIVO Y PATRIMONIO";
            rowTotalPasivoPatrimonio.DefaultCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            rowTotalPasivoPatrimonio.Cells[GridDatos.Columns.Count - 1].Value = TotalPerdida + TotalSituacion;

            //GridDatos.Rows.Add(Perdida);
            GridDatos.Rows.Add(rowTotalPasivoPatrimonio);

        }
        private void CrearColumnasParaNombre()
        {

            //var maxValue = (from c in ListaCuentas orderby c.GetNombreParaExcel(ListaCuentasBalanceSitucion.ToList()).Length select new { top = c.GetNombreParaExcel(ListaCuentas.ToList()).Length }).LastOrDefault();
            var maxValue = new { top = 0}; 

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
            //dataGridViewCellStyle7.Format = "$### ###.00";
            //if (intrCont < cont)
            //{
            //    for (int i = 0; i < (cont - intrCont); i++)
            //    {
            //        GridDatos.Columns.Remove(GridDatos.Columns[i]);
            //    }

            //}
            //cont = intrCont;

        }

        private void tbnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel|*.xlsx", FileName = $"REPORTE BALANCE DE SITUACIÓN {GlobalConfig.company.ToString()}" })
                {
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        //ReporteExcel.ReporteUtilidadPerdida(ConverRowsToExcel(), Encabezado(), sfd.FileName);
                      
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private decimal TotalSituacion
        {
            get
            {
                ///Situcion
                //var activo = ListaCuentas.ToList().Find(x => x.AccountTag == AccountTag.Cuenta_Titulo && x.AccountTag == TipoCuenta.Activo);
                var pasivo = ListaCuentas.ToList().Find(x => x.AccountType == AccountType.Cuenta_Titulo && x.AccountTag == AccountTag.Pasivo);
                var patrimonio = ListaCuentas.ToList().Find(x => x.AccountType == AccountType.Cuenta_Titulo && x.AccountTag == AccountTag.Patrimonio);

                return Decimal.Add(pasivo.CurrentBalance, patrimonio.CurrentBalance);
            }
        }
        private decimal TotalPerdida
        {
            get
            {
                var ingreso = ListaCuentasBalancePerdida.ToList().Find(x => x.AccountType == AccountType.Cuenta_Titulo && x.AccountTag == AccountTag.Ingreso);
                var Egreso = ListaCuentasBalancePerdida.ToList().Find(x => x.AccountType == AccountType.Cuenta_Titulo && x.AccountTag == AccountTag.Egreso);
                var costoVenta = ListaCuentasBalancePerdida.ToList().Find(x => x.AccountType == AccountType.Cuenta_Titulo && x.AccountTag == AccountTag.Costo_Venta);

                return ingreso.CurrentBalance - costoVenta.CurrentBalance - Egreso.CurrentBalance;


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

                $"{GlobalConfig.company}",
                $"BALANCE DE SITUACION AL MES {(((PostingPeriodDTO)EndPostingPeriod.SelectedItem).ToString().ToUpper())}",
                $"EMITIDO POR {GlobalConfig.UserDTO} ",
            };

        }

        private void checkCuentasConSaldo_CheckedChanged(object sender, EventArgs e)
        {
            CargarFormulario();
        }


    }
}
