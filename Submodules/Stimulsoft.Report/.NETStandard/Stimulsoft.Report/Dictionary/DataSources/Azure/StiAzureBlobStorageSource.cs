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

using Stimulsoft.Report.Dictionary.Design;
using System;
using System.ComponentModel;

namespace Stimulsoft.Report.Dictionary
{
    [TypeConverter(typeof(StiAzureBlobStorageSourceConverter))]
    public class StiAzureBlobStorageSource : StiNoSqlSource
    {
        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiAzureBlobStorageSource;
        #endregion IStiPropertyGridObject

        #region Methods
        protected override Type GetDataAdapterType()
        {
            return typeof(StiAzureBlobStorageAdapterService);
        }

        public override StiDataSource CreateNew()
        {
            return new StiAzureBlobStorageSource();
        }
        #endregion Methods

        public StiAzureBlobStorageSource()
            : this(string.Empty, string.Empty, string.Empty)
        {
        }

        public StiAzureBlobStorageSource(string nameInSource, string name) 
            : base(nameInSource, name)
        {
        }

        public StiAzureBlobStorageSource(string nameInSource, string name, string alias) 
            : base(nameInSource, name, alias)
        {
        }

        public StiAzureBlobStorageSource(string nameInSource, string name, string alias, string sqlCommand) 
            : base(nameInSource, name, alias, sqlCommand)
        {
        }

        public StiAzureBlobStorageSource(string nameInSource, string name, string alias, string sqlCommand, bool connectOnStart) 
            : base(nameInSource, name, alias, sqlCommand, connectOnStart)
        {
        }

        public StiAzureBlobStorageSource(string nameInSource, string name, string alias, string sqlCommand, bool connectOnStart, bool reconnectOnEachRow) 
            : base(nameInSource, name, alias, sqlCommand, connectOnStart, reconnectOnEachRow)
        {
        }

        public StiAzureBlobStorageSource(string nameInSource, string name, string alias, string sqlCommand, bool connectOnStart, bool reconnectOnEachRow, int commandTimeout) 
            : base(nameInSource, name, alias, sqlCommand, connectOnStart, reconnectOnEachRow, commandTimeout)
        {
        }

        public StiAzureBlobStorageSource(string nameInSource, string name, string alias, string sqlCommand, bool connectOnStart, bool reconnectOnEachRow, int commandTimeout, string key) 
            : base(nameInSource, name, alias, sqlCommand, connectOnStart, reconnectOnEachRow, commandTimeout, key)
        {
        }
    }
}
