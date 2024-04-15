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

using System.ComponentModel;
using Stimulsoft.Report.Dictionary.Design;

namespace Stimulsoft.Report.Dictionary
{

    [TypeConverter(typeof(StiSqlSourceConverter))]
    public class StiOracleODPSource : StiOracleSource
    {
        #region Methods.override
        public override StiComponentId ComponentId
        {
            get
            {
                return StiComponentId.StiOracleODPSource;
            }
        }

        public override StiDataSource CreateNew()
        {
            return new StiOracleODPSource();
        }
        #endregion

        public StiOracleODPSource()
            : this("", "", "")
        {
        }

        public StiOracleODPSource(string nameInSource, string name)
            : this(nameInSource, name, name)
        {
        }

        public StiOracleODPSource(string nameInSource, string name, string alias)
            : this(nameInSource, name, alias, string.Empty)
        {

        }

        public StiOracleODPSource(string nameInSource, string name, string alias, string sqlCommand)
            :
            base(nameInSource, name, alias, sqlCommand)
        {
        }

        public StiOracleODPSource(string nameInSource, string name, string alias, string sqlCommand,
            bool connectOnStart)
            :
        base(nameInSource, name, alias, sqlCommand, connectOnStart)
        {
        }

        public StiOracleODPSource(string nameInSource, string name, string alias, string sqlCommand,
            bool connectOnStart, bool reconnectOnEachRow)
            :
        base(nameInSource, name, alias, sqlCommand, connectOnStart, reconnectOnEachRow)
        {
        }

        public StiOracleODPSource(string nameInSource, string name, string alias, string sqlCommand,
            bool connectOnStart, bool reconnectOnEachRow, int commandTimeout)
            :
            base(nameInSource, name, alias, sqlCommand, connectOnStart, reconnectOnEachRow, commandTimeout)
        {
        }

        public StiOracleODPSource(string nameInSource, string name, string alias, string sqlCommand,
           bool connectOnStart, bool reconnectOnEachRow, int commandTimeout, string key)
            :
            base(nameInSource, name, alias, sqlCommand, connectOnStart, reconnectOnEachRow, commandTimeout, key)
        {
        }
    }
}


