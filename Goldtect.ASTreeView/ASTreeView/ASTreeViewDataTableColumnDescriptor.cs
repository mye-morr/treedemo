using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Goldtect
{
	/// <summary>
	/// ASTreeView DataTable Column Descriptor. Populates the ASTreeView with a datatable as datasource.
	/// </summary>
	[Serializable]
	public class ASTreeViewDataTableColumnDescriptor : ASTreeViewDataSourceDescriptor
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ASTreeViewDataTableColumnDescriptor"/> class.
		/// </summary>
		/// <param name="nodeTextColumn">The node text column.</param>
		/// <param name="nodeValueColumn">The node value column.</param>
		/// <param name="parentNodeValueColumn">The parent node value column.</param>
		public ASTreeViewDataTableColumnDescriptor( string nodeTextColumn
			, string nodeValueColumn
			, string parentNodeValueColumn ) : base(  nodeTextColumn
													, nodeValueColumn
													, parentNodeValueColumn )
		{
		}

		#region AddSingleQuotationOnQuery

		private bool addSingleQuotationOnQuery = true;

		/// <summary>
		/// Gets or sets a value indicating whether to add single quotation on query.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [add single quotation on query]; otherwise, <c>false</c>.
		/// </value>
		public bool AddSingleQuotationOnQuery
		{
			get { return addSingleQuotationOnQuery; }
			set { addSingleQuotationOnQuery = value; }
		}

		#endregion

		#region BuildTreeFromDataTable

		/// <summary>
		/// Builds the tree from data source.
		/// </summary>
		/// <param name="parentNode">The parent node.</param>
		/// <param name="dataSource">The data source.</param>
		/// <param name="parentNodeValue">The parent node value.</param>
		public override void BuildTreeFromDataSource( ASTreeViewNode parentNode, object dataSource, object parentNodeValue )
		{
			DataTable dt = (DataTable)dataSource;
			string parentValue = ( string )parentNodeValue;
			if( string.IsNullOrEmpty( this.NodeTextColumnName )
				|| string.IsNullOrEmpty( this.NodeValueColumnName )
				|| string.IsNullOrEmpty( this.ParentNodeValueColumnName ) )
				throw new ArgumentNullException( "Missing minimal descriptor information!" );

			string whereClause = string.Empty;
			if( parentValue != null )
			{
				string pattern = this.AddSingleQuotationOnQuery ? "[{0}]='{1}'" : "[{0}]={1}";

				whereClause = string.Format( pattern, this.ParentNodeValueColumnName, parentValue );
			}
			else
				whereClause = string.Format( "[{0}] is NULL", this.ParentNodeValueColumnName );

			DataRow[] drs = dt.Select( whereClause );
			foreach( DataRow dr in drs )
			{
				//convert node
				ASTreeViewNode node = new ASTreeViewNode();
				node.NodeText = GetString( dr[this.NodeTextColumnName], true );
				node.NodeValue = GetString( dr[this.NodeValueColumnName], true );
				parentNode.AppendChild( node );


				string currentNodeValue = node.NodeValue;
				//call recursively
				BuildTreeFromDataSource( node, dt, currentNodeValue );
			}

		}

		#endregion 

		/// <summary>
		/// Gets the string.
		/// </summary>
		/// <param name="o">The o.</param>
		/// <param name="useToString">if set to <c>true</c> [use to string].</param>
		/// <returns></returns>
		private static string GetString( object o, bool useToString )
		{
			if( useToString )
				return ( o == null ) ? string.Empty : o.ToString();
			else
				return ( o == null ) ? string.Empty : (string)o;

		}
	}
}
