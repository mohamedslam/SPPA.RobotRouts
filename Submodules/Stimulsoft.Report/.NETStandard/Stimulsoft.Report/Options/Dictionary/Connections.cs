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
using System.Linq;
using System.ComponentModel;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Dictionary;

namespace Stimulsoft.Report
{
    /// <summary>
    /// Class for adjustment all aspects of Stimulsoft Reports.
    /// </summary>
    public sealed partial class StiOptions
	{
        /// <summary>
        /// A class which controls settings of a dictionary in the report.
        /// </summary>
        public sealed partial class Dictionary
		{
			public sealed class Connections
            {
                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowCsv
                {
                    get
                    {
                        return Services.DataAdapters.Any(s => s.ServiceEnabled && s is StiCsvAdapterService);
                    }
                    set
                    {
                        Services.DataAdapters.Where(s => s is StiCsvAdapterService).ToList().ForEach(s => s.ServiceEnabled = value);
                        Services.Databases.Where(s => s is StiCsvDatabase).ToList().ForEach(s => s.ServiceEnabled = value);
                    }
                }

                [DefaultValue(true)]
			    [StiSerializable]
			    public static bool ShowDb2
			    {
			        get
			        {
			            return Services.DataAdapters.Any(s => s.ServiceEnabled && s is StiDB2AdapterService);
			        }
			        set
			        {
			            Services.DataAdapters.Where(s => s is StiDB2AdapterService).ToList().ForEach(s => s.ServiceEnabled = value);
                        Services.Databases.Where(s => s is StiDB2Database).ToList().ForEach(s => s.ServiceEnabled = value);
			        }
			    }

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowDBase
                {
                    get
                    {
                        return Services.DataAdapters.Any(s => s.ServiceEnabled && s is StiDBaseAdapterService);
                    }
                    set
                    {
                        Services.DataAdapters.Where(s => s is StiDBaseAdapterService).ToList().ForEach(s => s.ServiceEnabled = value);
                        Services.Databases.Where(s => s is StiDBaseDatabase).ToList().ForEach(s => s.ServiceEnabled = value);
                    }
                }

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowDotConnectUniversal
                {
                    get
                    {
                        return Services.DataAdapters.Any(s => s.ServiceEnabled && s is StiDotConnectUniversalAdapterService);
                    }
                    set
                    {
                        Services.DataAdapters.Where(s => s is StiDotConnectUniversalAdapterService).ToList().ForEach(s => s.ServiceEnabled = value);
                        Services.Databases.Where(s => s is StiDotConnectUniversalDatabase).ToList().ForEach(s => s.ServiceEnabled = value);
                    }
                }

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowFirebird
                {
                    get
                    {
                        return Services.DataAdapters.Any(s => s.ServiceEnabled && s is StiFirebirdAdapterService);
                    }
                    set
                    {
                        Services.DataAdapters.Where(s => s is StiFirebirdAdapterService).ToList().ForEach(s => s.ServiceEnabled = value);
                        Services.Databases.Where(s => s is StiFirebirdDatabase).ToList().ForEach(s => s.ServiceEnabled = value);
                    }
                }

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowInformix
                {
                    get
                    {
                        return Services.DataAdapters.Any(s => s.ServiceEnabled && s is StiInformixAdapterService);
                    }
                    set
                    {
                        Services.DataAdapters.Where(s => s is StiInformixAdapterService).ToList().ForEach(s => s.ServiceEnabled = value);
                        Services.Databases.Where(s => s is StiInformixDatabase).ToList().ForEach(s => s.ServiceEnabled = value);
                    }
                }

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowMySql
                {
                    get
                    {
                        return Services.DataAdapters.Any(s => s.ServiceEnabled && s is StiMySqlAdapterService);
                    }
                    set
                    {
                        Services.DataAdapters.Where(s => s is StiMySqlAdapterService).ToList().ForEach(s => s.ServiceEnabled = value);
                        Services.Databases.Where(s => s is StiMySqlDatabase).ToList().ForEach(s => s.ServiceEnabled = value);
                    }
                }

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowMongoDb
                {
                    get
                    {
                        return Services.DataAdapters.Any(s => s.ServiceEnabled && s is StiMongoDbAdapterService);
                    }
                    set
                    {
                        Services.DataAdapters.Where(s => s is StiMongoDbAdapterService).ToList().ForEach(s => s.ServiceEnabled = value);
                        Services.Databases.Where(s => s is StiMongoDbDatabase).ToList().ForEach(s => s.ServiceEnabled = value);
                    }
                }

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowMsAccess
                {
                    get
                    {
                        return Services.DataAdapters.Any(s => s.ServiceEnabled && s is StiMSAccessAdapterService);
                    }
                    set
                    {
                        Services.DataAdapters.Where(s => s is StiMSAccessAdapterService).ToList().ForEach(s => s.ServiceEnabled = value);
                        Services.Databases.Where(s => s is StiMSAccessDatabase).ToList().ForEach(s => s.ServiceEnabled = value);
                    }
                }

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowOdbc
                {
                    get
                    {
                        return Services.DataAdapters.Any(s => s.ServiceEnabled && s is StiOdbcAdapterService);
                    }
                    set
                    {
                        Services.DataAdapters.Where(s => s is StiOdbcAdapterService).ToList().ForEach(s => s.ServiceEnabled = value);
                        Services.Databases.Where(s => s is StiOdbcDatabase).ToList().ForEach(s => s.ServiceEnabled = value);
                    }
                }

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowOleDb
                {
                    get
                    {
                        return Services.DataAdapters.Any(s => s.ServiceEnabled && s is StiOleDbAdapterService);
                    }
                    set
                    {
                        Services.DataAdapters.Where(s => s is StiOleDbAdapterService).ToList().ForEach(s => s.ServiceEnabled = value);
                        Services.Databases.Where(s => s is StiOleDbDatabase).ToList().ForEach(s => s.ServiceEnabled = value);
                    }
                }

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowOracle
                {
                    get
                    {
                        return Services.DataAdapters.Any(s => s.ServiceEnabled && s is StiOracleAdapterService);
                    }
                    set
                    {
                        Services.DataAdapters.Where(s => s is StiOracleAdapterService).ToList().ForEach(s => s.ServiceEnabled = value);
                        Services.Databases.Where(s => s is StiOracleDatabase).ToList().ForEach(s => s.ServiceEnabled = value);
                    }
                }

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowPostgreSql
                {
                    get
                    {
                        return Services.DataAdapters.Any(s => s.ServiceEnabled && s is StiPostgreSQLAdapterService);
                    }
                    set
                    {
                        Services.DataAdapters.Where(s => s is StiPostgreSQLAdapterService).ToList().ForEach(s => s.ServiceEnabled = value);
                        Services.Databases.Where(s => s is StiPostgreSQLDatabase).ToList().ForEach(s => s.ServiceEnabled = value);
                    }
                }

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowSqLite
                {
                    get
                    {
                        return Services.DataAdapters.Any(s => s.ServiceEnabled && s is StiSQLiteAdapterService);
                    }
                    set
                    {
                        Services.DataAdapters.Where(s => s is StiSQLiteAdapterService).ToList().ForEach(s => s.ServiceEnabled = value);
                        Services.Databases.Where(s => s is StiSQLiteDatabase).ToList().ForEach(s => s.ServiceEnabled = value);
                    }
                }

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowMsSql
                {
                    get
                    {
                        return Services.DataAdapters.Any(s => s.ServiceEnabled && s.GetType() == typeof(StiSqlAdapterService));
                    }
                    set
                    {
                        Services.DataAdapters.Where(s => s.GetType() == typeof(StiSqlAdapterService)).ToList().ForEach(s => s.ServiceEnabled = value);
                        Services.Databases.Where(s => s.GetType() == typeof(StiSqlDatabase)).ToList().ForEach(s => s.ServiceEnabled = value);
                    }
                }

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowSqlCe
                {
                    get
                    {
                        return Services.DataAdapters.Any(s => s.ServiceEnabled && s is StiSqlCeAdapterService);
                    }
                    set
                    {
                        Services.DataAdapters.Where(s => s is StiSqlCeAdapterService).ToList().ForEach(s => s.ServiceEnabled = value);
                        Services.Databases.Where(s => s is StiSqlCeDatabase).ToList().ForEach(s => s.ServiceEnabled = value);
                    }
                }

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowSybase
                {
                    get
                    {
                        return Services.DataAdapters.Any(s => s.ServiceEnabled && s is StiSybaseAdapterService);
                    }
                    set
                    {
                        Services.DataAdapters.Where(s => s is StiSybaseAdapterService).ToList().ForEach(s => s.ServiceEnabled = value);
                        Services.Databases.Where(s => s is StiSybaseDatabase).ToList().ForEach(s => s.ServiceEnabled = value);
                    }
                }

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowSybaseAds
                {
                    get
                    {
                        return Services.DataAdapters.Any(s => s.ServiceEnabled && s is StiSybaseAdsAdapterService);
                    }
                    set
                    {
                        Services.DataAdapters.Where(s => s is StiSybaseAdsAdapterService).ToList().ForEach(s => s.ServiceEnabled = value);
                        Services.Databases.Where(s => s is StiSybaseAdsDatabase).ToList().ForEach(s => s.ServiceEnabled = value);
                    }
                }

                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowTeradata
                {
                    get
                    {
                        return Services.DataAdapters.Any(s => s.ServiceEnabled && s is StiTeradataAdapterService);
                    }
                    set
                    {
                        Services.DataAdapters.Where(s => s is StiTeradataAdapterService).ToList().ForEach(s => s.ServiceEnabled = value);
                        Services.Databases.Where(s => s is StiTeradataDatabase).ToList().ForEach(s => s.ServiceEnabled = value);
                    }
                }
                
                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowVistaDB
                {
                    get
                    {
                        return Services.DataAdapters.Any(s => s.ServiceEnabled && s is StiVistaDBAdapterService);
                    }
                    set
                    {
                        Services.DataAdapters.Where(s => s is StiVistaDBAdapterService).ToList().ForEach(s => s.ServiceEnabled = value);
                        Services.Databases.Where(s => s is StiVistaDBDatabase).ToList().ForEach(s => s.ServiceEnabled = value);
                    }
                }
            }
		}		
    }
}