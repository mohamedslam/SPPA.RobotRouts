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

using System;
using System.Data;

namespace Stimulsoft.Base
{
    public static class StiBIDataCacheHelper
    {
        #region Methods
        private static void CheckInitialization()
        {
            if (StiBIDataCacheOptions.Cache == null)
                throw new Exception("The StiBIDataCacheHelper.Cache property is not initialized!");
        }

        private static string GetKey(IStiAppDataSource dataSource)
        {
            //return dataSource?.GetKey();
            return dataSource?.GetNameInSource();
        }

        private static string GetAppKey(IStiAppDataSource dataSource)
        {
            return dataSource?.GetDictionary()?.GetApp()?.GetKey();
        }

        public static bool Exists(IStiAppDataSource dataSource)
        {
            CheckInitialization();

            return StiBIDataCacheOptions.Cache.Exists(GetKey(dataSource));
        }

        public static void Remove(IStiAppDataSource dataSource)
        {
            CheckInitialization();

            StiBIDataCacheOptions.Cache.Remove(GetKey(dataSource));
        }

        public static void Clean(string appKey)
        {
            CheckInitialization();

            StiBIDataCacheOptions.Cache.Clean(appKey);
        }

        public static void CleanAll()
        {
            CheckInitialization();

            StiBIDataCacheOptions.Cache.CleanAll();
        }

        public static long GetTableCount()
        {
            CheckInitialization();

            return StiBIDataCacheOptions.Cache.GetTableCount();
        }

        public static long GetRowCount(IStiAppDataSource dataSource)
        {
            CheckInitialization();

            return StiBIDataCacheOptions.Cache.GetRowCount(GetKey(dataSource));
        }

        public static DataTable RunQuery(string query)
        {
            CheckInitialization();

            return StiBIDataCacheOptions.Cache.RunQuery(query);
        }

        public static DataTable Get(IStiAppDataSource dataSource, bool loadData = false)
        {
            CheckInitialization();

            if (loadData)
                return StiBIDataCacheOptions.Cache.GetData(GetKey(dataSource));
            else
                return StiBIDataCacheOptions.Cache.GetSchema(GetKey(dataSource));
        }

        public static void SaveData(IStiAppDataSource dataSource, DataTable dataTable)
        {
            CheckInitialization();

            StiBIDataCacheOptions.Cache.SaveData(GetAppKey(dataSource), GetKey(dataSource), dataTable);
        }

        public static string GetTableName(IStiAppDataSource dataSource)
        {
            //return StiBIDataCacheOptions.Cache.GetTableName(appKey, tableKey);
            return null;
        }
        #endregion
    }
}
