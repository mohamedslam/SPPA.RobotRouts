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
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Components.Design;
using Stimulsoft.Report.Components.Gauge.Primitives;
using Stimulsoft.Report.Engine;
using Stimulsoft.Report.Gauge.Collections;
using Stimulsoft.Report.Painters;
using Stimulsoft.Report.PropertyGrid;
using System;
using System.Linq;
using System.Collections;
using System.ComponentModel;
using Stimulsoft.Report.Components.TextFormats;
using System.Drawing.Design;
using Stimulsoft.Base.Drawing.Design;
using System.Collections.Generic;
using Stimulsoft.Base.Context.Animation;
using System.Globalization;
using System.Threading;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report.Gauge
{
    [StiToolbox(true)]
    [StiServiceBitmap(typeof(StiComponent), "Stimulsoft.Report.Images.Components.StiGauge.png")]
    [StiServiceCategoryBitmap(typeof(StiComponent), "Stimulsoft.Report.Images.Components.catInfographics.png")]
    [StiGdiPainter(typeof(StiGaugeGdiPainter))]
    [StiWpfPainter("Stimulsoft.Report.Painters.StiGaugeWpfPainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
    [StiDesigner("Stimulsoft.Report.Design.Gauge.StiGaugeDesigner, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfDesigner("Stimulsoft.Report.WpfDesign.StiWpfGaugeDesigner, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
    [StiV2Builder(typeof(StiGaugeV2Builder))]
    [StiContextTool(typeof(IStiComponentDesigner))]
    public class StiGauge : 
        StiComponent,
        IStiExportImageExtended,
        IStiBorder,
        IStiBrush
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // NonSerialized
            jObject.RemoveProperty(nameof(ComponentStyle));
            jObject.RemoveProperty(nameof(UseParentStyles));
            jObject.RemoveProperty(nameof(CanGrow));
            jObject.RemoveProperty(nameof(CanShrink));
            jObject.RemoveProperty(nameof(GrowToHeight));
            jObject.RemoveProperty(nameof(Interaction));
            jObject.RemoveProperty(nameof(IsAnimation));

            // StiGauge
            jObject.AddPropertyBool(nameof(ShortValue), ShortValue, true);
            jObject.AddPropertyDecimal(nameof(Minimum), Minimum, 0M);
            jObject.AddPropertyDecimal(nameof(Maximum), Maximum, 100M);
            jObject.AddPropertyStringNullOrEmpty(nameof(CustomStyleName), CustomStyleName);
            jObject.AddPropertyBorder(nameof(Border), Border);
            jObject.AddPropertyBrush(nameof(Brush), Brush);
            jObject.AddPropertyEnum(nameof(Type), Type, StiGaugeType.FullCircular);
            jObject.AddPropertyEnum(nameof(CalculationMode), CalculationMode, StiGaugeCalculationMode.Auto);
            jObject.AddPropertyEnum(nameof(Mode), Mode, StiScaleMode.V2);
            
            jObject.AddPropertyJObject(nameof(Scales), Scales.SaveToJsonObject(mode));
            jObject.AddPropertyBool(nameof(AllowApplyStyle), AllowApplyStyle, true);
            jObject.AddPropertyJObject(nameof(Style), Style.SaveToJsonObject(mode));

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(CustomStyleName):
                        this.CustomStyleName = property.DeserializeString();
                        break;

                    case nameof(Style):
                        this.Style = StiGaugeStyleXF.CreateFromJsonObject((JObject)property.Value);
                        break;

                    case nameof(Type):
                        this.Type = property.DeserializeEnum<StiGaugeType>();
                        break;

                    case nameof(ShortValue):
                        this.ShortValue = property.DeserializeBool();
                        break;

                    case nameof(Minimum):
                        this.Minimum = property.DeserializeDecimal();
                        break;

                    case nameof(Maximum):
                        this.Maximum = property.DeserializeDecimal();
                        break;

                    case nameof(CalculationMode):
                        this.CalculationMode = property.DeserializeEnum<StiGaugeCalculationMode>();
                        break;

                    case nameof(Mode):
                        this.Mode = property.DeserializeEnum<StiScaleMode>();
                        break;

                    case nameof(AllowApplyStyle):
                        this.AllowApplyStyle = property.DeserializeBool();
                        break;

                    case nameof(Border):
                        this.Border = property.DeserializeBorder();
                        break;

                    case nameof(Brush):
                        this.Brush = property.DeserializeBrush();
                        break;
                                                                      
                    case nameof(Scales):
                        this.Scales.LoadFromJsonObject((JObject)property.Value);
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiGauge;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var collection = new StiPropertyCollection();

            collection.Add(StiPropertyCategories.Value, new[]
            {
                propHelper.AllowApplyStyle(),
                propHelper.ShortValue(),
            });

            if (level == StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.Position, new[]
                {
                    propHelper.Left(),
                    propHelper.Top(),
                    propHelper.Width(),
                    propHelper.Height(),
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.Position, new[]
                {
                    propHelper.Left(),
                    propHelper.Top(),
                    propHelper.Width(),
                    propHelper.Height(),
                    propHelper.MinSize(),
                    propHelper.MaxSize()
                });
            }
            
            collection.Add(StiPropertyCategories.Appearance, new[]
            {
                propHelper.Brush(),
                propHelper.Border(),
                propHelper.Conditions()
            });

            if (level == StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.Behavior, new[]
                {
                    propHelper.Enabled()
                });
            }
            else if (level == StiLevel.Standard)
            {
                collection.Add(StiPropertyCategories.Behavior, new[]
                {
                    propHelper.AnchorMode(),
                    propHelper.DockStyle(),
                    propHelper.Enabled(),
                    propHelper.PrintOn(),
                    propHelper.ShiftMode()
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.Behavior, new[]
                {
                    propHelper.AnchorMode(),
                    propHelper.DockStyle(),
                    propHelper.Enabled(),
                    propHelper.Printable(),
                    propHelper.PrintOn(),
                    propHelper.ShiftMode()
                });
            }
            
            if (level == StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.Design, new[]
                {
                    propHelper.Name()
                });
            }
            else if (level == StiLevel.Standard)
            {
                collection.Add(StiPropertyCategories.Design, new[]
                {
                    propHelper.Name(),
                    propHelper.Alias()
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.Design, new[]
                {
                    propHelper.Name(),
                    propHelper.Alias(),
                    propHelper.Restrictions(),
                    propHelper.Locked(),
                    propHelper.Linked()
                });
            }

            return collection;
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
                    StiPropertyCategories.ValueEvents,
                    new[]
                    {
                        StiPropertyEventId.GetTagEvent,
                        StiPropertyEventId.GetToolTipEvent,
                    }
                }
            };
        }
        #endregion

        #region StiComponent.Properties
        public override string HelpUrl => "user-manual/introduction.htm";
        #endregion

        #region ICloneable override
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone(bool cloneProperties)
        {
            var gauge = (StiGauge)base.Clone(cloneProperties);
            gauge.Scales = new StiScaleCollection(gauge);

            lock (((ICollection)this.Scales).SyncRoot)
            {
                foreach (StiScaleBase scale in this.Scales) 
                    gauge.Scales.Add((StiScaleBase)scale.Clone());
            }

            return gauge;
        }
        #endregion		

        #region IStiExportImageExtended
        public virtual Image GetImage(ref float zoom)
        {
            return GetImage(ref zoom, StiExportFormat.None);
        }

        public virtual Image GetImage(ref float zoom, StiExportFormat format)
        {
            var painter = StiPainter.GetPainter(this.GetType(), StiGuiMode.Gdi);
            return painter.GetImage(this, ref zoom, format);
        }

        [Browsable(false)]
        public override bool IsExportAsImage(StiExportFormat format)
        {
            return format != StiExportFormat.Pdf;
        }
        #endregion

        #region IStiBorder
        /// <summary>
        /// The appearance and behavior of the component border.
        /// </summary>
        [StiCategory("Appearance")]
        [StiOrder(StiPropertyOrder.AppearanceBorder)]
        [StiSerializable]
        [Description("The appearance and behavior of the component border.")]
        public StiBorder Border { get; set; } = new StiBorder();
        #endregion

        #region IStiBrush
        /// <summary>
        /// The brush, which is used to draw background.
        /// </summary>
        [StiCategory("Appearance")]
        [StiOrder(StiPropertyOrder.AppearanceBrush)]
        [StiSerializable]
        [Description("The brush, which is used to draw background.")]
        [TypeConverter(typeof(StiExpressionBrushConverter))]
        [Editor(StiEditors.ExpressionBrush, typeof(UITypeEditor))]
        [StiExpressionAllowed]
        public StiBrush Brush { get; set; } = new StiSolidBrush(Color.Transparent);

        private bool ShouldSerializeBrush()
        {
            return !(Brush is StiSolidBrush && ((StiSolidBrush)Brush).Color == Color.Transparent);
        }
        #endregion

        #region IStiTextFormat
        /// <summary>
        /// Gets or sets the format of the text.
        /// </summary>
        [Editor("Stimulsoft.Report.Components.TextFormats.Design.StiTextFormatGaugeElementEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiSerializable(StiSerializationVisibility.Class)]
        [StiCategory("Behavior")]
        [StiOrder(StiPropertyOrder.BehaviorValueFormat)]
        [Description("Gets or sets the format of the text.")]
        [StiPropertyLevel(StiLevel.Standard)]
        [Browsable(false)]
        public virtual StiFormatService ValueFormat { get; set; } = GetValueFormatDefault();

        private static StiFormatService GetValueFormatDefault()
        {
            return new StiNumberFormatService
            {
                DecimalDigits = 0,
                State = Stimulsoft.Report.Components.StiTextFormatState.DecimalDigits | Stimulsoft.Report.Components.StiTextFormatState.Abbreviation
            };
        }
        #endregion

        #region StiComponent override
        public override int ToolboxPosition => (int)StiComponentToolboxPosition.Gauge;

        /// <summary>
        /// Gets a localized name of the component category.
        /// </summary>
        public override string LocalizedCategory => StiLocalization.Get("Components", "StiGauge");

        public override string LocalizedName => StiLocalization.Get("Components", "StiGauge");

        public override RectangleD DefaultClientRectangle => new RectangleD(0, 0, 140, 140);
        #endregion

        #region Browsable(false)
        [StiNonSerialized]
        [Browsable(false)]
        public override string ComponentStyle
        {
            get
            {
                return base.ComponentStyle;
            }
            set
            {

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
                
            }
        }

        [StiNonSerialized]
        [Browsable(false)]
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

        [StiNonSerialized]
        [Browsable(false)]
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

        [StiNonSerialized]
        [Browsable(false)]
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

        #region Properties
        [StiSerializable]
        [DefaultValue(StiScaleMode.V2)]
        [Browsable(false)]
        [TypeConverter(typeof(StiEnumConverter))]
        public StiScaleMode Mode { get; set; } = StiScaleMode.V2;

        [DefaultValue(true)]
        [StiSerializable]
        [StiCategory("Gauge")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Basic)]
        [StiOrder(StiPropertyOrder.GaugeShortValue)]
        public bool ShortValue { get; set; } = true;

        [StiSerializable]
        [DefaultValue(0d)]
        [Browsable(false)]
        public decimal Minimum { get; set; }

        [StiSerializable]
        [DefaultValue(100d)]
        [Browsable(false)]
        public decimal Maximum { get; set; } = 100M;

        [StiSerializable]
        [DefaultValue(StiGaugeType.FullCircular)]
        [Browsable(false)]
        [TypeConverter(typeof(StiEnumConverter))]
        public StiGaugeType Type { get; set; } = StiGaugeType.FullCircular;

        [StiSerializable]
        [DefaultValue(StiGaugeCalculationMode.Auto)]
        [Browsable(false)]
        [TypeConverter(typeof(StiEnumConverter))]
        public StiGaugeCalculationMode CalculationMode { get; set; } = StiGaugeCalculationMode.Auto;

        [Browsable(false)]
        public StiGaugeContextPainter Painter { get; set; }

        private IStiGaugeStyle style = new StiGaugeStyleXF26();
        /// <summary>
        /// Gets or sets style of the chart.
        /// </summary>
        [TypeConverter(typeof(Design.StiGaugeStyleConverter))]
        [StiSerializable(StiSerializationVisibility.Class)]
        [Browsable(false)]
        [Description("Gets or sets style of the chart.")]
        public IStiGaugeStyle Style
        {
            get
            {
                return style;
            }
            set
            {
                if (style == value) return;

                style = value;
                if (value != null)
                    value.Core.Gauge = this;
            }
        }

        private bool allowApplyStyle = true;
        /// <summary>
        /// Gets or sets value which indicates that gauge style will be used.
        /// </summary>
        [StiSerializable]
        [StiCategory("Appearance")]
        [StiOrder(StiPropertyOrder.AppearanceAllowApplyStyle)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates that gauge style will be used.")]
        [DefaultValue(true)]
        public bool AllowApplyStyle
        {
            get
            {
                return allowApplyStyle;
            }
            set
            {
                if (allowApplyStyle != value)
                {
                    allowApplyStyle = value;
                    if (value)
                        this.ApplyStyle(Style);
                }
            }
        }

        [StiSerializable]
        [Browsable(false)]
        public string CustomStyleName { get; set; } = "";

        [Browsable(false)]
        [StiSerializable(StiSerializationVisibility.List)]
        public StiScaleCollection Scales { get; set; }

        [StiNonSerialized]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Browsable(false)]
        public bool IsAnimation { get; set; } = true;

        [Browsable(false)]
        public List<StiAnimation> PreviousAnimations { get; set; }
        #endregion

        #region Methods override
        internal IStiGaugeStyle GetGaugeStyle()
        {
            if (!string.IsNullOrEmpty(this.CustomStyleName))
            {
                if (this.Report != null)
                {
                    var style = this.Report.Styles.ToList().FirstOrDefault(x => x.Name == this.CustomStyleName) as StiGaugeStyle;
                    if (style != null)
                        return new StiCustomGaugeStyle(style);
                }
            }

            return this.Style;
        }

        public void DrawGauge(StiGaugeContextPainter context)
        {
            this.ApplyStyle(GetGaugeStyle());

            DrawGaugeInternal(context);
        }

        public void DrawGaugeInternal(StiGaugeContextPainter context)
        {
            var index = -1;
            while (++index < this.Scales.Count)
            {
                var scale = this.Scales[index];
                if (scale != null)
                {
                    scale.barGeometry.CheckRectGeometry(context.Rect);
                    scale.DrawElement(context);
                }
            }
        }

        public override StiComponent CreateNew()
        {
            return new StiGauge();
        }

        public void ApplyStyle(IStiGaugeStyle style)
        {
            if (AllowApplyStyle)
            {
                this.Brush = style.Core.Brush;
                this.Border.Color = style.Core.BorderColor;
                this.Border.Size = style.Core.BorderWidth;
            }

            foreach (StiScaleBase scale in this.Scales)
            {
                scale.ApplyStyle(style);
            }
        }
        #endregion

        /// <summary>
		/// Creates a new StiGauge.
		/// </summary>
		public StiGauge() : this(RectangleD.Empty)
		{
        }

		/// <summary>
		/// Creates a new StiGauge.
		/// </summary>
		/// <param name="rect">The rectangle describes size and position of the component.</param>
		public StiGauge(RectangleD rect) 
            : base(rect)
		{
            Scales = new StiScaleCollection(this);
            PlaceOnToolbox = true;
            PreviousAnimations = new List<StiAnimation>();
        }
    }
}