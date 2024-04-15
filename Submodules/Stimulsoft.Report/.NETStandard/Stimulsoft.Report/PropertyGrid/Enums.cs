﻿#region Copyright (C) 2003-2022 Stimulsoft
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

namespace Stimulsoft.Report.PropertyGrid
{
    #region StiPropertyObjectId
    public enum StiPropertyObjectId
    {
        ReportIcon,
        SignatureMode,
        SignatureLabel,
        SignatureDraw,
        SignatureImage,
        SignatureText,
        SignatureDescription,
        PageIcon,
        ReportImage,
        ImageMultipleFactor,
        BackgroundColor,
        SpaceRatio,
        PaperSize,
        PageWidth,
        PageHeight,
        PageOrientation,
        Watermark,
        Margins,
        NumberOfCopies,
        StopBeforePrint,
        TitleBeforeHeader,
        StretchToPrintArea,
        UpperMarks,
        UnlimitedHeight,
        UnlimitedBreakable,
        SegmentPerWidth,
        SegmentPerHeight,
        Columns,
        ColumnWidth,
        ColumnGaps,
        BorderSize,
        RightToLeft,
        Brush,
        ChartAreaBrush,
        SeriesLabelsBrush,
        LegendBrush,
        InterlacingVertBrush,
        InterlacingHorBrush,
        MarkerBrush,
        TickMarkMajorBrush,
        TickMarkMajorBorder,
        TickMarkMinorBrush,
        TickMarkMinorBorder,
        TickLabelMinorTextBrush,
        TickLabelMajorTextBrush,
        LinearBarBorderBrush,
        LinearBarBrush,
        LinearBarEmptyBorderBrush,
        LinearBarEmptyBrush,
        RadialBarBorderBrush,
        NeedleCapBrush,
        NeedleCapBorderBrush,
        NeedleBrush,
        NeedleBorderBrush,
        RadialBarBrush,
        RadialBarEmptyBorderBrush,
        RadialBarEmptyBrush,
        CapBrush,
        CapBrushBrush,
        BrushNegative,
        PageBrush,
        Border,
        Conditions,
        ComponentStyle,
        OddStyle,
        EvenStyle,
        Enabled,
        Interaction,
        PrintOnPreviousPage,
        PrintHeadersFootersFromPreviousPage,
        ResetPageNumber,
        Name,
        PathData,
        PathSchema,
        ConnectionString,
        Alias,
        LargeHeight,
        LargeHeightFactor,
        ExcelSheet,
        HorAlignment,
        VertAlignment,
        BarCodeChecksum,
        TextAngle,
        Font,
        TickLabelMajorFont,
        TickLabelMinorFont,
        ForeColor,
        BackColor,
        ChartAreaBorderColor,
        SeriesLabelsColor,
        TrendLineColor,
        LegendTitleColor,
        LegendLabelsColor,
        LegendBorderColor,
        AxisTitleColor,
        TextColor,
        AxisLineColor,
        AxisLabelsColor,
        GridLinesVertColor,
        GridLinesHorColor,
        HotBackColor,
        GlyphColor,
        PositiveColor,
        NegativeColor,
        IconColor,
        HotForeColor,
        DefaultColor,
        Zoom,
        Left,
        Top,
        Width,
        Height,
        MinSize,
        MaxSize,
        UseParentStyles,
        GrowToHeight,
        DockStyle,
        Printable,
        PrintOn,
        AnchorMode,
        ShiftMode,
        Restrictions,
        Locked,
        Linked,
        MirrorMargins,
        Checked,
        CheckStyleForTrue,
        CheckStyleForFalse,
        CheckBoxValues,
        Size,
        SizeFloat,
        ContourColor,
        Editable,
        TextBrush,
        NegativeTextBrush,
        ExcelValue,
        SummaryExpression,
        NewPageBefore,
        NewPageAfter,
        NewColumnBefore,
        NewColumnAfter,
        BreakIfLessThan,
        Bevel,
        SkipFirst,
        LimitRows,
        MaxHeight,
        MinHeight,
        KeepChildTogether,
        CanGrow,
        CanShrink,
        CanBreak,
        PrintAtBottom,
        PrintIfParentDisabled,
        ContainerEditor,
        KeepFooterTogether,
        PrintIfEmpty,
        PrintOnAllPages,
        PrintOnEvenOddPages,
        KeepHeaderTogether,
        MinWidth,
        MaxWidth,
        KeepGroupFooterTogether,
        Condition,
        SortDirection,
        SummarySortDirection,
        SummaryType,
        KeepGroupHeaderTogether,
        DataSource,
        BusinessObject,
        DataRelation,
        MasterComponent,
        CountData,
        RatioY,
        DataColumns,
        DataRows,
        FilterOn,
        ConvertEmptyStringToNull,
        Filters,
        Sort,
        ColumnDirection,
        MinRowsInColumn,
        KeepDetailsTogether,
        KeepDetails,
        FilterMode,
        FilterEngine,
        MapID,
        MapType,
        BarCodeAngle,
        PrintIfDetailEmpty,
        SizeMode,
        KeepGroupTogether,
        Indent,
        Color,
        TextBackground,
        HeaderColor,
        HeaderForeground,
        FooterForeground,
        DataColor,
        DataForeground,
        GridColor,
        Style,
        StartCap,
        EndCap,
		DataColumn,
        SimpleImage,
        ImageEditor,
        SignatureEditor,
        RichTextDesign,
        AspectRatio,
        MultipleFactor,
        ImageRotation,
        ProcessingDuplicates,
        TextQuality,
        Stretch,
        Smoothing,
        TopSide,
        LeftSide,
        BottomSide,
        RightSide,
        KeepReportSummaryTogether,
        Round,
        ShapeRound,
        ShapeEditor,
        BorderColor,
        SubReportDesigner,
        TableAutoWidth,
        AutoWidth,
        AutoWidthType,
        Direction,
        DataSourceType,
        ColumnCount,
        RowCount,
        HeaderRowsCount,
        FooterRowsCount,
        DockableTable,
        Text,
        TextEditor,
        TextFormat,
        AllowHtmlTags,
        HideZeros,
        LinesOfUnderline,
        LineSpacing,
        MaxNumberOfLines,
        OnlyText,
        DetectUrls,
        FullConvertExpression,
        ProcessAtEnd,
        ShrinkFontToFit,
        ShrinkFontToFitMinimumSize,
        TextOptions,
        WordWrap,
        Wysiwyg,
        ExportAsImage,
        CellDockStyle,
        CellWidth,
        CellHeight,
        HorSpacing,
        VertSpacing,
        ContinuousText,
        Code,
        Ratio,
        FixedWidth,
        CellType,
        ProcessAt,
        HeaderPrintOn,
        HeaderCanGrow,
        HeaderCanShrink,
        HeaderCanBreak,
        HeaderPrintAtBottom,
        HeaderPrintIfEmpty,
        HeaderPrintOnAllPages,
        HeaderPrintOnEvenOddPages,
        FooterPrintOn,
        FooterCanGrow,
        FooterCanShrink,
        FooterCanBreak,
        FooterPrintAtBottom,
        FooterPrintIfEmpty,
        FooterPrintOnAllPages,
        FooterPrintOnEvenOddPages,
        NameInSource,
        Type,
        StartNewPageIfLessThan,
        GlobalizedName,
        KeepSubReportTogether,
        RenderTo,
        BarCodeEditor,

        #region Report.Description
        ReportName,
        ReportAlias,
        ReportAuthor,
        ReportDescription,
        #endregion

        #region Report.Main
        AutoLocalizeReportOnRun,
        CacheAllData,
        CalculationMode,
        ConvertNulls,
        ReportCulture,
        ReportEngineVersion,
        Collate,
        ParameterWidth,
        CommandTimeout,
        CodePage,
        EngineVersion,
        GlobalizationStrings,
        NumberOfPass,
        PreviewMode,
        ReportPrinterSettings,
        PrinterSettings,
        ReferencedAssemblies,
        RefreshTime,
        ReportCacheMode,
        ReportUnit,
        ScriptLanguage,
        StopBeforePage,
        StoreImagesInResources,
        ReportStyles,
        PreviewSettings,
        Styles,
        ParametersOrientation,
        #endregion

        EmptyValue,
        Wrap,
        RequestParameters,
        WrapGap,
        KeepCrossTabTogether,
        Description,
        Category,
        StyleName,
        StyleCollectionName,
        StylesCollectionName,
        StyleConditions,
        Colors,
        StyleColors,
        Heatmap,
        HeatmapWithGroup,

        AllowUseBorderFormatting,
        AllowUseBorderSides,
        AllowUseBorderSidesFromLocation,
        AllowUseBrush,
        AllowUseFont,
        AllowUseImage,
        AllowUseTextBrush,
        AllowUseNegativeTextBrush,
        AllowUseTextOptions,
        AllowUseHorAlignment,
        AllowUseTextFormat,
        AllowUseVertAlignment,
        AutoDataColumns,
        AutoDataRows,
        BasicStyleColor,
        BrushType,
        EncodingType,
        CheckSum1,
        CheckSum2,
        EncodingMode,
        ErrorsCorrectionLevel,
        SupplementType,
        CheckSum,
        ErrorCorrectionLevel,
        MatrixSize,
        Expression,
        StringExpression,
        DataParameterSize,
        DataParameterType,
        AllowApplyStyle,
        PrintVerticalBars,
        AddClearZone,
        ShowQuietZoneIndicator,
        UseRectangularSymbols,
        AllowApplyColorNegative,
        iHorSpacing,
        iVertSpacing,
        ChartTitle,
        ChartTable,
        ChartTopN,
        ChartOptions3D,
        ChartTrendLine,
        Area,
        TreemapArea,
        WaterfallArea,
        PictorialArea,
        ClusteredColumnArea,
        PieArea,
        RadarAreaArea,
        FunnelArea,
        RadarLineArea,
        RadarPointArea,
        StackedColumnArea,
        RibbonColumnArea,
        Legend,
        Series,
        ChartStyle,
        AllowUseBackColor,
        AllowUseForeColor,
        GridLinesHor,
        Interlacing,
        XAxis,
        XTopAxis,
        StiYAxis,
        YRightAxis,
        RadarGridLines,
        XRadarAxis,
        YRadarAxis,

        #region ChartSeries
        Icon,
        BodyShape,
        EyeFrameShape,
        EyeBallShape,
        SeriesInteraction,
        SortBy,
        Format,
        AutoSeriesKeyDataColumn,
        AutoSeriesColorDataColumn,
        AutoSeriesTitleDataColumn,
        ShowShadow,
        ShowZeros,
        YAxis,
        ShowNulls,
        fWidth,
        ArrowWidth,
        ArrowHeight,
        ShowInLegend,
        ShowSeriesLabels,
        Title,
        ArgumentDataColumn,
        Argument,
        ListOfArguments,
        ValueDataColumn,
        ValueDataColumnOpen,
        ValueDataColumnClose,
        ValueDataColumnHigh,
        ValueDataColumnLow,
        ValueDataColumnEnd,
        Value,
        ValueOpen,
        ValueClose,
        ValueHigh,
        ValueLow,
        ValueEnd,
        ListOfValues,
        ListOfValuesOpen,
        ListOfValuesClose,
        ListOfValuesHigh,
        ListOfValuesLow,
        ListOfValuesEnd,
        LogarithmicScale,
        SeriesNoneLabels,
        SeriesTwoColumnsPieLabels,
        SeriesInsideEndAxisLabels,
        SeriesInsideBaseAxisLabels,
        SeriesCenterAxisLabels,
        SeriesCenterTreemapLabels,
        SeriesOutsideEndAxisLabels,
        SeriesOutsideBaseAxisLabels,
        SeriesOutsideAxisLabels,
        SeriesLeftAxisLabels,
        SeriesValueAxisLabels,
        SeriesRightAxisLabels,
        SeriesCenterPieLabels,
        SeriesOutsidePieLabels,
        SeriesCenterFunnelLabels,
        SeriesOutsideLeftFunnelLabels,
        SeriesOutsideRightFunnelLabels,
        LabelsOffset,
        Lighting,
        LineColor,
        LineColorNegative,
        LineMarker,
        LineStyle,
        LineWidth,
        Marker,
        PointAtCenter,
        Tension,
        AllowApplyBorderColor,
        AllowApplyBrush,
        AllowApplyBrushNegative,
        Diameter,
        StartAngle,
        Distance,
        CutPieList,
        WeightDataColumn,
        Weight,
        ListOfWeights,
        Rotation,
        TopmostLine,
        #endregion

        #region StiDialogInfoItem
        DialogInfoValue,
        StringDialogInfoKey,
        GuidDialogInfoKey,
        CharDialogInfoKey,
        BoolDialogInfoKey,
        ImageDialogInfoKey,
        DateDialogInfoKey,
        TimeDialogInfoKey,
        DoubleDialogInfoKey,
        DecimalDialogInfoKey,
        LongDialogInfoKey,
        RangeStringFromDialogInfo,
        RangeStringToDialogInfo,
        RangeGuidFromDialogInfo,
        RangeGuidToDialogInfo,
        RangeCharFromDialogInfo,
        RangeCharToDialogInfo,
        RangeDateFromDialogInfo,
        RangeDateToDialogInfo,
        RangeTimeFromDialogInfo,
        RangeTimeToDialogInfo,
        RangeDoubleFromDialogInfo,
        RangeDoubleToDialogInfo,
        RangeDecimalFromDialogInfo,
        RangeDecimalToDialogInfo,
        RangeLongFromDialogInfo,
        RangeLongToDialogInfo,
        RangeExpressionFromDialogInfo,
        RangeExpressionToDialogInfo,
        #endregion

        #region OracleConnectionStringBuilder
        OracleConnectionString,
        OracleOmitOracleConnectionName,
        OracleUnicode,
        OracleEnlist,
        OracleLoadBalanceTimeout,
        OracleMaxPoolSize,
        OracleMinPoolSize,
        OraclePooling,
        OracleIntegratedSecurity,
        OraclePassword,
        OraclePersistSecurityInfo,
        OracleUserID,
        OracleDataSource,
        #endregion

        CalcInvisible,
        ChartAreaShowShadow,
        TrimExcessData,
        TrendLineShowShadow,
        SeriesShowShadow,
        SeriesLighting,
        ShowLegend,
        AutoScale,
        ShortValue,
        CacheTotals,
        ShowQuickButtons,
        ShowLabelText,
        AutoSize,
        MinValue,
        MaxValue,
        Orientation,
        ShowBehind,
        StripBrush,
        Visible,
        Antialiasing,
        TitleColor,
        TitleVisible,
        AxisValue,
        SerialNumber,
        ExtensionDigit,
        CompanyPrefix,
        Query,
        Separator,
        Path,
        SqlCommand,
        SupplementCode,
        Position,
        TextNotEdit,
        fAspectRatio,

        #region StiHierarchicalBand
        KeyDataColumn,
        MasterKeyDataColumn,
        ParentValue,
        Headers,
        Footers,
        #endregion

        #region CrossTab
        DisplayValue,
        SortType,
        fAngle,
        Module,
        Space,
        fHeight,
        fRatio,
        EnumeratorSeparator,
        KeepMergedCellsTogether,
        EnumeratorType,
        MergeHeaders,
        ShowTotal,
        ShowPercents,
        Summary,
        SummaryValues,
        ImageHorAlignment,
        ImageVertAlignment,
        UseStyleOfSummaryInRowTotal,
        UseStyleOfSummaryInColumnTotal,

        PrintTitleOnAllPages,
        #endregion

        ChartFilters,
        ChartEditor,
        DataSourceEditor,
        DataBandEditor,
        MapEditor,
        CrossTabEditor,
        GroupHeaderEditor,
        GroupHeaderExpression,

        #region StiGauge
        Minimum,
        Maximum,
        MajorInterval,
        MinorInterval,
        StartWidth,
        EndWidth,
        IsReversed,
        SweepAngle,
        Radius,
        RadiusMode,
        Skin,
        Center,
        BorderBrush,
        CapBorderBrush,
        RelativeHeight,
        NullableMaximumValue,
        NullableMinimumValue,
        Offset,
        OffsetNeedle,
        SkipIndices,
        SkipValues,
        OffsetAngle,
        Placement,
        RelativeWidth,
        BorderWidth,
        CapBorderWidth,
        SkipMajorValues,
        ValueExpr,
        LabelRotationMode,
        TextFormatStr,
        NullableOffset,
        NullableOffsetAngle,
        ValueF,
        NullablePlacement,
        NullableRelativeHeight,
        NullableRelativeWidth,
        NullableBorderWidth,
        NullableLabelRotationMode,
        TextStr,
        EndValue,
        StartValue,
        UseValuesFromTheSpecifiedRange,
        UseRangeColor,
        EmptyBrush,
        EmptyBorderBrush,
        EmptyBorderWidth,
        ShowValue,
        AutoCalculateCenterPoint,
        CenterPoint,
        RangeColorMode,
        Trimming,
        #endregion

        BodyBrush,
        EyeBallBrush,
        EyeFrameBrush,

        XmlType,
        ConnectOnStart,
        RetrieveOnlyUsedData,
        AllowExpressions,
        ReconnectOnEachRow,

        Column,
        Label,
        ShowNullValues,
        TextAllignment,
        DimensionFunction,
        MeasureFunction,
        ShowTitles,
        TitleStr,
        SubTitle,
        Footnote,
        Mode,
        ProcessTilde,
        StructuredAppendPosition,
        StructuredAppendTotal,

        MapColorDataColumn,
        MapGroupDataColumn,
        MapKeyDataColumn,
        MapGssDataColumn,
        MapNameDataColumn,
        MapValueDataColumn,

        #region Resources
        AvailableInTheViewer,
        #endregion

        LaTexExpression
    }
    #endregion

    #region StiPropertyEventId
    public enum StiPropertyEventId
    {
        BeginRenderEvent,
        RenderingEvent,
        EndRenderEvent,
        ColumnBeginRenderEvent,
        ColumnEndRenderEvent,
        GetExcelSheetEvent,
        GetToolTipEvent,
        ProcessChartEvent,
        GetTagEvent,
        GetHyperlinkEvent,
        GetBookmarkEvent,
        BeforePrintEvent,
        AfterPrintEvent,
        GetDrillDownReportEvent,
        ClickEvent,
        DoubleClickEvent,
        MouseEnterEvent,
        MouseLeaveEvent,
        GetExcelValueEvent,
        GetValueEvent,
        GetBarCodeEvent,
        GetCheckedEvent,
        GetCollapsedEvent,
        GetImageURLEvent,
        GetImageDataEvent,
        GetZipCodeEvent,
        ExportingEvent,
        ExportedEvent,
        PrintingEvent,
        PrintedEvent,
        GetCrossValueEvent,
        ProcessCellEvent,
        NewAutoSeriesEvent,
        GetListOfValuesEvent,
        GetArgumentEvent,
        GetListOfArgumentsEvent,
        GetTitleEvent,
        GetListOfToolTipsEvent,
        GetListOfTagsEvent,
        GetListOfHyperlinksEvent,



        GetPointerEvent,
        GetSummaryExpressionEvent,
        FillParametersEvent,
        GetDataUrlEvent,
    }
    #endregion

    #region StiPropertyCategories
    public enum StiPropertyCategories
    {
        WithoutCategory,
        ComponentEditor,
        Page,
        Size,
        PageAdditional,
        Columns,
        Appearance,
        Map,
        Behavior,
        Design,
        Export,
        BarCode,
        BarCodeAdditional,
        Position,
        Needle,
        Common,
        CapNeedle,
        Check,
        PageColumnBreak,
        Data,
        DataEvents,
        Hierarchical,
        Primitive,
        Image,
        ImageAdditional,
        Shape,
        SubReport,
        Table,
        HeaderTable,
        FooterTable,
        Text,
        TextAdditional,
        ZipCode,
        Signature,
        Cell,
        Description,
        Main,
        Market,
        CrossTab,
        RenderEvents,
        ValueEvents,
        NavigationEvents,
        PrintEvents,
        MouseEvents,
        ExportEvents,
        Parameters,
        Argument,
        Value,
        ValueEnd,
        Chart,
        Misc,
        TickMarkMajor,
        TickMarkMinor,
        TickLabelMajor,
        TickLabelMinor,
        LinearScaleBar,
        RadialScaleBar,
        Initialization,
        Pooling,
        Security,
        Source,
        Weight,
        Title,
        Scale,
        Tick,
        Indicator,
        ValueOpen,
        ValueClose,
        ValueHigh,
        ValueLow,
        Area,
        Series,
        SeriesLabels,
        TrendLine,
        Legend,
        Axis,
        Interlacing,
        GridLines,
        Globalization,
        Engine,
        View
    }
    #endregion
}