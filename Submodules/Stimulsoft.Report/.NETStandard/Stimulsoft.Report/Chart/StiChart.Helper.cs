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

using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;

#if NETSTANDARD
using Stimulsoft.System.Drawing;
using ColorConverter = Stimulsoft.System.Drawing.ColorConverter;
#endif

namespace Stimulsoft.Report.Chart
{
    public class StiChartHelper
    {
        #region Properties.Static
        public static TimeSpan GlobalDurationElement { get; set; } = new TimeSpan(0, 0, 0, 0, 800);

        public static TimeSpan GlobalBeginTimeElement { get; set; } = new TimeSpan(0, 0, 0, 0, 800);
        #endregion

        #region Methods
        internal static void FillSeriesData(StiSeries series, List<StiDataItem> items)
        {
            series.Values = new double?[items.Count];
            series.Arguments = new object[items.Count];
            series.Tags = new object[items.Count];

            var anyTooltip = items.Any(i => i.ToolTip != null && !string.IsNullOrEmpty(i.ToolTip.ToString()));
            if (anyTooltip)
                series.ToolTips = new string[items.Count];

            for (var index = 0; index < items.Count; index++)
            {
                var item = items[index];

                series.Values[index] = item.Value is double ? (double)item.Value : 0d;
                series.Arguments[index] = item.Argument;

                if (anyTooltip)
                    series.ToolTips[index] = item.ToolTip.ToString();

                series.Tags[index] = item.Tag;

                if (series is IStiRangeSeries)
                    ((IStiRangeSeries)series).ValuesEnd[index] = item.ValueEnd is double ? (double)item.ValueEnd : 0d;

                if (series is IStiFinancialSeries)
                {
                    ((IStiFinancialSeries)series).ValuesOpen[index] = item.ValueOpen is double ? (double)item.ValueOpen : 0d;
                    ((IStiFinancialSeries)series).ValuesClose[index] = item.ValueClose is double ? (double)item.ValueClose : 0d;
                    ((IStiFinancialSeries)series).ValuesLow[index] = item.ValueLow is double ? (double)item.ValueLow : 0d;
                    ((IStiFinancialSeries)series).ValuesHigh[index] = item.ValueHigh is double ? (double)item.ValueHigh : 0d;
                }
            }
        }

        internal static object GetFilterData(StiReport report, StiChartFilter filter, string filterMethodName)
        {
            try
            {
                if (filter.Item == StiFilterItem.Expression)
                {
                    if (report.CalculationMode == StiCalculationMode.Interpretation)
                    {
                        var tempTxt = new StiText() { Name = "*Chart_Filter*", Page = report.Pages[0] };
                        var storeToPrint = false;
                        var parserResult = Engine.StiParser.ParseTextValue("{" + filter.Value + "}", tempTxt, ref storeToPrint, true);
                        return parserResult;
                    }
                    var method = report.GetType().GetMethod(filterMethodName);
                    return method.Invoke(report, new object[0]);
                }

                switch (filter.DataType)
                {
                    case StiFilterDataType.String:
                        return filter.Value;

                    case StiFilterDataType.Numeric:
                        double numberValue = 0;
                        double.TryParse(filter.Value.Replace(".", ",").Replace(",", Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator), out numberValue);

                        return numberValue;

                    case StiFilterDataType.DateTime:
                        var currentCulture = Thread.CurrentThread.CurrentCulture;
                        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-us", false);

                        DateTime result;
                        var state = DateTime.TryParse(filter.Value, out result);
                        Thread.CurrentThread.CurrentCulture = currentCulture;

                        if (state)
                        {
                            return result;
                        }
                        break;

                    case StiFilterDataType.Boolean:
                        return filter.Value.ToLower(CultureInfo.InvariantCulture) == "true";
                }
            }
            catch (Exception ee)
            {
                var error = $"Problem when parsing Chart {filter} value\n";

                throw new Exception(error, ee);
            }
            return null;
        }

        internal static bool GetFilterResult(StiChartFilter filter,
            object itemArgument, object itemValue, object itemValueEnd,
            object itemValueOpen, object itemValueClose,
            object itemValueLow, object itemValueHigh,
            object itemTooltip, object data)
        {
            if (filter.Item == StiFilterItem.Expression)
                return (bool)data;

            object itemData = null;

            switch (filter.Item)
            {
                case StiFilterItem.Argument:
                    itemData = itemArgument;
                    break;

                case StiFilterItem.Value:
                    itemData = itemValue;
                    break;

                case StiFilterItem.ValueEnd:
                    itemData = itemValueEnd;
                    break;

                case StiFilterItem.ValueOpen:
                    itemData = itemValueOpen;
                    break;

                case StiFilterItem.ValueClose:
                    itemData = itemValueClose;
                    break;

                case StiFilterItem.ValueLow:
                    itemData = itemValueLow;
                    break;

                case StiFilterItem.ValueHigh:
                    itemData = itemValueHigh;
                    break;

                case StiFilterItem.Tooltip:
                    itemData = itemTooltip;
                    break;
            }

            var result = false;
            if (itemData != null)
            {
                try
                {
                    if (filter.DataType == StiFilterDataType.Numeric)
                    {
                        double tempDouble = 0;
                        if (Double.TryParse(itemData.ToString(), out tempDouble))
                        {
                            itemData = tempDouble;
                        }
                    }

                    else if (filter.DataType == StiFilterDataType.DateTime)
                    {
                        DateTime resultDateTime;

                        if (DateTime.TryParse(itemData.ToString(), out resultDateTime))
                        {
                            itemData = resultDateTime;
                        }
                        else
                        {
                            itemData = DateTime.FromOADate((double)itemData);
                        }
                    }
                }
                catch
                {
                }

                var comparable = itemData as IComparable;
                if (comparable != null && data != null)
                {
                    if (itemData.GetType() != data.GetType())
                    {
                        itemData = itemData.ToString();
                        data = data.ToString();
                        comparable = itemData as IComparable;
                    }

                    try
                    {
                        #region filter.Condition
                        switch (filter.Condition)
                        {
                            case StiFilterCondition.EqualTo:
                                return comparable.CompareTo(data) == 0;

                            case StiFilterCondition.NotEqualTo:
                                return comparable.CompareTo(data) != 0;

                            case StiFilterCondition.GreaterThan:
                                return comparable.CompareTo(data) == 1;

                            case StiFilterCondition.GreaterThanOrEqualTo:
                                return comparable.CompareTo(data) >= 0;

                            case StiFilterCondition.LessThan:
                                return comparable.CompareTo(data) == -1;

                            case StiFilterCondition.LessThanOrEqualTo:
                                return comparable.CompareTo(data) <= 0;

                            case StiFilterCondition.Containing:
                                var str1 = itemData.ToString();
                                var str2 = data.ToString();
                                return str1.IndexOf(str2, StringComparison.InvariantCulture) != -1;

                            case StiFilterCondition.NotContaining:
                                var str3 = itemData.ToString();
                                var str4 = data.ToString();
                                return str3.IndexOf(str4, StringComparison.InvariantCulture) == -1;

                            case StiFilterCondition.BeginningWith:
                                var str5 = itemData.ToString();
                                var str6 = data.ToString();
                                return str5.StartsWith(str6, StringComparison.InvariantCulture);

                            case StiFilterCondition.EndingWith:
                                var str7 = itemData.ToString();
                                var str8 = data.ToString();
                                return str7.EndsWith(str8, StringComparison.InvariantCulture);
                        }
                        #endregion
                    }
                    catch
                    {
                    }
                }
            }
            return result;
        }

        internal static StiImage GetImageFromChart(StiChart masterChart)
        {
            var imageComp = new StiImage
            {
                Left = masterChart.Left,
                Top = masterChart.Top,
                Width = masterChart.Width,
                Height = masterChart.Height,
                Page = masterChart.Page,
                Smoothing = false,
                Stretch = true,
                Name = masterChart.Name
            };

            var rect = masterChart.GetPaintRectangle();
            rect.X = 0;
            rect.Y = 0;

            var zoom = StiOptions.Engine.RenderChartAsBitmapZoom;
            imageComp.PutImageToDraw(masterChart.GetImage(ref zoom));

            return imageComp;
        }

        internal static object ConvertStringToColor(string colorStr)
        {
            object color = null;
            if (colorStr != null)
            {
                colorStr = colorStr.Trim();
                var separator = ',';
                for (var index = 0; index < colorStr.Length; index++)
                {
                    if (!(Char.IsDigit(colorStr[index]) || Char.IsWhiteSpace(colorStr[index])))
                    {
                        separator = colorStr[index];
                        break;
                    }
                }

                var rgb = colorStr.Split(separator);
                if (rgb.Length == 3 || rgb.Length == 4)
                {
                    try
                    {
                        if (rgb.Length == 3)
                        {
                            color = Color.FromArgb(
                                int.Parse(rgb[0].Trim()),
                                int.Parse(rgb[1].Trim()),
                                int.Parse(rgb[2].Trim()));
                        }
                        else
                        {
                            color = Color.FromArgb(
                                int.Parse(rgb[0].Trim()),
                                int.Parse(rgb[1].Trim()),
                                int.Parse(rgb[2].Trim()),
                                int.Parse(rgb[3].Trim()));
                        }
                    }
                    catch
                    {
                        var error = $"Can't get color from '{colorStr}'";
                        StiLogService.Write($"StiChartHelper.ConvertStringToColor {error}");
                        throw new ArgumentException(error);
                    }
                }
                else
                {
                    try
                    {
                        var result = 0;
                        if (int.TryParse(colorStr, out result))
                        {
                            var color2 = Color.FromArgb(result);
                            if (result < 0xffffff) color2 = Color.FromArgb(0xff, color2.R, color2.G, color2.B);
                            return color2;
                        }
                        var converter = new ColorConverter();
                        return converter.ConvertFromString(colorStr);
                    }
                    catch
                    {
                        var error = $"Can't get color from '{colorStr}'";
                        StiLogService.Write($"StiChartHelper.ConvertStringToColor {error}");
                        throw new ArgumentException(error);
                    }
                }
            }
            return color;
        }

        internal static void CreateChart(StiChart masterChart, StiChart chartComp)
        {
            var seriesToColor = new Hashtable();
            var applyStyle = false;

            var seriesFilterIndex = 0;
            var seriesIndex = 0;
            while (seriesIndex < chartComp.Series.Count)
            {
                var series = chartComp.Series[seriesIndex] as StiSeries;

                #region Title
                SetTitle(masterChart, seriesIndex, series);
                #endregion

                #region Values
                series.ValuesStart = null;
                series.Values = CheckValueNaN(GetValues(masterChart, series));
                #endregion

                #region ValuesEnd
                var ganttSeries = chartComp.Series[seriesIndex] as StiGanttSeries;
                if (ganttSeries != null)
                {
                    ganttSeries.ValuesEnd =
                        GetValuesEnd(masterChart, ganttSeries, ganttSeries.ValueDataColumnEnd, ganttSeries.InvokeGetListOfValuesEnd, ganttSeries.InvokeGetValueEnd);
                }

                var rangSeries = chartComp.Series[seriesIndex] as StiRangeSeries;
                if (rangSeries != null)
                {
                    rangSeries.ValuesEnd =
                        GetValuesEnd(masterChart, rangSeries, rangSeries.ValueDataColumnEnd, rangSeries.InvokeGetListOfValuesEnd, rangSeries.InvokeGetValueEnd);
                }

                var steppedRangeSeries = chartComp.Series[seriesIndex] as StiSteppedRangeSeries;
                if (steppedRangeSeries != null)
                {
                    steppedRangeSeries.ValuesEnd =
                        GetValuesEnd(masterChart, steppedRangeSeries, steppedRangeSeries.ValueDataColumnEnd, steppedRangeSeries.InvokeGetListOfValuesEnd, steppedRangeSeries.InvokeGetValueEnd);
                }

                var rangeBarSeries = chartComp.Series[seriesIndex] as StiRangeBarSeries;
                if (rangeBarSeries != null)
                {
                    rangeBarSeries.ValuesEnd =
                        GetValuesEnd(masterChart, rangeBarSeries, rangeBarSeries.ValueDataColumnEnd, rangeBarSeries.InvokeGetListOfValuesEnd, rangeBarSeries.InvokeGetValueEnd);
                }

                var splineRangeSeries = chartComp.Series[seriesIndex] as StiSplineRangeSeries;
                if (splineRangeSeries != null)
                {
                    splineRangeSeries.ValuesEnd =
                        GetValuesEnd(masterChart, splineRangeSeries, splineRangeSeries.ValueDataColumnEnd, splineRangeSeries.InvokeGetListOfValuesEnd, splineRangeSeries.InvokeGetValueEnd);
                }
                #endregion

                #region Values High, Low, Close, Open
                var candlestickSeries = chartComp.Series[seriesIndex] as StiCandlestickSeries;
                if (candlestickSeries != null)
                {
                    candlestickSeries.ValuesHigh = GetValuesHigh(masterChart, candlestickSeries);
                    candlestickSeries.ValuesLow = GetValuesLow(masterChart, candlestickSeries);
                    candlestickSeries.ValuesClose = GetValuesClose(masterChart, candlestickSeries);
                    candlestickSeries.ValuesOpen = GetValuesOpen(masterChart, candlestickSeries);
                }
                #endregion

                #region Weights
                var bubbleSeries = chartComp.Series[seriesIndex] as StiBubbleSeries;
                if (bubbleSeries != null)
                    bubbleSeries.Weights = GetWeights(masterChart, bubbleSeries);
                #endregion

                #region CutPieList
                var pieSeries = series as StiPieSeries;
                if (pieSeries != null)
                    SetCutPieList(masterChart, pieSeries);
                #endregion

                masterChart.CacheValues(true);

                #region Arguments
                series.Arguments = GetArguments(masterChart, series);
                #endregion

                #region Tags
                series.Tags = GetTags(masterChart, series);
                #endregion

                #region Hyperlinks
                series.Hyperlinks = GetHyperlinks(masterChart, series);
                #endregion

                #region ToolTips
                series.ToolTips = GetToolTips(masterChart, series);
                #endregion

                CheckParetoValues(series);
                if (series is StiHistogramSeries)
                {
                    var formatService = ((StiAxisArea)series.Chart.Area).XAxis.Labels.FormatService;
                    StiHistogramHelper.CheckValuesAndArguments(series, formatService);
                }

                masterChart.CacheValues(false);

                #region AutoSeries
                var autoSeriesKeys = GetAutoSeriesKeysFromAutoSeriesKeyDataColumn(masterChart, series);
                var autoSeriesTitles = GetAutoSeriesTitleFromAutoSeriesTitleDataColumn(masterChart, series);
                var autoSeriesColors = GetAutoSeriesColorFromAutoSeriesColorDataColumn(masterChart, series);
                #endregion

                #region Sort & filter & autoseries data
                if (series.SortBy != StiSeriesSortType.None ||
                    series.Filters.Count > 0 ||
                    autoSeriesKeys.Length > 0)
                {
                    #region Create Data Items
                    var count = series.Values.Length > series.Arguments.Length ?
                        series.Values.Length : series.Arguments.Length;

                    var items = new List<StiDataItem>();

                    for (var index = 0; index < count; index++)
                    {
                        object value = series.Values.Length > index ? series.Values[index] : 0;
                        var argument = series.Arguments.Length > index ? series.Arguments[index] : index;
                        object key = autoSeriesKeys.Length > index ? autoSeriesKeys[index] : null;
                        object title = autoSeriesTitles.Length > index ? autoSeriesTitles[index] : null;
                        var toolTip = series.ToolTips.Length > index ? series.ToolTips[index] : string.Empty;
                        var color = autoSeriesColors.Length > index ? autoSeriesColors[index] : null;

                        var tag = series.Tags.Length > index ? series.Tags[index] : 0;

                        object valueEnd = null;
                        if (series is IStiRangeSeries)
                            valueEnd = ((IStiRangeSeries)series).ValuesEnd.Length > index ? ((IStiRangeSeries)series).ValuesEnd[index] : 0;

                        object valueOpen = null;
                        object valueClose = null;
                        object valueLow = null;
                        object valueHigh = null;

                        if (series is IStiFinancialSeries)
                        {
                            valueOpen = ((IStiFinancialSeries)series).ValuesOpen.Length > index ? ((IStiFinancialSeries)series).ValuesOpen[index] : 0;
                            valueClose = ((IStiFinancialSeries)series).ValuesClose.Length > index ? ((IStiFinancialSeries)series).ValuesClose[index] : 0;
                            valueLow = ((IStiFinancialSeries)series).ValuesLow.Length > index ? ((IStiFinancialSeries)series).ValuesLow[index] : 0;
                            valueHigh = ((IStiFinancialSeries)series).ValuesHigh.Length > index ? ((IStiFinancialSeries)series).ValuesHigh[index] : 0;
                        }

                        object weight = null;
                        if (bubbleSeries != null)
                            weight = bubbleSeries.Weights.Length > index ? bubbleSeries.Weights[index] : 0;

                        items.Add(new StiDataItem(index, argument ?? string.Empty, value, valueEnd, weight, valueOpen, valueClose, valueLow, valueHigh, title, key, color, toolTip, tag));
                    }
                    #endregion

                    #region Filter
                    #region Prepare filter values
                    var filterToValue = new Hashtable();

                    var filterIndex = 0;
                    foreach (StiChartFilter filter in series.Filters)
                    {
                        var filterMethodName = $"{chartComp.Name}Filters_{seriesFilterIndex}_{filterIndex}";
                        filterToValue[filter] = GetFilterData(chartComp.Report, filter, filterMethodName);
                        filterIndex++;
                    }
                    #endregion

                    var filteredItems = new List<StiDataItem>();

                    #region Get DataSource or BusinessObject for Filtering
                    StiDataSource filterSource = null;
                    StiBusinessObject filterBusinessObject = null;

                    if (series.Filters.Count > 0)
                    {
                        #region DataSource
                        try
                        {
                            filterSource = StiDataColumn.GetDataSourceFromDataColumn(chartComp.Report.Dictionary, series.ValueDataColumn);
                        }
                        catch
                        {
                        }

                        if (filterSource == null)
                        {
                            try
                            {
                                filterSource = StiDataColumn.GetDataSourceFromDataColumn(chartComp.Report.Dictionary, series.ArgumentDataColumn);
                            }
                            catch
                            {
                            }
                        }

                        if (filterSource == null)
                            filterSource = chartComp.DataSource;
                        #endregion

                        #region BusinessObject
                        try
                        {
                            filterBusinessObject = StiDataColumn.GetBusinessObjectFromDataColumn(chartComp.Report.Dictionary, series.ValueDataColumn);
                        }
                        catch
                        {
                        }

                        if (filterBusinessObject == null)
                        {
                            try
                            {
                                filterBusinessObject = StiDataColumn.GetBusinessObjectFromDataColumn(chartComp.Report.Dictionary, series.ArgumentDataColumn);
                            }
                            catch
                            {
                            }
                        }

                        if (filterBusinessObject == null)
                            filterBusinessObject = chartComp.BusinessObject;
                        #endregion
                    }
                    #endregion

                    #region Save States
                    if (filterSource != null)
                    {
                        filterSource.SaveState("ChartFilter");
                        filterSource.First();
                    }

                    if (filterBusinessObject != null)
                    {
                        filterBusinessObject.SaveState("ChartFilter");
                        filterBusinessObject.CreateEnumerator();
                    }
                    #endregion

                    foreach (var item in items)
                    {
                        var results = new bool[series.Filters.Count];

                        #region Create results
                        var index = 0;
                        foreach (StiChartFilter filter in series.Filters)
                        {
                            results[index] =
                                GetFilterResult(filter, item.Argument, item.Value, item.ValueEnd,
                                item.ValueOpen, item.ValueClose, item.ValueLow, item.ValueHigh, item.ToolTip, filterToValue[filter]);

                            if (filter.Item == StiFilterItem.Expression)
                            {
                                var filterMethodName = $"{chartComp.Name}Filters_{seriesFilterIndex}_{index}";
                                try
                                {
                                    results[index] = (bool)GetFilterData(chartComp.Report, filter, filterMethodName);
                                }
                                catch
                                {
                                }
                            }

                            index++;
                        }
                        #endregion

                        var result2 = true;

                        #region Check results
                        if (series.FilterMode == StiFilterMode.And)
                        {
                            foreach (var rs in results)
                            {
                                if (!rs)
                                {
                                    result2 = false;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            result2 = false;
                            foreach (var rs in results)
                            {
                                if (rs)
                                {
                                    result2 = true;
                                    break;
                                }
                            }
                        }
                        #endregion

                        if (result2) filteredItems.Add(item);
                        if (filterSource != null) filterSource.Next();
                        if (filterBusinessObject != null) filterBusinessObject.Next();
                    }

                    #region Restore States
                    if (filterSource != null)
                        filterSource.RestoreState("ChartFilter");

                    if (filterBusinessObject != null)
                        filterBusinessObject.RestoreState("ChartFilter");
                    #endregion

                    items = filteredItems;
                    #endregion

                    #region Sort
                    if (series.SortBy != StiSeriesSortType.None)
                    {
                        var comparer = new StiDataItemComparer(series.SortBy, series.SortDirection);
                        items.Sort(comparer);
                    }
                    #endregion

                    FillSeriesData(series, items);

                    #region AutoSeries
                    if (autoSeriesKeys.Length > 0)
                    {
                        #region Create list of arguments
                        var itemToArgument = new Hashtable();
                        object[] argumentKeys = null;
                        if (series.Arguments != null && series.Arguments.Length > 0)
                        {
                            #region Create list of argument keys
                            argumentKeys = items.Select(i => i.Argument).Distinct().ToArray();

                            if (series.SortBy == StiSeriesSortType.Argument)
                            {
                                Array.Sort(argumentKeys);
                                if (series.SortDirection == StiSeriesSortDirection.Descending) Array.Reverse(argumentKeys);
                            }
                            #endregion
                        }
                        else
                        {
                            #region Create list of argument keys
                            var lenghtOfItemsPerKey = new Hashtable();
                            foreach (var item in items)
                            {
                                var list = lenghtOfItemsPerKey[item.Key] as List<StiDataItem>;
                                if (list == null)
                                {
                                    list = new List<StiDataItem>();
                                    lenghtOfItemsPerKey[item.Key] = list;
                                }

                                list.Add(item);
                            }

                            var maxValue = 0;
                            foreach (List<StiDataItem> value in lenghtOfItemsPerKey.Values)
                            {
                                maxValue = Math.Max(maxValue, value.Count);
                                var index = 0;
                                foreach (var item in value)
                                {
                                    item.Argument = index.ToString();
                                    itemToArgument[item] = index.ToString();
                                    index++;
                                }
                            }

                            argumentKeys = new string[maxValue];
                            for (var index = 0; index < maxValue; index++)
                            {
                                argumentKeys[index] = index.ToString();
                            }
                            #endregion
                        }
                        #endregion

                        #region Create hash and list of item keys
                        var hashKeys = new Hashtable();
                        foreach (var key in autoSeriesKeys)
                        {
                            hashKeys[key] = key;
                        }

                        var keys = new string[hashKeys.Count];
                        hashKeys.Keys.CopyTo(keys, 0);

                        SortArray(ref keys);
                        #endregion

                        var itemsArray = new List<StiDataItem>[argumentKeys.Length, keys.Length];

                        #region Create indexes
                        var argumentToIndex = new Hashtable();
                        for (var index = 0; index < argumentKeys.Length; index++)
                        {
                            argumentToIndex[argumentKeys[index]] = index;
                        }

                        var keyToIndex = new Hashtable();
                        for (var index = 0; index < keys.Length; index++)
                        {
                            keyToIndex[keys[index]] = index;
                        }
                        #endregion

                        #region Calculate items
                        var keyToTitle = new Hashtable();
                        var keyToColor = new Hashtable();
                        foreach (var item in items)
                        {
                            object key = GetKey(item.Key); //get the correct key the same as used in keyToIndex
                            var argument = item.Argument;
                            if (series.Arguments == null || series.Arguments.Length == 0)
                                argument = itemToArgument[item];

                            var indexKey = (int)keyToIndex[key];
                            var indexArgument = (int)argumentToIndex[argument];

                            if (item.Title != null) keyToTitle[key] = item.Title;
                            else keyToTitle[key] = item.Key;
                            keyToColor[key] = item.Color;

                            if (itemsArray[indexArgument, indexKey] != null)
                                itemsArray[indexArgument, indexKey].Add(item);

                            else
                            {
                                var list = new List<StiDataItem> { item };
                                itemsArray[indexArgument, indexKey] = list;
                            }
                        }
                        #endregion

                        StiBubbleSeries newBubbleSeries = null;

                        if (bubbleSeries != null)
                            newBubbleSeries = bubbleSeries.Clone() as StiBubbleSeries;

                        for (var index = 0; index < keys.Length; index++)
                        {
                            var key = keys[index];

                            StiSeries newSeries;
                            if (index == 0)
                                newSeries = series;
                            else
                            {
                                newSeries = series.Clone() as StiSeries;
                                chartComp.Series.Insert(seriesIndex + index, newSeries);
                            }

                            newSeries.CoreTitle = keyToTitle[key] as string;
                            var color = keyToColor[key] as string;
                            seriesToColor[newSeries] = color;

                            var values = new List<double?>();
                            var valuesEnd = new List<double?>();
                            var arguments = new List<object>();
                            var weights = new List<double>();
                            var toolTips = new List<string>();
                            var tags = new List<object>();


                            if (newSeries is IStiScatterSeries)
                            {
                                var mainList = new List<StiDataItem>();
                                for (var argumentIndex = 0; argumentIndex < argumentKeys.Length; argumentIndex++)
                                {
                                    var list = itemsArray[argumentIndex, index];

                                    if (list != null)
                                        mainList.AddRange(list);                                        
                                }

                                mainList = mainList.OrderBy(x => x.Index).ToList();

                                foreach (var dataItem in mainList)
                                {
                                    double? value = null;
                                    var isNullArgument = newSeries is IStiScatterSeries;

                                    if (dataItem != null && dataItem.Value is double)
                                    {
                                        value = (double)dataItem.Value;
                                        isNullArgument = false;
                                    }

                                    if (dataItem.Argument == null)
                                        continue;

                                    values.Add(value);

                                    if (!isNullArgument)
                                        arguments.Add(dataItem.Argument);

                                    if (newBubbleSeries != null)
                                        weights.Add((double)dataItem.Weight);

                                    if (dataItem.ToolTip != null)
                                        toolTips.Add(dataItem.ToolTip.ToString());

                                    if (dataItem.Tag != null)
                                        tags.Add(dataItem.Tag);
                                }
                            }
                            else
                            {
                                for (var argumentIndex = 0; argumentIndex < argumentKeys.Length; argumentIndex++)
                                {
                                    var list = itemsArray[argumentIndex, index];
                                    if (list == null)
                                    {
                                        values.Add(null);
                                        weights.Add(0);
                                        toolTips.Add(null);
                                        tags.Add(null);

                                        if (newSeries is IStiRangeSeries) valuesEnd.Add(null);
                                        arguments.Add(argumentKeys[argumentIndex]);
                                    }
                                    else
                                    {
                                        foreach (var dataItem in list)
                                        {
                                            double? value = null;
                                            var isNullArgument = newSeries is IStiScatterSeries;

                                            if (dataItem != null && dataItem.Value is double)
                                            {
                                                value = (double)dataItem.Value;
                                                isNullArgument = false;
                                            }

                                            values.Add(value);

                                            if (!isNullArgument)
                                                arguments.Add(argumentKeys[argumentIndex]);

                                            if (newSeries is IStiRangeSeries)
                                                valuesEnd.Add((double)dataItem.ValueEnd);

                                            if (dataItem.ToolTip != null)
                                                toolTips.Add(dataItem.ToolTip.ToString());

                                            if (dataItem.Tag != null)
                                                tags.Add(dataItem.Tag);
                                        }
                                    }
                                }
                            }

                            var arrayValues = new double?[values.Count];
                            values.CopyTo(arrayValues);
                            newSeries.Values = arrayValues;
                            var arrayArguments = new object[arguments.Count];
                            arguments.CopyTo(arrayArguments);
                            newSeries.Arguments = arrayArguments;

                            if (newSeries is IStiRangeSeries)
                            {
                                var arrayValuesEnd = new double?[valuesEnd.Count];
                                valuesEnd.CopyTo(arrayValuesEnd);
                                ((IStiRangeSeries)newSeries).ValuesEnd = arrayValuesEnd;
                            }

                            if (newSeries is StiBubbleSeries)
                            {
                                var arrayWeights = new double[weights.Count];
                                weights.CopyTo(arrayWeights);
                                ((StiBubbleSeries)newSeries).Weights = arrayWeights;
                            }

                            newSeries.ToolTips = toolTips.ToArray();
                            newSeries.Tags = tags.ToArray();

                            CheckArgumentsDateTimeStep(newSeries);
                            CreateTopN(newSeries);

                            newSeries.Filters = series.Filters;

                            applyStyle = true;
                        }

                        seriesIndex += keys.Length - 1;
                    }
                    else
                    {
                        CreateTopN(series);
                    }
                    #endregion
                }
                #endregion

                else
                {
                    CheckArgumentsDateTimeStep(series);
                    CreateTopN(series);
                }

                seriesIndex++;
                seriesFilterIndex++;
            }

            StiWaterfallHelper.CheckTotals(chartComp);
            StiBoxAndWhiskerHelper.CheckArgument(chartComp);

            if (StiOptions.Engine.AllowSimplifyChartValues)
                chartComp.SimplifyValues();

            if (StiOptions.Engine.AllowInvokeProcessChartEventForTemplateOfChart)
                chartComp.InvokeEvents();

            ApplyStyleAutoSeries(applyStyle, chartComp, seriesToColor);
        }

        private static void ApplyStyleAutoSeries(bool applyStyle, StiChart chartComp, Hashtable seriesToColor)
        {
            if (applyStyle && StiOptions.Engine.ApplyStylesInAutoSeries)
            {
                chartComp.Series.ApplyStyle(chartComp.Style);
                var index = 0;

                foreach (StiSeries series in chartComp.Series)
                {
                    var color = ConvertStringToColor(seriesToColor[series] as string);

                    var arg = new StiNewAutoSeriesEventArgs(index, series, null);
                    if (color != null)
                        arg.Color = (Color)color;

                    series.InvokeNewAutoSeries(arg);

                    if (arg.Color is Color) series.Core.ApplyStyle(chartComp.Style, (Color)arg.Color);
                    index++;
                }
            }
        }

        internal static List<PointD> GetShorterListPoints(StiSeries series)
        {
            var points = new PointD[series.Values.Length];

            for (var index = 0; index < series.Values.Length; index++)
            {
                points[index] = new PointD(series.Values[index].GetValueOrDefault(), index);
            }

            var deltaArray = new[] { 0.3, 0.6, 1, 5, 9, 15, 20, 25, 30, 40 };
            List<PointD> shorterListPoints = null;
            List<PointD> prevShorterListPoints = null;

            foreach (var delta in deltaArray)
            {
                shorterListPoints = StiSimplifyHelper.Simplify(points, delta, true);

                if ((double)shorterListPoints.Count / points.Length < 0.02 && shorterListPoints.Count < 900 &&
                    prevShorterListPoints != null)
                {
                    shorterListPoints = prevShorterListPoints;
                    break;
                }

                prevShorterListPoints = shorterListPoints;
            }

            return shorterListPoints;
        }

        private static void CheckParetoValues(StiSeries series)
        {
            var paretoSeries = series as StiParetoSeries;
            if (paretoSeries != null)
            {
                var values = new List<double?>();
                var arguments = new List<object>();

                for (var index = 0; index < series.Values.Length; index++)
                {
                    var value = series.Values[index].GetValueOrDefault();
                    if (value > 0)
                    {
                        values.Add(value);
                        if (series.Arguments != null && index < series.Arguments.Length)
                        {
                            arguments.Add(series.Arguments[index]);
                        }
                        else
                        {
                            arguments.Add(index + 1);
                        }
                    }
                }

                paretoSeries.Values = values.ToArray();
                paretoSeries.Arguments = arguments.ToArray();
                paretoSeries.ValuesStart = null;
            }
        }

        private static double?[] CheckValueNaN(double?[] values)
        {
            for (var index = 0; index < values.Length; index++)
            {
                if (values[index] != null && double.IsNaN(values[index].GetValueOrDefault()))
                {
                    values[index] = null;
                }
            }
            return values;
        }

        private static void CheckArgumentsDateTimeStep(StiSeries series)
        {
            var objects = series.Arguments;
            if (!(series.Chart.Area is StiAxisArea)) return;

            var dateTimeStep = ((StiXAxis)((StiAxisArea)series.Chart.Area).XAxis).DateTimeStep;
            var step = dateTimeStep.Step;
            var countValues = dateTimeStep.NumberOfValues;

            if (IsArgumentsDateTime(objects) && step != StiTimeDateStep.None)
            {
                var arguments = new DateTime[objects.Length];

                for (var index = 0; index < objects.Length; index++)
                {
                    arguments[index] = DateTime.Parse(objects[index].ToString());
                }

                var dateMax = MaximumDate(arguments);
                var dateMin = MinimumDate(arguments);

                var totalStep = ((int)GetTotalTimeSpans(step, dateMax, dateMin)) + 1;


                var listArguments = new List<object>();
                var listValues = new List<double?>();

                #region Add Real Values
                var firstDate = dateMin;
                var secondDate = GetNextDate(dateMin, step);

                for (var index = 0; index < totalStep; index++)
                {
                    var ticks = (secondDate.Ticks - firstDate.Ticks) / countValues;
                    var halfNewSpan = new TimeSpan(ticks / 2);

                    for (var indexNumberOfValues = 0; indexNumberOfValues < countValues; indexNumberOfValues++)
                    {
                        var arg = new TimeSpan(ticks * indexNumberOfValues);

                        if (arg.Ticks > dateMax.Ticks)
                            continue;

                        listArguments.Add(firstDate.Add(arg));


                        var firstHalfDate = (DateTime)listArguments[listArguments.Count - 1] - halfNewSpan;
                        var secondHalfDate = (DateTime)listArguments[listArguments.Count - 1] + halfNewSpan;

                        var val = GetValueForDate(firstHalfDate, secondHalfDate, arguments, series.Values);
                        listValues.Add(val);
                    }

                    firstDate = secondDate;
                    secondDate = GetNextDate(secondDate, step);
                }
                #endregion

                var newArguments = new object[listArguments.Count];
                var newValues = new double?[listValues.Count];

                listArguments.CopyTo(newArguments);
                listValues.CopyTo(newValues);

                #region Add Interpolation Values
                var firstIndexRealValue = 0;

                double? firstValue = null;

                for (var index = 0; index < newValues.Length; index++)
                {
                    if (newValues[index] != null && firstValue == null)
                    {
                        firstIndexRealValue = index;
                        firstValue = newValues[index];
                        continue;
                    }

                    if (newValues[index] != null && firstValue != null)
                    {
                        var lastIndexRealValue = index;

                        firstValue = newValues[firstIndexRealValue];
                        var lastValue = newValues[lastIndexRealValue];

                        var stepRealValue = (firstValue.GetValueOrDefault() - lastValue.GetValueOrDefault()) / (lastIndexRealValue - firstIndexRealValue);

                        for (var indexRealValue = firstIndexRealValue + 1; indexRealValue < lastIndexRealValue; indexRealValue++)
                        {
                            if (dateTimeStep.Interpolation)
                                newValues[indexRealValue] = newValues[indexRealValue - 1] - stepRealValue;

                            else
                                newValues[indexRealValue] = 0;
                        }

                        firstIndexRealValue = 0;

                        firstValue = null;

                        index--;
                    }
                }
                #endregion

                series.Arguments = newArguments;
                series.Values = newValues;
            }
        }

        private static void CreateTopN(StiSeries series)
        {
            if (series.TopN.Mode == StiTopNMode.None || series.Values.Length <= series.TopN.Count) return;

            if (series is StiBubbleSeries bubbleSeries)
                CreateValuesWeightsTopN(bubbleSeries);

            else
                CreateValuesTopN(series);
        }

        private static void CreateValuesTopN(StiSeries series)
        {
            var count = series.TopN.Count;

            var tempValues = series.Values.Clone() as double?[];
            var tempIndexValues = new int[series.Values.Length];

            for (var indexValues = 0; indexValues < series.Values.Length; indexValues++)
            {
                tempIndexValues[indexValues] = indexValues;
            }

            Array.Sort(tempValues, tempIndexValues);

            if (series.TopN.Mode == StiTopNMode.Top)
            {
                Array.Reverse(tempValues);
                Array.Reverse(tempIndexValues);
            }

            var newValues = new double?[count];
            var newIndex = new int[count];

            Array.Copy(tempValues, newValues, count);
            Array.Copy(tempIndexValues, newIndex, count);

            var listValues = new List<double?>();
            var listArguments = new List<object>();
            double? otherValue = 0d;

            for (var index = 0; index < series.Values.Length; index++)
            {
                if (FindIndex(newIndex, index))
                {
                    listValues.Add(series.Values[index]);

                    if (index < series.Arguments.Length)
                        listArguments.Add(series.Arguments[index]);
                    else
                        listArguments.Add(index);
                }
                else
                {
                    otherValue += series.Values[index];
                }
            }

            if (series.TopN.ShowOthers)
            {
                if (series is StiClusteredBarSeries)
                {
                    listValues.Insert(0, otherValue);
                    listArguments.Insert(0, series.TopN.OthersText);
                }
                else
                {
                    listValues.Add(otherValue);
                    listArguments.Add(series.TopN.OthersText);
                }
            }

            var resultValues = new double?[listValues.Count];
            listValues.CopyTo(resultValues);

            var resultArgiments = new object[listArguments.Count];
            listArguments.CopyTo(resultArgiments);

            series.Values = resultValues;
            series.Arguments = resultArgiments;
        }

        private static void CreateValuesWeightsTopN(StiBubbleSeries series)
        {
            var count = series.TopN.Count;

            var tempWeights = series.Weights.Clone() as double[];
            var tempIndexWeights = new int[series.Weights.Length];

            for (var indexWeights = 0; indexWeights < series.Weights.Length; indexWeights++)
            {
                tempIndexWeights[indexWeights] = indexWeights;
            }

            Array.Sort(tempWeights, tempIndexWeights);

            if (series.TopN.Mode == StiTopNMode.Top)
            {
                Array.Reverse(tempWeights);
                Array.Reverse(tempIndexWeights);
            }

            var newWeights = new double[count];
            var newIndex = new int[count];

            Array.Copy(tempWeights, newWeights, count);
            Array.Copy(tempIndexWeights, newIndex, count);

            var listWeights = new List<double>();
            var listValues = new List<double?>();
            var listArguments = new List<object>();

            for (var index = 0; index < series.Weights.Length; index++)
            {
                if (FindIndex(newIndex, index))
                {
                    listWeights.Add(series.Weights[index]);

                    if (index < series.Values.Length)
                        listValues.Add(series.Values[index]);

                    if (index < series.Arguments.Length)
                        listArguments.Add(series.Arguments[index]);
                }
            }

            series.Weights = listWeights.ToArray();
            series.Values = listValues.ToArray();
            series.Arguments = listArguments.ToArray();
        }

        private static DateTime GetNextDate(DateTime firstDate, StiTimeDateStep step)
        {
            switch (step)
            {
                case StiTimeDateStep.Second:
                    return firstDate.AddSeconds(1);

                case StiTimeDateStep.Minute:
                    return firstDate.AddMinutes(1);

                case StiTimeDateStep.Day:
                    return firstDate.AddDays(1);

                case StiTimeDateStep.Hour:
                    return firstDate.AddHours(1);

                case StiTimeDateStep.Month:
                    return firstDate.AddMonths(1);

                case StiTimeDateStep.Year:
                    return firstDate.AddYears(1);
            }
            return firstDate;
        }

        private static string GetKey(object key)
        {
            double numbD;
            if (double.TryParse(key.ToString(), out numbD))
            {
                return numbD.ToString();
            }
            return key.ToString();
        }

        private static void SortArray(ref string[] arrayString)
        {
            var listDouble = new List<double>();
            var listString = new List<string>();

            for (var index = 0; index < arrayString.Length; index++)
            {
                double numbD;

                if (double.TryParse(arrayString[index], out numbD))
                    listDouble.Add(numbD);

                else
                    listString.Add(arrayString[index]);
            }

            listDouble.Sort();
            listString.Sort();

            var newArrayString = new string[arrayString.Length];

            var newIndex = 0;

            foreach (var numbD in listDouble)
            {
                newArrayString[newIndex] = numbD.ToString();
                newIndex++;
            }

            foreach (var str in listString)
            {
                newArrayString[newIndex] = str;
                newIndex++;
            }

            arrayString = newArrayString;
        }

        private static bool FindIndex(int[] array, int value)
        {
            return array.Any(t => t == value);
        }

        private static double? GetValueForDate(DateTime dateStart, DateTime dateEnd, DateTime[] arguments, double?[] values)
        {
            var count = 0;
            double sum = 0;
            double? valueForDate = null;

            for (var index = 0; index < arguments.Length; index++)
            {
                var currentDate = arguments[index];

                if (currentDate > dateStart && currentDate <= dateEnd)
                {
                    count++;
                    sum += values[index].GetValueOrDefault();
                }
            }

            if (count != 0)
            {
                valueForDate = sum / count;
            }

            return valueForDate;
        }

        private static double GetTotalTimeSpans(StiTimeDateStep step, DateTime dateMax, DateTime dateMin)
        {
            var span = dateMax - dateMin;

            double total = 0;
            switch (step)
            {
                case StiTimeDateStep.Second:
                    total = span.TotalSeconds;
                    break;

                case StiTimeDateStep.Minute:
                    total = span.TotalMinutes;
                    break;

                case StiTimeDateStep.Hour:
                    total = span.TotalHours;
                    break;

                case StiTimeDateStep.Day:
                    total = span.TotalDays;
                    break;

                case StiTimeDateStep.Month:
                    total = ((dateMax.Year - dateMin.Year) * 12) + dateMax.Month - dateMin.Month;
                    break;

                case StiTimeDateStep.Year:
                    total = dateMax.Year - dateMin.Year;
                    break;
            }
            return total;
        }

        private static bool IsArgumentsDateTime(object[] arguments)
        {
            if (arguments.Length == 0) return false;

            foreach (var argument in arguments)
            {
                if (argument == null)
                    return false;

                var strForDate = argument.ToString();

                DateTime dt;
                if (!DateTime.TryParse(strForDate, out dt))
                    return false;
            }
            return true;
        }

        private static DateTime MaximumDate(DateTime[] dates)
        {
            var maxDate = DateTime.MinValue;
            foreach (var date in dates)
            {
                if (date.ToOADate() > maxDate.ToOADate())
                {
                    maxDate = date;
                }
            }
            return maxDate;
        }

        private static DateTime MinimumDate(DateTime[] dates)
        {
            var minDate = DateTime.MaxValue;
            foreach (var date in dates)
            {
                if (date.ToOADate() < minDate.ToOADate())
                {
                    minDate = date;
                }
            }
            return minDate;
        }

        private static string[] GetAutoSeriesColorFromAutoSeriesColorDataColumn(StiChart masterChart, StiSeries series)
        {
            if (series.AutoSeriesColorDataColumn == null || series.AutoSeriesColorDataColumn.Trim().Length <= 0) return new string[0];

            #region DataSource
            var dataSource =
                StiDataColumn.GetDataSourceFromDataColumn(masterChart.Report.Dictionary,
                    series.AutoSeriesColorDataColumn);

            if (dataSource != null)
            {
                dataSource.SaveState("ChartRender_DataColumn");

                var autoSeriesColors = new string[dataSource.Count];
                dataSource.First();

                for (var posIndex = 0; posIndex < dataSource.Count; posIndex++)
                {
                    var valueObject = StiDataColumn.GetDataFromDataColumn(
                        masterChart.Report.Dictionary, series.AutoSeriesColorDataColumn);

                    if (valueObject == null)
                        valueObject = string.Empty;
                    else
                        valueObject = valueObject.ToString();

                    autoSeriesColors[posIndex] = valueObject as string;
                    dataSource.Next();
                }
                dataSource.RestoreState("ChartRender_DataColumn");
                return autoSeriesColors;
            }
            #endregion

            #region BusinessObject
            var businessObject =
                StiDataColumn.GetBusinessObjectFromDataColumn(masterChart.Report.Dictionary,
                    series.AutoSeriesColorDataColumn);

            if (businessObject != null)
            {

                businessObject.SaveState("ChartRender_DataColumn");
                businessObject.CreateEnumerator();
                businessObject.specTotalsCalculation = true;

                var autoSeriesColors = new string[businessObject.Count];
                businessObject.First();

                for (var posIndex = 0; posIndex < businessObject.Count; posIndex++)
                {
                    var valueObject = StiDataColumn.GetDataFromBusinessObject(
                        masterChart.Report.Dictionary, series.AutoSeriesColorDataColumn);

                    if (valueObject == null)
                        valueObject = string.Empty;
                    else
                        valueObject = valueObject.ToString();

                    autoSeriesColors[posIndex] = valueObject as string;
                    businessObject.Next();
                }
                businessObject.RestoreState("ChartRender_DataColumn");
                return autoSeriesColors;
            }
            #endregion
            return new string[0];
        }

        private static string[] GetAutoSeriesTitleFromAutoSeriesTitleDataColumn(StiChart masterChart, StiSeries series)
        {
            if (series.AutoSeriesTitleDataColumn == null || series.AutoSeriesTitleDataColumn.Trim().Length <= 0) return new string[0];

            #region DataSource
            var dataSource =
                StiDataColumn.GetDataSourceFromDataColumn(masterChart.Report.Dictionary,
                    series.AutoSeriesTitleDataColumn);

            if (dataSource != null)
            {

                dataSource.SaveState("ChartRender_DataColumn");

                var autoSeriesTitles = new string[dataSource.Count];
                dataSource.First();

                for (var posIndex = 0; posIndex < dataSource.Count; posIndex++)
                {
                    var valueObject = StiDataColumn.GetDataFromDataColumn(
                        masterChart.Report.Dictionary, series.AutoSeriesTitleDataColumn);

                    if (valueObject == null)
                        valueObject = string.Empty;
                    else
                        valueObject = valueObject.ToString();

                    autoSeriesTitles[posIndex] = valueObject as string;
                    dataSource.Next();
                }
                dataSource.RestoreState("ChartRender_DataColumn");
                return autoSeriesTitles;
            }
            #endregion

            #region BusinessObject
            var businessObject =
                StiDataColumn.GetBusinessObjectFromDataColumn(masterChart.Report.Dictionary,
                    series.AutoSeriesTitleDataColumn);

            if (businessObject != null)
            {

                businessObject.SaveState("ChartRender_DataColumn");
                businessObject.CreateEnumerator();
                businessObject.specTotalsCalculation = true;

                var autoSeriesTitles = new string[businessObject.Count];
                businessObject.First();

                for (var posIndex = 0; posIndex < businessObject.Count; posIndex++)
                {
                    var valueObject = StiDataColumn.GetDataFromBusinessObject(
                        masterChart.Report.Dictionary, series.AutoSeriesTitleDataColumn);

                    if (valueObject == null)
                        valueObject = string.Empty;
                    else
                        valueObject = valueObject.ToString();

                    autoSeriesTitles[posIndex] = valueObject as string;
                    businessObject.Next();
                }
                businessObject.RestoreState("ChartRender_DataColumn");
                return autoSeriesTitles;
            }
            #endregion
            return new string[0];
        }

        private static string[] GetAutoSeriesKeysFromAutoSeriesKeyDataColumn(StiChart masterChart, StiSeries series)
        {
            if (series.AutoSeriesKeyDataColumn == null || series.AutoSeriesKeyDataColumn.Trim().Length <= 0) return new string[0];

            #region Data Source
            var dataSource =
                StiDataColumn.GetDataSourceFromDataColumn(
                    masterChart.Report.Dictionary, series.AutoSeriesKeyDataColumn);

            if (dataSource != null)
            {

                dataSource.SaveState("ChartRender_DataColumn");

                var autoSeriesKeys = new string[dataSource.Count];
                dataSource.First();

                for (var posIndex = 0; posIndex < dataSource.Count; posIndex++)
                {
                    var valueObject = StiDataColumn.GetDataFromDataColumn(
                        masterChart.Report.Dictionary, series.AutoSeriesKeyDataColumn);

                    if (valueObject == null)
                        valueObject = string.Empty;
                    else
                        valueObject = valueObject.ToString();

                    autoSeriesKeys[posIndex] = valueObject as string;
                    dataSource.Next();
                }
                dataSource.RestoreState("ChartRender_DataColumn");
                return autoSeriesKeys;
            }
            #endregion

            #region BusinessObject
            var businessObject =
                StiDataColumn.GetBusinessObjectFromDataColumn(
                    masterChart.Report.Dictionary, series.AutoSeriesKeyDataColumn);

            if (businessObject != null)
            {

                businessObject.SaveState("ChartRender_DataColumn");
                businessObject.CreateEnumerator();
                businessObject.specTotalsCalculation = true;

                var autoSeriesKeys = new string[businessObject.Count];
                businessObject.First();

                for (var posIndex = 0; posIndex < businessObject.Count; posIndex++)
                {
                    var valueObject = StiDataColumn.GetDataFromBusinessObject(
                        masterChart.Report.Dictionary, series.AutoSeriesKeyDataColumn);

                    if (valueObject == null)
                        valueObject = string.Empty;
                    else
                        valueObject = valueObject.ToString();

                    autoSeriesKeys[posIndex] = valueObject as string;
                    businessObject.Next();
                }
                businessObject.RestoreState("ChartRender_DataColumn");
                return autoSeriesKeys;
            }
            #endregion
            return new string[0];
        }

        private static void SetTitle(StiChart masterChart, int seriesIndex, StiSeries series)
        {
            var eTitle = new StiGetTitleEventArgs
            {
                Series = series,
                Index = seriesIndex
            };
            series.InvokeGetTitle(masterChart, eTitle);

            if (!string.IsNullOrEmpty(eTitle.Value))
            {
                series.CoreTitle = eTitle.Value;
            }
        }

        private static void SetCutPieList(StiChart masterChart, StiPieSeries series)
        {
            var ep = new StiGetValueEventArgs();
            series.InvokeGetCutPieList(masterChart, ep);

            if (!string.IsNullOrEmpty(ep.Value))
                series.CutPieListValues = StiSeries.GetValuesFromString(ep.Value);
        }
        #endregion

        #region GetArguments
        private static object[] GetArguments(StiChart masterChart, StiSeries series)
        {
            var arguments = GetArgumentsFromListOfArguments(masterChart, series);
            if (arguments != null)
                return arguments;

            arguments = GetArgumentsFromArgumentDataColumn(masterChart, series);
            if (arguments != null)
                return arguments;

            arguments = GetArgumentsFromArgumentExpression(masterChart, series);
            if (arguments != null)
                return arguments;

            return new object[0];
        }

        private static object[] GetArgumentsFromArgumentExpression(StiChart masterChart, StiSeries series)
        {
            var arguments = new object[masterChart.Count];
            masterChart.First();

            for (var posIndex = 0; posIndex < masterChart.Count; posIndex++)
            {
                var ee = new StiValueEventArgs();
                series.InvokeGetArgument(masterChart, ee);

                arguments[posIndex] = ee.Value;
                masterChart.Next();
            }
            return arguments;
        }

        private static object[] GetArgumentsFromArgumentDataColumn(StiChart masterChart, StiSeries series)
        {
            if (series.ArgumentDataColumn == null || series.ArgumentDataColumn.Trim().Length <= 0) return null;

            #region DataSource
            var dataSource =
                StiDataColumn.GetDataSourceFromDataColumn(masterChart.Report.Dictionary, series.ArgumentDataColumn);

            if (dataSource != null)
            {
                dataSource.SaveState("ChartRender_DataColumn");

                var arguments = new object[dataSource.Count];
                dataSource.First();

                for (var posIndex = 0; posIndex < dataSource.Count; posIndex++)
                {
                    var dataColumn = StiDataColumn.GetDataColumnFromColumnName(masterChart.Report.Dictionary, series.ArgumentDataColumn);
                    if (dataColumn != null && dataColumn.Type == typeof(DateTime))
                    {
                        var dataTimeValue = StiDataColumn.GetDataFromDataColumn(masterChart.Report.Dictionary, series.ArgumentDataColumn);
                        if (!(dataTimeValue is DateTime))
                        {
                            DateTime date;
                            if (DateTime.TryParse(dataTimeValue.ToString(), out date))
                                arguments[posIndex] = date;

                            else
                                arguments[posIndex] = dataTimeValue;
                        }
                        else
                            arguments[posIndex] = dataTimeValue;
                    }
                    else
                    {
                        arguments[posIndex] = StiDataColumn.GetDataFromDataColumn(masterChart.Report.Dictionary, series.ArgumentDataColumn);
                    }

                    if (arguments[posIndex] is DateTime)
                        series.Core.IsDateTimeArguments = true;

                    dataSource.Next();
                }
                dataSource.RestoreState("ChartRender_DataColumn");
                return arguments;
            }
            #endregion

            #region BusinessObject
            var businessObject =
                StiDataColumn.GetBusinessObjectFromDataColumn(masterChart.Report.Dictionary, series.ArgumentDataColumn);

            if (businessObject != null)
            {
                businessObject.SaveState("ChartRender_DataColumn");
                businessObject.CreateEnumerator();
                businessObject.specTotalsCalculation = true;

                var arguments = new object[businessObject.Count];
                businessObject.First();

                for (var posIndex = 0; posIndex < businessObject.Count; posIndex++)
                {
                    arguments[posIndex] = StiDataColumn.GetDataFromBusinessObject(masterChart.Report.Dictionary, series.ArgumentDataColumn);
                    if (arguments[posIndex] is DateTime)
                        series.Core.IsDateTimeArguments = true;

                    businessObject.Next();
                }
                businessObject.RestoreState("ChartRender_DataColumn");
                return arguments;
            }
            #endregion
            return null;
        }

        private static object[] GetArgumentsFromListOfArguments(StiChart masterChart, StiSeries series)
        {
            var e2 = new StiGetValueEventArgs();
            series.InvokeGetListOfArguments(masterChart, e2);

            if (!string.IsNullOrEmpty(e2.Value))
                return StiSeries.GetArgumentsFromString(e2.Value);

            return null;
        }
        #endregion

        #region GetValues
        private static double?[] GetValues(StiChart masterChart, StiSeries series)
        {
            var values = GetValuesFromListOfValues(masterChart, series);
            if (values != null)
                return values;

            values = GetValuesFromValueDataColumn(masterChart, series);
            if (values != null)
                return values;

            values = GetValuesFromValueExpression(masterChart, series);
            if (values != null)
                return values;

            return new double?[0];
        }

        private static double?[] GetValuesFromValueExpression(StiChart masterChart, StiSeries series)
        {
            var values = new double?[masterChart.Count];

            masterChart.First();

            for (var posIndex = 0; posIndex < masterChart.Count; posIndex++)
            {
                var ee = new StiGetValueEventArgs { Value = "0" };

                series.InvokeGetValue(masterChart, ee);

                if (!string.IsNullOrEmpty(ee.Value))
                    values[posIndex] = (double?)StiReport.ChangeType(ee.Value, typeof(double?));

                masterChart.Next();
            }

            return values;
        }

        private static double?[] GetValuesFromValueDataColumn(StiChart masterChart, StiSeries series)
        {
            if (series.ValueDataColumn == null || series.ValueDataColumn.Trim().Length <= 0) return null;

            #region DataSource
            var dataSource =
                StiDataColumn.GetDataSourceFromDataColumn(masterChart.Report.Dictionary, series.ValueDataColumn);

            if (dataSource != null)
            {
                dataSource.SaveState("ChartRender_DataColumn");

                var values = new double?[dataSource.Count];
                dataSource.First();

                for (var posIndex = 0; posIndex < dataSource.Count; posIndex++)
                {
                    var data = StiDataColumn.GetDataFromDataColumn(masterChart.Report.Dictionary, series.ValueDataColumn);
                    double? value = null;

                    try
                    {
                        if (data is DateTime)
                        {
                            series.Core.IsDateTimeValues = true;
                            value = ((DateTime)data).ToOADate();
                        }
                        else if (data != null && data != DBNull.Value)
                            value = (double?)StiReport.ChangeType(data, typeof(double));
                    }
                    catch { }

                    values[posIndex] = value;
                    dataSource.Next();
                }
                dataSource.RestoreState("ChartRender_DataColumn");
                return values;
            }
            #endregion

            #region Business Object
            var businessObject =
                StiDataColumn.GetBusinessObjectFromDataColumn(masterChart.Report.Dictionary, series.ValueDataColumn);

            if (businessObject != null)
            {
                businessObject.SaveState("ChartRender_DataColumn");
                businessObject.CreateEnumerator();
                businessObject.specTotalsCalculation = true;

                var values = new double?[businessObject.Count];
                businessObject.First();

                for (var posIndex = 0; posIndex < businessObject.Count; posIndex++)
                {
                    var data = StiDataColumn.GetDataFromBusinessObject(masterChart.Report.Dictionary, series.ValueDataColumn);
                    double? value = null;
                    if (data is DateTime)
                    {
                        series.Core.IsDateTimeValues = true;
                        value = ((DateTime)data).ToOADate();
                    }
                    else if (data != null && data != DBNull.Value) value = (double?)StiReport.ChangeType(data, typeof(double));

                    values[posIndex] = value;
                    businessObject.Next();
                }
                businessObject.RestoreState("ChartRender_DataColumn");
                return values;
            }
            #endregion
            return null;
        }

        private static double?[] GetValuesFromListOfValues(StiChart masterChart, StiSeries series)
        {
            var e = new StiGetValueEventArgs();
            series.InvokeGetListOfValues(masterChart, e);

            if (!string.IsNullOrEmpty(e.Value))
                return StiSeries.GetNullableValuesFromString(series, e.Value);

            return null;
        }
        #endregion

        #region GetValuesEnd
        private delegate void InvokeGetValues(StiChart chart, StiGetValueEventArgs eg);

        private static double?[] GetValuesEnd(StiChart masterChart, StiSeries series, string valueDataColumnEnd, InvokeGetValues listValuesEnd, InvokeGetValues valuesEnd)
        {
            var values = GetValuesEndFromListOfValuesEnd(masterChart, series, listValuesEnd);
            if (values != null)
                return values;

            values = GetValuesEndFromValueDataColumnEnd(masterChart, series, valueDataColumnEnd);
            if (values != null)
                return values;

            values = GetValuesEndFromValueEndExpression(masterChart, valuesEnd);
            if (values != null)
                return values;

            return new double?[0];
        }

        private static double?[] GetValuesEndFromValueEndExpression(StiChart masterChart, InvokeGetValues valuesEnd)
        {
            var values = new double?[masterChart.Count];
            masterChart.First();

            for (var posIndex = 0; posIndex < masterChart.Count; posIndex++)
            {
                var eeg = new StiGetValueEventArgs { Value = "0" };

                valuesEnd(masterChart, eeg);

                values[posIndex] = (double)StiReport.ChangeType(eeg.Value, typeof(double));
                masterChart.Next();
            }
            return values;
        }

        private static double?[] GetValuesEndFromValueDataColumnEnd(StiChart masterChart, StiSeries series, string valueDataColumnEnd)
        {
            if (valueDataColumnEnd == null || valueDataColumnEnd.Trim().Length <= 0) return null;

            #region DataSource
            var dataSourceGantt =
                StiDataColumn.GetDataSourceFromDataColumn(masterChart.Report.Dictionary, valueDataColumnEnd);

            if (dataSourceGantt != null)
            {
                dataSourceGantt.SaveState("ChartRender_DataColumn");

                var values = new double?[dataSourceGantt.Count];
                dataSourceGantt.First();

                for (var posIndex = 0; posIndex < dataSourceGantt.Count; posIndex++)
                {
                    var data = StiDataColumn.GetDataFromDataColumn(masterChart.Report.Dictionary, valueDataColumnEnd);
                    double? value = null;
                    if (data is DateTime)
                    {
                        series.Core.IsDateTimeValues = true;
                        value = ((DateTime)data).ToOADate();
                    }
                    else if (data != null && data != DBNull.Value)
                        value = (double)StiReport.ChangeType(data, typeof(double));

                    values[posIndex] = value;
                    dataSourceGantt.Next();
                }
                dataSourceGantt.RestoreState("ChartRender_DataColumn");
                return values;
            }
            #endregion

            #region BusinessObject
            var businessObjectGantt =
                StiDataColumn.GetBusinessObjectFromDataColumn(masterChart.Report.Dictionary, valueDataColumnEnd);

            if (businessObjectGantt != null)
            {
                businessObjectGantt.SaveState("ChartRender_DataColumn");
                businessObjectGantt.CreateEnumerator();
                businessObjectGantt.specTotalsCalculation = true;

                var values = new double?[businessObjectGantt.Count];
                businessObjectGantt.First();

                for (var posIndex = 0; posIndex < businessObjectGantt.Count; posIndex++)
                {
                    var data = StiDataColumn.GetDataFromBusinessObject(masterChart.Report.Dictionary, valueDataColumnEnd);
                    double? value = null;
                    if (data is DateTime)
                    {
                        series.Core.IsDateTimeValues = true;
                        value = ((DateTime)data).ToOADate();
                    }
                    else if (data != null && data != DBNull.Value)
                        value = (double?)StiReport.ChangeType(data, typeof(double));

                    values[posIndex] = value;
                    businessObjectGantt.Next();
                }
                businessObjectGantt.RestoreState("ChartRender_DataColumn");
                return values;
            }
            #endregion
            return null;
        }

        private static double?[] GetValuesEndFromListOfValuesEnd(StiChart masterChart, StiSeries series, InvokeGetValues listValues)
        {
            var eg = new StiGetValueEventArgs();
            listValues(masterChart, eg);

            if (!string.IsNullOrEmpty(eg.Value))
                return StiSeries.GetNullableValuesFromString(series, eg.Value);

            return null;
        }
        #endregion

        #region GetValueOpen
        private static double?[] GetValuesOpen(StiChart masterChart, StiCandlestickSeries series)
        {
            var values = GetValuesOpenFromListOfValuesOpen(masterChart, series);
            if (values != null)
                return values;

            values = GetValuesOpenFromValueDataColumnOpen(masterChart, series);
            if (values != null)
                return values;

            values = GetValuesOpenFromValuesOpenExpression(masterChart, series);
            if (values != null)
                return values;

            return new double?[0];
        }

        private static double?[] GetValuesOpenFromValuesOpenExpression(StiChart masterChart, StiCandlestickSeries series)
        {
            var values = new double?[masterChart.Count];
            masterChart.First();

            for (var posIndex = 0; posIndex < masterChart.Count; posIndex++)
            {
                var eeg = new StiGetValueEventArgs { Value = "0" };

                series.InvokeGetValueOpen(masterChart, eeg);

                values[posIndex] = (double)StiReport.ChangeType(eeg.Value, typeof(double));
                masterChart.Next();
            }
            return values;
        }

        private static double?[] GetValuesOpenFromValueDataColumnOpen(StiChart masterChart, StiCandlestickSeries series)
        {
            if (series.ValueDataColumnOpen == null || series.ValueDataColumnOpen.Trim().Length <= 0) return null;

            #region DataSource
            var dataSource =
                StiDataColumn.GetDataSourceFromDataColumn(masterChart.Report.Dictionary, series.ValueDataColumnOpen);

            if (dataSource != null)
            {
                dataSource.SaveState("ChartRender_DataColumn");

                var values = new double?[dataSource.Count];
                dataSource.First();

                for (var posIndex = 0; posIndex < dataSource.Count; posIndex++)
                {
                    var data = StiDataColumn.GetDataFromDataColumn(masterChart.Report.Dictionary, series.ValueDataColumnOpen);
                    double value = 0;
                    if (data is DateTime)
                    {
                        series.Core.IsDateTimeValues = true;
                        value = ((DateTime)data).ToOADate();
                    }
                    else value = (double)StiReport.ChangeType(data, typeof(double));

                    values[posIndex] = value;
                    dataSource.Next();
                }
                dataSource.RestoreState("ChartRender_DataColumn");
                return values;
            }
            #endregion

            #region BusinessObject
            var businessObject =
                StiDataColumn.GetBusinessObjectFromDataColumn(masterChart.Report.Dictionary, series.ValueDataColumnOpen);

            if (businessObject != null)
            {
                businessObject.SaveState("ChartRender_DataColumn");
                businessObject.CreateEnumerator();
                businessObject.specTotalsCalculation = true;

                var values = new double?[businessObject.Count];
                businessObject.First();

                for (var posIndex = 0; posIndex < businessObject.Count; posIndex++)
                {
                    var data = StiDataColumn.GetDataFromBusinessObject(masterChart.Report.Dictionary, series.ValueDataColumnOpen);
                    double value = 0;
                    if (data is DateTime)
                    {
                        series.Core.IsDateTimeValues = true;
                        value = ((DateTime)data).ToOADate();
                    }
                    else value = (double)StiReport.ChangeType(data, typeof(double));

                    values[posIndex] = value;
                    businessObject.Next();
                }
                businessObject.RestoreState("ChartRender_DataColumn");
                return values;
            }
            #endregion
            return null;
        }

        private static double?[] GetValuesOpenFromListOfValuesOpen(StiChart masterChart, StiCandlestickSeries series)
        {
            var eg = new StiGetValueEventArgs();
            series.InvokeGetListOfValuesOpen(masterChart, eg);

            if (!string.IsNullOrEmpty(eg.Value))
                return StiSeries.GetNullableValuesFromString(series, eg.Value);

            return null;
        }
        #endregion

        #region GetValueClose
        private static double?[] GetValuesClose(StiChart masterChart, StiCandlestickSeries series)
        {
            var values = GetValuesCloseFromListOfValuesClose(masterChart, series);
            if (values != null) return values;

            values = GetValuesCloseFromValueDataColumnClose(masterChart, series);
            if (values != null) return values;

            values = GetValuesCloseFromValuesCloseExpression(masterChart, series);
            if (values != null) return values;

            return new double?[0];
        }

        private static double?[] GetValuesCloseFromValuesCloseExpression(StiChart masterChart, StiCandlestickSeries series)
        {
            var values = new double?[masterChart.Count];
            masterChart.First();

            for (var posIndex = 0; posIndex < masterChart.Count; posIndex++)
            {
                var eeg = new StiGetValueEventArgs { Value = "0" };

                series.InvokeGetValueClose(masterChart, eeg);

                values[posIndex] = (double)StiReport.ChangeType(eeg.Value, typeof(double));
                masterChart.Next();
            }
            return values;
        }

        private static double?[] GetValuesCloseFromValueDataColumnClose(StiChart masterChart, StiCandlestickSeries series)
        {
            if (series.ValueDataColumnClose == null || series.ValueDataColumnClose.Trim().Length <= 0) return null;

            #region DataSource
            var dataSource =
                StiDataColumn.GetDataSourceFromDataColumn(masterChart.Report.Dictionary, series.ValueDataColumnClose);

            if (dataSource != null)
            {
                dataSource.SaveState("ChartRender_DataColumn");

                var values = new double?[dataSource.Count];
                dataSource.First();

                for (var posIndex = 0; posIndex < dataSource.Count; posIndex++)
                {
                    var data = StiDataColumn.GetDataFromDataColumn(masterChart.Report.Dictionary, series.ValueDataColumnClose);
                    double value = 0;
                    if (data is DateTime)
                    {
                        series.Core.IsDateTimeValues = true;
                        value = ((DateTime)data).ToOADate();
                    }
                    else value = (double)StiReport.ChangeType(data, typeof(double));

                    values[posIndex] = value;
                    dataSource.Next();
                }
                dataSource.RestoreState("ChartRender_DataColumn");
                return values;
            }
            #endregion

            #region BusinessObject
            var businessObject =
                StiDataColumn.GetBusinessObjectFromDataColumn(masterChart.Report.Dictionary, series.ValueDataColumnClose);

            if (businessObject != null)
            {
                businessObject.SaveState("ChartRender_DataColumn");
                businessObject.CreateEnumerator();
                businessObject.specTotalsCalculation = true;

                var values = new double?[businessObject.Count];
                businessObject.First();

                for (var posIndex = 0; posIndex < businessObject.Count; posIndex++)
                {
                    var data = StiDataColumn.GetDataFromBusinessObject(masterChart.Report.Dictionary, series.ValueDataColumnClose);
                    double value = 0;
                    if (data is DateTime)
                    {
                        series.Core.IsDateTimeValues = true;
                        value = ((DateTime)data).ToOADate();
                    }
                    else value = (double)StiReport.ChangeType(data, typeof(double));

                    values[posIndex] = value;
                    businessObject.Next();
                }
                businessObject.RestoreState("ChartRender_DataColumn");
                return values;
            }
            #endregion
            return null;
        }

        private static double?[] GetValuesCloseFromListOfValuesClose(StiChart masterChart, StiCandlestickSeries series)
        {
            var eg = new StiGetValueEventArgs();
            series.InvokeGetListOfValuesClose(masterChart, eg);

            if (!string.IsNullOrEmpty(eg.Value))
                return StiSeries.GetNullableValuesFromString(series, eg.Value);

            return null;
        }
        #endregion

        #region GetValueHigh
        private static double?[] GetValuesHigh(StiChart masterChart, StiCandlestickSeries series)
        {
            var values = GetValuesHighFromListOfValuesHigh(masterChart, series);
            if (values != null) return values;

            values = GetValuesHighFromValueDataColumnHigh(masterChart, series);
            if (values != null) return values;

            values = GetValuesHighFromValuesHighExpression(masterChart, series);
            if (values != null) return values;

            return new double?[0];
        }

        private static double?[] GetValuesHighFromValuesHighExpression(StiChart masterChart, StiCandlestickSeries series)
        {
            var values = new double?[masterChart.Count];
            masterChart.First();

            for (var posIndex = 0; posIndex < masterChart.Count; posIndex++)
            {
                var eeg = new StiGetValueEventArgs { Value = "0" };

                series.InvokeGetValueHigh(masterChart, eeg);

                values[posIndex] = (double)StiReport.ChangeType(eeg.Value, typeof(double));
                masterChart.Next();
            }
            return values;
        }

        private static double?[] GetValuesHighFromValueDataColumnHigh(StiChart masterChart, StiCandlestickSeries series)
        {
            if (series.ValueDataColumnHigh == null || series.ValueDataColumnHigh.Trim().Length <= 0) return null;

            #region DataSource
            var dataSource =
                StiDataColumn.GetDataSourceFromDataColumn(masterChart.Report.Dictionary, series.ValueDataColumnHigh);

            if (dataSource != null)
            {
                dataSource.SaveState("ChartRender_DataColumn");

                var values = new double?[dataSource.Count];
                dataSource.First();

                for (var posIndex = 0; posIndex < dataSource.Count; posIndex++)
                {
                    var data = StiDataColumn.GetDataFromDataColumn(masterChart.Report.Dictionary, series.ValueDataColumnHigh);
                    double value = 0;
                    if (data is DateTime)
                    {
                        series.Core.IsDateTimeValues = true;
                        value = ((DateTime)data).ToOADate();
                    }
                    else value = (double)StiReport.ChangeType(data, typeof(double));

                    values[posIndex] = value;
                    dataSource.Next();
                }
                dataSource.RestoreState("ChartRender_DataColumn");
                return values;
            }
            #endregion

            #region BusinessObject
            var businessObject =
                StiDataColumn.GetBusinessObjectFromDataColumn(masterChart.Report.Dictionary, series.ValueDataColumnHigh);

            if (businessObject != null)
            {
                businessObject.SaveState("ChartRender_DataColumn");
                businessObject.CreateEnumerator();
                businessObject.specTotalsCalculation = true;

                var values = new double?[businessObject.Count];
                businessObject.First();

                for (var posIndex = 0; posIndex < businessObject.Count; posIndex++)
                {
                    var data = StiDataColumn.GetDataFromBusinessObject(masterChart.Report.Dictionary, series.ValueDataColumnHigh);
                    double value = 0;
                    if (data is DateTime)
                    {
                        series.Core.IsDateTimeValues = true;
                        value = ((DateTime)data).ToOADate();
                    }
                    else value = (double)StiReport.ChangeType(data, typeof(double));

                    values[posIndex] = value;
                    businessObject.Next();
                }
                businessObject.RestoreState("ChartRender_DataColumn");
                return values;
            }
            #endregion
            return null;
        }

        private static double?[] GetValuesHighFromListOfValuesHigh(StiChart masterChart, StiCandlestickSeries series)
        {
            var eg = new StiGetValueEventArgs();
            series.InvokeGetListOfValuesHigh(masterChart, eg);

            if (!string.IsNullOrEmpty(eg.Value))
                return StiSeries.GetNullableValuesFromString(series, eg.Value);

            return null;
        }
        #endregion

        #region GetValueLow
        private static double?[] GetValuesLow(StiChart masterChart, StiCandlestickSeries series)
        {
            var values = GetValuesLowFromListOfValuesLow(masterChart, series);
            if (values != null) return values;

            values = GetValuesLowFromValueDataColumnLow(masterChart, series);
            if (values != null) return values;

            values = GetValuesLowFromValuesLowExpression(masterChart, series);
            if (values != null) return values;

            return new double?[0];
        }

        private static double?[] GetValuesLowFromValuesLowExpression(StiChart masterChart, StiCandlestickSeries series)
        {
            var values = new double?[masterChart.Count];
            masterChart.First();

            for (var posIndex = 0; posIndex < masterChart.Count; posIndex++)
            {
                var eeg = new StiGetValueEventArgs { Value = "0" };

                series.InvokeGetValueLow(masterChart, eeg);

                values[posIndex] = (double)StiReport.ChangeType(eeg.Value, typeof(double));
                masterChart.Next();
            }
            return values;
        }

        private static double?[] GetValuesLowFromValueDataColumnLow(StiChart masterChart, StiCandlestickSeries series)
        {
            if (series.ValueDataColumnLow == null || series.ValueDataColumnLow.Trim().Length <= 0) return null;

            #region DataSource
            var dataSource =
                StiDataColumn.GetDataSourceFromDataColumn(masterChart.Report.Dictionary, series.ValueDataColumnLow);

            if (dataSource != null)
            {
                dataSource.SaveState("ChartRender_DataColumn");

                var values = new double?[dataSource.Count];
                dataSource.First();

                for (var posIndex = 0; posIndex < dataSource.Count; posIndex++)
                {
                    var data = StiDataColumn.GetDataFromDataColumn(masterChart.Report.Dictionary, series.ValueDataColumnLow);
                    double value = 0;
                    if (data is DateTime)
                    {
                        series.Core.IsDateTimeValues = true;
                        value = ((DateTime)data).ToOADate();
                    }
                    else value = (double)StiReport.ChangeType(data, typeof(double));

                    values[posIndex] = value;
                    dataSource.Next();
                }
                dataSource.RestoreState("ChartRender_DataColumn");
                return values;
            }
            #endregion

            #region BusinessObject
            var businessObject =
                StiDataColumn.GetBusinessObjectFromDataColumn(masterChart.Report.Dictionary, series.ValueDataColumnLow);

            if (businessObject != null)
            {
                businessObject.SaveState("ChartRender_DataColumn");
                businessObject.CreateEnumerator();
                businessObject.specTotalsCalculation = true;

                var values = new double?[businessObject.Count];
                businessObject.First();

                for (var posIndex = 0; posIndex < businessObject.Count; posIndex++)
                {
                    var data = StiDataColumn.GetDataFromBusinessObject(masterChart.Report.Dictionary, series.ValueDataColumnLow);
                    double value = 0;
                    if (data is DateTime)
                    {
                        series.Core.IsDateTimeValues = true;
                        value = ((DateTime)data).ToOADate();
                    }
                    else value = (double)StiReport.ChangeType(data, typeof(double));

                    values[posIndex] = value;
                    businessObject.Next();
                }
                businessObject.RestoreState("ChartRender_DataColumn");
                return values;
            }
            #endregion
            return null;
        }

        private static double?[] GetValuesLowFromListOfValuesLow(StiChart masterChart, StiCandlestickSeries series)
        {
            var eg = new StiGetValueEventArgs();
            series.InvokeGetListOfValuesLow(masterChart, eg);

            if (!string.IsNullOrEmpty(eg.Value))
                return StiSeries.GetNullableValuesFromString(series, eg.Value);

            return null;
        }
        #endregion

        #region GetWeights
        private static double[] GetWeights(StiChart masterChart, StiBubbleSeries series)
        {
            var values = GetWeightsFromListOfWeights(masterChart, series);
            if (values != null)
                return values;

            values = GetWeightsFromWeightDataColumn(masterChart, series);
            if (values != null)
                return values;

            values = GetWeightsWeightExpression(masterChart, series);
            if (values != null)
                return values;

            return new double[0];
        }

        private static double[] GetWeightsWeightExpression(StiChart masterChart, StiBubbleSeries series)
        {
            var values = new double[masterChart.Count];
            masterChart.First();

            for (var posIndex = 0; posIndex < masterChart.Count; posIndex++)
            {
                var eeg = new StiGetValueEventArgs { Value = "0" };

                series.InvokeGetWeight(masterChart, eeg);

                values[posIndex] = (double)StiReport.ChangeType(eeg.Value, typeof(double));
                masterChart.Next();
            }
            return values;
        }

        private static double[] GetWeightsFromWeightDataColumn(StiChart masterChart, StiBubbleSeries series)
        {
            if (series.WeightDataColumn == null || series.WeightDataColumn.Trim().Length <= 0) return null;

            #region DataSource
            var dataSource =
                StiDataColumn.GetDataSourceFromDataColumn(masterChart.Report.Dictionary, series.WeightDataColumn);

            if (dataSource != null)
            {
                dataSource.SaveState("ChartRender_DataColumn");

                var values = new double[dataSource.Count];
                dataSource.First();

                for (var posIndex = 0; posIndex < dataSource.Count; posIndex++)
                {
                    var data = StiDataColumn.GetDataFromDataColumn(masterChart.Report.Dictionary, series.WeightDataColumn);
                    double value = 0;
                    if (data is DateTime)
                    {
                        series.Core.IsDateTimeValues = true;
                        value = ((DateTime)data).ToOADate();
                    }
                    else value = (double)StiReport.ChangeType(data, typeof(double));

                    values[posIndex] = value;
                    dataSource.Next();
                }
                dataSource.RestoreState("ChartRender_DataColumn");
                return values;
            }
            #endregion

            #region BusinessObject
            var businessObject =
                StiDataColumn.GetBusinessObjectFromDataColumn(masterChart.Report.Dictionary, series.WeightDataColumn);

            if (businessObject != null)
            {
                businessObject.SaveState("ChartRender_DataColumn");
                businessObject.CreateEnumerator();
                businessObject.specTotalsCalculation = true;

                var values = new double[businessObject.Count];
                businessObject.First();

                for (var posIndex = 0; posIndex < businessObject.Count; posIndex++)
                {
                    var data = StiDataColumn.GetDataFromBusinessObject(masterChart.Report.Dictionary, series.WeightDataColumn);
                    double value = 0;
                    if (data is DateTime)
                    {
                        series.Core.IsDateTimeValues = true;
                        value = ((DateTime)data).ToOADate();
                    }
                    else value = (double)StiReport.ChangeType(data, typeof(double));

                    values[posIndex] = value;
                    businessObject.Next();
                }
                businessObject.RestoreState("ChartRender_DataColumn");
                return values;
            }
            #endregion
            return null;
        }

        private static double[] GetWeightsFromListOfWeights(StiChart masterChart, StiBubbleSeries series)
        {
            var eg = new StiGetValueEventArgs();
            series.InvokeGetListOfWeights(masterChart, eg);

            if (!string.IsNullOrEmpty(eg.Value))
                return StiSeries.GetValuesFromString(eg.Value);

            return null;
        }
        #endregion

        #region GetHyperlinks
        private static string[] GetHyperlinks(StiChart masterChart, StiSeries series)
        {
            var values = GetHyperlinksFromListOfHyperlinks(masterChart, series);
            if (values != null)
                return values;

            values = GetHyperlinksFromHyperlinkDataColumn(masterChart, series);
            if (values != null)
                return values;

            values = GetHyperlinksFromHyperlinkExpression(masterChart, series);
            if (values != null)
                return values;

            return new string[0];
        }

        private static string[] GetHyperlinksFromHyperlinkExpression(StiChart masterChart, StiSeries series)
        {
            var values = new string[masterChart.Count];

            masterChart.First();

            for (var posIndex = 0; posIndex < masterChart.Count; posIndex++)
            {
                var ee = new StiValueEventArgs();

                series.InvokeGetHyperlink(masterChart, ee);
                if (ee.Value != null)
                    values[posIndex] = ee.Value.ToString();
                masterChart.Next();
            }

            return values;
        }

        private static string[] GetHyperlinksFromHyperlinkDataColumn(StiChart masterChart, StiSeries series)
        {
            if (series.HyperlinkDataColumn == null || series.HyperlinkDataColumn.Trim().Length <= 0) return null;

            #region DataSource
            var dataSource =
                StiDataColumn.GetDataSourceFromDataColumn(masterChart.Report.Dictionary, series.HyperlinkDataColumn);

            if (dataSource != null)
            {
                dataSource.SaveState("ChartRender_DataColumn");

                var values = new string[dataSource.Count];
                dataSource.First();

                for (var posIndex = 0; posIndex < dataSource.Count; posIndex++)
                {
                    var data = StiDataColumn.GetDataFromDataColumn(masterChart.Report.Dictionary, series.HyperlinkDataColumn);
                    if (data != null)
                        values[posIndex] = data.ToString();

                    dataSource.Next();
                }
                dataSource.RestoreState("ChartRender_DataColumn");
                return values;
            }
            #endregion

            #region Business Object
            var businessObject =
                StiDataColumn.GetBusinessObjectFromDataColumn(masterChart.Report.Dictionary, series.HyperlinkDataColumn);

            if (businessObject != null)
            {
                businessObject.SaveState("ChartRender_DataColumn");
                businessObject.CreateEnumerator();
                businessObject.specTotalsCalculation = true;

                var values = new string[businessObject.Count];
                businessObject.First();

                for (var posIndex = 0; posIndex < businessObject.Count; posIndex++)
                {
                    var data = StiDataColumn.GetDataFromBusinessObject(masterChart.Report.Dictionary, series.HyperlinkDataColumn);
                    if (data != null)
                        values[posIndex] = data.ToString();

                    businessObject.Next();
                }
                businessObject.RestoreState("ChartRender_DataColumn");
                return values;
            }
            #endregion
            return null;
        }

        private static string[] GetHyperlinksFromListOfHyperlinks(StiChart masterChart, StiSeries series)
        {
            var e = new StiGetValueEventArgs();
            series.InvokeGetListOfHyperlinks(masterChart, e);

            if (!string.IsNullOrEmpty(e.Value))
                return StiSeries.GetStringsFromString(e.Value);

            return null;
        }
        #endregion

        #region GetTags
        private static object[] GetTags(StiChart masterChart, StiSeries series)
        {
            var values = GetTagsFromListOfTags(masterChart, series);
            if (values != null)
                return values;

            values = GetTagsFromTagDataColumn(masterChart, series);
            if (values != null)
                return values;

            values = GetTagsFromTagExpression(masterChart, series);
            if (values != null)
                return values;

            return new object[0];
        }

        private static object[] GetTagsFromTagExpression(StiChart masterChart, StiSeries series)
        {
            var values = new object[masterChart.Count];

            masterChart.First();

            for (var posIndex = 0; posIndex < masterChart.Count; posIndex++)
            {
                var ee = new StiValueEventArgs();

                series.InvokeGetTag(masterChart, ee);
                values[posIndex] = ee.Value;
                masterChart.Next();
            }

            return values;
        }

        private static object[] GetTagsFromTagDataColumn(StiChart masterChart, StiSeries series)
        {
            if (series.TagDataColumn == null || series.TagDataColumn.Trim().Length <= 0) return null;

            #region DataSource
            var dataSource =
                StiDataColumn.GetDataSourceFromDataColumn(masterChart.Report.Dictionary, series.TagDataColumn);

            if (dataSource != null)
            {
                dataSource.SaveState("ChartRender_DataColumn");

                var values = new object[dataSource.Count];
                dataSource.First();

                for (var posIndex = 0; posIndex < dataSource.Count; posIndex++)
                {
                    var data = StiDataColumn.GetDataFromDataColumn(masterChart.Report.Dictionary, series.TagDataColumn);
                    values[posIndex] = data;
                    dataSource.Next();
                }
                dataSource.RestoreState("ChartRender_DataColumn");
                return values;
            }
            #endregion

            #region Business Object
            var businessObject =
                StiDataColumn.GetBusinessObjectFromDataColumn(masterChart.Report.Dictionary, series.TagDataColumn);

            if (businessObject != null)
            {
                businessObject.SaveState("ChartRender_DataColumn");
                businessObject.CreateEnumerator();
                businessObject.specTotalsCalculation = true;

                var values = new object[businessObject.Count];
                businessObject.First();

                for (var posIndex = 0; posIndex < businessObject.Count; posIndex++)
                {
                    values[posIndex] = StiDataColumn.GetDataFromBusinessObject(masterChart.Report.Dictionary, series.TagDataColumn);
                    businessObject.Next();
                }
                businessObject.RestoreState("ChartRender_DataColumn");
                return values;
            }
            #endregion
            return null;
        }

        private static object[] GetTagsFromListOfTags(StiChart masterChart, StiSeries series)
        {
            var e = new StiGetValueEventArgs();
            series.InvokeGetListOfTags(masterChart, e);

            if (!string.IsNullOrEmpty(e.Value))
                return StiSeries.GetStringsFromString(e.Value);

            return null;
        }
        #endregion

        #region GetToolTip
        private static string[] GetToolTips(StiChart masterChart, StiSeries series)
        {
            var values = GetToolTipsFromListOfToolTips(masterChart, series);
            if (values != null)
                return values;

            values = GetToolTipsFromToolTipDataColumn(masterChart, series);
            if (values != null)
                return values;

            values = GetToolTipsFromToolTipExpression(masterChart, series);
            if (values != null)
                return values;

            return new string[0];
        }

        private static string[] GetToolTipsFromToolTipExpression(StiChart masterChart, StiSeries series)
        {
            var values = new string[masterChart.Count];

            masterChart.First();

            for (var posIndex = 0; posIndex < masterChart.Count; posIndex++)
            {
                var ee = new StiValueEventArgs();

                series.InvokeGetToolTip(masterChart, ee);
                if (ee.Value != null)
                    values[posIndex] = ee.Value.ToString();

                masterChart.Next();
            }

            return values;
        }

        private static string[] GetToolTipsFromToolTipDataColumn(StiChart masterChart, StiSeries series)
        {
            if (series.ToolTipDataColumn == null || series.ToolTipDataColumn.Trim().Length <= 0) return null;

            #region DataSource
            var dataSource =
                StiDataColumn.GetDataSourceFromDataColumn(masterChart.Report.Dictionary, series.ToolTipDataColumn);

            if (dataSource != null)
            {
                dataSource.SaveState("ChartRender_DataColumn");

                var values = new string[dataSource.Count];
                dataSource.First();

                for (var posIndex = 0; posIndex < dataSource.Count; posIndex++)
                {
                    var data = StiDataColumn.GetDataFromDataColumn(masterChart.Report.Dictionary, series.ToolTipDataColumn);
                    if (data != null)
                        values[posIndex] = data.ToString();
                    dataSource.Next();
                }
                dataSource.RestoreState("ChartRender_DataColumn");
                return values;
            }
            #endregion

            #region Business Object
            var businessObject =
                StiDataColumn.GetBusinessObjectFromDataColumn(masterChart.Report.Dictionary, series.ToolTipDataColumn);

            if (businessObject != null)
            {
                businessObject.SaveState("ChartRender_DataColumn");
                businessObject.CreateEnumerator();
                businessObject.specTotalsCalculation = true;

                var values = new string[businessObject.Count];
                businessObject.First();

                for (var posIndex = 0; posIndex < businessObject.Count; posIndex++)
                {
                    var data = StiDataColumn.GetDataFromBusinessObject(masterChart.Report.Dictionary, series.ToolTipDataColumn);
                    if (data != null)
                        values[posIndex] = data.ToString();

                    businessObject.Next();
                }
                businessObject.RestoreState("ChartRender_DataColumn");
                return values;
            }
            #endregion
            return null;
        }

        private static string[] GetToolTipsFromListOfToolTips(StiChart masterChart, StiSeries series)
        {
            var e = new StiGetValueEventArgs();
            series.InvokeGetListOfToolTips(masterChart, e);

            if (!string.IsNullOrEmpty(e.Value))
                return StiSeries.GetStringsFromString(e.Value);

            return null;
        }
        #endregion

        #region GetAnimationСompatibilitySeries
        public static bool GetAnimationСompatibilitySeries(IStiSeries series1, IStiSeries series2)
        {
            var type1 = GetAnimationSeriesType(series1);
            var type2 = GetAnimationSeriesType(series2);
            if (type1 == StiSeriesAnimationType.None || type2 == StiSeriesAnimationType.None) return false;

            return type1 == type2;
        }

        private static StiSeriesAnimationType GetAnimationSeriesType(IStiSeries series)
        {
            var seriesType = series.GetType();
            #region Column
            if (seriesType == typeof(StiClusteredColumnSeries)) return StiSeriesAnimationType.Column;
            if (seriesType == typeof(StiStackedColumnSeries)) return StiSeriesAnimationType.Column;
            if (seriesType == typeof(StiFullStackedColumnSeries)) return StiSeriesAnimationType.Column;

            if (seriesType == typeof(StiRangeBarSeries)) return StiSeriesAnimationType.Column;
            #endregion

            #region Line
            if (seriesType == typeof(StiLineSeries)) return StiSeriesAnimationType.Line;
            if (seriesType == typeof(StiStackedLineSeries)) return StiSeriesAnimationType.Line;
            if (seriesType == typeof(StiFullStackedLineSeries)) return StiSeriesAnimationType.Line;

            if (seriesType == typeof(StiSplineSeries)) return StiSeriesAnimationType.Line;
            if (seriesType == typeof(StiStackedSplineSeries)) return StiSeriesAnimationType.Line;
            if (seriesType == typeof(StiFullStackedSplineSeries)) return StiSeriesAnimationType.Line;

            if (seriesType == typeof(StiSteppedLineSeries)) return StiSeriesAnimationType.Line;

            if (seriesType == typeof(StiScatterLineSeries)) return StiSeriesAnimationType.Line;
            if (seriesType == typeof(StiScatterSplineSeries)) return StiSeriesAnimationType.Line;
            #endregion

            #region Bar
            if (seriesType == typeof(StiClusteredBarSeries)) return StiSeriesAnimationType.Bar;
            if (seriesType == typeof(StiStackedBarSeries)) return StiSeriesAnimationType.Bar;
            if (seriesType == typeof(StiFullStackedBarSeries)) return StiSeriesAnimationType.Bar;

            if (seriesType == typeof(StiGanttSeries)) return StiSeriesAnimationType.Bar;
            #endregion

            #region Range
            if (seriesType == typeof(StiAreaSeries)) return StiSeriesAnimationType.Range;
            if (seriesType == typeof(StiStackedAreaSeries)) return StiSeriesAnimationType.Range;
            if (seriesType == typeof(StiFullStackedAreaSeries)) return StiSeriesAnimationType.Range;

            if (seriesType == typeof(StiSplineAreaSeries)) return StiSeriesAnimationType.Range;
            if (seriesType == typeof(StiStackedSplineAreaSeries)) return StiSeriesAnimationType.Range;
            if (seriesType == typeof(StiFullStackedSplineAreaSeries)) return StiSeriesAnimationType.Range;

            if (seriesType == typeof(StiSteppedAreaSeries)) return StiSeriesAnimationType.Range;

            if (seriesType == typeof(StiRangeSeries)) return StiSeriesAnimationType.Range;
            if (seriesType == typeof(StiSplineRangeSeries)) return StiSeriesAnimationType.Range;
            if (seriesType == typeof(StiSteppedRangeSeries)) return StiSeriesAnimationType.Range;
            #endregion

            return StiSeriesAnimationType.None;
        }
        #endregion
    }
}