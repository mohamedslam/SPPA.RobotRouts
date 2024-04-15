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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.ComponentModel;
using Stimulsoft.Base;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Report.PropertyGrid;
using System.Threading.Tasks;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Dictionary
{
    public abstract class StiNoSqlDatabase : StiDatabase
    {
        #region IStiPropertyGridObject
        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            // DataCategory
            var list = new[]
            {
                propHelper.Name(),
                propHelper.Alias(),

                propHelper.ConnectionString()
            };
            objHelper.Add(StiPropertyCategories.Data, list);

            return objHelper;
        }
        #endregion

        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiSqlDatabase
            jObject.AddPropertyStringNullOrEmpty("ConnectionStringEncrypted", ConnectionStringEncrypted);
            jObject.AddPropertyBool("PromptUserNameAndPassword", PromptUserNameAndPassword);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "ConnectionStringEncrypted":
                        this.ConnectionStringEncrypted = property.DeserializeString();
                        break;

                    case "PromptUserNameAndPassword":
                        this.PromptUserNameAndPassword = property.DeserializeBool();
                        break;
                }
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets a connection type.
        /// </summary>
        public override StiConnectionType ConnectionType => StiConnectionType.NoSql;

        /// <summary>
        /// Gets or sets a connection string which contains SQL connection parameters.
        /// </summary>
        //[StiSerializable]
        [StiOrder((int)Order.ConnectionString)]
        [Description("Gets or sets a connection string which contains SQL connection parameters.")]
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets a connection string which contains SQL connection parameters, in encrypted form.
        /// </summary>
        [StiSerializable]
        [Browsable(false)]
        public string ConnectionStringEncrypted
        {
            get
            {
                return StiEncryption.Encrypt(ConnectionString, "8pTP5X15uKADcSw7");
            }
            set
            {
                ConnectionString = StiEncryption.Decrypt(value, "8pTP5X15uKADcSw7");
            }
        }

        /// <summary>
        /// Gets or sets a value which indicates that UserName and Password parameters should be requested from user.
        /// </summary>
        [StiSerializable]
        [Browsable(false)]
        [DefaultValue(false)]
        [StiOrder((int)Order.PromptUserNameAndPassword)]
        [Description("Gets or sets a value which indicates that UserName and Password parameters should be requested from user.")]
        public bool PromptUserNameAndPassword { get; set; }

        [Browsable(false)]
        public virtual bool CanEditConnectionString => StiDatabaseAssembly.CanEditConnectionString(GetConnectionStringHelper());

        protected virtual string DataAdapterType => GetDataAdapterType().ToString();
        #endregion

        #region Methods
        /// <summary>
        /// Returns new sql data connector for this database.
        /// </summary>
        /// <returns>Created sql connector.</returns>
        public virtual StiNoSqlDataConnector CreateNoSqlConnector(string connectionString = null)
        {
            return CreateConnector(connectionString) as StiNoSqlDataConnector;
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

        protected abstract Type GetDataAdapterType();

        public virtual StiNoSqlAdapterService GetDataAdapter()
        {
            var adapterType = GetDataAdapterType();
            if (adapterType != null)
                return StiActivator.CreateObject(adapterType) as StiNoSqlAdapterService;

            var adapter = StiOptions.Services.DataAdapters.Where(s => s.ServiceEnabled).FirstOrDefault(d => d.GetType().ToString() == DataAdapterType);
            if (adapter != null) 
                return adapter as StiNoSqlAdapterService;

            throw new Exception($"Data adapter for the {this} database is not found!");
        }
        #endregion

        #region Methods.DatabaseInformation
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
            var source = CreateDataSource(this.Name, StiNameCreation.CreateName(report, dataTable.TableName, false, false, true));
            source.Alias = dataTable.TableName;

            foreach (DataColumn dataColumn in dataTable.Columns)
            {
                source.Columns.Add(new StiDataColumn(dataColumn.ColumnName, dataColumn.DataType));
            }

            report.Dictionary.DataSources.Add(source);
        }


        /// <summary>
        /// Adds tables, views and stored procedures to report dictionary from database information.
        /// </summary>
        public override void ApplyDatabaseInformation(StiDatabaseInformation information, StiReport report)
        {
            ApplyDatabaseInformation(information, report, null);
        }

        /// <summary>
        /// Returns full database information.
        /// </summary>
        public override StiDatabaseInformation GetDatabaseInformation(StiReport report)
        {
            try
            {
                var information = new StiDatabaseInformation();
                var connector = CreateNoSqlConnector(this.ConnectionString);
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

                    if (!string.IsNullOrWhiteSpace(schemaTable.Query))
                        table.ExtendedProperties["Query"] = schemaTable.Query;

                    tables.Add(table);
                }
                catch
                {
                }
            }
            return tables;
        }

        public abstract StiNoSqlSource CreateDataSource(string nameInSource, string name);
        #endregion

        #region Methods.Editors
        public virtual string GetConnectionStringHelper()
        {
            return null;
        }

        public override DialogResult Edit(StiDictionary dictionary, bool newDatabase)
        {
            return StiDataEditorsHelper.Get().NoSqlDatabaseEdit(this, dictionary, newDatabase);
        }

        public override async Task<DialogResult> EditAsync(StiDictionary dictionary, bool newDatabase)
        {
            return await StiDataEditorsHelper.Get().NoSqlDatabaseEditAsync(this, dictionary, newDatabase);
        }

        public virtual string EditConnectionString(string connectionString)
        {
            return StiDatabaseAssembly.EditConnectionString(this, connectionString, GetConnectionStringHelper());
        }

        public virtual string MapUserNameAndPassword(string userName, string password)
        {
            return $"User Id = {userName}; Password = {password}";
        }
        #endregion


        public StiNoSqlDatabase()
            : this(string.Empty, string.Empty)
        {
        }

        public StiNoSqlDatabase(string name, string connectionString)
            : base(name)
        {
            this.ConnectionString = connectionString;
        }

        public StiNoSqlDatabase(string name, string alias, string connectionString)
            : base(name, alias)
        {
            this.ConnectionString = connectionString;
        }

        public StiNoSqlDatabase(string name, string alias, string connectionString, bool promptUserNameAndpassword)
            : base(name, alias)
        {
            this.ConnectionString = connectionString;
            this.PromptUserNameAndPassword = promptUserNameAndpassword;
        }

        public StiNoSqlDatabase(string name, string alias, string connectionString, bool promptUserNameAndpassword, string key)
            : base(name, alias, key)
        {
            this.ConnectionString = connectionString;
            this.PromptUserNameAndPassword = promptUserNameAndpassword;
        }

    }
}
