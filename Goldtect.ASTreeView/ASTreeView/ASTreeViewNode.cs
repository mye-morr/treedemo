using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using Goldtect.Utilities.Json;

namespace Goldtect
{
	/// <summary>
	/// Node class of ASTreeView. It generates LinkButton as tree nodes.
	/// </summary>
	[Serializable]
	public class ASTreeViewNode : CollectionBase
	{
		#region Properties

		#region ParentNode

		private ASTreeViewNode parentNode = null;

		/// <summary>
		/// Gets or sets the parent node.
		/// </summary>
		/// <value>The parent node.</value>
		public ASTreeViewNode ParentNode
		{
			get { return parentNode; }
			set { this.parentNode = value; }
		}

		#endregion

		#region ChildNodes

		private List<ASTreeViewNode> childNodes = new List<ASTreeViewNode>();

		/// <summary>
		/// Gets the child nodes.
		/// </summary>
		/// <value>The child nodes.</value>
		public List<ASTreeViewNode> ChildNodes
		{
			get { return childNodes; }
		}

		#endregion

		#region NodeText

		private string nodeText;

		/// <summary>
		/// Gets or sets the node text.
		/// </summary>
		/// <value>The node text.</value>
		public string NodeText
		{
			get { return nodeText; }
			set { nodeText = value; }
		}

		#endregion

		#region NodeValue

		private string nodeValue;

		/// <summary>
		/// Gets or sets the node value.
		/// </summary>
		/// <value>The node value.</value>
		public string NodeValue
		{
			get { return nodeValue; }
			set { nodeValue = value; }
		}

		#endregion NodeValue

		#region CheckedState

		private ASTreeViewCheckboxState checkedState = ASTreeViewCheckboxState.Unchecked;

		/// <summary>
		/// Gets or sets the state of the checked.
		/// </summary>
		/// <value>The state of the checked.</value>
		public ASTreeViewCheckboxState CheckedState
		{
			get { return checkedState; }
			set { checkedState = value; }
		}


		#endregion

		#region OpenState

		private ASTreeViewNodeOpenState openState = ASTreeViewNodeOpenState.Open;

		/// <summary>
		/// Gets or sets the state of the open.
		/// </summary>
		/// <value>The state of the open.</value>
		public ASTreeViewNodeOpenState OpenState
		{
			get { return openState; }
			set { openState = value; }
		}

		#endregion

		#region Selected

		private bool selected = false;

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="ASTreeViewNode"/> is selected.
		/// </summary>
		/// <value><c>true</c> if selected; otherwise, <c>false</c>.</value>
		public bool Selected
		{
			get { return selected; }
			set { selected = value; }
		}

		#endregion

		#region EnableEditContextMenu

		private bool enableEditContextMenu = true;

		/// <summary>
		/// Gets or sets a value indicating whether to enable edit context menu on this node.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [enable edit context menu]; otherwise, <c>false</c>.
		/// </value>
		public bool EnableEditContextMenu
		{
			get { return enableEditContextMenu; }
			set { enableEditContextMenu = value; }
		}

		#endregion

		#region EnableDeleteContextMenu

		private bool enableDeleteContextMenu = true;

		/// <summary>
		/// Gets or sets a value indicating whether to enable delete context menu on this node.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [enable delete context menu]; otherwise, <c>false</c>.
		/// </value>
		public bool EnableDeleteContextMenu
		{
			get { return enableDeleteContextMenu; }
			set { enableDeleteContextMenu = value; }
		}

		#endregion

		#region EnableAddContextMenu

		private bool enableAddContextMenu = true;

		/// <summary>
		/// Gets or sets a value indicating whether to enable add context menu on this node.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [enable add context menu]; otherwise, <c>false</c>.
		/// </value>
		public bool EnableAddContextMenu
		{
			get { return enableAddContextMenu; }
			set { enableAddContextMenu = value; }
		}

		#endregion

		#region NodeType

		/// <summary>
		/// Gets the type of the node.
		/// </summary>
		/// <value>The type of the node.</value>
		virtual public ASTreeViewNodeType NodeType
		{
			get
			{
				return ASTreeViewNodeType.LinkButton;
			}
		}

		#endregion

		#region NodeIcon

		private string nodeIcon;

		/// <summary>
		/// Gets or sets the node icon.
		/// </summary>
		/// <value>The node icon.</value>
		public string NodeIcon
		{
			get { return nodeIcon; }
			set { nodeIcon = value; }
		}

		#endregion

		#region EnableDragDrop

		private bool enableDragDrop = true;

		/// <summary>
		/// Gets or sets a value indicating whether to enable drag and drop on this node.
		/// </summary>
		/// <value><c>true</c> if [enable drag drop]; otherwise, <c>false</c>.</value>
		public bool EnableDragDrop
		{
			get { return enableDragDrop; }
			set { enableDragDrop = value; }
		}

		#endregion

		#region EnableSiblings

		private bool enableSiblings = true;

		/// <summary>
		/// Gets or sets a value indicating whether enable sibleing. This property only work on the root node.
		/// </summary>
		/// <value><c>true</c> if [enable siblings]; otherwise, <c>false</c>.</value>
		public bool EnableSiblings
		{
			get { return enableSiblings; }
			set { enableSiblings = value; }
		}

		#endregion

		#region EnableChildren

		private bool enableChildren = true;

		/// <summary>
		/// Gets or sets a value indicating whether this node can have children.
		/// </summary>
		/// <value><c>true</c> if [enable children]; otherwise, <c>false</c>.</value>
		public bool EnableChildren
		{
			get { return enableChildren; }
			set { enableChildren = value; }
		}

		#endregion

		#region EnableCheckbox

		private bool enableCheckbox = true;

		/// <summary>
		/// Gets or sets a value indicating whether to display the checkbox for the node.
		/// </summary>
		/// <value><c>true</c> if [enable checkbox]; otherwise, <c>false</c>.</value>
		public bool EnableCheckbox
		{
			get { return enableCheckbox; }
			set { enableCheckbox = value; }
		}

		#endregion

		#region EnableSelection

		private bool enableSelection = true;

		/// <summary>
		/// Gets or sets a value indicating whether the node is selectable.
		/// </summary>
		/// <value><c>true</c> if [enable selection]; otherwise, <c>false</c>.</value>
		public bool EnableSelection
		{
			get { return enableSelection; }
			set { enableSelection = value; }
		}

		#endregion

		#region AdditionalAttributes (KeyValuePair<string, string>)

		private List<KeyValuePair<string, string>> additionalAttributes = new List<KeyValuePair<string, string>>();

		/// <summary>
		/// Gets or sets the additional attributes on this node.
		/// </summary>
		/// <value>The additional attributes.</value>
		public List<KeyValuePair<string, string>> AdditionalAttributes
		{
			get { return additionalAttributes; }
			set { additionalAttributes = value; }
		}


		#endregion

		#region IsVirtualNode

		private bool isVirtualNode = false;

		/// <summary>
		/// Gets or sets a value indicating whether this instance is virtual node.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is virtual node; otherwise, <c>false</c>.
		/// </value>
		public bool IsVirtualNode
		{
			get { return isVirtualNode; }
			set { isVirtualNode = value; }
		}

		#endregion

		#region VirtualNodesCount

		private int virtualNodesCount = 0;

		/// <summary>
		/// Gets or sets the virtual nodes count. The virtual node count is a number which indicates how many children this node has.
		/// </summary>
		/// <value>The virtual nodes count.</value>
		public int VirtualNodesCount
		{
			get { return virtualNodesCount; }
			set { virtualNodesCount = value; }
		}

		#endregion

		#region VirtualParentKey

		private string virtualParentKey;

		/// <summary>
		/// Gets or sets the virtual parent key. This property is used when performing loading ajax calls.
		/// </summary>
		/// <value>The virtual parent key.</value>
		public string VirtualParentKey
		{
			get { return virtualParentKey; }
			set { virtualParentKey = value; }
		}

		#endregion

		#region NodeDepth

		private int nodeDepth = -1;

		/// <summary>
		/// Gets or sets the depth.
		/// </summary>
		/// <value>The depth.</value>
		public int NodeDepth
		{
			get { return nodeDepth; }
			set { nodeDepth = value; }
		}

		#endregion

		#region EnableOpenClose

		private bool enableOpenClose = true;

		/// <summary>
		/// Gets or sets a value indicating whether [enable open close].
		/// </summary>
		/// <value><c>true</c> if [enable open close]; otherwise, <c>false</c>.</value>
		public bool EnableOpenClose
		{
			get { return enableOpenClose; }
			set { enableOpenClose = value; }
		}

		#endregion

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="ASTreeViewNode"/> class.
		/// </summary>
		internal ASTreeViewNode()
		{
			this.nodeText = string.Empty;
			this.nodeValue = string.Empty;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ASTreeViewNode"/> class.
		/// </summary>
		/// <param name="nodeTextValue">The node text value.</param>
		public ASTreeViewNode( string nodeTextValue )
		{
			this.nodeText = nodeTextValue;
			this.nodeValue = nodeTextValue;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ASTreeViewNode"/> class.
		/// </summary>
		/// <param name="nodeText">The node text.</param>
		/// <param name="nodeValue">The node value.</param>
		public ASTreeViewNode( string nodeText
			, string nodeValue )
		{
			this.nodeText = nodeText;
			this.nodeValue = nodeValue;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ASTreeViewNode"/> class.
		/// </summary>
		/// <param name="nodeText">The node text.</param>
		/// <param name="nodeValue">The node value.</param>
		/// <param name="nodeIcon">The node icon.</param>
		public ASTreeViewNode( string nodeText
			, string nodeValue
			, string nodeIcon )
		{
			this.nodeText = nodeText;
			this.nodeValue = nodeValue;
			this.nodeIcon = nodeIcon;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ASTreeViewNode"/> class.
		/// </summary>
		/// <param name="nodeText">The node text.</param>
		/// <param name="nodeValue">The node value.</param>
		/// <param name="nodeIcon">The node icon.</param>
		/// <param name="virtualNodesCount">The virtual nodes count.</param>
		/// <param name="virtualParentKey">The virtual parent key.</param>
		public ASTreeViewNode( string nodeText
			, string nodeValue
			, string nodeIcon
			, int virtualNodesCount
			, string virtualParentKey ) : this( nodeText, nodeValue, nodeIcon )
		{
			this.isVirtualNode = true;
			this.virtualNodesCount = virtualNodesCount;
			this.virtualParentKey = virtualParentKey;
		}

		#endregion
   
		#region Public Methods

		#region AppendChild

		/// <summary>
		/// Appends the child.
		/// </summary>
		/// <param name="child">The child.</param>
		/// <returns></returns>
		public ASTreeViewNode AppendChild( ASTreeViewNode child )
		{
			child.SetParent( this );
			this.childNodes.Add( child );

			return this;
		}

		#endregion

		#region Insert

		/// <summary>
		/// Inserts the specified index.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <param name="node">The node.</param>
		/// <returns></returns>
		public ASTreeViewNode Insert( int index, ASTreeViewNode node )
		{
			if( index > this.childNodes.Count )
				this.AppendChild( node );
			else
				this.childNodes.Insert( index, node );

			node.SetParent( this );

			return this;
		}

		#endregion

		#region Remove

		/// <summary>
		/// Removes the specified node.
		/// </summary>
		/// <param name="node">The node.</param>
		/// <returns></returns>
		public ASTreeViewNode Remove( ASTreeViewNode node )
		{
			if( this.childNodes.Contains( node ) )
			{
				this.childNodes.Remove( node );
				node.SetParent( null );
			}

			return this;
		}

		#endregion

		#region RemoveAt

		/// <summary>
		/// Removes at.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns></returns>
		new public bool RemoveAt( int index )
		{
			if( index >= this.childNodes.Count )
				return false;

			this.childNodes.RemoveAt( index );

			return true;
		}

		#endregion

		#region Clear

		/// <summary>
		/// Clears this instance.
		/// </summary>
		/// <returns></returns>
		new public ASTreeViewNode Clear()
		{
			this.childNodes.Clear();

			return this;
		}

		#endregion

		#region IndexOf

		/// <summary>
		/// Indexes the of.
		/// </summary>
		/// <param name="node">The node.</param>
		/// <returns></returns>
		public int IndexOf( ASTreeViewNode node )
		{
			return this.childNodes.IndexOf( node );
		}

		#endregion

		#region ContainsOffspring

		/// <summary>
		/// Determines whether the specified node contains offspring.
		/// </summary>
		/// <param name="node">The node.</param>
		/// <returns>
		/// 	<c>true</c> if the specified node contains offspring; otherwise, <c>false</c>.
		/// </returns>
		public bool ContainsOffspring( ASTreeViewNode node )
		{
			if( this.childNodes.Contains( node ) )
				return true;

			foreach( ASTreeViewNode child in this.childNodes )
			{
				if( child.ContainsOffspring( node ) )
					return true;
			}

			return false;
		}

		/// <summary>
		/// Determines whether the specified node text contains offspring.
		/// </summary>
		/// <param name="nodeText">The node text.</param>
		/// <param name="nodeValue">The node value.</param>
		/// <returns>
		/// 	<c>true</c> if the specified node text contains offspring; otherwise, <c>false</c>.
		/// </returns>
		public bool ContainsOffspring( string nodeText, string nodeValue )
		{
			if( this.Contains( nodeText, nodeValue ) )
				return true;

			foreach( ASTreeViewNode child in this.childNodes )
			{
				if( child.ContainsOffspring( nodeText, nodeValue ) )
					return true;
			}

			return false;
		}

		/// <summary>
		/// Determines whether the specified node value contains offspring.
		/// </summary>
		/// <param name="nodeValue">The node value.</param>
		/// <returns>
		/// 	<c>true</c> if the specified node value contains offspring; otherwise, <c>false</c>.
		/// </returns>
		public bool ContainsOffspring( string nodeValue )
		{
			if( this.Contains( nodeValue ) )
				return true;

			foreach( ASTreeViewNode child in this.childNodes )
			{
				if( child.ContainsOffspring( nodeValue ) )
					return true;
			}

			return false;
		}

		#endregion

		#region Contains

		/// <summary>
		/// Determines whether [contains] [the specified node].
		/// </summary>
		/// <param name="node">The node.</param>
		/// <returns>
		/// 	<c>true</c> if [contains] [the specified node]; otherwise, <c>false</c>.
		/// </returns>
		public bool Contains( ASTreeViewNode node )
		{
			return this.childNodes.Contains( node );
		}

		/// <summary>
		/// Determines whether [contains] [the specified node text].
		/// </summary>
		/// <param name="nodeText">The node text.</param>
		/// <param name="nodeValue">The node value.</param>
		/// <returns>
		/// 	<c>true</c> if [contains] [the specified node text]; otherwise, <c>false</c>.
		/// </returns>
		public bool Contains( string nodeText, string nodeValue )
		{
			foreach( ASTreeViewNode node in this.childNodes )
			{
				if( node.NodeText == nodeText && node.NodeValue == nodeValue )
					return true;
			}

			return false;
		}

		/// <summary>
		/// Determines whether [contains] [the specified node value].
		/// </summary>
		/// <param name="nodeValue">The node value.</param>
		/// <returns>
		/// 	<c>true</c> if [contains] [the specified node value]; otherwise, <c>false</c>.
		/// </returns>
		public bool Contains( string nodeValue )
		{
			foreach( ASTreeViewNode node in this.childNodes )
			{
				if( node.NodeValue == nodeValue )
					return true;
			}

			return false;
		}

		#endregion

		#endregion

		#region Public Methods

		/// <summary>
		/// Sets the parent.
		/// </summary>
		/// <param name="parent">The parent.</param>
		protected void SetParent( ASTreeViewNode parent )
		{
			this.parentNode = parent;
		}

		#endregion

		#region Override Methods

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return this.ToString( true );
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <param name="recursive">if set to <c>true</c> [recursive].</param>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		public string ToString( bool recursive )
		{
			if( !recursive )
				return string.Format( "[\"{0}\",\"{1}\"]", this.nodeText, this.nodeValue );

			StringBuilder sbResult = new StringBuilder();

			string current = string.Format( "<ul>[\"{0}\",\"{1}\"]", this.nodeText, this.nodeValue );
			sbResult.Append( current );

			foreach( ASTreeViewNode child in this.childNodes )
			{
				sbResult.Append( "<li>" + child.ToString( recursive ) + "</li>" );
			}

			sbResult.Append( "</ul>" );
			return sbResult.ToString();
		}

		#endregion

	}
}
