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
using System.Drawing.Imaging;

namespace Stimulsoft.Report.Export
{
    #region StiTiffCompressionScheme
    /// <summary>
    /// Enumeration for setting compression scheme of the exported Tiff image.
    /// </summary>
    public enum StiTiffCompressionScheme
    {
        Default = EncoderValue.Flush,
        LZW = EncoderValue.CompressionLZW,
        CCITT3 = EncoderValue.CompressionCCITT3,
        CCITT4 = EncoderValue.CompressionCCITT4,
        Rle = EncoderValue.CompressionRle,
        None = EncoderValue.CompressionNone
    }
    #endregion

	#region StiHtmlExportMode
	/// <summary>
	/// Enumeration which sets an exported mode for the Html export.
	/// </summary>
	public enum StiHtmlExportMode
	{
		/// <summary>
		/// A span tag of the HTML will be used for the exporting of the rendered document.
		/// </summary>
		Span = 1,
		/// <summary>
		/// A div tag of the HTML will be used for the exporting of the rendered document.
		/// </summary>
		Div = 2,
		/// <summary>
		/// A table tag of the HTML will be used for the exporting of the rendered document.
		/// </summary>
		Table = 3
	}
	#endregion

	#region StiHtmlExportQuality
	/// <summary>
	/// Enumeration which sets a quality of images which will be exported.
	/// </summary>
	public enum StiHtmlExportQuality
	{
		/// <summary>
		/// Sets a high quality of the exported images.
		/// </summary>
		High = 1,
        
		/// <summary>
		/// Sets a low quality of the exported images.
		/// </summary>
		Low = 2
	}
	#endregion

	#region StiUserAccessPrivileges
	/// <summary>
	/// Enumeration describes possible user access privileges to the pdf document. 
	/// User access privileges are managed by the user password.
	/// Owner with the correct owner password has all possible privileges for the content of the 
	/// pdf document and a rule for setting document permissions.
	/// </summary>
	[Flags]
	public enum StiUserAccessPrivileges 
	{
		/// <summary>
		/// User password allows only opening the pdf document, decrypt it, and display it on the screen.
		/// </summary>
		None = 0,
		/// <summary>
		/// User password allows opening the pdf document, decrypt it, display it on the screen and print 
		/// its content.
		/// </summary>
		PrintDocument = 1,
		/// <summary>
		/// User password allows modifying the content of the pdf document.
		/// </summary>
		ModifyContents = 2,
		/// <summary>
		/// User password allows copying text and graphics objects from the content of the pdf document.
		/// </summary>
		CopyTextAndGraphics = 4,
		/// <summary>
		/// User password allows adding or modifying text annotations in the content of the pdf document.
		/// </summary>
		AddOrModifyTextAnnotations = 8,
		/// <summary>
		/// User password allows all modifications on the content of the pdf document.
		/// </summary>
		All = 15
	}
	#endregion

	#region StiPdfEncryptionKeyLength
	/// <summary>
	/// Enumeration which sets an encryption key length of the resulting pdf file.
	/// </summary>
	public enum StiPdfEncryptionKeyLength 
	{
		/// <summary>
        /// RC4 algorithm, 40 bit encryption key length (Acrobat 3).
		/// </summary>
		Bit40 = 1,
		/// <summary>
        /// RC4 algorithm, 128 bit encryption key length (Acrobat 5).
		/// </summary>
		Bit128 = 2,
		/// <summary>
        /// AES algorithm, 128 bit encryption key length, revision 4 (Acrobat 7).
		/// </summary>
		Bit128_r4 = 3,
        /// <summary>
        /// AES algorithm, 256 bit encryption key length, revision 5 (Acrobat 9).
        /// </summary>
        Bit256_r5 = 4,
        /// <summary>
        /// AES algorithm, 256 bit encryption key length, revision 6 (Acrobat X).
		/// </summary>
		Bit256_r6 = 5
	}
	#endregion

    #region StiPdfImageCompressionMethod
    /// <summary>
    /// Enumeration which sets an image compression method for PDF export.
    /// </summary>
    public enum StiPdfImageCompressionMethod
    {
        /// <summary>
        /// A Jpeg method (DCTDecode) will be used for the exporting of the rendered document.
        /// </summary>
        Jpeg = 1,
        /// <summary>
        /// A Flate method (FlateDecode) will be used for the exporting of the rendered document.
        /// </summary>
        Flate = 2,
        /// <summary>
        /// A Indexed method (IndexedColors + FlateDecode) will be used for the exporting of the rendered document.
        /// </summary>
        Indexed = 3
    }
    #endregion

    #region StiPdfAutoPrintMode
    /// <summary>
    /// Enumeration which sets an AutoPrint mode for pdf files
    /// </summary>
    public enum StiPdfAutoPrintMode
    {
        /// <summary>
        /// Do not use AutoPrint feature
        /// </summary>
        None = 1,
        /// <summary>
        /// Use printing with print dialog
        /// </summary>
        Dialog = 2,
        /// <summary>
        /// Use silent printing
        /// </summary>
        Silent = 3
    }
    #endregion

	#region StiTxtBorderType
	/// <summary>
	/// Enumeration describes a type of the border.
	/// </summary>
	public enum StiTxtBorderType
	{
		/// <summary>
		/// A border which consists of "+","-","|" symbols.
		/// </summary>
		Simple = 1,

		/// <summary>
		/// A border which consists of character graphics symbols. 
		/// A Single type of the border.
		/// </summary>
		UnicodeSingle = 2,

		/// <summary>
		/// A border consists of character graphics symbols. 
		/// A Double type of the border.
		/// </summary>
		UnicodeDouble = 3
	}
	#endregion

	#region StiPcxPaletteType
	/// <summary>
	/// Enumeration describes a type of palette of the PCX file.
	/// </summary>
	public enum StiPcxPaletteType
	{
		/// <summary>
		/// Monochrome palette (1 bit)
		/// </summary>
		Monochrome = 1,
		/// <summary>
		/// Color palette (24 bit)
		/// </summary>
		Color = 2
	}
	#endregion

	#region StiMonochromeDitheringType
	/// <summary>
	/// Enumeration describes a type of dithering for monochrome PCX file.
	/// </summary>
	public enum StiMonochromeDitheringType
	{
		/// <summary>
		/// Without dithering.
		/// Low quality, small size of file.
		/// </summary>
		None = 1,
		/// <summary>
		/// Floyd-Steinberg dithering.
		/// Good quality, big size of file.
		/// </summary>
		FloydSteinberg = 2,
		/// <summary>
		/// Ordered dithering with Bayer matrix 4x4.
		/// Poor quality, medium size of file.
		/// </summary>
		Ordered = 3
	}
	#endregion

    #region StiImageType
    /// <summary>
    /// Enumeration describes a type of the images for the exports.
    /// </summary>
    public enum StiImageType
    {
        Bmp = 1,
        Gif = 2,
        Jpeg = 3,
        Pcx = 4,
        Png = 5,
        Tiff = 6,
        Emf = 7,
        Svg = 8,
        Svgz = 9
    }
    #endregion

    #region StiHtmlType
    /// <summary>
    /// Enumeration describes a type of the html exports.
    /// </summary>
    public enum StiHtmlType
    {
        Html = 1,
        Html5 = 2,
        Mht = 3
    }
    #endregion

    #region StiHtmlChartType
    /// <summary>
    /// Enumeration describes a type of the chart in the html exports.
    /// </summary>
    public enum StiHtmlChartType
    {
        Image = 1,
        Vector = 2,
        AnimatedVector = 3
    }
    #endregion

    #region StiExcelType
    /// <summary>
    /// Enumeration describes a type of the excel exports.
    /// </summary>
    public enum StiExcelType
    {
        /// <summary>
        /// Excel format from Office 97 to Office 2003.
        /// </summary>
        ExcelBinary = 1,
        
        /// <summary>
        /// Xml Excel format starts from Office 2003.
        /// </summary>
        ExcelXml = 2,
        
        /// <summary>
        /// Excel format starts from Office 2007.
        /// </summary>
        Excel2007 = 3
    }
    #endregion

    #region StiDataType
    /// <summary>
    /// Enumeration describes a type of the data exports.
    /// </summary>
    public enum StiDataType
    {
        Csv = 1,
        Dbf = 2,
        Dif = 3,
        Sylk = 4,
        Xml = 5,
        Json = 6
    }
    #endregion

	#region StiExportPosition
	/// <summary>
	/// Enumeration which defines a position of the export item in the export menu.
	/// </summary>
	public enum StiExportPosition
	{	
		Pdf = 0,
		Xps = 1,
        Ppt2007 = 2,

		Html = 10,
        Html5 = 11,
        Mht = 12,

		Txt = 20,
		Rtf = 21,		
		Word2007 = 22,		
		Odt = 23,
				
		Excel = 30,
		ExcelXml = 31,
		Excel2007 = 32,		
		Ods = 33,
		
		Data = 40,

        [Obsolete("Please use Data value instead Dbf!")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        Dbf = 41,

        [Obsolete("Please use Data value instead Xml!")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        Xml = 42,

        [Obsolete("Please use Data value instead Dif!")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        Dif = 43,

        [Obsolete("Please use Data value instead Sylk!")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        Sylk = 44,

        Image = 50,
        
        [Obsolete("Please use Image value instead Bmp!")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        Bmp = 50,

        [Obsolete("Please use Image value instead Gif!")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        Gif = 51,

        [Obsolete("Please use Image value instead Jpeg!")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        Jpeg = 52,

        [Obsolete("Please use Image value instead Pcx!")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        Pcx = 53,

        [Obsolete("Please use Image value instead Png!")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        Png = 54,

        [Obsolete("Please use Image value instead Tiff!")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        Tiff = 55,

        [Obsolete("Please use Image value instead Emf!")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        Emf = 60,

        [Obsolete("Please use Image value instead Svg!")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        Svg = 61,

        [Obsolete("Please use Image value instead Svgz!")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        Svgz = 62
	}
	#endregion

    #region StiHtmlExportBookmarksMode
    public enum StiHtmlExportBookmarksMode
    {
        BookmarksOnly = 1,
        ReportOnly = 2,
        All = 3
    }
    #endregion

    #region StiDbfCodePages
    /// <summary>
    /// Enumeration for setting Code Pages.
    /// </summary>
    public enum StiDbfCodePages
    {
        /// <summary>
        /// A parameter indicating that the code page of the exported document will not be specified.
        /// </summary>
        Default = 0,
        /// <summary>
        /// A code page of the exported document is U.S. MS-DOS. Code page number 437.
        /// </summary>
        USDOS = 437,
        /// <summary>
        /// A code page of the exported document is Mazovia (Polish) MS-DOS. Code page number 620.
        /// </summary>
        MazoviaDOS = 620,
        /// <summary>
        /// A code page of the exported document is Greek MS-DOS (437G). Code page number 737.
        /// </summary>
        GreekDOS = 737,
        /// <summary>
        /// A code page of the exported document is International MS-DOS. Code page number 850.
        /// </summary>
        InternationalDOS = 850,
        /// <summary>
        /// A code page of the exported document is Eastern European MS-DOS. Code page number 852.
        /// </summary>
        EasternEuropeanDOS = 852,
        /// <summary>
        /// A code page of the exported document is Icelandic MS-DOS. Code page number 861.
        /// </summary>
        IcelandicDOS = 861,
        /// <summary>
        /// A code page of the exported document is Nordic MS-DOS. Code page number 865.
        /// </summary>
        NordicDOS = 865,
        /// <summary>
        /// A code page of the exported document is Russian MS-DOS. Code page number 866.
        /// </summary>
        RussianDOS = 866,
        /// <summary>
        /// A code page of the exported document is Kamenicky (Czech) MS-DOS. Code page number 895.
        /// </summary>
        KamenickyDOS = 895,
        /// <summary>
        /// A code page of the exported document is Turkish MS-DOS. Code page number 857.
        /// </summary>
        TurkishDOS = 857,
        /// <summary>
        /// A code page of the exported document is EasternEuropean MS-DOS. Code page number 1250.
        /// </summary>
        EasternEuropeanWindows = 1250,
        /// <summary>
        /// A code page of the exported document is Russian Windows. Code page number 1251.
        /// </summary>
        RussianWindows = 1251,
        /// <summary>
        /// A code page of the exported document is Windows ANSI. Code page number 1252.
        /// </summary>
        WindowsANSI = 1252,
        /// <summary>
        /// A code page of the exported document is Greek Windows. Code page number 1253.
        /// </summary>
        GreekWindows = 1253,
        /// <summary>
        /// A code page of the exported document is Turkish Windows. Code page number 1254.
        /// </summary>
        TurkishWindows = 1254,
        /// <summary>
        /// A code page of the exported document is Standard Macintosh. Code page number 10000.
        /// </summary>
        StandardMacintosh = 10000,
        /// <summary>
        /// A code page of the exported document is Greek Macintosh. Code page number 10006.
        /// </summary>
        GreekMacintosh = 10006,
        /// <summary>
        /// A code page of the exported document is Russian Macintosh. Code page number 10007.
        /// </summary>
        RussianMacintosh = 10007,
        /// <summary>
        /// A code page of the exported document is Eastern European Macintosh. Code page number 10029.
        /// </summary>
        EasternEuropeanMacintosh = 10029
    }
    #endregion

    #region StiExportDataType
    /// <summary>
    /// Enumeration for the types of data for the export. 
    /// </summary>
    public enum StiExportDataType
    {
        /// <summary>
        /// A string type.
        /// </summary>
        String = 0,
        /// <summary>
        /// An Integer32 type.
        /// </summary>
        Int,
        /// <summary>
        /// An Integer64 type.
        /// </summary>
        Long,
        /// <summary>
        /// A Float type.
        /// </summary>
        Float,
        /// <summary>
        /// A Double type.
        /// </summary>
        Double,
        /// <summary>
        /// A Data type.
        /// </summary>
        Date,
        /// <summary>
        /// A Boolean type.
        /// </summary>
        Bool
    }
    #endregion

    #region StiImageFormat
    /// <summary>
    /// Enumeration for setting format of the exported images.
    /// </summary>
    public enum StiImageFormat
    {
        /// <summary>
        /// Images are exported in the color mode.
        /// </summary>
        Color = 1,
        /// <summary>
        /// Images are exported in the grayscale mode.
        /// </summary>
        Grayscale = 2,
        /// <summary>
        /// Images are exported in the monochrome mode.
        /// </summary>
        Monochrome = 3
    }
    #endregion
    
    #region StiRtfExportMode
    /// <summary>
    /// Enumeration for setting modes of the rtf export.
    /// </summary>
    public enum StiRtfExportMode
    {
        /// <summary>
        /// 
        /// </summary>
        Table = 4,
        /// <summary>
        /// 
        /// </summary>
        Frame = 1,
        /// <summary>
        /// 
        /// </summary>
        WinWord = 2,
        /// <summary>
        /// 
        /// </summary>
        TabbedText = 3
    }
    #endregion

    #region StiDataExportMode
    /// <summary>
    /// Enumeration for setting modes of the data export.
    /// </summary>
    [Flags]
    public enum StiDataExportMode
    {
        /// <summary>
        /// 
        /// </summary>
        Data = 1,
        /// <summary>
        /// 
        /// </summary>
        Headers = 2,
        /// <summary>
        /// 
        /// </summary>
        DataAndHeaders = 3,
        /// <summary>
        /// 
        /// </summary>
        Footers = 4,
        /// <summary>
        /// 
        /// </summary>
        HeadersFooters = 6,
        /// <summary>
        /// 
        /// </summary>
        DataAndHeadersFooters = 7,
        /// <summary>
        /// 
        /// </summary>
        AllBands = 15
    }
    #endregion

    #region StiWord2007RestrictEditing
    /// <summary>
    /// Enumeration for setting modes of restrict editing
    /// </summary>
    public enum StiWord2007RestrictEditing
    {
        /// <summary>
        /// No restrictions
        /// </summary>
        No = 1,
        /// <summary>
        /// Except Editable fields
        /// </summary>
        ExceptEditableFields = 2,
        /// <summary>
        /// Yes
        /// </summary>
        Yes = 3
    }
    #endregion

    #region StiExcel2007RestrictEditing
    /// <summary>
    /// Enumeration for setting modes of restrict editing
    /// </summary>
    public enum StiExcel2007RestrictEditing
    {
        /// <summary>
        /// No restrictions
        /// </summary>
        No = 1,
        /// <summary>
        /// Except Editable fields
        /// </summary>
        ExceptEditableFields = 2,
        /// <summary>
        /// Yes
        /// </summary>
        Yes = 3
    }
    #endregion

    #region StiPdfAllowEditable
    /// <summary>
    /// Enumeration for setting modes of restrict editing
    /// </summary>
    public enum StiPdfAllowEditable
    {
        /// <summary>
        ///
        /// </summary>
        No = 1,
        /// <summary>
        ///
        /// </summary>
        Yes = 2
    }
    #endregion

    #region StiImageResolutionMode
    /// <summary>
    /// Enumeration for setting modes of using of image resolution
    /// </summary>
    public enum StiImageResolutionMode
    {
        /// <summary>
        ///
        /// </summary>
        Exactly = 1,
        /// <summary>
        ///
        /// </summary>
        NoMoreThan = 2,
        /// <summary>
        /// 
        /// </summary>
        Auto = 3
    }
    #endregion

    #region StiPdfComplianceMode
    /// <summary>
    /// Enumeration for setting modes of compliance
    /// </summary>
    public enum StiPdfComplianceMode
    {
        /// <summary>
        ///
        /// </summary>
        None,
        /// <summary>
        ///
        /// </summary>
        A1,
        /// <summary>
        /// 
        /// </summary>
        A2,
        /// <summary>
        /// 
        /// </summary>
        A3
    }
    #endregion

    #region StiPdfZUGFeRDComplianceMode
    /// <summary>
    /// Enumeration for setting modes of ZUGFeRD compliance
    /// </summary>
    public enum StiPdfZUGFeRDComplianceMode
    {
        /// <summary>
        ///
        /// </summary>
        None,
        /// <summary>
        ///
        /// </summary>
        V1,
        /// <summary>
        /// 
        /// </summary>
        V2,
        /// <summary>
        /// 
        /// </summary>
        V2_1
    }
    #endregion

    #region StiExcelSheetViewMode
    /// <summary>
    /// Enumeration for setting modes of Excel sheet view
    /// </summary>
    public enum StiExcelSheetViewMode
    {
        /// <summary>
        /// 
        /// </summary>
        Normal = 1,
        /// <summary>
        /// 
        /// </summary>
        PageLayout = 2,
        /// <summary>
        /// 
        /// </summary>
        PageBreakPreview = 3
    }
    #endregion

    #region StiPdfHighAccuracyMode
    /// <summary>
    /// Enumeration for setting modes of HighAccuracy
    /// </summary>
    public enum StiPdfHighAccuracyMode
    {
        /// <summary>
        /// Don't process any textbox
        /// </summary>
        No = 1,
        /// <summary>
        /// Process only textboxes with wordwrap
        /// </summary>
        WordwrapOnly = 2,
        /// <summary>
        /// Process all textboxes
        /// </summary>
        All = 3
    }
    #endregion

    #region StiImageCutEdgesMode
    /// <summary>
    /// Enumeration for the setting of edge cutting modes
    /// </summary>
    public enum StiImageCutEdgesMode
    {
        /// <summary>
        /// Cut margins except feed
        /// </summary>
        ExceptFeed,
        /// <summary>
        /// Cut margins completely
        /// </summary>
        Full
    }
    #endregion

}
