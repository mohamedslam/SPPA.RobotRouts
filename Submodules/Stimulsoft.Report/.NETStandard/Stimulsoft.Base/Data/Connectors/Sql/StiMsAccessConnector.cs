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
using System.Data;
using System.Data.OleDb;

#if BLAZER
using Stimulsoft.Data;
#endif

namespace Stimulsoft.Base
{
    public class StiMsAccessConnector : StiOleDbConnector
    {
        #region Properties
        /// <summary>
        /// Gets a type of the connection helper.
        /// </summary>
        public override StiConnectionIdent ConnectionIdent => StiConnectionIdent.MsAccessDataSource;

        /// <summary>
        /// Gets an order of the connector.
        /// </summary>
        public override StiConnectionOrder ConnectionOrder => StiConnectionOrder.MsAccessDataSource;

        public override string Name => "MS Access";

        /// <summary>
        /// Get a value which indicates that this data connector can be used now.
        /// </summary>
        public override bool IsAvailable => true;
        #endregion

        #region Methods
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
                    var tableHash = new Hashtable();
                    try
                    {
                        var tables = GetSchema(connection, "Tables");
                        foreach (var row in StiSchemaRow.All(tables))
                        {
                            if (row.TABLE_SCHEMA == "sys") continue;

                            var table = StiDataTableSchema.NewTableOrView(row.TABLE_NAME);
                            tableHash[table.Name] = table;

                            if (row.TABLE_TYPE == "VIEW")
                                schema.Views.Add(table);

                            if (row.TABLE_TYPE == "TABLE")
                                schema.Tables.Add(table);
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
                            if (row.TABLE_SCHEMA == "sys") continue;

                            var column = new StiDataColumnSchema(row.COLUMN_NAME, GetNetType(row.DATA_TYPE_INT));

                            var table = tableHash[row.TABLE_NAME] as StiDataTableSchema;
                            if (table != null)
                                table.Columns.Add(column);
                        }
                    }
                    catch
                    {
                    }
                    #endregion

                    #region Procedures
                    var procedureHash = new Hashtable();

                    try
                    {
                        var procedures = GetSchema(connection, "Procedures");
                        foreach (var row in StiSchemaRow.All(procedures))
                        {
                            if (row.PROCEDURE_SCHEMA == "sys") continue;

                            var procName = row.PROCEDURE_NAME;
                            if (procName.IndexOf(";", StringComparison.InvariantCulture) != -1) 
                                procName = procName.Substring(0, procName.IndexOf(";", StringComparison.InvariantCulture));

                            var procedure = StiDataTableSchema.NewProcedure(procName);

                            procedureHash[procedure.Name] = procedure;
                            schema.StoredProcedures.Add(procedure);
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
        /// Returns sample of the connection string to this connector.
        /// </summary>
        public override string GetSampleConnectionString()
        {
            return StiDataOptions.SampleConnectionString.MsAccess;
        }

        /// <summary>
        /// Returns the type of the DBType.
        /// </summary>
        public override Type GetDbType()
        {
            return typeof(OleDbType);
        }
        #endregion

        #region Methods.Static
        public static new StiMsAccessConnector Get(string connectionString = null)
        {
            return new StiMsAccessConnector(connectionString);
        }
        #endregion

        protected internal StiMsAccessConnector(string connectionString = null)
            : base(connectionString)
        {
            this.NameAssembly = "System.Data.OleDb.dll";
            this.TypeConnection = typeof(OleDbConnection).ToString();
            this.TypeDataAdapter = typeof(OleDbDataAdapter).ToString();
            this.TypeCommand = typeof(OleDbCommand).ToString();
            this.TypeParameter = typeof(OleDbParameter).ToString();
            this.TypeCommandBuilder = typeof(OleDbCommandBuilder).ToString();
        }
    }
}
