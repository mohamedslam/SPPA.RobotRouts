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
    [TypeConverter(typeof(StiGoogleAnalyticsDatabaseConverter))]
    public class StiGoogleAnalyticsDatabase : StiNoSqlDatabase
    {
        #region Properties
        public override StiConnectionType ConnectionType => StiConnectionType.Google;

        public string Base64EncodedAuthSecret => StiConnectionStringHelper.GetConnectionStringKey(ConnectionString, "Base64EncodedAuthSecret");

        public string AccountId => StiConnectionStringHelper.GetConnectionStringKey(ConnectionString, "AccountId");

        public string PropertyId => StiConnectionStringHelper.GetConnectionStringKey(ConnectionString, "PropertyId");

        public string ViewId => StiConnectionStringHelper.GetConnectionStringKey(ConnectionString, "ViewId");

        public string Metrics => StiConnectionStringHelper.GetConnectionStringKey(ConnectionString, "Metrics", new char[] { ';' });

        public string Dimensions => StiConnectionStringHelper.GetConnectionStringKey(ConnectionString, "Dimensions", new char[] { ';' });

        public string StartDate => StiConnectionStringHelper.GetConnectionStringKey(ConnectionString, "StartDate");

        public string EndDate => StiConnectionStringHelper.GetConnectionStringKey(ConnectionString, "EndDate");
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiGoogleAnalyticsDatabase;
        #endregion

        #region Methods.Editors
        public override DialogResult Edit(StiDictionary dictionary, bool isNewDatabase)
        {
            return StiDataEditorsHelper.Get().GoogleAnalyticsDatabaseEdit(this, dictionary, isNewDatabase);
        }

        public override async Task<DialogResult> EditAsync(StiDictionary dictionary, bool isNewDatabase)
        {
            return await StiDataEditorsHelper.Get().GoogleAnalyticsDatabaseEditAsync(this, dictionary, isNewDatabase);
        }
        #endregion

        #region Methods
        public override StiDatabase CreateNew()
        {
            return new StiGoogleAnalyticsDatabase();
        }

        public override StiDataConnector CreateConnector(string connectionString = null)
        {
            return StiGoogleAnalyticsConnector.Get(this.ConnectionString);
        }

        protected override Type GetDataAdapterType()
        {
            return typeof(StiGoogleAnalyticsAdapterService);
        }

        public override StiNoSqlSource CreateDataSource(string nameInSource, string name)
        {
            return new StiGoogleAnalyticsSource(nameInSource, name);
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

        public StiGoogleAnalyticsDatabase()
            : this(string.Empty, string.Empty)
        {
        }

        public StiGoogleAnalyticsDatabase(string name, string connectionString) : base(name, connectionString)
        {
        }

        public StiGoogleAnalyticsDatabase(string name, string alias, string connectionString) : base(name, alias, connectionString)
        {
        }

        public StiGoogleAnalyticsDatabase(string name, string alias, string connectionString, bool promptUserNameAndPassword)
            : base(name, alias, connectionString, promptUserNameAndPassword)
        {
        }

        public StiGoogleAnalyticsDatabase(string name, string alias, string connectionString, bool promptUserNameAndPassword, string key)
            : base(name, alias, connectionString, promptUserNameAndPassword, key)
        {
        }
    }
}
