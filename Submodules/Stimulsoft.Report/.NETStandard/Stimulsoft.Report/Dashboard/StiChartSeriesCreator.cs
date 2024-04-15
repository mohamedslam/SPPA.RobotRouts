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
{	TRADE SECRETS OF STIMULSOFT										}
{																	}
{	CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON		}
{	ADDITIONAL RESTRICTIONS.										}
{																	}
{*******************************************************************}
*/
#endregion Copyright (C) 2003-2022 Stimulsoft

using System;
using Stimulsoft.Report.Chart;

namespace Stimulsoft.Report.Dashboard
{
    /// <summary>
    /// This class helps in creation of new chart series object based on specified Ident of series type.
    /// </summary>
    internal static class StiChartSeriesCreator
    {
        #region Methods
        /// <summary>
        /// Creates new chart series object with help of its identification type name.
        /// </summary>
        /// <param name="typeName">A name of the identification type which is used for the series creation.</param>
        /// <returns>Created series object.</returns>
        public static StiSeries New(string typeName)
        {
            if (string.IsNullOrEmpty(typeName)) return null;

            StiChartSeriesType type;

            return Enum.TryParse(typeName, true, out type) ? New(type) : null;
        }

        /// <summary>
        /// Creates new chart series object with help of it type.
        /// </summary>
        /// <param name="type">An idendification type of the series.</param>
        /// <returns>Created series object.</returns>
        public static StiSeries New(StiChartSeriesType type)
        {
            switch (type)
            {
                #region ClusteredColumn
                case StiChartSeriesType.ClusteredColumn:
                    return new StiClusteredColumnSeries();

                case StiChartSeriesType.ClusteredColumn3D:
                    return new StiClusteredColumnSeries3D();

                case StiChartSeriesType.StackedColumn:
                    return new StiStackedColumnSeries();

                case StiChartSeriesType.StackedColumn3D:
                    return new StiStackedColumnSeries3D();

                case StiChartSeriesType.FullStackedColumn:
                    return new StiFullStackedColumnSeries();

                case StiChartSeriesType.FullStackedColumn3D:
                    return new StiFullStackedColumnSeries3D();

                case StiChartSeriesType.Pareto:
                    return new StiParetoSeries();

                case StiChartSeriesType.Ribbon:
                    return new StiRibbonSeries();

                case StiChartSeriesType.Histogram:
                    return new StiHistogramSeries();
                #endregion

                #region Line
                case StiChartSeriesType.Line:
                    return new StiLineSeries();

                case StiChartSeriesType.StackedLine:
                    return new StiStackedLineSeries();

                case StiChartSeriesType.FullStackedLine:
                    return new StiFullStackedLineSeries();

                case StiChartSeriesType.Spline:
                    return new StiSplineSeries();

                case StiChartSeriesType.StackedSpline:
                    return new StiStackedSplineSeries();

                case StiChartSeriesType.FullStackedSpline:
                    return new StiFullStackedSplineSeries();

                case StiChartSeriesType.SteppedLine:
                    return new StiSteppedLineSeries();
                #endregion

                #region Area
                case StiChartSeriesType.Area:
                    return new StiAreaSeries();

                case StiChartSeriesType.StackedArea:
                    return new StiStackedAreaSeries();

                case StiChartSeriesType.FullStackedArea:
                    return new StiFullStackedAreaSeries();

                case StiChartSeriesType.SplineArea:
                    return new StiSplineAreaSeries();

                case StiChartSeriesType.StackedSplineArea:
                    return new StiStackedSplineAreaSeries();

                case StiChartSeriesType.FullStackedSplineArea:
                    return new StiFullStackedSplineAreaSeries();

                case StiChartSeriesType.SteppedArea:
                    return new StiSteppedAreaSeries();
                #endregion

                #region Range
                case StiChartSeriesType.Range:
                    return new StiRangeSeries();

                case StiChartSeriesType.SplineRange:
                    return new StiSplineRangeSeries();

                case StiChartSeriesType.SteppedRange:
                    return new StiSteppedRangeSeries();

                case StiChartSeriesType.RangeBar:
                    return new StiRangeBarSeries();
                #endregion

                #region ClusteredBar
                case StiChartSeriesType.ClusteredBar:
                    return new StiClusteredBarSeries();

                case StiChartSeriesType.StackedBar:
                    return new StiStackedBarSeries();

                case StiChartSeriesType.FullStackedBar:
                    return new StiFullStackedBarSeries();
                #endregion

                #region Scatter
                case StiChartSeriesType.Scatter:
                    return new StiScatterSeries();

                case StiChartSeriesType.ScatterLine:
                    return new StiScatterLineSeries();

                case StiChartSeriesType.ScatterSpline:
                    return new StiScatterSplineSeries();
                #endregion

                #region Pie
                case StiChartSeriesType.Pie:
                    return new StiPieSeries();
                #endregion

                #region Pie 3d
                case StiChartSeriesType.Pie3d:
                    return new StiPie3dSeries();
                #endregion

                #region RadarArea
                case StiChartSeriesType.RadarPoint:
                    return new StiRadarPointSeries();

                case StiChartSeriesType.RadarLine:
                    return new StiRadarLineSeries();

                case StiChartSeriesType.RadarArea:
                    return new StiRadarAreaSeries();
                #endregion

                #region Funnel
                case StiChartSeriesType.Funnel:
                    return new StiFunnelSeries();

                case StiChartSeriesType.FunnelWeightedSlices:
                    return new StiFunnelWeightedSlicesSeries();
                #endregion

                #region Financial
                case StiChartSeriesType.Candlestick:
                    return new StiCandlestickSeries();

                case StiChartSeriesType.Stock:
                    return new StiStockSeries();
                #endregion

                #region Others
                case StiChartSeriesType.Treemap:
                    return new StiTreemapSeries();

                case StiChartSeriesType.Gantt:
                    return new StiGanttSeries();

                case StiChartSeriesType.Doughnut:
                    return new StiDoughnutSeries();

                case StiChartSeriesType.Bubble:
                    return new StiBubbleSeries();

                case StiChartSeriesType.Pictorial:
                    return new StiPictorialSeries();

                case StiChartSeriesType.PictorialStacked:
                    return new StiPictorialStackedSeries();

                case StiChartSeriesType.Sunburst:
                    return new StiSunburstSeries();

                case StiChartSeriesType.Waterfall:
                    return new StiWaterfallSeries();

                case StiChartSeriesType.BoxAndWhisker:
                    return new StiBoxAndWhiskerSeries();
                #endregion

                default:
                    throw new NotSupportedException();
            }
        }
        #endregion
    }
}
