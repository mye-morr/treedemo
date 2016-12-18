using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Goldtect
{
	/// <summary>
	/// Text Node for ASTreeView. It generates any html as tree node.
	/// </summary>
	[Serializable]
	public class ASTreeViewTextNode : ASTreeViewNode
	{
		#region properties

		#region NodeType

		/// <summary>
		/// Gets the type of the node.
		/// </summary>
		/// <value>The type of the node.</value>
		override public ASTreeViewNodeType NodeType
		{
			get { return ASTreeViewNodeType.Text; }
		}

		#endregion

		#endregion

		#region constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="ASTreeViewLinkNode"/> class.
		/// </summary>
		/// <param name="nodeText">The node text.</param>
		public ASTreeViewTextNode(
			string nodeText)
			: base( nodeText, string.Empty )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ASTreeViewLinkNode"/> class.
		/// </summary>
		/// <param name="nodeText">The node text.</param>
		/// <param name="nodeValue">The node value.</param>
		public ASTreeViewTextNode(
			string nodeText
			, string nodeValue)
			: base( nodeText, nodeValue )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ASTreeViewLinkNode"/> class.
		/// </summary>
		internal ASTreeViewTextNode()
			: base()
		{
		}

		#endregion
	}
}
