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
using System.Data;
using System.Linq;
using Stimulsoft.Base;
using Stimulsoft.Data.Extensions;
using Stimulsoft.Data.Functions;

namespace Stimulsoft.Data.Engine
{
    public static class StiDataFiltrator
    {
        #region Fields
        private static object lockObject = new object();
        private static Dictionary<string, StiDataTable> meterCache = new Dictionary<string, StiDataTable>();
        private static Dictionary<string, DataTable> netCache = new Dictionary<string, DataTable>();
        #endregion

        #region Methods
        public static DataTable Filter(DataTable inTable, IEnumerable<StiDataFilterRule> filters, IStiReport report, int hash)
        {
            if (filters == null || !filters.Any())
                return inTable;

            var columnNames = inTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();
            var columnTypes = inTable.Columns.Cast<DataColumn>().Select(c => c.DataType).ToList();

            var mapFilters = filters.Where(f => f.Condition == StiDataFilterCondition.MapEqualTo);
            filters = filters.Where(f => f.Condition != StiDataFilterCondition.MapEqualTo);

            var filter = StiDataFilterRuleHelper.GetDataTableFilterQuery(filters.ToList(), columnNames, columnTypes, report);
            if (string.IsNullOrWhiteSpace(filter) && !mapFilters.Any())
                return inTable;

            var outTable = GetFromCache(inTable, report, hash);
            if (outTable == DataTableExt.NullTable)
                return null;

            if (outTable != null)
                return outTable;

            if (!string.IsNullOrWhiteSpace(filter))
            {
                var prevFilter = inTable.DefaultView.RowFilter;

                try
                {
                    inTable.DefaultView.RowFilter = filter;
                }
                catch
                {
                    inTable.DefaultView.RowFilter = prevFilter;
                }
            }

            outTable = inTable.DefaultView.ToTable();

            if (mapFilters.Any())            
                outTable = FilterMapIdents(mapFilters, outTable);            

            AddToCache(inTable, report, hash, outTable);

            return outTable;
        }

        public static StiDataTable Filter(StiDataTable inTable, IEnumerable<StiDataFilterRule> filters, IStiReport report, int hash)
        {
            if (filters == null || !filters.Any())
                return inTable;

            var netTable = inTable.ToNetTable();
            var columnNames = netTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();
            var columnTypes = netTable.Columns.Cast<DataColumn>().Select(c => c.DataType).ToList();

            var filter = StiDataFilterRuleHelper.GetDataTableFilterQuery(filters.ToList(), columnNames, columnTypes, report);
            if (string.IsNullOrWhiteSpace(filter))
                return inTable;

            var outTable = GetFromCache(inTable, report, hash);
            if (outTable == StiDataTable.NullTable)
                return null;

            if (outTable != null)
                return outTable;

            netTable.DefaultView.RowFilter = filter;
            netTable = netTable.DefaultView.ToTable();

            var rows = netTable.DefaultView
                .ToTable()
                .AsEnumerableArray()
                .ToList();

            outTable = new StiDataTable(inTable.Meters, rows);

            AddToCache(inTable, report, hash, outTable);

            return outTable;
        }

        private static DataTable FilterMapIdents(IEnumerable<StiDataFilterRule> filters, DataTable outTable)
        {
            var idioms = filters.Where(f => !string.IsNullOrEmpty(f.Path)).GroupBy(f => f.Path).Select(g => new
            {
                ColumnId = outTable.Columns.IndexOf(g.Key),
                Idents = g.Select(f => Funcs.GetMapIdents(Simplify(f.Value)).Select(v => Simplify(v)).Distinct()).ToList()
            }).ToList();

            var rows = new Dictionary<DataRow, DataRow>();
            lock (rows)
            {
                foreach (var idiom in idioms)
                {
                    foreach (var identParent in idiom.Idents)
                    {
                        var isAdd = false;
                        foreach (var ident in identParent)
                        {
                            foreach (DataRow row in outTable.Rows)
                            {
                                if (!rows.ContainsKey(row) && ident == Simplify(row.Field<string>(idiom.ColumnId)))
                                {
                                    rows[row] = row;
                                    isAdd = true;
                                }
                            }
                            if (isAdd) break;
                        }
                    }
                }

                if (rows.Count > 0)
                    outTable = rows.Keys.CopyToDataTable();
                else
                    outTable.Rows.Clear();
            }

            return outTable;
        }

        private static string Simplify(string key)
        {
            return key?.ToLowerInvariant()?.Replace(" ", "")?.Replace("-", "");
        }
        #endregion

        #region Methods.Cache
        internal static void CleanCache(string appKey)
        {
            lock (lockObject)
            {
                if (appKey == null)
                {
                    netCache.Clear();
                    meterCache.Clear();
                }
                else
                {
                    netCache.Keys
                        .Where(k => k.StartsWith(appKey))
                        .ToList()
                        .ForEach(k => netCache.Remove(k));

                    meterCache.Keys
                        .Where(k => k.StartsWith(appKey))
                        .ToList()
                        .ForEach(k => meterCache.Remove(k));
                }
            }
        }

        private static string GetCacheKey(DataTable table, IStiApp app, int hash)
        {
            app?.SetKey(StiKeyHelper.GetOrGeneratedKey(app.GetKey()));
            var appKey = app?.GetKey() ?? string.Empty;

            var tableHash = table.Columns
                .Cast<DataColumn>()
                .Select(c => c.GetHashCode())
                .Aggregate(0, (c1, c2) => unchecked(c1 + c2));
            
            return $"{appKey}.{unchecked(tableHash + hash)}";
        }

        private static string GetCacheKey(StiDataTable table, IStiApp app, int hash)
        {
            app?.SetKey(StiKeyHelper.GetOrGeneratedKey(app.GetKey()));
            var appKey = app?.GetKey() ?? string.Empty;

            var tableHash = table.Meters
                .Select(m => m.GetUniqueCode())
                .Aggregate(0, (c1, c2) => unchecked(c1 + c2));

            return $"{appKey}.{unchecked(tableHash + hash)}";
        }

        private static DataTable GetFromCache(DataTable inTable, IStiApp app, int hash)
        {
            lock (lockObject)
            {
                var key = GetCacheKey(inTable, app, hash);
                return netCache.ContainsKey(key) ? netCache[key] : null;
            }
        }

        private static StiDataTable GetFromCache(StiDataTable inTable, IStiApp app, int hash)
        {
            lock (lockObject)
            {
                var key = GetCacheKey(inTable, app, hash);
                return meterCache.ContainsKey(key) ? meterCache[key] : null;
            }
        }

        private static void AddToCache(DataTable inTable, IStiApp app, int hash, DataTable outTable)
        {
            lock (lockObject)
            {
                var key = GetCacheKey(inTable, app, hash);

                if (outTable == null)
                    outTable = DataTableExt.NullTable;

                netCache[key] = outTable;
            }
        }

        private static void AddToCache(StiDataTable inTable, IStiApp app, int hash, StiDataTable outTable)
        {
            lock (lockObject)
            {
                var key = GetCacheKey(inTable, app, hash);

                if (outTable == null)
                    outTable = StiDataTable.NullTable;

                meterCache[key] = outTable;
            }
        }
        #endregion
    }
}