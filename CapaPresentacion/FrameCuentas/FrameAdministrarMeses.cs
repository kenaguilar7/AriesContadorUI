﻿using AriesContador.Entities.Financial.PostingPeriods;
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

namespace CapaPresentacion.FrameCuentas
{
    public partial class FrameAdministrarMeses : Form
    {

        private FechaTransaccionCL _fechaCL { get; } = new FechaTransaccionCL();
        public FrameAdministrarMeses()
        {
            InitializeComponent();
        }
        private async void FrameAdministrarMesesLoad(object sender, EventArgs e)
        {
            var companyId = GlobalConfig.company.Code;
            try
            {

                dtRegistros.DataSource = await _fechaCL.GetDataTableAsync(companyId);
                var lstMonths = (await _fechaCL.GetAllAsync(companyId)).TakeWhile(x => !x.Closed);
                var lstAvailableMonths = await _fechaCL.GetAvailableMonthsAsync(companyId);
                lstCerrarMes.DataSource = lstMonths.ToList();
                lstAbrirMes.DataSource = lstAvailableMonths.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private IPostingPeriod GetMonthForOpen() => (IPostingPeriod)lstAbrirMes.SelectedItem;
        private IPostingPeriod GetMonthForClose() => (IPostingPeriod)lstCerrarMes.SelectedItem;
        private async void SaveNewMonthBtnEvent(object sender, EventArgs e)
        {
            try
            {
                this.btnGuardar.Click -= new System.EventHandler(this.SaveNewMonthBtnEvent);
                var newEntity = await _fechaCL.InsertAsync(GetMonthForOpen(), GlobalConfig.company.Code);
                var mensaje = $"Perido contable abierto correctamente";
                MessageBox.Show(mensaje, StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Information);
                FrameAdministrarMesesLoad(sender, e);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnGuardar.Click += new System.EventHandler(this.SaveNewMonthBtnEvent);
            }
        }
        private async void CloseMonthBtnEvent(object sender, EventArgs e)
        {
            if (GetMonthForClose() != null)
            {
                try
                {
                    await _fechaCL.CloseMonthAsync(GlobalConfig.company.Code, GetMonthForClose());
                    var mensaje = "Mes cerrado correctamente";
                    MessageBox.Show(mensaje, StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    FrameAdministrarMesesLoad(sender, e);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + ex.StackTrace, StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }

        }
        private void BtnCerrarClick(object sender, EventArgs e)
        {
            this.Close();
        }


    }
}
