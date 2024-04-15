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
    public class StiBigQueryConnector : StiDbNoSqlDataConnector
    {
        #region Fields
        private StiDataAssemblyHelper assemblyHelper;
        private object bigQueryProvider;
        #endregion

        #region Properties
        /// <summary>
        /// Gets a type of the connection helper.
        /// </summary>
        public override StiConnectionIdent ConnectionIdent => StiConnectionIdent.BigQueryDataSource;

        /// <summary>
        /// Gets an order of the connector.
        /// </summary>
        public override StiConnectionOrder ConnectionOrder => StiConnectionOrder.BigQueryDataSource;
        
        public override string Name => "BigQuery";
        
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
        public override string[] NuGetPackages => new string[] { "Stimulsoft.Data.BigQuery" };

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

            assemblyHelper = new StiDataAssemblyHelper("Stimulsoft.Data.BigQuery.dll", $"Stimulsoft.Data.BigQuery-{this.NuGetVersion}");
            return assemblyHelper;
        }

        private object GetBigQueryProvider()
        {
            if (this.bigQueryProvider != null)
                return this.bigQueryProvider;

            var assemblyBaseData = GetAssemblyData();
            this.bigQueryProvider = assemblyBaseData.CreateObject("Stimulsoft.Data.BigQuery.StiBigQueryProvider", new object[] { this.ConnectionString });

            return bigQueryProvider;
        }

        public override StiTestConnectionResult TestConnection()
        {
            try
            {
                var provider = GetBigQueryProvider();

                var method = provider.GetType().GetMethod("IsConnectionAvailable");
                var isConnectionAvailable = (bool)method.Invoke(provider, null);

                return isConnectionAvailable
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

        public override StiDataSchema RetrieveSchema(bool allowException = false)
        {
            DataSet emptyDatasetOfSameSchema;
            try
            {
                var provider = GetBigQueryProvider();

                var method = provider.GetType().GetMethod("GetEmptyDatasetOfSameSchema");
                emptyDatasetOfSameSchema = method.Invoke(provider, null) as DataSet;
            }
            catch (Exception exception)
            {
                if (exception.InnerException != null)
                    throw exception.InnerException;
                else
                    throw;
            }

            var schema = new StiDataSchema(this.ConnectionIdent);

            #region Tables
            foreach (DataTable table in emptyDatasetOfSameSchema.Tables)
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
            foreach (DataRelation relation in emptyDatasetOfSameSchema.Relations)
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

        public override string GetSampleConnectionString()
        {
            return StiDataOptions.SampleConnectionString.BigQuery;
        }

        public override List<StiDataColumnSchema> GetColumns(string collectionName)
        {
            var dataTable = RetrieveSchema().Tables.FirstOrDefault(table => table.Name.ToLowerInvariant() == collectionName.ToLowerInvariant());

            return dataTable?.Columns;
        }

        public override DataTable GetDataTable(string collectionName, string query, int? index = null, int? count = null)
        {
            DataTable table;

            try
            {
                var provider = GetBigQueryProvider();

                var method = provider.GetType().GetMethod("QueryOneTable");
                table = method.Invoke(provider, new object[] { collectionName, query, index, count }) as DataTable;

                return table;
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
        public static StiBigQueryConnector Get(string connectionString = null)
        {
            return new StiBigQueryConnector(connectionString);
        }        
        #endregion

        public StiBigQueryConnector(string connectionString = null)
            : base(connectionString)
        {
        }
    }
}
