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

namespace Stimulsoft.Base
{
    public class StiAzureTableStorageConnector : StiDbNoSqlDataConnector
    {
        #region Fields
        private StiDataAssemblyHelper assemblyHelper;
        private object azureTableStorageProvider;
        #endregion

        #region Properties
        /// <summary>
        /// Gets a type of the connection helper.
        /// </summary>
        public override StiConnectionIdent ConnectionIdent => StiConnectionIdent.AzureTableStorage;

        /// <summary>
        /// Gets an order of the connector.
        /// </summary>
        public override StiConnectionOrder ConnectionOrder => StiConnectionOrder.AzureTableStorage;

        public override string Name => "Azure Table Storage";

        public override bool AllowTestConnection
        {
            get
            {
                try
                {
                    var provider = GetAzureTableStorageProvider();
                    var method = provider.GetType().GetMethod("AllowTestConnection");
                    return (bool)method.Invoke(provider, null);
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets the package identificator for this connector.
        /// </summary>
        public override string[] NuGetPackages => new string[] { "Stimulsoft.Data.AzureTableStorage" };

        /// <summary>
        /// Gets the package version for this connector.
        /// </summary>
        public override string NuGetVersion => "2021.4.3";
        #endregion

        #region Methods
        private StiDataAssemblyHelper GetAssemblyData()
        {
            if (assemblyHelper != null) 
                return assemblyHelper;

            assemblyHelper = new StiDataAssemblyHelper(
                "Stimulsoft.Data.AzureTableStorage.dll", $"Stimulsoft.Data.AzureTableStorage-{NuGetVersion}");

            return assemblyHelper;
        }

        private object GetAzureTableStorageProvider()
        {
            if (this.azureTableStorageProvider != null)
                return this.azureTableStorageProvider;

            var assemblyBaseData = GetAssemblyData();
            this.azureTableStorageProvider = assemblyBaseData.CreateObject(
                "Stimulsoft.Data.AzureTableStorage.StiAzureTableStorageProvider", ConnectionString);

            return azureTableStorageProvider;
        }

        /// <summary>
        /// Returns StiTestConnectionResult that is the information of whether the connection string specified in this class is correct.
        /// </summary>
        /// <returns>The result of testing the connection string.</returns>
        public override StiTestConnectionResult TestConnection()
        {
            try
            {
                var provider = GetAzureTableStorageProvider();
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

        /// <summary>
        /// Returns schema object which contains information about structure of the database. Schema returned start at specified root element (if it applicable).
        /// </summary>
        public override StiDataSchema RetrieveSchema(bool allowException = false)
        {
            if (string.IsNullOrEmpty(this.ConnectionString))
                return null;

            var schema = new StiDataSchema(this.ConnectionIdent);

            try
            {
                var provider = GetAzureTableStorageProvider();
                var method = provider.GetType().GetMethod("GetTableNames");
                var tableNames = method.Invoke(provider, null) as List<string>;

                foreach (var name in tableNames)
                {
                    var tableSchema = StiDataTableSchema.NewTable(name);
                    try
                    {
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

        /// <summary>
        /// Returns sample of the connection string to this connector.
        /// </summary>
        public override string GetSampleConnectionString()
        {
            return StiDataOptions.SampleConnectionString.AzureTableStorage;
        }

        public override List<StiDataColumnSchema> GetColumns(string collectionName)
        {
            var dataTable = GetDataTable(collectionName, null, 0, 5);
            return dataTable != null 
                ? dataTable.Columns.Cast<DataColumn>().ToList().Select(c => new StiDataColumnSchema(c.ColumnName, c.DataType)).ToList() : null;
        }

        public override DataTable GetDataTable(string collectionName, string query, int? index = null, int? count = null)
        {
            try
            {
                var provider = GetAzureTableStorageProvider();

                var method = provider.GetType().GetMethod("GetTableJsonData");
                var dataJson = method.Invoke(provider, new object[] { collectionName, index, count }) as List<string>;

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
                return dataSet != null && dataSet.Tables.Count > 0 ? dataSet.Tables[0] : null;
            }
            catch (Exception exception)
            {
                if (exception.InnerException != null)
                    throw exception.InnerException;
                else
                    throw;
            }
        }

        public string ConvertDateTimeToJsonStr(DateTime date)
        {
            try
            {
                var provider = GetAzureTableStorageProvider();
                var method = provider.GetType().GetMethod("ConvertDateTimeToJsonStr");
                return (string)method.Invoke(provider, new object[] { date });
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region Methods.Static
        public static StiAzureTableStorageConnector Get(string connectionString = null)
        {
            return new StiAzureTableStorageConnector(connectionString);
        }
        #endregion

        public StiAzureTableStorageConnector(string connectionString = null)
            : base(connectionString)
        {
            this.FolderAssembly = $"Stimulsoft.Data.AzureTableStorage-{this.NuGetVersion}";
            this.NameAssembly = "Stimulsoft.Data.AzureTableStorage.dll";
        }
    }
}
