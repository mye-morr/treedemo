using System;
using System.Collections.Generic;
using System.Text;

namespace Goldtect
{
	/// <summary>
	/// ASTreeView Invalid State Exception
	/// </summary>
	///<exclude/>
	public class ASTreeViewInvalidStateException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ASTreeViewInvalidStateException"/> class.
		/// </summary>
		/// <param name="msg">The MSG.</param>
		public ASTreeViewInvalidStateException( string msg )
			: base( "[Invalid State]" + msg )
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ASTreeViewInvalidStateException"/> class.
		/// </summary>
		/// <param name="msg">The MSG.</param>
		/// <param name="inner">The inner.</param>
		public ASTreeViewInvalidStateException( string msg, Exception inner )
			: base( "[Invalid State]" + msg, inner )
		{

		}

	}

	/// <summary>
	/// ASTreeView Invalid DataSource Exception
	/// </summary>
	///<exclude/>
	public class ASTreeViewInvalidDataSourceException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ASTreeViewInvalidDataSourceException"/> class.
		/// </summary>
		/// <param name="msg">The MSG.</param>
		public ASTreeViewInvalidDataSourceException( string msg )
			: base( "[Invalid Data Source]" + msg )
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ASTreeViewInvalidDataSourceException"/> class.
		/// </summary>
		/// <param name="msg">The MSG.</param>
		/// <param name="inner">The inner.</param>
		public ASTreeViewInvalidDataSourceException( string msg, Exception inner )
			: base( "[Invalid Data Source]" + msg, inner )
		{

		}

	}

}
