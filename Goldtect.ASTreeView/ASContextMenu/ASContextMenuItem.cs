using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.HtmlControls;

namespace Goldtect
{
	/// <summary>
	/// Context menu item class for ASContextMenu control
	/// </summary>
	[Serializable]
	public class ASContextMenuItem
	{
		#region Properies

		#region ItemId

		private string itemId = Guid.NewGuid().ToString().Replace( "-", "" );

		public string ItemId
		{
			get { return itemId; }
			set { itemId = value; }
		}

		#endregion

		#region ItemText

		private string itemText = string.Empty;

		public string ItemText
		{
			get { return itemText; }
			set { itemText = value; }
		}

		#endregion

		#region Href

		private string href = "javascript:void(0);";

		public string Href
		{
			get { return href; }
			set { href = value; }
		}

		#endregion

		#region Target

		private string target = string.Empty;

		public string Target
		{
			get { return target; }
			set { target = value; }
		}

		#endregion

		#region JsFunction

		private string jsFunction = string.Empty;

		public string JsFunction
		{
			get { return jsFunction; }
			set { jsFunction = value; }
		}

		#endregion

		#region Attributes

		private List<KeyValuePair<string, string>> attributes = new List<KeyValuePair<string, string>>();

		public List<KeyValuePair<string, string>> Attributes
		{
			get { return attributes; }
			set { attributes = value; }
		}

		#endregion

		#region CommandName

		private string commandName = string.Empty;

		public string CommandName
		{
			get { return commandName; }
			set { commandName = value; }
		}

		#endregion

		#endregion

		#region public methods

		public HtmlAnchor GenerateAnchor()
		{
			HtmlAnchor a = new HtmlAnchor();
			a.ID = this.itemId;
			a.HRef = this.href;
			a.Target = this.target;
			a.InnerHtml = this.itemText;

			if( !string.IsNullOrEmpty( this.jsFunction ) )
				a.Attributes.Add( "onclick", jsFunction );

			if( !string.IsNullOrEmpty( this.CommandName ) )
				a.Attributes.Add( "cmdName", this.commandName );

			foreach( KeyValuePair<string, string> attr in this.attributes )
				a.Attributes.Add( attr.Key, attr.Value );

			return a;
		}

		#endregion


		public ASContextMenuItem( string itemText )
		{
			this.itemText = itemText;
		}

		public ASContextMenuItem( string itemText, string jsFunction ) : this( itemText )
		{
			this.jsFunction = jsFunction;
		}

		public ASContextMenuItem( string itemText, string jsFunction, string commandName ) :  this( itemText, jsFunction )
		{
			this.commandName = commandName;
		}
	}
}
