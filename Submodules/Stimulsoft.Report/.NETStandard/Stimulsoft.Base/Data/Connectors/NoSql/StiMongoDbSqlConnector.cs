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
using System.Linq;

namespace Stimulsoft.Base
{
    public class StiMongoDbConnector : StiDbNoSqlDataConnector
    {
        #region Fields
        private StiDataAssemblyHelper assemblyHelper;
        private object mongoProvider;
        #endregion

        #region Properties
        /// <summary>
        /// Gets a type of the connection helper.
        /// </summary>
        public override StiConnectionIdent ConnectionIdent => StiConnectionIdent.MongoDbDataSource;

        /// <summary>
        /// Gets an order of the connector.
        /// </summary>
        public override StiConnectionOrder ConnectionOrder => StiConnectionOrder.MongoDbDataSource;

        public override string Name => "MongoDB";

        public override bool AllowTestConnection
        {
            get
            {
                try
                {
                    var provider = GetMongoDBProvider();
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
        public override string[] NuGetPackages => new string[] { "Stimulsoft.Data.MongoDB" };

        /// <summary>
        /// Gets the package version for this connector.
        /// </summary>
        public override string NuGetVersion => "2022.3.1";
        #endregion

        #region Methods
        private StiDataAssemblyHelper GetAssemblyData()
        {
            if (assemblyHelper != null) 
                return assemblyHelper;

            assemblyHelper = new StiDataAssemblyHelper("Stimulsoft.Data.MongoDB.dll", $"Stimulsoft.Data.MongoDB-{this.NuGetVersion}");
            return assemblyHelper;
        }

        private object GetMongoDBProvider()
        {
            if (this.mongoProvider != null)
                return this.mongoProvider;

            var assemblyBaseData = GetAssemblyData();
            this.mongoProvider = assemblyBaseData.CreateObject("Stimulsoft.Data.MongoDB.StiMongoDbProvider", this.ConnectionString);

            return mongoProvider;
        }

        /// <summary>
        /// Returns StiTestConnectionResult that is the information of whether the connection string specified in this class is correct.
        /// </summary>
        /// <returns>The result of testing the connection string.</returns>
        public override StiTestConnectionResult TestConnection()
        {
            try
            {
                var provider = GetMongoDBProvider();
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
                var provider = GetMongoDBProvider();
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

        /// <summary>
        /// Returns sample of the connection string to this connector.
        /// </summary>
        public override string GetSampleConnectionString()
        {
            return StiDataOptions.SampleConnectionString.MongoDb;
        }

        public override List<StiDataColumnSchema> GetColumns(string collectionName)
        {
            var dataTable = GetDataTable(collectionName, null, 0, 7);
            return dataTable != null 
                ? dataTable.Columns.Cast<DataColumn>().ToList().Select(c => new StiDataColumnSchema(c.ColumnName, c.DataType)).ToList() : null;
        }

        public override DataTable GetDataTable(string collectionName, string query, int? index = null, int? count = null)
        {
            try
            {
                var provider = GetMongoDBProvider();

                var method = provider.GetType().GetMethod("GetTableQueryData");
                var dataTable = method.Invoke(provider, new object[] { collectionName, query, index, count }) as DataTable;

                return dataTable;
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
                var provider = GetMongoDBProvider();
                var method = provider.GetType().GetMethod("ConvertDateTimeToJsonStr");
                var result = method.Invoke(provider, new object[] { date });

                return (string)result;
            }
            catch
            {
                return null;
            }
        }        
        #endregion

        #region Methods.Static
        public static StiMongoDbConnector Get(string connectionString = null)
        {
            return new StiMongoDbConnector(connectionString);
        }
        #endregion

        public StiMongoDbConnector(string connectionString = null)
            : base(connectionString)
        {
            this.FolderAssembly = $"Stimulsoft.Data.MongoDB-{this.NuGetVersion}";
            this.NameAssembly = "Stimulsoft.Data.MongoDB.dll";
        }
    }
}
