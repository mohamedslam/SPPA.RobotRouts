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


namespace Stimulsoft.Report.Components
{
    public class StiPropertyOrder
    {
        #region Appearance
        public const int AppearanceAllowApplyStyle = 100;
        public const int AppearanceBackColor = 110;
        public const int AppearanceBorder = 120;
        public const int AppearanceBrush = 130;        
        public const int AppearanceChecked = 135;
        public const int AppearanceCheckBrush = 136;
        public const int AppearanceCheckAlignment = 138;
        public const int AppearanceConditions = 140;
        public const int AppearanceComponentStyle = 150;
        public const int AppearanceContourColor = 155;
        public const int AppearanceCornerRadius = 150;
        public const int AppearanceCustomIcon = 159;        
        public const int AppearanceOddStyle = 160;
        public const int AppearanceEvenStyle = 170;
        public const int AppearanceFont = 180;
        public const int AppearanceFontSizeMode = 185;
        public const int AppearanceFooterFont = 190;
        public const int AppearanceFooterForeColor = 200;
        public const int AppearanceForeColor = 210;
        public const int AppearanceGlyphColor = 220;
        public const int AppearanceHeaderFont = 230;
        public const int AppearanceHeaderForeColor = 240;
        public const int AppearanceHorAlignment = 250;
        public const int AppearanceIcon = 253;
        public const int AppearanceIconAlignment = 254;
        public const int AppearanceIconBrush = 255;
        public const int AppearanceIconColor = 256;        
        public const int AppearanceNegativeSeriesColors = 260;
        public const int AppearanceSeriesColors = 270;
        public const int AppearanceParetoSeriesColors = 272;
        public const int AppearanceShadow = 276;
        public const int AppearanceShapeType = 277;
        public const int AppearanceStretch = 278;
        public const int AppearanceStyle = 280;
        public const int AppearanceText = 290;
        public const int AppearanceTextBrush = 300;
        public const int AppearanceThreeState = 305;
        public const int AppearanceUseParentStyles = 310;
        public const int AppearanceUseStyleOfSummaryInRowTotal = 320;
        public const int AppearanceUseStyleOfSummaryInColumnTotal = 330;
        public const int AppearanceVertAlignment = 335;
        public const int AppearanceVisible = 340;
        public const int AppearanceVisualStates = 350;
        public const int AppearanceWatermark = 360;
        public const int AppearanceWordWrap = 370;
        public const int AppearanceOption3d = 380;
        #endregion

        #region Scale
        public const int ScaleMinimum = 100;
        public const int ScaleMaximum = 110;
        public const int ScaleMajorInterval = 120;
        public const int ScaleMinorInterval = 130;
        public const int ScaleStartWidth = 140;
        public const int ScaleEndWidth = 150;
        public const int ScaleIsReversed = 160;
        public const int ScaleStartAngle = 170;
        public const int ScaleSweepAngle = 180;
        public const int ScaleRadius = 190;
        public const int ScaleRadiusMode = 200;
        public const int ScaleSkin = 210;
        public const int ScaleOrientation = 220;
        public const int ScaleRelativeHeight = 230;
        #endregion

        #region BarCode
        public const int BarCodeCode = 100;
        public const int BarCodeBarCodeType = 110;
        public const int BarCodeHorAlignment = 120;
        public const int BarCodeVertAlignment = 130;
        #endregion

        #region BarCode Additional
        public const int BarCodeAngle = 100;
        public const int BarCodeAutoScale = 110;
        public const int BarCodeImage = 120;
        public const int BarCodeImageMultipleFactor = 130;
        public const int BarCodeShowLabelText = 140;
        public const int BarCodeShowQuietZones = 150;
        public const int BarCodeZoom = 160;        
        #endregion

        #region Behavior
        public const int BehaviorAnchor = 100;
        public const int BehaviorArgumentFormat = 105;
        public const int BehaviorAutoWidth = 110;

        public const int BehaviorCanBreak = 120;
        public const int BehaviorCanGrow = 130;
        public const int BehaviorCanShrink = 140;

        public const int BehaviorDockStyle = 160;

        public const int BehaviorEnabled = 170;

        public const int BehaviorGrowToHeight = 190;
        public const int BehaviorHorAlignment = 195;

        public const int BehaviorIconAlignment = 197;
        public const int BehaviorInteractive = 200;
        public const int BehaviorInteraction = 205;

        public const int BehaviorKeepChildTogether = 210;
        public const int BehaviorKeepCrossTabTogether = 220;
        public const int BehaviorKeepDetailsTogether = 230;
        public const int BehaviorKeepGroupTogether = 240;
        public const int BehaviorKeepFooterTogether = 250;
        public const int BehaviorKeepHeaderTogether = 260;
        public const int BehaviorKeepSubReportTogether = 270;

        public const int BehaviorLayout = 275;

        public const int BehaviorMargin = 280;
        public const int BehaviorMergeHeaders = 290;

        public const int BehaviorPadding = 300;
        public const int BehaviorPrintable = 310;

        public const int BehaviorPrintAtBottom = 320;
        public const int BehaviorPrintIfEmpty = 330;
        public const int BehaviorPrintIfDetailEmpty = 340;
        public const int BehaviorPrintIfParentDisabled = 350;
        public const int BehaviorPrintOn = 360;
        public const int BehaviorPrintOnAllPages = 370;
        public const int BehaviorPrintOnEvenOddPages = 380;
        public const int BehaviorPrintOnFirstPage = 390;
        public const int BehaviorPrintOnPreviousPage = 400;
        public const int BehaviorPrintHeadersFootersFromPreviousPage = 410;

        public const int BehaviorResetPageNumber = 420;

        public const int BehaviorSizeMode = 430;
        public const int BehaviorSeriesTitle = 435;
        public const int BehaviorShiftMode = 440;
        public const int BehaviorShowTotal = 450;
        public const int BehaviorStartNewPage = 460;
        public const int BehaviorStartNewPageIfLessThan = 470;

        public const int BehaviorTextFormat = 480;
        public const int BehaviorToolTip = 485;
        public const int BehaviorTitle = 490;
                
        public const int BehaviorValueFormat = 495;
        public const int BehaviorVertAlignment = 500;
        public const int BehaviorVisible = 510;
        #endregion

        #region Button
        public const int CheckBoxText = 100;
        public const int CheckBoxChecked = 110;
        public const int CheckBoxCheckAlignment = 120;
        public const int CheckBoxCheckSize = 130;
        public const int CheckBoxIconSet = 140;
        public const int CheckBoxHorAlignment = 150;
        public const int CheckBoxVertAlignment = 160;
        public const int CheckBoxThreeState = 170;
        public const int CheckBoxWordWrap = 180;
        #endregion

        #region Button
        public const int ButtonText = 100;
        public const int ButtonChecked = 110;
        public const int ButtonGroup = 120;
        public const int ButtonIcon = 130;
        public const int ButtonIconAlignment = 140;
        public const int ButtonIconSet = 150;
        public const int ButtonHorAlignment = 160;
        public const int ButtonVertAlignment = 170;
        public const int ButtonThreeState = 180;
        public const int ButtonType = 190;
        public const int ButtonWordWrap = 200;
        #endregion

        #region Layout
        public const int LayoutWidth = 101;
        public const int LayoutHeight = 102;
        public const int LayoutDeviceWidth = 103;
        #endregion

        #region Dialog
        public const int DialogCheckedBinding = 100;
        public const int DialogDataBindings = 110;
        public const int DialogItemsBinding = 120;
        public const int DialogKeysBinding = 130;
        public const int DialogMaxDateBinding = 140;
        public const int DialogMaximumBinding = 150;
        public const int DialogMinDateBinding = 160;
        public const int DialogMinimumBinding = 170;
        public const int DialogSelectedIndexBinding = 180;
        public const int DialogSelectedItemBinding = 190;
        public const int DialogSelectedKeyBinding = 200;
        public const int DialogSelectedValueBinding = 210;
        public const int DialogTextBinding = 220;
        public const int DialogValueBinding = 230;

        public const int DialogAcceptsReturn = 100;
        public const int DialogAcceptsTab = 110;
        public const int DialogAlignment = 120;
        public const int DialogAlternatingBackColor = 130;
        public const int DialogBackColor = 140;
        public const int DialogBackgroundColor = 150;
        public const int DialogBorderStyle = 160;
        public const int DialogCancel = 170;
        public const int DialogChecked = 180;
        public const int DialogCheckOnClick = 190;
        public const int DialogColumnHeadersVisible = 200;
        public const int DialogColumns = 210;
        public const int DialogComponentStyle = 220;
        public const int DialogCustomFormat = 230;
        public const int DialogDataSource = 235;
        public const int DialogDataTextField = 240;
        public const int DialogDefault = 250;
        public const int DialogDialogResult = 260;
        public const int DialogDockStyle = 270;
        public const int DialogDropDownAlign = 280;
        public const int DialogDropDownStyle = 290;
        public const int DialogDropDownWidth = 300;
        public const int DialogEnabled = 310;
        public const int DialogFilter = 320;
        public const int DialogFont = 330;
        public const int DialogForeColor = 340;
        public const int DialogFormat = 350;
        public const int DialogGridLineColor = 360;
        public const int DialogGridLineStyle = 370;
        public const int DialogHeaderBackColor = 380;
        public const int DialogHeaderFont = 390;
        public const int DialogHeaderForeColor = 400;
        public const int DialogHeaderText = 410;
        public const int DialogImage = 420;
        public const int DialogImageAlign = 430;
        public const int DialogIncrement = 440;
        public const int DialogItemHeight = 450;
        public const int DialogItems = 460;
        public const int DialogKeys = 470;
        public const int DialogLocation = 480;
        public const int DialogMaxDate = 490;
        public const int DialogMaxDropDownItems = 500;
        public const int DialogMaximum = 510;
        public const int DialogMaxLength = 520;
        public const int DialogMinDate = 530;
        public const int DialogMinimum = 540;
        public const int DialogMultiline = 550;
        public const int DialogNullText = 560;
        public const int DialogPasswordChar = 570;
        public const int DialogPreferredColumnWidth = 580;
        public const int DialogPreferredRowHeight = 590;
        public const int DialogRightToLeft = 600;
        public const int DialogRowHeadersVisible = 610;
        public const int DialogRowHeaderWidth = 620;
        public const int DialogSelectionBackColor = 630;
        public const int DialogSelectionForeColor = 640;
        public const int DialogSelectionMode = 650;
        public const int DialogShowUpDown = 660;
        public const int DialogSize = 670;
        public const int DialogSizeMode = 680;
        public const int DialogSorted = 690;
        public const int DialogStartMode = 700;
        public const int DialogStartPosition = 710;
        public const int DialogTag = 720;
        public const int DialogText = 730;
        public const int DialogTextAlign = 740;
        public const int DialogToday = 750;
        public const int DialogToolTip = 760;
        public const int DialogTransparentColor = 770;
        public const int DialogValue = 780;
        public const int DialogVisible = 790;
        public const int DialogWidth = 800;
        public const int DialogWindowState = 810;
        public const int DialogWordWrap = 820;
        #endregion

        #region Hierarchical
        public const int HierarchicalKeyDataColumn = 100;
        public const int HierarchicalMasterKeyDataColumn = 110;
        public const int HierarchicalParentValue = 120;
        public const int HierarchicalIndent = 130;
        public const int HierarchicalHeaders = 140;
        public const int HierarchicalFooters = 150;
        #endregion

        #region Interaction
        public const int InteractionBookmark = 100;
        public const int InteractionCollapsingEnabled = 110;
        public const int InteractionCollapsed = 120;
        public const int InteractionCollapseGroupFooter = 130;
        public const int InteractionDrillDownEnabled = 140;
        public const int InteractionDrillDownPage = 150;
        public const int InteractionDrillDownParameters = 153;
        public const int InteractionDrillDownReport = 155;
        public const int InteractionDrillDownParameter1 = 156;
        public const int InteractionDrillDownParameter2 = 157;
        public const int InteractionDrillDownParameter3 = 158;
        public const int InteractionDrillDownParameter4 = 159;
        public const int InteractionDrillDownParameter5 = 160;
        public const int InteractionDrillDownParameter6 = 161;
        public const int InteractionDrillDownParameter7 = 162;
        public const int InteractionDrillDownParameter8 = 163;
        public const int InteractionDrillDownParameter9 = 164;
        public const int InteractionDrillDownParameter10 = 165;
        public const int InteractionHyperlink = 170;
        public const int InteractionSelectionEnabled = 175;
        public const int InteractionSortingEnabled = 180;
        public const int InteractionSortingColumn = 190;
        public const int InteractionTag = 200;
        public const int InteractionToolTip = 210;
        #endregion

        #region Page & Column Break
        public const int PageColumnBreakNewPageBefore = 100;
        public const int PageColumnBreakNewPageAfter = 110;
        public const int PageColumnBreakNewColumnBefore = 120;
        public const int PageColumnBreakNewColumnAfter = 130;
        public const int PageColumnBreakBreakIfLessThan = 140;
        public const int PageColumnBreakSkipFirst = 150;
        public const int PageColumnBreakLimitRows = 160;
        #endregion

        #region Dashboard
        public const int DashboardWatermark = 100;
        public const int DashboardWidth = 101;
        public const int DashboardHeight = 102;
        public const int DashboardDeviceWidth = 103;
        public const int DashboardInteraction = 104;
        public const int DashboardStyleType = 105;
        public const int DashboardStyle = 106;
        #endregion

        #region Form
        public const int FormWidth = 101;
        public const int FormHeight = 102;
        public const int FormDeviceWidth = 103;
        #endregion

        #region Gauge Element
        public const int GaugeElementCrossFiltering = 104;
		public const int GaugeElementDataTransformation = 105;
        public const int GaugeElementGroup = 106;
        public const int GaugeElementInteraction = 107;
        public const int GaugeElementLabels = 108;
        public const int GaugeElementShortValue = 109;
        #endregion

        #region OnlineMap Element
        public const int OnlineMapElementCrossFiltering = 100;
        public const int OnlineMapElementDataTransformation = 101;
        public const int OnlineMapElementGis = 102;
        public const int OnlineMapElementGroup = 103;
        public const int OnlineMapElementInteraction = 104;        
        #endregion

        #region RegionMap Element
        public const int RegionMapElementCrossFiltering = 107;
        public const int RegionMapElementDataTransformation = 108;
        public const int RegionMapElementGroup = 109;
        public const int RegionMapElementShortValue = 110;
        public const int RegionMapElementShowValue = 111;
        public const int RegionMapElementShowZeros = 112;
        public const int RegionMapElementShowBubble = 113;
        #endregion

        #region Indicator Element
        public const int IndicatorElementCrossFiltering = 102;
        public const int IndicatorElementDataTransformation = 103;
        public const int IndicatorElementGroup = 104;        
        public const int IndicatorElementTargetMode = 108;        
        public const int IndicatorElementInteraction = 109;
        public const int IndicatorElementTextFormat = 111;
        public const int IndicatorElementIconMode = 112;
        #endregion

        #region Progress Element
        public const int ProgressElementCrossFiltering = 102;
        public const int ProgressElementDataTransformation = 103;
        public const int ProgressElementGroup = 104;
        public const int ProgressElementTextFormat = 107;
        public const int ProgressElementColorEach = 108;
        #endregion

        #region Gauge
        public const int GaugeAllowApplyStyle = 90;
        public const int GaugeShortValue = 100;
        #endregion

        #region Cards
        public const int CardsElementCrossFiltering = 102;
        public const int CardsElementGroup = 103; 
        #endregion

        #region Chart
        public const int ChartAllowApplyStyle = 90;
        public const int ChartArea = 100;
        public const int ChartChartType = 110;
        public const int ChartConstantLines = 120;
        public const int ChartInterlacingHor = 124;
        public const int ChartInterlacingVert = 128;
        public const int ChartHorSpacing = 130;
        public const int ChartLabels = 140;
        public const int ChartLegend = 150;
        public const int ChartProcessAtEnd = 160;
        public const int ChartRotation = 170;
        public const int ChartSeries = 180;        
        public const int ChartStrips = 190;
        public const int ChartStyle = 200;
        public const int ChartTable = 210;
        public const int ChartTitle = 220;        
        public const int ChartVertSpacing = 230;
        public const int ChartXAxis = 240;
        public const int ChartXTopAxis = 250;
        public const int ChartYAxis = 260;
        public const int ChartYRightAxis = 270;
        #endregion

        #region Chart Element
        public const int ChartElementArea = 190;
        public const int ChartElementArgumentFormat = 200;
        public const int ChartElementColorEach = 210;
        public const int ChartElementCrossFiltering = 220;
        public const int ChartElementConstantLines = 230;
        public const int ChartElementDataTransformation = 240;
        public const int ChartElementGroup = 250;
        public const int ChartElementLabels = 260;
        public const int ChartElementLegend = 270;
        public const int ChartElementMarker = 280;
        public const int ChartElementTrendLines = 290;
        public const int ChartElementValueFormat = 300;
        public const int ChartElementOption3d = 310;
        public const int ChartElementToolTip = 320;
        #endregion

        #region Check
        public const int CheckChecked = 100;
        public const int CheckCheckStyleForTrue = 110;
        public const int CheckCheckStyleForFalse = 120;
        public const int CheckEditable = 130;
        public const int CheckSize = 140;
        public const int CheckValues = 150;
        #endregion

        #region Columns
        public const int ColumnsColumns = 100;
        public const int ColumnsColumnWidth = 110;
        public const int ColumnsColumnGaps = 120;
        public const int ColumnsColumnDirection = 130;
        public const int ColumnsMinRowsInColumn = 140;
        public const int ColumnsRightToLeft = 150;
        #endregion

        #region CrossTab
        public const int CrossTabEmptyValue = 100;
        public const int CrossTabHorAlignment = 110;
        public const int CrossTabPrintIfEmpty = 120;
        public const int CrossTabRightToLeft = 130;
        public const int CrossTabShowSummarySubHeaders = 140;
        public const int CrossTabWrap = 150;
        public const int CrossTabWrapGap = 160;
        public const int CrossTabPrintTitleOnAllPages = 170;
        #endregion

        #region TreeView Element
		public const int TreeViewElementDataTransformation = 101;
        public const int TreeViewElementGroup = 102;
        #endregion

        #region TreeViewBox Element
		public const int TreeViewBoxElementDataTransformation = 101;
        public const int TreeViewBoxElementGroup = 102;
        #endregion

        #region ListBox Element
		public const int ListBoxElementDataTransformation = 102;
        public const int ListBoxElementGroup = 103;
        #endregion

        #region ComboBox Element
		public const int ComboBoxElementDataTransformation = 102;
        public const int ComboBoxElementGroup = 103;
        #endregion

        #region DatePicker Element
		public const int DatePickerElementDataTransformation = 103;
        public const int DatePickerElementGroup = 101;
        #endregion

        #region Map
        public const int MapKeyDataColumn = 100;
        public const int MapNameDataColumn = 102;
        public const int MapValueDataColumn = 103;
        public const int MapGroupDataColumn = 104;
        public const int MapColorDataColumn = 105;
        public const int MapLatitude = 106;
        public const int MapLongitude = 107;
        #endregion

        #region Data
        public const int DataDataSource = 100;        
        public const int DataDataRelation = 120;
        public const int DataBusinessObject = 125;
        public const int DataMasterComponent = 130;
        public const int DataCountData = 140;
        
        public const int DataCalcInvisible = 150;
        public const int DataCondition = 160;

        public const int DataFilterOn = 160;
        public const int DataFilters = 170;
        public const int DataFilterEngine = 180;
        public const int DataFilterMode = 190;

        public const int DataResetDataSource = 200;
        public const int DataMultipleInitialization = 200;

        public const int DataSort = 210;
                
        public const int DataSortDirection = 220;
        public const int DataSummarySortDirection = 230;
        public const int DataSummaryExpression = 240;
        public const int DataSummaryType = 250;
        #endregion

        #region TableOfContents
        public const int TableOfContentsIndent = 100;
        public const int TableOfContentsMargins = 110;        
        public const int TableOfContentsNewPageBefore = 120;
        public const int TableOfContentsNewPageAfter = 130;
        public const int TableOfContentsRightToLeft = 140;
        public const int TableOfContentsStyles = 150;
        #endregion

        #region Table
        public const int TableAutoWidth = 100;
        public const int TableAutoWidthType = 110;
        public const int TableColumnCount = 120;
        public const int TableRowCount = 130;
        public const int TableHeaderRowsCount = 140;
        public const int TableFooterRowsCount = 150;
        public const int TableDefaultHeightCell = 160;
        public const int TableDockableTable = 170;
        public const int TableSkipLastColumn = 180;

        #region Table.Header
        public const int TableBandHeaderPrintOn = 100;
        public const int TableBandHeaderCanGrow = 110;
        public const int TableBandHeaderCanShrink = 120;
        public const int TableBandHeaderCanBreak = 130;
        public const int TableBandHeaderPrintAtBottom = 140;
        public const int TableBandHeaderPrintIfEmpty = 150;
        public const int TableBandHeaderPrintOnAllPages = 160;
        public const int TableBandHeaderPrintOnEvenOddPages = 170;
        #endregion

        #region Table.Footer
        public const int TableBandFooterPrintOn = 100;
        public const int TableBandFooterCanGrow = 110;
        public const int TableBandFooterCanShrink = 120;
        public const int TableBandFooterCanBreak = 130;
        public const int TableBandFooterPrintAtBottom = 140;
        public const int TableBandFooterPrintIfEmpty = 150;
        public const int TableBandFooterPrintOnAllPages = 160;
        public const int TableBandFooterPrintOnEvenOddPages = 170;
        #endregion
        #endregion

        #region Table Element
        public const int TableElementCrossFiltering = 101;
        public const int TableElementDataTransformation = 102;
        public const int TableElementFrozenColumns = 103;
        public const int TableElementGroup = 104;
        public const int TableElementInteraction = 105;
        public const int TableElementSizeMode = 106;
        public const int TableElementWordWrap = 107;
        #endregion

        #region Pivot Element
        public const int PivotTableElementCrossFiltering = 100;
        public const int PivotTableElementDataTransformation = 101;
        public const int PivotTableElementGroup = 102;
        #endregion

        #region Text Element
        public const int TextElementCrossFiltering = 100;
        public const int TextElementGroup = 102;
        #endregion

        #region Image Element
        public const int ImageElementCrossFiltering = 100;
        public const int ImageElementGroup = 102;
        #endregion

        #region Design
        public const int DesignName = 100;
        public const int DesignAlias = 110;
        public const int DesignGlobalizedName = 120;
        public const int DesignIcon = 125;
        public const int DesignRestrictions = 130;
        public const int DesignLocked = 140;
        public const int DesignLinked = 150;
        public const int DesignLargeHeight = 160;
        public const int DesignLargeHeightFactor = 170;
        public const int DesignTag = 180;
        public const int DesignToolTip = 190;
        #endregion

        #region Export
        public const int ExportExcelValue = 100;
        public const int ExportExportAsImage = 110;
        public const int ExportExcelSheet = 120;
        #endregion

        #region Image
        public const int ImageImage = 100;
        public const int ImageDataColumn = 110;
        public const int ImageFile = 120;
        public const int ImageIcon = 130;
        public const int ImageImageData = 140;
        public const int ImageImageURL = 150;
        #endregion

        #region Signature
        public const int SignatureSignatureType = 100;
        public const int SignatureLabel = 110;
        public const int SignatureDraw = 120;
        public const int SignatureImage = 130;
        public const int SignatureText = 140;
        public const int SignatureType = 150;
        public const int SignatureDescription = 160;
        public const int SignatureRequired = 170;
        public const int SignaturePlaceholder = 180;
        
        #endregion

        #region Image Additional
        public const int ImageAspectRatio = 100;
        public const int ImageHorAlignment = 110;
        public const int ImageVertAlignment = 120;
        public const int ImageImageRotation = 130;
        public const int ImageMargins = 140;
        public const int ImageMultipleFactor = 150;
        public const int ImageProcessingDuplicates = 160;
        public const int ImageSmoothing = 170;
        public const int ImageStretch = 180;        
        #endregion

        #region Sub-Report
        public const int SubReportSubReportPage = 100;
        public const int SubReportUseExternalReport = 110;
        public const int SubReportParameterCollection = 120;
        #endregion

        #region Navigation
        public const int NavigationHyperlink = 100;
        public const int NavigationBookmark = 110;
        public const int NavigationToolTip = 120;
        public const int NavigationTag = 130;
        #endregion

        #region MathFormula
        public const int MathFormulaLaTexExpression = 200; 
        #endregion

        #region Primitive
        public const int PrimitiveColor = 100;
        public const int PrimitiveSize = 110;
        public const int PrimitiveStyle = 120;
        public const int PrimitiveStartCap = 130;
        public const int PrimitiveEndCap = 140;
        public const int PrimitiveRound = 145;
        public const int PrimitiveTopSide = 150;
        public const int PrimitiveLeftSide = 160;
        public const int PrimitiveBottomSide = 170;
        public const int PrimitiveRightSide = 180;
        #endregion

        #region Page
        public const int PagePaperSize = 100;
        public const int PagePaperSourceOfFirstPage = 110;
        public const int PagePaperSourceOfOtherPages = 120;
        public const int PagePageWidth = 130;
        public const int PagePageHeight = 140;
        public const int PageOrientation = 150;
        public const int PageWatermark = 160;
        public const int PageMargins = 170;
        public const int PageNumberOfCopies = 180;
        #endregion

        #region Page Additional
        public const int PageMirrorMargins = 90;
        public const int PageStretchToPrintArea = 100;
        public const int PageStopBeforePrint = 110;
        public const int PageTitleBeforeHeader = 120;
        public const int PageUnlimitedWidth = 130;
        public const int PageUnlimitedHeight = 140;
        public const int PageUnlimitedBreakable = 150;
        public const int PageSegmentPerWidth = 160;
        public const int PageSegmentPerHeight = 170;
        #endregion

        #region Report
        public const int ReportDescriptionReportName = 100;
        public const int ReportDescriptionReportAlias = 110;
        public const int ReportDescriptionReportAuthor = 120;
        public const int ReportDescriptionReportDescription = 130;
        public const int ReportDescriptionReportIcon = 140;
        public const int ReportDescriptionReportImage = 150;

        public const int ReportMainAutoLocalizeReportOnRun = 200;
        public const int ReportMainCacheAllData = 210;
        public const int ReportMainCacheTotals = 212;
        public const int ReportMainCalculationMode = 215;
        public const int ReportMainConvertNulls = 220;
        public const int ReportMainCollate = 230;
        public const int ReportMainCulture = 235;
        public const int ReportMainEngineVersion = 240;
        public const int ReportMainGlobalizationStrings = 250;
        public const int ReportMainNumberOfPass = 260;
        public const int ReportMainPreviewMode = 270;
        public const int ReportMainPreviewSettings = 280;
        public const int ReportMainPrinterSettings = 290;
        public const int ReportMainReferencedAssemblies = 300;
        public const int ReportMainRefreshTime = 305;
        public const int ReportMainReportCacheMode = 310;
        public const int ReportMainReportUnit = 320;
        public const int ReportMainRetrieveOnlyUsedData = 322;
        public const int ReportMainParametersPanelOrientation = 324;
        public const int ReportMainRequestParameters = 325;
        public const int ReportMainParameterWidth = 327;
        public const int ReportMainScriptLanguage = 330;
        public const int ReportMainStopBeforePage = 340;
        public const int ReportMainStoreImagesInResources = 340;
        public const int ReportMainStyles = 350;
        #endregion

        #region Range
        public const int RangeAuto = 100;
        public const int RangeMinimum = 110;
        public const int RangeMaximum = 120; 
        #endregion

        #region Shape
        public const int ShapeBorderColor = 130;
        public const int ShapeFill = 140;
        public const int ShapeShapeType = 150;
        public const int ShapeSize = 160;
        public const int ShapeStroke = 170;
        public const int ShapeStyle = 180;
        public const int ShapeText = 200;
        public const int ShapeFont = 210;
        public const int ShapeMargins = 220;
        public const int ShapeBrush = 230;
        public const int ShapeBackgroundColor = 240;
        public const int ShapeHorAlignment = 250;
        public const int ShapeVertAlignment = 260;
        
        #endregion

        #region Style
        public const int StyleName = 100;
        public const int StyleDescription = 110;
        public const int StyleCollectionName = 111;
        public const int StyleConditions = 112;
        #endregion

        #region Text
        public const int TextText = 100;
        public const int TextCellWidth = 110;
        public const int TextCellHeight = 120;
        public const int TextFont = 125;
        public const int TextHorAlignment = 130;
        public const int TextVertAlignment = 140;
        public const int TextHorSpacing = 150;
        public const int TextVertSpacing = 160;
        public const int TextSizeMode = 165;
        public const int TextTextFormat = 170;

        public const int TextDataColumn = 180;
        public const int TextDataUrl = 190;        
        public const int TextMinSize = 200;
        public const int TextMaxSize = 210;        
        #endregion

        #region Text Additional
        public const int TextAllowHtmlText = 100;
        public const int TextAngle = 110;
        public const int TextContinuousText = 115;
        public const int TextDetectUrls = 120;
        public const int TextEditable = 130;
        public const int TextExceedMargins = 135;
        public const int TextFullConvertExpression = 140;
        public const int TextHideZeros = 150;
        public const int TextLinesOfUnderline = 160;
        public const int TextLineSpacing = 170;
        public const int TextMargins = 180;
        public const int TextMaxNumberOfLines = 190;
        public const int TextOnlyText = 200;
        public const int TextProcessAt = 210;
        public const int TextProcessAtEnd = 220;
        public const int TextProcessingDuplicates = 230;
        public const int TextRenderTo = 240;
        public const int TextRightToLeft = 250;
        public const int TextShrinkFontToFit = 260;
        public const int TextShrinkFontToFitMinimumSize = 270;
        public const int TextTextQuality = 280;
        public const int TextTextBrush = 290;
        public const int TextTextOptions = 295;
        public const int TextTrimming = 300;
        public const int TextWordWrap = 310;
        public const int TextWysiwyg = 320;
        #endregion

        #region Watermark
        public const int WatermarkEnabled = 100;

        public const int WatermarkText = 110;
        public const int WatermarkAngle = 120;
        public const int WatermarkFont = 130;
        public const int WatermarkTextBrush = 140;
        public const int WatermarkShowBehind = 150;
        public const int WatermarkShowImageBehind = 160;

        public const int WatermarkImage = 170;
        public const int WatermarkImageAlignment = 180;
        public const int WatermarkImageMultipleFactor = 190;
        public const int WatermarkImageStretch = 200;
        public const int WatermarkImageTiling = 210;
        public const int WatermarkImageTransparency = 220;
        #endregion

        #region WinControl
        public const int WinControlTypeName = 100;
        public const int WinControlText = 110;
        public const int WinControlFont = 120;
        public const int WinControlForeColor = 130;
        public const int WinControlBackColor = 140;
        #endregion

        #region ZipCode
        public const int ZipCodeCode = 100;
        public const int ZipCodeRatio = 110;
        public const int ZipCodeSize = 120;        
        public const int ZipCodeSpaceRatio = 130;
        public const int ZipCodeUpperMarks = 140;
        #endregion
    }
}
