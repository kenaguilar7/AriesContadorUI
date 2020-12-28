using AriesContador.Entities.Financial.Accounts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapaPresentacion.cods
{
    public class TreeViewCuentas
    {

        public static TreeNode[] CrearTreeView(List<AccountDTO> lstCuentas)
        {

            List<TreeNode> retorno = new List<TreeNode>();
            
            foreach (AccountDTO item in lstCuentas)
            {
                if (item.AccountType == AccountType.Cuenta_Titulo)
                {
                    TreeNode node = new TreeNode(item.Name)
                    {
                        Tag = item
                    };
                    retorno.Add(CrearCuenta(item, lstCuentas));
                    //retorno.Add(node);
                }
            }

            return retorno.ToArray();
        }
        private static TreeNode CrearCuenta(AccountDTO cuenta, List<AccountDTO> lst)
        {


            TreeNode retorno = new TreeNode(cuenta.Name)
            {
                Tag = cuenta
            };

            var sql = from c in lst where c.FatherAccount == cuenta.Id select c;

            var cueHijas = sql.ToArray<AccountDTO>();

            foreach (AccountDTO item in cueHijas)
            {

                var l3 = (from c in lst where c.FatherAccount == cuenta.Id select c).ToArray<AccountDTO>();

                if (l3.Length != 0)
                {

                    TreeNode b = CrearCuenta(item, lst);
                    retorno.Nodes.Add(b);

                }

            }

            return retorno;
        }
        public static List<TreeNode> BuscarNodo(string param, TreeView treeCuentas)
        {

            List<TreeNode> list = new List<TreeNode>();

            foreach (TreeNode item in treeCuentas.Nodes)
            {
                item.BackColor = new Color();
                
                if (item.Text.Contains(param))

                    list.Add(item);

                if (item.Nodes.Count > 0)

                    list.AddRange(FindChild(item.Nodes, param));

            }

            return list;
        }
        private static List<TreeNode> FindChild(TreeNodeCollection nodes, string param)
        {
            List<TreeNode> list = new List<TreeNode>();

            foreach (TreeNode item in nodes)
            {
                item.BackColor = new Color();
                if (item.Text.Contains(param))
                    list.Add(item);

                if (item.Nodes.Count > 0)
                    list.AddRange(FindChild(item.Nodes, param));
            }

            return list;
        }
        public static void CargarCuentaAlTreeView(AccountDTO cuenta, ref TreeView treeCuentas, List<AccountDTO> LstCuentas)
        {

            TreeNode node = new TreeNode(cuenta.Name)
            {
                Tag = cuenta
            };
            treeCuentas.SelectedNode.Nodes.Add(node);
            LstCuentas.Add(cuenta);
            BuscarCuentaPadre(cuenta);

            void BuscarCuentaPadre(AccountDTO cuentaHija)
            {
                if (LstCuentas.Count != 0)
                {
                    foreach (AccountDTO item in LstCuentas)
                    {
                        if (item.Id == cuentaHija.FatherAccount)
                        {
                            //item.SaldoAnteriorColones += cuenta.SaldoAnteriorColones;
                            if (item.AccountType != AccountType.Cuenta_Titulo)
                            {
                                item.AccountType = AccountType.Cuenta_De_Mayor;
                            }
                            BuscarCuentaPadre(item);
                            return;
                        }

                    }
                }
            }
        }
        public static List<AccountDTO> GetCuentasHijas(AccountDTO cuentaPadre, List<AccountDTO> cuentas)
        {

            var retorno = new List<AccountDTO>();
            retorno.Add(cuentaPadre); 
            foreach (var item in cuentas)
            {
                if (item.FatherAccount == cuentaPadre.Id)
                {
                    retorno.Add(item);

                    var hijas = from cc in cuentas where item.Id == cc.FatherAccount select cc;

                    foreach (var hitem in hijas)
                    {
                        retorno.AddRange(GetCuentasHijas(hitem, cuentas));
                    }
                }


            }

            return retorno;
        }
    }
}
