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

using Stimulsoft.Report.Dashboard;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Stimulsoft.Report.Dashboard
{
    internal static class StiChartGroups
    {
        #region Fields
        private static Hashtable hash = new Hashtable();
        private static bool isInit;
        #endregion

        #region Methods
        internal static bool SameGroup(StiChartSeriesType type1, StiChartSeriesType type2)
        {
            var list = GetGroup(type1);
            if (list == null)
                throw new NotSupportedException();

            return list.Contains(type2);
        }

        internal static List<StiChartSeriesType> GetGroup(StiChartSeriesType type)
        {
            Init();
            foreach (var key in hash.Keys)
            {
                var list = hash[key] as List<StiChartSeriesType>;
                if (list.Contains(type))
                    return list;
            }
            return null;
        }

        internal static void Init()
        {
            if (isInit) return;

            lock (hash)
            {
                hash.Clear();

                #region Bubble
                hash["Bubble"] = new List<StiChartSeriesType>
                {
                    StiChartSeriesType.Bubble
                };
                #endregion

                #region Candlestick
                hash["Candlestick"] = new List<StiChartSeriesType>
                {
                    StiChartSeriesType.Candlestick
                };
                #endregion

                #region ClusteredBar
                hash["ClusteredBar"] = new List<StiChartSeriesType>
                {
                    StiChartSeriesType.ClusteredBar
                };
                #endregion

                #region ClusteredColumn
                hash["ClusteredColumn"] = new List<StiChartSeriesType>
                {
                    StiChartSeriesType.ClusteredColumn,
                    StiChartSeriesType.Line,
                    StiChartSeriesType.SteppedLine,
                    StiChartSeriesType.Spline,
                    StiChartSeriesType.Area,
                    StiChartSeriesType.SteppedArea,
                    StiChartSeriesType.SplineArea
                };
                #endregion

                #region ClusteredColumn3D
                hash["ClusteredColumn3D"] = new List<StiChartSeriesType>
                {
                    StiChartSeriesType.ClusteredColumn3D
                };
                #endregion

                #region Doughnut
                hash["Doughnut"] = new List<StiChartSeriesType>
                {
                    StiChartSeriesType.Doughnut
                };
                #endregion

                #region FullStackedBar
                hash["FullStackedBar"] = new List<StiChartSeriesType>
                {
                    StiChartSeriesType.FullStackedBar
                };
                #endregion

                #region FullStackedColumn
                hash["FullStackedColumn"] = new List<StiChartSeriesType>
                {
                    StiChartSeriesType.FullStackedArea,
                    StiChartSeriesType.FullStackedColumn,
                    StiChartSeriesType.FullStackedLine,
                    StiChartSeriesType.FullStackedSpline,
                    StiChartSeriesType.FullStackedSplineArea
                };
                #endregion

                #region FullStackedColumn3D
                hash["FullStackedColumn3D"] = new List<StiChartSeriesType>
                {
                    StiChartSeriesType.FullStackedColumn3D
                };
                #endregion

                #region Funnel
                hash["Funnel"] = new List<StiChartSeriesType>
                {
                    StiChartSeriesType.Funnel
                };
                #endregion

                #region FunnelWeightedSlices
                hash["FunnelWeightedSlices"] = new List<StiChartSeriesType>
                {
                    StiChartSeriesType.FunnelWeightedSlices
                };
                #endregion

                #region Gantt
                hash["Gantt"] = new List<StiChartSeriesType>
                {
                    StiChartSeriesType.Gantt
                };
                #endregion

                #region Pie
                hash["Pie"] = new List<StiChartSeriesType>
                {
                    StiChartSeriesType.Pie
                };
                #endregion

                #region Pie
                hash["Pie3d"] = new List<StiChartSeriesType>
                {
                    StiChartSeriesType.Pie3d
                };
                #endregion

                #region Radar
                hash["Radar"] = new List<StiChartSeriesType>
                {
                    StiChartSeriesType.RadarArea,
                    StiChartSeriesType.RadarLine,
                    StiChartSeriesType.RadarPoint
                };
                #endregion

                #region Range
                hash["Range"] = new List<StiChartSeriesType>
                {
                    StiChartSeriesType.Range,
                    StiChartSeriesType.RangeBar,
                    StiChartSeriesType.SplineRange,
                    StiChartSeriesType.SteppedRange
                };
                #endregion

                #region Scatter
                hash["Scatter"] = new List<StiChartSeriesType>
                {
                    StiChartSeriesType.Scatter,
                    StiChartSeriesType.ScatterLine,
                    StiChartSeriesType.ScatterSpline
                };
                #endregion

                #region StackedBar
                hash["StackedBar"] = new List<StiChartSeriesType>
                {
                    StiChartSeriesType.StackedBar
                };
                #endregion

                #region StackedColumn
                hash["StackedColumn"] = new List<StiChartSeriesType>
                {
                    StiChartSeriesType.StackedArea,
                    StiChartSeriesType.StackedColumn,
                    StiChartSeriesType.StackedLine,
                    StiChartSeriesType.StackedSpline,
                    StiChartSeriesType.StackedSplineArea
                };
                #endregion

                #region StackedColumn3D
                hash["StackedColumn3D"] = new List<StiChartSeriesType>
                {
                    StiChartSeriesType.StackedColumn3D
                };
                #endregion

                #region Stock
                hash["Stock"] = new List<StiChartSeriesType>
                {
                    StiChartSeriesType.Stock
                };
                #endregion

                #region Treemap
                hash["Treemap"] = new List<StiChartSeriesType>
                {
                    StiChartSeriesType.Treemap
                };
                #endregion

                #region Pareto
                hash["Pareto"] = new List<StiChartSeriesType>
                {
                    StiChartSeriesType.Pareto
                };
                #endregion

                #region Ribbon
                hash["Ribbon"] = new List<StiChartSeriesType>
                {
                    StiChartSeriesType.Ribbon
                };
                #endregion

                #region Histogram
                hash["Histogram"] = new List<StiChartSeriesType>
                {
                    StiChartSeriesType.Histogram
                };
                #endregion

                #region Pictorial
                hash["Pictorial"] = new List<StiChartSeriesType>
                {
                    StiChartSeriesType.Pictorial
                };
                #endregion

                #region Pictorial
                hash["PictorialStacked"] = new List<StiChartSeriesType>
                {
                    StiChartSeriesType.PictorialStacked
                };
                #endregion

                #region Sunburst
                hash["Sunburst"] = new List<StiChartSeriesType>
                {
                    StiChartSeriesType.Sunburst
                };
                #endregion

                #region Waterfall
                hash["Waterfall"] = new List<StiChartSeriesType>
                {
                    StiChartSeriesType.Waterfall
                };
                #endregion

                #region BoxAndWhisker
                hash["BoxAndWhisker"] = new List<StiChartSeriesType>
                {
                    StiChartSeriesType.BoxAndWhisker
                };
                #endregion

                isInit = true;
            }
        }
        #endregion
    }
}
