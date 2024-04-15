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
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dictionary.Design;
using Stimulsoft.Report.PropertyGrid;
using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Dictionary
{
    /// <summary>
    /// Describes the Data Source realizing access to Sql.
    /// </summary>
    [TypeConverter(typeof(StiSqlSourceConverter))]
	public class StiSqlSource : StiDataTableSource
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiSqlSource
            jObject.AddPropertyBool("ConnectOnStart", ConnectOnStart, true);
            jObject.AddPropertyBool("AllowExpressions", AllowExpressions, true);
            jObject.AddPropertyEnum("Type", Type, StiSqlSourceType.Table);
            jObject.AddPropertyInt("CommandTimeout", CommandTimeout, 30);
            jObject.AddPropertyBool("ReconnectOnEachRow", ReconnectOnEachRow);
            jObject.AddPropertyStringNullOrEmpty("SqlCommand", SqlCommand);
            jObject.AddPropertyJObject("Parameters", Parameters.SaveToJsonObject(mode));

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "ConnectOnStart":
                        this.ConnectOnStart = property.DeserializeBool();
                        break;

                    case "AllowExpressions":
                        this.AllowExpressions = property.DeserializeBool();
                        break;

                    case "Type":
                        this.Type = property.DeserializeEnum<StiSqlSourceType>();
                        break;

                    case "CommandTimeout":
                        this.CommandTimeout = property.DeserializeInt();
                        break;

                    case "ReconnectOnEachRow":
                        this.ReconnectOnEachRow = property.DeserializeBool();
                        break;

                    case "SqlCommand":
                        this.SqlCommand = property.DeserializeString();
                        break;

                    case "Parameters":
                        this.Parameters.LoadFromJsonObject((JObject)property.Value);
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiSqlSource;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            // DataCategory
            var list = new[]
            {
                propHelper.Name(),
                propHelper.Alias(),
                propHelper.NameInSource(),
                propHelper.ConnectOnStart(),
                propHelper.AllowExpressions(),
                propHelper.DataSourceType(),
                propHelper.CommandTimeout(),
                propHelper.ReconnectOnEachRow(),
                propHelper.SqlCommand()
            };
            objHelper.Add(StiPropertyCategories.Data, list);

            return objHelper;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets value which indicates that datasource not connect to the data automatically.
        /// </summary>
        [StiSerializable]
        [Browsable(true)]
        [StiCategory("Data")]
        [DefaultValue(true)]
        [Description("Gets or sets value which indicates that datasource not connect to the data automatically.")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiOrder((int)Order.ConnectOnStart)]
        public override bool ConnectOnStart
        {
            get
            {
                return base.ConnectOnStart;
            }
            set
            {
                base.ConnectOnStart = value;
            }
        }

        /// <summary>
        /// Gets or sets value which indicates that sql query of the datasource can contain script expressions or no.
        /// </summary>
        [StiSerializable]
        [Browsable(true)]
        [StiCategory("Data")]
        [DefaultValue(true)]
        [Description("Gets or sets value which indicates that sql query of the datasource can contain script expressions or no.")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiOrder((int)Order.AllowExpressions)]
        public bool AllowExpressions { get; set; } = true;

        /// <summary>
        /// Gets or sets value which indicates type of sql datasource.
        /// </summary>
        [StiSerializable]
        [StiCategory("Data")]
        [DefaultValue(StiSqlSourceType.Table)]
        [Description("Gets or sets value which indicates type of sql datasource.")]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiOrder((int)Order.Type)]
        public StiSqlSourceType Type { get; set; } = StiSqlSourceType.Table;
        
        [Browsable(false)]
        public IDbDataAdapter DataAdapter { get; set; }

        /// <summary>
        /// Gets or sets a number of seconds to wait while attempting to execute a command, before canceling the attempt and generate an error. Default is 30.
        /// </summary>
        [StiSerializable]
        [StiCategory("Data")]
        [DefaultValue(0)]
        [Description("Gets or sets a number of seconds to wait while attempting to execute a command, before canceling the attempt and generate an error. Default is 30.")]
        [StiOrder((int)Order.CommandTimeout)]
        public int CommandTimeout { get; set; }

        /// <summary>
        /// Gets or sets value which indicates that datasource reconnect on each master row in master-detail reports.
        /// </summary>
        [StiSerializable]
        [StiCategory("Data")]
        [DefaultValue(false)]
        [Description("Gets or sets value which indicates that datasource reconnect on each master row in master-detail reports.")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiOrder((int)Order.ReconnectOnEachRow)]
        public bool ReconnectOnEachRow { get; set; }

        /// <summary>
        /// Gets or sets a SQL statement to execute at the Data Source.
        /// </summary>
        [StiSerializable]
        [StiCategory("Data")]
        [Description("Gets or sets a SQL statement to execute at the Data Source.")]
        [StiOrder((int)Order.SqlCommand)]
        public virtual string SqlCommand 
        { 
            get; 
            set;
        } = string.Empty;

        /// <summary>
        /// Gets or sets the parameter collection of the SQL query.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.List)]
        [StiCategory("Data")]
        [Description("Gets or sets the parameter collection of the SQL query.")]
        [Browsable(true)]
        [StiOrder((int)Order.Parameters)]
        public override StiDataParametersCollection Parameters
        {
            get
            {
                return base.Parameters;
            }
            set
            {
                base.Parameters = value;
            }
        }

        private bool ShouldSerializeParameters()
        {
            return Parameters == null || Parameters.Count > 0;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Returns new data connector for this datasource.
        /// </summary>
        /// <returns>Created connector.</returns>
        public virtual StiSqlDataConnector CreateConnector(string connectionString = null)
        {
            return StiMsSqlConnector.Get(connectionString);
        }

        protected override Type GetDataAdapterType()
        {
            return typeof(StiSqlAdapterService);
        }

		public virtual Type GetParameterTypesEnum()
		{
            return CreateConnector().SqlType;
		}

		/// <summary>
		/// Internal use only.
		/// </summary>
		public Type ConvertDbTypeToType(int sqlType)
		{
			return ConvertDbTypeToTypeInternal(sqlType);
		}

		protected virtual Type ConvertDbTypeToTypeInternal(int sqlType)
		{
		    return CreateConnector().GetNetType(sqlType);
		}

		public virtual StiDataParameter AddParameter()
		{
            var parameter = new StiDataParameter(Loc.Get("PropertyMain", "Parameter"), string.Empty, CreateConnector().DefaultSqlType, 0);
			Parameters.Add(parameter);
			return parameter;
		}

		public virtual void UpdateParameters()
		{
		    if (this.DataTable == null || this.DataAdapter == null || this.Parameters.Count == 0) return;

		    InvokeConnecting();

		    RetrieveData();
		}

        public void RetrieveData(bool schemaOnly = false)
	    {
            var connector = CreateConnector();
            var adapter = DataAdapter as DbDataAdapter;

            #region Clear DataTable
	        if (this.DataTable == null)
	            this.DataTable = new DataTable(this.Name);

	        else
	        {
	            this.DataTable.Rows.Clear();

	            if (Dictionary.CacheDataSet != null)
	            {
	                if (this.DataTable.DataSet == null)
	                    this.DataTable = DataTable; //fix, move to current CacheDataSet

	                else
	                {
	                    if (this.DataTable.DataSet != Dictionary.CacheDataSet)
	                    {
	                        this.DataTable.DataSet.Tables.Remove(this.DataTable);
	                        this.DataTable = DataTable; //fix, move to current CacheDataSet
	                    }
	                }
	            }
	        }
	        #endregion

            #region Set Parameters
            if (this.Parameters.Count > 0)
            {
                if (StiOptions.Engine.AllowPrepareSqlQueries)
                    adapter.SelectCommand.Prepare();
                
                this.InvokeConnecting();     
                
                foreach (StiDataParameter parameter in Parameters)
                {
                    var obj = parameter.GetParameterValue();
                    if (obj is DateTime && ((DateTime) obj).Ticks == 0)
                        obj = DBNull.Value;

                    adapter.SelectCommand.Parameters[parameter.Name].Value = obj ?? DBNull.Value;

                    // Oracle RefCursor for stored procedures
                    if (connector.ConnectionIdent == StiConnectionIdent.OracleDataSource && (StiDbType.Oracle)parameter.Type == StiDbType.Oracle.RefCursor)
                        adapter.SelectCommand.Parameters[parameter.Name].Direction = ParameterDirection.Output;
                }
            }
            #endregion

            connector.SetTimeout(adapter.SelectCommand, this.CommandTimeout);

            StiDataAdapterHelper.Fill(Dictionary, adapter, this.DataTable, schemaOnly);

            this.CheckColumnsIndexs();
	    }

		public override void Disconnect()
		{
			base.Disconnect();

		    try
		    {
		        var disposable = DataAdapter as IDisposable;
		        disposable?.Dispose();
		    }
		    catch
		    {
		    }

		    DataAdapter = null;
		}

        public string GetSqlFilterQuery()
        {
            if (Dictionary?.Report == null) 
                return null;

            var comps = Dictionary.Report.GetComponents();
            foreach (StiComponent comp in comps)
            {
                var dataBand = comp as StiDataBand;
                if (dataBand == null) continue;

                if (dataBand.FilterEngine == StiFilterEngine.SQLQuery && 
                    dataBand.Filters.Count > 0 && dataBand.DataSourceName == this.Name && dataBand.DataSource == this)
                {
                    return StiDataBandSQLFilterHelper.GetFilter(dataBand, false);
                }
            }
            return null;
        }

		public string GetFinalSqlCommand()
		{
			var methodName = $"Get{Name}_SqlCommand";
			var type = this.Dictionary.Report.GetType();

			var method = type.GetMethod(methodName, new[]{ typeof(object), typeof(EventArgs) });
            if (method == null) 
                return null;

			method.Invoke(this.Dictionary.Report, new object[]{this, EventArgs.Empty});

            var filter = GetSqlFilterQuery();
		    if (filter == null)
		        return SqlCommand;

		    if (SqlCommand != null && SqlCommand.ToLowerInvariant().Contains("where"))
		        return $"{SqlCommand} AND {filter}";
		    else
		        return $"{SqlCommand} WHERE {filter}";
		}

        public override StiDataSource CreateNew()
        {
            return new StiSqlSource();
        }
        #endregion

		/// <summary>
		/// Creates a new object of the type StiSqlSource.
		/// </summary>
		public StiSqlSource() : this("", "", "")
		{
		}

		/// <summary>
		/// Creates a new object of the type StiSqlSource.
		/// </summary>
		/// <param name="nameInSource">Name of Sql Connection in the DataStore.</param>
		/// <param name="name">Data Source name.</param>
		public StiSqlSource(string nameInSource, string name) : this(nameInSource, name, name)
		{
		}

		/// <summary>
		/// Creates a new object of the type StiSqlSource.
		/// </summary>
		/// <param name="nameInSource">Name of Sql Connection in the DataStore.</param>
		/// <param name="name">Data Source name.</param>
		/// <param name="alias">Data Source alias.</param>
		public StiSqlSource(string nameInSource, string name, string alias) : this(nameInSource, name, alias, string.Empty)
		{
		}

		/// <summary>
		/// Creates a new object of the type StiSqlSource.
		/// </summary>
		/// <param name="nameInSource">Name of SqlConnection in the DataStore.</param>
		/// <param name="name">Data Source name.</param>
		/// <param name="alias">Data Source alias.</param>
		/// <param name="sqlCommand">SQL statement to execute at the Data Source.</param>
		public StiSqlSource(string nameInSource, string name, string alias, string sqlCommand) : 
			this(nameInSource, name, alias, sqlCommand, true)
		{
		}

		public StiSqlSource(string nameInSource, string name, string alias, string sqlCommand, 
			bool connectOnStart) : 
			this(nameInSource, name, alias, sqlCommand, connectOnStart, false)
		{
		}

		public StiSqlSource(string nameInSource, string name, string alias, string sqlCommand, 
			bool connectOnStart, bool reconnectOnEachRow) : 
			this(nameInSource, name, alias, sqlCommand, connectOnStart, reconnectOnEachRow, 30)
		{
		}

		public StiSqlSource(string nameInSource, string name, string alias, string sqlCommand, 
			bool connectOnStart, bool reconnectOnEachRow, int commandTimeout) : 
			base(nameInSource, name, alias)
		{
			this.SqlCommand = sqlCommand;
			this.ConnectOnStart = connectOnStart;
			this.ReconnectOnEachRow = reconnectOnEachRow;
			this.CommandTimeout = commandTimeout;
			this.ConnectionOrder = (int)StiConnectionOrder.Sql;
		}

        public StiSqlSource(string nameInSource, string name, string alias, string sqlCommand, bool connectOnStart, bool reconnectOnEachRow, int commandTimeout, string key) :
            base(nameInSource, name, alias, key)
        {
            this.SqlCommand = sqlCommand;
            this.ConnectOnStart = connectOnStart;
            this.ReconnectOnEachRow = reconnectOnEachRow;
            this.CommandTimeout = commandTimeout;
            this.ConnectionOrder = (int)StiConnectionOrder.Sql;
        }
	}
}