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
using Stimulsoft.Report.Dictionary.Adapters.Azure;
using Stimulsoft.Report.Dictionary.Adapters.Google;
using Stimulsoft.Report.Dictionary.Adapters.OnlineService;

namespace Stimulsoft.Report
{
    /// <summary>
    /// Class for adjustment all aspects of Stimulsoft Reports.
    /// </summary>
    public sealed partial class StiOptions
	{
        public sealed partial class Services
        {
            private static List<StiDataAdapterService> dataAdapters;
            public static List<StiDataAdapterService> DataAdapters
            {
                get
                {
                    lock (lockObject)
                    {
                        return dataAdapters ?? (dataAdapters = new List<StiDataAdapterService>
                        {
                            new StiBusinessObjectAdapterService(),
                            new StiCrossTabAdapterService(),
                            new StiDataTableAdapterService(),
                            new StiDataViewAdapterService(),
                            new StiVirtualAdapterService(),
                            new StiUserAdapterService(),

                            new StiCsvAdapterService(),
                            new StiDBaseAdapterService(),

                            new StiDB2AdapterService(),
                            new StiDotConnectUniversalAdapterService(),
                            new StiFirebirdAdapterService(),
                            new StiInformixAdapterService(),
                            new StiMariaDbAdapterService(),
                            new StiMySqlAdapterService(),
                            new StiMSAccessAdapterService(),
                            new StiOdbcAdapterService(),
                            new StiOleDbAdapterService(),
                            new StiOracleAdapterService(),
                            new StiPostgreSQLAdapterService(),
                            new StiSQLiteAdapterService(),
                            new StiSqlAdapterService(),
                            new StiSqlCeAdapterService(),
                            new StiSybaseAdapterService(),
                            new StiSybaseAdsAdapterService(),
                            new StiTeradataAdapterService(),
                            new StiVistaDBAdapterService(),

                            new StiODataAdapterService(),
                            new StiGraphQLAdapterService(),
                            new StiMongoDbAdapterService(),
                            new StiGoogleSheetsAdapterService(),
                            new StiAzureTableStorageAdapterService(),
                            new StiAzureBlobStorageAdapterService(),
                            new StiCosmosDbAdapterService(),
                            new StiDataWorldAdapterService(),
                            new StiQuickBooksAdapterService(),
                            new StiFirebaseAdapterService(),
                            new StiBigQueryAdapterService(),
                            new StiGoogleAnalyticsAdapterService()
                        });
                    }
                }
            }
        }
	}
}