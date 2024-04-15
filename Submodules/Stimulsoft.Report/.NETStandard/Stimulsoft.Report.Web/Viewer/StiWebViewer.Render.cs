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
using System.Collections;
using System.Text;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Serialization;
using Stimulsoft.Base.Json.Converters;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Helpers;
using Stimulsoft.Base;
using System.Globalization;

namespace Stimulsoft.Report.Web
{
    public partial class StiWebViewer :
        WebControl,
        INamingContainer
    {
#if CLOUD
        private static string IntermediateVersion = "1";
#endif

        protected override void OnPreRender(EventArgs e)
        {
            if (this.Width.IsEmpty) this.Width = Unit.Percentage(100);
            if (this.Height.IsEmpty) this.Height = this.ScrollbarsMode ? Unit.Pixel(650) : Unit.Percentage(100);

            base.OnPreRender(e);
        }

        #region JSON parameters

        private string GetLinksTarget(string value)
        {
            if (value == "Blank") return StiTargetWindow.Blank;
            if (value == "Self") return StiTargetWindow.Self;
            if (value == "Top") return StiTargetWindow.Top;
            return value;
        }

        private string RenderJsonParameters(bool forJsProject = false)
        {
            #region POST form values

            Hashtable formValues = new Hashtable();
            if (this.PassFormValues && HttpContext.Current != null && HttpContext.Current.Request.Form != null)
            {
                foreach (string key in HttpContext.Current.Request.Form.Keys)
                {
                    if (!string.IsNullOrEmpty(key))
                        formValues[key] = HttpContext.Current.Request.Form[key];
                }
            }

            #endregion

            Hashtable jsOptions = new Hashtable();
            jsOptions["viewerId"] = this.ID;
            jsOptions["theme"] = this.Theme;
            jsOptions["clientGuid"] = this.ClientGuid;
            jsOptions["requestAbsoluteUrl"] = GetRequestUrl(false, true, this.PortNumber);
            jsOptions["requestUrl"] = GetRequestUrl(this.UseRelativeUrls, true, this.PortNumber);
            jsOptions["requestResourcesUrl"] = GetRequestUrl(this.UseRelativeUrls, this.PassQueryParametersForResources, this.PortNumber);
            jsOptions["formValues"] = formValues;
            jsOptions["cultureName"] = StiLocalization.CultureName;
            jsOptions["localization"] = this.Localization;
            jsOptions["shortProductVersion"] = StiVersionHelper.AssemblyVersion;
            jsOptions["productVersion"] = StiVersionHelper.ProductVersion.Trim();
            jsOptions["heightType"] = this.Height.Type.ToString();
            jsOptions["cloudMode"] = this.CloudMode;
            jsOptions["serverMode"] = this.ServerMode;
            jsOptions["reportDesignerMode"] = this.ReportDesignerMode;
            jsOptions["frameworkType"] = "ASP.NET";
            jsOptions["dashboardAssemblyLoaded"] = StiDashboardAssembly.IsAssemblyLoaded && StiDashboardExportAssembly.IsAssemblyLoaded && StiDashboardDrawingAssembly.IsAssemblyLoaded;
            jsOptions["alternateValid"] = StiLicenseHelper.CheckAnyLicense();
            jsOptions["buildDate"] = StiVersion.Created.ToString(CultureInfo.CreateSpecificCulture("en-US"));
            jsOptions["listSeparator"] = CultureInfo.CurrentCulture.TextInfo.ListSeparator;

            if (!forJsProject)
            {
                jsOptions["licenseUserName"] = StiLicenseHelper.GetUserName();
                jsOptions["licenseIsValid"] = StiLicenseHelper.IsValidOnWeb();
                jsOptions["stimulsoftFontContent"] = StiReportResourceHelper.GetStimulsoftFontBase64Data();
                jsOptions["customOpenTypeFonts"] = StiFontsHelper.GetCustomOpenTypeFontItems(this.AllowLoadingCustomFontsToClientSide);
                jsOptions["fontNames"] = StiFontsHelper.GetFontNames();
            }

            Hashtable server = new Hashtable();
            server["requestTimeout"] = this.RequestTimeout;
            server["cacheTimeout"] = this.CacheTimeout;
            server["cacheMode"] = this.CacheMode;
            server["cacheItemPriority"] = this.CacheItemPriority;
            server["allowAutoUpdateCache"] = this.AllowAutoUpdateCache;
            server["useRelativeUrls"] = this.UseRelativeUrls;
            server["passQueryParametersForResources"] = this.PassQueryParametersForResources;
            server["passQueryParametersToReport"] = this.PassQueryParametersToReport;
            server["passFormValues"] = this.PassFormValues;
            server["showServerErrorPage"] = this.ShowServerErrorPage;
            server["useCompression"] = this.UseCompression;
            server["useCacheForResources"] = this.UseCacheForResources;
            server["useLocalizedCache"] = this.UseLocalizedCache;
            server["allowAutoUpdateCookies"] = this.AllowAutoUpdateCookies;
            jsOptions["server"] = server;

            Hashtable appearance = new Hashtable();
            appearance["customStylesUrl"] = this.CustomCss;
            appearance["backgroundColor"] = this.BackgroundColor;
            appearance["rightToLeft"] = this.RightToLeft;
            appearance["fullScreenMode"] = this.FullScreenMode;
            appearance["scrollbarsMode"] = this.ScrollbarsMode;
            appearance["openLinksWindow"] = GetLinksTarget(this.OpenLinksWindow);
            appearance["openExportedReportWindow"] = GetLinksTarget(this.OpenExportedReportWindow);
            appearance["designWindow"] = GetLinksTarget(this.DesignWindow);
            appearance["showTooltips"] = this.ShowTooltips;
            appearance["showTooltipsHelp"] = this.ShowTooltipsHelp;
            appearance["showDialogsHelp"] = this.ShowDialogsHelp;
            appearance["pageAlignment"] = this.PageAlignment;
            appearance["showPageShadow"] = this.ShowPageShadow;
            appearance["pageBorderColor"] = this.PageBorderColor;
            appearance["bookmarksPrint"] = this.BookmarksPrint;
            appearance["bookmarksTreeWidth"] = this.BookmarksTreeWidth;
            appearance["parametersPanelPosition"] = this.ParametersPanelPosition;
            appearance["parametersPanelMaxHeight"] = this.ParametersPanelMaxHeight;
            appearance["parametersPanelColumnsCount"] = this.ParametersPanelColumnsCount;
            appearance["parametersPanelDateFormat"] = this.ParametersPanelDateFormat;
            appearance["parametersPanelSortDataItems"] = this.ParametersPanelSortDataItems;
            appearance["interfaceType"] = this.InterfaceType;
            appearance["allowMobileMode"] = this.AllowMobileMode;
            appearance["chartRenderType"] = this.ChartRenderType;
            appearance["reportDisplayMode"] = this.ReportDisplayMode;
            appearance["datePickerFirstDayOfWeek"] = this.DatePickerFirstDayOfWeek;
            appearance["datePickerIncludeCurrentDayForRanges"] = this.DatePickerIncludeCurrentDayForRanges;
            appearance["storeExportSettings"] = this.StoreExportSettings;
            appearance["allowTouchZoom"] = this.AllowTouchZoom;
            appearance["showReportIsNotSpecifiedMessage"] = this.ShowReportIsNotSpecifiedMessage;
            appearance["imagesQuality"] = this.ImagesQuality;
            appearance["printToPdfMode"] = this.PrintToPdfMode;
            appearance["combineReportPages"] = this.CombineReportPages;
            appearance["saveMenuImageSize"] = this.SaveMenuImageSize;
            jsOptions["appearance"] = appearance;

            Hashtable toolbar = new Hashtable();
            toolbar["visible"] = this.ShowToolbar;
            toolbar["displayMode"] = this.ToolbarDisplayMode;
            toolbar["backgroundColor"] = this.ToolbarBackgroundColor;
            toolbar["borderColor"] = this.ToolbarBorderColor;
            toolbar["fontColor"] = this.ToolbarFontColor;
            toolbar["fontFamily"] = this.ToolbarFontFamily;
            toolbar["alignment"] = this.ToolbarAlignment;
            toolbar["showButtonCaptions"] = this.ShowButtonCaptions;
            toolbar["showPrintButton"] = this.ShowPrintButton;
            toolbar["showOpenButton"] = this.ShowOpenButton;
            toolbar["showSaveButton"] = this.ShowSaveButton;
            toolbar["showSendEmailButton"] = this.ShowSendEmailButton;
            toolbar["showFindButton"] = this.ShowFindButton;
            toolbar["showSignatureButton"] = this.ShowSignatureButton;
            toolbar["showBookmarksButton"] = this.ShowBookmarksButton;
            toolbar["showParametersButton"] = this.ShowParametersButton;
            toolbar["showResourcesButton"] = this.ShowResourcesButton;
            toolbar["showEditorButton"] = this.ShowEditorButton;
            toolbar["showFullScreenButton"] = this.ShowFullScreenButton;
            toolbar["showFirstPageButton"] = this.ShowFirstPageButton;
            toolbar["showPreviousPageButton"] = this.ShowPreviousPageButton;
            toolbar["showCurrentPageControl"] = this.ShowCurrentPageControl;
            toolbar["showNextPageButton"] = this.ShowNextPageButton;
            toolbar["showLastPageButton"] = this.ShowLastPageButton;
            toolbar["showZoomButton"] = this.ShowZoomButton;
            toolbar["showViewModeButton"] = this.ShowViewModeButton;
            toolbar["showDesignButton"] = this.ShowDesignButton;
            toolbar["showAboutButton"] = this.ShowAboutButton;
            toolbar["showPinToolbarButton"] = this.ShowPinToolbarButton;
            toolbar["showRefreshButton"] = this.ShowRefreshButton;
            toolbar["printDestination"] = this.PrintDestination;
            toolbar["viewMode"] = this.ViewMode;
            toolbar["zoom"] = this.Zoom;
            toolbar["menuAnimation"] = this.MenuAnimation;
            toolbar["showMenuMode"] = this.ShowMenuMode;
            toolbar["autoHide"] = this.AutoHideToolbar;
            jsOptions["toolbar"] = toolbar;

            Hashtable exports = new Hashtable();
            exports["storeExportSettings"] = this.StoreExportSettings;
            exports["showExportDialog"] = this.ShowExportDialog;
            exports["showExportToDocument"] = this.ShowExportToDocument;
            exports["showExportToPdf"] = this.ShowExportToPdf;
            exports["showExportToXps"] = this.ShowExportToXps;
            exports["showExportToPowerPoint"] = this.ShowExportToPowerPoint;
            exports["showExportToHtml"] = this.ShowExportToHtml;
            exports["showExportToHtml5"] = this.ShowExportToHtml5;
            exports["showExportToMht"] = this.ShowExportToMht;
            exports["showExportToText"] = this.ShowExportToText;
            exports["showExportToRtf"] = this.ShowExportToRtf;
            exports["showExportToWord2007"] = this.ShowExportToWord2007;
            exports["showExportToOpenDocumentWriter"] = this.ShowExportToOpenDocumentWriter;
            exports["showExportToExcel"] = this.ShowExportToExcel;
            exports["showExportToExcelXml"] = this.ShowExportToExcelXml;
            exports["showExportToExcel2007"] = this.ShowExportToExcel2007;
            exports["showExportToOpenDocumentCalc"] = this.ShowExportToOpenDocumentCalc;
            exports["showExportToCsv"] = this.ShowExportToCsv;
            exports["showExportToDbf"] = this.ShowExportToDbf;
            exports["showExportToXml"] = this.ShowExportToXml;
            exports["showExportToDif"] = this.ShowExportToDif;
            exports["showExportToSylk"] = this.ShowExportToSylk;
            exports["showExportToJson"] = this.ShowExportToJson;
            exports["showExportToImageBmp"] = this.ShowExportToImageBmp;
            exports["showExportToImageGif"] = this.ShowExportToImageGif;
            exports["showExportToImageJpeg"] = this.ShowExportToImageJpeg;
            exports["showExportToImagePcx"] = this.ShowExportToImagePcx;
            exports["showExportToImagePng"] = this.ShowExportToImagePng;
            exports["showExportToImageTiff"] = this.ShowExportToImageTiff;
            exports["showExportToImageMetafile"] = this.ShowExportToImageMetafile;
            exports["showExportToImageSvg"] = this.ShowExportToImageSvg;
            exports["showExportToImageSvgz"] = this.ShowExportToImageSvgz;
            exports["showOpenAfterExport"] = this.ShowOpenAfterExport;
            exports["openAfterExport"] = this.OpenAfterExport;
            jsOptions["exports"] = exports;

            Hashtable email = new Hashtable();
            email["showEmailDialog"] = this.ShowEmailDialog;
            email["showExportDialog"] = this.ShowEmailExportDialog;
            email["defaultEmailAddress"] = this.DefaultEmailAddress;
            email["defaultEmailSubject"] = this.DefaultEmailSubject;
            email["defaultEmailMessage"] = this.DefaultEmailMessage;
            jsOptions["email"] = email;

            Hashtable actions = new Hashtable();
            actions["viewerEvent"] = "ViewerEvent";
            jsOptions["actions"] = actions;

            string jsonOptions = JsonConvert.SerializeObject(jsOptions, Formatting.None,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Converters = { new StringEnumConverter() }
                });

            string jsonDefaultExportSettings = JsonConvert.SerializeObject(
                StiExportsHelper.GetDefaultExportSettings(this.DefaultExportSettings),
                Formatting.None, new StringEnumConverter());

            if (ReportDesignerMode)
            {
                var scriptUrl = GetScriptsUrl();
                var jsonScriptsUrl = JsonConvert.SerializeObject(scriptUrl);
                return $"{{options:{jsonOptions},defaultExportSettings:{jsonDefaultExportSettings},scriptsUrl:{jsonScriptsUrl}}}";
            }

            return $"{{options:{jsonOptions},defaultExportSettings:{jsonDefaultExportSettings}}}";
        }

        #endregion

        private string GetScriptsUrl()
        {
            string localizationBase64 = string.IsNullOrEmpty(this.Localization)
                ? null : Convert.ToBase64String(Encoding.UTF8.GetBytes(StiEncryption.Encrypt(this.Localization, StiRequestParamsHelper.EncryptingKey)));

            string scriptUrl = GetRequestUrl(this.UseRelativeUrls, this.PassQueryParametersForResources, this.PortNumber);
            scriptUrl += scriptUrl.IndexOf("?") > 0 ? "&" : "?";
            scriptUrl += "stiweb_component=Viewer&stiweb_action=Resource&stiweb_data=scripts&stiweb_theme=" + this.Theme.ToString();
            if (!string.IsNullOrEmpty(localizationBase64)) scriptUrl += "&stiweb_loc=" + HttpUtility.UrlEncode(localizationBase64);
            scriptUrl += "&stiweb_cachemode=" + (this.UseCacheForResources
                ? this.CacheMode == StiServerCacheMode.ObjectSession || this.CacheMode == StiServerCacheMode.StringSession
                    ? "session"
                    : "cache"
                : "none");
            scriptUrl += "&stiweb_version=" + StiVersionHelper.AssemblyVersion;

#if CLOUD
            scriptUrl += $".{IntermediateVersion}";
#endif

            return scriptUrl;
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            #region Design Mode

            if (IsDesignMode)
            {
                StiRequestParams requestParams = this.CreateRequestParams();

                Panel panel = new Panel();
                panel.Width = this.Width == Unit.Empty ? Unit.Percentage(100) : this.Width;
                panel.Height = this.Height == Unit.Empty ? Unit.Percentage(100) : this.Height;
                panel.Style.Add("overflow", "hidden");

                Table mainTable = new Table();
                mainTable.CellPadding = 0;
                mainTable.CellSpacing = 0;
                mainTable.Width = Unit.Percentage(100);
                mainTable.Height = Unit.Percentage(100);
                mainTable.BorderColor = this.BorderColor.IsEmpty ? Color.DarkGray : this.BorderColor;
                mainTable.BorderWidth = this.BorderWidth.IsEmpty ? 2 : this.BorderWidth;
                mainTable.BorderStyle = this.BorderStyle == BorderStyle.NotSet ? BorderStyle.Solid : this.BorderStyle;
                panel.Controls.Add(mainTable);

                TableRow rowToolbar = new TableRow();
                rowToolbar.VerticalAlign = VerticalAlign.Top;
                mainTable.Rows.Add(rowToolbar);

                TableCell cellToolbarLeft = new TableCell();
                cellToolbarLeft.Style.Add("width", "642px");
                cellToolbarLeft.Style.Add("height", "31px");
                cellToolbarLeft.Style.Add("background", string.Format("url('{0}')", GetImageUrl(requestParams, "DesignToolbarLeftHalf.png")));
                rowToolbar.Cells.Add(cellToolbarLeft);

                TableCell cellToolbarMiddle = new TableCell();
                cellToolbarMiddle.Style.Add("height", "31px");
                cellToolbarMiddle.Style.Add("background", string.Format("url('{0}')", GetImageUrl(requestParams, "DesignToolbarMiddleHalf.png")));
                rowToolbar.Cells.Add(cellToolbarMiddle);

                TableCell cellToolbarRight = new TableCell();
                cellToolbarRight.Style.Add("width", "29px");
                cellToolbarRight.Style.Add("height", "31px");
                rowToolbar.Cells.Add(cellToolbarRight);

                Panel rightPanel = new Panel();
                rightPanel.Style.Add("width", "29px");
                rightPanel.Style.Add("height", "31px");
                rightPanel.Style.Add("background", string.Format("url('{0}')", GetImageUrl(requestParams, "DesignToolbarRightHalf.png")));
                cellToolbarRight.Controls.Add(rightPanel);

                TableRow rowCaption = new TableRow();
                mainTable.Rows.Add(rowCaption);

                TableCell cellCaption = new TableCell();
                cellCaption.ColumnSpan = 3;
                cellCaption.Height = Unit.Percentage(100);
                cellCaption.Font.Name = "Arial";
                cellCaption.Text = "<strong>HTML5 Web Viewer</strong><br />" + this.ID;
                cellCaption.HorizontalAlign = HorizontalAlign.Center;
                rowCaption.Cells.Add(cellCaption);

                panel.RenderControl(writer);
            }

            #endregion

            #region Runtime Mode

            else
            {
                Panel mainPanel = new Panel();
                mainPanel.CssClass = "stiJsViewerMainPanel";
                mainPanel.ID = this.ID + "_JsViewerMainPanel";
                this.Controls.Add(mainPanel);

                string jsParameters = RenderJsonParameters();
                if (ReportDesignerMode)
                {
                    StiJavaScript scriptInit = new StiJavaScript();
                    scriptInit.Text = string.Format("var js{0}Parameters = {1};", this.ID, jsParameters);
                    mainPanel.Controls.Add(scriptInit);
                }
                else
                {
                    StiJavaScript scriptEngine = new StiJavaScript();
                    scriptEngine.ScriptUrl = GetScriptsUrl();
                    mainPanel.Controls.Add(scriptEngine);

                    StiJavaScript scriptInit = new StiJavaScript();
                    scriptInit.Text = string.Format("var js{0} = new StiJsViewer({1});", this.ID, jsParameters);

                    mainPanel.Controls.Add(scriptInit);
                }
            }

            #endregion

            base.RenderContents(writer);
        }
    }
}
