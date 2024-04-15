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

using Stimulsoft.Base.Data.Connectors;
using Stimulsoft.Base.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Text;

namespace Stimulsoft.Base
{
    public class StiQuickBooksConnector : 
        StiRestDataConnector, 
        IDbConnection
    {
        #region Fields
        private string stimulsoftClientId = "ABPQz3WAKlBBQy07UBdcwkbeAZGG10hu9EpukpQinSUVXOM2iu";
        private string stimulsoftClientSecret = "DAAaTBmTphB0fJfsjdPw2TlzEaWbZyYrRGOA6PMV";
        private string oauth2Url = "https://appcenter.intuit.com/connect/oauth2";
        private string bearerUrl = "https://oauth.platform.intuit.com/oauth2/v1/tokens/bearer";
        private string baseUrl = "https://quickbooks.api.intuit.com/v3";
        //private string baseUrl = "https://sandbox-quickbooks.api.intuit.com/v3";
        private string stimulsoftRedirectUrl = "https://developer.intuit.com/v2/OAuth2Playground/RedirectUrl";
        //private string stimulsoftRedirectUrl = "http://localhost:58467/v2/OAuth2Playground/RedirectUrl";
        private string responseType = "code";
        private string scope = "com.intuit.quickbooks.accounting";
        private string state = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8);
        #endregion

        #region Properties
        /// <summary>
        /// Gets a type of the connection helper.
        /// </summary>
        public override StiConnectionIdent ConnectionIdent => StiConnectionIdent.QuickBooksDataSource;

        /// <summary>
        /// Gets an order of the connector.
        /// </summary>
        public override StiConnectionOrder ConnectionOrder => StiConnectionOrder.QuickBooksDataSource;

        public override string Name => "QuickBooks";

        public override bool IsAvailable => true;

        public bool UseApp
        {
            get
            {
                bool useApp;

                if (Boolean.TryParse(StiConnectionStringHelper.GetConnectionStringKey(ConnectionString, "UseApp"), out useApp))
                    return useApp;

                return false;
            }
            set
            {
                ConnectionString = StiConnectionStringHelper.SetConnectionStringKey(ConnectionString, "UseApp", value.ToString());
            }
        }

        public string ClientId
        {
            get
            {
                return StiConnectionStringHelper.GetConnectionStringKey(ConnectionString, "ClientId");
            }
            set
            {
                ConnectionString = StiConnectionStringHelper.SetConnectionStringKey(ConnectionString, "ClientId", value);
            }
        }

        public string ClientIdPrivate
        {
            get
            {
                if (UseApp) 
                    return ClientId;

                return stimulsoftClientId;
            }
            set
            {
                ClientId = value;
            }
        }

        public string ClientSecret
        {
            get
            {
                return StiConnectionStringHelper.GetConnectionStringKey(ConnectionString, "ClientSecret");
            }
            set
            {
                ConnectionString = StiConnectionStringHelper.SetConnectionStringKey(ConnectionString, "ClientSecret", value);
            }
        }

        private string ClientSecretPrivate
        {
            get
            {
                if (UseApp)
                    return ClientSecret;

                return stimulsoftClientSecret;
            }
            set
            {
                ClientSecret = value;
            }
        }

        public string RedirectURL
        {
            get
            {
                return StiConnectionStringHelper.GetConnectionStringKey(ConnectionString, "RedirectURL");
            }
            set
            {
                ConnectionString = StiConnectionStringHelper.SetConnectionStringKey(ConnectionString, "RedirectURL", value);
            }
        }

        private string RedirectURLPrivate
        {
            get
            {
                if (UseApp) 
                    return RedirectURL;

                return stimulsoftRedirectUrl;
            }
            set
            {
                RedirectURL = value;
            }
        }

        public string AuthorizationCode
        {
            get
            {
                return StiConnectionStringHelper.GetConnectionStringKey(ConnectionString, "AuthorizationCode");
            }
            set
            {
                ConnectionString = StiConnectionStringHelper.SetConnectionStringKey(ConnectionString, "AuthorizationCode", value);
            }
        }

        public string RealmId
        {
            get
            {
                return StiConnectionStringHelper.GetConnectionStringKey(ConnectionString, "RealmId");
            }
            set
            {
                ConnectionString = StiConnectionStringHelper.SetConnectionStringKey(ConnectionString, "RealmId", value);
            }
        }

        public string AccessToken
        {
            get
            {
                return StiConnectionStringHelper.GetConnectionStringKey(ConnectionString, "AccessToken");
            }
            set
            {
                ConnectionString = StiConnectionStringHelper.SetConnectionStringKey(ConnectionString, "AccessToken", value);
            }
        }

        public string RefreshToken
        {
            get
            {
                return StiConnectionStringHelper.GetConnectionStringKey(ConnectionString, "RefreshToken");
            }
            set
            {
                ConnectionString = StiConnectionStringHelper.SetConnectionStringKey(ConnectionString, "RefreshToken", value);
            }
        }

        public int ConnectionTimeout => 30;

        public string Database => throw new NotImplementedException();

        public ConnectionState State => ConnectionState.Closed;
        #endregion

        #region Methods
        public void FillAuthorizationCode()
        {
            var helper = new StiDataAssemblyHelper("Stimulsoft.Report.Win.dll");
            var oAuthForm = helper.CreateObject(
                "Stimulsoft.Report.Win.Dictionary.StiQuickBooksOAuthForm", GetAuthorizationUrl(), RedirectURLPrivate) as IStiQuickBooksOAuthForm;
            
            if (oAuthForm.ShowDialogForm())
            {
                RealmId = oAuthForm.RealmId;
                AuthorizationCode = oAuthForm.AuthorizationCode;
            }
            else if (!string.IsNullOrEmpty(oAuthForm.Error))
            {
                throw new AuthenticationException("Authentification Error");
            }
        }

        private WebClient GetDefaultWebClient()
        {
            var client = new WebClient();
            client.Proxy = StiProxy.GetProxy();
            client.Encoding = StiBaseOptions.WebClientEncoding;
            client.Headers["Authorization"] = "Bearer " + this.AccessToken;
            client.Headers["Accept"] = "application/json";

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            return client;
        }

        private string GetAuthorizationUrl()
        {
            var url = $"{oauth2Url}?client_id={ClientIdPrivate}&response_type={responseType}&scope={scope}&redirect_uri={RedirectURLPrivate}&state={state}";
            
            if (!string.IsNullOrWhiteSpace(RealmId)) 
                url += $"&realm_id={RealmId}";

            return url;
        }

        public void FillTokens()
        {
            var client = GetDefaultWebClient();
            client.Headers["Authorization"] = $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{ClientIdPrivate}:{ClientSecretPrivate}"))}";
            client.Headers["Content-Type"] = "application/x-www-form-urlencoded";

            var responseJson = client.UploadString(bearerUrl, "POST", $"code={AuthorizationCode}&redirect_uri={RedirectURLPrivate}&grant_type=authorization_code");
            var response = JObject.Parse(responseJson);
            AccessToken = response["access_token"].ToString();
            RefreshToken = response["refresh_token"].ToString();
        }

        public void RefreshAccessToken()
        {
            var client = GetDefaultWebClient();
            client.Headers["Authorization"] = $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{ClientIdPrivate}:{ClientSecretPrivate}"))}";
            client.Headers["Content-Type"] = "application/x-www-form-urlencoded";

            var responseJson = client.UploadString(bearerUrl, "POST", $"refresh_token={RefreshToken}&grant_type=refresh_token");
            var response = JObject.Parse(responseJson);
            AccessToken = response["access_token"].ToString();
            RefreshToken = response["refresh_token"].ToString();
        }

        private List<string> GetTableNames()
        {
            return new List<string>
            {
                "Account",
                "AccountListDetail",
                "APAgingDetail",
                "APAgingSummary",
                "ARAgingDetail",
                "ARAgingSummary",
                "Attachable",
                "BalanceSheet",
                "Batch",
                "Bill",
                "BillPayment",
                "Budget",
                "CashFlow",
                "ChangeDataCapture",
                "Class",
                "CompanyCurrency",
                "CompanyInfo",
                "CreditMemo",
                "Customer",
                "CustomerBalance",
                "CustomerBalanceDetail",
                "CustomerIncome",
                "CustomerType",
                "Department",
                "Deposit",
                "Employee",
                "Entitlements",
                "Estimate",
                "Exchangerate",
                "GeneralLedger",
                "InventoryValuationSummary",
                "Invoice",
                "Item",
                "JournalCode",
                "JournalEntry",
                "JournalReport",
                "JournalReportFR",
                "Payment",
                "PaymentMethod",
                "Preferences",
                "ProfitAndLoss",
                "ProfitAndLossDetail",
                "Purchase",
                "PurchaseOrder",
                "RefundReceipt",
                "SalesByClassSummary",
                "SalesByCustomer",
                "SalesByDepartment",
                "SalesByProduct",
                "SalesReceipt",
                "TaxClassification",
                "TaxCode",
                "TaxRate",
                "TaxService",
                "TaxSummary",
                "TaxAgency",
                "Term",
                "TimeActivity",
                "TransactionList",
                "Transfer",
                "TrialBalance",
                "Vendor",
                "VendorBalance",
                "VendorBalanceDetail",
                "VendorCredit",
                "VendorExpenses"
            };
        }

        private List<StiDataColumnSchema> GetColumns(string collectionName)
        {
            var dataTable = new DataTable(collectionName);
            // var dataTable = GetDataTable(collectionName, $"SELECT * FROM {collectionName} MAXRESULTS 10");
            
            return dataTable != null 
                ? dataTable.Columns.Cast<DataColumn>().ToList().Select(c => new StiDataColumnSchema(c.ColumnName, c.DataType)).ToList() : null;
        }

        public override StiDataSchema RetrieveSchema(bool allowException = false)
        {
            var schema = new StiDataSchema(StiConnectionIdent.QuickBooksDataSource);
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

        public DataTable GetDataTable(string collectionName, string query)
        {
            try
            {
                if (string.IsNullOrEmpty(query))
                    query = Uri.EscapeDataString($"SELECT * FROM {collectionName}");

                var content = ExecuteQuery(query);
                var dataSet = StiJsonToDataSetConverterV2.GetDataSet(content);

                return dataSet != null && dataSet.Tables.Count > 0 ? dataSet.Tables[0].Copy() : new DataTable();
            }
            catch (Exception exception)
            {
                if (exception.InnerException != null)
                    throw exception.InnerException;
                else
                    throw;
            }
        }

        public override void FillDataTable(DataTable table, string query)
        {
            var table1 = GetDataTable(null, query);
            foreach (DataRow row in table1.Rows)
            {
                DataRow newRow = table.NewRow();
                foreach (DataColumn column in table.Columns)
                {
                    if (table1.Columns.Contains(column.ColumnName) && table1.Columns[column.ColumnName].DataType == column.DataType)
                    {
                        newRow[column] = row[table1.Columns[column.ColumnName]];
                    }
                }
                table.Rows.Add(newRow);
            }
        }

        private string ExecuteQuery(string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(RealmId))
                    FillAuthorizationCode();

                if (string.IsNullOrWhiteSpace(AccessToken))
                {
                    if (!string.IsNullOrWhiteSpace(RefreshToken))
                    {
                        RefreshAccessToken();
                    }
                    else if (string.IsNullOrWhiteSpace(AuthorizationCode))
                    {
                        FillAuthorizationCode();
                        FillTokens();
                    }
                    else
                    {
                        FillTokens();
                    }
                }

                var client = GetDefaultWebClient();
                var url = $"{baseUrl}/company/{RealmId}/query?query={Uri.EscapeDataString(query)}";
                var jsonString = client.DownloadString(url);
                var jsonObj = JObject.Parse(jsonString)["QueryResponse"].First;
                if (jsonObj != null)
                {
                    RemoveUnsupportedColumns(jsonObj.First);
                    CorrectRefColumns(jsonObj.First);
                    return "{" + jsonObj.ToString() + "}";
                }

                return "{}";
            }
            catch (WebException e)
            {
                var stream = e.Response.GetResponseStream();
                var reader = new StreamReader(stream);
                var errorJson = reader.ReadToEnd();
                var error = JObject.Parse(errorJson);

                if (error["fault"]?["error"]?[0]?["code"]?.Value<string>() == "3200")
                {
                    AccessToken = "";
                    return ExecuteQuery(query);
                }

                if (error["fault"]?["error"]?[0]?["code"]?.Value<string>() == "3100")
                {
                    RealmId = "";
                    RefreshToken = "";
                    AccessToken = "";
                    return ExecuteQuery(query);
                }

                if (error["error"]?.Value<string>() == "invalid_grant")
                {
                    RefreshToken = "";
                    AccessToken = "";
                    AuthorizationCode = "";
                    return ExecuteQuery(query);
                }
            }
            catch
            {

            }
            return "{}";
        }

        private void RemoveUnsupportedColumns(JToken json)
        {
            var removedItems = new List<JProperty>();
            foreach (JToken obj in json)
            {
                foreach (JProperty item in obj)
                {
                    if (Char.IsLower(item.Name[0])) removedItems.Add(item);
                    if (!(item.Value is JValue) && item.Name.LastIndexOf("Ref") != item.Name.Length - 3) removedItems.Add(item);
                }
            }

            foreach (JProperty item in removedItems)
            {
                item.Remove();
            }
        }

        private void CorrectRefColumns(JToken json)
        {
            foreach (JToken obj in json)
            {
                foreach (JProperty item in obj)
                {
                    if (!(item.Value is JValue) && item.Name.LastIndexOf("Ref") == item.Name.Length - 3)
                    {
                        item.Value = item.Value["value"];
                    }
                }
            }
        }

        public override string GetSampleConnectionString()
        {
            return StiDataOptions.SampleConnectionString.QuickBooks;
        }
        #endregion

        #region Methods.Static
        public static StiQuickBooksConnector Get(string connectionString = null)
        {
            return new StiQuickBooksConnector(connectionString);
        }

        public IDbTransaction BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void ChangeDatabase(string databaseName)
        {
            throw new NotImplementedException();
        }

        public IDbCommand CreateCommand()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        public void Open()
        {
            throw new NotImplementedException();
        }
        #endregion

        public StiQuickBooksConnector(string connectionString = "")
            : base(connectionString)
        {
        }
    }
}
