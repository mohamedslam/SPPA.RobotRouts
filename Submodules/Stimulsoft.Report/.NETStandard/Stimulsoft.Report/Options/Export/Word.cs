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
using Stimulsoft.Report.Export;

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
        public partial class Export
        {
            /// <summary>
            /// Class for adjustment of the Word2007 export of a report.
            /// </summary>
            public class Word
            {
                /// <summary>
                /// Gets or sets a value indicating behavior of the exporting segmented pages will be forcibly broken in order of chosen page format.
                /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value indicating behavior of the exporting segmented pages will be forcibly broken in order of chosen page format.")]
                [StiSerializable]
                public static bool DivideSegmentPages { get; set; } = true;

                /// <summary>
                /// Gets or sets a value indicating whether it is necessary to use image comparer.
                /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value indicating whether it is necessary to use image comparer.")]
                [StiSerializable]
                public static bool AllowImageComparer { get; set; } = true;

                /// <summary>
                ///  Gets or sets a value indicating whether it is necessary to remove empty space at the bottom of page
                /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value indicating whether it is necessary to remove empty space at the bottom of page.")]
                public static bool RemoveEmptySpaceAtBottom { get; set; } = true;

                /// <summary>
                ///  Gets or sets a value of expansion or compression of the space between characters in twips; a negative value compresses.
                /// </summary>
                [DefaultValue(-2)]
                [Description("Gets or sets a value of expansion or compression of the space between characters in twips; a negative value compresses.")]
                [StiSerializable]
                public static int SpaceBetweenCharacters { get; set; } = -2;

                /// <summary>
                /// Gets or sets a value indicating a line height mode - "exactly" or "at least".
                /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value indicating a line height mode - 'exactly' or 'at least'.")]
                [StiSerializable]
                public static bool LineHeightExactly { get; set; } = true;

                /// <summary>
                /// Gets or sets a value indicating a line height mode - "exactly" or "at least" for "UsePageHeadersAndFooters" mode
                /// </summary>
                [DefaultValue(false)]
                [Description("Gets or sets a value indicating a line height mode - 'exactly' or 'at least' for 'UsePageHeadersAndFooters' mode.")]
                [StiSerializable]
                public static bool LineHeightExactlyForPHFMode { get; set; }

                /// <summary>
                /// Gets or sets a value indicating a forcing of line height.
                /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value indicating a forcing of line height.")]
                [StiSerializable]
                public static bool ForceLineHeight { get; set; } = true;

                /// <summary>
                ///  Gets or sets a value of right margin correction
                /// </summary>
                [DefaultValue(0)]
                [Description("Gets or sets a value of right margin correction.")]
                [StiSerializable]
                public static int RightMarginCorrection { get; set; }

                /// <summary>
                /// Gets or sets a value of bottom margin correction
                /// </summary>
                [DefaultValue(50)]
                [Description("Gets or sets a value of bottom margin correction.")]
                [StiSerializable]
                public static int BottomMarginCorrection { get; set; } = 50;

                /// <summary>
                /// Gets or sets a value indicating a RichText rendering mode.
                /// </summary>
                [DefaultValue(false)]
                [Description("Gets or sets a value indicating a RichText rendering mode.")]
                [StiSerializable]
                public static bool RenderRichTextAsImage { get; set; }

                /// <summary>
                /// Gets or sets a value indicating a Html tags rendering mode.
                /// </summary>
                [DefaultValue(false)]
                [Description("Gets or sets a value indicating a Html tags rendering mode.")]
                [StiSerializable]
                public static bool RenderHtmlTagsAsImage { get; set; }

                /// <summary>
                /// Gets or sets a value indicating a Html tags rendering mode.
                /// </summary>
                [DefaultValue(false)]
                [Description("Gets or sets a value indicating a Html tags rendering mode.")]
                [StiSerializable]
                public static bool RenderHtmlTagsAsEmbeddedRichText { get; set; }

                /// <summary>
                /// Gets or sets a value indicating a checkbox rendering mode.
                /// </summary>
                [DefaultValue(false)]
                [Description("Gets or sets a value indicating a checkbox rendering mode.")]
                [StiSerializable]
                public static bool RenderCheckBoxAsImage { get; set; }

                /// <summary>
                /// This value indicates that it is necessary to correct problem with FontSize=11 in embedded RichTexts.
                /// </summary>
                [DefaultValue(true)]
                [Description("This value indicates that it is necessary to correct problem with FontSize=11 in embedded RichTexts.")]
                [StiSerializable]
                public static bool AllowCorrectFontSize11Problem { get; set; } = true;

                /// <summary>
                ///  Gets or sets a value of line spacing correction in embedded RichTexts.
                /// </summary>
                [DefaultValue(0.95d)]
                [Description("Gets or sets a value of line spacing correction in embedded RichTexts.")]
                [StiSerializable]
                public static double RichTextLineSpacingCorrection { get; set; } = 0.95d;

                /// <summary>
                ///  Gets or sets a value of default FontSize for the Normal style
                /// </summary>
                [DefaultValue(1f)]
                [Description("Gets or sets a value of default FontSize for the Normal style.")]
                [StiSerializable]
                public static float NormalStyleDefaultFontSize { get; set; } = 1f;

                /// <summary>
                ///  Gets or sets a value of line spacing correction
                /// </summary>
                [DefaultValue(0.955d)]
                [Description("Gets or sets a value of line spacing correction.")]
                [StiSerializable]
                public static double LineSpacing { get; set; } = 0.955d;

                /// <summary>
                /// Gets or sets a value indicating behavior of the exporting big cells will be divided into small cells.
                /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value indicating behavior of the exporting big cells will be divided into small cells.")]
                [StiSerializable]
                public static bool DivideBigCells { get; set; } = true;

                /// <summary>
                /// Gets or sets a value indicating version of the document compatibility mode.
                /// Actual values: 2007=12, 2010=14, 2013=15, 2016=16.
                /// </summary>
                [DefaultValue(15)]
                [Description("Gets or sets a value indicating version of the document compatibility mode.")]
                [StiSerializable]
                public static int CompatibilityModeValue { get; set; } = 15;


                [DefaultValue(StiWord2007RestrictEditing.No)]
                [StiSerializable]
                public static StiWord2007RestrictEditing RestrictEditing { get; set; } = StiWord2007RestrictEditing.No;
            }

            #region Obsoleted
            [Obsolete("The class StiOptions.Word2007 is obsolete! Please use class StiOptions.Word instead it.")]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public sealed class Word2007 : Word
            {

            }
            #endregion
        }
	}
}
