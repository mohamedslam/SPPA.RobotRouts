#region Copyright (C) 2003-2022 Stimulsoft
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
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Dictionary.Design;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Net;
#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Dictionary
{
    [TypeConverter(typeof(StiODataDatabaseConverter))]
    public class StiODataDatabase : StiSqlDatabase
    {
        #region Methods
        public override StiDatabase CreateNew()
        {
            return new StiODataDatabase();
        }

        /// <summary>
        /// Returns new data connector for this database.
        /// </summary>
        /// <returns>Created connector.</returns>
        public override StiDataConnector CreateConnector(string connectionString = null)
        {
            var oDataConnection = StiODataConnector.Get(connectionString, this.Headers);
            oDataConnection.AllowException = this.AllowException;

            oDataConnection.Version = this.Version;
            oDataConnection.CookieContainer = this.CookieContainer;

            return oDataConnection;
        }

        /// <summary>
        /// Returns new data source for this database.
        /// </summary>
        /// <returns>Created data source.</returns>
        public override StiSqlSource CreateDataSource(string nameInSource, string name)
        {
            return new StiODataSource(nameInSource, name);
        }

        protected override Type GetDataAdapterType()
        {
            return typeof(StiODataAdapterService);
        }

        public override string GetConnectionStringHelper()
        {
            return "StiODataConnectionHelper";
        }

        public override string MapUserNameAndPassword(string userName, string password)
        {
            return $"UserName = {userName}; Password = {password}";
        }

        public override void RegData(StiDictionary dictionary, bool loadData)
        {
            this.CookieContainer = dictionary.Report.CookieContainer;
        }
        #endregion

        #region IStiPropertyGridObject        
        public override StiComponentId ComponentId => StiComponentId.StiODataDatabase;
        #endregion

        #region Properties
        /// <summary>
        /// Gets a connection type.
        /// </summary>
        public override StiConnectionType ConnectionType => StiConnectionType.Rest;

        [StiSerializable]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [DefaultValue(StiODataVersion.V4)]
        public StiODataVersion Version { get; set; } = StiODataVersion.V4;

        [Browsable(false)]
        public CookieContainer CookieContainer { get; set; }

        /// <summary>
        /// List of headers used for http requests to load data.
        /// </summary>
        /// 
        [Browsable(false)]
        [Description("List of headers used for http requests to load data.")]
        public NameValueCollection Headers { get; protected set; } = new NameValueCollection();
        #endregion

        /// <summary>
        /// Creates a new object of the type StiODataDatabase.
        /// </summary>
        public StiODataDatabase()
            : this(string.Empty, string.Empty)
        {
        }
        
        /// <summary>
        /// Creates a new object of the type StiODataDatabase.
        /// </summary>
        public StiODataDatabase(string name, string connectionString)
            : base(name, connectionString)
        {
        }

        /// <summary>
        /// Creates a new object of the type StiODataDatabase.
        /// </summary>
        public StiODataDatabase(string name, string alias, string connectionString)
            : base(name, alias, connectionString)
        {
        }

        /// <summary>
        /// Creates a new object of the type StiODataDatabase.
        /// </summary>
        public StiODataDatabase(string name, string alias, string connectionString, bool promptUserNameAndpassword)
            : base(name, alias, connectionString, promptUserNameAndpassword)
        {
        }

        /// <summary>
        /// Creates a new object of the type StiODataDatabase.
        /// </summary>
        public StiODataDatabase(string name, string alias, string connectionString, bool promptUserNameAndpassword, string key)
            : base(name, alias, connectionString, promptUserNameAndpassword, key)
        {
        }

        /// <summary>
        /// Creates a new object of the type StiODataDatabase.
        /// </summary>
        public StiODataDatabase(string name, string alias, string connectionString, bool promptUserNameAndpassword, string key, StiODataVersion version)
            : base(name, alias, connectionString, promptUserNameAndpassword, key)
        {
            this.Version = version;
        }
    }
}
