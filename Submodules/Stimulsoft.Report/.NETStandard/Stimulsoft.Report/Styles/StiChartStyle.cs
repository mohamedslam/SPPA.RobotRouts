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
using System.IO;
using Stimulsoft.Base;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Drawing.Design;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Report.Design;
using System.Linq;
using Stimulsoft.Report.Images;
using System.Drawing;
using System.Drawing.Drawing2D;
using Stimulsoft.Base.Design;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using Pen = Stimulsoft.Drawing.Pen;
using Font = Stimulsoft.Drawing.Font;
using StringFormat = Stimulsoft.Drawing.StringFormat;
#endif

namespace Stimulsoft.Report
{
    /// <summary>
    /// Describes the class that contains a style for Chart components.
    /// </summary>	
    public class StiChartStyle : StiBaseStyle
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiChartStyle
            jObject.AddPropertyBorder("Border", Border);
            jObject.AddPropertyBrush("Brush", Brush);
            jObject.AddPropertyBrush("ChartAreaBrush", ChartAreaBrush);
            jObject.AddPropertyColor("ChartAreaBorderColor", ChartAreaBorderColor, Color.FromArgb(171, 172, 173));
            jObject.AddPropertyInt(nameof(ChartAreaBorderThickness), ChartAreaBorderThickness, 1);
            jObject.AddPropertyBool("ChartAreaShowShadow", ChartAreaShowShadow);
            jObject.AddPropertyBool("SeriesLighting", SeriesLighting);
            jObject.AddPropertyBool("SeriesShowShadow", SeriesShowShadow);
            jObject.AddPropertyBool("SeriesShowBorder", SeriesShowBorder);
            jObject.AddPropertyInt("SeriesBorderThickness", SeriesBorderThickness, 1);
            jObject.AddPropertyBrush("SeriesLabelsBrush", SeriesLabelsBrush);
            jObject.AddPropertyColor("SeriesLabelsColor", SeriesLabelsColor, Color.FromArgb(90, 90, 90));
            jObject.AddPropertyColor("SeriesLabelsLineColor", SeriesLabelsLineColor, Color.FromArgb(140, 140, 140));
            jObject.AddPropertyColor("SeriesLabelsBorderColor", SeriesLabelsBorderColor, Color.FromArgb(90, 90, 90));
            jObject.AddPropertyColor("TrendLineColor", TrendLineColor, Color.FromArgb(140, 140, 140));
            jObject.AddPropertyBool("TrendLineShowShadow", TrendLineShowShadow);
            jObject.AddPropertyBrush("LegendBrush", LegendBrush);
            jObject.AddPropertyColor("LegendLabelsColor", LegendLabelsColor, Color.FromArgb(140, 140, 140));
            jObject.AddPropertyColor("LegendBorderColor", LegendBorderColor, Color.DimGray);
            jObject.AddPropertyColor("LegendTitleColor", LegendTitleColor, Color.DimGray);
            jObject.AddPropertyColor("AxisTitleColor", AxisTitleColor, Color.FromArgb(140, 140, 140));
            jObject.AddPropertyColor("AxisLineColor", AxisLineColor, Color.FromArgb(140, 140, 140));
            jObject.AddPropertyColor("AxisLabelsColor", AxisLabelsColor, Color.FromArgb(140, 140, 140));
            jObject.AddPropertyBool("MarkerVisible", MarkerVisible, true);
            jObject.AddPropertyBrush("InterlacingHorBrush", InterlacingHorBrush);
            jObject.AddPropertyBrush("InterlacingVertBrush", InterlacingVertBrush);
            jObject.AddPropertyColor("GridLinesHorColor", GridLinesHorColor, Color.FromArgb(100, 105, 105, 105));
            jObject.AddPropertyColor("GridLinesVertColor", GridLinesVertColor, Color.FromArgb(100, 105, 105, 105));
            jObject.AddPropertyEnum("BrushType", BrushType, StiBrushType.Solid);
            jObject.AddPropertyArrayColor("StyleColors", StyleColors);
            jObject.AddPropertyColor("BasicStyleColor", BasicStyleColor, Color.WhiteSmoke);
            jObject.AddPropertyBool("AllowUseBorderFormatting", AllowUseBorderFormatting, true);
            jObject.AddPropertyBool("AllowUseBorderSides", AllowUseBorderSides, true);
            jObject.AddPropertyBool("AllowUseBrush", AllowUseBrush, true);
            jObject.AddPropertyBrush(nameof(ToolTipBrush), ToolTipBrush);
            jObject.AddPropertyBrush(nameof(ToolTipTextBrush), ToolTipTextBrush);
            jObject.AddPropertyCornerRadius(nameof(ToolTipCornerRadius), ToolTipCornerRadius);
            jObject.AddPropertyBorder(nameof(ToolTipBorder), ToolTipBorder);
            jObject.AddPropertyCornerRadius(nameof(SeriesCornerRadius), SeriesCornerRadius);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Border":
                        this.Border = property.DeserializeBorder();
                        break;

                    case "Brush":
                        this.Brush = property.DeserializeBrush();
                        break;

                    case "ChartAreaBrush":
                        this.ChartAreaBrush = property.DeserializeBrush();
                        break;

                    case "ChartAreaBorderColor":
                        this.ChartAreaBorderColor = property.DeserializeColor();
                        break;

                    case nameof(ChartAreaBorderThickness):
                        this.ChartAreaBorderThickness = property.DeserializeInt();
                        break;

                    case "ChartAreaShowShadow":
                        this.ChartAreaShowShadow = property.DeserializeBool();
                        break;

                    case "SeriesLighting":
                        this.SeriesLighting = property.DeserializeBool();
                        break;

                    case "SeriesShowShadow":
                        this.SeriesShowShadow = property.DeserializeBool();
                        break;

                    case "SeriesShowBorder":
                        this.SeriesShowBorder = property.DeserializeBool();
                        break;

                    case "SeriesBorderThickness":
                        this.SeriesBorderThickness = property.DeserializeInt();
                        break;

                    case nameof(SeriesCornerRadius):
                        this.SeriesCornerRadius = property.DeserializeCornerRadius();
                        break;

                    case "SeriesLabelsBrush":
                        this.SeriesLabelsBrush = property.DeserializeBrush();
                        break;

                    case "SeriesLabelsColor":
                        this.SeriesLabelsColor = property.DeserializeColor();
                        break;

                    case "SeriesLabelsBorderColor":
                        this.SeriesLabelsBorderColor = property.DeserializeColor();
                        break;

                    case "SeriesLabelsLineColor":
                        this.SeriesLabelsLineColor = property.DeserializeColor();
                        break;

                    case "TrendLineColor":
                        this.TrendLineColor = property.DeserializeColor();
                        break;

                    case "TrendLineShowShadow":
                        this.TrendLineShowShadow = property.DeserializeBool();
                        break;

                    case nameof(ToolTipBrush):
                        this.ToolTipBrush = property.DeserializeBrush();
                        break;

                    case nameof(ToolTipTextBrush):
                        this.ToolTipTextBrush = property.DeserializeBrush();
                        break;

                    case nameof(ToolTipCornerRadius):
                        this.ToolTipCornerRadius = property.DeserializeCornerRadius();
                        break;

                    case nameof(ToolTipBorder):
                        this.ToolTipBorder = property.DeserializeSimpleBorder();
                            break;

                    case "LegendBrush":
                        this.LegendBrush = property.DeserializeBrush();
                        break;

                    case "LegendLabelsColor":
                        this.LegendLabelsColor = property.DeserializeColor();
                        break;

                    case "LegendBorderColor":
                        this.LegendBorderColor = property.DeserializeColor();
                        break;

                    case "LegendTitleColor":
                        this.LegendTitleColor = property.DeserializeColor();
                        break;

                    case "AxisTitleColor":
                        this.AxisTitleColor = property.DeserializeColor();
                        break;

                    case "AxisLineColor":
                        this.AxisLineColor = property.DeserializeColor();
                        break;

                    case "AxisLabelsColor":
                        this.AxisLabelsColor = property.DeserializeColor();
                        break;

                    case "MarkerVisible":
                        this.MarkerVisible = property.DeserializeBool();
                        break;

                    case "InterlacingHorBrush":
                        this.InterlacingHorBrush = property.DeserializeBrush();
                        break;

                    case "InterlacingVertBrush":
                        this.InterlacingVertBrush = property.DeserializeBrush();
                        break;

                    case "GridLinesHorColor":
                        this.GridLinesHorColor = property.DeserializeColor();
                        break;

                    case "GridLinesVertColor":
                        this.GridLinesVertColor = property.DeserializeColor();
                        break;

                    case "BrushType":
                        this.BrushType = property.DeserializeEnum<StiBrushType>();
                        break;

                    case "StyleColors":
                        this.StyleColors = property.DeserializeArrayColor();
                        break;

                    case "BasicStyleColor":
                        this.BasicStyleColor = property.DeserializeColor();
                        break;

                    case "AllowUseBorderFormatting":
                        this.AllowUseBorderFormatting = property.DeserializeBool();
                        break;

                    case "AllowUseBorderSides":
                        this.AllowUseBorderSides = property.DeserializeBool();
                        break;

                    case "AllowUseBrush":
                        this.AllowUseBrush = property.DeserializeBool();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiChartStyle;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            var list = new[]
            {
                propHelper.StyleName(),
                propHelper.Description(),
                propHelper.StyleCollectionName(),
                propHelper.StyleConditions()
            };
            objHelper.Add(StiPropertyCategories.Main, list);

            list = new[]
            {
                propHelper.Brush(),
                propHelper.Border()
            };
            objHelper.Add(StiPropertyCategories.Appearance, list);

            list = new[]
            {
                propHelper.BasicStyleColor(),
                propHelper.StyleColors(),
                propHelper.AllowUseBorderFormatting(),
                propHelper.AllowUseBorderSides(),
                propHelper.AllowUseBrush(),
                propHelper.BrushType()
            };
            objHelper.Add(StiPropertyCategories.Parameters, list);

            list = new[]
            {
                propHelper.ChartAreaShowShadow(),
                propHelper.ChartAreaBorderColor(),
                propHelper.ChartAreaBrush(),
            };
            objHelper.Add(StiPropertyCategories.Area, list);

            list = new[]
            {
                propHelper.SeriesLighting(),
                propHelper.SeriesShowShadow()
            };
            objHelper.Add(StiPropertyCategories.Series, list);

            list = new[]
            {
                propHelper.SeriesLabelsBorderColor(),
                propHelper.SeriesLabelsColor(),
                propHelper.SeriesLabelsBrush()
            };
            objHelper.Add(StiPropertyCategories.SeriesLabels, list);

            list = new[]
            {
                propHelper.TrendLineShowShadow(),
                propHelper.TrendLineColor(),
            };
            objHelper.Add(StiPropertyCategories.TrendLine, list);

            list = new[]
            {
                propHelper.LegendBorderColor(),
                propHelper.LegendLabelsColor(),
                propHelper.LegendTitleColor(),
                propHelper.LegendBrush(),
            };
            objHelper.Add(StiPropertyCategories.Legend, list);

            list = new[]
            {
                propHelper.AxisLabelsColor(),
                propHelper.AxisLineColor(),
                propHelper.AxisTitleColor(),
            };
            objHelper.Add(StiPropertyCategories.Axis, list);

            list = new[]
            {
                propHelper.InterlacingHorBrush(),
                propHelper.InterlacingVertBrush(),
            };
            objHelper.Add(StiPropertyCategories.Interlacing, list);

            list = new[]
            {
                propHelper.GridLinesHorColor(),
                propHelper.GridLinesVertColor(),
            };
            objHelper.Add(StiPropertyCategories.GridLines, list);

            return objHelper;
        }
        #endregion

        #region ICloneable
        public override object Clone()
        {
            var style = base.Clone() as StiChartStyle;

            style.Border = (StiBorder)this.Border.Clone();
            style.Brush = (StiBrush)this.Brush.Clone();
            style.ChartAreaBrush = (StiBrush)this.ChartAreaBrush.Clone();
            style.InterlacingHorBrush = (StiBrush)this.InterlacingHorBrush.Clone();
            style.InterlacingVertBrush = (StiBrush)this.InterlacingVertBrush.Clone();
            style.SeriesLabelsBrush = (StiBrush)this.SeriesLabelsBrush.Clone();
            style.LegendBrush = (StiBrush)this.LegendBrush.Clone();
            style.StyleColors = this.StyleColors?.ToArray();
            style.SeriesCornerRadius = (StiCornerRadius)this.SeriesCornerRadius.Clone();
            style.ToolTipBrush = (StiBrush)this.ToolTipBrush.Clone();
            style.ToolTipTextBrush = (StiBrush)this.ToolTipTextBrush.Clone();
            style.ToolTipCornerRadius = (StiCornerRadius)this.ToolTipCornerRadius.Clone();
            style.ToolTipBorder = (StiSimpleBorder)this.ToolTipBorder.Clone();

            return style;
        }
        #endregion

        #region Consts
        private Color[] DefaultColors =
        {
            Color.FromArgb(112, 173, 71),
            Color.FromArgb(68, 114, 196),
            Color.FromArgb(255, 192, 0),
            Color.FromArgb(67, 104, 43),
            Color.FromArgb(253, 106, 55),
            Color.FromArgb(153, 115, 0)
        };
        #endregion

        #region Properties
        /// <summary>
        /// The appearance and behavior of the component border.
        /// </summary>
        [StiCategory("Appearance")]
        [StiSerializable]
        [Description("The appearance and behavior of the component border.")]
        public StiBorder Border { get; set; } = new StiBorder();

        private bool ShouldSerializeBorder()
        {
            return Border == null || !Border.IsDefault;
        }

        /// <summary>
        /// Gets or sets a brush to fill the style.
        /// </summary>
        [StiCategory("Appearance")]
        [StiSerializable]
        [Description("Gets or sets a brush to fill the style.")]
        public StiBrush Brush { get; set; } = new StiSolidBrush(Color.White);

        private bool ShouldSerializeBrush()
        {
            return !(Brush is StiSolidBrush && ((StiSolidBrush)Brush).Color == Color.White);
        }

        #region Properties Area
        [StiCategory("Area")]
        [StiSerializable]
        public StiBrush ChartAreaBrush { get; set; } = new StiSolidBrush(Color.White);

        private bool ShouldSerializeChartAreaBrush()
        {
            return !(ChartAreaBrush is StiSolidBrush && ((StiSolidBrush)ChartAreaBrush).Color == Color.White);
        }

        [StiCategory("Area")]
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public Color ChartAreaBorderColor { get; set; } = Color.FromArgb(171, 172, 173);

        private bool ShouldSerializeChartAreaBorderColor()
        {
            return ChartAreaBorderColor != Color.FromArgb(171, 172, 173);
        }

        [StiCategory("Area")]
        [StiSerializable]
        [DefaultValue(1)]
        public int ChartAreaBorderThickness { get; set; } = 1;

        private bool ShouldChartAreaBorderThickness()
        {
            return this.ChartAreaBorderThickness != 1;
        }

        [StiCategory("Area")]
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [DefaultValue(false)]
        public bool ChartAreaShowShadow { get; set; }

        private bool ShouldSerializeChartAreaShowShadow()
        {
            return this.ChartAreaShowShadow;
        }

        [StiCategory("Area")]
        [StiSerializable]
        public StiBrush InterlacingHorBrush { get; set; } = new StiSolidBrush(Color.FromArgb(10, 155, 155, 155));

        private bool ShouldSerializeInterlacingHorBrush()
        {
            return !(InterlacingHorBrush is StiSolidBrush && ((StiSolidBrush)InterlacingHorBrush).Color == Color.FromArgb(10, 155, 155, 155));
        }

        [StiCategory("Area")]
        [StiSerializable]
        public StiBrush InterlacingVertBrush { get; set; } = new StiSolidBrush(Color.FromArgb(10, 155, 155, 155));

        private bool ShouldSerializeInterlacingVertBrush()
        {
            return !(InterlacingVertBrush is StiSolidBrush && ((StiSolidBrush)InterlacingVertBrush).Color == Color.FromArgb(10, 155, 155, 155));
        }

        [StiCategory("Area")]
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public Color GridLinesHorColor { get; set; } = Color.FromArgb(100, 105, 105, 105);

        private bool ShouldSerializeGridLinesHorColor()
        {
            return GridLinesHorColor != Color.FromArgb(100, 105, 105, 105);
        }

        [StiCategory("Area")]
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public Color GridLinesVertColor { get; set; } = Color.FromArgb(100, 105, 105, 105);

        private bool ShouldSerializeGridLinesVertColor()
        {
            return GridLinesVertColor != Color.FromArgb(100, 105, 105, 105);
        }
        #endregion

        #region Properties Series
        [StiCategory("Series")]
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [DefaultValue(false)]
        public bool SeriesLighting { get; set; }

        private bool ShouldSerializeSeriesLighting()
        {
            return this.SeriesLighting;
        }

        [StiCategory("Series")]
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [DefaultValue(false)]
        public bool SeriesShowShadow { get; set; }

        private bool ShouldSerializeSeriesShowShadow()
        {
            return this.SeriesShowShadow;
        }

        [StiCategory("Series")]
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [DefaultValue(false)]
        public bool SeriesShowBorder { get; set; }

        private bool ShouldSeriesShowBorder()
        {
            return this.SeriesShowBorder;
        }

        [StiCategory("Series")]
        [StiSerializable]
        [DefaultValue(1)]
        public int SeriesBorderThickness { get; set; } = 1;

        private bool ShouldSeriesBorderThickness()
        {
            return this.SeriesBorderThickness != 1;
        }

        [StiCategory("Series")]
        [StiSerializable]
        [Description("Represents the value to which the corners are rounded.")]
        public StiCornerRadius SeriesCornerRadius { get; set; } = new StiCornerRadius();

        private bool ShouldSeriesCornerRadius()
        {
            return this.SeriesCornerRadius != null && !this.SeriesCornerRadius.IsEmpty;
        }
        #endregion

        #region Properties Labels
        [StiCategory("Labels")]
        [StiSerializable]
        public StiBrush SeriesLabelsBrush { get; set; } = new StiSolidBrush(Color.White);

        private bool ShouldSerializeSeriesLabelsBrush()
        {
            return !(SeriesLabelsBrush is StiSolidBrush && ((StiSolidBrush)SeriesLabelsBrush).Color == Color.White);
        }

        [StiCategory("Labels")]
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public Color SeriesLabelsColor { get; set; } = Color.FromArgb(90, 90, 90);

        private bool ShouldSerializeSeriesLabelsColor()
        {
            return SeriesLabelsColor != Color.FromArgb(90, 90, 90);
        }
        
        [StiCategory("Labels")]
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public Color SeriesLabelsBorderColor { get; set; } = Color.FromArgb(140, 140, 140);

        private bool ShouldSerializeSeriesLabelsBorderColor()
        {
            return SeriesLabelsBorderColor != Color.FromArgb(140, 140, 140);
        }

        [StiCategory("Labels")]
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public Color SeriesLabelsLineColor { get; set; } = Color.FromArgb(140, 140, 140);

        private bool ShouldSerializeSeriesLabelsLineColor()
        {
            return SeriesLabelsLineColor != Color.FromArgb(140, 140, 140);
        }
        #endregion

        #region Properties Tooltip
        [StiCategory("ToolTip")]
        [StiSerializable]
        public StiBrush ToolTipBrush { get; set; } = new StiSolidBrush(Color.White);

        private bool ShouldSerializeToolTipBrush()
        {
            return !(ToolTipBrush is StiSolidBrush && ((StiSolidBrush)ToolTipBrush).Color == Color.White);
        }

        [StiCategory("ToolTip")]
        [StiSerializable]
        public StiBrush ToolTipTextBrush { get; set; } = new StiSolidBrush(Color.Black);

        private bool ShouldSerializeToolTipTextBrush()
        {
            return !(ToolTipTextBrush is StiSolidBrush && ((StiSolidBrush)ToolTipTextBrush).Color == Color.Black);
        }

        [StiCategory("ToolTip")]
        [StiSerializable]
        public StiCornerRadius ToolTipCornerRadius { get; set; } = new StiCornerRadius();

        private bool ShouldSerializeToolTipCornerRadius()
        {
            return ToolTipCornerRadius == null || !ToolTipCornerRadius.IsDefault;
        }

        [StiCategory("ToolTip")]
        [StiSerializable]
        public StiSimpleBorder ToolTipBorder { get; set; } = new StiSimpleBorder();

        private bool ShouldSerializeToolTipBorder()
        {
            return !ToolTipBorder.IsDefault;
        }
        #endregion

        #region Properties Trend Line
        [StiCategory("TrendLine")]
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public Color TrendLineColor { get; set; } = Color.FromArgb(140, 140, 140);

        private bool ShouldSerializeTrendLineColor()
        {
            return TrendLineColor != Color.FromArgb(140, 140, 140);
        }

        [StiCategory("TrendLine")]
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [DefaultValue(false)]
        public bool TrendLineShowShadow { get; set; }

        private bool ShouldSerializeTrendLineShowShadow()
        {
            return TrendLineShowShadow;
        }
        #endregion

        #region Properties Legend
        [StiCategory("Legend")]
        [StiSerializable]
        public StiBrush LegendBrush { get; set; } = new StiSolidBrush(Color.White);

        private bool ShouldSerializeLegendBrush()
        {
            return !(LegendBrush is StiSolidBrush && ((StiSolidBrush)LegendBrush).Color == Color.White);
        }

        [StiCategory("Legend")]
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public Color LegendLabelsColor { get; set; } = Color.FromArgb(140, 140, 140);

        private bool ShouldSerializeLegendLabelsColor()
        {
            return LegendLabelsColor != Color.FromArgb(140, 140, 140);
        }

        [StiCategory("Legend")]
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public Color LegendBorderColor { get; set; } = Color.DimGray;

        private bool ShouldSerializeLegendBorderColor()
        {
            return LegendBorderColor != Color.DimGray;
        }

        [StiCategory("Legend")]
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public Color LegendTitleColor { get; set; } = Color.DimGray;

        private bool ShouldSerializeLegendTitleColor()
        {
            return LegendTitleColor != Color.DimGray;
        }
        #endregion

        #region Properties Axis
        [StiCategory("Axis")]
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public Color AxisTitleColor { get; set; } = Color.FromArgb(140, 140, 140);

        private bool ShouldSerializeAxisTitleColor()
        {
            return AxisTitleColor != Color.FromArgb(140, 140, 140);
        }

        [StiCategory("Axis")]
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public Color AxisLineColor { get; set; } = Color.FromArgb(140, 140, 140);

        private bool ShouldSerializeAxisLineColor()
        {
            return AxisLineColor != Color.FromArgb(140, 140, 140);
        }

        [StiCategory("Axis")]
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public Color AxisLabelsColor { get; set; } = Color.FromArgb(140, 140, 140);

        private bool ShouldSerializeAxisLabelsColor()
        {
            return AxisLabelsColor != Color.FromArgb(140, 140, 140);
        }
        #endregion

        [StiCategory("Appearance")]
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [DefaultValue(true)]
        public bool MarkerVisible { get; set; } = true;

        private bool ShouldSerializeMarkerVisible()
        {
            return MarkerVisible != true;
        }

        /// <summary>
        /// Gets or sets a value which indicates which type of brush report engine will be used to draw this style.
        /// </summary>
        [StiSerializable]
        [StiCategory("Appearance")]
        [DefaultValue(StiBrushType.Solid)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("Gets or sets a value which indicates which type of brush report engine will be used to draw this style.")]
        public StiBrushType BrushType { get; set; } = StiBrushType.Solid;

        private bool ShouldSerializeBrushType()
        {
            return BrushType != StiBrushType.Solid;
        }

        /// <summary>
        /// Gets or sets a list of colors which will be used for drawing chart series.
        /// </summary>
        [StiCategory("Appearance")]
        [StiSerializable]
        [Description("Gets or sets a list of colors which will be used for drawing chart series.")]
        [TypeConverter(typeof(StiStyleColorsConverter))]
        [Editor("Stimulsoft.Report.Components.Design.StiColorsCollectionEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public Color[] StyleColors { get; set; }

        private bool ShouldSerializeStyleColors()
        {
            return StyleColors == null || 
                StyleColors.Length != DefaultColors.Length ||
                !StyleColors.ToList().SequenceEqual(DefaultColors);
        }

        /// <summary>
        /// Gets or sets a base color for drawing this style. 
        /// </summary>
        [StiCategory("Appearance")]
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [Description("Gets or sets a base color for drawing this style.")]
        public Color BasicStyleColor { get; set; } = Color.WhiteSmoke;

        private bool ShouldSerializeBasicStyleColor()
        {
            return BasicStyleColor != Color.WhiteSmoke;
        }

        /// <summary>
        /// Gets or sets a value which indicates whether a report engine can use Border formatting or not. 
        /// </summary>
        [StiSerializable]
        [StiCategory("Parameters")]
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets a value which indicates whether a report engine can use Border formatting or not.")]
        public bool AllowUseBorderFormatting { get; set; } = true;

        private bool ShouldAllowUseBorderFormatting()
        {
            return AllowUseBorderFormatting != true;
        }

        /// <summary>
        /// Gets or sets a value which indicates whether a report engine can use Border Sides or not. 
        /// </summary>
        [StiSerializable]
        [StiCategory("Parameters")]
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets a value which indicates whether a report engine can use Border Sides or not.")]
        public bool AllowUseBorderSides { get; set; } = true;

        private bool ShouldAllowAllowUseBorderSides()
        {
            return AllowUseBorderSides != true;
        }

        /// <summary>
        /// Gets or sets a value which indicates whether a report engine can use Brush formatting or not. 
        /// </summary>
        [StiSerializable]
        [StiCategory("Parameters")]
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets a value which indicates whether a report engine can use Brush formatting or not.")]
        public bool AllowUseBrush { get; set; } = true;

        private bool ShouldAllowUseBrush()
        {
            return AllowUseBrush != true;
        }

        #endregion

        #region Methods.Style
        public override void DrawStyle(Graphics g, Rectangle rect, bool paintValue, bool paintImage)
        {
            if (paintImage)
                DrawStyleImage(g, rect, StiReportImages.Styles.Chart(StiImageSize.Double));

            DrawStyleName(g, rect);
        }

        /// <summary>
        /// Gets a style from the component.
        /// </summary>
        /// <param name="component">Component.</param>
        public override void GetStyleFromComponent(StiComponent component, StiStyleElements styleElements)
        {
            if (styleElements != StiStyleElements.All)
                throw new Exception("StiCrossTabStyle support only StiStyleElements.All.");

            var chart = component as Stimulsoft.Report.Chart.StiChart;
            if (chart != null)
            {
                #region Color
                if (chart.Style is Stimulsoft.Report.Chart.StiCustomStyle)
                {
                    string name = chart.CustomStyleName;
                    if (!string.IsNullOrEmpty(name) && chart.Report != null && chart.Report.Styles[name] is StiChartStyle)
                    {
                        this.BasicStyleColor = ((StiChartStyle) chart.Report.Styles[name]).BasicStyleColor;
                        this.StyleColors = ((StiChartStyle) chart.Report.Styles[name]).StyleColors;
                    }
                }
                else
                {
                    this.BasicStyleColor = chart.Style.Core.BasicStyleColor;
                    this.StyleColors = chart.Style.Core.StyleColors;
                }
                #endregion

                #region IStiBorder
                if ((styleElements & StiStyleElements.Border) > 0)
                {
                    var cmp = component as IStiBorder;
                    this.Border = cmp.Border.Clone() as StiBorder;

                    this.AllowUseBorderFormatting = true;
                    this.AllowUseBorderSides = true;
                }
                #endregion

                #region IStiBrush
                if ((styleElements & StiStyleElements.Brush) > 0)
                {
                    var cmp = component as IStiBrush;
                    this.Brush = cmp.Brush.Clone() as StiBrush;

                    this.SeriesLabelsBorderColor = chart.Labels.BorderColor;
                    this.SeriesLabelsColor = chart.Labels.LabelColor;
                    this.LegendBorderColor = chart.Legend.BorderColor;

                    this.AllowUseBrush = true;
                }
                #endregion

                #region IStiBackColor
                if (component is IStiBackColor && ((styleElements & StiStyleElements.Brush) > 0))
                {
                    var cmp = component as IStiBackColor;
                    this.Brush = new StiSolidBrush(cmp.BackColor);

                    this.AllowUseBrush = true;
                }
                #endregion
            }
        }

        /// <summary>
        /// Sets style to a component.
        /// </summary>
        /// <param name="component">Component.</param>
        public override void SetStyleToComponent(StiComponent component)
        {
            var chart = component as Stimulsoft.Report.Chart.StiChart;
            if (chart == null) return;

            if (!StiStyleConditionHelper.IsAllowStyle(component, this))
                return;

            #region Color
            chart.CustomStyleName = this.Name;
            chart.Style = new Stimulsoft.Report.Chart.StiCustomStyle(this.Name);
            #endregion

            #region IStiBorder
            if ((this.AllowUseBorderFormatting || this.AllowUseBorderSides))
            {
                var cmp = component as IStiBorder;

                var sides = cmp.Border.Side;

                if (this.AllowUseBorderFormatting)
                {
                    cmp.Border = this.Border.Clone() as StiBorder;
                    cmp.Border.Side = sides;
                }

                if (this.AllowUseBorderSides)
                    cmp.Border.Side = this.Border.Side;
            }
            #endregion

            #region IStiBrush
            if (this.AllowUseBrush)
            {
                var cmp = component as IStiBrush;
                cmp.Brush = this.Brush.Clone() as StiBrush;
            }
            #endregion

            #region IStiBackColor
            if (component is IStiBackColor && this.AllowUseBrush)
            {
                var cmp = component as IStiBackColor;
                cmp.BackColor = StiBrush.ToColor(this.Brush);
            }
            #endregion
        }
        #endregion

        /// <summary>
        /// Creates a new object of the type StiChartStyle.
        /// </summary>
        /// <param name="name">Style name.</param>
        /// <param name="description">Style description.</param>
        internal StiChartStyle(string name, string description, StiReport report)
            : base(name, description, report)
        {
            StyleColors = DefaultColors.Clone() as Color[];
        }

        /// <summary>
        /// Creates a new object of the type StiChartStyle.
        /// </summary>
        /// <param name="name">Style name.</param>
        /// <param name="description">Style description.</param>
        public StiChartStyle(string name, string description)
            : this(name, description, null)
        {
        }

        /// <summary>
        /// Creates a new object of the type StiChartStyle.
        /// </summary>
        /// <param name="name">Style name.</param>
        public StiChartStyle(string name)
            : this(name, "")
        {
        }

        /// <summary>
        /// Creates a new object of the type StiChartStyle.
        /// </summary>
        public StiChartStyle()
            : this("")
        {
            
        }
    }
}