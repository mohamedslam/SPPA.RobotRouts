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
using Stimulsoft.Report.Gauge;

namespace Stimulsoft.Report
{
    /// <summary>
    /// Class for adjustment all aspects of Stimulsoft Reports.
    /// </summary>
    public sealed partial class StiOptions
    {
        public sealed partial class Services
        {
            private static List<StiGaugeStyleXF> gaugeStyles;
            public static List<StiGaugeStyleXF> GaugeStyles
            {
                get
                {
                    lock (lockObject)
                    {
                        return gaugeStyles ?? (gaugeStyles = new List<StiGaugeStyleXF>
                        {   
                            new StiGaugeStyleXF29(),
                            new StiGaugeStyleXF24(),
                            new StiGaugeStyleXF27(),
                            new StiGaugeStyleXF26(),
                            new StiGaugeStyleXF25(),
                            new StiGaugeStyleXF28(),
                            new StiGaugeStyleXF30(),
                            new StiGaugeStyleXF31(),
                            new StiGaugeStyleXF32(),
                            new StiGaugeStyleXF33(),
                            new StiGaugeStyleXF34(),
                            new StiGaugeStyleXF35(),

                        }.OrderBy(s => s.ToString()).ToList());
                    }
                }
            }
        }
    }
}
