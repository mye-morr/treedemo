using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Drawing;
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
    /// The ASDropDownTreeView class.
    /// </summary>
    [ToolboxData( "<{0}:ASDropDownTreeView runat=server></{0}:ASDropDownTreeView>" )
    , SupportsEventValidation
    , AspNetHostingPermission( SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal )
    , AspNetHostingPermission( SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal )]

    public class ASDropDownTreeView : ASTreeView, INamingContainer
    {
        #region controls

        private System.Web.UI.WebControls.TextBox txtOpenStatus;
        private System.Web.UI.WebControls.TextBox txtDisplayText;
        private System.Web.UI.WebControls.TextBox txtSelectedValue;
        private System.Web.UI.WebControls.TextBox txtCheckedValues;

        #region to generated html

        /*
         
<div id='dropdownTreeContainerctl00_cphContentPlaceHolder_astvMyTree' style='width:300px;'>
    <table class='defaultDropdownBox' id='dropdownTreeSelectorctl00_cphContentPlaceHolder_astvMyTree' width='300' cellpadding='0' cellspacing='0'>
        <tr>
            <td onclick='toggleDropdownTreeContainerctl00_cphContentPlaceHolder_astvMyTree("dropdownTreeObjectContainerctl00_cphContentPlaceHolder_astvMyTree");return false;' id='dropdownTreeTextctl00_cphContentPlaceHolder_astvMyTree' nowrap><div id='divDropdownTreeTextctl00_cphContentPlaceHolder_astvMyTree' style='padding:0px;margin:0px;white-space:nowrap;overflow:hidden;'>&nbsp;</div></td>
            <td width='17'><a class='defaultDropdownIcon' href='#' onclick='toggleDropdownTreeContainerctl00_cphContentPlaceHolder_astvMyTree("dropdownTreeObjectContainerctl00_cphContentPlaceHolder_astvMyTree");return false;'><img id='imgOpenIconctl00_cphContentPlaceHolder_astvMyTree' border='0' src='/Scripts/asdropdowntreeview/images/windropdown.gif' /></a></td><td>
            </td>
        </tr>
    </table>
    <table class='defaultDropdownTree' id='dropdownTreeObjectContainerctl00_cphContentPlaceHolder_astvMyTree' width='300' cellpadding='0' cellspacing='0' style='z-index:999;position:absolute;visibility:hidden;'>
    <tr>
        <td>
                <div id='dropdowntreectl00_cphContentPlaceHolder_astvMyTree'>
    
                </div>
        </td>
    </tr>
    </table>
</div>

         */

        #endregion

        protected HtmlTable tbDropDown;
        protected HtmlTable tbDropDownContainer;
        protected HtmlTableRow trDropDownContainerTR;
        protected HtmlTableCell tdDropDownContainerTD1;
        protected HtmlTableCell tdDropDownContainerTD2;
        protected HtmlTableRow trDropDownTR;
        protected HtmlTableCell tdDropDownTD1;
        protected HtmlTableCell tdDropDownTD2;
        protected HtmlTableCell tdDropDownTD3;
        protected HtmlGenericControl divDropdownTreeText;
        protected HtmlAnchor aDropDrownIcon;
        protected HtmlImage imgDropDrownIcon;

        protected HtmlTable tbDropdownTreeObjectContainer;
        protected HtmlTableRow trDropdownTreeObjectContainerTR;
        protected HtmlTableCell tdDropdownTreeObjectContainerTD;
        protected HtmlGenericControl divDropdownTreeObjectContainer;

        protected HtmlGenericControl divDebug;

        protected RequiredFieldValidator rfvChecked;
        protected RequiredFieldValidator rfvSelected;

        #endregion

        #region Properties

        #region override properties

        /// <summary>
        /// Gets the <see cref="T:System.Web.UI.HtmlTextWriterTag"></see> value that corresponds to this Web server control. This property is used primarily by control developers.
        /// </summary>
        /// <value></value>
        /// <returns>One of the <see cref="T:System.Web.UI.HtmlTextWriterTag"></see> enumeration values.</returns>
        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return  HtmlTextWriterTag.Div;
            }
        }

        /// <summary>
        /// Gets or sets the width of the Web server control.
        /// </summary>
        /// <value></value>
        /// <returns>A <see cref="T:System.Web.UI.WebControls.Unit"></see> that represents the width of the control. The default is <see cref="F:System.Web.UI.WebControls.Unit.Empty"></see>.</returns>
        /// <exception cref="T:System.ArgumentException">The width of the Web server control was set to a negative value. </exception>
        public override Unit Width
        {
            get
            {
                if( base.Width.Value == 0 )
                    return new Unit( 80 );
            
                return base.Width;
            }
            set
            {
                base.Width = value;
            }
        }

        #endregion

        #region Configuration

        #region BasePath

        /// <summary>
        /// Gets or sets the drop down tree base path.
        /// </summary>
        /// <value>The drop down tree base path.</value>
        [Browsable( true ),
        DefaultValue("~/Scripts/astreeview/"),
        Category( "Configuration" )]
        virtual public string DropDownTreeBasePath
        {
            get
            {
                object o = ViewState["DropDownTreeBasePath"];
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
                ViewState["DropDownTreeBasePath"] = value;
            }
        }

        #endregion

        #region EnableHalfCheckedAsChecked (bool)

        /// <summary>
        /// Gets or sets a value indicating whether to regard half checked nodes as checked.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [enable half checked as checked]; otherwise, <c>false</c>.
        /// </value>
        [Browsable( true )
        , DefaultValue( false )
        , Category( "Configuration" )]
        public bool EnableHalfCheckedAsChecked
        {
            get
            {
                object o = ViewState["EnableHalfCheckedAsChecked"];
                return o == null ? false : (bool)o;
            }
            set
            {
                ViewState["EnableHalfCheckedAsChecked"] = value;
            }
        }

        #endregion

        #region EnableCloseOnOutsideClick
        /// <summary>
        /// Gets or sets a value indicating whether to enable close on outside click.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [enable close on outside click]; otherwise, <c>false</c>.
        /// </value>
        [Bindable( true ),
        Category( "Configuration" ),
        DefaultValue( true )]
        public bool EnableCloseOnOutsideClick
        {
            get
            {
                return ViewState["EnableCloseOnOutsideClick"] == null ? true : (bool)ViewState["EnableCloseOnOutsideClick"];
            }
            set
            {
                ViewState["EnableCloseOnOutsideClick"] = value;
            }
        }
        #endregion

        #region InitialDropdownText

        /// <summary>
        /// Gets or sets the initial dropdown text.
        /// </summary>
        /// <value>The initial dropdown text.</value>
        [Bindable( true ),
        Category( "Configuration" ),
        DefaultValue( "" )]
        public string InitialDropdownText
        {
            get
            {
                return ViewState["InitialDropdownText"] == null ? string.Empty : (string)ViewState["InitialDropdownText"];
            }
            set
            {
                ViewState["InitialDropdownText"] = value;
            }
        }
        #endregion

        #region DropdownText

        /// <summary>
        /// Gets or sets the dropdown text.
        /// </summary>
        /// <value>The dropdown text.</value>
        [Bindable( true ),
        Category( "Configuration" ),
        DefaultValue( "" )]
        public string DropdownText
        {
            get
            {
                return this.txtDisplayText.Text;
            }
            set
            {
                this.txtDisplayText.Text = HttpUtility.UrlEncode( value );
            }
        }
        #endregion

        #region InitialDropdownOpen

        /// <summary>
        /// Gets or sets a value indicating whether [initial dropdown open].
        /// </summary>
        /// <value><c>true</c> if [initial dropdown open]; otherwise, <c>false</c>.</value>
        [Bindable( true ),
       Category( "Configuration" ),
        DefaultValue( "" )]
        public bool InitialDropdownOpen
        {
            get
            {
                return ViewState["InitialDropdownOpen"] == null ? false : (bool)ViewState["InitialDropdownOpen"];
            }
            set
            {
                ViewState["InitialDropdownOpen"] = value;
            }
        }

        #endregion

        #region OpenByClickText

        /// <summary>
        /// Gets or sets a value indicating whether [open by click text].
        /// </summary>
        /// <value><c>true</c> if [open by click text]; otherwise, <c>false</c>.</value>
        [Bindable( true ),
       Category( "Configuration" ),
        DefaultValue( "" )]
        public bool OpenByClickText
        {
            get
            {
                return ViewState["OpenByClickText"] == null ? true : (bool)ViewState["OpenByClickText"];
            }
            set
            {
                ViewState["OpenByClickText"] = value;
            }
        }

        #endregion

        #region DropdownIconUp

        /// <summary>
        /// Gets or sets the dropdown icon up.
        /// </summary>
        /// <value>The dropdown icon up.</value>
        [Bindable( true ),
       Category( "Configuration" ),
        DefaultValue( "" )]
        public string DropdownIconUp
        {
            get
            {
                object o = ViewState["DropdownIconUp"];
                return o == null ? this.DropDownTreeBasePath + "images/windropdown.gif" : this.ProcessImageUrl( (string)o );

                //return ViewState["DropdownIconUp"] == null ? this.DropDownTreeBasePath + "images/windropdown.gif"/*"dropdown_right_up.jpg"*/ : (string)ViewState["DropdownIconUp"];
            }
            set
            {
                ViewState["DropdownIconUp"] = value;
            }
        }

        #endregion

        #region DropdownIconDown

        /// <summary>
        /// Gets or sets the dropdown icon down.
        /// </summary>
        /// <value>The dropdown icon down.</value>
        [Bindable( true ),
       Category( "Configuration" ),
        DefaultValue( "" )]
        public string DropdownIconDown
        {
            get
            {
                object o = ViewState["DropdownIconDown"];
                return o == null ? this.DropDownTreeBasePath + "images/windropdown.gif" : this.ProcessImageUrl( (string)o );

                //return ViewState["DropdownIconDown"] == null ? this.DropDownTreeBasePath + "images/windropdown.gif"/*"dropdown_right_down.jpg"*/ : (string)ViewState["DropdownIconDown"];
            }
            set
            {
                ViewState["DropdownIconDown"] = value;
            }
        }

        #endregion

        #region DropdownIconUpDisabled

        /// <summary>
        /// Gets or sets the dropdown icon up disabled.
        /// </summary>
        /// <value>The dropdown icon up disabled.</value>
        [Bindable( true ),
       Category( "Configuration" ),
        DefaultValue( "" )]
        public string DropdownIconUpDisabled
        {
            get
            {
                object o = ViewState["DropdownIconUpDisabled"];
                return o == null ? this.DropDownTreeBasePath + "images/windropdown-disabled.gif" : this.ProcessImageUrl( (string)o );
            }
            set
            {
                ViewState["DropdownIconUpDisabled"] = value;
            }
        }

        #endregion

        #region DropdownIconDownDisabled

        /// <summary>
        /// Gets or sets the dropdown icon down disabled.
        /// </summary>
        /// <value>The dropdown icon down disabled.</value>
        [Bindable( true ),
       Category( "Configuration" ),
        DefaultValue( "" )]
        public string DropdownIconDownDisabled
        {
            get
            {
                object o = ViewState["DropdownIconDownDisabled"];
                return o == null ? this.DropDownTreeBasePath + "images/windropdown-disabled.gif" : this.ProcessImageUrl( (string)o );

            }
            set
            {
                ViewState["DropdownIconDownDisabled"] = value;
            }
        }

        #endregion

        #region EnableCloseOnNodeSelection
        /// <summary>
        /// Gets or sets a value indicating whether [enable close on node selection].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [enable close on node selection]; otherwise, <c>false</c>.
        /// </value>
        [Bindable( true ),
        Category( "Configuration" ),
        DefaultValue( false )]
        public bool EnableCloseOnNodeSelection
        {
            get
            {
                return ViewState["EnableCloseOnNodeSelection"] == null ? false : (bool)ViewState["EnableCloseOnNodeSelection"];
            }
            set
            {
                ViewState["EnableCloseOnNodeSelection"] = value;
            }
        }
        #endregion

        #region DropdownHeight

        #region MaxDropdownHeight (int)

        /// <summary>
        /// DropdownHeight
        /// </summary>
        /// <value>The height of the max dropdown.</value>
        [Browsable( true )
        , DefaultValue( -1 )
        , Category( "Configuration" )]
        public int MaxDropdownHeight
        {
            get
            {
                object o = ViewState["MaxDropdownHeight"];
                return o == null ? -1 : (int)o;
            }
            set
            {
                ViewState["MaxDropdownHeight"] = value;
            }
        }

        #endregion

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
        new public string OnNodeSelectedScript
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
        new public string OnNodeCheckedScript
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

        #region EnableOnDropDownClickScriptReturn

        /// <summary>
        /// Gets or sets a value indicating whether [enable on drop down click script return].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [enable on drop down click script return]; otherwise, <c>false</c>.
        /// </value>
        [Browsable( true )
        , DefaultValue( false )
        , Category( "Configuration" )]
        public bool EnableOnDropDownClickScriptReturn
        {
            get
            {
                object o = ViewState["EnableOnDropDownClickScriptReturn"];
                return o == null ? false : (bool)o;
            }
            set
            {
                ViewState["EnableOnDropDownClickScriptReturn"] = value;
            }
        }

        #endregion

        #region OnDropDownClickScript

        /// <summary>
        /// Gets or sets the on drop down click script.
        /// </summary>
        /// <value>
        /// The on drop down click script.
        /// </value>
        [Browsable( true )
        , DefaultValue( "" )
        , Category( "Configuration" )]
        public string OnDropDownClickScript
        {
            get
            {
                object o = ViewState["OnDropDownClickScript"];
                return o == null ? "" : (string)o;
            }
            set
            {
                ViewState["OnDropDownClickScript"] = value;
            }
        }

        #endregion

        #endregion

        #region css

        /// <summary>
        /// Gets or sets the CSS class dropdown box container.
        /// </summary>
        /// <value>The CSS class dropdown box container.</value>
        [Bindable( true ),
        Category( "Appearance" ),
        DefaultValue( "" )]
        public string CssClassDropdownBoxContainer
        {
            get
            {
                object o = ViewState["CssClassDropdownBoxContainer"];
                return o == null ? "defaultDropdownBoxContainer" : (string)o;
            }
            set
            {
                ViewState["CssClassDropdownBoxContainer"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the CSS class dropdown tree object container.
        /// </summary>
        /// <value>The CSS class dropdown tree object container.</value>
        [Bindable( true ),
        Category( "Appearance" ),
        DefaultValue( "" )]
        public string CssClassDropdownTreeObjectContainer
        {
            get
            {
                object o = ViewState["CssClassDropdownTreeObjectContainer"];
                return o == null ? "defaultDropdownTreeObjectContainer" : (string)o;
            }
            set
            {
                ViewState["CssClassDropdownTreeObjectContainer"] = value;
            }
        }


        /// <summary>
        /// Gets or sets the CSS class dropdown box.
        /// </summary>
        /// <value>The CSS class dropdown box.</value>
        [Bindable( true ),
        Category( "Appearance" ),
        DefaultValue( "" )]
        public string CssClassDropdownBox
        {
            get
            {
                return ViewState["CssClassDropdownBox"] == null ? "defaultDropdownBox" : (string)ViewState["CssClassDropdownBox"];
            }
            set
            {
                ViewState["CssClassDropdownBox"] = value;
            }
        }


        /// <summary>
        /// Gets or sets the CSS class dropdown box disabled.
        /// </summary>
        /// <value>The CSS class dropdown box disabled.</value>
        [Bindable( true ),
        Category( "Appearance" ),
        DefaultValue( "" )]
        public string CssClassDropdownBoxDisabled
        {
            get
            {
                return ViewState["CssClassDropdownBoxDisabled"] == null ? "defaultDropdownBoxDisabled" : (string)ViewState["CssClassDropdownBoxDisabled"];
            }
            set
            {
                ViewState["CssClassDropdownBoxDisabled"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the CSS class dropdown tree.
        /// </summary>
        /// <value>The CSS class dropdown tree.</value>
        [Bindable( true ),
        Category( "Appearance" ),
        DefaultValue( "" )]
        public string CssClassDropdownTree
        {
            get
            {
                return ViewState["CssClassDropdownTree"] == null ? "defaultDropdownTree" : (string)ViewState["CssClassDropdownTree"];
            }
            set
            {
                ViewState["CssClassDropdownTree"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the CSS class dropdown icon A.
        /// </summary>
        /// <value>The CSS class dropdown icon A.</value>
        [Bindable( true ),
        Category( "Appearance" ),
        DefaultValue( "" )]
        public string CssClassDropdownIconA
        {
            get
            {
                object o = ViewState["CssClassDropdownIconA"];
                return o == null ? "defaultDropdownIconA" : (string)o;
            }
            set
            {
                ViewState["CssClassDropdownIconA"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the CSS class dropdown icon.
        /// </summary>
        /// <value>The CSS class dropdown icon.</value>
        [Bindable( true ),
        Category( "Appearance" ),
        DefaultValue( "" )]
        public string CssClassDropdownIcon
        {
            get
            {
                return ViewState["CssClassDropdownIcon"] == null ? "defaultDropdownIcon" : (string)ViewState["CssClassDropdownIcon"];
            }
            set
            {
                ViewState["CssClassDropdownIcon"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the CSS class dropdown icon TD.
        /// </summary>
        /// <value>The CSS class dropdown icon TD.</value>
        [Bindable( true ),
        Category( "Appearance" ),
        DefaultValue( "" )]
        public string CssClassDropdownIconTD
        {
            get
            {
                object o = ViewState["CssClassDropdownIconTD"];
                return o == null ? "defaultDropdownIconTD" : (string)o;
            }
            set
            {
                ViewState["CssClassDropdownIconTD"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the CSS class dropdown icon TD disabled.
        /// </summary>
        /// <value>The CSS class dropdown icon TD disabled.</value>
        [Bindable( true ),
        Category( "Appearance" ),
        DefaultValue( "" )]
        public string CssClassDropdownIconTDDisabled
        {
            get
            {
                object o = ViewState["CssClassDropdownIconTDDisabled"];
                return o == null ? "defaultDropdownIconTDDisabled" : (string)o;
            }
            set
            {
                ViewState["CssClassDropdownIconTDDisabled"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the CSS class dropdown text container.
        /// </summary>
        /// <value>The CSS class dropdown text container.</value>
        [Bindable( true ),
        Category( "Appearance" ),
        DefaultValue( "" )]
        public string CssClassDropdownTextContainer
        {
            get
            {
                object o = ViewState["CssClassDropdownTextContainer"];
                return o == null ? "defaultDropdownTextContainer" : (string)o;
            }
            set
            {
                ViewState["CssClassDropdownTextContainer"] = value;
            }
        }


        /// <summary>
        /// Gets or sets the CSS class dropdown text container disabled.
        /// </summary>
        /// <value>The CSS class dropdown text container disabled.</value>
        [Bindable( true ),
        Category( "Appearance" ),
        DefaultValue( "" )]
        public string CssClassDropdownTextContainerDisabled
        {
            get
            {
                object o = ViewState["CssClassDropdownTextContainerDisabled"];
                return o == null ? "defaultDropdownTextContainerDisabled" : (string)o;
            }
            set
            {
                ViewState["CssClassDropdownTextContainerDisabled"] = value;
            }
        }

        #endregion

        #region validator property

        #region EnableForceServerSideValidate

        /// <summary>
        /// Gets or sets a value indicating whether [enable force server side validate].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [enable force server side validate]; otherwise, <c>false</c>.
        /// </value>
        [Bindable( false ),
        Category( "Validator" ),
        DefaultValue( "" )]
        public bool EnableForceServerSideValidate
        {
            get
            {
                return ViewState["EnableForceServerSideValidate"] == null ? false : (bool)ViewState["EnableForceServerSideValidate"];
            }
            set
            {
                ViewState["EnableForceServerSideValidate"] = value;
            }
        }

        #endregion

        #region RequiredValidatorColor
        /// <summary>
        /// Gets or sets the color of the required validator.
        /// </summary>
        /// <value>The color of the required validator.</value>
        [Bindable( true ),
        Category( "Validator" ),
        DefaultValue( "" )]
        public Color RequiredValidatorColor
        {
            get
            {
                return ViewState["RequiredValidatorColor"] == null ? Color.Red : (Color)ViewState["RequiredValidatorColor"];
            }
            set
            {
                ViewState["RequiredValidatorColor"] = value;
            }
        }

        #endregion

        #region RequiredValidatorDisplay
        /// <summary>
        /// Gets or sets the required validator display.
        /// </summary>
        /// <value>The required validator display.</value>
        [Bindable( true ),
        Category( "Validator" ),
        DefaultValue( "" )]
        public ValidatorDisplay RequiredValidatorDisplay
        {
            get
            {
                return ViewState["RequiredValidatorDisplay"] == null ? ValidatorDisplay.Dynamic : (ValidatorDisplay)ViewState["RequiredValidatorDisplay"];
            }
            set
            {
                ViewState["RequiredValidatorDisplay"] = value;
            }
        }

        #endregion

        #region RequiredValidatorErrorMessage
        /// <summary>
        /// Gets or sets the required validator error message.
        /// </summary>
        /// <value>The required validator error message.</value>
        [Bindable( true ),
        Category( "Validator" ),
        DefaultValue( "" )]
        public string RequiredValidatorErrorMessage
        {
            get
            {
                return ViewState["RequiredValidatorErrorMessage"] == null ? "mandatory" : (string)ViewState["RequiredValidatorErrorMessage"];
            }
            set
            {
                ViewState["RequiredValidatorErrorMessage"] = value;
            }
        }

        #endregion

        #region RequiredValidatorText
        /// <summary>
        /// RequiredValidatorText
        /// </summary>
        /// <value>The required validator text.</value>
        [Bindable( true ),
        Category( "Validator" ),
        DefaultValue( "" )]
        public string RequiredValidatorText
        {
            get
            {
                return ViewState["RequiredValidatorText"] == null ? "mandatory" : (string)ViewState["RequiredValidatorText"];
            }
            set
            {
                ViewState["RequiredValidatorText"] = value;
            }
        }

        #endregion

        #region EnableRequiredValidator
        /// <summary>
        /// Gets or sets a value indicating whether [enable required validator].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [enable required validator]; otherwise, <c>false</c>.
        /// </value>
        [Bindable( true ),
        Category( "Validator" ),
        DefaultValue( "" )]
        public bool EnableRequiredValidator
        {
            get
            {
                return ViewState["EnableRequiredValidator"] == null ? false : (bool)ViewState["EnableRequiredValidator"];
            }
            set
            {
                ViewState["EnableRequiredValidator"] = value;
            }
        }

        #endregion

        #region RequiredValidatorValidationGroup

        /// <summary>
        /// Gets or sets the required validator validation group.
        /// </summary>
        /// <value>The required validator validation group.</value>
        [Bindable( true ),
        Category( "Validator" ),
        DefaultValue( "" )]
        public string RequiredValidatorValidationGroup
        {
            get
            {
                object o = ViewState["RequiredValidatorValidationGroup"];
                return o == null ? string.Empty : (string)o;
            }
            set
            {
                ViewState["RequiredValidatorValidationGroup"] = value;
            }
        }

        #endregion

        #region CssClassRequiredValidator

        /// <summary>
        /// Gets or sets the CSS class required validator.
        /// </summary>
        /// <value>The CSS class required validator.</value>
        [Bindable( true ),
        Category( "Validator" ),
        DefaultValue( "" )]
        public string CssClassRequiredValidator
        {
            get
            {
                object o = ViewState["CssClassRequiredValidator"];
                return o == null ? string.Empty : (string)o;
            }
            set
            {
                ViewState["CssClassRequiredValidator"] = value;
            }
        }

        #endregion

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ASDropDownTreeView"/> class.
        /// </summary>
        public ASDropDownTreeView()
        {
            this.txtOpenStatus = new TextBox();
            this.txtDisplayText = new TextBox();
            this.txtSelectedValue = new TextBox();
            this.txtCheckedValues = new TextBox();

            tbDropDown = new HtmlTable();
            tbDropDownContainer = new HtmlTable();
            trDropDownContainerTR = new HtmlTableRow();
            tdDropDownContainerTD1 = new HtmlTableCell();
            tdDropDownContainerTD2 = new HtmlTableCell();

            trDropDownTR = new HtmlTableRow();
            tdDropDownTD1 = new HtmlTableCell();
            tdDropDownTD2 = new HtmlTableCell();
            tdDropDownTD3 = new HtmlTableCell();
            divDropdownTreeText = new HtmlGenericControl("div");
            aDropDrownIcon = new HtmlAnchor();
            imgDropDrownIcon = new HtmlImage();

            tbDropdownTreeObjectContainer = new HtmlTable();
            trDropdownTreeObjectContainerTR = new HtmlTableRow();
            tdDropdownTreeObjectContainerTD = new HtmlTableCell();
            divDropdownTreeObjectContainer = new HtmlGenericControl( "div" );

            divDebug = new HtmlGenericControl("div");

            rfvChecked = new RequiredFieldValidator();
            rfvSelected = new RequiredFieldValidator();
        }

        #endregion

        #region override methods

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

            //base.divDebugContainer.Controls.Add( this.txtOpenStatus );
            //base.divDebugContainer.Controls.Add( this.txtSelectedValue );
            //base.divDebugContainer.Controls.Add( this.txtCheckedValues );
            this.divDebug.Controls.Add( this.txtOpenStatus );
            this.divDebug.Controls.Add( this.txtSelectedValue );
            this.divDebug.Controls.Add( this.txtCheckedValues );

            if( !base.IsInPostback() )
                this.txtDisplayText.Text = HttpUtility.UrlEncode( this.InitialDropdownText );

            this.divDebug.Controls.Add( this.txtDisplayText );
            //base.divDebugContainer.Controls.Add( this.txtDisplayText );

            #region construct tbDropDownContainer

            this.tbDropDownContainer.Rows.Add( this.trDropDownContainerTR );
            this.trDropDownContainerTR.Cells.Add( this.tdDropDownContainerTD1 );
            this.trDropDownContainerTR.Cells.Add( this.tdDropDownContainerTD2 );

            this.tdDropDownContainerTD1.Controls.Add( this.tbDropDown );
            this.tdDropDownContainerTD2.Controls.Add( this.rfvChecked );
            this.tdDropDownContainerTD2.Controls.Add( this.rfvSelected );

            this.Controls.Add( tbDropDownContainer );

            #endregion

            #region construct tbDropdownTreeObjectContainer

            this.tbDropdownTreeObjectContainer.Rows.Add( this.trDropdownTreeObjectContainerTR );
            this.trDropdownTreeObjectContainerTR.Cells.Add( this.tdDropdownTreeObjectContainerTD );
            this.tdDropdownTreeObjectContainerTD.Controls.Add( this.divDropdownTreeObjectContainer );
            this.divDropdownTreeObjectContainer.Controls.Add( base.divTreeViewContainer );
            this.Controls.Add( this.tbDropdownTreeObjectContainer );

            #endregion

            #region manage divDebug

            this.Controls.Add( this.divDebug );

            #endregion

            #region construct tbDropDown

            this.tbDropDown.Rows.Add( this.trDropDownTR );
            this.trDropDownTR.Cells.Add( this.tdDropDownTD1 );
            this.trDropDownTR.Cells.Add( this.tdDropDownTD2 );
            //this.trDropDownTR.Controls.Add( this.tdDropDownTD3 );

            this.tdDropDownTD1.Controls.Add( this.divDropdownTreeText );
            this.tdDropDownTD2.Controls.Add( this.aDropDrownIcon );
            this.aDropDrownIcon.Controls.Add( this.imgDropDrownIcon );


            #endregion
        }

        /// <summary>
        /// Adds the tree view to control set.
        /// </summary>
        override protected void AddTreeViewToControlSet()
        {
            //do nothing, add parent container to divDropdownTreeObjectContainer
            string s = "";
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"></see> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"></see> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );
            
            #region validate data
            if( base.IsInPostback() )
            {
                if( this.rfvChecked.Enabled && this.EnableForceServerSideValidate )
                    this.rfvChecked.Validate();

                if( this.rfvSelected.Enabled && this.EnableForceServerSideValidate )
                    this.rfvSelected.Validate();
            }
            #endregion

        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.PreRender"></see> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> object that contains the event data.</param>
        protected override void OnPreRender( EventArgs e )
        {
            #region onSelectedScript

            #region closeOnNodeSelectionScript

            string closeOnNodeSelectionScript = string.Format( @"toggleDropdownTreeContainer{0}(""{1}"");return false;", this.ClientID /*0*/, this.tbDropdownTreeObjectContainer.ClientID /*1*/ );

            #endregion

            string onSelectedScript = string.Format( @"
var text{0} = '';
function ddtOnSelectHandler{0}(li){{
    if( li.getAttribute('selected') != 'true' )
        return;

    var a = li.getElementsByTagName('A')[0];
    if( !a )
        return;
    text{0} += (a.innerHTML + ',');
    
}}
{1}.traverseTreeNode( ddtOnSelectHandler{0} );

if( text{0}.length > 0 )
    text{0} = text{0}.substr( 0, text{0}.length - 1 );

setDisplayText{0}(text{0});

document.getElementById('{2}').value = encodeURIComponent( {1}.getSelectedNodesValue() );

{3}

{4}
", this.ClientID /*0*/
 , base.GetClientTreeObjectId() /*1*/
 , this.txtSelectedValue.ClientID /*2*/
 , this.EnableCloseOnNodeSelection ? closeOnNodeSelectionScript : string.Empty /*3*/
 , this.OnNodeSelectedScript /*4*/);

            base.OnNodeSelectedScript = onSelectedScript;

            #endregion

            #region onCheckedScript

            string onCheckedScript = string.Format( @"
var text{0} = '';
function ddtOnCheckedHandler{0}(li){{
    if( li.getAttribute('checkedState') == '0' {2} ){{
        var a = li.getElementsByTagName('A')[0];
        if( !a )
            return;
        text{0} += (a.innerHTML + ',');
    }}
}}
{1}.traverseTreeNode( ddtOnCheckedHandler{0} );

if( text{0}.length > 0 )
    text{0} = text{0}.substr( 0, text{0}.length - 1 );

setDisplayText{0}(text{0});

document.getElementById('{3}').value = encodeURIComponent( {1}.getCheckedNodesValues({4}, '{5}') );

{6}
", this.ClientID /*0*/
 , base.GetClientTreeObjectId() /*1*/
 , this.EnableHalfCheckedAsChecked ? "|| li.getAttribute('checkedState') == '1'" : string.Empty /*2*/
 , this.txtCheckedValues.ClientID /*3*/
 , this.EnableHalfCheckedAsChecked ? "true" : "false" /*4*/
 , base.Separator /*5*/
 , this.OnNodeCheckedScript /*6*/);

            base.OnNodeCheckedScript = onCheckedScript;

            #endregion

            #region manage tbDropdownContainer

            this.tbDropDownContainer.Attributes.Add( "class", this.CssClassDropdownBoxContainer );
            this.tbDropDownContainer.Attributes.Add( "cellpadding", "0" );
            this.tbDropDownContainer.Attributes.Add( "cellspacing", "0" );
            if( this.Width.Value > 10 )
                this.tbDropDownContainer.Attributes.Add( "width", this.Width.Value.ToString( "0" ) );

            #endregion

            #region manage tbDropdown

            #region tbDropdown

            this.tbDropDown.Attributes.Add( "class", this.Enabled ? this.CssClassDropdownBox : this.CssClassDropdownBoxDisabled );
            this.tbDropDown.Attributes.Add( "cellpadding", "0" );
            this.tbDropDown.Attributes.Add( "cellspacing", "0" );

            #endregion

            #region dropdown icon

            string strOpenStatus = "";
            string strOpenStatusTD = "";
            bool isVisible = false;

            if( this.txtOpenStatus.Text == "visible" )
                isVisible = true;//strOpenStatus = "visibility:visible;";
            else if( this.txtOpenStatus.Text == "hidden" )
                isVisible = false;//strOpenStatus = "visibility:hidden;";
            else
                isVisible = false;//strOpenStatus = "visibility:hidden;";

            if( !base.IsInPostback() )
            {
                if( this.InitialDropdownOpen )
                {
                    isVisible = true;//strOpenStatus = "visibility:visible;";
                }
            }

            string openIcon = string.Empty;
            if( isVisible )
            {//strOpenStatus == "visibility:visible;" )
                openIcon = this.Enabled ? this.DropdownIconUp : this.DropdownIconUpDisabled;//"dropdown_right_up.jpg";
                strOpenStatus = "visibility:visible;";
                strOpenStatusTD= "display:block;";
            }
            else
            {// if( strOpenStatus == "visibility:hidden;" )
                openIcon = this.Enabled ? this.DropdownIconDown : this.DropdownIconDownDisabled;//"dropdown_right_down.jpg";
                strOpenStatus = "visibility:hidden;";
                strOpenStatusTD = "display:none;";
            }

            this.imgDropDrownIcon.Src = openIcon;
            this.imgDropDrownIcon.Attributes.Add( "class", this.CssClassDropdownIcon );
            #endregion

            #region td1 click

            string openByClickTextScript = this.OpenByClickText ? string.Format( @"toggleDropdownTreeContainer{0}(""{1}"");return false;", this.ClientID /*0*/, this.tbDropdownTreeObjectContainer.ClientID /*1*/ ) : string.Empty;

            if( this.Enabled )
                this.tdDropDownTD1.Attributes.Add( "onclick", openByClickTextScript );
            else
                this.tdDropDownTD1.Attributes.Remove( "onclick" );


            #endregion

            #region td2 icon td

            this.tdDropDownTD2.Attributes.Add( "class", this.Enabled ? this.CssClassDropdownIconTD : this.CssClassDropdownIconTDDisabled );

            #endregion

            #region aDropDrownIcon

            this.aDropDrownIcon.HRef = "javascript:void(0);";
            this.aDropDrownIcon.Attributes.Add( "class", this.CssClassDropdownIconA );
            string aOnclick = string.Format( @"toggleDropdownTreeContainer{0}(""{1}"");return false;", this.ClientID /*0*/, this.tbDropdownTreeObjectContainer.ClientID /*1*/ );

            if( this.Enabled )
                this.aDropDrownIcon.Attributes.Add( "onclick", aOnclick );
            else
                this.aDropDrownIcon.Attributes.Remove( "onclick" );

            #endregion
            
            #region divDropdownTreeText

            if( !base.IsInPostback() )
                this.divDropdownTreeText.InnerHtml = this.InitialDropdownText;
            else
                this.divDropdownTreeText.InnerHtml = HttpUtility.UrlDecode( this.txtDisplayText.Text );

            this.divDropdownTreeText.Attributes.Add( "class", this.Enabled ? this.CssClassDropdownTextContainer : this.CssClassDropdownTextContainerDisabled );

            this.divDropdownTreeText.Style.Add( HtmlTextWriterStyle.Width, ( this.Width.Value - 19 ).ToString( "0" ) + "px" );
            #endregion

            #endregion

            #region manage tbDropdownTreeObjectContainer

            this.tbDropdownTreeObjectContainer.Attributes.Add( "class", this.CssClassDropdownTree );
            this.tbDropdownTreeObjectContainer.Attributes.Add( "width", this.Width.Value.ToString( "0" ) );
            this.tbDropdownTreeObjectContainer.Attributes.Add( "cellpadding", "0" );
            this.tbDropdownTreeObjectContainer.Attributes.Add( "cellspacing", "0" );
            this.tbDropdownTreeObjectContainer.Attributes.Add( "style", "z-index:999;position:absolute;" + strOpenStatus );
            
            #endregion

            #region manage tdDropdownTreeObjectContainerTD

            this.tdDropdownTreeObjectContainerTD.Attributes.Add( "style", strOpenStatusTD );
            
            #endregion

            #region manage divDropdownTreeObjectContainer
            this.divDropdownTreeObjectContainer.Attributes.Add( "class", this.CssClassDropdownTreeObjectContainer );
            if( this.MaxDropdownHeight > 0 )
            {
                this.divDropdownTreeObjectContainer.Attributes["style"] = string.Format( "max-height:{0}px;_height:{0}px;overflow:auto;", this.MaxDropdownHeight );
                //this.divDropdownTreeObjectContainer.Style.Add( "min-height", this.DropdownHeight.ToString() + "px" );
                //this.divDropdownTreeObjectContainer.Style.Add( "_height", this.DropdownHeight.ToString() + "px" );
                //this.divDropdownTreeObjectContainer.Style.Add( "overflow", "auto" );
            }
            #endregion

            #region manage divDebug

            if( !this.EnableDebugMode )
                this.divDebug.Style.Add( "display", "none" );

            #endregion

            #region initial txtCheckedValues & txtSelectedValue

            if( !base.IsInPostback() )
            {
                ASTreeViewNode selectedNode = base.GetSelectedNode();
                if( selectedNode != null )
                    this.txtSelectedValue.Text = HttpUtility.UrlEncode( selectedNode.NodeValue );

                List<ASTreeViewNode> checkedNodes = base.GetCheckedNodes( this.EnableHalfCheckedAsChecked );
                if( checkedNodes.Count > 0 )
                {
                    string result = string.Empty;
                    foreach( ASTreeViewNode node in checkedNodes )
                        result += ( node.NodeValue + base.Separator );

                    result = result.Substring( 0, result.Length - base.Separator.Length );
                    this.txtCheckedValues.Text = HttpUtility.UrlEncode( result );
                }
            }
            #endregion


            base.OnPreRender( e );

        }


        /// <summary>
        /// Renders the contents of the control to the specified writer. This method is used primarily by control developers.
        /// </summary>
        /// <param name="writer">A <see cref="T:System.Web.UI.HtmlTextWriter"></see> that represents the output stream to render HTML content on the client.</param>
        protected override void RenderContents( HtmlTextWriter writer )
        {
            

            #region initialDropdownHTML

            //string strOpenStatus = "";
            //if( this.txtOpenStatus.Text == "visible" )
            //    strOpenStatus = "visibility:visible;";
            //else if( this.txtOpenStatus.Text == "hidden" )
            //    strOpenStatus = "visibility:hidden;";
            //else
            //    strOpenStatus = "visibility:hidden;";

            //if( !base.IsInPostback() )
            //{
            //    if( this.InitialDropdownOpen )
            //    {
            //        strOpenStatus = "visibility:visible;";
            //    }
            //}

            //string openIcon = string.Empty;
            //if( strOpenStatus == "visibility:visible;" )
            //    openIcon = this.DropdownIconUp;//"dropdown_right_up.jpg";
            //else if( strOpenStatus == "visibility:hidden;" )
            //    openIcon = this.DropdownIconDown;//"dropdown_right_down.jpg";

            //string openByClickTextScript = this.OpenByClickText ? string.Format( @"onclick='toggleDropdownTreeContainer{0}(""{1}"");return false;'", this.ClientID /*0*/, this.tbDropdownTreeObjectContainer.ClientID /*1*/ ) : string.Empty;

        
//            string initialDropdownHTML1 = string.Format( @"
//			<div id='dropdownTreeContainer{0}' style='width:{2};'>
//				<table class='{6}' id='dropdownTreeSelector{0}' width='{9}' cellpadding='0' cellspacing='0'>
//					<tr>
//						<td {5} id='dropdownTreeText{0}' nowrap><div id='divDropdownTreeText{0}' style='padding:0px;margin:0px;white-space:nowrap;overflow:hidden;'>&nbsp;</div></td>
//						<td width='17'><a class='{8}' href='#' onclick='toggleDropdownTreeContainer{0}(""dropdownTreeObjectContainer{0}"");return false;'><img id='imgOpenIcon{0}' border='0' src='{1}images/{4}' /></a></td><td>", this.ClientID/*0*/
//                , this.DropDownTreeBasePath/*1*/
//                , this.Width.Value.ToString( "0" ) + "px"/*2*/
//                , strOpenStatus/*3*/
//                , openIcon /*4*/
//                , openByClickTextScript /*5*/
//                , this.CssClassDropdownBox /*6*/
//                , this.CssClassDropdownTree /*7*/
//                , this.CssClassDropdownIcon /*8*/
//                , this.Width.Value.ToString( "0" ) /*9*/
//                , "z-index:999;position:absolute;"/*10*/);
            //</td>
            //        </tr>
            //    </table>
//            string initialDropdownHTML2 = string.Format( @"
//				<table class='{7}' id='dropdownTreeObjectContainer{0}' width='{9}' cellpadding='0' cellspacing='0' style='{10}{3}'>
//    				<tr>
//    					<td>
//							<div id='dropdowntree{0}'>
//				", this.ClientID/*0*/
//                , this.DropDownTreeBasePath/*1*/
//                , this.Width.Value.ToString( "0" ) + "px"/*2*/
//                , strOpenStatus/*3*/
//                , openIcon /*4*/
//                , openByClickTextScript /*5*/
//                , this.CssClassDropdownBox /*6*/
//                , this.CssClassDropdownTree /*7*/
//                , this.CssClassDropdownIcon /*8*/
//                , this.Width.Value.ToString( "0" ) /*9*/
//                , "z-index:999;position:absolute;"/*10*/);

//            string initialDropdownHTML3 = string.Format( @"</div>
//    					</td>
//    				</tr>
//				</table>
//			</div>
//			", this.ClientID/*0*/
//             , this.DropDownTreeBasePath/*1*/
//             , this.Width.Value.ToString( "0" ) + "px"/*2*/
//             , strOpenStatus/*3*/
//             , openIcon /*4*/
//             , openByClickTextScript /*5*/
//             , this.CssClassDropdownBox /*6*/
//             , this.CssClassDropdownTree /*7*/
//             , this.CssClassDropdownIcon /*8*/
//             , this.Width.Value.ToString( "0" ) /*9*/
//             , "z-index:999;position:absolute;"/*10*/);

            #endregion

            //this.tbDropDownContainer.RenderControl( writer );
            //this.Controls.Remove( tbDropDownContainer );
            //writer.Write( initialDropdownHTML1 );
            //this.rfvSelected.RenderControl( writer );
            //this.rfvChecked.RenderControl( writer );
            //writer.Write( initialDropdownHTML2 );
            //writer.Write( initialDropdownHTML3 );

            base.RenderContents( writer );

            #region initialDropdownJS

            string strForceCloseByClickIcon = !this.EnableCloseOnOutsideClick ? "//" : string.Empty;
            string initialDropdownJS = string.Format( @"
            <script type=""text/javascript"">
                function first{0}( elem ) {{
                    elem = elem.firstChild;
                    return elem && elem.nodeType != 1 ? next{0}( elem ) : elem;
                }}

                function next{0}( elem ) {{
                    do {{
                        elem = elem.nextSibling;
                    }} while ( elem && elem.nodeType != 1 );
                    
                    return elem;
                }}

                function onDropdownClick{0}(visibility){{
                    {18}try{{
                        {19}{20}
                    {18}}}catch(err){{}}
                }}


                function toggleDropdownTreeContainer{0}( containerId ){{
                    var isVisible = document.getElementById( containerId ).style.visibility;

                    var visibility = ( isVisible =='hidden'?'visible':'hidden');
                    
                    var returnVal = onDropdownClick{0}(visibility);
                    if( returnVal != null && returnVal === false ){{
                        return;
                    }}

                    document.getElementById( containerId ).style.visibility = ( isVisible =='hidden'?'visible':'hidden');

                    var firstChild = first{0}( document.getElementById( containerId ));
                    if( firstChild ){{
                        if( firstChild.tagName == 'TBODY' )
                            firstChild = first{0}( firstChild );
                        
                        if( firstChild.tagName == 'TR' ){{
                            first{0}( firstChild ).style.display =  ( isVisible =='hidden'?'block':'none');

                        }}
                    }}

                    if( isVisible == 'visible' ){{
                        document.getElementById('{5}').value = 'hidden';
                        document.getElementById('{1}').src = document.getElementById('{1}').src.replace('{14}','{15}');
                        showSelect{0}();
                    }}
                    else{{
                        document.getElementById('{5}').value = 'visible';
                        document.getElementById('{1}').src = document.getElementById('{1}').src.replace('{15}','{14}');
                        hideSelect{0}();
                    }}
                }}

                function setDisplayText{0}( val ){{
                    document.getElementById('{2}').innerHTML = val;
                    document.getElementById('{11}').value = encodeURIComponent( val );
                }}

        
                function dropdownTreeSetup{0}(){{
                    //some setup code
                    /*
                    //initial div
                    var divNode = document.getElementById( '{2}' );
                    //alert(divNode.parentNode.clientWidth );
                    var pWidth = parseInt( divNode.parentNode.clientWidth ) - 19;
                    if(pWidth>=0)
                        divNode.style.width = pWidth + 'px';

                    setDisplayText{0}(decodeURIComponent('{10}'));
                    */
                    //alert(document.getElementById( '{5}' ).value);
                    if(	document.getElementById( '{5}' ).value == 'hidden' )
                        showSelect{0}();
                    else if( document.getElementById( '{5}' ).value == 'visible' )
                        hideSelect{0}();

                    {13}addEvent{0}( document, 'click', bodyClickHandler{0}, false);
                }}

                function dropdownTreeWindowLoadHandler{0}(){{
                    setTimeout(""dropdownTreeSetup{0}()"",100);//ie Sucks!!!
                }}
                
/*
                if (window.addEventListener) {{
                    window.addEventListener('load', dropdownTreeWindowLoadHandler{0}, false );
                }}
                else if (window.attachEvent) {{
                    var r = window.attachEvent('onload', dropdownTreeWindowLoadHandler{0} );
                }}
                else {{
                    window['onload'] = dropdownTreeWindowLoadHandler{0};
                }}
*/

/*
                if( _gt.ASTreeViewHelper.isIE )
                    window.setTimeout('dropdownTreeWindowLoadHandler{0}()',30);
                else
                    dropdownTreeWindowLoadHandler{0}();
*/			


                function addEvent{0}(elm, evType, fn, useCapture) {{
                    if (elm.addEventListener) {{
                        elm.addEventListener(evType, fn, useCapture);
                        return true;
                    }}
                    else if (elm.attachEvent) {{
                        var r = elm.attachEvent('on' + evType, fn);
                        return r;
                    }}
                    else {{
                        elm['on' + evType] = fn;
                    }}
                }}

                function bodyClickHandler{0}( e ){{

                    e = e || window.event;
                    var target = e.target || e.srcElement;
                    
                    var isFoundInContainer = false;
                    var node = target;
                    while( node.parentNode != null ){{
                        if( node.id.indexOf( '{0}' ) != -1 )
                        {{
                            isFoundInContainer = true;
                        }}
                        node = node.parentNode;
                    }}
                    
                    if( !isFoundInContainer ){{
                        var container = document.getElementById('{17}');
                        if( container )
                        {{
                            if( container.style.visibility == 'visible' ){{
                                document.getElementById('{5}').value = 'hidden';
                                container.style.visibility = 'hidden';
                                
                                var firstChild = first{0}( container );
                                if( firstChild ){{
                                    if( firstChild.tagName == 'TBODY' )
                                        firstChild = first{0}( firstChild );
                                    
                                    if( firstChild.tagName == 'TR' )
                                        first{0}( firstChild ).style.display = 'none';
                                }}

                                document.getElementById('{1}').src = document.getElementById('{1}').src.replace('{14}','{15}');						
                                showSelect{0}();					
                            }}
                        }}
                    }}
                }}


                var selectList{0} = new Array();
                function hideSelect{0}(){{
                    
                    if ( msieversion{0}() <= 6 && document.all ){{
                        
                        var selects = document.getElementsByTagName('select');
                        
                        for( var i = 0; i < selects.length; i++ ){{
                            var oneSelect = selects[i];
                                    
                            if( !isInRange{0}( oneSelect) )
                                continue;
                                
                            if( oneSelect.style.visibility != 'hidden' ){{
                                oneSelect.style.visibility = 'hidden';
                                selectList{0}.push( oneSelect );
                            }}
                        }}
                    }}
                }}


                function showSelect{0}(){{
                    for( var i = 0; i < selectList{0}.length; i++ )
                        selectList{0}[i].style.visibility = 'visible';
                    
                    while( selectList{0}.length > 0 )
                        selectList{0}.pop();
                }}


                function getX{0}( oElement )
                {{
                    var iReturnValue = 0;
                    while( oElement != null ) {{
                        iReturnValue += oElement.offsetLeft;
                        oElement = oElement.offsetParent;
                    }}
                    return iReturnValue;
                }}

                function getY{0}( oElement )
                {{
                    var iReturnValue = 0;
                    while( oElement != null ) {{
                        iReturnValue += oElement.offsetTop;
                        oElement = oElement.offsetParent;
                    }}
                    return iReturnValue;
                }}


                function msieversion{0}()
                {{
                    var ua = window.navigator.userAgent
                    var msie = ua.indexOf ( 'MSIE' )

                    if ( msie > 0 )      // If Internet Explorer, return version number
                        return parseInt (ua.substring (msie+5, ua.indexOf ('.', msie )))
                    else                 // If another browser, return 0
                        return 0
                }}
                    
                    
                function isInRange{0}( elem ){{
                    
                    var containerDiv = document.getElementById( '{17}' );
                    var elemX1 = getX{0}( elem );
                    var elemY1 = getY{0}( elem );
                    var elemX2 = elem.offsetWidth + elemX1;
                    var elemY2 = elem.offsetHeight + elemY1;
                    
                    var containerX1 = getX{0}( containerDiv );
                    var containerY1 = getY{0}( containerDiv);
                    var containerX2 = containerDiv.offsetWidth + containerX1;
                    var containerY2 = containerDiv.offsetHeight + containerY1;
                    
                    //alert(1);
                    if( elemX1 < containerX1 && elemX2 < containerX1 )
                        return false;
                    
                    //alert(2);
                    if( elemX1 > containerX2 && elemX2 > containerX2 )
                        return false;
                    
                    //alert(3);
                    if( elemY1 < containerY1 && elemY2 < containerY1 )
                        return false;
                    
                    //alert(' elemY1:' + elemY1 + ' containerY2:' + containerY2 + ' elemY2:' + elemY2 + ' containerY2:' + containerY2);
                    if( elemY1 > containerY2 && elemY2 > containerY2 )
                        return false;
                    
                    //alert(5);
                    return true;
                }}
            </script>
            ", this.ClientID/*0*/
            , this.imgDropDrownIcon.ClientID /*1*/
            , this.divDropdownTreeText.ClientID /*2*/
            , ""/*3*/
            , ""/*4*/
            , this.txtOpenStatus.ClientID /*5*/
            , ""/*6*/
            , ""/*7*/
            , ""/*8*/
            , ""/*9*/
            , this.txtDisplayText.Text.Trim().Replace( "'", "\\'" ).Replace( "\"", "\\\"" )/*10*/
            , this.txtDisplayText.ClientID /*11*/
            , "" /*12*/
            , strForceCloseByClickIcon /*13*/
            , this.Enabled ? this.DropdownIconUp : this.DropdownIconUpDisabled /*14*/
            , this.Enabled ? this.DropdownIconDown : this.DropdownIconDownDisabled /*15*/
            , ""/*16*/
            , this.tbDropdownTreeObjectContainer.ClientID /*17*/
            , this.EnableManageJSError ? "" : "//"  /*18*/
            , this.EnableOnDropDownClickScriptReturn ? "return " : string.Empty /*19*/ 
            , this.OnDropDownClickScript /*20*/
            );
            
            if( this.Enabled )
            {
                if( ScriptManager.GetCurrent( this.Page ) != null )
                {
                    ScriptManager.RegisterStartupScript( this.Page
                        , this.Page.GetType()
                        , "js" + Guid.NewGuid()
                        , initialDropdownJS
                        , false );
                }
                else
                {

                    this.Page.ClientScript.RegisterStartupScript( this.Page.GetType()
                        , "js" + Guid.NewGuid()
                        , initialDropdownJS );
                }
            }

            #region register intial script

            string postbackScript = string.Format( @"
                <script type='text/javascript'>dropdownTreeWindowLoadHandler{0}();</script>
", this.ClientID /*0*/ );

            string initialScript = string.Format( @"
<script type='text/javascript'>
if( _gt.ASTreeViewHelper.isIE ){{
    if (window.addEventListener) {{
        window.addEventListener('load', dropdownTreeWindowLoadHandler{0}, false );
    }}
    else if (window.attachEvent) {{
        var r = window.attachEvent('onload', dropdownTreeWindowLoadHandler{0} );
    }}
    else {{
        window['onload'] = dropdownTreeWindowLoadHandler{0};
    }}
}}
else
    dropdownTreeWindowLoadHandler{0}();
</script>
", this.ClientID /*0*/  );

            if( this.Enabled )
            {
                if( base.IsInPostback() )
                {

                    if( base.IsInUpdateAsyncPostback() )
                    {
                        ScriptManager.RegisterStartupScript( this.Page
                            , this.Page.GetType()
                            , "js" + Guid.NewGuid()
                            , postbackScript
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
                else
                {
                    if( ScriptManager.GetCurrent( this.Page ) != null )
                    {
                        ScriptManager.RegisterStartupScript( this.Page
                            , this.Page.GetType()
                            , "js" + Guid.NewGuid()
                            , postbackScript
                            , false );
                    }
                    else
                    {

                        this.Page.ClientScript.RegisterStartupScript( this.Page.GetType()
                            , "js" + Guid.NewGuid()
                            , postbackScript
                            , false );
                    }
                }
            }

            #endregion

            #endregion
        }

        #endregion

        #region protected methods

        #region InitializeControls

        /// <summary>
        /// Initializes the controls.
        /// </summary>
        new virtual protected void InitializeControls()
        {
            this.txtOpenStatus.ID = "txtOpenStatus";
            this.txtDisplayText.ID = "txtDisplayText";
            this.txtSelectedValue.ID = "txtSelectedValue";
            this.txtCheckedValues.ID = "txtCheckedValues";

            this.tbDropDown.ID = "tbDropDown";
            this.tbDropDown.ID = "tbDropDown";
            this.trDropDownTR.ID = "trDropDownTR";
            this.tdDropDownTD1.ID = "tdDropDownTD1";
            this.tdDropDownTD2.ID = "tdDropDownTD2";
            this.tdDropDownTD3.ID = "tdDropDownTD3";
            this.divDropdownTreeText.ID = "divDropdownTreeText";
            this.aDropDrownIcon.ID = "aDropDrownIcon";
            this.imgDropDrownIcon.ID = "imgDropDrownIcon";

            this.tbDropdownTreeObjectContainer.ID = "tbDropdownTreeObjectContainer";
            this.trDropdownTreeObjectContainerTR.ID = "trDropdownTreeObjectContainerTR";
            this.tdDropdownTreeObjectContainerTD.ID = "tdDropdownTreeObjectContainerTD";
            this.divDropdownTreeObjectContainer.ID = "divDropdownTreeObjectContainer";

            this.divDebug.ID = "divDebug";

            this.rfvSelected.ID = "rfvSelected";
            this.rfvSelected.EnableViewState = true;
            this.rfvSelected.ControlToValidate = this.txtSelectedValue.ID;

            this.rfvChecked.ID = "rfvChecked";
            this.rfvChecked.EnableViewState = true;
            this.rfvChecked.ControlToValidate = this.txtCheckedValues.ID;

            InitializeValidator();

            this.EnsureChildControls();

        }

        #endregion 

        #region InitializeValidator

        /// <summary>
        /// Initializes the validator.
        /// </summary>
        virtual protected void InitializeValidator()
        {
            this.rfvChecked.ForeColor = this.RequiredValidatorColor;
            this.rfvChecked.Display = this.RequiredValidatorDisplay;
            this.rfvChecked.ErrorMessage = this.RequiredValidatorErrorMessage;
            this.rfvChecked.Text = this.RequiredValidatorText;
            this.rfvChecked.Enabled = this.EnableRequiredValidator;
            if( !string.IsNullOrEmpty( this.RequiredValidatorValidationGroup ) && this.EnableCheckbox )
                this.rfvChecked.ValidationGroup = this.RequiredValidatorValidationGroup;
            this.rfvChecked.CssClass = this.CssClassRequiredValidator;

            this.rfvSelected.ForeColor = this.RequiredValidatorColor;
            this.rfvSelected.Display = this.RequiredValidatorDisplay;
            this.rfvSelected.ErrorMessage = this.RequiredValidatorErrorMessage;
            this.rfvSelected.Text = this.RequiredValidatorText;
            this.rfvSelected.Enabled = this.EnableRequiredValidator;
            if( !string.IsNullOrEmpty( this.RequiredValidatorValidationGroup ) && this.EnableNodeSelection )
                this.rfvSelected.ValidationGroup = this.RequiredValidatorValidationGroup;
            this.rfvSelected.CssClass = this.CssClassRequiredValidator;

            this.rfvChecked.Enabled = this.EnableCheckbox && this.EnableRequiredValidator;

            this.rfvSelected.Enabled = base.EnableNodeSelection && this.EnableRequiredValidator;
        }

        #endregion

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

        #region public methods

        /// <summary>
        /// Validates the check select.
        /// </summary>
        public void ValidateCheckSelect()
        {
            if( this.rfvChecked.Enabled )
                this.rfvChecked.Validate();

            if( this.rfvSelected.Enabled )
                this.rfvSelected.Validate();
        }

        #endregion
    }
}
