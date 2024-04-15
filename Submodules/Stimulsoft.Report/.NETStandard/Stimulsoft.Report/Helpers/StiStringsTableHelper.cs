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

using Stimulsoft.Base;
using Stimulsoft.Base.Helpers;
using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Data.Extensions;
using Stimulsoft.Report.Dashboard;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Stimulsoft.Report.Helpers
{
    public static class StiStringsTableHelper
    {
        #region Fields
        private static Hashtable rowsCache = new Hashtable();
        #endregion

        #region Methods
        public static DataTable LoadDataTableFromPackedString(string content, params string[] columns)
        {
            if (string.IsNullOrWhiteSpace(content)) 
                return null;

            var bytes = StiPacker.UnpackFromString(content);
            if (bytes == null || bytes.Length == 0) 
                return null;

            content = Encoding.UTF8.GetString(bytes);
            return LoadDataTableFromString(content, columns);
        }

        public static DataTable LoadDataTableFromString(string content, params string[] columns)
        {
            var rows = LoadStringRowsFromString(content, columns);
            return LoadDataTableFromStringRows(rows, columns);
        }

        public static DataTable LoadDataTableFromStringRows(List<string[]> rows, params string[] columns)
        {
            var dataTable = new DataTable().AddColumns(columns);
            dataTable.BeginLoadData();

            try
            {
                rows?.ForEach(r => dataTable.Rows.Add(r));
            }
            finally
            {
                dataTable.EndLoadData();
            }

            return dataTable;
        }

        public static List<string[]> LoadStringRowsFromPackedString(string content, params string[] columns)
        {
            if (string.IsNullOrWhiteSpace(content)) 
                return null;

            var bytes = StiPacker.UnpackFromString(content);
            if (bytes == null || bytes.Length == 0) 
                return null;

            content = Encoding.UTF8.GetString(bytes);
            return LoadStringRowsFromString(content, columns);
        }

        public static List<string[]> LoadColumnsAndStringRowsFromPackedString(string content, out string[] columns)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                columns = new string[0];
                return null;
            }

            var bytes = StiPacker.UnpackFromString(content);
            if (bytes == null || bytes.Length == 0)
            {
                columns = new string[0];
                return null;
            }

            content = Encoding.UTF8.GetString(bytes);
            return LoadColumnsAndStringRowsFromString(content, out columns);
        }

        public static List<string[]> LoadStringRowsFromString(string content, params string[] columns)
        {
            if (string.IsNullOrWhiteSpace(content))
                return null;

            var tokens = new List<object>();
            JsonConvert.PopulateObject(content, tokens);
            if (tokens.Count < 1)
                return null;

            var columnsList = columns.ToList();

            var headerIndexes = (tokens.FirstOrDefault() as JArray)
                .Values<string>()
                .Select(h => columnsList.IndexOf(h))
                .ToArray();

            tokens.RemoveAt(0);
            if (!tokens.Any())
                return null;

            var rows = new List<string[]>();

            foreach (JArray token in tokens)
            {
                var row = new string[columns.Count()];

                var headerIndex = 0;
                token.Values<string>().ToList().ForEach(v =>
                {
                    var columnIndex = headerIndexes[headerIndex++];
                    if (columnIndex != -1)
                        row[columnIndex] = v;
                });

                rows.Add(row);
            }
            return rows;
        }

        public static List<string[]> LoadColumnsAndStringRowsFromString(string content, out string[] columns)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                columns = new string[0];
                return null;
            }

            var tokens = new List<object>();
            JsonConvert.PopulateObject(content, tokens);
            if (tokens.Count < 1)
            {
                columns = new string[0];
                return null;
            }

            columns = (tokens.FirstOrDefault() as JArray).Values<string>().ToArray();

            tokens.RemoveAt(0);

            return tokens.Cast<JToken>()
                .Select(t => t.Values<string>().ToArray())
                .ToList();
        }

        public static string SaveDataTableToPackedString(DataTable table)
        {
            var content = SaveDataTableToString(table);
            if (string.IsNullOrWhiteSpace(content)) 
                return null;

            var bytes = Encoding.UTF8.GetBytes(content);
            return StiPacker.PackToString(bytes);
        }

        public static string SaveDataTableToString(DataTable table)
        {
            var list = SaveDataTableToStringRows(table);
            if (list == null || list.Count == 0)
                return null;

            return JsonConvert.SerializeObject(list, Formatting.Indented, StiJsonHelper.DefaultSerializerSettings);
        }

        public static List<string[]> SaveDataTableToStringRows(DataTable table, bool saveHeaders = true)
        {
            var list = new List<string[]>();

            if (table == null || table.Rows.Count == 0)
                return null;

            if (saveHeaders)
                list.Add(table.ColumnsList().Select(c => c.ColumnName).ToArray());

            foreach (DataRow row in table.Rows)
            {
                var strs = row.ItemArray.Select(i => i?.ToString()).ToArray();
                if (strs.All(s => string.IsNullOrWhiteSpace(s)))continue;

                list.Add(strs);
            }

            return list;
        }
        #endregion

        #region Methods.Cache
        public static List<string[]> LoadStringRowsFromCache(IStiElement element)
        {
            if (StiKeyHelper.IsEmptyKey(element?.Key)) 
                return null;

            lock (rowsCache)
            {
                if (rowsCache[element.Key] != null)
                    return rowsCache[element.Key] as List<string[]>;

                else
                    return null;
            }
        }

        public static List<string[]> LoadColumnsAndStringRowsFromCache(IStiElement element, out string[] columns)
        {
            if (StiKeyHelper.IsEmptyKey(element?.Key))
            {
                columns = new string[0];
                return null;
            }

            lock (rowsCache)
            {
                if (rowsCache[element.Key] != null)
                {
                    var rows = (rowsCache[element.Key] as List<string[]>).ToList();
                    if (rows.Any())
                    {
                        columns = rows.FirstOrDefault();
                        rows.RemoveAt(0);
                    }
                    else
                    {
                        columns = new string[0];
                    }
                    return rows;
                }
                else
                {
                    columns = new string[0];
                    return null;
                }
            }
        }

        public static void SaveDataTableToCache(IStiElement element, DataTable table, bool saveHeaders = false)
        {
            if (StiKeyHelper.IsEmptyKey(element?.Key) || table == null) return;

            SaveStringRowsToCache(element, SaveDataTableToStringRows(table, saveHeaders));
        }

        public static void SaveColumnsAndStringRowsToCache(IStiElement element, string[] columns, List<string[]> rows)
        {
            if (rows == null) return;

            rows = rows.ToList();
            rows.Insert(0, columns);
            SaveStringRowsToCache(element, rows);
        }

        public static void SaveStringRowsToCache(IStiElement element, List<string[]> rows)
        {
            if (StiKeyHelper.IsEmptyKey(element?.Key) || rows == null) return;

            lock (rowsCache)
            {
                rowsCache[element.Key] = rows;
            }
        }

        public static void RemoveFromCache(IStiElement element)
        {
            if (StiKeyHelper.IsEmptyKey(element?.Key)) return;

            lock (rowsCache)
            {
                if (rowsCache[element.Key] != null)
                    rowsCache.Remove(element.Key);
            }
        }
        #endregion
    }
}