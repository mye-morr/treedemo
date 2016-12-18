using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;

namespace Goldtect
{
	/// <summary>
	/// Theme class of ASTreeView. It applies different themes for ASTreeView.
	/// </summary>
	[Serializable]
	public class ASTreeViewTheme
	{
		#region BasePath (string)

		protected string basePath = "~/Scripts/astreeview/themes/default/";

		/// <summary>
		/// Gets or sets the base path.
		/// </summary>
		/// <value>The base path.</value>
		public string BasePath
		{
			get
			{
				return this.basePath;
			}
			set
			{
				this.basePath = value;
			}
		}

		#endregion

		#region ImagePath (string)

		protected string imagePath = "images/";

		/// <summary>
		/// Gets or sets the image path.
		/// </summary>
		/// <value>The image path.</value>
		public string ImagePath
		{
			get
			{
				return this.basePath + this.imagePath;
			}
			set
			{
				this.imagePath = value;
			}
		}

		#endregion

		#region CssFile (string)

		protected string cssFile = "default.css";

		/// <summary>
		/// Gets or sets the CSS file.
		/// </summary>
		/// <value>The CSS file.</value>
		public string CssFile
		{
			get
			{
				return this.basePath + this.cssFile;
			}
			set
			{
				this.cssFile = value;
			}
		}

		#endregion

		#region Appearance

		#region CssClass (string)

		protected string cssClass = "astreeview-tree";

		/// <summary>
		/// Gets or sets the CSS class.
		/// </summary>
		/// <value>The CSS class.</value>
		public string CssClass
		{
			get
			{
				return this.cssClass;
			}
			set
			{
				this.cssClass = value;
			}
		}

		#endregion

		#endregion

		#region Folder & Item Icon

		#region DefaultFolderIcon (string)

		protected string defaultFolderIcon = "astreeview-folder.gif";

		/// <summary>
		/// Gets or sets the default folder icon.
		/// </summary>
		/// <value>The default folder icon.</value>
		public string DefaultFolderIcon
		{
			get
			{
				return this.ImagePath + this.defaultFolderIcon;
			}
			set
			{
				this.defaultFolderIcon = value;
			}
		}

		#endregion

		#region DefaultFolderOpenIcon (string)

		protected string defaultFolderOpenIcon = "astreeview-folder-open.gif";

		/// <summary>
		/// Gets or sets the default folder open icon.
		/// </summary>
		/// <value>The default folder open icon.</value>
		public string DefaultFolderOpenIcon
		{
			get
			{
				return this.ImagePath + this.defaultFolderOpenIcon;
			}
			set
			{
				this.defaultFolderOpenIcon = value;
			}
		}

		#endregion

		#region DefaultNodeIcon (string)

		protected string defaultNodeIcon = "astreeview-node.gif";

		/// <summary>
		/// Gets or sets the default node icon.
		/// </summary>
		/// <value>The default node icon.</value>
		public string DefaultNodeIcon
		{
			get
			{
				return this.ImagePath + this.defaultNodeIcon;
			}
			set
			{
				this.defaultNodeIcon = value;
			}
		}

		#endregion

		#region CssClassIcon (string)

		protected string cssClassIcon = "astreeview-icon";

		/// <summary>
		/// Gets or sets the CSS class icon.
		/// </summary>
		/// <value>The CSS class icon.</value>
		public string CssClassIcon
		{
			get
			{
				return this.cssClassIcon;
			}
			set
			{
				this.cssClassIcon = value;
			}
		}

		#endregion

		#endregion

		#region Plus & Minus Icon

		#region ImgPlusIcon (string)

		protected string imgPlusIcon = "astreeview-plus.gif";

		/// <summary>
		/// Gets or sets the img plus icon.
		/// </summary>
		/// <value>The img plus icon.</value>
		public string ImgPlusIcon
		{
			get
			{
				return this.ImagePath + this.imgPlusIcon;
			}
			set
			{
				this.imgPlusIcon = value;
			}
		}

		#endregion

		#region ImgMinusIcon (string)

		protected string imgMinusIcon = "astreeview-minus.gif";

		/// <summary>
		/// Gets or sets the img minus icon.
		/// </summary>
		/// <value>The img minus icon.</value>
		public string ImgMinusIcon
		{
			get
			{
				return this.ImagePath + this.imgMinusIcon;
			}
			set
			{
				this.imgMinusIcon = value;
			}
		}

		#endregion

		#region CssClassPlusMinusIcon (string)

		protected string cssClassPlusMinusIcon = "astreeview-plus-minus";

		/// <summary>
		/// Gets or sets the CSS class plus minus icon.
		/// </summary>
		/// <value>The CSS class plus minus icon.</value>
		public string CssClassPlusMinusIcon
		{
			get
			{
				return this.cssClassPlusMinusIcon;
			}
			set
			{
				this.cssClassPlusMinusIcon = value;
			}
		}

		#endregion

		#endregion

		#region Checkbox

		#region ImgCheckboxChecked (string)

		private string imgCheckboxChecked = "astreeview-checkbox-checked.gif";

		/// <summary>
		/// Gets or sets the img checkbox checked.
		/// </summary>
		/// <value>The img checkbox checked.</value>
		public string ImgCheckboxChecked
		{
			get
			{
				return this.ImagePath + this.imgCheckboxChecked;
			}
			set
			{
				this.imgCheckboxChecked = value;
			}
		}

		#endregion

		#region ImgCheckboxHalfChecked (string)

		private string imgCheckboxHalfChecked = "astreeview-checkbox-half-checked.gif";

		/// <summary>
		/// Gets or sets the img checkbox half checked.
		/// </summary>
		/// <value>The img checkbox half checked.</value>
		public string ImgCheckboxHalfChecked
		{
			get
			{
				return this.ImagePath + this.imgCheckboxHalfChecked;
			}
			set
			{
				this.imgCheckboxHalfChecked = value;
			}
		}

		#endregion

		#region ImgCheckboxUnchecked (string)

		private string imgCheckboxUnchecked = "astreeview-checkbox-unchecked.gif";

		/// <summary>
		/// Gets or sets the img checkbox unchecked.
		/// </summary>
		/// <value>The img checkbox unchecked.</value>
		public string ImgCheckboxUnchecked
		{
			get
			{
				return this.ImagePath + this.imgCheckboxUnchecked;
			}
			set
			{
				this.imgCheckboxUnchecked = value;
			}
		}

		#endregion

		#region CssClassCheckbox (string)

		private string cssClassCheckbox = "astreeview-checkbox";

		/// <summary>
		/// Gets or sets the CSS class checkbox.
		/// </summary>
		/// <value>The CSS class checkbox.</value>
		public string CssClassCheckbox
		{
			get
			{
				return this.cssClassCheckbox;
			}
			set
			{
				this.cssClassCheckbox = value;
			}
		}

		#endregion

		#endregion

		#region Drag Drop Indicator

		#region ImgDragDropIndicator (string)

		private string imgDragDropIndicator = "astreeview-dragDrop-indicator1.gif";

		/// <summary>
		/// Gets or sets the img drag drop indicator.
		/// </summary>
		/// <value>The img drag drop indicator.</value>
		public string ImgDragDropIndicator
		{
			get
			{
				return this.ImagePath + this.imgDragDropIndicator;
			}
			set
			{
				this.imgDragDropIndicator = value;
			}
		}

		#endregion

		#region ImgDragDropIndicatorSub (string)

		private string imgDragDropIndicatorSub = "astreeview-dragDrop-indicator2.gif";

		/// <summary>
		/// Gets or sets the img drag drop indicator sub.
		/// </summary>
		/// <value>The img drag drop indicator sub.</value>
		public string ImgDragDropIndicatorSub
		{
			get
			{
				return this.ImagePath + this.imgDragDropIndicatorSub;
			}
			set
			{
				this.imgDragDropIndicatorSub = value;
			}
		}

		#endregion

		#endregion
	}
}
