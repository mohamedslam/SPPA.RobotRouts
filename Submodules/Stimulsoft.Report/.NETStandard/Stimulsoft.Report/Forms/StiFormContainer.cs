#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Form     											}
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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Units;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Design.Forms
{
    /// <summary>
    /// Represents a form panel.
    /// </summary>
    public class StiFormContainer :
        StiPage
    {
        #region ICloneable override
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone(bool cloneProperties)
        {
            var form = base.Clone(cloneProperties) as StiFormContainer;
            return form;
        }
        #endregion

        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject = new JObject();

            jObject.AddPropertyIdent("Ident", this.GetType().Name);

            if (Content != null)
                jObject.AddPropertyJObject(nameof(Content), JObject.Parse(Content));

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(Content):
                        this.Content = ((JObject)property.Value).ToString();
                        break;
                }
            }            
        }
        #endregion     

        #region StiComponent override
        /// <summary>
        /// ID code of this meter. Used in JSON saving.
        /// </summary>
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiFormContainer;

        /// <summary>
        /// Gets a localized component name.
        /// </summary>
        [Browsable(false)]
        public override string LocalizedName => "StiFormContainer";//TODO Loc.Get("Components", "StiFormContainer");
        #endregion

        #region Off
        /*[Browsable(false)]
        [StiNonSerialized]
        public sealed override string Name
        {
            get
            {
                return base.Name;
            }
            set
            {
            }
        }*/

        [Browsable(false)]
        [StiNonSerialized]
        public sealed override StiAppExpressionCollection Expressions 
        { 
            get 
            {
                return base.Expressions;
            }
            set
            {
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public sealed override StiComponentsCollection Components
        {
            get
            {
                return base.Components;
            }
            set
            {
               
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public sealed override StiMargins Margins
        {
            get
            {
                return base.Margins;
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
        public override double GridSize => Report.Info.GridSizePoints;

        [Browsable(false)]
        public override StiUnit Unit => StiUnit.HundredthsOfInch;

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

        [Browsable(false)]
        [StiNonSerialized]
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

        [Browsable(false)]
        [StiNonSerialized]
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
        #endregion

        #region IStiUnitConvert override Off
        public override void Convert(StiUnit oldUnit, StiUnit newUnit, bool isReportSnapshot = false)
        {
        }
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
        #endregion

        #region Methods
        public override StiComponent CreateNew()
        {
            return new StiFormContainer();
        }
        #endregion

        #region Properties.override    
        private object form;

        private string content;
        /// <summary>
        /// Stores an form content
        /// </summary>
        [StiSerializable]
        [Browsable(false)]
        public string Content {
            get
            {
                if (this.form != null)
                {
                    return StiFormAssembly.SaveForm(this.form);
                }
                return this.content;
            }
            set
            {
                this.content = value;
                form = StiFormAssembly.LoadForm(value);
            }
        }
        #endregion

        /// <summary>
        /// Creates a new StiFormContainer.
        /// </summary>
        public StiFormContainer() : this(null)
        {
        }

        /// <summary>
        /// Creates a new StiFormContainer.
        /// </summary>
        public StiFormContainer(StiReport report) : base(report)
        {
            this.form = StiFormAssembly.NewStiFormElement();
        }
    }
}