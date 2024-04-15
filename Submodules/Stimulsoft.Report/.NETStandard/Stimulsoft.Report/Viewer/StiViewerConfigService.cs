#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
{	                         										}
{																	}
{	Copyright (C) 2003-2022 Stimulsoft     							}
{	ALL RIGHTS RESERVED												}
{																	}
{	The entire contents of this file is protected by U.S. and		}
{	International Copyright Laws. Unauthorized reproduction,		}
{	reverse-engineering, and distribution of all or any portion of	}
{	the code contained in this file is strictly prohibited and may	}
{	result in severe civil and criminal penalties and will be		}
{	prosecuted to the maximum extent possible under the law.		}
{																	}
{	RESTRICTIONS													}
{																	}
{	THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES			}
{	ARE CONFIDENTIAL AND PROPRIETARY								}
{	TRADE SECRETS OF Stimulsoft										}
{																	}
{	CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON		}
{	ADDITIONAL RESTRICTIONS.										}
{																	}
{*******************************************************************}
*/
#endregion Copyright (C) 2003-2022 Stimulsoft

using System;
using System.ComponentModel;
using Stimulsoft.Base.Services;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Design;
#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Viewer
{
	/// <summary>
	/// Class describes the configuration of the viewer window.
	/// </summary>
	public class StiViewerConfigService : StiService
	{
		#region StiService override
		/// <summary>
		/// Gets a service category.
		/// </summary>
		public override string ServiceCategory => StiLocalization.Get("Services", "categoryRender");

	    /// <summary>
		/// Gets a service type.
		/// </summary>
		public override Type ServiceType => typeof(StiViewerConfigService);
	    #endregion

		#region Properties
		/// <summary>
		/// Gets or sets value indicates active instrument Select or no.
		/// </summary>
		[StiServiceParam]
		[DefaultValue(true)]
		[StiCategory("Parameters")]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		[RefreshProperties(RefreshProperties.All)]
		public virtual bool ToolSelectActive
		{
			get
			{
				StiSettings.Load();
				return StiSettings.GetBool("Viewer", "ToolSelectActive", true);
			}
			set
			{
				StiSettings.Load();
				StiSettings.Set("Viewer", "ToolSelectActive", value);
				StiSettings.Save();
			}
		}

		/// <summary>
		/// Gets or sets value indicates active instrument Editor or no.
		/// </summary>
		[StiServiceParam]
		[DefaultValue(false)]
		[StiCategory("Parameters")]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		[RefreshProperties(RefreshProperties.All)]
		public virtual bool ToolEditorActive
		{
			get
			{
				StiSettings.Load();
				return StiSettings.GetBool("Viewer", "ToolEditorActive", false);
			}
			set
			{
				StiSettings.Load();
				StiSettings.Set("Viewer", "ToolEditorActive", value);
				StiSettings.Save();
			}
		}

		/// <summary>
		/// Gets or sets value indicates active instrument Hand or no.
		/// </summary>
		[StiServiceParam]
		[DefaultValue(false)]
		[StiCategory("Parameters")]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		[RefreshProperties(RefreshProperties.All)]
		public virtual bool ToolHandActive
		{
			get
			{
				StiSettings.Load();
				return StiSettings.GetBool("Viewer", "ToolHandActive", false);
			}
			set
			{
				StiSettings.Load();
				StiSettings.Set("Viewer", "ToolHandActive", value);
				StiSettings.Save();
			}
		}
		
		/// <summary>
		/// Gets or sets zoom of the drawing of the pages in preview window.
		/// </summary>
		[DefaultValue(1d)]
		[StiCategory("Parameters")]
		public virtual double Zoom
		{
			get
			{
				StiSettings.Load();
				return StiSettings.GetDouble("Viewer", "Zoom", 1d);
			}
			set
			{
				StiSettings.Load();
				StiSettings.Set("Viewer", "Zoom", value);
				StiSettings.Save();
			}
		}

	    /// <summary>
		/// Gets or sets value indicates is thumb panel is showed or no.
		/// </summary>
		[StiServiceParam]
		[DefaultValue(null)]
		[StiCategory("Parameters")]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		public virtual bool? ThumbsPanelEnabled { get; set; }

	    /// <summary>
		/// Gets or sets value indicates is thumb panel is showed or no.
		/// </summary>
		[Browsable(false)]
		[Obsolete("Please use 'ThumbsPanelEnabled' property instead 'ShowThumbsPanel' property.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual bool? ShowThumbsPanel
		{
			get
			{
				return ThumbsPanelEnabled;
			}
			set
			{
				ThumbsPanelEnabled = value;
			}
		}

		/// <summary>
		/// Gets or sets value indicates is 'Zoom' menu is showed or no.
		/// </summary>
		[StiServiceParam]
		[DefaultValue(null)]
		[StiCategory("Parameters")]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		public virtual bool? ZoomEnabled
		{
			get
			{
				return StiOptions.Viewer.Windows.ShowZoom;
			}
			set
			{
				StiOptions.Viewer.Windows.ShowZoom = value;
			}
		}

		/// <summary>
		/// Gets or sets value indicates is 'FullScreen' buttom is showed or no.
		/// </summary>
		[StiServiceParam]
		[DefaultValue(null)]
		[StiCategory("Parameters")]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		public virtual bool? FullScreenEnabled
		{
			get
			{
				return StiOptions.Viewer.Windows.ShowFullScreen;
			}
			set
			{
				StiOptions.Viewer.Windows.ShowFullScreen = value;
			}
		}
		

		/// <summary>
		/// Gets or sets value indicates is enclosed instrument Select or no.
		/// </summary>
		[Browsable(false)]
		[Obsolete("Report Viewer does not use more SelectTool.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual bool? ToolSelectEnabled
		{
			get
			{
				return StiOptions.Viewer.Windows.ShowSelectTool;
			}
			set
			{
				StiOptions.Viewer.Windows.ShowSelectTool = value;
			}
		}

		/// <summary>
		/// Gets or sets value indicates is enclosed instrument Hand or no.
		/// </summary>
		[Browsable(false)]
		[Obsolete("Report Viewer does not use more SelectTool.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual bool? ToolHandEnabled
		{
			get
			{
				return StiOptions.Viewer.Windows.ShowHandTool;
			}
			set
			{
				StiOptions.Viewer.Windows.ShowHandTool = value;
			}
		}

		/// <summary>
		/// Gets or sets value indicates is enclosed instrument Find or no.
		/// </summary>
		[StiServiceParam]
		[DefaultValue(null)]
		[StiCategory("Parameters")]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		public virtual bool? ToolFindEnabled
		{
			get
			{
				return StiOptions.Viewer.Windows.ShowFindTool;
			}
			set
			{
				StiOptions.Viewer.Windows.ShowFindTool = value;
			}
		}

		/// <summary>
		/// Gets or sets value indicates is enclosed instrument Editor or no.
		/// </summary>
		[StiServiceParam]
        [DefaultValue(null)]
		[StiCategory("Parameters")]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		public virtual bool? ToolEditorEnabled
		{
			get
			{
				return StiOptions.Viewer.Windows.ShowEditorTool;
			}
			set
			{
				StiOptions.Viewer.Windows.ShowEditorTool = value;
			}
		}

		/// <summary>
		/// Gets or sets value indicates is enclosed instrument Signature or no.
		/// </summary>
		[StiServiceParam]
		[DefaultValue(null)]
		[StiCategory("Parameters")]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		public virtual bool? ToolSignatureEnabled
		{
			get
			{
				return StiOptions.Viewer.Windows.ShowSignatureTool;
			}
			set
			{
				StiOptions.Viewer.Windows.ShowSignatureTool = value;
			}
		}

		/// <summary>
		/// Gets or sets value indicates is enabled button PageNew or no.
		/// </summary>
		[StiServiceParam]
        [DefaultValue(null)]
		[StiCategory("Parameters")]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		public virtual bool? PageNewEnabled
		{
			get
			{
			    return StiOptions.Viewer.Windows.ShowEditorTool == false ? false : StiOptions.Viewer.Windows.ShowPageNewButton;
			}
			set
			{
				StiOptions.Viewer.Windows.ShowPageNewButton = value;
			}
		}
		
		/// <summary>
		/// Gets or sets value indicates is enabled button PageDelete or no.
		/// </summary>
		[StiServiceParam]
        [DefaultValue(null)]
		[StiCategory("Parameters")]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		public virtual bool? PageDeleteEnabled
		{
			get
			{
			    return StiOptions.Viewer.Windows.ShowEditorTool == false ? false : StiOptions.Viewer.Windows.ShowPageDeleteButton;
			}
			set
			{
				StiOptions.Viewer.Windows.ShowPageDeleteButton = value;
			}
		}
		
		/// <summary>
		/// Gets or sets value indicates is enabled button PageDesign or no.
		/// </summary>
		[StiServiceParam]
        [DefaultValue(null)]
		[StiCategory("Parameters")]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		public virtual bool? PageDesignEnabled
		{
			get
			{
			    return StiOptions.Viewer.Windows.ShowEditorTool == false ? false : StiOptions.Viewer.Windows.ShowPageDesignButton;
			}
			set
			{
				StiOptions.Viewer.Windows.ShowPageDesignButton = value;
			}
		}

        /// <summary>
        /// Gets or sets value indicates is enabled button PageSize or no.
        /// </summary>
        [StiServiceParam]
        [DefaultValue(null)]
        [StiCategory("Parameters")]
        [TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		public virtual bool? PageSizeEnabled
        {
            get
            {
                return StiOptions.Viewer.Windows.ShowEditorTool == false ? false : StiOptions.Viewer.Windows.ShowPageSizeButton;
            }
            set
            {
                StiOptions.Viewer.Windows.ShowPageSizeButton = value;
            }
        }

		[StiServiceParam]
        [DefaultValue(null)]
		[StiCategory("Parameters")]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		public virtual bool? PageViewModeEnabled
		{
			get
			{
				return StiOptions.Viewer.Windows.ShowPageViewMode;
			}
			set
			{
				StiOptions.Viewer.Windows.ShowPageViewMode = value;
			}
		}

		[StiServiceParam]
        [DefaultValue(null)]
		[StiCategory("Parameters")]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		public virtual bool? PageViewModeSingleEnabled
		{
			get
			{
				return StiOptions.Viewer.Windows.ShowPageViewSingleMode;
			}
			set
			{
				StiOptions.Viewer.Windows.ShowPageViewSingleMode = value;
			}
		}

		[StiServiceParam]
        [DefaultValue(null)]
		[StiCategory("Parameters")]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		public virtual bool? PageViewModeContinuousEnabled
		{
			get
			{
				return StiOptions.Viewer.Windows.ShowPageViewContinuousMode;
			}
			set
			{
				StiOptions.Viewer.Windows.ShowPageViewContinuousMode = value;
			}
		}

		[StiServiceParam]
        [DefaultValue(null)]
		[StiCategory("Parameters")]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		public virtual bool? PageViewModeMultipleEnabled
		{
			get
			{
				return StiOptions.Viewer.Windows.ShowPageViewMultipleMode;
			}
			set
			{
				StiOptions.Viewer.Windows.ShowPageViewMultipleMode = value;
			}
		}

		/// <summary>
		/// Gets or sets value indicates is enabled possibility of the printing or no.
		/// </summary>
		[StiServiceParam]
        [DefaultValue(null)]
		[StiCategory("Parameters")]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		public virtual bool? PrintEnabled
		{
			get
			{
				return StiOptions.Viewer.Windows.ShowPrintButton;
			}
			set
			{
				StiOptions.Viewer.Windows.ShowPrintButton = value;
			}
		}

		/// <summary>
		/// Gets or sets value indicates is enabled possibility of the closing or no.
		/// </summary>
		[StiServiceParam]
        [DefaultValue(null)]
		[StiCategory("Parameters")]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		public virtual bool? CloseEnabled
		{
			get
			{
				return StiOptions.Viewer.Windows.ShowCloseButton;
			}
			set
			{
				StiOptions.Viewer.Windows.ShowCloseButton = value;
			}
		}

        /// <summary>
        /// Gets or sets value indicates is enabled possibility of the calling help or not.
        /// </summary>
        [StiServiceParam]
        [DefaultValue(null)]
        [StiCategory("Parameters")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        public virtual bool? HelpEnabled
        {
            get
            {
                return StiOptions.Viewer.Windows.ShowHelpButton;
            }
            set
            {
                StiOptions.Viewer.Windows.ShowHelpButton = value;
            }
        }

        /// <summary>
        /// Gets or sets value indicates is enabled possibility of the page control or no.
        /// </summary>
        [StiServiceParam]
        [DefaultValue(null)]
		[StiCategory("Parameters")]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		public virtual bool? PageControlEnabled
		{
			get
			{
				return StiOptions.Viewer.Windows.ShowPageControl;
			}
			set
			{
				StiOptions.Viewer.Windows.ShowPageControl = value;
			}
		}

		/// <summary>
		/// Gets or sets value indicates is enabled possibility of the first page button or no.
		/// </summary>
		[StiServiceParam]
        [DefaultValue(null)]
		[StiCategory("Parameters")]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		public virtual bool? FirstPageEnabled
		{
			get
			{
				return StiOptions.Viewer.Windows.ShowFirstPage;
			}
			set
			{
				StiOptions.Viewer.Windows.ShowFirstPage = value;
			}
		}

		/// <summary>
		/// Gets or sets value indicates is enabled possibility of the previous page button or no.
		/// </summary>
		[StiServiceParam]
        [DefaultValue(null)]
		[StiCategory("Parameters")]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		public virtual bool? PreviousPageEnabled
		{
			get
			{
				return StiOptions.Viewer.Windows.ShowPreviousPage;
			}
			set
			{
				StiOptions.Viewer.Windows.ShowPreviousPage = value;
			}
		}

		/// <summary>
		/// Gets or sets value indicates is enabled possibility of the next page button or no.
		/// </summary>
		[StiServiceParam]
        [DefaultValue(null)]
		[StiCategory("Parameters")]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		public virtual bool? NextPageEnabled
		{
			get
			{
				return StiOptions.Viewer.Windows.ShowNextPage;
			}
			set
			{
				StiOptions.Viewer.Windows.ShowNextPage = value;
			}
		}

	    /// <summary>
	    /// Gets or sets value indicates is enabled possibility of the last page button or no.
	    /// </summary>
	    [StiServiceParam]
	    [DefaultValue(null)]
	    [StiCategory("Parameters")]
	    [TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		public virtual bool? LastPageEnabled
	    {
	        get
	        {
	            return StiOptions.Viewer.Windows.ShowLastPage;
	        }
	        set
	        {
	            StiOptions.Viewer.Windows.ShowLastPage = value;
	        }
	    }

	    /// <summary>
		/// Gets or sets value indicates is enabled possibility of the saving or no.
		/// </summary>
		[StiServiceParam]
        [DefaultValue(null)]
		[StiCategory("Parameters")]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		public virtual bool? SaveEnabled
		{
			get
			{
				return StiOptions.Viewer.Windows.ShowSaveButton;
			}
			set
			{
				StiOptions.Viewer.Windows.ShowSaveButton = value;
			}
		}

		/// <summary>
		/// Gets or sets value indicates is enabled possibility of the opening or no.
		/// </summary>
		[StiServiceParam]
        [DefaultValue(null)]
		[StiCategory("Parameters")]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		public virtual bool? OpenEnabled
		{
			get
			{
				return StiOptions.Viewer.Windows.ShowOpenButton;
			}
			set
			{
				StiOptions.Viewer.Windows.ShowOpenButton = value;
			}
		}

		/// <summary>
		/// Gets or sets value indicates is enabled possibility of the exporting or no.
		/// </summary>
		[StiServiceParam]
        [DefaultValue(null)]
		[StiCategory("Parameters")]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		[Obsolete("Please use SaveEnabled property instead ExportEnabled property.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual bool? ExportEnabled
		{
			get
			{
				return SaveEnabled;
			}
			set
			{
				SaveEnabled = value;
			}
		}

		/// <summary>
		/// Gets or sets value indicates is enabled possibility of the sending email or no.
		/// </summary>
		[StiServiceParam]
        [DefaultValue(null)]
		[StiCategory("Parameters")]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		public virtual bool? SendEMailEnabled
		{
			get
			{
				return StiOptions.Viewer.Windows.ShowSendEMailButton;
			}
			set
			{
				StiOptions.Viewer.Windows.ShowSendEMailButton = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the Save Document File button is visible in the viewer window.
		/// </summary>
		[StiServiceParam]
        [DefaultValue(null)]
		[StiCategory("Parameters")]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		public virtual bool? SaveDocumentFileEnabled
		{
			get
			{
				return StiOptions.Viewer.Windows.ShowSaveDocumentFileButton;
			}
			set
			{
				StiOptions.Viewer.Windows.ShowSaveDocumentFileButton = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the SendEMail Document File button is visible in the viewer window.
		/// </summary>
		[StiServiceParam]
        [DefaultValue(null)]
		[StiCategory("Parameters")]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		public virtual bool? SendEMailDocumentFileEnabled
		{
			get
			{
				return StiOptions.Viewer.Windows.ShowSendEMailDocumentFileButton;
			}
			set
			{
				StiOptions.Viewer.Windows.ShowSendEMailDocumentFileButton = value;
			}
		}

        /// <summary>
        /// Gets or sets a value indicating whether Toolbar is visible in the viewer window.
        /// </summary>
        [StiServiceParam]
        [DefaultValue(null)]
        [StiCategory("Parameters")]
        [TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		public virtual bool? ToolbarEnabled
        {
            get
            {
                return StiOptions.Viewer.Windows.ShowToolbar;
            }
            set
            {
                StiOptions.Viewer.Windows.ShowToolbar = value;
            }
        }
	    #endregion
    }
}
