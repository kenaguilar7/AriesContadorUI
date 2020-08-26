﻿using System;
using System.Diagnostics;
using System.Windows.Forms;
using CapaEntidad.Entidades.Ventanas;
using CapaEntidad.Textos;
using CapaPresentacion.FrameCompañias;
using CapaPresentacion.FrameCuentas;
using CapaPresentacion.Seguridad;
using CapaPresentacion.Reportes;
using CapaEntidad.Entidades.Compañias;
using CapaPresentacion.FrameIUsers;

namespace CapaPresentacion
{
    public partial class FrameMenu : Form
    {
        public Boolean comParametro { set { CargarCompañia(); } }
        public FrameMenu()
        {
            InitializeComponent();

            FrameLoginIUser n = new FrameLoginIUser();
            n.FormClosing += N_FormClosing;

            n.ShowDialog();
            void N_FormClosing(object sender, FormClosingEventArgs e)
            {
                if (GlobalConfig.IUser == null)
                {
                    Application.Exit();
                }
            }
            CargarDatos();

        }
        private void CargarDatos()
        {
            if (GlobalConfig.IUser != null)
            {
                this.txtIUser.Text = GlobalConfig.IUser.ToString();
                HideOptions();
                AddVersionNumber();
            }
        }
        private void AddVersionNumber()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            this.Text += $" v.{versionInfo.FileVersion}";
            ///fo
        }
        private void CargarCompañia()
        {
            this.txtCompaniaNombre.Text = GlobalConfig.Compañia.ToString();
        }
        private void MaestroDeCompañiasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrameMaestroCompañia n = new FrameMaestroCompañia();
            n.MdiParent = this;
            n.Show();
        }
        private void MaestroDeCuentasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GlobalConfig.Compañia != null)
            {
                FrameMaestroCuenta n = new FrameMaestroCuenta();
                n.MdiParent = this;
                n.Show();
            }
            else
            {
                MessageBox.Show("Seleccione una compañia", StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }
        private void AsientosContablesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (GlobalConfig.Compañia != null)
                {
                    if (!CheckForDuplicate(VentanaInfo.FormAsientos))
                    {
                        FrameAsientos n = new FrameAsientos();
                        n.MdiParent = this;
                        n.Show();
                    }
                }
                else
                {
                    MessageBox.Show("Seleccione una compañia", StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }
        private void MaestroDeUsaurio(object sender, EventArgs e)
        {
            FrameMaestroIUser n = new FrameMaestroIUser();
            n.MdiParent = this;
            n.Show();
        }
        private void SeleccioneCompañiaParaTrabajar(object sender, EventArgs e)
        {
            try
            {
                FrameSeleccionCompañia n = new FrameSeleccionCompañia(this);
                n.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void MaestroDeMeses(object sender, EventArgs e)
        {

            if (GlobalConfig.Compañia != null)
            {
                FrameAdministrarMeses n = new FrameAdministrarMeses();
                n.MdiParent = this;
                n.Show();
            }
            else
            {
                MessageBox.Show("Seleccione una compañia", StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        private void MaestroDeCompañia(object sender, EventArgs e)
        {
            try
            {
                FrameSeleccionCompañia n = new FrameSeleccionCompañia(this);
                n.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void balanceDeComprobaciónToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (GlobalConfig.Compañia != null)
                {
                    FrameReporteComprobacion n = new FrameReporteComprobacion();
                    n.MdiParent = this;
                    n.Show();

                }
                else
                {
                    MessageBox.Show("Seleccione una compañia", StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void balanceDeAuxiliaresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (GlobalConfig.Compañia != null)
                {
                    FrameReporteAuxiliares frame = new FrameReporteAuxiliares();
                    frame.MdiParent = this;
                    frame.Show();
                }
                else
                {
                    MessageBox.Show("Seleccione una compañia", StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void HideOptions()
        {
            ///Acultamos los modulos

            ///Modulo de conta
            ///
            //var mConta = ;

            if (GlobalConfig.IUser.TipoIUser == CapaEntidad.Enumeradores.TipoIUser.IUser)
            {



                if ((GlobalConfig.IUser.Modulos.Find(x => x.Codigo == 1) is var mConta) && mConta == null || !mConta.TienePermiso)
                {
                    contableToolStripMenuItem.Enabled = false;
                    contableToolStripMenuItem.Visible = false;
                }
                else
                {
                    ///Empezamos por ventanas
                    maestroDeCuentasToolStripMenuItem.Visible = (mConta.LstVentanas.Find(x => x.VentanaInfo == VentanaInfo.FormMaestroCuenta)).TienePermiso;
                    asientosContablesToolStripMenuItem.Visible = (mConta.LstVentanas.Find(x => x.VentanaInfo == VentanaInfo.FormAsientos)).TienePermiso;
                    administrarMesesToolStripMenuItem.Visible = (mConta.LstVentanas.Find(x => x.VentanaInfo == VentanaInfo.FormAdminMeses)).TienePermiso;
                }

                if ((GlobalConfig.IUser.Modulos.Find(x => x.Codigo == 2) is var MCompanias) && MCompanias == null || !MCompanias.TienePermiso)
                {
                    maestroDeCompañiasToolStripMenuItem.Enabled = false;
                    //maestroDeCompañiasToolStripMenuItem = false;
                }
                else
                {
                    maestroDeCompañiasToolStripMenuItem.Enabled = (MCompanias.LstVentanas.Find(x => x.VentanaInfo == VentanaInfo.FormMaestroCompanias)).TienePermiso;
                }

                if ((GlobalConfig.IUser.Modulos.Find(x => x.Codigo == 3) is var mSeguridad) && mSeguridad == null || !mSeguridad.TienePermiso)
                {
                    sistemaToolStripMenuItem.Enabled = false;
                    //maestroDeCompañiasToolStripMenuItem = false;
                }
                else
                {
                    PermisosDeIUserToolStripMenuItem.Enabled = (mSeguridad.LstVentanas.Find(x => x.VentanaInfo == VentanaInfo.FormPermisoIUser)).TienePermiso;

                }

                if ((GlobalConfig.IUser.Modulos.Find(x => x.Codigo == 4) is var mIUser) && mIUser == null || !mIUser.TienePermiso)
                {
                    IUsersToolStripMenuItem.Enabled = false;
                    //maestroDeCompañiasToolStripMenuItem = false;
                }
                else
                {
                    MaestroDeIUsertoolStripMenuItem.Enabled = (mIUser.LstVentanas.Find(x => x.VentanaInfo == VentanaInfo.FormMaestroIUser)).TienePermiso;

                }

            }

        }
        private void gestorDeVentanasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormPermisoIUser form = new FormPermisoIUser
            {
                MdiParent = this
            };
            form.Show();
        }
        private void movimientosDeCuentaToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (GlobalConfig.Compañia != null)
            {
                ReporteMovimientosCuenta form = new ReporteMovimientosCuenta
                {
                    MdiParent = this
                };
                form.Show();
            }
            else
            {
                MessageBox.Show("Seleccione una compañia", StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }
        private void perdiasYGananciasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GlobalConfig.Compañia != null)
            {
                ReportePedidasGanacias form = new ReportePedidasGanacias
                {
                    MdiParent = this
                };
                form.Show();
            }
            else
            {
                MessageBox.Show("Seleccione una compañia", StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        private void balanceDeSituaciónToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (GlobalConfig.Compañia != null)
            {
                ReporteBalanceSituacion form = new ReporteBalanceSituacion
                {
                    MdiParent = this
                };
                form.Show();
            }
            else
            {
                MessageBox.Show("Seleccione una compañia", StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        private bool CheckForDuplicate(VentanaInfo ventana)
        {
            bool bValue = false;
            foreach (Form fm in this.MdiChildren)
            {
                if (fm.Name == "FrameAsientos")
                {
                    fm.Activate();
                    fm.WindowState = FormWindowState.Normal;
                    bValue = true;
                }
            }
            return bValue;
        }
        private void cierreDePeriodoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (GlobalConfig.Compañia != null)
            //{
            //    FrameAsientoCierre form = new FrameAsientoCierre
            //    {
            //        MdiParent = this
            //    };
            //    form.Show();
            //}
            //else
            //{
            //    MessageBox.Show("Seleccione una compañia", StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //}
        }
        private void SalirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        public Boolean Bar
        {
            set
            {
                ProgressBar.Visible = value;
                ProgressBar.Value = 80;
                
            }
        }

        private void gestionDeCorreosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Correo n = new Correo();
            n.MdiParent = this;
            n.Show(); 
        }

        private void nbalanceDeComprobaciónToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReporteTest rtest = new ReporteTest();
            rtest.MdiParent = this;
            rtest.Show(); 
        }
    }
}
