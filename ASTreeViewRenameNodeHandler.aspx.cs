#region using
using System;
using System.Data;
using System.Data.OleDb;
using System.Web;
using System.Web.UI;

#endregion
namespace Goldtect.ASTreeViewDemo
{
	public partial class ASTreeViewRenameNodeHandler : PageBase
	{
		ASTreeViewAjaxReturnCode returnCode;
		string errorMessage = string.Empty;

		public string NodeValue
		{
			get
			{
				return HttpUtility.UrlDecode( Request.QueryString["nodeValue"] );
			}
		}

		public string NewNodeText
		{
			get
			{
				return HttpUtility.UrlDecode( Request.QueryString["newNodeText"] );
			}
		}


		protected void Page_Load( object sender, EventArgs e )
		{
			if( string.IsNullOrEmpty( this.NodeValue ) || string.IsNullOrEmpty( this.NewNodeText ) )
			{
				returnCode = ASTreeViewAjaxReturnCode.ERROR;
				return;
			}

			try
			{
				this.RenameNode();
			}
			catch( Exception ex )
			{
				this.returnCode = ASTreeViewAjaxReturnCode.ERROR;
				this.errorMessage = ex.Message;
			}
		}

		protected override void Render( HtmlTextWriter writer )
		{
			if( this.returnCode == ASTreeViewAjaxReturnCode.OK )
				writer.Write( (int)this.returnCode );
			else
				writer.Write( this.errorMessage );
		}

		protected void RenameNode()
		{
			string sql = "UPDATE [Products] SET [ProductName]=@NewName WHERE [ProductID]=@PId";
			OleDbHelper.ExecuteNonQuery( base.NorthWindConnectionString
				, CommandType.Text
				, sql
				, new OleDbParameter( "@NewName", this.NewNodeText )
				, new OleDbParameter( "@PId", this.NodeValue ) );
			//DataSet ds = OleDbHelper.ExecuteDataset( base.NorthWindConnectionString, CommandType.Text, "select * from [Products]" );

		}
	}
}
