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
using System.Data;
using System.Drawing.Design;
using System.ComponentModel;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Services;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Painters;
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using Stimulsoft.Report.Helpers;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Dialogs
{
	/// <summary>
	/// Displays datasource in a scrollable grid.
	/// </summary>
	[StiToolbox(true)]
	[StiServiceBitmap(typeof(StiGridControl), "Stimulsoft.Report.Dialogs.Bmp.StiGridControl.gif")]
    [StiGdiPainter(typeof(Stimulsoft.Report.Painters.StiGridControlGdiPainter))]
    [StiWpfPainter("Stimulsoft.Report.Painters.StiGridControlWpfPainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
    [StiGuiMode(StiGuiMode.Gdi)]
	public class StiGridControl : 
		StiReportControl, 
		IStiDataSource, 
		IStiEnumerator
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiGridControl
            jObject.AddPropertyColor("AlternatingBackColor", AlternatingBackColor, SystemColors.Window);
            jObject.AddPropertyColor("BackgroundColor", BackgroundColor, SystemColors.AppWorkspace);
            jObject.AddPropertyColor("GridLineColor", GridLineColor, SystemColors.Control);
            jObject.AddPropertyColor("HeaderBackColor", HeaderBackColor, SystemColors.Control);
            jObject.AddPropertyColor("HeaderForeColor", HeaderForeColor, SystemColors.ControlText);
            jObject.AddPropertyColor("SelectionBackColor", SelectionBackColor, SystemColors.ActiveCaption);
            jObject.AddPropertyColor("SelectionForeColor", SelectionForeColor, SystemColors.ActiveCaptionText);
            jObject.AddPropertyStringNullOrEmpty("Filter", Filter);
            jObject.AddPropertyBool("ColumnHeadersVisible", ColumnHeadersVisible, true);
            jObject.AddPropertyBool("RowHeadersVisible", RowHeadersVisible, true);
#if !NETCOREAPP && !BLAZOR
			jObject.AddPropertyEnum("GridLineStyle", GridLineStyle, DataGridLineStyle.Solid);
#endif
			jObject.AddPropertyInt("PreferredColumnWidth", PreferredColumnWidth, 75);
            jObject.AddPropertyInt("PreferredRowHeight", PreferredRowHeight, 16);
            jObject.AddPropertyInt("RowHeaderWidth", RowHeaderWidth, 35);
            jObject.AddPropertyStringNullOrEmpty("DataSourceName", DataSourceName);
            jObject.AddPropertyJObject("PositionChangedEvent", PositionChangedEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("Columns", Columns.SaveToJsonObject(mode));

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "AlternatingBackColor":
                        this.alternatingBackColor = property.DeserializeColor();
                        break;

                    case "BackgroundColor":
                        this.backgroundColor = property.DeserializeColor();
						break;

                    case "GridLineColor":
                        this.gridLineColor = property.DeserializeColor();
						break;

                    case "HeaderBackColor":
                        this.headerBackColor = property.DeserializeColor();
						break;

                    case "HeaderForeColor":
                        this.headerForeColor = property.DeserializeColor();
						break;

                    case "SelectionBackColor":
                        this.selectionBackColor = property.DeserializeColor();
						break;

                    case "SelectionForeColor":
                        this.selectionForeColor = property.DeserializeColor();
						break;

                    case "Filter":
                        this.filter = property.DeserializeString();
                        break;

                    case "ColumnHeadersVisible":
                        this.columnHeadersVisible = property.DeserializeBool();
                        break;

                    case "RowHeadersVisible":
                        this.rowHeadersVisible = property.DeserializeBool();
                        break;

#if !NETCOREAPP && !BLAZOR
                    case "GridLineStyle":
                        this.gridLineStyle = property.DeserializeEnum<DataGridLineStyle>();
                        break;
#endif

					case "PreferredColumnWidth":
                        this.preferredColumnWidth = property.DeserializeInt();
                        break;

                    case "PreferredRowHeight":
                        this.preferredRowHeight = property.DeserializeInt();
                        break;

                    case "RowHeaderWidth":
                        this.rowHeaderWidth = property.DeserializeInt();
                        break;

                    case "DataSourceName":
                        this.dataSourceName = property.DeserializeString();
                        break;

                    case "PositionChangedEvent":
                        {
                            var _event = new StiPositionChangedEvent();
                            _event.LoadFromJsonObject((JObject)property.Value);
                            this.PositionChangedEvent = _event;
                        }
                        break;

                    case "Columns":
                        this.columns.LoadFromJsonObject((JObject)property.Value);
                        break;
                }
            }
        }
		#endregion

		#region IStiGetFonts
		public override List<StiFont> GetFonts()
		{
			var result = base.GetFonts();
			result.Add(new StiFont(HeaderFont));
			return result.Distinct().ToList();
		}
		#endregion

		#region StiComponent override
		public override StiComponentId ComponentId
        {
            get
            {
                return StiComponentId.StiGridControl;
            }
        }

		/// <summary>
		/// Gets value to sort a position in the toolbox.
		/// </summary>
		public override int ToolboxPosition
		{
			get
			{
				return (int)StiReportControlToolboxPosition.GridControl;
			}
		}

        public override StiToolboxCategory ToolboxCategory
        {
            get
            {
                return StiToolboxCategory.Controls;
            }
        }


		/// <summary>
		/// Gets a localized component name.
		/// </summary>
		public override string LocalizedName
		{
			get 
			{
				return StiLocalization.Get("Dialogs", "StiGridControl");
			}
		}
		#endregion

		#region Controls Property
		/// <summary>
		/// Gets or sets the foreground color (typically the color of the text) property.
		/// </summary>
		[StiSerializable]
        [TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiColorConverter))]
		[Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[StiCategory("Colors")]
		[Description("Gets or sets the foreground color (typically the color of the text) property.")]
        [StiGuiMode(StiGuiMode.Gdi)]
        [StiOrder(StiPropertyOrder.DialogForeColor)]
        [StiPropertyLevel(StiLevel.Standard)]
		public override Color ForeColor
		{
			get 
			{
				return base.ForeColor;
			}
			set 
			{
				base.ForeColor = value;
			}
		}

		
		/// <summary>
		/// Gets or sets the background color of the grid.
		/// </summary>
		[StiSerializable]
		[StiCategory("Colors")]
		[TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiColorConverter))]
		[Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[Description("Gets or sets the background color of the grid.")]
        [StiGuiMode(StiGuiMode.Gdi)]
        [StiOrder(StiPropertyOrder.DialogBackColor)]
        [StiPropertyLevel(StiLevel.Standard)]
		public override Color BackColor
		{
			get
			{
				return base.BackColor;
			}
			set
			{
				base.BackColor = value;
			}
		}


		/// <summary>
		/// Gets or sets position in datagrid.
		/// </summary>
		[Browsable(false)]
		public virtual int Position
		{
			get
			{
#if !NETCOREAPP && !BLAZOR
				if (Control == null)return -1;
				var form = Control.FindForm();
				if (form == null)return -1;
				var manager = form.BindingContext[Control.DataSource, Control.DataMember] as CurrencyManager;
				if (manager == null)return -1;
				return manager.Position;
#else
				return -1;
#endif
			}
			set
			{
#if !NETCOREAPP && !BLAZOR
                if (Control == null)return;
				var form = Control.FindForm();
				if (form == null)return;
				var manager = form.BindingContext[Control.DataSource, Control.DataMember] as CurrencyManager;
				if (manager == null)return;
				manager.Position = value;
#else
				return;
#endif
			}
		}


		/// <summary>
		/// Gets or sets rows count.
		/// </summary>
		[Browsable(false)]
		public virtual int Count
		{
			get
			{
#if !NETCOREAPP && !BLAZOR
				if (Control == null)return -1;
				var form = Control.FindForm();
				if (form == null)return -1;
				var manager = form.BindingContext[Control.DataSource, Control.DataMember] as CurrencyManager;
				if (manager == null)return -1;
				return manager.Count;
#else
				return -1;
#endif
			}
		}


		/// <summary>
		/// Gets or sets current datarow.
		/// </summary>
		[Browsable(false)]
		public virtual DataRow Current
		{
			get
			{
#if !NETCOREAPP && !BLAZOR
				if (Control == null)return null;
				var form = Control.FindForm();
				if (form == null)return null;
				var manager = form.BindingContext[Control.DataSource, Control.DataMember] as CurrencyManager;
				if (manager == null)return null;
				var rowView = manager.Current as DataRowView;
				return rowView.Row;
#else
				return null;
#endif
			}
		}


		private Color alternatingBackColor = SystemColors.Window;
		/// <summary>
		/// Gets or sets the background color of alternating rows for a ledger appearance.
		/// </summary>
		[StiSerializable]
		[StiCategory("Colors")]
		[TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiColorConverter))]
		[Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[Description("Gets or sets the background color of alternating rows for a ledger appearance.")]
        [StiGuiMode(StiGuiMode.Gdi)]
        [StiOrder(StiPropertyOrder.DialogAlternatingBackColor)]
        [StiPropertyLevel(StiLevel.Professional)]
		public virtual Color AlternatingBackColor
		{
			get
			{
				return alternatingBackColor;
			}
			set
			{
				alternatingBackColor = value;
				UpdateReportControl("AlternatingBackColor");
			}
		}


		private Color backgroundColor = SystemColors.AppWorkspace;
		/// <summary>
		/// Gets or sets the color of the non-row area of the grid.
		/// </summary>
		[StiSerializable]
		[StiCategory("Colors")]
		[TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiColorConverter))]
		[Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[Description("Gets or sets the color of the non-row area of the grid.")]
        [StiGuiMode(StiGuiMode.Gdi)]
        [StiOrder(StiPropertyOrder.DialogBackgroundColor)]
        [StiPropertyLevel(StiLevel.Professional)]
		public virtual Color BackgroundColor
		{
			get
			{
				return backgroundColor;
			}
			set
			{
				backgroundColor = value;
				UpdateReportControl("BackgroundColor");
			}
		}


		private Color gridLineColor = SystemColors.Control;
		/// <summary>
		/// Gets or sets the color of the grid lines.
		/// </summary>
		[StiSerializable]
		[StiCategory("Colors")]
		[TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiColorConverter))]
		[Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[Description("Gets or sets the color of the grid lines.")]
        [StiGuiMode(StiGuiMode.Gdi)]
        [StiOrder(StiPropertyOrder.DialogGridLineColor)]
        [StiPropertyLevel(StiLevel.Professional)]
		public virtual Color GridLineColor
		{
			get
			{
				return gridLineColor;
			}
			set
			{
				gridLineColor = value;
				UpdateReportControl("GridLineColor");
			}
		}


		private Color headerBackColor = SystemColors.Control;
		/// <summary>
		/// Gets or sets the background color of all row and column headers.
		/// </summary>
		[StiSerializable]
		[StiCategory("Colors")]
		[TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiColorConverter))]
		[Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[Description("Gets or sets the background color of all row and column headers.")]
        [StiGuiMode(StiGuiMode.Gdi)]
        [StiOrder(StiPropertyOrder.DialogHeaderBackColor)]
        [StiPropertyLevel(StiLevel.Professional)]
		public virtual Color HeaderBackColor
		{
			get
			{
				return headerBackColor;
			}
			set
			{
				headerBackColor = value;
				UpdateReportControl("HeaderBackColor");
			}
		}


		private Color headerForeColor = SystemColors.ControlText;
		/// <summary>
		/// Gets or sets the foreground color of headers.
		/// </summary>
		[StiSerializable]
		[StiCategory("Colors")]
		[TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiColorConverter))]
		[Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[Description("Gets or sets the foreground color of headers.")]
        [StiGuiMode(StiGuiMode.Gdi)]
        [StiOrder(StiPropertyOrder.DialogHeaderForeColor)]
        [StiPropertyLevel(StiLevel.Professional)]
		public virtual Color HeaderForeColor
		{
			get
			{
				return headerForeColor;
			}
			set
			{
				headerForeColor = value;
				UpdateReportControl("HeaderForeColor");
			}
		}


		private Color selectionBackColor = SystemColors.ActiveCaption;
		/// <summary>
		/// Gets or sets the background color of selected rows.
		/// </summary>
		[StiSerializable]
		[StiCategory("Colors")]
		[TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiColorConverter))]
		[Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[Description("Gets or sets the background color of selected rows.")]
        [StiGuiMode(StiGuiMode.Gdi)]
        [StiOrder(StiPropertyOrder.DialogSelectionBackColor)]
        [StiPropertyLevel(StiLevel.Professional)]
		public virtual Color SelectionBackColor
		{
			get
			{
				return selectionBackColor;
			}
			set
			{
				selectionBackColor = value;
				UpdateReportControl("SelectionBackColor");
			}
		}


		private Color selectionForeColor = SystemColors.ActiveCaptionText;
		/// <summary>
		/// Gets or set the foreground color of selected rows.
		/// </summary>
		[StiSerializable]
		[StiCategory("Colors")]
		[TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiColorConverter))]
		[Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[Description("Gets or set the foreground color of selected rows.")]
        [StiGuiMode(StiGuiMode.Gdi)]
        [StiOrder(StiPropertyOrder.DialogSelectionForeColor)]
        [StiPropertyLevel(StiLevel.Professional)]
		public virtual Color SelectionForeColor
		{
			get
			{
				return selectionForeColor;
			}
			set
			{
				selectionForeColor = value;
				UpdateReportControl("SelectionForeColor");
			}
		}


		private bool ShouldSerializeAlternatingBackColor()
		{
			return alternatingBackColor != SystemColors.Window;
		}

		private bool ShouldSerializeBackgroundColor()
		{
			return backgroundColor != SystemColors.AppWorkspace;
		}

		private bool ShouldSerializeGridLineColor()
		{
			return gridLineColor != SystemColors.Control;
		}

		private bool ShouldSerializeHeaderBackColor()
		{
			return headerBackColor != SystemColors.Control;
		}

		private bool ShouldSerializeHeaderForeColor()
		{
			return headerForeColor != SystemColors.ControlText;
		}

		private bool ShouldSerializeSelectionBackColor()
		{
			return selectionBackColor != SystemColors.ActiveCaption;
		}

		private bool ShouldSerializeSelectionForeColor()
		{
			return selectionForeColor != SystemColors.ActiveCaptionText;
		}

		private bool ShouldSerializeBackColor()
		{
			return BackColor != SystemColors.Control;
		}

		private bool ShouldSerializeForeColor()
		{
			return ForeColor != SystemColors.ControlText;
		}


		private string filter = "";
		/// <summary>
		/// Gets or sets filter string.
		/// </summary>
		[StiSerializable]
		[DefaultValue("")]
		[StiCategory("Data")]
		[Description("Gets or sets filter string.")]
        [StiOrder(StiPropertyOrder.DialogFilter)]
        [StiPropertyLevel(StiLevel.Standard)]
		public virtual string Filter
		{
			get
			{
				return filter;
			}
			set
			{
				filter = value;
				UpdateReportControl("Filter");
			}
		}


		private Font headerFont = new Font("Microsoft Sans Serif", 8);
		/// <summary>
		/// Gets or sets the font used for column headers.
		/// </summary>
		[StiSerializable]
		[StiCategory("Appearance")]
		[Description("Gets or sets the font used for column headers.")]
        [StiGuiMode(StiGuiMode.Gdi)]
        [StiOrder(StiPropertyOrder.DialogHeaderFont)]
        [StiPropertyLevel(StiLevel.Professional)]
		public virtual Font HeaderFont
		{
			get
			{
				return headerFont;
			}
			set
			{
				headerFont = value;
				UpdateReportControl("HeaderFont");
			}
		}


		private bool columnHeadersVisible = true;
		/// <summary>
		/// Gets or sets a value indicating whether the column headers a table are visible.
		/// </summary>
		[StiSerializable]
		[DefaultValue(true)]
		[StiCategory("Display")]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		[Description("Gets or sets a value indicating whether the column headers a table are visible.")]
        [StiGuiMode(StiGuiMode.Gdi)]
        [StiOrder(StiPropertyOrder.DialogColumnHeadersVisible)]
        [StiPropertyLevel(StiLevel.Professional)]
		public virtual bool ColumnHeadersVisible
		{
			get
			{
				return columnHeadersVisible;
			}
			set
			{
				columnHeadersVisible = value;
				UpdateReportControl("ColumnHeadersVisible");
			}
		}


		private bool rowHeadersVisible = true;
		/// <summary>
		/// Gets or sets a value that specifies whether row headers are visible.
		/// </summary>
		[StiSerializable]
		[DefaultValue(true)]
		[StiCategory("Display")]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		[Description("Gets or sets a value that specifies whether row headers are visible.")]
        [StiGuiMode(StiGuiMode.Gdi)]
        [StiOrder(StiPropertyOrder.DialogRowHeadersVisible)]
        [StiPropertyLevel(StiLevel.Professional)]
		public virtual bool RowHeadersVisible
		{
			get
			{
				return rowHeadersVisible;
			}
			set
			{
				rowHeadersVisible = value;
				UpdateReportControl("RowHeadersVisible");
			}
		}

#if !NETCOREAPP && !BLAZOR
		private DataGridLineStyle gridLineStyle = DataGridLineStyle.Solid;
		/// <summary>
		/// Gets or sets the line style of the grid.
		/// </summary>
		[StiSerializable]
		[DefaultValue(DataGridLineStyle.Solid)]
		[StiCategory("Appearance")]
		[TypeConverter(typeof(Stimulsoft.Base.Localization.StiEnumConverter))]
		[Editor(StiEditors.Enum, typeof(UITypeEditor))]
		[Description("Gets or sets the line style of the grid.")]
        [StiGuiMode(StiGuiMode.Gdi)]
        [StiOrder(StiPropertyOrder.DialogGridLineStyle)]
        [StiPropertyLevel(StiLevel.Professional)]
		public virtual DataGridLineStyle GridLineStyle
		{
			get
			{
				return gridLineStyle;
			}
			set
			{
				gridLineStyle = value;
				UpdateReportControl("GridLineStyle");
			}
		}
#endif

		private int preferredColumnWidth = 75;
		/// <summary>
		/// Gets or sets the default width of the grid columns in pixels.
		/// </summary>
		[StiSerializable]
		[DefaultValue(75)]
		[StiCategory("Behavior")]
		[Description("Gets or sets the default width of the grid columns in pixels.")]
        [StiGuiMode(StiGuiMode.Gdi)]
        [StiOrder(StiPropertyOrder.DialogPreferredColumnWidth)]
        [StiPropertyLevel(StiLevel.Professional)]
		public virtual int PreferredColumnWidth
		{
			get
			{
				return preferredColumnWidth;
			}
			set
			{
				preferredColumnWidth = value;
				UpdateReportControl("PreferredColumnWidth");
			}
		}


		private int preferredRowHeight = 16;
		/// <summary>
		/// Gets or sets the preferred row height.
		/// </summary>
		[StiSerializable]
		[DefaultValue(16)]
		[StiCategory("Behavior")]
		[Description("Gets or sets the preferred row height.")]
        [StiGuiMode(StiGuiMode.Gdi)]
        [StiOrder(StiPropertyOrder.DialogPreferredRowHeight)]
        [StiPropertyLevel(StiLevel.Professional)]
		public virtual int PreferredRowHeight
		{
			get
			{
				return preferredRowHeight;
			}
			set
			{
				preferredRowHeight = value;
				UpdateReportControl("PreferredRowHeight");
			}
		}


		private int rowHeaderWidth = 35;
		/// <summary>
		/// Gets or sets the width of row headers.
		/// </summary>
		[StiSerializable]
		[DefaultValue(35)]
		[StiCategory("Behavior")]
		[Description("Gets or sets the width of row headers.")]
        [StiGuiMode(StiGuiMode.Gdi)]
        [StiOrder(StiPropertyOrder.DialogRowHeaderWidth)]
        [StiPropertyLevel(StiLevel.Professional)]
		public virtual int RowHeaderWidth
		{
			get
			{
				return rowHeaderWidth;
			}
			set
			{
				rowHeaderWidth = value;
				UpdateReportControl("RowHeaderWidth");
			}
		}


		private StiGridColumnsCollection columns;
		/// <summary>
		/// Gets or sets the column collection.
		/// </summary>
		[StiSerializable(StiSerializationVisibility.List)]
		[StiCategory("Data")]
		[Editor("Stimulsoft.Report.Dialogs.Design.StiGridColumnsEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[Description("Gets or sets the column collection.")]
        [StiOrder(StiPropertyOrder.DialogColumns)]
        [StiPropertyLevel(StiLevel.Basic)]
		public virtual StiGridColumnsCollection Columns
		{
			get
			{
				return columns;
			}
			set
			{
				columns = value;
			}
		}

#if !NETCOREAPP && !BLAZOR
        /// <summary>
        /// Gets ot sets Windows Forms Control for this Report Control.
        /// </summary>
        [Browsable(false)]
        public DataGridView Control { get; set; } = null;
#endif
        #endregion

        #region IStiEnumerator
        /// <summary>
        /// Moves to the next row.
        /// </summary>
        public virtual void MoveNext()
		{
			this.Next();
		}


		/// <summary>
		/// Moves to first row.
		/// </summary>
		public virtual void First()
		{
			if (this.DataSource != null)this.DataSource.First();
		}


		/// <summary>
		/// Moves to prior row.
		/// </summary>
		public virtual void Prior()
		{
			if (this.DataSource != null)this.DataSource.Prior();
		}


		/// <summary>
		/// Moves to next row.
		/// </summary>
		public virtual void Next()
		{
			if (this.DataSource != null)this.DataSource.Next();
		}


		/// <summary>
		/// Moves to last row.
		/// </summary>
		public virtual void Last()
		{
			if (this.DataSource != null)this.DataSource.Last();
		}


		internal bool isEofValue = false;
		/// <summary>
		/// Gets value indicates that position points to end data.
		/// </summary>
		[Browsable(false)]
		public virtual bool IsEof
		{
			get
			{
				if (DataSource != null)return DataSource.IsEof;
				return true;
			}
			set
			{
				if (DataSource != null)DataSource.IsEof = value;
				else isEofValue = value;
			}			
		}


		internal bool isBofValue = false;
		/// <summary>
		/// Gets value indicates that position points to begin data.
		/// </summary>
		[Browsable(false)]
		public virtual bool IsBof
		{
			get
			{
				if (DataSource != null)return DataSource.IsBof;
				return false;
			}
			set
			{
				if (DataSource != null)DataSource.IsBof = value;
				else isBofValue = value;
			}
		}


		/// <summary>
		/// Gets value indicates that no data.
		/// </summary>
		[Browsable(false)]
		public virtual bool IsEmpty
		{
			get
			{
				if (DataSource != null)return DataSource.IsEmpty;
				return false;
			}
		}
		
		#endregion

		#region IStiDataSource
		/// <summary>
		/// Gets data source.
		/// </summary>
		[Editor("Stimulsoft.Report.Components.Design.StiDataSourceEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[TypeConverter(typeof(Stimulsoft.Report.Components.Design.StiDataSourceConverter))]
		[StiCategory("Data")]
		[Description("Gets data source.")]
        [StiOrder(StiPropertyOrder.DialogDataSource)]
		public virtual StiDataSource DataSource
		{
			get
			{
				if (Page == null || 
					Report == null || 
					Report.DataSources == null || 
					DataSourceName == null ||
					DataSourceName.Length == 0)return null;
				return Report.DataSources[DataSourceName];
			}
		}


		private string dataSourceName = "";
        /// <summary>
        /// Gets or sets name of data source.
        /// </summary>
		[Browsable(false)]
		[StiSerializable]
		public string DataSourceName
		{
			get
			{
				return dataSourceName;
			}
			set
			{
				if (dataSourceName != value)
				{
					dataSourceName = value;
					StiOptions.Engine.GlobalEvents.InvokeDataSourceAssigned(this, EventArgs.Empty);
				}
			}
		}

        [Browsable(false)]
		public bool IsDataSourceEmpty
		{
			get
			{
				return string.IsNullOrEmpty(DataSourceName) || DataSource == null;
			}
		}
		#endregion

		#region Events
		#region OnPositionChanged
		private static readonly object EventPositionChanged = new object();

		public event EventHandler PositionChanged
		{
			add
			{
				base.Events.AddHandler(EventPositionChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(EventPositionChanged, value);
			}
		}

		public void InvokePositionChanged(EventArgs e)
		{
			var handler = base.Events[EventPositionChanged] as EventHandler;
			if (handler != null)handler(this, e);


			StiBlocklyHelper.InvokeBlockly(this.Report, this, PositionChangedEvent);
		}

		/// <summary>
		/// Gets or sets a script of the event PositionChanged.
		/// </summary>
		[StiSerializable]
		[StiCategory("ControlsEvents")]
		[Browsable(false)]
		[Description("Gets or sets a script of the event PositionChanged.")]
		public virtual StiPositionChangedEvent PositionChangedEvent
		{
			get
			{				
				return new StiPositionChangedEvent(this);
			}
			set
			{
				if (value != null)value.Set(this, value.Script);
			}
		}
		#endregion
		#endregion

        #region Report Control Off
        [Browsable(false)]
        public override RightToLeft RightToLeft
        {
            get
            {
                return base.RightToLeft;
            }
            set
            {
                base.RightToLeft = value;
            }
        }
        #endregion

        #region Report Control Override
        /// <summary>
		/// Gets default event for this report control.
		/// </summary>
		/// <returns>Default event.</returns>
		public override StiEvent GetDefaultEvent()
		{
			return this.MouseDownEvent;
		}
		#endregion

        #region Methods.override
        public override StiComponent CreateNew()
        {
            return new StiGridControl();
        }
        #endregion

		#region this
		/// <summary>
		/// Gets or sets the default client area of a component.
		/// </summary>
		[Browsable(false)]
		public override RectangleD DefaultClientRectangle
		{
			get
			{
				return new RectangleD(0, 0, 192, 192);
			}
		}

		/// <summary>
		/// Creates a new StiGridControl.
		/// </summary>
		public StiGridControl() : this(RectangleD.Empty)
		{
		}

		/// <summary>
		/// Creates a new StiGridControl.
		/// </summary>
		/// <param name="rect">The rectangle describes sizes and position of the control.</param>
		public StiGridControl(RectangleD rect) : base(rect)
		{
			columns = new StiGridColumnsCollection(this);
			PlaceOnToolbox = true;
		}
		#endregion
	}
}