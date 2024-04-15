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
using System.Threading.Tasks;

namespace Stimulsoft.Base.Data.Connectors.Google
{
    public class StiGoogleAnalyticsConnector : StiDbNoSqlDataConnector
    {
        #region Fields
        private StiDataAssemblyHelper assemblyHelper;
        private object googleAnalyticsProvider;
        #endregion

        #region Properties
        /// <summary>
        /// Gets a type of the connection helper.
        /// </summary>
        public override StiConnectionIdent ConnectionIdent => StiConnectionIdent.GoogleAnalyticsDataSource;

        /// <summary>
        /// Gets an order of the connector.
        /// </summary>
        public override StiConnectionOrder ConnectionOrder => StiConnectionOrder.GoogleAnalyticsDataSource;

        public override string Name => "Google Analytics";

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
        public override string[] NuGetPackages => new string[] { "Stimulsoft.Data.GoogleAnalytics" };

        /// <summary>
        /// Gets the package version for this connector.
        /// </summary>
        public override string NuGetVersion => "2021.4.3";
        #endregion

        #region Methods
        private StiDataAssemblyHelper GetAssemblyData()
        {
            if (assemblyHelper != null)
                return assemblyHelper;

            assemblyHelper = new StiDataAssemblyHelper("Stimulsoft.Data.GoogleAnalytics.dll", $"Stimulsoft.Data.GoogleAnalytics-{this.NuGetVersion}");
            return assemblyHelper;
        }

        private object GetGoogleAnalyticsProvider()
        {
            if (this.googleAnalyticsProvider != null)
                return this.googleAnalyticsProvider;

            var assemblyBaseData = GetAssemblyData();
            this.googleAnalyticsProvider = assemblyBaseData.CreateObject("Stimulsoft.Data.GoogleAnalytics.StiGoogleAnalyticsProvider", new object[] { this.ConnectionString });

            return googleAnalyticsProvider;
        }

        private object GetStiGoogleAnalyticsReportingProviderr()
        {
            if (this.googleAnalyticsProvider != null)
                return this.googleAnalyticsProvider;

            var assemblyBaseData = GetAssemblyData();
            this.googleAnalyticsProvider = assemblyBaseData.CreateObject("Stimulsoft.Data.GoogleAnalytics.StiGoogleAnalyticsReportingProvider", new object[] { this.ConnectionString });

            return googleAnalyticsProvider;
        }

        internal async Task<List<KeyValuePair<string, string>>> GetAccountsAsync()
        {
            return await Task.Run(() => GetAccounts());
        }

        internal List<KeyValuePair<string, string>> GetAccounts()
        {
            var provider = GetGoogleAnalyticsProvider();

            var method = provider.GetType().GetMethod("GetAccounts");
            return (List<KeyValuePair<string, string>>)method.Invoke(provider, null);
        }

        internal async Task<List<KeyValuePair<string, string>>> GetPropertiesAsync(string accountId)
        {
            return await Task.Run(() => GetProperties(accountId));
        }

        internal List<KeyValuePair<string, string>> GetProperties(string accountId)
        {
            var provider = GetGoogleAnalyticsProvider();

            var method = provider.GetType().GetMethod("GetProperties");
            return (List<KeyValuePair<string, string>>)method.Invoke(provider, new object[] { accountId });
        }

        internal async Task<List<KeyValuePair<string, string>>> GetViewsAsync(string accountId, string propertyId)
        {
            return await Task.Run(() => GetViews(accountId, propertyId));
        }

        internal List<KeyValuePair<string, string>> GetViews(string accountId, string propertyId)
        {
            var provider = GetGoogleAnalyticsProvider();

            var method = provider.GetType().GetMethod("GetViews");
            return (List<KeyValuePair<string, string>>)method.Invoke(provider, new object[] { accountId, propertyId });
        }

        internal List<KeyValuePair<string, Dictionary<string, string>>> GetMetrics()
        {
            var provider = GetGoogleAnalyticsProvider();

            var method = provider.GetType().GetMethod("GetAllMetrics");
            return (List<KeyValuePair<string, Dictionary<string, string>>>)method.Invoke(provider, null);
        }

        internal List<KeyValuePair<string, Dictionary<string, string>>> GetDimensions()
        {
            var provider = GetGoogleAnalyticsProvider();

            var method = provider.GetType().GetMethod("GetAllDimensions");
            return (List<KeyValuePair<string, Dictionary<string, string>>>)method.Invoke(provider, null);
        }

        public override StiTestConnectionResult TestConnection()
        {
            try
            {
                var provider = GetGoogleAnalyticsProvider();

                var method = provider.GetType().GetMethod("IsConnectionAvailable");
                var isConnectionAvailable = (bool)method.Invoke(provider, null);

                return isConnectionAvailable
                    ? StiTestConnectionResult.MakeFine()
                    : StiTestConnectionResult.MakeWrong("Couldn't open connection to server");
            }
            catch (Exception exception)
            {
                if (exception.InnerException != null)
                    return StiTestConnectionResult.MakeWrong(exception.InnerException.Message);
                else
                    return StiTestConnectionResult.MakeWrong(exception.Message);
            }
        }

        public override StiDataSchema RetrieveSchema(bool allowException = false)
        {
            DataSet emptyDatasetOfSameSchema;
            try
            {
                var provider = GetStiGoogleAnalyticsReportingProviderr();

                var method = provider.GetType().GetMethod("GetEmptyDatasetOfSameSchema");
                emptyDatasetOfSameSchema = method.Invoke(provider, null) as DataSet;
            }
            catch (Exception exception)
            {
                if (exception.InnerException != null)
                    throw exception.InnerException;
                else
                    throw;
            }

            var schema = new StiDataSchema(this.ConnectionIdent);

            #region Tables
            foreach (DataTable table in emptyDatasetOfSameSchema.Tables)
            {
                var tableSchema = StiDataTableSchema.NewTable(table.TableName);

                foreach (DataColumn column in table.Columns)
                {
                    tableSchema.Columns.Add(new StiDataColumnSchema
                    {
                        Name = column.ColumnName,
                        Type = column.DataType
                    });
                }
                schema.Tables.Add(tableSchema);
            }
            #endregion

            schema.Tables = schema.Tables.OrderBy(e => e.Name).ToList();

            return schema;
        }

        public override string GetSampleConnectionString()
        {
            return StiDataOptions.SampleConnectionString.GoogleAnalytics;
        }

        public override List<StiDataColumnSchema> GetColumns(string collectionName)
        {
            var dataTable = RetrieveSchema().Tables.FirstOrDefault(table => table.Name.ToLowerInvariant() == collectionName.ToLowerInvariant());

            return dataTable?.Columns;
        }

        public override DataTable GetDataTable(string collectionName, string query, int? index = null, int? count = null)
        {
            DataTable table;

            try
            {
                var provider = GetStiGoogleAnalyticsReportingProviderr();

                var method = provider.GetType().GetMethod("GetTable");
                table = method.Invoke(provider, new object[] { collectionName, query, index, count }) as DataTable;

                return table;
            }
            catch (Exception exception)
            {
                if (exception.InnerException != null)
                    throw exception.InnerException;
                else
                    throw;
            }
        }
        #endregion

        #region Methods.Static
        public static StiGoogleAnalyticsConnector Get(string connectionString = null)
        {
            return new StiGoogleAnalyticsConnector(connectionString);
        }
        #endregion

        public StiGoogleAnalyticsConnector(string connectionString = null)
            : base(connectionString)
        {
        }
    }
}
