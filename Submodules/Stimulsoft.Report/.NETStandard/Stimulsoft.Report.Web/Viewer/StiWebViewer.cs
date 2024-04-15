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

using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Stimulsoft.Report.Export;
using System.Collections;
using Stimulsoft.Report.Web.Helpers.Dashboards;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Converters;
using Stimulsoft.Base;

namespace Stimulsoft.Report.Web
{
    [ToolboxBitmap(typeof(StiWebViewer), "Viewer.Images.StiWebViewer.bmp")]
    public partial class StiWebViewer :
        WebControl,
        INamingContainer
    {
        internal void ProcessRequest(StiRequestParams requestParams)
        {
            if (requestParams.Action != StiAction.Undefined &&
                requestParams.ComponentType == StiComponentType.Viewer &&
                (requestParams.Id == ID || requestParams.Action == StiAction.Resource))
            {
                this.requestParams = requestParams;
                this.ClientGuid = requestParams.Cache.ClientGuid;
                InvokeViewerEvent();

                StiExportSettings settings = null;
                StiWebActionResult result = null;

                switch (requestParams.Action)
                {
                    case StiAction.Resource:
                        result = StiViewerResourcesHelper.Get(requestParams);
                        break;

                    case StiAction.GetReport:
                        try
                        {
                            InvokeGetReport();
                            if (Report != null) Report.Key = StiKeyHelper.GenerateKey();
                            result = StiReportHelper.ViewerResult(requestParams, Report);
                        }
                        catch (StiDashboardNotSupportedException e)
                        {
                            Console.Write(StiWebActionResult.GetErrorMessageText(e));
                            result = StiWebActionResult.DashboardNotSupportedResult(requestParams);
                        }
                        catch (Exception e)
                        {
                            result = StiWebActionResult.ErrorResult(requestParams, StiWebActionResult.GetErrorMessageText(e));
                        }
                        break;

                    case StiAction.GetPages:
                        if (Report == null) InvokeGetReport();
                        result = StiReportHelper.ViewerResult(requestParams, Report);
                        break;

                    case StiAction.OpenReport:
                        if (Report != null) StiCacheCleaner.Clean(Report);
                        result = StiReportHelper.OpenReportResult(requestParams);
                        break;

                    case StiAction.RefreshReport:
                        if (Report == null) InvokeGetReport();
                        if (requestParams.Viewer.ReportType == StiReportType.Dashboard) StiCacheCleaner.Clean(Report);
                        InvokeGetReportData();
                        result = StiReportHelper.InteractionResult(requestParams, Report);
                        break;

                    case StiAction.PrintReport:
                        settings = GetExportSettings(requestParams);
                        InvokePrintReport(settings);
                        result = StiExportsHelper.PrintReportResult(requestParams, Report, settings);
                        break;

                    case StiAction.ExportDashboard:
                        var dashboardExportSettings = GetDashboardExportSettings(requestParams);
                        InvokeExportReport(null);
                        var report = (!string.IsNullOrEmpty(requestParams.Cache.DashboardDrillDownGuid)) ? GetReportObject(requestParams) : Report;
                        result = StiExportsHelper.ExportDashboardResult(requestParams, report, dashboardExportSettings);
                        InvokeExportReportResponse(null, result);
                        break;

                    case StiAction.ExportReport:
                        settings = GetExportSettings(requestParams);
                        InvokeExportReport(settings);
                        result = StiExportsHelper.ExportReportResult(requestParams, Report, settings);
                        InvokeExportReportResponse(settings, result);
                        break;

                    case StiAction.EmailReport:
                        settings = GetExportSettings(requestParams);
                        StiEmailOptions options = GetEmailOptions(requestParams);
                        InvokeEmailReport(settings, options);
                        result = StiExportsHelper.EmailReportResult(requestParams, Report, settings, options);
                        break;

                    case StiAction.ReportResource:
                        result = StiReportHelper.ReportResourceResult(requestParams, Report);
                        break;

                    case StiAction.InitVars:
                        var currentReport = Report;
                        if (currentReport != null && !currentReport.IsDocument && currentReport.DataStore.Count == 0 && currentReport.Dictionary.Variables.Count > 0) InvokeGetReportData();
                        result = StiReportHelper.InteractionResult(requestParams, Report);
                        break;

                    case StiAction.Variables:
                    case StiAction.Sorting:
                    case StiAction.DrillDown:
                    case StiAction.Collapsing:
                    case StiAction.Signatures:
                    case StiAction.DashboardFiltering:
                    case StiAction.DashboardResetAllFilters:
                    case StiAction.DashboardSorting:
                    case StiAction.DashboardDrillDown:
                    case StiAction.DashboardElementDrillDown:
                    case StiAction.DashboardElementDrillUp:
                    case StiAction.DashboardButtonElementApplyEvents:
                        InvokeInteraction();
                        result = StiReportHelper.InteractionResult(requestParams, Report);
                        break;

                    case StiAction.ChangeTableElementSelectColumns:
                        result = StiWebActionResult.JsonResult(requestParams, StiTableElementViewHelper.ChangeTableElementSelectColumns(Report, requestParams));
                        break;

                    case StiAction.ChangeChartElementViewState:
                        result = StiWebActionResult.JsonResult(requestParams, StiChartElementViewHelper.ChangeChartElementViewState(Report, requestParams));
                        break;

                    case StiAction.DashboardGettingFilterItems:
                        result = StiWebActionResult.JsonResult(requestParams, StiDataFiltersHelper.GetFilterItems(Report, requestParams));
                        break;

                    case StiAction.DashboardViewData:
                        result = StiWebActionResult.JsonResult(requestParams, StiDataFiltersHelper.GetViewData(Report, requestParams));
                        break;

                    case StiAction.GetDatePickerFormattedValues:
                        result = StiWebActionResult.JsonResult(requestParams, StiDatePickerElementViewHelper.GetFormattedValues(Report, requestParams));
                        break;

                    case StiAction.DashboardGetSingleElementContent:
                        result = StiWebActionResult.JsonResult(requestParams, StiReportHelper.GetSingleElementContent(Report, requestParams));
                        break;

                    case StiAction.GetSignatureData:
                        result = StiWebActionResult.JsonResult(requestParams, StiElectronicSignatureHelper.GetSignatureData(Report, requestParams));
                        break;

                    case StiAction.DesignReport:
                        InvokeDesignReport();
                        break;

                    case StiAction.UpdateCache:
                        requestParams.Cache.Helper.UpdateObjectCacheInternal(requestParams, null);
                        break;
                }

                if (result != null)
                {
                    var useBrowserCache = requestParams.Action == StiAction.Resource;
                    StiReportResponse.ResponseBuffer(result.Data, result.ContentType, useBrowserCache, result.FileName, result.ShowSaveFileDialog);
                }
            }
        }
        
        protected override void OnInit(EventArgs e)
        {
            var requestParams = this.RequestParams;
            this.ProcessRequest(requestParams);
            
            base.OnInit(e);
        }

        #region Internal

        /// <summary>
        /// Get the URL for viewer requests
        /// </summary>
        internal static string GetRequestUrl(bool useRelativeUrls, bool passQueryParameters, int portNumber)
        {
            if (HttpContext.Current == null) return null;

            var httpRequest = HttpContext.Current.Request;
            var url = httpRequest.Url;
            var builder = new UriBuilder(url);

            if (httpRequest.Headers != null && httpRequest.Headers.Count > 0)
            {
                if (!string.IsNullOrEmpty(httpRequest.Headers["Host"]))
                {
                    var values = httpRequest.Headers["Host"].Split(':');
                    builder.Host = !string.IsNullOrEmpty(values[0]) ? values[0] : builder.Host;
                    if (values.Length > 1)
                    {
                        int port;
                        if (int.TryParse(values[1], out port))
                            builder.Port = port;
                    }
                }

                var loadbalancerReceivedSSLRequest = string.Equals(httpRequest.Headers["X-Forwarded-Proto"], "https");
                var serverReceivedSSLRequest = httpRequest.IsSecureConnection;
                if (loadbalancerReceivedSSLRequest || serverReceivedSSLRequest)
                    builder.Scheme = "https";

                int loadbalancerReceivedPort;
                if (int.TryParse(httpRequest.Headers["X-Forwarded-Port"], out loadbalancerReceivedPort) && loadbalancerReceivedPort > 0)
                    builder.Port = loadbalancerReceivedPort;
            }

            if (portNumber != 0)
                builder.Port = Math.Max(-1, portNumber);

            url = builder.Uri;

            if (useRelativeUrls)
                return HttpContext.Current.Response.ApplyAppPathModifier(passQueryParameters ? url.PathAndQuery : url.AbsolutePath);

            return url.AbsoluteUri;
        }
        
        /// <summary>
        /// Create default viewer RequestParams to save the report into cache
        /// </summary>
        private StiRequestParams CreateRequestParams()
        {
            StiRequestParams requestParams = new StiRequestParams();
            requestParams.Action = StiAction.GetReport;
            requestParams.ComponentType = StiComponentType.Viewer;
            requestParams.Id = this.ID;
            requestParams.CloudMode = this.CloudMode;
            requestParams.ServerMode = this.ServerMode;
            requestParams.Cache.Mode = this.CacheMode;
            requestParams.Cache.Timeout = new TimeSpan(0, this.CacheTimeout, 0);
            requestParams.Cache.Priority = this.CacheItemPriority;
            requestParams.Cache.ClientGuid = this.ClientGuid;
            requestParams.Cache.Helper = CacheHelper;
            requestParams.Viewer.PageNumber = 0;
            requestParams.Viewer.Zoom = 1;
            requestParams.Viewer.ViewMode = this.ViewMode;
            requestParams.Viewer.OpenLinksWindow = this.OpenLinksWindow;
            requestParams.Viewer.ChartRenderType = this.ChartRenderType;
            requestParams.Viewer.BookmarksPrint = this.BookmarksPrint;
            requestParams.Viewer.CombineReportPages = this.CombineReportPages;
            requestParams.Server.UseRelativeUrls = this.UseRelativeUrls;
            requestParams.Server.UseCompression = this.UseCompression;
            requestParams.Server.PassQueryParametersForResources = this.PassQueryParametersForResources;
            requestParams.Server.PassQueryParametersToReport = this.PassQueryParametersToReport;

            return requestParams;
        }

        private static void WriteToFile(string file, string text)
        {
            var path = Path.GetDirectoryName(file);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            var stream = File.Create(file);
            var buffer = Encoding.UTF8.GetBytes(text);
            stream.Write(buffer, 0, buffer.Length);
            stream.Flush();
            stream.Close();
        }

        /// <summary>
        /// Get the URL to load the images of the web component.
        /// </summary>
        internal static string GetImageUrl(StiRequestParams requestParams, string imageName)
        {
            string imageUrl = imageName;
            if (IsDesignMode)
            {
                imageUrl = Path.Combine(Environment.GetEnvironmentVariable("Temp"), "StiWeb" + requestParams.ComponentType.ToString());
                if (!Directory.Exists(imageUrl)) Directory.CreateDirectory(imageUrl);
                if (!string.IsNullOrEmpty(imageName))
                {
                    imageUrl = Path.Combine(imageUrl, imageName);
                    try
                    {
                        if (!File.Exists(imageUrl))
                        {
                            Bitmap bmp = StiViewerResourcesHelper.GetBitmap(requestParams, imageName);
                            bmp.Save(imageUrl);
                            bmp.Dispose();
                        }
                    }
                    catch
                    {
                    }
                }
            }

            return imageUrl.Replace("'", "\\'").Replace("\"", "&quot;");
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SaveResourcesForJS(string path)
        {
            var requestParams = CreateRequestParams();

            #region Write styles
            string[] themes = Enum.GetNames(typeof(StiViewerTheme));
            for (int i = 0; i < themes.Length; i++)
            {
                string fileName = themes[i];
                if (fileName.StartsWith("Simple")) fileName = fileName.Insert("Simple".Length, ".");
                else if (fileName.StartsWith("Office") && fileName != "Office2003") fileName = fileName.Insert("Office20xx".Length, ".");

                requestParams.Theme = themes[i];
                string styles = Encoding.UTF8.GetString(StiViewerResourcesHelper.GetStyles(requestParams));
                WriteToFile(Path.Combine(path, "Css", "stimulsoft.viewer." + fileName.ToLower() + ".css"), styles);
            }
            #endregion

            #region Write scripts
            requestParams.Theme = "Office2022";
            string jsParameters = RenderJsonParameters(true);

            string scripts = Encoding.UTF8.GetString(StiViewerResourcesHelper.GetScripts(requestParams, true));
            scripts = scripts.Replace("this.defaultParameters = {};",
                "this.defaultParameters = " + jsParameters + ";" +
                "this.mergeOptions(parameters, this.defaultParameters); " +
                "parameters = this.defaultParameters;");

            //Add all sizes scaling images
            var allImages = new Hashtable();
            var scaleFactors = new double[] { 1.25, 1.5, 1.75, 2, 2.25, 2.5 };
            foreach (var scaleFactor in scaleFactors)
            {
                allImages[scaleFactor * 100] = StiViewerResourcesHelper.GetImagesArray(requestParams, scaleFactor);
            }
            scripts += $"//@viewer_scaling_images \r\n var stimulsoftViewerScalingImages = { JsonConvert.SerializeObject(allImages, Formatting.None, new StringEnumConverter()) }\r\n //@end_viewer_scaling_images \r\n";

            WriteToFile(Path.Combine(path, "Scripts", "source.viewer.js"), scripts);


            #endregion
        }

        #endregion
        
        public StiWebViewer()
        {
            StiWebHelper.InitWeb();

            this.ClientIDMode = System.Web.UI.ClientIDMode.Static;
            if (this.BackColor.IsEmpty) this.BackColor = Color.White;
            
            if (IsDesignMode)
            {
                this.Width = Unit.Percentage(100);
                this.Height = Unit.Pixel(650);
            }

#if CLOUD
            this.CloudMode = true;
#endif

#if SERVER
            this.ServerMode = true;
#endif
        }
    }
}
