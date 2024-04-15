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
            private static List<Chart.StiChartStyle> chartStyles;
            public static List<Chart.StiChartStyle> ChartStyles
            {
                get
                {
                    lock (lockObject)
                    {
                        return chartStyles ?? (chartStyles = new List<Chart.StiChartStyle>
                        {
                            new StiStyle01(),
                            new StiStyle02(),
                            new StiStyle03(),
                            new StiStyle04(),
                            new StiStyle05(),
                            new StiStyle06(),
                            new StiStyle07(),
                            new StiStyle08(),
                            new StiStyle09(),
                            new StiStyle10(),
                            new StiStyle11(),
                            new StiStyle12(),
                            new StiStyle13(),
                            new StiStyle14(),
                            new StiStyle15(),
                            new StiStyle16(),
                            new StiStyle17(),
                            new StiStyle18(),
                            new StiStyle19(),
                            new StiStyle20(),
                            new StiStyle21(),
                            new StiStyle22(),
                            new StiStyle23(),
                            new StiStyle24(),
                            new StiStyle25(),
                            new StiStyle26(),
                            new StiStyle27(),
                            new StiStyle28(),
                            new StiStyle29(),
                            new StiStyle30(),
                            new StiStyle31(),
                            new StiStyle32(),
                            new StiStyle33(),
                            new StiStyle34(),
                            new StiStyle35()
                        }.OrderBy(s => s.ToString()).ToList());
                    }
                }
            }
        }
	}
}