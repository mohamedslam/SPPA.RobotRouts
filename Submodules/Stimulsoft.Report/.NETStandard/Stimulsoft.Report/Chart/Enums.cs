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

namespace Stimulsoft.Report.Chart
{
    #region StiColumnShape3D
    public enum StiColumnShape3D
    {
        Box,
        Pyramid,
        PartialPyramid
    } 
    #endregion

    #region StiPie3dShadowStyle
    public enum StiPie3dLightingStyle
    {
        No,
        Solid,
        Gradient
    } 
    #endregion

    #region StiSeriesLabelsPropertyOrder

    public class StiTrendLinePropertyOrder
    {
        public const int LineColor = 90;
        public const int LineStyle = 100;
        public const int LineWidth = 110;
        public const int ShowShadow = 120;
    }

    public class StiSeriesLabelsPropertyOrder
    {
        public const int AllowApplyStyle = 90;
        public const int Angle = 100;
        public const int Antialiasing = 110;
        public const int AutoRotate = 120;
        public const int Conditions = 125;
        public const int DrawBorder = 130;
        public const int BorderColor = 140;
        public const int Brush = 150;
        public const int Font = 160;
        public const int Format = 170;
        public const int LabelColor = 180;
        public const int LegendValueType = 190;
        public const int LineColor = 200;
        public const int LineColorNegative = 201;
        public const int LineLength = 205;
        public const int MarkerAlignment = 210;
        public const int MarkerSize = 220;
        public const int MarkerVisible = 230;
        public const int PreventIntersection = 240;
        public const int ShowInPercent = 250;
        public const int ShowNulls = 255;
        public const int ShowValue = 260;
        public const int ShowZeros = 270;
        public const int Step = 280;
        public const int TextAfter = 290;
        public const int TextBefore = 300;
        public const int UseSeriesColor = 310;
        public const int ValueType = 320;
        public const int ValueTypeSeparator = 330;
        public const int Visible = 340;
        public const int Width = 350;
        public const int WordWrap = 360;
    }
    #endregion

    #region StiSeriesPropertyOrder
    public class StiSeriesPropertyOrder
    {
        public const int AllowSeries = 90;
        public const int AllowSeriesElements = 95;
        public const int DrillDownEnabled = 100;
        public const int DrillDownPage = 110;
        public const int DrillDownReport = 120;
        public const int HyperlinkDataColumn = 130;
        public const int TagDataColumn = 140;
        public const int ToolTipDataColumn = 150;
        public const int WeightDataColumn = 160;
        public const int Hyperlink = 170;
        public const int Tag = 180;
        public const int ToolTip = 190;
        public const int Weight = 200;
        public const int ListOfHyperlinks = 210;
        public const int ListOfTags = 220;
        public const int ListOfToolTips = 230;
        public const int ListOfWeights = 240;

        #region Value
        public const int ValueValueDataColumn = 100;
        public const int ValueValue = 110;
        public const int ValueListOfValues = 120;
        #endregion                      

        #region ValueEnd
        public const int ValueValueDataColumnEnd = 100;
        public const int ValueValueEnd = 110;
        public const int ValueListOfValuesEnd = 120;
        #endregion

        #region ValueOpen
        public const int ValueValueDataColumnOpen = 1;
        public const int ValueValueOpen = 2;
        public const int ValueListOfValuesOpen = 3;
        #endregion

        #region ValueClose
        public const int ValueValueDataColumnClose = 100;
        public const int ValueValueClose = 110;
        public const int ValueListOfValuesClose = 120;
        #endregion

        #region ValueHigh
        public const int ValueValueDataColumnHigh = 100;
        public const int ValueValueHigh = 110;
        public const int ValueListOfValuesHigh = 120;
        #endregion

        #region ValueLow
        public const int ValueValueDataColumnLow = 100;
        public const int ValueValueLow = 110;
        public const int ValueListOfValuesLow = 120;
        #endregion

        #region Argument
        public const int ArgumentArgumentDataColumn = 130;
        public const int ArgumentArgument = 140;
        public const int ArgumentListOfArguments = 150;
        #endregion

        #region Weight
        public const int WeightWeightDataColumn = 100;
        public const int WeightWeight = 110;
        public const int WeightListOfWeights = 120;
        #endregion

        #region Data
        public const int DataAutoSeriesKeyDataColumn = 270;
        public const int DataAutoSeriesColorDataColumn = 280;
        public const int DataAutoSeriesTitleDataColumn = 290;
        #endregion
    }
    #endregion

    #region StiChartTitleDock
    public enum StiChartTitleDock
    {
        Top = 0,
        Right = 90,
        Bottom = 180,
        Left = 270
    }
    #endregion

    #region StiLegendDirection
    public enum StiLegendDirection
    {
        LeftToRight,
        RightToLeft,
        TopToBottom,
        BottomToTop
    }
    #endregion

    #region StiDirection
    public enum StiDirection : int
    {
        LeftToRight = 0,
        RightToLeft = 1,
        TopToBottom = 2,
        BottomToTop = 3
    }
    #endregion

    #region StiLegendHorAlignment
    public enum StiLegendHorAlignment
    {
        LeftOutside,
        Left,
        Center,
        Right,
        RightOutside
    }
    #endregion

    #region StiLegendVertAlignment
    public enum StiLegendVertAlignment
    {
        TopOutside,
        Top,
        Center,
        Bottom,
        BottomOutside
    }
    #endregion

    #region StiMarkerAlignment
    public enum StiMarkerAlignment
    {
        Left,
        Center,
        Right
    }
    #endregion

    #region StiChartAreaPosition
    /// <summary>
    /// Describes position of chart areas.
    /// </summary>
    public enum StiChartAreaPosition
    {
        ClusteredColumn = 0,
        StackedColumn = 1,
        FullStackedColumn = 2,
        Pareto = 3,
        Waterfall = 4,
        Histogram = 5,

        ClusteredBar = 10,
        StackedBar = 11,
        FullStackedBar = 12,

        Pie = 20,
        Doughnut = 21,

        Line = 30,
        SteppedLine = 31,
        StackedLine = 32,
        FullStackedLine = 33,

        Spline = 40,
        StackedSpline = 41,
        FullStackedSpline = 42,

        Area = 50,
        SteppedArea = 51,
        StackedArea = 52,
        FullStackedArea = 53,

        SplineArea = 60,
        StackedSplineArea = 61,
        FullStackedSplineArea = 62,

        Gantt = 70,

        Scatter = 80,
        Bubble = 81,

        RadarPoint = 82,
        RadarLine = 83,
        RadarArea = 84,

        Range = 90,
        SteppedRange = 91,
        RangeBar = 92,
        SplineRange = 93,

        Funnel = 100,

        Candlestick = 110,
        Stock = 120,
        BoxAndWhisker = 121,

        Treemap = 130,
        Pictorial = 131,
        PictorialStacked = 132,

        Sunburst = 140,

        Pie3d = 200,
        ClusteredColumn3d = 201,
        StackedColumn3d = 202,
        FullStackedColumn3d = 202,
    }
    #endregion

    #region StiChartSeriesOrientation
    /// <summary>
    /// Describes orientation of chart series.
    /// </summary>
    public enum StiChartSeriesOrientation
    {
        Horizontal,
        Vertical
    }
    #endregion

    #region StiArrowStyle
    public enum StiArrowStyle
    {
        None,
        Triangle,
        Lines,
        Circle,
        Arc,
        ArcAndCircle
    }
    #endregion

    #region StiLabelsPlacement
    public enum StiLabelsPlacement
    {
        None,
        OneLine,
        TwoLines,
        AutoRotation
    }
    #endregion

    #region StiXAxisDock
    public enum StiXAxisDock
    {
        Top,
        Bottom
    }
    #endregion

    #region StiYAxisDock
    public enum StiYAxisDock
    {
        Left,
        Right
    }
    #endregion

    #region StiTitlePosition
    public enum StiTitlePosition
    {
        Inside,
        Outside
    }
    #endregion

    #region StiSeriesLabelsPosition
    public enum StiSeriesLabelsPosition
    {
        None = 0,

        InsideEndAxis = 1,
        InsideBaseAxis = 2,
        CenterAxis = 3,
        OutsideEndAxis = 4,
        OutsideBaseAxis = 5,
        OutsideAxis = 6,
        Left = 7,
        Value = 8,
        Right = 9,

        InsideEndPie = 10,
        CenterPie = 11,
        OutsidePie = 12,
        TwoColumnsPie = 13,

        CenterFunnel = 14,
        OutsideRightFunnel = 15,
        OutsideLeftFunnel = 16,

        CenterTreemap = 17,
        CenterPie3d = 18,

        CenterPictorialStacked = 19,
        OutsideRightPictorialStacked = 20,
        OutsideLeftPictorialStacked = 21
    }
    #endregion

    #region StiSeriesLabelsType
    [Flags]
    public enum StiSeriesLabelsType
    {
        Axis = 1,
        Pie = 2,
        Doughnut = 4,
        Radar = 8,
        Funnel = 10,
        Treemap = 12,
        Pie3d = 13,
        PictorialStacked = 14,
        All = 15
    }
    #endregion

    #region StiSeriesLabelsValueType
    [Flags]
    public enum StiSeriesLabelsValueType
    {
        Value,
        SeriesTitle,
        Argument,
        Tag,
        Weight,
        ValueArgument,
        ArgumentValue,
        SeriesTitleValue,
        SeriesTitleArgument
    }
    #endregion

    #region StiMarkerType
    public enum StiMarkerType
    {
        /// <summary>
        /// Rectangle type of marker.
        /// </summary>
        Rectangle,
        /// <summary>
        /// Triangle type of marker.
        /// </summary>
        Triangle,
        /// <summary>
        /// Circle type of marker.
        /// </summary>
        Circle,
        /// <summary>
        /// HalfCircle type of marker.
        /// </summary>
        HalfCircle,
        /// <summary>
        /// Star5 type of marker.
        /// </summary>
        Star5,
        /// <summary>
        /// Star6 type of marker.
        /// </summary>
        Star6,
        /// <summary>
        /// Star7 type of marker.
        /// </summary>
        Star7,
        /// <summary>
        /// Star8 type of marker.
        /// </summary>
        Star8,
        /// <summary>
        /// Hexagon type of marker.
        /// </summary>
        Hexagon,
    }
    #endregion

    #region StiSeriesSortType
    public enum StiSeriesSortType
    {
        Value,
        Argument,
        None
    }
    #endregion

    #region StiSeriesSortDirection
    public enum StiSeriesSortDirection
    {
        Ascending,
        Descending
    }
    #endregion

    #region StiSeriesXAxis
    public enum StiSeriesXAxis
    {
        BottomXAxis,
        TopXAxis
    }
    #endregion

    #region StiSeriesYAxis
    public enum StiSeriesYAxis
    {
        LeftYAxis,
        RightYAxis
    }
    #endregion

    #region StiShowSeriesLabels
    public enum StiShowSeriesLabels
    {
        /// <summary>
        /// Obsolete. Please use StiShowSeriesLabels.FromChart instead.
        /// </summary>
        [Obsolete("Please use StiShowSeriesLabels.FromChart")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        None,

        /// <summary>
        /// Show series labels from chart
        /// </summary>
        FromChart,

        /// <summary>
        /// Show series labels from series
        /// </summary>
        FromSeries
    }
    #endregion

    #region StiShowYAxis
    public enum StiShowYAxis
    {
        Left,
        Center,
        Both
    }
    #endregion

    #region StiShowXAxis
    public enum StiShowXAxis
    {
        Bottom,
        Center,
        Both
    }
    #endregion

    #region StiRadarStyle
    public enum StiRadarStyle
    {
        Polygon,
        Circle
    }
    #endregion

    #region StiTimeDateStep
    public enum StiTimeDateStep
    {
        None,
        Second,
        Minute,
        Hour,
        Day,
        Month,
        Year
    }
    #endregion

    #region StiTopNMode
    public enum StiTopNMode
    {
        None,
        Top,
        Bottom
    }
    #endregion

    #region StiChartStyleId
    public enum StiChartStyleId
    {
        StiStyle01,
        StiStyle02,
        StiStyle03,
        StiStyle04,
        StiStyle05,
        StiStyle06,
        StiStyle07,
        StiStyle08,
        StiStyle09,
        StiStyle10,
        StiStyle11,
        StiStyle12,
        StiStyle13,
        StiStyle14,
        StiStyle15,
        StiStyle16,
        StiStyle17,
        StiStyle18,
        StiStyle19,
        StiStyle20,
        StiStyle21,
        StiStyle22,
        StiStyle23,
        StiStyle24,
        StiStyle25,
        StiStyle26,
        StiStyle27,
        StiStyle28,
        StiStyle29,
        StiStyle30,
        StiStyle31,
        StiStyle32,
        StiStyle33,
        StiStyle34,
        StiStyle35,
    }
    #endregion

    #region StiExtendedStyleBool
    public enum StiExtendedStyleBool
    {
        FromStyle,
        True,
        False
    }
    #endregion

    #region StiChartConditionalField
    public enum StiChartConditionalField
    {
        Value,
        Argument,
        Series
    }
    #endregion

    #region StiShowNullAs
    public enum StiShowEmptyCellsAs
    {
        Gap,
        ConnectPointsWithLine
    }
    #endregion

    #region StiChartEditorType
    public enum StiChartEditorType
    {
        Simple,
        Advanced
    }
    #endregion

    #region StiSeriesAnimationType
    public enum StiSeriesAnimationType
    {
        None,
        Column,
        Line,
        Bar,
        Range
    }
    #endregion

    #region StiToolTipAlignment
    /// <summary>
    /// Variants of the Tooltip object alignment.
    /// </summary>
    public enum StiToolTipAlignment
    {
        Left,
        Right,
        Top,
        Bottom,
        Center
    }
    #endregion
}