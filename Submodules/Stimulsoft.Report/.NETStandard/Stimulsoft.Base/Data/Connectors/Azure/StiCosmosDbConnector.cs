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

using Stimulsoft.Base.Data.Connectors.Azure.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace Stimulsoft.Base.Data.Connectors.Azure
{
    public class StiCosmosDbConnector : StiDbNoSqlDataConnector
    {
        #region Properties
        /// <summary>
        /// Gets a type of the connection helper.
        /// </summary>
        public override StiConnectionIdent ConnectionIdent => StiConnectionIdent.CosmosDbDataSource;

        /// <summary>
        /// Gets an order of the connector.
        /// </summary>
        public override StiConnectionOrder ConnectionOrder => StiConnectionOrder.CosmosDbDataSource;

        public override string Name => "Cosmos DB";

        public override bool AllowTestConnection
        {
            get
            {
                try
                {
                    var provider = GetCosmosDbProvider();
                    var method = provider.GetType().GetMethod("AllowTestConnection");
                    return (bool)method.Invoke(provider, null);
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets the package identificator for this connector.
        /// </summary>
        public override string[] NuGetPackages => new string[] { "Stimulsoft.Data.CosmosDB" };

        /// <summary>
        /// Gets the package version for this connector.
        /// </summary>
        public override string NuGetVersion => "2021.4.4";

        public StiCosmosDbApi Api { get; }
        #endregion

        #region Methods
        private object GetCosmosDbProvider()
        {
            switch (this.Api)
            {
                case StiCosmosDbApi.SQL:
                    return new StiCosmosSqlApiHelper(this.ConnectionString, this.NuGetVersion).GetCosmosDbProvider();

                default:
                    return new StiCosmosMongoDbApiHelper(this.ConnectionString, this.NuGetVersion).GetCosmosDbProvider();
            }
        }

        public override StiTestConnectionResult TestConnection()
        {
            switch (this.Api)
            {
                case StiCosmosDbApi.SQL:
                    return new StiCosmosSqlApiHelper(this.ConnectionString, this.NuGetVersion).TestConnection();

                default:
                    return new StiCosmosMongoDbApiHelper(this.ConnectionString, this.NuGetVersion).TestConnection();
            }
        }

        public override StiDataSchema RetrieveSchema(bool allowException = false)
        {
            switch (this.Api)
            {
                case StiCosmosDbApi.SQL:
                    return new StiCosmosSqlApiHelper(this.ConnectionString, this.NuGetVersion).RetrieveSchema();

                default:
                    return new StiCosmosMongoDbApiHelper(this.ConnectionString, this.NuGetVersion).RetrieveSchema(allowException);
            }
        }

        public override string GetSampleConnectionString()
        {
            switch (this.Api)
            {
                case StiCosmosDbApi.SQL:
                     return StiCosmosSqlApiHelper.GetSampleConnectionString();

                default:
                    return StiCosmosMongoDbApiHelper.GetSampleConnectionString();
            }
        }

        public override List<StiDataColumnSchema> GetColumns(string collectionName)
        {
            switch (this.Api)
            {
                case StiCosmosDbApi.SQL:
                    return new StiCosmosSqlApiHelper(this.ConnectionString, this.NuGetVersion).GetColumns(collectionName);

                default:
                    return new StiCosmosMongoDbApiHelper(this.ConnectionString, this.NuGetVersion).GetColumns(collectionName);
            }
        }

        public override DataTable GetDataTable(string collectionName, string query, int? index = null, int? count = null)
        {
            switch (this.Api)
            {
                case StiCosmosDbApi.SQL:
                    return new StiCosmosSqlApiHelper(this.ConnectionString, this.NuGetVersion).GetDataTable(collectionName, query, index, count);

                default:
                    return new StiCosmosMongoDbApiHelper(this.ConnectionString, this.NuGetVersion).GetDataTable(collectionName, query, index, count);
            }
        }

        public string ConvertDateTimeToJsonStr(DateTime date)
        {
            try
            {
                var provider = GetCosmosDbProvider();
                var method = provider.GetType().GetMethod("ConvertDateTimeToJsonStr");
                return (string)method.Invoke(provider, new object[] { date });
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region Methods.Static
        public static StiCosmosDbConnector Get(string connectionString = null)
        {
            StiCosmosDbApi api;
            Enum.TryParse(StiConnectionStringHelper.GetConnectionStringKey(connectionString, "Api"), true, out api);

            return new StiCosmosDbConnector(connectionString, api);
        }
        #endregion

        public StiCosmosDbConnector(string connectionString, StiCosmosDbApi api)
            : base(connectionString)
        {
            this.FolderAssembly = $"Stimulsoft.Data.CosmosDB-{this.NuGetVersion}";
            this.NameAssembly = "Stimulsoft.Data.CosmosDB.dll";
            this.Api = api;
        }
    }
}
