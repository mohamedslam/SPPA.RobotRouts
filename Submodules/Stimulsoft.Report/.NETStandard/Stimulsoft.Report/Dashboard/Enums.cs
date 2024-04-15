#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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

using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Converters;
using System;

namespace Stimulsoft.Report.Dashboard
{
    #region StiElementStyleIdent
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StiElementStyleIdent
    {
        Auto = 1,
        Blue,//Style29
        Orange,//Style24
        Green,//Style25
        Turquoise,//Style26
        SlateGray,//Style27
        DarkBlue,//Style28<-Style26
        DarkGray,//Style30
        DarkGreen,//Style34
        DarkTurquoise,//Style31
        Silver,//Style32
        AliceBlue,//Style33,
        Sienna,//Style35
        Custom
    }
    #endregion

    #region StiItemSelectionMode
    public enum StiItemSelectionMode
    {
        One,
        Multi
    }
    #endregion

    #region StiItemOrientation
    public enum StiItemOrientation
    {
        Vertical,
        Horizontal
    }
    #endregion

    #region StiDateSelectionMode
    public enum StiDateSelectionMode
    {
        Single,
        Range,
        AutoRange
    }
    #endregion

    #region StiInitialDateRangeSelectionSource
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StiInitialDateRangeSelectionSource
    {
        Selection,
        Variable
    }
    #endregion

    #region StiInitialDateRangeSelection
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StiInitialDateRangeSelection
    {
        DayTomorrow,
        DayToday,
        DayYesterday,

        WeekNext,
        WeekCurrent,
        WeekPrevious,

        MonthNext,
        MonthCurrent,
        MonthPrevious,

        QuarterNext,
        QuarterCurrent,
        QuarterPrevious,
        QuarterFirst,
        QuarterSecond,
        QuarterThird,
        QuarterFourth,

        YearNext,
        YearCurrent,
        YearPrevious,

        Last7Days,
        Last14Days,
        Last30Days,

        DateToWeek,
        DateToMonth,
        DateToQuarter,
        DateToYear
    }
    #endregion

    #region StiProgressElementMode
    public enum StiProgressElementMode
    {
        Pie,
        Circle,
        DataBars
    }
    #endregion

    #region StiDateCondition
    public enum StiDateCondition
    {
        EqualTo,
        NotEqualTo,
        GreaterThan,
        GreaterThanOrEqualTo,
        LessThan,
        LessThanOrEqualTo
    }
    #endregion

    #region StiTableSizeMode
    public enum StiTableSizeMode
    {
        AutoSize,
        Fit
    }
    #endregion

    #region StiChartLabelsPosition
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StiChartLabelsPosition
    {
        None,
        Center,//Pie, Pie3d Axis, Funnel, Treemap
        InsideBase,//Axis
        InsideEnd,//Pie, Axis
        Left,//Axis
        Outside,//Axis, Pie
        OutsideBase,//Axis
        OutsideEnd,//Axis
        OutsideLeft,//Funnel
        OutsideRight,//Funnel
        Right,//Axis
        TwoColumns,//Pie
        Value,//Axis
        Total//StackedColumn
    }
    #endregion
    
    #region StiInteractionIdent
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StiInteractionIdent
    {
        Chart = 1,
        Gauge,
        Image,
        Indicator,
        OnlineMap,
        PivotTable,
        Progress,
        RegionMap,
        Table,
        TableColumn,
        Text,
        Cards
    }
    #endregion

    #region StiAvailableInteractionOnHover
    [Flags]
    internal enum StiAvailableInteractionOnHover
    {
        ShowToolTip = 1,
        ShowHyperlink = 2,
        None = 0,
        All = ShowToolTip | ShowHyperlink
    }
    #endregion

    #region StiAvailableInteractionOnClick
    [Flags]
    internal enum StiAvailableInteractionOnClick
    {
        ShowDashboard = 1,
        OpenHyperlink = 2,
        ApplyFilter = 4,
        DrillDown = 8,
        None = 0,
        All = ShowDashboard | OpenHyperlink | ApplyFilter | DrillDown
    }
    #endregion

    #region StiAvailableInteractionOnDataManipulation
    [Flags]
    internal enum StiAvailableInteractionOnDataManipulation
    {
        AllowSorting = 1,
        AllowFiltering = 2,
        AllowDrillDown = 4,
        AllowColumnSelection = 8,
        All = AllowSorting | AllowFiltering | AllowDrillDown | AllowColumnSelection,
        None = 0
    }
    #endregion

    #region StiInteractionOnHover
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StiInteractionOnHover
    {
        None,
        ShowToolTip,
        ShowHyperlink
    }
    #endregion

    #region StiInteractionOnClick
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StiInteractionOnClick
    {
        None,
        ShowDashboard,
        OpenHyperlink,
        ApplyFilter,
        DrillDown
    }
    #endregion

    #region StiInteractionViewsState
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StiInteractionViewsState
    {
        Always,
        OnHover
    }
    #endregion

    #region StiInteractionOpenHyperlinkDestination
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StiInteractionOpenHyperlinkDestination
    {
        NewTab,
        CurrectTab
    }
    #endregion

    #region StiElementMeterAction
    public enum StiElementMeterAction
    {
        None,
        Rename,
        Delete,
        ClearAll
    }
    #endregion

    #region StiOnlineMapElement
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StiOnlineMapLocationType
    {
        Auto = 0,
        AdminDivision1,
        AdminDivision2,
        CountryRegion,
        Neighborhood,
        PopulatedPlace,
        Postcode1,
        Postcode2,
        Postcode3,
        Postcode4
    }

    [JsonConverter(typeof(StringEnumConverter))] 
    public enum StiOnlineMapLocationColorType
    {
        Single = 0,
        ColorEach,
        Group,
        Value
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum StiOnlineMapHeatmapColorGradientType
    {
        BlackAquaWhite = 0,
        BlueRed,
        DeepSea,
        ColorSpectrum,
        Incandescent,
        HeatedMetal,
        Sunrise,
        SteppedColors,
        VisibleSpectrum,
        Custom
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum StiOnlineMapValueViewMode
    {
        Bubble = 0,
        Value,
        Icon,
        Chart,
        Heatmap
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum StiOnlineMapCulture
    {
        ar_SA = 0,
        eu,
        bg,
        bg_BG,
        ca,
        ku_Arab,
        zh_CN,
        zh_HK,
        zh_Hans,
        zh_TW,
        zh_Hant,
        cs,
        cs_CZ,
        da,
        da_DK,
        nl_BE,
        nl,
        nl_NL,
        en_AU,
        en_CA,
        en_IN,
        en_GB,
        en_US,
        fi,
        fi_FI,
        fr_BE,
        fr_CA,
        fr,
        fr_FR,
        fr_CH,
        gl,
        de,
        de_DE,
        el,
        he,
        he_IL,
        hi,
        hi_IN,
        hu,
        hu_HU,
        is_IS,
        it,
        it_IT,
        ja,
        ja_JP,
        ko,
        Ko_KR,
        ky_Cyrl,
        lv,
        lv_LV,
        lt,
        lt_LT,
        nb,
        nb_NO,
        nn,
        pl,
        pl_PL,
        pt_BR,
        pt_P,
        ru,
        ru_RU,
        es_MX,
        es,
        es_ES,
        es_US,
        sv,
        sv_SE,
        tt_Cyrl,
        th,
        th_TH,
        tr,
        tr_TR,
        uk,
        uk_UA,
        ug_Arab,
        ca_ES_valencia,
        vi,
        vi_VN
    }
    #endregion

    #region StiIconAlignment
    /// <summary>
    /// Variants of the Icon object alignment.
    /// </summary>
    public enum StiIconAlignment
    {
        None,
        Left,
        Right,
        Top,
        Bottom,
        Center
    }
    #endregion

    #region StiCheckAlignment
    /// <summary>
    /// Variants of the Check symbol alignment.
    /// </summary>
    public enum StiCheckAlignment
    {
        Left,
        Right
    }
    #endregion

    #region StiIndicatorFieldCondition
    public enum StiIndicatorFieldCondition
    {
        Value,
        Series,
        Target,
        Variation
    }
    #endregion

    #region StiIndicatorConditionPermissions
    [Flags]
    public enum StiIndicatorConditionPermissions
    {
        None = 0,
        Font = 1,
        FontSize = 2,
        FontStyleBold = 4,
        FontStyleItalic = 8,
        FontStyleUnderline = 16,
        FontStyleStrikeout = 32,
        TextColor = 64,
        BackColor = 128,
        Borders = 256,
        Icon = 512,
        TargetIcon = 1024,
        All = None | Font | FontSize | FontStyleBold | FontStyleItalic | FontStyleUnderline | FontStyleStrikeout | TextColor | BackColor | Borders | Icon | TargetIcon
    }
    #endregion 

    #region StiIndicatorFieldCondition
    public enum StiTargetMode
    {
        Percentage,
        Variation
    }
    #endregion

    #region StiProgressFieldCondition
    public enum StiProgressFieldCondition
    {
        Value,
        Series,
        Target,
        Percentage
    }
    #endregion

    #region StiProgressConditionPermissions
    [Flags]
    public enum StiProgressConditionPermissions
    {
        None = 0,
        Font = 1,
        FontSize = 2,
        FontStyleBold = 4,
        FontStyleItalic = 8,
        FontStyleUnderline = 16,
        FontStyleStrikeout = 32,
        TextColor = 64,
        Color = 128,
        TrackColor = 256,
        All = None | Font | FontSize | FontStyleBold | FontStyleItalic | FontStyleUnderline | FontStyleStrikeout | TextColor | Color | TrackColor
    }
    #endregion 

    #region StiTableConditionPermissions
    [Flags]
    public enum StiTableConditionPermissions
    {
        None = 0,
        Font = 1,
        FontSize = 2,
        FontStyleBold = 4,
        FontStyleItalic = 8,
        FontStyleUnderline = 16,
        FontStyleStrikeout = 32,
        ForeColor = 64,
        BackColor = 128,
        All = None | Font | FontSize | FontStyleBold | FontStyleItalic | FontStyleUnderline | FontStyleStrikeout | ForeColor | BackColor
    }
    #endregion 

    #region StiChartTrendLine
    public enum StiChartTrendLineType
    {
        None,
        Exponential,
        Linear,
        Logarithmic
    }
    #endregion

    #region StiChartSeriesType
    /// <summary>
    /// Describes types of the chart series in the chart element.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StiChartSeriesType
    {
        // ClusteredColumn
        ClusteredColumn = 1,
        ClusteredColumn3D,
        StackedColumn,
        StackedColumn3D,
        FullStackedColumn,
        FullStackedColumn3D,
        Ribbon,
        Pareto,
        Histogram,
        // Line
        Line,
        Line3D,
        StackedLine,
        FullStackedLine,
        Spline,
        StackedSpline,
        FullStackedSpline,
        SteppedLine,
        // Area
        Area,
        StackedArea,
        FullStackedArea,
        SplineArea,
        StackedSplineArea,
        FullStackedSplineArea,
        SteppedArea,
        // Range
        Range,
        SplineRange,
        SteppedRange,
        RangeBar,
        // ClusteredBar
        ClusteredBar,
        StackedBar,
        FullStackedBar,
        // Scatter
        Scatter,
        ScatterLine,
        ScatterSpline,
        // Pie
        Pie,
        Pie3d,
        // RadarArea
        RadarPoint,
        RadarLine,
        RadarArea,
        // Funnel
        Funnel,
        FunnelWeightedSlices,
        // Financial
        Candlestick,
        Stock,
        // Others
        Treemap,
        Gantt,
        Doughnut,
        Bubble,
        Pictorial,
        PictorialStacked,
        Sunburst,
        Waterfall,
        BoxAndWhisker
    }
    #endregion

    #region StiEmptyCellsAs
    public enum StiEmptyCellsAs
    {
        Gap,
        Zero,
        ConnectPointsWithLine
    }
    #endregion

    #region StiFontSize
    public enum StiFontSizeMode
    {
        Auto,
        Value,
        Target
    }
    #endregion

    #region StiLabelPlacement
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StiLabelPlacement
    {
        Outside,
        Inside
    }
    #endregion

    #region StiDashboardContentAlignment
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StiDashboardContentAlignment
    {
        Left,
        Center,
        Right,
        StretchXY,
        StretchX
    }
    #endregion

    #region StiIndicatorMode
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StiIndicatorIconMode
    {
        Auto = 1,
        Custom
    }
    #endregion

    #region StiIndicatorIconRangeMode
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StiIndicatorIconRangeMode
    {
        Percentage = 1,
        Value
    }
    #endregion
        
    #region StiButtonStretch
    /// <summary>
    /// Idents of different types of button stretching.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StiButtonStretch
    {
        StretchXY,
        StretchX
    }
    #endregion

    #region StiButtonType
    /// <summary>
    /// Idents of different types of a button.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StiButtonType
    {
        Button,
        CheckBox,
        RadioButton
    }
    #endregion

    #region StiTextSizeMode
    /// <summary>
    /// Specifies how the text output in the component area depends on the text size.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StiTextSizeMode
    {
        Fit = 1,
        WordWrap,
        Trimming
    }
    #endregion
}