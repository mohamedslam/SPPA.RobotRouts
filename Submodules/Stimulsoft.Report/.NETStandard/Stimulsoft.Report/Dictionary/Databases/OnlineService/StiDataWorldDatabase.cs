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
using Stimulsoft.Base.Data.Connectors.OnlineServices;
using Stimulsoft.Report.Dictionary.Adapters.OnlineService;
using Stimulsoft.Report.Dictionary.Design;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Dictionary
{
    [TypeConverter(typeof(StiDataWorldDatabaseConverter))]
    public class StiDataWorldDatabase : StiNoSqlDatabase
    {
        #region Properties
        public override StiConnectionType ConnectionType => StiConnectionType.OnlineServices;

        public string Owner => StiConnectionStringHelper.GetConnectionStringKey(this.ConnectionString, "Owner");

        public string Token => StiConnectionStringHelper.GetConnectionStringKey(this.ConnectionString, "Token");

        public string Database => StiConnectionStringHelper.GetConnectionStringKey(this.ConnectionString, "Database");
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiDataWorldDatabase;
        #endregion

        #region Methods.Editors
        public override DialogResult Edit(StiDictionary dictionary, bool newDatabase)
        {
            return StiDataEditorsHelper.Get().DataWorldDatabaseEdit(this, dictionary, newDatabase);
        }

        public override async Task<DialogResult> EditAsync(StiDictionary dictionary, bool newDatabase)
        {
            return await StiDataEditorsHelper.Get().DataWorldDatabaseEditAsync(this, dictionary, newDatabase);
        }
        #endregion

        #region Methods
        public override StiDatabase CreateNew()
        {
            return new StiDataWorldDatabase();
        }

        public override StiDataConnector CreateConnector(string connectionString = null)
        {
            return StiDataWorldConnector.Get(connectionString);
        }

        protected override Type GetDataAdapterType()
        {
            return typeof(StiDataWorldAdapterService);
        }

        public override StiNoSqlSource CreateDataSource(string nameInSource, string name)
        {
            return new StiDataWorldSource(nameInSource, name);
        }
        #endregion

        public StiDataWorldDatabase()
            : this(string.Empty, string.Empty)
        {
        }

        public StiDataWorldDatabase(string name, string connectionString) : base(name, connectionString)
        {
        }

        public StiDataWorldDatabase(string name, string alias, string connectionString) : base(name, alias, connectionString)
        {
        }

        public StiDataWorldDatabase(string name, string alias, string connectionString, bool promptUserNameAndpassword) : base(name, alias, connectionString, promptUserNameAndpassword)
        {
        }

        public StiDataWorldDatabase(string name, string alias, string connectionString, bool promptUserNameAndpassword, string key)
            : base(name, alias, connectionString, promptUserNameAndpassword, key)
        {
        }
    }
}
