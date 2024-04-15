﻿#region Copyright (C) 2003-2022 Stimulsoft
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

using Stimulsoft.Report.Dictionary.Adapters.OnlineService;
using Stimulsoft.Report.Dictionary.Design;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stimulsoft.Report.Dictionary
{
    [TypeConverter(typeof(StiQuickBooksSourceConverter))]
    public class StiQuickBooksSource : StiSqlSource
    {
        #region Methods
        protected override Type GetDataAdapterType()
        {
            return typeof(StiQuickBooksAdapterService);
        }

        public override StiDataSource CreateNew()
        {
            return new StiQuickBooksSource();
        }
        #endregion

        #region Properties
        public override StiComponentId ComponentId => StiComponentId.StiQuickBooksSource;
        #endregion

        public StiQuickBooksSource()
            : this("", "", "")
        {
        }

        public StiQuickBooksSource(string nameInSource, string name)
            : this(nameInSource, name, name)
        {
        }

        public StiQuickBooksSource(string nameInSource, string name, string alias)
            : this(nameInSource, name, alias, string.Empty)
        {

        }

        public StiQuickBooksSource(string nameInSource, string name, string alias, string sqlCommand) :
            base(nameInSource, name, alias, sqlCommand)
        {
        }

        public StiQuickBooksSource(string nameInSource, string name, string alias, string sqlCommand,
            bool connectOnStart) :
            base(nameInSource, name, alias, sqlCommand, connectOnStart)
        {
        }

        public StiQuickBooksSource(string nameInSource, string name, string alias, string sqlCommand,
            bool connectOnStart, bool reconnectOnEachRow) :
            base(nameInSource, name, alias, sqlCommand, connectOnStart, reconnectOnEachRow)
        {
        }

        public StiQuickBooksSource(string nameInSource, string name, string alias, string sqlCommand,
            bool connectOnStart, bool reconnectOnEachRow, int commandTimeout) :
            base(nameInSource, name, alias, sqlCommand, connectOnStart, reconnectOnEachRow, commandTimeout)
        {
        }

        public StiQuickBooksSource(string nameInSource, string name, string alias, string sqlCommand,
            bool connectOnStart, bool reconnectOnEachRow, int commandTimeout, string key) :
            base(nameInSource, name, alias, sqlCommand, connectOnStart, reconnectOnEachRow, commandTimeout, key)
        {
        }
    }
}
