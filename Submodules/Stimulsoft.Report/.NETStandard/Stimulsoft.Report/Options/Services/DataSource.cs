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

using System.Collections.Generic;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Dictionary.DataSources.Google;

namespace Stimulsoft.Report
{
    /// <summary>
    /// Class for adjustment all aspects of Stimulsoft Reports.
    /// </summary>
    public sealed partial class StiOptions
	{
        public sealed partial class Services
        {
            private static List<StiDataSource> dataSource;
            public static List<StiDataSource> DataSource
            {
                get
                {
                    lock (lockObject)
                    {
                        return dataSource ?? (dataSource = new List<StiDataSource>
                        {
                            new StiDataWorldSource(),
                            new StiDataTableSource(),
                            new StiDataViewSource(),
                            new StiUndefinedDataSource(),
                            new StiCsvSource(),
                            new StiDBaseSource(),
                            new StiBusinessObjectSource(),
                            new StiCrossTabDataSource(),
                            new StiEnumerableSource(),
                            new StiUserSource(),
                            new StiVirtualSource(),
                            new StiOracleODPSource(),
                            new StiDB2Source(),
                            new StiFirebirdSource(),
                            new StiInformixSource(),
                            new StiMariaDbSource(),
                            new StiMSAccessSource(),
                            new StiMySqlSource(),
                            new StiOdbcSource(),
                            new StiOleDbSource(),
                            new StiOracleSource(),
                            new StiPostgreSQLSource(),
                            new StiSqlCeSource(),
                            new StiSQLiteSource(),
                            new StiSqlSource(),
                            new StiMongoDbSource(),
                            new StiSybaseSource(),
                            new StiTeradataSource(),
                            new StiVistaDBSource(),
                            new StiODataSource(),
                            new StiGraphQLSource(),
                            new StiQuickBooksSource(),
                            new StiDataTransformation(),
                            new StiBigQuerySource(),
                            new StiAzureBlobStorageSource()
                        });
                    }
                }
            }
        }
	}
}