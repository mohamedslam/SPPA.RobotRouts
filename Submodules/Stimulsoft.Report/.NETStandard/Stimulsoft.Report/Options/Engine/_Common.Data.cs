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
using Stimulsoft.Base;
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Serializing;

namespace Stimulsoft.Report
{
    /// <summary>
    /// Class for adjustment all aspects of Stimulsoft Reports.
    /// </summary>
    public sealed partial class StiOptions
	{
        /// <summary>
        /// A class which controls of settings of the report engine. 
        /// </summary>
		public sealed partial class Engine
        {
            /// <summary>
            /// If true then database information will be required from database with help of DataAdapter (instead Connection.GetSchema).
            /// </summary>
            [DefaultValue(false)]
            [Description("If true then database information will be required from database with help of DataAdapter (instead Connection.GetSchema).")]
            [StiSerializable]
            [Category("Data")]
            public static bool AdvancedRetrievalModeOfDatabaseInformation
            {
                get
                {
                    return StiDataConnector.AdvancedRetrievalModeOfDatabaseSchema;
                }
                set
                {
                    StiDataConnector.AdvancedRetrievalModeOfDatabaseSchema = value;
                }
            }

            /// <summary>
            /// If true then in GetSchema method, column reference by index else by name .
            /// </summary>
            [DefaultValue(true)]
            [Description("If true then in GetSchema method, column reference by index else by name.")]
            [StiSerializable]
            [Category("Data")]
            public static bool GetSchemaColumnsMode
            {
                get
                {
                    return StiDataConnector.GetSchemaColumnsMode;
                }
                set
                {
                    StiDataConnector.GetSchemaColumnsMode = value;
                }
            }

            /// <summary>
            /// StiOptions.Engine.RetriveColumnsMode was obsoleted. Please use StiOptions.Engine.RetrieveColumnsMode.
            /// </summary>
            [DefaultValue(StiRetrieveColumnsMode.SchemaOnly)]
            [Description("StiOptions.Engine.RetriveColumnsMode was obsoleted. Please use StiOptions.Engine.RetrieveColumnsMode.")]
            [StiSerializable]
            [Category("Data")]
            [Obsolete("StiOptions.Engine.RetriveColumnsMode was obsoleted. Please use StiOptions.Engine.RetrieveColumnsMode.")]
            [EditorBrowsable(EditorBrowsableState.Never)]
            [Browsable(false)]
            public static StiRetrieveColumnsMode RetriveColumnsMode
            {
                get
                {
                    return RetrieveColumnsMode;
                }
                set
                {
                    RetrieveColumnsMode = value; 
                }
            }

            /// <summary>
            /// Value which describes mode of retrieving database information.
            /// </summary>
            [DefaultValue(StiRetrieveColumnsMode.SchemaOnly)]
            [Description("Value which describes mode of retrieving database information.")]
            [StiSerializable]
            [Category("Data")]
            public static StiRetrieveColumnsMode RetrieveColumnsMode
            {
                get
                {
                    return StiDataOptions.RetrieveColumnsMode;
                }
                set
                {
                    StiDataOptions.RetrieveColumnsMode = value;
                }
            }

            /// <summary>
            /// StiOptions.Engine.WizardStoredProcRetriveMode was obsoleted. Please use StiOptions.Engine.WizardStoredProcRetrieveMode.
            /// </summary>
            [DefaultValue(StiWizardStoredProcRetriveMode.All)]
            [Description("StiOptions.Engine.WizardStoredProcRetriveMode was obsoleted. Please use StiOptions.Engine.WizardStoredProcRetrieveMode.")]
            [StiSerializable]
            [Category("Data")]
            [Browsable(false)]
            [Obsolete("StiOptions.Engine.WizardStoredProcRetriveMode was obsoleted. Please use StiOptions.Engine.WizardStoredProcRetrieveMode.")]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public static StiWizardStoredProcRetriveMode WizardStoredProcRetriveMode
            {
                get
                {
                    return (StiWizardStoredProcRetriveMode)WizardStoredProcRetrieveMode;
                }
                set
                {
                    WizardStoredProcRetrieveMode = (StiWizardStoredProcRetrieveMode)value;
                }
            }

            /// <summary>
            /// Value which describes mode of retrieving information for stored procedures.
            /// </summary>
            [DefaultValue(StiWizardStoredProcRetrieveMode.All)]
            [Description("Value which describes mode of retrieving information for stored procedures.")]
            [StiSerializable]
            [Category("Data")]
            public static StiWizardStoredProcRetrieveMode WizardStoredProcRetrieveMode
            {
                get
                {
                    return StiDataOptions.WizardStoredProcRetrieveMode;
                }
                set
                {
                    StiDataOptions.WizardStoredProcRetrieveMode = value;
                }
            }

            /// <summary>
            /// If true then will be call Prepare() method for queries for each run.
            /// </summary>
            [DefaultValue(true)]
            [Description("If true then will be call Prepare() method for queries for each run.")]
            [StiSerializable]
            [Category("Data")]
            public static bool AllowPrepareSqlQueries { get; set; } = true;

            /// <summary>
            /// Gets or sets a value, which indicates that if data are absent then controls of the data emulation. 
            /// This value is used for report rendering in the designer without data.
            /// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets a value, which indicates that if data are absent then controls of the data emulation. This value is used for report rendering in the designer without data.")]
            [StiSerializable]
            [Category("Designer")]
            public static bool EmulateData { get; set; }

            /// <summary>
            /// If true then exception will be generated if column does not exists.
            /// </summary>
            [DefaultValue(false)]
            [Description("If true then exception will be generated if column does not exists.")]
            [StiSerializable]
            [Category("Data")]
            public static bool AllowThrowExceptionWhenColumnDoesNotExists { get; set; }

            /// <summary>
            /// If true then filter data in DataSource before sorting. For backward compatibility with 2016.1.
            /// </summary>
            [DefaultValue(true)]
            [Description("If true then filter data in DataSource before sorting. For backward compatibility with 2016.1.")]
            [StiSerializable]
            [Category("Data")]
            public static bool FilterDataInDataSourceBeforeSorting { get; set; } = true;

            /// <summary>
            /// Reconnect DataSources used in the RequestFromUser variables if value of the variable was changed.
            /// </summary>
            [DefaultValue(false)]
            [Description("Reconnect DataSources used in the RequestFromUser variables if value of the variable was changed.")]
            [StiSerializable]
            [Category("Data")]
            public static bool ReconnectDataSourcesIfRequestFromUserVariableChanged { get; set; }

            /// <summary>
            /// If true then remove double call of SetData method for detail bands. For backward compatibility with 2016.1.
            /// </summary>
            [DefaultValue(true)]
            [Description("If true then remove double call of SetData method for detail bands. For backward compatibility with 2016.1.")]
            [StiSerializable]
            [Category("Data")]
            public static bool OptimizeDetailDataFiltering { get; set; } = true;

            /// <summary>
            /// Set schema name for retrieve information for PostgreSql.
            /// </summary>
            [DefaultValue(true)]
            [Description("Set schema name for retrieve information for PostgreSql")]
            [StiSerializable]
            [Category("Data")]
            public static string RetrieveSchemaNamePostgreSql
            {
                get
                {
                    return StiBaseOptions.RetrieveSchemaNamePostgreSql;
                }
                set
                {
                    StiBaseOptions.RetrieveSchemaNamePostgreSql = value;
                }
            }

            /// <summary>
            /// If true then call the DisconnectFromData method before the EndRender event. For backward compatibility with 2016.3.
            /// </summary>
            [DefaultValue(false)]
            [Description("If true then call the DisconnectFromData method before the EndRender event. For backward compatibility with 2016.3.")]
            [StiSerializable]
            [Category("Data")]
            public static bool DisconnectFromDataBeforeEndRender { get; set; }

        }
    }
}