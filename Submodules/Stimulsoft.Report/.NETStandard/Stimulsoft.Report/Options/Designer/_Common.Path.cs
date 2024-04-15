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
        /// Class which allows adjustment of the Designer of the report.
        /// </summary>
        public sealed partial class Designer
        {
            /// <summary>
            /// Gets or sets the name of the configuration file of the Docking Panels.
            /// </summary>
            [DefaultValue("")]
            [Description("Gets or sets the name of the configuration file of the Docking Panels.")]
            [StiSerializable]
            public static string DefaultDockingPanelsConfigPath { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the name of the configuration file of the Designer.
            /// </summary>
            [DefaultValue("")]
            [Description("Gets or sets the name of the configuration file of the Designer.")]
            [StiSerializable]
            public static string DefaultDesignerConfigPath { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the default path to "Templates" folder of the Designer.
            /// </summary>
            [DefaultValue("")]
            [Description("Gets or sets the default path to 'Templates' folder of the Designer.")]
            [StiSerializable]
            public static string DefaultTemplatesPath { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the default path to "StylesCollections" folder of the Designer.
            /// </summary>
            [DefaultValue("")]
            [Description("Gets or sets the default path to 'StylesCollections' folder of the Designer.")]
            [StiSerializable]
            public static string DefaultStylesCollectionsPath { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the name of the configuration file of the Ribbon Designer.
            /// </summary>
            [DefaultValue("")]
            [Description("Gets or sets the name of the configuration file of the Ribbon Designer.")]
            [StiSerializable]
            public static string DefaultRibbonDesignerConfigPath { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the name of the configuration file of quick access toolbar of the Ribbon Designer.
            /// </summary>
            [DefaultValue("")]
            [Description("Gets or sets the name of the configuration file of quick access toolbar of the Ribbon Designer.")]
            [StiSerializable]
            public static string DefaultRibbonDesignerQatConfigPath { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets a file name which was used for saving or for loading report template last time.
            /// </summary>
            [DefaultValue("")]
            [Description("Gets or sets a file name which was used for saving or for loading report template last time.")]
            [StiSerializable]
            public static string ReportSaveLoadPath { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets a file name which was used for saving or for loading one page of the report template last time.
            /// </summary>
            [DefaultValue("")]
            [Description("Gets or sets a file name which was used for saving or for loading one page of the report template last time.")]
            [StiSerializable]
            public static string PageSaveLoadPath { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets a file name which was used for saving or for loading component templates.
            /// </summary>
            [DefaultValue("")]
            [Description("Gets or sets a file name which was used for saving or for loading component templates.")]
            [StiSerializable]
            public static string TemplatesSaveLoadPath { get; set; } = string.Empty;
        }
    }
}