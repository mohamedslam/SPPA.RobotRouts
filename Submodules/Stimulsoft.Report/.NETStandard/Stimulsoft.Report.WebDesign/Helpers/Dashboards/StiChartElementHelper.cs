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

using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Meters;
using Stimulsoft.Data.Helpers;
using Stimulsoft.Report.Chart;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Dictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Stimulsoft.Report.Web.Helpers.Dashboards
{
    internal class StiChartElementHelper
    {
        #region Fields
        private IStiChartElement chartElement;
        #endregion

        #region Helper Methods
        private Hashtable GetChartElementJSProperties(Hashtable parameters)
        {
            Hashtable properties = StiReportEdit.GetAllProperties(chartElement as StiComponent);
            properties["meters"] = GetMetersHash();
            properties["manuallyEnteredData"] = StiManuallyDataHelper.ConvertPackedStringToJSData(chartElement.ManuallyEnteredData);
            properties["manuallyEnteredSeriesType"] = StiInvokeMethodsHelper.GetPropertyValue(chartElement.GetManuallyEnteredChartMeter(), "SeriesType");

            if ((string)parameters["command"] != "GetChartElementProperties")
            {
                properties["svgContent"] = StiEncodingHelper.Encode(StiDashboardsSvgHelper.SaveElementToString(chartElement, 1, 1, true));
            }

            return properties;
        }

        private Hashtable GetMeterHashItem(IStiMeter meter)
        {
            Hashtable meterItem = new Hashtable();
            meterItem["typeItem"] = "Meter";
            meterItem["type"] = StiTableElementHelper.GetMeterType(meter);
            meterItem["typeIcon"] = StiTableElementHelper.GetMeterTypeIcon(meter);
            meterItem["label"] = StiTableElementHelper.GetMeterLabel(meter);
            meterItem["expression"] = meter.Expression != null ? StiEncodingHelper.Encode(meter.Expression) : string.Empty;
            meterItem["functions"] = StiTableElementHelper.GetMeterFunctions(meter, chartElement.Page as IStiDashboard);
            meterItem["currentFunction"] = StiExpressionHelper.GetFunction(meter.Expression);

            return meterItem;
        }

        private Hashtable GetMetersHash()
        {
            var meters = chartElement.FetchAllMeters();
            var metersItems = new Hashtable();
            metersItems["values"] = new ArrayList();
            metersItems["endValues"] = new ArrayList();
            metersItems["closeValues"] = new ArrayList();
            metersItems["lowValues"] = new ArrayList();
            metersItems["highValues"] = new ArrayList();
            metersItems["arguments"] = new ArrayList();
            metersItems["weights"] = new ArrayList();
            metersItems["series"] = new ArrayList();

            foreach (IStiMeter meter in meters)
            {
                switch (meter.GetType().Name)
                {
                    case "StiValueChartMeter":
                        {
                            var valueMeterItem = GetMeterHashItem(meter);
                            var seriesType = StiInvokeMethodsHelper.GetPropertyValue(meter, "SeriesType");

                            if (seriesType != null)
                            {
                                valueMeterItem["seriesType"] = ((StiChartSeriesType)seriesType).ToString();
                                valueMeterItem["showEmptyValuesInSimpleWay"] = ShowEmptyValuesInSimpleWay(valueMeterItem["seriesType"] as string);

                                if (chartElement.IsLinesChart)
                                {
                                    valueMeterItem["lineWidth"] = StiInvokeMethodsHelper.GetPropertyValue(meter, "LineWidth");
                                    valueMeterItem["lineStyle"] = StiInvokeMethodsHelper.GetPropertyValue(meter, "LineStyle");
                                }

                                if (IsShowZeros(valueMeterItem["seriesType"] as string))
                                    valueMeterItem["showZeros"] = StiInvokeMethodsHelper.GetPropertyValue(meter, "ShowZeros");

                                if (IsShowNulls(valueMeterItem["seriesType"] as string))
                                    valueMeterItem["showNulls"] = StiInvokeMethodsHelper.GetPropertyValue(meter, "ShowNulls");

                                var yRightAxis = StiInvokeMethodsHelper.GetPropertyValue(chartElement, "YRightAxis");
                                var yRightAxisVisible = StiInvokeMethodsHelper.GetPropertyValue(yRightAxis, "Visible");

                                if (Convert.ToBoolean(yRightAxisVisible) && chartElement.IsAxisAreaChart)
                                    valueMeterItem["yAxis"] = StiInvokeMethodsHelper.GetPropertyValue(meter, "YAxis");
                            }
                                
                            ((ArrayList)metersItems["values"]).Add(valueMeterItem);
                            break;
                        }
                    case "StiEndValueChartMeter":
                        {
                            ((ArrayList)metersItems["endValues"]).Add(GetMeterHashItem(meter));
                            break;
                        }
                    case "StiCloseValueChartMeter":
                        {
                            ((ArrayList)metersItems["closeValues"]).Add(GetMeterHashItem(meter));
                            break;
                        }
                    case "StiLowValueChartMeter":
                        {
                            ((ArrayList)metersItems["lowValues"]).Add(GetMeterHashItem(meter));
                            break;
                        }
                    case "StiHighValueChartMeter":
                        {
                            ((ArrayList)metersItems["highValues"]).Add(GetMeterHashItem(meter));
                            break;
                        }
                    case "StiArgumentChartMeter":
                        {
                            ((ArrayList)metersItems["arguments"]).Add(GetMeterHashItem(meter));
                            break;
                        }
                    case "StiWeightChartMeter":
                        {
                            ((ArrayList)metersItems["weights"]).Add(GetMeterHashItem(meter));
                            break;
                        }
                    case "StiSeriesChartMeter":
                        {
                            ((ArrayList)metersItems["series"]).Add(GetMeterHashItem(meter));
                            break;
                        }
                }
            }

            return metersItems;
        }

        internal static ArrayList GetUserViewStates(IStiChartElement chartElement)
        {
            var viewStates = new ArrayList();

            (chartElement as IStiUserViewStates).UserViewStates.ForEach(viewState =>
            {
                viewStates.Add(
                    new Hashtable()
                    {
                        ["name"] = viewState.Name,
                        ["key"] = viewState.Key,
                        ["seriesType"] = viewState.SeriesType
                    });
            });

            return viewStates;
        }

        private void SetPropertySeriesType(IStiMeter meter, string seriesType)
        {
            if (meter != null)
            {
                var property = meter.GetType().GetProperty("SeriesType");
                if (property != null) property.SetValue(meter, Enum.Parse(property.PropertyType, seriesType), null);
            }
        }

        private bool ShowEmptyValuesInSimpleWay(string seriesType)
        {
            return !(StiChartSeriesCreator.New(seriesType) is IStiBaseLineSeries);
        }

        private bool IsShowZeros(string seriesType)
        {
            return StiChartSeriesCreator.New(seriesType) is IStiShowZerosSeries;
        }

        private bool IsShowNulls(string seriesType)
        {
            return StiChartSeriesCreator.New(seriesType) is IStiShowNullsSeries;
        }
        #endregion

        #region Methods
        public void ExecuteJSCommand(Hashtable parameters, Hashtable callbackResult)
        {
            switch ((string)parameters["command"])
            {

                case "InsertMeters":
                    {
                        InsertMeters(parameters, callbackResult);
                        break;
                    }
                case "RemoveMeter":
                    {
                        RemoveMeter(parameters, callbackResult);
                        break;
                    }
                case "RenameMeter":
                    {
                        RenameMeter(parameters, callbackResult);
                        break;
                    }
                case "RemoveAllMeters":
                    {
                        RemoveAllMeters(parameters, callbackResult);
                        break;
                    }
                case "MoveMeter":
                case "MoveAndDuplicateMeter":
                    {
                        MoveMeter(parameters, callbackResult);
                        break;
                    }
                case "DuplicateMeter":
                    {
                        DuplicateMeter(parameters, callbackResult);
                        break;
                    }
                case "SetExpression":
                    {
                        SetExpression(parameters, callbackResult);
                        break;
                    }
                case "SetFunction":
                    {
                        SetFunction(parameters, callbackResult);
                        break;
                    }
                case "SetSeriesType":
                    {
                        SetSeriesType(parameters, callbackResult);
                        break;
                    }
                case "NewItem":
                    {
                        CreateNewItem(parameters, callbackResult);
                        break;
                    }
                case "GetChartElementProperties":
                    {
                        break;
                    }
                case "SetPropertyValue":
                    {
                        SetPropertyValue(parameters, callbackResult);
                        break;
                    }
                case "SetPropertyValueToValueMeter":
                    {
                        SetPropertyValueToValueMeter(parameters, callbackResult);
                        break;
                    }
                case "AddConstantLine":
                    {
                        AddConstantLine(parameters, callbackResult);
                        break;
                    }
                case "RemoveConstantLine":
                    {
                        RemoveConstantLine(parameters, callbackResult);
                        break;
                    }
                case "MoveConstantLine":
                    {
                        MoveConstantLine(parameters, callbackResult);
                        break;
                    }
                case "SetUserViewState":
                    {
                        SetUserViewState(parameters, callbackResult);
                        break;
                    }
                case "RemoveUserViewState":
                    {
                        RemoveUserViewState(parameters, callbackResult);
                        break;
                    }
                case "SetValueToManuallyEnteredData":
                    {
                        SetValueToManuallyEnteredData(parameters, callbackResult);
                        break;
                    }
            }

            callbackResult["elementProperties"] = GetChartElementJSProperties(parameters);
        }

        private void SetPropertyValue(Hashtable parameters, Hashtable callbackResult)
        {
            var propertyName = parameters["propertyName"] as string;
            var propertyValue = parameters["propertyValue"] as string;

            switch (propertyName)
            {
                case "Icon":
                    chartElement.Icon = !string.IsNullOrEmpty(propertyValue) ? (Report.Helpers.StiFontIcons?)Enum.Parse(typeof(Report.Helpers.StiFontIcons), propertyValue) : null;
                    break;

                case "ColumnShape":
                    chartElement.ColumnShape = (StiColumnShape3D)Enum.Parse(typeof(StiColumnShape3D), propertyValue);
                    break;

                case "RoundValues":
                    chartElement.RoundValues = Convert.ToBoolean(parameters["propertyValue"]);
                    break;

                case "UserViewState":
                    var elementViewStates = chartElement as IStiUserViewStates;
                    var currentViewState = elementViewStates.UserViewStates.FirstOrDefault(viewState => viewState.Key == elementViewStates.SelectedViewStateKey);
                    if (currentViewState != null)
                        currentViewState.Name = propertyValue;
                    break;

                case "DataMode":
                    {
                        chartElement.DataMode = (StiDataMode)Enum.Parse(typeof(StiDataMode), propertyValue);
                        break;
                    }
            }
        }

        private void SetPropertyValueToValueMeter(Hashtable parameters, Hashtable callbackResult)
        {
            var propertyName = parameters["propertyName"] as string;
            var propertyValue = parameters["propertyValue"] as string;
            var valueMeter = chartElement.GetValueByIndex(Convert.ToInt32(parameters["itemIndex"]));

            if (valueMeter != null)
            {
                var property = valueMeter.GetType().GetProperty(propertyName);
                if (property != null)
                {
                    switch (propertyName)
                    {
                        case "LineWidth":
                            {
                                property.SetValue(valueMeter, (float)Convert.ToDouble(propertyValue), null);
                                break;
                            }
                        case "LineStyle":
                            {
                                property.SetValue(valueMeter, Enum.Parse(typeof(StiPenStyle), propertyValue));
                                break;
                            }
                        case "ShowZeros":
                            {
                                property.SetValue(valueMeter, Enum.Parse(typeof(StiEmptyCellsAs), propertyValue));
                                break;
                            }
                        case "ShowNulls":
                            {
                                property.SetValue(valueMeter, Enum.Parse(typeof(StiEmptyCellsAs), propertyValue));
                                break;
                            }
                        case "YAxis":
                            {
                                property.SetValue(valueMeter, Enum.Parse(typeof(StiSeriesYAxis), propertyValue));
                                break;
                            }
                    }
                }
            }
        }

        private IStiMeter GetMeterFromContainer(string containerName, int index)
        {
            switch (containerName)
            {
                case "values": return chartElement.GetValueByIndex(index);
                case "endValues": return chartElement.GetEndValueByIndex(index);
                case "closeValues": return chartElement.GetCloseValueByIndex(index);
                case "lowValues": return chartElement.GetLowValueByIndex(index);
                case "highValues": return chartElement.GetHighValueByIndex(index);
                case "arguments": return chartElement.GetArgumentByIndex(index);
                case "weights": return chartElement.GetWeightByIndex(index);
                case "series": return chartElement.GetSeries();
            }

            return null;
        }

        private void SetExpression(Hashtable parameters, Hashtable callbackResult)
        {
            var meter = GetMeterFromContainer(parameters["containerName"] as string, Convert.ToInt32(parameters["itemIndex"]));
            if (meter != null)
            {
                meter.Expression = StiEncodingHelper.DecodeString(parameters["expressionValue"] as string);
            }
        }

        private void RenameMeter(Hashtable parameters, Hashtable callbackResult)
        {
            var meter = GetMeterFromContainer(parameters["containerName"] as string, Convert.ToInt32(parameters["itemIndex"]));
            if (meter != null && parameters["newLabel"] != null)
            {
                StiElementChangedProcessor.ProcessElementChanging(chartElement, StiElementChangedArgs.CreateRenamingArgs(meter.Label, parameters["newLabel"] as string));
                meter.Label = parameters["newLabel"] as string;
            }
        }

        private void SetFunction(Hashtable parameters, Hashtable callbackResult)
        {
            var meter = GetMeterFromContainer(parameters["containerName"] as string, Convert.ToInt32(parameters["itemIndex"]));
            if (meter != null)
            {
                meter.Expression = StiExpressionHelper.ReplaceFunction(meter.Expression, parameters["function"] as string);
            }
        }

        private void SetValueToManuallyEnteredData(Hashtable parameters, Hashtable callbackResult)
        {
            chartElement.ManuallyEnteredData = StiManuallyDataHelper.ConvertJSDataToPackedString(parameters["propertyValue"] as string);
        }

        private void RemoveMeter(Hashtable parameters, Hashtable callbackResult)
        {
            var containerName = parameters["containerName"] as string;
            var itemIndex = Convert.ToInt32(parameters["itemIndex"]);
            var oldLabel = string.Empty;
            switch (containerName)
            {
                case "values":
                    {
                        oldLabel = chartElement.GetValueByIndex(itemIndex)?.Label;
                        chartElement.RemoveValue(itemIndex);
                        break;
                    }
                case "endValues":
                    {
                        oldLabel = chartElement.GetEndValueByIndex(itemIndex)?.Label;
                        chartElement.RemoveEndValue(itemIndex);
                        break;
                    }
                case "closeValues":
                    {
                        oldLabel = chartElement.GetCloseValueByIndex(itemIndex)?.Label;
                        chartElement.RemoveCloseValue(itemIndex);
                        break;
                    }
                case "lowValues":
                    {
                        oldLabel = chartElement.GetLowValueByIndex(itemIndex)?.Label;
                        chartElement.RemoveLowValue(itemIndex);
                        break;
                    }
                case "highValues":
                    {
                        oldLabel = chartElement.GetHighValueByIndex(itemIndex)?.Label;
                        chartElement.RemoveHighValue(itemIndex);
                        break;
                    }
                case "arguments":
                    {
                        oldLabel = chartElement.GetArgumentByIndex(itemIndex)?.Label;
                        chartElement.RemoveArgument(itemIndex);
                        break;
                    }

                case "weights":
                    {
                        oldLabel = chartElement.GetWeightByIndex(itemIndex)?.Label;
                        chartElement.RemoveWeight(itemIndex);
                        break;
                    }

                case "series":
                    {
                        oldLabel = chartElement.GetSeries()?.Label;
                        chartElement.RemoveSeries();
                        break;
                    }
            }

            if (!String.IsNullOrEmpty(oldLabel))
                StiElementChangedProcessor.ProcessElementChanging(chartElement, StiElementChangedArgs.CreateDeletingArgs(oldLabel));
        }

        private void RemoveAllMeters(Hashtable parameters, Hashtable callbackResult)
        {
            var containerName = parameters["containerName"] as string;

            switch (containerName)
            {
                case "values":
                    {
                        chartElement.RemoveAllValues();
                        break;
                    }
                case "endValues":
                    {
                        chartElement.RemoveAllEndValues();
                        break;
                    }
                case "closeValues":
                    {
                        chartElement.RemoveAllCloseValues();
                        break;
                    }
                case "lowValues":
                    {
                        chartElement.RemoveAllLowValues();
                        break;
                    }
                case "highValues":
                    {
                        chartElement.RemoveAllHighValues();
                        break;
                    }
                case "arguments":
                    {
                        chartElement.RemoveAllArguments();
                        break;
                    }
                case "weights":
                    {
                        chartElement.RemoveAllWeights();
                        break;
                    }
            }

            StiElementChangedProcessor.ProcessElementChanging(chartElement, StiElementChangedArgs.CreateClearingAllArgs());
        }

        private void MoveMeter(Hashtable parameters, Hashtable callbackResult)
        {
            var duplicateMeter = (string)parameters["command"] == "MoveAndDuplicateMeter";
            var fromIndex = Convert.ToInt32(parameters["fromIndex"]);
            var toIndex = Convert.ToInt32(parameters["toIndex"]);
            var toContainerName = parameters["toContainerName"] as string;
            var fromContainerName = parameters["fromContainerName"] as string;
            IStiMeter movingMeter = null;

            #region Get and Remove meter
            switch (fromContainerName)
            {
                case "values":
                    {
                        movingMeter = chartElement.GetValueByIndex(fromIndex);
                        if (movingMeter != null && !duplicateMeter)
                            chartElement.RemoveValue(fromIndex);
                        break;
                    }
                case "endValues":
                    {
                        movingMeter = chartElement.GetEndValueByIndex(fromIndex);
                        if (movingMeter != null && !duplicateMeter)
                            chartElement.RemoveEndValue(fromIndex);
                        break;
                    }
                case "closeValues":
                    {
                        movingMeter = chartElement.GetCloseValueByIndex(fromIndex);
                        if (movingMeter != null && !duplicateMeter)
                            chartElement.RemoveCloseValue(fromIndex);
                        break;
                    }
                case "lowValues":
                    {
                        movingMeter = chartElement.GetLowValueByIndex(fromIndex);
                        if (movingMeter != null && !duplicateMeter)
                            chartElement.RemoveLowValue(fromIndex);
                        break;
                    }
                case "highValues":
                    {
                        movingMeter = chartElement.GetHighValueByIndex(fromIndex);
                        if (movingMeter != null && !duplicateMeter)
                            chartElement.RemoveHighValue(fromIndex);
                        break;
                    }
                case "arguments":
                    {
                        movingMeter = chartElement.GetArgumentByIndex(fromIndex);
                        if (movingMeter != null && !duplicateMeter)
                            chartElement.RemoveArgument(fromIndex);
                        break;
                    }

                case "weights":
                    {
                        movingMeter = chartElement.GetWeightByIndex(fromIndex);
                        if (movingMeter != null && !duplicateMeter)
                            chartElement.RemoveWeight(fromIndex);
                        break;
                    }
                case "series":
                    {
                        movingMeter = chartElement.GetSeries();
                        if (movingMeter != null && !duplicateMeter)
                            chartElement.RemoveSeries();
                        break;
                    }
            }
            #endregion

            #region Insert meter
            if (duplicateMeter)
            {
                var cloneMeter = (movingMeter as ICloneable).Clone();
                movingMeter = cloneMeter as IStiMeter;
            }

            if (movingMeter != null)
            {
                switch (toContainerName)
                {
                    case "values":
                        {
                            var valueMeter = chartElement.GetValue(movingMeter);
                            if (valueMeter != null)
                            {
                                if (parameters["oldSeriesType"] != null)
                                {
                                    SetPropertySeriesType(valueMeter, parameters["oldSeriesType"] as string);
                                }
                                chartElement.InsertValue(toIndex, valueMeter);
                            }
                            break;
                        }
                    case "closeValues":
                        {
                            var closeValueMeter = chartElement.GetCloseValue(movingMeter);
                            if (closeValueMeter != null)
                                chartElement.InsertCloseValue(toIndex, closeValueMeter);
                            break;
                        }
                    case "lowValues":
                        {
                            var lowValueMeter = chartElement.GetLowValue(movingMeter);
                            if (lowValueMeter != null)
                                chartElement.InsertLowValue(toIndex, lowValueMeter);
                            break;
                        }
                    case "highValues":
                        {
                            var highValueMeter = chartElement.GetHighValue(movingMeter);
                            if (highValueMeter != null)
                                chartElement.InsertHighValue(toIndex, highValueMeter);
                            break;
                        }
                    case "endValues":
                        {
                            var endValueMeter = chartElement.GetEndValue(movingMeter);
                            if (endValueMeter != null)
                                chartElement.InsertEndValue(toIndex, endValueMeter);
                            break;
                        }
                    case "arguments":
                        {
                            var argumentsMeter = chartElement.GetArgument(movingMeter);
                            if (argumentsMeter != null)
                                chartElement.InsertArgument(toIndex, argumentsMeter);
                            break;
                        }
                    case "weights":
                        {
                            var weightsMeter = chartElement.GetWeight(movingMeter);
                            if (weightsMeter != null)
                                chartElement.InsertWeight(toIndex, weightsMeter);
                            break;
                        }
                    case "series":
                        {
                            var seriesMeter = chartElement.GetSeries(movingMeter);
                            if (seriesMeter != null)
                                chartElement.InsertSeries(seriesMeter);
                            break;
                        }
                }
            }
            #endregion
        }

        private void DuplicateMeter(Hashtable parameters, Hashtable callbackResult)
        {
            var containerName = parameters["containerName"] as string;
            var itemIndex = Convert.ToInt32(parameters["itemIndex"]);
            callbackResult["insertIndex"] = itemIndex + 1;

            switch (containerName)
            {
                case "values":
                    {
                        var meter = chartElement.GetValueByIndex(itemIndex);
                        if (meter != null)
                        {
                            var cloneValue = (meter as ICloneable).Clone();
                            chartElement.InsertValue(itemIndex + 1, cloneValue as IStiMeter);
                        }
                        break;
                    }
                case "endValues":
                    {
                        var meter = chartElement.GetEndValueByIndex(itemIndex);
                        if (meter != null)
                        {
                            var cloneValue = (meter as ICloneable).Clone();
                            chartElement.InsertEndValue(itemIndex + 1, cloneValue as IStiMeter);
                        }
                        break;
                    }
                case "closeValues":
                    {
                        var meter = chartElement.GetCloseValueByIndex(itemIndex);
                        if (meter != null)
                        {
                            var cloneValue = (meter as ICloneable).Clone();
                            chartElement.InsertCloseValue(itemIndex + 1, cloneValue as IStiMeter);
                        }
                        break;
                    }
                case "lowValues":
                    {
                        var meter = chartElement.GetLowValueByIndex(itemIndex);
                        if (meter != null)
                        {
                            var cloneValue = (meter as ICloneable).Clone();
                            chartElement.InsertLowValue(itemIndex + 1, cloneValue as IStiMeter);
                        }
                        break;
                    }
                case "highValues":
                    {
                        var meter = chartElement.GetHighValueByIndex(itemIndex);
                        if (meter != null)
                        {
                            var cloneValue = (meter as ICloneable).Clone();
                            chartElement.InsertHighValue(itemIndex + 1, cloneValue as IStiMeter);
                        }
                        break;
                    }
                case "arguments":
                    {
                        var meter = chartElement.GetArgumentByIndex(itemIndex);
                        if (meter != null)
                        {
                            var cloneArgument = (meter as ICloneable).Clone();
                            chartElement.InsertArgument(itemIndex + 1, cloneArgument as IStiMeter);
                        }
                        break;
                    }

                case "weights":
                    {
                        var meter = chartElement.GetWeightByIndex(itemIndex);
                        if (meter != null)
                        {
                            var cloneWeight = (meter as ICloneable).Clone();
                            chartElement.InsertWeight(itemIndex + 1, cloneWeight as IStiMeter);
                        }
                        break;
                    }
            }
        }

        private void InsertMeters(Hashtable parameters, Hashtable callbackResult)
        {
            var containerName = parameters["containerName"] as string;
            var draggedItem = parameters["draggedItem"] as Hashtable;
            var draggedItemObject = draggedItem["itemObject"] as Hashtable;
            var typeDraggedItem = draggedItemObject["typeItem"] as string;
            var draggedColumns = new List<IStiAppDataCell>();
            var insertIndex = parameters["insertIndex"] != null ? Convert.ToInt32(parameters["insertIndex"]) : -1;
            var oldSeriesType = parameters["oldSeriesType"] as string;

            if (typeDraggedItem == "Variable")
            {
                var variableDataCell = chartElement.Report.Dictionary.Variables[draggedItemObject["name"] as string] as IStiAppDataCell;
                if (variableDataCell != null) draggedColumns.Add(variableDataCell);
            }
            else
            {
                var allColumns = StiDictionaryHelper.GetColumnsByTypeAndNameOfObject(chartElement.Report, draggedItem);
                if (allColumns != null)
                {
                    if (typeDraggedItem == "Column")
                    {
                        var draggedColumn = allColumns[draggedItemObject["name"] as string];
                        if (draggedColumn != null) draggedColumns.Add(draggedColumn);
                    }
                    else
                    {
                        draggedColumns.AddRange(allColumns.Cast<IStiAppDataCell>());
                    }
                }
            }

            foreach (IStiAppDataCell dataColumn in draggedColumns)
            {
                switch (containerName)
                {
                    case "values":
                        {
                            
                            var meter = chartElement.GetValue(dataColumn);

                            if (oldSeriesType == "Gantt")
                                meter = chartElement.GetGanttStartValue(dataColumn);
                            else if (oldSeriesType == "Bubble")
                                meter = chartElement.GetY(dataColumn);
                            
                            if (meter != null)
                            {
                                if (oldSeriesType != null)
                                    SetPropertySeriesType(meter, oldSeriesType);

                                chartElement.InsertValue(insertIndex, meter);
                            }
                            break;
                        }
                    case "endValues":
                        {
                            var meter = chartElement.GetEndValue(dataColumn);

                            if (oldSeriesType == "Gantt")
                                meter = chartElement.GetGanttEndValue(dataColumn);

                            if (meter != null)
                                chartElement.InsertEndValue(insertIndex, meter);
                            break;
                        }
                    case "closeValues":
                        {
                            var meter = chartElement.GetCloseValue(dataColumn);
                            if (meter != null)
                                chartElement.InsertCloseValue(insertIndex, meter);
                            break;
                        }
                    case "lowValues":
                        {
                            var meter = chartElement.GetLowValue(dataColumn);
                            if (meter != null)
                                chartElement.InsertLowValue(insertIndex, meter);
                            break;
                        }
                    case "highValues":
                        {
                            var meter = chartElement.GetHighValue(dataColumn);
                            if (meter != null)
                                chartElement.InsertHighValue(insertIndex, meter);
                            break;
                        }
                    case "arguments":
                        {
                            var meter = chartElement.GetArgument(dataColumn);

                             if (oldSeriesType == "Bubble")
                                meter = chartElement.GetX(dataColumn);

                            if (meter != null)
                                chartElement.InsertArgument(insertIndex, meter);
                            break;
                        }

                    case "weights":
                        {
                            var meter = chartElement.GetWeight(dataColumn);
                            if (meter != null)
                                chartElement.InsertWeight(insertIndex, meter);
                            break;
                        }

                    case "series":
                        {
                            chartElement.AddSeries(dataColumn);
                            break;
                        }
                }
            }
        }

        private void CreateNewItem(Hashtable parameters, Hashtable callbackResult)
        {
            switch (parameters["containerName"] as string)
            {
                case "values":
                    {                        
                        var valueMeter = chartElement.CreateNewValue();
                        if (parameters["oldSeriesType"] != null)
                        {
                            SetPropertySeriesType(valueMeter, parameters["oldSeriesType"] as string);
                        }
                        break;
                    }
                case "endValues":
                    {
                        chartElement.CreateNewEndValue();
                        break;
                    }
                case "closeValues":
                    {
                        chartElement.CreateNewCloseValue();
                        break;
                    }
                case "lowValues":
                    {
                        chartElement.CreateNewLowValue();
                        break;
                    }
                case "highValues":
                    {
                        chartElement.CreateNewHighValue();
                        break;
                    }
                case "arguments":
                    {
                        chartElement.CreateNewArgument();
                        break;
                    }
                case "weights":
                    {
                        chartElement.CreateNewWeight();
                        break;
                    }
                case "series":
                    {
                        chartElement.CreateNewSeries();
                        break;
                    }
            }
        }

        private bool IsPictorialSeries(StiSeries series)
        {
            return series is IStiPictorialSeries || series is IStiPictorialStackedSeries;
        }

        private void SetSeriesType(Hashtable parameters, Hashtable callbackResult)
        {   
            var newSeriesType = parameters["seriesType"] as string;

            if (chartElement.DataMode == StiDataMode.ManuallyEnteringData)
            {
                SetPropertySeriesType(chartElement.GetManuallyEnteredChartMeter(), newSeriesType);

                if (newSeriesType == "Pictorial" || newSeriesType == "PictorialStacked")
                {
                    chartElement.Icon = newSeriesType == "PictorialStacked" ? Report.Helpers.StiFontIcons.Coffee : Report.Helpers.StiFontIcons.QuarterFull;
                }
                else
                {
                    chartElement.Icon = null;
                }
            }
            else
            {
                var oldIsBubble = (bool)parameters["oldIsBubble"];
                var oldIsGantt = (bool)parameters["oldIsGantt"];
                var itemIndex = Convert.ToInt32(parameters["itemIndex"]);
                var currentMeter = chartElement.GetValueByIndex(itemIndex);

                if (currentMeter != null)
                {   
                    var newSeries = StiChartSeriesCreator.New(newSeriesType);
                    var oldSeries = StiChartSeriesCreator.New(StiInvokeMethodsHelper.GetPropertyValue(currentMeter, "SeriesType")?.ToString());

                    if (IsPictorialSeries(newSeries))
                    {
                        chartElement.Icon = newSeries is IStiPictorialStackedSeries ? Report.Helpers.StiFontIcons.Coffee : Report.Helpers.StiFontIcons.QuarterFull;
                    }
                    else if (IsPictorialSeries(oldSeries))
                    {
                        chartElement.Icon = null;
                    }

                    SetPropertySeriesType(currentMeter, newSeriesType);

                    if (!(oldSeries is IStiBaseLineSeries && newSeries is IStiBaseLineSeries))
                    {
                        if (IsShowZeros(newSeriesType))
                            StiInvokeMethodsHelper.SetPropertyValue(currentMeter, "ShowZeros", (newSeries as IStiShowZerosSeries).ShowZeros ? StiEmptyCellsAs.Zero : StiEmptyCellsAs.Gap);

                        if (IsShowNulls(newSeriesType))
                            StiInvokeMethodsHelper.SetPropertyValue(currentMeter, "ShowNulls", (newSeries as IStiShowNullsSeries).ShowNulls ? StiEmptyCellsAs.Zero : StiEmptyCellsAs.Gap);
                    }
                }

                var valueMeters = chartElement.FetchAllValues();

                if (valueMeters.Count > 1)
                {
                    var seriesTypes = chartElement.GetChartSeriesTypes(newSeriesType);

                    foreach (IStiMeter valueMeter in valueMeters)
                    {
                        var seriesType = StiInvokeMethodsHelper.GetPropertyValue(valueMeter, "SeriesType");
                        if (seriesType != null && !seriesTypes.Contains(seriesType.ToString()))
                        {
                            SetPropertySeriesType(valueMeter, newSeriesType);
                        }
                    }
                }

                if (oldIsBubble && newSeriesType != "Bubble")
                    chartElement.ConvertFromBubble();

                if (!oldIsBubble && newSeriesType == "Bubble")
                    chartElement.ConvertToBubble();

                if (!oldIsGantt && newSeriesType == "Gantt")
                    chartElement.ConvertToGantt();

                var elementViewStates = chartElement as IStiUserViewStates;
                var currentViewState = elementViewStates.UserViewStates.FirstOrDefault(viewState => viewState.Key == elementViewStates.SelectedViewStateKey);

                if (currentViewState != null)
                    currentViewState.SeriesType = (StiChartSeriesType)Enum.Parse(typeof(StiChartSeriesType), newSeriesType);

                chartElement.CheckBrowsableProperties();
            }
        }

        private static List<Chart.StiChartStyle> GetChartElementStyles(StiReport report)
        {
            var styles = new List<Chart.StiChartStyle>();

            foreach (StiBaseStyle style in report.Styles)
            {
                if (style is StiChartStyle)
                {
                    var customStyle = new StiCustomStyle(style.Name);
                    customStyle.CustomCore.ReportChartStyle = style as StiChartStyle;
                    styles.Add(customStyle);
                }
            }

            styles.AddRange(StiOptions.Services.ChartStyles.OrderBy(x => x.GetType() != typeof(StiStyle29)).Where(x => x.AllowDashboard));

            return styles;
        }

        public static ArrayList GetStylesContent(StiReport report, Hashtable param)
        {
            ArrayList stylesContent = new ArrayList();
            var chartElement = param["componentName"] != null ? report.Pages.GetComponentByName((string)param["componentName"]) as IStiChartElement : null;

            if (chartElement != null)
            {
                var chart = StiChartHelper.CloneChart(chartElement);
                chart.Page = chartElement.Page;

                foreach (var style in GetChartElementStyles(report))
                {
                    Hashtable content = new Hashtable();

                    chart.Style = (Chart.StiChartStyle)style.Clone();

                    var customStyle = style as StiCustomStyle;
                    if (customStyle != null)
                    {
                        chart.CustomStyleName = customStyle.CustomCore.ReportChartStyle.Name;
                        content["name"] = chart.CustomStyleName;
                    }

                    chart.Core.ApplyStyle(chart.Style);

                    int width = 138;
                    int height = 67;
                    
                    content["image"] = StiChartHelper.GetChartSampleImage(chart, width, height, 1f);
                    content["ident"] = style is StiCustomStyle ? StiElementStyleIdent.Custom : style.StyleIdent;
                    content["localizedName"] = style.DashboardName;
                    content["width"] = width;
                    content["height"] = height;
                    stylesContent.Add(content);
                }
            }

            return stylesContent;
        }

        public static ArrayList GetValueMeterItems(IStiChartElement chartElement)
        {
            var items = new ArrayList();
            chartElement.FetchAllValues().ForEach(m =>
            {
                var meterObject = new Hashtable();
                meterObject["label"] = StiTableElementHelper.GetMeterLabel(m);
                meterObject["key"] = m.Key;
                meterObject["seriesType"] = StiInvokeMethodsHelper.GetPropertyValue(m, "SeriesType");
                items.Add(meterObject);
            });
            return items;
        }

        public static ArrayList GetArgumentMeterItems(IStiChartElement chartElement)
        {
            var items = new ArrayList();
            chartElement.FetchAllArguments().ForEach(m =>
            {
                var meterObject = new Hashtable();
                meterObject["label"] = StiTableElementHelper.GetMeterLabel(m);
                meterObject["key"] = m.Key;
                items.Add(meterObject);
            });
            return items;
        }

        public static ArrayList GetSeriesMeterItems(IStiChartElement chartElement)
        {
            var items = new ArrayList();
            var seriesMeter = chartElement.GetSeries();
            if (seriesMeter != null)
            {
                var meterObject = new Hashtable();
                meterObject["label"] = StiTableElementHelper.GetMeterLabel(seriesMeter);
                meterObject["key"] = seriesMeter.Key;
                items.Add(meterObject);
            }
            return items;
        }

        private void SetUserViewState(Hashtable parameters, Hashtable callbackResult)
        {
            var viewStateKey = parameters["viewStateKey"] as string;

            switch (viewStateKey) 
            {
                case "new": 
                    {
                        CreateNewUserViewState(parameters);
                        break;
                    }
                case "duplicate":
                    {
                        CreateNewUserViewState(parameters, true);
                        break;
                    }
                default:
                    {
                        ChangeViewState(viewStateKey);
                        break;
                    }
            }
        }

        #region UserViewStates
        private void ChangeViewState(string stateKey)
        {
            var elementViewStates = chartElement as IStiUserViewStates;
            elementViewStates.SwitchSelectedViewState(stateKey);
        }

        private void RemoveUserViewState(Hashtable parameters, Hashtable callbackResult)
        {
            var viewStateKey = parameters["viewStateKey"] as string;
            if (viewStateKey == null) return;

            var elementViewStates = chartElement as IStiUserViewStates;
            var currentViewState = elementViewStates.UserViewStates.FirstOrDefault(viewState => viewState.Key == viewStateKey);

            if (currentViewState != null)
            {
                var index = elementViewStates.UserViewStates.IndexOf(currentViewState);

                elementViewStates.UserViewStates.Remove(currentViewState);

                if (elementViewStates.UserViewStates.Count > 0)
                {
                    if (index == elementViewStates.UserViewStates.Count)
                        index--;

                    var state = elementViewStates.UserViewStates[index];
                    elementViewStates.SwitchSelectedViewState(state.Key);
                }
                else
                {
                    elementViewStates.SwitchSelectedViewState(null);
                }
            }
        }

        private void CreateNewUserViewState(Hashtable parameters, bool isDuplicating = false)
        {
            var seriesType = (StiChartSeriesType)Enum.Parse(typeof(StiChartSeriesType), parameters["seriesType"] as string);
            var newItemName = parameters["newItemName"] as string;
            var elementViewStates = chartElement as IStiUserViewStates;
            var stateJson = isDuplicating ? elementViewStates.SaveToJsonForViewState() : null;

            if (elementViewStates.UserViewStates.Count == 0)
            {
                elementViewStates.UserViewStates.Add(new StiUserViewState
                {
                    SeriesType = seriesType,
                    Name = $"{newItemName} 1"
                });
            }

            var nextName = GetNextItemName(newItemName, elementViewStates);
            var state = new StiUserViewState();
            state.Name = nextName;
            state.SeriesType = seriesType;
            ((IStiUserViewStates)chartElement).UserViewStates.Add(state);
            ((IStiUserViewStates)chartElement).SwitchSelectedViewState(state?.Key);

            if (isDuplicating)
                elementViewStates.LoadFromJsonForViewState(stateJson);
        }

        private string GetNextItemName(string itemName, IStiUserViewStates elementViewStates)
        {
            var index = 2;
            var name = $"{itemName} {index}";

            while (elementViewStates.UserViewStates.Any(v => v.Name == name))
            {
                name = $"{itemName} {++index}";
            }

            return name;
        }
        #endregion

        #region ConstantLines
        public static ArrayList GetConstantLines(IStiChartElement chartElement)
        {
            var constantLinesArray = new ArrayList();
            var constantLines = chartElement.FetchConstantLines();
            if (constantLines != null)
            {
                constantLines.ForEach(constantLine =>
                {
                    Hashtable constantLineObj = new Hashtable();
                    constantLineObj["name"] = constantLine.Text;
                    constantLineObj["properties"] = GetConstantLineProperties(constantLine);
                    constantLinesArray.Add(constantLineObj);
                });
            }
            return constantLinesArray;
        }

        private static Hashtable GetConstantLineProperties(IStiChartConstantLines constantLine)
        {
            Hashtable properties = new Hashtable();

            string[] propNames = { "AxisValue", "LineColor", "LineStyle", "LineWidth", "Position", "Text" };
            foreach (string propName in propNames)
            {
                var value = StiReportEdit.GetPropertyValue(propName, constantLine);
                if (value != null) { properties[propName] = value; }
            }

            return properties;
        }

        private void AddConstantLine(Hashtable parameters, Hashtable callbackResult)
        {
            chartElement.AddConstantLine();
        }

        private void RemoveConstantLine(Hashtable parameters, Hashtable callbackResult)
        {
            chartElement.RemoveConstantLine(Convert.ToInt32(parameters["constLineIndex"]));
        }

        private void MoveConstantLine(Hashtable parameters, Hashtable callbackResult)
        {
            chartElement.MoveConstantLine(Convert.ToInt32(parameters["fromIndex"]), Convert.ToInt32(parameters["toIndex"]));
        }
        #endregion
        #endregion

        #region Constructor
        public StiChartElementHelper(IStiChartElement chartElement)
        {
            this.chartElement = chartElement;
        }
        #endregion   
    }
}
