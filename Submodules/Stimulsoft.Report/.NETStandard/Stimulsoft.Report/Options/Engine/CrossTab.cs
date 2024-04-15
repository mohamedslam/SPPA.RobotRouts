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
        /// A class which controls of settings of the report engine. 
        /// </summary>
		public sealed partial class Engine
        {
            public sealed class CrossTab
            {
                [DefaultValue(5d)]
                [StiSerializable]
                public static double DefaultWidth { get; set; } = 5d;

                [DefaultValue(5d)]
                [StiSerializable]
                public static double DefaultHeight { get; set; } = 5d;

                [DefaultValue(true)]
                [StiSerializable]
                public static bool MemoryOptimization { get; set; } = true;

                /// <summary>
                /// Gets or sets value which indicates that report engine can start the Garbage Collector during rendering.
                /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets value which indicates that report engine can start the Garbage Collector during rendering.")]
                [StiSerializable]
                public static bool AllowGCCollect { get; set; } = true;
            }
		}
    }
}