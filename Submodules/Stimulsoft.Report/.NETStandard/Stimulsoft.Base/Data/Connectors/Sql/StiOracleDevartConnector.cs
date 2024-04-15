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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;

namespace Stimulsoft.Base
{
    public class StiOracleDevartConnector : StiOracleConnector
    {
        #region Properties
        /// <summary>
        /// Gets the type of an enumeration which describes data types.
        /// </summary>
        public override Type SqlType => typeof(StiDbType.Devart.Oracle);

        /// <summary>
        /// Gets the default value of the data parameter.
        /// </summary>
        public override int DefaultSqlType => (int)StiDbType.Devart.Oracle.NVarChar;

        /// <summary>
        /// Gets the package identificator for this connector.
        /// </summary>
        public override string[] NuGetPackages => new string[] { "dotConnect.Express.for.Oracle" };

        /// <summary>
        /// Gets the package version for this connector.
        /// </summary>
        public override string NuGetVersion => "9.3.230";
        #endregion

        #region Methods
        /// <summary>
        /// Returns new connection to this type of the database.
        /// </summary>
        /// <returns>Created connection.</returns>
        public override DbConnection CreateConnection()
        {
            string dbSchemaName;

            var connection = AssemblyHelper.CreateConnection(TypeConnection, 
                GetConvertedConnectionStringToDotConnect(this.ConnectionString, out dbSchemaName));

            var prop = connection.GetType().GetProperty("Direct");
            prop.SetValue(connection, true, null);

            return connection;
        }

        /// <summary>
        /// Retrieves SQL parameters for the specified command.
        /// </summary>
        public override void DeriveParameters(DbCommand command)
        {
            var prop = command.GetType().GetProperty("ParameterCheck");
            prop.SetValue(command, true, null);
        }

        /// <summary>
        /// Returns schema object which contains information about structure of the database. Schema returned start at specified root element (if it applicable).
        /// </summary>
        public override StiDataSchema RetrieveSchema(bool allowException = false)
        {
            if (string.IsNullOrEmpty(this.ConnectionString)) 
                return null;

            var schema = new StiDataSchema(this.ConnectionIdent);

            try
            {
                using (var connection = CreateConnection())
                {
                    OpenConnection(connection);

                    string dbSchemaName = null;
                    GetConvertedConnectionStringToDotConnect(this.ConnectionString, out dbSchemaName);

                    #region Tables
                    var tableList = new List<StiDataTableSchema>();
                    try
                    {
                        var tables = GetSchema(connection, "Tables", new[] { dbSchemaName, null });
                        foreach (DataRow row in tables.Rows)
                        {
                            if (row["SCHEMA"] != DBNull.Value && ((string)row["SCHEMA"]) != "dbo")
                            {
                                var tableName = StiTableName.GetName("", row["NAME"] as string);
                                var query = StiTableQuery.Get(this).GetSelectQuery(row["SCHEMA"] as string, row["NAME"] as string);
                                var table = StiDataTableSchema.NewTableOrView(tableName, this, query);

                                tableList.Add(table);
                                schema.Tables.Add(table);
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                    #endregion

                    #region Views
                    try
                    {
                        var views = GetSchema(connection, "Views", new[] { dbSchemaName, null });
                        foreach (var row in StiSchemaRow.All(views))
                        {
                            var viewName = StiTableName.GetName(row.OWNER, row.TABLE_NAME);
                            var query = StiTableQuery.Get(this).GetSelectQuery(row.OWNER, row.TABLE_NAME);
                            var view = StiDataTableSchema.NewTableOrView(viewName, this, query);

                            tableList.Add(view);
                            schema.Views.Add(view);
                        }
                    }
                    catch
                    {
                    }
                    #endregion

                    #region Columns
                    try
                    {
                        foreach (var table in tableList)
                        {
                            var columns = GetSchema(connection, "Columns", new[] { dbSchemaName, table.Name, null });
                            foreach (DataRow row in columns.Rows)
                            {
                                var tableName = string.Empty;
                                if (row["SCHEMA"] != DBNull.Value && ((string)row["SCHEMA"]) != "sys" && ((string)row["SCHEMA"]) != "dbo")
                                    tableName = string.Format("{1}.{0}", row["TABLE"] as string, row["SCHEMA"] as string);
                                else
                                    tableName = row["TABLE"] as string;

                                var column = new StiDataColumnSchema(row["NAME"] as string, GetNetType(row["DATATYPE"] as string));
                                table.Columns.Add(column);
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                    #endregion

                    #region Procedures
                    var procedureHash = new Hashtable();
                    try
                    {
                        var procedures = GetSchema(connection, "Procedures", new[] { dbSchemaName, null });
                        foreach (var row in StiSchemaRow.All(procedures))
                        {
                            var procName = StiTableName.GetName(row.OWNER, row.OBJECT_NAME);
                            var query = StiTableQuery.Get(this).GetProcQuery(row.OWNER, row.OBJECT_NAME);
                            var proc = StiDataTableSchema.NewProcedure(procName, this, query);

                            procedureHash[procName] = proc;
                            schema.StoredProcedures.Add(proc);
                        }
                    }
                    catch
                    {
                    }
                    #endregion

                    #region Procedures Parameters and Columns
                    foreach (var procedure in schema.StoredProcedures)
                    {
                        try
                        {
                            using (var command = CreateCommand(procedure.Query, connection, CommandType.StoredProcedure))
                            {
                                DeriveParameters(command);

                                using (var reader = ExecuteReader(command, CommandBehavior.SchemaOnly))
                                using (var table = new DataTable(procedure.Name))
                                {
                                    table.Load(reader);

                                    foreach (DataColumn column in table.Columns)
                                    {
                                        procedure.Columns.Add(new StiDataColumnSchema
                                        {
                                            Name = column.ColumnName,
                                            Type = column.DataType
                                        });
                                    }

                                    foreach (DbParameter param in command.Parameters)
                                    {
                                        if (param.Direction == ParameterDirection.Input)
                                        {
                                            procedure.Parameters.Add(new StiDataParameterSchema
                                            {
                                                Name = param.ParameterName,
                                                Type = StiDbTypeConversion.GetNetType(param.DbType)
                                            });
                                        }
                                    }
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                    #endregion

                    #region Relations
                    try
                    {
                        var commandText = @"
                            SELECT a.table_name, a.column_name, a.constraint_name, c.owner, 
                                   c.r_owner, c_pk.table_name r_table_name, c_pk.constraint_name r_pk
                            FROM all_cons_columns a
                            JOIN all_constraints c ON a.owner = c.owner
                                 AND a.constraint_name = c.constraint_name
                            JOIN all_constraints c_pk ON c.r_owner = c_pk.owner
                                 AND c.r_constraint_name = c_pk.constraint_name
                            WHERE c.constraint_type = 'R'";

                        using (var dataSet = new DataSet())
                        using (var adapter = CreateAdapter(commandText, connection))
                        {
                            FillAdapter(adapter, dataSet);

                            var dataTable = (dataSet.Tables.Count > 0) ? dataSet.Tables[0] : null;
                            if (dataTable != null)
                            {
                                foreach (var row in StiSchemaRow.All(dataTable))
                                {
                                    schema.Relations.Add(new StiDataRelationSchema
                                    {
                                        Name = row.CONSTRAINT_NAME,
                                        ParentSourceName = string.Format("{0}.{1}", row.OWNER, row.TABLE_NAME),
                                        ChildSourceName = string.Format("{0}.{1}", row.R_OWNER, row.R_TABLE_NAME),
                                        ParentColumns = new List<string> { row.COLUMN_NAME },
                                        ChildColumns = new List<string> { row.R_PK }
                                    });
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                    #endregion

                    CloseConnection(connection);
                }

                return schema.Sort();
            }
            catch (Exception)
            {
                if (allowException) throw;

                return null;
            }
        }

        /// <summary>
        /// Returns a SQL based type from the .NET type.
        /// </summary>
        public override int GetSqlType(Type type)
        {
            if (type == typeof(DateTime)) 
                return (int)StiDbType.Devart.Oracle.Date;

            if (type == typeof(TimeSpan)) 
                return (int)StiDbType.Devart.Oracle.TimeStamp;

            if (type == typeof(Int64)) 
                return (int)StiDbType.Devart.Oracle.Number;

            if (type == typeof(Int32)) 
                return (int)StiDbType.Devart.Oracle.Integer;

            if (type == typeof(Int16)) 
                return (int)StiDbType.Devart.Oracle.Integer;

            if (type == typeof(Byte)) 
                return (int)StiDbType.Devart.Oracle.Integer;

            if (type == typeof(UInt64)) 
                return (int)StiDbType.Devart.Oracle.Number;

            if (type == typeof(UInt32)) 
                return (int)StiDbType.Devart.Oracle.Integer;

            if (type == typeof(UInt16)) 
                return (int)StiDbType.Devart.Oracle.Integer;

            if (type == typeof(SByte)) 
                return (int)StiDbType.Devart.Oracle.Integer;

            if (type == typeof(Single)) 
                return (int)StiDbType.Devart.Oracle.Float;

            if (type == typeof(Double)) 
                return (int)StiDbType.Devart.Oracle.Double;

            if (type == typeof(Decimal)) 
                return (int)StiDbType.Devart.Oracle.Number;

            if (type == typeof(String)) 
                return (int)StiDbType.Devart.Oracle.VarChar;

            if (type == typeof(Boolean)) 
                return (int)StiDbType.Devart.Oracle.Boolean;

            if (type == typeof(Char)) 
                return (int)StiDbType.Devart.Oracle.Char;

            if (type == typeof(Byte[])) 
                return (int)StiDbType.Devart.Oracle.Blob;

            if (type == typeof(Guid)) 
                return (int)StiDbType.Devart.Oracle.Raw;

            return (int)StiDbType.Devart.Oracle.Integer;
        }

        /// <summary>
        /// Returns a .NET type from the specified string representaion of the database type.
        /// </summary>
        public override Type GetNetType(int dbType)
        {
            switch ((StiDbType.Devart.Oracle)dbType)
            {
                case StiDbType.Devart.Oracle.Date:
                    return typeof(byte[]);

                case StiDbType.Devart.Oracle.BFile:
                case StiDbType.Devart.Oracle.Blob:
                case StiDbType.Devart.Oracle.LongRaw:
                case StiDbType.Devart.Oracle.Raw:
                    return typeof(byte[]);

                case StiDbType.Devart.Oracle.Float:
                    return typeof(Single);

                case StiDbType.Devart.Oracle.Double:
                    return typeof(Double);

                case StiDbType.Devart.Oracle.Number:
                    return typeof(Decimal);

                case StiDbType.Devart.Oracle.Byte:
                    return typeof(Byte);

                case StiDbType.Devart.Oracle.Int16:
                    return typeof(Int16);

                case StiDbType.Devart.Oracle.Integer:
                    return typeof(Int32);

                case StiDbType.Devart.Oracle.Int64:
                case StiDbType.Devart.Oracle.Long:
                case StiDbType.Devart.Oracle.IntervalDS:
                case StiDbType.Devart.Oracle.IntervalYM:
                    return typeof(Int64);

                default:
                    return typeof(string);
            }
        }
        
        public override Type GetNetType(string dbType)
        {
            switch (dbType.ToLowerInvariant())
            {
                case "bigint":
                case "int":
                case "uniqueidentifier":
                case "smallint":
                case "tinyint":
                    return typeof(Int64);

                case "decimal":
                case "money":
                case "smallmoney":
                case "number":
                    return typeof(decimal);

                case "float":
                case "real":
                    return typeof(double);

                case "datetime":
                case "smalldatetime":
                case "timestamp":
                case "date":
                    return typeof(DateTime);

                default:
                    return typeof(string);
            }
        }
        #endregion

        #region Methods.Helpers
        private string GetConvertedConnectionStringToDotConnect(string connectionString, out string dbSchemaName)
        {
            var dbServer = string.Empty;
            var dbUser = string.Empty;
            var dbPassword = string.Empty;
            var dbSid = string.Empty;
            var dbDirect = string.Empty;

            #region Parse host (server) name
            var regex = new Regex(@"\(HOST\s*=\s*(?<val>[0-9a-zA-Z_-]+)\)");
            foreach (var match in regex.Matches(connectionString).Cast<Match>().Where(match => match.Success))
            {
                dbServer = match.Groups["val"].Value;
            }
            #endregion

            #region Parse server (server) name
            regex = new Regex(@"Server\s*=\s*(?<val>[\d\.]+)|Server\s*=\s*(?<val>[0-9a-zA-Z_-]+)");
            foreach (var match in regex.Matches(connectionString).Cast<Match>().Where(match => match.Success))
            {
                dbServer = match.Groups["val"].Value;
            }
            #endregion

            #region Parse Data Source (server) name
            regex = new Regex(@"Data Source\s*=\s*(?<val>[0-9a-zA-Z_-]+)");
            foreach (var match in regex.Matches(connectionString).Cast<Match>().Where(match => match.Success))
            {
                dbServer = match.Groups["val"].Value;
            }
            #endregion

            #region Parse sid
            regex = new Regex(@"Sid\s*=\s*(?<val>[0-9a-zA-Z_-]+)");
            foreach (var match in regex.Matches(connectionString).Cast<Match>().Where(match => match.Success))
            {
                dbSid = match.Groups["val"].Value;
            }
            #endregion

            #region Parse username
            regex = new Regex(@"User Id\s*=\s*(?<val>[0-9a-zA-Z_-]+)");
            foreach (var match in regex.Matches(connectionString).Cast<Match>().Where(match => match.Success))
            {
                dbUser = match.Groups["val"].Value;
            }

            regex = new Regex(@"uid\s*=\s*(?<val>[0-9a-zA-Z_-]+)");
            foreach (var match in regex.Matches(connectionString).Cast<Match>().Where(match => match.Success))
            {
                dbUser = match.Groups["val"].Value;
            }
            #endregion

            #region Parse password
            regex = new Regex(@"Password\s*=\s*(?<val>[0-9a-zA-Z_-]+)");
            foreach (var match in regex.Matches(connectionString).Cast<Match>().Where(match => match.Success))
            {
                dbPassword = match.Groups["val"].Value;
            }

            regex = new Regex(@"pwd\s*=\s*(?<val>[0-9a-zA-Z_-]+)");
            foreach (var match in regex.Matches(connectionString).Cast<Match>().Where(match => match.Success))
            {
                dbPassword = match.Groups["val"].Value;
            }
            #endregion

            regex = new Regex(@"Direct\s*=\s*(?<val>[0-9a-zA-Z_-]+)");
            foreach (var match in regex.Matches(connectionString).Cast<Match>().Where(match => match.Success))
            {
                dbDirect = match.Groups["val"].Value;
            }

            dbSchemaName = dbUser;
            return string.Format("User Id={0};Password={1};Server={2};Direct={3};SID={4};", dbUser, dbPassword, dbServer, dbDirect, dbSid);
        }
        #endregion

        public StiOracleDevartConnector(string connectionString = null)
            : base(connectionString)
        {
            this.NameAssembly = "Devart.Data.Oracle.dll";
            this.TypeConnection = "Devart.Data.Oracle.OracleConnection";
            this.TypeDataAdapter = "Devart.Data.Oracle.OracleDataAdapter";
            this.TypeCommand = "Devart.Data.Oracle.OracleCommand";
            this.TypeParameter = "Devart.Data.Oracle.OracleParameter";
            this.TypeDbType = "Devart.Data.Oracle.OracleDbType";
            this.TypeCommandBuilder = "Devart.Data.Oracle.CommandBuilder";
            this.TypeConnectionStringBuilder = "Devart.Data.Oracle.OracleConnectionStringBuilder";
            this.TypeDataSourceEnumerator = "Devart.Data.Oracle.OracleDataSourceEnumerator";
        }
    }
}