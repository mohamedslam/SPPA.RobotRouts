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
using Stimulsoft.Base.Localization;
using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Linq;
using static Stimulsoft.Report.StiRecentConnections;

namespace Stimulsoft.Report.Dictionary
{
    /// <summary>
    /// Describes the adapter for access to SqlConnection.
    /// </summary>
    public class StiSqlAdapterService : StiDataStoreAdapterService
    {
        #region Properties
        /// <summary>
		/// Gets a service name.
		/// </summary>
        public override string ServiceName => string.Format(StiLocalization.Get("Adapters", "AdapterConnection"), CreateConnector().Name);
        #endregion

        #region Methods
        /// <summary>
        /// Returns name of category for data.
        /// </summary>
        public override string GetDataCategoryName(StiData data)
		{
			return data.Name;
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
                    var connection = data.ViewData as DbConnection;
                    var connector = CreateConnector(data);
                    if (data.Data != null && data.Data.GetType() == connector.ConnectionType)
					{						
						OpenConnection(connection, data, dataSource.Dictionary);
						
						var storedProc = sqlSource.Type == StiSqlSourceType.StoredProcedure;

                        #region Stored Procedure
                        if (storedProc)
                        {
                            try
                            {
                                using (var command = connector.CreateCommand(sqlSource.SqlCommand, connection, CommandType.StoredProcedure))
                                {
                                    connector.SetTimeout(command, sqlSource.CommandTimeout);
                                    if (dataSource.Parameters.Count > 0 && command.Parameters.Count == 0)
                                    {
                                        foreach (StiDataParameter parameter in dataSource.Parameters)
                                        {
                                            command.Parameters.Add(connector.CreateParameter(parameter.Name, parameter.Value == "" ? parameter.ParameterValue : parameter.Value, parameter.Size));
                                        }
                                    }
                                    else
                                    {
                                        connector.DeriveParameters(command);
                                        if (retrieveMode == CommandBehavior.SchemaOnly)
                                        {
                                            foreach (DbParameter param in command.Parameters)
                                            {
                                                if (param.Direction == ParameterDirection.Input)
                                                    dataSource.Parameters.Add(new StiDataParameter(param.ParameterName, connector.GetSqlType(StiDbTypeConversion.GetNetType(param.DbType)), param.Size));
                                            }
                                        }
                                    }

                                    using (var reader = connector.ExecuteReader(command, retrieveMode))
                                    using (var table = new DataTable())
                                    {
                                        table.Load(reader);

                                        foreach (DataColumn column in table.Columns)
                                        {
                                            dataColumns.Add(new StiDataColumn(column.ColumnName, column.Caption, column.DataType));
                                        }
                                    }
                                }
                            }
                            catch
                            {
                            }
                        }
                        #endregion

                        if (!storedProc)
                        {
                            if (StiOptions.Engine.RetrieveColumnsMode == StiRetrieveColumnsMode.FillSchema)
                            {
                                using (var adapter = connector.CreateAdapter(sqlSource.SqlCommand, connection))
                                using (var table = new DataTable())
                                {
                                    connector.SetTimeout(adapter.SelectCommand, sqlSource.CommandTimeout);

                                    foreach (StiDataParameter parameter in dataSource.Parameters)
                                    {
                                        adapter.SelectCommand.Parameters.Add(connector.CreateParameter(parameter.Name, parameter.Value ?? parameter.ParameterValue, parameter.Size));
                                    }

                                    connector.FillSchemaAdapter(adapter, table, SchemaType.Source);

                                    foreach (DataColumn col in table.Columns)
                                    {
                                        var type = Type.GetType(col.DataType.ToString());
                                        if (type == null)
                                            type = typeof(object);

                                        dataColumns.Add(new StiDataColumn(col.ColumnName, col.ColumnName, type));
                                    }
                                }
                            }
                            else
                            {
                                using (var command = connector.CreateCommand(sqlSource.SqlCommand, connection))
                                {
                                    connector.SetTimeout(command, sqlSource.CommandTimeout);

                                    #region Parameters
                                    foreach (StiDataParameter parameter in dataSource.Parameters)
                                    {
                                        command.Parameters.Add(connector.CreateParameter(parameter.Name, parameter.Value == "" ? parameter.ParameterValue : parameter.Value, parameter.Size));
                                    }
                                    #endregion

                                    #region Variables as Parameters
                                    var variables = StiVariableAsParameterHelper.FetchAll(sqlSource.SqlCommand, sqlSource.Parameters.ToList(), sqlSource.Dictionary.Report);
                                    if (variables != null && variables.Count > 0)
                                    {
                                        foreach (var variable in variables)
                                        {
                                            try
                                            {
                                                var value = sqlSource.Dictionary.Report[variable.Name];
                                                if (value == null) value = variable.Eval(sqlSource.Dictionary.Report);

                                                var parameter = connector.CreateParameter($"@{variable.Name}", value);
                                                command.Parameters.Add(parameter);
                                            }
                                            catch
                                            {
                                            }
                                        }
                                    }
                                    #endregion

                                    using (var reader = connector.ExecuteReader(command, retrieveMode))
                                    using (var table = reader.GetSchemaTable())
                                    {
                                        if (table != null)
                                        {
                                            foreach (DataRow row in table.Rows)
                                            {
                                                var type = Type.GetType(row["DataType"].ToString());
                                                if (type == null)
                                                    type = typeof(object);

                                                dataColumns.Add(new StiDataColumn(row["ColumnName"].ToString(), row["ColumnName"].ToString(), type));
                                            }
                                        }
                                    }
                                }
                            }

                        }

					    CloseConnection(data, connection);
					}
				}
			}
			catch (Exception e)
			{
				StiLogService.Write(this.GetType(), e);
				if (!StiOptions.Engine.HideExceptions)throw;
			}
			
			return dataColumns;
		}

        /// <summary>
        /// Returns a collection of parameters of data.
        /// </summary>
        /// <param name="data">Data to find parameters.</param>
        /// <returns>Collection of parameters found.</returns>
        public override StiDataParametersCollection GetParametersFromData(StiData data, StiDataSource dataSource)
        {
            var dataParameters = new StiDataParametersCollection();
            var sqlSource = dataSource as StiSqlSource;

            if (sqlSource.Type != StiSqlSourceType.StoredProcedure)
                return dataParameters;

            try
            {
                if (!string.IsNullOrEmpty(sqlSource.SqlCommand))
                {
                    var connection = data.ViewData as DbConnection;
                    var connector = CreateConnector(data);

                    if (data.Data != null && data.Data.GetType() == connector.ConnectionType)
                    {
                        OpenConnection(connection, data, dataSource.Dictionary);
                        try
                        {
                            using (var command = connector.CreateCommand(sqlSource.SqlCommand, connection, CommandType.StoredProcedure))
                            {
                                connector.SetTimeout(command, sqlSource.CommandTimeout);
                                connector.DeriveParameters(command);
                                if (command.Parameters.Count > 0)
                                {
                                    foreach (DbParameter param in command.Parameters)
                                    {
                                        if (param.Direction == ParameterDirection.Input)
                                            dataParameters.Add(new StiDataParameter(param.ParameterName, connector.GetSqlType(StiDbTypeConversion.GetNetType(param.DbType)), param.Size));
                                    }
                                }
                            }
                        }
                        catch
                        {
                        }
                        CloseConnection(data, connection);
                    }
                }
            }
            catch
            {
            }
            return dataParameters;
        }

		/// <summary>
		/// Fills a name and alias of the Data Source relying on data.
		/// </summary>
		/// <param name="data">Data relying on which names will be filled.</param>
		/// <param name="dataSource">Data Source in which names will be filled.</param>
		public override void SetDataSourceNames(StiData data, StiDataSource dataSource)
		{
			base.SetDataSourceNames(data, dataSource);

            var connector = CreateConnector(data);
			dataSource.Name = dataSource.Alias = $"{connector.Name.Replace(" ", "")}Source";
		}

		/// <summary>
		/// Returns the type of the Data Source.
		/// </summary>
		/// <returns>The type of Data Source.</returns>
		public override Type GetDataSourceType()
		{
			return typeof(StiSqlSource);
		}
                        
	    /// <summary>
	    /// Returns the array of data types to which the Data Source may refer.
	    /// </summary>
	    /// <returns>Array of data types.</returns>
	    public override Type[] GetDataTypes()
	    {
	        try
	        {
	            return new[] { CreateConnector().CreateConnection().GetType() };
	        }
	        catch
	        {
                return null;
	        }
	    }

        /// <summary>
        /// Returns true if the specified type is supported by this data adapters.
        /// </summary>
        public override bool IsAdapterDataType(Type type)
        {
            if (type == null)
                return false;

            var connector = CreateConnector();
            if (connector == null)
                return false;

            return connector.TypeConnection == type.ToString();
        }
	
		public override void ConnectDataSourceToData(StiDictionary dictionary, StiDataSource dataSource, bool loadData)
		{
            StiDataLeader.Disconnect(dataSource);

            if (!loadData)
			{
				dataSource.DataTable = new DataTable();
				return;
			}			
			
			var sqlSource = dataSource as StiSqlSource;
			var nameInSource = sqlSource.NameInSource.ToLowerInvariant();

            var datas = dataSource.Dictionary.DataStore.ToList().Where(d => d.Name.ToLowerInvariant() == nameInSource);
            foreach (var data in datas)
            {
                try
                {
                    var connector = CreateConnector(data);
                    if (data.Data == null || data.Data.GetType() != connector.ConnectionType) continue;

                    var connection = data.ViewData as DbConnection;
                    OpenConnection(connection, data, dataSource.Dictionary);

                    var commandType = sqlSource.Type == StiSqlSourceType.StoredProcedure ? CommandType.StoredProcedure : CommandType.Text;
                    sqlSource.DataAdapter = connector.CreateAdapter(sqlSource.SqlCommand, connection, commandType);

                    #region Parameters
                    foreach (StiDataParameter parameter in sqlSource.Parameters)
                    {
                        var param = connector.CreateParameter(parameter.Name, parameter.Type, parameter.Size) as IDbDataParameter;
                        if (param.DbType == DbType.Decimal)
                            param.Precision = (byte)parameter.Size;

                        sqlSource.DataAdapter.SelectCommand.Parameters.Add(param);
                    }
                    #endregion

                    #region Variables as Parameters
                    var variables = StiVariableAsParameterHelper.FetchAll(sqlSource.SqlCommand, sqlSource.Parameters.ToList(), dictionary.Report);
                    if (variables != null && variables.Count > 0)
                    {
                        foreach (var variable in variables)
                        {
                            try
                            {
                                var value = dictionary.Report[variable.Name];
                                if (value == null)
                                    value = variable.Eval(dictionary.Report);

                                var parameter = connector.CreateParameter($"@{variable.Name}", value);
                                sqlSource.DataAdapter.SelectCommand.Parameters.Add(parameter);
                            }
                            catch
                            {
                                
                            }
                        }
                    }
                    #endregion

                    if (sqlSource.Type == StiSqlSourceType.StoredProcedure && this is StiOleDbAdapterService)
                    {
                        using (var tempTableCommand = connector.CreateCommand("set nocount on", connection))
                        {
                            connector.ExecuteNonQuery(tempTableCommand);
                        }
                    }

                    StiDataLeader.RetrieveData(sqlSource, !loadData);

                    break;
                }
                catch (Exception e)
                {
                    StiLogService.Write(this.GetType(), e);
                    if (!StiOptions.Engine.HideExceptions) throw;
                }
            }
		}

        /// <summary>
        /// Returns new data connector for this type of the database.
        /// </summary>
        /// <returns>Created connector.</returns>
        public virtual StiSqlDataConnector CreateConnector(StiData data)
        {
            if (data == null) 
                return CreateConnector();

            var connection = data.ViewData as DbConnection;
            return CreateConnector(connection != null ? connection.ConnectionString : null);
        }
	    
	    /// <summary>
	    /// Returns new data connector for this type of the database.
	    /// </summary>
        /// <returns>Created connector.</returns>
	    public virtual StiSqlDataConnector CreateConnector(string connectionString = "")
	    {
	        return StiMsSqlConnector.Get(connectionString);
	    }        
		#endregion

        #region Methods.Connection
        protected virtual void OpenConnection(IDbConnection connection)
        {
            OpenConnection(connection, null, null);
        }

        protected virtual void OpenConnection(IDbConnection connection, StiData data, StiDictionary dictionary)
        {
            var database = data != null && dictionary != null ?
                dictionary.Databases.ToList().FirstOrDefault(d => d.Name.ToLowerInvariant() == data.Name.ToLowerInvariant()) as StiSqlDatabase : null;

            if (database != null && database.PromptUserNameAndPassword)
            {
                var connectionString = connection.ConnectionString;

                string userName = null;
                string password = null;

                if (StiDictionary.CacheUserNamesAndPasswords != null && StiDictionary.CacheUserNamesAndPasswords[connection.ConnectionString] != null)
                {
                    lock (StiDictionary.CacheUserNamesAndPasswords)
                    {
                        var info = StiDictionary.CacheUserNamesAndPasswords[connection.ConnectionString] as StiUserNameAndPassword;

                        userName = info.UserName;
                        password = info.Password;
                    }
                }
                else if (!dictionary.Report.IsDesigning)
                {
                    var info = StiDataEditorsHelper.Get().PromptUserNameAndPassword();
                    if (info != null)
                    {
                        userName = info.UserName;
                        password = info.Password;

                        if (info.CacheData)
                        {
                            if (StiDictionary.CacheUserNamesAndPasswords == null)
                                StiDictionary.CacheUserNamesAndPasswords = new Hashtable();

                            lock (StiDictionary.CacheUserNamesAndPasswords)
                            {
                                StiDictionary.CacheUserNamesAndPasswords[connection.ConnectionString] = info;
                            }
                        }
                    }
                }

                if (userName != null && password != null)
                {
                    connectionString += database.MapUserNameAndPassword(userName, password);

                    if (connection.State != ConnectionState.Closed)
                    {
                        StiDataMonitor.Close(connection);
                        connection.Close();
                    }

                    connection.ConnectionString = connectionString;
                }
            }

            if (connection.State == ConnectionState.Closed)
            {
                StiDataMonitor.Open(connection);
                connection.Open();
            }
        }

        protected virtual void CloseConnection(StiData data, IDbConnection connection)
        {
            if (data.OriginalConnectionState is ConnectionState)
            {
                if ((ConnectionState)data.OriginalConnectionState != ConnectionState.Open)
                {
                    StiDataMonitor.Close(connection);
                    connection.Close();
                }
            }
            else if (connection.State != ConnectionState.Closed)
            {
                StiDataMonitor.Close(connection);
                connection.Close();
            }
        }

        public virtual string TestConnection(string connectionString)
        {
            try
            {
                var connector = CreateConnector(connectionString);
                var result = connector.TestConnection();

                return result.Success ? Loc.Get("DesignerFx", "ConnectionSuccessfull") : result.Notice;

            }
            catch (Exception e)
            {
                return Loc.Get("DesignerFx", "ConnectionError") + ": " + e.Message;
            }
        }

        public virtual void CreateConnectionInDataStore(StiDictionary dictionary, StiSqlDatabase database)
        {
            try
            {
                if (database.Name == null) return;

                //Remove all old data from datastore
                var data = dictionary.DataStore.ToList().FirstOrDefault(d => d.Name != null && d.Name.ToLowerInvariant() == database.Name.ToLowerInvariant());
                if (data != null)
                    dictionary.DataStore.Remove(data);

                var connector = CreateConnector(database.ConnectionString);
                dictionary.DataStore.Add(new StiData(database.Name, connector.CreateConnection())
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
