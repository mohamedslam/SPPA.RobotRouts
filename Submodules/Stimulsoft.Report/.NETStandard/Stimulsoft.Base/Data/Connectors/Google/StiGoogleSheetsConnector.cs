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
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Stimulsoft.Base.Data.Connectors.Google
{
    public class StiGoogleSheetsConnector : StiDataConnector
    {
        #region Fields
        private StiDataAssemblyHelper assemblyHelper;
        private object googleSheetsProvider;
        #endregion

        #region Properties
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string SpreadsheetId { get; set; }

        public bool FirstRowIsHeader { get; set; }

        /// <summary>
        /// Gets a type of the connection helper.
        /// </summary>
        public override StiConnectionIdent ConnectionIdent => StiConnectionIdent.GoogleSheetsStorage;

        /// <summary>
        /// Gets an order of the connector.
        /// </summary>
        public override StiConnectionOrder ConnectionOrder => StiConnectionOrder.GoogleSheetsStorage;

        public override string Name => "Google Sheets";

        /// <summary>
        /// Get a value which indicates that this data connector can be used now.
        /// </summary>
        public override bool IsAvailable
        {
            get
            {
                var assemblyHelper = GetAssemblyData();
                return assemblyHelper != null && assemblyHelper.IsAllowed;
            }
        }

        /// <summary>
        /// Gets the package identificator for this connector.
        /// </summary>
        public override string[] NuGetPackages => new string[] { "Stimulsoft.Data.GoogleSheets" };

        /// <summary>
        /// Gets the package version for this connector.
        /// </summary>
        public override string NuGetVersion => "2022.4.1";
        #endregion

        #region Methods
        private StiDataAssemblyHelper GetAssemblyData()
        {
            if (assemblyHelper != null) 
                return assemblyHelper;

            assemblyHelper = new StiDataAssemblyHelper("Stimulsoft.Data.GoogleSheets.dll", $"Stimulsoft.Data.GoogleSheets-{this.NuGetVersion}");
            return assemblyHelper;
        }

        private object GetGoogleSheetsProvider()
        {
            if (this.googleSheetsProvider != null)
                return this.googleSheetsProvider;

            var assemblyBaseData = GetAssemblyData();
            this.googleSheetsProvider = assemblyBaseData.CreateObject("Stimulsoft.Data.GoogleSheets.StiGoogleSheetsProvider", 
                new object[] { this.ClientId, this.ClientSecret, this.SpreadsheetId, this.FirstRowIsHeader });

            return googleSheetsProvider;
        }

        public StiDataSchema RetrieveSchema(bool allowException = false)
        {
            var schema = new StiDataSchema(this.ConnectionIdent);

            try
            {
                var provider = GetGoogleSheetsProvider();
                var method = provider.GetType().GetMethod("GetTableNames");
                var tableNames = method.Invoke(provider, null) as List<string>;

                foreach (var name in tableNames)
                {
                    var tableSchema = StiDataTableSchema.NewTable(name);
                    try
                    {
                        var columns = GetColumns(name);
                        if (columns != null)
                            tableSchema.Columns = columns;
                    }
                    catch
                    {
                    }

                    schema.Tables.Add(tableSchema);
                }

                return schema.Sort();
            }
            catch (Exception)
            {
                if (allowException) throw;

                return null;
            }
        }

        public DataTable GetDataTable(string collectionName, string query, int? index = null, int? count = null)
        {
            try
            {
                var provider = GetGoogleSheetsProvider();

                var method = provider.GetType().GetMethod("GetDataTable");
                var dataTable = method.Invoke(provider, new object[] { collectionName, index, count }) as DataTable;

                return dataTable;
            }
            catch (Exception exception)
            {
                if (exception.InnerException != null)
                    throw exception.InnerException;
                else
                    throw;
            }
        }

        public List<StiDataColumnSchema> GetColumns(string collectionName)
        {
            var dataTable = GetDataTable(collectionName, null, 0, 5);
            return dataTable != null 
                ? dataTable.Columns.Cast<DataColumn>().ToList().Select(c => new StiDataColumnSchema(c.ColumnName, c.DataType)).ToList() : null;
        }
        #endregion

        #region Methods.Static
        public static StiGoogleSheetsConnector Get(string clientId, string clientSecret, string spreadsheetId, bool firstRowIsHeader)
        {
            return new StiGoogleSheetsConnector
            {
                ClientId = clientId,
                ClientSecret = clientSecret,
                SpreadsheetId = spreadsheetId,
                FirstRowIsHeader = firstRowIsHeader
            };
        }
        #endregion

        public StiGoogleSheetsConnector()
        {
        }
    }
}
