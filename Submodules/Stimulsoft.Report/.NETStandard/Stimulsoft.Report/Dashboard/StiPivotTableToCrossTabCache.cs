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
using Stimulsoft.Report.CrossTab;
using System.Collections.Generic;
using System.Linq;

namespace Stimulsoft.Report.Dashboard
{
    public static class StiPivotTableToCrossTabCache
    {
        #region Fields
        private static Dictionary<string, StiCrossTab> cache = new Dictionary<string, StiCrossTab>();
        #endregion

        #region Methods
        public static string GetKey(IStiPivotTableElement element)
        {
            var appKey = StiAppKey.GetOrGeneratedKey(element) ?? string.Empty;
            var elementKey = element?.GetKey() ?? string.Empty;
            var intKey = StiElementDataCache.GetKey(element) ?? string.Empty;

            return $"{appKey}.{elementKey}.{intKey}";
        }

        public static StiCrossTab Get(IStiPivotTableElement element)
        {
            lock (cache)
            {
                var key = GetKey(element);
                return cache.ContainsKey(key) ? cache[key] : null;
            }
        }

        public static void Put(IStiPivotTableElement element, StiCrossTab crossTab)
        {
            lock (cache)
            {
                var key = GetKey(element);
                cache[key] = crossTab;
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

        public static void Remove(string key)
        {
            lock (cache)
            {
                if (cache.ContainsKey(key))
                    cache.Remove(key);
            }
        }

        public static void Remove(IStiPivotTableElement element)
        {
            lock (cache)
            {
                var key = GetKey(element);
                if (cache.ContainsKey(key))
                    cache.Remove(key);
            }
        }

        public static void Clean(string reportKey)
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
