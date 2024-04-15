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
using System.Text;
using System.ComponentModel;
using Stimulsoft.Base;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Report.Dictionary.Design;
using Stimulsoft.Report.PropertyGrid;
using System.Threading.Tasks;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Dictionary
{
    [TypeConverter(typeof(StiSqlDatabaseConverter))]
    public class StiSqlDatabase : StiDatabase
    {
        #region Consts
        private const string EncryptedId = "8pTP5X15uKADcSw7";
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiSqlDatabase;

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
        protected virtual string DataAdapterType => GetDataAdapterType().ToString();

        /// <summary>
        /// Gets a connection type.
        /// </summary>
        public override StiConnectionType ConnectionType => StiConnectionType.Sql;

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
                return StiEncryption.Encrypt(ConnectionString, EncryptedId);
            }
            set
            {
                try
                {
                    ConnectionString = StiEncryption.Decrypt(value, EncryptedId);
                }
                catch
                {
                    ConnectionString = value;
                }
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
        #endregion

        #region Methods
        public override StiDatabase CreateNew()
        {
            return new StiSqlDatabase();
        }

        /// <summary>
        /// Returns new data connector for this database.
        /// </summary>
        /// <returns>Created connector.</returns>
        public override StiDataConnector CreateConnector(string connectionString = null)
        {
            return StiMsSqlConnector.Get(connectionString);
        }

        /// <summary>
        /// Returns new sql data connector for this database.
        /// </summary>
        /// <returns>Created sql connector.</returns>
        public StiSqlDataConnector CreateSqlConnector(string connectionString = null)
        {
            return CreateConnector(connectionString) as StiSqlDataConnector;
        }

        /// <summary>
        /// Returns new data source for this database.
        /// </summary>
        /// <returns>Created data source.</returns>
        public virtual StiSqlSource CreateDataSource(string nameInSource, string name)
        {
            return new StiSqlSource(nameInSource, name);
        }

        public virtual StiSqlAdapterService GetDataAdapter()
        {
            var adapterType = GetDataAdapterType();
            if (adapterType != null)
                return StiActivator.CreateObject(adapterType) as StiSqlAdapterService;

            var adapter = StiOptions.Services.DataAdapters.Where(s => s.ServiceEnabled).FirstOrDefault(d => d.GetType().ToString() == DataAdapterType);
            if (adapter != null)
                return adapter as StiSqlAdapterService;
            
            throw new Exception($"Data adapter for the {GetType()} database is not found!");
        }
       
        protected virtual Type GetDataAdapterType()
        {
            return typeof (StiSqlAdapterService);
        }

        public override void RegData(StiDictionary dictionary, bool loadData)
        {
            var adapter = GetDataAdapter();
            if (adapter == null)
                throw new Exception($"Database {GetType()} not found");

            adapter.CreateConnectionInDataStore(dictionary, this);
        }
        #endregion

        #region Methods.Editors
        public virtual string GetConnectionStringHelper()
        {
            return "StiSqlConnectionHelper";
        }

        public override DialogResult Edit(StiDictionary dictionary, bool newDatabase)
        {
            return StiDataEditorsHelper.Get().SqlDatabaseEdit(this, dictionary, newDatabase);
        }

        public override async Task<DialogResult> EditAsync(StiDictionary dictionary, bool newDatabase)
        {
            return await StiDataEditorsHelper.Get().SqlDatabaseEditAsync(this, dictionary, newDatabase);
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

        #region Methods.DatabaseInformation
        /// <summary>
        /// Adds tables, views and stored procedures to report dictionary from database information.
        /// </summary>
        public override void ApplyDatabaseInformation(StiDatabaseInformation information, StiReport report, StiDatabaseInformation informationAll)
        {
            ApplyDatabaseInformationTables(information, report, informationAll);
            ApplyDatabaseInformationViews(information, report, informationAll);
            ApplyDatabaseInformationProcedures(information, report, informationAll);
        }

        protected virtual void ApplyDatabaseInformationTables(StiDatabaseInformation information, StiReport report, StiDatabaseInformation informationAll)
        {
            foreach (var table in information.Tables)
            {
                ApplyDatabaseInformationSource(information, report, informationAll, table);
            }
        }

        protected virtual void ApplyDatabaseInformationViews(StiDatabaseInformation information, StiReport report, StiDatabaseInformation informationAll)
        {
            foreach (var view in information.Views)
            {
                ApplyDatabaseInformationSource(information, report, informationAll, view);
            }
        }
        
        protected virtual void ApplyDatabaseInformationProcedures(StiDatabaseInformation information, StiReport report, StiDatabaseInformation informationAll)
        {
            foreach (var proc in information.StoredProcedures)
            {
                ApplyDatabaseInformationSource(information, report, informationAll, proc, StiSqlSourceType.StoredProcedure);
            }
        }

        protected virtual void ApplyDatabaseInformationSource(StiDatabaseInformation information, StiReport report, StiDatabaseInformation informationAll, DataTable dataTable, StiSqlSourceType type = StiSqlSourceType.Table)
        {
            var connector = CreateSqlConnector(this.ConnectionString);
            var source = CreateDataSource(Name, StiNameCreation.CreateName(report, dataTable.TableName, false, false, true));

            source.AllowExpressions = !(this is StiOdbcDatabase && type == StiSqlSourceType.StoredProcedure);

            source.SqlCommand = dataTable.ExtendedProperties["Query"] != null 
                ? source.SqlCommand = dataTable.ExtendedProperties["Query"] as string 
                : $"select * from {dataTable.TableName}";
            
            #region Create Columns & Parameters
            var columns = new StringBuilder();

            foreach (DataColumn dataColumn in dataTable.Columns)
            {
                if (columns.Length == 0)
                    columns = columns.Append(dataColumn.ColumnName);
                else
                    columns = columns.AppendFormat(", {0}", dataColumn.ColumnName);
                
                if (dataColumn.Caption == "Parameters")
                    source.Parameters.Add(new StiDataParameter { Name = dataColumn.ColumnName, Type = connector.GetSqlType(dataColumn.DataType) });
                else
                    source.Columns.Add(new StiDataColumn(dataColumn.ColumnName, dataColumn.DataType));
            }

            source.Type = type;

            var tableAll = informationAll?.Tables.FirstOrDefault(t => t.TableName == dataTable.TableName);
            if (tableAll != null && dataTable.Columns.Count != tableAll.Columns.Count && source.SqlCommand.Contains("*"))
            {
                if (columns.Length > 0)
                    source.SqlCommand = source.SqlCommand.Replace("*", columns.ToString());
            }
            #endregion

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
                var connector = CreateSqlConnector(this.ConnectionString);

                var dataSchema = StiDataLeader.RetrieveSchema(connector, AllowException);
                if (dataSchema == null)
                    return information;

                information.Tables.AddRange(GetDatabaseInformationTables(dataSchema));
                information.Views.AddRange(GetDatabaseInformationViews(dataSchema));
                information.StoredProcedures.AddRange(GetDatabaseInformationProcedures(dataSchema));

                return information;
            }
            catch (Exception)
            {
                if (AllowException)throw;

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

        protected virtual List<DataTable> GetDatabaseInformationViews(StiDataSchema dataSchema)
        {
            var views = new List<DataTable>();
            foreach (var schemaView in dataSchema.Views)
            {
                try
                {
                    var view = new DataTable(schemaView.Name);
                    schemaView.Columns.ForEach(schemaColumn =>
                    {
                        if (view.Columns[schemaColumn.Name] == null)
                            view.Columns.Add(schemaColumn.Name, schemaColumn.Type);
                    });

                    if (!string.IsNullOrWhiteSpace(schemaView.Query))
                        view.ExtendedProperties["Query"] = schemaView.Query;

                    views.Add(view);
                }
                catch
                {
                    
                }
            }
            return views;
        }

        protected virtual List<DataTable> GetDatabaseInformationProcedures(StiDataSchema dataSchema)
        {
            var procedures = new List<DataTable>();
            foreach (var schemaProc in dataSchema.StoredProcedures)
            {
                try
                {
                    var proc = new DataTable(schemaProc.Name);
                    schemaProc.Columns.ForEach(schemaColumn =>
                    {
                        if (proc.Columns[schemaColumn.Name] == null)
                        {
                            proc.Columns.Add(new DataColumn(schemaColumn.Name, schemaColumn.Type)
                            {
                                Caption = "Columns"
                            });
                        }
                    });

                    schemaProc.Parameters.ForEach(schemaParameter =>
                    {
                        if (proc.Columns[schemaParameter.Name] == null)
                        {
                            proc.Columns.Add(new DataColumn(schemaParameter.Name, schemaParameter.Type)
                            {
                                Caption = "Parameters"
                            });
                        }
                    });

                    if (!string.IsNullOrWhiteSpace(schemaProc.Query))
                        proc.ExtendedProperties["Query"] = schemaProc.Query;

                    procedures.Add(proc);
                }
                catch
                {
                }
            }
            return procedures;
        }
        #endregion

        /// <summary>
        /// Creates a new object of the type StiSqlDatabase.
        /// </summary>
        public StiSqlDatabase()
            : this(string.Empty, string.Empty)
        {
        }

        /// <summary>
        /// Creates a new object of the type StiSqlDatabase.
        /// </summary>
        public StiSqlDatabase(string name, string connectionString)
            : base(name)
        {
            this.ConnectionString = connectionString;
        }

        /// <summary>
        /// Creates a new object of the type StiSqlDatabase.
        /// </summary>
        public StiSqlDatabase(string name, string alias, string connectionString)
            : base(name, alias)
        {
            this.ConnectionString = connectionString;
        }

        /// <summary>
        /// Creates a new object of the type StiSqlDatabase.
        /// </summary>
        public StiSqlDatabase(string name, string alias, string connectionString, bool promptUserNameAndpassword)
            : base(name, alias)
        {
            this.ConnectionString = connectionString;
            this.PromptUserNameAndPassword = promptUserNameAndpassword;
        }

        /// <summary>
        /// Creates a new object of the type StiSqlDatabase.
        /// </summary>
        public StiSqlDatabase(string name, string alias, string connectionString, bool promptUserNameAndpassword, string key)
            : base(name, alias, key)
        {
            this.ConnectionString = connectionString;
            this.PromptUserNameAndPassword = promptUserNameAndpassword;
        }
    }
}
