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
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Chart
{
    [TypeConverter(typeof(StiUniversalConverter))]
    public abstract class StiAxis : IStiAxis
    {
        #region IStiJsonReportObject.override
        public virtual JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyIdent("Ident", this.GetType().Name);

            jObject.AddPropertyBool(nameof(LogarithmicScale), LogarithmicScale);
            jObject.AddPropertyBool(nameof(AllowApplyStyle), AllowApplyStyle, true);
            jObject.AddPropertyBool(nameof(StartFromZero), StartFromZero, true);
            jObject.AddPropertyJObject(nameof(Interaction), Interaction.SaveToJsonObject(mode));
            jObject.AddPropertyJObject(nameof(Labels), Labels.SaveToJsonObject(mode));
            jObject.AddPropertyJObject(nameof(Ticks), Ticks.SaveToJsonObject(mode));
            jObject.AddPropertyJObject(nameof(Range), Range.SaveToJsonObject(mode));
            jObject.AddPropertyBool(nameof(Visible), Visible, true);
            jObject.AddPropertyEnum(nameof(ArrowStyle), ArrowStyle, StiArrowStyle.None);
            jObject.AddPropertyEnum(nameof(LineStyle), LineStyle, StiPenStyle.Solid);
            jObject.AddPropertyColor(nameof(LineColor), LineColor, Color.Gray);
            jObject.AddPropertyFloat(nameof(LineWidth), LineWidth, 1);

            if (title != null)
                jObject.AddPropertyJObject(nameof(Title), title.SaveToJsonObject(mode));

            return jObject;
        }

        public virtual void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(LogarithmicScale):
                        this.LogarithmicScale = property.DeserializeBool();
                        break;

                    case nameof(AllowApplyStyle):
                        this.AllowApplyStyle = property.DeserializeBool();
                        break;

                    case nameof(StartFromZero):
                        this.StartFromZero = property.DeserializeBool();
                        break;

                    case nameof(Interaction):
                        this.Interaction.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case nameof(Labels):
                        this.Labels.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case nameof(Ticks):
                        this.Ticks.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case nameof(Range):
                        this.Range.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case nameof(Title):
                        this.Title.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case nameof(Visible):
                        this.Visible = property.DeserializeBool();
                        break;

                    case nameof(ArrowStyle):
                        this.ArrowStyle = property.DeserializeEnum<StiArrowStyle>();
                        break;

                    case nameof(LineStyle):
                        this.LineStyle = property.DeserializeEnum<StiPenStyle>(); 
                        break;

                    case nameof(LineColor):
                        this.LineColor = property.DeserializeColor();
                        break;

                    case nameof(LineWidth):
                        this.LineWidth = property.DeserializeFloat();
                        break;
                }
            }
        }
        #endregion

        #region ICloneable override
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
        {
            var axis = this.MemberwiseClone() as IStiAxis;

            axis.ArrowStyle = this.ArrowStyle;
            axis.LineStyle = this.LineStyle;
            axis.Labels = this.Labels.Clone() as IStiAxisLabels;
            axis.Range = this.Range.Clone() as IStiAxisRange;
            axis.Title = this.Title.Clone() as IStiAxisTitle;
            axis.Ticks = this.Ticks.Clone() as IStiAxisTicks;

            if (this.Core != null)
            {
                axis.Core = this.Core.Clone() as StiAxisCoreXF;
                axis.Core.Axis = axis;
            }

            return axis;
        }
        #endregion

        #region IStiDefault
        [Browsable(false)]
        public virtual bool IsDefault
        {
            get
            {
                //We especially don't check LineColor property because its default state controls 
                //by AllowApplySyle property
                return
                    !LogarithmicScale
                    && AllowApplyStyle
                    && StartFromZero
                    && !ShouldSerializeInteraction()
                    && !ShouldSerializeLabels()
                    && !ShouldSerializeRange()
                    && !ShouldSerializeTitle()
                    && !ShouldSerializeTicks()
                    && ArrowStyle == StiArrowStyle.None
                    && LineStyle == StiPenStyle.Solid
                    && LineWidth == 1f
                    && !ShouldSerializeVisible();
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets value which indicates that logarithmic scale will be used.
        /// </summary>
        [StiSerializable]
        [DefaultValue(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates that logarithmic scale will be used.")]
        public virtual bool LogarithmicScale { get; set; }

        [Browsable(false)]
        public StiAxisCoreXF Core { get; set; }

        private bool allowApplyStyle = true;
        /// <summary>
        /// Gets or sets value which indicates that chart style will be used.
        /// </summary>
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates that chart style will be used.")]
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

                    if (value && this.Area != null && this.Area.Chart != null)
                        this.Core.ApplyStyle(this.Area.Chart.Style);
                }
            }
        }

        /// <summary>
        /// Gets or sets value which indicates that all arguments will be shows from zero.
        /// </summary>
        [StiSerializable]
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates that all arguments will be shows from zero.")]
        public virtual bool StartFromZero { get; set; } = true;

        /// <summary>
        /// Gets or sets value which indicates with what steps do labels be shown on axis.
        /// </summary>
        [StiSerializable]
        [DefaultValue(0f)]
        [Obsolete("Step property is obsolete. Please use Labels.Step property instead it.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public virtual float Step
        {
            get
            {
                if (Labels == null)
                    return 0;

                return Labels.Step;
            }
            set
            {
                if (Labels != null)
                    Labels.Step = value;
            }
        }

        /// <summary>
        /// Gets or sets axis interaction settings.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.Class)]
        [TypeConverter(typeof(StiUniversalConverter))]
        public IStiAxisInteraction Interaction { get; set; } = new StiAxisInteraction();

        private bool ShouldSerializeInteraction()
        {
            return Interaction == null || !Interaction.IsDefault;
        }

        /// <summary>
        /// Gets or sets axis labels settings.
        /// </summary>
		[StiSerializable(StiSerializationVisibility.Class)]
        [TypeConverter(typeof(StiUniversalConverter))]
        public IStiAxisLabels Labels { get; set; } = new StiAxisLabels();

        private bool ShouldSerializeLabels()
        {
            return Labels == null || !Labels.IsDefault;
        }

        /// <summary>
        /// Gets or sets axis range settings.
        /// </summary>
		[StiSerializable(StiSerializationVisibility.Class)]
        [TypeConverter(typeof(StiUniversalConverter))]
        public virtual IStiAxisRange Range { get; set; } = new StiAxisRange();

        protected virtual bool ShouldSerializeRange()
        {
            return Range == null || !Range.IsDefault;
        }

        private IStiAxisTitle title;
        /// <summary>
        /// Gets or sets axis title settings.
        /// </summary>
		[StiSerializable(StiSerializationVisibility.Class)]
        [TypeConverter(typeof(StiUniversalConverter))]
        public IStiAxisTitle Title
        {
            get
            {
                if (title == null)
                {
                    title = new StiAxisTitle();

                    if (this is StiXBottomAxis)
                        title.Direction = StiDirection.LeftToRight;

                    else if (this is StiXTopAxis)
                        title.Direction = StiDirection.LeftToRight;

                    else if (this is StiYRightAxis)
                        title.Direction = StiDirection.TopToBottom;

                    else if (this is StiYLeftAxis)
                        title.Direction = StiDirection.BottomToTop;
                }

                return title;
            }
            set
            {
                if (title == null)
                {
                    title = new StiAxisTitle();

                    if (this is StiXBottomAxis)
                        title.Direction = StiDirection.LeftToRight;

                    else if (this is StiXTopAxis)
                        title.Direction = StiDirection.LeftToRight;

                    else if (this is StiYRightAxis)
                        title.Direction = StiDirection.TopToBottom;

                    else if (this is StiYLeftAxis)
                        title.Direction = StiDirection.BottomToTop;
                }

                title = value;
            }
        }

        private bool ShouldSerializeTitle()
        {
            return Title == null || !Title.IsDefault;
        }

        /// <summary>
        /// Gets or sets ticks settings.
        /// </summary>
		[StiSerializable(StiSerializationVisibility.Class)]
        [TypeConverter(typeof(StiUniversalConverter))]
        public IStiAxisTicks Ticks { get; set; } = new StiAxisTicks();

        private bool ShouldSerializeTicks()
        {
            return Ticks == null || !Ticks.IsDefault;
        }

        /// <summary>
        /// Gets or sets style of axis arrow.
        /// </summary>
        [DefaultValue(StiArrowStyle.None)]
        [StiSerializable]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        public StiArrowStyle ArrowStyle { get; set; } = StiArrowStyle.None;

        /// <summary>
        /// Gets or sets line style of axis.
        /// </summary>
		[StiSerializable]
        [DefaultValue(StiPenStyle.Solid)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        public StiPenStyle LineStyle { get; set; } = StiPenStyle.Solid;

        /// <summary>
        /// Gets or sets line color which used to draw axis.
        /// </summary>
		[StiSerializable]
        [TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public Color LineColor { get; set; } = Color.Gray;

        private bool ShouldSerializeLineColor()
        {
            return LineColor != Color.Gray;
        }

        /// <summary>
        /// Gets or sets line width which used to draw axis.
        /// </summary>
        [DefaultValue(1f)]
        [StiSerializable]
        public float LineWidth { get; set; } = 1f;

        /// <summary>
        /// Gets or sets visibility of axis.
        /// </summary>
		[StiSerializable]
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets visibility of axis.")]
        public virtual bool Visible { get; set; } = true;

        protected virtual bool ShouldSerializeVisible()
        {
            return !Visible;
        }

        /// <summary>
        /// 'TitleDirection' property is obsolete. Please Title.Direction property instead it.
        /// </summary>
        [Description("'TitleDirection' property is obsolete. Please Title.Direction property instead it.")]
        [Obsolete("'TitleDirection' property is obsolete. Please Title.Direction property instead it.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public virtual StiLegendDirection TitleDirection
        {
            get
            {
                if (Title == null)
                    return StiLegendDirection.BottomToTop;

                switch (Title.Direction)
                {
                    case StiDirection.BottomToTop:
                        return StiLegendDirection.BottomToTop;

                    case StiDirection.LeftToRight:
                        return StiLegendDirection.LeftToRight;

                    case StiDirection.RightToLeft:
                        return StiLegendDirection.RightToLeft;

                    case StiDirection.TopToBottom:
                        return StiLegendDirection.TopToBottom;
                }

                return StiLegendDirection.BottomToTop;
            }
            set
            {
                if (Title == null) return;

                switch (value)
                {
                    case StiLegendDirection.BottomToTop:
                        Title.Direction = StiDirection.BottomToTop;
                        break;

                    case StiLegendDirection.LeftToRight:
                        Title.Direction = StiDirection.LeftToRight;
                        break;

                    case StiLegendDirection.RightToLeft:
                        Title.Direction = StiDirection.RightToLeft;
                        break;

                    case StiLegendDirection.TopToBottom:
                        Title.Direction = StiDirection.TopToBottom;
                        break;
                }
            }
        }

        [StiSerializable(StiSerializationVisibility.Reference)]
        [Browsable(false)]
        public IStiAxisArea Area { get; set; }

        [Browsable(false)]
        public StiAxisInfoXF Info { get; set; } = new StiAxisInfoXF();
        #endregion

        public StiAxis()
        {
        }

        public StiAxis(
            IStiAxisLabels labels,
            IStiAxisRange range,
            IStiAxisTitle title,
            IStiAxisTicks ticks,
            StiArrowStyle arrowStyle,
            StiPenStyle lineStyle,
            Color lineColor,
            float lineWidth,
            bool visible,
            bool startFromZero,
            bool allowApplyStyle)
        {
            this.Labels = labels;
            this.Range = range;
            this.title = title;
            this.Ticks = ticks;
            this.ArrowStyle = arrowStyle;
            this.LineStyle = lineStyle;
            this.LineColor = lineColor;
            this.LineWidth = lineWidth;
            this.Visible = visible;
            this.StartFromZero = startFromZero;
            this.allowApplyStyle = allowApplyStyle;
        }

        public StiAxis(
            IStiAxisLabels labels,
            IStiAxisRange range,
            IStiAxisTitle title,
            IStiAxisTicks ticks,
            IStiAxisInteraction interaction,
            StiArrowStyle arrowStyle,
            StiPenStyle lineStyle,
            Color lineColor,
            float lineWidth,
            bool visible,
            bool startFromZero,
            bool allowApplyStyle)
        {
            this.Labels = labels;
            this.Range = range;
            this.title = title;
            this.Ticks = ticks;
            this.Interaction = interaction;
            this.ArrowStyle = arrowStyle;
            this.LineStyle = lineStyle;
            this.LineColor = lineColor;
            this.LineWidth = lineWidth;
            this.Visible = visible;
            this.StartFromZero = startFromZero;
            this.allowApplyStyle = allowApplyStyle;
        }

        public StiAxis(
            IStiAxisLabels labels,
            IStiAxisRange range,
            IStiAxisTitle title,
            IStiAxisTicks ticks,
            IStiAxisInteraction interaction,
            StiArrowStyle arrowStyle,
            StiPenStyle lineStyle,
            Color lineColor,
            float lineWidth,
            bool visible,
            bool startFromZero,
            bool allowApplyStyle,
            bool logarithmicScale)
        {
            this.Labels = labels;
            this.Range = range;
            this.title = title;
            this.Ticks = ticks;
            this.Interaction = interaction;
            this.ArrowStyle = arrowStyle;
            this.LineStyle = lineStyle;
            this.LineColor = lineColor;
            this.LineWidth = lineWidth;
            this.Visible = visible;
            this.StartFromZero = startFromZero;
            this.allowApplyStyle = allowApplyStyle;
            this.LogarithmicScale = logarithmicScale;
        }
    }
}
