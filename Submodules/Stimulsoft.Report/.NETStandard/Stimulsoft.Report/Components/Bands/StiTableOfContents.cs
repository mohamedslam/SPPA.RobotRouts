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

using Stimulsoft.Base;
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Services;
using Stimulsoft.Report.Components.Design;
using Stimulsoft.Report.Design;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Engine;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Painters;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Components
{
    [StiServiceBitmap(typeof(StiComponent), "Stimulsoft.Report.Images.Components.StiTableOfContents.png")]
    [StiToolbox(true)]
    [StiDesigner("Stimulsoft.Report.Components.Design.StiTableOfContentsDesigner, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfDesigner("Stimulsoft.Report.WpfDesign.StiWpfTableOfContentsDesigner, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
    [StiWpfPainter("Stimulsoft.Report.Painters.StiTableOfContentsWpfPainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
    [StiGdiPainter(typeof(StiTableOfContentsGdiPainter))]
    [StiV2Builder(typeof(StiTableOfContentsV2Builder))]
    [StiContextTool(typeof(IStiComponentDesigner))]
    [StiEngine(StiEngineVersion.EngineV2)]
    public class StiTableOfContents : StiDataBand
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            //Old
            jObject.RemoveProperty("CanBreak");
            jObject.RemoveProperty("CanGrow");
            jObject.RemoveProperty("CanShrink");
            jObject.RemoveProperty("CountData");
            jObject.RemoveProperty("DataSource");
            jObject.RemoveProperty("DataRelation");
            jObject.RemoveProperty("MasterComponent");
            jObject.RemoveProperty("BusinessObject");
            jObject.RemoveProperty("Filters");
            jObject.RemoveProperty("FilterOn");
            jObject.RemoveProperty("FilterEngine");
            jObject.RemoveProperty("FilterMode");
            jObject.RemoveProperty("SkipFirst");
            jObject.RemoveProperty("Sort");
            jObject.RemoveProperty("NewColumnAfter");
            jObject.RemoveProperty("NewColumnBefore");
            jObject.RemoveProperty("BreakIfLessThan");
            jObject.RemoveProperty("LimitRows");            
            jObject.RemoveProperty("Columns");
            jObject.RemoveProperty("ColumnWidth");
            jObject.RemoveProperty("ColumnGaps");
            jObject.RemoveProperty("ColumnDirection");
            jObject.RemoveProperty("MinRowsInColumn");
            jObject.RemoveProperty("EvenStyle");
            jObject.RemoveProperty("OddStyle");            
            jObject.RemoveProperty("CalcInvisible");
            jObject.RemoveProperty("KeepDetails");
            jObject.RemoveProperty("PrintAtBottom");
            jObject.RemoveProperty("PrintIfDetailEmpty");
            jObject.RemoveProperty("PrintOnAllPages");
            jObject.RemoveProperty("PrintOn");
            jObject.RemoveProperty("ResetPageNumber");
            jObject.RemoveProperty("Interaction");
            
            jObject.RemoveProperty("BeginRenderEvent");
            jObject.RemoveProperty("RenderingEvent");
            jObject.RemoveProperty("EndRenderEvent");
            jObject.RemoveProperty("GetCollapsedEvent");
            jObject.RemoveProperty("GetBookmarkEvent");
            jObject.RemoveProperty("GetHyperlinkEvent");
            jObject.RemoveProperty("GetTagEvent");
            jObject.RemoveProperty("GetBookmarkEvent");
            jObject.RemoveProperty("GetHyperlinkEvent");
            jObject.RemoveProperty("GetTagEvent");
            jObject.RemoveProperty("GetToolTipEvent");
            jObject.RemoveProperty("BeforePrintEvent");
            jObject.RemoveProperty("AfterPrintEvent");
            jObject.RemoveProperty("GetDrillDownReportEvent");
            jObject.RemoveProperty("GetPointerEvent");
            jObject.RemoveProperty("ClickEvent");
            jObject.RemoveProperty("DoubleClickEvent");
            jObject.RemoveProperty("MouseEnterEvent");
            jObject.RemoveProperty("MouseLeaveEvent");

            // StiTableOfContents
            jObject.AddPropertyStringNullOrEmpty("ReportPointer", ReportPointer);
            jObject.AddPropertyInt("Indent", Indent, 10);
            jObject.AddPropertyJObject("Styles", Styles.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("Margins", Margins.SaveToJsonObject(10, 10, 10, 10));

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            this.Styles.Clear();

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "ReportPointer":
                        this.ReportPointer = property.DeserializeString();
                        break;

                    case "Indent":
                        this.Indent = property.DeserializeInt();
                        break;

                    case "Styles":
                        this.Styles.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "Margins":                        
                        var margins = new StiMargins();
                        margins.LoadFromJsonObject((JObject)property.Value);
                        this.Margins = margins;                        
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiTableOfContents;
        #endregion

        #region ICloneable override
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone(bool cloneProperties)
        {
            var table = base.Clone(cloneProperties) as StiTableOfContents;

            table.Border = Border?.Clone() as StiBorder;
            table.Brush = Brush?.Clone() as StiBrush;

            return table;
        }
        #endregion

        #region StiComponent override
        /// <summary>
        /// Gets a component priority.
        /// </summary>
        public override int Priority => (int)StiComponentPriority.TableOfContents;

        /// <summary>
        /// Gets or sets the default client area of a component.
        /// </summary>
        [Browsable(false)]
        public override RectangleD DefaultClientRectangle => new RectangleD(0, 0, 300, 100);

        public override int ToolboxPosition => (int)StiComponentToolboxPosition.TableOfContents;

        /// <summary>
        /// Gets a localized component name.
        /// </summary>
        public override string LocalizedName => StiLocalization.Get("Components", "StiTableOfContents");

        public override string HelpUrl => "user-manual/index.html?report_internals_tableofcontents.htm";
        #endregion

        #region Properties.Off
        [Browsable(false)]
        [StiNonSerialized]
        public sealed override int CountData
        {
            get
            {
                return 1;
            }
            set
            {
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public sealed override bool CanBreak
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
        public sealed override bool CanGrow
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
        public sealed override bool CanShrink
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
        public sealed override StiDataSource DataSource => base.DataSource;

        [Browsable(false)]
        [StiNonSerialized]
        public sealed override StiDataRelation DataRelation => base.DataRelation;

        [Browsable(false)]
        [StiNonSerialized]
        public sealed override StiComponent MasterComponent
        {
            get
            {
                return base.MasterComponent;
            }
            set
            {
                base.MasterComponent = value;
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public sealed override StiBusinessObject BusinessObject => base.BusinessObject;

        [Browsable(false)]
        [StiNonSerialized]
        public sealed override bool FilterOn
        {
            get
            {
                return base.FilterOn;
            }
            set
            {
                base.FilterOn = value;
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public sealed override StiFiltersCollection Filters
        {
            get
            {
                return base.Filters;
            }
            set
            {
                base.Filters = value;
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public sealed override StiFilterEngine FilterEngine
        {
            get
            {
                return base.FilterEngine;
            }
            set
            {
                base.FilterEngine = value;
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public sealed override StiFilterMode FilterMode
        {
            get 
            { 
                return base.FilterMode; 
            }
            set
            {
                base.FilterMode = value;
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public sealed override bool SkipFirst
        {
            get
            {
                return base.SkipFirst;
            }
            set
            {
                base.SkipFirst = value;
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public sealed override string[] Sort
        {
            get 
            { 
                return base.Sort; 
            }
            set
            {
                base.Sort = value;
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public sealed override bool NewColumnAfter
        {
            get
            {
                return base.NewColumnAfter;
            }
            set
            {
                base.NewColumnAfter = value;
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public sealed override bool NewColumnBefore
        {
            get
            {
                return base.NewColumnBefore;
            }
            set
            {
                base.NewColumnBefore = value;
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public sealed override float BreakIfLessThan
        {
            get
            {
                return base.BreakIfLessThan;
            }
            set
            {
                base.BreakIfLessThan = value;
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public sealed override string LimitRows
        {
            get
            {
                return base.LimitRows;
            }
            set
            {
                base.LimitRows = value;
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

        [Browsable(false)]
        [StiNonSerialized]
        public sealed override StiColumnDirection ColumnDirection
        {
            get
            {
                return base.ColumnDirection;
            }
            set
            {
                base.ColumnDirection = value;
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
        public sealed override int MinRowsInColumn
        {
            get
            {
                return base.MinRowsInColumn;
            }
            set
            {
                base.MinRowsInColumn = value;
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public sealed override string OddStyle
        {
            get
            {
                return base.OddStyle;
            }
            set
            {
                base.OddStyle = value;
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public sealed override string EvenStyle
        {
            get
            {
                return base.EvenStyle;
            }
            set
            {
                base.EvenStyle = value;
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public sealed override bool CalcInvisible
        {
            get
            {
                return base.CalcInvisible;
            }
            set
            {
                base.CalcInvisible = value;
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public sealed override StiKeepDetails KeepDetails
        {
            get
            {
                return base.KeepDetails;
            }
            set
            {
                base.KeepDetails = value;
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public sealed override bool PrintAtBottom
        {
            get
            {
                return base.PrintAtBottom;
            }
            set
            {
                base.PrintAtBottom = value;
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public sealed override bool PrintIfDetailEmpty
        {
            get
            {
                return base.PrintIfDetailEmpty;
            }
            set
            {
                base.PrintIfDetailEmpty = value;
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public sealed override bool PrintOnAllPages
        {
            get
            {
                return base.PrintOnAllPages;
            }
            set
            {
                base.PrintOnAllPages = value;
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public sealed override StiPrintOnType PrintOn
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
                base.ResetPageNumber = value;
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
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
        #endregion

        #region Events.Off
        [StiEventHide]
        [StiNonSerialized]
        public override StiBeginRenderEvent BeginRenderEvent
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
        public override StiRenderingEvent RenderingEvent
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
        public override StiEndRenderEvent EndRenderEvent
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
        public override StiGetCollapsedEvent GetCollapsedEvent
        {
            get
            {
                return base.GetCollapsedEvent;
            }
            set
            {
                base.GetCollapsedEvent = value;
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
        public sealed override StiGetTagEvent GetTagEvent
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

        [StiEventHide]
        [StiNonSerialized]
        public sealed override StiClickEvent ClickEvent
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

        [StiEventHide]
        [StiNonSerialized]
        public sealed override StiDoubleClickEvent DoubleClickEvent
        {
            get
            {
                return base.DoubleClickEvent;
            }
            set
            {
                base.DoubleClickEvent = value;
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
        #endregion

        #region StiBand override
        /// <summary>
        /// Gets header start color.
        /// </summary>
        [Browsable(false)]
        public override Color HeaderStartColor => StiColor.Get("edc87e");

        /// <summary>
		/// Gets header end color.
		/// </summary>
		[Browsable(false)]
        public override Color HeaderEndColor => StiColor.Get("77edc87e");

        /// <summary>
        /// Returns the band header text.
        /// </summary>
        /// <returns>Band header text.</returns>
        public override string GetHeaderText()
        {
            return ToString();
        }
        #endregion

        #region Methods
        public override StiComponent CreateNew()
        {
            return new StiTableOfContents();
        }

        internal List<StiStyle> GetStylesList()
        {
            return Styles
                .Cast<StiBaseStyle>()
                .Where(s => s is StiStyle)
                .Cast<StiStyle>()
                .ToList();
        }
        #endregion

        #region Properties
        [StiCategory("TableOfContents")]
        [StiOrder(StiPropertyOrder.TableOfContentsNewPageAfter)]
        public override bool NewPageAfter
        {
            get
            {
                return base.NewPageAfter;
            }
            set
            {
                base.NewPageAfter = value;
            }
        }

        [StiCategory("TableOfContents")]
        [StiOrder(StiPropertyOrder.TableOfContentsNewPageBefore)]
        public override bool NewPageBefore
        {
            get
            {
                return base.NewPageBefore;
            }
            set
            {
                base.NewPageBefore = value;
            }
        }

        /// <summary>
        /// Gets or sets horizontal output direction.
        /// </summary>
        [DefaultValue(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets horizontal output direction.")]
        [StiOrder(StiPropertyOrder.TableOfContentsRightToLeft)]
        [StiSerializable]
        [StiCategory("TableOfContents")]
        [StiPropertyLevel(StiLevel.Basic)]
        public override bool RightToLeft
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

        /// <summary>
        /// Gets true if this component is first in the report.
        /// </summary>
        internal bool IsFirstInReport
        {
            get
            {
                if (Report == null)
                    return false;

                return Report.GetComponents().Cast<StiComponent>().Where(c => c is StiTableOfContents && c.Enabled).ToList().IndexOf(this) == 0;
            }
        }

        /// <summary>
        /// Gets a collection which consists of report styles.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.List)]
        [StiCategory("TableOfContents")]
        [Editor("Stimulsoft.Report.Design.StiTableOfContentsStylesEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [TypeConverter(typeof(StiStylesConverter))]
        [Description("Gets a collection which consists of report styles.")]
        [StiOrder(StiPropertyOrder.TableOfContentsStyles)]
        [StiPropertyLevel(StiLevel.Standard)]
        public StiStylesCollection Styles { get; }

        /// <summary>
        /// Gets or sets indent from the left side of band for offset of data levels. 
        /// </summary>
        [StiSerializable]
        [StiCategory("TableOfContents")]
        [Description("Gets or sets indent from the left side of band for offset of data levels. ")]
        [StiOrder(StiPropertyOrder.TableOfContentsIndent)]
        [StiPropertyLevel(StiLevel.Standard)]
        [DefaultValue(15)]
        public int Indent { get; set; } = 15;

        /// <summary>
        /// Internal use only.
        /// </summary>
        [StiSerializable]
        [DefaultValue("")]
        [Browsable(false)]
        public string ReportPointer { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets table margins.
        /// </summary>
        [StiSerializable]
        [StiCategory("TableOfContents")]
        [StiOrder(StiPropertyOrder.TableOfContentsMargins)]
        [Description("Gets or sets table margins.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual StiMargins Margins { get; set; } = new StiMargins(10);

        private bool ShouldSerializeMargins()
        {
            return Margins == null 
                || Margins.Left != 10 
                || Margins.Right != 10 
                || Margins.Top != 10 
                || Margins.Bottom != 10;
        }
        #endregion

        /// <summary>
        /// Creates a new StiTableOfContents.
        /// </summary>
        public StiTableOfContents() : this(RectangleD.Empty)
        {
        }

        /// <summary>
        /// Creates a new StiTableOfContents.
        /// </summary>
        /// <param name="rect">The rectangle describes sizes and position of the component.</param>
        public StiTableOfContents(RectangleD rect) : base(rect)
        {
            PlaceOnToolbox = false;

            Styles = new StiStylesCollection
            {
                new StiStyle($"{Loc.GetMain("Heading")}1")
                {
                    Font = new Font("Arial", 10, FontStyle.Bold)
                },

                new StiStyle($"{Loc.GetMain("Heading")}2")
                {
                    Font = new Font("Arial", 8)
                },

                new StiStyle($"{Loc.GetMain("Heading")}3")
                {
                    Font = new Font("Arial", 8),
                    TextBrush = new StiSolidBrush(Color.DimGray)
                }
            };
        }
    }
}