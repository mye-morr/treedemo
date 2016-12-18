using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.ComponentModel;
using System.Reflection;
using System.Security.Permissions;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;

using Goldtect.Utilities;
using Goldtect.Utilities.Xml;

namespace Goldtect
{
	/// <summary>
	/// A full functinal treeview control for ASP.Net.
	/// </summary>
	[ToolboxData( "<{0}:ASTreeView runat=server></{0}:ASTreeView>" )
   , DefaultEvent( "SelectedNodeChanged" )
   , SupportsEventValidation
   , AspNetHostingPermission( SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal )
   , AspNetHostingPermission( SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal )]
	public class ASTreeView : WebControl, INamingContainer, IPostBackDataHandler
	{
		#region Declaration

		protected HiddenField txtASTreeViewNodes;
		protected string strASTreeViewNodes = string.Empty;
		protected HiddenField txtSelectedASTreeViewNodes;
		protected HiddenField txtCheckedASTreeViewNodes;
		protected HiddenField txtIsBound;
		protected HtmlGenericControl divDebugContainer;
		protected HtmlGenericControl divAjaxIndicatorContainer;
		protected HtmlGenericControl ulASTreeView;
		protected HtmlGenericControl divTreeViewContainer;
		protected ASTreeViewHelper treeViewHelper;
		protected ASTreeViewNode rootNode;
		protected ASContextMenu ascmContextMenu;

		string selectedNodeTextboxSurfix = "_selectedNodeInput";
		string checkedNodeTextboxSurfix = "_checkedNodeInput";

		bool selectedNodeChanged = false;
		bool checkedNodeChanged = false;

		bool forceRenderInitialScript = false;

		#endregion

		#region Properties

		#region Configuration

		#region ID

		/// <summary>
		/// Gets or sets the programmatic identifier assigned to the server control.
		/// </summary>
		/// <value></value>
		/// <returns>The programmatic identifier assigned to the control.</returns>
		[Browsable( true ),
		Category( "Configuration" )]
		public override string ID
		{
			get
			{
				return base.ID;
			}
			set
			{
				base.ID = value;
			}
		}

		#endregion

		#region Version

		/// <summary>
		/// Gets the version of the ASTreeView Control.
		/// </summary>
		/// <value>The version.</value>
		[Browsable( true ),
		Category( "Configuration" )]
		virtual public string Version
		{
			get
			{
				return typeof( ASTreeView ).Assembly.GetName().Version.ToString();
			}
		}

		#endregion

		#region EnableAutoLinkScriptFiles

		/// <summary>
		/// Gets or sets a value indicating whether [enable auto link script files].
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [enable auto link script files]; otherwise, <c>false</c>.
		/// </value>
		[Browsable( true ),
		DefaultValue( false ),
		Category( "Configuration" )]
		[Obsolete( "not be used any more" )]
		virtual public bool EnableAutoLinkScriptFiles
		{
			get
			{
				object o = ViewState["EnableAutoLinkScriptFiles"];
				return o == null ? false : (bool)o;
			}
			set
			{
				ViewState["EnableAutoLinkScriptFiles"] = value;
			}
		}

		#endregion

		#region BasePath

		/// <summary>
		/// Gets or sets the base path. The base path is the folder which contains the astreeview client side stuff.
		/// </summary>
		/// <value>The base path.</value>
		[Browsable( true ),
		DefaultValue( "~/Scripts/astreeview/" ),
		Category( "Configuration" )]
		virtual public string BasePath
		{
			get
			{
				object o = ViewState["BasePath"];
				if( o == null )
					return this.Page.ResolveUrl( "~/Scripts/astreeview/" );
				else
				{
					string path = (string)o;
					if( !path.EndsWith( "/" ) )
						path += "/";

					return this.Page.ResolveUrl( path );
				}
			}
			set
			{
				ViewState["BasePath"] = value;
			}
		}

		#endregion

		#region ImagePath (string)

		/// <summary>
		/// Gets or sets the image path. The default value of ImagePath is BasePath + 'images/'
		/// </summary>
		/// <value>The image path.</value>
		[Browsable( true )
		, Category( "Configuration" )]
		public string ImagePath
		{
			get
			{
				object o = ViewState["ImagePath"];
				return o == null ? this.BasePath + "images/" : (string)o;
			}
			set
			{
				ViewState["ImagePath"] = value;
			}
		}

		#endregion

		#region StyleFile

		/// <summary>
		/// Gets or sets the style file. It's obsolete since automatically linking the style file feature is cancelled.
		/// </summary>
		/// <value>The style file.</value>
		[Browsable( true ),
		DefaultValue( "astreeview.css" ),
		Category( "Configuration" ),
		Obsolete( "no more use" )]
		virtual public string StyleFile
		{
			get
			{
				object o = ViewState["StyleFile"];
				return o == null ? "astreeview.css" : (string)o;
			}
			set
			{
				ViewState["StyleFile"] = value;
			}
		}

		#endregion

		#region ScriptFile

		/// <summary>
		/// Gets or sets the script file. It's obsolete since automatically linking the script file feature is cancelled.
		/// </summary>
		/// <value>The script file.</value>
		[Browsable( true ),
		DefaultValue( "astreeview.js" ),
		Category( "Configuration" ),
		Obsolete( "no more use" )]
		virtual public string ScriptFile
		{
			get
			{
				object o = ViewState["ScriptFile"];
				return o == null ? "astreeview.js" : (string)o;
			}
			set
			{
				ViewState["ScriptFile"] = value;
			}
		}

		#endregion

		#region EnableDebugMode

		/// <summary>
		/// Gets or sets a value indicating whether [enable debug mode].
		/// </summary>
		/// <value><c>true</c> if [enable debug mode]; otherwise, <c>false</c>.</value>
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

		#region Separator (string)

		/// <summary>
		/// Gets or sets the separator. To set the separator for client html container, normally just keep it as default.
		/// </summary>
		/// <value>The separator.</value>
		[Browsable( true )
		, DefaultValue( "$$$^^^" )
		, Category( "Configuration" )]
		public string Separator
		{
			get
			{
				object o = ViewState["Separator"];
				return o == null ? "$$$^^^" : (string)o;
			}
			set
			{
				ViewState["Separator"] = value;
			}
		}

		#endregion

		#region AutoPostBack (bool)

		/// <summary>
		/// Gets or sets a value indicating whether [auto post back].
		/// </summary>
		/// <value><c>true</c> if [auto post back]; otherwise, <c>false</c>.</value>
		[Browsable( true )
		, DefaultValue( false )
		, Category( "Configuration" )]
		public bool AutoPostBack
		{
			get
			{
				object o = ViewState["AutoPostBack"];
				return o == null ? false : (bool)o;
			}
			set
			{
				ViewState["AutoPostBack"] = value;
			}
		}

		#endregion

		#region EnableParentNodeSelection (bool)

		/// <summary>
		/// Gets or sets a value indicating whether parent node is selectable.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [enable parent node selection]; otherwise, <c>false</c>.
		/// </value>
		[Browsable( true )
		, DefaultValue( true )
		, Category( "Configuration" )]
		public bool EnableParentNodeSelection
		{
			get
			{
				object o = ViewState["EnableParentNodeSelection"];
				return o == null ? true : (bool)o;
			}
			set
			{
				ViewState["EnableParentNodeSelection"] = value;
			}
		}

		#endregion

		#region EnableParentNodeExpand (bool)

		/// <summary>
		/// Gets or sets a value indicating whether expand the node if click on it.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [enable parent node expand]; otherwise, <c>false</c>.
		/// </value>
		[Browsable( true )
		, DefaultValue( true )
		, Category( "Configuration" )]
		public bool EnableParentNodeExpand
		{
			get
			{
				object o = ViewState["EnableParentNodeExpand"];
				return o == null ? false : (bool)o;
			}
			set
			{
				ViewState["EnableParentNodeExpand"] = value;
			}
		}

		#endregion

		#region OnNodeSelectedScript (string)

		/// <summary>
		/// Gets or sets the on node selected script.
		/// Usage:
		/// 
		/// OnNodeSelectedScript="nodeSelectHandler(elem);"
		/// 
		/// function nodeSelectHandler(elem){
		///		var val = "selected node:" + elem.parentNode.getAttribute("treeNodeValue");
		///		alert( val );
		/// }
		/// </summary>
		/// <value>The on node selected script.</value>
		[Browsable( true )
		, DefaultValue( "" )
		, Category( "Configuration" )]
		public string OnNodeSelectedScript
		{
			get
			{
				object o = ViewState["OnNodeSelectedScript"];
				return o == null ? "" : (string)o;
			}
			set
			{
				ViewState["OnNodeSelectedScript"] = value;
			}
		}

		#endregion

		#region OnNodeCheckedScript (string)

		/// <summary>
		/// Gets or sets the on node checked script.
		/// Usage:
		/// 
		/// OnNodeCheckedScript="nodeCheckHandler(elem);"
		/// 
		/// function nodeCheckHandler(elem){
		///		var cs = elem.parentNode.getAttribute("checkedState");
		///		var csStr = "";
		///		switch(cs){
		///			case "0":
		///			csStr = "checked";
		///			break;
		///			case "1":
		///			csStr = "half checked";
		///			break;
		///			case "2":
		///			csStr = "unchecked";
		///			break;
		///		}
		///		var val = csStr +" node:" + elem.parentNode.getAttribute("treeNodeValue");
		///		alert( val );
		/// }
		/// </summary>
		/// <value>The on node checked script.</value>
		[Browsable( true )
		, DefaultValue( "" )
		, Category( "Configuration" )]
		public string OnNodeCheckedScript
		{
			get
			{
				object o = ViewState["OnNodeCheckedScript"];
				return o == null ? "" : (string)o;
			}
			set
			{
				ViewState["OnNodeCheckedScript"] = value;
			}
		}

		#endregion

		#region OnNodeDragAndDropStartScript (string)

		/// <summary>
		/// Gets or sets the on node drag and drop start script.
		/// Usage:
		/// 
		/// OnNodeDragAndDropStartScript="dndStartHandler(elem);"
		/// 
		/// function dndStartHandler( elem ){
		///		alert( elem.getAttribute("treeNodeValue") );
		/// }
		/// </summary>
		/// <value>The on node drag and drop start script.</value>
		[Browsable( true )
		, DefaultValue( "" )
		, Category( "Configuration" )]
		public string OnNodeDragAndDropStartScript
		{
			get
			{
				object o = ViewState["OnNodeDragAndDropStartScript"];
				return o == null ? "" : (string)o;
			}
			set
			{
				ViewState["OnNodeDragAndDropStartScript"] = value;
			}
		}

		#endregion

		#region EnableOnNodeDragAndDropStartScriptReturn (bool)

		/// <summary>
		/// Gets or sets a value indicating whether to add 'return' before OnNodeDragAndDropStartScript.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [enable on node drag and drop start script return]; otherwise, <c>false</c>.
		/// </value>
		[Browsable( true )
		, DefaultValue( false )
		, Category( "Configuration" )]
		public bool EnableOnNodeDragAndDropStartScriptReturn
		{
			get
			{
				object o = ViewState["EnableOnNodeDragAndDropStartScriptReturn"];
				return o == null ? false : (bool)o;
			}
			set
			{
				ViewState["EnableOnNodeDragAndDropStartScriptReturn"] = value;
			}
		}

		#endregion

		#region OnNodeDragAndDropCompletingScript (string)

		/// <summary>
		/// Gets or sets the on node drag and drop completing script.
		/// Usage:
		/// 
		/// OnNodeDragAndDropCompletingScript="dndCompletingHandler(elem);"
		/// 
		/// function dndCompletingHandler( elem ){
		///		alert( elem.getAttribute("treeNodeValue") );
		/// }
		/// </summary>
		/// <value>The on node drag and drop completing script.</value>
		[Browsable( true )
		, DefaultValue( "" )
		, Category( "Configuration" )]
		public string OnNodeDragAndDropCompletingScript
		{
			get
			{
				object o = ViewState["OnNodeDragAndDropCompletingScript"];
				return o == null ? "" : (string)o;
			}
			set
			{
				ViewState["OnNodeDragAndDropCompletingScript"] = value;
			}
		}

		#endregion

		#region EnableOnNodeDragAndDropCompletingScriptReturn (bool)

		/// <summary>
		/// Gets or sets a value indicating whether to add 'return' before OnNodeDragAndDropCompletingScript
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [enable on node drag and drop complete script return]; otherwise, <c>false</c>.
		/// </value>
		[Browsable( true )
		, DefaultValue( false )
		, Category( "Configuration" )]
		public bool EnableOnNodeDragAndDropCompletingScriptReturn
		{
			get
			{
				object o = ViewState["EnableOnNodeDragAndDropCompletingScriptReturn"];
				return o == null ? false : (bool)o;
			}
			set
			{
				ViewState["EnableOnNodeDragAndDropCompletingScriptReturn"] = value;
			}
		}

		#endregion

		#region OnNodeDragAndDropCompletedScript

		/// <summary>
		/// Gets or sets the on node drag and drop completed script.
		/// OnNodeDragAndDropCompletedScript="dndCompletedHandler(elem);"
		/// 
		/// function dndCompletedHandler( elem ){
		///		alert( elem.getAttribute("treeNodeValue") );
		/// }
		/// </summary>
		/// <value>The on node drag and drop completed script.</value>
		[Browsable( true )
		, DefaultValue( "" )
		, Category( "Configuration" )]
		public string OnNodeDragAndDropCompletedScript
		{
			get
			{
				object o = ViewState["OnNodeDragAndDropCompletedScript"];
				return o == null ? "" : (string)o;
			}
			set
			{
				ViewState["OnNodeDragAndDropCompletedScript"] = value;
			}
		}

		#endregion

		#region OnNodeAddedScript (string)

		/// <summary>
		/// Gets or sets the on node added script.
		/// Usage:
		/// 
		/// OnNodeAddedScript="addedHandler(elem);"
		/// 
		/// function addedHandler( elem ){
		///		alert( elem.getAttribute("treeNodeValue") );
		/// }
		/// </summary>
		/// <value>The on node added script.</value>
		[Browsable( true )
		, DefaultValue( "" )
		, Category( "Configuration" )]
		public string OnNodeAddedScript
		{
			get
			{
				object o = ViewState["OnNodeAddedScript"];
				return o == null ? "" : (string)o;
			}
			set
			{
				ViewState["OnNodeAddedScript"] = value;
			}
		}

		#endregion

		#region EnableOnNodeAddedScriptReturn (bool)

		/// <summary>
		/// EnableOnNodeAddedScriptReturn
		/// </summary>
		[Browsable( true )
		, DefaultValue( false )
		, Category( "Configuration" )]
		public bool EnableOnNodeAddedScriptReturn
		{
			get
			{
				object o = ViewState["EnableOnNodeAddedScriptReturn"];
				return o == null ? false : (bool)o;
			}
			set
			{
				ViewState["EnableOnNodeAddedScriptReturn"] = value;
			}
		}

		#endregion

		#region OnNodeEditedScript (string)

		/// <summary>
		/// Gets or sets the on node edited script.
		/// Usage:
		/// 
		/// OnNodeEditedScript="editedHandler(elem);"
		/// 
		/// function editedHandler( elem ){
		///		alert( elem.getAttribute("treeNodeValue") );
		/// }
		/// </summary>
		/// <value>The on node edited script.</value>
		[Browsable( true )
		, DefaultValue( "" )
		, Category( "Configuration" )]
		public string OnNodeEditedScript
		{
			get
			{
				object o = ViewState["OnNodeEditedScript"];
				return o == null ? "" : (string)o;
			}
			set
			{
				ViewState["OnNodeEditedScript"] = value;
			}
		}

		#endregion

		#region EnableOnNodeEditedScriptReturn (bool)

		/// <summary>
		/// EnableOnNodeEditedScriptReturn
		/// </summary>
		[Browsable( true )
		, DefaultValue( false )
		, Category( "Configuration" )]
		public bool EnableOnNodeEditedScriptReturn
		{
			get
			{
				object o = ViewState["EnableOnNodeEditedScriptReturn"];
				return o == null ? false : (bool)o;
			}
			set
			{
				ViewState["EnableOnNodeEditedScriptReturn"] = value;
			}
		}

		#endregion

		#region OnNodeDeletedScript (string)

		/// <summary>
		/// Gets or sets the on node deleted script.
		/// Usage:
		/// 
		/// OnNodeDeletedScript="deletedHandler( val );"
		/// 
		/// function deletedHandler( val ){
		///		alert( val );
		/// }
		/// </summary>
		/// <value>The on node deleted script.</value>
		[Browsable( true )
		, DefaultValue( "" )
		, Category( "Configuration" )]
		public string OnNodeDeletedScript
		{
			get
			{
				object o = ViewState["OnNodeDeletedScript"];
				return o == null ? "" : (string)o;
			}
			set
			{
				ViewState["OnNodeDeletedScript"] = value;
			}
		}

		#endregion

		#region EnableOnNodeDeletedScriptReturn (bool)

		/// <summary>
		/// EnableOnNodeDeletedScriptReturn
		/// </summary>
		[Browsable( true )
		, DefaultValue( false )
		, Category( "Configuration" )]
		public bool EnableOnNodeDeletedScriptReturn
		{
			get
			{
				object o = ViewState["EnableOnNodeDeletedScriptReturn"];
				return o == null ? false : (bool)o;
			}
			set
			{
				ViewState["EnableOnNodeDeletedScriptReturn"] = value;
			}
		}

		#endregion

		#region EnableOnNodeOpenedAndClosedReturn (bool)

		/// <summary>
		/// Gets or sets a value indicating whether [enable on node opened and closed return].
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [enable on node opened and closed return]; otherwise, <c>false</c>.
		/// </value>
		[Browsable( true )
		, DefaultValue( false )
		, Category( "Configuration" )]
		public bool EnableOnNodeOpenedAndClosedReturn
		{
			get
			{
				object o = ViewState["EnableOnNodeOpenedAndClosedReturn"];
				return o == null ? false : (bool)o;
			}
			set
			{
				ViewState["EnableOnNodeOpenedAndClosedReturn"] = value;
			}
		}

		#endregion

		#region EnableNodeSelection (bool)

		/// <summary>
		/// Gets or sets a value indicating whether the node is selectable.
		/// </summary>
		/// <value><c>true</c> if [enable node selection]; otherwise, <c>false</c>.</value>
		[Browsable( true )
		, DefaultValue( true )
		, Category( "Configuration" )]
		public bool EnableNodeSelection
		{
			get
			{
				object o = ViewState["EnableNodeSelection"];
				return o == null ? true : (bool)o;
			}
			set
			{
				ViewState["EnableNodeSelection"] = value;
			}
		}

		#endregion

		#region EnableDragDrop (bool)

		/// <summary>
		/// Gets or sets a value indicating whether the end user and drag and drop nodes.
		/// </summary>
		/// <value><c>true</c> if [enable drag drop]; otherwise, <c>false</c>.</value>
		[Browsable( true )
		, DefaultValue( true )
		, Category( "Configuration" )]
		public bool EnableDragDrop
		{
			get
			{
				object o = ViewState["EnableDragDrop"];
				return o == null ? true : (bool)o;
			}
			set
			{
				ViewState["EnableDragDrop"] = value;
			}
		}

		#endregion

		#region EnableDragDropOnIcon (bool)

		/// <summary>
		/// EnableDragDropOnIcon
		/// </summary>
		[Browsable( true )
	    , DefaultValue( false )
		, Category( "Configuration" )]
		public bool EnableDragDropOnIcon
		{
			get
			{
				object o = ViewState["EnableDragDropOnIcon"];
				return o == null ? false : (bool)o;
			}
			set
			{
				ViewState["EnableDragDropOnIcon"] = value;
			}
		}

		#endregion

		#region RelatedTrees (string)

		/// <summary>
		/// Gets or sets the related trees. End user and drag drop the nodes from current tree to the related trees.
		/// </summary>
		/// <value>The related trees.</value>
		[Browsable( true )
		, DefaultValue( "" )
		, Category( "Configuration" )]
		public string RelatedTrees
		{
			get
			{
				object o = ViewState["RelatedTrees"];
				return o == null ? "" : (string)o;
			}
			set
			{
				ViewState["RelatedTrees"] = value;
			}
		}

		#endregion

		#region EnableSaveStateEveryStep (bool)

		/// <summary>
		/// Gets or sets a value indicating whether client side should save the whole tree state on every change.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [enable save state every step]; otherwise, <c>false</c>.
		/// </value>
		[Browsable( true )
		, DefaultValue( false )
		, Category( "Configuration" )]
		public bool EnableSaveStateEveryStep
		{
			get
			{
				object o = ViewState["EnableSaveStateEveryStep"];
				return o == null ? false : (bool)o;
			}
			set
			{
				ViewState["EnableSaveStateEveryStep"] = value;
			}
		}

		#endregion

		#region EnableNodeIcon (bool)

		/// <summary>
		/// Gets or sets a value indicating whether to display node icons.
		/// </summary>
		/// <value><c>true</c> if [enable node icon]; otherwise, <c>false</c>.</value>
		[Browsable( true )
		, DefaultValue( true )
		, Category( "Node Icon" )]
		public bool EnableNodeIcon
		{
			get
			{
				object o = ViewState["EnableNodeIcon"];
				return o == null ? true : (bool)o;
			}
			set
			{
				ViewState["EnableNodeIcon"] = value;
			}
		}

		#endregion

		#region EnableCustomizedNodeIcon (bool)

		/// <summary>
		/// Gets or sets a value indicating whether to use the customized node icons.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [enable customized node icon]; otherwise, <c>false</c>.
		/// </value>
		[Browsable( true )
		, DefaultValue( false )
		, Category( "Node Icon" )]
		public bool EnableCustomizedNodeIcon
		{
			get
			{
				object o = ViewState["EnableCustomizedNodeIcon"];
				return o == null ? false : (bool)o;
			}
			set
			{
				ViewState["EnableCustomizedNodeIcon"] = value;
			}
		}

		#endregion

		#region EnableRightToLeftRender (bool)

		/// <summary>
		/// Gets or sets a value indicating whether to render the tree node from right to left.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [enable right to left render]; otherwise, <c>false</c>.
		/// </value>
		[Browsable( true )
		, DefaultValue( false )
		, Category( "Configuration" )]
		public bool EnableRightToLeftRender
		{
			get
			{
				object o = ViewState["EnableRightToLeftRender"];
				return o == null ? false : (bool)o;
			}
			set
			{
				ViewState["EnableRightToLeftRender"] = value;
			}
		}

		#endregion

		#region EnableManageJSError (bool)

		/// <summary>
		/// Gets or sets a value indicating whether the astreeview should manage the javascript error cause by onXXX script properties.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [enable manage JS error]; otherwise, <c>false</c>.
		/// </value>
		[Browsable( true )
		, DefaultValue( false )
		, Category( "Configuration" )]
		public bool EnableManageJSError
		{
			get
			{
				object o = ViewState["EnableManageJSError"];
				return o == null ? true : (bool)o;
			}
			set
			{
				ViewState["EnableManageJSError"] = value;
			}
		}

		#endregion

		#region EnableMultiLineEdit (bool)

		/// <summary>
		/// Gets or sets a value indicating whether to display a textarea when editing nodes.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [enable multi line edit]; otherwise, <c>false</c>.
		/// </value>
		[Browsable( true )
		, DefaultValue( false )
		, Category( "Configuration" )]
		public bool EnableMultiLineEdit
		{
			get
			{
				object o = ViewState["EnableMultiLineEdit"];
				return o == null ? false : (bool)o;
			}
			set
			{
				ViewState["EnableMultiLineEdit"] = value;
			}
		}

		#endregion

		#region EnableEscapeInput (bool)

		/// <summary>
		/// Gets or sets a value indicating whether to escape the html tag for node editing.
		/// </summary>
		/// <value><c>true</c> if [enable escape input]; otherwise, <c>false</c>.</value>
		[Browsable( true )
		, DefaultValue( true )
		, Category( "Configuration" )]
		public bool EnableEscapeInput
		{
			get
			{
				object o = ViewState["EnableEscapeInput"];
				return o == null ? true : (bool)o;
			}
			set
			{
				ViewState["EnableEscapeInput"] = value;
			}
		}

		#endregion

		#region EnableStripAjaxResponse (bool)

		/// <summary>
		/// Gets or sets a value indicating whether to stripe ajax response, that is to get the partial of the ajax response. This can be useful when to use the astreeview in a custom server side control.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [enable strip ajax response]; otherwise, <c>false</c>.
		/// </value>
		[Browsable( true )
		, DefaultValue( false )
		, Category( "Configuration" )]
		public bool EnableStripAjaxResponse
		{
			get
			{
				object o = ViewState["EnableStripAjaxResponse"];
				return o == null ? false : (bool)o;
			}
			set
			{
				ViewState["EnableStripAjaxResponse"] = value;
			}
		}

		#endregion

		#region StripAjaxResponseRegex (string)

		/// <summary>
		/// Gets or sets the regular expression for ajax response striping. 
		/// </summary>
		/// <value>The strip ajax response regex.</value>
		[Browsable( true )
		, DefaultValue( "<!--ast_ajax_start-->(.|\\\\\\s)*?<!--ast_ajax_end-->" )
		, Category( "Configuration" )]
		public string StripAjaxResponseRegex
		{
			get
			{
				object o = ViewState["StripAjaxResponseRegex"];
				return o == null ? "<!--ast_ajax_start-->(.|\\\\\\s)*?<!--ast_ajax_end-->" : (string)o;
			}
			set
			{
				ViewState["StripAjaxResponseRegex"] = value;
			}
		}

		#endregion

		#region AjaxResponseStartTag (string)

		/// <summary>
		/// Gets or sets the ajax response start tag.
		/// </summary>
		/// <value>The ajax response start tag.</value>
		[Browsable( true )
		, DefaultValue( "<!--ast_ajax_start-->" )
		, Category( "Configuration" )]
		public string AjaxResponseStartTag
		{
			get
			{
				object o = ViewState["AjaxResponseStartTag"];
				return o == null ? "<!--ast_ajax_start-->" : (string)o;
			}
			set
			{
				ViewState["AjaxResponseStartTag"] = value;
			}
		}

		#endregion

		#region AjaxResponseEndTag (string)

		/// <summary>
		/// Gets or sets the ajax response end tag.
		/// </summary>
		/// <value>The ajax response end tag.</value>
		[Browsable( true )
		, DefaultValue( "<!--ast_ajax_end-->" )
		, Category( "Configuration" )]
		public string AjaxResponseEndTag
		{
			get
			{
				object o = ViewState["AjaxResponseEndTag"];
				return o == null ? "<!--ast_ajax_end-->" : (string)o;
			}
			set
			{
				ViewState["AjaxResponseEndTag"] = value;
			}
		}

		#endregion

		#region EnableTreeNodesViewState (bool)

		/// <summary>
		/// Gets or sets a value indicating whether to whole treeview state should be save in viewstate, if you want a lower html size rendering to client, turn it off, but it may cause lose of treenodes if the user perform a postback while the page is not fully loaded.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if EnableTreeNodesViewState; otherwise, <c>false</c>.
		/// </value>
		[Browsable( true )
		, DefaultValue( true )
		, Category( "Configuration" )]
		public bool EnableTreeNodesViewState
		{
			get
			{
				object o = ViewState["EnableTreeNodesViewState"];
				return o == null ? true : (bool)o;
			}
			set
			{
				ViewState["EnableTreeNodesViewState"] = value;
			}
		}

		#endregion
		
		#region EnablePersistentTreeNodesOnFirstLoad (bool)
		/*
		/// <summary>
		/// Gets or sets a value indicating whether to serialize the treeview states to a hidden on the first load of the page.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if EnablePersistentTreeNodesOnFirstLoad; otherwise, <c>false</c>.
		/// </value>
		[Browsable( true )
		, DefaultValue( false )
		, Category( "Configuration" )]
		public bool EnablePersistentTreeNodesOnFirstLoad
		{
			get
			{
				object o = ViewState["EnablePersistentTreeNodesOnFirstLoad"];
				return o == null ? false : (bool)o;
			}
			set
			{
				ViewState["EnablePersistentTreeNodesOnFirstLoad"] = value;
			}
		}
		*/
		#endregion

		#region EnableFixedDepthDragDrop (bool)

		/// <summary>
		/// Gets or sets a value indicating whether the nodes can be dragged and dropped only it the depth which is same as the nodes'. Please note if the EnableFixedParentDragDrop is true, EnableFixedDepthDragDrop is true.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [enable fixed depth drag drop]; otherwise, <c>false</c>.
		/// </value>
		[Browsable( true )
		, DefaultValue( false )
		, Category( "Configuration" )]
		public bool EnableFixedDepthDragDrop
		{
			get
			{
				if( this.EnableFixedParentDragDrop )
					return true;

				object o = ViewState["EnableFixedDepthDragDrop"];
				return o == null ? false : (bool)o;
			}
			set
			{
				ViewState["EnableFixedDepthDragDrop"] = value;
			}
		}

		#endregion

		#region EnableFixedParentDragDrop (bool)

		/// <summary>
		/// Gets or sets a value indicating whether the nodes are only can be drag drop under their parent.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [enable fixed parent drag drop]; otherwise, <c>false</c>.
		/// </value>
		[Browsable( true )
		, DefaultValue( false )
		, Category( "Configuration" )]
		public bool EnableFixedParentDragDrop
		{
			get
			{
				object o = ViewState["EnableFixedParentDragDrop"];
				return o == null ? false : (bool)o;
			}
			set
			{
				ViewState["EnableFixedParentDragDrop"] = value;
			}
		}

		#endregion

		#region EnableHorizontalLock (bool)

		/// <summary>
		/// Gets or sets a value indicating whether to lock horizontal movement.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [enable horizontal lock]; otherwise, <c>false</c>.
		/// </value>
		[Browsable( true )
		, DefaultValue( false )
		, Category( "Configuration" )]
		public bool EnableHorizontalLock
		{
			get
			{
				object o = ViewState["EnableHorizontalLock"];
				return o == null ? false : (bool)o;
			}
			set
			{
				ViewState["EnableHorizontalLock"] = value;
			}
		}

		#endregion

		#region EnableContainerDragDrop (bool)

		/// <summary>
		/// EnableContainerDragDrop
		/// </summary>
		[Browsable( true )
		, DefaultValue( false )
		, Category( "Configuration" )]
		public bool EnableContainerDragDrop
		{
			get
			{
				object o = ViewState["EnableContainerDragDrop"];
				return o == null ? false : (bool)o;
			}
			set
			{
				ViewState["EnableContainerDragDrop"] = value;
			}
		}

		#endregion

		#region EnableThreeStateCheckbox (bool)

		/// <summary>
		/// Gets or sets a value indicating whether to enable three state checkbox. Please note that if EnableLeafOnlyCheckbox=true, then EnableThreeStateCheckbox will automatically be false;
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [enable three state checkbox]; otherwise, <c>false</c>.
		/// </value>
		[Browsable( true )
		, DefaultValue( true )
		, Category( "Configuration" )]
		public bool EnableThreeStateCheckbox
		{
			get
			{
				if( this.EnableLeafOnlyCheckbox )
					return false;

				object o = ViewState["EnableThreeStateCheckbox"];
				return o == null ? true : (bool)o;
			}
			set
			{
				ViewState["EnableThreeStateCheckbox"] = value;
			}
		}

		#endregion

		#region EnableLeafOnlyCheckbox (bool)

		/// <summary>
		/// Gets or sets a value indicating whether to enable leaf only checkbox.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [enable leaf only checkbox]; otherwise, <c>false</c>.
		/// </value>
		[Browsable( true )
		, DefaultValue( false )
		, Category( "Configuration" )]
		public bool EnableLeafOnlyCheckbox
		{
			get
			{
				object o = ViewState["EnableLeafOnlyCheckbox"];
				return o == null ? false : (bool)o;
			}
			set
			{
				ViewState["EnableLeafOnlyCheckbox"] = value;
			}
		}

		#endregion

		#region EnablePersistentTreeState (bool)

		/// <summary>
		/// Gets or sets a value indicating whether to submit the whole treeview state, values on postback. If you want to regenerate treeview on every postback, you may set it false. Generally it should be true to keep the treeview state on postback.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [enable persistent tree state]; otherwise, <c>false</c>.
		/// </value>
		[Browsable( true )
		, DefaultValue( true )
		, Category( "Configuration" )]
		public bool EnablePersistentTreeState
		{
			get
			{
				object o = ViewState["EnablePersistentTreeState"];
				return o == null ? true : (bool)o;
			}
			set
			{
				ViewState["EnablePersistentTreeState"] = value;
			}
		}

		#endregion

		#region OnNodeOpenedAndClosedScript (string)



		/// <summary>
		/// Gets or sets the on node opened and closed script.
		/// Usage:
		/// 
		/// OnNodeOpenedAndClosedScript="nodeOpenedAndClosedHandler( state, elem );"
		/// 
		/// function nodeOpenedAndClosedHandler( state, elem ){
		///		alert( state );
		///		alert( elem );
		/// }
		/// </summary>
		/// <value>The on node opened and closed script.</value>
		[Browsable( true )
		, DefaultValue( "" )
		, Category( "Configuration" )]
		public string OnNodeOpenedAndClosedScript
		{
			get
			{
				object o = ViewState["OnNodeOpenedAndClosedScript"];
				return o == null ? "" : (string)o;
			}
			set
			{
				ViewState["OnNodeOpenedAndClosedScript"] = value;
			}
		}

		#endregion

		#region EnableXmlValidation (bool)

		/// <summary>
		/// Gets or sets a value indicating whether to enable xml validation.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if xml validation enabled; otherwise, <c>false</c>.
		/// </value>
		[Browsable( true )
		, DefaultValue( true )
		, Category( "Configuration" )]
		public bool EnableXmlValidation
		{
			get
			{
				object o = ViewState["EnableXmlValidation"];
				return o == null ? true : (bool)o;
			}
			set
			{
				ViewState["EnableXmlValidation"] = value;
			}
		}

		#endregion

		#endregion

		#region Data

		#region DataTableColumnDescriptor (ASTreeViewDataTableColumnDescriptor)

		/// <summary>
		/// Gets or sets the data source descriptor. Datasource descriptor converts datasource to treeview nodes.
		/// </summary>
		/// <value>The data source descriptor.</value>
		[Browsable( false )
		, Category( "Data" )]
		public ASTreeViewDataSourceDescriptor DataSourceDescriptor
		{
			get
			{
				object o = ViewState["DataSourceDescriptor"];
				return o == null ? null : (ASTreeViewDataSourceDescriptor)o;
			}
			set
			{
				ViewState["DataSourceDescriptor"] = value;
			}
		}

		#endregion

		#region DataSource (object)

		object dataSource;

		/// <summary>
		/// Gets or sets the data source. 
		/// </summary>
		/// <value>The data source.</value>
		[Browsable( false )
		, Category( "Data" )]
		public object DataSource
		{
			get
			{
				return this.dataSource;
			}
			set
			{
				this.dataSource = value;
			}
		}

		#endregion

		#region DataTableRootNodeValue (string)

		/// <summary>
		/// Gets or sets the data table root node value. When you set a datatable as datasource to astreeview, the astreeview need to know from which line of data it should start to convert to tree nodes.
		/// </summary>
		/// <value>The data table root node value.</value>
		[Browsable( false )
		, Category( "Data" )]
		public string DataTableRootNodeValue
		{
			get
			{
				object o = ViewState["DataTableRootNodeValue"];
				return o == null ? null : (string)o;
			}
			set
			{
				ViewState["DataTableRootNodeValue"] = value;
			}
		}

		#endregion

		#region TreeNodesState

		/// <summary>
		/// Gets or sets the state of the tree nodes in viewstate.
		/// </summary>
		/// <value>The state of the tree nodes.</value>
		virtual protected string TreeNodesState
		{
			get
			{
				object o = ViewState["TreeNodesState"];
				return o == null ? string.Empty : (string)o;
			}
			set
			{
				ViewState["TreeNodesState"] = value;
			}
		}

		#endregion

		#region IsBound

		/// <summary>
		/// Gets or sets a value indicating whether this instance is bound.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is bound; otherwise, <c>false</c>.
		/// </value>
		virtual public bool IsBound
		{
			get
			{
				object o = ViewState["IsBound"];
				return o == null ? true : (bool)o;
			}
			set
			{
				ViewState["IsBound"] = value;
			}
		}

		#endregion

		#endregion

		#region RootNode

		#region EnableRoot (bool)

		/// <summary>
		/// Gets or sets a value indicating whether to display a node as the root root node of the tree.
		/// </summary>
		/// <value><c>true</c> if [enable root]; otherwise, <c>false</c>.</value>
		[Browsable( true )
		, DefaultValue( true )
		, Category( "Configuration" )]
		public bool EnableRoot
		{
			get
			{
				object o = ViewState["EnableRoot"];
				return o == null ? true : (bool)o;
			}
			set
			{
				ViewState["EnableRoot"] = value;
			}
		}

		#endregion

		#region RootNode object

		/// <summary>
		/// Gets or sets the root node.
		/// </summary>
		/// <value>The root node.</value>
		[Browsable( false ),
		Category( "Configuration" )]
		virtual public ASTreeViewNode RootNode
		{
			get
			{
				return rootNode;
			}
			//set
			//{
				//rootNode = value;//ViewState["RootNode"] = value;
			//}
		}

		#endregion

		#region RootNodeText (string)

		/// <summary>
		/// Gets or sets the root node text.
		/// </summary>
		/// <value>The root node text.</value>
		[Browsable( true )
		, DefaultValue( "Root" )
		, Category( "Configuration" )]
		public string RootNodeText
		{
			get
			{
				object o = ViewState["RootNodeText"];
				return o == null ? "Root" : (string)o;
			}
			set
			{
				ViewState["RootNodeText"] = value;
			}
		}

		#endregion

		#region RootNodeValue (string)

		/// <summary>
		/// Gets or sets the root node value.
		/// </summary>
		/// <value>The root node value.</value>
		[Browsable( true )
		, DefaultValue( "root" )
		, Category( "Configuration" )]
		public string RootNodeValue
		{
			get
			{
				object o = ViewState["RootNodeValue"];
				return o == null ? "root" : (string)o;
			}
			set
			{
				ViewState["RootNodeValue"] = value;
			}
		}

		#endregion

		#endregion

		#region TreeViewHelper

		/// <summary>
		/// Gets the tree view helper.
		/// </summary>
		/// <value>The tree view helper.</value>
		public ASTreeViewHelper TreeViewHelper
		{
			get { return this.treeViewHelper; }
		}


		#endregion

		#region Checkbox

		#region EnableCheckbox (bool)

		/// <summary>
		/// Gets or sets a value indicating whether to display check box.
		/// </summary>
		/// <value><c>true</c> if [enable checkbox]; otherwise, <c>false</c>.</value>
		[Browsable( true )
		, DefaultValue( true )
	   , Category( "Checkbox" )]
		public bool EnableCheckbox
		{
			get
			{
				object o = ViewState["EnableCheckbox"];
				return o == null ? true : (bool)o;
			}
			set
			{
				ViewState["EnableCheckbox"] = value;
			}
		}

		#endregion

		#region ImgCheckboxChecked (string)

		/// <summary>
		/// Gets or sets the checkbox checked icon.
		/// </summary>
		/// <value>The img checkbox checked.</value>
		[Browsable( true )
		, DefaultValue( "astreeview-checkbox-checked.gif" )
		, Category( "Checkbox" )]
		public string ImgCheckboxChecked
		{
			get
			{
				if( this.EnableTheme )
					return this.Page.ResolveUrl( this.Theme.ImgCheckboxChecked );

				object o = ViewState["ImgCheckboxChecked"];
				return o == null ? this.ImagePath + "astreeview-checkbox-checked.gif" : this.ProcessImageUrl( (string)o );
			}
			set
			{
				ViewState["ImgCheckboxChecked"] = value;
			}
		}

		#endregion

		#region ImgCheckboxHalfChecked (string)

		/// <summary>
		/// Gets or sets the checkbox half-checked icon.
		/// </summary>
		/// <value>The img checkbox half checked.</value>
		[Browsable( true )
		, DefaultValue( "astreeview-checkbox-half-checked.gif" )
		, Category( "Checkbox" )]
		public string ImgCheckboxHalfChecked
		{
			get
			{
				if( this.EnableTheme )
					return this.Page.ResolveUrl( this.Theme.ImgCheckboxHalfChecked );

				object o = ViewState["ImgCheckboxHalfChecked"];
				return o == null ? this.ImagePath + "astreeview-checkbox-half-checked.gif" : this.ProcessImageUrl( (string)o );
			}
			set
			{
				ViewState["ImgCheckboxHalfChecked"] = value;
			}
		}

		#endregion

		#region ImgCheckboxUnchecked (string)

		/// <summary>
		/// Gets or sets the checkbox unchecked icon.
		/// </summary>
		/// <value>The img checkbox unchecked.</value>
		[Browsable( true )
		, DefaultValue( "astreeview-checkbox-unchecked.gif" )
		, Category( "Checkbox" )]
		public string ImgCheckboxUnchecked
		{
			get
			{
				if( this.EnableTheme )
					return this.Page.ResolveUrl( this.Theme.ImgCheckboxUnchecked );

				object o = ViewState["ImgCheckboxUnchecked"];
				return o == null ? this.ImagePath + "astreeview-checkbox-unchecked.gif" : this.ProcessImageUrl( (string)o );
			}
			set
			{
				ViewState["ImgCheckboxUnchecked"] = value;
			}
		}

		#endregion

		#region CssClassCheckbox (string)

		/// <summary>
		/// Gets or sets the CSS class of checkbox.
		/// </summary>
		/// <value>The CSS class checkbox.</value>
		[Browsable( true )
		, DefaultValue( "astreeview-checkbox" )
		, Category( "Checkbox" )]
		public string CssClassCheckbox
		{
			get
			{
				if( this.EnableTheme )
					return this.Theme.CssClassCheckbox;

				object o = ViewState["CssClassCheckbox"];
				return o == null ? "astreeview-checkbox" : (string)o;
			}
			set
			{
				ViewState["CssClassCheckbox"] = value;
			}
		}

		#endregion

		#region CssClassCheckboxTextNode (string)

		/// <summary>
		/// CssClassCheckboxTextNode
		/// </summary>
		[Browsable( true )
		, DefaultValue( "astreeview-checkbox-text" )
		, Category( "Checkbox" )]
		public string CssClassCheckboxTextNode
		{
			get
			{
				object o = ViewState["CssClassCheckboxTextNode"];
				return o == null ? "astreeview-checkbox-text" : (string)o;
			}
			set
			{
				ViewState["CssClassCheckboxTextNode"] = value;
			}
		}

		#endregion

		#endregion

		#region Theme

		#region EnableTheme (bool)

		/// <summary>
		/// Gets or sets a value indicating whether to use theme.
		/// </summary>
		/// <value><c>true</c> if [enable theme]; otherwise, <c>false</c>.</value>
		[Browsable( true )
		, DefaultValue( true )
		, Category( "Theme" )]
		public bool EnableTheme
		{
			get
			{
				object o = ViewState["EnableTheme"];
				return o == null ? false : (bool)o;
			}
			set
			{
				ViewState["EnableTheme"] = value;
			}
		}

		#endregion

		#region Theme (ASTreeViewTheme)

		/// <summary>
		/// Gets or sets the theme of the treeview.
		/// </summary>
		/// <value>The theme.</value>
		[Browsable( true )
		, Category( "Theme" )]
		public ASTreeViewTheme Theme
		{
			get
			{
				object o = ViewState["Theme"];
				return o == null ? new ASTreeViewTheme() : (ASTreeViewTheme)o;
			}
			set
			{
				ViewState["Theme"] = value;
			}
		}

		#endregion

		#region ThemeCssFile (string)

		/// <summary>
		/// Gets the theme CSS file.
		/// </summary>
		/// <value>The theme CSS file.</value>
		[Browsable( false )
		, Category( "Theme" )]
		public string ThemeCssFile
		{
			get
			{
				return this.Page.ResolveUrl( this.Theme.CssFile );
			}
		}

		#endregion

		#endregion

		#region Appearance

		#region CssClass (string)

		/// <summary>
		/// Gets or sets the Cascading Style Sheet (CSS) class rendered by the Web server control on the client.
		/// </summary>
		/// <value></value>
		/// <returns>The CSS class rendered by the Web server control on the client. The default is <see cref="F:System.String.Empty"></see>.</returns>
		[Browsable( true )
		, DefaultValue( "astreeview-tree" )
		, Category( "Appearance" )]
		public override string CssClass
		{
			get
			{
				if( this.EnableTheme )
					return this.Theme.CssClass;

				object o = ViewState["CssClass"];
				return o == null ? "astreeview-tree" : (string)o;
			}
			set
			{
				ViewState["CssClass"] = value;
			}
		}

		#endregion

		#region CssClassTextNode (string)

		/// <summary>
		/// CssClassTextNode
		/// </summary>
		[Browsable( true )
		, DefaultValue( "astreeview-text-node" )
		, Category( "Appearance" )]
		public string CssClassTextNode
		{
			get
			{
				object o = ViewState["CssClassTextNode"];
				return o == null ? "astreeview-text-node" : (string)o;
			}
			set
			{
				ViewState["CssClassTextNode"] = value;
			}
		}

		#endregion

		#region CssClassTextNodeContainer (string)

		/// <summary>
		/// CssClassTextNodeContainer
		/// </summary>
		[Browsable( true )
		, DefaultValue( "astreeview-text-node-container" )
		, Category( "Appearance" )]
		public string CssClassTextNodeContainer
		{
			get
			{
				object o = ViewState["CssClassTextNodeContainer"];
				return o == null ? "astreeview-text-node-container" : (string)o;
			}
			set
			{
				ViewState["CssClassTextNodeContainer"] = value;
			}
		}

		#endregion


		#endregion

		#region Folder & Item Icon

		#region DefaultFolderIcon (string)

		/// <summary>
		/// Gets or sets the default folder icon.
		/// </summary>
		/// <value>The default folder icon.</value>
		[Browsable( true )
		, DefaultValue( "astreeview-folder.gif" )
		, Category( "Icon" )]
		public string DefaultFolderIcon
		{
			get
			{
				if( this.EnableTheme )
					return this.Page.ResolveUrl( this.Theme.DefaultFolderIcon );

				object o = ViewState["DefaultFolderIcon"];
				return o == null ? this.ImagePath + "astreeview-folder.gif" : this.ProcessImageUrl( (string)o );
			}
			set
			{
				ViewState["DefaultFolderIcon"] = value;
			}
		}

		#endregion

		#region DefaultFolderOpenIcon (string)

		/// <summary>
		/// Gets or sets the default folder open icon.
		/// </summary>
		/// <value>The default folder open icon.</value>
		[Browsable( true )
		, DefaultValue( "astreeview-folder-open.gif" )
		, Category( "Icon" )]
		public string DefaultFolderOpenIcon
		{
			get
			{
				if( this.EnableTheme )
					return this.Page.ResolveUrl( Theme.DefaultFolderOpenIcon );

				object o = ViewState["DefaultFolderOpenIcon"];
				return o == null ? this.ImagePath + "astreeview-folder-open.gif" : this.ProcessImageUrl( (string)o );
			}
			set
			{
				ViewState["DefaultFolderOpenIcon"] = value;
			}
		}

		#endregion

		#region DefaultNodeIcon (string)

		/// <summary>
		/// Gets or sets the default node icon.
		/// </summary>
		/// <value>The default node icon.</value>
		[Browsable( true )
		, DefaultValue( "astreeview-node.gif" )
		, Category( "Icon" )]
		public string DefaultNodeIcon
		{
			get
			{
				if( this.EnableTheme )
					return this.Page.ResolveUrl( Theme.DefaultNodeIcon );

				object o = ViewState["DefaultNodeIcon"];
				return o == null ? this.ImagePath + "astreeview-node.gif" : this.ProcessImageUrl( (string)o );
			}
			set
			{
				ViewState["DefaultNodeIcon"] = value;
			}
		}

		#endregion

		#region CssClassIcon (string)

		/// <summary>
		/// Gets or sets the CSS class of the icons.
		/// </summary>
		/// <value>The CSS class icon.</value>
		[Browsable( true )
		, DefaultValue( "astreeview-icon" )
		, Category( "Icon" )]
		public string CssClassIcon
		{
			get
			{
				if( this.EnableTheme )
					return this.Theme.CssClassIcon;

				object o = ViewState["CssClassIcon"];
				return o == null ? "astreeview-icon" : (string)o;
			}
			set
			{
				ViewState["CssClassIcon"] = value;
			}
		}

		#endregion


		#endregion

		#region Plus & Minus Icon

		#region ImgPlusIcon (string)

		/// <summary>
		/// Gets or sets the open icon of the node.
		/// </summary>
		/// <value>The img plus icon.</value>
		[Browsable( true )
		, DefaultValue( "astreeview-plus.gif" )
		, Category( "Icon" )]
		public string ImgPlusIcon
		{
			get
			{
				if( this.EnableTheme )
					return this.Page.ResolveUrl( this.Theme.ImgPlusIcon );

				object o = ViewState["ImgPlusIcon"];
				return o == null ? this.ImagePath + "astreeview-plus.gif" : this.ProcessImageUrl( (string)o );
			}
			set
			{
				ViewState["ImgPlusIcon"] = value;
			}
		}

		#endregion

		#region ImgMinusIcon (string)

		/// <summary>
		/// Gets or sets the close icon of the node.
		/// </summary>
		/// <value>The img minus icon.</value>
		[Browsable( true )
		, DefaultValue( "astreeview-minus.gif" )
		, Category( "Icon" )]
		public string ImgMinusIcon
		{
			get
			{
				if( this.EnableTheme )
					return this.Page.ResolveUrl( this.Theme.ImgMinusIcon );

				object o = ViewState["ImgMinusIcon"];
				return o == null ? this.ImagePath + "astreeview-minus.gif" : this.ProcessImageUrl( (string)o );
			}
			set
			{
				ViewState["ImgMinusIcon"] = value;
			}
		}

		#endregion

		#region CssClassPlusMinus (string)

		/// <summary>
		/// Gets or sets the CSS class of the open and close icon of the tree node.
		/// </summary>
		/// <value>The CSS class plus minus icon.</value>
		[Browsable( true )
		, DefaultValue( "astreeview-plus-minus" )
		, Category( "Icon" )]
		public string CssClassPlusMinusIcon
		{
			get
			{
				if( this.EnableTheme )
					return this.Theme.CssClassPlusMinusIcon;

				object o = ViewState["CssClassPlusMinusIcon"];
				return o == null ? "astreeview-plus-minus" : (string)o;
			}
			set
			{
				ViewState["CssClassPlusMinusIcon"] = value;
			}
		}

		#endregion

		#region CssClassPlusMinusTextNode (string)

		/// <summary>
		/// CssClassPlusMinusTextNode
		/// </summary>
		[Browsable( true )
		, DefaultValue( "astreeview-plus-minus-text" )
		, Category( "Icon" )]
		public string CssClassPlusMinusTextNode
		{
			get
			{
				object o = ViewState["CssClassPlusMinusTextNode"];
				return o == null ? "astreeview-plus-minus-text" : (string)o;
			}
			set
			{
				ViewState["CssClassPlusMinusTextNode"] = value;
			}
		}

		#endregion

		#region ImgDragDropIndicator (string)

		/// <summary>
		/// Gets or sets the drag drop indicator icon.
		/// </summary>
		/// <value>The img drag drop indicator.</value>
		[Browsable( true )
		, DefaultValue( "astreeview-dragDrop-indicator1.gif" )
		, Category( "Icon" )]
		public string ImgDragDropIndicator
		{
			get
			{
				if( this.EnableTheme )
					return this.Page.ResolveUrl( this.Theme.ImgDragDropIndicator );

				object o = ViewState["ImgDragDropIndicator"];
				return o == null ? this.ImagePath + "astreeview-dragDrop-indicator1.gif" : this.ProcessImageUrl( (string)o );
			}
			set
			{
				ViewState["ImgDragDropIndicator"] = value;
			}
		}

		#endregion

		#region ImgDragDropIndicatorSub (string)

		/// <summary>
		/// Gets or sets the drag drop indicator sub icon.
		/// </summary>
		/// <value>The img drag drop indicator sub.</value>
		[Browsable( true )
		, DefaultValue( "astreeview-dragDrop-indicator2.gif" )
		, Category( "Icon" )]
		public string ImgDragDropIndicatorSub
		{
			get
			{
				if( this.EnableTheme )
					return this.Page.ResolveUrl( this.Theme.ImgDragDropIndicatorSub );

				object o = ViewState["ImgDragDropIndicatorSub"];
				return o == null ? this.ImagePath + "astreeview-dragDrop-indicator2.gif" : this.ProcessImageUrl( (string)o );
			}
			set
			{
				ViewState["ImgDragDropIndicatorSub"] = value;
			}
		}

		#endregion

		#endregion

		#region ContextMenu

		#region Context Menu Instance

		/// <summary>
		/// Gets the context menu object.
		/// </summary>
		/// <value>The context menu.</value>
		public ASContextMenu ContextMenu
		{
			get
			{
				return this.ascmContextMenu;
			}
		}

		#endregion

		#region Context Menu ClientID

		/// <summary>
		/// Gets the context menu client ID.
		/// </summary>
		/// <value>The context menu client ID.</value>
		public string ContextMenuClientID
		{
			get
			{
				return this.ascmContextMenu.GetContextMenuObjectId();
			}
		}

		#endregion

		#region EnableContextMenu (bool)

		/// <summary>
		/// Gets or sets a value indicating whether to enable context menu.
		/// </summary>
		/// <value><c>true</c> if [enable context menu]; otherwise, <c>false</c>.</value>
		[Browsable( true )
		, DefaultValue( true )
		, Category( "Context Menu" )]
		public bool EnableContextMenu
		{
			get
			{
				object o = ViewState["EnableContextMenu"];
				return o == null ? true : (bool)o;
			}
			set
			{
				ViewState["EnableContextMenu"] = value;
			}
		}

		#endregion

		#region EnableContextMenuAdd (bool)

		/// <summary>
		/// Gets or sets a value indicating whether to enable the "Add" menu. Tree scope.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [enable context menu add]; otherwise, <c>false</c>.
		/// </value>
		[Browsable( true )
		, DefaultValue( true )
		, Category( "Context Menu" )]
		public bool EnableContextMenuAdd
		{
			get
			{
				object o = ViewState["EnableContextMenuAdd"];
				return o == null ? true : (bool)o;
			}
			set
			{
				ViewState["EnableContextMenuAdd"] = value;
			}
		}

		#endregion

		#region EnableContextMenuEdit (bool)

		/// <summary>
		/// Gets or sets a value indicating whether to enable the "Edit" menu. Tree scope.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [enable context menu edit]; otherwise, <c>false</c>.
		/// </value>
		[Browsable( true )
		, DefaultValue( true )
		, Category( "Context Menu" )]
		public bool EnableContextMenuEdit
		{
			get
			{
				object o = ViewState["EnableContextMenuEdit"];
				return o == null ? true : (bool)o;
			}
			set
			{
				ViewState["EnableContextMenuEdit"] = value;
			}
		}

		#endregion

		#region EnableContextMenuDelete (bool)

		/// <summary>
		/// Gets or sets a value indicating whether to enable the "Delete" menu. Tree scope.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [enable context menu delete]; otherwise, <c>false</c>.
		/// </value>
		[Browsable( true )
		, DefaultValue( true )
		, Category( "Context Menu" )]
		public bool EnableContextMenuDelete
		{
			get
			{
				object o = ViewState["EnableContextMenuDelete"];
				return o == null ? true : (bool)o;
			}
			set
			{
				ViewState["EnableContextMenuDelete"] = value;
			}
		}

		#endregion

		#region ContextMenuTargetCssClass (string)

		/// <summary>
		/// Gets the context menu target CSS class for attaching context menu to node.
		/// </summary>
		/// <value>The context menu target CSS class.</value>
		[Browsable( false )
		, Category( "Context Menu" )]
		public string ContextMenuTargetCssClass
		{
			get
			{
				return "ContextMenu" + this.ClientID;
			}
		}

		#endregion

		#region ContextMenuAddText (string)

		/// <summary>
		/// Gets or sets the context menu add text.
		/// </summary>
		/// <value>The context menu add text.</value>
		[Browsable( true )
		, DefaultValue( "Add" )
		, Category( "Context Menu" )]
		public string ContextMenuAddText
		{
			get
			{
				object o = ViewState["ContextMenuAddText"];
				return o == null ? "Add" : (string)o;
			}
			set
			{
				ViewState["ContextMenuAddText"] = value;
			}
		}

		#endregion

		#region ContextMenuEditText (string)

		/// <summary>
		/// Gets or sets the context menu edit text.
		/// </summary>
		/// <value>The context menu edit text.</value>
		[Browsable( true )
		, DefaultValue( "Edit" )
		, Category( "Context Menu" )]
		public string ContextMenuEditText
		{
			get
			{
				object o = ViewState["ContextMenuEditText"];
				return o == null ? "Edit" : (string)o;
			}
			set
			{
				ViewState["ContextMenuEditText"] = value;
			}
		}

		#endregion

		#region ContextMenuDeleteText (string)

		/// <summary>
		/// Gets or sets the context menu delete text.
		/// </summary>
		/// <value>The context menu delete text.</value>
		[Browsable( true )
		, DefaultValue( "Delete" )
		, Category( "Context Menu" )]
		public string ContextMenuDeleteText
		{
			get
			{
				object o = ViewState["ContextMenuDeleteText"];
				return o == null ? "Delete" : (string)o;
			}
			set
			{
				ViewState["ContextMenuDeleteText"] = value;
			}
		}

		#endregion

		#region ContextMenuAddCommandName (string)

		/// <summary>
		/// Gets or sets the name of the context menu add command.
		/// </summary>
		/// <value>The name of the context menu add command.</value>
		[Browsable( true )
		, DefaultValue( "add" )
		, Category( "Context Menu" )]
		public string ContextMenuAddCommandName
		{
			get
			{
				object o = ViewState["ContextMenuAddCommandName"];
				return o == null ? "add" : (string)o;
			}
			set
			{
				ViewState["ContextMenuAddCommandName"] = value;
			}
		}

		#endregion

		#region ContextMenuEditCommandName (string)

		/// <summary>
		/// Gets or sets the name of the context menu edit command.
		/// </summary>
		/// <value>The name of the context menu edit command.</value>
		[Browsable( true )
		, DefaultValue( "edit" )
		, Category( "Context Menu" )]
		public string ContextMenuEditCommandName
		{
			get
			{
				object o = ViewState["ContextMenuEditCommandName"];
				return o == null ? "edit" : (string)o;
			}
			set
			{
				ViewState["ContextMenuEditCommandName"] = value;
			}
		}

		#endregion

		#region ContextMenuDeleteCommandName (string)

		/// <summary>
		/// Gets or sets the name of the context menu delete command.
		/// </summary>
		/// <value>The name of the context menu delete command.</value>
		[Browsable( true )
		, DefaultValue( "delete" )
		, Category( "Context Menu" )]
		public string ContextMenuDeleteCommandName
		{
			get
			{
				object o = ViewState["ContextMenuDeleteCommandName"];
				return o == null ? "delete" : (string)o;
			}
			set
			{
				ViewState["ContextMenuDeleteCommandName"] = value;
			}
		}

		#endregion

		#endregion

		#region override properties

		#region TagKey

		/// <summary>
		/// Gets the <see cref="T:System.Web.UI.HtmlTextWriterTag"></see> value that corresponds to this Web server control. This property is used primarily by control developers.
		/// </summary>
		/// <value></value>
		/// <returns>One of the <see cref="T:System.Web.UI.HtmlTextWriterTag"></see> enumeration values.</returns>
		protected override HtmlTextWriterTag TagKey
		{
			get
			{
				return HtmlTextWriterTag.Div;
			}
		}

		#endregion

		#endregion

		#region Event Properties

		#region SelectedNodeString (string)

		/// <summary>
		/// Gets or sets the selected node string.
		/// </summary>
		/// <value>The selected node string.</value>
		[Browsable( true )
		, DefaultValue( "" )
		, Category( "Event Properties" )]
		virtual protected string SelectedNodeString
		{
			get
			{
				object o = ViewState["SelectedNodeString"];
				return o == null ? "" : (string)o;
			}
			set
			{
				ViewState["SelectedNodeString"] = value;
			}
		}

		#endregion

		#region CheckedNodeString (string)

		/// <summary>
		/// Gets or sets the checked node string.
		/// </summary>
		/// <value>The checked node string.</value>
		[Browsable( true )
		, DefaultValue( "" )
		, Category( "Event Properties" )]
		virtual protected string CheckedNodeString
		{
			get
			{
				object o = ViewState["CheckedNodeString"];
				return o == null ? "" : (string)o;
			}
			set
			{
				ViewState["CheckedNodeString"] = value;
			}
		}

		#endregion

		#endregion

		#region Tree Lines

		#region EnableTreeLines (bool)

		/// <summary>
		/// Gets or sets a value indicating whether tree lines.
		/// </summary>
		/// <value><c>true</c> if [enable tree lines]; otherwise, <c>false</c>.</value>
		[Browsable( true )
		, DefaultValue( false )
		, Category( "Tree Lines" )]
		public bool EnableTreeLines
		{
			get
			{
				object o = ViewState["EnableTreeLines"];
				return o == null ? true : (bool)o;
			}
			set
			{
				ViewState["EnableTreeLines"] = value;
			}
		}

		#endregion

		#region CssClassLineVertical (string)

		/// <summary>
		/// Gets or sets the CSS class for vertical line.
		/// </summary>
		/// <value>The CSS class line vertical.</value>
		[Browsable( true )
		, DefaultValue( "line-vertical" )
		, Category( "Tree Lines" )]
		public string CssClassLineVertical
		{
			get
			{
				object o = ViewState["CssClassLineVertical"];
				return o == null ? "line-vertical" : (string)o;
			}
			set
			{
				ViewState["CssClassLineVertical"] = value;
			}
		}

		#endregion

		#region CssClassLineRoot (string)

		/// <summary>
		/// Gets or sets the CSS class for root line.
		/// </summary>
		/// <value>The CSS class line root.</value>
		[Browsable( true )
		, DefaultValue( "line-root" )
		, Category( "Tree Lines" )]
		public string CssClassLineRoot
		{
			get
			{
				object o = ViewState["CssClassLineRoot"];
				return o == null ? "line-root" : (string)o;
			}
			set
			{
				ViewState["CssClassLineRoot"] = value;
			}
		}

		#endregion

		#region CssClassLineTop (string)

		/// <summary>
		/// Gets or sets the CSS class for top line.
		/// </summary>
		/// <value>The CSS class line top.</value>
		[Browsable( true )
		, DefaultValue( "line-top" )
		, Category( "Tree Lines" )]
		public string CssClassLineTop
		{
			get
			{
				object o = ViewState["CssClassLineTop"];
				return o == null ? "line-top" : (string)o;
			}
			set
			{
				ViewState["CssClassLineTop"] = value;
			}
		}

		#endregion

		#region CssClassLineMiddle (string)

		/// <summary>
		/// Gets or sets the CSS class for middle line.
		/// </summary>
		/// <value>The CSS class line middle.</value>
		[Browsable( true )
		, DefaultValue( "line-middle" )
		, Category( "Tree Lines" )]
		public string CssClassLineMiddle
		{
			get
			{
				object o = ViewState["CssClassLineMiddle"];
				return o == null ? "line-middle" : (string)o;
			}
			set
			{
				ViewState["CssClassLineMiddle"] = value;
			}
		}

		#endregion

		#region CssClassLineBottom (string)

		/// <summary>
		/// Gets or sets the CSS class for bottom line.
		/// </summary>
		/// <value>The CSS class line bottom.</value>
		[Browsable( true )
		, DefaultValue( "line-bottom" )
		, Category( "Tree Lines" )]
		public string CssClassLineBottom
		{
			get
			{
				object o = ViewState["CssClassLineBottom"];
				return o == null ? "line-bottom" : (string)o;
			}
			set
			{
				ViewState["CssClassLineBottom"] = value;
			}
		}

		#endregion

		#region CssClassLineNone (string)

		/// <summary>
		/// Gets or sets the CSS class for no line.
		/// </summary>
		/// <value>The CSS class line none.</value>
		[Browsable( true )
		, DefaultValue( "line-none" )
		, Category( "Tree Lines" )]
		public string CssClassLineNone
		{
			get
			{
				object o = ViewState["CssClassLineNone"];
				return o == null ? "line-none" : (string)o;
			}
			set
			{
				ViewState["CssClassLineNone"] = value;
			}
		}

		#endregion

		#endregion

		#region Ajax Add & Edit & Delete & Load Nodes

		#region EnableAjaxOnEditDelete (bool)

		/// <summary>
		/// Gets or sets a value indicating whether to enable ajax on edit and delete.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [enable ajax on edit delete]; otherwise, <c>false</c>.
		/// </value>
		[Browsable( true )
		, DefaultValue( false )
		, Category( "Ajax" )]
		public bool EnableAjaxOnEditDelete
		{
			get
			{
				object o = ViewState["EnableAjaxOnEditDelete"];
				return o == null ? false : (bool)o;
			}
			set
			{
				ViewState["EnableAjaxOnEditDelete"] = value;
			}
		}

		#endregion

		#region Add

		#region AddNodeProvider (string)

		/// <summary>
		/// Gets or sets the add node provider.
		/// The provider is to render a piece of html to the client, usually is a ul element.
		/// for example:
		///		HtmlGenericControl ulRoot = new HtmlGenericControl( "ul" );
		///		astvMyTree.TreeViewHelper.ConvertTree( ulRoot, root, false );
		///		foreach( Control c in ulRoot.Controls )
		///			c.RenderControl( writer );
		/// </summary>
		/// <value>The add node provider.</value>
		[Browsable( true )
		, DefaultValue( "AddNode.aspx" )
		, Category( "Ajax" )]
		public string AddNodeProvider
		{
			get
			{
				object o = ViewState["AddNodeProvider"];
				return o == null ? "AddNode.aspx" : ResolveUrl( (string)o );
			}
			set
			{
				ViewState["AddNodeProvider"] = value;
			}
		}

		#endregion

		#region AddNodePromptMessage (string)

		/// <summary>
		/// Gets or sets the add node prompt message.
		/// </summary>
		/// <value>The add node prompt message.</value>
		[Browsable( true )
		, DefaultValue( "Please enter name for the new node." )
		, Category( "Ajax" )]
		public string AddNodePromptMessage
		{
			get
			{
				object o = ViewState["AddNodePromptMessage"];
				return o == null ? "Please enter name for the new node." : (string)o;
			}
			set
			{
				ViewState["AddNodePromptMessage"] = value;
			}
		}

		#endregion

		#region AddNodePromptDefaultValue (string)

		/// <summary>
		/// Gets or sets the add node default value.
		/// </summary>
		/// <value>The add node default value of adding a new node.</value>
		[Browsable( true )
		, DefaultValue( "" )
		, Category( "Ajax" )]
		public string AddNodePromptDefaultValue
		{
			get
			{
				object o = ViewState["AddNodePromptDefaultValue"];
				return o == null ? "" : (string)o;
			}
			set
			{
				ViewState["AddNodePromptDefaultValue"] = value;
			}
		}

		#endregion

		#region AddNodeDataValueProvider (string)

		/// <summary>
		/// Gets or sets the cutomized function which return a name of the new node.
		/// </summary>
		/// <value>The add node prompt message.</value>
		[Browsable( true )
		, DefaultValue( "" )
		, Category( "Ajax" )]
		public string AddNodeDataValueProvider
		{
			get
			{
				object o = ViewState["AddNodeDataValueProvider"];
				return o == null ? "" : (string)o;
			}
			set
			{
				ViewState["AddNodeDataValueProvider"] = value;
			}
		}

		#endregion



		#region AdditionalAddRequestParameters (KeyValuePair<string, string>)

		/// <summary>
		/// Gets or sets the additional add request parameters. in json format, like {'a':'b', 'c','c'}
		/// </summary>
		/// <value>The additional add request parameters.</value>
		[Browsable( true )
		, DefaultValue( "{}" )
		, Category( "Ajax" )
		, Description( "json format, like {'a':'b', 'c','c'}" )]
		public string AdditionalAddRequestParameters
		{
			get
			{
				object o = ViewState["AdditionalAddRequestParameters"];
				return o == null ? "{}" : (string)o;
			}
			set
			{
				ViewState["AdditionalAddRequestParameters"] = value;
			}
		}

		#endregion

		#endregion

		#region edit

		#region EditNodeProvider (string)

		/// <summary>
		/// Gets or sets the edit node provider.
		/// The edit provider should return a result code to the client.
		/// For example:
		/// protected override void Render( HtmlTextWriter writer )
		///	{
		///		if( this.returnCode == ASTreeViewAjaxReturnCode.OK )
		///			writer.Write( (int)this.returnCode );
		///		else
		///			writer.Write( this.errorMessage );
		///	}
		/// </summary>
		/// <value>The edit node provider.</value>
		[Browsable( true )
		, DefaultValue( "EditNode.aspx" )
		, Category( "Ajax" )]
		public string EditNodeProvider
		{
			get
			{
				object o = ViewState["EditNodeProvider"];
				return o == null ? "EditNode.aspx" : ResolveUrl( (string)o );
			}
			set
			{
				ViewState["EditNodeProvider"] = value;
			}
		}

		#endregion

		#region AdditionalEditRequestParameters (string)

		/// <summary>
		/// Gets or sets the additional edit request parameters. in json format, like {'a':'b', 'c','c'}
		/// </summary>
		/// <value>The additional edit request parameters.</value>
		[Browsable( true )
		, DefaultValue( "{}" )
		, Category( "Ajax" )
		, Description( "json format, like {'a':'b', 'c','c'}" )]
		public string AdditionalEditRequestParameters
		{
			get
			{
				object o = ViewState["AdditionalEditRequestParameters"];
				return o == null ? "{}" : (string)o;
			}
			set
			{
				ViewState["AdditionalEditRequestParameters"] = value;
			}
		}

		#endregion

		#endregion

		#region delete

		#region DeleteNodeProvider (string)

		/// <summary>
		/// Gets or sets the delete node provider.
		/// The delete provider should return a result code.
		/// For example:
		/// protected override void Render( HtmlTextWriter writer )
		///	{
		///		if( this.returnCode == ASTreeViewAjaxReturnCode.OK )
		///			writer.Write( (int)this.returnCode );
		///		else
		///			writer.Write( this.errorMessage );
		///	}
		/// </summary>
		/// <value>The delete node provider.</value>
		[Browsable( true )
		, DefaultValue( "DeleteNode.aspx" )
		, Category( "Ajax" )]
		public string DeleteNodeProvider
		{
			get
			{
				object o = ViewState["DeleteNodeProvider"];
				return o == null ? "DeleteNode.aspx" : ResolveUrl( (string)o );
			}
			set
			{
				ViewState["DeleteNodeProvider"] = value;
			}
		}

		#endregion

		#region DeleteNodePromptMessage (string)

		/// <summary>
		/// Gets or sets the delete node prompt message.
		/// </summary>
		/// <value>The delete node prompt message.</value>
		[Browsable( true )
		, DefaultValue( "Are you sure to delete {0}?" )
		, Category( "Ajax" )]
		public string DeleteNodePromptMessage
		{
			get
			{
				object o = ViewState["DeleteNodePromptMessage"];
				return o == null ? "Are you sure to delete {0}?" : (string)o;
			}
			set
			{
				ViewState["DeleteNodePromptMessage"] = value;
			}
		}

		#endregion

		#region DeleteNodeWithSubPromptMessage (string)

		/// <summary>
		/// Gets or sets the delete node with sub prompt message.
		/// </summary>
		/// <value>The delete node with sub prompt message.</value>
		[Browsable( true )
		, DefaultValue( "Are you sure to delete {0}? It has sub nodes." )
		, Category( "Ajax" )]
		public string DeleteNodeWithSubPromptMessage
		{
			get
			{
				object o = ViewState["DeleteNodeWithSubPromptMessage"];
				return o == null ? "Are you sure to delete {0}? It has sub nodes." : (string)o;
			}
			set
			{
				ViewState["DeleteNodeWithSubPromptMessage"] = value;
			}
		}

		#endregion

		#region AdditionalDeleteRequestParameters (KeyValuePair<string, string>)

		/// <summary>
		/// Gets or sets the additional delete request parameters. in json format, like {'a':'b', 'c','c'}.
		/// </summary>
		/// <value>The additional delete request parameters.</value>
		[Browsable( true )
		, DefaultValue( "{}" )
		, Category( "Ajax" )
		, Description( "json format, like {'a':'b', 'c','c'}" )]
		public string AdditionalDeleteRequestParameters
		{
			get
			{
				object o = ViewState["AdditionalDeleteRequestParameters"];
				return o == null ? "{}" : (string)o;
			}
			set
			{
				ViewState["AdditionalDeleteRequestParameters"] = value;
			}
		}

		#endregion

		#endregion

		#region load

		/// <summary>
		/// Gets or sets the load nodes provider.
		/// The load nodes provider should renturn one or more UL elements.
		/// For example:
		///		HtmlGenericControl ulRoot = new HtmlGenericControl( "ul" );
		///		astvMyTree.TreeViewHelper.ConvertTree( ulRoot, root, false );
		///		foreach( Control c in ulRoot.Controls )
		///			c.RenderControl( writer );
		/// </summary>
		/// <value>The load nodes provider.</value>
		[Browsable( true )
		, DefaultValue( "LoadNodes.aspx" )
		, Category( "Ajax" )]
		public string LoadNodesProvider
		{
			get
			{
				object o = ViewState["LoadNodesProvider"];
				return o == null ? "LoadNodes.aspx" : ResolveUrl( (string)o );
			}
			set
			{
				ViewState["LoadNodesProvider"] = value;
			}
		}

		#region VirtualNodePlaceHolderText (string)

		/// <summary>
		/// Gets or sets the text displayed while loading nodes.
		/// </summary>
		/// <value>The virtual node place holder text.</value>
		[Browsable( true )
		, DefaultValue( "Loading {0} node(s)..." )
		, Category( "Ajax" )]
		public string VirtualNodePlaceHolderText
		{
			get
			{
				object o = ViewState["VirtualNodePlaceHolderText"];
				return o == null ? "Loading {0} nodes..." : (string)o;
			}
			set
			{
				ViewState["VirtualNodePlaceHolderText"] = value;
			}
		}

		#endregion

		#region AdditionalLoadNodesRequestParameters (KeyValuePair<string, string>)

		/// <summary>
		/// Gets or sets the additional load nodes request parameters. in json format, like {'a':'b', 'c','c'}.
		/// </summary>
		/// <value>The additional load nodes request parameters.</value>
		[Browsable( true )
		, DefaultValue( "{}" )
		, Category( "Ajax" )
		, Description( "json format, like {'a':'b', 'c','c'}" )]
		public string AdditionalLoadNodesRequestParameters
		{
			get
			{
				object o = ViewState["AdditionalLoadNodesRequestParameters"];
				return o == null ? "{}" : (string)o;
			}
			set
			{
				ViewState["AdditionalLoadNodesRequestParameters"] = value;
			}
		}

		#endregion

		#endregion

		#region indicator

		#region CssClassAjaxIndicatorContainer (string)

		/// <summary>
		/// Gets or sets the CSS class of ajax indicator container.
		/// </summary>
		/// <value>The CSS class ajax indicator container.</value>
		[Browsable( true )
		, DefaultValue( "astreeview-ajax-indicator-container" )
		, Category( "Ajax" )]
		public string CssClassAjaxIndicatorContainer
		{
			get
			{
				object o = ViewState["CssClassAjaxIndicatorContainer"];
				return o == null ? "astreeview-ajax-indicator-container" : (string)o;
			}
			set
			{
				ViewState["CssClassAjaxIndicatorContainer"] = value;
			}
		}

		#endregion

		#region AjaxIndicatorContainerID (string)

		/// <summary>
		/// Gets or sets the ajax indicator container ID.
		/// </summary>
		/// <value>The ajax indicator container ID.</value>
		[Browsable( true )
		, Category( "Ajax" )]
		public string AjaxIndicatorContainerID
		{
			get
			{
				object o = ViewState["AjaxIndicatorContainerID"];
				return o == null ? this.divAjaxIndicatorContainer.ClientID : (string)o;
			}
			set
			{
				ViewState["AjaxIndicatorContainerID"] = value;
			}
		}

		#endregion

		#region AjaxIndicatorText (string)

		/// <summary>
		/// Gets or sets the ajax indicator text.
		/// </summary>
		/// <value>The ajax indicator text.</value>
		[Browsable( true )
		, DefaultValue( "Processing..." )
		, Category( "Ajax" )]
		public string AjaxIndicatorText
		{
			get
			{
				object o = ViewState["AjaxIndicatorText"];
				return o == null ? "Processing..." : (string)o;
			}
			set
			{
				ViewState["AjaxIndicatorText"] = value;
			}
		}

		#endregion

		#endregion

		#endregion

		#region Max Depth

		#region MaxDepth (int)

		/// <summary>
		/// Gets or sets the max depth of the treeview.
		/// </summary>
		/// <value>The max depth.</value>
		[Browsable( true )
		, DefaultValue( 999 )
		, Category( "Max Depth" )]
		public int MaxDepth
		{
			get
			{
				object o = ViewState["MaxDepth"];
				return o == null ? 999 : (int)o;
			}
			set
			{
				ViewState["MaxDepth"] = value;
			}
		}

		#endregion

		#region MaxDepthReachedMessage (string)

		/// <summary>
		/// Gets or sets the max depth reached message.
		/// </summary>
		/// <value>The max depth reached message.</value>
		[Browsable( true )
		, DefaultValue( "Maximum depth ({0}) reached!" )
		, Category( "Max Depth" )]
		public string MaxDepthReachedMessage
		{
			get
			{
				object o = ViewState["MaxDepthReachedMessage"];
				return o == null ? "Maximum depth ({0}) reached!" : (string)o;
			}
			set
			{
				ViewState["MaxDepthReachedMessage"] = value;
			}
		}

		#endregion

		#endregion

		#region ExpandDepth (int)

		/// <summary>
		/// Gets or sets the expand depth of the treeview.
		/// </summary>
		/// <value>The expand depth.</value>
		[Browsable( true )
		, DefaultValue( -1 )
		, Category( "ExpandDepth" )]
		public int ExpandDepth
		{
			get
			{
				object o = ViewState["ExpandDepth"];
				if( o == null )
					return -1;

				int depth = (int)o;
				if( depth < 0 )
					return depth;

				if( !this.EnableRoot )
					depth++;

				return depth;
			}
			set
			{
				ViewState["ExpandDepth"] = value;
			}
		}

		#endregion

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="ASTreeView"/> class.
		/// </summary>
		public ASTreeView()
		{
			this.txtASTreeViewNodes = new HiddenField();
			this.txtSelectedASTreeViewNodes = new HiddenField();
			this.txtCheckedASTreeViewNodes = new HiddenField();
			this.txtIsBound = new HiddenField();
			this.divDebugContainer = new HtmlGenericControl();
			this.divAjaxIndicatorContainer = new HtmlGenericControl();
			this.ulASTreeView = new HtmlGenericControl();
			this.ascmContextMenu = new ASContextMenu();
			this.divTreeViewContainer = new HtmlGenericControl();
		}

		#endregion

		#region Public Methods

		#region GetClientTreeObjectId

		/// <summary>
		/// Gets the client tree object id.
		/// </summary>
		/// <returns></returns>
		virtual public string GetClientTreeObjectId()
		{
			string returnVal = string.Format( "asTreeViewObj{0}", this.ClientID );
			return this.EnableDebugMode ? returnVal : this.treeViewHelper.ObfuscateScript( returnVal );
		}

		#endregion

		#region GetExpandAllScript

		/// <summary>
		/// Gets the expand all client side script.
		/// </summary>
		/// <returns></returns>
		virtual public string GetExpandAllScript()
		{
			string returnVal = string.Format( "asTreeViewObj{0}.expandAll();", this.ClientID );
			return this.EnableDebugMode ? returnVal : this.treeViewHelper.ObfuscateScript( returnVal );
		}

		#endregion

		#region GetCollapseAllScript

		/// <summary>
		/// Gets the collapse all client side script.
		/// </summary>
		/// <returns></returns>
		virtual public string GetCollapseAllScript()
		{
			string returnVal = string.Format( "asTreeViewObj{0}.collapseAll();", this.ClientID );
			return this.EnableDebugMode ? returnVal : this.treeViewHelper.ObfuscateScript( returnVal );
		}

		#endregion

		#region GetToggleExpandCollapseAllScript

		/// <summary>
		/// Gets the toggle expand collapse all client side script.
		/// </summary>
		/// <returns></returns>
		virtual public string GetToggleExpandCollapseAllScript()
		{
			string returnVal = string.Format( "asTreeViewObj{0}.toggleExpandCollapseAll();", this.ClientID );
			return this.EnableDebugMode ? returnVal : this.treeViewHelper.ObfuscateScript( returnVal );
		}

		#endregion

		#region GetSelectedNode

		/// <summary>
		/// Gets the selected node.
		/// </summary>
		/// <returns></returns>
		virtual public ASTreeViewNode GetSelectedNode()
		{
			List<ASTreeViewNode> selectedNodes = new List<ASTreeViewNode>();
			ASTreeNodeHandlerDelegate selectedNodeDelegate = delegate( ASTreeViewNode node )
			{
				if( node.Selected )
					selectedNodes.Add( node );
			};

			this.TraverseTreeNode( this.rootNode, selectedNodeDelegate );

			if( selectedNodes.Count == 0 )
				return null;

			if( selectedNodes.Count > 1 )
				throw new ASTreeViewInvalidStateException( "More than one nodes are selected!" );

			return selectedNodes[0];
		}

		#endregion

		#region GetCheckedNodes

		/// <summary>
		/// Gets the checked nodes.
		/// </summary>
		/// <returns></returns>
		virtual public List<ASTreeViewNode> GetCheckedNodes()
		{
			return GetCheckedNodes( false );
		}

		#endregion

		#region GetCheckedNodes

		/// <summary>
		/// Gets the checked nodes.
		/// </summary>
		/// <param name="includeHalfChecked">if set to <c>true</c> [include half checked].</param>
		/// <returns></returns>
		virtual public List<ASTreeViewNode> GetCheckedNodes( bool includeHalfChecked )
		{
			List<ASTreeViewNode> checkedNodes = new List<ASTreeViewNode>();
			ASTreeNodeHandlerDelegate checkedNodesDelegate = delegate( ASTreeViewNode node )
			{
				if( node.CheckedState == ASTreeViewCheckboxState.Checked )
					checkedNodes.Add( node );
				else if( includeHalfChecked && node.CheckedState == ASTreeViewCheckboxState.HalfChecked )
					checkedNodes.Add( node );
			};

			if( this.EnableRoot )
				this.TraverseTreeNode( this.rootNode, checkedNodesDelegate );
			else
			{
				foreach( ASTreeViewNode node in rootNode.ChildNodes )
					this.TraverseTreeNode( node, checkedNodesDelegate );
			}


			return checkedNodes;
		}

		#endregion

		#region TraverseTreeNode

		/// <summary>
		/// Traverses the tree node.
		/// Usage:
		///		ASTreeView.ASTreeNodeHandlerDelegate nodeDelegate = delegate( ASTreeViewNode node )
		///		{
		///			Console.Write( node.NodeValue );
		///		};
		///
		///		astvMyTree.TraverseTreeNode( this.astvMyTree.RootNode, nodeDelegate );
		/// </summary>
		/// <param name="parent">The parent.</param>
		/// <param name="handler">The handler.</param>
		virtual public void TraverseTreeNode( ASTreeViewNode parent, ASTreeNodeHandlerDelegate handler )
		{
			if( parent == null )
				return;

			handler( parent );

			foreach( ASTreeViewNode child in parent.ChildNodes )
				TraverseTreeNode( child, handler );
		}

		#endregion

		#region TraverseTreeUL

		#region usage
		/*
		base.TraverseTreeUL( base.ulASTreeView, new ASTreeULHandlerDelegate( delegate( HtmlGenericControl parentUL )
		{
			foreach( Control c in parentUL.Controls )
			{
				if( c is HtmlGenericControl )
				{
					HtmlGenericControl li = (HtmlGenericControl)c;
					foreach( Control liChild in li.Controls )
					{
						if( liChild is HtmlImage )
						{
							HtmlImage hiCheckbox = (HtmlImage)liChild;
							if( hiCheckbox.Attributes["icon-type"] == ( (int)ASTreeViewIconType.Checkbox ).ToString() )
								hiCheckbox.Attributes.Add( "onclick", "alert('checkbox clicked!')" );
						}
					}
				}
			}
		} ) );
		*/
		#endregion
		/// <summary>
		/// Traverses the tree UL.
		/// Usage:
		///		base.TraverseTreeUL( base.ulASTreeView, new ASTreeULHandlerDelegate( delegate( HtmlGenericControl parentUL )
		///		{
		///			foreach( Control c in parentUL.Controls )
		///			{
		///				if( c is HtmlGenericControl )
		///				{
		///					HtmlGenericControl li = (HtmlGenericControl)c;
		///					foreach( Control liChild in li.Controls )
		///					{
		///						if( liChild is HtmlImage )
		///						{
		///							HtmlImage hiCheckbox = (HtmlImage)liChild;
		///							if( hiCheckbox.Attributes["icon-type"] == ( (int)ASTreeViewIconType.Checkbox ).ToString() )
		///								hiCheckbox.Attributes.Add( "onclick", "alert('checkbox clicked!')" );
		///						}
		///					}
		///				}
		///			}
		///		} ) );
		/// </summary>
		/// <param name="parentUl">The parent ul.</param>
		/// <param name="handler">The handler.</param>
		virtual public void TraverseTreeUL( HtmlGenericControl parentUl, ASTreeULHandlerDelegate handler )
		{
			if( parentUl == null )
				return;

			handler( parentUl );

			foreach( Control wc in parentUl.Controls )
			{
				if( wc is HtmlGenericControl )
				{
					HtmlGenericControl hgc = (HtmlGenericControl)wc;
					if( hgc.TagName.ToLower() == "li" )
					{
						foreach( Control wc2 in hgc.Controls )
						{
							if( wc2 is HtmlGenericControl )
							{
								HtmlGenericControl hgc2 = (HtmlGenericControl)wc2;
								if( hgc2.TagName.ToLower() == "ul" )
									TraverseTreeUL( hgc2, handler );
							}
						}
					}
				}
			}
		}

		#endregion

		#region GetTreeViewXML

		/// <summary>
		/// Gets the tree view XML.
		/// </summary>
		/// <returns></returns>
		virtual public XmlDocument GetTreeViewXML()
		{
			return this.treeViewHelper.GetTreeViewXML();
		}

		#endregion

		#region CollapseAll

		/// <summary>
		/// Collapses all tree nodes.
		/// </summary>
		virtual public void CollapseAll()
		{
			ASTreeView.ASTreeNodeHandlerDelegate nodeDelegate = delegate( ASTreeViewNode node )
			{
				node.OpenState = ASTreeViewNodeOpenState.Close;
			};

			this.TraverseTreeNode( this.rootNode, nodeDelegate );
		}

		#endregion

		#region ExpandAll

		/// <summary>
		/// Expands all tree nodes.
		/// </summary>
		virtual public void ExpandAll()
		{
			ASTreeView.ASTreeNodeHandlerDelegate nodeDelegate = delegate( ASTreeViewNode node )
			{
				node.OpenState = ASTreeViewNodeOpenState.Open;
			};

			this.TraverseTreeNode( this.rootNode, nodeDelegate );
		}

		#endregion

		#region ExpandToDepth

		/// <summary>
		/// Expands to depth.
		/// </summary>
		/// <param name="depth">The depth.</param>
		virtual public void ExpandToDepth( int depth )
		{
			if( depth < 0 )
				return;

			CollapseAll();

			int curDepth = 0;

			ExpandNode( this.rootNode, depth, curDepth );
		}

		#endregion

		#region ForceRenderInitialScript
		/// <summary>
		/// Forces the render initial script. Especially for this kind of situation: astreeview in a updatepanel which UpdateMode=Conditional, and a trigger calls this update panel's Update() methods. before call Update(), you need call ForceRenderIntialScript first.
		/// </summary>
		virtual public void ForceRenderInitialScript()
		{
			this.forceRenderInitialScript = true;
			this.ascmContextMenu.ForceRenderInitialScript();
		}
		#endregion

		#region ClearNodesCheck
		/// <summary>
		/// Clears the nodes check.
		/// </summary>
		virtual public void ClearNodesCheck()
		{
			ASTreeNodeHandlerDelegate nodeDelegate = delegate( ASTreeViewNode node )
			{
				node.CheckedState = ASTreeViewCheckboxState.Unchecked;
			};

            this.TraverseTreeNode(this.rootNode, nodeDelegate);

            this.CheckedNodeString = string.Empty;
            this.txtCheckedASTreeViewNodes.Value = string.Empty;
		}
		#endregion

		#region ClearNodesSelection
		/// <summary>
		/// Clears the nodes selection.
		/// </summary>
		virtual public void ClearNodesSelection()
		{
			ASTreeNodeHandlerDelegate nodeDelegate = delegate( ASTreeViewNode node )
			{
				node.Selected = false;
			};

            this.TraverseTreeNode(this.rootNode, nodeDelegate);

            this.SelectedNodeString = string.Empty;
            this.txtSelectedASTreeViewNodes.Value = string.Empty;
		}
		#endregion

		#region SelectNode

		/// <summary>
		/// Selects the node.
		/// </summary>
		/// <param name="nodeValue">The node value.</param>
		virtual public void SelectNode( string nodeValue )
		{
			ASTreeNodeHandlerDelegate selectNodeDelegate = delegate( ASTreeViewNode node )
			{
				if( node.NodeValue == nodeValue )
					node.Selected = true;
				else
					node.Selected = false;
			};

			this.TraverseTreeNode( this.rootNode, selectNodeDelegate );
		}

		#endregion

		#region CheckNodes

		/// <summary>
		/// Checks the nodes.
		/// </summary>
		/// <param name="nodeValues">The node values.</param>
		virtual public void CheckNodes( string[] nodeValues )
		{
			CheckNodes( nodeValues, true );
		}

		/// <summary>
		/// Checks the nodes.
		/// </summary>
		/// <param name="nodeValues">The node values.</param>
		/// <param name="isRecursive">if set to <c>true</c> [is recursive].</param>
		virtual public void CheckNodes( string[] nodeValues, bool isRecursive )
		{
			ASTreeNodeHandlerDelegate checkNodesDelegate = delegate( ASTreeViewNode node )
			{
				foreach( string val in nodeValues )
				{
					if( node.NodeValue == val )
					{
						if( isRecursive )
							this.SetNodeCheckStateRecursive( node, ASTreeViewCheckboxState.Checked );
						else
							node.CheckedState = ASTreeViewCheckboxState.Checked;

						break;
					}
				}
			};

			this.TraverseTreeNode( this.rootNode, checkNodesDelegate );
		}

		#endregion

		#region UnCheckNodes

		/// <summary>
		/// UnCheck the check nodes.
		/// </summary>
		/// <param name="nodeValues">The node values.</param>
		virtual public void UnCheckNodes( string[] nodeValues )
		{
			this.UnCheckNodes( nodeValues, true );
		}

		/// <summary>
		/// Uncheck the check nodes.
		/// </summary>
		/// <param name="nodeValues">The node values.</param>
		/// <param name="isRecursive">if set to <c>true</c> [is recursive].</param>
		virtual public void UnCheckNodes( string[] nodeValues, bool isRecursive )
		{
			ASTreeNodeHandlerDelegate unCheckNodesDelegate = delegate( ASTreeViewNode node )
			{
				foreach( string val in nodeValues )
				{
					if( node.NodeValue == val )
					{
						if( isRecursive )
							this.SetNodeCheckStateRecursive( node, ASTreeViewCheckboxState.Unchecked );
						else
							node.CheckedState = ASTreeViewCheckboxState.Unchecked;

						break;
					}
				}
			};

			this.TraverseTreeNode( this.rootNode, unCheckNodesDelegate );
		}

		#endregion

		#region SetNodeCheckStateRecursive

		/// <summary>
		/// Sets the node check state recursive.
		/// </summary>
		/// <param name="node">The node.</param>
		/// <param name="state">The state.</param>
		virtual public void SetNodeCheckStateRecursive( ASTreeViewNode node, ASTreeViewCheckboxState state )
		{
			node.CheckedState = state;
			foreach( ASTreeViewNode child in node.ChildNodes )
				SetNodeCheckStateRecursive( child, state );
		}

		#endregion

		#region FindByValue

		/// <summary>
		/// Finds the by value. Return the first node found. return null if nothing found.
		/// </summary>
		/// <param name="nodeValue">The node value.</param>
		/// <returns></returns>
		virtual public ASTreeViewNode FindByValue( string nodeValue )
		{
			ASTreeViewNode returnVal = null;
			ASTreeNodeHandlerDelegate findNodeDelegate = delegate( ASTreeViewNode node )
			{
				if( node.NodeValue == nodeValue )
					returnVal = node;

				return;
			};

			this.TraverseTreeNode( this.rootNode, findNodeDelegate );

			return returnVal;
		}

		#endregion

		/// <summary>
		/// Finds the by text. Return the first node found. return null if nothing found.
		/// </summary>
		/// <param name="nodeText">The node text.</param>
		/// <returns></returns>
		virtual public ASTreeViewNode FindByText( string nodeText )
		{
			ASTreeViewNode returnVal = null;
			ASTreeNodeHandlerDelegate findNodeDelegate = delegate( ASTreeViewNode node )
			{
				if( node.NodeText == nodeText )
					returnVal = node;

				return;
			};

			this.TraverseTreeNode( this.rootNode, findNodeDelegate );

			return returnVal;
		}

		#endregion

		#region Protected Methods

		#region InitializeControls

		/// <summary>
		/// Initializes the controls.
		/// </summary>
		virtual protected void InitializeControls()
		{
			this.treeViewHelper = new ASTreeViewHelper();
			this.treeViewHelper.CurrentTreeView = this;

			this.txtASTreeViewNodes.ID = "txtASTreeViewNodes";
			this.txtSelectedASTreeViewNodes.ID = "txtSelectedASTreeViewNodes";
			this.txtCheckedASTreeViewNodes.ID = "txtCheckedASTreeViewNodes";
			this.txtIsBound.ID = "txtIsBound";
			
			this.divDebugContainer.ID = "divDebugContainer";
			this.divDebugContainer.TagName = "div";

			this.divAjaxIndicatorContainer.ID = "divAjaxIndicatorContainer";
			this.divAjaxIndicatorContainer.TagName = "div";

			this.ulASTreeView.ID = "ulASTreeView";
			this.ulASTreeView.TagName = "ul";

			//initial root
			this.rootNode = new ASTreeViewNode();

			this.ascmContextMenu.ID = "ascmContextMenu";

			this.divTreeViewContainer.ID = "divTreeViewContainer";
			this.divTreeViewContainer.TagName = "div";

			this.EnsureChildControls();

		}

		#endregion

		#region LoadTreeNodes

		/// <summary>
		/// Loads the tree nodes.
		/// </summary>
		virtual protected void LoadTreeNodes()
		{
			//load tree nodes from textbox
			treeViewHelper.ParseJsonTree( this.RootNode, this.strASTreeViewNodes );//this.txtASTreeViewNodes.Value.Trim() );

			//save state when treeview is not visible
			if( !this.Visible )
				this.txtASTreeViewNodes.Value = treeViewHelper.GetNodeJsonString( this.RootNode );
		}

		#endregion

		#region IsInPostback
		/// <summary>
		/// return true is in postback or async postback
		/// </summary>
		/// <returns></returns>
		virtual protected bool IsInPostback()
		{
			return IsInUpdateAsyncPostback() || this.Page.IsPostBack;
		}

		#endregion

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

		#region RenderSelectedNodeContainer

		/// <summary>
		/// Renders the selected node container.
		/// </summary>
		/// <param name="writer">The writer.</param>
		virtual protected void RenderSelectedNodeContainer( HtmlTextWriter writer )
		{
			string selectedNodeTextboxId = this.UniqueID + this.selectedNodeTextboxSurfix;

			//if( !this.EnableDebugMode )
			//	writer.AddStyleAttribute( HtmlTextWriterStyle.Display, "none" );

			writer.AddAttribute( HtmlTextWriterAttribute.Type, "hidden" );
			writer.AddAttribute( HtmlTextWriterAttribute.Value, this.SelectedNodeString );
			writer.AddAttribute( HtmlTextWriterAttribute.Name, this.UniqueID );
			writer.AddAttribute( HtmlTextWriterAttribute.Id, selectedNodeTextboxId );
			writer.RenderBeginTag( HtmlTextWriterTag.Input );
			writer.RenderEndTag();

		}

		#endregion

		#region RenderCheckedNodeContainer

		/// <summary>
		/// Renders the checked node container.
		/// </summary>
		/// <param name="writer">The writer.</param>
		virtual protected void RenderCheckedNodeContainer( HtmlTextWriter writer )
		{
			string checkedNodeTextboxId = this.UniqueID + this.checkedNodeTextboxSurfix;

			//if( !this.EnableDebugMode )
			//	writer.AddStyleAttribute( HtmlTextWriterStyle.Display, "none" );

			writer.AddAttribute( HtmlTextWriterAttribute.Type, "hidden" );
			writer.AddAttribute( HtmlTextWriterAttribute.Value, this.CheckedNodeString );
			writer.AddAttribute( HtmlTextWriterAttribute.Name, this.UniqueID );
			writer.AddAttribute( HtmlTextWriterAttribute.Id, checkedNodeTextboxId );
			writer.RenderBeginTag( HtmlTextWriterTag.Input );
			writer.RenderEndTag();

		}

		#endregion

		#region CreateTreeControls

		/// <summary>
		/// Creates the tree controls.
		/// </summary>
		virtual protected void CreateTreeControls()
		{
			this.divDebugContainer.Controls.Add( this.txtASTreeViewNodes );
			this.divDebugContainer.Controls.Add( this.txtSelectedASTreeViewNodes );
			this.divDebugContainer.Controls.Add( this.txtCheckedASTreeViewNodes );
			this.divDebugContainer.Controls.Add( this.txtIsBound );
			//this.divDebugContainer.Controls.Add( this.ascmContextMenu );
			if( !this.EnableDebugMode )
				this.divDebugContainer.Style.Add( "display", "none" );
			else
			{
				HtmlGenericControl divDebugIndicator = new HtmlGenericControl( "div" );
				divDebugIndicator.InnerHtml = "<div style='border:2px solid green;padding:4px;background:#eee;font-weight:bold;font-famliy:Arial;text-align:center;'>ASTREEVIEW DEBUG MODE</div>";
				this.divDebugContainer.Controls.Add( divDebugIndicator );
			}

			this.divTreeViewContainer.Controls.Add( this.divDebugContainer );

			#region divAjaxIndicatorContainer

			this.divTreeViewContainer.Controls.Add( this.divAjaxIndicatorContainer );
			this.divAjaxIndicatorContainer.Attributes.Add( "class", this.CssClassAjaxIndicatorContainer );
			this.divAjaxIndicatorContainer.Style.Add( "display", "none" );
			this.divAjaxIndicatorContainer.InnerHtml = this.AjaxIndicatorText;

			if( this.EnableContextMenu )
				this.divTreeViewContainer.Controls.Add( this.ascmContextMenu );

			#endregion

			AddTreeViewToControlSet();
		}

		#endregion

		#region AddTreeViewToControlSet

		/// <summary>
		/// Adds the tree view to control set.
		/// </summary>
		virtual protected void AddTreeViewToControlSet()
		{
			this.Controls.Add( divTreeViewContainer );
		}

		#endregion
		
		#region ManageRootNodePostBack

		/// <summary>
		/// Manages the root node post back.
		/// </summary>
		protected virtual void ManageRootNodePostBack()
		{
			if( this.IsInPostback() )
			{
				if( this.EnableRoot && this.RootNode.ChildNodes.Count > 0 )
				{
					this.rootNode = this.RootNode.ChildNodes[0];
					this.rootNode.ParentNode = null;
				}

				LoadIsBound();
			}
		}
		#endregion

		#region ManageRootNodeProperty

		/// <summary>
		/// Manages the root node property.
		/// </summary>
		protected virtual void ManageRootNodeProperty()
		{
			//generate root
			this.rootNode.NodeText = this.RootNodeText;// "RootNodeValue";
			this.rootNode.NodeValue = this.RootNodeValue;//"asTreeViewRoot";
			this.rootNode.EnableEditContextMenu = false;
			this.rootNode.EnableDeleteContextMenu = false;
		}

		#endregion

		#region GenerateTree

		/// <summary>
		/// Generates the tree.
		/// </summary>
		virtual protected void GenerateTree()
		{
			//manage EnableLeafOnlyCheckbox
			if( this.EnableLeafOnlyCheckbox )
			{
				ASTreeNodeHandlerDelegate leafDelegate = delegate( ASTreeViewNode node )
				{
					//if node is parent
					if( node.ChildNodes.Count != 0 )
						node.EnableCheckbox = false;
				};

				this.TraverseTreeNode( this.rootNode, leafDelegate );
			}

			//manage checkbox
			if( this.EnableThreeStateCheckbox )
				treeViewHelper.ManageCheckboxState( this.RootNode );

			//manage expand depth
			if( !IsInPostback() )
				ManageExpendDepth();

			//manage tree node depth
			if( this.EnableFixedDepthDragDrop )
				GenerateAllNodesDepth();

			//generate tree
			treeViewHelper.ConvertTree( this.ulASTreeView, this.RootNode, this.EnableRoot );
			this.divTreeViewContainer.Controls.Add( ulASTreeView );

			//set container data
			//modified by weijie. we don't need to set the txt data
			if( !IsInPostback() )
			{
				/*
				if( this.EnablePersistentTreeNodesOnFirstLoad )
					this.txtASTreeViewNodes.Value = treeViewHelper.GetNodeJsonString( this.RootNode );
				*/
				if( this.EnableTreeNodesViewState )
					this.TreeNodesState = treeViewHelper.GetNodeJsonString( this.RootNode );
			}
			//else if( !this.EnableTreeNodesViewState )
			//	this.txtASTreeViewNodes.Value = string.Empty;//treeViewHelper.GetNodeJsonString( this.RootNode );

		}



		#endregion

		#region GenerateAllNodesDepth
		/// <summary>
		/// Generates all nodes depth.
		/// </summary>
		virtual protected void GenerateAllNodesDepth()
		{
			ASTreeNodeHandlerDelegate nodeDelegate = delegate( ASTreeViewNode node )
			{
				//set the first level
				if( ( node == this.rootNode && this.EnableRoot )
					|| ( !this.EnableRoot && node.ParentNode == this.rootNode ) )
				{
					node.NodeDepth = 1;
				}
				else if( node != this.rootNode ) //other nodes except root
					node.NodeDepth = node.ParentNode.NodeDepth + 1;
			};

			this.TraverseTreeNode( this.rootNode, nodeDelegate );
		}
		#endregion

		#region ManageExpendDepth

		/// <summary>
		/// Manages the expend depth.
		/// </summary>
		virtual protected void ManageExpendDepth()
		{
			ExpandToDepth( this.ExpandDepth );
		}

		#endregion

		#region ExpandNode

		/// <summary>
		/// Expands the node.
		/// </summary>
		/// <param name="node">The node.</param>
		/// <param name="depth">The depth.</param>
		/// <param name="curDepth">The cur depth.</param>
		virtual protected void ExpandNode( ASTreeViewNode node, int depth, int curDepth )
		{
			if( curDepth >= depth )
				return;

			node.OpenState = ASTreeViewNodeOpenState.Open;
			curDepth++;

			foreach( ASTreeViewNode child in node.ChildNodes )
				ExpandNode( child, depth, curDepth );
		}

		#endregion

		#region GenerateContextMenu

		/// <summary>
		/// Generates the context menu.
		/// </summary>
		virtual protected void GenerateContextMenu()
		{

			this.ascmContextMenu.AttachCssClass = this.ContextMenuTargetCssClass;
			this.ascmContextMenu.EnableDebugMode = this.EnableDebugMode;
			//GetContextMenuObjectId()
			string addJS = string.Format( "{0}.addItem(event, {1});return false;"
				, this.GetClientTreeObjectId()
				, this.ascmContextMenu.GetContextMenuObjectId() + ".getSelectedItem()" );

			string editJS = string.Format( "{0}.editItem(event, {1});return false;"
				, this.GetClientTreeObjectId()
				, this.ascmContextMenu.GetContextMenuObjectId() + ".getSelectedItem()" );

			string deleteJS = string.Format( "{0}.deleteItem(event, {1});return false;"
				, this.GetClientTreeObjectId()
				, this.ascmContextMenu.GetContextMenuObjectId() + ".getSelectedItem()" );

			this.ascmContextMenu.MenuItems.Add( new ASContextMenuItem( this.ContextMenuAddText, addJS, this.ContextMenuAddCommandName ) );
			this.ascmContextMenu.MenuItems.Add( new ASContextMenuItem( this.ContextMenuEditText, editJS, this.ContextMenuEditCommandName ) );
			this.ascmContextMenu.MenuItems.Add( new ASContextMenuItem( this.ContextMenuDeleteText, deleteJS, this.ContextMenuDeleteCommandName ) );
		}

		#endregion

		#region SetStyle

		/// <summary>
		/// Sets the style.
		/// </summary>
		virtual protected void SetStyle()
		{
			this.ulASTreeView.Attributes.Add( "class", this.CssClass );
		}

		#endregion

		#region IsControlPostbackKey

		/// <summary>
		/// Determines whether [is control postback key] [the specified control unique id].
		/// </summary>
		/// <param name="controlUniqueId">The control unique id.</param>
		/// <param name="postbackKey">The postback key.</param>
		/// <returns>
		/// 	<c>true</c> if [is control postback key] [the specified control unique id]; otherwise, <c>false</c>.
		/// </returns>
		virtual protected bool IsControlPostbackKey( string controlUniqueId, string postbackKey )
		{
			if( postbackKey == null )
				return false;

			return controlUniqueId == postbackKey;
			/*
			string[] tokens = postbackKey.Split( '$' );
			if( tokens.Length < 2 )
				return false;

			if( tokens[tokens.Length - 2] == this.ID && tokens[tokens.Length - 1] == controlUniqueId )
				return true;
			else
				return false;
			*/
		}

		#endregion

		#region GenerateOnNodeDragAndDropStartScript

		/// <summary>
		/// Generates the on node drag drop start script.
		/// </summary>
		/// <returns></returns>
		virtual protected string GenerateOnNodeDragAndDropStartScript()
		{
			string script = string.Format( @"
		function astreeviewNodeDragDropStart{0}( elem ){{
				{2}try{{
					{3}{1}
				{2}}}catch(err){{}}
			}}
", this.ClientID /*0*/
 , this.OnNodeDragAndDropStartScript /*1*/
 , this.EnableManageJSError ? "" : "//" /*2*/
 , this.EnableOnNodeDragAndDropStartScriptReturn ? "return " : string.Empty /*3*/ );

			return script;
		}

		#endregion

		#region GenerateOnNodeDragAndDropCompletingScript

		/// <summary>
		/// Generates the on node drag drop complete script.
		/// </summary>
		/// <returns></returns>
		virtual protected string GenerateOnNodeDragAndDropCompletingScript()
		{
			string script = string.Format( @"
		function astreeviewNodeDragDropCompleting{0}( elem, newParent ){{
				//alert(node.innerHTML);
				{2}try{{
					{3}{1}
				{2}}}catch(err){{}}
			}}
", this.ClientID /*0*/
 , this.OnNodeDragAndDropCompletingScript /*1*/
 , this.EnableManageJSError ? "" : "//" /*2*/
 , this.EnableOnNodeDragAndDropCompletingScriptReturn ? "return " : string.Empty /*3*/ );

			return script;
		}

		#endregion

		#region GenerateOnNodeDragAndDropCompletedScript

		/// <summary>
		/// Generates the on node drag drop complete script.
		/// </summary>
		/// <returns></returns>
		virtual protected string GenerateOnNodeDragAndDropCompletedScript()
		{
			string script = string.Format( @"
		function astreeviewNodeDragDropCompleted{0}( elem, newParent ){{
				//alert(node.innerHTML);
				{2}try{{
					{1}
				{2}}}catch(err){{}}
			}}
", this.ClientID /*0*/
 , this.OnNodeDragAndDropCompletedScript /*1*/
 , this.EnableManageJSError ? "" : "//" /*2*/ );

			return script;
		}

		#endregion
		
		#region GenerateOnNodeAddedScript

		/// <summary>
		/// Generates the on node added script.
		/// </summary>
		/// <returns></returns>
		virtual protected string GenerateOnNodeAddedScript()
		{
			string script = string.Format( @"
			function astreeviewNodeAdded{0}( elem ){{
				{2}try{{
					{3}{1}
				{2}}}catch(err){{}}
			}}
", this.ClientID /*0*/
 , this.OnNodeAddedScript /*1*/
 , this.EnableManageJSError ? "" : "//" /*2*/
 , this.EnableOnNodeAddedScriptReturn ? "return " : string.Empty /*3*/);

			return script;
		}

		#endregion

		#region GenerateOnNodeEditedScript

		/// <summary>
		/// Generates the on node edited script.
		/// </summary>
		/// <returns></returns>
		virtual protected string GenerateOnNodeEditedScript()
		{
			string script = string.Format( @"
			function astreeviewNodeNodeEdited{0}( elem, info ){{
				{2}try{{
					{3}{1}
				{2}}}catch(err){{}}
			}}
", this.ClientID /*0*/
 , this.OnNodeEditedScript /*1*/
 , this.EnableManageJSError ? "" : "//" /*2*/
 , this.EnableOnNodeEditedScriptReturn ? "return " : string.Empty /*3*/ );

			return script;
		}

		#endregion

		#region GenerateOnNodeDeletedScript

		/// <summary>
		/// Generates the on node deleted script.
		/// </summary>
		/// <returns></returns>
		virtual protected string GenerateOnNodeDeletedScript()
		{
			string script = string.Format( @"
			function astreeviewNodeDeleted{0}( val, info ){{
				{2}try{{
					{3}{1}
				{2}}}catch(err){{}}
			}}
", this.ClientID /*0*/
 , this.OnNodeDeletedScript /*1*/
 , this.EnableManageJSError ? "" : "//" /*2*/
 , this.EnableOnNodeDeletedScriptReturn ? "return " : string.Empty /*3*/ );

			return script;
		}

		#endregion

		#region GenerateOnNodeOpenedAndClosedScript

		/// <summary>
		/// Generates the on node opened and closed script.
		/// </summary>
		/// <returns></returns>
		private string GenerateOnNodeOpenedAndClosedScript()
		{
			string script = string.Format( @"
			function astreeviewNodeOpenedAndClosed{0}( state, elem ){{
				{2}try{{
					{3}{1}
				{2}}}catch(err){{}}
			}}
", this.ClientID /*0*/
 , this.OnNodeOpenedAndClosedScript /*1*/
 , this.EnableManageJSError ? "" : "//" /*2*/
 , this.EnableOnNodeOpenedAndClosedReturn ? "return " : string.Empty /*3*/ );

			return script;
		}

		#endregion

		#region GenerateAddNodeDataValueProviderScript

		/// <summary>
		/// Generates the add node data value provider script.
		/// </summary>
		/// <returns></returns>
		virtual protected string GenerateAddNodeDataValueProviderScript()
		{
			string script = string.Format( @"
		function astreeviewAddNodeDataValueProvider{0}( elem ){{
				{2}try{{
					{1}
					return 'New Node';
				{2}}}catch(err){{}}
			}}
", this.ClientID /*0*/
 , this.AddNodeDataValueProvider /*1*/
 , this.EnableManageJSError ? "" : "//" /*2*/);

			return script;
		}

		#endregion

		#region GenerateOnNodeSelectedScript

		/// <summary>
		/// Generates the on node selected script.
		/// </summary>
		/// <returns></returns>
		virtual protected string GenerateOnNodeSelectedScript()
		{
			string script = string.Format( @"
		function astreeviewOnNodeSelected{0}(e){{
				var evt = e || window.event;
				var elem = evt.srcElement || evt.target;
				
				if( !elem.getAttribute('is-astreeview-node') )
					elem = _gt.ASTreeViewHelper.findRealTarget( elem );

				//alert(elem.tagName);				

				if('{1}'==''||!_gt.$('{1}'))
					return;
				_gt.$('{1}').value = encodeURIComponent( elem.innerHTML ) + '{2}' + encodeURIComponent( elem.parentNode.getAttribute('treenodevalue') );
				
				_gt.$('{4}').value = _gt.$('{1}').value;

				{5}try{{
					{3}
				{5}}}catch(err){{}}
			}}
", this.ClientID /*0*/
 , this.UniqueID + this.selectedNodeTextboxSurfix/*1, select node textbox id*/
 , this.Separator /*2*/
 , this.OnNodeSelectedScript /*3*/
 , this.txtSelectedASTreeViewNodes.ClientID /*4*/
 , this.EnableManageJSError ? "" : "//" /*5*/);

			return script;
		}

		#endregion

		#region GenerateOnNodeCheckedScript

		/// <summary>
		/// Generates the on node checked script.
		/// </summary>
		/// <returns></returns>
		virtual protected string GenerateOnNodeCheckedScript()
		{
			string script = string.Format( @"
		function astreeviewOnNodeChecked{0}(e){{
				var evt = e || window.event;
				var elem = evt.srcElement || evt.target;
				
				//if( !elem.getAttribute('is-astreeview-node') )
				//	elem = _gt.ASTreeViewHelper.findRealTarget( elem );				

				if('{1}'==''||!_gt.$('{1}'))
					return;
				var nodeText = '';
				var nodeA = asTreeViewObj{0}.getFirstChildElement( elem.parentNode, 'A' );
				if( nodeA )
					nodeText = nodeA.innerHTML		

				_gt.$('{1}').value = encodeURIComponent( nodeText ) +'{2}'+ encodeURIComponent( elem.parentNode.getAttribute('treenodevalue') ) +'{2}'+ encodeURIComponent( elem.parentNode.getAttribute('checkedState') );
				_gt.$('{5}').value = _gt.$('{1}').value;

				{6}try{{
					{3}
				{6}}}catch(err){{}}

				if({4}){{
					var checkboxPostbackScript = elem.getAttribute('postbackscript');
					if( checkboxPostbackScript ){{
						eval(checkboxPostbackScript);
					}}
				}}
			}}
", this.ClientID /*0*/
 , this.UniqueID + this.checkedNodeTextboxSurfix/*1, checked node textbox id*/
 , this.Separator /*2*/
 , this.OnNodeCheckedScript /*3*/
 , this.AutoPostBack ? "true" : "false" /*4*/
 , this.txtCheckedASTreeViewNodes.ClientID /*5*/
 , this.EnableManageJSError ? "" : "//" /*6*/);

			return script;
		}

		#endregion

		#region GenerateOnNodesBoundScript

		/// <summary>
		/// Generates the on nodes boundcript.
		/// </summary>
		/// <returns></returns>
		virtual protected string GenerateOnNodesBoundScript()
		{
			string script = string.Format( @"
function astreeviewOnNodesBound{0}(){{
	_gt.$('{1}').value = '{2}';
}}
", this.ClientID /*0*/
	, this.txtIsBound.ClientID
	, true.ToString() );

			return script;
		}

		#endregion

		#region GenerateIsNodesBoundScript

		/// <summary>
		/// Generates the is nodes bound script.
		/// </summary>
		/// <returns></returns>
		virtual protected string GenerateIsNodesBoundScript()
		{
			string script = string.Format( @"
function astreeviewIsNodesBound{0}(){{
	return _gt.$('{1}').value != '{2}';
}}
", this.ClientID /*0*/
	, this.txtIsBound.ClientID
	, false.ToString() );

			return script;
		}

		#endregion

		#region GenerateASTreeViewInitialScript

		/// <summary>
		/// Generates the ASTreeView initial script.
		/// </summary>
		/// <returns></returns>
		virtual protected string GenerateASTreeViewInitialScript()
		{
			#region nodeSelectScript

			string nodeSelectScriptBlock = this.GenerateOnNodeSelectedScript();

			#endregion

			#region nodeCheckedScript

			string nodeCheckedScriptBlock = this.GenerateOnNodeCheckedScript();

			#endregion

			#region nodeDragDropStartScript

			string nodeDragDropStartScriptBlock = this.GenerateOnNodeDragAndDropStartScript();

			#endregion

			#region nodeDragDropCompletingScript

			string nodeDragDropCompletingScriptBlock = this.GenerateOnNodeDragAndDropCompletingScript();

			#endregion

			#region nodeDragDropCompletedScript

			string nodeDragDropCompletedScriptBlock = this.GenerateOnNodeDragAndDropCompletedScript();

			#endregion

			#region nodeAddedScript

			string nodeAddedScriptBlock = this.GenerateOnNodeAddedScript();

			#endregion

			#region nodeEditedScript

			string nodeEditedScriptBlock = this.GenerateOnNodeEditedScript();

			#endregion

			#region nodeDeletedScript

			string nodeDeletedScriptBlock = this.GenerateOnNodeDeletedScript();

			#endregion

			#region nodeOpenedAndClosedScript

			string nodeOpenedAndClosedScriptBlock = this.GenerateOnNodeOpenedAndClosedScript();

			#endregion

			#region AddNodeDataValueProvider

			string addNodeDataValueProviderScriptBlock = string.IsNullOrEmpty( this.AddNodeDataValueProvider )
				? string.Empty : GenerateAddNodeDataValueProviderScript();

			#endregion

			#region onNodesBound

			string onNodesBoundScriptBlock = this.GenerateOnNodesBoundScript();

			#endregion

			#region isNodesBound

			string isNodesBoundScriptBlock = this.GenerateIsNodesBoundScript();

			#endregion

			return string.Format( @"
<script type=""text/javascript"">
{0}
{1}
{2}
{3}
{4}
{5}
{6}
{7}
{8}
{9}
{10}
{11}
</script>
", nodeSelectScriptBlock /*0, 7*/
 , nodeCheckedScriptBlock /*1, 18*/
 , nodeDragDropCompletingScriptBlock /*2, 51*/
 , nodeDragDropStartScriptBlock /*3, 59*/
 , nodeAddedScriptBlock /*4, 62*/
 , nodeEditedScriptBlock /*5, 65*/
 , nodeDeletedScriptBlock /*6, 67*/
 , addNodeDataValueProviderScriptBlock /*7, 68*/
 , nodeOpenedAndClosedScriptBlock /*8, 76*/
 , nodeDragDropCompletedScriptBlock /*9, 77*/
 , onNodesBoundScriptBlock /*10, 78*/
 , isNodesBoundScriptBlock /*11, 79*/);
		}



		#endregion

		#region GenerateASTreeViewScript

		/// <summary>
		/// Generates the AS tree view script.
		/// </summary>
		/// <returns></returns>
		virtual protected string GenerateASTreeViewScript()
		{
			#region nodeSelectScript

			string nodeSelectScriptBlock = string.Empty;//this.GenerateOnNodeSelectedScript();
			string nodeSelectScriptSetup = string.Format( @"asTreeViewObj{0}.onNodeSelected=astreeviewOnNodeSelected{0};", this.ClientID /*0*/ );

			#endregion

			#region nodeCheckedScript

			string nodeCheckedScriptBlock = string.Empty;//this.GenerateOnNodeCheckedScript();
			string nodeCheckedScriptSetup = string.Format( @"asTreeViewObj{0}.onNodeChecked=astreeviewOnNodeChecked{0};", this.ClientID /*0*/ );

			#endregion

			#region nodeDragDropStartScript

			string nodeDragDropStartScriptBlock = string.Empty;//this.GenerateOnNodeDragAndDropStartScript();
			string nodeDragDropStartScriptSetup = string.Format( @"asTreeViewObj{0}.onDragDropStart=astreeviewNodeDragDropStart{0};", this.ClientID /*0*/ );

			#endregion

			#region nodeDragDropCompletingScript

			string nodeDragDropCompletingScriptBlock = string.Empty;//this.GenerateOnNodeDragAndDropCompletingScript();
			string nodeDragDropCompletingScriptSetup = string.Format( @"asTreeViewObj{0}.onDragDropCompleting=astreeviewNodeDragDropCompleting{0};", this.ClientID /*0*/ );

			#endregion

			#region nodeDragDropCompletedScript

			string nodeDragDropCompletedScriptBlock = string.Empty;//this.GenerateOnNodeDragAndDropCompletedScript();
			string nodeDragDropCompletedScriptSetup = string.Format( @"asTreeViewObj{0}.onDragDropCompleted=astreeviewNodeDragDropCompleted{0};", this.ClientID /*0*/ );

			#endregion

			#region nodeAddedScript

			string nodeAddedScriptBlock = string.Empty;//this.GenerateOnNodeAddedScript();
			string nodeAddedScriptSetup = string.Format( @"asTreeViewObj{0}.onNodeAdded=astreeviewNodeAdded{0};", this.ClientID /*0*/ );

			#endregion

			#region nodeEditedScript

			string nodeEditedScriptBlock = string.Empty;//this.GenerateOnNodeEditedScript();
			string nodeEditedScriptSetup = string.Format( @"asTreeViewObj{0}.onNodeEdited=astreeviewNodeNodeEdited{0};", this.ClientID /*0*/ );

			#endregion

			#region onNodesBoundScript

			string onNodesBoundScriptSetup = string.Format( @"asTreeViewObj{0}.onNodesBound=astreeviewOnNodesBound{0};", this.ClientID /*0*/ );

			#endregion

			#region isNodesBoundScript

			string isNodesBoundScriptSetup = string.Format( @"asTreeViewObj{0}.isNodesBound=astreeviewIsNodesBound{0};", this.ClientID /*0*/ );

			#endregion

			#region nodeDeletedScript

			string nodeDeletedScriptBlock = string.Empty;//this.GenerateOnNodeDeletedScript();
			string nodeDeletedScriptSetup = string.Format( @"asTreeViewObj{0}.onNodeDeleted=astreeviewNodeDeleted{0};", this.ClientID /*0*/ );

			#endregion

			#region AddNodeDataValueProvider

			string addNodeDataValueProviderScriptBlock = string.Empty;//string.IsNullOrEmpty( this.AddNodeDataValueProvider ) ? string.Empty : GenerateAddNodeDataValueProviderScript();
			string addNodeDataValueProviderScriptSetup = string.IsNullOrEmpty( this.AddNodeDataValueProvider )
				? string.Empty : string.Format( @"asTreeViewObj{0}.addNodeDataValueProvider=astreeviewAddNodeDataValueProvider{0}", this.ClientID /*0*/ );

			#endregion

			#region related trees script

			string relatedTreesScript = string.Empty;
			StringBuilder sbRelatedTreesArray = new StringBuilder();
			if( this.RelatedTrees.Length > 0 )
			{
				string[] relatedTrees = this.RelatedTrees.Split( ',' );
				if( relatedTrees.Length > 0 )
				{
					sbRelatedTreesArray.Append( "[" );
					bool scriptCreated = false;
					foreach( string treeId in relatedTrees )
					{
						if( string.IsNullOrEmpty( treeId ) )
							continue;

						ASTreeView curTreeView = ASTreeViewHelper.FindControlRecursive( this.Page, treeId ) as ASTreeView;//this.Page.FindControl( treeId ) as ASTreeView;
						if( curTreeView != null )
						{
							sbRelatedTreesArray.Append( string.Format( @"""{0}"",", curTreeView.GetClientTreeObjectId() ) );
							scriptCreated = true;
						}
					}

					sbRelatedTreesArray.Remove( sbRelatedTreesArray.Length - 1, 1 );
					sbRelatedTreesArray.Append( "]" );

					relatedTreesScript = scriptCreated ? string.Format( @"
asTreeViewObj{0}.setRelatedTrees({1});
", this.ClientID /*0*/
 , sbRelatedTreesArray.ToString() /*1*/ ) : string.Empty;

				}
			}

			#endregion

			#region nodeOpenedAndClosedScript

			string nodeOpenedAndClosedScriptBlock = string.Empty;
			string nodeOpenedAndClosedScriptSetup = string.Format( @"asTreeViewObj{0}.onNodeOpenedAndClosed=astreeviewNodeOpenedAndClosed{0};", this.ClientID /*0*/ );

			#endregion

			string script = string.Format( @"
<script type=""text/javascript"">
if( !(_gt && _gt.ASTreeView ) ){{
	alert('missing script file [_gt.ASTreeView]!');
}}
else{{

	{7}
	{18}
	{51}
	{59}
	{62}
	{65}
	{67}
	{69}

	var asTreeViewObj{0};
	function initialASTreeView{0}(){{
		asTreeViewObj{0} = new _gt.ASTreeView(""asTreeViewObj{0}"");
		asTreeViewObj{0}.setTreeId('{1}');
		asTreeViewObj{0}.setMaxDepth({29});
		asTreeViewObj{0}.setMaxDepthReachedMessage('{30}');
		asTreeViewObj{0}.setEnableCheckbox( {10} );
		asTreeViewObj{0}.setEnableNodeSelection( {9} );
		//asTreeViewObj{0}.expandAll();
		asTreeViewObj{0}.setNodesJsonStringContainerId('{3}');
		asTreeViewObj{0}.setEnableRoot({4})
		asTreeViewObj{0}.setRootNodeId('{5}');
		asTreeViewObj{0}.setEnableParentNodeSelection({6});
		asTreeViewObj{0}.setEnableDragDrop({11})
		asTreeViewObj{0}.setEnableTreeLines({12})
		asTreeViewObj{0}.setCssClassLineVertical('{13}');
		asTreeViewObj{0}.setCssClassLineRoot('{27}');
		asTreeViewObj{0}.setCssClassLineTop('{28}');
		asTreeViewObj{0}.setCssClassLineMiddle('{14}');
		asTreeViewObj{0}.setCssClassLineBottom('{15}');
		asTreeViewObj{0}.setCssClassLineNone('{16}');
		asTreeViewObj{0}.setEnableSaveStateEveryStep({20});
		//asTreeViewObj{0}.setImageFolder('{21}');
		asTreeViewObj{0}.setEnableCustomizedNodeIcon({22});
		asTreeViewObj{0}.setEnableAjaxOnEditDelete({23});
		asTreeViewObj{0}.setEditNodeProvider('{24}');
		asTreeViewObj{0}.setDeleteNodeProvider('{25}');
		asTreeViewObj{0}.setAjaxIndicatorContainerId('{26}');
		asTreeViewObj{0}.setAdditionalEditRequestParameters({31});
		asTreeViewObj{0}.setAdditionalDeleteRequestParameters({32});
		asTreeViewObj{0}.setAdditionalLoadNodesRequestParameters({33});
		asTreeViewObj{0}.setDeleteNodePromptMessage('{34}');
		asTreeViewObj{0}.setDeleteNodeWithSubPromptMessage('{35}');
		asTreeViewObj{0}.setAddNodeProvider('{36}');
		asTreeViewObj{0}.setAddNodePromptMessage('{37}');
		asTreeViewObj{0}.setAddNodePromptDefaultValue('{61}');
		asTreeViewObj{0}.setAdditionalAddRequestParameters({38});
		asTreeViewObj{0}.setLoadNodesProvider('{39}');
		asTreeViewObj{0}.setEnableParentNodeExpand({40});

		//set icons
		asTreeViewObj{0}.setFolderImage('{41}');
		asTreeViewObj{0}.setFolderOpenImage('{42}');
		asTreeViewObj{0}.setTreeNodeImage('{43}');
		asTreeViewObj{0}.setPlusImage('{44}');
		asTreeViewObj{0}.setMinusImage('{45}');
		asTreeViewObj{0}.setCheckboxUncheckedImage('{46}');
		asTreeViewObj{0}.setCheckboxCheckedImage('{47}');
		asTreeViewObj{0}.setCheckboxHalfCheckedImage('{48}');
		asTreeViewObj{0}.setDragDropIndicator('{49}');
		asTreeViewObj{0}.setDragDropIndicatorSub('{50}');
		asTreeViewObj{0}.setEnableMultiLineEdit({53});
		asTreeViewObj{0}.setEnableEscapeInput({54});
		asTreeViewObj{0}.setEnableStripAjaxResponse({55});
		asTreeViewObj{0}.setStripAjaxResponseRegex('{56}');
		asTreeViewObj{0}.setEnableFixedDepthDragDrop({57});
		asTreeViewObj{0}.setEnableHorizontalLock({58});
		asTreeViewObj{0}.setEnableContainerDragDrop({64});
		asTreeViewObj{0}.setEnableThreeStateCheckbox({71});
		asTreeViewObj{0}.setEnableRightToLeftRender({72});
		asTreeViewObj{0}.setEnableFixedParentDragDrop({73});
		asTreeViewObj{0}.setEnablePersistentTreeState({74});
		asTreeViewObj{0}.setEnableDragDropOnIcon({75});
		{17}
		{8}
		{19}
		{52}
		{60}
		{63}
		{66}
		{68}
		{70}
		{76}
		{77}
		{78}
		{79}
		
		asTreeViewObj{0}.initializeTree();

		//_gt.$('{2}').value = asTreeViewObj{0}.getTreeViewJSON().toJSONString();
		//asTreeViewObj{0}.processCheckboxWholeTree();
	}}
}}
</script>
", this.ClientID /*0*/
 , this.ulASTreeView.ClientID /*1*/
 , this.txtASTreeViewNodes.ClientID /*2*/
 , this.txtASTreeViewNodes.ClientID /*3*/
 , this.EnableRoot ? "true" : "false" /*4*/
 , this.EnableRoot ? ( (HtmlGenericControl)this.ulASTreeView.Controls[0] ).ClientID : string.Empty /*5*/
 , this.EnableParentNodeSelection ? "true" : "false" /*6*/
 , nodeSelectScriptBlock /*7*/
 , nodeSelectScriptSetup /*8*/
 , this.EnableNodeSelection ? "true" : "false" /*9*/
 , this.EnableCheckbox ? "true" : "false" /*10*/
 , this.EnableDragDrop ? "true" : "false" /*11*/
 , this.EnableTreeLines ? "true" : "false" /*12*/
 , this.CssClassLineVertical /*13*/
 , this.CssClassLineMiddle /*14*/
 , this.CssClassLineBottom /*15*/
 , this.CssClassLineNone /*16*/
 , relatedTreesScript /*17*/
 , nodeCheckedScriptBlock /*18*/
 , nodeCheckedScriptSetup /*19*/
 , this.EnableSaveStateEveryStep ? "true" : "false" /*20*/
 , this.EnableTheme ? this.Page.ResolveUrl( this.Theme.ImagePath ) : this.ImagePath/*this.BasePath + "images/"*/ /*21*/
 , this.EnableCustomizedNodeIcon ? "true" : "false" /*22*/
 , this.EnableAjaxOnEditDelete ? "true" : "false" /*23*/
 , this.EditNodeProvider /*24*/
 , this.DeleteNodeProvider /*25*/
 , this.AjaxIndicatorContainerID /*26*/
 , this.CssClassLineRoot /*27*/
 , this.CssClassLineTop /*28*/
 , this.MaxDepth /*29*/
 , string.Format( this.MaxDepthReachedMessage, this.MaxDepth )/*30*/
 , this.AdditionalEditRequestParameters /*31*/
 , this.AdditionalDeleteRequestParameters /*32*/
 , this.AdditionalLoadNodesRequestParameters /*33*/
 , this.DeleteNodePromptMessage /*34*/
 , this.DeleteNodeWithSubPromptMessage /*35*/
 , this.AddNodeProvider /*36*/
 , this.AddNodePromptMessage /*37*/
 , this.AdditionalAddRequestParameters /*38*/
 , this.LoadNodesProvider /*39*/
 , this.EnableParentNodeExpand ? "true" : "false" /*40*/
 , this.DefaultFolderIcon /*41*/
 , this.DefaultFolderOpenIcon /*42*/
 , this.DefaultNodeIcon /*43*/
 , this.ImgPlusIcon /*44*/
 , this.ImgMinusIcon /*45*/
 , this.ImgCheckboxUnchecked /*46*/
 , this.ImgCheckboxChecked /*47*/
 , this.ImgCheckboxHalfChecked /*48*/
 , this.ImgDragDropIndicator /*49*/
 , this.ImgDragDropIndicatorSub /*50*/
 , nodeDragDropCompletingScriptBlock /*51*/
 , nodeDragDropCompletingScriptSetup /*52*/
 , this.EnableMultiLineEdit ? "true" : "false" /*53*/
 , this.EnableEscapeInput ? "true" : "false" /*54*/
 , this.EnableStripAjaxResponse ? "true" : "false" /*55*/
 , this.StripAjaxResponseRegex /*56*/
 , this.EnableFixedDepthDragDrop ? "true" : "false" /*57*/
 , this.EnableHorizontalLock ? "true" : "false" /*58*/
 , nodeDragDropStartScriptBlock /*59*/
 , nodeDragDropStartScriptSetup /*60*/
 , this.AddNodePromptDefaultValue /*61*/
 , addNodeDataValueProviderScriptBlock /*62*/
 , addNodeDataValueProviderScriptSetup /*63*/
 , this.EnableContainerDragDrop ? "true" : "false" /*64*/
 , nodeAddedScriptBlock /*65*/
 , nodeAddedScriptSetup /*66*/
 , nodeEditedScriptBlock /*67*/
 , nodeEditedScriptSetup /*68*/
 , nodeDeletedScriptBlock /*69*/
 , nodeDeletedScriptSetup /*70*/
 , this.EnableThreeStateCheckbox ? "true" : "false" /*71*/
 , this.EnableRightToLeftRender ? "true" : "false" /*72*/
 , this.EnableFixedParentDragDrop ? "true" : "false" /*73*/
 , this.EnablePersistentTreeState ? "true" : "false" /*74*/
 , this.EnableDragDropOnIcon ? "true" : "false" /*75*/
 , nodeOpenedAndClosedScriptSetup  /*76*/
 , nodeDragDropCompletedScriptSetup /*77*/
 , onNodesBoundScriptSetup /*78*/
 , isNodesBoundScriptSetup /*79*/);

			return script;
		}

		#endregion

		#region ProcessImageUrl

		/// <summary>
		/// Processes the image URL.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <returns></returns>
		virtual protected string ProcessImageUrl( string url )
		{
			if( url != null && url.StartsWith( "~" ) )
				return this.Page.ResolveUrl( url );
			else
				return url;
		}

		#endregion 

		#endregion

		#region Private Methods

		#endregion

		#region Override Methods

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Load"></see> event.
		/// </summary>
		/// <param name="e">The <see cref="T:System.EventArgs"></see> object that contains the event data.</param>
		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );

			ManageRootNodePostBack();
		}

		#region LoadIsBound

		/// <summary>
		/// Loads the is bound.
		/// </summary>
		private void LoadIsBound()
        {
            if (!string.IsNullOrEmpty(txtIsBound.Value))
			    this.IsBound = bool.Parse( txtIsBound.Value );
		}

		#endregion

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Init"></see> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"></see> object that contains the event data.</param>
		protected override void OnInit( EventArgs e )
		{
			base.OnInit( e );

			InitializeControls();
		}

		/// <summary>
		/// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
		/// </summary>
		protected override void CreateChildControls()
		{
			base.CreateChildControls();

			CreateTreeControls();
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.PreRender"></see> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"></see> object that contains the event data.</param>
		protected override void OnPreRender( EventArgs e )
		{
			base.OnPreRender( e );

			SetIsBound();

			ManageRootNodeProperty();

			GenerateTree();

			GenerateContextMenu();

			SetStyle();

			string script = GenerateASTreeViewScript();
			string initialSetupScript = GenerateASTreeViewInitialScript();

			
			if( !this.EnableSaveStateEveryStep )
			{
				string stringToWrite = string.Format( "asTreeViewObj{0}.saveTreeClientState();", this.ClientID );

				if( !this.EnableDebugMode )
					stringToWrite = treeViewHelper.ObfuscateScript( stringToWrite );

				if( ScriptManager.GetCurrent( this.Page ) != null )
				{
					bool needRenderSubmitStatement =
						( IsControlInPartialRendering() || !this.IsInUpdateAsyncPostback() || this.forceRenderInitialScript );

					if( needRenderSubmitStatement )
					{
						ScriptManager.RegisterOnSubmitStatement( this.Page
							, this.Page.GetType()
							, "js" + Guid.NewGuid()
							, stringToWrite );
					}
				}
				else
				{
					this.Page.ClientScript.RegisterOnSubmitStatement( this.Page.GetType()
						, "js" + Guid.NewGuid()
						, stringToWrite );
				}
			}

			if( ScriptManager.GetCurrent( this.Page ) != null )
			{
				if( !this.IsInUpdateAsyncPostback() || this.forceRenderInitialScript )
				{
					ScriptManager.RegisterStartupScript( this.Page
						, this.Page.GetType()
						, "js" + Guid.NewGuid()
						, initialSetupScript
						, false );
				}

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
									, initialSetupScript );

				this.Page.ClientScript.RegisterStartupScript( this.Page.GetType()
					, "js" + Guid.NewGuid()
					, script );
			}


			#region register intial script

			string postbackScript = string.Format( @"
				<script type='text/javascript'>initialASTreeView{0}();</script>
", this.ClientID /*0*/ );

			string initialScript = string.Format( @"
<script type='text/javascript'>
if( _gt.ASTreeViewHelper.isIE ){{
	if (window.addEventListener) {{
		window.addEventListener('load', initialASTreeView{0}, false );
	}}
	else if (window.attachEvent) {{
		var r = window.attachEvent('onload', initialASTreeView{0} );
	}}
	else {{
		window['onload'] = initialASTreeView{0};
	}}
}}
else
	initialASTreeView{0}();
</script>
", this.ClientID /*0*/  );

			
			if( this.IsInPostback() )
			{

				if( this.IsInUpdateAsyncPostback() )
				{
					if( IsControlInPartialRendering() || this.forceRenderInitialScript )
					{
						ScriptManager.RegisterStartupScript( this.Page
							, this.Page.GetType()
							, "js" + Guid.NewGuid()
							, postbackScript
							, false );
					}
				}
				else
				{

					this.Page.ClientScript.RegisterStartupScript( this.Page.GetType()
						, "js" + Guid.NewGuid()
						, initialScript
						, false );
				}
			}
			else
			{
				if( ScriptManager.GetCurrent( this.Page ) != null )
				{
					ScriptManager.RegisterStartupScript( this.Page
						, this.Page.GetType()
						, "js" + Guid.NewGuid()
						, initialScript
						, false );
				}
				else
				{

					this.Page.ClientScript.RegisterStartupScript( this.Page.GetType()
						, "js" + Guid.NewGuid()
						, initialScript
						, false );
				}
			}

			#endregion
		}

		#region SetIsBound

		/// <summary>
		/// Sets the is bound.
		/// </summary>
		protected virtual void SetIsBound()
		{
			this.txtIsBound.Value = this.IsBound.ToString();
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

		/// <summary>
		/// Restores view-state information from a previous request that was saved with the <see cref="M:System.Web.UI.WebControls.WebControl.SaveViewState"></see> method.
		/// </summary>
		/// <param name="savedState">An object that represents the control state to restore.</param>
		protected override void LoadViewState( object savedState )
		{
			base.LoadViewState( savedState );
			//LoadTreeNodes();
			//if( ViewState["RootNode"] != null )
			//	this.RootNode = (ASTreeViewNode)ViewState["RootNode"];

			string postedNodesValue = this.Page.Request.Form[this.txtASTreeViewNodes.UniqueID];
			if( string.IsNullOrEmpty( postedNodesValue ) )
				this.strASTreeViewNodes = this.TreeNodesState;//this.txtASTreeViewNodes.Value = this.TreeNodesState;
			else
				this.strASTreeViewNodes = postedNodesValue;

			ManageRootNodeProperty();
			LoadTreeNodes();
		}

		/// <summary>
		/// Saves any state that was modified after the <see cref="M:System.Web.UI.WebControls.Style.TrackViewState"></see> method was invoked.
		/// </summary>
		/// <returns>
		/// An object that contains the current view state of the control; otherwise, if there is no view state associated with the control, null.
		/// </returns>
		protected override object SaveViewState()
		{
			//ViewState["RootNode"] = this.RootNode;

			if( this.IsInPostback() )
			{
				if( this.EnableTreeNodesViewState )
					this.TreeNodesState = this.strASTreeViewNodes;//this.txtASTreeViewNodes.Value;

				this.txtASTreeViewNodes.Value = string.Empty;
			}

			return base.SaveViewState();
		}

		/// <summary>
		/// Renders the HTML opening tag of the control to the specified writer. This method is used primarily by control developers.
		/// </summary>
		/// <param name="writer">A <see cref="T:System.Web.UI.HtmlTextWriter"></see> that represents the output stream to render HTML content on the client.</param>
		public override void RenderBeginTag( HtmlTextWriter writer )
		{
			base.RenderBeginTag( writer );
		}

		/// <summary>
		/// Renders the contents of the control to the specified writer. This method is used primarily by control developers.
		/// </summary>
		/// <param name="writer">A <see cref="T:System.Web.UI.HtmlTextWriter"></see> that represents the output stream to render HTML content on the client.</param>
		protected override void RenderContents( HtmlTextWriter writer )
		{
			#region write a box in design mode

			if( this.Site != null && this.Site.DesignMode == true )
			{
				string htmlForDesignMode = @"
<div style='border:1px dashed blue;width:100px;height:100px;background-color:#ccc;'>
To config ASTreeView, please refer to the property page in your Visual Studio.
</div>
";
				writer.Write( htmlForDesignMode );
			}
			else
			{
				RenderSelectedNodeContainer( writer );
				RenderCheckedNodeContainer( writer );
				base.RenderContents( writer );
			}
			#endregion
		}

		/// <summary>
		/// Renders the HTML closing tag of the control into the specified writer. This method is used primarily by control developers.
		/// </summary>
		/// <param name="writer">A <see cref="T:System.Web.UI.HtmlTextWriter"></see> that represents the output stream to render HTML content on the client.</param>
		public override void RenderEndTag( HtmlTextWriter writer )
		{
			base.RenderEndTag( writer );
		}

		/// <summary>
		/// Renders the control to the specified HTML writer.
		/// </summary>
		/// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"></see> object that receives the control content.</param>
		protected override void Render( HtmlTextWriter writer )
		{
			base.Render( writer );

			//string asTreeViewScript = GenerateASTreeViewScript();
			//writer.Write( asTreeViewScript );
		}

		/// <summary>
		/// Binds a data source to the invoked server control and all its child controls.
		/// </summary>
		public override void DataBind()
		{
			//if null return
			if( this.dataSource == null )
				return;


			object o = this.DataSource;

			//bind dataTable
			if( o is DataTable )
			{
				if( this.DataSourceDescriptor != null )
				{
					ASTreeViewDataSourceDescriptor columnDescriptor = (ASTreeViewDataSourceDescriptor)this.DataSourceDescriptor;
					this.rootNode.Clear();
					columnDescriptor.BuildTreeFromDataSource( this.rootNode, (DataTable)o, this.DataTableRootNodeValue );
				}
			}
			else if( o is XmlDocument )
			{
				if( this.EnableXmlValidation )
					treeViewHelper.ValidateXmlDataSource( (XmlDocument)o );

				ASTreeViewDataSourceDescriptor xmlDescriptor = (ASTreeViewDataSourceDescriptor)this.DataSourceDescriptor;
				this.rootNode.Clear();
				xmlDescriptor.BuildTreeFromDataSource( this.rootNode, o, null );

			}
			else
			{
				ASTreeViewDataSourceDescriptor otherDescriptor = (ASTreeViewDataSourceDescriptor)this.DataSourceDescriptor;
				this.rootNode.Clear();
				otherDescriptor.BuildTreeFromDataSource( this.rootNode, o, null );
			}

			//base.DataBind();
		}

		#endregion

		#region Event

		#region OnSelectedNodeChanged

		private ASTreeViewNodeSelectedEventHandler _onSelectedNodeChanged;

		/// <summary>
		/// Occurs when [on selected node changed].
		/// </summary>
		public event ASTreeViewNodeSelectedEventHandler OnSelectedNodeChanged
		{
			add { _onSelectedNodeChanged += value; }
			remove { _onSelectedNodeChanged -= value; }
		}

		/// <summary>
		/// Fires the node selected event.
		/// </summary>
		protected virtual void FireNodeSelectedEvent()
		{
			if( _onSelectedNodeChanged != null )
			{
				string nodeText = string.Empty;
				string nodeValue = string.Empty;

				string[] tokens = this.SelectedNodeString.Split( new String[] { this.Separator }, StringSplitOptions.None );
				if( tokens.Length == 2 )
				{
					nodeText = HttpUtility.UrlDecode( tokens[0] );
					nodeValue = HttpUtility.UrlDecode( tokens[1] );
				}

				_onSelectedNodeChanged( this, new ASTreeViewNodeSelectedEventArgs( nodeText, nodeValue ) );
			}
		}

		#endregion

		#region OnCheckedNodeChanged

		private ASTreeViewNodeCheckedEventHandler _onCheckedNodeChanged;

		/// <summary>
		/// Occurs when [on checked node changed].
		/// </summary>
		public event ASTreeViewNodeCheckedEventHandler OnCheckedNodeChanged
		{
			add { _onCheckedNodeChanged += value; }
			remove { _onCheckedNodeChanged -= value; }
		}

		/// <summary>
		/// Fires the node checked event.
		/// </summary>
		protected virtual void FireNodeCheckedEvent()
		{
			if( _onCheckedNodeChanged != null )
			{
				string nodeText = string.Empty;
				string nodeValue = string.Empty;
				ASTreeViewCheckboxState state = ASTreeViewCheckboxState.Unchecked;

				string[] tokens = this.CheckedNodeString.Split( new String[] { this.Separator }, StringSplitOptions.None );
				if( tokens.Length == 3 )
				{
					nodeText = HttpUtility.UrlDecode( tokens[0] );
					nodeValue = HttpUtility.UrlDecode( tokens[1] );
					state = (ASTreeViewCheckboxState)int.Parse( HttpUtility.UrlDecode( tokens[2] ) );
				}

				_onCheckedNodeChanged( this, new ASTreeViewNodeCheckedEventArgs( nodeText, nodeValue, state ) );
			}
		}

		#endregion

		#endregion

		#region Delegates
		///<exclude/>
		public delegate void ASTreeNodeHandlerDelegate( ASTreeViewNode node );

		///<exclude/>
		public delegate void ASTreeULHandlerDelegate( HtmlGenericControl nodeUL );

		#endregion

		#region IPostBackDataHandler Members

		#region LoadPostData

		/// <summary>
		/// When implemented by a class, processes postback data for an ASP.NET server control.
		/// </summary>
		/// <param name="postDataKey">The key identifier for the control.</param>
		/// <param name="postCollection">The collection of all incoming name values.</param>
		/// <returns>
		/// true if the server control's state changes as a result of the postback; otherwise, false.
		/// </returns>
		public bool LoadPostData( string postDataKey, NameValueCollection postCollection )
		{
			bool postbackDataChanged = false;
			foreach( string key in postCollection.AllKeys )
			{
				//handle selectedNode
				if( this.IsControlPostbackKey( this.txtSelectedASTreeViewNodes.UniqueID, key ) )
				{
					string rawSelect = postCollection[key];
					if( !rawSelect.Equals( this.SelectedNodeString ) )
					{
						this.selectedNodeChanged = true;
						postbackDataChanged = true;

						this.SelectedNodeString = rawSelect;
						this.txtSelectedASTreeViewNodes.Value = rawSelect;
					}
				}

				//handle checkedNode
				if( this.IsControlPostbackKey( this.txtCheckedASTreeViewNodes.UniqueID, key ) )
				{
					string rawCheck = postCollection[key];
					if( !rawCheck.Equals( this.CheckedNodeString ) )
					{
						this.checkedNodeChanged = true;
						postbackDataChanged = true;

						this.CheckedNodeString = rawCheck;
						this.txtCheckedASTreeViewNodes.Value = rawCheck;
					}
				}
			}

			return postbackDataChanged;
		}

		#endregion

		#region RaisePostDataChangedEvent

		/// <summary>
		/// When implemented by a class, signals the server control to notify the ASP.NET application that the state of the control has changed.
		/// </summary>
		public void RaisePostDataChangedEvent()
		{
			if( this.selectedNodeChanged )
				FireNodeSelectedEvent();

			if( this.checkedNodeChanged )
				FireNodeCheckedEvent();
		}

		#endregion

		#endregion
	}
}
