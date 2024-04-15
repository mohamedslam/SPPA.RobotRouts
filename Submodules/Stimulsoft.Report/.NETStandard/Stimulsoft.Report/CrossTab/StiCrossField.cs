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

using System.Drawing.Design;
using System.ComponentModel;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Services;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Components.TextFormats;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Design;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Base.Drawing.Design;
using System.Drawing;
using Stimulsoft.Base.Design;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.CrossTab
{
    [StiToolbox(false)]
    [StiServiceBitmap(typeof(StiComponent), "Stimulsoft.Report.Images.Components.StiCrossTab.png")]
    public abstract class StiCrossField :
        StiText,
        IStiCrossTabField
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // Old
            jObject.AddPropertyEnum("HorAlignment", HorAlignment, StiTextHorAlignment.Center);
            jObject.AddPropertyEnum("VertAlignment", VertAlignment, StiVertAlignment.Center);
            jObject.RemoveProperty("ExcelValue");
            jObject.RemoveProperty("PrintOn");
            jObject.RemoveProperty("ExportAsImage");
            jObject.RemoveProperty("ShrinkFontToFit");
            jObject.RemoveProperty("ShrinkFontToFitMinimumSize");
            jObject.RemoveProperty("Locked");
            jObject.RemoveProperty("Linked");
            jObject.RemoveProperty("CanBreak");
            jObject.RemoveProperty("WYSIWYG");
            jObject.RemoveProperty("Editable");
            jObject.RemoveProperty("GlobalizedName");
            jObject.RemoveProperty("LinesOfUnderline");
            jObject.RemoveProperty("ProcessingDuplicates");
            jObject.RemoveProperty("OnlyText");
            jObject.RemoveProperty("ProcessAtEnd");
            jObject.RemoveProperty("ProcessAt");
            jObject.RemoveProperty("MaxNumberOfLines");
            jObject.RemoveProperty("DockStyle");
            jObject.RemoveProperty("GrowToHeight");
            jObject.RemoveProperty("ShiftMode");
            jObject.RemoveProperty("Left");
            jObject.RemoveProperty("Top");
            jObject.RemoveProperty("Width");
            jObject.RemoveProperty("Height");
            jObject.RemoveProperty("AutoWidth");
            jObject.RemoveProperty("Printable");
            jObject.RemoveProperty("Pointer");
            jObject.RemoveProperty("GetPointerEvent");
            jObject.RemoveProperty("Bookmark");
            jObject.RemoveProperty("GetBookmarkEvent");

            // StiCrossField
            jObject.AddPropertyJObject("ProcessCellEvent", ProcessCellEvent.SaveToJsonObject(mode));
            jObject.AddPropertyBool("MergeHeaders", MergeHeaders, true);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "ProcessCellEvent":
                        {
                            var _event = new StiProcessCellEvent();
                            _event.LoadFromJsonObject((JObject)property.Value);
                            this.ProcessCellEvent = _event;
                        }
                        break;

                    case "MergeHeaders":
                        this.MergeHeaders = property.DeserializeBool();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiEventCollection GetEvents(IStiPropertyGrid propertyGrid)
        {
            var collection = new StiEventCollection();

            collection.Add(StiPropertyCategories.DataEvents, new[]
            {
                StiPropertyEventId.ProcessCellEvent
            });

            collection.Add(StiPropertyCategories.ValueEvents, new[]
            {
                StiPropertyEventId.GetExcelValueEvent,
                StiPropertyEventId.GetValueEvent,
                StiPropertyEventId.GetToolTipEvent,
                StiPropertyEventId.GetTagEvent
            });

            collection.Add(StiPropertyCategories.NavigationEvents, new[]
            {
                StiPropertyEventId.GetHyperlinkEvent,
                StiPropertyEventId.GetBookmarkEvent
            });

            collection.Add(StiPropertyCategories.PrintEvents, new[]
            {
                StiPropertyEventId.BeforePrintEvent,
                StiPropertyEventId.AfterPrintEvent
            });

            collection.Add(StiPropertyCategories.MouseEvents, new[]
            {
                StiPropertyEventId.GetDrillDownReportEvent,
                StiPropertyEventId.ClickEvent,
                StiPropertyEventId.DoubleClickEvent,
                StiPropertyEventId.MouseEnterEvent,
                StiPropertyEventId.MouseLeaveEvent
            });

            return collection;
        }

        #endregion

        #region IStiTextFormat override
        /// <summary>
        /// Gets or sets the format of the text.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.Class)]
        [DefaultValue("")]
        [Editor("Stimulsoft.Report.Components.TextFormats.Design.StiTextFormatEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiCategory("TextAdditional")]
        [StiOrder(StiPropertyOrder.TextTextFormat)]
        [Description("Gets or sets the format of the text.")]
        public override StiFormatService TextFormat
        {
            get
            {
                return base.TextFormat;
            }
            set
            {
                base.TextFormat = value;
            }
        }
        #endregion

        #region IStiTextBrush override
        /// <summary>
        /// The brush of the component, which is used to display text.
        /// </summary>
        [StiCategory("Appearance")]
        [StiOrder(StiPropertyOrder.AppearanceTextBrush)]
        [StiSerializable]
        [Description("The brush of the component, which is used to display text.")]
        [TypeConverter(typeof(StiExpressionBrushConverter))]
        [Editor(StiEditors.ExpressionBrush, typeof(UITypeEditor))]
        [StiExpressionAllowed]
        public override StiBrush TextBrush
        {
            get
            {
                return base.TextBrush;
            }
            set
            {
                base.TextBrush = value;
            }
        }
        #endregion

        #region IStiFont
        /// <summary>
        /// Gets or sets font of component.
        /// </summary>
        [StiCategory("Appearance")]
        [StiOrder(StiPropertyOrder.AppearanceFont)]
        [StiSerializable]
        [Description("Gets or sets font of component.")]
        public override Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                base.Font = value;
            }
        }
        #endregion

        #region IStiTextHorAlignment
        protected override StiTextHorAlignment DefaultHorAlignment => StiTextHorAlignment.Center;

        /// <summary>
        /// Gets or sets the text horizontal alignment.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiTextHorAlignment.Center)]
        [StiCategory("Appearance")]
        [TypeConverter(typeof(Stimulsoft.Base.Localization.StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("Gets or sets the text horizontal alignment.")]
        [StiOrder(StiPropertyOrder.TextHorAlignment)]
        public override StiTextHorAlignment HorAlignment
        {
            get
            {
                return base.HorAlignment;
            }
            set
            {
                base.HorAlignment = value;
            }
        }
        #endregion

        #region IStiVertAlignment
        /// <summary>
        /// Gets or sets the vertical alignment of an object.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiVertAlignment.Center)]
        [StiCategory("Appearance")]
        [TypeConverter(typeof(Stimulsoft.Base.Localization.StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("Gets or sets the vertical alignment of an object.")]
        [StiOrder(StiPropertyOrder.TextVertAlignment)]
        public override StiVertAlignment VertAlignment
        {
            get
            {
                return base.VertAlignment;
            }
            set
            {
                base.VertAlignment = value;
            }
        }
        #endregion

        #region Off
        [StiNonSerialized]
        [Browsable(false)]
        public override StiExcelValueExpression ExcelValue
        {
            get
            {
                return base.ExcelValue;
            }
            set
            {
                base.ExcelValue = value;
            }
        }

        [StiNonSerialized]
        [Browsable(false)]
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

        [StiNonSerialized]
        [Browsable(false)]
        public sealed override bool ExportAsImage
        {
            get
            {
                return base.ExportAsImage;
            }
            set
            {
            }
        }

        [StiNonSerialized]
        [Browsable(false)]
        public override bool ShrinkFontToFit
        {
            get
            {
                return base.ShrinkFontToFit;
            }
            set
            {
            }
        }

        [StiNonSerialized]
        [Browsable(false)]
        public override float ShrinkFontToFitMinimumSize
        {
            get
            {
                return base.ShrinkFontToFitMinimumSize;
            }
            set
            {
            }
        }

        [Browsable(false)]
        public sealed override string RenderTo
        {
            get
            {
                return base.RenderTo;
            }
            set
            {
                base.RenderTo = value;
            }
        }

        [Browsable(false)]
        public sealed override string Alias
        {
            get
            {
                return base.Alias;
            }
            set
            {
                base.Alias = value;
            }
        }

        [Browsable(false)]
        public sealed override string Name
        {
            get
            {
                return base.Name;
            }
            set
            {
                if (base.Name != value && !string.IsNullOrEmpty(base.Name) && this.Parent != null)//rename column/row titles 
                {
                    foreach (var comp in this.Parent?.Components)
                    {
                        if (this is StiCrossRow && (comp is StiCrossTitle crossTitle) && crossTitle.TypeOfComponent == $"Row:{base.Name}")
                        {
                            crossTitle.TypeOfComponent = $"Row:{value}";
                        }
                        else if (this is StiCrossColumn && (comp is StiCrossTitle crossTitle2) && crossTitle2.TypeOfComponent == $"Col:{base.Name}")
                        {
                            crossTitle2.TypeOfComponent = $"Row:{value}";
                        }
                    }
                }

                base.Name = value;
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public sealed override bool Locked
        {
            get
            {
                return IsDesigning && (!Report.IsPageDesigner);
            }
            set
            {
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public sealed override bool Linked
        {
            get
            {
                return IsDesigning && (!Report.IsPageDesigner);
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
                return base.CanBreak;
            }
            set
            {
                base.CanBreak = value;
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public sealed override bool WYSIWYG
        {
            get
            {
                return base.WYSIWYG;
            }
            set
            {
                base.WYSIWYG = value;
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public sealed override bool Editable
        {
            get
            {
                return base.Editable;
            }
            set
            {
                base.Editable = value;
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public sealed override string GlobalizedName
        {
            get
            {
                return base.GlobalizedName;
            }
            set
            {
                base.GlobalizedName = value;
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public override StiPenStyle LinesOfUnderline
        {
            get
            {
                return base.LinesOfUnderline;
            }
            set
            {
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public sealed override StiProcessingDuplicatesType ProcessingDuplicates
        {
            get
            {
                return StiProcessingDuplicatesType.None;
            }
            set
            {
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public sealed override bool OnlyText
        {
            get
            {
                return base.OnlyText;
            }
            set
            {
                base.OnlyText = value;
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public sealed override bool ProcessAtEnd
        {
            get
            {
                return base.ProcessAtEnd;
            }
            set
            {
                base.ProcessAtEnd = value;
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public sealed override StiProcessAt ProcessAt
        {
            get
            {
                return base.ProcessAt;
            }
            set
            {
                base.ProcessAt = value;
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public sealed override int MaxNumberOfLines
        {
            get
            {
                return base.MaxNumberOfLines;
            }
            set
            {
                base.MaxNumberOfLines = value;
            }
        }

        [Browsable(false)]
        public override bool Enabled
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

        [Browsable(false)]
        [StiNonSerialized]
        public sealed override StiDockStyle DockStyle
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

        [Browsable(false)]
        [StiNonSerialized]
        public sealed override bool GrowToHeight
        {
            get
            {
                return base.GrowToHeight;
            }
            set
            {
                base.GrowToHeight = value;
            }
        }

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

        [Browsable(false)]
        [StiNonSerialized]
        public sealed override StiShiftMode ShiftMode
        {
            get
            {
                return base.ShiftMode;
            }
            set
            {
                base.ShiftMode = value;
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public sealed override double Left
        {
            get
            {
                return base.Left;
            }
            set
            {
                base.Left = value;
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public sealed override double Top
        {
            get
            {
                return base.Top;
            }
            set
            {
                base.Top = value;
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public override double Width
        {
            get
            {
                return base.Width;
            }
            set
            {
                base.Width = value;
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public override double Height
        {
            get
            {
                return base.Height;
            }
            set
            {
                base.Height = value;
            }
        }

        #region IStiCanShrink Off
        [Browsable(false)]
        [StiNonSerialized]
        public sealed override bool CanShrink
        {
            get
            {
                return false;
            }
            set
            {
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
                return false;
            }
            set
            {
            }
        }
        #endregion

        #region IStiAutoWidth Off
        [Browsable(false)]
        [StiNonSerialized]
        public sealed override bool AutoWidth
        {
            get
            {
                return base.AutoWidth;
            }
            set
            {
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
            }
        }
        #endregion
        #endregion

        #region Events Off
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
        #endregion

        #region Events
        #region ProcessCell
        public event StiProcessCellEventHandler ProcessCell;

        /// <summary>
        /// Raises the ProcessCell event for this component.
        /// </summary>
        protected virtual void OnProcessCell(StiProcessCellEventArgs e)
        {
        }

        public void InvokeProcessCell(StiProcessCellEventArgs e)
        {
            OnProcessCell(e);
            this.ProcessCell?.Invoke(this, e);
        }

        /// <summary>
        /// Gets or sets a script of the event ProcessCell.
        /// </summary>
        [StiSerializable]
        [Browsable(false)]
        [StiCategory("Data")]
        [Description("Gets or sets a script of the event ProcessCellEvent.")]
        public StiProcessCellEvent ProcessCellEvent { get; set; } = new StiProcessCellEvent();
        #endregion
        #endregion

        #region StiComponent override
        public override string HelpUrl => null;

        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Gets a localized name of the component category.
        /// </summary>
        public override string LocalizedCategory => StiLocalization.Get("Report", "CrossBands");

        [Browsable(false)]
        public override StiRestrictions Restrictions
        {
            get
            {
                return this.Report != null && this.Report.IsPageDesigner 
                    ? base.Restrictions ^ StiRestrictions.AllowDelete 
                    : base.Restrictions;
            }
            set
            {
                if (StiRestrictionsHelper.IsAllowDelete(value))
                    value ^= StiRestrictions.AllowDelete;

                base.Restrictions = value;
            }
        }
        #endregion

        #region this
        public StiText GetTextBoxFromField()
        {
            return new StiText
            {
                Border = (StiBorder) this.Border.Clone(),
                Brush = (StiBrush) this.Brush.Clone(),
                Font = (Font) this.Font.Clone(),
                TextFormat = (StiFormatService) this.TextFormat.Clone(),
                TextBrush = (StiBrush) this.TextBrush.Clone(),
                TextOptions = (StiTextOptions) this.TextOptions.Clone(),
                HorAlignment = this.HorAlignment,
                VertAlignment = this.VertAlignment
            };
        }

        [Browsable(false)]
        public virtual string CellText => Alias;

        /// <summary>
        /// Gets or sets value which indicates that all equal values of header will be merged into one.
        /// </summary>
        [DefaultValue(true)]
        [StiSerializable]
        [Browsable(true)]
        [StiCategory("Behavior")]
        [StiOrder(StiPropertyOrder.BehaviorMergeHeaders)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates that all equal values of header will be merged into one.")]
        public bool MergeHeaders { get; set; } = true;

        [Browsable(false)]
        public object OriginalValue { get; set; }

        [Browsable(false)]
        public bool DisabledByCondition { get; set; }

        [Browsable(false)]
        public StiBrush ConditionBrush { get; set; }

        [Browsable(false)]
        public StiBrush ConditionTextBrush { get; set; }

        [Browsable(false)]
        public StiConditionPermissions ConditionPermissions { get; set; } = StiConditionPermissions.None;

        public StiCrossField() : base(new RectangleD(0, 0, 1, 1))
        {
            this.NewGuid();
            this.Border.Side = StiBorderSides.All;
            this.VertAlignment = StiVertAlignment.Center;
            this.HorAlignment = StiTextHorAlignment.Center;
            this.Restrictions = StiRestrictions.None | 
                                StiRestrictions.AllowMove | 
                                StiRestrictions.AllowResize | 
                                StiRestrictions.AllowSelect | 
                                StiRestrictions.AllowChange;
        }
        #endregion
    }
}