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
using System.Drawing;
using Stimulsoft.Base.Serializing;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

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
            /// Gets or sets window state of the Designer window.
            /// </summary>
            [DefaultValue(FormWindowState.Maximized)]
            [Description("Gets or sets window state of the Designer window.")]
            [StiSerializable]
            public static FormWindowState DesignerWindowState { get; set; } = FormWindowState.Maximized;

            /// <summary>
            /// Gets or sets window size of the Designer window.
            /// </summary>
            public static Size? DesignerWindowSize { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether Designer will be shown in the task bar.
            /// </summary>
            [DefaultValue(true)]
            [Description("Gets or sets a value indicating whether Designer will be shown in the task bar.")]
            [StiSerializable]
            public static bool ShowDesignerInTaskbar { get; set; } = true;

            /// <summary>
            /// Gets or sets a value indicating whether Designer will be shown in the styles mode.
            /// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets a value indicating whether Designer will be shown in the styles mode.")]
            [StiSerializable]
            public static bool ShowDesignerInStylesMode { get; set; }
        }
    }
}