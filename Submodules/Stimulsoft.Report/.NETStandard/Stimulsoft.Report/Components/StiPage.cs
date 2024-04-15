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
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Report.SaveLoad;
using Stimulsoft.Report.Units;
using Stimulsoft.Report.Components.Design;
using Stimulsoft.Report.Events;
using Stimulsoft.Base;
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Services;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Painters;
using Stimulsoft.Report.Engine;
using Stimulsoft.Report.BarCodes;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Report.Dialogs;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Helpers;
using System.Threading;
using System.Globalization;
using Stimulsoft.Base.Json;
using System.Collections.Generic;
using Stimulsoft.Report.App;
using Stimulsoft.Report.Design.Forms;
using System.Drawing;
using System.Drawing.Printing;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
using Graphics = Stimulsoft.Drawing.Graphics;
#endif

namespace Stimulsoft.Report.Components
{
	/// <summary>
	/// Report page class.
	/// </summary>
	[StiServiceBitmap(typeof(StiPage), "Stimulsoft.Report.Images.Components.StiPage.png")]
	[StiDesigner("Stimulsoft.Report.Components.Design.StiPageDesigner, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
	[StiWpfDesigner("Stimulsoft.Report.WpfDesign.StiWpfPageDesigner, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
	[StiGdiPainter(typeof(StiPageGdiPainter))]
	[StiWpfPainter("Stimulsoft.Report.Painters.StiPageWpfPainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
	[StiV1Builder(typeof(StiPageV1Builder))]
	[StiV2Builder(typeof(StiPageV2Builder))]
	[StiContextTool(typeof(IStiComponentDesigner))]
	public class StiPage :
		StiPanel,
		IStiReportPage,
		IStiNonSerialized,
		IStiResetPageNumber,
		IStiGlobalizationProvider
	{
		#region IStiJsonReportObject.override
		public override JObject SaveToJsonObject(StiJsonSaveMode mode)
		{
			var jObject = base.SaveToJsonObject(mode);

			// Old
			jObject.RemoveProperty("CanShrink");
			jObject.RemoveProperty("CanGrow");
			jObject.RemoveProperty("PrintOn");
			jObject.RemoveProperty("CanBreak");
			jObject.RemoveProperty("DockStyle");
			jObject.RemoveProperty("MinSize");
			jObject.RemoveProperty("MaxSize");
			jObject.RemoveProperty("ClientRectangle");
			jObject.RemoveProperty("DisplayRectangle");
			jObject.RemoveProperty("UseParentStyles");
			jObject.RemoveProperty("GrowToHeight");
			jObject.RemoveProperty("ShiftMode");
			jObject.RemoveProperty("Printable");
			jObject.RemoveProperty("Page");
			jObject.RemoveProperty("Parent");
			jObject.RemoveProperty("Restrictions");
			jObject.RemoveProperty("Locked");
			jObject.RemoveProperty("Linked");

			// StiPage
			jObject.AddPropertyBool("ResetPageNumber", ResetPageNumber);
			jObject.AddPropertyJObject("BeginRenderEvent", BeginRenderEvent.SaveToJsonObject(mode));
			jObject.AddPropertyJObject("RenderingEvent", RenderingEvent.SaveToJsonObject(mode));
			jObject.AddPropertyJObject("EndRenderEvent", EndRenderEvent.SaveToJsonObject(mode));
			jObject.AddPropertyJObject("ColumnBeginRenderEvent", ColumnBeginRenderEvent.SaveToJsonObject(mode));
			jObject.AddPropertyJObject("ColumnEndRenderEvent", ColumnEndRenderEvent.SaveToJsonObject(mode));
			jObject.AddPropertyJObject("GetExcelSheetEvent", GetExcelSheetEvent.SaveToJsonObject(mode));
			jObject.AddPropertyStringNullOrEmpty("ExcelSheetValue", ExcelSheetValue);
			
			if (mode == StiJsonSaveMode.Report) 
				jObject.AddPropertyJObject("ExcelSheet", ExcelSheet.SaveToJsonObject(mode));

			jObject.AddPropertyBool("PrintOnPreviousPage", PrintOnPreviousPage);
			jObject.AddPropertyBool("PrintHeadersFootersFromPreviousPage", PrintHeadersFootersFromPreviousPage);
			jObject.AddPropertyEnum("PaperSize", PaperSize, PaperKind.Custom);
			jObject.AddPropertyStringNullOrEmpty("PaperSourceOfFirstPage", PaperSourceOfFirstPage);
			jObject.AddPropertyStringNullOrEmpty("PaperSourceOfOtherPages", PaperSourceOfOtherPages);
			jObject.AddPropertyInt("NumberOfCopies", NumberOfCopies, 1);
			jObject.AddPropertyBool("UnlimitedBreakable", UnlimitedBreakable, true);
			jObject.AddPropertyBool("LargeHeight", LargeHeight);
			jObject.AddPropertyInt("LargeHeightFactor", LargeHeightFactor, 4);
			jObject.AddPropertyInt("StopBeforePrint", StopBeforePrint);
			jObject.AddPropertyBool("StretchToPrintArea", StretchToPrintArea);
			jObject.AddPropertyBool("TitleBeforeHeader", TitleBeforeHeader);
			jObject.AddPropertyBool("UnlimitedHeight", UnlimitedHeight);
			jObject.AddPropertyBool("UnlimitedWidth", UnlimitedWidth, true);
			jObject.AddPropertyEnum("Orientation", Orientation, StiPageOrientation.Portrait);
			jObject.AddPropertyDouble("PageWidth", PageWidth, 827d);
			jObject.AddPropertyDouble("PageHeight", PageHeight, 1169d);
			jObject.AddPropertyInt("SegmentPerWidth", SegmentPerWidth, 1);
			jObject.AddPropertyInt("SegmentPerHeight", SegmentPerHeight, 1);
			jObject.AddPropertyJObject("Watermark", Watermark.SaveToJsonObject(mode));
			jObject.AddPropertyJObject("Margins", Margins.SaveToJsonObject(39, 39, 39, 39));
			jObject.AddPropertyBool("MirrorMargins", MirrorMargins, false);

			if (ReportUnit != null)
				jObject.AddPropertyJObject("ReportUnit", StiUnit.SaveToJsonObject(ReportUnit));

			if (Icon != null)
				jObject.AddPropertyByteArray("Icon", Icon);

			return jObject;
		}

		internal void LoadFromJsonInternal(string text)
		{
			var currentCulture = Thread.CurrentThread.CurrentCulture;

			try
			{
				Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

				var jObject = (JObject)JsonConvert.DeserializeObject(text);

				this.LoadFromJsonObject(jObject);

			}
			catch (Exception ex)
			{
				StiLogService.Write(this.GetType(), "Loading report...ERROR");
				StiLogService.Write(this.GetType(), ex);

				if ((!StiOptions.Engine.HideExceptions) || this.IsDesigning) throw;
			}
			finally
			{
				Thread.CurrentThread.CurrentCulture = currentCulture;
			}
		}

		public override void LoadFromJsonObject(JObject jObject)
		{
			base.LoadFromJsonObject(jObject);

			foreach (var property in jObject.Properties())
			{
				switch (property.Name)
				{
					case "ResetPageNumber":
						this.ResetPageNumber = property.DeserializeBool();
						break;

					case "BeginRenderEvent":
						this.BeginRenderEvent.LoadFromJsonObject((JObject)property.Value);
						break;

					case "RenderingEvent":
						this.RenderingEvent.LoadFromJsonObject((JObject)property.Value);
						break;

					case "EndRenderEvent":
						this.EndRenderEvent.LoadFromJsonObject((JObject)property.Value);
						break;

					case "ColumnBeginRenderEvent":
						this.ColumnBeginRenderEvent.LoadFromJsonObject((JObject)property.Value);
						break;

					case "ColumnEndRenderEvent":
						this.ColumnEndRenderEvent.LoadFromJsonObject((JObject)property.Value);
						break;

					case "GetExcelSheetEvent":
						this.GetExcelSheetEvent.LoadFromJsonObject((JObject)property.Value);
						break;

					case "ExcelSheetValue":
						this.ExcelSheetValue = property.DeserializeString();
						break;

					case "ExcelSheet":
						this.ExcelSheet.LoadFromJsonObject((JObject)property.Value);
						break;

					case "PrintOnPreviousPage":
						this.PrintOnPreviousPage = property.DeserializeBool();
						break;

					case "PrintHeadersFootersFromPreviousPage":
						this.PrintHeadersFootersFromPreviousPage = property.DeserializeBool();
						break;

					case "PaperSize":
						this.paperSize = property.DeserializeEnum<PaperKind>();
						break;

					case "PaperSourceOfFirstPage":
						this.PaperSourceOfFirstPage = property.DeserializeString();
						break;

					case "PaperSourceOfOtherPages":
						this.PaperSourceOfOtherPages = property.DeserializeString();
						break;

					case "NumberOfCopies":
						this.numberOfCopies = property.DeserializeInt();
						break;

					case "UnlimitedBreakable":
						this.UnlimitedBreakable = property.DeserializeBool();
						break;

					case "LargeHeight":
						this.LargeHeight = property.DeserializeBool();
						break;

					case "LargeHeightFactor":
						this.largeHeightFactor = property.DeserializeInt();
						break;

					case "StopBeforePrint":
						this.StopBeforePrint = property.DeserializeInt();
						break;

					case "StretchToPrintArea":
						this.StretchToPrintArea = property.DeserializeBool();
						break;

					case "TitleBeforeHeader":
						this.titleBeforeHeader = property.DeserializeBool();
						break;

					case "UnlimitedHeight":
						this.UnlimitedHeight = property.DeserializeBool();
						break;

					case "UnlimitedWidth":
						this.UnlimitedWidth = property.DeserializeBool();
						break;

					case "Orientation":
						this.orientation = property.DeserializeEnum<StiPageOrientation>();
						break;

					case "PageWidth":
						this.pageWidth = property.DeserializeDouble();
						break;

					case "PageHeight":
						this.pageHeight = property.DeserializeDouble();
						break;

					case "SegmentPerWidth":
						this.segmentPerWidth = property.DeserializeInt();
						break;

					case "SegmentPerHeight":
						this.segmentPerHeight = property.DeserializeInt();
						break;

					case "Watermark":
						this.Watermark.LoadFromJsonObject((JObject)property.Value);
						break;

					case "Margins":
						this.Margins.LoadFromJsonObject((JObject)property.Value);
						break;

					case "MirrorMargins":
						this.MirrorMargins = property.DeserializeBool();
						break;

					case "ReportUnit":
						this.ReportUnit = StiUnit.LoadFromJsonObject((JObject)property.Value);
						break;

					case "Icon":
						this.Icon = property.DeserializeByteArray();
						break;
				}
			}
		}
		#endregion

		#region IStiPropertyGridObject
		public override StiComponentId ComponentId => StiComponentId.StiPage;

		public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
		{
			var helper = new StiPropertyCollection();
			var collection = propertyGrid.PropertiesHelper;

			if (level == StiLevel.Basic)
			{
				helper.Add(StiPropertyCategories.Page, new[]
				{
					collection.PaperSize(),
					collection.PageWidth(),
					collection.PageHeight(),
					collection.PageOrientation(),
					collection.Margins(),
					collection.Watermark()
				});
			}
			else if (level == StiLevel.Standard)
			{
				helper.Add(StiPropertyCategories.Page, new[]
				{
					collection.PaperSize(),
					collection.PageWidth(),
					collection.PageHeight(),
					collection.PageOrientation(),
					collection.Margins(),
					collection.Watermark(),
					collection.NumberOfCopies()
				});
			}
			else
			{
				helper.Add(StiPropertyCategories.Page, new[]
				{
					collection.PaperSize(),
					collection.PageWidth(),
					collection.PageHeight(),
					collection.PageOrientation(),
					collection.Margins(),
					collection.Watermark(),
					collection.NumberOfCopies()
				});
			}
			
			if (level == StiLevel.Standard)
			{
				helper.Add(StiPropertyCategories.PageAdditional, new[]
				{
					collection.MirrorMargins(),
					collection.StopBeforePrint(),
					collection.TitleBeforeHeader()
				});
			}
			else if (level == StiLevel.Professional)
			{
				helper.Add(StiPropertyCategories.PageAdditional, new[]
				{
					collection.MirrorMargins(),
					collection.StopBeforePrint(),
					collection.TitleBeforeHeader(),
					collection.UnlimitedHeight(),
					collection.UnlimitedBreakable(),
					collection.SegmentPerWidth(),
					collection.SegmentPerHeight()
				});
			}

			helper.Add(StiPropertyCategories.Columns, new[]
			{
				collection.Columns(),
				collection.ColumnWidth(),
				collection.ColumnGaps(),
				collection.RightToLeft()
			});

			helper.Add(StiPropertyCategories.Appearance, new[]
			{
				collection.PageBrush(),
				collection.Border(),
				collection.Conditions(),
				collection.ComponentStyle()
			});

			if (level == StiLevel.Basic)
			{
				helper.Add(StiPropertyCategories.Behavior, new[]
				{
					propertyGrid.PropertiesHelper.Enabled()
				});
			}
			else
			{
				helper.Add(StiPropertyCategories.Behavior, new[]
				{
					collection.InteractionEditor(),
					collection.Enabled(),
					collection.PrintOnPreviousPage(),
					collection.PrintHeadersFootersFromPreviousPage(),
					collection.ResetPageNumber()
				});
			}
			
			if (level == StiLevel.Basic)
			{
				helper.Add(StiPropertyCategories.Design, new[]
				{
					collection.Name(),
					collection.PageIcon()
				});
			}
			else if (level == StiLevel.Standard)
			{
				helper.Add(StiPropertyCategories.Design, new[]
				{
					collection.PageName(),
					collection.PageAlias(),
					collection.PageIcon()
				});
			}
			else
			{
				helper.Add(StiPropertyCategories.Design, new[]
				{
					collection.PageName(),
					collection.PageAlias(),
					collection.LargeHeight(),
					collection.LargeHeightFactor(),
					collection.PageIcon()
				});
			}

			if (level == StiLevel.Professional)
			{
				helper.Add(StiPropertyCategories.Export, new[]
				{
					collection.ExcelSheet()
				});
			}

			return helper;
		}

		public override StiEventCollection GetEvents(IStiPropertyGrid propertyGrid)
		{
            return new StiEventCollection
            {
                {
                    StiPropertyCategories.MouseEvents,
                    new[]
					{
						StiPropertyEventId.ClickEvent,
						StiPropertyEventId.DoubleClickEvent,
						StiPropertyEventId.MouseEnterEvent,
						StiPropertyEventId.MouseLeaveEvent
					}
                },
                {
                    StiPropertyCategories.NavigationEvents,
                    new[]
					{
						StiPropertyEventId.GetBookmarkEvent,
						StiPropertyEventId.GetDrillDownReportEvent,
						StiPropertyEventId.GetHyperlinkEvent,
						StiPropertyEventId.GetPointerEvent,
					}
                },
                {
                    StiPropertyCategories.PrintEvents,
                    new[]
					{
						StiPropertyEventId.AfterPrintEvent,
						StiPropertyEventId.BeforePrintEvent,
					}
                },
                {
                    StiPropertyCategories.RenderEvents,
                    new[]
					{
						StiPropertyEventId.BeginRenderEvent,
						StiPropertyEventId.ColumnBeginRenderEvent,
						StiPropertyEventId.ColumnEndRenderEvent,
						StiPropertyEventId.EndRenderEvent,
						StiPropertyEventId.RenderingEvent,
					}
                },
                {
                    StiPropertyCategories.ValueEvents,
                    new[]
					{
						StiPropertyEventId.GetExcelSheetEvent,
						StiPropertyEventId.GetTagEvent,
						StiPropertyEventId.GetToolTipEvent,
					}
                }
            };
		}
		#endregion

		#region IStiResetPageNumber
		/// <summary>
		/// Allows to reset page number on this page.
		/// </summary>
		[DefaultValue(false)]
		[StiSerializable]
		[StiCategory("Behavior")]
		[StiOrder(StiPropertyOrder.BehaviorResetPageNumber)]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		[Description("Allows to reset page number on this page.")]
		[StiShowInContextMenu]
		[StiPropertyLevel(StiLevel.Standard)]
		public virtual bool ResetPageNumber { get; set; }
		#endregion

		#region IStiUnitConvert
		public double ConvertToHInches(StiUnit unit, double value)
		{
			if (unit is StiCentimetersUnit)
			{
				if (value == 21) 
					return 827;

				if (value == 29.7) 
					return 1169;

				if (value == 1) 
					return 39;
			}
			else if (unit is StiMillimetersUnit)
			{
				if (value == 210) 
					return 827;

				if (value == 297) 
					return 1169;

				if (value == 10)
					return 39;
			}

			return unit.ConvertToHInches(value);
		}

		public double ConvertFromHInches(StiUnit unit, double value)
		{
			if (unit is StiCentimetersUnit)
			{
				if (value == 827)
					return 21;

				if (value == 1169) 
					return 29.7;

				if (value == 39) 
					return 1;
			}
			else if (unit is StiMillimetersUnit)
			{
				if (value == 827) 
					return 210;

				if (value == 1169) 
					return 297;

				if (value == 39) 
					return 10;
			}

			return unit.ConvertFromHInches(value);
		}

		/// <summary>
		/// Converts a component out of one unit into another.
		/// </summary>
		/// <param name="oldUnit">Old units.</param>
		/// <param name="newUnit">New units.</param>
		public override void Convert(StiUnit oldUnit, StiUnit newUnit, bool isReportSnapshot = false)
		{
			this.PageWidth = ConvertFromHInches(newUnit, ConvertToHInches(oldUnit, this.PageWidth));
			this.PageHeight = ConvertFromHInches(newUnit, ConvertToHInches(oldUnit, this.PageHeight));

			this.Margins = new StiMargins(
				ConvertFromHInches(newUnit, ConvertToHInches(oldUnit, this.Margins.Left)),
				ConvertFromHInches(newUnit, ConvertToHInches(oldUnit, this.Margins.Right)),
				ConvertFromHInches(newUnit, ConvertToHInches(oldUnit, this.Margins.Top)),
				ConvertFromHInches(newUnit, ConvertToHInches(oldUnit, this.Margins.Bottom)));

			this.ColumnWidth = ConvertFromHInches(newUnit, ConvertToHInches(oldUnit, this.ColumnWidth));
			this.ColumnGaps = ConvertFromHInches(newUnit, ConvertToHInches(oldUnit, this.ColumnGaps));

			foreach (StiComponent component in Components)
				component.Convert(oldUnit, newUnit, isReportSnapshot);

		}
		#endregion

		#region IStiCanShrink override
		[Browsable(false)]
		[StiNonSerialized]
		public override bool CanShrink
		{
			get
			{
				return base.CanShrink;
			}
			set
			{
			}
		}
		#endregion

		#region ICloneable override
		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public override object Clone(bool cloneProperties, bool cloneComponents)
		{
			var page = base.Clone(cloneProperties, cloneComponents) as StiPage;

			page.pageInfoV1 = this.PageInfoV1.Clone() as StiPageInfoV1;
			page.pageInfoV2 = this.PageInfoV2.Clone() as StiPageInfoV2;

			page.Margins = (StiMargins)Margins.Clone();
			page.orientation = this.orientation;

			page.watermark = this.watermark != null ? (StiWatermark)this.watermark.Clone() : null;

			page.Components = new StiComponentsCollection(page);

			if (cloneComponents)
			{
				foreach (StiComponent comp in this.Components)
					page.Components.Add((StiComponent)comp.Clone());

				//Sets the reference to current page
				var comps = page.GetComponents();
				foreach (StiComponent comp in comps) comp.Page = page;
			}

			return page;
		}
		#endregion

		#region IStiGlobalizationProvider
		/// <summary>
		/// Sets localized string to specified property name.
		/// </summary>
		void IStiGlobalizationProvider.SetString(string propertyName, string value)
		{
			if (propertyName == "ExcelSheet")
				this.ExcelSheet.Value = value;

			else
				throw new ArgumentException($"Property with name {propertyName}");
		}

		/// <summary>
		/// Gets localized string from specified property name.
		/// </summary>
		string IStiGlobalizationProvider.GetString(string propertyName)
		{
			if (propertyName == "ExcelSheet")
				return this.ExcelSheet.Value;

			throw new ArgumentException($"Property with name {propertyName}");
		}

		/// <summary>
		/// Returns array of the property names which can be localized.
		/// </summary>
		string[] IStiGlobalizationProvider.GetAllStrings()
		{
			var strs = new List<string>
			{
				"ExcelSheet"
			};

			return strs.ToArray();
		}
		#endregion

		#region IStiCanGrow override
		[Browsable(false)]
		[StiNonSerialized]
		public override bool CanGrow
		{
			get
			{
				return base.CanGrow;
			}
			set
			{
			}
		}
		#endregion

		#region IStiPrintOn Browsable(false)
		[Browsable(false)]
		[StiNonSerialized]
		public override StiPrintOnType PrintOn
		{
			get
			{
				return base.PrintOn;
			}
			set
			{
				base.PrintOn = value;
			}
		}
		#endregion

		#region IStiCanBreak
		[StiNonSerialized]
		[Browsable(false)]
		public override bool CanBreak
		{
			get
			{
				return base.CanBreak;
			}
			set
			{
				base.CanBreak = value;
			}
		}
		#endregion

		#region IStiReportRage
		public string ParseExpression(string text, bool allowReturnNull = false)
		{
			return StiExpressionHelper.ParseText(this, text, allowReturnNull);
		}
		#endregion

		#region Renderer margins
		/// <summary>
		/// Returns bounds of printing of the printer.
		/// </summary>
		public RectangleF GetPrinterMargins(Graphics g)
		{
			try
			{
				var graphicsPtr = g.GetHdc();

				var offx = StiPageWin32.GetDeviceCaps(graphicsPtr, StiPageWin32.PHYSICALOFFSETX);
				var offy = StiPageWin32.GetDeviceCaps(graphicsPtr, StiPageWin32.PHYSICALOFFSETY);
				var resx = StiPageWin32.GetDeviceCaps(graphicsPtr, StiPageWin32.HORZRES);
				var hsz = StiPageWin32.GetDeviceCaps(graphicsPtr, StiPageWin32.HORZSIZE) / 25.4f;
				var vsz = StiPageWin32.GetDeviceCaps(graphicsPtr, StiPageWin32.VERTSIZE) / 25.4f;

				g.ReleaseHdc(graphicsPtr);

				var ppix = resx / hsz;

				var left = offx / ppix * 100.0f;
				var top = offy / ppix * 100.0f;
				var width = hsz * 100.0f;
				var heigth = vsz * 100.0f;

				return new RectangleF(left, top, width, heigth);
			}
			catch
			{
				if (StiDpiHelper.IsLinux)
					return new RectangleF(0, 0, 827, 1069);
				return new RectangleF(39, 39, 748, 1043);
			}
		}
		#endregion

		#region Render override
		private StiPageInfoV1 pageInfoV1;
		[Browsable(false)]
		public StiPageInfoV1 PageInfoV1
		{
			get
			{
				return pageInfoV1 ?? (pageInfoV1 = new StiPageInfoV1());
			}
		}

		private StiPageInfoV2 pageInfoV2;
		[Browsable(false)]
		public StiPageInfoV2 PageInfoV2
		{
			get
			{
				return pageInfoV2 ?? (pageInfoV2 = new StiPageInfoV2());
			}
		}

		[StiEngine(StiEngineVersion.EngineV1)]
		internal void ProcessPageAfterRender()
		{
			var builder = StiV1Builder.GetBuilder(typeof(StiPage)) as StiPageV1Builder;
			builder.ProcessPageAfterRender(this);
		}

		[StiEngine(StiEngineVersion.EngineV1)]
		internal void ProcessPageBeforeRender()
		{
			var builder = StiV1Builder.GetBuilder(typeof(StiPage)) as StiPageV1Builder;
			builder.ProcessPageBeforeRender(this);
		}
		#endregion

		#region Dock override
		/// <summary>
		/// Gets value indicates that this is an automatic docking.
		/// </summary>
		[Browsable(false)]
		public override bool IsAutomaticDock => true;

		/// <summary>
		/// Gets or sets a type of the component docking.
		/// </summary>
		[Browsable(false)]
		[StiNonSerialized]
		public override StiDockStyle DockStyle
		{
			get
			{
				return base.DockStyle;
			}
			set
			{
				base.DockStyle = value;
			}
		}
		#endregion

		#region Position override
		[Browsable(false)]
		[StiNonSerialized]
		public override SizeD MinSize
		{
			get
			{
				return base.MinSize;
			}
			set
			{
				base.MinSize = value;
			}
		}

		[Browsable(false)]
		[StiNonSerialized]
		public override SizeD MaxSize
		{
			get
			{
				return base.MaxSize;
			}
			set
			{
				base.MaxSize = value;
			}
		}

		/// <summary>
		/// Gets or sets left margin.
		/// </summary>
		[Browsable(false)]
		public override double Left
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		/// <summary>
		/// Gets or sets top margin.
		/// </summary>
		[Browsable(false)]
		public override double Top
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		/// <summary>
		/// Gets or sets width.
		/// </summary>
		[Browsable(false)]
		public override double Width
		{
			get
			{
				return (PageWidth - Margins.Left - Margins.Right) * SegmentPerWidth;
			}
			set
			{
				pageWidth = value / SegmentPerWidth + Margins.Left + Margins.Right;
			}
		}

		/// <summary>
		/// Gets or sets height.
		/// </summary>
		[Browsable(false)]
		public override double Height
		{
			get
			{
				var value = (pageHeight - Margins.Top - Margins.Bottom) * SegmentPerHeight;

				if (IsDesigning)
				{
					if (LargeHeight)
						value *= LargeHeightAutoFactor;
					else
						value += (pageHeight - Margins.Top - Margins.Bottom) * (LargeHeightAutoFactor - 1);
				}
				if (value < 0) value = 0;
				return value;
			}
			set
			{
				pageHeight = value / SegmentPerHeight + Margins.Top + Margins.Bottom;

				if (Page.LargeHeight && IsDesigning)
					pageHeight /= LargeHeightFactor;
			}
		}

		/// <summary>
		/// Gets right margin.
		/// </summary>
		[Browsable(false)]
		public override double Right => Width;

		/// <summary>
		/// Gets bottom margin.
		/// </summary>
		[Browsable(false)]
		public override double Bottom => Height;

		/// <summary>
		/// Gets or sets the client area of a component.
		/// </summary>
		[Browsable(false)]
		[StiNonSerialized]
		public override RectangleD ClientRectangle
		{
			get
			{
				return new RectangleD(0, 0, Width, Height);
			}
			set
			{
				Width = value.Width;
				Height = value.Height;
			}
		}

		/// <summary>
		/// Gets or sets a rectangle of the component which it fills. Docking occurs in accordance to the area
		/// (Cross - components are docked by ClientRectangle).
		/// </summary>
		[Browsable(false)]
		[StiNonSerialized]
		public override RectangleD DisplayRectangle
		{
			get
			{
				return new RectangleD(0, 0, Width + Margins.Left + Margins.Right,
					Height + Margins.Top + Margins.Bottom);
			}
		}
		#endregion

		#region Paint
		/// <summary>
		/// Internal use only.
		/// </summary>
		[Browsable(false)]
		public bool DenyDrawSegmentMode { get; set; }

		/// <summary>
		/// Internal use only.
		/// </summary>
		[Browsable(false)]
		public RectangleD SelectedRectangle { get; set; } = RectangleD.Empty;

		private ArrayList selectedComponents;
		[Browsable(false)]
		public ArrayList SelectedComponents
		{
			get
			{
				return selectedComponents ?? (selectedComponents = new ArrayList());
			}
		}

		/// <summary>
		/// Paints a page.
		/// </summary>
		/// <param name="g">Graphics for paints on.</param>
		public void Paint(Graphics g)
		{
			Paint(new StiPaintEventArgs(g, RectangleD.Empty));
		}
		#endregion

		#region StiComponent override
		[Browsable(false)]
		[StiNonSerialized]
		public override StiAnchorMode Anchor
		{
			get
			{
				return base.Anchor;
			}
			set
			{
				base.Anchor = value;
			}
		}

		[StiNonSerialized]
		[Browsable(false)]
		public override bool UseParentStyles
		{
			get
			{
				return base.UseParentStyles;
			}
			set
			{
				if (base.UseParentStyles != value)
					base.UseParentStyles = value;
			}
		}

		[Browsable(false)]
		[StiNonSerialized]
		public override bool GrowToHeight
		{
			get
			{
				return base.GrowToHeight;
			}
			set
			{
			}
		}

		[StiNonSerialized]
		[Browsable(false)]
		public override StiShiftMode ShiftMode
		{
			get
			{
				return StiShiftMode.None;
			}
			set
			{
			}
		}

		[Browsable(false)]
		[StiNonSerialized]
		public override bool Printable
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		/// <summary>
		/// Return events collection of this component.
		/// </summary>
		public override StiEventsCollection GetEvents()
		{
			var events = base.GetEvents();

			if (BeginRenderEvent != null)
				events.Add(BeginRenderEvent);

			if (RenderingEvent != null)
				events.Add(RenderingEvent);

			if (EndRenderEvent != null)
				events.Add(EndRenderEvent);

			if (GetExcelSheetEvent != null)
				events.Add(GetExcelSheetEvent);

			if (ColumnBeginRenderEvent != null)
				events.Add(ColumnBeginRenderEvent);
			
			if (ColumnEndRenderEvent != null)
				events.Add(ColumnEndRenderEvent);

			return events;
		}

		/// <summary>
		/// Gets a localized component name.
		/// </summary>
		[Browsable(false)]
		public override string LocalizedName
		{
			get
			{
				return StiLocalization.CultureName == "en"
					? "Page"
					: StiLocalization.Get("Components", "StiPage");
			}
		}

		[StiNonSerialized]
		public override StiPage Page
		{
			get
			{
				return this;
			}
			set
			{
			}
		}

		[StiNonSerialized]
		public override StiContainer Parent
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		[StiNonSerialized]
		[Browsable(false)]
		public override StiRestrictions Restrictions
		{
			get
			{
				return base.Restrictions;
			}
			set
			{
				base.Restrictions = value;
			}
		}
		#endregion

		#region Events
		/// <summary>
		/// Invokes all events for this components.
		/// </summary>
		public override void InvokeEvents()
		{
			base.InvokeEvents();

			try
			{
				var isInterpretationMode = Report != null && Report.CalculationMode == StiCalculationMode.Interpretation;

				#region GetExcelSheet
				if (isInterpretationMode)
				{
					if (ExcelSheetValue == null && this.ExcelSheet.Value.Length > 0)
					{
						var parserResult = StiParser.ParseTextValue(this.ExcelSheet.Value, this);
						if (parserResult != null)
							this.ExcelSheetValue = Report.ToString(parserResult);
					}
				}

				if (this.Events[EventGetExcelSheet] != null && this.ExcelSheetValue == null)
				{
					var e = new StiGetExcelSheetEventArgs();
					InvokeGetExcelSheet(this, e);
					if (e.Value != null) this.ExcelSheetValue = e.Value;
				}

				if (this.PrintOnPreviousPage && this.PrintHeadersFootersFromPreviousPage && Report != null && Report.RenderedPages.Count > 0)
					this.ExcelSheetValue = Report.RenderedPages[Report.RenderedPages.Count - 1].ExcelSheetValue;
				#endregion
			}
			catch (Exception e)
			{
				StiLogService.Write(this.GetType(), "DoEvents...ERROR");
				StiLogService.Write(this.GetType(), e);

				if (Report != null)
					Report.WriteToReportRenderingMessages(this.Name + " " + e.Message);
			}
		}


		#region PagePainting
		/// <summary>
		/// Occurs before the page painting.
		/// </summary>
		public static event StiPagePaintEventHandler PagePainting;

		/// <summary>
		/// Raises the PagePainting event for this page.
		/// </summary>
		public static void InvokePagePainting(StiPage sender, StiPagePaintEventArgs e)
		{
			PagePainting?.Invoke(sender, e);
		}
		#endregion

		#region PagePainted
		/// <summary>
		/// Occurs after the page painted.
		/// </summary>
		public static event StiPagePaintEventHandler PagePainted;

		/// <summary>
		/// Raises the PagePainted event for this page.
		/// </summary>
		public static void InvokePagePainted(StiPage sender, StiPagePaintEventArgs e)
		{
			PagePainted?.Invoke(sender, e);
		}
		#endregion

		#region BeginRender
		private static readonly object EventBeginRender = new object();

		/// <summary>
		/// Occurs when when a page begins to render.
		/// </summary>
		public event EventHandler BeginRender
		{
			add
			{
				Events.AddHandler(EventBeginRender, value);
			}
			remove
			{
				Events.RemoveHandler(EventBeginRender, value);
			}
		}

		/// <summary>
		/// Raises the BeginRender event for this component.
		/// </summary>
		protected virtual void OnBeginRender(EventArgs e)
		{
		}


		/// <summary>
		/// Raises the BeginRender event for this component.
		/// </summary>
		public void InvokeBeginRender()
		{
			try
			{
				OnBeginRender(EventArgs.Empty);
				var handler = Events[EventBeginRender] as EventHandler;
				handler?.Invoke(this, EventArgs.Empty);

				StiBlocklyHelper.InvokeBlockly(this.Report, this, BeginRenderEvent, EventArgs.Empty);
			}
			catch (Exception ex)
			{
				StiLogService.Write(this.GetType(), $"{Name} InvokeBeginRender...ERROR");
				StiLogService.Write(this.GetType(), $"{Name} {ex.Message}");

				if (Report != null)
					Report.WriteToReportRenderingMessages($"{Name}.BeginRender event error: {ex.Message}");
			}
		}


		/// <summary>
		/// Occurs when when a page begins to render.
		/// </summary>
		[StiSerializable(StiSerializeTypes.SerializeToCode | StiSerializeTypes.SerializeToDesigner | StiSerializeTypes.SerializeToSaveLoad)]
		[StiCategory("RenderEvents")]
		[Browsable(false)]
		[Description("Occurs when when a page begins to render.")]
		public virtual StiBeginRenderEvent BeginRenderEvent
		{
			get
			{
				return new StiBeginRenderEvent(this);
			}
			set
			{
				if (value != null) value.Set(this, value.Script);
			}
		}

		#endregion

		#region Rendering
		private static readonly object EventRendering = new object();

		/// <summary>
		/// Occurs when a page rendering.
		/// </summary>
		public event EventHandler Rendering
		{
			add
			{
				Events.AddHandler(EventRendering, value);
			}
			remove
			{
				Events.RemoveHandler(EventRendering, value);
			}
		}

		/// <summary>
		/// Raises the Rendering event for this component.
		/// </summary>
		protected virtual void OnRendering(EventArgs e)
		{

		}


		/// <summary>
		/// Raises the Rendering event for this component.
		/// </summary>
		public void InvokeRendering()
		{
			OnRendering(EventArgs.Empty);
			var handler = Events[EventRendering] as EventHandler;
			handler?.Invoke(this, EventArgs.Empty);

			StiBlocklyHelper.InvokeBlockly(this.Report, this, RenderingEvent);
		}


		/// <summary>
		/// Occurs when a page rendering.
		/// </summary>
		[StiSerializable(StiSerializeTypes.SerializeToCode | StiSerializeTypes.SerializeToDesigner | StiSerializeTypes.SerializeToSaveLoad)]
		[StiCategory("RenderEvents")]
		[Browsable(false)]
		[Description("Occurs when a page rendering.")]
		public virtual StiRenderingEvent RenderingEvent
		{
			get
			{
				return new StiRenderingEvent(this);
			}
			set
			{
				if (value != null) value.Set(this, value.Script);
			}
		}

		#endregion

		#region EndRender
		private static readonly object EventEndRender = new object();

		/// <summary>
		/// Occurs when when a page ends to render.
		/// </summary>
		public event EventHandler EndRender
		{
			add
			{
				Events.AddHandler(EventEndRender, value);
			}
			remove
			{
				Events.RemoveHandler(EventEndRender, value);
			}
		}

		/// <summary>
		/// Raises the EndRender event for this component.
		/// </summary>
		protected virtual void OnEndRender(EventArgs e)
		{
		}


		/// <summary>
		/// Raises the EndRender event for this component.
		/// </summary>
		public void InvokeEndRender()
		{
			OnEndRender(EventArgs.Empty);
			var handler = Events[EventEndRender] as EventHandler;
			handler?.Invoke(this, EventArgs.Empty);

			StiBlocklyHelper.InvokeBlockly(this.Report, this, EndRenderEvent);
		}


		/// <summary>
		/// Occurs when when a page ends to render.
		/// </summary>
		[StiSerializable(StiSerializeTypes.SerializeToCode | StiSerializeTypes.SerializeToDesigner | StiSerializeTypes.SerializeToSaveLoad)]
		[StiCategory("RenderEvents")]
		[Browsable(false)]
		[Description("Occurs when when a page ends to render.")]
		public virtual StiEndRenderEvent EndRenderEvent
		{
			get
			{
				return new StiEndRenderEvent(this);
			}
			set
			{
				if (value != null) value.Set(this, value.Script);
			}
		}

		#endregion

		#region ColumnBeginRender
		private static readonly object EventColumnBeginRender = new object();

		/// <summary>
		/// Occurs when start render column.
		/// </summary>
		public event EventHandler ColumnBeginRender
		{
			add
			{
				Events.AddHandler(EventColumnBeginRender, value);
			}
			remove
			{
				Events.RemoveHandler(EventColumnBeginRender, value);
			}
		}


		/// <summary>
		/// Raises the ColumnBeginRender event for this component.
		/// </summary>
		protected virtual void OnColumnBeginRender(EventArgs e)
		{
		}


		/// <summary>
		/// Raises the ColumnBeginRender event for this component.
		/// </summary>
		public void InvokeColumnBeginRender()
		{
			InvokeColumnBeginRender(this, EventArgs.Empty);
		}


		/// <summary>
		/// Raises the ColumnBeginRender event for this component.
		/// </summary>
		public void InvokeColumnBeginRender(object sender, EventArgs e)
		{
			OnColumnBeginRender(e);
			var handler = Events[EventColumnBeginRender] as EventHandler;
			handler?.Invoke(sender, e);

			StiBlocklyHelper.InvokeBlockly(this.report, sender, ColumnBeginRenderEvent);
		}


		/// <summary>
		/// Occurs when start render column.
		/// </summary>
		[StiSerializable]
		[StiCategory("RenderEvents")]
		[Browsable(false)]
		[Description("Occurs when start render column.")]
		public virtual StiColumnBeginRenderEvent ColumnBeginRenderEvent
		{
			get
			{
				return new StiColumnBeginRenderEvent(this);
			}
			set
			{
				if (value != null) value.Set(this, value.Script);
			}
		}

		#endregion

		#region ColumnEndRender
		private static readonly object EventColumnEndRender = new object();

		/// <summary>
		/// Occurs when end render column.
		/// </summary>
		public event EventHandler ColumnEndRender
		{
			add
			{
				Events.AddHandler(EventColumnEndRender, value);
			}
			remove
			{
				Events.RemoveHandler(EventColumnEndRender, value);
			}
		}


		/// <summary>
		/// Raises the ColumnEndRender event for this component.
		/// </summary>
		protected virtual void OnColumnEndRender(EventArgs e)
		{
		}


		/// <summary>
		/// Raises the ColumnEndRender event for this component.
		/// </summary>
		public void InvokeColumnEndRender()
		{
			InvokeColumnEndRender(this, EventArgs.Empty);
		}


		/// <summary>
		/// Raises the ColumnBeginRender event for this component.
		/// </summary>
		public void InvokeColumnEndRender(object sender, EventArgs e)
		{
			OnColumnEndRender(e);
			var handler = Events[EventColumnEndRender] as EventHandler;
			handler?.Invoke(sender, e);

			StiBlocklyHelper.InvokeBlockly(this.report, sender, ColumnEndRenderEvent);
		}


		/// <summary>
		/// Occurs when end render column.
		/// </summary>
		[StiSerializable]
		[StiCategory("RenderEvents")]
		[Browsable(false)]
		[Description("Occurs when end render column.")]
		public virtual StiColumnEndRenderEvent ColumnEndRenderEvent
		{
			get
			{
				return new StiColumnEndRenderEvent(this);
			}
			set
			{
				if (value != null) value.Set(this, value.Script);
			}
		}

		#endregion

		#region GetExcelSheet
		private static readonly object EventGetExcelSheet = new object();

		/// <summary>
		/// Occurs when the ExcelSheet is calculated.
		/// </summary>
		public event StiGetExcelSheetEventHandler GetExcelSheet
		{
			add
			{
				Events.AddHandler(EventGetExcelSheet, value);
			}
			remove
			{
				Events.RemoveHandler(EventGetExcelSheet, value);
			}
		}


		/// <summary>
		/// Raises the GetExcelSheet event.
		/// </summary>
		protected virtual void OnGetExcelSheet(StiGetExcelSheetEventArgs e)
		{
		}


		/// <summary>
		/// Raises the GetExcelSheet event.
		/// </summary>
		public virtual void InvokeGetExcelSheet(StiComponent sender, StiGetExcelSheetEventArgs e)
		{
			try
			{
				OnGetExcelSheet(e);

				var handler = Events[EventGetExcelSheet] as StiGetExcelSheetEventHandler;
				handler?.Invoke(sender, e);

				StiBlocklyHelper.InvokeBlockly(this.Report, this, GetExcelSheetEvent, e);
			}
			catch (Exception ex)
			{
				string str = string.Format("Expression in ExcelSheet property of '{0}' can't be evaluated!", this.Name);
				StiLogService.Write(this.GetType(), str);
				StiLogService.Write(this.GetType(), ex.Message);
				Report.WriteToReportRenderingMessages(str);
			}
		}


		/// <summary>
		/// Occurs when the ExcelSheet is calculated.
		/// </summary>
		[StiSerializable]
		[StiCategory("ValueEvents")]
		[Browsable(false)]
		[Description("Occurs when the ExcelSheet is calculated.")]
		public virtual StiGetExcelSheetEvent GetExcelSheetEvent
		{
			get
			{
				return new StiGetExcelSheetEvent(this);
			}
			set
			{
				if (value != null) value.Set(this, value.Script);
			}
		}

		#endregion
		#endregion

		#region Expression
		#region ExcelSheet
		/// <summary>
		/// Gets or sets name of excel sheet.
		/// </summary>
		[Browsable(false)]
		[Description("Gets or sets name of excel sheet.")]
		[DefaultValue(null)]
		[StiSerializable]
		public string ExcelSheetValue { get; set; }

		/// <summary>
		/// Gets or sets an expression used for generation name of excel sheet.
		/// </summary>
		[StiCategory("Export")]
		[StiOrder(StiPropertyOrder.ExportExcelSheet)]
		[StiSerializable]
		[Description("Gets or sets an expression used for generation name of excel sheet.")]
		[StiPropertyLevel(StiLevel.Professional)]
		public virtual StiExcelSheetExpression ExcelSheet
		{
			get
			{
				return new StiExcelSheetExpression(this, "ExcelSheet");
			}
			set
			{
				if (value != null)
					value.Set(this, "ExcelSheet", value.Value);
			}
		}
		#endregion
		#endregion

		#region Properties.This
		[Browsable(false)]
		public bool IsPrinted { get; set; }

		/// <summary>
		/// Gets zoom of a report.
		/// </summary>
		[Browsable(false)]
		public virtual double Zoom => Report == null || Report.Info == null ? 1 : Report.Info.Zoom;

		/// <summary>
		/// Gets grid size in the unit.
		/// </summary>
		[Browsable(false)]
		public virtual double GridSize
		{
			get
			{
				if (IsDashboard)
					return Report.Info.GridSizePoints;

				if (IsScreen)
					return Report.Info.GridSizeScreenPoints;

				if (Report.Unit is StiMillimetersUnit)
					return Report.Info.GridSizeMillimeters;

				if (Report.Unit is StiCentimetersUnit)
					return Report.Info.GridSizeCentimetres;

				if (Report.Unit is StiHundredthsOfInchUnit)
					return Report.Info.GridSizeHundredthsOfInch;

				return Report.Info.GridSizeInch;
			}
		}

		/// <summary>
		/// Gets or sets value that indicates that the page will start to be 
		/// rendered on the free space of the previous page.
		/// </summary>
		[StiSerializable(StiSerializeTypes.SerializeToCode | StiSerializeTypes.SerializeToDesigner | StiSerializeTypes.SerializeToSaveLoad)]
		[DefaultValue(false)]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		[StiCategory("Behavior")]
		[StiOrder(StiPropertyOrder.BehaviorPrintOnPreviousPage)]
		[Description("Gets or sets value that indicates that the page will start to be " +
			 "rendered on the free space of the previous page.")]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		[StiShowInContextMenu]
		[StiPropertyLevel(StiLevel.Standard)]
		public virtual bool PrintOnPreviousPage { get; set; }

		/// <summary>
		/// Gets or sets value which indicates that, on this page, it is necessary to print
		/// headers and footers of the previous page.
		/// </summary>
		[StiSerializable(StiSerializeTypes.SerializeToCode | StiSerializeTypes.SerializeToDesigner | StiSerializeTypes.SerializeToSaveLoad)]
		[DefaultValue(false)]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		[StiCategory("Behavior")]
		[StiOrder(StiPropertyOrder.BehaviorPrintHeadersFootersFromPreviousPage)]
		[Description("Gets or sets value which indicates that, on this page, it is necessary to print" +
			 "headers and footers of the previous page.")]
		[StiShowInContextMenu]
		[StiPropertyLevel(StiLevel.Standard)]
		public virtual bool PrintHeadersFootersFromPreviousPage { get; set; }

		private PaperKind paperSize = PaperKind.Custom;
		/// <summary>
		/// Gets or sets the page size.
		/// </summary>
		[StiSerializable]
		[DefaultValue(PaperKind.Custom)]
		[StiCategory("Page")]
		[StiOrder(StiPropertyOrder.PagePaperSize)]
		[Editor("Stimulsoft.Report.Components.Design.StiPaperSizeEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[Description("Gets or sets the page size.")]
		[RefreshProperties(RefreshProperties.All)]
		[StiPropertyLevel(StiLevel.Basic)]
		public virtual PaperKind PaperSize
		{
			get
			{
				return paperSize;
			}
			set
			{
				if (this.paperSize != value)
				{
					this.paperSize = value;

					if (value == PaperKind.Custom || report == null) return;

					var size = StiPageHelper.GetPaperSizeFromPaperKind(value);
					if (size == null) return;

					var pageSize = StiPageHelper.GetPaperSize(this, size);
					this.PageWidth = pageSize.Width;
					this.PageHeight = pageSize.Height;
				}
			}
		}

		/// <summary>
		/// Gets or sets the paper source for first page. Some printers does not support this feature.
		/// </summary>
		[StiNonSerialized]
		[Browsable(false)]
		[DefaultValue(PaperSourceKind.Custom)]
		[StiCategory("Page")]
		[Editor("Stimulsoft.Report.Components.Design.StiPaperSourceEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[Description("Gets or sets the paper source for first page. Some printers does not support this feature.")]
		[RefreshProperties(RefreshProperties.All)]
		[StiGuiMode(StiGuiMode.Gdi)]
		[StiPropertyLevel(StiLevel.Professional)]
		public virtual PaperSourceKind PaperSourceFirstPage
		{
			get
			{
				if (!StiOptions.Print.AllowUsePaperSizesFromPrinterSettings) return PaperSourceKind.Custom;

				var printerSetting = new PrinterSettings();
				foreach (PaperSource paper in printerSetting.PaperSources)
				{
					if (paper.SourceName == this.PaperSourceOfFirstPage)
						return paper.Kind;
				}

				return PaperSourceKind.Custom;
			}
			set
			{
				if (value == PaperSourceKind.Custom || !StiOptions.Print.AllowUsePaperSizesFromPrinterSettings) return;

				var printerSetting = new PrinterSettings();
				foreach (PaperSource paper in printerSetting.PaperSources)
				{
					if (paper.Kind == value)
						this.PaperSourceOfFirstPage = paper.SourceName;
				}
			}
		}

		/// <summary>
		/// Gets or sets the paper source for first page. Some printers does not support this feature.
		/// </summary>
		[StiSerializable]
		[DefaultValue("")]
		[StiCategory("Page")]
		[StiOrder(StiPropertyOrder.PagePaperSourceOfFirstPage)]
		[Editor("Stimulsoft.Report.Components.Design.StiPaperSourceEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[Description("Gets or sets the paper source for first page. Some printers does not support this feature.")]
		[RefreshProperties(RefreshProperties.All)]
		[StiGuiMode(StiGuiMode.Gdi)]
		[StiPropertyLevel(StiLevel.Professional)]
		public virtual string PaperSourceOfFirstPage { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the paper source for other pages. Some printers does not support this feature.
		/// </summary>
		[StiNonSerialized]
		[Browsable(false)]
		[DefaultValue(PaperSourceKind.Custom)]
		[StiCategory("Page")]
		[Editor("Stimulsoft.Report.Components.Design.StiPaperSourceEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[Description("Gets or sets the paper source for other pages. Some printers does not support this feature.")]
		[RefreshProperties(RefreshProperties.All)]
		[StiGuiMode(StiGuiMode.Gdi)]
		[StiPropertyLevel(StiLevel.Professional)]
		public virtual PaperSourceKind PaperSourceOtherPages
		{
			get
			{
				if (!StiOptions.Print.AllowUsePaperSizesFromPrinterSettings)
					return PaperSourceKind.Custom;

				var printerSetting = new PrinterSettings();
				foreach (PaperSource paper in printerSetting.PaperSources)
				{
					if (paper.SourceName == this.PaperSourceOfOtherPages)
						return paper.Kind;
				}

				return PaperSourceKind.Custom;
			}
			set
			{
				if (value == PaperSourceKind.Custom || !StiOptions.Print.AllowUsePaperSizesFromPrinterSettings) return;

				var printerSetting = new PrinterSettings();
				foreach (PaperSource paper in printerSetting.PaperSources)
				{
					if (paper.Kind == value)
						this.PaperSourceOfOtherPages = paper.SourceName;
				}
			}
		}

		/// <summary>
		/// Gets or sets the paper source for first page. Some printers does not support this feature.
		/// </summary>
		[StiSerializable]
		[DefaultValue("")]
		[StiCategory("Page")]
		[StiOrder(StiPropertyOrder.PagePaperSourceOfOtherPages)]
		[Editor("Stimulsoft.Report.Components.Design.StiPaperSourceEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[Description("Gets or sets the paper source for other pages. Some printers does not support this feature.")]
		[RefreshProperties(RefreshProperties.All)]
		[StiGuiMode(StiGuiMode.Gdi)]
		[StiPropertyLevel(StiLevel.Professional)]
		public virtual string PaperSourceOfOtherPages { get; set; } = string.Empty;

		private int numberOfCopies = 1;
		/// <summary>
		/// Gets or sets a value of number of copies of the current page.
		/// </summary>
		[StiSerializable(StiSerializeTypes.SerializeToCode | StiSerializeTypes.SerializeToDesigner | StiSerializeTypes.SerializeToSaveLoad)]
		[DefaultValue(1)]
		[StiCategory("Page")]
		[StiOrder(StiPropertyOrder.PageNumberOfCopies)]
		[Description("Gets or sets a value of number of copies of the current page.")]
		[StiPropertyLevel(StiLevel.Standard)]
		public virtual int NumberOfCopies
		{
			get
			{
				return numberOfCopies;
			}
			set
			{
				if (numberOfCopies != value)
				{
					if (value < 1)
						throw new ArgumentException("Value of NumberOfCopies must be greater than or equal to 1.");

					numberOfCopies = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets value indicates that when printing of multiple-sheet cross-reports, columns and strings are to be broken.
		/// </summary>
		[StiSerializable(StiSerializeTypes.SerializeToCode | StiSerializeTypes.SerializeToDesigner | StiSerializeTypes.SerializeToSaveLoad)]
		[DefaultValue(true)]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		[StiCategory("PageAdditional")]
		[StiOrder(StiPropertyOrder.PageUnlimitedBreakable)]
		[Description("Gets or sets value indicates that when printing of multiple-sheet cross-reports, columns and strings are to be broken.")]
		[StiPropertyLevel(StiLevel.Professional)]
		public virtual bool UnlimitedBreakable { get; set; } = true;

		/// <summary>
		/// Gets or sets value indicates that this page has in designer large height.
		/// </summary>
		[StiSerializable(StiSerializeTypes.SerializeToCode | StiSerializeTypes.SerializeToDesigner | StiSerializeTypes.SerializeToSaveLoad)]
		[DefaultValue(false)]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		[StiCategory("Design")]
		[StiOrder(StiPropertyOrder.DesignLargeHeight)]
		[Description("Gets or sets value indicates that this page has in designer large height.")]
		[StiPropertyLevel(StiLevel.Professional)]
		public virtual bool LargeHeight { get; set; }

		private int largeHeightFactor = 4;
		/// <summary>
		/// Gets or sets large height factor for LargeHeight property of this page.
		/// </summary>
		[StiSerializable(StiSerializeTypes.SerializeToCode | StiSerializeTypes.SerializeToDesigner | StiSerializeTypes.SerializeToSaveLoad)]
		[DefaultValue(4)]
		[StiCategory("Design")]
		[StiOrder(StiPropertyOrder.DesignLargeHeightFactor)]
		[Description("Gets or sets large height factor for LargeHeight property of this page.")]
		[StiPropertyLevel(StiLevel.Professional)]
		public virtual int LargeHeightFactor
		{
			get
			{
				return largeHeightFactor;
			}
			set
			{
				if (value >= 1 && value <= 20)
					largeHeightFactor = value;
			}
		}

		private double largeHeightAutoFactor = 1;
		/// <summary>
		/// Gets or sets large height factor for LargeHeightAuto mode of this page.
		/// </summary>
		[Browsable(false)]
		[StiNonSerialized]
		public virtual double LargeHeightAutoFactor
		{
			get
			{
				if (LargeHeight)
					return LargeHeightFactor;

				if (StiOptions.Designer.AutoLargeHeight)
					return largeHeightAutoFactor;

				return 1;
			}
			set
			{
				if (value < 1)
					largeHeightAutoFactor = 1;

				if (value >= 1 && value <= 21)
					largeHeightAutoFactor = value;
			}
		}

		/// <summary>
		/// Gets or sets the current width of a page segment.
		/// </summary>
		[Browsable(false)]
		public virtual int CurrentWidthSegment { get; set; }

		/// <summary>
		/// Gets or sets the current height of a page segment.
		/// </summary>
		[Browsable(false)]
		public virtual int CurrentHeightSegment { get; set; }

		/// <summary>
		/// Gets or sets the page number. When it is reached then stop rendering.
		/// If the property is 0 then rendering of the report will be stopped.
		/// </summary>
		[StiSerializable(StiSerializeTypes.SerializeToCode | StiSerializeTypes.SerializeToDesigner | StiSerializeTypes.SerializeToSaveLoad)]
		[DefaultValue(0)]
		[StiCategory("PageAdditional")]
		[StiOrder(StiPropertyOrder.PageStopBeforePrint)]
		[Description("Gets or sets the page number. When it is reached then stop rendering. " +
			 "If the property is 0 then rendering of the report will be stopped.")]
		[StiPropertyLevel(StiLevel.Standard)]
		public virtual int StopBeforePrint { get; set; }

		/// <summary>
		/// Gets or sets value, indicates that, when rendering of the report, this page of the report is to be skipped.
		/// Subreport component is used.
		/// </summary>
		[StiNonSerialized]
		[DefaultValue(false)]
		[Browsable(false)]
		public virtual bool Skip { get; set; }

		/// <summary>
		/// Gets or sets value, indicates that, when printing, a page stretches into print area.
		/// </summary>
		[StiSerializable]
		[DefaultValue(false)]
		[StiCategory("PageAdditional")]
		[StiOrder(StiPropertyOrder.PageStretchToPrintArea)]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		[Description("Gets or sets value, indicates that, when printing, a page stretches into print area.")]
		[StiShowInContextMenu]
		[StiGuiMode(StiGuiMode.Gdi)]
		[StiPropertyLevel(StiLevel.Standard)]
		public virtual bool StretchToPrintArea { get; set; }

		private bool titleBeforeHeader;
		/// <summary>
		/// Gets or sets value, indicates that it is necessary to put the report title before the page header.
		/// </summary>
		[StiSerializable(StiSerializeTypes.SerializeToCode | StiSerializeTypes.SerializeToDesigner | StiSerializeTypes.SerializeToSaveLoad)]
		[DefaultValue(false)]
		[StiCategory("PageAdditional")]
		[StiOrder(StiPropertyOrder.PageTitleBeforeHeader)]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		[Description("Gets or sets value, indicates that it is necessary to put the report title before the page header.")]
		[StiShowInContextMenu]
		[StiPropertyLevel(StiLevel.Standard)]
		public virtual bool TitleBeforeHeader
		{
			get
			{
				return titleBeforeHeader;
			}
			set
			{
				if (titleBeforeHeader != value)
				{
					titleBeforeHeader = value;
					this.Correct();
				}
			}
		}

		/// <summary>
		/// Gets or sets value, indicates that the page has an unlimited height.
		/// </summary>
		[StiSerializable]
		[DefaultValue(false)]
		[StiCategory("PageAdditional")]
		[StiOrder(StiPropertyOrder.PageUnlimitedHeight)]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		[Description("Gets or sets value, indicates that the page has an unlimited height.")]
		[StiPropertyLevel(StiLevel.Professional)]
		public virtual bool UnlimitedHeight { get; set; }

		/// <summary>
		/// Gets or sets value, indicates that the page has an unlimited width.
		/// </summary>
		[StiSerializable]
		[DefaultValue(true)]
		[StiCategory("PageAdditional")]
		[StiOrder(StiPropertyOrder.PageUnlimitedWidth)]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		[Description("Gets or sets value, indicates that the page has an unlimited width.")]
		[StiEngine(StiEngineVersion.EngineV1)]
		[StiPropertyLevel(StiLevel.Professional)]
		public virtual bool UnlimitedWidth { get; set; } = true;

		/// <summary>
		/// Gets or sets offset of the selected components on the page.
		/// </summary>
		[Browsable(false)]
		public virtual RectangleD OffsetRectangle { get; set; } = RectangleD.Empty;

		private StiPageOrientation orientation;
		/// <summary>
		/// Gets or sets page orientation.
		/// </summary>
		[RefreshProperties(RefreshProperties.All)]
		[StiSerializable]
		[DefaultValue(StiPageOrientation.Portrait)]
		[StiCategory("Page")]
		[StiOrder(StiPropertyOrder.PageOrientation)]
		[TypeConverter(typeof(StiEnumConverter))]
		[Editor(StiEditors.Enum, typeof(UITypeEditor))]
		[Description("Gets or sets page orientation.")]
		[StiPropertyLevel(StiLevel.Basic)]
		public virtual StiPageOrientation Orientation
		{
			get
			{
				return orientation;
			}
			set
			{
				var left = Margins.Left;
				var right = Margins.Right;
				var top = Margins.Top;
				var bottom = Margins.Bottom;

				if (orientation != value && report != null && (!report.IsSerializing))
				{
					var temp = PageHeight;
					PageHeight = PageWidth;
					PageWidth = temp;

					Margins = value == StiPageOrientation.Landscape
						? new StiMargins(top, bottom, right, left)
						: new StiMargins(bottom, top, left, right);
				}
				orientation = value;
			}
		}

		[Browsable(false)]
		[StiNonSerialized]
		public override bool Locked
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		[Browsable(false)]
		[StiNonSerialized]
		public override bool Linked
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		private double pageWidth;
		/// <summary>
		/// Gets or sets the total width of the page.
		/// </summary>
		[StiCategory("Page")]
		[StiOrder(StiPropertyOrder.PagePageWidth)]
		[StiSerializable]
		[DefaultValue(827d)]
		[Description("Gets or sets the total width of the page.")]
		[RefreshProperties(RefreshProperties.All)]
		[StiPropertyLevel(StiLevel.Basic)]
		public virtual double PageWidth
		{
			get
			{
				return pageWidth;
			}
			set
			{
				pageWidth = Math.Round(value, 2);
			}
		}

		private double pageHeight;
		/// <summary>
		/// Gets or sets the total height of the page.
		/// </summary>
		[StiCategory("Page")]
		[StiOrder(StiPropertyOrder.PagePageHeight)]
		[StiSerializable]
		[DefaultValue(1169d)]
		[Description("Gets or sets the total height of the page.")]
		[RefreshProperties(RefreshProperties.All)]
		[StiPropertyLevel(StiLevel.Basic)]
		public virtual double PageHeight
		{
			get
			{
				return pageHeight;
			}
			set
			{
				pageHeight = Math.Round(value, 2);
			}
		}

		private int segmentPerWidth = 1;
		/// <summary>
		/// Gets or sets the number of segments per width.
		/// </summary>
		[StiCategory("PageAdditional")]
		[StiOrder(StiPropertyOrder.PageSegmentPerWidth)]
		[StiSerializable]
		[DefaultValue(1)]
		[Description("Gets or sets the number of segments per width.")]
		[StiPropertyLevel(StiLevel.Professional)]
		public virtual int SegmentPerWidth
		{
			get
			{
				return segmentPerWidth;
			}
			set
			{
				if (value > 0)
					segmentPerWidth = value;
			}
		}

		private int segmentPerHeight = 1;
		/// <summary>
		/// Gets or sets the number of segments per height.
		/// </summary>
		[StiCategory("PageAdditional")]
		[StiOrder(StiPropertyOrder.PageSegmentPerHeight)]
		[StiSerializable]
		[DefaultValue(1)]
		[Description("Gets or sets the number of segments per height.")]
		[StiPropertyLevel(StiLevel.Professional)]
		public virtual int SegmentPerHeight
		{
			get
			{
				return segmentPerHeight;
			}
			set
			{
				if (value > 0)
					segmentPerHeight = value;
			}
		}

		private StiWatermark watermark;
		/// <summary>
		/// The watermark used for the component.
		/// </summary>
		[StiSerializable(StiSerializationVisibility.Class)]
		[StiCategory("Page")]
		[StiOrder(StiPropertyOrder.PageWatermark)]
		[Description("The watermark used for the component.")]
		[Editor("Stimulsoft.Report.Components.Design.StiWatermarkEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[StiPropertyLevel(StiLevel.Basic)]
		public virtual StiWatermark Watermark
		{
			get
			{
				return watermark ?? (watermark = new StiWatermark());
			}
			set
			{
				watermark = value;
			}
		}

		private bool ShouldSerializeWatermark()
		{
			return watermark == null || !watermark.IsDefault;
		}

		/// <summary>
		/// Gets or sets page margins.
		/// </summary>
		[StiSerializable]
		[StiCategory("Page")]
		[StiOrder(StiPropertyOrder.PageMargins)]
		[Description("Gets or sets page margins.")]
		[StiPropertyLevel(StiLevel.Basic)]
		public virtual StiMargins Margins { get; set; } = new StiMargins(39, 39, 39, 39);

		/// <summary>
		/// Gets or sets value indicates that mirror margins are set.
		/// </summary>
		[StiSerializable(StiSerializeTypes.SerializeToCode | StiSerializeTypes.SerializeToDesigner | StiSerializeTypes.SerializeToSaveLoad)]
		[DefaultValue(false)]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		[StiCategory("PageAdditional")]
		[StiOrder(StiPropertyOrder.PageMirrorMargins)]
		[Description("Gets or sets value indicates that mirror margins are set.")]
		[StiPropertyLevel(StiLevel.Standard)]
		public virtual bool MirrorMargins { get; set; }

		/// <summary>
		/// Gets or sets an icon.
		/// </summary>
		[StiSerializable]
		[DefaultValue(null)]
		[StiCategory("Design")]
		[StiOrder(StiPropertyOrder.DesignIcon)]
		[ParenthesizePropertyName(true)]
		[Description("Gets or sets an icon.")]
		[Editor("Stimulsoft.Report.Components.Design.StiReportIconEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[TypeConverter(typeof(StiReportIconConverter))]
		public byte[] Icon { get; set; }

		private StiReport report;
		/// <summary>
		/// Gets or sets the report in which the page is located.
		/// </summary>
		[StiSerializable(StiSerializationVisibility.Class,
			 StiSerializeTypes.SerializeToCode |
			 StiSerializeTypes.SerializeToSaveLoad)]
		[Browsable(false)]
		[DefaultValue(null)]
		public override StiReport Report
		{
			get
			{
				return report;
			}
			set
			{
				report = value;
			}
		}

		/// <summary>
		/// Internal use only. Used in the serialization of the rendered report (document) for access to the report properties.
		/// </summary>
		internal StiDocument Document { get; set; }

		[Browsable(false)]
		public virtual StiUnit Unit => report == null ? StiUnit.HundredthsOfInch : report.Unit;

		/// <summary>
		/// Used for save/load page. Internal use only.
		/// </summary>
		[Browsable(false)]
		[StiSerializable(StiSerializationVisibility.Class, StiSerializeTypes.SerializeToDesigner)]
		[DefaultValue(null)]
		public StiUnit ReportUnit { get; set; }

		[Browsable(false)]
		public bool LockHighlight { get; set; }

		[Browsable(false)]
		public bool DrillDownActivated { get; set; }

		[Browsable(false)]
		public bool IsForm => this is IStiForm;

		[Browsable(false)]
		public bool IsScreen => this is IStiScreen;

		[Browsable(false)]
		public bool IsDashboard => this is IStiDashboard;

		[Browsable(false)]
		public bool IsFormContainer => this is StiFormContainer;

		[Browsable(false)]
		public bool IsDashboardOrScreen => IsDashboard || IsScreen;

		[Browsable(false)]
		public bool IsPage => !IsForm && !IsScreen && !IsDashboard && !IsFormContainer;
		#endregion

		#region Properties.CacheGuid
		private string cacheGuid;
		/// <summary>
		/// Gets or sets a cache guid of page.
		/// </summary>
		[Browsable(false)]
		[DefaultValue(null)]
		public string CacheGuid
		{
			get
			{
				if (cacheGuid == null)
					NewCacheGuid();

				return cacheGuid;
			}
			set
			{
				cacheGuid = value;
			}
		}

		public void NewCacheGuid()
		{
			cacheGuid = global::System.Guid.NewGuid().ToString().Replace("-", "");
		}
		#endregion

		#region Methods.Load
		/// <summary>
		/// Loads a page template from the stream using the provider.
		/// </summary>
		/// <param name="service">The provider which will load a page.</param>
		/// <param name="stream">The stream for loading a page.</param>
		public void Load(StiPageSLService service, Stream stream)
		{
			service.Load(this, stream);
		}

		/// <summary>
		/// Loads a page template from the file using the provider.
		/// </summary>
		/// <param name="service">The provider which will load a page.</param>
		/// <param name="path">The file for loading a page.</param>
		public void Load(StiPageSLService service, string path)
		{
			using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				service.Load(this, stream);
				stream.Close();
			}
		}

		/// <summary>
		/// Loads a page template into the stream in the format XML.
		/// </summary>
		/// <param name="stream">The stream for loading a page.</param>
		public void Load(Stream stream)
		{
			var service = new StiXmlPageSLService();
			Load(service, stream);
		}

		/// <summary>
		/// Loads a page template from the stream in the format XML.
		/// </summary>
		/// <param name="path">A file that contains a page.</param>
		public void Load(string path)
		{
			using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				this.Load(stream);
				stream.Close();
			}
		}
		#endregion

		#region Methods.Save
		/// <summary>
		/// Saves a page template into the stream using the provider.
		/// </summary>
		/// <param name="service">The provider which will save a page.</param>
		/// <param name="stream">The stream for saving a page.</param>
		public void Save(StiPageSLService service, Stream stream)
		{
			service.Save(this, stream);
		}

		/// <summary>
		/// Saves a page template into the file using the provider.
		/// </summary>
		/// <param name="service">The provider which will save a page.</param>
		/// <param name="path">The file for saving a page.</param>
		public void Save(StiPageSLService service, string path)
		{
			StiFileUtils.ProcessReadOnly(path);
			using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
			{
				service.Save(this, stream);
				stream.Close();
			}
		}

		/// <summary>
		/// Saves a page template into the stream in the format XML.
		/// </summary>
		/// <param name="stream">The stream for saving a page.</param>
		public void Save(Stream stream)
		{
			var service = new StiXmlPageSLService();
			Save(service, stream);
		}

		/// <summary>
		/// Saves a page template in the file in the format XML.
		/// </summary>
		/// <param name="path">The file for saving a page.</param>
		public void Save(string path)
		{
			StiFileUtils.ProcessReadOnly(path);
			using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
			{
				Save(stream);
				stream.Close();
			}
		}
		#endregion

		#region Methods.LoadDocument
		/// <summary>
		/// Loads a rendered page from the stream using the provider.
		/// </summary>
		/// <param name="service">The provider which will load a page.</param>
		/// <param name="stream">The stream for loading a page.</param>
		public void LoadDocument(StiDocumentPageSLService service, Stream stream)
		{
			service.Load(this, stream);
		}

		/// <summary>
		/// Loads a rendered page from the file using the provider.
		/// </summary>
		/// <param name="service">The provider which will load a page.</param>
		/// <param name="path">The file for loading a page.</param>
		public void LoadDocument(StiDocumentPageSLService service, string path)
		{
			using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				service.Load(this, stream);
				stream.Close();
			}
		}

		/// <summary>
		/// Loads a rendered page from the stream in the XML format.
		/// </summary>
		/// <param name="stream">The stream for loading a page.</param>
		public void LoadDocument(Stream stream)
		{
			var service = new StiXmlDocumentPageSLService();
			LoadDocument(service, stream);
		}

		/// <summary>
		/// Loads a rendered page from the packed stream in the XML format.
		/// </summary>
		/// <param name="stream">The stream for loading a page.</param>
		public void LoadPackedDocument(Stream stream)
		{
			using (var unpackedStream = StiGZipHelper.Unpack(stream))
			{
				var service = new StiXmlDocumentPageSLService();
				LoadDocument(service, unpackedStream);
			}
		}

		/// <summary>
		/// Loads a rendered page from the stream in the XML format.
		/// </summary>
		/// <param name="path">A file that contains a page.</param>
		public void LoadDocument(string path)
		{
			using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				this.LoadDocument(stream);
				stream.Close();
			}
		}
		#endregion

		#region Methods.SaveDocument
		/// <summary>
		/// Saves a rendered page into the stream using the specified provider.
		/// </summary>
		/// <param name="service">The provider which will save a page.</param>
		/// <param name="stream">The stream for saving a page.</param>
		public void SaveDocument(StiDocumentPageSLService service, Stream stream)
		{
			service.Save(this, stream);
		}

		/// <summary>
		/// Saves a rendered page into the file using the provider.
		/// </summary>
		/// <param name="service">The provider which will save a page.</param>
		/// <param name="path">The file for saving a page.</param>
		public void SaveDocument(StiDocumentPageSLService service, string path)
		{
			StiFileUtils.ProcessReadOnly(path);
			using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
			{
				service.Save(this, stream);
				stream.Close();
			}
		}

		/// <summary>
		/// Saves a rendered page into the stream in the XML format.
		/// </summary>
		/// <param name="stream">The stream for saving a page.</param>
		public void SaveDocument(Stream stream)
		{
			var service = new StiXmlDocumentPageSLService();
			SaveDocument(service, stream);
		}

		/// <summary>
		/// Saves a rendered page into the packed stream in the XML format.
		/// </summary>
		/// <param name="stream">The stream for saving a page.</param>
		public void SavePackedDocument(Stream stream)
		{
			using (var packedStream = StiGZipHelper.Pack(stream))
			{
				var service = new StiXmlDocumentPageSLService();
				SaveDocument(service, packedStream);
			}
		}

		/// <summary>
		/// Saves a rendered page in the file in the XML format.
		/// </summary>
		/// <param name="path">The file for saving a page.</param>
		public void SaveDocument(string path)
		{
			StiFileUtils.ProcessReadOnly(path);
			using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
			{
				SaveDocument(stream);
				stream.Close();
			}
		}
		#endregion

		#region Methods.Design
		/// <summary>
		/// Runs the designer for one page.
		/// </summary>
		public DialogResult DesignPage()
		{
			return DesignPage(StiGuiMode.Gdi);
		}

		/// <summary>
		/// Runs the designer for one page with using WPF technology.
		/// </summary>
		public DialogResult DesignPageWithWpf()
		{
			return DesignPage(StiGuiMode.Wpf);
		}

		/// <summary>
		/// Runs the designer for one page.
		/// </summary>
		private DialogResult DesignPage(StiGuiMode guiMode)
		{
			var renderedPage = false;
			var pageIndex = Report.Pages.IndexOf(this);
			if (pageIndex == -1)
			{
				renderedPage = true;
				pageIndex = Report.RenderedPages.IndexOf(this);
			}

			var tmpPage = this.Clone() as StiPage;
			var oldReport = this.Report;
			var tmpReport = StiReport.GetReport();
			tmpReport.CalculationMode = StiCalculationMode.Interpretation;
			tmpReport.Unit = oldReport.Unit;

			foreach (StiBaseStyle style in this.report.Styles)
			{
				tmpReport.Styles.Add((StiBaseStyle)style.Clone());
			}

			tmpReport.Pages.Clear();
			tmpReport.Pages.AddV2Internal(tmpPage);
			tmpPage.Report = tmpReport;

			#region Process Components in report
			var components = tmpPage.GetComponents();
			foreach (StiComponent component in components)
			{
				component.Page = tmpPage;
				component.Report = tmpReport;

				if (component.PointerValue != null)
				{
					component.Pointer = new StiPointerExpression();
					component.Pointer.Value = component.PointerValue.ToString();
				}

				if (component.BookmarkValue != null)
				{
					component.Bookmark = new StiBookmarkExpression();
					component.Bookmark.Value = component.BookmarkValue.ToString();
				}

				if (component.ToolTipValue != null)
				{
					component.ToolTip = new StiToolTipExpression();
					component.ToolTip.Value = component.ToolTipValue.ToString();
				}

				if (component.HyperlinkValue != null)
				{
					component.Hyperlink = new StiHyperlinkExpression();
					component.Hyperlink.Value = component.HyperlinkValue.ToString();
				}

				if (component.TagValue != null)
				{
					component.Tag = new StiTagExpression();
					component.Tag.Value = component.TagValue.ToString();
				}

				if (component.Interaction == null)
					component.Interaction = new StiInteraction();

				#region Image
				var image = component as StiImage;
				if (image != null)
				{
					if (image.ExistImageToDraw())
						image.PutImage(image.TakeImageToDraw());
					else
						image.PutImageToDraw(image.TakeImage());
				}
				#endregion

				#region CheckBox
				var checkBox = component as StiCheckBox;
				if (checkBox != null)
				{
					checkBox.Checked = new StiCheckedExpression();
					if (checkBox.CheckedValue != null)
						checkBox.Checked.Value = checkBox.CheckedValue.ToString();
				}
				#endregion

				#region BarCode
				var barCode = component as StiBarCode;
				if (barCode != null)
				{
					barCode.Code = new StiBarCodeExpression();
					barCode.Code.Value = barCode.CodeValue;
				}
				#endregion

				#region RichText
				var richText = component as StiRichText;
				if (richText != null)
					richText.DataColumn = string.Empty;
				#endregion
			}
			#endregion

			tmpPage.ExcelSheet.Value = tmpPage.ExcelSheetValue;

			tmpReport.IsPageDesigner = true;

			var resShowEventsTab = StiOptions.Designer.ShowEventsTab;
			var resCodeTabVisible = StiOptions.Designer.CodeTabVisible;
			var resPreviewReportVisible = StiOptions.Designer.PreviewReportVisible;
			var resWpfPreviewReportVisible = StiOptions.Designer.WpfPreviewReportVisible;

			StiOptions.Designer.ShowEventsTab = false;
			StiOptions.Designer.PreviewReportVisible = false;
			StiOptions.Designer.WpfPreviewReportVisible = false;
			StiOptions.Designer.CodeTabVisible = false;

			DialogResult result;
			if (guiMode == StiGuiMode.Gdi)
				result = tmpReport.Design();
			else
			{
				if (StiOptions.Configuration.IsWPFV2)
					result = tmpReport.DesignV2WithWpf() ? DialogResult.Yes : DialogResult.No;
				else
					result = tmpReport.DesignWithWpf() ? DialogResult.Yes : DialogResult.No;
			}

			if (result == DialogResult.Yes)
			{
				tmpPage.ExcelSheetValue = tmpPage.ExcelSheet.Value;

				if (renderedPage)
					oldReport.RenderedPages[pageIndex] = tmpPage;
				else
					oldReport.Pages[pageIndex] = tmpPage;

				components = tmpPage.GetComponents();
				foreach (StiComponent component in components)
				{
					component.Report = oldReport;

					if (component.Pointer != null && component.Pointer.Value.Length != 0)
						component.PointerValue = component.Pointer.Value;
					else
						component.PointerValue = null;

					if (component.Bookmark != null && component.Bookmark.Value.Length != 0)
						component.BookmarkValue = component.Bookmark.Value;
					else
						component.BookmarkValue = null;

					if (component.ToolTip != null && component.ToolTip.Value.Length != 0)
						component.ToolTipValue = component.ToolTip.Value;
					else
						component.ToolTipValue = null;

					if (component.Hyperlink != null && component.Hyperlink.Value.Length != 0)
						component.HyperlinkValue = component.Hyperlink.Value;
					else
						component.HyperlinkValue = null;

					if (component.Tag != null && component.Tag.Value.Length != 0)
						component.TagValue = component.Tag.Value;
					else
						component.TagValue = null;

					if (component.Interaction != null && component.Interaction.IsDefault)
						component.Interaction = null;

					#region CheckBox
					var checkBox = component as StiCheckBox;
					if (checkBox != null)
						checkBox.CheckedValue = checkBox.Checked.Value;
					#endregion

					#region BarCode
					var barCode = component as StiBarCode;
					if (barCode != null)
						barCode.CodeValue = barCode.Code.Value;
					#endregion
				}

				tmpPage.Report = oldReport;
				tmpPage.ClearPage();
			}

			StiOptions.Designer.ShowEventsTab = resShowEventsTab;
			StiOptions.Designer.CodeTabVisible = resCodeTabVisible;
			StiOptions.Designer.PreviewReportVisible = resPreviewReportVisible;
			StiOptions.Designer.WpfPreviewReportVisible = resWpfPreviewReportVisible;

			return result;
		}
		#endregion

		#region Methods.This
		private bool GetIsPageTotalDataBand(string name)
		{
			if (report == null || report.DataBandsUsedInPageTotals == null)
				return false;

			foreach (string str in report.DataBandsUsedInPageTotals)
			{
				if (str == name || $"Breaked_{str}" == name || $"Continued_{str}" == name)
					return true;
			}
			return false;
		}

		internal void ClearPage()
		{
			var cache = new Hashtable();
			var resultTrue = new object();
			var resultFalse = new object();

			this.Components.SetParent(this);

			if (this.Height == 100000000000 && StiOptions.Engine.RenderExternalSubReportsWithHelpOfUnlimitedHeightPages) return;

			this.RemoveNewPageContainers(this);

			this.MoveComponentsToPage();

			var tempComps = new StiComponentsCollection(this);

			var index = 0;
			while (index < this.Components.Count)
			{
				var comp = this.Components[index];
				var container = comp as StiContainer;

				var isPageTotalDataBand = false;
				if (container != null)
				{
					if (cache[container.Name] == null)
					{
						isPageTotalDataBand = GetIsPageTotalDataBand(container.Name);
						if (isPageTotalDataBand)
							cache[container.Name] = resultTrue;
						else
							cache[container.Name] = resultFalse;
					}
					else if (cache[container.Name] == resultTrue)
						isPageTotalDataBand = true;
				}

				var needRemove =
					!isPageTotalDataBand &&
					container != null &&
					container.TagValue == null &&
					container.PointerValue == null &&
					container.BookmarkValue == null &&
					container.ToolTipValue == null &&
					container.HyperlinkValue == null &&
					container.Guid == null &&
					(container.Interaction == null || container.Interaction.IsDefault) &&
					(container.Border == null ||
					container.Border.Side == StiBorderSides.None && container.Border.DropShadow == false) &&
					(container.Brush == null ||
					container.Brush is StiSolidBrush &&
					((StiSolidBrush)container.Brush).Color == Color.Transparent);

				if (!needRemove)
				{
					tempComps.Add(comp);
				}
				index++;
			}
			this.Components.Clear();
			this.Components.AddRange(tempComps);
			tempComps.Clear();
		}

		/// <summary>
		/// Dispose all images and clear components list. 
		/// Used in ReportCacheMode.
		/// </summary>
		internal void DisposeImagesAndClearComponents()
		{
			if (StiOptions.Engine.ReportCache.DisposeImagesOnPageClear)
			{
				foreach (StiComponent comp in this.Components)
				{
					var image = comp as StiImage;
					if (image != null && image.imageBytesToDraw != null)
						image.imageBytesToDraw = null;
				}
			}
			this.Components.Clear();
		}

		private void RemoveNewPageContainers(StiContainer cont)
		{
			var found = false;
			for (var index = cont.Components.Count - 1; index >= 0; index--)
			{
				var comp = cont.Components[index];
				if (comp is StiNewPageContainer)
				{
					cont.Components.RemoveAt(index);
					found = true;
				}
				else
				{
					var cont2 = comp as StiContainer;
					if (cont2 != null)
					{
						RemoveNewPageContainers(cont2);
					}
				}
			}
			if (found)
			{
				if (cont.CanGrow) cont.CanShrink = true;
				StiContainerHelper.CheckSize(cont);
			}
		}

		private int GetComponentsCount(StiContainer container)
		{
			var count = container.Components.Count;
			foreach (StiComponent comp in container.Components)
			{
				var cont = comp as StiContainer;
				if (comp == container) continue;
				if (cont != null) count += GetComponentsCount(cont);
			}
			return count;
		}

		public override int GetComponentsCount()
		{
			return GetComponentsCount(this);
		}

		public void ResizePage(double factorX, double factorY)
		{
			ResizePage(factorX, factorY, true);
		}

		public void ResizePage(double factorX, double factorY, bool allowPageMarginsRescaling)
		{
			#region Change page margins
			if (allowPageMarginsRescaling)
			{
				Margins = new StiMargins(
					(double)Math.Round((decimal)(Margins.Left * factorX), 2),
					(double)Math.Round((decimal)(Margins.Right * factorX), 2),
					(double)Math.Round((decimal)(Margins.Top * factorY), 2),
					(double)Math.Round((decimal)(Margins.Bottom * factorY), 2));
			}
			#endregion

			#region Change ColumnGaps and ColumnWidth
			this.ColumnWidth *= factorX;
			this.ColumnGaps *= factorX;
			#endregion

			if (Conditions != null)
			{
				foreach (StiCondition condition in Conditions)
				{
					condition.Font = new Font(condition.Font.Name, condition.Font.Size * (float)factorX, condition.Font.Style);
				}
			}

			#region Foreach all components on each page
			var comps = GetComponents();
			foreach (StiComponent comp in comps)
			{
				if (comp is StiLinePrimitive)
				{
					((StiLinePrimitive)comp).Size *= (float)factorX;
				}

				#region Change component location
				comp.Left *= factorX;
				comp.Top *= factorY;
				comp.Width *= factorX;
				comp.Height *= factorY;
				#endregion

				#region Change font size, if need
				var font = comp as IStiFont;
				if (font != null)
					font.Font = new Font(font.Font.Name, font.Font.Size * (float)factorX, font.Font.Style);
				#endregion

				#region DataBand ColumnGaps and ColumnWidth
				var dataBand = comp as StiDataBand;
				if (dataBand != null)
				{
					dataBand.ColumnWidth *= factorX;
					dataBand.ColumnGaps *= factorX;
				}
				#endregion

				#region Panel ColumnGaps and ColumnWidth
				var panel = comp as StiPanel;
				if (panel != null)
				{
					panel.ColumnWidth *= factorX;
					panel.ColumnGaps *= factorX;
				}
				#endregion

				#region Conditions
				if (comp.Conditions != null)
				{
					foreach (StiCondition condition in comp.Conditions)
					{
						condition.Font = new Font(condition.Font.Name, condition.Font.Size * (float)factorX, condition.Font.Style);
					}
				}
				#endregion

				#region Borders
				var border = comp as IStiBorder;
				if (border != null)
				{
					if (border is StiAdvancedBorder)
					{
						border.Border = border.Border.Clone() as StiBorder;

						((StiAdvancedBorder)border.Border).LeftSide.Size *= factorX;
						((StiAdvancedBorder)border.Border).RightSide.Size *= factorX;
						((StiAdvancedBorder)border.Border).BottomSide.Size *= factorY;
						((StiAdvancedBorder)border.Border).TopSide.Size *= factorY;
					}
					else
					{
						border.Border = border.Border.Clone() as StiBorder;
						border.Border.Size *= factorX;
					}
				}
				#endregion
			}
			#endregion
		}

		/// <summary>
		/// Returns a container of services which control of pages.
		/// </summary>
		/// <returns>Contaner of services.</returns>
		public static StiServiceContainer GetPageServices()
		{
			var pages = new StiServiceContainer();

			foreach (var page in StiOptions.Services.Components.Where(s => s.ServiceEnabled))
			{
				if (page is StiPage)
					pages.Add(page);
			}

			return pages;
		}
		#endregion

		#region Methods.override
		public override StiComponent CreateNew()
		{
			return StiActivator.CreateObject("Stimulsoft.Report.Components.StiPage") as StiPage;
		}
		#endregion

		/// <summary>
		/// Creates a new component of the type StiPage.
		/// </summary>
		public StiPage() : this(null)
		{
		}

		/// <summary>
		/// Creates a new component of the type StiPage.
		/// </summary>
		/// <param name="report">The report in which a page will be added.</param>
		public StiPage(StiReport report) : base(new RectangleD(0, 0, 0, 0))
		{
			try
			{
				this.NewGuid();
				Border.Size = 2;
				Reset();
				Parent = null;

				#region Paper Size
				this.PaperSize = StiOptions.Engine.DefaultPaperSize;

				PaperSize size = null;

				if (this.PaperSize != PaperKind.Custom)
					size = StiPageHelper.GetPaperSizeFromPaperKind(this.PaperSize);

				if (size == null || this.PaperSize == PaperKind.Custom)
				{
					pageWidth = 827;
					pageHeight = 1169;
				}
				else
				{
					var pageSize = StiPageHelper.GetPaperSize(this, size);
					this.PageWidth = pageSize.Width;
					this.PageHeight = pageSize.Height;
				}
				#endregion

				this.report = report;

				segmentPerWidth = 1;
				segmentPerHeight = 1;

				Margins = new StiMargins(39, 39, 39, 39);

				if (report != null && !report.IsSerializing)
					this.Convert(StiUnit.HundredthsOfInch, Unit);
			}
			catch
			{
			}

			PlaceOnToolbox = false;
		}
	}
}