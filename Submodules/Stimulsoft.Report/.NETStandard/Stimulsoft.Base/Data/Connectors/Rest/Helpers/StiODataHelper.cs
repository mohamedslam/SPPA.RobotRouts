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

using Stimulsoft.Base.Data.Connectors;
using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Linq;

namespace Stimulsoft.Base
{
    public class StiODataHelper
    {
        #region Methods
        /// <summary>
        /// Returns schema object which contains information about structure of the database. Schema returned start at specified root element (if it applicable).
        /// </summary>
        public StiDataSchema RetrieveSchema()
        {
            if (string.IsNullOrEmpty(ConnectionString))
                return null;

            var schema = new StiDataSchema(StiConnectionIdent.ODataDataSource);

            try
            {
                using (var client = GetDefaultWebClient())
                {
                    var metadata = client.DownloadString(StiUrl.Combine(Address, "$metadata"));

                    XmlReaderSettings settings = new XmlReaderSettings();
                    settings.DtdProcessing = DtdProcessing.Prohibit;

                    using (var reader = new StringReader(metadata))
                    using (var xmlReader = XmlReader.Create(reader, settings))
                    {
                        var root = XElement.Load(xmlReader);
                        var edmx = root.GetNamespaceOfPrefix("edmx");
                        var edm = root.Element(edmx + "DataServices").Elements().First().GetDefaultNamespace();
                        var edmDataServices = root.Element(edmx + "DataServices");
                        var elementSchema = GetElementShemaEntityType(edmDataServices.Elements());
                        var namespaceStr = elementSchema.Attribute("Namespace") != null ? (string)elementSchema.Attribute("Namespace") : null;

                        var types = new Hashtable();
                        var hash = new Hashtable();

                        #region Parse Types
                        foreach (var entityType in elementSchema.Elements().Where(e => e.Name.LocalName == "EntityType" || e.Name.LocalName == "ComplexType"))
                        {
                            try
                            {
                                var name = (string)entityType.Attribute("Name");
                                var baseType = entityType.Attribute("BaseType") != null ? (string)entityType.Attribute("BaseType") : null;

                                if (string.IsNullOrWhiteSpace(name)) continue;
                                var properties = entityType.Elements(edm + "Property");

                                var table = new StiDataTableSchema(name, name);

                                if (baseType != null)
                                {
                                    var str = baseType.Replace(namespaceStr + ".", "");
                                    hash[str] = table;
                                }

                                foreach (var property in properties)
                                {
                                    try
                                    {
                                        var propertyName = (string)property.Attribute("Name");
                                        if (string.IsNullOrWhiteSpace(propertyName)) continue;

                                        var propertyNullable = property.Attribute("Nullable") != null && property.Attribute("Nullable").Value == "true";
                                        var propertyType = (string)property.Attribute("Type");
                                        var columnType = GetNetType(propertyType);

                                        if (propertyNullable)
                                            columnType = typeof(Nullable<>).MakeGenericType(columnType);

                                        var column = new StiDataColumnSchema(propertyName, columnType);
                                        table.Columns.Add(column);
                                    }
                                    catch
                                    {
                                    }
                                }

                                types[namespaceStr + "." + table.Name] = table;
                            }
                            catch
                            {
                            }

                            foreach (string tableName in hash.Keys)
                            {
                                var table = hash[tableName] as StiDataTableSchema;
                                var baseTable = schema.Tables.FirstOrDefault(t => t.Name == tableName);
                                if (baseTable == null) continue;

                                foreach (var column in baseTable.Columns)
                                {
                                    if (table.Columns.Any(c => c.Name == column.Name)) continue;
                                    table.Columns.Add(column);
                                }
                            }
                        }
                        #endregion

                        elementSchema = GetElementShemaEntityContainer(edmDataServices.Elements());

                        #region Parse Containers
                        foreach (var entityCont in elementSchema.Elements().Where(e => e.Name.LocalName == "EntityContainer"))
                        {
                            foreach (var entitySet in entityCont.Elements().Where(e => e.Name.LocalName == "EntitySet"))
                            {
                                try
                                {
                                    var name = (string)entitySet.Attribute("Name");
                                    var type = (string)entitySet.Attribute("EntityType");

                                    if (string.IsNullOrWhiteSpace(name)) continue;

                                    var table = new StiDataTableSchema(name, name);
                                    var columnsTable = types[type] as StiDataTableSchema;
                                    if (columnsTable != null)
                                        table.Columns.AddRange(columnsTable.Columns);

                                    schema.Tables.Add(table);
                                }
                                catch
                                {
                                }

                                foreach (string tableName in hash.Keys)
                                {
                                    var table = hash[tableName] as StiDataTableSchema;
                                    var baseTable = schema.Tables.FirstOrDefault(t => t.Name == tableName);
                                    if (baseTable == null) continue;

                                    foreach (var column in baseTable.Columns)
                                    {
                                        if (table.Columns.Any(c => c.Name == column.Name)) continue;
                                        table.Columns.Add(column);
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                }
                return schema;
            }
            catch
            {
                if (AllowException)
                    throw;
                else
                    return null;
            }
        }

        private XElement GetElementShemaEntityType(IEnumerable<XElement> elements)
        {
            foreach(var element in elements)
            {
                var childElements = element.Elements().Where(e => e.Name.LocalName == "EntityType" || e.Name.LocalName == "ComplexType");
                if (childElements.Any())
                    return element;
            }

            return elements.FirstOrDefault();
        }

        private XElement GetElementShemaEntityContainer(IEnumerable<XElement> elements)
        {
            foreach (var element in elements)
            {
                var childElements = element.Elements().Where(e => e.Name.LocalName == "EntityContainer");
                if (childElements.Any())
                    return element;

            }
            return elements.FirstOrDefault();
        }

        public void FillDataTable(DataTable table, string query)
        {
            if (string.IsNullOrEmpty(this.ConnectionString)) return;

            var currentCulture = Thread.CurrentThread.CurrentCulture;

            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

                using (var client = GetDefaultWebClient())
                {
                    var url = StiUrl.Combine(this.Address, query);
                    var metadata = client.DownloadString(url);

                    #region JSON
                    object odata = null;
                    try
                    {
                        odata = JsonConvert.DeserializeObject(metadata);
                    }
                    catch
                    {
                    }

                    if (odata != null && odata is JObject)
                    {
                        JArray values = null;

                        var jObject = odata as JObject;
                        if (jObject != null)
                        {
                            foreach (var child in jObject.Children())
                            {
                                var jProperty = child as JProperty;
                                if (jProperty != null && jProperty.Name == "value" && jProperty.Value is JArray)
                                {
                                    values = jProperty.Value as JArray;
                                }
                            }
                        }

                        if (values != null)
                        {
                            foreach (JObject value in values.Children())
                            {
                                var row = table.NewRow();

                                foreach (JProperty columnObjValue in value.Children())
                                {
                                    try
                                    {
                                        var columnName = columnObjValue.Name;
                                        var columnValue = columnObjValue.Value;

                                        var currentColumn = table.Columns[columnName];
                                        if (currentColumn != null)
                                        {
                                            var currentValue = StiConvert.ChangeType(columnValue, currentColumn.DataType);
                                            row[columnName] = currentValue ?? DBNull.Value;
                                        }
                                    }
                                    catch
                                    {

                                    }
                                }

                                table.Rows.Add(row);
                            }
                        }
                    }
                    #endregion

                    #region XML
                    else
                    {
                        XmlReaderSettings settings = new XmlReaderSettings();
                        settings.DtdProcessing = DtdProcessing.Prohibit;

                        using (var reader = new StringReader(metadata))
                        using (var xmlReader = XmlReader.Create(reader, settings))
                        {
                            var root = XElement.Load(xmlReader);
                            var title = root.Elements().FirstOrDefault(e => e.Name.LocalName == "title");
                            if (title != null) table.TableName = title.Value;

                            foreach (var entry in root.Elements().Where(e => e.Name.LocalName == "entry"))
                            {
                                var elementContent = entry.Elements().FirstOrDefault(e => e.Name.LocalName == "content");
                                if (elementContent == null) continue;

                                var elementProperties = elementContent.Elements().FirstOrDefault(e => e.Name.LocalName.EndsWith("properties"));
                                if (elementProperties == null) continue;

                                var row = table.NewRow();

                                #region Name
                                try
                                {
                                    var elementTitle = entry.Elements().FirstOrDefault(e => e.Name.LocalName == "title");
                                    if (elementTitle != null && table.Columns["Name"] != null) row["Name"] = elementTitle.Value;
                                }
                                catch
                                {
                                }
                                #endregion

                                #region Description
                                try
                                {
                                    var elementSummary = entry.Elements().FirstOrDefault(e => e.Name.LocalName == "summary");
                                    if (elementSummary != null && table.Columns["Description"] != null) row["Description"] = elementSummary.Value;
                                }
                                catch
                                {
                                }
                                #endregion


                                foreach (var elementProperty in elementProperties.Elements())
                                {
                                    try
                                    {
                                        var columnName = elementProperty.Name.LocalName.Replace("d:", "");
                                        var columnValue = elementProperty.Value;

                                        var currentColumn = table.Columns[columnName];
                                        if (currentColumn != null)
                                        {
                                            var value = StiConvert.ChangeType(columnValue, currentColumn.DataType);
                                            row[columnName] = value ?? DBNull.Value;
                                        }
                                    }
                                    catch
                                    {

                                    }
                                }

                                table.Rows.Add(row);
                            }
                        }
                    }
                    #endregion
                }
            }
            catch
            {
                if (AllowException)
                    throw;
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
        }

        public List<StiDataColumnSchema> GetColumns(string collectionName)
        {
            if (string.IsNullOrEmpty(ConnectionString))
                return null;

            var columns = new List<StiDataColumnSchema>();

            foreach (var table in RetrieveSchema().Tables)
            {
                if (table.Name == collectionName)
                {
                    foreach (var column in table.Columns)
                    {
                        columns.Add(new StiDataColumnSchema(column.Name, column.Type));
                    }
                }
            }

            return columns;
        }
        /// <summary>
        /// Returns StiTestConnectionResult that is the information of whether the connection string specified in this class is correct.
        /// </summary>
        /// <returns>The result of testing the connection string.</returns>
        public StiTestConnectionResult TestConnection()
        {
            try
            {
                using (var client = GetDefaultWebClient())
                {
                    client.DownloadString(Address);
                }
            }
            catch (Exception exception)
            {
                return StiTestConnectionResult.MakeWrong(exception.Message);
            }

            return StiTestConnectionResult.MakeFine();
        }
        #endregion

        #region Methods.Helpers
        /// <summary>
        /// Returns a .NET type from the specified string representaion of the database type.
        /// </summary>
        public static Type GetNetType(string dbType)
        {
            if (string.IsNullOrWhiteSpace(dbType))
                return null;

            dbType = dbType.ToLowerInvariant();

            if (dbType.StartsWith("edm."))
                dbType = dbType.Replace("edm.", "");

            switch (dbType)
            {
                case "int64":
                    return typeof(Int64);

                case "int32":
                    return typeof(Int32);

                case "int16":
                    return typeof(Int16);

                case "byte":
                    return typeof(Byte);

                case "sbyte":
                    return typeof(SByte);

                case "int":
                    return typeof(Int32);

                case "boolean":
                    return typeof(Boolean);

                case "decimal":
                    return typeof(decimal);

                case "float":
                    return typeof(float);

                case "double":
                    return typeof(double);

                case "time":
                case "datetime":
                    return typeof(DateTime);

                case "datetimeoffset":
                    return typeof(DateTimeOffset);

                case "guid":
                    return typeof(Guid);

                case "binary":
                    return typeof(byte[]);

                default:
                    return typeof(string);
            }
        }

        public static string GetBearerAccessToken(string url, string userName, string password, string clientId)
        {
            using (var client = new WebClient())
            {
                client.Headers["Content-Type"] = "application/json";

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                string parametersJson = $"{{\"grant_type\":\"password\",\"username\":\"{userName}\",\"password\":\"{password}\",\"client_id\":\"{clientId}\"}}";

                var result = client.UploadString(url, "POST", parametersJson);

                var value = JObject.Parse(result)["access_token"].ToString();

                if (!string.IsNullOrEmpty(value))
                    return value;
            }

            return null;
        }

        private WebClient GetDefaultWebClient()
        {
            var client = new WebClient();
            client.Proxy = StiProxy.GetProxy();
            client.Encoding = StiBaseOptions.WebClientEncoding;
            client.Headers.Add(this.Headers);

            if (!string.IsNullOrWhiteSpace(this.Token))
            {
                client.Headers["Authorization"] = "Bearer " + this.Token;
            }
            else if (!string.IsNullOrWhiteSpace(this.AddressBearer))
            {
                if (string.IsNullOrWhiteSpace(this.bearerAccessToken))
                {
                    this.bearerAccessToken = GetBearerAccessToken(this.AddressBearer, this.UserName, this.Password, this.ClientId);

                    if (!string.IsNullOrWhiteSpace(this.bearerAccessToken))
                        client.Headers["Authorization"] = "Bearer " + this.bearerAccessToken;
                }
            }
            else if (!string.IsNullOrWhiteSpace(this.UserName))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(this.UserName, this.Password);
            }

            return client;
        }
        #endregion

        #region Fields
        private string bearerAccessToken;
        #endregion

        #region Properties
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

        public StiODataHelper(string connectionString, NameValueCollection headers = null)
        {
            this.ConnectionString = connectionString.Replace("\r\n", "").Replace(" ", "");
            this.Headers = headers ?? new NameValueCollection();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        }
    }
}
