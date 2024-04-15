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
        /// Class for adjustment of the preview of a report.
        /// </summary>
        public partial class Viewer
		{
            public static class HotKeys
            {
                #region Methods
                public static void DisableAllHotkeys()
                {
                    SwitchAllHotkeys(false);
                }

                public static void EnableAllHotkeys()
                {
                    SwitchAllHotkeys(true);
                }

                public static void SwitchAllHotkeys(bool enabled)
                {
                    CtrlBEnabled = enabled;
                    CtrlEEnabled = enabled;
                    CtrlDEnabled = enabled;
                    CtrlFEnabled = enabled;
                    CtrlOEnabled = enabled;
                    CtrlPEnabled = enabled;
                    CtrlSEnabled = enabled;
                    CtrlGEnabled = enabled;
                    CtrlREnabled = enabled;
                    CtrlTEnabled = enabled;
                    CtrlEnterEnabled = enabled;
                    CtrlShiftDEnabled = enabled;
                    CtrlShiftEEnabled = enabled;
                    CtrlShiftNEnabled = enabled;
                    CtrlShiftPEnabled = enabled;
                    CtrlShiftSEnabled = enabled;
                    EscapeEnabled = enabled;
                    F2Enabled = enabled;
                    F3Enabled = enabled;
                    F4Enabled = enabled;
                    F5Enabled = enabled;
                    F7Enabled = enabled;
                    ShiftF2Enabled = enabled;
                    ShiftF3Enabled = enabled;
                    ShiftF4Enabled = enabled;
                }
                #endregion

                #region Properties
                /// <summary>
                /// Get or set value indicating whether the Escape shortcut is enabled.
                /// </summary>
                [Description("Get or set value indicating whether the Escape shortcut is enabled.")]
                [DefaultValue(true)]
                [StiSerializable]
                public static bool EscapeEnabled { get; set; } = true;

                /// <summary>
                /// Get or set value indicating whether the Ctrl + P shortcut is enabled.
                /// </summary>
                [Description("Get or set value indicating whether the Ctrl + P shortcut is enabled.")]
                [DefaultValue(true)]
                [StiSerializable]
                public static bool CtrlPEnabled { get; set; } = true;

                /// <summary>
                /// Get or set value indicating whether the Ctrl + O shortcut is enabled.
                /// </summary>
                [Description("Get or set value indicating whether the Ctrl + O shortcut is enabled.")]
                [DefaultValue(true)]
                [StiSerializable]
                public static bool CtrlOEnabled { get; set; } = true;

                /// <summary>
                /// Get or set value indicating whether the Ctrl + S shortcut is enabled.
                /// </summary>
                [Description("Get or set value indicating whether the Ctrl + S shortcut is enabled.")]
                [DefaultValue(true)]
                [StiSerializable]
                public static bool CtrlSEnabled { get; set; } = true;

                /// <summary>
                /// Get or set value indicating whether the Ctrl + G shortcut is enabled.
                /// </summary>
                [Description("Get or set value indicating whether the Ctrl + G shortcut is enabled.")]
                [DefaultValue(true)]
                [StiSerializable]
                public static bool CtrlGEnabled { get; set; } = true;

                /// <summary>
                /// Get or set value indicating whether the Ctrl + Enter shortcut is enabled.
                /// </summary>
                [Description("Get or set value indicating whether the Ctrl + Enter shortcut is enabled.")]
                [DefaultValue(true)]
                [StiSerializable]
                public static bool CtrlEnterEnabled { get; set; } = true;

                /// <summary>
                /// Get or set value indicating whether the Ctrl + R shortcut is enabled.
                /// </summary>
                [Description("Get or set value indicating whether the Ctrl + R shortcut is enabled.")]
                [DefaultValue(true)]
                [StiSerializable]
                public static bool CtrlREnabled { get; set; } = true;

                /// <summary>
                /// Get or set value indicating whether the Ctrl + Shift + N shortcut is enabled.
                /// </summary>
                [Description("Get or set value indicating whether the Ctrl + Shift + N shortcut is enabled.")]
                [DefaultValue(true)]
                [StiSerializable]
                public static bool CtrlShiftNEnabled { get; set; } = true;

                /// <summary>
                /// Get or set value indicating whether the Ctrl + Shift + P shortcut is enabled.
                /// </summary>
                [Description("Get or set value indicating whether the Ctrl + Shift + P shortcut is enabled.")]
                [DefaultValue(true)]
                [StiSerializable]
                public static bool CtrlShiftPEnabled { get; set; } = true;

                /// <summary>
                /// Get or set value indicating whether the Ctrl + Shift + D shortcut is enabled.
                /// </summary>
                [Description("Get or set value indicating whether the Ctrl + Shift + D shortcut is enabled.")]
                [DefaultValue(true)]
                [StiSerializable]
                public static bool CtrlShiftDEnabled { get; set; } = true;

                /// <summary>
                /// Get or set value indicating whether the Ctrl + Shift + E shortcut is enabled.
                /// </summary>
                [Description("Get or set value indicating whether the Ctrl + Shift + E shortcut is enabled.")]
                [DefaultValue(true)]
                [StiSerializable]
                public static bool CtrlShiftEEnabled { get; set; } = true;

                /// <summary>
                /// Get or set value indicating whether the Ctrl + Shift + S shortcut is enabled.
                /// </summary>
                [Description("Get or set value indicating whether the Ctrl + Shift + S shortcut is enabled.")]
                [DefaultValue(true)]
                [StiSerializable]
                public static bool CtrlShiftSEnabled { get; set; } = true;

                /// <summary>
                /// Get or set value indicating whether the Ctrl + B shortcut is enabled.
                /// </summary>
                [Description("Get or set value indicating whether the Ctrl + B shortcut is enabled.")]
                [DefaultValue(true)]
                [StiSerializable]
                public static bool CtrlBEnabled { get; set; } = true;

                /// <summary>
                /// Get or set value indicating whether the Ctrl + T shortcut is enabled.
                /// </summary>
                [Description("Get or set value indicating whether the Ctrl + T shortcut is enabled.")]
                [DefaultValue(true)]
                [StiSerializable]
                public static bool CtrlTEnabled { get; set; } = true;

                /// <summary>
                /// Get or set value indicating whether the Ctrl + F shortcut is enabled.
                /// </summary>
                [Description("Get or set value indicating whether the Ctrl + F shortcut is enabled.")]
                [DefaultValue(true)]
                [StiSerializable]
                public static bool CtrlFEnabled { get; set; } = true;

                /// <summary>
                /// Get or set value indicating whether the Ctrl + E shortcut is enabled.
                /// </summary>
                [Description("Get or set value indicating whether the Ctrl + E shortcut is enabled.")]
                [DefaultValue(true)]
                [StiSerializable]
                public static bool CtrlEEnabled { get; set; } = true;

                /// <summary>
                /// Get or set value indicating whether the Ctrl + D shortcut is enabled.
                /// </summary>
                [Description("Get or set value indicating whether the Ctrl + D shortcut is enabled.")]
                [DefaultValue(true)]
                [StiSerializable]
                public static bool CtrlDEnabled { get; set; } = true;

                /// <summary>
                /// Get or set value indicating whether the Shift + F2 shortcut is enabled.
                /// </summary>
                [Description("Get or set value indicating whether the Shift + F2 shortcut is enabled.")]
                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShiftF2Enabled { get; set; } = true;

                /// <summary>
                /// Get or set value indicating whether the Shift + F3 shortcut is enabled.
                /// </summary>
                [Description("Get or set value indicating whether the Shift + F3 shortcut is enabled.")]
                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShiftF3Enabled { get; set; } = true;

                /// <summary>
                /// Get or set value indicating whether the Shift + F4 shortcut is enabled.
                /// </summary>
                [Description("Get or set value indicating whether the Shift + F4 shortcut is enabled.")]
                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShiftF4Enabled { get; set; } = true;

                /// <summary>
                /// Get or set value indicating whether the F2 shortcut is enabled.
                /// </summary>
                [Description("Get or set value indicating whether the F2 shortcut is enabled.")]
                [DefaultValue(true)]
                [StiSerializable]
                public static bool F2Enabled { get; set; } = true;

                /// <summary>
                /// Get or set value indicating whether the F3 shortcut is enabled.
                /// </summary>
                [Description("Get or set value indicating whether the F3 shortcut is enabled.")]
                [DefaultValue(true)]
                [StiSerializable]
                public static bool F3Enabled { get; set; } = true;

                /// <summary>
                /// Get or set value indicating whether the F4 shortcut is enabled.
                /// </summary>
                [Description("Get or set value indicating whether the F4 shortcut is enabled.")]
                [DefaultValue(true)]
                [StiSerializable]
                public static bool F4Enabled { get; set; } = true;

                /// <summary>
                /// Get or set value indicating whether the F5 shortcut is enabled.
                /// </summary>
                [Description("Get or set value indicating whether the F5 shortcut is enabled.")]
                [DefaultValue(true)]
                [StiSerializable]
                public static bool F5Enabled { get; set; } = true;

                /// <summary>
                /// Get or set value indicating whether the F7 shortcut is enabled.
                /// </summary>
                [Description("Get or set value indicating whether the F7 shortcut is enabled.")]
                [DefaultValue(true)]
                [StiSerializable]
                public static bool F7Enabled { get; set; } = true;
                #endregion
            }
        }
    }
}