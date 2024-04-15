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

using Stimulsoft.System.Web.UI.WebControls;
using System.ComponentModel;
using Stimulsoft.Report.Web;
using Stimulsoft.System.Web.Caching;
using System;
using System.Drawing;
using Stimulsoft.Base.Json;

namespace Stimulsoft.Report.Mvc
{
    public class StiNetCoreDesignerOptions
    {
        #region Actions
        public class ActionOptions
        {
            /// <summary>
            /// Gets or sets the action method name of loading the report template.
            /// </summary>
            public string GetReport { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the action method name of opening the report template using the file menu.
            /// </summary>
            public string OpenReport { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the action method name of creating the new report template using the file menu.
            /// </summary>
            public string CreateReport { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the action method name of saving the report template.
            /// </summary>
            public string SaveReport { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the action method name of saving as file the report template.
            /// </summary>
            public string SaveReportAs { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the action method name of preparing the rendered report for preview.
            /// </summary>
            public string PreviewReport { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the action method name of loading the rendered report in the preview.
            /// </summary>
            public string GetPreviewReport { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the action method name of exporting a report to the required format.
            /// </summary>
            public string ExportReport { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the name of the action method for redirect to the desired view when exiting the designer.
            /// </summary>
            public string Exit { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the action method name of the report designer default events.
            /// </summary>
            public string DesignerEvent { get; set; } = string.Empty;
        }
        #endregion

        #region Appearance
        public class AppearanceOptions
        {
            /// <summary>
            /// Gets or sets a path to the custom css file for the designer.
            /// </summary>
            public string CustomCss { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets a default value of the report unit in the designer.
            /// </summary>
            public StiReportUnitType DefaultUnit { get; set; } = StiReportUnitType.Centimeters;

            /// <summary>
            /// Gets or sets the report showing zoom. The default value is 100.
            /// </summary>
            private int zoom = 100;
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
            public bool ShowAnimation { get; set; } = true;

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
            /// Gets or sets a value which allows to display the open dialog, or to open with the open event.
            /// </summary>
            public bool ShowOpenDialog { get; set; } = true;

            /// <summary>
            /// Gets or sets the interface type of the designer.
            /// </summary>
            public StiInterfaceType InterfaceType { get; set; } = StiInterfaceType.Auto;

            /// <summary>
            /// Gets or sets the type of the chart in the preview.
            /// </summary>
            public StiChartRenderType ChartRenderType { get; set; } = StiChartRenderType.AnimatedVector;

            /// <summary>
            /// Gets or sets the first day of week in the date picker.
            /// </summary>
            public StiFirstDayOfWeek DatePickerFirstDayOfWeek { get; set; } = StiFirstDayOfWeek.Auto;

            /// <summary>
            /// Gets or sets a value, which indicates that the current day will be included in the ranges of the date picker.
            /// </summary>
            public bool DatePickerIncludeCurrentDayForRanges { get; set; } = false;

            /// <summary>
            /// Gets or sets a date format for datetime parameters in parameters panel. To use a server date format please set the StiDateFormatMode.FromServer or "FromServer" string value.
            /// The default is the client browser date format.
            /// </summary>
            public string ParametersPanelDateFormat { get; set; } = StiDateFormatMode.FromClient;

            /// <summary>
            /// Gets or sets a visibility of the report tree in the designer.
            /// </summary>
            public bool ShowReportTree { get; set; } = true;

            /// <summary>
            /// Gets or sets a method how will show a report in the preview mode.
            /// </summary>
            public StiReportDisplayMode ReportDisplayMode { get; set; } = StiReportDisplayMode.FromReport;

            /// <summary>
            /// Gets or sets a value which indicates that the designer will be closed without asking.
            /// </summary>
            public bool CloseDesignerWithoutAsking { get; set; } = false;

            /// <summary>
            /// Gets or sets a visibility of the system fonts in the fonts list.
            /// </summary>
            public bool ShowSystemFonts { get; set; } = true;

            /// <summary>
            /// Gets or sets a value of the wizard type which should be run after designer starts.
            /// </summary>
            public StiWizardType WizardTypeRunningAfterLoad { get; set; } = StiWizardType.None;

            /// <summary>
            /// Gets or sets a value which indicates that allows word wrap in the text editors.
            /// </summary>
            public bool AllowWordWrapTextEditors { get; set; } = true;
        }
        #endregion

        #region Bands
        public class BandsOptions
        {
            /// <summary>
            /// Gets or sets a visibility of the ReportTitleBand item in the Bands menu of the designer.
            /// </summary>
            public bool ShowReportTitleBand { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the ReportSummaryBand item in the Bands menu of the designer.
            /// </summary>
            public bool ShowReportSummaryBand { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the PageHeaderBand item in the Bands menu of the designer.
            /// </summary>
            public bool ShowPageHeaderBand { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the PageFooterBand item in the Bands menu of the designer.
            /// </summary>
            public bool ShowPageFooterBand { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the GroupHeaderBand item in the Bands menu of the designer.
            /// </summary>
            public bool ShowGroupHeaderBand { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the GroupFooterBand item in the Bands menu of the designer.
            /// </summary>
            public bool ShowGroupFooterBand { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the HeaderBand item in the Bands menu of the designer.
            /// </summary>
            public bool ShowHeaderBand { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the FooterBand item in the Bands menu of the designer.
            /// </summary>
            public bool ShowFooterBand { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the ColumnHeaderBand item in the Bands menu of the designer.
            /// </summary>
            public bool ShowColumnHeaderBand { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the ColumnFooterBand item in the Bands menu of the designer.
            /// </summary>
            public bool ShowColumnFooterBand { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the DataBand item in the Bands menu of the designer.
            /// </summary>
            public bool ShowDataBand { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the HierarchicalBand item in the Bands menu of the designer.
            /// </summary>
            public bool ShowHierarchicalBand { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the ChildBand item in the Bands menu of the designer.
            /// </summary>
            public bool ShowChildBand { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the EmptyBand item in the Bands menu of the designer.
            /// </summary>
            public bool ShowEmptyBand { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the OverlayBand item in the Bands menu of the designer.
            /// </summary>
            public bool ShowOverlayBand { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the TableOfContents item in the Bands menu of the designer.
            /// </summary>
            public bool ShowTableOfContents { get; set; } = true;
        }
        #endregion

        #region Behavior
        public class BehaviorOptions
        {
            /// <summary>
            /// Gets or sets a maximum level of undo actions with the report. A large number of actions consume more memory on the server side.
            /// </summary>
            public int UndoMaxLevel { get; set; } = 6;

            /// <summary>
            /// Gets or sets the value that displays or hides the report file name dialog at saving.
            /// </summary>
            public bool ShowSaveDialog { get; set; } = true;

            /// <summary>
            /// Allow the designer to change the window title.
            /// </summary>
            public bool AllowChangeWindowTitle { get; set; } = true;

            /// <summary>
            /// Gets or sets the save report mode - Hidden (AJAX mode), Visible (POST mode) or New Window.
            /// </summary>
            public StiSaveMode SaveReportMode { get; set; } = StiSaveMode.Hidden;

            /// <summary>
            /// Gets or sets the save report mode - Hidden (AJAX mode), Visible (POST mode) or New Window.
            /// </summary>
            public StiSaveMode SaveReportAsMode { get; set; } = StiSaveMode.Hidden;

            /// <summary>
            /// Gets or sets the value that allows running the report checker before preview.
            /// </summary>
            public bool CheckReportBeforePreview { get; set; } = true;

            #region Hidden

            /// <summary>
            /// Focusing on the X axis. Required if the lines are displayed double.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public bool FocusingX { get; set; } = false;

            /// <summary>
            /// Focusing on the Y axis. Required if the lines are displayed double.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public bool FocusingY { get; set; } = false;

            #endregion
        }
        #endregion

        #region Components
        public class ComponentsOptions
        {
            /// <summary>
            /// Gets or sets a visibility of the Text item in the Components menu of the designer.
            /// </summary>
            public bool ShowText { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the TextInCells item in the Components menu of the designer.
            /// </summary>
            public bool ShowTextInCells { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the RichText item in the Components menu of the designer.
            /// </summary>
            public bool ShowRichText { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the Image item in the Components menu of the designer.
            /// </summary>
            public bool ShowImage { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the BarCode item in the Components menu of the designer.
            /// </summary>
            public bool ShowBarCode { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the Shape item in the Components menu of the designer.
            /// </summary>
            public bool ShowShape { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the Horizontal Line Primitive item in the Components menu of the designer.
            /// </summary>
            public bool ShowHorizontalLinePrimitive { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the Vertical Line Primitive item in the Components menu of the designer.
            /// </summary>
            public bool ShowVerticalLinePrimitive { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the Rectangle Primitive item in the Components menu of the designer.
            /// </summary>
            public bool ShowRectanglePrimitive { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the Rounded Rectangle Primitive item in the Components menu of the designer.
            /// </summary>
            public bool ShowRoundedRectanglePrimitive { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the Panel item in the Components menu of the designer.
            /// </summary>
            public bool ShowPanel { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the Clone item in the Components menu of the designer.
            /// </summary>
            public bool ShowClone { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the CheckBox item in the Components menu of the designer.
            /// </summary>
            public bool ShowCheckBox { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the SubReport item in the Components menu of the designer.
            /// </summary>
            public bool ShowSubReport { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the ZipCode item in the Components menu of the designer.
            /// </summary>
            public bool ShowZipCode { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the Chart item in the Components menu of the designer.
            /// </summary>
            public bool ShowChart { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the Map item in the Components menu of the designer.
            /// </summary>
            public bool ShowMap { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the Gauge item in the Components menu of the designer.
            /// </summary>
            public bool ShowGauge { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the CrossTab item in the Components menu of the designer.
            /// </summary>
            public bool ShowCrossTab { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the Table item in the Bands menu of the designer.
            /// </summary>
            public bool ShowTable { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the Sparkline item in the Components menu of the designer.
            /// </summary>
            public bool ShowSparkline { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the MathFormula item in the Components menu of the designer.
            /// </summary>
            public bool ShowMathFormula { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the Electronic Signature item in the Components menu of the designer.
            /// </summary>
            public bool ShowElectronicSignature { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the PdfDigitalSignature item in the Components menu of the designer.
            /// </summary>
            public bool ShowPdfDigitalSignature { get; set; } = true;
        }
        #endregion

        #region Pages
        public class PagesOptions
        {
            /// <summary>
            /// Gets or sets a visibility of the new page button in the designer.
            /// </summary>
            public bool ShowNewPageButton { get; set; } = true;
        }
        #endregion

        #region Cross Bands
        public class CrossBandsOptions
        {
            /// <summary>
            /// Gets or sets a visibility of the CrossGroupHeaderBand item in the CrossBands menu of the designer.
            /// </summary>
            public bool ShowCrossGroupHeaderBand { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the CrossGroupFooterBand item in the CrossBands menu of the designer.
            /// </summary>
            public bool ShowCrossGroupFooterBand { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the CrossHeaderBand item in the CrossBands menu of the designer.
            /// </summary>
            public bool ShowCrossHeaderBand { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the CrossFooterBand item in the CrossBands menu of the designer.
            /// </summary>
            public bool ShowCrossFooterBand { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the CrossDataBand item in the CrossBands menu of the designer.
            /// </summary>
            public bool ShowCrossDataBand { get; set; } = true;
        }
        #endregion

        #region Dashboards
        public class DashboardsOptions
        {
            /// <summary>
            /// Gets or sets a visibility of the new dashboard button in the designer.
            /// </summary>
            public bool ShowNewDashboardButton { get; set; } = true;
        }
        #endregion

        #region Dashboard Elements
        public class DashboardElementsOptions
        {
            /// <summary>
            /// Gets or sets a visibility of the TableElement item in the Dashboard Elements menu of the designer.
            /// </summary>
            public bool ShowTableElement { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the ChartElement item in the Dashboard Elements menu of the designer.
            /// </summary>
            public bool ShowChartElement { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the GaugeElement item in the Dashboard Elements menu of the designer.
            /// </summary>
            public bool ShowGaugeElement { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the PivotTableElement item in the Dashboard Elements menu of the designer.
            /// </summary>
            public bool ShowPivotTableElement { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the IndicatorElement item in the Dashboard Elements menu of the designer.
            /// </summary>
            public bool ShowIndicatorElement { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the ProgressElement item in the Dashboard Elements menu of the designer.
            /// </summary>
            public bool ShowProgressElement { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the RegionMapElement item in the Dashboard Elements menu of the designer.
            /// </summary>
            public bool ShowRegionMapElement { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the OnlineMapElement item in the Dashboard Elements menu of the designer.
            /// </summary>
            public bool ShowOnlineMapElement { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the ImageElement item in the Dashboard Elements menu of the designer.
            /// </summary>
            public bool ShowImageElement { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the TextElement item in the Dashboard Elements menu of the designer.
            /// </summary>
            public bool ShowTextElement { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the PanelElement item in the Dashboard Elements menu of the designer.
            /// </summary>
            public bool ShowPanelElement { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the ShapeElement item in the Dashboard Elements menu of the designer.
            /// </summary>
            public bool ShowShapeElement { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the ListBoxElement item in the Dashboard Elements menu of the designer.
            /// </summary>
            public bool ShowListBoxElement { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the ComboBoxElement item in the Dashboard Elements menu of the designer.
            /// </summary>
            public bool ShowComboBoxElement { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the TreeViewElement item in the Dashboard Elements menu of the designer.
            /// </summary>
            public bool ShowTreeViewElement { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the TreeViewBoxElement item in the Dashboard Elements menu of the designer.
            /// </summary>
            public bool ShowTreeViewBoxElement { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the DatePickerElement item in the Dashboard Elements menu of the designer.
            /// </summary>
            public bool ShowDatePickerElement { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the CardsElement item in the Dashboard Elements menu of the designer.
            /// </summary>
            public bool ShowCardsElement { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the ButtonElement item in the Dashboard Elements menu of the designer.
            /// </summary>
            public bool ShowButtonElement { get; set; } = true;
        }
        #endregion

        #region Dictionary
        public class DictionaryOptions
        {
            /// <summary>
            /// Gets or sets a visibility of the dictionary in the designer.
            /// </summary>
            public bool Visible { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that using aliases in the dictionary.
            /// </summary>
            public StiUseAliases UseAliases { get; set; } = StiUseAliases.Auto;

            /// <summary>
            /// Gets or sets a value which indicates what to do with the dictionary when creating a new report in the designer.
            /// </summary>
            public StiNewReportDictionary NewReportDictionary { get; set; } = StiNewReportDictionary.Auto;

            /// <summary>
            /// Gets or sets a visibility of the Properties item in the dictionary context menu.
            /// </summary>
            public bool ShowDictionaryContextMenuProperties { get; set; } = true;

            /// <summary>
            /// Gets or sets a value of permissions for datasources in the designer.
            /// </summary>
            public StiDesignerPermissions PermissionDataSources { get; set; } = StiDesignerPermissions.All;

            /// <summary>
            /// Gets or sets a value of permissions for connections in the designer.
            /// </summary>
            public StiDesignerPermissions PermissionDataConnections { get; set; } = StiDesignerPermissions.All;

            /// <summary>
            /// Gets or sets a value of permissions for columns in the designer.
            /// </summary>
            public StiDesignerPermissions PermissionDataColumns { get; set; } = StiDesignerPermissions.All;

            /// <summary>
            /// Gets or sets a value of permissions for relations in the designer.
            /// </summary>
            public StiDesignerPermissions PermissionDataRelations { get; set; } = StiDesignerPermissions.All;

            /// <summary>
            /// Gets or sets a value of permissions for business objects in the designer.
            /// </summary>
            public StiDesignerPermissions PermissionBusinessObjects { get; set; } = StiDesignerPermissions.All;

            /// <summary>
            /// Gets or sets a value of permissions for variables in the designer.
            /// </summary>
            public StiDesignerPermissions PermissionVariables { get; set; } = StiDesignerPermissions.All;

            /// <summary>
            /// Gets or sets a value of permissions for resources in the designer.
            /// </summary>
            public StiDesignerPermissions PermissionResources { get; set; } = StiDesignerPermissions.All;

            /// <summary>
            /// Gets or sets a value of permissions for sql parameters in the designer.
            /// </summary>
            public StiDesignerPermissions PermissionSqlParameters { get; set; } = StiDesignerPermissions.All;
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
            /// Gets or sets the default email address of the message created in the preview.
            /// </summary>
            public string DefaultEmailAddress { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the default subject of the message created in the preview.
            /// </summary>
            public string DefaultEmailSubject { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the default text of the message created in the preview.
            /// </summary>
            public string DefaultEmailMessage { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the default replyTo of the message created in the preview.
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
            /// Gets or sets a value which indicates that the user can save the report from the preview to the report document file.
            /// </summary>
            public bool ShowExportToDocument { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the preview to the PDF format.
            /// </summary>
            public bool ShowExportToPdf { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the preview to the XPS format.
            /// </summary>
            public bool ShowExportToXps { get; set; } = false;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the preview to the Power Point 2007-2010 format.
            /// </summary>
            public bool ShowExportToPowerPoint { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the preview to the HTML format.
            /// </summary>
            public bool ShowExportToHtml { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the preview to the HTML5 format.
            /// </summary>
            public bool ShowExportToHtml5 { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the preview to the MHT (Web Archive) format.
            /// </summary>
            public bool ShowExportToMht { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the preview to the TEXT format.
            /// </summary>
            public bool ShowExportToText { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the preview to the Rich Text format.
            /// </summary>
            public bool ShowExportToRtf { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the preview to the Word 2007-2010 format.
            /// </summary>
            public bool ShowExportToWord2007 { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the preview to the Open Document Text format.
            /// </summary>
            public bool ShowExportToOpenDocumentWriter { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the preview to the Excel BIFF format.
            /// </summary>
            public bool ShowExportToExcel { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the preview to the Excel XML format.
            /// </summary>
            public bool ShowExportToExcelXml { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the preview to the Excel 2007-2010 format.
            /// </summary>
            public bool ShowExportToExcel2007 { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the preview to the Open Document Calc format.
            /// </summary>
            public bool ShowExportToOpenDocumentCalc { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the preview to the CSV format.
            /// </summary>
            public bool ShowExportToCsv { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the preview to the DBF format.
            /// </summary>
            public bool ShowExportToDbf { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the preview to the XML format.
            /// </summary>
            public bool ShowExportToXml { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the preview to the DIF format.
            /// </summary>
            public bool ShowExportToDif { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the preview to the Sylk format.
            /// </summary>
            public bool ShowExportToSylk { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the preview to the Json format.
            /// </summary>
            public bool ShowExportToJson { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the preview to the BMP image format.
            /// </summary>
            public bool ShowExportToImageBmp { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the preview to the GIF image format.
            /// </summary>
            public bool ShowExportToImageGif { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the preview to the JPEG image format.
            /// </summary>
            public bool ShowExportToImageJpeg { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the preview to the PCX image format.
            /// </summary>
            public bool ShowExportToImagePcx { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the preview to the PNG image format.
            /// </summary>
            public bool ShowExportToImagePng { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the preview to the TIFF image format.
            /// </summary>
            public bool ShowExportToImageTiff { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the preview to the Metafile image format.
            /// Not supported in .NET Core
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            [JsonIgnore]
            public bool ShowExportToImageMetafile { get; set; } = false;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the preview to the SVG image format.
            /// </summary>
            public bool ShowExportToImageSvg { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the user can save the report from the preview to the SVGZ image format.
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

        #region File Menu
        public class FileMenuOptions
        {
            /// <summary>
            /// Gets or sets a visibility of the file menu of the designer.
            /// </summary>
            public bool Visible { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the New item in the file menu.
            /// </summary>
            public bool ShowNew { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the Open Report item in the file menu.
            /// </summary>
            public bool ShowOpen { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the Save Report item in the file menu.
            /// </summary>
            public bool ShowSave { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the Save Report As item in the file menu.
            /// </summary>
            public bool ShowSaveAs { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the Close item in the file menu.
            /// </summary>
            public bool ShowClose { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the Exit item in the file menu.
            /// </summary>
            public bool ShowExit { get; set; } = false;

            /// <summary>
            /// Gets or sets a visibility of the Report Setup item in the file menu.
            /// </summary>
            public bool ShowReportSetup { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the Options item in the file menu.
            /// </summary>
            public bool ShowOptions { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the Info item in the file menu.
            /// </summary>
            public bool ShowInfo { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the About item in the file menu.
            /// </summary>
            public bool ShowAbout { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the Help item in the file menu.
            /// </summary>
            public bool ShowHelp { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the new report button in the designer.
            /// </summary>
            public bool ShowFileMenuNewReport { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the new dashboard button in the designer.
            /// </summary>
            public bool ShowFileMenuNewDashboard { get; set; } = true;
        }
        #endregion

        #region Preview Toolbar
        public class PreviewToolbarOptions
        {
            /// <summary>
            /// Gets or sets a value which indicates that toolbar will be shown in the preview.
            /// </summary>
            public bool Visible { get; set; } = true;

            /// <summary>
            /// Gets or sets the display mode of the preview toolbar - simple or separated into upper and lower parts.
            /// </summary>
            public StiToolbarDisplayMode DisplayMode { get; set; } = StiToolbarDisplayMode.Simple;

            /// <summary>
            /// Gets or sets a color of the preview toolbar background. The default value is the theme color.
            /// </summary>
            public Color BackgroundColor { get; set; } = Color.Empty;

            /// <summary>
            /// Gets or sets a color of the preview toolbar border. The default value is the theme color.
            /// </summary>
            public Color BorderColor { get; set; } = Color.Empty;

            /// <summary>
            /// Gets or sets a color of the preview toolbar texts.
            /// </summary>
            public Color FontColor { get; set; } = Color.Empty;

            /// <summary>
            /// Gets or sets a value which indicates which font family will be used for drawing texts in the preview.
            /// </summary>
            public string FontFamily { get; set; } = "Arial";

            /// <summary>
            /// Gets or sets the alignment of the preview toolbar.
            /// </summary>
            public StiContentAlignment Alignment { get; set; } = StiContentAlignment.Default;

            /// <summary>
            /// Gets or sets a value which allows displaying or hiding preview toolbar buttons captions.
            /// </summary>
            public bool ShowButtonCaptions { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the Print button in the toolbar of the preview.
            /// </summary>
            public bool ShowPrintButton { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the Open button in the toolbar of the preview.
            /// </summary>
            public bool ShowOpenButton { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the Save button in the toolbar of the preview.
            /// </summary>
            public bool ShowSaveButton { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the Send Email button in the toolbar of the preview.
            /// </summary>
            public bool ShowSendEmailButton { get; set; } = false;

            /// <summary>
            /// Gets or sets a visibility of the Find button in the toolbar of the preview.
            /// </summary>
            public bool ShowFindButton { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the Signature button in the toolbar of the preview.
            /// </summary>
            public bool ShowSignatureButton { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the Bookmarks button in the toolbar of the preview.
            /// </summary>
            public bool ShowBookmarksButton { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the Parameters button in the toolbar of the preview.
            /// </summary>
            public bool ShowParametersButton { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the Editor button in the toolbar of the preview.
            /// </summary>
            public bool ShowEditorButton { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the First Page button in the toolbar of the preview.
            /// </summary>
            public bool ShowFirstPageButton { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the Prev Page button in the toolbar of the preview.
            /// </summary>
            public bool ShowPreviousPageButton { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the current page control in the toolbar of the preview.
            /// </summary>
            public bool ShowCurrentPageControl { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the Next Page button in the toolbar of the preview.
            /// </summary>
            public bool ShowNextPageButton { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the Last Page button in the toolbar of the preview.
            /// </summary>
            public bool ShowLastPageButton { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the Zoom control in the toolbar of the preview.
            /// </summary>
            public bool ShowZoomButton { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the View Mode button in the toolbar of the preview.
            /// </summary>
            public bool ShowViewModeButton { get; set; } = true;

            /// <summary>
            /// Gets or sets the default mode of the report print destination.
            /// </summary>
            public StiPrintDestination PrintDestination { get; set; } = StiPrintDestination.Default;

            /// <summary>
            /// Gets or sets the mode of showing a report in the preview - Single Page, Continuous or Multiple Pages.
            /// </summary>
            public StiWebViewMode ViewMode { get; set; } = StiWebViewMode.SinglePage;
        }
        #endregion

        #region Properties Grid
        public class PropertiesGridOptions
        {
            /// <summary>
            /// Gets or sets a visibility of the properties grid in the designer.
            /// </summary>
            public bool Visible { get; set; } = true;

            /// <summary>
            /// Gets or sets a width of the properties grid in the designer.
            /// </summary>
            public int Width { get; set; } = 370;

            /// <summary>
            /// Gets or sets a width of the property label in properties grid in the designer.
            /// </summary>
            public int LabelWidth { get; set; } = 160;

            /// <summary>
            /// Gets or sets a position of the properties grid.
            /// </summary>
            public StiPropertiesGridPosition PropertiesGridPosition { get; set; } = StiPropertiesGridPosition.Left;

            /// <summary>
            /// Gets or sets a visibility of the properties which used from styles in the designer.
            /// </summary>
            public bool ShowPropertiesWhichUsedFromStyles { get; set; } = false;
        }
        #endregion
        
        #region Server
        public class ServerOptions
        {
            /// <summary>
            /// Gets or sets the name of the query processing controller of the report designer. 
            /// </summary>
            public string Controller { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the URL pattern of the route of the report designer. The {action} parameter can be used for the component actions, for example: /Home/{action}
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
            /// Allows the designer to update the cache automatically. The cache will be updated about once every three minutes if there are no actions in the designer.
            /// </summary>
            public bool AllowAutoUpdateCache { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that the designer will use relative or absolute URLs.
            /// </summary>
            public bool UseRelativeUrls { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which specifies the port number to use in the URL. A value of 0 defines automatic detection. A value of -1 removes the port number.
            /// </summary>
            public int PortNumber { get; set; } = 0;

            /// <summary>
            /// Gets or sets a value which enables or disables the transfer of URL parameters when requesting the scripts and styles of the designer.
            /// </summary>
            public bool PassQueryParametersForResources { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which enables or disables the transfer POST parameters of the form.
            /// </summary>
            public bool PassFormValues { get; set; }

            /// <summary>
            /// Gets or sets a value which enables or disables the display of the detailed server error in the preview.
            /// </summary>
            public bool ShowServerErrorPage { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which enables or disables the GZip compression of requests to the server. This allows to reduce the volume of Internet traffic, but slightly slows down the designer actions.
            /// </summary>
            public bool UseCompression { get; set; }

            /// <summary>
            /// Gets or sets a value which enables caching for designer scripts and styles.
            /// </summary>
            public bool UseCacheForResources { get; set; } = true;

            /// <summary>
            /// Allows the designer to update the cookies automatically on every request to the server. By default, cookies are set when creating the designer, if they are not specified in the report.
            /// </summary>
            public bool AllowAutoUpdateCookies { get; set; }

            /// <summary>
            /// Allow the designer to automatically request and send the antiforgery token.
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
            /// Gets or sets a value which indicates that toolbar will be shown in the designer.
            /// </summary>           
            public bool ShowToolbar { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the setup toolbox button in the designer.
            /// </summary>
            public bool ShowSetupToolboxButton { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the insert button in the toolbar of the designer.
            /// </summary>
            public bool ShowInsertButton { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the layout button in the toolbar of the designer.
            /// </summary>
            public bool ShowLayoutButton { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the page button in the toolbar of the designer.
            /// </summary>
            public bool ShowPageButton { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the preview button in the toolbar of the designer.
            /// </summary>
            public bool ShowPreviewButton { get; set; } = true;

            /// <summary>
            /// Gets or sets a visibility of the save button in the toolbar of the designer.
            /// </summary>
            public bool ShowSaveButton { get; set; } = false;

            /// <summary>
            /// Gets or sets a visibility of the about button in the toolbar of the designer.
            /// </summary>
            public bool ShowAboutButton { get; set; } = false;

            #region Hidden

            [EditorBrowsable(EditorBrowsableState.Never)]
            public StiDesignerComponents[] ComponentsIntoInsertTab { get; set; } = null;

            #endregion
        }
        #endregion

        #region StiNetCoreDesignerOptions

        /// <summary>
        /// A class which controls settings of the designer actions.
        /// </summary>
        public ActionOptions Actions { get; set; } = new ActionOptions();

        /// <summary>
        /// A class which controls settings of the designer appearance.
        /// </summary>
        public AppearanceOptions Appearance { get; set; } = new AppearanceOptions();

        /// <summary>
        /// A class which controls settings of the pages.
        /// </summary>
        public PagesOptions Pages { get; set; } = new PagesOptions();

        /// <summary>
        /// A class which allows gets or sets a visibility of the specified item in the Bands menu of the designer.
        /// </summary>
        public BandsOptions Bands { get; set; } = new BandsOptions();

        /// <summary>
        /// A class which controls settings of the designer behavior.
        /// </summary>
        public BehaviorOptions Behavior { get; set; } = new BehaviorOptions();

        /// <summary>
        /// A class which allows gets or sets a visibility of the specified item in the Components menu of the designer.
        /// </summary>
        public ComponentsOptions Components { get; set; } = new ComponentsOptions();

        /// <summary>
        /// A class which allows gets or sets a visibility of the specified item in the CrossBands menu of the designer.
        /// </summary>
        public CrossBandsOptions CrossBands { get; set; } = new CrossBandsOptions();

        /// <summary>
        /// A class which controls settings of the dashboards.
        /// </summary>
        public DashboardsOptions Dashboards { get; set; } = new DashboardsOptions();

        /// <summary>
        /// A class which allows gets or sets a visibility of the specified item in the Components menu of the designer.
        /// </summary>
        public DashboardElementsOptions DashboardElements { get; set; } = new DashboardElementsOptions();

        /// <summary>
        /// A class which controls settings of the designer dictionary.
        /// </summary>
        public DictionaryOptions Dictionary { get; set; } = new DictionaryOptions();

        /// <summary>
        /// A class which controls the email options.
        /// </summary>
        public EmailOptions Email { get; set; } = new EmailOptions();

        /// <summary>
        /// A class which controls the export options.
        /// </summary>
        public ExportOptions Exports { get; set; } = new ExportOptions();

        /// <summary>
        /// A class which controls settings of the designer file menu.
        /// </summary>
        public FileMenuOptions FileMenu { get; set; } = new FileMenuOptions();

        /// <summary>
        /// A class which controls settings of the preview toolbar.
        /// </summary>
        public PreviewToolbarOptions PreviewToolbar { get; set; } = new PreviewToolbarOptions();

        /// <summary>
        /// A class which controls settings of the designer properties grid.
        /// </summary>
        public PropertiesGridOptions PropertiesGrid { get; set; } = new PropertiesGridOptions();

        /// <summary>
        /// A class which controls the server options.
        /// </summary>
        public ServerOptions Server { get; set; } = new ServerOptions();

        /// <summary>
        /// A class which controls settings of the designer toolbar.
        /// </summary>
        public ToolbarOptions Toolbar { get; set; } = new ToolbarOptions();

        /// <summary>
        /// Gets or sets the current visual theme which is used for drawing visual elements of the designer.
        /// </summary>
        public StiDesignerTheme Theme { get; set; } = StiDesignerTheme.Office2022WhiteBlue;

        /// <summary>
        /// Gets or sets a path to the localization file for the designer.
        /// </summary>
        public string Localization { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a path to the localization directory for the designer.
        /// </summary>
        public string LocalizationDirectory { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the width of the designer.
        /// </summary>
        public Unit Width { get; set; } = Unit.Empty;

        /// <summary>
        /// Gets or sets the height of the designer.
        /// </summary>
        public Unit Height { get; set; } = Unit.Empty;

        #endregion
    }
}
