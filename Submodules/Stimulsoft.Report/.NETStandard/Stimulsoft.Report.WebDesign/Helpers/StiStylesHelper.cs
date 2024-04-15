#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports  											}
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
using System.Reflection;
using System.Collections;
using Stimulsoft.Report.Components;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;
using System.IO;
using System.Collections.Generic;
using Stimulsoft.Report.Chart;
using Stimulsoft.Report.CrossTab;
using Stimulsoft.Report.Maps;
using Stimulsoft.Report.Gauge;
using Stimulsoft.Report.Components.Table;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Dashboard.Styles;
using System.Linq;
using System.Drawing;
using Stimulsoft.Report.Dashboard.Styles.Cards;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;
using static Stimulsoft.Report.Images.StiReportImages;
using Stimulsoft.Report.Web.Helpers.Dashboards;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Web
{
    internal class StiStylesHelper
    {
        #region Style Item
        private static Hashtable GetStyleProperties(StiBaseStyle style)
        {
            Hashtable properties = new Hashtable();
            properties["name"] = style.Name;
            properties["collectionName"] = style.CollectionName;
            properties["description"] = style.Description;
            properties["conditions"] = GetStyleConditionsProprty(style.Conditions);
            string brush = StiReportEdit.GetPropertyValue("Brush", style) as string;
            if (!String.IsNullOrEmpty(brush)) properties["brush"] = brush;
            string textBrush = StiReportEdit.GetPropertyValue("TextBrush", style) as string;
            if (textBrush != string.Empty) properties["textBrush"] = textBrush;
            string border = StiReportEdit.GetPropertyValue("Border", style) as string;
            if (!String.IsNullOrEmpty(border)) properties["border"] = border;
            string font = StiReportEdit.GetPropertyValue("Font", style) as string;
            if (!String.IsNullOrEmpty(font)) properties["font"] = font;
            string horAlignment = StiReportEdit.GetPropertyValue("HorAlignment", style) as string;
            if (horAlignment != string.Empty) properties["horAlignment"] = horAlignment;
            string vertAlignment = StiReportEdit.GetPropertyValue("VertAlignment", style) as string;
            if (horAlignment != string.Empty) properties["vertAlignment"] = vertAlignment;
            string color = StiReportEdit.GetPropertyValue("Color", style) as string;
            if (!String.IsNullOrEmpty(color)) properties["color"] = color;
            string foreColor = StiReportEdit.GetPropertyValue("ForeColor", style) as string;
            if (!String.IsNullOrEmpty(foreColor)) properties["foreColor"] = foreColor;
            string backColor = StiReportEdit.GetPropertyValue("BackColor", style) as string;
            if (!String.IsNullOrEmpty(backColor)) properties["backColor"] = backColor;
            string basicStyleColor = StiReportEdit.GetPropertyValue("BasicStyleColor", style) as string;
            if (!String.IsNullOrEmpty(basicStyleColor)) properties["basicStyleColor"] = basicStyleColor;
            string brushType = StiReportEdit.GetPropertyValue("BrushType", style) as string;
            if (!String.IsNullOrEmpty(brushType)) properties["brushType"] = brushType;

            string[] boolProps = { "AllowUseBackColor", "AllowUseForeColor", "AllowUseBorderFormatting", "AllowUseBorderSides", "AllowUseBorderSidesFromLocation", "AllowUseBrush",
                "AllowUseFont", "AllowUseImage", "AllowUseNegativeTextBrush", "AllowUseTextBrush", "AllowUseTextFormat", "AllowUseHorAlignment", "AllowUseVertAlignment"};
            foreach (string propertyName in boolProps)
            {
                bool property;
                PropertyInfo pi = style.GetType().GetProperty(propertyName);
                if (pi != null)
                {
                    property = (bool)pi.GetValue(style, null);
                    string propNameLower = propertyName[0].ToString().ToLowerInvariant() + propertyName.Remove(0, 1);
                    properties[propNameLower] = property;
                }
            }

            if (style is StiCrossTabStyle)
            {
                var crossTabStyle = style as StiCrossTabStyle;
                properties["alternatingCellBackColor"] = StiReportEdit.GetStringFromColor(crossTabStyle.AlternatingCellBackColor);
                properties["alternatingCellForeColor"] = StiReportEdit.GetStringFromColor(crossTabStyle.AlternatingCellForeColor);
                properties["cellBackColor"] = StiReportEdit.GetStringFromColor(crossTabStyle.CellBackColor);
                properties["cellForeColor"] = StiReportEdit.GetStringFromColor(crossTabStyle.CellForeColor);
                properties["lineColor"] = StiReportEdit.GetStringFromColor(crossTabStyle.LineColor);
                properties["columnHeaderBackColor"] = StiReportEdit.GetStringFromColor(crossTabStyle.ColumnHeaderBackColor);
                properties["columnHeaderForeColor"] = StiReportEdit.GetStringFromColor(crossTabStyle.ColumnHeaderForeColor);
                properties["hotColumnHeaderBackColor"] = StiReportEdit.GetStringFromColor(crossTabStyle.HotColumnHeaderBackColor);
                properties["hotRowHeaderBackColor"] = StiReportEdit.GetStringFromColor(crossTabStyle.HotRowHeaderBackColor);
                properties["rowHeaderBackColor"] = StiReportEdit.GetStringFromColor(crossTabStyle.RowHeaderBackColor);
                properties["rowHeaderForeColor"] = StiReportEdit.GetStringFromColor(crossTabStyle.RowHeaderForeColor);
                properties["selectedCellBackColor"] = StiReportEdit.GetStringFromColor(crossTabStyle.SelectedCellBackColor);
                properties["selectedCellForeColor"] = StiReportEdit.GetStringFromColor(crossTabStyle.SelectedCellForeColor);
                properties["totalCellColumnBackColor"] = StiReportEdit.GetStringFromColor(crossTabStyle.TotalCellColumnBackColor);
                properties["totalCellColumnForeColor"] = StiReportEdit.GetStringFromColor(crossTabStyle.TotalCellColumnForeColor);
                properties["totalCellRowBackColor"] = StiReportEdit.GetStringFromColor(crossTabStyle.TotalCellRowBackColor);
                properties["totalCellRowForeColor"] = StiReportEdit.GetStringFromColor(crossTabStyle.TotalCellRowForeColor);
                properties.Remove("color");
            }
            else if (style is StiMapStyle)
            {
                var mapStyle = style as StiMapStyle;
                properties["borderColor"] = StiReportEdit.GetStringFromColor(mapStyle.BorderColor);
                properties["borderSize"] = StiReportEdit.DoubleToStr(mapStyle.BorderSize);
                properties["colors"] = StiReportEdit.GetColorsCollectionProperty(mapStyle.Colors);
                properties["defaultColor"] = StiReportEdit.GetStringFromColor(mapStyle.DefaultColor);
                properties["heatmap.color"] = StiReportEdit.GetStringFromColor(mapStyle.Heatmap.Color);
                properties["heatmap.zeroColor"] = StiReportEdit.GetStringFromColor(mapStyle.Heatmap.ZeroColor);
                properties["heatmap.mode"] = mapStyle.Heatmap.Mode;
                properties["heatmapWithGroup.colors"] = StiReportEdit.GetColorsCollectionProperty(mapStyle.HeatmapWithGroup.Colors);
                properties["heatmapWithGroup.zeroColor"] = StiReportEdit.GetStringFromColor(mapStyle.HeatmapWithGroup.ZeroColor);
                properties["heatmapWithGroup.mode"] = ((StiMapStyle)style).HeatmapWithGroup.Mode;
                properties["individualColor"] = StiReportEdit.GetStringFromColor(mapStyle.IndividualColor);
                properties["labelForeground"] = StiReportEdit.GetStringFromColor(mapStyle.LabelForeground);
                properties["labelShadowForeground"] = StiReportEdit.GetStringFromColor(mapStyle.LabelShadowForeground);
                properties["toolTipBrush"] = StiReportEdit.BrushToStr(mapStyle.ToolTipBrush);
                properties["toolTipBorder"] = StiReportEdit.SimpleBorderToStr(mapStyle.ToolTipBorder);
                properties["toolTipTextBrush"] = StiReportEdit.BrushToStr(mapStyle.ToolTipTextBrush);
                properties["toolTipCornerRadius"] = StiReportEdit.CornerRadiusToStr(mapStyle.ToolTipCornerRadius);
            }
            else if (style is StiChartStyle)
            {
                var chartStyle = style as StiChartStyle;
                properties["gridLinesHorColor"] = StiReportEdit.GetStringFromColor(chartStyle.GridLinesHorColor);
                properties["gridLinesVertColor"] = StiReportEdit.GetStringFromColor(chartStyle.GridLinesVertColor);
                properties["chartAreaShowShadow"] = chartStyle.ChartAreaShowShadow;
                properties["chartAreaBrush"] = StiReportEdit.BrushToStr(chartStyle.ChartAreaBrush);
                properties["chartAreaBorderColor"] = StiReportEdit.GetStringFromColor(chartStyle.ChartAreaBorderColor);
                properties["seriesLabelsBorderColor"] = StiReportEdit.GetStringFromColor(chartStyle.SeriesLabelsBorderColor);
                properties["seriesLabelsColor"] = StiReportEdit.GetStringFromColor(chartStyle.SeriesLabelsColor);
                properties["seriesLabelsBrush"] = StiReportEdit.BrushToStr(chartStyle.SeriesLabelsBrush);
                properties["trendLineShowShadow"] = chartStyle.TrendLineShowShadow;
                properties["trendLineColor"] = StiReportEdit.GetStringFromColor(chartStyle.TrendLineColor);
                properties["legendBorderColor"] = StiReportEdit.GetStringFromColor(chartStyle.LegendBorderColor);
                properties["legendBrush"] = StiReportEdit.BrushToStr(chartStyle.LegendBrush);
                properties["legendLabelsColor"] = StiReportEdit.GetStringFromColor(chartStyle.LegendLabelsColor);
                properties["legendTitleColor"] = StiReportEdit.GetStringFromColor(chartStyle.LegendTitleColor);
                properties["axisLabelsColor"] = StiReportEdit.GetStringFromColor(chartStyle.AxisLabelsColor);
                properties["axisLineColor"] = StiReportEdit.GetStringFromColor(chartStyle.AxisLineColor);
                properties["axisTitleColor"] = StiReportEdit.GetStringFromColor(chartStyle.AxisTitleColor);
                properties["interlacingHorBrush"] = StiReportEdit.BrushToStr(chartStyle.InterlacingHorBrush);
                properties["interlacingVertBrush"] = StiReportEdit.BrushToStr(chartStyle.InterlacingVertBrush);
                properties["seriesLighting"] = chartStyle.SeriesLighting;
                properties["seriesShowShadow"] = chartStyle.SeriesShowShadow;
                properties["seriesShowBorder"] = chartStyle.SeriesShowBorder;
                properties["seriesBorderThickness"] = chartStyle.SeriesBorderThickness.ToString();
                properties["seriesCornerRadius"] = StiReportEdit.CornerRadiusToStr(chartStyle.SeriesCornerRadius);
                properties["styleColors"] = StiReportEdit.GetColorsCollectionProperty(chartStyle.StyleColors);
                properties["markerVisible"] = chartStyle.MarkerVisible;
                properties["toolTipBrush"] = StiReportEdit.BrushToStr(chartStyle.ToolTipBrush);
                properties["toolTipBorder"] = StiReportEdit.SimpleBorderToStr(chartStyle.ToolTipBorder);
                properties["toolTipTextBrush"] = StiReportEdit.BrushToStr(chartStyle.ToolTipTextBrush);
                properties["toolTipCornerRadius"] = StiReportEdit.CornerRadiusToStr(chartStyle.ToolTipCornerRadius);
            }
            else if (style is StiTableStyle)
            {
                var tableStyle = style as StiTableStyle;
                properties["alternatingDataColor"] = StiReportEdit.GetStringFromColor(tableStyle.AlternatingDataColor);
                properties["alternatingDataForeground"] = StiReportEdit.GetStringFromColor(tableStyle.AlternatingDataForeground);
                properties["dataColor"] = StiReportEdit.GetStringFromColor(tableStyle.DataColor);
                properties["dataForeground"] = StiReportEdit.GetStringFromColor(tableStyle.DataForeground);
                properties["footerColor"] = StiReportEdit.GetStringFromColor(tableStyle.FooterColor);
                properties["footerForeground"] = StiReportEdit.GetStringFromColor(tableStyle.FooterForeground);
                properties["gridColor"] = StiReportEdit.GetStringFromColor(tableStyle.GridColor);
                properties["headerColor"] = StiReportEdit.GetStringFromColor(tableStyle.HeaderColor);
                properties["headerForeground"] = StiReportEdit.GetStringFromColor(tableStyle.HeaderForeground);
                properties["hotHeaderColor"] = StiReportEdit.GetStringFromColor(tableStyle.HotHeaderColor);
                properties["selectedDataColor"] = StiReportEdit.GetStringFromColor(tableStyle.SelectedDataColor);
                properties["selectedDataForeground"] = StiReportEdit.GetStringFromColor(tableStyle.SelectedDataForeground);
            }
            else if (style is StiCardsStyle)
            {
                var cardsStyle = style as StiCardsStyle;
                properties["lineColor"] = StiReportEdit.GetStringFromColor(cardsStyle.LineColor);
                properties["seriesColors"] = StiReportEdit.GetColorsCollectionProperty(cardsStyle.SeriesColors);
                properties["toolTipBrush"] = StiReportEdit.BrushToStr(cardsStyle.ToolTipBrush);
                properties["toolTipBorder"] = StiReportEdit.SimpleBorderToStr(cardsStyle.ToolTipBorder);
                properties["toolTipTextBrush"] = StiReportEdit.BrushToStr(cardsStyle.ToolTipTextBrush);
                properties["toolTipCornerRadius"] = StiReportEdit.CornerRadiusToStr(cardsStyle.ToolTipCornerRadius);
            }
            else if (style is StiGaugeStyle)
            {
                properties["borderColor"] = StiReportEdit.GetStringFromColor(((StiGaugeStyle)style).BorderColor);
                properties["borderWidth"] = StiReportEdit.DoubleToStr(((StiGaugeStyle)style).BorderWidth);
                properties["brush"] = StiReportEdit.BrushToStr(((StiGaugeStyle)style).Brush);
                properties["linearBarBorderBrush"] = StiReportEdit.BrushToStr(((StiGaugeStyle)style).LinearBarBorderBrush);
                properties["linearBarBrush"] = StiReportEdit.BrushToStr(((StiGaugeStyle)style).LinearBarBrush);
                properties["linearBarEmptyBorderBrush"] = StiReportEdit.BrushToStr(((StiGaugeStyle)style).LinearBarEmptyBorderBrush);
                properties["linearBarEmptyBrush"] = StiReportEdit.BrushToStr(((StiGaugeStyle)style).LinearBarEmptyBrush);
                properties["linearScaleBrush"] = StiReportEdit.BrushToStr(((StiGaugeStyle)style).LinearScaleBrush);
                properties["markerBrush"] = StiReportEdit.BrushToStr(((StiGaugeStyle)style).MarkerBrush);
                properties["needleBorderBrush"] = StiReportEdit.BrushToStr(((StiGaugeStyle)style).NeedleBorderBrush);
                properties["needleBorderWidth"] = StiReportEdit.DoubleToStr(((StiGaugeStyle)style).NeedleBorderWidth);
                properties["needleBrush"] = StiReportEdit.BrushToStr(((StiGaugeStyle)style).NeedleBrush);
                properties["needleCapBorderBrush"] = StiReportEdit.BrushToStr(((StiGaugeStyle)style).NeedleCapBorderBrush);
                properties["needleCapBrush"] = StiReportEdit.BrushToStr(((StiGaugeStyle)style).NeedleCapBrush);
                properties["radialBarBorderBrush"] = StiReportEdit.BrushToStr(((StiGaugeStyle)style).RadialBarBorderBrush);
                properties["radialBarBrush"] = StiReportEdit.BrushToStr(((StiGaugeStyle)style).RadialBarBrush);
                properties["radialBarEmptyBorderBrush"] = StiReportEdit.BrushToStr(((StiGaugeStyle)style).RadialBarEmptyBorderBrush);
                properties["radialBarEmptyBrush"] = StiReportEdit.BrushToStr(((StiGaugeStyle)style).RadialBarEmptyBrush);
                properties["tickLabelMajorFont"] = StiReportEdit.FontToStr(((StiGaugeStyle)style).TickLabelMajorFont);
                properties["tickLabelMajorTextBrush"] = StiReportEdit.BrushToStr(((StiGaugeStyle)style).TickLabelMajorTextBrush);
                properties["tickLabelMinorFont"] = StiReportEdit.FontToStr(((StiGaugeStyle)style).TickLabelMinorFont);
                properties["tickLabelMinorTextBrush"] = StiReportEdit.BrushToStr(((StiGaugeStyle)style).TickLabelMinorTextBrush);
                properties["tickMarkMajorBorder"] = StiReportEdit.BrushToStr(((StiGaugeStyle)style).TickMarkMajorBorder);
                properties["tickMarkMajorBrush"] = StiReportEdit.BrushToStr(((StiGaugeStyle)style).TickMarkMajorBrush);
                properties["tickMarkMinorBorder"] = StiReportEdit.BrushToStr(((StiGaugeStyle)style).TickMarkMinorBorder);
                properties["tickMarkMinorBrush"] = StiReportEdit.BrushToStr(((StiGaugeStyle)style).TickMarkMinorBrush);
            }
            else if (style is StiDialogStyle)
            {
                properties["glyphColor"] = StiReportEdit.GetStringFromColor(((StiDialogStyle)style).GlyphColor);
                properties["hotBackColor"] = StiReportEdit.GetStringFromColor(((StiDialogStyle)style).HotBackColor);
                properties["hotForeColor"] = StiReportEdit.GetStringFromColor(((StiDialogStyle)style).HotForeColor);
                properties["hotGlyphColor"] = StiReportEdit.GetStringFromColor(((StiDialogStyle)style).HotGlyphColor);
                properties["hotSelectedBackColor"] = StiReportEdit.GetStringFromColor(((StiDialogStyle)style).HotSelectedBackColor);
                properties["hotSelectedForeColor"] = StiReportEdit.GetStringFromColor(((StiDialogStyle)style).HotSelectedForeColor);
                properties["hotSelectedGlyphColor"] = StiReportEdit.GetStringFromColor(((StiDialogStyle)style).HotSelectedGlyphColor);
                properties["selectedBackColor"] = StiReportEdit.GetStringFromColor(((StiDialogStyle)style).SelectedBackColor);
                properties["selectedForeColor"] = StiReportEdit.GetStringFromColor(((StiDialogStyle)style).SelectedForeColor);
                properties["selectedGlyphColor"] = StiReportEdit.GetStringFromColor(((StiDialogStyle)style).SelectedGlyphColor);
                properties["separatorColor"] = StiReportEdit.GetStringFromColor(((StiDialogStyle)style).SeparatorColor);
            }
            else if (style is StiProgressStyle)
            {
                var progressStyle = style as StiProgressStyle;
                properties["bandColor"] = StiReportEdit.GetStringFromColor(progressStyle.BandColor);
                properties["trackColor"] = StiReportEdit.GetStringFromColor(progressStyle.TrackColor);
                properties["seriesColors"] = StiReportEdit.GetColorsCollectionProperty(progressStyle.SeriesColors);
            }
            else if (style is StiIndicatorStyle)
            {
                var indicatorStyle = style as StiIndicatorStyle;
                properties["glyphColor"] = StiReportEdit.GetStringFromColor(indicatorStyle.GlyphColor);
                properties["hotBackColor"] = StiReportEdit.GetStringFromColor(indicatorStyle.HotBackColor);
                properties["positiveColor"] = StiReportEdit.GetStringFromColor(indicatorStyle.PositiveColor);
                properties["negativeColor"] = StiReportEdit.GetStringFromColor(indicatorStyle.NegativeColor);
                properties["toolTipBrush"] = StiReportEdit.BrushToStr(indicatorStyle.ToolTipBrush);
                properties["toolTipBorder"] = StiReportEdit.SimpleBorderToStr(indicatorStyle.ToolTipBorder);
                properties["toolTipTextBrush"] = StiReportEdit.BrushToStr(indicatorStyle.ToolTipTextBrush);
                properties["toolTipCornerRadius"] = StiReportEdit.CornerRadiusToStr(indicatorStyle.ToolTipCornerRadius);
            }
            else if (style is StiStyle) {                
                properties["image"] = ((StiStyle)style).Image != null ? StiReportEdit.ImageToBase64(((StiStyle)style).Image) : string.Empty;
                properties["negativeTextBrush"] = StiReportEdit.BrushToStr(((StiStyle)style).NegativeTextBrush);
                properties["textFormat"] = StiTextFormatHelper.GetTextFormatItem(((StiStyle)style).TextFormat);
                properties["lineSpacing"] = StiReportEdit.DoubleToStr(((StiStyle)style).LineSpacing);
            }

            return properties;
        }

        public static Hashtable StyleItem(StiBaseStyle style)
        {
            Hashtable styleItem = new Hashtable();
            styleItem["type"] = style.GetType().Name;
            styleItem["properties"] = GetStyleProperties(style);

            return styleItem;
        }
        #endregion

        #region Helper Methods
        public static void SetConditionTypeProperty(StiStyleCondition styleCondition, string propertyValue)
        {
            int value = 0;
            if (propertyValue.IndexOf(" ComponentType,") >= 0) value += 1;
            if (propertyValue.IndexOf(" Placement,") >= 0) value += 2;
            if (propertyValue.IndexOf(" PlacementNestedLevel,") >= 0) value += 4;
            if (propertyValue.IndexOf(" ComponentName,") >= 0) value += 8;
            if (propertyValue.IndexOf(" Location,") >= 0) value += 16;

            styleCondition.Type = (StiStyleConditionType)value;
        }

        public static void SetLocationProperty(StiStyleCondition styleCondition, string propertyValue)
        {
            int value = 0;
            if (propertyValue.IndexOf(" TopLeft,") >= 0) value += 1;
            if (propertyValue.IndexOf(" TopCenter,") >= 0) value += 2;
            if (propertyValue.IndexOf(" TopRight,") >= 0) value += 4;
            if (propertyValue.IndexOf(" MiddleLeft,") >= 0) value += 8;
            if (propertyValue.IndexOf(" MiddleCenter,") >= 0) value += 16;
            if (propertyValue.IndexOf(" MiddleRight,") >= 0) value += 32;
            if (propertyValue.IndexOf(" BottomLeft,") >= 0) value += 64;
            if (propertyValue.IndexOf(" BottomCenter,") >= 0) value += 128;
            if (propertyValue.IndexOf(" BottomRight,") >= 0) value += 256;
            if (propertyValue.IndexOf(" Left,") >= 0) value += 512;
            if (propertyValue.IndexOf(" Right,") >= 0) value += 1024;
            if (propertyValue.IndexOf(" Top,") >= 0) value += 2048;
            if (propertyValue.IndexOf(" Bottom,") >= 0) value += 4096;
            if (propertyValue.IndexOf(" CenterHorizontal,") >= 0) value += 8192;
            if (propertyValue.IndexOf(" CenterVertical,") >= 0) value += 16384;

            styleCondition.Location = (StiStyleLocation)value;
        }

        public static void SetComponentTypeProperty(StiStyleCondition styleCondition, string propertyValue)
        {
            int value = 0;
            if (propertyValue.IndexOf(" Text,") >= 0) value += 1;
            if (propertyValue.IndexOf(" Primitive,") >= 0) value += 2;
            if (propertyValue.IndexOf(" Image,") >= 0) value += 4;
            if (propertyValue.IndexOf(" CrossTab,") >= 0) value += 8;
            if (propertyValue.IndexOf(" Chart,") >= 0) value += 16;
            if (propertyValue.IndexOf(" CheckBox,") >= 0) value += 32;

            styleCondition.ComponentType = (StiStyleComponentType)value;
        }

        public static void SetPlacementProperty(StiStyleCondition styleCondition, string propertyValue)
        {
            int value = 0;
            if (propertyValue.IndexOf(" ReportTitle,") >= 0) value += 1;
            if (propertyValue.IndexOf(" ReportSummary,") >= 0) value += 2;
            if (propertyValue.IndexOf(" PageHeader,") >= 0) value += 4;
            if (propertyValue.IndexOf(" PageFooter,") >= 0) value += 8;
            if (propertyValue.IndexOf(" GroupHeader,") >= 0) value += 16;
            if (propertyValue.IndexOf(" GroupFooter,") >= 0) value += 32;
            if (propertyValue.IndexOf(" Header,") >= 0) value += 64;
            if (propertyValue.IndexOf(" Footer,") >= 0) value += 128;
            if (propertyValue.IndexOf(" ColumnHeader,") >= 0) value += 256;
            if (propertyValue.IndexOf(" ColumnFooter,") >= 0) value += 512;
            if (propertyValue.IndexOf(" Data,") >= 0) value += 1024;
            if (propertyValue.IndexOf(" DataEvenStyle,") >= 0)
            {
                styleCondition.Placement = (StiStyleComponentPlacement)2048;
                return;
            }
            if (propertyValue.IndexOf(" DataOddStyle,") >= 0)
            {
                styleCondition.Placement = (StiStyleComponentPlacement)4096;
                return;
            }
            if (propertyValue.IndexOf(" Table,") >= 0) value += 8192;
            if (propertyValue.IndexOf(" Hierarchical,") >= 0) value += 16384;
            if (propertyValue.IndexOf(" Child,") >= 0) value += 32768;
            if (propertyValue.IndexOf(" Empty,") >= 0) value += 65536;
            if (propertyValue.IndexOf(" Overlay,") >= 0) value += 131072;
            if (propertyValue.IndexOf(" Panel,") >= 0) value += 262144;
            if (propertyValue.IndexOf(" Page,") >= 0) value += 524288;
            if (propertyValue.IndexOf(" Empty,") >= 0) value += 65536;

            if (propertyValue == " AllExeptStyles,") styleCondition.Placement = StiStyleComponentPlacement.AllExeptStyles;
            else styleCondition.Placement = (StiStyleComponentPlacement)value;
        }

        public static void SetStyleConditionsProprty(StiBaseStyle style, ArrayList conditions)
        {
            style.Conditions.Clear();

            foreach (Hashtable conditionObject in conditions)
            {
                StiStyleCondition styleCondition = new StiStyleCondition();
                style.Conditions.Add(styleCondition);
                SetConditionTypeProperty(styleCondition, conditionObject["type"] as string);
                SetPlacementProperty(styleCondition, conditionObject["placement"] as string);
                styleCondition.OperationPlacement = (StiStyleConditionOperation)Enum.Parse(typeof(StiStyleConditionOperation), conditionObject["operationPlacement"] as string);
                styleCondition.PlacementNestedLevel = StiReportEdit.StrToInt(conditionObject["placementNestedLevel"] as string);
                styleCondition.OperationPlacementNestedLevel = (StiStyleConditionOperation)Enum.Parse(typeof(StiStyleConditionOperation), conditionObject["operationPlacementNestedLevel"] as string);
                SetComponentTypeProperty(styleCondition, conditionObject["componentType"] as string);
                styleCondition.OperationComponentType = (StiStyleConditionOperation)Enum.Parse(typeof(StiStyleConditionOperation), conditionObject["operationComponentType"] as string);
                SetLocationProperty(styleCondition, conditionObject["location"] as string);
                styleCondition.OperationLocation = (StiStyleConditionOperation)Enum.Parse(typeof(StiStyleConditionOperation), conditionObject["operationLocation"] as string);
                styleCondition.ComponentName = conditionObject["componentName"] as string;
                styleCondition.OperationComponentName = (StiStyleConditionOperation)Enum.Parse(typeof(StiStyleConditionOperation), conditionObject["operationComponentName"] as string);
            }
        }

        public static ArrayList GetStyleConditionsProprty(StiStyleConditionsCollection conditions)
        {
            ArrayList result = new ArrayList();

            foreach (StiStyleCondition condition in conditions)
            {
                Hashtable conditionObject = new Hashtable();
                result.Add(conditionObject);

                conditionObject["type"] = " " + condition.Type.ToString() + ",";
                conditionObject["placement"] = " " + condition.Placement.ToString() + ",";
                conditionObject["operationPlacement"] = condition.OperationPlacement.ToString();
                conditionObject["placementNestedLevel"] = condition.PlacementNestedLevel.ToString();
                conditionObject["operationPlacementNestedLevel"] = condition.OperationPlacementNestedLevel.ToString();
                conditionObject["componentType"] = " " + condition.ComponentType.ToString() + ",";
                conditionObject["operationComponentType"] = condition.OperationComponentType.ToString();
                conditionObject["location"] = " " + condition.Location.ToString() + ",";
                conditionObject["operationLocation"] = condition.OperationLocation.ToString();
                conditionObject["componentName"] = condition.ComponentName;
                conditionObject["operationComponentName"] = condition.OperationComponentName.ToString();
            }

            return result;
        }

        public static ArrayList GetStyles(StiReport report)
        {
            ArrayList items = new ArrayList();
            foreach (StiBaseStyle style in report.Styles)
            {
                items.Add(StyleItem(style));
            }

            return items;
        }

        public static void GenerateNewName(StiStylesCollection styles, StiBaseStyle newStyle)
        {
            bool fail = true;
            int index = 1;

            while (fail)
            {
                fail = false;
                string name = StiLocalization.Get("FormStyleDesigner", "Style") + index.ToString();

                foreach (StiBaseStyle st in styles)
                {
                    if (st.Name == name)
                    {
                        fail = true;
                        break;
                    }
                }
                index++;
                newStyle.Name = name;
            }
        }

        public static void ApplyStyleProperties(StiBaseStyle style, Hashtable properties)
        {
            if (properties == null) return;

            foreach (DictionaryEntry property in properties)
            {
                StiReportEdit.SetPropertyValue(style.Report, StiReportEdit.UpperFirstChar((string)property.Key), style, property.Value);
            }
        }

        public static void WriteStylesToReport(StiReport report, ArrayList stylesCollection)
        {
            report.Styles.Clear();

            foreach (Hashtable styleObject in stylesCollection)
            {
                Assembly assembly = typeof(StiReport).Assembly;
                StiBaseStyle style = assembly.CreateInstance("Stimulsoft.Report." + (string)styleObject["type"]) as StiBaseStyle;

                if (style != null)
                {
                    var properties = styleObject["properties"] as Hashtable;

                    if (properties != null && !String.IsNullOrEmpty(properties["name"] as string))
                        style.Name = properties["name"] as string;
                    else
                        GenerateNewName(report.Styles, style);

                    report.Styles.Add(style);
                    ApplyStyleProperties(style, properties);
                    
                    if (properties != null && properties["oldName"] != null) {
                        ChangeStyleNameInReport(report, properties["oldName"] as string, style);
                    }
                }
            }
        }

        private static void ChangeStyleNameInReport(StiReport report, string oldStyleName, StiBaseStyle newStyle)
        {
            foreach (StiComponent component in report.GetComponents())
            {
                if (component is StiChart && ((StiChart)component).CustomStyleName == oldStyleName)
                {
                    var chart = component as StiChart;
                    chart.Style = new StiCustomStyle(newStyle.Name);
                    chart.CustomStyleName = newStyle.Name;
                }
                else if (component is StiGauge && ((StiGauge)component).CustomStyleName == oldStyleName)
                {
                    var gauge = component as StiGauge;
                    gauge.Style = new StiCustomGaugeStyle(newStyle as StiGaugeStyle);
                    gauge.CustomStyleName = newStyle.Name;
                }
                else if (component is StiCrossTab && ((StiCrossTab)component).CrossTabStyle == oldStyleName)
                {
                    ((StiCrossTab)component).CrossTabStyle = newStyle.Name;
                }
                else if (component is IStiDashboardElementStyle && (component as IStiDashboardElementStyle).CustomStyleName == oldStyleName)
                {
                    (component as IStiDashboardElementStyle).CustomStyleName = newStyle.Name;
                }
                else if (component.ComponentStyle == oldStyleName)
                {
                    component.ComponentStyle = newStyle.Name;
                }
            }
        }
        #endregion

        #region Callback Methods
        public static void UpdateStyles(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            if (param["reportFile"] != null) report.ReportFile = (string)param["reportFile"];
            if (param["stylesCollection"] != null) WriteStylesToReport(report, (ArrayList)param["stylesCollection"]);
            if (param["collectionName"] != null) report.ApplyStyleCollection((string)param["collectionName"]);
            report.ApplyStyles();
            report.Info.Zoom = StiReportEdit.StrToDouble((string)param["zoom"]);
            callbackResult["reportObject"] = StiReportEdit.WriteReportToJsObject(report);
            if (param["selectedObjectName"] != null) callbackResult["selectedObjectName"] = (string)param["selectedObjectName"];
            callbackResult["reportGuid"] = (string)param["reportGuid"];
        }

        public static void AddStyle(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            Assembly assembly = typeof(StiReport).Assembly;
            string styleType = param["type"] as string;
            StiBaseStyle newStyle = assembly.CreateInstance("Stimulsoft.Report." + styleType) as StiBaseStyle;

            if (newStyle != null)
            {
                callbackResult["styleObject"] = StyleItem(newStyle);
            }
        }

        private static Color[] CloneColors(Color[] colors)
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

        public static void CreateStyleCollection(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            Hashtable properties = (Hashtable)param["styleCollectionProperties"];            
            Color baseColor = StiReportEdit.StrToColor((string)properties["color"]);

            var creator = new StiStylesCreator(report);
            creator.ShowBorders = (bool)properties["borders"];
            creator.MaxNestedLevel = StiReportEdit.StrToInt((string)properties["nestedLevel"]);

            creator.NestedFactor = (StiNestedFactor)Enum.Parse(typeof(StiNestedFactor), (string)properties["nestedFactor"]);
            creator.ShowReportTitles = (bool)properties["reportTitle"];
            creator.ShowReportSummaries = (bool)properties["reportSummary"];
            creator.ShowPageHeaders = (bool)properties["pageHeader"];
            creator.ShowPageFooters = (bool)properties["pageFooter"];
            creator.ShowGroupHeaders = (bool)properties["groupHeader"];
            creator.ShowGroupFooters = (bool)properties["groupFooter"];
            creator.ShowHeaders = (bool)properties["header"];
            creator.ShowDatas = (bool)properties["data"];
            creator.ShowFooters = (bool)properties["footer"];

            var styles = creator.CreateStyles((string)properties["collectionName"], baseColor);
            ArrayList newStylesCollection = new ArrayList();

            foreach (var style in styles)
            {
                newStylesCollection.Add(StyleItem(style));
            }

            callbackResult["removeExistingStyles"] = properties["removeExistingStyles"];
            callbackResult["collectionName"] = properties["collectionName"];
            callbackResult["newStylesCollection"] = newStylesCollection;
        }

        internal static StiStylesCollection CreateStylesCollectionFromBaseColor(Color baseColor, string name)
        {
            using (var report = new StiReport())
            {
                var creator = new StiStylesCreator(report)
                {
                    MaxNestedLevel = 1,
                    ShowBorders = true
                };

                var styles = creator.CreateStyles(name, baseColor);
                var stylesCollection = new StiStylesCollection();

                foreach (StiBaseStyle style in styles)
                    stylesCollection.Add(style);

                return stylesCollection;
            }
        }

        public static void CreateStylesFromComponents(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            var stylesList = new List<StiBaseStyle>();
            var stylesListForJS = new ArrayList();
            var componentsNames = param["componentsNames"] as ArrayList;

            foreach (string componentName in componentsNames)
            {
                var comp = report.Pages.GetComponentByName(componentName);
                if (comp != null)
                {
                    StiBaseStyle style = null;

                    #region DBS
                    if (comp is IStiChartElement)
                    {
                        var element = (IStiChartElement)comp;
                        IStiChartStyle chartStyle = null;
                        if (element.Style == StiElementStyleIdent.Custom && !string.IsNullOrEmpty(element.CustomStyleName))
                        {
                            var customStyle = element.Report.Styles.ToList().FirstOrDefault(x => x.Name == element.CustomStyleName) as StiChartStyle;
                            if (customStyle != null)
                                chartStyle = new StiCustomStyle(element.CustomStyleName);
                        }
                        else
                        {
                            var styleIdent = element.Style == StiElementStyleIdent.Auto
                                ? ((IStiDashboard)element.Page).Style
                                : element.Style;

                            chartStyle = StiDashboardStyleHelper.GetChartStyle(styleIdent);
                        }

                        if (chartStyle == null)
                            chartStyle = new StiStyle29();

                        style = StiDashboardStyleHelper.GetCopyChartStyle(chartStyle, element);
                    }
                    else if (comp is IStiTableElement)
                    {
                        style = StiDashboardStyleHelper.GetCopyTableStyle((IStiTableElement)comp, StiDashboardStyleHelper.GetTableStyle((IStiTableElement)comp));
                    }
                    else if (comp is IStiGaugeElement)
                    {
                        style = StiDashboardStyleHelper.ConvertToReportGaugeStyle((IStiGaugeElement)comp);
                    }
                    else if (comp is IStiPivotTableElement)
                    {
                        style = StiDashboardStyleHelper.ConvertToReportPivotTableStyle((IStiPivotTableElement)comp);
                    }
                    else if (comp is IStiIndicatorElement)
                    {
                        style = StiDashboardStyleHelper.ConvertToReportIndicatorStyle((IStiIndicatorElement)comp);
                    }
                    else if (comp is IStiProgressElement)
                    {
                        style = StiDashboardStyleHelper.ConvertToReportProgressStyle((IStiProgressElement)comp);
                    }
                    else if (comp is IStiRegionMapElement)
                    {
                        style = StiDashboardStyleHelper.ConvertToReportRegionMapStyle((IStiRegionMapElement)comp);
                    }
                    else if (comp is IStiControlElement)
                    {
                        style = StiDashboardStyleHelper.ConvertToReportControlStyle((IStiControlElement)comp);
                    }
                    #endregion
                    else if (comp is StiChart)
                    {
                        style = new StiChartStyle();
                        style.GetStyleFromComponent(comp, StiStyleElements.All);
                    }
                    else if (comp is StiCrossTab)
                    {
                        style = new StiCrossTabStyle();
                        style.GetStyleFromComponent(comp, StiStyleElements.All);
                    }
                    else if (comp is StiMap)
                    {
                        style = new StiMapStyle();
                        style.GetStyleFromComponent(comp, StiStyleElements.All);
                    }
                    else if (comp is StiGauge)
                    {
                        style = new StiGaugeStyle();
                        style.GetStyleFromComponent(comp, StiStyleElements.All);
                    }
                    else if (comp is StiTable)
                    {
                        style = new StiTableStyle();
                        style.GetStyleFromComponent(comp, StiStyleElements.All);
                    }
                    else
                    {
                        style = new StiStyle();
                        style.GetStyleFromComponent(comp, StiStyleElements.All);
                        ((StiStyle)style).AllowUseHorAlignment = false;
                        ((StiStyle)style).AllowUseVertAlignment = false;
                    }
                    style.Name = comp.Name + "Style";
                    style.Description = string.Format("Style based on formating of {0} component", comp.Name);

                    bool find = false;
                    foreach (StiBaseStyle baseStyle in stylesList)
                    {
                        if (baseStyle.Equals(style, false, false))
                        {
                            find = true;
                            break;
                        }
                    }
                    if (!find)
                    {
                        stylesList.Add(style);
                        stylesListForJS.Add(StyleItem(style));
                    }   
                }
            }

            callbackResult["styles"] = stylesListForJS;
        }

        public static void OpenStyle(StiRequestParams requestParams, StiReport report, Hashtable callbackResult)
        {
            using (var stream = new MemoryStream(requestParams.Data))
            {
                StiStylesCollection styles = new StiStylesCollection();
                styles.Load(stream);

                ArrayList stylesCollectionForJS = new ArrayList();
                foreach (StiBaseStyle style in styles)
                {
                    if (style != null) stylesCollectionForJS.Add(StyleItem(style));
                }
                callbackResult["stylesCollection"] = stylesCollectionForJS;
            }
        }

        public static void GetStylesContentByType(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            var styleType = param["styleType"] as string;

            switch (styleType)
            {
                case "StiChartStyle":
                    StiChartHelper.GetStylesContent(report, param, callbackResult, true, false);
                    break;

                case "StiGaugeStyle":
                    StiGaugeHelper.GetStylesContent(report, param, callbackResult, true, false);
                    break;

                case "StiMapStyle":
                    StiMapHelper.GetStylesContent(report, param, callbackResult, false);
                    break;

                case "StiCrossTabStyle":
                    callbackResult["stylesContent"] = StiCrossTabHelper.GetColorStyles();
                    break;

                case "StiTableStyle":
                    callbackResult["stylesContent"] = StiTableHelper.GetTableStyles(report, false);
                    break;

                case "StiCardsStyle":
                    callbackResult["stylesContent"] = StiCardsElementHelper.GetStylesContent(report, param, false);
                    break;

                case "StiIndicatorStyle":
                    callbackResult["stylesContent"] = StiIndicatorElementHelper.GetStylesContent(report, param, false);
                    break;

                case "StiProgressStyle":
                    callbackResult["stylesContent"] = StiProgressElementHelper.GetStylesContent(report, param, false);
                    break;
            }
        }

        private static StiElementStyleIdent ParseElementStyleIdent(string styleIdent)
        {
            return (StiElementStyleIdent)Enum.Parse(typeof(StiElementStyleIdent), styleIdent);
        }

        private static StiBaseStyle GetBaseStyle(StiReport report, string styleType, Hashtable styleObject)
        {
            StiBaseStyle newStyle = null;
            var assembly = typeof(StiReport).Assembly;

            switch (styleType)
            {
                case "StiChartStyle":
                    newStyle = assembly.CreateInstance("Stimulsoft.Report.Chart." + styleObject["type"]) as Stimulsoft.Report.Chart.StiChartStyle;
                    break;

                case "StiMapStyle":
                    var mapStyleIdent = (StiMapStyleIdent)Enum.Parse(typeof(StiMapStyleIdent), styleObject["name"] as string);
                    newStyle = StiMap.GetMapStyle(mapStyleIdent);
                    break;

                case "StiGaugeStyle":
                    newStyle = assembly.CreateInstance("Stimulsoft.Report.Gauge." + styleObject["type"]) as StiGaugeStyleXF;
                    break;

                case "StiCrossTabStyle":
                    var styleColor = StiOptions.Designer.CrossTab.StyleColors[Convert.ToInt32(styleObject["crossTabStyleIndex"])];
                    newStyle = new StiCrossTabStyle() { Color = styleColor };
                    break;

                case "StiTableStyle":
                    var tableStyleIdent = (StiTableStyleIdent)Enum.Parse(typeof(StiTableStyleIdent), styleObject["styleId"] as string);
                    newStyle = StiOptions.Services.TableStyles.FirstOrDefault(s => s.StyleId == tableStyleIdent);
                    break;

                case "StiCardsStyle":
                    var cardsStyleIdent = ParseElementStyleIdent(styleObject["ident"] as string);
                    newStyle = StiOptions.Services.Dashboards.CardsStyles.FirstOrDefault(s => s.Ident == cardsStyleIdent);
                    break;

                case "StiIndicatorStyle":
                    var indicatorStyleIdent = ParseElementStyleIdent(styleObject["ident"] as string);
                    newStyle = StiOptions.Services.Dashboards.IndicatorStyles.FirstOrDefault(s => s.Ident == indicatorStyleIdent);
                    break;

                case "StiProgressStyle":
                    var progressStyleIdent = ParseElementStyleIdent(styleObject["ident"] as string);
                    newStyle = StiOptions.Services.Dashboards.ProgressStyles.FirstOrDefault(s => s.Ident == progressStyleIdent);
                    break;
            }

            return newStyle;
        }

        public static void CreateStyleBasedAnotherStyle(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            var styleType = param["styleType"] as string;
            var styleObject = param["styleObject"] as Hashtable;

            var baseStyle = GetBaseStyle(report, styleType, styleObject);
            if (baseStyle != null)
            {
                var newStyle = GenerateStyle(styleType, baseStyle);
                callbackResult["styleObject"] = StyleItem(newStyle);
            }
        }

        private static StiBaseStyle GenerateStyle(string styleType, StiBaseStyle baseStyle)
        {
            if (baseStyle == null) return null;
            
            switch (styleType)
            {
                #region Cards
                case "StiCardsStyle":
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
                case "StiChartStyle":
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
                case "StiCrossTabStyle":
                    {
                        return (StiCrossTabStyle)((StiCrossTabStyle)baseStyle).Clone();
                    }
                #endregion

                #region Table
                case "StiTableStyle":
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
                case "StiGaugeStyle":
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
                case "StiMapStyle":
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
                case "StiIndicatorStyle":
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
                case "StiProgressStyle":
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
        #endregion
    }
}