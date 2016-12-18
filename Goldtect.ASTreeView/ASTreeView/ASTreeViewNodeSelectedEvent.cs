using System;
using System.Collections.Generic;
using System.Text;

namespace Goldtect
{
	///<exclude/>
	public delegate void ASTreeViewNodeSelectedEventHandler( object src, ASTreeViewNodeSelectedEventArgs e );

	/// <summary>
	/// ASTreeView Node Selected EventArgs
	/// </summary>
	///<exclude/>
	public sealed class ASTreeViewNodeSelectedEventArgs : EventArgs
	{
		private readonly string _nodeText = string.Empty;
		private readonly string _nodeValue = string.Empty;

		/// <summary>
		/// Initializes a new instance of the <see cref="ASTreeViewNodeSelectedEventArgs"/> class.
		/// </summary>
		/// <param name="nodeText">The node text.</param>
		/// <param name="nodeValue">The node value.</param>
		public ASTreeViewNodeSelectedEventArgs( string nodeText, string nodeValue )
		{
			this._nodeText = nodeText;
			this._nodeValue = nodeValue;
		}

		/// <summary>
		/// Gets the node text.
		/// </summary>
		/// <value>The node text.</value>
		public string NodeText
		{
			get { return this._nodeText; }
		}

		/// <summary>
		/// Gets the node value.
		/// </summary>
		/// <value>The node value.</value>
		public string NodeValue
		{
			get { return this._nodeValue; }
		}
	}
}
