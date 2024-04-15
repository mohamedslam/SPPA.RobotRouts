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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Stimulsoft.Base;
using Stimulsoft.Base.Meters;
using Stimulsoft.Data.Helpers;

namespace Stimulsoft.Data.Engine
{
    public static class StiDataJoiner
    {
        #region enum StiDataJoinEngine
        public enum StiDataJoinEngine
        {
            V1,
            V2,
            V3,
            V4,
            /// <summary>
            /// Adds support of multicolumn relations. V4 supports only first columns from relations.
            /// </summary>
            V5
        }
        #endregion

        #region Fields
        private static object lockObject = new object();
        private static Dictionary<string, DataTable> cache = new Dictionary<string, DataTable>();
        #endregion

        #region Properties
        public static StiDataJoinEngine JoinEngine { get; set; } = StiDataJoinEngine.V5;
        #endregion

        #region Methods
        public static DataTable Join(IEnumerable<DataTable> tables, List<StiDataLink> links, IEnumerable<IStiMeter> meters, IStiApp app)
        {
            if (tables.Count() < 2)
                return tables.FirstOrDefault();

            var resultTable = GetFromCache(tables, app);
            if (resultTable == DataTableExt.NullTable)
                return null;

            if (resultTable != null)
                return resultTable;

            var stackedTables = tables.ToList();

            var primaryTable = stackedTables.First();
            stackedTables = stackedTables.Skip(1).ToList();

            var onlyTableHash = new Hashtable();
            var onlyTables = new List<DataTable>();

            resultTable = primaryTable?.AsEnumerable().CopyToDataTableExt();
            CopyColumns(primaryTable, resultTable);

            var mergedTables = new List<DataTable> { primaryTable };
            var putOffTables = new List<DataTable>();

            while (stackedTables.Any())
            {
                var stackedTable = stackedTables.FirstOrDefault();
                if (stackedTable == null) continue;

                var mergedTable = mergedTables.FirstOrDefault(t => FindLink(stackedTable, t, links) != null);
                var dataRelation = FindLink(mergedTable, stackedTable, links);

                #region If a data relation can't be found right now then put off this table to the end of the list
                if ((JoinEngine == StiDataJoinEngine.V2 || JoinEngine == StiDataJoinEngine.V3 || JoinEngine == StiDataJoinEngine.V4 || JoinEngine == StiDataJoinEngine.V5) && 
                    dataRelation == null && stackedTables.Count > 2 && putOffTables.Count < stackedTables.Count)
                {
                    stackedTables.Remove(stackedTable);
                    stackedTables.Add(stackedTable);
                    putOffTables.Add(stackedTable);
                    continue;
                }   
                #endregion

                #region Find table and relation to merge
                if (mergedTable == null || dataRelation == null)
                {
                    stackedTables.Remove(stackedTable);

                    if (onlyTableHash[stackedTable] == null)
                        stackedTables.Add(stackedTable);
                    else
                        onlyTables.Add(stackedTable);

                    onlyTableHash[stackedTable] = stackedTable;

                    continue;
                }
                #endregion

                CopyColumns(stackedTable, resultTable);

                var destination = resultTable.Rows.Count == 0 ? mergedTable : resultTable;
                var rows = new StiDataRowJoiner(resultTable, destination, stackedTable)
                    .Join(StiDataJoinType.Left, dataRelation, meters);

                resultTable = rows.Any() ? rows.CopyToDataTable() : new DataTable();

                stackedTables.Remove(stackedTable);
                mergedTables.Add(stackedTable);
            }

            foreach (var table in onlyTables)
            {
                resultTable = MergeInSequence(resultTable, table);
            }

            AddToCache(tables, resultTable, app);

            return resultTable;
        }

        public static List<StiDataLink> FindPath(IEnumerable<string> tables, List<StiDataLink> links)
        {
            if (tables.Count() < 2)
                return null;

            var stackedTables = tables.ToList();
            var primaryTable = stackedTables.First();
            stackedTables = stackedTables.Skip(1).ToList();

            var relations = new List<StiDataLink>();

            var mergedTables = new List<string> { primaryTable };

            while (stackedTables.Any())
            {
                var stackedTable = stackedTables.FirstOrDefault();
                if (stackedTable == null) continue;

                var mergedTable = mergedTables.FirstOrDefault(t => FindLink(stackedTable, t, links) != null);
                var dataRelation = FindLink(mergedTable, stackedTable, links);
                if (dataRelation != null)
                    relations.Add(dataRelation);

                #region Find table and relation to merge
                if (mergedTable == null || dataRelation == null)
                {
                    stackedTables.Remove(stackedTable);
                    continue;
                }
                #endregion

                stackedTables.Remove(stackedTable);
                mergedTables.Add(stackedTable);
            }

            return relations;
        }

        private static void CopyColumns(DataTable from, DataTable to)
        {
            if (from == null || to == null) return;

            foreach (DataColumn column in from.Columns)
            {
                if (!to.Columns.Contains(column.ColumnName))
                    to.Columns.Add(new DataColumn(column.ColumnName, column.DataType));
            }
        }

        private static DataTable MergeInSequence(DataTable table1, DataTable table2)
        {
            var result = new DataTable();

            CopyColumns(table1, result);
            CopyColumns(table2, result);

            for (var rowIndex = 0; rowIndex < table1.Rows.Count; rowIndex++)
            {
                var row = result.NewRow();

                if (rowIndex < table1.Rows.Count)
                {
                    for (var index = 0; index < table1.Columns.Count; index++)
                    {
                        try
                        {
                            row[index] = table1.Rows[rowIndex][index];
                        }
                        catch
                        {
                        }
                    }
                }

                result.Rows.Add(row);
            }

            for (var rowIndex = 0; rowIndex < table2.Rows.Count; rowIndex++)
            {
                var row = result.NewRow();

                if (rowIndex < table2.Rows.Count)
                {
                    for (var index = 0; index < table2.Columns.Count; index++)
                    {
                        try
                        {
                            row[table1.Columns.Count + index] = table2.Rows[rowIndex][index];
                        }
                        catch
                        {
                        }
                    }
                }

                result.Rows.Add(row);
            }

            return result;
        }

        public static StiDataLink FindLink(DataTable table1, DataTable table2, List<StiDataLink> links)
        {
            return FindLink(table1?.TableName, table2?.TableName, links);
        }

        public static StiDataLink FindLink(string table1, string table2, List<StiDataLink> links)
        {
            if (table1 == null || table2 == null) return null;

            var relations = links.Where(l =>
                (l.ParentTable == table1 && l.ChildTable == table2) ||
                (l.ParentTable == table2 && l.ChildTable == table1));

            if (relations.Any(r => r.Active))
                return relations.First(r => r.Active);

            else if (relations.Any())
                return relations.FirstOrDefault();

            relations = links.Where(r =>
                (r.ParentTable == table1 && r.ChildTable == table2) ||
                (r.ParentTable == table2 && r.ChildTable == table1));

            if (relations.Any(r => r.Active))
                return relations.First(r => r.Active);

            else if (relations.Any())
                return relations.FirstOrDefault();

            else
                return null;
        }
        #endregion

        #region Methods.Cache
        private static string GetCacheKey(IEnumerable<DataTable> tables, IStiApp app)
        {
            app?.SetKey(StiKeyHelper.GetOrGeneratedKey(app.GetKey()));
            var appKey = app?.GetKey() ?? string.Empty;

            if (tables == null || tables.Count(t => t != null) == 0)
                return $"{appKey}.0";

            var hash = tables
                .Where(t => t != null)
                .SelectMany(t => t.Columns.Cast<DataColumn>())
                .Select(c => c.GetHashCode())
                .Aggregate(0, (c1, c2) => unchecked(c1 + c2));

            return $"{appKey}.{hash}";
        }

        private static DataTable GetFromCache(IEnumerable<DataTable> tables, IStiApp app)
        {
            lock (lockObject)
            {
                var key = GetCacheKey(tables, app);
                return cache.ContainsKey(key) ? cache[key] : null;
            }
        }

        private static void AddToCache(IEnumerable<DataTable> tables, DataTable dataTable, IStiApp app)
        {
            lock (lockObject)
            {
                var key = GetCacheKey(tables, app);

                if (dataTable == null)
                    dataTable = DataTableExt.NullTable;

                cache[key] = dataTable;
            }
        }

        internal static void CleanCache(string appKey)
        {
            lock (cache)
            {
                if (appKey == null)
                {
                    cache.Clear();
                }
                else
                {
                    cache.Keys
                        .Where(k => k.StartsWith(appKey))
                        .ToList()
                        .ForEach(k => cache.Remove(k));
                }
            }
        }
        #endregion
    }
}