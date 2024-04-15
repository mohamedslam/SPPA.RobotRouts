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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Stimulsoft.Report.Export;
using Stimulsoft.Report.Maps;
using System.Linq;
using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using System.Drawing;
using Stimulsoft.Report.Toolbox;

namespace Stimulsoft.Report.Web
{
    internal class StiMapHelper
    {
        public static Hashtable GetMapProperties(StiMap map)
        {
            Hashtable properties = new Hashtable();
            properties["name"] = map.Name;
            properties["mapMode"] = map.MapMode;
            properties["mapType"] = map.MapType;
            properties["mapID"] = map.MapIdent;
            properties["showValue"] = map.ShowValue;
            properties["displayNameType"] = map.DisplayNameType;
            properties["mapData"] = GetMapDataForJS(map);
            properties["dataFrom"] = map.DataFrom;
            properties["colorEach"] = map.ColorEach;
            properties["language"] = map.Language;
            properties["languages"] = GetMapLanguages(map.MapIdent);

            properties["keyDataColumn"] = map.KeyDataColumn;
            properties["nameDataColumn"] = map.NameDataColumn;
            properties["valueDataColumn"] = map.ValueDataColumn;
            properties["groupDataColumn"] = map.GroupDataColumn;
            properties["colorDataColumn"] = map.ColorDataColumn;

            properties["latitudeDataColumn"] = map.Latitude;
            properties["longitudeDataColumn"] = map.Longitude;

            return properties;
        }

        internal static ArrayList GetMapLanguages(string mapIdent)
        {
            var langItems = new ArrayList();
            var mapsInfos = new List<StiMapToolboxInfo>();

            var items = StiToolboxHelper.GetMapToolboxItems();
            foreach (var item in items)
            {
                foreach (var info in item.Infos)
                {
                    if (mapsInfos.FirstOrDefault(x => x.MapID == info.MapID) == null)
                        mapsInfos.Add(info);
                }
            }

            var currInfo = mapsInfos.FirstOrDefault(x => x.MapID == mapIdent);
            if (currInfo != null && currInfo.Language != null)
            {
                foreach (var lang in currInfo.Language)
                {
                    langItems.Add(new Hashtable()
                    {
                        ["langKey"] = lang,
                        ["langName"] = currInfo.GetLangOriginalName(lang)
                    });
                }
            }

            return langItems;
        }

        public static void SetMapProperties(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            var map = report.Pages.GetComponentByName(param["componentName"] as string) as StiMap;
            if (map != null)
            {
                var props = param["properties"] as Hashtable;
                map.MapMode = (StiMapMode)Enum.Parse(typeof(StiMapMode), props["mapMode"] as string);
                map.MapIdent = props["mapID"] as string;
                map.MapType = (StiMapType)Enum.Parse(typeof(StiMapType), props["mapType"] as string);
                map.ShowValue = (bool)props["showValue"];
                map.ColorEach = (bool)props["colorEach"];
                map.DisplayNameType = (StiDisplayNameType)Enum.Parse(typeof(StiDisplayNameType), props["displayNameType"] as string);
                map.DataFrom = (StiMapSource)Enum.Parse(typeof(StiMapSource), props["dataFrom"] as string);
                map.Language = props["language"] as string;

                bool dataFromDataColumns = map.DataFrom == StiMapSource.DataColumns;
                map.KeyDataColumn = dataFromDataColumns ? props["keyDataColumn"] as string : string.Empty;
                map.NameDataColumn = dataFromDataColumns ? props["nameDataColumn"] as string : string.Empty;
                map.ValueDataColumn = dataFromDataColumns ? props["valueDataColumn"] as string : string.Empty;
                map.GroupDataColumn = dataFromDataColumns ? props["groupDataColumn"] as string : string.Empty;
                map.ColorDataColumn = dataFromDataColumns ? props["colorDataColumn"] as string : string.Empty;

                map.Latitude = props["latitudeDataColumn"] as string;
                map.Longitude = props["longitudeDataColumn"] as string;

                if (param["updateMapData"] != null)
                {
                    map.MapData = null;
                    callbackResult["mapData"] = GetMapDataForJS(map);
                }

                callbackResult["svgContent"] = StiReportEdit.GetSvgContent(map, StiReportEdit.StrToDouble(param["zoom"] as string));
                callbackResult["componentName"] = param["componentName"];
            }
        }

        public static void UpdateMapData(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            var map = report.Pages.GetComponentByName(param["componentName"] as string) as StiMap;
            if (map != null)
            {
                List<StiMapData> mapData = map.GetMapData().OrderBy(x => x.Key).ToList();
                if (param["rowIndex"] != null && param["columnName"] != null)
                {
                    var rowData = mapData[Convert.ToInt32(param["rowIndex"])];

                    switch (param["columnName"] as string)
                    {
                        case "name":
                            rowData.Name = param["textValue"] as string;
                            break;

                        case "value":
                            rowData.Value = param["textValue"] as string;
                            break;

                        case "group":
                            rowData.Group = param["textValue"] as string;
                            break;

                        case "color":
                            rowData.Color = param["textValue"] as string;
                            break;
                    }

                    map.MapData = StiJsonHelper.SaveToJsonString(mapData);
                }               

                callbackResult["svgContent"] = StiReportEdit.GetSvgContent(map, StiReportEdit.StrToDouble(param["zoom"] as string));
                callbackResult["componentName"] = param["componentName"];
            }
        }

        #region Helper methods
        public static ArrayList GetMapDataForJS(StiMap map)
        {
            ArrayList resultData = new ArrayList();
            List<StiMapData> mapData = map.GetMapData().OrderBy(x => x.Key).ToList();
            bool allowGroup = AllowGroup(map);
            bool allowColor = AllowColor(map);

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

        private static bool AllowGroup(StiMap map)
        {
            return (map.MapType == StiMapType.Group || map.MapType == StiMapType.HeatmapWithGroup);
        }

        private static bool AllowColor(StiMap map)
        {
            return (map.MapType == StiMapType.Individual);
        }
        #endregion

        #region Styles methods
        public static Hashtable GetStyle(StiMap map)
        {
            Hashtable style = new Hashtable();
            style["type"] = string.IsNullOrEmpty(map.ComponentStyle) ? "StiMapStyleIdent" : "StiMapStyle";
            style["name"] = string.IsNullOrEmpty(map.ComponentStyle) ? map.MapStyle.ToString() : map.ComponentStyle;

            return style;
        }

        private static List<StiMapStyle> GetMapStyles(StiReport report, bool withReportStyles = true)
        {
            var mapStyles = new List<StiMapStyle>();

            if (withReportStyles)
            {
                foreach (StiBaseStyle style in report.Styles)
                {
                    if (style is StiMapStyle)
                        mapStyles.Add(style as StiMapStyle);
                }
            }

            mapStyles.AddRange(StiOptions.Services.MapStyles);

            return mapStyles;
        }

        public static string GetMapSampleImage(StiMap map, int width, int height, float zoom)
        {
            var svgData = new StiSvgData()
            {
                X = 0,
                Y = 0,
                Width = width,
                Height = height,
                Component = map
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

                StiMapSvgHelper.DrawMap(writer, map, 0, 0, width, height, false);

                writer.WriteFullEndElement();
                writer.Flush();
                ms.Flush();
                writer.Close();
                ms.Close();
            }

            return sb.ToString();
        }

        public static void SetMapStyle(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            var component = report.Pages.GetComponentByName((string)param["componentName"]);
            string styleType = (string)param["styleType"];
            string styleName = (string)param["styleName"];

            if (component == null) return;
            StiMap map = component as StiMap;
            
            if (styleType == "StiMapStyle")
            {
                map.ComponentStyle = styleName;
            }
            else
            {
                map.ComponentStyle = string.Empty;
                map.MapStyle = (StiMapStyleIdent)Enum.Parse(typeof(StiMapStyleIdent), styleName);
            }
        }

        public static void GetStylesContent(StiReport report, Hashtable param, Hashtable callbackResult, bool withReportStyles = true)
        {
            var component = report.Pages.GetComponentByName(param["componentName"] as string);
            var mapCloned = component != null ? component.Clone() as StiMap : new StiMap();

            mapCloned.ColorEach = false;
            mapCloned.Stretch = true;
            mapCloned.DataFrom = StiMapSource.Manual;
            mapCloned.ShortValue = false;
            mapCloned.DisplayNameType = StiDisplayNameType.None;
            mapCloned.Brush = new StiSolidBrush(Color.Transparent);
            mapCloned.MapType = StiMapType.Individual;

            var stylesContent = new ArrayList();

            if (mapCloned != null)
            {
                foreach (var style in GetMapStyles(report, withReportStyles))
                {
                    if (style is StiMapStyleFX)
                    {
                        mapCloned.MapStyle = ((StiMapStyleFX)style).StyleId;
                        mapCloned.ComponentStyle = string.Empty;
                    }
                    else {

                        mapCloned.ComponentStyle = style.Name;
                    }

                    Hashtable content = new Hashtable();
                    var width = 125;
                    var height = 50;
                    content["image"] = GetMapSampleImage(mapCloned, width, height, 1f);
                    content["type"] = style is StiMapStyleFX ? "StiMapStyleIdent" : "StiMapStyle";
                    content["name"] = style is StiMapStyleFX ? mapCloned.MapStyle.ToString() : mapCloned.ComponentStyle;
                    content["width"] = width;
                    content["height"] = height;
                    stylesContent.Add(content);
                }
            }

            callbackResult["stylesContent"] = stylesContent;
        }
        #endregion
    }
}