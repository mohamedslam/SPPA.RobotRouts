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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Components;

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
            /// The default value for the TextQuality property.
            /// </summary>
            [DefaultValue(StiTextQuality.Standard)]
            [Description("The default value for the TextQuality property.")]
            [StiSerializable]
            [Category("Engine")]
            public static StiTextQuality DefaultTextQualityMode { get; set; } = StiTextQuality.Standard;

            /// <summary>
            /// Gets or sets a value which indicates whether Gdi32 is used for text rendering. For .Net 2.0 only.
            /// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets a value which indicates whether Gdi32 is used for text rendering. For .Net 2.0 only.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool UseGdi32ForTextRendering { get; set; }

            /// <summary>
            /// A value indicates that text drawing engine will measure text string including trailing spaces.
            /// </summary>
            [DefaultValue(false)]
            [StiSerializable]
            [Description("A value indicates that text drawing engine will measure text string including trailing spaces.")]
            [Category("Engine")]
            public static bool MeasureTrailingSpaces
            {
                get
                {
                    return StiTextDrawing.MeasureTrailingSpaces;
                }
                set
                {
                    StiTextDrawing.MeasureTrailingSpaces = value;
                }
            }

            [DefaultValue(false)]
            [StiSerializable]
            [Category("Engine")]
            public static bool UseOldWYSIWYGTextQuality { get; set; }

            /// <summary>
            /// Gets or sets a value which defines a previous realization of the WYSIWYG mode of the StiText component.
            /// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets a value which defines a previous realization of the WYSIWYG mode of the StiText component.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool OldWYSIWYG { get; set; }

            /// <summary>
            /// Gets or sets a value forcing drawing rectangles instead of font with tiny size.
            /// </summary>
            [DefaultValue(true)]
            [Description("Gets or sets a value forcing drawing rectangles instead of font with tiny size.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool TinyTextOptimization { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which allow using cache for GetActualSize methods.
            /// </summary>
            [DefaultValue(true)]
            [Description("Gets or sets a value which allow using cache for GetActualSize methods.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool AllowCacheForGetActualSize { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which allow assign empty string in condition of textbox. 
            /// For backward compatibility with 2016.2
            /// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets a value which allow assign empty string in condition of textbox. For back compatibility with 2016.2")]
            [StiSerializable]
            [Category("Engine")]
            public static bool AllowAssignEmptyStringInCondition { get; set; }

            /// <summary>
            /// Gets or sets a value of the font scaling for print with Standard TextQuality mode, in percent. 
            /// </summary>
            [DefaultValue(96)]
            [Description("Gets or sets a value of the font scaling for print with Standard TextQuality mode, in percent.")]
            [StiSerializable]
            [Category("Engine")]
            public static float StandardTextQualityScale { get; set; } = 96;

            /// <summary>
            /// Gets or sets a value of the font scaling for print with Typographic TextQuality mode, in percent. 
            /// </summary>
            [DefaultValue(96)]
            [Description("Gets or sets a value of the font scaling for print with Typographic TextQuality mode, in percent.")]
            [StiSerializable]
            [Category("Engine")]
            public static float TypographicTextQualityScale { get; set; } = 96;

            /// <summary>
            /// Gets or sets a value of the text LineSpacing scaling, usually used for libgdiplus.
            /// </summary>
            [DefaultValue(1.0f)]
            [Description("Gets or sets a value of the text LineSpacing scaling, usually used for libgdiplus.")]
            [StiSerializable]
            [Category("Engine")]
#if NETSTANDARD && !STIDRAWING
            public static float TextLineSpacingScale
            {
                get
                {
                    if (textLineSpacingScale == 0)
                    {
                        textLineSpacingScale = 1;
                        if (StiDpiHelper.IsLinux) textLineSpacingScale = 1.23f;
                        if (StiDpiHelper.IsMacOsX) textLineSpacingScale = 1.23f;
                    }
                    return textLineSpacingScale;
                }
                set
                {
                    textLineSpacingScale = value;
                }
            }
            private static float textLineSpacingScale = 0;
#else
            public static float TextLineSpacingScale { get; set; } = 1f;
#endif

            public static bool MarkBreakedText { get; set; }

#if NETSTANDARD || STIDRAWING
            public static bool UseNewHtmlEngine { get; set; } = true;
#else
            public static bool UseNewHtmlEngine { get; set; } = false;
#endif

            public sealed class Html
            {
                /// <summary>
                /// Gets or sets a value that allows indent for the second line of the list item.
                /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value that allows indent for the second line of the list item.")]
                [StiSerializable]
                public static bool AllowListItemSecondLineIndent
                {
                    get
                    {
                        return Stimulsoft.Base.StiBaseOptions.AllowHtmlListItemSecondLineIndent;
                    }
                    set
                    {
                        Stimulsoft.Base.StiBaseOptions.AllowHtmlListItemSecondLineIndent = value;
                    }
                }
            }

        }
    }
}