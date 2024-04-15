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
using System;

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
            /// <summary>
            /// Class for adjustment of the viewer window.
            /// </summary>
            public partial class Windows
            {
                public class WPFViewer
                {
                    [DefaultValue(15d)]
                    [StiSerializable]
                    public static double DistanceBetweenPagesHorz { get; set; } = 15d;

                    [DefaultValue(15d)]
                    [StiSerializable]
                    public static double DistanceBetweenPagesVert { get; set; } = 15d;

                    [DefaultValue(10d)]
                    [StiSerializable]
                    public static double DistanceFromLeft { get; set; } = 10d;

                    [DefaultValue(10d)]
                    [StiSerializable]
                    public static double DistanceFromRight { get; set; } = 10d;

                    [DefaultValue(10d)]
                    [StiSerializable]
                    public static double DistanceFromTop { get; set; } = 10d;

                    [DefaultValue(10d)]
                    [StiSerializable]
                    public static double DistanceFromBottom { get; set; } = 10d;

                    [DefaultValue(1.020d)]
                    [StiSerializable]
                    [Obsolete]
                    [EditorBrowsable(EditorBrowsableState.Never)]
                    public static double ScaleActivePage { get; set; } = 1.020d;
                }
			}
        }
    }
}