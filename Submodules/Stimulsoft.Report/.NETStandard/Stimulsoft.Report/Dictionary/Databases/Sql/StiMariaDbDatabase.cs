﻿#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports       										}
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
using Stimulsoft.Report.Dictionary.Design;
using System;
using System.ComponentModel;

namespace Stimulsoft.Report.Dictionary
{
    [TypeConverter(typeof(StiSqlDatabaseConverter))]
    public class StiMariaDbDatabase : StiMySqlDatabase
    {
        #region Properties
        public override StiComponentId ComponentId => StiComponentId.StiMariaDbDatabase;
        #endregion

        #region Methods
        /// <summary>
        /// Returns new data connector for this database.
        /// </summary>
        /// <returns>Created connector.</returns>
        public override StiDataConnector CreateConnector(string connectionString = null)
        {
            return StiMariaDbConnector.GetNew(connectionString);
        }

        public override StiDatabase CreateNew()
        {
            return new StiMariaDbDatabase();
        }

        /// <summary>
        /// Returns new data source for this database.
        /// </summary>
        /// <returns>Created data source.</returns>
        public override StiSqlSource CreateDataSource(string nameInSource, string name)
        {
            return new StiMariaDbSource(nameInSource, name);
        }

        protected override Type GetDataAdapterType()
        {
            return typeof(StiMariaDbAdapterService);
        }
        #endregion

        /// <summary>
        /// Creates a new object of the type StiMariaDbDatabase.
        /// </summary>
        public StiMariaDbDatabase()
            : this(string.Empty, string.Empty)
        {
        }

        /// <summary>
        /// Creates a new object of the type StiMariaDbDatabase.
        /// </summary>
        public StiMariaDbDatabase(string name, string connectionString)
            : base(name, connectionString)
        {
        }

        /// <summary>
        /// Creates a new object of the type StiMariaDbDatabase.
        /// </summary>
        public StiMariaDbDatabase(string name, string alias, string connectionString)
            : base(name, alias, connectionString)
        {
        }

        /// <summary>
        /// Creates a new object of the type StiMariaDbDatabase.
        /// </summary>
        public StiMariaDbDatabase(string name, string alias, string connectionString, bool promptUserNameAndpassword)
            : base(name, alias, connectionString, promptUserNameAndpassword)
        {
        }

        /// <summary>
        /// Creates a new object of the type StiMariaDbDatabase.
        /// </summary>
        public StiMariaDbDatabase(string name, string alias, string connectionString, bool promptUserNameAndpassword, string key)
            : base(name, alias, connectionString, promptUserNameAndpassword, key)
        {
        }
    }
}
