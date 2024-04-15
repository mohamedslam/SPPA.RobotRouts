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
using System.Linq;
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
            private static List<StiArea> chartAreas;
            public static List<StiArea> ChartAreas
            {
                get
                {
                    lock (lockObject)
                    {
                        return chartAreas ?? (chartAreas = new List<StiArea>
                        {
                            new StiClusteredColumnArea(),
                            new StiHistogramArea(),
                            new StiRibbonArea(),
                            new StiLineArea(),
                            new StiAreaArea(),
                            new StiSplineArea(),
                            new StiSplineAreaArea(),

                            new StiStackedColumnArea(),
                            new StiStackedLineArea(),
                            new StiStackedAreaArea(),
                            new StiStackedSplineArea(),
                            new StiStackedSplineAreaArea(),

                            new StiSteppedLineArea(),
                            new StiSteppedAreaArea(),

                            new StiFullStackedColumnArea(),
                            new StiFullStackedLineArea(),
                            new StiFullStackedAreaArea(),
                            new StiFullStackedSplineArea(),
                            new StiFullStackedSplineAreaArea(),

                            new StiClusteredBarArea(),
                            new StiStackedBarArea(),
                            new StiFullStackedBarArea(),

                            new StiPieArea(),
                            new StiPie3dArea(),
                            new StiDoughnutArea(),

                            new StiRadarPointArea(),
                            new StiRadarLineArea(),
                            new StiRadarAreaArea(),

                            new StiGanttArea(),

                            new StiScatterArea(),
                            new StiBubbleArea(),

                            new StiRangeArea(),
                            new StiSplineRangeArea(),
                            new StiSteppedRangeArea(),
                            new StiRangeBarArea(),

                            new StiCandlestickArea(),
                            new StiStockArea(),

                            new StiFunnelArea(),
                            new StiFunnelWeightedSlicesArea(),
                            new StiTreemapArea(),
                            new StiPictorialArea(),
                            new StiPictorialStackedArea(),

                            new StiParetoArea(),
                            new StiWaterfallArea(),
                            new StiSunburstArea(),
                            new StiBoxAndWhiskerArea(),

                            new StiClusteredColumnArea3D(),

                            new StiStackedColumnArea3D(),

                            new StiFullStackedColumnArea3D(),

                        }.OrderBy(a => a.Core.Position).ToList());
                    }
                }
            }
        }
    }
}