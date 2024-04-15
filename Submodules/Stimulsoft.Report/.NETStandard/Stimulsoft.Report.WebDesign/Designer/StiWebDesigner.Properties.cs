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
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Caching;
using System.Drawing.Imaging;
using Stimulsoft.Base;
using System.Drawing;
using System.Globalization;

namespace Stimulsoft.Report.Web
{
    public partial class StiWebDesigner :
        WebControl,
        INamingContainer
    {
        #region Appearance
        
        /// <summary>
        /// Gets or sets the current visual theme which is used for drawing visual elements of the designer.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(StiDesignerTheme.Office2022WhiteBlue)]
        [Description("Gets or sets the current visual theme which is used for drawing visual elements of the designer.")]
        public StiDesignerTheme Theme { get; set; } = StiDesignerTheme.Office2022WhiteBlue;
        
        /// <summary>
        /// Gets or sets a path to the custom css file for the designer.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue("")]
        [Description("Gets or sets a path to the custom css file for the designer.")]
        public string CustomCss { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a path to the localization file for the designer.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue("")]
        [Description("Gets or sets a path to the localization file for the designer.")]
        public string Localization { get; set; } = string.Empty;

        private string localizationDirectory = string.Empty;
        /// <summary>
        /// Gets or sets a path to the localization files. These localizations are displayed on the toolbar of the designer.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue("")]
        [Description("Gets or sets a path to the localization files. These localizations are displayed on the toolbar of the designer.")]
        public string LocalizationDirectory
        {
            get
            {
                return localizationDirectory;
            }
            set
            {
                if (string.IsNullOrEmpty(value)) localizationDirectory = string.Empty;
                else localizationDirectory = value;
            }
        }

        /// <summary>
        /// Gets or sets a default value of the report unit in the designer.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(StiReportUnitType.Centimeters)]
        [Description("Gets or sets a default value of the report unit in the designer.")]
        public StiReportUnitType DefaultUnit { get; set; } = StiReportUnitType.Centimeters;
        
        /// <summary>
        /// Gets or sets the report showing zoom. The default value is 100.
        /// </summary>
        private int zoom = 100;
        [Category("Appearance")]
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
                if (value == -1 || value == -2) zoom = value;
                else if (value > 200) zoom = 200;
                else if (value < 10) zoom = 10;
                else zoom = value;
            }
        }

        /// <summary>
        /// Gets or sets a value which indicates that animation of the user interface is enabled.
        /// </summary>
        [DefaultValue(true)]
        [Category("Appearance")]
        [Description("Gets or sets a value which indicates that animation of the user interface is enabled.")]
        public bool ShowAnimation { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that show or hide tooltips.
        /// </summary>
        [DefaultValue(true)]
        [Category("Appearance")]
        [Description("Gets or sets a value which indicates that show or hide tooltips.")]
        public bool ShowTooltips { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that show or hide the help link in tooltips.
        /// </summary>
        [DefaultValue(true)]
        [Category("Appearance")]
        [Description("Gets or sets a value which indicates that show or hide the help link in tooltips.")]
        public bool ShowTooltipsHelp { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that show or hide the help button in dialogs.
        /// </summary>
        [DefaultValue(true)]
        [Category("Appearance")]
        [Description("Gets or sets a value which indicates that show or hide the help button in dialogs.")]
        public bool ShowDialogsHelp { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which allows to display the open dialog, or to open with the open event.
        /// </summary>
        [DefaultValue(true)]
        [Category("Appearance")]
        [Description("Gets or sets a value which allows to display the open dialog, or to open with the open event.")]
        public bool ShowOpenDialog { get; set; } = true;

        /// <summary>
        /// Gets or sets the interface type of the designer.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(StiInterfaceType.Auto)]
        [Description("Gets or sets the interface type of the designer.")]
        public StiInterfaceType InterfaceType { get; set; } = StiInterfaceType.Auto;

        /// <summary>
        /// Gets or sets the type of the chart in the preview.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(StiChartRenderType.AnimatedVector)]
        [Description("Gets or sets the type of the chart in the viewer.")]
        public StiChartRenderType ChartRenderType { get; set; } = StiChartRenderType.AnimatedVector;

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

        private string parametersPanelDateFormat = StiDateFormatMode.FromClient;
        /// <summary>
        /// Gets or sets a date format for datetime parameters in parameters panel. To use a server date format please set the StiDateFormatMode.FromServer or "FromServer" string value.
        /// The default is the client browser date format.
        /// </summary>
        [DefaultValue("")]
        [Category("Appearance")]
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
        /// Gets or sets a value which indicates that variable items will be sorted.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(false)]
        [Description("Gets or sets a value which indicates that variable items will be sorted.")]
        public bool ParametersPanelSortDataItems { get; set; } = false;

        /// <summary>
        /// Gets or sets a visibility of the report tree in the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("Appearance")]
        [Description("Gets or sets a visibility of the report tree in the designer.")]
        public bool ShowReportTree { get; set; } = true;

        /// <summary>
        /// Gets or sets a method how will show a report in the preview mode.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(StiReportDisplayMode.FromReport)]
        [Description("Gets or sets a method how will show a report in the preview mode.")]
        public StiReportDisplayMode ReportDisplayMode { get; set; } = StiReportDisplayMode.FromReport;

        /// <summary>
        /// Gets or sets a value which indicates that the designer will be closed without asking.
        /// </summary>
        [DefaultValue(false)]
        [Category("Appearance")]
        [Description("Gets or sets a value which indicates that the designer will be closed without asking.")]
        public bool CloseDesignerWithoutAsking { get; set; } = false;

        /// <summary>
        /// Gets or sets a visibility of the report tree in the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("Appearance")]
        [Description("Gets or sets a visibility of the system fonts in the fonts list.")]
        public bool ShowSystemFonts { get; set; } = true;

        /// <summary>
        /// Gets or sets a value of the wizard type which should be run after designer starts.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(StiWizardType.None)]
        [Description("Gets or sets a value of the wizard type which should be run after designer starts.")]
        public StiWizardType WizardTypeRunningAfterLoad { get; set; } = StiWizardType.None;

        /// <summary>
        /// Gets or sets a value which indicates that allows word wrap in the text editors.
        /// </summary>
        [DefaultValue(true)]
        [Category("Appearance")]
        [Description("Gets or sets a value which indicates that allows word wrap in the text editors.")]
        public bool AllowWordWrapTextEditors { get; set; } = true;
        #endregion

        #region Pages
        /// <summary>
        /// Gets or sets a visibility of the new page button in the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("Pages")]
        [Description("Gets or sets a visibility of the new page button in the designer.")]
        public bool ShowNewPageButton { get; set; } = true;
        #endregion

        #region Bands
        /// <summary>
        /// Gets or sets a visibility of the ReportTitleBand item in the Bands menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("Bands")]
        [Description("Gets or sets a visibility of the ReportTitleBand item in the Bands menu of the designer.")]
        public bool ShowReportTitleBand { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the ReportSummaryBand item in the Bands menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("Bands")]
        [Description("Gets or sets a visibility of the ReportSummaryBand item in the Bands menu of the designer.")]
        public bool ShowReportSummaryBand { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the PageHeaderBand item in the Bands menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("Bands")]
        [Description("Gets or sets a visibility of the PageHeaderBand item in the Bands menu of the designer.")]
        public bool ShowPageHeaderBand { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the PageFooterBand item in the Bands menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("Bands")]
        [Description("Gets or sets a visibility of the PageFooterBand item in the Bands menu of the designer.")]
        public bool ShowPageFooterBand { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the GroupHeaderBand item in the Bands menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("Bands")]
        [Description("Gets or sets a visibility of the GroupHeaderBand item in the Bands menu of the designer.")]
        public bool ShowGroupHeaderBand { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the GroupFooterBand item in the Bands menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("Bands")]
        [Description("Gets or sets a visibility of the GroupFooterBand item in the Bands menu of the designer.")]
        public bool ShowGroupFooterBand { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the HeaderBand item in the Bands menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("Bands")]
        [Description("Gets or sets a visibility of the HeaderBand item in the Bands menu of the designer.")]
        public bool ShowHeaderBand { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the FooterBand item in the Bands menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("Bands")]
        [Description("Gets or sets a visibility of the FooterBand item in the Bands menu of the designer.")]
        public bool ShowFooterBand { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the ColumnHeaderBand item in the Bands menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("Bands")]
        [Description("Gets or sets a visibility of the ColumnHeaderBand item in the Bands menu of the designer.")]
        public bool ShowColumnHeaderBand { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the ColumnFooterBand item in the Bands menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("Bands")]
        [Description("Gets or sets a visibility of the ColumnFooterBand item in the Bands menu of the designer.")]
        public bool ShowColumnFooterBand { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the DataBand item in the Bands menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("Bands")]
        [Description("Gets or sets a visibility of the DataBand item in the Bands menu of the designer.")]
        public bool ShowDataBand { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the HierarchicalBand item in the Bands menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("Bands")]
        [Description("Gets or sets a visibility of the HierarchicalBand item in the Bands menu of the designer.")]
        public bool ShowHierarchicalBand { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the ChildBand item in the Bands menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("Bands")]
        [Description("Gets or sets a visibility of the ChildBand item in the Bands menu of the designer.")]
        public bool ShowChildBand { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the EmptyBand item in the Bands menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("Bands")]
        [Description("Gets or sets a visibility of the EmptyBand item in the Bands menu of the designer.")]
        public bool ShowEmptyBand { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the OverlayBand item in the Bands menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("Bands")]
        [Description("Gets or sets a visibility of the OverlayBand item in the Bands menu of the designer.")]
        public bool ShowOverlayBand { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the TableOfContents item in the Bands menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("Bands")]
        [Description("Gets or sets a visibility of the TableOfContents item in the Bands menu of the designer.")]
        public bool ShowTableOfContents { get; set; } = true;
        #endregion

        #region Behaviour

        private int undoMaxLevel = 6;
        /// <summary>
        /// Gets or sets a maximum level of undo actions with the report. A large number of actions consume more memory on the server side.
        /// </summary>
        [Category("Behaviour")]
        [DefaultValue(6)]
        [Description("Gets or sets a maximum level of undo actions with the report. A large number of actions consume more memory on the server side.")]
        public int UndoMaxLevel
        {
            get
            {
                return undoMaxLevel;
            }
            set
            {
                undoMaxLevel = Math.Max(1, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that displays or hides the report file name dialog at saving.
        /// </summary>
        [DefaultValue(true)]
        [Category("Behaviour")]
        [Description("Gets or sets the value that displays or hides the report file name dialog at saving.")]
        public bool ShowSaveDialog { get; set; } = true;

        /// <summary>
        /// Allow the designer to change the window title.
        /// </summary>
        [DefaultValue(true)]
        [Category("Behaviour")]
        [Description("Allow the designer to change the window title.")]
        public bool AllowChangeWindowTitle { get; set; } = true;

        /// <summary>
        /// Gets or sets the save report mode - Hidden (AJAX mode), Visible (POST mode) or New Window.
        /// </summary>
        [Category("Behaviour")]
        [DefaultValue(StiSaveMode.Hidden)]
        [Description("Gets or sets the save report mode - Hidden (AJAX mode), Visible (POST mode) or New Window.")]
        public StiSaveMode SaveReportMode { get; set; } = StiSaveMode.Hidden;

        /// <summary>
        /// Gets or sets the save report mode - Hidden (AJAX mode), Visible (POST mode) or New Window.
        /// </summary>
        [Category("Behaviour")]
        [DefaultValue(StiSaveMode.Hidden)]
        [Description("Gets or sets the save report mode - Hidden (AJAX mode), Visible (POST mode) or New Window.")]
        public StiSaveMode SaveReportAsMode { get; set; } = StiSaveMode.Hidden;

        /// <summary>
        /// Gets or sets the value that allows running the report checker before preview.
        /// </summary>
        [DefaultValue(true)]
        [Category("Behaviour")]
        [Description("Gets or sets the value that allows running the report checker before preview.")]
        public bool CheckReportBeforePreview { get; set; } = true;

        #endregion

        #region Components

        /// <summary>
        /// Gets or sets a visibility of the Text item in the Components menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("Components")]
        [Description("Gets or sets a visibility of the Text item in the Components menu of the designer.")]
        public bool ShowText { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the TextInCells item in the Components menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("Components")]
        [Description("Gets or sets a visibility of the TextInCells item in the Components menu of the designer.")]
        public bool ShowTextInCells { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the RichText item in the Components menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("Components")]
        [Description("Gets or sets a visibility of the RichText item in the Components menu of the designer.")]
        public bool ShowRichText { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the Image item in the Components menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("Components")]
        [Description("Gets or sets a visibility of the Image item in the Components menu of the designer.")]
        public bool ShowImage { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the BarCode item in the Components menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("Components")]
        [Description("Gets or sets a visibility of the BarCode item in the Components menu of the designer.")]
        public bool ShowBarCode { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the Shape item in the Components menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("Components")]
        [Description("Gets or sets a visibility of the Shape item in the Components menu of the designer.")]
        public bool ShowShape { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the Horizontal Line Primitive item in the Components menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("Components")]
        [Description("Gets or sets a visibility of the Horizontal Line Primitive item in the Components menu of the designer.")]
        public bool ShowHorizontalLinePrimitive { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the Vertical Line Primitive item in the Components menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("Components")]
        [Description("Gets or sets a visibility of the Vertical Line Primitive item in the Components menu of the designer.")]
        public bool ShowVerticalLinePrimitive { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the Rectangle Primitive item in the Components menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("Components")]
        [Description("Gets or sets a visibility of the Rectangle Primitive item in the Components menu of the designer.")]
        public bool ShowRectanglePrimitive { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the Rounded Rectangle Primitive item in the Components menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("Components")]
        [Description("Gets or sets a visibility of the Rounded Rectangle Primitive item in the Components menu of the designer.")]
        public bool ShowRoundedRectanglePrimitive { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the Panel item in the Components menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("Components")]
        [Description("Gets or sets a visibility of the Panel item in the Components menu of the designer.")]
        public bool ShowPanel { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the Clone item in the Components menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("Components")]
        [Description("Gets or sets a visibility of the Clone item in the Components menu of the designer.")]
        public bool ShowClone { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the CheckBox item in the Components menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("Components")]
        [Description("Gets or sets a visibility of the CheckBox item in the Components menu of the designer.")]
        public bool ShowCheckBox { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the SubReport item in the Components menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("Components")]
        [Description("Gets or sets a visibility of the SubReport item in the Components menu of the designer.")]
        public bool ShowSubReport { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the ZipCode item in the Components menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("Components")]
        [Description("Gets or sets a visibility of the ZipCode item in the Components menu of the designer.")]
        public bool ShowZipCode { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the Chart item in the Components menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("Components")]
        [Description("Gets or sets a visibility of the Chart item in the Components menu of the designer.")]
        public bool ShowChart { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the Map item in the Components menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("Components")]
        [Description("Gets or sets a visibility of the Map item in the Components menu of the designer.")]
        public bool ShowMap { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the Gauge item in the Components menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("Components")]
        [Description("Gets or sets a visibility of the Gauge item in the Components menu of the designer.")]
        public bool ShowGauge { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the CrossTab item in the Components menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("Components")]
        [Description("Gets or sets a visibility of the CrossTab item in the Components menu of the designer.")]
        public bool ShowCrossTab { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the Table item in the Components menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("Components")]
        [Description("Gets or sets a visibility of the Table item in the Components menu of the designer.")]
        public bool ShowTable { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the Sparkline item in the Components menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("Components")]
        [Description("Gets or sets a visibility of the Sparkline item in the Components menu of the designer.")]
        public bool ShowSparkline { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the MathFormula item in the Components menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("Components")]
        [Description("Gets or sets a visibility of the MathFormula item in the Components menu of the designer.")]
        public bool ShowMathFormula { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the Electronic Signature item in the Components menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("Components")]
        [Description("Gets or sets a visibility of the Electronic Signature item in the Components menu of the designer.")]
        public bool ShowElectronicSignature { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the PdfDigitalSignature item in the Components menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("Components")]
        [Description("Gets or sets a visibility of the PdfDigitalSignature item in the Components menu of the designer.")]
        public bool ShowPdfDigitalSignature { get; set; } = true;
        #endregion

        #region Cross Bands

        /// <summary>
        /// Gets or sets a visibility of the CrossGroupHeaderBand item in the CrossBands menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("CrossBands")]
        [Description("Gets or sets a visibility of the CrossGroupHeaderBand item in the CrossBands menu of the designer.")]
        public bool ShowCrossGroupHeaderBand { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the CrossGroupFooterBand item in the CrossBands menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("CrossBands")]
        [Description("Gets or sets a visibility of the CrossGroupFooterBand item in the CrossBands menu of the designer.")]
        public bool ShowCrossGroupFooterBand { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the CrossHeaderBand item in the CrossBands menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("CrossBands")]
        [Description("Gets or sets a visibility of the CrossHeaderBand item in the CrossBands menu of the designer.")]
        public bool ShowCrossHeaderBand { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the CrossFooterBand item in the CrossBands menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("CrossBands")]
        [Description("Gets or sets a visibility of the CrossFooterBand item in the CrossBands menu of the designer.")]
        public bool ShowCrossFooterBand { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the CrossDataBand item in the CrossBands menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("CrossBands")]
        [Description("Gets or sets a visibility of the CrossDataBand item in the CrossBands menu of the designer.")]
        public bool ShowCrossDataBand { get; set; } = true;

        #endregion

        #region Dashboards
        /// <summary>
        /// Gets or sets a visibility of the new dashboard button in the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("Dashboards")]
        [Description("Gets or sets a visibility of the new dashboard button in the designer.")]
        public bool ShowNewDashboardButton { get; set; } = true;
        #endregion

        #region DashboardElements
        /// <summary>
        /// Gets or sets a visibility of the TableElement item in the Dashboard Elements menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("DashboardElements")]
        [Description("Gets or sets a visibility of the TableElement item in the Dashboard Elements menu of the designer.")]
        public bool ShowTableElement { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the ChartElement item in the Dashboard Elements menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("DashboardElements")]
        [Description("Gets or sets a visibility of the ChartElement item in the Dashboard Elements menu of the designer.")]
        public bool ShowChartElement { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the GaugeElement item in the Dashboard Elements menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("DashboardElements")]
        [Description("Gets or sets a visibility of the GaugeElement item in the Dashboard Elements menu of the designer.")]
        public bool ShowGaugeElement { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the PivotTableElement item in the Dashboard Elements menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("DashboardElements")]
        [Description("Gets or sets a visibility of the PivotTableElement item in the Dashboard Elements menu of the designer.")]
        public bool ShowPivotTableElement { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the IndicatorElement item in the Dashboard Elements menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("DashboardElements")]
        [Description("Gets or sets a visibility of the IndicatorElement item in the Dashboard Elements menu of the designer.")]
        public bool ShowIndicatorElement { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the ProgressElement item in the Dashboard Elements menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("DashboardElements")]
        [Description("Gets or sets a visibility of the ProgressElement item in the Dashboard Elements menu of the designer.")]
        public bool ShowProgressElement { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the ProgressElement item in the Dashboard Elements menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("DashboardElements")]
        [Description("Gets or sets a visibility of the CardsElement item in the Dashboard Elements menu of the designer.")]
        internal bool ShowCardsElement { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the ButtonElement item in the Dashboard Elements menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("DashboardElements")]
        [Description("Gets or sets a visibility of the ButtonElement item in the Dashboard Elements menu of the designer.")]
        internal bool ShowButtonElement { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the RegionMapElement item in the Dashboard Elements menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("DashboardElements")]
        [Description("Gets or sets a visibility of the RegionMapElement item in the Dashboard Elements menu of the designer.")]
        public bool ShowRegionMapElement { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the OnlineMapElement item in the Dashboard Elements menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("DashboardElements")]
        [Description("Gets or sets a visibility of the OnlineMapElement item in the Dashboard Elements menu of the designer.")]
        public bool ShowOnlineMapElement { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the ImageElement item in the Dashboard Elements menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("DashboardElements")]
        [Description("Gets or sets a visibility of the ImageElement item in the Dashboard Elements menu of the designer.")]
        public bool ShowImageElement { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the TextElement item in the Components menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("DashboardElements")]
        [Description("Gets or sets a visibility of the TextElement item in the Dashboard Elements menu of the designer.")]
        public bool ShowTextElement { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the PanelElement item in the Dashboard Elements menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("DashboardElements")]
        [Description("Gets or sets a visibility of the PanelElement item in the Dashboard Elements menu of the designer.")]
        public bool ShowPanelElement { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the ShapeElement item in the Dashboard Elements menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("DashboardElements")]
        [Description("Gets or sets a visibility of the ShapeElement item in the Dashboard Elements menu of the designer.")]
        public bool ShowShapeElement { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the ListBoxElement item in the Dashboard Elements menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("DashboardElements")]
        [Description("Gets or sets a visibility of the ListBoxElement item in the Dashboard Elements menu of the designer.")]
        public bool ShowListBoxElement { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the ComboBoxElement item in the Dashboard Elements menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("DashboardElements")]
        [Description("Gets or sets a visibility of the ComboBoxElement item in the Dashboard Elements menu of the designer.")]
        public bool ShowComboBoxElement { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the TreeViewElement item in the Dashboard Elements menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("DashboardElements")]
        [Description("Gets or sets a visibility of the TreeViewElement item in the Dashboard Elements menu of the designer.")]
        public bool ShowTreeViewElement { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the TreeViewBoxElement item in the Dashboard Elements menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("DashboardElements")]
        [Description("Gets or sets a visibility of the TreeViewBoxElement item in the Dashboard Elements menu of the designer.")]
        public bool ShowTreeViewBoxElement { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the DatePickerElement item in the Dashboard Elements menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("DashboardElements")]
        [Description("Gets or sets a visibility of the DatePickerElement item in the Dashboard Elements menu of the designer.")]
        public bool ShowDatePickerElement { get; set; } = true;
        #endregion

        #region Dictionary
        /// <summary>
        /// Gets or sets a value which indicates that using aliases in the dictionary.
        /// </summary>
        [DefaultValue(StiUseAliases.Auto)]
        [Category("Dictionary")]
        [Description("Gets or sets a value which indicates that using aliases in the dictionary.")]
        public StiUseAliases UseAliases { get; set; } = StiUseAliases.Auto;

        /// <summary>
        /// Gets or sets a visibility of the dictionary in the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("Dictionary")]
        [Description("Gets or sets a visibility of the dictionary in the designer.")]
        public bool ShowDictionary { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates what to do with the dictionary when creating a new report in the designer.
        /// </summary>
        [DefaultValue(StiNewReportDictionary.Auto)]
        [Category("Dictionary")]
        [Description("Gets or sets a value which indicates what to do with the dictionary when creating a new report in the designer.")]
        public StiNewReportDictionary NewReportDictionary { get; set; } = StiNewReportDictionary.Auto;

        /// <summary>
        /// Gets or sets a visibility of the Properties item in the dictionary context menu.
        /// </summary>
        [DefaultValue(true)]
        [Category("Dictionary")]
        [Description("Gets or sets a visibility of the Properties item in the dictionary context menu.")]
        public bool ShowDictionaryContextMenuProperties { get; set; } = true;

        /// <summary>
        /// Gets or sets a value of permissions for connections in the designer.
        /// </summary>
        [DefaultValue(StiDesignerPermissions.All)]
        [Category("Dictionary")]
        [Description("Gets or sets a value of permissions for connections in the designer.")]
        public StiDesignerPermissions PermissionDataConnections { get; set; } = StiDesignerPermissions.All;
        
        /// <summary>
        /// Gets or sets a value of permissions for datasources in the designer.
        /// </summary>
        [DefaultValue(StiDesignerPermissions.All)]
        [Category("Dictionary")]
        [Description("Gets or sets a value of permissions for datasources in the designer.")]
        public StiDesignerPermissions PermissionDataSources { get; set; } = StiDesignerPermissions.All;

        /// <summary>
        /// Gets or sets a value of permissions for dataTransformations in the designer.
        /// </summary>
        [DefaultValue(StiDesignerPermissions.All)]
        [Category("Dictionary")]
        [Description("Gets or sets a value of permissions for dataTransformations in the designer.")]
        public StiDesignerPermissions PermissionDataTransformations { get; set; } = StiDesignerPermissions.All;

        /// <summary>
        /// Gets or sets a value of permissions for columns in the designer.
        /// </summary>
        [DefaultValue(StiDesignerPermissions.All)]
        [Category("Dictionary")]
        [Description("Gets or sets a value of permissions for columns in the designer.")]
        public StiDesignerPermissions PermissionDataColumns { get; set; } = StiDesignerPermissions.All;
        
        /// <summary>
        /// Gets or sets a value of permissions for relations in the designer.
        /// </summary>
        [DefaultValue(StiDesignerPermissions.All)]
        [Category("Dictionary")]
        [Description("Gets or sets a value of permissions for relations in the designer.")]
        public StiDesignerPermissions PermissionDataRelations { get; set; } = StiDesignerPermissions.All;
        
        /// <summary>
        /// Gets or sets a value of permissions for business objects in the designer.
        /// </summary>
        [DefaultValue(StiDesignerPermissions.All)]
        [Category("Dictionary")]
        [Description("Gets or sets a value of permissions for business objects in the designer.")]
        public StiDesignerPermissions PermissionBusinessObjects { get; set; } = StiDesignerPermissions.All;
        
        /// <summary>
        /// Gets or sets a value of permissions for variables in the designer.
        /// </summary>
        [DefaultValue(StiDesignerPermissions.All)]
        [Category("Dictionary")]
        [Description("Gets or sets a value of permissions for variables in the designer.")]
        public StiDesignerPermissions PermissionVariables { get; set; } = StiDesignerPermissions.All;
        
        /// <summary>
        /// Gets or sets a value of permissions for resources in the designer.
        /// </summary>
        [DefaultValue(StiDesignerPermissions.All)]
        [Category("Dictionary")]
        [Description("Gets or sets a value of permissions for resources in the designer.")]
        public StiDesignerPermissions PermissionResources { get; set; } = StiDesignerPermissions.All;
        
        /// <summary>
        /// Gets or sets a value of permissions for sql parameters in the designer.
        /// </summary>
        [DefaultValue(StiDesignerPermissions.All)]
        [Category("Dictionary")]
        [Description("Gets or sets a value of permissions for sql parameters in the designer.")]
        public StiDesignerPermissions PermissionSqlParameters { get; set; } = StiDesignerPermissions.All;

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
        /// Gets or sets the default email address of the message created in the preview.
        /// </summary>
        [Category("Email")]
        [DefaultValue("")]
        [Description("Gets or sets the default email address of the message created in the preview.")]
        public string DefaultEmailAddress { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the default subject of the message created in the preview.
        /// </summary>
        [Category("Email")]
        [DefaultValue("")]
        [Description("Gets or sets the default subject of the message created in the preview.")]
        public string DefaultEmailSubject { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the default text of the message created in the preview.
        /// </summary>
        [Category("Email")]
        [DefaultValue("")]
        [Description("Gets or sets the default text of the message created in the preview.")]
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
        /// Gets or sets a value which indicates that the user can save the report from the preview to the report document file.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the preview to the report document file.")]
        public bool ShowExportToDocument { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the preview to the PDF format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the preview to the PDF format.")]
        public bool ShowExportToPdf { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the preview to the XPS format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(false)]
        [Description("Gets or sets a value which indicates that the user can save the report from the preview to the XPS format.")]
        public bool ShowExportToXps { get; set; } = false;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the preview to the Power Point 2007-2010 format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the preview to the Power Point 2007-2010 format.")]
        public bool ShowExportToPowerPoint { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the preview to the HTML format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the preview to the HTML format.")]
        public bool ShowExportToHtml { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the preview to the HTML5 format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the preview to the HTML5 format.")]
        public bool ShowExportToHtml5 { get; set; } = true;
        
        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the preview to the MHT (Web Archive) format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the preview to the MHT (Web Archive) format.")]
        public bool ShowExportToMht { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the preview to the TEXT format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the preview to the TEXT format.")]
        public bool ShowExportToText { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the preview to the Rich Text format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the preview to the Rich Text format.")]
        public bool ShowExportToRtf { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the preview to the Word 2007-2010 format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the preview to the Word 2007-2010 format.")]
        public bool ShowExportToWord2007 { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the preview to the Open Document Text format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the preview to the Open Document Text format.")]
        public bool ShowExportToOpenDocumentWriter { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the preview to the Excel BIFF format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the preview to the Excel BIFF format.")]
        public bool ShowExportToExcel { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the preview to the Excel XML format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the preview to the Excel XML format.")]
        public bool ShowExportToExcelXml { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the preview to the Excel 2007-2010 format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the preview to the Excel 2007-2010 format.")]
        public bool ShowExportToExcel2007 { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the preview to the Open Document Calc format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the preview to the Open Document Calc format.")]
        public bool ShowExportToOpenDocumentCalc { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the preview to the CSV format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the preview to the CSV format.")]
        public bool ShowExportToCsv { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the preview to the DBF format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the preview to the DBF format.")]
        public bool ShowExportToDbf { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the preview to the XML format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the preview to the XML format.")]
        public bool ShowExportToXml { get; set; } = true;
        
        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the preview to the DIF format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the preview to the DIF format.")]
        public bool ShowExportToDif { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the preview to the Sylk format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the preview to the Sylk format.")]
        public bool ShowExportToSylk { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the preview to the BMP image format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the preview to the BMP image format.")]
        public bool ShowExportToImageBmp { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the preview to the GIF image format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the preview to the GIF image format.")]
        public bool ShowExportToImageGif { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the preview to the JPEG image format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the preview to the JPEG image format.")]
        public bool ShowExportToImageJpeg { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the preview to the PCX image format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the preview to the PCX image format.")]
        public bool ShowExportToImagePcx { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the preview to the PNG image format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the preview to the PNG image format.")]
        public bool ShowExportToImagePng { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the preview to the TIFF image format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the preview to the TIFF image format.")]
        public bool ShowExportToImageTiff { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the preview to the Metafile image format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the preview to the Metafile image format.")]
        public bool ShowExportToImageMetafile { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the preview to the SVG image format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the preview to the SVG image format.")]
        public bool ShowExportToImageSvg { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the user can save the report from the preview to the SVGZ image format.
        /// </summary>
        [Category("Exports")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the user can save the report from the preview to the SVGZ image format.")]
        public bool ShowExportToImageSvgz { get; set; } = true;

        #endregion

        #region Internal
        
        private string SaveReportErrorString = string.Empty;

        private static bool IsDesignMode
        {
            get
            {
                return HttpContext.Current == null;
            }
        }

        /// <summary>
        /// Internal use only.
        /// </summary>
        [Browsable(false)]
        [Description("Internal use only.")]
        public bool ServerMode { get; set; }

        /// <summary>
        /// Internal use only.
        /// </summary>
        [Browsable(false)]
        [Description("Internal use only.")]
        public bool CloudMode { get; set; }

        private string clientGuid;
        internal string ClientGuid
        {
            get
            {
                if (string.IsNullOrEmpty(clientGuid)) clientGuid = StiGuidUtils.NewGuid();
                return clientGuid;
            }
            set
            {
                clientGuid = value;
            }
        }

        private StiWebViewer viewer;
        private StiWebViewer Viewer
        {
            get
            {
                if (viewer == null) viewer = CreateViewer();
                return viewer;
            }
            set
            {
                viewer = value;
            }
        }

        private StiRequestParams requestParams;
        private StiRequestParams RequestParams
        {
            get
            {
                if (requestParams == null) requestParams = GetRequestParams();
                return requestParams;
            }
        }

        #endregion
        
        #region File Menu
        
        /// <summary>
        /// Gets or sets a visibility of the file menu of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("FileMenu")]
        [Description("Gets or sets a visibility of the file menu of the designer.")]
        public bool ShowFileMenu { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the New item in the file menu.
        /// </summary>
        [DefaultValue(true)]
        [Category("FileMenu")]
        [Description("Gets or sets a visibility of the New item in the file menu.")]
        public bool ShowFileMenuNew { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the Open item in the file menu.
        /// </summary>
        [DefaultValue(true)]
        [Category("FileMenu")]
        [Description("Gets or sets a visibility of the Open item in the file menu.")]
        public bool ShowFileMenuOpen { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the Save item in the file menu.
        /// </summary>
        [DefaultValue(true)]
        [Category("FileMenu")]
        [Description("Gets or sets a visibility of the Save item in the file menu.")]
        public bool ShowFileMenuSave { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the Save As item in the file menu.
        /// </summary>
        [DefaultValue(true)]
        [Category("FileMenu")]
        [Description("Gets or sets a visibility of the Save As item in the file menu.")]
        public bool ShowFileMenuSaveAs { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the Close item in the file menu.
        /// </summary>
        [DefaultValue(true)]
        [Category("FileMenu")]
        [Description("Gets or sets a visibility of the Close item in the file menu.")]
        public bool ShowFileMenuClose { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the Exit item in the file menu.
        /// </summary>
        [DefaultValue(false)]
        [Category("FileMenu")]
        [Description("Gets or sets a visibility of the Exit item in the file menu.")]
        public bool ShowFileMenuExit { get; set; }

        /// <summary>
        /// Gets or sets a visibility of the Report Setup item in the file menu.
        /// </summary>
        [DefaultValue(true)]
        [Category("FileMenu")]
        [Description("Gets or sets a visibility of the Report Setup item in the file menu.")]
        public bool ShowFileMenuReportSetup { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the Options item in the file menu.
        /// </summary>
        [DefaultValue(true)]
        [Category("FileMenu")]
        [Description("Gets or sets a visibility of the Options item in the file menu.")]
        public bool ShowFileMenuOptions { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the Info item in the file menu.
        /// </summary>
        [DefaultValue(true)]
        [Category("FileMenu")]
        [Description("Gets or sets a visibility of the Info item in the file menu.")]
        public bool ShowFileMenuInfo { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the About item in the file menu.
        /// </summary>
        [DefaultValue(true)]
        [Category("FileMenu")]
        [Description("Gets or sets a visibility of the About item in the file menu.")]
        public bool ShowFileMenuAbout { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the Help item in the file menu.
        /// </summary>
        [DefaultValue(true)]
        [Category("FileMenu")]
        [Description("Gets or sets a visibility of the Help item in the file menu.")]
        public bool ShowFileMenuHelp { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the new report button in the file menu.
        /// </summary>
        [DefaultValue(true)]
        [Category("Pages")]
        [Description("Gets or sets a visibility of the new report button in the designer.")]
        public bool ShowFileMenuNewReport { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the new dashboard button in the file menu.
        /// </summary>
        [DefaultValue(true)]
        [Category("Pages")]
        [Description("Gets or sets a visibility of the new dashboard button in the designer.")]
        public bool ShowFileMenuNewDashboard { get; set; } = true;
        #endregion

        #region Hidden

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Category("Behaviour")]
        public StiDesignerComponents[] ComponentsIntoInsertTab { get; set; }

        /// <summary>
        /// Focusing on the X axis. Required if the lines are displayed double.
        /// </summary>
        [DefaultValue(false)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Category("Behaviour")]
        [Description("Focusing on the X axis. Required if the lines are displayed double.")]
        public bool FocusingX { get; set; }

        /// <summary>
        /// Focusing on the Y axis. Required if the lines are displayed double.
        /// </summary>
        [DefaultValue(false)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Category("Behaviour")]
        [Description("Focusing on the Y axis. Required if the lines are displayed double.")]
        public bool FocusingY { get; set; }

        #endregion

        #region Preview Toolbar
        
        /// <summary>
        /// Gets or sets a value which indicates that toolbar will be shown in the preview.
        /// </summary>
        [Category("Preview")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that toolbar will be shown in the preview.")]
        public bool ShowPreviewToolbar { get; set; } = true;

        /// <summary>
        /// Gets or sets a color of the preview toolbar background. The default value is the theme color.
        /// </summary>
        [Category("Preview")]
        [DefaultValue(typeof(Color), "Empty")]
        [Description("Gets or sets a color of the preview toolbar background. The default value is the theme color.")]
        [TypeConverter(typeof(WebColorConverter))]
        public Color PreviewToolbarBackgroundColor { get; set; } = Color.Empty;

        /// <summary>
        /// Gets or sets a color of the preview toolbar border. The default value is the theme color.
        /// </summary>
        [DefaultValue(typeof(Color), "Empty")]
        [Category("Preview")]
        [Description("Gets or sets a color of the preview toolbar border. The default value is the theme color.")]
        [TypeConverter(typeof(WebColorConverter))]
        public Color PreviewToolbarBorderColor { get; set; } = Color.Empty;

        /// <summary>
        /// Gets or sets a color of the preview toolbar texts.
        /// </summary>
        [DefaultValue(typeof(Color), "Empty")]
        [Category("Preview")]
        [Description("Gets or sets a color of the preview toolbar texts.")]
        [TypeConverter(typeof(WebColorConverter))]
        public Color PreviewToolbarFontColor { get; set; } = Color.Empty;

        /// <summary>
        /// Gets or sets a value which indicates which font family will be used for drawing texts in the toolbar of the preview.
        /// </summary>
        [Category("Preview")]
        [DefaultValue("Arial")]
        [Description("Gets or sets a value which indicates which font family will be used for drawing texts in the toolbar of the preview.")]
        public string PreviewToolbarFontFamily { get; set; } = "Arial";

        /// <summary>
        /// Gets or sets the alignment of the preview toolbar.
        /// </summary>
        [Category("Preview")]
        [DefaultValue(StiContentAlignment.Default)]
        [Description("Gets or sets the alignment of the preview toolbar.")]
        public StiContentAlignment PreviewToolbarAlignment { get; set; } = StiContentAlignment.Default;

        /// <summary>
        /// Gets or sets a value which allows displaying or hiding toolbar buttons captions in the toolbar of the preview.
        /// </summary>
        [Category("Preview")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which allows displaying or hiding toolbar buttons captions in the toolbar of the preview.")]
        public bool ShowPreviewButtonCaptions { get; set; } = true;
        
        /// <summary>
        /// Gets or sets a visibility of the Print button in the toolbar of the preview.
        /// </summary>
        [Category("Preview")]
        [DefaultValue(true)]
        [Description("Gets or sets a visibility of the Print button in the toolbar of the preview.")]
        public bool ShowPreviewPrintButton { get; set; } = true;
        
        /// <summary>
        /// Gets or sets a visibility of the Open button in the toolbar of the preview.
        /// </summary>
        [Category("Preview")]
        [DefaultValue(true)]
        [Description("Gets or sets a visibility of the Open button in the toolbar of the preview.")]
        public bool ShowPreviewOpenButton { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the Save button in the toolbar of the preview.
        /// </summary>
        [Category("Preview")]
        [DefaultValue(true)]
        [Description("Gets or sets a visibility of the Save button in the toolbar of the preview.")]
        public bool ShowPreviewSaveButton { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the Send Email button in the toolbar of the preview.
        /// </summary>
        [Category("Preview")]
        [DefaultValue(false)]
        [Description("Gets or sets a visibility of the Send Email button in the toolbar of the preview.")]
        public bool ShowPreviewSendEmailButton { get; set; }

        /// <summary>
        /// Gets or sets a visibility of the Find button in the toolbar of the preview.
        /// </summary>
        [Category("Preview")]
        [DefaultValue(true)]
        [Description("Gets or sets a visibility of the Find button in the toolbar of the preview.")]
        public bool ShowPreviewFindButton { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the Signature button in the toolbar of the preview.
        /// </summary>
        [Category("Preview")]
        [DefaultValue(true)]
        [Description("Gets or sets a visibility of the Signature button in the toolbar of the preview.")]
        public bool ShowPreviewSignatureButton { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the Bookmarks button in the toolbar of the preview.
        /// </summary>
        [Category("Preview")]
        [DefaultValue(true)]
        [Description("Gets or sets a visibility of the Bookmarks button in the toolbar of the preview.")]
        public bool ShowPreviewBookmarksButton { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the Parameters button in the toolbar of the preview.
        /// </summary>
        [Category("Preview")]
        [DefaultValue(true)]
        [Description("Gets or sets a visibility of the Parameters button in the toolbar of the preview.")]
        public bool ShowPreviewParametersButton { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the Editor button in the toolbar of the preview.
        /// </summary>
        [Category("Preview")]
        [DefaultValue(true)]
        [Description("Gets or sets a visibility of the Editor button in the toolbar of the preview.")]
        public bool ShowPreviewEditorButton { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the First Page button in the toolbar of the preview.
        /// </summary>
        [Category("Preview")]
        [DefaultValue(true)]
        [Description("Gets or sets a visibility of the First Page button in the toolbar of the preview.")]
        public bool ShowPreviewFirstPageButton { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the Prev Page button in the toolbar of the preview.
        /// </summary>
        [Category("Preview")]
        [DefaultValue(true)]
        [Description("Gets or sets a visibility of the Prev Page button in the toolbar of the preview.")]
        public bool ShowPreviewPreviousPageButton { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the current page control in the toolbar of the preview.
        /// </summary>
        [Category("Preview")]
        [DefaultValue(true)]
        [Description("Gets or sets a visibility of the current page control in the toolbar of the preview.")]
        public bool ShowPreviewCurrentPageControl { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the Next Page button in the toolbar of the preview.
        /// </summary>
        [Category("Preview")]
        [DefaultValue(true)]
        [Description("Gets or sets a visibility of the Next Page button in the toolbar of the preview.")]
        public bool ShowPreviewNextPageButton { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the Last Page button in the toolbar of the preview.
        /// </summary>
        [Category("Preview")]
        [DefaultValue(true)]
        [Description("Gets or sets a visibility of the Last Page button in the toolbar of the preview.")]
        public bool ShowPreviewLastPageButton { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the Zoom control in the toolbar of the preview.
        /// </summary>
        [Category("Preview")]
        [DefaultValue(true)]
        [Description("Gets or sets a visibility of the Zoom control in the toolbar of the preview.")]
        public bool ShowPreviewZoomButton { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the View Mode button in the toolbar of the preview.
        /// </summary>
        [Category("Preview")]
        [DefaultValue(true)]
        [Description("Gets or sets a visibility of the View Mode button in the toolbar of the preview.")]
        public bool ShowPreviewViewModeButton { get; set; } = true;
        
        /// <summary>
        /// Gets or sets the default mode of the report print destination.
        /// </summary>
        [Category("Preview")]
        [DefaultValue(StiPrintDestination.Default)]
        [Description("Gets or sets the default mode of the report print destination.")]
        public StiPrintDestination PrintDestination { get; set; } = StiPrintDestination.Default;

        /// <summary>
        /// Gets or sets the mode of showing a report in the preview - Single Page, Continuous or Multiple Pages.
        /// </summary>
        [Category("Preview")]
        [DefaultValue(StiWebViewMode.SinglePage)]
        [Description("Gets or sets the mode of showing a report in the preview - Single Page, Continuous or Multiple Pages.")]
        public StiWebViewMode PreviewViewMode { get; set; } = StiWebViewMode.SinglePage;

        #endregion

        #region Properties Grid
        
        /// <summary>
        /// Gets or sets a visibility of the properties grid in the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("PropertiesGrid")]
        [Description("Gets or sets a visibility of the properties grid in the designer.")]
        public bool ShowPropertiesGrid { get; set; } = true;
        
        /// <summary>
        /// Gets or sets a width of the properties grid in the designer.
        /// </summary>
        [Category("PropertiesGrid")]
        [DefaultValue(370)]
        [Description("Gets or sets a width of the properties grid in the designer.")]
        public int PropertiesGridWidth { get; set; } = 370;

        /// <summary>
        /// Gets or sets a width of the property label in properties grid in the designer.
        /// </summary>
        [Category("PropertiesGrid")]
        [DefaultValue(160)]
        [Description("Gets or sets a width of the property label in properties grid in the designer.")]
        public int PropertiesGridLabelWidth { get; set; } = 160;

        /// <summary>
        /// Gets or sets a position of the properties grid.
        /// </summary>
        [Category("PropertiesGrid")]
        [DefaultValue(StiPropertiesGridPosition.Left)]
        [Description("Gets or sets a position of the properties grid.")]
        public StiPropertiesGridPosition PropertiesGridPosition { get; set; } = StiPropertiesGridPosition.Left;


        /// <summary>
        /// Gets or sets a visibility of the properties which used from styles in the designer.
        /// </summary>
        [DefaultValue(false)]
        [Category("PropertiesGrid")]
        [Description("Gets or sets a visibility of the properties which used from styles in the designer.")]
        public bool ShowPropertiesWhichUsedFromStyles { get; set; } = false;
        #endregion

        #region Report

        private StiReport report;
        /// <summary>
        /// Gets or sets a report object which to edit in the designer.
        /// </summary>
        [Browsable(false)]
        [Description("Gets or sets a report object which to edit in the designer.")]
        public StiReport Report
        {
            get
            {
                if (report == null) report = GetReportObject(this.RequestParams);
                if (report != null)
                {
                    report.Info.ForceDesigningMode = true;
                    if ((CacheMode == StiServerCacheMode.StringCache || CacheMode == StiServerCacheMode.StringSession || requestParams.Cache.UseCacheHelper) && this.RequestParams.GetString("designerOptions") != null)
                        StiDesignerOptionsHelper.ApplyDesignerOptionsToReport(StiDesignerOptionsHelper.GetDesignerOptions(this.RequestParams), report);
                }
                return report;
            }
            set
            {
                report = value;
                if (this.RequestParams.ComponentType != StiComponentType.Designer) this.requestParams = this.CreateRequestParams();
                if (report != null) this.RequestParams.Cache.Helper.SaveReportInternal(this.RequestParams, report);
                else this.RequestParams.Cache.Helper.RemoveReportInternal(this.RequestParams);
            }
        }

        #endregion

        #region Server
        
        private static StiCacheHelper cacheHelper;
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
        /// Allows the designer to update the cache automatically. The cache will be updated about once every three minutes if there are no actions in the designer.
        /// </summary>
        [Category("Server")]
        [DefaultValue(true)]
        [Description("Allows the designer to update the cache automatically. The cache will be updated about once every three minutes if there are no actions in the designer.")]
        public bool AllowAutoUpdateCache { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that the designer will use relative or absolute URLs.
        /// </summary>
        [Category("Server")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that the designer will use relative or absolute URLs.")]
        public bool UseRelativeUrls { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which specifies the port number to use in the URL. A value of 0 defines automatic detection. A value of -1 removes the port number.
        /// </summary>
        [Category("Server")]
        [DefaultValue(0)]
        [Description("Gets or sets a value which specifies the port number to use in the URL. A value of 0 defines automatic detection. A value of -1 removes the port number.")]
        public int PortNumber { get; set; } = 0;

        /// <summary>
        /// Gets or sets a value which enables or disables the transfer of URL parameters when requesting the scripts and styles of the designer.
        /// </summary>
        [Category("Server")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which enables or disables the transfer of URL parameters when requesting the scripts and styles of the designer.")]
        public bool PassQueryParametersForResources { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which enables or disables the display of the detailed server error in the preview.
        /// </summary>
        [Category("Server")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which enables or disables the display of the detailed server error in the preview.")]
        public bool ShowServerErrorPage { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which enables or disables the GZip compression of requests to the server. This allows to reduce the volume of Internet traffic, but slightly slows down the designer actions.
        /// </summary>
        [Category("Server")]
        [DefaultValue(false)]
        [Description("Gets or sets a value which enables or disables the GZip compression of requests to the server. This allows to reduce the volume of Internet traffic, but slightly slows down the designer actions.")]
        public bool UseCompression { get; set; }

        /// <summary>
        /// Gets or sets a value which enables caching for designer scripts and styles.
        /// </summary>
        [Category("Server")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which enables caching for designer scripts and styles.")]
        public bool UseCacheForResources { get; set; } = true;

        /// <summary>
        /// Allows the designer to update the cookies automatically on every request to the server. By default, cookies are set when creating the designer, if they are not specified in the report.
        /// </summary>
        [Category("Server")]
        [DefaultValue(false)]
        [Description("Allows the designer to update the cookies automatically on every request to the server. By default, cookies are set when creating the designer, if they are not specified in the report.")]
        public bool AllowAutoUpdateCookies { get; set; }

        /// <summary>
        /// "Allows loading custom fonts to the client side.
        /// </summary>
        [Category("Server")]
        [DefaultValue(false)]
        [Description("Allows loading custom fonts to the client side.")]
        public bool AllowLoadingCustomFontsToClientSide { get; set; }
        #endregion

        #region Toolbar
        /// <summary>
        /// Gets or sets a value which indicates that toolbar will be shown in the designer.
        /// </summary>
        [Category("Toolbar")]
        [DefaultValue(true)]
        [Description("Gets or sets a value which indicates that toolbar will be shown in the designer.")]
        public bool ShowToolbar { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the setup toolbox button in the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("Toolbar")]
        [Description("Gets or sets a visibility of the setup toolbox button in the designer.")]
        public bool ShowSetupToolboxButton { get; set; } = true;
               
        /// <summary>
        /// Gets or sets a visibility of the insert button in the toolbar of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("Toolbar")]
        [Description("Gets or sets a visibility of the insert button in the toolbar of the designer.")]
        public bool ShowInsertButton { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the layout button in the toolbar of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("Toolbar")]
        [Description("Gets or sets a visibility of the layout button in the toolbar of the designer.")]
        public bool ShowLayoutButton { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the page button in the toolbar of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("Toolbar")]
        [Description("Gets or sets a visibility of the page button in the toolbar of the designer.")]
        public bool ShowPageButton { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the preview button in the toolbar of the designer.
        /// </summary>
        [DefaultValue(true)]
        [Category("Toolbar")]
        [Description("Gets or sets a visibility of the preview button in the toolbar of the designer.")]
        public bool ShowPreviewButton { get; set; } = true;

        /// <summary>
        /// Gets or sets a visibility of the save button in the toolbar of the designer.
        /// </summary>
        [DefaultValue(false)]
        [Category("Toolbar")]
        [Description("Gets or sets a visibility of the save button in the toolbar of the designer.")]
        public bool ShowSaveButton { get; set; } = false;
        
        /// <summary>
        /// Gets or sets a visibility of the about button in the toolbar of the designer.
        /// </summary>
        [DefaultValue(false)]
        [Category("Toolbar")]
        [Description("Gets or sets a visibility of the about button in the toolbar of the designer.")]
        public bool ShowAboutButton { get; set; }
        #endregion
    }
}
