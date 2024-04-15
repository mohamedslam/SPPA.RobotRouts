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

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Stimulsoft.Base.Data.Connectors.Google
{
    public class StiFirebaseConnector : StiDbNoSqlDataConnector
    {
        #region Fields
        private StiDataAssemblyHelper assemblyHelper;
        private object firebaseProvider;
        #endregion

        #region Properties
        public string AuthSecret { get; set; }

        public string BasePath { get; set; }

        /// <summary>
        /// Gets a type of the connection helper.
        /// </summary>
        public override StiConnectionIdent ConnectionIdent => StiConnectionIdent.FirebirdDataSource;

        /// <summary>
        /// Gets an order of the connector.
        /// </summary>
        public override StiConnectionOrder ConnectionOrder => StiConnectionOrder.FirebirdDataSource;
        public override string Name => "Firebase";
        #endregion

        #region Methods
        /// <summary>
        /// Get a value which indicates that this data connector can be used now.
        /// </summary>
        public override bool IsAvailable
        {
            get
            {
                var assemblyHelper = GetAssemblyData();
                return assemblyHelper != null && assemblyHelper.IsAllowed;
            }
        }

        /// <summary>
        /// Gets the package identificator for this connector.
        /// </summary>
        public override string[] NuGetPackages => new string[] { "Stimulsoft.Data.Firebase" };

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

            assemblyHelper = new StiDataAssemblyHelper("Stimulsoft.Data.Firebase.dll", $"Stimulsoft.Data.Firebase-{this.NuGetVersion}");
            return assemblyHelper;
        }

        private object GetFirebaseProvider()
        {
            if (this.firebaseProvider != null)
                return this.firebaseProvider;

            var assemblyBaseData = GetAssemblyData();
            this.firebaseProvider = assemblyBaseData.CreateObject("Stimulsoft.Data.Firebase.StiFirebaseProvider",
                new object[] { this.ConnectionString});

            return firebaseProvider;
        }

        /// <summary>
        /// Returns sample of the connection string to this connector.
        /// </summary>
        public override string GetSampleConnectionString()
        {
            return StiDataOptions.SampleConnectionString.Firebase;
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
                var provider = GetFirebaseProvider();

                var method = provider.GetType().GetMethod("GetJsonData");
                var content = method.Invoke(provider, new object[] { collectionName, index, count }) as string;

                var dataSet = StiBaseOptions.DefaultJsonConverterVersion == StiJsonConverterVersion.ConverterV2
                    ? StiJsonToDataSetConverterV2.GetDataSet(content)
                    : StiJsonToDataSetConverter.GetDataSet(content);

                if (dataSet != null && dataSet.Tables.Contains(collectionName))
                {
                    return dataSet.Tables[collectionName].Copy();
                }

                return null;
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

        #region Methods.Static
        public static StiFirebaseConnector Get(string connectionString = null)
        {
            return new StiFirebaseConnector(connectionString);
        }

        public override StiTestConnectionResult TestConnection()
        {
            try
            {
                var provider = GetFirebaseProvider();

                var method = provider.GetType().GetMethod("TestConnection");
                var testConnection = (bool)method.Invoke(provider, null);

                return testConnection ? StiTestConnectionResult.MakeFine() : StiTestConnectionResult.MakeWrong("Couldn't open connection to server");
            }
            catch (Exception exception)
            {
                if (exception.InnerException != null)
                    return StiTestConnectionResult.MakeWrong(exception.InnerException.Message);
                else
                    return StiTestConnectionResult.MakeWrong(exception.Message);
            }
        }

        public  DataSet GetDataSet()
        {
            var provider = GetFirebaseProvider();

            var method = provider.GetType().GetMethod("GetJsonData");
            var content = method.Invoke(provider, new object[] { "", 0, 0}) as string;

            return StiBaseOptions.DefaultJsonConverterVersion == StiJsonConverterVersion.ConverterV2
                ? StiJsonToDataSetConverterV2.GetDataSet(content)
                : StiJsonToDataSetConverter.GetDataSet(content);
        }

        public override StiDataSchema RetrieveSchema(bool allowException = false)
        {
            var dataSet = GetDataSet();
            if (dataSet == null)
                return null;

            var schema = new StiDataSchema(this.ConnectionIdent);

            #region Tables
            foreach (DataTable table in dataSet.Tables)
            {
                var tableSchema = StiDataTableSchema.NewTable(table.TableName);

                foreach (DataColumn column in table.Columns)
                {
                    tableSchema.Columns.Add(new StiDataColumnSchema
                    {
                        Name = column.ColumnName,
                        Type = column.DataType
                    });
                }
                schema.Tables.Add(tableSchema);
            }
            #endregion

            #region Relations
            foreach (DataRelation relation in dataSet.Relations)
            {
                schema.Relations.Add(new StiDataRelationSchema
                {
                    Name = relation.RelationName,
                    ParentSourceName = relation.ParentTable.TableName,
                    ChildSourceName = relation.ChildTable.TableName,
                    ParentColumns = relation.ParentColumns.Select(col => col.ColumnName).ToList(),
                    ChildColumns = relation.ChildColumns.Select(col => col.ColumnName).ToList()
                });
            }
            #endregion

            schema.Relations = schema.Relations.OrderBy(e => e.Name).ToList();
            schema.Tables = schema.Tables.OrderBy(e => e.Name).ToList();

            return schema;
        }
        #endregion

        public StiFirebaseConnector(string connectionString = null)
            : base(connectionString)
        {
        }
    }
}
