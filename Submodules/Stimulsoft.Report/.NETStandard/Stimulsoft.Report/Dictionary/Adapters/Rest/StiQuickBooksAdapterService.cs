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
using System;
using System.Data;
using System.Linq;

namespace Stimulsoft.Report.Dictionary
{
    public class StiQuickBooksAdapterService : StiSqlAdapterService
    {
        #region Methods
        /// <summary>
        /// Returns the type of the Data Source.
        /// </summary>
        /// <returns>The type of Data Source.</returns>
        public override Type GetDataSourceType()
        {
            return typeof(StiQuickBooksSource);
        }

        /// <summary>
        /// Returns new data connector for this type of the database.
        /// </summary>
        /// <returns>Created connector.</returns>
        public override StiSqlDataConnector CreateConnector(string connectionString = null)
        {
            return StiQuickBooksConnector.Get(connectionString);
        }

        /// <summary>
        /// Returns a collection of columns of data.
        /// </summary>
        /// <param name="data">Data to find column.</param>
        /// <returns>Collection of columns found.</returns>
        public override StiDataColumnsCollection GetColumnsFromData(StiData data, StiDataSource dataSource)
        {
            return GetColumnsFromData(data, dataSource, CommandBehavior.SchemaOnly);
        }

        /// <summary>
        /// Returns a collection of columns of data.
        /// </summary>
        /// <param name="data">Data to find column.</param>
        /// <returns>Collection of columns found.</returns>
        public override StiDataColumnsCollection GetColumnsFromData(StiData data, StiDataSource dataSource, CommandBehavior retrieveMode)
        {
            var dataColumns = new StiDataColumnsCollection();
            var sqlSource = dataSource as StiSqlSource;

            try
            {
                if (!string.IsNullOrEmpty(sqlSource.SqlCommand))
                {
                    var connector = data.ViewData as StiQuickBooksConnector;
                    var table = connector.GetDataTable(null, sqlSource.SqlCommand);
                    foreach (DataColumn col in table.Columns)
                    {
                        var type = Type.GetType(col.DataType.ToString());
                        if (type == null)
                            type = typeof(object);

                        dataColumns.Add(new StiDataColumn(col.ColumnName, col.ColumnName, type));
                    }
                }
            }

            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), e);
                if (!StiOptions.Engine.HideExceptions) throw;
            }

            return dataColumns;

            /*var dataColumns = new StiDataColumnsCollection();
            var source = dataSource as StiQuickBooksSource;

            var connection = dataSource.Dictionary.Databases.Cast<StiDatabase>().FirstOrDefault(d => d.Name.ToLowerInvariant() == source.NameInSource.ToLowerInvariant()) as StiQuickBooksDatabase;
            if (connection == null) return dataColumns;

            var connector = CreateConnector(connection.ConnectionString) as StiQuickBooksConnector;
            var columns = connector.GetColumns(source.Name);
            columns.ForEach(column => dataColumns.Add(new StiDataColumn(column.Name, column.Type)));
            return dataColumns;*/
        }

        /// <summary>
        /// Returns a collection of parameters of data.
        /// </summary>
        /// <param name="data">Data to find parameters.</param>
        /// <returns>Collection of parameters found.</returns>
        public override StiDataParametersCollection GetParametersFromData(StiData data, StiDataSource dataSource)
        {
            return new StiDataParametersCollection();
        }

        public override void ConnectDataSourceToData(StiDictionary dictionary, StiDataSource dataSource, bool loadData)
        {
            if (!loadData) return;

            StiDataLeader.Disconnect(dataSource);

            var quickBooksSource = dataSource as StiQuickBooksSource;
            var connection = dictionary.Databases.ToList().FirstOrDefault(d => d.Name == quickBooksSource.NameInSource) as StiQuickBooksDatabase;
            if (connection == null) return;

            var connector = CreateConnector(connection.ConnectionString) as StiQuickBooksConnector;

            var table = new DataTable(quickBooksSource.Name);
            quickBooksSource.Columns.ToList().ForEach(c => table.Columns.Add(c.Name, c.Type));

            connector.FillDataTable(table, quickBooksSource.SqlCommand);
            connection.ConnectionString = connector.ConnectionString;
            quickBooksSource.DataTable = table;
        }

        public override void CreateConnectionInDataStore(StiDictionary dictionary, StiSqlDatabase database)
        {
            try
            {
                if (database.Name == null) return;

                //Remove all old data from datastore
                var data = dictionary.DataStore.ToList().FirstOrDefault(d => d.Name != null && d.Name.ToLowerInvariant() == database.Name.ToLowerInvariant());
                if (data != null)
                    dictionary.DataStore.Remove(data);

                var connector = CreateConnector(database.ConnectionString);
                dictionary.DataStore.Add(new StiData(database.Name, connector)
                {
                    IsReportData = true
                });
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), e);
                if (!StiOptions.Engine.HideExceptions) throw;
            }
        }
        #endregion
    }
}
