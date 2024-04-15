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
using System.Globalization;
using System.Text;
using System.Collections;
using System.IO;
using Stimulsoft.Report.Components;
using Stimulsoft.Base;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Services;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;
using Stimulsoft.Report.Helpers;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

#if STIDRAWING
using ImageEncoder = Stimulsoft.Drawing.Imaging.Encoder;
using ImageCodecInfo = Stimulsoft.Drawing.Imaging.ImageCodecInfo;
using Image = Stimulsoft.Drawing.Image;
using ImageFormat = Stimulsoft.Drawing.Imaging.ImageFormat;
using EncoderParameter = Stimulsoft.Drawing.Imaging.EncoderParameter;
using EncoderParameters = Stimulsoft.Drawing.Imaging.EncoderParameters;
#else
using ImageEncoder = System.Drawing.Imaging.Encoder;
#endif

namespace Stimulsoft.Report.Export
{
    /// <summary>
    /// A class for the Excel Export.
    /// </summary>
    [StiServiceBitmap(typeof(StiExportService), "Stimulsoft.Report.Images.Dictionary.ResourceExcel.png")]
    public class StiExcelExportService : StiExportService
    {
        #region StiExportService override
        /// <summary>
        /// Gets or sets a default extension of export. 
        /// </summary>
        public override string DefaultExtension
        {
            get
            {
                if (exportSettings is StiExcel2007ExportSettings) return "xlsx";
                if (exportSettings is StiExcelExportSettings && (exportSettings as StiExcelExportSettings).ExcelType == StiExcelType.Excel2007) return "xlsx";
                return "xls";
            }
        }

        public override StiExportFormat ExportFormat
        {
            get
            {
                if (exportSettings is StiExcel2007ExportSettings) return StiExportFormat.Excel2007;
                if (exportSettings is StiExcelXmlExportSettings) return StiExportFormat.ExcelXml;
                if (exportSettings is StiExcelExportSettings)
                {
                    StiExcelType excelType = (exportSettings as StiExcelExportSettings).ExcelType;
                    if (excelType == StiExcelType.Excel2007) return StiExportFormat.Excel2007;
                    if (excelType == StiExcelType.ExcelXml) return StiExportFormat.ExcelXml;
                }
                return StiExportFormat.Excel;
            }
        }


        /// <summary>
        /// Gets a group of the export in the context menu.
        /// </summary>
        public override string GroupCategory => "Excel";


        /// <summary>
        /// Gets a position of the export in the context menu.
        /// </summary>
        public override int Position => (int)StiExportPosition.Excel;


        /// <summary>
        /// Gets an export name in the context menu.
        /// </summary>
        public override string ExportNameInMenu => StiLocalization.Get("Export", "ExportTypeExcelFile");

        /// <summary>
        /// Exports a document to the stream without dialog of the saving file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream in which report will be exported.</param>
        /// <param name="settings">A settings for the report exporting.</param>
        public override void ExportTo(StiReport report, Stream stream, StiExportSettings settings)
        {
            exportSettings = settings;

            var excelSettings = settings as StiExcelExportSettings;
            var excelType = excelSettings != null ? excelSettings.ExcelType : StiExcelType.ExcelBinary;

            if (excelType == StiExcelType.ExcelXml || excelSettings is StiExcelXmlExportSettings)
            {
                var excelXmlExportService = new StiExcelXmlExportService();
                excelXmlExportService.ExportExcel(report, stream, excelSettings as StiExcelXmlExportSettings);
            }
            else if (excelType == StiExcelType.Excel2007 || excelSettings is StiExcel2007ExportSettings)
            {
                var excel2007ExportService = new StiExcel2007ExportService();
                excel2007ExportService.ExportExcel(report, stream, excelSettings as StiExcel2007ExportSettings);
            }
            else if (excelType == StiExcelType.ExcelBinary)
            {
                var excelBinaryExportService = new StiExcelExportService();
                excelBinaryExportService.ExportExcel(report, stream, excelSettings);
            }
            else
            {
                var excel2007ExportService = new StiExcel2007ExportService();
                excel2007ExportService.ExportExcel(report, stream, excelSettings);
            }
            InvokeExporting(100, 100, 1, 1);
        }

        /// <summary>
        /// Exports a rendered report to the Excel file.
        /// Also exported document can be sent via e-mail.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="fileName">A name of the file for exporting a rendered report.</param>
        /// <param name="sendEMail">A parameter indicating whether the exported report will be sent via e-mail.</param>
        public override void Export(StiReport report, string fileName, bool sendEMail, StiGuiMode guiMode)
        {
            using (var form = StiGuiOptions.GetExportFormRunner("StiExcelSetupForm", guiMode, this.OwnerWindow))
            {
                form["CurrentPage"] = report.CurrentPrintPage;
                form["OpenAfterExportEnabled"] = !sendEMail;
                if (!string.IsNullOrWhiteSpace(fileName))
                {
                    form["ExportFormat"] = StiExportFormat.Excel;
                }

                this.report = report;
                this.fileName = fileName;
                this.sendEMail = sendEMail;
                this.guiMode = guiMode;
                form.Complete += Form_Complete;
                form.ShowDialog();
            }
        }

        /// <summary>
        /// Gets a value indicating a number of files in exported document as a result of export
        /// of one page of the rendered report.
        /// </summary>
        public override bool MultipleFiles => false;

        /// <summary>
        /// Returns a filter for the Excel files.
        /// </summary>
        /// <returns>Returns a filter for the Excel files.</returns>
		public override string GetFilter() => StiLocalization.Get("FileFilters", "ExcelFiles");
        #endregion

        #region Handlers
        private void Form_Complete(IStiFormRunner form, StiShowDialogCompleteEvetArgs e)
        {
            if (e.DialogResult)
            {
                var exportFormat = (StiExportFormat)form["ExportFormat"];

                #region Excel2007

                if (exportFormat == StiExportFormat.Excel2007)
                {
                    var excel2007ExportService = new StiExcel2007ExportService();
                    if (string.IsNullOrEmpty(fileName))
                        fileName = excel2007ExportService.GetFileName(report, sendEMail);

                    if (fileName != null)
                    {
                        StiFileUtils.ProcessReadOnly(fileName);
                        try
                        {
                            using (var stream = new FileStream(fileName, FileMode.Create))
                            {
                                excel2007ExportService.StartProgress(guiMode);

                                var settings = new StiExcel2007ExportSettings
                                {
                                    PageRange = form["PagesRange"] as StiPagesRange,
                                    UseOnePageHeaderAndFooter = (bool)form["UseOnePageHeaderAndFooter"],
                                    DataExportMode = (StiDataExportMode)form["DataExportMode"],
                                    ExportObjectFormatting = (bool)form["ExportObjectFormatting"],
                                    ExportEachPageToSheet = (bool)form["ExportEachPageToSheet"],
                                    ExportPageBreaks = (bool)form["ExportPageBreaks"],
                                    ImageResolution = (float)form["Resolution"],
                                    ImageQuality = (float)form["ImageQuality"],
                                    RestrictEditing = (StiExcel2007RestrictEditing)form["RestrictEditing"]
                                };

                                exportSettings = settings;

                                excel2007ExportService.StartExport(report, stream, settings, sendEMail, (bool)form["OpenAfterExport"], fileName, guiMode);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }

                #endregion

                #region ExcelXml

                else if (exportFormat == StiExportFormat.ExcelXml)
                {
                    var excelXmlExportService = new StiExcelXmlExportService();
                    if (string.IsNullOrEmpty(fileName))
                        fileName = excelXmlExportService.GetFileName(report, sendEMail);

                    if (fileName != null)
                    {
                        StiFileUtils.ProcessReadOnly(fileName);
                        try
                        {
                            using (var stream = new FileStream(fileName, FileMode.Create))
                            {
                                excelXmlExportService.StartProgress(guiMode);

                                var settings = new StiExcelXmlExportSettings
                                {
                                    PageRange = form["PagesRange"] as StiPagesRange
                                };

                                exportSettings = settings;

                                excelXmlExportService.StartExport(report, stream, settings, sendEMail, (bool)form["OpenAfterExport"], fileName, guiMode);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }

                #endregion

                #region Excel

                else
                {
                    exportSettings = null;
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

                                var settings = new StiExcelExportSettings
                                {
                                    PageRange = form["PagesRange"] as StiPagesRange,
                                    UseOnePageHeaderAndFooter = (bool)form["UseOnePageHeaderAndFooter"],
                                    DataExportMode = (StiDataExportMode)form["DataExportMode"],
                                    ExportObjectFormatting = (bool)form["ExportObjectFormatting"],
                                    ExportEachPageToSheet = (bool)form["ExportEachPageToSheet"],
                                    ExportPageBreaks = (bool)form["ExportPageBreaks"],
                                    ImageResolution = (float)form["Resolution"],
                                    ImageQuality = (float)form["ImageQuality"]
                                };

                                exportSettings = settings;

                                base.StartExport(report, stream, settings, sendEMail, (bool)form["OpenAfterExport"], fileName,
                                    guiMode);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }

                #endregion
            }
            else IsStopped = true;
        }
        #endregion

        #region Fields
        private StiExportSettings exportSettings;
        private StiReport report;
        private string fileName;
        private bool sendEMail;
        private StiGuiMode guiMode;
        #endregion

        #region this
        private StiMatrix matrix = null;
        internal StiMatrix Matrix
        {
            get
            {
                return matrix;
            }
        }

        private ArrayList xfList = null;
        private ArrayList colorList = null;
        private ArrayList fontList = null;
        private ArrayList sstList = null;
        private Hashtable sstHash = null;
        private ArrayList formatList = null;
        private int sstCounter;
        private byte[] bfHeader = null;
        private byte[] bfData = null;
        private MemoryStream memst = null;
        private MemoryStream memStMain = null;
        private ArrayList imageList = null;
        private Escher escher = null;
        private const int maxRecordLength = 8224;
        private bool useOnePageHeaderAndFooter = false;
        private StiDataExportMode dataExportMode = StiDataExportMode.Data | StiDataExportMode.Headers;
        private bool exportObjectFormatting = true;
        private bool exportEachPageToSheet = false;
        private ArrayList pagesList = null;
        private ArrayList bookmarksList = null;
        private ArrayList hlinksList = null;
        private ArrayList boundsheetsOffsetsList = null;
        private ArrayList boundsheetsNamesList = null;
        private float imageResolution = 1;
        private float imageQuality = 0.75f;
        private ImageCodecInfo imageCodec = null;
        private bool exportHorizontalPageBreaks = false;
        private int minRowIndex = 0;
        private int maxRowIndex = 0;
        private int maximumSheetHeight = 65534;
        private CultureInfo reportCulture = null;


        #region struct DataXF
        private struct DataXF
        {
            public int FontIndex;
            public int FormatIndex;
            public int XFType;
            public int ParentStyleXF;
            public int HorAlign;
            public int VertAlign;
            public int TextWrapped;
            public int TextRotationAngle;
            public int TextDirection;
            public int UsedAttrib;
            public int LineStyleLeft;
            public int LineStyleRight;
            public int LineStyleTop;
            public int LineStyleBottom;
            public int ColorIndexLeft;
            public int ColorIndexRight;
            public int ColorIndexTop;
            public int ColorIndexBottom;
            public int FillPatern;
            public int ColorIndexPattern;
            public int ColorIndexBackground;

            public DataXF(
                int FontIndex,
                int FormatIndex,
                int XFType,
                int ParentStyleXF,
                int HorAlign,
                int VertAlign,
                int TextWrapped,
                int TextRotationAngle,
                int TextDirection,
                int UsedAttrib,
                int LineStyleLeft,
                int LineStyleRight,
                int LineStyleTop,
                int LineStyleBottom,
                int ColorIndexLeft,
                int ColorIndexRight,
                int ColorIndexTop,
                int ColorIndexBottom,
                int FillPatern,
                int ColorIndexPattern,
                int ColorIndexBackground)
            {
                this.FontIndex = FontIndex;
                this.FormatIndex = FormatIndex;
                this.XFType = XFType;
                this.ParentStyleXF = ParentStyleXF;
                this.HorAlign = HorAlign;
                this.VertAlign = VertAlign;
                this.TextWrapped = TextWrapped;
                this.TextRotationAngle = TextRotationAngle;
                this.TextDirection = TextDirection;
                this.UsedAttrib = UsedAttrib;
                this.LineStyleLeft = LineStyleLeft;
                this.LineStyleRight = LineStyleRight;
                this.LineStyleTop = LineStyleTop;
                this.LineStyleBottom = LineStyleBottom;
                this.ColorIndexLeft = ColorIndexLeft;
                this.ColorIndexRight = ColorIndexRight;
                this.ColorIndexTop = ColorIndexTop;
                this.ColorIndexBottom = ColorIndexBottom;
                this.FillPatern = FillPatern;
                this.ColorIndexPattern = ColorIndexPattern;
                this.ColorIndexBackground = ColorIndexBackground;
            }
        }
        #endregion

        #region struct DataFont
        private struct DataFont
        {
            public string Name;
            public bool Bold;
            public bool Italic;
            public bool Underlined;
            public bool Strikeout;
            public int Height;
            public int Color;
            public int Charset;

            public DataFont(
                string Name,
                bool Bold,
                bool Italic,
                bool Underlined,
                bool Strikeout,
                int Height,
                int Color,
                int Charset)
            {
                this.Name = Name;
                this.Bold = Bold;
                this.Italic = Italic;
                this.Underlined = Underlined;
                this.Strikeout = Strikeout;
                this.Height = Height;
                this.Color = Color;
                this.Charset = Charset;
            }
        }
        #endregion

        #region struct CellRangeAddress
        private struct CellRangeAddress
        {
            public int FirstRow;
            public int LastRow;
            public int FirstColumn;
            public int LastColumn;

            public CellRangeAddress(
                int FirstRow,
                int LastRow,
                int FirstColumn,
                int LastColumn)
            {
                this.FirstRow = FirstRow;
                this.LastRow = LastRow;
                this.FirstColumn = FirstColumn;
                this.LastColumn = LastColumn;
            }
        }
        #endregion

        #region struct HlinkData
        private struct HlinkData
        {
            public CellRangeAddress Range;
            public string Description;
            public string Bookmark;

            public HlinkData(
                CellRangeAddress Range,
                string Description,
                string Bookmark)
            {
                this.Range = Range;
                this.Description = Description;
                this.Bookmark = Bookmark;
            }
        }
        #endregion

        private int GetLineStyle(StiBorderSide border)
        {
            StiPenStyle penStyle = StiPenStyle.None;
            double width = 0;
            const double mediumWidth = 1.5;
            const double thickWidth = 3;
            if (border != null)
            {
                penStyle = border.Style;
                width = border.Size;
            }
            switch (penStyle)
            {
                case StiPenStyle.Solid:
                    if (width > thickWidth) return 0x05;
                    if (width > mediumWidth) return 0x02;
                    return 0x01;

                case StiPenStyle.Dot:
                    if (width > mediumWidth) return 0x04;
                    return 0x07;

                case StiPenStyle.Dash:
                    if (width > mediumWidth) return 0x08;
                    return 0x03;

                case StiPenStyle.DashDot:
                    if (width > mediumWidth) return 0x0A;
                    return 0x09;

                case StiPenStyle.DashDotDot:
                    if (width > mediumWidth) return 0x0C;
                    return 0x0B;

                case StiPenStyle.Double:
                    return 0x06;

                default:
                    return 0x00;
            }
        }

        #region GetRefString
        private string GetRefString(int column, int row)
        {
            int columnHigh = column / 26;
            int columnLow = column % 26;
            StringBuilder output = new StringBuilder();
            if (columnHigh > 0)
            {
                output.Append((char)((byte)'A' + columnHigh - 1));
            }
            output.Append((char)((byte)'A' + columnLow));
            output.Append((row + 1).ToString());
            return output.ToString();
        }
        private string GetRefAbsoluteString(int column, int row)
        {
            int columnHigh = column / 26;
            int columnLow = column % 26;
            StringBuilder output = new StringBuilder();
            output.Append("$");
            if (columnHigh > 0)
            {
                output.Append((char)((byte)'A' + columnHigh - 1));
            }
            output.Append((char)((byte)'A' + columnLow));
            output.Append("$");
            output.Append((row + 1).ToString());
            return output.ToString();
        }
        #endregion

        #region GetXFNumber
        private int GetXFNumber(DataXF dataIn)
        {
            //			if (arrayXF.Count > 0)
            if (xfList.Count > 16)  //first 16 standard
            {
                for (int index = 0; index < xfList.Count; index++)
                {
                    if (Equals((DataXF)xfList[index], dataIn))
                    {
                        //are already in table, return number 
                        return index;
                    }
                }
            }
            //add to table, return number 
            xfList.Add(dataIn);
            int temp = xfList.Count - 1;
            return temp;
        }
        #endregion

        #region GetColorNumber
        /// <summary>
        /// Returns the number of color in color palette.
        /// </summary>
        private int GetColorNumber(Color incomingColor)
        {
            if (colorList.Count > 0)
            {
                for (int index = 0; index < colorList.Count; index++)
                {
                    if ((Color)colorList[index] == incomingColor)
                    {
                        //color is already in table, return color number
                        return index + 8;
                    }
                }
            }
            //add color to table, return color number
            colorList.Add(incomingColor);
            int temp = colorList.Count - 1;
            return temp + 8;
        }
        #endregion

        #region GetFontNumber
        private int GetFontNumber(DataFont dataIn)
        {
            if (fontList.Count > 4)
            {
                for (int index = 4; index < fontList.Count; index++)
                {
                    if (Equals((DataFont)fontList[index], dataIn))
                    {
                        //is already in table, return number 
                        return index + 1;   //peculiarity of Excell  
                    }
                }
            }
            //add to table, return number 
            fontList.Add(dataIn);
            int temp = fontList.Count - 1;
            if (temp >= 4) temp++;
            return temp;
        }
        #endregion

        #region GetSSTNumber
        private int GetSSTNumber(string dataIn)
        {
            sstCounter++;
            if (sstList.Count > 0)
            {
                if (sstHash.ContainsKey(dataIn))
                {
                    //is already in table, return number 
                    return (int)sstHash[dataIn];
                }
                //				for (int index = 0; index < sstList.Count; index++)
                //				{
                //					if (Equals((string)sstList[index], dataIn))
                //					{
                //						//is already in table, return number 
                //						return index;
                //					}
                //				}
            }
            //add to table, return number 
            sstList.Add(dataIn);
            int temp = sstList.Count - 1;
            sstHash.Add(dataIn, temp);
            return temp;
        }
        #endregion

        #region GetFormatNumber
        private int GetFormatNumber(string dataIn)
        {
            if (formatList.Count > 0)
            {
                for (int index = 0; index < formatList.Count; index++)
                {
                    if (Equals((string)formatList[index], dataIn))
                    {
                        return index + 164;
                    }
                }
            }
            formatList.Add(dataIn);
            int temp = formatList.Count - 1;
            return temp + 164;
        }
        #endregion

        #region BIFF Data Write procedures
        private void AddRec(int com, int len)
        {
            bfHeader[0] = (byte)(com & 0xff);
            bfHeader[1] = (byte)((com >> 8) & 0xff);
            bfHeader[2] = (byte)(len & 0xff);
            bfHeader[3] = (byte)((len >> 8) & 0xff);
            memst.Write(bfHeader, 0, 4);
            memst.Write(bfData, 0, len);
        }
        private void dwByte(int position, byte dataValue)   //1
        {
            bfData[position] = dataValue;
        }
        private void dwWord(int position, Int16 dataValue)
        {
            bfData[position + 0] = (byte)(dataValue & 0xff);
            bfData[position + 1] = (byte)((dataValue >> 8) & 0xff);
        }
        private void dwWord(int position, UInt16 dataValue)
        {
            bfData[position + 0] = (byte)(dataValue & 0xff);
            bfData[position + 1] = (byte)((dataValue >> 8) & 0xff);
        }
        private void dwDWord(int position, Int32 dataValue)
        {
            bfData[position + 0] = (byte)(dataValue & 0xff);
            bfData[position + 1] = (byte)((dataValue >> 8) & 0xff);
            bfData[position + 2] = (byte)((dataValue >> 16) & 0xff);
            bfData[position + 3] = (byte)((dataValue >> 24) & 0xff);
        }
        private void dwByte(int position, byte dv1, byte dv2)   //2
        {
            dwByte(position, dv1);
            dwByte(position + 1, dv2);
        }
        private void dwByte(int position, byte dv1, byte dv2, byte dv3) //3
        {
            dwByte(position, dv1, dv2);
            dwByte(position + 2, dv3);
        }
        private void dwByte(int position, byte dv1, byte dv2, byte dv3, byte dv4)   //4
        {
            dwByte(position, dv1, dv2, dv3);
            dwByte(position + 3, dv4);
        }
        private void dwByte(int position, byte dv1, byte dv2, byte dv3, byte dv4, byte dv5) //5
        {
            dwByte(position, dv1, dv2, dv3, dv4);
            dwByte(position + 4, dv5);
        }
        private void dwByte(int position, byte dv1, byte dv2, byte dv3, byte dv4, byte dv5, byte dv6)   //6
        {
            dwByte(position, dv1, dv2, dv3, dv4, dv5);
            dwByte(position + 5, dv6);
        }
        private void dwByte(int position, byte dv1, byte dv2, byte dv3, byte dv4, byte dv5, byte dv6, byte dv7) //7
        {
            dwByte(position, dv1, dv2, dv3, dv4, dv5, dv6);
            dwByte(position + 6, dv7);
        }
        private void dwFill(int position, byte dataValue, int count)
        {
            for (int index = 0; index < count; index++)
            {
                bfData[position + index] = dataValue;
            }
        }
        private bool stringMayBePacked(string data)
        {
            bool mayBePacked = true;
            for (int index = 0; index < data.Length; index++)
            {
                if (data[index] >= 128) mayBePacked = false;
            }
            return mayBePacked;
        }
        private void dwString(int position, string dataValue, int bytesLen, out int offset, bool pack)
        {
            short len = (short)dataValue.Length;
            bool isStringPacked = stringMayBePacked(dataValue);
            if (pack == false) isStringPacked = false;
            if (bytesLen != 0)
            {
                dwWord(position, len);
                position += 2;
                if (bytesLen == 1) position--;
                dwByte(position, (byte)(isStringPacked ? 0x00 : 0x01));     //packed/unpacked
                position++;
            }
            for (int index = 0; index < len; index++)
            {
                if (isStringPacked == true)
                {
                    dwByte(position, (byte)dataValue[index]);
                    position += 1;
                }
                else
                {
                    dwWord(position, (short)dataValue[index]);
                    position += 2;
                }
            }
            offset = position;
        }
        private void dwDouble(int position, double dataValue)
        {
            byte[] buf = BitConverter.GetBytes((double)dataValue);
            buf.CopyTo(bfData, position);
        }
        #endregion

        private static byte percentScaleForFontHeightCorrection = 100;
        /// <summary>
        /// Gets or sets a value indicates ratio of the font for the whole exported document.
        /// </summary>
		public static byte PercentScaleForFontHeightCorrection
        {
            get
            {
                return percentScaleForFontHeightCorrection;
            }
            set
            {
                if (value >= 10)
                {
                    percentScaleForFontHeightCorrection = value;
                }
            }
        }

        //conversion from hundredths of inch to twips
        //		const double HiToTwips = 14.4 * 1.06;
        //		const double HiToTwips = 14.4 * 1.027
        private static double HiToTwips
        {
            get
            {
                //return 14.4 * 1.022 * 100d / (double)PercentScaleForFontHeightCorrection;
                return 14.4 * 1.01 * 100d / (double)PercentScaleForFontHeightCorrection;
            }
        }

        // 1 point = 1/72 inch
        // 20 twips = 1 point 
        // 75 point = 100 pixels
        // 75 pixels = 10 width of column
        // 100 pixels = 3657 colinfo width
        //
        // 1 twips = (1/20 point) * (100/75 pixels) * (3657/100 width) = 100*3657 / 20*75*100 = 
        // = 3657 / 1500 = 2.438 colinfo width
        //
        //conversion from twips to units of column
        //		const double TwipsToColinfo = 2.438 * 1.01;
        //const double TwipsToColinfo = 2.438 * 0.921;
        //const double TwipsToColinfo = 2.438 * 0.914;
        //const double TwipsToColinfo = 2.438 * 0.914 * (1.027 / 1.024);
        //const double TwipsToColinfo = 2.438 * 0.914 * (1.027 / 1.022);
        const double TwipsToColinfo = 2.438 * 0.935;

        private int Convert(double x)
        {
            return (int)(x * HiToTwips);
        }

        private bool CompareExcellSheetNames(string name1, string name2)
        {
            string st1 = name1;
            if ((st1 == null) || (st1.Length == 0)) st1 = string.Empty;
            string st2 = name2;
            if ((st2 == null) || (st2.Length == 0)) st2 = string.Empty;
            return (st1 == st2);
        }

        #region prepare data
        private void PrepareData()
        {
            xfList = new ArrayList();
            colorList = new ArrayList();
            fontList = new ArrayList();
            sstList = new ArrayList();
            sstHash = new Hashtable();
            formatList = new ArrayList();
            imageList = new ArrayList();
            sstCounter = 0;
            pagesList = new ArrayList();
            bookmarksList = new ArrayList();
            hlinksList = new ArrayList();
            boundsheetsOffsetsList = new ArrayList();
            boundsheetsNamesList = new ArrayList();

            imageCodec = StiImageCodecInfo.GetImageCodec("image/jpeg");

            escher = new Escher();

            DataXF tempXF;
            DataFont tempFont;

            #region add default style XF
            tempXF = new DataXF(0, 0, 1, 0x0FFF, 0, 2, 0, 0, 0, 0x00, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0x40, 0x41);   //0
            GetXFNumber(tempXF);
            tempXF = new DataXF(1, 0, 1, 0x0FFF, 0, 2, 0, 0, 0, 0x3D, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0x40, 0x41);   //1
            GetXFNumber(tempXF);
            tempXF = new DataXF(1, 0, 1, 0x0FFF, 0, 2, 0, 0, 0, 0x3D, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0x40, 0x41);   //2
            GetXFNumber(tempXF);
            tempXF = new DataXF(2, 0, 1, 0x0FFF, 0, 2, 0, 0, 0, 0x3D, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0x40, 0x41);   //3
            GetXFNumber(tempXF);
            tempXF = new DataXF(2, 0, 1, 0x0FFF, 0, 2, 0, 0, 0, 0x3D, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0x40, 0x41);   //4
            GetXFNumber(tempXF);
            tempXF = new DataXF(0, 0, 1, 0x0FFF, 0, 2, 0, 0, 0, 0x3D, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0x40, 0x41);   //5
            GetXFNumber(tempXF);
            tempXF = new DataXF(0, 0, 1, 0x0FFF, 0, 2, 0, 0, 0, 0x3D, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0x40, 0x41);   //6
            GetXFNumber(tempXF);
            tempXF = new DataXF(0, 0, 1, 0x0FFF, 0, 2, 0, 0, 0, 0x3D, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0x40, 0x41);   //7
            GetXFNumber(tempXF);
            tempXF = new DataXF(0, 0, 1, 0x0FFF, 0, 2, 0, 0, 0, 0x3D, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0x40, 0x41);   //8
            GetXFNumber(tempXF);
            tempXF = new DataXF(0, 0, 1, 0x0FFF, 0, 2, 0, 0, 0, 0x3D, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0x40, 0x41);   //9
            GetXFNumber(tempXF);
            tempXF = new DataXF(0, 0, 1, 0x0FFF, 0, 2, 0, 0, 0, 0x3D, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0x40, 0x41);   //10
            GetXFNumber(tempXF);
            tempXF = new DataXF(0, 0, 1, 0x0FFF, 0, 2, 0, 0, 0, 0x3D, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0x40, 0x41);   //11
            GetXFNumber(tempXF);
            tempXF = new DataXF(0, 0, 1, 0x0FFF, 0, 2, 0, 0, 0, 0x3D, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0x40, 0x41);   //12
            GetXFNumber(tempXF);
            tempXF = new DataXF(0, 0, 1, 0x0FFF, 0, 2, 0, 0, 0, 0x3D, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0x40, 0x41);   //13
            GetXFNumber(tempXF);
            tempXF = new DataXF(0, 0, 1, 0x0FFF, 0, 2, 0, 0, 0, 0x3D, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0x40, 0x41);   //14
            GetXFNumber(tempXF);
            tempXF = new DataXF(0, 0, 0, 0x0000, 0, 2, 0, 0, 0, 0x00, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0x40, 0x41);   //15
            GetXFNumber(tempXF);
            tempXF = new DataXF(1, 44, 1, 0x0FFF, 0, 2, 0, 0, 0, 0x3E, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0x40, 0x41);  //16
            GetXFNumber(tempXF);
            tempXF = new DataXF(1, 42, 1, 0x0FFF, 0, 2, 0, 0, 0, 0x3E, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0x40, 0x41);  //17
            GetXFNumber(tempXF);
            tempXF = new DataXF(1, 9, 1, 0x0FFF, 0, 2, 0, 0, 0, 0x3E, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0x40, 0x41);   //18
            GetXFNumber(tempXF);
            tempXF = new DataXF(1, 43, 1, 0x0FFF, 0, 2, 0, 0, 0, 0x3E, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0x40, 0x41);  //19
            GetXFNumber(tempXF);
            tempXF = new DataXF(1, 41, 1, 0x0FFF, 0, 2, 0, 0, 0, 0x3E, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0x40, 0x41);  //20
            GetXFNumber(tempXF);
            #endregion

            #region add default fonts
            tempFont = new DataFont("Arial", false, false, false, false, 200, 0x7FFF, 0x01);
            GetFontNumber(tempFont);
            GetFontNumber(tempFont);
            GetFontNumber(tempFont);
            GetFontNumber(tempFont);
            #endregion

        }
        #endregion

        #region make main stream
        private Size MakeWorksheetStream(StiPagesCollection pages, int sheetIndex)
        {
            DataXF tempXF;
            DataFont tempFont;
            ArrayList mergedCells = new ArrayList();
            ArrayList hlinks = new ArrayList();
            ArrayList hlinkLocal = new ArrayList();
            CellRangeAddress tempCellRange;

            int startImageIndex = imageList.Count;
            StiPage page = pages[0];

            if (matrix == null)
            {
                StiDataExportMode dataMode = dataExportMode;
#pragma warning disable 612, 618
                if (StiOptions.Export.Excel.AllowExportFootersInDataOnlyMode) dataMode |= StiDataExportMode.Footers;
#pragma warning restore 612, 618

                matrix = new StiMatrix(pages, StiOptions.Export.Excel.DivideBigCells, this, null, dataMode);
                if (IsStopped) return new Size(0, 0);

                if (useOnePageHeaderAndFooter)
                {
                    matrix.ScanComponentsPlacement(true, exportObjectFormatting);

                    #region prepare line info

                    //array must hold only first page header
                    int tempOffset = 0;
                    //find first header
                    while ((matrix.LinePlacement[tempOffset] != StiMatrix.StiTableLineInfo.PageHeader) &&
                        (tempOffset < matrix.CoordY.Count - 1))
                        tempOffset++;
                    //header exist
                    if (matrix.LinePlacement[tempOffset] == StiMatrix.StiTableLineInfo.PageHeader)
                    {
                        //find first line after header
                        while ((matrix.LinePlacement[tempOffset] == StiMatrix.StiTableLineInfo.PageHeader) &&
                            (tempOffset < matrix.CoordY.Count - 1))
                            tempOffset++;
                        //scan remains lines 
                        while (tempOffset < matrix.CoordY.Count - 1)
                        {
                            //it's header
                            if (matrix.LinePlacement[tempOffset] == StiMatrix.StiTableLineInfo.PageHeader)
                            {
                                //cut this line
                                matrix.LinePlacement[tempOffset] = StiMatrix.StiTableLineInfo.Trash;    //for cut
                            }
                            tempOffset++;
                        }
                    }

                    //array must hold only last page footer
                    tempOffset = matrix.CoordY.Count - 1;
                    //find last footer
                    while ((matrix.LinePlacement[tempOffset] != StiMatrix.StiTableLineInfo.PageFooter) &&
                        (tempOffset > 0))
                        tempOffset--;
                    //header exist
                    if (matrix.LinePlacement[tempOffset] == StiMatrix.StiTableLineInfo.PageFooter)
                    {
                        //find first line before header
                        while ((matrix.LinePlacement[tempOffset] == StiMatrix.StiTableLineInfo.PageFooter) &&
                            (tempOffset > 0))
                            tempOffset--;
                        //scan remains lines 
                        while (tempOffset > 0)
                        {
                            //it's header
                            if (matrix.LinePlacement[tempOffset] == StiMatrix.StiTableLineInfo.PageFooter)
                            {
                                //cut this line
                                matrix.LinePlacement[tempOffset] = StiMatrix.StiTableLineInfo.Trash;    //for cut
                            }
                            tempOffset--;
                        }
                    }

                    #endregion

                    matrix.AllowModification(true);

                    #region remake matrix
                    int linesCount = 0;
                    for (int rowIndex = 0; rowIndex < Matrix.CoordY.Count - 1; rowIndex++)
                    {
                        if (Matrix.LinePlacement[rowIndex] != StiMatrix.StiTableLineInfo.Trash)
                        {
                            //move line
                            for (int columnIndex = 0; columnIndex < Matrix.CoordX.Count - 1; columnIndex++)
                            {
                                //move cell
                                matrix.Cells[linesCount, columnIndex] = matrix.Cells[rowIndex, columnIndex];
                                StiCell cell = matrix.Cells[linesCount, columnIndex];
                                if (cell != null)
                                {
                                    //correct coordinate
                                    cell.Top = linesCount;

                                    if (cell.ExportImage != null)
                                    {
                                        StiComponent component = cell.Component;
                                        object objRect = matrix.imagesBaseRect[component];
                                        if (objRect != null)
                                        {
                                            RectangleD rect = (RectangleD)objRect;
                                            double offset = (double)matrix.CoordY.GetByIndex(rowIndex) - (double)matrix.CoordY.GetByIndex(linesCount);
                                            rect.Y -= offset;
                                            matrix.imagesBaseRect[component] = rect;   //new RectangleD(rectD.X, rectD.Y + TotalHeight, rectD.Width, rectD.Height);
                                        }
                                    }
                                }
                                //move border
                                matrix.BordersX[linesCount, columnIndex] = matrix.BordersX[rowIndex, columnIndex];
                                matrix.BordersY[linesCount, columnIndex] = matrix.BordersY[rowIndex, columnIndex];
                                //move bookmarks
                                matrix.Bookmarks[linesCount, columnIndex] = matrix.Bookmarks[rowIndex, columnIndex];
                            }
                            //move border - right line
                            matrix.BordersY[linesCount, Matrix.CoordX.Count - 1] = matrix.BordersY[rowIndex, Matrix.CoordX.Count - 1];

                            //count line height
                            double lineHeight = (double)matrix.CoordY.GetByIndex(rowIndex + 1) - (double)matrix.CoordY.GetByIndex(rowIndex);
                            matrix.CoordY.SetByIndex(linesCount + 1, (double)matrix.CoordY.GetByIndex(linesCount) + lineHeight);
                            linesCount++;
                        }
                    }
                    for (int columnIndex = 0; columnIndex < Matrix.CoordX.Count - 1; columnIndex++)
                    {
                        //move border - bottom line
                        matrix.BordersX[linesCount, columnIndex] = matrix.BordersX[Matrix.CoordY.Count - 1, columnIndex];
                    }

                    int numAbove = Matrix.CoordY.Count - 1 - linesCount;
                    if (numAbove > 0)
                    {
                        //remove lines at end of array
                        for (int tempIndex = 0; tempIndex < numAbove; tempIndex++)
                        {
                            matrix.CoordY.RemoveAt(linesCount + 1);
                        }
                    }
                    #endregion

                    matrix.AllowModification(false);
                }

                if (dataExportMode != StiDataExportMode.AllBands)
                {
                    matrix.ScanComponentsPlacement(true, exportObjectFormatting);

                    matrix.AllowModification(true);

                    #region remake matrix
                    int linesCount = 0;
                    Hashtable headerNames = new Hashtable();
                    string lastParentBandName = null;
                    for (int rowIndex = 0; rowIndex < Matrix.CoordY.Count - 1; rowIndex++)
                    {
                        bool isHeader = false;
                        if ((Matrix.LinePlacement[rowIndex] == StiMatrix.StiTableLineInfo.HeaderD) || (Matrix.LinePlacement[rowIndex] == StiMatrix.StiTableLineInfo.HeaderAP))
                        {
                            string tempSt = Matrix.ParentBandName[rowIndex];
                            //check for new header component 
                            int symPos = tempSt.IndexOf('\x1f');
                            if (symPos != -1)
                            {
                                string parentBandName = tempSt.Substring(0, symPos);
                                if (parentBandName != lastParentBandName)
                                {
                                    lastParentBandName = parentBandName;
                                    headerNames.Clear();
                                }
                            }
                            //check for repeated lines
                            if (!headerNames.ContainsKey(tempSt))
                            {
                                isHeader = true;
                                headerNames.Add(tempSt, tempSt);
                            }
                        }
                        if ((Matrix.LinePlacement[rowIndex] == StiMatrix.StiTableLineInfo.Data) ||
                            (Matrix.LinePlacement[rowIndex] == StiMatrix.StiTableLineInfo.FooterD) ||
                            isHeader)
                        {
                            //move line
                            for (int columnIndex = 0; columnIndex < Matrix.CoordX.Count - 1; columnIndex++)
                            {
                                //move cell
                                matrix.Cells[linesCount, columnIndex] = matrix.Cells[rowIndex, columnIndex];
                                StiCell cell = matrix.Cells[linesCount, columnIndex];
                                if (cell != null)
                                {
                                    //correct coordinate
                                    cell.Top = linesCount;

                                    if (cell.ExportImage != null)
                                    {
                                        StiComponent component = cell.Component;
                                        object objRect = matrix.imagesBaseRect[component];
                                        if (objRect != null)
                                        {
                                            RectangleD rect = (RectangleD)objRect;
                                            double offset = (double)matrix.CoordY.GetByIndex(rowIndex) - (double)matrix.CoordY.GetByIndex(linesCount);
                                            rect.Y -= offset;
                                            matrix.imagesBaseRect[component] = rect;   //new RectangleD(rectD.X, rectD.Y + TotalHeight, rectD.Width, rectD.Height);
                                        }
                                    }
                                }
                                //move border
                                if ((linesCount == 0) || (matrix.BordersX[rowIndex, columnIndex] != null))
                                {
                                    matrix.BordersX[linesCount, columnIndex] = matrix.BordersX[rowIndex, columnIndex];
                                }
                                matrix.BordersX[linesCount + 1, columnIndex] = matrix.BordersX[rowIndex + 1, columnIndex];
                                matrix.BordersY[linesCount, columnIndex] = matrix.BordersY[rowIndex, columnIndex];
                                //move bookmarks
                                matrix.Bookmarks[linesCount, columnIndex] = matrix.Bookmarks[rowIndex, columnIndex];
                            }
                            //move border - right line
                            matrix.BordersY[linesCount, Matrix.CoordX.Count - 1] = matrix.BordersY[rowIndex, Matrix.CoordX.Count - 1];

                            //count line height
                            double lineHeight = (double)matrix.CoordY.GetByIndex(rowIndex + 1) - (double)matrix.CoordY.GetByIndex(rowIndex);
                            matrix.CoordY.SetByIndex(linesCount + 1, (double)matrix.CoordY.GetByIndex(linesCount) + lineHeight);
                            linesCount++;
                        }
                    }
                    //				for (int columnIndex = 0; columnIndex < Matrix.CoordX.Count - 1; columnIndex++)
                    //				{
                    //					//move border - bottom line
                    //					matrix.BordersX[linesCount, columnIndex] = matrix.BordersX[Matrix.CoordY.Count - 1, columnIndex];
                    //				}

                    int numAbove = Matrix.CoordY.Count - 1 - linesCount;
                    if (numAbove > 0)
                    {
                        //remove lines at end of array
                        for (int tempIndex = 0; tempIndex < numAbove; tempIndex++)
                        {
                            matrix.CoordY.RemoveAt(linesCount + 1);
                        }
                    }
                    #endregion

                    matrix.AllowModification(false);
                }

                minRowIndex = 1;
            }

            maxRowIndex = Matrix.CoordY.Count;
            if (maxRowIndex - minRowIndex > maximumSheetHeight) //max for BIFF format is 65535
            {
                maxRowIndex = minRowIndex + maximumSheetHeight;
            }

            #region make worksheet
            dwFill(0, 0x00, 16);
            dwWord(0, 0x0600);  //BIFF8
            dwWord(2, 0x0010);  //worksheet
            dwWord(4, 0x0F46);  //build
            dwWord(6, 0x07CC);  //1996
            dwDWord(8, 0x000080C9); //file history flags
            dwDWord(12, 0x00000006);    //Lowest Excel version that can read all records in this file - Excel97
            AddRec(0x0809, 16); //BOF

            #region calculation settings block
            dwWord(0, 0x0001);  //automatically
            AddRec(0x000D, 2);  //Calcmode 

            dwWord(0, 100);     //number of times
            AddRec(0x000C, 2);  //Calccount

            dwWord(0, 0x0001);  //A1
            AddRec(0x000F, 2);  //Refmode

            dwWord(0, 0x0000);  //off
            AddRec(0x0011, 2);  //Iteration

            dwDouble(0, 0.001);
            AddRec(0x0010, 8);  //Delta

            dwWord(0, 0x0000);  //off
            AddRec(0x005F, 2);  //Saverecalc
            #endregion

            dwWord(0, 0x0000);  //no
            AddRec(0x002A, 2);  //Print row headers

            dwWord(0, 0x0000);  //no
            AddRec(0x002B, 2);  //Print gridlines

            dwWord(0, 0x0001);  //Print grid lines option changed
            AddRec(0x0082, 2);  //Gridset	6.48

            dwWord(0, 0x0000);  //
            dwWord(2, 0x0000);  //
            dwWord(4, 0x0000);  //not used
            dwWord(6, 0x0000);  //not used
            AddRec(0x0080, 8);  //Guts		6.49

            dwWord(0, 0x0000);  //flags
            dwWord(2, 0x00FF);  //value	= 12.75 twips
            AddRec(0x0225, 4);  //DEFAULT ROW HEIGHT	6.28

            dwWord(0, 0x04C1);  //flags
            AddRec(0x0081, 2);  //WSBOOL	6.113

            #region page settings block

            #region horizontalPageBreaks
            int horizontalPageBreaksCount = Matrix.HorizontalPageBreaks.Count;
            if ((exportHorizontalPageBreaks) && (horizontalPageBreaksCount > 0))
            {
                if (horizontalPageBreaksCount > 320) horizontalPageBreaksCount = 320;
                short firstColumn = 0;
                short lastColumn = (short)(Matrix.CoordX.Count - 2);

                dwWord(0, (short)horizontalPageBreaksCount);
                for (short indexBreak = 0; indexBreak < horizontalPageBreaksCount; indexBreak++)
                {
                    dwWord(2 + indexBreak * 6 + 0, (short)((int)Matrix.HorizontalPageBreaks[indexBreak]));
                    dwWord(2 + indexBreak * 6 + 2, firstColumn);
                    dwWord(2 + indexBreak * 6 + 4, lastColumn);
                }
                AddRec(0x001B, horizontalPageBreaksCount * 6 + 2);  //HORIZONTALPAGEBREAKS 6.54
            }
            #endregion

            AddRec(0x0014, 0);  //HEADER text
            AddRec(0x0015, 0);  //FOOTER text

            dwWord(0, 0x0000);  //value
            AddRec(0x0083, 2);  //HCENTER 0

            dwWord(0, 0x0000);  //value
            AddRec(0x0084, 2);  //VCENTER 0

            dwDouble(0, page.Unit.ConvertToHInches(page.Margins.Top) / 100);
            AddRec(0x0028, 8);  //Top margin		6.103

            dwDouble(0, page.Unit.ConvertToHInches(page.Margins.Bottom) / 100);
            AddRec(0x0029, 8);  //Bottom margin		6.11

            dwDouble(0, page.Unit.ConvertToHInches(page.Margins.Left) / 100);
            AddRec(0x0026, 8);  //Left margin		6.62

            dwDouble(0, page.Unit.ConvertToHInches(page.Margins.Right) / 100);
            AddRec(0x0027, 8);  //Right margin		6.81

            //old version
            //			dwWord(0, 0x0000);	//Paper size
            //			dwWord(2, 0x00FF);	//Scaling factor in percent
            //			dwWord(4, 0x0001);	//Start page number
            //			dwWord(6, 0x0001);	//Fit worksheet width to this number of pages (0 = use as many as needed)
            //			dwWord(8, 0x0001);	//Fit worksheet height to this number of pages (0 = use as many as needed)
            //			dwWord(10, 0x0004);	//Option flags
            //			dwWord(12, 0x0000);	//Print resolution in dpi
            //			dwWord(14, 0x0000);	//Vertical print resolution in dpi
            //			dwDouble(16, 0x0000);	//Header margin (IEEE 754 floating-point value, 64-bit double precision)
            //			dwDouble(24, 0x0000);	//Footer margin (IEEE 754 floating-point value, 64-bit double precision)
            //			dwWord(32, 0x0000);	//Number of copies to print

            //new version
            dwWord(0, (short)page.PaperSize);   //Paper size (0 - undefined)
            dwWord(2, PercentScaleForFontHeightCorrection); //Scaling factor in percent (default 100%)
            dwWord(4, 0x0001);  //Start page number
            dwWord(6, 0x0001);  //Fit worksheet width to this number of pages (0 = use as many as needed)
            dwWord(8, 0x0001);  //Fit worksheet height to this number of pages (0 = use as many as needed)
            int option = 0x0000 | (page.Orientation == StiPageOrientation.Portrait ? 0x0002 : 0x0000);
            dwWord(10, (short)option);  //Option flags
            dwWord(12, 0x0000); //Print resolution in dpi
            dwWord(14, 0x0000); //Vertical print resolution in dpi
            dwDouble(16, 0x0000);   //Header margin (IEEE 754 floating-point value, 64-bit double precision)
            dwDouble(24, 0x0000);   //Footer margin (IEEE 754 floating-point value, 64-bit double precision)
            dwWord(32, 0x0001); //Number of copies to print
            AddRec(0x00A1, 34); //SETUP		6.89
            #endregion

            dwWord(0, 0x0008);  //value
            AddRec(0x0055, 2);  //DEFCOLWIDTH

            #region COLINFO
            if (exportObjectFormatting)
            {
                double value0 = (double)Matrix.CoordX.GetByIndex(0);
                int sum2 = 0;
                for (int columnIndex = 1; columnIndex < Matrix.CoordX.Count; columnIndex++)
                {
                    double value2 = (double)Matrix.CoordX.GetByIndex(columnIndex);
                    double offsetX = (value2 - value0) * HiToTwips * TwipsToColinfo;
                    Int16 colWidth2 = (Int16)(Math.Round(offsetX) - sum2);
                    sum2 += colWidth2;

                    dwWord(0, (Int16)(columnIndex - 1));    //index to first column in the range
                    dwWord(2, (Int16)(columnIndex - 1));    //index to last column in the range
                    dwWord(4, colWidth2);
                    dwWord(6, 0x000F);      //index to XF record
                    dwWord(8, 0x0000);      //flags
                    dwWord(10, 0x0000);     //not used
                    AddRec(0x007D, 12);     //COLINFO
                }
            }
            #endregion

            dwFill(0, 0x00, 14);
            dwDWord(0, 0x0000);                         //first row
            dwDWord(4, maxRowIndex - minRowIndex);                  //last row
            dwWord(8, 0x0000);                          //first column
            dwWord(10, (Int16)(Matrix.CoordX.Count));   //last column
            AddRec(0x0200, 14);                         //Dimensions	6.31

            #region ROWS
            RichTextBox richtextForConvert = null;
            bool[,] readyCells = new bool[maxRowIndex, Matrix.CoordX.Count];
            int[,] mergedCellsStyle = new int[maxRowIndex + 1, Matrix.CoordX.Count];

            CurrentPassNumber = 2;
            double progressScale = Math.Max(Matrix.CoordY.Count / 200f, 1f);
            int progressValue = 0;

            for (int rowIndex = minRowIndex; rowIndex < maxRowIndex; rowIndex++)
            {
                int currentProgress = (int)(rowIndex / progressScale);
                if (currentProgress > progressValue)
                {
                    progressValue = currentProgress;
                    InvokeExporting(rowIndex, Matrix.CoordY.Count, CurrentPassNumber, MaximumPassNumber);
                }

                #region write ROW block
                if ((rowIndex - minRowIndex) % 32 == 0) //begin block of 32 row
                {
                    for (int tempIndex1 = 0; tempIndex1 < 32; tempIndex1++)
                    {
                        int tempRowIndex = rowIndex - 1 + tempIndex1;
                        if (tempRowIndex < maxRowIndex - 1)
                        {
                            double height = (double)Matrix.CoordY.GetByIndex(tempRowIndex + 1)
                                - (double)Matrix.CoordY.GetByIndex(tempRowIndex);
                            dwFill(0, 0x00, 16);
                            dwWord(0, (Int16)(tempRowIndex - (minRowIndex - 1)));   //index of row
                            dwWord(2, 0x0000);                          //first column
                            dwWord(4, (Int16)(Matrix.CoordX.Count));    //last column
                            if (exportObjectFormatting)
                            {
                                dwWord(6, (Int16)Convert(height));      //height
                                dwDWord(12, 0x000F0140);                    //flags
                            }
                            else
                            {
                                dwDWord(6, 0x00FF);                     //default height
                                dwDWord(12, 0x000F0100);                    //flags
                            }
                            AddRec(0x0208, 16); //Row	6.83
                        }
                    }
                }
                #endregion

                for (int columnIndex = 1; columnIndex < Matrix.CoordX.Count; columnIndex++)
                {
                    StiCell cell = Matrix.Cells[rowIndex - 1, columnIndex - 1];

                    #region cells exists
                    if ((!readyCells[rowIndex - 1, columnIndex - 1]) && (cell != null))
                    {
                        readyCells[rowIndex - 1, columnIndex - 1] = true;
                        StiRichText rtf = cell.Component as StiRichText;
                        StiText textComp = cell.Component as StiText;

                        string str = cell.Text;
                        if ((rtf != null) && (rtf.RtfText != string.Empty))
                        {
                            if (richtextForConvert == null) richtextForConvert = new Controls.StiRichTextBox(false);
                            rtf.GetPreparedText(richtextForConvert);
                            str = richtextForConvert.Text;
                        }

                        StiCheckBox checkComp = cell.Component as StiCheckBox;
                        bool hasCheckBoxExcelDataValue = false;
                        if ((checkComp != null) && (checkComp.ExcelDataValue != null) && (checkComp.ExcelDataValue.Length > 0))
                        {
                            hasCheckBoxExcelDataValue = true;
                            str = checkComp.ExcelDataValue;
                        }

                        #region Hyperlink
                        if (cell.Component.HyperlinkValue != null)
                        {
                            string hyperlink = cell.Component.HyperlinkValue.ToString().Trim();
                            if (hyperlink.Length > 0 && !hyperlink.StartsWith("javascript:"))
                            {
                                string description = str;
                                if (description == null || description.Length == 0) description = hyperlink;

                                if (hyperlink.StartsWith("#", StringComparison.InvariantCulture))
                                {
                                    hyperlink = hyperlink.Substring(1);
                                    CellRangeAddress range = new CellRangeAddress(
                                        rowIndex - minRowIndex,
                                        rowIndex - minRowIndex + cell.Height,
                                        columnIndex - 1,
                                        columnIndex - 1 + cell.Width);
                                    HlinkData hl = new HlinkData(range, description, hyperlink);
                                    hlinkLocal.Add(hl);
                                }
                                else
                                {
                                    #region Store URL
                                    dwWord(0, (short)(rowIndex - minRowIndex));
                                    dwWord(2, (short)(rowIndex - minRowIndex + cell.Height));
                                    dwWord(4, (short)(columnIndex - 1));
                                    dwWord(6, (short)(columnIndex - 1 + cell.Width));
                                    dwByte(8, 0xD0, 0xC9, 0xEA, 0x79);	//guid
                                    dwByte(12, 0xF9, 0xBA, 0xCE, 0x11);
                                    dwByte(16, 0x8C, 0x82, 0x00, 0xAA);
                                    dwByte(20, 0x00, 0x4B, 0xA9, 0x0B);
                                    dwDWord(24, 0x00000002);	//unknown
                                    dwDWord(28, 0x00000017);	//option flag

                                    //store description
                                    int hpos = 32;
                                    dwDWord(hpos, description.Length + 1);
                                    hpos += 4;
                                    for (int indexChar = 0; indexChar < description.Length; indexChar++)
                                    {
                                        dwWord(hpos, (ushort)description[indexChar]);
                                        hpos += 2;
                                    }
                                    dwWord(hpos, 0);
                                    hpos += 2;

                                    //store url
                                    dwByte(hpos, 0xE0, 0xC9, 0xEA, 0x79);	//guid
                                    dwByte(hpos + 4, 0xF9, 0xBA, 0xCE, 0x11);
                                    dwByte(hpos + 8, 0x8C, 0x82, 0x00, 0xAA);
                                    dwByte(hpos + 12, 0x00, 0x4B, 0xA9, 0x0B);
                                    hpos += 16;
                                    dwDWord(hpos, (hyperlink.Length + 1) * 2);
                                    hpos += 4;
                                    for (int indexChar = 0; indexChar < hyperlink.Length; indexChar++)
                                    {
                                        dwWord(hpos, (ushort)hyperlink[indexChar]);
                                        hpos += 2;
                                    }
                                    dwWord(hpos, 0);
                                    hpos += 2;

                                    byte[] buf = new byte[hpos];
                                    Array.Copy(bfData, 0, buf, 0, hpos);
                                    hlinks.Add(buf);
                                    #endregion
                                }
                            }
                        }
                        #endregion

                        #region Image
                        IStiExportImage exportImage = cell.Component as IStiExportImage;
                        if ((exportImage != null) && (imageList.Count < 1024))
                        {
                            IStiExportImageExtended exportImageExtended = exportImage as IStiExportImageExtended;

                            float zoom = imageResolution;

                            var imageComp = exportImage as StiImage;
                            if (StiOptions.Export.Excel.UseImageResolution && imageComp != null && imageComp.ExistImageToDraw())
                            {
                                using (var gdiImage = imageComp.TakeGdiImageToDraw())
                                {
                                    if (gdiImage != null)
                                    {
                                        var dpix = gdiImage.HorizontalResolution;
                                        if (dpix >= 50 && dpix <= 600) zoom *= dpix / 100f;
                                    }
                                }
                            }

                            Image image = null;
                            if (cell.Component.IsExportAsImage(StiExportFormat.Excel))
                            {
                                if (exportImageExtended != null && exportImageExtended.IsExportAsImage(StiExportFormat.Excel))
                                    image = exportImageExtended.GetImage(ref zoom, StiExportFormat.Excel);
                                else image = exportImage.GetImage(ref zoom);
                            }

                            if (image != null)
                            {
                                Image img = Matrix.GetRealImageData(cell, image);
                                if (img != null) image = img;

                                MemoryStream memw = new MemoryStream();

                                #region save jpeg without parameters
                                if (imageCodec == null) image.Save(memw, ImageFormat.Jpeg);
                                #endregion

                                #region Save jpeg with quality parameter
                                else
                                {
                                    EncoderParameters imageEncoderParameters = new EncoderParameters(1);
                                    imageEncoderParameters.Param[0] = new EncoderParameter(ImageEncoder.Quality, (long)(imageQuality * 100));  // <------------;
                                    image.Save(memw, imageCodec, imageEncoderParameters);
                                }
                                #endregion

                                byte[] bytes = memw.ToArray();

                                Escher.BiffImageData imageData = new Escher.BiffImageData(
                                    (ushort)(rowIndex - minRowIndex),
                                    0,
                                    (ushort)(columnIndex - 1),
                                    0,
                                    (ushort)(rowIndex - (minRowIndex - 1) + cell.Height),
                                    0,
                                    (ushort)(columnIndex + cell.Width),
                                    0,
                                    bytes);
                                imageList.Add(imageData);

                                image.Dispose();
                            }
                        }
                        #endregion

                        #region check for exceltext tag
                        bool isExcelText = false;
                        if (cell.Component != null && cell.Component.TagValue != null)
                        {
                            string cellTag = cell.Component.TagValue.ToString().ToLower();
                            if (cellTag.IndexOf("exceltext", StringComparison.InvariantCulture) != -1)
                            {
                                isExcelText = true;
                            }
                        }
                        #endregion

                        #region Scan value format
                        string inputFormat = string.Empty;
                        if (textComp != null) inputFormat = textComp.Format;

                        bool isFormatCurrency = false;
                        bool isFormatNumeric = false;
                        bool isFormatPercent = false;
                        bool isFormatDate = false;
                        bool isFormatTime = false;
                        bool isFormatCustom = false;
                        bool isDefaultFormat = false;
                        string outputFormat = string.Empty;
                        int decimalDigits = 2;
                        int groupDigits = 0;
                        string currencySymbol = "$";
                        bool currencyPositionBefore = false;
                        bool negativeBraces = false;
                        bool hideZeros = (textComp != null) && textComp.HideZeros;

                        string positivePatternString = null;
                        string negativePatternString = null;
                        int posPatternDelimiter = (inputFormat != null ? inputFormat.IndexOf("|") : -1);
                        if (posPatternDelimiter != -1)
                        {
                            positivePatternString = StiExportUtils.GetPositivePattern((int)inputFormat[posPatternDelimiter + 1] - (int)'A');
                            negativePatternString = StiExportUtils.GetNegativePattern((int)inputFormat[posPatternDelimiter + 2] - (int)'A');
                            inputFormat = inputFormat.Substring(0, posPatternDelimiter);
                        }

                        #region get value format
                        if ((inputFormat != null) && (inputFormat.Length > 0))
                        {
                            if (inputFormat[0] == 'C')
                            {
                                isFormatCurrency = true;
                            }
                            else if (inputFormat[0] == 'N')
                            {
                                isFormatNumeric = true;
                            }
                            else if (inputFormat[0] == 'P')
                            {
                                isFormatPercent = true;
                            }
                            else if (inputFormat[0] == 'D')
                            {
                                isFormatDate = true;
                            }
                            else if (inputFormat[0] == 'T')
                            {
                                isFormatTime = true;
                            }
                            else if (inputFormat[0] == 'U')
                            {
                                isFormatCustom = true;
                            }
                            if (inputFormat.Length == 1)
                            {
                                isDefaultFormat = true;
                            }
                            else
                            {
                                if (isFormatCurrency || isFormatNumeric || isFormatPercent)
                                {
                                    #region scan parameters
                                    int indexPos = 1;
                                    if (char.IsDigit(inputFormat[indexPos]))
                                    {
                                        StringBuilder decimalSB = new StringBuilder();
                                        while ((indexPos < inputFormat.Length) && (char.IsDigit(inputFormat[indexPos])))
                                        {
                                            decimalSB.Append(inputFormat[indexPos]);
                                            indexPos++;
                                        }
                                        decimalDigits = int.Parse(decimalSB.ToString());
                                    }
                                    if ((indexPos < inputFormat.Length) && (inputFormat[indexPos] == 'G'))
                                    {
                                        indexPos++;
                                        groupDigits = 3;
                                    }
                                    if ((indexPos < inputFormat.Length) && (inputFormat[indexPos] == '('))
                                    {
                                        indexPos++;
                                        negativeBraces = true;
                                    }
                                    if ((indexPos < inputFormat.Length) && (inputFormat[indexPos] == '.' || inputFormat[indexPos] == ','))
                                    {
                                        //excel don't allow to use custom decimal separator from file, only from options menu
                                        indexPos++;
                                    }
                                    if ((indexPos < inputFormat.Length) &&
                                        ((inputFormat[indexPos] == '+') || (inputFormat[indexPos] == '-')))
                                    {
                                        if (inputFormat[indexPos] == '+') currencyPositionBefore = true;
                                        indexPos++;
                                        if (indexPos < inputFormat.Length)
                                        {
                                            currencySymbol = inputFormat.Substring(indexPos);
                                        }
                                    }
                                    #endregion
                                }
                            }
                        }
                        #endregion

                        if (!(isFormatCurrency || isFormatNumeric || isFormatPercent))
                        {
                            isDefaultFormat = true;
                        }

                        if (hideZeros && (inputFormat != null) && (inputFormat.Length > 0))
                            isDefaultFormat = false;

                        if (isExcelText) isDefaultFormat = true;

                        #region make format string
                        if (!isDefaultFormat)
                        {
                            if (posPatternDelimiter != -1)
                            {
                                StringBuilder outputSB = new StringBuilder();
                                if (groupDigits > 1)
                                {
                                    outputSB.Append("#,");
                                    outputSB.Append('#', groupDigits - 1);
                                }
                                outputSB.Append('0');
                                if (decimalDigits > 0)
                                {
                                    outputSB.Append(".");
                                    outputSB.Append('0', decimalDigits);
                                }
                                string nn = outputSB.ToString();

                                string positivePattern = positivePatternString.Replace("n", nn).Replace("$", $"\"{currencySymbol}\"");
                                string negativePattern = negativePatternString.Replace("n", nn).Replace("$", $"\"{currencySymbol}\"");

                                outputFormat = positivePattern + ";" + negativePattern + (hideZeros ? ";" : "");
                            }
                            else
                            {
                                StringBuilder outputSB = new StringBuilder();
                                if ((isFormatCurrency) && (currencyPositionBefore == true))
                                {
                                    outputSB.Append("\"");
                                    outputSB.Append(currencySymbol);
                                    outputSB.Append("\"");
                                }
                                if (groupDigits > 1)
                                {
                                    outputSB.Append("#,");
                                    outputSB.Append('#', groupDigits - 1);
                                }
                                outputSB.Append('0');
                                if (decimalDigits > 0)
                                {
                                    outputSB.Append(".");
                                    outputSB.Append('0', decimalDigits);
                                }
                                if ((isFormatCurrency) && (currencyPositionBefore == false))
                                {
                                    outputSB.Append("\"");
                                    outputSB.Append(currencySymbol);
                                    outputSB.Append("\"");
                                }
                                if (isFormatPercent)
                                {
                                    outputSB.Append("%");
                                }
                                outputFormat = outputSB.ToString();
                                string negativePattern = (negativeBraces ? "(" : "-") + outputFormat + (negativeBraces ? ")" : "");
                                if ((textComp != null) && textComp.HideZeros)   //add 25.11.2008
                                {
                                    outputFormat = outputFormat + ";" + negativePattern + ";";
                                }
                                else
                                {
                                    if (negativeBraces) outputFormat = outputFormat + ";" + negativePattern;
                                }
                            }
                        }
                        #endregion

                        int formatIndex = 0;    //general
                        if (isDefaultFormat)
                        {
                            if (isFormatNumeric)
                            {
                                formatIndex = 4;    //decimal	# ##0.00
                            }
                            if (isFormatCurrency)
                            {
                                formatIndex = 7;    //currency  # ##0.00
                            }
                            if (isFormatPercent)
                            {
                                formatIndex = 10;   //percent 0.00%
                            }
                            if (isFormatDate)
                            {
                                formatIndex = 14;   //date M/D/YY
                            }
                            if (isFormatTime)
                            {
                                formatIndex = 21;   //time h:mm:ss
                            }
                            if (isExcelText)
                            {
                                formatIndex = 49;   //text @
                            }
                        }
                        else
                        {
                            formatIndex = GetFormatNumber(outputFormat);
                        }
                        #endregion

                        #region Style
                        StiCellStyle style = cell.CellStyle;
                        tempFont = new DataFont(
                            style.Font.Name,
                            style.Font.Bold,
                            style.Font.Italic,
                            style.Font.Underline,
                            style.Font.Strikeout,
                            (int)(style.Font.SizeInPoints * 20), //* 0.964
                            GetColorNumber(style.TextColor),
                            style.Font.GdiCharSet);
                        if (hasCheckBoxExcelDataValue)
                        {
                            tempFont = new DataFont(
                                StiOptions.Export.CheckBoxReplacementForExcelValue.Font.Name,
                                StiOptions.Export.CheckBoxReplacementForExcelValue.Font.Bold,
                                StiOptions.Export.CheckBoxReplacementForExcelValue.Font.Italic,
                                StiOptions.Export.CheckBoxReplacementForExcelValue.Font.Underline,
                                StiOptions.Export.CheckBoxReplacementForExcelValue.Font.Strikeout,
                                (int)(StiOptions.Export.CheckBoxReplacementForExcelValue.Font.SizeInPoints * 20), //* 0.964
                                GetColorNumber(style.TextColor),
                                StiOptions.Export.CheckBoxReplacementForExcelValue.Font.GdiCharSet);
                        }

                        bool rightToLeft = false;
                        if (style.TextOptions != null)
                        {
                            rightToLeft = style.TextOptions.RightToLeft;
                        }

                        int rotationAngle = 0;
                        if (style.TextOptions != null)
                        {
                            int tempAngle = (int)(style.TextOptions.Angle % 360);
                            if (tempAngle < 0) tempAngle = 360 + tempAngle;
                            if ((tempAngle >= 0) && (tempAngle <= 90))
                            {
                                rotationAngle = tempAngle;
                            }
                            if ((tempAngle >= 270) && (tempAngle < 360))
                            {
                                rotationAngle = 360 - tempAngle + 90;
                            }
                        }

                        StiTextHorAlignment horAlignment = style.HorAlignment;
                        StiVertAlignment vertAlignment = style.VertAlignment;
                        StiExcel2007ExportService.ConvertRotatedAlignment(rotationAngle, ref horAlignment, ref vertAlignment);

                        int HorAlign;
                        switch (hasCheckBoxExcelDataValue ? StiOptions.Export.CheckBoxReplacementForExcelValue.HorAlignment : horAlignment)
                        {
                            case StiTextHorAlignment.Left: HorAlign = (!rightToLeft ? 0x01 : 0x03); break;
                            case StiTextHorAlignment.Right: HorAlign = (rightToLeft ? 0x01 : 0x03); break;
                            case StiTextHorAlignment.Center: HorAlign = 0x02; break;
                            case StiTextHorAlignment.Width: HorAlign = 0x07; break;
                            default: HorAlign = 0x00; break;
                        }

                        int VertAlign;
                        switch (hasCheckBoxExcelDataValue ? StiOptions.Export.CheckBoxReplacementForExcelValue.VertAlignment : vertAlignment)
                        {
                            case StiVertAlignment.Top: VertAlign = 0x00; break;
                            case StiVertAlignment.Center: VertAlign = 0x01; break;
                            case StiVertAlignment.Bottom: VertAlign = 0x02; break;
                            default: VertAlign = 0x00; break;
                        }

                        int textWordWrap = 0;
                        bool tempWordWrap = false;
                        if (style.TextOptions != null) tempWordWrap = style.TextOptions.WordWrap;
                        if ((str != null) && (str.Length > 0) &&
                            ((str.IndexOf("\r", StringComparison.InvariantCulture) != -1) ||
                            (str.IndexOf("\n", StringComparison.InvariantCulture) != -1)))
                            tempWordWrap = true;
                        if (tempWordWrap == true)
                        {
                            textWordWrap = 0x01;
                        }

                        bool needBorderLeft = true;
                        bool needBorderRight = true;
                        for (int index = 0; index < cell.Height + 1; index++)
                        {
                            if (matrix.BordersY[cell.Top + index, cell.Left] == null) needBorderLeft = false;
                            if (matrix.BordersY[cell.Top + index, cell.Left + cell.Width + 1] == null) needBorderRight = false;
                        }
                        bool needBorderTop = true;
                        bool needBorderBottom = true;
                        for (int index = 0; index < cell.Width + 1; index++)
                        {
                            if (matrix.BordersX[cell.Top, cell.Left + index] == null) needBorderTop = false;
                            if (matrix.BordersX[cell.Top + cell.Height + 1, cell.Left + index] == null) needBorderBottom = false;
                        }

                        tempXF = new DataXF(
                            GetFontNumber(tempFont),
                            formatIndex,    //formatIndex
                            0x00,   //XF style 
                            0x0000, //parent style XF
                            HorAlign,
                            VertAlign,
                            textWordWrap,
                            rotationAngle,
                            (rightToLeft ? 0x02 : 0x01),
                            0x3F,   //usedAttrib
                            GetLineStyle(needBorderLeft ? matrix.BordersY[rowIndex - 1, columnIndex - 1] : null),   //left
                            GetLineStyle(needBorderRight ? matrix.BordersY[rowIndex - 1, columnIndex - 1 + cell.Width + 1] : null),
                            GetLineStyle(needBorderTop ? matrix.BordersX[rowIndex - 1, columnIndex - 1] : null),
                            GetLineStyle(needBorderBottom ? matrix.BordersX[rowIndex - 1 + cell.Height + 1, columnIndex - 1] : null),
                            (needBorderLeft ? GetColorNumber(matrix.BordersY[rowIndex - 1, columnIndex - 1].Color) : 0x00),
                            (needBorderRight ? GetColorNumber(matrix.BordersY[rowIndex - 1, columnIndex - 1 + cell.Width + 1].Color) : 0x00),
                            (needBorderTop ? GetColorNumber(matrix.BordersX[rowIndex - 1, columnIndex - 1].Color) : 0x00),
                            (needBorderBottom ? GetColorNumber(matrix.BordersX[rowIndex - 1 + cell.Height + 1, columnIndex - 1].Color) : 0x00),
                            (((style.Color.A != 0) && (style.Color != Color.White)) ? 0x01 : 0x00), //fill pattern
                            GetColorNumber(style.Color),    //color index pattern
                            GetColorNumber(style.Color));   //color index background

                        int indexStyle = GetXFNumber(tempXF);

                        if (!exportObjectFormatting) indexStyle = 15;   //default style

                        #endregion

                        #region Range
                        if (exportObjectFormatting)
                        {
                            for (int xx = 0; xx <= cell.Width; xx++)
                            {
                                for (int yy = 0; yy <= cell.Height; yy++)
                                {
                                    readyCells[rowIndex - 1 + yy, columnIndex - 1 + xx] = true;
                                    mergedCellsStyle[rowIndex + yy, columnIndex + xx] = indexStyle;
                                }
                            }
                            mergedCellsStyle[rowIndex, columnIndex] = 0;
                            if ((cell.Width > 0) || (cell.Height > 0))
                            {
                                tempCellRange = new CellRangeAddress(
                                    rowIndex - minRowIndex,
                                    rowIndex + cell.Height - minRowIndex,
                                    columnIndex - 1,
                                    columnIndex + cell.Width - 1);
                                mergedCells.Add(tempCellRange);
                            }
                        }
                        else
                        {
                            readyCells[rowIndex - 1, columnIndex - 1] = true;
                            mergedCellsStyle[rowIndex, columnIndex] = 0;
                        }
                        #endregion

                        #region Text
                        string excelDataValue = null;
                        if (textComp != null)
                        {
                            excelDataValue = textComp.ExcelDataValue;
                            if (isFormatNumeric && excelDataValue == "" && str == " ")
                            {
                                str = "";
                            }
                        }

                        if ((!cell.Component.IsExportAsImage(StiExportFormat.Excel)) &&
                            (!string.IsNullOrEmpty(str) || (!string.IsNullOrEmpty(excelDataValue) && excelDataValue != "-")))
                        {
                            bool isNumber = false;
                            double Number = 0;
                            if (!string.IsNullOrEmpty(excelDataValue) && excelDataValue != "-")
                            {
                                string value = excelDataValue;

                                string sep = reportCulture.NumberFormat.NumberDecimalSeparator;
                                string value2 = value.Replace(".", ",").Replace(",", sep);

                                if (!string.IsNullOrEmpty(value2))
                                {
                                    isNumber = true;
                                    try
                                    {
                                        if (isFormatDate || isFormatTime)
                                        {
                                            if (StiOptions.Export.Excel.AllowExportDateTime)
                                            {
                                                var dt = DateTime.Parse(value2, reportCulture);
                                                //if (isFormatDate)
                                                //{
                                                //    Number = dt.Subtract(new DateTime(1900, 1, 1)).Days + 1 + 1;
                                                //}
                                                //else
                                                //{
                                                //    Number = (double)dt.TimeOfDay.TotalSeconds / 86400d;
                                                //}
                                                double days = dt.Subtract(new DateTime(1900, 1, 1)).Days + 1 + 1;
                                                double seconds = (double)dt.TimeOfDay.TotalSeconds / 86400d;
                                                Number = days + seconds;
                                            }
                                            else
                                            {
                                                isNumber = false;
                                            }
                                        }
                                        else
                                        {
                                            //Number = double.Parse(value2, NumberStyles.Any);
                                            if (!double.TryParse(value2, NumberStyles.Any, reportCulture, out Number))
                                            {
                                                isNumber = false;
                                            }
                                        }
                                    }
                                    catch (FormatException)
                                    {
                                        isNumber = false;
                                    }
                                }

                                if (isFormatCustom) isNumber = false;

                                if ((!isNumber) && (str == null))
                                {
                                    str = value;
                                }
                            }

                            if (isNumber == true)
                            {
                                dwWord(0, (Int16)(rowIndex - minRowIndex));     //index to row
                                dwWord(2, (Int16)(columnIndex - 1));    //index to column
                                dwWord(4, (Int16)indexStyle);   //index to XF
                                dwDouble(6, Number);
                                AddRec(0x0203, 14);             //Number	6.68
                            }
                            else
                            {
                                string strs = str.Replace("\r", "").Replace('\t', ' ');
                                if (StiOptions.Export.Excel.TrimTrailingSpaces) strs = StiExportUtils.TrimEndWhiteSpace(strs);
                                int indexSST = GetSSTNumber(strs);
                                dwFill(0, 0x00, 10);
                                dwWord(0, (Int16)(rowIndex - minRowIndex));     //index to row
                                dwWord(2, (Int16)(columnIndex - 1));    //index to column
                                dwWord(4, (Int16)indexStyle);   //index to XF
                                dwDWord(6, indexSST);           //index into SST
                                AddRec(0x00FD, 10);             //LabelSST	6.61
                            }
                        }
                        else
                        {
                            dwFill(0, 0x00, 6);
                            dwWord(0, (Int16)(rowIndex - minRowIndex));     //index to row
                            dwWord(2, (Int16)(columnIndex - 1));    //index to column
                            dwWord(4, (Int16)indexStyle);   //index to XF
                            AddRec(0x0201, 6);              //Blank		6.7
                        }
                        #endregion
                    }
                    else
                    {
                        if (mergedCellsStyle[rowIndex, columnIndex] != 0)
                        {
                            int indexStyle = mergedCellsStyle[rowIndex, columnIndex];
                            dwFill(0, 0x00, 6);
                            dwWord(0, (Int16)(rowIndex - minRowIndex));     //index to row
                            dwWord(2, (Int16)(columnIndex - 1));    //index to column
                            dwWord(4, (Int16)indexStyle);   //index to XF
                            AddRec(0x0201, 6);              //Blank		6.7
                        }
                        else
                        {
                            bool needBorderLeft = true;
                            bool needBorderRight = true;
                            bool needBorderTop = true;
                            bool needBorderBottom = true;
                            if (matrix.BordersY[rowIndex - 1, columnIndex - 1] == null) needBorderLeft = false;
                            if (matrix.BordersY[rowIndex - 1, columnIndex - 0] == null) needBorderRight = false;
                            if (matrix.BordersX[rowIndex - 1, columnIndex - 1] == null) needBorderTop = false;
                            if (matrix.BordersX[rowIndex - 0, columnIndex - 1] == null) needBorderBottom = false;

                            if (needBorderLeft || needBorderRight || needBorderTop || needBorderBottom)
                            {
                                #region Style
                                tempXF = new DataXF(
                                    0,//font
                                    0x00,   //formatIndex
                                    0x00,   //XF style 
                                    0x0000, //parent style XF
                                    0,
                                    0,
                                    0,
                                    0,
                                    0,
                                    0x3F,   //usedAttrib
                                    GetLineStyle(needBorderLeft ? matrix.BordersY[rowIndex - 1, columnIndex - 1] : null),   //left
                                    GetLineStyle(needBorderRight ? matrix.BordersY[rowIndex - 1, columnIndex - 0] : null),
                                    GetLineStyle(needBorderTop ? matrix.BordersX[rowIndex - 1, columnIndex - 1] : null),
                                    GetLineStyle(needBorderBottom ? matrix.BordersX[rowIndex - 0, columnIndex - 1] : null),
                                    (needBorderLeft ? GetColorNumber(matrix.BordersY[rowIndex - 1, columnIndex - 1].Color) : 0x00),
                                    (needBorderRight ? GetColorNumber(matrix.BordersY[rowIndex - 1, columnIndex - 0].Color) : 0x00),
                                    (needBorderTop ? GetColorNumber(matrix.BordersX[rowIndex - 1, columnIndex - 1].Color) : 0x00),
                                    (needBorderBottom ? GetColorNumber(matrix.BordersX[rowIndex - 0, columnIndex - 1].Color) : 0x00),
                                    0x00,   //fill pattern
                                    0,  //color index pattern
                                    0); //color index background

                                int indexStyle = GetXFNumber(tempXF);

                                if (!exportObjectFormatting) indexStyle = 15;   //default style

                                #endregion

                                dwFill(0, 0x00, 6);
                                dwWord(0, (Int16)(rowIndex - minRowIndex));     //index to row
                                dwWord(2, (Int16)(columnIndex - 1));    //index to column
                                dwWord(4, (Int16)indexStyle);   //index to XF
                                AddRec(0x0201, 6);              //Blank		6.7

                            }
                        }
                    }
                    #endregion

                    //					#region merged cells style write
                    //					if (mergedCellsStyle[rowIndex, columnIndex] != 0)
                    //					{
                    //						int indexStyle = mergedCellsStyle[rowIndex, columnIndex];
                    //						dwFill(0, 0x00, 6);
                    //						dwWord(0, (Int16)(rowIndex - minRowIndex));		//index to row
                    //						dwWord(2, (Int16)(columnIndex - 1));	//index to column
                    //						dwWord(4, (Int16)indexStyle);	//index to XF
                    //						AddRec(0x0201, 6);				//Blank		6.7
                    //					}
                    //					#endregion
                }
            }
            if (richtextForConvert != null) richtextForConvert.Dispose();
            #endregion

            #region MergedCells
            if ((mergedCells.Count > 0) && ((dataExportMode == StiDataExportMode.AllBands) || exportObjectFormatting))
            {
                int pos = 0;
                for (int index = 0; index < mergedCells.Count; index++)
                {
                    if (pos >= 1027)    //limit
                    {
                        dwWord(0, (Int16)pos);
                        AddRec(0x00E5, pos * 8 + 2);    //MERGEDCELLS
                        pos = 0;
                    }
                    tempCellRange = (CellRangeAddress)mergedCells[index];
                    dwWord(2 + pos * 8 + 0, (Int16)tempCellRange.FirstRow);
                    dwWord(2 + pos * 8 + 2, (Int16)tempCellRange.LastRow);
                    dwWord(2 + pos * 8 + 4, (Int16)tempCellRange.FirstColumn);
                    dwWord(2 + pos * 8 + 6, (Int16)tempCellRange.LastColumn);
                    pos++;
                }
                dwWord(0, (Int16)pos);
                AddRec(0x00E5, pos * 8 + 2);    //MERGEDCELLS
            }
            #endregion

            #region Images
            escher.WriteDG(imageList, startImageIndex);
            escher.mem.Seek(0, SeekOrigin.Begin);
            for (int index = startImageIndex; index < imageList.Count; index++)
            {
                int recordLength = escher.MemBookmarks[index + 1 - startImageIndex] - escher.MemBookmarks[index - startImageIndex];
                escher.mem.Read(bfData, 0, recordLength);
                AddRec(0x00EC, recordLength);   //MSODRAWING

                dwWord(0, 0x0015);  //ftCmo (15h)
                dwWord(2, 0x0012);  //len
                dwWord(4, 0x0008);  //Object type - picture
                dwWord(6, (short)(index + 1));  //object ID		- blipId
                dwWord(8, 0x6011);  //Option flags
                dwDWord(10, 0x0000);    //reserved
                dwDWord(14, 0x0000);    //reserved
                dwDWord(18, 0x0000);    //reserved
                dwWord(22, 0x0007); //ftCf (07h)
                dwWord(24, 0x0002); //len
                dwWord(26, -1); //reserved	//must be 0xFFFF
                dwWord(28, 0x0008); //ftPioGrbit (08h)
                dwWord(30, 0x0002); //len
                dwWord(32, 0x0000); //reserved
                dwWord(34, 0x0000); //ftEnd (00h)
                dwWord(36, 0x0000); //len
                AddRec(0x005D, 38); //OBJ - Graphic Object
            }
            escher.mem.Close();
            #endregion

            ushort window2OptionFlags = 0x04b4;
            if (sheetIndex == 0)
            {
                window2OptionFlags |= 0x0200;
            }
            if (StiOptions.Export.Excel.ColumnsRightToLeft)
            {
                window2OptionFlags |= 0x0040;
            }
            if (StiOptions.Export.Excel.ShowGridLines)
            {
                window2OptionFlags |= 0x0002;
            }
            dwFill(0, 0x00, 18);
            dwWord(0, window2OptionFlags);  //
            dwWord(2, 0x0000);  //
            dwWord(4, 0x0000);  //
            dwWord(6, 0x0040);  //
            dwWord(8, 0x0000);  //
            dwWord(10, 0x0000); //
            dwWord(12, 0x0000); //
            AddRec(0x023E, 18); //WINDOW2		6.109

            dwByte(0, 0x03);    //Pane identifier (see PANE record, 6.71)
            dwWord(1, 0x0000);  //Index to row of the active cell	
            dwWord(3, 0x0000);  //Index to column of the active cell	
            dwWord(5, 0x0000);  //Index into the following cell range list to the entry that contains the active cell	
            dwWord(7, 0x0001);  //Number of following cell range addresses	
            dwWord(9, 0x0000);  //Index to first row	
            dwWord(11, 0x0000); //Index to last row	
            dwByte(13, 0x0000); //Index to first column	
            dwByte(14, 0x0000); //Index to last column
            AddRec(0x001D, 15); //SELECTION		6.88

            if (hlinks.Count > 0)
            {
                for (int indexLink = 0; indexLink < hlinks.Count; indexLink++)
                {
                    byte[] buf = (byte[])hlinks[indexLink];
                    Array.Copy(buf, 0, bfData, 0, buf.Length);
                    AddRec(0x01B8, buf.Length); //HLINK		6.54
                }
            }

            if (maxRowIndex < Matrix.CoordY.Count)
            {
                //string[,] tempBookmark = new string[maxRowIndex - minRowIndex, Matrix.CoordX.Count];
                //Array.Copy(matrix.Bookmarks.GetBookmarksTable(), minRowIndex, tempBookmark, 0, maxRowIndex - minRowIndex);
                //bookmarksList.Add(tempBookmark);

                Hashtable tempTable = matrix.Bookmarks.BookmarksTable;
                Hashtable bookmarks = new Hashtable();
                if (tempTable != null)
                {
                    foreach (DictionaryEntry de in tempTable)
                    {
                        Size pos = (Size)de.Value;
                        if ((pos.Height >= minRowIndex) && (pos.Height < maxRowIndex))
                        {
                            bookmarks[de.Key] = new Size(pos.Width, pos.Height - minRowIndex);
                        }
                    }
                }
                bookmarksList.Add(bookmarks);
            }
            else
            {
                bookmarksList.Add(matrix.Bookmarks.BookmarksTable);
            }
            hlinksList.Add(hlinkLocal);

            //AddRec(0x000a, 0);	//EOF
            #endregion

            Size printArea = new Size(Matrix.CoordX.Count - 2, maxRowIndex - minRowIndex - 1);

            if (Matrix.CoordY.Count > maxRowIndex)
            {
                minRowIndex = maxRowIndex;
            }
            else
            {
                matrix = null;
            }

            return printArea;
        }

        private void MakeMainStream(StiReport report, StiPagesCollection allPages)
        {
            DataXF tempXF;
            int lenRec;
            int tempValue;
            ArrayList printAreasList = new ArrayList();

            CurrentPassNumber = 0;
            MaximumPassNumber = 3;

            int indexPage = 0;
            while (indexPage < allPages.Count)
            {
                StiPagesCollection pages = new StiPagesCollection(report, allPages);
                //pages.CacheMode = report.RenderedPages.CacheMode;
                pages.AddV2Internal(allPages.GetPageWithoutCache(indexPage));
                if (!exportEachPageToSheet)
                {
                    string pageName = allPages.GetPageWithoutCache(indexPage).ExcelSheetValue;
                    while ((indexPage < allPages.Count - 1) && CompareExcellSheetNames(allPages.GetPageWithoutCache(indexPage + 1).ExcelSheetValue, pageName))
                    {
                        indexPage++;
                        pages.AddV2Internal(allPages.GetPageWithoutCache(indexPage));
                    }
                }
                pages.CacheMode = report.RenderedPages.CacheMode;

                string sheetName = pages[0].ExcelSheetValue;
                if ((sheetName == null) || (sheetName == string.Empty))
                {
                    sheetName = string.Format("Page {0}", boundsheetsNamesList.Count + 1);
                }
                string sheetSuffix = "";
                int sheetIndex = 1;

                do
                {
                    memst = new Tools.StiCachedStream();
                    printAreasList.Add(MakeWorksheetStream(pages, pagesList.Count));
                    if (IsStopped) return;
                    pagesList.Add(memst);
                    if ((matrix != null) || (sheetSuffix.Length > 0)) sheetSuffix = string.Format(" part{0}", sheetIndex++);
                    boundsheetsNamesList.Add(sheetName + sheetSuffix);
                }
                while (matrix != null);

                indexPage++;
            }

            #region check sheet names
            Hashtable ht = new Hashtable();
            for (int index = 0; index < boundsheetsNamesList.Count; index++)
            {
                string titleString = (string)boundsheetsNamesList[index];
                titleString = titleString.Replace("*", "_").Replace("\\", "_").Replace("/", "_").Replace("[", "_").Replace("]", "_").Replace(":", "_").Replace("?", "_");
                //				if ((titleString == null) || (titleString == string.Empty))
                //				{
                //					titleString = string.Format("Page {0}", index + 1);
                //				}
                if (ht.Contains(titleString))
                {
                    int numVariant = 1;
                    while (ht.Contains(titleString + "-" + numVariant.ToString()))
                    {
                        numVariant++;
                    }
                    titleString = titleString + "-" + numVariant.ToString();
                }
                ht.Add(titleString, titleString);
                boundsheetsNamesList[index] = titleString;
            }
            #endregion

            memst = new Tools.StiCachedStream();

            #region make workbook globals
            dwFill(0, 0x00, 16);
            dwWord(0, 0x0600);  //BIFF8
            dwWord(2, 0x0005);  //workbook globals
            dwWord(4, 0x189D);  //build
            dwWord(6, 0x07CD);  //
            dwDWord(8, 0x000080C9); //
            dwDWord(12, 0x00000206);    //
            AddRec(0x0809, 16); //BOF

            #region write global parameters
            dwFill(0, 0x20, 128);
            dwString(0, new string(' ', 34) + Stimulsoft.Report.Export.StiExportUtils.GetReportVersion(report),
                2, out lenRec, true);
            AddRec(0x005C, 0x0070); //Writeaccess

            dwWord(0, 0x04B0);  //UTF-16
            AddRec(0x0042, 2);  //Codepage

            dwWord(0, 0x0000);
            AddRec(0x0161, 2);  //DSF 0

            dwWord(0, 0x00);
            AddRec(0x0019, 2);  //WINDOW PROTECT 0

            dwWord(0, 0x00);
            AddRec(0x0012, 2);  //PROTECT 0			6.77

            dwWord(0, 0x0000);
            AddRec(0x0013, 2);  //Password

            //empiric parameters!
            dwWord(0, 0x01E0);  //horz win pos
            dwWord(2, 0x001E);  //vert win pos
            dwWord(4, 0x379B);  //width 
            dwWord(6, 0x21FC);  //height
            dwWord(8, 0x0038);  //flags
            dwWord(10, 0x0000); //active sheet
            dwWord(12, 0x0000); //index visible bar
            dwWord(14, 0x0001); //selected tabs
            dwWord(16, 0x0258); //width sheet tab bar
            AddRec(0x003D, 18); //WINDOW1		6.108

            dwWord(0, 0x0000);
            AddRec(0x0040, 2);  //Backup

            dwWord(0, 0x0000);
            AddRec(0x008d, 2);  //HIDEOBJ 0

            dwWord(0, 0x0000);
            AddRec(0x0022, 2);  //1904 date system 0

            dwWord(0, 0x0001);
            AddRec(0x000e, 2);  //PRECISION 1

            dwWord(0, 0x0000);
            AddRec(0x00DA, 2);  //Bookbool
            #endregion

            #region fonts
            for (int index = 0; index < fontList.Count; index++)
            {
                DataFont dFont = (DataFont)fontList[index];
                dwFill(0, 0x00, 14);
                dwWord(0, (Int16)dFont.Height);		//height in twips (=1/20 of a point)
                int styleFlags = 0x0000;
                if (dFont.Italic)
                {
                    styleFlags |= 0x0002;   //italic
                }
                if (dFont.Strikeout)
                {
                    styleFlags |= 0x0008;	//strikeout
                }
                dwWord(2, (Int16)styleFlags);
                dwWord(4, (Int16)dFont.Color);      //color index	6.70
                if (dFont.Bold)
                {
                    dwWord(6, 0x02BC);  //font weight bold
                }
                else
                {
                    dwWord(6, 0x0190);  //font weight normal
                }
                dwWord(8, 0x0000);      //script
                if (dFont.Underlined)
                {
                    dwByte(10, 0x01);   //underline single
                }
                else
                {
                    dwByte(10, 0x00);   //underline none
                }
                dwByte(11, 0x00);       //font family
                dwByte(12, (byte)dFont.Charset);
                dwString(14, dFont.Name, 1, out lenRec, false);
                AddRec(0x0031, lenRec);     //FONT		6.43
            }
            #endregion

            #region format
            if (formatList.Count > 0)
            {
                for (int indexFormat = 0; indexFormat < formatList.Count; indexFormat++)
                {
                    dwWord(0, (short)(indexFormat + 164));  //index
                    dwString(2, (string)formatList[indexFormat], 2, out lenRec, false);
                    AddRec(0x041E, lenRec); //Format	6.45
                }
            }

            //			dwWord(0, 5);	//index
            //			dwString(2, "#,##0\".\";\\-#,##0\".\"", 2, out lenRec, false);
            //			AddRec(0x041E, lenRec);	//Format	6.45
            //
            //			dwWord(0, 6);	//index
            //			dwString(2, "#,##0\".\";[Red]\\-#,##0\".\"", 2, out lenRec, false);
            //			AddRec(0x041E, lenRec);	//Format	6.45
            //
            //			dwWord(0, 7);	//index
            //			dwString(2, "#,##0.00\".\";\\-#,##0.00\".\"", 2, out lenRec, false);
            //			AddRec(0x041E, lenRec);	//Format	6.45
            //
            //			dwWord(0, 8);	//index
            //			dwString(2, "#,##0.00\".\";[Red]\\-#,##0.00\".\"", 2, out lenRec, false);
            //			AddRec(0x041E, lenRec);	//Format	6.45
            //
            //			dwWord(0, 42);	//index
            //			dwString(2, "_-* #,##0\".\"_-;\\-* #,##0\".\"_-;_-* \"-\"\".\"_-;_-@_-", 2, out lenRec, false);
            //			AddRec(0x041E, lenRec);	//Format	6.45
            //
            //			dwWord(0, 41);	//index
            //			dwString(2, "_-* #,##0__._-;\\-* #,##0__._-;_-* \"-\"__._-;_-@_-", 2, out lenRec, false);
            //			AddRec(0x041E, lenRec);	//Format	6.45
            //
            //			dwWord(0, 44);	//index
            //			dwString(2, "_-* #,##0.00\".\"_-;\\-* #,##0.00\".\"_-;_-* \"-\"??\".\"_-;_-@_-", 2, out lenRec, false);
            //			AddRec(0x041E, lenRec);	//Format	6.45
            //
            //			dwWord(0, 43);	//index
            //			dwString(2, "_-* #,##0.00__._-;\\-* #,##0.00__._-;_-* \"-\"??__._-;_-@_-", 2, out lenRec, false);
            //			AddRec(0x041E, lenRec);	//Format	6.45
            #endregion

            #region XF
            for (int index = 0; index < xfList.Count; index++)
            {
                tempXF = (DataXF)xfList[index];
                dwFill(0, 0x00, 20);
                dwWord(0, (Int16)tempXF.FontIndex);     //index to FONT
                dwWord(2, (Int16)tempXF.FormatIndex);   //index to FORMAT
                tempValue = ((tempXF.XFType << 2) & 0x04) | ((tempXF.ParentStyleXF << 4) & 0xFFF0);
                dwWord(4, (UInt16)(tempValue | 0x01));  //XF type, index to parent style xf
                tempValue = tempXF.HorAlign | ((tempXF.TextWrapped << 3) & 0x08) | ((tempXF.VertAlign << 4) & 0x70);
                dwByte(6, (byte)tempValue);
                dwByte(7, (byte)tempXF.TextRotationAngle);
                dwByte(8, (byte)(tempXF.TextDirection << 6));
                dwByte(9, (byte)(tempXF.UsedAttrib << 2));
                tempValue = (tempXF.LineStyleLeft & 0x0000000F) | ((tempXF.LineStyleRight << 4) & 0x000000F0) |
                    ((tempXF.LineStyleTop << 8) & 0x00000F00) | ((tempXF.LineStyleBottom << 12) & 0x0000F000) |
                    ((tempXF.ColorIndexLeft << 16) & 0x007F0000) | ((tempXF.ColorIndexRight << 23) & 0x3F800000);
                dwDWord(10, tempValue);
                tempValue = (tempXF.ColorIndexTop & 0x0000007F) | ((tempXF.ColorIndexBottom << 7) & 0x00003F80) |
                    (int)((tempXF.FillPatern << 26) & 0xFC000000);
                dwDWord(14, tempValue);
                tempValue = (tempXF.ColorIndexPattern & 0x007F) | ((tempXF.ColorIndexBackground << 7) & 0x3F80);
                dwWord(18, (Int16)tempValue);
                AddRec(0x00E0, 20); //XF	6.115
            }
            #endregion

            #region style
            dwDWord(0, 0x8010); //built-in style XF
            dwByte(2, 0x04);        //style normal
            dwByte(3, 0xFF);        //
            AddRec(0x0293, 4);  //STYLE		6.99

            dwDWord(0, 0x8011); //built-in style XF
            dwByte(2, 0x07);        //style normal
            dwByte(3, 0xFF);        //
            AddRec(0x0293, 4);  //STYLE		6.99

            dwDWord(0, 0x8000); //built-in style XF = 0
            dwByte(2, 0x00);        //style normal
            dwByte(3, 0xFF);        //
            AddRec(0x0293, 4);  //STYLE		6.99

            dwDWord(0, 0x8012); //built-in style XF
            dwByte(2, 0x05);        //style normal
            dwByte(3, 0xFF);        //
            AddRec(0x0293, 4);  //STYLE		6.99

            dwDWord(0, 0x8013); //built-in style XF
            dwByte(2, 0x03);        //style normal
            dwByte(3, 0xFF);        //
            AddRec(0x0293, 4);  //STYLE		6.99

            dwDWord(0, 0x8014); //built-in style XF
            dwByte(2, 0x06);        //style normal
            dwByte(3, 0xFF);        //
            AddRec(0x0293, 4);  //STYLE		6.99
            #endregion

            #region palette
            if (colorList.Count > 0)
            {
                int paletteCount = colorList.Count;
                if (paletteCount > 56) paletteCount = 56;
                dwWord(0, (Int16)paletteCount);
                for (int index = 0; index < paletteCount; index++)
                {
                    Color tempColor = (Color)colorList[index];
                    dwByte(2 + index * 4 + 0, tempColor.R);
                    dwByte(2 + index * 4 + 1, tempColor.G);
                    dwByte(2 + index * 4 + 2, tempColor.B);
                    dwByte(2 + index * 4 + 3, 0);
                }
                AddRec(0x0092, 2 + paletteCount * 4);   //Palette	6.70
            }
            #endregion

            dwWord(0, 0x00);
            AddRec(0x0160, 2);  //USESELFS 0

            #region boundsheets
            for (int index = 0; index < pagesList.Count; index++)
            {
                string titleString = (string)boundsheetsNamesList[index];
                int boundsheetOffset = (int)memst.Position + 4; //4 bytes - record header
                boundsheetsOffsetsList.Add(boundsheetOffset);

                dwDWord(0, 0x0000); //offset	temporary
                dwByte(4, 0x00);    //visible
                dwByte(5, 0x00);    //worksheet
                dwString(6, titleString, 1, out lenRec, false);
                AddRec(0x0085, lenRec); //BOUNDSHEET
            }
            #endregion

            //			dwWord(0, 0x0007);	//Russia
            //			dwWord(2, 0x0007);	//Russia
            //			AddRec(0x008C, 4);	//Country

            dwWord(0, (short)pagesList.Count);  //Number of sheets in this document
            dwWord(2, 0x0401);  //
            AddRec(0x01AE, 4);  //SUPBOOK

            dwWord(0, (short)pagesList.Count);  //Number of following REF structures
            for (int index = 0; index < pagesList.Count; index++)
            {
                dwWord(2 + index * 6, 0x0000);  //
                dwWord(4 + index * 6, (short)index);    //
                dwWord(6 + index * 6, (short)index);    //
            }
            AddRec(0x0017, 2 + pagesList.Count * 6);    //EXTERNSHEET

            #region NAME - set sheets print area
            for (int index = 0; index < pagesList.Count; index++)
            {
                Size area = (Size)printAreasList[index];
                if (area.Height > 0xFFFF) area.Height = 0xFFFF;
                if (area.Width > 0xFF) area.Width = 0xFF;

                //dwFill(0, 0x00, 27);
                dwWord(0, 0x0020);  //Option flags - Built-in name
                dwByte(2, 0x00);    //Keyboard shortcut (only for command macro names)
                dwByte(3, 0x01);    //Length of the name (character count, ln)
                dwWord(4, 0x000B);  //Size of the formula data (sz)
                dwWord(6, 0x0000);  //Not used
                dwWord(8, (short)(index + 1));  //index to sheet (one-based)
                dwByte(10, 0x00);   //Length of menu text (character count, lm)
                dwByte(11, 0x00);   //Length of description text (character count, ld)
                dwByte(12, 0x00);   //Length of help topic text (character count, lh)
                dwByte(13, 0x00);   //Length of status bar text (character count, ls)
                dwByte(14, 0x00);   //Unicode string flags
                dwByte(15, 0x06);   //Unicode string - 06 - Print_Area
                dwByte(16, 0x3B);   //
                dwWord(17, (short)index);   //sheet index
                dwWord(19, 0x0000); //
                dwWord(21, (short)area.Height); //row
                dwWord(23, 0x0000); //
                dwWord(25, (short)area.Width);  //column

                AddRec(0x0018, 27); //NAME
            }
            #endregion

            #region Image store
            escher.WriteDGG(imageList);
            escher.mem.Seek(0, SeekOrigin.Begin);
            int posMem = 0;
            short tempDGGCommand = 0x00EB;  //MSODRAWINGGROUP
            while (posMem + maxRecordLength < escher.mem.Length)
            {
                escher.mem.Read(bfData, 0, maxRecordLength);
                AddRec(tempDGGCommand, maxRecordLength);
                tempDGGCommand = 0x003C;    //CONTINUE
                posMem += maxRecordLength;
            }
            escher.mem.Read(bfData, 0, (int)(escher.mem.Length - posMem));
            AddRec(tempDGGCommand, (int)(escher.mem.Length - posMem));
            escher.mem.Close();
            #endregion

            #region SST
            dwDWord(0, sstCounter);
            dwDWord(4, sstList.Count);

            int extSstPortionSize = sstList.Count / 1020 + 1;
            if (extSstPortionSize < 8) extSstPortionSize = 8;
            int extSstPos = 0;
            int extSstSize = sstList.Count / extSstPortionSize;
            if (sstList.Count % extSstPortionSize > 0) extSstSize++;
            int[,] extSstTable = new int[extSstSize, 2];

            int tempCommand = 0x00FC;   //SST	6.96
            int tempPos = 8;    //4(count) + 4(count)
            if (sstList.Count > 0)
            {
                for (int index = 0; index < sstList.Count; index++)
                {
                    //extSstPortionSize strings in each portion of EXTSST
                    if (index % extSstPortionSize == 0)
                    {
                        extSstTable[extSstPos, 0] = (int)memst.Position + tempPos + 4;	//4(header)
                        extSstTable[extSstPos, 1] = tempPos + 4;	//4(header)
                        extSstPos++;
                    }

                    string tempString = (string)sstList[index];
                    //int tempStringLen = 3 + tempString.Length;
                    //if (stringMayBePacked(tempString) == false)	
                    //{
                    //    tempStringLen = 3 + tempString.Length * 2;
                    //}
                    dwString(tempPos, tempString, 2, out lenRec, true);
                    byte flagByte = bfData[tempPos + 2];
                    tempPos = lenRec;

                    int recSize = 0x2000 + (tempPos & 0x01);
                    while (tempPos > recSize)
                    {
                        AddRec(tempCommand, recSize);
                        bfData[0] = flagByte;
                        Array.Copy(bfData, recSize, bfData, 1, tempPos);
                        tempPos -= recSize - 1;
                        recSize = 0x2000 + (tempPos & 0x01);

                        tempCommand = 0x003C;   //Continue
                                                //tempPos = 0;
                    }
                    if (tempPos > recSize - 10)
                    {
                        AddRec(tempCommand, tempPos);
                        tempCommand = 0x003C;	//Continue
                        tempPos = 0;
                    }
                }
            }
            if (tempPos != 0) AddRec(tempCommand, tempPos);
            #endregion

            #region EXTSST
            dwWord(0, (Int16)extSstPortionSize);    //8 strings in each portion of EXTSST
            tempPos = 2;
            if (sstList.Count > 0)
            {
                for (int index = 0; index < extSstSize; index++)
                {
                    dwDWord(tempPos + 0, extSstTable[index, 0]);
                    dwDWord(tempPos + 4, extSstTable[index, 1]);
                    dwWord(tempPos + 6, 0x0000);
                    tempPos += 8;
                }
            }
            AddRec(0x00FF, tempPos);    //EXTSST		6.40
            #endregion

            AddRec(0x000a, 0);  //EOF
            #endregion

            for (int index = 0; index < pagesList.Count; index++)
            {
                int temppos = (int)memst.Position;
                memst.Seek((int)boundsheetsOffsetsList[index], SeekOrigin.Begin);
                dwDWord(0, temppos);    //offset
                memst.Write(bfData, 0, 4);
                memst.Seek(temppos, SeekOrigin.Begin);

                MemoryStream memst2 = (MemoryStream)pagesList[index];
                memst2.WriteTo(memst);
                memst2.Close();

                ArrayList hlinks = (ArrayList)hlinksList[index];
                for (int indexHlink = 0; indexHlink < hlinks.Count; indexHlink++)
                {
                    HlinkData hl = (HlinkData)hlinks[indexHlink];
                    string hyperlink = hl.Bookmark;

                    #region find bookmark
                    bool isFounded = false;
                    for (int indexSheet = 0; indexSheet < bookmarksList.Count; indexSheet++)
                    {
                        //string[,] bookmarks = (string[,])bookmarksList[indexSheet];
                        //int sizeRow = bookmarks.GetLength(0);
                        //int sizeColumn = bookmarks.GetLength(1);
                        //for (int indexRow = 0; indexRow < sizeRow; indexRow++)
                        //{
                        //    for (int indexColumn = 0; indexColumn < sizeColumn; indexColumn++)
                        //    {
                        //        if (bookmarks[indexRow, indexColumn] == hyperlink)
                        //        {
                        //            hyperlink = string.Format("'{0}'!{1}", (string)boundsheetsNamesList[indexSheet], GetRefString(indexColumn, indexRow));
                        //            isFounded = true;
                        //            break;
                        //        }
                        //    }
                        //    if (isFounded) break;
                        //}
                        //if (isFounded) break;

                        Hashtable bookmarks = (Hashtable)bookmarksList[indexSheet];
                        if (bookmarks != null)
                        {
                            object obj = bookmarks[hyperlink];
                            if (obj != null)
                            {
                                Size pos = (Size)obj;
                                hyperlink = string.Format("'{0}'!{1}", (string)boundsheetsNamesList[indexSheet], GetRefString(pos.Width, pos.Height));
                                isFounded = true;
                                break;
                            }
                        }
                    }
                    #endregion

                    if (isFounded)
                    {
                        #region Store link to workbook
                        dwWord(0, (short)hl.Range.FirstRow);
                        dwWord(2, (short)hl.Range.LastRow);
                        dwWord(4, (short)hl.Range.FirstColumn);
                        dwWord(6, (short)hl.Range.LastColumn);
                        dwByte(8, 0xD0, 0xC9, 0xEA, 0x79);  //guid
                        dwByte(12, 0xF9, 0xBA, 0xCE, 0x11);
                        dwByte(16, 0x8C, 0x82, 0x00, 0xAA);
                        dwByte(20, 0x00, 0x4B, 0xA9, 0x0B);
                        dwDWord(24, 0x00000002);    //unknown
                        dwDWord(28, 0x0000001C);    //option flag

                        //store description
                        int hpos = 32;
                        dwDWord(hpos, hl.Description.Length + 1);
                        hpos += 4;
                        for (int indexChar = 0; indexChar < hl.Description.Length; indexChar++)
                        {
                            dwWord(hpos, (ushort)hl.Description[indexChar]);
                            hpos += 2;
                        }
                        dwWord(hpos, 0);
                        hpos += 2;

                        //store link to workbook
                        dwDWord(hpos, hyperlink.Length + 1);
                        hpos += 4;
                        for (int indexChar = 0; indexChar < hyperlink.Length; indexChar++)
                        {
                            dwWord(hpos, (ushort)hyperlink[indexChar]);
                            hpos += 2;
                        }
                        dwWord(hpos, 0);
                        hpos += 2;

                        AddRec(0x01B8, hpos);   //HLINK		6.54
                        #endregion
                    }
                }

                AddRec(0x000a, 0);  //EOF	end of substream
            }
        }
        #endregion

        #region write to OLE container
        private void WriteToOLEContainer()
        {
            int lenRec;

            #region calculate file parameters
            //main stream length in sectors
            int DirLenSec = 1;
            int memLenSec = (int)(memst.Length / 512);
            if ((memst.Length % 512) != 0) memLenSec++;
            //minimal stream size
            if (memLenSec < 8) memLenSec = 8;

            int SatLenSec = 0;
            int MSatLenSec = 0;
            int SatLenSecOld;
            do
            {
                SatLenSecOld = SatLenSec;
                if (SatLenSec >= 109)
                {
                    MSatLenSec = ((SatLenSec - 109) / 127);
                    if (((SatLenSec - 109) % 127) != 0) MSatLenSec++;
                }
                else
                {
                    MSatLenSec = 0;
                }
                SatLenSec = ((memLenSec + DirLenSec + SatLenSecOld + MSatLenSec) / 128);
                if (((memLenSec + DirLenSec + SatLenSecOld + MSatLenSec) % 128) != 0) SatLenSec++;
            }
            while (SatLenSec != SatLenSecOld);
            #endregion

            memStMain = new Tools.StiCachedStream();

            #region write header and MSAT
            //write header
            dwFill(0, 0, 512);
            dwByte(0, 0xD0, 0xCF, 0x11, 0xE0);  //~identifier
            dwByte(4, 0xA1, 0xB1, 0x1A, 0xE1);  //~identifier
            dwWord(24, 0x003E); //~revision
            dwWord(26, 0x0003); //~version
            dwWord(28, -2);     //~Little-Endian, FFFE
            dwWord(30, 0x0009); //size of sector == 512 bytes
            dwWord(32, 0x0006); //size of short sector == 64 bytes
            dwDWord(44, SatLenSec); // number of sectors used for SAT	-----+
            dwDWord(48, MSatLenSec + SatLenSec + memLenSec);
            // SID of first sector of directory	-----+
            dwDWord(56, 0x1000);    //minimum size of stream
            dwDWord(60, -2);        //SID of first sector of SSAT
            dwDWord(64, 0);         //number of sectors used for SSAT
            dwDWord(68, ((MSatLenSec == 0) ? -2 : 0));
            //SID of first sector of MSAT		-----+
            dwDWord(72, MSatLenSec);    //number of sectors used for MSAT	-----+
            dwFill(76, 0xFF, 436 + MSatLenSec * 512);   //MSAT free sectors
                                                        //write MSAT
            int SatLenIndex = 0;
            int SatLenOffset = 0;
            while (SatLenIndex < SatLenSec)
            {
                if (((SatLenOffset % 128) == 108) && (SatLenOffset > 108))
                {
                    dwDWord(76 + SatLenOffset * 4, SatLenOffset / 128);
                }
                else
                {
                    dwDWord(76 + SatLenOffset * 4, MSatLenSec + SatLenIndex);
                    SatLenIndex++;
                }
                SatLenOffset++;
            }
            if (MSatLenSec > 0)
            {
                dwDWord(512 + MSatLenSec * 512 - 2, -2);
            }
            memStMain.Write(bfData, 0, 512 + MSatLenSec * 512);
            #endregion

            //check for big reports
            if (bfData.Length < (SatLenSec + 1) * 512)
            {
                bfData = new byte[(SatLenSec + 1) * 512];
            }

            #region write SAT
            dwFill(0, 0xFF, SatLenSec * 512);   //SAT free sectors
            for (int index = 0; index < MSatLenSec; index++)
            {
                dwDWord(index * 4, -4);
            }
            for (int index = 0; index < SatLenSec; index++)
            {
                dwDWord((MSatLenSec + index) * 4, -3);
            }
            for (int index = 0; index < memLenSec; index++)
            {
                dwDWord((MSatLenSec + SatLenSec + index) * 4, MSatLenSec + SatLenSec + index + 1);
            }
            dwDWord((MSatLenSec + SatLenSec + memLenSec - 1) * 4, -2);  //end of chain
            dwDWord((MSatLenSec + SatLenSec + memLenSec) * 4, -2);      //directory have only 1 sec
            memStMain.Write(bfData, 0, SatLenSec * 512);
            #endregion

            #region write main stream
            //append for full 512 bytes sector
            //			int lenAppend = (int)(memst.Position % 512);
            //			if (lenAppend != 0)	lenAppend = 512 - lenAppend;
            int lenAppend = memLenSec * 512 - (int)memst.Length;
            memst.WriteTo(memStMain);
            dwFill(0, 0, lenAppend);
            memStMain.Write(bfData, 0, lenAppend);
            #endregion

            #region write directory
            dwFill(0, 0, 512);
            dwString(0, "Root Entry", 0, out lenRec, false);
            dwWord(64, 22);     //size of used area
            dwByte(66, 0x05);   //root storage
            dwByte(67, 0x01);   //black node
            dwDWord(68, -1);    //DID of left child
            dwDWord(72, -1);    //DID of right child
            dwDWord(76, 1);     //DID of root node
            dwDWord(116, -2);   //SID of first sector short container
            dwDWord(120, 0);    //size of short container
            dwString(128 + 0, "Workbook", 0, out lenRec, false);
            dwWord(128 + 64, 18);       //size of used area
            dwByte(128 + 66, 0x02); //user stream
            dwByte(128 + 67, 0x01); //black node
            dwDWord(128 + 68, -1);  //DID of left child
            dwDWord(128 + 72, -1);  //DID of right child
            dwDWord(128 + 76, -1);  //DID of root node
            dwDWord(128 + 116, MSatLenSec + SatLenSec); //SID of first sector	-----+
            dwDWord(128 + 120, memLenSec * 512);          //size of stream		-----+
            memStMain.Write(bfData, 0, 512);
            #endregion

            memst.Close();
        }
        #endregion

        #region Escher Layer
        private class Escher
        {
            public MemoryStream mem = null;
            public int[] MemBookmarks = null;
            private int[] containerPos = null;
            private int containerNum;
            private byte[] buf = null;

            #region struct BiffImageData
            public struct BiffImageData
            {
                public ushort FirstRowIndex;
                public ushort FirstRowOffset;
                public ushort FirstColumnIndex;
                public ushort FirstColumnOffset;
                public ushort LastRowIndex;
                public ushort LastRowOffset;
                public ushort LastColumnIndex;
                public ushort LastColumnOffset;
                public byte[] ImageData;

                public BiffImageData(
                    ushort FirstRowIndex,
                    ushort FirstRowOffset,
                    ushort FirstColumnIndex,
                    ushort FirstColumnOffset,
                    ushort LastRowIndex,
                    ushort LastRowOffset,
                    ushort LastColumnIndex,
                    ushort LastColumnOffset,
                    byte[] ImageData)
                {
                    this.FirstRowIndex = FirstRowIndex;
                    this.FirstRowOffset = FirstRowOffset;
                    this.FirstColumnIndex = FirstColumnIndex;
                    this.FirstColumnOffset = FirstColumnOffset;
                    this.LastRowIndex = LastRowIndex;
                    this.LastRowOffset = LastRowOffset;
                    this.LastColumnIndex = LastColumnIndex;
                    this.LastColumnOffset = LastColumnOffset;
                    this.ImageData = ImageData;
                }
            }
            #endregion

            public Escher()
            {
                mem = new MemoryStream();
                containerPos = new int[10];
                containerNum = 0;
                buf = new byte[64];
            }

            public void WriteToStream(Stream stream)
            {
                mem.WriteTo(stream);
                mem.Close();
                stream.Flush();
            }

            #region Data Write procedures
            private void dwByte(int position, byte dataValue)   //1
            {
                buf[position] = dataValue;
            }
            private void dwWord(int position, ushort dataValue)
            {
                buf[position + 0] = (byte)(dataValue & 0xff);
                buf[position + 1] = (byte)((dataValue >> 8) & 0xff);
            }
            private void dwDWord(int position, uint dataValue)
            {
                buf[position + 0] = (byte)(dataValue & 0xff);
                buf[position + 1] = (byte)((dataValue >> 8) & 0xff);
                buf[position + 2] = (byte)((dataValue >> 16) & 0xff);
                buf[position + 3] = (byte)((dataValue >> 24) & 0xff);
            }
            private void dwByte(int position, byte dv1, byte dv2)   //2
            {
                dwByte(position, dv1);
                dwByte(position + 1, dv2);
            }
            private void dwByte(int position, byte dv1, byte dv2, byte dv3) //3
            {
                dwByte(position, dv1, dv2);
                dwByte(position + 2, dv3);
            }
            private void dwByte(int position, byte dv1, byte dv2, byte dv3, byte dv4)   //4
            {
                dwByte(position, dv1, dv2, dv3);
                dwByte(position + 3, dv4);
            }
            private void dwFill(int position, byte dataValue, int count)
            {
                for (int index = 0; index < count; index++)
                {
                    buf[position + index] = dataValue;
                }
            }
            #endregion

            private void WriteRecordHeader(ushort recordId, byte ver, ushort inst)
            {
                ushort options = (ushort)((inst << 4) | (ver & 0x0F));
                dwWord(0, options);
                dwWord(2, recordId);
                dwDWord(4, 0x0000);
                mem.Write(buf, 0, 8);
                containerPos[containerNum] = (int)mem.Position;
                containerNum++;
            }

            private void CloseRecord()
            {
                long endPos = mem.Position;
                containerNum--;
                long beginPos = containerPos[containerNum];
                mem.Seek(beginPos - 4, SeekOrigin.Begin);
                dwDWord(0, (uint)(endPos - beginPos));  //length of record
                mem.Write(buf, 0, 4);
                mem.Seek(endPos, SeekOrigin.Begin);
            }

            #region WriteDGG
            public void WriteDGG(ArrayList imageList)
            {
                mem = new MemoryStream();
                containerNum = 0;
                uint imageCount = (uint)imageList.Count;

                WriteRecordHeader(0xF000, 0x0F, 0x0000);    //MsofbtDggContainer

                #region MsofbtDgg
                WriteRecordHeader(0xF006, 0x00, 0x0000);    //MsofbtDgg
                dwDWord(0, (uint)(1024 + imageCount + 1024));   //spidMax; maybe (1024 + imageCount + 1)
                dwDWord(4, 0x0002);         //cidcl
                dwDWord(8, imageCount + 1); //cspSaved
                dwDWord(12, 1);             //cdgSaved
                dwDWord(16, 1);             //FIDCL
                dwDWord(20, imageCount + 1);//
                mem.Write(buf, 0, 24);
                CloseRecord();
                #endregion

                #region MsofbtBstoreContainer
                WriteRecordHeader(0xF001, 0x0F, (ushort)imageCount);    //MsofbtBstoreContainer 

                for (int index = 0; index < imageCount; index++)
                {
                    BiffImageData image = (BiffImageData)imageList[index];
                    byte[] bufBLIP = image.ImageData;

                    uint checksum = StiExportUtils.GetAdler32Checksum(bufBLIP);

                    #region MsofbtBSE
                    WriteRecordHeader(0xF007, 0x02, 0x0005);    //MsofbtBSE 
                    dwByte(0, 5);       //btWin32
                    dwByte(1, 5);       //btMacOS
                    dwFill(2, 0, 16);   //rgbUid
                    dwDWord(2, checksum);
                    dwDWord(6, checksum);
                    dwDWord(10, checksum);
                    dwDWord(14, checksum);
                    dwWord(18, 0x00FF); //tag
                    dwDWord(20, (uint)(bufBLIP.Length + 25));   //size
                    dwDWord(24, 1); //cRef
                    dwDWord(28, 0); //foDelay
                    dwByte(32, 0);  //usage
                    dwByte(33, 0);  //cbName
                    dwByte(34, 0);  //unused
                    dwByte(35, 0);  //unused
                    mem.Write(buf, 0, 36);

                    WriteRecordHeader(0xF01D, 0x00, 0x046A);    //MsofbtBLIP
                    dwFill(0, 0, 16);	//rgbUid
                    dwDWord(0, checksum);
                    dwDWord(4, checksum);
                    dwDWord(8, checksum);
                    dwDWord(12, checksum);
                    dwByte(16, 0xFF);   //tag
                    mem.Write(buf, 0, 17);

                    mem.Write(bufBLIP, 0, bufBLIP.Length);

                    CloseRecord();  //MsofbtBLIP
                    CloseRecord();  //MsofbtBSE
                    #endregion

                }
                CloseRecord();  //MsofbtBstoreContainer
                #endregion

                #region MsofbtOPT
                WriteRecordHeader(0xF00B, 0x03, 0x0003);    //MsofbtOPT 
                dwWord(0, 0x00BF);
                dwDWord(2, 0x00080008);
                dwWord(6, 0x0181);
                dwDWord(8, 0x08000041);
                dwWord(12, 0x01C0);
                dwDWord(14, 0x08000040);
                mem.Write(buf, 0, 18);
                CloseRecord();  //MsofbtOPT
                #endregion

                #region MsofbtSplitMenuColors
                WriteRecordHeader(0xF11E, 0x00, 0x0004);    //MsofbtSplitMenuColors 
                dwDWord(0, 0x0800000D);
                dwDWord(4, 0x0800000C);
                dwDWord(8, 0x08000017);
                dwDWord(12, 0x100000F7);
                mem.Write(buf, 0, 16);
                CloseRecord();  //MsofbtSplitMenuColors
                #endregion

                CloseRecord();  //MsofbtDggContainer
            }
            #endregion

            #region WriteDG
            public void WriteDG(ArrayList imageList, int startImageIndex)
            {
                mem = new MemoryStream();
                containerNum = 0;
                uint imageCount = (uint)imageList.Count - (uint)startImageIndex;
                MemBookmarks = new int[imageCount + 1];

                WriteRecordHeader(0xF002, 0x0F, 0x0000);    //MsofbtDgContainer

                #region MsofbtDg
                WriteRecordHeader(0xF008, 0x00, 0x0001);    //MsofbtDg
                dwDWord(0, imageCount + 1);     //csp
                dwDWord(4, (ushort)(1024 + imageCount)); //spidCur
                mem.Write(buf, 0, 8);
                CloseRecord();
                #endregion

                #region MsofbtSpgrContainer
                WriteRecordHeader(0xF003, 0x0F, 0x0000);    //MsofbtSpgrContainer 

                #region MsofbtSpContainer default
                WriteRecordHeader(0xF004, 0x0F, 0x0000);    //MsofbtSpContainer 

                #region MsofbtSpgr
                WriteRecordHeader(0xF009, 0x01, 0x0000);    //MsofbtSpgr
                dwFill(0, 0, 16);
                mem.Write(buf, 0, 16);  //rect
                CloseRecord();  //MsofbtSpgr
                #endregion

                #region MsofbtSp
                WriteRecordHeader(0xF00A, 0x02, 0x0000);    //MsofbtSp 
                dwDWord(0, 1024);       //spid
                dwDWord(4, 0x00000005); //grfPersistent
                mem.Write(buf, 0, 8);
                CloseRecord();  //MsofbtSp
                #endregion

                CloseRecord();  //MsofbtSpContainer default
                #endregion

                for (int index = 0; index < imageCount; index++)
                {
                    MemBookmarks[index] = (int)mem.Position;

                    #region MsofbtSpContainer
                    WriteRecordHeader(0xF004, 0x0F, 0x0000);    //MsofbtSpContainer 

                    #region MsofbtSp
                    WriteRecordHeader(0xF00A, 0x02, 0x004B);    //MsofbtSp 
                    dwDWord(0, (ushort)(1024 + startImageIndex + index + 1));   //spid
                    dwDWord(4, 0x00000A00);             //grfPersistent
                    mem.Write(buf, 0, 8);
                    CloseRecord();  //MsofbtSp
                    #endregion

                    #region MsofbtOPT
                    WriteRecordHeader(0xF00B, 0x03, 0x0002);    //MsofbtOPT 
                    dwWord(0, 0x4104);      //blip.bliptodisplay
                    dwDWord(2, (uint)(startImageIndex + index + 1));
                    string st = string.Format("graph{0:X4}", startImageIndex + index + 1) + (char)0x0000;
                    dwWord(6, 0xC105);      //blip.blipfilename
                    dwDWord(8, (uint)(st.Length * 2));
                    for (int tempIndex = 0; tempIndex < st.Length; tempIndex++)
                    {
                        dwWord(12 + tempIndex * 2, (ushort)st[tempIndex]);
                    }
                    mem.Write(buf, 0, 12 + st.Length * 2);
                    CloseRecord();  //MsofbtOPT
                    #endregion

                    #region MsofbtClientAnchor
                    BiffImageData image = (BiffImageData)imageList[startImageIndex + index];
                    WriteRecordHeader(0xF010, 0x00, 0x0000);    //MsofbtClientAnchor 
                    dwWord(0, 2);   //flags	
                    dwWord(2, image.FirstColumnIndex);  //col1
                    dwWord(4, image.FirstColumnOffset); //dx1
                    dwWord(6, image.FirstRowIndex);     //row1
                    dwWord(8, image.FirstRowOffset);    //dy1
                    dwWord(10, image.LastColumnIndex);  //col2
                    dwWord(12, image.LastColumnOffset); //dx2
                    dwWord(14, image.LastRowIndex);     //row2
                    dwWord(16, image.LastRowOffset);    //dy2
                    mem.Write(buf, 0, 18);
                    CloseRecord();  //MsofbtClientAnchor
                    #endregion

                    WriteRecordHeader(0xF011, 0x00, 0x0000);    //MsofbtClientData
                    CloseRecord();  //MsofbtClientData

                    CloseRecord();  //MsofbtSpContainer
                    #endregion

                }
                CloseRecord();  //MsofbtSpgrContainer
                #endregion

                CloseRecord();  //MsofbtDgContainer

                MemBookmarks[0] = 0;
                MemBookmarks[imageCount] = (int)mem.Position;
            }
            #endregion

        }
        #endregion

        /// <summary>
        /// Exports rendered report to an Excel file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="fileName">A name of the file for exporting a rendered report.</param>
        public void ExportExcel(StiReport report, string fileName)
        {
            StiFileUtils.ProcessReadOnly(fileName);
            using (var stream = new FileStream(fileName, FileMode.Create))
            {
                ExportExcel(report, stream);
                stream.Flush();
                stream.Close();
            }
        }


        /// <summary>
        /// Exports rendered report to an Excel file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream for export of a document.</param>
        public void ExportExcel(StiReport report, Stream stream)
        {
            ExportExcel(report, stream, new StiExcelExportSettings());
        }


        /// <summary>
        /// Exports rendered report to an Excel file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream for export of a document.</param>
        /// <param name="pageRange">Describes range of pages of the document for the export.</param>
        public void ExportExcel(StiReport report, Stream stream, StiPagesRange pageRange)
        {
            var settings = new StiExcelExportSettings
            {
                PageRange = pageRange
            };

            ExportExcel(report, stream, settings);
        }


        /// <summary>
        /// Exports rendered report to an Excel file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream for export of a document.</param>
        /// <param name="pageRange">Describes range of pages of the document for the export.</param>
        /// <param name="useOnePageHeaderAndFooter">If true then the first page header and the last page footer
        /// will be exported for all exported documents.</param>
        public void ExportExcel(StiReport report, Stream stream, StiPagesRange pageRange, bool useOnePageHeaderAndFooter)
        {
            var settings = new StiExcelExportSettings
            {
                UseOnePageHeaderAndFooter = useOnePageHeaderAndFooter,
                PageRange = pageRange
            };

            ExportExcel(report, stream, settings);
        }


        /// <summary>
        /// Exports rendered report to an Excel file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream for export of a document.</param>
        /// <param name="pageRange">Describes range of pages of the document for the export.</param>
        /// <param name="useOnePageHeaderAndFooter">If true then the first page header and the last page footer
        /// will be exported for all exported documents.</param>
        /// <param name="exportDataOnly">If true then only data from the DataBand of the page will be exported.</param>
        /// <param name="exportObjectFormatting">If true then the Object formatting will be applied for the exported file.
        /// If the ExportDataOnly is false then this parameter is ignored.</param>
        public void ExportExcel(StiReport report, Stream stream, StiPagesRange pageRange, bool useOnePageHeaderAndFooter,
            bool exportDataOnly, bool exportObjectFormatting)
        {
            var settings = new StiExcelExportSettings
            {
                UseOnePageHeaderAndFooter = useOnePageHeaderAndFooter,
                DataExportMode = exportDataOnly ? StiDataExportMode.Data | StiDataExportMode.Headers : StiDataExportMode.AllBands,
                ExportObjectFormatting = exportObjectFormatting,
                PageRange = pageRange
            };

            ExportExcel(report, stream, settings);
        }


        public void ExportExcel(StiReport report, Stream stream, StiPagesRange pageRange, bool useOnePageHeaderAndFooter,
            bool exportDataOnly, bool exportObjectFormatting, bool exportEachPageToSheet)
        {
            var settings = new StiExcelExportSettings
            {
                UseOnePageHeaderAndFooter = useOnePageHeaderAndFooter,
                DataExportMode = exportDataOnly ? StiDataExportMode.Data | StiDataExportMode.Headers : StiDataExportMode.AllBands,
                ExportObjectFormatting = exportObjectFormatting,
                ExportEachPageToSheet = exportEachPageToSheet,
                PageRange = pageRange
            };

            ExportExcel(report, stream, settings);
        }


        public void ExportExcel(StiReport report, Stream stream, StiExcelExportSettings settings)
        {
            StiLogService.Write(this.GetType(), "Export report to Excel format");

            #region Read settings
            if (settings == null)
                throw new ArgumentNullException("The 'settings' argument cannot be equal in null.");

            var pageRange = settings.PageRange;
            this.useOnePageHeaderAndFooter = settings.UseOnePageHeaderAndFooter;
            this.dataExportMode = settings.DataExportMode;
            this.exportObjectFormatting = settings.ExportObjectFormatting;
            this.exportEachPageToSheet = settings.ExportEachPageToSheet;
            this.exportHorizontalPageBreaks = settings.ExportPageBreaks;
            this.imageResolution = settings.ImageResolution;
            this.imageQuality = settings.ImageQuality;
            #endregion

            bfHeader = new byte[4];
            bfData = new byte[0x40000];

            if (imageResolution < 10) imageResolution = 10;
            imageResolution = imageResolution / 100;

            maximumSheetHeight = StiOptions.Export.Excel.MaximumSheetHeight;
            if (maximumSheetHeight < 1) maximumSheetHeight = 1;
            if (maximumSheetHeight > 65535) maximumSheetHeight = 65535;

            if (this.dataExportMode != StiDataExportMode.AllBands)
            {
                this.useOnePageHeaderAndFooter = false;
            }
            else
            {
                this.exportObjectFormatting = true;
            }

            var currentCulture = Thread.CurrentThread.CurrentCulture;
            try
            {
                //StiExportUtils.DisableFontSmoothing();
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

                reportCulture = currentCulture;
                var culture = report.GetParsedCulture();
                if (!string.IsNullOrEmpty(culture))
                {
                    try
                    {
                        reportCulture = new CultureInfo(culture);
                    }
                    catch
                    {
                    }
                }

                var pages = pageRange.GetSelectedPages(report.RenderedPages);
                if (IsStopped) return;

                StatusString = StiLocalization.Get("Export", "ExportingCreatingDocument");

                PrepareData();
                MakeMainStream(report, pages);
                WriteToOLEContainer();

                memStMain.WriteTo(stream);
                memStMain.Close();
            }
            finally
            {
                StiExportUtils.EnableFontSmoothing(report);
                Thread.CurrentThread.CurrentCulture = currentCulture;
                if (matrix != null)
                {
                    matrix.Clear();
                    matrix = null;
                }
                xfList = null;
                colorList = null;
                fontList = null;
                sstList = null;
                sstHash = null;
                formatList = null;
                memst = null;
                memStMain = null;
                imageList = null;
                escher = null;
                pagesList = null;
                bookmarksList = null;
                hlinksList = null;
                boundsheetsOffsetsList = null;
                boundsheetsNamesList = null;

                imageCodec = null;

                //bfHeader = null;
                //bfData = null;

                if (report.RenderedPages.CacheMode) StiMatrix.GCCollect();
            }
        }
        #endregion
    }
}