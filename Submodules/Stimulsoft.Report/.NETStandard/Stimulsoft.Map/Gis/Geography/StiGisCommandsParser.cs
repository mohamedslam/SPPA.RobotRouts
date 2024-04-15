#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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
using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Map.Gis.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace Stimulsoft.Map.Gis.Geography
{
    internal sealed class StiGisCommandsParser
    {
        public StiGisCommandsParser(List<string> commands, List<string> colors, List<object> lineSizes, 
            List<string> descriptions, StiGisMapViewData viewData)
        {
            this.commands = commands;
            this.colors = colors;
            this.lineSizes = lineSizes;
            this.descriptions = descriptions;

            this.viewData = viewData;
            this.isGdiMode = viewData.RenderMode == StiGeoRenderMode.Gdi;
        }

        #region classes
        private class StiGeoJsonObject
        {
            public string type = null;
            public List<List<object>> coordinates = new List<List<object>>();
        }

        private class StiGeoJsonSimpleObject
        {
            public string type = null;
            public List<double> coordinates = new List<double>();
        }
        #endregion

        #region Fields
        private List<string> commands;
        private List<string> colors;
        private List<object> lineSizes;
        private List<string> descriptions;
        private StiGisMapViewData viewData;
        private StiGisContainerMapGeometry currentContainer;
        private bool isGdiMode;
        private int currentParseCommandIndex = -1;
        #endregion

        #region Methods
        public void Parse()
        {
            var currentCulture = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

                foreach (var command in commands)
                {
                    currentParseCommandIndex++;
                    if (string.IsNullOrEmpty(command)) continue;

                    try
                    {
                        if (command.StartsWith("{") && command.EndsWith("}"))
                        {
                            ParseGeoJsonCommand(command);
                        }
                        else
                        {
                            ParseCommand(command.ToUpperInvariant());
                        }
                    }
                    catch { }
                }
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
        }

        private double? ParseLineSize(object value)
        {
            try
            {
                if (value == null) return null;
                if (value is string)
                {
                    double result;
                    if (double.TryParse(value as string, out result))
                        return result;
                }

                return Convert.ToDouble(value);
            }
            catch { }

            return null;
        }

        private Color? ParseGdiColor(string color)
        {
            try
            {
                if (!string.IsNullOrEmpty(color))
                {
                    if (color.StartsWithInvariant("#"))
                        return ColorTranslator.FromHtml(color);
                    else
                        return Color.FromName(color);
                }
            }
            catch { }

            return null;
        }

        private System.Windows.Media.Color? ParseWpfColor(string color)
        {
            try
            {
                if (!string.IsNullOrEmpty(color))
                {
                    if (color.StartsWithInvariant("#"))
                        return Extensions.ToWpfColor(ColorTranslator.FromHtml(color));
                    else
                        return Extensions.ToWpfColor(Color.FromName(color));
                }
            }
            catch { }

            return null;
        }

        private void ParseGeoJsonCommand(string command)
        {
            #region MultiPolygon
            if (command.Contains("\"MultiPolygon\""))
            {
                var json = JsonConvert.DeserializeObject<StiGeoJsonObject>(command);
                var resGeom = new StiGisMultiPolygonMapGeometry();

                foreach (var obj in json.coordinates)
                {
                    var points = new List<StiGisPointLatLng>();
                    foreach (JArray array in obj)
                    {
                        foreach (var token in array.Children())
                        {
                            var obj1 = token.Children().ToArray();
                            if (obj1.Length == 2)
                            {
                                var point = new StiGisPointLatLng(obj1[1].Value<double>(), obj1[0].Value<double>());
                                points.Add(point);
                            }
                        }
                    }

                    var geom = new StiGisPolygonMapGeometry(points);
                    resGeom.Geoms.Add(geom);
                }

                if (colors != null)
                {
                    if (this.isGdiMode)
                    {
                        resGeom.ColorGdi = ParseGdiColor(colors[currentParseCommandIndex]);
                    }
                    else
                    {
                        resGeom.ColorWpf = ParseWpfColor(colors[currentParseCommandIndex]);
                    }
                }
                if (lineSizes != null)
                {
                    resGeom.LineSize = ParseLineSize(lineSizes[currentParseCommandIndex]);
                }

                viewData.Geoms.Add(resGeom);
                return;
            }
            #endregion

            #region MultiLineString
            if (command.Contains("\"MultiLineString\""))
            {
                var json = JsonConvert.DeserializeObject<StiGeoJsonObject>(command);
                var resGeom = new StiGisMultiLineStringMapGeometry();

                foreach (var obj in json.coordinates)
                {
                    var points = new List<StiGisPointLatLng>();
                    foreach (JArray array in obj)
                    {
                        foreach (var token in array.Children())
                        {
                            var obj1 = token.Children().ToArray();
                            if (obj1.Length == 2)
                            {
                                var point = new StiGisPointLatLng(obj1[1].Value<double>(), obj1[0].Value<double>());
                                points.Add(point);
                            }
                        }
                    }

                    var geom = new StiGisLineStringMapGeometry(points);
                    resGeom.Geoms.Add(geom);
                }

                if (colors != null)
                {
                    if (this.isGdiMode)
                    {
                        resGeom.ColorGdi = ParseGdiColor(colors[currentParseCommandIndex]);
                    }
                    else
                    {
                        resGeom.ColorWpf = ParseWpfColor(colors[currentParseCommandIndex]);
                    }
                }
                if (lineSizes != null)
                {
                    resGeom.LineSize = ParseLineSize(lineSizes[currentParseCommandIndex]);
                }

                viewData.Geoms.Add(resGeom);
                return;
            }
            #endregion

            #region MultiPoint
            if (command.Contains("\"MultiPoint\""))
            {
                var json = JsonConvert.DeserializeObject<StiGeoJsonObject>(command);
                var resGeom = new StiGisMultiPointMapGeometry();

                foreach (var obj in json.coordinates)
                {
                    var point = new StiGisPointLatLng(Convert.ToDouble(obj[1]), Convert.ToDouble(obj[0]));
                    var geom = new StiGisPointMapGeometry(point);
                    resGeom.Geoms.Add(geom);
                }

                if (colors != null)
                {
                    if (this.isGdiMode)
                    {
                        resGeom.ColorGdi = ParseGdiColor(colors[currentParseCommandIndex]);
                    }
                    else
                    {
                        resGeom.ColorWpf = ParseWpfColor(colors[currentParseCommandIndex]);
                    }
                }
                if (lineSizes != null)
                {
                    resGeom.LineSize = ParseLineSize(lineSizes[currentParseCommandIndex]);
                }

                viewData.Geoms.Add(resGeom);
                return;
            }
            #endregion

            #region Polygon
            if (command.Contains("\"Polygon\""))
            {
                var json = JsonConvert.DeserializeObject<StiGeoJsonObject>(command);

                var points = new List<StiGisPointLatLng>();
                if (json.coordinates.Count > 0)
                {
                    foreach (JArray array in json.coordinates[0])
                    {
                        var obj = array.Children().ToArray();
                        if (obj.Length == 2)
                        {
                            var point = new StiGisPointLatLng(obj[1].Value<double>(), obj[0].Value<double>());
                            points.Add(point);
                        }
                    }
                }

                var resGeom = new StiGisPolygonMapGeometry(points);
                if (colors != null)
                {
                    if (this.isGdiMode)
                    {
                        resGeom.ColorGdi = ParseGdiColor(colors[currentParseCommandIndex]);
                    }
                    else
                    {
                        resGeom.ColorWpf = ParseWpfColor(colors[currentParseCommandIndex]);
                    }
                }
                if (lineSizes != null)
                {
                    resGeom.LineSize = ParseLineSize(lineSizes[currentParseCommandIndex]);
                }

                viewData.Geoms.Add(resGeom);
                return;
            }
            #endregion

            #region LineString
            if (command.Contains("\"LineString\""))
            {
                var json = JsonConvert.DeserializeObject<StiGeoJsonObject>(command);

                var points = new List<StiGisPointLatLng>();
                if (json.coordinates.Count > 0)
                {
                    foreach (List<object> array in json.coordinates)
                    {
                        if (array.Count == 2)
                        {
                            var point = new StiGisPointLatLng((double)array[1], (double)array[0]);
                            points.Add(point);
                        }
                    }
                } 

                var resGeom = new StiGisLineStringMapGeometry(points);
                if (colors != null)
                {
                    if (this.isGdiMode)
                    {
                        resGeom.ColorGdi = ParseGdiColor(colors[currentParseCommandIndex]);
                    }
                    else
                    {
                        resGeom.ColorWpf = ParseWpfColor(colors[currentParseCommandIndex]);
                    }
                }
                if (lineSizes != null)
                {
                    resGeom.LineSize = ParseLineSize(lineSizes[currentParseCommandIndex]);
                }

                viewData.Geoms.Add(resGeom);
                return;
            }
            #endregion

            #region Point
            if (command.Contains("\"Point\""))
            {
                var json = JsonConvert.DeserializeObject<StiGeoJsonSimpleObject>(command);

                var resGeom = new StiGisPointMapGeometry(new StiGisPointLatLng(json.coordinates[1], json.coordinates[0]));
                if (colors != null)
                {
                    if (this.isGdiMode)
                    {
                        resGeom.ColorGdi = ParseGdiColor(colors[currentParseCommandIndex]);
                    }
                    else
                    {
                        resGeom.ColorWpf = ParseWpfColor(colors[currentParseCommandIndex]);
                    }
                }
                if (lineSizes != null)
                {
                    resGeom.LineSize = ParseLineSize(lineSizes[currentParseCommandIndex]);
                }
                if (descriptions != null)
                {
                    resGeom.SetDescription(descriptions[currentParseCommandIndex]);
                    if (resGeom.ShowPlacemark)
                        viewData.Placemarks.Add(resGeom);
                }

                viewData.Geoms.Add(resGeom);
                return;
            }
            #endregion
        }

        private void ParseCommand(string command)
        {
            if (command.StartsWith("POINT"))
            {
                ParsePoint(command);
            }
            else if (command.StartsWith("POLYGON"))
            {
                ParsePolygon(command);
            }
            else if (command.StartsWith("LINESTRING"))
            {
                ParseLineString(command);
            }
            else if (command.StartsWith("CIRCULARSTRING"))
            {
                ParseCircularString(command);
            }
            else if (command.StartsWith("COMPOUNDCURVE"))
            {
                ParseCompoundCurve(command);
            }
            else if (command.StartsWith("CURVEPOLYGON"))
            {
                ParseCurvePolygon(command);
            }
            else if (command.StartsWith("MULTIPOINT"))
            {
                ParseMultiPoint(command);
            }
            else if (command.StartsWith("MULTILINESTRING"))
            {
                ParseMultiLineString(command);
            }
            else if (command.StartsWith("MULTIPOLYGON"))
            {
                ParseMultiPolygon(command);
            }
            else if (command.StartsWith("GEOMETRYCOLLECTION"))
            {
                ParseGeomertyCollection(command);
            }
        }

        private int ParsePoint(string command)
        {
            int startIndex = "POINT".Length;
            int lastIndex = command.IndexOf(')', startIndex);

            var text = command.Substring(startIndex, lastIndex - startIndex + 1);

            var list = ParsePoints(text);
            if (list.Count != 2)
                throw new StiGisParserExeption(command);

            var geom = new StiGisPointMapGeometry(list[1], list[0]);

            if (currentContainer != null)
            {
                this.currentContainer.Geoms.Add(geom);
            }
            else
            {
                if (colors != null)
                {
                    if (this.isGdiMode)
                    {
                        geom.ColorGdi = ParseGdiColor(colors[currentParseCommandIndex]);
                    }
                    else
                    {
                        geom.ColorWpf = ParseWpfColor(colors[currentParseCommandIndex]);
                    }
                }
                if (lineSizes != null)
                {
                    geom.LineSize = ParseLineSize(lineSizes[currentParseCommandIndex]);
                }
                if (descriptions != null)
                {
                    geom.SetDescription(descriptions[currentParseCommandIndex]);
                    if (geom.ShowPlacemark)
                        viewData.Placemarks.Add(geom);
                }
                this.viewData.Geoms.Add(geom);
            }

            return lastIndex + 1;
        }

        private int ParsePolygon(string command)
        {
            if (command.StartsWith("POLYGON EMPTY"))
                return "POLYGON EMPTY".Length + 1;

            int startIndex = "POLYGON".Length + 1;
            int lastIndex = command.IndexOf(')', startIndex);

            var text1 = command.Substring(startIndex, lastIndex - startIndex + 1);

            var list1 = ParsePoints(text1);
            if (list1.Count == 0) return lastIndex + 1;

            StiGisPolygonMapGeometry geom = null;
            int resultIndex;

            lastIndex++;
            // затем проверяем есть ли clip у этого объекта
            int lastIndex1 = command.IndexOf(',', lastIndex);
            int lastIndex2 = command.IndexOf('(', lastIndex);
            int lastIndex3 = command.IndexOf(')', lastIndex);

            if (lastIndex1 != -1 && lastIndex2 != -1 && lastIndex1 < lastIndex3 && lastIndex2 > lastIndex1 && lastIndex2 < lastIndex3)
            {
                resultIndex = lastIndex3 + 1;
                var text2 = command.Substring(lastIndex2, lastIndex3 - lastIndex2 + 1);

                var list2 = ParsePoints(text2);
                if (list2.Count == 0)
                    throw new StiGisParserExeption(command);

                geom = new StiGisPolygonMapGeometry(GetPoints(list1), GetPoints(list2));
            }
            else
            {
                resultIndex = lastIndex + 1;
            }

            if (geom == null)
                geom = new StiGisPolygonMapGeometry(GetPoints(list1));

            if (colors != null)
            {
                if (this.isGdiMode)
                {
                    geom.ColorGdi = ParseGdiColor(colors[currentParseCommandIndex]);
                }
                else
                {
                    geom.ColorWpf = ParseWpfColor(colors[currentParseCommandIndex]);
                }
            }
            if (lineSizes != null)
            {
                geom.LineSize = ParseLineSize(lineSizes[currentParseCommandIndex]);
            }
            this.viewData.Geoms.Add(geom);

            return resultIndex;
        }

        private int ParsePolygonWithoutName(string command, int startIndex)
        {
            int lastIndex = command.IndexOf(')', startIndex);

            var text1 = command.Substring(startIndex, lastIndex - startIndex + 1);

            var list1 = ParsePoints(text1);
            if (list1.Count == 0) return lastIndex + 1;

            StiGisPolygonMapGeometry geom = null;
            int resultIndex;

            lastIndex++;
            // затем проверяем есть ли clip у этого объекта
            int lastIndex1 = command.IndexOf(',', lastIndex);
            int lastIndex2 = command.IndexOf('(', lastIndex);
            int lastIndex3 = command.IndexOf(')', lastIndex);

            if (lastIndex1 != -1 && lastIndex2 != -1 && lastIndex1 < lastIndex3 && lastIndex2 > lastIndex1 && lastIndex2 < lastIndex3)
            {
                resultIndex = lastIndex3 + 2;
                var text2 = command.Substring(lastIndex2, lastIndex3 - lastIndex2 + 1);

                var list2 = ParsePoints(text2);
                if (list2.Count == 0)
                    throw new StiGisParserExeption(command);

                geom = new StiGisPolygonMapGeometry(GetPoints(list1), GetPoints(list2));
            }
            else
            {
                resultIndex = lastIndex + 1;
            }

            if (geom == null)
                geom = new StiGisPolygonMapGeometry(GetPoints(list1));

            this.currentContainer.Geoms.Add(geom);

            return resultIndex;
        }

        private int ParseLineString(string command)
        {
            if (command.StartsWith("LINESTRING EMPTY"))
                return "LINESTRING EMPTY".Length;

            int startIndex = "LINESTRING".Length;
            int lastIndex = command.IndexOf(')', startIndex);

            var text = command.Substring(startIndex, lastIndex - startIndex + 1);

            var list = ParsePoints(text);
            if (list.Count != 0)
            {
                var geom = new StiGisLineStringMapGeometry(GetPoints(list));
                if (colors != null)
                {
                    if (this.isGdiMode)
                    {
                        geom.ColorGdi = ParseGdiColor(colors[currentParseCommandIndex]);
                    }
                    else
                    {
                        geom.ColorWpf = ParseWpfColor(colors[currentParseCommandIndex]);
                    }
                }
                if (lineSizes != null)
                {
                    geom.LineSize = ParseLineSize(lineSizes[currentParseCommandIndex]);
                }

                this.viewData.Geoms.Add(geom);
            }

            return lastIndex + 1;
        }

        private int ParseCircularString(string command)
        {
            if (command.StartsWith("CIRCULARSTRING EMPTY"))
                return "CIRCULARSTRING EMPTY".Length;

            int startIndex = "CIRCULARSTRING".Length;
            int lastIndex = command.IndexOf(')', startIndex);

            var text = command.Substring(startIndex, lastIndex - startIndex + 1);

            var list = ParsePoints(text);
            if (list.Count != 0)
            {
                var geom = new StiGisCircularStringMapGeometry(GetPoints(list));
                if (colors != null)
                {
                    if (this.isGdiMode)
                    {
                        geom.ColorGdi = ParseGdiColor(colors[currentParseCommandIndex]);
                    }
                    else
                    {
                        geom.ColorWpf = ParseWpfColor(colors[currentParseCommandIndex]);
                    }
                }
                if (lineSizes != null)
                {
                    geom.LineSize = ParseLineSize(lineSizes[currentParseCommandIndex]);
                }

                this.viewData.Geoms.Add(geom);
            }

            return lastIndex + 1;
        }

        private int ParseCompoundCurve(string command)
        {
            if (command.StartsWith("COMPOUNDCURVE  EMPTY")) return "COMPOUNDCURVE  EMPTY".Length;

            int index = "COMPOUNDCURVE".Length;
            var geom = new StiGisCompoundCurveMapGeometry();

            int start = 0;
            while (true)
            {
                var symbol = command[index];

                if (symbol == '(')
                {
                    if (start == 0)
                    {
                        start++;
                    }
                    else
                    {
                        // LineString
                        int lastIndex = command.IndexOf(')', index);
                        if (lastIndex == -1)
                            throw new StiGisParserExeption(command);

                        var text1 = command.Substring(index, lastIndex - index + 1);
                        var list1 = ParsePoints(text1);

                        if (list1.Count > 0)
                        {
                            var geom1 = new StiGisLineStringMapGeometry(GetPoints(list1));
                            if (this.currentContainer != null)
                                this.currentContainer.Geoms.Add(geom1);
                            else
                                geom.Geoms.Add(geom1);
                        }

                        index = lastIndex + 1;
                        continue;
                    }
                }
                else if (symbol == ',' || symbol == ' ')
                {

                }
                else if (symbol == 'C')
                {
                    var text1 = command.Substring(index);
                    if (text1.StartsWith("CIRCULARSTRING"))
                    {
                        var index1 = text1.IndexOf('(');
                        var index2 = text1.IndexOf(')', index1 + 1);

                        if (index1 == -1 || index2 == -1 || index2 < index1)
                            throw new StiGisParserExeption(command);

                        var text2 = text1.Substring(index1, index2 - index1 + 1);

                        var list2 = ParsePoints(text2);
                        if (list2.Count != 0)
                        {
                            var geom1 = new StiGisCircularStringMapGeometry(GetPoints(list2));
                            if (this.currentContainer != null)
                                this.currentContainer.Geoms.Add(geom1);
                            else
                                geom.Geoms.Add(geom1);
                        }

                        index += index2 + 1;
                        continue;
                    }
                    else
                    {
                        throw new StiGisParserExeption(command);
                    }
                }
                else if (symbol == ')')
                {
                    if (start == 1)
                        break;

                    throw new StiGisParserExeption(command);
                }
                else
                {
                    throw new StiGisParserExeption(command);
                }

                index++;
            }

            if (colors != null)
            {
                if (this.isGdiMode)
                {
                    geom.ColorGdi = ParseGdiColor(colors[currentParseCommandIndex]);
                }
                else
                {
                    geom.ColorWpf = ParseWpfColor(colors[currentParseCommandIndex]);
                }
            }
            if (lineSizes != null)
            {
                geom.LineSize = ParseLineSize(lineSizes[currentParseCommandIndex]);
            }
            this.viewData.Geoms.Add(geom);

            return index + 1;
        }

        private void ParseCurvePolygon(string command)
        {
            if (command.StartsWith("CURVEPOLYGON  EMPTY")) return;

            int index = "CURVEPOLYGON".Length;
            var geom = new StiGisCompoundCurveMapGeometry();

            int start = 0;
            while (true)
            {
                var symbol = command[index];

                if (symbol == '(')
                {
                    if (start == 0)
                    {
                        start++;
                    }
                    else
                    {
                        // LineString
                        int lastIndex = command.IndexOf(')', index);
                        if (lastIndex == -1)
                            throw new StiGisParserExeption(command);

                        var text1 = command.Substring(index, lastIndex - index + 1);
                        var list1 = ParsePoints(text1);

                        if (list1.Count > 0)
                        {
                            var geom1 = new StiGisLineStringMapGeometry(GetPoints(list1));
                            geom.Geoms.Add(geom1);
                        }

                        index = lastIndex + 1;
                        continue;
                    }
                }
                else if (symbol == ',' || symbol == ' ')
                {

                }
                else if (symbol == 'C')
                {
                    var text1 = command.Substring(index);
                    if (text1.StartsWith("CIRCULARSTRING"))
                    {
                        var index1 = text1.IndexOf('(');
                        var index2 = text1.IndexOf(')', index1 + 1);

                        if (index1 == -1 || index2 == -1 || index2 < index1)
                            throw new StiGisParserExeption(command);

                        var text2 = text1.Substring(index1, index2 - index1 + 1);

                        var list2 = ParsePoints(text2);
                        if (list2.Count != 0)
                        {
                            var geom1 = new StiGisCircularStringMapGeometry(GetPoints(list2));
                            geom.Geoms.Add(geom1);
                        }

                        index += index2 + 1;
                        continue;
                    }
                    else if (text1.StartsWith("COMPOUNDCURVE"))
                    {
                        this.currentContainer = geom;

                        var offset = ParseCompoundCurve(text1);
                        index += offset;

                        this.currentContainer = null;

                        continue;
                    }
                    else
                    {
                        throw new StiGisParserExeption(command);
                    }
                }
                else if (symbol == ')')
                {
                    if (start == 1)
                        break;

                    throw new StiGisParserExeption(command);
                }
                else
                {
                    throw new StiGisParserExeption(command);
                }

                index++;
            }

            if (colors != null)
            {
                if (this.isGdiMode)
                {
                    geom.ColorGdi = ParseGdiColor(colors[currentParseCommandIndex]);
                }
                else
                {
                    geom.ColorWpf = ParseWpfColor(colors[currentParseCommandIndex]);
                }
            }
            if (lineSizes != null)
            {
                geom.LineSize = ParseLineSize(lineSizes[currentParseCommandIndex]);
            }

            this.viewData.Geoms.Add(geom);
        }

        private void ParseMultiPoint(string command)
        {
            if (command.StartsWith("MULTIPOINT  EMPTY")) return;

            int index = "MULTIPOINT".Length;
            var geom = new StiGisCompoundCurveMapGeometry();

            int start = 0;
            while (true)
            {
                var symbol = command[index];

                if (symbol == '(')
                {
                    if (start == 0)
                    {
                        start++;
                    }
                    else
                    {
                        // POINT
                        int lastIndex = command.IndexOf(')', index);
                        if (lastIndex == -1)
                            throw new StiGisParserExeption(command);

                        var text1 = command.Substring(index, lastIndex - index + 1);
                        var list1 = ParsePoints(text1);

                        if (list1.Count == 2)
                        {
                            var geom1 = new StiGisPointMapGeometry(list1[1], list1[0]);
                            geom.Geoms.Add(geom1);
                        }

                        index = lastIndex + 1;
                        continue;
                    }
                }
                else if (symbol == ',' || symbol == ' ')
                {

                }
                else if (symbol == ')')
                {
                    if (start == 1)
                        break;

                    throw new StiGisParserExeption(command);
                }
                else
                {
                    throw new StiGisParserExeption(command);
                }

                index++;
            }

            if (colors != null)
            {
                if (this.isGdiMode)
                {
                    geom.ColorGdi = ParseGdiColor(colors[currentParseCommandIndex]);
                }
                else
                {
                    geom.ColorWpf = ParseWpfColor(colors[currentParseCommandIndex]);
                }
            }
            if (lineSizes != null)
            {
                geom.LineSize = ParseLineSize(lineSizes[currentParseCommandIndex]);
            }

            this.viewData.Geoms.Add(geom);
        }

        private void ParseMultiLineString(string command)
        {
            if (command.StartsWith("MULTILINESTRING  EMPTY")) return;

            int index = "MULTILINESTRING".Length;
            var geom = new StiGisMultiLineStringMapGeometry();

            int start = 0;
            while (true)
            {
                var symbol = command[index];

                if (symbol == '(')
                {
                    if (start == 0)
                    {
                        start++;
                    }
                    else
                    {
                        // LINESTRING
                        int lastIndex = command.IndexOf(')', index);
                        if (lastIndex == -1)
                            throw new StiGisParserExeption(command);

                        var text1 = command.Substring(index, lastIndex - index + 1);
                        var list1 = ParsePoints(text1);

                        if (list1.Count > 0)
                        {
                            var geom1 = new StiGisLineStringMapGeometry(GetPoints(list1));
                            geom.Geoms.Add(geom1);
                        }

                        index = lastIndex + 1;
                        continue;
                    }
                }
                else if (symbol == ',' || symbol == ' ')
                {

                }
                else if (symbol == ')')
                {
                    if (start == 1)
                        break;

                    throw new StiGisParserExeption(command);
                }
                else
                {
                    throw new StiGisParserExeption(command);
                }

                index++;
            }

            if (colors != null)
            {
                if (this.isGdiMode)
                {
                    geom.ColorGdi = ParseGdiColor(colors[currentParseCommandIndex]);
                }
                else
                {
                    geom.ColorWpf = ParseWpfColor(colors[currentParseCommandIndex]);
                }
            }
            if (lineSizes != null)
            {
                geom.LineSize = ParseLineSize(lineSizes[currentParseCommandIndex]);
            }

            this.viewData.Geoms.Add(geom);
        }

        private void ParseMultiPolygon(string command)
        {
            if (command.StartsWith("MULTIPOLYGON  EMPTY")) return;

            int index = "MULTIPOLYGON".Length;
            var geom = new StiGisMultiPolygonMapGeometry();

            int start = 0;
            while (true)
            {
                var symbol = command[index];

                if (symbol == '(')
                {
                    if (start == 0)
                    {
                        start++;
                    }
                    else
                    {
                        // POLYGON
                        int lastIndex = command.IndexOf(')', index);
                        if (lastIndex == -1)
                            throw new StiGisParserExeption(command);

                        this.currentContainer = geom;
                        int offset = ParsePolygonWithoutName(command, index);
                        index = offset;
                        this.currentContainer = null;

                        continue;
                    }
                }
                else if (symbol == ',' || symbol == ' ')
                {

                }
                else if (symbol == ')')
                {
                    if (start == 1)
                        break;

                    throw new StiGisParserExeption(command);
                }
                else
                {
                    throw new StiGisParserExeption(command);
                }

                index++;
            }

            if (colors != null)
            {
                if (this.isGdiMode)
                {
                    geom.ColorGdi = ParseGdiColor(colors[currentParseCommandIndex]);
                }
                else
                {
                    geom.ColorWpf = ParseWpfColor(colors[currentParseCommandIndex]);
                }
            }
            if (lineSizes != null)
            {
                geom.LineSize = ParseLineSize(lineSizes[currentParseCommandIndex]);
            }

            this.viewData.Geoms.Add(geom);
        }

        private void ParseGeomertyCollection(string command)
        {
            if (command.StartsWith("GEOMETRYCOLLECTION  EMPTY")) return;

            int index = "GEOMETRYCOLLECTION".Length;
            var geom = new StiGisGeomertyCollectionMapGeometry();

            int start = 0;
            while (true)
            {
                var symbol = command[index];

                if (symbol == '(')
                {
                    if (start == 0)
                    {
                        start++;
                    }
                    else
                    {
                        throw new StiGisParserExeption(command);
                    }
                }
                else if (symbol == ',' || symbol == ' ')
                {

                }
                else if (symbol == 'P')
                {
                    var text1 = command.Substring(index);
                    if (text1.StartsWith("POINT"))
                    {
                        this.currentContainer = geom;

                        var offset = ParsePoint(text1);
                        index += offset;

                        this.currentContainer = null;

                        continue;
                    }
                    else if (text1.StartsWith("POLYGON"))
                    {
                        this.currentContainer = geom;

                        var offset = ParsePolygon(text1);
                        index += offset;

                        this.currentContainer = null;

                        continue;
                    }
                    else
                    {
                        throw new StiGisParserExeption(command);
                    }
                }
                else if (symbol == 'L')
                {
                    var text1 = command.Substring(index);
                    if (text1.StartsWith("LINESTRING"))
                    {
                        this.currentContainer = geom;

                        var offset = ParseLineString(text1);
                        index += offset;

                        this.currentContainer = null;

                        continue;
                    }
                    else
                    {
                        throw new StiGisParserExeption(command);
                    }
                }
                else if (symbol == 'C')
                {
                    var text1 = command.Substring(index);
                    if (text1.StartsWith("CIRCULARSTRING"))
                    {
                        this.currentContainer = geom;

                        var offset = ParseCircularString(text1);
                        index += offset;

                        this.currentContainer = null;

                        continue;
                    }
                    else
                    {
                        throw new StiGisParserExeption(command);
                    }
                }
                else if (symbol == ')')
                {
                    if (start == 1)
                        break;

                    throw new StiGisParserExeption(command);
                }
                else
                {
                    throw new StiGisParserExeption(command);
                }

                index++;
            }

            if (colors != null)
            {
                if (this.isGdiMode)
                {
                    geom.ColorGdi = ParseGdiColor(colors[currentParseCommandIndex]);
                }
                else
                {
                    geom.ColorWpf = ParseWpfColor(colors[currentParseCommandIndex]);
                }
            }
            if (lineSizes != null)
            {
                geom.LineSize = ParseLineSize(lineSizes[currentParseCommandIndex]);
            }

            this.viewData.Geoms.Add(geom);
        }
        #endregion

        #region Methods.Helpers
        private List<StiGisPointLatLng> GetPoints(List<double> list)
        {
            var result = new List<StiGisPointLatLng>();

            int index = 0;
            while (index < list.Count)
            {
                result.Add(new StiGisPointLatLng(list[index + 1], list[index]));
                index += 2;
            }

            return result;
        }

        private List<double> ParsePoints(string text)
        {
            var points = new List<double>();
            var builder = new StringBuilder();

            int count = 0;
            int index = 0;
            while (true)
            {
                var symbol = text[index];
                if (symbol == '(')
                {
                }
                else if (symbol == '-' || symbol == '.')
                {
                    builder.Append(symbol);
                }
                else if (symbol >= '0' && symbol <= '9')
                {
                    builder.Append(symbol);
                }
                else if (symbol == ' ')
                {
                    if (builder.Length > 0)
                    {
                        double point;
                        if (!double.TryParse(builder.ToString(), out point))
                            throw new StiGisParserExeption(text);

                        points.Add(point);
                        builder.Clear();
                        count++;
                    }
                }
                else if (symbol == ',')
                {
                    // В некоторых случаях можно использовать дополнительную третью Z координату, которая игнорируется при отрисовке
                    if (builder.Length > 0 && count < 2)
                    {
                        double point;
                        if (!double.TryParse(builder.ToString(), out point))
                            throw new StiGisParserExeption(text);

                        points.Add(point);
                    }

                    builder.Clear();
                    count = 0;
                }
                else if (symbol == ')')
                {
                    if (builder.Length > 0 && count < 2)
                    {
                        double point;
                        if (!double.TryParse(builder.ToString(), out point))
                            throw new StiGisParserExeption(text);

                        points.Add(point);
                    }

                    builder.Clear();

                    if (points.Count % 2 != 0)
                        throw new StiGisParserExeption(text);

                    return points;
                }
                else
                {
                    throw new StiGisParserExeption(text);
                }

                index++;
            }
        }
        #endregion
    }
}