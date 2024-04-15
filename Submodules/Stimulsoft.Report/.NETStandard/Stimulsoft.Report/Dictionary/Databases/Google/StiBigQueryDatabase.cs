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
using Stimulsoft.Base.Data.Connectors.Google;
using Stimulsoft.Report.Dictionary.Adapters.Google;
using Stimulsoft.Report.Dictionary.DataSources.Google;
using Stimulsoft.Report.Dictionary.Design;
using System;
using System.ComponentModel;
using System.Data;
using System.Threading.Tasks;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Dictionary.Databases.Google
{
    [TypeConverter(typeof(StiBigQueryDatabaseConverter))]
    public class StiBigQueryDatabase : StiNoSqlDatabase
    {
        #region Properties
        public override StiConnectionType ConnectionType => StiConnectionType.Google;

        public string Base64EncodedAuthSecret => StiConnectionStringHelper.GetConnectionStringKey(ConnectionString, "Base64EncodedAuthSecret");

        public string ProjectId => StiConnectionStringHelper.GetConnectionStringKey(ConnectionString, "ProjectId");

        public string DatasetId => StiConnectionStringHelper.GetConnectionStringKey(ConnectionString, "DatasetId");
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiBigQueryDatabase;
        #endregion

        #region Methods.Editors
        public override DialogResult Edit(StiDictionary dictionary, bool isNewDatabase)
        {
            return StiDataEditorsHelper.Get().BigQueryDatabaseEdit(this, dictionary, isNewDatabase);
        }

        public override async Task<DialogResult> EditAsync(StiDictionary dictionary, bool isNewDatabase)
        {
            return await StiDataEditorsHelper.Get().BigQueryDatabaseEditAsync(this, dictionary, isNewDatabase);
        }
        #endregion

        #region Methods
        public override StiDatabase CreateNew()
        {
            return new StiBigQueryDatabase();
        }

        public override StiDataConnector CreateConnector(string connectionString = null)
        {
            return StiBigQueryConnector.Get(this.ConnectionString);
        }

        protected override Type GetDataAdapterType()
        {
            return typeof(StiBigQueryAdapterService);
        }

        public override StiNoSqlSource CreateDataSource(string nameInSource, string name)
        {
            return new StiBigQuerySource(nameInSource, name);
        }

        protected override void ApplyDatabaseInformationSource(StiDatabaseInformation information, StiReport report, StiDatabaseInformation informationAll, DataTable dataTable, StiSqlSourceType type = StiSqlSourceType.Table)
        {
            var source = CreateDataSource(this.Name, StiNameCreation.CreateName(report, dataTable.TableName, false, false, true));
            source.Alias = dataTable.TableName;

            source.Query = $"select * from `{dataTable.TableName}`";

            foreach (DataColumn dataColumn in dataTable.Columns)
            {
                source.Columns.Add(new StiDataColumn(dataColumn.ColumnName, dataColumn.DataType));
            }

            report.Dictionary.DataSources.Add(source);
        }
        #endregion

        public StiBigQueryDatabase()
            : this(string.Empty, string.Empty)
        {
        }

        public StiBigQueryDatabase(string name, string connectionString) : base(name, connectionString)
        {
        }

        public StiBigQueryDatabase(string name, string alias, string connectionString) : base(name, alias, connectionString)
        {
        }

        public StiBigQueryDatabase(string name, string alias, string connectionString, bool promptUserNameAndPassword) 
            : base(name, alias, connectionString, promptUserNameAndPassword)
        {
        }

        public StiBigQueryDatabase(string name, string alias, string connectionString, bool promptUserNameAndPassword, string key)
            : base(name, alias, connectionString, promptUserNameAndPassword, key)
        {
        }
    }
}
