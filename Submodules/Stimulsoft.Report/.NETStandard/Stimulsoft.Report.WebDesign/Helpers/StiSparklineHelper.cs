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

using System.Xml;
using System.Text;
using System.Reflection;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Stimulsoft.Report.Export;
using Stimulsoft.Report.Gauge;
using System;
using Stimulsoft.Report.Gauge.Helpers;
using Stimulsoft.Report.Components.Gauge.Primitives;
using System.Linq;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Export.Services.Helpers;
using Stimulsoft.Base.Drawing;

namespace Stimulsoft.Report.Web
{
    internal class StiSparklineHelper
    {
        public static Hashtable GetSparklineProperties(StiSparkline sparkline)
        {
            Hashtable properties = new Hashtable();
            properties["name"] = sparkline.Name;
            properties["valueDataColumn"] = StiEncodingHelper.Encode(sparkline.ValueDataColumn);
            properties["type"] = sparkline.Type.ToString();
            properties["showHighLowPoints"] = sparkline.ShowHighLowPoints;
            properties["showFirstLastPoints"] = sparkline.ShowFirstLastPoints;
            properties["positiveColor"] = StiReportEdit.GetStringFromColor(sparkline.PositiveColor);
            properties["negativeColor"] = StiReportEdit.GetStringFromColor(sparkline.NegativeColor);
            properties["svgContent"] = StiReportEdit.GetSvgContent(sparkline);

            return properties;
        }

        public static void UpdateSparkline(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            var command = param["innerCommand"] as string;
            var component = report.Pages.GetComponentByName((string)param["componentName"]);
            if (component == null) return;
            StiSparkline sparkline = component as StiSparkline;

            switch (command)
            {                
                case "SetPropertyValue":
                    {
                        SetPropertyValue(sparkline, param, callbackResult);
                        break;
                    }
            }

            if (!callbackResult.Contains("sparklineProperties"))
                callbackResult["sparklineProperties"] = GetSparklineProperties(sparkline);
        }

        private static void SetPropertyValue(StiSparkline sparkline, Hashtable param, Hashtable callbackResult)
        {
            var propertyName = param["propertyName"] as string;
            var propertyValue = param["propertyValue"];

            switch (propertyName)
            {
                case "ValueDataColumn":
                    sparkline.ValueDataColumn = StiEncodingHelper.DecodeString(propertyValue as string);
                    break;

                case "Type":
                    sparkline.Type = (StiSparklineType)Enum.Parse(typeof(StiSparklineType), propertyValue as string);
                    break;

                case "ShowHighLowPoints":
                    sparkline.ShowHighLowPoints = Convert.ToBoolean(propertyValue);
                    break;

                case "ShowFirstLastPoints":
                    sparkline.ShowFirstLastPoints = Convert.ToBoolean(propertyValue);
                    break;

                case "PositiveColor":
                    sparkline.PositiveColor = StiReportEdit.StrToColor(propertyValue as string);
                    break;

                case "NegativeColor":
                    sparkline.NegativeColor = StiReportEdit.StrToColor(propertyValue as string);
                    break;
            }
        }

        public static ArrayList GetSparklineStyles(StiReport report)
        {
            ArrayList styles = new ArrayList();

            foreach (StiBaseStyle style in report.Styles)
            {
                if (style is StiIndicatorStyle)
                {
                    styles.Add(StiStylesHelper.StyleItem(style));
                }
            }

            return styles;
        }

        private static string GetSparklineSampleImage(StiSparkline sparkLine, int width, int height)
        {
            var svgData = new StiSvgData()
            {
                X = 0,
                Y = 0,
                Width = width,
                Height = height,
                Component = sparkLine
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

                writer.WriteStartElement("rect");

                writer.WriteAttributeString("height", svgData.Height.ToString().Replace(",", "."));
                writer.WriteAttributeString("width", svgData.Width.ToString().Replace(",", "."));

                writer.WriteAttributeString("style", StiGaugeSvgHelper.WriteFillBrush(writer, sparkLine.Brush, new RectangleF(0, 0, (float)svgData.Width, (float)svgData.Height)));
                writer.WriteFullEndElement();

                StiSparklineSvgHelper.WriteSparkline(writer, svgData);

                writer.WriteFullEndElement();
                writer.Flush();
                ms.Flush();
                writer.Close();
                ms.Close();
            }

            return sb.ToString();
        }

        public static void GetStylesContent(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            var stylesContent = new ArrayList();
            var sparkLine = report.Pages.GetComponentByName(param["componentName"] as string) as StiSparkline;

            if (sparkLine != null)
            {
                var sparkLineCloned = sparkLine.Clone() as StiSparkline;
                sparkLineCloned.Values = new decimal[] { 2, 3, 1, 3, 7, 6, 2, 3 };                

                int width = 120;
                int height = 55;

                sparkLine.Page.Components.Add(sparkLineCloned);

                foreach (StiBaseStyle style in report.Styles)
                {
                    var indicatorStyle = style as StiIndicatorStyle;
                    if (indicatorStyle != null)
                    {
                        var content = new Hashtable();
                        sparkLineCloned.ComponentStyle = indicatorStyle.Name;
                        sparkLineCloned.Brush = new StiSolidBrush(indicatorStyle.BackColor);

                        content["image"] = GetSparklineSampleImage(sparkLineCloned, width, height);
                        content["name"] = indicatorStyle.Name;
                        content["width"] = width;
                        content["height"] = height;
                        stylesContent.Add(content);
                    }
                }

                //Remove temp elements
                if (sparkLine.Page.Components.Contains(sparkLineCloned))
                    sparkLine.Page.Components.Remove(sparkLineCloned);
            }

            callbackResult["stylesContent"] = stylesContent;
        }
    }
}