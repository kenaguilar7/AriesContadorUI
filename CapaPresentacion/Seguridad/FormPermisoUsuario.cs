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
using AriesContador.Entities.Administration.Users;
using AriesContador.Entities.Administration.Companies;
using AriesContador.Entities.Seguridad;

namespace CapaPresentacion.Seguridad
{
    public partial class FormPermisoIUser : Form
    {

        UserCL IUserCL = new UserCL();
        CompañiaCL compañiaCL = new CompañiaCL();
        PermisoCL permisoCL = new PermisoCL();
        private List<UserDTO> TodosLosIUsers = new List<UserDTO>();
        private List<CompanyDTO> CompañiasDelIUser = new List<CompanyDTO>();
        private List<CompanyDTO> TodasLasCompañias = new List<CompanyDTO>();
        private List<Modulo> modulos = new List<Modulo>();

        public FormPermisoIUser()
        {
            InitializeComponent();
            CargarDatos();
        }

        private async Task CargarDatos()
        {
            ///Cargamos los IUsers,
            //TodosLosIUsers = await  IUserCL.GetAllAsync();
            var lst = await IUserCL.GetAllAsync();
            lstIUsers.DataSource = TodosLosIUsers;
            lstIUsers.SelectedIndex = -1;
        }
        /// <summary>
        /// Agrega las compañias seleccionadas en la lista (compañias sin asignar)
        /// a la lista (compañias asignadas)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AgregarCompania_Click(object sender, EventArgs e)
        {
            var lst = (listCompañiasSinAsignar.SelectedItems.Cast<CompanyDTO>()).ToList();
            lst.ForEach((compañia) =>
            {
                listCompañiasAsignadas.Items.Add(compañia);
                listCompañiasSinAsignar.Items.Remove(compañia);
            });
        }
        /// <summary>
        /// Agrega las compañias seleccionadas en la lista (Compañias asignadas)
        /// a la lista (compañias por asignar)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoverCompañia_Click(object sender, EventArgs e)
        {
            var lst = (listCompañiasAsignadas.SelectedItems.Cast<CompanyDTO>()).ToList();
            lst.ForEach((compañia) =>
            {
                listCompañiasSinAsignar.Items.Add(compañia);
                listCompañiasAsignadas.Items.Remove(compañia);
            });
        }
        /// <summary>
        /// Cargar los datos del UserDTO seleccionado
        /// </summary>
        /// <param name="UserDTO"></param>
        private async Task CargarIUserAsync(UserDTO UserDTO)
        {

            ///Traigo todas las compañias que el UserDTO tenga asignado
            listCompañiasAsignadas.Items.Clear();
            listCompañiasSinAsignar.Items.Clear();

            TodasLasCompañias = await compañiaCL.GetAllAsync();
            CompañiasDelIUser = await compañiaCL.GetAllAsync();

            ///Buscamos todas las compañias
            TodasLasCompañias.ForEach((CompanyDTO) =>
            {
                if (CompañiasDelIUser.Find(x => x.Code == CompanyDTO.Code) == null)
                {
                    ///si la busqueda fue nula
                    ///quiere decir que el UserDTO no tiene asginada la compañia
                    listCompañiasSinAsignar.Items.Add(CompanyDTO);
                }
                else
                {
                    listCompañiasAsignadas.Items.Add(CompanyDTO);
                }

            });
            CargarModulos();
        }
        /// <summary>
        /// Evento que ocurre cuando la lista que contiene los IUsers cambia de indice
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LstIUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                var user = (UserDTO)lstIUsers.SelectedItem;
                if (user != null)
                {
                    CargarIUserAsync(user);
                }
                else
                {
                    MessageBox.Show("No se encontro ningun usurario", StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        /// <summary>
        /// Carga los modulos disponible
        /// y selecciona los asignados al UserDTO
        /// y los que no tiene asignados
        /// </summary>
        private void CargarModulos()
        {
            treeViewModulos.Nodes.Clear();
            var user = (UserDTO)lstIUsers.SelectedItem;
            ///El UserDTO admin puede tener acceso a todas las compañias??? si es asi entonces 
            ///no ponerlos en la lista
            panelAsignacionModulos.Enabled = (user.UserType == UserType.Administrador) ? false : true;


            throw new NotImplementedException(); 
            //modulos = permisoCL.GetAllModules(user);

            foreach (var item in modulos)
            {
                var x = new TreeNode(item.ToString())
                {
                    Tag = item,
                    Checked = item.TienePermiso
                };

                CargarVentanasAlNodo(item, ref x);

                treeViewModulos.Nodes.Add(x);
            }

        }
        /// <summary>
        /// Carga las lista de ventanas al su respectivo modulo asignado al nodo
        /// </summary>
        /// <param name="modulo"></param>
        /// <param name="treeNode"></param>
        private void CargarVentanasAlNodo(Modulo modulo, ref TreeNode treeNode)
        {
            foreach (var item in modulo.Ventanas)
            {
                var x = new TreeNode(item.NombreExterno)
                {
                    Tag = item,
                    Checked = item.TienePermiso
                };
                x.Checked = item.TienePermiso;
                x.Nodes.Add(new TreeNode(item.CRUDInsert.Nombre.ToString()) { Tag = item.CRUDInsert, Checked = item.CRUDInsert.TienePermiso });
                x.Nodes.Add(new TreeNode(item.CRUDUpdate.Nombre.ToString()) { Tag = item.CRUDUpdate, Checked = item.CRUDUpdate.TienePermiso });
                x.Nodes.Add(new TreeNode(item.CRUDLIst.Nombre.ToString()) { Tag = item.CRUDLIst, Checked = item.CRUDLIst.TienePermiso });
                x.Nodes.Add(new TreeNode(item.CRUDDeleted.Nombre.ToString()) { Tag = item.CRUDDeleted, Checked = item.CRUDDeleted.TienePermiso });

                treeNode.Nodes.Add(x);
                ///
            }
        }
        /// <summary>
        /// Guarda los datos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            ///Primero guarda las compañias 

            var nuevas = (listCompañiasAsignadas.Items.Cast<CompanyDTO>()).ToList();
            var remover = (listCompañiasSinAsignar.Items.Cast<CompanyDTO>()).ToList();

            var user = (UserDTO)lstIUsers.SelectedItem;
            if (user != null)
            {
                throw new NotImplementedException(); 
                //permisoCL.InsertCompany(nuevas, user, GlobalConfig.UserDTO);
                //permisoCL.RemoveCompany(remover, user, GlobalConfig.UserDTO);
                //permisoCL.UpdatePermisos(modulos, user, GlobalConfig.UserDTO);
                var ss = modulos;
                MessageBox.Show("UserDTO actulizado correctamente", StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Seleccione un UserDTO", StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }
        /// <summary>
        /// Evento que ocurre cuando se checa un modulo 
        /// se hace un casteo del modulo checado y se le asigna el estado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewModulos_AfterCheck(object sender, TreeViewEventArgs e)
        {
            //((Permiso)e.Node.Tag).TienePermiso = e.Node.Checked;
        }
        /// <summary>
        /// Se sale de la venatana
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BbnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// Evento que ocurre cuando se presiona un tecla en el cuadro de texto 
        /// de buscar IUsers 
        /// es basicamente por si el UserDTO presiona enter poder hacer el tap
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Buscar_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((Keys)e.KeyChar == Keys.Enter)
            {
                BoxBuscar_Leave(null, null);
            }
        }
        /// <summary>
        /// Evento que ocurre cuando el control que busca los IUsers por el codigo deja el foco
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BoxBuscar_Leave(object sender, EventArgs e)
        {
            if (this.Visible && txtBoxBuscar.Text.Length > 0)
            {
                var tyxt = txtBoxBuscar.Text;

                var r = TodosLosIUsers.Find(x => x.UserName.Equals(txtBoxBuscar.Text, StringComparison.OrdinalIgnoreCase));

                if (r != null)
                {
                    lstIUsers.SelectedItem = r;
                    //CargarIUser(r); 
                }
                else
                {
                    MessageBox.Show($"No se encontro ningun UserDTO con el UserDTO {tyxt}", StaticInfoString.NombreApp, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }
        /// <summary>
        /// Evento que ocurre cuando el la lista de IUsers deja el foco
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IUsers_Leave(object sender, EventArgs e)
        {
            LstIUsers_SelectedIndexChanged(null, null);
        }
    }
}
