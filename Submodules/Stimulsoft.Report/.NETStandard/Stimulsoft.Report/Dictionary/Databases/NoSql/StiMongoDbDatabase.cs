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
using Stimulsoft.Report.Dictionary.Design;

namespace Stimulsoft.Report.Dictionary
{
    [TypeConverter(typeof(StiMongoDbDatabaseConverter))]
    public class StiMongoDbDatabase : StiNoSqlDatabase
    {
        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiMongoDbDatabase;
        #endregion

        #region Methods
        public override StiDatabase CreateNew()
        {
            return new StiMongoDbDatabase();
        }

        public override StiDataConnector CreateConnector(string connectionString = null)
        {
            return StiMongoDbConnector.Get(connectionString);
        }

        protected override Type GetDataAdapterType()
        {
            return typeof(StiMongoDbAdapterService);
        }

        public override StiNoSqlSource CreateDataSource(string nameInSource, string name)
        {
            return new StiMongoDbSource(nameInSource, name);
        }
        #endregion

        public StiMongoDbDatabase()
            : this(string.Empty, string.Empty)
        {
        }

        public StiMongoDbDatabase(string name, string connectionString) : base(name, connectionString)
        {
        }

        public StiMongoDbDatabase(string name, string alias, string connectionString) : base(name, alias, connectionString)
        {
        }

        public StiMongoDbDatabase(string name, string alias, string connectionString, bool promptUserNameAndpassword) : base(name, alias, connectionString, promptUserNameAndpassword)
        {
        }

        public StiMongoDbDatabase(string name, string alias, string connectionString, bool promptUserNameAndpassword, string key)
            : base(name, alias, connectionString, promptUserNameAndpassword, key)
        {
        }
    }
}
