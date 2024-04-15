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

using Stimulsoft.Data.Extensions;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Stimulsoft.Base;

namespace Stimulsoft.Data.Engine
{
    public static class StiDataSorter
    {
        #region Fields
        private static object lockObject = new object();
        private static Dictionary<string, StiDataTable> hashCache = new Dictionary<string, StiDataTable>();
        #endregion

        #region Methods
        public static StiDataTable Sort(StiDataTable inTable, 
            IEnumerable<StiDataSortRule> sorts, IStiApp app, int hash,
            StiDataRequestOption option = StiDataRequestOption.All)
        {
            if (StiDataSortVariation.IsVariationSort(sorts))
                return inTable;

            var allowSort = (option & StiDataRequestOption.AllowDataSort) > 0;
            if (sorts == null || !sorts.Any() || !allowSort)
                return inTable;

            var netTable = inTable.ToNetTable();

            netTable.Columns.Cast<DataColumn>().ToList().ForEach(c =>
            {
                if (c.ColumnName.Contains(","))
                    c.ColumnName = c.ColumnName.Replace(",", "");
            });

            var columnKeys = inTable.Meters.Select(m => m.Key).ToList();
            var columnNames = netTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();
            
            sorts = GetFixedDataSortRules(sorts, columnNames, columnKeys);
            var sort = StiDataSortRuleHelper.GetDataTableSortQuery(sorts.ToList(), columnKeys, columnNames);
            if (string.IsNullOrWhiteSpace(sort))
                return inTable;

            var outTable = GetFromCache(inTable, app, hash);
            if (outTable == StiDataTable.NullTable)
                return null;

            if (outTable != null)
                return outTable;

            netTable.DefaultView.Sort = sort;

            var rows = netTable.DefaultView.ToTable()
                .AsEnumerableArray()
                .ToList();

            outTable = new StiDataTable(inTable.Meters, rows);

            AddToCache(inTable, app, hash, outTable);

            return outTable;
        }

        /// <summary>
        /// Replaces column name which used as key by real column key.
        /// </summary>
        private static IEnumerable<StiDataSortRule> GetFixedDataSortRules(IEnumerable<StiDataSortRule> sorts, List<string> columnNames, List<string> columnKeys)
        {
            var sortList = sorts.Select(s => s.Clone() as StiDataSortRule).ToList();

            foreach (var sort in sortList)
            {
                var index = columnNames.IndexOf(sort.Key);
                if (index != -1)
                    sort.Key = columnKeys[index];
            }

            return sortList;
        }
        #endregion

        #region Methods.Cache
        internal static void CleanCache(string appKey)
        {
            lock (lockObject)
            {
                if (appKey == null)
                {
                    hashCache.Clear();
                }
                else
                {
                    hashCache.Keys
                        .Where(k => k.StartsWith(appKey))
                        .ToList()
                        .ForEach(k => hashCache.Remove(k));
                }
            }
        }

        private static string GetCacheKey(StiDataTable table, IStiApp app, int hash)
        {
            app?.SetKey(StiKeyHelper.GetOrGeneratedKey(app.GetKey()));
            var appKey = app?.GetKey() ?? string.Empty;

            var tableHash = table.Meters
                .Select(c => c.GetHashCode())
                .Aggregate(0, (c1, c2) => unchecked(c1 + c2));

            return $"{appKey}.{unchecked(tableHash + hash)}";
        }

        private static StiDataTable GetFromCache(StiDataTable inTable, IStiApp app, int hash)
        {
            lock (lockObject)
            {
                var key = GetCacheKey(inTable, app, hash);
                return hashCache.ContainsKey(key) ? hashCache[key] : null;
            }
        }

        private static void AddToCache(StiDataTable inTable, IStiApp app, int hash, StiDataTable outTable)
        {
            lock (lockObject)
            {
                var key = GetCacheKey(inTable, app, hash);

                if (outTable == null)
                    outTable = StiDataTable.NullTable;

                hashCache[key] = outTable;
            }
        }
        #endregion
    }
}