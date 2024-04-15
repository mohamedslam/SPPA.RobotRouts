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

namespace Stimulsoft.Report
{
    /// <summary>
    /// Class for adjustment all aspects of Stimulsoft Reports.
    /// </summary>
    public sealed partial class StiOptions
	{
        /// <summary>
        /// Class for adjustment of the report Export.
        /// </summary>
        public sealed partial class Export
		{	
            /// <summary>
            /// Class for adjustment of the Xml export of a report.
            /// </summary>
            public sealed class Xml
            {
                /// <summary>
                /// Gets or sets a value which indicates whether it is necessary to use aliases instead of names of components for columns name.
                /// </summary>
                [DefaultValue(false)]
                [Description("Gets or sets a value which indicates whether it is necessary to use aliases instead of names of components for columns name.")]
                [StiSerializable]
                public static bool UseAliases { get; set; }

                /// <summary>
                /// Gets or sets a value which indicates whether it is necessary to write Xml Document Declaration.
                /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value which indicates whether it is necessary to write Xml Document Declaration.")]
                [StiSerializable]
                public static bool WriteXmlDocumentDeclaration { get; set; } = true;

                /// <summary>
                /// Gets or sets a value which indicates whether it is necessary to convert tags to upperCase.
                /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value which indicates whether it is necessary to convert tags to upperCase.")]
                [StiSerializable]
                public static bool ConvertTagsToUpperCase { get; set; } = true;

                /// <summary>
                /// Gets or sets a value which indicates whether it is necessary to remove html-tags from text.
                /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value which indicates whether it is necessary to remove html-tags from text.")]
                [StiSerializable]
                public static bool RemoveHtmlTags { get; set; } = true;
            }
        }
    }
}