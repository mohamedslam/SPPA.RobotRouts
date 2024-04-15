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
using Stimulsoft.Report.Maps;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace Stimulsoft.Report.Web.Helpers.Dashboards
{
    internal class StiRegionMapElementHelper
    {
        #region Fields
        private IStiRegionMapElement regionMapElement;
        #endregion

        #region Helper Methods
        private Hashtable GetRegionMapElementJSProperties(Hashtable parameters)
        {
            Hashtable properties = StiReportEdit.GetAllProperties(regionMapElement as StiComponent);
            properties["mapData"] = GetMapDataForJS(regionMapElement);
            properties["meters"] = GetMetersHash();
            properties["language"] = regionMapElement.Language;
            properties["languages"] = StiMapHelper.GetMapLanguages(regionMapElement.MapIdent);

            if ((string)parameters["command"] != "GetMapElementProperties")
            {
                properties["svgContent"] = StiEncodingHelper.Encode(StiDashboardsSvgHelper.SaveElementToString(regionMapElement, 1, 1, true));
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
            meterItem["functions"] = StiTableElementHelper.GetMeterFunctions(meter, regionMapElement.Page as IStiDashboard);
            meterItem["currentFunction"] = StiExpressionHelper.GetFunction(meter.Expression);

            return meterItem;
        }

        private Hashtable GetMetersHash()
        {
            var meters = regionMapElement.FetchAllMeters();
            var metersItems = new Hashtable();

            foreach (IStiMeter meter in meters)
            {
                if (meter is IStiColorMapMeter)
                    metersItems["color"] = GetMeterHashItem(meter);
                else if (meter is IStiGroupMapMeter)
                    metersItems["group"] = GetMeterHashItem(meter);
                else if (meter is IStiKeyMapMeter)
                    metersItems["key"] = GetMeterHashItem(meter);
                else if (meter is IStiNameMapMeter)
                    metersItems["name"] = GetMeterHashItem(meter);
                else if (meter is IStiValueMapMeter)
                    metersItems["value"] = GetMeterHashItem(meter);
            }

            return metersItems;
        }

        public static ArrayList GetMapDataForJS(IStiRegionMapElement regionMapElement)
        {
            ArrayList resultData = new ArrayList();
            List<StiMapData> mapData = regionMapElement.GetMapData().OrderBy(x => x.Key).ToList();
            bool allowGroup = AllowGroup(regionMapElement);
            bool allowColor = AllowColor(regionMapElement);

            foreach (var data in mapData)
            {
                var row = new Hashtable();
                row["key"] = data.Key;
                row["name"] = data.Name;
                row["value"] = data.Value;
                if (allowGroup)
                    row["group"] = data.Group;
                if (allowColor)
                    row["color"] = data.Color;

                resultData.Add(row);
            }

            return resultData;
        }
                
        private static bool AllowGroup(IStiRegionMapElement regionMapElement)
        {
            return (regionMapElement.MapType == StiMapType.Group || regionMapElement.MapType == StiMapType.HeatmapWithGroup);
        }

        private static bool AllowColor(IStiRegionMapElement regionMapElement)
        {
            return (regionMapElement.MapType == StiMapType.Individual);
        }

        private IStiMeter GetMeterByContainerName(string containerName)
        {
            var meters = regionMapElement.FetchAllMeters();

            foreach (var meter in meters)
            {
                if (meter.GetType().Name == string.Format("Sti{0}MapMeter", containerName))
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
                case "SetProperties":
                    {
                        SetProperties(parameters, callbackResult);
                        break;
                    }
                case "NewItem":
                    {
                        CreateNewItem(parameters, callbackResult);
                        break;
                    }
                case "UpdateMapData":
                    {
                        UpdateMapData(parameters, callbackResult);
                        break;
                    }                    
                case "GetRegionMapElementProperties":
                    {
                        break;
                    }
            }

            callbackResult["elementProperties"] = GetRegionMapElementJSProperties(parameters);
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
                StiElementChangedProcessor.ProcessElementChanging(regionMapElement, StiElementChangedArgs.CreateRenamingArgs(meter.Label, parameters["newLabel"] as string));
                meter.Label = parameters["newLabel"] as string;
            }
        }

        private void CreateNewItem(Hashtable parameters, Hashtable callbackResult)
        {
            switch (parameters["containerName"] as string)
            {
                case "Key":
                    {
                        regionMapElement.CreateNewKeyMeter();
                        break;
                    }
                case "Name":
                    {
                        regionMapElement.CreateNewNameMeter();
                        break;
                    }
                case "Value":
                    {
                        regionMapElement.CreateNewValueMeter();
                        break;
                    }
                case "Group":
                    {
                        regionMapElement.CreateNewGroupMeter();
                        break;
                    }
                case "Color":
                    {
                        regionMapElement.CreateNewColorMeter();
                        break;
                    }
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

        private void SetDataColumn(Hashtable parameters, Hashtable callbackResult)
        {
            var dataColumnObject = parameters["dataColumnObject"] as Hashtable;
            IStiAppDataCell dataCell = null;
            var oldLabel = string.Empty;

            if (dataColumnObject != null)
            {
                if (dataColumnObject["typeItem"] as string == "Variable")
                {
                    dataCell = regionMapElement.Report.Dictionary.Variables[dataColumnObject["name"] as string] as IStiAppDataCell;
                }
                else
                {
                    var allColumns = dataColumnObject != null ? StiDictionaryHelper.GetColumnsByTypeAndNameOfObject(regionMapElement.Report, dataColumnObject) : null;
                    if (allColumns != null) dataCell = allColumns[dataColumnObject["name"] as string];
                }
            }

            switch (parameters["containerName"] as string)
            {
                case "Key":
                    {
                        oldLabel = regionMapElement.GetKeyMeter()?.Label;
                        regionMapElement.AddKeyMeter(dataCell);
                        break;
                    }
                case "Name":
                    {
                        oldLabel = regionMapElement.GetNameMeter()?.Label;
                        regionMapElement.AddNameMeter(dataCell);
                        break;
                    }
                case "Value":
                    {
                        oldLabel = regionMapElement.GetValueMeter()?.Label;
                        regionMapElement.AddValueMeter(dataCell);
                        break;
                    }
                case "Group":
                    {
                        oldLabel = regionMapElement.GetGroupMeter()?.Label;
                        regionMapElement.AddGroupMeter(dataCell);
                        break;
                    }
                case "Color":
                    {
                        oldLabel = regionMapElement.GetColorMeter()?.Label;
                        regionMapElement.AddColorMeter(dataCell);
                        break;
                    }
            }

            if (dataCell == null && !String.IsNullOrEmpty(oldLabel))
                StiElementChangedProcessor.ProcessElementChanging(regionMapElement, StiElementChangedArgs.CreateDeletingArgs(oldLabel));
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
                case "Key":
                    {
                        movingMeter = regionMapElement.GetKeyMeter();
                        if (!duplicateMeter)
                            regionMapElement.RemoveKeyMeter();
                        break;
                    }
                case "Name":
                    {
                        movingMeter = regionMapElement.GetNameMeter();
                        if (!duplicateMeter)
                            regionMapElement.RemoveNameMeter();
                        break;
                    }
                case "Value":
                    {
                        movingMeter = regionMapElement.GetValueMeter();
                        if (!duplicateMeter)
                            regionMapElement.RemoveValueMeter();
                        break;
                    }
                case "Group":
                    {
                        movingMeter = regionMapElement.GetGroupMeter();
                        if (!duplicateMeter)
                            regionMapElement.RemoveGroupMeter();
                        break;
                    }
                case "Color":
                    {
                        movingMeter = regionMapElement.GetColorMeter();
                        if (!duplicateMeter)
                            regionMapElement.RemoveColorMeter();
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
                    case "Key":
                        {
                            regionMapElement.AddKeyMeter(movingMeter);
                            break;
                        }
                    case "Name":
                        {
                            regionMapElement.AddNameMeter(movingMeter);
                            break;
                        }
                    case "Value":
                        {
                            regionMapElement.AddValueMeter(movingMeter);
                            break;
                        }
                    case "Group":
                        {
                            regionMapElement.AddGroupMeter(movingMeter);
                            break;
                        }
                    case "Color":
                        {
                            regionMapElement.AddColorMeter(movingMeter);
                            break;
                        }
                }
            }
            #endregion
        }

        private void SetProperties(Hashtable parameters, Hashtable callbackResult)
        {
            var props = parameters["properties"] as Hashtable;
            regionMapElement.MapIdent = props["mapID"] as string;
            regionMapElement.MapType = (StiMapType)Enum.Parse(typeof(StiMapType), props["mapType"] as string);
            regionMapElement.ShowValue = (bool)props["showValue"];
            regionMapElement.ColorEach = (bool)props["colorEach"];
            regionMapElement.ShowBubble = (bool)props["showBubble"];
            regionMapElement.ShowName = (StiDisplayNameType)Enum.Parse(typeof(StiDisplayNameType), props["displayNameType"] as string);
            regionMapElement.DataFrom = (StiMapSource)Enum.Parse(typeof(StiMapSource), props["dataFrom"] as string);
            regionMapElement.Language = props["language"] as string;

            if (parameters["updateMapData"] != null)
            {
                regionMapElement.MapData = null;
                callbackResult["mapData"] = GetMapDataForJS(regionMapElement);
            }
        }

        private void UpdateMapData(Hashtable parameters, Hashtable callbackResult)
        {
            List<StiMapData> mapData = regionMapElement.GetMapData().OrderBy(x => x.Key).ToList();
            var rowData = mapData[Convert.ToInt32(parameters["rowIndex"])];

            switch (parameters["columnName"] as string)
            {
                case "name":
                    rowData.Name = parameters["textValue"] as string;
                    break;

                case "value":
                    rowData.Value = parameters["textValue"] as string;
                    break;

                case "group":
                    rowData.Group = parameters["textValue"] as string;
                    break;

                case "color":
                    rowData.Color = parameters["textValue"] as string;
                    break;
            }

            regionMapElement.MapData = StiJsonHelper.SaveToJsonString(mapData);
        }

        internal static StiMap CreateMapComponentFromRegionMapElement(IStiRegionMapElement regionMapElement)
        {
            var mapComponent = new StiMap();
            mapComponent.MapIdent = regionMapElement.MapIdent;
            mapComponent.MapType = StiMapType.Individual;
            mapComponent.ShowValue = false;
            mapComponent.ColorEach = false;
            mapComponent.Stretch = true;
            mapComponent.DisplayNameType = StiDisplayNameType.None;
            mapComponent.DataFrom = StiMapSource.Manual;
            mapComponent.Page = regionMapElement.Page;
            mapComponent.Width = (regionMapElement as StiComponent).Width;
            mapComponent.Height = (regionMapElement as StiComponent).Height;
                       
            foreach (var style in StiOptions.Services.MapStyles.Where(x => x.AllowDashboard))
            {
                if (style.StyleIdent == regionMapElement.Style)
                {
                    mapComponent.MapStyle = style.StyleId;
                    break;
                }
            }

            return mapComponent;
        }

        private static List<StiMapStyle> GetRegionMapStyles(StiReport report)
        {
            var mapStyles = new List<StiMapStyle>();

            foreach (StiBaseStyle style in report.Styles)
            {
                if (style is StiMapStyle)
                    mapStyles.Add(style as StiMapStyle);
            }

            mapStyles.AddRange(StiOptions.Services.MapStyles.OrderBy(x => x.GetType() != typeof(StiMap29StyleFX)).Where(x => x.AllowDashboard));

            return mapStyles;
        }

        public static ArrayList GetStylesContent(StiReport report, Hashtable param)
        {
            ArrayList stylesContent = new ArrayList();
            var component = param["componentName"] != null ? report.Pages.GetComponentByName((string)param["componentName"]) as IStiRegionMapElement : null;

            if (component != null)
            {
                var mapComponent = CreateMapComponentFromRegionMapElement(component);
                var width = 130;
                var height = 50;

                foreach (var style in GetRegionMapStyles(report))
                {
                    Hashtable content = new Hashtable();

                    if (style is StiMapStyleFX)
                    {
                        content["ident"] = ((StiMapStyleFX)style).StyleIdent;
                        content["localizedName"] = style.DashboardName;
                        mapComponent.MapStyle = ((StiMapStyleFX)style).StyleId;
                        mapComponent.ComponentStyle = string.Empty;
                    }
                    else
                    {
                        content["name"] = style.Name;
                        content["ident"] = StiElementStyleIdent.Custom;
                        mapComponent.ComponentStyle = style.Name;
                    }
                    
                    content["image"] = StiMapHelper.GetMapSampleImage(mapComponent, width, height, 1f);
                    content["width"] = width;
                    content["height"] = height;
                    stylesContent.Add(content);
                }
            }

            return stylesContent;
        }
        #endregion

        #region Constructor
        public StiRegionMapElementHelper(IStiRegionMapElement regionMapElement)
        {
            this.regionMapElement = regionMapElement;
        }
        #endregion   
    }
}
