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
using Stimulsoft.Base.Data.Connectors.Azure;

namespace Stimulsoft.Report.Dictionary.Databases.Azure
{
    public class StiAzureSqlDatabase : StiSqlDatabase
    {
        public override StiConnectionType ConnectionType => StiConnectionType.Azure;

        public override StiDataConnector CreateConnector(string connectionString = null)
        {
            return StiAzureSqlConnector.Get(connectionString);
        }

        public override StiDatabase CreateNew()
        {
            return new StiAzureSqlDatabase();
        }

        public override string GetConnectionStringHelper()
        {
            return "StiAzureSqlConnectionHelper";
        }

        /// <summary>
        /// Creates a new object of the type StiAzureSqlDatabase.
        /// </summary>
        public StiAzureSqlDatabase()
            : base()
        {
        }

        /// <summary>
        /// Creates a new object of the type StiAzureSqlDatabase.
        /// </summary>
        public StiAzureSqlDatabase(string name, string connectionString)
            : base(name, connectionString)
        {
        }

        /// <summary>
        /// Creates a new object of the type StiAzureSqlDatabase.
        /// </summary>
        public StiAzureSqlDatabase(string name, string alias, string connectionString)
            : base(name, alias, connectionString)
        {
        }

        /// <summary>
        /// Creates a new object of the type StiAzureSqlDatabase.
        /// </summary>
        public StiAzureSqlDatabase(string name, string alias, string connectionString, bool promptUserNameAndpassword)
            : base(name, alias, connectionString, promptUserNameAndpassword)
        {
        }

        /// <summary>
        /// Creates a new object of the type StiAzureSqlDatabase.
        /// </summary>
        public StiAzureSqlDatabase(string name, string alias, string connectionString, bool promptUserNameAndpassword, string key)
            : base(name, alias, connectionString, promptUserNameAndpassword, key)
        {
        }
    }
}
