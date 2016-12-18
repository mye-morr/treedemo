using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Xml;

namespace Goldtect
{
	/// <summary>
	/// ASTreeView XML Descriptor. Populates the ASTreeView with an xml as datasource.
	/// </summary>
	[Serializable]
	public class ASTreeViewXMLDescriptor : ASTreeViewDataSourceDescriptor
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ASTreeViewXMLDescriptor"/> class.
		/// </summary>
		public ASTreeViewXMLDescriptor()
		{
			
		}

		/// <summary>
		/// Builds the tree from data source.
		/// </summary>
		/// <param name="parentNode">The parent node.</param>
		/// <param name="dataSource">The data source.</param>
		/// <param name="parentNodeValue">The parent node value.</param>
		public override void BuildTreeFromDataSource( ASTreeViewNode parentNode, object dataSource, object parentNodeValue )
		{
			XmlDocument doc = (XmlDocument)dataSource;
			XmlNode docRoot = doc.SelectSingleNode( "/astreeview" );

			ConvertXmlNodeRecursive( parentNode, docRoot );

			/*
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
			*/
		}

		/// <summary>
		/// Converts the XML node recursive.
		/// </summary>
		/// <param name="parentNode">The parent node.</param>
		/// <param name="node">The node.</param>
		private void ConvertXmlNodeRecursive( ASTreeViewNode parentNode, XmlNode node )
		{
			XmlNodeList childNodes = node.SelectNodes( "astreeview-nodes/astreeview-node" );

			if( childNodes == null || childNodes.Count == 0 )
				return;

			foreach( XmlNode childNode in childNodes )
			{
				ASTreeViewNode treeNode = ConvertXmlNodeToTreeNode( childNode );
				
				parentNode.AppendChild( treeNode );

				ConvertXmlNodeRecursive( treeNode, childNode );
			}
		}

		/// <summary>
		/// Converts the XML node to tree node.
		/// </summary>
		/// <param name="node">The node.</param>
		/// <returns></returns>
		private ASTreeViewNode ConvertXmlNodeToTreeNode( XmlNode node )
		{
			ASTreeViewNode treeNode = new ASTreeViewNode();
			ASTreeViewNodeType nodeType = (ASTreeViewNodeType)int.Parse( node.Attributes["node-type"].Value );

			//handle linkbutton
			if( nodeType == ASTreeViewNodeType.HyperLink )
			{
				ASTreeViewLinkNode treeLinkNode = new ASTreeViewLinkNode();

				if( node.Attributes["navigate-url"].Value != null )
					treeLinkNode.NavigateUrl = node.Attributes["navigate-url"].Value;
				if( node.Attributes["target"].Value != null )
					treeLinkNode.Target = node.Attributes["target"].Value;
				if( node.Attributes["tooltip"].Value != null )
					treeLinkNode.Tooltip = node.Attributes["tooltip"].Value;

				treeNode = treeLinkNode;

			}


			treeNode.NodeText = node.Attributes["node-text"].Value;
			treeNode.NodeValue = node.Attributes["node-value"].Value;

			if( node.Attributes["checked-state"].Value != null )
				treeNode.CheckedState = (ASTreeViewCheckboxState)int.Parse( node.Attributes["checked-state"].Value );

			if( node.Attributes["open-state"].Value != null )
				treeNode.OpenState = (ASTreeViewNodeOpenState)int.Parse( node.Attributes["open-state"].Value );

			if( node.Attributes["selected"].Value != null )
				treeNode.Selected = bool.Parse( node.Attributes["selected"].Value );

			if( node.Attributes["enable-edit-context-menu"].Value != null )
				treeNode.EnableEditContextMenu = bool.Parse( node.Attributes["enable-edit-context-menu"].Value );

			if( node.Attributes["enable-delete-context-menu"].Value != null )
				treeNode.EnableDeleteContextMenu = bool.Parse( node.Attributes["enable-delete-context-menu"].Value );

			if( node.Attributes["enable-add-context-menu"] != null )
				treeNode.EnableAddContextMenu = bool.Parse( node.Attributes["enable-add-context-menu"].Value );

			if( node.Attributes["node-icon"].Value != null )
				treeNode.NodeIcon = node.Attributes["node-icon"].Value;

			if( node.Attributes["enable-drag-drop"] != null )
				treeNode.EnableDragDrop = bool.Parse( node.Attributes["enable-drag-drop"].Value );

			if( node.Attributes["enable-siblings"] != null )
				treeNode.EnableSiblings = bool.Parse( node.Attributes["enable-siblings"].Value );

			if( node.Attributes["enable-children"] != null )
				treeNode.EnableChildren = bool.Parse( node.Attributes["enable-children"].Value );

			if( node.Attributes["enable-checkbox"] != null )
				treeNode.EnableCheckbox = bool.Parse( node.Attributes["enable-checkbox"].Value );

			if( node.Attributes["enable-selection"] != null )
				treeNode.EnableSelection = bool.Parse( node.Attributes["enable-selection"].Value );

			if( node.Attributes["enable-open-close"] != null )
				treeNode.EnableOpenClose = bool.Parse( node.Attributes["enable-open-close"].Value );

			return treeNode;
		}
	}
}