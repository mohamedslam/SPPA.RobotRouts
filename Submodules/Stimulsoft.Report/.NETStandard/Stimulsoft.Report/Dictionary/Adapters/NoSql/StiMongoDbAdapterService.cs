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
    /// <summary>
    /// Describes the adapter for access to MongoDbConnection.
    /// </summary>
    public class StiMongoDbAdapterService : StiNoSqlAdapterService
    {
		#region Methods
		/// <summary>
		/// Returns the type of the Data Source.
		/// </summary>
		/// <returns>The type of Data Source.</returns>
		public override Type GetDataSourceType()
		{
			return typeof(StiMongoDbSource);
		}
 
	    /// <summary>
	    /// Returns new data connector for this type of the database.
	    /// </summary>
        /// <returns>Created connector.</returns>
	    public override StiNoSqlDataConnector CreateConnector(string connectionString = null)
	    {
	        return StiMongoDbConnector.Get(connectionString);
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
            var source = dataSource as StiMongoDbSource;

            var connection = dataSource.Dictionary.Databases.Cast<StiDatabase>().FirstOrDefault(d => d.Name.ToLowerInvariant() == source.NameInSource.ToLowerInvariant()) as StiMongoDbDatabase;
            if (connection == null) return dataColumns;

            var connector = CreateConnector(connection.ConnectionString);
            var columns = connector.GetColumns(source.Name);
            columns.ForEach(column => dataColumns.Add(new StiDataColumn(column.Name, column.Type)));
            return dataColumns;
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
            StiDataLeader.Disconnect(dataSource);

            if (!loadData)
            {
                dataSource.DataTable = new DataTable();
                return;
            }

            var mongoSource = dataSource as StiMongoDbSource;
            
            var database = dictionary.Databases.Cast<StiDatabase>().FirstOrDefault(d => d is StiMongoDbDatabase && d.Name.ToLowerInvariant() == mongoSource.NameInSource.ToLowerInvariant()) as StiMongoDbDatabase;
            if (database == null) return;
            var connector = CreateConnector(database.ConnectionString);

            var query = mongoSource.Query;
            var report = dictionary.Report;

            foreach (StiVariable variable in dictionary.Variables)
            {
                var value = report.ToString(report[variable.Name]);
                if (variable.Type == typeof(DateTime) && report[variable.Name] is DateTime)
                    value = ((StiMongoDbConnector) connector).ConvertDateTimeToJsonStr((DateTime)report[variable.Name]);
                    
                var str = $"Param({variable.Name})";
                if (query.Contains(str))
                    query = query.Replace(str, value);
            }

            dataSource.DataTable = connector.GetDataTable(mongoSource.Name, query);
        }
	    #endregion
	}
}
