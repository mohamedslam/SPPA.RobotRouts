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
            /// <summary>
            /// A Class for managing of the report cache.
            /// </summary>
            public sealed class ReportCache
            {
                private static int amountOfQuickAccessPages = 50;
                /// <summary>
                /// Gets or sets amount of quick access pages. This value must be greater than 5.
                /// </summary>
                [DefaultValue(50)]
                [Description("Gets or sets amount of quick access pages. This value must be greater than 5.")]
                [StiSerializable]
                public static int AmountOfQuickAccessPages
                {
                    get
                    {
                        return amountOfQuickAccessPages;
                    }
                    set
                    {
                        if (value < 5)
                            throw new ArgumentException("Property AmountOfQuickAccessPages can't be less then 5.");

                        amountOfQuickAccessPages = value;
                    }
                }

                private static int limitForStartUsingCache = 200;
                /// <summary>
                /// Gets or sets lower bound of starting using the cache of the report.
                /// </summary>
                [DefaultValue(200)]
                [Description("Gets or sets lower bound of starting using the cache of the report.")]
                [StiSerializable]
                public static int LimitForStartUsingCache
                {
                    get
                    {
                        return limitForStartUsingCache;
                    }
                    set
                    {
                        if (value < 0)
                            throw new ArgumentException("Property LimitForStartUsingCache can't be less then 0.");
                        limitForStartUsingCache = value;
                    }
                }

                private static int amountOfProcessedPagesForStartGCCollect = 60;
                /// <summary>
                /// Gets or sets lower bound of starting the Garbage Collector.
                /// </summary>
                [DefaultValue(60)]
                [Description("Gets or sets lower bound of starting the Garbage Collector.")]
                [StiSerializable]
                public static int AmountOfProcessedPagesForStartGCCollect
                {
                    get
                    {
                        return amountOfProcessedPagesForStartGCCollect;
                    }
                    set
                    {
                        if (value < 0)
                            throw new ArgumentException("Property AmountOfProcessedPagesForStartGCCollect can't be less then 0.");
                        amountOfProcessedPagesForStartGCCollect = value;
                    }
                }

                /// <summary>
                /// Gets or sets value which indicates that report engine can start the Garbage Collector during report rendering in the report cache.
                /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets value which indicates that report engine can start the Garbage Collector during report rendering in the report cache.")]
                [StiSerializable]
                public static bool AllowGCCollect { get; set; } = true;

                /// <summary>
                /// Gets or sets a thread mode.
                /// </summary>
                [DefaultValue(StiReportCacheThreadMode.Auto)]
                [Description("Gets or sets a thread mode.")]
                [StiSerializable]
                [Category("Engine")]
                public static StiReportCacheThreadMode ThreadMode { get; set; } = StiReportCacheThreadMode.Auto;

                /// <summary>
                /// Gets or sets value which specifies path for the report cache.
                /// </summary>
                [DefaultValue(null)]
                [Description("Gets or sets value which specifies path for the report cache.")]
                [StiSerializable]
                public static string CachePath { get; set; }

                /// <summary>
                /// Gets or sets value which indicates that report engine can dispose the images during report rendering in the report cache.
                /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets value which indicates that report engine can dispose the images during report rendering in the report cache.")]
                [StiSerializable]
                public static bool DisposeImagesOnPageClear { get; set; } = true;

                /// <summary>
                /// Gets or sets value which indicates that optimization is used for SetText on EndRender.
                /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets value which indicates that optimization is used for SetText on EndRender.")]
                [StiSerializable]
                public static bool OptimizeEndRenderSetText { get; set; } = true;


            }
        }
    }
}