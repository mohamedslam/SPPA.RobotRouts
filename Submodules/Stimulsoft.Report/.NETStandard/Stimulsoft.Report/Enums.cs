#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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

using System;
using System.ComponentModel;
using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Converters;

namespace Stimulsoft.Report
{
    #region StiParserType
    public enum StiParserType
    {
        ReportParser,
        DataParser
    }
    #endregion

    #region StiLoadThemeMode
    public enum StiLoadThemeMode
    {
        OnceForTheWholeApp,
        ForEveryOpenedWindow
    }
    #endregion
        
    #region StiDesignerScaleMode
    /// <summary>
    /// A mode of the UI scaling in the Designer.
    /// </summary>
    public enum StiDesignerScaleMode
    {
        AutomaticScaling,
        Scaling100        
    }
    #endregion

    #region StiOrientation
    public enum StiOrientation
    {
        Horizontal,
        Vertical
    }
    #endregion

    #region StiResizeReportOptions
    [Flags]
    public enum StiResizeReportOptions
    {
        ProcessAllPages = 1,
        RebuildReport = 2,
        RescaleContent = 4,
        PageOrientationChanged = 8,
        ShowProgressOnRebuildReport = 16,
        AllowPageMarginsRescaling = 32
    }
    #endregion

    #region StiCalculationMode
    public enum StiCalculationMode
    {
        Compilation,
        Interpretation
    }
    #endregion

    #region StiReportLanguageType
    /// <summary>
	/// Languages of the report.
	/// </summary>
	public enum StiReportLanguageType
    {
        /// <summary>
        /// Visual C# .Net language.
        /// </summary>
        CSharp,
        /// <summary>
        /// Visual Basic .Net language.
        /// </summary>
        VB,
        /// <summary>
        /// Javascript language.
        /// </summary>
        JS
    }
    #endregion

    #region StiReportUnitType
    /// <summary>
    /// Units of the report.
    /// </summary>
    public enum StiReportUnitType
    {
        /// <summary>
        /// Centimeters as report units.
        /// </summary>
        Centimeters,
        /// <summary>
        /// Hundredth of inches as Report units.
        /// </summary>
        HundredthsOfInch,
        /// <summary>
        /// Inches as report units.
        /// </summary>
        Inches,
        /// <summary>
        /// Millimeters as Report units.
        /// </summary>
        Millimeters
    }
    #endregion

    #region StiViewMode
    /// <summary>
    /// Modes for showing a page in the designer.
    /// </summary>
    public enum StiViewMode
    {
        /// <summary>
        /// Normal view mode.
        /// </summary>
        Normal,
        /// <summary>
        /// PageBreak preview mode. 
        /// </summary>
        PageBreakPreview
    }
    #endregion

    #region StiGridMode
    /// <summary>
    /// Modes for drawing a grid in the designer.
    /// </summary>
    public enum StiGridMode
    {
        /// <summary>
        /// Lines mode.
        /// </summary>
        Lines,
        /// <summary>
        /// Dots mode.
        /// </summary>
        Dots
    }
    #endregion

    #region StiExportFormat
    /// <summary>
    /// Modes for formats the report to be exported to.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StiExportFormat
    {
        /// <summary>
        /// Export will not be done. 
        /// </summary>
        None = 0,

        /// <summary>
        /// Adobe PDF format for export.
        /// </summary>
        Pdf = 1,

        /// <summary>
        /// Microsoft Xps format for export.
        /// </summary>
        Xps = 2,

        /// <summary>
        /// HTML Table format for export.
        /// </summary>
        HtmlTable = 3,

        /// <summary>
        /// HTML Span format for export.
        /// </summary>
        HtmlSpan = 4,

        /// <summary>
        /// HTML Div format for export.
        /// </summary>
        HtmlDiv = 5,

        /// <summary>
        /// RTF format for export.
        /// </summary>
        Rtf = 6,

        /// <summary>
        /// Table in Rtf format for export. 
        /// </summary>
        RtfTable = 7,

        /// <summary>
        /// Components of the report will be placed into RTF frames for export.
        /// </summary>
        RtfFrame = 8,

        /// <summary>
        /// Components of the report will be placed into RTF frames with borders in Microsoft Word graphic format for export.
        /// </summary>
        RtfWinWord = 9,

        /// <summary>
        /// Mode for export to the RTF format with Tab symbol as delimiter of the text.
        /// </summary>
        RtfTabbedText = 10,

        /// <summary>
        /// Please use StiExportFormat.RtfFrame instead.
        /// </summary>
        [Obsolete("Please use StiExportFormat.RtfFrame")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        RtfMode1 = 8,

        /// <summary>
        /// StiExportFormat.RtfWinWord should be used instead.
        /// </summary>
        [Obsolete("Please use StiExportFormat.RtfWinWord")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        RtfMode2 = 9,

        /// <summary>
        /// StiExportFormat.RtfTabbedText should be used instead.
        /// </summary>
        [Obsolete("Please use StiExportFormat.RtfTabbedText")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        RtfMode3 = 10,

        /// <summary>
        /// Text format for export.
        /// </summary>
        Text = 11,

        /// <summary>
        /// Excel BIFF (Binary Interchange File Format) format for export.
        /// </summary>
        Excel = 12,

        /// <summary>
        /// Excel Xml format for export.
        /// </summary>
        ExcelXml = 13,

        /// <summary>
        /// Excel 2007 format for export.
        /// </summary>
        Excel2007 = 14,

        /// <summary>
        /// Word 2007 format for export.
        /// </summary>
        Word2007 = 15,

        /// <summary>
        /// Xml format for export.
        /// </summary>
        Xml = 16,

        /// <summary>
        /// CSV (Comma Separated Value) file format for export.
        /// </summary>
        Csv = 17,

        /// <summary>
        /// Dif file format for export.
        /// </summary>
        Dif = 18,

        /// <summary>
        /// Sylk file format for export.
        /// </summary>
        Sylk = 19,

        /// <summary>
        /// Image format for export.
        /// </summary>
        Image = 20,

        /// <summary>
        /// Image in GIF format for export.
        /// </summary>
        ImageGif = 21,

        /// <summary>
        /// Image in BMP format for export.
        /// </summary>
        ImageBmp = 22,

        /// <summary>
        /// Image in PNG format for export.
        /// </summary>
        ImagePng = 23,

        /// <summary>
        /// Image in TIFF format for export.
        /// </summary>
        ImageTiff = 24,

        /// <summary>
        /// Image in JPEG format for export.
        /// </summary>
        ImageJpeg = 25,

        /// <summary>
        /// Image in PCX format for export.
        /// </summary>
        ImagePcx = 26,

        /// <summary>
        /// Image in EMF format for export.
        /// </summary>
        ImageEmf = 27,

        /// <summary>
        /// Image in SVG format for export.
        /// </summary>
        ImageSvg = 28,

        /// <summary>
        /// Image in SVGZ format for export.
        /// </summary>
        ImageSvgz = 29,

        /// <summary>
        /// WebArchive format for export.
        /// </summary>
        Mht = 30,

        /// <summary>
        /// dBase/FoxPro format for export.
        /// </summary>
        Dbf = 31,

        /// <summary>
        /// HTML format for export.
        /// </summary>
        Html = 32,

        /// <summary>
        /// OpenDocument Calc file
        /// </summary>
        Ods = 33,

        /// <summary>
        /// OpenDocument Writer file
        /// </summary>
        Odt = 34,

        /// <summary>
        /// PowerPoint 2007 format for export
        /// </summary>
        Ppt2007 = 35,

        /// <summary>
        /// HTML5 format for export.
        /// </summary>
        Html5 = 36,

        /// <summary>
        /// Universal format for all data type of exports.
        /// </summary>
        Data = 37,

        /// <summary>
        /// JSON format for export.
        /// </summary>
        Json = 38,

        /// <summary>
        /// Document MDC file.
        /// </summary>
        Document = 1000
    }
    #endregion

    #region StiPreviewMode
    public enum StiPreviewMode
    {
        /// <summary>
        /// Standard viewer window for the report rendering.
        /// </summary>
        Standard,

        /// <summary>
        /// Standard viewer window and Dot-Matrix viewer window for the report rendering. Standard mode is default.
        /// </summary>
        StandardAndDotMatrix,

        /// <summary>
        /// Dot-Matrix viewer window for the report rendering.
        /// </summary>
        DotMatrix,
    }
    #endregion

    #region StiReportCacheMode
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StiReportCacheMode
    {
        Off,
        On,
        Auto,
    }
    #endregion

    #region StiReportCacheThreadMode
    public enum StiReportCacheThreadMode
    {
        Off,
        On,
        Auto,
    }
    #endregion

    #region StiReportPass
    public enum StiReportPass
    {
        None,
        First,
        Second
    }
    #endregion

    #region StiNumberOfPass
    public enum StiNumberOfPass
    {
        SinglePass,
        DoublePass
    }
    #endregion

    #region StiStandaloneReportType
    public enum StiStandaloneReportType
    {
        Show,
        Print,
        ShowWithWpf,
        PrintWithWpf,
    }
    #endregion

    #region StiRangeType
    /// <summary>
    /// Range types of pages.
    /// </summary>
    public enum StiRangeType
    {
        /// <summary>
        /// All pages.
        /// </summary>
        All = 1,
        /// <summary>
        /// Current page.
        /// </summary>
        CurrentPage = 2,
        /// <summary>
        /// Range of pages.
        /// </summary>
        Pages = 3
    }
    #endregion

    #region StiArabicDigitsType
    /// <summary>
    /// Enumeration for the representation of arabic digits.
    /// </summary>
    public enum StiArabicDigitsType
    {
        /// <summary>
        /// A value for the standard arabic digits.
        /// </summary>
        Standard,
        /// <summary>
        /// A value for the Eastern arabic digits.
        /// </summary>
        Eastern
    }
    #endregion

    #region StiRightToLeftType
    /// <summary>
    /// Enumeration which controls of output of objects in the right to left mode.
    /// </summary>
    public enum StiRightToLeftType
    {
        /// <summary>
        /// Use the right to left mode.
        /// </summary>
        Yes,
        /// <summary>
        /// Do not use the right to left mode.
        /// </summary>
        No
    }
    #endregion

    #region StiStyleElements
    [Flags]
    public enum StiStyleElements
    {
        Font = 1,
        Border = 2,
        Brush = 4,
        TextBrush = 8,
        TextOptions = 16,
        HorAlignment = 32,
        VertAlignment = 64,
        All = 127
    }
    #endregion

    #region StiGlobalGuiStyle
    public enum StiGlobalGuiStyle
    {
        [Obsolete("A 'StiGlobalGuiStyle.Office2000' value is deprecated. Please remove it from your code!")]
        Office2000,

        [Obsolete("A 'StiGlobalGuiStyle.OfficeXP' value is deprecated. Please remove it from your code!")]
        OfficeXP,

        [Obsolete("A 'StiGlobalGuiStyle.Office2003Blue' value is deprecated. Please remove it from your code!")]
        Office2003Blue,

        [Obsolete("A 'StiGlobalGuiStyle.Office2003Black' value is deprecated. Please remove it from your code!")]
        Office2003Black,

        [Obsolete("A 'StiGlobalGuiStyle.Office2003Silver' value is deprecated. Please remove it from your code!")]
        Office2003Silver,

        [Obsolete("A 'StiGlobalGuiStyle.Office2007Blue' value is deprecated. Please remove it from your code!")]
        Office2007Blue,

        [Obsolete("A 'StiGlobalGuiStyle.Office2007Black' value is deprecated. Please remove it from your code!")]
        Office2007Black,

        [Obsolete("A 'StiGlobalGuiStyle.Office2007Silver' value is deprecated. Please remove it from your code!")]
        Office2007Silver,

        [Obsolete("A 'StiGlobalGuiStyle.Office2010Blue' value is deprecated. Please remove it from your code!")]
        Office2010Blue,

        [Obsolete("A 'StiGlobalGuiStyle.Office2010Black' value is deprecated. Please remove it from your code!")]
        Office2010Black,

        [Obsolete("A 'StiGlobalGuiStyle.Office2010Silver' value is deprecated. Please remove it from your code!")]
        Office2010Silver,

        Office2013,

        [Obsolete("A 'StiGlobalGuiStyle.Windows7' value is deprecated. Please remove it from your code!")]
        Windows7,

        [Obsolete("A 'StiGlobalGuiStyle.Vista' value is deprecated. Please remove it from your code!")]
        Vista
    }
    #endregion

    #region StiGuiStandardStyle
    public enum StiGuiStandardStyle
    {
        Default,
        Office2007,
        Office2003,
        OfficeXP
    }
    #endregion

    #region StiGuiStyle
    public enum StiGuiStyle
    {
        Default,
        Office2007Blue,
        Office2007Black,
        Office2007Silver,
        Office2003,
        OfficeXP
    }
    #endregion

    #region StiGuiRibbonStyle
    public enum StiGuiRibbonStyle
    {
        Blue,
        Silver,
        Black
    }
    #endregion

    #region StiBrushType
    public enum StiBrushType
    {
        Solid,
        Glare,
        Gradient0,
        Gradient90,
        Gradient180,
        Gradient270,
        Gradient45,
    }
    #endregion

    #region StiOptionsUniversalType
    public enum StiOptionsUniversalType
    {
        Enum
    }
    #endregion

    #region StiInteractionType
    public enum StiInteractionType
    {
        Sorting,
        DrillDownPage,
        Collapsing
    }
    #endregion

    #region StiComponentId
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StiComponentId
    {
        StiComponent = 0,
        StiBarCode,
        StiButtonControl,
        StiChart,
        StiSparkline,
        StiChartCommon,
        StiCheckBox,
        StiCheckBoxControl,
        StiCheckedListBoxControl,
        StiChildBand,
        StiClone,
        StiColumnFooterBand,
        StiColumnHeaderBand,
        StiComboBoxControl,
        StiContainer,
        StiContourText,
        StiCrossColumn,
        StiCrossColumnTotal,
        StiCrossDataBand,
        StiCrossFooterBand,
        StiCrossGroupFooterBand,
        StiCrossGroupHeaderBand,
        StiCrossHeaderBand,
        StiCrossRow,
        StiCrossRowTotal,
        StiCrossSummary,
        StiCrossTab,
        StiCrossTitle,
        StiDashboardPage,
        StiDataBand,
        StiDateTimePickerControl,
        StiEmptyBand,
        StiFooterBand,
        StiForm,
        StiGridControl,
        StiGroupBoxControl,
        StiGroupFooterBand,
        StiGroupHeaderBand,
        StiHeaderBand,
        StiHierarchicalBand,
        StiHorizontalLinePrimitive,
        StiImage,
        StiLabelControl,
        StiListBoxControl,
        StiListViewControl,
        StiLookUpBoxControl,
        StiNumericUpDownControl,
        StiOverlayBand,
        StiPage,
        StiPageFooterBand,
        StiPageHeaderBand,
        StiPanel,
        StiPanelControl,
        StiPictureBoxControl,
        StiRadioButtonControl,
        StiRectanglePrimitive,
        StiReportControl,
        StiReportSummaryBand,
        StiReportTitleBand,
        StiRichText,
        StiRichTextBoxControl,
        StiRoundedRectanglePrimitive,
        StiShape,
        StiSubReport,
        StiSystemText,
        StiTable,
        StiTableCell,
        StiText,
        StiTextBoxControl,
        StiTextInCells,
        StiTreeViewControl,
        StiVerticalLinePrimitive,
        StiWinControl,
        StiUndefinedComponent,
        StiZipCode,
        StiElectronicSignature,
        StiPdfDigitalSignature,
        StiTableCellCheckBox,
        StiTableCellImage,
        StiTableCellRichText,
        StiDataColumn,
        StiCalcDataColumn,
        StiBusinessObject,
        StiDataSource,
        StiDataStoreSource,
        StiFileDataSource,
        StiDataRelation,
        StiVariable,
        StiResource,
        StiReport,
        StiStyle,
        StiCrossTabStyle,
        StiChartStyle,
        StiMapStyle,
        StiTableStyle,
        StiCardsStyle,
        StiGaugeStyle,
        StiIndicatorStyle,
        StiProgressStyle,
        StiDialogStyle,
        StiDataParameter,
        StiCrossField,
        StiCrossTotal,
        StiCrossCell,
        StiCrossHeader,
        StiCrossSummaryHeader,
        StiStartPointPrimitive,
        StiEndPointPrimitive,
        StiEvent,
        StiTableOfContents,

        #region DBS
        StiChartElement,
        StiGaugeElement,
        StiImageElement,
        StiIndicatorElement,
        StiRegionMapElement,
        StiOnlineMapElement,
        StiTableElement,
        StiPivotTableElement,
        StiProgressElement,
        StiTextElement,
        StiPanelElement,
        StiShapeElement,
        StiTreeViewElement,
        StiTreeViewBoxElement,
        StiListBoxElement,
        StiComboBoxElement,
        StiDatePickerElement,
        StiDateRangeElement,
        StiButtonElement,
        StiCardsElement,

        StiDashboard,
        #endregion

        #region APPS
        StiButtonUI,
        StiCheckBoxUI,

        StiScreen,
        #endregion

        #region Charts
        Sti3dOptions,
        StiSeries,
        StiBubbleSeries,
        StiClusteredColumnSeries,
        StiClusteredColumnSeries3D,
        StiHistogramSeries,
        StiParetoSeries,
        StiLineSeries,
        StiSteppedLineSeries,
        StiSplineSeries,
        StiAreaSeries,
        StiRibbonSeries,
        StiSteppedAreaSeries,
        StiSplineAreaSeries,
        StiStackedColumnSeries,
        StiStackedColumnSeries3D,
        StiStackedLineSeries,
        StiStackedSplineSeries,
        StiStackedAreaSeries,
        StiStackedSplineAreaSeries,
        StiFullStackedColumnSeries,
        StiFullStackedColumnSeries3D,
        StiFullStackedLineSeries,
        StiFullStackedAreaSeries,
        StiFullStackedSplineSeries,
        StiFullStackedSplineAreaSeries,
        StiClusteredBarSeries,
        StiStackedBarSeries,
        StiTreemapSeries,
        StiSunburstSeries,
        StiPictorialSeries,
        StiPictorialStackedSeries,
        StiFullStackedBarSeries,
        StiPieSeries,
        StiPie3dSeries,
        StiDoughnutSeries,
        StiWaterfallSeries,
        StiGanttSeries,
        StiScatterSeries,
        StiScatterLineSeries,
        StiScatterSplineSeries,
        StiRadarAreaSeries,
        StiRadarLineSeries,
        StiRadarPointSeries,
        StiRangeSeries,
        StiSteppedRangeSeries,
        StiFunnelSeries,
        StiFunnelWeightedSlicesSeries,
        StiRangeBarSeries,
        StiSplineRangeSeries,
        StiCandlestickSeries,
        StiStockSeries,
        StiBoxAndWhiskerSeries,
        StiChartTitle,
        StiLineMarker,
        StiMarker,
        StiChartTable,
        StiSeriesTopN,
        StiSeriesInteraction,
        StiTrendLine,
        StiSeriesLabels,
        StiNoneLabels,
        StiInsideEndAxisLabels,
        StiInsideBaseAxisLabels,
        StiCenterTreemapLabels,
        StiCenterAxisLabels,
        StiOutsideEndAxisLabels,
        StiOutsideBaseAxisLabels,
        StiOutsideAxisLabels,
        StiLeftAxisLabels,
        StiValueAxisLabels,
        StiRightAxisLabels,
        StiCenterFunnelLabels,
        StiCenterPieLabels,
        StiCenterPie3dLabels,
        StiOutsidePieLabels,
        StiTwoColumnsPieLabels,
        StiOutsideLeftFunnelLabels,
        StiOutsideRightFunnelLabels,
        StiLegend,
        StiClusteredColumnArea,
        StiClusteredColumnArea3D,
        StiWaterfallArea,
        StiHistorgamArea,
        StiPieArea,
        StiPie3dArea,
        StiTreemapArea,
        StiSunburstArea,
        StiFunnelArea,
        StiFunnelWeightedSlicesArea,
        StiPictorialArea,
        StiPictorialStackedArea,
        StiRadarAreaArea,
        StiRadarLineArea,
        StiRadarPointArea,
        StiRibbonArea,
        StiStackedColumnArea,
        StiStackedColumnArea3D,
        StiGridLines,
        StiInterlacing,
        StiXAxis,
        StiXAxis3D,
        StiXTopAxis,
        StiYAxis,
        StiYAxis3D,
        StiYRightAxis,
        StiRadarGridLines,
        StiXRadarAxis,
        StiYRadarAxis,
        #endregion

        #region StiDialogInfoItem
        StiDialogInfoItem,
        StiStringDialogInfoItem,
        StiGuidDialogInfoItem,
        StiCharDialogInfoItem,
        StiBoolDialogInfoItem,
        StiImageDialogInfoItem,
        StiDateTimeDialogInfoItem,
        StiTimeSpanDialogInfoItem,
        StiDoubleDialogInfoItem,
        StiDecimalDialogInfoItem,
        StiLongDialogInfoItem,
        StiExpressionDialogInfoItem,
        StiStringRangeDialogInfoItem,
        StiGuidRangeDialogInfoItem,
        StiByteArrayRangeDialogInfoItem,
        StiCharRangeDialogInfoItem,
        StiDateTimeRangeDialogInfoItem,
        StiTimeSpanRangeDialogInfoItem,
        StiDoubleRangeDialogInfoItem,
        StiDecimalRangeDialogInfoItem,
        StiLongRangeDialogInfoItem,
        StiExpressionRangeDialogInfoItem,
        #endregion

        OracleConnectionStringBuilder,
        StiStrips,
        StiConstantLines,

        #region ShapeType

        StiShapeTypeService,
        StiDiagonalDownLineShapeType,
        StiRoundedRectangleShapeType,
        StiTriangleShapeType,
        StiComplexArrowShapeType,
        StiBentArrowShapeType,
        StiChevronShapeType,
        StiEqualShapeType,
        StiFlowchartCollateShapeType,
        StiFlowchartOffPageConnectorShapeType,
        StiArrowShapeType,
        StiOctagonShapeType,

        #endregion

        #region BarCodes

        StiAustraliaPost4StateBarCodeType,
        StiAztecBarCodeType,
        StiIntelligentMail4StateBarCodeType,
        StiCode11BarCodeType,
        StiCode128aBarCodeType,
        StiCode128bBarCodeType,
        StiCode128cBarCodeType,
        StiCode128AutoBarCodeType,
        StiCode39BarCodeType,
        StiCode39ExtBarCodeType,
        StiCode93BarCodeType,
        StiCode93ExtBarCodeType,
        StiCodabarBarCodeType,
        StiEAN128aBarCodeType,
        StiEAN128bBarCodeType,
        StiEAN128cBarCodeType,
        StiEAN128AutoBarCodeType,
        StiGS1_128BarCodeType,
        StiEAN13BarCodeType,
        StiEAN8BarCodeType,
        StiFIMBarCodeType,
        StiIsbn10BarCodeType,
        StiIsbn13BarCodeType,
        StiITF14BarCodeType,
        StiJan13BarCodeType,
        StiJan8BarCodeType,
        StiMsiBarCodeType,
        StiPdf417BarCodeType,
        StiPharmacodeBarCodeType,
        StiPlesseyBarCodeType,
        StiPostnetBarCodeType,
        StiQRCodeBarCodeType,
        StiRoyalMail4StateBarCodeType,
        StiDutchKIXBarCodeType,
        StiSSCC18BarCodeType,
        StiUpcABarCodeType,
        StiUpcEBarCodeType,
        StiUpcSup2BarCodeType,
        StiUpcSup5BarCodeType,
        StiInterleaved2of5BarCodeType,
        StiStandard2of5BarCodeType,
        StiDataMatrixBarCodeType,
        StiMaxicodeBarCodeType,
        StiGS1DataMatrixBarCodeType,
        StiGS1QRCodeBarCodeType,

        #endregion

        #region StiDatabase
        StiDatabase,
        StiFileDatabase,
        StiCsvDatabase,
        StiDBaseDatabase,
        StiExcelDatabase,
        StiJsonDatabase,
        StiGisDatabase,
        StiXmlDatabase,
        StiSqlDatabase,
        StiFirebaseDatabase,
        StiGraphQLDatabase,
        StiBigQueryDatabase,
        StiGoogleAnalyticsDatabase,
        StiAzureBlobStorageDatabase,
        #endregion

        #region Form
        StiFormContainer,
        #endregion

        StiGauge,
        StiMap,
        StiMathFormula,
        StiFullStackedColumnArea,
        StiFullStackedColumnArea3D,
        StiClusteredBarArea,
        StiStackedBarArea,
        StiFullStackedBarArea,
        StiDoughnutArea,
        StiLineArea,
        StiParetoArea,
        StiSteppedLineArea,
        StiStackedLineArea,
        StiFullStackedLineArea,
        StiSplineArea,
        StiStackedSplineArea,
        StiFullStackedSplineArea,
        StiAreaArea,
        StiSteppedAreaArea,
        StiStackedAreaArea,
        StiFullStackedAreaArea,
        StiSplineAreaArea,
        StiStackedSplineAreaArea,
        StiFullStackedSplineAreaArea,
        StiGanttArea,
        StiScatterArea,
        StiBubbleArea,
        StiRangeArea,
        StiSteppedRangeArea,
        StiRangeBarArea,
        StiSplineRangeArea,
        StiCandlestickArea,
        StiBoxAndWhiskerArea,
        StiStockArea,
        StiInsideEndPieLabels,
        StiTrendLineNone,
        StiTrendLineLinear,
        StiTrendLineExponential,
        StiTrendLineLogarithmic,
        StiDB2Database,
        StiDotConnectUniversalDatabase,
        StiFirebirdDatabase,
        StiInformixDatabase,
        StiMongoDbDatabase,
        StiDataWorldDatabase,
        StiQuickBooksDatabase,
        StiCosmosDbDatabase,
        StiAzureTableStorageDatabase,
        StiAzureBlobStorageSource,
        StiGoogleSheetsDatabase,
        StiMariaDbDatabase,
        StiMySqlDatabase,
        StiMSAccessDatabase,
        StiOdbcDatabase,
        StiOleDbDatabase,
        StiOracleDatabase,
        StiPostgreSQLDatabase,
        StiSQLiteDatabase,
        StiSqlCeDatabase,
        StiSybaseDatabase,
        StiSybaseAdsDatabase,
        StiTeradataDatabase,
        StiVistaDBDatabase,
        StiODataDatabase,
        StiDataTableSource,
        StiDataViewSource,
        StiUndefinedDataSource,
        StiCsvSource,
        StiDBaseSource,
        StiBusinessObjectSource,
        StiCrossTabDataSource,
        StiEnumerableSource,
        StiUserSource,
        StiVirtualSource,
        StiDataTransformation,
        StiOracleODPSource,
        StiFirebirdSource,
        StiInformixSource,
        StiMongoDbSource,
        StiDataWorldSource,
        StiQuickBooksSource,
        StiCosmosDbSource,
        StiAzureTableStorageSource,
        StiMSAccessSource,
        StiMySqlSource,
        StiMariaDbSource,
        StiOdbcSource,
        StiOleDbSource,
        StiOracleSource,
        StiPostgreSQLSource,
        StiSqlCeSource,
        StiSQLiteSource,
        StiSqlSource,
        StiNoSqlSource,
        StiSybaseSource,
        StiSybaseAdsSource,
        StiTeradataSource,
        StiVistaDBSource,
        StiDB2Source,
        StiDiagonalUpLineShapeType,
        StiHorizontalLineShapeType,
        StiLeftAndRightLineShapeType,
        StiOvalShapeType,
        StiRectangleShapeType,
        StiTopAndBottomLineShapeType,
        StiVerticalLineShapeType,
        StiDivisionShapeType,
        StiFlowchartCardShapeType,
        StiFlowchartDecisionShapeType,
        StiFlowchartManualInputShapeType,
        StiFlowchartSortShapeType,
        StiFrameShapeType,
        StiMinusShapeType,
        StiMultiplyShapeType,
        StiParallelogramShapeType,
        StiPlusShapeType,
        StiRegularPentagonShapeType,
        StiTrapezoidShapeType,
        StiSnipSameSideCornerRectangleShapeType,
        StiSnipDiagonalSideCornerRectangleShapeType,
        StiFlowchartPreparationShapeType,

        StiRadialScale,
        StiV1RadialScale,
        StiLinearScale,
        StiV1LinearScale,
        StiLinearBar,
        StiRadialBar,
        StiNeedle,
        StiRadialMarker,
        StiScaleRangeList,
        StiRadialRange,
        StiStateIndicator,
        StiStateIndicatorFilter,
        StiRadialRangeList,
        StiLinearRangeList,
        StiLinearRange,
        StiLinearTickMarkMajor,
        StiLinearTickMarkMinor,
        StiLinearTickMarkCustomValue,
        StiLinearTickLabelMajor,
        StiLinearTickLabelMinor,
        StiLinearTickLabelCustom,
        StiLinearTickLabelCustomValue,
        StiRadialTickMarkMajor,
        StiRadialTickMarkMinor,
        StiRadialTickMarkCustom,
        StiRadialTickMarkCustomValue,
        StiRadialTickLabelMajor,
        StiRadialTickLabelMinor,
        StiRadialTickLabelCustom,
        StiRadialTickLabelCustomValue,
        StiLinearMarker,
        StiLinearTickMarkCustom,
        StiLinearIndicatorRangeInfo,
        StiRadialIndicatorRangeInfo,

        StiBlueDashboardControlStyle,
        StiBlueDashboardIndicatorStyle,
        StiBlueDashboardPageStyle,
        StiBlueDashboardPivotStyle,
        StiBlueDashboardProgressStyle,
        StiBlueDashboardTableStyle,
        StiBlueDashboardCardsStyle,

        StiOrangeDashboardControlStyle,
        StiOrangeDashboardIndicatorStyle,
        StiOrangeDashboardPageStyle,
        StiOrangeDashboardPivotStyle,
        StiOrangeDashboardProgressStyle,
        StiOrangeDashboardTableStyle,
        StiOrangeDashboardCardsStyle,

        StiGreenDashboardControlStyle,
        StiGreenDashboardIndicatorStyle,
        StiGreenDashboardPageStyle,
        StiGreenDashboardProgressStyle,
        StiGreenDashboardPivotStyle,
        StiGreenDashboardTableStyle,
        StiGreenDashboardCardsStyle,

        StiTurquoiseDashboardControlStyle,
        StiTurquoiseDashboardIndicatorStyle,
        StiTurquoiseDashboardPageStyle,
        StiTurquoiseDashboardProgressStyle,
        StiTurquoiseDashboardPivotStyle,
        StiTurquoiseDashboardTableStyle,
        StiTurquoiseDashboardCardsStyle,

        StiSlateGrayDashboardControlStyle,
        StiSlateGrayDashboardIndicatorStyle,
        StiSlateGrayDashboardPageStyle,
        StiSlateGrayDashboardProgressStyle,
        StiSlateGrayDashboardPivotStyle,
        StiSlateGrayDashboardTableStyle,
        StiSlateGrayDashboardCardsStyle,

        StiDarkBlueDashboardControlStyle,
        StiDarkBlueDashboardIndicatorStyle,
        StiDarkBlueDashboardPageStyle,
        StiDarkBlueDashboardProgressStyle,
        StiDarkBlueDashboardPivotStyle,
        StiDarkBlueDashboardTableStyle,
        StiDarkBlueDashboardCardsStyle,

        StiDarkGrayDashboardControlStyle,
        StiDarkGrayDashboardIndicatorStyle,
        StiDarkGrayDashboardPageStyle,
        StiDarkGrayDashboardProgressStyle,
        StiDarkGrayDashboardPivotStyle,
        StiDarkGrayDashboardTableStyle,
        StiDarkGrayDashboardCardsStyle,

        StiDarkTurquoiseDashboardControlStyle,
        StiDarkTurquoiseDashboardIndicatorStyle,
        StiDarkTurquoiseDashboardPageStyle,
        StiDarkTurquoiseDashboardProgressStyle,
        StiDarkTurquoiseDashboardPivotStyle,
        StiDarkTurquoiseDashboardTableStyle,
        StiDarkTurquoiseDashboardCardsStyle,

        StiSilverDashboardControlStyle,
        StiSilverDashboardIndicatorStyle,
        StiSilverDashboardPageStyle,
        StiSilverDashboardPivotStyle,
        StiSilverDashboardProgressStyle,
        StiSilverDashboardTableStyle,
        StiSilverDashboardCardsStyle,

        StiAliceBlueDashboardControlStyle,
        StiAliceBlueDashboardIndicatorStyle,
        StiAliceBlueDashboardPageStyle,
        StiAliceBlueDashboardPivotStyle,
        StiAliceBlueDashboardProgressStyle,
        StiAliceBlueDashboardTableStyle,
        StiAliceBlueDashboardCardsStyle,

        StiDarkGreenDashboardControlStyle,
        StiDarkGreenDashboardIndicatorStyle,
        StiDarkGreenDashboardPageStyle,
        StiDarkGreenDashboardProgressStyle,
        StiDarkGreenDashboardPivotStyle,
        StiDarkGreenDashboardTableStyle,
        StiDarkGreenDashboardCardsStyle,

        StiSiennaDashboardControlStyle,
        StiSiennaDashboardIndicatorStyle,
        StiSiennaDashboardPageStyle,
        StiSiennaDashboardPivotStyle,
        StiSiennaDashboardProgressStyle,
        StiSiennaDashboardTableStyle,
        StiSiennaDashboardCardsStyle,

        StiCustomDashboardControlStyle,
        StiCustomDashboardPivotStyle,
        StiCustomDashboardIndicatorStyle,
        StiCustomDashboardProgressStyle,
        StiCustomDashboardTableStyle,
        StiCustomDashboardCardsStyle
    }
    #endregion

    #region StiRenderedWith
    public enum StiRenderedWith
    {
        Unknown = 0,
        Net,
        Wpf,
        Silverlight,
        WinRT,
        Flex,
        Java,
        JS
    }
    #endregion

    #region StiRankOrder
    public enum StiRankOrder
    {
        Asc,
        Desc
    }
    #endregion

    #region StiXmlType
    public enum StiXmlType
    {
        AdoNetXml,
        Xml
    }
    #endregion

    #region StiDateRangeKind
    public enum StiDateRangeKind
    {
        CurrentMonth,
        CurrentQuarter,
        CurrentWeek,
        CurrentYear,

        NextMonth,
        NextQuarter,
        NextWeek,
        NextYear,

        PreviousMonth,
        PreviousQuarter,
        PreviousWeek,
        PreviousYear,

        FirstQuarter,
        SecondQuarter,
        ThirdQuarter,
        FourthQuarter,

        MonthToDate,
        QuarterToDate,
        WeekToDate,
        YearToDate,

        Today,
        Tomorrow,
        Yesterday,

        Last7Days,
        Last14Days,
        Last30Days
    }
    #endregion

    #region StiRefreshTimeValues
    public enum StiRefreshTimeValues
    {
        None = 0,
        Refresh10Seconds = 10,
        Refresh20Seconds = 20,
        Refresh30Seconds = 30,
        Refresh1Minute = 60,
        Refresh2Minutes = 120,
        Refresh5Minutes = 300,
        Refresh10Minutes = 600,
        Refresh30Minutes = 1800,
        Refresh1Hour = 3600
    }
    #endregion

    #region StiDashboardViewerSettings
    [Flags]
    public enum StiDashboardViewerSettings
    {
        All = ShowToolBar | ShowRefreshButton | ShowParametersButton | ShowResetAllFilters | ShowOpenButton | ShowFullScreenButton | ShowEditButton 
            | ShowMenuButton | ShowReportSnapshots | ShowExports,
        None = 0,
        ShowToolBar = 1,
        ShowRefreshButton = 2,
        ShowOpenButton = 4,
        ShowFullScreenButton = 8,
        ShowMenuButton = 16,
        ShowEditButton = 32,
        ShowReportSnapshots = 64,
        ShowExports = 128,
        ShowResetAllFilters = 256,
        ShowParametersButton = 512
    }
    #endregion

    #region StiNewReportDictionaryBehavior
    public enum StiNewReportDictionaryBehavior
    {
        MergeDictionary,
        NewDictionary
    }
    #endregion

    #region StiViewerRefreshingMode
    public enum StiViewerRefreshingMode
    {
        DataOnly,
        Full
    }
    #endregion

    #region StiNamingRule
    public enum StiNamingRule
    {
        Simple,
        Advanced
    }
    #endregion

    #region StiDataMode
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StiDataMode
    {
        UsingDataFields,
        ManuallyEnteringData
    }
    #endregion

    #region StiAnimationPlaybackType
    [Flags]
    public enum StiAnimationPlaybackType
    {
        None = 0,
        OnStart = 1,
        OnPreview = 2
    }
    #endregion

    #region StiHtmlPreviewMode
    /// <summary>
    /// Enumeration which contains modes for exporting report to html format for viewing in the web viewer.
    /// </summary>
    public enum StiHtmlPreviewMode
    {
        /// <summary>
        /// A div tag of the HTML will be used for the exporting of a viewing report.
        /// </summary>
        Div = 1,
        /// <summary>
        /// A table tag of the HTML will be used for the exporting of a viewing report.
        /// </summary>
        Table = 2
    }
    #endregion
}