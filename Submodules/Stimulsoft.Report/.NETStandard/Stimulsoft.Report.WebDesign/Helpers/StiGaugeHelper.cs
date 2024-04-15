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

namespace Stimulsoft.Report.Web
{
    internal class StiGaugeHelper
    {
        public static Hashtable GetGaugeProperties(StiGauge gauge)
        {
            Hashtable properties = new Hashtable();
            properties["name"] = gauge.Name;
            properties["type"] = gauge.Type;
            properties["minimum"] = gauge.Minimum;
            properties["maximum"] = gauge.Maximum;
            properties["calculationMode"] = gauge.CalculationMode;
            properties["indicatorColumn"] = GetIndicatorColumn(gauge);

            return properties;
        }
                
        public static void SetGaugeProperties(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            var gauge = report.Pages.GetComponentByName(param["componentName"] as string) as StiGauge;
            if (gauge != null)
            {
                var props = param["properties"] as Hashtable;
                gauge.Type = (StiGaugeType)Enum.Parse(typeof(StiGaugeType), props["type"] as string);
                gauge.CalculationMode = (StiGaugeCalculationMode)Enum.Parse(typeof(StiGaugeCalculationMode), props["calculationMode"] as string);
                gauge.Minimum = Convert.ToDecimal(props["minimum"]);
                gauge.Maximum = Convert.ToDecimal(props["maximum"]);
                SetIndicatorColumn(gauge, props["indicatorColumn"] as string);
                StiGaugeV2InitHelper.Init(gauge, gauge.Type, true, false);

                callbackResult["svgContent"] = StiReportEdit.GetSvgContent(gauge);
                callbackResult["componentName"] = param["componentName"];
            }
        }

        private static string GetIndicatorColumn(StiGauge gauge)
        {
            var indicator = gauge.Scales.Count > 0 ? gauge.Scales[0].Items.ToArray().FirstOrDefault(x => x is StiIndicatorBase) as StiIndicatorBase : null;
            return indicator?.Value?.Value;
        }

        private static void SetIndicatorColumn(StiGauge gauge, string value)
        {
            var indicator = gauge.Scales.Count > 0 ? gauge.Scales[0].Items.ToArray().FirstOrDefault(x => x is StiIndicatorBase) as StiIndicatorBase : null;
            if (indicator != null) indicator.Value.Value = value;
        }

        #region Styles methods
        public static Hashtable GetStyle(StiGauge gauge)
        {
            Hashtable style = new Hashtable();
            style["type"] = !string.IsNullOrEmpty(gauge.CustomStyleName) ? "StiCustomGaugeStyle" : gauge.Style.GetType().Name;
            style["name"] = !string.IsNullOrEmpty(gauge.CustomStyleName) ? gauge.CustomStyleName : (gauge.Style as StiGaugeStyleXF)?.ServiceName;

            return style;
        }

        private static List<StiGaugeStyleXF> GetGaugeStyles(StiReport report, bool withReportStyles = true)
        {
            var gaugeStyles = new List<StiGaugeStyleXF>();

            if (withReportStyles)
            {
                foreach (StiBaseStyle style in report.Styles)
                {
                    if (style is StiGaugeStyle)
                    {
                        var customStyle = new StiCustomGaugeStyle((StiGaugeStyle)style);
                        gaugeStyles.Add(customStyle);
                    }
                }
            }

            gaugeStyles.AddRange(StiOptions.Services.GaugeStyles);

            return gaugeStyles;
        }

        public static string GetGaugeSampleImage(StiGauge gauge, int width, int height, float zoom)
        {
            var svgData = new StiSvgData()
            {
                X = 0,
                Y = 0,
                Width = width,
                Height = height,
                Component = gauge
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

                writer.WriteAttributeString("style", StiGaugeSvgHelper.WriteFillBrush(writer, gauge.Brush, new RectangleF(0, 0, (float)svgData.Width, (float)svgData.Height)));
                writer.WriteFullEndElement();

                StiGaugeSvgHelper.WriteGauge(writer, svgData, zoom, false, true);

                writer.WriteFullEndElement();
                writer.Flush();
                ms.Flush();
                writer.Close();
                ms.Close();
            }

            return sb.ToString();
        }

        public static void SetGaugeStyle(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            var component = report.Pages.GetComponentByName((string)param["componentName"]);
            string styleType = (string)param["styleType"];
            string styleName = (string)param["styleName"];

            if (component == null) return;
            StiGauge gauge = component as StiGauge;

            if (styleType == "StiCustomGaugeStyle")
            {
                var reportStyle = gauge.Report.Styles.ToList().FirstOrDefault(x => x.Name == styleName) as StiGaugeStyle;
                if (reportStyle != null)
                {
                    gauge.Style = new StiCustomGaugeStyle(reportStyle);
                    gauge.CustomStyleName = reportStyle.Name;
                }
            }
            else
            {
                Assembly assembly = typeof(StiReport).Assembly;
                var baseStyle = assembly.CreateInstance("Stimulsoft.Report.Gauge." + styleType) as StiGaugeStyleXF;
                if (baseStyle != null)
                {
                    gauge.CustomStyleName = string.Empty;
                    gauge.Style = (IStiGaugeStyle)baseStyle;
                }
            }

            gauge.ApplyStyle(gauge.Style);
        }

        public static void GetStylesContent(StiReport report, Hashtable param, Hashtable callbackResult, bool forStylesControl, bool withReportStyles = true)
        {
            var component = report.Pages.GetComponentByName((string)param["componentName"]);
            StiGauge gaugeCloned = null;

            if (component != null) {
                gaugeCloned = component.Clone() as StiGauge;
            }
            else {
                gaugeCloned = new StiGauge();
                StiGaugeV2InitHelper.Init(gaugeCloned, StiGaugeType.FullCircular, true, false);
            }

            var stylesContent = new ArrayList();

            if (gaugeCloned != null)
            {
                foreach (var style in GetGaugeStyles(report, withReportStyles))
                {
                    gaugeCloned.CustomStyleName = style is StiCustomGaugeStyle ? style.Name : string.Empty;
                    gaugeCloned.Style = (StiGaugeStyleXF)style.Clone();
                    gaugeCloned.CalculationMode = StiGaugeCalculationMode.Custom;
                    gaugeCloned.ApplyStyle(gaugeCloned.Style);
                    SetIndicatorColumn(gaugeCloned, "50");

                    int width = forStylesControl ? 125 : 80;
                    int height = forStylesControl ? 50 : 80;

                    Hashtable content = new Hashtable();
                    content["image"] = GetGaugeSampleImage(gaugeCloned, width, height, 0.5f);
                    content["type"] = style.GetType().Name;
                    content["name"] = gaugeCloned.Style is StiCustomGaugeStyle
                        ? ((StiCustomGaugeStyleCoreXF)gaugeCloned.Style.Core).ReportStyleName
                        : ((StiGaugeStyleXF)gaugeCloned.Style).ServiceName;
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