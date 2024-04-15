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

using System;
using System.ComponentModel;
using Stimulsoft.Base;
using Stimulsoft.Report.Dictionary.Design;

namespace Stimulsoft.Report.Dictionary
{
	[TypeConverter(typeof(StiSqlSourceConverter))]
	public class StiDB2Source : StiSqlSource
	{
        #region Methods
        /// <summary>
        /// Returns new data connector for this datasource.
        /// </summary>
        /// <returns>Created connector.</returns>
        public override StiSqlDataConnector CreateConnector(string connectionString = null)
        {
            return StiDb2Connector.Get(connectionString);
        }

        protected override Type GetDataAdapterType()
        {
            return typeof(StiDB2AdapterService);
        }
        #endregion

        #region Methods.override
        public override StiComponentId ComponentId => StiComponentId.StiDB2Source;

        public override StiDataSource CreateNew()
        {
            return new StiDB2Source();
        }
        #endregion

		public StiDB2Source() : this("", "", "")
		{
		}

		public StiDB2Source(string dataName, string name) : this(dataName, name, name)
		{
		}

		public StiDB2Source(string dataName, string name, string alias) : this(dataName, name, alias, string.Empty)
		{
		}

		public StiDB2Source(string dataName, string name, string alias, string sqlCommand) : 
			base(dataName, name, alias, sqlCommand)			
		{
		}

		public StiDB2Source(string dataName, string name, string alias, string sqlCommand, 
			bool connectOnStart) : 
		base(dataName, name, alias, sqlCommand, connectOnStart)
		{
		}

		public StiDB2Source(string dataName, string name, string alias, string sqlCommand, 
			bool connectOnStart, bool reconnectOnEachRow) : 
			base(dataName, name, alias, sqlCommand, connectOnStart, reconnectOnEachRow)
		{
		}

		public StiDB2Source(string dataName, string name, string alias, string sqlCommand, 
			bool connectOnStart, bool reconnectOnEachRow, int commandTimeout) : 
			base(dataName, name, alias, sqlCommand, connectOnStart, reconnectOnEachRow, commandTimeout)
		{
		}

        public StiDB2Source(string dataName, string name, string alias, string sqlCommand, 
            bool connectOnStart, bool reconnectOnEachRow, int commandTimeout, string key) :
            base(dataName, name, alias, sqlCommand, connectOnStart, reconnectOnEachRow, commandTimeout, key)
        {
        }
	}
}