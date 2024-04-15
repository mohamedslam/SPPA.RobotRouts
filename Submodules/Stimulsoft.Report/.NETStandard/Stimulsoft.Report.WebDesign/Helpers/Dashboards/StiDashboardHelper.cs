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
using Stimulsoft.Base.Licenses;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Dashboard.Styles;
using Stimulsoft.Report.Export;
using Stimulsoft.Report.Export.Services.Helpers;
using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Xml;
using System.Text;
using Stimulsoft.Report.Surface;
using System.ComponentModel;

#if SERVER
using Stimulsoft.Server.Objects;
using Stimulsoft.Base.Plans;
#endif

#if NETSTANDARD
using Stimulsoft.System.Security.Cryptography;
#else
using System.Security.Cryptography;
#endif

namespace Stimulsoft.Report.Web.Helpers.Dashboards
{
    public class StiDashboardHelper
    { 
        #region Methods
        public static void AddDashboard(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            report.IsModified = true;
            var dashboardPage = StiDashboardCreator.CreateDashboard(report) as StiPage;

            int currentPageIndex = int.Parse((string)param["pageIndex"]);

            if (currentPageIndex + 1 == report.Pages.Count)
                report.Pages.Add(dashboardPage);
            else
            {
                report.Pages.Insert(currentPageIndex + 1, dashboardPage);
                dashboardPage.Name = StiNameCreation.CreateName(report, StiNameCreation.GenerateName(dashboardPage));
            }

            #region LicenseKey
#if CLOUD
            var isValid = StiCloudPlan.IsDashboardsAvailable(report.ReportGuid);
#elif SERVER
            var isValid = !StiVersionX.IsSvr;
#else
            var isValidKey = false;
            var licenseKey = StiLicenseKeyValidator.GetLicenseKey();
            if (licenseKey != null)
            {
                try
                {
                    using (var rsa = new RSACryptoServiceProvider(512))
                    using (var sha = new SHA1CryptoServiceProvider())
                    {
                        rsa.FromXmlString("<RSAKeyValue><Modulus>iyWINuM1TmfC9bdSA3uVpBG6cAoOakVOt+juHTCw/gxz/wQ9YZ+Dd9vzlMTFde6HAWD9DC1IvshHeyJSp8p4H3qXUKSC8n4oIn4KbrcxyLTy17l8Qpi0E3M+CI9zQEPXA6Y1Tg+8GVtJNVziSmitzZddpMFVr+6q8CRi5sQTiTs=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");
                        isValidKey = rsa.VerifyData(licenseKey.GetCheckBytes(), sha, licenseKey.GetSignatureBytes());
                    }
                }
                catch (Exception)
                {
                    isValidKey = false;
                }
            }

            var isValid = isValidKey & StiLicenseKeyValidator.IsValid(StiProductIdent.DbsWeb, licenseKey) && typeof(StiLicense).AssemblyQualifiedName.Contains(StiPublicKeyToken.Key);
#endif
            #endregion

            callbackResult["name"] = dashboardPage.Name;
            callbackResult["pageIndex"] = report.Pages.IndexOf(dashboardPage).ToString();
            callbackResult["properties"] = StiReportEdit.GetAllProperties(dashboardPage);
            callbackResult["pageIndexes"] = StiReportEdit.GetPageIndexes(report);            

            if (isValid)
                callbackResult["valid"] = true;
        }

        public static Color GetDashboardGridLinesColor(StiPage page)
        {
            var dashboard = page as IStiDashboard;
            return StiDashboardStyleHelper.IsDarkStyle(dashboard.Style)
                ? ColorTranslator.FromHtml("#3b4d5f")
                : Color.Gainsboro;
        }

        public static Color GetDashboardGridDotsColor(StiPage page)
        {
            var dashboard = page as IStiDashboard;
            return StiDashboardStyleHelper.IsDarkStyle(dashboard.Style)
                ? ColorTranslator.FromHtml("#3b4d5f")
                : StiColorUtils.Dark(Color.Gainsboro, 30);
        }

        public static Color GetSelectionBorderColor(StiPage page)
        {
            var dashboard = page as IStiDashboard;
            return StiDashboardStyleHelper.IsDarkStyle(dashboard.Style)
                ? Color.SteelBlue
                : Color.DimGray;
        }

        public static Color GetSelectionCornerColor(StiPage page)
        {
            var dashboard = page as IStiDashboard;
            return StiDashboardStyleHelper.IsDarkStyle(dashboard.Style)
                ? Color.LightGray
                : Color.Gray;
        }

        public static Color GetDashboardBackColor(StiPage page)
        {
            return StiDashboardStyleHelper.GetDashboardBackColor(page as IStiDashboard, false);
        }

        public static StiComponent CreateDashboardElement(StiReport report, string typeComponent)
        {
            StiComponent component = null;

            switch (typeComponent)
            {
                case "StiTableElement":
                    component = StiActivator.CreateObject("Stimulsoft.Dashboard.Components.Table.StiTableElement") as StiComponent;
                    break;

                case "StiChartElement":
                    component = StiActivator.CreateObject("Stimulsoft.Dashboard.Components.Chart.StiChartElement") as StiComponent;
                    break;

                case "StiGaugeElement":
                    component = StiActivator.CreateObject("Stimulsoft.Dashboard.Components.Gauge.StiGaugeElement") as StiComponent;
                    break;

                case "StiPivotTableElement":
                    component = StiActivator.CreateObject("Stimulsoft.Dashboard.Components.PivotTable.StiPivotTableElement") as StiComponent;
                    break;

                case "StiIndicatorElement":
                    component = StiActivator.CreateObject("Stimulsoft.Dashboard.Components.Indicator.StiIndicatorElement") as StiComponent;
                    break;

                case "StiProgressElement":
                    component = StiActivator.CreateObject("Stimulsoft.Dashboard.Components.Progress.StiProgressElement") as StiComponent;
                    break;

                case "StiRegionMapElement":
                    component = StiActivator.CreateObject("Stimulsoft.Dashboard.Components.RegionMap.StiRegionMapElement") as StiComponent;
                    break;

                case "StiOnlineMapElement":
                    component = StiActivator.CreateObject("Stimulsoft.Dashboard.Components.OnlineMap.StiOnlineMapElement") as StiComponent;
                    break;

                case "StiImageElement":
                    component = StiActivator.CreateObject("Stimulsoft.Dashboard.Components.Image.StiImageElement") as StiComponent;
                    break;

                case "StiTextElement":
                    component = StiActivator.CreateObject("Stimulsoft.Dashboard.Components.Text.StiTextElement") as StiComponent;
                    break;

                case "StiPanelElement":
                    component = StiActivator.CreateObject("Stimulsoft.Dashboard.Components.Panel.StiPanelElement") as StiComponent;
                    break;

                case "StiShapeElement":
                    component = StiActivator.CreateObject("Stimulsoft.Dashboard.Components.Shape.StiShapeElement") as StiComponent;
                    break;

                case "StiListBoxElement":
                    component = StiActivator.CreateObject("Stimulsoft.Dashboard.Components.ListBox.StiListBoxElement") as StiComponent;
                    break;

                case "StiComboBoxElement":
                    component = StiActivator.CreateObject("Stimulsoft.Dashboard.Components.ComboBox.StiComboBoxElement") as StiComponent;
                    break;

                case "StiTreeViewElement":
                    component = StiActivator.CreateObject("Stimulsoft.Dashboard.Components.TreeView.StiTreeViewElement") as StiComponent;
                    break;

                case "StiTreeViewBoxElement":
                    component = StiActivator.CreateObject("Stimulsoft.Dashboard.Components.TreeViewBox.StiTreeViewBoxElement") as StiComponent;
                    break;

                case "StiDatePickerElement":
                    component = StiActivator.CreateObject("Stimulsoft.Dashboard.Components.DatePicker.StiDatePickerElement") as StiComponent;
                    break;

                case "StiCardsElement":
                    component = StiActivator.CreateObject("Stimulsoft.Dashboard.Components.Cards.StiCardsElement") as StiComponent;
                    break;

                case "StiButtonElement":
                    component = StiActivator.CreateObject("Stimulsoft.Dashboard.Components.Button.StiButtonElement") as StiComponent;
                    break;
            }
           
            component.Dockable = false;

            return component;
        }

        public static ArrayList GetDashboardStyles(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            ArrayList styles = new ArrayList();
            var typeComponent = param["typeComponent"] as string;

            var itemAuto = new Hashtable();
            itemAuto["ident"] = "Auto";

            switch (typeComponent)
            {
                case "StiDashboard":
                    {   
                        foreach (var style in StiOptions.Services.Dashboards.DashboardStyles)
                        {
                            var styleObject = new Hashtable();
                            styleObject["ident"] = style.Ident;
                            styleObject["localizedName"] = style.LocalizedName;
                            styleObject["foreColor"] = StiReportHelper.GetHtmlColor(style.ForeColor);
                            styleObject["backColor"] = StiReportHelper.GetHtmlColor(style.BackColor);
                            styleObject["titleBackColor"] = StiReportHelper.GetHtmlColor(style.TitleBackColor);
                            styleObject["titleForeColor"] = StiReportHelper.GetHtmlColor(style.TitleForeColor);
                            styles.Add(styleObject);
                        }
                        break;
                    }
                case "StiTableElement":
                    {
                        styles.Add(itemAuto);

                        #region Report Styles
                        foreach (var style in report.Styles)
                        {
                            if (style is StiTableStyle) {
                                var tableStyle = style as StiTableStyle;
                                var styleObject = new Hashtable();
                                styleObject["name"] = tableStyle.Name;
                                styleObject["ident"] = StiElementStyleIdent.Custom;
                                styleObject["headerBackColor"] = StiReportHelper.GetHtmlColor(tableStyle.HeaderColor);
                                styleObject["cellBackColor"] = StiReportHelper.GetHtmlColor(tableStyle.DataColor);
                                styleObject["alternatingCellBackColor"] = StiReportHelper.GetHtmlColor(tableStyle.AlternatingDataColor);
                                styleObject["lineColor"] = StiReportHelper.GetHtmlColor(tableStyle.GridColor);
                                styles.Add(styleObject);
                            }
                        }
                        #endregion

                        #region Services Styles
                        foreach (var style in StiOptions.Services.Dashboards.TableStyles)
                        {
                            var styleObject = new Hashtable();
                            styleObject["ident"] = style.Ident;
                            styleObject["localizedName"] = style.LocalizedName;
                            styleObject["headerBackColor"] = StiReportHelper.GetHtmlColor(style.HeaderBackColor);
                            styleObject["cellBackColor"] = StiReportHelper.GetHtmlColor(style.CellBackColor);
                            styleObject["alternatingCellBackColor"] = StiReportHelper.GetHtmlColor(style.AlternatingCellBackColor);
                            styleObject["lineColor"] = StiReportHelper.GetHtmlColor(style.LineColor);
                            styles.Add(styleObject);
                        }
                        #endregion;

                        break;
                    }
                case "StiPivotTableElement":
                    {
                        styles.Add(itemAuto);

                        #region Report Styles
                        foreach (var style in report.Styles)
                        {
                            if (style is StiCrossTabStyle)
                            {
                                var crossTabStyle = style as StiCrossTabStyle;
                                var styleObject = new Hashtable();
                                styleObject["name"] = crossTabStyle.Name;
                                styleObject["ident"] = StiElementStyleIdent.Custom;
                                styleObject["columnHeaderBackColor"] = StiReportHelper.GetHtmlColor(crossTabStyle.ColumnHeaderBackColor);
                                styleObject["rowHeaderBackColor"] = StiReportHelper.GetHtmlColor(crossTabStyle.RowHeaderBackColor);
                                styleObject["cellBackColor"] = StiReportHelper.GetHtmlColor(crossTabStyle.CellBackColor);
                                styleObject["alternatingCellBackColor"] = StiReportHelper.GetHtmlColor(crossTabStyle.AlternatingCellBackColor);
                                styleObject["lineColor"] = StiReportHelper.GetHtmlColor(crossTabStyle.LineColor);
                                styles.Add(styleObject);
                            }
                        }
                        #endregion

                        #region Services Styles
                        foreach (var style in StiOptions.Services.Dashboards.PivotStyles)
                        {
                            var styleObject = new Hashtable();
                            styleObject["ident"] = style.Ident;
                            styleObject["localizedName"] = style.LocalizedName;
                            styleObject["columnHeaderBackColor"] = StiReportHelper.GetHtmlColor(style.ColumnHeaderBackColor);
                            styleObject["rowHeaderBackColor"] = StiReportHelper.GetHtmlColor(style.RowHeaderBackColor);
                            styleObject["cellBackColor"] = StiReportHelper.GetHtmlColor(style.CellBackColor);
                            styleObject["alternatingCellBackColor"] = StiReportHelper.GetHtmlColor(style.AlternatingCellBackColor);
                            styleObject["lineColor"] = StiReportHelper.GetHtmlColor(style.LineColor);
                            styles.Add(styleObject);
                        }
                        #endregion

                        break;

                    }
                case "StiRegionMapElement":
                    {                        
                        styles = StiRegionMapElementHelper.GetStylesContent(report, param);
                        styles.Insert(0, itemAuto);
                        break;
                    }
                case "StiChartElement":
                    {
                        styles = StiChartElementHelper.GetStylesContent(report, param);
                        styles.Insert(0, itemAuto);
                        break;
                    }
                case "StiGaugeElement":
                    {
                        styles = StiGaugeElementHelper.GetStylesContent(report, param);
                        styles.Insert(0, itemAuto);
                        break;
                    }
                case "StiProgressElement":
                    {
                        styles = StiProgressElementHelper.GetStylesContent(report, param);
                        styles.Insert(0, itemAuto);
                        break;
                    }
                case "StiIndicatorElement":
                    {
                        styles = StiIndicatorElementHelper.GetStylesContent(report, param);
                        styles.Insert(0, itemAuto);
                        break;
                    }
                case "StiCardsElement":
                    {
                        styles = StiCardsElementHelper.GetStylesContent(report, param);
                        styles.Insert(0, itemAuto);
                        break;
                    }
                case "StiComboBoxElement":
                case "StiListBoxElement":
                case "StiDatePickerElement":
                case "StiTreeViewElement":
                case "StiTreeViewBoxElement":
                case "StiButtonElement":
                    {
                        styles.Add(itemAuto);

                        #region Report Styles
                        foreach (var style in report.Styles)
                        {
                            if (style is StiDialogStyle)
                            {
                                var dialogStyle = style as StiDialogStyle;
                                var styleObject = new Hashtable();
                                styleObject["name"] = dialogStyle.Name;
                                styleObject["ident"] = StiElementStyleIdent.Custom;
                                styleObject["backColor"] = StiReportHelper.GetHtmlColor(dialogStyle.BackColor);
                                styleObject["selectedBackColor"] = StiReportHelper.GetHtmlColor(dialogStyle.SelectedBackColor);
                                styleObject["foreColor"] = StiReportHelper.GetHtmlColor(dialogStyle.ForeColor);
                                styles.Add(styleObject);
                            }
                        }
                        #endregion

                        #region Services Styles
                        foreach (var style in StiOptions.Services.Dashboards.ControlStyles)
                        {
                            var styleObject = new Hashtable();
                            styleObject["ident"] = style.Ident;
                            styleObject["localizedName"] = style.LocalizedName;
                            styleObject["backColor"] = StiReportHelper.GetHtmlColor(style.BackColor);
                            styleObject["selectedBackColor"] = StiReportHelper.GetHtmlColor(style.SelectedBackColor);
                            styleObject["foreColor"] = StiReportHelper.GetHtmlColor(style.ForeColor);
                            styles.Add(styleObject);
                        }
                        #endregion;

                        break;
                    }
            }

            return styles;
        }

        public static string GetDashboardStyleSampleImage(IStiElement element, int width, int height)
        {
            var svgData = new StiSvgData()
            {
                X = 0,
                Y = 0,
                Width = width,
                Height = height,
                Component = element as StiComponent
            };

            var sb = new StringBuilder();

            using (var ms = new StringWriter(sb))
            {
                var writer = new XmlTextWriter(ms);

                writer.WriteStartElement("svg");
                writer.WriteAttributeString("version", "1.1");
                writer.WriteAttributeString("baseProfile", "full");
                writer.WriteAttributeString("baseProfile", "full");

                writer.WriteAttributeString("xmlns", "http://www.w3.org/2000/svg");
                writer.WriteAttributeString("xmlns:xlink", "http://www.w3.org/1999/xlink");
                writer.WriteAttributeString("xmlns:ev", "http://www.w3.org/2001/xml-events");

                writer.WriteAttributeString("height", svgData.Height.ToString());
                writer.WriteAttributeString("width", svgData.Width.ToString());

                var backColor = StiDashboardStyleHelper.GetBackColor(element);
                writer.WriteAttributeString("style", string.Format("background: rgb({0},{1},{2});", backColor.R, backColor.G, backColor.B));

                if (element is IStiGaugeElement)
                    StiGaugeElementSvgHelper.WriteGauge(writer, svgData, 1, true);

                else if (element is IStiProgressElement)
                    StiProgressElementSvgHelper.WriteProgress(writer, svgData, 1, true);

                else if (element is IStiIndicatorElement)
                    StiIndicatorElementSvgHelper.WriteIndicator(writer, svgData, 1, true);

                writer.WriteFullEndElement();
                writer.Flush();
                ms.Flush();
                writer.Close();
                ms.Close();
            }

            return sb.ToString();
        }

        public static void ChangeDashboardStyle(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            var dashboard = report.Pages[param["dashboardName"] as string] as IStiDashboard;
            if (dashboard != null)
            {
                dashboard.Style = (StiElementStyleIdent)Enum.Parse(typeof(StiElementStyleIdent), param["styleIdent"] as string);
                
                callbackResult["elementsProperties"] = new Hashtable();
                callbackResult["dashboardName"] = param["dashboardName"];

                foreach (var element in ((StiPage)dashboard).GetComponents())
                {
                    var elementStyle = element as IStiDashboardElementStyle;
                    if (elementStyle == null || elementStyle.Style == StiElementStyleIdent.Auto)
                    {
                        var elementProps = StiReportEdit.GetAllProperties(element as StiComponent);
                        elementProps["svgContent"] = StiReportEdit.GetSvgContent(element as StiComponent);
                        ((Hashtable)callbackResult["elementsProperties"])[((IStiElement)element).Name] = elementProps;
                    }
                }
            }
            callbackResult["dashboardProperties"] = StiReportEdit.GetAllProperties(dashboard as StiPage);
        }

        public static void ChangeDashboardViewMode(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            var dashboard = report.Pages[param["dashboardName"] as string] as IStiDashboard;
            if (dashboard != null)
            {
                var dashboardViewMode = (StiDashboardViewMode)Enum.Parse(typeof(StiDashboardViewMode), param["dashboardViewMode"] as string);
                dashboard.SwitchSurfaceViewMode(dashboardViewMode == StiDashboardViewMode.Desktop 
                    ? StiSurfaceViewMode.Desktop 
                    : StiSurfaceViewMode.Mobile);

                if (Convert.ToBoolean(param["removeMobileSurface"]))
                    dashboard.RemoveMobileSurface();

                callbackResult["pageName"] = dashboard.Name;
                callbackResult["pageProperties"] = StiReportEdit.GetAllProperties(dashboard as StiPage);
                callbackResult["rebuildProps"] = StiReportEdit.GetPropsRebuildPage(report, dashboard as StiPage);

                var svgContents = new ArrayList();
                ((StiPage)dashboard).GetComponents().ToList().ForEach(c =>
                {
                    if (c is IStiElement)
                    {
                        var compProps = new Hashtable();
                        compProps["name"] = c.Name;
                        compProps["svgContent"] = StiReportEdit.GetSvgContent(c);
                        svgContents.Add(compProps);
                    }
                });
                callbackResult["svgContents"] = svgContents;
            }
        }

        public static void GetMobileViewUnplacedElements(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            var dashboard = report.Pages[param["dashboardName"] as string] as IStiDashboard;
            if (dashboard != null)
            {
                var elements = dashboard.GetUnplacedElements();
                if (elements == null || elements.Count == 0) return;

                var elementsJsArray = new ArrayList();

                elements.ForEach(e =>
                {
                    var comp = (e as StiComponent)?.Clone() as StiComponent;
                    if (comp != null)
                    {
                        var fixHeight = comp is IStiComboBoxElement || comp is IStiTreeViewBoxElement || comp is IStiDatePickerElement;
                        comp.Width = fixHeight ? 115 : 250;
                        comp.Height = fixHeight ? comp.DefaultClientRectangle.Height : 250;

                        var elementJsObj = new Hashtable();
                        elementJsObj["type"] = comp.GetType().Name;
                        elementJsObj["name"] = comp.Name;

                        if (comp is IStiTitleElement)
                            ((IStiTitleElement)comp).Title.Visible = false;

                        if (comp is IStiPanel)
                        {
                            var backColor = StiReportHelper.GetHtmlColor(StiDashboardStyleHelper.GetBackColor(comp as IStiElement));
                            elementJsObj["svgContent"] = StiEncodingHelper.Encode($"<div style=\"display:inline-block; width:110px; height:110px; background:{backColor};\"></div>");
                        }
                        else if (comp is IStiButtonElement)
                        {
                            var buttonElement = comp as IStiButtonElement;
                            var backBrush = (buttonElement as IStiBrush).Brush;
                            var backColor = StiReportHelper.GetHtmlColor(StiBrush.ToColor(backBrush));
                            
                            if (backBrush is StiStyleBrush) {
                                var controlElementStyle = StiDashboardStyleHelper.GetControlStyle(buttonElement);
                                if (controlElementStyle != null) 
                                    backColor = StiReportHelper.GetHtmlColor(controlElementStyle.BackColor);
                            }

                            var textBrush = (buttonElement as IStiTextBrush).TextBrush;
                            var textColor = StiReportHelper.GetHtmlColor(StiBrush.ToColor(textBrush));

                            if (textBrush is StiStyleBrush)
                            {
                                var controlElementStyle = StiDashboardStyleHelper.GetControlStyle(buttonElement);
                                if (controlElementStyle != null)
                                    textColor = StiReportHelper.GetHtmlColor(controlElementStyle.ForeColor);
                            }

                            var font = buttonElement.Font;
                            var borderStyle = buttonElement.ShapeType == StiButtonShapeType.Circle ? "border-radius: 100px;" : "";

                            elementJsObj["svgContent"] = StiEncodingHelper.Encode($"<table style=\"{borderStyle}; display:inline-block;\"><tr><td style=\"overflow: hidden; width:110px; height:110px;" +
                                $"font-family:{font.Name}; font-size:{font.Size}pt; text-align:center; vertical-align:middle; {borderStyle}; background:{backColor}; color:{textColor}\">{buttonElement.Text}</td></tr></table>");
                        }
                        else
                        {
                            elementJsObj["svgContent"] = StiReportEdit.GetSvgContent(comp);
                        }

                        elementsJsArray.Add(elementJsObj);
                    }
                });

                callbackResult["elements"] = elementsJsArray;
            }
        }
        #endregion
    }
}
