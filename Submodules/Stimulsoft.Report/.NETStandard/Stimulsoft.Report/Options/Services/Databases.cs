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
using Stimulsoft.Report.Dictionary.Databases.Azure;
using Stimulsoft.Report.Dictionary.Databases.Google;

namespace Stimulsoft.Report
{
    /// <summary>
    /// Class for adjustment all aspects of Stimulsoft Reports.
    /// </summary>
    public sealed partial class StiOptions
	{
        public sealed partial class Services
        {
            private static List<StiDatabase> databases;
            public static List<StiDatabase> Databases
            {
                get
                {
                    lock (lockObject)
                    {
                        return databases ?? (databases = new List<StiDatabase>
                        {
                            new StiDB2Database(),
                            new StiDotConnectUniversalDatabase(),
                            new StiFirebirdDatabase(),
                            new StiInformixDatabase(),
                            new StiMariaDbDatabase(),
                            new StiMySqlDatabase(),
                            new StiMSAccessDatabase(),
                            new StiOdbcDatabase(),
                            new StiOleDbDatabase(),
                            new StiOracleDatabase(),
                            new StiPostgreSQLDatabase(),
                            new StiSQLiteDatabase(),
                            new StiSqlDatabase(),
                            new StiSqlCeDatabase(),
                            new StiSybaseDatabase(),
                            new StiSybaseAdsDatabase(),
                            new StiTeradataDatabase(),
                            new StiVistaDBDatabase(),

                            new StiODataDatabase(),
                            new StiGraphQLDatabase(),
                            new StiAzureTableStorageDatabase(),
                            new StiCosmosDbDatabase(),
                            new StiMongoDbDatabase(),

                            new StiGoogleSheetsDatabase(),
                            new StiCsvDatabase(),
                            new StiDBaseDatabase(),
                            new StiExcelDatabase(),

                            new StiJsonDatabase(),
                            new StiXmlDatabase(),
                            new StiGisDatabase(),

                            new StiDataWorldDatabase(),
                            new StiQuickBooksDatabase(),
                            new StiFirebaseDatabase(),

                            new StiBigQueryDatabase(),
                            new StiGoogleAnalyticsDatabase(),
                            new StiAzureSqlDatabase(),
                            new StiAzureBlobStorageDatabase()
                        });
                    }
                }
            }
        }
	}
}