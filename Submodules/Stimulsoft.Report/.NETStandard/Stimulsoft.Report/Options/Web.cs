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

using Stimulsoft.Base.Serializing;
using System.ComponentModel;
using System.Text;

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
        public class Web
        {
            [DefaultValue(true)]
            [StiSerializable]
            public static bool AllowUseResponseEnd { get; set; } = true;

            [DefaultValue(true)]
            [StiSerializable]
            public static bool AllowUseResponseFlush { get; set; } = true;

            [DefaultValue(false)]
            [StiSerializable]
            public static bool ClearResponseHeaders { get; set; }

            [DefaultValue(60)]
            [StiSerializable]
            public static int ResponseCacheTimeout { get; set; } = 60;

            [DefaultValue(false)]
            [StiSerializable]
            public static bool AllowGCCollect { get; set; }

            [StiSerializable]
            public static Encoding WebClientEncoding
            {
                get
                {
                    return Stimulsoft.Base.StiBaseOptions.WebClientEncoding;
                }
                set
                {
                    Stimulsoft.Base.StiBaseOptions.WebClientEncoding = value;
                }
            }

        }
    }
}