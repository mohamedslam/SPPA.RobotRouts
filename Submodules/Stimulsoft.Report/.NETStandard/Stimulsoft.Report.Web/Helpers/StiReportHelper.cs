#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Converters;
using Stimulsoft.Base.Licenses;
using Stimulsoft.Base.Plans;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Chart;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.CrossTab;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Dashboard.Styles;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Engine;
using Stimulsoft.Report.Export;
using Stimulsoft.Report.Helpers;
using Stimulsoft.Report.Painters;
using Stimulsoft.Report.Viewer;
using Stimulsoft.Report.Web.Helpers.Dashboards;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Web;
using System.Drawing.Imaging;
using Stimulsoft.Report.Surface;
using System.Threading.Tasks;
using Stimulsoft.Base.SignatureFonts;

#if NETSTANDARD
using Stimulsoft.System.Web;
using Stimulsoft.System.Security.Cryptography;
#else
using System.Security.Cryptography;
#endif

namespace Stimulsoft.Report.Web
{
    internal class StiReportHelper
    {
        #region Fields.Static
        private static object lockObject = new object();
        #endregion

        #region Methods
        /// <summary>
        /// Get the color in HEX format.
        /// </summary>
        public static string GetHtmlColor(Color color)
        {
            if (color.A < 255)
                return $"rgba({color.R},{color.G},{color.B},{(color.A / 255f).ToString().Replace(",", ".")})";
            else
                return "#" + color.R.ToString("x2") + color.G.ToString("x2") + color.B.ToString("x2");
        }

        public static StiReport GetCompiledReport(StiReport report)
        {
            if (report.CheckNeedsCompiling() && !report.IsCompiled && report.CalculationMode == StiCalculationMode.Compilation && !StiOptions.Engine.ForceInterpretationMode)
            {
                try
                {
                    report.Compile();
                }
                catch
                {
                }
            }

            return report.CompiledReport != null ? report.CompiledReport : report;
        }

        public static string GetCompileErrorHtmlPage(StiReport report)
        {
            if (report.CompilerResults == null || report.CompilerResults.Errors.Count == 0) return null;

            string reportName = report.ReportName;
            if (report.ReportAlias.Length > 0) reportName = report.ReportAlias;

            #region Create HTML error page

            var result = string.Empty;
            result += "<html><head><title>Compilation Error</title><style>\n";
            result += "span.header { font-family: Verdana; font-weight: normal; font-size: 18pt; color: Maroon }\n";
            result += "span.status { font-family: Verdana; font-weight: normal; font-size: 9pt; color: #505050 }\n";
            result += "table { border-right: 1px solid DarkGray; border-top: 1px solid DarkGray }\n";
            result += "td.header { border-left: 1px solid DarkGray; border-bottom: 1px solid DarkGray; background-color: #e0e0e0; font-family: Verdana; font-size: 12pt; padding: 3px 0px 3px 0px; text-align: Center }\n";
            result += "td.error { border-left: 1px solid DarkGray; border-bottom: 1px solid DarkGray; padding: 3px 0px 3px 4px; font-family: Verdana; font-weight: normal; font-size: 12pt; color: #266294 }\n";
            result += "</style></head>\n";
            result += "<body bgcolor=\"White\"><span class=\"header\">Compilation Error in " + reportName + "</span><br><br>\n";
            result += "<table cellspacing=0 cellpadding=0 border=0 width=\"100%\"><tr>\n";
            result += "<td class=\"header\" width=\"30%\">File Name</td><td class=\"header\" width=\"10%\">Line, Column</td>\n";
            result += "<td class=\"header\" width=\"10%\">Error Number</td><td class=\"header\" width=\"50%\">Error Text</td></tr>\n";

            foreach (CompilerError error in report.CompilerResults.Errors)
            {
                result += string.Format("<tr><td class=\"error\">{0}</td><td class=\"error\">{1}, {2}</td><td class=\"error\">{3}</td><td class=\"error\">{4}</td></tr>\n",
                                        error.FileName, error.Line, error.Column, error.ErrorNumber, error.ErrorText);
            }
            
            result += string.Format("</table><br><span class=\"status\">Report {0}, Version: {1}</span></body></html>\n",
                                    report.EngineVersion, StiVersionHelper.ProductVersion);
            #endregion

            return result;
        }

        public static CookieContainer GetCookieContainer()
        {
            if (HttpContext.Current != null) return GetCookieContainer(HttpContext.Current.Request);
            return null;
        }

        public static CookieContainer GetCookieContainer(HttpRequest request)
        {
            if (request != null && request.Cookies.Count > 0)
            {
                var cookieContainer = new CookieContainer();
                foreach (string key in request.Cookies.AllKeys)
                {
                    if (!string.IsNullOrEmpty(request.Cookies[key].Value))
                    {
                        var cookie = new Cookie()
                        {
                            Name = request.Cookies[key].Name,
                            Value = HttpUtility.UrlEncode(request.Cookies[key].Value),
                            Domain = request.Url.Host,
                            Path = "/",
                            Expires = DateTime.Now.AddYears(1)
                        };
                        cookieContainer.Add(cookie);
                    }
                }
                if (cookieContainer.Count > 0) return cookieContainer;
            }

            return null;
        }

        public static string GetReportFileName(StiReport report)
        {
            string fileName = (report.ReportAlias == null || report.ReportAlias.Trim().Length == 0)
                ? report.ReportName
                : report.ReportAlias;

            if (fileName == null || fileName.Trim().Length == 0)
            {
                if (report.ReportFile != null && report.ReportFile.Trim().Length > 0)
                {
                    fileName = report.ReportFile.Replace(".mrt", "").Replace(".mrz", "").Replace(".mrx", "").Replace(".mdc", "").Replace(".mdz", "").Replace(".mdx", "");
                    fileName = fileName.Substring(fileName.LastIndexOf("/") + 1);
                }
                else
                    fileName = "Report";
            }

            return string.Join("_", fileName.Split('<', '>', ':', ';', '"', '/', '\\', '|', '?', '*', '%'));
        }

        public static void ApplyQueryParameters(StiRequestParams requestParams, StiReport report)
        {
            if (requestParams.Server.PassQueryParametersToReport && report != null && report.Dictionary.Variables.Count > 0)
            {
                foreach (string key in requestParams.HttpContext.Request.QueryString)
                {
                    if (key != null && !key.StartsWith("stiweb_") && report.Dictionary.Variables.Contains(key))
                    {
                        if (report.IsCompiled)
                            report[key] = report[key] != null 
                                ? StiConvert.ChangeType(requestParams.HttpContext.Request.QueryString[key], report[key].GetType(), true)
                                : requestParams.HttpContext.Request.QueryString[key];
                        else report.Dictionary.Variables[key].Value = requestParams.HttpContext.Request.QueryString[key];
                    }
                }
            }
        }

        private static List<StiPage> GetNestedPages(StiReport report)
        {
            var pages = report?.Pages.OfType<StiPage>().ToList();
            var nestedPages = new List<StiPage>();
            foreach (StiPage page in pages)
            {
                var comps = page.GetComponents().ToList().Where(c => c is IStiElement);
                foreach (IStiElement comp in comps)
                {
                    var list = comp.GetNestedPages();
                    if (list != null)
                        nestedPages.AddRange(list);
                }
            }

            lock (lockObject)
            {
                StiRenderProviderV2.PrepareSubReportsAndDrillDownPages(report);
                var pages2 = report.Pages.ToList().Where(p => p.IsPage).ToList();

                pages2.Where(p => p.Skip && !nestedPages.Contains(p))
                    .ToList()
                    .ForEach(p => nestedPages.Add(p));

                pages2.ForEach(p => p.Skip = false);
            }

            return nestedPages;
        }

        private static ArrayList GetDashboards(StiReport report, StiRequestParams requestParams)
        {
            var nestedPages = GetNestedPages(report);
            var isReportExist = false;
            var dashboards = new ArrayList();
            foreach (StiPage page in report.Pages)
            {
                var dashboard = page as IStiDashboard;
                var allowToAdd = page.IsEnabled && (requestParams.Viewer.CombineReportPages ? (!isReportExist || dashboard != null) : true);
                if (allowToAdd)
                {
                    var isNestedPage = nestedPages.Contains(page);
                    if (dashboard == null && !isNestedPage) isReportExist = true;

                    var info = new Hashtable();
                    info["type"] = dashboard != null ? StiReportType.Dashboard : StiReportType.Report;
                    info["name"] = page.Name;
                    info["alias"] = string.IsNullOrWhiteSpace(page.Alias) ? page.Name : page.Alias;
                    info["index"] = report.Pages.IndexOf(page);
                    info["isNestedPage"] = isNestedPage;
                    dashboards.Add(info);
                }
            }

            #region LicenseKey
#if CLOUD
            var isValidWeb = StiCloudPlan.IsReportsAvailable(report != null ? report.ReportGuid : null);
            var isValidDbsWeb = StiCloudPlan.IsDashboardsAvailable(report != null ? report.ReportGuid : null);
#elif SERVER
            var isValidWeb = !StiVersionX.IsSvr;
            var isValidDbsWeb = !StiVersionX.IsSvr;
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

            var isValidWeb = isValidKey && StiLicenseKeyValidator.IsValid(StiProductIdent.Web, licenseKey) && typeof(StiLicense).AssemblyQualifiedName.Contains(StiPublicKeyToken.Key);
            var isValidDbsWeb = isValidKey && StiLicenseKeyValidator.IsValid(StiProductIdent.DbsWeb, licenseKey) && typeof(StiLicense).AssemblyQualifiedName.Contains(StiPublicKeyToken.Key);
#endif
            foreach (Hashtable info in dashboards)
            {
                if ((StiReportType)info["type"] == StiReportType.Report && isValidWeb || 
                    (StiReportType)info["type"] == StiReportType.Dashboard && isValidDbsWeb)
                    info["valid"] = true;
            }
            #endregion

            return dashboards;
        }
        #endregion

        #region Interactions
        /// <summary>
        /// Returns information about the presence of interactive report.
        /// </summary>
        public static bool IsReportHasInteractions(StiReport report)
        {
            foreach (StiPage page in report.Pages)
            {
                foreach (StiComponent comp in page.GetComponents())
                {
                    if (IsComponentHasInteraction(comp)) return true;
                }
            }

            return false;
        }

        public static bool IsComponentHasInteraction(StiComponent comp)
        {
            if (comp.Interaction != null)
            {
                if (comp.Interaction.SortingEnabled && !string.IsNullOrWhiteSpace(comp.Interaction.SortingColumn)) return true;
                if (comp.Interaction.DrillDownEnabled && (!string.IsNullOrEmpty(comp.Interaction.DrillDownPageGuid) || !string.IsNullOrEmpty(comp.Interaction.DrillDownReport))) return true;
                if (comp.Interaction is StiBandInteraction && ((StiBandInteraction)comp.Interaction).CollapsingEnabled) return true;
                if (comp is StiChart && StiOptions.Engine.AllowInteractionInChartWithComponents) return true;
            }
            if (comp is StiChart)
            {
                foreach (StiSeries series in ((StiChart)comp).Series)
                {
                    if (series.Interaction != null && series.Interaction.DrillDownEnabled) return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Applying the signatures to the report.
        /// </summary>
        public static void ApplySignatures(StiReport report, ArrayList signatures)
        {
            if (signatures == null || signatures.Count == 0) return;

            foreach (Hashtable signParams in signatures)
            {
                var signComp = report.GetComponentByName(signParams["name"] as string) as StiElectronicSignature;
                if (signComp != null)
                {
                    signComp.Type.FullName = signParams["typeFullName"] as string;
                    signComp.Type.Initials = signParams["typeInitials"] as string;
                    signComp.Type.Style = (StiSignatureStyle)Enum.Parse(typeof(StiSignatureStyle), signParams["typeStyle"] as string);
                    signComp.Mode = (StiSignatureMode)Enum.Parse(typeof(StiSignatureMode), signParams["signatureMode"] as string);

                    signComp.Text.Text = Encoding.UTF8.GetString(Convert.FromBase64String(signParams["drawText"] as string));
                    signComp.Text.Font = StiElectronicSignatureHelper.JsonToFont(signParams["drawTextFont"] as Hashtable);
                    signComp.Text.Color = StiElectronicSignatureHelper.StringToColor(signParams["drawTextColor"] as string);
                    signComp.Text.HorAlignment = (StiTextHorAlignment)Enum.Parse(typeof(StiTextHorAlignment), signParams["drawTextHorAlignment"] as string);

                    signComp.Image.Image = !string.IsNullOrEmpty(signParams["drawImage"] as string) ? StiElectronicSignatureHelper.Base64ToByteArray(signParams["drawImage"] as string) : null;
                    signComp.Image.HorAlignment = (StiHorAlignment)Enum.Parse(typeof(StiHorAlignment), signParams["drawImageHorAlignment"] as string);
                    signComp.Image.VertAlignment = (StiVertAlignment)Enum.Parse(typeof(StiVertAlignment), signParams["drawImageVertAlignment"] as string);
                    signComp.Image.Stretch = Convert.ToBoolean(signParams["drawImageStretch"]);
                    signComp.Image.AspectRatio = Convert.ToBoolean(signParams["drawImageAspectRatio"]);
                }
            }

            report.IsRendered = false;
        }

        /// <summary>
        /// Apply the parameters for sorting and rebuild the report
        /// </summary>
        public static void ApplySorting(StiReport report, Hashtable parameters)
        {
            if (parameters == null || parameters.Count == 0) return;
            string[] values = parameters["ComponentName"].ToString().Split(';');
            StiComponent comp = report.GetComponentByName(values[0]);
            var isCtrl = bool.Parse(values[1]);

            values = parameters["DataBand"].ToString().Split(';');
            StiDataBand dataBand = report.GetComponentByName(values[0]) as StiDataBand;
            if (dataBand != null) dataBand.Sort = values.Where((val, i) => i != 0 && !string.IsNullOrEmpty(val)).ToArray();

            #region Apply interaction sorting

            if (comp != null && dataBand != null)
            {
                string dataBandColumnString = comp.Interaction.GetSortColumnsString();

                if ((dataBand != null) && StiOptions.Engine.Interaction.ForceSortingWithFullTypeConversion)
                {
                    if (report.CalculationMode == StiCalculationMode.Interpretation || StiOptions.Engine.ForceInterpretationMode)
                    {
                        if (dataBand.DataSource != null)
                            dataBandColumnString = "{" + StiNameValidator.CorrectName(dataBand.DataSource.Name, report) + "." + dataBandColumnString + "}";
                    }
                    else
                    {
                        dataBandColumnString = "{Get" + StiNameValidator.CorrectName(dataBand.Name + "." + dataBandColumnString, report) + "_Sort}";
                    }
                }

                if (dataBand.Sort == null || dataBand.Sort.Length == 0)
                {
                    dataBand.Sort = StiSortHelper.AddColumnToSorting(dataBand.Sort, dataBandColumnString, true);
                }
                else
                {
                    int sortIndex = StiSortHelper.GetColumnIndexInSorting(dataBand.Sort, dataBandColumnString);
                    if (isCtrl)
                    {
                        if (sortIndex == -1) dataBand.Sort = StiSortHelper.AddColumnToSorting(dataBand.Sort, dataBandColumnString, true);
                        else dataBand.Sort = StiSortHelper.ChangeColumnSortDirection(dataBand.Sort, dataBandColumnString);
                    }
                    else
                    {
                        if (sortIndex != -1)
                        {
                            StiInteractionSortDirection direction = StiSortHelper.GetColumnSortDirection(dataBand.Sort, dataBandColumnString);

                            if (direction == StiInteractionSortDirection.Ascending) direction = StiInteractionSortDirection.Descending;
                            else direction = StiInteractionSortDirection.Ascending;

                            dataBand.Sort = StiSortHelper.AddColumnToSorting(new string[0], dataBandColumnString, direction == StiInteractionSortDirection.Ascending);
                            comp.Interaction.SortingDirection = direction;
                        }
                        else
                        {
                            dataBand.Sort = StiSortHelper.AddColumnToSorting(new string[0], dataBandColumnString, true);
                            comp.Interaction.SortingDirection = StiInteractionSortDirection.Ascending;
                        }
                    }
                }

                report.IsRendered = false;
            }

            #endregion
        }

        /// <summary>
        /// Apply the parameters for collapsing and rebuild the report
        /// </summary>
        public static void ApplyCollapsing(StiReport report, Hashtable parameters)
        {
            string componentName = parameters["ComponentName"] as string;

            StiComponent comp = report.GetComponentByName(componentName);
            IStiInteraction interactionComp = comp as IStiInteraction;

            if (interactionComp != null && interactionComp.Interaction != null)
            {
                report.InteractionCollapsingStates = parameters["CollapsingStates"] as Hashtable;

                #region Replase 'string' keys to 'int' keys

                foreach (Hashtable states in report.InteractionCollapsingStates.Values)
                {
                    Hashtable statesCopy = states.Clone() as Hashtable;
                    foreach (var obj in statesCopy.Keys)
                    {
                        if (obj is string)
                        {
                            int num = int.Parse((string)obj);
                            states.Add(num, states[obj]);
                            states.Remove(obj);
                        }
                    }
                    statesCopy = null;
                }

                #endregion

                #region CrossHeader Collapsing

                StiCrossHeaderInteraction crossHeaderInteraction = interactionComp.Interaction as StiCrossHeaderInteraction;
                if (crossHeaderInteraction != null && crossHeaderInteraction.CollapsingEnabled)
                {
                    StiCrossHeader header = comp as StiCrossHeader;
                    StiCrossTabV2Builder.SetCollapsed(header, !StiCrossTabV2Builder.IsCollapsed(header));
                }

                #endregion

                report.IsRendered = false;
            }
        }

        /// <summary>
        /// Apply the parameters for drill-down and build the detailed report
        /// </summary>
        public static StiReport ApplyDrillDown(StiReport report, StiReport renderedReport, Hashtable parameters, Hashtable drillDownParameters)
        {
            if (renderedReport == null) renderedReport = report;
            if (parameters == null || parameters.Count == 0) return renderedReport;

            int pageIndex = Convert.ToInt32(parameters["PageIndex"]);
            int componentIndex = Convert.ToInt32(parameters["ComponentIndex"]);
            int elementIndex = Convert.ToInt32(parameters["ElementIndex"]);
            string pageGuid = Convert.ToString(parameters["PageGuid"]);
            string reportFile = Convert.ToString(parameters["ReportFile"]);

            StiPage drillDownPage = null;
            StiReport newReport = report;

            if (!renderedReport.IsRendered)
            {
                try
                {
                    renderedReport.Render(false);
                }
                catch { }
            }

            #region Drill-down page
            if (!string.IsNullOrEmpty(pageGuid))
            {
                #region Get drill-down page and disable all pages except drill-down page

                foreach (StiPage page in report.Pages)
                {
                    if (page.Guid == pageGuid)
                    {
                        drillDownPage = page;
                        page.Enabled = true;
                        page.Skip = false;
                    }
                    else
                        page.Enabled = false;
                }

                #endregion

                #region Clear any reference to drill-down page from other components in report
                // We need do this because during report rendering drill-down pages is skipped

                StiComponentsCollection comps = report.GetComponents();
                foreach (StiComponent comp in comps)
                {
                    #region Components
                    if (comp.Interaction != null &&
                        comp.Interaction.DrillDownEnabled &&
                        comp.Interaction.DrillDownPageGuid == drillDownPage?.Guid)
                    {
                        comp.Interaction.DrillDownPage = null;
                    }
                    #endregion

                    #region Charts
                    if (comp is StiChart)
                    {
                        StiChart chart = comp as StiChart;
                        foreach (StiSeries series in chart.Series)
                        {
                            StiSeriesInteraction seriesInteraction = (StiSeriesInteraction)series.Interaction;
                            if (series.Interaction != null &&
                                seriesInteraction.DrillDownEnabled &&
                                seriesInteraction.DrillDownPageGuid == drillDownPage?.Guid)
                            {
                                seriesInteraction.DrillDownPage = null;
                            }
                        }
                    }
                    #endregion
                }

                #endregion
            }
            #endregion

            #region Check drill-down for a report file
            else if (!string.IsNullOrEmpty(reportFile))
            {
                newReport = new StiReport();
                if (reportFile.StartsWith(StiHyperlinkProcessor.ResourceIdent))
                {
                    var resourceName = reportFile.Replace(StiHyperlinkProcessor.ResourceIdent, "");
                    if (renderedReport.Dictionary.Resources.Contains(resourceName))
                    {
                        newReport.Load(renderedReport.Dictionary.Resources[resourceName].Content);
                    }
                }
                else
                {
                    newReport.Load(reportFile);
                }
                newReport = GetCompiledReport(newReport);
            }
            #endregion

            #region Fill report alias, description
            if (report.ReportAlias == newReport.ReportAlias && drillDownPage != null) newReport.ReportAlias = string.IsNullOrEmpty(drillDownPage.Alias) ? drillDownPage.Name : drillDownPage.Alias;
            if (report.ReportDescription == newReport.ReportDescription) newReport.ReportDescription = newReport.ReportAlias;
            #endregion

            #region Fill drill-down parameters
            StiPage renderedPage = renderedReport.RenderedPages[pageIndex];
            StiComponent interactionComp = renderedPage.Components[componentIndex];
            if (interactionComp != null && interactionComp.DrillDownParameters != null)
            {
                foreach (KeyValuePair<string, object> entry in interactionComp.DrillDownParameters)
                {                    
                    newReport[entry.Key] = entry.Value;
                    drillDownParameters[entry.Key] = entry.Value;
                    
                    if (entry.Key != null && entry.Key.ToLower() == "title" && entry.Value != null && !string.IsNullOrEmpty(entry.Value.ToString()))
                    {
                        newReport.ReportAlias = entry.Value.ToString();
                    }
                }
            }

            #region Fill chart drill-down parameters
            var interactionChart = interactionComp as StiChart;
            if (interactionChart != null)
            {
                StiGdiContextPainter painter = new StiGdiContextPainter(StiReport.GlobalMeasureGraphics);
                StiContext context = new StiContext(painter, true, false, false, 1);
                RectangleD rect = interactionChart.Report.Unit.ConvertToHInches(interactionChart.ClientRectangle);
                StiCellGeom chartGeom = interactionChart.Core.Render(context, new RectangleF(0, 0, (float)rect.Width, (float)rect.Height), true);
                List<StiCellGeom> seriesGeomItems = chartGeom.GetSeriesElementGeoms();
                StiSeriesElementGeom seriesElementGeom = elementIndex != -1 ? seriesGeomItems[elementIndex] as StiSeriesElementGeom : null;
                if (seriesElementGeom != null && seriesElementGeom.Interaction != null)
                {
                    newReport["Series"] = seriesElementGeom.Series.Core;
                    newReport["SeriesIndex"] = seriesElementGeom.Series.Core.Series.Chart.Series.IndexOf(seriesElementGeom.Series.Core.Series);
                    newReport["SeriesArgument"] = seriesElementGeom.Interaction.Argument;
                    newReport["SeriesValue"] = seriesElementGeom.Interaction.Value;
                    newReport["SeriesEndValue"] = seriesElementGeom.Interaction.EndValue;
                    newReport["SeriesPointIndex"] = seriesElementGeom.Interaction.PointIndex;
                    newReport["SeriesTag"] = seriesElementGeom.Interaction.Tag;
                    newReport["SeriesHyperlink"] = seriesElementGeom.Interaction.Hyperlink;
                    newReport["SeriesTooltip"] = seriesElementGeom.Interaction.Tooltip;
                    newReport["SeriesTitle"] = seriesElementGeom.Series.CoreTitle;

                    #region Report Alias

                    string arg1 = seriesElementGeom.Interaction.Series != null ? seriesElementGeom.Interaction.Series.CoreTitle : null;
                    string arg2 = seriesElementGeom.Interaction.Argument != null ? seriesElementGeom.Interaction.Argument.ToString() : null;

                    if (string.IsNullOrEmpty(arg2)) arg2 = seriesElementGeom.Interaction.Value.ToString();
                    
                    if (!string.IsNullOrEmpty(arg1) && !string.IsNullOrEmpty(arg2)) newReport.ReportAlias = string.Format("{0} - {1}", arg1, arg2);
                    else if (!string.IsNullOrEmpty(arg2)) newReport.ReportAlias = arg1;
                    else newReport.ReportAlias = arg2;

                    #endregion
                }
            }
            #endregion

            #endregion

            #region Render new report
            try
            {
                newReport.IsInteractionRendering = true;
                try
                {
                    newReport.Render(false);
                }
                catch { }
            }
            finally
            {
                newReport.IsInteractionRendering = false;
            }
            #endregion

            return newReport;
        }

        /// <summary>
        /// Apply the parameters for dashboard drill-down and build the detailed dashboard
        /// </summary>
        public static StiReport ApplyDashboardDrillDown(StiReport report, Hashtable drillDownParameters)
        {
            if (drillDownParameters == null || drillDownParameters.Count == 0) return report;

            var drillDownPageKey = drillDownParameters["drillDownPageKey"] as string;
            var parameters = drillDownParameters["parameters"] as ArrayList;
            var value = drillDownParameters["value"] as string;

            var cloneReport = StiReportCopier.CloneReport(report);
            cloneReport.Key = StiKeyHelper.GenerateKey();

            cloneReport.Pages.ToList().ForEach(p => { p.Enabled = p.Guid == drillDownPageKey; });

            StiReportCopier.CopyElementsDrillDown(report, cloneReport);

            cloneReport.Pages.ToList().ForEach(p =>
            {
                if (p.Guid == drillDownPageKey)
                    cloneReport.ReportAlias = !string.IsNullOrEmpty(p.Alias) ? p.Alias : p.Name;
            });

            if (parameters != null)
            {
                parameters?.Cast<Hashtable>().ToList().ForEach(p =>
                {
                    var pValue = p["value"];
                    var pKey = p["key"]?.ToString();
                    try
                    {
                        var strValue = pValue as string;
                        if (!string.IsNullOrEmpty(strValue))
                        {
                            if (strValue.Contains("{") && strValue.Contains("}"))
                            {
                                pValue = StiReportParser.Parse(strValue, cloneReport.Pages.ToList().FirstOrDefault(page => page.Guid == drillDownPageKey), false);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    cloneReport[pKey] = pValue;
                });
            }

            if (cloneReport.ContainsDashboard)
            {
                StiDataFiltersHelper.ApplyDefaultFiltersForFilterElements(cloneReport);
            }

            return cloneReport;
        }
        #endregion

        #region Report Pages
        private static int Round(double value)
        {
            return (int)Math.Round(value, MidpointRounding.AwayFromZero);
        }

        private static Rectangle Round(RectangleD rect)
        {
            return new Rectangle(Round(rect.Left), Round(rect.Top), Round(rect.Width), Round(rect.Height));
        }

        private static List<StiRangeBand> GetElementsPositions(List<IStiElement> elements, IStiDashboard dashboard)
        {
            // All elements client rectangles
            var rects = elements
                .Cast<StiComponent>()
                .ToDictionary(e => e as IStiElement, e => Round(e.GetPaintRectangle(false, false)));

            // All top and bottom coordinates of the elements to a list
            var yy = new List<int>();
            foreach (var element in elements)
            {
                yy.Add(Round(rects[element].Top));
                yy.Add(Round(rects[element].Bottom));
            }

            yy.Add(0);
            yy.Add(Round(((StiComponent)dashboard).Height));
            yy = yy.Distinct().OrderBy(y => y).ToList();

            // Creates a list of the all element positions
            var bands = new List<StiRangeBand>();
            for (var index = 0; index < yy.Count - 1; index++)
            {
                bands.Add(new StiRangeBand(yy[index], yy[index + 1]));
            }

            // Detects all fixed bands
            foreach (var element in elements.Where(e => (e is IStiFixedHeightElement) && (e as IStiFixedHeightElement).IsFixedHeight))
            {
                bands.Where(b => b.Intersect(rects[element])).ToList().ForEach(c => c.IsFixed = true);
            }

            return bands;
        }

        private static void CorrectElementLocations(StiPage page, List<IStiElement> elements)
        {
            var dashboard = page as IStiDashboard;

            foreach (StiComponent element in elements)
            {
                if (element.Page != dashboard) continue;

                if (element.Right > page.Width)
                    element.Left = page.Width - element.Width;

                if (element.Bottom > page.Height)
                    element.Top = page.Height - element.Height;

                if (element.Left < 0)
                {
                    element.Left = 0;
                    if (element.Width > page.Width)
                        element.Width = page.Width;
                }

                if (element.Top < 0)
                {
                    element.Top = 0;
                    if (element.Height > page.Height)
                        element.Height = page.Height;
                }
            }
        }

        private static Hashtable GetDashboardPage(StiReport report, int pageIndex, StiRequestParams requestParams)
        {
            var pageAttributes = new Hashtable();
            var page = report.Pages[pageIndex];
            var dashboard = (IStiDashboard)page;
            var elements = dashboard.GetElements(true).Where(e => e.IsEnabled && ((IStiElement)((StiComponent)e).Parent).IsEnabled).ToList();
            var renderSingleElement = !string.IsNullOrEmpty(requestParams.Viewer.ElementName) && requestParams.Action != StiAction.DashboardDrillDown;
            var totalFixedHeight = 0;
            List<StiRangeBand> bands = null;

            if (requestParams.Viewer.DashboardWidth == 0 && requestParams.Viewer.DashboardHeight == 0)
            {
                //fix for an exceptional situation when the viewer is hidden and sizes are not available
                requestParams.Viewer.DashboardWidth = Convert.ToInt32(page.Width);
                requestParams.Viewer.DashboardHeight = Convert.ToInt32(page.Height);
            }

            CorrectElementLocations(page, elements);

            if (dashboard.IsMobileSurfacePresent)
                dashboard.SwitchSurfaceViewMode(
                    requestParams.Viewer.DashboardWidth <= dashboard.DeviceWidth 
                        ? StiSurfaceViewMode.Mobile 
                        : StiSurfaceViewMode.Desktop);

            if (!renderSingleElement)
                CalculatePositionForEachBand(requestParams, elements, page, out bands, out totalFixedHeight);

            var pageSize = new Hashtable();
            pageAttributes["width"] = requestParams.Viewer.DashboardWidth;
            pageAttributes["height"] = requestParams.Viewer.DashboardHeight;
            pageAttributes["margin"] = page.Margins;
            pageAttributes["background"] = GetHtmlColor(StiDashboardStyleHelper.GetDashboardBackColor(dashboard, true));
            pageAttributes["actionColors"] = StiDashboardElementViewHelper.GetActionColors(dashboard);
            pageAttributes["contentAlignment"] = dashboard.ContentAlignment.ToString();
            pageAttributes["dashboardWatermark"] = StiDashboardElementViewHelper.GetDashboardWatermark(dashboard);

            var elementsList = new ArrayList();
            foreach (var element in elements)
            {
                if (!renderSingleElement || element.Name == requestParams.Viewer.ElementName)
                {
                    if (((StiComponent)element).ClientRectangle.IsEmpty) continue;

                    elementsList.Add(GetElementAttributes(page, element, renderSingleElement, requestParams, bands, totalFixedHeight));
                }
            }

            pageAttributes["elements"] = elementsList;

            return pageAttributes;
        }

        internal static Hashtable GetElementAttributes(StiPage page, IStiElement element, bool renderSingleElement, StiRequestParams requestParams, List<StiRangeBand> bands = null, int totalFixedHeight = 0)
        {   
            double? oldElementHeight = null;
            var component = (StiComponent)element;
            var contentAlignment = ((IStiDashboard)page).ContentAlignment;

            var elementAttributes = new Hashtable();
            elementAttributes["name"] = element.Name;
            elementAttributes["type"] = element.GetType().Name;
            elementAttributes["group"] = (element as IStiGroupElement)?.Group;
            elementAttributes["key"] = element.Key;

            // Use default margin for single full-screen element
            var elementMargin = ((IStiMargin)element).Margin;
            if (renderSingleElement) ((IStiMargin)element).Margin = new StiMargin();
            var margin = ((IStiMargin)element).Margin;
            var padding = ((IStiPadding)element).Padding;

            // Calculate ScaleX and ScaleY
            var scaleX = requestParams.Viewer.DashboardWidth / (renderSingleElement ? component.Width : page.Width);
            var scaleY = (element is IStiFixedHeightElement) && (element as IStiFixedHeightElement).IsFixedHeight
                ? 1 
                : (requestParams.Viewer.DashboardHeight - totalFixedHeight) / (renderSingleElement ? component.Height : page.Height - totalFixedHeight);

            #region Correct Positions By ContentAlignment
            if (contentAlignment == StiDashboardContentAlignment.Center ||
                contentAlignment == StiDashboardContentAlignment.Left ||
                contentAlignment == StiDashboardContentAlignment.Right)
                scaleX = 1;

            if (contentAlignment == StiDashboardContentAlignment.Center ||
                contentAlignment == StiDashboardContentAlignment.Left ||
                contentAlignment == StiDashboardContentAlignment.Right)
                scaleY = 1;

            if (contentAlignment == StiDashboardContentAlignment.StretchX)
                scaleY = scaleX;

            var offsetX = 0;

            if (contentAlignment == StiDashboardContentAlignment.Right)
                offsetX = requestParams.Viewer.DashboardWidth - Convert.ToInt32(page.Width);

            if (contentAlignment == StiDashboardContentAlignment.Center)
                offsetX = (requestParams.Viewer.DashboardWidth - Convert.ToInt32(page.Width)) / 2;
            #endregion

            elementAttributes["left"] = 0;
            elementAttributes["top"] = 0;
            elementAttributes["width"] = Round(component.Width * scaleX) - margin.Left - margin.Right;
            elementAttributes["height"] = Round(component.Height * scaleY) - margin.Top - margin.Bottom;
            elementAttributes["margin"] = $"{Round(margin.Top)},{Round(margin.Right)},{Round(margin.Bottom)},{Round(margin.Left)}";
            elementAttributes["padding"] = $"{Round(padding.Top)},{Round(padding.Right)},{Round(padding.Bottom)},{Round(padding.Left)}";
            elementAttributes["parentKey"] = (element as IStiFilterElement)?.GetParentKey();
            elementAttributes["backColor"] = StiDashboardElementViewHelper.GetBackColor(element);
            elementAttributes["foreColor"] = StiDashboardElementViewHelper.GetForeColor(element);
            elementAttributes["border"] = StiDashboardElementViewHelper.GetBorder(element);
            elementAttributes["font"] = StiDashboardElementViewHelper.GetFont(element);
            elementAttributes["layout"] = StiDashboardElementViewHelper.GetLayout(element);
            elementAttributes["actionColors"] = StiDashboardElementViewHelper.GetActionColors(element);
            elementAttributes["shadow"] = StiDashboardElementViewHelper.GetShadow(element);
            elementAttributes["cornerRadius"] = StiDashboardElementViewHelper.GetCornerRadius((element as IStiCornerRadius)?.CornerRadius);
            elementAttributes["dashboardWatermark"] = StiDashboardElementViewHelper.GetDashboardWatermark(element);

            if (!renderSingleElement)
            {
                var componentClientRectangle = Round(component.GetPaintRectangle(false, false));
                elementAttributes["left"] = Round(componentClientRectangle.Left * scaleX) + offsetX;
                elementAttributes["top"] = bands.FirstOrDefault(b => b.OriginalTop == componentClientRectangle.Top).Top;

                var bandsBottom = bands.FirstOrDefault(b => b.OriginalBottom == componentClientRectangle.Bottom);
                var bandsTop = bands.FirstOrDefault(b => b.OriginalTop == componentClientRectangle.Top);

                var correctedHeight = (bandsBottom != null ? bandsBottom.Bottom : 0) - (bandsTop != null ? bandsTop.Top : 0) - margin.Top - margin.Bottom;
                if (correctedHeight != (double)elementAttributes["height"])
                {
                    oldElementHeight = component.Height;
                    component.Height = Round(correctedHeight / scaleY) + margin.Top + margin.Bottom;
                }
                elementAttributes["height"] = correctedHeight;
            }

            elementAttributes["contentAttributes"] = StiDashboardElementViewHelper.GetElementContentAttributes(element, scaleX, scaleY, requestParams);

            if (oldElementHeight != null) component.Height = (double)oldElementHeight;            
            ((IStiMargin)element).Margin = elementMargin;

            return elementAttributes;
        }

        internal static Hashtable GetSingleElementContent(StiReport report, StiRequestParams requestParams)
        {   
            var elementName = requestParams.GetString("elementNameForRefreshing");
            var element = report != null && elementName != null ? report.Pages.GetComponentByName(elementName) as IStiElement : null;
            if (element != null)
            {
                var renderSingleElement = !string.IsNullOrEmpty(requestParams.Viewer.ElementName) && requestParams.Action != StiAction.DashboardDrillDown;
                var elements = ((IStiDashboard)element.Page).GetElements(true).Where(e => e.IsEnabled).ToList();
                var totalFixedHeight = 0;
                List<StiRangeBand> bands = null;

                if (!renderSingleElement)
                    CalculatePositionForEachBand(requestParams, elements, element.Page, out bands, out totalFixedHeight, element as StiComponent);

                return GetElementAttributes(element.Page, element, renderSingleElement, requestParams, bands, totalFixedHeight);
            }
            
            return null;
        }

        internal static void CalculatePositionForEachBand(StiRequestParams requestParams, List<IStiElement> elements, StiPage page,  out List<StiRangeBand> bands, out int totalFixedHeight, StiComponent component = null)
        {
            var dashboard = page as IStiDashboard;
            bands = GetElementsPositions(elements, dashboard);

            totalFixedHeight = bands.Where(b => b.IsFixed).Sum(b => b.Height);
            var contentAlignment = dashboard.ContentAlignment;
            var scaleX = requestParams.Viewer.DashboardWidth / (component != null ? component.Width : page.Width);
            var scaleY = (requestParams.Viewer.DashboardHeight - totalFixedHeight) / (page.Height - totalFixedHeight);
            var posY = 0;
            
            if (contentAlignment == StiDashboardContentAlignment.Center ||
                contentAlignment == StiDashboardContentAlignment.Left ||
                contentAlignment == StiDashboardContentAlignment.Right)
                scaleY = 1;

            if (contentAlignment == StiDashboardContentAlignment.StretchX)
                scaleY = scaleX;

            foreach (var band in bands)
            {
                band.Top = posY;
                band.Bottom = band.Top + Round(band.IsFixed ? band.OriginalHeight : band.OriginalHeight * scaleY);
                posY += band.Bottom - band.Top;
            }
        }

        private static Hashtable GetReportPage(StiReport report, StiHtmlExportService service, int pageIndex, StiRequestParams requestParams)
        {
            var settings = new StiHtmlExportSettings();
            settings.PageRange = new StiPagesRange(pageIndex);
            settings.Zoom = requestParams.Viewer.Zoom;
            settings.ImageFormat = null; // ImageFormat.Png;    //null for Auto-Format
            settings.ImageQuality = 0.85f;
            settings.ExportQuality = StiHtmlExportQuality.High;
            settings.ExportBookmarksMode = StiHtmlExportBookmarksMode.ReportOnly;
            settings.RemoveEmptySpaceAtBottom = false;
            settings.OpenLinksTarget = requestParams.Viewer.OpenLinksWindow;
            settings.ChartType = (StiHtmlChartType)Enum.Parse(typeof(StiHtmlChartType), requestParams.Viewer.ChartRenderType.ToString());
            settings.UseWatermarkMargins = true;
            settings.AddPageBreaks = false;

            switch (requestParams.Viewer.ReportDisplayMode)
            {
                case StiReportDisplayMode.FromReport:
                    settings.ExportMode = GetReportDisplayModeFromReport(report);
                    break;

                case StiReportDisplayMode.Table:
                    settings.ExportMode = StiHtmlExportMode.Table;
                    break;

                case StiReportDisplayMode.Div:
                    settings.ExportMode = StiHtmlExportMode.Div;
                    break;

                case StiReportDisplayMode.Span:
                    settings.ExportMode = StiHtmlExportMode.Span;
                    break;
            }

            switch (requestParams.Viewer.ImagesQuality)
            {
                case StiImagesQuality.Low:
                    settings.ImageResolution = 50f;
                    break;

                case StiImagesQuality.Normal:
                    settings.ImageResolution = 100f;
                    break;

                case StiImagesQuality.High:
                    settings.ImageResolution = 200f;
                    break;
            }

            var stream = new MemoryStream();
            service.ExportHtml(report, stream, settings);
            var htmlPageContent = Encoding.UTF8.GetString(stream.ToArray()).Substring(1);
            stream.Close();

            var pageAttr = new Hashtable();
            pageAttr["content"] = report.RenderedPages.Count > 0 ? htmlPageContent : string.Empty;

            StiPage page = report.RenderedPages.Count > 0 ? report.RenderedPages[Math.Min(pageIndex, report.RenderedPages.Count - 1)] : new StiPage(report);
            var scaleX = StiOptions.Export.Html.PrintLayoutOptimization && settings.ExportMode == StiHtmlExportMode.Div ? 0.96 : 1;
            var scaleY = StiOptions.Export.Html.PrintLayoutOptimization && settings.ExportMode == StiHtmlExportMode.Div ? 0.956 : 1;

            pageAttr["margins"] = string.Format("{0}px {1}px {2}px {3}px",
                Math.Round(report.Unit.ConvertToHInches(page.Margins.Top) * requestParams.Viewer.Zoom),
                Math.Round(report.Unit.ConvertToHInches(page.Margins.Right) * requestParams.Viewer.Zoom),
                Math.Round(report.Unit.ConvertToHInches(page.Margins.Bottom) * requestParams.Viewer.Zoom),
                Math.Round(report.Unit.ConvertToHInches(page.Margins.Left) * requestParams.Viewer.Zoom));
            pageAttr["sizes"] = string.Format("{0};{1}",
                    Math.Round(report.Unit.ConvertToHInches(page.PageWidth * page.SegmentPerWidth * scaleX) * requestParams.Viewer.Zoom),
                    Math.Round(report.Unit.ConvertToHInches(page.PageHeight * page.SegmentPerHeight * scaleY) * requestParams.Viewer.Zoom));
            pageAttr["brush"] = BrushToStr(page.Brush);
            pageAttr["existsWatermark"] = (page.Watermark.Image != null || !String.IsNullOrEmpty(page.Watermark.ImageHyperlink) || !String.IsNullOrEmpty(page.Watermark.Text)) && page.Watermark.Enabled;

            return pageAttr;
        }

        private static StiHtmlExportMode GetReportDisplayModeFromReport(StiReport report)
        {
            return report?.HtmlPreviewMode == StiHtmlPreviewMode.Div ? StiHtmlExportMode.Div : StiHtmlExportMode.Table;
        }

        private static int GetPagesCount(StiReport report, StiRequestParams requestParams)
        {
            if (!requestParams.Viewer.CombineReportPages && IsMixedReport(report) && report.Pages.Count > requestParams.Viewer.OriginalPageNumber)
            {
                string pageName = report.Pages[requestParams.Viewer.OriginalPageNumber].Name;
                var startIndex = report.RenderedPages.ToList().FindIndex(p => p.Name == pageName);
                var endIndex = report.RenderedPages.ToList().FindLastIndex(p => p.Name == pageName);

                if (endIndex >= 0 && startIndex >= 0)
                    return endIndex - startIndex + 1;
            }
            return report.RenderedPages.Count;
        }

        private static string BrushToStr(StiBrush brush)
        {   
            if (brush is StiEmptyBrush)
            {
                return "0";
            }
            else if (brush is StiSolidBrush)
            {
                var solid = brush as StiSolidBrush;
                return string.Format("1;{0}", 
                    GetHtmlColor(solid.Color));
            }
            else if (brush is StiHatchBrush)
            {
                var hatch = brush as StiHatchBrush;
                return string.Format("2;{0};{1};{2}",
                    GetHtmlColor(hatch.ForeColor), 
                    GetHtmlColor(hatch.BackColor),
                    ((int)hatch.Style).ToString());
            }
            else if (brush is StiGradientBrush)
            {
                var gradient = brush as StiGradientBrush;
                return string.Format("3;{0};{1};{2}",
                    GetHtmlColor(gradient.StartColor),
                    GetHtmlColor(gradient.EndColor),
                    gradient.Angle.ToString());
            }
            else if (brush is StiGlareBrush)
            {
                var glare = brush as StiGlareBrush;
                return string.Format("4;{0};{1};{2};{3};{4}",
                    GetHtmlColor(glare.StartColor),
                    GetHtmlColor(glare.EndColor),
                    glare.Angle.ToString(),
                    glare.Focus.ToString(),
                    glare.Scale.ToString());
            }
            else if (brush is StiGlassBrush)
            {
                var glass = brush as StiGlassBrush;
                return string.Format("5;{0};{1};{2};{3}",
                    GetHtmlColor(glass.Color),                    
                    glass.Blend.ToString(),
                    glass.DrawHatch,
                    GetHtmlColor(StiColorUtils.Light(glass.Color, (byte)(64 * glass.Blend))));
            }

            return "";
        }

        private static bool IsMixedReport(StiReport report)
        {
            if (report != null)
            {
                var nestedPages = GetNestedPages(report);
                var reportsPresent = false;
                var dashboardsPresent = false;

                foreach (StiPage page in report.Pages)
                {
                    if ((page.IsDashboard ? page.IsEnabled : page.Enabled) && !nestedPages.Contains(page))
                    {
                        if (page is IStiDashboard)
                            dashboardsPresent = true;
                        else
                            reportsPresent = true;
                    }
                    if (dashboardsPresent && reportsPresent)
                        return true;
                }
            }

            return false;
        }

        internal static ArrayList GetPagesArray(StiReport report, StiRequestParams requestParams)
        {
            var service = new StiHtmlExportService();
            service.RenderWebViewer = true;
            service.RenderWebInteractions = true;
            service.RenderAsDocument = false;
            service.Styles = new ArrayList();
            service.ClearOnFinish = false;
            service.RenderStyles = false;

            var htmlText = string.Empty;
            var pageMargins = string.Empty;
            var pageSizes = string.Empty;
            var pageBackgrounds = string.Empty;

            #region Render Pages
            var pages = new ArrayList();

            if (requestParams.Viewer.ReportType == StiReportType.Dashboard)
            {
                Hashtable pageAttrs = null;
                var pageNumber = requestParams.Viewer.PageNumber;
                var pageIsEnabled = report.Pages[pageNumber].IsDashboard ? report.Pages[pageNumber].IsEnabled : report.Pages[pageNumber].Enabled;

                if (!pageIsEnabled)
                {
                    var newPageNumber = report.Pages.IndexOf(report.Pages.ToList().FirstOrDefault(p => p.IsDashboard ? p.IsEnabled : p.Enabled));
                    if (newPageNumber >= 0) pageNumber = newPageNumber;
                }

                if (report.Pages[pageNumber].IsDashboard)
                {
                    report.CurrentPage = pageNumber;
                    pageAttrs = GetDashboardPage(report, pageNumber, requestParams);
                    pages.Add(pageAttrs);
                }
                else
                {
                    pageNumber = requestParams.Viewer.PageNumber;
                    

                    if (requestParams.Viewer.ViewMode == StiWebViewMode.SinglePage)
                    {
                        report.CurrentPage = report.Pages.IndexOf(report.Pages.ToList().FirstOrDefault(p => p.Name == report.RenderedPages[pageNumber < report.RenderedPages.Count ? pageNumber : 0].Name));
                        pageAttrs = GetReportPage(report, service, pageNumber, requestParams);
                        pages.Add(pageAttrs);
                    }
                    else
                    {
                        var firstPageNumber = requestParams.Viewer.ViewMode == StiWebViewMode.SinglePage ? requestParams.Viewer.PageNumber : 0;
                        var lastPageNumber = report.RenderedPages.Count - 1;
                        report.CurrentPage = report.Pages.IndexOf(report.Pages.ToList().FirstOrDefault(p => p.Name == report.RenderedPages[firstPageNumber < report.RenderedPages.Count ? firstPageNumber : 0].Name));
                        for (int index = firstPageNumber; index <= lastPageNumber; index++)
                        {
                            pages.Add(GetReportPage(report, service, index, requestParams));
                        }
                    }
                    requestParams.Viewer.ReportType = StiReportType.Report;
                }
            }
            else
            {
                var firstPageNumber = requestParams.Viewer.ViewMode == StiWebViewMode.SinglePage ? requestParams.Viewer.PageNumber : 0;
                var lastPageNumber = Math.Max(0, report.RenderedPages.Count - 1);
                report.CurrentPage = report.RenderedPages.Count > 0 ? report.Pages.IndexOf(report.Pages.ToList().FirstOrDefault(p => p.Name == report.RenderedPages[firstPageNumber < report.RenderedPages.Count ? firstPageNumber : 0].Name)) : 0;

                if (!requestParams.Viewer.CombineReportPages && IsMixedReport(report) && report.Pages.Count > requestParams.Viewer.OriginalPageNumber)
                {
                    var pageName = report.Pages[requestParams.Viewer.OriginalPageNumber].Name;
                    report.CurrentPage = requestParams.Viewer.OriginalPageNumber;
                    var startIndex = report.RenderedPages.ToList().FindIndex(p => p.Name == pageName);
                    var endIndex = report.RenderedPages.ToList().FindLastIndex(p => p.Name == pageName);
                    if (startIndex >= 0 && endIndex >= 0)
                    {
                        firstPageNumber += startIndex;
                        lastPageNumber = endIndex;
                    }
                }

                if (requestParams.Viewer.ViewMode == StiWebViewMode.SinglePage)
                {
                    pages.Add(GetReportPage(report, service, firstPageNumber, requestParams));
                }
                else
                {
                    for (int index = firstPageNumber; index <= lastPageNumber; index++)
                    {
                        pages.Add(GetReportPage(report, service, index, requestParams));
                    }
                }
            }
            #endregion

            #region Render Styles
            var writer = new StringWriter();
            var htmlWriter = new StiHtmlTextWriter(writer);
            service.HtmlWriter = htmlWriter;
            if (service.TableRender != null) service.TableRender.RenderStylesTable(true, false, false);
            htmlWriter.Flush();
            writer.Flush();
            string htmlTextStyles = writer.GetStringBuilder().ToString();
            writer.Close();

            pages.Add(htmlTextStyles);
            #endregion

            //Add Chart Scripts
            string chartScript = service.GetChartScript();
            pages.Add(chartScript);
            service.Clear();

            return pages;
        }
        #endregion

        #region Bookmarks
        private class StiBookmarkTreeNode
        {
            public int Parent;
            public string Title;
            public string Url;
            public string ComponentGuid;
            public bool Used;
        }

        private static void AddBookmarkNode(StiBookmark bkm, int parentNode, ArrayList bookmarksTree)
        {
            var tn = new StiBookmarkTreeNode();
            tn.Parent = parentNode;
            string st = bkm.Text.Replace("'", "\\\'").Replace("\r", "").Replace("\n", "");
            tn.Title = st;
            tn.Url = "#" + st;
            tn.Used = true;
            tn.ComponentGuid = bkm.ComponentGuid;

            bookmarksTree.Add(tn);
            int currentNode = bookmarksTree.Count - 1;
            if (bkm.Bookmarks.Count != 0)
            {
                for (int tempCount = 0; tempCount < bkm.Bookmarks.Count; tempCount++)
                {
                    AddBookmarkNode(bkm.Bookmarks[tempCount], currentNode, bookmarksTree);
                }
            }
        }

        private static Hashtable GetBookmarksPageIndexes(StiReport report)
        {
            var bookmarksPageIndexes = new Hashtable();

            int tempPageNumber = 1;
            foreach (StiPage page in report.RenderedPages)
            {
                report.RenderedPages.GetPage(page);
                StiComponentsCollection components = page.GetComponents();
                foreach (StiComponent comp in components)
                {
                    if (comp.Enabled)
                    {
                        string bookmarkValue = comp.BookmarkValue as string;
                        string componentGuid = comp.Guid as string;
                        if (componentGuid != null && !bookmarksPageIndexes.ContainsKey(componentGuid))
                            bookmarksPageIndexes[componentGuid] = tempPageNumber;
                        if (bookmarkValue == null) bookmarkValue = string.Empty;
                        bookmarkValue = bookmarkValue.Replace("'", "\\\'");
                        if (bookmarkValue != string.Empty && !bookmarksPageIndexes.ContainsKey(bookmarkValue))
                            bookmarksPageIndexes.Add(bookmarkValue, tempPageNumber);
                    }
                }
                tempPageNumber++;
            }

            return bookmarksPageIndexes;
        }

        public static string GetBookmarksContent(StiReport report, string viewerId, int pageNumber)
        {
            var bookmarksPageIndexes = GetBookmarksPageIndexes(report);
            var bookmarksTree = new ArrayList();
            AddBookmarkNode(report.Bookmark, -1, bookmarksTree);

            var html = string.Empty;
            html += string.Format("bookmarks = new stiTree('bookmarks','{0}',{1}, imagesForBookmarks);", viewerId, pageNumber);
            for (int index = 0; index < bookmarksTree.Count; index++)
            {
                StiBookmarkTreeNode node = (StiBookmarkTreeNode)bookmarksTree[index];
                string pageTitle = string.Empty;
                
                if (node.ComponentGuid != null && bookmarksPageIndexes.ContainsKey(node.ComponentGuid)) 
                    pageTitle = string.Format("Page {0}", (int)bookmarksPageIndexes[node.ComponentGuid]);
                else if (bookmarksPageIndexes.ContainsKey(node.Title)) 
                    pageTitle = string.Format("Page {0}", (int)bookmarksPageIndexes[node.Title]);
                else 
                    pageTitle = "Page 0";
                
                html += string.Format("bookmarks.add({0},{1},'{2}','{3}','{4}','{5}');", index, node.Parent, SecurityElement.Escape(node.Title), node.Url, pageTitle, node.ComponentGuid);
            }

            return html;
        }

        public static ArrayList GetBookmarkPointers(StiReport report, StiBookmark bookmark)
        {
            var bookmarksPageIndexes = GetBookmarksPageIndexes(report);
            var pointers = new ArrayList();
            var bookmarksTree = new ArrayList();
            AddBookmarkNode(bookmark, -1, bookmarksTree);
                        
            for (int i = 0; i < bookmarksTree.Count; i++)
            {
                var bookmarkParams = new Hashtable();
                var node = bookmarksTree[i] as StiBookmarkTreeNode;
                int pageIndex = 1;

                if (node.ComponentGuid != null && bookmarksPageIndexes.ContainsKey(node.ComponentGuid)) 
                    pageIndex = (int)bookmarksPageIndexes[node.ComponentGuid];
                else if (bookmarksPageIndexes.ContainsKey(node.Title))
                    pageIndex = (int)bookmarksPageIndexes[node.Title];

                bookmarkParams["componentGuid"] = node.ComponentGuid;
                bookmarkParams["anchor"] = node.Url;
                bookmarkParams["pageIndex"] = pageIndex;                
                pointers.Add(bookmarkParams);
            }

            return pointers;
        }
        #endregion

        #region Result

        #region Viewer
        public static StiWebActionResult ViewerResult(StiRequestParams requestParams, StiReport report)
        {
            if (report == null)
                return StiWebActionResult.EmptyReportResult();
            
            if (!report.IsRendered && !report.ContainsOnlyDashboard)
            {
                try
                {
                    report.Render(false);
                }
                catch (Exception e)
                {
                    var message = e.Message;
                    if (e.InnerException != null && !string.IsNullOrEmpty(e.InnerException.Message))
                        message += " " + e.InnerException.Message;

                    return new StiWebActionResult("ServerError:" + message);
                }
            }

            if (requestParams.Action == StiAction.ChangeTableElementSelectColumns)
                return StiWebActionResult.JsonResult(requestParams, StiTableElementViewHelper.ChangeTableElementSelectColumns(report, requestParams));

            if (requestParams.Action == StiAction.ChangeChartElementViewState)
                return StiWebActionResult.JsonResult(requestParams, StiChartElementViewHelper.ChangeChartElementViewState(report, requestParams));

            if (requestParams.Action == StiAction.DashboardGetSingleElementContent)
                return StiWebActionResult.JsonResult(requestParams, GetSingleElementContent(report, requestParams));

            if (requestParams.Action == StiAction.DashboardGettingFilterItems)
                return StiWebActionResult.JsonResult(requestParams, StiDataFiltersHelper.GetFilterItems(report, requestParams));

            if (requestParams.Action == StiAction.GetDatePickerFormattedValues)
                return StiWebActionResult.JsonResult(requestParams, StiDatePickerElementViewHelper.GetFormattedValues(report, requestParams));

            if (requestParams.Action == StiAction.DashboardViewData)
                return StiWebActionResult.JsonResult(requestParams, StiDataFiltersHelper.GetViewData(report, requestParams));

            if (requestParams.Action == StiAction.GetSignatureData)
                return StiWebActionResult.JsonResult(requestParams, StiElectronicSignatureHelper.GetSignatureData(report, requestParams));

            StiEditableFieldsHelper.ApplyEditableFieldsToReport(report, requestParams.Interaction.Editable);

            // Correct current page number
            if (requestParams.Action == StiAction.DrillDown) requestParams.Viewer.PageNumber = 0;
            if ((requestParams.Action == StiAction.Variables || requestParams.Action == StiAction.Collapsing) && requestParams.Viewer.ReportType == StiReportType.Report)
                requestParams.Viewer.PageNumber = Math.Min(requestParams.Viewer.PageNumber, report.RenderedPages.Count - 1);

            // Correct report type
            if (requestParams.Viewer.ReportType == StiReportType.Auto)
            {
                var nestedPages = GetNestedPages(report);
                var firstPage = report.Pages.ToList().FirstOrDefault(p => (p.IsDashboard ? p.IsEnabled : p.Enabled) && !nestedPages.Contains(p));
                if (requestParams.Viewer.ReportDesignerMode && requestParams.Viewer.OriginalPageNumber > 0 && requestParams.Viewer.OriginalPageNumber < report.Pages.Count) 
                {
                    var page = report.Pages[requestParams.Viewer.OriginalPageNumber];
                    if ((page.IsDashboard ? page.IsEnabled : page.Enabled) && !nestedPages.Contains(page)) 
                        firstPage = page;
                }
                requestParams.Viewer.ReportType = firstPage != null && firstPage.IsDashboard ? StiReportType.Dashboard : StiReportType.Report;
                requestParams.Viewer.OriginalPageNumber = report.Pages.IndexOf(firstPage);
                if (requestParams.Viewer.ReportType == StiReportType.Dashboard) requestParams.Viewer.PageNumber = requestParams.Viewer.OriginalPageNumber;
            }

            // Create parameters table for viewer
            var parameters = new Hashtable();
            parameters["action"] = requestParams.Action;
            parameters["refreshTime"] = report.IsDocument ? 0 : report.RefreshTime;
            parameters["parameterWidth"] = report.ParameterWidth;

            if (requestParams.Action == StiAction.GetReport || requestParams.Action == StiAction.OpenReport)
            {
                parameters["customFonts"] = StiReportResourceHelper.GetFontResourcesArray(report);
                parameters["stimulsoftFontContent"] = !requestParams.Viewer.ReportDesignerMode && StiFontIconsExHelper.NeedToUseStimulsoftFont(report)
                    ? StiReportResourceHelper.GetStimulsoftFontBase64Data()
                    : null;

                if (report.ContainsDashboard)
                {
                    StiDataFiltersHelper.ApplyDefaultFiltersForFilterElements(report);
                    parameters["dashboards"] = GetDashboards(report, requestParams);
                }
            }

            if (requestParams.Viewer.ReportType == StiReportType.Report || requestParams.Action != StiAction.GetReport && requestParams.Action != StiAction.OpenReport && requestParams.Action != StiAction.InitVars)
                parameters["pagesArray"] = GetPagesArray(report, requestParams);

            if (requestParams.Action == StiAction.DashboardFiltering ||
                requestParams.Action == StiAction.DashboardSorting ||
                requestParams.Action == StiAction.DashboardElementDrillDown ||
                requestParams.Action == StiAction.DashboardElementDrillUp)
            {                
                parameters["repaintOnlyDashboardContent"] = true;

                Hashtable interactionParameters = null;

                if (requestParams.Action == StiAction.DashboardFiltering || requestParams.Action == StiAction.DashboardButtonElementApplyEvents)
                {
                    interactionParameters = requestParams.Interaction.DashboardFiltering;
                }
                else if (requestParams.Action == StiAction.DashboardSorting)
                {
                    interactionParameters = requestParams.Interaction.DashboardSorting;
                }
                else if (requestParams.Action == StiAction.DashboardElementDrillDown || requestParams.Action == StiAction.DashboardElementDrillUp)
                {
                    interactionParameters = requestParams.Interaction.DashboardElementDrillDownParameters;
                }

                if (interactionParameters != null)
                {
                    if (interactionParameters["elementName"] != null) parameters["currentElementName"] = interactionParameters["elementName"];
                    if (interactionParameters["elementGroup"] != null) parameters["currentElementGroup"] = interactionParameters["elementGroup"];
                    if (interactionParameters["filterGuid"] != null) parameters["filterGuid"] = interactionParameters["filterGuid"];
                }
            }

            if (requestParams.Action != StiAction.GetPages)
            {
                parameters["reportType"] = requestParams.Viewer.ReportType.ToString();
                parameters["drillDownGuid"] = requestParams.Cache.DrillDownGuid;
                parameters["dashboardDrillDownGuid"] = requestParams.Cache.DashboardDrillDownGuid;
                parameters["zoom"] = Convert.ToInt32(requestParams.Viewer.Zoom * 100);
                parameters["viewMode"] = requestParams.Viewer.ViewMode;
                parameters["isEditableReport"] = StiEditableFieldsHelper.CheckEditableReport(report);
                parameters["isSignedReport"] = StiElectronicSignatureHelper.CheckSignedReport(report);
                parameters["isCompilationMode"] = report.CalculationMode == StiCalculationMode.Compilation;
                parameters["pagesCount"] = GetPagesCount(report, requestParams);
                parameters["reportFileName"] = GetReportFileName(report);
                parameters["collapsingStates"] = report.InteractionCollapsingStates;
                parameters["resources"] = StiReportResourceHelper.GetResourcesItems(report);
                parameters["previewSettings"] = GetReportPreviewSettings(report);
                parameters["variablesValues"] = StiVariablesHelper.GetVariablesValues(report);
                parameters["tableOfContentsPointers"] = GetTableOfContentsPointers(report, requestParams);
                parameters["parametersOrientation"] = report.ParametersOrientation.ToString();

                if (requestParams.Action == StiAction.DashboardButtonElementApplyEvents) 
                    parameters["dashboards"] = GetDashboards(report, requestParams);

                if (requestParams.Viewer.ReportDisplayMode == StiReportDisplayMode.FromReport)
                    parameters["reportDisplayMode"] = GetReportDisplayModeFromReport(report);

                if (!string.IsNullOrEmpty(report.ParametersDateFormat))
                    parameters["parametersDateFormat"] = report.ParametersDateFormat;

                if (requestParams.Action == StiAction.DrillDown || requestParams.Action == StiAction.DashboardDrillDown)
                {
                    parameters["drillDownParameters"] = requestParams.Interaction.DrillDown;
                    parameters["variablesPresentsInReport"] = report != null && report.Dictionary.Variables.ToList().Any(v => v.RequestFromUser);
                }

                if (report.Bookmark != null && report.Bookmark.Bookmarks.Count > 0 && requestParams.Viewer.ShowBookmarks)
                    parameters["bookmarksContent"] = GetBookmarksContent(report, requestParams.Id, requestParams.Viewer.ViewMode == StiWebViewMode.SinglePage ? requestParams.Viewer.PageNumber : -1);
            }

            if (requestParams.UserValues != null)
                parameters["userValues"] = requestParams.UserValues;

#if CLOUD
            parameters["cloudPlanIdent"] = requestParams.CloudPlanIdent;
            CheckCloudLimits(parameters, report);
#endif

            return StiWebActionResult.JsonResult(requestParams, parameters);
        }

        public static void CheckCloudLimits(Hashtable parameters, StiReport report)
        {
            var plan = StiCloudPlan.GetPlan(report.ReportGuid);
            StiCloudReportLimits reportLimits = StiCloudReportResults.GetAndRemoveLimits(report.ReportGuid);
            parameters["maxDataRows"] = reportLimits.MaxDataRows;
            parameters["maxReportPages"] = reportLimits.MaxReportPages;

            if (report.Dictionary.Resources.Count > plan.MaxResources)
                parameters["maxResources"] = plan.MaxResources;

            if (report.Dictionary.Resources.ToList().Any(r => r.Content?.Length > plan.MaxResourceSize))
                parameters["maxResourceSize"] = plan.MaxResourceSize;

            if (!plan.AllowDatabases)
            {
                var database = report.Dictionary.Databases.ToList().FirstOrDefault(d => !(d is StiFileDatabase));
                if (database != null) parameters["notAllowDatabase"] = database.ServiceName;
            }

            if (!plan.AllowDataTransformation && report.Dictionary.DataSources.ToList().Any(d => d is StiDataTransformation))
                parameters["notAllowDataTransformation"] = true;
        }

        public static ArrayList GetTableOfContentsPointers(StiReport report, StiRequestParams requestParams)
        {
            var pointers = new ArrayList();
            var tableOfContents = report?.GetComponents().ToList().FirstOrDefault(comp => comp is StiTableOfContents) as StiTableOfContents;
            if (tableOfContents != null) 
            {
                var pointer = report.Pointer;

                if (string.IsNullOrWhiteSpace(tableOfContents.ReportPointer))
                {
                    pointers = GetBookmarkPointers(report, pointer);
                }
                else
                {
                    pointer.Text = StiReportParser.Parse(tableOfContents.ReportPointer, tableOfContents);
                    pointer.ParentComponent = report;
                    pointers = GetBookmarkPointers(report, pointer);
                }
            }
            return pointers;
        }
        #endregion

        #region Interaction
        public static StiWebActionResult InteractionResult(StiRequestParams requestParams, StiReport report)
        {
            if (requestParams.Action == StiAction.InitVars)
            {
                if (report == null || report.IsDocument)
                    return new StiWebActionResult("null", "application/json");

                report = GetCompiledReport(report);
                
                var isBindingVariable = requestParams.GetBoolean("isBindingVariable");

                if (!isBindingVariable && report.ContainsDashboard && report.Variables == null && (report.CalculationMode == StiCalculationMode.Interpretation || StiOptions.Engine.ForceInterpretationMode))
                    StiParser.PrepareReportVariables(report);

                StiVariablesHelper.FillDialogInfoItems(report);
                StiVariablesHelper.ApplyReportBindingVariables(report, requestParams.Interaction.Variables);

                if (!isBindingVariable)
                    StiVariableHelper.SetDefaultValueForRequestFromUserVariables(report, true, true);

                var variables = StiVariablesHelper.GetVariables(report, requestParams.Interaction.Variables, requestParams.Viewer.ParametersPanelSortDataItems);
                var json = JsonConvert.SerializeObject(variables, Stimulsoft.Base.Json.Formatting.None, new StringEnumConverter());
                return new StiWebActionResult(json, "application/json");
            }

            // If report not assigned, return 'The report is not specified' error message.
            if (report == null)
                return StiWebActionResult.EmptyReportResult();

            // If report is renderen document, return current report.
            if (report.IsDocument) return ViewerResult(requestParams, report);
            
            report = GetCompiledReport(report);

            // If report stored as StiReport object and process drill-down, we need to create report copy.
            if ((requestParams.Cache.Mode == StiServerCacheMode.ObjectCache || requestParams.Cache.Mode == StiServerCacheMode.ObjectSession) &&
                 requestParams.Interaction.DrillDown != null && requestParams.Interaction.DrillDown.Count > 0)
            {
                report = StiReportCopier.CloneReport(report);
            }

            // Apply URL query parameters
            ApplyQueryParameters(requestParams, report);

            // Force build when updating a report
            if (requestParams.Action == StiAction.RefreshReport)
            {
                report.IsRendered = false;
                report.InvokeRefreshing();
                StiCacheCleaner.Clean(report.Key);
            }

            // Apply report variables
            if (requestParams.Action == StiAction.Variables ||
                requestParams.Action == StiAction.Sorting ||
                requestParams.Action == StiAction.Collapsing ||
                requestParams.Action == StiAction.DrillDown)
            {
                StiVariablesHelper.ApplyReportParameters(report, requestParams.Interaction.Variables);
                if (requestParams.Action == StiAction.Variables) StiCacheCleaner.Clean(report.Key);
            }

            // Apply drill-down parameters
            if (requestParams.Interaction.DrillDown != null && requestParams.Interaction.DrillDown.Count > 0)
            {
                var renderedReport = requestParams.Cache.Helper.GetReportInternal(requestParams, false);
                if (renderedReport == null) renderedReport = report;
                
                StiDashboardElementViewHelper.ParseDashboardDrillDownParameters(requestParams.Interaction.DrillDown, report);
                var drillDownParameters = new Hashtable();
                
                foreach (Hashtable parameters in requestParams.Interaction.DrillDown)
                {
                    if (renderedReport.Dictionary.Variables.Count > 0 && requestParams.Interaction.Variables?.Count > 0)
                    {
                        StiVariablesHelper.TransferParametersValuesToReport(renderedReport, requestParams.Interaction.Variables);
                    }

                    renderedReport = parameters.Contains("isDashboardDrillDown")
                        ? ApplyDashboardDrillDown(renderedReport, parameters)
                        : ApplyDrillDown(report, renderedReport, parameters, drillDownParameters);

                    if (!string.IsNullOrEmpty(parameters["ReportFile"] as string))
                    {
                        report = renderedReport;
                    }
                }

                var reportIsChanged = report != renderedReport;
                report = renderedReport;

                if (reportIsChanged && report != null && requestParams.Action == StiAction.Variables && report.Dictionary.Variables.Count > 0 && requestParams.Interaction.Variables.Count > 0)
                {
                    StiVariablesHelper.ApplyReportParameters(report, requestParams.Interaction.Variables);

                    foreach (DictionaryEntry drillDownParameter in drillDownParameters)
                        report[drillDownParameter.Key?.ToString()] = drillDownParameter.Value;
                }
            }

            // Apply report interactions
            if (requestParams.Action == StiAction.Collapsing || requestParams.Action == StiAction.Sorting)
            {
                if (requestParams.Interaction.Collapsing?.Count > 0)
                    ApplyCollapsing(report, requestParams.Interaction.Collapsing);

                if (requestParams.Interaction.Sorting?.Count > 0)
                    ApplySorting(report, requestParams.Interaction.Sorting);
            }

            // Apply report signatures
            if (requestParams.Action == StiAction.Signatures)
            {
                if (requestParams.Interaction.Signatures?.Count > 0)
                    ApplySignatures(report, requestParams.Interaction.Signatures);
            }

            // Apply dashboard element drill-down parameters
            if (requestParams.Action == StiAction.DashboardElementDrillDown && requestParams.Interaction.DashboardElementDrillDownParameters != null)
            {
                StiDashboardElementDrillDownHelper.ApplyDashboardElementDrillDown(report, requestParams.Interaction.DashboardElementDrillDownParameters);
            }

            // Apply dashboard element drill-up parameters
            if (requestParams.Action == StiAction.DashboardElementDrillUp && requestParams.Interaction.DashboardElementDrillDownParameters != null)
            {
                StiDashboardElementDrillDownHelper.ApplyDashboardElementDrillUp(report, requestParams.Interaction.DashboardElementDrillDownParameters);
            }

            // Apply dashboard filtering
            if (requestParams.Action == StiAction.DashboardFiltering)
                StiDataFiltersHelper.ApplyFiltering(report, requestParams.Interaction.DashboardFiltering);

            // Apply dashboard sorting
            if (requestParams.Action == StiAction.DashboardSorting)
                StiDataSortsHelper.ApplySorting(report, requestParams.Interaction.DashboardSorting);

            // Apply button element click event
            if (requestParams.Action == StiAction.DashboardButtonElementApplyEvents)
                StiButtonElementHelper.ApplyButtonEvent(report, requestParams.Interaction.DashboardFiltering);

            // Reset All Filters
            if (requestParams.Action == StiAction.DashboardResetAllFilters)
                StiDataFiltersHelper.ResetAllFilters(report, requestParams);

            if (!report.IsRendered && !report.ContainsOnlyDashboard)
            {
                report.IsReportRenderingAfterSubmit = true;

                try
                {                    
                    report.Render(false);                    
                }
                catch (Exception e)
                {
                    var message = e.Message;
                    if (e.InnerException != null && !string.IsNullOrEmpty(e.InnerException.Message))
                        message += " " + e.InnerException.Message;

                    return new StiWebActionResult("ServerError:" + message);
                }
                finally
                {
                    report.IsReportRenderingAfterSubmit = false;
                }
            }

            requestParams.Cache.Helper.SaveReportInternal(requestParams, report);

            return ViewerResult(requestParams, report);
        }
        #endregion

        #region Get
        public static StiWebActionResult GetReportResult(StiRequestParams requestParams, StiReport report)
        {
            try
            {
                requestParams.Cache.Helper.RemoveReportInternal(requestParams);

                if (report != null)
                {
                    report.Key = StiKeyHelper.GenerateKey();
                    requestParams.Cache.Helper.SaveReportInternal(requestParams, report);

                    ApplyQueryParameters(requestParams, report);
                }

                return ViewerResult(requestParams, report);
            }
            catch (StiDashboardNotSupportedException)
            {
                return StiWebActionResult.DashboardNotSupportedResult(requestParams);
            }
            catch (Exception e)
            {
                return StiWebActionResult.ErrorResult(requestParams, StiWebActionResult.GetErrorMessageText(e));
            }
        }

        public static async Task<StiWebActionResult> GetReportResultAsync(StiRequestParams requestParams, StiReport report)
        {
            return await Task.Run(() => GetReportResult(requestParams, report));
        }
        #endregion

        #region Open
        public static StiReport LoadReportFromContent(StiRequestParams requestParams)
        {
            var report = new StiReport();
            var fileName = requestParams.Viewer.OpeningFileName;
            var password = requestParams.Viewer.OpeningFilePassword;
            var isTemplate = !string.IsNullOrEmpty(fileName) && (fileName.ToLower().EndsWith(".mrt") || fileName.ToLower().EndsWith(".mrx") || fileName.ToLower().EndsWith(".mrz"));

            if (requestParams.Data != null)
            {
                using (var stream = new MemoryStream())
                {
                    stream.Write(requestParams.Data, 0, requestParams.Data.Length);
                    stream.Position = 0;

                    if (isTemplate)
                    {
                        if (string.IsNullOrEmpty(password))
                            report.Load(stream);
                        else
                            report.LoadEncryptedReport(stream, password);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(password))
                            report.LoadDocument(stream);
                        else
                            report.LoadEncryptedDocument(stream, password);
                    }
                }
            }

            return report;
        }

        public static StiWebActionResult OpenReportResult(StiRequestParams requestParams)
        {
            try
            {
                var report = LoadReportFromContent(requestParams);
                return GetReportResult(requestParams, report);
            }
            catch (StiDashboardNotSupportedException)
            {
                return StiWebActionResult.DashboardNotSupportedResult(requestParams);
            }
            catch (Exception e)
            {
                return StiWebActionResult.ErrorResult(requestParams, StiWebActionResult.GetErrorMessageText(e));
            }
        }
        #endregion

        #region ReportResource
        public static StiWebActionResult ReportResourceResult(StiRequestParams requestParams, StiReport report)
        {
            var resourceName = requestParams.ReportResourceParams["resourceName"] as string;
            if (report != null)
            {
                var resource = report.Dictionary.Resources[resourceName];
                if (resource != null && resource.Content != null)
                {
                    var stream = new MemoryStream(resource.Content);
                    var viewType = requestParams.ReportResourceParams["viewType"] as string;
                    var fileName = viewType == "SaveFile" ? resource.Name + resource.GetFileExt() : null;
                    return new StiWebActionResult(stream, resource.GetContentType(), fileName);
                }
            }

            return null;
        }

        public static async Task<StiWebActionResult> ReportResourceResultAsync(StiRequestParams requestParams, StiReport report)
        {
            return await Task.Run(() => ReportResourceResult(requestParams, report));
        }
        #endregion

        #region ReportResource
        public static Hashtable GetReportPreviewSettings(StiReport report)
        {
            Hashtable settingsJsObject = new Hashtable();

            var repSettings = (StiPreviewSettings)report.PreviewSettings;
            settingsJsObject["reportPrint"] = (repSettings & StiPreviewSettings.Print) > 0;
            settingsJsObject["reportOpen"] = (repSettings & StiPreviewSettings.Open) > 0;
            settingsJsObject["reportSave"] = (repSettings & StiPreviewSettings.Save) > 0;
            settingsJsObject["reportSendEMail"] = (repSettings & StiPreviewSettings.SendEMail) > 0;
            settingsJsObject["reportPageControl"] = (repSettings & StiPreviewSettings.PageControl) > 0;
            settingsJsObject["reportEditor"] = (repSettings & StiPreviewSettings.Editor) > 0;
            settingsJsObject["reportFind"] = (repSettings & StiPreviewSettings.Find) > 0;
            settingsJsObject["reportSignature"] = (repSettings & StiPreviewSettings.Signature) > 0;
            settingsJsObject["reportPageViewMode"] = (repSettings & StiPreviewSettings.PageViewMode) > 0;
            settingsJsObject["reportZoom"] = (repSettings & StiPreviewSettings.Zoom) > 0;
            settingsJsObject["reportBookmarks"] = (repSettings & StiPreviewSettings.Bookmarks) > 0;
            settingsJsObject["reportParameters"] = (repSettings & StiPreviewSettings.Parameters) > 0;
            settingsJsObject["reportResources"] = (repSettings & StiPreviewSettings.Resources) > 0;
            settingsJsObject["reportStatusBar"] = (repSettings & StiPreviewSettings.StatusBar) > 0;
            settingsJsObject["reportToolbar"] = (repSettings & StiPreviewSettings.Toolbar) > 0;

            var dbsSettings = report.DashboardViewerSettings;
            settingsJsObject["dashboardToolBar"] = (dbsSettings & StiDashboardViewerSettings.ShowToolBar) > 0;
            settingsJsObject["dashboardRefreshButton"] = (dbsSettings & StiDashboardViewerSettings.ShowRefreshButton) > 0;
            settingsJsObject["dashboardOpenButton"] = (dbsSettings & StiDashboardViewerSettings.ShowOpenButton) > 0;
            settingsJsObject["dashboardEditButton"] = (dbsSettings & StiDashboardViewerSettings.ShowEditButton) > 0;
            settingsJsObject["dashboardFullScreenButton"] = (dbsSettings & StiDashboardViewerSettings.ShowFullScreenButton) > 0;            
            settingsJsObject["dashboardMenuButton"] = (dbsSettings & StiDashboardViewerSettings.ShowMenuButton) > 0;
            settingsJsObject["dashboardResetAllFiltersButton"] = (dbsSettings & StiDashboardViewerSettings.ShowResetAllFilters) > 0;
            settingsJsObject["dashboardParametersButton"] = (dbsSettings & StiDashboardViewerSettings.ShowParametersButton) > 0;
            settingsJsObject["dashboardShowReportSnapshots"] = (dbsSettings & StiDashboardViewerSettings.ShowReportSnapshots) > 0;
            settingsJsObject["dashboardShowExports"] = (dbsSettings & StiDashboardViewerSettings.ShowExports) > 0;

            settingsJsObject["reportToolbarHorAlignment"] = report.PreviewToolBarOptions.ReportToolbarHorAlignment;
            settingsJsObject["reportToolbarReverse"] = report.PreviewToolBarOptions.ReportToolbarReverse;
            settingsJsObject["dashboardToolbarHorAlignment"] = report.PreviewToolBarOptions.DashboardToolbarHorAlignment;
            settingsJsObject["dashboardToolbarReverse"] = report.PreviewToolBarOptions.DashboardToolbarReverse;

            return settingsJsObject;
        }
        #endregion

        #endregion
    }
}
