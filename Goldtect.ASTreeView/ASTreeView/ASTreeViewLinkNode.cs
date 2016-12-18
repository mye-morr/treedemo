using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;


namespace Goldtect
{
	/// <summary>
	/// HyperLink node type of ASTreeView. It generates hyperlinks as tree nodes.
	/// </summary>
	[Serializable]
	public class ASTreeViewLinkNode : ASTreeViewNode
	{
		#region properties

		#region NodeType

		/// <summary>
		/// Gets the type of the node.
		/// </summary>
		/// <value>The type of the node.</value>
		override public ASTreeViewNodeType NodeType
		{
			get { return ASTreeViewNodeType.HyperLink; }
		}

		#endregion

		#region NavigateUrl

		private string navigateUrl = string.Empty;

		/// <summary>
		/// Gets or sets the navigate URL.
		/// </summary>
		/// <value>The navigate URL.</value>
		public string NavigateUrl
		{
			get { return navigateUrl; }
			set { navigateUrl = value; }
		}

		#endregion

		#region Target

		private string target = string.Empty;

		/// <summary>
		/// Gets or sets the target.
		/// </summary>
		/// <value>The target.</value>
		public string Target
		{
			get { return target; }
			set { target = value; }
		}

		#endregion

		#region Tooltip

		private string tooltip = string.Empty;

		/// <summary>
		/// Gets or sets the tooltip.
		/// </summary>
		/// <value>The tooltip.</value>
		public string Tooltip
		{
			get { return tooltip; }
			set { tooltip = value; }
		}

		#endregion

		#endregion

		#region constructor


		/// <summary>
		/// Initializes a new instance of the <see cref="ASTreeViewLinkNode"/> class.
		/// </summary>
		internal ASTreeViewLinkNode()
			: base()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ASTreeViewLinkNode"/> class.
		/// </summary>
		/// <param name="nodeText">The node text.</param>
		/// <param name="nodeValue">The node value.</param>
		public ASTreeViewLinkNode(
			string nodeText
			, string nodeValue)
			: base( nodeText, nodeValue )
		{
			this.navigateUrl = "#";
			this.target = "_self";
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ASTreeViewLinkNode"/> class.
		/// </summary>
		/// <param name="nodeText">The node text.</param>
		/// <param name="nodeValue">The node value.</param>
		/// <param name="navigateUrl">The navigate URL.</param>
		public ASTreeViewLinkNode( 
			string nodeText
			, string nodeValue
			, string navigateUrl ) : base( nodeText, nodeValue )
		{
			this.navigateUrl = navigateUrl;
			this.target = "_self";
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ASTreeViewLinkNode"/> class.
		/// </summary>
		/// <param name="nodeText">The node text.</param>
		/// <param name="nodeValue">The node value.</param>
		/// <param name="navigateUrl">The navigate URL.</param>
		/// <param name="nodeIcon">The node icon.</param>
		public ASTreeViewLinkNode( 
			string nodeText
			, string nodeValue
			, string navigateUrl
			, string nodeIcon ) : base( nodeText, nodeValue, nodeIcon )
		{
			this.navigateUrl = navigateUrl;
			this.target = "_self";
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ASTreeViewLinkNode"/> class.
		/// </summary>
		/// <param name="nodeText">The node text.</param>
		/// <param name="nodeValue">The node value.</param>
		/// <param name="navigateUrl">The navigate URL.</param>
		/// <param name="target">The target.</param>
		/// <param name="tooltip">The tooltip.</param>
		public ASTreeViewLinkNode(
			string nodeText
			, string nodeValue
			, string navigateUrl
			, string target
			, string tooltip ) : base( nodeText, nodeValue )
		{
			this.navigateUrl = navigateUrl;
			this.target = target;
			this.tooltip = tooltip;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ASTreeViewLinkNode"/> class.
		/// </summary>
		/// <param name="nodeText">The node text.</param>
		/// <param name="nodeValue">The node value.</param>
		/// <param name="navigateUrl">The navigate URL.</param>
		/// <param name="target">The target.</param>
		/// <param name="tooltip">The tooltip.</param>
		/// <param name="nodeIcon">The node icon.</param>
		public ASTreeViewLinkNode(
			string nodeText
			, string nodeValue
			, string navigateUrl
			, string target
			, string tooltip
			, string nodeIcon ) : base( nodeText, nodeValue, nodeIcon )
		{
			this.navigateUrl = navigateUrl;
			this.target = target;
			this.tooltip = tooltip;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ASTreeViewLinkNode"/> class.
		/// </summary>
		/// <param name="nodeText">The node text.</param>
		/// <param name="nodeValue">The node value.</param>
		/// <param name="navigateUrl">The navigate URL.</param>
		/// <param name="target">The target.</param>
		/// <param name="tooltip">The tooltip.</param>
		/// <param name="nodeIcon">The node icon.</param>
		/// <param name="virtualNodesCount">The virtual nodes count.</param>
		/// <param name="virtualParentKey">The virtual parent key.</param>
		public ASTreeViewLinkNode(
			string nodeText
			, string nodeValue
			, string navigateUrl
			, string target
			, string tooltip
			, string nodeIcon
			, int virtualNodesCount
			, string virtualParentKey ) : this( nodeText, nodeValue ,navigateUrl, target, tooltip, nodeIcon )
		{
			base.IsVirtualNode = true;
			base.VirtualNodesCount = virtualNodesCount;
			base.VirtualParentKey = virtualParentKey;
		}

		#endregion

	}
}
