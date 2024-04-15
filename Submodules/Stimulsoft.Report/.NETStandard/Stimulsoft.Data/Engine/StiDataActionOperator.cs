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
    public static class StiDataActionOperator
    {
        #region Fields
        private static object lockObject = new object();
        private static Dictionary<string, DataTable> netCache = new Dictionary<string, DataTable>();
        private static Dictionary<string, StiDataTable> meterCache = new Dictionary<string, StiDataTable>();
        #endregion

        #region Methods
        public static DataTable Apply(DataTable inTable, IEnumerable<StiDataActionRule> actions, IStiReport report, int hash)
        {
            if (actions == null)
                return inTable;

            actions = actions.Where(a => a.Priority == StiDataActionPriority.BeforeTransformation);
            if (!actions.Any())
                return inTable;

            var outTable = GetFromCache(inTable, report, hash);
            if (outTable == DataTableExt.NullTable)
                return null;

            if (outTable == null)
            {
                outTable = inTable.Copy();
                
                var columnNames = inTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();
                StiDataActionRuleHelper.ApplyActions(outTable, actions.ToList(), null, columnNames, report);

                AddToCache(inTable, report, hash, outTable);
            }
            
            return outTable;
        }

        public static StiDataTable ApplyAfterTransformation(StiDataTable inTable, IEnumerable<StiDataActionRule> actions, 
            StiDataActionPriority priority, IStiReport report, int hash)
        {
            if (actions == null)
                return inTable;

            actions = actions.Where(a => a.Priority == priority);
            if (!actions.Any())
                return inTable;

            var outTable = GetFromCache(inTable, report, hash);
            if (outTable == null)
            {
                var netTable = inTable.ToNetTable();
                var columnKeys = inTable.Meters.Select(m => m.Key).ToList();
                var columnNames = netTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();
                StiDataActionRuleHelper.ApplyActions(netTable, actions.ToList(), columnKeys, columnNames, report);

                var rows = netTable.DefaultView
                    .ToTable()
                    .AsEnumerableArray()
                    .ToList();

                outTable = new StiDataTable(inTable.Meters, rows);
                AddToCache(inTable, report, hash, outTable);
            }

            return outTable;
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
                .Select(c => c.GetHashCode())
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
                if (outTable == null)
                    outTable = DataTableExt.NullTable;

                var key = GetCacheKey(inTable, app, hash);
                netCache[key] = outTable;
            }
        }
        
        private static void AddToCache(StiDataTable inTable, IStiApp app, int hash, StiDataTable outTable)
        {
            lock (lockObject)
            {
                if (outTable == null)
                    outTable = StiDataTable.NullTable;

                var key = GetCacheKey(inTable, app, hash);
                meterCache[key] = outTable;
            }
        }
        #endregion
    }
}