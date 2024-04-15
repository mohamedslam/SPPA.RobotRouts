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
            /// <summary>
            /// Gets or sets a value which controls synchronization of the Dictionary in automatic mode.
            /// </summary>
            [DefaultValue(StiAutoSynchronizeMode.Always)]
            [Description("Gets or sets a value which controls synchronization of the Dictionary in automatic mode.")]
            [StiSerializable]
            public static StiAutoSynchronizeMode AutoSynchronize { get; set; } = StiAutoSynchronizeMode.Always;

            [DefaultValue(false)]
            [StiSerializable]
            public static bool RemoveUnusedDataBeforeReportRendering { get; set; }

            public static Type DefaultDataAdapterType { get; set; }

            [DefaultValue(30)]
            [StiSerializable]
            public static int QueryBuilderConnectTimeout { get; set; } = 30;

            [DefaultValue(true)]
            [StiSerializable]
            public static bool UseAdvancedDataSearch { get; set; } = true;

            [DefaultValue(true)]
            [StiSerializable]
            public static bool AllowConnectToFirstTableForEmptyDataSource { get; set; } = true;

		    /// <summary>
		    /// Gets or sets a value which indicates that ConnectOnStart property will turn on second pass of report rendering automatically.
		    /// </summary>
		    [DefaultValue(false)]
		    [Description("Gets or sets a value which indicates that ConnectOnStart property will turn on second pass of report rendering automatically.")]
		    [StiSerializable]
		    public static bool EnableConnectOnStartOnSecondPass { get; set; }

            /// <summary>
            /// Gets or sets a value indicating that instead of a component name an alias will be shown.
            /// </summary>
            [DefaultValue(true)]
            [Description("Gets or sets a value indicating that instead of a component name an alias will be shown.")]
            [StiSerializable]
            public static bool ShowOnlyAliasForComponents { get; set; } = true;

            /// <summary>
            /// Gets or sets a value indicating that instead of a page name an alias will be shown.
            /// </summary>
            [DefaultValue(true)]
            [Description("Gets or sets a value indicating that instead of a page name an alias will be shown.")]
            [StiSerializable]
            public static bool ShowOnlyAliasForPages { get; set; } = true;

            /// <summary>
			/// Gets or sets a value indicating that instead of a database name an alias will be shown.
			/// </summary>
            [DefaultValue(true)]
            [Description("Gets or sets a value indicating that instead of a database name an alias will be shown.")]
            [StiSerializable]
            public static bool ShowOnlyAliasForDatabase { get; set; } = true;

            /// <summary>
			/// Gets or sets a value indicating that instead of a data name an alias will be shown.
			/// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets a value indicating that instead of a data name an alias will be shown.")]
            [StiSerializable]
            public static bool ShowOnlyAliasForData { get; set; }

            /// <summary>
            /// Gets or sets a value indicating that instead of a variable name an alias will be shown.
            /// </summary>
            [DefaultValue(true)]
            [Description("Gets or sets a value indicating that instead of a variable name an alias will be shown.")]
            [StiSerializable]
            public static bool ShowOnlyAliasForVariable { get; set; } = true;

            /// <summary>
            /// Gets or sets a value indicating that instead of a resource name an alias will be shown.
            /// </summary>
            [DefaultValue(true)]
            [Description("Gets or sets a value indicating that instead of a resource name an alias will be shown.")]
            [StiSerializable]
            public static bool ShowOnlyAliasForResource { get; set; } = true;

            /// <summary>
            /// Gets or sets a value indicating that instead of a total name an alias will be shown.
            /// </summary>
            [DefaultValue(true)]
            [Description("Gets or sets a value indicating that instead of a total name an alias will be shown.")]
            [StiSerializable]
            [Obsolete("Property 'StiOptions.Dictionary.ShowOnlyAliasForTotal' is not used more!")]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public static bool ShowOnlyAliasForTotal { get; set; } = true;

            /// <summary>
            /// Gets or sets a value indicating that instead of a DataSource name an alias will be shown.
            /// </summary>
            [DefaultValue(true)]
            [Description("Gets or sets a value indicating that instead of a DataSource name an alias will be shown.")]
            [StiSerializable]
            public static bool ShowOnlyAliasForDataSource { get; set; } = true;

            /// <summary>
            /// Gets or sets a value indicating that instead of a business object name an alias will be shown.
            /// </summary>
            [DefaultValue(true)]
            [Description("Gets or sets a value indicating that instead of a business object name an alias will be shown.")]
            [StiSerializable]
            public static bool ShowOnlyAliasForBusinessObject { get; set; } = true;

            /// <summary>
            /// Gets or sets a value indicating that instead of a DataColumn name an alias will be shown.
            /// </summary>
            [DefaultValue(true)]
            [Description("Gets or sets a value indicating that instead of a DataColumn name an alias will be shown.")]
            [StiSerializable]
            public static bool ShowOnlyAliasForDataColumn { get; set; } = true;

            /// <summary>
            /// Gets or sets a value indicating that instead of a DataRelation name an alias will be shown.
            /// </summary>
            [DefaultValue(true)]
            [Description("Gets or sets a value indicating that instead of a DataRelation name an alias will be shown.")]
            [StiSerializable]
            public static bool ShowOnlyAliasForDataRelation { get; set; } = true;

            [DefaultValue(true)]
            [StiSerializable]
            public static bool HideRelationExceptions { get; set; } = true;

            [DefaultValue(false)]
            [StiSerializable]
            public static bool UseNullableDateTime { get; set; }

            [DefaultValue(false)]
            [StiSerializable]
            public static bool UseNullableTimeSpan { get; set; }

            [DefaultValue(false)]
            [StiSerializable]
            public static bool NotIdenticalNameAndAliasAtRegistrationOfNewData { get; set; }

            [DefaultValue(true)]
            [StiSerializable]
            public static bool ReplaceExistingDataAtRegistrationOfNewData { get; set; } = true;

            [DefaultValue(StiColumnsSynchronizationMode.KeepAbsentColumns)]
            [StiSerializable]
            public static StiColumnsSynchronizationMode ColumnsSynchronizationMode { get; set; } = StiColumnsSynchronizationMode.KeepAbsentColumns;

            [DefaultValue(StiColumnsSynchronizationMode.KeepAbsentColumns)]
            [StiSerializable]
            public static StiColumnTypeSynchronizationMode ColumnTypeSynchronizationMode { get; set; } = StiColumnTypeSynchronizationMode.KeepAsIs;

            [DefaultValue(false)]
            [StiSerializable]
            public static bool UseAllowDBNullProperty { get; set; }

		}		
    }
}