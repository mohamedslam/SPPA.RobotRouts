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

namespace Stimulsoft.Report.Dictionary
{
	/// <summary>
	/// Describes the Data Source realizing access to OleDb.
	/// </summary>
	[TypeConverter(typeof(Stimulsoft.Report.Dictionary.Design.StiSqlSourceConverter))]
	public class StiOleDbSource : StiSqlSource
    {
        #region Methods
        /// <summary>
        /// Returns new data connector for this datasource.
        /// </summary>
        /// <returns>Created connector.</returns>
        public override StiSqlDataConnector CreateConnector(string connectionString = null)
        {
            return StiOleDbConnector.Get(connectionString);
        }

        protected override Type GetDataAdapterType()
        {
            return typeof(StiOleDbAdapterService);
        }
        #endregion

        #region Methods.override
        public override StiComponentId ComponentId => StiComponentId.StiOleDbSource;

        public override StiDataSource CreateNew()
        {
            return new StiOleDbSource();
        }
        #endregion

		/// <summary>
		/// Creates a new object of the type StiDataViewSource.
		/// </summary>
		public StiOleDbSource() : this("", "", "")
		{
		}

		/// <summary>
		/// Creates a new object of the type StiOleDbSource.
		/// </summary>
		/// <param name="nameInSource">Name of OleDbConnection in the DataStore.</param>
		/// <param name="name">Data Source name.</param>
		public StiOleDbSource(string nameInSource, string name) : this(nameInSource, name, name)
		{
		}

		/// <summary>
		/// Creates a new object of the type StiOleDbSource.
		/// </summary>
		/// <param name="nameInSource">Name of OleDbConnection in the DataStore.</param>
		/// <param name="name">Data Source name.</param>
		/// <param name="alias">Data Source alias.</param>
		public StiOleDbSource(string nameInSource, string name, string alias) : this(nameInSource, name, alias, string.Empty)
		{
		}

		/// <summary>
		/// Creates a new object of the type StiOleDbSource.
		/// </summary>
		/// <param name="nameInSource">Name of OleDbConnection in the DataStore.</param>
		/// <param name="name">Data Source name.</param>
		/// <param name="alias">Data Source alias.</param>
		/// <param name="sqlCommand">Transact-SQL statement to execute at the Data Source.</param>
		public StiOleDbSource(string nameInSource, string name, string alias, string sqlCommand) : 
			base(nameInSource, name, alias, sqlCommand)			
		{
		}

		public StiOleDbSource(string nameInSource, string name, string alias, string sqlCommand, 
			bool connectOnStart) : 
			base(nameInSource, name, alias, sqlCommand, connectOnStart)
		{
		}

		public StiOleDbSource(string nameInSource, string name, string alias, string sqlCommand, 
			bool connectOnStart, bool reconnectOnEachRow) : 
		base(nameInSource, name, alias, sqlCommand, connectOnStart, reconnectOnEachRow)
		{
		}

		public StiOleDbSource(string nameInSource, string name, string alias, string sqlCommand, 
			bool connectOnStart, bool reconnectOnEachRow, int commandTimeout) : 
			base(nameInSource, name, alias, sqlCommand, connectOnStart, reconnectOnEachRow, commandTimeout)
		{
		}

        public StiOleDbSource(string nameInSource, string name, string alias, string sqlCommand, bool connectOnStart, bool reconnectOnEachRow, int commandTimeout, string key) :
            base(nameInSource, name, alias, sqlCommand, connectOnStart, reconnectOnEachRow, commandTimeout, key)
        {
        }		
	}
}