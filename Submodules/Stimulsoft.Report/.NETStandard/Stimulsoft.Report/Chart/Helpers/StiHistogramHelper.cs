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

using Stimulsoft.Report.Components.TextFormats;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stimulsoft.Report.Chart
{
    public static class StiHistogramHelper
    {
        #region Methods
        public static void CheckValuesAndArguments(IStiSeries series, StiFormatService formatService = null)
        {
            if (!(series is StiHistogramSeries)) return;

            var values = series.Values;
            var deviation = GetStandardDeviation(values);
            var scottH = 3.5d * deviation / Math.Pow(values.Length, 1d / 3d);

            if (double.IsNaN(scottH))
                scottH = 1;

            scottH = (double)RoundToSignificantDigits(scottH, 6);

            var minValue = values.Min();
            var startValue = minValue - scottH / 2;
            if (minValue < 0 && startValue < 0)
            {
                startValue = values.Min();
            }
            else if (startValue < 0)
            {
                startValue = 0;
            }

            var listValues = new List<double?>();
            var listArguments = new List<string>();

            var sumCurrent = 0;
            var index = 0;
            var indexRangeCurrent = 0;
            var endValueCurrnet = startValue + scottH;

            var area = series.Chart.Area as StiAxisArea;
            
            foreach (var valueCurrent in values.OrderBy(v => v.GetValueOrDefault()))
            {
                if (valueCurrent > endValueCurrnet || index == values.Length - 1)
                {
                    listValues.Add(sumCurrent);

                    var formatStartValue = formatService != null
                        ? formatService.Format(startValue)
                        : ((StiXAxisCoreXF)area.XAxis.Core).GetLabelText(startValue.GetValueOrDefault(), startValue.GetValueOrDefault(), series);

                    var formatEndValueCurrnet = formatService != null
                        ? formatService.Format(endValueCurrnet)
                        : ((StiXAxisCoreXF)area.XAxis.Core).GetLabelText(endValueCurrnet.GetValueOrDefault(), endValueCurrnet.GetValueOrDefault(), series);

                    var argumentCurrent = indexRangeCurrent == 0
                        ? $"[{formatStartValue}, {formatEndValueCurrnet})"
                        : $"({formatStartValue}, {formatEndValueCurrnet}]";

                    listArguments.Add(argumentCurrent);

                    sumCurrent = 0;
                    indexRangeCurrent++;
                    startValue = startValue + scottH * indexRangeCurrent;
                    endValueCurrnet = startValue + scottH;
                }

                index++;
                sumCurrent++;
            }

            series.Values = listValues.ToArray();
            series.Arguments = listArguments.ToArray();
            series.OriginalArguments = listArguments.ToArray();
            series.ValuesStart = null;
        }

        private static decimal RoundToSignificantDigits(double d, int digits)
        {
            if (d == 0)
                return 0;

            double scale = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(d))) + 1);
            return (decimal)(scale * Math.Round(d / scale, digits));
        }

        private static double GetStandardDeviation(double?[] values)
        {
            var xAverage = values.Sum() / values.Length;
            var t1 = values.Sum(v => Math.Pow(v.GetValueOrDefault() - xAverage.GetValueOrDefault(), 2));

            return Math.Sqrt(t1 / (values.Length - 1));
        }
        #endregion
    }
}
