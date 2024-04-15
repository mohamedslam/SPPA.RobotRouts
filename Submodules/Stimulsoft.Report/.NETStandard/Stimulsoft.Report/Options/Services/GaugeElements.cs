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

using Stimulsoft.Report.Components.Gauge;
using Stimulsoft.Report.Components.Gauge.Primitives;
using System.Collections.Generic;

namespace Stimulsoft.Report
{
    public sealed partial class StiOptions
    {
        public sealed partial class Services
        {
            private static List<StiGaugeElement> gaugeElements;
            public static List<StiGaugeElement> GaugeElements
            {
                get
                {
                    lock (lockObject)
                    {
                        return gaugeElements ?? (gaugeElements = new List<StiGaugeElement>
                        {
                            new StiLinearBar(),
                            new StiLinearMarker(),
                            new StiNeedle(),
                            new StiRadialBar(),
                            new StiRadialMarker(),
                            new StiStateIndicator(),
                            new StiLinearRangeList(),
                            new StiRadialRangeList(),
                            new StiLinearTickLabelCustom(),
                            new StiLinearTickLabelMajor(),
                            new StiLinearTickLabelMinor(),
                            new StiLinearTickMarkCustom(),
                            new StiLinearTickMarkMajor(),
                            new StiLinearTickMarkMinor(),
                            new StiRadialTickLabelCustom(),
                            new StiRadialTickLabelMajor(),
                            new StiRadialTickLabelMinor(),
                            new StiRadialTickMarkCustom(),
                            new StiRadialTickMarkMajor(),
                            new StiRadialTickMarkMinor()
                        });
                    }
                }
            }
        }
    }
}
