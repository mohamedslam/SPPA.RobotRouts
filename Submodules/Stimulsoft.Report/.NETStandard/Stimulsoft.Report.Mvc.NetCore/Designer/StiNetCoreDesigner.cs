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
using Stimulsoft.Report.Web;
using System;
using System.Drawing;
using Stimulsoft.System.Web.UI.WebControls;
using Microsoft.AspNetCore.Mvc.Rendering;
using Stimulsoft.System.Web;

namespace Stimulsoft.Report.Mvc
{
    public partial class StiNetCoreDesigner : Panel
    {
        #region Fields

        private IHtmlHelper htmlHelper;
        private StiNetCoreDesignerOptions options;

        #endregion

        #region Properties
        
        private static StiCacheHelper cacheHelper = null;
        /// <summary>
        /// Gets or sets an instance of the StiCacheHelper class that will be used for report caching on the server side.
        /// </summary>
        public static StiCacheHelper CacheHelper
        {
            get
            {
                if (cacheHelper == null) cacheHelper = new StiCacheHelper();
                return cacheHelper;
            }
            set
            {
                cacheHelper = value;
            }
        }

        private string clientGuid = null;
        internal string ClientGuid
        {
            get
            {
                if (clientGuid == null) clientGuid = StiGuidUtils.NewGuid();
                return clientGuid;
            }
            set
            {
                clientGuid = value;
            }
        }

        #endregion

        #region Internal

        /// <summary>
        /// Create default designer RequestParams to save the report into cache
        /// </summary>
        private StiRequestParams CreateRequestParams()
        {
            StiRequestParams requestParams = new StiRequestParams();
            requestParams.HttpContext = new HttpContext(this.htmlHelper.ViewContext.HttpContext);
            requestParams.Action = StiAction.GetReport;
            requestParams.ComponentType = StiComponentType.Designer;
            requestParams.Id = this.ID;
            requestParams.Cache.Mode = options.Server.CacheMode;
            requestParams.Cache.Timeout = new TimeSpan(0, options.Server.CacheTimeout, 0);
            requestParams.Cache.Priority = options.Server.CacheItemPriority;
            requestParams.Cache.ClientGuid = this.ClientGuid;
            requestParams.Cache.Helper = StiNetCoreDesigner.CacheHelper;
            requestParams.Server.UseRelativeUrls = options.Server.UseRelativeUrls;
            requestParams.Server.UseCompression = options.Server.UseCompression;
            requestParams.Server.PassQueryParametersForResources = options.Server.PassQueryParametersForResources;
            requestParams.Server.AllowAutoUpdateCookies = options.Server.AllowAutoUpdateCookies;

            return requestParams;
        }

        /// <summary>
        /// Get the URL to load the images, scripts or styles of the report designer.
        /// </summary>
        private string GetResourcesUrl()
        {
            string url = GetRequestUrl(htmlHelper, options.Server.RouteTemplate, options.Server.Controller, options.Server.UseRelativeUrls, options.Server.PassQueryParametersForResources, options.Server.PortNumber);
            url = url.Replace("{action}", options.Actions.DesignerEvent);
            url += url.IndexOf("?") < 0 ? "?" : "&";
            url += "stiweb_component=Designer&stiweb_action=Resource&stiweb_cachemode=" + (options.Server.UseCacheForResources
                ? options.Server.CacheMode == StiServerCacheMode.ObjectSession || options.Server.CacheMode == StiServerCacheMode.StringSession
                    ? "session"
                    : "cache"
                : "none");
            return url + "&stiweb_version=" + StiVersionHelper.AssemblyVersion + "&stiweb_data=";
        }

        /// <summary>
        /// Get the URL for the designer requests.
        /// </summary>
        internal static string GetRequestUrl(IHtmlHelper htmlHelper, string template, string controller, bool useRelativeUrls, bool passQueryParameters, int portNumber)
        {
            return StiNetCoreViewer.GetRequestUrl(htmlHelper, template, controller, useRelativeUrls, passQueryParameters, portNumber);
        }

        /// <summary>
        /// Get the URL to load the images of the report designer.
        /// </summary>
        internal static string GetImageUrl(StiRequestParams requestParams, string imageName)
        {
            return imageName.Replace("'", "\\'").Replace("\"", "&quot;");
        }

        /// <summary>
        /// Create the report viewer for Preview tab in the designer
        /// </summary>
        private StiNetCoreViewer CreatePreviewControl()
        {
            var viewerOptions = GetPreviewControlOptions(options);
            var viewer = new StiNetCoreViewer(htmlHelper, this.ID + "Viewer", viewerOptions);
            viewer.ReportDesignerMode = true;
            viewer.Style.Add("display", "none");

            return viewer;
        }

        internal static StiNetCoreViewerOptions GetPreviewControlOptions(StiNetCoreDesignerOptions options)
        {
            var viewerOptions = new StiNetCoreViewerOptions();
            viewerOptions.Localization = options.Localization;
            viewerOptions.Theme = (StiViewerTheme)Enum.Parse(typeof(StiViewerTheme), options.Theme.ToString());

            #region Actions
            viewerOptions.Actions.GetReport = options.Actions.GetPreviewReport;
            viewerOptions.Actions.ExportReport = options.Actions.ExportReport;
            viewerOptions.Actions.ViewerEvent = options.Actions.DesignerEvent;
            #endregion

            #region Server
            viewerOptions.Server.Controller = options.Server.Controller;
            viewerOptions.Server.RouteTemplate = options.Server.RouteTemplate;
            viewerOptions.Server.RequestTimeout = options.Server.RequestTimeout;
            viewerOptions.Server.CacheTimeout = options.Server.CacheTimeout;
            viewerOptions.Server.CacheMode = options.Server.CacheMode;
            viewerOptions.Server.CacheItemPriority = options.Server.CacheItemPriority;
            viewerOptions.Server.UseRelativeUrls = options.Server.UseRelativeUrls;
            viewerOptions.Server.PassQueryParametersForResources = options.Server.PassQueryParametersForResources;
            viewerOptions.Server.PassFormValues = options.Server.PassFormValues;
            viewerOptions.Server.ShowServerErrorPage = options.Server.ShowServerErrorPage;
            viewerOptions.Server.UseCompression = options.Server.UseCompression;
            viewerOptions.Server.UseCacheForResources = options.Server.UseCacheForResources;
            viewerOptions.Server.AllowAutoUpdateCache = false;
            viewerOptions.Server.AllowAutoUpdateCookies = options.Server.AllowAutoUpdateCookies;
            viewerOptions.Server.AllowAntiforgeryToken = options.Server.AllowAntiforgeryToken;
            viewerOptions.Server.AllowLoadingCustomFontsToClientSide = options.Server.AllowLoadingCustomFontsToClientSide;
            #endregion

            #region Appearance
            viewerOptions.Appearance.CustomCss = options.Appearance.CustomCss;
            viewerOptions.Appearance.PageBorderColor = Color.FromArgb(198, 198, 198);
            viewerOptions.Appearance.BackgroundColor = Color.FromArgb(241, 241, 241);
            viewerOptions.Appearance.FullScreenMode = true;
            viewerOptions.Appearance.DatePickerFirstDayOfWeek = options.Appearance.DatePickerFirstDayOfWeek;
            viewerOptions.Appearance.ShowTooltips = options.Appearance.ShowTooltips;
            viewerOptions.Appearance.ShowTooltipsHelp = options.Appearance.ShowTooltipsHelp;
            viewerOptions.Appearance.InterfaceType = options.Appearance.InterfaceType;
            viewerOptions.Appearance.ChartRenderType = options.Appearance.ChartRenderType;
            viewerOptions.Appearance.ReportDisplayMode = options.Appearance.ReportDisplayMode;
            viewerOptions.Appearance.ShowPageShadow = false;
            viewerOptions.Appearance.ParametersPanelPosition = StiParametersPanelPosition.FromReport;
            viewerOptions.Appearance.ParametersPanelDateFormat = options.Appearance.ParametersPanelDateFormat;            
            #endregion

            #region Email
            viewerOptions.Email.ShowEmailDialog = options.Email.ShowEmailDialog;
            viewerOptions.Email.ShowExportDialog = options.Email.ShowExportDialog;
            viewerOptions.Email.DefaultEmailAddress = options.Email.DefaultEmailAddress;
            viewerOptions.Email.DefaultEmailSubject = options.Email.DefaultEmailSubject;
            viewerOptions.Email.DefaultEmailMessage = options.Email.DefaultEmailMessage;
            viewerOptions.Email.DefaultEmailReplyTo = options.Email.DefaultEmailReplyTo;
            #endregion

            #region Exports
            viewerOptions.Exports.DefaultSettings = options.Exports.DefaultSettings;
            viewerOptions.Exports.StoreExportSettings = options.Exports.StoreExportSettings;
            viewerOptions.Exports.ShowExportDialog = options.Exports.ShowExportDialog;
            viewerOptions.Exports.ShowExportToDocument = options.Exports.ShowExportToDocument;
            viewerOptions.Exports.ShowExportToPdf = options.Exports.ShowExportToPdf;
            viewerOptions.Exports.ShowExportToXps = options.Exports.ShowExportToXps;
            viewerOptions.Exports.ShowExportToPowerPoint = options.Exports.ShowExportToPowerPoint;
            viewerOptions.Exports.ShowExportToHtml = options.Exports.ShowExportToHtml;
            viewerOptions.Exports.ShowExportToHtml5 = options.Exports.ShowExportToHtml5;
            viewerOptions.Exports.ShowExportToMht = options.Exports.ShowExportToMht;
            viewerOptions.Exports.ShowExportToText = options.Exports.ShowExportToText;
            viewerOptions.Exports.ShowExportToRtf = options.Exports.ShowExportToRtf;
            viewerOptions.Exports.ShowExportToWord2007 = options.Exports.ShowExportToWord2007;
            viewerOptions.Exports.ShowExportToOpenDocumentWriter = options.Exports.ShowExportToOpenDocumentWriter;
            viewerOptions.Exports.ShowExportToExcel = options.Exports.ShowExportToExcel;
            viewerOptions.Exports.ShowExportToExcelXml = options.Exports.ShowExportToExcelXml;
            viewerOptions.Exports.ShowExportToExcel2007 = options.Exports.ShowExportToExcel2007;
            viewerOptions.Exports.ShowExportToOpenDocumentCalc = options.Exports.ShowExportToOpenDocumentCalc;
            viewerOptions.Exports.ShowExportToCsv = options.Exports.ShowExportToCsv;
            viewerOptions.Exports.ShowExportToDbf = options.Exports.ShowExportToDbf;
            viewerOptions.Exports.ShowExportToXml = options.Exports.ShowExportToXml;
            viewerOptions.Exports.ShowExportToDif = options.Exports.ShowExportToDif;
            viewerOptions.Exports.ShowExportToSylk = options.Exports.ShowExportToSylk;
            viewerOptions.Exports.ShowExportToJson = options.Exports.ShowExportToJson;
            viewerOptions.Exports.ShowExportToImageBmp = options.Exports.ShowExportToImageBmp;
            viewerOptions.Exports.ShowExportToImageGif = options.Exports.ShowExportToImageGif;
            viewerOptions.Exports.ShowExportToImageJpeg = options.Exports.ShowExportToImageJpeg;
            viewerOptions.Exports.ShowExportToImagePcx = options.Exports.ShowExportToImagePcx;
            viewerOptions.Exports.ShowExportToImagePng = options.Exports.ShowExportToImagePng;
            viewerOptions.Exports.ShowExportToImageTiff = options.Exports.ShowExportToImageTiff;
            viewerOptions.Exports.ShowExportToImageMetafile = options.Exports.ShowExportToImageMetafile;
            viewerOptions.Exports.ShowExportToImageSvg = options.Exports.ShowExportToImageSvg;
            viewerOptions.Exports.ShowExportToImageSvgz = options.Exports.ShowExportToImageSvgz;
            #endregion

            #region Toolbar
            viewerOptions.Toolbar.DisplayMode = StiToolbarDisplayMode.Separated;
            viewerOptions.Toolbar.Visible = options.PreviewToolbar.Visible;
            viewerOptions.Toolbar.BackgroundColor = options.PreviewToolbar.BackgroundColor;
            viewerOptions.Toolbar.BorderColor = options.PreviewToolbar.BorderColor;
            viewerOptions.Toolbar.FontColor = options.PreviewToolbar.FontColor;
            viewerOptions.Toolbar.FontFamily = options.PreviewToolbar.FontFamily;
            viewerOptions.Toolbar.Alignment = options.PreviewToolbar.Alignment;
            viewerOptions.Toolbar.ShowButtonCaptions = options.PreviewToolbar.ShowButtonCaptions;
            viewerOptions.Toolbar.ShowPrintButton = options.PreviewToolbar.ShowPrintButton;
            viewerOptions.Toolbar.ShowOpenButton = options.PreviewToolbar.ShowOpenButton;
            viewerOptions.Toolbar.ShowSaveButton = options.PreviewToolbar.ShowSaveButton;
            viewerOptions.Toolbar.ShowSendEmailButton = options.PreviewToolbar.ShowSendEmailButton;
            viewerOptions.Toolbar.ShowFindButton = options.PreviewToolbar.ShowFindButton;
            viewerOptions.Toolbar.ShowSignatureButton = options.PreviewToolbar.ShowSignatureButton;
            viewerOptions.Toolbar.ShowBookmarksButton = options.PreviewToolbar.ShowBookmarksButton;
            viewerOptions.Toolbar.ShowParametersButton = options.PreviewToolbar.ShowParametersButton;
            viewerOptions.Toolbar.ShowEditorButton = options.PreviewToolbar.ShowEditorButton;
            viewerOptions.Toolbar.ShowDesignButton = false;
            viewerOptions.Toolbar.ShowAboutButton = false;
            viewerOptions.Toolbar.ShowFullScreenButton = false;
            viewerOptions.Toolbar.ShowFirstPageButton = options.PreviewToolbar.ShowFirstPageButton;
            viewerOptions.Toolbar.ShowPreviousPageButton = options.PreviewToolbar.ShowPreviousPageButton;
            viewerOptions.Toolbar.ShowCurrentPageControl = options.PreviewToolbar.ShowCurrentPageControl;
            viewerOptions.Toolbar.ShowNextPageButton = options.PreviewToolbar.ShowNextPageButton;
            viewerOptions.Toolbar.ShowLastPageButton = options.PreviewToolbar.ShowLastPageButton;
            viewerOptions.Toolbar.ShowZoomButton = options.PreviewToolbar.ShowZoomButton;
            viewerOptions.Toolbar.ShowViewModeButton = options.PreviewToolbar.ShowViewModeButton;
            viewerOptions.Toolbar.PrintDestination = options.PreviewToolbar.PrintDestination;
            viewerOptions.Toolbar.ViewMode = options.PreviewToolbar.ViewMode;
            #endregion

            return viewerOptions;
        }

        #endregion

        public StiNetCoreDesigner(IHtmlHelper htmlHelper, string id, StiNetCoreDesignerOptions options)
        {
            this.htmlHelper = htmlHelper;
            this.ID = id;
            this.options = options;
        }
    }
}
