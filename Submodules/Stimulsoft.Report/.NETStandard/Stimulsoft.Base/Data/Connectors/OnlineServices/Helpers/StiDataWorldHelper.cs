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

using Stimulsoft.Base.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;

namespace Stimulsoft.Base.Data.Connectors.OnlineServices.Helpers
{
    public class StiDataWorldHelper
    {
        #region Fields
        private string urlBase = "https://api.data.world/v0/";
        #endregion

        #region Methods
        private WebClient GetDefaultWebClient()
        {
            var client = new WebClient();
            client.Proxy = StiProxy.GetProxy();
            client.Encoding = StiBaseOptions.WebClientEncoding;
            client.Headers["Authorization"] = "Bearer " + this.Token;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            return client;
        }

        public List<string> GetTableNames()
        {
            using (var client = GetDefaultWebClient())
            {
                var query = Uri.EscapeDataString($"SELECT * FROM Tables");
                var urlDatasets = $"sql/{this.Owner.ToLower()}/{this.Database.ToLower()}?query={query}";
                var url = new Uri($"{this.urlBase}{urlDatasets}");

                var json = client.DownloadString(url);
                var tables = JsonConvert.DeserializeObject<List<StiDataWorldMetadataTable>>(json);
                return tables.Where(t => t.Dataset.ToLower() == this.Database.ToLower()).Select(t => t.TableId).ToList();
            }
        }

        public List<StiDataColumnSchema> GetColumns(string collectionName)
        {
            var dataTable = GetDataTable(collectionName, $"select * from {collectionName} limit 29");
            return dataTable != null
                ? dataTable.Columns.Cast<DataColumn>().ToList().Select(c => new StiDataColumnSchema(c.ColumnName, c.DataType)).ToList() : null;
        }

        public DataTable GetDataTable(string collectionName, string query)
        {
            try
            {
                using (var client = GetDefaultWebClient())
                {
                    var urlSql = $"sql/{this.Owner.ToLower()}/{this.Database.ToLower()}";

                    if (string.IsNullOrEmpty(query))
                        query = Uri.EscapeDataString($"select * from {collectionName}");

                    var url = new Uri($"{this.urlBase}{urlSql}?query={query}");

                    var content = client.DownloadString(url);

                    var dataSet = StiBaseOptions.DefaultJsonConverterVersion == StiJsonConverterVersion.ConverterV2
                    ? StiJsonToDataSetConverterV2.GetDataSet(content)
                    : StiJsonToDataSetConverter.GetDataSet(content);

                    return dataSet != null && dataSet.Tables.Count > 0 ? dataSet.Tables[0].Copy() : null;
                }
            }
            catch (Exception exception)
            {
                if (exception.InnerException != null)
                    throw exception.InnerException;
                else
                    throw;
            }
        }

        internal StiTestConnectionResult TestConnection()
        {
            try
            {
                using (var client = GetDefaultWebClient())
                {
                    var url = new Uri($"{this.urlBase}users/{this.Owner.ToLower()}");
                    var content = client.DownloadString(url);

                    return StiTestConnectionResult.MakeFine();
                }
            }
            catch (Exception exception)
            {
                if (exception.InnerException != null)
                    return StiTestConnectionResult.MakeWrong(exception.InnerException.Message);
                else
                    return StiTestConnectionResult.MakeWrong(exception.Message);
            }
        }

        public StiDataSchema RetrieveSchema(bool allowException = false)
        {
            if (string.IsNullOrEmpty(this.ConnectionString))
                return null;

            var schema = new StiDataSchema(StiConnectionIdent.DataWorldDataSource);
            try
            {
                var tableNames = GetTableNames();

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

        #endregion

        #region Properties
        public string ConnectionString { get; }

        public string Owner => StiConnectionStringHelper.GetConnectionStringKey(ConnectionString, "Owner");

        public string Token => StiConnectionStringHelper.GetConnectionStringKey(ConnectionString, "Token");

        public string Database => StiConnectionStringHelper.GetConnectionStringKey(ConnectionString, "Database");
        #endregion

        public StiDataWorldHelper(string connectionString)
        {
            this.ConnectionString = connectionString;
        }
    }
}
