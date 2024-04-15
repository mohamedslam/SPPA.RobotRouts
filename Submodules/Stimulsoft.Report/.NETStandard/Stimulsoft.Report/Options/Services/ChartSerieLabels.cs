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

            private static List<StiSeriesLabels> chartSerieLabels;
            public static List<StiSeriesLabels> ChartSerieLabels
            {
                get
                {
                    lock (lockObject)
                    {
                        return chartSerieLabels ?? (chartSerieLabels = new List<StiSeriesLabels>
                        {
                            new StiNoneLabels(),

                            new StiCenterAxisLabels(),
                            new StiOutsideAxisLabels(),
                            new StiInsideBaseAxisLabels(),
                            new StiInsideEndAxisLabels(),
                            new StiOutsideBaseAxisLabels(),
                            new StiOutsideEndAxisLabels(),
                            new StiLeftAxisLabels(),
                            new StiValueAxisLabels(),
                            new StiRightAxisLabels(),

                            new StiInsideEndPieLabels(),
                            new StiCenterPieLabels(),
                            new StiOutsidePieLabels(),
                            new StiTwoColumnsPieLabels(),

                            new StiCenterPie3dLabels(),

                            new StiCenterFunnelLabels(),
                            new StiOutsideRightFunnelLabels(),
                            new StiOutsideLeftFunnelLabels(),

                            new StiCenterPictorialStackedLabels(),
                            new StiOutsideLeftPictorialStackedLabels(),
                            new StiOutsideRightPictorialStackedLabels(),

                            new StiCenterTreemapLabels(),

                            new StiCenterAxisLabels3D(),
                            new StiOutsideAxisLabels3D()
                        }.OrderBy(a => a.Core.Position).ToList());
                    }
                }
            }
        }
	}
}