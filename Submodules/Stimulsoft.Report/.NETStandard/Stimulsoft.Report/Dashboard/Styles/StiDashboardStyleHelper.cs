#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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

using Stimulsoft.Base.Dashboard;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.App;
using Stimulsoft.Report.Chart;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dashboard.Helpers;
using Stimulsoft.Report.Dashboard.Styles.Cards;
using Stimulsoft.Report.Gauge;
using Stimulsoft.Report.Maps;
using Stimulsoft.Report.Styles;
using Stimulsoft.Report.Surface;
using System;
using System.Linq;
using System.Reflection;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
using FontFamily = Stimulsoft.Drawing.FontFamily;
#endif

namespace Stimulsoft.Report.Dashboard.Styles
{
    public static class StiDashboardStyleHelper
    {
        #region Fields
        private static FontFamily iconFontFamily;
        #endregion

        #region Methods.ConvertTo
        private static Color[] CloneColors(Color[] value)
        {
            if (value == null) return null;

            var result = new Color[value.Length];
            for (int index = 0; index < value.Length; index++)
            {
                result[index] = value[index];
            }

            return result;
        }

        public static StiChartStyle GetCopyChartStyle(IStiChartStyle chartStyle, IStiChartElement element)
        {
            var core = chartStyle.Core;
            return new StiChartStyle
            {
                Brush = new StiSolidBrush(GetBackColor(element, null, true)),
                ChartAreaBrush = (StiBrush)core.ChartAreaBrush.Clone(),
                ChartAreaBorderColor = core.ChartAreaBorderColor,
                ChartAreaShowShadow = core.ChartAreaShowShadow,
                SeriesLighting = core.SeriesLighting,
                SeriesShowShadow = core.SeriesShowShadow,
                SeriesShowBorder = core.SeriesShowBorder,
                SeriesLabelsBrush = (StiBrush)core.SeriesLabelsBrush.Clone(),
                SeriesLabelsColor = core.SeriesLabelsColor,
                SeriesLabelsBorderColor = core.SeriesLabelsBorderColor,
                SeriesLabelsLineColor = core.SeriesLabelsLineColor,
                TrendLineColor = core.TrendLineColor,
                TrendLineShowShadow = core.TrendLineShowShadow,
                LegendBrush = core.LegendBrush,
                LegendLabelsColor = core.LegendLabelsColor,
                LegendBorderColor = core.LegendBorderColor,
                LegendTitleColor = core.LegendTitleColor,
                AxisTitleColor = core.AxisTitleColor,
                AxisLineColor = core.AxisLineColor,
                AxisLabelsColor = core.AxisLabelsColor,
                MarkerVisible = core.MarkerVisible,
                InterlacingHorBrush = (StiBrush)core.InterlacingHorBrush.Clone(),
                InterlacingVertBrush = (StiBrush)core.InterlacingVertBrush.Clone(),
                GridLinesHorColor = core.GridLinesHorColor,
                GridLinesVertColor = core.GridLinesVertColor,
                StyleColors = CloneColors(core.StyleColors),
                BasicStyleColor = core.BasicStyleColor,
                ToolTipBrush = (StiBrush)core.ToolTipBrush.Clone(),
                ToolTipTextBrush = (StiBrush)core.ToolTipTextBrush.Clone(),
                ToolTipBorder = (StiSimpleBorder)core.ToolTipBorder.Clone(),
                ToolTipCornerRadius = (StiCornerRadius)core.ToolTipCornerRadius.Clone()
            };
        }

        public static StiTableStyle GetCopyTableStyle(IStiTableElement element, StiTableElementStyle tableStyle)
        {
            return new StiTableStyle
            {
                BackColor = GetBackColor(element, null, true),
                AlternatingDataColor = tableStyle.AlternatingCellBackColor,
                AlternatingDataForeground = tableStyle.AlternatingCellForeColor,
                DataColor = tableStyle.CellBackColor,
                DataForeground = tableStyle.CellForeColor,
                GridColor = tableStyle.LineColor,
                HeaderColor = tableStyle.HeaderBackColor,
                HeaderForeground = tableStyle.HeaderForeColor,
                HotHeaderColor = tableStyle.HotHeaderBackColor,
                SelectedDataColor = tableStyle.SelectedCellBackColor,
                SelectedDataForeground = tableStyle.SelectedCellForeColor,
                FooterColor = tableStyle.FooterBackColor,
                FooterForeground = tableStyle.FooterForeColor
            };
        }

        public static StiGaugeStyle ConvertToReportGaugeStyle(IStiGaugeElement element)
        {
            StiGaugeStyleXF gaugeStyle = null;
            if (element.Style == StiElementStyleIdent.Custom && !string.IsNullOrEmpty(element.CustomStyleName))
            {
                var customStyle = element.Report.Styles.ToList().FirstOrDefault(x => x.Name == element.CustomStyleName) as StiGaugeStyle;
                if (customStyle != null)
                    gaugeStyle = new StiCustomGaugeStyle(customStyle);
            }
            else
            {
                var styleIdent = element.Style == StiElementStyleIdent.Auto
                    ? ((IStiDashboard)element.Page).Style
                    : element.Style;

                gaugeStyle = GetGaugeStyle(styleIdent);
            }

            if (gaugeStyle == null)
                gaugeStyle = new StiGaugeStyleXF29();

            var core = gaugeStyle.Core;
            return new StiGaugeStyle
            {
                Brush = (StiBrush)core.Brush.Clone(),
                BorderColor = core.BorderColor,
                ForeColor = core.ForeColor,
                TargetColor = core.TargetColor,
                BorderWidth = core.BorderWidth,
                TickMarkMajorBrush = new StiEmptyBrush(),
                TickMarkMajorBorder = (StiBrush)core.TickMarkMajorBorder.Clone(),
                TickMarkMinorBrush = (StiBrush)core.TickMarkMinorBrush.Clone(),
                TickMarkMinorBorder = (StiBrush)core.TickMarkMinorBorder.Clone(),
                TickLabelMajorTextBrush = (StiBrush)core.TickLabelMajorTextBrush.Clone(),
                TickLabelMajorFont = (Font)core.TickLabelMajorFont.Clone(),
                TickLabelMinorTextBrush = (StiBrush)core.TickLabelMinorTextBrush.Clone(),
                TickLabelMinorFont = (Font)core.TickLabelMinorFont.Clone(),
                MarkerBrush = (StiBrush)core.MarkerBrush.Clone(),
                LinearBarBrush = (StiBrush)core.LinearBarBrush.Clone(),
                LinearBarBorderBrush = (StiBrush)core.LinearBarBorderBrush.Clone(),
                LinearBarEmptyBrush = (StiBrush)core.LinearBarEmptyBrush.Clone(),
                LinearBarEmptyBorderBrush = (StiBrush)core.LinearBarEmptyBorderBrush.Clone(),
                RadialBarBrush = (StiBrush)core.RadialBarBrush.Clone(),
                RadialBarBorderBrush = (StiBrush)core.RadialBarBorderBrush.Clone(),
                RadialBarEmptyBrush = (StiBrush)core.RadialBarEmptyBrush.Clone(),
                RadialBarEmptyBorderBrush = (StiBrush)core.RadialBarEmptyBorderBrush.Clone(),
                NeedleBrush = (StiBrush)core.NeedleBrush.Clone(),
                NeedleBorderBrush = (StiBrush)core.NeedleBorderBrush.Clone(),
                NeedleCapBrush = (StiBrush)core.NeedleCapBrush.Clone(),
                NeedleCapBorderBrush = (StiBrush)core.NeedleCapBorderBrush.Clone(),
            };
        }

        public static StiCrossTabStyle ConvertToReportPivotTableStyle(IStiPivotTableElement element)
        {
            var pivotStyle = GetPivotTableStyle(element);
            return new StiCrossTabStyle
            {
                BackColor = pivotStyle.BackColor,
                AlternatingCellForeColor = pivotStyle.AlternatingCellForeColor,
                AlternatingCellBackColor = pivotStyle.AlternatingCellBackColor,
                CellBackColor = pivotStyle.CellBackColor,
                CellForeColor = pivotStyle.CellForeColor,
                LineColor = pivotStyle.LineColor,
                ColumnHeaderBackColor = pivotStyle.ColumnHeaderBackColor,
                ColumnHeaderForeColor = pivotStyle.ColumnHeaderForeColor,
                HotColumnHeaderBackColor = pivotStyle.HotColumnHeaderBackColor,
                HotRowHeaderBackColor = pivotStyle.HotRowHeaderBackColor,
                RowHeaderBackColor = pivotStyle.RowHeaderBackColor,
                RowHeaderForeColor = pivotStyle.RowHeaderForeColor,
                SelectedCellBackColor = pivotStyle.SelectedCellBackColor,
                SelectedCellForeColor = pivotStyle.SelectedCellForeColor,
            };
        }

        public static StiCardsStyle ConvertToReportCardsStyle(IStiCardsElement element)
        {
            var cardsStyle = GetCardsStyle(element);
            return new StiCardsStyle
            {
                BackColor = GetBackColor(element, null, true),
                ForeColor = cardsStyle.CellForeColor,
                LineColor = cardsStyle.LineColor,
                SeriesColors = cardsStyle.SeriesColors,
                ToolTipBorder = cardsStyle.ToolTipBorder,
                ToolTipTextBrush = cardsStyle.ToolTipTextBrush,
                ToolTipCornerRadius = cardsStyle.ToolTipCornerRadius,
                ToolTipBrush = cardsStyle.ToolTipBrush
            };
        }

        public static StiIndicatorStyle ConvertToReportIndicatorStyle(IStiIndicatorElement element)
        {
            var indicatorStyle = GetIndicatorStyle(element);
            return new StiIndicatorStyle
            {
                BackColor = GetBackColor(element, null, true),                
                ForeColor = GetForeColor(element),
                HotBackColor = GetHotBackColor(element),
                GlyphColor = indicatorStyle.GlyphColor,
                PositiveColor = indicatorStyle.PositiveColor,
                NegativeColor = indicatorStyle.NegativeColor,
                ToolTipBorder = indicatorStyle.ToolTipBorder,
                ToolTipTextBrush = indicatorStyle.ToolTipTextBrush,
                ToolTipCornerRadius = indicatorStyle.ToolTipCornerRadius,
                ToolTipBrush = indicatorStyle.ToolTipBrush
            };
        }

        public static StiProgressStyle ConvertToReportProgressStyle(IStiProgressElement element)
        {
            var progressStyle = GetProgressStyle(element);
            return new StiProgressStyle
            {
                BackColor = progressStyle.BackColor,
                TrackColor = progressStyle.TrackColor,
                BandColor = progressStyle.BandColor,
                ForeColor = progressStyle.ForeColor,
                SeriesColors = progressStyle.SeriesColors,
            };
        }

        public static StiMapStyle ConvertToReportRegionMapStyle(IStiRegionMapElement element)
        {
            StiMapStyle mapStyle;
            if (element.Style == StiElementStyleIdent.Custom && !string.IsNullOrEmpty(element.CustomStyleName))
            {
                mapStyle = element.Report.Styles.ToList().FirstOrDefault(x => x.Name == element.CustomStyleName) as StiMapStyle;
            }
            else
            {
                var styleIdent = element.Style == StiElementStyleIdent.Auto
                    ? ((IStiDashboard)element.Page).Style
                    : element.Style;

                mapStyle = GetMapStyle(styleIdent);
            }

            if (mapStyle == null)
                mapStyle = new StiMap29StyleFX();

            var resultStyle = new StiMapStyle
            {
                BackColor = mapStyle.BackColor,
                BorderColor = mapStyle.BorderColor,
                BorderSize = mapStyle.BorderSize,
                Colors = mapStyle.Colors,
                DefaultColor = mapStyle.DefaultColor,
                IndividualColor = mapStyle.IndividualColor,
                LabelForeground = mapStyle.LabelForeground,
                LabelShadowForeground = mapStyle.LabelShadowForeground,
                BubbleBackColor = mapStyle.BubbleBackColor,
                BubbleBorderColor = mapStyle.BubbleBorderColor,
            };

            resultStyle.Heatmap.Color = mapStyle.Heatmap.Color;
            resultStyle.Heatmap.ZeroColor = mapStyle.Heatmap.ZeroColor;
            resultStyle.Heatmap.Mode = mapStyle.Heatmap.Mode;
            resultStyle.HeatmapWithGroup.Colors = mapStyle.HeatmapWithGroup.Colors;
            resultStyle.HeatmapWithGroup.ZeroColor = mapStyle.HeatmapWithGroup.ZeroColor;
            resultStyle.HeatmapWithGroup.Mode = mapStyle.HeatmapWithGroup.Mode;

            return resultStyle;
        }

        public static StiDialogStyle ConvertToReportControlStyle(IStiControlElement element)
        {
            var controlStyle = GetControlStyle(element);
            return new StiDialogStyle
            {
                BackColor = controlStyle.BackColor,
                ForeColor = controlStyle.ForeColor,
                GlyphColor = controlStyle.GlyphColor,
                HotBackColor = controlStyle.HotBackColor,
                HotForeColor = controlStyle.HotForeColor,
                HotGlyphColor = controlStyle.HotGlyphColor,
                HotSelectedBackColor = controlStyle.HotSelectedBackColor,
                HotSelectedForeColor = controlStyle.HotSelectedForeColor,
                HotSelectedGlyphColor = controlStyle.HotSelectedGlyphColor,
                SelectedBackColor = controlStyle.SelectedBackColor,
                SelectedForeColor = controlStyle.SelectedForeColor,
                SelectedGlyphColor = controlStyle.SelectedGlyphColor,
                SeparatorColor = controlStyle.SeparatorColor
            };
        }
        #endregion

        #region Methods
        public static Color GetDashboardBackColor(IStiSurface surface, bool isViewer)
        {
            if (surface == null)
                return Color.White;

            var backColor = surface as IStiBackColor;
            if (backColor != null)
            {
                var expBackColor = StiDashboardExpressionHelper.GetBackColor(surface, backColor.BackColor);

                if (expBackColor != Color.Transparent)
                    return expBackColor;
            }

            var surfaceStyle = surface as IStiDashboardElementStyle;
            if (surfaceStyle == null)
                return Color.White;

            switch (surfaceStyle.Style)
            {
                case StiElementStyleIdent.SlateGray:
                case StiElementStyleIdent.DarkBlue:
                case StiElementStyleIdent.DarkGray:
                    return StiColor.Get("333333");

                case StiElementStyleIdent.DarkGreen:
                    return StiColor.Get("144b45");

                case StiElementStyleIdent.DarkTurquoise:
                    return StiColor.Get("1e4a61");

                case StiElementStyleIdent.Silver:
                    return StiColor.Get("43545e");

                case StiElementStyleIdent.AliceBlue:
                    return StiColor.Get("40508d");

                case StiElementStyleIdent.Sienna:
                    return StiColor.Get("ede5d8");

                default:
                    return isViewer ? StiColor.Get("#f3f3f3") : Color.White;
            }
        }

        public static bool IsDarkStyle(IStiDashboard dashboard)
        {
            if (dashboard == null)
                return false;

            return IsDarkStyle(dashboard.Style);
        }

        public static bool IsDarkStyle(IStiElement element)
        {
            var ident = GetStyle(element);
            if (ident == StiElementStyleIdent.Custom || ident == StiElementStyleIdent.Auto)
                ident = GetStyle(element?.Page as IStiDashboard);

            if (ident == StiElementStyleIdent.AliceBlue && !(element is IStiDashboard))
                return false;

            return IsDarkStyle(ident);
        }

        public static bool IsDarkStyle(StiElementStyleIdent ident)
        {
            switch (ident)
            {
                case StiElementStyleIdent.SlateGray:
                case StiElementStyleIdent.DarkBlue:
                case StiElementStyleIdent.DarkGray:
                case StiElementStyleIdent.DarkGreen:
                case StiElementStyleIdent.DarkTurquoise:
                case StiElementStyleIdent.Silver:
                case StiElementStyleIdent.AliceBlue:
                    return true;

                default:
                    return false;
            }
        }

        public static Font GetFont(object element)
        {
            var styleIdent = GetStyle(element);
            if (styleIdent == StiElementStyleIdent.Custom)
            {
                var report = (element as StiComponent).Report;
                var elementStyle = element as IStiDashboardElementStyle;
                if (elementStyle != null)
                {
                    var style = report.Styles[elementStyle.CustomStyleName] as StiDialogStyle;
                    if (style != null && style.AllowUseFont && style.Font != null)
                        return style.Font;
                }
            }

            return (element as IStiFont).Font;
        }

        public static Color GetDataEmptyColor(IStiElement element)
        {
            return IsDarkStyle(element)
                ? Color.FromArgb(255, 196, 196, 196)
                : Color.FromArgb(255, 240, 240, 240);
        }

        public static Color GetDataEmptyForeColor(IStiElement element)
        {
            return IsDarkStyle(element)
                ? Color.FromArgb(255, 196, 196, 196)
                : Color.DimGray;
        }

        public static Color GetForeColor(object element, Color? defaultColor = null)
        {
            var foreColor = element as IStiForeColor;
            if (foreColor != null)
            {
                var expForeColor = StiDashboardExpressionHelper.GetForeColor(element, foreColor.ForeColor);

                if (expForeColor != Color.Transparent)
                {
                    return expForeColor;
                }
            }

            var styleColor = GetStyleForeColor(element);
            if (styleColor != Color.Transparent)
                return styleColor;

            var controlElement = element as IStiControlElement;
            if (controlElement != null && GetControlStyle(controlElement).ForeColor != Color.Transparent)
                return GetControlStyle(controlElement).ForeColor;

            if (defaultColor != null)
                return defaultColor.Value;

            return GetNativeForeColor(element);
        }

        private static Color GetStyleForeColor(object element)
        {
            var styleIdent = GetStyle(element);
            if (styleIdent != StiElementStyleIdent.Custom)
                return Color.Transparent;

            var customStyleName = (element as IStiDashboardElementStyle)?.CustomStyleName;
            var report = (element as StiComponent).Report;

            if (element is IStiIndicatorElement)
            {
                var style = GetIndicatorStyle(element as IStiIndicatorElement) as StiCustomIndicatorElementStyle;
                return style == null ? Color.Transparent : style.ForeColor;
            }

            if (element is IStiCardsElement)
            {
                var style = GetCardsStyle(element as IStiCardsElement) as StiCustomCardsElementStyle;
                return style == null ? Color.Transparent : style.ForeColor;
            }

            if (element is IStiProgressElement)
            {
                var style = GetProgressStyle(element as IStiProgressElement) as StiCustomProgressElementStyle;
                return style == null ? Color.Transparent : style.ForeColor;
            }

            if (element is IStiGaugeElement)
            {
                var style = report.Styles.GetCustomGaugeStyle(customStyleName);
                return style?.Core?.ForeColor == null ? Color.Transparent : style.Core.ForeColor;
            }

            if (element is IStiRegionMapElement)
            {
                var style = report.Styles[customStyleName] as StiMapStyle;
                return style == null ? Color.Transparent : style.LabelForeground;
            }

            if (element is IStiControlElement)
            {
                var style = report.Styles[customStyleName] as StiDialogStyle;
                return style == null ? Color.Transparent : style.ForeColor;
            }

            return Color.Transparent;
        }

        public static Color GetForeColor(StiElementStyleIdent ident)
        {
            if (ident == StiElementStyleIdent.Silver)
                return StiColor.Get("e9f4fc");

            if (ident == StiElementStyleIdent.AliceBlue)
                return StiColor.Get("2e2e2e");

            if (ident == StiElementStyleIdent.Sienna)
                return StiColor.Get("4c453d");

            return IsDarkStyle(ident)
                ? StiColor.Get("dddddd")
                : StiElementConsts.ForegroundColor;
        }

        public static Color GetNativeForeColor(object element = null)
        {
            return GetForeColor(GetStyle(element));
        }

        public static Color GetSelectedForeColor(object element)
        {
            var styleIdent = GetStyle(element);
            if (styleIdent == StiElementStyleIdent.Custom)
            {
                var report = (element as StiComponent).Report;
                var elementStyle = element as IStiDashboardElementStyle;
                if (elementStyle != null)
                {
                    var style = report.Styles[elementStyle.CustomStyleName] as StiDialogStyle;
                    if (style != null && style.SelectedForeColor != Color.Transparent)
                        return style.SelectedForeColor;
                }
            }

            return GetControlStyle(element).ForeColor;
        }

        public static Color GetSelectedBackColor(object element)
        {
            var styleIdent = GetStyle(element);
            if (styleIdent == StiElementStyleIdent.Custom)
            {
                var report = (element as StiComponent).Report;
                var elementStyle = element as IStiDashboardElementStyle;
                if (elementStyle != null)
                {
                    var style = report.Styles[elementStyle.CustomStyleName] as StiDialogStyle;
                    if (style != null && style.SelectedBackColor != Color.Transparent)
                        return style.SelectedBackColor;
                }
            }

            return GetControlStyle(element).BackColor;
        }

        public static Color GetGlyphColor(object element)
        {
            var styleIdent = GetStyle(element);
            if (styleIdent == StiElementStyleIdent.Custom)
            {
                var report = (element as StiComponent).Report;
                var elementStyle = element as IStiDashboardElementStyle;
                if (elementStyle != null)
                {
                    var style = report.Styles[elementStyle.CustomStyleName] as StiDialogStyle;
                    if (style != null && style.GlyphColor != Color.Transparent)
                        return style.GlyphColor;
                }
            }

            return GetControlStyle(element).GlyphColor;
        }

        public static Color GetGlyphColor(IStiIndicatorElement element)
        {
            if (element.GlyphColor != Color.Transparent)
                return element.GlyphColor;

            var styleIdent = GetStyle(element);
            if (styleIdent == StiElementStyleIdent.Custom)
            {
                var style = element.Report.Styles[element.CustomStyleName] as StiIndicatorStyle;
                if (style != null && style.GlyphColor != Color.Transparent)
                    return style.GlyphColor;
            }

            var styleIndicator = GetIndicatorStyle(element);
            return styleIndicator.GlyphColor;
        }

        public static Color GetSeparatorColor(object element)
        {
            var styleIdent = GetStyle(element);
            if (styleIdent == StiElementStyleIdent.Custom)
            {
                var report = (element as StiComponent).Report;
                var elementStyle = element as IStiDashboardElementStyle;
                if (elementStyle != null)
                {
                    var style = report.Styles[elementStyle.CustomStyleName] as StiDialogStyle;
                    if (style != null && style.SeparatorColor != Color.Transparent)
                        return style.SeparatorColor;
                }
            }

            return GetControlStyle(element).SeparatorColor;
        }

        public static Color GetBackColor(object element, Color? defaultColor = null, bool allowOpacity = false)
        {
            var backColor = element as IStiBackColor;

            if (backColor != null)
            {
                var expBackColor = StiDashboardExpressionHelper.GetBackColor(element, backColor.BackColor);
                if (Color.Transparent.ToArgb() != expBackColor.ToArgb())
                {
                    return expBackColor.A != 255 ? (allowOpacity ? expBackColor : Color.FromArgb(255, expBackColor)) : expBackColor;
                }
            }

            var styleColor = GetStyleBackColor(element);
            if (allowOpacity && GetStyle(element) == StiElementStyleIdent.Custom)
                return styleColor;

            else if (styleColor != Color.Transparent)
                return styleColor;

            var controlElement = element as IStiControlElement;
            if (controlElement != null && GetControlStyle(controlElement).BackColor != Color.Transparent)
                return GetControlStyle(controlElement).BackColor;

            if (defaultColor != null)
                return defaultColor.Value;


            return GetBackColor(GetStyle(element));
        }

        private static Color GetStyleBackColor(object element)
        {
            var styleIdent = GetStyle(element);
            if (styleIdent != StiElementStyleIdent.Custom)
                return Color.Transparent;

            var customStyleName = (element as IStiDashboardElementStyle)?.CustomStyleName;
            var report = (element as StiComponent).Report;

            if (element is IStiIndicatorElement indicatorElement)
            {
                var style = GetIndicatorStyle(indicatorElement) as StiCustomIndicatorElementStyle;
                return style == null ? Color.Transparent : style.BackColor;
            }

            if (element is IStiCardsElement cardsElement)
            {
                var style = GetCardsStyle(cardsElement) as StiCustomCardsElementStyle;
                return style == null ? Color.Transparent : style.BackColor;
            }

            if (element is IStiProgressElement)
            {
                var style = GetProgressStyle(element as IStiProgressElement) as StiCustomProgressElementStyle;
                return style == null ? Color.Transparent : style.BackColor;
            }

            if (element is IStiGaugeElement)
            {
                var style = report.Styles.GetCustomGaugeStyle(customStyleName);
                return style?.Core?.Brush == null ? Color.Transparent : StiBrush.ToColor(style.Core.Brush);
            }

            if (element is IStiChartElement)
            {
                var style = report.Styles[customStyleName] as StiChartStyle;
                return style?.Brush == null ? Color.Transparent : StiBrush.ToColor(style.Brush);
            }

            if (element is IStiRegionMapElement)
            {
                var style = report.Styles[customStyleName] as StiMapStyle;
                return style == null ? Color.Transparent : style.BackColor;
            }

            if (element is IStiTableElement)
            {
                var style = report.Styles[customStyleName] as StiTableStyle;
                return style == null ? Color.Transparent : style.BackColor;
            }

            if (element is IStiPivotTableElement)
            {
                var style = report.Styles[customStyleName] as StiCrossTabStyle;
                return style == null ? Color.Transparent : style.BackColor;
            }

            if (element is IStiControlElement || element is IStiComponentUI)
            {
                var style = report.Styles[customStyleName] as StiDialogStyle;
                return style == null ? Color.Transparent : style.BackColor;
            }

            return Color.Transparent;
        }

        private static Color GetStyleHotBackColor(IStiElement element)
        {
            var styleIdent = GetStyle(element);
            if (styleIdent != StiElementStyleIdent.Custom)
                return Color.Transparent;

            var customStyleName = (element as IStiDashboardElementStyle)?.CustomStyleName;

            if (element is IStiIndicatorElement)
            {
                var style = GetIndicatorStyle(element as IStiIndicatorElement) as StiCustomIndicatorElementStyle;
                return style == null ? Color.Transparent : style.HotBackColor;
            }

            if (element is IStiControlElement)
            {
                var style = element.Report.Styles[customStyleName] as StiDialogStyle;
                return style == null ? Color.Transparent : style.HotBackColor;
            }

            return Color.Transparent;
        }

        public static Color GetHotBackColor(IStiElement element)
        {
            var styleIdent = GetStyle(element);

            if (styleIdent == StiElementStyleIdent.Custom)
            {
                return GetStyleHotBackColor(element);
            }

            if (IsDarkStyle(styleIdent))
                return StiColorUtils.Light(GetBackColor(element), 15);
            else
                return StiColorUtils.Dark(GetBackColor(element), 15);
        }

        public static Color GetBackColor(StiElementStyleIdent style)
        {
            switch (style)
            {
                case StiElementStyleIdent.DarkBlue:
                    return StiColor.Get("#0a325a");

                case StiElementStyleIdent.SlateGray:
                    return StiColor.Get("#33475b");

                case StiElementStyleIdent.DarkGray:
                    return StiColor.Get("#595b65");

                case StiElementStyleIdent.DarkGreen:
                    return StiColor.Get("#3f745e");

                case StiElementStyleIdent.DarkTurquoise:
                    return StiColor.Get("#235e6d");

                case StiElementStyleIdent.Silver:
                    return StiColor.Get("#6d7e8b");

                case StiElementStyleIdent.AliceBlue:
                    return StiColor.Get("#f2f5fc");

                case StiElementStyleIdent.Sienna:
                    return StiColor.Get("#fefefe");

                default:
                    return StiElementConsts.BackgroundColor;
            }
        }

        public static Color GetTitleForeColor(IStiElement element)
        {
            var ident = GetStyle(element);
            if (ident == StiElementStyleIdent.Silver)
                return StiColor.Get("e9f4fc");

            if (ident == StiElementStyleIdent.AliceBlue)
                return StiColor.Get("1f377f");

            return IsDarkStyle(ident) 
                ? StiColor.Get("dddddd") 
                : StiElementConsts.TitleFont.Color;
        }

        public static StiGaugeStyleXF GetGaugeStyle(IStiGaugeElement element)
        {
            return GetGaugeStyle(GetStyle(element));
        }

        public static StiGaugeStyleXF GetGaugeStyle(StiElementStyleIdent style)
        {
            switch (style)
            {
                case StiElementStyleIdent.Orange:
                    return new StiGaugeStyleXF24();

                case StiElementStyleIdent.Green:
                    return new StiGaugeStyleXF25();

                case StiElementStyleIdent.Turquoise:
                    return new StiGaugeStyleXF26();

                case StiElementStyleIdent.SlateGray:
                    return new StiGaugeStyleXF27();

                case StiElementStyleIdent.DarkBlue:
                    return new StiGaugeStyleXF28();

                case StiElementStyleIdent.Blue:
                    return new StiGaugeStyleXF29();

                case StiElementStyleIdent.DarkGray:
                    return new StiGaugeStyleXF30();

                case StiElementStyleIdent.DarkTurquoise:
                    return new StiGaugeStyleXF31();

                case StiElementStyleIdent.Silver:
                    return new StiGaugeStyleXF32();

                case StiElementStyleIdent.AliceBlue:
                    return new StiGaugeStyleXF33();

                case StiElementStyleIdent.DarkGreen:
                    return new StiGaugeStyleXF34();

                case StiElementStyleIdent.Sienna:
                    return new StiGaugeStyleXF35();

                default:
                    return new StiGaugeStyleXF29();
            }
        }

        public static IStiChartStyle GetChartStyle(IStiChartElement element)
        {
            return GetChartStyle(GetStyle(element));
        }

        public static IStiChartStyle GetChartStyle(StiElementStyleIdent style)
        {
            switch (style)
            {
                case StiElementStyleIdent.Orange:
                    var style24 = new StiStyle24();
                    style24.Core.MarkerVisible = false;
                    return style24;

                case StiElementStyleIdent.Green:
                    var style25 = new StiStyle25();
                    style25.Core.MarkerVisible = false;
                    return style25;

                case StiElementStyleIdent.Turquoise:
                    var style26 = new StiStyle26();
                    style26.Core.MarkerVisible = false;
                    return style26;

                case StiElementStyleIdent.SlateGray:
                    var style27 = new StiStyle27();
                    style27.Core.MarkerVisible = false;
                    return style27;

                case StiElementStyleIdent.DarkBlue:
                    var style28 = new StiStyle28();
                    style28.Core.MarkerVisible = false;
                    return style28;

                case StiElementStyleIdent.Blue:
                    var style29 = new StiStyle29();
                    style29.Core.MarkerVisible = false;
                    return style29;

                case StiElementStyleIdent.DarkGray:
                    var style30 = new StiStyle30();
                    style30.Core.MarkerVisible = false;
                    return style30;

                case StiElementStyleIdent.DarkTurquoise:
                    var style31 = new StiStyle31();
                    style31.Core.MarkerVisible = false;
                    return style31;

                case StiElementStyleIdent.Silver:
                    var style32 = new StiStyle32();
                    style32.Core.MarkerVisible = false;
                    return style32;

                case StiElementStyleIdent.AliceBlue:
                    var style33 = new StiStyle33();
                    style33.Core.MarkerVisible = false;
                    return style33;

                case StiElementStyleIdent.DarkGreen:
                    var style34 = new StiStyle34();
                    style34.Core.MarkerVisible = false;
                    return style34;

                case StiElementStyleIdent.Sienna:
                    var style35 = new StiStyle35();
                    style35.Core.MarkerVisible = false;
                    return style35;

                default:
                    return new StiStyle29();
            }
        }

        public static StiMapStyleIdent GetMapStyleIdent(IStiRegionMapElement element)
        {
            switch (GetStyle(element))
            {
                case StiElementStyleIdent.Orange:
                    return StiMapStyleIdent.Style24;

                case StiElementStyleIdent.Green:
                    return StiMapStyleIdent.Style25;

                case StiElementStyleIdent.Turquoise:
                    return StiMapStyleIdent.Style26;

                case StiElementStyleIdent.SlateGray:
                    return StiMapStyleIdent.Style27;

                case StiElementStyleIdent.DarkBlue:
                    return StiMapStyleIdent.Style28;

                case StiElementStyleIdent.Blue:
                    return StiMapStyleIdent.Style29;

                case StiElementStyleIdent.DarkGray:
                    return StiMapStyleIdent.Style30;

                case StiElementStyleIdent.DarkTurquoise:
                    return StiMapStyleIdent.Style31;

                case StiElementStyleIdent.Silver:
                    return StiMapStyleIdent.Style32;

                case StiElementStyleIdent.AliceBlue:
                    return StiMapStyleIdent.Style33;

                case StiElementStyleIdent.DarkGreen:
                    return StiMapStyleIdent.Style34;

                case StiElementStyleIdent.Sienna:
                    return StiMapStyleIdent.Style35;

                default:
                    return StiMapStyleIdent.Style29;
            }
        }

        public static StiMapStyleFX GetMapStyle(IStiRegionMapElement element)
        {
            return GetMapStyle(GetStyle(element));
        }

        public static StiMapStyleFX GetMapStyle(StiElementStyleIdent style)
        {
            switch (style)
            {
                case StiElementStyleIdent.Orange:
                    return new StiMap24StyleFX();

                case StiElementStyleIdent.Green:
                    return new StiMap25StyleFX();

                case StiElementStyleIdent.Turquoise:
                    return new StiMap26StyleFX();

                case StiElementStyleIdent.SlateGray:
                    return new StiMap27StyleFX();

                case StiElementStyleIdent.DarkBlue:
                    return new StiMap28StyleFX();

                case StiElementStyleIdent.Blue:
                    return new StiMap29StyleFX();

                case StiElementStyleIdent.DarkGray:
                    return new StiMap30StyleFX();

                case StiElementStyleIdent.DarkTurquoise:
                    return new StiMap31StyleFX();

                case StiElementStyleIdent.Silver:
                    return new StiMap32StyleFX();

                case StiElementStyleIdent.AliceBlue:
                    return new StiMap33StyleFX();

                case StiElementStyleIdent.DarkGreen:
                    return new StiMap34StyleFX();

                case StiElementStyleIdent.Sienna:
                    return new StiMap35StyleFX();

                default:
                    return new StiMap29StyleFX();
            }
        }

        public static StiControlElementStyle GetControlStyle(object element)
        {
            switch (GetStyle(element))
            {
                case StiElementStyleIdent.Orange:
                    return new StiOrangeControlElementStyle();

                case StiElementStyleIdent.Green:
                    return new StiGreenControlElementStyle();

                case StiElementStyleIdent.Turquoise:
                    return new StiTurquoiseControlElementStyle();

                case StiElementStyleIdent.SlateGray:
                    return new StiSlateGrayControlElementStyle();

                case StiElementStyleIdent.DarkBlue:
                    return new StiDarkBlueControlElementStyle();

                case StiElementStyleIdent.Blue:
                    return new StiBlueControlElementStyle();

                case StiElementStyleIdent.DarkGray:
                    return new StiDarkGrayControlElementStyle();

                case StiElementStyleIdent.DarkTurquoise:
                    return new StiDarkTurquoiseControlElementStyle();

                case StiElementStyleIdent.Silver:
                    return new StiSilverControlElementStyle();

                case StiElementStyleIdent.AliceBlue:
                    return new StiAliceBlueControlElementStyle();

                case StiElementStyleIdent.DarkGreen:
                    return new StiDarkGreenControlElementStyle();

                case StiElementStyleIdent.Sienna:
                    return new StiSiennaControlElementStyle();

                case StiElementStyleIdent.Custom:
                    var elementStyle = element as IStiDashboardElementStyle;
                    if (elementStyle != null && !string.IsNullOrWhiteSpace(elementStyle.CustomStyleName))
                    {
                        var report = (element as StiComponent).Report;
                        var styleName = elementStyle.CustomStyleName.ToLowerInvariant();
                        var style = report.Styles.ToList()
                            .Where(s => s is StiDialogStyle && s.Name != null)
                            .Cast<StiDialogStyle>().FirstOrDefault(s => s.Name.ToLowerInvariant() == styleName);

                        if (style != null)
                            return new StiCustomControlElementStyle(style);
                    }
                    break;
            }

            return new StiDarkBlueControlElementStyle();
        }

        public static StiIndicatorElementStyle GetIndicatorStyle(IStiIndicatorElement element)
        {
            switch (GetStyle(element))
            {
                case StiElementStyleIdent.Orange:
                    return new StiOrangeIndicatorElementStyle();

                case StiElementStyleIdent.Green:
                    return new StiGreenIndicatorElementStyle();

                case StiElementStyleIdent.Turquoise:
                    return new StiTurquoiseIndicatorElementStyle();

                case StiElementStyleIdent.SlateGray:
                    return new StiSlateGrayIndicatorElementStyle();

                case StiElementStyleIdent.DarkBlue:
                    return new StiDarkBlueIndicatorElementStyle();

                case StiElementStyleIdent.Blue:
                    return new StiBlueIndicatorElementStyle();

                case StiElementStyleIdent.DarkGray:
                    return new StiDarkGrayIndicatorElementStyle();

                case StiElementStyleIdent.DarkTurquoise:
                    return new StiDarkTurquoiseIndicatorElementStyle();

                case StiElementStyleIdent.Silver:
                    return new StiSilverIndicatorElementStyle();

                case StiElementStyleIdent.AliceBlue:
                    return new StiAliceBlueIndicatorElementStyle();

                case StiElementStyleIdent.DarkGreen:
                    return new StiDarkGreenIndicatorElementStyle();

                case StiElementStyleIdent.Sienna:
                    return new StiSiennaIndicatorElementStyle();

                case StiElementStyleIdent.Custom:
                    if (!string.IsNullOrWhiteSpace(element.CustomStyleName))
                    {
                        var styleName = element.CustomStyleName.ToLowerInvariant();
                        var style = element.Report.Styles.ToList()
                            .Where(s => s is StiIndicatorStyle && s.Name != null)
                            .Cast<StiIndicatorStyle>().FirstOrDefault(s => s.Name.ToLowerInvariant() == styleName);

                        if (style != null)
                            return new StiCustomIndicatorElementStyle(style);
                    }
                    break;
            }

            return new StiOrangeIndicatorElementStyle();
        }

        public static StiProgressElementStyle GetProgressStyle(IStiProgressElement element)
        {
            switch (GetStyle(element))
            {
                case StiElementStyleIdent.Blue:
                    return new StiBlueProgressElementStyle();

                case StiElementStyleIdent.Green:
                    return new StiGreenProgressElementStyle();

                case StiElementStyleIdent.Turquoise:
                    return new StiTurquoiseProgressElementStyle();

                case StiElementStyleIdent.SlateGray:
                    return new StiSlateGrayProgressElementStyle();

                case StiElementStyleIdent.DarkBlue:
                    return new StiDarkBlueProgressElementStyle();

                case StiElementStyleIdent.DarkGray:
                    return new StiDarkGrayProgressElementStyle();

                case StiElementStyleIdent.DarkTurquoise:
                    return new StiDarkTurquoiseProgressElementStyle();

                case StiElementStyleIdent.Silver:
                    return new StiSilverProgressElementStyle();

                case StiElementStyleIdent.AliceBlue:
                    return new StiAliceBlueProgressElementStyle();

                case StiElementStyleIdent.DarkGreen:
                    return new StiDarkGreenProgressElementStyle();

                case StiElementStyleIdent.Sienna:
                    return new StiSiennaProgressElementStyle();

                case StiElementStyleIdent.Custom:
                    if (!string.IsNullOrWhiteSpace(element.CustomStyleName))
                    {
                        var styleName = element.CustomStyleName.ToLowerInvariant();
                        var style = element.Report.Styles.ToList()
                            .Where(s => s is StiProgressStyle && s.Name != null)
                            .Cast<StiProgressStyle>().FirstOrDefault(s => s.Name.ToLowerInvariant() == styleName);

                        if (style != null)
                            return new StiCustomProgressElementStyle(style);
                    }
                    break;
            }

            return new StiOrangeProgressElementStyle();
        }

        public static StiTableElementStyle GetTableStyle(IStiTableElement element)
        {
            var elementStyle = GetStyle(element);
            switch (elementStyle)
            {
                case StiElementStyleIdent.Custom:
                    if (!string.IsNullOrWhiteSpace(element.CustomStyleName))
                    {
                        var styleName = element.CustomStyleName.ToLowerInvariant();
                        var style = element.Report.Styles.ToList()
                            .Where(s => s is StiTableStyle && s.Name != null)
                            .Cast<StiTableStyle>().FirstOrDefault(s => s.Name.ToLowerInvariant() == styleName);

                        if (style != null)
                            return new StiCustomTableElementStyle(style);
                    }
                    break;

                default:
                    return GetTableStyle(elementStyle);
            }

            return new StiBlueTableElementStyle();
        }

        public static StiTableElementStyle GetTableStyle(StiElementStyleIdent style)
        {
            switch (style)
            {
                case StiElementStyleIdent.Orange:
                    return new StiOrangeTableElementStyle();

                case StiElementStyleIdent.Green:
                    return new StiGreenTableElementStyle();

                case StiElementStyleIdent.Turquoise:
                    return new StiTurquoiseTableElementStyle();

                case StiElementStyleIdent.SlateGray:
                    return new StiSlateGrayTableElementStyle();

                case StiElementStyleIdent.DarkBlue:
                    return new StiDarkBlueTableElementStyle();

                case StiElementStyleIdent.Blue:
                    return new StiBlueTableElementStyle();

                case StiElementStyleIdent.DarkGray:
                    return new StiDarkGrayTableElementStyle();

                case StiElementStyleIdent.DarkTurquoise:
                    return new StiDarkTurquoiseTableElementStyle();

                case StiElementStyleIdent.Silver:
                    return new StiSilverTableElementStyle();

                case StiElementStyleIdent.AliceBlue:
                    return new StiAliceBlueTableElementStyle();

                case StiElementStyleIdent.DarkGreen:
                    return new StiDarkGreenTableElementStyle();

                case StiElementStyleIdent.Sienna:
                    return new StiSiennaTableElementStyle();
            }

            return new StiBlueTableElementStyle();
        }

        public static StiCardsElementStyle GetCardsStyle(IStiCardsElement element)
        {
            var elementStyle = GetStyle(element);
            switch (elementStyle)
            {
                case StiElementStyleIdent.Custom:
                    if (!string.IsNullOrWhiteSpace(element.CustomStyleName))
                    {
                        var styleName = element.CustomStyleName.ToLowerInvariant();
                        var style = element.Report.Styles.ToList()
                            .Where(s => s is StiCardsStyle && s.Name != null)
                            .Cast<StiCardsStyle>().FirstOrDefault(s => s.Name.ToLowerInvariant() == styleName);

                        if (style != null)
                            return new StiCustomCardsElementStyle(style);
                    }
                    break;

                default:
                    return GetCardsStyle(elementStyle);
            }

            return new StiBlueCardsElementStyle();
        }

        public static StiCardsElementStyle GetCardsStyle(StiElementStyleIdent style)
        {
            switch (style)
            {
                case StiElementStyleIdent.Orange:
                    return new StiOrangeCardsElementStyle();

                case StiElementStyleIdent.Green:
                    return new StiGreenCardsElementStyle();

                case StiElementStyleIdent.Turquoise:
                    return new StiTurquoiseCardsElementStyle();

                case StiElementStyleIdent.SlateGray:
                    return new StiSlateGrayCardsElementStyle();

                case StiElementStyleIdent.DarkBlue:
                    return new StiDarkBlueCardsElementStyle();

                case StiElementStyleIdent.Blue:
                    return new StiBlueCardsElementStyle();

                case StiElementStyleIdent.DarkGray:
                    return new StiDarkGrayCardsElementStyle();

                case StiElementStyleIdent.DarkTurquoise:
                    return new StiDarkTurquoiseCardsElementStyle();

                case StiElementStyleIdent.Silver:
                    return new StiSilverCardsElementStyle();

                case StiElementStyleIdent.AliceBlue:
                    return new StiAliceBlueCardsElementStyle();

                case StiElementStyleIdent.DarkGreen:
                    return new StiDarkGreenCardsElementStyle();

                case StiElementStyleIdent.Sienna:
                    return new StiSiennaCardsElementStyle();
            }

            return new StiBlueCardsElementStyle();
        }

        public static StiPivotElementStyle GetPivotTableStyle(IStiPivotTableElement element)
        {
            switch (GetStyle(element))
            {
                case StiElementStyleIdent.Orange:
                    return new StiOrangePivotElementStyle();

                case StiElementStyleIdent.Green:
                    return new StiGreenPivotElementStyle();

                case StiElementStyleIdent.Turquoise:
                    return new StiTurquoisePivotElementStyle();

                case StiElementStyleIdent.SlateGray:
                    return new StiSlateGrayPivotElementStyle();

                case StiElementStyleIdent.DarkBlue:
                    return new StiDarkBluePivotElementStyle();

                case StiElementStyleIdent.Blue:
                    return new StiBluePivotElementStyle();

                case StiElementStyleIdent.DarkGray:
                    return new StiDarkGrayPivotElementStyle();

                case StiElementStyleIdent.DarkTurquoise:
                    return new StiDarkTurquoisePivotElementStyle();

                case StiElementStyleIdent.Silver:
                    return new StiSilverPivotElementStyle();

                case StiElementStyleIdent.AliceBlue:
                    return new StiAliceBluePivotElementStyle();

                case StiElementStyleIdent.DarkGreen:
                    return new StiDarkGreenPivotElementStyle();

                case StiElementStyleIdent.Sienna:
                    return new StiSiennaPivotElementStyle();

                case StiElementStyleIdent.Custom:
                    if (!string.IsNullOrWhiteSpace(element.CustomStyleName))
                    {
                        var styleName = element.CustomStyleName.ToLowerInvariant();
                        var style = element.Report.Styles.ToList()
                            .Where(s => s is StiCrossTabStyle && s.Name != null)
                            .Cast<StiCrossTabStyle>().FirstOrDefault(s => s.Name.ToLowerInvariant() == styleName);

                        if (style != null)
                        return new StiCustomPivotElementStyle(style);
                    }
                    break;
            }

            return new StiOrangePivotElementStyle();
        }

        public static StiElementStyleIdent GetStyle(object element)
        {
            var elementStyle = element as IStiDashboardElementStyle;
            var dashboardStyle = (element as StiComponent)?.Page as IStiDashboardElementStyle;

            if (elementStyle == null && dashboardStyle == null) 
				return StiElementStyleIdent.Blue;

            if (elementStyle == null)
                return dashboardStyle.Style;

            if (dashboardStyle == null)
                return elementStyle.Style;

            return elementStyle.Style == StiElementStyleIdent.Auto
                ? dashboardStyle.Style
                : elementStyle.Style;
        }
        #endregion

        #region Methods.Helpers
        internal static FontFamily GetIconFontFamily()
        {
            if (iconFontFamily == null)
            {
                var type = Type.GetType("Stimulsoft.Dashboard.Helpers.StiFontIconsHelper, Stimulsoft.Dashboard");
                if (type != null)
                {
                    var method = type.GetMethod("GetFontFamaliIcons", BindingFlags.NonPublic | BindingFlags.Static);
                    if (method != null)
                        iconFontFamily = method.Invoke(null, null) as FontFamily;
                }
            }

            if (iconFontFamily == null)
                iconFontFamily = new FontFamily("Arial");

            return iconFontFamily;
        }
        #endregion
    }
}