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
    [TypeConverter(typeof(StiMongoDbSourceConverter))]
    public class StiMongoDbSource : StiNoSqlSource
    {
        #region Methods
        /// <summary>
        /// Returns new data connector for this datasource.
        /// </summary>
        /// <returns>Created connector.</returns>
        public override StiSqlDataConnector CreateConnector(string connectionString = null)
        {
            throw new NotImplementedException();
        }

        protected override Type GetDataAdapterType()
        {
            return typeof(StiMongoDbAdapterService);
        }

        public override StiDataSource CreateNew()
        {
            return new StiMongoDbSource();
        }
        #endregion

        #region Properties
        public override StiComponentId ComponentId => StiComponentId.StiMongoDbSource;
        #endregion

        public StiMongoDbSource()
            : this("", "", "")
        {
        }

        public StiMongoDbSource(string nameInSource, string name)
            : this(nameInSource, name, name)
        {
        }

        public StiMongoDbSource(string nameInSource, string name, string alias)
            : this(nameInSource, name, alias, string.Empty)
        {

        }

        public StiMongoDbSource(string nameInSource, string name, string alias, string sqlCommand) :
            base(nameInSource, name, alias, sqlCommand)
        {
        }

        public StiMongoDbSource(string nameInSource, string name, string alias, string sqlCommand,
            bool connectOnStart) :
            base(nameInSource, name, alias, sqlCommand, connectOnStart)
        {
        }

        public StiMongoDbSource(string nameInSource, string name, string alias, string sqlCommand,
            bool connectOnStart, bool reconnectOnEachRow) :
            base(nameInSource, name, alias, sqlCommand, connectOnStart, reconnectOnEachRow)
        {
        }

        public StiMongoDbSource(string nameInSource, string name, string alias, string sqlCommand,
            bool connectOnStart, bool reconnectOnEachRow, int commandTimeout) :
            base(nameInSource, name, alias, sqlCommand, connectOnStart, reconnectOnEachRow, commandTimeout)
        {
        }

        public StiMongoDbSource(string nameInSource, string name, string alias, string sqlCommand,
            bool connectOnStart, bool reconnectOnEachRow, int commandTimeout, string key) :
            base(nameInSource, name, alias, sqlCommand, connectOnStart, reconnectOnEachRow, commandTimeout, key)
        {
        }
    }
}