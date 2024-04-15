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
using Stimulsoft.Report.Dictionary;

namespace Stimulsoft.Report
{
    /// <summary>
    /// Class for adjustment all aspects of Stimulsoft Reports.
    /// </summary>
    public sealed partial class StiOptions
	{
        public sealed partial class Services
        {
            private static List<StiAggregateFunctionService> aggregateFunctions;
            public static List<StiAggregateFunctionService> AggregateFunctions
            {
                get
                {
                    lock (lockObject)
                    {
                        return aggregateFunctions ?? (aggregateFunctions = new List<StiAggregateFunctionService>
                        {
                            new StiAvgDecimalFunctionService(),
                            new StiAvgDoubleFunctionService(),
                            new StiAvgIntFunctionService(),
                            new StiAvgDateFunctionService(),
                            new StiAvgTimeFunctionService(),

                            new StiCountDistinctFunctionService(),
                            new StiCountFunctionService(),

                            new StiFirstFunctionService(),
                            new StiLastFunctionService(),

                            new StiMaxDecimalFunctionService(),
                            new StiMaxDoubleFunctionService(),
                            new StiMaxIntFunctionService(),
                            new StiMaxDateFunctionService(),
                            new StiMaxTimeFunctionService(),
                            new StiMaxStrFunctionService(),

                            new StiMinDecimalFunctionService(),
                            new StiMinDoubleFunctionService(),
                            new StiMinIntFunctionService(),
                            new StiMinDateFunctionService(),
                            new StiMinTimeFunctionService(),
                            new StiMinStrFunctionService(),

                            new StiMedianDecimalFunctionService(),
                            new StiMedianDoubleFunctionService(),
                            new StiMedianIntFunctionService(),

                            new StiModeDecimalFunctionService(),
                            new StiModeDoubleFunctionService(),
                            new StiModeIntFunctionService(),

                            new StiSumDecimalFunctionService(),
                            new StiSumDoubleFunctionService(),
                            new StiSumIntFunctionService(),
                            new StiSumTimeFunctionService(),
                            new StiSumDistinctDecimalFunctionService()
                        });
                    }
                }
            }
        }
	}
}