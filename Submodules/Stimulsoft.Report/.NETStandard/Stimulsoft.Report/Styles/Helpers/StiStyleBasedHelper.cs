#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports 									            }
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
{	TRADE SECRETS OF STIMULSOFT										}
{																	}
{	CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON		}
{	ADDITIONAL RESTRICTIONS.										}
{																	}
{*******************************************************************}
*/
#endregion Copyright (C) 2003-2022 Stimulsoft

using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Dashboard.Styles;
using System;
using System.Collections.Generic;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Drawing;
#endif

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Styles.Helpers
{
    public static class StiStyleBasedHelper
    {
        #region class StyleInfo
        public class StyleInfo
        {
            public StyleInfo(StiStyleType styleType, List<StiBaseStyle> styles)
            {
                this.StyleType = styleType;
                this.Styles = styles;
            }

            #region Properties
            public StiStyleType StyleType { get; }
            public List<StiBaseStyle> Styles { get; }
            #endregion

            #region Methods
            public string GetDefaultStyle()
            {
                switch (StyleType)
                {
                    case StiStyleType.Cards:
                        return DefaultCardsStyle;

                    case StiStyleType.Chart:
                        return DefaultChartStyle;

                    case StiStyleType.CrossTab:
                        return DefaultCrossTabStyle;

                    case StiStyleType.Table:
                        return DefaultTableStyle;

                    case StiStyleType.Gauge:
                        return DefaultGaugeStyle;

                    case StiStyleType.Map:
                        return DefaultMapStyle;

                    case StiStyleType.Indicator:
                        return DefaultIndicatorStyle;

                    case StiStyleType.Progress:
                        return DefaultProgressStyle;

                    default:
                        throw new NotSupportedException();
                }
            }

            public void SetDefaultStyle(string value)
            {
                switch (StyleType)
                {
                    case StiStyleType.Cards:
                        DefaultCardsStyle = value;
                        break;

                    case StiStyleType.Chart:
                        DefaultChartStyle = value;
                        break;

                    case StiStyleType.CrossTab:
                        DefaultCrossTabStyle = value;
                        break;

                    case StiStyleType.Table:
                        DefaultTableStyle = value;
                        break;

                    case StiStyleType.Gauge:
                        DefaultGaugeStyle = value;
                        break;

                    case StiStyleType.Map:
                        DefaultMapStyle = value;
                        break;

                    case StiStyleType.Indicator:
                        DefaultIndicatorStyle = value;
                        break;

                    case StiStyleType.Progress:
                        DefaultProgressStyle = value;
                        break;

                    default:
                        throw new NotSupportedException();
                }
            }

            public StiBaseStyle GenerateStyle(StiBaseStyle baseStyle)
            {
                switch (StyleType)
                {
                    #region Cards
                    case StiStyleType.Cards:
                        {
                            var core = (StiCardsElementStyle)baseStyle;
                            return new StiCardsStyle
                            {
                                BackColor = core.BackColor,
                                ForeColor = core.CellForeColor,
                                LineColor = core.LineColor,
                                SeriesColors = CloneColors(core.SeriesColors),
                                ToolTipBrush = (StiBrush)core.ToolTipBrush.Clone(),
                                ToolTipTextBrush = (StiBrush)core.ToolTipTextBrush.Clone(),
                                ToolTipBorder = (StiSimpleBorder)core.ToolTipBorder.Clone(),
                                ToolTipCornerRadius = (StiCornerRadius)core.ToolTipCornerRadius.Clone()
                            };
                        }
                    #endregion

                    #region Chart
                    case StiStyleType.Chart:
                        {
                            var core = ((Stimulsoft.Report.Chart.StiChartStyle)baseStyle).Core;
                            return new Stimulsoft.Report.StiChartStyle
                            {
                                Brush = (StiBrush)core.ChartBrush.Clone(),
                                ChartAreaBrush = (StiBrush)core.ChartAreaBrush.Clone(),
                                ChartAreaBorderColor = core.ChartAreaBorderColor,
                                ChartAreaBorderThickness = core.ChartAreaBorderThickness,
                                ChartAreaShowShadow = core.ChartAreaShowShadow,
                                SeriesLabelsBrush = (StiBrush)core.SeriesLabelsBrush.Clone(),
                                SeriesLabelsColor = core.SeriesLabelsColor,
                                SeriesLabelsBorderColor = core.SeriesLabelsBorderColor,
                                SeriesLabelsLineColor = core.SeriesLabelsLineColor,
                                //SeriesLabelsFont = core.SeriesLabelsFont.Clone(),
                                TrendLineColor = core.TrendLineColor,
                                TrendLineShowShadow = core.TrendLineShowShadow,
                                LegendBrush = (StiBrush)core.LegendBrush.Clone(),
                                LegendLabelsColor = core.LegendLabelsColor,
                                LegendBorderColor = core.LegendBorderColor,
                                LegendTitleColor = core.LegendTitleColor,
                                //LegendShowShadow = core.LegendShowShadow,
                                //LegendFont = core.LegendFont.Clone(),
                                AxisTitleColor = core.AxisTitleColor,
                                AxisLineColor = core.AxisLineColor,
                                AxisLabelsColor = core.AxisLabelsColor,
                                InterlacingHorBrush = (StiBrush)core.InterlacingHorBrush.Clone(),
                                InterlacingVertBrush = (StiBrush)core.InterlacingVertBrush.Clone(),
                                GridLinesHorColor = core.GridLinesHorColor,
                                GridLinesVertColor = core.GridLinesVertColor,
                                SeriesLighting = core.SeriesLighting,
                                SeriesShowShadow = core.SeriesShowShadow,
                                SeriesShowBorder = core.SeriesShowBorder,
                                MarkerVisible = core.MarkerVisible,
                                //FirstStyleColor = core.FirstStyleColor,
                                //LastStyleColor = core.LastStyleColor,
                                StyleColors = CloneColors(core.StyleColors),
                                BasicStyleColor = core.BasicStyleColor,
                                ToolTipBrush = (StiBrush)core.ToolTipBrush.Clone(),
                                ToolTipTextBrush = (StiBrush)core.ToolTipTextBrush.Clone(),
                                ToolTipBorder = (StiSimpleBorder)core.ToolTipBorder.Clone(),
                                ToolTipCornerRadius = (StiCornerRadius)core.ToolTipCornerRadius.Clone()
                            };
                        }
                    #endregion

                    #region CrossTab
                    case StiStyleType.CrossTab:
                        {
                            return (StiCrossTabStyle)((StiCrossTabStyle)baseStyle).Clone();
                        }
                    #endregion

                    #region Table
                    case StiStyleType.Table:
                        {
                            var core = (Stimulsoft.Report.Components.Table.StiTableStyleFX)baseStyle;
                            return new StiTableStyle
                            {
                                BackColor = core.BackColor,
                                DataColor = core.DataColor,
                                DataForeground = core.DataForeground,
                                SelectedDataColor = core.SelectedDataColor,
                                SelectedDataForeground = core.SelectedDataForeground,
                                AlternatingDataColor = core.AlternatingDataColor,
                                AlternatingDataForeground = core.AlternatingDataForeground,
                                HeaderColor = core.HeaderColor,
                                HeaderForeground = core.HeaderForeground,
                                HotHeaderColor = core.HotHeaderColor,
                                FooterColor = core.FooterColor,
                                FooterForeground = core.FooterForeground,
                                GridColor = core.GridColor,

                            };
                        }
                    #endregion

                    #region Gauge
                    case StiStyleType.Gauge:
                        {
                            var core = ((Stimulsoft.Report.Gauge.StiGaugeStyleXF)baseStyle).Core;
                            return new StiGaugeStyle
                            {
                                Brush = core.Brush,
                                ForeColor = core.ForeColor,
                                TargetColor = core.TargetColor,
                                BorderColor = core.BorderColor,
                                BorderWidth = core.BorderWidth,
                                TickMarkMajorBrush = (StiBrush)core.TickMarkMajorBrush.Clone(),
                                TickMarkMajorBorder = (StiBrush)core.TickMarkMajorBorder.Clone(),
                                TickMarkMajorBorderWidth = core.TickMarkMajorBorderWidth,
                                TickMarkMinorBrush = (StiBrush)core.TickMarkMinorBrush.Clone(),
                                TickMarkMinorBorder = (StiBrush)core.TickMarkMinorBorder.Clone(),
                                TickMarkMinorBorderWidth = core.TickMarkMinorBorderWidth,
                                TickLabelMajorTextBrush = (StiBrush)core.TickLabelMajorTextBrush.Clone(),
                                TickLabelMajorFont = (Font)core.TickLabelMajorFont.Clone(),
                                TickLabelMinorTextBrush = (StiBrush)core.TickLabelMinorTextBrush.Clone(),
                                TickLabelMinorFont = (Font)core.TickLabelMinorFont.Clone(),
                                LinearScaleBrush = (StiBrush)core.LinearScaleBrush.Clone(),
                                LinearBarBrush = (StiBrush)core.LinearBarBrush.Clone(),
                                LinearBarBorderBrush = (StiBrush)core.LinearBarBorderBrush.Clone(),
                                LinearBarEmptyBrush = (StiBrush)core.LinearBarEmptyBrush.Clone(),
                                LinearBarEmptyBorderBrush = (StiBrush)core.LinearBarEmptyBorderBrush.Clone(),
                                //LinearBarStartWidth = core.LinearBarStartWidth,
                                //LinearBarEndWidth = core.LinearBarEndWidth,
                                RadialBarBrush = (StiBrush)core.RadialBarBrush.Clone(),
                                RadialBarBorderBrush = (StiBrush)core.RadialBarBorderBrush.Clone(),
                                RadialBarEmptyBrush = (StiBrush)core.RadialBarEmptyBrush.Clone(),
                                RadialBarEmptyBorderBrush = (StiBrush)core.RadialBarEmptyBorderBrush.Clone(),
                                //RadialBarStartWidth = core.RadialBarStartWidth,
                                //RadialBarEndWidth = core.RadialBarEndWidth,
                                NeedleBrush = (StiBrush)core.NeedleBrush.Clone(),
                                NeedleBorderBrush = (StiBrush)core.NeedleBorderBrush.Clone(),
                                NeedleCapBrush = (StiBrush)core.NeedleCapBrush.Clone(),
                                NeedleCapBorderBrush = (StiBrush)core.NeedleCapBorderBrush.Clone(),
                                NeedleBorderWidth = core.NeedleBorderWidth,
                                //NeedleCapBorderWidth = core.NeedleCapBorderWidth,
                                //NeedleStartWidth = core.NeedleStartWidth,
                                //NeedleEndWidth = core.NeedleEndWidth,
                                //NeedleRelativeHeight = core.NeedleRelativeHeight,
                                //NeedleRelativeWith = core.NeedleRelativeWith,
                                //MarkerSkin = core.MarkerSkin,
                                MarkerBrush = (StiBrush)core.MarkerBrush.Clone(),
                                //LinearMarkerBorder = (StiBrush)core.LinearMarkerBorder.Clone(),
                                //MarkerBorderBrush = (StiBrush)core.MarkerBorderBrush.Clone(),
                                //MarkerBorderWidth = core.MarkerBorderWidth,
                            };
                        }
                    #endregion

                    #region Map
                    case StiStyleType.Map:
                        {
                            var core = (Stimulsoft.Report.Maps.StiMapStyleFX)baseStyle;
                            var resultStyle = new StiMapStyle
                            {
                                IndividualColor = core.IndividualColor,
                                Colors = CloneColors(core.Colors),
                                DefaultColor = core.DefaultColor,
                                BackColor = core.BackColor,
                                BorderSize = core.BorderSize,
                                BorderColor = core.BorderColor,
                                LabelShadowForeground = core.LabelShadowForeground,
                                LabelForeground = core.LabelForeground,
                                BubbleBackColor = core.BubbleBackColor,
                                BubbleBorderColor = core.BubbleBorderColor,
                                ToolTipBrush = (StiBrush)core.ToolTipBrush.Clone(),
                                ToolTipTextBrush = (StiBrush)core.ToolTipTextBrush.Clone(),
                                ToolTipBorder = (StiSimpleBorder)core.ToolTipBorder.Clone(),
                                ToolTipCornerRadius = (StiCornerRadius)core.ToolTipCornerRadius.Clone()
                            };

                            resultStyle.Heatmap.Color = core.Heatmap.Color;
                            resultStyle.Heatmap.ZeroColor = core.Heatmap.ZeroColor;
                            resultStyle.Heatmap.Mode = core.Heatmap.Mode;
                            resultStyle.HeatmapWithGroup.Colors = core.HeatmapWithGroup.Colors;
                            resultStyle.HeatmapWithGroup.ZeroColor = core.HeatmapWithGroup.ZeroColor;
                            resultStyle.HeatmapWithGroup.Mode = core.HeatmapWithGroup.Mode;

                            return resultStyle;
                        }
                    #endregion

                    #region Indicator
                    case StiStyleType.Indicator:
                        {
                            var core = (StiIndicatorElementStyle)baseStyle;
                            return new StiIndicatorStyle
                            {
                                GlyphColor = core.GlyphColor,
                                BackColor = core.BackColor,
                                ForeColor = StiDashboardStyleHelper.GetForeColor(core.Ident),
                                HotBackColor = core.HotBackColor,
                                PositiveColor = core.PositiveColor,
                                NegativeColor = core.NegativeColor,
                                ToolTipBrush = (StiBrush)core.ToolTipBrush.Clone(),
                                ToolTipTextBrush = (StiBrush)core.ToolTipTextBrush.Clone(),
                                ToolTipBorder = (StiSimpleBorder)core.ToolTipBorder.Clone(),
                                ToolTipCornerRadius = (StiCornerRadius)core.ToolTipCornerRadius.Clone()
                            };
                        }
                    #endregion

                    #region Progress
                    case StiStyleType.Progress:
                        {
                            var core = (StiProgressElementStyle)baseStyle;
                            return new StiProgressStyle
                            {
                                ForeColor = core.ForeColor,
                                TrackColor = core.TrackColor,
                                BandColor = core.BandColor,
                                SeriesColors = CloneColors(core.SeriesColors),
                                BackColor = core.BackColor,
                            };
                        }
                    #endregion
                }

                return null;
            }

            private Color[] CloneColors(Color[] colors)
            {
                if (colors != null)
                {
                    var result = new Color[colors.Length];
                    for (int index = 0; index < colors.Length; index++)
                    {
                        result[index] = colors[index];
                    }

                    return result;
                }

                return null;
            }
            #endregion
        }
        #endregion

        #region Properties
        public static string DefaultChartStyle
        {
            get
            {
                StiSettings.Load();
                return StiSettings.GetStr("StyleBasedForm", "ChartStyle", "StiStyle29");
            }
            set
            {
                StiSettings.Set("StyleBasedForm", "ChartStyle", value);
                StiSettings.Save();
            }
        }

        public static string DefaultCrossTabStyle
        {
            get
            {
                StiSettings.Load();
                return StiSettings.GetStr("StyleBasedForm", "CrossTabStyle", "ffffff");
            }
            set
            {
                StiSettings.Set("StyleBasedForm", "CrossTabStyle", value);
                StiSettings.Save();
            }
        }

        public static string DefaultTableStyle
        {
            get
            {
                StiSettings.Load();
                return StiSettings.GetStr("StyleBasedForm", "TableStyle", "StiTable21StyleFX");
            }
            set
            {
                StiSettings.Set("StyleBasedForm", "TableStyle", value);
                StiSettings.Save();
            }
        }

        public static string DefaultGaugeStyle
        {
            get
            {
                StiSettings.Load();
                return StiSettings.GetStr("StyleBasedForm", "GaugeStyle", "StiGaugeStyleXF26");
            }
            set
            {
                StiSettings.Set("StyleBasedForm", "GaugeStyle", value);
                StiSettings.Save();
            }
        }

        public static string DefaultMapStyle
        {
            get
            {
                StiSettings.Load();
                return StiSettings.GetStr("StyleBasedForm", "MapStyle", "StiMap25StyleFX");
            }
            set
            {
                StiSettings.Set("StyleBasedForm", "MapStyle", value);
                StiSettings.Save();
            }
        }

        public static string DefaultIndicatorStyle
        {
            get
            {
                StiSettings.Load();
                return StiSettings.GetStr("StyleBasedForm", "IndicatorStyle", "StiBlueIndicatorElementStyle");
            }
            set
            {
                StiSettings.Set("StyleBasedForm", "IndicatorStyle", value);
                StiSettings.Save();
            }
        }

        public static string DefaultProgressStyle
        {
            get
            {
                StiSettings.Load();
                return StiSettings.GetStr("StyleBasedForm", "ProgressStyle", "StiBlueProgressElementStyle");
            }
            set
            {
                StiSettings.Set("StyleBasedForm", "ProgressStyle", value);
                StiSettings.Save();
            }
        }

        public static string DefaultCardsStyle
        {
            get
            {
                StiSettings.Load();
                return StiSettings.GetStr("StyleBasedForm", "CardsStyle", "StiBlueCardsElementStyle");
            }
            set
            {
                StiSettings.Set("StyleBasedForm", "CardsStyle", value);
                StiSettings.Save();
            }
        }
        #endregion

        #region Methods
        public static StyleInfo GetInfo(Type styleType)
        {
            #region Chart
            if (styleType == typeof(Stimulsoft.Report.StiChartStyle))
            {
                var styles = new List<StiBaseStyle>(StiOptions.Services.ChartStyles);
                styles.Reverse();
                if (styles.Count > 0)
                    return new StyleInfo(StiStyleType.Chart, styles);
            }
            #endregion

            #region CrossTab
            else if (styleType == typeof(StiCrossTabStyle))
            {
                var styles = new List<StiBaseStyle>();
                foreach (var color in StiOptions.Designer.CrossTab.StyleColors)
                {
                    var style = new StiCrossTabStyle
                    {
                        Color = color
                    };
                    styles.Add(style);
                }

                if (styles.Count > 0)
                    return new StyleInfo(StiStyleType.CrossTab, styles);
            }
            #endregion

            #region Table
            else if (styleType == typeof(StiTableStyle))
            {
                var styles = new List<StiBaseStyle>(StiOptions.Services.TableStyles);
                styles.Reverse();
                if (styles.Count > 0)
                    return new StyleInfo(StiStyleType.Table, styles);
            }
            #endregion

            #region Gauge
            else if (styleType == typeof(StiGaugeStyle))
            {
                var styles = new List<StiBaseStyle>(StiOptions.Services.GaugeStyles);
                styles.Reverse();
                if (styles.Count > 0)
                    return new StyleInfo(StiStyleType.Gauge, styles);
            }
            #endregion

            #region Map
            else if (styleType == typeof(StiMapStyle))
            {
                var styles = new List<StiBaseStyle>(StiOptions.Services.MapStyles);
                styles.Reverse();
                if (styles.Count > 0)
                    return new StyleInfo(StiStyleType.Map, styles);
            }
            #endregion

            #region Indicator
            else if (styleType == typeof(StiIndicatorStyle))
            {
                var styles = new List<StiBaseStyle>(StiOptions.Services.Dashboards.IndicatorStyles);
                if (styles.Count > 0)
                    return new StyleInfo(StiStyleType.Indicator, styles);
            }
            #endregion

            #region Progress
            else if (styleType == typeof(StiProgressStyle))
            {
                var styles = new List<StiBaseStyle>(StiOptions.Services.Dashboards.ProgressStyles);
                if (styles.Count > 0)
                    return new StyleInfo(StiStyleType.Progress, styles);
            }
            #endregion

            #region Cards
            else if (styleType == typeof(StiCardsStyle))
            {
                var styles = new List<StiBaseStyle>(StiOptions.Services.Dashboards.CardsStyles);
                if (styles.Count > 0)
                    return new StyleInfo(StiStyleType.Cards, styles);
            }
            #endregion

            return null;
        }

        public static string GetStyleName(StiBaseStyle style)
        {
            if (style is StiCrossTabStyle crossTabStyle)
            {
                return GetCrossTabStyleName(crossTabStyle);
            }
            else
            {
                return style.GetType().Name;
            }
        }

        public static string GetCrossTabStyleName(StiCrossTabStyle style)
        {
            var color = style.Color;

            string name;
            switch (color.Name)
            {
                case "ff0bac45":
                    name = "Dark Pastel Green";
                    break;

                case "ffb5a1dd":
                    name = "Perfume";
                    break;

                case "ffffc000":
                    name = "Amber";
                    break;

                case "ffed7d31":
                    name = "Sun";
                    break;

                case "ff239fd9":
                    name = "Summer Sky";
                    break;

                default:
			        if (color.IsKnownColor)
                        name = color.Name;

                    else
                        name = ToHex(style.Color);

                    break;
            }

            return name;
        }

        private static string ToHex(Color c) => $"{c.R:X2}{c.G:X2}{c.B:X2}".ToLowerInvariant();
        #endregion
    }
}