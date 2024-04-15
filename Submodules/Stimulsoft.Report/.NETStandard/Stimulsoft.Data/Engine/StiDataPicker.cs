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

using Stimulsoft.Base;
using Stimulsoft.Data.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Stimulsoft.Data.Engine
{
    public class StiDataPicker
    {
        #region Fields.Static
        private static object lockObject = new object();
        private static Dictionary<string, DataTable> cache = new Dictionary<string, DataTable>();
        #endregion

        #region Methods
        /// <summary>
        /// Returns all data tables which is used in all elements of the dashboard
        /// </summary>
        public static IEnumerable<DataTable> Fetch(IStiQueryObject query, string group, StiDataRequestOption option = StiDataRequestOption.All,
            IEnumerable<string> filterNames = null, List<StiDataLink> links = null)
        {
            if (query == null)
                return null;

            var dataNames = query.RetrieveUsedDataNames(group);
            if (filterNames != null)
                dataNames = dataNames.Union(filterNames).ToList();

            var dataSources = query.GetDataSources(dataNames);
            if (dataSources == null || !dataSources.Any())
                return null;
            
            dataSources = StiDataJoiner.JoinEngine == StiDataJoiner.StiDataJoinEngine.V1
                ? dataSources
                : dataSources.Where(d => d != null).OrderBy(d => GetRelationLevel(d, links)).ToList();

            dataSources = StiDataSourceChainFinder.Find(dataSources);

            var connections = dataSources
                .Select(d => d.GetConnection())
                .Where(d => d != null);//Take only connections for the specified datasources

            if (dataSources.All(ExistsInCache))
                return dataSources.Select(GetFromCache);

            var dictionary = query.GetDictionary();
            IEnumerable<IStiAppConnection> openedConnections = null;

            try
            {
                openedConnections = (option & StiDataRequestOption.AllowOpenConnections) > 0
                    ? dictionary.OpenConnections(connections)
                    : null;

                var dataTables = new List<DataTable>();
                foreach (var dataSource in dataSources)
                {
                    var dataTable = GetDataTable(option, dataSource);

                    AddToCache(dataSource, dataTable);

                    if (dataTable == null) continue;

                    dataTables.Add(dataTable);
                    AddTableNameToColumnNames(dataTable, dataSource);
                }
                return dataTables;
            }
            finally
            {
                if ((option & StiDataRequestOption.AllowOpenConnections) > 0 && openedConnections != null && openedConnections.Any())
                    dictionary.CloseConnections(openedConnections);
            }            
        }

        private static int GetRelationLevel(IStiAppDataSource dataSource, List<StiDataLink> links)
        {
            if (!links.Any(l => l.ParentTable == dataSource.GetName() || l.ChildTable == dataSource.GetName()))
                return 100;

            var skipTable = new Hashtable();
            var level = 0;
            var table = dataSource.GetName();

            while (true)
            {
                if (skipTable[table] != null) break;
                skipTable[table] = table;

                table = links.FirstOrDefault(l => l.ChildTable == table)?.ParentTable;
                if (table == null) break;
                level++;
                if (level > 100) return 100;
            }

            return level;
        }

        /// <summary>
        /// Fetches all used data sources in the specified query.
        /// </summary>
        public static IEnumerable<IStiAppDataSource> RetrieveUsedDataSources(IStiQueryObject query, string group, IEnumerable<string> filterNames)
        {
            if (query == null)
                return null;

            var dataNames = query.RetrieveUsedDataNames(group);
            if (filterNames != null)
                dataNames = dataNames.Union(filterNames).ToList();

            var dataSources = query.GetDataSources(dataNames);
            if (dataSources == null || !dataSources.Any())
                return null;

            return StiDataSourceChainFinder.Find(dataSources);
        }

        public static DataTable Fetch(IStiApp app, string dataSourceName, StiDataRequestOption option = StiDataRequestOption.All)
        {
            var dataSource = app.GetDictionary().GetDataSourceByName(dataSourceName);

            return Fetch(app, dataSource, option);
        }

        public static DataTable Fetch(IStiApp app, IStiAppDataSource dataSource, StiDataRequestOption option = StiDataRequestOption.All)
        {
            var dict = app.GetDictionary();

            if (!ExistsInCache(dataSource))
            {
                var connection = dataSource.GetConnection();
                dict.OpenConnections(new[] { connection });

                try
                {
                    var dataTable = GetDataTable(option, dataSource);

                    AddToCache(dataSource, dataTable);
                    AddTableNameToColumnNames(dataTable, dataSource);

                    return dataTable;
                }
                finally
                {
                    dict.CloseConnections(new[] { connection });
                }
            }
            else
            {
                return GetFromCache(dataSource);
            }
        }

        public static bool IsAllBICached(IStiQueryObject query, string group, StiDataRequestOption option = StiDataRequestOption.All)
        {
            if (!StiBIDataCacheOptions.Enabled)
                return false;

            if (query == null)
                return false;

            var dataNames = query.RetrieveUsedDataNames(group);
            var dataSources = query.GetDataSources(dataNames);
            if (dataSources == null || !dataSources.Any())
                return false;

            dataSources = StiDataSourceChainFinder.Find(dataSources);
            return dataSources.All(d => StiBIDataCacheOptions.Cache.Exists(d));
        }
        
        public static DataTable GetDataTable(IStiApp app, IStiAppDataSource dataSource, StiDataRequestOption option = StiDataRequestOption.All)
        {
            var dict = app.GetDictionary();

            var connection = dataSource.GetConnection();
            dict.OpenConnections(new[] { connection });

            try
            {
                return GetDataTable(option, dataSource);
            }
            finally
            {
                dict.CloseConnections(new[] { connection });
            }
        }


        internal static DataTable GetDataTable(StiDataRequestOption option, IStiAppDataSource dataSource)
        {
            var dataTable = dataSource.GetDataTable((option & StiDataRequestOption.AllowOpenConnections) > 0);
            if (dataTable != null)
            {
                dataTable = dataTable.Copy();
                dataTable.TableName = dataSource.GetName();
            }

            return ProcessCalculatedColumns(dataTable, dataSource);
        }

        internal static DataTable ProcessCalculatedColumns(DataTable dataTable, IStiAppDataSource dataSource)
        {
            if (dataTable == null)
                return null;

            var calcColumns = dataSource.FetchColumns().Where(c => c is IStiAppCalcDataColumn);
            if (!calcColumns.Any())
                return dataTable;

            foreach (var column in calcColumns)
            {
                if (!dataTable.Columns.Contains(column.GetName()))
                    dataTable.Columns.Add(column.GetName(), column.GetDataType());
            }

            var columnNames = calcColumns.Select(c => c.GetName()).ToArray();
            var values = dataSource.FetchColumnValues(columnNames).ToArray();

            if (!values.Any())
                return dataTable;

            try
            {
                dataTable.BeginLoadData();

                var dataColumnIndexes = calcColumns.Select(c => dataTable.Columns.IndexOf(c.GetName()));
                var valueIndex = 0;
                foreach (DataRow row in dataTable.Rows)
                {
                    try
                    {
                        var items = valueIndex >= values.Length ? values[values.Length - 1] : values[valueIndex];
                        row.BeginEdit();
                        var itemIndex = 0;
                        foreach (var index in dataColumnIndexes)
                        {
                            if (itemIndex >= items.Length)break;

                            row[index] = items[itemIndex++];
                        }

                        row.EndEdit();
                    }
                    catch
                    {
                    }

                    valueIndex++;
                }
            }
            finally
            {
                dataTable.AcceptChanges();
                dataTable.EndLoadData();
            }

            return dataTable;
        }

        internal static void AddTableNameToColumnNames(DataTable table, IStiAppDataSource dataSource)
        {
            if (table == null) return;

            var columns = dataSource.FetchColumns();

            var tablePrefix = $"{table.TableName}.";
            foreach (DataColumn tableColumn in table.Columns)
            {
                //A table column name already contains table name like a prefix
                if (tableColumn.ColumnName.StartsWith(tablePrefix))continue;
                
                //Find a column in a data source based on its NameInSource property
                var column = columns.FirstOrDefault(c => c.GetNameInSource() == tableColumn.ColumnName);

                tableColumn.ColumnName = column?.GetName() != column?.GetNameInSource()
                    ? $"{tablePrefix}{column.GetName()}"
                    : $"{tablePrefix}{tableColumn.ColumnName}";
            }
        }
        #endregion

        #region Methods.Cache
        public static DataTable GetFromCache(IStiAppDataSource dataSource)
        {
            lock (lockObject)
            {
                var key = GetCacheKey(dataSource);
                return cache.ContainsKey(key) ? cache[key] : null;
            }
        }

        public static bool ExistsInCache(IStiAppDataSource dataSource)
        {
            lock (lockObject)
            {
                var key = GetCacheKey(dataSource);
                return cache.ContainsKey(key);
            }
        }

        internal static void AddToCache(IStiAppDataSource dataSource, DataTable dataTable)
        {
            lock (lockObject)
            {
                if (dataTable == null)
                    dataTable = DataTableExt.NullTable;

                var key = GetCacheKey(dataSource);
                cache[key] = dataTable;
            }
        }

        private static string GetCacheKey(IStiAppDataSource dataSource)
        {
            if (dataSource == null)
                return null;

            var appKey = StiAppKey.GetOrGeneratedKey(dataSource) ?? string.Empty;
            var connection = dataSource?.GetConnection();
            var dataSourceKey = dataSource.GetNameInSource() == dataSource.GetName()
                ? dataSource.GetName()
                : $"{dataSource.GetNameInSource()}.{dataSource.GetName()}";

            return connection == null
                ? $"{appKey}.{dataSourceKey}"
                : $"{appKey}.{connection.GetName()}.{dataSourceKey}";
        }

        public static void CleanCache(string appKey)
        {
            lock (lockObject)
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