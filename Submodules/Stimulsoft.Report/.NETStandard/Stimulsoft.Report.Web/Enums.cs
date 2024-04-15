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

namespace Stimulsoft.Report.Web
{
    #region StiAction
    /// <summary>
    /// Enumeration describes a type of the current component action.
    /// </summary>
    public enum StiAction
    {
        /// <summary>
        /// Unknown action or incorrect data.
        /// </summary>
        Undefined,

        /// <summary>
        /// Request viewer resources - scripts, styles.
        /// </summary>
        Resource,

        /// <summary>
        /// The printing of the report.
        /// </summary>
        PrintReport,

        /// <summary>
        /// The opening of the report.
        /// </summary>
        OpenReport,

        /// <summary>
        /// The exporting of the report.
        /// </summary>
        ExportReport,

        /// <summary>
        /// The exporting of the dashboard.
        /// </summary>
        ExportDashboard,

        /// <summary>
        /// Sending a report via Email.
        /// </summary>
        EmailReport,

        /// <summary>
        /// Request a report parameters (request from user variables) for the viewer.
        /// </summary>
        InitVars,

        /// <summary>
        /// Request a report template for designer or report snapshot with parameters (number of pages, zoom, interactivity and others) for viewer.
        /// </summary>
        GetReport,

        /// <summary>
        /// Request to force rendering of the current report template.
        /// </summary>
        RefreshReport,

        /// <summary>
        /// Request the necessary page of the report for viewer.
        /// </summary>
        GetPages,

        /// <summary>
        /// Applying the parameters (variable values) to the report.
        /// </summary>
        Variables,

        /// <summary>
        /// Applying the sort interaction for the report.
        /// </summary>
        Sorting,

        /// <summary>
        /// Applying the drill-down interaction for the report.
        /// </summary>
        DrillDown,

        /// <summary>
        /// Applying the collapsing interaction for the report.
        /// </summary>
        Collapsing,

        /// <summary>
        /// Call the report designer from toolbar of the viewer.
        /// </summary>
        DesignReport,

        /// <summary>
        /// Get the localization file.
        /// </summary>
        GetLocalization,

        /// <summary>
        /// Call the Exit event from the viewer or designer.
        /// </summary>
        Exit,

        /// <summary>
        /// Get the report snapshot for preview in the designer.
        /// </summary>
        PreviewReport,

        /// <summary>
        /// Get the C#/VB.net code of the report template.
        /// </summary>
        GetReportCode,

        /// <summary>
        /// Get the image of the report component which does not support the designer.
        /// </summary>
        GetImage,

        /// <summary>
        /// Test the connection string.
        /// </summary>
        TestConnection,

        /// <summary>
        /// Get the data source columns using the Connection String and SQL query.
        /// </summary>
        RetrieveColumns,

        /// <summary>
        /// Save the report template from the designer.
        /// </summary>
        SaveReport,

        /// <summary>
        /// Create the new report template in the designer.
        /// </summary>
        CreateReport,

        /// <summary>
        /// Run the designer's command, which is contained in the query or POST parameters.
        /// </summary>
        RunCommand,

        /// <summary>
        /// Update cache of the report on the server side, without any actions.
        /// </summary>
        UpdateCache,

        /// <summary>
        /// Get the resource content of the report.
        /// </summary>
        ReportResource,

        /// <summary>
        /// Applying the filter interaction for the dashboard.
        /// </summary>
        DashboardFiltering,

        /// <summary>
        /// Reset all filters of the current dashboard.
        /// </summary>
        DashboardResetAllFilters,

        /// <summary>
        /// Applying the sort interaction for the dashboard.
        /// </summary>
        DashboardSorting,

        /// <summary>
        /// Get filter items for sorts and filters menu.
        /// </summary>
        DashboardGettingFilterItems,

        /// <summary>
        /// Applying the drill-down interaction for the dashboard.
        /// </summary>
        DashboardDrillDown,

        /// <summary>
        /// Applying the drill-down interaction for the dashboard element.
        /// </summary>
        DashboardElementDrillDown,

        /// <summary>
        /// Applying the drill-up interaction for the dashboard element.
        /// </summary>
        DashboardElementDrillUp,

        /// <summary>
        /// Get formatted values for datepicker element.
        /// </summary>
        GetDatePickerFormattedValues,

        /// <summary>
        /// Get data for angular
        /// </summary>
        AngularViewerData,

        /// <summary>
        /// Get data for dashboard element
        /// </summary>
        DashboardViewData,

        /// <summary>
        /// Get dashboard element content
        /// </summary>
        DashboardGetSingleElementContent,

        /// <summary>
        /// Change chart element view state
        /// </summary>
        ChangeChartElementViewState,

        /// <summary>
        /// Select columns for table element
        /// </summary>
        ChangeTableElementSelectColumns,

        /// <summary>
        /// Applying the click event for button element.
        /// </summary>
        DashboardButtonElementApplyEvents,

        /// <summary>
        /// Get data for signature
        /// </summary>
        GetSignatureData,

        /// <summary>
        /// Applying the signatures to the report.
        /// </summary>
        Signatures
    }
    #endregion

    #region BarCodeGeomId
    public enum BarCodeGeomId
    {
        BaseTransform,
        BaseFillRectangle,
        BaseFillRectangle2D,
        BaseDrawRectangle,
        BaseDrawString
    }
    #endregion

    #region StiCacheObjectType
    /// <summary>
    /// Enumeration describes a type of the object to store in the cache.
    /// </summary>
    public enum StiCacheObjectType
    {
        Undefined,
        ReportSnapshot,
        ReportTemplate,
        Clipboard,
        ComponentClone,
        ReportCheckers,
        UndoArray,
        DataTransformation
    }
    #endregion

    #region StiChartRenderType
    /// <summary>
    /// Enumeration describes a type of the chart in the viewer.
    /// </summary>
    public enum StiChartRenderType
    {
        Image = 1,
        Vector = 2,
        AnimatedVector = 3
    }
    #endregion

    #region StiClientType
    /// <summary>
    /// Enumeration describes a type of the viewer client.
    /// </summary>
    public enum StiClientType
    {
        /// <summary>
        /// HTML5 JavaScript client type.
        /// </summary>
        JavaScript,

        /// <summary>
        /// Flash client type.
        /// </summary>
        Flash
    }
    #endregion

    #region StiComponentType
    public enum StiComponentType
    {
        Undefined,
        Viewer,
        Designer
    }
    #endregion

    #region StiContentAlignment
    public enum StiContentAlignment
    {
        Left,
        Center,
        Right,
        Default
    }
    #endregion

    #region StiDateFormatMode
    public sealed class StiDateFormatMode
    {
        public const string FromServer = "FromServer";
        public const string FromClient = "";
    }
    #endregion

    #region StiDesignerCommand
    public enum StiDesignerCommand
    {
        Undefined,
        Synchronization,
        UpdateCache,
        GetReportForDesigner,
        CreateReport,
        OpenReport,
        CloseReport,
        SaveReport,
        SaveAsReport,
        MoveComponent,
        ResizeComponent,
        CreateComponent,
        RemoveComponent,
        AddPage,
        AddDashboard,
        RemovePage,
        SendProperties,
        ChangeUnit,
        RebuildPage,
        LoadReportToViewer,
        SetToClipboard,
        GetFromClipboard,
        Undo,
        Redo,
        RenameComponent,
        WizardResult,
        GetWizardData,
        GetConnectionTypes,
        CreateOrEditConnection,
        DeleteConnection,
        CreateOrEditRelation,
        DeleteRelation,
        CreateOrEditColumn,
        DeleteColumn,
        CreateOrEditParameter,
        DeleteParameter,
        CreateOrEditDataSource,
        DeleteDataSource,
        CreateOrEditBusinessObject,
        DeleteBusinessObject,
        DeleteBusinessObjectCategory,
        EditBusinessObjectCategory,
        CreateOrEditVariable,
        DeleteVariable,
        DeleteVariablesCategory,
        EditVariablesCategory,
        CreateVariablesCategory,
        CreateOrEditResource,
        DeleteResource,
        SynchronizeDictionary,
        GetAllConnections,
        RetrieveColumns,
        UpdateStyles,
        AddStyle,
        CreateStyleCollection,
        StartEditChartComponent,
        StartEditChartElement,
        StartEditMapComponent,
        CanceledEditComponent,
        AddSeries,
        RemoveSeries,
        SeriesMove,
        AddConstantLineOrStrip,
        RemoveConstantLineOrStrip,
        ConstantLineOrStripMove,
        GetLabelsContent,
        GetTrendLineContent,
        GetStylesContent,
        SetLabelsType,
        SetTrendLineType,
        SetChartStyle,
        SetChartPropertyValue,
        SendContainerValue,
        GetReportFromData,
        ItemResourceSave,
        CloneItemResourceSave,
        GetDatabaseData,
        ApplySelectedData,
        CreateTextComponent,
        CreateDataComponent,
        SetReportProperties,
        PageMove,
        TestConnection,
        RunQueryScript,
        ViewData,
        ApplyDesignerOptions,
        GetSqlParameterTypes,
        AlignToGridComponents,
        ChangeArrangeComponents,
        UpdateSampleTextFormat,
        StartEditCrossTabComponent,
        UpdateCrossTabComponent,
        GetCrossTabColorStyles,
        DuplicatePage,
        SetEventValue,
        GetChartStylesContent,
        GetGaugeStylesContent,
        GetMapStylesContent,
        GetCrossTabStylesContent,
        GetTableStylesContent,
        GetSparklineStylesContent,
        ChangeTableComponent,
        UpdateImagesArray,
        OpenStyle,
        CreateStylesFromComponents,
        ChangeSizeComponents,
        CreateFieldOnDblClick,
        GetParamsFromQueryString,
        CreateMovingCopyComponent,
        GetReportCheckItems,
        GetCheckPreview,
        ActionCheck,
        CheckExpression,
        GetGlobalizationStrings,
        AddGlobalizationStrings,
        EditGlobalizationStrings,
        RemoveGlobalizationStrings,
        GetCultureSettingsFromReport,
        SetCultureSettingsToReport,
        ApplyGlobalizationStrings,
        RemoveUnlocalizedGlobalizationStrings,
        StartEditGaugeComponent,
        StartEditSparklineComponent,
        GetResourceContent,
        ConvertResourceContent,
        GetResourceText,
        SetResourceText,
        GetResourceViewData,
        UpdateGaugeComponent,
        GetImagesGallery,
        CreateComponentFromResource,
        CreateElementFromResource,
        GetSampleConnectionString,
        CreateDatabaseFromResource,
        GetRichTextGallery,
        GetRichTextContent,
        DeleteAllDataSources,
        LoadReportFromCloud,
        StartEditBarCodeComponent,
        StartEditShapeComponent,
        ApplyBarCodeProperty,
        ApplyShapeProperty,
        DownloadReport,
        DownloadStyles,
        ExitDesigner,
        GetVariableItemsFromDataColumn,
        MoveDictionaryItem,
        ConvertMetaFileToPng,
        GetReportString,
        NewDictionary,
        SetLocalization,
        UpdateReportAliases,
        MoveConnectionDataToResource,
        SetMapProperties,
        UpdateMapData,
        OpenPage,
        DownloadPage,
        UpdateTableElement,
        UpdateCardsElement,
        UpdateImageElement,
        UpdateTextElement,
        UpdateRegionMapElement,
        UpdateOnlineMapElement,
        UpdateProgressElement,
        UpdateIndicatorElement,
        UpdateChartElement,
        UpdateGaugeElement,
        UpdateShapeElement,
        UpdatePivotTableElement,
        UpdateListBoxElement,
        UpdateComboBoxElement,
        UpdateTreeViewElement,
        UpdateTreeViewBoxElement,
        UpdateDatePickerElement,
        GetDashboardStylesContent,
        CreateTextElement,
        CreateTableElement,
        CreateDatePickerElement,
        CreateComboBoxElement,
        ExecuteCommandForDataTransformation,
        GetRtfResourceContentFromHtmlText,
        ChangeDashboardStyle,
        SetGaugeProperties,
        OpenDictionary,
        MergeDictionary,
        DownloadDictionary,
        CreateDashboard,
        OpenWizardDashboard,
        SetPreviewSettings,
        UpdateElementDataFilters,
        RepaintAllDbsElements,
        GetImagesArray,
        ChangeTypeElement,
        GetDbsElementSortItems,
        ApplySortsToDashboardElement,
        UpdateTextFormatItemsByReportCulture,
        ChangeDashboardViewMode,
        GetMobileViewUnplacedElements,
        OpenWizardReport,
        PrepareReportBeforeGetData,
        ChangeReportType,
        RestoreOldReport,
        AddDemoDataToReport,
        GetSpecialSymbols,
        EmbedAllDataToResources,
        AssociatedGoogleAuthWithYourAccount,
        GetQuickBooksAuthorizationUrl,
        GetQuickBooksTokens,
        RefreshQuickBooksTokens,
        UpdateChart,
        UpdateSparkline,
        ViewQuery,
        SaveReportThumbnail,
        TestODataConnection,
        GetTableOfContentsIdents,
        UpdateComponentsPointerValues,
        ChangeDashboardSettingsValue,
        GetAzureBlobStorageContainerNamesItems,
        GetAzureBlobStorageBlobNameItems,
        GetAzureBlobContentTypeOrDefault,
        GetMathFormulaInfo,
        GetGoogleAnalyticsParameters,
        DuplicateDictionaryElement,
        GetBlocklyInitParameters,
        DownloadBlockly,
        SetDictionaryElementProperty,
        CreateForm,
        GetStylesForSignature,
        GetSignatureData,
        EncryptMachineName,
        GetGitHubAuthorizationUrl,
        DownloadResource,
        DownloadImageContent,
        MoveImageToResource,
        GetFacebookAuthorizationUrl,
        GetStylesContentByType,
        CreateStyleBasedAnotherStyle
    }
    #endregion

    #region StiDesignerComponents
    public enum StiDesignerComponents
    {
        StiText,
        StiTextInCells,
        StiRichText,
        StiImage,
        StiBarCode,
        StiPanel,
        StiClone,
        StiCheckBox,
        StiSubReport,
        StiZipCode,
        StiTable,
        StiCrossTab,
        StiCrossGroupHeaderBand,
        StiCrossGroupFooterBand,
        StiCrossHeaderBand,
        StiCrossFooterBand,
        StiCrossDataBand,
        StiReportTitleBand,
        StiReportSummaryBand,
        StiPageHeaderBand,
        StiPageFooterBand,
        StiGroupHeaderBand,
        StiGroupFooterBand,
        StiHeaderBand,
        StiFooterBand,
        StiColumnHeaderBand,
        StiColumnFooterBand,
        StiDataBand,
        StiHierarchicalBand,
        StiChildBand,
        StiEmptyBand,
        StiOverlayBand
    }
    #endregion

    #region StiDesignerPermissions
    [Flags]
    public enum StiDesignerPermissions
    {
        /// <summary>
        /// Deny all.
        /// </summary>
        None = 0,

        /// <summary>
        /// Allows to create an item.
        /// </summary>
        Create = 1,

        /// <summary>
        /// Allows to delete an item.
        /// </summary>
        Delete = 2,

        /// <summary>
        /// Allows to modify an item.
        /// </summary>
        Modify = 4,

        /// <summary>
        /// Allows to view an item.
        /// </summary>
        View = 8,

        /// <summary>
        /// Allows modify and view an item.
        /// </summary>
        ModifyView = Modify + View,

        /// <summary>
        /// Allow any action with an item.
        /// </summary>
        All = Create + Delete + Modify + View
    }
    #endregion

    #region StiDesignerTheme
    public enum StiDesignerTheme
    {
        Office2013WhiteBlue,
        Office2013WhiteCarmine,
        Office2013WhiteGreen,
        Office2013WhiteOrange,
        Office2013WhitePurple,
        Office2013WhiteTeal,
        Office2013WhiteViolet,
        Office2013LightGrayBlue,
        Office2013LightGrayCarmine,
        Office2013LightGrayGreen,
        Office2013LightGrayOrange,
        Office2013LightGrayPurple,
        Office2013LightGrayTeal,
        Office2013LightGrayViolet,
        Office2013DarkGrayBlue,
        Office2013DarkGrayCarmine,
        Office2013DarkGrayGreen,
        Office2013DarkGrayOrange,
        Office2013DarkGrayPurple,
        Office2013DarkGrayTeal,
        Office2013DarkGrayViolet,
        Office2013VeryDarkGrayBlue,
        Office2013VeryDarkGrayCarmine,
        Office2013VeryDarkGrayGreen,
        Office2013VeryDarkGrayOrange,
        Office2013VeryDarkGrayPurple,
        Office2013VeryDarkGrayTeal,
        Office2013VeryDarkGrayViolet,        
        Office2022WhiteBlue,
        Office2022WhiteCarmine,
        Office2022WhiteGreen,
        Office2022WhiteOrange,
        Office2022WhitePurple,
        Office2022WhiteTeal,
        Office2022WhiteViolet,
        Office2022LightGrayBlue,
        Office2022LightGrayCarmine,
        Office2022LightGrayGreen,
        Office2022LightGrayOrange,
        Office2022LightGrayPurple,
        Office2022LightGrayTeal,
        Office2022LightGrayViolet,
        Office2022DarkGrayBlue,
        Office2022DarkGrayCarmine,
        Office2022DarkGrayGreen,
        Office2022DarkGrayOrange,
        Office2022DarkGrayPurple,
        Office2022DarkGrayTeal,
        Office2022DarkGrayViolet,
        Office2022BlackBlue,
        Office2022BlackCarmine,
        Office2022BlackGreen,
        Office2022BlackOrange,
        Office2022BlackPurple,
        Office2022BlackTeal,
        Office2022BlackViolet
    }
    #endregion
    
    #region StiDocumentType
    public enum StiDocumentType
    {
        Mdc,
        Mdz,
        Mdx
    }
    #endregion

    #region StiImagesID
    public enum StiImagesID
    {
        BusinessObject = 0,
        CalcColumn,
        CalcColumnBinary,
        CalcColumnBool,
        CalcColumnChar,
        CalcColumnDateTime,
        CalcColumnDecimal,
        CalcColumnFloat,
        CalcColumnImage,
        CalcColumnInt,
        CalcColumnString,
        Class,
        Close,
        ColumnsOrder,
        Connection,
        ConnectionFail,
        DataColumn,
        DataColumnBinary,
        DataColumnBool,
        DataColumnChar,
        DataColumnDateTime,
        DataColumnDecimal,
        DataColumnFloat,
        DataColumnImage,
        DataColumnInt,
        DataColumnString,
        DataSource,
        DataSources,
        DataStore,
        DataTable,
        DataTables,
        Folder,
        Format,
        FormatBoolean,
        FormatCurrency,
        FormatDate,
        FormatGeneral,
        FormatNumber,
        FormatPercentage,
        FormatTime,
        Function,
        HtmlTag,
        LabelType,
        LockedCalcColumn,
        LockedCalcColumnBinary,
        LockedCalcColumnBool,
        LockedCalcColumnChar,
        LockedCalcColumnDateTime,
        LockedCalcColumnDecimal,
        LockedCalcColumnFloat,
        LockedCalcColumnImage,
        LockedCalcColumnInt,
        LockedCalcColumnString,
        LockedConnection,
        LockedDataColumn,
        LockedDataColumnBinary,
        LockedDataColumnBool,
        LockedDataColumnChar,
        LockedDataColumnDateTime,
        LockedDataColumnDecimal,
        LockedDataColumnFloat,
        LockedDataColumnImage,
        LockedDataColumnInt,
        LockedDataColumnString,
        LockedDataSource,
        LockedFolder,
        LockedParameter,
        LockedRelation,
        LockedVariable,
        LockedVariableBinary,
        LockedVariableBool,
        LockedVariableChar,
        LockedVariableDateTime,
        LockedVariableDecimal,
        LockedVariableFloat,
        LockedVariableImage,
        LockedVariableInt,
        LockedVariableString,
        Namespace,
        Parameter,
        Property,
        Queries,
        Query,
        RecentConnection,
        Relation,
        StoredProcedure,
        StoredProcedures,
        SystemVariable,
        SystemVariableColumn,
        SystemVariableGroupLine,
        SystemVariableIsFirstPage,
        SystemVariableIsFirstPageThrough,
        SystemVariableIsLastPage,
        SystemVariableIsLastPageThrough,
        SystemVariableLine,
        SystemVariableLineABC,
        SystemVariableLineRoman,
        SystemVariableLineThrough,
        SystemVariablePageNofM,
        SystemVariablePageNofMThrough,
        SystemVariablePageNumber,
        SystemVariablePageNumberThrough,
        SystemVariableReportAlias,
        SystemVariableReportAuthor,
        SystemVariableReportChanged,
        SystemVariableReportCreated,
        SystemVariableReportDescription,
        SystemVariableReportName,
        SystemVariables,
        SystemVariableTime,
        SystemVariableToday,
        SystemVariableTotalPageCount,
        SystemVariableTotalPageCountThrough,
        UndefinedConnection,
        UndefinedDataSource,
        Variable,
        VariableBinary,
        VariableBool,
        VariableChar,
        VariableDateTime,
        VariableDecimal,
        VariableFloat,
        VariableImage,
        VariableInt,
        VariableString,
        View,
        Views,
        LockedVariableListBool,
        LockedVariableListChar,
        LockedVariableListDateTime,
        LockedVariableListDecimal,
        LockedVariableListFloat,
        LockedVariableListImage,
        LockedVariableListInt,
        LockedVariableListString,
        LockedVariableRangeChar,
        LockedVariableRangeDateTime,
        LockedVariableRangeDecimal,
        LockedVariableRangeFloat,
        LockedVariableRangeInt,
        LockedVariableRangeString,
        VariableListBool,
        VariableListChar,
        VariableListDateTime,
        VariableListDecimal,
        VariableListFloat,
        VariableListImage,
        VariableListInt,
        VariableListString,
        VariableRangeChar,
        VariableRangeDateTime,
        VariableRangeDecimal,
        VariableRangeFloat,
        VariableRangeInt,
        VariableRangeString
    }
    #endregion

    #region StiImagesQuality
    public enum StiImagesQuality
    {
        Low,
        Normal,
        High
    }
    #endregion

    #region StiInterfaceType
    /// <summary>
    /// Enumeration describes a type of the interface in the viewer.
    /// </summary>
    public enum StiInterfaceType
    {
        Auto,
        Mouse,
        Touch,

        // Only for Viewer
        Mobile
    }
    #endregion

    #region StiFirstDayOfWeek
    public enum StiFirstDayOfWeek
    {
        Auto,
        Monday,
        Sunday
    }
    #endregion

    #region StiParametersPanelPosition
    public enum StiParametersPanelPosition
    {
        Top,
        Left,
        FromReport
    }
    #endregion

    #region StiPrintAction
    public enum StiPrintAction
    {
        PrintPdf,
        PrintWithoutPreview,
        PrintWithPreview
    }
    #endregion

    #region StiPrintDestination
    public enum StiPrintDestination
    {
        Default,
        Pdf,
        Direct,
        WithPreview
    }
    #endregion

    #region StiReportDisplayMode
    public enum StiReportDisplayMode
    {
        Table,
        Div,
        Span,
        FromReport
    }
    #endregion

    #region StiPrintToPdfMode
    public enum StiPrintToPdfMode
    {
        Hidden,
        Popup
    }
    #endregion

    #region StiReportType
    public enum StiReportType
    {
        Auto,
        Report,
        Dashboard
    }
    #endregion

    #region StiRequestFromUserType
    public enum StiRequestFromUserType
    {
        ListBool,
        ListChar,
        ListDateTime,
        ListTimeSpan,
        ListDecimal,
        ListFloat,
        ListDouble,
        ListByte,
        ListShort,
        ListInt,
        ListLong,
        ListGuid,
        ListString,
        RangeChar,
        RangeDateTime,
        RangeDouble,
        RangeFloat,
        RangeDecimal,
        RangeGuid,
        RangeByte,
        RangeShort,
        RangeInt,
        RangeLong,
        RangeString,
        RangeTimeSpan,
        ValueBool,
        ValueChar,
        ValueDateTime,
        ValueFloat,
        ValueDouble,
        ValueDecimal,
        ValueGuid,
        ValueImage,
        ValueString,
        ValueTimeSpan,
        ValueShort,
        ValueInt,
        ValueLong,
        ValueSbyte,
        ValueUshort,
        ValueUint,
        ValueUlong,
        ValueByte,
        ValueNullableBool,
        ValueNullableChar,
        ValueNullableDateTime,
        ValueNullableFloat,
        ValueNullableDouble,
        ValueNullableDecimal,
        ValueNullableGuid,
        ValueNullableTimeSpan,
        ValueNullableShort,
        ValueNullableInt,
        ValueNullableLong,
        ValueNullableSbyte,
        ValueNullableUshort,
        ValueNullableUint,
        ValueNullableUlong,
        ValueNullableByte
    }
    #endregion    

    #region StiSaveMode
    public enum StiSaveMode
    {
        Hidden,
        Visible,
        NewWindow
    }
    #endregion

    #region StiServerCacheMode
    public enum StiServerCacheMode
    {
        None,
        ObjectCache,
        ObjectSession,
        StringCache,
        StringSession
    }
    #endregion

    #region StiShapeId
    internal enum StiShapeId
    {
        StiShapeTypeService = 0,
        StiSnipDiagonalSideCornerRectangleShapeType = 1,
        StiSnipSameSideCornerRectangleShapeType = 2,
        StiTrapezoidShapeType = 3,
        StiRegularPentagonShapeType = 4,
        StiPlusShapeType = 5,
        StiParallelogramShapeType = 6,
        StiMultiplyShapeType = 7,
        StiMinusShapeType = 8,
        StiFrameShapeType = 9,
        StiFlowchartSortShapeType = 10,
        StiFlowchartPreparationShapeType = 11,
        StiFlowchartOffPageConnectorShapeType = 12,
        StiFlowchartManualInputShapeType = 13,
        StiFlowchartDecisionShapeType = 14,
        StiFlowchartCollateShapeType = 15,
        StiFlowchartCardShapeType = 16,
        StiEqualShapeType = 17,
        StiDivisionShapeType = 18,
        StiChevronShapeType = 19,
        StiBentArrowShapeType = 20,
        StiComplexArrowShapeType = 21,
        StiVerticalLineShapeType = 22,
        StiTriangleShapeType = 23,
        StiTopAndBottomLineShapeType = 24,
        StiRoundedRectangleShapeType = 25,
        StiRectangleShapeType = 26,
        StiOvalShapeType = 27,
        StiLeftAndRightLineShapeType = 28,
        StiHorizontalLineShapeType = 29,
        StiDiagonalUpLineShapeType = 30,
        StiDiagonalDownLineShapeType = 31,
        StiArrowShapeType = 32,
    }
    #endregion

    #region StiShowMenuMode
    public enum StiShowMenuMode
    {
        Click,
        Hover
    }
    #endregion

    #region StiTargetWindow
    public sealed class StiTargetWindow
    {
        public const string Self = "_self";
        public const string Blank = "_blank";
        public const string Top = "_top";
    }
    #endregion

    #region StiToolbarDisplayMode
    public enum StiToolbarDisplayMode
    {
        Simple,
        Separated
    }
    #endregion

    #region StiViewerTheme
    public enum StiViewerTheme
    {
        SimpleGray,
        WindowsXP,
        Windows7,
        Office2003,
        Office2007Blue,
        Office2007Black,
        Office2007Silver,
        Office2010Blue,
        Office2010Black,
        Office2010Silver,
        Office2013WhiteBlue,
        Office2013WhiteCarmine,
        Office2013WhiteGreen,
        Office2013WhiteOrange,
        Office2013WhitePurple,
        Office2013WhiteTeal,
        Office2013WhiteViolet,
        Office2013LightGrayBlue,
        Office2013LightGrayCarmine,
        Office2013LightGrayGreen,
        Office2013LightGrayOrange,
        Office2013LightGrayPurple,
        Office2013LightGrayTeal,
        Office2013LightGrayViolet,
        Office2013DarkGrayBlue,
        Office2013DarkGrayCarmine,
        Office2013DarkGrayGreen,
        Office2013DarkGrayOrange,
        Office2013DarkGrayPurple,
        Office2013DarkGrayTeal,
        Office2013DarkGrayViolet,
        Office2013VeryDarkGrayBlue,
        Office2013VeryDarkGrayCarmine,
        Office2013VeryDarkGrayGreen,
        Office2013VeryDarkGrayOrange,
        Office2013VeryDarkGrayPurple,
        Office2013VeryDarkGrayTeal,
        Office2013VeryDarkGrayViolet,
        Office2022WhiteBlue,
        Office2022WhiteCarmine,
        Office2022WhiteGreen,
        Office2022WhiteOrange,
        Office2022WhitePurple,
        Office2022WhiteTeal,
        Office2022WhiteViolet,
        Office2022LightGrayBlue,
        Office2022LightGrayCarmine,
        Office2022LightGrayGreen,
        Office2022LightGrayOrange,
        Office2022LightGrayPurple,
        Office2022LightGrayTeal,
        Office2022LightGrayViolet,
        Office2022DarkGrayBlue,
        Office2022DarkGrayCarmine,
        Office2022DarkGrayGreen,
        Office2022DarkGrayOrange,
        Office2022DarkGrayPurple,
        Office2022DarkGrayTeal,
        Office2022DarkGrayViolet,
        Office2022BlackBlue,
        Office2022BlackCarmine,
        Office2022BlackGreen,
        Office2022BlackOrange,
        Office2022BlackPurple,
        Office2022BlackTeal,
        Office2022BlackViolet
    }
    #endregion

    #region StiWebViewMode
    public enum StiWebViewMode
    {
        SinglePage,
        Continuous,
        MultiplePages,

        [Obsolete("This value is obsolete. It will be removed in next versions. Please use the SinglePage value instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        OnePage,

        [Obsolete("This value is obsolete. It will be removed in next versions. Please use the MultiplePages value instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        WholeReport
    }
    #endregion

    #region StiWMode
    public enum StiWMode
    {
        Window,
        Opaque,
        Direct,
        Transparent
    }
    #endregion

    #region StiZoomMode
    public sealed class StiZoomMode
    {
        public const int PageWidth = -1;
        public const int PageHeight = -2;
    }
    #endregion

    #region StiPropertiesGridPosition
    public enum StiPropertiesGridPosition
    {
        Left,
        Right
    }
    #endregion

    #region StiImageSize
    public enum StiImageSize
    {
        None,
        Small,
        Big
    }
    #endregion

    #region StiNewReportDictionary
    public enum StiNewReportDictionary
    {
        Auto,
        DictionaryNew,
        DictionaryMerge
    }
    #endregion

    #region StiWizardType
    public enum StiWizardType
    {
        None,
        StandardReport,
        MasterDetailReport,
        LabelReport,
        InvoicesReport,
        OrdersReport,
        QuotationReport
    }
    #endregion

    #region StiReportContentType
    public enum StiReportContentType
    {
        XmlTemplate,
        XmlDocument,
        JsonTemplate,
        JsonDocument,
        Undefined
    }
    #endregion

    #region StiUseAliases
    public enum StiUseAliases
    {
        Auto,
        True,
        False
    }
    #endregion
}
