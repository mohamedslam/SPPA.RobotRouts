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

using Stimulsoft.Base.Data.Connectors.Google;
using Stimulsoft.Report.Dictionary.Databases.Google;
using Stimulsoft.Report.Dictionary.DataSources.Google;
using System;
using System.Data;
using System.Linq;

namespace Stimulsoft.Report.Dictionary.Adapters.Google
{
    public class StiGoogleSheetsAdapterService : StiDataStoreAdapterService
    {
        #region Methods
        /// <summary>
        /// Returns the type of the Data Source.
        /// </summary>
        /// <returns>The type of Data Source.</returns>
        public override Type GetDataSourceType()
        {
            return typeof(StiGoogleSheetsSource);
        }

        public override void ConnectDataSourceToData(StiDictionary dictionary, StiDataSource dataSource, bool loadData)
        {
            dataSource.Disconnect();

            if (!loadData)
            {
                dataSource.DataTable = new DataTable();
                return;
            }

            var googleSheetsTableSource = dataSource as StiGoogleSheetsSource;

            var database = dictionary.Databases.Cast<StiDatabase>().FirstOrDefault(d => d is StiGoogleSheetsDatabase && d.Name.ToLowerInvariant() == googleSheetsTableSource.NameInSource.ToLowerInvariant()) as StiGoogleSheetsDatabase;
            if (database == null) return;
            var connector = CreateConnector(database.ClientId, database.ClientSecret, database.SpreadsheetId, database.FirstRowIsHeader);
            
            dataSource.DataTable = connector.GetDataTable(googleSheetsTableSource.Name, "");
        }

        /// <summary>
        /// Returns new data connector for this type of the database.
        /// </summary>
        /// <returns>Created connector.</returns>
        public StiGoogleSheetsConnector CreateConnector(string clientId, string clientSecret, string spreadsheetId, bool firstRowIsHeader)
        {
            return StiGoogleSheetsConnector.Get(clientId, clientSecret, spreadsheetId, firstRowIsHeader);
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
            var source = dataSource as StiGoogleSheetsSource;

            var database = dataSource.Dictionary.Databases.Cast<StiDatabase>().FirstOrDefault(d => d.Name.ToLowerInvariant() == source.NameInSource.ToLowerInvariant()) as StiGoogleSheetsDatabase;
            if (database == null) return dataColumns;

            var connector = CreateConnector(database.ClientId, database.ClientSecret, database.SpreadsheetId, database.FirstRowIsHeader);
            var columns = connector.GetColumns(source.Name);
            columns.ForEach(column => dataColumns.Add(new StiDataColumn(column.Name, column.Type)));
            return dataColumns;
        }

        public void CreateConnectionInDataStore(StiDictionary dictionary, StiGoogleSheetsDatabase database)
        {
            try
            {
                if (database.Name == null) return;

                //Remove all old data from datastore
                var data = dictionary.DataStore.ToList().FirstOrDefault(d => d.Name != null && d.Name.ToLowerInvariant() == database.Name.ToLowerInvariant());
                if (data != null) dictionary.DataStore.Remove(data);

                var connector = CreateConnector(database.ClientId, database.ClientSecret, database.SpreadsheetId, database.FirstRowIsHeader);
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


        public override string GetDataCategoryName(StiData data)
        {
            return data.Name;
        }        

        public override Type[] GetDataTypes()
        {
            return new[] { typeof(StiGoogleSheetsSource) };
        }

        public override StiDataParametersCollection GetParametersFromData(StiData data, StiDataSource dataSource)
        {
            return new StiDataParametersCollection();
        }
        #endregion
    }
}
