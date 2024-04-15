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
using Stimulsoft.Report.Dictionary.Design;
using System;
using System.ComponentModel;

namespace Stimulsoft.Report.Dictionary
{
    [TypeConverter(typeof(StiSqlSourceConverter))]
	public class StiSybaseSource : StiSqlSource
	{
		#region Methods
        /// <summary>
        /// Returns new data connector for this datasource.
        /// </summary>
        /// <returns>Created connector.</returns>
        public override StiSqlDataConnector CreateConnector(string connectionString = null)
        {
            return StiSybaseConnector.Get(connectionString);
        }

        protected override Type GetDataAdapterType()
        {
            return typeof(StiSybaseAdapterService);
        }
        #endregion

        #region Methods.override
        public override StiComponentId ComponentId => StiComponentId.StiSybaseSource;

        public override StiDataSource CreateNew()
        {
            return new StiSybaseSource();
        }
        #endregion

		public StiSybaseSource() : this("", "", "")
		{
		}

		public StiSybaseSource(string nameInSource, string name) : this(nameInSource, name, name)
		{
		}

		public StiSybaseSource(string nameInSource, string name, string alias) : this(nameInSource, name, alias, string.Empty)
		{

		}

		public StiSybaseSource(string nameInSource, string name, string alias, string sqlCommand) : 
			base(nameInSource, name, alias, sqlCommand)			
		{
		}

		public StiSybaseSource(string nameInSource, string name, string alias, string sqlCommand, 
			bool connectOnStart) : 
		base(nameInSource, name, alias, sqlCommand, connectOnStart)
		{
		}

		public StiSybaseSource(string nameInSource, string name, string alias, string sqlCommand, 
			bool connectOnStart, bool reconnectOnEachRow) : 
			base(nameInSource, name, alias, sqlCommand, connectOnStart, reconnectOnEachRow)
		{
		}

		public StiSybaseSource(string nameInSource, string name, string alias, string sqlCommand, 
			bool connectOnStart, bool reconnectOnEachRow, int commandTimeout) : 
			base(nameInSource, name, alias, sqlCommand, connectOnStart, reconnectOnEachRow, commandTimeout)
		{
		}

        public StiSybaseSource(string nameInSource, string name, string alias, string sqlCommand,
            bool connectOnStart, bool reconnectOnEachRow, int commandTimeout, string key) :
            base(nameInSource, name, alias, sqlCommand, connectOnStart, reconnectOnEachRow, commandTimeout, key)
        {
        }
	}
}