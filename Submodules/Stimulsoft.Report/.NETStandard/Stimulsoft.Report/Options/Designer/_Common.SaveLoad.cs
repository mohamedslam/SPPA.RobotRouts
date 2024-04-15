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
            [DefaultValue(true)]
            [StiSerializable]
            public static bool AllowSaveReportTemplateWithEmbededData { get; set; } = true;

            [DefaultValue(true)]
            [StiSerializable]
#if NETSTANDARD || NETCOREAPP
            public static bool AllowSaveToSourceCode { get; set; } = false;
#else
            public static bool AllowSaveToSourceCode { get; set; } = true;
#endif

            [DefaultValue(false)]
            [StiSerializable]
            public static bool AllowSaveToSourceCodeForSilverlight { get; set; }

            [DefaultValue(true)]
            [StiSerializable]
#if NETSTANDARD || NETCOREAPP
            public static bool AllowSaveToAssembly { get; set; } = false;
#else
            public static bool AllowSaveToAssembly { get; set; } = true;
#endif

            [DefaultValue(false)]
            [StiSerializable]
            public static bool AllowSaveStandaloneReport { get; set; }

            [DefaultValue(false)]
            [StiSerializable]
            public static bool AllowSaveSourceCodeForInheritedReports { get; set; }

            [DefaultValue(false)]
            [StiSerializable]
            public static bool DontSaveFormsSettings { get; set; }

            public static int[] AutoSaveIntervals { get; set; } = { 5, 10, 15, 20, 30, 60 };

            /// <summary>
            /// Gets or sets a value which controls of whether to save configuration of a Docking Manager object or not.
            /// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets a value which controls of whether to save configuration of a Docking Manager object or not.")]
            [StiSerializable]
            public static bool DontSaveDockingPanelsConfig { get; set; }

            /// <summary>
            /// Gets or sets a value which controls saving configuration of the designer.
            /// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets a value which controls saving configuration of the designer.")]
            [StiSerializable]
            public static bool DontSaveDesignerConfig { get; set; }

            /// <summary>
            /// Gets or sets a value, which controls of saving configuration of the editor.
            /// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets a value, which controls of saving configuration of the editor.")]
            [StiSerializable]
            public static bool DontSaveEditorConfig { get; set; }

            /// <summary>
            /// Gets or sets a value, which controls of questioning about saving the report when the designer is closed. 
            /// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets a value, which controls of questioning about saving the report when the designer is closed.")]
            [StiSerializable]
            public static bool DontAskSaveReport { get; set; }

            [DefaultValue(true)]
            [StiSerializable]
            public static bool SaveReportWhenDontAskSaveReport { get; set; } = true;

            [DefaultValue(true)]
            [StiSerializable]
            public static bool AllowPageSaveAs { get; set; } = true;

            [DefaultValue(false)]
            [StiSerializable]
            public static bool PlaceMRZBeforeMRTInSaveDialog { get; set; }

            [DefaultValue(false)]
            [StiSerializable]
            public static bool IgnoreOptionReportNeverSaved { get; set; }

            [DefaultValue(true)]
            [StiSerializable]
            public static bool CanDesignerChangeReportFileName { get; set; } = true;

            [DefaultValue(false)]
            [StiSerializable]
            public static bool ReadOnlyAlertOnSave { get; set; }
        }
    }
}