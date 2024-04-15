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
using Stimulsoft.Report.Export;

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
            /// Gets or sets a value indicating to show or to hide Code Tab of the report designer.
            /// </summary>
            [DefaultValue(true)]
            [Description("Gets or sets a value indicating to show or to hide Code Tab of the report designer.")]
            [StiSerializable]
            public static bool CodeTabVisible { get; set; } = true;

            /// <summary>
            /// Gets or sets a value, which shows or hides a Preview Tab in the report designer.
            /// </summary>
            [DefaultValue(true)]
            [Description("Gets or sets a value, which shows or hides a Preview Tab in the report designer.")]
            [StiSerializable]
            public static bool PreviewReportVisible { get; set; } = true;

            /// <summary>
            /// Gets or sets a value, which shows or hides a WPF Preview Tab in the report designer.
            /// </summary>
            [DefaultValue(true)]
            [Description("Gets or sets a value, which shows or hides a WPF Preview Tab in the report designer.")]
            [StiSerializable]
            public static bool WpfPreviewReportVisible { get; set; } = true;

            /// <summary>
            /// 'StiOptions.Designer.WebPreviewReportVisible' doesn't used more!
            /// </summary>
            [DefaultValue(false)]
            [Obsolete("'StiOptions.Designer.WebPreviewReportVisible' doesn't used more!")]
            [Browsable(false)]
            public static bool WebPreviewReportVisible { get; set; }

            /// <summary>
            /// 'StiOptions.Designer.SLPreviewReportVisible' doesn't used more!
            /// </summary>
            [DefaultValue(false)]
            [Obsolete("'StiOptions.Designer.SLPreviewReportVisible' doesn't used more!")]
            [Browsable(false)]
            public static bool SLPreviewReportVisible { get; set; }

            /// <summary>
            /// 'StiOptions.Designer.WinRTPreviewReportVisible' doesn't used more!
            /// </summary>
            [DefaultValue(false)]
            [Obsolete("'StiOptions.Designer.WinRTPreviewReportVisible' doesn't used more!")]
            [Browsable(false)]
            public static bool WinRTPreviewReportVisible { get; set; }

            /// <summary>
            /// 'StiOptions.Designer.HtmlPreviewReportVisible' doesn't used more!
            /// </summary>
            [DefaultValue(false)]
            [Obsolete("'StiOptions.Designer.HtmlPreviewReportVisible' doesn't used more!")]
            [Browsable(false)]
            public static bool HtmlPreviewReportVisible { get; set; }

            /// <summary>
            /// 'StiOptions.Designer.JsPreviewReportVisible' doesn't used more!
            /// </summary>
            [DefaultValue(false)]
            [Obsolete("'StiOptions.Designer.JsPreviewReportVisible' doesn't used more!")]
            [Browsable(false)]
            public static bool JsPreviewReportVisible { get; set; }

            /// <summary>
            /// 'StiOptions.Designer.WebPreviewReportEnabled' doesn't used more!
            /// </summary>
            [DefaultValue(false)]
            [Obsolete("'StiOptions.Designer.WebPreviewReportEnabled' doesn't used more!")]
            [Browsable(false)]
            public static bool WebPreviewReportEnabled { get; set; }

            /// <summary>
            /// 'StiOptions.Designer.HtmlPreviewReportEnabled' doesn't used more!
            /// </summary>
            [DefaultValue(false)]
            [Obsolete("'StiOptions.Designer.HtmlPreviewReportEnabled' doesn't used more!")]
            [Browsable(false)]
            public static bool HtmlPreviewReportEnabled { get; set; }

            /// <summary>
            /// 'StiOptions.Designer.SLPreviewReportEnabled' doesn't used more!
            /// </summary>
            [DefaultValue(false)]
            [Obsolete("'StiOptions.Designer.SLPreviewReportEnabled' doesn't used more!")]
            [Browsable(false)]
            public static bool SLPreviewReportEnabled { get; set; }

            /// <summary>
            /// 'StiOptions.Designer.WinRTPreviewReportEnabled' doesn't used more!
            /// </summary>
            [DefaultValue(false)]
            [Obsolete("'StiOptions.Designer.WinRTPreviewReportEnabled' doesn't used more!")]
            [Browsable(false)]
            public static bool WinRTPreviewReportEnabled { get; set; }

            [DefaultValue(StiHtmlExportMode.Table)]
            [StiSerializable]
            public static StiHtmlExportMode PreviewHtmlExportMode { get; set; } = StiHtmlExportMode.Table;

            /// <summary>
            /// Gets or sets a value indicating whether the Preview Window of the report will be showing as MDI window.
            /// </summary>
            [DefaultValue(true)]
            [Description("Gets or sets a value indicating whether the Preview Window of the report will be showing as MDI window.")]
            [StiSerializable]
            public static bool ShowPreviewInMdi { get; set; } = true;
        }
    }
}