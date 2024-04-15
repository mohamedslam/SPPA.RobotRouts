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
using Stimulsoft.Base.Serializing;

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
			public sealed class DataAdapters
            {
                #region Obsolete
                /// <summary>
			    /// TryToLoadOracleClientAdapter property is obsolete. 
			    /// Starts from version 2015.2 data adapters fully integrated to the report engine. So you need only copy a .Net data provider assemblies to your application folder.
			    /// </summary>
			    [DefaultValue(false)]
			    [StiSerializable]
                [Obsolete("StiOptions.Dictionary.DataAdapters.TryToLoadOracleClientAdapter is obsolete. Starts from version 2015.2 data adapters fully integrated to the report engine. " +
                          "So you need only copy a .Net data provider assemblies to your application folder.")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static bool TryToLoadOracleClientAdapter { get; set; }

                /// <summary>
                /// TryToLoadOracleODPAdapter property is obsolete. 
                /// Starts from version 2015.2 data adapters fully integrated to the report engine. So you need only copy a .Net data provider assemblies to your application folder.
                /// </summary>
                [DefaultValue(false)]
			    [StiSerializable]
                [Obsolete("StiOptions.Dictionary.DataAdapters.TryToLoadOracleODPAdapter is obsolete. Starts from version 2015.2 data adapters fully integrated to the report engine. " +
                          "So you need only copy a .Net data provider assemblies to your application folder.")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static bool TryToLoadOracleODPAdapter { get; set; }

                /// <summary>
                /// TryToLoadOracleODPManagedAdapter property is obsolete. 
                /// Starts from version 2015.2 data adapters fully integrated to the report engine. So you need only copy a .Net data provider assemblies to your application folder.
                /// </summary>
                [DefaultValue(false)]
			    [StiSerializable]
                [Obsolete("StiOptions.Dictionary.DataAdapters.TryToLoadOracleODPManagedAdapter is obsolete. Starts from version 2015.2 data adapters fully integrated to the report engine. " +
                          "So you need only copy a .Net data provider assemblies to your application folder.")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static bool TryToLoadOracleODPManagedAdapter { get; set; }

                /// <summary>
                /// TryToLoadFirebirdAdapter property is obsolete. 
                /// Starts from version 2015.2 data adapters fully integrated to the report engine. So you need only copy a .Net data provider assemblies to your application folder.
                /// </summary>
                [DefaultValue(false)]
			    [StiSerializable]
                [Obsolete("StiOptions.Dictionary.DataAdapters.TryToLoadFirebirdAdapter is obsolete. Starts from version 2015.2 data adapters fully integrated to the report engine. " +
                          "So you need only copy a .Net data provider assemblies to your application folder.")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static bool TryToLoadFirebirdAdapter { get; set; }

                /// <summary>
                /// TryToLoadMySqlAdapter property is obsolete. 
                /// Starts from version 2015.2 data adapters fully integrated to the report engine. So you need only copy a .Net data provider assemblies to your application folder.
                /// </summary>
                [DefaultValue(false)]
			    [StiSerializable]
                [Obsolete("StiOptions.Dictionary.DataAdapters.TryToLoadMySqlAdapter is obsolete. Starts from version 2015.2 data adapters fully integrated to the report engine. " +
                          "So you need only copy a .Net data provider assemblies to your application folder.")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static bool TryToLoadMySqlAdapter { get; set; }

                /// <summary>
                /// TryToLoadDotConnectUniversalAdapter property is obsolete. 
                /// Starts from version 2015.2 data adapters fully integrated to the report engine. So you need only copy a .Net data provider assemblies to your application folder.
                /// </summary>
                [DefaultValue(false)]
			    [StiSerializable]
                [Obsolete("StiOptions.Dictionary.DataAdapters.TryToLoadDotConnectUniversalAdapter is obsolete. Starts from version 2015.2 data adapters fully integrated to the report engine. " +
                          "So you need only copy a .Net data provider assemblies to your application folder.")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static bool TryToLoadDotConnectUniversalAdapter { get; set; }

                /// <summary>
                /// TryToLoadDotConnectOracleAdapter property is obsolete. 
                /// Starts from version 2015.2 data adapters fully integrated to the report engine. So you need only copy a .Net data provider assemblies to your application folder.
                /// </summary>
                [DefaultValue(false)]
			    [StiSerializable]
                [Obsolete("StiOptions.Dictionary.DataAdapters.TryToLoadDotConnectOracleAdapter is obsolete. Starts from version 2015.2 data adapters fully integrated to the report engine. " +
                          "So you need only copy a .Net data provider assemblies to your application folder.")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static bool TryToLoadDotConnectOracleAdapter { get; set; }

                /// <summary>
                /// TryToLoadPostgreSQLAdapter property is obsolete. 
                /// Starts from version 2015.2 data adapters fully integrated to the report engine. So you need only copy a .Net data provider assemblies to your application folder.
                /// </summary>
                [DefaultValue(false)]
			    [StiSerializable]
                [Obsolete("StiOptions.Dictionary.DataAdapters.TryToLoadPostgreSQLAdapter is obsolete. Starts from version 2015.2 data adapters fully integrated to the report engine. " +
                          "So you need only copy a .Net data provider assemblies to your application folder.")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static bool TryToLoadPostgreSQLAdapter { get; set; }

                /// <summary>
                /// TryToLoadVistaDBAdapter property is obsolete. 
                /// Starts from version 2015.2 data adapters fully integrated to the report engine. So you need only copy a .Net data provider assemblies to your application folder.
                /// </summary>
                [DefaultValue(false)]
			    [StiSerializable]
                [Obsolete("StiOptions.Dictionary.DataAdapters.TryToLoadVistaDBAdapter is obsolete. Starts from version 2015.2 data adapters fully integrated to the report engine. " +
                          "So you need only copy a .Net data provider assemblies to your application folder.")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static bool TryToLoadVistaDBAdapter { get; set; }

                /// <summary>
                /// TryToLoadSqlCeAdapter property is obsolete. 
                /// Starts from version 2015.2 data adapters fully integrated to the report engine. So you need only copy a .Net data provider assemblies to your application folder.
                /// </summary>
                [DefaultValue(false)]
			    [StiSerializable]
                [Obsolete("StiOptions.Dictionary.DataAdapters.TryToLoadSqlCeAdapter is obsolete. Starts from version 2015.2 data adapters fully integrated to the report engine. " +
                          "So you need only copy a .Net data provider assemblies to your application folder.")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static bool TryToLoadSqlCeAdapter { get; set; }

                /// <summary>
                /// TryToLoadSQLiteAdapter property is obsolete. 
                /// Starts from version 2015.2 data adapters fully integrated to the report engine. So you need only copy a .Net data provider assemblies to your application folder.
                /// </summary>
                [DefaultValue(false)]
			    [StiSerializable]
                [Obsolete("StiOptions.Dictionary.DataAdapters.TryToLoadSQLiteAdapter is obsolete. Starts from version 2015.2 data adapters fully integrated to the report engine. " +
                          "So you need only copy a .Net data provider assemblies to your application folder.")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static bool TryToLoadSQLiteAdapter { get; set; }

                /// <summary>
                /// TryToLoadDB2Adapter property is obsolete. 
                /// Starts from version 2015.2 data adapters fully integrated to the report engine. So you need only copy a .Net data provider assemblies to your application folder.
                /// </summary>
                [DefaultValue(false)]
			    [StiSerializable]
                [Obsolete("StiOptions.Dictionary.DataAdapters.TryToLoadDB2Adapter is obsolete. Starts from version 2015.2 data adapters fully integrated to the report engine. " +
                          "So you need only copy a .Net data provider assemblies to your application folder.")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static bool TryToLoadDB2Adapter { get; set; }

                /// <summary>
                /// TryToLoadUniDirectAdapter property is obsolete. 
                /// Starts from version 2015.2 data adapters fully integrated to the report engine. So you need only copy a .Net data provider assemblies to your application folder.
                /// </summary>
                [DefaultValue(false)]
			    [StiSerializable]
                [Obsolete("StiOptions.Dictionary.DataAdapters.TryToLoadUniDirectAdapter is obsolete. Starts from version 2015.2 data adapters fully integrated to the report engine. " +
                          "So you need only copy a .Net data provider assemblies to your application folder.")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static bool TryToLoadUniDirectAdapter { get; set; }

                /// <summary>
                /// TryToLoadSybaseADSAdapter property is obsolete. 
                /// Starts from version 2015.2 data adapters fully integrated to the report engine. So you need only copy a .Net data provider assemblies to your application folder.
                /// </summary>
                [DefaultValue(false)]
			    [StiSerializable]
                [Obsolete("StiOptions.Dictionary.DataAdapters.TryToLoadSybaseADSAdapter is obsolete. Starts from version 2015.2 data adapters fully integrated to the report engine. " +
                          "So you need only copy a .Net data provider assemblies to your application folder.")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static bool TryToLoadSybaseADSAdapter { get; set; }

                /// <summary>
                /// TryToLoadSybaseASEAdapter property is obsolete. 
                /// Starts from version 2015.2 data adapters fully integrated to the report engine. So you need only copy a .Net data provider assemblies to your application folder.
                /// </summary>
                [DefaultValue(false)]
			    [StiSerializable]
                [Obsolete("StiOptions.Dictionary.DataAdapters.TryToLoadSybaseASEAdapter is obsolete. Starts from version 2015.2 data adapters fully integrated to the report engine. " +
                          "So you need only copy a .Net data provider assemblies to your application folder.")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static bool TryToLoadSybaseASEAdapter { get; set; }

                /// <summary>
                /// TryToLoadInformixAdapter property is obsolete. 
                /// Starts from version 2015.2 data adapters fully integrated to the report engine. So you need only copy a .Net data provider assemblies to your application folder.
                /// </summary>
                [DefaultValue(false)]
			    [StiSerializable]
                [Obsolete("StiOptions.Dictionary.DataAdapters.TryToLoadInformixAdapter is obsolete. Starts from version 2015.2 data adapters fully integrated to the report engine. " +
                          "So you need only copy a .Net data provider assemblies to your application folder.")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static bool TryToLoadInformixAdapter { get; set; }

                /// <summary>
                /// TryToLoadEffiProzAdapter property is obsolete. 
                /// Starts from version 2015.2 data adapters fully integrated to the report engine. So you need only copy a .Net data provider assemblies to your application folder.
                /// </summary>
                [DefaultValue(false)]
			    [StiSerializable]
                [Obsolete("StiOptions.Dictionary.DataAdapters.TryToLoadEffiProzAdapter is obsolete. Starts from version 2015.2 data adapters fully integrated to the report engine. " +
                          "So you need only copy a .Net data provider assemblies to your application folder.")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static bool TryToLoadEffiProzAdapter { get; set; }

                /// <summary>
                /// TryToLoadTeradataAdapter property is obsolete. 
                /// Starts from version 2015.2 data adapters fully integrated to the report engine. So you need only copy a .Net data provider assemblies to your application folder.
                /// </summary>
                [DefaultValue(false)]
			    [StiSerializable]
                [Obsolete("StiOptions.Dictionary.DataAdapters.TryToLoadTeradataAdapter is obsolete. Starts from version 2015.2 data adapters fully integrated to the report engine. " +
                          "So you need only copy a .Net data provider assemblies to your application folder.")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static bool TryToLoadTeradataAdapter { get; set; }

                /// <summary>
                /// TryToLoadSybaseSAAdapter property is obsolete. 
                /// Starts from version 2015.2 data adapters fully integrated to the report engine. So you need only copy a .Net data provider assemblies to your application folder.
                /// </summary>
                [DefaultValue(false)]
			    [StiSerializable]
                [Obsolete("StiOptions.Dictionary.DataAdapters.TryToLoadSybaseSAAdapter is obsolete. Starts from version 2015.2 data adapters fully integrated to the report engine. " +
                          "So you need only copy a .Net data provider assemblies to your application folder.")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static bool TryToLoadSybaseSAAdapter;
                #endregion
            }
		}		
    }
}