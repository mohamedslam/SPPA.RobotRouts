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
using System.Collections.Generic;
using System;
using Stimulsoft.Base.Json.Converters;
using Stimulsoft.Base.Json;

namespace Stimulsoft.Report.Export
{
    /// <summary>
    /// Class describes settings for export to Adobe PDF format.
    /// </summary>
	public class StiPdfExportSettings : 
        StiPageRangeExportSettings
    {
        #region Methods
        public override StiExportFormat GetExportFormat()
        {
            return StiExportFormat.Pdf;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets image quality of images which will be exported to result PDF file.
        /// </summary>
        [DefaultValue(0.75f)]
		public float ImageQuality { get; set; } = 0.75f;

        [DefaultValue(StiImageResolutionMode.Exactly)]
        [JsonConverter(typeof(StringEnumConverter))]
        public StiImageResolutionMode ImageResolutionMode { get; set; } = StiImageResolutionMode.Exactly;

        /// <summary>
        /// Gets or sets image resolution of images which will be exported to result PDF file.
        /// </summary>
        [DefaultValue(100f)]
		public float ImageResolution { get; set; } = 100f;

        /// <summary>
        /// Gets or sets value which indicates that fonts which used in report will be included in PDF file.
        /// </summary>
        [DefaultValue(true)]
		public bool EmbeddedFonts { get; set; } = true;

        /// <summary>
        /// Gets or sets value which indicates that only standard PDF fonts will be used in result PDF file.
        /// </summary>
        [DefaultValue(false)]
		public bool StandardPdfFonts { get; set; }

        /// <summary>
        /// Gets or sets value which indicates that result file will be used compression.
        /// </summary>
        [DefaultValue(true)]
		public bool Compressed { get; set; } = true;

        /// <summary>
        /// Gets or sets value which indicates that rtf text will be exported as bitmap images or as vector images.
        /// </summary>
        [DefaultValue(false)]
		public bool ExportRtfTextAsImage { get; set; }

        /// <summary>
        /// Gets or sets user password for created PDF file.
        /// </summary>
        [DefaultValue("")]
		public string PasswordInputUser { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets owner password for created PDF file.
        /// </summary>
        [DefaultValue("")]
		public string PasswordInputOwner { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets user access privileges when Adobe PDF file is viewing.
        /// </summary>
        [DefaultValue(StiUserAccessPrivileges.All)]
        [JsonConverter(typeof(StringEnumConverter))]
        public StiUserAccessPrivileges UserAccessPrivileges { get; set; } = StiUserAccessPrivileges.All;

        /// <summary>
        /// Gets or sets length of encryption key.
        /// </summary>
        [DefaultValue(StiPdfEncryptionKeyLength.Bit40)]
        [JsonConverter(typeof(StringEnumConverter))]
        public StiPdfEncryptionKeyLength KeyLength { get; set; } = StiPdfEncryptionKeyLength.Bit40;

        /// <summary>
        /// Gets or sets value which indicates that unicode symbols must be used in result PDF file.
        /// </summary>
        [DefaultValue(true)]
		public bool UseUnicode { get; set; } = true;

        /// <summary>
        /// Gets or sets value which indicates that digital signature is used for creating PDF file.
        /// </summary>
        [DefaultValue(false)]
		public bool UseDigitalSignature { get; set; }

        /// <summary>
        /// Gets or sets value which indicates that certificate for PDF file digital signing must be get with help of special GUI.
        /// </summary>
        [DefaultValue(false)]
		public bool GetCertificateFromCryptoUI { get; set; } = false;

        /// <summary>
        /// Gets or sets subject name string which will be used in digital signature of result PDF file.
        /// </summary>
        [DefaultValue("")]
		public string SubjectNameString { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets value which indicates that certificates from local machine certificate store or from current user certificate store will be used for digital signature.
        /// </summary>
        [DefaultValue(false)]
		public bool UseLocalMachineCertificates { get; set; }

        /// <summary>
        /// Gets or sets raw certificate data for digital signature.
        /// </summary>
        [DefaultValue(null)]
        public byte[] CertificateData { get; set; }

        /// <summary>
        /// Gets or sets raw certificate thumbprint for digital signature.
        /// </summary>
        [DefaultValue(null)]
        public string CertificateThumbprint { get; set; }

        /// <summary>
        /// Gets or sets password for certificate for digital signature.
        /// </summary>
        [DefaultValue(null)]
        public string CertificatePassword { get; set; }

        /// <summary>
        /// Gets or sets information about the creator to be inserted into result PDF file.
        /// </summary>
        [DefaultValue("")]
		public string CreatorString { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets keywords information to be inserted into result PDF file.
        /// </summary>
        [DefaultValue("")]
		public string KeywordsString { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets mode of image compression in PDF file.
        /// </summary>
        [DefaultValue(StiPdfImageCompressionMethod.Jpeg)]
        [JsonConverter(typeof(StringEnumConverter))]
        public StiPdfImageCompressionMethod ImageCompressionMethod { get; set; } = StiPdfImageCompressionMethod.Jpeg;

        /// <summary>
        /// Gets or sets a Palette size for the Indexed color mode of image compression.
        /// </summary>
        [DefaultValue(32)]
        public int ImageIndexedColorPaletteSize { get; set; } = 96;

        /// <summary>
        /// Gets or sets image format for exported images.
        /// </summary>
        [DefaultValue(StiImageFormat.Color)]
        [JsonConverter(typeof(StringEnumConverter))]
        public StiImageFormat ImageFormat { get; set; } = StiImageFormat.Color;

        /// <summary>
        /// Gets or sets type of dithering.
        /// </summary>
        [DefaultValue(StiMonochromeDitheringType.FloydSteinberg)]
        [JsonConverter(typeof(StringEnumConverter))]
        public StiMonochromeDitheringType DitheringType { get; set; } = StiMonochromeDitheringType.FloydSteinberg;

        /// <summary>
        /// Obsolete. Please use PdfComplianceMode instead.
        /// </summary>
        [Obsolete("Please use PdfComplianceMode")]
        [DefaultValue(false)]
        public bool PdfACompliance
        {
            get
            {
                return PdfComplianceMode != StiPdfComplianceMode.None;
            }
            set
            {
                PdfComplianceMode = value ? StiPdfComplianceMode.A1 : StiPdfComplianceMode.None;
            }
        }

        /// <summary>
        /// Gets or sets value which indicates the PDF/A compliance mode.
        /// </summary>
        [DefaultValue(StiPdfComplianceMode.None)]
        [JsonConverter(typeof(StringEnumConverter))]
        public StiPdfComplianceMode PdfComplianceMode { get; set; } = StiPdfComplianceMode.None;

        /// <summary>
        /// Gets or sets a value indicating AutoPrint mode
        /// </summary>
        [DefaultValue(StiPdfAutoPrintMode.None)]
        [JsonConverter(typeof(StringEnumConverter))]
        public StiPdfAutoPrintMode AutoPrintMode { get; set; } = StiOptions.Export.Pdf.DefaultAutoPrintMode;

        [DefaultValue(StiPdfAllowEditable.No)]
        [JsonConverter(typeof(StringEnumConverter))]
        public StiPdfAllowEditable AllowEditable { get; set; } = StiPdfAllowEditable.No;

        /// <summary>
        /// Gets or sets a value indicating the Reason field of digital signature.
        /// </summary>
        [DefaultValue(null)]
        public string DigitalSignatureReason { get; set; } = StiOptions.Export.Pdf.DigitalSignatureReason;

        /// <summary>
        /// Gets or sets a value indicating the Location field of digital signature.
        /// </summary>
        [DefaultValue(null)]
        public string DigitalSignatureLocation { get; set; } = StiOptions.Export.Pdf.DigitalSignatureLocation;

        /// <summary>
        /// Gets or sets a value indicating the ContactInfo field of digital signature.
        /// </summary>
        [DefaultValue(null)]
        public string DigitalSignatureContactInfo { get; set; } = StiOptions.Export.Pdf.DigitalSignatureContactInfo;

        /// <summary>
        /// Gets or sets a value indicating the SignedBy field of digital signature.
        /// </summary>
        [DefaultValue(null)]
        public string DigitalSignatureSignedBy { get; set; } = StiOptions.Export.Pdf.DigitalSignatureSignedBy;

        /// <summary>
        /// Gets or sets a list of embedded files info.
        /// </summary>
        [DefaultValue(null)]
        public List<StiPdfEmbeddedFileData> EmbeddedFiles { get; set; } = new List<StiPdfEmbeddedFileData>();

        /// <summary>
        /// Obsolete. Please use ZUGFeRDComplianceMode instead.
        /// </summary>
        [Obsolete("Please use ZUGFeRDComplianceMode")]
        [DefaultValue(false)]
        public bool ZUGFeRDCompliance
        {
            get
            {
                return ZUGFeRDComplianceMode != StiPdfZUGFeRDComplianceMode.None;
            }
            set
            {
                ZUGFeRDComplianceMode = value ? StiPdfZUGFeRDComplianceMode.V1 : StiPdfZUGFeRDComplianceMode.None;
            }
        }

        /// <summary>
        /// Gets or sets value which indicates the ZUGFeRD compliance mode.
        /// </summary>
        [DefaultValue(StiPdfZUGFeRDComplianceMode.None)]
        [JsonConverter(typeof(StringEnumConverter))]
        public StiPdfZUGFeRDComplianceMode ZUGFeRDComplianceMode { get; set; } = StiPdfZUGFeRDComplianceMode.None;

        /// <summary>
        /// Gets or sets value which indicates the ZUGFeRD Conformance Level.
        /// </summary>
        [DefaultValue("BASIC")]
        public string ZUGFeRDConformanceLevel { get; set; } = "BASIC";

        /// <summary>
        /// Gets or sets value of the ZUGFeRD Invoice data.
        /// </summary>
        [DefaultValue(null)]
        public byte[] ZUGFeRDInvoiceData { get; set; } = null;
        #endregion
    }
}
