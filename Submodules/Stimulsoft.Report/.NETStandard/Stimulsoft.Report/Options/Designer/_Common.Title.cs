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

using System.IO;
using System.ComponentModel;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using System.Drawing;

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
            #region Methods
            /// <summary>
            /// Internal use only.
            /// </summary>
            public static string GetDesignerTitle()
            {
                return DesignerTitle ?? StiLocalization.Get("FormDesigner", "title");
            }

            /// <summary>
            /// Internal use only.
            /// </summary>
            public static string GetDesignerTitleWithMask(string file)
            {
                if (DesignerTitleText != null) return DesignerTitleText;

                if (!string.IsNullOrEmpty(file))
                    return string.Format(DesignerTitleMask, Path.GetFileNameWithoutExtension(file), GetDesignerTitle());
                else
                    return GetDesignerTitle();
            }
            #endregion

            #region Properties
            [DefaultValue(true)]
            [StiSerializable]
            public static bool AllowMultiColorTitle { get; set; } = true;

            [DefaultValue(false)]
            [StiSerializable]
            public static bool ShowFullFileNameInTitle { get; set; }

            [DefaultValue(null)]
            [StiSerializable]
            public static string DesignerTitle { get; set; }

            [DefaultValue(null)]
            [StiSerializable]
            public static string DesignerTitleText { get; set; }

            [DefaultValue("{0} - {1}")]
            [StiSerializable]
            public static string DesignerTitleMask { get; set; } = "{0} - {1}";

            [DefaultValue(true)]
            [StiSerializable]
            public static bool ShowRibbonWindowTitleInWpf { get; set; } = true;
            
            public static Icon DesignerIcon { get; set; }
            #endregion
        }
    }
}