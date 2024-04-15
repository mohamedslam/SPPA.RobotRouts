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
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Export;
using Stimulsoft.Report.Helpers;
using Stimulsoft.Report.Maps;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;

namespace Stimulsoft.Report.Web.Helpers.Dashboards
{
    internal class StiOnlineMapElementHelper
    {
        #region Fields
        private IStiOnlineMapElement onlineMapElement;
        #endregion

        #region Helper Methods
        private Hashtable GetOnlineMapElementJSProperties(Hashtable parameters)
        {
            Hashtable properties = StiReportEdit.GetAllProperties(onlineMapElement as StiComponent);
            properties["meters"] = GetMetersHash();

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
            meterItem["functions"] = StiTableElementHelper.GetMeterFunctions(meter, onlineMapElement.Page as IStiDashboard);
            meterItem["currentFunction"] = Data.Helpers.StiExpressionHelper.GetFunction(meter.Expression);

            return meterItem;
        }

        private Hashtable GetMetersHash()
        {
            var meters = onlineMapElement.FetchAllMeters();
            var metersItems = new Hashtable();

            foreach (IStiMeter meter in meters)
            {
                switch (meter.GetType().Name)
                {
                    case "StiLocationMapMeter":
                        {
                            metersItems["location"] = GetMeterHashItem(meter);
                            break;
                        }
                    case "StiLocationValueMapMeter":
                        {
                            metersItems["value"] = GetMeterHashItem(meter);
                            break;
                        }
                    case "StiLocationColorMapMeter":
                        {
                            metersItems["color"] = GetMeterHashItem(meter);
                            break;
                        }
                    case "StiLocationArgumentMapMeter":
                        {
                            metersItems["argument"] = GetMeterHashItem(meter);
                            break;
                        }
                    case "StiLatitudeMapMeter":
                        {
                            metersItems["latitude"] = GetMeterHashItem(meter);
                            break;
                        }
                    case "StiLongitudeMapMeter":
                        {
                            metersItems["longitude"] = GetMeterHashItem(meter);
                            break;
                        }
                }
            }

            return metersItems;
        }

        private IStiMeter GetMeterByContainerName(string containerName)
        {
            var meters = onlineMapElement.FetchAllMeters();

            foreach (var meter in meters)
            {
                if (meter.GetType().Name == string.Format("Sti{0}MapMeter", containerName) || meter.GetType().Name == string.Format("StiLocation{0}MapMeter", containerName))
                {
                    return meter;
                }
            }

            return null;
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
                    {
                        MoveMeter(parameters, callbackResult);
                        break;
                    }
                case "SetFunction":
                    {
                        SetFunction(parameters, callbackResult);
                        break;
                    }
                case "NewItem":
                    {
                        CreateNewItem(parameters, callbackResult);
                        break;
                    }              
                case "GetOnlineMapElementProperties":
                    {
                        break;
                    }
                case "UpdateOnlineMapElementProperties":
                    {
                        UpdateOnlineMapElementProperties(parameters, callbackResult);
                        break;
                    }
            }

            callbackResult["elementProperties"] = GetOnlineMapElementJSProperties(parameters);
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
                StiElementChangedProcessor.ProcessElementChanging(onlineMapElement, StiElementChangedArgs.CreateRenamingArgs(meter.Label, parameters["newLabel"] as string));
                meter.Label = parameters["newLabel"] as string;
            }
        }

        private void UpdateOnlineMapElementProperties(Hashtable parameters, Hashtable callbackResult)
        {
            if (parameters["locationType"] != null)
                onlineMapElement.LocationType = (StiOnlineMapLocationType)Enum.Parse(typeof(StiOnlineMapLocationType), parameters["locationType"] as string);

            if (parameters["culture"] != null)
                onlineMapElement.Culture = (StiOnlineMapCulture)Enum.Parse(typeof(StiOnlineMapCulture), parameters["culture"] as string);

            if (parameters["locationColorType"] != null)
                onlineMapElement.LocationColorType = (StiOnlineMapLocationColorType)Enum.Parse(typeof(StiOnlineMapLocationColorType), parameters["locationColorType"] as string);

            if (parameters["heatmapColorGradientType"] != null)
                onlineMapElement.HeatmapColorGradientType = (StiOnlineMapHeatmapColorGradientType)Enum.Parse(typeof(StiOnlineMapHeatmapColorGradientType), parameters["heatmapColorGradientType"] as string);

            if (parameters["locationColor"] != null)
                onlineMapElement.LocationColor = StiReportEdit.StrToColor(parameters["locationColor"] as string);

            if (parameters["iconColor"] != null)
                onlineMapElement.IconColor = StiReportEdit.StrToColor(parameters["iconColor"] as string);

            if (parameters["icon"] != null)
                onlineMapElement.Icon = (StiFontIcons)Enum.Parse(typeof(StiFontIcons), parameters["icon"] as string);

            var customIcon = parameters["customIcon"] as string;
            onlineMapElement.CustomIcon = !String.IsNullOrEmpty(customIcon) ? Convert.FromBase64String(customIcon.Substring(customIcon.IndexOf("base64,") + 7)) : null;

            if (parameters["valueViewMode"] != null)
                onlineMapElement.ValueViewMode = (StiOnlineMapValueViewMode)Enum.Parse(typeof(StiOnlineMapValueViewMode), parameters["valueViewMode"] as string);
        }

        private void CreateNewItem(Hashtable parameters, Hashtable callbackResult)
        {
            switch (parameters["containerName"] as string)
            {
                case "Latitude":
                    {
                        onlineMapElement.CreateNewLatitudeMeter();
                        break;
                    }
                case "Longitude":
                    {
                        onlineMapElement.CreateNewLongitudeMeter();
                        break;
                    }
                case "Location":
                    {
                        onlineMapElement.CreateNewLocationMeter();
                        break;
                    }
                case "Value":
                    {
                        onlineMapElement.CreateNewLocationValueMeter();
                        break;
                    }
                case "Color":
                    {
                        onlineMapElement.CreateNewLocationColorMeter();
                        break;
                    }
                case "Argument":
                    {
                        onlineMapElement.CreateNewLocationArgumentMeter();
                        break;
                    }
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

        private void SetDataColumn(Hashtable parameters, Hashtable callbackResult)
        {
            var dataColumnObject = parameters["dataColumnObject"] as Hashtable;
            IStiAppDataCell dataCell = null;
            IStiAppDataCell nullDataCell = null;
            var oldLabel = string.Empty;

            if (dataColumnObject != null)
            {
                if (dataColumnObject["typeItem"] as string == "Variable")
                {
                    dataCell = onlineMapElement.Report.Dictionary.Variables[dataColumnObject["name"] as string] as IStiAppDataCell;
                }
                else
                {
                    var allColumns = dataColumnObject != null ? StiDictionaryHelper.GetColumnsByTypeAndNameOfObject(onlineMapElement.Report, dataColumnObject) : null;
                    if (allColumns != null) dataCell = allColumns[dataColumnObject["name"] as string];
                }
            }

            switch (parameters["containerName"] as string)
            {
                case "Latitude":
                    {
                        oldLabel = onlineMapElement.GetLatitudeMeter()?.Label;
                        onlineMapElement.AddLatitudeMeter(dataCell);
                        onlineMapElement.AddLocationMeter(nullDataCell);
                        break;
                    }
                case "Longitude":
                    {
                        oldLabel = onlineMapElement.GetLongitudeMeter()?.Label;
                        onlineMapElement.AddLongitudeMeter(dataCell);
                        onlineMapElement.AddLocationMeter(nullDataCell);
                        break;
                    }
                case "Location":
                    {
                        oldLabel = onlineMapElement.GetLocationMeter()?.Label;
                        onlineMapElement.AddLocationMeter(dataCell);
                        onlineMapElement.AddLongitudeMeter(nullDataCell);
                        onlineMapElement.AddLatitudeMeter(nullDataCell);
                        break;
                    }
                case "Value":
                    {
                        oldLabel = onlineMapElement.GetLocationValueMeter()?.Label;
                        onlineMapElement.AddLocationValueMeter(dataCell);
                        break;
                    }
                case "Color":
                    {
                        oldLabel = onlineMapElement.GetLocationColorMeter()?.Label;
                        onlineMapElement.AddLocationColorMeter(dataCell);
                        break;
                    }
                case "Argument":
                    {
                        oldLabel = onlineMapElement.GetLocationArgumentMeter()?.Label;
                        onlineMapElement.AddLocationArgumentMeter(dataCell);
                        break;
                    }
            }

            if (dataCell == null && !String.IsNullOrEmpty(oldLabel))
                StiElementChangedProcessor.ProcessElementChanging(onlineMapElement, StiElementChangedArgs.CreateDeletingArgs(oldLabel));
        }

        private void MoveMeter(Hashtable parameters, Hashtable callbackResult)
        {
            var toContainerName = parameters["toContainerName"] as string;
            var fromContainerName = parameters["fromContainerName"] as string;
            IStiMeter movingMeter = null;

            #region Get and Remove meter
            switch (fromContainerName)
            {
                
                case "Latitude":
                    {
                        movingMeter = onlineMapElement.GetLatitudeMeter();
                        onlineMapElement.RemoveLatitudeMeter();
                        break;
                    }
                case "Longitude":
                    {
                        movingMeter = onlineMapElement.GetLongitudeMeter();
                        onlineMapElement.RemoveLongitudeMeter();
                        break;
                    }
                case "Location":
                    {
                        movingMeter = onlineMapElement.GetLocationMeter();
                        onlineMapElement.RemoveLocationMeter();
                        break;
                    }
                case "Value":
                    {
                        movingMeter = onlineMapElement.GetLocationValueMeter();
                        onlineMapElement.RemoveLocationValueMeter();
                        break;
                    }
                case "Color":
                    {
                        movingMeter = onlineMapElement.GetLocationColorMeter();
                        onlineMapElement.RemoveLocationColorMeter();
                        break;
                    }
                case "Argument":
                    {
                        movingMeter = onlineMapElement.GetLocationArgumentMeter();
                        onlineMapElement.RemoveLocationArgumentMeter();
                        break;
                    }
            }
            #endregion

            #region Insert meter
            if (movingMeter != null)
            {
                switch (toContainerName)
                {
                    case "Latitude":
                        {
                            onlineMapElement.AddLatitudeMeter(movingMeter);
                            break;
                        }
                    case "Longitude":
                        {
                            onlineMapElement.AddLongitudeMeter(movingMeter);
                            break;
                        }
                    case "Location":
                        {
                            onlineMapElement.AddLocationMeter(movingMeter);
                            break;
                        }
                    case "Value":
                        {
                            onlineMapElement.AddLocationValueMeter(movingMeter);
                            break;
                        }
                    case "Color":
                        {
                            onlineMapElement.AddLocationColorMeter(movingMeter);
                            break;
                        }
                    case "Argument":
                        {
                            onlineMapElement.AddLocationArgumentMeter(movingMeter);
                            break;
                        }
                }
            }
            #endregion
        }
        #endregion

        #region Constructor
        public StiOnlineMapElementHelper(IStiOnlineMapElement onlineMapElement)
        {
            this.onlineMapElement = onlineMapElement;
        }
        #endregion   
    }
}
