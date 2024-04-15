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

using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Dashboard.Styles;
using Stimulsoft.Report.Export;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Linq;
using Stimulsoft.Base.Context.Animation;
using System.Drawing;
using Stimulsoft.Report.Maps;

namespace Stimulsoft.Report.Web.Helpers.Dashboards
{
    internal class StiDashboardsSvgHelper
    {
        private static double GetSvgImageValue(string content, string name)
        {
            var image = content.Substring(content.IndexOf("<image"));
            var attribute = image.Substring(image.IndexOf(name) + name.Length + 2);
            attribute = attribute.Substring(0, attribute.IndexOf("\""));

            double value;
            double.TryParse(attribute, NumberStyles.Float, CultureInfo.InvariantCulture, out value);

            return value;
        }

        private static string SaveElementToVectorString(IStiElement element, ref bool needToVertScroll, ref bool needToHorScroll, double scaleX, double scaleY, string content, bool needAnimation = false)
        {
            if (content == null || content.IndexOf("<image") < 0)
                return content;

            using (var stream = new MemoryStream())
            using (var writer = new XmlTextWriter(stream, Encoding.UTF8))
            {
                StiSvgData svgData = new StiSvgData();
                svgData.X = GetSvgImageValue(content, "x");
                svgData.Y = GetSvgImageValue(content, "y");
                svgData.Width = GetSvgImageValue(content, "width");
                svgData.Height = GetSvgImageValue(content, "height");
                svgData.Component = (StiComponent)element;
                var contentHeight = 0;
                var contentWidth = 0;

                if (element is IStiProgressElement)
                {
                    
                    var progressVisualSvgHelper = StiDashboardHelperCreator.CreateProgressVisualSvgHelper();
                    if (progressVisualSvgHelper != null)
                        progressVisualSvgHelper.WriteProgress(writer, svgData, ref needToVertScroll, ref contentHeight);
                }
                else if (element is IStiIndicatorElement)
                {
                    var indicatorVisualSvgHelper = StiDashboardHelperCreator.CreateIndicatorVisualSvgHelper();
                    if (indicatorVisualSvgHelper != null)
                        indicatorVisualSvgHelper.WriteIndicator(writer, svgData, ref needToVertScroll, ref contentHeight);
                }
                else if (element is IStiCardsElement)
                {
                    needToHorScroll = true;
                    var cardsVisualSvgHelper = StiDashboardHelperCreator.CreateCardsVisualSvgHelper();
                    if (cardsVisualSvgHelper != null)
                        cardsVisualSvgHelper.WriteCards(writer, svgData, ref needToVertScroll, ref needToHorScroll, ref contentHeight, ref contentWidth);
                }
                else if (element is IStiGaugeElement)
                {
                    var gaugeVisualSvgHelper = StiDashboardHelperCreator.CreateGaugeVisualSvgHelper();
                    if (gaugeVisualSvgHelper != null)
                        gaugeVisualSvgHelper.WriteGauge(writer, svgData, needAnimation, ref needToVertScroll, ref contentHeight);
                }

                writer.Flush();

                stream.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(stream))
                {
                    var vector = reader.ReadToEnd();

                    if (needToVertScroll || needToHorScroll)
                    {
                        //Sets sizes and content for scrolling
                        return $"<svg x=\"0\" y=\"0\" width=\"{(contentWidth > 0 ? contentWidth : svgData.Width)}\" height=\"{(contentHeight > 0 ? contentHeight : svgData.Height)}\">{vector}</svg>";
                    }
                    else
                    {
                        if (content.IndexOf("<image") >= 0)
                        {
                            //Replace in the export content png image to svg 
                            var title = content.Substring(0, content.IndexOf("<image"));
                            return $"{title}{vector}</svg>";
                        }
                        else
                        {
                            return content;
                        }
                    }
                }
            }
        }

        public static string SaveElementToString(IStiElement element, double scaleX = 1, double scaleY = 1, bool designMode = false, StiExportFormat exportFormat = StiExportFormat.ImageSvg, StiRequestParams requestParams = null)
        {
            var needToVertScroll = false;
            var needToHorScroll = false;

            return SaveElementToString(element, ref needToVertScroll, ref needToHorScroll, scaleX, scaleY, designMode, exportFormat, requestParams);
        }

        public static string SaveElementToString(IStiElement element, ref bool needToVertScroll, ref bool needToHorScroll, double scaleX = 1, double scaleY = 1, bool designMode = false,
        StiExportFormat exportFormat = StiExportFormat.ImageSvg, StiRequestParams requestParams = null)
        {
            var settings = StiInvokeMethodsHelper.InvokeStaticMethod(
                "Stimulsoft.Dashboard.Export", "Helpers.StiExportSettingsHelper", "GetDashboardExportSettings",
                new object[] { exportFormat }, new Type[] { typeof(StiExportFormat) });

            StiInvokeMethodsHelper.SetPropertyValue(settings, "RenderBorders", false);
            StiInvokeMethodsHelper.SetPropertyValue(settings, "RenderSinglePage", true);
            StiInvokeMethodsHelper.SetPropertyValue(settings, "RenderEmptyContent", !designMode || element is IStiTextElement || element is IStiOnlineMapElement);

            if (exportFormat == StiExportFormat.ImageSvg)
                StiInvokeMethodsHelper.SetPropertyValue(settings, "DesignMode", designMode);

            using (var report = new StiReport())
            using (var stream = new MemoryStream())
            {
                report.Culture = element.Report.GetParsedCulture();
                report.IsDocument = true;
                if (element.Report != null)
                {
                    report.Dictionary.Resources.AddRange(element.Report.Dictionary.Resources);
                }
                report.ReportUnit = StiReportUnitType.HundredthsOfInch;

                if (!string.IsNullOrEmpty(element.Report.ReportGuid))
                    report.ReportGuid = element.Report.ReportGuid;

                var component = (StiComponent)element;
                var margin = ((IStiMargin)element).Margin;
                var page = report.RenderedPages[0];
                page.Margins = new StiMargins(0);
                page.Width = Math.Round(component.Width * scaleX, MidpointRounding.AwayFromZero) - margin.Left - margin.Right;
                page.Height = Math.Round(component.Height * scaleY, MidpointRounding.AwayFromZero) - margin.Top - margin.Bottom;

                var elementBackColor = StiDashboardStyleHelper.GetBackColor(element, null, true);
                var setTransparency = elementBackColor.A != 255;
                page.Brush = new StiSolidBrush(setTransparency ? Color.Transparent : elementBackColor);

                //Get chart start values from cache
                object chartStartValues = null;
                object chartAnimations = null;
                var cacheGuid = element.Report != null && !string.IsNullOrEmpty(element.Report.ReportGuid) ? $"{element.Report.ReportGuid}_{element.Name}" : element.Name;

                if (!designMode && (element is IStiChartElement || element is IStiGaugeElement) && requestParams != null)
                {
                    chartAnimations = StiChartElementViewHelper.GetChartAnimationsFromCache(cacheGuid, requestParams);

                    if (requestParams.Interaction.DashboardFiltering == null && requestParams.Interaction.DashboardSorting == null && requestParams.Action != StiAction.RefreshReport)
                    {
                        StiChartElementViewHelper.RemoveChartAnimationsFromCache(cacheGuid, requestParams);
                        chartAnimations = null;
                    }

                    if (requestParams.Interaction.DashboardFiltering == null)
                        requestParams.Cache.Helper.RemoveObject(cacheGuid);
                    else if (chartAnimations == null)
                        chartStartValues = StiChartElementViewHelper.GetChartValuesFromCache(cacheGuid, requestParams);
                }

                if (chartAnimations == null)
                    chartAnimations = new List<StiAnimation>();

                var panel = (StiComponent)StiInvokeMethodsHelper.InvokeStaticMethod(
                    "Stimulsoft.Dashboard.Export", "StiDashboardExportTools", "RenderElement",
                    new object[] { page, element, scaleX, scaleY, settings, chartStartValues, chartAnimations },
                    new Type[] { typeof(StiContainer), typeof(IStiElement), typeof(double), typeof(double), settings.GetType(), typeof(List<double?[]>), typeof(List<StiAnimation>) });

                panel.Left = 0;
                panel.Top = 0;

                foreach (StiPage finalPage in report.RenderedPages)
                {
                    finalPage.GetComponentsList().ForEach(c => c.Page = finalPage);
                    finalPage.MoveComponentsToPage();

                    //for support transparency dbs elements
                    if (setTransparency)
                    {
                        ApplyTransparencyToComponents(finalPage, elementBackColor);
                    }

                    //Save chart start values to cache
                    if (!designMode && element is IStiChartElement && requestParams != null)
                        StiChartElementViewHelper.SaveChartValuesToCache(cacheGuid, finalPage, requestParams);
                }

                var storedCulture = StiInvokeMethodsHelper.InvokeStaticMethod("Stimulsoft.Dashboard", "Helpers.StiCultureHelper", "Set", new object[] { report }, new Type[] { typeof(StiReport) });
                try
                {
                    if (exportFormat == StiExportFormat.Html5)  // Html5 format calls a special HTML export of dashboards
                    {
                        var exportSettings = new StiHtmlExportSettings
                        {
                            ChartType = !designMode ? (requestParams == null || requestParams.Viewer.ChartRenderType == StiChartRenderType.AnimatedVector ? StiHtmlChartType.AnimatedVector : StiHtmlChartType.Vector) : StiHtmlChartType.Vector,
                            ExportMode = StiHtmlExportMode.Div,
                            AddPageBreaks = false
                        };

                        //correct width and margins for chart & map titles
                        var titleComp = report.GetComponentByName($"{element.Name}_Title") as StiText;
                        if (titleComp != null)
                        {
                            var cornerRadius = (element as IStiCornerRadius)?.CornerRadius;
                            if (cornerRadius != null)
                            {
                                if (cornerRadius.TopLeft > 0)
                                    titleComp.Margins.Left = Math.Max(5, cornerRadius.TopLeft / 1.5);

                                if (cornerRadius.TopRight > 0)
                                    titleComp.Margins.Right = Math.Max(5, cornerRadius.TopRight / 1.5);
                            }
                        }

                        report.ExportDocument(StiExportFormat.Html, stream, exportSettings);

                        foreach (StiPage finalPage in report.RenderedPages)
                        {
                            if (!designMode && (element is IStiChartElement || element is IStiGaugeElement) && requestParams != null)
                            {
                                StiChartElementViewHelper.SaveChartAnimationsToCache(element, cacheGuid, finalPage, requestParams);
                            }
                        }

                        return Encoding.UTF8.GetString(stream.ToArray()).Replace(">Trial<", "><");
                    }
                    else
                    {
                        //Check GIF Image element
                        if (!designMode && element is IStiImageElement)
                        {
                            var gifContent = GetImageElementGifContent(element as IStiImageElement);
                            
                            if (gifContent != null) 
                            {
                                return gifContent;
                            }
                        }

                        var exportSettings = new StiSvgExportSettings();
                        report.ExportDocument(StiExportFormat.ImageSvg, stream, exportSettings);

                        var content = Encoding.UTF8.GetString(stream.ToArray());

                        // Progress and indicator are currently not supported in the SVG report renderer
                        if (element is IStiGaugeElement || element is IStiProgressElement || element is IStiIndicatorElement || element is IStiCardsElement)
                        {
                            if (element.GetMeters().Count > 0 || (element as IStiManuallyEnteredData)?.DataMode == StiDataMode.ManuallyEnteringData)
                            {
                                content = SaveElementToVectorString(element, ref needToVertScroll, ref needToHorScroll, scaleX, scaleY, content, !designMode);
                            }
                            else if (!designMode)
                            {
                                return string.Empty;
                            }
                        }

                        foreach (StiPage finalPage in report.RenderedPages)
                        {
                            if (!designMode && element is IStiGaugeElement && requestParams != null)
                            {
                                StiChartElementViewHelper.SaveChartAnimationsToCache(element, cacheGuid, finalPage, requestParams);
                            }
                        }

                        return content.IndexOf("<svg") >= 0 ? content.Substring(content.IndexOf("<svg")) : content;
                    }
                }
                finally
                {
                    StiInvokeMethodsHelper.InvokeStaticMethod("Stimulsoft.Dashboard.Helpers", "StiCultureHelper", "Restore", new object[] { storedCulture }, new Type[] { typeof(CultureInfo) });
                }
            }
        }

        private static void ApplyTransparencyToComponents(StiPage page, Color elementBackColor)
        {
            if (elementBackColor.A != 255)
            {
                page.Brush = new StiEmptyBrush();

                page.Components.ToList().ForEach(comp =>
                {
                    var map = comp as StiMap;
                    var brushComp = comp as IStiBrush;

                    if (map != null && StiBrush.ToColor(map.Brush).Equals(elementBackColor))
                    {
                        map.Brush = new StiSolidBrush(Color.FromArgb(0, StiBrush.ToColor(map.Brush)));
                    }
                    else if (brushComp != null && StiBrush.ToColor(brushComp.Brush).Equals(elementBackColor))
                    {
                        brushComp.Brush = new StiSolidBrush(Color.FromArgb(0, StiBrush.ToColor(brushComp.Brush)));
                    }
                });
            }
        }

        private static string GetImageElementGifContent(IStiImageElement imageElement)
        {
            if (Report.Helpers.StiImageHelper.IsGif(imageElement.Image))
            {
                return $"gifContent:data:image/gif;base64,{Convert.ToBase64String(imageElement.Image)}";
            }
            else if (!string.IsNullOrEmpty(imageElement.ImageHyperlink) && imageElement.ImageHyperlink.ToLower().EndsWith("gif"))
            {
                return $"gifContent:{imageElement.ImageHyperlink}";
            }

            return null;
        }
    }
}
