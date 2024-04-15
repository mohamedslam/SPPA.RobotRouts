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

using Stimulsoft.Base;
using Stimulsoft.Base.Data.Connectors;
using Stimulsoft.Report.Dictionary.Design;
using System;
using System.ComponentModel;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Dictionary
{
    [TypeConverter(typeof(StiAzureBlobStorageDatabaseConverter))]
    public class StiAzureBlobStorageDatabase : StiNoSqlDatabase
    {
        #region Properties
        public override StiConnectionType ConnectionType => StiConnectionType.Azure;

        public string AccountKey => StiConnectionStringHelper.GetConnectionStringKey(ConnectionString, "AccountKey");
        
        public string AccountName => StiConnectionStringHelper.GetConnectionStringKey(ConnectionString, "AccountName");

        public string ContainerName => StiConnectionStringHelper.GetConnectionStringKey(ConnectionString, "ContainerName");

        public string BlobName => StiConnectionStringHelper.GetConnectionStringKey(ConnectionString, "BlobName");

        public string BlobContentType => StiConnectionStringHelper.GetConnectionStringKey(ConnectionString, "BlobContentType");

        public int CodePage => int.Parse(StiConnectionStringHelper.GetConnectionStringKey(ConnectionString, "CodePage") ?? "0");

        public string Delimiter => Regex.Match(ConnectionString, @"Delimiter=""(.*?)"";").Groups[1].Value;
        
        public bool FirstRowIsHeader => bool.Parse(StiConnectionStringHelper.GetConnectionStringKey(ConnectionString, "FirstRowIsHeader") ?? bool.TrueString);
        #endregion Properties

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiAzureBlobStorageDatabase;
        #endregion

        #region Methods.Editors
        public override DialogResult Edit(StiDictionary dictionary, bool isNewDatabase)
        {
            return StiDataEditorsHelper.Get().AzureBlobStorageDatabaseEdit(this, dictionary, isNewDatabase);
        }

        public override async Task<DialogResult> EditAsync(StiDictionary dictionary, bool isNewDatabase)
        {
            return await StiDataEditorsHelper.Get().AzureBlobStorageDatabaseEditAsync(this, dictionary, isNewDatabase);
        }
        #endregion Methods.Editors

        #region Methods
        public override StiDataConnector CreateConnector(string connectionString = null)
        {
            return StiAzureBlobStorageConnector.Get(connectionString);
        }

        public override StiNoSqlSource CreateDataSource(string nameInSource, string name)
        {
            return new StiAzureBlobStorageSource(nameInSource, name);
        }

        protected override Type GetDataAdapterType()
        {
            return typeof(StiAzureBlobStorageAdapterService);
        }

        public override StiDatabase CreateNew()
        {
            return new StiAzureBlobStorageDatabase();
        }

        protected override void ApplyDatabaseInformationSource(StiDatabaseInformation information, StiReport report, StiDatabaseInformation informationAll, DataTable dataTable, StiSqlSourceType type = StiSqlSourceType.Table)
        {
            var source = CreateDataSource(this.Name, StiNameCreation.CreateName(report, dataTable.TableName, false, false, true));
            source.Alias = dataTable.TableName;

            // NoSQL DataSources' Name property contains not a 'table', but the whole 'source' name, 'Azure Blob Storage' in this case.
            // So Query property is being used to store table name, 'cause Name property can be changed by user.
            source.Query = dataTable.TableName;

            foreach (DataColumn dataColumn in dataTable.Columns)
            {
                source.Columns.Add(new StiDataColumn(dataColumn.ColumnName, dataColumn.DataType));
            }

            report.Dictionary.DataSources.Add(source);
        }
        #endregion Methods

        public StiAzureBlobStorageDatabase()
            : this(string.Empty, string.Empty)
        {
        }

        public StiAzureBlobStorageDatabase(string name, string connectionString) : base(name, connectionString)
        {
        }

        public StiAzureBlobStorageDatabase(string name, string alias, string connectionString)
            : base(name, alias, connectionString)
        {
        }

        public StiAzureBlobStorageDatabase(string name, string alias, string connectionString, bool promptUserNameAndpassword)
            : base(name, alias, connectionString, promptUserNameAndpassword)
        {
        }

        public StiAzureBlobStorageDatabase(string name, string alias, string connectionString, bool promptUserNameAndpassword, string key)
            : base(name, alias, connectionString, promptUserNameAndpassword, key)
        {
        }
    }
}
