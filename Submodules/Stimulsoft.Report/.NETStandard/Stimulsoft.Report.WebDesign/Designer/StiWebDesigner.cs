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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Reflection;
using Stimulsoft.Report.Dashboard;
using System.ComponentModel;

namespace Stimulsoft.Report.Web
{
    [ToolboxBitmap(typeof(StiWebDesigner), "Designer.Images.StiWebDesigner.bmp")]
    public partial class StiWebDesigner :
        WebControl,
        INamingContainer
    {
#if CLOUD
        private static string IntermediateVersion = "3";
#endif

        #region Component Helpers

        /// <summary>
        /// Create default designer RequestParams to save the report into cache
        /// </summary>
        private StiRequestParams CreateRequestParams()
        {
            StiRequestParams requestParams = new StiRequestParams();
            requestParams.Action = StiAction.GetReport;
            requestParams.ComponentType = StiComponentType.Designer;
            requestParams.Id = this.ClientID;
            requestParams.CloudMode = this.CloudMode;
            requestParams.ServerMode = this.ServerMode;
            requestParams.Cache.Mode = this.CacheMode;
            requestParams.Cache.Timeout = new TimeSpan(0, this.CacheTimeout, 0);
            requestParams.Cache.Priority = this.CacheItemPriority;
            requestParams.Cache.ClientGuid = this.ClientGuid;
            requestParams.Cache.Helper = CacheHelper;
            requestParams.Server.UseRelativeUrls = this.UseRelativeUrls;
            requestParams.Server.UseCompression = this.UseCompression;
            requestParams.Server.PassQueryParametersForResources = this.PassQueryParametersForResources;
            
            return requestParams;
        }
        
        private static Hashtable GetDesignerParametersFromServer(StiRequestParams requestParams)
        {
            if (!requestParams.ServerMode || HttpContext.Current.Request.Form.AllKeys == null || HttpContext.Current.Request.Form.AllKeys.Length == 0) return null;
            string[] keys = HttpContext.Current.Request.Form.AllKeys;
            Hashtable parameters = new Hashtable();
            for (int i = 0; i < keys.Length; i++)
            {
                parameters[keys[i]] = HttpContext.Current.Request.Form[keys[i]];
            }
            return parameters;
        }
        
        private void RegisterClientScriptBlockIntoHeader(string key, string script)
        {
            if (HttpContext.Current.Items.Contains("scriptblock_" + key)) return;
            HttpContext.Current.Items.Add("scriptblock_" + key, string.Empty);
            int? index = HttpContext.Current.Items["__ScriptResourceIndex"] as int?;
            if (index == null) index = 0;
            Page.Header.Controls.Add(new LiteralControl(script));
            HttpContext.Current.Items["__ScriptResourceIndex"] = index;
        }

        #endregion

        #region Preview Control

        /// <summary>
        /// Create the report viewer for Preview tab in the designer
        /// </summary>
        private StiWebViewer CreateViewer()
        {
            StiWebViewer viewer = new StiWebViewer();
            viewer.ID = $"{ID}Viewer";
            viewer.CssClass = "StiMobileViewerClass";
            viewer.Style.Add("display", "none");
            viewer.ReportDesignerMode = true;
            viewer.CloudMode = CloudMode;
            viewer.ServerMode = ServerMode;

            #region Appearance
            viewer.Localization = this.Localization;
            viewer.CustomCss = CustomCss;
            viewer.PageBorderColor = Color.FromArgb(198, 198, 198);
            viewer.BackgroundColor = Color.FromArgb(241, 241, 241);
            viewer.Theme = (StiViewerTheme)Enum.Parse(typeof(StiViewerTheme), this.Theme.ToString());
            viewer.FullScreenMode = true;
            viewer.DatePickerFirstDayOfWeek = this.DatePickerFirstDayOfWeek;
            viewer.DatePickerIncludeCurrentDayForRanges = this.DatePickerIncludeCurrentDayForRanges;
            viewer.ShowTooltips = this.ShowTooltips;
            viewer.ShowTooltipsHelp = this.ShowTooltipsHelp;
            viewer.InterfaceType = this.InterfaceType;
            viewer.ChartRenderType = this.ChartRenderType;
            viewer.ReportDisplayMode = this.ReportDisplayMode;
            viewer.ShowPageShadow = false;
            viewer.ParametersPanelPosition = StiParametersPanelPosition.FromReport;
            viewer.ParametersPanelDateFormat = this.ParametersPanelDateFormat;
            viewer.ParametersPanelSortDataItems = this.ParametersPanelSortDataItems;
            viewer.AllowLoadingCustomFontsToClientSide = this.AllowLoadingCustomFontsToClientSide;
            #endregion

            #region Email
            viewer.ShowEmailDialog = this.ShowEmailDialog;
            viewer.ShowEmailExportDialog = this.ShowEmailExportDialog;
            viewer.DefaultEmailAddress = this.DefaultEmailAddress;
            viewer.DefaultEmailSubject = this.DefaultEmailSubject;
            viewer.DefaultEmailMessage = this.DefaultEmailMessage;
            #endregion

            #region Exports
            viewer.DefaultExportSettings = this.DefaultExportSettings;
            viewer.StoreExportSettings = this.StoreExportSettings;
            viewer.ShowExportDialog = this.ShowExportDialog;
            viewer.ShowExportToDocument = this.ShowExportToDocument;
            viewer.ShowExportToPdf = this.ShowExportToPdf;
            viewer.ShowExportToXps = this.ShowExportToXps;
            viewer.ShowExportToPowerPoint = this.ShowExportToPowerPoint;
            viewer.ShowExportToHtml = this.ShowExportToHtml;
            viewer.ShowExportToHtml5 = this.ShowExportToHtml5;
            viewer.ShowExportToMht = this.ShowExportToMht;
            viewer.ShowExportToText = this.ShowExportToText;
            viewer.ShowExportToRtf = this.ShowExportToRtf;
            viewer.ShowExportToWord2007 = this.ShowExportToWord2007;
            viewer.ShowExportToOpenDocumentWriter = this.ShowExportToOpenDocumentWriter;
            viewer.ShowExportToExcel = this.ShowExportToExcel;
            viewer.ShowExportToExcelXml = this.ShowExportToExcelXml;
            viewer.ShowExportToExcel2007 = this.ShowExportToExcel2007;
            viewer.ShowExportToOpenDocumentCalc = this.ShowExportToOpenDocumentCalc;
            viewer.ShowExportToCsv = this.ShowExportToCsv;
            viewer.ShowExportToDbf = this.ShowExportToDbf;
            viewer.ShowExportToXml = this.ShowExportToXml;
            viewer.ShowExportToDif = this.ShowExportToDif;
            viewer.ShowExportToSylk = this.ShowExportToSylk;
            viewer.ShowExportToImageBmp = this.ShowExportToImageBmp;
            viewer.ShowExportToImageGif = this.ShowExportToImageGif;
            viewer.ShowExportToImageJpeg = this.ShowExportToImageJpeg;
            viewer.ShowExportToImagePcx = this.ShowExportToImagePcx;
            viewer.ShowExportToImagePng = this.ShowExportToImagePng;
            viewer.ShowExportToImageTiff = this.ShowExportToImageTiff;
            viewer.ShowExportToImageMetafile = this.ShowExportToImageMetafile;
            viewer.ShowExportToImageSvg = this.ShowExportToImageSvg;
            viewer.ShowExportToImageSvgz = this.ShowExportToImageSvgz;
            #endregion

            #region Toolbar
            viewer.ToolbarDisplayMode = StiToolbarDisplayMode.Separated;
            viewer.ShowToolbar = this.ShowPreviewToolbar;
            viewer.ToolbarBackgroundColor = this.PreviewToolbarBackgroundColor;
            viewer.ToolbarBorderColor = this.PreviewToolbarBorderColor;
            viewer.ToolbarFontColor = this.PreviewToolbarFontColor;
            viewer.ToolbarFontFamily = this.PreviewToolbarFontFamily;
            viewer.ToolbarAlignment = this.PreviewToolbarAlignment;
            viewer.ShowButtonCaptions = this.ShowPreviewButtonCaptions;
            viewer.ShowPrintButton = this.ShowPreviewPrintButton;
            viewer.ShowOpenButton = this.ShowPreviewOpenButton;
            viewer.ShowSaveButton = this.ShowPreviewSaveButton;
            viewer.ShowSendEmailButton = this.ShowPreviewSendEmailButton;
            viewer.ShowFindButton = this.ShowPreviewFindButton;
            viewer.ShowSignatureButton = this.ShowPreviewSignatureButton;
            viewer.ShowBookmarksButton = this.ShowPreviewBookmarksButton;
            viewer.ShowParametersButton = this.ShowPreviewParametersButton;
            viewer.ShowEditorButton = this.ShowPreviewEditorButton;
            viewer.ShowDesignButton = false;
            viewer.ShowAboutButton = false;
            viewer.ShowFullScreenButton = false;
            viewer.ShowFirstPageButton = this.ShowPreviewFirstPageButton;
            viewer.ShowPreviousPageButton = this.ShowPreviewPreviousPageButton;
            viewer.ShowCurrentPageControl = this.ShowPreviewCurrentPageControl;
            viewer.ShowNextPageButton = this.ShowPreviewNextPageButton;
            viewer.ShowLastPageButton = this.ShowPreviewLastPageButton;
            viewer.ShowZoomButton = this.ShowPreviewZoomButton;
            viewer.ShowViewModeButton = this.ShowPreviewViewModeButton;
            viewer.PrintDestination = this.PrintDestination;
            viewer.ViewMode = this.PreviewViewMode;
            #endregion

            #region Server
            viewer.RequestTimeout = this.RequestTimeout;
            viewer.CacheMode = this.CacheMode;
            viewer.CacheItemPriority = this.CacheItemPriority;
            viewer.CacheTimeout = this.CacheTimeout;
            viewer.PassQueryParametersForResources = this.PassQueryParametersForResources;
            viewer.ShowServerErrorPage = this.ShowServerErrorPage;
            viewer.UseCompression = this.UseCompression;
            viewer.UseRelativeUrls = this.UseRelativeUrls;
            viewer.UseCacheForResources = this.UseCacheForResources;
            viewer.AllowAutoUpdateCache = false;
            #endregion

            #region Events
            viewer.GetReportData += new StiReportDataEventHandler(ViewerGetReportData);
            viewer.ExportReport += new StiExportReportEventHandler(ViewerExportReport);
            viewer.ExportReportResponse += new StiExportReportResponseEventHandler(ViewerExportReportResponse);
            #endregion

            return viewer;
        }

        private void ViewerGetReportData(object sender, StiReportDataEventArgs e)
        {
            InvokePreviewReport(e.Report);
        }

        private void ViewerExportReport(object sender, StiExportReportEventArgs e)
        {
            InvokeExportReport(e);
        }

        private void ViewerExportReportResponse(object sender, StiExportReportResponseEventArgs e)
        {
            InvokeExportReportResponse(e);
        }

        #endregion

        #region URLs
        private static string CloudServerAdress = "https://cloud.stimulsoft.com/";//"http://localhost:17764/";

        /// <summary>
        /// Get the URL for designer requests
        /// </summary>
        private static string GetRequestUrl(bool useRelativeUrls, bool passQueryParameters, int portNumber)
        {
            if (HttpContext.Current == null) return null;

            var result = StiWebViewer.GetRequestUrl(useRelativeUrls, passQueryParameters, portNumber);

            // Remove all "stiweb_xxx" parameters from result
            if (result.IndexOf("?") > 0)
            {
                var query = result.Substring(result.IndexOf("?") + 1);
                result = result.Substring(0, result.IndexOf("?") + 1);

                var parameters = HttpUtility.ParseQueryString(query);
                foreach (string parameter in parameters)
                {
                    if (parameter != null && !parameter.StartsWith("stiweb_")) result = string.Format("{0}{1}={2}&", result, parameter, parameters[parameter]);
                }
                result = result.Substring(0, result.Length - 1);
            }

            return result;
        }

        /// <summary>
        /// Get the URL to load the scripts, styles or images of the report designer.
        /// </summary>
        private string GetResourcesUrl()
        {
            string url = GetRequestUrl(this.UseRelativeUrls, this.PassQueryParametersForResources, this.PortNumber);
            url += url.IndexOf("?") < 0 ? "?" : "&";
            url += "stiweb_component=Designer&stiweb_action=Resource&stiweb_cachemode=" + (this.UseCacheForResources
                ? this.CacheMode == StiServerCacheMode.ObjectSession || this.CacheMode == StiServerCacheMode.StringSession
                    ? "session"
                    : "cache"
                : "none");

            if (!IsDesignMode && CloudMode)
            {
                if (this.Page.Request.Params["localizationName"] != null) url += "&localizationName=" + this.Page.Request.Params["localizationName"];
                if (this.Page.Request.Params["sessionKey"] != null) url += "&sessionKey=" + this.Page.Request.Params["sessionKey"];
                url += "&stiweb_cloudmode=true";
            }

            url += "&stiweb_version=" + StiVersionHelper.AssemblyVersion;

#if CLOUD
            url += $".{IntermediateVersion}";
#endif

            return url + "&stiweb_data=";
        }

        /// <summary>
        /// Get the URL to load the images of the report designer.
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
                            Bitmap bmp = StiDesignerResourcesHelper.GetBitmap(requestParams, imageName);
                            bmp.Save(imageUrl);
                            bmp.Dispose();
                        }
                    }
                    catch
                    {
                    }
                }
            }

            return StiUrlHelper.EscapeUrlQuotes(imageUrl);
        }
        #endregion

        #region Get resources for Reports.JS product

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

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SaveResourcesForJS(string path)
        {
            var parameters = RenderJsonParameters(true) as Hashtable;
            var includedDashboards = StiDashboardAssembly.IsAssemblyLoaded;
            if (includedDashboards) parameters["dashboardAssemblyLoaded"] = true;

            //Override Parameters
            parameters["report"] = null;
            parameters["haveExitDesignerEvent"] = false;
            parameters["haveSaveEvent"] = true;
            parameters["haveSaveAsEvent"] = true;
            parameters["jsMode"] = true;
            parameters["loc"] = "";
            parameters["textFormats"] = "";
            parameters["dateFormats"] = "";
            parameters["timeFormats"] = "";
            parameters["requestTimeout"] = 30;
            parameters["cacheTimeout"] = 10;
            parameters["images"] = StiDesignerResourcesHelper.GetImagesArray(this.CreateRequestParams(), null, 1, false);

            Assembly a = typeof(StiWebDesigner).Assembly;
            var names = a.GetManifestResourceNames();
            string scripts = string.Empty;

            foreach (var name in names)
            {
                if (name.IndexOf("Stimulsoft.Report.Web.Designer.Scripts") == 0 && 
                    name.EndsWith(".js") &&
                    name.IndexOf(".LoginControls.") < 0 &&
                    (name.IndexOf(".Dashboards.") < 0 || includedDashboards) &&
                    !name.EndsWith(".Scripts.Client.js"))
                {
                    Stream stream = a.GetManifestResourceStream(name);
                    using (StreamReader reader = new StreamReader(stream)) scripts += reader.ReadToEnd() + "\r\n";
                    stream.Dispose();
                }
            }

            string jsParameters = JSON.Encode(parameters);

            scripts = scripts.Replace("function StiMobileDesigner", "function StiJsDesigner").Replace("StiMobileDesigner.prototype", "StiJsDesigner.prototype").Replace("StiMobileDesigner.", "StiJsDesigner.");
            scripts = scripts.Replace("this.defaultParameters = {};",
                "this.defaultParameters = " + jsParameters + "; " +
                "this.mergeOptions(parameters, this.defaultParameters); " +
                "parameters = this.defaultParameters;");

            //Add all sizes scaling images
            var allImages = new Hashtable();
            var scaleFactors = new double[] { 1.25, 1.5, 1.75, 2, 2.25, 2.5 };
            foreach (var scaleFactor in scaleFactors)
            {
                allImages[scaleFactor * 100] = StiDesignerResourcesHelper.GetImagesArray(this.CreateRequestParams(), null, scaleFactor, false);
            }
            scripts += $"//@designer_scaling_images\r\n var stimulsoftDesignerScalingImages = {JSON.Encode(allImages)}\r\n//@end_designer_scaling_images\r\n";

            WriteToFile(Path.Combine(path, "Scripts", "source.designer.js"), scripts);
            
            //Write Styles
            string[] themes = new string[] {"Office2013DarkGrayBlue", "Office2013DarkGrayCarmine", "Office2013DarkGrayGreen", "Office2013DarkGrayOrange", "Office2013DarkGrayPurple",
                "Office2013DarkGrayTeal", "Office2013DarkGrayViolet", "Office2013LightGrayBlue", "Office2013LightGrayCarmine", "Office2013LightGrayGreen", "Office2013LightGrayOrange",
                "Office2013LightGrayPurple", "Office2013LightGrayTeal", "Office2013LightGrayViolet", "Office2013WhiteBlue", "Office2013WhiteCarmine", "Office2013WhiteGreen", "Office2013WhiteOrange",
                "Office2013WhitePurple", "Office2013WhiteTeal", "Office2013WhiteViolet", "Office2013VeryDarkGrayBlue", "Office2013VeryDarkGrayCarmine", "Office2013VeryDarkGrayGreen",
                "Office2013VeryDarkGrayOrange", "Office2013VeryDarkGrayPurple", "Office2013VeryDarkGrayTeal", "Office2013VeryDarkGrayViolet" };

            for (int i = 0; i < themes.Length; i++)
            {
                string themeName = themes[i];
                var stylesStr = string.Empty;
                string pathThemeName = themeName.StartsWith("Office2013") ? "Office2013" : themeName;
                var pathStyles = string.Format("Stimulsoft.Report.Web.Designer.Styles.{0}.", pathThemeName);
                Hashtable constants = null;

                foreach (var name in names)
                {
                    if (name.IndexOf(pathStyles) == 0 && name.EndsWith(".css") &&
                        name.IndexOf(".LoginControls.") < 0 && (name.IndexOf(".Dashboards.") < 0 || includedDashboards))
                    {
                        Stream stream = a.GetManifestResourceStream(name);
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            string cssText = reader.ReadToEnd();
                            if (name.EndsWith(themeName + ".Constants.css")) constants = StiDesignerResourcesHelper.GetStylesConstants(cssText);
                            else if (!name.EndsWith(".Constants.css")) stylesStr += cssText + "\r\n";
                        }
                    }
                }

                if (constants != null)
                {
                    foreach (DictionaryEntry constant in constants)
                    {
                        stylesStr = stylesStr.Replace((string)constant.Key, (string)constant.Value);
                    }
                }

                WriteToFile(Path.Combine(path, "Css", "stimulsoft.designer." + themes[i].Insert("Office2013".Length, ".").ToLower() + ".css"), stylesStr);
            }
        }

        #endregion

        public StiWebDesigner()
        {
            this.ClientIDMode = System.Web.UI.ClientIDMode.Static;
            this.Width = Unit.Empty;
            this.Height = Unit.Empty;
        }
    }
}
