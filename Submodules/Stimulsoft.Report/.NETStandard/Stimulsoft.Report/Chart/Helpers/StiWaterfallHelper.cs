#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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
using System.Collections.Generic;
using System.Linq;

namespace Stimulsoft.Report.Chart
{
    public static class StiWaterfallHelper
    {
        #region Methods
        public static void CheckTotals(StiChart chart)
        {
            #region Calculate Count Values
            var maxCountValues = 0;
            foreach (var series in chart.Series)
            {
                var waterfallSeries = series as StiWaterfallSeries;

                if (waterfallSeries != null)
                {
                    maxCountValues = Math.Max(maxCountValues, waterfallSeries.Values.Length);
                }
            }
            #endregion

            
            for (var indexSeries = 0; indexSeries < chart.Series.Count; indexSeries++)
            {
                var waterfallSeries = chart.Series[indexSeries] as StiWaterfallSeries;

                if (waterfallSeries != null)
                {
                    var listArguments = new List<object>();
                    var listValues = new List<double?>();
                    double? totalValue = 0d;

                    for (var indexValue = 0; indexValue < maxCountValues; indexValue++)
                    {
                        #region Arguments
                        if (indexSeries == 0)
                        {
                            if (indexValue < waterfallSeries.Arguments.Length)
                                listArguments.Add(waterfallSeries.Arguments[indexValue]);
                            else
                                listArguments.Add(indexValue);
                        }
                        #endregion

                        #region Values
                        if (waterfallSeries.Values.Length > indexValue)
                        {
                            listValues.Add(waterfallSeries.Values[indexValue]);
                            totalValue += waterfallSeries.Values[indexValue];
                        }
                        else
                        {
                            listValues.Add(null);
                        }
                        #endregion
                    }

                    if (waterfallSeries.Total.Visible)
                        listValues.Add(totalValue);

                    if (indexSeries == 0)
                        listArguments.Add(waterfallSeries.Total.Text);

                    waterfallSeries.Values = listValues.ToArray();
                    waterfallSeries.Arguments = listArguments.ToArray();
                    waterfallSeries.ValuesStart = null;
                }
            }
        }
        #endregion
    }
}
