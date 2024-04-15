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
using Stimulsoft.Base.Data.Connectors.Google;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Dictionary.Adapters.Google;
using Stimulsoft.Report.Dictionary.DataSources.Google;
using Stimulsoft.Report.Dictionary.Design;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Threading.Tasks;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Dictionary.Databases.Google
{
    [TypeConverter(typeof(StiGoogleSheetsDatabaseConverter))]
    public class StiGoogleSheetsDatabase : StiDatabase
    {
        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiGoogleSheetsDatabase;
        #endregion

        #region Properties
        public override StiConnectionType ConnectionType => StiConnectionType.Google;

        [StiSerializable]
        public string ClientId { get; set; }

        [StiSerializable]
        public string ClientSecret { get; set; }

        [StiSerializable]
        public string SpreadsheetId { get; set; }

        [StiSerializable]
        [StiOrder((int)Order.FirstRowIsHeader)]
        [DefaultValue(true)]
        public bool FirstRowIsHeader { get; set; }
        #endregion

        #region Methods
        public override StiDatabase CreateNew()
        {
            return new StiGoogleSheetsDatabase();
        }

        public StiGoogleSheetsConnector CreateGoogleSheetsConnector()
        {
            return CreateConnector() as StiGoogleSheetsConnector;
        }

        public override StiDataConnector CreateConnector(string connectionString = null)
        {
            return StiGoogleSheetsConnector.Get(this.ClientId, this.ClientSecret, this.SpreadsheetId, this.FirstRowIsHeader);
        }

        /// <summary>
        /// Adds tables, views and stored procedures to report dictionary from database information.
        /// </summary>
        public override void ApplyDatabaseInformation(StiDatabaseInformation information, StiReport report)
        {
            ApplyDatabaseInformation(information, report, null);
        }

        /// <summary>
        /// Adds tables, views and stored procedures to report dictionary from database information.
        /// </summary>
        public override void ApplyDatabaseInformation(StiDatabaseInformation information, StiReport report, StiDatabaseInformation informationAll)
        {
            foreach (var table in information.Tables)
            {
                ApplyDatabaseInformationSource(information, report, informationAll, table);
            }
        }

        protected virtual void ApplyDatabaseInformationSource(StiDatabaseInformation information, StiReport report, StiDatabaseInformation informationAll, DataTable dataTable, StiSqlSourceType type = StiSqlSourceType.Table)
        {
            var source = new StiGoogleSheetsSource(this.Name, StiNameCreation.CreateName(report, dataTable.TableName, false, false, true));
            source.Alias = dataTable.TableName;

            foreach (DataColumn dataColumn in dataTable.Columns)
            {
                source.Columns.Add(new StiDataColumn(dataColumn.ColumnName, dataColumn.DataType));
            }

            report.Dictionary.DataSources.Add(source);
        }

        public override DialogResult Edit(StiDictionary dictionary, bool newDatabase)
        {
            return StiDataEditorsHelper.Get().GoogleSheetsDatabaseEdit(this, dictionary, newDatabase);
        }

        public override async Task<DialogResult> EditAsync(StiDictionary dictionary, bool newDatabase)
        {
            return await StiDataEditorsHelper.Get().GoogleSheetsDatabaseEditAsync(this, dictionary, newDatabase); ;
        }

        public StiGoogleSheetsAdapterService GetDataAdapter()
        {
            return new StiGoogleSheetsAdapterService();
        }

        /// <summary>
        /// Registers the database in dictionary.
        /// </summary>
        /// <param name="dictionary">Dictionary in which is registered database.</param>
        /// <param name="loadData">Load the data or no.</param>
        public override void RegData(StiDictionary dictionary, bool loadData)
        {
            var adapter = GetDataAdapter();
            if (adapter == null)
                throw new StiException($"A data adapter for the database {GetType()} not found");

            adapter.CreateConnectionInDataStore(dictionary, this);
        }

        /// <summary>
        /// Returns full database information.
        /// </summary>
        public override StiDatabaseInformation GetDatabaseInformation(StiReport report)
        {
            try
            {
                var information = new StiDatabaseInformation();
                var connector = CreateGoogleSheetsConnector();
                var dataSchema = connector.RetrieveSchema(AllowException);
                if (dataSchema == null)
                    return information;

                information.Tables.AddRange(GetDatabaseInformationTables(dataSchema));

                return information;
            }
            catch (Exception)
            {
                if (AllowException) throw;
                return null;
            }
        }

        protected virtual List<DataTable> GetDatabaseInformationTables(StiDataSchema dataSchema)
        {
            var tables = new List<DataTable>();
            foreach (var schemaTable in dataSchema.Tables)
            {
                try
                {
                    var table = new DataTable(schemaTable.Name);
                    schemaTable.Columns.ForEach(schemaColumn =>
                    {
                        if (table.Columns[schemaColumn.Name] == null)
                            table.Columns.Add(schemaColumn.Name, schemaColumn.Type);
                    });
                    
                    tables.Add(table);
                }
                catch
                {
                }
            }
            return tables;
        }
        #endregion

        public StiGoogleSheetsDatabase() : base()
        {
            this.FirstRowIsHeader = true;
        }

        public StiGoogleSheetsDatabase(string name, string alias, string key, string clientId, string clientSecret, string spreadsheetId, bool firstRowIsHeader)
            : base(name, alias, key)
        {
            this.ClientId = clientId;
            this.ClientSecret = clientSecret;
            this.SpreadsheetId = spreadsheetId;
            this.FirstRowIsHeader = firstRowIsHeader;
        }
    }
}
