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
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Caching;
using Stimulsoft.Report.Export;
using Stimulsoft.Base;
using System.Globalization;

namespace Stimulsoft.Report.Web
{
    public partial class StiWebViewer :
        WebControl, 
        INamingContainer
    {
        #region Appearance
        /// <summary>
        /// Gets or sets the current visual theme which is used for drawing visual elements of the viewer.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(StiViewerTheme.Office2022WhiteBlue)]
        [Description("Gets or sets the current visual theme which is used for drawing visual elements of the viewer.")]
        public StiViewerTheme Theme { get; set; } = StiViewerTheme.Office2022WhiteBlue;

        /// <summary>
        /// Gets or sets a path to the custom css file for the viewer.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue("")]
        [Description("Gets or sets a path to the custom css file for the viewer.")]
        public string CustomCss { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a path to the localization file for the viewer.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue("")]
        [Description("Gets or sets a path to the localization file for the viewer.")]
        public string Localization { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the background color of the viewer.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "White")]
        [Description("Gets or sets the background color of the viewer.")]
        [TypeConverter(typeof(WebColorConverter))]
        public Color BackgroundColor
        {
            get
            {
                return this.BackColor;
            }
            set
            {
                this.BackColor = value;
            }
        }

        /// <summary>
        /// Gets or sets a color of the report page border.
        /// </summary>
        [DefaultValue(typeof(Color), "Gray")]
        [Category("Appearance")]
        [Description("Gets or sets a color of the report page border.")]
        [TypeConverter(typeof(WebColorConverter))]
        public Color PageBorderColor { get; set; } = Color.Gray;

        /// <summary>
        /// Gets or sets a value which controls of output objects in the right to left mode.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(false)]
        [Description("Gets or sets a value which controls of output objects in the right to left mode.")]
        public bool RightToLeft { get; set; }

        /// <summary>
        /// Gets or sets a value which indicates which indicates that the viewer is displayed in full screen mode.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(false)]
        [Description("Gets or sets a value which indicates which indicates that the viewer is displayed in full screen mode.")]
        public bool FullScreenMode { get; set; }

        /// <summary>
        /// Gets or sets a value which indicates that the viewer will show the report area with scrollbars.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(false)]
        [Description("Gets or sets a value which indicates that the viewer will show the report area with scrollbars.")]
        public bool ScrollbarsMode { get; set; }

        /// <summary>
        /// Gets or sets a browser window to open links from the report.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(StiTargetWindow.Blank)]
        [Description("Gets or sets a browser window to open links from the report.")]
        public string OpenLinksWindow { get; set; } = StiTargetWindow.Blank;

        /// <summary>
        /// Gets or sets a browser window to open the exported report.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(StiTargetWindow.Blank)]
        [Description("Gets or sets a browser window to open the exported report.")]
        public string OpenExportedReportWindow { get; set; } = StiTargetWindow.Blank;

        /// <summary>
        /// Gets or sets a browser window to open page at design event.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(StiTargetWindow.Self)]
        [Description("Gets or sets a browser window to open page at design event.")]
        public string DesignWindow { get; set; } = StiTargetWindow.Self;

        /// <summary>
        /// Gets or sets a value which indicates that show or hide tooltips.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that show or hide tooltips.")]
        public bool ShowTooltips { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that show or hide the help link in tooltips.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that show or hide the help link in tooltips.")]
        public bool ShowTooltipsHelp { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that show or hide the help button in dialogs.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that show or hide the help button in dialogs.")]
        public bool ShowDialogsHelp { get; set; } = true;

        /// <summary>
        /// Gets or sets the alignment of the viewer page.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(StiContentAlignment.Center)]
        [Description("Gets or sets the alignment of the viewer page.")]
        public StiContentAlignment PageAlignment { get; set; } = StiContentAlignment.Center;

        /// <summary>
        /// Gets or sets a value which indicates that the shadow of the page will be displayed in the viewer.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the shadow of the page will be displayed in the viewer.")]
        public bool ShowPageShadow { get; set; } = false;

        /// <summary>
        /// Gets or sets a value which allows printing report bookmarks.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(false)]
        [Description("Gets or sets a value which allows printing report bookmarks.")]
        public bool BookmarksPrint { get; set; }

        /// <summary>
        /// Gets or sets a width of the bookmarks tree in the viewer.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(180)]
        [Description("Gets or sets a width of the bookmarks tree in the viewer.")]
        public int BookmarksTreeWidth { get; set; } = 180;

        /// <summary>
        /// Gets or sets a position of the parameters panel.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(StiParametersPanelPosition.FromReport)]
        [Description("Gets or sets a position of the parameters panel.")]
        public StiParametersPanelPosition ParametersPanelPosition { get; set; } = StiParametersPanelPosition.FromReport;

        /// <summary>
        /// Gets or sets a max height of parameters panel in the viewer.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(300)]
        [Description("Gets or sets a max height of parameters panel in the viewer.")]
        public int ParametersPanelMaxHeight { get; set; } = 300;

        /// <summary>
        /// Gets or sets a count columns in parameters panel.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(2)]
        [Description("Gets or sets a count columns in parameters panel.")]
        public int ParametersPanelColumnsCount { get; set; } = 2;

        /// <summary>
        /// Gets or sets a value which indicates that variable items will be sorted.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(false)]
        [Description("Gets or sets a value which indicates that variable items will be sorted.")]
        public bool ParametersPanelSortDataItems { get; set; } = false;

        private string parametersPanelDateFormat = StiDateFormatMode.FromClient;
        /// <summary>
        /// Gets or sets a date format for datetime parameters in parameters panel. To use a server date format please set the StiDateFormatMode.FromServer or "FromServer" string value.
        /// The default is the client browser date format.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue("")]
        [Description("Gets or sets a date format for datetime parameters in parameters panel. To use a server date format please set the StiDateFormatMode.FromServer or \"FromServer\" string value. The default is the client browser date format.")]
        public string ParametersPanelDateFormat
        {
            get
            {
                if (parametersPanelDateFormat == StiDateFormatMode.FromServer)
                    return CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern + " " + CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern;
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
        [Category("Appearance")]
        [DefaultValue(StiInterfaceType.Auto)]
        [Description("Gets or sets the type of the viewer interface.")]
        public StiInterfaceType InterfaceType { get; set; } = StiInterfaceType.Auto;

        /// <summary>
        /// Gets or sets a value which indicates that allows mobile mode of the viewer interface.
        /// </summary>
        [DefaultValue(true)]
        [Category("Appearance")]
        [Description("Gets or sets a value which indicates that allows mobile mode of the viewer interface.")]
        public bool AllowMobileMode { get; set; } = true;

        /// <summary>
        /// Gets or sets the type of the chart in the viewer.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(StiChartRenderType.AnimatedVector)]
        [Description("Gets or sets the type of the chart in the viewer.")]
        public StiChartRenderType ChartRenderType { get; set; } = StiChartRenderType.AnimatedVector;

        /// <summary>
        /// Gets or sets a method how the viewer will show a report.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(StiReportDisplayMode.FromReport)]
        [Description("Gets or sets a method how the viewer will show a report.")]
        public StiReportDisplayMode ReportDisplayMode { get; set; } = StiReportDisplayMode.FromReport;

        /// <summary>
        /// Gets or sets the first day of week in the date picker.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(StiFirstDayOfWeek.Auto)]
        [Description("Gets or sets the first day of week in the date picker.")]
        public StiFirstDayOfWeek DatePickerFirstDayOfWeek { get; set; } = StiFirstDayOfWeek.Auto;

        /// <summary>
        /// "Gets or sets a value, which indicates that the current day will be included in the ranges of the date picker.".
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(false)]
        [Description("Gets or sets a value, which indicates that the current day will be included in the ranges of the date picker.")]
        public bool DatePickerIncludeCurrentDayForRanges { get; set; } = false;

        /// <summary>
        /// Gets or sets a value which allows touch zoom in the viewer.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which allows touch zoom in the viewer.")]
        public bool AllowTouchZoom { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that 'The report is not specified' message will be shown.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that 'The report is not specified' message will be shown.")]
        public bool ShowReportIsNotSpecifiedMessage { get; set; } = true;

        /// <summary>
        /// Gets or sets the image quality that will be used on the viewer page.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(StiImagesQuality.Normal)]
        [Description("Gets or sets the image quality that will be used on the viewer page.")]
        public StiImagesQuality ImagesQuality { get; set; } = StiImagesQuality.Normal;

        /// <summary>
        /// Gets or sets the PDF print mode - hidden or in a pop-up window.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(StiPrintToPdfMode.Hidden)]
        [Description("Gets or sets the PDF print mode - hidden or in a pop-up window.")]
        public StiPrintToPdfMode PrintToPdfMode { get; set; }

        /// <summary>
        /// Gets or sets a value which indicates that if a report contains several pages, then they will be combined in preview.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that if a report contains several pages, then they will be combined in preview.")]
        public bool CombineReportPages { get; set; } = false;

        /// <summary>
        /// Gets or sets a value which indicates images size of the save menu in the viewer.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates images size of the save menu in the viewer.")]
        public StiImageSize SaveMenuImageSize { get; set; } = StiImageSize.Small;
        #endregion

        #region Email
        /// <summary>
        /// Gets or sets a value which allows to display the Email dialog, or send Email with the default settings.
        /// </summary>
        [Category("Email")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which allows to display the Email dialog, or send Email with the default settings.")]
        public bool ShowEmailDialog { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which allows to display the export dialog for Email, or export report for Email with the default settings.
        /// </summary>
        [Category("Email")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which allows to display the export dialog for Email, or export report for Email with the default settings.")]
        public bool ShowEmailExportDialog { get; set; } = true;

        /// <summary>
        /// Gets or sets the default email address of the message created in the viewer.
        /// </summary>
        [Category("Email")]
        [DefaultValue("")]
        [Description("Gets or sets the default email address of the message created in the viewer.")]
        public string DefaultEmailAddress { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the default subject of the message created in the viewer.
        /// </summary>
        [Category("Email")]
        [DefaultValue("")]
        [Description("Gets or sets the default subject of the message created in the viewer.")]
        public string DefaultEmailSubject { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the default text of the message created in the viewer.
        /// </summary>
        [Category("Email")]
        [DefaultValue("")]
        [Description("Gets or sets the default text of the message created in the viewer.")]
        public string DefaultEmailMessage { get; set; } = string.Empty;
        #endregion

        #region Exports
        /// <summary>
        /// A class which controls default settings of exports.
        /// </summary>
        [Browsable(false)]
        [Description("A class which controls default settings of exports.")]
        public StiDefaultExportSettings DefaultExportSettings { get; set; } = new StiDefaultExportSettings();

        /// <summary>
        /// Gets or sets a value which allows store the export settings in the cookies.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which allows store the export settings in the cookies.")]
        public bool StoreExportSettings { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which allows to display the export dialog, or to export with the default settings.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which allows to display the export dialog, or to export with the default settings.")]
        public bool ShowExportDialog { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the viewer to the report document file.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(false)]
        [Description("Gets or sets a value which indicates that the user can save the report from the viewer to the report document file.")]
        public bool ShowExportToDocument { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the viewer to the PDF format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the viewer to the PDF format.")]
        public bool ShowExportToPdf { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the viewer to the XPS format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(false)]
        [Description("Gets or sets a value which indicates that the user can save the report from the viewer to the XPS format.")]
        public bool ShowExportToXps { get; set; } = false;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the viewer to the Power Point 2007-2010 format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the viewer to the Power Point 2007-2010 format.")]
        public bool ShowExportToPowerPoint { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the viewer to the HTML format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the viewer to the HTML format.")]
        public bool ShowExportToHtml { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the viewer to the HTML5 format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the viewer to the HTML5 format.")]
        public bool ShowExportToHtml5 { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the viewer to the MHT (Web Archive) format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the viewer to the MHT (Web Archive) format.")]
        public bool ShowExportToMht { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the viewer to the TEXT format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the viewer to the TEXT format.")]
        public bool ShowExportToText { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the viewer to the Rich Text format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the viewer to the Rich Text format.")]
        public bool ShowExportToRtf { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the viewer to the Word 2007-2010 format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the viewer to the Word 2007-2010 format.")]
        public bool ShowExportToWord2007 { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the viewer to the Open Document Text format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the viewer to the Open Document Text format.")]
        public bool ShowExportToOpenDocumentWriter { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the viewer to the Excel BIFF format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the viewer to the Excel BIFF format.")]
        public bool ShowExportToExcel { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the viewer to the Excel XML format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the viewer to the Excel XML format.")]
        public bool ShowExportToExcelXml { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the viewer to the Excel 2007-2010 format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the viewer to the Excel 2007-2010 format.")]
        public bool ShowExportToExcel2007 { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the viewer to the Open Document Calc format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the viewer to the Open Document Calc format.")]
        public bool ShowExportToOpenDocumentCalc { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the viewer to the CSV format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the viewer to the CSV format.")]
        public bool ShowExportToCsv { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the viewer to the DBF format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the viewer to the DBF format.")]
        public bool ShowExportToDbf { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the viewer to the XML format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the viewer to the XML format.")]
        public bool ShowExportToXml { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the viewer to the DIF format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the viewer to the DIF format.")]
        public bool ShowExportToDif { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the viewer to the Sylk format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the viewer to the Sylk format.")]
        public bool ShowExportToSylk { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the viewer to the Json format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the viewer to the Json format.")]
        public bool ShowExportToJson { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the viewer to the BMP image format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the viewer to the BMP image format.")]
        public bool ShowExportToImageBmp { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the viewer to the GIF image format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the viewer to the GIF image format.")]
        public bool ShowExportToImageGif { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the viewer to the JPEG image format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the viewer to the JPEG image format.")]
        public bool ShowExportToImageJpeg { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the viewer to the PCX image format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the viewer to the PCX image format.")]
        public bool ShowExportToImagePcx { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the viewer to the PNG image format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the viewer to the PNG image format.")]
        public bool ShowExportToImagePng { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the viewer to the TIFF image format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the viewer to the TIFF image format.")]
        public bool ShowExportToImageTiff { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the viewer to the Metafile image format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the viewer to the Metafile image format.")]
        public bool ShowExportToImageMetafile { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the viewer to the SVG image format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the viewer to the SVG image format.")]
        public bool ShowExportToImageSvg { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the viewer to the SVGZ image format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the viewer to the SVGZ image format.")]
        public bool ShowExportToImageSvgz { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the viewer to the report document file.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which allows to display the option Open After Export.")]
        public bool ShowOpenAfterExport { get; set; } = true;

        /// <summary>
        /// Gets or sets a value the option Open After Export.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value the option Open After Export.")]
        public bool OpenAfterExport { get; set; } = true;
        #endregion

        #region Internal
        private static bool IsDesignMode
        {
            get
            {
                return HttpContext.Current == null;
            }
        }

        private StiRequestParams requestParams = null;
        private StiRequestParams RequestParams
        {
            get
            {
                if (requestParams == null) requestParams = GetRequestParams();
                return requestParams;
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

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        internal bool ReportDesignerMode { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        internal bool CloudMode { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        internal bool ServerMode { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override ClientIDMode ClientIDMode
        {
            get
            {
                return base.ClientIDMode;
            }
            set
            {
                base.ClientIDMode = value;
            }
        }
        #endregion

        #region Report
        private StiReport report = null;
        /// <summary>
        /// Gets or sets a report object which is shown in the viewer.
        /// </summary>
        [Browsable(false)]
        [Description("Gets or sets a report object which is shown in the viewer.")]
        public StiReport Report
        {
            get
            {
                if (report == null) report = GetReportObject(this.RequestParams);
                return report;
            }
            set
            {
                report = value;
                if (RequestParams.ComponentType != StiComponentType.Viewer) requestParams = CreateRequestParams();
                InvokeGetReportData();
                StiReportHelper.ApplyQueryParameters(RequestParams, report);
                RequestParams.Cache.Helper.SaveReportInternal(RequestParams, report);
            }
        }
        #endregion

        #region Server
        private static StiCacheHelper cacheHelper = null;
        /// <summary>
        /// Gets or sets an instance of the StiCacheHelper class that will be used for report caching on the server side.
        /// </summary>
        [Browsable(false)]
        [Description("Gets or sets an instance of the StiCacheHelper class that will be used for report caching on the server side.")]
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

        private int requestTimeout = 30;
        /// <summary>
        /// Gets or sets time which indicates how many seconds the client side will wait for the response from the server side. The default value is 30 seconds.
        /// </summary>
        [Category("Server")]
        [DefaultValue(30)]
        [Description("Gets or sets time which indicates how many seconds the client side will wait for the response from the server side. The default value is 30 seconds.")]
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
        /// Gets or sets time which indicates how many minutes the report will be stored in the server cache or session. The default value is 10 minutes.
        /// </summary>
        [Category("Server")]
        [DefaultValue(10)]
        [Description("Gets or sets time which indicates how many minutes the report will be stored in the server cache or session. The default value is 10 minutes.")]
        public int CacheTimeout { get; set; } = 10;

        /// <summary>
        /// Gets or sets the mode of the report caching.
        /// </summary>
        [Category("Server")]
        [DefaultValue(StiServerCacheMode.ObjectCache)]
        [Description("Gets or sets the mode of the report caching.")]
        public StiServerCacheMode CacheMode { get; set; } = StiServerCacheMode.ObjectCache;

        /// <summary>
        /// Specifies the relative priority of report, stored in the system cache.
        /// </summary>
        [Category("Server")]
        [DefaultValue(CacheItemPriority.Default)]
        [Description("Specifies the relative priority of report, stored in the system cache.")]
        public CacheItemPriority CacheItemPriority { get; set; } = CacheItemPriority.Default;

        /// <summary>
        /// Allows the viewer to update the cache automatically. The cache will be updated about once every three minutes if there are no actions in the viewer.
        /// </summary>
        [Category("Server")]
        [DefaultValue(true)]
        [Description("Allows the viewer to update the cache automatically. The cache will be updated about once every three minutes if there are no actions in the viewer.")]
        public bool AllowAutoUpdateCache { get; set; } = true;

        /// <summary>
        /// "Allows loading custom fonts to the client side.
        /// </summary>
        [Category("Server")]
        [DefaultValue(false)]
        [Description("Allows loading custom fonts to the client side.")]
        public bool AllowLoadingCustomFontsToClientSide { get; set; }

        /// <summary>
        /// Gets or sets a value which indicates that the viewer will use relative or absolute URLs.
        /// </summary>
        [Category("Server")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the viewer will use relative or absolute URLs.")]
        public bool UseRelativeUrls { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which specifies the port number to use in the URL. A value of 0 defines automatic detection. A value of -1 removes the port number.
        /// </summary>
        [Category("Server")]
        [DefaultValue(0)]
        [Description("Gets or sets a value which specifies the port number to use in the URL. A value of 0 defines automatic detection. A value of -1 removes the port number.")]
        public int PortNumber { get; set; } = 0;

        /// <summary>
        /// Gets or sets a value which enables or disables the transfer of URL parameters when requesting the scripts and styles of the viewer.
        /// </summary>
        [Category("Server")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which enables or disables the transfer of URL parameters when requesting the scripts and styles of the viewer.")]
        public bool PassQueryParametersForResources { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which enables or disables the use of URL parameters as report parameters.
        /// </summary>
        [Category("Server")]
        [DefaultValue(false)]
        [Description("Gets or sets a value which enables or disables the use of URL parameters as report parameters.")]
        public bool PassQueryParametersToReport { get; set; }

        /// <summary>
        /// Gets or sets a value which enables or disables the transfer POST parameters of the form.
        /// </summary>
        [Category("Server")]
        [DefaultValue(false)]
        [Description("Gets or sets a value which enables or disables the transfer POST parameters of the form.")]
        public bool PassFormValues { get; set; }

        /// <summary>
        /// Gets or sets a value which enables or disables the display of the detailed server error in the viewer.
        /// </summary>
        [Category("Server")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which enables or disables the display of the detailed server error in the viewer.")]
        public bool ShowServerErrorPage { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which enables or disables the GZip compression of requests to the server. This allows to reduce the volume of Internet traffic, but slightly slows down the viewer actions.
        /// </summary>
        [Category("Server")]
        [DefaultValue(false)]
        [Description("Gets or sets a value which enables or disables the GZip compression of requests to the server. This allows to reduce the volume of Internet traffic, but slightly slows down the viewer actions.")]
        public bool UseCompression { get; set; }

        /// <summary>
        /// Gets or sets a value which enables caching for viewer scripts and styles.
        /// </summary>
        [Category("Server")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which enables caching for viewer scripts and styles.")]
        public bool UseCacheForResources { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which enables the use of a different cache depending on the selected localization.
        /// </summary>
        [Category("Server")]
        [DefaultValue(false)]
        [Description("Gets or sets a value which enables the use of a different cache depending on the selected localization.")]
        public bool UseLocalizedCache { get; set; }

        /// <summary>
        /// Allows the viewer to update the cookies automatically on every request to the server. By default, cookies are set when creating the viewer, if they are not specified in the report.
        /// </summary>
        [Category("Server")]
        [DefaultValue(false)]
        [Description("Allows the viewer to update the cookies automatically on every request to the server. By default, cookies are set when creating the viewer, if they are not specified in the report.")]
        public bool AllowAutoUpdateCookies { get; set; }
        #endregion

        #region Toolbar
        /// <summary>
        /// Gets or sets a value which indicates that toolbar will be shown in the viewer.
        /// </summary>
        [Category("Toolbar")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that toolbar will be shown in the viewer.")]
        public bool ShowToolbar { get; set; } = true;

        /// <summary>
        /// Gets or sets the display mode of the toolbar - simple or separated into upper and lower parts.
        /// </summary>
        [Category("Toolbar")]
        [DefaultValue(StiToolbarDisplayMode.Simple)]
        [Description("Gets or sets the display mode of the toolbar - simple or separated into upper and lower parts.")]
        public StiToolbarDisplayMode ToolbarDisplayMode { get; set; } = StiToolbarDisplayMode.Simple;

        /// <summary>
        /// Gets or sets a color of the toolbar background. The default value is the theme color.
        /// </summary>
        [Category("Toolbar")]
        [DefaultValue(typeof(Color), "Empty")]
        [Description("Gets or sets a color of the toolbar background. The default value is the theme color.")]
        [TypeConverter(typeof(WebColorConverter))]
        public Color ToolbarBackgroundColor { get; set; } = Color.Empty;

        /// <summary>
        /// Gets or sets a color of the toolbar border. The default value is the theme color.
        /// </summary>
        [DefaultValue(typeof(Color), "Empty")]
        [Category("Toolbar")]
        [Description("Gets or sets a color of the toolbar border. The default value is the theme color.")]
        [TypeConverter(typeof(WebColorConverter))]
        public Color ToolbarBorderColor { get; set; } = Color.Empty;

        /// <summary>
        /// Gets or sets a color of the toolbar texts.
        /// </summary>
        [DefaultValue(typeof(Color), "Empty")]
        [Category("Toolbar")]
        [Description("Gets or sets a color of the toolbar texts.")]
        [TypeConverter(typeof(WebColorConverter))]
        public Color ToolbarFontColor { get; set; } = Color.Empty;

        /// <summary>
        /// Gets or sets a value which indicates which font family will be used for drawing texts in the viewer.
        /// </summary>
        [Category("Toolbar")]
        [DefaultValue("Arial")]
        [Description("Gets or sets a value which indicates which font family will be used for drawing texts in the viewer.")]
        public string ToolbarFontFamily { get; set; } = "Arial";

        /// <summary>
        /// Gets or sets the alignment of the viewer toolbar.
        /// </summary>
        [Category("Toolbar")]
        [DefaultValue(StiContentAlignment.Default)]
        [Description("Gets or sets the alignment of the viewer toolbar.")]
        public StiContentAlignment ToolbarAlignment { get; set; } = StiContentAlignment.Default;

        /// <summary>
        /// Gets or sets a value which allows displaying or hiding toolbar buttons captions.
        /// </summary>
        [Category("Toolbar")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which allows displaying or hiding toolbar buttons captions.")]
        public bool ShowButtonCaptions { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the Print button in the toolbar of the viewer.
        /// </summary>
        [Category("Toolbar")]
        [DefaultValue(true)]
        [Description("Gets or sets a visibility of the Print button in the toolbar of the viewer.")]
        public bool ShowPrintButton { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the Open button in the toolbar of the viewer.
        /// </summary>
        [Category("Toolbar")]
        [DefaultValue(true)]
        [Description("Gets or sets a visibility of the Open button in the toolbar of the viewer.")]
        public bool ShowOpenButton { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the Save button in the toolbar of the viewer.
        /// </summary>
        [Category("Toolbar")]
        [DefaultValue(true)]
        [Description("Gets or sets a visibility of the Save button in the toolbar of the viewer.")]
        public bool ShowSaveButton { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the Send Email button in the toolbar of the viewer.
        /// </summary>
        [Category("Toolbar")]
        [DefaultValue(false)]
        [Description("Gets or sets a visibility of the Send Email button in the toolbar of the viewer.")]
        public bool ShowSendEmailButton { get; set; }

        /// <summary>
        /// Gets or sets a visibility of the Find button in the toolbar of the viewer.
        /// </summary>
        [Category("Toolbar")]
        [DefaultValue(true)]
        [Description("Gets or sets a visibility of the Find button in the toolbar of the viewer.")]
        public bool ShowFindButton { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the Signature button in the toolbar of the viewer.
        /// </summary>
        [Category("Toolbar")]
        [DefaultValue(true)]
        [Description("Gets or sets a visibility of the Signature button in the toolbar of the viewer.")]
        public bool ShowSignatureButton { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the Bookmarks button in the toolbar of the viewer.
        /// </summary>
        [Category("Toolbar")]
        [DefaultValue(true)]
        [Description("Gets or sets a visibility of the Bookmarks button in the toolbar of the viewer.")]
        public bool ShowBookmarksButton { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the Parameters button in the toolbar of the viewer.
        /// </summary>
        [Category("Toolbar")]
        [DefaultValue(true)]
        [Description("Gets or sets a visibility of the Parameters button in the toolbar of the viewer.")]
        public bool ShowParametersButton { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the Resources button in the toolbar of the viewer.
        /// </summary>
        [Category("Toolbar")]
        [DefaultValue(true)]
        [Description("Gets or sets a visibility of the Resources button in the toolbar of the viewer.")]
        public bool ShowResourcesButton { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the Editor button in the toolbar of the viewer.
        /// </summary>
        [Category("Toolbar")]
        [DefaultValue(true)]
        [Description("Gets or sets a visibility of the Editor button in the toolbar of the viewer.")]
        public bool ShowEditorButton { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the Full Screen button in the toolbar of the viewer.
        /// </summary>
        [Category("Toolbar")]
        [DefaultValue(true)]
        [Description("Gets or sets a visibility of the Full Screen button in the toolbar of the viewer.")]
        public bool ShowFullScreenButton { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the First Page button in the toolbar of the viewer.
        /// </summary>
        [Category("Toolbar")]
        [DefaultValue(true)]
        [Description("Gets or sets a visibility of the First Page button in the toolbar of the viewer.")]
        public bool ShowFirstPageButton { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the Prev Page button in the toolbar of the viewer.
        /// </summary>
        [Category("Toolbar")]
        [DefaultValue(true)]
        [Description("Gets or sets a visibility of the Prev Page button in the toolbar of the viewer.")]
        public bool ShowPreviousPageButton { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the current page control in the toolbar of the viewer.
        /// </summary>
        [Category("Toolbar")]
        [DefaultValue(true)]
        [Description("Gets or sets a visibility of the current page control in the toolbar of the viewer.")]
        public bool ShowCurrentPageControl { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the Next Page button in the toolbar of the viewer.
        /// </summary>
        [Category("Toolbar")]
        [DefaultValue(true)]
        [Description("Gets or sets a visibility of the Next Page button in the toolbar of the viewer.")]
        public bool ShowNextPageButton { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the Last Page button in the toolbar of the viewer.
        /// </summary>
        [Category("Toolbar")]
        [DefaultValue(true)]
        [Description("Gets or sets a visibility of the Last Page button in the toolbar of the viewer.")]
        public bool ShowLastPageButton { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the Zoom control in the toolbar of the viewer.
        /// </summary>
        [Category("Toolbar")]
        [DefaultValue(true)]
        [Description("Gets or sets a visibility of the Zoom control in the toolbar of the viewer.")]
        public bool ShowZoomButton { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the View Mode button in the toolbar of the viewer.
        /// </summary>
        [Category("Toolbar")]
        [DefaultValue(true)]
        [Description("Gets or sets a visibility of the View Mode button in the toolbar of the viewer.")]
        public bool ShowViewModeButton { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the Design button in the toolbar of the viewer.
        /// </summary>
        [Category("Toolbar")]
        [DefaultValue(false)]
        [Description("Gets or sets a visibility of the Design button in the toolbar of the viewer.")]
        public bool ShowDesignButton { get; set; }

        /// <summary>
        /// Gets or sets a visibility of the About button in the toolbar of the viewer.
        /// </summary>
        [Category("Toolbar")]
        [DefaultValue(true)]
        [Description("Gets or sets a visibility of the About button in the toolbar of the viewer.")]
        public bool ShowAboutButton { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the Pin button in the toolbar of the viewer in mobile mode.
        /// </summary>
        [Category("Toolbar")]
        [DefaultValue(true)]
        [Description("Gets or sets a visibility of the Pin button in the toolbar of the viewer in mobile mode.")]
        public bool ShowPinToolbarButton { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the Refresh button in the toolbar of the viewer.
        /// </summary>
        [Category("Toolbar")]
        [DefaultValue(true)]
        [Description("Gets or sets a visibility of the Refresh button in the toolbar of the viewer.")]
        public bool ShowRefreshButton { get; set; } = true;

        /// <summary>
        /// Gets or sets the default mode of the report print destination.
        /// </summary>
        [Category("Toolbar")]
        [DefaultValue(StiPrintDestination.Default)]
        [Description("Gets or sets the default mode of the report print destination.")]
        public StiPrintDestination PrintDestination { get; set; } = StiPrintDestination.Default;

        /// <summary>
        /// Gets or sets the mode of showing a report in the viewer - Single Page, Continuous or Multiple Pages.
        /// </summary>
        [Category("Toolbar")]
        [DefaultValue(StiWebViewMode.SinglePage)]
        [Description("Gets or sets the mode of showing a report in the viewer - Single Page, Continuous or Multiple Pages.")]
        public StiWebViewMode ViewMode { get; set; } = StiWebViewMode.SinglePage;

        private int zoom = 100;
        /// <summary>
        /// Gets or sets the report showing zoom. The default value is 100.
        /// </summary>
        [Category("Toolbar")]
        [DefaultValue(100)]
        [Description("Gets or sets the report showing zoom. The default value is 100.")]
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
        [Category("Toolbar")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that menu animation is enabled.")]
        public bool MenuAnimation { get; set; } = true;

        /// <summary>
        /// Gets or sets the mode that shows menu of the viewer.
        /// </summary>
        [Category("Toolbar")]
        [DefaultValue(StiShowMenuMode.Click)]
        [Description("Gets or sets the mode that shows menu of the viewer.")]
        public StiShowMenuMode ShowMenuMode { get; set; } = StiShowMenuMode.Click;

        /// <summary>
        /// Gets or sets a value which allows automatically hide the viewer toolbar in mobile mode.
        /// </summary>
        [Category("Toolbar")]
        [DefaultValue(false)]
        [Description("Gets or sets a value which allows automatically hide the viewer toolbar in mobile mode.")]
        public bool AutoHideToolbar { get; set; }        
        #endregion
    }
}
