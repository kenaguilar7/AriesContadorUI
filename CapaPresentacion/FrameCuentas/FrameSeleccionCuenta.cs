using AriesContador.Entities.Financial.Accounts;
using CapaLogica;
using CapaPresentacion.cods;
using CapaPresentacion.utils;
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
    public partial class FrameSeleccionCuenta : Form, ICallingForm
    {
        private CuentaCL _cuentaCL { get; } = new CuentaCL(); 
        private List<AccountDTO> LstCuentas { get; set; }
        private ICallingForm _getCuenta;
        public FrameSeleccionCuenta(ICallingForm callingForm)
        {
            _getCuenta = callingForm as ICallingForm;
            InitializeComponent();

        }
        private async void FrameSeleccionCuenta_Load(object sender, EventArgs e)
        {
            LstCuentas = await _cuentaCL.GetAllAsync(GlobalConfig.company.Code); 
            treeCuentas.Nodes.AddRange(TreeViewCuentas.CrearTreeView(LstCuentas));

        }
        private void SeleccionaCuentaEnTreeView(object sender, EventArgs e)
        {
            DevolverCuenta();
        }
        private void DevolverCuenta()
        {
            try
            {
                AccountDTO sele = (AccountDTO)treeCuentas.SelectedNode.Tag;
                sele.PathDirection = treeCuentas.SelectedNode.FullPath;

                if (_getCuenta.TransferirCuenta(sele))
                {
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Cuenta no valida", StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void CerrarClick(object sender, EventArgs e)
        {
            this.Close();
        }
        private void SeleccionarClick(object sender, EventArgs e)
        {
            if (treeCuentas.SelectedNode != null)
            {
                DevolverCuenta();
            }
            else
            {
                MessageBox.Show("Seleccione una cuenta", StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        private void ExpandirArbol(object sender, EventArgs e)
        {
            treeCuentas.ExpandAll();
        }
        private void ContraerArbol(object sender, EventArgs e)
        {
            treeCuentas.CollapseAll();
        }
        private void CrearNuevaCuenta(object sender, EventArgs e)
        {
            try
            {
                if (treeCuentas.SelectedNode is null)
                {
                    MessageBox.Show("Seleccione una cuenta ", StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;

                }
                else if (!(treeCuentas.SelectedNode.Tag is AccountDTO cuenta) || cuenta.AccountType == AccountType.Cuenta_Titulo)
                {
                    MessageBox.Show("No se puede crear cuentas en este nivel", StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                else
                {
                    FrameNuevaCuenta nv = new FrameNuevaCuenta(this, cuenta);
                    
                    nv.ShowDialog();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void BucarNodes(object sender, KeyEventArgs e)
        {
            var t = txtNombreCuenta.Text;
            var nod = TreeViewCuentas.BuscarNodo(txtNombreCuenta.Text, this.treeCuentas);

            if (nod != null && nod.Count > 0)
            {
                treeCuentas.SelectedNode = nod[0];
                treeCuentas.SelectedNode.BackColor = Color.CornflowerBlue;
            }
            else
            {
                treeCuentas.SelectedNode = null;
            }
        }
        private void TxtNombreCuentaKeyPress(object sender, KeyPressEventArgs e)
        {
            if ((Keys)e.KeyChar == Keys.Enter)
            {
                e.Handled = true;
                DevolverCuenta();
            }
        }

        public bool TransferirCuenta(AccountDTO cuenta)
        {
            if (cuenta != null)
            {
                TreeViewCuentas.CargarCuentaAlTreeView(cuenta, ref treeCuentas, LstCuentas);
                return true;
            }
            else { return false; }
        }


    }
}
