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
using System.Drawing.Design;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Units;
using Stimulsoft.Report.Components.Design;
using Stimulsoft.Report.Events;
using Stimulsoft.Base;
using Stimulsoft.Base.Services;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Design;
using Stimulsoft.Report.Painters;
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using Stimulsoft.Report.Helpers;
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
#endif

namespace Stimulsoft.Report.Dialogs
{
	/// <summary>
	/// Represents a dialog box.
	/// </summary>
	[StiServiceBitmap(typeof(StiForm), "Stimulsoft.Report.Dialogs.Bmp.StiForm.gif")]
	[StiServiceCategoryBitmap(typeof(StiForm), "Stimulsoft.Report.Dialogs.Bmp.StiReportControl.gif")]
	[StiDesigner("Stimulsoft.Report.Dialogs.Design.StiFormDesigner, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfDesigner("Stimulsoft.Report.WpfDesign.StiWpfFormDesigner, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
    [StiGdiPainter(typeof(StiFormGdiPainter))]
	[StiToolbox(false)]
	public class StiForm : 
		StiPage, 
		IStiForm
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // Old
            jObject.RemoveProperty("Margins");
            jObject.RemoveProperty("PrintHeadersFootersFromPreviousPage");
            jObject.RemoveProperty("PrintOnPreviousPage");
            jObject.RemoveProperty("StopBeforePrint");
            jObject.RemoveProperty("LargeHeight");
            jObject.RemoveProperty("LargeHeightFactor");
            jObject.RemoveProperty("ResetPageNumber");
            jObject.RemoveProperty("Watermark");
            jObject.RemoveProperty("ExcelSheet");
            jObject.RemoveProperty("CanBreak");
            jObject.RemoveProperty("Interaction");
            jObject.RemoveProperty("PaperSourceFirstPage");
            jObject.RemoveProperty("PaperSourceOfFirstPage");
            jObject.RemoveProperty("PaperSourceOtherPages");
            jObject.RemoveProperty("PaperSourceOfOtherPages");
            jObject.RemoveProperty("UseParentStyles");
            jObject.RemoveProperty("Conditions");
            jObject.RemoveProperty("PaperSize");
            jObject.RemoveProperty("NumberOfCopies");
            jObject.RemoveProperty("PrintOn");
            jObject.RemoveProperty("Border");
            jObject.RemoveProperty("UnlimitedBreakable");
            jObject.RemoveProperty("Skip");
            jObject.RemoveProperty("StretchToPrintArea");
            jObject.RemoveProperty("TitleBeforeHeader");
            jObject.RemoveProperty("UnlimitedHeight");
            jObject.RemoveProperty("UnlimitedWidth");
            jObject.RemoveProperty("Orientation");
            jObject.RemoveProperty("PageWidth");
            jObject.RemoveProperty("PageHeight");
            jObject.RemoveProperty("SegmentPerWidth");
            jObject.RemoveProperty("SegmentPerHeight");
            jObject.RemoveProperty("ColumnGaps");
            jObject.RemoveProperty("ColumnWidth");
            jObject.RemoveProperty("Columns");
            jObject.RemoveProperty("CanShrink");
            jObject.RemoveProperty("CanGrow");
            jObject.RemoveProperty("Printable");
            jObject.RemoveProperty("Pointer");
            jObject.RemoveProperty("Bookmark");
            jObject.RemoveProperty("ToolTip");
            jObject.RemoveProperty("Hyperlink");
            jObject.RemoveProperty("Enabled");
            jObject.RemoveProperty("BeginRenderEvent");
            jObject.RemoveProperty("RenderingEvent");
            jObject.RemoveProperty("EndRenderEvent");
            jObject.RemoveProperty("GetToolTipEvent");
            jObject.RemoveProperty("GetPointerEvent");
            jObject.RemoveProperty("GetBookmarkEvent");
            jObject.RemoveProperty("GetHyperlinkEvent");
            jObject.RemoveProperty("BeforePrintEvent");
            jObject.RemoveProperty("AfterPrintEvent");
            jObject.RemoveProperty("MouseEnterEvent");
            jObject.RemoveProperty("MouseLeaveEvent");
            jObject.RemoveProperty("ColumnBeginRenderEvent");
            jObject.RemoveProperty("ColumnEndRenderEvent");
            jObject.RemoveProperty("GetExcelSheetEvent");
            jObject.RemoveProperty("GetDrillDownReportEvent");

            // StiForm
            jObject.AddPropertyJObject("ClosedFormEvent", ClosedFormEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("ClosingFormEvent", ClosingFormEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("LoadFormEvent", LoadFormEvent.SaveToJsonObject(mode));
            jObject.AddPropertySize("Size", Size);
            jObject.AddPropertyPoint("Location", Location);
            jObject.AddPropertyString("Text", Text);
            jObject.AddPropertyFontMicrosoftSansSerif8("Font", Font);
            jObject.AddPropertyColor("BackColor", BackColor, SystemColors.Control);
            jObject.AddPropertyEnum("StartPosition", StartPosition, FormStartPosition.CenterScreen);
            jObject.AddPropertyEnum("WindowState", WindowState, FormWindowState.Normal);
            jObject.AddPropertyEnum("StartMode", StartMode, StiFormStartMode.OnStart);
            jObject.AddPropertyBool("Visible", Visible, true);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "ClosedFormEvent":
                        this.ClosedFormEvent.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "ClosingFormEvent":
                        this.ClosingFormEvent.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "LoadFormEvent":
                        this.LoadFormEvent.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "Size":
                        this.Size = property.DeserializeSize();
                        break;

                    case "Location":
                        this.location = property.DeserializePoint();
                        break;

                    case "Text":
                        this.text = property.DeserializeString();
                        break;

                    case "Font":
                        this.font = property.DeserializeFont(this.font);
                        break;

                    case "BackColor":
                        this.backColor = property.DeserializeColor();
                        break;

                    case "StartPosition":
                        this.startPosition = property.DeserializeEnum<FormStartPosition>();
                        break;

                    case "WindowState":
                        this.windowState = property.DeserializeEnum<FormWindowState>(); 
                        break;

                    case "StartMode":
                        this.startMode = property.DeserializeEnum<StiFormStartMode>(); 
                        break;

                    case "Visible":
                        this.visible = property.DeserializeBool();
                        break;
                }
            }
        }
        #endregion

		#region StiService override

		/// <summary>
		/// Gets a service category.
		/// </summary>
		[Browsable(false)]
		public override string ServiceCategory
		{
			get
			{
				return StiLocalization.Get("Report", "Dialogs");
			}
		}
		#endregion

		#region IStiGetFonts
		public override List<StiFont> GetFonts()
		{
			var result = base.GetFonts();
			result.Add(new StiFont(Font));
			return result.Distinct().ToList();
		}
		#endregion

		#region ICloneable override
		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public override object Clone()
		{
            return base.Clone();
		}

		#endregion

		#region StiComponent override
        public override StiComponentId ComponentId
        {
            get
            {
                return StiComponentId.StiForm;
            }
        }

		/// <summary>
		/// Return events collection of this component;
		/// </summary>
		public override StiEventsCollection GetEvents()
		{
			StiEventsCollection events = new StiEventsCollection();
			if (ClickEvent != null)events.Add(ClickEvent);
			if (GetTagEvent != null)events.Add(GetTagEvent);
			if (ClosedFormEvent != null)events.Add(ClosedFormEvent);
			if (ClosingFormEvent != null)events.Add(ClosingFormEvent);
			if (LoadFormEvent != null)events.Add(LoadFormEvent);
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
				return StiLocalization.Get("Dialogs", "StiForm");
			}
		}
		#endregion

		#region StiPage override
		[Browsable(false)]
		[StiNonSerialized]
		public sealed override StiMargins Margins
		{
			get
			{
				return new StiMargins(4, 4, 20, 4);
			}
			set
			{
			}
		}

        [Browsable(false)]
        [StiNonSerialized]
        public sealed override bool MirrorMargins
        {
            get
            {
                return base.MirrorMargins;
            }
            set
            {
            }
        }

        [Browsable(false)]
		[StiNonSerialized]
		public sealed override bool PrintHeadersFootersFromPreviousPage
		{
			get
			{
				return base.PrintHeadersFootersFromPreviousPage;
			}
			set
			{
			}
		}
		

		[Browsable(false)]
		[StiNonSerialized]
		public sealed override bool PrintOnPreviousPage
		{
			get
			{
				return base.PrintOnPreviousPage;
			}
			set
			{
			}
		}


		[Browsable(false)]
		[StiNonSerialized]
		public sealed override int StopBeforePrint
		{
			get
			{
				return base.StopBeforePrint;
			}
			set
			{
			}
		}


		[Browsable(false)]
		[StiNonSerialized]
		public sealed override bool LargeHeight
		{
			get
			{
				return base.LargeHeight;
			}
			set
			{
			}
		}


		[Browsable(false)]
		[StiNonSerialized]
		public sealed override int LargeHeightFactor
		{
			get
			{
				return base.LargeHeightFactor;
			}
			set
			{
			}
		}


		[Browsable(false)]
		[StiNonSerialized]
		public sealed override bool ResetPageNumber
		{
			get
			{
				return base.ResetPageNumber;
			}
			set
			{
			}
		}


		[Browsable(false)]
		[StiNonSerialized]
		public sealed override StiWatermark Watermark
		{
			get
			{
				return base.Watermark;
			}
			set
			{
			}
		}


		[Browsable(false)]
		[StiNonSerialized]
		public sealed override StiExcelSheetExpression ExcelSheet
		{
			get
			{
				return base.ExcelSheet;
			}
			set
			{
			}
		}

		

		[Browsable(false)]
		public override double Zoom
		{
			get
			{
				return 1;
			}
		}


		[Browsable(false)]
		public override double GridSize
		{
			get
			{
				return Report.Info.GridSizePixels;
			}
		}


		[Browsable(false)]
		public override StiUnit Unit
		{
			get
			{
				return StiUnit.HundredthsOfInch;
			}
		}
		#endregion

		#region Off
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

		[StiNonSerialized]
		[Browsable(false)]
		public override StiInteraction Interaction
		{
			get
			{
				return base.Interaction;
			}
			set
			{
				base.Interaction = value;
			}
		}

        [StiNonSerialized]
        [Browsable(false)]
        public override PaperSourceKind PaperSourceFirstPage
        {
            get
            {
                return base.PaperSourceFirstPage;
            }
            set
            {
                base.PaperSourceFirstPage = value;
            }
        }

		[StiNonSerialized]
		[Browsable(false)]
		public override string PaperSourceOfFirstPage
		{
			get
			{
				return base.PaperSourceOfFirstPage;
			}
			set
			{
                base.PaperSourceOfFirstPage = value;
			}
		}


        [StiNonSerialized]
        [Browsable(false)]
        public override PaperSourceKind PaperSourceOtherPages
        {
            get
            {
                return base.PaperSourceOtherPages;
            }
            set
            {
                base.PaperSourceOtherPages = value;
            }
        }

		[StiNonSerialized]
		[Browsable(false)]
		public override string PaperSourceOfOtherPages
		{
			get
			{
				return base.PaperSourceOfOtherPages;
			}
			set
			{
                base.PaperSourceOfOtherPages = value;
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
				{
					base.UseParentStyles = value;
				}
			}
		}

		[Browsable(false)]
		[StiNonSerialized]
		public sealed override StiConditionsCollection Conditions
		{
			get
			{
				return base.Conditions;
			}
			set
			{
				base.Conditions = value;
			}
		}

        [StiSerializable]
        [DefaultValue("")]
        [StiCategory("Appearance")]
        [Description("Gets or sets a style of a component.")]
        [Editor("Stimulsoft.Report.Design.StiDialogStyleEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Basic)]
        [StiOrder(StiPropertyOrder.DialogComponentStyle)]
        public override string ComponentStyle
        {
            get
            {
                return base.ComponentStyle;
            }
            set
            {
                base.ComponentStyle = value;
            }
        }


		[Browsable(false)]
		[StiNonSerialized]
		public override PaperKind PaperSize
		{
			get
			{
				return base.PaperSize;
			}
			set
			{
				base.PaperSize = value;
			}
		}


		[Browsable(false)]
		[StiNonSerialized]
		public override int NumberOfCopies
		{
			get
			{
				return base.NumberOfCopies;
			}
			set
			{
				base.NumberOfCopies = value;
			}
		}

		#region IStiPrintOn Off
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

		#region IStiBorder Off
		[Browsable(false)]
		[StiNonSerialized]
		public override StiBorder Border
		{
			get 
			{
				return base.Border;
			}
			set 
			{
				base.Border = value;
			}
		}
		#endregion

		#region IStiBrush Off
		[Browsable(false)]
		[StiNonSerialized]
		public override StiBrush Brush
		{
			get 
			{
				return base.Brush;
			}
			set 
			{
				base.Brush = value;
			}
		}
		#endregion

		#region IStiUnitConvert override Off
        public override void Convert(StiUnit oldUnit, StiUnit newUnit, bool isReportSnapshot = false)
		{
		}
		#endregion

		#region StiPage Off
		[Browsable(false)]
		[StiNonSerialized]
		public sealed override bool UnlimitedBreakable
		{
			get
			{
				return base.UnlimitedBreakable;
			}
			set
			{
			}
		}


		[Browsable(false)]
		[StiNonSerialized]
		public sealed override bool Skip
		{
			get
			{
				return true;
			}
			set
			{
			}
		}


		[Browsable(false)]
		[StiNonSerialized]
		public sealed override bool StretchToPrintArea
		{
			get
			{
				return base.StretchToPrintArea;
			}
			set
			{
			}
		}


		[Browsable(false)]
		[StiNonSerialized]
		public sealed override bool TitleBeforeHeader
		{
			get
			{
				return base.TitleBeforeHeader;
			}
			set
			{
			}
		}


		[Browsable(false)]
		[StiNonSerialized]
		public sealed override bool UnlimitedHeight
		{
			get
			{
				return base.UnlimitedHeight;
			}
			set
			{
			}
		}


		[Browsable(false)]
		[StiNonSerialized]
		public sealed override bool UnlimitedWidth
		{
			get
			{
				return base.UnlimitedWidth;
			}
			set
			{
			}
		}

		
		[Browsable(false)]
		[StiNonSerialized]
		public sealed override StiPageOrientation Orientation
		{
			get
			{
				return base.Orientation;
			}
			set
			{
			}
		}



		[Browsable(false)]
		[StiNonSerialized]
		public sealed override double PageWidth
		{
			get 
			{
				return base.PageWidth;
			}
			set 
			{
				base.PageWidth = value;
			}
		}

		
		[Browsable(false)]
		[StiNonSerialized]
		public sealed override double PageHeight
		{
			get
			{
				return base.PageHeight;
			}
			set 
			{
				base.PageHeight = value;
			}
		}
		

		[Browsable(false)]
		[StiNonSerialized]
		public sealed override int SegmentPerWidth
		{
			get
			{
				return base.SegmentPerWidth;
			}
			set
			{
				base.SegmentPerWidth = value;
			}
		}

		
		[Browsable(false)]
		[StiNonSerialized]
		public sealed override int SegmentPerHeight
		{
			get
			{
				return base.SegmentPerHeight;
			}
			set
			{
				base.SegmentPerHeight = value;
			}
		}
		
		#endregion

		#region Columns Off
		[Browsable(false)]
		[StiNonSerialized]
		public sealed override double ColumnGaps
		{
			get
			{
				return base.ColumnGaps;
			}
			set
			{
				base.ColumnGaps = value;
			}
		}


		[Browsable(false)]
		[StiNonSerialized]
		public sealed override double ColumnWidth
		{
			get
			{
				return base.ColumnWidth;
			}
			set
			{
				base.ColumnWidth = value;
			}
		}


		[Browsable(false)]
		[StiNonSerialized]
		public sealed override int Columns
		{
			get
			{
				return base.Columns;
			}
			set
			{
				base.Columns = value;
			}
		}
		#endregion

		#region IStiCanShrink Off
		[Browsable(false)]
		[StiNonSerialized]
		public sealed override bool CanShrink
		{
			get
			{
				return base.CanShrink;
			}
			set
			{
				base.CanShrink = value;
			}
		}
		#endregion

		#region IStiCanGrow Off
		[Browsable(false)]
		[StiNonSerialized]
		public sealed override bool CanGrow
		{
			get
			{
				return base.CanGrow;
			}
			set
			{
				base.CanGrow = value;
			}
		}
		#endregion

		#region StiComponent Off
		[Browsable(false)]
		[StiNonSerialized]
		public sealed override bool Printable
		{
			get 
			{
				return base.Printable;
			}
			set 
			{
				base.Printable = value;
			}
		}

		[Browsable(false)]
		[StiNonSerialized]
		public sealed override StiPointerExpression Pointer
		{
			get
			{
				return base.Pointer;
			}
			set
			{
				base.Pointer = value;
			}
		}

		[Browsable(false)]
		[StiNonSerialized]
		public sealed override StiBookmarkExpression Bookmark
		{
			get
			{
				return base.Bookmark;
			}
			set
			{
				base.Bookmark = value;
			}
		}


		[Browsable(false)]
		[StiNonSerialized]
		public sealed override StiToolTipExpression ToolTip
		{
			get
			{
				return base.ToolTip;
			}
			set
			{
				base.ToolTip = value;
			}
		}

	
		[Browsable(false)]
		[StiNonSerialized]
		public sealed override StiHyperlinkExpression Hyperlink
		{
			get
			{
				return base.Hyperlink;
			}
			set
			{
				base.Hyperlink = value;
			}
		}


		[Browsable(false)]
		[StiNonSerialized]
		public sealed override bool Enabled
		{
			get
			{
				return base.Enabled;
			}
			set
			{
				base.Enabled = value;
			}
		}

		#endregion
		#endregion

		#region Events Off
		[StiEventHide]
		[StiNonSerialized]
		public sealed override StiBeginRenderEvent BeginRenderEvent
		{
			get
			{
				return base.BeginRenderEvent;
			}
			set
			{
				base.BeginRenderEvent = value;
			}
		}


		[StiEventHide]
		[StiNonSerialized]
		public sealed override StiRenderingEvent RenderingEvent
		{
			get
			{
				return base.RenderingEvent;
			}
			set
			{
				base.RenderingEvent = value;
			}
		}

		
		[StiEventHide]
		[StiNonSerialized]
		public sealed override StiEndRenderEvent EndRenderEvent
		{
			get
			{
				return base.EndRenderEvent;
			}
			set
			{
				base.EndRenderEvent = value;
			}
		}


		[StiEventHide]
		[StiNonSerialized]
		public sealed override StiGetToolTipEvent GetToolTipEvent
		{
			get
			{
				return base.GetToolTipEvent;
			}
			set
			{
				base.GetToolTipEvent = value;
			}
		}

		[StiEventHide]
		[StiNonSerialized]
		public sealed override StiGetPointerEvent GetPointerEvent
		{
			get
			{
				return base.GetPointerEvent;
			}
			set
			{
				base.GetPointerEvent = value;
			}
		}

		[StiEventHide]
		[StiNonSerialized]
		public sealed override StiGetBookmarkEvent GetBookmarkEvent
		{
			get
			{
				return base.GetBookmarkEvent;
			}
			set
			{
				base.GetBookmarkEvent = value;
			}
		}


		[StiEventHide]
		[StiNonSerialized]
		public sealed override StiGetHyperlinkEvent GetHyperlinkEvent
		{
			get
			{
				return base.GetHyperlinkEvent;
			}
			set
			{
				base.GetHyperlinkEvent = value;
			}
		}


		[StiEventHide]
		[StiNonSerialized]
		public sealed override StiBeforePrintEvent BeforePrintEvent
		{
			get
			{
				return base.BeforePrintEvent;
			}
			set
			{
				base.BeforePrintEvent = value;
			}
		}


		[StiEventHide]
		[StiNonSerialized]
		public sealed override StiAfterPrintEvent AfterPrintEvent
		{
			get
			{
				return base.AfterPrintEvent;
			}
			set
			{
				base.AfterPrintEvent = value;
			}
		}

		
		[StiEventHide]
		[StiNonSerialized]
		public sealed override StiMouseEnterEvent MouseEnterEvent
		{
			get
			{
				return base.MouseEnterEvent;
			}
			set
			{
				base.MouseEnterEvent = value;
			}
		}


		[StiEventHide]
		[StiNonSerialized]
		public sealed override StiMouseLeaveEvent MouseLeaveEvent
		{
			get
			{
				return base.MouseLeaveEvent;
			}
			set
			{
				base.MouseLeaveEvent = value;
			}
		}

		
		[StiEventHide]
		[StiNonSerialized]
		public sealed override StiColumnBeginRenderEvent ColumnBeginRenderEvent
		{
			get
			{
				return base.ColumnBeginRenderEvent;
			}
			set
			{
				base.ColumnBeginRenderEvent = value;
			}
		}


		[StiEventHide]
		[StiNonSerialized]
		public sealed override StiColumnEndRenderEvent ColumnEndRenderEvent
		{
			get
			{
				return base.ColumnEndRenderEvent;
			}
			set
			{
				base.ColumnEndRenderEvent = value;
			}
		}

        [StiEventHide]
        [StiNonSerialized]
        public sealed override StiGetExcelSheetEvent GetExcelSheetEvent
        {
            get
            {
                return base.GetExcelSheetEvent;
            }
            set
            {
                base.GetExcelSheetEvent = value;
            }
        }

		[StiEventHide]
		[StiNonSerialized]
		public sealed override StiGetDrillDownReportEvent GetDrillDownReportEvent
		{
			get
			{
				return base.GetDrillDownReportEvent;
			}
			set
			{
				base.GetDrillDownReportEvent = value;
			}
		}
		#endregion

		#region Events
		#region ClickEvent
		/// <summary>
		/// Gets or sets a script of the event Click.
		/// </summary>
		[StiSerializable]
		[StiCategory("MouseEvents")]
		[Browsable(false)]
		[Description("Gets or sets a script of the event Click.")]
		public override StiClickEvent ClickEvent
		{
			get
			{
				return base.ClickEvent;
			}
			set
			{
				base.ClickEvent = value;
			}
		}
		#endregion

		#region ReportControlUpdate
		public event StiReportControlUpdateEventHandler ReportControlUpdate;

		public void InvokeReportControlUpdate(object sender,  string propertyName)
		{
			InvokeReportControlUpdate(sender, new StiReportControlUpdateEventArgs(propertyName));
		}

		public void InvokeReportControlUpdate(object sender, StiReportControlUpdateEventArgs e)
		{
			if (this.ReportControlUpdate != null)this.ReportControlUpdate(sender, e);
		}

		#endregion

		#region FormClose
		private static readonly object EventFormClose = new object();

		public event EventHandler FormClose
		{
			add
			{
				base.Events.AddHandler(EventFormClose, value);
			}
			remove
			{
				base.Events.RemoveHandler(EventFormClose, value);
			}
		}

		/// <summary>
		/// Closes the form.
		/// </summary>
		public void Close()
		{
			EventHandler handler = base.Events[EventFormClose] as EventHandler;
			if (handler != null)handler(this, EventArgs.Empty);
		}

		#endregion

		#region GetTag
		[StiSerializable]
		[StiCategory("ControlsEvents")]
		[Browsable(false)]
		public override StiGetTagEvent GetTagEvent
		{
			get
			{
				return base.GetTagEvent;
			}
			set
			{
				base.GetTagEvent = value;
			}
		}
		#endregion

		#region OnClosedForm
		private static readonly object EventClosedForm = new object();

		/// <summary>
		/// Occurs when the form is closed.
		/// </summary>
		public event EventHandler ClosedForm
		{
			add
			{
				base.Events.AddHandler(EventClosedForm, value);
			}
			remove
			{
				base.Events.RemoveHandler(EventClosedForm, value);
			}
		}

		public void InvokeClosedForm(EventArgs e)
		{
			EventHandler handler = base.Events[EventClosedForm] as EventHandler;
			if (handler != null)handler(this, e);

			StiBlocklyHelper.InvokeBlockly(this.Report, this, ClosedFormEvent);
		}


		/// <summary>
		/// Gets or sets a script of the event ClosedForm.
		/// </summary>
		[StiSerializable]
		[StiCategory("ControlsEvents")]
		[Browsable(false)]
		[Description("Gets or sets a script of the event ClosedForm.")]
		public virtual StiClosedFormEvent ClosedFormEvent
		{
			get
			{				
				return new StiClosedFormEvent(this);
			}
			set
			{
				if (value != null)value.Set(this, value.Script);
			}
		}
		#endregion

		#region OnClosingForm
		private static readonly object EventClosingForm = new object();

		/// <summary>
		/// Occurs when the form is closing.
		/// </summary>
		public event CancelEventHandler ClosingForm
		{
			add
			{
				base.Events.AddHandler(EventClosingForm, value);
			}
			remove
			{
				base.Events.RemoveHandler(EventClosingForm, value);
			}
		}

		public void InvokeClosingForm(CancelEventArgs e)
		{
			CancelEventHandler handler = base.Events[EventClosingForm] as CancelEventHandler;
			if (handler != null)handler(this, e);

			StiBlocklyHelper.InvokeBlockly(this.Report, this, ClosingFormEvent);
		}

		/// <summary>
		/// Gets or sets a script of the event ClosingForm.
		/// </summary>
		[StiSerializable]
		[StiCategory("ControlsEvents")]
		[Browsable(false)]
		[Description("Gets or sets a script of the event ClosingForm.")]
		public virtual StiClosingFormEvent ClosingFormEvent
		{
			get
			{				
				return new StiClosingFormEvent(this);
			}
			set
			{
				if (value != null)value.Set(this, value.Script);
			}
		}
		#endregion

		#region OnLoadForm
		private static readonly object EventLoadForm = new object();

		/// <summary>
		/// Occurs before a form is displayed for the first time.
		/// </summary>
		public event EventHandler LoadForm
		{
			add
			{
				base.Events.AddHandler(EventLoadForm, value);
			}
			remove
			{
				base.Events.RemoveHandler(EventLoadForm, value);
			}
		}

		public void InvokeLoadForm(EventArgs e)
		{
			var handler = base.Events[EventLoadForm] as EventHandler;
			if (handler != null)handler(this, e);

			StiBlocklyHelper.InvokeBlockly(this.Report, this, LoadFormEvent);
		}

		/// <summary>
		/// Gets or sets a script of the event LoadForm.
		/// </summary>
		[StiSerializable]
		[StiCategory("ControlsEvents")]
		[Browsable(false)]
		[Description("Gets or sets a script of the event LoadForm.")]
		public virtual StiLoadFormEvent LoadFormEvent
		{
			get
			{				
				return new StiLoadFormEvent(this);
			}
			set
			{
				if (value != null)value.Set(this, value.Script);
			}
		}
		#endregion
		#endregion

		#region Controls Property
		private Form control = null;
		/// <summary>
		/// Gets ot sets Windows Forms Control for this Report Control.
		/// </summary>
		[Browsable(false)]
		public Form Control
		{
			get
			{
				return control;
			}
			set
			{
				control = value;
			}
		}

        private object controlWpf = null;
        /// <summary>
        /// Gets ot sets WPF control for this Report Control.
        /// </summary>
        [Browsable(false)]
        public object ControlWpf
        {
            get
            {
                return controlWpf;
            }
            set
            {
                controlWpf = value;
            }
        }


		[StiNonSerialized]
		[Browsable(false)]
		public new int Width
		{
			get
			{
				return Size.Width;
			}
			set
			{
				Size = new Size(value, Size.Height);
			}
		}

		
		[StiNonSerialized]
		[Browsable(false)]
		public new int Height
		{
			get
			{
				return Size.Height;
			}
			set
			{
				Size = new Size(Size.Width, value);
			}
		}

		
		[StiNonSerialized]
		[Browsable(false)]
		public new int Left
		{
			get
			{
				return Location.X;
			}
			set
			{
				Location = new Point(value, Location.Y);
			}
		}


		[StiNonSerialized]
		[Browsable(false)]
		public new int Top
		{
			get
			{
				return Location.Y;
			}
			set
			{
				Location = new Point(Location.X, value);
			}
		}

		
		/// <summary>
		/// Gets or sets the size of the form.
		/// </summary>
		[StiSerializable]
		[StiCategory("Behavior")]
		[Browsable(true)]
		[Description("Gets or sets the size of the form.")]
        [StiOrder(StiPropertyOrder.DialogSize)]
        [StiPropertyLevel(StiLevel.Basic)]
		public Size Size
		{
			get
			{
				return new Size((int)base.PageWidth, (int)base.PageHeight);
			}
			set
			{
				base.PageWidth = value.Width;
				base.PageHeight = value.Height;
				UpdateReportControl("Size");
			}
		}


		private Point location = new Point(0, 0);
		/// <summary>
		/// Gets or sets the coordinates of the upper-left corner of the form.
		/// </summary>
		[StiSerializable]
		[StiCategory("Behavior")]
		[Browsable(true)]
        [Description("Gets or sets the coordinates of the upper-left corner of the form.")]
        [StiOrder(StiPropertyOrder.DialogLocation)]
        [StiPropertyLevel(StiLevel.Basic)]
		public Point Location
		{
			get
			{
				return location;
			}
			set
			{
				location = value;
				UpdateReportControl("Location");
			}
		}


		private DialogResult dialogResult = DialogResult.None;
		/// <summary>
		/// Gets or sets the dialog result for the form.
		/// </summary>
		[Browsable(false)]
		[Description("Gets or sets the dialog result for the form.")]
        [StiOrder(StiPropertyOrder.DialogDialogResult)]
		public DialogResult DialogResult
		{
			get
			{
				return dialogResult;
			}
			set
			{
				dialogResult = value;
				UpdateReportControl("DialogResult");
			}
		}

	

		private RightToLeft rightToLeft = RightToLeft.No;
		/// <summary>
		/// Gets or sets a value indicating whether control's elements are aligned to support locales using right-to-left fonts.
		/// </summary>
		[StiSerializable]
		[StiCategory("Appearance")]
		[DefaultValue(RightToLeft.No)]
		[TypeConverter(typeof(Stimulsoft.Base.Localization.StiEnumConverter))]
		[Editor(StiEditors.Enum, typeof(UITypeEditor))]
		[Description("Gets or sets a value indicating whether control's elements are aligned to support locales using right-to-left fonts.")]
        [StiGuiMode(StiGuiMode.Gdi)]
        [StiOrder(StiPropertyOrder.DialogRightToLeft)]
        [StiPropertyLevel(StiLevel.Basic)]
		public new RightToLeft RightToLeft
		{
			get
			{
				return rightToLeft;
			}
			set
			{
				rightToLeft = value;
				UpdateReportControl("RightToLeft");
			}
		}


		private string text = "Form";
		/// <summary>
		/// Gets or sets the text associated with this control.
		/// </summary>
		[StiSerializable]
		[StiCategory("Appearance")]
		[Description("Gets or sets the text associated with this control.")]
        [StiOrder(StiPropertyOrder.DialogText)]
        [StiPropertyLevel(StiLevel.Basic)]
		public virtual string Text
		{
			get
			{
				return text;
			}
			set
			{
				text = value;
				UpdateReportControl("Text");
			}
		}


		private Font font = new Font("Microsoft Sans Serif", 8);
		/// <summary>
		/// Gets or sets font of control.
		/// </summary>
		[StiSerializable]
		[StiCategory("Appearance")]
		[Description("Gets or sets font of control.")]
        [StiGuiMode(StiGuiMode.Gdi)]
        [StiOrder(StiPropertyOrder.DialogFont)]
        [StiPropertyLevel(StiLevel.Basic)]
		public virtual Font Font
		{
			get
			{
				return font;
			}
			set
			{
				font = value;
				UpdateReportControl("Font");
			}
		}


		private Color backColor = SystemColors.Control;
		/// <summary>
		/// Gets or sets the background color for the control.
		/// </summary>
		[StiSerializable]
		[StiCategory("Appearance")]
		[Description("Gets or sets the background color for the control.")]
        [StiGuiMode(StiGuiMode.Gdi)]
        [StiOrder(StiPropertyOrder.DialogBackColor)]
        [StiPropertyLevel(StiLevel.Basic)]
		public virtual Color BackColor
		{
			get
			{
				return backColor;
			}
			set
			{
				backColor = value;
				UpdateReportControl("BackColor");
			}
		}


		private bool ShouldSerializeBackColor()
		{
			return backColor != SystemColors.Control;
		}


		private FormStartPosition startPosition = FormStartPosition.CenterScreen;
		/// <summary>
		/// Gets or sets the starting position of the form at run time.
		/// </summary>
		[StiSerializable]
		[StiCategory("Behavior")]
		[DefaultValue(FormStartPosition.CenterScreen)]
		[TypeConverter(typeof(Stimulsoft.Base.Localization.StiEnumConverter))]
		[Editor(StiEditors.Enum, typeof(UITypeEditor))]
		[Description("Gets or sets the starting position of the form at run time.")]
        [StiOrder(StiPropertyOrder.DialogStartPosition)]
        [StiPropertyLevel(StiLevel.Professional)]
		public virtual FormStartPosition StartPosition
		{
			get
			{
				return startPosition;
			}
			set
			{
				startPosition = value;
				UpdateReportControl("StartPosition");
			}
		}


		private FormWindowState windowState = FormWindowState.Normal;
		/// <summary>
		/// Gets or sets the form's window state.
		/// </summary>
		[StiSerializable]
		[StiCategory("Behavior")]
		[DefaultValue(FormWindowState.Normal)]
		[TypeConverter(typeof(Stimulsoft.Base.Localization.StiEnumConverter))]
		[Editor(StiEditors.Enum, typeof(UITypeEditor))]
		[Description("Gets or sets the form's window state.")]
        [StiOrder(StiPropertyOrder.DialogWindowState)]
        [StiPropertyLevel(StiLevel.Professional)]
    	public virtual FormWindowState WindowState
		{
			get
			{
				return windowState;
			}
			set
			{
				windowState = value;
				UpdateReportControl("WindowState");
			}
		}
		

		[StiCategory("Design")]
        [StiOrder(StiPropertyOrder.DialogTag)]
        [Browsable(true)]
        [StiPropertyLevel(StiLevel.Standard)]
		public override StiTagExpression Tag
		{
			get
			{
				return base.Tag;
			}
			set
			{
				base.Tag = value;
			}
		}


		private StiFormStartMode startMode = StiFormStartMode.OnStart;
		/// <summary>
		/// Gets or sets value which indicates time when form appears.
		/// </summary>
		[StiCategory("Behavior")]
		[DefaultValue(StiFormStartMode.OnStart)]
		[Description("Gets or sets value which indicates time when form appears.")]
		[StiSerializable]
		[TypeConverter(typeof(Stimulsoft.Base.Localization.StiEnumConverter))]
		[Editor(StiEditors.Enum, typeof(UITypeEditor))]
		[StiOrder(StiPropertyOrder.DialogStartMode)]
        [StiPropertyLevel(StiLevel.Professional)]
		public StiFormStartMode StartMode
		{
			get
			{
				return startMode;
			}
			set
			{
				startMode = value;
				UpdateReportControl("StartMode");
			}
		}


		private bool visible = true;
		/// <summary>
		/// Gets or sets a value indicating whether the control is displayed.
		/// </summary>
		[StiSerializable]
		[DefaultValue(true)]
		[StiCategory("Behavior")]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		[Description("Gets or sets a value indicating whether the control is displayed.")]
        [StiOrder(StiPropertyOrder.DialogVisible)]
        [StiPropertyLevel(StiLevel.Basic)]
		public virtual bool Visible
		{
			get
			{
				return visible;
			}
			set
			{
				visible = value;
				UpdateReportControl("Visible");
			}
		}
		#endregion

        #region Methods.override
        public override StiComponent CreateNew()
        {
            return new StiForm();
        }
        #endregion

		#region this
		/// <summary>
		/// Use for update report control properties.
		/// </summary>
		/// <param name="propertyName">A name of the property which will be updated.</param>
		protected void UpdateReportControl(string propertyName)
		{
			if (Page != null)((StiForm)Page).InvokeReportControlUpdate(this, propertyName);
		}

		public DialogResult ShowDialog()
        {
            var provider = StiDialogsProvider.GetProvider(Report);

            bool resVisible = this.Visible;

			if (!this.Visible)this.Visible = true;
			
			try
			{
				return provider.RenderForm(this) == true ? DialogResult.OK : DialogResult.Cancel;
			}
			finally
			{
				this.Visible = resVisible;
			}
		}

		/// <summary>
		/// Gets default event for this report control.
		/// </summary>
		/// <returns>Default event.</returns>
		public StiEvent GetDefaultEvent()
		{
			return this.LoadFormEvent;
		}


		/// <summary>
		/// Creates a new StiForm.
		/// </summary>
		public StiForm() : this(null)
		{
		}


		/// <summary>
		/// Creates a new StiForm.
		/// </summary>
		public StiForm(StiReport report) : base(report)
		{
			this.Size = new Size(304, 232);
		}
		#endregion
	}
}