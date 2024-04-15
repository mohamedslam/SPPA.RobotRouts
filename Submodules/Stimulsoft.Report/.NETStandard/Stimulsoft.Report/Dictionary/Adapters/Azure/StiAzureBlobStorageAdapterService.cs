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
using System;
using System.Data;
using System.Linq;

namespace Stimulsoft.Report.Dictionary
{
    public class StiAzureBlobStorageAdapterService : StiNoSqlAdapterService
    {
        #region Methods
        public override Type GetDataSourceType()
        {
            return typeof(StiAzureBlobStorageSource);
        }

        public override StiNoSqlDataConnector CreateConnector(string connectionString = null)
        {
            return StiAzureBlobStorageConnector.Get(connectionString);
        }

        public override StiDataColumnsCollection GetColumnsFromData(StiData data, StiDataSource dataSource)
        {
            return GetColumnsFromData(data, dataSource, CommandBehavior.SchemaOnly);
        }

        public override StiDataColumnsCollection GetColumnsFromData(StiData data, StiDataSource dataSource, CommandBehavior retrieveMode)
        {
            var dataColumns = new StiDataColumnsCollection();
            var azureBlobStorageSource = dataSource as StiAzureBlobStorageSource;

            var connectionString =
                (dataSource.Dictionary.Databases
                .Cast<StiDatabase>()
                .FirstOrDefault(database =>
                    database is StiAzureBlobStorageDatabase
                    && database.Name.ToLowerInvariant().Equals(azureBlobStorageSource.NameInSource.ToLowerInvariant()))
                as StiAzureBlobStorageDatabase)
                ?.ConnectionString;

            if (connectionString == null) return dataColumns;

            // NoSQL DataSources' Name property contains not a 'table', but the whole 'source' name, 'Azure Blob Storage' in this case.
            // So Query property is being used to get table name.
            var columns = CreateConnector(connectionString).GetColumns(azureBlobStorageSource.Query);

            columns.ForEach(column => dataColumns.Add(new StiDataColumn(column.Name, column.Type)));
            
            return dataColumns;
        }

        public override StiDataParametersCollection GetParametersFromData(StiData data, StiDataSource dataSource)
        {
            return new StiDataParametersCollection();
        }

        public override void ConnectDataSourceToData(StiDictionary dictionary, StiDataSource dataSource, bool loadData)
        {
            StiDataLeader.Disconnect(dataSource);

            if (!loadData)
            {
                dataSource.DataTable = new DataTable();
                return;
            }

            var azureBlobStorageSource = dataSource as StiAzureBlobStorageSource;

            var connectionString =
                (dataSource.Dictionary.Databases
                .Cast<StiDatabase>()
                .FirstOrDefault(database =>
                    database is StiAzureBlobStorageDatabase
                    && database.Name.ToLowerInvariant().Equals(azureBlobStorageSource.GetCategoryName().ToLowerInvariant()))
                as StiAzureBlobStorageDatabase)
                ?.ConnectionString;

            if (connectionString == null) return;

            // NoSQL DataSources' Name property contains not a 'table', but the whole 'source' name, 'Azure Blob Storage' in this case.
            // So Query property is being used to get table name.
            dataSource.DataTable =
                CreateConnector(connectionString)
                .GetDataTable(azureBlobStorageSource.Query, azureBlobStorageSource.Query);
        }
        #endregion Methods
    }
}
