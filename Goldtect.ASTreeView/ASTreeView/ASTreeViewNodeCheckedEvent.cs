using System;
using System.Collections.Generic;
using System.Text;

namespace Goldtect
{
	///<exclude/>
	public delegate void ASTreeViewNodeCheckedEventHandler( object src, ASTreeViewNodeCheckedEventArgs e );

	/// <summary>
	/// ASTreeView Node Checked EventArgs
	/// </summary>
	///<exclude/>
	public sealed class ASTreeViewNodeCheckedEventArgs : EventArgs
	{
		private readonly string _nodeText = string.Empty;
		private readonly string _nodeValue = string.Empty;
		private readonly ASTreeViewCheckboxState _checkedState = ASTreeViewCheckboxState.Unchecked;

		/// <summary>
		/// Initializes a new instance of the <see cref="ASTreeViewNodeCheckedEventArgs"/> class.
		/// </summary>
		/// <param name="nodeText">The node text.</param>
		/// <param name="nodeValue">The node value.</param>
		/// <param name="checkedState">State of the checked.</param>
		public ASTreeViewNodeCheckedEventArgs( string nodeText, string nodeValue, ASTreeViewCheckboxState checkedState )
		{
			this._nodeText = nodeText;
			this._nodeValue = nodeValue;
			this._checkedState = checkedState;
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

		/// <summary>
		/// Gets the state of the checked.
		/// </summary>
		/// <value>The state of the checked.</value>
		public ASTreeViewCheckboxState CheckedState
		{
			get { return this._checkedState; }
		}
	}
}
