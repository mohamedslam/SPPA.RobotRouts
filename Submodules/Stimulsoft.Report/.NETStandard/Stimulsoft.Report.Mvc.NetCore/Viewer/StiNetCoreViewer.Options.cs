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
using System.Drawing;
using System.ComponentModel;
using Stimulsoft.Base.Json;
using Stimulsoft.Report.Web;
using System.Globalization;
using Stimulsoft.System.Web.UI.WebControls;
using Stimulsoft.System.Web.Caching;

namespace Stimulsoft.Report.Mvc
{
    public class StiNetCoreViewerOptions
    {
        #region Actions

        public class ActionOptions
        {
            /// <summary>
            /// Gets or sets the action method name of preparing the rendered report.
            /// </summary>
            public string GetReport { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the action method name of printing a report.
            /// </summary>
            public string PrintReport { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the action method name of exporting a report to the required format.
            /// </summary>
            public string ExportReport { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the action method name of exporting a report to the required format and send it by email.
            /// </summary>
            public string EmailReport { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the action method name of the report viewer interactions (request from user variables, sorting, drill-down, collasing).
            /// </summary>
            public string Interaction { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the action method name of clicking to design button.
            /// </summary>
            public string DesignReport { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the action method name of the report viewer default events.
            /// </summary>
            public string ViewerEvent { get; set; } = string.Empty;

            #region Obsolete

            [Obsolete("This option is obsolete. It will be removed in next versions. Please use the option GetReport instead.")]
            [EditorBrowsable(EditorBrowsableState.Never)]
            [JsonIgnore]
            public string GetReportSnapshot
            {
                get
                {
                    return GetReport;
                }
                set
                {
                    GetReport = value;
                }
            }

            #endregion
        }

        #endregion
        
        #region Appearance

        public class AppearanceOptions
        {
            /// <summary>
            /// Gets or sets a path to the custom css file for the viewer.
            /// </summary>
            [JsonProperty("CustomStylesUrl")]
            public string CustomCss { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the background color of the viewer.
            /// </summary>
            public Color BackgroundColor { get; set; } = Color.White;

            /// <summary>
            /// Gets or sets a color of the report page border.
            /// </summary>
            public Color PageBorderColor { get; set; } = Color.Gray;

            /// <summary>
            /// Gets or sets a value which controls of output objects in the right to left mode.
            /// </summary>
            public bool RightToLeft { get; set; } = false;

            /// <summary>
            /// Gets or sets a value which indicates which indicates that the viewer is displayed in full screen mode.
            /// </summary>        
            public bool FullScreenMode { get; set; } = false;

            /// <summary>
            /// Gets or sets a value which indicates that the viewer will show the report area with scrollbars.
            /// </summary>
            public bool ScrollbarsMode { get; set; } = false;

            /// <summary>
            /// Gets or sets a browser window to open links from the report.
            /// </summary>
            public string OpenLinksWindow { get; set; } = StiTargetWindow.Blank;

            /// <summary>
            /// Gets or sets a browser window to open the exported report.
            /// </summary>
            public string OpenExportedReportWindow { get; set; } = StiTargetWindow.Blank;

            /// <summary>
            /// Gets or sets a browser window to open page at design action.
            /// </summary>
            public string DesignWindow { get; set; } = StiTargetWindow.Self;

            /// <summary>
            /// Gets or sets a value which indicates that show or hide tooltips.
            /// </summary>
            public bool ShowTooltips { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that show or hide the help link in tooltips.
            /// </summary>
            public bool ShowTooltipsHelp { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that show or hide the help button in dialogs.
            /// </summary>
            public bool ShowDialogsHelp { get; set; } = true;

            /// <summary>
            /// Gets or sets the alignment of the viewer page.
            /// </summary>
            public StiContentAlignment PageAlignment { get; set; } = StiContentAlignment.Center;

            /// <summary>
            /// Gets or sets a value which indicates that the shadow of the page will be displayed in the viewer.
            /// </summary>
            public bool ShowPageShadow { get; set; }

            /// <summary>
            /// Gets or sets a value which allows printing report bookmarks.
            /// </summary>
            public bool BookmarksPrint { get; set; } = false;

            /// <summary>
            /// Gets or sets a width of the bookmarks tree in the viewer.
            /// </summary>
            public int BookmarksTreeWidth { get; set; } = 180;

            /// <summary>
            /// Gets or sets a position of the parameters panel.
            /// </summary>
            public StiParametersPanelPosition ParametersPanelPosition { get; set; } = StiParametersPanelPosition.FromReport;

            /// <summary>
            /// Gets or sets a max height of parameters panel in the viewer.
            /// </summary>
            public int ParametersPanelMaxHeight { get; set; } = 300;

            /// <summary>
            /// Gets or sets a count columns in parameters panel.
            /// </summary>
            public int ParametersPanelColumnsCount { get; set; } = 2;

            /// <summary>
            /// Gets or sets a value which indicates that variable items will be sorted.
            /// </summary>
            public bool ParametersPanelSortDataItems { get; set; } = true;

            private string parametersPanelDateFormat = StiDateFormatMode.FromClient;
            /// <summary>
            /// Gets or sets a date format for datetime parameters in parameters panel. To use a server date format please set the StiDateFormatMode.FromServer or "FromServer" string value.
            /// The default is the client browser date format.
            /// </summary>
            public string ParametersPanelDateFormat
            {
                get
                {
                    if (parametersPanelDateFormat == StiDateFormatMode.FromServer) return CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern + " " + CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern;
                    return parametersPanelDateFormat;
                }
                set
                {
                    parametersPanelDateFormat = value;
                }
            }

            /// <summary>
            /// Gets or sets the type of the viewer interface.
            /// </summary>
            public StiInterfaceType InterfaceType { get; set; } = StiInterfaceType.Auto;

            /// <summary>
            /// Gets or sets a value which indicates that allows mobile mode of the viewer interface.
            /// </summary>
            public bool AllowMobileMode { get; set; } = true;

            /// <summary>
            /// Gets or sets the type of the chart in the viewer.
            /// </summary>
            public StiChartRenderType ChartRenderType { get; set; } = StiChartRenderType.AnimatedVector;

            /// <summary>
            /// Gets or sets a method how the viewer will show a report.
            /// </summary>
            public StiReportDisplayMode ReportDisplayMode { get; set; } = StiReportDisplayMode.FromReport;

            /// <summary>
            /// Gets or sets the first day of week in the date picker.
            /// </summary>
            public StiFirstDayOfWeek DatePickerFirstDayOfWeek { get; set; } = StiFirstDayOfWeek.Auto;

            /// <summary>
            /// Gets or sets a value, which indicates that the current day will be included in the ranges of the date picker.
            /// </summary>
            public bool DatePickerIncludeCurrentDayForRanges { get; set; } = false;

            /// <summary>
            /// Gets or sets a value which allows touch zoom in the viewer.
            /// </summary>
            public bool AllowTouchZoom { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that 'The report is not specified' message will be shown.
            /// </summary>
            public bool ShowReportIsNotSpecifiedMessage { get; set; } = true;

            /// <summary>
            /// Gets or sets the image quality that will be used on the viewer page.
            /// </summary>
            public StiImagesQuality ImagesQuality { get; set; } = StiImagesQuality.Normal;

            /// <summary>
            /// Gets or sets the PDF print mode - hidden or in a pop-up window.
            /// </summary>
            public StiPrintToPdfMode PrintToPdfMode { get; set; }

            /// <summary>
            /// Gets or sets a value which indicates that if a report contains several pages, then they will be combined in preview.
            /// </summary>
            public bool CombineReportPages { get; set; } = false;
        }

        #endregion

        #region Email

        public class EmailOptions
        {
            /// <summary>
            /// Gets or sets a value which allows to display the Email dialog, or send Email with the default settings.
            /// </summary>
            public bool ShowEmailDialog { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which allows to display the export dialog for Email, or export report for Email with the default settings.
            /// </summary>
            public bool ShowExportDialog { get; set; } = true;

            /// <summary>
            /// Gets or sets the default email address of the message created in the viewer.
            /// </summary>
            public string DefaultEmailAddress { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the default subject of the message created in the viewer.
            /// </summary>
            public string DefaultEmailSubject { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the default text of the message created in the viewer.
            /// </summary>
            public string DefaultEmailMessage { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the default text of the replyTo of the message created in the viewer.
            /// </summary>
            public string DefaultEmailReplyTo { get; set; } = string.Empty;
        }

        #endregion

        #region Exports

        public class ExportOptions
        {
            /// <summary>
            /// A class which controls default settings of exports.
            /// </summary>
            [JsonIgnore]
            public StiDefaultExportSettings DefaultSettings { get; set; } = new StiDefaultExportSettings();

            /// <summary>
            /// Gets or sets a value which allows store the export settings in the cookies.
            /// </summary>
            public bool StoreExportSettings { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which allows to display the export dialog, or to export with the default settings.
            /// </summary>
            public bool ShowExportDialog { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the viewer to the report document file.
            /// </summary>
            public bool ShowExportToDocument { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the viewer to the PDF format.
            /// </summary>
            public bool ShowExportToPdf { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the viewer to the XPS format.
            /// </summary>
            public bool ShowExportToXps { get; set; } = false;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the viewer to the Power Point 2007-2010 format.
            /// </summary>
            public bool ShowExportToPowerPoint { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the viewer to the HTML format.
            /// </summary>
            public bool ShowExportToHtml { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the viewer to the HTML5 format.
            /// </summary>
            public bool ShowExportToHtml5 { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the viewer to the MHT (Web Archive) format.
            /// </summary>
            public bool ShowExportToMht { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the viewer to the TEXT format.
            /// </summary>
            public bool ShowExportToText { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the viewer to the Rich Text format.
            /// </summary>
            public bool ShowExportToRtf { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the viewer to the Word 2007-2010 format.
            /// </summary>
            public bool ShowExportToWord2007 { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the viewer to the Open Document Text format.
            /// </summary>
            public bool ShowExportToOpenDocumentWriter { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the viewer to the Excel BIFF format.
            /// </summary>
            public bool ShowExportToExcel { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the viewer to the Excel XML format.
            /// </summary>
            public bool ShowExportToExcelXml { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the viewer to the Excel 2007-2010 format.
            /// </summary>
            public bool ShowExportToExcel2007 { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the viewer to the Open Document Calc format.
            /// </summary>
            public bool ShowExportToOpenDocumentCalc { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the viewer to the CSV format.
            /// </summary>
            public bool ShowExportToCsv { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the viewer to the DBF format.
            /// </summary>
            public bool ShowExportToDbf { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the viewer to the XML format.
            /// </summary>
            public bool ShowExportToXml { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the viewer to the DIF format.
            /// </summary>
            public bool ShowExportToDif { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the viewer to the Sylk format.
            /// </summary>
            public bool ShowExportToSylk { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the viewer to the Json format.
            /// </summary>
            public bool ShowExportToJson { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the viewer to the BMP image format.
            /// </summary>
            public bool ShowExportToImageBmp { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the viewer to the GIF image format.
            /// </summary>
            public bool ShowExportToImageGif { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the viewer to the JPEG image format.
            /// </summary>
            public bool ShowExportToImageJpeg { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the viewer to the PCX image format.
            /// </summary>
            public bool ShowExportToImagePcx { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the viewer to the PNG image format.
            /// </summary>
            public bool ShowExportToImagePng { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the viewer to the TIFF image format.
            /// </summary>
            public bool ShowExportToImageTiff { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the viewer to the Metafile image format.
            /// Not supported in .NET Core
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            [JsonIgnore]
            public bool ShowExportToImageMetafile { get; set; } = false;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the viewer to the SVG image format.
            /// </summary>
            public bool ShowExportToImageSvg { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the viewer to the SVGZ image format.
            /// </summary>
            public bool ShowExportToImageSvgz { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which allows to display the option Open After Export.
            /// </summary>
            public bool ShowOpenAfterExport { get; set; } = true;

            /// <summary>
            /// Gets or sets a value the option Open After Export.
            /// </summary>
            public bool OpenAfterExport { get; set; } = true;
        }

        #endregion
        
        #region Server

        public class ServerOptions
        {
            /// <summary>
            /// Gets or sets the name of the query processing controller of the report viewer. 
            /// </summary>
            public string Controller { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the URL pattern of the route of the report viewer. The {action} parameter can be used for the component actions, for example: /Home/{action}
            /// </summary>
            public string RouteTemplate { get; set; } = string.Empty;

            private int requestTimeout = 30;
            /// <summary>
            /// Gets or sets time which indicates how many seconds the client side will wait for the response from the server side. The default value is 30 seconds.
            /// </summary>
            public int RequestTimeout
            {
                get
                {
                    return requestTimeout;
                }
                set
                {
                    // Min 1 sec. Max 6 hours.
                    requestTimeout = Math.Max(1, Math.Min(21600, value));
                }
            }

            /// <summary>
            /// Gets or sets time which indicates how many minutes the result of the report rendering will be stored in the server cache or session. The default value is 10 minutes.
            /// </summary>
            public int CacheTimeout { get; set; } = 10;

            /// <summary>
            /// Gets or sets the mode of the report caching.
            /// </summary>
            public StiServerCacheMode CacheMode { get; set; } = StiServerCacheMode.ObjectCache;

            /// <summary>
            /// Specifies the relative priority of report, stored in the system cache.
            /// </summary>
            public CacheItemPriority CacheItemPriority { get; set; } = CacheItemPriority.Default;

            /// <summary>
            /// Allows the viewer to update the cache automatically. The cache will be updated about once every three minutes if there are no actions in the viewer.
            /// </summary>
            public bool AllowAutoUpdateCache { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the viewer will use relative or absolute URLs.
            /// </summary>
            public bool UseRelativeUrls { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which specifies the port number to use in the URL. A value of 0 defines automatic detection. A value of -1 removes the port number.
            /// </summary>
            public int PortNumber { get; set; } = 0;

            /// <summary>
            /// Gets or sets a value which enables or disables the transfer of URL parameters when requesting the scripts and styles of the viewer.
            /// </summary>
            public bool PassQueryParametersForResources { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which enables or disables the use of URL parameters as report parameters.
            /// </summary>
            public bool PassQueryParametersToReport { get; set; } = false;

            /// <summary>
            /// Gets or sets a value which enables or disables the transfer POST parameters of the form.
            /// </summary>
            public bool PassFormValues { get; set; } = false;

            /// <summary>
            /// Gets or sets a value which enables or disables the display of the detailed server error in the viewer.
            /// </summary>
            public bool ShowServerErrorPage { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which enables or disables the GZip compression for the requests. This allows to reduce the volume of Internet traffic, but slightly slows down the viewer actions.
            /// </summary>
            public bool UseCompression { get; set; } = false;

            /// <summary>
            /// Gets or sets a value which enables caching for viewer scripts and styles.
            /// </summary>
            public bool UseCacheForResources { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which enables the use of a different cache depending on the selected localization.
            /// </summary>
            public bool UseLocalizedCache { get; set; }

            /// <summary>
            /// Allows the viewer to update the cookies automatically on every request to the server. By default, cookies are set when creating the viewer, if they are not specified in the report.
            /// </summary>
            public bool AllowAutoUpdateCookies { get; set; }

            /// <summary>
            /// Allow the viewer to automatically request and send the antiforgery token.
            /// </summary>
            public bool AllowAntiforgeryToken { get; set; } = true;

            /// <summary>
            /// Allows loading custom fonts to the client side.
            /// </summary>
            public bool AllowLoadingCustomFontsToClientSide { get; set; } = false;
        }

        #endregion

        #region Toolbar

        public class ToolbarOptions
        {
            /// <summary>
            /// Gets or sets a value which indicates that toolbar will be shown in the viewer.
            /// </summary>
            public bool Visible { get; set; } = true;

            /// <summary>
            /// Gets or sets the display mode of the toolbar - simple or separated into upper and lower parts.
            /// </summary>
            public StiToolbarDisplayMode DisplayMode { get; set; } = StiToolbarDisplayMode.Simple;

            /// <summary>
            /// Gets or sets a color of the toolbar background. The default value is the theme color.
            /// </summary>
            public Color BackgroundColor { get; set; } = Color.Empty;

            /// <summary>
            /// Gets or sets a color of the toolbar border. The default value is the theme color.
            /// </summary>
            public Color BorderColor { get; set; } = Color.Empty;

            /// <summary>
            /// Gets or sets a color of the toolbar texts.
            /// </summary>
            public Color FontColor { get; set; } = Color.Empty;

            /// <summary>
            /// Gets or sets a value which indicates which font family will be used for drawing texts in the viewer.
            /// </summary>
            public string FontFamily { get; set; } = "Arial";

            /// <summary>
            /// Gets or sets the alignment of the viewer toolbar.
            /// </summary>
            public StiContentAlignment Alignment { get; set; } = StiContentAlignment.Default;

            /// <summary>
            /// Gets or sets a value which allows displaying or hiding toolbar buttons captions.
            /// </summary>
            public bool ShowButtonCaptions { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the Print button in the toolbar of the viewer.
            /// </summary>
            public bool ShowPrintButton { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the Open button in the toolbar of the viewer.
            /// </summary>
            public bool ShowOpenButton { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the Save button in the toolbar of the viewer.
            /// </summary>
            public bool ShowSaveButton { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the Send Email button in the toolbar of the viewer.
            /// </summary>
            public bool ShowSendEmailButton { get; set; } = false;

            /// <summary>
            /// Gets or sets a visibility of the Find button in the toolbar of the viewer.
            /// </summary>
            public bool ShowFindButton { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the Signature button in the toolbar of the viewer.
            /// </summary>
            public bool ShowSignatureButton { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the Bookmarks button in the toolbar of the viewer.
            /// </summary>
            public bool ShowBookmarksButton { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the Parameters button in the toolbar of the viewer.
            /// </summary>
            public bool ShowParametersButton { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the Resources button in the toolbar of the viewer.
            /// </summary>
            public bool ShowResourcesButton { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the Editor button in the toolbar of the viewer.
            /// </summary>
            public bool ShowEditorButton { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the Full Screen button in the toolbar of the viewer.
            /// </summary>
            public bool ShowFullScreenButton { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the First Page button in the toolbar of the viewer.
            /// </summary>
            public bool ShowFirstPageButton { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the Prev Page button in the toolbar of the viewer.
            /// </summary>
            public bool ShowPreviousPageButton { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the current page control in the toolbar of the viewer.
            /// </summary>
            public bool ShowCurrentPageControl { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the Next Page button in the toolbar of the viewer.
            /// </summary>
            public bool ShowNextPageButton { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the Last Page button in the toolbar of the viewer.
            /// </summary>
            public bool ShowLastPageButton { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the Zoom control in the toolbar of the viewer.
            /// </summary>
            public bool ShowZoomButton { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the View Mode button in the toolbar of the viewer.
            /// </summary>
            public bool ShowViewModeButton { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the Design button in the toolbar of the viewer.
            /// </summary>
            public bool ShowDesignButton { get; set; } = false;

            /// <summary>
            /// Gets or sets a visibility of the About button in the toolbar of the viewer.
            /// </summary>
            public bool ShowAboutButton { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the Refresh button in the toolbar of the viewer.
            /// </summary>
            public bool ShowRefreshButton { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the Pin button in the toolbar of the viewer in mobile mode.
            /// </summary>
            public bool ShowPinToolbarButton { get; set; } = true;

            /// <summary>
            /// Gets or sets the default mode of the report print destination.
            /// </summary>
            public StiPrintDestination PrintDestination { get; set; } = StiPrintDestination.Default;

            /// <summary>
            /// Gets or sets the mode of showing a report in the viewer - Single Page, Continuous or Multiple Pages.
            /// </summary>
            public StiWebViewMode ViewMode { get; set; } = StiWebViewMode.SinglePage;

            private int zoom = 100;
            /// <summary>
            /// Gets or sets the report showing zoom. The default value is 100.
            /// </summary>
            public int Zoom
            {
                get
                {
                    return zoom;
                }
                set
                {
                    if (value > 500) zoom = 500;
                    else if (value < 10 && value >= 0) zoom = 10;
                    else zoom = value;
                }
            }

            /// <summary>
            /// Gets or sets a value which indicates that menu animation is enabled.
            /// </summary>
            public bool MenuAnimation { get; set; } = true;

            /// <summary>
            /// Gets or sets the mode that shows menu of the viewer.
            /// </summary>
            public StiShowMenuMode ShowMenuMode { get; set; } = StiShowMenuMode.Click;

            /// <summary>
            /// Gets or sets a value which allows automatically hide the viewer toolbar in mobile mode.
            /// </summary>
            public bool AutoHide { get; set; } = false;
        }

        #endregion

        #region StiNetCoreViewerOptions

        /// <summary>
        /// A class which controls settings of the viewer actions.
        /// </summary>
        public ActionOptions Actions { get; set; } = new ActionOptions();

        /// <summary>
        /// A class which controls settings of the viewer appearance.
        /// </summary>
        public AppearanceOptions Appearance { get; set; } = new AppearanceOptions();

        /// <summary>
        /// A class which controls the email options.
        /// </summary>
        public EmailOptions Email { get; set; } = new EmailOptions();

        /// <summary>
        /// A class which controls the export options.
        /// </summary>
        public ExportOptions Exports { get; set; } = new ExportOptions();

        /// <summary>
        /// A class which controls the server options.
        /// </summary>
        public ServerOptions Server { get; set; } = new ServerOptions();

        /// <summary>
        /// A class which controls settings of the viewer toolbar.
        /// </summary>
        public ToolbarOptions Toolbar { get; set; } = new ToolbarOptions();

        /// <summary>
        /// Gets or sets the current visual theme which is used for drawing visual elements of the viewer.
        /// </summary>
        public StiViewerTheme Theme { get; set; } = StiViewerTheme.Office2022WhiteBlue;

        /// <summary>
        /// Gets or sets a path to the localization file for the viewer.
        /// </summary>
        public string Localization { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the width of the viewer.
        /// </summary>
        public Unit Width { get; set; } = Unit.Percentage(100);

        /// <summary>
        /// Gets or sets the height of the viewer.
        /// </summary>
        public Unit Height { get; set; } = Unit.Empty;

        #endregion
    }
}
