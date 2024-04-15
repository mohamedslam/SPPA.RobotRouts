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
            /// <summary>
            /// Gets or sets a value which controls of output barcodes as bitmaps in WebViewerFx.
            /// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets a value which controls of output barcodes as bitmaps in WebViewerFx.")]
            [StiSerializable]
            [Category("BarCode")]
            public static bool RenderBarCodeAsBitmap { get; set; }

            /// <summary>
            /// Gets or sets a value of barcode dpi multiplier factor.
            /// </summary>
            [DefaultValue(3f)]
            [Description("Gets or sets a value of barcode dpi multiplier factor.")]
            [StiSerializable]
            [Category("Engine")]
            public static float BarcodeDpiMultiplierFactor { get; set; } = 3f;

            /// <summary>
            /// Gets or sets a value of a default encoding of byte mode.
            /// </summary>
            [DefaultValue(BarCodes.StiQRCodeECIMode.ISO_8859_1)]
            [Description("Gets or sets a value of a default encoding of byte mode.")]
            [StiSerializable]
            [Category("Engine")]
            public static BarCodes.StiQRCodeECIMode BarcodeQRCodeDefaultByteModeEncoding { get; set; } = BarCodes.StiQRCodeECIMode.ISO_8859_1;

            /// <summary>
            /// Gets or sets a value which allow to use BOM for the Unicode encoding in the QR-Code.
            /// </summary>
            [DefaultValue(true)]
            [Description("Gets or sets a value which allow to use BOM for the Unicode encoding in the QR-Code.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool BarcodeQRCodeAllowUnicodeBOM { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which force improve barcode quality in HTML export
            /// </summary>
            [DefaultValue(true)]
            [Description("Gets or sets a value which allow to use BOM for the Unicode encoding in the QR-Code.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool BarcodeImproveQualityHtmlExport { get; set; } = false;
        }
    }
}
