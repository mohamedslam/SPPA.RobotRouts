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

namespace Stimulsoft.Report.Dictionary
{
	public class StiUndefinedDataSource : StiSqlSource
    {
        #region Properties
        public override StiComponentId ComponentId => StiComponentId.StiUndefinedDataSource;
        #endregion

        #region Methods.override
        public override StiDataSource CreateNew()
        {
            return new StiUndefinedDataSource();
        }
        #endregion

		/// <summary>
		/// Creates a new object of the type StiUndefinedDataSource.
		/// </summary>
		public StiUndefinedDataSource() : this("", "", "")
		{
		}

		/// <summary>
		/// Creates a new object of the type StiUndefinedDataSource.
		/// </summary>
		/// <param name="nameInSource">Name of Sql Connection in the DataStore.</param>
		/// <param name="name">Data Source name.</param>
		public StiUndefinedDataSource(string nameInSource, string name) : this(nameInSource, name, name)
		{
		}

		/// <summary>
		/// Creates a new object of the type StiSqlSource.
		/// </summary>
		/// <param name="nameInSource">Name of Sql Connection in the DataStore.</param>
		/// <param name="name">Data Source name.</param>
		/// <param name="alias">Data Source alias.</param>
		public StiUndefinedDataSource(string nameInSource, string name, string alias) : this(nameInSource, name, alias, string.Empty)
		{
		}

		/// <summary>
		/// Creates a new object of the type StiSqlSource.
		/// </summary>
		/// <param name="nameInSource">Name of SqlConnection in the DataStore.</param>
		/// <param name="name">Data Source name.</param>
		/// <param name="alias">Data Source alias.</param>
		/// <param name="sqlCommand">SQL statement to execute at the Data Source.</param>
		public StiUndefinedDataSource(string nameInSource, string name, string alias, string sqlCommand) : 
			this(nameInSource, name, alias, sqlCommand, true)
		{
		}

		public StiUndefinedDataSource(string nameInSource, string name, string alias, string sqlCommand, 
			bool connectOnStart) : 
			this(nameInSource, name, alias, sqlCommand, connectOnStart, false)
		{
		}

        public StiUndefinedDataSource(string nameInSource, string name, string alias, string sqlCommand,
            bool connectOnStart, bool reconnectOnEachRow) :
            this(nameInSource, name, alias, sqlCommand, connectOnStart, reconnectOnEachRow, 30)
        {

        }

		public StiUndefinedDataSource(string nameInSource, string name, string alias, string sqlCommand, 
			bool connectOnStart, bool reconnectOnEachRow, int commandTimeout) : 
			base(nameInSource, name, alias, sqlCommand, 
			connectOnStart, reconnectOnEachRow, commandTimeout)
		{		
		}

        public StiUndefinedDataSource(string nameInSource, string name, string alias, string sqlCommand, bool connectOnStart, bool reconnectOnEachRow, int commandTimeout, string key) :
            base(nameInSource, name, alias, sqlCommand, connectOnStart, reconnectOnEachRow, commandTimeout, key)
        {
        }
	}
}