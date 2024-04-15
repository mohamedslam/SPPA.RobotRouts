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

using System.Collections.Generic;
using Stimulsoft.Report.Chart;

namespace Stimulsoft.Report
{
    /// <summary>
    /// Class for adjustment all aspects of Stimulsoft Reports.
    /// </summary>
    public sealed partial class StiOptions
	{
        public sealed partial class Services
        {
            private static List<StiSeries> chartSeries;
            public static List<StiSeries> ChartSeries
            {
                get
                {
                    lock (lockObject)
                    {
                        return chartSeries ?? (chartSeries = new List<StiSeries>
                        {
                            new StiClusteredColumnSeries(),
                            new StiHistogramSeries(),
                            new StiLineSeries(),
                            new StiAreaSeries(),
                            new StiSplineSeries(),
                            new StiSplineAreaSeries(),

                            new StiStackedColumnSeries(),
                            new StiStackedLineSeries(),
                            new StiStackedAreaSeries(),
                            new StiStackedSplineSeries(),
                            new StiStackedSplineAreaSeries(),
                            new StiRibbonSeries(),

                            new StiSteppedLineSeries(),
                            new StiSteppedAreaSeries(),

                            new StiFullStackedColumnSeries(),
                            new StiFullStackedLineSeries(),
                            new StiFullStackedAreaSeries(),
                            new StiFullStackedSplineSeries(),
                            new StiFullStackedSplineAreaSeries(),

                            new StiClusteredBarSeries(),
                            new StiStackedBarSeries(),
                            new StiFullStackedBarSeries(),

                            new StiDoughnutSeries(),
                            new StiGanttSeries(),
                            new StiPieSeries(),
                            new StiPie3dSeries(),
                            new StiBubbleSeries(),

                            new StiScatterSeries(),
                            new StiScatterLineSeries(),
                            new StiScatterSplineSeries(),

                            new StiRadarPointSeries(),
                            new StiRadarLineSeries(),
                            new StiRadarAreaSeries(),

                            new StiFunnelSeries(),
                            new StiStockSeries(),
                            new StiCandlestickSeries(),
                            new StiFunnelWeightedSlicesSeries(),
                            new StiSteppedRangeSeries(),
                            new StiSplineRangeSeries(),
                            new StiRangeSeries(),
                            new StiRangeBarSeries(),

                            new StiTreemapSeries(),
                            new StiPictorialSeries(),
                            new StiPictorialStackedSeries(),
                            new StiParetoSeries(),
                            new StiWaterfallSeries(),
                            new StiSunburstSeries(),
                            new StiBoxAndWhiskerSeries(),

                            new StiClusteredColumnSeries3D(),
                            new StiStackedColumnSeries3D(),
                            new StiFullStackedColumnSeries3D(),
                        });
                    }
                }
            }
        }
	}
}