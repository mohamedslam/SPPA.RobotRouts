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
            public sealed class BusinessObjects
            {
                [DefaultValue(true)]
                [StiSerializable]
                public static bool AllowUseDataColumn { get; set; } = true;

                [DefaultValue(false)]
                [StiSerializable]
                public static bool AllowUseFields { get; set; }

                [DefaultValue(true)]
                [StiSerializable]
                public static bool AllowUseProperties { get; set; } = true;

                /// <summary>
                /// Gets or sets a value, which controls processing of properties in business objects.
                /// </summary>
                [DefaultValue(StiPropertiesProcessingType.Browsable)]
                [Description("Gets or sets a value, which controls processing of properties in business objects.")]
                [StiSerializable]
                public static StiPropertiesProcessingType PropertiesProcessingType { get; set; } = StiPropertiesProcessingType.Browsable;

                /// <summary>
                /// Gets or sets a value, which controls processing of fields in business objects.
                /// </summary>
                [DefaultValue(StiFieldsProcessingType.Browsable)]
                [Description("Gets or sets a value, which controls processing of fields in business objects.")]
                [StiSerializable]
                public static StiFieldsProcessingType FieldsProcessingType { get; set; } = StiFieldsProcessingType.Browsable;

                /// <summary>
                /// Gets or sets a delimiter between a name of the enumerable data source
                /// and its column names for the child tables in the master-detail relation.
                /// </summary>
                [DefaultValue('_')]
                [Description("Gets or sets a delimiter between a name of the enumerable data source and its column names for the child tables in the master-detail relation.")]
                [StiSerializable]
                public static char Delimeter { get; set; } = '_';

                /// <summary>
                /// Gets or sets the maximal nested level of master-detail relations in enumerable sources.
                /// </summary>
                [DefaultValue(10)]
                [Description("Gets or sets the maximal nested level of master-detail relations in enumerable sources.")]
                [StiSerializable]
                public static int MaxLevel { get; set; } = 10;

                /// <summary>
                /// Gets or sets a value, which controls that enumerable sources with the same names.
                /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value, which controls that enumerable sources with the same names.")]
                [StiSerializable]
                public static bool CheckTableDuplication { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool AddBusinessObjectAssemblyToReferencedAssembliesAutomatically { get; set; } = true;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool AllowProcessNullItemsInEnumerables { get; set; } = true;

                [DefaultValue(StiColumnsSynchronizationMode.KeepAbsentColumns)]
                [StiSerializable]
                public static StiColumnsSynchronizationMode ColumnsSynchronizationMode { get; set; } = StiColumnsSynchronizationMode.KeepAbsentColumns;
            }
		}		
    }
}