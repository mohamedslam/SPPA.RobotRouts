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
using Stimulsoft.Base.Meters;
using Stimulsoft.Data.Helpers;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Dashboard.Styles;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Export;
using Stimulsoft.Report.Export.Services.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Stimulsoft.Report.Web.Helpers.Dashboards
{
    internal class StiProgressElementHelper
    {
        #region Fields
        private IStiProgressElement progressElement;
        #endregion

        #region Helper Methods
        private Hashtable GetProgressElementJSProperties(Hashtable parameters)
        {
            Hashtable properties = StiReportEdit.GetAllProperties(progressElement as StiComponent);
            properties["meters"] = GetMetersHash();
            properties["manuallyEnteredData"] = StiManuallyDataHelper.ConvertPackedStringToJSData(progressElement.ManuallyEnteredData);

            if ((string)parameters["command"] != "GetProgressElementProperties")
            {
                properties["svgContent"] = StiEncodingHelper.Encode(StiDashboardsSvgHelper.SaveElementToString(progressElement, 1, 1, true));
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
            meterItem["functions"] = StiTableElementHelper.GetMeterFunctions(meter, progressElement.Page as IStiDashboard);
            meterItem["currentFunction"] = StiExpressionHelper.GetFunction(meter.Expression);

            return meterItem;
        }

        private Hashtable GetMetersHash()
        {
            var meters = progressElement.FetchAllMeters();
            var metersItems = new Hashtable();

            foreach (IStiMeter meter in meters)
            {
                switch (meter.GetType().Name)
                {
                    case "StiValueProgressMeter":
                        {
                            metersItems["value"] = GetMeterHashItem(meter);
                            break;
                        }
                    case "StiTargetProgressMeter":
                        {
                            metersItems["target"] = GetMeterHashItem(meter);
                            break;
                        }
                    case "StiSeriesProgressMeter":
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
                case "SetValueToManuallyEnteredData":
                    {
                        SetValueToManuallyEnteredData(parameters, callbackResult);
                        break;
                    }
                case "GetProgressElementProperties":
                    {
                        break;
                    }
            }

            callbackResult["elementProperties"] = GetProgressElementJSProperties(parameters);
        }

        private IStiMeter GetMeterByContainerName(string containerName)
        {
            var meters = progressElement.FetchAllMeters();

            foreach (var meter in meters)
            {
                if (meter.GetType().Name == string.Format("Sti{0}ProgressMeter", containerName))
                {
                    return meter;
                }
            }

            return null;
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
                StiElementChangedProcessor.ProcessElementChanging(progressElement, StiElementChangedArgs.CreateRenamingArgs(meter.Label, parameters["newLabel"] as string));
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
                        movingMeter = progressElement.GetValue();
                        if (!duplicateMeter)
                            progressElement.RemoveValue();
                        break;
                    }
                case "Target":
                    {
                        movingMeter = progressElement.GetTarget();
                        if (!duplicateMeter)
                            progressElement.RemoveTarget();
                        break;
                    }
                case "Series":
                    {
                        movingMeter = progressElement.GetSeries();
                        if (!duplicateMeter)
                            progressElement.RemoveSeries();
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
                            progressElement.AddValue(movingMeter);
                            break;
                        }
                    case "Target":
                        {
                            progressElement.AddTarget(movingMeter);
                            break;
                        }
                    case "Series":
                        {
                            progressElement.AddSeries(movingMeter);
                            break;
                        }
                }
            }
            #endregion
        }

        private void CreateNewItem(Hashtable parameters, Hashtable callbackResult)
        {
            switch (parameters["containerName"] as string)
            {
                case "Value":
                    {
                        progressElement.CreateNewValue();
                        break;
                    }
                case "Target":
                    {
                        progressElement.CreateNewTarget();
                        break;
                    }
                case "Series":
                    {
                        progressElement.CreateNewSeries();
                        break;
                    }
            }
        }

        private void SetValueToManuallyEnteredData(Hashtable parameters, Hashtable callbackResult)
        {
            progressElement.ManuallyEnteredData = StiManuallyDataHelper.ConvertJSDataToPackedString(parameters["propertyValue"] as string);
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
                    dataCell = progressElement.Report.Dictionary.Variables[dataColumnObject["name"] as string] as IStiAppDataCell;
                }
                else
                {
                    var allColumns = dataColumnObject != null ? StiDictionaryHelper.GetColumnsByTypeAndNameOfObject(progressElement.Report, dataColumnObject) : null;
                    if (allColumns != null) dataCell = allColumns[dataColumnObject["name"] as string];
                }
            }

            switch (parameters["containerName"] as string)
            {
                case "Value":
                    {
                        oldLabel = progressElement.GetValue()?.Label;
                        progressElement.AddValue(dataCell);
                        break;
                    }
                case "Target":
                    {
                        oldLabel = progressElement.GetTarget()?.Label;
                        progressElement.AddTarget(dataCell);
                        break;
                    }
                case "Series":
                    {
                        oldLabel = progressElement.GetSeries()?.Label;
                        progressElement.AddSeries(dataCell);
                        break;
                    }
            }

            if (dataCell == null && !String.IsNullOrEmpty(oldLabel))
                StiElementChangedProcessor.ProcessElementChanging(progressElement, StiElementChangedArgs.CreateDeletingArgs(oldLabel));
        }

        private void SetPropertyValue(Hashtable parameters, Hashtable callbackResult)
        {
            var propertyName = parameters["propertyName"] as string;
            var propertyValue = parameters["propertyValue"] as string;

            switch (propertyName)
            {
                case "Mode":
                    {
                        progressElement.Mode = (StiProgressElementMode)Enum.Parse(typeof(StiProgressElementMode), propertyValue);
                        break;
                    }
                case "DataMode":
                    {
                        progressElement.DataMode = (StiDataMode)Enum.Parse(typeof(StiDataMode), propertyValue);
                        break;
                    }
            }
        }

        private static List<StiBaseStyle> GetProgressElementStyles(StiReport report, bool withReportStyles = true)
        {
            var styles = new List<StiBaseStyle>();

            if (withReportStyles)
            {
                foreach (StiBaseStyle style in report.Styles)
                {
                    if (style is StiProgressStyle)
                        styles.Add(style);
                }
            }

            styles.AddRange(StiOptions.Services.Dashboards.ProgressStyles);

            return styles;
        }

        public static ArrayList GetStylesContent(StiReport report, Hashtable param, bool withReportStyles = true)
        {
            ArrayList stylesContent = new ArrayList();
            var progressElement = param["componentName"] != null ? report.Pages.GetComponentByName((string)param["componentName"]) as IStiProgressElement : null;
            var currentPage = progressElement != null ? progressElement.Page : new StiPage(report);

            int width = 120;
            int height = 55;

            var tempProgressElement = StiDashboardHelper.CreateDashboardElement(report, "StiProgressElement") as IStiProgressElement;
            currentPage.Components.Add(tempProgressElement as StiComponent);

            if (progressElement != null)
            {
                tempProgressElement.Mode = progressElement.Mode;
            }

            var isSampleForStyles = tempProgressElement.GetType().GetProperty("IsSampleForStyles", BindingFlags.NonPublic | BindingFlags.Instance);
            if (isSampleForStyles != null) isSampleForStyles.SetValue(tempProgressElement, true, null);

            foreach (var style in GetProgressElementStyles(report, withReportStyles))
            {
                Hashtable content = new Hashtable();

                if (style is StiProgressStyle)
                {
                    tempProgressElement.Style = StiElementStyleIdent.Custom;
                    tempProgressElement.CustomStyleName = ((StiProgressStyle)style).Name;
                    content["name"] = tempProgressElement.CustomStyleName;
                    content["ident"] = StiElementStyleIdent.Custom;
                }
                else if (style is StiProgressElementStyle)
                {
                    tempProgressElement.Style = ((StiProgressElementStyle)style).Ident;
                    tempProgressElement.CustomStyleName = string.Empty;
                    content["ident"] = tempProgressElement.Style;
                    content["localizedName"] = ((StiProgressElementStyle)style).LocalizedName;
                }

                content["image"] = StiDashboardHelper.GetDashboardStyleSampleImage(tempProgressElement, width, height);
                content["width"] = width;
                content["height"] = height;
                stylesContent.Add(content);
            }

            //Remove temp progress element
            if (currentPage.Components.Contains(tempProgressElement as StiComponent))
                currentPage.Components.Remove(tempProgressElement as StiComponent);

            return stylesContent;
        }

        public static bool IsSeriesPresent(IStiProgressElement progressElement)
        {
            return progressElement.GetSeries() != null;
        }
        #endregion

        #region Constructor
        public StiProgressElementHelper(IStiProgressElement progressElement)
        {
            this.progressElement = progressElement;
        }
        #endregion   
    }
}
