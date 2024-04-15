#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft BI												    }
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
using Stimulsoft.Report.Helpers;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Stimulsoft.Report.Dictionary
{
    public static class StiDataLeader
    {
        #region Fields
        private static object lockObject = new object();
        #endregion

        #region Methods
        private static List<StiDataSource> FetchAll(StiDatabase database, StiDictionary dictionary)
        {
            return dictionary.DataSources.ToList()
                .Where(d => d.GetCategoryName()?.ToLowerInvariant() == database.Name?.ToLowerInvariant()).ToList();
        }

        public static void RegData(StiDatabase database, StiDictionary dictionary, bool loadData)
        {
            if (ExistsInCache(database, dictionary))return;

            database?.RegData(dictionary, loadData);

            RegDataAfter(database, dictionary, loadData);
        }

        private static void RegDataAfter(StiDatabase database, StiDictionary dictionary, bool loadData)
        {
            if (!StiBIDataCacheOptions.Enabled) return;

            if (database is StiFileDatabase)
                loadData = true;

            if (!loadData) return;

            var dataSources = FetchAll(database, dictionary);
            foreach (StiDataSource dataSource in dataSources)
            {
                if (StiBIDataCacheHelper.Exists(dataSource)) continue;

                lock (lockObject)
                {
                    dataSource.Connect(loadData);
                    if (dataSource.DataTable != null && dataSource.DataTable.Columns.Count > 0)
                    {
                        StiBIDataCacheHelper.SaveData(dataSource, dataSource.DataTable);
                    }

                    dataSource.Disconnect();
                }
            }
        }

        public static bool ExistsInCache(StiDatabase database, StiDictionary dictionary)
        {
            if (!StiBIDataCacheOptions.Enabled)
                return false;

            var dataSources = FetchAll(database, dictionary);
            if (dataSources == null)
                return false;

            return dataSources.All(StiBIDataCacheHelper.Exists);
        }

        public static StiDataSchema RetrieveSchema(StiFileDataConnector connector, StiFileDataOptions options)
        {
            return connector?.RetrieveSchema(options);
        }

        public static StiDataSchema RetrieveSchema(StiSqlDataConnector connector, bool allowException = false)
        {
            return connector?.RetrieveSchema(allowException);
        }

        public static DataSet GetDataSet(StiFileDataConnector connector, StiFileDataOptions options)
        {
            return connector?.GetDataSet(options);
        }

        public static StiDataColumnsCollection GetColumnsFromData(StiDataAdapterService adapter,
            StiData data, StiDataSource dataSource, CommandBehavior retrieveMode = CommandBehavior.SchemaOnly)
        {
            var columns = adapter?.GetColumnsFromData(data, dataSource, retrieveMode);
            return columns == null ? new StiDataColumnsCollection() : columns;
        }

        public static void ConnectDataSourceToData(StiDataAdapterService adapter,
            StiDictionary dictionary, StiDataSource dataSource, bool loadData)
        {
            if (StiBIDataCacheOptions.Enabled && StiBIDataCacheHelper.Exists(dataSource))
            {
                dataSource.DataTable = StiBIDataCacheHelper.Get(dataSource, loadData);
                return;
            }
            
            adapter?.ConnectDataSourceToData(dictionary, dataSource, loadData);

            if (StiBIDataCacheOptions.Enabled && dataSource.DataTable != null &&
                dataSource.DataTable.Columns.Count > 0 && loadData && dataSource.DataTable.Rows.Count > 0)
            {
                lock (lockObject)
                {
                    StiBIDataCacheHelper.SaveData(dataSource, dataSource.DataTable);
                }
            }
        }

        public static void RetrieveData(StiSqlSource dataSource, bool schemaOnly = false)
        {
            dataSource?.RetrieveData(schemaOnly);
        }

        public static void Connect(StiDataSource dataSource, bool loadData = true, bool invokeEvents = true)
        {
            dataSource?.Connect(loadData, invokeEvents);
        }

        public static void Connect(StiDataSource dataSource, StiDataCollection datas, bool loadData = true)
        {
            dataSource?.Connect(datas, loadData);
        }

        public static void Disconnect(StiDataSource dataSource)
        {
            dataSource?.Disconnect();
        }
        #endregion
    }
}
