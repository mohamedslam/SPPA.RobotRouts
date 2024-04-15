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
using System.Globalization;
using Stimulsoft.Base.Serializing;

namespace Stimulsoft.Report
{
    /// <summary>
    /// Class for adjustment all aspects of Stimulsoft Reports.
    /// </summary>
    public sealed partial class StiOptions
	{
        /// <summary>
        /// Class for adjustment of the report Export.
        /// </summary>
        public sealed partial class Export
		{	
            /// <summary>
            /// Class for adjusting the Html export of a report.
            /// </summary>
            public sealed class Html
            {
                /// <summary>
                /// Gets or sets a value, which indicates whether it is necessary to convert all digits to arabic digits.
                /// </summary>
                [DefaultValue(false)]
                [Description("Gets or sets a value, which indicates whether it is necessary to convert all digits to arabic digits.")]
                [StiSerializable]
                public static bool ConvertDigitsToArabic { get; set; }

                /// <summary>
                /// Gets or sets a value indicating a current arabic type of digits.
                /// </summary>
                [DefaultValue(StiArabicDigitsType.Standard)]
                [Description("Gets or sets a value indicating a current arabic type of digits.")]
                [StiSerializable]
                public static StiArabicDigitsType ArabicDigitsType { get; set; } = StiArabicDigitsType.Standard;

                /// <summary>
                /// Gets or sets a value indicating whether it is necessary to use image comparer.
                /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value indicating whether it is necessary to use image comparer.")]
                [StiSerializable]
                public static bool AllowImageComparer { get; set; } = true;

                /// <summary>
                /// Gets or sets a value indicating whether it is necessary to force wordwrap in wysiwyg mode.
                /// </summary>
                [DefaultValue(false)]
                [Description("Gets or sets a value indicating whether it is necessary to force wordwrap in wysiwyg mode.")]
                [StiSerializable]
                public static bool ForceWysiwygWordwrap { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether it is necessary to replace special characters such as '<', '>', '&', '"'
                /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value indicating whether it is necessary to replace special characters such as '<', '>', '&'.")]
                [StiSerializable]
                public static bool ReplaceSpecialCharacters { get; set; } = true;

                /// <summary>
                /// Gets or sets a value indicating whether it is necessary to use image resolution
                /// </summary>
                [DefaultValue(false)]
                [Description("Gets or sets a value indicating whether it is necessary to use image resolution.")]
                [StiSerializable]
                public static bool UseImageResolution { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether it is necessary to use BreakWord mode of wordwrap
                /// For compatibility with 2009.2 and earlier versions set this property to false.
                /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value indicating whether it is necessary to use BreakWord mode of wordwrap. For compatibility with 2009.2 and earlier versions set this property to false.")]
                [StiSerializable]
                public static bool UseWordWrapBreakWordMode { get; set; } = true;

                /// <summary>
                /// Gets or sets a value indicating whether it is necessary to use strict table cell size.
                /// For compatibility with 2012.3 and earlier versions set this property to false. 
                /// </summary>
                [DefaultValue(false)]
                [Description("Gets or sets a value indicating whether it is necessary to use strict table cell size. For compatibility with 2012.3 and earlier versions set this property to false.")]
                [StiSerializable]
                public static bool UseStrictTableCellSize { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether it is necessary to use strict table cell size.
                /// For compatibility with 2016.3 and earlier versions set this property to false. 
                /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value indicating whether it is necessary to use strict table cell size. For compatibility with 2016.3 and earlier versions set this property to false.")]
                [StiSerializable]
                public static bool UseStrictTableCellSizeV2 { get; set; } = true;

                /// <summary>
                /// Gets or sets a forced culture for the charts rendering.
                /// </summary>
                public static CultureInfo ForcedCultureForCharts { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether it is necessary to use compatibility with IE6.
                /// </summary>
                [DefaultValue(false)]
                [Description("Gets or sets a value indicating whether it is necessary to use compatibility with IE6.")]
                [StiSerializable]
                public static bool ForceIE6Compatibility { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether it is necessary to use stripped images.
                /// For compatibility with 2011.3 and earlier versions set this property to false.
                /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value indicating whether it is necessary to use stripped images. For compatibility with 2011.3 and earlier versions set this property to false.")]
                [StiSerializable]
                public static bool AllowStrippedImages { get; set; } = true;

                /// <summary>
                /// Gets or sets a value indicating whether it is necessary to remove empty space at the bottom of page
                /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value indicating whether it is necessary to remove empty space at the bottom of page.")]
                [StiSerializable]
                public static bool RemoveEmptySpaceAtBottom { get; set; } = true;

                /// <summary>
                /// Gets or sets a value indicating whether to use extended style description for fix the styles inheritance problem
                /// </summary>
                [DefaultValue(false)]
                [Description("Gets or sets a value indicating whether to use extended style description for fix the styles inheritance problem.")]
                [StiSerializable]
                public static bool UseExtendedStyle { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether to use layout optimization for correct printing.
                /// Disable for compatibility with version 2014.1
                /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value indicating whether to use layout optimization for correct printing.")]
                [StiSerializable]
                public static bool PrintLayoutOptimization { get; set; } = true;

                /// <summary>
                /// Gets or sets a value indicating whether to force export chart as bitmap.
                /// </summary>
                [DefaultValue(false)]
                [Description("Gets or sets a value indicating whether to force export chart as bitmap.")]
                [StiSerializable]
                public static bool ChartAsBitmap { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether to use ComponentStyle as the style name.
                /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value indicating whether to use ComponentStyle as the style name.")]
                [StiSerializable]
                public static bool UseComponentStyleName { get; set; } = true;

                /// <summary>
                /// Gets or sets a value indicating whether it is necessary to preserve the white spaces. HTML5 export mode is not supported.
                /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value indicating whether it is necessary to preserve the white spaces. HTML5 export mode is not supported.")]
                [StiSerializable]
                public static bool PreserveWhiteSpaces { get; set; } = true;

                /// <summary>
                /// Gets or sets a target to open links from the exported report.
                /// </summary>
                [DefaultValue(null)]
                [Description("Gets or sets a target to open links from the exported report.")]
                [StiSerializable]
                public static string OpenLinksTarget { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether it is necessary to process RoundedRectangles
                /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value indicating whether it is necessary to process RoundedRectangles.")]
                [StiSerializable]
                public static bool AllowRoundedRectangles { get; set; } = true;
                
                /// <summary>
                /// Gets or sets a value indicating whether it is necessary to export component from page margins (div/span mode only).
                /// For compatibility with 2021.3 and earlier versions set this property to false. 
                /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value indicating whether it is necessary to export component from page margins (div/span mode only). For compatibility with 2021.3 and earlier versions set this property to false.")]
                [StiSerializable]
                public static bool ExportComponentsFromPageMargins { get; set; } = true;

                /// <summary>
                /// Gets or sets a value indicating default coordinates precision.
                /// </summary>
                [DefaultValue(3)]
                [Description("Gets or sets a value indicating default coordinates precision.")]
                [StiSerializable]
                public static int DefaultCoordinatesPrecision { get; set; } = 3;

                /// <summary>
                /// Gets or sets a value indicating whether it is necessary to disable Javascript in hyperlinks
                /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value indicating whether it is necessary to disable Javascript in hyperlinks.")]
                [StiSerializable]
                public static bool DisableJavascriptInHyperlinks { get; set; } = true;

            }
        }
    }
}