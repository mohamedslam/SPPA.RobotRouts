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
using System.Xml;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Chart;
using Stimulsoft.Base.Drawing;
using System.IO;
using System.Linq;
using Stimulsoft.Base;
using Stimulsoft.Report.Export;
using System.ComponentModel;
using System.Drawing;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Data.Functions;
using Stimulsoft.Report.Helpers;
using Stimulsoft.Report.Web.Helpers.Dashboards;

namespace Stimulsoft.Report.Web
{
    internal class StiChartHelper
    {
        #region Advanced chart form
        #region Properties
        public static Hashtable GetChartProperties(StiChart chart)
        {
            Hashtable properties = new Hashtable();
            properties["name"] = chart.Name;
            properties["series"] = GetSeriesArray(chart);
            properties["properties"] = GetMainProperties(chart);
            properties["editorType"] = chart.EditorType.ToString();
            properties["chartImage"] = GetChartSampleImage(chart, 400, 400, 1f);
            properties["typesCollection"] = GetTypesCollection(chart);
            properties["area"] = GetArea(chart);
            properties["style"] = GetStyle(chart);
            properties["labels"] = GetLabels(chart.SeriesLabels);
            properties["constantLines"] = GetConstantLines(chart);
            properties["strips"] = GetStrips(chart);
            properties["conditions"] = GetConditions(chart.SeriesLabelsConditions);
            properties["svgContent"] = StiReportEdit.GetSvgContent(chart);

            return properties;
        }

        public static ArrayList GetSeriesArray(StiChart chart)
        {
            ArrayList seriesArray = new ArrayList();
            foreach (StiSeries series in chart.Series)
            {
                seriesArray.Add(GetSeries(series));
            }
            return seriesArray;
        }

        public static Hashtable GetSeries(StiSeries series)
        {
            Hashtable seriesObject = new Hashtable();
            seriesObject["name"] = series.Title + " [" + series.ToString() + "]";
            seriesObject["type"] = series.GetType().Name;
            seriesObject["properties"] = GetSeriesProperties(series);
            seriesObject["labels"] = GetLabels(series.SeriesLabels);
            seriesObject["conditions"] = GetConditions(series.Conditions);
            seriesObject["trendLines"] = GetTrendLines(series.TrendLines);
            seriesObject["filters"] = GetFilters(series.Filters);
            seriesObject["filterMode"] = series.FilterMode;


            return seriesObject;
        }

        public static ArrayList GetConditions(StiChartConditionsCollection conditions)
        {
            ArrayList conditionsArray = new ArrayList();

            foreach (StiChartCondition condition in conditions)
            {
                Hashtable propertiesCondition = new Hashtable();
                propertiesCondition["FieldIs"] = condition.Item;
                propertiesCondition["Value"] = StiEncodingHelper.Encode(condition.Value);
                propertiesCondition["Color"] = StiReportEdit.GetStringFromColor(condition.Color);
                propertiesCondition["DataType"] = condition.DataType;
                propertiesCondition["Condition"] = condition.Condition;

                conditionsArray.Add(propertiesCondition);
            }

            return conditionsArray;
        }

        public static ArrayList GetTrendLines(StiTrendLinesCollection trendLines)
        {
            ArrayList tLinesArray = new ArrayList();

            foreach (StiTrendLine trendLine in trendLines)
            {
                Hashtable properties = new Hashtable();
                properties["Type"] = trendLine.GetType().Name.Replace("StiTrendLine", "");
                properties["LineColor"] = StiReportEdit.GetStringFromColor(trendLine.LineColor);
                properties["LineStyle"] = trendLine.LineStyle.ToString();
                properties["LineWidth"] = StiReportEdit.DoubleToStr(trendLine.LineWidth);
                properties["Text"] = StiEncodingHelper.Encode(trendLine.Text);
                properties["Position"] = trendLine.Position.ToString();
                properties["Font"] = StiReportEdit.FontToStr(trendLine.Font);
                properties["AllowApplyStyle"] = trendLine.AllowApplyStyle;

                tLinesArray.Add(properties);
            }

            return tLinesArray;
        }

        public static ArrayList GetFilters(StiChartFiltersCollection filters)
        {
            ArrayList filtersArray = new ArrayList();

            foreach (StiChartFilter filter in filters)
            {
                Hashtable propertiesFilter = new Hashtable();
                propertiesFilter["FieldIs"] = filter.Item;
                propertiesFilter["DataType"] = filter.DataType;
                propertiesFilter["Value"] = StiEncodingHelper.Encode(filter.Value);
                propertiesFilter["Condition"] = filter.Condition;

                filtersArray.Add(propertiesFilter);
            }

            return filtersArray;
        }

        public static ArrayList GetTypesCollection(StiChart chart)
        {
            ArrayList typesCollection = new ArrayList();
            var types = (chart.Area != null)
                ? chart.Area.GetSeriesTypes()
                : new Type[0];

            foreach (Type type in types)
            {
                typesCollection.Add(type.Name);
            }
            return typesCollection;
        }

        public static Hashtable GetArea(StiChart chart)
        {
            Hashtable area = new Hashtable();
            if (chart.Area != null)
            {
                area["type"] = chart.Area.GetType().Name;
                area["properties"] = GetAreaProperties(chart.Area as StiArea);
            }

            return area;
        }

        public static Hashtable GetStyle(StiChart chart)
        {
            Hashtable style = new Hashtable();
            style["type"] = chart.Style.GetType().Name;
            style["name"] = (chart.Style is StiCustomStyle && ((StiCustomStyleCoreXF)chart.Style.Core).ReportStyle != null)
                ? ((StiCustomStyleCoreXF)chart.Style.Core).ReportStyle.Name : "";

            return style;
        }

        public static Hashtable GetMainProperties(StiChart chart)
        {
            Hashtable properties = new Hashtable();
            #region Common
            Hashtable propertiesCommon = new Hashtable();
            string[] propNamesCommon = { "AllowApplyStyle", "ProcessAtEnd", "Rotation", "HorSpacing", "VertSpacing", "DataSource", "BusinessObject", "DataRelation",
                "MasterComponent", "CountData", "Filters", "Sort"};
            foreach (string propName in propNamesCommon)
            {

                var value = StiReportEdit.GetPropertyValue(propName, chart, true);
                if (value != null) { propertiesCommon[propName] = value; }
            }

            properties["Common"] = propertiesCommon;
            #endregion

            #region Legend
            Hashtable propertiesLegend = new Hashtable();
            string[] propNamesLegend = { "AllowApplyStyle", "BorderColor", "Brush", "Columns", "ColumnWidth", "Direction", "Font", "HideSeriesWithEmptyTitle", "HorAlignment",
                "HorSpacing", "LabelsColor", "MarkerAlignment", "MarkerBorder", "MarkerSize", "MarkerVisible", "ShowShadow", "Title", "TitleColor", "TitleFont",
                "VertAlignment", "VertSpacing", "Visible" };
            foreach (string propName in propNamesLegend)
            {
                var value = StiReportEdit.GetPropertyValue(propName, chart.Legend, true);
                if (value != null) { propertiesLegend[propName] = value; }
            }

            properties["Legend"] = propertiesLegend;
            #endregion

            #region Title
            Hashtable propertiesTitle = new Hashtable();
            string[] propNamesTitle = { "Alignment", "AllowApplyStyle", "Antialiasing", "Brush", "Dock", "Font", "Spacing", "Text", "Visible" };
            foreach (string propName in propNamesTitle)
            {
                var value = StiReportEdit.GetPropertyValue(propName, chart.Title, true);
                if (value != null) { propertiesTitle[propName] = value; }
            }

            properties["Title"] = propertiesTitle;
            #endregion

            #region Table
            Hashtable propertiesTable = new Hashtable();
            string[] propNamesTable = { "AllowApplyStyle", "Font", "GridLineColor", "GridLinesHor", "GridLinesVert", "GridOutline", "MarkerVisible", "Visible",
                "Header.TextAfter", "Header.TextColor", "Header.WordWrap", "Header.Brush", "Header.Font", "Header.Format", "DataCells.TextColor",
                "DataCells.ShrinkFontToFit", "DataCells.ShrinkFontToFitMinimumSize", "DataCells.Font" };
            foreach (string propName in propNamesTable)
            {
                var value = StiReportEdit.GetPropertyValue(propName, chart.Table, true);
                if (value != null) { propertiesTable[propName] = value; }
            }

            properties["Table"] = propertiesTable;
            #endregion

            return properties;
        }

        public static ArrayList GetConstantLines(StiChart chart)
        {
            ArrayList constantLinesArray = new ArrayList();
            foreach (StiConstantLines constantLine in chart.ConstantLines)
            {
                Hashtable constantLineObj = new Hashtable();
                constantLineObj["name"] = constantLine.Text;
                constantLineObj["properties"] = GetConstantLineProperties(constantLine);
                constantLinesArray.Add(constantLineObj);
            }
            return constantLinesArray;
        }

        public static Hashtable GetConstantLineProperties(StiConstantLines constantLine)
        {
            Hashtable properties = new Hashtable();

            string[] propNames = { "AxisValue", "LineColor", "LineStyle", "LineWidth", "Orientation", "ShowBehind", "Visible", "AllowApplyStyle", "Antialiasing",
                "Font", "Position", "Text", "TitleVisible" };
            foreach (string propName in propNames)
            {
                var value = StiReportEdit.GetPropertyValue(propName, constantLine, true);
                if (value != null) { properties[propName] = value; }
            }

            return properties;
        }

        public static ArrayList GetStrips(StiChart chart)
        {
            ArrayList stripsArray = new ArrayList();
            foreach (StiStrips strip in chart.Strips)
            {
                Hashtable stripObj = new Hashtable();
                stripObj["name"] = strip.Text;
                stripObj["properties"] = GetStripsProperties(strip);
                stripsArray.Add(stripObj);
            }
            return stripsArray;
        }

        public static Hashtable GetStripsProperties(StiStrips strip)
        {
            Hashtable properties = new Hashtable();

            string[] propNames = { "AllowApplyStyle", "MaxValue", "MinValue", "Orientation", "ShowBehind", "StripBrush", "Visible", "Antialiasing", "Font", "Text",
                "TitleColor", "TitleVisible" };
            foreach (string propName in propNames)
            {
                var value = StiReportEdit.GetPropertyValue(propName, strip, true);
                if (value != null) { properties[propName] = value; }
            }

            return properties;
        }

        public static Hashtable GetLabels(IStiSeriesLabels seriesLabels)
        {
            Hashtable labels = new Hashtable();
            if (seriesLabels != null)
            {
                labels["type"] = seriesLabels.GetType().Name;
                labels["serviceName"] = seriesLabels.ToString();
                labels["properties"] = GetLabelsProperties(seriesLabels as StiSeriesLabels);
            }
            return labels;
        }

        public static Hashtable GetSeriesProperties(StiSeries series)
        {
            Hashtable properties = new Hashtable();

            #region Common
            Hashtable propertiesCommon = new Hashtable();
            string[] propNamesCommon = { "ValueDataColumn", "Value", "ListOfValues", "ValueDataColumnEnd", "ValueEnd", "ListOfValuesEnd", "ValueDataColumnClose", "ValueClose", "ListOfValuesClose",
                "ValueDataColumnHigh", "ValueHigh", "ListOfValuesHigh","ValueDataColumnLow", "ValueLow", "ListOfValuesLow", "ValueDataColumnOpen", "ValueOpen", "ListOfValuesOpen",
                "WeightDataColumn", "Weight", "ListOfWeights", "ArgumentDataColumn", "Argument", "ListOfArguments", "Format", "SortBy", "SortDirection", "AutoSeriesKeyDataColumn",
                "AutoSeriesColorDataColumn", "AutoSeriesTitleDataColumn", "BorderColor", "BorderColorNegative", "BorderWidth", "BorderThickness", "Brush", "BrushNegative", "ShowShadow", "AllowApplyStyle",
                "AllowApplyBrushNegative", "AllowApplyColorNegative", "ShowInLegend", "ShowSeriesLabels", "ShowZeros", "Title", "Width", "YAxis", "XAxis", "LabelsOffset", "Lighting", "LineColor", "LineColorNegative",
                "LineStyle", "LineWidth", "ShowNulls", "PointAtCenter", "Tension", "TopmostLine", "AllowApplyBorderColor", "AllowApplyBrush", "Diameter", "CutPieList", "StartAngle", "Distance", "Icon",
                "AllowApplyLineColor", "Filters", "FilterMode", "Conditions", "TrendLines", "Options3D.Distance", "Options3D.Height", "Options3D.Lighting", "Options3D.Opacity", "CornerRadius", "Length",
                "ColumnShape", "Color"
            };

            foreach (string propName in propNamesCommon)
            {
                if (propName == "YAxis" && (series is IStiClusteredBarSeries || series is IStiStackedBarSeries || series is IStiFullStackedBarSeries))
                    continue;

                if ((propName == "Value" || propName == "ValueDataColumn" || propName == "ListOfValues") && series is IStiFinancialSeries)
                    continue;

                if (propName == "Filters")
                {
                    propertiesCommon["Filters"] = GetFilters(series.Filters);
                    continue;
                }

                if (propName == "Conditions")
                {
                    propertiesCommon["Conditions"] = GetConditions(series.Conditions);
                    continue;
                }

                if (propName == "TrendLines")
                {
                    propertiesCommon["TrendLines"] = GetTrendLines(series.TrendLines);
                    continue;
                }

                var value = StiReportEdit.GetPropertyValue(propName, series, true);
                if (value != null) { propertiesCommon[propName] = value; }
            }

            properties["Common"] = propertiesCommon;
            #endregion

            #region Marker && LineMarker
            string[] markerGroups = { "Marker", "LineMarker" };
            foreach (string markerGroup in markerGroups)
            {
                PropertyInfo piMarkerGroup = series.GetType().GetProperty(markerGroup);
                if (piMarkerGroup != null)
                {
                    Hashtable propertiesMarker = new Hashtable();
                    string[] propNamesMarker = { "Angle", "BorderColor", "Brush", "ShowInLegend", "Size", "Step", "Type", "Visible", "Icon" };
                    foreach (string propName in propNamesMarker)
                    {
                        var value = StiReportEdit.GetPropertyValue(propName, piMarkerGroup.GetValue(series, null), true);
                        if (value != null) { propertiesMarker[propName] = value; }
                    }

                    properties[markerGroup] = propertiesMarker;
                }
            }
            #endregion

            #region Interaction
            PropertyInfo piInteraction = series.GetType().GetProperty("Interaction");
            if (piInteraction != null)
            {
                Hashtable propertiesInteraction = new Hashtable();
                string[] propNamesInteraction = { "AllowSeries", "AllowSeriesElements", "DrillDownEnabled", "DrillDownPage", "DrillDownReport", "HyperlinkDataColumn",
                    "TagDataColumn", "ToolTipDataColumn", "Hyperlink", "Tag", "ToolTip", "ListOfHyperlinks", "ListOfTags", "ListOfToolTips" };
                foreach (string propName in propNamesInteraction)
                {
                    var value = StiReportEdit.GetPropertyValue(propName, piInteraction.GetValue(series, null), true);
                    if (value != null) { propertiesInteraction[propName] = value; }
                }

                properties["Interaction"] = propertiesInteraction;
            }
            #endregion

            #region TopN
            PropertyInfo piTopN = series.GetType().GetProperty("TopN");
            if (piTopN != null)
            {
                Hashtable propertiesTopN = new Hashtable();
                string[] propNamesTopN = { "Count", "Mode", "OthersText", "ShowOthers" };
                foreach (string propName in propNamesTopN)
                {
                    var value = StiReportEdit.GetPropertyValue(propName, piTopN.GetValue(series, null), true);
                    if (value != null) { propertiesTopN[propName] = value; }
                }

                properties["TopN"] = propertiesTopN;
            }
            #endregion

            return properties;
        }

        public static Hashtable GetAreaProperties(StiArea area)
        {
            Hashtable properties = new Hashtable();

            #region Common
            Hashtable propertiesCommon = new Hashtable();
            string[] propNamesCommon = { "Brush", "AllowApplyStyle", "BorderColor", "BorderThickness", "ColorEach", "RadarStyle", "ReverseHor", "ReverseVert", "ShowShadow", "SideBySide" };
            foreach (string propName in propNamesCommon)
            {
                var value = StiReportEdit.GetPropertyValue(propName, area, true);
                if (value != null) { propertiesCommon[propName] = value; }
            }

            properties["Common"] = propertiesCommon;
            #endregion

            #region XAxis && YAxis && XTopAxis && YRightAxis
            string[] axisTypes = { "XAxis", "YAxis", "XTopAxis", "YRightAxis" };
            foreach (string axisType in axisTypes)
            {
                PropertyInfo piAxis = area.GetType().GetProperty(axisType);
                if (piAxis != null)
                {
                    Hashtable propertiesAxis = new Hashtable();
                    string[] propAxisNames = { "AllowApplyStyle", "ArrowStyle", "DateTimeStep.Interpolation", "DateTimeStep.NumberOfValues", "DateTimeStep.Step",
                    "Interaction.RangeScrollEnabled", "Interaction.ShowScrollBar", "Labels.Brush", "Labels.AllowApplyStyle", "Labels.Angle", "Labels.Antialiasing", "Labels.Color",
                    "Labels.DrawBorder", "Labels.Font", "Labels.Format", "Labels.RotationLabels", "Labels.Placement", "Labels.Step", "Labels.TextAfter", "Labels.TextAlignment", "Labels.TextBefore",
                    "Labels.Width", "Labels.WordWrap", "LineColor", "LineStyle", "LineWidth", "LogarithmicScale", "Range.Auto", "Range.Minimum", "Range.Maximum",
                    "ShowEdgeValues", "ShowXAxis", "StartFromZero", "Ticks.Length", "Ticks.LengthUnderLabels", "Ticks.MinorCount", "Ticks.MinorLength", "Ticks.MinorVisible",
                    "Ticks.Step", "Ticks.Visible", "Title.Alignment", "Title.AllowApplyStyle", "Title.Antialiasing", "Title.Color", "Title.Direction", "Title.Font",
                    "Title.Text", "Visible" };
                    foreach (string propName in propAxisNames)
                    {
                        var value = StiReportEdit.GetPropertyValue(propName, piAxis.GetValue(area, null), true);
                        if (value != null) { propertiesAxis[propName] = value; }
                    }

                    properties[axisType] = propertiesAxis;
                }
            }
            #endregion

            #region GridLinesHor && GridLinesHorRight && GridLinesVert
            string[] gridLinesTypes = { "GridLinesHor", "GridLinesHorRight", "GridLinesVert" };
            foreach (string gridLinesType in gridLinesTypes)
            {
                PropertyInfo piGridLines = area.GetType().GetProperty(gridLinesType);
                if (piGridLines != null)
                {
                    Hashtable propertiesGridLines = new Hashtable();
                    string[] propGridLinesNames = { "AllowApplyStyle", "Color", "MinorColor", "MinorCount", "MinorStyle", "MinorVisible", "Style", "Visible" };
                    foreach (string propName in propGridLinesNames)
                    {
                        var value = StiReportEdit.GetPropertyValue(propName, piGridLines.GetValue(area, null), true);
                        if (value != null) { propertiesGridLines[propName] = value; }
                    }

                    properties[gridLinesType] = propertiesGridLines;
                }
            }
            #endregion

            #region InterlacingHor && InterlacingVert
            string[] interlacingTypes = { "InterlacingHor", "InterlacingVert" };
            foreach (string interlacingType in interlacingTypes)
            {
                PropertyInfo piInterlacing = area.GetType().GetProperty(interlacingType);
                if (piInterlacing != null)
                {
                    Hashtable propertiesInterlacing = new Hashtable();
                    string[] propInterlacingNames = { "AllowApplyStyle", "InterlacedBrush", "Visible" };
                    foreach (string propName in propInterlacingNames)
                    {
                        var value = StiReportEdit.GetPropertyValue(propName, piInterlacing.GetValue(area, null), true);
                        if (value != null) { propertiesInterlacing[propName] = value; }
                    }

                    properties[interlacingType] = propertiesInterlacing;
                }
            }
            #endregion

            return properties;
        }

        public static Hashtable GetLabelsProperties(StiSeriesLabels labels)
        {
            Hashtable properties = new Hashtable();

            #region Common
            Hashtable propertiesCommon = new Hashtable();
            string[] propNamesCommon = { "AllowApplyStyle", "Angle", "Antialiasing", "AutoRotate", "DrawBorder", "BorderColor", "Brush", "Font", "Format", "LabelColor", "LineColor",
                "LegendValueType", "LineLength", "MarkerAlignment", "MarkerSize", "MarkerVisible", "PreventIntersection", "ShowInPercent", "ShowNulls", "ShowZeros", "Step",
                "TextAfter", "TextBefore", "UseSeriesColor", "ValueType", "ValueTypeSeparator", "Visible", "Width", "WordWrap" };
            foreach (string propName in propNamesCommon)
            {
                var value = StiReportEdit.GetPropertyValue(propName, labels, true);
                if (value != null) { propertiesCommon[propName] = value; }
            }

            properties["Common"] = propertiesCommon;
            #endregion

            return properties;
        }
        #endregion

        #region Helper Methods
        internal static void SetConditionsValue(StiChartConditionsCollection conditions, ArrayList conditionValues)
        {
            conditions.Clear();
            foreach (Hashtable conditionValue in conditionValues)
            {
                StiChartCondition condition = new StiChartCondition();
                if (conditionValue["FieldIs"] != null) condition.Item = (StiFilterItem)Enum.Parse(typeof(StiFilterItem), (string)conditionValue["FieldIs"]);
                if (conditionValue["DataType"] != null) condition.DataType = (StiFilterDataType)Enum.Parse(typeof(StiFilterDataType), (string)conditionValue["DataType"]);
                if (conditionValue["Condition"] != null) condition.Condition = (StiFilterCondition)Enum.Parse(typeof(StiFilterCondition), (string)conditionValue["Condition"]);
                if (conditionValue["Color"] != null) condition.Color = StiReportEdit.StrToColor((string)conditionValue["Color"]);
                if (conditionValue["Value"] != null) condition.Value = StiEncodingHelper.DecodeString((string)conditionValue["Value"]);

                conditions.Add(condition);
            }
        }

        internal static void SetFiltersValue(StiChartFiltersCollection filters, ArrayList filterValues)
        {
            filters.Clear();
            foreach (Hashtable filterValue in filterValues)
            {
                StiChartFilter filter = new StiChartFilter();
                if (filterValue["FieldIs"] != null) filter.Item = (StiFilterItem)Enum.Parse(typeof(StiFilterItem), (string)filterValue["FieldIs"]);
                if (filterValue["DataType"] != null) filter.DataType = (StiFilterDataType)Enum.Parse(typeof(StiFilterDataType), (string)filterValue["DataType"]);
                if (filterValue["Condition"] != null) filter.Condition = (StiFilterCondition)Enum.Parse(typeof(StiFilterCondition), (string)filterValue["Condition"]);
                if (filterValue["Value"] != null) filter.Value = StiEncodingHelper.DecodeString((string)filterValue["Value"]);

                filters.Add(filter);
            }
        }

        internal static void SetTrendLinesValue(StiTrendLinesCollection trendLines, ArrayList trendLinesValues)
        {
            trendLines.Clear();
            foreach (Hashtable trendLinesValue in trendLinesValues)
            {
                StiTrendLine trendLine = null;
                switch (trendLinesValue["Type"] as string)
                {
                    case "None":
                        trendLine = new StiTrendLineNone();
                        break;

                    case "Linear":
                        trendLine = new StiTrendLineLinear();
                        break;

                    case "Exponential":
                        trendLine = new StiTrendLineExponential();
                        break;

                    case "Logarithmic":
                        trendLine = new StiTrendLineLogarithmic();
                        break;
                }

                trendLine.LineColor = StiReportEdit.StrToColor((string)trendLinesValue["LineColor"]);
                trendLine.LineStyle = (StiPenStyle)Enum.Parse(typeof(StiPenStyle), (string)trendLinesValue["LineStyle"]);
                trendLine.LineWidth = (float)StiReportEdit.StrToDouble((string)trendLinesValue["LineWidth"]);
                trendLine.Text = StiEncodingHelper.DecodeString((string)trendLinesValue["Text"]);
                trendLine.Position = (StiTrendLine.StiTextPosition)Enum.Parse(typeof(StiTrendLine.StiTextPosition), (string)trendLinesValue["Position"]);
                trendLine.Font = StiReportEdit.StrToFont((string)trendLinesValue["Font"]);
                trendLine.AllowApplyStyle = (bool)trendLinesValue["AllowApplyStyle"];

                trendLines.Add(trendLine);
            }
        }

        public static string GetChartSampleImage(StiChart chart, int width, int height, float zoom)
        {
            var svgData = new StiSvgData()
            {
                X = 0,
                Y = 0,
                Width = width,
                Height = height,
                Component = chart
            };

            var sb = new StringBuilder();

            using (var ms = new StringWriter(sb))
            {
                var writer = new XmlTextWriter(ms);

                writer.WriteStartElement("svg");
                writer.WriteAttributeString("version", "1.1");
                writer.WriteAttributeString("baseProfile", "full");

                writer.WriteAttributeString("xmlns", "http://www.w3.org/2000/svg");
                writer.WriteAttributeString("xmlns:xlink", "http://www.w3.org/1999/xlink");
                writer.WriteAttributeString("xmlns:ev", "http://www.w3.org/2001/xml-events");

                writer.WriteAttributeString("height", svgData.Height.ToString());
                writer.WriteAttributeString("width", svgData.Width.ToString());

                StiChartSvgHelper.WriteChart(writer, svgData, zoom, false);

                writer.WriteFullEndElement();
                writer.Flush();
                ms.Flush();
                writer.Close();
                ms.Close();
            }

            return sb.ToString();
        }

        private static void AddDefaultSeries(StiChart chart)
        {
            Type series1Type = null;
            if (chart.Series.Count > 0)
                series1Type = chart.Series[0].GetType();
            else
                series1Type = chart.Area.GetDefaultSeriesType();
            var series1 = (StiSeries)StiActivator.CreateObject(series1Type);

            Type series2Type = null;
            if (chart.Series.Count > 1)
                series2Type = chart.Series[1].GetType();
            else
                series2Type = series1Type;
            var series2 = (StiSeries)StiActivator.CreateObject(series2Type);

            chart.Series.Clear();

            if (series1 is IStiSunburstSeries || series2 is IStiSunburstSeries)
            {
                series1.Arguments = new string[] { "A", "A", "B", "C" };
                series2.Arguments = new string[] { "C", "D", "E", "" };
            }

            if (series2 is StiParetoSeries)
            {
                series1.Values = new double?[] { 4, 2, 1 };
            }

            chart.Series.Add(series1);

            if (!(series2 is StiFunnelSeries) && !(series2 is StiParetoSeries))
                chart.Series.Add(series2);

            chart.Series.ApplyStyle(chart.Style);

            chart.Series.OfType<StiPie3dSeries>().ToList().ForEach(s =>
            {
                s.Options3D.Height = 10;
                s.Options3D.Distance = 2;
            });
        }

        private static void SetAreaToSimpleMode(IStiArea chartArea)
        {
            if (chartArea is StiAxisArea area)
            {
                area.AxisCore.SwitchOff();
            }
            else if (chartArea is StiAxisArea3D area3D)
            {
                area3D.AxisCore.SwitchOff();
            }
        }

        public static StiChart CloneChart(StiChart chart)
        {
            var chartCloned = (StiChart)chart.Clone();

            chartCloned.Legend.Visible = false;
            chartCloned.SeriesLabels.Visible = false;
            chartCloned.Title.Visible = false;
            chartCloned.Table.Visible = false;
            chartCloned.Area.ShowShadow = false;
            chartCloned.HorSpacing = 10;
            chartCloned.VertSpacing = 10;

            SetAreaToSimpleMode(chartCloned.Area);
            AddDefaultSeries(chartCloned);

            chartCloned.ConstantLines.Clear();
            chartCloned.Strips.Clear();
            chartCloned.Core.ApplyStyle(chartCloned.Style);

            chartCloned.Brush = new StiEmptyBrush();
            chartCloned.Area.Brush = new StiEmptyBrush();

            return chartCloned;
        }

        public static StiChart CloneChart(IStiChartElement element)
        {
            var chartCloned = new StiChart();
            chartCloned.Page = element.Page;
            chartCloned.Series.Clear();
            chartCloned.Series.Add(element.GetChartSeries());

            SetAreaToSimpleMode(chartCloned.Area);
            AddDefaultSeries(chartCloned);

            chartCloned.ConstantLines.Clear();
            chartCloned.Strips.Clear();
            chartCloned.Core.ApplyStyle(chartCloned.Style);

            chartCloned.Brush = new StiEmptyBrush();
            chartCloned.Area.Brush = new StiEmptyBrush();

            chartCloned.Legend.Visible = false;
            chartCloned.SeriesLabels.Visible = false;
            chartCloned.Title.Visible = false;
            chartCloned.Table.Visible = false;

            chartCloned.AllowApplyStyle = true;
            chartCloned.Area.AllowApplyStyle = true;

            return chartCloned;
        }

        private static List<Chart.StiChartStyle> GetChartStyles(StiReport report, bool withReportStyles = true)
        {
            var styles = new List<Chart.StiChartStyle>();

            if (withReportStyles)
            {
                foreach (StiBaseStyle style in report.Styles)
                {
                    if (style is StiChartStyle)
                    {
                        var customStyle = new StiCustomStyle(style.Name);
                        customStyle.CustomCore.ReportChartStyle = style as StiChartStyle;
                        styles.Add(customStyle);
                    }
                }
            }

            styles.AddRange(StiOptions.Services.ChartStyles.Where(s => s.ServiceEnabled).Reverse());

            return styles;
        }
        #endregion

        #region Callback Methods
        public static void AddSeries(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            var chart = report.Pages.GetComponentByName((string)param["componentName"]) as StiChart;
            if (chart != null)
            {
                string seriesType = (string)param["seriesType"];
                Assembly assembly = typeof(StiReport).Assembly;
                var series = assembly.CreateInstance("Stimulsoft.Report.Chart." + seriesType) as StiSeries;
                chart.Series.Add(series);
                callbackResult["properties"] = GetChartProperties(chart);
            }
        }

        public static void RemoveSeries(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            var chart = report.Pages.GetComponentByName((string)param["componentName"]) as StiChart;
            int seriesIndex = Convert.ToInt32(param["seriesIndex"]);
            if (chart != null && seriesIndex >= 0 && seriesIndex < chart.Series.Count)
            {
                chart.Series.RemoveAt(seriesIndex);
                callbackResult["properties"] = GetChartProperties(chart);
            }
        }

        public static void SeriesMove(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            var chart = report.Pages.GetComponentByName((string)param["componentName"]) as StiChart;
            int seriesIndex = Convert.ToInt32(param["seriesIndex"]);
            string direction = (string)param["direction"];

            if (chart != null && seriesIndex >= 0 && seriesIndex < chart.Series.Count)
            {
                var series = chart.Series[seriesIndex];
                chart.Series.RemoveAt(seriesIndex);

                if (direction == "Up")
                    seriesIndex--;
                else
                    seriesIndex++;

                chart.Series.Insert(seriesIndex, series);
                callbackResult["properties"] = GetChartProperties(chart);
            }

            callbackResult["selectedIndex"] = seriesIndex;
        }

        public static void AddStrip(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            var chart = report.Pages.GetComponentByName((string)param["componentName"]) as StiChart;
            if (chart != null)
            {
                var strip = new StiStrips();
                strip.StripBrush = new StiGradientBrush(Color.FromArgb(100, 250, 240, 150), Color.FromArgb(100, 150, 120, 60), 90);
                chart.Strips.Add(strip);
            }
        }

        public static void RemoveStrip(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            var chart = report.Pages.GetComponentByName((string)param["componentName"]) as StiChart;
            int stripIndex = Convert.ToInt32(param["stripIndex"]);
            if (chart != null && stripIndex >= 0 && stripIndex < chart.Strips.Count)
                chart.Strips.RemoveAt(stripIndex);
        }

        public static void MoveStrip(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            var chart = report.Pages.GetComponentByName((string)param["componentName"]) as StiChart;
            int fromIndex = Convert.ToInt32(param["fromIndex"]);
            int toIndex = Convert.ToInt32(param["toIndex"]);

            if (chart != null && fromIndex < chart.Strips.Count && toIndex < chart.Strips.Count)
            {
                var strip = chart.Strips[fromIndex];
                chart.Strips.RemoveAt(fromIndex);
                chart.Strips.Insert(toIndex, strip);
            }
        }

        public static void AddConstantLine(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            var chart = report.Pages.GetComponentByName((string)param["componentName"]) as StiChart;
            if (chart != null)
                chart.ConstantLines.Add(new StiConstantLines());
        }

        public static void RemoveConstantLine(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            var chart = report.Pages.GetComponentByName((string)param["componentName"]) as StiChart;
            int constLineIndex = Convert.ToInt32(param["constLineIndex"]);
            if (chart != null && constLineIndex >= 0 && constLineIndex < chart.ConstantLines.Count)
                chart.ConstantLines.RemoveAt(constLineIndex);
        }

        public static void MoveConstantLine(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            var chart = report.Pages.GetComponentByName((string)param["componentName"]) as StiChart;
            int fromIndex = Convert.ToInt32(param["fromIndex"]);
            int toIndex = Convert.ToInt32(param["toIndex"]);

            if (chart != null && fromIndex < chart.ConstantLines.Count && toIndex < chart.ConstantLines.Count)
            {
                var constantLine = chart.ConstantLines[fromIndex];
                chart.ConstantLines.RemoveAt(fromIndex);
                chart.ConstantLines.Insert(toIndex, constantLine);
            }
        }

        public static void AddConstantLineOrStrip(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            var component = report.Pages.GetComponentByName((string)param["componentName"]);
            if (component == null) return;
            StiChart chart = component as StiChart;

            string itemType = (string)param["itemType"];
            if (itemType == "ConstantLines")
            {
                StiConstantLines constantLine = new StiConstantLines();
                chart.ConstantLines.Add(constantLine);
            }
            else
            {
                StiStrips strip = new StiStrips();
                StiBrush brush = new StiGradientBrush(Color.FromArgb(100, 250, 240, 150), Color.FromArgb(100, 150, 120, 60), 90);
                strip.StripBrush = brush;
                chart.Strips.Add(strip);
            }

            callbackResult["properties"] = GetChartProperties(chart as StiChart);
            callbackResult["itemType"] = itemType;
        }

        public static void RemoveConstantLineOrStrip(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            var component = report.Pages.GetComponentByName((string)param["componentName"]);
            int itemIndex = Convert.ToInt32(param["itemIndex"]);
            string itemType = (string)param["itemType"];

            if (component == null || itemIndex == -1) return;
            StiChart chart = component as StiChart;

            if (itemType == "ConstantLines")
            {
                chart.ConstantLines.RemoveAt(itemIndex);
            }
            else
            {
                chart.Strips.RemoveAt(itemIndex);
            }

            callbackResult["properties"] = GetChartProperties(chart as StiChart);
            callbackResult["itemType"] = itemType;
        }

        public static void ConstantLineOrStripMove(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            var component = report.Pages.GetComponentByName((string)param["componentName"]);
            int itemIndex = Convert.ToInt32(param["itemIndex"]);
            string direction = (string)param["direction"];
            string itemType = (string)param["itemType"];

            if (component == null || itemIndex == -1) return;
            StiChart chart = component as StiChart;

            int nextIndex = (direction == "Up") ? itemIndex - 1 : itemIndex + 1;

            if (itemType == "ConstantLines")
            {
                var constantLine = chart.ConstantLines[itemIndex];
                chart.ConstantLines.RemoveAt(itemIndex);
                chart.ConstantLines.Insert(nextIndex, constantLine);
            }
            else
            {
                var strip = chart.Strips[itemIndex];
                chart.Strips.RemoveAt(itemIndex);
                chart.Strips.Insert(nextIndex, strip);
            }

            callbackResult["properties"] = GetChartProperties(chart as StiChart);
            callbackResult["selectedIndex"] = nextIndex;
            callbackResult["itemType"] = itemType;
        }

        public static void GetLabelsContent(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            var component = report.Pages.GetComponentByName((string)param["componentName"]);
            var seriesIndex = (param["seriesIndex"] != null) ? Convert.ToInt32(param["seriesIndex"]) : -1;
            if (component == null) return;

            StiChart chart = component as StiChart;
            ArrayList labelsContent = new ArrayList();
            var currentSeries = (seriesIndex != -1) ? chart.Series[seriesIndex] : null;

            #region clone chart
            var chartCloned = CloneChart(chart);

            if (currentSeries != null)
            {
                chartCloned.Series.Clear();

                var seriesClone = currentSeries.Clone() as StiSeries;
                seriesClone.ShowSeriesLabels = StiShowSeriesLabels.FromChart;

                chartCloned.Series.Add(seriesClone);
            }
            #endregion

            #region Check Series
            var services = new List<StiSeriesLabels>();
            foreach (var service in StiOptions.Services.ChartSerieLabels.Where(s => s.ServiceEnabled))
            {
                if (!chart.Area.Core.CheckInLabelsTypes(service.GetType()))
                    continue;

                services.Add(service);
            }
            #endregion

            #region Get button size
            int width;
            int height;
            if (services.Count < 6)
            {
                width = 160;
                height = 110;
            }
            else
            {
                width = 80;
                height = 80;
            }
            #endregion

            #region Create buttons

            foreach (var labels in services)
            {
                if (!chart.Area.Core.CheckInLabelsTypes(labels.GetType())) continue;

                chartCloned.SeriesLabels = (StiSeriesLabels)labels.Clone();
                chartCloned.Core.ApplyStyle(chartCloned.Style);

                Hashtable content = new Hashtable();
                content["caption"] = labels.ToString();
                content["image"] = GetChartSampleImage(chartCloned, width, height, 0.5f);
                content["type"] = labels.GetType().Name;
                content["width"] = width;
                content["height"] = height;
                labelsContent.Add(content);
            }
            #endregion

            callbackResult["labelsContent"] = labelsContent;
            if (currentSeries != null) callbackResult["isSeriesLables"] = true;
        }

        public static void GetStylesContent(StiReport report, Hashtable param, Hashtable callbackResult, bool forStylesControl, bool withReportStyles = true)
        {
            var component = report.Pages.GetComponentByName(param["componentName"] as string);
            var chart = component != null ? component as StiChart : new StiChart();
            var stylesContent = new ArrayList();

            #region clone chart
            var chartCloned = CloneChart(chart);
            #endregion

            foreach (var style in GetChartStyles(report, withReportStyles))
            {
                chartCloned.Style = (Chart.StiChartStyle)style.Clone();

                var customStyle = style as StiCustomStyle;
                if (customStyle != null)
                    chartCloned.CustomStyleName = customStyle.CustomCore.ReportChartStyle.Name;

                chartCloned.Core.ApplyStyle(chartCloned.Style);

                int width = forStylesControl ? 138 : 80;

                Hashtable content = new Hashtable();
                var zoom = chartCloned.Series.Count > 0 && chartCloned.Series[0] is StiPictorialSeries ? 0.5f : 1f;
                content["image"] = GetChartSampleImage(chartCloned, width, forStylesControl ? 67 : 80, zoom);
                content["type"] = style.GetType().Name;
                content["name"] = (style is StiCustomStyle) ? ((StiCustomStyleCoreXF)style.Core).ReportStyleName : "";
                content["width"] = width;
                content["height"] = forStylesControl ? 67 : 80;
                stylesContent.Add(content);
            }

            callbackResult["stylesContent"] = stylesContent;
        }

        public static void SetLabelsType(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            var component = report.Pages.GetComponentByName((string)param["componentName"]);
            string labelsType = (string)param["labelsType"];
            int seriesIndex = (param["seriesIndex"] != null) ? Convert.ToInt32(param["seriesIndex"]) : -1;

            if (component == null) return;
            StiChart chart = component as StiChart;

            Assembly assembly = typeof(StiReport).Assembly;
            var labels = assembly.CreateInstance("Stimulsoft.Report.Chart." + labelsType) as StiSeriesLabels;
            if (labels != null)
            {
                if (seriesIndex != -1)
                {
                    chart.Series[seriesIndex].SeriesLabels = labels;
                    chart.Series[seriesIndex].SeriesLabels.Core.ApplyStyle(chart.Style);
                }
                else
                {
                    chart.SeriesLabels = labels;
                    chart.SeriesLabels.Core.ApplyStyle(chart.Style);
                }

            }
            callbackResult["properties"] = GetChartProperties(chart);
            callbackResult["isSeriesLabels"] = seriesIndex != -1;
        }

        public static void SetLabelsType(StiComponent component, object propertyValue)
        {
            var chart = component as StiChart;
            Assembly assembly = typeof(StiReport).Assembly;
            var labels = assembly.CreateInstance("Stimulsoft.Report.Chart." + propertyValue) as StiSeriesLabels;
            if (labels != null)
            {
                chart.SeriesLabels = labels;
                chart.SeriesLabels.Core.ApplyStyle(chart.Style);
            }
        }

        public static void SetChartStyle(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            var component = report.Pages.GetComponentByName((string)param["componentName"]);
            if (component == null) return;

            StiChart chart = component as StiChart;
            if (chart == null) return;

            string styleType = (string)param["styleType"];
            string styleName = (string)param["styleName"];

            if (styleType == "StiCustomStyle")
            {
                chart.Style = new StiCustomStyle(styleName);
                chart.CustomStyleName = styleName;
            }
            else
            {
                Assembly assembly = typeof(StiReport).Assembly;
                var baseStyle = assembly.CreateInstance("Stimulsoft.Report.Chart." + styleType) as Stimulsoft.Report.Chart.StiChartStyle;
                if (baseStyle != null) chart.Style = baseStyle;
            }

            chart.Core.ApplyStyle(chart.Style);
            callbackResult["properties"] = GetChartProperties(chart);
        }

        public static void SetChartPropertyValue(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            var component = report.Pages.GetComponentByName((string)param["componentName"]);
            if (component == null) return;
            var chart = component as StiChart;
            var panelName = param["panelName"] as string;
            var propertyName = param["propertyName"] as string;
            object mainObject = null;

            if (param["ownerName"] != null && (string)param["ownerName"] != "ConstantLines" && (string)param["ownerName"] != "Strips")
                propertyName = ((string)param["ownerName"]) + "." + propertyName;

            switch (panelName)
            {
                case "Chart":
                    {
                        if (param["indexConstantLines"] != null)
                        {
                            int indexConstantLines = Convert.ToInt32(param["indexConstantLines"]);
                            if (indexConstantLines != -1)
                            {
                                if (component is StiChart)
                                {
                                    //Chart
                                    mainObject = chart.ConstantLines[indexConstantLines];
                                }
                                else if (component is IStiChartElement)
                                {
                                    //Chart Element
                                    var constLines = ((IStiChartElement)component).FetchConstantLines();
                                    if (indexConstantLines < constLines.Count)
                                    {
                                        StiReportEdit.SetPropertyValue(component.Report, propertyName, constLines[indexConstantLines], param["propertyValue"]);
                                        var properties = StiReportEdit.GetAllProperties(component);
                                        properties["svgContent"] = StiEncodingHelper.Encode(StiDashboardsSvgHelper.SaveElementToString(component as IStiChartElement, 1, 1, true));
                                        callbackResult["properties"] = properties;
                                        return;
                                    }
                                }
                            }
                        }
                        else if (param["indexStrips"] != null)
                        {
                            int indexStrips = Convert.ToInt32(param["indexStrips"]);
                            if (indexStrips != -1)
                                mainObject = chart.Strips[indexStrips];
                        }
                        else
                        {
                            mainObject = chart;
                        }
                        break;
                    }
                case "Series":
                case "SeriesLabels":
                    {
                        var seriesIndex = param["seriesIndex"] != null ? Convert.ToInt32(param["seriesIndex"]) : -1;
                        mainObject = seriesIndex != -1 ? chart.Series[seriesIndex] : null;
                        if (panelName == "SeriesLabels") { propertyName = "SeriesLabels." + propertyName; }
                        break;
                    }
                case "Labels":
                    {
                        mainObject = chart.SeriesLabels;
                        break;
                    }
                case "Area":
                    {
                        mainObject = chart.Area;
                        break;
                    }
            }

            if (mainObject != null)
                StiReportEdit.SetPropertyValue(chart.Report, propertyName, mainObject, param["propertyValue"]);

            callbackResult["properties"] = GetChartProperties(chart);
        }

        public static void SetContainerValue(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            var component = report.Pages.GetComponentByName((string)param["componentName"]);
            var seriesIndex = (param["seriesIndex"] != null) ? Convert.ToInt32(param["seriesIndex"]) : -1;
            if (component == null) return;
            StiChart chart = (StiChart)component;


            switch ((string)param["containerType"])
            {
                case "SeriesConditions":
                    {
                        if (seriesIndex != -1)
                        {
                            SetConditionsValue(chart.Series[seriesIndex].Conditions, (ArrayList)param["value"]);
                        }
                        break;
                    }
                case "LabelsConditions":
                    {
                        SetConditionsValue(chart.SeriesLabelsConditions, (ArrayList)param["value"]);
                        break;
                    }
                case "SeriesFilters":
                    {
                        if (seriesIndex != -1)
                        {
                            if (param["filterMode"] != null) ((StiSeries)chart.Series[seriesIndex]).FilterMode = (StiFilterMode)Enum.Parse(typeof(StiFilterMode), (string)param["filterMode"]);
                            SetFiltersValue(chart.Series[seriesIndex].Filters, (ArrayList)param["value"]);
                        }
                        break;
                    }
                case "SeriesTrendLines":
                    {
                        if (seriesIndex != -1)
                        {
                            SetTrendLinesValue(chart.Series[seriesIndex].TrendLines, (ArrayList)param["value"]);
                        }
                        break;
                    }
            }

            callbackResult["properties"] = GetChartProperties(chart);

            if (param["andCloseForm"] != null)
                callbackResult["closeChartForm"] = param["andCloseForm"];
        }
        #endregion
        #endregion

        #region Simple chart form
        #region Helper Methods
        private static StiSeries ConvertSeries(StiChart chart, StiSeries series, string newSeriesType)
        {
            var newSeries = CreateSeries(newSeriesType);

            TransferSeriesData(series, newSeries);
                        
            var index = chart.Series.IndexOf(series);
            chart.Series.RemoveAt(index);
            chart.Series.Insert(index, newSeries);

            return newSeries;
        }

        private static void TransferSeriesData(StiSeries seriesFrom, StiSeries seriesTo)
        {
            var valueDataColumn = seriesFrom.ValueDataColumn;            

            if (seriesFrom is StiStockSeries)
                valueDataColumn = ((StiStockSeries)seriesFrom).ValueDataColumnOpen;
            else if (seriesFrom is StiCandlestickSeries)
                valueDataColumn = ((StiCandlestickSeries)seriesFrom).ValueDataColumnOpen;

            if (seriesTo is StiStockSeries)
                ((StiStockSeries)seriesTo).ValueDataColumnOpen = valueDataColumn;
            else if (seriesTo is StiCandlestickSeries)
                ((StiCandlestickSeries)seriesTo).ValueDataColumnOpen = valueDataColumn;
            else
                seriesTo.ValueDataColumn = valueDataColumn;

            seriesTo.ListOfValues = seriesFrom.ListOfValues;
            seriesTo.Value = seriesFrom.Value;
            seriesTo.ArgumentDataColumn = seriesFrom.ArgumentDataColumn;
        }

        private static void SetSeriesType(StiChart chart, StiSeries series, string seriesType)
        {
            var newSeries = ConvertSeries(chart, series, seriesType);

            var seriesTypes = GetChartSeriesTypes(newSeries);
            if (seriesTypes != null)
            {
                chart.Series.ToList().ForEach(s =>
                {
                    if (newSeries.GetType() != s.GetType() && !seriesTypes.Contains(ConvertSeriesToChartSeriesType(s).ToString()))
                        ConvertSeries(chart, s, seriesType);
                });
            }
        }

        private static void RewriteArgumentDataColumns(StiChart chart, List<string> argumentDataColumns)
        {
            var index = 0;
            chart.Series.ToList().ForEach(s =>
            {
                s.ArgumentDataColumn = index < argumentDataColumns.Count ? argumentDataColumns[index] : string.Empty;
                index++;
            });
        }

        private static string GetDraggedDataColumnValue(StiChart chart, Hashtable param)
        {
            var draggedItem = param["draggedItem"] as Hashtable;
            var draggedItemObject = draggedItem["itemObject"] as Hashtable;
            var dataColumnValue = string.Empty;

            if (draggedItemObject.Contains("fullName"))
            {
                dataColumnValue = StiEncodingHelper.DecodeString(draggedItemObject["fullName"] as string);
                if (dataColumnValue.StartsWith("{") && dataColumnValue.EndsWith("}"))
                    dataColumnValue = dataColumnValue.Substring(1, dataColumnValue.Length - 2);
            }

            return dataColumnValue;
        }

        private static StiChartSeriesType ConvertSeriesToChartSeriesType(StiSeries series)
        {
            var seriesTypeStr = series.GetType().ToString();
            var seriesIndex = seriesTypeStr.LastIndexOf(".") + 1;
            seriesTypeStr = seriesTypeStr.Substring(seriesIndex).Substring(3).Replace("Series", "");

            return (StiChartSeriesType)Enum.Parse(typeof(StiChartSeriesType), seriesTypeStr);
        }

        private static List<string> GetChartSeriesTypes(StiSeries series)
        {
            var seriesType = ConvertSeriesToChartSeriesType(series);
            var seriesTypes = new List<string>();

            var group = StiChartGroups.GetGroup(seriesType);
            if (group != null)
            {
                foreach (var groupSeriesType in group)
                {
                    seriesTypes.Add(groupSeriesType.ToString());
                }
            }

            return seriesTypes;
        }

        private static StiSeries CreateSeries(string seriesType)
        {
            if (seriesType.EndsWith("3D"))
                seriesType = $"Stimulsoft.Report.Chart.Sti{seriesType.Substring(0, seriesType.Length - 2)}Series3D";
            else
                seriesType = $"Stimulsoft.Report.Chart.Sti{seriesType}Series";

            return typeof(StiReport).Assembly.CreateInstance(seriesType) as StiSeries;
        }

        private static List<string> GetArgumentDataColumns(StiChart chart)
        {
            var argumentDataColumns = new List<string>();
            chart.Series.ToList().ForEach(s =>
            {
                if (!string.IsNullOrEmpty(s.ArgumentDataColumn))
                    argumentDataColumns.Add(s.ArgumentDataColumn);
            });
            return argumentDataColumns;
        }

        private static void InsertSeries(StiChart chart, int insertIndex, StiSeries series)
        {
            chart.Series.Insert(insertIndex >= 0 && insertIndex < chart.Series.Count ? insertIndex : chart.Series.Count, series);
        }

        private static string getSeriesDataColumnPropertyName(StiSeries series, string containerName)
        {
            var propertyName = series is IStiFinancialSeries ? "ValueDataColumnOpen" : "ValueDataColumn";
            if (containerName == "endValues") propertyName = "ValueDataColumnEnd";
            else if (containerName == "closeValues") propertyName = "ValueDataColumnClose";
            else if (containerName == "lowValues") propertyName = "ValueDataColumnLow";
            else if (containerName == "highValues") propertyName = "ValueDataColumnHigh";
            else if (containerName == "arguments") propertyName = "ArgumentDataColumn";
            else if (containerName == "weights") propertyName = "WeightDataColumn";
            return propertyName;
        }

        private static void SetDataColumnValue(StiChart chart, StiSeries series, string containerName, string dataColumnValue)
        {
            var propertyName = getSeriesDataColumnPropertyName(series, containerName);
            StiReportEdit.SetPropertyValue(chart.Report, propertyName, series, dataColumnValue);
        }

        private static string GetDataColumnValue(StiSeries series, string containerName)
        {
            var propertyName = getSeriesDataColumnPropertyName(series, containerName);
            return series.GetType().GetProperty(propertyName)?.GetValue(series, null) as string;
        }

        internal static bool IsAxisAreaChart(StiChart chart)
        {
            return chart.Series != null && chart.Series.ToList().Any(s => s != null &&
                 (s is StiClusteredBarSeries ||
                 s is StiClusteredColumnSeries ||
                 s is StiParetoSeries ||
                 s is StiHistogramSeries ||
                 s is StiRibbonSeries ||
                 s is StiFullStackedAreaSeries ||
                 s is StiFullStackedBarSeries ||
                 s is StiFullStackedColumnSeries ||
                 s is StiFullStackedLineSeries ||
                 s is StiFullStackedSplineSeries ||
                 s is StiFullStackedSplineAreaSeries ||

                 s is StiStackedBarSeries ||
                 s is StiStackedColumnSeries ||
                 s is StiStackedLineSeries ||
                 s is StiStackedSplineSeries ||
                 s is StiStackedAreaSeries ||
                 s is StiStackedSplineAreaSeries ||
                 s is StiScatterSeries ||
                 s is StiScatterLineSeries ||
                 s is StiScatterSplineSeries ||

                 s is StiCandlestickSeries ||
                 s is StiStockSeries ||

                 s is StiGanttSeries ||
                 s is StiBubbleSeries ||

                 s is StiAreaSeries ||
                 s is StiLineSeries ||
                 s is StiSplineSeries ||
                 s is StiSplineAreaSeries ||
                 s is StiSplineRangeSeries ||
                 s is StiRangeSeries ||
                 s is StiRangeBarSeries ||
                 s is StiSteppedAreaSeries ||
                 s is StiSteppedLineSeries ||
                 s is StiSteppedRangeSeries));
        }

        internal static bool IsAxisAreaChart3D(StiChart chart)
        {
            return chart.Series != null && chart.Series.ToList().Any(s => s != null &&
                 (s is StiClusteredColumnSeries3D ||
                 s is StiFullStackedColumnSeries3D ||
                 s is StiStackedColumnSeries3D));
        }

        internal static bool IsClusteredColumnChart3D(StiChart chart)
        {
            return chart.Series != null && chart.Series.ToList().Any(s => s != null && s is StiClusteredColumnSeries3D);
        }

        internal static bool IsPieChart(StiChart chart)
        {
            return chart.Series != null && chart.Series.ToList().Any(s => s != null && s is StiPieSeries);
        }

        internal static bool IsDoughnutChart(StiChart chart)
        {
            return chart.Series != null && chart.Series.ToList().Any(s => s != null && s is StiDoughnutSeries);
        }

        internal static bool IsFunnelChart(StiChart chart)
        {
            return chart.Series != null && chart.Series.ToList().Any(s => s != null && (s is StiFunnelSeries || s is StiFunnelWeightedSlicesSeries));
        }

        internal static bool IsTreemapChart(StiChart chart)
        {
            return chart.Series != null && chart.Series.ToList().Any(s => s != null && s is StiTreemapSeries);
        }

        internal static bool IsSunburstChart(StiChart chart)
        {
            return chart.Series != null && chart.Series.ToList().Any(s => s != null && s is StiSunburstSeries);
        }

        internal static bool IsWaterfallChart(StiChart chart)
        {
            return chart.Series != null && chart.Series.ToList().Any(s => s != null && s is StiWaterfallSeries);
        }

        internal static bool IsPictorialStackedChart(StiChart chart)
        {
            return chart.Series != null && chart.Series.ToList().Any(s => s != null && s is StiPictorialStackedSeries);
        }

        internal static bool IsStackedChart(StiChart chart)
        {
            return chart.Series != null && chart.Series.ToList().Any(s => s != null &&
            (s is StiStackedBarSeries ||
            s is StiStackedColumnSeries ||
            s is StiStackedLineSeries ||
            s is StiStackedSplineSeries ||
            s is StiStackedAreaSeries ||
            s is StiStackedSplineAreaSeries));
        }
        #endregion

        #region Callback Methods
        public static void UpdateChart(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            var command = param["innerCommand"] as string;
            var component = report.Pages.GetComponentByName((string)param["componentName"]);
            if (component == null) return;
            var chart = component as StiChart;

            switch (command)
            {
                case "InsertDataColumn":
                    {
                        InsertDataColumn(chart, param, callbackResult);
                        break;
                    }
                case "RemoveDataColumn":
                    {
                        RemoveDataColumn(chart, param, callbackResult);
                        break;
                    }
                case "SetPropertyValue":
                    {
                        SetPropertyValue(chart, param, callbackResult);
                        break;
                    }
                case "AddSeries":
                    {
                        AddSeries(chart.Report, param, callbackResult);
                        break;
                    }
                case "RemoveSeries":
                    {
                        RemoveSeries(chart.Report, param, callbackResult);
                        break;
                    }
                case "MoveSeries":
                    {
                        MoveSeries(chart, param, callbackResult);
                        break;
                    }
                case "MoveArgument":
                    {
                        MoveArgument(chart, param, callbackResult);
                        break;
                    }
                case "MoveMeter":
                    {
                        MoveMeter(chart, param, callbackResult);
                        break;
                    }
                case "AddStrip":
                    {
                        AddStrip(chart.Report, param, callbackResult);
                        break;
                    }
                case "RemoveStrip":
                    {
                        RemoveStrip(chart.Report, param, callbackResult);
                        break;
                    }
                case "MoveStrip":
                    {
                        MoveStrip(chart.Report, param, callbackResult);
                        break;
                    }
                case "AddConstantLine":
                    {
                        AddConstantLine(chart.Report, param, callbackResult);
                        break;
                    }
                case "RemoveConstantLine":
                    {
                        RemoveConstantLine(chart.Report, param, callbackResult);
                        break;
                    }
                case "MoveConstantLine":
                    {
                        MoveConstantLine(chart.Report, param, callbackResult);
                        break;
                    }
            }

            if (!callbackResult.Contains("chartProperties"))
                callbackResult["chartProperties"] = GetChartProperties(chart);
        }

        private static void InsertDataColumn(StiChart chart, Hashtable param, Hashtable callbackResult)
        {
            var insertIndex = param["insertIndex"] != null ? Convert.ToInt32(param["insertIndex"]) : -1;
            var seriesType = param["seriesType"] != null ? param["seriesType"] as string : "ClusteredColumn";
            var containerName = param["containerName"] as string;
            var dataColumnValue = GetDraggedDataColumnValue(chart, param);

            if (containerName == "arguments")
            {
                var argumentDataColumns = GetArgumentDataColumns(chart);
                argumentDataColumns.Insert(insertIndex >= 0 && insertIndex < argumentDataColumns.Count ? insertIndex : argumentDataColumns.Count, dataColumnValue);

                if (argumentDataColumns.Count > chart.Series.Count)
                    InsertSeries(chart, insertIndex, CreateSeries(seriesType));

                RewriteArgumentDataColumns(chart, argumentDataColumns);
            }
            else
            {
                CreateSeriesOrSetDataColumn(chart, containerName, insertIndex, seriesType, dataColumnValue);
            }
        }

        private static void CreateSeriesOrSetDataColumn(StiChart chart, string containerName, int seriesIndex, string seriesType, string dataColumnValue)
        {
            var series = seriesIndex >= 0 && seriesIndex < chart.Series.Count ? chart.Series[seriesIndex] : chart.Series.Count > 0 ? chart.Series[chart.Series.Count - 1] : null;
            var seriesDataColumn = series != null ? GetDataColumnValue(series as StiSeries, containerName) : null;

            if (series != null && string.IsNullOrEmpty(seriesDataColumn))
            {
                SetDataColumnValue(chart, series as StiSeries, containerName, dataColumnValue);
            }
            else
            {
                var newSeries = CreateSeries(seriesType);
                InsertSeries(chart, seriesIndex, newSeries);
                SetDataColumnValue(chart, newSeries, containerName, dataColumnValue);
            }
        }

        private static void RemoveDataColumn(StiChart chart, Hashtable param, Hashtable callbackResult)
        {
            var itemIndex = Convert.ToInt32(param["itemIndex"]);
            var containerName = param["containerName"] as string;

            if (containerName == "arguments")
            {
                var argumentDataColumns = GetArgumentDataColumns(chart);

                if (itemIndex < argumentDataColumns.Count)
                    argumentDataColumns.RemoveAt(itemIndex);

                RewriteArgumentDataColumns(chart, argumentDataColumns);
            }
            else
            {
                if (itemIndex < chart.Series.Count)
                {
                    if (containerName == "values")
                        chart.Series.RemoveAt(itemIndex);
                    else
                        SetDataColumnValue(chart, chart.Series[itemIndex] as StiSeries, containerName, string.Empty);
                }
            }
        }

        private static void SetPropertyValue(StiChart chart, Hashtable param, Hashtable callbackResult)
        {
            var propertyName = param["propertyName"] as string;
            var propertyValue = param["propertyValue"];
            var itemIndex = Convert.ToInt32(param["itemIndex"]);
            var currentSeries = itemIndex < chart.Series.Count ? chart.Series[itemIndex] as StiSeries : null;

            if (currentSeries != null)
            {
                switch (propertyName)
                {
                    case "Icon":
                        StiReportEdit.SetPropertyValue(chart.Report, "Icon", currentSeries, propertyValue);
                        break;

                    case "SeriesType":
                        SetSeriesType(chart, currentSeries, param["propertyValue"] as string);
                        break;

                    case "DataColumn":
                        SetDataColumnValue(chart, currentSeries, param["containerName"] as string, StiEncodingHelper.DecodeString(param["propertyValue"] as string));
                        break;
                }
            }
        }

        public static void MoveSeries(StiChart chart, Hashtable param, Hashtable callbackResult)
        {
            int fromIndex = Convert.ToInt32(param["fromIndex"]);
            int toIndex = Convert.ToInt32(param["toIndex"]);

            if (fromIndex < chart.Series.Count)
            {
                var argumentDataColumns = GetArgumentDataColumns(chart);
                var series = chart.Series[fromIndex];
                chart.Series.RemoveAt(fromIndex);
                chart.Series.Insert(toIndex, series);
                RewriteArgumentDataColumns(chart, argumentDataColumns);
            }
        }

        public static void MoveArgument(StiChart chart, Hashtable param, Hashtable callbackResult)
        {
            int fromIndex = Convert.ToInt32(param["fromIndex"]);
            int toIndex = Convert.ToInt32(param["toIndex"]);
            var argumentDataColumns = GetArgumentDataColumns(chart);

            if (fromIndex < argumentDataColumns.Count)
            {
                var removingArgument = argumentDataColumns[fromIndex];
                argumentDataColumns.RemoveAt(fromIndex);
                argumentDataColumns.Insert(toIndex, removingArgument);
                RewriteArgumentDataColumns(chart, argumentDataColumns);
            }
        }

        public static void MoveMeter(StiChart chart, Hashtable param, Hashtable callbackResult)
        {
            var fromIndex = param["fromIndex"] != null ? Convert.ToInt32(param["fromIndex"]) : -1;
            var toIndex = param["toIndex"] != null ? Convert.ToInt32(param["toIndex"]) : -1;
            var toContainerName = param["toContainerName"] as string;
            var fromContainerName = param["fromContainerName"] as string;
            var seriesType = param["seriesType"] as string;
            var argumentDataColumns = GetArgumentDataColumns(chart);

            if (fromContainerName == "arguments")
            {
                if (fromIndex >= 0 && fromIndex < argumentDataColumns.Count)
                {
                    var removingArgument = argumentDataColumns[fromIndex];
                    argumentDataColumns.RemoveAt(fromIndex);

                    CreateSeriesOrSetDataColumn(chart, toContainerName, toIndex, seriesType, removingArgument);
                    RewriteArgumentDataColumns(chart, argumentDataColumns);
                }
            }
            else
            {
                if (fromIndex >= 0 && fromIndex < chart.Series.Count)
                {
                    var removingSeries = chart.Series[fromIndex];
                    var seriesDataColumn = GetDataColumnValue(removingSeries as StiSeries, fromContainerName);

                    if (toContainerName == "arguments")
                    {
                        argumentDataColumns.Insert(toIndex >= 0 && toIndex < argumentDataColumns.Count ? toIndex : argumentDataColumns.Count, seriesDataColumn);

                        if (argumentDataColumns.Count < chart.Series.Count)
                            chart.Series.RemoveAt(fromIndex);
                        else
                            SetDataColumnValue(chart, removingSeries as StiSeries, fromContainerName, string.Empty);

                        if (argumentDataColumns.Count > chart.Series.Count && !string.IsNullOrEmpty(seriesDataColumn))
                        {
                            var newSeries = CreateSeries(seriesType);
                            chart.Series.Add(newSeries);
                        }

                        RewriteArgumentDataColumns(chart, argumentDataColumns);
                    }
                    else
                    {
                        CreateSeriesOrSetDataColumn(chart, toContainerName, toIndex, seriesType, seriesDataColumn);
                        SetDataColumnValue(chart, removingSeries as StiSeries, fromContainerName, string.Empty);
                    }
                }
            }
            #endregion
            #endregion
        }
    }
}