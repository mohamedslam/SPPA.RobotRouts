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

namespace Stimulsoft.Base
{
    public class StiPostgreSqlConnector : StiSqlDataConnector
    {
        #region Properties
        /// <summary>
        /// Gets a type of the connection helper.
        /// </summary>
        public override StiConnectionIdent ConnectionIdent => StiConnectionIdent.PostgreSqlDataSource;

        /// <summary>
        /// Gets an order of the connector.
        /// </summary>
        public override StiConnectionOrder ConnectionOrder => StiConnectionOrder.PostgreSqlDataSource;

        public override string Name => "PostgreSQL";

        /// <summary>
        /// Gets the type of an enumeration which describes data types.
        /// </summary>
        public override Type SqlType => typeof(StiDbType.PostgreSql);

        /// <summary>
        /// Gets the default value of the data parameter.
        /// </summary>
        public override int DefaultSqlType => (int)StiDbType.PostgreSql.Varchar;

        /// <summary>
        /// Gets the package identificator for this connector.
        /// </summary>
        public override string[] NuGetPackages => new string[] { "Npgsql" };

        /// <summary>
        /// Gets the package version for this connector.
        /// </summary>
        public override string NuGetVersion => "3.2.7";
        #endregion

        #region Methods
        public override void ResetSettings()
        {
            isGeneric = null;
            isDevart = null;
        }

        /// <summary>
        /// Return an array of the data connectors which can be used also to access data for this type of the connector.
        /// </summary>
        public override StiDataConnector[] GetFamilyConnectors()
        {
            return new StiDataConnector[]
            {
                new StiPostgreSqlConnector(),
                new StiPostgreSqlDevartConnector()
            };
        }

        /// <summary>
        /// Returns new SQL parameter with specified parameter.
        /// </summary>
        public override DbParameter CreateParameter(string parameterName, object value, int size)
        {
            var parameter = AssemblyHelper.CreateParameterWithValue(TypeParameter, parameterName, value);
            parameter.Size = size;
            return parameter;
        }

        /// <summary>
        /// Returns new SQL parameter with specified parameter.
        /// </summary>
        public override DbParameter CreateParameter(string parameterName, int type, int size)
        {
            var dbType = GetDbType();
            var parameter = AssemblyHelper.CreateParameterWithType(TypeParameter, parameterName, type, dbType ?? typeof(int));
            parameter.Size = size;
            return parameter;
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

                    var dbName = ExtractDataBaseNameFromConnectionString();
                    var schemaName = StiBaseOptions.RetrieveSchemaNamePostgreSql;

                    #region Tables & Views
                    var tableHash = new Hashtable();

                    try
                    {
                        var userTables = GetSchema(connection, "Tables", new[] { dbName, schemaName, null, null });
                        foreach (var row in StiSchemaRow.All(userTables))
                        {
                            var tableName = (schemaName != "public" ? string.Format("{0}.{1}", row.TABLE_SCHEMA, row.TABLE_NAME) : row.TABLE_NAME);
                            var table = StiDataTableSchema.NewTableOrView(tableName);
                            tableHash[row.TABLE_NAME] = table;

                            if (row.TABLE_TYPE == "BASE TABLE")
                                schema.Tables.Add(table);
                        }
                    }
                    catch
                    {
                    }

                    try
                    {
                        var userTables = GetSchema(connection, "Views", new[] { dbName, schemaName, null, null });
                        foreach (var row in StiSchemaRow.All(userTables))
                        {
                            var tableName = (schemaName != "public" 
                                ? string.Format("{0}.{1}", row.TABLE_SCHEMA, row.TABLE_NAME) 
                                : row.TABLE_NAME);

                            var table = StiDataTableSchema.NewTableOrView(tableName);
                            tableHash[row.TABLE_NAME] = table;

                            schema.Views.Add(table);
                        }
                    }
                    catch
                    {
                    }
                    #endregion

                    #region Columns
                    try
                    {
                        var columns = GetSchema(connection, "Columns", new string[] { dbName, schemaName, null });
                        foreach (var row in StiSchemaRow.All(columns))
                        {
                            var column = new StiDataColumnSchema(row.COLUMN_NAME, GetNetType(row.DATA_TYPE));

                            var table = tableHash[row.TABLE_NAME] as StiDataTableSchema;
                            if (table != null)
                                table.Columns.Add(column);
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
                                        procedure.Columns.Add(new StiDataColumnSchema(column.ColumnName, column.DataType));
                                    }

                                    foreach (DbParameter param in command.Parameters)
                                    {
                                        if (param.Direction != ParameterDirection.Input) continue;

                                        procedure.Parameters.Add(new StiDataParameterSchema(param.ParameterName, StiDbTypeConversion.GetNetType(param.DbType)));
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
                            SELECT
                                tc.constraint_name, tc.table_name, kcu.column_name, 
                                ccu.table_name AS foreign_table_name,
                                ccu.column_name AS foreign_column_name 
                            FROM 
                                information_schema.table_constraints AS tc 
                                JOIN information_schema.key_column_usage AS kcu
                                  ON tc.constraint_name = kcu.constraint_name
                                JOIN information_schema.constraint_column_usage AS ccu
                                  ON ccu.constraint_name = tc.constraint_name
                            WHERE constraint_type = 'FOREIGN KEY'";

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
                                        ParentSourceName = row.TABLE_NAME,
                                        ChildSourceName = row.FOREIGN_TABLE_NAME,
                                        ParentColumns = new List<string> { row.COLUMN_NAME },
                                        ChildColumns = new List<string> { row.FOREIGN_COLUMN_NAME }
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
                return (int)StiDbType.PostgreSql.Date;

            if (type == typeof(TimeSpan)) 
                return (int)StiDbType.PostgreSql.Interval;

            if (type == typeof(Int64)) 
                return (int)StiDbType.PostgreSql.Bigint;

            if (type == typeof(Int32)) 
                return (int)StiDbType.PostgreSql.Integer;

            if (type == typeof(Int16)) 
                return (int)StiDbType.PostgreSql.Smallint;

            if (type == typeof(Byte)) 
                return (int)StiDbType.PostgreSql.Smallint;

            if (type == typeof(UInt64)) 
                return (int)StiDbType.PostgreSql.Bigint;

            if (type == typeof(UInt32)) 
                return (int)StiDbType.PostgreSql.Integer;

            if (type == typeof(UInt16)) 
                return (int)StiDbType.PostgreSql.Smallint;

            if (type == typeof(SByte)) 
                return (int)StiDbType.PostgreSql.Smallint;

            if (type == typeof(Single)) 
                return (int)StiDbType.PostgreSql.Real;

            if (type == typeof(Double)) 
                return (int)StiDbType.PostgreSql.Double;

            if (type == typeof(Decimal)) 
                return (int)StiDbType.PostgreSql.Numeric;

            if (type == typeof(String)) 
                return (int)StiDbType.PostgreSql.Text;

            if (type == typeof(Boolean)) 
                return (int)StiDbType.PostgreSql.Boolean;

            if (type == typeof(Char)) 
                return (int)StiDbType.PostgreSql.Char;

            if (type == typeof(Byte[])) 
                return (int)StiDbType.PostgreSql.Bytea;

            if (type == typeof(Guid)) 
                return (int)StiDbType.PostgreSql.Uuid;

            return (int)StiDbType.PostgreSql.Integer;
        }

        /// <summary>
        /// Returns a .NET type from the specified string representaion of the database type.
        /// </summary>
        public override Type GetNetType(int dbType)
        {
            switch ((StiDbType.PostgreSql)dbType)
            {
                case StiDbType.PostgreSql.Bigint:
                case StiDbType.PostgreSql.Integer:
                case StiDbType.PostgreSql.Numeric:
                    return typeof(Int64);

                case StiDbType.PostgreSql.Smallint:
                    return typeof(Int16);

                case StiDbType.PostgreSql.Money:
                    return typeof(decimal);

                case StiDbType.PostgreSql.Real:
                    return typeof(float);

                case StiDbType.PostgreSql.Double:
                    return typeof(double);

                case StiDbType.PostgreSql.Date:
                case StiDbType.PostgreSql.Abstime:
                case StiDbType.PostgreSql.Timestamp:
                case StiDbType.PostgreSql.TimestampTZ:
                    return typeof(DateTime);

                case StiDbType.PostgreSql.Time:
                    return typeof(TimeSpan);

                case StiDbType.PostgreSql.TimeTZ:
                    return typeof(DateTimeOffset);

                case StiDbType.PostgreSql.Boolean:
                    return typeof(Boolean);

                default:
                    return typeof(string);
            }
        }

        /// <summary>
        /// Returns a .NET type from the specified string representaion of the database type.
        /// </summary>
        public override Type GetNetType(string dbType)
        {
            switch (dbType.ToLowerInvariant())
            {
                case "bigint":
                case "int":
                case "int4":
                case "int8":
                case "integer":
                case "numeric":
                case "uniqueidentifier":
                    return typeof(Int64);

                case "smallint":
                    return typeof(Int16);

                case "tinyint":
                    return typeof (SByte);

                case "decimal":
                case "money":
                case "smallmoney":
                    return typeof (decimal);

                case "float":
                case "real":
                    return typeof(float);

                case "double":
                    return typeof (double);

                case "abstime":
                case "date":
                case "datetime":
                case "smalldatetime":
                case "timestamp":
                case "timestamptz":
                    return typeof (DateTime);

                case "time":
                    return typeof(TimeSpan);

                case "timetz":
                    return typeof(DateTimeOffset);

                case "boolean":
                    return typeof (Boolean);

                case "bytea":
                    return typeof(Byte[]);

                default:
                    return typeof (string);
            }
        }

        /// <summary>
        /// Returns sample of the connection string to this connector.
        /// </summary>
        public override string GetSampleConnectionString()
        {
            return StiDataOptions.SampleConnectionString.PostgreSql;
        }
        #endregion

        #region Methods.Helpers
        private string ExtractDataBaseNameFromConnectionString()
        {
            var dbName = string.Empty;

            var keys = ConnectionString.Split(';');
            foreach (var key in keys)
            {
                var parameters = key.Split('=');
                if (parameters.Length == 2 && parameters[0] != null && parameters[1] != null && parameters[0].ToLower().Contains("database"))
                    dbName = parameters[1].Trim(' ');
                
                break;
            }

            return dbName;
        }
        #endregion

        #region Fields.Static
        private static object lockObject = new object();
        private static bool? isGeneric = null;
        private static bool? isDevart = null;
        #endregion

        #region Methods.Static
        public static StiPostgreSqlConnector Get(string connectionString = null)
        {
            lock (lockObject)
            {
                if (isGeneric == true) 
                    return new StiPostgreSqlConnector(connectionString);

                if (isDevart == true) 
                    return new StiPostgreSqlDevartConnector(connectionString);

                if (connectionString == null) 
                    return new StiPostgreSqlConnector();

                if (isGeneric != true && isDevart != true)
                {
                    isGeneric = null;
                    isDevart = null;
                }

                if (isGeneric == null)
                {
                    var connector = LoadGenericConnector(connectionString);
                    if (isGeneric == true) 
                        return connector;
                }

                if (isDevart == null)
                {
                    var connector = LoadDevartConnector(connectionString);
                    if (isDevart == true) 
                        return connector;
                }

                isGeneric = true;
                return new StiPostgreSqlConnector(connectionString);
            }
        }

        public static StiPostgreSqlDevartConnector LoadDevartConnector(string connectionString = null)
        {
            var connector = new StiPostgreSqlDevartConnector(connectionString);
            isDevart = connector.IsAvailable;
            return connector;
        }

        public static StiPostgreSqlConnector LoadGenericConnector(string connectionString = null)
        {
            var connector = new StiPostgreSqlConnector(connectionString);
            isGeneric = connector.IsAvailable;
            return connector;
        }
        #endregion

        public StiPostgreSqlConnector(string connectionString = null)
            : base(connectionString)
        {
            this.NameAssembly = "Npgsql.dll";
            this.TypeConnection = "Npgsql.NpgsqlConnection";
            this.TypeDataAdapter = "Npgsql.NpgsqlDataAdapter";
            this.TypeCommand = "Npgsql.NpgsqlCommand";
            this.TypeParameter = "Npgsql.NpgsqlParameter";
            this.TypeDbType = "NpgsqlTypes.NpgsqlDbType";
            this.TypeCommandBuilder = "Npgsql.NpgsqlCommandBuilder";
        }
    }
}