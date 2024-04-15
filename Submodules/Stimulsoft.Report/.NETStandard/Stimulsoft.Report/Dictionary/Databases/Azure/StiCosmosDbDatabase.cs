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

using System;
using System.ComponentModel;
using Stimulsoft.Base;
using Stimulsoft.Base.Data.Connectors.Azure;
using Stimulsoft.Report.Dictionary.Adapters.Azure;
using Stimulsoft.Report.Dictionary.DataSources.Azure;
using Stimulsoft.Report.Dictionary.Design;

namespace Stimulsoft.Report.Dictionary.Databases.Azure
{
    [TypeConverter(typeof(StiCosmosDbDatabaseConverter))]
    public class StiCosmosDbDatabase : StiNoSqlDatabase
    {
        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiCosmosDbDatabase;
        #endregion

        #region Properties
        public override StiConnectionType ConnectionType => StiConnectionType.Azure;
        #endregion

        #region Methods
        public override StiDatabase CreateNew()
        {
            return new StiCosmosDbDatabase();
        }

        public override StiDataConnector CreateConnector(string connectionString = null)
        {
            return StiCosmosDbConnector.Get(connectionString);
        }

        protected override Type GetDataAdapterType()
        {
            return typeof(StiCosmosDbAdapterService);
        }

        public override StiNoSqlSource CreateDataSource(string nameInSource, string name)
        {
            return new StiCosmosDbSource(nameInSource, name);
        }
        #endregion

        public StiCosmosDbDatabase()
            : this(string.Empty, string.Empty)
        {
        }

        public StiCosmosDbDatabase(string name, string connectionString) : base(name, connectionString)
        {
        }

        public StiCosmosDbDatabase(string name, string alias, string connectionString) : base(name, alias, connectionString)
        {
        }

        public StiCosmosDbDatabase(string name, string alias, string connectionString, bool promptUserNameAndpassword) : base(name, alias, connectionString, promptUserNameAndpassword)
        {
        }

        public StiCosmosDbDatabase(string name, string alias, string connectionString, bool promptUserNameAndpassword, string key)
            : base(name, alias, connectionString, promptUserNameAndpassword, key)
        {
        }
    }
}
