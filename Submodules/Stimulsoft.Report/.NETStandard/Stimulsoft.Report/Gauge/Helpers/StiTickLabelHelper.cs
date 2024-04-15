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

using Stimulsoft.Report.Components.Gauge.Primitives;
using Stimulsoft.Report.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Stimulsoft.Report.Gauge.Helpers
{
    public static class StiTickLabelHelper
    {
        #region class CacheInfo
        private class CacheInfo
        {
            #region Properties
            public float ValueKey { get; }

            public string ValueStr { get; }

            public int Count { get; set; }
            #endregion

            #region Methods.override
            public override string ToString() => $"{ValueStr} - {Count}";
            #endregion

            public CacheInfo(float valueKey, string valueStr, int count)
            {
                this.ValueKey = valueKey;
                this.ValueStr = valueStr;
                this.Count = count;
            }
        }
        #endregion

        #region Methods
        public static Dictionary<float, string> GetLabels(Dictionary<float, float> collection, StiScaleBase scale)
        {
            var result = new Dictionary<float, string>();
            var cacheActualvalues = new List<CacheInfo>();

            if (scale.DateTimeMode)
            {
                #region Check All Values - Step1
                foreach (var pair in collection)
                {
                    var ticks = (long)pair.Key;
                    var valueDT = new DateTime(ticks);

                    var valueStr = scale.Gauge.ShortValue ? valueDT.ToShortDateString() : valueDT.ToLongDateString();

                    var cache = cacheActualvalues.FirstOrDefault(x => x.ValueStr == valueStr);
                    if (cache == null)
                        cacheActualvalues.Add(new CacheInfo(pair.Key, valueStr, 1));
                    else
                        cache.Count++;

                    result.Add(pair.Key, valueStr);
                }
                #endregion
            }
            else
            {
                #region Check All Values - Step1
                foreach (var pair in collection)
                {
                    double value = (pair.Key < 5)
                        ? Math.Round(pair.Key, 2)
                        : Math.Round(pair.Key);

                    if (value == 0f)
                    {
                        result.Add(pair.Key, "0");
                        continue;
                    }

                    var negative = false;
                    if (value < 0)
                    {
                        negative = true;
                        value = Math.Abs(value);
                    }

                    string valueStr;
                    if (value < 1000)
                        valueStr = value.ToString(CultureInfo.InvariantCulture);

                    else if (value < 1000000)
                        valueStr = $"{(int)(value / 1000)}K";

                    else if (value < 1000000000)
                        valueStr = $"{(int)(value / 1000000)}M";

                    else if (value < 1000000000000)
                        valueStr = $"{(int)(value / 1000000000)}B";

                    else if (value < 1000000000000000)
                        valueStr = $"{(int)(value / 1000000000000)}T";

                    else
                        valueStr = value.ToString(CultureInfo.InvariantCulture);

                    if (negative)
                        valueStr = $"-{valueStr}";

                    var cache = cacheActualvalues.FirstOrDefault(x => x.ValueStr == valueStr);
                    if (cache == null)
                        cacheActualvalues.Add(new CacheInfo(pair.Key, valueStr, 1));
                    else
                        cache.Count++;

                    result.Add(pair.Key, valueStr);
                }
                #endregion
            }

            #region check - step2
            if (!scale.DateTimeMode)
            {
                foreach (var cache in cacheActualvalues)
                {
                    if (cache.Count > 1)
                        Prepare(cache, ref result);
                }
            }
            #endregion

            return result;
        }

        private static void Prepare(CacheInfo cache, ref Dictionary<float, string> result)
        {
            var keys = result.Keys.ToArray();
            foreach (var key in keys)
            {
                var valueF = result[key];
                if (valueF != cache.ValueStr)continue;

                result[key] = StiAbbreviationNumberFormatHelper.Format(key);
            }
        }
        #endregion
    }
}