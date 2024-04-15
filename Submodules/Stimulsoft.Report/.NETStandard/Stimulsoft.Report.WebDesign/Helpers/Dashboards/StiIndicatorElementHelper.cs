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
using Stimulsoft.Base.Helpers;
using Stimulsoft.Base.Meters;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Dashboard.Styles;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Export;
using Stimulsoft.Report.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Text;

namespace Stimulsoft.Report.Web.Helpers.Dashboards
{
    internal class StiIndicatorElementHelper
    {
        #region Fields
        private IStiIndicatorElement indicatorElement;        
        #endregion

        #region Helper Methods
        private Hashtable GetIndicatorElementJSProperties(Hashtable parameters)
        {
            Hashtable properties = StiReportEdit.GetAllProperties(indicatorElement as StiComponent);
            properties["iconRanges"] = GetIconRanges();
            properties["meters"] = GetMetersHash();
            properties["manuallyEnteredData"] = StiManuallyDataHelper.ConvertPackedStringToJSData(indicatorElement.ManuallyEnteredData);

            if ((string)parameters["command"] != "GetIndicatorElementProperties")
            {
                properties["svgContent"] = StiEncodingHelper.Encode(StiDashboardsSvgHelper.SaveElementToString(indicatorElement, 1, 1, true));
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
            meterItem["functions"] = StiTableElementHelper.GetMeterFunctions(meter, indicatorElement.Page as IStiDashboard);
            meterItem["currentFunction"] = Data.Helpers.StiExpressionHelper.GetFunction(meter.Expression);

            return meterItem;
        }

        private Hashtable GetMetersHash()
        {
            var meters = indicatorElement.FetchAllMeters();
            var metersItems = new Hashtable();

            foreach (IStiMeter meter in meters)
            {
                switch (meter.GetType().Name)
                {
                    case "StiValueIndicatorMeter":
                        {
                            metersItems["value"] = GetMeterHashItem(meter);
                            break;
                        }
                    case "StiTargetIndicatorMeter":
                        {
                            metersItems["target"] = GetMeterHashItem(meter);
                            break;
                        }
                    case "StiSeriesIndicatorMeter":
                        {
                            metersItems["series"] = GetMeterHashItem(meter);
                            break;
                        }
                }
            }

            return metersItems;
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
                case "RenameMeter":
                    {
                        RenameMeter(parameters, callbackResult);
                        break;
                    }
                case "MoveMeter":
                case "MoveAndDuplicateMeter":
                    {
                        MoveMeter(parameters, callbackResult);
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
                case "AddIndicatorIconRange":
                    {
                        AddIndicatorIconRange(parameters, callbackResult);
                        break;
                    }
                case "RemoveIndicatorIconRange":
                    {
                        RemoveIndicatorIconRange(parameters, callbackResult);
                        break;
                    }
                case "SetPropertyValueToIndicatorIconRange":
                    {
                        SetPropertyValueToIndicatorIconRange(parameters, callbackResult);
                        break;
                    }
                case "SetValueToManuallyEnteredData":
                    {
                        SetValueToManuallyEnteredData(parameters, callbackResult);
                        break;
                    }
                case "GetIndicatorElementProperties":
                    {
                        break;
                    }
            }

            callbackResult["elementProperties"] = GetIndicatorElementJSProperties(parameters);
        }

        private IStiMeter GetMeterByContainerName(string containerName)
        {
            var meters = indicatorElement.FetchAllMeters();

            foreach (var meter in meters)
            {
                if (meter.GetType().Name == string.Format("Sti{0}IndicatorMeter", containerName))
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
                        movingMeter = indicatorElement.GetValue();
                        if (!duplicateMeter)
                            indicatorElement.RemoveValue();
                        break;
                    }
                case "Target":
                    {
                        movingMeter = indicatorElement.GetTarget();
                        if (!duplicateMeter)
                            indicatorElement.RemoveTarget();
                        break;
                    }
                case "Series":
                    {
                        movingMeter = indicatorElement.GetSeries();
                        if (!duplicateMeter)
                            indicatorElement.RemoveSeries();
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
                            indicatorElement.AddValue(movingMeter);
                            break;
                        }
                    case "Target":
                        {
                            indicatorElement.AddTarget(movingMeter);
                            break;
                        }
                    case "Series":
                        {
                            indicatorElement.AddSeries(movingMeter);
                            break;
                        }
                }
            }
            #endregion
        }

        private void SetPropertyValue(Hashtable parameters, Hashtable callbackResult)
        {
            var propertyName = parameters["propertyName"] as string;
            var propertyValue = parameters["propertyValue"] as string;

            switch (propertyName)
            {
                case "Icon":
                    {
                        indicatorElement.Icon = (StiFontIcons)Enum.Parse(typeof(StiFontIcons), propertyValue);
                        break;
                    }
                case "IconSet":
                    {
                        indicatorElement.IconSet = (StiFontIconSet)Enum.Parse(typeof(StiFontIconSet), propertyValue);
                        break;
                    }
                case "CustomIcon":
                    {
                        indicatorElement.CustomIcon = !String.IsNullOrEmpty(propertyValue) ? Convert.FromBase64String(propertyValue.Substring(propertyValue.IndexOf("base64,") + 7)) : null;
                        break;
                    }
                case "IconMode":
                    {
                        indicatorElement.IconMode = (StiIndicatorIconMode)Enum.Parse(typeof(StiIndicatorIconMode), propertyValue);
                        break;
                    }
                case "IconRangeMode":
                    {
                        indicatorElement.IconRangeMode = (StiIndicatorIconRangeMode)Enum.Parse(typeof(StiIndicatorIconRangeMode), propertyValue);
                        break;
                    }
                case "DataMode":
                    {
                        indicatorElement.DataMode = (StiDataMode)Enum.Parse(typeof(StiDataMode), propertyValue);
                        break;
                    }
            }
        }

        private ArrayList GetIconRanges()
        {
            ArrayList ranges = new ArrayList();
            foreach (var range in indicatorElement.GetIconRanges())
            {
                ranges.Add(GetIconRangeItem(range));
            }

            return ranges;
        }

        private Hashtable GetIconRangeItem(IStiIndicatorIconRange range)
        {
            Hashtable rangeItem = new Hashtable();
            rangeItem["icon"] = range.Icon;
            rangeItem["startExpression"] = range.StartExpression;
            rangeItem["endExpression"] = range.EndExpression;

            return rangeItem;
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
                StiElementChangedProcessor.ProcessElementChanging(indicatorElement, StiElementChangedArgs.CreateRenamingArgs(meter.Label, parameters["newLabel"] as string));
                meter.Label = parameters["newLabel"] as string;
            }
        }

        private void SetFunction(Hashtable parameters, Hashtable callbackResult)
        {
            var meter = GetMeterByContainerName(parameters["containerName"] as string);
            if (meter != null)
            {
                meter.Expression = Data.Helpers.StiExpressionHelper.ReplaceFunction(meter.Expression, parameters["function"] as string);
            }
        }

        private void CreateNewItem(Hashtable parameters, Hashtable callbackResult)
        {
            switch (parameters["containerName"] as string)
            {
                case "Value":
                    {
                        indicatorElement.CreateNewValue();
                        break;
                    }
                case "Target":
                    {
                        indicatorElement.CreateNewTarget();
                        break;
                    }
                case "Series":
                    {
                        indicatorElement.CreateNewSeries();
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
                    dataCell = indicatorElement.Report.Dictionary.Variables[dataColumnObject["name"] as string] as IStiAppDataCell;
                }
                else
                {
                    var allColumns = dataColumnObject != null ? StiDictionaryHelper.GetColumnsByTypeAndNameOfObject(indicatorElement.Report, dataColumnObject) : null;
                    if (allColumns != null) dataCell = allColumns[dataColumnObject["name"] as string];
                }
            }

            switch (parameters["containerName"] as string)
            {
                case "Value":
                    {
                        oldLabel = indicatorElement.GetValue()?.Label;
                        indicatorElement.AddValue(dataCell);
                        break;
                    }
                case "Target":
                    {
                        oldLabel = indicatorElement.GetTarget()?.Label;
                        indicatorElement.AddTarget(dataCell);
                        break;
                    }
                case "Series":
                    {
                        oldLabel = indicatorElement.GetSeries()?.Label;
                        indicatorElement.AddSeries(dataCell);
                        break;
                    }
            }

            if (dataCell == null && !String.IsNullOrEmpty(oldLabel))
                StiElementChangedProcessor.ProcessElementChanging(indicatorElement, StiElementChangedArgs.CreateDeletingArgs(oldLabel));
        }

        private void SetValueToManuallyEnteredData(Hashtable parameters, Hashtable callbackResult)
        {
            indicatorElement.ManuallyEnteredData = StiManuallyDataHelper.ConvertJSDataToPackedString(parameters["propertyValue"] as string);
        }

        private void SetPropertyValueToIndicatorIconRange(Hashtable parameters, Hashtable callbackResult)
        {
            var propertyName = parameters["propertyName"] as string;
            var rangeIndex = Convert.ToInt32(parameters["rangeIndex"]);
            var ranges = indicatorElement.GetIconRanges();

            if (rangeIndex < ranges.Count)
            {
                var range = ranges[rangeIndex];

                switch (propertyName)
                {
                    case "Icon":
                        {
                            range.Icon = (StiFontIcons)Enum.Parse(typeof(StiFontIcons), parameters["propertyValue"] as string);
                            break;
                        }
                    case "StartExpression":
                        {
                            range.StartExpression = parameters["propertyValue"] as string;
                            break;
                        }
                    case "EndExpression":
                        {
                            range.EndExpression = parameters["propertyValue"] as string;
                            break;
                        }
                }
            }
        }

        private void AddIndicatorIconRange(Hashtable parameters, Hashtable callbackResult)
        {
            callbackResult["newRange"] = GetIconRangeItem(indicatorElement.AddRange());
        }

        private void RemoveIndicatorIconRange(Hashtable parameters, Hashtable callbackResult)
        {
            indicatorElement.RemoveRange(Convert.ToInt32(parameters["rangeIndex"]));
        }

        private static List<StiBaseStyle> GetIndicatorElementStyles(StiReport report, bool withReportStyles = true)
        {
            var styles = new List<StiBaseStyle>();

            if (withReportStyles)
            {
                foreach (StiBaseStyle style in report.Styles)
                {
                    if (style is StiIndicatorStyle)
                        styles.Add(style);
                }
            }

            styles.AddRange(StiOptions.Services.Dashboards.IndicatorStyles);

            return styles;
        }

        public static ArrayList GetStylesContent(StiReport report, Hashtable param, bool withReportStyles = true)
        {
            ArrayList stylesContent = new ArrayList();
            var indicatorElement = param["componentName"] != null ? report.Pages.GetComponentByName((string)param["componentName"]) as IStiIndicatorElement : null;
            var currentPage = indicatorElement != null ? indicatorElement.Page : new StiPage(report);

            int width = 120;
            int height = 55;

            var tempIndicatorElement = StiDashboardHelper.CreateDashboardElement(report, "StiIndicatorElement") as IStiIndicatorElement;
            currentPage.Components.Add(tempIndicatorElement as StiComponent);

            if (indicatorElement != null)
            {
                tempIndicatorElement.Icon = indicatorElement.Icon;
                tempIndicatorElement.IconSet = indicatorElement.IconSet;
            }

            foreach (var style in GetIndicatorElementStyles(report, withReportStyles))
            {
                Hashtable content = new Hashtable();

                if (style is StiIndicatorStyle)
                {
                    tempIndicatorElement.Style = StiElementStyleIdent.Custom;
                    tempIndicatorElement.CustomStyleName = ((StiIndicatorStyle)style).Name;
                    content["name"] = tempIndicatorElement.CustomStyleName;
                    content["ident"] = StiElementStyleIdent.Custom;
                }
                else if (style is StiIndicatorElementStyle)
                {
                    tempIndicatorElement.Style = ((StiIndicatorElementStyle)style).Ident;
                    tempIndicatorElement.CustomStyleName = string.Empty;
                    content["ident"] = tempIndicatorElement.Style;
                    content["localizedName"] = ((StiIndicatorElementStyle)style).LocalizedName;
                }

                content["image"] = StiDashboardHelper.GetDashboardStyleSampleImage(tempIndicatorElement, width, height);
                content["width"] = width;
                content["height"] = height;
                stylesContent.Add(content);
            }

            //Remove temp indicator element
            if (currentPage.Components.Contains(tempIndicatorElement as StiComponent))
                currentPage.Components.Remove(tempIndicatorElement as StiComponent);

            return stylesContent;
        }

        public static bool IsSeriesPresent(IStiIndicatorElement indicatorElement)
        {
            return indicatorElement.GetSeries() != null;
        }

        public static bool IsTargetPresent(IStiIndicatorElement indicatorElement)
        {
            return indicatorElement.GetTarget() != null || indicatorElement.DataMode == StiDataMode.ManuallyEnteringData;
        }
        #endregion

        #region Constructor
        public StiIndicatorElementHelper(IStiIndicatorElement indicatorElement)
        {
            this.indicatorElement = indicatorElement;
        }
        #endregion   
    }
}
