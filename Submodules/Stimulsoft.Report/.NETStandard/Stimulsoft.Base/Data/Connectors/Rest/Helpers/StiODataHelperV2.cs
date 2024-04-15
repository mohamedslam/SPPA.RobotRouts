#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports 									            }
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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Net;

namespace Stimulsoft.Base.Data.Connectors.Rest.Helpers
{
    public class StiODataHelperV2
    {
        #region Fields
        private StiDataAssemblyHelper assemblyHelper;
        private object odataProvider;
        private object model;
        private string bearerAccessToken;
        private string nuGetVersion;
        #endregion

        #region Methods
        private StiDataAssemblyHelper GetAssemblyBaseData()
        {
            if (assemblyHelper != null)
                return assemblyHelper;

            assemblyHelper = new StiDataAssemblyHelper("Stimulsoft.Data.OData.dll", $"Stimulsoft.Data.OData-{this.nuGetVersion}");
            return assemblyHelper;
        }

        private object GetODataProvider()
        {
            if (this.odataProvider != null)
                return this.odataProvider;

            if (!string.IsNullOrWhiteSpace(this.Token))
            {
                this.bearerAccessToken = this.Token;
            }
            else if (!string.IsNullOrWhiteSpace(this.AddressBearer) && string.IsNullOrWhiteSpace(this.bearerAccessToken))
            {
                this.bearerAccessToken = StiODataHelper.GetBearerAccessToken(this.AddressBearer, this.UserName, this.Password, this.ClientId);
            }

            var connectionString = string.IsNullOrWhiteSpace(this.bearerAccessToken)
                ? this.ConnectionString
                : $"Address={this.Address};BearerAccessToken={this.bearerAccessToken}";

            var assemblyBaseData = GetAssemblyBaseData();
            this.odataProvider = assemblyBaseData.CreateObject("Stimulsoft.Data.OData.StiODataProvider", new object[] { connectionString, this.CookieContainer, this.Headers, this.AllowException }); ;

            return odataProvider;
        }

        private object GetEdmModel()
        {
            if (this.model != null)
                return this.model;

            var odataProvider = GetODataProvider();
            var method = odataProvider.GetType().GetMethod("GetEdmModel");
            this.model = method.Invoke(odataProvider, null);
            return this.model;
        }

        private List<string> GetTableNames()
        {
            var model = GetEdmModel();

            var provider = GetODataProvider();
            var method = provider.GetType().GetMethod("GetTableNames");
            var names = method.Invoke(provider, new object[] { model });
            return names as List<string>;
        }

        public StiDataSchema RetrieveSchema()
        {
            var schema = new StiDataSchema(StiConnectionIdent.ODataDataSource);

            var model = GetEdmModel();
            var tableNames = GetTableNames();

            foreach (var tableName in tableNames)
            {
                var table = new StiDataTableSchema(tableName, tableName);

                var provider = GetODataProvider();
                var method = provider.GetType().GetMethod("GetColumnsInfo");
                var columnsInfo = method.Invoke(provider, new object[] { model, tableName }) as List<KeyValuePair<string, string>>;

                foreach (KeyValuePair<string, string> columnInfo in columnsInfo)
                {
                    var type = StiODataHelper.GetNetType(columnInfo.Value);

                    var column = new StiDataColumnSchema(columnInfo.Key, type);
                    table.Columns.Add(column);
                }

                schema.Tables.Add(table);
            }

            return schema;
        }

        public void FillDataTable(DataTable table, string query)
        {
            try
            {
                var func = new Func<object, Type, object>(StiConvert.ChangeType);

                var provider = GetODataProvider();
                var method = provider.GetType().GetMethod("FillDataTable");
                method.Invoke(provider, new object[] { table, query, func });
            }
            catch
            {
                if (AllowException)
                    throw;
            }
        }

        public List<StiDataColumnSchema> GetColumns(string tableName)
        {
            var columns = new List<StiDataColumnSchema>();

            var model = GetEdmModel();

            var provider = GetODataProvider();
            var method = provider.GetType().GetMethod("GetColumnsInfo");
            var columnsInfo = method.Invoke(provider, new object[] { model, tableName }) as List<KeyValuePair<string, string>>;

            foreach (KeyValuePair<string, string> columnInfo in columnsInfo)
            {
                var type = StiODataHelper.GetNetType(columnInfo.Value);

                var column = new StiDataColumnSchema(columnInfo.Key, type);
                columns.Add(column);
            }

            return columns;
        }
        #endregion

        #region Properties
        public CookieContainer CookieContainer { get; set; }

        public string ConnectionString { get; private set; }

        public string Address
        {
            get
            {
                var address = StiConnectionStringHelper.GetConnectionStringKey(ConnectionString, "Address=")
                    ?? StiConnectionStringHelper.GetConnectionStringKey(ConnectionString);

                return address ?? ConnectionString;
            }
        }

        public string UserName => StiConnectionStringHelper.GetConnectionStringKey(ConnectionString, "UserName");

        public string Password => StiConnectionStringHelper.GetConnectionStringKey(ConnectionString, "Password");

        public string ClientId => StiConnectionStringHelper.GetConnectionStringKey(ConnectionString, "Client_Id");

        public string AddressBearer => StiConnectionStringHelper.GetConnectionStringKey(ConnectionString, "AddressBearer");

        public string Token => StiConnectionStringHelper.GetConnectionStringKey(ConnectionString, "Token");

        public NameValueCollection Headers { get; protected set; }

        /// <summary>
        /// Allows exceptions from the retrieving schema methods and data.
        /// </summary>
        [Browsable(false)]
        public bool AllowException { get; set; }
        #endregion

        public StiODataHelperV2(string connectionString, string nuGetVersion, CookieContainer cookieContainer, NameValueCollection headers = null)
        {
            this.nuGetVersion = nuGetVersion;
            this.CookieContainer = cookieContainer;
            this.ConnectionString = connectionString.Replace("\r\n", "").Replace(" ", "");
            this.Headers = headers ?? new NameValueCollection();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        }
    }
}
