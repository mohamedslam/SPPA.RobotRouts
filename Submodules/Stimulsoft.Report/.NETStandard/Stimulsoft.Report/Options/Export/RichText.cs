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
        /// Class for adjustment of the report Export.
        /// </summary>
        public sealed partial class Export
		{	
            /// <summary>
            /// Class for adjustment of the RichText export of a report.
            /// </summary>
            public class RichText
            {
                /// <summary>
                /// Gets or sets a value indicating whether it is necessary to use PAGEREF field of MS-Word.
                /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value indicating whether it is necessary to use PAGEREF field of MS-Word.")]
                [StiSerializable]
                public static bool UsePageRefField { get; set; } = true;

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
                /// Gets or sets a value indicating behavior of the exporting segmented pages will be forcibly broken in order of chosen page format.
                /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value indicating behavior of the exporting segmented pages will be forcibly broken in order of chosen page format.")]
                [StiSerializable]
                public static bool DivideSegmentPages { get; set; } = true;

                /// <summary>
                /// When Rtf exporting segmented pages will be forcibly use the page size from template.
                /// This property work only if DivideSegmentPages=false.
                /// </summary>
                [DefaultValue(false)]
                [Description("When Rtf exporting segmented pages will be forcibly use the page size from template. This property work only if DivideSegmentPages=false.")]
                [StiSerializable]
                public static bool UseTemplatePageSize { get; set; }

                /// <summary>
				/// Gets or sets a value indicating a line height mode - "exactly" or "at least".
				/// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value indicating a line height mode - 'exactly' or 'at least'.")]
                [StiSerializable]
                public static bool LineHeightExactly { get; set; } = true;

                /// <summary>
                /// Gets or sets a value indicating a forcing of line height.
                /// </summary>
                [DefaultValue(false)]
                [Description("Gets or sets a value indicating a forcing of line height.")]
                [StiSerializable]
                public static bool ForceLineHeight { get; set; }

                /// <summary>
				/// Gets or sets a value indicating whether it is necessary to remove empty space at the bottom of page
				/// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value indicating whether it is necessary to remove empty space at the bottom of page.")]
                [StiSerializable]
                public static bool RemoveEmptySpaceAtBottom { get; set; } = true;

                /// <summary>
                ///  Gets or sets a value of line spacing
                /// </summary>
                [DefaultValue(0.922d)]
                [Description("Gets or sets a value of line spacing.")]
                [StiSerializable]
                public static double LineSpacing { get; set; } = 0.922d;

                /// <summary>
                /// Gets or sets a value of right margin correction
                /// </summary>
                [DefaultValue(0)]
                [Description("Gets or sets a value of right margin correction.")]
                [StiSerializable]
                public static int RightMarginCorrection { get; set; }

                /// <summary>
                /// Gets or sets a value of bottom margin correction
                /// </summary>
                [DefaultValue(42)]
                [Description("Gets or sets a value of bottom margin correction.")]
                [StiSerializable]
                public static int BottomMarginCorrection { get; set; } = 42;

                /// <summary>
                /// Gets or sets a value indicating behavior of the exporting big cells will be divided into small cells.
                /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value indicating behavior of the exporting big cells will be divided into small cells.")]
                [StiSerializable]
                public static bool DivideBigCells { get; set; } = true;

                /// <summary>
				/// Gets or sets a value indicating whether it is necessary to use CanBreak property of components.
				/// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value indicating whether it is necessary to use CanBreak property of components.")]
                [StiSerializable]
                public static bool UseCanBreakProperty { get; set; } = true;

                /// <summary>
                /// Gets or sets a value of expansion or compression of the space between characters in twips; a negative value compresses.
                /// </summary>
                [DefaultValue(-2)]
                [Description("Gets or sets a value of expansion or compression of the space between characters in twips; a negative value compresses.")]
                [StiSerializable]
                public static int SpaceBetweenCharacters { get; set; } = -2;

                /// <summary>
                /// Gets or sets a value indicating a forcing of empty cells optimization.
                /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value indicating a forcing of empty cells optimization.")]
                [StiSerializable]
                public static bool ForceEmptyCellsOptimization { get; set; } = true;

                /// <summary>
                /// Gets or sets a value indicating whether it is necessary to use NewPage command instead of NewSection
                /// </summary>
                [DefaultValue(false)]
                [Description("Gets or sets a value indicating whether it is necessary to use NewPage command instead of NewSection.")]
                [StiSerializable]
                public static bool UseNewPageCommandInsteadOfNewSection { get; set; }
            }

            #region Obsoleted
            /// <summary>
            /// Class for adjustment of the RichText export of a report.
            /// </summary>
            [Obsolete("The class StiOptions.Rtf is obsolete! Please use class StiOptions.RichText instead it.")]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public sealed class Rtf : RichText
            {

            }
            #endregion
            
		}
    }
}