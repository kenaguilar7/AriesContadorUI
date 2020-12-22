using AriesContador.Entities.Financial.Accounts;
using AriesContador.Entities.Financial.PostingPeriods;
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
    public partial class FrameReporteAuxiliares : Form
    {

        FechaTransaccionCL fechaTransaccionCL = new FechaTransaccionCL();
        CuentaCL cuentaCL = new CuentaCL();
        private List<PostingPeriodDTO> fechaTransaccions = new List<PostingPeriodDTO>();

        public FrameReporteAuxiliares()
        {
            InitializeComponent();
            CargarDatos();
        }
        private async void CargarDatos()
        {

            var lstMeses = await fechaTransaccionCL.GetAllAsync(GlobalConfig.company.Id);
            fechaTransaccions = lstMeses.ToList();
            this.lstMesInicio.DataSource = lstMeses;
        }

        private void lstMesInicio_SelectedIndexChanged(object sender, EventArgs e)
        {
            var meses = (from n in fechaTransaccions where n.Date >= ((PostingPeriodDTO)lstMesInicio.SelectedItem).Date select n).ToList<PostingPeriodDTO>();

            lstMesFinal.DataSource = meses;
        }

        private async void btnGenerarExcel_Click(object sender, EventArgs e)
        {
            try
            {


                var lstCuentas = new Dictionary<PostingPeriodDTO, List<AccountDTO>>();
                var cuentas = await  cuentaCL.GetAllAsync(GlobalConfig.company.Id);

                //cuentaCL.LLenarConSaldoB(((FechaTransaccion)lstMesInicio.SelectedItem).Fecha, ((FechaTransaccion)lstMesInicio.SelectedItem).Fecha, cuentas, GlobalConfig.Compañia); 
                foreach (var item in fechaTransaccions)
                {
                    if (item.Date >= ((PostingPeriodDTO)lstMesInicio.SelectedItem).Date && item.Date <= ((PostingPeriodDTO)lstMesFinal.SelectedItem).Date)
                    {
                        var cuentasClonadas = new List<AccountDTO>(cuentas.Count());

                        //cuentas.ForEach((Cuenta) =>
                        //{
                        //    cuentasClonadas.Add(Cuenta.DeepCopy());
                        //});

                        //new CuentaCL().LLenarConSaldos(item.Date, item.Date, cuentasClonadas, GlobalConfig.company);

                        cuentasClonadas = cuentaCL.QuitarCuentasSinSaldos(cuentasClonadas);

                        lstCuentas.Add(item, cuentasClonadas);
                    }
                }

                using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel|*.xlsx", Title = "Reporte auxiliares", FileName = $"REPORTE DE AUXILIARES {GlobalConfig.company.ToString()} - {GlobalConfig.company.IdNumber}" })
                {
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        //ReporteAuxiliares.GenerarReporte(lstCuentas, GlobalConfig.company, GlobalConfig.UserDTO, GlobalConfig.company.TipoMoneda, sfd.FileName);
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); 
            }

        }

        private void BtnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
