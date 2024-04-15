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

using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Helpers;
using Stimulsoft.Report.Maps;
using Stimulsoft.Report.Maps.Helpers;
using Stimulsoft.Report.Painters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Drawing;

#if STIDRAWING
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using Font = Stimulsoft.Drawing.Font;
using Graphics = Stimulsoft.Drawing.Graphics;
using Bitmap = Stimulsoft.Drawing.Bitmap;
#endif

namespace Stimulsoft.Report.Export
{
    public static class StiMapSvgHelper
    {
        #region Methods
        public static void DrawMap(XmlTextWriter xmlsWriter, StiMap map, double x, double y, double width, double height, bool animated)
        {
            StiMapLoader.DeleteAllCustomMaps();
            StiCustomMapFinder.Clear();
            var resource = StiMapLoader.LoadResource(map.Report, map.MapIdent, map.Language);
            if (resource == null) return;

            var sScale = width / resource.Width < height / resource.Height ? width / resource.Width : height / resource.Height;
            var xPos = NormalizeDecimal((float)((width - resource.Width * sScale) / 2 + x));
            var yPos = NormalizeDecimal((float)((height - resource.Height * sScale) / 2 + y));

            if (!map.Stretch)
            {
                sScale = 1;
                xPos = yPos = "0";
            }

            xmlsWriter.WriteStartElement("rect");
            if (x != 0) xmlsWriter.WriteAttributeString("x", NormalizeDecimal(x));
            if (y != 0) xmlsWriter.WriteAttributeString("y", NormalizeDecimal(y));
            xmlsWriter.WriteAttributeString("width", NormalizeDecimal(width));
            xmlsWriter.WriteAttributeString("height", NormalizeDecimal(height));

            var color = ((StiSolidBrush)map.GetStyleBackground()).Color;

            if (map.Brush is StiSolidBrush && ((StiSolidBrush)map.Brush).Color != Color.Transparent)
            {
                color = ((StiSolidBrush)map.Brush).Color;
            }

            xmlsWriter.WriteAttributeString("style", $"fill:rgb({color.R},{color.G},{color.B});fill-opacity:{Math.Round(color.A / 255f, 3).ToString().Replace(",", ".")};");
            xmlsWriter.WriteEndElement();

            xmlsWriter.WriteStartElement("g");
            xmlsWriter.WriteAttributeString("transform", string.Format("translate({0},{1})", xPos, yPos));

            Render(map, xmlsWriter, animated, sScale);

            xmlsWriter.WriteEndElement();
        }

        public static void Render(StiMap map, XmlTextWriter xmlsWriter, bool animated, double sScale)
        {
            var painter = new StiGdiMapContextPainter(map);
            painter.MapStyle = StiMap.GetMapStyle(map);
            painter.DataTable = map.DataTable;
            painter.PrepareDataColumns();
            painter.UpdateGroupedData();
            painter.UpdateHeatmapWithGroup();

            var mapData = painter.MapData;

            StiMapStyle mapStyle = null;
            if (!string.IsNullOrEmpty(map.ComponentStyle))
                mapStyle = map.Report.Styles[map.ComponentStyle] as StiMapStyle;

            if (mapStyle == null)
                mapStyle = StiMap.GetMapStyle(map.MapStyle);

            painter.DefaultBrush = new SolidBrush(mapStyle.DefaultColor);
            painter.DefaultBrush1 = new StiSolidBrush(mapStyle.DefaultColor);

            var svgContainer = StiMapLoader.LoadResource(map.Report, map.MapIdent, map.Language);
            var labels = new Hashtable();
            using (var img = new Bitmap(1, 1))
            {
                using (var graphics = Graphics.FromImage(img))
                {
                    if (svgContainer != null)
                    {                        
                        var count = 0;
                        var sCount = svgContainer.HashPaths.Keys.Count;
                        var skipLabels = !((map.ShowValue || map.DisplayNameType != StiDisplayNameType.None) && sScale >= 0.1);
                        float individualStepValue = 0.5f / sCount;
                        painter.IndividualStep = individualStepValue;
                        var keys = new List<String>(svgContainer.HashPaths.Keys);
                        keys.Sort();
                        foreach (var key in keys)
                        {
                            var data = mapData[0];
                            foreach (var md in mapData)
                            {
                                if (md.Key == key)
                                    data = md;
                            }
                            var fill = painter.GetGeomBrush(data);
                            painter.IndividualStep += individualStepValue;
                            xmlsWriter.WriteStartElement("path");
                            xmlsWriter.WriteAttributeString("d", svgContainer.HashPaths[key].Data);

                            var style = new StringBuilder();
                            style.Append(GetFillBrush(fill as SolidBrush));

                            var stroke = $"{GetBorderStroke(mapStyle.BorderColor)}";
                            style.Append($"{stroke};stroke-width:{NormalizeDecimal(mapStyle.BorderSize)};");

                            xmlsWriter.WriteAttributeString("style", style.ToString());
                            xmlsWriter.WriteAttributeString("transform", $"scale({NormalizeDecimal(sScale)})");
                            xmlsWriter.WriteAttributeString("_ismap", "true");
                            xmlsWriter.WriteAttributeString("_text1", GetToolTipIdent(data));

                            var valueText = GetToolTipValueText(map, data.Value);
                            var totalText = GetToolTipTotalText(map, mapData, data);

                            if (!string.IsNullOrEmpty(totalText))
                                valueText = $"{StiLocalization.Get("FormFormatEditor", "BooleanValue").Replace(":", string.Empty)} = {valueText}<div style='margin-top: 5px;'>{totalText}<div>";

                            xmlsWriter.WriteAttributeString("_text2", valueText);
                            xmlsWriter.WriteAttributeString("elementargument", key);
                            xmlsWriter.WriteAttributeString("elementident", key);
                            xmlsWriter.WriteAttributeString("elementvalue", data.Value != null ? data.Value.ToString() : string.Empty);

                            AddToolTipStyle(xmlsWriter, mapStyle);

                            var color = (fill as SolidBrush).Color;
                            xmlsWriter.WriteAttributeString("_color", $"#{color.R:X2}{color.G:X2}{color.B:X2}");

                            if (animated)
                            {
                                xmlsWriter.WriteAttributeString("opacity", "0");
                                xmlsWriter.WriteAttributeString("_animation",
                                    string.Format("{{\"actions\":[[\"opacity\", 0, 1, \"\"], [\"scale\", {2}, {2},\"\"]], \"begin\":{0}, \"duration\":{1}}}",
                                    NormalizeDecimal(200 / sCount * count), "100", NormalizeDecimal(sScale)));
                            }
                            xmlsWriter.WriteEndElement();

                            if (!skipLabels)
                            {
                                labels[key] = data;
                            }
                            count++;
                        }

                        #region draw labels
                        if (labels.Keys.Count > 0)
                        {
                            float fontSize = 19;
                            if (svgContainer.TextScale != null)
                            {
                                fontSize *= (float)svgContainer.TextScale.GetValueOrDefault();
                                skipLabels = false;
                            }
                            var typeface = new Font("Calibri", fontSize);
                            var foregrounds = new Color[2] { Color.FromArgb(180, 251, 251, 251), Color.FromArgb(255, 37, 37, 37) };
                            foreach (string key in labels.Keys)
                            {
                                var path = svgContainer.HashPaths[key];
                                var skipText = false;
                                if (map.DisplayNameType == StiDisplayNameType.Short)
                                    skipText = path.SkipTextIso == null ? path.SkipText : path.SkipTextIso.GetValueOrDefault();
                                else
                                    skipText = path.SkipText;
                                if (skipText) continue;

                                var info = labels[key] as StiMapData;
                                var text = GetPathText(map, info, path, key);

                                if (!string.IsNullOrEmpty(text))
                                {
                                    var bounds = GetPathRect(map, path);
                                    var textSize = (map.DisplayNameType == StiDisplayNameType.Full && path.SetMaxWidth)
                                        ? graphics.MeasureString(text, typeface, bounds.Width)
                                        : graphics.MeasureString(text, typeface);

                                    #region Calc position
                                    float x = 0;
                                    float y = 0;

                                    switch (GetPathHorAlignment(map, path))
                                    {
                                        case StiTextHorAlignment.Left:
                                        case StiTextHorAlignment.Width:
                                            x = bounds.X;
                                            break;

                                        case StiTextHorAlignment.Right:
                                            x = bounds.Right - textSize.Width;
                                            break;

                                        case StiTextHorAlignment.Center:
                                            x = bounds.X + (bounds.Width - textSize.Width) / 2;
                                            break;
                                    }

                                    x += 25;

                                    switch (GetPathVertAlignment(map, path))
                                    {
                                        case StiVertAlignment.Top:
                                            y = bounds.Y;
                                            break;

                                        case StiVertAlignment.Bottom:
                                            y = bounds.Bottom - textSize.Height;
                                            break;

                                        case StiVertAlignment.Center:
                                            y = bounds.Y + (bounds.Height - textSize.Height) / 2;
                                            break;
                                    }

                                    y += 30;
                                    #endregion

                                    if (map.ShowValue && !string.IsNullOrEmpty(info?.Value) && text.Contains(Environment.NewLine))
                                        y += 8;

                                    for (var step = 0; step < 2; step++)
                                    {
                                        xmlsWriter.WriteStartElement("text");
                                        xmlsWriter.WriteAttributeString("font-size", NormalizeDecimal(typeface.Size * (float)sScale));
                                        xmlsWriter.WriteAttributeString("font-family", "Calibri");
                                        if (animated)
                                        {
                                            xmlsWriter.WriteAttributeString("opacity", "0");
                                            xmlsWriter.WriteAttributeString("_animation",
                                                string.Format("{{\"actions\":[[\"opacity\", 0, 1, \"\"], [\"scale\", {2}, {2},\"\"]], \"begin\":{0}, \"duration\":{1}}}",
                                                NormalizeDecimal(200 / sCount * count), "100", NormalizeDecimal(sScale)));
                                        }

                                        var shadowStyle = string.Empty;

                                        if (step == 1)
                                        {
                                            var shadowColor = string.Format("#{0:X2}{1:X2}{2:X2}", foregrounds[0].R, foregrounds[0].G, foregrounds[0].B);
                                            shadowStyle = string.Format("text-shadow: -1px -1px 1px {0},-1px 1px 1px {0},1px -1px 1px {0}, 1px 1px 1px {0};", shadowColor);
                                        }

                                        xmlsWriter.WriteAttributeString("transform", string.Format("translate({0}, {1})", NormalizeDecimal((x - step) * sScale), NormalizeDecimal((y - step) * sScale)));
                                        xmlsWriter.WriteAttributeString("style", string.Format("fill:#{0:X2}{1:X2}{2:X2}; pointer-events:none;{3}", foregrounds[step].R, foregrounds[step].G, foregrounds[step].B, shadowStyle));
                                        xmlsWriter.WriteAttributeString("font-weight", "bold");

                                        if (path.SetMaxWidth)
                                        {
                                            var words = Regex.Split(text, $"[ ]|{Environment.NewLine}");
                                            var lineIndex = 0;
                                            var lineText = "";

                                            for (var i = 0; i < words.Length; i++)
                                            {
                                                lineText += lineText != "" ? $" {words[i]}" : words[i];
                                                var rectLine = graphics.MeasureString(i < words.Length - 1 ? lineText + $" {words[i + 1]}" : lineText, typeface);
                                                if (rectLine.Width > textSize.Width + 20 || i == words.Length - 1 || lineText.EndsWith("\r"))
                                                {
                                                    xmlsWriter.WriteStartElement("tspan");
                                                    xmlsWriter.WriteAttributeString("x", "0");
                                                    if (lineIndex == 0)
                                                        xmlsWriter.WriteAttributeString("y", "0");
                                                    else
                                                        xmlsWriter.WriteAttributeString("y", NormalizeDecimal(lineIndex * typeface.Size * sScale));
                                                    xmlsWriter.WriteString(lineText);
                                                    xmlsWriter.WriteEndElement();
                                                    lineIndex++;
                                                    lineText = "";
                                                }
                                            }
                                        }
                                        else
                                        {
                                            var lines = Regex.Split(text, Environment.NewLine);
                                            for (var i = 0; i < lines.Length; i++)
                                            {
                                                xmlsWriter.WriteStartElement("tspan");
                                                xmlsWriter.WriteAttributeString("x", "0");
                                                if (i == 0)
                                                    xmlsWriter.WriteAttributeString("y", "0");
                                                else
                                                    xmlsWriter.WriteAttributeString("y", NormalizeDecimal(i * typeface.Size * sScale));
                                                xmlsWriter.WriteString(lines[i]);
                                                xmlsWriter.WriteEndElement();
                                            }
                                        }
                                        xmlsWriter.WriteEndElement();
                                    }
                                }
                            }
                        }
                        #endregion

                        #region draw bubble
                        if (map.ShowBubble)
                        {
                            var fillBrush = new SolidBrush(Color.FromArgb(150, mapStyle.BubbleBackColor));
                            var strokeColor = Color.FromArgb(170, mapStyle.BubbleBorderColor);

                            foreach (var key in keys)
                            {
                                var bubbleSize = GetBubbleSize(mapData, key);
                                if (bubbleSize > 0)
                                {
                                    var path = svgContainer.HashPaths[key];
                                    var centerPos = new Point(path.RectIso.X + path.RectIso.Width / 2, path.RectIso.Y + path.RectIso.Height / 2);
                                    xmlsWriter.WriteStartElement("ellipse");
                                    xmlsWriter.WriteAttributeString("cx", NormalizeDecimal(centerPos.X * sScale));
                                    xmlsWriter.WriteAttributeString("cy", NormalizeDecimal(centerPos.Y * sScale));
                                    xmlsWriter.WriteAttributeString("rx", NormalizeDecimal(bubbleSize / 2 * sScale));
                                    xmlsWriter.WriteAttributeString("ry", NormalizeDecimal(bubbleSize / 2 * sScale));
                                    xmlsWriter.WriteAttributeString("style", $"{GetFillBrush(fillBrush)}{GetBorderStroke(strokeColor)} stroke-width:2; pointer-events:none;");
                                    xmlsWriter.WriteEndElement();
                                }
                            }
                        }
                        #endregion
                    }
                }
            }
            if (mapData != null)
                mapData.Clear();
        }

        private static void AddToolTipStyle(XmlTextWriter writer, StiMapStyle style)
        {
            if (style != null)
            {
                writer.WriteAttributeString("_ttbrush", StiJsonReportObjectHelper.Serialize.JBrush(style.ToolTipBrush));
                writer.WriteAttributeString("_tttextbrush", StiJsonReportObjectHelper.Serialize.JBrush(style.ToolTipTextBrush));
                writer.WriteAttributeString("_ttborder", StiJsonReportObjectHelper.Serialize.JBorder(style.ToolTipBorder));
                writer.WriteAttributeString("_ttcornerradius", style.ToolTipCornerRadius.ToString());
            }
        }

        private static double GetScaleStep(double factor)
        {
            if (factor < 1.5)
                return 1;

            else if (factor < 2)
                return 1.5;

            else
                return 2;
        }

        private static string GetPathText(StiMap map, StiMapData info, StiMapSvg path, string key)
        {
            string text = null;
            switch (map.DisplayNameType)
            {
                case StiDisplayNameType.Full:
                    {
                        text = (info != null)
                            ? info.Name
                            : path.EnglishName;
                    }
                    break;

                case StiDisplayNameType.Short:
                    {
                        text = StiMapHelper.PrepareIsoCode(path.ISOCode);
                    }
                    break;
            }

            if (map.ShowValue)
            {
                if (info != null && info.Value != null)
                {
                    string valueStr = null;
                    if (map.ShortValue)
                    {
                        double resValue = 0d;
                        if (double.TryParse(info.Value, out resValue))
                        {
                            valueStr = StiAbbreviationNumberFormatHelper.Format(resValue);
                        }
                    }

                    if (valueStr == null)
                        valueStr = info.Value;

                    if (text == null)
                    {
                        text = valueStr;
                    }
                    else
                    {
                        text += Environment.NewLine;
                        text += valueStr;
                    }
                }
            }

            return text;
        }

        private static Rectangle GetPathRect(StiMap map, StiMapSvg path)
        {
            Rectangle rect;
            if (map.DisplayNameType == StiDisplayNameType.Short)
                rect = path.RectIso.IsEmpty ? path.Rect : path.RectIso;
            else
                rect = path.Rect;

            return rect;
        }

        private static StiTextHorAlignment GetPathHorAlignment(StiMap map, StiMapSvg path)
        {
            if (map.DisplayNameType == StiDisplayNameType.Short)
                return path.HorAlignmentIso != null ? path.HorAlignmentIso.GetValueOrDefault() : path.HorAlignment;

            return path.HorAlignment;
        }

        private static StiVertAlignment GetPathVertAlignment(StiMap map, StiMapSvg path)
        {
            if (map.DisplayNameType == StiDisplayNameType.Short)
                return path.VertAlignmentIso != null ? path.VertAlignmentIso.GetValueOrDefault() : path.VertAlignment;

            return path.VertAlignment;
        }

        private static string GetToolTipIdent(StiMapData data)
        {
            return string.IsNullOrEmpty(data.Name) ? data.Key : data.Name;
        }

        private static string GetToolTipValueText(StiMap map, string value)
        {
            var valueStr = string.Empty;

            if (map.ShowValue && !string.IsNullOrEmpty(value))
            {
                if (map.ShortValue)
                {
                    double resValue;
                    if (double.TryParse(value, out resValue))
                        valueStr = StiAbbreviationNumberFormatHelper.Format(resValue);
                }

                if (string.IsNullOrEmpty(valueStr))
                    valueStr = value;
            }

            return valueStr;
        }

        private static string GetToolTipTotalText(StiMap map, List<StiMapData> mapData, StiMapData data)
        {
            if (map.MapType == StiMapType.Group || map.MapType == StiMapType.HeatmapWithGroup)
            {
                var sumValues = mapData.Where(x => x.Group == data.Group && x.Value != null);
                var sum = 0d;
                foreach (var sumValue in sumValues)
                {
                    double value;
                    if (double.TryParse(sumValue.Value, out value))
                        sum += value;
                }

                var sumFormat = StiAbbreviationNumberFormatHelper.Format(sum);
                return $"{StiLocalization.Get("PropertyMain", "Total")} = {sumFormat}";
            }

            return null;
        }

        private static String NormalizeDecimal(double value)
        {
            return value.ToString().Replace(",", ".");
        }

        private static string GetBorderStroke(Color color)
        {
            var result = $"stroke:rgb({color.R},{color.G},{color.B});";

            var alfa = Math.Round(color.A / 255f, 3);

            if (alfa != 1)
                result += $"stroke-opacity:{alfa.ToString().Replace(",", ".")};";

            return result;
        }

        public static string GetFillBrush(SolidBrush brush)
        {
            var color = brush.Color;
            return $"fill:rgb({color.R},{color.G},{color.B});fill-opacity:{Math.Round(color.A / 255f, 3).ToString().Replace(",", ".")};";
        }

        private static float GetBubbleSize(List<StiMapData> mapData, string key)
        {
            var diameterMin = 14;
            var diameterMax = 130;
            var minValue = 0d;
            var maxValue = 0d;
            var restValue = 0d;

            float bubbleSize = diameterMax - diameterMin;
            var info = mapData.FirstOrDefault(x => x.Key == key);

            bool isFirst = true;
            foreach (var data in mapData)
            {
                if (data.Value == null) continue;

                var value = data.GetValue();
                if (value == null) continue;

                if (isFirst)
                {
                    isFirst = false;

                    minValue = value.GetValueOrDefault();
                    maxValue = value.GetValueOrDefault();
                }
                else
                {
                    if (value < minValue)
                        minValue = value.GetValueOrDefault();
                    else if (value > maxValue)
                        maxValue = value.GetValueOrDefault();
                }
            }

            restValue = maxValue - minValue;

            if (info != null && info.Value != null)
            {
                var resValue = info.GetValue();
                if (resValue != null)
                {
                    var percent = (resValue.GetValueOrDefault() - minValue) / restValue;
                    return (float)(diameterMin + bubbleSize * percent);
                }
            }

            return 0;
        }
        #endregion
    }
}
