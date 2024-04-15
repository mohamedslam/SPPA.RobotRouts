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
using System.Data.Common;

namespace Stimulsoft.Base
{
    public class StiOracleConnector : StiSqlDataConnector
    {
        #region Properties
        /// <summary>
        /// Gets a type of the connection helper.
        /// </summary>
        public override StiConnectionIdent ConnectionIdent => StiConnectionIdent.OracleDataSource;

        /// <summary>
        /// Gets an order of the connector.
        /// </summary>
        public override StiConnectionOrder ConnectionOrder => StiConnectionOrder.OracleDataSource;

        public override string Name => "Oracle";

        /// <summary>
        /// Gets the type of an enumeration which describes data types.
        /// </summary>
        public override Type SqlType => typeof(StiDbType.Oracle);

        /// <summary>
        /// Gets the default value of the data parameter.
        /// </summary>
        public override int DefaultSqlType => (int)StiDbType.Oracle.NVarchar2;
        #endregion

        #region Methods
        public override void ResetSettings()
        {
            isGeneric = null;
            isManaged = null;
            isDevart = null;
            isClient = null;
        }

        /// <summary>
        /// Return an array of the data connectors which can be used also to access data for this type of the connector.
        /// </summary>
        public override StiDataConnector[] GetFamilyConnectors()
        {
            return new StiDataConnector[]
            {
                new StiOracleConnector(),
                new StiOracleManagedConnector(),
                new StiOracleDevartConnector(),
                new StiOracleClientConnector()
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

                    #region Tables
                    var tables = GetSchema(connection, "Tables");
                    var tableHash = new Hashtable();

                    try
                    {

                        foreach (var row in StiSchemaRow.All(tables))
                        {
                            if (row.TYPE == "System" || IsSystemOwner(row.OWNER)) continue;

                            var tableName = StiTableName.GetName(row.OWNER, row.TABLE_NAME);
                            var query = StiTableQuery.Get(this).GetSelectQuery(row.OWNER, row.TABLE_NAME);

                            var table = StiDataTableSchema.NewTableOrView(tableName, this, query);

                            tableHash[table.Name] = table;
                            schema.Tables.Add(table);
                        }
                    }
                    catch
                    {
                    }
                    #endregion

                    #region Views
                    var views = GetSchema(connection, "Views");
                    try
                    {
                        foreach (var row in StiSchemaRow.All(views))
                        {
                            if (IsSystemOwner(row.OWNER)) continue;

                            var viewName = StiTableName.GetName(row.OWNER, row.VIEW_NAME);
                            var query = StiTableQuery.Get(this).GetSelectQuery(row.OWNER, row.VIEW_NAME);

                            var view = StiDataTableSchema.NewTableOrView(viewName, this, query);

                            tableHash[view.Name] = view;
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
                        var columns = GetSchema(connection, "Columns");
                        foreach (var row in StiSchemaRow.All(columns))
                        {
                            if (IsSystemOwner(row.OWNER)) continue;

                            var tableName = StiTableName.GetName(row.OWNER, row.TABLE_NAME);
                            var column = new StiDataColumnSchema(row.COLUMN_NAME, GetNetType(row.DATATYPE));

                            var table = tableHash[tableName] as StiDataTableSchema;
                            if (table != null) 
                                table.Columns.Add(column);
                        }
                    }
                    catch
                    {
                    }
                    #endregion

                    #region Procedures
                    var procedures = GetSchema(connection, "Procedures");
                    var procedureHash = new Hashtable();

                    try
                    {

                        foreach (var row in StiSchemaRow.All(procedures))
                        {
                            if ((row.OWNER == "SYS") || IsSystemOwner(row.OWNER)) continue;

                            var procName = StiTableName.GetName(row.OWNER, row.OBJECT_NAME);
                            var query = StiTableQuery.Get(this).GetProcQuery(row.OWNER, row.OBJECT_NAME);

                            var proc = StiDataTableSchema.NewProcedure(procName, this, query);

                            procedureHash[proc.Name] = proc;
                            schema.StoredProcedures.Add(proc);
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
                return (int)StiDbType.Oracle.Date;

            if (type == typeof(TimeSpan)) 
                return (int)StiDbType.Oracle.TimeStamp;

            if (type == typeof(Int64))
                return (int)StiDbType.Oracle.Int64;

            if (type == typeof(Int32)) 
                return (int)StiDbType.Oracle.Int32;

            if (type == typeof(Int16)) 
                return (int)StiDbType.Oracle.Int16;

            if (type == typeof(Byte)) 
                return (int)StiDbType.Oracle.Byte;

            if (type == typeof(UInt64)) 
                return (int)StiDbType.Oracle.Int64;

            if (type == typeof(UInt32)) 
                return (int)StiDbType.Oracle.Int32;

            if (type == typeof(UInt16)) 
                return (int)StiDbType.Oracle.Int16;

            if (type == typeof(SByte)) 
                return (int)StiDbType.Oracle.Byte;

            if (type == typeof(Single)) 
                return (int)StiDbType.Oracle.Single;

            if (type == typeof(Double)) 
                return (int)StiDbType.Oracle.Double;

            if (type == typeof(Decimal)) 
                return (int)StiDbType.Oracle.Decimal;

            if (type == typeof(String)) 
                return (int)StiDbType.Oracle.Varchar2;

            if (type == typeof(Boolean)) 
                return (int)StiDbType.Oracle.Byte;

            if (type == typeof(Char)) 
                return (int)StiDbType.Oracle.Char;

            if (type == typeof(Byte[])) 
                return (int)StiDbType.Oracle.Blob;

            if (type == typeof(Guid)) 
                return (int)StiDbType.Oracle.Raw;

            return (int)StiDbType.Oracle.Int32;
        }

        /// <summary>
        /// Returns a .NET type from the specified string representaion of the database type.
        /// </summary>
        public override Type GetNetType(int dbType)
        {
            switch ((StiDbType.Oracle)dbType)
            {
                case StiDbType.Oracle.Date:
                    return typeof(byte[]);

                case StiDbType.Oracle.BFile:
                case StiDbType.Oracle.Blob:
                case StiDbType.Oracle.LongRaw:
                case StiDbType.Oracle.Raw:
                    return typeof(byte[]);

                case StiDbType.Oracle.Single:
                    return typeof(Single);

                case StiDbType.Oracle.Double:
                    return typeof(Double);

                case StiDbType.Oracle.Decimal:
                    return typeof(Decimal);

                case StiDbType.Oracle.Byte:
                    return typeof(Byte);

                case StiDbType.Oracle.Int16:
                    return typeof(Int16);

                case StiDbType.Oracle.Int32:
                    return typeof(Int32);

                case StiDbType.Oracle.Int64:
                case StiDbType.Oracle.Long:
                case StiDbType.Oracle.IntervalDS:
                case StiDbType.Oracle.IntervalYM:
                    return typeof(Int64);

                default:
                    return typeof(string);
            }
        }

        /// <summary>
        /// Returns a .NET type from the specified string representaion of the database type.
        /// </summary>
        public override Type GetNetType(string dbType)
        {
            switch (dbType.Replace(" ", "").ToUpperInvariant())
            {
                case "BFILE":
                case "BLOB":
                case "LONGRAW":
                case "RAW":
                    return typeof(byte[]);

                case "DATE":
                case "TIMESTAMP":
                case "TIMESTAMPWITHLOCALTIMEZONE":
                case "TIMESTAMPWITHTIMEZONE":
                    return typeof(DateTime);

                case "INTERVALDAYTOSECOND":
                    return typeof(TimeSpan);

                case "INTERVALYEARTOMONTH":
                    return typeof(Int64);

                case "NUMBER":
                case "BINARY_DOUBLE":
                case "BINARY_FLOAT":
                case "BINARY_INTEGER":
                    return typeof(Decimal);

                default:
                    return typeof(string);

            }
        }

        /// <summary>
        /// Returns sample of the connection string to this connector.
        /// </summary>
        public override string GetSampleConnectionString()
        {
            return StiDataOptions.SampleConnectionString.Oracle;
        }

        /// <summary>
        /// Bracketing string with specials characters
        /// </summary>
        /// <param name="name">unput string</param>
        /// <returns>Bracketed string</returns>
        public override string GetDatabaseSpecificName(string name)
        {
            return name;
        }
        #endregion

        #region Methods.Helper
        private bool IsSystemOwner(string owner)
        {
            owner = owner.ToUpper();

            switch (owner)
            {
                case "SYS":
                case "SYSMAN":
                case "SYSTEM":
                case "WMSYS":
                case "EXFSYS":
                case "ORDSYS":
                case "MDSYS":
                case "XDB":
                case "OUTLN":
                case "CTXMGR":
                case "OEMGR":
                    return true;

                default:
                    return false;
            }
        }
        #endregion

        #region Fields.Static
        private static object lockObject = new object();
        private static bool? isGeneric = null;
        private static bool? isManaged = null;
        private static bool? isDevart = null;
        private static bool? isClient = null;
        #endregion

        #region Methods.Static
        public static StiOracleConnector Get(string connectionString = null)
        {
            lock (lockObject)
            {
                if (isGeneric == true) 
                    return new StiOracleConnector(connectionString);

                if (isManaged == true)
                    return new StiOracleManagedConnector(connectionString);

                if (isDevart == true) 
                    return new StiOracleDevartConnector(connectionString);

                if (isClient == true) 
                    return new StiOracleClientConnector(connectionString);

                if (connectionString == null)
                    return new StiOracleConnector();

                if (isGeneric != true && isDevart != true && isClient != true && isManaged != true)
                {
                    isGeneric = null;
                    isManaged = null;
                    isDevart = null;
                    isClient = null;
                }

                if (isGeneric == null)
                {
                    var connector = LoadGenericConnector(connectionString);
                    if (isGeneric == true)
                        return connector;
                }

                if (isManaged == null)
                {
                    var connector = LoadManagedConnector(connectionString);
                    if (isManaged == true) 
                        return connector;
                }

                if (isDevart == null)
                {
                    var connector = LoadDevartConnector(connectionString);
                    if (isDevart == true)
                        return connector;
                }

                if (isClient == null && StiDataOptions.AllowUseOracleClientConnector)
                {
                    var connector = LoadClientConnector(connectionString);
                    if (isClient == true) 
                        return connector;
                }

                isGeneric = true;
                return new StiOracleConnector(connectionString);
            }
        }

        public static StiOracleClientConnector LoadClientConnector(string connectionString = null)
        {
            var connector = new StiOracleClientConnector(connectionString);
            isClient = connector.IsAvailable;
            return connector;
        }

        public static StiOracleDevartConnector LoadDevartConnector(string connectionString = null)
        {
            var connector = new StiOracleDevartConnector(connectionString);
            isDevart = connector.IsAvailable;
            return connector;
        }

        public static StiOracleManagedConnector LoadManagedConnector(string connectionString = null)
        {
            var connector = new StiOracleManagedConnector(connectionString);
            isManaged = connector.IsAvailable;
            return connector;
        }

        public static StiOracleConnector LoadGenericConnector(string connectionString = null)
        {
            var connector = new StiOracleConnector(connectionString);
            isGeneric = connector.IsAvailable;
            return connector;
        }
        #endregion

        public StiOracleConnector(string connectionString = null)
            : base(connectionString)
        {
            this.NameAssembly = "Oracle.DataAccess.dll";
            this.TypeConnection = "Oracle.DataAccess.Client.OracleConnection";
            this.TypeDataAdapter = "Oracle.DataAccess.Client.OracleDataAdapter";
            this.TypeCommand = "Oracle.DataAccess.Client.OracleCommand";
            this.TypeParameter = "Oracle.DataAccess.Client.OracleParameter";
            this.TypeDbType = "Oracle.DataAccess.Client.OracleDbType";
            this.TypeCommandBuilder = "Oracle.DataAccess.Client.OracleCommandBuilder";
            this.TypeConnectionStringBuilder = "Oracle.DataAccess.Client.OracleConnectionStringBuilder";
            this.TypeDataSourceEnumerator = "Oracle.DataAccess.Client.OracleDataSourceEnumerator";
        }
    }
}