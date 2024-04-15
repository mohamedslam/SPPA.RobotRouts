#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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
{	TRADE SECRETS OF STIMULSOFT										}
{																	}
{	CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON		}
{	ADDITIONAL RESTRICTIONS.										}
{																	}
{*******************************************************************}
*/
#endregion Copyright (C) 2003-2022 Stimulsoft

using Stimulsoft.Base;
using System.Collections.Generic;
using System.Linq;

namespace Stimulsoft.Report.Dashboard
{
    public static class StiPivotToConvertedStateCache
    {
        #region Fields
        private static Dictionary<string, bool> cache = new Dictionary<string, bool>();
        #endregion

        #region Methods
        private static string GetIntKey(IStiPivotTableElement element)
        {
            var meters = element?.GetMeters();
            if (meters == null || !meters.Any())
                return "0";

            var index = 0;
            var keys = meters.Select(m => index++.ToString() + m.GetUniqueCode().ToString()).ToList();

            var title = (element as IStiTitleElement)?.Title?.Text ?? string.Empty;
            keys.Add(title.GetHashCode().ToString());

            var hashKeys = 0L;
            foreach (var key in keys)
            {
                hashKeys = unchecked(hashKeys + key.GetHashCode());
            }

            return $"{hashKeys}";
        }

        private static string GetKey(IStiPivotTableElement element)
        {
            var appKey = StiAppKey.GetOrGeneratedKey(element) ?? string.Empty;
            var elementKey = element?.GetKey() ?? string.Empty;
            var intKey = GetIntKey(element) ?? string.Empty;

            return $"{appKey}.{elementKey}.{intKey}";
        }

        public static bool IsConverted(IStiPivotTableElement element)
        {
            lock (cache)
            {
                var key = GetKey(element);
                return cache.ContainsKey(key) && cache[key];
            }
        }

        public static void PutTrue(IStiPivotTableElement element)
        {
            Put(element, true);
        }

        public static void PutFalse(IStiPivotTableElement element)
        {
            Put(element, false);
        }

        private static void Put(IStiPivotTableElement element, bool converted)
        {
            lock (cache)
            {
                var key = GetKey(element);
                cache[key] = converted;
            }
        }

        public static bool Contains(IStiPivotTableElement element)
        {
            lock (cache)
            {
                var key = GetKey(element);
                return cache.ContainsKey(key);
            }
        }

        public static void Clean(string reportKey = null)
        {
            lock (cache)
            {
                if (reportKey == null)
                {
                    cache.Clear();
                }
                else
                {
                    cache.Keys
                        .Where(k => k.StartsWith(reportKey))
                        .ToList()
                        .ForEach(k => cache.Remove(k));
                }
            }
        }
        #endregion
    }
}
