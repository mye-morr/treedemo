using System.Web.UI;
using System.Data.SqlClient;

namespace Goldtect.ASTreeViewDemo
{
	public class PageBase : Page
	{
		protected string NorthWindConnectionString
		{
			get
			{
				//bind data from data table
				string path = Server.MapPath( "~/" ); //System.AppDomain.CurrentDomain.BaseDirectory;
				//string connStr = string.Format( "Provider=Microsoft.Jet.OLEDB.4.0;Data source={0}db\\NorthWind.mdb", path );
                string connStr = string.Format("Provider=SQLNCLI11;Data source=184.168.194.77;Initial Catalog=narfdaddy1;User ID=narfdaddy;Password=TreeDemo1");
                //string connStr = string.Format("Provider=SQLNCLI11;Server=kerrapp_jeffry\\MSSQLSERVER12;Database=Northwind;User Id=upwork;Password=upwork;Trusted_Connection=True;");

				return connStr;
			}
		}
	}
}
