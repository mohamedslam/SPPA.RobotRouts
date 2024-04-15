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
using Stimulsoft.Base.Meters;
using Stimulsoft.Data.Helpers;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Gauge;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Stimulsoft.Report.Web.Helpers.Dashboards
{
    internal class StiGaugeElementHelper
    {
        #region Fields
        private IStiGaugeElement gaugeElement;
        #endregion

        #region Helper Methods
        private Hashtable GetGaugeElementJSProperties(Hashtable parameters)
        {
            Hashtable properties = StiReportEdit.GetAllProperties(gaugeElement as StiComponent);
            properties["ranges"] = GetGaugeRanges();
            properties["meters"] = GetMetersHash();
            properties["manuallyEnteredData"] = StiManuallyDataHelper.ConvertPackedStringToJSData(gaugeElement.ManuallyEnteredData);

            if ((string)parameters["command"] != "GetGaugeElementProperties")
            {
                properties["svgContent"] = StiEncodingHelper.Encode(StiDashboardsSvgHelper.SaveElementToString(gaugeElement, 1, 1, true));
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
            meterItem["functions"] = StiTableElementHelper.GetMeterFunctions(meter, gaugeElement.Page as IStiDashboard);
            meterItem["currentFunction"] = StiExpressionHelper.GetFunction(meter.Expression);

            return meterItem;
        }

        private Hashtable GetMetersHash()
        {
            var meters = gaugeElement.FetchAllMeters();
            var metersItems = new Hashtable();

            foreach (IStiMeter meter in meters)
            {
                switch (meter.GetType().Name)
                {
                    case "StiValueGaugeMeter":
                        {
                            metersItems["value"] = GetMeterHashItem(meter);
                            break;
                        }
                    case "StiSeriesGaugeMeter":
                        {
                            metersItems["series"] = GetMeterHashItem(meter);
                            break;
                        }
                    case "StiTargetGaugeMeter":
                        {
                            metersItems["target"] = GetMeterHashItem(meter);
                            break;
                        }
                }
            }

            return metersItems;
        }

        private ArrayList GetGaugeRanges()
        {
            ArrayList ranges = new ArrayList();
            foreach (var range in gaugeElement.GetRanges())
            {
                ranges.Add(GetGaugeRangeItem(range));
            }

            return ranges;
        }

        private Hashtable GetGaugeRangeItem(IStiGaugeRange range)
        {
            Hashtable rangeItem = new Hashtable();
            rangeItem["color"] = StiReportEdit.GetStringFromColor(range.Color);
            rangeItem["start"] = range.Start;
            rangeItem["end"] = range.End;

            return rangeItem;
        }
        #endregion

        #region Methods
        public void ExecuteJSCommand(Hashtable parameters, Hashtable callbackResult)
        {
            switch ((string)parameters["command"])
            {

                case "SetExpression":
                    {
                        SetExpression(parameters, callbackResult);
                        break;
                    }
                case "SetDataColumn":
                    {
                        SetDataColumn(parameters, callbackResult);
                        break;
                    }
                case "MoveMeter":
                case "MoveAndDuplicateMeter":
                    {
                        MoveMeter(parameters, callbackResult);
                        break;
                    }
                case "RenameMeter":
                    {
                        RenameMeter(parameters, callbackResult);
                        break;
                    }
                case "SetFunction":
                    {
                        SetFunction(parameters, callbackResult);
                        break;
                    }
                case "SetPropertyValue":
                    {
                        SetPropertyValue(parameters, callbackResult);
                        break;
                    }
                case "NewItem":
                    {
                        CreateNewItem(parameters, callbackResult);
                        break;
                    }
                case "AddGaugeRange":
                    {
                        AddGaugeRange(parameters, callbackResult);
                        break;
                    }
                case "RemoveGaugeRange":
                    {
                        RemoveGaugeRange(parameters, callbackResult);
                        break;
                    }
                case "SetPropertyValueToGaugeRange":
                    {
                        SetPropertyValueToGaugeRange(parameters, callbackResult);
                        break;
                    }
                case "SetValueToManuallyEnteredData":
                    {
                        SetValueToManuallyEnteredData(parameters, callbackResult);
                        break;
                    }
                case "GetGaugeElementProperties":
                    {
                        break;
                    }
            }

            callbackResult["elementProperties"] = GetGaugeElementJSProperties(parameters);
        }

        private IStiMeter GetMeterByContainerName(string containerName)
        {
            var meters = gaugeElement.FetchAllMeters();

            foreach (var meter in meters)
            {
                if (meter.GetType().Name == string.Format("Sti{0}GaugeMeter", containerName))
                {
                    return meter;
                }
            }

            return null;
        }

        private void MoveMeter(Hashtable parameters, Hashtable callbackResult)
        {
            var duplicateMeter = (string)parameters["command"] == "MoveAndDuplicateMeter";
            var toContainerName = parameters["toContainerName"] as string;
            var fromContainerName = parameters["fromContainerName"] as string;
            IStiMeter movingMeter = null;

            #region Get and Remove meter
            switch (fromContainerName)
            {
                case "Value":
                    {
                        movingMeter = gaugeElement.GetValue();
                        if (!duplicateMeter)
                            gaugeElement.RemoveValue();
                        break;
                    }
                case "Series":
                    {
                        movingMeter = gaugeElement.GetSeries();
                        if (!duplicateMeter)
                            gaugeElement.RemoveSeries();
                        break;
                    }
                case "Target":
                    {
                        movingMeter = gaugeElement.GetTarget();
                        if (!duplicateMeter)
                            gaugeElement.RemoveTarget();
                        break;
                    }
            }
            #endregion

            #region Insert meter
            if (movingMeter != null)
            {
                if (duplicateMeter)
                {
                    var cloneMeter = (movingMeter as ICloneable).Clone();
                    movingMeter = cloneMeter as IStiMeter;
                }

                switch (toContainerName)
                {
                    case "Value":
                        {
                            gaugeElement.AddValue(movingMeter);
                            break;
                        }
                    case "Series":
                        {
                            gaugeElement.AddSeries(movingMeter);
                            break;
                        }
                    case "Target":
                        {
                            gaugeElement.AddTarget(movingMeter);
                            break;
                        }
                }
            }
            #endregion
        }

        private void SetExpression(Hashtable parameters, Hashtable callbackResult)
        {
            var meter = GetMeterByContainerName(parameters["containerName"] as string);
            if (meter != null)
            {
                meter.Expression = StiEncodingHelper.DecodeString(parameters["expressionValue"] as string);
            }
        }

        private void RenameMeter(Hashtable parameters, Hashtable callbackResult)
        {
            var meter = GetMeterByContainerName(parameters["containerName"] as string);
            if (meter != null && parameters["newLabel"] != null)
            {
                StiElementChangedProcessor.ProcessElementChanging(gaugeElement, StiElementChangedArgs.CreateRenamingArgs(meter.Label, parameters["newLabel"] as string));
                meter.Label = parameters["newLabel"] as string;
            }
        }

        private void SetFunction(Hashtable parameters, Hashtable callbackResult)
        {
            var meter = GetMeterByContainerName(parameters["containerName"] as string);
            if (meter != null)
            {
                meter.Expression = StiExpressionHelper.ReplaceFunction(meter.Expression, parameters["function"] as string);
            }
        }

        private void CreateNewItem(Hashtable parameters, Hashtable callbackResult)
        {
            switch (parameters["containerName"] as string)
            {
                case "Value":
                    {
                        gaugeElement.CreateNewValue();
                        break;
                    }                
                case "Series":
                    {
                        gaugeElement.CreateNewSeries();
                        break;
                    }
                case "Target":
                    {
                        gaugeElement.CreateNewTarget();
                        break;
                    }
            }
        }

        private void SetDataColumn(Hashtable parameters, Hashtable callbackResult)
        {
            var dataColumnObject = parameters["dataColumnObject"] as Hashtable;
            IStiAppDataCell dataCell = null;
            var oldLabel = string.Empty;

            if (dataColumnObject != null)
            {
                if (dataColumnObject["typeItem"] as string == "Variable")
                {
                    dataCell = gaugeElement.Report.Dictionary.Variables[dataColumnObject["name"] as string] as IStiAppDataCell;
                }
                else
                {
                    var allColumns = dataColumnObject != null ? StiDictionaryHelper.GetColumnsByTypeAndNameOfObject(gaugeElement.Report, dataColumnObject) : null;
                    if (allColumns != null) dataCell = allColumns[dataColumnObject["name"] as string];
                }
            }

            switch (parameters["containerName"] as string)
            {
                case "Value":
                    {
                        oldLabel = gaugeElement.GetValue()?.Label;
                        gaugeElement.AddValue(dataCell);
                        break;
                    }
                case "Series":
                    {
                        oldLabel = gaugeElement.GetSeries()?.Label;
                        gaugeElement.AddSeries(dataCell);
                        break;
                    }
                case "Target":
                    {
                        oldLabel = gaugeElement.GetTarget()?.Label;
                        gaugeElement.AddTarget(dataCell);
                        break;
                    }
            }

            if (dataCell == null && !String.IsNullOrEmpty(oldLabel))
                StiElementChangedProcessor.ProcessElementChanging(gaugeElement, StiElementChangedArgs.CreateDeletingArgs(oldLabel));
        }

        private void SetValueToManuallyEnteredData(Hashtable parameters, Hashtable callbackResult)
        {
            gaugeElement.ManuallyEnteredData = StiManuallyDataHelper.ConvertJSDataToPackedString(parameters["propertyValue"] as string);
        }

        private void SetPropertyValue(Hashtable parameters, Hashtable callbackResult)
        {
            var propertyName = parameters["propertyName"] as string;
            var propertyValue = parameters["propertyValue"] as string;

            switch (propertyName)
            {
                case "Type":
                    {
                        gaugeElement.Type = (StiGaugeType)Enum.Parse(typeof(StiGaugeType), propertyValue);
                        break;
                    }
                case "CalculationMode":
                    {
                        gaugeElement.CalculationMode = (StiGaugeCalculationMode)Enum.Parse(typeof(StiGaugeCalculationMode), propertyValue);
                        break;
                    }
                case "Minimum":
                    {
                        gaugeElement.Minimum = Convert.ToDecimal(propertyValue);
                        break;
                    }
                case "Maximum":
                    {
                        gaugeElement.Maximum = Convert.ToDecimal(propertyValue);
                        break;
                    }
                case "RangeType":
                    {
                        gaugeElement.RangeType = (StiGaugeRangeType)Enum.Parse(typeof(StiGaugeRangeType), propertyValue);
                        if (gaugeElement.RangeType == StiGaugeRangeType.Color && gaugeElement.GetRanges().Count == 0)
                        {
                            gaugeElement.CreatedDefaultRanges();
                        }
                        break;
                    }
                case "RangeMode":
                    {
                        gaugeElement.RangeMode = (StiGaugeRangeMode)Enum.Parse(typeof(StiGaugeRangeMode), propertyValue);
                        break;
                    }
                case "DataMode":
                    {
                        gaugeElement.DataMode = (StiDataMode)Enum.Parse(typeof(StiDataMode), propertyValue);
                        break;
                    }
            }
        }

        private void SetPropertyValueToGaugeRange(Hashtable parameters, Hashtable callbackResult)
        {
            var propertyName = parameters["propertyName"] as string;
            var rangeIndex = Convert.ToInt32(parameters["rangeIndex"]);
            var ranges = gaugeElement.GetRanges();

            if (rangeIndex < ranges.Count)
            {
                var range = ranges[rangeIndex];

                switch (propertyName)
                {
                    case "Color":
                        {
                            range.Color = StiReportEdit.StrToColor(parameters["propertyValue"] as string);
                            break;
                        }
                    case "Start":
                        {
                            range.Start = Convert.ToDouble(parameters["propertyValue"]);
                            break;
                        }
                    case "End":
                        {
                            range.End = Convert.ToDouble(parameters["propertyValue"]);
                            break;
                        }
                }
            }
        }

        private void AddGaugeRange(Hashtable parameters, Hashtable callbackResult)
        {
            callbackResult["newRange"] = GetGaugeRangeItem(gaugeElement.AddRange());
        }

        private void RemoveGaugeRange(Hashtable parameters, Hashtable callbackResult)
        {
            gaugeElement.RemoveRange(Convert.ToInt32(parameters["rangeIndex"]));
        }

        private static List<StiGaugeStyleXF> GetGaugeElementStyles(StiReport report)
        {
            var gaugeStyles = new List<StiGaugeStyleXF>();

            foreach (StiBaseStyle style in report.Styles)
            {
                if (style is StiGaugeStyle)
                {
                    var customStyle = new StiCustomGaugeStyle((StiGaugeStyle)style);
                    gaugeStyles.Add(customStyle);
                }
            }

            gaugeStyles.AddRange(StiOptions.Services.GaugeStyles.OrderBy(x => x.GetType() != typeof(StiGaugeStyleXF29)).Where(x => x.AllowDashboard));

            return gaugeStyles;
        }

        public static ArrayList GetStylesContent(StiReport report, Hashtable param)
        {
            ArrayList stylesContent = new ArrayList();
            var gaugeElement = param["componentName"] != null ? report.Pages.GetComponentByName((string)param["componentName"]) as IStiGaugeElement : null;

            if (gaugeElement != null)
            {
                int width = 125;
                int height = 60;

                var tempGaugeElement = StiDashboardHelper.CreateDashboardElement(report, "StiGaugeElement") as IStiGaugeElement;
                gaugeElement.Page.Components.Add(tempGaugeElement as StiComponent);
                tempGaugeElement.Type = gaugeElement.Type;
                tempGaugeElement.RangeType = StiGaugeRangeType.None;

                foreach (var style in GetGaugeElementStyles(report))
                {
                    Hashtable content = new Hashtable();

                    if (style is StiCustomGaugeStyle)
                    {
                        var styleName = ((StiCustomGaugeStyleCoreXF)style.Core).ReportStyleName;
                        content["name"] = styleName;
                        content["ident"] = StiElementStyleIdent.Custom;
                        tempGaugeElement.Style = StiElementStyleIdent.Custom;
                        tempGaugeElement.CustomStyleName = styleName;
                    }
                    else
                    {
                        content["ident"] = style.StyleIdent;
                        content["localizedName"] = style.DashboardName;
                        tempGaugeElement.Style = style.StyleIdent;
                        tempGaugeElement.CustomStyleName = string.Empty;
                    }
                    
                    content["image"] = StiDashboardHelper.GetDashboardStyleSampleImage(tempGaugeElement, width, height);
                    content["width"] = width;
                    content["height"] = height;
                    stylesContent.Add(content);
                }

                //Remove temp gauge element
                if (gaugeElement.Page.Components.Contains(tempGaugeElement as StiComponent))
                    gaugeElement.Page.Components.Remove(tempGaugeElement as StiComponent);
            }

            return stylesContent;
        }
        #endregion

        #region Constructor
        public StiGaugeElementHelper(IStiGaugeElement gaugeElement)
        {
            this.gaugeElement = gaugeElement;
        }
        #endregion   
    }
}
