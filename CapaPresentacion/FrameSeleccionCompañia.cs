using AriesContador.Entities.Administration.Companies;
using CapaLogica;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapaPresentacion
{
    public partial class FrameSeleccionCompañia : Form
    {

        private readonly CompañiaCL _comñiaCL = new CompañiaCL();
        FrameMenu fm = null;
        public FrameSeleccionCompañia(FrameMenu fm)
        {
            this.fm = fm as FrameMenu;
            InitializeComponent();
            CargarCompañiasAsync();
        }

        private async Task CargarCompañiasAsync()
        {
            try
            {
                var lst = await _comñiaCL.GetAllAsync();
                var _lstCompanies = lst; 
                lstCompanias.DataSource = _lstCompanies;
                lstCompanias.SelectedIndex = -1;
                this.lstCompanias.SelectedIndexChanged += new System.EventHandler(this.lstCompanias_SelectedIndexChanged);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            try
            {
                var c = (CompanyDTO)btnAceptar.Tag;
                if (c != null)
                {

                    ///TODO si hay ventanas abiertas deberian de cerrarse

                    GlobalConfig.company = c;
                    fm.comParametro = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("DEBE DE SELECIONAR UNA COMPAÑIA", "", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        private void CargarCompaniaFormulario(CompanyDTO compañia)
        {
            // lstCompanias.SelectedIndex = -1;
            txtCompaniaBuscada.Text = compañia.ToString();
            txtCompaniaBuscada.Visible = true;
            txtIdentificacion.Text = compañia.IdNumber;
            txtIdentificacion.Visible = true;
            btnAceptar.Tag = compañia;

        }

        private void TxtBoxBuscar_Leave(object sender, EventArgs e)
        {
            try
            {
                /// Editamos el codigo de la compañia
                // /

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

        private void lstCompanias_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.txtCompaniaBuscada.Visible = false;
            var c = (CompanyDTO)lstCompanias.SelectedItem;
            CargarCompaniaFormulario(c);
        }

        private void BtnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtBoxBuscar_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((Keys)e.KeyChar == Keys.Enter)
            {
                TxtBoxBuscar_Leave(null, null);
            }
        }

        private void FrameSeleccionCompañia_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((Keys)e.KeyChar == Keys.Enter)
            {
                btnAceptar_Click(null, null);
            }
        }
    }
}
