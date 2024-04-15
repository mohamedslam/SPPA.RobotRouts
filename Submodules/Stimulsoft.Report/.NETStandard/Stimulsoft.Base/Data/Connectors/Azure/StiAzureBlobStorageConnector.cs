#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
{	                         										}
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
{	TRADE SECRETS OF Stimulsoft										}
{																	}
{	CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON		}
{	ADDITIONAL RESTRICTIONS.										}
{																	}
{*******************************************************************}
*/
#endregion Copyright (C) 2003-2022 Stimulsoft

using Stimulsoft.Base.Data.Connectors;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Stimulsoft.Base
{
    public class StiAzureBlobStorageConnector : StiDbNoSqlDataConnector
    {
        #region Fields
        private object azureBlobStorageProvider;
        #endregion Fields

        #region Fields.Static
        /// <summary>
        /// Many instances of connector are created to retrieve only one read-only property (Name, Ident, etc.).
        /// This is object to return when just that connector was requested. See <see cref="Get(string)"/> method.
        /// </summary>
        private static readonly StiAzureBlobStorageConnector emptyConnector = new StiAzureBlobStorageConnector();
        #endregion Fields.Static

        #region Properties
        public override StiConnectionIdent ConnectionIdent => StiConnectionIdent.AzureBlobStorage;

        public override StiConnectionOrder ConnectionOrder => StiConnectionOrder.AzureBlobStorage;

        public override string Name => "Azure Blob Storage";

        public override string[] NuGetPackages => new string[] { "Stimulsoft.Data.AzureBlobStorage" };

        public override string NuGetVersion => "2021.4.3";
        #endregion Properties

        #region Methods
        public override string GetSampleConnectionString()
        {
            return StiDataOptions.SampleConnectionString.AzureBlobStorage;
        }

        public override StiTestConnectionResult TestConnection()
        {
            try
            {
                var provider = GetAzureBlobStorageProvider();

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
            DataSet datasetOfSameSchema = GetFullDataset();

            var schema = new StiDataSchema(this.ConnectionIdent);

            #region Tables
            foreach (DataTable table in datasetOfSameSchema.Tables)
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

            #region Relations
            foreach (DataRelation relation in datasetOfSameSchema.Relations)
            {
                schema.Relations.Add(new StiDataRelationSchema
                {
                    Name = relation.RelationName,
                    ParentSourceName = relation.ParentTable.TableName,
                    ChildSourceName = relation.ChildTable.TableName,
                    ParentColumns = relation.ParentColumns.Select(col => col.ColumnName).ToList(),
                    ChildColumns = relation.ChildColumns.Select(col => col.ColumnName).ToList()
                });
            }
            #endregion

            schema.Relations = schema.Relations.OrderBy(e => e.Name).ToList();
            schema.Tables = schema.Tables.OrderBy(e => e.Name).ToList();

            return schema;
        }

        public override List<StiDataColumnSchema> GetColumns(string collectionName)
        {
            var dataTableSchema = RetrieveSchema().Tables.FirstOrDefault(table => table.Name.ToLowerInvariant() == collectionName.ToLowerInvariant());

            return dataTableSchema?.Columns;
        }

        public override DataTable GetDataTable(string collectionName, string query, int? index = null, int? count = null)
        {
            // In order to get rid of references to DataSet (DataTable.DataSet is referencing it) copy of table is being returned.
            // That copy has DataTable.DataSet == null, so DataSet is not referenced and can be removed by GC.
            return GetFullDataset().Tables[collectionName].Copy();
        }

        /// <summary>
        /// Retrieve all container names starting with specified <paramref name="prefix"/>, or all names if no <paramref name="prefix"/> specified.
        /// </summary>
        /// <param name="prefix">Only names starting with this prefix will be returned.</param>
        public List<string> GetContainerNamesList(string prefix = null)
        {
            try
            {
                var provider = GetAzureBlobStorageProvider();

                var method = provider.GetType().GetMethod("GetContainerNamesList");
                var containerNames = method.Invoke(provider, new[] { prefix }) as List<string>;

                return containerNames;
            }
            catch (Exception exception)
            {
                if (exception.InnerException != null)
                    throw exception.InnerException;
                else
                    throw;
            }
        }

        /// <summary>
        /// Retrieve all blob names starting with specified <paramref name="prefix"/>, or all names if no <paramref name="prefix"/> specified.
        /// </summary>
        /// <param name="prefix">Only names starting with this prefix will be returned.</param>
        public List<string> GetBlobNamesList(string prefix = null)
        {
            try
            {
                var provider = GetAzureBlobStorageProvider();

                var method = provider.GetType().GetMethod("GetBlobNamesList");
                var blobNames = method.Invoke(provider, new object[] { prefix }) as List<string>;

                return blobNames;
            }
            catch (Exception exception)
            {
                if (exception.InnerException != null)
                    throw exception.InnerException;
                else
                    throw;
            }
        }

        /// <summary>
        /// Try to connect to Azure Blob Storage using current connection string, download blob contents and infer type of data stored.
        /// </summary>
        /// <returns>"Excel", "JSON" or "XML" if succeeded, or <see langword="null"/> if failed. "CSV" cannot be inferred.</returns>
        public string GetBlobContentTypeOrDefault()
        {
            // Read first 10 bytes (or less, if content size < 10 bytes) of content and try to use it to infer type.
            // First bytes of data can be "non-chars" (BOM for example), so let's take just a bit more than first byte.
            byte[] contentBeginning;

            try
            {
                var provider = GetAzureBlobStorageProvider();

                var method = provider.GetType().GetMethod("GetBlobContentPart");
                contentBeginning = method.Invoke(provider, new object[] { 0, 10 }) as byte[];
            }
            catch
            {
                return null;            
            }

            if (contentBeginning.Length <= 0) return null;

            char firstChar;

            using (var reader = new StreamReader(new MemoryStream(contentBeginning, 0, contentBeginning.Length), Encoding.Default, true, 128))// 128 chars is the minimum allowable buffer size
            {
                // Reader will read first significant symbol (without BOM or whatever else)
                firstChar = (char)reader.Read();
            }

            // There's no (easy) way to find out if data is CSV.
            switch (firstChar)
            {
                case '{':
                    return "JSON";

                case '<':
                    return "XML";

                case 'P':
                    return "Excel";

                default:
                    return null;
            }
        }

        /// <summary>
        /// Create object of type exported from data adapter package (presumably downloaded from nuget).
        /// That type is responsible for all network-related operations with Azure Blob Storage.
        /// </summary>
        private object GetAzureBlobStorageProvider()
        {
            if (azureBlobStorageProvider != null)
                return azureBlobStorageProvider;

            // Provider needs only the beginning of connection string
            azureBlobStorageProvider = AssemblyHelper.CreateObject(
                "Stimulsoft.Data.AzureBlobStorage.StiAzureBlobStorageProvider",
                new object[] { this.ConnectionString.Substring(0, ConnectionString.IndexOf(";BlobContentType=")) });

            return azureBlobStorageProvider;
        }

        /// <summary>
        /// Get dataset stored in Blob specified by this connector's connection string.
        /// </summary>
        private DataSet GetFullDataset()
        {
            try
            {
                var provider = GetAzureBlobStorageProvider();

                var method = provider.GetType().GetMethod("GetBlobContent");
                var blobBytes = method.Invoke(provider, null) as byte[];

                return GetDataSetFromBlobContent(blobBytes);
            }
            catch (Exception exception)
            {
                if (exception.InnerException != null)
                    throw exception.InnerException;
                else
                    throw;
            }
        }

        /// <summary>
        /// Create <see cref="DataSet"/> from <paramref name="blobContent"/> bytes,
        /// assuming content is of type specified in "BlobContentType" property of connection string
        /// </summary>
        private DataSet GetDataSetFromBlobContent(byte[] blobContent)
        {
            var contentTypeName = StiConnectionStringHelper.GetConnectionStringKey(ConnectionString, "BlobContentType") ?? string.Empty;

            DataSet resultDataset;

            switch (contentTypeName)
            {
                case "CSV":
                    var codePage = StiConnectionStringHelper.GetConnectionStringKey(ConnectionString, "CodePage") ?? "0";

                    var separator = Regex.Match(ConnectionString, @"Delimiter=""(.*?)"";").Groups[1].Value;

                    resultDataset = StiCsvConnector.Get().GetDataSet(new StiCsvOptions(blobContent, int.Parse(codePage), separator));
                    break;

                case "Excel":
                    var firstRowIsHeader = StiConnectionStringHelper.GetConnectionStringKey(ConnectionString, "FirstRowIsHeader") ?? bool.TrueString;

                    resultDataset = StiExcelConnector.Get().GetDataSet(new StiExcelOptions(blobContent, bool.Parse(firstRowIsHeader)));
                    break;

                case "JSON":
                    resultDataset = StiJsonConnector.Get().GetDataSet(new StiJsonOptions(blobContent));
                    break;

                case "XML":
                    resultDataset = StiXmlConnector.Get().GetDataSet(new StiXmlOptions(blobContent));
                    break;

                default:
                    throw new Exception($"Blob content type \"{contentTypeName}\" is not supported.");
            }

            return resultDataset ?? new DataSet();
        }
        #endregion Methods

        #region Methods.Static
        public static StiAzureBlobStorageConnector Get(string connectionString = null)
        {
            if (connectionString == null)
                return emptyConnector;

            return new StiAzureBlobStorageConnector(connectionString);
        }
        #endregion Methods.Static

        private StiAzureBlobStorageConnector(string connectionString = null)
            : base(connectionString)
        {
            this.FolderAssembly = $"Stimulsoft.Data.AzureBlobStorage-{this.NuGetVersion}";
            this.NameAssembly = "Stimulsoft.Data.AzureBlobStorage.dll";
        }
    }
}
