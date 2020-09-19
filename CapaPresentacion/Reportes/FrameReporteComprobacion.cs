using AriesContador.Entities.Financial.Accounts;
using AriesContador.Entities.Financial.PostingPeriods;
using AriesContador.Entities.Utils;
using CapaLogica;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CapaPresentacion.Reportes
{
    public partial class FrameReporteComprobacion : Form
    {
        private int cont = 0;
        private List<AccountDTO> _lstCuentas { get; set; } = new List<AccountDTO>();
        private List<AccountDTO> _lstExcel { get; set; } = new List<AccountDTO>(); 
        private FechaTransaccionCL _fechaTransaccionCL = new FechaTransaccionCL();
        private CuentaCL _cuentaCL = new CuentaCL();
        public FrameReporteComprobacion()
        {
            InitializeComponent();
            CargarDatos();
        }
        public FrameReporteComprobacion(IEnumerable<AccountDTO> cuentas, IPostingPeriod fechaTransaccion)
        {
            InitializeComponent();
            _lstCuentas = cuentas.ToList();
            lstMesesAbiertos.SelectedIndexChanged -= LstMesesAbiertos_SelectedIndexChanged;
            var lst = new List<IPostingPeriod>() { fechaTransaccion }; 
            lstMesesAbiertos.DataSource = lst;
            CargarDatosFormulario(_lstCuentas);
            this.Text += $"- Cierre de periodo al mes { fechaTransaccion.ToString() }";
            lstMesesAbiertos.Enabled = false; 
        }
        /// <summary>
        /// Carga los datos, llena la lista con las cuentas y trae todos los meses
        /// </summary>
        private async void CargarDatos()
        {
            _lstCuentas = await _cuentaCL.GetAllAsync(GlobalConfig.company.Code);
            this.lstMesesAbiertos.DataSource = await _fechaTransaccionCL.GetAllAsync(GlobalConfig.company.Code);
        }
        /// <summary>
        /// Actualiza la vista del grid
        /// </summary>
        private void UpdateView()
        {
            if (lstMesesAbiertos.Items.Count != 0)
            {
                var nuevalst = _lstCuentas;

                //_cuentaCL.LLenarConSaldos(((FechaTransaccion)lstMesesAbiertos.Items[lstMesesAbiertos.Items.Count - 1]).Fecha, ((FechaTransaccion)lstMesesAbiertos.SelectedItem).Fecha,  nuevalst, GlobalConfig.Compañia);

                ///Imprimir solo cuentas con saldo 
                ///quitar las cuentas que no tienen saldo
                if (chekImprimirSaldosCero.Checked)
                {
                    nuevalst = _cuentaCL.QuitarCuentasSinSaldos(_lstCuentas);
                }
                CargarDatosFormulario(nuevalst);
            }
        }
        /// <summary>
        /// evento que ocurre cuando el indice de los meses cambia de posicion, 
        /// actualiza la vista
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LstMesesAbiertos_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateView();
        }
        /// <summary>
        /// metodo que se encarga de cargar los datos al formulario
        /// </summary>
        /// <param name="_lstCuentas"></param>
        /// <returns></returns>
        private List<AccountDTO> CargarDatosFormulario(List<AccountDTO> _lstCuentas)
        {
            _lstExcel = _lstCuentas; 
            GridDatos.Rows.Clear();

            decimal saldoAnteriordedito = 0m;
            decimal saldoAnteriorCreditos = 0m;

            decimal saldoActualCuentaDebitos = 0m;
            decimal saldoActualCuentaCreditos = 0m;

            decimal saldoActualDebitos = 0m;
            decimal saldoActualCredito = 0m;
            CrearColumnasParaNombre();
            foreach (var cuenta in _lstCuentas)
            {

                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(GridDatos);

                var name = GetNombreParaExcel(_lstCuentas, cuenta);

                row.Cells[name.Length - 1].Value = name.Last();

                row.Cells[cont].Value = (cuenta.DebOCred == DebOCred.Debito) ? MetodoSumatorio(cuenta.PriorBalance, 1) : 0;
                row.Cells[cont + 1].Value = (cuenta.DebOCred == DebOCred.Credito) ? MetodoSumatorio(cuenta.PriorBalance, 2) : 0;

                row.Cells[cont + 2].Value = (cuenta.DebOCred == DebOCred.Debito) ? MetodoSumatorio(cuenta.MontlyBalance, 3) : 0;
                row.Cells[cont + 3].Value = (cuenta.DebOCred == DebOCred.Credito) ? MetodoSumatorio(cuenta.MontlyBalance, 4) : 0;

                row.Cells[cont + 4].Value = (cuenta.DebOCred == DebOCred.Debito) ? MetodoSumatorio(cuenta.CurrentBalance, 5) : 0;
                row.Cells[cont + 5].Value = (cuenta.DebOCred == DebOCred.Credito) ? MetodoSumatorio(cuenta.CurrentBalance, 6) : 0;

                GridDatos.Rows.Add(row);

                decimal MetodoSumatorio(decimal monto, int Posicion)
                {

                    if (cuenta.AccountType == AccountType.Cuenta_Auxiliar)
                    {
                        switch (Posicion)
                        {
                            case 1: saldoAnteriordedito += monto; return monto;
                            case 2: saldoAnteriorCreditos += monto; return monto;
                            case 3: saldoActualCuentaDebitos += monto; return monto;
                            case 4: saldoActualCuentaCreditos += monto; return monto;
                            case 5: saldoActualDebitos += monto; return monto;
                            case 6: saldoActualCredito += monto; return monto;
                            default: return monto;

                        }
                    }
                    else
                    {
                        return monto;
                    }

                }
            }


            DataGridViewRow rowFinal = new DataGridViewRow();
            rowFinal.DefaultCellStyle.BackColor = Color.Green;
            rowFinal.CreateCells(GridDatos);

            rowFinal.Cells[0].Value = "Total";
            rowFinal.Cells[cont ].Value = saldoAnteriordedito;
            rowFinal.Cells[cont + 1].Value = saldoAnteriorCreditos;
            rowFinal.Cells[cont + 2].Value = saldoActualCuentaDebitos;
            rowFinal.Cells[cont + 3].Value = saldoActualCuentaCreditos;
            rowFinal.Cells[cont + 4].Value = saldoActualDebitos;
            rowFinal.Cells[cont + 5].Value = saldoActualCredito;

            GridDatos.Rows.Add(rowFinal);




            return _lstCuentas;
        }
        /// <summary>
        /// Evento que ocurre cuando el checkbox de saldos cero cambia de estado
        /// se actualiza la vista
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChekImprimirSaldosCero_CheckedChanged(object sender, EventArgs e)
        {
            UpdateView();
        }
        /// <summary>
        /// Evento que ocurre cuando se presiona el boton de exportar a excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnExcel_Click(object sender, EventArgs e)
        {
            try
            {

                if (_lstCuentas != null)
                {
                    using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel|*.xlsx", FileName = $"REPORTE DE COMPROBACIÓN {GlobalConfig.company.ToString()}" })
                    {
                        if (sfd.ShowDialog() == DialogResult.OK)
                        {
                            //ReporteBalanceComprobacion.GenerarReporte(_lstExcel, GlobalConfig.Compañia, GlobalConfig.UserDTO, GlobalConfig.Compañia.TipoMoneda, sfd.FileName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, StaticInfoString.MensajeBannerError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// Cierra la ventana
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CerrarVentana(object sender, EventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// Se va a implementar como un metodo glbal***
        /// añade las columnas con los nombres de las cuentas
        /// </summary>
        private void CrearColumnasParaNombre()
        {

            ///Sacamos el numero maximo de la lista y creamos esa cantidad de columnas - 1 
            ///-1, porque ya tenemos una

            ///Esta varibable se va a utilizar para al final del ciclo saber si se deben eliminar columnas o no
            var intrCont = 1;

            _lstExcel.ForEach((_account) =>
            {

                var cnt = GetNombreParaExcel(_lstExcel, _account).Length;

                intrCont = (cnt > intrCont) ? cnt : intrCont;

                if (cnt > cont)
                {
                    DataGridViewTextBoxColumn column1P = new DataGridViewTextBoxColumn
                    {
                        HeaderText = "",
                        Name = $"columnN{cont}",
                        ReadOnly = true
                    };
                    GridDatos.Columns.Insert(cont, column1P);

                    cont = cnt;
                }

            });

            if (intrCont < cont)
            {
                for (int i = 0; i < (cont - intrCont); i++)
                {
                    GridDatos.Columns.Remove(GridDatos.Columns[i]);
                }

            }
            cont = intrCont;

        }

        public String[] GetNombreParaExcel(List<AccountDTO> list, AccountDTO account)
        {

            ///BUscar cuantos padres tiene 
            var dummy = account;
            var listaRetorno = new List<AccountDTO>();

            while ((dummy = BuscarCuentaPadre(dummy)) != null)
            {

                listaRetorno.Add(dummy);
            }

            if (listaRetorno.Count == 0)
            {
                var retorno = new String[1];
                retorno[0] = this.Name;
                for (int i = 0; i < retorno.Length; i++)
                {
                    if (retorno[i] == null)
                    {
                        retorno[i] = "";
                    }
                }
                return retorno;
            }
            else
            {
                var retorno = new String[listaRetorno.Count + 1];

                retorno[retorno.Length - 1] = this.Name;
                for (int i = 0; i < retorno.Length; i++)
                {
                    if (retorno[i] == null)
                    {
                        retorno[i] = "";
                    }
                }
                return retorno;
            }

            AccountDTO BuscarCuentaPadre(AccountDTO cuentaHija)
            {
                if (list.Count != 0)
                {
                    foreach (AccountDTO item in list)
                    {
                        if (item.Id == cuentaHija.FatherAccount.Id)
                        {
                            return item;
                        }

                    }
                    return null;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
