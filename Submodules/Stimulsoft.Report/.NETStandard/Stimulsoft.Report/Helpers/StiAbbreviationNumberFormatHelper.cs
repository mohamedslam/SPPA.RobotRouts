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

using System;
using System.Globalization;
using System.Threading;

namespace Stimulsoft.Report.Helpers
{
    public static class StiAbbreviationNumberFormatHelper
    {
        public static string Format(double value)
        {
            return Format((decimal)value);
        }

        public static string Format(decimal value)
        {
            string postfix;
            var result = Format(value, out postfix);

            if (result == 0m)
                return "0";

            else if (postfix?.Length == 0)
                return Math.Round(value, 2).ToString(CultureInfo.InvariantCulture);

            else
                return $"{result}{postfix}";
        }

        private static readonly int[] indianThresholds = { 3, 5, 7, 10, 12 };
        private static readonly string[] indianPostfixes = { "K", "L", "Cr", "K Cr", "L Cr" };
        private static readonly int[] defaultThresholds = { 3, 6, 9, 12, 15, 18, 21, 24, 27 };
        private static readonly string[] defaultPostfixes = { "K", "M", "B", "T", "q", "Q", "s", "S", "O" };
        private static readonly string[] russianPostifxes = { " тыс.", " млн", " млрд", " трлн", " квдрлн", " квнтлн", " скстлн", " сптлн", " ктлн" };
            
        public static decimal Format(decimal value, out string postfix, int decimalDigits = 0, int? totalNumberCapacity = null)
        {
            postfix = "";
            if (value == 0)
                return 0;

            var negative = false;
            if (value < 0)
            {
                negative = true;
                value = Math.Abs(value);
            }

            int pow = (int)Math.Log10((double)value);
            int level = pow / 3;
            
            decimal result;
            decimal threshold = 1;
            if (level < 0 || level > 21)
            {
                result = value;
            }
            else
            {
                int[] thresholds;
                string[] postfixes;

                if (StiOptions.Engine.Formats.UseAbbreviationForIndia)
                {
                    thresholds = indianThresholds;
                    postfixes = indianPostfixes;
                }
                else
                {
                    thresholds = defaultThresholds;
                    var isNoRu = !Thread.CurrentThread.CurrentCulture.Name.Equals("ru-RU");
                    postfixes = isNoRu ? defaultPostfixes : russianPostifxes;
                }

                for (var i = thresholds.Length - 1; i >= 0; i--)
                {
                    if (pow >= thresholds[i])
                    { 
                        postfix = postfixes[i];
                        threshold = (decimal)Math.Pow(10, thresholds[i]);
                        break;
                    }
                }

                result = value / threshold;

                #region DBS
                if (decimalDigits == 0 && totalNumberCapacity != null)
                {
                    if (level == 0)
                        decimalDigits = totalNumberCapacity.GetValueOrDefault();
                    else
                        decimalDigits = level * 4 - totalNumberCapacity.GetValueOrDefault();
                }
                #endregion

                if (decimalDigits > 0)
                {
                    result = Math.Truncate(result);
                    var round = Math.Round(value / threshold, Math.Min(28, decimalDigits));

                    //fix case, sample: 98.9999 - issue #2677
                    var delta = round - Math.Truncate(round);
                    if (delta == 0 && round - result == 1)
                        delta = 1;

                    result += delta;
                }
                else
                {
                    result = Math.Round(result);
                }
            }

            if (negative)
                result = -result;

            return result;
        }
    }
}