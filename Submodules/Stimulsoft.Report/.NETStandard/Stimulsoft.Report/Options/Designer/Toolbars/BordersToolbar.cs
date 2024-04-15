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
            public sealed partial class Toolbars
            {
                public static class BordersToolbar
                {
                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowBorderAll { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowBorderNone { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowBorderTop { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowBorderLeft { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowBorderBottom { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowBorderRight { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowBorderShadow { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowFillBrush { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowBorderColor { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowBorderStyle { get; set; } = true;
                }
            }
		}
    }
}