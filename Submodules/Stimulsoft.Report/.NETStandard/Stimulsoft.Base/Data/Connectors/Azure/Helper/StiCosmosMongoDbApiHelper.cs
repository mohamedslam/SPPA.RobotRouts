#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports  											}
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

using Stimulsoft.Base.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Stimulsoft.Base.Data.Connectors.Azure.Helper
{
    public class StiCosmosMongoDbApiHelper
    {
        #region Fields
        private static StiDataAssemblyHelper assemblyHelper;
        private string nugetVersion;
        private string connectionString;
        #endregion

        #region Methods
        public StiDataAssemblyHelper GetAssemblyData()
        {
            if (assemblyHelper != null) 
                return assemblyHelper;

            assemblyHelper = new StiDataAssemblyHelper("Stimulsoft.Data.CosmosDB.dll", $"Stimulsoft.Data.CosmosDB-{this.nugetVersion}");
            return assemblyHelper;
        }

        public StiTestConnectionResult TestConnection()
        {
            try
            {
                var provider = GetCosmosDbProvider();
                var method = provider.GetType().GetMethod("TestConnection");
                var testConnection = (bool)method.Invoke(provider, null);

                return testConnection 
                    ? StiTestConnectionResult.MakeFine() 
                    : StiTestConnectionResult.MakeWrong("Couldn't open connection to server");
            }
            catch (Exception exception)
            {
                if (exception.InnerException != null)
                    return StiTestConnectionResult.MakeWrong(exception.InnerException.Message);
                else
                    return StiTestConnectionResult.MakeWrong(exception.Message);
            }
        }

        public static string GetSampleConnectionString()
        {
            return StiDataOptions.SampleConnectionString.CosmosMongoDb;
        }

        public object GetCosmosDbProvider()
        {
            return GetAssemblyData().CreateObject("Stimulsoft.Data.CosmosDB.StiCosmosDbMongoDbApiProvider", connectionString);
        }

        public StiDataSchema RetrieveSchema(bool allowException = false)
        {
            if (string.IsNullOrEmpty(connectionString)) 
                return null;

            var schema = new StiDataSchema(StiConnectionIdent.CosmosDbDataSource);

            try
            {
                var provider = GetCosmosDbProvider();
                var method = provider.GetType().GetMethod("GetTableNames");
                var tableNames = method.Invoke(provider, null) as List<string>;

                foreach (var name in tableNames)
                {
                    var tableSchema = StiDataTableSchema.NewTable(name);
                    try
                    {
                        if (name == "system.indexes") continue;
                        if (name == "system.users") continue;
                        if (name == "system.profile") continue;

                        var columns = GetColumns(name);
                        if (columns != null)
                            tableSchema.Columns = columns;
                    }
                    catch
                    {
                    }

                    schema.Tables.Add(tableSchema);
                }

                return schema.Sort();
            }
            catch (Exception)
            {
                if (allowException) throw;

                return null;
            }
        }

        public List<StiDataColumnSchema> GetColumns(string collectionName)
        {
            var dataTable = GetDataTable(collectionName, null, 0, 5);
            return dataTable != null 
                ? dataTable.Columns.Cast<DataColumn>().ToList().Select(c => new StiDataColumnSchema(c.ColumnName, c.DataType)).ToList() : null;
        }

        public DataTable GetDataTable(string collectionName, string query, int? index = null, int? count = null)
        {
            try
            {
                var provider = GetCosmosDbProvider();

                var method = provider.GetType().GetMethod("GetTableJsonQueryData");
                var dataJson = method.Invoke(provider, new object[] { collectionName, query, index, count }) as List<string>;

                var list = new List<JToken>();

                foreach (var jStr in dataJson)
                {
                    try
                    {
                        var jToken = JToken.Parse(jStr);
                        list.Add(jToken);
                    }
                    catch
                    {

                    }
                }

                var dataSet = StiJsonToDataSetConverter.GetDataSet(list, true);
                return dataSet != null && dataSet.Tables.Count > 0 ? dataSet.Tables[0].Copy() : null;
            }
            catch (Exception exception)
            {
                if (exception.InnerException != null)
                    throw exception.InnerException;
                else
                    throw;
            }
        }
        #endregion

        public StiCosmosMongoDbApiHelper(string connectionString, string nugetVersion)
        {
            this.nugetVersion = nugetVersion;
            this.connectionString = connectionString.Replace("\r\n", "").Replace(" ", ""); ;
        }
    }
}