using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.ComponentModel;
using System.Security.Permissions;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Goldtect.Utilities;

namespace Goldtect
{
	/// <summary>
	/// A context menu control for ASP.Net
	/// </summary>
	[ToolboxData( "<{0}:ASContextMenu runat=server></{0}:ASContextMenu>" )
	, SupportsEventValidation
	, AspNetHostingPermission( SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal )
	, AspNetHostingPermission( SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal )]
	public class ASContextMenu : WebControl, INamingContainer
	{
		#region Declaration

		bool forceRenderInitialScript = false;

		#endregion

		#region properties

		#region MenuItems (ASContextMenuItem)

		private List<ASContextMenuItem> menuItems = new List<ASContextMenuItem>();

		/// <summary>
		/// MenuItems
		/// </summary>
		virtual public List<ASContextMenuItem> MenuItems
		{
			get
			{
				return this.menuItems;
			}
			set
			{
				this.menuItems = value;
			}
		}

		#endregion

		#region Configuration

		#region AttachCssClass (string)

		/// <summary>
		/// AttachCssClass
		/// </summary>
		[Browsable( true )
		, DefaultValue( "" )
		, Category( "Configuration" )]
		public string AttachCssClass
		{
			get
			{
				object o = ViewState["AttachCssClass"];
				return o == null ? "" : (string)o;
			}
			set
			{
				ViewState["AttachCssClass"] = value;
			}
		}

		#endregion

		#region EnableDebugMode

		/// <summary>
		/// EnableDebugMode
		/// </summary>
		[Browsable( true ),
		DefaultValue( false ),
		Category( "Configuration" )]
		virtual public bool EnableDebugMode
		{
			get
			{
				object o = ViewState["EnableDebugMode"];
				return o == null ? false : (bool)o;
			}
			set
			{
				ViewState["EnableDebugMode"] = value;
			}
		}

		#endregion

		#region PreventDefault (bool)

		/// <summary>
		/// PreventDefault
		/// </summary>
		[Browsable( true )
		, DefaultValue( false )
		, Category( "Configuration" )]
		public bool PreventDefault
		{
			get
			{
				object o = ViewState["PreventDefault"];
				return o == null ? false : (bool)o;
			}
			set
			{
				ViewState["PreventDefault"] = value;
			}
		}

		#endregion

		#region PreventForms (bool)

		/// <summary>
		/// PreventForms
		/// </summary>
		[Browsable( true )
		, DefaultValue( false )
		, Category( "Configuration" )]
		public bool PreventForms
		{
			get
			{
				object o = ViewState["PreventForms"];
				return o == null ? false : (bool)o;
			}
			set
			{
				ViewState["PreventForms"] = value;
			}
		}

		#endregion

		#endregion

		#region override properties

		protected override HtmlTextWriterTag TagKey
		{
			get
			{
				return HtmlTextWriterTag.Ul;
			}
		}

		public override string CssClass
		{
			get
			{
				if( string.IsNullOrEmpty( base.CssClass ) )
					return "ASContextMenu";

				return base.CssClass;
			}
			set
			{
				base.CssClass = value;
			}
		}


		#endregion

		#endregion

		#region public methods

		#region ForceRenderInitialScript
		/// <summary>
		/// Forces the render initial script. Especially for this kind of situation: astreeview in a updatepanel which UpdateMode=Conditional, and a trigger calls this update panel's Update() methods. before call Update(), you need call ForceRenderIntialScript first.
		/// </summary>
		public void ForceRenderInitialScript()
		{
			this.forceRenderInitialScript = true;
		}
		#endregion

		public string GetContextMenuObjectId()
		{
			return string.Format( "contextmenu{0}", this.ClientID );
		}

		#endregion

		#region protected methods

		virtual protected void GenerateContextMenu()
		{
			foreach( ASContextMenuItem item in this.MenuItems )
			{
				HtmlGenericControl liItem = new HtmlGenericControl( "li" );
				liItem.Controls.Add( item.GenerateAnchor() );
				this.Controls.Add( liItem );
			}
		}

		virtual protected string GenerateASContextMenuScript()
		{
			if( string.IsNullOrEmpty( this.AttachCssClass ) )
				return string.Empty;

			string script = string.Format( @"
<script type=""text/javascript"">
if( !(_gt && _gt.ASContextMenu ) ){{
	alert('missing script file [_gt.ASContextMenu]!');
}}
else{{
	var contextmenu{0} = new _gt.ASContextMenu();
	contextmenu{0}.setup({{'preventDefault':{1}, 'preventForms':{2}}});
	contextmenu{0}.attachContextMenu('{3}', '{0}');
}}
</script>
", this.ClientID /*0*/
 , this.PreventDefault ? "true" : "false" /*1*/
 , this.PreventForms ? "true" : "false" /*2*/
 , this.AttachCssClass /*3*/ );

			return script;
		}

		#region IsInUpdateAsyncPostback

		/// <summary>
		/// Determines whether [is in update async postback].
		/// </summary>
		/// <returns>
		/// 	<c>true</c> if [is in update async postback]; otherwise, <c>false</c>.
		/// </returns>
		virtual protected bool IsInUpdateAsyncPostback()
		{

			ScriptManager sm = ScriptManager.GetCurrent( this.Page );
			if( sm != null )
				return sm.IsInAsyncPostBack;
			else
				return false;
		}

		#endregion

		/// <summary>
		/// Determines whether control is in partial rendering.
		/// </summary>
		/// <returns>
		/// 	<c>true</c> if control is in partial rendering otherwise, <c>false</c>.
		/// </returns>
		protected virtual bool IsControlInPartialRendering()
		{
			Control p = this.Parent;
			while( p != null )
			{
				if( p is UpdatePanel )
				{
					ScriptManager sm = ScriptManager.GetCurrent( this.Page );
					string s = sm.AsyncPostBackSourceElementID;

					UpdatePanel up = (UpdatePanel)p;
					if( sm.EnablePartialRendering
						&& sm.IsInAsyncPostBack
						&& up.UpdateMode == UpdatePanelUpdateMode.Always )
					{
						return true;
					}
					else if( sm.EnablePartialRendering
						&& sm.IsInAsyncPostBack
						&& up.UpdateMode == UpdatePanelUpdateMode.Conditional )
					{
						//try to find if the trigger's update panel is same as the treeview
						string triggerId = sm.AsyncPostBackSourceElementID;
						if( !string.IsNullOrEmpty( triggerId ) )
						{
							Control trigger = this.Page.FindControl( triggerId );
							if( trigger != null )
							{
								Control triggerParent = trigger.Parent;
								while( triggerParent != null )
								{
									if( triggerParent is UpdatePanel )
									{
										return ( (UpdatePanel)triggerParent ).ID == up.ID;
									}

									triggerParent = triggerParent.Parent;
								}
							}
						}
					}
				}

				p = p.Parent;
			}

			return false;
		}

		#endregion

		#region Override Methods

		/// <summary>
		/// RenderContents
		/// </summary>
		/// <param name="writer"></param>
		protected override void RenderContents( HtmlTextWriter writer )
		{

			#region write a box in design mode

			if( this.Site != null && this.Site.DesignMode == true )
			{
				string htmlForDesignMode = @"
<div style='border:1px dashed blue;width:100px;height:100px;background-color:blue;'>
To config ASContextMenu, please refer to the property page in your Visual Studio.
</div>
";
				writer.Write( htmlForDesignMode );
			}
			else
			{
				GenerateContextMenu();
			}
			#endregion

			base.RenderContents( writer );

		}

		protected override void Render( HtmlTextWriter writer )
		{
			writer.AddAttribute( HtmlTextWriterAttribute.Class, this.CssClass );

			base.Render( writer );
		}

		/// <summary>
		/// OnPreRender
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPreRender( EventArgs e )
		{

			base.OnPreRender( e );

			string script = GenerateASContextMenuScript();
			
			if( ScriptManager.GetCurrent( this.Page ) != null )
			{
				if( IsControlInPartialRendering() || !this.IsInUpdateAsyncPostback() || this.forceRenderInitialScript )
				{
					ScriptManager.RegisterStartupScript( this.Page
							, this.Page.GetType()
							, "js" + Guid.NewGuid()
							, script
							, false );
				}
			}
			else
			{
				this.Page.ClientScript.RegisterStartupScript( this.Page.GetType()
					, "js" + Guid.NewGuid()
					, script );
			}

		}



		#endregion

	}
}
