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
using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Serializing;
using System.Drawing.Printing;

namespace Stimulsoft.Report
{
    /// <summary>
    /// Class for adjustment all aspects of Stimulsoft Reports.
    /// </summary>
    public sealed partial class StiOptions
	{
        /// <summary>
        /// Class for adjustment of the report printing.
        /// </summary>
        public sealed class Print
        {
            /// <summary>
            /// Gets or sets a value which indicating that printer settings will be stored in report.PrinterSetting property after printer dialog.
            /// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets a value which indicating that printer settings will be stored in report.PrinterSetting property after printer dialog.")]
            [StiSerializable]
            public static bool StorePrinterSettingsInReportAfterPrintDialog { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether to render charts as Bitmap.
            /// </summary>
            [DefaultValue(true)]
            [Description("Gets or sets a value indicating whether to render charts as Bitmap.")]
            [StiSerializable]
            public static bool ChartAsBitmap { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicating that PaperSize and PaperSource from PrintDialog
            /// will be used for all pages if they changed in the PrintDialog.
            /// Otherwise, each page will be printed with their own settings.
            /// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets a value which indicating that PaperSize and PaperSource from PrintDialog will be used for all pages if they changed in the PrintDialog.")]
            [StiSerializable]
            public static bool UsePaperSizeAndSourceFromPrintDialogIfTheyChanged { get; set; }

            /// <summary>
            /// Gets or sets a value which indicating that PaperSize and PaperSource from PrintDialog
            /// will be used for all pages always.
            /// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets a value which indicating that PaperSize and PaperSource from PrintDialog will be used for all pages always.")]
            [StiSerializable]
            public static bool UsePaperSizeAndSourceFromPrintDialogAlways { get; set; }

            /// <summary>
            /// Gets or sets a value which indicating that PrinterSettings must be used entirely.
            /// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets a value which indicating that PrinterSettings must be used entirely.")]
            [StiSerializable]
            public static bool UsePrinterSettingsEntirely { get; set; }

            /// <summary>
            /// Gets or sets a value which force to find the custom PaperSize in the standard PaperSizes
            /// </summary>
            [DefaultValue(true)]
            [Description("Gets or sets a value which force to find the custom PaperSize in the standard PaperSizes.")]
            [StiSerializable]
            public static bool FindCustomPaperSizeInStandardPaperSizes { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which force to use PaperSizes collection from PrinterSettings
            /// </summary>
            [DefaultValue(true)]
            [Description("Gets or sets a value which force to use PaperSizes collection from PrinterSettings.")]
            [StiSerializable]
            public static bool AllowUsePaperSizesFromPrinterSettings { get; set; } = true;

            /// <summary>
            /// Gets or sets the page horizontal alignment
            /// </summary>
            [DefaultValue(StiHorAlignment.Left)]
            [Description("Gets or sets the page horizontal alignment.")]
            [StiSerializable]
            public static StiHorAlignment PageHorAlignment { get; set; } = StiHorAlignment.Left;

            /// <summary>
            /// Gets or sets the page vertical alignment
            /// </summary>
            [DefaultValue(StiVertAlignment.Top)]
            [Description("Gets or sets the page vertical alignment.")]
            [StiSerializable]
            public static StiVertAlignment PageVertAlignment { get; set; } = StiVertAlignment.Top;

            /// <summary>
            /// Gets or sets a value indicating whether to render metafile as Bitmap.
            /// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets a value indicating whether to render metafile as Bitmap.")]
            [StiSerializable]
            public static bool MetafileAsBitmap { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether to fix problem with invariant culture when printing in wpf.
            /// </summary>
            [DefaultValue(true)]
            [Description("Gets or sets a value indicating whether to fix problem with invariant culture when printing in wpf.")]
            [StiSerializable]
            public static bool FixNet46WpfCulture { get; set; } = true;

            [DefaultValue(null)]
            [StiSerializable]
            public static string DotMatrixFormatSequence { get; set; }

            public static PrinterSettings.PaperSizeCollection CustomPaperSizes { get; set; }

            public static PrinterSettings.PaperSourceCollection CustomPaperSources { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether to fix the banded gradients on some printers.
            /// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets a value indicating whether to fix the banded gradients on some printers.")]
            [StiSerializable]
            public static bool FixBandedGradients
            {
                get
                {
                    return StiBaseOptions.FixBandedGradients; 
                }
                set
                {
                    StiBaseOptions.FixBandedGradients = value; 
                }
            }

            /// <summary>
            /// Gets or sets a value indicating whether to render barcodes as Bitmap.
            /// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets a value indicating whether to render barcodes as Bitmap.")]
            [StiSerializable]
            public static bool BarcodeAsBitmap { get; set; }

        }
    }
}