using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;

using Goldtect.Utilities;
using Goldtect.Utilities.Json;
using Goldtect.Utilities.Xml;
using System.Collections;
using System.Web;

namespace Goldtect
{
    /// <summary>
    /// The ASTreeView Helper
    /// </summary>
    ///<exclude/>
    public class ASTreeViewHelper
    {
        #region Declaration

        private int liCounter = 0;

        #endregion

        #region Properties

        #region CurrentTreeView
        private ASTreeView currentTreeView;

        /// <summary>
        /// Gets or sets the current astreeview object.
        /// </summary>
        /// <value>The current tree view.</value>
        public ASTreeView CurrentTreeView
        {
            get { return currentTreeView; }
            set { currentTreeView = value; }
        }
        #endregion

        #endregion

        #region public ParseJsonTree

        /// <summary>
        /// Parses the json tree.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <param name="treeJsonString">The tree json string.</param>
        /// <returns></returns>
        public ASTreeViewNode ParseJsonTree( ASTreeViewNode root , string treeJsonString )
        {
            //if root is null, genereate the root
            if( root == null )
            {
                root = new ASTreeViewNode();
                root.NodeValue = this.currentTreeView.RootNodeValue;
                root.NodeText = this.currentTreeView.RootNodeText;
            }

            if( string.IsNullOrEmpty( treeJsonString ) )
                return root;

            JsonArray treeJson;
            using( JsonParser parser = new JsonParser( new StringReader( treeJsonString ), true ) )
            {
                parser.MaximumDepth = this.currentTreeView.MaxDepth;
                treeJson = parser.ParseArray();
            }

            this.ParseJsonTreeRecursive( root, treeJson );

            //if( this.currentTreeView.EnableThreeStateCheckbox )
            //	this.ManageCheckboxState( root );

            return root;
        }

        #endregion

        #region public ParseJsonToNode

        private string GetObfuscatedString( string val )
        {
            bool isDebug = this.currentTreeView.EnableDebugMode;
            return isDebug ? val : this.ObfuscateScript( val );
        }
        /// <summary>
        /// Parses the json to node.
        /// </summary>
        /// <param name="joTreeNode">The jo tree node.</param>
        /// <returns></returns>
        public ASTreeViewNode ParseJsonToNode( JsonObject joTreeNode )
        {
            
            JsonString nodeText = (JsonString)joTreeNode[ this.GetObfuscatedString( "node_treeNodeText" )];
            JsonString nodeValue = (JsonString)joTreeNode[this.GetObfuscatedString( "node_treeNodeValue" )];
            JsonNumber checkedState = (JsonNumber)joTreeNode[this.GetObfuscatedString( "node_checkedState" )];
            JsonNumber openState = (JsonNumber)joTreeNode[this.GetObfuscatedString( "node_openState" )];
            JsonBoolean selected = (JsonBoolean)joTreeNode[this.GetObfuscatedString( "node_selected" )];
            JsonBoolean enableEditContextMenu = (JsonBoolean)joTreeNode[this.GetObfuscatedString( "node_enableEditContextMenu" )];
            JsonBoolean enableDeleteContextMenu = (JsonBoolean)joTreeNode[this.GetObfuscatedString( "node_enableDeleteContextMenu" )];
            JsonBoolean enableAddContextMenu = (JsonBoolean)joTreeNode[this.GetObfuscatedString( "node_enableAddContextMenu" )];
            JsonNumber treeNodeType = (JsonNumber)joTreeNode[this.GetObfuscatedString( "node_treeNodeType" )];
            JsonString treeNodeIcon = null;
            if( joTreeNode.ContainsKey(this.GetObfuscatedString( "node_treeNodeIcon" )) )
                treeNodeIcon = (JsonString)joTreeNode[this.GetObfuscatedString( "node_treeNodeIcon" )];

            ASTreeViewNodeType astNodeType = (ASTreeViewNodeType)( (int)treeNodeType.Value );

            JsonBoolean enableDragDrop = (JsonBoolean)joTreeNode[this.GetObfuscatedString( "node_enableDragDrop" )];
            JsonBoolean enableSiblings = (JsonBoolean)joTreeNode[this.GetObfuscatedString( "node_enableSiblings" )];
            JsonBoolean enableChildren = (JsonBoolean)joTreeNode[this.GetObfuscatedString( "node_enableChildren" )];
            JsonBoolean enableCheckbox = (JsonBoolean)joTreeNode[this.GetObfuscatedString( "node_enableCheckbox" )];
            JsonBoolean enableSelection = (JsonBoolean)joTreeNode[this.GetObfuscatedString( "node_enableSelection" )];
            JsonBoolean enableOpenClose = (JsonBoolean)joTreeNode[this.GetObfuscatedString( "node_enableOpenClose" )];

            ASTreeViewNode node;// = new ASTreeViewNode();
            switch( astNodeType )
            {
                case ASTreeViewNodeType.LinkButton:
                    node = new ASTreeViewNode();
                    break;
                case ASTreeViewNodeType.HyperLink:
                    node = new ASTreeViewLinkNode();
                    break;
                case ASTreeViewNodeType.Text:
                    node = new ASTreeViewTextNode();
                    break;
                default:
                    node = new ASTreeViewNode();
                    break;
            }

            node.NodeText = HttpUtility.UrlDecode( nodeText.Value );
            node.NodeValue = HttpUtility.UrlDecode( nodeValue.Value );
            node.CheckedState = (ASTreeViewCheckboxState)( (int) checkedState.Value );
            node.OpenState = (ASTreeViewNodeOpenState)( (int)openState.Value );
            node.Selected = selected.Value;
            node.EnableEditContextMenu = enableEditContextMenu.Value;
            node.EnableDeleteContextMenu = enableDeleteContextMenu.Value;
            node.EnableAddContextMenu = enableAddContextMenu.Value;

            node.EnableDragDrop = enableDragDrop.Value;
            node.EnableSiblings = enableSiblings.Value;
            node.EnableChildren = enableChildren.Value;
            node.EnableCheckbox = enableCheckbox.Value;
            node.EnableSelection = enableSelection.Value;
            node.EnableOpenClose = enableOpenClose.Value;

            #region handle tree node depth

            if( this.currentTreeView.EnableFixedDepthDragDrop )
            {
                JsonNumber treeNodeDepth = null;
                if( joTreeNode.ContainsKey( this.GetObfuscatedString( "node_treeNodeDepth" ) ) )
                {
                    treeNodeDepth = (JsonNumber)joTreeNode[this.GetObfuscatedString( "node_treeNodeDepth" )];
                    node.NodeDepth = (int)treeNodeDepth.Value;
                }

            }

            #endregion

            #region handle virtual node
            JsonBoolean isVirtualNode = null;
            if( joTreeNode.ContainsKey( this.GetObfuscatedString( "node_isVirtualNode" ) ) )
            {
                isVirtualNode = (JsonBoolean)joTreeNode[this.GetObfuscatedString( "node_isVirtualNode" )];
                node.IsVirtualNode = isVirtualNode.Value;
            }

            JsonNumber virtualNodesCount = null;
            if( joTreeNode.ContainsKey( this.GetObfuscatedString( "node_virtualNodesCount" ) ) )
            {
                virtualNodesCount = (JsonNumber)joTreeNode[this.GetObfuscatedString( "node_virtualNodesCount" )];
                node.VirtualNodesCount = (int)virtualNodesCount.Value;
            }

            JsonString virtualParentKey = null;
            if( joTreeNode.ContainsKey( this.GetObfuscatedString( "node_virtualParentKey" ) ) )
            {
                virtualParentKey = (JsonString)joTreeNode[this.GetObfuscatedString( "node_virtualParentKey" )];
                node.VirtualParentKey = virtualParentKey.Value;
            }

            JsonString jsAttr = null;
            if( joTreeNode.ContainsKey( this.GetObfuscatedString( "node_additionalAttributes" ) ) )
            {
                jsAttr = (JsonString)joTreeNode[this.GetObfuscatedString( "node_additionalAttributes" )];
                string attrString = jsAttr.Value;

                if( !string.IsNullOrEmpty( attrString ) )
                {
                    try
                    {
                        StringReader rdr = new StringReader( attrString );
                        JsonParser parser = new JsonParser( rdr, true );
                        JsonObject jo = (JsonObject)parser.ParseObject();
                        List<KeyValuePair<string, string>> additionalAttributes = new List<KeyValuePair<string, string>>();
                        foreach( string key in jo.Keys )
                            additionalAttributes.Add( new KeyValuePair<string,string>( key, ( (JsonString)jo[key] ).Value ));

                        node.AdditionalAttributes = additionalAttributes;
                    }
                    catch { }
                }

            }


            #endregion


            //handle hyperlink
            if( astNodeType == ASTreeViewNodeType.HyperLink )
            {
                ASTreeViewLinkNode hlNode = (ASTreeViewLinkNode)node;
                JsonString href = (JsonString)joTreeNode[this.GetObfuscatedString( "node_href" )];
                JsonString target = (JsonString)joTreeNode[this.GetObfuscatedString( "node_target" )];
                JsonString tooltip = (JsonString)joTreeNode[this.GetObfuscatedString( "node_tooltip" )];

                hlNode.NavigateUrl = href.Value;
                hlNode.Target = target.Value;
                hlNode.Tooltip = tooltip.Value;
            }

            if( treeNodeIcon != null )
                node.NodeIcon = treeNodeIcon.Value;

            return node;
        }

        #endregion

        #region private ParseJsonTreeRecursive

        /// <summary>
        /// Parses the json tree recursive.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="treeJson">The tree json.</param>
        private void ParseJsonTreeRecursive( ASTreeViewNode parent, JsonArray treeJson )
        {
            for( int i = 0; i < treeJson.Count; i++ )
            {
                IJsonType type = treeJson[i];

                if( type is JsonObject )
                {
                    JsonObject joTreeNode = (JsonObject)type;
                    ASTreeViewNode curNode = ParseJsonToNode( joTreeNode );

                    if( i + 1 < treeJson.Count )
                    {
                        //test next item
                        IJsonType nextType = treeJson[i + 1];
                        if( nextType is JsonArray )
                        {
                            //has children
                            ParseJsonTreeRecursive( curNode, (JsonArray)nextType );

                            //skip next
                            i++;
                        }
                    }

                    parent.AppendChild( curNode );
                }

            }
        }

        #endregion

        #region public ConvertTree

        /// <summary>
        /// Converts the tree.
        /// </summary>
        /// <param name="ulTree">The ul tree.</param>
        /// <param name="root">The root.</param>
        /// <param name="includeRoot">if set to <c>true</c> [include root].</param>
        public void ConvertTree( HtmlGenericControl ulTree, ASTreeViewNode root, bool includeRoot )
        {
            //BulletedList blTree = new BulletedList();

            if( includeRoot )
            {
                HtmlGenericControl liRoot = this.ConvertNode( root );
                liRoot.ID = string.Format( "t_l_{0}", liCounter++/*, new Random().Next( 1000, 9000 )*/ );
                liRoot.Attributes.Add( "disableDragDrop", "true" );
                liRoot.Attributes.Add( "disableSiblings", "true" );
                //liRoot.Attributes.Add( "noDelete", "true" );
                //liRoot.Attributes.Add( "noRename", "true" );
                if( this.currentTreeView.EnableTreeLines )
                    liRoot.Attributes.Add( "class", this.currentTreeView.CssClassLineRoot/*"line-bottom"*/ );

                ulTree.Controls.Add( liRoot );

                if( root.ChildNodes.Count > 0 )
                {

                    HtmlGenericControl ulRoot = new HtmlGenericControl();
                    //ulRoot.ID = string.Format( "ul_node_{0}", ulCounter++ );
                    ulRoot.TagName = "ul";
                    if( root.OpenState == ASTreeViewNodeOpenState.Close )
                        ulRoot.Attributes.Add( "style", "display:none;" );
                    liRoot.Controls.Add( ulRoot );

                    ConvertTreeRecursive( ulRoot, root );
                }
            }
            else
            {
                liCounter++;
                ConvertTreeRecursive( ulTree, root );
            }


            //controlContinaer.Controls.Add( blTree );
        }
        
        #endregion

        #region public ConvertTreeRecursive

        /// <summary>
        /// Converts the tree recursive.
        /// </summary>
        /// <param name="ulParent">The ul parent.</param>
        /// <param name="node">The node.</param>
        public void ConvertTreeRecursive( HtmlGenericControl ulParent, ASTreeViewNode node )
        {
            foreach( ASTreeViewNode child in node.ChildNodes )
            {
                HtmlGenericControl liCurrent = ConvertNode( child );
                liCurrent.ID = string.Format( "t_l_{0}", liCounter++/*, new Random().Next(1000, 9000)*/ );
                ulParent.Controls.Add( liCurrent );

                if( child.ChildNodes.Count > 0 )
                {
                    HtmlGenericControl ulCurrent = new HtmlGenericControl();
                    ulCurrent.TagName = "ul";
                    if( child.OpenState == ASTreeViewNodeOpenState.Close  )
                        ulCurrent.Attributes.Add( "style" , "display:none;" );

                    if( this.currentTreeView.EnableTreeLines )
                    {
                        if( !( child.ParentNode != null && child.ParentNode.ChildNodes.IndexOf( child ) == child.ParentNode.ChildNodes.Count - 1 ) )
                            ulCurrent.Attributes.Add( "class", this.currentTreeView.CssClassLineVertical /*"line-vertical"*/ );
                        else
                            ulCurrent.Attributes.Add( "class", this.currentTreeView.CssClassLineNone /*"line-none"*/ );
                    }

                    liCurrent.Controls.Add( ulCurrent );

                    this.ConvertTreeRecursive( ulCurrent, child );
                }
            }
        }

        #endregion 

        #region public AddAdditionalAttributes

        /// <summary>
        /// Adds the additional attributes.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="attrs">The attrs.</param>
        public void AddAdditionalAttributes( WebControl control, List<KeyValuePair<string, string>> attrs )
        {
            foreach( KeyValuePair<string, string> attr in attrs )
            {
                if( attr.Key == "class" )
                {
                    if( control.CssClass.IndexOf( attr.Value ) < 0 )
                        control.CssClass += ( " " + attr.Value );
                }
                else
                    control.Attributes.Add( attr.Key, attr.Value );
            }
        }

        #endregion

        #region public ConvertNode

        /// <summary>
        /// Converts the node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        public HtmlGenericControl ConvertNode( ASTreeViewNode node )
        {
            #region manage li

            HtmlGenericControl li = new HtmlGenericControl();
            li.TagName = "li";
            li.Attributes.Add( "treeNodeType", ( (int)node.NodeType ).ToString() );

            #endregion

            #region manage selection

            li.Attributes.Add( "treeNodeValue", node.NodeValue );
            if( node.Selected )
                li.Attributes.Add( "selected", "true" );

            #endregion

            #region manage treelines

            if( this.currentTreeView.EnableTreeLines )
            {

                //if the current node is the last node of parent's childnodes.
                if( node.ParentNode != null && node.ParentNode.ChildNodes.IndexOf( node ) == ( node.ParentNode.ChildNodes.Count - 1 ) )
                {
                    if( ( this.currentTreeView.EnableRoot && node == this.currentTreeView.RootNode )
                            ||
                            ( !this.currentTreeView.EnableRoot
                            && this.currentTreeView.RootNode.ChildNodes.Count == 1
                            && node == this.currentTreeView.RootNode.ChildNodes[0] )
                        )
                    {
                        //if the current node is root node.
                        li.Attributes.Add( "class", this.currentTreeView.CssClassLineRoot /* "line-root"*/ );
                    }
                    else
                        li.Attributes.Add( "class", this.currentTreeView.CssClassLineBottom /* "line-bottom"*/ );
                }
                // if the current is the root node and has siblings.
                else if( !this.currentTreeView.EnableRoot
                            && this.currentTreeView.RootNode.ChildNodes.Count > 1
                            && node == this.currentTreeView.RootNode.ChildNodes[0]
                        )
                {
                    li.Attributes.Add( "class", this.currentTreeView.CssClassLineTop /* "line-top"*/ );
                }
                else
                    li.Attributes.Add( "class", this.currentTreeView.CssClassLineMiddle /*"line-middle"*/ );
            }

            #endregion

            #region manage open close

            li.Attributes.Add( "openState", ( (int)node.OpenState ).ToString() );

            #endregion

            #region manage node icon

            //set li attribute for save viewstate
            if( !string.IsNullOrEmpty( node.NodeIcon ) )
                li.Attributes.Add( "treeNodeIcon", node.NodeIcon );

            #endregion

            #region manage context menu attributes

            li.Attributes.Add( "enable-edit-context-menu", node.EnableEditContextMenu ? "true" : "false" );
            li.Attributes.Add( "enable-delete-context-menu", node.EnableDeleteContextMenu ? "true" : "false" );
            li.Attributes.Add( "enable-add-context-menu", node.EnableAddContextMenu ? "true" : "false" );

            #endregion

            #region manage node drag drop
            if( !node.EnableDragDrop )
                li.Attributes.Add( "enable-drag-drop", "false" );
            #endregion

            #region manage node siblings
            if( !node.EnableSiblings )
                li.Attributes.Add( "enable-siblings", "false" );
            #endregion

            #region manage node children
            if( !node.EnableChildren )
                li.Attributes.Add( "enable-children", "false" );
            #endregion

            #region manage enable checkbox
            if( !node.EnableCheckbox )
                li.Attributes.Add( "enable-checkbox", "false" );
            #endregion

            #region manage enable selection
            if( !node.EnableSelection )
                li.Attributes.Add( "enable-selection", "false" );
            #endregion


            #region manage enable open close
            if( !node.EnableOpenClose )
                li.Attributes.Add( "enable-open-close", "false" );
            #endregion

            #region manage line (not implemented)

            /*
            HtmlGenericControl divLine = new HtmlGenericControl();
            divLine.TagName = "div";
            divLine.InnerHtml = "&nbsp;";
            divLine.Attributes.Add( "class", "line" );

            //HtmlImage imgLine = new HtmlImage();
            //imgLine.Src = "javascript/astreeview/images/astreeview-vertical-line.gif";
            li.Controls.Add( divLine );
            */
            #endregion

            #region manage plus minus icon

            HtmlImage imgPlusMinus = new HtmlImage();
            imgPlusMinus.Attributes.Add( "icon-type", ( (int)ASTreeViewIconType.OpenClose ).ToString() );
            imgPlusMinus.Attributes.Add( "class", this.currentTreeView.CssClassPlusMinusIcon );

            switch( node.OpenState )
            {
                case ASTreeViewNodeOpenState.Open:
                    imgPlusMinus.Src = this.currentTreeView.ImgMinusIcon;
                    break;
                case ASTreeViewNodeOpenState.Close:
                    imgPlusMinus.Src = this.currentTreeView.ImgPlusIcon;
                    break;
                default:
                    imgPlusMinus.Src = this.currentTreeView.ImgMinusIcon;
                    break;
            }


            if( node.ChildNodes.Count == 0 || !node.EnableOpenClose )
                imgPlusMinus.Style.Add( "visibility", "hidden" );

            li.Controls.Add( imgPlusMinus );

            #endregion

            #region manage checkbox

            HtmlImage imgCheckbox = null;
            if( this.currentTreeView.EnableCheckbox )
            {
                imgCheckbox = this.ConvertCheckbox( node );
                imgCheckbox.Attributes.Add( "icon-type", ( (int)ASTreeViewIconType.Checkbox ).ToString() );

                #region handle postback
                
                if( this.currentTreeView.AutoPostBack )
                    imgCheckbox.Attributes.Add( "postbackscript", this.CurrentTreeView.Page.ClientScript.GetPostBackEventReference( this.CurrentTreeView, node.NodeValue ) );
                
                #endregion

                li.Controls.Add( imgCheckbox );

                #region manage checkbox for single node

                if( !node.EnableCheckbox )
                    imgCheckbox.Style.Add( HtmlTextWriterStyle.Display, "none" );

                #endregion
            }

            li.Attributes.Add( "checkedState", ( (int)node.CheckedState ).ToString() );

            #endregion

            #region manage folder icon
            if( this.currentTreeView.EnableNodeIcon )
            {
                HtmlImage imgIcon = new HtmlImage();
                imgIcon.Attributes.Add( "icon-type", ( (int)ASTreeViewIconType.NodeIcon ).ToString() );

                imgIcon.Attributes.Add( "class", this.currentTreeView.CssClassIcon );

                if( this.currentTreeView.EnableCustomizedNodeIcon && !string.IsNullOrEmpty( node.NodeIcon ) )
                {
                    //if node has custom icon
                    imgIcon.Src = node.NodeIcon;
                }
                else
                {
                    //use default icons
                    if( node.ChildNodes.Count > 0 )
                    {
                        if( node.OpenState == ASTreeViewNodeOpenState.Open )
                            imgIcon.Src = this.currentTreeView.DefaultFolderOpenIcon;
                        else
                            imgIcon.Src = this.currentTreeView.DefaultFolderIcon;
                    }
                    else
                        imgIcon.Src = this.currentTreeView.DefaultNodeIcon;

                    //if node is virtual node
                    if( node.IsVirtualNode && node.VirtualNodesCount > 0 )
                        imgIcon.Src = this.currentTreeView.DefaultFolderIcon;
                }

                //add drag & drap ability
                if( this.currentTreeView.EnableDragDropOnIcon )
                    imgIcon.Attributes.Add( "is-astreeview-node", "true" );

                li.Controls.Add( imgIcon );
            }

            #endregion

            #region manage link
            
            int counter = liCounter + 1;

            if( node.NodeType == ASTreeViewNodeType.LinkButton )
            {

                LinkButton lb = new LinkButton();
                lb.ID = "lbASTreeNode" + counter.ToString();
                lb.Text = node.NodeText;
                string disableClickScript = "return false;";
                if( !this.currentTreeView.AutoPostBack )
                    lb.Attributes.Add( "onclick", disableClickScript );
                if( node.Selected )
                    lb.CssClass += " astreeview-node-selected";

                if( this.currentTreeView.EnableContextMenu )
                {
                    lb.CssClass += ( " " + this.currentTreeView.ContextMenuTargetCssClass );

                    if( !node.EnableAddContextMenu || !this.currentTreeView.EnableContextMenuAdd )
                        lb.Attributes.Add( "disable" + this.currentTreeView.ContextMenuAddCommandName, "true" );

                    if( !node.EnableEditContextMenu || !this.currentTreeView.EnableContextMenuEdit )
                        lb.Attributes.Add( "disable" + this.currentTreeView.ContextMenuEditCommandName, "true" );

                    if( !node.EnableDeleteContextMenu || !this.currentTreeView.EnableContextMenuDelete )
                        lb.Attributes.Add( "disable" + this.currentTreeView.ContextMenuDeleteCommandName, "true" );
                }

                this.AddAdditionalAttributes( lb, node.AdditionalAttributes );

                //add astreeviewnode tag
                lb.Attributes.Add( "is-astreeview-node", "true" );
                li.Controls.Add( lb );
            }
            else if( node.NodeType == ASTreeViewNodeType.HyperLink )
            {
                ASTreeViewLinkNode linkNode = (ASTreeViewLinkNode)node;
                HyperLink hl = new HyperLink();
                
                hl.ID = "hlASTreeNode" + counter.ToString();
                hl.Text = linkNode.NodeText;
                if( linkNode.Selected )
                    hl.CssClass += " astreeview-node-selected";

                if( this.currentTreeView.EnableContextMenu )
                {
                    hl.CssClass += ( " " + this.currentTreeView.ContextMenuTargetCssClass );
                    if( !linkNode.EnableAddContextMenu  || !this.currentTreeView.EnableContextMenuAdd )
                        hl.Attributes.Add( "disable" + this.currentTreeView.ContextMenuAddCommandName, "true" );

                    if( !linkNode.EnableEditContextMenu || !this.currentTreeView.EnableContextMenuEdit )
                        hl.Attributes.Add( "disable" + this.currentTreeView.ContextMenuEditCommandName, "true" );

                    if( !linkNode.EnableDeleteContextMenu || !this.currentTreeView.EnableContextMenuDelete )
                        hl.Attributes.Add( "disable" + this.currentTreeView.ContextMenuDeleteCommandName, "true" );
                }

                hl.NavigateUrl = linkNode.NavigateUrl;
                hl.Target = linkNode.Target;
                hl.ToolTip = linkNode.Tooltip;


                this.AddAdditionalAttributes( hl, linkNode.AdditionalAttributes );

                //add astreeviewnode tag
                hl.Attributes.Add( "is-astreeview-node", "true" );
                li.Controls.Add( hl );
            }
            else if( node.NodeType == ASTreeViewNodeType.Text )
            {
                ASTreeViewTextNode textNode = (ASTreeViewTextNode)node;

                HtmlGenericControl div = new HtmlGenericControl( "div" );
                div.InnerHtml = textNode.NodeText;
                div.Attributes.Add( "class", this.currentTreeView.CssClassTextNodeContainer/* "astreeview-text-node"*/ );
                div.Attributes.Add( "is-astreeview-node", "true" );
                div.Attributes.Add( "isTreeNodeChild", "true" );
                li.Controls.Add( div );

                //for li
                string strLiCss = li.Attributes["class"];
                string strLiToAdd = this.currentTreeView.CssClassTextNode;
                li.Attributes["class"] = string.IsNullOrEmpty( strLiToAdd ) ? strLiToAdd : strLiCss + " " + strLiToAdd;

                //for plus minus
                string strPlusMinusCss = imgPlusMinus.Attributes["class"];
                string strPlusMinusToAdd = this.currentTreeView.CssClassPlusMinusTextNode;//"astreeview-plus-minus-text";
                imgPlusMinus.Attributes["class"] = string.IsNullOrEmpty( strPlusMinusCss ) ? strPlusMinusToAdd : strPlusMinusCss + " " + strPlusMinusToAdd;

                //for checkbox
                if( imgCheckbox != null )
                {
                    string strCheckboxCss = imgCheckbox.Attributes["class"];
                    string strCheckboxToAdd = this.currentTreeView.CssClassCheckboxTextNode;//"astreeview-checkbox-text";
                    imgCheckbox.Attributes["class"] = string.IsNullOrEmpty( strCheckboxCss ) ? strCheckboxToAdd : strCheckboxCss + " " + strCheckboxToAdd;
                }

            }

            #endregion

            #region manage virtual node

            if( node.IsVirtualNode )
            {
                li.Attributes.Add( "is-virtual-node", "true" );
                li.Attributes.Add( "virtual-nodes-count", node.VirtualNodesCount.ToString() );
                li.Attributes.Add( "virtual-parent-key", node.VirtualParentKey );

                li.Attributes["openState"] = ( (int)ASTreeViewNodeOpenState.Close ).ToString();
                imgPlusMinus.Style["visibility"] = "visible";
                imgPlusMinus.Src = this.currentTreeView.ImgPlusIcon;

                HtmlGenericControl ulVirtualNodesContainer = new HtmlGenericControl( "ul" );
                HtmlGenericControl liVirtualNodesContainer = new HtmlGenericControl( "li" );
                HtmlGenericControl divTextContainer = new HtmlGenericControl( "div" );
                liVirtualNodesContainer.Attributes.Add( "virtial-node-placeholder-li", "true" );
                ulVirtualNodesContainer.Attributes.Add( "virtial-node-placeholder-ul", "true" );
                
                ulVirtualNodesContainer.Controls.Add( liVirtualNodesContainer );
                ulVirtualNodesContainer.Style.Add( "display", "none" );
                divTextContainer.InnerHtml = string.Format( this.currentTreeView.VirtualNodePlaceHolderText, node.VirtualNodesCount ); //"loading..." + node.VirtualNodesCount.ToString();
                divTextContainer.Attributes.Add( "class", "astreeview-loading-placeholder" );
                liVirtualNodesContainer.Controls.Add( divTextContainer );
                li.Controls.Add( ulVirtualNodesContainer );

                /*
                HtmlGenericControl divVirtualNodesContainer = new HtmlGenericControl( "div" );
                divVirtualNodesContainer.InnerHtml = "loading..." + node.VirtualNodesCount.ToString();
                li.Controls.Add( divVirtualNodesContainer );
                */

            }

            #endregion

            #region manage additional attributes

            if( node.AdditionalAttributes.Count > 0 )
            {
                JsonObject joAdditionalAttr = new JsonObject();

                foreach( KeyValuePair<string, string> attr in node.AdditionalAttributes )
                {
                    if( !joAdditionalAttr.ContainsKey( attr.Key ) )
                        joAdditionalAttr.Add( attr.Key, attr.Value );
                }

                string attrString = string.Empty;
                using( JsonWriter writer = new JsonWriter() )
                {
                    joAdditionalAttr.Write( writer );
                    attrString = writer.ToString();
                }

                li.Attributes.Add( "additional-attributes", attrString );
            }

            #endregion

            #region manage right to left render

            if( this.currentTreeView.EnableRightToLeftRender )
            {
                //reverse child controls of li
                ArrayList al = new ArrayList();
                foreach( Control c in li.Controls )
                    al.Add( c );

                while( li.Controls.Count > 0 )
                    li.Controls.RemoveAt( 0 );

                al.Reverse();

                foreach( Control con in al )
                    li.Controls.Add( con );

            }

            #endregion

            #region manage node depth

            if( this.currentTreeView.EnableFixedDepthDragDrop )
                li.Attributes.Add( "tree-node-depth", node.NodeDepth.ToString() );

            #endregion

            return li;
        }

        #endregion

        #region private ConvertCheckbox

        /// <summary>
        /// Converts the checkbox.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        private HtmlImage ConvertCheckbox( ASTreeViewNode node )
        {
            HtmlImage imgCheckbox = new HtmlImage();
            imgCheckbox.Attributes.Add( "class", this.currentTreeView.CssClassCheckbox );


            switch( node.CheckedState )
            {
                case ASTreeViewCheckboxState.Checked:
                    imgCheckbox.Src = this.currentTreeView.ImgCheckboxChecked;
                    break;
                case ASTreeViewCheckboxState.HalfChecked:
                    imgCheckbox.Src = this.currentTreeView.ImgCheckboxHalfChecked;
                    break;
                case ASTreeViewCheckboxState.Unchecked:
                    imgCheckbox.Src = this.currentTreeView.ImgCheckboxUnchecked;
                    break;
                default:
                    imgCheckbox.Src = this.currentTreeView.ImgCheckboxUnchecked;
                    break;
            }


            imgCheckbox.Attributes.Add( "checkedState", ( (int)node.CheckedState ).ToString() );

            return imgCheckbox;
        }
        
        #endregion

        #region public ManageCheckboxState

        /// <summary>
        /// Manages the state of the check box.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        public ASTreeViewCheckboxState ManageCheckboxState( ASTreeViewNode node )
        {
            if( node.ChildNodes.Count == 0 )
                return node.CheckedState;

            int totalChildrenCount = node.ChildNodes.Count;
            int checkedNodesCount = 0;
            int halfCheckedNodesCount = 0;

            foreach( ASTreeViewNode curChildNode in node.ChildNodes )
            {
                ASTreeViewCheckboxState curState = this.ManageCheckboxState( curChildNode );

                if( curState == ASTreeViewCheckboxState.Checked )
                    checkedNodesCount++;

                if( curChildNode.CheckedState == ASTreeViewCheckboxState.Checked && curChildNode.ChildNodes.Count > 0 )
                    checkedNodesCount++;

                if( curChildNode.CheckedState == ASTreeViewCheckboxState.HalfChecked && curChildNode.ChildNodes.Count > 0 )
                    halfCheckedNodesCount++;
            }

            if( halfCheckedNodesCount > 0 )
                node.CheckedState = ASTreeViewCheckboxState.HalfChecked;
            else
            {
                if( checkedNodesCount == 0 )
                    node.CheckedState = ASTreeViewCheckboxState.Unchecked;
                else if( checkedNodesCount == totalChildrenCount )
                    node.CheckedState = ASTreeViewCheckboxState.Checked;
                else
                    node.CheckedState = ASTreeViewCheckboxState.HalfChecked;
            }

            return ASTreeViewCheckboxState.Unchecked;//return unchecked if not leaf node
        }

        #endregion

        #region public GetNodeJsonString

        /// <summary>
        /// Gets the node json string.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        public string GetNodeJsonString( ASTreeViewNode node )
        {
            using( JsonWriter writer = new JsonWriter() )
            {
                this.GetNodeJson( node ).Write( writer );

                return writer.ToString();
            }
        }

        #endregion

        #region public GetNodeJson

        /// <summary>
        /// Gets the node json.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        public IJsonType GetNodeJson( ASTreeViewNode node )
        {
            JsonArray jaNode = new JsonArray();
            JsonObject joNode = new JsonObject();

            joNode.Add( this.GetObfuscatedString( "node_treeNodeText" ), this.currentTreeView.EnableEscapeInput ? HttpUtility.UrlEncode( node.NodeText ) : node.NodeText );
            joNode.Add( this.GetObfuscatedString( "node_treeNodeValue" ), node.NodeValue );
            joNode.Add( this.GetObfuscatedString( "node_checkedState" ), (int)node.CheckedState );
            joNode.Add( this.GetObfuscatedString( "node_openState" ), (int)node.OpenState );
            joNode.Add( this.GetObfuscatedString( "node_selected" ), node.Selected );
            joNode.Add( this.GetObfuscatedString( "node_enableEditContextMenu" ), node.EnableEditContextMenu );
            joNode.Add( this.GetObfuscatedString( "node_enableDeleteContextMenu" ), node.EnableDeleteContextMenu );
            joNode.Add( this.GetObfuscatedString( "node_enableAddContextMenu" ), node.EnableAddContextMenu );
            joNode.Add( this.GetObfuscatedString( "node_treeNodeType" ), (int)node.NodeType );
            joNode.Add( this.GetObfuscatedString( "node_treeNodeIcon" ), node.NodeIcon );

            joNode.Add( this.GetObfuscatedString( "node_enableDragDrop" ), node.EnableDragDrop );
            joNode.Add( this.GetObfuscatedString( "node_enableSiblings" ), node.EnableSiblings );
            joNode.Add( this.GetObfuscatedString( "node_enableChildren" ), node.EnableChildren );
            joNode.Add( this.GetObfuscatedString( "node_enableCheckbox" ), node.EnableCheckbox );
            joNode.Add( this.GetObfuscatedString( "node_enableSelection" ), node.EnableSelection );
            joNode.Add( this.GetObfuscatedString( "node_enableOpenClose" ), node.EnableOpenClose );

            #region additionalAttributes

            string attrString = string.Empty;
            if( node.AdditionalAttributes.Count > 0 )
            {
                JsonObject joAdditionalAttr = new JsonObject();

                foreach( KeyValuePair<string, string> attr in node.AdditionalAttributes )
                    joAdditionalAttr.Add( attr.Key, attr.Value );

                
                using( JsonWriter writer = new JsonWriter() )
                {
                    joAdditionalAttr.Write( writer );
                    attrString = writer.ToString();
                }

            }
            joNode.Add( this.GetObfuscatedString( "node_additionalAttributes" ), attrString );

            #endregion

            joNode.Add( this.GetObfuscatedString( "node_isVirtualNode" ), node.IsVirtualNode );
            joNode.Add( this.GetObfuscatedString( "node_virtualNodesCount" ), node.VirtualNodesCount );
            joNode.Add( this.GetObfuscatedString( "node_virtualParentKey" ), node.VirtualParentKey );

            joNode.Add( this.GetObfuscatedString( "node_treeNodeDepth" ), node.NodeDepth );

            /*
             
            this.enableDragDrop = true;//
            this.enableSiblings = true;//
            this.enableChildren = true;//
             * 
            this.additionalAttributes = "";
             * 
            this.isVirtualNode = false;
            this.virtualNodesCount = 0;
            this.virtualParentKey = "";
            this.treeNodeDepth = -1;
             * 
            */

            if( node.NodeType == ASTreeViewNodeType.HyperLink )
            {
                ASTreeViewLinkNode linkNode = (ASTreeViewLinkNode)node;
                joNode.Add( this.GetObfuscatedString( "node_href" ), linkNode.NavigateUrl );
                joNode.Add( this.GetObfuscatedString( "node_target" ), linkNode.Target );
                joNode.Add( this.GetObfuscatedString( "node_tooltip" ), linkNode.Tooltip);

            }

            if( node.ChildNodes.Count == 0 )
            {
                //handle root, speical
                if( node.ParentNode == null ) //handle root, speical
                {
                    JsonArray jaRoot = new JsonArray();
                    jaRoot.Add( joNode );
                    jaRoot.Add( jaNode );


                    if( !this.currentTreeView.EnableRoot )
                        return jaRoot[1];

                    return jaRoot;
                }

                return joNode;
            }

            if( node.ParentNode != null ) //handle root, speical
                jaNode.Add( joNode );

            JsonArray jaChildren = new JsonArray();
            foreach( ASTreeViewNode child in node.ChildNodes )
                jaChildren.Add( this.GetNodeJson( child ));

            foreach( IJsonType childJson in jaChildren )
            {
                //get the first object from child to current
                if( childJson is JsonObject )
                    jaNode.Add( childJson );
                else if( childJson is JsonArray && ((JsonArray)childJson).Count > 0 )
                {
                    jaNode.Add( ( (JsonArray)childJson )[0] );
                    ( (JsonArray)childJson ).RemoveAt( 0 );
                    jaNode.Add( childJson );
                }
            }

            //handle root, speical
            if( node.ParentNode == null ) //handle root, speical
            {
                JsonArray jaRoot = new JsonArray();
                jaRoot.Add( joNode );
                jaRoot.Add( jaNode );


                if( !this.currentTreeView.EnableRoot )
                    return jaRoot[1];

                return jaRoot;
            }



            return jaNode;
        }
        
        #endregion

        #region public ObfuscateScript

        /// <summary>
        /// Obfuscates the script.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public string ObfuscateScript( string source )
        {
            return source;
        }

        #endregion

        #region public MinifyScript

        /// <summary>
        /// Minifies the script.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public string MinifyScript( string source )
        {
            return source;
        }
        #endregion

        #region public ValidateXmlDataSource

        virtual public void ValidateXmlDataSource( XmlDocument doc )
        {
            XMLValidationHelper validator = new XMLValidationHelper();
            Assembly assembly = Assembly.GetExecutingAssembly();
            XmlReader xsdReader = XmlReader.Create( assembly.GetManifestResourceStream( "Goldtect.ASTreeView.ASTreeView.astreeview-node.xsd" ) );

            bool validateResult = validator.ValidateXML( doc, xsdReader );
            if( !validateResult )
                throw new ASTreeViewInvalidDataSourceException( "Details:" + validator.LastXMLValidationError );
        }

        #endregion

        #region GetTreeViewXML

        /// <summary>
        /// Gets the tree view XML.
        /// </summary>
        /// <returns></returns>
        public XmlDocument GetTreeViewXML()
        {
            XmlDocument doc = XmlHelper.CreateDocument();

            XmlNode astreeview = doc.CreateElement( "astreeview" );
            doc.AppendChild( astreeview );

            if( this.currentTreeView != null && this.currentTreeView.RootNode.ChildNodes.Count > 0 )
            {
                XmlNode astreeviewNodes = XmlHelper.AddElement( astreeview, "astreeview-nodes", null );
                foreach( ASTreeViewNode node in this.currentTreeView.RootNode.ChildNodes )
                {
                    CreateXMLNodeRecursive( astreeviewNodes, node );
                }
            }

            return doc;
        }

        #endregion

        #region CreateXMLNodeRecursive

        /// <summary>
        /// Creates the XML node recursive.
        /// </summary>
        /// <param name="xmlParentNode">The XML parent node.</param>
        /// <param name="treeNode">The tree node.</param>
        protected void CreateXMLNodeRecursive( XmlNode xmlParentNode, ASTreeViewNode treeNode )
        {
            XmlNode xmlNode = XmlHelper.AddElement( xmlParentNode, "astreeview-node", null );

            XmlHelper.AddAttribute( xmlNode, "node-text", treeNode.NodeText );
            XmlHelper.AddAttribute( xmlNode, "node-value", treeNode.NodeValue );
            #region nodeType
            ASTreeViewNodeType nodeType = treeNode.NodeType;
            XmlHelper.AddAttribute( xmlNode, "node-type", ( (int)nodeType ).ToString() );
            #endregion
            XmlHelper.AddAttribute( xmlNode, "checked-state", ( (int)treeNode.CheckedState ).ToString() );
            XmlHelper.AddAttribute( xmlNode, "open-state", ( (int)treeNode.OpenState ).ToString() );
            XmlHelper.AddAttribute( xmlNode, "selected", treeNode.Selected.ToString().ToLower() );
            XmlHelper.AddAttribute( xmlNode, "enable-edit-context-menu", treeNode.EnableEditContextMenu.ToString().ToLower() );
            XmlHelper.AddAttribute( xmlNode, "enable-delete-context-menu", treeNode.EnableDeleteContextMenu.ToString().ToLower() );
            XmlHelper.AddAttribute( xmlNode, "enable-add-context-menu", treeNode.EnableAddContextMenu.ToString().ToLower() );
            XmlHelper.AddAttribute( xmlNode, "node-icon", treeNode.NodeIcon );
            XmlHelper.AddAttribute( xmlNode, "enable-drag-drop", treeNode.EnableDragDrop.ToString().ToLower() );
            XmlHelper.AddAttribute( xmlNode, "enable-siblings", treeNode.EnableSiblings.ToString().ToLower() );
            XmlHelper.AddAttribute( xmlNode, "enable-children", treeNode.EnableChildren.ToString().ToLower() );
            XmlHelper.AddAttribute( xmlNode, "enable-checkbox", treeNode.EnableCheckbox.ToString().ToLower() );
            XmlHelper.AddAttribute( xmlNode, "enable-selection", treeNode.EnableSelection.ToString().ToLower() );
            XmlHelper.AddAttribute( xmlNode, "enable-open-close", treeNode.EnableOpenClose.ToString().ToLower() );


            if( nodeType == ASTreeViewNodeType.HyperLink )
            {
                ASTreeViewLinkNode linkNode = (ASTreeViewLinkNode)treeNode;
                XmlHelper.AddAttribute( xmlNode, "navigate-url", linkNode.NavigateUrl );
                XmlHelper.AddAttribute( xmlNode, "target", linkNode.Target );
                XmlHelper.AddAttribute( xmlNode, "tooltip", linkNode.Tooltip );
            }

            if( treeNode.ChildNodes.Count > 0 )
            {
                XmlNode astreeviewNodes = XmlHelper.AddElement( xmlNode, "astreeview-nodes", null );
                foreach( ASTreeViewNode node in treeNode.ChildNodes )
                {
                    CreateXMLNodeRecursive( astreeviewNodes, node );
                }
            }
        }

        #endregion

        #region FindControlRecursive
        /// <summary>
        /// Finds a Control recursively. Note finds the first match and exists
        /// </summary>
        /// <param name="ContainerCtl"></param>
        /// <param name="IdToFind"></param>
        /// <returns></returns>
        public static Control FindControlRecursive( Control Root, string Id )
        {
            if( Root.ID == Id )
                return Root;

            foreach( Control Ctl in Root.Controls )
            {
                Control FoundCtl = FindControlRecursive( Ctl, Id );
                if( FoundCtl != null )
                    return FoundCtl;
            }

            return null;
        }

        #endregion

    }
}
