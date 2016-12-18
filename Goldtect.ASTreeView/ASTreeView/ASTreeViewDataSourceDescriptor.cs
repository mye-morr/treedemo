using System;
using System.Collections.Generic;
using System.Text;

namespace Goldtect
{
	/// <summary>
	/// Data Source Descriptor for ASTreeView control to populate treeview from datasource. 
	/// </summary>
	[Serializable]
	public abstract class ASTreeViewDataSourceDescriptor
	{
		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="ASTreeViewDataSourceDescriptor"/> class.
		/// </summary>
		public ASTreeViewDataSourceDescriptor()
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ASTreeViewDataSourceDescriptor"/> class.
		/// </summary>
		/// <param name="nodeTextColumn">The node text column.</param>
		/// <param name="nodeValueColumn">The node value column.</param>
		/// <param name="parentNodeValueColumn">The parent node value column.</param>
		public ASTreeViewDataSourceDescriptor( string nodeTextColumn
			, string nodeValueColumn
			, string parentNodeValueColumn )
		{
			this.nodeTextColumnName = nodeTextColumn;
			this.nodeValueColumnName = nodeValueColumn;
			this.parentNodeValueColumnName = parentNodeValueColumn;
		}

		#endregion

		#region NodeValueColumnName

		private string nodeValueColumnName = string.Empty;

		/// <summary>
		/// Gets or sets the name of the node value column.
		/// </summary>
		/// <value>The name of the node value column.</value>
		public string NodeValueColumnName
		{
			get { return nodeValueColumnName; }
			set { nodeValueColumnName = value; }
		}

		#endregion

		#region ParentNodeValueColumnName

		private string parentNodeValueColumnName = string.Empty;

		/// <summary>
		/// Gets or sets the name of the parent node value column.
		/// </summary>
		/// <value>The name of the parent node value column.</value>
		public string ParentNodeValueColumnName
		{
			get { return parentNodeValueColumnName; }
			set { parentNodeValueColumnName = value; }
		}

		#endregion

		#region NodeTextColumnName

		private string nodeTextColumnName = string.Empty;

		/// <summary>
		/// Gets or sets the name of the node text column.
		/// </summary>
		/// <value>The name of the node text column.</value>
		public string NodeTextColumnName
		{
			get { return nodeTextColumnName; }
			set { nodeTextColumnName = value; }
		}

		#endregion

		/// <summary>
		/// Builds the tree from data source.
		/// </summary>
		/// <param name="parentNode">The parent node.</param>
		/// <param name="dataSource">The data source.</param>
		/// <param name="parentNodeValue">The parent node value.</param>
		public abstract void BuildTreeFromDataSource( ASTreeViewNode parentNode, object dataSource, object parentNodeValue );

	}
}
