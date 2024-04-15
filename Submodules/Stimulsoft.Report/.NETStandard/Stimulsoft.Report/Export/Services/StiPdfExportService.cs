#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
{	                         										}
{																	}
{	Copyright (C) 2003-2022 Stimulsoft   							}
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
{	TRADE SECRETS OF STIMULSOFT										}
{																	}
{	CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON		}
{	ADDITIONAL RESTRICTIONS.										}
{																	}
{*******************************************************************}
*/
#endregion Copyright (C) 2003-2022 Stimulsoft 

using Stimulsoft.Base;
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Licenses;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Maps.Geoms;
using Stimulsoft.Base.Services;
using Stimulsoft.Gauge.Painters;
using Stimulsoft.Report.BarCodes;
using Stimulsoft.Report.Chart;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Engine;
using Stimulsoft.Report.Gauge;
using Stimulsoft.Report.Gauge.GaugeGeoms;
using Stimulsoft.Report.Gauge.Helpers;
using Stimulsoft.Report.Maps;
using Stimulsoft.Report.Painters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Xml;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Helpers;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Components.MathFormula;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

#if CLOUD
using Stimulsoft.Base.Plans;
#endif

#if NETSTANDARD
using Stimulsoft.System.Security.Cryptography;
using Stimulsoft.System.Drawing;
#else
using System.Security.Cryptography;
#endif

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using StringFormat = Stimulsoft.Drawing.StringFormat;
using HatchBrush = Stimulsoft.Drawing.Drawing2D.HatchBrush;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using Font = Stimulsoft.Drawing.Font;
using Metafile = Stimulsoft.Drawing.Imaging.Metafile;
using FontFamily = Stimulsoft.Drawing.FontFamily;
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Image = Stimulsoft.Drawing.Image;
using ImageFormat = Stimulsoft.Drawing.Imaging.ImageFormat;
using Matrix = Stimulsoft.Drawing.Drawing2D.Matrix;
using Pen = Stimulsoft.Drawing.Pen;
using ImageAttributes = Stimulsoft.Drawing.Imaging.ImageAttributes;
#endif

namespace Stimulsoft.Report.Export
{
    /// <summary>
    /// Class for the Pdf export.
    /// </summary>
	[SuppressUnmanagedCodeSecurity]
    [StiServiceBitmap(typeof(StiExportService), "Stimulsoft.Report.Images.Dictionary.ResourcePdf.png")]
    public partial class StiPdfExportService : StiExportService
    {
        #region StiExportService override
        /// <summary>
		/// Gets or sets a default extension of export. 
		/// </summary>
		public override string DefaultExtension
        {
            get
            {
                return "pdf";
            }
        }


        public override StiExportFormat ExportFormat
        {
            get
            {
                return StiExportFormat.Pdf;
            }
        }

        /// <summary>
        /// Gets a group of the export in the context menu.
        /// </summary>
        public override string GroupCategory
        {
            get
            {
                return "Document";
            }
        }

        /// <summary>
        /// Gets a position of the export in the context menu.
        /// </summary>
        public override int Position
        {
            get
            {
                return (int)StiExportPosition.Pdf;
            }
        }

        /// <summary>
        /// Gets a name of the export in menu.
        /// </summary>
		public override string ExportNameInMenu
        {
            get
            {
                return StiLocalization.Get("Export", "ExportTypePdfFile");
            }
        }

        /// <summary>
        /// Returns a filter for the pdf files.
        /// </summary>
        /// <returns>Returns a filter for the Pdf files.</returns>
		public override string GetFilter()
        {
            return StiLocalization.Get("FileFilters", "PdfFiles");
        }

        /// <summary>
        /// Exports a document to the stream without dialog of the saving file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream in which report will be exported.</param>
        /// <param name="settings">A settings for the report exporting.</param>
        public override void ExportTo(StiReport report, Stream stream, StiExportSettings settings)
        {
            ExportPdf(report, stream, settings as StiPdfExportSettings);
            InvokeExporting(100, 100, 1, 1);
        }

        /// <summary>
        /// Exports rendered report to a pdf file.
        /// Also file may be sent via e-mail.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="fileName">A name of the file for exporting a rendered report.</param>
        /// <param name="sendEMail">A parameter indicating whether the exported report will be sent via e-mail.</param>
        public override void Export(StiReport report, string fileName, bool sendEMail, StiGuiMode guiMode)
        {
            using (var form = StiGuiOptions.GetExportFormRunner("StiPdfExportSetupForm", guiMode, this.OwnerWindow))
            {
                form["CurrentPage"] = report.CurrentPrintPage;
                form["OpenAfterExportEnabled"] = !sendEMail;

                this.report = report;
                this.fileName = fileName;
                this.sendEMail = sendEMail;
                this.guiMode = guiMode;

                form.Complete += form_Complete;
                form.ShowDialog();
            }
        }

        internal StiReport report;
        private string fileName;
        private bool sendEMail;
        private StiGuiMode guiMode;
        private void form_Complete(IStiFormRunner form, StiShowDialogCompleteEvetArgs e)
        {
            if (e.DialogResult)
            {
                if (string.IsNullOrEmpty(fileName))
                    fileName = base.GetFileName(report, sendEMail);

                if (fileName != null)
                {
                    StiFileUtils.ProcessReadOnly(fileName);
                    try
                    {
                        using (var stream = new FileStream(fileName, FileMode.Create))
                        {
                            StartProgress(guiMode);

                            var settings = new StiPdfExportSettings
                            {
                                PageRange = form["PagesRange"] as StiPagesRange,
                                ImageQuality = (float)form["ImageQuality"],
                                ImageCompressionMethod = (StiPdfImageCompressionMethod)form["ImageCompressionMethod"],
                                ImageResolution = (float)form["Resolution"],
                                EmbeddedFonts = (bool)form["EmbeddedFonts"] & (bool)form["EmbeddedFontsEnabled"],
                                ExportRtfTextAsImage = (bool)form["ExportRtfTextAsImage"],
                                PasswordInputUser = form["UserPassword"] as string,
                                PasswordInputOwner = form["OwnerPassword"] as string,
                                UserAccessPrivileges = (StiUserAccessPrivileges)form["UserAccessPrivileges"],
                                KeyLength = (StiPdfEncryptionKeyLength)form["EncryptionKeyLength"],
                                GetCertificateFromCryptoUI = (bool)form["GetCertificateFromCryptoUI"],
                                UseDigitalSignature = (bool)form["UseDigitalSignature"],
                                SubjectNameString = form["SubjectNameString"] as string,
                                PdfComplianceMode = (StiPdfComplianceMode)form["PdfComplianceMode"],
                                ImageFormat = (StiImageFormat)form["ImageFormat"],
                                DitheringType = (StiMonochromeDitheringType)form["MonochromeDitheringType"],
                                AllowEditable = (StiPdfAllowEditable)form["AllowEditable"],
                                ImageResolutionMode = (StiImageResolutionMode)form["ImageResolutionMode"],
                                CertificateThumbprint = (string)form["CertificateThumbprint"]
                            };

                            base.StartExport(report, stream, settings, sendEMail, (bool)form["OpenAfterExport"], fileName, guiMode);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            else IsStopped = true;
        }

        /// <summary>
        /// Gets a value indicating a number of files in exported document as a result of export
        /// of one page of the rendered report.
        /// </summary>
		public override bool MultipleFiles
        {
            get
            {
                return false;
            }
        }
        #endregion

        #region struct StiPdfData
        /// <summary>
        /// Inner representation of the export objects.
        /// </summary>
        internal struct StiPdfData
        {
            /// <summary>
            /// Coordinate of the border.
            /// </summary>
            public double X;

            /// <summary>
            /// Y coordinate of the border.
            /// </summary>
            public double Y;

            /// <summary>
            /// Width of the border.
            /// </summary>
            public double Width;

            /// <summary>
            /// Height of the border.
            /// </summary>
            public double Height;

            /// <summary>
            /// Component.
            /// </summary>
            public StiComponent Component;

            public double Right
            {
                get
                {
                    return X + Width;
                }
            }
            public double Top
            {
                get
                {
                    return Y + Height;
                }
            }

            public StiPdfData Clone()
            {
                StiPdfData pp = new StiPdfData();
                pp.X = X;
                pp.Y = Y;
                pp.Width = Width;
                pp.Height = Height;
                pp.Component = Component;
                return pp;
            }
        }
        #endregion

        #region StiImageData
        private struct StiImageData
        {
            public int Width;
            public int Height;
            public string Name;
            public StiImageFormat ImageFormat;
        }
        #endregion

        #region StiShadingData
        private struct StiShadingData
        {
            public double X;
            public double Y;
            public double Width;
            public double Height;
            public int Page;
            public double Angle;
            public int FunctionIndex;
        }
        private struct StiShadingFunctionData
        {
            public Color Color1;
            public Color Color2;
            public bool IsGlare;
        }
        #endregion

        #region Variables definition
        private float imageQuality = 0.75f;
        private float imageResolutionMain = 1f;
        private StiImageResolutionMode imageResolutionMode = StiImageResolutionMode.Exactly;
        private StreamWriter sw = null;
        private Stream stream2 = null;
        internal MemoryStream memoryPageStream = null;
        internal StreamWriter pageStream = null;
        private global::System.Text.Encoder enc = null;
        private ArrayList imageList = null;
        private StiImageCache imageCache = null;
        private Hashtable imageInterpolationTable = null;
        private Hashtable imageCacheIndexToList = null;
        private Hashtable imageInfoList = null;
        private int imageInfoCounter = 0;
        private int imagesCounter;
        private int imagesCurrent;
        private int fontsCounter;
        private int bookmarksCounter;
        //private int patternsCounter;
        private int linksCounter;
        private int annotsCounter;
        private int annotsCurrent;
        private int annots2Counter;
        private int annots2Current;
        private int unsignedSignaturesCounter;
        private int shadingCounter;
        private int shadingFunctionCounter;
        private int shadingCurrent;
        private int hatchCounter;
        private int tooltipsCounter;
        //private CultureInfo currentCulture = null;
        private NumberFormatInfo currentNumberFormat = null;
        private string[] colorTable = new string[256];
        private bool[] alphaTable = new bool[256];
        internal PdfFonts pdfFont = null;
        //internal StiBidirectionalConvert bidi = null;
        private int precision_digits;
        private bool clipLongTextLines;
        private bool standardPdfFonts = true;
        private bool embeddedFonts = false;
        private bool useUnicodeMode = false;
        private bool reduceFontSize = true;
        private bool compressed = false;
        private bool compressedFonts = false;
        private bool encrypted = false;
        private bool usePdfA = false;
        private StiPdfComplianceMode pdfComplianceMode = StiPdfComplianceMode.None;
        private bool exportRtfTextAsImage = false;
        private StiPdfAutoPrintMode autoPrint = StiPdfAutoPrintMode.None;
        private StiPdfImageCompressionMethod imageCompressionMethod = StiPdfImageCompressionMethod.Jpeg;
        private StiImageFormat imageFormat = StiImageFormat.Color;
        private StiMonochromeDitheringType monochromeDitheringType = StiMonochromeDitheringType.FloydSteinberg;
        private StiPdfAllowEditable allowEditable = StiPdfAllowEditable.No;
        private List<StiPdfEmbeddedFileData> embeddedFiles = null;
        private bool useTransparency = true;
        private StiPdfZUGFeRDComplianceMode zugferdComplianceMode = StiPdfZUGFeRDComplianceMode.None;
        private string zugferdConformanceLevel = "BASIC";

        private bool[] fontGlyphsReduceNotNeed = null;
        private SortedList xref;
        private ArrayList bookmarksTree;
        private ArrayList bookmarksTreeTemp;
        private ArrayList linksArray;
        private List<StiLinkObject> tagsArray;
        private List<StiLinkObject> tooltipsArray;
        private ArrayList annotsArray;
        private List<StiEditableObject> annots2Array;
        private List<StiEditableObject> unsignedSignaturesArray;
        private ArrayList shadingArray;
        private ArrayList hatchArray;
        private List<StiShadingFunctionData> shadingFunctionArray;
        private bool haveBookmarks = false;
        private bool haveLinks = false;
        private bool haveAnnots = false;
        private bool haveDigitalSignature = false;
        private bool haveTooltips = false;
        private int offsetSignatureData = 0;
        private int offsetSignatureFilter = 0;
        private int offsetSignatureName = 0;
        private int offsetSignatureLen2 = 0;

        private static int[] CodePage1252part80AF = {
            0x20AC, 0x2022, 0x201A, 0x0192, 0x201E, 0x2026, 0x2020, 0x2021, 0x02C6, 0x2030, 0x0160, 0x2039, 0x0152, 0x2022, 0x017D, 0x2022,
            0x2022, 0x2018, 0x2019, 0x201C, 0x201D, 0x2022, 0x2013, 0x2014, 0x02DC, 0x2122, 0x0161, 0x203A, 0x0153, 0x2022, 0x017E, 0x0178 };
        private int[] CodePage1252 = new int[256];

        private const double hiToTwips = 0.72;  //convert from hi(hundredths of inch) to points
        private int precision_digits_font = 3;
        private const float pdfCKT = 0.55228f;     //pdf circle round koefficient
        private const float italicAngleTanValue = 0.325f;   //18 degrees
        private const float boldFontStrokeWidthValue = 0.031f;

        private byte[] IDValue = null;
        private string IDValueString = string.Empty;
        private string IDValueStringMeta = string.Empty;
        private string currentDateTime = string.Empty;
        private string currentDateTimeMeta = string.Empty;
        private string producerName = "Stimulsoft Reports";
        private string creatorName = string.Empty;
        private string keywords = string.Empty;

        private int currentObjectNumber;
        private int currentGenerationNumber;
        StiPdfEncryptionKeyLength keyLength = StiPdfEncryptionKeyLength.Bit40;
        private byte lastColorStrokeA = 0xFF;
        private byte lastColorNonStrokeA = 0xFF;
        private Stack colorStack = null;

        private const int signatureDataLen = 16384;
        private RectangleD signaturePlacement;
        private int signaturePageNumber = 0;
        private string digitalSignatureReason = null;
        private string digitalSignatureLocation = null;
        private string digitalSignatureContactInfo = null;
        private string digitalSignatureSignedBy = null;

        private Graphics graphicsForTextRenderer = null;
        private Image imageForGraphicsForTextRenderer = null;
        private Hashtable watermarkImageExist = null;

        private StiPdfStructure info = null;

        private Tools.StiPdfMetafileRender mfRender = null;
        private Tools.StiPdfSecurity pdfSecurity = null;
        private StiPdfGeomWriter tempGeomWriter = null;

        private bool isWpf = false;
        private static bool isWpfException = false;

        private static string[] hatchData =
            {
                "000000FF00000000",	//HatchStyleHorizontal = 0
				"1010101010101010",	//HatchStyleVertical = 1,			
				"8040201008040201",	//HatchStyleForwardDiagonal = 2,	
				"0102040810204080",	//HatchStyleBackwardDiagonal = 3,	
				"101010FF10101010",	//HatchStyleCross = 4,			
				"8142241818244281",	//HatchStyleDiagonalCross = 5,	
				"8000000008000000",	//HatchStyle05Percent = 6,		
				"0010000100100001",	//HatchStyle10Percent = 7,		
				"2200880022008800",	//HatchStyle20Percent = 8,		
				"2288228822882288",	//HatchStyle25Percent = 9,		
				"2255885522558855",	//HatchStyle30Percent = 10,		
				"AA558A55AA55A855",	//HatchStyle40Percent = 11,		
				"AA55AA55AA55AA55",	//HatchStyle50Percent = 12,		
				"BB55EE55BB55EE55",	//HatchStyle60Percent = 13,		
				"DD77DD77DD77DD77",	//HatchStyle70Percent = 14,		
				"FFDDFF77FFDDFF77",	//HatchStyle75Percent = 15,		
				"FF7FFFF7FF7FFFF7",	//HatchStyle80Percent = 16,		
				"FF7FFFFFFFF7FFFF",	//HatchStyle90Percent = 17,		
				"8844221188442211",	//HatchStyleLightDownwardDiagonal = 18,	
				"1122448811224488",	//HatchStyleLightUpwardDiagonal = 19,	
				"CC663399CC663399",	//HatchStyleDarkDownwardDiagonal = 20,	
				"993366CC993366CC",	//HatchStyleDarkUpwardDiagonal = 21,	
				"E070381C0E0783C1",	//HatchStyleWideDownwardDiagonal = 22,	
				"C183070E1C3870E0",	//HatchStyleWideUpwardDiagonal = 23,	
				"4040404040404040",	//HatchStyleLightVertical = 24,			
				"00FF000000FF0000",	//HatchStyleLightHorizontal = 25,		
				"AAAAAAAAAAAAAAAA",	//HatchStyleNarrowVertical = 26,		
				"FF00FF00FF00FF00",	//HatchStyleNarrowHorizontal = 27,		
				"CCCCCCCCCCCCCCCC",	//HatchStyleDarkVertical = 28,			
				"FFFF0000FFFF0000",	//HatchStyleDarkHorizontal = 29,		
				"8844221100000000",	//HatchStyleDashedDownwardDiagonal = 30,
				"1122448800000000",	//HatchStyleDashedUpwardDiagonal = 311,	
				"F00000000F000000",	//HatchStyleDashedHorizontal = 32,		
				"8080808008080808",	//HatchStyleDashedVertical = 33,		
				"0240088004200110",	//HatchStyleSmallConfetti = 34,			
				"0C8DB130031BD8C0",	//HatchStyleLargeConfetti = 35,		
				"8403304884033048",	//HatchStyleZigZag = 36,			
				"00304A8100304A81",	//HatchStyleWave = 37,				
				"0102040818244281",	//HatchStyleDiagonalBrick = 38,		
				"202020FF020202FF",	//HatchStyleHorizontalBrick = 39,	
				"1422518854224588",	//HatchStyleWeave = 40,				
				"F0F0F0F0AA55AA55",	//HatchStylePlaid = 41,				
				"0100201020000102",	//HatchStyleDivot = 42,				
				"AA00800080008000",	//HatchStyleDottedGrid = 43,		
				"0020008800020088",	//HatchStyleDottedDiamond = 44,		
				"8448300C02010103",	//HatchStyleShingle = 45,			
				"33FFCCFF33FFCCFF",	//HatchStyleTrellis = 46,			
				"98F8F877898F8F77",	//HatchStyleSphere = 47,			
				"111111FF111111FF",	//HatchStyleSmallGrid = 48,			
				"3333CCCC3333CCCC",	//HatchStyleSmallCheckerBoard = 49,	
				"0F0F0F0FF0F0F0F0",	//HatchStyleLargeCheckerBoard = 50,	
				"0502058850205088",	//HatchStyleOutlinedDiamond = 51,	
				"10387CFE7C381000",	//HatchStyleSolidDiamond = 52,
				"0000000000000000"	//HatchStyleTotal = 53
			};

        private static List<string> reservedKeywords = new List<string>
        {
            "Title",
            "Author",
            "Subject",
            "Keywords",
            "Creator",
            "Producer",
            "CreationDate",
            "ModDate",
            "Trapped"
        };

        #region sRGBprofile
        private static byte[] sRGBprofile = {
              0,   0,  12,  72,  76, 105, 110, 111,   2,  16,   0,   0, 109, 110, 116, 114,
             82,  71,  66,  32,  88,  89,  90,  32,   7, 206,   0,   2,   0,   9,   0,   6,
              0,  49,   0,   0,  97,  99, 115, 112,  77,  83,  70,  84,   0,   0,   0,   0,
             73,  69,  67,  32, 115,  82,  71,  66,   0,   0,   0,   0,   0,   0,   0,   0,
              0,   0,   0,   0,   0,   0, 246, 214,   0,   1,   0,   0,   0,   0, 211,  45,
             72,  80,  32,  32,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,
              0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,
              0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,
              0,   0,   0,  17,  99, 112, 114, 116,   0,   0,   1,  80,   0,   0,   0,  51,
            100, 101, 115,  99,   0,   0,   1, 132,   0,   0,   0, 108, 119, 116, 112, 116,
              0,   0,   1, 240,   0,   0,   0,  20,  98, 107, 112, 116,   0,   0,   2,   4,
              0,   0,   0,  20, 114,  88,  89,  90,   0,   0,   2,  24,   0,   0,   0,  20,
            103,  88,  89,  90,   0,   0,   2,  44,   0,   0,   0,  20,  98,  88,  89,  90,
              0,   0,   2,  64,   0,   0,   0,  20, 100, 109, 110, 100,   0,   0,   2,  84,
              0,   0,   0, 112, 100, 109, 100, 100,   0,   0,   2, 196,   0,   0,   0, 136,
            118, 117, 101, 100,   0,   0,   3,  76,   0,   0,   0, 134, 118, 105, 101, 119,
              0,   0,   3, 212,   0,   0,   0,  36, 108, 117, 109, 105,   0,   0,   3, 248,
              0,   0,   0,  20, 109, 101,  97, 115,   0,   0,   4,  12,   0,   0,   0,  36,
            116, 101,  99, 104,   0,   0,   4,  48,   0,   0,   0,  12, 114,  84,  82,  67,
              0,   0,   4,  60,   0,   0,   8,  12, 103,  84,  82,  67,   0,   0,   4,  60,
              0,   0,   8,  12,  98,  84,  82,  67,   0,   0,   4,  60,   0,   0,   8,  12,
            116, 101, 120, 116,   0,   0,   0,   0,  67, 111, 112, 121, 114, 105, 103, 104,
            116,  32,  40,  99,  41,  32,  49,  57,  57,  56,  32,  72, 101, 119, 108, 101,
            116, 116,  45,  80,  97,  99, 107,  97, 114, 100,  32,  67, 111, 109, 112,  97,
            110, 121,   0,   0, 100, 101, 115,  99,   0,   0,   0,   0,   0,   0,   0,  18,
            115,  82,  71,  66,  32,  73,  69,  67,  54,  49,  57,  54,  54,  45,  50,  46,
             49,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,  18, 115,  82,  71,
             66,  32,  73,  69,  67,  54,  49,  57,  54,  54,  45,  50,  46,  49,   0,   0,
              0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,
              0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,
              0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,
             88,  89,  90,  32,   0,   0,   0,   0,   0,   0, 243,  81,   0,   1,   0,   0,
              0,   1,  22, 204,  88,  89,  90,  32,   0,   0,   0,   0,   0,   0,   0,   0,
              0,   0,   0,   0,   0,   0,   0,   0,  88,  89,  90,  32,   0,   0,   0,   0,
              0,   0, 111, 162,   0,   0,  56, 245,   0,   0,   3, 144,  88,  89,  90,  32,
              0,   0,   0,   0,   0,   0,  98, 153,   0,   0, 183, 133,   0,   0,  24, 218,
             88,  89,  90,  32,   0,   0,   0,   0,   0,   0,  36, 160,   0,   0,  15, 132,
              0,   0, 182, 207, 100, 101, 115,  99,   0,   0,   0,   0,   0,   0,   0,  22,
             73,  69,  67,  32, 104, 116, 116, 112,  58,  47,  47, 119, 119, 119,  46, 105,
            101,  99,  46,  99, 104,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,
             22,  73,  69,  67,  32, 104, 116, 116, 112,  58,  47,  47, 119, 119, 119,  46,
            105, 101,  99,  46,  99, 104,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,
              0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,
              0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,
              0,   0,   0,   0, 100, 101, 115,  99,   0,   0,   0,   0,   0,   0,   0,  46,
             73,  69,  67,  32,  54,  49,  57,  54,  54,  45,  50,  46,  49,  32,  68, 101,
            102,  97, 117, 108, 116,  32,  82,  71,  66,  32,  99, 111, 108, 111, 117, 114,
             32, 115, 112,  97,  99, 101,  32,  45,  32, 115,  82,  71,  66,   0,   0,   0,
              0,   0,   0,   0,   0,   0,   0,   0,  46,  73,  69,  67,  32,  54,  49,  57,
             54,  54,  45,  50,  46,  49,  32,  68, 101, 102,  97, 117, 108, 116,  32,  82,
             71,  66,  32,  99, 111, 108, 111, 117, 114,  32, 115, 112,  97,  99, 101,  32,
             45,  32, 115,  82,  71,  66,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,
              0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0, 100, 101, 115,  99,
              0,   0,   0,   0,   0,   0,   0,  44,  82, 101, 102, 101, 114, 101, 110,  99,
            101,  32,  86, 105, 101, 119, 105, 110, 103,  32,  67, 111, 110, 100, 105, 116,
            105, 111, 110,  32, 105, 110,  32,  73,  69,  67,  54,  49,  57,  54,  54,  45,
             50,  46,  49,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,  44,  82,
            101, 102, 101, 114, 101, 110,  99, 101,  32,  86, 105, 101, 119, 105, 110, 103,
             32,  67, 111, 110, 100, 105, 116, 105, 111, 110,  32, 105, 110,  32,  73,  69,
             67,  54,  49,  57,  54,  54,  45,  50,  46,  49,   0,   0,   0,   0,   0,   0,
              0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,
              0,   0,   0,   0, 118, 105, 101, 119,   0,   0,   0,   0,   0,  19, 164, 254,
              0,  20,  95,  46,   0,  16, 207,  20,   0,   3, 237, 204,   0,   4,  19,  11,
              0,   3,  92, 158,   0,   0,   0,   1,  88,  89,  90,  32,   0,   0,   0,   0,
              0,  76,   9,  86,   0,  80,   0,   0,   0,  87,  31, 231, 109, 101,  97, 115,
              0,   0,   0,   0,   0,   0,   0,   1,   0,   0,   0,   0,   0,   0,   0,   0,
              0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   2, 143,   0,   0,   0,   2,
            115, 105, 103,  32,   0,   0,   0,   0,  67,  82,  84,  32,  99, 117, 114, 118,
              0,   0,   0,   0,   0,   0,   4,   0,   0,   0,   0,   5,   0,  10,   0,  15,
              0,  20,   0,  25,   0,  30,   0,  35,   0,  40,   0,  45,   0,  50,   0,  55,
              0,  59,   0,  64,   0,  69,   0,  74,   0,  79,   0,  84,   0,  89,   0,  94,
              0,  99,   0, 104,   0, 109,   0, 114,   0, 119,   0, 124,   0, 129,   0, 134,
              0, 139,   0, 144,   0, 149,   0, 154,   0, 159,   0, 164,   0, 169,   0, 174,
              0, 178,   0, 183,   0, 188,   0, 193,   0, 198,   0, 203,   0, 208,   0, 213,
              0, 219,   0, 224,   0, 229,   0, 235,   0, 240,   0, 246,   0, 251,   1,   1,
              1,   7,   1,  13,   1,  19,   1,  25,   1,  31,   1,  37,   1,  43,   1,  50,
              1,  56,   1,  62,   1,  69,   1,  76,   1,  82,   1,  89,   1,  96,   1, 103,
              1, 110,   1, 117,   1, 124,   1, 131,   1, 139,   1, 146,   1, 154,   1, 161,
              1, 169,   1, 177,   1, 185,   1, 193,   1, 201,   1, 209,   1, 217,   1, 225,
              1, 233,   1, 242,   1, 250,   2,   3,   2,  12,   2,  20,   2,  29,   2,  38,
              2,  47,   2,  56,   2,  65,   2,  75,   2,  84,   2,  93,   2, 103,   2, 113,
              2, 122,   2, 132,   2, 142,   2, 152,   2, 162,   2, 172,   2, 182,   2, 193,
              2, 203,   2, 213,   2, 224,   2, 235,   2, 245,   3,   0,   3,  11,   3,  22,
              3,  33,   3,  45,   3,  56,   3,  67,   3,  79,   3,  90,   3, 102,   3, 114,
              3, 126,   3, 138,   3, 150,   3, 162,   3, 174,   3, 186,   3, 199,   3, 211,
              3, 224,   3, 236,   3, 249,   4,   6,   4,  19,   4,  32,   4,  45,   4,  59,
              4,  72,   4,  85,   4,  99,   4, 113,   4, 126,   4, 140,   4, 154,   4, 168,
              4, 182,   4, 196,   4, 211,   4, 225,   4, 240,   4, 254,   5,  13,   5,  28,
              5,  43,   5,  58,   5,  73,   5,  88,   5, 103,   5, 119,   5, 134,   5, 150,
              5, 166,   5, 181,   5, 197,   5, 213,   5, 229,   5, 246,   6,   6,   6,  22,
              6,  39,   6,  55,   6,  72,   6,  89,   6, 106,   6, 123,   6, 140,   6, 157,
              6, 175,   6, 192,   6, 209,   6, 227,   6, 245,   7,   7,   7,  25,   7,  43,
              7,  61,   7,  79,   7,  97,   7, 116,   7, 134,   7, 153,   7, 172,   7, 191,
              7, 210,   7, 229,   7, 248,   8,  11,   8,  31,   8,  50,   8,  70,   8,  90,
              8, 110,   8, 130,   8, 150,   8, 170,   8, 190,   8, 210,   8, 231,   8, 251,
              9,  16,   9,  37,   9,  58,   9,  79,   9, 100,   9, 121,   9, 143,   9, 164,
              9, 186,   9, 207,   9, 229,   9, 251,  10,  17,  10,  39,  10,  61,  10,  84,
             10, 106,  10, 129,  10, 152,  10, 174,  10, 197,  10, 220,  10, 243,  11,  11,
             11,  34,  11,  57,  11,  81,  11, 105,  11, 128,  11, 152,  11, 176,  11, 200,
             11, 225,  11, 249,  12,  18,  12,  42,  12,  67,  12,  92,  12, 117,  12, 142,
             12, 167,  12, 192,  12, 217,  12, 243,  13,  13,  13,  38,  13,  64,  13,  90,
             13, 116,  13, 142,  13, 169,  13, 195,  13, 222,  13, 248,  14,  19,  14,  46,
             14,  73,  14, 100,  14, 127,  14, 155,  14, 182,  14, 210,  14, 238,  15,   9,
             15,  37,  15,  65,  15,  94,  15, 122,  15, 150,  15, 179,  15, 207,  15, 236,
             16,   9,  16,  38,  16,  67,  16,  97,  16, 126,  16, 155,  16, 185,  16, 215,
             16, 245,  17,  19,  17,  49,  17,  79,  17, 109,  17, 140,  17, 170,  17, 201,
             17, 232,  18,   7,  18,  38,  18,  69,  18, 100,  18, 132,  18, 163,  18, 195,
             18, 227,  19,   3,  19,  35,  19,  67,  19,  99,  19, 131,  19, 164,  19, 197,
             19, 229,  20,   6,  20,  39,  20,  73,  20, 106,  20, 139,  20, 173,  20, 206,
             20, 240,  21,  18,  21,  52,  21,  86,  21, 120,  21, 155,  21, 189,  21, 224,
             22,   3,  22,  38,  22,  73,  22, 108,  22, 143,  22, 178,  22, 214,  22, 250,
             23,  29,  23,  65,  23, 101,  23, 137,  23, 174,  23, 210,  23, 247,  24,  27,
             24,  64,  24, 101,  24, 138,  24, 175,  24, 213,  24, 250,  25,  32,  25,  69,
             25, 107,  25, 145,  25, 183,  25, 221,  26,   4,  26,  42,  26,  81,  26, 119,
             26, 158,  26, 197,  26, 236,  27,  20,  27,  59,  27,  99,  27, 138,  27, 178,
             27, 218,  28,   2,  28,  42,  28,  82,  28, 123,  28, 163,  28, 204,  28, 245,
             29,  30,  29,  71,  29, 112,  29, 153,  29, 195,  29, 236,  30,  22,  30,  64,
             30, 106,  30, 148,  30, 190,  30, 233,  31,  19,  31,  62,  31, 105,  31, 148,
             31, 191,  31, 234,  32,  21,  32,  65,  32, 108,  32, 152,  32, 196,  32, 240,
             33,  28,  33,  72,  33, 117,  33, 161,  33, 206,  33, 251,  34,  39,  34,  85,
             34, 130,  34, 175,  34, 221,  35,  10,  35,  56,  35, 102,  35, 148,  35, 194,
             35, 240,  36,  31,  36,  77,  36, 124,  36, 171,  36, 218,  37,   9,  37,  56,
             37, 104,  37, 151,  37, 199,  37, 247,  38,  39,  38,  87,  38, 135,  38, 183,
             38, 232,  39,  24,  39,  73,  39, 122,  39, 171,  39, 220,  40,  13,  40,  63,
             40, 113,  40, 162,  40, 212,  41,   6,  41,  56,  41, 107,  41, 157,  41, 208,
             42,   2,  42,  53,  42, 104,  42, 155,  42, 207,  43,   2,  43,  54,  43, 105,
             43, 157,  43, 209,  44,   5,  44,  57,  44, 110,  44, 162,  44, 215,  45,  12,
             45,  65,  45, 118,  45, 171,  45, 225,  46,  22,  46,  76,  46, 130,  46, 183,
             46, 238,  47,  36,  47,  90,  47, 145,  47, 199,  47, 254,  48,  53,  48, 108,
             48, 164,  48, 219,  49,  18,  49,  74,  49, 130,  49, 186,  49, 242,  50,  42,
             50,  99,  50, 155,  50, 212,  51,  13,  51,  70,  51, 127,  51, 184,  51, 241,
             52,  43,  52, 101,  52, 158,  52, 216,  53,  19,  53,  77,  53, 135,  53, 194,
             53, 253,  54,  55,  54, 114,  54, 174,  54, 233,  55,  36,  55,  96,  55, 156,
             55, 215,  56,  20,  56,  80,  56, 140,  56, 200,  57,   5,  57,  66,  57, 127,
             57, 188,  57, 249,  58,  54,  58, 116,  58, 178,  58, 239,  59,  45,  59, 107,
             59, 170,  59, 232,  60,  39,  60, 101,  60, 164,  60, 227,  61,  34,  61,  97,
             61, 161,  61, 224,  62,  32,  62,  96,  62, 160,  62, 224,  63,  33,  63,  97,
             63, 162,  63, 226,  64,  35,  64, 100,  64, 166,  64, 231,  65,  41,  65, 106,
             65, 172,  65, 238,  66,  48,  66, 114,  66, 181,  66, 247,  67,  58,  67, 125,
             67, 192,  68,   3,  68,  71,  68, 138,  68, 206,  69,  18,  69,  85,  69, 154,
             69, 222,  70,  34,  70, 103,  70, 171,  70, 240,  71,  53,  71, 123,  71, 192,
             72,   5,  72,  75,  72, 145,  72, 215,  73,  29,  73,  99,  73, 169,  73, 240,
             74,  55,  74, 125,  74, 196,  75,  12,  75,  83,  75, 154,  75, 226,  76,  42,
             76, 114,  76, 186,  77,   2,  77,  74,  77, 147,  77, 220,  78,  37,  78, 110,
             78, 183,  79,   0,  79,  73,  79, 147,  79, 221,  80,  39,  80, 113,  80, 187,
             81,   6,  81,  80,  81, 155,  81, 230,  82,  49,  82, 124,  82, 199,  83,  19,
             83,  95,  83, 170,  83, 246,  84,  66,  84, 143,  84, 219,  85,  40,  85, 117,
             85, 194,  86,  15,  86,  92,  86, 169,  86, 247,  87,  68,  87, 146,  87, 224,
             88,  47,  88, 125,  88, 203,  89,  26,  89, 105,  89, 184,  90,   7,  90,  86,
             90, 166,  90, 245,  91,  69,  91, 149,  91, 229,  92,  53,  92, 134,  92, 214,
             93,  39,  93, 120,  93, 201,  94,  26,  94, 108,  94, 189,  95,  15,  95,  97,
             95, 179,  96,   5,  96,  87,  96, 170,  96, 252,  97,  79,  97, 162,  97, 245,
             98,  73,  98, 156,  98, 240,  99,  67,  99, 151,  99, 235, 100,  64, 100, 148,
            100, 233, 101,  61, 101, 146, 101, 231, 102,  61, 102, 146, 102, 232, 103,  61,
            103, 147, 103, 233, 104,  63, 104, 150, 104, 236, 105,  67, 105, 154, 105, 241,
            106,  72, 106, 159, 106, 247, 107,  79, 107, 167, 107, 255, 108,  87, 108, 175,
            109,   8, 109,  96, 109, 185, 110,  18, 110, 107, 110, 196, 111,  30, 111, 120,
            111, 209, 112,  43, 112, 134, 112, 224, 113,  58, 113, 149, 113, 240, 114,  75,
            114, 166, 115,   1, 115,  93, 115, 184, 116,  20, 116, 112, 116, 204, 117,  40,
            117, 133, 117, 225, 118,  62, 118, 155, 118, 248, 119,  86, 119, 179, 120,  17,
            120, 110, 120, 204, 121,  42, 121, 137, 121, 231, 122,  70, 122, 165, 123,   4,
            123,  99, 123, 194, 124,  33, 124, 129, 124, 225, 125,  65, 125, 161, 126,   1,
            126,  98, 126, 194, 127,  35, 127, 132, 127, 229, 128,  71, 128, 168, 129,  10,
            129, 107, 129, 205, 130,  48, 130, 146, 130, 244, 131,  87, 131, 186, 132,  29,
            132, 128, 132, 227, 133,  71, 133, 171, 134,  14, 134, 114, 134, 215, 135,  59,
            135, 159, 136,   4, 136, 105, 136, 206, 137,  51, 137, 153, 137, 254, 138, 100,
            138, 202, 139,  48, 139, 150, 139, 252, 140,  99, 140, 202, 141,  49, 141, 152,
            141, 255, 142, 102, 142, 206, 143,  54, 143, 158, 144,   6, 144, 110, 144, 214,
            145,  63, 145, 168, 146,  17, 146, 122, 146, 227, 147,  77, 147, 182, 148,  32,
            148, 138, 148, 244, 149,  95, 149, 201, 150,  52, 150, 159, 151,  10, 151, 117,
            151, 224, 152,  76, 152, 184, 153,  36, 153, 144, 153, 252, 154, 104, 154, 213,
            155,  66, 155, 175, 156,  28, 156, 137, 156, 247, 157, 100, 157, 210, 158,  64,
            158, 174, 159,  29, 159, 139, 159, 250, 160, 105, 160, 216, 161,  71, 161, 182,
            162,  38, 162, 150, 163,   6, 163, 118, 163, 230, 164,  86, 164, 199, 165,  56,
            165, 169, 166,  26, 166, 139, 166, 253, 167, 110, 167, 224, 168,  82, 168, 196,
            169,  55, 169, 169, 170,  28, 170, 143, 171,   2, 171, 117, 171, 233, 172,  92,
            172, 208, 173,  68, 173, 184, 174,  45, 174, 161, 175,  22, 175, 139, 176,   0,
            176, 117, 176, 234, 177,  96, 177, 214, 178,  75, 178, 194, 179,  56, 179, 174,
            180,  37, 180, 156, 181,  19, 181, 138, 182,   1, 182, 121, 182, 240, 183, 104,
            183, 224, 184,  89, 184, 209, 185,  74, 185, 194, 186,  59, 186, 181, 187,  46,
            187, 167, 188,  33, 188, 155, 189,  21, 189, 143, 190,  10, 190, 132, 190, 255,
            191, 122, 191, 245, 192, 112, 192, 236, 193, 103, 193, 227, 194,  95, 194, 219,
            195,  88, 195, 212, 196,  81, 196, 206, 197,  75, 197, 200, 198,  70, 198, 195,
            199,  65, 199, 191, 200,  61, 200, 188, 201,  58, 201, 185, 202,  56, 202, 183,
            203,  54, 203, 182, 204,  53, 204, 181, 205,  53, 205, 181, 206,  54, 206, 182,
            207,  55, 207, 184, 208,  57, 208, 186, 209,  60, 209, 190, 210,  63, 210, 193,
            211,  68, 211, 198, 212,  73, 212, 203, 213,  78, 213, 209, 214,  85, 214, 216,
            215,  92, 215, 224, 216, 100, 216, 232, 217, 108, 217, 241, 218, 118, 218, 251,
            219, 128, 220,   5, 220, 138, 221,  16, 221, 150, 222,  28, 222, 162, 223,  41,
            223, 175, 224,  54, 224, 189, 225,  68, 225, 204, 226,  83, 226, 219, 227,  99,
            227, 235, 228, 115, 228, 252, 229, 132, 230,  13, 230, 150, 231,  31, 231, 169,
            232,  50, 232, 188, 233,  70, 233, 208, 234,  91, 234, 229, 235, 112, 235, 251,
            236, 134, 237,  17, 237, 156, 238,  40, 238, 180, 239,  64, 239, 204, 240,  88,
            240, 229, 241, 114, 241, 255, 242, 140, 243,  25, 243, 167, 244,  52, 244, 194,
            245,  80, 245, 222, 246, 109, 246, 251, 247, 138, 248,  25, 248, 168, 249,  56,
            249, 199, 250,  87, 250, 231, 251, 119, 252,   7, 252, 152, 253,  41, 253, 186,
            254,  75, 254, 220, 255, 109, 255, 255};
        #endregion

        private static double fontCorrectValue
        {
            get
            {
                if (CompatibleMode160) return 1;
                else return 0.957;
            }
        }

        private static bool compatibleMode160 = false;
        public static bool CompatibleMode160
        {
            get
            {
                return compatibleMode160;
            }
            set
            {
                compatibleMode160 = value;
            }
        }

        private static bool printScaling = true;
        /// <summary>
        /// PrintScaling property shows, how Acrobat Reader must to use margins of the printer.
        /// if true, then default settings of the Acrobat Reader will be used (usually "Fit to printer margin")
        /// else PrintScaling parameter of the pdf file will be set to None.
        /// </summary>
		public static bool PrintScaling
        {
            get
            {
                return printScaling;
            }
            set
            {
                printScaling = value;
            }
        }
        #endregion

        #region GetHatchNumber
        /// <summary>
        /// Returns number of hatch in table of hatches.
        /// </summary>
        public int GetHatchNumber(StiHatchBrush brush)
        {
            if (hatchArray.Count > 0)
            {
                for (int index = 0; index < hatchArray.Count; index++)
                {
                    StiHatchBrush tempBrush = (StiHatchBrush)hatchArray[index];
                    if ((brush.Style == tempBrush.Style) &&
                        (brush.BackColor == tempBrush.BackColor) &&
                        (brush.ForeColor == tempBrush.ForeColor))
                    {
                        return index;
                    }
                }
            }
            //add hatch to table
            hatchArray.Add(brush);
            return hatchArray.Count - 1;
        }

        public int GetHatchNumber(HatchBrush brush)
        {
            return GetHatchNumber(new StiHatchBrush(brush.HatchStyle, brush.ForegroundColor, brush.BackgroundColor));
        }
        #endregion

        #region GetShadingFunctionNumber
        /// <summary>
        /// Returns number of shadingFunctionArray in table of shadingFunctions.
        /// </summary>
        public int GetShadingFunctionNumber(Color color1, Color color2, bool isGlare)
        {
            if (shadingFunctionArray.Count > 0)
            {
                for (int index = 0; index < shadingFunctionArray.Count; index++)
                {
                    var tempSf = shadingFunctionArray[index];
                    if (tempSf.Color1.Equals(color1) &&
                        tempSf.Color2.Equals(color2) &&
                        (tempSf.IsGlare == isGlare))
                    {
                        return index;
                    }
                }
            }
            //add shadingFunction to table
            var sf = new StiShadingFunctionData();
            sf.Color1 = color1;
            sf.Color2 = color2;
            sf.IsGlare = isGlare;
            shadingFunctionArray.Add(sf);
            return shadingFunctionArray.Count - 1;
        }
        #endregion

        #region Render procedures

        #region IsWordWrapSymbol
        private bool IsWordWrapSymbol(StringBuilder sb, int index)
        {
            char sym1 = sb[index];
            if (((sym1 >= 0x3000) && (sym1 <= 0xd7af)) ||	//East Asian & Unified CJK
                (char.IsWhiteSpace(sym1) && (sym1 != 0xa0)) ||
                sym1 == '(' || sym1 == '{') return true;
            if (index > 0)
            {
                char sym2 = sb[index - 1];
                if (((sym2 >= 0x3000) && (sym2 <= 0xd7af)) || sym2 == '!' || sym2 == '%' || sym2 == ')' || sym2 == '}' || sym2 == '-' || sym2 == '?') return true;
            }
            return false;
        }

        //private bool IsWordWrapSymbol(StringBuilder sb, int index)
        //{
        //    char sym = ' ';
        //    if (index < sb.Length - 1) sym = sb[index + 1];
        //    return IsWordWrapSymbol(sb[index], sym);
        //}

        //private bool IsWordWrapSymbol(char sym1, char sym2)
        //{
        //    if ((sym1 >= 0x3000) && (sym1 <= 0xd7af)) return true;	//East Asian & Unified CJK
        //    UnicodeCategory cat1 = char.GetUnicodeCategory(sym1);
        //    UnicodeCategory cat2 = char.GetUnicodeCategory(sym2);
        //    return
        //        (cat1 == UnicodeCategory.OpenPunctuation) ||
        //        ((cat1 == UnicodeCategory.ClosePunctuation) && (cat2 != UnicodeCategory.OtherPunctuation)) ||
        //        (cat1 == UnicodeCategory.DashPunctuation) ||
        //        (cat1 == UnicodeCategory.InitialQuotePunctuation) ||
        //        (cat1 == UnicodeCategory.FinalQuotePunctuation) ||
        //        (cat1 == UnicodeCategory.SpaceSeparator) ||
        //        (char.IsWhiteSpace(sym1) ||
        //        sym1 == '!' || sym1 == '%' || sym1 == ',' || sym1 == '.' || sym1 == '/' || sym1 == ';');
        //}
        #endregion

        #region Get tabs size
        private float GetTabsSize(IStiTextOptions textOp, double sizeInPt, double currentPosition)
        {
            if ((textOp != null) && (textOp.TextOptions != null))
            {
                double position = currentPosition;

                float spaceWidth = 754f / (float)sizeInPt;      //empiric; for CourierNew must be 726 ?
                float otherTab = spaceWidth * textOp.TextOptions.DistanceBetweenTabs;
                float firstTab = spaceWidth * textOp.TextOptions.FirstTabOffset + otherTab;
                if (isWpf)
                {
                    spaceWidth = 751f / (float)sizeInPt;
                    firstTab = otherTab = spaceWidth * 64;
                }

                if (currentPosition < firstTab)
                {
                    position = firstTab;
                }
                else
                {
                    if (textOp.TextOptions.DistanceBetweenTabs > 0)
                    {
                        int kolTabs = (int)((currentPosition - firstTab) / otherTab);
                        kolTabs++;
                        position = firstTab + kolTabs * otherTab;
                    }
                }

                return (float)(position - currentPosition);
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region CheckGraphicsForTextRenderer
        private void CheckGraphicsForTextRenderer()
        {
            if (graphicsForTextRenderer == null)
            {
                var isGdi = true;
#if STIDRAWING
                isGdi = Graphics.GraphicsEngine == Stimulsoft.Drawing.GraphicsEngine.Gdi;
#endif
                if (StiOptions.Engine.FullTrust && StiOptions.Export.Pdf.AllowInvokeWindowsLibraries && isGdi)
                {
                    graphicsForTextRenderer = Graphics.FromHwnd(IntPtr.Zero);
                }
                else
                {
                    imageForGraphicsForTextRenderer = new Bitmap(1, 1);
                    graphicsForTextRenderer = Graphics.FromImage(imageForGraphicsForTextRenderer);
                }
            }
        }
        #endregion

        #region Add Xref
        private void AddXref(int num)
        {
            sw.Flush();
            long offs = sw.BaseStream.Position;
            xref.Add(num, offs.ToString("D10") + " 00000 n");
            currentObjectNumber = num;
            currentGenerationNumber = 0;
        }
        #endregion

        #region Convert numeric value to string
        /// <summary>
        /// Convert numeric value to string 
        /// </summary>
        /// <param name="Value">Numeric value</param>
        /// <returns>String</returns>
        internal string ConvertToString(double Value)
        {
            int digits = precision_digits;
            decimal numValue = Math.Round((decimal)Value, digits);
            return numValue.ToString("G", currentNumberFormat);
        }
        internal string ConvertToString(double Value, int precision)
        {
            decimal numValue = Math.Round((decimal)Value, precision);
            return numValue.ToString("G", currentNumberFormat);
        }
        #endregion

        #region Convert string to Escape sequence
        internal static string ConvertToEscapeSequence(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;

            string escapeChars = "()\\" + (char)13 + (char)10 + (char)8 + (char)12;     //tabs processing in another place
            string replaceChars = "()\\rnbf";
            StringBuilder sb = new StringBuilder();

            for (int index = 0; index < value.Length; index++)
            {
                int ind = escapeChars.IndexOf(value[index]);
                if (ind >= 0)
                {
                    sb.Append("\\" + replaceChars[ind]);
                }
                else
                {
                    sb.Append(value[index]);
                }
            }

            return sb.ToString();
        }

        internal static string ConvertToEscapeSequencePlusTabs(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;

            string escapeChars = "()\\" + (char)13 + (char)10 + (char)9 + (char)8 + (char)12;
            string replaceChars = "()\\rntbf";

            var sb = new StringBuilder();
            for (int index = 0; index < value.Length; index++)
            {
                int ind = escapeChars.IndexOf(value[index]);
                if (ind >= 0)
                {
                    sb.Append("\\" + replaceChars[ind]);
                }
                else
                {
                    sb.Append(value[index]);
                }
            }

            return sb.ToString();
        }
        #endregion

        #region CorrectNameEnconding
        internal static string CorrectNameEnconding(string input)
        {
            var buf = Encoding.UTF8.GetBytes(input);

            StringBuilder sb = new StringBuilder();

            foreach (byte b in buf)
            {
                if ((b < 0x21) || (b > 0x7e) || (b == 0x23) || (b == 0x2f))
                {
                    sb.Append($"#{b:X2}");
                }
                else
                {
                    sb.Append((char)b);
                }
            }

            return sb.ToString();
        }
        #endregion

        #region Store Wysiwyg symbols
        internal void StoreWysiwygSymbols(StiText text, int pageNumber = -1)
        {
            #region Native parsing of the Wysiwyg text
            CheckGraphicsForTextRenderer();
            var outRunsList = new List<Stimulsoft.Base.Drawing.StiTextRenderer.RunInfo>();
            var outFontsList = new List<Stimulsoft.Base.Drawing.StiTextRenderer.StiFontState>();
            string textForOutput = text.Text;

            RectangleD rectComp = text.Page.Unit.ConvertToHInches(text.ComponentToPage(text.ClientRectangle));
            RectangleD rectText = text.ConvertTextMargins(rectComp, false);
            rectText = text.ConvertTextBorders(rectText, false);
            IStiTextOptions textOpt = text as IStiTextOptions;

            bool useNewHtmlEngine = StiOptions.Engine.UseNewHtmlEngine && text.AllowHtmlTags;
            if (useNewHtmlEngine)
            {
                StiHtmlTextRender.DrawTextForOutput(text, out outRunsList, out outFontsList);

                for (int index = 0; index < outRunsList.Count; index++)
                {
                    var rune = outRunsList[index];
                    string oldText = rune.Text;
                    rune.Text = StiBidirectionalConvert2.ConvertString(oldText, textOpt.TextOptions.RightToLeft);
                    if (rune.Text.Length < oldText.Length)
                    {
                        rune.Text += new string('\u200B', oldText.Length - rune.Text.Length);
                    }
                    outRunsList[index] = rune;
                }
            }
            else
            {
                StiTextRenderer.DrawTextForOutput(
                graphicsForTextRenderer,
                textForOutput,
                text.Font,
                rectText,
                StiBrush.ToColor(text.TextBrush),
                StiBrush.ToColor(text.Brush),
                1,
                text.HorAlignment,
                text.VertAlignment,
                textOpt.TextOptions.WordWrap,
                textOpt.TextOptions.RightToLeft,
                1,
                textOpt.TextOptions.Angle,
                textOpt.TextOptions.Trimming,
                textOpt.TextOptions.LineLimit,
                text.CheckAllowHtmlTags(),
                outRunsList,
                outFontsList,
                textOpt.TextOptions,
                1);
            }

            pdfFont.StoreUnicodeSymbolsInMap(new StringBuilder(textForOutput));

            foreach (StiTextRenderer.RunInfo runInfo in outRunsList)
            {
                Font tempFont = ((StiTextRenderer.StiFontState)outFontsList[runInfo.FontIndex]).FontBase;
                int fnt = pdfFont.GetFontNumber(tempFont);
                pdfFont.CurrentFont = fnt;

                StringBuilder sb = new StringBuilder();
                if (!useNewHtmlEngine)
                {
                    for (int indexGlyph = 0; indexGlyph < runInfo.GlyphIndexList.Length; indexGlyph++)
                    {
                        sb.Append((char)runInfo.GlyphIndexList[indexGlyph]);
                    }
                }
                pdfFont.StoreGlyphsInMap(sb);
                pdfFont.StoreUnicodeSymbolsInMap(new StringBuilder(runInfo.Text));

                if ((pageNumber != -1) && !string.IsNullOrWhiteSpace(runInfo.Href) && !runInfo.Href.Trim().StartsWith("javascript:") && !usePdfA)
                {
                    StiLinkObject stl = new StiLinkObject();
                    stl.Link = runInfo.Href;
                    stl.Page = pageNumber;
                    linksArray.Add(stl);
                }
            }
            #endregion
        }
        #endregion

        #region Store colors
        internal void SetStrokeColor(Color tempColor)
        {
            string st = (string)colorHash1[tempColor];
            if (st == null)
            {
                st = string.Format("{0} {1} {2} RG",
                    colorTable[tempColor.R],
                    colorTable[tempColor.G],
                    colorTable[tempColor.B]);
                colorHash1[tempColor] = st;
            }
            pageStream.WriteLine(st);
            byte alpha = tempColor.A;
            if ((alpha != lastColorStrokeA) && (StiOptions.Export.Pdf.AllowExtGState && useTransparency))
            {
                pageStream.WriteLine(GsTable[alpha, 0]);
                lastColorStrokeA = alpha;
                alphaTable[alpha] = true;
            }
        }
        internal void SetNonStrokeColor(Color tempColor)
        {
            string st = (string)colorHash2[tempColor];
            if (st == null)
            {
                st = string.Format("{0} {1} {2} rg",
                    colorTable[tempColor.R],
                    colorTable[tempColor.G],
                    colorTable[tempColor.B]);
                colorHash2[tempColor] = st;
            }
            pageStream.WriteLine(st);
            byte alpha = tempColor.A;
            if ((alpha != lastColorNonStrokeA) && (StiOptions.Export.Pdf.AllowExtGState && useTransparency))
            {
                pageStream.WriteLine(GsTable[alpha, 1]);
                lastColorNonStrokeA = alpha;
                alphaTable[alpha] = true;
            }
        }

        private Hashtable colorHash1 = new Hashtable();
        private Hashtable colorHash2 = new Hashtable();

        private static string[,] gsTable = null;
        private static string[,] GsTable
        {
            get
            {
                if (gsTable == null)
                {
                    gsTable = new string[256, 2];
                    for (int index = 0; index < 256; index++)
                    {
                        gsTable[index, 0] = string.Format("/GS{0}S gs", string.Format("{0:X2}", index));
                        gsTable[index, 1] = string.Format("/GS{0}N gs", string.Format("{0:X2}", index));
                    }
                }
                return gsTable;
            }
        }

        internal void PushColorToStack()
        {
            colorStack.Push(lastColorStrokeA);
            colorStack.Push(lastColorNonStrokeA);
        }
        internal void PopColorFromStack()
        {
            lastColorNonStrokeA = (byte)colorStack.Pop();
            lastColorStrokeA = (byte)colorStack.Pop();
        }
        #endregion

        #region Fill rect with brush
        private void FillRectBrush(StiBrush brush, RectangleD rect, StiCornerRadius cornerRadius = null)
        {
            Color tempColor = StiBrush.ToColor(brush);
            SetNonStrokeColor(tempColor);
            StoreShadingData2(rect.X, rect.Y, rect.Width, rect.Height, brush);
            if (brush is StiGradientBrush || brush is StiGlareBrush)
            {
                pageStream.WriteLine("/Pattern cs /P{0} scn", 1 + shadingCurrent);
            }
            if (brush is StiHatchBrush)
            {
                StiHatchBrush hBrush = brush as StiHatchBrush;
                pageStream.WriteLine("/Cs1 cs /PH{0} scn", GetHatchNumber(hBrush) + 1);
            }
            if (brush is StiGlassBrush)
            {
                StiGlassBrush glass = brush as StiGlassBrush;
                if (glass.DrawHatch)
                {
                    pageStream.WriteLine("/Cs1 cs /PH{0} scn", GetHatchNumber(glass.GetTopBrush() as HatchBrush) + 1);
                }
                else
                {
                    SetNonStrokeColor(glass.GetTopColor());
                }
                if (cornerRadius == null)
                    pageStream.WriteLine(tempGeomWriter.GetRectString(rect.X, rect.Y + rect.Height / 2, rect.Width, rect.Height / 2) + "f");
                else
                    pageStream.WriteLine(tempGeomWriter.GetRectWithCornersString(new RectangleD(rect.X, rect.Y + rect.Height / 2, rect.Width, rect.Height / 2).ToRectangleF(), new StiCornerRadius(cornerRadius.BottomLeft, cornerRadius.BottomRight, 0, 0)) + "f");
                if (glass.DrawHatch)
                {
                    pageStream.WriteLine("/Cs1 cs /PH{0} scn", GetHatchNumber(glass.GetBottomBrush() as HatchBrush) + 1);
                }
                else
                {
                    SetNonStrokeColor(glass.GetBottomColor());
                }
                if (cornerRadius == null)
                    pageStream.WriteLine(tempGeomWriter.GetRectString(rect.X, rect.Y, rect.Width, rect.Height / 2) + "f");
                else
                    pageStream.WriteLine(tempGeomWriter.GetRectWithCornersString(new RectangleD(rect.X, rect.Y, rect.Width, rect.Height / 2).ToRectangleF(), new StiCornerRadius(0, 0, cornerRadius.TopRight, cornerRadius.TopLeft)) + "f");
                tempColor = Color.Transparent;
            }
            if (tempColor.A != 0)
            {
                StiCornerRadius newCornerRadius = cornerRadius == null ? null : new StiCornerRadius(cornerRadius.BottomLeft, cornerRadius.BottomRight, cornerRadius.TopRight, cornerRadius.TopLeft);
                pageStream.WriteLine(tempGeomWriter.GetRectWithCornersString(rect.ToRectangleF(), newCornerRadius) + "f");
            }
        }
        #endregion

        #region Store encoded data
        private void StoreStringLine(string noCryptString, string cryptString)
        {
            StoreStringLine(noCryptString, cryptString, false);
        }

        private void StoreStringLine(string noCryptString, string cryptString, bool escaping)
        {
            string tempSt = noCryptString + ConvertToHexString(cryptString, escaping);
            StoreString(tempSt);
            sw.WriteLine();
        }

        internal void StoreString(string st)
        {
            byte[] data = new byte[st.Length];
            for (int index = 0; index < st.Length; index++)
            {
                data[index] = (byte)st[index];
            }
            sw.Flush();
            stream2.Write(data, 0, data.Length);
        }

        internal string ConvertToHexString(string inString, bool escaping, bool forceHex = false)
        {
            StringBuilder outString = new StringBuilder();
            if (!string.IsNullOrEmpty(inString))
            {
                bool needHex = forceHex;
                for (int tempIndex = 0; tempIndex < inString.Length; tempIndex++)
                {
                    if ((int)inString[tempIndex] > 127)
                    {
                        needHex = true;
                    }
                }
                if (!encrypted)
                {
                    if (needHex == true)
                    {
#if TESTPDF
                        outString.Append("<FEFF");
                        for (int index = 0; index < inString.Length; index++)
                        {
                            int tempInt = (int)inString[index];
                            outString.Append(tempInt.ToString("X4"));
                        }
                        outString.Append(">");
#else
                        if (!escaping) outString.Append("(\xFE\xFF");
                        for (int index = 0; index < inString.Length; index++)
                        {
                            int charValue = (int)inString[index];
                            outString.Append((char)((charValue >> 8) & 0xff));
                            outString.Append((char)((charValue) & 0xff));
                        }
                        if (escaping)
                        {
                            outString = new StringBuilder("(\xFE\xFF" + ConvertToEscapeSequencePlusTabs(outString.ToString()));
                        }
                        outString.Append(")");
#endif
                    }
                    else
                    {
                        if (escaping)
                        {
                            outString.Append("(" + ConvertToEscapeSequencePlusTabs(inString) + ")");
                        }
                        else
                        {
                            outString.Append("(" + inString + ")");
                        }
                    }
                }
                else
                {
                    byte[] forCrypt = null;
                    if (needHex == true)
                    {
                        forCrypt = new byte[(inString.Length + 1) * 2];
                        for (int index = 0; index < inString.Length; index++)
                        {
                            //BitConverter.GetBytes(inString[index]).CopyTo(forCrypt, index * 2 + 2);
                            int charValue = (int)inString[index];
                            forCrypt[2 + index * 2 + 0] = (byte)((charValue >> 8) & 0xff);
                            forCrypt[2 + index * 2 + 1] = (byte)((charValue) & 0xff);
                        }
                        forCrypt[0] = 0xFE;
                        forCrypt[1] = 0xFF;
                    }
                    else
                    {
                        forCrypt = new byte[inString.Length];
                        for (int index = 0; index < inString.Length; index++)
                        {
                            forCrypt[index] = (byte)inString[index];
                        }
                    }
                    byte[] encryptedData = pdfSecurity.EncryptData(forCrypt, currentObjectNumber, currentGenerationNumber);
                    StringBuilder tempSB = new StringBuilder();
                    for (int index = 0; index < encryptedData.Length; index++)
                    {
                        tempSB.Append((char)encryptedData[index]);
                    }
                    outString.Append("(" + ConvertToEscapeSequencePlusTabs(tempSB.ToString()) + ")");
                }
            }
            else
            {
                if (encrypted)
                {
                    byte[] encryptedData = pdfSecurity.EncryptData(new byte[0], currentObjectNumber, currentGenerationNumber);
                    StringBuilder tempSB = new StringBuilder();
                    for (int index = 0; index < encryptedData.Length; index++)
                    {
                        tempSB.Append((char)encryptedData[index]);
                    }
                    outString.Append("(" + ConvertToEscapeSequencePlusTabs(tempSB.ToString()) + ")");
                }
                else
                {
                    outString.Append("()");
                }
            }
            return outString.ToString();
        }

        //private int StoreMemoryStream(MemoryStream mem)
        //{
        //    int result = -1;
        //    if (encrypted)
        //    {
        //        byte[] forCrypt = mem.ToArray();
        //        byte[] encryptedData = EncryptData(forCrypt);
        //        sw.Flush();
        //        stream2.Write(encryptedData, 0, encryptedData.Length);
        //        result = encryptedData.Length;
        //    }
        //    else
        //    {
        //        sw.Flush();
        //        mem.WriteTo(stream2);
        //        result = (int)mem.Length;
        //    }
        //    mem.Close();
        //    return result;
        //}

        private void StoreMemoryStream2(MemoryStream mem, string dictionaryString)
        {
            if (encrypted)
            {
                byte[] forCrypt = mem.ToArray();
                byte[] encryptedData = pdfSecurity.EncryptData(forCrypt, currentObjectNumber, currentGenerationNumber);
                sw.WriteLine(string.Format(dictionaryString, encryptedData.Length));
                sw.WriteLine(">>");
                sw.WriteLine("stream");
                sw.Flush();
                stream2.Write(encryptedData, 0, encryptedData.Length);
            }
            else
            {
                sw.WriteLine(string.Format(dictionaryString, mem.Length));
                sw.WriteLine(">>");
                sw.WriteLine("stream");
                sw.Flush();
                mem.WriteTo(stream2);
            }
            mem.Close();
        }
        private void StoreMemoryStream2(byte[] data, string dictionaryString)
        {
            if (encrypted)
            {
                byte[] encryptedData = pdfSecurity.EncryptData(data, currentObjectNumber, currentGenerationNumber);
                sw.WriteLine(string.Format(dictionaryString, encryptedData.Length));
                sw.WriteLine(">>");
                sw.WriteLine("stream");
                sw.Flush();
                stream2.Write(encryptedData, 0, encryptedData.Length);
            }
            else
            {
                sw.WriteLine(string.Format(dictionaryString, data.Length));
                sw.WriteLine(">>");
                sw.WriteLine("stream");
                sw.Flush();
                stream2.Write(data, 0, data.Length);
            }
        }
        #endregion

        #region RenderStartDoc
        private void RenderStartDoc(StiReport report, StiPagesCollection pages)
        {
            //header
            //sw.WriteLine(keyLength == StiPdfEncryptionKeyLength.Bit256_r5 || keyLength == StiPdfEncryptionKeyLength.Bit256_r6 ? "%PDF-1.7" : "%PDF-1.4");
            sw.WriteLine("%PDF-1.7");
            byte[] buff = new byte[5] { 37, 226, 227, 207, 211 };
            sw.Flush();
            stream2.Write(buff, 0, buff.Length);
            sw.WriteLine("");

            #region Catalog
            AddXref(1);
            sw.WriteLine("1 0 obj");
            sw.WriteLine("<<");

            if (keyLength == StiPdfEncryptionKeyLength.Bit256_r5)
                sw.WriteLine("/Extensions<</ADBE<</BaseVersion/1.7/ExtensionLevel 3>>>>");
            if (keyLength == StiPdfEncryptionKeyLength.Bit256_r6)
                sw.WriteLine("/Extensions<</ADBE<</BaseVersion/1.7/ExtensionLevel 8>>>>");

            sw.WriteLine("/Type /Catalog");
            sw.WriteLine("/Pages 4 0 R");

            sw.WriteLine("/MarkInfo<</Marked true>>");
            sw.WriteLine("/Metadata {0} 0 R", info.Metadata.Ref);
            sw.WriteLine("/OutputIntents {0} 0 R", info.OutputIntents.Ref);

            sw.WriteLine("/StructTreeRoot 5 0 R");

            if (haveBookmarks)
            {
                sw.WriteLine("/Outlines {0} 0 R", info.Outlines.Ref);
                sw.WriteLine("/PageMode /UseOutlines");
            }
            else
            {
                sw.WriteLine("/PageMode /UseNone");
            }
            if (!PrintScaling)
            {
                sw.WriteLine("/ViewerPreferences");
                sw.WriteLine("<<");
                sw.WriteLine("/PrintScaling /None");
                sw.WriteLine(">>");
            }
            if (haveAnnots || haveDigitalSignature)
            {
                sw.WriteLine("/AcroForm {0} 0 R", info.AcroForm.Ref);
            }

            bool needAutoPrint = autoPrint != StiPdfAutoPrintMode.None;
            bool needEmbeddedFiles = info.EmbeddedFilesList.Count > 0;
            if (needAutoPrint || needEmbeddedFiles)
            {
                sw.WriteLine("/Names <<");
                if (needAutoPrint)
                {
                    sw.WriteLine("/JavaScript {0} 0 R", info.EmbeddedJS.Ref);
                }
                if (needEmbeddedFiles)
                {
                    sw.WriteLine("/EmbeddedFiles <<");
                    sw.Write("/Names [ ");
                    for (int index = 0; index < info.EmbeddedFilesList.Count; index++)
                    {
                        var file = embeddedFiles[index];
                        sw.Write("{0} {1} 0 R ", ConvertToHexString(file.Name, true, true), info.EmbeddedFilesList[index].Ref);
                    }
                    sw.WriteLine("]");
                    sw.WriteLine(">>");
                }
                sw.WriteLine(">>");
            }

            if (zugferdComplianceMode != StiPdfZUGFeRDComplianceMode.None || pdfComplianceMode == StiPdfComplianceMode.A3)
            {
                sw.Write("/AF [ ");
                for (int index = 0; index < info.EmbeddedFilesList.Count; index++)
                {
                    sw.Write("{0} 0 R ", info.EmbeddedFilesList[index].Ref);
                }
                sw.WriteLine("]");
            }

            if (!usePdfA)
            {
                sw.WriteLine("/OCProperties << /OCGs [{0} 0 R] /D << /ON [{0} 0 R] /AS [<</Event /Print /OCGs [{0} 0 R] /Category [/Print]>>] >> >>", info.OptionalContentGroup.Ref);
            }

            var culture = report.GetParsedCulture();
            if (!string.IsNullOrWhiteSpace(culture))
            {
                StoreStringLine("/Lang ", culture, true);
            }

            sw.WriteLine(">>");
            sw.WriteLine("endobj");
            sw.WriteLine("");
            #endregion

            #region Info
            AddXref(2);
            sw.WriteLine("2 0 obj");
            sw.WriteLine("<<");

            StoreStringLine("/Producer ", producerName, true);
            StoreStringLine("/Creator ", creatorName, true);
            if (!string.IsNullOrEmpty(report.ReportAuthor))
            {
                StoreStringLine("/Author ", report.ReportAuthor, true);
            }
            if (!string.IsNullOrEmpty(report.ReportAlias))
            {
                StoreStringLine("/Subject ", report.ReportAlias, true);
            }
            if (!string.IsNullOrEmpty(report.ReportName))
            {
                StoreStringLine("/Title ", report.ReportName, true);
            }
            if (!string.IsNullOrEmpty(keywords))
            {
                StoreStringLine("/Keywords ", keywords, true);
            }
            StoreStringLine("/CreationDate ", "D:" + currentDateTime);
            StoreStringLine("/ModDate ", "D:" + currentDateTime);

            if (pdfComplianceMode == StiPdfComplianceMode.None)
            {
                #region Store metatags
                foreach (StiMetaTag meta in report.MetaTags)
                {
                    if (meta.Name.StartsWith("pdf:"))
                    {
                        string metaName = meta.Name.Substring(4);
                        if (!reservedKeywords.Contains(metaName))
                        {
                            var bytes = Encoding.UTF8.GetBytes(metaName);

                            StringBuilder sbName = new StringBuilder();
                            for (int index = 0; index < bytes.Length; index++)
                            {
                                char ch = (char)bytes[index];
                                if (char.IsLetterOrDigit(ch) && (ch < 128) || ch == '_')
                                {
                                    sbName.Append(ch);
                                }
                                else
                                {
                                    sbName.Append("#" + ((int)ch).ToString("X2"));
                                }
                            }

                            StoreStringLine(string.Format("/{0} ", sbName.ToString()), meta.Tag, true);
                        }
                    }
                }
                #endregion
            }

            sw.WriteLine(">>");
            sw.WriteLine("endobj");
            sw.WriteLine("");
            #endregion

            #region Colorspace
            AddXref(3);
            sw.WriteLine("3 0 obj");
            sw.WriteLine("<<");
            sw.WriteLine("/Cs1 [/Pattern /DeviceRGB]");
            sw.WriteLine(">>");
            sw.WriteLine("endobj");
            sw.WriteLine("");
            #endregion

            #region Pages
            AddXref(4);
            sw.WriteLine("4 0 obj");
            sw.WriteLine("<<");
            sw.WriteLine("/Type /Pages");
            sw.Write("/Kids [ ");
            for (int index = 0; index < pages.Count; index++)
            {
                sw.Write("{0} 0 R ", info.PageList[index].Ref);
            }
            sw.WriteLine("]");
            sw.WriteLine("/Count {0}", pages.Count);
            if (StiOptions.Export.Pdf.AllowInheritedPageResources)
            {
                #region make resources list
                ArrayList resourcesList = new ArrayList();
                resourcesList.Add("/Resources");
                resourcesList.Add("<<");
                resourcesList.Add("/ProcSet [/PDF /Text /ImageC]");
                resourcesList.Add("/Font");
                resourcesList.Add("<<");
                for (int index = 0; index < fontsCounter; index++)
                {
                    resourcesList.Add(string.Format("/F{0} {1} 0 R", index, info.FontList[index].Ref));
                }
                resourcesList.Add(">>");
                if (imagesCounter > 0)
                {
                    resourcesList.Add("/XObject");
                    resourcesList.Add("<<");
                    for (int index = 0; index < imagesCounter; index++)
                    {
                        resourcesList.Add(string.Format("/Image{0} {1} 0 R", index, info.XObjectList[index].Ref));
                    }
                    resourcesList.Add(">>");
                }
                resourcesList.Add("/Pattern");
                resourcesList.Add("<<");
                resourcesList.Add(string.Format("/P1 {0} 0 R", info.Patterns.First.Ref));
                for (int indexPattern = 0; indexPattern < hatchCounter; indexPattern++)
                {
                    resourcesList.Add(string.Format("/PH{0} {1} 0 R", 1 + indexPattern, info.Patterns.HatchItems[indexPattern].Ref));
                }
                for (int indexPattern = 0; indexPattern < shadingCounter; indexPattern++)
                {
                    resourcesList.Add(string.Format("/P{0} {1} 0 R", 2 + indexPattern, info.Patterns.ShadingItems[indexPattern].Ref));
                }
                resourcesList.Add(">>");
                resourcesList.Add("/ColorSpace 3 0 R");
                if (StiOptions.Export.Pdf.AllowExtGState && useTransparency)
                {
                    resourcesList.Add(string.Format("/ExtGState {0} 0 R", info.ExtGState.Ref));
                }
                if (!usePdfA)
                {
                    resourcesList.Add(string.Format("/Properties << /oc1 {0} 0 R >>", info.OptionalContentGroup.Ref));
                }
                resourcesList.Add(">>");
                #endregion

                for (int indexResourcesList = 0; indexResourcesList < resourcesList.Count; indexResourcesList++)
                {
                    sw.WriteLine((string)resourcesList[indexResourcesList]);
                }
            }
            sw.WriteLine(">>");
            sw.WriteLine("endobj");
            sw.WriteLine("");
            #endregion

            #region StructTreeRoot
            AddXref(5);
            sw.WriteLine("5 0 obj");
            sw.WriteLine("<<");
            sw.WriteLine("/Type /StructTreeRoot");
            sw.WriteLine(">>");
            sw.WriteLine("endobj");
            sw.WriteLine("");
            #endregion

            #region OptionalContentGroup
            AddXref(6);
            sw.WriteLine("6 0 obj");
            sw.WriteLine("<<");
            sw.WriteLine("/Type /OCG");
            sw.WriteLine("/Name (Printable off)");
            sw.WriteLine("/Usage <<");
            sw.WriteLine("/Print << /PrintState /OFF >>");
            sw.WriteLine("/View << /ViewState /ON >>");
            sw.WriteLine(">>");
            sw.WriteLine(">>");
            sw.WriteLine("endobj");
            sw.WriteLine("");
            #endregion

            #region Page
            for (int indexPage = 0; indexPage < pages.Count; indexPage++)
            {
                AddXref(info.PageList[indexPage].Ref);
                sw.WriteLine("{0} 0 obj", info.PageList[indexPage].Ref);
                sw.WriteLine("<<");
                sw.WriteLine("/Type /Page");
                sw.WriteLine("/Parent 4 0 R");
                var tPage = pages.GetPageWithoutCache(indexPage);
                double pageHeight = hiToTwips * report.Unit.ConvertToHInches(tPage.PageHeight * tPage.SegmentPerHeight);
                double pageWidth = hiToTwips * report.Unit.ConvertToHInches(tPage.PageWidth * tPage.SegmentPerWidth);

                //Adobe Acrobat limitation of page size: maximum 200" * 200"
                if (pageHeight > 14400)
                {
                    pageHeight = 14400;
                }
                if (pageWidth > 14400)
                {
                    pageWidth = 14400;
                }

                sw.WriteLine("/MediaBox [ 0 0 {0} {1} ]", ConvertToString(pageWidth), ConvertToString(pageHeight));
                if (!StiOptions.Export.Pdf.AllowInheritedPageResources)
                {
                    #region make resources list
                    ArrayList resourcesList = new ArrayList();
                    resourcesList.Add("/Resources");
                    resourcesList.Add("<<");
                    resourcesList.Add("/ProcSet [/PDF /Text /ImageC]");
                    resourcesList.Add("/Font");
                    resourcesList.Add("<<");
                    for (int index = 0; index < fontsCounter; index++)
                    {
                        resourcesList.Add(string.Format("/F{0} {1} 0 R", index, info.FontList[index].Ref));
                    }
                    resourcesList.Add(">>");
                    if (imagesCounter > 0)
                    {
                        resourcesList.Add("/XObject");
                        resourcesList.Add("<<");
                        for (int index = 0; index < imagesCounter; index++)
                        {
                            resourcesList.Add(string.Format("/Image{0} {1} 0 R", index, info.XObjectList[index].Ref));
                        }
                        resourcesList.Add(">>");
                    }
                    StringBuilder sbb = new StringBuilder();
                    sbb.Append(string.Format("/P1 {0} 0 R ", info.Patterns.First.Ref));
                    for (int indexPattern = 0; indexPattern < hatchCounter; indexPattern++)
                    {
                        sbb.Append(string.Format("/PH{0} {1} 0 R ", 1 + indexPattern, info.Patterns.HatchItems[indexPattern].Ref));
                    }
                    for (int indexPattern = 0; indexPattern < shadingCounter; indexPattern++)
                    {
                        var ssd = (StiShadingData)shadingArray[indexPattern];
                        if (ssd.Page == indexPage)
                            sbb.Append(string.Format("/P{0} {1} 0 R ", 2 + indexPattern, info.Patterns.ShadingItems[indexPattern].Ref));
                    }
                    resourcesList.Add("/Pattern << " + sbb.ToString() + " >>");
                    resourcesList.Add("/ColorSpace 3 0 R");
                    if (StiOptions.Export.Pdf.AllowExtGState && useTransparency)
                    {
                        resourcesList.Add(string.Format("/ExtGState {0} 0 R", info.ExtGState.Ref));
                    }
                    if (!usePdfA)
                    {
                        resourcesList.Add(string.Format("/Properties << /oc1 {0} 0 R >>", info.OptionalContentGroup.Ref));
                    }
                    resourcesList.Add(">>");
                    #endregion

                    for (int indexResourcesList = 0; indexResourcesList < resourcesList.Count; indexResourcesList++)
                    {
                        sw.WriteLine((string)resourcesList[indexResourcesList]);
                    }
                }
                sw.WriteLine("/Contents {0} 0 R", info.PageList[indexPage].Content.Ref);
                if (useTransparency)
                {
                    sw.WriteLine("/Group");
                    sw.WriteLine("<<");
                    sw.WriteLine("/Type /Group");
                    sw.WriteLine("/S /Transparency");
                    sw.WriteLine("/CS /DeviceRGB");
                    sw.WriteLine(">>");
                }
                if (haveLinks || haveAnnots || haveDigitalSignature || haveTooltips)
                {
                    sw.WriteLine("/Annots [");
                    if (haveLinks)
                    {
                        for (int tempIndex = 0; tempIndex < linksCounter; tempIndex++)
                        {
                            StiLinkObject stl = (StiLinkObject)linksArray[tempIndex];
                            if (stl.Page == indexPage)
                            {
                                sw.WriteLine("{0} 0 R ", info.LinkList[tempIndex].Ref);
                            }
                        }
                    }
                    if (haveAnnots)
                    {
                        for (int tempIndex = 0; tempIndex < annotsCounter; tempIndex++)
                        {
                            StiEditableObject seo = (StiEditableObject)annotsArray[tempIndex];
                            if (seo.Page == indexPage)
                            {
                                sw.WriteLine("{0} 0 R ", info.AcroForm.Annots[tempIndex].Ref);
                            }
                        }
                        for (int tempIndex = 0; tempIndex < annots2Counter; tempIndex++)
                        {
                            StiEditableObject seo = annots2Array[tempIndex];
                            if (seo.Page == indexPage)
                            {
                                for (int indexState = 0; indexState < info.AcroForm.CheckBoxes[tempIndex].Count; indexState++)
                                {
                                    sw.WriteLine("{0} 0 R ", info.AcroForm.CheckBoxes[tempIndex][indexState].Ref);
                                }
                            }
                        }
                        for (int tempIndex = 0; tempIndex < unsignedSignaturesCounter; tempIndex++)
                        {
                            StiEditableObject seo = unsignedSignaturesArray[tempIndex];
                            if (seo.Page == indexPage)
                            {
                                sw.WriteLine("{0} 0 R ", info.AcroForm.UnsignedSignatures[tempIndex].Ref);
                            }
                        }
                    }
                    if (haveDigitalSignature && (indexPage == signaturePageNumber))
                    {
                        sw.WriteLine("{0} 0 R ", info.AcroForm.Signatures[0].Ref);
                    }
                    if (haveTooltips)
                    {
                        for (int tempIndex = 0; tempIndex < tooltipsCounter; tempIndex++)
                        {
                            StiLinkObject stl = tooltipsArray[tempIndex];
                            if (stl.Page == indexPage)
                            {
                                sw.WriteLine("{0} 0 R ", info.AcroForm.Tooltips[tempIndex].Ref);
                            }
                        }
                    }
                    sw.WriteLine("]");
                }

                sw.WriteLine(">>");
                sw.WriteLine("endobj");
                sw.WriteLine("");
            }
            #endregion
        }
        #endregion

        #region RenderEndDoc
        private void RenderEndDoc()
        {
            sw.Flush();
            long offs = sw.BaseStream.Position;

            sw.WriteLine("xref");
            sw.WriteLine("0 {0}", xref.Count + 1);
            sw.WriteLine("0000000000 65535 f");
            for (int index = 0; index < xref.Count; index++)
            {
                sw.WriteLine((string)xref.GetByIndex(index));
            }
            sw.WriteLine("trailer");
            sw.WriteLine("<<");
            sw.WriteLine("/Size {0}", xref.Count + 1);
            sw.WriteLine("/Root 1 0 R");
            sw.WriteLine("/Info 2 0 R");
            if (encrypted)
            {
                sw.WriteLine("/Encrypt {0} 0 R", info.Encode.Ref);
            }
            sw.WriteLine("/ID[<{0}><{0}>]", IDValueString);
            sw.WriteLine(">>");
#if NETSTANDARD
            sw.WriteLine("%StimulsoftNetCore" + (StiDpiHelper.IsWindows ? "Win" : "") + (StiDpiHelper.IsLinux ? "Linux" : "") + (StiDpiHelper.IsMacOsX ? "Mac" : ""));
#else
            sw.WriteLine("%Stimulsoft" + (StiOptions.Configuration.IsWPF ? "Wpf" : "Net") + (StiOptions.Configuration.IsWeb ? "Web" : ""));
#endif
            sw.WriteLine("%" + StiVersion.Version);
            sw.WriteLine("startxref");
            sw.WriteLine("{0}", offs);
            sw.WriteLine("%%EOF");
        }
        #endregion

        #region RenderPageHeader
        private void RenderPageHeader(int pageNumber)
        {
            AddXref(info.PageList[pageNumber].Content.Ref);
            sw.WriteLine("{0} 0 obj", info.PageList[pageNumber].Content.Ref);
            sw.WriteLine("<<");

            memoryPageStream = new MemoryStream();
            pageStream = new StreamWriter(memoryPageStream);
            pageStream.NewLine = "\r\n";

            pageStream.WriteLine("2 J");//**

            lastColorNonStrokeA = 255;
            lastColorStrokeA = 255;
        }
        #endregion

        #region RenderPageFooter
        private void RenderPageFooter(double pageH, double pageW)
        {
            #region Trial
#if CLOUD
            var isTrial = StiCloudPlan.IsTrial(report != null ? report.ReportGuid : null);
#elif SERVER
            var isTrial = StiVersionX.IsSvr;
#else
            var key = StiLicenseKeyValidator.GetLicenseKey();

            var isValidInDesigner = StiLicenseKeyValidator.IsValidInReportsDesignerOrOnPlatform(StiProductIdent.Net, key);
            var isTrial = !(isValidInDesigner && Base.Design.StiDesignerAppStatus.IsRunning || StiLicenseKeyValidator.IsValidOnNetFramework(key));

            if (!typeof(StiLicense).AssemblyQualifiedName.Contains(StiPublicKeyToken.Key))
                isTrial = true;

            #region IsValidLicenseKey
		    if (!isTrial)
		    {
		        try
		        {
		            using (var rsa = new RSACryptoServiceProvider(512))
		            using (var sha = new SHA1CryptoServiceProvider())
		            {
		                rsa.FromXmlString("<RSAKeyValue><Modulus>iyWINuM1TmfC9bdSA3uVpBG6cAoOakVOt+juHTCw/gxz/wQ9YZ+Dd9vzlMTFde6HAWD9DC1IvshHeyJSp8p4H3qXUKSC8n4oIn4KbrcxyLTy17l8Qpi0E3M+CI9zQEPXA6Y1Tg+8GVtJNVziSmitzZddpMFVr+6q8CRi5sQTiTs=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");
		                isTrial = !rsa.VerifyData(key.GetCheckBytes(), sha, key.GetSignatureBytes());
		            }
		        }
		        catch (Exception)
		        {
		            isTrial = true;
		        }
		    }
            #endregion
#endif

            if (isTrial)
            {
                double tempX = pageW / 596d * 1.4d;
                double tempY = pageH / 840d * 1.4d;
                if (tempX > tempY) tempX = tempY;
                else tempY = tempX;
                pageStream.WriteLine("q");
                PushColorToStack();
                pageStream.WriteLine("1 J 1 j 20 w");
                //				pageStream.WriteLine("/Pattern CS /P1 SCN");
                SetStrokeColor(Color.FromArgb(64, 100, 100, 100));
                pageStream.WriteLine("{0} 0 0 {1} {2} {3} cm 0.707 0.707 -0.707 0.707 0 0 cm 1 0 0 1 -155 -50 cm",
                    ConvertToString(tempX), ConvertToString(tempY), ConvertToString(pageW / 2), ConvertToString(pageH / 2));
                pageStream.WriteLine("40 0 m 40 100 l 0 100 m 80 100 l S");
                pageStream.WriteLine("100 0 m 100 70 l 100 45 m 120 65 l 130 72 l 140 68 l S");
                pageStream.WriteLine("170 0 m 170 70 l 169 100 m 171 100 l S");
                pageStream.WriteLine("215 60 m 222 69 l 232 71 l 255 70 l 265 60 l 265 5 l 270 0 l 265 44 m 220 31 l 212 20 l 212 10 l 225 0 l 235 0 l 250 5 l 265 18 l S");
                pageStream.WriteLine("310 0 m 310 100 l S");
                pageStream.WriteLine("Q");
                PopColorFromStack();
            }
            #endregion

            pageStream.Flush();
            if (memoryPageStream.Position < memoryPageStream.Length)
            {
                memoryPageStream.SetLength(memoryPageStream.Position);
            }

            if (compressed == true)
            {
                byte[] bytedata = memoryPageStream.ToArray();
                MemoryStream TmpStream = StiExportUtils.MakePdfDeflateStream(bytedata);

                //sw.WriteLine("/Filter [/FlateDecode] /Length {0}", TmpStream.Length);
                //sw.WriteLine(">>");
                //sw.WriteLine("stream");
                //StoreMemoryStream(TmpStream);
                StoreMemoryStream2(TmpStream, "/Filter [/FlateDecode] /Length {0}");
            }
            else
            {
                //sw.WriteLine("/Filter [] /Length {0}", memoryPageStream.Length);
                //sw.WriteLine(">>");
                //sw.WriteLine("stream");
                //StoreMemoryStream(memoryPageStream);
                StoreMemoryStream2(memoryPageStream, "/Filter [] /Length {0}");
            }
            sw.WriteLine("");
            pageStream.Close();
            memoryPageStream.Close();

            sw.WriteLine("endstream");
            sw.WriteLine("endobj");
            sw.WriteLine("");
        }
        #endregion

        #region RenderFontTable
        private void RenderFontTable()
        {
            //write fonts table
            for (int index = 0; index < fontsCounter; index++)
            {
                StringBuilder tempSb = new StringBuilder();
                pdfFont.CurrentFont = index;
                PdfFonts.pfontInfo tempInfo = (PdfFonts.pfontInfo)pdfFont.fontList[index];

                string fontName = tempInfo.Name;
                if (StiOptions.Export.UseAlternativeFontNames && StiOptions.Export.AlternativeFontNames.ContainsKey(fontName))
                {
                    fontName = (string)StiOptions.Export.AlternativeFontNames[fontName];
                }
                bool needEmbeddedFonts = embeddedFonts || PdfFonts.IsFontStimulsoft(tempInfo.Name) || SignatureFonts.StiSignatureFontsHelper.IsSignatureFont(tempInfo.Name);
                bool isSymbolic = PdfFonts.IsFontSymbolic(fontName);

                if (pdfFont.UseUnicode)
                {
                    #region main font info for Unicode
                    tempSb = new StringBuilder(CorrectNameEnconding(fontName));
                    if ((tempInfo.Bold == true) || (tempInfo.Italic == true))
                    {
                        tempSb.Append(",");
                        if (tempInfo.Bold == true) tempSb.Append("Bold");
                        if (tempInfo.Italic == true) tempSb.Append("Italic");
                    }
                    if (needEmbeddedFonts)
                    {
                        Random rand = new Random();
                        tempSb.Insert(0, string.Format("SR{0}{1}{2}{3}+",
                            (char)rand.Next('A', 'Z'),
                            (char)rand.Next('A', 'Z'),
                            (char)rand.Next('A', 'Z'),
                            (char)rand.Next('A', 'Z')));
                    }

                    AddXref(info.FontList[index].Ref);
                    sw.WriteLine("{0} 0 obj", info.FontList[index].Ref);
                    sw.WriteLine("<<");
                    sw.WriteLine("/Type /Font");
                    sw.WriteLine("/Subtype /Type0");
                    sw.WriteLine("/BaseFont /{0}", tempSb);
                    sw.WriteLine("/DescendantFonts [{0} 0 R]", info.FontList[index].DescendantFont.Ref);
                    sw.WriteLine("/Encoding /Identity-H");
                    sw.WriteLine("/ToUnicode {0} 0 R", info.FontList[index].ToUnicode.Ref);
                    sw.WriteLine("/Name /F{0}", index);
                    sw.WriteLine(">>");
                    sw.WriteLine("endobj");
                    sw.WriteLine("");

                    AddXref(info.FontList[index].DescendantFont.Ref);
                    sw.WriteLine("{0} 0 obj", info.FontList[index].DescendantFont.Ref);
                    sw.WriteLine("<<");
                    sw.WriteLine("/Type /Font");
                    sw.WriteLine("/Subtype /CIDFontType2");
                    sw.WriteLine("/BaseFont /{0}", tempSb);
                    sw.WriteLine("/CIDSystemInfo");
                    sw.WriteLine("<<");
                    //sw.WriteLine("/Registry (Adobe)");
                    //sw.WriteLine("/Ordering (Identity)");
                    StoreStringLine("/Registry", "Adobe");
                    StoreStringLine("/Ordering", "Identity");
                    sw.WriteLine("/Supplement 0");
                    sw.WriteLine(">>");
                    sw.WriteLine("/FontDescriptor {0} 0 R", info.FontList[index].FontDescriptor.Ref);
                    if (usePdfA)
                    {
                        sw.WriteLine("/CIDToGIDMap /Identity");
                    }
                    sw.WriteLine("/W [0 [1000]");

                    ushort[] pdfGlyphList = pdfFont.GlyphList;
                    int pdfGlyphListLength = pdfGlyphList.Length;

                    //make arrays
                    int[] glyphList = new int[pdfGlyphListLength];
                    int[] glyphListBack = new int[pdfGlyphListLength];
                    for (int glyphIndex = 32; glyphIndex < pdfGlyphListLength; glyphIndex++)
                    {
                        glyphList[glyphIndex] = pdfGlyphList[glyphIndex];
                        glyphListBack[glyphIndex] = glyphIndex;
                    }
                    //sort arrays
                    for (int index1 = 32; index1 < glyphList.Length - 1; index1++)
                    {
                        for (int index2 = index1 + 1; index2 < glyphList.Length; index2++)
                        {
                            if (glyphList[index1] > glyphList[index2])
                            {
                                int tempGlyph = glyphList[index1];
                                glyphList[index1] = glyphList[index2];
                                glyphList[index2] = tempGlyph;
                                int tempGlyphBack = glyphListBack[index1];
                                glyphListBack[index1] = glyphListBack[index2];
                                glyphListBack[index2] = tempGlyphBack;
                            }
                        }
                    }

                    StringBuilder sb = new StringBuilder();
                    int mapIndex = 32;
                    while (mapIndex < pdfFont.MappedSymbolsCount)
                    {
                        sb.Append(glyphList[mapIndex].ToString() + " [");
                        sb.Append(pdfFont.Widths[glyphListBack[mapIndex] - 32].ToString());
                        mapIndex++;
                        while ((mapIndex < pdfFont.MappedSymbolsCount) &&
                            (glyphList[mapIndex] - 1 == glyphList[mapIndex - 1]))
                        {
                            sb.Append(" " + pdfFont.Widths[glyphListBack[mapIndex] - 32].ToString());
                            mapIndex++;
                        }
                        //some symbols may be translated to one glyph
                        while ((mapIndex < pdfFont.MappedSymbolsCount) && (glyphList[mapIndex] == glyphList[mapIndex - 1]))
                        {
                            mapIndex++;
                        }
                        sb.Append("]");
                        sw.WriteLine("{0}", sb);
                        sb = new StringBuilder();
                    }
                    if (pdfFont.GlyphWidths != null && pdfFont.GlyphWidths[0xFFFF] != 0)
                    {
                        for (int indexGlyph = 0; indexGlyph < 65535; indexGlyph++)
                        {
                            if ((pdfFont.GlyphWidths[indexGlyph] > 0) && (pdfFont.GlyphBackList[indexGlyph] == 0))
                            {
                                sw.WriteLine(string.Format("{0} [{1}]", indexGlyph, pdfFont.GlyphWidths[indexGlyph]));
                            }
                        }
                    }
                    sw.WriteLine("]");
                    sw.WriteLine(">>");
                    sw.WriteLine("endobj");
                    sw.WriteLine("");
                    #endregion

                    #region ToUnicode
                    AddXref(info.FontList[index].ToUnicode.Ref);
                    string cmapFontName = "SR+F" + index.ToString();
                    sw.WriteLine("{0} 0 obj", info.FontList[index].ToUnicode.Ref);
                    sw.WriteLine("<<");

                    #region prepare memory stream and buffer
                    MemoryStream mem = new MemoryStream();
                    StreamWriter swmem = new StreamWriter(mem);
                    swmem.NewLine = "\r\n";
                    swmem.WriteLine("/CIDInit /ProcSet findresource begin");
                    swmem.WriteLine("12 dict begin");
                    swmem.WriteLine("begincmap");
                    swmem.WriteLine("/CIDSystemInfo");
                    swmem.WriteLine("<<");
                    swmem.WriteLine("/Registry (Adobe)");
                    swmem.WriteLine("/Ordering ({0})", cmapFontName);
                    swmem.WriteLine("/Supplement 0");
                    swmem.WriteLine(">> def");
                    swmem.WriteLine("/CMapName /{0} def", cmapFontName);
                    swmem.WriteLine("/CMapType 2 def");
                    swmem.WriteLine("1 begincodespacerange");
                    swmem.WriteLine("<0000> <FFFF>");
                    swmem.WriteLine("endcodespacerange");

                    int countBfChar = pdfFont.MappedSymbolsCount - 32;
                    int offsetBfChar = 32;
                    while (countBfChar > 0)
                    {
                        int count = countBfChar;
                        if (count > 100) count = 100;
                        swmem.WriteLine("{0} beginbfchar", count);
                        for (int indexUni = 0; indexUni < count; indexUni++)
                        {
                            swmem.WriteLine("<{0:X4}> <{1:X4}>", pdfFont.GlyphList[offsetBfChar], pdfFont.UnicodeMapBack[offsetBfChar]);
                            offsetBfChar++;
                        }
                        swmem.WriteLine("endbfchar");
                        countBfChar -= count;
                    }

                    swmem.WriteLine("endcmap");
                    swmem.WriteLine("CMapName currentdict /CMap defineresource pop");
                    swmem.WriteLine("end");
                    swmem.WriteLine("end");
                    swmem.Flush();
                    byte[] buff = mem.ToArray();
                    mem.Close();
                    #endregion

                    if (compressed == true)
                    {
                        MemoryStream TmpStream = StiExportUtils.MakePdfDeflateStream(buff);

                        //sw.WriteLine("/Length {0} /Filter [/FlateDecode] /Length1 {1}", TmpStream.Length, buff.Length);
                        //sw.WriteLine(">>");
                        //sw.WriteLine("stream");
                        //StoreMemoryStream(TmpStream);
                        StoreMemoryStream2(TmpStream, "/Length {0} /Filter [/FlateDecode] /Length1 " + buff.Length.ToString());
                    }
                    else
                    {
                        //MemoryStream TmpStream = new MemoryStream();
                        //TmpStream.Write(buff, 0, buff.Length);
                        //sw.WriteLine("/Length {0} /Filter [] /Length1 {0}", buff.Length);
                        //sw.WriteLine(">>");
                        //sw.WriteLine("stream");
                        //StoreMemoryStream(TmpStream);
                        StoreMemoryStream2(buff, "/Length {0} /Filter [] /Length1 " + buff.Length.ToString());
                    }
                    sw.WriteLine("");
                    sw.WriteLine("endstream");
                    sw.WriteLine("endobj");
                    sw.WriteLine("");
                    #endregion

                    #region CIDSet
                    AddXref(info.FontList[index].CIDSet.Ref);
                    sw.WriteLine("{0} 0 obj", info.FontList[index].CIDSet.Ref);
                    sw.WriteLine("<<");

                    #region prepare buffer
                    BitArray bits = new BitArray(65536 + 32, false);
                    int maxIndex = 0;
                    for (int indexGlyph = 0; indexGlyph < pdfFont.GlyphList.Length; indexGlyph++)
                    {
                        ushort glyph = pdfFont.GlyphList[indexGlyph];
                        if (glyph != 65535)
                        {
                            bits[glyph] = true;
                            if (glyph > maxIndex) maxIndex = glyph;
                        }
                    }
                    if (pdfFont.GlyphWidths != null && pdfFont.GlyphWidths[0xFFFF] != 0)
                    {
                        for (int indexGlyph = 0; indexGlyph < 65535; indexGlyph++)
                        {
                            if ((pdfFont.GlyphWidths[indexGlyph] > 0) && (pdfFont.GlyphBackList[indexGlyph] == 0))
                            {
                                bits[indexGlyph] = true;
                                if (indexGlyph > maxIndex) maxIndex = indexGlyph;
                            }
                        }
                    }
                    int bytesCount = maxIndex / 8 + 1;
                    byte[] buff2 = new byte[bytesCount + 1];
                    int pos = 0;
                    while (pos < bytesCount)
                    {
                        int offset = pos * 8;
                        byte val = 0;
                        if (bits[offset]) val |= 0x80;
                        if (bits[offset + 1]) val |= 0x40;
                        if (bits[offset + 2]) val |= 0x20;
                        if (bits[offset + 3]) val |= 0x10;
                        if (bits[offset + 4]) val |= 0x08;
                        if (bits[offset + 5]) val |= 0x04;
                        if (bits[offset + 6]) val |= 0x02;
                        if (bits[offset + 7]) val |= 0x01;
                        buff2[pos] = val;
                        pos++;
                    }
                    #endregion

                    if (compressed == true)
                    {
                        MemoryStream TmpStream = StiExportUtils.MakePdfDeflateStream(buff2);
                        //sw.WriteLine("/Length {0} /Filter [/FlateDecode] /Length1 {1}", TmpStream.Length, buff2.Length);
                        //sw.WriteLine(">>");
                        //sw.WriteLine("stream");
                        //StoreMemoryStream(TmpStream);
                        StoreMemoryStream2(TmpStream, "/Length {0} /Filter [/FlateDecode] /Length1 " + buff2.Length.ToString());
                    }
                    else
                    {
                        //sw.WriteLine("/Length {0} /Filter [] /Length1 {0}", buff2.Length);
                        //sw.WriteLine(">>");
                        //sw.WriteLine("stream");
                        //MemoryStream TmpStream = new MemoryStream();
                        //TmpStream.Write(buff2, 0, buff2.Length);
                        //StoreMemoryStream(TmpStream);
                        StoreMemoryStream2(buff2, "/Length {0} /Filter [] /Length1 " + buff2.Length.ToString());
                    }
                    sw.WriteLine("");
                    sw.WriteLine("endstream");
                    sw.WriteLine("endobj");
                    sw.WriteLine("");
                    #endregion

                }
                else
                {
                    #region main font info
                    AddXref(info.FontList[index].Ref);
                    sw.WriteLine("{0} 0 obj", info.FontList[index].Ref);
                    sw.WriteLine("<<");
                    sw.WriteLine("/Type /Font");
                    if (standardPdfFonts == true)
                    {
                        sw.WriteLine("/Subtype /Type1");
                        sw.WriteLine("/BaseFont /{0}", tempInfo.PdfName);
                    }
                    else
                    {
                        sw.WriteLine("/Subtype /TrueType");
                        tempSb = new StringBuilder(CorrectNameEnconding(fontName));
                        if ((tempInfo.Bold == true) || (tempInfo.Italic == true))
                        {
                            tempSb.Append(",");
                            if (tempInfo.Bold == true) tempSb.Append("Bold");
                            if (tempInfo.Italic == true) tempSb.Append("Italic");
                        }
                        sw.WriteLine("/BaseFont /{0}", tempSb);
                        sw.WriteLine("/FontDescriptor {0} 0 R", info.FontList[index].FontDescriptor.Ref);
                    }
                    sw.WriteLine("/Encoding {0} 0 R", info.FontList[index].Encoding.Ref);

                    //make string "Widths"
                    StringBuilder sbWidths = new StringBuilder(" ");
                    //if (isSymbolic)
                    //{
                    //    for (int indexUni = 32; indexUni < 255; indexUni++)
                    //    {
                    //        int sym = pdfFont.UnicodeMap[indexUni + 0xF000];
                    //        int width = (sym > 0) ? pdfFont.Widths[sym - 32] : 1000;
                    //        sbWidths.Append(width.ToString() + " ");
                    //    }
                    //}
                    //else
                    //{
                        for (int indexUni = 32; indexUni < pdfFont.MappedSymbolsCount; indexUni++)
                        {
                            sbWidths.Append(pdfFont.Widths[indexUni - 32].ToString() + " ");
                        }
                    //}
                    sw.WriteLine("/FirstChar {0}", 32);
                    //sw.WriteLine("/LastChar {0}", isSymbolic ? 254 : pdfFont.MappedSymbolsCount - 1);
                    sw.WriteLine("/LastChar {0}", pdfFont.MappedSymbolsCount - 1);
                    sw.WriteLine("/Widths [{0}]", sbWidths);
                    sw.WriteLine("/Name /F{0}", index);
                    sw.WriteLine(">>");
                    sw.WriteLine("endobj");
                    sw.WriteLine("");
                    #endregion

                    #region Encoding
                    AddXref(info.FontList[index].Encoding.Ref);
                    sw.WriteLine("{0} 0 obj", info.FontList[index].Encoding.Ref);
                    sw.WriteLine("<<");
                    sw.WriteLine("/Type /Encoding");
                    sw.WriteLine("/BaseEncoding /WinAnsiEncoding");
                    if ((pdfFont.MappedSymbolsCount > PdfFonts.FirstMappedSymbol) && !isSymbolic)
                    {
                        //make string "Differences"
                        StringBuilder sbDifferences = new StringBuilder(PdfFonts.FirstMappedSymbol.ToString() + " ");
                        for (int indexUni = PdfFonts.FirstMappedSymbol; indexUni < pdfFont.MappedSymbolsCount; indexUni++)
                        {
                            sbDifferences.Append("/" + pdfFont.CharPdfNames[indexUni]);
                        }
                        sw.WriteLine("/Differences [{0}]", sbDifferences);
                    }
                    sw.WriteLine(">>");
                    sw.WriteLine("endobj");
                    sw.WriteLine("");
                    #endregion
                }

                byte[] fontBuff = null;
                if (needEmbeddedFonts == true)
                {
                    pdfFont.GetFontDataBuf(tempInfo.Font, out fontBuff, pdfFont.GlyphBackList == null);
                }

                if (standardPdfFonts == false)
                {
                    #region FontDescriptor
                    AddXref(info.FontList[index].FontDescriptor.Ref);
                    sw.WriteLine("{0} 0 obj", info.FontList[index].FontDescriptor.Ref);
                    sw.WriteLine("<<");
                    sw.WriteLine("/Type /FontDescriptor");
                    sw.WriteLine("/FontName /{0}", tempSb);
                    sw.WriteLine("/Flags {0}", isSymbolic ? 4 : 32);
                    //sw.WriteLine("/Ascent {0}", pdfFont.ASC);
                    sw.WriteLine("/Ascent {0}", pdfFont.tmASC);
                    sw.WriteLine("/CapHeight {0}", pdfFont.CH);
                    //sw.WriteLine("/Descent {0}", pdfFont.DESC);
                    sw.WriteLine("/Descent {0}", pdfFont.tmDESC);
                    sw.WriteLine("/FontBBox [{0} {1} {2} {3}]", pdfFont.LLX, pdfFont.LLY, pdfFont.URX, pdfFont.URY);
                    sw.WriteLine("/ItalicAngle {0}", pdfFont.ItalicAngle);
                    sw.WriteLine("/StemV {0}", pdfFont.StemV);
                    if (needEmbeddedFonts && (fontBuff != null) && (fontBuff.Length > 0))
                    {
                        sw.WriteLine("/FontFile2 {0} 0 R", info.FontList[tempInfo.ParentFontNumber == -1 ? index : tempInfo.ParentFontNumber].FontFile2.Ref);
                    }

                    bool needCIDSet = pdfFont.UseUnicode == true;
                    if (pdfComplianceMode == StiPdfComplianceMode.A2 || pdfComplianceMode == StiPdfComplianceMode.A3) needCIDSet = false;
                    if (needCIDSet)
                    {
                        sw.WriteLine("/CIDSet {0} 0 R", info.FontList[index].CIDSet.Ref);
                    }

                    sw.WriteLine(">>");
                    sw.WriteLine("endobj");
                    sw.WriteLine("");
                    #endregion
                }

                if (needEmbeddedFonts)
                {
                    #region embedded
                    if ((fontBuff != null) && (fontBuff.Length > 0) && (tempInfo.ParentFontNumber == -1))
                    {
                        if (reduceFontSize)
                        {
                            pdfFont.OpenTypeHelper.ReduceFontSize(ref fontBuff, tempInfo.Name, pdfFont.UseUnicode && !fontGlyphsReduceNotNeed[index], pdfFont.GlyphList, pdfFont.GlyphRtfList); //tempInfo.Name or fontName ?
                        }

                        AddXref(info.FontList[index].FontFile2.Ref);
                        sw.WriteLine("{0} 0 obj", info.FontList[index].FontFile2.Ref);
                        sw.WriteLine("<<");
                        if (compressedFonts == true)
                        {
                            MemoryStream TmpStream = StiExportUtils.MakePdfDeflateStream(fontBuff);
                            //sw.WriteLine("/Length {0} /Filter [/FlateDecode] /Length1 {1}", TmpStream.Length, buff.Length);
                            //sw.WriteLine(">>");
                            //sw.WriteLine("stream");
                            //StoreMemoryStream(TmpStream);
                            StoreMemoryStream2(TmpStream, "/Length {0} /Filter [/FlateDecode] /Length1 " + fontBuff.Length.ToString());
                        }
                        else
                        {
                            //sw.WriteLine("/Length {0} /Filter [] /Length1 {0}", buff.Length);
                            //sw.WriteLine(">>");
                            //sw.WriteLine("stream");
                            //MemoryStream TmpStream = new MemoryStream();
                            //TmpStream.Write(buff, 0, buff.Length);
                            //StoreMemoryStream(TmpStream);
                            StoreMemoryStream2(fontBuff, "/Length {0} /Filter [] /Length1 " + fontBuff.Length.ToString());
                        }
                        sw.WriteLine("");
                        sw.WriteLine("endstream");
                        sw.WriteLine("endobj");
                        sw.WriteLine("");
                    }
                    else
                    {
                        AddXref(info.FontList[index].FontFile2.Ref);
                        sw.WriteLine("{0} 0 obj", info.FontList[index].FontFile2.Ref);
                        sw.WriteLine("<< >>");
                        sw.WriteLine("endobj");
                        sw.WriteLine("");
                    }
                    #endregion
                }

            }

        }
        #endregion

        #region RenderImageTable
        private void RenderImageTable()
        {
            if (imagesCounter > 0)
            {
                for (int index = 0; index < imagesCounter; index++)
                {
                    //StiImageData pd = (StiImageData)imageList[index];
                    StiImageData pd = (StiImageData)imageCacheIndexToList[index];
                    byte[] bytes = (byte[])imageCache.ImagePackedStore[index];
                    byte[] mask = (byte[])imageCache.ImageMaskStore[index];
                    Color[] palette = (Color[])imageCache.ImagePaletteStore[index];
                    StiImageFormat imageFormatCurrent = pd.ImageFormat;

                    #region write image
                    AddXref(info.XObjectList[index].Ref);
                    sw.WriteLine("{0} 0 obj", info.XObjectList[index].Ref);
                    sw.WriteLine("<<");
                    sw.WriteLine("/Type /XObject");
                    sw.WriteLine("/Subtype /Image");
                    if ((imageCompressionMethod == StiPdfImageCompressionMethod.Indexed) && (imageFormatCurrent != StiImageFormat.Monochrome))
                    {
                        if (encrypted)
                        {
                            byte[] forEncrypt = new byte[palette.Length * 3];
                            int indexColor = 0;
                            foreach (Color color in palette)
                            {
                                forEncrypt[indexColor++] = color.R;
                                forEncrypt[indexColor++] = color.G;
                                forEncrypt[indexColor++] = color.B;
                            }
                            byte[] encryptedData = pdfSecurity.EncryptData(forEncrypt, currentObjectNumber, currentGenerationNumber);
                            StringBuilder tempSB = new StringBuilder();
                            for (int indexByte = 0; indexByte < encryptedData.Length; indexByte++)
                            {
                                tempSB.Append(encryptedData[indexByte].ToString("X2"));
                            }
                            sw.WriteLine("/ColorSpace [/Indexed /DeviceRGB {0} <{1}> ]", palette.Length - 1, tempSB.ToString());
                        }
                        else
                        {
                            StringBuilder sbPalette = new StringBuilder();
                            foreach (Color color in palette) sbPalette.AppendFormat("{0:X2}{1:X2}{2:X2} ", color.R, color.G, color.B);
                            sw.WriteLine("/ColorSpace [/Indexed /DeviceRGB {0} <{1}> ]", palette.Length - 1, sbPalette);
                        }
                    }
                    else
                    {
                        sw.WriteLine("/ColorSpace /{0}", imageFormatCurrent == StiImageFormat.Monochrome ? "DeviceGray" : "DeviceRGB");
                    }
                    sw.WriteLine("/Width {0}", pd.Width);
                    sw.WriteLine("/Height {0}", pd.Height);
                    sw.WriteLine("/BitsPerComponent {0}", imageFormatCurrent == StiImageFormat.Monochrome ? "1" : "8");
                    if (!usePdfA && imageInterpolationTable.ContainsKey(index))
                    {
                        sw.WriteLine("/Interpolate true");
                    }
                    if (mask != null)
                    {
                        sw.WriteLine("/SMask {0} 0 R", info.XObjectList[index].Mask.Ref);
                    }
                    sw.WriteLine("/Name /{0}", pd.Name);

                    string compressionType = string.Empty;
                    switch (imageCompressionMethod)
                    {
                        case StiPdfImageCompressionMethod.Flate:
                        case StiPdfImageCompressionMethod.Indexed:
                            compressionType = "FlateDecode";
                            break;

                        //case StiPdfImageCompressionMethod.Jpeg:
                        default:
                            compressionType = "DCTDecode";
                            break;
                    }

#if TESTPDF
                    //sw.WriteLine("/Length {0}",  bytes.Length * 2);
                    //sw.WriteLine("/Filter [/ASCIIHexDecode /{0}]", compressionType);
                    string stImage = "/Length {0} " + string.Format("/Filter [/ASCIIHexDecode /{0}]", compressionType);
#else
                    //sw.WriteLine("/Length {0}", bytes.Length);
                    //sw.WriteLine("/Filter [/{0}]", compressionType);
                    string stImage = "/Length {0} " + string.Format("/Filter [/{0}]", compressionType);
#endif

                    //sw.WriteLine(">>");
                    //sw.WriteLine("stream");
                    MemoryStream TmpStream = new MemoryStream();

#if TESTPDF
					byte[] buf = new byte[bytes.Length * 2];
					for (int indexb = 0; indexb < bytes.Length; indexb++)
					{
						string st = string.Format("{0:X2}", bytes[indexb]);
						buf[indexb * 2 + 0] = (byte)st[0];
						buf[indexb * 2 + 1] = (byte)st[1];
					}
					TmpStream.Write(buf, 0, buf.Length);
#else
                    TmpStream.Write(bytes, 0, bytes.Length);
#endif

                    //StoreMemoryStream(TmpStream);
                    StoreMemoryStream2(TmpStream, stImage);

                    sw.WriteLine("");
                    sw.WriteLine("endstream");
                    sw.WriteLine("endobj");
                    sw.WriteLine("");
                    #endregion

                    if (mask != null)
                    {
                        #region write mask
                        AddXref(info.XObjectList[index].Mask.Ref);
                        sw.WriteLine("{0} 0 obj", info.XObjectList[index].Mask.Ref);
                        sw.WriteLine("<<");
                        sw.WriteLine("/Type /XObject");
                        sw.WriteLine("/Subtype /Image");
                        sw.WriteLine("/ColorSpace /DeviceGray");
                        //if (!((imageCompressionMethod == StiPdfImageCompressionMethod.Indexed) && (imageFormatCurrent != StiImageFormat.Monochrome)))
                        //{
                        //    sw.WriteLine("/Matte [ 0 0 0 ]");
                        //}
                        sw.WriteLine("/Width {0}", pd.Width);
                        sw.WriteLine("/Height {0}", pd.Height);
                        sw.WriteLine("/BitsPerComponent 8");
                        if (!usePdfA && imageInterpolationTable.ContainsKey(index))
                        {
                            sw.WriteLine("/Interpolate true");
                        }

#if TESTPDF
                        //sw.WriteLine("/Length {0}",  mask.Length * 2);
                        //sw.WriteLine("/Filter [/ASCIIHexDecode /FlateDecode]");
                        string stMask = "/Length {0} /Filter [/ASCIIHexDecode /FlateDecode]";
#else
                        //sw.WriteLine("/Length {0}", mask.Length);
                        //sw.WriteLine("/Filter /FlateDecode");
                        string stMask = "/Length {0} /Filter /FlateDecode";
#endif

                        //sw.WriteLine(">>");
                        //sw.WriteLine("stream");
                        MemoryStream TmpStream2 = new MemoryStream();

#if TESTPDF
						byte[] buf2 = new byte[mask.Length * 2];
						for (int indexb = 0; indexb < mask.Length; indexb++)
						{
							string st = string.Format("{0:X2}", mask[indexb]);
							buf2[indexb * 2 + 0] = (byte)st[0];
							buf2[indexb * 2 + 1] = (byte)st[1];
						}
						TmpStream2.Write(buf2, 0, buf2.Length);
#else
                        TmpStream2.Write(mask, 0, mask.Length);
#endif

                        //StoreMemoryStream(TmpStream2);
                        StoreMemoryStream2(TmpStream2, stMask);

                        sw.WriteLine("");
                        sw.WriteLine("endstream");
                        sw.WriteLine("endobj");
                        sw.WriteLine("");
                        #endregion
                    }
                }
            }
        }
        #endregion

        #region RenderBookmarksTable
        private void RenderBookmarksTable()
        {
            if (haveBookmarks == true)
            {
                AddXref(info.Outlines.Ref);
                sw.WriteLine("{0} 0 obj", info.Outlines.Ref);
                sw.WriteLine("<<");
                sw.WriteLine("/Type /Outlines");
                sw.WriteLine("/First {0} 0 R", info.Outlines.Items[0].Ref);
                sw.WriteLine("/Last {0} 0 R", info.Outlines.Items[0].Ref);
                sw.WriteLine("/Count {0}", 1);
                sw.WriteLine(">>");
                sw.WriteLine("endobj");
                sw.WriteLine("");

                for (int index = 0; index < bookmarksCounter; index++)
                {
                    AddXref(info.Outlines.Items[index].Ref);
                    StiBookmarkTreeNode tn = (StiBookmarkTreeNode)bookmarksTree[index];
                    sw.WriteLine("{0} 0 obj", info.Outlines.Items[index].Ref);
                    sw.WriteLine("<<");
                    //					sw.WriteLine("/Title {0}", ConvertToHexString(tn.Title));
                    StoreStringLine("/Title ", tn.Title, true);
                    if (tn.Parent != -1) sw.WriteLine("/Parent {0} 0 R", info.Outlines.Items[tn.Parent].Ref);
                    if (tn.Prev != -1) sw.WriteLine("/Prev {0} 0 R", info.Outlines.Items[tn.Prev].Ref);
                    if (tn.Next != -1) sw.WriteLine("/Next {0} 0 R", info.Outlines.Items[tn.Next].Ref);
                    if (tn.First != -1) sw.WriteLine("/First {0} 0 R", info.Outlines.Items[tn.First].Ref);
                    if (tn.Last != -1) sw.WriteLine("/Last {0} 0 R", info.Outlines.Items[tn.Last].Ref);
                    if (tn.Count > 0)
                    {
                        if (index == 0)
                        {
                            sw.WriteLine("/Count {0}", tn.Count);
                        }
                        else
                        {
                            sw.WriteLine("/Count {0}", -tn.Count);
                        }
                    }
                    if (tn.Y > -1) sw.WriteLine("/Dest [{0} 0 R /XYZ null {1} null]", info.PageList[tn.Page].Ref, ConvertToString(tn.Y));
                    sw.WriteLine(">>");
                    sw.WriteLine("endobj");
                    sw.WriteLine("");
                }
            }
        }
        #endregion

        #region RenderPatternTable
        private void RenderPatternTable()
        {
            AddXref(info.Patterns.Resources.Ref);
            sw.WriteLine("{0} 0 obj", info.Patterns.Resources.Ref);
            sw.WriteLine("<< /ProcSet [/PDF] >>");
            sw.WriteLine("endobj");
            sw.WriteLine("");

            #region render first pattern
            AddXref(info.Patterns.First.Ref);
            sw.WriteLine("{0} 0 obj", info.Patterns.First.Ref);
            sw.WriteLine("<<");
            sw.WriteLine("/Type /Pattern");
            sw.WriteLine("/PatternType 1");
            sw.WriteLine("/PaintType 1");
            sw.WriteLine("/TilingType 1");
            sw.WriteLine("/BBox [0 0 2 2]");
            sw.WriteLine("/XStep 3");
            sw.WriteLine("/YStep 3");
            sw.WriteLine("/Resources {0} 0 R", info.Patterns.Resources.Ref);
            //
            memoryPageStream = new MemoryStream();
            StreamWriter swmemStream = new StreamWriter(memoryPageStream);
            swmemStream.NewLine = "\r\n";
            swmemStream.WriteLine("1 J 1 j 1 w");
            swmemStream.WriteLine("1 0 0 RG");
            swmemStream.Write("1 1 m 1.1 1.1 l S");
            swmemStream.Flush();
            //
            //sw.WriteLine("/Length {0}", memoryPageStream.Length);
            //sw.WriteLine(">>");
            //sw.WriteLine("stream");
            //StoreMemoryStream(memoryPageStream);
            StoreMemoryStream2(memoryPageStream, "/Length {0}");

            sw.WriteLine("");
            sw.WriteLine("endstream");
            sw.WriteLine("endobj");
            sw.WriteLine("");
            #endregion

            if (hatchCounter > 0)
            {
                for (int indexHatch = 0; indexHatch < hatchCounter; indexHatch++)
                {
                    WriteHatchPattern(indexHatch);
                }
            }

            if (shadingCounter > 0)
            {
                for (int indexShading = 0; indexShading < shadingCounter; indexShading++)
                {
                    WriteShadingPattern(indexShading);
                }
                sw.WriteLine("");
            }
            if (shadingFunctionCounter > 0)
            {
                for (int indexShadingFunction = 0; indexShadingFunction < shadingFunctionCounter; indexShadingFunction++)
                {
                    WriteShadingFunction(indexShadingFunction);
                }
                sw.WriteLine("");
            }

            memoryPageStream.Close();
        }

        private void WriteHatchPattern(int indexHatch)
        {
            StiHatchBrush hatch = (StiHatchBrush)hatchArray[indexHatch];
            int hatchNumber = (int)hatch.Style;
            if (hatchNumber > 53) hatchNumber = 53;

            AddXref(info.Patterns.HatchItems[indexHatch].Ref);
            sw.WriteLine("{0} 0 obj", info.Patterns.HatchItems[indexHatch].Ref);
            sw.WriteLine("<<");
            sw.WriteLine("/Type /Pattern");
            sw.WriteLine("/PatternType 1");
            sw.WriteLine("/PaintType 1");
            sw.WriteLine("/TilingType 1");
            sw.WriteLine("/BBox [0 0 1 1]");
            sw.WriteLine("/XStep 1");
            sw.WriteLine("/YStep 1");
            sw.WriteLine("/Resources {0} 0 R", info.Patterns.Resources.Ref);
            sw.WriteLine("/Matrix [5.5 0 0 5.5 0 0]");
            //
            memoryPageStream = new MemoryStream();
            StreamWriter memsw = new StreamWriter(memoryPageStream);
            memsw.NewLine = "\r\n";

            if (hatch.BackColor.A != 0)
            {
                memsw.WriteLine("{0} {1} {2} rg",
                    colorTable[hatch.BackColor.R],
                    colorTable[hatch.BackColor.G],
                    colorTable[hatch.BackColor.B]);
                memsw.WriteLine("0 0 1 1 re f");
            }
            memsw.WriteLine("{0} {1} {2} rg",
                colorTable[hatch.ForeColor.R],
                colorTable[hatch.ForeColor.G],
                colorTable[hatch.ForeColor.B]);
            memsw.WriteLine("BI");
            memsw.WriteLine("/W 8");
            memsw.WriteLine("/H 8");
            memsw.WriteLine("/BPC 1");
            memsw.WriteLine("/IM true");
            memsw.WriteLine("/D [1 0]");
            memsw.WriteLine("/F [/AHx]");
            memsw.WriteLine("ID");
            memsw.WriteLine(hatchData[hatchNumber] + ">");
            memsw.WriteLine("EI");
            memsw.Flush();
            //
            //sw.WriteLine("/Length {0}", memoryPageStream.Length);
            //sw.WriteLine(">>");
            //sw.WriteLine("stream");
            //StoreMemoryStream(memoryPageStream);
            StoreMemoryStream2(memoryPageStream, "/Length {0}");

            //sw.WriteLine("");
            sw.WriteLine("endstream");
            sw.WriteLine("endobj");
            sw.WriteLine("");
        }

        private void WriteShadingPattern(int indexShading)
        {
            StiShadingData ssd = (StiShadingData)shadingArray[indexShading];

            double xs = 1;
            double ys = 1;
            double angle = ssd.Angle;
            if (angle < 0) angle += 360;
            if ((angle >= 270) && (angle < 360))
            {
                angle = 360 - angle;
                ys = -1;
            };
            if ((angle >= 180) && (angle < 270))
            {
                angle = angle - 180;
                ys = -1;
                xs = -1;
            };
            if ((angle >= 90) && (angle < 180))
            {
                angle = 180 - angle;
                xs = -1;
            };
            angle = angle * Math.PI / 180f;

            double x0 = ssd.X + ssd.Width / 2f;
            double y0 = ssd.Y + ssd.Height / 2f;
            double r = Math.Sqrt(ssd.Width * ssd.Width + ssd.Height * ssd.Height) / 2f;
            double a2 = Math.Atan2(ssd.Height, ssd.Width);
            double st = Math.PI / 2f - angle + a2;
            double b = r * Math.Sin(st);
            double xr = b * Math.Cos(angle) * xs;
            double yr = b * Math.Sin(angle) * ys;

            double x1 = x0 - xr;
            double x2 = x0 + xr;
            double y1 = y0 + yr;
            double y2 = y0 - yr;

            AddXref(info.Patterns.ShadingItems[indexShading].Ref);
            sw.WriteLine("{0} 0 obj", info.Patterns.ShadingItems[indexShading].Ref);
            sw.Write("<<");
            sw.Write("/PatternType 2");
            sw.Write("/Shading <<");
            sw.Write("/ColorSpace /DeviceRGB");
            sw.Write("/ShadingType 2");
            sw.Write("/Coords [{0} {1} {2} {3}]",
                ConvertToString(x1),
                ConvertToString(y1),
                ConvertToString(x2),
                ConvertToString(y2));
            sw.Write("/Extend [true true]");
            sw.Write("/Function {0} 0 R", info.Patterns.ShadingFunctionItems[ssd.FunctionIndex].Ref);
            sw.Write(">> ");
            sw.WriteLine(">>");
            sw.WriteLine("endobj");
            //sw.WriteLine("");
        }

        private void WriteShadingFunction(int indexShadingFunction)
        {
            var sf = shadingFunctionArray[indexShadingFunction];

            AddXref(info.Patterns.ShadingFunctionItems[indexShadingFunction].Ref);
            sw.WriteLine("{0} 0 obj", info.Patterns.ShadingFunctionItems[indexShadingFunction].Ref);
            sw.Write("<<");
            sw.Write("/FunctionType 0");
            sw.Write("/Size [3]");
            sw.Write("/Decode [0 1 0 1 0 1]");
            sw.Write("/Range [0 1 0 1 0 1]");
            sw.Write("/BitsPerSample 8");
            sw.Write("/Domain [0 1]");
            sw.Write("/Encode [0 {0}]", sf.IsGlare ? 2 : 1);
            sw.Write("/Order 1");
            //
            memoryPageStream = new MemoryStream();
#if TESTPDF
                string str = string.Format("{0:X2}{1:X2}{2:X2}{3:X2}{4:X2}{5:X2}{0:X2}{1:X2}{2:X2}",
                    sf.Color1.R,
                    sf.Color1.G,
                    sf.Color1.B,
                    sf.Color2.R,
                    sf.Color2.G,
                    sf.Color2.B);
                for (int indexb = 0; indexb < str.Length; indexb++)
                {
                    memoryPageStream.WriteByte((byte)str[indexb]);
                }
                //sw.WriteLine("/Length {0}", memoryPageStream.Length);
                //sw.WriteLine("/Filter [/ASCIIHexDecode]");
                string stShade = "/Length {0} /Filter [/ASCIIHexDecode]";
#else
            memoryPageStream.WriteByte(sf.Color1.R);
            memoryPageStream.WriteByte(sf.Color1.G);
            memoryPageStream.WriteByte(sf.Color1.B);
            memoryPageStream.WriteByte(sf.Color2.R);
            memoryPageStream.WriteByte(sf.Color2.G);
            memoryPageStream.WriteByte(sf.Color2.B);
            memoryPageStream.WriteByte(sf.Color1.R);
            memoryPageStream.WriteByte(sf.Color1.G);
            memoryPageStream.WriteByte(sf.Color1.B);
            //sw.WriteLine("/Length {0}", memoryPageStream.Length);
            string stShade = "/Length {0}";
#endif
            //
            //sw.WriteLine(">>");
            //sw.WriteLine("stream");
            //StoreMemoryStream(memoryPageStream);
            StoreMemoryStream2(memoryPageStream, stShade);

            sw.WriteLine("");
            sw.WriteLine("endstream");
            sw.WriteLine("endobj");
            //sw.WriteLine("");
        }
        #endregion

        #region RenderLinkTable
        private void RenderLinkTable()
        {
            if (haveLinks == true)
            {
                for (int index = 0; index < linksCounter; index++)
                {
                    StiLinkObject stl = (StiLinkObject)linksArray[index];

                    //check link - intenal or external; if internal - get destination
                    if (stl.Link.StartsWith("##") && tagsArray.Count > 0)
                    {
                        //internal link to tag
                        string st = stl.Link.Substring(2);
                        for (int indexTag = 0; indexTag < tagsArray.Count; indexTag++)
                        {
                            StiLinkObject tag = tagsArray[indexTag];
                            if (tag.Link == st)
                            {
                                stl.DestPage = tag.Page;
                                stl.DestY = tag.Y + tag.Height;
                                break;
                            }
                        }
                    }
                    else
                    {
                        //internal link to bookmark
                        if ((stl.Link[0] == '#') && (haveBookmarks == true))
                        {
                            string st = stl.Link.Substring(1);
                            if ((st.Length > 2) && (st[0] == '%'))
                            {
                                int pos = st.LastIndexOf(st[1]);
                                if (pos < st.Length - 1)
                                {
                                    st = st.Substring(pos + 1);
                                }
                            }
                            for (int indexBookmark = 0; indexBookmark < bookmarksCounter; indexBookmark++)
                            {
                                StiBookmarkTreeNode tn = (StiBookmarkTreeNode)bookmarksTree[indexBookmark];
                                if (tn.Title == st)
                                {
                                    stl.DestPage = tn.Page;
                                    stl.DestY = tn.Y;
                                    break;
                                }
                            }
                        }
                    }
                    linksArray[index] = stl;

                    AddXref(info.LinkList[index].Ref);
                    sw.WriteLine("{0} 0 obj", info.LinkList[index].Ref);
                    sw.WriteLine("<<");
                    sw.Write("/Type /Annot ");
                    sw.WriteLine("/Subtype /Link");
                    sw.WriteLine("/Rect [{0} {1} {2} {3}]", ConvertToString(stl.X), ConvertToString(stl.Y), ConvertToString(stl.X + stl.Width), ConvertToString(stl.Y + stl.Height));
                    sw.WriteLine("/Border [0 0 0]");
                    if (stl.Link[0] == '#')
                    {
                        if (stl.DestY > -1) sw.WriteLine("/Dest [{0} 0 R /XYZ null {1} null]", info.PageList[stl.DestPage].Ref, ConvertToString(stl.DestY));
                        //else sw.WriteLine("/Dest [{0} 0 R /XYZ null null null]", info.PageList[0].Ref);
                    }
                    else
                    {
                        string link = stl.Link.Replace('\\', '/');
                        bool flag = (link.Length > 6) &&
                            ((char.IsLetter(link, 0) && (link[1] == ':') && (link[2] == '/')) ||
                            ((link[0] == '/') && char.IsLetter(link, 1) && (link[2] == '/')));
                        if (flag && (link[1] == ':')) link = "/" + link[0] + link.Substring(2);

                        sw.Write("/A << ");
                        sw.Write("/Type /Action ");
                        if (flag)
                        {
                            sw.Write("/S /Launch ");
                            sw.Write("/F << ");
                            sw.Write("/Type /Filespec ");
                            StoreStringLine("/F ", link);
                            StoreStringLine("/UF ", link);
                            sw.WriteLine(">>");
                        }
                        else
                        {
                            sw.Write("/S /URI ");
                            StoreStringLine("/URI ", StiExportUtils.StringToUrl(link));
                        }
                        sw.WriteLine(">>");
                    }
                    sw.WriteLine("/F 4");
                    sw.WriteLine(">>");
                    sw.WriteLine("endobj");
                    sw.WriteLine("");
                }
            }
        }
        #endregion

        #region RenderAnnotTable
        private void RenderAnnotTable()
        {
            if (haveAnnots || haveDigitalSignature || haveTooltips)
            {
                #region AcroForm object
                AddXref(info.AcroForm.Ref);
                sw.WriteLine("{0} 0 obj", info.AcroForm.Ref);
                sw.WriteLine("<<");
                sw.WriteLine("/Fields [");
                for (int index = 0; index < annotsCounter; index++)
                {
                    sw.WriteLine("{0} 0 R", info.AcroForm.Annots[index].Ref);
                }
                for (int index = 0; index < annots2Counter; index++)
                {
                    for (int indexState = 0; indexState < info.AcroForm.CheckBoxes[index].Count; indexState++)
                    {
                        sw.WriteLine("{0} 0 R", info.AcroForm.CheckBoxes[index][indexState].Ref);
                    }
                }
                for (int index = 0; index < unsignedSignaturesCounter; index++)
                {
                    sw.WriteLine("{0} 0 R", info.AcroForm.UnsignedSignatures[index].Ref);
                }
                if (haveDigitalSignature)
                {
                    sw.WriteLine("{0} 0 R", info.AcroForm.Signatures[0].Ref);
                }
                for (int index = 0; index < tooltipsCounter; index++)
                {
                    sw.WriteLine("{0} 0 R", info.AcroForm.Tooltips[index].Ref);
                }
                sw.WriteLine("]");

                sw.WriteLine("/DR <<");
                if (haveAnnots)
                //if (annotsCounter > 0 || annots2Counter > 0)
                {
                    sw.WriteLine("/Font <<");
                    for (int index = 0; index < fontsCounter; index++)
                    {
                        sw.WriteLine("/FA{0} {1} 0 R", index, info.AcroForm.AnnotFontItems[index].Ref);
                    }
                    sw.WriteLine(">>");
                    //sw.WriteLine("/Encoding << /PDFDocEncoding {0} 0 R >>", tempOffset);
                }
                if (unsignedSignaturesCounter > 0)
                {
                    sw.WriteLine("/XObject <<");
                    for (int index = 0; index < info.AcroForm.UnsignedSignatures.Count; index++)
                    {
                        sw.WriteLine("/DSz{0} {1} 0 R", index, info.AcroForm.UnsignedSignatures[index].AP.Ref);
                    }
                    sw.WriteLine(">>");
                }
                sw.WriteLine(">>");
                if (haveAnnots && (fontsCounter > 0))
                {
                    StoreStringLine("/DA ", "/FA0 0 Tf 0 g");
                }
                if (haveDigitalSignature)
                {
                    sw.WriteLine("/SigFlags 3");
                }
                sw.WriteLine(">>");
                sw.WriteLine("endobj");
                sw.WriteLine("");
                #endregion

                if (haveAnnots)
                {
                    Hashtable fieldsNames = new Hashtable();

                    for (int index = 0; index < annotsCounter; index++)
                    {
                        StiEditableObject seo = (StiEditableObject)annotsArray[index];

                        #region Make field name
                        string fieldName = string.Empty;
                        if (StiOptions.Export.Pdf.UseEditableFieldName) fieldName = seo.Component.Name;
                        if (StiOptions.Export.Pdf.UseEditableFieldAlias) fieldName = seo.Component.Alias;
                        if (StiOptions.Export.Pdf.UseEditableFieldTag) fieldName = seo.Component.TagValue as string;
                        if (string.IsNullOrEmpty(fieldName)) fieldName = string.Format("Field{0}", index);
                        if (fieldsNames.ContainsKey(fieldName))
                        {
                            int indexName = 2;
                            string nameAdd = string.Empty;
                            while (fieldsNames.ContainsKey(fieldName + nameAdd))
                            {
                                nameAdd = "_" + indexName.ToString();
                                indexName++;
                            }
                            fieldName = fieldName + nameAdd;
                        }
                        fieldsNames.Add(fieldName, fieldName);
                        #endregion

                        #region Annot object
                        AddXref(info.AcroForm.Annots[index].Ref);
                        sw.WriteLine("{0} 0 obj", info.AcroForm.Annots[index].Ref);
                        sw.WriteLine("<<");
                        sw.WriteLine("/Type /Annot");
                        sw.WriteLine("/Subtype /Widget");
                        sw.WriteLine("/Rect [{0} {1} {2} {3}]",
                            ConvertToString(seo.X),
                            ConvertToString(seo.Y),
                            ConvertToString(seo.X + seo.Width),
                            ConvertToString(seo.Y + seo.Height));
                        sw.WriteLine("/F 4");
                        sw.WriteLine("/P {0} 0 R", info.PageList[seo.Page].Ref);
                        sw.WriteLine("/FT /Tx");
                        sw.WriteLine("/BS << /W 0 >>");
                        StoreStringLine("/T ", fieldName);
                        sw.WriteLine("/MK << /TP 2 >>");
                        sw.WriteLine("/H /P");
                        sw.WriteLine("/AP << /N {0} 0 R >>", info.AcroForm.Annots[index].AP.Ref);
                        int flagFf = seo.Multiline ? 0x1000 : 0;
                        if (((seo.Component as StiText) != null) && ((seo.Component as StiText).TextOptions != null))
                        {
                            if ((seo.Component as StiText).TextOptions.LineLimit) flagFf |= 0x800000;
                        }
                        sw.WriteLine("/Ff {0}", flagFf);
                        //  "/MaxLen 55"  //Limit of NN characters
                        StoreStringLine("/DA ", string.Format("/FA{0} {1} Tf {2} {3} {4} rg",
                            seo.FontNumber,
                            ConvertToString(seo.FontSize, precision_digits_font),
                            colorTable[seo.FontColor.R],
                            colorTable[seo.FontColor.G],
                            colorTable[seo.FontColor.B]));
                        StoreStringLine("/V ", seo.Text, true);
                        StoreStringLine("/DV ", seo.Text, true);
                        sw.WriteLine("/Q {0}", (seo.Alignment == StiTextHorAlignment.Center ? 1 : 0) +
                            (seo.Alignment == StiTextHorAlignment.Right ? 2 : 0));
                        sw.WriteLine(">>");
                        sw.WriteLine("endobj");
                        sw.WriteLine("");
                        #endregion

                        #region /AP object
                        AddXref(info.AcroForm.Annots[index].AP.Ref);
                        sw.WriteLine("{0} 0 obj", info.AcroForm.Annots[index].AP.Ref);
                        sw.WriteLine("<<");
                        sw.WriteLine("/Subtype /Form");
                        sw.WriteLine("/BBox [{0} {1} {2} {3}]",
                            ConvertToString(0),
                            ConvertToString(0),
                            ConvertToString(seo.Width),
                            ConvertToString(seo.Height));
                        sw.WriteLine("/Resources <<");
                        sw.WriteLine("/ProcSet [ /PDF /Text ]");
                        sw.Write("/Font << ");
                        for (int indexf = 0; indexf < fontsCounter; indexf++)
                        {
                            sw.Write(string.Format("/F{0} {1} 0 R ", indexf, info.FontList[indexf].Ref));
                        }
                        sw.WriteLine(">> >>");

                        MemoryStream annotMemSw = new MemoryStream();
                        StreamWriter annotSw = new StreamWriter(annotMemSw, Encoding.GetEncoding(1252));
                        annotSw.NewLine = "\r\n";

                        //stream
                        annotSw.WriteLine("/Tx BMC");
                        annotSw.WriteLine("q");
                        int borderOffset = 1;
                        annotSw.WriteLine("{0} {1} {2} {3} re W n",
                            ConvertToString(borderOffset),
                            ConvertToString(borderOffset),
                            ConvertToString(seo.Width - borderOffset),
                            ConvertToString(seo.Height - borderOffset));
                        annotSw.Flush();
                        annotSw.BaseStream.Write(seo.Content, 0, seo.Content.Length);
                        annotSw.WriteLine("Q");
                        annotSw.WriteLine("EMC");
                        annotSw.Flush();

                        //sw.WriteLine("/Filter [] /Length {0}", annotMemSw.Length);
                        //sw.WriteLine(">>");
                        //sw.WriteLine("stream");
                        //StoreMemoryStream(annotMemSw);
                        StoreMemoryStream2(annotMemSw, "/Filter [] /Length {0}");

                        sw.WriteLine("");
                        annotSw.Close();
                        annotMemSw.Close();

                        sw.WriteLine("endstream");
                        sw.WriteLine("endobj");
                        sw.WriteLine("");
                        #endregion
                    }

                    for (int indexFont = 0; indexFont < fontsCounter; indexFont++)
                    {
                        #region Annot font 
                        pdfFont.CurrentFont = indexFont;
                        PdfFonts.pfontInfo tempInfo = (PdfFonts.pfontInfo)pdfFont.fontList[indexFont];

                        string fontName = tempInfo.Name;
                        if (StiOptions.Export.UseAlternativeFontNames && StiOptions.Export.AlternativeFontNames.ContainsKey(fontName))
                        {
                            fontName = (string)StiOptions.Export.AlternativeFontNames[fontName];
                        }

                        StringBuilder tempSb = new StringBuilder(CorrectNameEnconding(fontName));
                        if (tempInfo.Bold || tempInfo.Italic)
                        {
                            tempSb.Append(",");
                            if (tempInfo.Bold) tempSb.Append("Bold");
                            if (tempInfo.Italic) tempSb.Append("Italic");
                        }

                        AddXref(info.AcroForm.AnnotFontItems[indexFont].Ref);
                        sw.WriteLine("{0} 0 obj", info.AcroForm.AnnotFontItems[indexFont].Ref);
                        sw.WriteLine("<<");
                        sw.WriteLine("/Type /Font");
                        sw.WriteLine("/Subtype /TrueType");
                        sw.WriteLine("/Name /FA{0}", indexFont);
                        sw.WriteLine("/BaseFont /{0}", tempSb);
                        sw.WriteLine("/Encoding /WinAnsiEncoding");
                        sw.WriteLine("/FontDescriptor {0} 0 R", info.AcroForm.AnnotFontItems[indexFont].FontDescriptor.Ref);

                        if (fontGlyphsReduceNotNeed[indexFont])
                        {
                            //make string "Widths"
                            StringBuilder sbWidths = new StringBuilder(" ");
                            for (int indexUni = 32; indexUni < 256; indexUni++)
                            {
                                sbWidths.Append(pdfFont.Widths[pdfFont.UnicodeMap[CodePage1252[indexUni]] - 32].ToString() + " ");
                            }
                            sw.WriteLine("/FirstChar 32");
                            sw.WriteLine("/LastChar 255");
                            sw.WriteLine("/Widths [{0}]", sbWidths);
                        }

                        sw.WriteLine(">>");
                        sw.WriteLine("endobj");
                        sw.WriteLine("");

                        byte[] fontBuff = null;
                        bool needEmbeddedFonts = embeddedFonts || PdfFonts.IsFontStimulsoft(tempInfo.Name) || SignatureFonts.StiSignatureFontsHelper.IsSignatureFont(tempInfo.Name);
                        if (needEmbeddedFonts)
                        {
                            pdfFont.GetFontDataBuf(tempInfo.Font, out fontBuff, pdfFont.GlyphBackList == null);
                        }

                        AddXref(info.AcroForm.AnnotFontItems[indexFont].FontDescriptor.Ref);
                        sw.WriteLine("{0} 0 obj", info.AcroForm.AnnotFontItems[indexFont].FontDescriptor.Ref);
                        sw.WriteLine("<<");
                        sw.WriteLine("/Type /FontDescriptor");
                        sw.WriteLine("/FontName /{0}", tempSb);
                        sw.WriteLine("/Flags 32");
                        //sw.WriteLine("/Ascent {0}", pdfFont.ASC);
                        sw.WriteLine("/Ascent {0}", pdfFont.MacAscend);
                        sw.WriteLine("/CapHeight {0}", pdfFont.CH);
                        //sw.WriteLine("/Descent {0}", pdfFont.DESC);
                        sw.WriteLine("/Descent {0}", pdfFont.MacDescend);
                        sw.WriteLine("/FontBBox [{0} {1} {2} {3}]", pdfFont.LLX, pdfFont.LLY, pdfFont.URX, pdfFont.URY);
                        sw.WriteLine("/ItalicAngle {0}", pdfFont.ItalicAngle);
                        sw.WriteLine("/StemV {0}", pdfFont.StemV);
                        if (needEmbeddedFonts && (fontBuff != null) && (fontBuff.Length > 0))
                        {
                            sw.WriteLine("/FontFile2 {0} 0 R", info.FontList[indexFont].FontFile2.Ref);
                        }
                        sw.WriteLine(">>");
                        sw.WriteLine("endobj");
                        sw.WriteLine("");
                        #endregion
                    }

                    for (int index = 0; index < annots2Counter; index++)
                    {
                        StiEditableObject seo = annots2Array[index];

                        #region Make field name
                        string fieldName = string.Empty;
                        if (StiOptions.Export.Pdf.UseEditableFieldName) fieldName = seo.Component.Name;
                        if (StiOptions.Export.Pdf.UseEditableFieldAlias) fieldName = seo.Component.Alias;
                        if (StiOptions.Export.Pdf.UseEditableFieldTag) fieldName = seo.Component.TagValue as string;
                        if (string.IsNullOrEmpty(fieldName)) fieldName = string.Format("Checkbox{0}", index + 1);
                        if (fieldsNames.ContainsKey(fieldName))
                        {
                            int indexName = 2;
                            string nameAdd = string.Empty;
                            while (fieldsNames.ContainsKey(fieldName + nameAdd))
                            {
                                nameAdd = "_" + indexName.ToString();
                                indexName++;
                            }
                            fieldName = fieldName + nameAdd;
                        }
                        fieldsNames.Add(fieldName, fieldName);
                        #endregion

                        const double checkboxSizeCorrection = 0.01;

                        StiCheckBox checkbox = seo.Component as StiCheckBox;
                        bool checkBoxValue = GetCheckBoxValue(checkbox).GetValueOrDefault(false);

                        if (info.AcroForm.CheckBoxes[index].Count > 1)
                        {
                            #region Annot object - Yes
                            var annot = info.AcroForm.CheckBoxes[index][0];
                            AddXref(annot.Ref);
                            sw.WriteLine("{0} 0 obj", annot.Ref);
                            sw.WriteLine("<<");
                            sw.WriteLine("/Type /Annot");
                            sw.WriteLine("/Subtype /Widget");
                            sw.WriteLine("/FT /Btn");
                            sw.WriteLine("/Rect [{0} {1} {2} {3}]",
                                ConvertToString(seo.X),
                                ConvertToString(seo.Y),
                                ConvertToString(seo.X + seo.Width - checkboxSizeCorrection),
                                ConvertToString(seo.Y + seo.Height - checkboxSizeCorrection));
                            sw.WriteLine("/F {0}", 4 + (info.AcroForm.CheckBoxes[index].Count == 3 || !checkBoxValue ? 2 : 0));
                            sw.WriteLine("/P {0} 0 R", info.PageList[seo.Page].Ref);
                            StoreStringLine("/T ", fieldName + "Yes");
                            sw.WriteLine("/MK << >>");
                            sw.WriteLine("/Ff 65536");
                            sw.WriteLine("/A {0} 0 R", annot.AA[0].Ref);
                            sw.WriteLine("/AP << /N {0} 0 R >>", annot.AP.Ref);
                            sw.WriteLine("/H /O");
                            sw.WriteLine(">>");
                            sw.WriteLine("endobj");
                            sw.WriteLine("");
                            #endregion

                            #region /AP object
                            AddXref(annot.AP.Ref);
                            sw.WriteLine("{0} 0 obj", annot.AP.Ref);
                            sw.WriteLine("<<");
                            sw.WriteLine("/Type /XObject");
                            sw.WriteLine("/Subtype /Form");
                            sw.WriteLine("/FormType 1");
                            sw.WriteLine("/BBox [{0} {1} {2} {3}]",
                                ConvertToString(0),
                                ConvertToString(0),
                                ConvertToString(seo.Width),
                                ConvertToString(seo.Height));
                            sw.WriteLine("/Matrix [ 1 0 0 1 0 0 ]");
                            sw.WriteLine("/Resources <<");
                            sw.WriteLine("/ProcSet [ /PDF ]");
                            sw.WriteLine("/Pattern");
                            sw.WriteLine("<<");
                            for (int indexPattern = 0; indexPattern < hatchCounter; indexPattern++)
                            {
                                sw.WriteLine(string.Format("/PH{0} {1} 0 R", 1 + indexPattern, info.Patterns.HatchItems[indexPattern].Ref));
                            }
                            for (int indexPattern = 0; indexPattern < shadingCounter; indexPattern++)
                            {
                                sw.WriteLine(string.Format("/P{0} {1} 0 R", 2 + indexPattern, info.Patterns.ShadingItems[indexPattern].Ref));
                            }
                            sw.WriteLine(">>");
                            sw.WriteLine("/ColorSpace 3 0 R");
                            sw.WriteLine(">>");

                            MemoryStream annotMemSw = new MemoryStream();
                            StreamWriter annotSw = new StreamWriter(annotMemSw, Encoding.GetEncoding(1252));
                            annotSw.NewLine = "\r\n";

                            //stream
                            annotSw.WriteLine("/Tx BMC");
                            annotSw.WriteLine("q");
                            int borderOffset = 1;
                            annotSw.WriteLine("{0} {1} {2} {3} re W n",
                                ConvertToString(borderOffset),
                                ConvertToString(borderOffset),
                                ConvertToString(seo.Width - borderOffset),
                                ConvertToString(seo.Height - borderOffset));
                            annotSw.Flush();
                            annotSw.BaseStream.Write(seo.Content, 0, seo.Content.Length);
                            annotSw.WriteLine("Q");
                            annotSw.WriteLine("EMC");
                            annotSw.Flush();

                            StoreMemoryStream2(annotMemSw, "/Filter [] /Length {0}");

                            sw.WriteLine("");
                            annotSw.Close();
                            annotMemSw.Close();

                            sw.WriteLine("endstream");
                            sw.WriteLine("endobj");
                            sw.WriteLine("");
                            #endregion

                            #region /AA objects
                            AddXref(annot.AA[0].Ref);
                            sw.WriteLine("{0} 0 obj", annot.AA[0].Ref);
                            sw.WriteLine("<<");
                            sw.WriteLine("/Next {0} 0 R", annot.AA[1].Ref);
                            sw.WriteLine("/S /Hide");
                            sw.WriteLine("/T ({0})", fieldName + "Yes");
                            sw.WriteLine(">>");
                            sw.WriteLine("endobj");
                            sw.WriteLine("");

                            AddXref(annot.AA[1].Ref);
                            sw.WriteLine("{0} 0 obj", annot.AA[1].Ref);
                            sw.WriteLine("<<");
                            sw.WriteLine("/H false");
                            sw.WriteLine("/S /Hide");
                            sw.WriteLine("/T ({0})", fieldName + "No");
                            sw.WriteLine(">>");
                            sw.WriteLine("endobj");
                            sw.WriteLine("");
                            #endregion

                            #region Annot object - No
                            annot = info.AcroForm.CheckBoxes[index][1];
                            AddXref(annot.Ref);
                            sw.WriteLine("{0} 0 obj", annot.Ref);
                            sw.WriteLine("<<");
                            sw.WriteLine("/Type /Annot");
                            sw.WriteLine("/Subtype /Widget");
                            sw.WriteLine("/FT /Btn");
                            sw.WriteLine("/Rect [{0} {1} {2} {3}]",
                                ConvertToString(seo.X),
                                ConvertToString(seo.Y),
                                ConvertToString(seo.X + seo.Width - checkboxSizeCorrection),
                                ConvertToString(seo.Y + seo.Height - checkboxSizeCorrection));
                            sw.WriteLine("/F {0}", 4 + (info.AcroForm.CheckBoxes[index].Count == 3 || checkBoxValue ? 2 : 0));
                            sw.WriteLine("/P {0} 0 R", info.PageList[seo.Page].Ref);
                            StoreStringLine("/T ", fieldName + "No");
                            sw.WriteLine("/MK << >>");
                            sw.WriteLine("/Ff 65536");
                            sw.WriteLine("/A {0} 0 R", annot.AA[0].Ref);
                            sw.WriteLine("/AP << /N {0} 0 R >>", annot.AP.Ref);
                            sw.WriteLine("/H /O");
                            sw.WriteLine(">>");
                            sw.WriteLine("endobj");
                            sw.WriteLine("");
                            #endregion

                            #region /AP object
                            AddXref(annot.AP.Ref);
                            sw.WriteLine("{0} 0 obj", annot.AP.Ref);
                            sw.WriteLine("<<");
                            sw.WriteLine("/Type /XObject");
                            sw.WriteLine("/Subtype /Form");
                            sw.WriteLine("/FormType 1");
                            sw.WriteLine("/BBox [{0} {1} {2} {3}]",
                                ConvertToString(0),
                                ConvertToString(0),
                                ConvertToString(seo.Width),
                                ConvertToString(seo.Height));
                            sw.WriteLine("/Matrix [ 1 0 0 1 0 0 ]");
                            sw.WriteLine("/Resources <<");
                            sw.WriteLine("/ProcSet [ /PDF ]");
                            sw.WriteLine("/Pattern");
                            sw.WriteLine("<<");
                            for (int indexPattern = 0; indexPattern < hatchCounter; indexPattern++)
                            {
                                sw.WriteLine(string.Format("/PH{0} {1} 0 R", 1 + indexPattern, info.Patterns.HatchItems[indexPattern].Ref));
                            }
                            for (int indexPattern = 0; indexPattern < shadingCounter; indexPattern++)
                            {
                                sw.WriteLine(string.Format("/P{0} {1} 0 R", 2 + indexPattern, info.Patterns.ShadingItems[indexPattern].Ref));
                            }
                            sw.WriteLine(">>");
                            sw.WriteLine("/ColorSpace 3 0 R");
                            sw.WriteLine(">>");

                            annotMemSw = new MemoryStream();
                            annotSw = new StreamWriter(annotMemSw, Encoding.GetEncoding(1252));
                            annotSw.NewLine = "\r\n";

                            //stream
                            annotSw.WriteLine("/Tx BMC");
                            annotSw.WriteLine("q");
                            borderOffset = 1;
                            annotSw.WriteLine("{0} {1} {2} {3} re W n",
                                ConvertToString(borderOffset),
                                ConvertToString(borderOffset),
                                ConvertToString(seo.Width - borderOffset),
                                ConvertToString(seo.Height - borderOffset));
                            annotSw.Flush();
                            annotSw.BaseStream.Write(seo.Content2, 0, seo.Content2.Length);
                            annotSw.WriteLine("Q");
                            annotSw.WriteLine("EMC");
                            annotSw.Flush();

                            StoreMemoryStream2(annotMemSw, "/Filter [] /Length {0}");

                            sw.WriteLine("");
                            annotSw.Close();
                            annotMemSw.Close();

                            sw.WriteLine("endstream");
                            sw.WriteLine("endobj");
                            sw.WriteLine("");
                            #endregion

                            #region /AA objects
                            AddXref(annot.AA[0].Ref);
                            sw.WriteLine("{0} 0 obj", annot.AA[0].Ref);
                            sw.WriteLine("<<");
                            sw.WriteLine("/H false");
                            sw.WriteLine("/Next {0} 0 R", annot.AA[1].Ref);
                            sw.WriteLine("/S /Hide");
                            sw.WriteLine("/T ({0})", fieldName + "Yes");
                            sw.WriteLine(">>");
                            sw.WriteLine("endobj");
                            sw.WriteLine("");

                            AddXref(annot.AA[1].Ref);
                            sw.WriteLine("{0} 0 obj", annot.AA[1].Ref);
                            sw.WriteLine("<<");
                            sw.WriteLine("/S /Hide");
                            sw.WriteLine("/T ({0})", fieldName + "No");
                            sw.WriteLine(">>");
                            sw.WriteLine("endobj");
                            sw.WriteLine("");
                            #endregion
                        }

                        if (info.AcroForm.CheckBoxes[index].Count == 3)
                        {
                            #region Annot object - None
                            var annot = info.AcroForm.CheckBoxes[index][2];
                            AddXref(annot.Ref);
                            sw.WriteLine("{0} 0 obj", annot.Ref);
                            sw.WriteLine("<<");
                            sw.WriteLine("/Type /Annot");
                            sw.WriteLine("/Subtype /Widget");
                            sw.WriteLine("/FT /Btn");
                            sw.WriteLine("/Rect [{0} {1} {2} {3}]",
                                ConvertToString(seo.X),
                                ConvertToString(seo.Y),
                                ConvertToString(seo.X + seo.Width - checkboxSizeCorrection),
                                ConvertToString(seo.Y + seo.Height - checkboxSizeCorrection));
                            sw.WriteLine("/F 4");
                            sw.WriteLine("/P {0} 0 R", info.PageList[seo.Page].Ref);
                            StoreStringLine("/T ", fieldName + "None");
                            sw.WriteLine("/MK << >>");
                            sw.WriteLine("/Ff 65536");
                            sw.WriteLine("/A {0} 0 R", annot.AA[0].Ref);
                            sw.WriteLine("/H /O");
                            sw.WriteLine(">>");
                            sw.WriteLine("endobj");
                            sw.WriteLine("");
                            #endregion

                            #region /AA objects
                            AddXref(annot.AA[0].Ref);
                            sw.WriteLine("{0} 0 obj", annot.AA[0].Ref);
                            sw.WriteLine("<<");
                            sw.WriteLine("/Next {0} 0 R", annot.AA[1].Ref);
                            sw.WriteLine("/S /Hide");
                            sw.WriteLine("/T ({0})", fieldName + "None");
                            sw.WriteLine(">>");
                            sw.WriteLine("endobj");
                            sw.WriteLine("");

                            AddXref(annot.AA[1].Ref);
                            sw.WriteLine("{0} 0 obj", annot.AA[1].Ref);
                            sw.WriteLine("<<");
                            sw.WriteLine("/H false");
                            sw.WriteLine("/S /Hide");
                            sw.WriteLine("/T ({0})", fieldName + (checkbox.CheckStyleForTrue == StiCheckStyle.None ? "No" : "Yes"));
                            sw.WriteLine(">>");
                            sw.WriteLine("endobj");
                            sw.WriteLine("");
                            #endregion
                        }
                    }

                    for (int index = 0; index < unsignedSignaturesCounter; index++)
                    {
                        StiEditableObject seo = unsignedSignaturesArray[index];

                        #region Make field name
                        string fieldName = string.Empty;
                        if (StiOptions.Export.Pdf.UseEditableFieldName) fieldName = seo.Component.Name;
                        if (StiOptions.Export.Pdf.UseEditableFieldAlias) fieldName = seo.Component.Alias;
                        if (StiOptions.Export.Pdf.UseEditableFieldTag) fieldName = seo.Component.TagValue as string;
                        if (string.IsNullOrEmpty(fieldName)) fieldName = string.Format("UnsignedSignature{0}", index);
                        if (fieldsNames.ContainsKey(fieldName))
                        {
                            int indexName = 2;
                            string nameAdd = string.Empty;
                            while (fieldsNames.ContainsKey(fieldName + nameAdd))
                            {
                                nameAdd = "_" + indexName.ToString();
                                indexName++;
                            }
                            fieldName = fieldName + nameAdd;
                        }
                        fieldsNames.Add(fieldName, fieldName);
                        #endregion

                        #region Annot object
                        int indexOffset3 = info.AcroForm.UnsignedSignatures[index].Ref;
                        AddXref(indexOffset3);
                        sw.WriteLine("{0} 0 obj", indexOffset3);
                        sw.WriteLine("<<");
                        sw.WriteLine("/Type /Annot");
                        sw.WriteLine("/Subtype /Widget");
                        sw.WriteLine("/Rect [{0} {1} {2} {3}]",
                            ConvertToString(seo.X),
                            ConvertToString(seo.Y),
                            ConvertToString(seo.X + seo.Width),
                            ConvertToString(seo.Y + seo.Height));
                        sw.WriteLine("/F 4");
                        sw.WriteLine("/P {0} 0 R", info.PageList[seo.Page].Ref);
                        sw.WriteLine("/FT /Sig");
                        StoreStringLine("/T ", fieldName);
                        sw.WriteLine("/MK << >>");
                        sw.WriteLine("/AP << /N {0} 0 R >>", info.AcroForm.UnsignedSignatures[index].AP.Ref);
                        sw.WriteLine(">>");
                        sw.WriteLine("endobj");
                        sw.WriteLine("");

                        int indexOffset4 = info.AcroForm.UnsignedSignatures[index].AP.Ref;
                        AddXref(indexOffset4);
                        sw.WriteLine("{0} 0 obj", indexOffset4);
                        sw.WriteLine("<<");
                        sw.WriteLine("/Type /XObject");
                        sw.WriteLine("/Subtype /Form");
                        sw.WriteLine("/BBox [ 0 0 100 100 ]");
                        sw.WriteLine("/Resources << >>");

                        string stMask = "/Length {0}";
                        byte[] dsblank = new byte[] { 0x25, 0x20, 0x44, 0x53, 0x42, 0x6c, 0x61, 0x6e, 0x6b, 0x0a }; //% DSBlank
                        StoreMemoryStream2(dsblank, stMask);

                        sw.WriteLine("");
                        sw.WriteLine("endstream");
                        sw.WriteLine("endobj");
                        sw.WriteLine("");
                        #endregion
                    }
                }
            }
        }
        #endregion

        #region RenderSignatureTable
        private void RenderSignatureTable(StiReport report)
        {
            if (haveDigitalSignature == true)
            {
                string signedBy = "Stimulsoft Reports.Net";
                if (string.IsNullOrEmpty(digitalSignatureSignedBy))
                {
                    signedBy = pdfSecurity.MakeSignedByString(signedBy);
                }
                else
                {
                    signedBy = pdfSecurity.MakeSignedByString(digitalSignatureSignedBy, false);
                }

                AddXref(info.AcroForm.Signatures[0].Ref);
                sw.WriteLine("{0} 0 obj", info.AcroForm.Signatures[0].Ref);
                sw.WriteLine("<<");
                sw.WriteLine("/Rect [{0} {1} {2} {3}]",
                    ConvertToString(signaturePlacement.Left),
                    ConvertToString(signaturePlacement.Bottom),
                    ConvertToString(signaturePlacement.Right),
                    ConvertToString(signaturePlacement.Top));
                sw.WriteLine("/Type /Annot");
                sw.WriteLine("/Subtype /Widget");
                sw.WriteLine("/F 132");
                sw.WriteLine("/P {0} 0 R", info.PageList[signaturePageNumber].Ref);
                StoreStringLine("/T ", "Signature1");
                sw.WriteLine("/V {0} 0 R", info.AcroForm.Signatures[0].AP.Ref);
                StoreStringLine("/DA ", "0 g");
                sw.WriteLine("/FT /Sig");
                sw.WriteLine("/MK<<>>");
                sw.WriteLine(">>");
                sw.WriteLine("endobj");
                sw.WriteLine("");

                AddXref(info.AcroForm.Signatures[0].AP.Ref);
                sw.WriteLine("{0} 0 obj", info.AcroForm.Signatures[0].AP.Ref);
                sw.WriteLine("<<");
                sw.WriteLine("/Type /Sig");
                sw.Write("/Filter /");
                sw.Flush();
                offsetSignatureFilter = (int)(sw.BaseStream.Position);
                sw.WriteLine("Adobe.PPKLite          ");
                sw.WriteLine("/SubFilter /adbe.pkcs7.detached");

                //StoreStringLine("/Name ", signedBy);
                sw.Write("/Name ");
                sw.Flush();
                offsetSignatureName = (int)(sw.BaseStream.Position);
                sw.WriteLine(signedBy);

                sw.Write("/Contents<{0}>", new string('0', signatureDataLen));
                sw.Flush();
                offsetSignatureData = (int)(sw.BaseStream.Position - signatureDataLen - 2);
                sw.WriteLine("");
                StoreStringLine("/M ", currentDateTime);
                if (!string.IsNullOrEmpty(digitalSignatureReason))
                {
                    StoreStringLine("/Reason ", digitalSignatureReason);
                }
                if (!string.IsNullOrEmpty(digitalSignatureLocation))
                {
                    StoreStringLine("/Location ", digitalSignatureLocation);
                }
                if (!string.IsNullOrEmpty(digitalSignatureContactInfo))
                {
                    StoreStringLine("/ContactInfo ", digitalSignatureContactInfo);
                }
                sw.Write("/ByteRange [0 {0} {1} ",
                    offsetSignatureData,
                    offsetSignatureData + signatureDataLen + 2);
                sw.Flush();
                offsetSignatureLen2 = (int)(sw.BaseStream.Position);
                sw.WriteLine("0     ]");
                sw.WriteLine("/Prop_Build");
                sw.WriteLine("<<");

                sw.WriteLine("/App<<");
                string creatorString = GetCreatorString();
                int posFrom = creatorString.ToLowerInvariant().IndexOf(" from ");
                if (posFrom != -1) creatorString = creatorString.Substring(0, posFrom);
                sw.WriteLine("/Name/{0}", CorrectNameEnconding(creatorString));
                sw.WriteLine(">>");

                sw.WriteLine(">>");
                sw.WriteLine(">>");
                sw.WriteLine("endobj");
                sw.WriteLine("");
            }
        }
        #endregion

        #region RenderTooltipTable
        private void RenderTooltipTable()
        {
            if (haveTooltips == true)
            {
                for (int index = 0; index < tooltipsCounter; index++)
                {
                    StiLinkObject stl = (StiLinkObject)tooltipsArray[index];

                    //find matches hyperlink if one exists
                    bool needHyperlink = false;
                    StiLinkObject stlink = new StiLinkObject();
                    for (int indexLink = 0; indexLink < linksCounter; indexLink++)
                    {
                        stlink = (StiLinkObject)linksArray[indexLink];
                        if (stlink.X == stl.X &&
                            stlink.Y == stl.Y &&
                            stlink.Width == stl.Width &&
                            stlink.Height == stl.Height &&
                            stlink.Page - 1 == stl.Page)
                        {
                            needHyperlink = true;
                            break;
                        }
                    }

                    // /Type /Annot
                    // /Subtype /Widget
                    // /Rect [ 10.2443 718.772 84.9895 784.032 ]
                    // /TU (It's tooltip for this text)
                    // /F 4
                    // /P 5 0 R
                    // /T (Check Box1)
                    // /FT /Btn
                    // /Ff 65536
                    // /H /N
                    //// /MK <<
                    //// /BG [ 1 ]
                    //// /BC [ 0 ]
                    //// >>
                    // /AP <<
                    //// /N 20 0 R
                    // >>

                    AddXref(info.AcroForm.Tooltips[index].Ref);
                    sw.WriteLine("{0} 0 obj", info.AcroForm.Tooltips[index].Ref);
                    sw.WriteLine("<<");
                    sw.WriteLine("/Type /Annot");
                    sw.WriteLine("/Subtype /Widget");
                    sw.WriteLine("/Rect [{0} {1} {2} {3}]",
                        ConvertToString(stl.X),
                        ConvertToString(stl.Y),
                        ConvertToString(stl.X + stl.Width),
                        ConvertToString(stl.Y + stl.Height));
                    StoreStringLine("/TU ", stl.Link, true);
                    sw.WriteLine("/F 0");
                    sw.WriteLine("/P {0} 0 R", info.PageList[stl.Page].Ref);
                    StoreStringLine("/T ", string.Format("Tooltip{0}", index));
                    sw.WriteLine("/FT /Btn");
                    sw.WriteLine("/Ff 65536");
                    sw.WriteLine("/H /N");
                    //sw.WriteLine("/MK <<");
                    //sw.WriteLine("/BG [ 1 ]");
                    //sw.WriteLine("/BC [ 0 ]");
                    //sw.WriteLine(">>");
                    //sw.WriteLine("/AP << /N {0} 0 R >>", xannotOffset + annotsCounter + index + 1);
                    sw.WriteLine("/AP <<");
                    sw.WriteLine(">>");
                    //sw.WriteLine("/N {0} 0 R", indexOffset + tooltipsCounter + index);
                    if (needHyperlink)
                    {
                        #region Hyperlink info
                        if (stlink.Link[0] == '#')
                        {
                            if (stlink.DestY > -1) sw.WriteLine("/Dest [{0} 0 R /XYZ null {1} null]", info.PageList[stlink.DestPage].Ref, ConvertToString(stlink.DestY));
                            else sw.WriteLine("/Dest [{0} 0 R /XYZ null null null]", info.PageList[0].Ref);
                        }
                        else
                        {
                            sw.WriteLine("/A <<");
                            sw.WriteLine("/Type /Action");
                            sw.WriteLine("/S /URI");
                            //StoreStringLine("/URI ", ConvertToEscapeSequence(stlink.Link.Replace('\\', '/')));
                            StoreStringLine("/URI ", StiExportUtils.StringToUrl(stlink.Link.Replace('\\', '/')));
                            sw.WriteLine(">>");
                        }
                        #endregion
                    }
                    sw.WriteLine(">>");
                    sw.WriteLine("endobj");
                    sw.WriteLine("");
                }
            }
        }
        #endregion

        #region RenderEncodeRecord
        private void RenderEncodeRecord()
        {
            if (encrypted)
            {
                AddXref(info.Encode.Ref);
                sw.WriteLine("{0} 0 obj", info.Encode.Ref);
                sw.WriteLine("<<");

                pdfSecurity.RenderEncodeRecord(sw);

                sw.WriteLine(">>");
                sw.WriteLine("endobj");
                sw.WriteLine("");
            }
        }
        #endregion

        #region RenderExtGStateRecord
        private void RenderExtGStateRecord()
        {
            AddXref(info.ExtGState.Ref);
            sw.WriteLine("{0} 0 obj", info.ExtGState.Ref);
            sw.WriteLine("<<");

            for (int index = 0; index < 256; index++)
            {
                if (alphaTable[index])
                {
                    string stNum = string.Format("{0:X2}", index);
                    sw.WriteLine("/GS{0}S <</Type /ExtGState /BM /Normal /CA {1}>>", stNum, colorTable[index]);
                    sw.WriteLine("/GS{0}N <</Type /ExtGState /BM /Normal /ca {1}>>", stNum, colorTable[index]);
                }
            }

            sw.WriteLine(">>");
            sw.WriteLine("endobj");
            sw.WriteLine("");
        }
        #endregion

        #region RenderBorder
        private void RenderBorder1(StiPdfData pp)
        {
            StiBrush brush = null;

            IStiBrush mBrush = pp.Component as IStiBrush;
            if ((mBrush != null) && (mBrush.Brush != null))
            {
                brush = mBrush.Brush;
            }
            StiRichText mRich = pp.Component as StiRichText;
            if (mRich != null)
            {
                brush = new StiSolidBrush(mRich.BackColor);
            }

            if (brush != null)
            {
                FillRectBrush(brush, new RectangleD(pp.X, pp.Y, pp.Width, pp.Height), (pp.Component as IStiCornerRadius)?.CornerRadius);
            }
        }

        private void RenderBorder2(StiPdfData pp)
        {
            //draw border
            IStiBorder mBorder = pp.Component as IStiBorder;
            if (mBorder != null)
            {
                var cornerRadius = (pp.Component as IStiCornerRadius)?.CornerRadius;
                if (cornerRadius != null)
                {
                    RenderBorderWithCorners2(pp, new StiCornerRadius(cornerRadius.BottomLeft, cornerRadius.BottomRight, cornerRadius.TopRight, cornerRadius.TopLeft));
                    return;
                }

                #region draw shadow
                if ((mBorder.Border.DropShadow) && (mBorder.Border.ShadowBrush != null))
                {
                    var tempColor = StiBrush.ToColor(mBorder.Border.ShadowBrush);
                    if (tempColor.A != 0)
                    {
                        double shadowSize = mBorder.Border.ShadowSize * 0.8;
                        //fill color
                        SetNonStrokeColor(tempColor);
                        pageStream.WriteLine("{0} {1} {2} {3} re f",
                            ConvertToString(pp.X + shadowSize),
                            ConvertToString(pp.Y - shadowSize),
                            ConvertToString(pp.Width - shadowSize),
                            ConvertToString(shadowSize));
                        pageStream.WriteLine("{0} {1} {2} {3} re f",
                            ConvertToString(pp.X + pp.Width),
                            ConvertToString(pp.Y - shadowSize),
                            ConvertToString(shadowSize),
                            ConvertToString(pp.Height));
                    }
                }
                #endregion

                StiBorderSide border = new StiBorderSide(mBorder.Border.Color, mBorder.Border.Size, mBorder.Border.Style);
                StiAdvancedBorder advBorder = mBorder.Border as StiAdvancedBorder;
                bool useAdvBorder = advBorder != null;

                bool needBorderLeft = mBorder.Border.IsLeftBorderSidePresent;
                bool needBorderRight = mBorder.Border.IsRightBorderSidePresent;
                bool needBorderTop = mBorder.Border.IsTopBorderSidePresent;
                bool needBorderBottom = mBorder.Border.IsBottomBorderSidePresent;
                bool needPush = (mBorder.Border.Style != StiPenStyle.None) && (mBorder.Border.Style != StiPenStyle.Solid);
                bool needDraw = (mBorder.Border.Side != StiBorderSides.None) && (mBorder.Border.Style != StiPenStyle.None);
                if (useAdvBorder)
                {
                    needDraw = advBorder.Side != StiBorderSides.None;
                }

                if (needDraw)
                {
                    double offset = 0;
                    if (!useAdvBorder)
                    {
                        if (needPush)
                        {
                            pageStream.WriteLine("q");
                            PushColorToStack();
                        }
                        offset = StoreBorderSideData(border);
                    }

                    if ((!useAdvBorder) && (mBorder.Border.Side == StiBorderSides.All))
                    {
                        #region stroke border
                        pageStream.WriteLine("{0} {1} {2} {3} re S",
                            ConvertToString(pp.X - offset),
                            ConvertToString(pp.Y - offset),
                            ConvertToString(pp.Width + offset * 2),
                            ConvertToString(pp.Height + offset * 2));
                        if (mBorder.Border.Style == StiPenStyle.Double)
                        {
                            pageStream.WriteLine("{0} {1} {2} {3} re S",
                                ConvertToString(pp.X + offset),
                                ConvertToString(pp.Y + offset),
                                ConvertToString(pp.Width - offset * 2),
                                ConvertToString(pp.Height - offset * 2));
                        }
                        #endregion
                    }
                    else
                    {
                        #region paint border by line
                        if (needBorderLeft)
                        {
                            if (useAdvBorder)
                            {
                                border = advBorder.LeftSide;
                                if (border.Style != StiPenStyle.Solid)
                                {
                                    pageStream.WriteLine("q");
                                    PushColorToStack();
                                }
                                offset = StoreBorderSideData(border);
                            }
                            pageStream.WriteLine("{0} {1} m", ConvertToString(pp.X - offset), ConvertToString(pp.Y - offset));
                            pageStream.WriteLine("{0} {1} l S", ConvertToString(pp.X - offset), ConvertToString(pp.Y + pp.Height + offset));
                            if (border.Style == StiPenStyle.Double)
                            {
                                pageStream.WriteLine("{0} {1} m",
                                    ConvertToString(pp.X + offset),
                                    ConvertToString(pp.Y + (needBorderBottom ? offset : -offset)));
                                pageStream.WriteLine("{0} {1} l S",
                                    ConvertToString(pp.X + offset),
                                    ConvertToString(pp.Y + pp.Height + (needBorderTop ? -offset : offset)));
                            }
                            if (useAdvBorder && (border.Style != StiPenStyle.Solid))
                            {
                                pageStream.WriteLine("Q");
                                PopColorFromStack();
                            }
                        }
                        if (needBorderRight)
                        {
                            if (useAdvBorder)
                            {
                                border = advBorder.RightSide;
                                if (border.Style != StiPenStyle.Solid)
                                {
                                    pageStream.WriteLine("q");
                                    PushColorToStack();
                                }
                                offset = StoreBorderSideData(border);
                            }
                            pageStream.WriteLine("{0} {1} m", ConvertToString(pp.X + pp.Width + offset), ConvertToString(pp.Y - offset));
                            pageStream.WriteLine("{0} {1} l S", ConvertToString(pp.X + pp.Width + offset), ConvertToString(pp.Y + pp.Height + offset));
                            if (border.Style == StiPenStyle.Double)
                            {
                                pageStream.WriteLine("{0} {1} m",
                                    ConvertToString(pp.X + pp.Width - offset),
                                    ConvertToString(pp.Y + (needBorderBottom ? offset : -offset)));
                                pageStream.WriteLine("{0} {1} l S",
                                    ConvertToString(pp.X + pp.Width - offset),
                                    ConvertToString(pp.Y + pp.Height + (needBorderTop ? -offset : offset)));
                            }
                            if (useAdvBorder && (border.Style != StiPenStyle.Solid))
                            {
                                pageStream.WriteLine("Q");
                                PopColorFromStack();
                            }
                        }
                        if (needBorderTop)
                        {
                            if (useAdvBorder)
                            {
                                border = advBorder.TopSide;
                                if (border.Style != StiPenStyle.Solid)
                                {
                                    pageStream.WriteLine("q");
                                    PushColorToStack();
                                }
                                offset = StoreBorderSideData(border);
                            }
                            pageStream.WriteLine("{0} {1} m", ConvertToString(pp.X - offset), ConvertToString(pp.Y + pp.Height + offset));
                            pageStream.WriteLine("{0} {1} l S", ConvertToString(pp.X + pp.Width + offset), ConvertToString(pp.Y + pp.Height + offset));
                            if (border.Style == StiPenStyle.Double)
                            {
                                pageStream.WriteLine("{0} {1} m",
                                    ConvertToString(pp.X + (needBorderLeft ? offset : -offset)),
                                    ConvertToString(pp.Y + pp.Height - offset));
                                pageStream.WriteLine("{0} {1} l S",
                                    ConvertToString(pp.X + pp.Width + (needBorderRight ? -offset : offset)),
                                    ConvertToString(pp.Y + pp.Height - offset));
                            }
                            if (useAdvBorder && (border.Style != StiPenStyle.Solid))
                            {
                                pageStream.WriteLine("Q");
                                PopColorFromStack();
                            }
                        }
                        if (needBorderBottom)
                        {
                            if (useAdvBorder)
                            {
                                border = advBorder.BottomSide;
                                if (border.Style != StiPenStyle.Solid)
                                {
                                    pageStream.WriteLine("q");
                                    PushColorToStack();
                                }
                                offset = StoreBorderSideData(border);
                            }
                            pageStream.WriteLine("{0} {1} m", ConvertToString(pp.X - offset), ConvertToString(pp.Y - offset));
                            pageStream.WriteLine("{0} {1} l S", ConvertToString(pp.X + pp.Width + offset), ConvertToString(pp.Y - offset));
                            if (border.Style == StiPenStyle.Double)
                            {
                                pageStream.WriteLine("{0} {1} m",
                                    ConvertToString(pp.X + (needBorderLeft ? offset : -offset)),
                                    ConvertToString(pp.Y + offset));
                                pageStream.WriteLine("{0} {1} l S",
                                    ConvertToString(pp.X + pp.Width + (needBorderRight ? -offset : offset)),
                                    ConvertToString(pp.Y + offset));
                            }
                            if (useAdvBorder && (border.Style != StiPenStyle.Solid))
                            {
                                pageStream.WriteLine("Q");
                                PopColorFromStack();
                            }
                        }
                        #endregion
                    }

                    if (!useAdvBorder && needPush)
                    {
                        pageStream.WriteLine("Q");
                        PopColorFromStack();
                    }
                }
            }
        }

        private void RenderBorderWithCorners2(StiPdfData pp, StiCornerRadius cornerRadius)
        {
            //draw border
            IStiBorder mBorder = pp.Component as IStiBorder;
            if (mBorder != null)
            {
                tempGeomWriter.pageStream = pageStream;

                #region draw shadow
                if ((mBorder.Border.DropShadow) && (mBorder.Border.ShadowBrush != null))
                {
                    var tempColor = StiBrush.ToColor(mBorder.Border.ShadowBrush);
                    if (tempColor.A != 0)
                    {
                        double shadowSize = mBorder.Border.ShadowSize * 0.8;
                        //fill color
                        SetNonStrokeColor(tempColor);
                        pageStream.WriteLine("{0} {1} {2} {3} re f",
                            ConvertToString(pp.X + shadowSize),
                            ConvertToString(pp.Y - shadowSize),
                            ConvertToString(pp.Width - shadowSize),
                            ConvertToString(shadowSize));
                        pageStream.WriteLine("{0} {1} {2} {3} re f",
                            ConvertToString(pp.X + pp.Width),
                            ConvertToString(pp.Y - shadowSize),
                            ConvertToString(shadowSize),
                            ConvertToString(pp.Height));
                    }
                }
                #endregion

                bool needDraw = (mBorder.Border.Side != StiBorderSides.None) && (mBorder.Border.Style != StiPenStyle.None);
                if (needDraw)
                {
                    bool needPush = mBorder.Border.Style != StiPenStyle.Solid;
                    if (needPush)
                    {
                        pageStream.WriteLine("q");
                        PushColorToStack();
                    }

                    StiBorderSide border = new StiBorderSide(mBorder.Border.Color, mBorder.Border.Size, mBorder.Border.Style);
                    double doubleOffset = StoreBorderSideData(border);

                    WriteBorderWithCorners2(pp, new RectangleD(pp.X, pp.Y, pp.Width, pp.Height).ToRectangleF(), cornerRadius);
                    if (mBorder.Border.Style == StiPenStyle.Double)
                    {
                        float cornerOffset = (float)(doubleOffset * 2);
                        WriteBorderWithCorners2(pp,
                            new RectangleD(pp.X + doubleOffset * 2, pp.Y + doubleOffset * 2, pp.Width - doubleOffset * 4, pp.Height - doubleOffset * 4).ToRectangleF(),
                            new StiCornerRadius(cornerRadius.TopLeft - cornerOffset, cornerRadius.TopRight - cornerOffset, cornerRadius.BottomRight - cornerOffset, cornerRadius.BottomLeft - cornerOffset));
                    }

                    if (needPush)
                    {
                        pageStream.WriteLine("Q");
                        PopColorFromStack();
                    }
                }
            }
        }

        private void WriteBorderWithCorners2(StiPdfData pp, RectangleF rect, StiCornerRadius cornerRadius)
        {
            IStiBorder mBorder = pp.Component as IStiBorder;
            if (mBorder != null)
            {
                bool needBorderLeft = mBorder.Border.IsLeftBorderSidePresent;
                bool needBorderRight = mBorder.Border.IsRightBorderSidePresent;
                bool needBorderTop = mBorder.Border.IsTopBorderSidePresent;
                bool needBorderBottom = mBorder.Border.IsBottomBorderSidePresent;

                if (mBorder.Border.Side == StiBorderSides.All)
                {
                    pageStream.WriteLine(tempGeomWriter.GetRectWithCornersString(rect, cornerRadius) + "S");
                }
                else
                {
                    #region paint border by line
                    float offsetLT = cornerRadius.TopLeft * (float)hiToTwips;
                    float offsetRT = cornerRadius.TopRight * (float)hiToTwips;
                    float offsetRB = cornerRadius.BottomRight * (float)hiToTwips;
                    float offsetLB = cornerRadius.BottomLeft * (float)hiToTwips;
                    tempGeomWriter.BeginPath();
                    tempGeomWriter.forceNewPoint = true;
                    if (needBorderLeft)
                    {
                        tempGeomWriter.MoveTo(new PointF(rect.X, rect.Y + offsetLB));
                        tempGeomWriter.DrawLineTo(new PointF(rect.X, rect.Y + rect.Height - offsetLT), null);
                    }
                    if (needBorderTop)
                    {
                        if (needBorderLeft && offsetLT > 0)
                        {
                            tempGeomWriter.DrawArc(new RectangleF(rect.X, rect.Y + rect.Height - offsetLT * 2, offsetLT * 2, offsetLT * 2), 180, -90);
                        }
                        tempGeomWriter.MoveTo(new PointF(rect.X + offsetLT, rect.Y + rect.Height));
                        tempGeomWriter.DrawLineTo(new PointF(rect.X + rect.Width - offsetRT, rect.Y + rect.Height), null);
                        if (needBorderRight && offsetRT > 0)
                        {
                            tempGeomWriter.DrawArc(new RectangleF(rect.X + rect.Width - offsetRT * 2, rect.Y + rect.Height - offsetRT * 2, offsetRT * 2, offsetRT * 2), 90, -90);
                        }
                    }
                    if (needBorderRight)
                    {
                        tempGeomWriter.MoveTo(new PointF(rect.X + rect.Width, rect.Y + rect.Height - offsetRT));
                        tempGeomWriter.DrawLineTo(new PointF(rect.X + rect.Width, rect.Y + offsetRB), null);
                    }
                    if (needBorderBottom)
                    {
                        if (needBorderRight && offsetRB > 0)
                        {
                            tempGeomWriter.DrawArc(new RectangleF(rect.X + rect.Width - offsetRB * 2, rect.Y, offsetRB * 2, offsetRB * 2), 0, -90);
                        }
                        tempGeomWriter.MoveTo(new PointF(rect.X + rect.Width - offsetRB, rect.Y));
                        tempGeomWriter.DrawLineTo(new PointF(rect.X + offsetLB, rect.Y), null);
                        if (needBorderLeft && offsetLB > 0)
                        {
                            tempGeomWriter.DrawArc(new RectangleF(rect.X, rect.Y, offsetLB * 2, offsetLB * 2), 270, -90);
                        }
                    }
                    tempGeomWriter.EndPath();
                    pageStream.WriteLine(tempGeomWriter.path.ToString() + "S");
                    #endregion
                }
            }
        }

        private double StoreBorderSideData(StiBorderSide border)
        {
            //set border size
            double borderSizeHi = border.Size;
            if (borderSizeHi < 0.5) borderSizeHi = 0.5;
            if (border.Style == StiPenStyle.Double) borderSizeHi = 1;
            double borderSize = borderSizeHi * hiToTwips * 0.955;
            pageStream.WriteLine("{0} w", ConvertToString(borderSize));

            //set border style
            string dash = GetPenStyleDashString(border.Style, borderSize * 0.04);
            if (dash != null)
            {
                pageStream.WriteLine(dash);
            }

            //stroke color
            Color tempColor2 = border.Color;
            SetStrokeColor(tempColor2);

            return (border.Style == StiPenStyle.Double ? borderSize : 0);
        }

        private string GetPenStyleDashString(StiPenStyle style, double step)
        {
            switch (style)
            {
                case StiPenStyle.Dot:
                    return string.Format("[{0} {1}] 0 d", ConvertToString(step), ConvertToString(step * 58));

                case StiPenStyle.Dash:
                    return string.Format("[{0} {1}] 0 d", ConvertToString(step * 49.5), ConvertToString(step * 62));

                case StiPenStyle.DashDot:
                    return string.Format("[{0} {1} {2} {1}] 0 d", ConvertToString(step * 50), ConvertToString(step * 55), ConvertToString(step));

                case StiPenStyle.DashDotDot:
                    return string.Format("[{0} {1} {2} {1} {2} {1}] 0 d", ConvertToString(step * 50), ConvertToString(step * 55), ConvertToString(step));
            }
            return null;
        }
        #endregion

        #region StoreImageData, WriteImageInfo
        private float StoreWatermarkMetafileData(Image image, float imageResolution, StiPage page)
        {
            double pageWidth = report.Unit.ConvertToHInches(page.PageWidth * page.SegmentPerWidth);
            double pageHeight = report.Unit.ConvertToHInches(page.PageHeight * page.SegmentPerHeight);

            int newWidth = (int)Math.Round(pageWidth * imageResolution);
            int newHeight = (int)Math.Round(pageHeight * imageResolution);
            using (Bitmap newBmp = new Bitmap(newWidth, newHeight))
            {
                using (Graphics g = Graphics.FromImage(newBmp))
                {
                    if (useTransparency)
                        g.Clear(Color.FromArgb(1, 255, 255, 255));
                    else
                        g.Clear(Color.White);

                    var destRect = new RectangleF(0, 0, newWidth, newHeight);
                    if (destRect.Width > 0 && destRect.Height > 0)
                        g.DrawImage(image, destRect);
                }
                StoreImageData(newBmp, imageResolution, false, false);
            }
            return imageResolution;
        }

        internal float StoreImageData(Image image, float imageResolution, bool isImageComponent, bool needSmoothing, bool maxQuality = false)
        {
            #region Check resolution for Image component
            if (isImageComponent && ((imageResolutionMode == StiImageResolutionMode.NoMoreThan) && (imageResolution > imageResolutionMain)))
            {
                int newWidth = (int)Math.Round(image.Width * imageResolutionMain / imageResolution);
                int newHeight = (int)Math.Round(image.Height * imageResolutionMain / imageResolution);
                Bitmap newBmp = new Bitmap(newWidth, newHeight);
                Graphics g = Graphics.FromImage(newBmp);
                if (!useTransparency) g.Clear(Color.White);
                g.DrawImage(image, new Rectangle(0, 0, newWidth, newHeight), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);
                g.Dispose();
                image = newBmp;
                imageResolution = imageResolutionMain;

                imageCache.UsedMemoryCounter += newBmp.Width * newBmp.Height * 4;
            }
            #endregion

            #region Remove transparency for PDF/A mode
            if (isImageComponent && !useTransparency && ((imageResolutionMode == StiImageResolutionMode.Auto) || ((imageResolutionMode == StiImageResolutionMode.NoMoreThan) && (imageResolution <= imageResolutionMain))))
            {
                int newWidth = image.Width;
                int newHeight = image.Height;
                Bitmap newBmp = new Bitmap(newWidth, newHeight);
                Graphics g = Graphics.FromImage(newBmp);
                g.Clear(Color.White);
                g.DrawImage(image, new Rectangle(0, 0, newWidth, newHeight), 0, 0, newWidth, newHeight, GraphicsUnit.Pixel);
                g.Dispose();
                image = newBmp;

                imageCache.UsedMemoryCounter += newBmp.Width * newBmp.Height * 4;
            }
            #endregion

            #region Gray colors or monochrome
            if (imageFormat != StiImageFormat.Color)
            {
                Bitmap newBmp = new Bitmap(image.Width, image.Height);
                Graphics g = Graphics.FromImage(newBmp);

                ColorMatrix matrix = new ColorMatrix(new float[][]{
                                                                        new float[]{0.3f, 0.3f, 0.3f, 0, 0},
                                                                        new float[]{0.55f, 0.55f, 0.55f, 0, 0},
                                                                        new float[]{0.15f, 0.15f, 0.15f, 0, 0},
                                                                        new float[]{0, 0, 0, 1, 0, 0},
                                                                        new float[]{0, 0, 0, 0, 1, 0},
                                                                        new float[]{0, 0, 0, 0, 0, 1}});

                ImageAttributes attributes = new ImageAttributes();
                attributes.SetColorMatrix(matrix);
                g.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
                g.Dispose();

                //image.Dispose();
                image = newBmp;

                imageCache.UsedMemoryCounter += newBmp.Width * newBmp.Height * 4;
            }
            if (imageFormat == StiImageFormat.Monochrome)
            {
                image = StiTiffHelper.MakeMonochromeImage(image, monochromeDitheringType, 128);

                imageCache.UsedMemoryCounter += image.Width * image.Height * 4;
            }
            #endregion

            int imageIndex = maxQuality ? imageCache.AddImageInt(image, 1) : imageCache.AddImageInt(image);

            StiImageData pd = new StiImageData();
            pd.Width = image.Width;
            pd.Height = image.Height;
            pd.Name = string.Format("Image{0}", imageIndex);
            pd.ImageFormat = imageFormat;
            if ((image.PixelFormat == PixelFormat.Format1bppIndexed) && (imageCompressionMethod == StiPdfImageCompressionMethod.Flate || imageCompressionMethod == StiPdfImageCompressionMethod.Indexed))
            {
                pd.ImageFormat = StiImageFormat.Monochrome;
            }

            imageList.Add(pd);
            imageCacheIndexToList[imageIndex] = pd;

            if (needSmoothing)
            {
                imageInterpolationTable[imageIndex] = true;
            }

            return imageResolution;
        }

        internal void StoreImageDataForGeom(StiImage image)
        {
            using (var gdiImage = image.TakeGdiImageToDraw(imageResolutionMain))
            {
                if (gdiImage != null)
                {
                    float rsImageResolution = gdiImage.HorizontalResolution / 100;
                    if (imageResolutionMode == StiImageResolutionMode.NoMoreThan)
                    {
                        if (image.Stretch)
                        {
                            rsImageResolution = (float) (gdiImage.Width / report.Unit.ConvertToHInches(image.Width));
                        }
                        else
                        {
                            rsImageResolution = (float) (1 / image.MultipleFactor);
                        }
                    }
                    rsImageResolution = StoreImageData(gdiImage, rsImageResolution, true, image.Smoothing);
                    imageInfoList[imageInfoCounter] = rsImageResolution;
                }
            }
        }

        private void WriteImageInfo(StiPdfData pp, float imageResolution, bool forceResolutionModeAuto = false)
        {
            StiImageData pd = (StiImageData)imageList[imagesCurrent];
            imagesCurrent++;

            StiImage view = pp.Component as StiImage;

            var tempImageResolutionMode = imageResolutionMode;
            if (forceResolutionModeAuto) tempImageResolutionMode = StiImageResolutionMode.Auto;

            if ((tempImageResolutionMode != StiImageResolutionMode.Exactly) && (view != null) && !view.ImageToDrawIsMetafile() && !view.Margins.IsEmpty)
            {
                double marginsLeft = hiToTwips * view.Margins.Left;
                double marginsRight = hiToTwips * view.Margins.Right;
                double marginsTop = hiToTwips * view.Margins.Top;
                double marginsBottom = hiToTwips * view.Margins.Bottom;

                if (marginsLeft != 0)
                {
                    pp.X += marginsLeft;
                    pp.Width -= marginsLeft;
                }
                if (marginsBottom != 0)
                {
                    pp.Y += marginsBottom;
                    pp.Height -= marginsBottom;
                }
                if (marginsRight != 0) pp.Width -= marginsRight;
                if (marginsTop != 0) pp.Height -= marginsTop;
            }

            //values for fixed resolution
            double cx = pp.X;
            double cy = pp.Y;
            double cw = hiToTwips * (pd.Width - 1) / imageResolution;
            double ch = hiToTwips * (pd.Height - 1) / imageResolution;
            bool needClip = false;

            #region Check for StiImage
            if ((tempImageResolutionMode != StiImageResolutionMode.Exactly) && (view != null) && view.ExistImageToDraw() && !view.ImageToDrawIsMetafile())
            {
                using (var gdiImage = view.TakeGdiImageToDraw(imageResolutionMain))
                {
                    if (gdiImage != null)
                    {
                        int gdiImageWidth = gdiImage.Width;
                        int gdiImageHeight = gdiImage.Height;
                        if (view.ImageRotation == StiImageRotation.Rotate90CCW || view.ImageRotation == StiImageRotation.Rotate90CW)
                        {
                            gdiImageWidth = gdiImage.Height;
                            gdiImageHeight = gdiImage.Width;
                        }

                        var rect = view.GetPaintRectangle(true, false);
                        rect = view.ConvertImageMargins(rect, false);
                        var destRect = new RectangleF(0, 0, (float) rect.Width, (float) rect.Height);

                        bool isStretch = view.Stretch || (view.Icon != null);

                        #region !Stretch
                        if (!isStretch)
                        {
                            float imageWidth = (float) (gdiImageWidth * view.MultipleFactor);
                            float imageHeight = (float) (gdiImageHeight * view.MultipleFactor);

                            destRect.Width = imageWidth;
                            destRect.Height = imageHeight;

                            #region HorAlignment
                            switch (view.HorAlignment)
                            {
                                case StiHorAlignment.Center:
                                    destRect.X = (float) ((rect.Width - imageWidth) / 2);
                                    break;

                                case StiHorAlignment.Right:
                                    destRect.X = (float) (rect.Width - imageWidth);
                                    break;
                            }
                            #endregion

                            #region VertAlignment
                            switch (view.VertAlignment)
                            {
                                case StiVertAlignment.Center:
                                    destRect.Y = (float) ((rect.Height - imageHeight) / 2);
                                    break;

                                case StiVertAlignment.Top:
                                    destRect.Y = (float) (rect.Height - imageHeight);
                                    break;
                            }
                            #endregion

                            cx += destRect.X * hiToTwips;
                            cy += destRect.Y * hiToTwips;
                            cw = destRect.Width * hiToTwips;
                            ch = destRect.Height * hiToTwips;

                            needClip = true;
                        }
                        #endregion

                        #region Stretch
                        else
                        {
                            float imageWidth = gdiImageWidth;
                            float imageHeight = gdiImageHeight;

                            #region AspectRatio
                            if (view.AspectRatio)
                            {
                                float xRatio = destRect.Width / imageWidth;
                                float yRatio = destRect.Height / imageHeight;

                                if (xRatio > yRatio) destRect.Width = imageWidth * yRatio;
                                else destRect.Height = imageHeight * xRatio;

                                #region VertAlignment
                                switch (view.VertAlignment)
                                {
                                    case StiVertAlignment.Center:
                                        destRect.Y = (float) ((rect.Height - destRect.Height) / 2);
                                        break;

                                    case StiVertAlignment.Top:
                                        destRect.Y = (float) (rect.Height - destRect.Height);
                                        break;
                                }
                                #endregion

                                #region HorAlignment
                                switch (view.HorAlignment)
                                {
                                    case StiHorAlignment.Center:
                                        destRect.X = (float) ((rect.Width - destRect.Width) / 2);
                                        break;

                                    case StiHorAlignment.Right:
                                        destRect.X = (float) (rect.Width - destRect.Width);
                                        break;
                                }
                                #endregion

                                cx += destRect.X * hiToTwips;
                                cy += destRect.Y * hiToTwips;
                                cw = destRect.Width * hiToTwips;
                                ch = destRect.Height * hiToTwips;
                            }
                            #endregion

                            #region !AspectRatio
                            else
                            {
                                cw = pp.Width;
                                ch = pp.Height;
                            }
                            #endregion
                        }
                        #endregion
                    }
                }
            }
            #endregion

            pageStream.WriteLine("q");
            PushColorToStack();
            SetNonStrokeColor(Color.Black);
            if (needClip)
            {
                pageStream.WriteLine("{0} {1} {2} {3} re W n",
                        ConvertToString(pp.X),
                        ConvertToString(pp.Y),
                        ConvertToString(pp.Width),
                        ConvertToString(pp.Height));
            }
            pageStream.WriteLine("{0} 0 0 {1} {2} {3} cm",
                ConvertToString(cw),
                ConvertToString(ch),
                ConvertToString(cx),
                ConvertToString(cy));
            pageStream.WriteLine("/{0} Do", pd.Name);
            pageStream.WriteLine("Q");
            PopColorFromStack();
        }
        internal void WriteImageInfo2(StiPdfData pp, double imageResolutionX, double imageResolutionY)
        {
            StiImageData pd = (StiImageData)imageList[imagesCurrent];
            imagesCurrent++;

            pageStream.WriteLine("q");
            PushColorToStack();
            SetNonStrokeColor(Color.Black);
            pageStream.WriteLine("{0} 0 0 {1} {2} {3} cm",
                ConvertToString(hiToTwips * (pd.Width) / imageResolutionX),
                ConvertToString(hiToTwips * (pd.Height) / imageResolutionY),
                ConvertToString(pp.X),
                ConvertToString(pp.Y));
            pageStream.WriteLine("/{0} Do", pd.Name);
            pageStream.WriteLine("Q");
            PopColorFromStack();
        }
        #endregion

        #region RenderImage
        internal void RenderImage(StiPdfData pp, float imageResolution, bool forceResolutionModeAuto = false)
        {
            if (imageInfoList.ContainsKey(imageInfoCounter))
            {
                float rsImageResolution = (float)imageInfoList[imageInfoCounter];
                WriteImageInfo(pp, rsImageResolution, forceResolutionModeAuto);
            }
            else
            {
                IStiExportImageExtended exportImage = pp.Component as IStiExportImageExtended;
                if (exportImage != null && pp.Component.IsExportAsImage(StiExportFormat.Pdf))
                {
                    float rsImageResolution = imageResolution;
                    using (var image = exportImage.GetImage(ref rsImageResolution, StiExportFormat.Pdf))
                    {
                        if (image != null)
                        {
                            WriteImageInfo(pp, rsImageResolution, forceResolutionModeAuto);
                        }
                    }
                }
            }
        }
        #endregion

        #region RenderChart
        private void RenderChart(StiPdfData pp, bool assemble, int pageNumber)
        {
            var chart = pp.Component as StiChart;
            StiContext context = null;

            #region Calculate rotation transform
            float scale = 0.96f;
            float rectWidth = (float)(report.Unit.ConvertToHInches(pp.Component.Width) * scale);
            float rectHeight = (float)(report.Unit.ConvertToHInches(pp.Component.Height) * scale);

            float angle = 0;
            float deltaX = 0;
            float deltaY = 0;
            float scaleX = 1;
            float scaleY = 1;

            switch (chart.Rotation)
            {
                case StiImageRotation.Rotate90CCW:
                    angle = -90;
                    deltaY = rectHeight;
                    break;

                case StiImageRotation.Rotate90CW:
                    angle = 90;
                    deltaX = rectWidth;
                    break;

                case StiImageRotation.Rotate180:
                    angle = -180;
                    deltaX = rectWidth;
                    deltaY = rectHeight;
                    break;

                case StiImageRotation.FlipVertical:
                    scaleY = -1;
                    deltaY = rectHeight;
                    break;

                case StiImageRotation.FlipHorizontal:
                    scaleX = -1;
                    deltaX = rectWidth;
                    break;
            }

            switch (chart.Rotation)
            {
                case StiImageRotation.Rotate90CCW:
                case StiImageRotation.Rotate90CW:
                    var temp = rectWidth;
                    rectWidth = rectHeight;
                    rectHeight = temp;
                    break;
            }

            if (!assemble && (chart.Rotation != StiImageRotation.None))
            {
                Matrix matrix2 = new Matrix(1, 0, 0, 1, 0, 0);
                matrix2.Translate(deltaX, deltaY);
                matrix2.Scale(scaleX, scaleY);
                matrix2.Rotate(angle);

                pageStream.WriteLine("{0} {1} {2} {3} {4} {5} cm",
                ConvertToString(matrix2.Elements[0]),
                ConvertToString(matrix2.Elements[1]),
                ConvertToString(matrix2.Elements[2]),
                ConvertToString(matrix2.Elements[3]),
                ConvertToString(matrix2.Elements[4]),
                ConvertToString(matrix2.Elements[5]));
            }
            #endregion

            using (var img = new Bitmap(1, 1))
            {
                using (var g = Graphics.FromImage(img))
                {
                    var painter = new StiGdiContextPainter(g);
                    context = new StiContext(painter, true, false, false, 1f);

                    bool storeIsAnimation = chart.IsAnimation;
                    chart.IsAnimation = false;


                    var chartGeom = chart.Core.Render(
                        context,
                        new RectangleF(0, 0, rectWidth, rectHeight),
                        true);

                    chartGeom.DrawGeom(context);

                    chart.IsAnimation = storeIsAnimation;
                }
            }

            RenderGdiContext(pp, assemble, pageNumber, context, true);
        }

        private void RenderSparkline(StiPdfData pp, bool assemble, int pageNumber)
        {
            var sparkline = pp.Component as StiSparkline;
            StiContext context = null;

            float scale = 0.96f;
            float rectWidth = (float)(report.Unit.ConvertToHInches(pp.Component.Width) * scale);
            float rectHeight = (float)(report.Unit.ConvertToHInches(pp.Component.Height) * scale);

            using (var img = new Bitmap(1, 1))
            using (var g = Graphics.FromImage(img))
            {
                var painter = new StiGdiContextPainter(g);
                context = new StiContext(painter, true, false, false, 1f);

                StiSparklineGdiPainter.RenderSparkline(context, new RectangleD(0, 0, rectWidth, rectHeight), sparkline, 1f);
            }

            RenderGdiContext(pp, assemble, pageNumber, context);
        }

        private void RenderGdiContext(StiPdfData pp, bool assemble, int pageNumber, StiContext context, bool allowThinLines = false)
        {
            var matrix = new Matrix(1, 0, 0, 1, 0, 0);
            const float chartScale = (float)(hiToTwips / 0.96);
            matrix.Translate((float)(pp.X), (float)(pp.Y + pp.Height));
            matrix.Scale(1, -1);
            matrix.Scale(chartScale, chartScale);

            var pdfGeomWriter = new StiPdfGeomWriter(pageStream, this, assemble, allowThinLines);
            pdfGeomWriter.pageNumber = pageNumber;
            pdfGeomWriter.matrixCache.Push(matrix);
            pdfGeomWriter.basePoint.X = pp.X;
            pdfGeomWriter.basePoint.Y = pp.Y;

            foreach (StiGeom geom in context.Geoms)
            {
                if (geom is StiPushTranslateTransformGeom)
                {
                    pdfGeomWriter.SaveState();
                    pdfGeomWriter.TranslateTransform((geom as StiPushTranslateTransformGeom).X, (geom as StiPushTranslateTransformGeom).Y);
                }
                if (geom is StiPushRotateTransformGeom)
                {
                    pdfGeomWriter.SaveState();
                    pdfGeomWriter.RotateTransform((geom as StiPushRotateTransformGeom).Angle);
                }
                if (geom is StiPopTransformGeom)
                {
                    pdfGeomWriter.RestoreState();
                }

                if (geom is StiPushClipGeom)
                {
                    pdfGeomWriter.SaveState();
                    pdfGeomWriter.SetClip((geom as StiPushClipGeom).ClipRectangle);
                }
                if (geom is StiPushClipPathGeom)
                {
                    pdfGeomWriter.SaveState();

                    #region Draw path
                    var path = geom as StiPushClipPathGeom;
                    pdfGeomWriter.BeginPath();

                    foreach (var geom2 in path.Geoms)
                    {
                        if (geom2 is StiPieSegmentGeom)
                        {
                            var pie = geom2 as StiPieSegmentGeom;
                            pdfGeomWriter.DrawPie(pie.Rect, pie.StartAngle, pie.SweepAngle);
                        }
                        if (geom2 is StiArcSegmentGeom)
                        {
                            var arc = geom2 as StiArcSegmentGeom;
                            pdfGeomWriter.DrawArc(arc.Rect, arc.StartAngle, arc.SweepAngle);
                        }
                        if (geom2 is StiLineSegmentGeom)
                        {
                            var line = geom2 as StiLineSegmentGeom;
                            pdfGeomWriter.DrawLine(new PointF(line.X1, line.Y1), new PointF(line.X2, line.Y2), null);
                        }
                        if (geom2 is StiLinesSegmentGeom)
                        {
                            var lines = geom2 as StiLinesSegmentGeom;
                            pdfGeomWriter.DrawPolyline(lines.Points, null);
                        }
                        if (geom2 is StiCurveSegmentGeom)
                        {
                            var curve = geom2 as StiCurveSegmentGeom;
                            pdfGeomWriter.DrawSpline(curve.Points, curve.Tension, null);
                        }
                        if (geom2 is StiCloseFigureSegmentGeom)
                        {
                            pdfGeomWriter.CloseFigure();
                        }
                    }

                    pdfGeomWriter.CloseFigure();
                    pdfGeomWriter.EndPath();
                    #endregion

                    pdfGeomWriter.SetClipPath();
                }
                if (geom is StiPopClipGeom)
                {
                    pdfGeomWriter.RestoreState();
                }

                if (geom is StiPushSmothingModeToAntiAliasGeom)
                {
                    pdfGeomWriter.SaveState();
                }
                if (geom is StiPopSmothingModeGeom)
                {
                    pdfGeomWriter.RestoreState();
                }
                if (geom is StiPushTextRenderingHintToAntiAliasGeom)
                {
                    pdfGeomWriter.SaveState();
                }
                if (geom is StiPopTextRenderingHintGeom)
                {
                    pdfGeomWriter.RestoreState();
                }

                if (geom is StiBorderGeom)
                {
                    var border = geom as StiBorderGeom;
                    if (border.Background != null)
                    {
                        pdfGeomWriter.FillRectangle(RectToRectangleF(border.Rect), border.Background, border.CornerRadius);
                    }
                    if (CheckPenGeom(border.BorderPen))
                    {
                        pdfGeomWriter.DrawRectangle(RectToRectangleF(border.Rect), border.BorderPen, border.CornerRadius);
                    }
                }

                if (geom is StiLineGeom)
                {
                    var line = geom as StiLineGeom;
                    if (CheckPenGeom(line.Pen))
                    {
                        pdfGeomWriter.DrawLine(new PointF(line.X1, line.Y1), new PointF(line.X2, line.Y2), line.Pen);
                    }
                }

                if (geom is StiLinesGeom)
                {
                    var lines = geom as StiLinesGeom;
                    if (CheckPenGeom(lines.Pen))
                    {
                        pdfGeomWriter.DrawPolyline(lines.Points, lines.Pen);
                    }
                }

                if (geom is StiCurveGeom)
                {
                    var curve = geom as StiCurveGeom;
                    if (CheckPenGeom(curve.Pen))
                    {
                        pdfGeomWriter.DrawSpline(curve.Points, curve.Tension, curve.Pen);
                    }
                }

                if (geom is StiEllipseGeom)
                {
                    var ellipse = geom as StiEllipseGeom;
                    if (ellipse.Background != null)
                    {
                        pdfGeomWriter.FillEllipse(RectToRectangleF(ellipse.Rect), ellipse.Background);
                    }
                    if (CheckPenGeom(ellipse.BorderPen))
                    {
                        pdfGeomWriter.DrawEllipse(RectToRectangleF(ellipse.Rect), ellipse.BorderPen);
                    }
                }

                if (geom is StiCachedShadowGeom)
                {
                    #region Draw shadow
                    var shadow = geom as StiCachedShadowGeom;
                    var rect = shadow.Rect;
                    var sides = shadow.Sides;
                    float yTop = rect.Y + 8;
                    float xLeft = rect.X + 8;
                    float edgeOffset = ((sides & StiShadowSides.Edge) > 0) ? 4 : 0;
                    if ((sides & StiShadowSides.Top) > 0)
                    {
                        yTop = rect.Y + 4;
                    }
                    if ((sides & StiShadowSides.Left) > 0)
                    {
                        xLeft = rect.X + 4;
                    }

                    for (int index = 0; index < 3; index++)
                    {
                        List<PointF> pts = new List<PointF>();
                        if ((sides & StiShadowSides.Right) > 0)
                        {
                            pts.Add(new PointF(rect.Right, yTop));
                            if ((sides & StiShadowSides.Bottom) > 0)
                            {
                                pts.Add(new PointF(rect.Right, rect.Bottom));
                                pts.Add(new PointF(xLeft, rect.Bottom));
                                pts.Add(new PointF(xLeft, rect.Bottom + index + 1));
                                pts.Add(new PointF(rect.Right + index + 1, rect.Bottom + index + 1));
                            }
                            else
                            {
                                pts.Add(new PointF(rect.Right, rect.Bottom + edgeOffset));
                                pts.Add(new PointF(rect.Right + index + 1, rect.Bottom + edgeOffset));
                            }
                            pts.Add(new PointF(rect.Right + index + 1, yTop));
                        }
                        else
                        {
                            pts.Add(new PointF(xLeft, rect.Bottom));
                            pts.Add(new PointF(rect.Right + edgeOffset, rect.Bottom));
                            pts.Add(new PointF(rect.Right + edgeOffset, rect.Bottom + index + 1));
                            pts.Add(new PointF(xLeft, rect.Bottom + index + 1));
                        }

                        PointF[] ptss = new PointF[pts.Count];
                        pts.CopyTo(ptss);

                        var shadowBrush = new StiSolidBrush(Color.FromArgb(40 - 10 * index, Color.Black));

                        pdfGeomWriter.FillPolygon(ptss, shadowBrush);
                    }
                    #endregion
                }

                if (geom is StiShadowGeom)
                {
                    var shadow = geom as StiShadowGeom;
                    // todo
                }

                if (geom is StiTextGeom)
                {
                    #region Draw text
                    var text = geom as StiTextGeom;

                    FontFamily family = null;
                    if (PdfFonts.IsFontStimulsoft(text.Font.FontName)) family = StiFontIconsHelper.GetFontFamilyIcons();
                    if (family == null) family = StiFontCollection.GetFontFamily(text.Font.FontName);
                    var font = new Font(family, text.Font.FontSize, text.Font.FontStyle, text.Font.Unit, text.Font.GdiCharSet, text.Font.GdiVerticalFont);

                    StringFormat sf = text.StringFormat.IsGeneric ? StringFormat.GenericDefault.Clone() as StringFormat : new StringFormat();
                    sf.Alignment = text.StringFormat.Alignment;
                    sf.FormatFlags = text.StringFormat.FormatFlags;
                    sf.HotkeyPrefix = text.StringFormat.HotkeyPrefix;
                    sf.LineAlignment = text.StringFormat.LineAlignment;
                    sf.Trimming = text.StringFormat.Trimming;

                    var brush = BrushToStiBrush(text.Brush);

                    bool allowHtmlTags = StiOptions.Engine.AllowHtmlTagsInChart && StiTextRenderer.CheckTextForHtmlTags(text.Text);

                    if (text.IsRotatedText)
                    {
                        PointF point = new PointF();
                        if (text.Location is PointF)
                        {
                            point = (PointF)text.Location;
                        }
                        else
                        {
                            RectangleF rect = RectToRectangleF(text.Location);
                            point = new PointF(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
                        }

                        SizeF textSize;
                        using (var img2 = new Bitmap(1, 1))
                        {
                            using (var gg = Graphics.FromImage(img2))
                            {
                                textSize = gg.MeasureString(text.Text, font, (int)Math.Round(text.MaximalWidth.GetValueOrDefault() / StiDpiHelper.DeviceCapsScale), sf);
                            }
                            textSize.Width *= (float)StiDpiHelper.DeviceCapsScale;
                            textSize.Height *= (float)StiDpiHelper.DeviceCapsScale;
                        }
                        RectangleF textRect = new RectangleF(0, 0, textSize.Width, textSize.Height);
                        PointF startPoint = GetStartPoint(text.RotationMode.GetValueOrDefault(), textRect);
                        textRect.X -= startPoint.X;
                        textRect.Y -= startPoint.Y;

                        //correction
                        if (sf.Alignment == StringAlignment.Center) textRect.X -= 1;
                        if (sf.Alignment == StringAlignment.Far) textRect.X -= 2;
                        textRect.Width += 2;

                        pdfGeomWriter.SaveState();
                        pdfGeomWriter.TranslateTransform(point.X, point.Y);
                        if (text.Angle != 0)
                        {
                            pdfGeomWriter.RotateTransform(text.Angle);
                        }

                        pdfGeomWriter.DrawString(text.Text, font, brush, textRect, sf, allowHtmlTags);

                        pdfGeomWriter.RestoreState();
                    }
                    else
                    {
                        pdfGeomWriter.DrawString(text.Text, font, brush, RectToRectangleF(text.Location), sf, allowHtmlTags);
                    }
                    #endregion
                }

                if (geom is StiPathGeom)
                {
                    #region Draw path
                    var path = geom as StiPathGeom;
                    if ((path.Background != null) || CheckPenGeom(path.Pen))
                    {
                        pdfGeomWriter.BeginPath();

                        foreach (var geom2 in path.Geoms)
                        {
                            if (geom2 is StiPieSegmentGeom)
                            {
                                var pie = geom2 as StiPieSegmentGeom;
                                var startAngle = pie.RealStartAngle ?? pie.StartAngle;
                                var sweepAngle = pie.RealSweepAngle ?? pie.SweepAngle;
                                pdfGeomWriter.DrawPie(pie.Rect, startAngle, sweepAngle);
                            }
                            if (geom2 is StiArcSegmentGeom)
                            {
                                var arc = geom2 as StiArcSegmentGeom;
                                var startAngle = arc.RealStartAngle ?? arc.StartAngle;
                                var sweepAngle = arc.RealSweepAngle ?? arc.SweepAngle;
                                pdfGeomWriter.DrawArc(arc.Rect, startAngle, sweepAngle);
                            }
                            if (geom2 is StiLineSegmentGeom)
                            {
                                var line = geom2 as StiLineSegmentGeom;
                                pdfGeomWriter.DrawLine(new PointF(line.X1, line.Y1), new PointF(line.X2, line.Y2), null);
                            }
                            if (geom2 is StiLinesSegmentGeom)
                            {
                                var lines = geom2 as StiLinesSegmentGeom;
                                pdfGeomWriter.DrawPolyline(lines.Points, null);
                            }
                            if (geom2 is StiCurveSegmentGeom)
                            {
                                var curve = geom2 as StiCurveSegmentGeom;
                                pdfGeomWriter.DrawSpline(curve.Points, curve.Tension, null);
                            }
                            if (geom2 is StiCloseFigureSegmentGeom)
                            {
                                pdfGeomWriter.CloseFigure();
                            }
                        }

                        pdfGeomWriter.CloseFigure();
                        pdfGeomWriter.EndPath();

                        if (path.Background != null)
                        {
                            pdfGeomWriter.FillPath(path.Background);
                        }
                        if (CheckPenGeom(path.Pen))
                        {
                            pdfGeomWriter.StrokePath(path.Pen);
                        }
                    }
                    #endregion
                }
            }
        }

        private static PointF GetStartPoint(StiRotationMode rotationMode, RectangleF textRect)
        {
            PointF centerPoint = new PointF(textRect.X + (textRect.Width / 2), textRect.Y + (textRect.Height / 2));

            switch (rotationMode)
            {
                case StiRotationMode.LeftCenter:
                    return new PointF(textRect.X, centerPoint.Y);

                case StiRotationMode.LeftBottom:
                    return new PointF(textRect.X, textRect.Bottom);

                case StiRotationMode.CenterTop:
                    return new PointF(centerPoint.X, textRect.Top);

                case StiRotationMode.CenterCenter:
                    return centerPoint;

                case StiRotationMode.CenterBottom:
                    return new PointF(centerPoint.X, textRect.Bottom);

                case StiRotationMode.RightTop:
                    return new PointF(textRect.Right, textRect.Top);

                case StiRotationMode.RightCenter:
                    return new PointF(textRect.Right, centerPoint.Y);

                case StiRotationMode.RightBottom:
                    return new PointF(textRect.Right, textRect.Bottom);

                default:
                    return textRect.Location;
            }
        }

        private RectangleF RectToRectangleF(object rect)
        {
            if (rect is RectangleF) return (RectangleF)rect;
            if (rect is Rectangle)
            {
                Rectangle rectangle = (Rectangle)rect;
                return new RectangleF((float)rectangle.X, (float)rectangle.Y, (float)rectangle.Width, (float)rectangle.Height);
            }
            if (rect is RectangleD)
            {
                RectangleD rectangle = (RectangleD)rect;
                return new RectangleF((float)rectangle.X, (float)rectangle.Y, (float)rectangle.Width, (float)rectangle.Height);
            }
            //if (rect is PointF)
            //{
            //    PointF point = (PointF)rect;
            //    return new RectangleF(point.X, point.Y, 0, 0);
            //}
            return new RectangleF();
        }

        private StiBrush BrushToStiBrush(object brush)
        {
            if (brush == null) return null;
            if (brush is Color)
            {
                return new StiSolidBrush((Color)brush);
            }
            if (brush is StiBrush)
            {
                return (StiBrush)brush;
            }
            return null;
        }

        private bool CheckPenGeom(StiPenGeom penGeom)
        {
            return !((penGeom == null) || (penGeom.Brush == null) || (penGeom.PenStyle == StiPenStyle.None));
        }
        #endregion

        #region RenderGauge
        private void RenderGauge(StiPdfData pp, bool assemble, int pageNumber)
        {
            var gauge = pp.Component as StiGauge;
            StiGdiGaugeContextPainter painter;

            float addScale = (float)StiScale.Factor;

            using (var img = new Bitmap(1, 1))
            {
                using (var g = Graphics.FromImage(img))
                {
                    var storeIsAnimation = gauge.IsAnimation;
                    gauge.IsAnimation = false;

                    float scale = 0.96f;

                    painter = new StiGdiGaugeContextPainter(
                        g,
                        gauge,
                        new RectangleF(0, 0, (float)(report.Unit.ConvertToHInches(pp.Component.Width) * scale * addScale), (float)(report.Unit.ConvertToHInches(pp.Component.Height) * scale * addScale)),
                        scale);

                    painter.Geoms.Clear();
                    gauge.DrawGauge(painter);
                    gauge.IsAnimation = storeIsAnimation;
                }
            }

            addScale = 1 / addScale;

            Matrix matrix = new Matrix(1, 0, 0, 1, 0, 0);
            float gaugeScale = (float)(hiToTwips / 0.96 * addScale);
            matrix.Translate((float)(pp.X), (float)(pp.Y + pp.Height));
            matrix.Scale(1, -1);
            matrix.Scale(gaugeScale, gaugeScale);

            StiPdfGeomWriter pdfGeomWriter = new StiPdfGeomWriter(pageStream, this, assemble);
            pdfGeomWriter.pageNumber = pageNumber;
            pdfGeomWriter.matrixCache.Push(matrix);

            pdfGeomWriter.SaveState();
            pdfGeomWriter.ScaleTransform(addScale, addScale);

            foreach (var geom in painter.Geoms)
            {
                if (geom is StiPushMatrixGaugeGeom)
                {
                    var matrixGeom = geom as StiPushMatrixGaugeGeom;
                    pdfGeomWriter.SaveState();
                    pdfGeomWriter.RotateTransform(matrixGeom.Angle, matrixGeom.CenterPoint.X, matrixGeom.CenterPoint.Y);
                    pdfGeomWriter.SaveState();
                    pdfGeomWriter.TranslateTransform(-matrixGeom.CenterPoint.X, -matrixGeom.CenterPoint.Y);
                }
                else if (geom is StiPopTranformGaugeGeom)
                {
                    pdfGeomWriter.RestoreState();
                    pdfGeomWriter.RestoreState();
                }
                else if (geom.Type == StiGaugeGeomType.RoundedRectangle)
                {
                    var roundedRectangleGeom = geom as StiRoundedRectangleGaugeGeom;
                    pdfGeomWriter.BeginPath();

                    var offset = 1;
                    var rightTop = roundedRectangleGeom.RightTop;
                    var leftTop = roundedRectangleGeom.LeftTop;
                    var rightBottom = roundedRectangleGeom.RightBottom;
                    var leftBottom = roundedRectangleGeom.LeftBottom;
                    var rect = roundedRectangleGeom.Rect;
                    var right = rect.X + rect.Width - offset;
                    var bottom = rect.Y + rect.Height - offset;

                    pdfGeomWriter.MoveTo(new PointF(rect.X + leftTop, rect.Y));
                    if (rightTop != 0)
                    {
                        pdfGeomWriter.DrawLineTo(new PointF(right - rightTop, rect.Y), null);
                        pdfGeomWriter.DrawArc(new RectangleF(right - rightTop, rect.Y, rightTop * 2, rightTop * 2), 270, 90);
                    }
                    else
                    {
                        pdfGeomWriter.DrawLineTo(new PointF(right, rect.Y), null);
                    }

                    if (rightBottom != 0)
                    {
                        pdfGeomWriter.DrawLineTo(new PointF(right, bottom - rightBottom), null);
                        pdfGeomWriter.DrawArc(new RectangleF(right - rightBottom, bottom - rightBottom, rightBottom * 2, rightBottom * 2), 0, 90);
                    }
                    else
                    {
                        pdfGeomWriter.DrawLineTo(new PointF(right, bottom), null);
                    }

                    if (leftBottom != 0)
                    {
                        pdfGeomWriter.DrawLineTo(new PointF(rect.X + leftBottom, bottom), null);
                        pdfGeomWriter.DrawArc(new RectangleF(rect.X - leftBottom, bottom - leftBottom, leftBottom * 2, leftBottom * 2), 90, 90);
                    }
                    else
                    {
                        pdfGeomWriter.DrawLineTo(new PointF(rect.X, bottom), null);
                    }

                    pdfGeomWriter.CloseFigure();
                    pdfGeomWriter.EndPath();

                    if (!StiBrush.IsTransparent(roundedRectangleGeom.Background))
                    {
                        pdfGeomWriter.FillPath(roundedRectangleGeom.Background);
                    }

                    var penGeom = GetPenGeom(roundedRectangleGeom);
                    if (CheckPenGeom(penGeom))
                    {
                        pdfGeomWriter.StrokePath(penGeom);
                    }
                }
                else if (geom.Type == StiGaugeGeomType.Rectangle)
                {
                    var rectangleGeom = geom as StiRectangleGaugeGeom;
                    if (rectangleGeom.Background != null)
                    {
                        pdfGeomWriter.FillRectangle(RectToRectangleF(rectangleGeom.Rect), rectangleGeom.Background);
                    }
                    var penGeom = GetPenGeom(rectangleGeom);
                    if (CheckPenGeom(penGeom))
                    {
                        pdfGeomWriter.DrawRectangle(RectToRectangleF(rectangleGeom.Rect), penGeom);
                    }
                }
                else if (geom.Type == StiGaugeGeomType.GraphicsPath)
                {
                    var pathGeom = geom as StiGraphicsPathGaugeGeom;
                    pdfGeomWriter.BeginPath();

                    var isClosed = false;

                    foreach (var pointGeom in pathGeom.Geoms)
                    {
                        if (pointGeom.Type == StiGaugeGeomType.GraphicsPathArc)
                        {
                            var arcGeom = pointGeom as StiGraphicsPathArcGaugeGeom;
                            pdfGeomWriter.DrawArc(new RectangleF(arcGeom.X, arcGeom.Y, arcGeom.Width, arcGeom.Height), arcGeom.StartAngle, arcGeom.SweepAngle);
                        }
                        else if (pointGeom.Type == StiGaugeGeomType.GraphicsPathLine)
                        {
                            var lineGeom = pointGeom as StiGraphicsPathLineGaugeGeom;
                            pdfGeomWriter.DrawLine(lineGeom.P1, lineGeom.P2, null);
                        }
                        else if (pointGeom.Type == StiGaugeGeomType.GraphicsPathLines)
                        {
                            var linesSegment = pointGeom as StiGraphicsPathLinesGaugeGeom;
                            pdfGeomWriter.DrawPolyline(linesSegment.Points, null);
                            pdfGeomWriter.CloseFigure();
                            isClosed = true;
                        }
                        else if (pointGeom.Type == StiGaugeGeomType.GraphicsPathCloseFigure)
                        {
                            pdfGeomWriter.CloseFigure();
                            isClosed = true;
                        }
                    }
                    
                    pdfGeomWriter.EndPath();

                    if (isClosed && pathGeom.Background != null)
                    {
                        pdfGeomWriter.FillPath(pathGeom.Background);
                    }
                    var penGeom = GetPenGeom(pathGeom);
                    if (CheckPenGeom(penGeom) && pathGeom.BorderWidth > 0)
                    {
                        pdfGeomWriter.StrokePath(penGeom);
                    }
                }
                else if (geom.Type == StiGaugeGeomType.Pie)
                {
                    var pieGeom = geom as StiPieGaugeGeom;
                    pdfGeomWriter.BeginPath();

                    pdfGeomWriter.DrawPie(pieGeom.Rect, pieGeom.StartAngle, pieGeom.SweepAngle);

                    pdfGeomWriter.CloseFigure();
                    pdfGeomWriter.EndPath();

                    var penGeom = GetPenGeom(pieGeom);
                    if (CheckPenGeom(penGeom))
                    {
                        pdfGeomWriter.StrokePath(penGeom);
                    }
                }
                else if (geom.Type == StiGaugeGeomType.Ellipse)
                {
                    var ellipseGeom = geom as StiEllipseGaugeGeom;
                    if (ellipseGeom.Background != null)
                    {
                        pdfGeomWriter.FillEllipse(RectToRectangleF(ellipseGeom.Rect), ellipseGeom.Background);
                    }

                    var penGeom = GetPenGeom(ellipseGeom);
                    if (CheckPenGeom(penGeom))
                    {
                        pdfGeomWriter.DrawEllipse(RectToRectangleF(ellipseGeom.Rect), penGeom);
                    }
                }
                else if (geom.Type == StiGaugeGeomType.GraphicsArcGeometry)
                {
                    var arcGeom = geom as StiGraphicsArcGeometryGaugeGeom;

                    var centerPoint = new PointF(StiRectangleHelper.CenterX(arcGeom.Rect), StiRectangleHelper.CenterY(arcGeom.Rect));
                    float radius = Math.Min(arcGeom.Rect.Width / 2, arcGeom.Rect.Height / 2);
                    float secondStartRadius = radius - (arcGeom.Rect.Width * arcGeom.StartWidth);
                    float secondEndRadius = radius - (arcGeom.Rect.Width * arcGeom.EndWidth);
                    if (secondStartRadius > 0 && secondEndRadius > 0)
                    {
                        float steps = Round(Math.Abs(arcGeom.SweepAngle / 90));
                        float stepAngle = arcGeom.SweepAngle / steps;
                        var currentStartAngle = arcGeom.StartAngle;

                        #region First Arc
                        pdfGeomWriter.BeginPath();
                        var isFirst = true;
                        for (var indexStep = 0; indexStep < steps; indexStep++)
                        {
                            List<PointF> points = StiDrawingHelper.ConvertArcToCubicBezier(centerPoint, radius, currentStartAngle, stepAngle);
                            if (isFirst)
                            {
                                pdfGeomWriter.MoveTo(points[0]);
                                isFirst = false;
                            }
                            pdfGeomWriter.DrawBezierTo(points[1], points[2], points[3], null);

                            currentStartAngle += stepAngle;
                        }
                        #endregion

                        #region Second Arc
                        float offsetSecondRadius = secondStartRadius - secondEndRadius;
                        float offsetStep = 1 / steps;
                        float offset = 0;
                        isFirst = true;

                        currentStartAngle = arcGeom.StartAngle + arcGeom.SweepAngle;
                        for (var indexStep = 0; indexStep < steps; indexStep++)
                        {
                            float startRadius = secondStartRadius - (offsetSecondRadius * offset);
                            float endRadius = secondStartRadius - (offsetSecondRadius * (offset + offsetStep));

                            var points = StiDrawingHelper.ConvertArcToCubicBezier(centerPoint, startRadius, endRadius, currentStartAngle, -stepAngle);
                            if (isFirst)
                            {
                                pdfGeomWriter.DrawLineTo(points[0], null);
                                isFirst = false;
                            }
                            pdfGeomWriter.DrawBezierTo(points[1], points[2], points[3], null);

                            currentStartAngle -= stepAngle;
                            offset += offsetStep;
                        }

                        pdfGeomWriter.CloseFigure();
                        pdfGeomWriter.EndPath();
                        #endregion

                        if (steps > 0)
                        {
                            if (!StiBrush.IsTransparent(arcGeom.Background))
                            {
                                pdfGeomWriter.FillPath(arcGeom.Background);
                            }

                            var penGeom = GetPenGeom(arcGeom);
                            if (CheckPenGeom(penGeom))
                            {
                                pdfGeomWriter.StrokePath(penGeom);
                            }
                        }
                    }
                }
                else if (geom.Type == StiGaugeGeomType.Text)
                {
                    var textGeom = geom as StiTextGaugeGeom;
                    var font = new Font(StiFontCollection.GetFontFamily(textGeom.Font.Name), (float)(textGeom.Font.Size / addScale), textGeom.Font.Style, textGeom.Font.Unit, textGeom.Font.GdiCharSet, textGeom.Font.GdiVerticalFont);

                    StringFormat sf = new StringFormat();
                    sf.Alignment = textGeom.StringFormat.Alignment;
                    sf.FormatFlags = textGeom.StringFormat.FormatFlags;
                    sf.HotkeyPrefix = textGeom.StringFormat.HotkeyPrefix;
                    sf.LineAlignment = textGeom.StringFormat.LineAlignment;
                    sf.Trimming = textGeom.StringFormat.Trimming;

                    bool allowHtmlTags = StiOptions.Engine.AllowHtmlTagsInChart && StiTextRenderer.CheckTextForHtmlTags(textGeom.Text);

                    pdfGeomWriter.DrawString(textGeom.Text, font, textGeom.Foreground, textGeom.Rect, sf, allowHtmlTags);
                }
                else if (geom.Type == StiGaugeGeomType.RadialRange)
                {
                    var radialRangeGeom = geom as StiRadialRangeGaugeGeom;

                    var centerPoint = radialRangeGeom.CenterPoint;
                    var startAngle = radialRangeGeom.StartAngle;
                    var sweepAngle = radialRangeGeom.SweepAngle;
                    var radius1 = radialRangeGeom.Radius1;
                    var radius2 = radialRangeGeom.Radius2;
                    var radius3 = radialRangeGeom.Radius3;
                    var radius4 = radialRangeGeom.Radius4;

                    float steps = Round(Math.Abs(sweepAngle / 90));
                    float stepAngle = sweepAngle / steps;

                    #region First Arc
                    pdfGeomWriter.BeginPath();
                    var restRadius = radius1 - radius2;
                    float offsetStep = 1 / (steps);
                    float offset = 0;

                    var isFirst = true;
                    var currentStartAngle = startAngle + sweepAngle;
                    for (var indexStep = 0; indexStep < steps; indexStep++)
                    {
                        var startRadius = radius1 - (restRadius * offset);
                        var endRadius = radius1 - (restRadius * (offset + offsetStep));

                        List<PointF> points = StiDrawingHelper.ConvertArcToCubicBezier(centerPoint, startRadius, endRadius, currentStartAngle, -stepAngle);
                        if (isFirst)
                        {
                            pdfGeomWriter.MoveTo(points[0]);
                            isFirst = false;
                        }
                        pdfGeomWriter.DrawBezierTo(points[1], points[2], points[3], null);

                        currentStartAngle -= stepAngle;
                        offset += offsetStep;
                    }
                    #endregion

                    #region Second Arc
                    restRadius = radius3 - radius4;
                    offset = 0;
                    isFirst = true;

                    currentStartAngle = startAngle;
                    for (var indexStep = 0; indexStep < steps; indexStep++)
                    {
                        float startRadius = radius3 - (restRadius * offset);
                        float endRadius = radius3 - (restRadius * (offset + offsetStep));

                        List<PointF> points = StiDrawingHelper.ConvertArcToCubicBezier(centerPoint, startRadius, endRadius, currentStartAngle, stepAngle);
                        if (isFirst)
                        {
                            pdfGeomWriter.DrawLineTo(points[0], null);
                            isFirst = false;
                        }
                        pdfGeomWriter.DrawBezierTo(points[1], points[2], points[3], null);

                        currentStartAngle += stepAngle;
                        offset += offsetStep;
                    }

                    pdfGeomWriter.CloseFigure();
                    pdfGeomWriter.EndPath();
                    #endregion

                    if (steps > 0)
                    {
                        if (!StiBrush.IsTransparent(radialRangeGeom.Background))
                        {
                            pdfGeomWriter.FillPath(radialRangeGeom.Background);
                        }

                        var penGeom = GetPenGeom(radialRangeGeom);
                        if (CheckPenGeom(penGeom))
                        {
                            pdfGeomWriter.StrokePath(penGeom);
                        }
                    }
                }
            }
            pdfGeomWriter.RestoreState();
        }

        private StiPenGeom GetPenGeom(StiBrush brush, float width)
        {
            return new StiPenGeom(brush, width);
        }

        private StiPenGeom GetPenGeom(StiGaugeGeom geom)
        {
            if (geom is StiRectangleGaugeGeom) return GetPenGeom((geom as StiRectangleGaugeGeom).BorderBrush, (geom as StiRectangleGaugeGeom).BorderWidth);
            if (geom is StiRoundedRectangleGaugeGeom) return GetPenGeom((geom as StiRoundedRectangleGaugeGeom).BorderBrush, (geom as StiRoundedRectangleGaugeGeom).BorderWidth);
            if (geom is StiEllipseGaugeGeom) return GetPenGeom((geom as StiEllipseGaugeGeom).BorderBrush, (geom as StiEllipseGaugeGeom).BorderWidth);
            if (geom is StiGraphicsPathGaugeGeom) return GetPenGeom((geom as StiGraphicsPathGaugeGeom).BorderBrush, (geom as StiGraphicsPathGaugeGeom).BorderWidth);
            if (geom is StiPieGaugeGeom) return GetPenGeom((geom as StiPieGaugeGeom).BorderBrush, (geom as StiPieGaugeGeom).BorderWidth);
            if (geom is StiGraphicsArcGeometryGaugeGeom) return GetPenGeom((geom as StiGraphicsArcGeometryGaugeGeom).BorderBrush, (geom as StiGraphicsArcGeometryGaugeGeom).BorderWidth);
            if (geom is StiRadialRangeGaugeGeom) return GetPenGeom((geom as StiRadialRangeGaugeGeom).BorderBrush, (geom as StiRadialRangeGaugeGeom).BorderWidth);
            return null;
        }

        private static float Round(float value)
        {
            int value1 = (int)value;
            float rest = value - value1;
            return (rest > 0) ? (float)(value1 + 1) : (float)(value1);
        }
        #endregion

        #region RenderMap
        private void RenderMap(StiPdfData pp, bool assemble, int pageNumber)
        {
            var map = pp.Component as StiMap;

            var painter = new StiGdiMapContextPainter(map)
            {
                MapStyle = StiMap.GetMapStyle(map),
                DataTable = map.DataTable
            };
            painter.PrepareDataColumns();
            painter.UpdateGroupedData();
            painter.UpdateHeatmapWithGroup();
            StiBrush DefaultBrush1 = new StiSolidBrush(painter.MapStyle.DefaultColor);

            var geomContainer = StiMapLoader.GetGeomsObject(map.Report, map.MapIdent, map.Language);
            bool skipLabels = false;
            bool center = true;
            float zoom = 1;
            if (map.Stretch)
            {
                var rect = map.GetPaintRectangle(true, false);

                zoom = Math.Min(
                    (float)rect.Width / (float)geomContainer.Width,
                    (float)rect.Height / (float)geomContainer.Height);

                if (zoom == 0) zoom = 1f;
            }

            Matrix matrix = new Matrix(1, 0, 0, 1, 0, 0);
            const float gaugeScale = (float) (hiToTwips / 0.96);
            matrix.Translate((float) (pp.X), (float) (pp.Y + pp.Height));
            matrix.Scale(1, -1);
            matrix.Scale(gaugeScale, gaugeScale);

            var pdfGeomWriter = new StiPdfGeomWriter(pageStream, this, assemble);
            pdfGeomWriter.pageNumber = pageNumber;
            pdfGeomWriter.matrixCache.Push(matrix);

            pdfGeomWriter.SaveState();
            pdfGeomWriter.ScaleTransform(zoom * 0.96f, zoom * 0.96f);

            if (map.Stretch && center)
            {
                var rect = map.GetPaintRectangle(true, false);
                var destWidth = rect.Width / zoom;
                var destHeight = rect.Height / zoom;

                pdfGeomWriter.TranslateTransform(
                    (float)((destWidth - geomContainer.Width) / 2),
                    (float)((destHeight - geomContainer.Height) / 2));
            }

            var hashLabels = new List<string>();

            float individualStepValue = 0.5f / geomContainer.Geoms.Count;
            painter.IndividualStep = individualStepValue;
            foreach (var geoms in geomContainer.Geoms.OrderBy(x => x.Key))
            {
                var data = painter.MapData.FirstOrDefault(x => x.Key == geoms.Key);
                var pointCount = 0;

                var brush = painter.GetGeomBrush1(data);
                if (brush == null) brush = DefaultBrush1;
                painter.IndividualStep += individualStepValue;

                foreach (StiMapGeom geom in geoms.Geoms)
                {
                    switch (geom.GeomType)
                    {
                        case StiMapGeomType.MoveTo:
                        {
                            if (pointCount > 0)
                            {
                                FillMapPath(pdfGeomWriter, painter.IsBorderEmpty, brush, painter.MapStyle.BorderColor, painter.MapStyle.BorderSize);
                                pointCount = 0;
                            }
                            pdfGeomWriter.BeginPath();

                            var moveTo = (StiMoveToMapGeom) geom;
                            pdfGeomWriter.MoveTo(new PointF((float) moveTo.X, (float) moveTo.Y));
                        }
                            break;

                        case StiMapGeomType.Line:
                        {
                            var line = (StiLineMapGeom) geom;

                            var point = new PointF((float) line.X, (float) line.Y);
                            pdfGeomWriter.DrawLineTo(point, null);
                            pointCount++;
                        }
                            break;

                        case StiMapGeomType.Bezier:
                        {
                            var bezier = (StiBezierMapGeom) geom;

                            var point1 = new PointF((float) bezier.X1, (float) bezier.Y1);
                            var point2 = new PointF((float) bezier.X2, (float) bezier.Y2);
                            var point3 = new PointF((float) bezier.X3, (float) bezier.Y3);

                            pdfGeomWriter.DrawBezierTo(point1, point2, point3, null);
                            pointCount++;
                            //pdfGeomWriter.StrokePath(new Pen(Color.Red));
                            //return;
                        }
                            break;

                        case StiMapGeomType.Beziers:
                        {
                            var beziers = (StiBeziersMapGeom) geom;

                            var listp = new List<PointF>();
                            var p = PointF.Empty;
                            for (var indexp = 0; indexp < beziers.Array.Length; indexp += 2)
                            {
                                p = new PointF((float)beziers.Array[indexp], (float)beziers.Array[indexp + 1]);
                                listp.Add(p);
                            }
                            listp.Add(p);

                            if (listp.Count % 3 == 2) listp.RemoveAt(listp.Count - 1);
                            if (listp.Count % 3 == 1) listp.RemoveAt(listp.Count - 1);

                            var index = 0;
                            while (index < listp.Count)
                            {
                                pdfGeomWriter.DrawBezierTo(listp[index], listp[index + 1], listp[index + 2], null);
                                index += 3;
                            }
                        }
                            break;

                        case StiMapGeomType.Close:
                        {
                            pointCount = 0;
                            pdfGeomWriter.CloseFigure();
                            FillMapPath(pdfGeomWriter, painter.IsBorderEmpty, brush, painter.MapStyle.BorderColor, painter.MapStyle.BorderSize);
                        }
                            break;
                    }
                }

                if (pointCount > 0)
                {
                    pointCount = 0;
                    FillMapPath(pdfGeomWriter, painter.IsBorderEmpty, brush, painter.MapStyle.BorderColor, painter.MapStyle.BorderSize);
                }

                if (map.ShowValue || map.DisplayNameType != StiDisplayNameType.None)
                    hashLabels.Add(geoms.Key);
            }

            #region Draw Labels
            if (hashLabels.Count > 0 && !skipLabels)
            {
                var svgContainer = StiMapLoader.LoadResource(map.Report, map.MapIdent, map.Language);

                float fontScale = gaugeScale * (zoom > 1 ? 1 : zoom);
                var typeface = new Font("Calibri", 18 * fontScale);
                var foregroundDark = new SolidBrush(Color.FromArgb(255, 37, 37, 37));
                var foregroundLight = new SolidBrush(Color.FromArgb(180, 251, 251, 251));
                var stiForegroundDark = new StiSolidBrush(Color.FromArgb(255, 37, 37, 37));
                var stiForegroundLight = new StiSolidBrush(Color.FromArgb(180, 251, 251, 251));

                foreach (var key in hashLabels)
                {
                    var path = svgContainer.HashPaths[key];
                    if (path.SkipText) continue;

                    string text = null;
                    var info = painter.MapData.FirstOrDefault(x => x.Key == key);
                    switch (map.DisplayNameType)
                    {
                        case StiDisplayNameType.Full:
                            {
                                text = (info != null)
                                    ? info.Name
                                    : path.EnglishName;
                            }
                            break;

                        case StiDisplayNameType.Short:
                            {
                                text = Stimulsoft.Report.Maps.Helpers.StiMapHelper.PrepareIsoCode(path.ISOCode);
                            }
                            break;
                    }

                    if (map.ShowValue)
                    {
                        if (info != null && info.Value != null)
                        {
                            string valueStr = null;
                            if (map.ShortValue)
                            {
                                double resValue = 0d;
                                if (double.TryParse(info.Value, out resValue))
                                {
                                    valueStr = StiAbbreviationNumberFormatHelper.Format(resValue);
                                }
                            }

                            if (valueStr == null)
                                valueStr = info.Value;

                            if (text == null)
                            {
                                text = valueStr;
                            }
                            else
                            {
                                text += Environment.NewLine;
                                text += valueStr;
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(text))
                    {
                        if (assemble)
                        {
                            int fnt = pdfFont.GetFontNumber(typeface);
                            //StringBuilder sb = new StringBuilder(text);
                            //sb = bidi.Convert(sb, false);
                            var sb = new StringBuilder(StiBidirectionalConvert2.ConvertString(text, false));
                            pdfFont.StoreUnicodeSymbolsInMap(sb);
                            continue;
                        }

                        var bounds = path.Rect;

                        StiText textComp = new StiText(new RectangleD(1, 0, bounds.Width, bounds.Height), text);
                        textComp.Font = typeface;
                        textComp.HorAlignment = path.HorAlignment;
                        textComp.VertAlignment = path.VertAlignment;
                        if (path.SetMaxWidth) textComp.WordWrap = true;

                        StiPdfData pp2 = new StiPdfData();
                        pp2.Component = textComp;
                        pp2.X = 1;
                        pp2.Y = 0;
                        pp2.Width = bounds.Width * hiToTwips;
                        pp2.Height = bounds.Height * hiToTwips;

                        pageStream.WriteLine("q");
                        PushColorToStack();
                        pageStream.WriteLine("1 0 0 1 {0} {1} cm", ConvertToString(bounds.X), ConvertToString(bounds.Y + bounds.Height));
                        pageStream.WriteLine("1 0 0 -1 0 0 cm");
                        pageStream.WriteLine("1.39 0 0 1.39 0 0 cm");

                        RenderTextFont(pp2);
                        textComp.TextBrush = stiForegroundLight;
                        RenderText(pp2);
                        pp2.X--;
                        pp2.Y++;
                        textComp.TextBrush = stiForegroundDark;
                        RenderText(pp2);

                        pageStream.WriteLine("Q");
                        PopColorFromStack();
                    }
                }
            }
            #endregion

            pdfGeomWriter.RestoreState();
        }

        private void FillMapPath(StiPdfGeomWriter pdfGeomWriter, bool isBorderEmpty, StiBrush fillBrush, Color borderColor, double borderSize)
        {
            pdfGeomWriter.FillPath(StiBrush.ToColor(fillBrush));

            if (!isBorderEmpty)
            {
                using (var pen = new Pen(borderColor, (float)borderSize))
                {
                    pdfGeomWriter.StrokePath(pen);
                }
            }
        }

        #endregion

        #region RenderSparkline
        #endregion

        #region Render Watermark
        private void RenderWatermark(StiPage page, bool behind, double pageWidth, double pageHeight, float imageResolution)
        {
            StiWatermark watermark = page.Watermark;
            if ((watermark != null) && watermark.Enabled)
            {
                #region watermark image
                if (watermarkImageExist.ContainsKey(page) && (watermark.ShowImageBehind == behind))
                {
                    RenderWatermarkImage(
                        new RectangleD(0, 0, pageWidth, pageHeight),
                        watermark.ImageStretch,
                        watermark.ImageTiling,
                        watermark.AspectRatio,
                        watermark.ImageMultipleFactor,
                        watermark.ImageAlignment);
                }
                #endregion

                #region watermark text
                if ((!string.IsNullOrEmpty(watermark.Text)) && (watermark.ShowBehind == behind))
                {
                    StiPdfData pp = new StiPdfData();
                    pp.X = 0;
                    pp.Y = 0;
                    pp.Width = pageWidth;
                    pp.Height = pageHeight;
                    StiText stt = new StiText(new RectangleD(pp.X, pp.Y, pp.Width, pp.Height));
                    stt.Text = watermark.Text;
                    stt.TextBrush = watermark.TextBrush;
                    stt.Font = watermark.Font;
                    stt.TextOptions = new StiTextOptions();
                    stt.TextOptions.Angle = watermark.Angle;
                    stt.HorAlignment = StiTextHorAlignment.Center;
                    stt.VertAlignment = StiVertAlignment.Center;
                    stt.Page = page;
                    stt.TextQuality = StiTextQuality.Standard;
                    pp.Component = stt;
                    RenderTextFont(pp);
                    RenderText(pp);
                }
                #endregion

                #region Watermark Weave
                var advWatermark = page.TagValue as StiAdvancedWatermark;
                if (advWatermark != null && advWatermark.WeaveEnabled && behind)
                {
                    RenderWatermarkWeave(advWatermark, new RectangleD(0, 0, pageWidth, pageHeight), page);
                }
                #endregion
            }
        }

        private void RenderWatermarkImage(RectangleD mainRect, bool imageStretch, bool isImageTiling, bool imageAspectRatio, double imageMultipleFactor, ContentAlignment imageAlignment)
        {
            StiImageData pd = (StiImageData)imageList[imagesCurrent];

            double imageWidth = pd.Width * hiToTwips * imageMultipleFactor;
            double imageHeight = pd.Height * hiToTwips * imageMultipleFactor;
            double imageX = mainRect.Left;
            double imageY = mainRect.Top;
            int dupX = 1;
            int dupY = 1;

            if (imageStretch)
            {
                double aspectRatio = imageHeight / imageWidth;
                imageWidth = mainRect.Width;
                imageHeight = mainRect.Height;
                isImageTiling = false;
                if (imageAspectRatio)
                {
                    if (mainRect.Height / mainRect.Width > aspectRatio)
                    {
                        imageHeight = imageWidth * aspectRatio;
                    }
                    else
                    {
                        imageWidth = imageHeight / aspectRatio;
                    }
                }
            }
            if (imageStretch || (imageMultipleFactor > 1))
            {
                //pd.Interpolate = true;
                //imageList[imagesCurrent] = pd;
                imageInterpolationTable[(int)imageCache.ImageIndex[imagesCurrent]] = true;
            }
            if (isImageTiling)
            {
                imageAlignment = ContentAlignment.TopLeft;
                dupX = (int)(mainRect.Width / imageWidth) + 1;
                dupY = (int)(mainRect.Height / imageHeight) + 1;
            }

            switch (imageAlignment)
            {
                case ContentAlignment.TopCenter:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.BottomCenter:
                    imageX += (mainRect.Width - imageWidth) / 2f;
                    break;

                case ContentAlignment.TopRight:
                case ContentAlignment.MiddleRight:
                case ContentAlignment.BottomRight:
                    imageX += mainRect.Width - imageWidth;
                    break;
            }
            switch (imageAlignment)
            {
                case ContentAlignment.TopLeft:
                case ContentAlignment.TopCenter:
                case ContentAlignment.TopRight:
                    imageY += mainRect.Height - imageHeight;
                    break;

                case ContentAlignment.MiddleLeft:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.MiddleRight:
                    imageY += (mainRect.Height - imageHeight) / 2f;
                    break;
            }

            SetNonStrokeColor(Color.Black);

            for (int indexY = 0; indexY < dupY; indexY++)
            {
                for (int indexX = 0; indexX < dupX; indexX++)
                {
                    pageStream.WriteLine("q");
                    PushColorToStack();
                    pageStream.WriteLine("{0} 0 0 {1} {2} {3} cm",
                        ConvertToString(imageWidth),
                        ConvertToString(imageHeight),
                        ConvertToString(imageX + imageWidth * indexX),
                        ConvertToString(imageY - imageHeight * indexY));
                    pageStream.WriteLine("/{0} Do", pd.Name);
                    pageStream.WriteLine("Q");
                    PopColorFromStack();
                }
            }

            imagesCurrent++;
        }

        private void RenderWatermarkWeave(StiAdvancedWatermark advWatermark, RectangleD mainRect, StiPage page)
        {
            if (advWatermark == null) return;

            #region Prepare components
            StiText txt = new StiText();
            txt.HorAlignment = StiTextHorAlignment.Center;
            txt.VertAlignment = StiVertAlignment.Center;
            txt.TextQuality = StiTextQuality.Standard;
            txt.Page = page;
            txt.Border = new StiBorder(StiBorderSides.All, Color.Green, 3, StiPenStyle.Solid);

            var brush1 = new StiSolidBrush(advWatermark.WeaveMajorColor);
            var brush2 = new StiSolidBrush(advWatermark.WeaveMinorColor);

            FontFamily family = StiFontIconsHelper.GetFontFamilyIcons();
            if (family == null) family = StiFontCollection.GetFontFamily("Stimulsoft");
            var font1 = new Font(family, (float)(advWatermark.WeaveMajorSize * 5 * hiToTwips * 0.85));
            var font2 = new Font(family, (float)(advWatermark.WeaveMinorSize * 5 * hiToTwips * 0.85));

            string text1 = StiFontIconsHelper.GetContent(advWatermark.WeaveMajorIcon);
            string text2 = StiFontIconsHelper.GetContent(advWatermark.WeaveMinorIcon);

            double step = advWatermark.WeaveDistance * hiToTwips;

            StiPdfData pp = new StiPdfData();
            pp.Component = txt;
            pp.Width = step * 2;
            pp.Height = step * 2;
         
            int range = (int)(Math.Max(mainRect.Width, mainRect.Height) / 2 * 1.4 / step) + 1;
            #endregion

            #region Render weave
            var storeClipMode = clipLongTextLines;
            clipLongTextLines = false;

            pageStream.WriteLine("q");
            PushColorToStack();

            pageStream.WriteLine("1 0 0 1 {0} {1} cm", ConvertToString(mainRect.X + mainRect.Width / 2), ConvertToString(mainRect.Y + mainRect.Height / 2));
            if (advWatermark.WeaveAngle != 0)
            {
                double AngleInRadians = -advWatermark.WeaveAngle * Math.PI / 180f;
                pageStream.WriteLine("{0} {1} {2} {3} {4} {5} cm",
                    ConvertToString(Math.Cos(AngleInRadians)),
                    ConvertToString(Math.Sin(AngleInRadians)),
                    ConvertToString(-Math.Sin(AngleInRadians)),
                    ConvertToString(Math.Cos(AngleInRadians)),
                    ConvertToString(0),
                    ConvertToString(0));
            }

            for (int dy = -range; dy <= range; dy++)
            {
                for (int dx = -range; dx <= range; dx++)
                {
                    int x = (int)(dx * step);
                    int y = (int)(dy * step);
                    bool isOdd = (Math.Abs(dx + dy) & 1) == 0;  //almost odd :)

                    pageStream.WriteLine("q");
                    pageStream.WriteLine("1 0 0 1 {0} {1} cm", ConvertToString(x), ConvertToString(y));

                    txt.ClientRectangle = new RectangleD(-step, -step, step * 2, step * 2);
                    txt.TextBrush = isOdd ? brush1 : brush2;
                    txt.Font = isOdd ? font1 : font2;
                    txt.Text = isOdd ? text1 : text2;

                    pp.X = -step;
                    pp.Y = -step;

                    RenderTextFont(pp);
                    RenderText(pp);
                    //RenderBorder2(pp);

                    pageStream.WriteLine("Q");
                }
            }

            pageStream.WriteLine("Q");
            PopColorFromStack();

            clipLongTextLines = storeClipMode;
            #endregion
        }
        #endregion

        #region Store brushes data
        internal void StoreShadingData1(StiBrush brush, int pageNumber)
        {
            if (brush != null)
            {
                if (brush is StiGradientBrush)
                {
                    StiGradientBrush gbr = brush as StiGradientBrush;
                    StiShadingData ssd = new StiShadingData();
                    ssd.Angle = gbr.Angle;
                    ssd.Page = pageNumber;
                    ssd.FunctionIndex = GetShadingFunctionNumber(gbr.StartColor, gbr.EndColor, false);
                    shadingArray.Add(ssd);
                }
                if (brush is StiGlareBrush)
                {
                    StiGlareBrush gbr = brush as StiGlareBrush;
                    StiShadingData ssd = new StiShadingData();
                    ssd.Angle = gbr.Angle;
                    ssd.FunctionIndex = GetShadingFunctionNumber(gbr.StartColor, gbr.EndColor, true);
                    ssd.Page = pageNumber;
                    shadingArray.Add(ssd);
                }
            }
        }
        internal int StoreShadingData2(double x, double y, double width, double height, StiBrush brush, float compAngle = 0)
        {
            if ((brush != null) && (brush is StiGradientBrush || brush is StiGlareBrush))
            {
                StiShadingData ssd = (StiShadingData)shadingArray[shadingCurrent];
                ssd.X = x;
                ssd.Y = y;
                ssd.Width = width;
                ssd.Height = height;

                if ((ssd.Angle == 0) && (width > 0) && (height > 0))
                {
                    ssd.Angle = Math.Atan2(height, width) * 180 / Math.PI;
                }
                ssd.Angle += compAngle;

                shadingArray[shadingCurrent] = ssd;
                shadingCurrent++;
            }
            return shadingCurrent;
        }
        internal void StoreHatchData(StiBrush brush)
        {
            if (brush != null)
            {
                if (brush is StiHatchBrush)
                {
                    GetHatchNumber(brush as StiHatchBrush);
                }
                if (brush is StiGlassBrush)
                {
                    StiGlassBrush glass = brush as StiGlassBrush;
                    if (glass.DrawHatch)
                    {
                        GetHatchNumber(glass.GetTopBrush() as HatchBrush);
                        GetHatchNumber(glass.GetBottomBrush() as HatchBrush);
                    }
                }
            }
        }
        #endregion

        #region Render Metadata
        private void RenderMetadata(StiReport report)
        {
            #region Make memory stream
            MemoryStream mainStream = new MemoryStream();

            string stXpacket = "<?xpacket begin=\"\xEF\xBB\xBF\" id=\"W5M0MpCehiHzreSzNTczkc9d\"?>\r\n";
            byte[] bufXpacket = new byte[stXpacket.Length];
            for (int indexChar = 0; indexChar < bufXpacket.Length; indexChar++) bufXpacket[indexChar] = (byte)stXpacket[indexChar];
            mainStream.Write(bufXpacket, 0, bufXpacket.Length);

            #region Make XML data
            MemoryStream ms = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(ms, Encoding.UTF8);
            writer.Formatting = Formatting.Indented;

            writer.WriteStartElement("x:xmpmeta");
            writer.WriteAttributeString("xmlns:x", "adobe:ns:meta/");
            writer.WriteAttributeString("x:xmptk", "Adobe XMP Core 4.0-c316 44.253921, Sun Oct 01 2006 17:14:39");
            writer.WriteStartElement("rdf:RDF");
            writer.WriteAttributeString("xmlns:rdf", "http://www.w3.org/1999/02/22-rdf-syntax-ns#");

            writer.WriteStartElement("rdf:Description");
            writer.WriteAttributeString("rdf:about", "");
            writer.WriteAttributeString("xmlns:xap", "http://ns.adobe.com/xap/1.0/");
            writer.WriteElementString("xap:ModifyDate", currentDateTimeMeta);
            writer.WriteElementString("xap:CreateDate", currentDateTimeMeta);
            writer.WriteElementString("xap:MetadataDate", currentDateTimeMeta);
            writer.WriteElementString("xap:CreatorTool", GetCreatorString());
            writer.WriteFullEndElement();   //rdf:Description

            #region Report info
            writer.WriteStartElement("rdf:Description");
            writer.WriteAttributeString("rdf:about", "");
            writer.WriteAttributeString("xmlns:dc", "http://purl.org/dc/elements/1.1/");
            writer.WriteElementString("dc:format", "application/pdf");

            writer.WriteStartElement("dc:title");
            writer.WriteStartElement("rdf:Alt");
            if (!string.IsNullOrEmpty(report.ReportName))
            {
                writer.WriteStartElement("rdf:li");
                writer.WriteAttributeString("xml:lang", "x-default");
                writer.WriteString(report.ReportName);
                writer.WriteFullEndElement();   //rdf:li
            }
            writer.WriteEndElement();   //rdf:Alt
            writer.WriteFullEndElement();   //dc:title

            writer.WriteStartElement("dc:description");
            writer.WriteStartElement("rdf:Alt");
            if (!string.IsNullOrEmpty(report.ReportAlias))
            {
                writer.WriteStartElement("rdf:li");
                writer.WriteAttributeString("xml:lang", "x-default");
                writer.WriteString(report.ReportAlias);
                writer.WriteFullEndElement();   //rdf:li
            }
            writer.WriteEndElement();   //rdf:Alt
            writer.WriteFullEndElement();   //dc:description

            writer.WriteStartElement("dc:creator");
            writer.WriteStartElement("rdf:Seq");
            if (!string.IsNullOrEmpty(report.ReportAuthor))
            {
                writer.WriteStartElement("rdf:li");
                writer.WriteAttributeString("xml:lang", "x-default");
                writer.WriteString(report.ReportAuthor);
                writer.WriteFullEndElement();   //rdf:li
            }
            writer.WriteEndElement();   //rdf:Seq
            writer.WriteFullEndElement();   //dc:creator

            writer.WriteStartElement("dc:subject");
            writer.WriteStartElement("rdf:Bag");
            if (!string.IsNullOrEmpty(keywords))
            {
                string[] words = keywords.Split(new char[] { ';' });
                foreach (string word in words)
                {
                    writer.WriteStartElement("rdf:li");
                    //writer.WriteAttributeString("xml:lang", "x-default");
                    writer.WriteString(word.Trim());
                    writer.WriteFullEndElement();   //rdf:li
                }
            }
            writer.WriteEndElement();   //rdf:Bag
            writer.WriteFullEndElement();   //dc:subject

            writer.WriteFullEndElement();   //rdf:Description
            #endregion

            writer.WriteStartElement("rdf:Description");
            writer.WriteAttributeString("rdf:about", "");
            writer.WriteAttributeString("xmlns:xapMM", "http://ns.adobe.com/xap/1.0/mm/");
            writer.WriteElementString("xapMM:DocumentID", IDValueStringMeta);
            writer.WriteElementString("xapMM:InstanceID", IDValueStringMeta);
            writer.WriteFullEndElement();   //rdf:Description

            #region Custom metatags
            writer.WriteStartElement("rdf:Description");
            writer.WriteAttributeString("rdf:about", "");
            writer.WriteAttributeString("xmlns:pdf", "http://ns.adobe.com/pdf/1.3/");
            writer.WriteAttributeString("xmlns:pdfx", "http://ns.adobe.com/pdf/1.3/");
            writer.WriteElementString("pdf:Producer", producerName);
            writer.WriteElementString("pdf:Keywords", keywords);

            if (pdfComplianceMode == StiPdfComplianceMode.None)
            {
                foreach (StiMetaTag meta in report.MetaTags)
                {
                    if (meta.Name.StartsWith("pdf:"))
                    {
                        string metaName = meta.Name.Substring(4);

                        StringBuilder sbName = new StringBuilder();
                        for (int index = 0; index < metaName.Length; index++)
                        {
                            char ch = metaName[index];
                            if (char.IsLetterOrDigit(ch) || ch == '_')
                            {
                                sbName.Append(ch);
                            }
                            else
                            {
                                sbName.Append("\u2182" + ((int)ch).ToString("X4"));
                            }
                        }
                        metaName = sbName.ToString();

                        if (!reservedKeywords.Contains(metaName))
                        {
                            writer.WriteElementString("pdfx:" + metaName, meta.Tag);
                        }
                    }
                }
            }

            writer.WriteFullEndElement();   //rdf:Description
            #endregion

            #region ZUGFeRD
            if (zugferdComplianceMode != StiPdfZUGFeRDComplianceMode.None)
            {
                #region ZUGFeRD PDFA Extension Schema
                writer.WriteStartElement("rdf:Description");
                writer.WriteAttributeString("xmlns:pdfaExtension", "http://www.aiim.org/pdfa/ns/extension/");
                writer.WriteAttributeString("xmlns:pdfaField", "http://www.aiim.org/pdfa/ns/field#");
                writer.WriteAttributeString("xmlns:pdfaProperty", "http://www.aiim.org/pdfa/ns/property#");
                writer.WriteAttributeString("xmlns:pdfaSchema", "http://www.aiim.org/pdfa/ns/schema#");
                writer.WriteAttributeString("xmlns:pdfaType", "http://www.aiim.org/pdfa/ns/type#");
                writer.WriteAttributeString("rdf:about", "");

                writer.WriteStartElement("pdfaExtension:schemas");

                writer.WriteStartElement("rdf:Bag");

                writer.WriteStartElement("rdf:li");
                writer.WriteAttributeString("rdf:parseType", "Resource");

                if (zugferdComplianceMode == StiPdfZUGFeRDComplianceMode.V1)
                {
                    writer.WriteElementString("pdfaSchema:schema", "ZUGFeRD PDFA Extension Schema");
                    writer.WriteElementString("pdfaSchema:namespaceURI", "urn:ferd:pdfa:CrossIndustryDocument:invoice:1p0#");
                    writer.WriteElementString("pdfaSchema:prefix", "zf");
                }
                if (zugferdComplianceMode == StiPdfZUGFeRDComplianceMode.V2)
                {
                    writer.WriteElementString("pdfaSchema:schema", "ZUGFeRD PDFA Extension Schema");
                    writer.WriteElementString("pdfaSchema:namespaceURI", "urn:zugferd:pdfa:CrossIndustryDocument:invoice:2p0#");
                    writer.WriteElementString("pdfaSchema:prefix", "zf");
                }
                if (zugferdComplianceMode == StiPdfZUGFeRDComplianceMode.V2_1)
                {
                    writer.WriteElementString("pdfaSchema:schema", "Factur-X/ZUGFeRD PDF/A Extension Schema");
                    writer.WriteElementString("pdfaSchema:namespaceURI", "urn:factur-x:pdfa:CrossIndustryDocument:invoice:1p0#");
                    writer.WriteElementString("pdfaSchema:prefix", "fx");
                }

                writer.WriteStartElement("pdfaSchema:property");

                writer.WriteStartElement("rdf:Seq");

                writer.WriteStartElement("rdf:li");
                writer.WriteAttributeString("rdf:parseType", "Resource");
                writer.WriteElementString("pdfaProperty:name", "DocumentFileName");
                writer.WriteElementString("pdfaProperty:valueType", "Text");
                writer.WriteElementString("pdfaProperty:category", "external");
                writer.WriteElementString("pdfaProperty:description", "Name of the embedded XML invoice file");
                writer.WriteFullEndElement();   //rdf:li

                writer.WriteStartElement("rdf:li");
                writer.WriteAttributeString("rdf:parseType", "Resource");
                writer.WriteElementString("pdfaProperty:name", "DocumentType");
                writer.WriteElementString("pdfaProperty:valueType", "Text");
                writer.WriteElementString("pdfaProperty:category", "external");
                writer.WriteElementString("pdfaProperty:description", "INVOICE");
                writer.WriteFullEndElement();   //rdf:li

                writer.WriteStartElement("rdf:li");
                writer.WriteAttributeString("rdf:parseType", "Resource");
                writer.WriteElementString("pdfaProperty:name", "Version");
                writer.WriteElementString("pdfaProperty:valueType", "Text");
                writer.WriteElementString("pdfaProperty:category", "external");
                if (zugferdComplianceMode == StiPdfZUGFeRDComplianceMode.V2_1)
                    writer.WriteElementString("pdfaProperty:description", "Version of the Factur-X/ZUGFeRD XML schema");
                else
                    writer.WriteElementString("pdfaProperty:description", "The actual version of the ZUGFeRD data");
                writer.WriteFullEndElement();   //rdf:li

                writer.WriteStartElement("rdf:li");
                writer.WriteAttributeString("rdf:parseType", "Resource");
                writer.WriteElementString("pdfaProperty:name", "ConformanceLevel");
                writer.WriteElementString("pdfaProperty:valueType", "Text");
                writer.WriteElementString("pdfaProperty:category", "external");
                if (zugferdComplianceMode == StiPdfZUGFeRDComplianceMode.V2_1)
                    writer.WriteElementString("pdfaProperty:description", "Conformance level of the embedded Factur-X/ZUGFeRD XML invoice data");
                else
                    writer.WriteElementString("pdfaProperty:description", "The conformance level of the ZUGFeRD data");
                writer.WriteFullEndElement();   //rdf:li

                writer.WriteFullEndElement();   //rdf:Seq
                writer.WriteFullEndElement();   //pdfaSchema:property
                writer.WriteFullEndElement();   //rdf:li
                writer.WriteFullEndElement();   //rdf:Bag
                writer.WriteFullEndElement();   //pdfaExtension:schemas
                writer.WriteFullEndElement();   //rdf:Description
                #endregion

                writer.WriteStartElement("rdf:Description");
                if (zugferdComplianceMode == StiPdfZUGFeRDComplianceMode.V1)
                {
                    writer.WriteAttributeString("xmlns:zf", "urn:ferd:pdfa:CrossIndustryDocument:invoice:1p0#");
                    writer.WriteAttributeString("zf:DocumentFileName", "ZUGFeRD-invoice.xml");
                    writer.WriteAttributeString("zf:ConformanceLevel", zugferdConformanceLevel);
                    writer.WriteAttributeString("zf:DocumentType", "INVOICE");
                    writer.WriteAttributeString("zf:Version", "1.0");
                }
                if (zugferdComplianceMode == StiPdfZUGFeRDComplianceMode.V2)
                {
                    writer.WriteAttributeString("xmlns:zf", "urn:zugferd:pdfa:CrossIndustryDocument:invoice:2p0#");
                    writer.WriteAttributeString("zf:DocumentFileName", "zugferd-invoice.xml");
                    writer.WriteAttributeString("zf:ConformanceLevel", zugferdConformanceLevel);
                    writer.WriteAttributeString("zf:DocumentType", "INVOICE");
                    writer.WriteAttributeString("zf:Version", "1.0");
                }
                if (zugferdComplianceMode == StiPdfZUGFeRDComplianceMode.V2_1)
                {
                    writer.WriteAttributeString("xmlns:fx", "urn:factur-x:pdfa:CrossIndustryDocument:invoice:1p0#");
                    writer.WriteAttributeString("fx:DocumentFileName", "factur-x.xml");
                    writer.WriteAttributeString("fx:ConformanceLevel", zugferdConformanceLevel);
                    writer.WriteAttributeString("fx:DocumentType", "INVOICE");
                    writer.WriteAttributeString("fx:Version", "1.0");
                }
                writer.WriteAttributeString("rdf:about", "");
                writer.WriteFullEndElement();   //rdf:Description
            }
            #endregion

            #region PDF/A
            if (usePdfA)
            {
                writer.WriteStartElement("rdf:Description");
                writer.WriteAttributeString("rdf:about", "");
                writer.WriteAttributeString("xmlns:pdfaid", "http://www.aiim.org/pdfa/ns/id/");
                string pdfaidPart = "1";
                switch (pdfComplianceMode)
                {
                    case StiPdfComplianceMode.A1: pdfaidPart = "1"; break;
                    case StiPdfComplianceMode.A2: pdfaidPart = "2"; break;
                    case StiPdfComplianceMode.A3: pdfaidPart = "3"; break;
                }
                string pdfaidConformance = "A";
                switch (zugferdComplianceMode)
                {
                    case StiPdfZUGFeRDComplianceMode.V1: pdfaidConformance = "A"; break;
                    case StiPdfZUGFeRDComplianceMode.V2: pdfaidConformance = "B"; break;
                    case StiPdfZUGFeRDComplianceMode.V2_1: pdfaidConformance = "B"; break;
                }
                writer.WriteElementString("pdfaid:part", pdfaidPart);
                writer.WriteElementString("pdfaid:conformance", pdfaidConformance);
                writer.WriteFullEndElement();   //rdf:Description
            }
            #endregion

            writer.WriteFullEndElement();   //rdf:RD
            writer.WriteFullEndElement();   //x:xmpmeta
            writer.WriteRaw("                              \r\n");
            writer.WriteRaw("                              \r\n");
            writer.WriteRaw("                              \r\n");
            writer.WriteRaw("                              \r\n");
            writer.WriteRaw("                              \r\n");
            writer.WriteProcessingInstruction("xpacket", "end=\"w\"");
            #endregion

            writer.Flush();
            byte[] tempBuffer = ms.ToArray();
            ms.Close();
            writer.Close();
            mainStream.Write(tempBuffer, 3, tempBuffer.Length - 3);
            #endregion

            AddXref(info.Metadata.Ref);
            sw.WriteLine("{0} 0 obj", info.Metadata.Ref);
            sw.WriteLine("<<");
            sw.WriteLine("/Type /Metadata");
            sw.WriteLine("/Subtype /XML");

            //sw.WriteLine("/Length {0}", mainStream.Length);
            //sw.WriteLine(">>");
            //sw.WriteLine("stream");
            //StoreMemoryStream(mainStream);
            StoreMemoryStream2(mainStream, "/Length {0}");

            sw.WriteLine("");
            sw.WriteLine("endstream");
            sw.WriteLine("endobj");
            sw.WriteLine("");
        }
        #endregion

        #region Render ColorSpace
        private void RenderColorSpace()
        {
            AddXref(info.DestOutputProfile.Ref);
            sw.WriteLine("{0} 0 obj", info.DestOutputProfile.Ref);
            sw.WriteLine("<<");
            sw.WriteLine("/N 3");
            MemoryStream TmpStream = null;
            string stColorSpace = null;
            if (compressed == true)
            {
                TmpStream = StiExportUtils.MakePdfDeflateStream(sRGBprofile);
                //sw.WriteLine("/Length {0} /Filter [/FlateDecode] /Length1 {1}", TmpStream.Length, sRGBprofile.Length);
                stColorSpace = "/Length {0} /Filter [/FlateDecode] /Length1 " + sRGBprofile.Length.ToString();
            }
            else
            {
                TmpStream = new MemoryStream();
#if TESTPDF
                    byte[] buf = new byte[sRGBprofile.Length * 2];
                    for (int indexb = 0; indexb < sRGBprofile.Length; indexb++)
                    {
                        string st = string.Format("{0:X2}", sRGBprofile[indexb]);
                        buf[indexb * 2 + 0] = (byte)st[0];
                        buf[indexb * 2 + 1] = (byte)st[1];
                    }
                    TmpStream.Write(buf, 0, buf.Length);
                    //sw.WriteLine("/Length {0}", sRGBprofile.Length * 2);
                    //sw.WriteLine("/Filter [/ASCIIHexDecode]");
                    stColorSpace = "/Length {0} /Filter [/ASCIIHexDecode]";
#else
                TmpStream.Write(sRGBprofile, 0, sRGBprofile.Length);
                //sw.WriteLine("/Length {0}", sRGBprofile.Length);
                stColorSpace = "/Length {0}";
#endif
            }
            //sw.WriteLine(">>");
            //sw.WriteLine("stream");
            //StoreMemoryStream(TmpStream);
            StoreMemoryStream2(TmpStream, stColorSpace);

            sw.WriteLine("");
            sw.WriteLine("endstream");
            sw.WriteLine("endobj");
            sw.WriteLine("");


            AddXref(info.OutputIntents.Ref);
            sw.WriteLine("{0} 0 obj", info.OutputIntents.Ref);
            sw.WriteLine("[<<");

            //sw.WriteLine("/Info(sRGB IEC61966-2.1)");
            //sw.WriteLine("/OutputConditionIdentifier(Custom)");
            //sw.WriteLine("/OutputCondition()");
            //sw.WriteLine("/RegistryName()");
            StoreStringLine("/Info", "sRGB IEC61966-2.1");
            StoreStringLine("/OutputConditionIdentifier", "Custom");
            StoreStringLine("/OutputCondition", string.Empty);
            StoreStringLine("/RegistryName", string.Empty);

            sw.WriteLine("/S /GTS_PDFA1");
            sw.WriteLine("/DestOutputProfile {0} 0 R", info.DestOutputProfile.Ref);
            sw.WriteLine("/Type /OutputIntent");
            sw.WriteLine(">>]");
            sw.WriteLine("endobj");
            sw.WriteLine("");
        }
        #endregion

        #region Render AutoPrint
        private void RenderAutoPrint()
        {
            if (autoPrint != StiPdfAutoPrintMode.None)
            {
                AddXref(info.EmbeddedJS.Ref);
                sw.WriteLine("{0} 0 obj", info.EmbeddedJS.Ref);
                sw.WriteLine("<<");
                sw.WriteLine("/Names [(EmbeddedJS) {0} 0 R]", info.EmbeddedJS.Content.Ref);
                sw.WriteLine(">>");
                sw.WriteLine("endobj");
                sw.WriteLine("");

                AddXref(info.EmbeddedJS.Content.Ref);
                sw.WriteLine("{0} 0 obj", info.EmbeddedJS.Content.Ref);
                sw.WriteLine("<<");
                sw.WriteLine("/S /JavaScript");
                sw.WriteLine("/JS (print\\({0}\\);)", autoPrint == StiPdfAutoPrintMode.Dialog ? "true" : "false");
                //if dialog
                //sw.WriteLine("/JS (var pp = getPrintParams\\(\\); pp.printerName = '\\\\\\\\serverName\\\\printerName'; pp.interactive = pp.constants.interactionLevel.full; print\\(pp\\);)");
                //else
                //sw.WriteLine("/JS (var pp = getPrintParams\\(\\); pp.printerName = '\\\\\\\\serverName\\\\printerName'; pp.interactive = pp.constants.interactionLevel.automatic; print\\(pp\\);)");
                sw.WriteLine(">>");
                sw.WriteLine("endobj");
                sw.WriteLine("");
            }
        }
        #endregion

        #region Render EmbeddedFiles
        private void RenderEmbeddedFiles()
        {
            for (int index = 0; index < info.EmbeddedFilesList.Count; index++)
            {
                var file = embeddedFiles[index];

                AddXref(info.EmbeddedFilesList[index].Ref);
                sw.WriteLine("{0} 0 obj", info.EmbeddedFilesList[index].Ref);
                sw.WriteLine("<<");
                StoreStringLine("/F", file.Name, true);
                sw.WriteLine("/Type /Filespec");
                sw.WriteLine("/EF <<");
                sw.WriteLine("/F {0} 0 R", info.EmbeddedFilesList[index].Content.Ref);
                //sw.WriteLine("/UF {0} 0 R", info.EmbeddedFilesList[index].Content.Ref);
                sw.WriteLine(">>");
                if (zugferdComplianceMode != StiPdfZUGFeRDComplianceMode.None || pdfComplianceMode == StiPdfComplianceMode.A3)
                {
                    sw.WriteLine("/AFRelationship /Alternative");
                }
                StoreStringLine("/UF", file.Name, true);
                StoreStringLine("/Desc", file.Description, true);
                sw.WriteLine(">>");
                sw.WriteLine("endobj");
                sw.WriteLine("");

                AddXref(info.EmbeddedFilesList[index].Content.Ref);
                sw.WriteLine("{0} 0 obj", info.EmbeddedFilesList[index].Content.Ref);
                sw.WriteLine("<<");
                sw.WriteLine("/Subtype /{0}", CorrectNameEnconding(file.MIMEType));
                sw.WriteLine("/Type /EmbeddedFile");
                sw.WriteLine("/Params <<");
                StoreStringLine("/ModDate ", "D:" + currentDateTime);
                sw.WriteLine("/Size {0}", file.Data.Length);
                sw.WriteLine(">>");
                if (compressed == true)
                {
                    MemoryStream TmpStream = StiExportUtils.MakePdfDeflateStream(file.Data);
                    StoreMemoryStream2(TmpStream, "/Filter [/FlateDecode] /Length {0}");
                }
                else
                {
                    StoreMemoryStream2(file.Data, "/Length {0}");
                }
                sw.WriteLine("");
                sw.WriteLine("endstream");
                sw.WriteLine("endobj");
                sw.WriteLine("");
            }
        }
        #endregion

        #endregion

        #region StiBookmarkTreeNode
        private class StiBookmarkTreeNode
        {
            public int Parent;
            public int First;
            public int Last;
            public int Prev;
            public int Next;
            public int Count;
            public int Page;
            public double Y;
            public string Title;
            public string Guid;
            public bool Used;
        }
        #endregion

        #region AddBookmarkNode
        private void AddBookmarkNode(StiBookmark bkm, int parentNode)
        {
            StiBookmarkTreeNode tn = new StiBookmarkTreeNode();
            tn.Parent = -1;
            tn.First = -1;
            tn.Last = -1;
            tn.Prev = -1;
            tn.Next = -1;
            tn.Count = -1;
            tn.Page = -1;
            tn.Y = -1;
            tn.Title = string.Empty;
            tn.Guid = string.Empty;
            tn.Used = (parentNode == -1 ? true : false);
            bookmarksTree.Add(tn);
            int currentNode = bookmarksTree.Count - 1;

            tn.Parent = parentNode;
            tn.Title = bkm.Text;
            tn.Guid = bkm.ComponentGuid;
            if (bkm.Bookmarks.Count == 0)
            {
                tn.Count = 0;
            }
            else
            {
                int prevNode = -1;
                for (int tempCount = 0; tempCount <= bkm.Bookmarks.Count - 1; tempCount++)
                {
                    int memNode = bookmarksTree.Count;
                    StiBookmark sbm = bkm.Bookmarks[tempCount];
                    AddBookmarkNode(sbm, currentNode);
                    StiBookmarkTreeNode tempBM = (StiBookmarkTreeNode)bookmarksTree[memNode];
                    if (tempCount < bkm.Bookmarks.Count - 1)
                    {
                        tempBM.Next = bookmarksTree.Count;
                    }
                    if (tempCount > 0)
                    {
                        tempBM.Prev = prevNode;
                    }
                    bookmarksTree[memNode] = tempBM;
                    prevNode = memNode;
                }
                tn.First = currentNode + 1;
                tn.Last = bookmarksTree.Count - 1;
                tn.Count = bookmarksTree.Count - currentNode - 1;
            }
            bookmarksTree[currentNode] = tn;
        }
        #endregion

        #region MakeBookmarkFromTree
        private void MakeBookmarkFromTree(StiBookmark bookmark, StiBookmarkTreeNode treeNode)
        {
            bookmark.Text = treeNode.Title;
            bookmark.Bookmarks = new StiBookmarksCollection();
            if (treeNode.Count > 0)
            {
                int nodePos = treeNode.First;
                while (nodePos != -1)
                {
                    StiBookmarkTreeNode tnChild = (StiBookmarkTreeNode)bookmarksTree[nodePos];
                    if (tnChild.Used)
                    {
                        StiBookmark childBookmark = new StiBookmark();
                        bookmark.Bookmarks.Add(childBookmark);
                        MakeBookmarkFromTree(childBookmark, tnChild);
                    }
                    nodePos = tnChild.Next;
                }
            }
        }
        #endregion

        #region StiLinkObject
        private struct StiLinkObject
        {
            public double X;
            public double Y;
            public double Width;
            public double Height;
            public int Page;
            public int DestPage;
            public double DestY;
            public string Link;
        }
        #endregion

        #region StiEditableObject
        private class StiEditableObject
        {
            public double X;
            public double Y;
            public double Width;
            public double Height;
            public int Page;
            public string Text;
            public byte[] Content;
            public byte[] Content2;
            public bool Multiline;
            public StiTextHorAlignment Alignment;
            public int FontNumber;
            public double FontSize;
            public Color FontColor;
            public StiComponent Component;
        }
        #endregion

        #region Wpf text event
        public static event Events.StiSplitTextIntoLinesEventHandler SplitTextIntoLinesEvent;

        private static object lockSplitTextIntoLinesEventFlag = new object();

        protected virtual string OnSplitTextIntoLines(Stimulsoft.Report.Events.StiSplitTextIntoLinesEventArgs e)
        {
            if (SplitTextIntoLinesEvent != null)
            {
                return SplitTextIntoLinesEvent(this, e);
            }
            return null;
        }

        private void PreparePdfTextHelper()
        {
            isWpf = false;
            if (isWpfException) return;
            try
            {
                lock (lockSplitTextIntoLinesEventFlag)
                {
                    if (SplitTextIntoLinesEvent == null)
                    {
                        object stiPdfTextHelper = StiActivator.CreateObject("Stimulsoft.Report.Wpf.Helpers.StiPdfTextHelper, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo, new object[0]);
                    }
                }
                if (SplitTextIntoLinesEvent != null)
                {
                    isWpf = true;
                }
            }
            catch
            {
                isWpf = false;
                isWpfException = true;
            }
        }
        #endregion

        public string GetCreatorString()
        {
            if (!string.IsNullOrEmpty(StiOptions.Export.Pdf.CreatorString)) return StiOptions.Export.Pdf.CreatorString;
            return StiExportUtils.GetReportVersion(report);
        }

        public void ClearFontsCache()
        {
            PdfFonts.ClearFontsCache();
        }

        public void InitPdfFonts()
        {
            pdfFont = new PdfFonts();
        }

        #region Main Export Class
        /// <summary>
        /// Exports a document to the pdf file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="fileName">A name of the file for exporting a rendered report.</param>
        public void ExportPdf(StiReport report, string fileName)
        {
            StiFileUtils.ProcessReadOnly(fileName);
            FileStream stream = new FileStream(fileName, FileMode.Create);
            ExportPdf(report, stream);
            stream.Flush();
            stream.Close();
        }


        /// <summary>
        /// Exports a document to the pdf file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream for the export of a document.</param>
        public void ExportPdf(StiReport report, Stream stream)
        {
            StiPdfExportSettings settings = new StiPdfExportSettings();
            ExportPdf(report, stream, settings);
        }


        /// <summary>
        /// Exports a document to the pdf file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream for the export of a document.</param>
        /// <param name="pageRange">Describes range of pages of the document for the export.</param>
        public void ExportPdf(StiReport report, Stream stream, StiPagesRange pageRange)
        {
            StiPdfExportSettings settings = new StiPdfExportSettings();

            settings.PageRange = pageRange;

            ExportPdf(report, stream, settings);
        }


        /// <summary>
        /// Exports a document to the pdf file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream for the export of a document.</param>
        /// <param name="pageRange">Describes range of pages of the document for the export.</param>
        /// <param name="imageQuality">A float value that sets the quality of exporting images. Default value is 1.</param>
        /// <param name="imageResolution">A float value that sets the resolution of exporting images. Default value is 100.</param>
        public void ExportPdf(StiReport report, Stream stream, StiPagesRange pageRange,
            float imageQuality, float imageResolution)
        {
            StiPdfExportSettings settings = new StiPdfExportSettings();

            settings.PageRange = pageRange;
            settings.ImageQuality = imageQuality;
            settings.ImageResolution = imageResolution;

            ExportPdf(report, stream, settings);
        }


        /// <summary>
        /// Exports a document to the pdf file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream for export of a document.</param>
        /// <param name="pageRange">Describes pages range of the document for the export.</param>
        /// <param name="imageQuality">A float value that sets the quality of exporting images. Default value is 1.</param>
        /// <param name="imageResolution">A float value that sets the resolution of exporting images. Default value is 100.</param>
        /// <param name="embeddedFonts">If embeddedFont is true then, when exporting, fonts of the report will be included in the resulting document.</param>
        /// <param name="standardPdfFonts">If standardPdfFont is true then, when exporting, non-standard fonts of the report will be replaced by the standard fonts in resulting document.</param>
        /// <param name="compressed">A parameter which controls a compression of the exported pdf document.</param>
        public void ExportPdf(StiReport report, Stream stream, StiPagesRange pageRange,
            float imageQuality, float imageResolution, bool embeddedFonts, bool standardPdfFonts, bool compressed)
        {
            StiPdfExportSettings settings = new StiPdfExportSettings();

            settings.PageRange = pageRange;
            settings.ImageQuality = imageQuality;
            settings.ImageResolution = imageResolution;
            settings.EmbeddedFonts = embeddedFonts;
            settings.StandardPdfFonts = standardPdfFonts;
            settings.Compressed = compressed;

            ExportPdf(report, stream, settings);
        }


        /// <summary>
        /// Exports a document to the pdf file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream for export of a document.</param>
        /// <param name="pageRange">Describes range of pages of the document for the export.</param>
        /// <param name="imageQuality">A float value that sets the quality of exporting images. Default value is 1.</param>
        /// <param name="imageResolution">A float value that sets the resolution of exporting images. Default value is 100.</param>
        /// <param name="embeddedFonts">If embeddedFont is true then, when exporting, fonts of the report will be included in the resulting document.</param>
        /// <param name="standardPdfFonts">If standardPdfFont is true then, when exporting, non-standard fonts of the report will be replaced by the standard fonts in resulting document.</param>
        /// <param name="compressed">A parameter which controls a compression of the exported pdf document.</param>
        /// <param name="passwordInputUser">An user password for the exported pdf file which enables access to content of the document
        /// in according with the privileges from the userAccesPrivileges parameter.</param>
        /// <param name="passwordInputOwner">An owner password which supplies full control for the content of the exported pdf file.</param>
        /// <param name="userAccessPrivileges">A parameter which controls access privileges for the user.</param>
        public void ExportPdf(StiReport report, Stream stream, StiPagesRange pageRange,
            float imageQuality, float imageResolution, bool embeddedFonts, bool standardPdfFonts,
            bool compressed, string passwordInputUser, string passwordInputOwner,
            StiUserAccessPrivileges userAccessPrivileges)
        {
            StiPdfExportSettings settings = new StiPdfExportSettings();

            settings.PageRange = pageRange;
            settings.ImageQuality = imageQuality;
            settings.ImageResolution = imageResolution;
            settings.EmbeddedFonts = embeddedFonts;
            settings.StandardPdfFonts = standardPdfFonts;
            settings.Compressed = compressed;
            settings.PasswordInputUser = passwordInputUser;
            settings.PasswordInputOwner = passwordInputOwner;
            settings.UserAccessPrivileges = userAccessPrivileges;

            ExportPdf(report, stream, settings);
        }


        /// <summary>
        /// Exports a rendered report to the pdf file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream for the export of a document.</param>
        /// <param name="pageRange">Describes range of pages of the document for the export.</param>
        /// <param name="imageQuality">A float value that sets the quality of exporting images. Default value is 1.</param>
        /// <param name="imageResolution">A float value that sets the resolution of exporting images. Default value is 100.</param>
        /// <param name="embeddedFonts">If the embeddedFont is true then, when exporting, fonts of the report will be included in the resulting document.</param>
        /// <param name="standardPdfFonts">If standardPdfFont is true then, when exporting, non-standard fonts of the report will be replaced by the standard fonts in resulting document.</param>
        /// <param name="compressed">A parameter which controls a compression of the exported pdf document.</param>
        /// <param name="passwordInputUser">An user password for the exported pdf file which enables access to content of the document
        /// in according with the privileges from the userAccesPrivileges parameter.</param>
        /// <param name="passwordInputOwner">An owner password which supplies full control for the content of the exported pdf file.</param>
        /// <param name="userAccessPrivileges">A parameter which controls access privileges for the user.</param>
        /// <param name="keyLength">A parameter for setting an encryption key lenght of the resulting pdf file.</param>
        public void ExportPdf(StiReport report, Stream stream, StiPagesRange pageRange,
            float imageQuality, float imageResolution, bool embeddedFonts, bool standardPdfFonts,
            bool compressed, string passwordInputUser, string passwordInputOwner, StiUserAccessPrivileges userAccessPrivileges,
            StiPdfEncryptionKeyLength keyLength)
        {
            StiPdfExportSettings settings = new StiPdfExportSettings();

            settings.PageRange = pageRange;
            settings.ImageQuality = imageQuality;
            settings.ImageResolution = imageResolution;
            settings.EmbeddedFonts = embeddedFonts;
            settings.StandardPdfFonts = standardPdfFonts;
            settings.Compressed = compressed;
            settings.PasswordInputUser = passwordInputUser;
            settings.PasswordInputOwner = passwordInputOwner;
            settings.UserAccessPrivileges = userAccessPrivileges;
            settings.KeyLength = keyLength;

            ExportPdf(report, stream, settings);
        }


        /// <summary>
        /// Exports a document to the pdf file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream for the export of a document.</param>
        /// <param name="pageRange">Describes range of pages of the document for the export.</param>
        /// <param name="imageQuality">A float value that sets the quality of exporting images. Default value is 1.</param>
        /// <param name="imageResolution">A float value that sets the resolution of exporting images. Default value is 100.</param>
        /// <param name="embeddedFonts">If embeddedFont is true then, when exporting, fonts of the report will be included in the resulting document.</param>
        /// <param name="standardPdfFonts">If standardPdfFont is true then, when exporting, non-standard fonts of the report will be replaced by the standard fonts in resulting document.</param>
        /// <param name="compressed">A parameter which controls a compression of the exported pdf document.</param>
        /// <param name="exportRtfTextAsImage">If true then the rendered report will be exported as one image.</param>
        /// <param name="passwordInputUser">A user password for the exported Pdf file which enables access to content of the document
        /// in according with the privileges from the userAccesPrivileges parameter.</param>
        /// <param name="passwordInputOwner">An owner password which supplies full control for the content of the exported pdf file.</param>
        /// <param name="userAccessPrivileges">A parameter which controls access privileges for the user.</param>
        /// <param name="keyLength">A parameter for setting an encryption key length of the resulting pdf file.</param>
        public void ExportPdf(StiReport report, Stream stream, StiPagesRange pageRange,
            float imageQuality, float imageResolution, bool embeddedFonts, bool standardPdfFonts,
            bool compressed, bool exportRtfTextAsImage,
            string passwordInputUser, string passwordInputOwner, StiUserAccessPrivileges userAccessPrivileges,
            StiPdfEncryptionKeyLength keyLength)
        {
            StiPdfExportSettings settings = new StiPdfExportSettings();

            settings.PageRange = pageRange;
            settings.ImageQuality = imageQuality;
            settings.ImageResolution = imageResolution;
            settings.EmbeddedFonts = embeddedFonts;
            settings.StandardPdfFonts = standardPdfFonts;
            settings.Compressed = compressed;
            settings.ExportRtfTextAsImage = exportRtfTextAsImage;
            settings.PasswordInputUser = passwordInputUser;
            settings.PasswordInputOwner = passwordInputOwner;
            settings.UserAccessPrivileges = userAccessPrivileges;
            settings.KeyLength = keyLength;

            ExportPdf(report, stream, settings);
        }


        public void ExportPdf(StiReport report, Stream stream, StiPagesRange pageRange,
            float imageQuality, float imageResolution, bool embeddedFonts, bool standardPdfFonts,
            bool compressed, bool exportRtfTextAsImage,
            string passwordInputUser, string passwordInputOwner, StiUserAccessPrivileges userAccessPrivileges,
            StiPdfEncryptionKeyLength keyLength, bool useUnicode)
        {
            StiPdfExportSettings settings = new StiPdfExportSettings();

            settings.PageRange = pageRange;
            settings.ImageQuality = imageQuality;
            settings.ImageResolution = imageResolution;
            settings.EmbeddedFonts = embeddedFonts;
            settings.StandardPdfFonts = standardPdfFonts;
            settings.Compressed = compressed;
            settings.ExportRtfTextAsImage = exportRtfTextAsImage;
            settings.PasswordInputUser = passwordInputUser;
            settings.PasswordInputOwner = passwordInputOwner;
            settings.UserAccessPrivileges = userAccessPrivileges;
            settings.KeyLength = keyLength;
            settings.UseUnicode = useUnicode;

            ExportPdf(report, stream, settings);
        }


        public void ExportPdf(StiReport report, Stream stream, StiPdfExportSettings settings)
        {
            #region Export Dashboard
            if (!report.IsDocument && report.GetCurrentPage() is IStiDashboard)
            {
                StiDashboardExport.Export(report, stream, settings);
                return;
            }
            #endregion

            try
            {
                //StiExportUtils.DisableFontSmoothing();
                ExportPdf1(report, stream, settings);
            }
            finally
            {
                StiExportUtils.EnableFontSmoothing(report);
            }
        }

        /// <summary>
        /// Exports a rendered report to the pdf file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream for the export of a document.</param>
        /// <param name="settings">Pdf export settings.</param>
		private void ExportPdf1(StiReport report, Stream stream, StiPdfExportSettings settings)
        {
            StiLogService.Write(this.GetType(), "Export report to Pdf format");
            this.report = report;

#if NETSTANDARD || NETCOREAPP
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif

            #region Read settings
            if (settings == null)
                throw new ArgumentNullException("The 'settings' argument cannot be equal in null.");

            StiPagesRange pageRange = settings.PageRange;
            float imageQuality = settings.ImageQuality;
            float imageResolution = settings.ImageResolution;
            this.imageResolutionMode = settings.ImageResolutionMode;
            bool embeddedFonts = settings.EmbeddedFonts;
            bool standardPdfFonts = settings.StandardPdfFonts;
            bool compressed = settings.Compressed;
            bool exportRtfTextAsImage = settings.ExportRtfTextAsImage;
            string passwordInputUser = settings.PasswordInputUser;
            string passwordInputOwner = settings.PasswordInputOwner;
            StiUserAccessPrivileges userAccessPrivileges = settings.UserAccessPrivileges;
            this.keyLength = settings.KeyLength;
            this.useUnicodeMode = settings.UseUnicode;
            this.pdfComplianceMode = settings.PdfComplianceMode;
            bool getCertificateFromCryptoUI = settings.GetCertificateFromCryptoUI;
            bool useDigitalSignature = settings.UseDigitalSignature;
            string subjectNameString = settings.SubjectNameString;
            bool useLocalMachineCertificates = settings.UseLocalMachineCertificates;
            this.imageFormat = settings.ImageFormat;
            this.monochromeDitheringType = settings.DitheringType;
            this.autoPrint = settings.AutoPrintMode;
            StiPdfImageCompressionMethod imageCompressionMethod = settings.ImageCompressionMethod;
            byte[] certificateData = settings.CertificateData;
            string certificatePassword = settings.CertificatePassword;
            string certificateThumbprint = settings.CertificateThumbprint;
            this.allowEditable = settings.AllowEditable;
            this.digitalSignatureReason = settings.DigitalSignatureReason;
            this.digitalSignatureLocation = settings.DigitalSignatureLocation;
            this.digitalSignatureContactInfo = settings.DigitalSignatureContactInfo;
            this.digitalSignatureSignedBy = settings.DigitalSignatureSignedBy;
            this.embeddedFiles = settings.EmbeddedFiles;
            this.zugferdComplianceMode = settings.ZUGFeRDComplianceMode;
            this.zugferdConformanceLevel = settings.ZUGFeRDConformanceLevel;
            byte[] zugferdInvoiceData = settings.ZUGFeRDInvoiceData;
            int imageIndexedColorPaletteSize = settings.ImageIndexedColorPaletteSize;

            creatorName = GetCreatorString();
            keywords = StiOptions.Export.Pdf.KeywordsString;
            if (!string.IsNullOrEmpty(settings.CreatorString))
            {
                creatorName = settings.CreatorString;
            }
            if (!string.IsNullOrEmpty(settings.KeywordsString))
            {
                keywords = settings.KeywordsString;
            }
            if (string.IsNullOrEmpty(creatorName))
            {
                creatorName = producerName;
            }
            #endregion

            #region Report resources to embedded files
            if (embeddedFiles == null)
                embeddedFiles = new List<StiPdfEmbeddedFileData>();
            if (report.Dictionary.Resources != null)
            {
                foreach (StiResource resource in report.Dictionary.Resources)
                {
                    if (resource.AvailableInTheViewer)
                    {
                        var description = resource.Alias != resource.Name ? resource.Alias : string.Empty;
                        var embeddedFile = new StiPdfEmbeddedFileData(resource.Name + resource.GetFileExt(), description, resource.Content, resource.GetContentType());
                        embeddedFiles.Add(embeddedFile);
                    }
                    if (resource.Type == StiResourceType.Xml)
                    {
                        if (resource.Name == "ZUGFeRD-invoice")
                        {
                            zugferdComplianceMode = StiPdfZUGFeRDComplianceMode.V1;
                            embeddedFiles.Add(new StiPdfEmbeddedFileData("ZUGFeRD-invoice.xml", "ZUGFeRD Invoice", resource.Content, "text/xml"));
                        }
                        else if (resource.Name.ToLower() == "zugferd-invoice")
                        {
                            zugferdComplianceMode = StiPdfZUGFeRDComplianceMode.V2;
                            embeddedFiles.Add(new StiPdfEmbeddedFileData("zugferd-invoice.xml", "ZUGFeRD Invoice", resource.Content, "text/xml"));
                        }
                        else if (resource.Name.ToLower() == "factur-x")
                        {
                            zugferdComplianceMode = StiPdfZUGFeRDComplianceMode.V2_1;
                            embeddedFiles.Add(new StiPdfEmbeddedFileData("factur-x.xml", "Factur-X/ZUGFeRD", resource.Content, "text/xml"));
                        }
                    }
                }
            }
            #endregion

            #region ZUGFeRD
            if (zugferdComplianceMode != StiPdfZUGFeRDComplianceMode.None)
            {
                pdfComplianceMode = StiPdfComplianceMode.A3;
                if (zugferdInvoiceData != null)
                {
                    var founded = false;
                    foreach (var file in embeddedFiles)
                    {
                        string fileName = file.Name.ToLowerInvariant();
                        if (fileName == "zugferd-invoice.xml" || fileName == "factur-x.xml") founded = true;
                    }
                    if (!founded)
                    {
                        if (zugferdComplianceMode == StiPdfZUGFeRDComplianceMode.V1)
                        {
                            embeddedFiles.Add(new StiPdfEmbeddedFileData("ZUGFeRD-invoice.xml", "ZUGFeRD Rechnung", zugferdInvoiceData));
                        }
                        if (zugferdComplianceMode == StiPdfZUGFeRDComplianceMode.V2)
                        {
                            embeddedFiles.Add(new StiPdfEmbeddedFileData("zugferd-invoice.xml", "ZUGFeRD Rechnung", zugferdInvoiceData));
                        }
                        if (zugferdComplianceMode == StiPdfZUGFeRDComplianceMode.V2_1)
                        {
                            embeddedFiles.Add(new StiPdfEmbeddedFileData("factur-x.xml", "Factur-X/ZUGFeRD", zugferdInvoiceData));
                        }
                    }
                }
            }
            #endregion

            #region Sort EmbeddedFiles
            if (embeddedFiles.Count > 1)
            {
                embeddedFiles = embeddedFiles.OrderBy(ef => ef.Name).ToList();
            }
            #endregion

            #region Process settings
            this.usePdfA = pdfComplianceMode != StiPdfComplianceMode.None;
            this.useTransparency = pdfComplianceMode != StiPdfComplianceMode.A1;

            if (!embeddedFonts) useUnicodeMode = false;

            compressedFonts = true;
            if (usePdfA)
            {
                standardPdfFonts = false;
                embeddedFonts = true;
                this.useUnicodeMode = true;
            }

            bool deleteEmptyBookmarks = (pageRange.RangeType != StiRangeType.All);

            this.reduceFontSize = StiOptions.Export.Pdf.ReduceFontFileSize;
            bool useImageComparer = StiOptions.Export.Pdf.AllowImageComparer;
            bool useImageTransparency = StiOptions.Export.Pdf.AllowImageTransparency && useTransparency;

#pragma warning disable 612, 618
            if (StiOptions.Export.Pdf.AllowEditablePdf)
#pragma warning restore 612, 618
            {
                allowEditable = StiPdfAllowEditable.Yes;
            }

            //if (report.RenderedPages.CacheMode)StatusString = StiLocalization.Get("Export", "ExportingCreatingDocument") + " 1";
            //else StatusString = StiLocalization.Get("Export", "ExportingCreatingDocument");
            StatusString = StiLocalization.Get("Report", "PreparingReport");

            if (embeddedFonts) standardPdfFonts = false;
            bool clipText = true;
            if (imageQuality < 0) imageQuality = 0;
            if (imageQuality > 1) imageQuality = 1;
            if (imageResolution < 10) imageResolution = 10;
            if (useUnicodeMode) standardPdfFonts = false;

            bool fullTrust = true;
            if (!StiOptions.Engine.FullTrust)
            {
                //embeddedFonts = false;
                //standardPdfFonts = true;
                useUnicodeMode = false;
                useDigitalSignature = false;
                fullTrust = false;
            }

            if (!StiOptions.Export.Pdf.AllowInvokeWindowsLibraries)
            {
                standardPdfFonts = false;
                useDigitalSignature = false;
                //embeddedFonts = false;
                //useUnicodeMode = false;
            }

            this.imageQuality = imageQuality;
            this.embeddedFonts = embeddedFonts;
            this.standardPdfFonts = standardPdfFonts;
            this.compressed = compressed;
            this.exportRtfTextAsImage = exportRtfTextAsImage;

            if (imageFormat == StiImageFormat.Monochrome) imageCompressionMethod = StiPdfImageCompressionMethod.Flate;
            this.imageCompressionMethod = imageCompressionMethod;
            imageResolution = imageResolution / 100;
            imageResolutionMain = imageResolution;
            this.haveDigitalSignature = false;
            #endregion

            this.isWpf = false;
            if (report.IsWpf || report.RenderedWith == StiRenderedWith.Wpf)
            {
                PreparePdfTextHelper();
            }

            #region Initialization
            //bidi = new StiBidirectionalConvert(StiBidirectionalConvert.Mode.Pdf);
            pdfFont = new PdfFonts();
            pdfFont.standardPdfFonts = standardPdfFonts;
            pdfFont.forceUseUnicode = useUnicodeMode;
            fontGlyphsReduceNotNeed = null;

            precision_digits = StiOptions.Export.Pdf.DefaultCoordinatesPrecision;
            clipLongTextLines = clipText;

            //prepare procedure ConvertToString()
            currentNumberFormat = (new CultureInfo("en-US", false)).NumberFormat;
            currentNumberFormat.NumberDecimalSeparator = ".";

            //prepare color convert table
            int colorDigits = 3;
            for (int indexColor = 0; indexColor <= 255; indexColor++)
            {
                double doubleColor = Math.Round((double)((double)indexColor / 255), colorDigits);
                string stringColor = doubleColor.ToString("G", currentNumberFormat);
                colorTable[indexColor] = stringColor;
                alphaTable[indexColor] = false;
            }
            lastColorStrokeA = 0xFF;
            lastColorNonStrokeA = 0xFF;
            colorStack = new Stack(32);

            imageList = new ArrayList();
            switch (imageCompressionMethod)
            {
                case StiPdfImageCompressionMethod.Flate:
                    imageCache = new StiImageCache(useImageComparer, ImageFormat.MemoryBmp, imageQuality, useImageTransparency, 0);
                    break;

                case StiPdfImageCompressionMethod.Indexed:
                    imageCache = new StiImageCache(useImageComparer, ImageFormat.MemoryBmp, imageQuality, useImageTransparency, imageIndexedColorPaletteSize);
                    break;

                default:
                    imageCache = new StiImageCache(useImageComparer, ImageFormat.Jpeg, imageQuality, useImageTransparency, 0);
                    break;
            }
            imageInterpolationTable = new Hashtable();
            imageCacheIndexToList = new Hashtable();
            imageInfoList = new Hashtable();
            imageInfoCounter = 0;

            watermarkImageExist = new Hashtable();

            pdfFont.fontList = new ArrayList();
            xref = new SortedList();
            bookmarksTree = new ArrayList();
            haveBookmarks = false;
            linksArray = new ArrayList();
            annotsArray = new ArrayList();
            annots2Array = new List<StiEditableObject>();
            unsignedSignaturesArray = new List<StiEditableObject>();
            shadingArray = new ArrayList();
            shadingFunctionArray = new List<StiShadingFunctionData>();
            hatchArray = new ArrayList();
            tooltipsArray = new List<StiLinkObject>();

            sw = new StreamWriter(stream, Encoding.GetEncoding(1252));
            sw.NewLine = "\r\n";
            stream2 = stream;

            enc = global::System.Text.Encoding.GetEncoding(1252).GetEncoder();

            //init all counters
            imagesCounter = 0;
            fontsCounter = 0;
            bookmarksCounter = 0;
            //patternsCounter = 0;
            linksCounter = 0;
            annotsCounter = 0;
            annots2Counter = 0;
            shadingCounter = 0;
            shadingFunctionCounter = 0;
            hatchCounter = 0;
            tooltipsCounter = 0;

            //prepare codepage1252 info
            for (int index = 0; index < 256; index++)
            {
                CodePage1252[index] = index;
            }
            for (int index = 0; index < 32; index++)
            {
                CodePage1252[0x80 + index] = CodePage1252part80AF[index];
            }

            fontGlyphsReduceNotNeed = new bool[256];

            mfRender = new Stimulsoft.Report.Export.Tools.StiPdfMetafileRender(this, useUnicodeMode);
            tempGeomWriter = new StiPdfGeomWriter(null, this);
            #endregion

            #region Make ID string
            DateTime dt = DateTime.Now;
            string tempDT = dt.ToString("yyyyMMddHHmmsszzz");
            currentDateTime = tempDT.Substring(0, 17) + "'" + tempDT.Substring(18, 2) + "'";
            currentDateTimeMeta = dt.ToString("yyyy-MM-ddTHH:mm:sszzz");

            StringBuilder IDString = new StringBuilder();
            IDString.Append(dt.ToString("yyyyMMddHHmmssffff"));
            IDString.Append(producerName);
            IDString.Append(creatorName);
            IDString.Append(report.ReportAuthor);
            IDString.Append(report.ReportAlias);
            IDString.Append(report.ReportName);

            byte[] forHash = new byte[IDString.Length];
            for (int index = 0; index < IDString.Length; index++)
            {
                forHash[index] = (byte)IDString[index];
            }

            //IDValue = new byte[16];
            //Random rnd = new Random();
            //rnd.NextBytes(IDValue);
            IDValue = StiMD5Helper.ComputeHash(forHash);

            StringBuilder tempSB = new StringBuilder();
            for (int index = 0; index < IDValue.Length; index++)
            {
                tempSB.Append(IDValue[index].ToString("X2"));
            }
            IDValueString = tempSB.ToString();
            IDValueStringMeta = string.Format("uuid:{0}-{1}-{2}-{3}-{4}",
                IDValueString.Substring(0, 8),
                IDValueString.Substring(8, 4),
                IDValueString.Substring(12, 4),
                IDValueString.Substring(16, 4),
                IDValueString.Substring(20, 12)).ToLowerInvariant();
            #endregion

            pdfSecurity = new Tools.StiPdfSecurity(this);
            if (!usePdfA && fullTrust)
            {
                encrypted = pdfSecurity.ComputingCryptoValues(userAccessPrivileges, passwordInputOwner, passwordInputUser, keyLength, IDValue);
            }

            if (encrypted && useUnicodeMode)
            {
                embeddedFonts = true;
                this.embeddedFonts = embeddedFonts;
            }

            CurrentPassNumber = 0;
            MaximumPassNumber = (StiOptions.Export.Pdf.DivideSegmentPages ? 3 : 2);

            StiPagesCollection pages = pageRange.GetSelectedPages(report.RenderedPages);
            if (StiOptions.Export.Pdf.DivideSegmentPages)
            {
                pages = StiSegmentPagesDivider.Divide(pages, this);
                CurrentPassNumber++;
            }

            #region Make bookmarks table
            if ((report.Bookmark != null) && (report.Bookmark.Bookmarks.Count != 0))
            {
                AddBookmarkNode(report.Bookmark, -1);
                haveBookmarks = true;
                bookmarksCounter = bookmarksTree.Count;
            }
            #endregion

            StatusString = StiLocalization.Get("Export", "ExportingFormatingObjects");

            #region Scan for assemble data
            int tempPageNumber = 0;
            foreach (StiPage page in pages)
            {
                pages.GetPage(page);
                InvokeExporting(tempPageNumber, pages.Count, CurrentPassNumber, MaximumPassNumber);

                StoreShadingData1(page.Brush, tempPageNumber);
                StoreHatchData(page.Brush);

                if ((page.HyperlinkValue != null) && (page.HyperlinkValue.ToString().Trim().Length > 0) && (!page.HyperlinkValue.ToString().Trim().StartsWith("javascript:")) && !usePdfA)
                {
                    StiLinkObject stl = new StiLinkObject();
                    stl.Link = page.HyperlinkValue.ToString();
                    stl.Page = tempPageNumber;
                    linksArray.Add(stl);
                }

                if ((page.Watermark != null) && (page.Watermark.Enabled))
                {
                    if (!string.IsNullOrEmpty(page.Watermark.Text))
                    {
                        if (page.Watermark.Font != null)
                        {
                            int fnt = pdfFont.GetFontNumber(page.Watermark.Font);
                        }
                        //StringBuilder sb = new StringBuilder(page.Watermark.Text);
                        //sb = bidi.Convert(sb, false);
                        var sb = new StringBuilder(StiBidirectionalConvert2.ConvertString(page.Watermark.Text, false));
                        pdfFont.StoreUnicodeSymbolsInMap(sb);

                        StoreShadingData1(page.Watermark.TextBrush, tempPageNumber);
                        StoreHatchData(page.Watermark.TextBrush);
                    }
                    var advWatermark = page.TagValue as StiAdvancedWatermark;
                    if (advWatermark != null && advWatermark.WeaveEnabled)
                    {
                        FontFamily family = StiFontIconsHelper.GetFontFamilyIcons();
                        if (family == null) family = StiFontCollection.GetFontFamily("Stimulsoft");
                        var font1 = new Font(family, 8f);
                        pdfFont.GetFontNumber(font1);

                        var sb = new StringBuilder(StiFontIconsHelper.GetContent(advWatermark.WeaveMajorIcon) + StiFontIconsHelper.GetContent(advWatermark.WeaveMinorIcon));
                        pdfFont.StoreUnicodeSymbolsInMap(sb);
                    }

                    if (page.Watermark.ShowImageBehind == true)
                    {
                        var image = page.Watermark.GetImage(page.Report);
                        if (image != null)
                        {
                            if (Stimulsoft.Base.Helpers.StiSvgHelper.IsSvg(image))
                            {
                                using (var gdiImage = Stimulsoft.Base.Helpers.StiSvgHelper.ConvertSvgToImageScaled(image, imageResolution))
                                {
                                    StoreImageData(gdiImage, imageResolution, false, false);
                                }
                            }
                            else
                            {
                                using (var gdiImage = StiImageConverter.BytesToImage(image))
                                {
                                    if (gdiImage is Metafile)
                                        StoreWatermarkMetafileData(gdiImage, imageResolution, page);
                                    else
                                        StoreImageData(gdiImage, imageResolution, false, false);
                                }
                            }
                            watermarkImageExist[page] = null;
                        }
                    }
                }

                foreach (StiComponent component in page.Components)
                {
                    if (component.Enabled && !(report.IsPrinting && !component.Printable))
                    {
                        imageInfoCounter += 2;

                        #region render components
                        bool needPaint = (component.Width > 0) && (component.Height > 0);
                        if (needPaint)
                        {
                            IStiBrush mBrush = component as IStiBrush;
                            if (mBrush != null)
                            {
                                StoreShadingData1(mBrush.Brush, tempPageNumber);
                                StoreHatchData(mBrush.Brush);
                            }

                            if (component is StiCheckBox)
                            {
                                StiCheckBox checkBox = component as StiCheckBox;
                                StoreShadingData1(checkBox.TextBrush, tempPageNumber);
                                StoreHatchData(checkBox.TextBrush);

                                if ((allowEditable == StiPdfAllowEditable.Yes) && ((IStiEditable)checkBox).Editable)
                                {
                                    StiEditableObject seo = new StiEditableObject();
                                    seo.Page = tempPageNumber;
                                    seo.Component = component;
                                    annots2Array.Add(seo);
                                }
                                continue;
                            }

                            var advWatermark = component.TagValue as StiAdvancedWatermark;
                            if (advWatermark != null)
                            {
                                #region Process AdvancedWatermark
                                if (advWatermark.WeaveEnabled)
                                {
                                    FontFamily family = StiFontIconsHelper.GetFontFamilyIcons();
                                    if (family == null) family = StiFontCollection.GetFontFamily("Stimulsoft");
                                    var font1 = new Font(family, 8f);
                                    pdfFont.GetFontNumber(font1);

                                    var sb = new StringBuilder(StiFontIconsHelper.GetContent(advWatermark.WeaveMajorIcon) + StiFontIconsHelper.GetContent(advWatermark.WeaveMinorIcon));
                                    pdfFont.StoreUnicodeSymbolsInMap(sb);
                                }
                                if (advWatermark.ImageEnabled && advWatermark.ImageBytes != null)
                                {
                                    if (Stimulsoft.Base.Helpers.StiSvgHelper.IsSvg(advWatermark.ImageBytes))
                                    {
                                        using (var gdiImage = Stimulsoft.Base.Helpers.StiSvgHelper.ConvertSvgToImageScaled(advWatermark.ImageBytes, imageResolution))
                                        {
                                            StoreImageData(gdiImage, imageResolution, false, false);
                                        }
                                    }
                                    else
                                    {
                                        using (var gdiImage = StiImageConverter.BytesToImage(advWatermark.ImageBytes))
                                        {
                                            if (gdiImage is Metafile)
                                                StoreWatermarkMetafileData(gdiImage, imageResolution, page);
                                            else
                                                StoreImageData(gdiImage, imageResolution, false, false);
                                        }
                                    }
                                    //watermarkImageExist[page] = null;
                                }
                                if (advWatermark.TextEnabled && !string.IsNullOrEmpty(advWatermark.Text))
                                {
                                    if (advWatermark.TextFont != null)
                                    {
                                        int fnt = pdfFont.GetFontNumber(advWatermark.TextFont);
                                    }
                                    var sb = new StringBuilder(StiBidirectionalConvert2.ConvertString(advWatermark.Text, false));
                                    pdfFont.StoreUnicodeSymbolsInMap(sb);
                                }
                                #endregion
                            }

                            StiMathFormulaCache.Clear();

                            bool isExportAsImage = component.IsExportAsImage(StiExportFormat.Pdf);

                            StiShape shape = component as StiShape;
                            if ((shape != null) && (CheckShape(shape)))
                            {
                                isExportAsImage = false;
                            }

                            if (component is StiElectronicSignature || component is StiPdfDigitalSignature)
                            {
                                isExportAsImage = false;
                            }

                            if ((exportRtfTextAsImage || !(component is StiRichText)) && isExportAsImage)
                            {
                                bool flagIsImage = false;
                                StiImage imageTemp = component as StiImage;
                                if ((imageResolutionMode != StiImageResolutionMode.Exactly) && (imageTemp != null) && imageTemp.ExistImageToDraw() && !imageTemp.ImageToDrawIsMetafile())
                                {
                                    using (var gdiImage = imageTemp.TakeGdiImageToDraw(imageResolutionMain))
                                    {
                                        if (gdiImage != null)
                                        {
                                            flagIsImage = true;

                                            if (imageTemp.ImageRotation != StiImageRotation.None)
                                            {
                                                Helpers.StiImageHelper.RotateImage(gdiImage, imageTemp.ImageRotation, true);
                                            }

                                            float rsImageResolution = gdiImage.HorizontalResolution / 100;
                                            if (imageResolutionMode == StiImageResolutionMode.NoMoreThan)
                                            {
                                                if (imageTemp.Stretch)
                                                {
                                                    rsImageResolution = (float) (gdiImage.Width / report.Unit.ConvertToHInches(component.Width));
                                                }
                                                else
                                                {
                                                    rsImageResolution = (float) (1 / imageTemp.MultipleFactor);
                                                }
                                            }
                                            rsImageResolution = StoreImageData(gdiImage, rsImageResolution, true, imageTemp.Smoothing);
                                            imageInfoList[imageInfoCounter] = rsImageResolution;
                                        }
                                    }
                                }
                                if (!flagIsImage)
                                {
                                    IStiExportImageExtended exportImage = component as IStiExportImageExtended;
                                    if (exportImage != null)
                                    {
                                        float rsImageResolution = imageResolution;
                                        using (Image image = exportImage.GetImage(ref rsImageResolution, StiExportFormat.Pdf))
                                        {
                                            if (image != null)
                                            {
                                                //imagesCounter++;
                                                StoreImageData(image, rsImageResolution, false, (imageTemp != null) && imageTemp.Smoothing);
                                                imageInfoList[imageInfoCounter + 1] = rsImageResolution;
                                            }
                                        }
                                    }
                                }
                            }

                            IStiFont mFont = component as IStiFont;
                            if (mFont != null)
                            {
                                int fnt = pdfFont.GetFontNumber(mFont.Font);
                            }

                            //make map of the unicode symbols
                            IStiTextOptions textOpt = component as IStiTextOptions;
                            if (component is StiText && (!isExportAsImage))
                            {
                                StiText text = component as StiText;
                                if (StiOptions.Engine.UseNewHtmlEngine && text.CheckAllowHtmlTags() ||
                                    StiDpiHelper.IsWindows && (text.CheckAllowHtmlTags() || (StiOptions.Export.Pdf.UseWysiwygRender && text.TextQuality == StiTextQuality.Wysiwyg)))
                                {
                                    if (StiOptions.Export.Pdf.UseOldModeAllowHtmlTags)
                                    {
                                        mfRender.AssembleTextToEmf(component, 1f);
                                    }
                                    else
                                    {
                                        StoreWysiwygSymbols(text, tempPageNumber);
                                    }
                                }
                                else
                                {
                                    bool useRightToLeft =
                                        ((textOpt != null) && (textOpt.TextOptions != null) && (textOpt.TextOptions.RightToLeft));
                                    //StringBuilder sb = new StringBuilder(text.Text);
                                    //sb = bidi.Convert(sb, useRightToLeft);
                                    var sb = new StringBuilder(StiBidirectionalConvert2.ConvertString(text.Text, useRightToLeft));
                                    pdfFont.StoreUnicodeSymbolsInMap(sb);

                                    IStiTextBrush mTextBrush = component as IStiTextBrush;
                                    if ((text != null) && (mTextBrush != null))
                                    {
                                        StoreShadingData1(mTextBrush.TextBrush, tempPageNumber);
                                        StoreHatchData(mTextBrush.TextBrush);
                                    }
                                }
                                if ((allowEditable == StiPdfAllowEditable.Yes) && ((IStiEditable)text).Editable)
                                {
                                    StiEditableObject seo = new StiEditableObject();
                                    seo.Page = tempPageNumber;
                                    annotsArray.Add(seo);
                                    fontGlyphsReduceNotNeed[pdfFont.CurrentFont] = true;
                                }
                                var indicator = text.Indicator as StiIconSetIndicator;
                                if (indicator != null && (indicator.Icon != StiIcon.None || indicator.CustomIcon != null))
                                {
                                    using (var gdiImage = StiIconSetHelper.GetIcon(indicator))
                                    {
                                        var rsImageResolution = 1f;
                                        rsImageResolution = StoreImageData(gdiImage, rsImageResolution, false, true, true);
                                        imageInfoList[imageInfoCounter] = rsImageResolution;
                                    }
                                }
                                var barIndicator = text.Indicator as StiDataBarIndicator;
                                if ((barIndicator != null) && (barIndicator.Value != 0) && (barIndicator.BrushType == Stimulsoft.Report.Components.StiBrushType.Gradient))
                                {
                                    Color startColor = barIndicator.Value < 0 ? barIndicator.NegativeColor : barIndicator.PositiveColor;
                                    Color endColor = StiColorUtils.Light(startColor, 200);

                                    //StoreShadingData1
                                    StiShadingData ssd = new StiShadingData();
                                    ssd.Page = tempPageNumber;
                                    ssd.FunctionIndex = GetShadingFunctionNumber(startColor, endColor, false);
                                    shadingArray.Add(ssd);
                                }
                            }
                            if (!exportRtfTextAsImage && (component is StiRichText))
                            {
                                mfRender.AssembleRtf(component);
                            }

                            if (shape != null && !isExportAsImage && !string.IsNullOrWhiteSpace(shape.GetParsedText()))
                            {
                                var sb = new StringBuilder(StiBidirectionalConvert2.ConvertString(shape.GetParsedText(), false));
                                pdfFont.StoreUnicodeSymbolsInMap(sb);
                            }

                            if (component is StiBarCode && !isExportAsImage)
                            {
                                StiPdfGeomWriter pdfGeomWriter = new StiPdfGeomWriter(pageStream, this, true);
                                pdfGeomWriter.pageNumber = tempPageNumber;
                                StiBarCodeExportPainter barCodePainter = new StiBarCodeExportPainter(pdfGeomWriter);
                                StiBarCode barCode = component as StiBarCode;
                                if (!string.IsNullOrEmpty(barCode.CodeValue) && barCode.Page != null)
                                {
                                    RectangleF rectf = report.Unit.ConvertToHInches(barCode.ClientRectangle).ToRectangleF();
                                    barCode.BarCodeType.Draw(barCodePainter, barCode, rectf, 1);
                                }
                            }

                            else if (component is StiChart && !isExportAsImage)
                            {
                                StiPdfData pp = new StiPdfData();
                                pp.Component = component;
                                RenderChart(pp, true, tempPageNumber);
                            }

                            else if (component is StiGauge && !isExportAsImage)
                            {
                                StiPdfData pp = new StiPdfData();
                                pp.Component = component;
                                RenderGauge(pp, true, tempPageNumber);
                            }

                            else if (component is StiMap && !isExportAsImage)
                            {
                                StiPdfData pp = new StiPdfData();
                                pp.Component = component;
                                RenderMap(pp, true, tempPageNumber);
                            }

                            else if (component is StiSparkline && !isExportAsImage)
                            {
                                StiPdfData pp = new StiPdfData();
                                pp.Component = component;
                                RenderSparkline(pp, true, tempPageNumber);
                            }

                            if (component is StiElectronicSignature)
                            {
                                isExportAsImage = false;
                                var signature = component as StiElectronicSignature;
                                if (signature.Mode == StiSignatureMode.Draw)
                                {
                                    if ((signature.Image?.Image != null) && (signature.Image.Image.Length > 0))
                                    {
                                        using (var gdiImage = StiImageConverter.BytesToImage(signature.Image.Image))
                                        {
                                            float rsImageResolution = gdiImage.HorizontalResolution / 100;
                                            rsImageResolution = StoreImageData(gdiImage, rsImageResolution, false, true);
                                            imageInfoList[imageInfoCounter] = rsImageResolution;
                                        }
                                    }
                                    if (signature.Draw.Image != null)
                                    {
                                        using (var gdiImage = StiImageConverter.BytesToImage(signature.Draw.Image))
                                        {
                                            using (var newBmp = new Bitmap(gdiImage.Width, gdiImage.Height, gdiImage.PixelFormat))
                                            {
                                                using (var g = Graphics.FromImage(newBmp))
                                                {
                                                    g.Clear(Color.FromArgb(1, 255, 255, 255));  //fix for StiImageCache
                                                    g.DrawImage(gdiImage, 0, 0, gdiImage.Width, gdiImage.Height);
                                                }
                                                StoreImageData(newBmp, 1, false, true);
                                                imageInfoList[imageInfoCounter] = 1f;
                                            }
                                        }
                                    }
                                    if (!string.IsNullOrWhiteSpace(signature.Text?.Text))
                                    {
                                        int fnt = pdfFont.GetFontNumber(signature.Text.Font);
                                        var sb = new StringBuilder(StiBidirectionalConvert2.ConvertString(signature.Text.Text, false));
                                        pdfFont.StoreUnicodeSymbolsInMap(sb);
                                    }
                                }
                                if (signature.Mode == StiSignatureMode.Type)
                                {
                                    string signText = $" {signature.Type.FullName} {signature.Type.Initials}";
                                    if (!string.IsNullOrWhiteSpace(signText))
                                    {
                                        var signFont = new Font(SignatureFonts.StiSignatureFontsHelper.GetFont(signature.Type.Style), 16);
                                        int fnt = pdfFont.GetFontNumber(signFont);
                                        var sb = new StringBuilder(StiBidirectionalConvert2.ConvertString(signText, false));
                                        pdfFont.StoreUnicodeSymbolsInMap(sb);
                                    }
                                }
                            }
                            if (component is StiPdfDigitalSignature)
                            {
                                isExportAsImage = false;
                                var signature = component as StiPdfDigitalSignature;
                                if (!string.IsNullOrWhiteSpace(signature.Placeholder))
                                {
                                    int fnt = pdfFont.GetFontNumber(new Font("Segoe UI", 10f));
                                    var sb = new StringBuilder(StiBidirectionalConvert2.ConvertString(signature.Placeholder, false));
                                    pdfFont.StoreUnicodeSymbolsInMap(sb);
                                }
                            }
                        }

                        if ((component.HyperlinkValue != null) && (component.HyperlinkValue.ToString().Trim().Length > 0) && (!component.HyperlinkValue.ToString().Trim().StartsWith("javascript:")))
                        {
                            StiLinkObject stl = new StiLinkObject();
                            stl.Link = component.HyperlinkValue.ToString();
                            stl.Page = tempPageNumber;
                            linksArray.Add(stl);
                        }

                        if ((component.ToolTipValue != null) && (component.ToolTipValue.ToString().Trim().Length > 0) && !usePdfA)
                        {
                            StiLinkObject stl = new StiLinkObject();
                            //stl.Link = component.ToolTipValue.ToString();
                            stl.Page = tempPageNumber;
                            tooltipsArray.Add(stl);
                        }

                        if (component is StiPdfDigitalSignature)
                        {
                            var stl = new StiEditableObject();
                            stl.Page = tempPageNumber;
                            unsignedSignaturesArray.Add(stl);
                        }

                        if ((component.TagValue != null) && (component.TagValue.ToString().ToLower(CultureInfo.InvariantCulture) == "pdfunsignedsignaturefield"))
                        {
                            var stl = new StiEditableObject();
                            stl.Page = tempPageNumber;
                            unsignedSignaturesArray.Add(stl);
                        }

                        if ((useDigitalSignature) && (!haveDigitalSignature) && (component.TagValue != null) && (component.TagValue.ToString().ToLower(CultureInfo.InvariantCulture) == "pdfdigitalsignature"))
                        {
                            signaturePageNumber = tempPageNumber;
                            haveDigitalSignature = true;
                        }
                        #endregion
                    }
                }
                tempPageNumber++;

                if ((page.Watermark != null) && (page.Watermark.Enabled))
                {
                    if (page.Watermark.ShowImageBehind == false)
                    {
                        var image = page.Watermark.GetImage(page.Report);
                        if (image != null)
                        {
                            if (Stimulsoft.Base.Helpers.StiSvgHelper.IsSvg(image))
                            {
                                using (var gdiImage = Stimulsoft.Base.Helpers.StiSvgHelper.ConvertSvgToImageScaled(image, imageResolution))
                                {
                                    StoreImageData(gdiImage, imageResolution, false, false);
                                }
                            }
                            else
                            {
                                using (var gdiImage = StiImageConverter.BytesToImage(image))
                                {
                                    if (gdiImage is Metafile)
                                        StoreWatermarkMetafileData(gdiImage, imageResolution, page);
                                    else
                                        StoreImageData(gdiImage, imageResolution, false, false);
                                }
                            }
                            watermarkImageExist[page] = null;
                        }
                    }
                }
            }

            //for trimming
            for (int indexFont = 0; indexFont < pdfFont.fontList.Count; indexFont++)
            {
                pdfFont.CurrentFont = indexFont;
                pdfFont.StoreUnicodeSymbolsInMap(new StringBuilder("…"));
            }

            if ((unsignedSignaturesArray.Count > 0) && (annotsArray.Count > 0 || annots2Array.Count > 0))
            {
                for (int indexFont = 0; indexFont < pdfFont.fontList.Count; indexFont++)
                {
                    var font = (PdfFonts.pfontInfo)pdfFont.fontList[indexFont];
                    if (font.Name == "Arial")
                    {
                        fontGlyphsReduceNotNeed[indexFont] = true;
                    }
                }
            }

            //prepare codepage1252 string
            StringBuilder sbb = new StringBuilder();
            for (int indexs = 32; indexs < 256; indexs++)
            {
                sbb.Append((char)CodePage1252[indexs]);
            }
            //add all codepage1252 to symbols
            for (int indexf = 0; indexf < pdfFont.fontList.Count; indexf++)
            {
                if (fontGlyphsReduceNotNeed[indexf])
                {
                    pdfFont.CurrentFont = indexf;
                    pdfFont.StoreUnicodeSymbolsInMap(sbb);
                }
            }

            imagesCurrent = 0;
            annotsCurrent = 0;
            annots2Current = 0;
            shadingCurrent = 0;

            linksCounter = linksArray.Count;
            haveLinks = linksCounter > 0;
            annotsCounter = annotsArray.Count;
            annots2Counter = annots2Array.Count;
            unsignedSignaturesCounter = unsignedSignaturesArray.Count;
            haveAnnots = annotsCounter > 0 || annots2Counter > 0 || unsignedSignaturesCounter > 0;
            shadingCounter = shadingArray.Count;
            shadingFunctionCounter = shadingFunctionArray.Count;
            hatchCounter = hatchArray.Count;
            tooltipsCounter = tooltipsArray.Count;
            haveTooltips = tooltipsCounter > 0;

            imagesCounter = imageCache.ImageStore.Count;
            int imagesMaskCounter = 0;
            for (int indexMask = 0; indexMask < imageCache.ImageMaskStore.Count; indexMask++)
            {
                if (imageCache.ImageMaskStore[indexMask] != null) imagesMaskCounter++;
            }
            #endregion

            if (useDigitalSignature && (!haveDigitalSignature))
            {
                haveDigitalSignature = true;
                signaturePageNumber = 0;

                StiPage page = pages[0];

                double pageHeight2 = hiToTwips * report.Unit.ConvertToHInches(page.PageHeight * page.SegmentPerHeight);
                double pageWidth2 = hiToTwips * report.Unit.ConvertToHInches(page.PageWidth * page.SegmentPerWidth);
                double mgLeft2 = hiToTwips * report.Unit.ConvertToHInches(page.Margins.Left);
                double mgRight2 = hiToTwips * report.Unit.ConvertToHInches(page.Margins.Right);
                double mgTop2 = hiToTwips * report.Unit.ConvertToHInches(page.Margins.Top);
                double mgBottom2 = hiToTwips * report.Unit.ConvertToHInches(page.Margins.Bottom);

                signaturePlacement = new RectangleD(0, pageHeight2 - mgTop2, mgLeft2, mgBottom2);
            }

            pdfFont.InitFontsData(isWpf);
            fontsCounter = pdfFont.fontList.Count;

            #region Fill structure info
            info = new StiPdfStructure();
            for (int index = 0; index < pages.Count; index++)
            {
                info.PageList.Add(info.CreateContentObject(true));
            }
            for (int index = 0; index < imageCache.ImagePackedStore.Count; index++)
            {
                info.XObjectList.Add(info.CreateXObject(true, imageCache.ImageMaskStore[index] != null));
            }
            for (int index = 0; index < pdfFont.fontList.Count; index++)
            {
                var tempFontInfo = (PdfFonts.pfontInfo)pdfFont.fontList[index];
                info.FontList.Add(info.CreateFontObject(true, tempFontInfo.UseUnicode, standardPdfFonts, embeddedFonts || PdfFonts.IsFontStimulsoft(tempFontInfo.Name) || SignatureFonts.StiSignatureFontsHelper.IsSignatureFont(tempFontInfo.Name)));
            }
            info.Outlines = info.CreateOutlinesObject(haveBookmarks);
            if (haveBookmarks)
            {
                for (int index = 0; index < bookmarksCounter; index++)
                {
                    info.Outlines.Items.Add(info.CreateObject(true));
                }
            }
            info.Patterns = info.CreatePatternsObject(true);
            for (int index = 0; index < hatchCounter; index++)
            {
                info.Patterns.HatchItems.Add(info.CreateObject(true));
            }
            for (int index = 0; index < shadingCounter; index++)
            {
                info.Patterns.ShadingItems.Add(info.CreateObject(true));
            }
            for (int index = 0; index < shadingFunctionCounter; index++)
            {
                info.Patterns.ShadingFunctionItems.Add(info.CreateObject(true));
            }
            for (int index = 0; index < linksCounter; index++)
            {
                info.LinkList.Add(info.CreateObject(true));
            }
            info.Encode = info.CreateObject(encrypted);
            info.ExtGState = info.CreateObject(true);

            info.AcroForm = info.CreateAcroFormObject(haveAnnots || haveDigitalSignature || haveTooltips);
            if (haveAnnots)
            {
                for (int index = 0; index < annotsCounter; index++)
                {
                    info.AcroForm.Annots.Add(info.CreateAnnotObject(true, true, 0));
                }
                for (int index = 0; index < fontsCounter; index++)
                {
                    info.AcroForm.AnnotFontItems.Add(info.CreateFontObject(true, false, false, false, true));
                }
                for (int index = 0; index < annots2Counter; index++)
                {
                    info.AcroForm.CheckBoxes.Add(new List<StiPdfStructure.StiPdfAnnotObjInfo>());
                    info.AcroForm.CheckBoxes[index].Add(info.CreateAnnotObject(true, true, 2));
                    info.AcroForm.CheckBoxes[index].Add(info.CreateAnnotObject(true, true, 2));
                    if ((annots2Array[index].Component as StiCheckBox).CheckedValue == null)
                    {
                        info.AcroForm.CheckBoxes[index].Add(info.CreateAnnotObject(true, false, 2));
                    }
                }
                for (int index = 0; index < unsignedSignaturesCounter; index++)
                {
                    info.AcroForm.UnsignedSignatures.Add(info.CreateAnnotObject(true, true, 0));
                }
            }
            info.AcroForm.Signatures.Add(info.CreateAnnotObject(haveDigitalSignature, true, 0));
            for (int index = 0; index < tooltipsCounter; index++)
            {
                info.AcroForm.Tooltips.Add(info.CreateAnnotObject(true, false, 0));
            }

            info.Metadata = info.CreateObject(true);

            info.DestOutputProfile = info.CreateObject(true);
            info.OutputIntents = info.CreateObject(true);

            info.EmbeddedJS = info.CreateContentObject(autoPrint != StiPdfAutoPrintMode.None);

            if (embeddedFiles != null && embeddedFiles.Count > 0)
            {
                foreach (var element in embeddedFiles)
                {
                    info.EmbeddedFilesList.Add(info.CreateContentObject(true));
                }
            }
            #endregion

            RenderStartDoc(report, pages);

            #region Write data
            int pageNumber = 0;
            linksArray.Clear();
            tagsArray = new List<StiLinkObject>();
            tooltipsArray.Clear();
            unsignedSignaturesArray.Clear();
            imageInfoCounter = 0;

            StatusString = StiLocalization.Get("Export", "ExportingCreatingDocument");

            CurrentPassNumber++;

            foreach (StiPage page in pages)
            {
                pages.GetPage(page);
                InvokeExporting(pageNumber, pages.Count, CurrentPassNumber, MaximumPassNumber);
                if (IsStopped) return;

                RenderPageHeader(pageNumber++);

                double pageHeight = hiToTwips * report.Unit.ConvertToHInches(page.PageHeight * page.SegmentPerHeight);
                double pageWidth = hiToTwips * report.Unit.ConvertToHInches(page.PageWidth * page.SegmentPerWidth);
                double mgLeft = hiToTwips * report.Unit.ConvertToHInches(page.Margins.Left);
                double mgRight = hiToTwips * report.Unit.ConvertToHInches(page.Margins.Right);
                double mgTop = hiToTwips * report.Unit.ConvertToHInches(page.Margins.Top);
                double mgBottom = hiToTwips * report.Unit.ConvertToHInches(page.Margins.Bottom);

                if (pageHeight > 14400)
                {
                    pageHeight = 14400;     //Adobe Acrobat limitation of page size: maximum 200" * 200"
                }

                #region Bookmarks on pages
                if (report.Bookmark.Engine == StiEngineVersion.EngineV1)
                {
                    if ((haveBookmarks == true) && (page.BookmarkValue != null) && ((string)page.BookmarkValue != ""))
                    {
                        bool bookmarkFinded = false;
                        int tempIndex = 0;
                        while ((bookmarkFinded == false) && (tempIndex < bookmarksCounter))
                        {
                            StiBookmarkTreeNode tn = (StiBookmarkTreeNode)bookmarksTree[tempIndex];
                            if ((tn.Used == false) && (tn.Page == -1) && (tn.Title == (string)page.BookmarkValue))
                            {
                                tn.Page = pageNumber - 1;
                                tn.Y = pageHeight;
                                tn.Used = true;
                                bookmarkFinded = true;
                                bookmarksTree[tempIndex] = tn;
                                break;
                            }
                            tempIndex++;
                        }
                    }
                }
                else
                {
                    if ((haveBookmarks == true) && (!string.IsNullOrEmpty(page.Guid) || ((page.BookmarkValue != null) && ((string)page.BookmarkValue != ""))))
                    {
                        int tempIndex = 0;
                        while (tempIndex < bookmarksCounter)
                        {
                            StiBookmarkTreeNode tn = (StiBookmarkTreeNode)bookmarksTree[tempIndex];
                            if ((tn.Used == false) && (tn.Page == -1))
                            {
                                bool finded = false;
                                if (!string.IsNullOrEmpty(tn.Guid))
                                {
                                    if (!string.IsNullOrEmpty(page.Guid) && (tn.Guid == page.Guid)) finded = true;
                                }
                                else if (!string.IsNullOrEmpty(tn.Title))
                                {
                                    if ((page.BookmarkValue != null) && (((string)page.BookmarkValue).Length > 0) && (tn.Title == (string)page.BookmarkValue)) finded = true;
                                }

                                if (finded)
                                {
                                    tn.Page = pageNumber - 1;
                                    tn.Y = pageHeight;
                                    tn.Used = true;
                                    bookmarksTree[tempIndex] = tn;
                                    break;
                                }
                            }
                            tempIndex++;
                        }
                    }
                }
                #endregion

                #region Hyperlinks on page
                if ((page.HyperlinkValue != null) && (page.HyperlinkValue.ToString().Trim().Length > 0) && (!page.HyperlinkValue.ToString().Trim().StartsWith("javascript:")) && !usePdfA)
                {
                    StiLinkObject stl = new StiLinkObject();
                    stl.Link = page.HyperlinkValue.ToString();
                    stl.X = 0;
                    stl.Y = 0;
                    stl.Width = pageWidth;
                    stl.Height = pageHeight;
                    stl.Page = pageNumber - 1;
                    stl.DestPage = -1;
                    stl.DestY = -1;
                    linksArray.Add(stl);
                }
                #endregion

                #region Pointer on page
                if ((page.PointerValue != null) && !string.IsNullOrWhiteSpace((string)page.PointerValue) && !string.IsNullOrWhiteSpace(page.Guid))
                {
                    string pointerValue = $"#{page.PointerValue}#GUID#{page.Guid}";

                    StiLinkObject stl = new StiLinkObject();
                    stl.Link = pointerValue;
                    stl.X = 0;
                    stl.Y = 0;
                    stl.Width = pageWidth;
                    stl.Height = pageHeight;
                    stl.Page = pageNumber - 1;
                    stl.DestPage = -1;
                    stl.DestY = -1;
                    tagsArray.Add(stl);
                }
                #endregion

                #region Render page
                if (page.Brush != null)
                {
                    StiPdfData pp = new StiPdfData();
                    pp.X = 0;
                    pp.Y = 0;
                    pp.Width = pageWidth;
                    pp.Height = pageHeight;
                    pp.Component = new StiContainer();
                    (pp.Component as StiContainer).Brush = page.Brush;
                    (pp.Component as StiContainer).Border = null;
                    RenderBorder1(pp);  //fullpage fill
                }

                RenderWatermark(page, true, pageWidth, pageHeight, imageResolution);

                #region Add segment clipping
                string pageTag = page.TagValue as string;
                if (!string.IsNullOrWhiteSpace(pageTag) && pageTag.StartsWith("Segments:"))
                {
                    int resInt = 0;
                    if (int.TryParse(pageTag.Substring(9), out resInt))
                    {
                        StiBorderSides sides = (StiBorderSides)resInt;

                        RectangleD clipd = new RectangleD(0, 0, pageWidth, pageHeight);
                        double offsetd = 1;
                        if (sides.HasFlag(StiBorderSides.Left))
                        {
                            clipd.X += mgLeft - offsetd;
                            clipd.Width -= mgLeft - offsetd;
                        }
                        if (sides.HasFlag(StiBorderSides.Right))
                        {
                            clipd.Width -= mgRight - offsetd;
                        }
                        if (sides.HasFlag(StiBorderSides.Top))
                        {
                            clipd.Height -= mgTop - offsetd;
                        }
                        if (sides.HasFlag(StiBorderSides.Bottom))
                        {
                            clipd.Y += mgBottom - offsetd;
                            clipd.Height -= mgBottom - offsetd;
                        }

                        pageStream.WriteLine("{0} {1} {2} {3} re W n",
                            ConvertToString(clipd.X),
                            ConvertToString(clipd.Y),
                            ConvertToString(clipd.Width),
                            ConvertToString(clipd.Height));
                    }
                }
                #endregion

                #region Paint ExceedMargins
                foreach (StiComponent component2 in page.Components)
                {
                    if (component2.Enabled && !(report.IsPrinting && !component2.Printable) && (component2.Width > 0) && (component2.Height > 0))
                    {
                        StiText stiText = component2 as StiText;
                        if ((stiText != null) && (stiText.ExceedMargins != StiExceedMargins.None))
                        {
                            #region Process ExceedMargins
                            double x1 = hiToTwips * report.Unit.ConvertToHInches(component2.Left);
                            double y1 = hiToTwips * report.Unit.ConvertToHInches(component2.Top);
                            double x2 = hiToTwips * report.Unit.ConvertToHInches(component2.Right);
                            double y2 = hiToTwips * report.Unit.ConvertToHInches(component2.Bottom);

                            var pp = new StiPdfData();
                            pp.X = x1 + mgLeft;
                            pp.Y = y1 + mgTop;
                            pp.Width = x2 - x1;
                            pp.Height = y2 - y1;
                            pp.Y = pageHeight - (pp.Y + pp.Height);
                            pp.Component = component2;

                            var pp2 = pp.Clone();

                            if ((stiText.ExceedMargins & StiExceedMargins.Left) > 0)
                            {
                                pp2.X = 0;
                                pp2.Width = pp.Right;
                            }
                            if ((stiText.ExceedMargins & StiExceedMargins.Right) > 0)
                            {
                                pp2.Width = pageWidth - pp2.X;
                            }
                            if ((stiText.ExceedMargins & StiExceedMargins.Bottom) > 0)
                            {
                                pp2.Y = 0;
                                pp2.Height = pp.Top;
                            }
                            if ((stiText.ExceedMargins & StiExceedMargins.Top) > 0)
                            {
                                pp2.Height = pageHeight - pp2.Y;
                            }
                            #endregion

                            RenderBorder1(pp2);
                        }
                    }
                }
                #endregion

                List<StiPdfData> storedBorders = new List<StiPdfData>();

                #region Sorting, DrawTopMost, 
                List<StiComponent> comps1 = new List<StiComponent>();
                List<StiComponent> comps2 = new List<StiComponent>();
                foreach (StiComponent component in page.Components)
                {
                    if ((component.TagValue != null) && (component.TagValue.ToString().ToLowerInvariant() == Stimulsoft.Report.Painters.StiContainerGdiPainter.TopmostToken))
                    {
                        comps2.Add(component);
                    }
                    else
                    {
                        comps1.Add(component);
                    }
                }

                if (StiOptions.Export.Pdf.OrderComponentsByPlacement)
                {
                    #region Sorting by Y, then by X
                    for (int index1 = 0; index1 < comps1.Count - 1; index1++)
                    {
                        var tempComp1 = comps1[index1];
                        for (int index2 = index1 + 1; index2 < comps1.Count; index2++)
                        {
                            var tempComp2 = comps1[index2];
                            if (tempComp1.Top < tempComp2.Top) continue;
                            if ((tempComp1.Top == tempComp2.Top) && (tempComp1.Left <= tempComp2.Left)) continue;
                            comps1[index2] = tempComp1;
                            comps1[index1] = tempComp2;
                            tempComp1 = tempComp2;
                        }
                    }
                    #endregion
                }

                comps1.AddRange(comps2);
                comps2.Clear();
                #endregion

                foreach (StiComponent component in comps1)
                {
                    if (component.Enabled && !(report.IsPrinting && !component.Printable))
                    {
                        imageInfoCounter += 2;

                        double x1 = hiToTwips * report.Unit.ConvertToHInches(component.Left);
                        double y1 = hiToTwips * report.Unit.ConvertToHInches(component.Top);
                        double x2 = hiToTwips * report.Unit.ConvertToHInches(component.Right);
                        double y2 = hiToTwips * report.Unit.ConvertToHInches(component.Bottom);

                        StiPdfData pp = new StiPdfData();

                        pp.X = x1;
                        pp.Y = y1;
                        pp.Width = x2 - x1;
                        pp.Height = y2 - y1;

                        pp.Y += mgTop;
                        pp.X += mgLeft;
                        pp.Y = pageHeight - (pp.Y + pp.Height);
                        pp.Component = component;

                        bool needPaint = (component.Width > 0) && (component.Height > 0);
                        if (needPaint)
                        {
                            bool isExportAsImage = component.IsExportAsImage(StiExportFormat.Pdf);
                            StiText stiText = component as StiText;
                            bool needExceedMargins = (stiText != null) && (stiText.ExceedMargins != StiExceedMargins.None);

                            if (!usePdfA && StiOptions.Export.Pdf.AllowPrintable && !component.Printable)
                            {
                                pageStream.WriteLine("/OC /oc1 BDC");
                            }

                            if (!(component is StiShape) && !needExceedMargins)
                            {
                                RenderBorder1(pp);
                            }

                            var advWatermark = component.TagValue as StiAdvancedWatermark;
                            if (advWatermark != null)
                            {
                                #region Render AdvancedWatermark
                                pageStream.WriteLine("q");
                                PushColorToStack();
                                pageStream.WriteLine("{0} {1} {2} {3} re W n",
                                    ConvertToString(pp.X),
                                    ConvertToString(pp.Y),
                                    ConvertToString(pp.Width),
                                    ConvertToString(pp.Height));

                                var advRect = new RectangleD(pp.X, pp.Y, pp.Width, pp.Height);
                                if (advWatermark.ImageEnabled && advWatermark.ImageBytes != null)
                                {
                                    RenderWatermarkImage(
                                        advRect,
                                        advWatermark.ImageStretch,
                                        advWatermark.ImageTiling,
                                        advWatermark.ImageAspectRatio,
                                        advWatermark.ImageMultipleFactor,
                                        advWatermark.ImageAlignment);
                                }
                                if (!string.IsNullOrEmpty(advWatermark.Text))
                                {
                                    StiPdfData pp3 = pp.Clone();
                                    StiText stt = new StiText(advRect);
                                    stt.Text = advWatermark.Text;
                                    stt.TextBrush = new StiSolidBrush(advWatermark.TextColor);
                                    stt.Font = advWatermark.TextFont;
                                    stt.Angle = advWatermark.TextAngle;
                                    stt.HorAlignment = StiTextHorAlignment.Center;
                                    stt.VertAlignment = StiVertAlignment.Center;
                                    stt.Page = page;
                                    stt.TextQuality = StiTextQuality.Standard;
                                    pp3.Component = stt;
                                    RenderTextFont(pp3);
                                    RenderText(pp3);
                                }
                                if (advWatermark.WeaveEnabled)
                                {
                                    RenderWatermarkWeave(advWatermark, advRect, page);
                                }
                                pageStream.WriteLine("Q");
                                PopColorFromStack();
                                #endregion
                            }

                            if ((stiText != null) && !isExportAsImage)
                            {
                                #region prepare annot
                                int memPos = 0;
                                if (haveAnnots && stiText.Editable)
                                {
                                    pageStream.Flush();
                                    memPos = (int)pageStream.BaseStream.Position;
                                    //some correction - GS is not supported in editable
                                    PushColorToStack();
                                    lastColorStrokeA = 0xFF;
                                    lastColorNonStrokeA = 0xFF;
                                }
                                #endregion

                                var pp2 = RenderIndicators(pp);

                                if (StiOptions.Engine.UseNewHtmlEngine && stiText.CheckAllowHtmlTags() ||
                                    StiDpiHelper.IsWindows && (stiText.CheckAllowHtmlTags() || (StiOptions.Export.Pdf.UseWysiwygRender && stiText.TextQuality == StiTextQuality.Wysiwyg && !isWpf)))
                                {
                                    RenderTextFont(pp); //for set seo.font

                                    if (StiOptions.Export.Pdf.UseOldModeAllowHtmlTags)
                                    {
                                        mfRender.RenderTextToEmf(pp2, 1f);
                                    }
                                    else
                                    {
                                        RenderText2(pp2, pageNumber);
                                    }

                                    #region Store editable info
                                    if (haveAnnots && (pp.Component as StiText).Editable)
                                    {
                                        var seo = (StiEditableObject)annotsArray[annotsCurrent];
                                        seo.Multiline = stiText.WordWrap;
                                        seo.X = pp.X;
                                        seo.Y = pp.Y;
                                        seo.Width = pp.Width;
                                        seo.Height = pp.Height;
                                        seo.Component = pp.Component;

                                        seo.Alignment = StiTextHorAlignment.Left;
                                        var mTextHorAlign = pp.Component as IStiTextHorAlignment;
                                        if (mTextHorAlign != null)
                                        {
                                            StiTextHorAlignment horAlign = mTextHorAlign.HorAlignment;
                                            IStiTextOptions textOpt = pp.Component as IStiTextOptions;
                                            if (textOpt != null && textOpt.TextOptions != null && textOpt.TextOptions.RightToLeft)
                                            {
                                                if (horAlign == StiTextHorAlignment.Left) horAlign = StiTextHorAlignment.Right;
                                                else if (horAlign == StiTextHorAlignment.Right) horAlign = StiTextHorAlignment.Left;
                                            }
                                            seo.Alignment = horAlign;
                                        }

                                        var baseState = new StiTextRenderer.StiHtmlState(string.Empty);
                                        var listStates = StiTextRenderer.ParseHtmlToStates(stiText.Text.Value, baseState);
                                        var sb = new StringBuilder();
                                        foreach (StiTextRenderer.StiHtmlState state in listStates)
                                        {
                                            sb.Append(state.Text);
                                        }
                                        seo.Text = sb.ToString().Replace("\n", "");
                                    }
                                    #endregion
                                }
                                else
                                {
                                    RenderTextFont(pp);
                                    RenderText(pp2);
                                }

                                #region end annot
                                if ((annotsCounter > 0 || annots2Counter > 0) && stiText.Editable)
                                {
                                    pageStream.Flush();
                                    int memPos2 = (int)pageStream.BaseStream.Position;
                                    pageStream.BaseStream.Seek(memPos, SeekOrigin.Begin);
                                    byte[] bufAnnot = new byte[memPos2 - memPos];
                                    pageStream.BaseStream.Read(bufAnnot, 0, memPos2 - memPos);
                                    pageStream.BaseStream.Seek(memPos, SeekOrigin.Begin);
                                    //								memoryPageStream.SetLength(memPos);
                                    StiEditableObject seo = (StiEditableObject)annotsArray[annotsCurrent];
                                    seo.Content = bufAnnot;
                                    annotsCurrent++;
                                    //some correction - GS is not supported in editable
                                    PopColorFromStack();
                                }
                                #endregion
                            }
                            if (component is StiShape) RenderShape(pp, imageResolution);

                            else if (component is StiBarCode && !isExportAsImage)
                            {
                                #region Render barcode
                                StiPdfGeomWriter pdfGeomWriter = new StiPdfGeomWriter(pageStream, this);
                                StiBarCodeExportPainter barCodePainter = new StiBarCodeExportPainter(pdfGeomWriter);
                                StiBarCode barCode = component as StiBarCode;
                                pdfGeomWriter.componentAngle = -(int)barCode.Angle;
                                if (!string.IsNullOrEmpty(barCode.CodeValue) && barCode.Page != null)
                                {
                                    pageStream.WriteLine("q");
                                    PushColorToStack();
                                    pageStream.WriteLine("{0} {1} {2} {3} re W n",
                                        ConvertToString(pp.X),
                                        ConvertToString(pp.Y),
                                        ConvertToString(pp.Width),
                                        ConvertToString(pp.Height));
                                    pageStream.WriteLine("1 0 0 1 {0} {1} cm",
                                        ConvertToString(mgLeft),
                                        ConvertToString(pageHeight - mgTop));
                                    pageStream.WriteLine("1 0 0 -1 0 0 cm");
                                    pageStream.WriteLine("{0} 0 0 {0} 0 0 cm", ConvertToString(hiToTwips));

                                    var matrix = new Matrix(1, 0, 0, 1, 0, 0);
                                    matrix.Translate((float)mgLeft, (float)(pageHeight - mgTop));
                                    matrix.Scale(1, -1);
                                    matrix.Scale((float)hiToTwips, (float)hiToTwips);
                                    pdfGeomWriter.pageNumber = pageNumber;
                                    pdfGeomWriter.matrixCache.Push(matrix);

                                    RectangleF rectf = report.Unit.ConvertToHInches(barCode.ClientRectangle).ToRectangleF();
                                    barCode.BarCodeType.Draw(barCodePainter, barCode, rectf, 1);

                                    pageStream.WriteLine("Q");
                                    PopColorFromStack();
                                }
                                #endregion
                            }

                            else if (component is StiChart && !isExportAsImage)
                            {
                                #region Render chart
                                pageStream.WriteLine("q");
                                PushColorToStack();

                                //pageStream.WriteLine("{0} {1} {2} {3} re W n",
                                //    ConvertToString(pp.X),
                                //    ConvertToString(pp.Y),
                                //    ConvertToString(pp.Width),
                                //    ConvertToString(pp.Height));
                                pageStream.WriteLine("1 0 0 1 {0} {1} cm",
                                    ConvertToString(pp.X),
                                    ConvertToString(pp.Y + pp.Height));
                                pageStream.WriteLine("1 0 0 -1 0 0 cm");
                                pageStream.WriteLine("{0} 0 0 {0} 0 0 cm", ConvertToString(hiToTwips / 0.96));

                                RenderChart(pp, false, pageNumber - 1);

                                pageStream.WriteLine("Q");
                                PopColorFromStack();
                                #endregion
                            }
                            else if (component is StiGauge && !isExportAsImage)
                            {
                                #region Render gauge
                                pageStream.WriteLine("q");
                                PushColorToStack();

                                pageStream.WriteLine("1 0 0 1 {0} {1} cm",
                                    ConvertToString(pp.X),
                                    ConvertToString(pp.Y + pp.Height));
                                pageStream.WriteLine("1 0 0 -1 0 0 cm");
                                pageStream.WriteLine("{0} 0 0 {0} 0 0 cm", ConvertToString(hiToTwips / 0.96));

                                RenderGauge(pp, false, pageNumber - 1);

                                pageStream.WriteLine("Q");
                                PopColorFromStack();
                                #endregion
                            }
                            else if (component is StiMap && !isExportAsImage)
                            {
                                #region Render map
                                pageStream.WriteLine("q");
                                PushColorToStack();

                                pageStream.WriteLine("1 0 0 1 {0} {1} cm",
                                    ConvertToString(pp.X),
                                    ConvertToString(pp.Y + pp.Height));
                                pageStream.WriteLine("1 0 0 -1 0 0 cm");
                                pageStream.WriteLine("{0} 0 0 {0} 0 0 cm", ConvertToString(hiToTwips / 0.96));

                                RenderMap(pp, false, pageNumber - 1);

                                pageStream.WriteLine("Q");
                                PopColorFromStack();
                                #endregion
                            }
                            else if (component is StiSparkline && !isExportAsImage)
                            {
                                #region Render sparkline
                                pageStream.WriteLine("q");
                                PushColorToStack();

                                pageStream.WriteLine("1 0 0 1 {0} {1} cm",
                                    ConvertToString(pp.X),
                                    ConvertToString(pp.Y + pp.Height));
                                pageStream.WriteLine("1 0 0 -1 0 0 cm");
                                pageStream.WriteLine("{0} 0 0 {0} 0 0 cm", ConvertToString(hiToTwips / 0.96));

                                RenderSparkline(pp, false, pageNumber - 1);

                                pageStream.WriteLine("Q");
                                PopColorFromStack();
                                #endregion
                            }
                            else if (component is StiElectronicSignature)
                            {
                                #region Render StiElectronicSignature
                                isExportAsImage = false;

                                var signature = component as StiElectronicSignature;
                                StiPdfData pp2 = pp.Clone();

                                if (signature.Mode == StiSignatureMode.Draw)
                                {
                                    if ((signature.Image?.Image != null) && (signature.Image.Image.Length > 0))
                                    {
                                        var img = new StiImage(new RectangleD(component.Left, component.Top, component.Width, component.Height));
                                        img.ImageBytes = signature.Image.Image;
                                        img.HorAlignment = signature.Image.HorAlignment;
                                        img.VertAlignment = signature.Image.VertAlignment;
                                        img.Stretch = signature.Image.Stretch;
                                        img.AspectRatio = signature.Image.AspectRatio;
                                        img.Page = page;

                                        pp2.Component = img;
                                        RenderImage(pp2, imageResolution, true);
                                    }
                                    if (signature.Draw.Image != null)
                                    {
                                        var img = new StiImage(new RectangleD(component.Left, component.Top, component.Width, component.Height));
                                        img.ImageBytes = signature.Draw.Image;
                                        img.HorAlignment = signature.Draw.HorAlignment;
                                        img.VertAlignment = signature.Draw.VertAlignment;
                                        img.Stretch = signature.Draw.Stretch;
                                        img.AspectRatio = signature.Draw.AspectRatio;
                                        img.Page = page;

                                        pp2.Component = img;
                                        imageInfoCounter++;
                                        RenderImage(pp2, imageResolution, true);
                                        imageInfoCounter--;
                                    }
                                    if (!string.IsNullOrWhiteSpace(signature.Text?.Text))
                                    {
                                        var stt = new StiText(new RectangleD(component.Left, component.Top, component.Width, component.Height));
                                        stt.Text = signature.Text?.Text;
                                        stt.TextBrush = new StiSolidBrush(signature.Text.Color);
                                        stt.Font = signature.Text.Font;
                                        stt.HorAlignment = signature.Text.HorAlignment;
                                        stt.VertAlignment = signature.Text.VertAlignment;
                                        stt.Page = page;
                                        stt.TextQuality = StiTextQuality.Standard;

                                        pp2.Component = stt;
                                        RenderTextFont(pp2);
                                        RenderText(pp2);
                                    }
                                }
                                if (signature.Mode == StiSignatureMode.Type)
                                {
                                    string signText = $" {signature.Type.FullName} {signature.Type.Initials}";
                                    if (!string.IsNullOrWhiteSpace(signText))
                                    {
                                        var signFont = new Font(SignatureFonts.StiSignatureFontsHelper.GetFont(signature.Type.Style), 16);

                                        var stt = new StiText(new RectangleD(component.Left, component.Top, component.Width, component.Height));
                                        stt.Text = signText;
                                        stt.TextBrush = new StiSolidBrush(StiUX.Foreground);
                                        stt.Font = signFont;
                                        stt.HorAlignment = StiTextHorAlignment.Left;
                                        stt.VertAlignment = StiVertAlignment.Center;
                                        stt.Page = page;
                                        stt.TextQuality = StiTextQuality.Standard;

                                        pp2.Component = stt;
                                        RenderTextFont(pp2);
                                        RenderText(pp2);
                                    }
                                }
                                #endregion
                            }
                            else if (component is StiPdfDigitalSignature)
                            {
                                #region Render StiPdfDigitalSignature
                                isExportAsImage = false;
                                var signature = component as StiPdfDigitalSignature;
                                if (!string.IsNullOrWhiteSpace(signature.Placeholder))
                                {
                                    var stt = new StiText(new RectangleD(component.Left, component.Top, component.Width, component.Height));
                                    stt.Text = signature.Placeholder;
                                    stt.TextBrush = new StiSolidBrush(Color.Red);
                                    stt.Font = new Font("Segoe UI", 10f);
                                    stt.HorAlignment = StiTextHorAlignment.Center;
                                    stt.VertAlignment = StiVertAlignment.Center;
                                    stt.Page = page;
                                    stt.TextQuality = StiTextQuality.Standard;

                                    StiPdfData pp2 = pp.Clone();
                                    pp2.Component = stt;
                                    RenderTextFont(pp2);
                                    RenderText(pp2);
                                }
                                #endregion
                            }
                            else
                            {
                                if (!exportRtfTextAsImage && (component is StiRichText))
                                {
                                    mfRender.RenderRtf(pp);
                                }
                                else
                                {
                                    if (component is StiCheckBox)
                                    {
                                        #region Render checkbox
                                        StiCheckBox checkbox = component as StiCheckBox;
                                        if (haveAnnots && (allowEditable == StiPdfAllowEditable.Yes) && checkbox.Editable)
                                        {
                                            #region store annots info
                                            StiEditableObject seo = annots2Array[annots2Current];
                                            seo.Multiline = false;
                                            seo.X = pp.X;
                                            seo.Y = pp.Y;
                                            seo.Width = pp.Width;
                                            seo.Height = pp.Height;
                                            seo.Component = pp.Component;
                                            seo.Alignment = StiTextHorAlignment.Center;
                                            seo.Text = "";
                                            #endregion

                                            double storeX = pp.X;
                                            double storeY = pp.Y;
                                            pp.X = 0;
                                            pp.Y = 0;

                                            #region prepare annot
                                            pageStream.Flush();
                                            int memPos = (int)pageStream.BaseStream.Position;
                                            //some correction - GS is not supported in editable
                                            PushColorToStack();
                                            lastColorStrokeA = 0xFF;
                                            lastColorNonStrokeA = 0xFF;
                                            #endregion

                                            RenderCheckbox(pp, true);

                                            #region end annot
                                            pageStream.Flush();
                                            int memPos2 = (int)pageStream.BaseStream.Position;
                                            pageStream.BaseStream.Seek(memPos, SeekOrigin.Begin);
                                            byte[] bufAnnot = new byte[memPos2 - memPos];
                                            pageStream.BaseStream.Read(bufAnnot, 0, memPos2 - memPos);
                                            pageStream.BaseStream.Seek(memPos, SeekOrigin.Begin);
                                            //memoryPageStream.SetLength(memPos);
                                            seo.Content = bufAnnot;
                                            //some correction - GS is not supported in editable
                                            PopColorFromStack();
                                            #endregion

                                            #region prepare annot
                                            pageStream.Flush();
                                            memPos = (int)pageStream.BaseStream.Position;
                                            //some correction - GS is not supported in editable
                                            PushColorToStack();
                                            lastColorStrokeA = 0xFF;
                                            lastColorNonStrokeA = 0xFF;
                                            #endregion

                                            RenderCheckbox(pp, false, false);

                                            #region end annot
                                            pageStream.Flush();
                                            memPos2 = (int)pageStream.BaseStream.Position;
                                            pageStream.BaseStream.Seek(memPos, SeekOrigin.Begin);
                                            bufAnnot = new byte[memPos2 - memPos];
                                            pageStream.BaseStream.Read(bufAnnot, 0, memPos2 - memPos);
                                            pageStream.BaseStream.Seek(memPos, SeekOrigin.Begin);
                                            //memoryPageStream.SetLength(memPos);
                                            seo.Content2 = bufAnnot;
                                            //some correction - GS is not supported in editable
                                            PopColorFromStack();
                                            #endregion

                                            annots2Current++;

                                            pp.X = storeX;
                                            pp.Y = storeY;
                                        }
                                        else
                                        {
                                            bool? checkboxValue = GetCheckBoxValue(checkbox);
                                            if (checkboxValue != null)
                                            {
                                                RenderCheckbox(pp, checkboxValue.Value);
                                            }
                                        }
                                        #endregion
                                    }
                                    else if (isExportAsImage)
                                    {
                                        RenderImage(pp, imageResolution);
                                    }
                                }
                            }
                            if (component is StiLinePrimitive)
                            {
                                RenderPrimitives(pp);
                            }
                            else
                            {
                                IStiBorder mBorder = pp.Component as IStiBorder;
                                if ((mBorder != null) && (mBorder.Border != null) && (mBorder.Border.Topmost))
                                {
                                    storedBorders.Add(pp);
                                }
                                else
                                {
                                    RenderBorder2(pp);
                                }
                            }

                            if (!usePdfA && StiOptions.Export.Pdf.AllowPrintable && !component.Printable)
                            {
                                pageStream.WriteLine("EMC");
                            }
                        }

                        #region bookmarks on components
                        if (report.Bookmark.Engine == StiEngineVersion.EngineV1)
                        {
                            if ((haveBookmarks == true) && (component.BookmarkValue != null) && ((string)component.BookmarkValue != ""))
                            {
                                bool bookmarkFinded = false;
                                int tempIndex = 0;
                                while ((bookmarkFinded == false) && (tempIndex < bookmarksCounter))
                                {
                                    StiBookmarkTreeNode tn = (StiBookmarkTreeNode)bookmarksTree[tempIndex];
                                    if ((tn.Used == false) && (tn.Page == -1) && (tn.Title == (string)component.BookmarkValue))
                                    {
                                        tn.Page = pageNumber - 1;
                                        tn.Y = pp.Y + pp.Height;
                                        tn.Used = true;
                                        bookmarkFinded = true;
                                        bookmarksTree[tempIndex] = tn;
                                        break;
                                    }
                                    tempIndex++;
                                }
                            }
                        }
                        else
                        {
                            if ((haveBookmarks == true) && (!string.IsNullOrEmpty(component.Guid) || ((component.BookmarkValue != null) && ((string)component.BookmarkValue != ""))))
                            {
                                int tempIndex = 0;
                                while (tempIndex < bookmarksCounter)
                                {
                                    StiBookmarkTreeNode tn = (StiBookmarkTreeNode)bookmarksTree[tempIndex];
                                    if ((tn.Used == false) && (tn.Page == -1))
                                    {
                                        bool finded = false;
                                        if (!string.IsNullOrEmpty(tn.Guid))
                                        {
                                            if ((!string.IsNullOrEmpty(component.Guid)) && (tn.Guid == component.Guid)) finded = true;
                                        }
                                        else if (!string.IsNullOrEmpty(tn.Title))
                                        {
                                            if ((component.BookmarkValue != null) && (((string)component.BookmarkValue).Length > 0) && (tn.Title == (string)component.BookmarkValue)) finded = true;
                                        }

                                        if (finded)
                                        {
                                            tn.Page = pageNumber - 1;
                                            tn.Y = pp.Y + pp.Height;
                                            tn.Used = true;
                                            bookmarksTree[tempIndex] = tn;
                                            break;
                                        }
                                    }
                                    tempIndex++;
                                }
                            }
                        }
                        #endregion

                        #region hyperlinks on components
                        if ((component.HyperlinkValue != null) && (component.HyperlinkValue.ToString().Trim().Length > 0) && (!component.HyperlinkValue.ToString().Trim().StartsWith("javascript:")))
                        {
                            StiLinkObject stl = new StiLinkObject();
                            stl.Link = component.HyperlinkValue.ToString();
                            stl.X = pp.X;
                            stl.Y = pp.Y;
                            stl.Width = pp.Width;
                            stl.Height = pp.Height;
                            stl.Page = pageNumber;
                            stl.DestPage = -1;
                            stl.DestY = -1;
                            linksArray.Add(stl);
                        }
                        #endregion

                        #region tags on components
                        if ((component.TagValue != null) && (component.TagValue.ToString().Trim().Length > 0))
                        {
                            StiLinkObject stl = new StiLinkObject();
                            stl.Link = component.TagValue.ToString();
                            stl.X = pp.X;
                            stl.Y = pp.Y;
                            stl.Width = pp.Width;
                            stl.Height = pp.Height;
                            stl.Page = pageNumber - 1;
                            stl.DestPage = -1;
                            stl.DestY = -1;
                            tagsArray.Add(stl);

                            if (stl.Link.Trim().ToLower(CultureInfo.InvariantCulture) == "pdfunsignedsignaturefield")
                            {
                                var ste = new StiEditableObject();
                                ste.X = stl.X;
                                ste.Y = stl.Y;
                                ste.Width = stl.Width;
                                ste.Height = stl.Height;
                                ste.Page = stl.Page;
                                ste.Component = component;

                                unsignedSignaturesArray.Add(ste);
                            }
                        }
                        #endregion

                        #region Pointer on components
                        if ((component.PointerValue != null) && !string.IsNullOrWhiteSpace((string)component.PointerValue) && !string.IsNullOrWhiteSpace(component.Guid))
                        {
                            string pointerValue = $"#{component.PointerValue}#GUID#{component.Guid}";

                            StiLinkObject stl = new StiLinkObject();
                            stl.Link = pointerValue;
                            stl.X = pp.X;
                            stl.Y = pp.Y;
                            stl.Width = pp.Width;
                            stl.Height = pp.Height;
                            stl.Page = pageNumber - 1;
                            stl.DestPage = -1;
                            stl.DestY = -1;
                            tagsArray.Add(stl);
                        }
                        #endregion

                        #region tooltips on components
                        if ((component.ToolTipValue != null) && (component.ToolTipValue.ToString().Trim().Length > 0) && !usePdfA)
                        {
                            StiLinkObject stl = new StiLinkObject();
                            stl.Link = component.ToolTipValue.ToString().Trim();
                            stl.X = pp.X;
                            stl.Y = pp.Y;
                            stl.Width = pp.Width;
                            stl.Height = pp.Height;
                            stl.Page = pageNumber - 1;
                            stl.DestPage = -1;
                            stl.DestY = -1;
                            tooltipsArray.Add(stl);
                        }
                        #endregion

                        #region digital signature on component
                        if ((component.TagValue != null) && (component.TagValue.ToString().ToLower(CultureInfo.InvariantCulture) == "pdfdigitalsignature"))
                        {
                            signaturePlacement = new RectangleD(pp.X, pp.Y, pp.Width, pp.Height);
                        }
                        #endregion

                        #region StiSignature.UnsignedSignature
                        if (component is StiPdfDigitalSignature)
                        {
                            var ste = new StiEditableObject();
                            ste.X = pp.X;
                            ste.Y = pp.Y;
                            ste.Width = pp.Width;
                            ste.Height = pp.Height;
                            ste.Page = pageNumber - 1;
                            ste.Component = component;

                            unsignedSignaturesArray.Add(ste);
                        }
                        #endregion
                    }
                }

                foreach (StiPdfData ppd in storedBorders)
                {
                    RenderBorder2(ppd);
                }
                storedBorders.Clear();

                if (page.Border != null)
                {
                    StiPdfData pp = new StiPdfData();
                    pp.X = mgLeft;
                    pp.Y = mgBottom;
                    pp.Width = pageWidth - mgLeft - mgRight;
                    pp.Height = pageHeight - mgTop - mgBottom;
                    pp.Component = new StiContainer();
                    (pp.Component as StiContainer).Border = page.Border;
                    RenderBorder2(pp);
                }

                RenderWatermark(page, false, pageWidth, pageHeight, imageResolution);

                #endregion

                RenderPageFooter(pageHeight, pageWidth);
            }
            #endregion

            #region Remake bookmarks table
            bookmarksTreeTemp = null;
            if (haveBookmarks && deleteEmptyBookmarks)
            {
                //mark node path
                for (int indexNode = 0; indexNode < bookmarksTree.Count; indexNode++)
                {
                    StiBookmarkTreeNode tn = (StiBookmarkTreeNode)bookmarksTree[indexNode];
                    if (tn.Used)
                    {
                        while (tn.Parent != -1)
                        {
                            tn = (StiBookmarkTreeNode)bookmarksTree[tn.Parent];
                            tn.Used = true;
                        }
                    }
                }

                //make new Bookmarks
                StiBookmark rootBookmark = new StiBookmark();
                MakeBookmarkFromTree(rootBookmark, (StiBookmarkTreeNode)bookmarksTree[0]);
                bookmarksTreeTemp = bookmarksTree;
                bookmarksTree = new ArrayList();
                AddBookmarkNode(rootBookmark, -1);

                //add empty records
                int numberNodesToAdd = bookmarksCounter - bookmarksTree.Count;
                if (numberNodesToAdd > 0)
                {
                    StiBookmarkTreeNode tn = new StiBookmarkTreeNode();
                    tn.Parent = -1;
                    tn.First = -1;
                    tn.Last = -1;
                    tn.Prev = -1;
                    tn.Next = -1;
                    tn.Count = -1;
                    tn.Page = -1;
                    tn.Y = -1;
                    tn.Title = "";
                    for (int indexNode = 0; indexNode < numberNodesToAdd; indexNode++)
                    {
                        bookmarksTree.Add(tn);
                    }
                }

                //fill new Bookmarks
                Hashtable htNameToNode = new Hashtable();
                for (int indexBookmark = 0; indexBookmark < bookmarksTreeTemp.Count; indexBookmark++)
                {
                    StiBookmarkTreeNode tn = (StiBookmarkTreeNode)bookmarksTreeTemp[indexBookmark];
                    htNameToNode[tn.Title] = tn;
                }
                for (int indexBookmark = 0; indexBookmark < bookmarksTree.Count; indexBookmark++)
                {
                    StiBookmarkTreeNode tn = (StiBookmarkTreeNode)bookmarksTree[indexBookmark];
                    if (!string.IsNullOrEmpty(tn.Title))
                    {
                        StiBookmarkTreeNode tnOld = (StiBookmarkTreeNode)htNameToNode[tn.Title];
                        tn.Page = tnOld.Page;
                        tn.Y = tnOld.Y;
                    }
                }

                bookmarksTreeTemp = null;
            }
            #endregion

            RenderImageTable();
            RenderFontTable();
            RenderBookmarksTable();
            RenderPatternTable();
            RenderLinkTable();
            RenderEncodeRecord();
            RenderExtGStateRecord();
            RenderAnnotTable();
            RenderSignatureTable(report);
            RenderTooltipTable();
            RenderMetadata(report);
            RenderColorSpace();
            RenderAutoPrint();
            RenderEmbeddedFiles();
            RenderEndDoc();

            #region Clear
            imageList.Clear();
            imageList = null;
            imageInterpolationTable.Clear();
            imageInterpolationTable = null;
            imageCacheIndexToList.Clear();
            imageCacheIndexToList = null;
            imageInfoList.Clear();
            imageInfoList = null;
            xref = null;
            bookmarksTree = null;
            linksArray = null;
            tagsArray = null;
            tooltipsArray = null;
            annotsArray = null;
            annots2Array = null;
            unsignedSignaturesArray = null;
            shadingArray = null;
            hatchArray = null;
            colorStack = null;
            enc = null;

            if (graphicsForTextRenderer != null)
            {
                graphicsForTextRenderer.Dispose();
                graphicsForTextRenderer = null;
            }
            if (imageForGraphicsForTextRenderer != null)
            {
                imageForGraphicsForTextRenderer.Dispose();
                imageForGraphicsForTextRenderer = null;
            }

            pdfFont.Clear();
            pdfFont = null;
            //bidi.Clear();
            //bidi = null;

            imageCache.Clear();
            watermarkImageExist.Clear();

            mfRender.Clear();
            mfRender = null;
            tempGeomWriter = null;
            #endregion

            sw.Flush();

            if (haveDigitalSignature)
            {
                #region Sign document
                int fileLen = (int)stream.Length;
                int signDataLen2 = fileLen - (offsetSignatureData + signatureDataLen + 2);

                //write length of second data block to stream
                byte[] buf5 = Encoding.ASCII.GetBytes(signDataLen2.ToString());
                stream.Seek(offsetSignatureLen2, SeekOrigin.Begin);
                stream.Write(buf5, 0, buf5.Length);
                stream.Flush();

                //prepare data for creating signature
                byte[] buf = new byte[offsetSignatureData + signDataLen2];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(buf, 0, offsetSignatureData);
                stream.Seek(offsetSignatureData + signatureDataLen + 2, SeekOrigin.Begin);
                stream.Read(buf, offsetSignatureData, signDataLen2);

                bool isGost = false;
                string signedBy = digitalSignatureSignedBy;

                byte[] signData = pdfSecurity.CreateSignature(buf, getCertificateFromCryptoUI, subjectNameString, useLocalMachineCertificates, certificateData, certificatePassword, certificateThumbprint, out isGost, ref signedBy, offsetSignatureFilter, offsetSignatureName);
                byte[] buf4 = Encoding.ASCII.GetBytes(BitConverter.ToString(signData).Replace("-", "").ToLower(CultureInfo.InvariantCulture));

                //write signature to stream
                stream.Seek(offsetSignatureData + 1, SeekOrigin.Begin);
                stream.Write(buf4, 0, buf4.Length);

                if (isGost)
                {
                    byte[] buf6 = Encoding.ASCII.GetBytes("CryptoPro#20PDF");
                    stream.Seek(offsetSignatureFilter, SeekOrigin.Begin);
                    stream.Write(buf6, 0, buf6.Length);
                }
                if (string.IsNullOrEmpty(digitalSignatureSignedBy) && !string.IsNullOrEmpty(signedBy))
                {
                    stream.Seek(offsetSignatureName, SeekOrigin.Begin);
                    StoreString(signedBy);
                }

                stream.Seek(fileLen, SeekOrigin.Begin);
                stream.Flush();
                #endregion
            }

        }
        #endregion
    }
}
