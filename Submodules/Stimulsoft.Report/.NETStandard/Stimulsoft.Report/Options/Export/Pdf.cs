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
        public sealed partial class Export
		{	
            /// <summary>
            /// Class for adjustment of the PDF exporting.
            /// </summary>
            public sealed class Pdf
            {
                /// <summary>
                /// Gets or sets a value indicating behavior of the exporting segmented pages will be forcibly broken in order of chosen page format.
                /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value indicating behavior of the exporting segmented pages will be forcibly broken in order of chosen page format.")]
                [StiSerializable]
                public static bool DivideSegmentPages { get; set; } = true;

                /// <summary>
                /// Gets or sets a value which indicates whether it is necessary to convert all digits to arabic digits.
                /// </summary>
                [DefaultValue(false)]
                [Description("Gets or sets a value which indicates whether it is necessary to convert all digits to arabic digits.")]
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
                /// Gets or sets a value indicating whether it is necessary to reduce font file size.
                /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value indicating whether it is necessary to reduce font file size.")]
                [StiSerializable]
                public static bool ReduceFontFileSize { get; set; } = true;

                [DefaultValue(false)]
                [StiSerializable]
                [Obsolete("AllowEditablePdf property is obsolete. Please use StiPdfExportSettings instead.")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static bool AllowEditablePdf { get; set; }

                [DefaultValue(false)]
                [StiSerializable]
                public static bool UseEditableFieldName { get; set; }

                [DefaultValue(false)]
                [StiSerializable]
                public static bool UseEditableFieldAlias { get; set; }

                [DefaultValue(false)]
                [StiSerializable]
                public static bool UseEditableFieldTag { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether it is necessary to use image comparer.
                /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value indicating whether it is necessary to use image comparer.")]
                [StiSerializable]
                public static bool AllowImageComparer { get; set; } = true;

                /// <summary>
                /// Gets or sets a value indicating whether it is necessary to use image transparency.
                /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value indicating whether it is necessary to use image transparency.")]
                [StiSerializable]
                public static bool AllowImageTransparency { get; set; } = true;

                /// <summary>
                /// Gets or sets a value indicating whether it is necessary to use Printable property of the component.
                /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value indicating whether it is necessary to use Printable property of the component.")]
                [StiSerializable]
                public static bool AllowPrintable { get; set; } = true;

                /// <summary>
                /// Gets or sets a value indicating whether it is necessary to allow inherited page resources.
                /// </summary>
                [DefaultValue(false)]
                [Description("Gets or sets a value indicating whether it is necessary to allow inherited page resources.")]
                [StiSerializable]
                public static bool AllowInheritedPageResources { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether it is necessary to allow using of ExtGState.
                /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value indicating whether it is necessary to allow using of ExtGState.")]
                [StiSerializable]
                public static bool AllowExtGState { get; set; } = true;

                /// <summary>
                /// Gets or sets a CreatorString
                /// </summary>
                [DefaultValue("")]
                [Description("Gets or sets a CreatorString.")]
                [StiSerializable]
                public static string CreatorString { get; set; } = string.Empty;

                /// <summary>
                /// Gets or sets a KeywordsString
                /// </summary>
                [DefaultValue("")]
                [Description("Gets or sets a KeywordsString.")]
                [StiSerializable]
                public static string KeywordsString { get; set; } = string.Empty;

                /// <summary>
                /// Gets or sets a value indicating whether it is necessary to use fonts cache.
                /// </summary>
                [DefaultValue(false)]
                [Description("Gets or sets a value indicating whether it is necessary to use fonts cache.")]
                [StiSerializable]
                public static bool AllowFontsCache { get; set; }

                /// <summary>
                /// Gets or sets a value indicating the possibility to use the windows system libraries for working with fonts.
                /// </summary>
                [Obsolete("AllowImportSystemLibraries property is obsolete. Please use AllowInvokeWindowsLibraries instead.")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static bool AllowImportSystemLibraries
                {
                    get => AllowInvokeWindowsLibraries;
                    set => AllowInvokeWindowsLibraries = value;
                }

                /// <summary>
                /// Gets or sets a value indicating the possibility to use the windows system libraries for working with fonts.
                /// </summary>
                [DefaultValue(false)]
                [Description("Gets or sets a value indicating the possibility to use the windows system libraries for working with fonts.")]
                [StiSerializable]
#if NETSTANDARD
                public static bool AllowInvokeWindowsLibraries { get; set; }
#else
                public static bool AllowInvokeWindowsLibraries { get; set; } = true;
#endif

                /// <summary>
                /// Gets or sets a value indicating whether it is necessary to force the use of the FontsInfoStore for Wysiwyg mode.
                /// </summary>
                [DefaultValue(false)]
                [Description("Gets or sets a value indicating whether it is necessary to force the use of the FontsInfoStore for Wysiwyg mode.")]
                [StiSerializable]
                public static bool ForceUseFontsInfoStoreForWysiwyg { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether it is necessary to use old mode of the AllowHtmlTags rendering.
                /// </summary>
                [DefaultValue(false)]
                [Description("Gets or sets a value indicating whether it is necessary to use old mode of the AllowHtmlTags rendering.")]
                [StiSerializable]
                public static bool UseOldModeAllowHtmlTags { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether it is necessary to use wysiwyg render.
                /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value indicating whether it is necessary to use wysiwyg render.")]
                [StiSerializable]
                public static bool UseWysiwygRender { get; set; } = true;

                /// <summary>
                /// Gets or sets a value indicating default coordinates precision.
                /// </summary>
                [DefaultValue(2)]
                [Description("Gets or sets a value indicating default coordinates precision.")]
                [StiSerializable]
                public static int DefaultCoordinatesPrecision { get; set; } = 2;

                /// <summary>
                /// Gets or sets a value indicating default AutoPrint mode.
                /// </summary>
                [DefaultValue(StiPdfAutoPrintMode.None)]
                [Description("Gets or sets a value indicating default AutoPrint mode.")]
                [StiSerializable]
                public static StiPdfAutoPrintMode DefaultAutoPrintMode { get; set; } = StiPdfAutoPrintMode.None;


                /// <summary>
                /// Gets or sets a value indicating whether it is necessary to use high accuracy wordwrap in Wpf mode.
                /// </summary>
                [Obsolete("WpfHighAccuracyWordWrap property is obsolete. Please use WpfHighAccuracyMode instead.")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static bool WpfHighAccuracyWordWrap
                {
                    get => WpfHighAccuracyMode == StiPdfHighAccuracyMode.All;
                    set => WpfHighAccuracyMode = StiPdfHighAccuracyMode.All;
                }

                /// <summary>
                /// Gets or sets a value indicating whether it is necessary to use high accuracy wordwrap in WinForms mode.
                /// </summary>
                [Obsolete("WinFormsHighAccuracyWordWrap property is obsolete. Please use WinFormsHighAccuracyMode instead.")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static bool WinFormsHighAccuracyWordWrap
                {
                    get => WinFormsHighAccuracyMode == StiPdfHighAccuracyMode.All;
                    set => WinFormsHighAccuracyMode = StiPdfHighAccuracyMode.All;
                }

                /// <summary>
                /// Gets or sets a value indicating whether it is necessary to use high accuracy in Wpf mode.
                /// </summary>
                [DefaultValue(StiPdfHighAccuracyMode.No)]
                [Description("Gets or sets a value indicating whether it is necessary to use high accuracy in Wpf mode.")]
                [StiSerializable]
                public static StiPdfHighAccuracyMode WpfHighAccuracyMode { get; set; } = StiPdfHighAccuracyMode.No;

                /// <summary>
                /// Gets or sets a value indicating whether it is necessary to use high accuracy in WinForms mode.
                /// </summary>
                [DefaultValue(StiPdfHighAccuracyMode.No)]
                [Description("Gets or sets a value indicating whether it is necessary to use high accuracy in WinForms mode.")]
                [StiSerializable]
                public static StiPdfHighAccuracyMode WinFormsHighAccuracyMode { get; set; } = StiPdfHighAccuracyMode.No;


                /// <summary>
                /// Gets or sets a value indicating the Reason field of digital signature.
                /// </summary>
                [DefaultValue(null)]
                [Description("Gets or sets a value indicating the Reason field of digital signature.")]
                [StiSerializable]
                public static string DigitalSignatureReason { get; set; }

                /// <summary>
                /// Gets or sets a value indicating the Location field of digital signature.
                /// </summary>
                [DefaultValue(null)]
                [Description("Gets or sets a value indicating the Location field of digital signature.")]
                [StiSerializable]
                public static string DigitalSignatureLocation { get; set; }

                /// <summary>
                /// Gets or sets a value indicating the ContactInfo field of digital signature.
                /// </summary>
                [DefaultValue(null)]
                [Description("Gets or sets a value indicating the ContactInfo field of digital signature.")]
                [StiSerializable]
                public static string DigitalSignatureContactInfo { get; set; }

                /// <summary>
                /// Gets or sets a value indicating the SignedBy field of digital signature.
                /// </summary>
                [DefaultValue(null)]
                [Description("Gets or sets a value indicating the SignedBy field of digital signature.")]
                [StiSerializable]
                public static string DigitalSignatureSignedBy { get; set; }

                /// <summary>
                /// Gets or sets a value indicating the digest used in the digital signature.
                /// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value indicating the digest used in the digital signature.")]
                [StiSerializable]
                public static bool DigitalSignatureDigestSHA256 { get; set; } = true;

                /// <summary>
                /// Gets or sets a value indicating whether it is necessary to disable DigitalSignature button in the export menu.
                /// </summary>
                [DefaultValue(false)]
                [Description("Gets or sets a value indicating whether it is necessary to disable DigitalSignature button in the export menu.")]
                [StiSerializable]
                public static bool DisableDigitalSignatureButtonInExportMenu { get; set; }

                /// <summary>
                /// Gets or sets a value indicating a BarCode rendering mode.
                /// </summary>
                [DefaultValue(false)]
                [Description("Gets or sets a value indicating a BarCode rendering mode.")]
                [StiSerializable]
                public static bool RenderBarCodeAsImage { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether it is necessary to order components by placement.
                /// </summary>
                [DefaultValue(false)]
                [Description("Gets or sets a value indicating whether it is necessary to order components by placement.")]
                [StiSerializable]
                public static bool OrderComponentsByPlacement { get; set; }

                /// <summary>
                /// Gets or sets a value of vertical scaling for RichText in Wpf mode.
                /// </summary>
                [DefaultValue(0.95)]
                [Description("Gets or sets a value of vertical scaling for RichText in Wpf mode.")]
                [StiSerializable]
                public static float WpfRichTextVerticalScaling { get; set; } = 0.95f;

                /// <summary>
                /// Class for adjustment of the PDF security.
                /// </summary>
                public sealed class Security
                {
                    /// <summary>
                    /// Gets or sets a default UserPassword
                    /// </summary>
                    [DefaultValue(null)]
                    [Description("Gets or sets a default UserPassword.")]
                    [StiSerializable]
                    public static string DefaultUserPassword { get; set; }

                    /// <summary>
                    /// Gets or sets a default OwnerPassword
                    /// </summary>
                    [DefaultValue(null)]
                    [Description("Gets or sets a default OwnerPassword.")]
                    [StiSerializable]
                    public static string DefaultOwnerPassword { get; set; }
                }
            }
		}
    }
}
