#region using
using System;
using System.Data;
using System.Collections.Generic;

using System.Data.OleDb;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using System.Xml;

#endregion

namespace Goldtect.ASTreeViewDemo
{
    public partial class DnDSaveDB : PageBase
    {
        protected void Page_Load( object sender, EventArgs e )
        {
            if (Request.QueryString["ID"] != null)
                lblRoot.Text = Request.QueryString["ID"];
            if (!IsPostBack)
            {
                Page.Header.DataBind();    
                BindData();
            }
        }

        private void BindData()
        {
            GenerateTree1();
            //GenerateTree2();

            ManageNodeTreeName();
        }

        private void GenerateTree1()
        {
            String qry="select ProductID,SUBSTRING([ProductName], 1, CASE CHARINDEX(CHAR(10), [ProductName]) WHEN 0 THEN LEN([ProductName]) ELSE CHARINDEX(char(10), [ProductName]) - 1 END) as ProductName,ParentID from [ProductsTree] where ProductID>0";

            DataSet ds = OleDbHelper.ExecuteDataset(base.NorthWindConnectionString, CommandType.Text, qry);

            ASTreeViewDataTableColumnDescriptor descripter = new ASTreeViewDataTableColumnDescriptor("ProductName"
                , "ProductID"
                , "ParentID");

            this.astvMyTree1.DataSourceDescriptor = descripter;
            this.astvMyTree1.DataSource = ds.Tables[0];

            this.astvMyTree1.DataBind();
             if (!String.IsNullOrEmpty(tbItem.Text))
             {
                 this.astvMyTree1.SelectNode(lblRoot.Text);
             }
            

            this.astvMyTree2.DataSourceDescriptor = descripter;
            this.astvMyTree2.DataSource = ds.Tables[0];

            this.astvMyTree2.DataBind();
            if (!String.IsNullOrEmpty(tbItem2.Text))
            {
                this.astvMyTree2.SelectNode(lblRoot2.Text);
            }
        }

        private void ManageNodeTreeName()
        {
            ASTreeView.ASTreeNodeHandlerDelegate nodeDelegate1 = delegate(ASTreeViewNode node)
            {
                node.AdditionalAttributes.Add(new KeyValuePair<string, string>("treeName", "astvMyTree1"));
            };

            astvMyTree1.TraverseTreeNode(this.astvMyTree1.RootNode, nodeDelegate1);

            ASTreeView.ASTreeNodeHandlerDelegate nodeDelegate2 = delegate(ASTreeViewNode node)
            {
                node.AdditionalAttributes.Add(new KeyValuePair<string, string>("treeName", "astvMyTree2"));
            };

            astvMyTree2.TraverseTreeNode(this.astvMyTree2.RootNode, nodeDelegate2);
        }

        protected void btnSaveDragDrop_Click(object sender, EventArgs e)
        {
            string nodeValue = this.txtNodeValue.Text;
            string nodeTreeName = this.txtNodeTreeName.Text;
            string parentValue = this.txtParentValue.Text;
            string parentTreeName = this.txtParentTreeName.Text;

           ChangeParent("ProductsTree", int.Parse(nodeValue), int.Parse(parentValue));
            BindData();
        }

        

        protected void astvMyTree_OnSelectedNodeChanged(object src, ASTreeViewNodeSelectedEventArgs e)
        {
            //tb1.Text = e.NodeText;
            ASTreeViewNode selectedNode = astvMyTree1.GetSelectedNode();
            if (selectedNode != null)
            {
                lblRoot.Text = selectedNode.NodeValue;

                tbItem.Text = (string)OleDbHelper.ExecuteScalar(base.NorthWindConnectionString, CommandType.Text, "select ProductName from ProductsTree where ProductID=" + lblRoot.Text);
            }
        }
        
        protected void astvMyTree2_OnSelectedNodeChanged(object src, ASTreeViewNodeSelectedEventArgs e)
        {
            //tb1.Text = e.NodeText;
            ASTreeViewNode selectedNode = astvMyTree2.GetSelectedNode();
            if (selectedNode != null)
            {
                lblRoot2.Text = selectedNode.NodeValue;
                tbItem2.Text = (string)OleDbHelper.ExecuteScalar(base.NorthWindConnectionString, CommandType.Text, "select ProductName from ProductsTree where ProductID=" + lblRoot2.Text);
            }
        }

        private void MoveNode(string sourceTableName, string targetTableName, int nodeId, int parentId)
        {
            /*
             * WARNING
             * For Demo purpose, cascade situation is not handled.
             */

            //get the source node text
            string movedNodeText = (string)OleDbHelper.ExecuteScalar(base.NorthWindConnectionString, CommandType.Text, string.Format("select ProductName from {0} where ProductId={1}", sourceTableName, nodeId));

            movedNodeText = movedNodeText.Replace("'", "");

            //delete the source node
            OleDbHelper.ExecuteNonQuery(base.NorthWindConnectionString, CommandType.Text, string.Format("DELETE FROM {0} where ProductId={1}", sourceTableName, nodeId));

            //get new id
            string maxSql = string.Format("select max( productId ) from {0}", targetTableName);
            int max = (int)OleDbHelper.ExecuteScalar(base.NorthWindConnectionString, CommandType.Text, maxSql);
            int newId = max + 1;

            //add new node to target parent
            OleDbHelper.ExecuteNonQuery(base.NorthWindConnectionString, CommandType.Text, string.Format("INSERT INTO {0} (ProductId, ProductName, ParentId) VALUES({1}, '{2}', {3})", targetTableName, newId, movedNodeText, parentId));
            BindData();
        }

        private void ChangeParent(string tableName, int nodeId, int parentId)
        {
            OleDbHelper.ExecuteNonQuery(base.NorthWindConnectionString, CommandType.Text, string.Format("UPDATE {0} SET ParentId={1} WHERE ProductId={2}", tableName, parentId, nodeId));

            //get new id
            string maxSql = string.Format("select max( productId ) from {0}", tableName);
            int max = (int)OleDbHelper.ExecuteScalar(base.NorthWindConnectionString, CommandType.Text, maxSql);
            int newId = max + 1;

            OleDbHelper.ExecuteNonQuery(base.NorthWindConnectionString, CommandType.Text, string.Format("UPDATE {0} SET ProductID={1} WHERE ProductId={2}", tableName, newId, nodeId));
            OleDbHelper.ExecuteNonQuery(base.NorthWindConnectionString, CommandType.Text, string.Format("UPDATE {0} SET ParentId={1} WHERE ParentID={2}", tableName, newId, nodeId));
        }

        protected void btnUpdat_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(tbItem.Text))
                return;
            OleDbHelper.ExecuteNonQuery(base.NorthWindConnectionString, CommandType.Text, string.Format("Update ProductsTree set ProductName='{0}' where ProductId={1}", tbItem.Text, lblRoot.Text));
            BindData();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(tbItem.Text))
                return;

            string maxSql = string.Format("select max( productId ) from ProductsTree");
            int max = (int)OleDbHelper.ExecuteScalar(base.NorthWindConnectionString, CommandType.Text, maxSql);
            int newId = max + 1;

            OleDbHelper.ExecuteNonQuery(base.NorthWindConnectionString, CommandType.Text, string.Format("INSERT INTO ProductsTree (ProductId, ProductName, ParentId) VALUES({0}, '{1}', 0)", newId, tbItem.Text));
            lblRoot.Text = newId.ToString();
            tbItem.Text = "";
            BindData();
        }

        protected void btnUpdat2_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(tbItem2.Text))
                return;
            OleDbHelper.ExecuteNonQuery(base.NorthWindConnectionString, CommandType.Text, string.Format("Update ProductsTree set ProductName='{0}' where ProductId={1}", tbItem2.Text, lblRoot2.Text));
            BindData();
        }

        protected void btnAdd2_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(tbItem2.Text))
                return;

            string maxSql = string.Format("select max( productId ) from ProductsTree");
            int max = (int)OleDbHelper.ExecuteScalar(base.NorthWindConnectionString, CommandType.Text, maxSql);
            int newId = max + 1;

            OleDbHelper.ExecuteNonQuery(base.NorthWindConnectionString, CommandType.Text, string.Format("INSERT INTO ProductsTree (ProductId, ProductName, ParentId) VALUES({0}, '{1}', 0)", newId, tbItem2.Text));
            lblRoot2.Text = newId.ToString();
            tbItem2.Text = "";
            BindData();
        }
    }
}
